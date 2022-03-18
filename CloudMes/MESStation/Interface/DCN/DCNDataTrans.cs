using DcnSfcModel;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDBHelper;
using MESPubLab.MESStation;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MESStation.Interface.DCN
{
    public class DCNDataTrans : MesAPIBase
    {
        private APIInfo FGetDCNSNDataFromOlddb = new APIInfo()
        {
            FunctionName = "GetDCNSNDataFromOlddb",
            Description = "从DCN旧系统抓取SN信息",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName = "SN", InputType = "String", DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo FGetDCNCSVFromOlddb = new APIInfo()
        {
            FunctionName = "GetDCNCSVFromOlddb",
            Description = "从DCN旧系统抓取CSV信息",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName = "FILENAME", InputType = "String", DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo FTranDCNSNDataToOlddb = new APIInfo()
        {
            FunctionName = "TranDCNSNDataToOlddb",
            Description = "把数据传送到DCN旧系统",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName = "SN", InputType = "String", DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }
        };

        public DCNDataTrans()
        {
            this.Apis.Add(FGetDCNSNDataFromOlddb.FunctionName, FGetDCNSNDataFromOlddb);
            this.Apis.Add(FGetDCNCSVFromOlddb.FunctionName, FGetDCNCSVFromOlddb);
            this.Apis.Add(FTranDCNSNDataToOlddb.FunctionName, FTranDCNSNDataToOlddb);
        }

        public void GetDCNCSVFromOlddb(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {


                string strSN = Data["FILENAME"].ToString();
                var OldDB = new OleExec("server =10.120.176.101,3000; uid = dcnadmin; pwd = nsdiisfc169!; database = sjefox", SqlSugar.DbType.SqlServer);

                //var OldDB = new OleExec("server =10.120.176.82,3000; uid = dcnadmin; pwd = nsdiisfc169!; database = sjefox", SqlSugar.DbType.SqlServer);


                var strsql = $@"select* from [dbo].[CSV_HEAD] where filename = '{strSN}'";
                var ret = OldDB.RunSelect(strsql);
                if (ret.Tables[0].Rows.Count > 0)
                {
                    sfcdb.ORM.Deleteable<BROADCOM_CSV_HEAD>().Where(t => t.FILENAME == strSN).ExecuteCommand();
                    BROADCOM_CSV_HEAD V = new BROADCOM_CSV_HEAD()
                    { FILENAME = ret.Tables[0].Rows[0]["FILENAME"].ToString(), CREATEDT = (DateTime)ret.Tables[0].Rows[0]["CREATEDT"], STATUS = "0" };
                    sfcdb.ORM.Insertable<BROADCOM_CSV_HEAD>(V).ExecuteCommand();
                }
                else
                {
                    throw new Exception("无此文件!");
                }

                strsql = $@"SELECT * FROM [dbo].[CSV_DETAIL] where filename='{strSN}'  order by 3 ";
                ret = OldDB.RunSelect(strsql);
                if (ret.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("无此文件的CSV_DETAIL!");
                }
                Type T = typeof(BROADCOM_CSV_DETAIL);
                var Properties = T.GetProperties();
                sfcdb.ORM.Deleteable<BROADCOM_CSV_DETAIL>().Where(t => t.FILENAME == strSN).ExecuteCommand();

                T_BROADCOM_CSV_DETAIL T_BCD = new T_BROADCOM_CSV_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);


                for (int i = 0; i < ret.Tables[0].Rows.Count; i++)
                {
                    var nr = (Row_BROADCOM_CSV_DETAIL)T_BCD.NewRow();
                    var r = ret.Tables[0].Rows[i];
                    for (int j = 0; j < ret.Tables[0].Columns.Count; j++)
                    {
                        if (ret.Tables[0].Columns[j].ColumnName != "ID")
                        {
                            nr[ret.Tables[0].Columns[j].ColumnName] = ret.Tables[0].Rows[i][ret.Tables[0].Columns[j].ColumnName];
                        }
                    }
                    var SQL_ret = sfcdb.ExecSQL(nr.GetInsertString(DB_TYPE_ENUM.Oracle));
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                //StationReturn.Data = retMessage;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void GetDCNSNDataFromOlddb(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string strSN = Data["SN"].ToString();

                var SNs = strSN.Split(new string[] { ",", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                string retMessage = "";
                for (int i = 0; i < SNs.Count(); i++)
                {
                    try
                    {
                        sfcdb.BeginTrain();
                        GetDataFromDCNOldDB(SNs[i], "TEST", sfcdb);
                        sfcdb.CommitTrain();
                    }
                    catch (Exception ee)
                    {
                        sfcdb.RollbackTrain();
                        retMessage += SNs[i] + ":" + ee.Message + "\r\n";
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = retMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void TranDCNSNDataToOlddb(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string strSN = Data["SN"].ToString();

                var SNs = strSN.Split(new string[] { ",", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                string retMessage = "";
                for (int i = 0; i < SNs.Count(); i++)
                {
                    try
                    {
                        sfcdb.BeginTrain();
                        TranDataToDCNOldDB(SNs[i], this.BU, sfcdb);
                        sfcdb.CommitTrain();
                    }
                    catch (Exception ee)
                    {
                        sfcdb.RollbackTrain();
                        retMessage += SNs[i] + ":" + ee.Message + "\r\n";
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = retMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public static void GetDataFromDCNOldDB(string SN, string BU, OleExec SFCDB, OleExec _OldDB = null, List<string> ProccessedSN = null)
        {

            if (ProccessedSN == null)
            {
                ProccessedSN = new List<string>();
            }
            SN = SN.Trim().ToUpper();
            OleExec OldDB = null;
            if (_OldDB == null)
            {
                OldDB = new OleExec("server =10.120.176.101,3000; uid = dcnadmin; pwd = nsdiisfc169!; database = sjefox", SqlSugar.DbType.SqlServer);
            }
            else
            {
                OldDB = _OldDB;
            }

            var Inf1 = OldDB.RunSelect($@"select workorderno WO, skuno from [dbo].[mfsysproduct] where sysserialno ='{SN}'").Tables[0];
            string strSku = Inf1.Rows[0]["skuno"].ToString();
            //SKU Sku = new SKU();
            //Sku.Init(Inf1.Rows[0]["skuno"].ToString(), SFCDB, DB_TYPE_ENUM.Oracle);
            var inf2 = OldDB.RunSelect($@"select * from [dbo].[mfworkstatus] where sysserialno ='{SN}'").Tables[0];
            var woinfo = OldDB.RunSelect($@"select * from mfworkorder where workorderno ='{Inf1.Rows[0]["WO"].ToString()}'").Tables[0];

            var wot = SFCDB.RunSelect($@"select * from r_wo_base where workorderno='{Inf1.Rows[0]["WO"].ToString()}' ").Tables[0];
            var WoInf = woinfo.Rows[0];
            if (wot.Rows.Count == 0)
            //抓取工单信息
            {

                R_WO_BASE WOT = new R_WO_BASE()
                {
                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_WO_BASE"),
                    WORKORDERNO = WoInf["WORKORDERNO"].ToString(),
                    CLOSED_FLAG = "1",
                    CLOSE_DATE = (DateTime)WoInf["closedate"],
                    WORKORDER_QTY = Int32.Parse(WoInf["workorderqty"].ToString()),
                    FINISHED_QTY = Int32.Parse(WoInf["finishedqty"].ToString()),
                    SKUNO = WoInf["skuno"].ToString(),
                    ROUTE_ID = ""
                };
                SFCDB.ORM.Insertable(WOT).ExecuteCommand();
            }

            var WO = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WoInf["WORKORDERNO"].ToString()).First();
            var rsn = inf2.Rows[0];
            var snt = SFCDB.RunSelect($@"select * from R_SN where SN = '{SN}'").Tables[0];
            if (snt.Rows.Count > 0)
            {
                throw new Exception($@"{SN} is in MESDB ");
            }

            var r_sn = new R_SN()
            {
                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN"),
                SN = SN,
                SKUNO = strSku,
                VALID_FLAG = "1",
                ROUTE_ID = WO.ROUTE_ID,
                WORKORDERNO = WO.WORKORDERNO,
                CURRENT_STATION = rsn["currentevent"].ToString(),
                NEXT_STATION = rsn["nextevent"].ToString(),
                SHIPPED_FLAG = (bool)rsn["shipped"] ? "1" : "0",
                SHIPDATE = rsn["shipdate"]?.ObjToDate(),
                EDIT_EMP = rsn["lasteditby"].ToString(),
                EDIT_TIME = rsn["lasteditdt"]?.ObjToDate()
            };
            SFCDB.ORM.Insertable(r_sn).ExecuteCommand();

            var KP_INFO = OldDB.RunSelect($@"select '' ID,'' R_SN_ID,sysserialno SN ,cserialno value , partno ,categoryname kp_name,prodcategoryname scantype,mpn,
	1 ITEMSEQ , 1 SCANSEQ , 1 DETAILSEQ,eventpoint station,'' regex,1 valid_flag,'EEECODE' exkey1,EEECODE exvalue1,null extkey2,null extvalue2,lasteditdt EDIT_TIME , lasteditby EDIT_EMP from mfsyscserial m where sysserialno in 
	(
	'{SN}'
	)").Tables[0];
            for (int i = 0; i < KP_INFO.Rows.Count; i++)
            {
                var kpr = KP_INFO.Rows[i];
                if (OldDB.RunSelect($@"select 1 from mfworkstatus where sysserialno='{kpr["value"].ToString()}'").Tables[0].Rows.Count > 0)
                {
                    if (!ProccessedSN.Contains(kpr["value"].ToString()))
                    {
                        ProccessedSN.Add(kpr["value"].ToString());
                        try
                        {
                            GetDataFromDCNOldDB(kpr["value"].ToString(), BU, SFCDB, _OldDB, ProccessedSN);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                var r_sn_kp = new R_SN_KP()
                {
                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_KP"),
                    SN = r_sn.SN,
                    R_SN_ID = r_sn.ID,
                    DETAILSEQ = 1,
                    KP_NAME = kpr["kp_name"].ToString(),
                    VALUE = kpr["VALUE"].ToString(),
                    MPN = kpr["mpn"].ToString(),
                    PARTNO = kpr["partno"].ToString(),
                    SCANSEQ = i,
                    ITEMSEQ = i,
                    SCANTYPE = kpr["scantype"].ToString(),
                    STATION = kpr["station"].ToString(),
                    VALID_FLAG = 1,
                    EDIT_EMP = kpr["edit_emp"].ToString(),
                    EDIT_TIME = kpr["edit_time"].ObjToDate(),
                    //EXKEY1 = kpr["EXKEY1"].ToString(),
                    //EXVALUE1 = kpr["EXVALUE1"].ToString().Trim(),
                    LOCATION = kpr["EXVALUE1"].ToString().Trim()

                };
                SFCDB.ORM.Insertable(r_sn_kp).ExecuteCommand();

            }
            if (SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.WSN == r_sn.SN).ToList().Count == 0)
            {
                var wwn = OldDB.RunSelect($@"select * from WWN_DATASHARING where wsn='{r_sn.SN}'").Tables[0];
                if (wwn.Rows.Count != 0)
                {
                    var w = wwn.Rows[0];
                    WWN_DATASHARING WWN = new WWN_DATASHARING()
                    {
                        ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "WWN_DATASHARING"),
                        WSN = w["WSN"].ToString().Trim(),
                        CSKU = w["CSKU"].ToString(),
                        SKU = w["SKU"].ToString(),
                        WWN = w["WWN"].ToString(),
                        CSSN = w["CSSN"].ToString().Trim(),
                        LASTEDITBY = w["LASTEDITBY"].ToString(),
                        LASTEDITDT = (DateTime)w["LASTEDITDT"],
                        MAC = w["MAC"].ToString(),
                        MACTB0 = w["MACTB0"].ToString(),
                        MACTB1 = w["MACTB1"].ToString(),
                        MACTB2 = w["MACTB2"].ToString(),
                        MACTB3 = w["MACTB3"].ToString(),
                        MACTB4 = w["MACTB4"].ToString(),
                        VSKU = w["VSKU"].ToString(),
                        VSSN = w["VSSN"].ToString().Trim(),
                        WWNTB0 = w["WWNTB0"].ToString(),
                        WWNTB1 = w["WWNTB1"].ToString(),
                        WWNTB2 = w["WWNTB2"].ToString(),
                        WWNTB3 = w["WWNTB3"].ToString(),
                        WWNTB4 = w["WWNTB4"].ToString(),

                    };
                    try
                    {
                        WWN.WWN_BLOCK_SIZE = double.Parse(w["WWN_BLOCK_SIZE"].ToString());
                    }
                    catch
                    { }
                    try
                    {
                        WWN.MAC_BLOCK_SIZE = double.Parse(w["MAC_BLOCK_SIZE"].ToString());
                    }
                    catch
                    { }
                    SFCDB.ORM.Insertable(WWN).ExecuteCommand();
                }

            }

            var tBrcd = OldDB.RunSelect($@"select * from r_test_brcd where sysserialno ='{SN}'").Tables[0];
            for (int i = 0; i < tBrcd.Rows.Count; i++)
            {
                var dr = tBrcd.Rows[i];
                R_TEST_RECORD RTR = new R_TEST_RECORD()
                {
                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_RECORD"),
                    DETAILTABLE = "R_TEST_BRCD",
                    TESTATION = dr["eventname"].ToString(),
                    R_SN_ID = r_sn.ID,
                    SN = r_sn.SN,
                    STATE = dr["status"].ToString(),
                    STARTTIME = (DateTime?)dr["testdate"],
                    EDIT_TIME = (DateTime?)dr["TATIME"],
                    EDIT_EMP = "AUTO TRAN",

                };

                R_TEST_BRCD RTB = new R_TEST_BRCD()
                {
                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_BRCD"),
                    SYSSERIALNO = dr["SYSSERIALNO"].ToString(),
                    TESTDATE = (DateTime?)dr["TESTDATE"],
                    EVENTNAME = dr["EVENTNAME"].ToString(),
                    STATUS = dr["STATUS"].ToString(),
                    PARTNO = dr["PARTNO"].ToString(),
                    SFPMPN = dr["SFPMPN"].ToString(),
                    LOCATION = dr["LOCATION"].ToString(),
                    SYMPTOM = dr["SYMPTOM"].ToString(),
                    PCBASN = dr["PCBASN"].ToString(),
                    PCBAPN = dr["PCBAPN"].ToString(),
                    VSN = dr["VSN"].ToString(),
                    VPN = dr["VPN"].ToString(),
                    CSN = dr["CSN"].ToString(),
                    CPN = dr["CPN"].ToString(),
                    FRUPCBASN = dr["FRUPCBASN"].ToString(),
                    TESTREPORTNAME = dr["FRUPCBASN"].ToString(),
                    LASTEDITDT = (DateTime?)dr["LASTEDITDT"],
                    LASTEDITBY = dr["LASTEDITBY"].ToString(),
                    FAILURECODE = dr["FAILURECODE"].ToString(),
                    TRAY_SN = dr["TRAY_SN"].ToString(),
                    TESTERNO = dr["TESTERNO"].ToString(),
                    TEMP4 = (double?)dr["TEMP4"]?.ObjToDecimal(),
                    TEMP5 = (double?)dr["TEMP5"]?.ObjToDecimal(),
                    TATIME = (DateTime?)dr["TATIME"],
                    R_TEST_RECORD_ID = RTR.ID
                };
                SFCDB.ORM.Insertable(RTR).ExecuteCommand();
                SFCDB.ORM.Insertable(RTB).ExecuteCommand();
            }

            try
            {



                if (!SFCDB.ORM.Queryable<R_SN_PACKING, R_SN>((rsp, rs) => rsp.SN_ID == rs.ID)
                    .Where((rsp, rs) => rs.SN == SN).Select((rsp, rs) => rsp)
                    .Any())
                {
                    var rsnpackobj = OldDB.ORM.Queryable<mfsysproduct>().Where(t => t.sysserialno == SN).ToList()
                        .FirstOrDefault();
                    if (rsnpackobj != null && rsnpackobj.location != "")
                    {
                        var packc = OldDB.ORM
                            .Queryable<sfcshippack, sfcshippack>((c, p) => new Object[]
                                {JoinType.Left, c.parentbundleno == p.packno})
                            .Where((c, p) => c.packno == rsnpackobj.location)
                            .Select((c, p) => new { c }).ToList().FirstOrDefault();

                        var packp = OldDB.ORM
                            .Queryable<sfcshippack, sfcshippack>((c, p) => new Object[]
                                {JoinType.Left, c.parentbundleno == p.packno})
                            .Where((c, p) => c.packno == rsnpackobj.location)
                            .Select((c, p) => new { p }).ToList().FirstOrDefault();
                        if (packc !=null && packp!=null) {
                            var pl = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packp.p.packno).ToList()
                                .FirstOrDefault();
                            ///PL
                            if (packp.p != null && pl == null)
                            {
                                pl = new R_PACKING()
                                {
                                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_PACKING"),
                                    STATION = "CARTON",
                                    IP = "SYSTEM",
                                    LINE = "Line1",
                                    PACK_NO = packp.p.packno,
                                    PACK_TYPE = "PALLET",
                                    PARENT_PACK_ID = "",
                                    SKUNO = r_sn.SKUNO,
                                    MAX_QTY = Convert.ToDouble(packp.p.defaultuomqty),
                                    QTY = Convert.ToDouble(packp.p.stuffingqty),
                                    CLOSED_FLAG = "1",
                                    EDIT_TIME = packp.p.lasteditdt,
                                    EDIT_EMP = packp.p.lasteditby
                                };
                                SFCDB.ORM.Insertable(pl).ExecuteCommand();
                            }

                            var carton = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packc.c.packno).ToList()
                                .FirstOrDefault();
                            //CARTON
                            if (carton == null)
                            {
                                carton = new R_PACKING()
                                {
                                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_PACKING"),
                                    STATION = "CARTON",
                                    IP = "SYSTEM",
                                    LINE = "Line1",
                                    PACK_NO = packc.c.packno,
                                    PACK_TYPE = "CARTON",
                                    PARENT_PACK_ID = pl.ID,
                                    SKUNO = r_sn.SKUNO,
                                    MAX_QTY = Convert.ToDouble(packc.c.defaultuomqty),
                                    QTY = Convert.ToDouble(packc.c.stuffingqty),
                                    CLOSED_FLAG = "1",
                                    EDIT_TIME = packc.c.lasteditdt,
                                    EDIT_EMP = packc.c.lasteditby
                                };
                                SFCDB.ORM.Insertable(carton).ExecuteCommand();
                            }

                            if (!SFCDB.ORM.Queryable<R_SN_PACKING>().Where(t => t.SN_ID == r_sn.ID).Any())
                            {
                                SFCDB.ORM.Insertable(new R_SN_PACKING()
                                {
                                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_PACKING"),
                                    SN_ID = r_sn.ID,
                                    PACK_ID = carton.ID,
                                    EDIT_TIME = packc.c.lasteditdt,
                                    EDIT_EMP = packc.c.lasteditby
                                }).ExecuteCommand();
                            }

                        }
                    }
                }

                ///sfcCodeLike
                var skuobj = OldDB.ORM.Queryable<sfcCodeLike>().Where(t => t.SkuNo == r_sn.SKUNO).ToList()
                    .FirstOrDefault();
                if (skuobj != null)
                {
                    var skumes = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == skuobj.SkuNo).ToList()
                        .FirstOrDefault();
                    if (skumes == null)
                    {
                        SFCDB.ORM.Insertable(new C_SKU()
                        {
                            ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "C_SKU"),
                            BU = "DCN",
                            SKUNO = skuobj.SkuNo,
                            VERSION = skuobj.VERSION,
                            C_SERIES_ID = "DCN00000000000000000000000000000P",
                            CUST_PARTNO = skuobj.CustPartNo,
                            SKU_TYPE = skuobj.category
                        }).ExecuteCommand();
                    }
                }

                var assobj = OldDB.ORM.Queryable<asShipped>().Where(t => t.topasssn == r_sn.SN).ToList()
                    .FirstOrDefault();

                if (assobj != null)
                {
                    //R_DN_BROCADE
                    var rdnstatus = OldDB.ORM.Queryable<R_DN_BROCADE>()
                        .Where(t => t.SHIP_ORDER == assobj.shiporderno).ToList().FirstOrDefault();
                    ////R_DN_BROCADE
                    //var rdnstatus = OldDB.ORM.Queryable<R_DN_BROCADE, asShipped>((dn, ass) => dn.SHIPMENTNO == ass.shiporderno)
                    //    .Where((dn, ass) => ass.topasssn == r_sn.SN).Select((dn, ass) => dn).ToList().FirstOrDefault();


                    if (rdnstatus != null)
                    {
                        var dnline = (rdnstatus.PAKLINE_NO * 10).ToString().PadLeft(6, '0');
                        var dnmes = SFCDB.ORM.Queryable<R_DN_STATUS>()
                            .Where(t => t.DN_NO == rdnstatus.DN_NO && t.DN_LINE == dnline).ToList()
                            .FirstOrDefault();
                        if (dnmes == null)
                        {
                            dnmes = new R_DN_STATUS()
                            {
                                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_DN_STATUS"),
                                DN_NO = rdnstatus.DN_NO,
                                DN_LINE = (rdnstatus.PAKLINE_NO * 10).ToString().PadLeft(6, '0'),
                                PO_NO = rdnstatus.PO_NO,
                                PO_LINE = rdnstatus.POLINE,
                                //SO_NO = rdnstatus.
                                SKUNO = rdnstatus.SKUNO,
                                QTY = rdnstatus.QTY,
                                GT_FLAG = "1",
                                DN_FLAG = "3",
                                GTEVENT = "END"
                            };
                            SFCDB.ORM.Insertable(dnmes).ExecuteCommand();
                        }


                        var todetialobj = OldDB.ORM.Queryable<DcnSfcModel.R_TO_DETAIL>()
                            .Where(t => t.DN_NO == rdnstatus.DN_NO).ToList().FirstOrDefault();
                        if (todetialobj != null)
                        {
                            var todetailmes = SFCDB.ORM.Queryable<MESDataObject.Module.R_TO_DETAIL>().Where(t => t.DN_NO == rdnstatus.DN_NO)
                                .ToList()
                                .FirstOrDefault();
                            if (todetailmes == null)
                            {
                                SFCDB.ORM.Insertable(new MESDataObject.Module.R_TO_DETAIL()
                                {
                                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TO_DETAIL"),
                                    TO_NO = todetialobj.TO_NO,
                                    TO_ITEM_NO = todetialobj.TO_ITEM_NO,
                                    DN_NO = todetialobj.DN_NO,
                                    DN_CUSTOMER = todetialobj.DN_CUSTOMER
                                }).ExecuteCommand();
                            }
                        }

                        var assshipobj = OldDB.ORM.Queryable<DcnSfcModel.asShipped>().Where(t => t.topasssn == r_sn.SN)
                            .ToList().FirstOrDefault();
                        if (assshipobj != null)
                        {
                            var shipdetailmes = SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(t => t.ID == r_sn.ID)
                                .ToList()
                                .FirstOrDefault();
                            if (shipdetailmes == null)
                            {
                                SFCDB.ORM.Insertable(new R_SHIP_DETAIL()
                                {
                                    ID = r_sn.ID,
                                    SN = r_sn.SN,
                                    SKUNO = r_sn.SKUNO,
                                    DN_NO = dnmes.DN_NO,
                                    DN_LINE = (rdnstatus.PAKLINE_NO * 10).ToString().PadLeft(6, '0'),
                                    SHIPDATE = assshipobj.shipdate,
                                    SHIPORDERID = dnmes.ID
                                }).ExecuteCommand();
                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void TranDataToDCNOldDB(string SN, string BU, OleExec SFCDB, OleExec _OldDB = null, List<string> ProccessedSN = null)
        {

            if (ProccessedSN == null)
            {
                ProccessedSN = new List<string>();
            }
            SN = SN.Trim().ToUpper();
            OleExec OldDB = null;
            if (_OldDB == null)
            {
                OldDB = new OleExec("server =10.120.176.101,3000; uid = dcnadmin; pwd = nsdiisfc169!; database = sjefox", SqlSugar.DbType.SqlServer);
                //OldDB = new OleExec("server =10.120.246.232,3000; uid = dcnadmin; pwd = nsdiisfc169!; database = sjefox", SqlSugar.DbType.SqlServer);
            }
            else
            {
                OldDB = _OldDB;
            }
            var r_sn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").First();
            var r_wo = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == r_sn.WORKORDERNO).First();
            var r_sn_kp = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.VALID_FLAG == 1).ToList();
            var test_brcd = SFCDB.ORM.Queryable<R_TEST_BRCD>().Where(t => t.SYSSERIALNO == SN).ToList();
            var wwn_dt = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.WSN == SN || t.VSSN == SN || t.CSSN == SN).ToList();
            C_SKU c_sku = null;
            if (r_wo != null)
            {
                c_sku = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == r_wo.SKUNO && t.VERSION == r_wo.SKU_VER).First();
            }

            var mfws = OldDB.ORM.Queryable<mfworkstatus>().Where(t => t.sysserialno == SN).First();
            if (mfws == null)
            {
                mfws = new mfworkstatus()
                {
                    sysserialno = r_sn.SN,
                    assigndate = r_sn.START_TIME.HasValue ? r_sn.START_TIME.Value : DateTime.Parse("1900-01-01"),
                    workorderno = r_sn.WORKORDERNO,
                    factoryid = BU,
                    productionline = "Line1",
                    shift = "Shift1",
                    packed = r_sn.PACKED_FLAG == "1" ? true : false,
                    started = true,
                    startdate = r_sn.START_TIME.HasValue ? r_sn.START_TIME.Value : DateTime.Parse("1900-01-01"),
                    completed = r_sn.COMPLETED_FLAG == "1" ? true : false,
                    completedate = r_sn.COMPLETED_TIME.HasValue ? r_sn.COMPLETED_TIME.Value : DateTime.Parse("1900-01-01"),
                    Quited = false,
                    QuitDate = DateTime.Parse("1900-01-01"),
                    shipped = false,
                    shipdate = r_sn.SHIPDATE.HasValue ? r_sn.SHIPDATE.Value : DateTime.Parse("1900-01-01"),
                    repairheld = false,
                    repairdate = DateTime.Parse("1900-01-01"),
                    ORT_FAIL_TIME = DateTime.Parse("1900-01-01"),
                    ORT_IN_TIME = DateTime.Parse("1900-01-01"),
                    ORT_OUT_TIME = DateTime.Parse("1900-01-01"),
                    ReFlowTime = DateTime.Parse("1900-01-01"),
                    packdate = r_sn.PACKDATE.HasValue ? r_sn.PACKDATE.Value : DateTime.Parse("1900-01-01"),
                    stockintime = DateTime.Parse("1900-01-01"),
                    stockouttime = DateTime.Parse("1900-01-01"),
                    lasteditdt = r_sn.EDIT_TIME.HasValue ? r_sn.EDIT_TIME.Value : DateTime.Parse("1900-01-01"),
                    lasteditby = r_sn.EDIT_EMP,
                    field1 = 0,
                    currentevent = "STOCKIN",
                    nextevent = "JOBFINISH",
                    ReFlow = false,
                    ORT_COUNT = 0,
                    ORT_FLAG = false,
                    ORT_OUTFLAG = false,
                    stockstatus = ""
                };
                OldDB.ORM.Insertable(mfws).ExecuteCommand();
            }

            var mfsp = OldDB.ORM.Queryable<mfsysproduct>().Where(t => t.sysserialno == SN).First();
            if (mfsp == null)
            {
                mfsp = new mfsysproduct()
                {
                    sysserialno = r_sn.SN,
                    workorderno = r_sn.WORKORDERNO,
                    skuno = r_sn.SKUNO,
                    custpartno = r_sn.SKUNO,
                    eeecode = r_sn.WORKORDERNO,
                    routeid = r_sn.ROUTE_ID,
                    factoryid = BU,
                    shipped = false,
                    Reseat = false,
                    ReseatTag = 0,
                    shipdate = r_sn.SHIPDATE.HasValue ? r_sn.SHIPDATE.Value : DateTime.Parse("1900-01-01"),
                    lasteditdt = r_sn.EDIT_TIME.HasValue ? r_sn.EDIT_TIME.Value : DateTime.Parse("1900-01-01"),
                    lasteditby = r_sn.EDIT_EMP
                };
                OldDB.ORM.Insertable(mfsp).ExecuteCommand();
            }

            var mfsc = OldDB.ORM.Queryable<DcnSfcModel.mfsyscserial>().Where(t => t.sysserialno == SN).ToList();
            if (mfsc.Count == 0)
            {
                //新增判斷MES系統是否有綁定關係 Add By ZHB 2020年8月12日15:09:24
                if (r_sn_kp.Count > 0)
                {
                    mfsc = new List<DcnSfcModel.mfsyscserial>();
                    r_sn_kp.ForEach(item =>
                    {
                        var ee = item.KP_NAME == null ? item.LOCATION : item.KP_NAME.Contains("/") ? item.KP_NAME.Substring(0, item.KP_NAME.Length - 3) : item.KP_NAME;
                        var tt = mfsc.FindAll(t => t.eeecode.Contains(ee));
                        var eeecode = item.KP_NAME == null ? ee.Trim() : ee.Trim() + "-" + (tt.Count + 1).ToString();
                        var temp1 = new DcnSfcModel.mfsyscserial()
                        {
                            sysserialno = SN,
                            cserialno = item.VALUE == null ? "" : item.VALUE,
                            eventpoint = item.STATION,
                            custpartno = item.PARTNO,
                            eeecode = eeecode,
                            partno = item.PARTNO,
                            seqno = (int)(item.ITEMSEQ.HasValue ? item.ITEMSEQ.Value : 0),
                            categoryname = item.KP_NAME,
                            prodcategoryname = item.SCANTYPE,
                            prodtype = "",
                            OriginalCSN = item.SN,
                            scanby = item.EDIT_EMP,
                            scandt = item.EDIT_TIME.HasValue ? item.EDIT_TIME.Value : DateTime.Parse("1900-01-01"),
                            lasteditby = item.EDIT_EMP,
                            lasteditdt = item.EDIT_TIME.HasValue ? item.EDIT_TIME.Value : DateTime.Parse("1900-01-01"),
                            MDSGet = false
                        };
                        mfsc.Add(temp1);

                    });
                    OldDB.ORM.Insertable(mfsc).ExecuteCommand();
                }
            }

            var mfscp = OldDB.ORM.Queryable<mfsyscomponent>().Where(t => t.sysserialno == SN).ToList();
            if (mfscp.Count == 0)
            {
                mfscp = new List<mfsyscomponent>();
                var temp1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.STATION != "SMT1" && t.STATION != "SMT2" && t.STATION != "AOI1" && t.STATION != "AOI2" && t.VALID_FLAG == 1)
                    .GroupBy(t => new { t.SN, t.PARTNO, t.KP_NAME, t.SCANTYPE, t.STATION })
                    .Select(t => new { t.SN, t.PARTNO, t.KP_NAME, t.SCANTYPE, t.STATION, QTY = SqlFunc.AggregateCount(1) })
                    .ToList();
                //新增判斷MES系統是否有綁定關係 Add By ZHB 2020年8月12日15:09:24
                if (temp1.Count > 0)
                {
                    temp1.ForEach((item) =>
                    {
                        var ee = item.KP_NAME.Contains("/") ? item.KP_NAME.Substring(0, item.KP_NAME.Length - 3) : item.KP_NAME;
                        var eeecode = ee.Trim();
                        var temp2 = new mfsyscomponent()
                        {
                            sysserialno = SN,
                            partno = item.PARTNO,
                            qty = item.QTY,
                            custpartno = item.PARTNO,
                            keypart = true,
                            installed = true,
                            installedqty = item.QTY,
                            eeecode = item.STATION,
                            categoryname = item.KP_NAME,
                            prodcategoryname = item.KP_NAME,
                            noreplacepart = false,
                            lasteditdt = DateTime.Parse("1900-01-01")
                        };
                        mfscp.Add(temp2);

                    });
                    OldDB.ORM.Insertable(mfscp).ExecuteCommand();
                }
            }

            var rtb = OldDB.ORM.Queryable<R_test_BRCD>().Where(t => t.sysserialno == SN).ToList();
            if (rtb.Count == 0)
            {
                //新增判斷MES系統是否有測試記錄 Add By ZHB 2020年8月12日15:09:24
                if (test_brcd.Count > 0)
                {
                    rtb = new List<R_test_BRCD>();
                    test_brcd.ForEach((item) =>
                    {
                        var temp1 = new DcnSfcModel.R_test_BRCD()
                        {
                            sysserialno = item.SYSSERIALNO,
                            testdate = item.TESTDATE.Value,
                            eventname = item.EVENTNAME,
                            status = item.STATUS,
                            partno = item.PARTNO,
                            sfpmpn = item.SFPMPN,
                            location = item.LOCATION,
                            symptom = item.SYMPTOM,
                            pcbasn = item.PCBASN,
                            pcbapn = item.PCBAPN,
                            vsn = item.VSN,
                            vpn = item.VPN,
                            csn = item.CSN,
                            cpn = item.CPN,
                            frupcbasn = item.FRUPCBASN,
                            testreportname = item.TESTREPORTNAME,
                            lasteditdt = item.LASTEDITDT.HasValue ? item.LASTEDITDT.Value : DateTime.Parse("1900-01-01"),
                            lasteditby = item.LASTEDITBY,
                            failurecode = item.FAILURECODE,
                            Tray_SN = item.TRAY_SN,
                            TesterNO = item.TESTERNO,
                            temp4 = item.TEMP4.HasValue ? (decimal)item.TEMP4.Value : new decimal(),
                            temp5 = item.TEMP5.HasValue ? (decimal)item.TEMP5.Value : new decimal(),
                            TATIME = item.TATIME.Value
                        };
                        rtb.Add(temp1);
                    });
                    OldDB.ORM.Insertable(rtb).ExecuteCommand();
                }
            }

            var wwn = OldDB.ORM.Queryable<WWN_Datasharing>().Where(t => t.WSN == SN || t.VSSN == SN || t.CSSN == SN).ToList();
            if (wwn.Count == 0)
            {
                //新增判斷MES系統是否有WWN表記錄 Add By ZHB 2020年8月12日15:09:24
                if (wwn_dt.Count > 0)
                {
                    wwn = new List<WWN_Datasharing>();
                    wwn_dt.ForEach((item) =>
                    {
                        var temp1 = new WWN_Datasharing()
                        {
                            WSN = item.WSN,
                            SKU = item.SKU,
                            VSSN = item.VSSN,
                            VSKU = item.VSKU,
                            CSSN = item.CSSN,
                            CSKU = item.CSKU,
                            MAC = item.MAC,
                            WWN = item.WWN,
                            MAC_block_size = item.MAC_BLOCK_SIZE.HasValue ? (int)item.MAC_BLOCK_SIZE.Value : 0,
                            WWN_block_size = item.WWN_BLOCK_SIZE.HasValue ? (int)item.WWN_BLOCK_SIZE.Value : 0,
                            lasteditby = item.LASTEDITBY,
                            lasteditdt = item.LASTEDITDT.HasValue ? item.LASTEDITDT.Value : DateTime.Parse("1900-01-01"),
                            MACTB0 = item.MACTB0,
                            MACTB1 = item.MACTB1,
                            MACTB2 = item.MACTB2,
                            MACTB3 = item.MACTB3,
                            MACTB4 = item.MACTB4,
                            WWNTB0 = item.WWNTB0,
                            WWNTB1 = item.WWNTB1,
                            WWNTB2 = item.WWNTB2,
                            WWNTB3 = item.WWNTB3,
                            WWNTB4 = item.WWNTB4
                        };
                        wwn.Add(temp1);
                    });
                    OldDB.ORM.Insertable(wwn).ExecuteCommand();
                }
            }

            if (r_wo != null)
            {
                var mfwo = OldDB.ORM.Queryable<mfworkorder>().Where(t => t.workorderno == r_wo.WORKORDERNO).First();
                if (mfwo == null)
                {
                    mfwo = new mfworkorder()
                    {
                        workorderno = r_wo.WORKORDERNO,
                        factoryid = r_wo.PLANT,
                        workorderdate = r_wo.DOWNLOAD_DATE.HasValue ? r_wo.DOWNLOAD_DATE.Value : DateTime.Parse("1900-01-01"),
                        scheduledate = r_wo.RELEASE_DATE.HasValue ? r_wo.RELEASE_DATE.Value : DateTime.Parse("1900-01-01"),
                        //WorkRouteType = "",
                        WorkOrderType = r_wo.WO_TYPE,
                        productiontype = r_wo.PRODUCTION_TYPE,
                        skuno = r_wo.SKUNO,
                        skuversion = r_wo.SKU_VER,
                        skuname = c_sku.SKU_NAME,
                        skudesc = c_sku.DESCRIPTION,
                        custpartno = c_sku.CUST_PARTNO,
                        //custpartDesc="",
                        //custmodelno="",
                        customerid = c_sku.SERIES_NAME,
                        //firmware="",
                        //eeecode="",
                        productfamily = "PROD",
                        productlevel = "DEFAULT",
                        productcolor = r_wo.START_STATION,
                        //productlangulage="",
                        prioritycode = "99",
                        //shipcountry="",
                        routeid = r_wo.ROUTE_ID,
                        //orderno="",
                        //custpono="",
                        //compcode="",
                        released = true,
                        releaseddate = r_wo.RELEASE_DATE.HasValue ? r_wo.RELEASE_DATE.Value : DateTime.Parse("1900-01-01"),
                        jobstarted = true,
                        startdate = r_wo.DOWNLOAD_DATE.HasValue ? r_wo.DOWNLOAD_DATE.Value : DateTime.Parse("1900-01-01"),
                        closed = Convert.ToInt32(r_wo.CLOSED_FLAG) == 0 ? false : true,
                        closedate = r_wo.CLOSE_DATE.HasValue ? r_wo.CLOSE_DATE.Value : DateTime.Parse("1900-01-01"),
                        workorderqty = (int)r_wo.WORKORDER_QTY,
                        finishedqty = (int)r_wo.FINISHED_QTY,
                        ScrapedQty = (int)r_wo.SCRAPED_QTY,
                        //stockinrequestqty=0,
                        //stockinprocessqty=0,
                        //batchno="",
                        batchdate = DateTime.Parse("1900-01-01"),
                        //batchseqno=0,
                        //jobnote1="",
                        //RMANo="",
                        //RMALineNo="",
                        //custpartversion="",
                        //packageno=0,
                        //OrderLineNo="",
                        lasteditby = r_wo.EDIT_EMP,
                        lasteditdt = r_wo.EDIT_TIME.HasValue ? r_wo.EDIT_TIME.Value : DateTime.Parse("1900-01-01")
                    };
                    OldDB.ORM.Insertable(mfwo).ExecuteCommand();
                }
            }
        }

    }
}

