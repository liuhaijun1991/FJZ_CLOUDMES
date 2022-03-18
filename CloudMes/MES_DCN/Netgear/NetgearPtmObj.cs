using MESDataObject;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.ModuleHelp;
using MESPubLab.Common;
using MESPubLab.MesBase;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESPubLab.MesException;
using static MESDataObject.Common.EnumExtensions;
using Renci.SshNet;

namespace MES_DCN.Broadcom
{
    public class NetgearPtmObj
    {
        #region CONST
        private string _dbstr, _bustr, _filepath, _filebackpath;
        private OleExecPool _dbExecPool = null;
        static string PTM_FILE_PREFIX = "PTM_FOXCONN";
        static string PTM_FILE_EXTEND = ".txt";
        static string PTM_FILE_CONTENTS_SPLIT = "\t";
        string PTM_CONFIG_FLAG = "PTM_TA/FA";
        static string PTM_MANUFACTURER_NAME = "Foxconn";
        static string PTM_COO = "Viet Nam";
        static string PTM_CATEGORY_SKUNO = "APS250W-100NES,APS250W-100AJS,APM408F-10000S";
        static string CONST_NOT_FILE_COL = "SEQNO";
        static string PTM_SYSTEM = "SYSTEM";
        public static string PTM_FLAG = "PTM";
        //static string PTM_FUNCTION_CONTROL_CATEGORY = PTM_CONFIG_FLAG;
        #region cust sftp
        string CONST_SFTPHost = "208.185.37.152";
        string CONST_SFTPPort = "22";
        string CONST_SFTPLogin = "ptm_foxconn_dcn";
        string CONST_SFTPPassword = "netgear123";
        string CONST_SFTPPath = "/home/ptm_foxconn_dcn";
        private string CONST_SFTP_KEY = $@"PTM_FOXCONN_NETGEAR";
        #endregion
        #region proxy
        string CONST_ProxyHost = "10.228.10.14";
        int CONST_ProxyPort = 3128;
        string CONST_ProxyLogin = "";
        string CONST_ProxyPassword = "";
        #endregion
        public MESPubLab.MESStation.LogicObject.User LoginUser;
        /// <summary>
        /// P-BOX
        /// </summary>
        string PTM_STATION = "P-BOX";
        enum Ptm_Err
        {
            /// <summary>
            /// [EnumName("MIS_WO_FA")]
            /// [EnumValue("MIS_WO_FA")]
            /// </summary>
            [EnumName("MIS_WO_FA")]
            [EnumValue("MIS_WO_FA")]
            MIS_WO_FA,
            /// <summary>
            /// [EnumName("MIS_WO_TA")]
            /// [EnumValue("MIS_WO_TA")]
            /// </summary>
            [EnumName("MIS_WO_TA")]
            [EnumValue("MIS_WO_TA")]
            MIS_WO_TA,
            /// <summary>
            /// [EnumName("MIS_CSN_TA")]
            /// [EnumValue("MIS_CSN_TA")]
            /// </summary>
            [EnumName("MIS_CSN_TA")]
            [EnumValue("MIS_CSN_TA")]
            MIS_CSN_TA,
            /// <summary>
            /// [EnumName("MIS_CSN_FA")]
            /// [EnumValue("MIS_CSN_FA")]
            /// </summary>
            [EnumName("MIS_CSN_FA")]
            [EnumValue("MIS_CSN_FA")]
            MIS_CSN_FA,
            /// <summary>
            /// [EnumName("GENERATE_FILE")]
            /// [EnumValue("GENERATE_FILE")]
            /// </summary>
            [EnumName("GENERATE_FILE")]
            [EnumValue("GENERATE_FILE")]
            GENERATE_FILE,
            /// <summary>
            /// [EnumName("SENDMDSDATA")]
            /// [EnumValue("SENDMDSDATA")]
            /// </summary>
            [EnumName("SENDMDSDATA")]
            [EnumValue("SENDMDSDATA")]
            SENDMDSDATA,
            /// <summary>
            /// [EnumName("NOT_EXIST_FILE")]
            /// [EnumValue("NOT_EXIST_FILE")]
            /// </summary>
            [EnumName("NOT_EXIST_FILE")]
            [EnumValue("NOT_EXIST_FILE")]
            NOT_EXIST_FILE
        }
        enum Ptm_Ta_Fa
        {
            [EnumName("TA")]
            [EnumValue("TA")]
            TA,
            [EnumName("FA")]
            [EnumValue("FA")]
            FA
        }
        enum Ptm_Dn_Customer
        {
            [EnumName("BNI061US")]
            [EnumValue("N10")]
            BNI061US,
            [EnumName("BNI061HK")]
            [EnumValue("N20")]
            BNI061HK,
            [EnumName("BNI061NL")]
            [EnumValue("N30")]
            BNI061NL,
            [EnumName("OTHER")]
            [EnumValue("N10")]
            OTHER
        }
        #endregion
        public NetgearPtmObj(string bustr, OleExecPool dbExecPool = null, string filepath = null, string dbstr = null)
        {
            _dbstr = dbstr;
            _bustr = bustr;
            _filepath = filepath;
            _dbExecPool = dbExecPool;
        }
        public void Build()
        {
            this.BuildData();
            this.GanarationFile();
            //this.SendPtmData();
        }
        /// <summary>
        /// create ptm data in db
        /// this function not use tran,pls use tran at parent
        /// </summary>
        /// <param name="item"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public FuncExecRes BuildDataByDnObj(R_DN_STATUS item, SqlSugarClient db)
        {
            var funcExecres = new FuncExecRes();
            try
            {
                var toobj = db.Queryable<R_TO_DETAIL>().Where(t => t.DN_NO == item.DN_NO).ToList().FirstOrDefault();
                var ctl = db.Queryable<R_NETGEAR_PTM_CTL>().Where(t => t.SHIPORDERID == item.ID).ToList()
                    .FirstOrDefault();
                //Nhập biểu trạng thái
                #region 寫入狀態表

                if (ctl == null)
                {
                    ctl = new R_NETGEAR_PTM_CTL()
                    {
                        ID = MesDbBase.GetNewID<R_NETGEAR_PTM_CTL>(db, _bustr),
                        SHIPORDERID = item.ID,
                        DN = item.DN_NO,
                        TONO = toobj.TO_NO,
                        PO = item.PO_NO,
                        PTMFILE = string.Empty,
                        ORGCODE = new Func<string>(() =>
                        {
                            if (toobj.DN_CUSTOMER.ToUpper().Trim().Equals(Ptm_Dn_Customer.BNI061US.ExtName()))
                                return Ptm_Dn_Customer.BNI061US.Ext<EnumValueAttribute>().Description;
                            if (toobj.DN_CUSTOMER.ToUpper().Trim().Equals(Ptm_Dn_Customer.BNI061NL.ExtName()))
                                return Ptm_Dn_Customer.BNI061NL.Ext<EnumValueAttribute>().Description;
                            if (toobj.DN_CUSTOMER.ToUpper().Trim().Equals(Ptm_Dn_Customer.BNI061HK.ExtName()))
                                return Ptm_Dn_Customer.BNI061HK.Ext<EnumValueAttribute>().Description;
                            return Ptm_Dn_Customer.OTHER.Ext<EnumValueAttribute>().Description;
                        })(),
                        SHIPTOPART = toobj.DN_CUSTOMER,
                        SHIPQTY = item.QTY,
                        CQA = ENUM_R_NETGEAR_PTM_CTL.WAIT_CQA.Ext<EnumValueAttribute>().Description,
                        CONVERTED = ENUM_R_NETGEAR_PTM_CTL.WAIT_CONVER.Ext<EnumValueAttribute>().Description,
                        SENT = ENUM_R_NETGEAR_PTM_CTL.WAIT_SEND.Ext<EnumValueAttribute>().Description,
                        EDITBY = PTM_SYSTEM,
                        EDITTIME = DateTime.Now
                    };
                    db.Insertable(ctl).ExecuteCommand();
                }

                #endregion
                //SN chờ xử lý
                #region 緩存本筆訂單待處理的SN

                var tempque = db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING>(
                        (rsd, rsp, rs, rp1, rp2) =>
                            rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID &&
                            rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsd.ID == rs.ID)
                    .Where((rsd, rsp, rs, rp1, rp2) => rsd.SHIPORDERID == item.ID)
                    .Select((rsd, rsp, rs, rp1, rp2) => new
                    {
                        rsd.SHIPORDERID,
                        PACKAGENO = item.DN_LINE,
                        ORDERLINENO = item.DN_LINE,
                        rsd.SN,
                        SNID = rs.ID,
                        rsd.SHIPDATE,
                        item.SKUNO,
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

                //var macobjlist = db.Queryable<WWN_DATASHARING>()
                //    .Where(t => tempsnlist.Contains(t.VSSN) && t.MAC_BLOCK_SIZE != 0)
                //    .Select(t => new {SN = t.VSSN, t.MAC}).ToList();
                //macobjlist.AddRange(db.Queryable<WWN_DATASHARING>()
                //    .Where(t => tempsnlist.Contains(t.CSSN) && t.MAC_BLOCK_SIZE != 0)
                //    .Select(t => new {SN = t.CSSN, t.MAC})
                //    .ToList());
                //如果數量大於1000則Contains方法就會報錯(Oracle默認不能超1000)，改用下面寫法 Edit By ZHB 2020年10月3日16:48:37
                var macobjlist = db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING, WWN_DATASHARING>(
                        (rsd, rsp, rs, rp1, rp2, wwn) =>
                            rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID && rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsd.ID == rs.ID)
                    .Where((rsd, rsp, rs, rp1, rp2, wwn) => rsd.SHIPORDERID == item.ID && rs.SN == wwn.VSSN && wwn.MAC_BLOCK_SIZE != 0)
                    .Select((rsd, rsp, rs, rp1, rp2, wwn) => new { SN = wwn.VSSN, wwn.MAC }).ToList();
                macobjlist.AddRange(db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING, WWN_DATASHARING>(
                        (rsd, rsp, rs, rp1, rp2, wwn) =>
                            rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID && rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsd.ID == rs.ID)
                    .Where((rsd, rsp, rs, rp1, rp2, wwn) => rsd.SHIPORDERID == item.ID && rs.SN == wwn.CSSN && wwn.MAC_BLOCK_SIZE != 0)
                    .Select((rsd, rsp, rs, rp1, rp2, wwn) => new { SN = wwn.CSSN, wwn.MAC }).ToList());
                #endregion

                #region public

                //机种
                var cskuobj = db.Queryable<C_SKU>()
                    .Where(t => t.SKUNO == item.SKUNO).ToList();
                //PS S/N KP
                //var rsnkpobj = db.UnionAll(db.Queryable<R_SN_KP>().Where(t => t.VALID_FLAG == 1 &&
                //                                                              tempsnidlist.Contains(t.R_SN_ID) &&
                //                                                              DcnKeyPartScantype.PS_SN.ExtName()
                //                                                                  .Equals(t.SCANTYPE.Trim())).Select(
                //        t =>
                //            new R_SN_KP()
                //            {
                //                SN = t.SN,
                //                VALUE = t.VALUE,
                //                PARTNO = t.PARTNO,
                //                SCANTYPE = t.SCANTYPE,
                //            }),
                //    db.Queryable<R_SN_KP, R_SN_KP>((rsk1, rsk2) => rsk1.VALUE == rsk2.SN).Where(
                //            (rsk1, rsk2) => rsk1.VALID_FLAG == 1 && rsk2.VALID_FLAG == 1 &&
                //                            tempsnidlist.Contains(rsk1.R_SN_ID) &&
                //                            DcnKeyPartScantype.PS_SN.ExtName().Equals(rsk2.SCANTYPE.Trim()))
                //        .Select((rsk1, rsk2) => new R_SN_KP()
                //        {
                //            SN = rsk1.SN,
                //            VALUE = rsk2.VALUE,
                //            PARTNO = rsk2.PARTNO,
                //            SCANTYPE = rsk2.SCANTYPE,
                //        })
                //).ToList();
                //如果數量大於1000則Contains方法就會報錯(Oracle默認不能超1000)，改用下面寫法 Edit By ZHB 2020年10月3日16:48:37
                //List<R_SN_KP> rsnkpobj = null;
                var rsnkpobj = db.UnionAll(db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING, R_SN_KP>(
                        (rsd, rsp, rs, rp1, rp2, rsk) =>
                            rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID && rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsd.ID == rs.ID)
                    .Where((rsd, rsp, rs, rp1, rp2, rsk) => rsd.SHIPORDERID == item.ID && rs.ID == rsk.R_SN_ID && rsk.VALID_FLAG == 1 && (rsk.VALUE.Length >= 18 || rsk.VALUE == null) && DcnKeyPartScantype.PS_SN.ExtName().Equals(rsk.SCANTYPE.Trim())).Select((rsd, rsp, rs, rp1, rp2, rsk) =>
                     new R_SN_KP { SN = rsk.SN, VALUE = rsk.VALUE, PARTNO = rsk.PARTNO, SCANTYPE = rsk.SCANTYPE }),
                    db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING, R_SN_KP, R_SN_KP>(
                        (rsd, rsp, rs, rp1, rp2, rsk, rsk2) =>
                            rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID && rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsd.ID == rs.ID && rsk.VALUE == rsk2.SN)
                    .Where((rsd, rsp, rs, rp1, rp2, rsk, rsk2) => rsd.SHIPORDERID == item.ID && rs.ID == rsk.R_SN_ID && rsk.VALID_FLAG == 1 && rsk2.VALID_FLAG == 1 && (rsk2.VALUE.Length >= 18 || rsk2.VALUE == null) && DcnKeyPartScantype.PS_SN.ExtName().Equals(rsk2.SCANTYPE.Trim())).Select((rsd, rsp, rs, rp1, rp2, rsk, rsk2) =>
                        new R_SN_KP { SN = rsk.SN, VALUE = rsk2.VALUE, PARTNO = rsk2.PARTNO, SCANTYPE = rsk2.SCANTYPE })
                    ).Distinct().ToList();

                //sku TA/FA
                var rfunctioncontrolobj = db
                    .Queryable<R_F_CONTROL, R_F_CONTROL_EX
                    >((rfc, rfcx) => rfc.ID == rfcx.DETAIL_ID).Where((rfc, rfcx) =>
                        rfc.FUNCTIONNAME == PTM_CONFIG_FLAG && rfc.CATEGORY == PTM_CONFIG_FLAG &&
                        rfc.FUNCTIONTYPE ==
                        ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.Ext<EnumValueAttribute>().Description &&
                        rfc.VALUE == item.SKUNO && rfc.CONTROLFLAG == "Y")
                    .Select((rfc, rfcx) => new { rfc, rfcx }).ToList();
                //wo release TA/FA
                //var rptmtaconfigobj = db.Queryable<R_PTM_TACONFIG>()
                //    .Where(t => tempworkorderno.Contains(t.WO)).ToList();
                //如果數量大於1000則Contains方法就會報錯(Oracle默認不能超1000)，改用下面寫法 Edit By ZHB 2020年10月3日16:48:37
                var rptmtaconfigobj = db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING, R_PTM_TACONFIG>(
                        (rsd, rsp, rs, rp1, rp2, rpt) =>
                            rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID && rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsd.ID == rs.ID)
                    .Where((rsd, rsp, rs, rp1, rp2, rpt) => rsd.SHIPORDERID == item.ID && rs.WORKORDERNO == rpt.WO).Select((rsd, rsp, rs, rp1, rp2, rpt) => rpt).ToList();

                ///pbox
                //var rsnkpPbox = db
                //    .Queryable<R_SN_KP, R_F_CONTROL>((rsk, rfc) =>
                //        rsk.PARTNO == rfc.VALUE)
                //    .Where((rsk, rfc) => tempsnlist.Contains(rsk.SN) && rfc.FUNCTIONNAME == PTM_CONFIG_FLAG &&
                //                         rfc.CATEGORY == PTM_CONFIG_FLAG &&
                //                         rfc.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM
                //                             .Ext<EnumValueAttribute>().Description &&
                //                         rsk.SCANTYPE == DcnKeyPartScantype.PPM_SN.ExtName() &&
                //                         SqlFunc.ToUpper(rsk.STATION) == PTM_STATION)
                //    .Select((rsk, rfc) => new {rsk, rfc})
                //    .ToList();
                //如果數量大於1000則Contains方法就會報錯(Oracle默認不能超1000)，改用下面寫法 Edit By ZHB 2020年10月3日16:48:37
                var rsnkpPbox = db.Queryable<R_SHIP_DETAIL, R_SN_PACKING, R_SN, R_PACKING, R_PACKING, R_SN_KP, R_F_CONTROL>(
                        (rsd, rsp, rs, rp1, rp2, rsk, rfc) =>
                            rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID && rs.VALID_FLAG == ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumValueAttribute>().Description &&
                            rsd.ID == rs.ID && rsk.PARTNO == rfc.VALUE)
                    .Where((rsd, rsp, rs, rp1, rp2, rsk, rfc) => rsd.SHIPORDERID == item.ID && rs.SN == rsk.SN && rfc.FUNCTIONNAME == PTM_CONFIG_FLAG && rfc.CATEGORY == PTM_CONFIG_FLAG &&
                    rfc.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.Ext<EnumValueAttribute>().Description && rsk.SCANTYPE == DcnKeyPartScantype.PPM_SN.ExtName() &&
                    SqlFunc.ToUpper(rsk.STATION) == PTM_STATION).Select((rsd, rsp, rs, rp1, rp2, rsk, rfc) => new { rsk, rfc }).ToList();

                var rsnkpPboxsnid = rsnkpPbox.Select(s => s.rfc.ID);
                ///pboxTa/Fa
                var rsnkpPboxTaFa = db.Queryable<R_F_CONTROL_EX>()
                    .Where(t => rsnkpPboxsnid.Contains(t.DETAIL_ID)).ToList();

                #endregion

                db.Deleteable<R_NETGEAR_PTM_DATA>().Where(t => t.SHIPORDERID == item.ID)
                    .ExecuteCommand();
                //lấy thông tin MAC
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
                                ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr),
                                SHIPORDERID = tempqueobjitem.SHIPORDERID,
                                PACKAGENO = (float.Parse(tempqueobjitem.PACKAGENO) / 10),
                                ORDERLINENO =
                                    (float.Parse(tempqueobjitem.ORDERLINENO) / 10 + 0.1).ToString(),
                                PALLET_ID = tempqueobjitem.PARENTPACKNO,
                                MASTER_CARTON_ID = tempqueobjitem.PACKNO,
                                ITEM_NUMBER = tempqueobjitem.SKUNO,
                                SERIAL_NUMBER = tempqueobjitem.SN,
                                TOP_SERIAL_NUMBER = tempqueobjitem.SN,
                                MAC_ID = macitem.MAC ?? string.Empty,
                                ASN_NUMBER = item.DN_NO,
                                INVOICE_NUMBER = item.DN_NO,
                                PACKING_SLIP_NUMBER = item.DN_NO,
                                CONTAINER_NUMBER = item.DN_NO,
                                PO_NUMBER = item.PO_NO ?? string.Empty,
                                PO_LINE_NUMBER = string.Empty,
                                XFACTORY_DATE =
                                    tempqueobjitem.SHIPDATE?.ToString(MES_CONST_DATETIME_FORMAT.MDY_B
                                        .Ext<EnumValueAttribute>().Description) ??
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.MDY_B.Ext<EnumValueAttribute>()
                                        .Description),
                                MANUFACTURER_NAME = PTM_MANUFACTURER_NAME,
                                AS_DATE_OF_MANUFACTURE =
                                    tempqueobjitem.CUSTSSN?.ToString(MES_CONST_DATETIME_FORMAT.MDY_B
                                        .Ext<EnumValueAttribute>().Description) ??
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.MDY_B.Ext<EnumValueAttribute>()
                                        .Description),
                                COUNTRY_OF_ORIGIN = PTM_COO,
                                ORG_CODE = ctl.ORGCODE,
                                IMEI_NUMBER = string.Empty,
                                MASTERLOCK_NUMBER = string.Empty,
                                NETWORKLOCK_NUMBER = string.Empty,
                                SERVICELOCK_NUMBER = string.Empty,
                                FA_NUMBER_LEVEL_REV = string.Empty,
                                ITEM_NUMBER_LEVEL_REV = string.Empty,
                                TA = new Func<string>(() =>
                                {
                                    var taobj = rptmtaconfigobj.FindAll(t =>
                                        t.WO == tempqueobjitem.WORKORDERNO);
                                    if (taobj.Count == 0)
                                    {
                                        ptmLog(db, Ptm_Err.MIS_WO_TA.ExtValue(), item.ID,
                                            tempqueobjitem.WORKORDERNO);
                                        throw new MesException(Ptm_Err.MIS_WO_TA.Ext<EnumValueAttribute>().Description);
                                    }

                                    return taobj.FirstOrDefault().TA_NUMBER;
                                })(),
                                FA_NUMBER = new Func<string>(() =>
                                {
                                    var faobj = rfunctioncontrolobj.FindAll(t =>
                                        t.rfc.EXTVAL == tempqueobjitem.SN.Substring(0, 4) &&
                                        t.rfcx.NAME == Ptm_Ta_Fa.FA.ExtName());
                                    if (faobj.Count == 0)
                                    {
                                        ptmLog(db, Ptm_Err.MIS_WO_FA.ExtValue(), item.ID,
                                            tempqueobjitem.WORKORDERNO);
                                        throw new MesException(Ptm_Err.MIS_WO_FA.Ext<EnumValueAttribute>().Description);
                                    }

                                    return faobj.FirstOrDefault().rfcx.VALUE ?? string.Empty;
                                })(),
                                WEP_KEY = string.Empty,
                                WIFI_ID = string.Empty,
                                ACCESS_CODE = string.Empty,
                                PRIMARY_SSID = string.Empty,
                                WPA_KEY = string.Empty,
                                MAC_ID_CABLE = string.Empty,
                                MAC_ID_EMTA = string.Empty,
                                HARDWARE_VERSION = string.Empty,
                                FIRMWARE_VERSION = string.Empty,
                                EAN_CODE = string.Empty,
                                SOFTWARE_VERSION = string.Empty,
                                SRM_PASSWORD = string.Empty,
                                RF_MAC_ID = string.Empty,
                                MACID_IN_MTA = string.Empty,
                                MTA_MAN_ROUTER_MAC = string.Empty,
                                MTADATA_MAC = string.Empty,
                                ETHERNET_MAC = string.Empty,
                                USB_MAC = string.Empty,
                                PRIMARYSSID_PASSPHRASE = string.Empty,
                                CMCI_MAC = string.Empty,
                                LAN_MAC = string.Empty,
                                WAN_MAC = string.Empty,
                                DEVICE_MAC = string.Empty,
                                WIRELESS_MAC = string.Empty,
                                WIFI_MAC_SSID1 = string.Empty,
                                SSID1 = string.Empty,
                                SSID1_PASSPHRASE = string.Empty,
                                WPA_PASSPHRASE = string.Empty,
                                WPS_PIN_CODE = string.Empty,
                                PPPOA_USERNAME = string.Empty,
                                PPPOA_PASSPHRASE = string.Empty,
                                TR069_UNIQUE_KEY_64_BIT = string.Empty,
                                FON_KEY = string.Empty,
                                PA_SN = string.Empty,
                                PA_ITEM_NUMBER = string.Empty,
                                LASTEDITBY = string.Empty,
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
                                    ptemitemobj.PA_SN = rsnitem.VALUE ?? string.Empty;
                                    ptemitemobj.PA_ITEM_NUMBER = new Func<string>(() =>
                                    {
                                        if (rsnitem.PARTNO == null) return string.Empty;
                                        if (rsnitem.PARTNO.StartsWith("B"))
                                            return rsnitem.PARTNO.Substring(1);
                                        return rsnitem.PARTNO;
                                    })();
                                    ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                    db.Insertable(ptemitemobj).ExecuteCommand();

                                    ///PPM S/N
                                    foreach (var pboxitem in rsnkpPboxItem)
                                    {
                                        ptemitemobj.SEQNO = seqno++;
                                        ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                        ptemitemobj.TA = new Func<string>(() =>
                                        {
                                            var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                    t.DETAIL_ID == pboxitem.rfc.ID &&
                                                    t.NAME == Ptm_Ta_Fa.TA.ExtName())
                                                .ToList();
                                            if (taobj.Count == 0)
                                            {
                                                ptmLog(db, Ptm_Err.MIS_CSN_TA.ExtValue(),
                                                    item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                throw new MesException(Ptm_Err.MIS_CSN_TA.Ext<EnumValueAttribute>()
                                                    .Description);
                                            }

                                            return taobj.FirstOrDefault().VALUE ?? string.Empty;
                                        })();
                                        ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                        {
                                            var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                    t.DETAIL_ID == pboxitem.rfc.ID &&
                                                    t.NAME == Ptm_Ta_Fa.FA.ExtName())
                                                .ToList();
                                            if (faobj.Count == 0)
                                            {
                                                ptmLog(db, Ptm_Err.MIS_CSN_FA.ExtValue(),
                                                    item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                throw new MesException(Ptm_Err.MIS_CSN_FA.Ext<EnumValueAttribute>()
                                                    .Description);
                                            }

                                            return faobj.FirstOrDefault().VALUE ?? string.Empty;
                                        })();
                                        ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                        db.Insertable(ptemitemobj).ExecuteCommand();
                                    }
                                }
                            }
                            else //無PS S/N
                            {
                                var ptemitemobj = ptmitem;
                                ptemitemobj.SEQNO = seqno++;
                                ptemitemobj.PA_SN = string.Empty;
                                ptemitemobj.PA_ITEM_NUMBER = string.Empty;
                                db.Insertable(ptemitemobj).ExecuteCommand();

                                ///PPM S/N
                                foreach (var pboxitem in rsnkpPboxItem)
                                {
                                    ptemitemobj.SEQNO = seqno++;
                                    ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                    ptemitemobj.TA = new Func<string>(() =>
                                    {
                                        var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == Ptm_Ta_Fa.TA.ExtName())
                                            .ToList();
                                        if (taobj.Count == 0)
                                        {
                                            ptmLog(db, Ptm_Err.MIS_CSN_TA.ExtValue(), item.ID,
                                                ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                            throw new MesException(
                                                Ptm_Err.MIS_CSN_TA.Ext<EnumValueAttribute>().Description);
                                        }

                                        return taobj.FirstOrDefault().VALUE ?? string.Empty;
                                    })();
                                    ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                    {
                                        var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == Ptm_Ta_Fa.FA.ExtName())
                                            .ToList();
                                        if (faobj.Count == 0)
                                        {
                                            ptmLog(db, Ptm_Err.MIS_CSN_TA.ExtValue(), item.ID,
                                                ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                            throw new MesException(
                                                Ptm_Err.MIS_CSN_FA.Ext<EnumValueAttribute>().Description);
                                        }

                                        return faobj.FirstOrDefault().VALUE ?? string.Empty;
                                    })();
                                    ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                    db.Insertable(ptemitemobj).ExecuteCommand();
                                }
                            }
                        }
                    }

                }

                #endregion
                //Không có Mac
                #region 無MAC的取值

                else
                {
                    if (PTM_CATEGORY_SKUNO.IndexOf(item.SKUNO) > -1)
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
                                MAC_ID = string.Empty,
                                ASN_NUMBER = item.DN_NO,
                                INVOICE_NUMBER = item.DN_NO,
                                PACKING_SLIP_NUMBER = item.DN_NO,
                                CONTAINER_NUMBER = item.DN_NO,
                                PO_NUMBER = item.PO_NO ?? string.Empty,
                                PO_LINE_NUMBER = string.Empty,
                                XFACTORY_DATE =
                                    tempqueobjitem.SHIPDATE?.ToString(MES_CONST_DATETIME_FORMAT.MDY_B
                                        .Ext<EnumValueAttribute>().Description) ??
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.MDY_B.Ext<EnumValueAttribute>()
                                        .Description),
                                MANUFACTURER_NAME = PTM_MANUFACTURER_NAME,
                                AS_DATE_OF_MANUFACTURE =
                                    tempqueobjitem.CUSTSSN?.ToString(MES_CONST_DATETIME_FORMAT.MDY_B
                                        .Ext<EnumValueAttribute>().Description) ??
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.MDY_B.Ext<EnumValueAttribute>()
                                        .Description),
                                COUNTRY_OF_ORIGIN = PTM_COO,
                                ORG_CODE = ctl.ORGCODE,
                                ITEM_NUMBER_LEVEL_REV = string.Empty,
                                TA = new Func<string>(() =>
                                {
                                    var taobj = rptmtaconfigobj.FindAll(t =>
                                        t.WO == tempqueobjitem.WORKORDERNO);
                                    if (taobj.Count == 0)
                                    {
                                        ptmLog(db, Ptm_Err.MIS_WO_TA.ExtValue(), item.ID,
                                            tempqueobjitem.WORKORDERNO);
                                        throw new MesException(Ptm_Err.MIS_WO_TA.Ext<EnumValueAttribute>().Description);
                                    }

                                    return taobj.FirstOrDefault().TA_NUMBER;
                                })(),
                                FA_NUMBER = new Func<string>(() =>
                                {
                                    var faobj = rfunctioncontrolobj.FindAll(t =>
                                        t.rfc.EXTVAL == tempqueobjitem.SN.Substring(0, 4) &&
                                        t.rfcx.NAME == Ptm_Ta_Fa.FA.ExtName());
                                    if (faobj.Count == 0)
                                    {
                                        ptmLog(db, Ptm_Err.MIS_WO_FA.ExtValue(), item.ID,
                                            tempqueobjitem.WORKORDERNO);
                                        throw new MesException(Ptm_Err.MIS_WO_FA.Ext<EnumValueAttribute>().Description);
                                    }

                                    return faobj.FirstOrDefault().rfcx.VALUE ?? string.Empty;
                                })(),
                                WEP_KEY = string.Empty,
                                WIFI_ID = string.Empty,
                                ACCESS_CODE = string.Empty,
                                PRIMARY_SSID = string.Empty,
                                WPA_KEY = string.Empty,
                                MAC_ID_CABLE = string.Empty,
                                MAC_ID_EMTA = string.Empty,
                                HARDWARE_VERSION = string.Empty,
                                FIRMWARE_VERSION = string.Empty,
                                EAN_CODE = string.Empty,
                                SOFTWARE_VERSION = string.Empty,
                                SRM_PASSWORD = string.Empty,
                                RF_MAC_ID = string.Empty,
                                MACID_IN_MTA = string.Empty,
                                MTA_MAN_ROUTER_MAC = string.Empty,
                                MTADATA_MAC = string.Empty,
                                ETHERNET_MAC = string.Empty,
                                USB_MAC = string.Empty,
                                PRIMARYSSID_PASSPHRASE = string.Empty,
                                CMCI_MAC = string.Empty,
                                LAN_MAC = string.Empty,
                                WAN_MAC = string.Empty,
                                DEVICE_MAC = string.Empty,
                                WIRELESS_MAC = string.Empty,
                                WIFI_MAC_SSID1 = string.Empty,
                                SSID1 = string.Empty,
                                SSID1_PASSPHRASE = string.Empty,
                                WPA_PASSPHRASE = string.Empty,
                                WPS_PIN_CODE = string.Empty,
                                PPPOA_USERNAME = string.Empty,
                                PPPOA_PASSPHRASE = string.Empty,
                                TR069_UNIQUE_KEY_64_BIT = string.Empty,
                                FON_KEY = string.Empty,
                                PA_SN = string.Empty,
                                PA_ITEM_NUMBER = string.Empty,
                                LASTEDITBY = string.Empty,
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
                                    ptemitemobj.PA_SN = rsnitem.VALUE ?? string.Empty;
                                    ptemitemobj.PA_ITEM_NUMBER = new Func<string>(() =>
                                    {
                                        if (rsnitem.PARTNO == null) return string.Empty;
                                        if (rsnitem.PARTNO.StartsWith("B"))
                                            return rsnitem.PARTNO.Substring(1);
                                        return rsnitem.PARTNO;
                                    })();
                                    ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                    db.Insertable(ptemitemobj).ExecuteCommand();

                                    ///PPM S/N
                                    foreach (var pboxitem in rsnkpPboxItem)
                                    {
                                        ptemitemobj.SEQNO = seqno++;
                                        ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                        ptemitemobj.TA = new Func<string>(() =>
                                        {
                                            var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                    t.DETAIL_ID == pboxitem.rfc.ID &&
                                                    t.NAME == Ptm_Ta_Fa.TA.ExtName())
                                                .ToList();
                                            if (taobj.Count == 0)
                                            {
                                                ptmLog(db, Ptm_Err.MIS_CSN_TA.ExtValue(),
                                                    item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                throw new MesException(Ptm_Err.MIS_CSN_TA.Ext<EnumValueAttribute>()
                                                    .Description);
                                            }

                                            return taobj.FirstOrDefault().VALUE ?? string.Empty;
                                        })();
                                        ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                        {
                                            var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                    t.DETAIL_ID == pboxitem.rfc.ID &&
                                                    t.NAME == Ptm_Ta_Fa.FA.ExtName())
                                                .ToList();
                                            if (faobj.Count == 0)
                                            {
                                                ptmLog(db, Ptm_Err.MIS_CSN_FA.ExtValue(),
                                                    item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                throw new MesException(Ptm_Err.MIS_CSN_FA.Ext<EnumValueAttribute>()
                                                    .Description);
                                            }

                                            return faobj.FirstOrDefault().VALUE ?? string.Empty;
                                        })();
                                        ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                        db.Insertable(ptemitemobj).ExecuteCommand();
                                    }
                                }
                            }
                            else //無PS S/N
                            {
                                var ptemitemobj = ptmitem;
                                ptemitemobj.SEQNO = seqno++;
                                ptemitemobj.PA_SN = string.Empty;
                                ptemitemobj.PA_ITEM_NUMBER = string.Empty;
                                ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                db.Insertable(ptemitemobj).ExecuteCommand();

                                ///PPM S/N
                                foreach (var pboxitem in rsnkpPboxItem)
                                {
                                    ptemitemobj.SEQNO = seqno++;
                                    ptemitemobj.SERIAL_NUMBER = pboxitem.rsk.VALUE;
                                    ptemitemobj.TA = new Func<string>(() =>
                                    {
                                        var taobj = rsnkpPboxTaFa.FindAll(t =>
                                                t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == Ptm_Ta_Fa.TA.ExtName())
                                            .ToList();
                                        if (taobj.Count == 0)
                                        {
                                            ptmLog(db, Ptm_Err.MIS_CSN_TA.ExtValue(), item.ID,
                                                ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                            throw new MesException(
                                                Ptm_Err.MIS_CSN_TA.Ext<EnumValueAttribute>().Description);
                                        }

                                        return taobj.FirstOrDefault().VALUE ?? string.Empty;
                                    })();
                                    ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                    {
                                        var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                t.DETAIL_ID == pboxitem.rfc.ID && t.NAME == Ptm_Ta_Fa.FA.ExtName())
                                            .ToList();
                                        if (faobj.Count == 0)
                                        {
                                            ptmLog(db, Ptm_Err.MIS_CSN_FA.ExtValue(), item.ID,
                                                ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                            throw new MesException(
                                                Ptm_Err.MIS_CSN_FA.Ext<EnumValueAttribute>().Description);
                                        }

                                        return faobj.FirstOrDefault().VALUE ?? string.Empty;
                                    })();
                                    ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
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
                                MAC_ID = string.Empty,
                                ASN_NUMBER = item.DN_NO,
                                INVOICE_NUMBER = item.DN_NO,
                                PACKING_SLIP_NUMBER = item.DN_NO,
                                CONTAINER_NUMBER = item.DN_NO,
                                PO_NUMBER = item.PO_NO ?? string.Empty,
                                PO_LINE_NUMBER = string.Empty,
                                XFACTORY_DATE =
                                    tempqueobjitem.SHIPDATE?.ToString(MES_CONST_DATETIME_FORMAT.MDY_B
                                        .Ext<EnumValueAttribute>().Description) ??
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.MDY_B.Ext<EnumValueAttribute>()
                                        .Description),
                                MANUFACTURER_NAME = PTM_MANUFACTURER_NAME,
                                AS_DATE_OF_MANUFACTURE =
                                    tempqueobjitem.CUSTSSN?.ToString(MES_CONST_DATETIME_FORMAT.MDY_B
                                        .Ext<EnumValueAttribute>().Description) ??
                                    DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.MDY_B.Ext<EnumValueAttribute>()
                                        .Description),
                                COUNTRY_OF_ORIGIN = PTM_COO,
                                ORG_CODE = ctl.ORGCODE,
                                ITEM_NUMBER_LEVEL_REV = string.Empty,
                                TA = new Func<string>(() =>
                                {
                                    var taobj = rptmtaconfigobj.FindAll(t =>
                                        t.WO == tempqueobjitem.WORKORDERNO);
                                    if (taobj.Count == 0)
                                    {
                                        ptmLog(db, Ptm_Err.MIS_WO_TA.ExtValue(), item.ID,
                                            tempqueobjitem.WORKORDERNO);
                                        throw new MesException(Ptm_Err.MIS_WO_TA.Ext<EnumValueAttribute>().Description);
                                    }

                                    return taobj.FirstOrDefault().TA_NUMBER;
                                })(),
                                FA_NUMBER = new Func<string>(() =>
                                {
                                    var faobj = rfunctioncontrolobj.FindAll(t =>
                                        t.rfc.EXTVAL == tempqueobjitem.SN.Substring(0, 4) &&
                                        t.rfcx.NAME == Ptm_Ta_Fa.FA.ExtName());
                                    if (faobj.Count == 0)
                                    {
                                        ptmLog(db, Ptm_Err.MIS_WO_FA.ExtValue(), item.ID,
                                            tempqueobjitem.WORKORDERNO);
                                        throw new MesException(Ptm_Err.MIS_WO_FA.Ext<EnumValueAttribute>().Description);
                                    }

                                    return faobj.FirstOrDefault().rfcx.VALUE ?? string.Empty;
                                })(),
                                WEP_KEY = string.Empty,
                                WIFI_ID = string.Empty,
                                ACCESS_CODE = string.Empty,
                                PRIMARY_SSID = string.Empty,
                                WPA_KEY = string.Empty,
                                MAC_ID_CABLE = string.Empty,
                                MAC_ID_EMTA = string.Empty,
                                HARDWARE_VERSION = string.Empty,
                                FIRMWARE_VERSION = string.Empty,
                                EAN_CODE = string.Empty,
                                SOFTWARE_VERSION = string.Empty,
                                SRM_PASSWORD = string.Empty,
                                RF_MAC_ID = string.Empty,
                                MACID_IN_MTA = string.Empty,
                                MTA_MAN_ROUTER_MAC = string.Empty,
                                MTADATA_MAC = string.Empty,
                                ETHERNET_MAC = string.Empty,
                                USB_MAC = string.Empty,
                                PRIMARYSSID_PASSPHRASE = string.Empty,
                                CMCI_MAC = string.Empty,
                                LAN_MAC = string.Empty,
                                WAN_MAC = string.Empty,
                                DEVICE_MAC = string.Empty,
                                WIRELESS_MAC = string.Empty,
                                WIFI_MAC_SSID1 = string.Empty,
                                SSID1 = string.Empty,
                                SSID1_PASSPHRASE = string.Empty,
                                WPA_PASSPHRASE = string.Empty,
                                WPS_PIN_CODE = string.Empty,
                                PPPOA_USERNAME = string.Empty,
                                PPPOA_PASSPHRASE = string.Empty,
                                TR069_UNIQUE_KEY_64_BIT = string.Empty,
                                FON_KEY = string.Empty,
                                PA_SN = string.Empty,
                                PA_ITEM_NUMBER = string.Empty,
                                LASTEDITBY = string.Empty,
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
                                    ptemitemobj.MAC_ID = string.Empty;
                                    ptemitemobj.SEQNO = seqno++;
                                    ptemitemobj.PA_SN = rsnitem.VALUE ?? string.Empty;
                                    ptemitemobj.PA_ITEM_NUMBER = new Func<string>(() =>
                                    {
                                        if (rsnitem.PARTNO == null) return string.Empty;
                                        if (rsnitem.PARTNO.StartsWith("B"))
                                            return rsnitem.PARTNO.Substring(1);
                                        return rsnitem.PARTNO;
                                    })();
                                    ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
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
                                                        t.NAME == Ptm_Ta_Fa.TA.ExtName())
                                                    .ToList();
                                                if (taobj.Count == 0)
                                                {
                                                    ptmLog(db, Ptm_Err.MIS_CSN_TA.ExtValue(),
                                                        item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                    throw new MesException(Ptm_Err.MIS_CSN_TA.Ext<EnumValueAttribute>()
                                                        .Description);
                                                }

                                                return taobj.FirstOrDefault().VALUE ?? string.Empty;
                                            })();
                                            ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                            {
                                                var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                        t.DETAIL_ID == pboxitem.rfc.ID &&
                                                        t.NAME == Ptm_Ta_Fa.FA.ExtName())
                                                    .ToList();
                                                if (faobj.Count == 0)
                                                {
                                                    ptmLog(db, Ptm_Err.MIS_CSN_FA.ExtValue(),
                                                        item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                    throw new MesException(Ptm_Err.MIS_CSN_FA.Ext<EnumValueAttribute>()
                                                        .Description);
                                                }

                                                return faobj.FirstOrDefault().VALUE ?? string.Empty;
                                            })();
                                            ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                            db.Insertable(ptemitemobj).ExecuteCommand();
                                        }
                                    }
                                }
                            }
                            else //無PS S/N
                            {
                                var ptemitemobj = ptmitem;
                                ptemitemobj.SEQNO = seqno++;
                                ptemitemobj.PA_SN = string.Empty;
                                ptemitemobj.PA_ITEM_NUMBER = string.Empty;
                                ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
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
                                                    t.DETAIL_ID == pboxitem.rfc.ID &&
                                                    t.NAME == Ptm_Ta_Fa.TA.ExtName())
                                                .ToList();
                                            if (taobj.Count == 0)
                                            {
                                                ptmLog(db, Ptm_Err.MIS_CSN_TA.ExtValue(),
                                                    item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                throw new MesException(Ptm_Err.MIS_CSN_TA.Ext<EnumValueAttribute>()
                                                    .Description);
                                            }

                                            return taobj.FirstOrDefault().VALUE ?? string.Empty;
                                        })();
                                        ptemitemobj.FA_NUMBER = new Func<string>(() =>
                                        {
                                            var faobj = rsnkpPboxTaFa.FindAll(t =>
                                                    t.DETAIL_ID == pboxitem.rfc.ID &&
                                                    t.NAME == Ptm_Ta_Fa.FA.ExtName())
                                                .ToList();
                                            if (faobj.Count == 0)
                                            {
                                                ptmLog(db, Ptm_Err.MIS_CSN_FA.ExtValue(),
                                                    item.ID, ptemitemobj.TOP_SERIAL_NUMBER, pboxitem.rsk.VALUE);
                                                throw new MesException(Ptm_Err.MIS_CSN_FA.Ext<EnumValueAttribute>()
                                                    .Description);
                                            }

                                            return faobj.FirstOrDefault().VALUE ?? string.Empty;
                                        })();
                                        ptemitemobj.ID = MesDbBase.GetNewID<R_NETGEAR_PTM_DATA>(db, _bustr);
                                        db.Insertable(ptemitemobj).ExecuteCommand();
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
                //Update trạng thái
                #region 更新狀態

                db.Updateable<R_DN_STATUS>().SetColumns(t =>
                        new R_DN_STATUS()
                        {
                            DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_CQA.Ext<EnumValueAttribute>().Description
                        })
                    .Where(t => t.ID == item.ID).ExecuteCommand();
                funcExecres.IsSuccess = true;

                #endregion
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(MesException))
                    ptmLog(db, "TolErr", item.ID, item.ID, string.Empty, e.Message);
                funcExecres.ErrorException = e;
                funcExecres.ErrorMessage = e.Message;
                funcExecres.IsSuccess = false;
            }

            return funcExecres;
        }
        /// <summary>
        /// create ptm file
        /// </summary>
        /// <param name="item"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public FuncExecRes GanarationFileByCtl(R_NETGEAR_PTM_CTL item, string Filepath, SqlSugarClient db)
        {
            var funcExecres = new FuncExecRes();
            try
            {
                #region Get Ptm Data By SHIPORDERID

                var targetitem = db.Queryable<R_NETGEAR_PTM_DATA>()
                    .Where(t => t.SHIPORDERID == item.SHIPORDERID).Select(t => new
                    {
                        t.SEQNO,
                        t.PALLET_ID,
                        t.MASTER_CARTON_ID,
                        t.ITEM_NUMBER,
                        t.SERIAL_NUMBER,
                        t.TOP_SERIAL_NUMBER,
                        t.MAC_ID,
                        t.ASN_NUMBER,
                        t.INVOICE_NUMBER,
                        t.PACKING_SLIP_NUMBER,
                        t.CONTAINER_NUMBER,
                        t.PO_NUMBER,
                        t.PO_LINE_NUMBER,
                        t.XFACTORY_DATE,
                        t.MANUFACTURER_NAME,
                        t.AS_DATE_OF_MANUFACTURE,
                        t.COUNTRY_OF_ORIGIN,
                        t.ORG_CODE,
                        t.IMEI_NUMBER,
                        t.MASTERLOCK_NUMBER,
                        t.NETWORKLOCK_NUMBER,
                        t.SERVICELOCK_NUMBER,
                        t.FA_NUMBER_LEVEL_REV,
                        t.FA_NUMBER,
                        t.ITEM_NUMBER_LEVEL_REV,
                        t.WEP_KEY,
                        t.WIFI_ID,
                        t.ACCESS_CODE,
                        t.PRIMARY_SSID,
                        t.WPA_KEY,
                        t.MAC_ID_CABLE,
                        t.MAC_ID_EMTA,
                        t.HARDWARE_VERSION,
                        t.FIRMWARE_VERSION,
                        t.EAN_CODE,
                        t.SOFTWARE_VERSION,
                        t.SRM_PASSWORD,
                        t.RF_MAC_ID,
                        t.MACID_IN_MTA,
                        t.MTA_MAN_ROUTER_MAC,
                        t.MTADATA_MAC,
                        t.ETHERNET_MAC,
                        t.USB_MAC,
                        t.PRIMARYSSID_PASSPHRASE,
                        t.CMCI_MAC,
                        t.LAN_MAC,
                        t.WAN_MAC,
                        t.DEVICE_MAC,
                        t.WIRELESS_MAC,
                        t.WIFI_MAC_SSID1,
                        t.SSID1,
                        t.SSID1_PASSPHRASE,
                        t.WPA_PASSPHRASE,
                        t.WPS_PIN_CODE,
                        t.PPPOA_USERNAME,
                        t.PPPOA_PASSPHRASE,
                        t.TR069_UNIQUE_KEY_64_BIT,
                        t.FON_KEY,
                        t.TA,
                        t.PA_SN,
                        t.PA_ITEM_NUMBER
                    }).OrderBy(t => t.SEQNO, OrderByType.Asc).ToList();

                #endregion
                var itemfilename =
                    $@"{PTM_FILE_PREFIX}_{item.DN}_{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description)}{PTM_FILE_EXTEND}";
                var res = GanarationFile(targetitem, Filepath, itemfilename, item.SHIPORDERID, db, (objList, shiporder, path, filename, cdb) =>
                {
                    FileStream stream = new FileInfo($@"{path}/{filename}").OpenRead();
                    Byte[] buffer = new Byte[stream.Length];
                    stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                    if (cdb != null)
                    {
                        cdb.Updateable<R_FILE>().SetColumns(t => new R_FILE() { VALID = 0 })
                            .Where(t => t.NAME == shiporder && t.USETYPE == PTM_FLAG).ExecuteCommand();
                        cdb.Insertable(new R_FILE()
                        {
                            ID = MesDbBase.GetNewID<R_FILE>(cdb, this._bustr),
                            FILENAME = filename,
                            NAME = shiporder,
                            USETYPE = PTM_FLAG,
                            VALID = 1,
                            BLOB_FILE = buffer
                        }).ExecuteCommand();
                    }
                });
                if (res.IsSuccess)
                {
                    #region 更新狀態
                    db.Updateable<R_NETGEAR_PTM_CTL>().SetColumns(t => new R_NETGEAR_PTM_CTL()
                    {
                        CONVERTED = ENUM_R_NETGEAR_PTM_CTL.CONVER_END.Ext<EnumValueAttribute>().Description,
                        PTMFILE = itemfilename
                    }).Where(t => t.SHIPORDERID == item.SHIPORDERID).ExecuteCommand();
                    db.Updateable<R_DN_STATUS>().SetColumns(t => new R_DN_STATUS()
                    {
                        DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_SEND_ASN.Ext<EnumValueAttribute>().Description
                    }).Where(t => t.ID == item.SHIPORDERID).ExecuteCommand();
                    #endregion
                }
                else if (res.ErrorException.GetType() != typeof(MesException))
                {
                    ptmLog(db, Ptm_Err.GENERATE_FILE.ExtValue(), item.SHIPORDERID,
                        string.Empty, res.ErrorMessage);
                    throw res.ErrorException;
                }
                else
                    throw res.ErrorException;
                funcExecres.IsSuccess = true;
            }
            catch (Exception e)
            {
                funcExecres.ErrorException = e;
                funcExecres.ErrorMessage = e.Message;
                funcExecres.IsSuccess = false;
            }
            return funcExecres;
        }
        /// <summary>
        /// genaration file
        /// </summary>
        FuncExecRes GanarationFile<T>(List<T> objList, string path, string filename, string Shiporder = null, SqlSugarClient db = null, Action<List<T>, string, string, string, SqlSugarClient> callback = null)
        {
            var res = new FuncExecRes();
            try
            {
                var fullfilename = $@"{path}/{filename}";
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                if (System.IO.File.Exists(fullfilename))
                {
                    FileInfo fi = new FileInfo(fullfilename);
                    fi.MoveTo(
                        $@"{path}/{filename}_{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description)}");
                }

                using (var fs1 = new FileStream(fullfilename, FileMode.Create, FileAccess.ReadWrite))
                {
                    StreamWriter sw = new StreamWriter(fs1);
                    var newTypecol = objList[0].GetType();
                    var linestrcol = "";
                    foreach (var item in newTypecol.GetRuntimeProperties())
                        if (CONST_NOT_FILE_COL.IndexOf(item.Name) == -1)
                            linestrcol += $@"{item.Name}{PTM_FILE_CONTENTS_SPLIT}";
                    sw.WriteLine(linestrcol);
                    foreach (var objitem in objList)
                    {
                        var newType = objitem.GetType();
                        var linestr = "";
                        foreach (var item in newType.GetRuntimeProperties())
                            if (CONST_NOT_FILE_COL.IndexOf(item.Name) == -1)
                                linestr += $@"{item.GetValue(objitem)}{PTM_FILE_CONTENTS_SPLIT}";
                        sw.WriteLine(linestr);
                    }
                    sw.Flush();
                }

                if (callback != null)
                    callback(objList, Shiporder, path, filename, db);

                res.IsSuccess = true;
            }
            catch (Exception e)
            {
                res.ErrorException = e;
                res.ErrorMessage = e.Message;
            }
            return res;
        }
        void GanarationFile()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var waitdolist = db.Queryable<R_NETGEAR_PTM_CTL>().Where(t =>
                    t.CQA == ENUM_R_NETGEAR_PTM_CTL.CQA_END.Ext<EnumValueAttribute>().Description && t.CONVERTED ==
                    ENUM_R_NETGEAR_PTM_CTL.WAIT_CONVER.Ext<EnumValueAttribute>().Description).ToList();
                foreach (var item in waitdolist)
                    GanarationFileByCtl(item, _filepath, db);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="functiontype">DCN_PTM_ALARM</param>
        /// <param name="message"></param>
        /// <param name="head"></param>
        /// <param name="mdskey"></param>
        void ptmLog(SqlSugarClient dbClient, string type, string data2, string data3, string data4 = null,
            string data8 = null)
        {
            OleExec dbobj = null;
            if (_dbExecPool != null)
                dbobj = _dbExecPool.Borrow();
            SqlSugarClient db = _dbExecPool != null ? dbobj.ORM : dbClient;
            try
            {
                db.Insertable(new R_SERVICE_LOG()
                {
                    ID = MesDbBase.GetNewID<R_SERVICE_LOG>(db, _bustr),
                    FUNCTIONTYPE = "DCN_PTM_ALARM",
                    CURRENTEDITTIME = DateTime.Now,
                    SOURCECODE = MesLog.GetCurrentMethodFullName(),
                    DATA1 = type,
                    DATA2 = data2,
                    DATA3 = data3,
                    DATA4 = data4,
                    DATA8 = (!string.IsNullOrEmpty(data8) && data8.Length > 249 )? data8.Substring(0, 249) : data8,
                    MAILFLAG = "N"
                }).ExecuteCommand();
            }
            catch
            {
            }
            finally
            {
                if (dbobj != null)
                    _dbExecPool.Return(dbobj);
            }
        }
        /// <summary>
        /// 傳送數據給客人
        /// </summary>
        void SendPtmData()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                #region send to custermor
                var waitsendobj = db.Queryable<R_NETGEAR_PTM_CTL>().Where(t =>
                    t.SENT == ENUM_R_NETGEAR_PTM_CTL.WAIT_SEND.Ext<EnumValueAttribute>().Description &&
                    t.CONVERTED == ENUM_R_NETGEAR_PTM_CTL.CONVER_END.Ext<EnumValueAttribute>().Description &&
                    t.CQA == ENUM_R_NETGEAR_PTM_CTL.CQA_END.Ext<EnumValueAttribute>().Description &&
                    String.IsNullOrEmpty(t.PTMFILE)).ToList();
                foreach (var item in waitsendobj)
                {
                    try
                    {
                        var res = SendPtmData(item, _filepath, db);
                        if (!res.IsSuccess)
                            throw res.ErrorException;
                    }
                    catch (Exception e)
                    {
                        ptmLog(db, Ptm_Err.SENDMDSDATA.ExtValue(), item.SHIPORDERID, item.PTMFILE, item.PO, e.Message);
                        //throw;
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 傳送數據給客人
        /// </summary>
        public FuncExecRes SendPtmData(R_NETGEAR_PTM_CTL item, string Filepath, SqlSugarClient db)
        {
            var funcExecres = new FuncExecRes();
            var currentptmobj = db.Queryable<R_NETGEAR_PTM_CTL>().Where(t => t.SHIPORDERID == item.SHIPORDERID).ToList()
                .FirstOrDefault();
            #region send to custermor
            //SFTPHelper sftpHelp =
            //    new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, $@"{Filepath}/{CONST_SFTP_KEY}");
            SFTPHelper sftpHelp =
                new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, $@"{Filepath}/{CONST_SFTP_KEY}", new ProxyObj() { proxyHost = CONST_ProxyHost, proxyPort = CONST_ProxyPort, proxyTypes = ProxyTypes.Http });
            try
            {
                var rfile = db.Queryable<R_FILE>().Where(t =>
                    t.NAME == currentptmobj.SHIPORDERID &&
                    t.USETYPE == ENUM_R_FILE.USETYPE_PTM.Ext<EnumValueAttribute>().Description && t.VALID.ToString() ==
                    ENUM_R_FILE.VALID_TRUE.Ext<EnumValueAttribute>().Description).ToList().FirstOrDefault();
                if (rfile == null)
                    throw new Exception(Ptm_Err.NOT_EXIST_FILE.ExtValue());
                var fullfilename = $@"{Filepath}/{currentptmobj.PTMFILE}";
                if (Directory.Exists(Filepath) == false)
                    Directory.CreateDirectory(Filepath);
                if (System.IO.File.Exists(fullfilename))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(fullfilename);
                        fi.MoveTo(
                            $@"{Filepath}/{currentptmobj.PTMFILE}_{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.Ext<EnumValueAttribute>().Description)}");
                    }
                    catch { }
                }
                if (!System.IO.File.Exists(fullfilename))
                    using (var fs1 = new FileStream(fullfilename, FileMode.Create, FileAccess.ReadWrite))
                    {
                        var pReadByte = (byte[])rfile.BLOB_FILE;
                        fs1.Write(pReadByte, 0, pReadByte.Length);
                    }
                sftpHelp.Put(fullfilename, $@"{CONST_SFTPPath}/{currentptmobj.PTMFILE}");
                currentptmobj.SENT = ENUM_R_NETGEAR_PTM_CTL.SEND_END.ExtValue();
                currentptmobj.EDITTIME = DateTime.Now;
                db.Updateable(currentptmobj).ExecuteCommand();
                funcExecres.IsSuccess = true;
            }
            catch (Exception e)
            {
                ptmLog(db, Ptm_Err.SENDMDSDATA.ExtValue(), item.SHIPORDERID, item.PTMFILE, item.PO, e.Message);
                funcExecres.ErrorException = e;
                funcExecres.ErrorMessage = e.Message;
                funcExecres.IsSuccess = false;
            }
            return funcExecres;
            #endregion
        }
        void BuildData()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(this._dbstr, false))
            {
                var weidojoblist = db.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER, R_TO_DETAIL>((rds, csk, css, cc, rtd) =>
                        rds.SKUNO == csk.SKUNO && csk.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID && rds.DN_NO == rtd.DN_NO)
                    .Where((rds, csk, css, cc, rtd) => rds.DN_FLAG == ENUM_R_DN_STATUS.DN_WAIT_GENERATE_ASN.Ext<EnumValueAttribute>().Description && cc.CUSTOMER_NAME.ToUpper() == Customer.NETGEAR.Ext<EnumValueAttribute>().Description)
                    .Select((rds, csk, css, cc, rtd) => rds).ToList();
                foreach (var item in weidojoblist)
                    db.Ado.UseTran(() => { BuildDataByDnObj(item, db); });
            }
        }
    }
}
