using HWDNNSFCBase;
using MESInterface.Common;
using MESPubLab.SAP_RFC;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using MESStation.LogicObject;
using SqlSugar;

namespace MESInterface.DCN
{
    public class NetgearPtm : taskBase
    {
        public bool IsRuning = false;
        string dbstr = "",bustr = "";
        OleExec db = null;
        OleExec ldb = null;
        public override void init()
        {
            try
            {
                dbstr =ConfigGet("DB");
                bustr = ConfigGet("BU");
            }
            catch (Exception)
            {
            }  
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("start");
                using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this.dbstr, false))
                {
                    var weidojoblist = db.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER,R_TO_DETAIL>((rds, csk, css, cc,rtd) =>
                            rds.SKUNO == csk.SKUNO && csk.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID && rds.DN_NO==rtd.DN_NO)
                        .Where((rds, csk, css, cc, rtd) => rds.DN_FLAG == "5" && cc.CUSTOMER_NAME.ToUpper() == "NETGEAR")
                        .Select((rds, csk, css, cc, rtd) => new {rds,rtd}).ToList();
                    foreach (var item in weidojoblist)
                    {
                        var ctl = db.Queryable<R_NETGEAR_PTM_CTL>().Where(t => t.SHIPORDERID == item.rds.ID).ToList().FirstOrDefault();
                        #region 寫入狀態表
                        if (ctl==null)
                        {
                            ctl = new R_NETGEAR_PTM_CTL()
                            {
                                ID = MesDbBase.GetNewID(dbstr, bustr, "R_NETGEAR_PTM_CTL", false),
                                SHIPORDERID = item.rds.ID,
                                DN = item.rds.DN_NO,
                                TONO = item.rtd.TO_NO,
                                PO = item.rds.PO_NO,
                                PTMFILE = "",
                                ORGCODE = new Func<string>(() =>
                                {
                                    switch (item.rtd.DN_CUSTOMER.ToUpper().Trim())
                                    {
                                        case "BNI061US": return "N10";
                                        case "BNI061HK": return "N20";
                                        case "BNI061NL": return "N30";
                                        default: return "N10";
                                    }
                                })(),
                                SHIPTOPART = item.rtd.DN_CUSTOMER,
                                SHIPQTY = item.rds.QTY,
                                CQA = "0",
                                CONVERTED = "0",
                                SENT = "0",
                                EDITTIME = DateTime.Now,
                                EDITBY = "SYSTEM"
                            };
                            db.Insertable(ctl).ExecuteCommand();
                        }
                        #endregion

                        #region 緩存本筆訂單待處理的SN
                        var tempque = db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING>(
                                (rsd, rsp, rs, rp1, rp2) =>
                                    rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID && rs.VALID_FLAG=="1" &&
                                    rsd.ID == rs.ID)
                            .Where((rsd, rsp, rs, rp1, rp2) => rsd.SHIPORDERID == item.rds.ID)
                            .Select((rsd, rsp, rs, rp1, rp2) => new
                            {
                                rsd.SHIPORDERID,
                                PACKAGENO = item.rds.DN_LINE,
                                ORDERLINENO = item.rds.DN_LINE,
                                //PACKAGENO = (float.Parse(item.rds.DN_LINE) / 10).ToString(),
                                //ORDERLINENO = (float.Parse(item.rds.DN_LINE) / 10 + 0.1).ToString(),
                                rsd.SN,
                                SNID = rs.ID,
                                rsd.SHIPDATE,
                                item.rds.SKUNO,
                                rs.WORKORDERNO,
                                CUSTSSN = rs.START_TIME,
                                PACKNO = rp1.PACK_NO,
                                PARENTPACKNO = rp2.PACK_NO
                            });
                        #endregion

                        var tempqueobj = tempque.ToList();
                        var tempsnlist = tempqueobj.Select(s => s.SN);
                        var tempsnidlist = tempqueobj.Select(s => s.SNID);
                        var tempworkorderno = tempqueobj.Select(s => s.WORKORDERNO);
                        #region MAC 
                        var macobjlist = db.Queryable<WWN_DATASHARING>()
                            .Where(t => tempsnlist.Contains(t.VSSN) && t.MAC_BLOCK_SIZE != 0)
                            .Select(t => new { SN = t.VSSN, t.MAC }).ToList();
                        macobjlist.AddRange(db.Queryable<WWN_DATASHARING>()
                            .Where(t => tempsnlist.Contains(t.CSSN) && t.MAC_BLOCK_SIZE != 0).Select(t => new { SN = t.CSSN, t.MAC }).ToList());
                        #endregion

                        #region public
                        //机种
                        var cskuobj = db.Queryable<C_SKU>()
                            .Where(t => t.SKUNO == item.rds.SKUNO).ToList();
                        //PS S/N KP
                        var rsnkpobj = db.UnionAll(db.Queryable<R_SN_KP>().Where(t =>
                                tempsnidlist.Contains(t.R_SN_ID) && t.SCANTYPE == "PS S/N").Select(t =>
                                new R_SN_KP()
                                {
                                    SN = t.SN,
                                    VALUE = t.VALUE,
                                    PARTNO = t.PARTNO,
                                    SCANTYPE = t.SCANTYPE,
                                }),
                            db.Queryable<R_SN_KP, R_SN_KP>((rsk1, rsk2) => rsk1.VALUE == rsk2.SN).Where(
                                    (rsk1, rsk2) => tempsnidlist.Contains(rsk1.R_SN_ID) &&
                                        rsk2.SCANTYPE == "PS S/N")
                                .Select((rsk1, rsk2) => new R_SN_KP()
                                {
                                    SN = rsk1.SN,
                                    VALUE = rsk2.VALUE,
                                    PARTNO = rsk2.PARTNO,
                                    SCANTYPE = rsk2.SCANTYPE,
                                })
                        ).ToList();
                        //sku TA/FA
                        var rfunctioncontrolobj = db
                            .Queryable<R_F_CONTROL, R_F_CONTROL_EX
                            >((rfc, rfcx) => rfc.ID == rfcx.DETAIL_ID).Where((rfc, rfcx) =>
                                rfc.FUNCTIONNAME == "PTM_TA/FA" && rfc.CATEGORY == "PTM_TA/FA" &&
                                rfc.FUNCTIONTYPE == "NOSYSTEM" && rfc.VALUE == item.rds.SKUNO)
                            .Select((rfc, rfcx) => new { rfc, rfcx }).ToList();
                        //wo release TA/FA
                        var rptmtaconfigobj = db.Queryable<R_PTM_TACONFIG>()
                            .Where(t => tempworkorderno.Contains(t.WO)).ToList();

                        ///pbox
                        var rsnkpPbox = db
                            .Queryable<R_SN_KP, R_F_CONTROL>((rsk, rfc) =>
                                rsk.PARTNO == rfc.VALUE)
                            .Where((rsk, rfc) => tempsnlist.Contains(rsk.SN) && rfc.FUNCTIONNAME == "PTM_TA/FA" &&
                                rfc.CATEGORY == "PTM_TA/FA" &&
                                rfc.FUNCTIONTYPE == "NOSYSTEM" && rsk.SCANTYPE == "PPM S/N" &&
                                SqlFunc.ToUpper(rsk.STATION) == "P-BOX").Select((rsk, rfc) => new { rsk, rfc }).ToList();   
                        var rsnkpPboxsnid = rsnkpPbox.Select(s => s.rfc.ID);
                        ///pboxTa/Fa
                        var rsnkpPboxTaFa = db.Queryable<R_F_CONTROL_EX>()
                            .Where(t => rsnkpPboxsnid.Contains(t.DETAIL_ID)).ToList();  
                        #endregion

                        var res = db.Ado.UseTran(() =>
                        {
                            db.Deleteable<R_NETGEAR_PTM_DATA>().Where(t => t.SHIPORDERID == item.rds.ID)
                                .ExecuteCommand();
                            #region 有MAC的取值

                            if (macobjlist.Count > 0)
                            {
                                    var seqno = 1;
                                    foreach (var tempqueobjitem in tempqueobj)
                                    {
                                        foreach (var macitem in macobjlist.FindAll(t => t.SN == tempqueobjitem.SN))
                                        {
                                            var ptmitem = new R_NETGEAR_PTM_DATA()
                                            {
                                                ID = MesDbBase.GetNewID(dbstr, bustr, "R_NETGEAR_PTM_DATA", false),
                                                SHIPORDERID = tempqueobjitem.SHIPORDERID,
                                                //PACKAGENO =  Convert.ToDouble(tempqueobjitem.PACKAGENO),
                                                //ORDERLINENO = tempqueobjitem.ORDERLINENO,
                                                PACKAGENO = (float.Parse(tempqueobjitem.PACKAGENO) / 10),
                                                ORDERLINENO =
                                                    (float.Parse(tempqueobjitem.ORDERLINENO) / 10 + 0.1).ToString(),
                                                PALLET_ID = tempqueobjitem.PARENTPACKNO,
                                                MASTER_CARTON_ID = tempqueobjitem.PACKNO,
                                                ITEM_NUMBER = tempqueobjitem.SKUNO,
                                                SERIAL_NUMBER = tempqueobjitem.SN,
                                                TOP_SERIAL_NUMBER = tempqueobjitem.SN,
                                                MAC_ID = macitem.MAC ?? "",
                                                ASN_NUMBER = item.rds.DN_NO,
                                                INVOICE_NUMBER = item.rds.DN_NO,
                                                PACKING_SLIP_NUMBER = item.rds.DN_NO,
                                                CONTAINER_NUMBER = item.rds.DN_NO,
                                                PO_NUMBER = item.rds.PO_NO ?? "",
                                                PO_LINE_NUMBER = "",
                                                XFACTORY_DATE =
                                                    tempqueobjitem.SHIPDATE?.ToString("MM/dd/yyyy") ??
                                                    DateTime.Now.ToString("MM/dd/yyyy"),
                                                MANUFACTURER_NAME = "Foxconn",
                                                AS_DATE_OF_MANUFACTURE =
                                                    tempqueobjitem.CUSTSSN?.ToString("MM/dd/yyyy") ??
                                                    DateTime.Now.ToString("MM/dd/yyyy"),
                                                COUNTRY_OF_ORIGIN = "Viet Nam",
                                                ORG_CODE = ctl.ORGCODE,
                                                IMEI_NUMBER = "",
                                                MASTERLOCK_NUMBER = "",
                                                NETWORKLOCK_NUMBER = "",
                                                SERVICELOCK_NUMBER = "",
                                                FA_NUMBER_LEVEL_REV = "",
                                                ITEM_NUMBER_LEVEL_REV = "",
                                                TA = new Func<string>(() =>
                                                {
                                                    var taobj = rptmtaconfigobj.FindAll(t =>
                                                        t.WO == tempqueobjitem.WORKORDERNO);
                                                    if (taobj.Count == 0)
                                                    {
                                                        db.Insertable(new R_SERVICE_LOG()
                                                        {
                                                            ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                false),
                                                            FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                            CURRENTEDITTIME = DateTime.Now,
                                                            SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                            DATA1 = "MIS_WO_TA",
                                                            DATA2 = item.rds.ID,
                                                            DATA3 = tempqueobjitem.WORKORDERNO,
                                                            MAILFLAG = "N"
                                                        }).ExecuteCommand();
                                                        throw new Exception("MIS_WO_TA");
                                                    }

                                                    return taobj.FirstOrDefault().TA_NUMBER;
                                                })(),
                                                FA_NUMBER = new Func<string>(() =>
                                                {
                                                    var faobj = rfunctioncontrolobj.FindAll(t =>
                                                        t.rfc.EXTVAL == tempqueobjitem.SN.Substring(0, 4) &&
                                                        t.rfcx.NAME == "FA");
                                                    if (faobj.Count == 0)
                                                    {
                                                        db.Insertable(new R_SERVICE_LOG()
                                                        {
                                                            ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                false),
                                                            FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                            CURRENTEDITTIME = DateTime.Now,
                                                            SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                            DATA1 = "MIS_WO_FA",
                                                            DATA2 = item.rds.ID,
                                                            DATA3 = tempqueobjitem.WORKORDERNO,
                                                            MAILFLAG = "N"
                                                        }).ExecuteCommand();
                                                        throw new Exception("MIS_WO_FA");
                                                    }

                                                    return faobj.FirstOrDefault().rfcx.VALUE ?? "";
                                                })(),
                                                WEP_KEY = "",
                                                WIFI_ID = "",
                                                ACCESS_CODE = "",
                                                PRIMARY_SSID = "",
                                                WPA_KEY = "",
                                                MAC_ID_CABLE = "",
                                                MAC_ID_EMTA = "",
                                                HARDWARE_VERSION = "",
                                                FIRMWARE_VERSION = "",
                                                EAN_CODE = "",
                                                SOFTWARE_VERSION = "",
                                                SRM_PASSWORD = "",
                                                RF_MAC_ID = "",
                                                MACID_IN_MTA = "",
                                                MTA_MAN_ROUTER_MAC = "",
                                                MTADATA_MAC = "",
                                                ETHERNET_MAC = "",
                                                USB_MAC = "",
                                                PRIMARYSSID_PASSPHRASE = "",
                                                CMCI_MAC = "",
                                                LAN_MAC = "",
                                                WAN_MAC = "",
                                                DEVICE_MAC = "",
                                                WIRELESS_MAC = "",
                                                WIFI_MAC_SSID1 = "",
                                                SSID1 = "",
                                                SSID1_PASSPHRASE = "",
                                                WPA_PASSPHRASE = "",
                                                WPS_PIN_CODE = "",
                                                PPPOA_USERNAME = "",
                                                PPPOA_PASSPHRASE = "",
                                                TR069_UNIQUE_KEY_64_BIT = "",
                                                FON_KEY = "",
                                                PA_SN = "",
                                                PA_ITEM_NUMBER = "",
                                                LASTEDITBY = "SYSTEM",
                                                LASTEDITDT = DateTime.Now
                                            };
                                            ///P-box data
                                            var rsnkpPboxItem = rsnkpPbox.FindAll(t => t.rsk.SN == tempqueobjitem.SN);
                                            ///PS S/N kp data
                                            var rsnlist = rsnkpobj.FindAll(t => t.SN == tempqueobjitem.SN);
                                            if (rsnlist.Count > 0) //有PS S/N
                                            {
                                                foreach (var rsnitem in rsnlist)
                                                {
                                                    var ptemitemobj = ptmitem;
                                                    ptemitemobj.SEQNO = seqno++;
                                                    ptemitemobj.PA_SN = rsnitem.VALUE ?? "";
                                                    ptemitemobj.PA_ITEM_NUMBER = new Func<string>(() =>
                                                    {
                                                        if (rsnitem.PARTNO == null) return "";
                                                        if (rsnitem.PARTNO.StartsWith("B"))
                                                            return rsnitem.PARTNO.Substring(1);
                                                        return rsnitem.PARTNO;
                                                    })();
                                                    db.Insertable(ptemitemobj).ExecuteCommand();

                                                    ///PPM S/N
                                                    foreach (var pboxitem in rsnkpPboxItem)
                                                    {
                                                        ptemitemobj.SEQNO = seqno++;
                                                        ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                                        ptemitemobj.TA = new Func<string>(() =>
                                                        {
                                                            var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                                    t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "TA")
                                                                .ToList();
                                                            if (taobj.Count == 0)
                                                            {
                                                                db.Insertable(new R_SERVICE_LOG()
                                                                {
                                                                    ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                        "R_SERVICE_LOG", false),
                                                                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                    CURRENTEDITTIME = DateTime.Now,
                                                                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                    DATA1 = "MIS_CSN_TA",
                                                                    DATA2 = item.rds.ID,
                                                                    DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                    DATA4 = pboxitem.rsk.VALUE,
                                                                    MAILFLAG = "N"
                                                                }).ExecuteCommand();
                                                                throw new Exception("MIS_CSN_TA");
                                                            }

                                                            return taobj.FirstOrDefault().VALUE ?? "";
                                                        })();
                                                        ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                                        {
                                                            var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                                    t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "FA")
                                                                .ToList();
                                                            if (faobj.Count == 0)
                                                            {
                                                                db.Insertable(new R_SERVICE_LOG()
                                                                {
                                                                    ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                        "R_SERVICE_LOG", false),
                                                                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                    CURRENTEDITTIME = DateTime.Now,
                                                                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                    DATA1 = "MIS_CSN_FA",
                                                                    DATA2 = item.rds.ID,
                                                                    DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                    DATA4 = pboxitem.rsk.VALUE,
                                                                    MAILFLAG = "N"
                                                                }).ExecuteCommand();
                                                                throw new Exception("MIS_CSN_FA");
                                                            }

                                                            return faobj.FirstOrDefault().VALUE ?? "";
                                                        })();
                                                        ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                            "R_NETGEAR_PTM_DATA",
                                                            false);
                                                        db.Insertable(ptemitemobj).ExecuteCommand();
                                                    }
                                                }
                                            }
                                            else //無PS S/N
                                            {
                                                var ptemitemobj = ptmitem;
                                                ptemitemobj.SEQNO = seqno++;
                                                ptemitemobj.PA_SN = "";
                                                ptemitemobj.PA_ITEM_NUMBER = "";
                                                db.Insertable(ptemitemobj).ExecuteCommand();

                                                ///PPM S/N
                                                foreach (var pboxitem in rsnkpPboxItem)
                                                {
                                                    ptemitemobj.SEQNO = seqno++;
                                                    ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                                    ptemitemobj.TA = new Func<string>(() =>
                                                    {
                                                        var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                            t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "TA").ToList();
                                                        if (taobj.Count == 0)
                                                        {
                                                            db.Insertable(new R_SERVICE_LOG()
                                                            {
                                                                ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                    false),
                                                                FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                CURRENTEDITTIME = DateTime.Now,
                                                                SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                DATA1 = "MIS_CSN_TA",
                                                                DATA2 = item.rds.ID,
                                                                DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                DATA4 = pboxitem.rsk.VALUE,
                                                                MAILFLAG = "N"
                                                            }).ExecuteCommand();
                                                            throw new Exception("MIS_CSN_TA");
                                                        }

                                                        return taobj.FirstOrDefault().VALUE ?? "";
                                                    })();
                                                    ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                                    {
                                                        var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                            t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "FA").ToList();
                                                        if (faobj.Count == 0)
                                                        {
                                                            db.Insertable(new R_SERVICE_LOG()
                                                            {
                                                                ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                    false),
                                                                FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                CURRENTEDITTIME = DateTime.Now,
                                                                SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                DATA1 = "MIS_CSN_FA",
                                                                DATA2 = item.rds.ID,
                                                                DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                DATA4 = pboxitem.rsk.VALUE,
                                                                MAILFLAG = "N"
                                                            }).ExecuteCommand();
                                                            throw new Exception("MIS_CSN_FA");
                                                        }

                                                        return faobj.FirstOrDefault().VALUE ?? "";
                                                    })();
                                                    ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                        "R_NETGEAR_PTM_DATA",
                                                        false);
                                                    db.Insertable(ptemitemobj).ExecuteCommand();
                                                }
                                            }
                                        }
                                    }
                                
                            }

                            #endregion

                            #region 無MAC的取值

                            else
                            {
                                if ("APS250W-100NES,APS250W-100AJS,APM408F-10000S".IndexOf(item.rds.SKUNO) > -1)
                                {
                    
                                        var seqno = 1;
                                        foreach (var tempqueobjitem in tempqueobj)
                                        {
                                            var ptmitem = new R_NETGEAR_PTM_DATA()
                                            {
                                                SHIPORDERID = tempqueobjitem.SHIPORDERID,
                                                //PACKAGENO = Convert.ToDouble(tempqueobjitem.PACKAGENO),
                                                //ORDERLINENO = tempqueobjitem.ORDERLINENO,
                                                PACKAGENO = (float.Parse(tempqueobjitem.PACKAGENO) / 10),
                                                ORDERLINENO =
                                                    (float.Parse(tempqueobjitem.ORDERLINENO) / 10 + 0.1).ToString(),
                                                PALLET_ID = tempqueobjitem.PARENTPACKNO,
                                                MASTER_CARTON_ID = tempqueobjitem.PACKNO,
                                                ITEM_NUMBER = tempqueobjitem.SKUNO,
                                                SERIAL_NUMBER = tempqueobjitem.SN,
                                                TOP_SERIAL_NUMBER = tempqueobjitem.SN,
                                                MAC_ID = "",
                                                ASN_NUMBER = item.rds.DN_NO,
                                                INVOICE_NUMBER = item.rds.DN_NO,
                                                PACKING_SLIP_NUMBER = item.rds.DN_NO,
                                                CONTAINER_NUMBER = item.rds.DN_NO,
                                                PO_NUMBER = item.rds.PO_NO ?? "",
                                                PO_LINE_NUMBER = "",
                                                XFACTORY_DATE =
                                                    tempqueobjitem.SHIPDATE?.ToString("MM/dd/yyyy") ??
                                                    DateTime.Now.ToString("MM/dd/yyyy"),
                                                MANUFACTURER_NAME = "Foxconn",
                                                AS_DATE_OF_MANUFACTURE =
                                                    tempqueobjitem.CUSTSSN?.ToString("MM/dd/yyyy") ??
                                                    DateTime.Now.ToString("MM/dd/yyyy"),
                                                COUNTRY_OF_ORIGIN = "Viet Nam",
                                                ORG_CODE = ctl.ORGCODE,
                                                ITEM_NUMBER_LEVEL_REV = "",
                                                TA = new Func<string>(() =>
                                                {
                                                    var taobj = rptmtaconfigobj.FindAll(t =>
                                                        t.WO == tempqueobjitem.WORKORDERNO);
                                                    if (taobj.Count == 0)
                                                    {
                                                        db.Insertable(new R_SERVICE_LOG()
                                                        {
                                                            ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                false),
                                                            FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                            CURRENTEDITTIME = DateTime.Now,
                                                            SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                            DATA1 = "MIS_WO_TA",
                                                            DATA2 = item.rds.ID,
                                                            DATA3 = tempqueobjitem.WORKORDERNO,
                                                            MAILFLAG = "N"
                                                        }).ExecuteCommand();
                                                        throw new Exception("MIS_WO_TA");
                                                    }

                                                    return taobj.FirstOrDefault().TA_NUMBER;
                                                })(),
                                                FA_NUMBER = new Func<string>(() =>
                                                {
                                                    var faobj = rfunctioncontrolobj.FindAll(t =>
                                                        t.rfc.EXTVAL == tempqueobjitem.SN.Substring(0, 4) &&
                                                        t.rfcx.NAME == "FA");
                                                    if (faobj.Count == 0)
                                                    {
                                                        db.Insertable(new R_SERVICE_LOG()
                                                        {
                                                            ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                false),
                                                            FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                            CURRENTEDITTIME = DateTime.Now,
                                                            SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                            DATA1 = "MIS_WO_FA",
                                                            DATA2 = item.rds.ID,
                                                            DATA3 = tempqueobjitem.WORKORDERNO,
                                                            MAILFLAG = "N"
                                                        }).ExecuteCommand();
                                                        throw new Exception("MIS_WO_FA");
                                                    }

                                                    return faobj.FirstOrDefault().rfcx.VALUE ?? "";
                                                })(),
                                                WEP_KEY = "",
                                                WIFI_ID = "",
                                                ACCESS_CODE = "",
                                                PRIMARY_SSID = "",
                                                WPA_KEY = "",
                                                MAC_ID_CABLE = "",
                                                MAC_ID_EMTA = "",
                                                HARDWARE_VERSION = "",
                                                FIRMWARE_VERSION = "",
                                                EAN_CODE = "",
                                                SOFTWARE_VERSION = "",
                                                SRM_PASSWORD = "",
                                                RF_MAC_ID = "",
                                                MACID_IN_MTA = "",
                                                MTA_MAN_ROUTER_MAC = "",
                                                MTADATA_MAC = "",
                                                ETHERNET_MAC = "",
                                                USB_MAC = "",
                                                PRIMARYSSID_PASSPHRASE = "",
                                                CMCI_MAC = "",
                                                LAN_MAC = "",
                                                WAN_MAC = "",
                                                DEVICE_MAC = "",
                                                WIRELESS_MAC = "",
                                                WIFI_MAC_SSID1 = "",
                                                SSID1 = "",
                                                SSID1_PASSPHRASE = "",
                                                WPA_PASSPHRASE = "",
                                                WPS_PIN_CODE = "",
                                                PPPOA_USERNAME = "",
                                                PPPOA_PASSPHRASE = "",
                                                TR069_UNIQUE_KEY_64_BIT = "",
                                                FON_KEY = "",
                                                PA_SN = "",
                                                PA_ITEM_NUMBER = "",
                                                LASTEDITBY = "SYSTEM",
                                                LASTEDITDT = DateTime.Now
                                            };
                                            ///P-box data
                                            var rsnkpPboxItem = rsnkpPbox.FindAll(t => t.rsk.SN == tempqueobjitem.SN);
                                            ///PS S/N kp data
                                            var rsnlist = rsnkpobj.FindAll(t => t.SN == tempqueobjitem.SN);
                                            if (rsnlist.Count > 0) //有PS S/N
                                            {
                                                foreach (var rsnitem in rsnlist)
                                                {
                                                    var ptemitemobj = ptmitem;
                                                    ptemitemobj.SEQNO = seqno++;
                                                    ptemitemobj.PA_SN = rsnitem.VALUE ?? "";
                                                    ptemitemobj.PA_ITEM_NUMBER = new Func<string>(() =>
                                                    {
                                                        if (rsnitem.PARTNO == null) return "";
                                                        if (rsnitem.PARTNO.StartsWith("B"))
                                                            return rsnitem.PARTNO.Substring(1);
                                                        return rsnitem.PARTNO;
                                                    })();
                                                    ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                        "R_NETGEAR_PTM_DATA",
                                                        false);
                                                    db.Insertable(ptemitemobj).ExecuteCommand();

                                                    ///PPM S/N
                                                    foreach (var pboxitem in rsnkpPboxItem)
                                                    {
                                                        ptemitemobj.SEQNO = seqno++;
                                                        ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                                        ptemitemobj.TA = new Func<string>(() =>
                                                        {
                                                            var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                                    t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "TA")
                                                                .ToList();
                                                            if (taobj.Count == 0)
                                                            {
                                                                db.Insertable(new R_SERVICE_LOG()
                                                                {
                                                                    ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                        "R_SERVICE_LOG", false),
                                                                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                    CURRENTEDITTIME = DateTime.Now,
                                                                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                    DATA1 = "MIS_CSN_TA",
                                                                    DATA2 = item.rds.ID,
                                                                    DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                    DATA4 = pboxitem.rsk.VALUE,
                                                                    MAILFLAG = "N"
                                                                }).ExecuteCommand();
                                                                throw new Exception("MIS_CSN_TA");
                                                            }

                                                            return taobj.FirstOrDefault().VALUE ?? "";
                                                        })();
                                                        ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                                        {
                                                            var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                                    t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "FA")
                                                                .ToList();
                                                            if (faobj.Count == 0)
                                                            {
                                                                db.Insertable(new R_SERVICE_LOG()
                                                                {
                                                                    ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                        "R_SERVICE_LOG", false),
                                                                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                    CURRENTEDITTIME = DateTime.Now,
                                                                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                    DATA1 = "MIS_CSN_FA",
                                                                    DATA2 = item.rds.ID,
                                                                    DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                    DATA4 = pboxitem.rsk.VALUE,
                                                                    MAILFLAG = "N"
                                                                }).ExecuteCommand();
                                                                throw new Exception("MIS_CSN_FA");
                                                            }

                                                            return faobj.FirstOrDefault().VALUE ?? "";
                                                        })();
                                                        ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                            "R_NETGEAR_PTM_DATA",
                                                            false);
                                                        db.Insertable(ptemitemobj).ExecuteCommand();
                                                    }
                                                }
                                            }
                                            else //無PS S/N
                                            {
                                                var ptemitemobj = ptmitem;
                                                ptemitemobj.SEQNO = seqno++;
                                                ptemitemobj.PA_SN = "";
                                                ptemitemobj.PA_ITEM_NUMBER = "";
                                                ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr, "R_NETGEAR_PTM_DATA",
                                                    false);
                                                db.Insertable(ptemitemobj).ExecuteCommand();

                                                ///PPM S/N
                                                foreach (var pboxitem in rsnkpPboxItem)
                                                {
                                                    ptemitemobj.SEQNO = seqno++;
                                                    ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                                    ptemitemobj.TA = new Func<string>(() =>
                                                    {
                                                        var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                            t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "TA").ToList();
                                                        if (taobj.Count == 0)
                                                        {
                                                            db.Insertable(new R_SERVICE_LOG()
                                                            {
                                                                ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                    false),
                                                                FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                CURRENTEDITTIME = DateTime.Now,
                                                                SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                DATA1 = "MIS_CSN_TA",
                                                                DATA2 = item.rds.ID,
                                                                DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                DATA4 = pboxitem.rsk.VALUE,
                                                                MAILFLAG = "N"
                                                            }).ExecuteCommand();
                                                            throw new Exception("MIS_CSN_TA");
                                                        }

                                                        return taobj.FirstOrDefault().VALUE ?? "";
                                                    })();
                                                    ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                                    {
                                                        var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                            t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "FA").ToList();
                                                        if (faobj.Count == 0)
                                                        {
                                                            db.Insertable(new R_SERVICE_LOG()
                                                            {
                                                                ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                    false),
                                                                FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                CURRENTEDITTIME = DateTime.Now,
                                                                SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                DATA1 = "MIS_CSN_FA",
                                                                DATA2 = item.rds.ID,
                                                                DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                DATA4 = pboxitem.rsk.VALUE,
                                                                MAILFLAG = "N"
                                                            }).ExecuteCommand();
                                                            throw new Exception("MIS_CSN_FA");
                                                        }

                                                        return faobj.FirstOrDefault().VALUE ?? "";
                                                    })();
                                                    ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                        "R_NETGEAR_PTM_DATA",
                                                        false);
                                                    db.Insertable(ptemitemobj).ExecuteCommand();
                                                }
                                            }
                                        }
                                   
                                }
                                else
                                {
                     
                                        var seqno = 1;
                                        foreach (var tempqueobjitem in tempqueobj)
                                        {
                                            var ptmitem = new R_NETGEAR_PTM_DATA()
                                            {
                                                SHIPORDERID = tempqueobjitem.SHIPORDERID,
                                                //PACKAGENO = Convert.ToDouble(tempqueobjitem.PACKAGENO),
                                                //ORDERLINENO = tempqueobjitem.ORDERLINENO,
                                                PACKAGENO = (float.Parse(tempqueobjitem.PACKAGENO) / 10),
                                                ORDERLINENO =
                                                    (float.Parse(tempqueobjitem.ORDERLINENO) / 10 + 0.1).ToString(),
                                                PALLET_ID = tempqueobjitem.PARENTPACKNO,
                                                MASTER_CARTON_ID = tempqueobjitem.PACKNO,
                                                ITEM_NUMBER = tempqueobjitem.SKUNO,
                                                SERIAL_NUMBER = tempqueobjitem.SN,
                                                TOP_SERIAL_NUMBER = tempqueobjitem.SN,
                                                MAC_ID = "",
                                                ASN_NUMBER = item.rds.DN_NO,
                                                INVOICE_NUMBER = item.rds.DN_NO,
                                                PACKING_SLIP_NUMBER = item.rds.DN_NO,
                                                CONTAINER_NUMBER = item.rds.DN_NO,
                                                PO_NUMBER = item.rds.PO_NO ?? "",
                                                PO_LINE_NUMBER = "",
                                                XFACTORY_DATE =
                                                    tempqueobjitem.SHIPDATE?.ToString("MM/dd/yyyy") ??
                                                    DateTime.Now.ToString("MM/dd/yyyy"),
                                                MANUFACTURER_NAME = "Foxconn",
                                                AS_DATE_OF_MANUFACTURE =
                                                    tempqueobjitem.CUSTSSN?.ToString("MM/dd/yyyy") ??
                                                    DateTime.Now.ToString("MM/dd/yyyy"),
                                                COUNTRY_OF_ORIGIN = "Viet Nam",
                                                ORG_CODE = ctl.ORGCODE,
                                                ITEM_NUMBER_LEVEL_REV = "",
                                                TA = new Func<string>(() =>
                                                {
                                                    var taobj = rptmtaconfigobj.FindAll(t =>
                                                        t.WO == tempqueobjitem.WORKORDERNO);
                                                    if (taobj.Count == 0)
                                                    {
                                                        db.Insertable(new R_SERVICE_LOG()
                                                        {
                                                            ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                false),
                                                            FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                            CURRENTEDITTIME = DateTime.Now,
                                                            SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                            DATA1 = "MIS_WO_TA",
                                                            DATA2 = item.rds.ID,
                                                            DATA3 = tempqueobjitem.WORKORDERNO,
                                                            MAILFLAG = "N"
                                                        }).ExecuteCommand();
                                                        throw new Exception("MIS_WO_TA");
                                                    }

                                                    return taobj.FirstOrDefault().TA_NUMBER;
                                                })(),
                                                FA_NUMBER = new Func<string>(() =>
                                                {
                                                    var faobj = rfunctioncontrolobj.FindAll(t =>
                                                        t.rfc.EXTVAL == tempqueobjitem.SN.Substring(0, 4) &&
                                                        t.rfcx.NAME == "FA");
                                                    if (faobj.Count == 0)
                                                    {
                                                        db.Insertable(new R_SERVICE_LOG()
                                                        {
                                                            ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",
                                                                false),
                                                            FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                            CURRENTEDITTIME = DateTime.Now,
                                                            SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                            DATA1 = "MIS_WO_FA",
                                                            DATA2 = item.rds.ID,
                                                            DATA3 = tempqueobjitem.WORKORDERNO,
                                                            MAILFLAG = "N"
                                                        }).ExecuteCommand();
                                                        throw new Exception("MIS_WO_FA");
                                                    }

                                                    return faobj.FirstOrDefault().rfcx.VALUE ?? "";
                                                })(),
                                                WEP_KEY = "",
                                                WIFI_ID = "",
                                                ACCESS_CODE = "",
                                                PRIMARY_SSID = "",
                                                WPA_KEY = "",
                                                MAC_ID_CABLE = "",
                                                MAC_ID_EMTA = "",
                                                HARDWARE_VERSION = "",
                                                FIRMWARE_VERSION = "",
                                                EAN_CODE = "",
                                                SOFTWARE_VERSION = "",
                                                SRM_PASSWORD = "",
                                                RF_MAC_ID = "",
                                                MACID_IN_MTA = "",
                                                MTA_MAN_ROUTER_MAC = "",
                                                MTADATA_MAC = "",
                                                ETHERNET_MAC = "",
                                                USB_MAC = "",
                                                PRIMARYSSID_PASSPHRASE = "",
                                                CMCI_MAC = "",
                                                LAN_MAC = "",
                                                WAN_MAC = "",
                                                DEVICE_MAC = "",
                                                WIRELESS_MAC = "",
                                                WIFI_MAC_SSID1 = "",
                                                SSID1 = "",
                                                SSID1_PASSPHRASE = "",
                                                WPA_PASSPHRASE = "",
                                                WPS_PIN_CODE = "",
                                                PPPOA_USERNAME = "",
                                                PPPOA_PASSPHRASE = "",
                                                TR069_UNIQUE_KEY_64_BIT = "",
                                                FON_KEY = "",
                                                PA_SN = "",
                                                PA_ITEM_NUMBER = "",
                                                LASTEDITBY = "SYSTEM",
                                                LASTEDITDT = DateTime.Now
                                            };
                                            ///P-box data
                                            var rsnkpPboxItem = rsnkpPbox.FindAll(t => t.rsk.SN == tempqueobjitem.SN);
                                            ///PS S/N kp data
                                            var rsnlist = rsnkpobj.FindAll(t => t.SN == tempqueobjitem.SN);

                                            if (rsnlist.Count > 0) //有PS S/N
                                            {
                                                foreach (var rsnitem in rsnlist)
                                                {
                                                    var ptemitemobj = ptmitem;
                                                    ptemitemobj.MAC_ID = "";
                                                    ptemitemobj.SEQNO = seqno++;
                                                    ptemitemobj.PA_SN = rsnitem.VALUE ?? "";
                                                    ptemitemobj.PA_ITEM_NUMBER = new Func<string>(() =>
                                                    {
                                                        if (rsnitem.PARTNO == null) return "";
                                                        if (rsnitem.PARTNO.StartsWith("B"))
                                                            return rsnitem.PARTNO.Substring(1);
                                                        return rsnitem.PARTNO;
                                                    })();
                                                    ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                        "R_NETGEAR_PTM_DATA",
                                                        false);
                                                    db.Insertable(ptemitemobj).ExecuteCommand();

                                                    foreach (var macitem in macobjlist.FindAll(t =>
                                                        t.SN == tempqueobjitem.SN))
                                                    {
                                                        ///PPM S/N
                                                        foreach (var pboxitem in rsnkpPboxItem)
                                                        {
                                                            ptemitemobj.SEQNO = seqno++;
                                                            ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                                            ptemitemobj.MAC_ID = macitem.MAC;
                                                            ptemitemobj.TA = new Func<string>(() =>
                                                            {
                                                                var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                                        t.DETAIL_ID == pboxitem.rfc.ID &&
                                                                        t.NAME == "TA")
                                                                    .ToList();
                                                                if (taobj.Count == 0)
                                                                {
                                                                    db.Insertable(new R_SERVICE_LOG()
                                                                    {
                                                                        ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                            "R_SERVICE_LOG", false),
                                                                        FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                        CURRENTEDITTIME = DateTime.Now,
                                                                        SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                        DATA1 = "MIS_CSN_TA",
                                                                        DATA2 = item.rds.ID,
                                                                        DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                        DATA4 = pboxitem.rsk.VALUE,
                                                                        MAILFLAG = "N"
                                                                    }).ExecuteCommand();
                                                                    throw new Exception("MIS_CSN_TA");
                                                                }

                                                                return taobj.FirstOrDefault().VALUE ?? "";
                                                            })();
                                                            ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                                            {
                                                                var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                                        t.DETAIL_ID == pboxitem.rfc.ID &&
                                                                        t.NAME == "FA")
                                                                    .ToList();
                                                                if (faobj.Count == 0)
                                                                {
                                                                    db.Insertable(new R_SERVICE_LOG()
                                                                    {
                                                                        ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                            "R_SERVICE_LOG", false),
                                                                        FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                        CURRENTEDITTIME = DateTime.Now,
                                                                        SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                        DATA1 = "MIS_CSN_FA",
                                                                        DATA2 = item.rds.ID,
                                                                        DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                        DATA4 = pboxitem.rsk.VALUE,
                                                                        MAILFLAG = "N"
                                                                    }).ExecuteCommand();
                                                                    throw new Exception("MIS_CSN_FA");
                                                                }

                                                                return faobj.FirstOrDefault().VALUE ?? "";
                                                            })();
                                                            ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                "R_NETGEAR_PTM_DATA",
                                                                false);
                                                            db.Insertable(ptemitemobj).ExecuteCommand();
                                                        }
                                                    }
                                                }
                                            }
                                            else //無PS S/N
                                            {
                                                var ptemitemobj = ptmitem;
                                                ptemitemobj.SEQNO = seqno++;
                                                ptemitemobj.PA_SN = "";
                                                ptemitemobj.PA_ITEM_NUMBER = "";
                                                ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr, "R_NETGEAR_PTM_DATA",
                                                    false);
                                                db.Insertable(ptemitemobj).ExecuteCommand();

                                                foreach (var macitem in macobjlist.FindAll(t =>
                                                    t.SN == tempqueobjitem.SN))
                                                {
                                                    ///PPM S/N
                                                    foreach (var pboxitem in rsnkpPboxItem)
                                                    {
                                                        ptemitemobj.SEQNO = seqno++;
                                                        ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                                        ptemitemobj.TA = new Func<string>(() =>
                                                        {
                                                            var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                                    t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "TA")
                                                                .ToList();
                                                            if (taobj.Count == 0)
                                                            {
                                                                db.Insertable(new R_SERVICE_LOG()
                                                                {
                                                                    ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                        "R_SERVICE_LOG", false),
                                                                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                    CURRENTEDITTIME = DateTime.Now,
                                                                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                    DATA1 = "MIS_CSN_TA",
                                                                    DATA2 = item.rds.ID,
                                                                    DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                    DATA4 = pboxitem.rsk.VALUE,
                                                                    MAILFLAG = "N"
                                                                }).ExecuteCommand();
                                                                throw new Exception("MIS_CSN_TA");
                                                            }

                                                            return taobj.FirstOrDefault().VALUE ?? "";
                                                        })();
                                                        ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                                        {
                                                            var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                                    t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == "FA")
                                                                .ToList();
                                                            if (faobj.Count == 0)
                                                            {
                                                                db.Insertable(new R_SERVICE_LOG()
                                                                {
                                                                    ID = MesDbBase.GetNewID(dbstr, bustr,
                                                                        "R_SERVICE_LOG", false),
                                                                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                                                                    CURRENTEDITTIME = DateTime.Now,
                                                                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                                                    DATA1 = "MIS_CSN_FA",
                                                                    DATA2 = item.rds.ID,
                                                                    DATA3 = ptemitemobj.TOP_SERIAL_NUMBER,
                                                                    DATA4 = pboxitem.rsk.VALUE,
                                                                    MAILFLAG = "N"
                                                                }).ExecuteCommand();
                                                                throw new Exception("MIS_CSN_FA");
                                                            }

                                                            return faobj.FirstOrDefault().VALUE ?? "";
                                                        })();
                                                        ptemitemobj.ID = MesDbBase.GetNewID(dbstr, bustr,
                                                            "R_NETGEAR_PTM_DATA",
                                                            false);
                                                        db.Insertable(ptemitemobj).ExecuteCommand();
                                                    }
                                                }
                                            }

                                        }
                                  
                                }
                            }

                            #endregion
                        });
                        #region 更新狀態

                        if (res.IsSuccess)
                        {
                            db.Updateable<R_DN_STATUS>().UpdateColumns(t =>new R_DN_STATUS() { DN_FLAG = "1" } )
                                .Where(t => t.ID == item.rds.ID).ExecuteCommand();
                        }
                        else
                        {
                                db.Insertable(new R_SERVICE_LOG()
                                {
                                    ID = MesDbBase.GetNewID(dbstr, bustr, "R_SERVICE_LOG",false),
                                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                                    CURRENTEDITTIME = DateTime.Now,
                                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                                    DATA1 = "TolErr",
                                    DATA2 = item.rds.ID,
                                    DATA8 = res.ErrorMessage,
                                    MAILFLAG = "N"
                                }).ExecuteCommand();
                        }
                        #endregion
                    }
                }
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsRuning = false;
            }
        }

    }
}
