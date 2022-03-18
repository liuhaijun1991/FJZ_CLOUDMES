using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject;
using MESDataObject.Common;
using System.Data;
using MESDBHelper;
using MESStation.LogicObject;
using MESDataObject.Module.Juniper;
using MESPubLab.MESStation.MESReturnView.Station;
using MESPubLab.MESStation.SNMaker;
using Oracle.ManagedDataAccess.Client;
using MESPubLab.SAP_RFC;
using MESStation.Config;
using MESPubLab.MesClient;
using MESDataObject.Module.OM;
using System.Text.RegularExpressions;
using MESPubLab.Json;
using MESStation.Interface.Juniper;
using MESPubLab.MESInterface;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class JuniperAction
    {

        public static void ReplaceSNAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var SFCDB = Station.SFCDB;
            var skuno = SkuSession.Value.ToString();
            var sn = SNSession.Value.ToString();
            var config = SFCDB.ORM.Queryable<C_SKU_DETAIL>()
                .Where(t => t.CATEGORY == "REPLACE_SN_ACTION" && t.CATEGORY_NAME == "REPLACE_SN_ACTION" && t.SKUNO == skuno).First();

            if (config == null)
            {
                return;
            }
            if (config.STATION_NAME != Station.StationName)
            {
                return;
            }

            try
            {
                SNmaker.CkeckSNRule(sn, config.VALUE, SFCDB);
                return;
            }
            catch
            {

            }
            UIInputData O = new UIInputData() { Timeout = 50000, IconType = IconType.None, Message = "Plase Scan New SN ", Tittle = "SN Replace", Type = UIInputType.String, Name = "NewSN", ErrMessage = "No Input Data" };
            var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station);
            var newSN = ret1.ToString();
            SNmaker.CkeckSNRule(newSN, config.VALUE, SFCDB);
            var extSN = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == newSN && t.VALID_FLAG == "1").First();
            if (extSN != null)
            {
                //"{0}  already exists"
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814182042", new string[] { newSN });
                throw new MESReturnMessage(ErrMessage);
            }

            var SNbaseData = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
            SNbaseData.SN = newSN;
            SFCDB.ORM.Updateable(SNbaseData).Where(t => t.ID == SNbaseData.ID).ExecuteCommand();

            var kps = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == SNbaseData.ID).ToList();
            for (int i = 0; i < kps.Count; i++)
            {
                kps[i].SN = newSN;
                SFCDB.ORM.Updateable(kps[i]).Where(t => t.ID == kps[i].ID).ExecuteCommand();
            }

            var eventRecord = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.R_SN_ID == SNbaseData.ID).ToList();
            for (int i = 0; i < eventRecord.Count; i++)
            {
                eventRecord[i].SN = newSN;
                SFCDB.ORM.Updateable(eventRecord[i]).Where(t => t.ID == eventRecord[i].ID).ExecuteCommand();
            }

            var testRecord = SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.R_SN_ID == SNbaseData.ID).ToList();
            for (int i = 0; i < testRecord.Count; i++)
            {
                testRecord[i].SN = newSN;
                SFCDB.ORM.Updateable(testRecord[i]).Where(t => t.ID == testRecord[i].ID).ExecuteCommand();
            }
            SN SNObj = null;
            SNObj = new SN(newSN, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (string.IsNullOrEmpty(SNObj.SerialNo))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + newSN }));
            }
            Input.Value = SNObj.baseSN.SN;
            SNSession.Value = SNObj;

        }

        public static void AddTestFailRecordAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            Int16 FailCount = 0;
            string StrSn = "";
            string R_SN_STATION_DETAIL_ID = "";
            List<Dictionary<string, string>> FailList = null;

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailListSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[3].VALUE, SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                //      Station.StationSession.Add(StatusSession);
                if (StatusSession.Value == null)
                {
                    StatusSession.Value = "FAIL";
                }
                Station.StationSession.Add(StatusSession);
            }
            var SN = (SN)(SNSession.Value);
            var strSN = SNSession.Value.ToString();

            var SFCDB = Station.SFCDB;
            var wo = new WorkOrder();
            wo.Init(SN.WorkorderNo, SFCDB, DB_TYPE_ENUM.Oracle);
            var ConfigPara = Paras.FindAll(t => t.SESSION_TYPE.ToUpper() == "CONFIG");
            Dictionary<string, string> Config = new Dictionary<string, string>();
            for (int i = 0; i < ConfigPara.Count; i++)
            {
                var data = ConfigPara[i].VALUE.Split(new char[] { ',', ':' });
                Config.Add(data[0], data[1]);
            }

            if (Config.ContainsKey(Station.StationName))
            {
                var StationName = Station.StationName;
                var TestStation = Config[StationName];
                FailCount = Convert.ToInt16(FailCountSession.Value.ToString());
                FailList = (List<Dictionary<string, string>>)FailListSession.Value;
                if (FailList.Count >= FailCount && FailCount != 0) //允許掃描多個Fail
                {
                    StrSn = SNSession.InputValue.ToString();
                    string repairMainId = "";
                    for (int i = 0; i < FailList.Count; i++)
                    {
                        //獲取頁面傳過來的數據
                        string failCode = FailList[i]["FailCode"].ToString();
                        string failLocation = FailList[i]["FailLocation"].ToString();
                        string failProcess = FailList[i]["FailProcess"].ToString();
                        string failDescription = FailList[i]["FailDesc"].ToString();

                        var testcount = SFCDB.ORM.Queryable<R_TEST_JUNIPER>().Where(t => t.SYSSERIALNO == strSN && t.EVENTNAME == TestStation).Count();
                        //最少為1，次數加1
                        testcount++;
                        R_TEST_JUNIPER tj = new R_TEST_JUNIPER()
                        {
                            ID = MesDbBase.GetNewID(SFCDB.ORM, Station.BU, "R_TEST_JUNIPER"),
                            EVENTNAME = TestStation,
                            SYSSERIALNO = strSN,
                            SERIAL_NUMBER = strSN,
                            PART_NUMBER = SN.SkuNo,
                            CAPTURE_TIME = DateTime.Now,
                            PART_NUMBER_REVISION = wo.SKU_VER,
                            STATUS = "FAIL",
                            TATIME = DateTime.Now,
                            TESTERNO = Station.LoginUser.EMP_NO,
                            TESTDATE = DateTime.Now,
                            TEST_RESULT = "FAIL",
                            TEST_STATION_NAME = TestStation,
                            TEST_START_TIMESTAMP = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            R_TEST_RECORD_ID = MesDbBase.GetNewID(SFCDB.ORM, Station.BU, "R_TEST_RECORD"),
                            FAILCODE = failCode,
                            CM_ODM_PARTNUMBER = SN.SkuNo,
                            PHASE = "PRODUCTION",
                            TEST_STATION_NUMBER = Station.Line + Station.StationName,
                            TEST_STEP = StationName,
                            TEST_CYCLE_TEST_LOOP = testcount.ToString(),
                            LOAD_DATE = DateTime.Now

                        };
                        tj.UNIQUE_TEST_ID = tj.R_TEST_RECORD_ID;
                        R_TEST_RECORD tr = new R_TEST_RECORD()
                        {
                            ID = tj.R_TEST_RECORD_ID,
                            MESSTATION = StationName,
                            DETAILTABLE = "R_TEST_JUNIPER",
                            R_SN_ID = SN.baseSN.ID,
                            SN = strSN,
                            EDIT_TIME = DateTime.Now,
                            STARTTIME = DateTime.Now,
                            ENDTIME = DateTime.Now,
                            TESTATION = TestStation,
                            STATE = "FAIL",
                            EDIT_EMP = Station.LoginUser.EMP_NO,
                        };

                        SFCDB.ORM.Insertable(tj).ExecuteCommand();
                        SFCDB.ORM.Insertable(tr).ExecuteCommand();
                        break;//只寫第一個不良
                    }
                }

            }

        }

        public static void AddTestPassRecordAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            try
            {
                var SFCDB = Station.SFCDB;
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new Exception($@"Can't Find SNSession (type:{Paras[0].SESSION_TYPE},key:{Paras[0].SESSION_KEY})");
                }
                var SN = (SN)(SNSession.Value);
                var strSN = SNSession.Value.ToString();
                var ConfigPara = Paras.FindAll(t => t.SESSION_TYPE.ToUpper() == "CONFIG");
                Dictionary<string, string> Config = new Dictionary<string, string>();
                for (int i = 0; i < ConfigPara.Count; i++)
                {
                    var data = ConfigPara[i].VALUE.Split(new char[] { ',', ':' });
                    Config.Add(data[0], data[1]);
                }

                if (Config.ContainsKey(Station.StationName))
                {
                    var StationName = Station.StationName;
                    var TestStation = Config[StationName];

                    var wo = new WorkOrder();
                    //wo.Initwo(SN.WorkorderNo, SFCDB, DB_TYPE_ENUM.Oracle);
                    wo.Init(SN.WorkorderNo, SFCDB, DB_TYPE_ENUM.Oracle);
                    var testcount = SFCDB.ORM.Queryable<R_TEST_JUNIPER>().Where(t => t.SYSSERIALNO == strSN && t.EVENTNAME == TestStation).Count();
                    testcount++;
                    R_TEST_JUNIPER tj = new R_TEST_JUNIPER()
                    {
                        ID = MesDbBase.GetNewID(SFCDB.ORM, Station.BU, "R_TEST_JUNIPER"),
                        EVENTNAME = TestStation,
                        SYSSERIALNO = strSN,
                        SERIAL_NUMBER = strSN,
                        PART_NUMBER = SN.SkuNo,
                        CAPTURE_TIME = DateTime.Now,
                        PART_NUMBER_REVISION = wo.SKU_VER,
                        STATUS = "PASS",
                        TATIME = DateTime.Now,
                        TESTERNO = Station.LoginUser.EMP_NO,
                        TESTDATE = DateTime.Now,
                        TEST_RESULT = "PASS",
                        TEST_STATION_NAME = TestStation,
                        TEST_START_TIMESTAMP = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        R_TEST_RECORD_ID = MesDbBase.GetNewID(SFCDB.ORM, Station.BU, "R_TEST_RECORD"),
                        CM_ODM_PARTNUMBER = SN.SkuNo,
                        PHASE = "PRODUCTION",
                        TEST_STATION_NUMBER = Station.Line + Station.StationName,
                        TEST_STEP = StationName,
                        TEST_CYCLE_TEST_LOOP = testcount.ToString(),
                        LOAD_DATE = DateTime.Now

                    };
                    tj.UNIQUE_TEST_ID = tj.R_TEST_RECORD_ID;
                    R_TEST_RECORD tr = new R_TEST_RECORD()
                    {
                        ID = tj.R_TEST_RECORD_ID,
                        MESSTATION = StationName,
                        DETAILTABLE = "R_TEST_JUNIPER",
                        R_SN_ID = SN.baseSN.ID,
                        SN = strSN,
                        EDIT_TIME = DateTime.Now,
                        STARTTIME = DateTime.Now,
                        ENDTIME = DateTime.Now,
                        TESTATION = TestStation,
                        STATE = "PASS",
                        EDIT_EMP = Station.LoginUser.EMP_NO,
                    };

                    SFCDB.ORM.Insertable(tj).ExecuteCommand();
                    SFCDB.ORM.Insertable(tr).ExecuteCommand();

                }


            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Station.SFCDB.ThrowSqlExeception = false;
            }


        }

        public static int IntervalSince1970()
        {
            return (int)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
        }

        public static void SN_LINK_TRSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession TRSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TRSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession TRSNQTYSession = null;
            try
            {
                TRSNQTYSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (TRSNQTYSession == null)
                {
                    TRSNQTYSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                    Station.StationSession.Add(TRSNQTYSession);
                }
            }
            catch
            {
                TRSNQTYSession = new MESStationSession();
            }

            MESStationSession LINKTRSNSession = null;
            try
            {
                LINKTRSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (LINKTRSNSession == null)
                {
                    LINKTRSNSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                    Station.StationSession.Add(LINKTRSNSession);
                }
            }
            catch
            {
                TRSNQTYSession = new MESStationSession();
            }
            //Tag 2021.12.11
            //MESStationSession TagSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            //if (TagSession == null)
            //{
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
            //    throw new MESReturnMessage(ErrMessage);
            //}

            //MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            //if (WoSession == null)
            //{
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY });
            //    throw new MESReturnMessage(ErrMessage);
            //}


            R_SN_KP TRSN = (R_SN_KP)TRSNSession.Value;
            var strSN = SNSession.Value.ToString();
            //var TagSN = TagSession.Value.ToString();
            //var WO = WoSession.Value.ToString();
            var db = Station.SFCDB;
            var linkCount = db.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == TRSN.SN && t.KP_NAME == "LINK_TR_SN" && t.SCANTYPE == "TR_SN" && t.VALID_FLAG == 1).Count();
            LINKTRSNSession.Value = linkCount.ToString();
            TRSNQTYSession.Value = TRSN.EXVALUE2;
            var qty = int.Parse(TRSN.EXVALUE2);

            if (qty <= linkCount)
            {
                throw new Exception($@"LinkCount({linkCount})>= TR_SN QTY({qty})");
            }

            //SN sn = (SN)SNSession.Value;
            //var WoInfo = db.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == sn.WorkorderNo).First();

            //if (linkCount >= WoInfo.WORKORDER_QTY)
            //{
            //    throw new Exception($@"LinkCount({linkCount})>= WO_QTY({WoInfo.WORKORDER_QTY})");
            //}
            var kpo = db.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strSN && t.KP_NAME == "LINK_TR_SN" && t.SCANTYPE == "TR_SN" && t.VALID_FLAG == 1).First();
            if (kpo != null)
            {
                kpo.VALID_FLAG = 0;
                kpo.EDIT_EMP = Station.LoginUser.EMP_NO;
                kpo.EDIT_TIME = DateTime.Now;
                db.ORM.Updateable(kpo).ExecuteCommand();
            }

            R_SN_KP kp = new R_SN_KP()
            {
                ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP"),
                ITEMSEQ = 1,
                SCANSEQ = 1,
                DETAILSEQ = 1,
                KP_NAME = "LINK_TR_SN",
                PARTNO = TRSN.PARTNO,
                MPN = TRSN.MPN,
                SN = strSN,
                VALUE = TRSN.SN,
                STATION = Station.StationName,
                SCANTYPE = "TR_SN",
                VALID_FLAG = 1,
                //EXKEY1 = "1",
                //EXVALUE1 = TagSN,
                //EXVALUE2 = WO,
                EDIT_EMP = Station.LoginUser.EMP_NO,
                EDIT_TIME = DateTime.Now
            };
            db.ORM.Insertable(kp).ExecuteCommand();

            linkCount = db.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == TRSN.SN && t.KP_NAME == "LINK_TR_SN" && t.SCANTYPE == "TR_SN" && t.VALID_FLAG == 1).Count();
            LINKTRSNSession.Value = linkCount.ToString();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddSNToSilverWIPAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var sfcdb = Station.SFCDB;
            //SN sn = (SN)SNSession.Value;
            string strSN = SNSession.Value.ToString();
            string strBU = Station.BU;
            string strPlant = "";

            if (string.IsNullOrEmpty(strSN))
            {
                throw new Exception($@"Serial Number Cannot be empty");
            }

            var strsku = SKUSession.Value.ToString();

            if (strsku == "750-049040")
            {
                throw new Exception($@"750-049040 CANNOT BE SILVERWIP. PLEASE CHECK WITH PM CRUZ GARCIA");
            }

            C_SKU sku = sfcdb.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == strsku).First();

            var SWConfig = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == strsku && t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "SilverWip").First();

            if (sku.SKU_TYPE.Contains("VIRTUAL"))
            {
                SNmaker.CkeckSNRule(strSN, sku.SN_RULE, sfcdb);
            }
            else
            {
                var snObj = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (snObj == null && Station.BU == "VNJUNIPER") //buy part
                {
                    var SuparMarket = sfcdb.ORM.Queryable<R_SUPERMARKET>().Where(t => t.R_SN_ID == strSN && t.STATUS.Trim() == "1").First();
                    if (SuparMarket != null)
                    {
                        throw new Exception($@"{strSN} is in SUPERMARKET ");
                    }
                }
                else //make part
                {
                    SN sn = new SN(strSN, sfcdb, DB_TYPE_ENUM.Oracle);

                    if (sn.baseSN != null)
                    {
                        if (sn.SkuNo == null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { strSN }));
                        }
                        if (sn.SkuNo != strsku)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000095", new string[] { strSN, strsku }));
                        }
                        if (sn.baseSN.COMPLETED_FLAG != "1")
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { strSN }));
                        }
                        if (sn.baseSN.REPAIR_FAILED_FLAG == "1")
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { strSN }));
                        }
                        if (sn.SkuNo != sku.SKUNO)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000095", new string[] { strSN, strsku }));
                        }
                        if (sn.baseSN.CURRENT_STATION == "MRB")
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { strSN }));
                        }
                        var SuparMarket = sfcdb.ORM.Queryable<R_SUPERMARKET>().Where(t => t.R_SN_ID == sn.baseSN.ID && t.STATUS.Trim() == "1").First();
                        if (SuparMarket != null)
                        {
                            throw new Exception($@"{strSN} is in SUPERMARKET ");
                        }
                    }
                }                
            }

            var count = sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SKUNO == strsku && t.STATE_FLAG == "1").Count();

            var maxCount = int.Parse(SWConfig.VALUE);
            if (count >= maxCount)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20211012153641", new string[] { sku.SKUNO, count.ToString(), maxCount.ToString() }));
            }

            var wipobj = sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SN == strSN).First();
            if (wipobj != null)
            {
                if (wipobj.STATE_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211105151002", new string[] { strSN }));
                }
                if (wipobj.STATE_FLAG == "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211124144715", new string[] { strSN }));
                }
            }

            /*---------------------------------------SilverWip Supermarket Section, to activate just uncomment---------------------------------------------  */

            if (strBU == "FJZ") //DELETE THIS "IF" WHEN FVN STARTS TO SCAN IN SUPERMARKET
            {

                var objSM = sfcdb.ORM.Queryable<R_SUPERMARKET, R_SN>((sm, sn) =>
                         new object[]
                         {
                                SqlSugar.JoinType.Inner, sm.R_SN_ID == sn.ID
                         })
                        .Where((sm, sn) => sn.SN == strSN)
                        .OrderBy(sm => sm.IN_TIME, SqlSugar.OrderByType.Desc)
                        .First();
                if (objSM == null)
                {
                    throw new Exception($@"{strSN} needs to go first to Supermarket and then to Silverwip");
                }

                if (objSM.STATUS == "1")
                {
                    throw new Exception($@"{strSN} is still in supermarket. Please ask to transfer to Silverwip");
                }
            }
               /*---------------------------------------SilverWip Supermarket Section, to activate just uncomment---------------------------------------------  */

            if (strBU == "VNJUNIPER")
            {
                //SAP Movement  ps:FVN do sap movement in silver wip check in/out , not in supermarket
                string movID = DoSAP311Movement(strSN, strsku, "VUEA", "B73F", "B23N", 1, strBU, Station.LoginUser.EMP_NO, sfcdb);//FVN PLant/fromStorage/toStorage supply by PE吳忠義
            }

            wipobj = new R_JUNIPER_SILVER_WIP()
            {
                ID = MesDbBase.GetNewID(sfcdb.ORM, Station.BU, "R_JUNIPER_SILVER_WIP"),
                START_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM),
                STATE_FLAG = "1",
                IN_WIP_USER = Station.LoginUser.EMP_NO,
                EDIT_EMP = Station.LoginUser.EMP_NO,
                EDIT_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM),
                SKUNO = strsku,
                SN = strSN,
                TEST_HOURS = 0,
            };

            var RET = sfcdb.ORM.Insertable<R_JUNIPER_SILVER_WIP>(wipobj).ExecuteCommand();

        }

        /// <summary>
        /// Check SKU Have silverwip config
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// 
        public static string DoSAP311Movement(string strSN, string strSKU, string plant, string fromLocation, string toLocation, int qty, string BU, string empNO, OleExec sfcdb)
        {
            string rfcDocument, movID;
            try
            {
                var date = MesDbBase.GetOraDbTime(sfcdb.ORM);
                string gtID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_STOCK_GT");
                var postDate = InterfacePublicValues.GetPostDate(sfcdb);
                DateTime pDate = Convert.ToDateTime(postDate);

                //if (postDate 

                ZCMM_NSBG_0051 rfc = new ZCMM_NSBG_0051(BU);//change to BU
                rfc.SetValue(plant, strSKU, fromLocation, toLocation, qty.ToString(), "", pDate, "311");
                rfc.CallRFC();

                string rfcResult = rfc.GetValue("O_FLAG");
                string rfcMessage = rfc.GetValue("O_MESSAGE");
                rfcDocument = rfc.GetValue("O_MBLNR");

                sfcdb.BeginTrain();

                #region Old Tables
                //var stockGT = new R_STOCK_GT()
                //{
                //    ID = gtID,
                //    SKUNO = strSKU,
                //    TOTAL_QTY = qty,
                //    FROM_STORAGE = fromLocation,
                //    TO_STORAGE = toLocation,
                //    CONFIRMED_FLAG = (rfcResult == "1") ? "0" : "1",
                //    SAP_FLAG = rfcResult,
                //    SAP_MESSAGE = (rfcResult == "1") ? rfcMessage : "SUCCESS",
                //    EDIT_EMP = empNO,
                //    EDIT_TIME = date,
                //    SAP_STATION_CODE = rfcDocument,
                //};

                //var stock = new R_STOCK()
                //{
                //    ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_STOCK"),
                //    SN = strSN,
                //    SKUNO = strSKU,
                //    FROM_STORAGE = fromLocation,
                //    TO_STORAGE = toLocation,
                //    CONFIRMED_FLAG = (rfcResult == "1") ? "0" : "1",
                //    SAP_FLAG = rfcResult,
                //    EDIT_EMP = empNO,
                //    EDIT_TIME = date,
                //    GT_ID = gtID,
                //};

                //var retGT = sfcdb.ORM.Insertable(stockGT).ExecuteCommand();
                //var retStock = sfcdb.ORM.Insertable(stock).ExecuteCommand();
                #endregion

                movID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_SAP_MOVEMENTS");

                var r_sap_m = new R_SAP_MOVEMENTS()
                {
                    ID = movID,
                    SKUNO = strSKU,
                    SN = strSN,
                    TOTAL_QTY = qty,
                    FROM_STORAGE = fromLocation,
                    TO_STORAGE = toLocation,
                    CONFIRMED_FLAG = (rfcResult == "1") ? "0" : "1",
                    SAP_FLAG = rfcResult,
                    SAP_MESSAGE = (rfcResult == "1") ? rfcMessage : "SUCCESS",
                    EDIT_EMP = empNO,
                    EDIT_TIME = date,
                    DOCUMENT_ID = rfcDocument,
                    MOVEMENT_TYPE = "311"
                };

                var retr_sap_m = sfcdb.ORM.Insertable(r_sap_m).ExecuteCommand();

                sfcdb.CommitTrain();

                if (rfcResult == "1")
                {
                    throw new Exception(rfcMessage);
                }
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Goods Transfer 311 fail!"))
                {
                    throw new MESReturnMessage(ex.Message + " Please verify existing inventory for SKU " + strSKU + " in " + fromLocation + ".");
                }
                else
                {
                    throw new MESReturnMessage(ex.Message);
                }
                
            }

            return movID;
        }

        public static void CheckSkuHaveSilverWipConfig(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var sfcdb = Station.SFCDB;
            var strsku = SKUSession.Value.ToString();
            C_SKU sku = sfcdb.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == strsku).First();

            var SWConfig = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == strsku && t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "SilverWip").First();

            if (SWConfig == null)
            {
                //The SKU: {0} is not configured for SilverWIP, please contact the Process Engineer!
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211004123053", new string[] { strsku });
                throw new MESReturnMessage(ErrMessage);
                //throw new Exception($@" SKU: {strsku} no config for SilverWIP in SKU Setting, favor de contactar al Ingeniero de Procesos! C_SKU_DETAIL ");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RemoveSNToSilverWIPAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var sfcdb = Station.SFCDB;
            var strSN = SNSession.Value.ToString();
            string strBU = Station.BU;

            //SN sn = new SN(strSN, sfcdb, DB_TYPE_ENUM.Oracle);
            var strsku = SKUSession.Value.ToString();

            var wipobj = sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SN == strSN).OrderBy(t => t.START_TIME, SqlSugar.OrderByType.Desc).First();
            if (wipobj != null)
            {
                if (wipobj.STATE_FLAG == "0")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124144715", new string[] { strSN });
                    throw new Exception(ErrMessage);
                }
            }
            if (wipobj == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210803125239", new string[] { strSN });
                throw new Exception(ErrMessage);
            }

            if(wipobj.SKUNO != strsku)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124144628", new string[] { strSN, strsku });
                throw new Exception(ErrMessage);
            }

            var SWConfig = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == strsku && t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "SilverWip").First();

            if (SWConfig == null && wipobj.IN_WIP_USER != "PI_NOT_IN_SM")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211004123053", new string[] { strsku });
                throw new Exception(ErrMessage);
            }

            //检查是否陪测中
            var kps = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strSN && t.VALID_FLAG == 1 && t.SCANTYPE == "SILVER_SN").First();
            if (kps != null)
            {
                throw new Exception($@"{strSN} is a kp with {kps.SN}");
            }

            if (wipobj.IN_WIP_USER == "PI_NOT_IN_SM" && wipobj.TEST_HOURS == 0)
            {
                wipobj.STATE_FLAG = "0";
                wipobj.OUT_WIP_USER = Station.LoginUser.EMP_NO;
                wipobj.END_TIME = DateTime.Now;
                wipobj.EDIT_EMP = Station.LoginUser.EMP_NO;
                wipobj.EDIT_TIME = DateTime.Now;
                sfcdb.ORM.Updateable(wipobj).Where(t => t.ID == wipobj.ID).ExecuteCommand();
            }
            else
            {
                R_TEST_RECORD testrecObj = sfcdb.ORM.Queryable<R_TEST_RECORD>()
                        .Where(tr => tr.SN == wipobj.SN && tr.STARTTIME >= wipobj.START_TIME)
                        .OrderBy(tr => tr.STARTTIME, SqlSugar.OrderByType.Desc).First();
                if (testrecObj == null && wipobj.TEST_HOURS > 0)
                {
                    throw new Exception($@"The Unit SN: {strSN} not have Test Record but have Test Hours, please check");
                }

                string virtualWO = null, virtualPath = null, routeID = null, empNO = null;
                string messageAlert = null;
                DateTime dateTime = sfcdb.ORM.GetDate();
                empNO = Station.LoginUser.EMP_NO;
                string currentNode = null;
                string nextNode = null;
                string lastNode = null;

                /* UNCOMMENT WITH NEW LOGIC FOR FJZ
              Config.Route routeVirtual = new Config.Route();
              Config.Route routeVirtualRes = new Config.Route();

              if (strBU == "FJZ") //DELETE THIS "IF" WHEN FVN STARTS TO SCAN IN SUPERMARKET
              {


                  if ((testrecObj == null || sfcdb.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.TEGROUP == "FUNCTIONAL" && t.TE_STATION == testrecObj.TESTATION).Any()
                      && testrecObj.STATE == "PASS" || testrecObj.TESTATION == "DBG"))
                  {
                      #region Create a Virtual Path: OUT_SILVER_WIP --> VI --> FCT --> FQA_OUT in MES DB                

                      C_ROUTE mainMessage = new C_ROUTE()
                      {
                          ID = "",
                          ROUTE_NAME = "SW_VIRTUAL_PASS",
                          ROUTE_TYPE = "REGULAR",
                          EDIT_TIME = null,
                          EDIT_EMP = "",
                      };

                      routeVirtual.MainMessage = mainMessage;

                      List<RouteDetailItem> detailList = new List<RouteDetailItem>();
                      currentNode = "node" + Guid.NewGuid().ToString();
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(
                        new RouteDetailItem
                        {
                            ID = currentNode,
                            SEQ_NO = 10,
                            ROUTE_ID = "",
                            STATION_NAME = "OUT_SILVER_WIP",
                            STATION_TYPE = "JOBSTART",
                            RETURN_FLAG = "N",
                            NextStation = new C_ROUTE_DETAIL()
                            {
                                ID = nextNode,
                                SEQ_NO = 20,
                                ROUTE_ID = "",
                                STATION_NAME = "VI",
                                STATION_TYPE = "NORMAL",
                                RETURN_FLAG = ""
                            }

                        });

                      lastNode = nextNode;
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(new RouteDetailItem
                      {
                          ID = lastNode,
                          SEQ_NO = 20,
                          ROUTE_ID = "",
                          STATION_NAME = "VI",
                          STATION_TYPE = "NORMAL",
                          RETURN_FLAG = "N",
                          NextStation = new C_ROUTE_DETAIL()
                          {
                              ID = nextNode,
                              SEQ_NO = 30,
                              ROUTE_ID = "",
                              STATION_NAME = "FCT",
                              STATION_TYPE = "NORMAL",
                              RETURN_FLAG = ""
                          }
                      });

                      lastNode = nextNode;
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(new RouteDetailItem
                      {
                          ID = lastNode,
                          SEQ_NO = 30,
                          ROUTE_ID = "",
                          STATION_NAME = "FCT",
                          STATION_TYPE = "NORMAL",
                          RETURN_FLAG = "N",
                          NextStation = new C_ROUTE_DETAIL()
                          {
                              ID = nextNode,
                              SEQ_NO = 40,
                              ROUTE_ID = "",
                              STATION_NAME = "FQA",
                              STATION_TYPE = "JOBFINISH",
                              RETURN_FLAG = ""
                          }
                      });

                      lastNode = nextNode;
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(
                          new RouteDetailItem
                          {
                              ID = lastNode,
                              SEQ_NO = 40,
                              ROUTE_ID = "",
                              STATION_NAME = "FQA",
                              STATION_TYPE = "JOBFINISH",
                              RETURN_FLAG = "N",
                              NextStation = null
                          }

                      );

                      routeVirtual.Detail = detailList;

                      virtualPath = "OUT_SILVER_WIP --> VI --> FCT --> FQA";

                      #endregion
                  }
                  else
                  {
                      #region Create a Virtual Path: VI --> FCT --> FQA_OUT in MES DB

                      C_ROUTE mainMessage = new C_ROUTE()
                      {
                          ID = "",
                          ROUTE_NAME = "SW_VIRTUAL_FAIL_" + testrecObj.TESTATION,
                          ROUTE_TYPE = "REGULAR",
                          EDIT_TIME = null,
                          EDIT_EMP = "",
                      };

                      routeVirtual.MainMessage = mainMessage;

                      List<RouteDetailItem> detailList = new List<RouteDetailItem>();
                      currentNode = "node" + Guid.NewGuid().ToString();
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(
                        new RouteDetailItem
                        {
                            ID = currentNode,
                            SEQ_NO = 10,
                            ROUTE_ID = "",
                            STATION_NAME = "OUT_SILVER_WIP",
                            STATION_TYPE = "JOBSTART",
                            RETURN_FLAG = "N",
                            NextStation = new C_ROUTE_DETAIL()
                            {
                                ID = nextNode,
                                SEQ_NO = 20,
                                ROUTE_ID = "",
                                STATION_NAME = "VI",
                                STATION_TYPE = "NORMAL",
                                RETURN_FLAG = ""
                            }

                        });

                      lastNode = nextNode;
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(new RouteDetailItem
                      {
                          ID = lastNode,
                          SEQ_NO = 20,
                          ROUTE_ID = "",
                          STATION_NAME = "VI",
                          STATION_TYPE = "NORMAL",
                          RETURN_FLAG = "N",
                          NextStation = new C_ROUTE_DETAIL()
                          {
                              ID = nextNode,
                              SEQ_NO = 30,
                              ROUTE_ID = "",
                              STATION_NAME = "FCT",
                              STATION_TYPE = "NORMAL",
                              RETURN_FLAG = ""
                          }
                      });

                      lastNode = nextNode;
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(new RouteDetailItem
                      {
                          ID = lastNode,
                          SEQ_NO = 30,
                          ROUTE_ID = "",
                          STATION_NAME = "FCT",
                          STATION_TYPE = "NORMAL",
                          RETURN_FLAG = "N",
                          NextStation = new C_ROUTE_DETAIL()
                          {
                              ID = nextNode,
                              SEQ_NO = 40,
                              ROUTE_ID = "",
                              STATION_NAME = testrecObj.TESTATION,
                              STATION_TYPE = "NORMAL",
                              RETURN_FLAG = ""
                          }
                      });

                      lastNode = nextNode;
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(new RouteDetailItem
                      {
                          ID = lastNode,
                          SEQ_NO = 40,
                          ROUTE_ID = "",
                          STATION_NAME = testrecObj.TESTATION,
                          STATION_TYPE = "NORMAL",
                          RETURN_FLAG = "N",
                          NextStation = new C_ROUTE_DETAIL()
                          {
                              ID = nextNode,
                              SEQ_NO = 50,
                              ROUTE_ID = "",
                              STATION_NAME = "FQA",
                              STATION_TYPE = "NORMAL",
                              RETURN_FLAG = ""
                          }
                      });

                      lastNode = nextNode;
                      nextNode = "node" + Guid.NewGuid().ToString();

                      detailList.Add(
                          new RouteDetailItem
                          {
                              ID = lastNode,
                              SEQ_NO = 50,
                              ROUTE_ID = "",
                              STATION_NAME = "FQA",
                              STATION_TYPE = "JOBFINISH",
                              RETURN_FLAG = "N",
                              NextStation = null
                          }
                      );

                      routeVirtual.Detail = detailList;
                      virtualPath = "VI --> FCT --> " + testrecObj.TESTATION + " --> FQA";

                      #endregion
                  }

                  if (routeVirtual.MainMessage != null)
                  {
                      MESAPIData mesdata = new MESAPIData();
                      mesdata.Class = "MESStation.Config.RouteConfig";
                      mesdata.Function = "AddRoute";
                      mesdata.Data = new { RouteJsonString = routeVirtual };

                      MESAPIClient MESAPI = new MESAPIClient("ws://localhost:2130/ReportService", "WEBAPI", "FOXCONN168!!");
                      int TimeOut = 50000;

                      Newtonsoft.Json.Linq.JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);

                      if (JO["Status"].ToString().ToUpper() == "PASS")
                      {

                          string ResData = JO["Data"].ToString();
                          routeVirtualRes = Newtonsoft.Json.JsonConvert.DeserializeObject<Config.Route>(ResData);
                          routeID = routeVirtualRes.MainMessage.ID;

                          messageAlert = "Success Created Virtual WO: " + virtualWO + " RouteName: " + routeVirtualRes.MainMessage.ROUTE_NAME + " Path: " + virtualPath + "!";
                          Station.AddMessage("MSGCODE20211126112355", new string[] { messageAlert }, StationMessageState.Message, StationMessageDisplayType.Swal);
                          Station.AddMessage("MSGCODE20211126112355", new string[] { messageAlert }, StationMessageState.CMCMessage);
                      }
                      else
                      {
                          if (!JO["Message"].ToString().Contains("already existed"))
                          {
                              messageAlert = "Virtual WO: " + virtualWO + "  Message: " + JO["Message"].ToString() +
                              "! Virtual Path : " + virtualPath + " could not be created! ";
                              Station.AddMessage("MSGCODE20211126112355", new string[] { messageAlert }, StationMessageState.Message, StationMessageDisplayType.Swal);
                              Station.AddMessage("MSGCODE20211126112355", new string[] { messageAlert }, StationMessageState.CMCMessage);
                          }

                          C_ROUTE routeExist = sfcdb.ORM.Queryable<C_ROUTE>().Where(a => a.ROUTE_NAME == routeVirtual.MainMessage.ROUTE_NAME).First();

                          if (routeExist == null)
                          {
                              throw new Exception($@" RouteName: {routeVirtual.MainMessage.ROUTE_NAME} not exist in MES!");
                          }

                          routeID = routeExist.ID;
                      }
                  }
              }
              */

                try
                {
                    //SW Updateable: R_JUNIPER_SILVER_WIP & R_SN_LOCK
                    sfcdb.BeginTrain();

                    if (strBU == "FJZ")
                    {
                        /* ---------------------------------------SilverWip Supermarket Section, to activate just uncomment--------------------------------------------- */

                        #region Call RFC
                        string strPlant;
                        //mov from SWJN to JWIP --> en SM Checkin from JWIP to JNSM
                        strPlant = "MBGA";
                        string res = DoSAP311Movement(strSN, strsku, strPlant, "JNSW", "JWIP", 1, strBU, empNO, sfcdb);
                        #endregion

                        #region Born New SN
                        /*
                        string r_snID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_SN");
                        string detail_snID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_SN_STATION_DETAIL");
                        string woID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_WO_BASE");
                        virtualWO = MesDbBase.GetNewWorkorder(sfcdb.ORM, "JNPSWVWO");
                        var objSN = sfcdb.ORM.Queryable<R_SN>().Where(rsn => rsn.SN == strSN && rsn.VALID_FLAG == "1").First();
                        //var objLastWO = sfcdb.ORM.Queryable<R_WO_BASE>(wo => wo.WORKORDERNO == )

                        if (objSN != null)
                        {
                            objSN.VALID_FLAG = "2";
                            sfcdb.ORM.Updateable(objSN).ExecuteCommand();
                        }

                        R_WO_BASE RWB = new R_WO_BASE()
                        {
                            ID = woID,
                            WORKORDERNO = virtualWO,
                            PLANT = "FJZ",
                            RELEASE_DATE = dateTime,
                            DOWNLOAD_DATE = dateTime,
                            PRODUCTION_TYPE = "MODEL",
                            WO_TYPE = "VIRTUAL",
                            SKUNO = strsku,
                            ROUTE_ID = routeID,
                            START_STATION = "OUT_SILVER_WIP",
                            CLOSED_FLAG = "0",
                            WORKORDER_QTY = 1,
                            INPUT_QTY = 0,
                            FINISHED_QTY = 0,
                            SCRAPED_QTY = 0,
                            EDIT_EMP = empNO,
                            EDIT_TIME = dateTime
                        };

                        R_SN RSN = new R_SN()
                        {
                            ID = r_snID,
                            SN = strSN,
                            SKUNO = strsku,
                            WORKORDERNO = virtualWO,
                            PLANT = "MBGA",
                            ROUTE_ID = routeID,
                            STARTED_FLAG = "1",
                            START_TIME = dateTime,
                            PACKED_FLAG = "0",
                            COMPLETED_FLAG = "0",
                            SHIPPED_FLAG = "0",
                            REPAIR_FAILED_FLAG = "0",
                            CURRENT_STATION = "OUT_SILVER_WIP",
                            NEXT_STATION = "VI",
                            CUST_PN = strsku,
                            SCRAPED_FLAG = "0",
                            PRODUCT_STATUS = "FRESH",
                            REWORK_COUNT = 0,
                            VALID_FLAG = "1",
                            EDIT_EMP = empNO,
                            EDIT_TIME = dateTime
                        };

                        R_SN_STATION_DETAIL RSSD = new R_SN_STATION_DETAIL()
                        {
                            ID = detail_snID,
                            R_SN_ID = r_snID,
                            SN = strSN,
                            SKUNO = strsku,
                            WORKORDERNO = virtualWO,
                            PLANT = "FJZ",
                            ROUTE_ID = routeID,
                            LINE = "Line1",
                            STARTED_FLAG = "1",
                            START_TIME = dateTime,
                            PACKED_FLAG = "0",
                            COMPLETED_FLAG = "0",
                            SHIPPED_FLAG = "0",
                            REPAIR_FAILED_FLAG = "0",
                            CURRENT_STATION = "OUT_SILVER_WIP",
                            NEXT_STATION = "VI",
                            CUST_PN = strsku,
                            DEVICE_NAME = "VI",
                            STATION_NAME = "VI",
                            PRODUCT_STATUS = "FRESH",
                            REWORK_COUNT = 0,
                            VALID_FLAG = "1",
                            EDIT_EMP = empNO,
                            EDIT_TIME = dateTime
                        };

                        sfcdb.ORM.Insertable(RWB).ExecuteCommand();
                        sfcdb.ORM.Insertable(RSN).ExecuteCommand();
                        sfcdb.ORM.Insertable(RSSD).ExecuteCommand();

                        messageAlert = "New Virtual WO: " + virtualWO;
                        */
                        #endregion


                    }

                    if (Station.BU == "VNJUNIPER")
                    {
                        //SAP Movement  ps:FVN do sap movement in silver wip check in/out , not in supermarket
                        string movID = DoSAP311Movement(strSN, strsku, "VUEA", "B23N", "B73F", 1, Station.BU, Station.LoginUser.EMP_NO, sfcdb);//FVN PLant/fromStorage/toStorage supply by PE吳忠義
                    }

                    wipobj.STATE_FLAG = "0";
                    wipobj.OUT_WIP_USER = Station.LoginUser.EMP_NO;
                    wipobj.END_TIME = DateTime.Now;
                    wipobj.EDIT_EMP = Station.LoginUser.EMP_NO;
                    wipobj.EDIT_TIME = DateTime.Now;
                    sfcdb.ORM.Updateable(wipobj).Where(t => t.ID == wipobj.ID).ExecuteCommand();

                    List<R_SN_LOCK> locks = sfcdb.ORM.Queryable<R_SN_LOCK>()
                        .WhereIF(!String.IsNullOrEmpty(strSN), lk => lk.SN == strSN && lk.LOCK_STATUS == "1"
                                    && lk.LOCK_STATION == "ALL"
                                    && (lk.LOCK_REASON.Contains("TEST_HOURS")
                                    || lk.LOCK_REASON.Contains("WIP_DAYS")
                                    || lk.LOCK_REASON.Contains("UNIT_QTY"))
                        ).ToList();

                    var unlock_ret = sfcdb.ORM.Updateable<R_SN_LOCK>()
                        .SetColumns(lk => new R_SN_LOCK()
                        {
                            LOCK_STATUS = "0",
                            UNLOCK_REASON = "Automatic Unlock SW Unit in CheckOut Station",
                            UNLOCK_EMP = "MESSYSTEM",
                            UNLOCK_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM)
                        })
                        .Where(lk => lk.SN == strSN & lk.LOCK_STATUS == "1"
                                & (lk.LOCK_REASON.Contains("SilverWIP Control: TEST_HOURS TimeOut")
                                    || lk.LOCK_REASON.Contains("SilverWIP Control: WIP_DAYS TimeOut")
                                    || lk.LOCK_REASON.Contains("SilverWIP MAX_TEST_HOURS TimeOut")
                                    || lk.LOCK_REASON.Contains("SilverWIP MAX_WIP_DAYS TimeOut")
                        )).ExecuteCommand();

                    sfcdb.CommitTrain();

                }
                catch (Exception ex)
                {
                    sfcdb.RollbackTrain();
                    throw new Exception(ex.Message);
                }                
            }
        }


        public static void RemoveSilverWipKPAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession KPSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var sfcdb = Station.SFCDB;
            var strSN = SNSession.Value.ToString();
            var strKP = KPSNSession.Value.ToString();
            var kp = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strSN && t.VALUE == strKP && t.SCANTYPE == "SILVER_SN").First();

            if (kp == null)
            {
                throw new Exception($@"Can't find kp data");
            }

            if (kp.VALID_FLAG != 1)
            {
                throw new Exception($@"KP  already be delink");
            }

            kp.VALID_FLAG = 0;
            kp.EDIT_EMP = Station.LoginUser.EMP_NO;
            kp.EDIT_TIME = DateTime.Now;

            sfcdb.ORM.Updateable(kp).ExecuteCommand();

        }

        public static void AddTRSN_WHSQty_CheckoutAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession TRSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + ":" + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession TRSN_OrigQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TRSN_OrigQtySession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + ":" + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }


            MESStationSession QtySession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (QtySession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + ":" + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            string TRSN = TRSNSession.Value.ToString().Trim();

            try
            {

                string Original_Qty = TRSN_OrigQtySession.Value.ToString().Trim();
                string Input_qty = QtySession.Value.ToString().Trim();

                int _original_qty = 0;
                int _input_qty = 0;

                if (!Input_qty.All(char.IsDigit))
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210815110654", new string[] { Input_qty });
                    throw new MESReturnMessage(ErrMessage);
                }

                _original_qty = int.Parse(Original_Qty);
                _input_qty = int.Parse(Input_qty);

                if (_input_qty > _original_qty)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210815104739", new string[] { Input_qty, Original_Qty });
                    throw new MESReturnMessage(ErrMessage);
                }

                DataTable checkDt = null;

                string sql = string.Format(@"
                        SELECT a.ID, a.TR_SN, a.ORIGINAL_QTY, a.CHECKOUT_QTY, a.CHECKOUT_BY, a.CHECKOUT_TIME, a.RETURN_QTY, a.RETURN_BY, a.RETURN_TIME
                        FROM MES4.R_WHS_MATL_WIP_CTRL a where a.TR_SN = '{0}'", TRSN);

                string strApconn = "User Id = FUSER; Password = Fuser#01; Data Source = (DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.14.253.219)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=jalp)))";

                using (OracleConnection conn = new OracleConnection(strApconn))
                {
                    conn.Open();
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.AddRange(new OracleParameter[] { });
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        checkDt = new DataTable();
                        adapter.Fill(checkDt);
                    }
                    conn.Close();
                }

                //DataTable dt = Station.APDB.ExecuteDataTable(sql, CommandType.Text, null); //No work

                if (checkDt != null)
                {
                    if (checkDt.Rows.Count > 0)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210815130448");
                        throw new MESReturnMessage(ErrMessage);
                    }
                }

                string insertsql = "insert into MES4.R_WHS_MATL_WIP_CTRL  " +
                            "(ID, TR_SN, ORIGINAL_QTY, CHECKOUT_QTY, CHECKOUT_BY, CHECKOUT_TIME) " +
                            "values ('FJZ" + DateTime.Now.ToString("yyyyMMddhhmmssff") + "', " +
                                "'" + TRSN + "','" + Original_Qty + "','" + Input_qty + "','" + Station.LoginUser.EMP_NO + "',sysdate )";

                int rowsInserted = 0;

                using (OracleConnection conn = new OracleConnection(strApconn))
                {
                    conn.Open();
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = insertsql;
                        cmd.Parameters.AddRange(new OracleParameter[] { });
                        rowsInserted = cmd.ExecuteNonQuery();

                    }
                    conn.Close();
                }

                if (rowsInserted == 0)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210815160720");
                    throw new MESReturnMessage(ErrMessage);
                }

            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(" TR_SN: " + TRSN + " " + ex.Message);
            }

        }



        public static void AddSNToSupermarket(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession LocationFromSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (LocationFromSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            try
            {
                string strSKU = SKUSession.Value.ToString();
                string LocationFrom = LocationFromSession.Value.ToString();

                if (LocationFrom == "SELECT LOCATION")
                {
                    throw new Exception($@"Please choose correct location");
                }

                string strLocationFrom = LocationFrom.Substring(LocationFrom.IndexOf('~') + 2);
                string strSN = SNSession.Value.ToString();
                string strBU = Station.BU;
                var sfcdb = Station.SFCDB;
                string strPlant = "", movID = "";
                string empNO = Station.LoginUser.EMP_NO;
                string movType = null;
                DateTime dateTime = sfcdb.ORM.GetDate();

                if (String.IsNullOrEmpty(strSN))
                {
                    throw new Exception($@"Please Input a valid SN");
                }

                if (strBU == "FJZ")
                {
                    strPlant = "MBGA";

                    string cValue = sfcdb.ORM.Queryable<C_CONTROL>().Where(c => c.CONTROL_NAME == "BACKFLUSH").Select(c => c.CONTROL_VALUE).First().ToString();

                    string strdtTO = cValue.Substring(cValue.IndexOf('~') + 1);
                    DateTime dtTO = Convert.ToDateTime(strdtTO);
                    string strdtFrom = cValue.Substring(0, 19);
                    DateTime dtFrom = Convert.ToDateTime(strdtFrom);

                    if (dateTime > dtFrom && dateTime < dtTO)
                    {
                        movID = "END OF MONTH EXCEPTION";
                    }

                    else
                    {
                        #region SAP Movement  ps:FVN do sap movement in silver wip check in/out , not in supermarket
                        movID = DoSAP311Movement(strSN, strSKU, strPlant, strLocationFrom, "SMJN", 1, strBU, Station.LoginUser.EMP_NO, sfcdb);
                        #endregion
                    }

                }
                else
                {
                    strPlant = "TBD"; //FVN PLant name in SAP
                }

                #region Insert R_SUPERMARKET
                string smID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_SUPERMARKET");

                var objSN = sfcdb.ORM.Queryable<R_SN>().Where(sn => sn.SN == strSN && sn.SKUNO == strSKU && sn.VALID_FLAG == "1").First();
                if (objSN != null)
                {
                    if(strBU == "FJZ")
                    {
                        if(movID != "END OF MONTH EXCEPTION")
                        {
                            movType = "311";
                        }
                        else
                        {
                            movType = strLocationFrom;
                        }
                    }
                    else
                    {
                        movType = "";
                    }
                    var newSM = new R_SUPERMARKET()
                    {
                        ID = smID,
                        R_SN_ID = objSN.ID,
                        IN_TIME = dateTime,
                        IN_BY = empNO,
                        IN_MOV_TYPE = movType,
                        IN_R_SAP_MOV_ID = movID,
                        STATUS = "1",
                    };
                    var resSM = sfcdb.ORM.Insertable(newSM).ExecuteCommand();
                }
                else if (strBU == "VNJUNIPER")
                {
                    //FVN BuyPart SM CheckIn Logic By PE 譚義康 2022-02-11
                    var newSM = new R_SUPERMARKET()
                    {
                        ID = smID,
                        R_SN_ID = strSN,
                        IN_TIME = dateTime,
                        IN_BY = empNO,
                        IN_MOV_TYPE = "BuyPart",
                        IN_R_SAP_MOV_ID = strSKU,
                        STATUS = "1",
                    };
                    var resSM = sfcdb.ORM.Insertable(newSM).ExecuteCommand();
                }
                else
                {
                    throw new Exception($@"Serial Number not valid");
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw new MESReturnMessage("AddSNToSupermarket: " + ex.Message);
                //sfcdb.RollbackTrain();
            }
        }
        /// <summary>
        /// MATL_LINK 使用，將循環標籤與工單綁定。
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LinkTagToWo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var sfcdb = Station.SFCDB;
            var strWo = WOSession.Value.ToString();

            UIInputData O = new UIInputData()
            { Timeout = 50000, IconType = IconType.None, Message = "TempTag", Tittle = "Link TempTag with Wo", Type = UIInputType.Password, Name = "TempTag", ErrMessage = "No Input" };

            var Tag = O.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper();
            if (Tag.Length == 10)
            {
                if (!Tag.StartsWith("WO TAG #") || !Regex.IsMatch(Tag.Substring(8, 2), @"^[0-9]{2}$"))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("Tag Regex Not match"));
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("Tag length Not match"));
            }

            var log = sfcdb.ORM.Queryable<R_MES_LOG>().Where(t => t.PROGRAM_NAME == "MesStation" && t.FUNCTION_NAME == "WoTag" && t.DATA1 == Tag &&t.DATA3=="1").First();
            if (log != null)
            {
                if (log.DATA2 != strWo)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814196325", new string[] { log.DATA2, log.DATA1 }));
                }
                //log.DATA2 = Tag;
                //sfcdb.ORM.Updateable(log).Where(t => t.ID == log.ID).ExecuteCommand();
            }
            else
            {
                log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID(sfcdb.ORM, Station.BU, "R_MES_LOG"),
                    PROGRAM_NAME = "MesStation",
                    FUNCTION_NAME = "WoTag",
                    DATA1 = Tag,
                    DATA2 = strWo,
                    DATA3 = "1"
                };
                sfcdb.ORM.Insertable(log).ExecuteCommand();
            }
        }

        /// <summary>
        /// MATL_LINK 使用，Inputqty=woqty will closed TAG。
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ClesedTagToWo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession WOQTYSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession INputQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var sfcdb = Station.SFCDB;
            var strWo = WOSession.Value.ToString();

            var log = sfcdb.ORM.Queryable<R_MES_LOG>().Where(t => t.PROGRAM_NAME == "MesStation" && t.FUNCTION_NAME == "WoTag" && t.DATA2 == strWo && t.DATA3 == "1").ToList();
            if (WOQTYSession.Value.ToString() == INputQtySession.Value.ToString())
            {
                if (log.Count > 0)
                {
                    foreach (var wo in log)
                    {
                        Station.SFCDB.ORM.Updateable<R_MES_LOG>().SetColumns(r => new R_MES_LOG
                        {
                            DATA3 = "0"
                        }).Where(r => r.ID == wo.ID && r.DATA3 == "1").ExecuteCommand();
                    }
                }
            }
        }


        /// <summary>
        /// 從臨時標籤讀取工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadWoFromLinkTag(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            
            
            var sfcdb = Station.SFCDB;

            var Tag = Input.Value.ToString().Trim().ToUpper();

            var log = sfcdb.ORM.Queryable<R_MES_LOG>().Where(t => t.PROGRAM_NAME == "MesStation" && t.FUNCTION_NAME == "WoTag" && t.DATA1 == Tag && t.DATA3 == "1").First();
            if (log != null )
            {
                Input.Value = log.DATA2;
            }
        }


        [Obsolete]
        public static void SNOutSupermarket(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession LocationToSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LocationToSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            try
            {
                string strSKU = SKUSession.Value.ToString();
                string strLocation = LocationToSession.Value.ToString();
                string strLocationTo = strLocation.Substring(strLocation.IndexOf('~') + 2);
                string strSN = SNSession.Value.ToString();
                string strBU = Station.BU;
                var sfcdb = Station.SFCDB;
                string strPlant = "", movID = "", smID = "", movType = "311";
                string empNO = Station.LoginUser.EMP_NO;
                DateTime dateTime = sfcdb.ORM.GetDate();

                if (strLocation == "SELECT LOCATION")
                {
                    throw new Exception($@"Please choose correct location");
                }
                if (String.IsNullOrEmpty(strSN))
                {
                    throw new Exception($@"Please Input a valid SN");
                }

                if (strBU == "FJZ")
                {
                    strPlant = "MBGA";
                    

                    var objSN = sfcdb.ORM.Queryable<R_SUPERMARKET, R_SN>((sm, sn) =>
                     new object[]
                     {
                        SqlSugar.JoinType.Inner, sm.R_SN_ID == sn.ID
                     })
                    .Where((sm, sn) => sn.SN == strSN && sn.VALID_FLAG == "1" && sm.STATUS == "1")
                    .Select((sm, sn) => new { sm.ID, sn.SN, sn.SKUNO }).First();

                    if (objSN == null)
                    {
                        throw new Exception($@"Serial Number is not in Supermarket.");
                    }
                    if (objSN.SKUNO != strSKU)
                    {
                        throw new Exception($@"SKU " + strSKU + " does not match with SKU for SN " + strSN);
                    }

                    string cValue = sfcdb.ORM.Queryable<C_CONTROL>().Where(c => c.CONTROL_NAME == "BACKFLUSH").Select(c => c.CONTROL_VALUE).First().ToString();

                    string strdtTO = cValue.Substring(cValue.IndexOf('~') + 1);
                    DateTime dtTO = Convert.ToDateTime(strdtTO);
                    string strdtFrom = cValue.Substring(0,19);
                    DateTime dtFrom = Convert.ToDateTime(strdtFrom);

                    if (dateTime > dtFrom && dateTime < dtTO)
                    {
                        movID = "END OF MONTH EXCEPTION";
                        movType = strLocationTo;
                    }
                    else
                    {
                        #region SAP Movement ps:FVN do sap movement in silver wip check in/out , not in supermarket
                        movID = DoSAP311Movement(strSN, strSKU, strPlant, "SMJN", strLocationTo, 1, strBU, empNO, sfcdb);
                        #endregion
                    }


                    smID = objSN.ID;
                }
                else
                {
                    strPlant = "TBD"; //FVN PLant name in SAP

                    #region FVN SM CheckOut Logic By PE 譚義康 2022-02-11
                    var objSM = new R_SUPERMARKET();
                    var dtSM = new DataTable();
                    var objSN = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();
                    if (objSN == null)
                    {
                        var sql = $@"select sm.id,sm.r_sn_id sn,sm.in_r_sap_mov_id skuno from r_supermarket sm where sm.status=1 and sm.in_r_sap_mov_id='{strSKU}' order by sm.in_time";
                        dtSM = sfcdb.RunSelect(sql).Tables[0];
                    }
                    else
                    {
                        if (objSN.SHIPPED_FLAG == "1")
                        {
                            throw new Exception($@"SN: {strSN} has been shipped, pls check!");
                        }
                        var sql = $@"select sm.id,s.sn,s.skuno from r_supermarket sm, r_sn s where sm.r_sn_id=s.id and s.valid_flag=1 and sm.status=1 and sm.in_r_sap_mov_id='{strSKU}' order by sm.in_time";
                        dtSM = sfcdb.RunSelect(sql).Tables[0];                       
                    }
                    if (dtSM.Rows.Count == 0)
                    {
                        throw new Exception($@"SN:{strSN} has already CheckOut Supermarket, pls check!");
                    }
                    if (dtSM.Rows[0]["SN"].ToString() != strSN)
                    {
                        throw new Exception($@"FIFO Check, pls check out SN:{dtSM.Rows[0]["SN"]} first!");
                    }
                    if (dtSM.Rows[0]["SKUNO"].ToString() != strSKU)
                    {
                        throw new Exception($@"SKU " + strSKU + " does not match with SKU for SN " + strSN);
                    }                    
                    #endregion

                    smID = dtSM.Rows[0]["ID"].ToString();
                    movID = strLocationTo;
                    movType = "Location";
                }

                #region Update R_SUPERMARKET
                //var objSM = sfcdb.ORM.Queryable<R_SUPERMARKET>().Where(sm => sm.ID == objSN.ID && sm.STATUS == "1").First();
                //objSM.OUT_TIME = dateTime;
                //objSM.OUT_BY = empNO;
                //objSM.OUT_R_SAP_MOV_ID = movID;
                //objSM.OUT_MOV_TYPE = "311";
                //objSM.STATUS = "0";

                sfcdb.ORM.Updateable<R_SUPERMARKET>()
                .UpdateColumns(a => new R_SUPERMARKET()
                {
                    OUT_TIME = dateTime,
                    OUT_BY = empNO,
                    OUT_R_SAP_MOV_ID = movID,
                    OUT_MOV_TYPE = movType,
                    STATUS = "0"
                }).Where(sm => sm.ID == smID && sm.STATUS == "1").ExecuteCommand();

                //var ret = sfcdb.ORM.Updateable(objSM).ExecuteCommand();
                #endregion

                if (string.IsNullOrEmpty(strSN))
                {
                    throw new Exception($@"Serial Number Cannot be empty");
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage("SNOutSupermarket: " + ex.Message);
                //sfcdb.RollbackTrain();
            }
        }

        /// <summary>
        /// Check In/Out 都需要檢查測試記錄 
        /// CheckIn:BuyPart檢查配置的工站,MakePart檢查完工(測試工站都在路由裡,完工即表示Pass)
        /// CheckOut:BuyPart/MakePart檢查配置的工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SilverWipSNTestRecordChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            string strSN = SNSession.Value.ToString();
            string strSKU = SKUSession.Value.ToString();
            //SN _sn = new SN(strSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            var sn = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").First();
            T_R_FUNCTION_CONTROL t_R_FUNCTION = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            
            if (Station.StationName == "IN_SILVER_WIP")
            {        
                if (sn == null)
                {
                    //BuyPart
                    var rfc = t_R_FUNCTION.GetListByFcv("CHECK_BUYPART_SN_TEST_RECORD", "PARTNO", strSKU, Station.SFCDB);
                    if (rfc.Count != 0)
                    {
                        T_R_TEST_RECORD t_R_TEST = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
                        var rt = t_R_TEST.GetLastTestRecord(strSN, rfc[0].EXTVAL, Station.SFCDB);
                        if (rt == null)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20190604110249");
                            throw new MESReturnMessage(ErrMessage);
                        }
                        else if (rt.STATE != "PASS")
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { strSN, rfc[0].EXTVAL });
                            throw new MESReturnMessage(ErrMessage);
                        }
                    }
                    //else
                    //{
                    //    throw new Exception($@"{strSKU} Has Not Set Up CHECK_BUYPART_SN_TEST_RECORD, Pls Call PE");
                    //}
                }
                else
                {
                    //MakePart
                    if (sn.COMPLETED_FLAG != "1")
                    {
                        throw new Exception($@"{strSN} not COMPLETED");
                    }
                    if (sn.CURRENT_STATION == "MRB")
                    {
                        throw new Exception($@"{strSN} is in MRB");
                    }
                }
            }            
            else if (Station.StationName == "OUT_SILVER_WIP")
            {
                var rfc = t_R_FUNCTION.GetListByFcv("CHECK_SWOUT_SN_TEST_RECORD", "PARTNO", strSKU, Station.SFCDB);
                if (rfc.Count != 0)
                {
                    T_R_TEST_RECORD t_R_TEST = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
                    var rt = t_R_TEST.GetLastTestRecord(strSN, rfc[0].EXTVAL, Station.SFCDB);
                    if (rt == null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20190604110249");
                        throw new MESReturnMessage(ErrMessage);
                    }
                    else if (rt.STATE != "PASS")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { strSN, rfc[0].EXTVAL });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                //else
                //{
                //    throw new Exception($@"{strSKU} Has Not Set Up CHECK_SWOUT_SN_TEST_RECORD, Pls Call PE");
                //}
            }
        }

        /// <summary>
        /// 判斷是否IDOA類型,是則直接跳到SHIPOUT工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void IDOATypeSkipToShipOut(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNo == null) //這裡傳的是PALLETNO 
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string palletNo = PackNo.Value.ToString();
            var woList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.ID == b.PARENT_PACK_ID && b.ID == c.PACK_ID && c.SN_ID == d.ID)
                .Where((a, b, c, d) => a.PACK_TYPE == "PALLET" && a.PACK_NO == palletNo).Select((a, b, c, d) => d.WORKORDERNO).Distinct().ToList();
            var isIDOA = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == woList[0] && t.ORDERTYPE == "IDOA").Any();
            if (isIDOA && Station.BU == "VNJUNIPER")
            {
                T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
                T_R_SN_STATION_DETAIL rRSnDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
                List<R_SN> rSnList = new List<R_SN>();
                tRSn.GetSNsByPackNo(palletNo, ref rSnList, Station.SFCDB);

                foreach (R_SN snobj in rSnList)
                {
                    R_SN TemplateSNObject = snobj;
                    if (TemplateSNObject.VALID_FLAG == "1")
                    {
                        TemplateSNObject.NEXT_STATION = "SHIPOUT";
                        TemplateSNObject.EDIT_TIME = tRSn.GetDBDateTime(Station.SFCDB);
                        TemplateSNObject.EDIT_EMP = Station.LoginUser.EMP_NO;// Station.User.EMP_NO;
                        var result = tRSn.Update(TemplateSNObject, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + snobj.SN, "UPDATE" }));
                        }

                        var detailId = rRSnDetail.GetNewID(Station.BU, Station.SFCDB);
                        var res = rRSnDetail.AddDetailToRSnStationDetail(detailId, TemplateSNObject, Station.Line, "SHIPOUT", $@"IDOA Skip Over {snobj.NEXT_STATION}", Station.SFCDB);
                        bool b = int.TryParse(res, out result);
                        if (!b)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + snobj.SN, "UPDATE" }));
                        }
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + snobj.SN, "UPDATE" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { snobj.SN }));
                    }
                }
            }
        }

        public static void ReverseByTypeJuniper(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                MESStationSession vType = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (vType == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                MESStationSession vInput = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (vInput == null) //這裡傳的是PALLETNO 
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }

                MESStationSession vReason = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (vReason == null) //這裡傳的是PALLETNO 
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }

                string strUser = Station.LoginUser.EMP_NO;
                string strType = vType.Value.ToString();
                string strInput = vInput.Value.ToString();
                string strReason = vReason.Value.ToString();

                if (string.IsNullOrEmpty(strReason))
                {
                    throw new MESReturnMessage("Reason cannot be empty");
                }

                if (strReason.Length > 4000)
                {
                    throw new MESReturnMessage("Reason cannot be grater than 4000 characters");
                }

                var sfcdb = Station.SFCDB;

                string res = MesDbBase.ReverseJuniperByType(strType, strInput, strUser, strReason, sfcdb);
                if (res == "REVERSE_SUCCESSFULL")
                {
                    Station.AddMessage("MSGCODEREVERSEJNP00001", new string[] { res }, StationMessageState.CMCMessage);
                }
                else
                {
                    throw new MESReturnMessage(res);
                }
            }
            catch(Exception ex)
            {
                throw new MESReturnMessage(ex.Message);
            }
           
        }

        public static void ReworkBuySnAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                MESStationSession vSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (vSN == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                MESStationSession vWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (vWO == null) //這裡傳的是PALLETNO 
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }


                string strUser = Station.LoginUser.EMP_NO;
                string strSN = vSN.Value.ToString();
                string strWO = vWO.Value.ToString();
                var sfcdb = Station.SFCDB;
                string strBU = Station.BU;
                DateTime dateTime = sfcdb.ORM.GetDate();
                
                #region Born New SN

                string r_snID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_SN");
                string detail_snID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_SN_STATION_DETAIL");
                var objWO = sfcdb.ORM.Queryable<R_WO_BASE>().Where(w => w.WORKORDERNO == strWO).First();
                var objSN = sfcdb.ORM.Queryable<R_SN>().Where(rsn => rsn.SN == strSN && rsn.VALID_FLAG == "1").First();
                string woID = objWO.ID;
                string strSKU = objWO.SKUNO;
                string routeID = objWO.ROUTE_ID;

                if(objWO.INPUT_QTY == objWO.WORKORDER_QTY)
                {
                    throw new MESReturnMessage($@"WO {strWO} is full. Please select a new one." );
                }

                //var objRoute = sfcdb.ORM.Queryable<C_ROUTE_DETAIL>().Where(c => c.ROUTE_ID == routeID).Select(c => c.STATION_NAME).ToList();
                //int listCount = objRoute.Count();

                //string currStation = objRoute[listCount-1];

                R_SN RSN = new R_SN()
                {
                    ID = r_snID,
                    SN = strSN,
                    SKUNO = strSKU,
                    WORKORDERNO = strWO,
                    PLANT = "MBGA",
                    ROUTE_ID = routeID,
                    STARTED_FLAG = "1",
                    START_TIME = dateTime,
                    PACKED_FLAG = "0",
                    COMPLETED_FLAG = "0",
                    SHIPPED_FLAG = "0",
                    REPAIR_FAILED_FLAG = "0",
                    CURRENT_STATION = "EXTERNAL-REWORK",
                    NEXT_STATION = "JOBFINISH",
                    CUST_PN = strSKU,
                    SCRAPED_FLAG = "0",
                    PRODUCT_STATUS = "BUY PRODUCT",
                    REWORK_COUNT = 0,
                    VALID_FLAG = "1",
                    EDIT_EMP = strUser,
                    EDIT_TIME = dateTime
                };

                R_SN_STATION_DETAIL RSSD = new R_SN_STATION_DETAIL()
                {
                    ID = detail_snID,
                    R_SN_ID = r_snID,
                    SN = strSN,
                    SKUNO = strSKU,
                    WORKORDERNO = strWO,
                    PLANT = "FJZ",
                    ROUTE_ID = routeID,
                    LINE = "Line1",
                    STARTED_FLAG = "1",
                    START_TIME = dateTime,
                    PACKED_FLAG = "0",
                    COMPLETED_FLAG = "0",
                    SHIPPED_FLAG = "0",
                    REPAIR_FAILED_FLAG = "0",
                    CURRENT_STATION = "EXTERNAL-REWORK",
                    NEXT_STATION = "JOBFINISH",
                    CUST_PN = strSKU,
                    DEVICE_NAME = "BUY-PRODUCT",
                    STATION_NAME = "EXTERNAL-REWORK",
                    PRODUCT_STATUS = "FRESH",
                    REWORK_COUNT = 0,
                    VALID_FLAG = "1",
                    EDIT_EMP = strUser,
                    EDIT_TIME = dateTime
                };

                #endregion

                #region Update WO Info

                objWO.INPUT_QTY = objWO.INPUT_QTY + 1;
                objWO.FINISHED_QTY = objWO.FINISHED_QTY + 1;
                objWO.EDIT_TIME = dateTime;
                objWO.EDIT_EMP = strUser;

                if(objWO.FINISHED_QTY == objWO.WORKORDER_QTY)
                {
                    objWO.CLOSED_FLAG = "1";
                    objWO.CLOSE_DATE = dateTime;    
                }

                sfcdb.ORM.Updateable(objWO).Where(t => t.ID == objWO.ID).ExecuteCommand();
                sfcdb.ORM.Insertable(RSN).ExecuteCommand();
                sfcdb.ORM.Insertable(RSSD).ExecuteCommand();

                #endregion
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(ex.Message);
            }

        }
        //

        public static void WarehouseSNInAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                string ErrMessage;
                MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SKUSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }

                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (SNSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }

                MESStationSession LocationFromSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (LocationFromSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                string strSN = SNSession.Value.ToString();

                if (!string.IsNullOrEmpty(Station.SFCDB.ORM.Queryable<R_WHS_SN>().Where(t => t.SN == strSN).First().ToString()))
                {
                    throw new Exception($@"This SN has already been in Warehouse!");
                }
                
                string strSKU = SKUSession.Value.ToString();
                string LocationFrom = LocationFromSession.Value.ToString();

                if (LocationFrom == "SELECT LOCATION")
                {
                    throw new Exception($@"Please choose correct location");
                }

                string strLocationFrom = LocationFrom.Substring(LocationFrom.IndexOf('~') + 2);
                string strBU = Station.BU;
                var sfcdb = Station.SFCDB;
                string strPlant = "", movID = "";
                string strUser = Station.LoginUser.EMP_NO;
                string movType = null;
                DateTime dateTime = sfcdb.ORM.GetDate();
                string virtualWO = MesDbBase.GetNewWorkorder(sfcdb.ORM, "JNPWHSVWO");


                //string strWO = vWO.Value.ToString();

                #region Born New SN

                string r_snID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_SN");
                string detail_snID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_SN_STATION_DETAIL"); //R_WHS_SN
                string whsID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_WHS_SN"); //R_WHS_SN

                //movID = DoSAP311Movement(strSN, strSKU, "MBGA", strLocationFrom, "FGJN", 1, strBU, Station.LoginUser.EMP_NO, sfcdb);

                R_SN RSN = new R_SN()
                {
                    ID = r_snID,
                    SN = strSN,
                    SKUNO = strSKU,
                    WORKORDERNO = virtualWO,
                    PLANT = "MBGA",
                    ROUTE_ID = "FJZ000000002Q5",
                    STARTED_FLAG = "1",
                    START_TIME = dateTime,
                    PACKED_FLAG = "0",
                    COMPLETED_FLAG = "0",
                    SHIPPED_FLAG = "0",
                    REPAIR_FAILED_FLAG = "0",
                    CURRENT_STATION = "WAREHOUSE-SN-IN",
                    NEXT_STATION = "WAREHOUSE-SN-OUT",
                    CUST_PN = strSKU,
                    SCRAPED_FLAG = "0",
                    PRODUCT_STATUS = "BUY PRODUCT",
                    REWORK_COUNT = 0,
                    VALID_FLAG = "1",
                    EDIT_EMP = strUser,
                    EDIT_TIME = dateTime
                };

                R_SN_STATION_DETAIL RSSD = new R_SN_STATION_DETAIL()
                {
                    ID = detail_snID,
                    R_SN_ID = r_snID,
                    SN = strSN,
                    SKUNO = strSKU,
                    WORKORDERNO = virtualWO,
                    PLANT = "FJZ",
                    ROUTE_ID = "FJZ000000002Q5",
                    LINE = "Line1",
                    STARTED_FLAG = "1",
                    START_TIME = dateTime,
                    PACKED_FLAG = "0",
                    COMPLETED_FLAG = "0",
                    SHIPPED_FLAG = "0",
                    REPAIR_FAILED_FLAG = "0",
                    CURRENT_STATION = "WAREHOUSE-SN-IN",
                    NEXT_STATION = "WAREHOUSE-SN-OUT",
                    CUST_PN = strSKU,
                    DEVICE_NAME = "BUY-PRODUCT",
                    STATION_NAME = "WAREHOUSE-SN-IN",
                    PRODUCT_STATUS = "FRESH",
                    REWORK_COUNT = 0,
                    VALID_FLAG = "1",
                    EDIT_EMP = strUser,
                    EDIT_TIME = dateTime
                };

                #endregion

                #region WO Info

                string woID = MesDbBase.GetNewID(sfcdb.ORM, strBU, "R_WO_BASE");


                R_WO_BASE RWB = new R_WO_BASE()
                {
                    ID = woID,
                    WORKORDERNO = virtualWO,
                    PLANT = "FJZ",
                    RELEASE_DATE = dateTime,
                    DOWNLOAD_DATE = dateTime,
                    PRODUCTION_TYPE = "MODEL",
                    WO_TYPE = "VIRTUAL",
                    SKUNO = strSKU,
                    ROUTE_ID = "FJZ000000002Q5",
                    START_STATION = "SILOADING",
                    CLOSED_FLAG = "0",
                    WORKORDER_QTY = 1,
                    INPUT_QTY = 0,
                    FINISHED_QTY = 0,
                    SCRAPED_QTY = 0,
                    EDIT_EMP = strUser,
                    EDIT_TIME = dateTime
                };

                var newWHS = new R_WHS_SN()
                {
                    ID = whsID,
                    SN = strSN,
                    SKUNO = strSKU,
                    IN_TIME = dateTime,
                    IN_BY = strUser,
                    IN_MOV_TYPE = "",
                    IN_R_SAP_MOV_ID = "",
                    STATUS = "1",
                };

                var resWHS = sfcdb.ORM.Insertable(newWHS).ExecuteCommand();
                sfcdb.ORM.Insertable(RSN).ExecuteCommand();
                sfcdb.ORM.Insertable(RSSD).ExecuteCommand();
                sfcdb.ORM.Insertable(RWB).ExecuteCommand();
                #endregion

            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(ex.Message);
            }

        }
    }
}