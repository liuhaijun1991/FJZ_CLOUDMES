using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class OutLineTestConfig : MesAPIBase
    { 
        protected APIInfo FSearch = new APIInfo()
        {
            FunctionName = "Search",
            Description = "Search",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "TYPE", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "SEARCH_TEXT", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "PAGE_SIZE", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "PAGE_NUM", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewRecord = new APIInfo()
        {
            FunctionName = "AddNewRecord",
            Description = "AddNewRecord",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "TYPE", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "LINE", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "STATION", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAction = new APIInfo()
        {
            FunctionName = "Action",
            Description = "Action",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "ROW_OBJECT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDelete = new APIInfo()
        {
            FunctionName = "Delete",
            Description = "Delete",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "ROW_OBJECT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDownload = new APIInfo()
        {
            FunctionName = "Download",
            Description = "Download",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "TYPE", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "SEARCH_TEXT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };        
        protected APIInfo FGetSeleteValue = new APIInfo()
        {
            FunctionName = "GetSeleteValue",
            Description = "GetSeleteValue",
            Parameters = new List<APIInputInfo>()
            {                 
            },
            Permissions = new List<MESPermission>() { }
        };
        public OutLineTestConfig()
        {
            this.Apis.Add(FSearch.FunctionName, FSearch);
            this.Apis.Add(FAddNewRecord.FunctionName, FAddNewRecord);           
            this.Apis.Add(FAction.FunctionName, FAction);
            this.Apis.Add(FDelete.FunctionName, FDelete);
            this.Apis.Add(FDownload.FunctionName, FDownload);
            this.Apis.Add(FGetSeleteValue.FunctionName, FGetSeleteValue);
        }

        public void GetSeleteValue(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<string> listType = new List<string>();
                List<string> listLine = new List<string>();
                List<string> listStation = new List<string>();
                string sql = $@"select distinct line_name from c_line order by line_name";
                DataTable dt = SFCDB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    listLine.Add(row["LINE_NAME"].ToString());
                }
                sql = $@"select distinct station_name from c_station order by station_name";
                dt = SFCDB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    listStation.Add(row["STATION_NAME"].ToString());
                }
                sql = "select * from R_OUTLINE_TEST where data_type='DATA_TYPE' order by CREAT_DATE asc";
                dt = SFCDB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    listType.Add(row["SN"].ToString());
                }
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
               
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add("OK");
                StationReturn.Data = new { Type = listType, Line = listLine, Station = listStation };
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void AddNewRecord(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string type = Data["TYPE"] == null ? "" : Data["TYPE"].ToString();
                string line = Data["LINE"] == null ? "" : Data["LINE"].ToString();
                string station = Data["STATION"] == null ? "" : Data["STATION"].ToString();
                string sn = Data["SN"] == null ? "" : Data["SN"].ToString().Trim();
                if (string.IsNullOrEmpty(type))
                {
                    throw new Exception("Type Error!");
                }
                if (string.IsNullOrEmpty(line))
                {
                    throw new Exception("Please Input Line");
                }
                if (string.IsNullOrEmpty(station))
                {
                    throw new Exception("Please Input Station");
                }
                if (string.IsNullOrEmpty(sn))
                {
                    throw new Exception("Please Input SN");
                }
                T_R_OUTLINE_TEST t_r_outline_test = new T_R_OUTLINE_TEST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_SN t_r_sn = new T_R_SN(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                R_SN sn_obj = t_r_sn.LoadSN(sn, SFCDB);
                if (sn_obj == null)
                {
                    throw new Exception($@"{sn} Not Exist!");
                }
                if (t_r_outline_test.SNExist(SFCDB, type, sn))
                {
                    throw new Exception($@"{sn} Already Exist!");
                }
                R_WO_BASE wo_obj = t_r_wo_base.GetWoByWoNo(sn_obj.WORKORDERNO, SFCDB);
                if (wo_obj == null)
                {
                    throw new Exception($@"WO:{sn_obj.WORKORDERNO} Not Exist!");
                }
                if (wo_obj.CLOSED_FLAG == "1")
                {
                    throw new Exception($@"WO:{sn_obj.WORKORDERNO} Already Closed!");
                }

                R_OUTLINE_TEST test_obj = new R_OUTLINE_TEST();
                test_obj.ID = t_r_outline_test.GetNewID(BU, SFCDB);
                test_obj.SN = sn_obj.SN;
                test_obj.WORKORDERNO = sn_obj.WORKORDERNO;
                test_obj.SKUNO = sn_obj.SKUNO;
                test_obj.LINE = line;
                test_obj.STATION = station;
                test_obj.STATUS = "0";//0--WAIT_TEST,1--PASS,2--Fail
                test_obj.VALID_FLAG = 1;
                test_obj.CREAT_DATE = t_r_outline_test.GetDBDateTime(SFCDB);
                test_obj.CREAT_EMP = LoginUser.EMP_NO;
                test_obj.LASTEDIT_DATE = test_obj.CREAT_DATE;
                test_obj.LASTEDIT_BY = LoginUser.EMP_NO;
                test_obj.DATA_TYPE = type;
                int result = t_r_outline_test.Save(SFCDB, test_obj);
                if (result == 0)
                {
                    throw new Exception("Save Data Error!");
                }
                
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add("OK");
                StationReturn.Data = "OK";
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void Search(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string status_sql = "";
                if (Data["STATUS"] != null)
                {
                    status_sql = Data["STATUS"].ToString().ToUpper() == "ALL" ? "" : $@" and status='{Data["STATUS"].ToString()}'";
                }
                string page_size = Data["PAGE_SIZE"] == null ? "10" : Data["PAGE_SIZE"].ToString();
                string page_num = Data["PAGE_NUM"] == null ? "1" : Data["PAGE_NUM"].ToString();
                string type_sql = Data["TYPE"] == null ? "" : $@" and data_type='{Data["TYPE"].ToString()}'";
                string search_sql = (Data["SEARCH_TEXT"] == null || Data["SEARCH_TEXT"].ToString() == "") ? "" : $@" and ( sn like '%{ Data["SEARCH_TEXT"].ToString()}%' or workorderno like '%{ Data["SEARCH_TEXT"].ToString()}%'  or skuno like '%{ Data["SEARCH_TEXT"].ToString()}%')";

                string total_sql = $@"select * from R_OUTLINE_TEST where valid_flag='1' {status_sql} {type_sql} {search_sql} order by CREAT_DATE desc";
                DataTable total_dt = SFCDB.ExecSelect(total_sql, null).Tables[0];               
                if (total_dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");
                }

                string page_sql = $@"select ID,DATA_TYPE,SN,WORKORDERNO,SKUNO,STATION,LINE,decode(STATUS,'0','WAIT_TEST','1','PASS','2','FAIL','ERROR') as STATUS,
                                    FAIL_DESC,CREAT_DATE,CREAT_EMP,LASTEDIT_DATE,LASTEDIT_BY 
                                    from (select temp.*,rownum as rn from ({total_sql} ) temp )
                                    where rn> ({page_num} - 1) * {page_size} and rn<= {page_num} * {page_size}";
                DataTable page_dt = SFCDB.ExecSelect(page_sql, null).Tables[0];
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
               
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                StationReturn.Data = new { Total = total_dt.Rows.Count, Rows = page_dt };
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void Action(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string id = Data["DATA_ROW"]["ID"] == null ? "" : Data["DATA_ROW"]["ID"].ToString();
                string status= Data["STATUS"]== null ? "" : Data["STATUS"].ToString();
                string fail_desc = Data["FAIL_DESC"] == null ? "" : Data["FAIL_DESC"].ToString();
                T_R_OUTLINE_TEST t_r_outline_test = new T_R_OUTLINE_TEST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                DateTime systemTime = t_r_outline_test.GetDBDateTime(SFCDB);
                R_OUTLINE_TEST test_obj = SFCDB.ORM.Queryable<R_OUTLINE_TEST>().Where(r => r.ID == id).ToList().FirstOrDefault();
                if (test_obj == null)
                {
                    throw new Exception($@"ID:{id} Not Exist!");
                }
                TimeSpan timeSpan = DateTime.Now.Subtract((DateTime)test_obj.CREAT_DATE);
                if ((timeSpan.Hours * 60 + timeSpan.Minutes) < 10)
                {
                    throw new Exception("Please take to test");
                }
                //0--WAIT_TEST,1--PASS,2--Fail
                switch (status)
                {
                    case "PASS":
                        test_obj.STATUS = "1";
                        test_obj.FAIL_DESC = "";
                        break;
                    case "FAIL":
                        if (string.IsNullOrEmpty(fail_desc))
                        {
                            throw new Exception("Please Input Fail Desc!");
                        }
                        test_obj.FAIL_DESC = fail_desc;
                        test_obj.STATUS = "2";
                        break;
                    default:
                        throw new Exception($@"Status[{status}] Error!");                        
                }
                test_obj.LASTEDIT_BY = LoginUser.EMP_NO;
                test_obj.LASTEDIT_DATE = systemTime;
                int result = t_r_outline_test.Update(SFCDB, test_obj);
                if (result == 0)
                {
                    throw new Exception("Update Data Error!");
                }

                //namespace     MESStation.Stations.StationActions.ActionRunners
                //class         SampleAction
                //functionname  Taken2DXSample
                //2DXSample Insert Into R_SN_LOCK
                R_SN_LOCK lockObj = SFCDB.ORM.Queryable<R_SN_LOCK>()
                    .Where(r => r.SN == test_obj.SN && r.WORKORDERNO == test_obj.WORKORDERNO && r.LOCK_STATION == test_obj.STATION && r.LOCK_REASON == test_obj.DATA_TYPE && r.LOCK_EMP == "SYSTEM" && r.TYPE == "SN" && r.LOCK_STATUS == "1")
                    .ToList().FirstOrDefault();
                if (lockObj != null)
                {
                    result = SFCDB.ORM.Updateable<R_SN_LOCK>().UpdateColumns(r => new R_SN_LOCK { LOCK_STATUS = "0", UNLOCK_EMP = LoginUser.EMP_NO, UNLOCK_REASON = status, UNLOCK_TIME = systemTime })
                        .Where(r => r.ID == lockObj.ID).ExecuteCommand();
                    if (result == 0)
                    {
                        throw new Exception("Update Data Error!");
                    }
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add("OK");
                StationReturn.Data = "OK";
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void Delete(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(SFCDB, DBTYPE);
                if (!TCUP.CheckpPivilegeByName(SFCDB, "DELETE_OUTLINE_TEST", this.LoginUser.EMP_NO))
                {
                    throw new Exception("No Pivilege!");
                }
                T_R_OUTLINE_TEST t_r_outline_test = new T_R_OUTLINE_TEST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                DateTime systemTime = t_r_outline_test.GetDBDateTime(SFCDB);
                R_OUTLINE_TEST test_obj = SFCDB.ORM.Queryable<R_OUTLINE_TEST>().Where(r => r.ID == id).ToList().FirstOrDefault();
                if (test_obj == null)
                {
                    throw new Exception($@"ID:{id} Not Exist!");
                }
                test_obj.VALID_FLAG = 0;
                test_obj.LASTEDIT_BY = LoginUser.EMP_NO;
                test_obj.LASTEDIT_DATE = systemTime;
                int result = t_r_outline_test.Update(SFCDB, test_obj);
                if (result == 0)
                {
                    throw new Exception("Update Data Error!");
                }


                //namespace     MESStation.Stations.StationActions.ActionRunners
                //class         SampleAction
                //functionname  Taken2DXSample
                //2DXSample Insert Into R_SN_LOCK
                R_SN_LOCK lockObj = SFCDB.ORM.Queryable<R_SN_LOCK>()
                   .Where(r => r.SN == test_obj.SN && r.WORKORDERNO == test_obj.WORKORDERNO && r.LOCK_STATION == test_obj.STATION && r.LOCK_REASON == test_obj.DATA_TYPE && r.LOCK_EMP == "SYSTEM" && r.TYPE == "SN" && r.LOCK_STATUS == "1")
                   .ToList().FirstOrDefault();
                if (lockObj != null)
                {
                    result = SFCDB.ORM.Updateable<R_SN_LOCK>().UpdateColumns(r => new R_SN_LOCK { LOCK_STATUS = "0", UNLOCK_EMP = LoginUser.EMP_NO, UNLOCK_REASON = "DELETE", UNLOCK_TIME = systemTime })
                        .Where(r => r.ID == lockObj.ID).ExecuteCommand();
                    if (result == 0)
                    {
                        throw new Exception("Update Data Error!");
                    }
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add("OK");
                StationReturn.Data = "OK";
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void Download(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string status_sql = "";
                if (Data["STATUS"] != null)
                {
                    status_sql = Data["STATUS"].ToString().ToUpper() == "ALL" ? "" : $@" and status='{Data["STATUS"].ToString()}'";
                }
                string type_sql = Data["TYPE"] == null ? "" : $@" and data_type='{Data["TYPE"].ToString()}'";
                string search_sql = (Data["SEARCH_TEXT"] == null || Data["SEARCH_TEXT"].ToString() == "") ? "" : $@" and ( sn like '%{ Data["SEARCH_TEXT"].ToString()}%' or workorderno like '%{ Data["SEARCH_TEXT"].ToString()}%'  or skuno like '%{ Data["SEARCH_TEXT"].ToString()}%')";

                string total_sql = $@"select DATA_TYPE,SN,WORKORDERNO,SKUNO,STATION,LINE,decode(STATUS,'0','WAIT_TEST','1','PASS','2','FAIL','ERROR') as STATUS,
                                    FAIL_DESC,CREAT_DATE,CREAT_EMP,LASTEDIT_DATE,LASTEDIT_BY from R_OUTLINE_TEST where valid_flag='1' {status_sql} {type_sql} {search_sql} order by CREAT_DATE desc";
                DataTable total_dt = SFCDB.ExecSelect(total_sql, null).Tables[0];
                if (total_dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");
                }
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(total_dt);
                string fileName = "TestRecord_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                StationReturn.Data = new { FileName = fileName, FileContent = content };
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
    }
}
