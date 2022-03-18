using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MESDataObject;
using MESDBHelper;
using System.Data;
using System.Threading;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.Stations.StationActions.DataCheckers;
using SqlSugar;
using MESStation.Label.Public;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Common;

namespace MESWebService
{
    /// <summary>
    /// VertivTestService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class VertivTestService : System.Web.Services.WebService
    {
        public static MESDBHelper.OleExecPool VertivSfbDbPool = new OleExecPool("VERTIVDB", false);
        public static MESDBHelper.OleExecPool VertivApDbPool = new OleExecPool("VERTIVAPDB", false);
        public MESServiceRes resObj = new MESServiceRes();
        public VertivTestService()
        {

        }

        [WebMethod]
        public string DBPoolStatus()
        {
            int MaxPoolSize = VertivSfbDbPool.MaxPoolSize;
            int ActiveTimeOut = VertivSfbDbPool.ActiveTimeOut;
            //string lendinfo = "", outPutinfo = "";
            List<string> SfclendsList = VertivSfbDbPool.ShowLend();
            List<string> AplendsList = VertivApDbPool.ShowLend();
            List<object> Sfcal = VertivSfbDbPool.GetAllStatus();
            List<object> Apal = VertivSfbDbPool.GetAllStatus();
            List<string> SfcOutPutList = VertivSfbDbPool.OutPutMessage;
            List<string> ApOutPutList = VertivSfbDbPool.OutPutMessage;
            List<object> o = new List<object>()
            {
                MaxPoolSize, ActiveTimeOut, "SFCLend:"+ SfclendsList.Count, SfclendsList, SfcOutPutList, Sfcal
                , "APLend:"+ AplendsList.Count, AplendsList, ApOutPutList, Apal
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(o);

        }

        [WebMethod]
        public string GetServerTime()
        {
            OleExec DB = VertivSfbDbPool.Borrow();
            string o = DB.ExecSelectOneValue("select sysdate from dual").ObjToString();
            return Newtonsoft.Json.JsonConvert.SerializeObject(o);

        }

        /// <summary>
        /// TE上传测试记录;
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="MODEL"></param>
        /// <param name="TESTTIME"></param>
        /// <param name="STATE"></param>
        /// <param name="STATION"></param>
        /// <param name="CELL"></param>
        /// <param name="OPERATOR"></param>
        /// <param name="ERROR_CODE"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes TestDataUploadMES(string SN, string MODEL, string TESTTIME, string STATE, string STATION,
            string CELL, string OPERATOR, string ERROR_CODE, string LINE)
        {
            //Sql注入;
            TestRecordData testRecord = new TestRecordData();

            #region DataCheck;

            try
            {
                testRecord = TestDataUploadMES_CheckInputData(SN, MODEL, TESTTIME, STATE, STATION, CELL, OPERATOR,
                    ERROR_CODE,LINE);
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int) StatusValue.fail;
                resObj.MessageCode = "MES00001";
                resObj.Message = "输入参数错误:" + e.ToString();
                return resObj;
            }

            #endregion

            OleExec DB = VertivSfbDbPool.Borrow();
            //OleExec DB = new OleExec("VERTIVDB",false);

            T_C_TEMES_STATION_MAPPING cTeMesStationMappingControl = new T_C_TEMES_STATION_MAPPING(DB,
                DB_TYPE_ENUM.Oracle);
            T_R_TEST_RECORD rTestRecordControl = new T_R_TEST_RECORD(DB, DB_TYPE_ENUM.Oracle);
            T_R_TEST_DETAIL_VERTIV rTestDetailVertivControl = new T_R_TEST_DETAIL_VERTIV(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN rSnControl = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);

            Row_R_TEST_RECORD rowRTestRecord = (Row_R_TEST_RECORD) rTestRecordControl.NewRow();
            Row_R_TEST_DETAIL_VERTIV rowRTestDetailVertiv = (Row_R_TEST_DETAIL_VERTIV) rTestDetailVertivControl.NewRow();
            try
            {
                C_TEMES_STATION_MAPPING cTeMesStationMapping = cTeMesStationMappingControl.GetTeMesStationMapping(DB,
                    STATION, "A");
                if (cTeMesStationMapping == null)
                {
                    resObj.Statusvalue = (int) StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:未在MES系統找到Te-Station:{STATION}對應的工站,請聯繫IT配置!";
                    return resObj;
                }

                R_SN rSn = rSnControl.LoadSN(SN, DB);
                //RTestRecord
                rowRTestRecord.ID = cTeMesStationMappingControl.GetNewID("VERTIV", DB);
                rowRTestRecord.R_SN_ID = rSn?.ID;
                rowRTestRecord.SN = testRecord.SN;
                rowRTestRecord.ENDTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestRecord.STARTTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestRecord.STATE = testRecord.STATE;
                rowRTestRecord.TEGROUP = "A";
                rowRTestRecord.TESTATION = testRecord.STATION;
                rowRTestRecord.MESSTATION = cTeMesStationMapping?.MES_STATION;
                rowRTestRecord.DETAILTABLE = "R_TEST_DETAIL_VERTIV";
                //RTestDetailVertiv
                rowRTestDetailVertiv.ID = rTestDetailVertivControl.GetNewID("VERTIV", DB);
                rowRTestDetailVertiv.R_TEST_RECORD_ID = rowRTestRecord.ID;
                rowRTestDetailVertiv.SN = testRecord.SN;
                rowRTestDetailVertiv.SKUNO = testRecord.MODEL;
                rowRTestDetailVertiv.CREATETIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestDetailVertiv.STATE = testRecord.STATE;
                rowRTestDetailVertiv.STATION = testRecord.STATION;
                rowRTestDetailVertiv.CELL = testRecord.CELL;
                rowRTestDetailVertiv.OPERATOR = testRecord.OPERATOR;
                rowRTestDetailVertiv.ERROR_CODE = testRecord.ERROR_CODE;
                rowRTestDetailVertiv.LINE = testRecord.LINE;
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int) StatusValue.fail;
                resObj.MessageCode = "MES00003";
                resObj.Message = "MESDB异常:" + e.ToString();
                VertivSfbDbPool.Return(DB);
                return resObj;
            }

            try
            {
                DB.BeginTrain();
                DB.ExecSQL(rowRTestDetailVertiv.GetInsertString(DB_TYPE_ENUM.Oracle));
                DB.ExecSQL(rowRTestRecord.GetInsertString(DB_TYPE_ENUM.Oracle));
                DB.CommitTrain();
                resObj.Statusvalue = (int) StatusValue.success;
                resObj.MessageCode = "";
                resObj.Message = "Upload Success!";
            }
            catch (Exception e)
            {
                DB.RollbackTrain();
                resObj.Statusvalue = (int) StatusValue.fail;
                resObj.MessageCode = "MES00002";
                resObj.Message = "写入MESERR:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }
            return resObj;
        }

        /// <summary>
        /// TE上传测试记录;
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="MODEL"></param>
        /// <param name="TESTTIME"></param>
        /// <param name="STATE"></param>
        /// <param name="STATION"></param>
        /// <param name="CELL"></param>
        /// <param name="OPERATOR"></param>
        /// <param name="ERROR_CODE"></param>
        /// <param name="BURNIN_TIME"></param>
        /// <param name="LINE"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes AgeingDataUploadMES(string SN, string MODEL, string TESTTIME, string STATE, string STATION,
            string CELL, string OPERATOR, string ERROR_CODE,string BURNIN_TIME,string LINE)
        {
            //Sql注入;
            TestRecordData testRecord = new TestRecordData();

            #region DataCheck;

            try
            {
                testRecord = AgeingDataUploadMES_CheckInputData(SN, MODEL, TESTTIME, STATE, STATION, CELL, OPERATOR,
                    ERROR_CODE, BURNIN_TIME, LINE);
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00001";
                resObj.Message = "输入参数错误:" + e.ToString();
                return resObj;
            }

            #endregion

            OleExec DB = VertivSfbDbPool.Borrow();
            DB.ThrowSqlExeception = true;
            //OleExec DB = new OleExec("VERTIVDB",false);

            T_C_TEMES_STATION_MAPPING cTeMesStationMappingControl = new T_C_TEMES_STATION_MAPPING(DB,
                DB_TYPE_ENUM.Oracle);
            T_R_TEST_RECORD rTestRecordControl = new T_R_TEST_RECORD(DB, DB_TYPE_ENUM.Oracle);
            T_R_TEST_DETAIL_VERTIV rTestDetailVertivControl = new T_R_TEST_DETAIL_VERTIV(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN rSnControl = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);

            Row_R_TEST_RECORD rowRTestRecord = (Row_R_TEST_RECORD)rTestRecordControl.NewRow();
            Row_R_TEST_DETAIL_VERTIV rowRTestDetailVertiv = (Row_R_TEST_DETAIL_VERTIV)rTestDetailVertivControl.NewRow();
            T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(DB, DB_TYPE_ENUM.Oracle);
            //R_SN_LOCK r_sn_lock = null;
            try
            {
                C_TEMES_STATION_MAPPING cTeMesStationMapping = cTeMesStationMappingControl.GetTeMesStationMapping(DB,
                    STATION, "A");
                if (cTeMesStationMapping == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:未在MES系統找到Te-Station:{STATION}對應的工站,請聯繫IT配置!";
                    return resObj;
                }

                if (!BurninTimeIsValid(testRecord.SN, testRecord.BURNIN_TIME, DB))
                {
                    //r_sn_lock = t_r_sn_lock.GetLockObject("", "SN", testRecord.SN, "", "BURNIN_TIME_ERROR", STATION, DB);
                    //if (r_sn_lock == null)
                    //{
                    //    t_r_sn_lock.AddNewLock("VERTIV", "", "SN", testRecord.SN, "", STATION, "BURNIN_TIME_ERROR", "web_service", DB);
                    //    resObj.Statusvalue = (int)StatusValue.fail;
                    //    resObj.MessageCode = "MES000017";
                    //    resObj.Message = $@"上傳失敗:上傳的BURNIN_TIME與PQE配置的時間不一致，已被鎖定，請找PQE解鎖!";
                    //    return resObj;
                    //}
                    //if (r_sn_lock.LOCK_STATUS == "1")
                    //{
                    //    resObj.Statusvalue = (int)StatusValue.fail;
                    //    resObj.MessageCode = "MES000017";
                    //    resObj.Message = $@"上傳失敗:上傳的BURNIN_TIME與PQE配置的時間不一致，已被鎖定，請找PQE解鎖!";
                    //    return resObj;
                    //}
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:老化時間與設置不符，請與TE&PQE確認!";
                    return resObj;
                }

                R_SN rSn = rSnControl.LoadSN(SN, DB);
                //RTestRecord
                rowRTestRecord.ID = cTeMesStationMappingControl.GetNewID("VERTIV", DB);
                rowRTestRecord.R_SN_ID = rSn?.ID;
                rowRTestRecord.SN = testRecord.SN;
                rowRTestRecord.ENDTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestRecord.STARTTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestRecord.STATE = testRecord.STATE;
                rowRTestRecord.TEGROUP = "A";
                rowRTestRecord.TESTATION = testRecord.STATION;
                rowRTestRecord.MESSTATION = cTeMesStationMapping?.MES_STATION;
                rowRTestRecord.DETAILTABLE = "R_TEST_DETAIL_VERTIV";
                //RTestDetailVertiv
                rowRTestDetailVertiv.ID = rTestDetailVertivControl.GetNewID("VERTIV", DB);
                rowRTestDetailVertiv.R_TEST_RECORD_ID = rowRTestRecord.ID;
                rowRTestDetailVertiv.SN = testRecord.SN;
                rowRTestDetailVertiv.SKUNO = testRecord.MODEL;
                rowRTestDetailVertiv.CREATETIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestDetailVertiv.STATE = testRecord.STATE;
                rowRTestDetailVertiv.STATION = testRecord.STATION;
                rowRTestDetailVertiv.CELL = testRecord.CELL;
                rowRTestDetailVertiv.OPERATOR = testRecord.OPERATOR;
                rowRTestDetailVertiv.ERROR_CODE = testRecord.ERROR_CODE;
                rowRTestDetailVertiv.BURNIN_TIME = testRecord.BURNIN_TIME;
                rowRTestDetailVertiv.LINE = testRecord.LINE;
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00003";
                resObj.Message = "上傳失敗:" + e.ToString();
                VertivSfbDbPool.Return(DB);
                return resObj;
            }

            try
            {
                DB.BeginTrain();
                DB.ExecSQL(rowRTestDetailVertiv.GetInsertString(DB_TYPE_ENUM.Oracle));
                DB.ExecSQL(rowRTestRecord.GetInsertString(DB_TYPE_ENUM.Oracle));
                DB.CommitTrain();
                resObj.Statusvalue = (int)StatusValue.success;
                resObj.MessageCode = "";
                resObj.Message = "Upload Success!";
            }
            catch (Exception e)
            {
                DB.RollbackTrain();
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00002";
                resObj.Message = "写入MESERR:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }
            return resObj;
        }


        /// <summary>
        /// TE上传LLT测试记录;
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="MODEL"></param>
        /// <param name="TESTTIME"></param>
        /// <param name="STATE"></param>
        /// <param name="STATION"></param>
        /// <param name="CELL"></param>
        /// <param name="OPERATOR"></param>
        /// <param name="ERROR_CODE"></param>
        /// <param name="BURNIN_TIME"></param>
        /// <param name="LINE"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes LLTDataUploadMES(string SN, string MODEL, string TESTTIME, string STATE, string STATION,
            string CELL, string OPERATOR, string ERROR_CODE, float BURNIN_TIME, string LINE)
        {
            //Sql注入;
            TestRecordData testRecord = new TestRecordData();

            #region DataCheck;

            try
            {
                testRecord = AgeingDataUploadMES_CheckInputData(SN, MODEL, TESTTIME, STATE, STATION, CELL, OPERATOR,
                    ERROR_CODE, BURNIN_TIME.ToString(), LINE);
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00001";
                resObj.Message = "输入参数错误:" + e.ToString();
                return resObj;
            }

            #endregion

            OleExec DB = VertivSfbDbPool.Borrow();
            DB.ThrowSqlExeception = true;
            //OleExec DB = new OleExec("VERTIVDB",false);

            T_C_TEMES_STATION_MAPPING cTeMesStationMappingControl = new T_C_TEMES_STATION_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            T_R_LLT_TEST RLLTTestControl = new T_R_LLT_TEST(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN rSnControl = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            T_R_LLT Rllt = new T_R_LLT(DB, DB_TYPE_ENUM.Oracle);

            Row_R_LLT_TEST rowRLttTest = (Row_R_LLT_TEST)RLLTTestControl.NewRow();
            try
            {
                C_TEMES_STATION_MAPPING cTeMesStationMapping = cTeMesStationMappingControl.GetTeMesStationMapping(DB,
                    STATION, "A");
                if (cTeMesStationMapping == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:未在MES系統找到Te-Station:{STATION}對應的工站,請聯繫IT配置!";
                    return resObj;
                }

                R_SN rSn = rSnControl.LoadSN(SN, DB);

                var IsLLTSN = DB.ORM.Queryable<R_LLT>().Where(r => r.SN == testRecord.SN && (r.STATUS == "0" || r.STATUS == "1")).ToList().FirstOrDefault() ;
                if (IsLLTSN == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:上傳的SN:{testRecord.SN}失敗，此SN非測試LLT SN!";
                    return resObj;
                }

                Row_R_LLT_TEST IsTwoHour = RLLTTestControl.IsTwoHour(SN, DB);
                if (IsTwoHour == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:上傳測試時間不足兩小時!";
                    return resObj;
                }

                //RLLTTest
                rowRLttTest.ID = RLLTTestControl.GetNewID("VERTIV", DB);
                rowRLttTest.R_SN_ID = rSn?.ID;
                rowRLttTest.SN = testRecord.SN;
                rowRLttTest.STATUS = "PASS";
                rowRLttTest.TESTATION = testRecord.STATION;
                rowRLttTest.MESSTATION = cTeMesStationMapping?.MES_STATION;
                rowRLttTest.ENDTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRLttTest.BURNIN_TIME = BURNIN_TIME.ToString();
                rowRLttTest.CELL = CELL;
                rowRLttTest.TESTTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRLttTest.ENDTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRLttTest.CREATETIME = DateTime.Parse(testRecord.TESTTIME);
                rowRLttTest.CREATEBY = OPERATOR;

                DB.BeginTrain();
                DB.ExecSQL(rowRLttTest.GetInsertString(DB_TYPE_ENUM.Oracle));
                DB.CommitTrain();

                //更改有被抽中過且正在測試未完成的SN LTT狀態

                Row_R_LLT_TEST Testtime1 = RLLTTestControl.GetTestTime1(SN, DB);

                var LLTSNID =DB.ORM.Queryable<R_LLT>().Where (r=>r.R_SN_ID== rowRLttTest.R_SN_ID).ToList().FirstOrDefault();

                if (Testtime1 != null)
                {
                    Row_R_LLT R_TTL = (Row_R_LLT)Rllt.GetObjByID(LLTSNID.ID, DB);
                    R_TTL.STATUS = "1";
                    var result = DB.ExecSQL(R_TTL.GetUpdateString(DB_TYPE_ENUM.Oracle));
                }
                //更改已經測試完成的SN LTT狀態

                Row_R_LLT_TEST Testtime2 = RLLTTestControl.GetTestTime2(SN, DB);
                if (Testtime2 != null)
                {
                    Row_R_LLT R_TTL = (Row_R_LLT)Rllt.GetObjByID(LLTSNID.ID, DB);
                    R_TTL.STATUS = "2";
                    var result = DB.ExecSQL(R_TTL.GetUpdateString(DB_TYPE_ENUM.Oracle));
                }

            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00003";
                resObj.Message = "上傳失敗:" + e.ToString();
                VertivSfbDbPool.Return(DB);
                return resObj;
            }

            try
            {
                //DB.BeginTrain();
                //DB.ExecSQL(rowRLttTest.GetInsertString(DB_TYPE_ENUM.Oracle));
                //DB.CommitTrain();
                resObj.Statusvalue = (int)StatusValue.success;
                resObj.MessageCode = "";
                resObj.Message = "Upload Success!";
            }
            catch (Exception e)
            {
                DB.RollbackTrain();
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00002";
                resObj.Message = "写入MESERR:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }
            return resObj;
        }

        /// <summary>
        /// TE上传测试记录;
        /// </summary>
        /// <param name="CATEGORYNAME"></param>
        /// <param name="SN"></param>
        /// <param name="VAL"></param>
        /// <param name="STATION"></param>
        /// <param name="EDITBY"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes UploadIdentityMES(string CATEGORYNAME, string SN, string VAL, string STATION, string EDITBY)
        {

            OleExec DB = VertivSfbDbPool.Borrow();
            DB.ThrowSqlExeception = true;

            T_C_TEMES_STATION_MAPPING cTeMesStationMappingControl = new T_C_TEMES_STATION_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            T_R_TEST_IDENTITY IDENTestControl = new T_R_TEST_IDENTITY(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN rSnControl = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);

            Row_R_TEST_IDENTITY IDENTest = (Row_R_TEST_IDENTITY)IDENTestControl.NewRow();
            try
            {
                C_TEMES_STATION_MAPPING cTeMesStationMapping = cTeMesStationMappingControl.GetTeMesStationMapping(DB,
                    STATION, "A");
                if (cTeMesStationMapping == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:未在MES系統找到Te-Station:{STATION}對應的工站,請聯繫IT配置!";
                    return resObj;
                }

                R_SN rSn = rSnControl.LoadSN(SN, DB);

                if (rSn == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:輸入的SN錯誤,SN在MES系統中不存在!";
                    return resObj;
                }

                if (CATEGORYNAME != "ITEM" && CATEGORYNAME != "MAC")
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:輸入的CATEGORYNAME錯誤,應為ITEM或者MAC!";
                    return resObj;
                }

                var IsExist = DB.ORM.Queryable<R_TEST_IDENTITY>().Where(r => r.SN == SN && r.CATEGORYNAME == CATEGORYNAME && r.VAL == VAL && r.VALIDFLAG == "1").ToList().FirstOrDefault();
                if (IsExist == null)
                {
                    //RLLTTest
                    IDENTest.ID = IDENTestControl.GetNewID("VERTIV", DB);
                    IDENTest.R_SN_ID = rSn?.ID;
                    IDENTest.SN = SN;
                    IDENTest.CATEGORYNAME = CATEGORYNAME;
                    IDENTest.VAL = VAL;
                    IDENTest.STATION = cTeMesStationMapping?.MES_STATION;
                    IDENTest.EDITBY = EDITBY;
                    IDENTest.ENDTIME = System.DateTime.Now;
                    IDENTest.CREATEBY = EDITBY;
                    IDENTest.CREATETIME = System.DateTime.Now;
                    IDENTest.VALIDFLAG = "1";

                    DB.BeginTrain();
                    DB.ExecSQL(IDENTest.GetInsertString(DB_TYPE_ENUM.Oracle));
                    DB.CommitTrain();
                }
                else
                {
                    Row_R_TEST_IDENTITY IdenTy = (Row_R_TEST_IDENTITY)IDENTestControl.GetObjByID(IsExist.ID, DB);
                    IdenTy.VALIDFLAG = "0";
                    IdenTy.EDITBY = EDITBY;
                    IdenTy.ENDTIME = System.DateTime.Now;
                    var result = DB.ExecSQL(IdenTy.GetUpdateString(DB_TYPE_ENUM.Oracle));

                    IDENTest.ID = IDENTestControl.GetNewID("VERTIV", DB);
                    IDENTest.R_SN_ID = rSn?.ID;
                    IDENTest.SN = SN;
                    IDENTest.CATEGORYNAME = CATEGORYNAME;
                    IDENTest.VAL = VAL;
                    IDENTest.STATION = cTeMesStationMapping?.MES_STATION;
                    IDENTest.EDITBY = EDITBY;
                    IDENTest.ENDTIME = System.DateTime.Now;
                    IDENTest.CREATEBY = EDITBY;
                    IDENTest.CREATETIME = System.DateTime.Now;
                    IDENTest.VALIDFLAG = "1";

                    DB.BeginTrain();
                    DB.ExecSQL(IDENTest.GetInsertString(DB_TYPE_ENUM.Oracle));
                    DB.CommitTrain();
                }
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00003";
                resObj.Message = "上傳失敗:" + e.ToString();
                VertivSfbDbPool.Return(DB);
                return resObj;
            }

            try
            {
                resObj.Statusvalue = (int)StatusValue.success;
                resObj.MessageCode = "";
                resObj.Message = "Upload Success!";
            }
            catch (Exception e)
            {
                DB.RollbackTrain();
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00002";
                resObj.Message = "写入MESERR:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }
            return resObj;
        }
       
        /// <summary>
        /// TE刪除测试记录;
        /// </summary>
        /// <param name="CATEGORYNAME"></param>
        /// <param name="SN"></param>
        /// <param name="VAL"></param>
        /// <param name="STATION"></param>
        /// <param name="EDITBY"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes DeleteIdentityData(string CATEGORYNAME, string SN, string VAL, string STATION, string EDITBY)
        {

            OleExec DB = VertivSfbDbPool.Borrow();
            DB.ThrowSqlExeception = true;

            T_C_TEMES_STATION_MAPPING cTeMesStationMappingControl = new T_C_TEMES_STATION_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            T_R_TEST_IDENTITY IDENTestControl = new T_R_TEST_IDENTITY(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN rSnControl = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);

            Row_R_TEST_IDENTITY IDENTest = (Row_R_TEST_IDENTITY)IDENTestControl.NewRow();
            try
            {
                C_TEMES_STATION_MAPPING cTeMesStationMapping = cTeMesStationMappingControl.GetTeMesStationMapping(DB,
                    STATION, "A");
                if (cTeMesStationMapping == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"UPDATE失敗:未在MES系統找到Te-Station:{STATION}對應的工站,請聯繫IT配置!";
                    return resObj;
                }

                R_SN rSn = rSnControl.LoadSN(SN, DB);

                if (rSn == null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"UPDATE失敗:輸入的SN錯誤,SN在MES系統中不存在!";
                    return resObj;
                }

                if (CATEGORYNAME != "ITEM" && CATEGORYNAME != "MAC")
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"UPDATE失敗:輸入的CATEGORYNAME錯誤,應為ITEM或者MAC!";
                    return resObj;
                }

                var IsExist = DB.ORM.Queryable<R_TEST_IDENTITY>().Where(r => r.SN == SN && r.CATEGORYNAME == CATEGORYNAME && r.VAL == VAL && r.VALIDFLAG == "1").ToList().FirstOrDefault();
                if (IsExist != null)
                {
                    Row_R_TEST_IDENTITY IdenTy = (Row_R_TEST_IDENTITY)IDENTestControl.GetObjByID(IsExist.ID, DB);
                    IdenTy.VALIDFLAG = "0";
                    IdenTy.EDITBY = EDITBY;
                    IdenTy.ENDTIME = System.DateTime.Now;
                    var result = DB.ExecSQL(IdenTy.GetUpdateString(DB_TYPE_ENUM.Oracle));
                }
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00003";
                resObj.Message = "上傳失敗:" + e.ToString();
                VertivSfbDbPool.Return(DB);
                return resObj;
            }

            try
            {
                resObj.Statusvalue = (int)StatusValue.success;
                resObj.MessageCode = "";
                resObj.Message = "Update Success!";
            }
            catch (Exception e)
            {
                DB.RollbackTrain();
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00002";
                resObj.Message = "写入MESERR:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }
            return resObj;
        }


        [WebMethod]
        public MESServiceRes PassTestStation(string SN,string STATION,string TESTLINE)
        {
            //MESDBHelper.OleExecPool VertivSfbDbPool = new OleExecPool("VERTIVDB", false);
            //MESDBHelper.OleExecPool VertivApDbPool = new OleExecPool("VERTIVDB", false);
            CallWebService c = new CallWebService(VertivSfbDbPool, VertivApDbPool);
            resObj = new MESServiceRes {Message= "", Statusvalue = (int)StatusValue.success, MessageCode = ""};
            MESStationReturn s = new MESStationReturn();
            try
            {
                #region StationPara setting
                StationPara sp = new StationPara { Station = STATION, Line = TESTLINE,Bu= "VERTIV" };
                #endregion

                #region InitStation
                c.InitStation(s,sp);
                #endregion

                #region Setting Inputs Value
                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = (MESPubLab.MESStation.MESReturnView.Station.CallStationReturn)s.Data;
                ((MESStationBase)ret.Station).Inputs[0].Value = SN;
                #endregion

                #region Doing Inputs Events
                c.StationInput(s, "PASS", "SN",false);
                #endregion

                #region setting run results
                foreach (var stationRes in ((MESStationBase)ret.Station).StationMessages)
                {
                    if (stationRes.State == StationMessageState.Fail)
                    {
                        resObj = new MESServiceRes { Message = stationRes.Message , Statusvalue = (int)StatusValue.fail, MessageCode = "MES00011" };
                        break;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                resObj = new MESServiceRes { Message = e.Message, Statusvalue = (int)StatusValue.fail, MessageCode = "MES00012" };
            }

            return resObj;
        }

        /// <summary>
        /// 過站前檢查狀態,不檢查測試記錄
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="STATION"></param>
        /// <param name="TESTLINE"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes PassTestStationCheck(string SN, string STATION, string TESTLINE)
        {
            //MESDBHelper.OleExecPool VertivSfbDbPool = new OleExecPool("VERTIVDB", false);
            //MESDBHelper.OleExecPool VertivApDbPool = new OleExecPool("VERTIVDB", false);
            CallWebService c = new CallWebService(VertivSfbDbPool, VertivApDbPool);
            resObj = new MESServiceRes { Message = "", Statusvalue = (int)StatusValue.success, MessageCode = "" };
            MESStationReturn s = new MESStationReturn();
            try
            {
                #region StationPara setting

                StationPara sp = new StationPara {Station = STATION, Line = TESTLINE, Bu = "VERTIV"};

                #endregion

                #region InitStation

                c.InitStation(s, sp);

                #endregion

                #region Setting Inputs Value

                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret =
                    (MESPubLab.MESStation.MESReturnView.Station.CallStationReturn) s.Data;
                ((MESStationBase)ret.Station).Inputs[0].Value = SN;

                #endregion

                #region Doing Inputs Events

                c.StationInput(s, "PASS", "SN", true);

                #endregion

                #region CheckTestRecordWithBeforeStation

                try
                {
                   ( (MESStationBase)ret.Station).SFCDB = VertivSfbDbPool.Borrow();
                    CheckSN.SNTestCheckStationBefore((MESStationBase)ret.Station, ((MESStationBase)ret.Station).Inputs[0], null);
                    VertivSfbDbPool.Return(((MESStationBase)ret.Station).SFCDB);
                }
                catch (Exception e)
                {
                    ((MESStationBase)ret.Station).StationMessages.Add(new StationMessage()
                    {
                        Message = e.Message,
                        State = StationMessageState.Fail
                    });
                }

                #endregion


                #region setting run results

                foreach (var stationRes in ((MESStationBase)ret.Station).StationMessages)
                {
                    if (stationRes.State == StationMessageState.Fail &&
                        stationRes.Message.IndexOf("檢查SN對應工站的測試記錄") == -1)
                    {
                        resObj = new MESServiceRes
                        {
                            Message = stationRes.Message,
                            Statusvalue = (int) StatusValue.fail,
                            MessageCode = "MES00011"
                        };
                        break;
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                resObj = new MESServiceRes
                {
                    Message = e.Message,
                    Statusvalue = (int) StatusValue.fail,
                    MessageCode = "MES00012"
                };
            }
            //finally
            //{
            //    VertivSfbDbPool.FreePool();
            //    VertivApDbPool.FreePool();
            //}

            return resObj;
        }

        [WebMethod]
        public MESServiceRes PassTestStation_Test(string WO,string SN, string STATION, string TESTLINE)
        {
            //MESDBHelper.OleExecPool VertivSfbDbPool = new OleExecPool("VERTIVDB", false);
            //MESDBHelper.OleExecPool VertivApDbPool = new OleExecPool("VERTIVDB", false);
            CallWebService c = new CallWebService(VertivSfbDbPool, VertivApDbPool);
            resObj = new MESServiceRes { Message = "", Statusvalue = (int)StatusValue.success, MessageCode = "" };
            MESStationReturn s = new MESStationReturn();
            try
            {
                #region StationPara setting

                StationPara sp = new StationPara {Station = STATION, Line = TESTLINE, Bu = "VERTIV"};

                #endregion

                #region InitStation

                c.InitStation(s, sp);

                #endregion

                #region Setting Inputs Valu

                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret =
                    (MESPubLab.MESStation.MESReturnView.Station.CallStationReturn) s.Data;
                ((MESStationBase)ret.Station).Inputs[0].Value = "002328000011";

                #endregion

                #region Doing Inputs Events

                c.StationInput(s, "PASS", "WO", false);
                ((MESStationBase)ret.Station).Inputs[1].Value = SN;
                c.StationInput(s, "PASS", "SN", false);

                #endregion

                #region setting run results

                foreach (var stationRes in ((MESStationBase)ret.Station).StationMessages)
                {
                    if (stationRes.State == StationMessageState.Fail)
                    {
                        resObj = new MESServiceRes
                        {
                            Message = stationRes.Message,
                            Statusvalue = (int) StatusValue.fail,
                            MessageCode = "MES00011"
                        };
                        break;
                    }
                }

                #endregion
            }
            catch (Exception)
            {
                resObj = new MESServiceRes
                {
                    Message = s.Message,
                    Statusvalue = (int) StatusValue.fail,
                    MessageCode = "MES00012"
                };
            }
            //finally
            //{
            //    VertivSfbDbPool.FreePool();
            //    VertivApDbPool.FreePool();
            //}

            return resObj;
        }


       /// <summary>
       /// 多連板
       /// </summary>
       /// <param name="WO"></param>
       /// <param name="TRSN"></param>
       /// <param name="LINKQTY"></param>
       /// <param name="SN"></param>
       /// <param name="TESTLINE"></param>
       /// <param name="isCommit"></param>
       /// <returns></returns>
        [WebMethod]
        public MESServiceRes SmtLoadingWithPanel(string WO, string TRSN, string LINKQTY, string SN, string LINE,bool COMMIT=false)
        {
            CallWebService c = new CallWebService(VertivSfbDbPool, VertivApDbPool);
            resObj = new MESServiceRes { Message = "", Statusvalue = (int)StatusValue.success, MessageCode = "" };
            MESStationReturn s = new MESStationReturn();
            try
            {
                #region StationPara setting
                StationPara sp = new StationPara { Station = "SFC_SMT_LOADING", Line = LINE, Bu = "VERTIV" };
                #endregion

                #region InitStation

                c.InitStation(s, sp);

                #endregion

                #region Setting Inputs Value

                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret =
                    (MESPubLab.MESStation.MESReturnView.Station.CallStationReturn)s.Data;
                ((MESStationBase)ret.Station).Inputs[0].Value = WO;
                ((MESStationBase)ret.Station).Inputs[1].Value = TRSN;
                ((MESStationBase)ret.Station).Inputs[2].Value = LINKQTY;
                ((MESStationBase)ret.Station).Inputs[3].Value = SN;

                #endregion

                #region Doing Inputs Events
                foreach (var item in ((MESStationBase)ret.Station).Inputs)
                    if (!doStationEvent(c, s, ret, item.DisplayName, !COMMIT))
                    {
                        return new MESServiceRes
                        {
                            Message = ((MESStationBase)ret.Station).StationMessages.FindAll(t => t.State == StationMessageState.Fail).FirstOrDefault().Message,
                            Statusvalue = (int)StatusValue.fail,
                            MessageCode = "MES00011"
                        };
                    }    
                #endregion
            }
            catch (Exception e)
            {
                resObj = new MESServiceRes
                {
                    Message = e.Message,
                    Statusvalue = (int)StatusValue.fail,
                    MessageCode = "MES00012"
                };
            }

            return resObj;
        }

        /// <summary>
        /// 單板
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="TRSN"></param>
        /// <param name="LINKQTY"></param>
        /// <param name="SN"></param>
        /// <param name="TESTLINE"></param>
        /// <param name="isCommit"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes SmtLoading(string WO, string TRSN, string SN, string LINE, bool COMMIT = false)
        {
            CallWebService c = new CallWebService(VertivSfbDbPool, VertivApDbPool);
            resObj = new MESServiceRes { Message = "", Statusvalue = (int)StatusValue.success, MessageCode = "" };
            MESStationReturn s = new MESStationReturn();
            try
            {
                #region StationPara setting
                StationPara sp = new StationPara { Station = "PCBA_SMTLOADING", Line = LINE, Bu = "VERTIV" };
                #endregion

                #region InitStation

                c.InitStation(s, sp);

                #endregion

                #region Setting Inputs Value

                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret =
                    (MESPubLab.MESStation.MESReturnView.Station.CallStationReturn)s.Data;
                ((MESStationBase)ret.Station).Inputs[0].Value = WO;
                ((MESStationBase)ret.Station).Inputs[1].Value = TRSN;
                ((MESStationBase)ret.Station).Inputs[2].Value = SN;

                #endregion

                #region Doing Inputs Events
                foreach (var item in ((MESStationBase)ret.Station).Inputs)
                    if (!doStationEvent(c, s, ret, item.DisplayName, !COMMIT))
                    {
                        return new MESServiceRes
                        {
                            Message = ((MESStationBase)ret.Station).StationMessages.FindAll(t => t.State == StationMessageState.Fail).FirstOrDefault().Message,
                            Statusvalue = (int)StatusValue.fail,
                            MessageCode = "MES00011"
                        };
                    }
                #endregion
            }
            catch (Exception e)
            {
                resObj = new MESServiceRes
                {
                    Message = e.Message,
                    Statusvalue = (int)StatusValue.fail,
                    MessageCode = "MES00012"
                };
            }

            return resObj;
        }

        /// <summary>
        /// return false=>fail;true=>ok
        /// </summary>
        /// <param name="c"></param>
        /// <param name="s"></param>
        /// <param name="ret"></param>
        /// <param name="inputName"></param>
        /// <param name="isCheck"></param>
        /// <returns></returns>
        bool doStationEvent(CallWebService c, MESStationReturn s, CallStationReturn ret, string inputName, bool isCheck)
        {
            c.StationInput(s, "PASS", inputName, isCheck);
            var checkfail = ((MESStationBase)ret.Station).StationMessages.FindAll(t => t.State == StationMessageState.Fail).FirstOrDefault();
            if (checkfail != null)
                return false;
            return true;
        }

        /// <summary>
        /// 获取上传到MES的测试记录
        /// </summary>
        /// <param name="SN"></param>
        /// <returns>MESServiceRes</returns> 
        [WebMethod]
        public MESServiceRes GetTestDataFromMES(string SN)
        {
            //DataCheck;
            //Sql注入;
            OleExec DB= VertivSfbDbPool.Borrow();
            //OleExec DB = new OleExec("VERTIVDB", false);
            //Thread.Sleep(10000);
            try
            {
                T_R_TEST_DETAIL_VERTIV rTestDetailVertivControl = new T_R_TEST_DETAIL_VERTIV(DB, DB_TYPE_ENUM.Oracle);
                DataTable dt = new DataTable();

                dt = rTestDetailVertivControl.GetDTRTestDetailVertivBySn(DB, SN);
                string resDtjson = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"});
                if (dt.Rows.Count > 0)
                {
                    resObj.Statusvalue = (int) StatusValue.success;
                    resObj.MessageCode = "";
                    resObj.Message = resDtjson;
                }
                else
                {
                    resObj.Statusvalue = (int) StatusValue.fail;
                    resObj.MessageCode = "MES00005";
                    resObj.Message = "No Data!";
                }
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int) StatusValue.fail;
                resObj.MessageCode = "MES00004";
                resObj.Message = "查询错误:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }

            return resObj;
        }

        /// <summary>
        /// 获取SN的KP
        /// </summary>
        /// <param name="SN"></param>
        /// <returns>MESServiceRes</returns>
        [WebMethod]
        public MESServiceRes GetKpDataBySn(string SN,string MAC)
        {
            OleExec DB = VertivSfbDbPool.Borrow();
            try
            {
                DataTable dt = DB.ORM.Queryable<R_SN_KP,R_SN>((rsk,rs)=>rsk.SN==rs.SN)
                    .Where((rsk, rs)=>rsk.VALID_FLAG==1 && rs.VALID_FLAG=="1" && rs.SN==SN )
                    .Select((rsk, rs) =>rsk).ToDataTable();
                string resDtjson = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                if (dt.Rows.Count > 0)
                {
                    resObj.Statusvalue = (int)StatusValue.success;
                    resObj.MessageCode = "";
                    resObj.Message = resDtjson;
                }
                else
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES00005";
                    resObj.Message = $@"{SN} haven't link other bar codes!";
                }
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00004";
                resObj.Message = "查询错误:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }

            return resObj;
        }


        /// <summary>
        /// 获取SN的KP
        /// </summary>
        /// <param name="SN"></param>
        /// <returns>MESServiceRes</returns>
        [WebMethod]
        public MESServiceRes GetIdentityData(string SN,string VAL)
        {
            OleExec DB = VertivSfbDbPool.Borrow();
            try
            {
                DataTable dt = new DataTable();
                if (SN != "" && VAL != "")
                {
                    dt = DB.ORM.Queryable<R_TEST_IDENTITY>()
                       .Where((rs) => rs.SN == SN && rs.VAL == VAL && rs.VALIDFLAG == "1")
                       .Select((rs) => rs).ToDataTable();
                }
                if (SN != "" && VAL == "")
                {
                    dt = DB.ORM.Queryable<R_TEST_IDENTITY>()
                       .Where((rs) => rs.SN == SN && rs.VALIDFLAG == "1")
                       .Select((rs) => rs).ToDataTable();
                }
                if (SN == "" && VAL != "")
                {
                    dt = DB.ORM.Queryable<R_TEST_IDENTITY>()
                       .Where((rs) => rs.VAL == VAL && rs.VALIDFLAG == "1")
                       .Select((rs) => rs).ToDataTable();
                }

                string resDtjson = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                if (dt.Rows.Count > 0)
                {
                    resObj.Statusvalue = (int)StatusValue.success;
                    resObj.MessageCode = "";
                    resObj.Message = resDtjson;
                }
                else
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES00005";
                    resObj.Message = $@"{SN} No Data!";
                }
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00004";
                resObj.Message = "查询错误:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }

            return resObj;
        }


        /// <summary>
        /// TE獲取MAC
        /// </summary>
        /// <param name="SN"></param>
        /// <returns>MESServiceRes</returns>
        [WebMethod]
        public MESServiceRes GetMacBySn(string SN)
        {
            OleExec DB = VertivSfbDbPool.Borrow();
            try
            {
                var MacClass = new MACValueGroup();
                var snobj = DB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                var res = MacClass.GetWONewMACRange(DB, snobj.WORKORDERNO, "1", snobj.CURRENT_STATION, "TE", "").FirstOrDefault();
                resObj.Statusvalue = (int)StatusValue.success;
                resObj.MessageCode = "";
                resObj.Message = res;
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00004";
                resObj.Message = "Err:" + e.ToString();
            }
            finally
            {
                VertivSfbDbPool.Return(DB);
                //DB.FreeMe();
            }
            return resObj;
        }


        TestRecordData TestDataUploadMES_CheckInputData(string SN, string MODEL, string TESTTIME, string STATE, string STATION, string CELL, string OPERATOR, string ERROR_CODE,string LINE)
        {
            TestRecordData testRecord = new TestRecordData();
            testRecord.SN = SN;
            testRecord.MODEL = MODEL;
            testRecord.TESTTIME = TESTTIME;
            testRecord.STATE = STATE;
            testRecord.STATION = STATION;
            testRecord.CELL = CELL;
            testRecord.OPERATOR = OPERATOR;
            testRecord.ERROR_CODE = ERROR_CODE;
            testRecord.LINE = LINE;
            return testRecord;
        }

        TestRecordData AgeingDataUploadMES_CheckInputData(string SN, string MODEL, string TESTTIME, string STATE, string STATION, string CELL, string OPERATOR, string ERROR_CODE,string BURNIN_TIME,string LINE)
        {
            TestRecordData testRecord = new TestRecordData();
            testRecord.SN = SN;
            testRecord.MODEL = MODEL;
            testRecord.TESTTIME = TESTTIME;
            testRecord.STATE = STATE;
            testRecord.STATION = STATION;
            testRecord.CELL = CELL;
            testRecord.OPERATOR = OPERATOR;
            testRecord.ERROR_CODE = ERROR_CODE;
            testRecord.BURNIN_TIME = BURNIN_TIME;
            testRecord.LINE = LINE;
            return testRecord;
        }

        public bool BurninTimeIsValid(string sn, string burninTime, OleExec DB)
        {
            bool isValid = true;
            R_SN snObject = DB.ORM.Queryable<R_SN>().Where(r => (r.SN == sn || r.BOXSN == sn) && r.VALID_FLAG == "1").ToList().FirstOrDefault();           
            if (snObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { sn }));
            }
            R_WO_AGEING woAging = DB.ORM.Queryable<R_WO_AGEING>().Where(r => r.WO == snObject.WORKORDERNO).ToList().FirstOrDefault();
            if (woAging != null)
            {
                if (woAging.AGEING_AREA_CODE == null || woAging.AGEING_AREA_CODE.ToString() == "")
                {
                    throw new Exception($@"PC沒有配置工單{snObject.WORKORDERNO}出貨地址");
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181029140651", new string[] { snObject.WORKORDERNO }));
                }
                if (woAging.AGEING_TYPE == null || woAging.AGEING_TYPE.ToString() == "")
                {
                    throw new Exception($@"PQE沒有配置工單{snObject.WORKORDERNO}老化階段");
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181029140941", new string[] { snObject.WORKORDERNO }));
                }
                if (burninTime != woAging.MAX_AGEING_TIME && burninTime != woAging.MIN_AGEING_TIME)
                {
                    isValid = false;
                }
            }
            else
            {
                throw new Exception($@"PC&PQE沒有配置工單{snObject.WORKORDERNO}老化參數");//2018.11.1程序測試階段，工單沒有配置先不卡；2018.12.14應黃友要求全部卡
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181029141139", new string[] { snObject.WORKORDERNO }));
            }           
            return isValid;
        }

        /// <summary>
        /// Service Return Obj
        /// </summary>
        public class MESServiceRes
        { 
            public int Statusvalue;
            public string MessageCode;
            public string Message;
        }

        public enum StatusValue
        {
            success = 0,
            fail =1
        }

        public class TestRecordData
        {
            //string SN,string MODEL,string TESTTIME,string STATE,string STATION,string CELL,string OPERATOR,string ERROR_CODE
            private string sn;
            private string model;
            private string testtime;
            private string state;
            private string station;
            private string cell;
            private string operatordata;
            private string errorcode;
            private string burnintime;
            private string line;
            //public TestRecordData(string _sn, string _model, string _testtime, string _state, string _station, string _cell, string _operatordata, string _errorcode)
            //{
            //    sn = _sn;
            //    model = _model;
            //    TESTTIME = _testtime;
            //    state = _state;
            //    station = _station;
            //    cell = _cell;
            //    operatordata = _operatordata;
            //    errorcode = _errorcode;
            //}
            public string SN
            {
                get
                {
                    return sn;
                } 
                set {
                    if (value.Length > 30)
                        throw new Exception("SN长度超过30");
                    sn = value;
                }
            }

            public string MODEL
            {
                get
                {
                    return model;
                }
                set
                {
                    if (value.Length > 20)
                        throw new Exception("MODEL长度超过20");
                    model = value;
                }
            }

            public string TESTTIME
            {
                get
                {
                    return testtime;
                }
                set
                {
                    try
                    {
                       DateTime.Parse(value);
                       testtime = value;
                    }
                    catch(Exception)
                    {
                        throw new Exception("TESTTIME格式應為:yyyy-mm-dd hh24:mi:ss");
                    }
                }
            }
            public string STATE
            {
                get
                {
                    return state;
                }
                set
                {
                    if (value.Length > 5)
                        throw new Exception("STATE长度超过5");
                    switch (value.ToUpper().Trim())
                    {
                        case "P": state="PASS";break;
                        case "R": state = "R"; break;
                        case "F": state = "FAIL"; break;
                        case "PASS": state = "PASS"; break;
                        case "FAIL": state = "FAIL"; break;
                        default: throw new Exception("STATE格式有误!");
                    }
                }
            }
            public string STATION
            {
                get
                {
                    return station;
                }
                set
                {
                    if (value.Length > 10)
                        throw new Exception("STATION长度超过10");
                    station = value;
                }
            }
            public string CELL
            {
                get
                {
                    return cell;
                }
                set
                {
                    if (value.Length > 5)
                        throw new Exception("CELL长度超过5");
                    cell = value;
                }
            }
            public string OPERATOR
            {
                get
                {
                    return operatordata;
                }
                set
                {
                    if (value.Length > 10)
                        throw new Exception("OPERATOR长度超过10");
                    operatordata = value;
                }
            }
            public string ERROR_CODE
            {
                get
                {
                    return errorcode;
                }
                set
                {
                    if (value.Length > 120)
                        throw new Exception("ERROR_CODE长度超过120");
                    errorcode = value;
                }
            }
            
            public string BURNIN_TIME {
                get
                {
                    return burnintime;
                }
                set
                {
                    if (value.Length > 10)
                        throw new Exception("BURNIN_TIME长度超过10");
                    burnintime = value;
                }
            }

            public string LINE
            {
                get
                {
                    return line;
                }
                set
                {
                    if (value.Length > 20)
                        throw new Exception("LINE长度超过10");
                    line = value;
                }
            }
        }

    }
}
