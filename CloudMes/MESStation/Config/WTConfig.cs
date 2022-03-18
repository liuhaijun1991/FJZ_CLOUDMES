using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MESDBHelper;
using MESDataObject.Module;
using Newtonsoft.Json;
using MESDataObject;
using MESPubLab.MESStation;

namespace MESStation.Config
{
    public class WTConfig : MesAPIBase
    {

        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        #region ApiInfos
        private APIInfo AllWTList = new APIInfo()
        {
            FunctionName = "GetAllWTList",
            Description = "獲取所有機種稱重記錄",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteWTByID = new APIInfo()
        {
            FunctionName = "DeleteWTByID",
            Description = "依ID刪除稱重參數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="IDList",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _ApproveWTByID = new APIInfo()
        {
            FunctionName = "ApproveWTByID",
            Description = "根據ID上線稱重工站",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="IDList",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };


        private APIInfo _UpdateWT = new APIInfo()
        {
            FunctionName = "UpdateWT",
            Description = "修改稱重參數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SKU_EditRow",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _AddWT = new APIInfo()
        {
            FunctionName = "AddWT",
            Description = "添加稱重參數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SKU_EditRow",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _Get_WTPrivilege = new APIInfo()
        {
            FunctionName = "Get_WTPrivilege",
            Description = "獲取稱重修改刪除Approve權限",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };
        #endregion

        public WTConfig()
        {
            this.Apis.Add(AllWTList.FunctionName, AllWTList);
            this.Apis.Add(_DeleteWTByID.FunctionName, _DeleteWTByID);
            this.Apis.Add(_ApproveWTByID.FunctionName, _ApproveWTByID);
            this.Apis.Add(_AddWT.FunctionName, _AddWT);
            this.Apis.Add(_UpdateWT.FunctionName, _UpdateWT);
            this.Apis.Add(_Get_WTPrivilege.FunctionName, _Get_WTPrivilege);
        }

        public void WriteIntoMESLog(OleExec SFCDB, string bu, string programName, string className, string functionName, string logMessage, string logSql, string editEmp)
        {
            //OleExec SFCDB = new OleExec(db, false);
            T_R_MES_LOG mesLog = new T_R_MES_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
            string id = mesLog.GetNewID(bu, SFCDB);
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
            rowMESLog.ID = id;
            rowMESLog.PROGRAM_NAME = programName;
            rowMESLog.CLASS_NAME = className;
            rowMESLog.FUNCTION_NAME = functionName;
            rowMESLog.LOG_MESSAGE = logMessage;
            rowMESLog.LOG_SQL = logSql;
            rowMESLog.EDIT_EMP = editEmp;
            rowMESLog.EDIT_TIME = GetDBDateTime(); 
            SFCDB.ThrowSqlExeception = true;
            SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
        }
        /// <summary>
        /// 返回所有料號的稱重參數
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAllWTList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<TVC_WT_CONFIG> SkuWTList = new List<TVC_WT_CONFIG>();
            T_C_WT_CONFIG TCWT = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCWT = new T_C_WT_CONFIG(sfcdb, DBTYPE);
                SkuWTList = TCWT.GetAllWTList(sfcdb);
                if (SkuWTList.Count() == 0)
                {
                    //沒有獲取到任何數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到{0}行數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuWTList.Count().ToString());
                    StationReturn.Data = SkuWTList;
                }
            }
            catch (Exception e)
            {
                //執行出現異常
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        
        public void Get_WTPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<string> List = new List<string>();
            T_C_WT_CONFIG TCWT = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();                
                if (LoginUser.EMP_LEVEL == "9")
                {
                    List.Add("WTConfigModify");
                    List.Add("WTConfigApprove");
                }
                else
                {
                    TCWT = new T_C_WT_CONFIG(sfcdb, this.DBTYPE);
                    List = TCWT.Get_WTPrivilegeList(sfcdb,LoginUser.EMP_NO);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "成功獲取權限列表!";
                StationReturn.Data = List;
                
            }
            catch(Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = e.Message;
                StationReturn.Message = "獲取權限失敗!";
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }        /// <summary>
        /// 依ID清單刪除稱重參數記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeleteWTByID(Newtonsoft.Json.Linq.JObject requestValue,Newtonsoft.Json.Linq.JObject Data,MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_WT_CONFIG TCWT = null;
            Row_C_WT_CONFIG RCWT = null;
            string sql = "";
            string strReturn = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                TCWT = new T_C_WT_CONFIG(sfcdb, this.DBTYPE);
                RCWT=(Row_C_WT_CONFIG)TCWT.NewRow();
                foreach(string item in Data["IDList"])
                {
                    string strID = item.Trim('\'').Trim('\"');
                    RCWT = (Row_C_WT_CONFIG)TCWT.GetObjByID(strID, sfcdb);
                    sql += RCWT.GetDeleteString(this.DBTYPE)+";\n";
                }
                strReturn=sfcdb.ExecSQL("Begin\n" + sql + "End;");
                WriteIntoMESLog(sfcdb, BU, "WTConfig", "MESStation.Config.WTConfig", "DeleteWTByID", "刪除稱重記錄", sql.Substring(0, sql.Length - 1 > 900 ? 900 : sql.Length - 1), LoginUser.EMP_NO);

                //如果數據更新成功，則返回數據更新的記錄數,如果為Begn...END返回為-1,可被 Int32.Parse 方法轉換成 int
                if (Int32.Parse(strReturn) > -2)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "刪除成功!";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Delete Fail!";
                    StationReturn.Data = strReturn;
                }
                sfcdb.CommitTrain();
            }catch(Exception e)
            {
                sfcdb.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "刪除記錄失敗或該記錄已經不存在";
                StationReturn.Data = e.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            
            
        }

        /// <summary>
        /// 依據ID清單Approve稱重工站
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void ApproveWTByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_WT_CONFIG TCWT = null;
            Row_C_WT_CONFIG RCWT = null;
            string sql = "";
            string strReturn = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                TCWT = new T_C_WT_CONFIG(sfcdb, this.DBTYPE);
                RCWT = (Row_C_WT_CONFIG)TCWT.NewRow();
                foreach(string item in Data["IDList"])
                {
                    string strID = item.Trim('\'').Trim('\"');
                    RCWT = (Row_C_WT_CONFIG)TCWT.GetObjByID(strID,sfcdb);
                    RCWT.APPROVE_EMP = (string)Data["LoginUserEmp"];
                    RCWT.APPROVE_TIME = GetDBDateTime();
                    sql += RCWT.GetUpdateString(this.DBTYPE)+";\n";
                }
                strReturn=sfcdb.ExecSQL("Begin\n" + sql + "End;");
                WriteIntoMESLog(sfcdb, BU, "WTConfig", "MESStation.Config.WTConfig", "ApproveWT", "簽核稱重記錄", sql.Substring(0, sql.Length - 1 > 900 ? 900 : sql.Length - 1), LoginUser.EMP_NO);
                //如果數據更新成功，則返回數據更新的記錄數,如果為Begn...END返回為-1,可被 Int32.Parse 方法轉換成 int
                if (Int32.Parse(strReturn) >-2)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "Approve成功!";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Approve 數據更新失敗!";
                    StationReturn.Data = strReturn;
                }
                sfcdb.CommitTrain();
            }catch(Exception e)
            {
                sfcdb.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "Approve失敗或該記錄已經不存在!";
                StationReturn.Data = e.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 依據ID 更新WT參數
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateWT(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string SKU_EditRow = string.Empty;
            string sql = "";
            string strReturn = "";
            OleExec sfcdb = null;
            C_WT_CONFIG CWT = null;
            T_C_WT_CONFIG TCWT = null;
            Row_C_WT_CONFIG RCWT= null;
            try {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCWT = new T_C_WT_CONFIG(sfcdb, this.DBTYPE);
                SKU_EditRow = Data["SKU_EditRow"].ToString();
                CWT = (C_WT_CONFIG)JsonConvert.Deserialize(SKU_EditRow, typeof(C_WT_CONFIG));
                RCWT = (Row_C_WT_CONFIG)TCWT.GetObjByID(CWT.ID, sfcdb);
                RCWT.BMINVALUE = CWT.BMINVALUE;
                RCWT.BMAXVALUE = CWT.BMAXVALUE;
                RCWT.CMINVALUE = CWT.CMINVALUE;
                RCWT.CMAXVALUE = CWT.CMAXVALUE;
                RCWT.ONLINE_FLAG = CWT.ONLINE_FLAG;
                RCWT.EDIT_EMP = LoginUser.EMP_NO;
                RCWT.EDIT_TIME = GetDBDateTime();
                RCWT.APPROVE_EMP = "";
                RCWT.APPROVE_TIME = null;
                sql = RCWT.GetUpdateString(this.DBTYPE);
                strReturn=sfcdb.ExecSQL(sql);
                WriteIntoMESLog(sfcdb, BU, "WTConfig", "MESStation.Config.WTConfig", "UpdateWT", "更新稱重記錄", sql.Substring(0, sql.Length - 1 > 900 ? 900 : sql.Length - 1), LoginUser.EMP_NO);
                //如果數據更新成功，則返回數據更新的記錄數,如果為Begn...END返回為-1,可被 Int32.Parse 方法轉換成 int
                if (Int32.Parse(strReturn) >-2)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "Update successful!";
                    StationReturn.Data = CWT.ID;
                }else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Update DB Fail!";
                    StationReturn.Data = strReturn;
                }
            }catch(Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "Update Fail!";
                StationReturn.Data = e.Message;
            }
            finally
            {
                if(sfcdb!=null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 新增稱重參數記錄
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddWT(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_WT_CONFIG TCWT = null;
            Row_C_WT_CONFIG RCWT = null;
            C_WT_CONFIG CWT = null;
            string sql = "";
            string SKU_EditRow = string.Empty;
            string strReturn = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SKU_EditRow = Data["SKU_EditRow"].ToString();
                CWT = (C_WT_CONFIG)JsonConvert.Deserialize(SKU_EditRow, typeof(C_WT_CONFIG));
                TCWT = new T_C_WT_CONFIG(sfcdb, this.DBTYPE);
                RCWT = (Row_C_WT_CONFIG)TCWT.NewRow();
                RCWT.ID = TCWT.GetNewID(BU, sfcdb);
                RCWT.SKUNO = CWT.SKUNO;
                RCWT.BMINVALUE = CWT.BMINVALUE;
                RCWT.BMAXVALUE = CWT.BMAXVALUE;
                RCWT.CMINVALUE = CWT.CMINVALUE;
                RCWT.CMAXVALUE = CWT.CMAXVALUE;
                RCWT.ONLINE_FLAG = CWT.ONLINE_FLAG;
                RCWT.EDIT_EMP = LoginUser.EMP_NO;
                RCWT.EDIT_TIME = GetDBDateTime();
                sql=RCWT.GetInsertString(this.DBTYPE);
                strReturn=sfcdb.ExecSQL(sql);
                WriteIntoMESLog(sfcdb, BU, "WTConfig", "MESStation.Config.WTConfig", "AddWT", "新增稱重記錄", sql.Substring(0, sql.Length - 1 > 900 ? 900 : sql.Length - 1), LoginUser.EMP_NO);
                //如果數據更新成功，則返回數據更新的記錄數,如果為Begn...END返回為-1,可被 Int32.Parse 方法轉換成 int
                if (Int32.Parse(strReturn)>-2)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "Add Successful!";
                    StationReturn.Data = RCWT.ID;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "數據更新失敗!";
                    StationReturn.Data =strReturn;
                }
            }
            catch(Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "Add Fail!";
                StationReturn.Data = e.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        
    }
}
