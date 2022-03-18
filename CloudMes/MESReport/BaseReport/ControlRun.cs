using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.OleDb;


namespace MESReport.BaseReport
{
    public class ControlRun : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();


        private APIInfo _GetMainMenuList = new APIInfo()
        {

            FunctionName = "GetMainMenuList",
            Description = "頁面主頁的報表",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetExtendListStation = new APIInfo()
        {

            FunctionName = "GetExtendListStation",
            Description = "獲取子報表，CONTROLID對應工站",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _NewControlRun = new APIInfo()
        {

            FunctionName = "NewControlRun",
            Description = "新增Control Run",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _DelControlRun = new APIInfo()
        {

            FunctionName = "DelControlRun",
            Description = "刪除Control Run單號",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _ModifyControlRun = new APIInfo()
        {

            FunctionName = "ModifyControlRun",
            Description = "編輯Control Run單號",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _UploadExcelSN = new APIInfo()
        {

            FunctionName = "UploadExcelSN",
            Description = "上传 SN 至 选择单号",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _UploadExcelWO = new APIInfo()
        {

            FunctionName = "UploadExcelWO",
            Description = "上传WO 至 选择单号",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _ShowFailRecord = new APIInfo()
        {
            FunctionName = "ShowFailRecord",
            Description = "ShowFailRecord",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        private APIInfo _GetStation = new APIInfo()
        {
            FunctionName = "getStation",
            Description = "getStation",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        private APIInfo _GetEditStation = new APIInfo()
        {

            FunctionName = "GetEditStation",
            Description = "編輯時獲取已有工站",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetYieldRate = new APIInfo()
        {

            FunctionName = "GetYieldRate",
            Description = "獲取工站生產通過率報表",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };


        public ControlRun()
        {
            Apis.Add(_GetMainMenuList.FunctionName, _GetMainMenuList);
            Apis.Add(_GetExtendListStation.FunctionName, _GetExtendListStation);
            Apis.Add(_NewControlRun.FunctionName, _NewControlRun);
            Apis.Add(_DelControlRun.FunctionName, _DelControlRun);
            Apis.Add(_ModifyControlRun.FunctionName, _ModifyControlRun);
            Apis.Add(_UploadExcelSN.FunctionName, _UploadExcelSN);
            Apis.Add(_UploadExcelWO.FunctionName, _UploadExcelWO);
            Apis.Add(_ShowFailRecord.FunctionName, _ShowFailRecord);
            Apis.Add(_GetStation.FunctionName, _GetStation);
            Apis.Add(_GetEditStation.FunctionName, _GetEditStation);
            Apis.Add(_GetYieldRate.FunctionName, _GetYieldRate);
        }


        public void GetMainMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec sfcdb = null;

            try
            {
                int DetailQTY = 0;
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_CONTROLRUN TRC = new T_R_CONTROLRUN(sfcdb, DB_TYPE_ENUM.Oracle);
                T_R_CONTROLRUN_DETAIL TRCD = new T_R_CONTROLRUN_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                List<R_CONTROLRUN> lrc = TRC.GetMainMenuList(sfcdb);
                List<R_CONTROLRUN_MENU_LIST> MenuList = new List<R_CONTROLRUN_MENU_LIST>();
                foreach (var item in lrc)
                {
                    DetailQTY = TRCD.GetSNQTYbyControlID(item.ID, sfcdb);
                    MenuList.Add(new R_CONTROLRUN_MENU_LIST
                    {
                        ID = item.ID,
                        TYPE = item.TYPE,
                        QTY = DetailQTY,
                        CUS_DEV = item.CUS_DEV,
                        REVISION = item.REVISION,
                        STARTTIME = item.STARTTIME,
                        ENDTIME = item.ENDTIME,
                        STATUS = item.STATUS,
                        REASON = item.REASON,
                        COMMENTS = item.COMMENTS,
                        VALID_FLAG = item.VALID_FLAG,
                        EDITTIME = item.EDITTIME,
                        EDITBY = item.EDITBY,
                    }
                 );

                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = MenuList;


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
        public void GetExtendListStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            string CONTROLID = Data["CONTROLID"].ToString().Trim();

            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_CONTROLRUN_STATION TRCS = new T_R_CONTROLRUN_STATION(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_CONTROLRUN_STATION> Listtrcs = TRCS.GetListByControlID(CONTROLID,SFCDB);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = Listtrcs;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void NewControlRun(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            string TYPE = Data["TYPE"].ToString();
            string CUS_DEV = Data["CUS_DEV"].ToString();
            string REVISION = Data["REVISION"].ToString();
            DateTime STARTTIME = Convert.ToDateTime(Data["STARTTIME"].ToString());
            DateTime ENDTIME = Convert.ToDateTime(Data["ENDTIME"].ToString());
            string REASON = Data["REASON"].ToString();
            string COMMENTS = Data["COMMENTS"].ToString();
            var selectdStation=Data["STATIONDATA"];
            int Seqno = 0;
            try
            {   
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_CONTROLRUN_STATION TRCS = new T_R_CONTROLRUN_STATION(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_CONTROLRUN TRC = new T_R_CONTROLRUN(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_CONTROLRUN RowRC = (Row_R_CONTROLRUN)TRC.NewRow();
                RowRC.ID = TRC.GetNewID(BU, SFCDB);
                RowRC.TYPE = TYPE;
                RowRC.CUS_DEV = CUS_DEV;
                RowRC.REVISION = REVISION;
                RowRC.STARTTIME = STARTTIME;
                RowRC.ENDTIME = ENDTIME;
                RowRC.STATUS = "OPEN";
                RowRC.REASON = REASON;
                RowRC.COMMENTS = COMMENTS;
                RowRC.VALID_FLAG = "1";
                RowRC.EDITTIME = GetDBDateTime();
                RowRC.EDITBY = LoginUser.EMP_NO;

                foreach(var item in selectdStation)
                {
                    Row_R_CONTROLRUN_STATION RowRCS = (Row_R_CONTROLRUN_STATION)TRCS.NewRow();
                    RowRCS.ID= TRCS.GetNewID(BU, SFCDB);
                    RowRCS.CONTROLID = RowRC.ID;
                    RowRCS.STATION_NAME = item["value"].ToString();
                    RowRCS.VALID_FLAG = "1";
                    RowRCS.EDITTIME = GetDBDateTime();
                    RowRCS.EDITBY = LoginUser.EMP_NO;
                    RowRCS.SEQNO = Seqno;
                    Seqno++;

                    SFCDB.ExecSQL(RowRCS.GetInsertString(DBTYPE));
                }

                SFCDB.ExecSQL(RowRC.GetInsertString(DBTYPE));
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = RowRC.ID;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void DelControlRun(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            string ID = Data["ID"].ToString().Trim();

            try
            {
                T_R_CONTROLRUN TRC = new T_R_CONTROLRUN(db, DB_TYPE_ENUM.Oracle);
                TRC.DelControlRun(ID, db);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "";
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void ModifyControlRun(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            string ID = Data["ID"].ToString().Trim();
            string CUS_DEV = Data["CUS_DEV"].ToString().Trim();
            string REVISION = Data["REVISION"].ToString().Trim();
            DateTime STARTTIME = Convert.ToDateTime(Data["STARTTIME"].ToString());
            DateTime ENDTIME = Convert.ToDateTime(Data["ENDTIME"].ToString());
            string REASON = Data["REASON"].ToString().Trim();
            string COMMENTS = Data["COMMENTS"].ToString().Trim();
            var selectdStation = Data["STATIONDATA"];
            int Seqno = 0;

            try
            {
                T_R_CONTROLRUN TRC = new T_R_CONTROLRUN(db, DB_TYPE_ENUM.Oracle);
                T_R_CONTROLRUN_STATION TRCS = new T_R_CONTROLRUN_STATION(db, DB_TYPE_ENUM.Oracle);
                

                TRCS.SetInvalidByControlID(ID, LoginUser.EMP_NO, GetDBDateTime(),db);

                Row_R_CONTROLRUN RowRC = (Row_R_CONTROLRUN)TRC.GetObjByID(ID, db);
                RowRC.CUS_DEV = CUS_DEV;
                RowRC.REVISION = REVISION;
                RowRC.STARTTIME = STARTTIME;
                RowRC.ENDTIME = ENDTIME;
                RowRC.REASON = REASON;
                RowRC.COMMENTS = COMMENTS;
                RowRC.EDITTIME = GetDBDateTime();
                RowRC.EDITBY = LoginUser.EMP_NO;

                foreach (var item in selectdStation)
                {
                    Row_R_CONTROLRUN_STATION RowRCS = (Row_R_CONTROLRUN_STATION)TRCS.NewRow();
                    RowRCS.ID = TRCS.GetNewID(BU, db);
                    RowRCS.CONTROLID = RowRC.ID;
                    RowRCS.STATION_NAME = item["value"].ToString();
                    RowRCS.VALID_FLAG = "1";
                    RowRCS.EDITTIME = GetDBDateTime();
                    RowRCS.EDITBY = LoginUser.EMP_NO;
                    RowRCS.SEQNO = Seqno;
                    Seqno++;

                    db.ExecSQL(RowRCS.GetInsertString(DBTYPE));
                }
                
                db.ExecSQL(RowRC.GetUpdateString(DB_TYPE_ENUM.Oracle));

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "";
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void UploadExcelSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            var successCount = 0;
            var failCount = 0;

            string CONTROLID = Data["CONTROLID"].ToString();
            string data = Data["DataList"].ToString();

            try
            {
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                Newtonsoft.Json.Linq.JToken dataList = Data["DataList"];

                string SN = string.Empty;
                bool checkRCD = true;
                bool checkSN = true;


                for (int i = 0; i < array.Count; i++)
                {
                    T_R_CONTROLRUN_DETAIL TRCD = new T_R_CONTROLRUN_DETAIL(SFCDB, DBTYPE);
                    T_R_SN TRSN = new T_R_SN(SFCDB, DBTYPE);

                    SN = array[i]["Data"].ToString().Trim();
                    checkSN = TRSN.CheckSNExists(SN, SFCDB);
                    checkRCD = TRCD.CheckControlDataExist(CONTROLID, SN, SFCDB);


                    if (checkSN && !checkRCD)
                    {
                        try
                        {
                            Row_R_CONTROLRUN_DETAIL rowRCD = null;
                            rowRCD = (Row_R_CONTROLRUN_DETAIL)TRCD.NewRow();
                            rowRCD.ID = TRCD.GetNewID(BU, SFCDB);
                            rowRCD.CONTROLID = CONTROLID;
                            rowRCD.DATA = SN;
                            rowRCD.VALID_FLAG = "1";
                            rowRCD.EDITBY = LoginUser.EMP_NO;
                            rowRCD.EDITTIME = TRCD.GetDBDateTime(SFCDB);
                            SFCDB.ExecSQL(rowRCD.GetInsertString(DBTYPE));
                            successCount += 1;
                        }
                        catch (Exception ex)
                        {
                            InsertFailLog(ex.Message, SN, "", "UploadExcelSN", SFCDB);
                            failCount += 1;
                            continue;
                        }
                    }
                    else if (!checkSN)
                    {
                        // InsertFailLog("SN不存在", SN, "", "UploadExcelSN", SFCDB);
                        InsertFailLog("SN not exists", SN, "", "UploadExcelSN", SFCDB);

                        failCount += 1;
                    }
                    else if (checkRCD)
                    {
                        //InsertFailLog("SN已存在於該單號", SN, "", "UploadExcelSN", SFCDB);
                        InsertFailLog("SN exists in the odd numbers", SN, "", "UploadExcelSN", SFCDB);
                        failCount += 1;
                    }

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = failCount;
                //StationReturn.Message = $@"配置數據成功[{successCount.ToString()}]筆,失敗[{failCount.ToString()}]筆!";
                StationReturn.Message = $@"Data configuration succeeded[{successCount.ToString()}]records,Fail[{failCount.ToString()}]records!";
                SFCDB.CommitTrain();
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void UploadExcelWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            var successCount = 0;
            var failCount = 0;

            string CONTROLID = Data["CONTROLID"].ToString();
            string data = Data["DataList"].ToString();

            try
            {
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                Newtonsoft.Json.Linq.JToken dataList = Data["DataList"];

                string WO = string.Empty;
                bool checkRCD = true;
                bool checkWO = true;


                for (int i = 0; i < array.Count; i++)
                {
                    T_R_CONTROLRUN_DETAIL TRCD = new T_R_CONTROLRUN_DETAIL(SFCDB, DBTYPE);
                    T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBTYPE);
                    T_R_SN TRSN = new T_R_SN(SFCDB, DBTYPE);

                    WO = array[i]["Data"].ToString().Trim();
                    checkWO = TRWB.CheckDataExist(WO, SFCDB);



                    if (checkWO)
                    {
                        DataTable SNLIST = TRSN.GetSNByWo(WO, SFCDB);
                        foreach (DataRow sn in SNLIST.Rows) {
                            checkRCD = TRCD.CheckControlDataExist(CONTROLID, sn["SN"].ToString(), SFCDB);
                            if (!checkRCD) {
                                try
                                {
                                    Row_R_CONTROLRUN_DETAIL rowRCD = null;
                                    rowRCD = (Row_R_CONTROLRUN_DETAIL)TRCD.NewRow();
                                    rowRCD.ID = TRCD.GetNewID(BU, SFCDB);
                                    rowRCD.CONTROLID = CONTROLID;
                                    rowRCD.DATA = sn["SN"].ToString();
                                    rowRCD.VALID_FLAG = "1";
                                    rowRCD.EDITBY = LoginUser.EMP_NO;
                                    rowRCD.EDITTIME = TRCD.GetDBDateTime(SFCDB);
                                    SFCDB.ExecSQL(rowRCD.GetInsertString(DBTYPE));
                                    successCount += 1;
                                }
                                catch (Exception ex)
                                {
                                    InsertFailLog(ex.Message, WO, "", "UploadExcelWO", SFCDB);
                                    failCount += 1;
                                    continue;
                                }
                            }
                            else
                            {
                                //InsertFailLog("SN已存在於該單號", sn.ToString(), WO, "UploadExcelWO", SFCDB);
                                InsertFailLog("SN exists in the odd numbers", WO, "", "UploadExcelWO", SFCDB);
                                failCount += 1;
                            }
                        }
                    }
                    else
                    {
                        //InsertFailLog("WO不存在", WO, "", "UploadExcelWO", SFCDB);

                        InsertFailLog("SN not exists", WO, "", "UploadExcelWO", SFCDB);

                        failCount += 1;
                    }

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = failCount;
                // StationReturn.Message = $@"配置數據成功[{successCount.ToString()}]筆,失敗[{failCount.ToString()}]筆!";
                StationReturn.Message = $@"Data configuration succeeded[{successCount.ToString()}]records,Fail[{failCount.ToString()}]records!";
                SFCDB.CommitTrain();
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void ShowFailRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string FUNCTION_NAME = Data["FUNCTION_NAME"].ToString();
            string SearchTime = GetDBDateTime().AddSeconds(-12).ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_MES_LOG TRML = new T_R_MES_LOG(DB, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TRML.GetMESLog("CloudMES", "MESReport.BaseReport.ControlRun", FUNCTION_NAME, SearchTime, "", LoginUser.EMP_NO.ToString(), DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void GetStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec sfcdb = null;

            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION TCS = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<string> stationName = new List<string>();
                stationName = TCS.GetNormalStation(sfcdb);
                List<StationList> stationJsonList = new List<StationList>();
                foreach (var item in stationName)
                {
                    stationJsonList.Add( new StationList {
                        value=item.ToString(),
                        title= item.ToString()
                    });
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = stationJsonList;

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
        public void GetEditStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            string CONTROLID = Data["CONTROLID"].ToString().Trim();

            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_CONTROLRUN_STATION TRCS = new T_R_CONTROLRUN_STATION(SFCDB, DB_TYPE_ENUM.Oracle);
                List<string> ListStationName = TRCS.GetStationNameByControlID(CONTROLID, SFCDB);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ListStationName;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetYieldRate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            string CONTROLID = Data["CONTROLID"].ToString().Trim();
          
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_CONTROLRUN_STATION TRCS = new T_R_CONTROLRUN_STATION(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_CONTROLRUN_DETAIL TRCD = new T_R_CONTROLRUN_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
            
                List<string> ListStationName = TRCS.GetStationNameByControlID(CONTROLID, SFCDB);
                List<string> ListSN  = TRCD.GetSNListByControlID(CONTROLID,SFCDB);
                List<StationYieldRateReport> ListReport = new List<StationYieldRateReport>();
                int buildQTY = 0;
                int failQTY = 0;
                double failRate = 0.00;
                foreach (var item in ListStationName)
                {
                    buildQTY = TRCD.GetBuildQty(CONTROLID, item, SFCDB);
                    failQTY= TRCD.GetFailQty(CONTROLID, item, SFCDB);
                    failRate = buildQTY == 0 ? 0 : Math.Round(1-(Convert.ToDouble(failQTY) / Convert.ToDouble(buildQTY)), 5)*100;
                    ListReport.Add(new StationYieldRateReport
                    {
                        Staion = item,
                        BulidQTY= buildQTY,
                        FailQTY=failQTY,
                        FailRate = failRate == 0?"-": failRate.ToString()+"%",
                    }) ;
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ListReport;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        private void InsertFailLog(string LOG_MESSAGE, string Data1, string Data2, string FUNCTION_NAME, MESDBHelper.OleExec DB)
        {
            T_R_MES_LOG TRML = new T_R_MES_LOG(DB, DBTYPE);
            R_MES_LOG LOG = new R_MES_LOG
            {
                ID = TRML.GetNewID(BU, DB),
                DATA1 = Data1,
                DATA2 = Data2,
                FUNCTION_NAME = FUNCTION_NAME,
                CLASS_NAME = "MESReport.BaseReport.ControlRun",
                PROGRAM_NAME = "CloudMES",
                EDIT_TIME = GetDBDateTime(),
                EDIT_EMP = LoginUser.EMP_NO,
                LOG_MESSAGE = LOG_MESSAGE
            };
            TRML.InsertMESLog(LOG, DB);
        }

        protected class StationList
        {
            public string value { get; set; }
            public string title { get; set; }
        }

        protected class StationYieldRateReport
        {
            public string Staion { get; set; }
            public int BulidQTY { get; set; }
            public int FailQTY { get; set; }
            public string FailRate { get; set; }
        }
    }
}
