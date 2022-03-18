using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class WorkClassConfig : MesAPIBase
    {
        protected APIInfo FAddWorkClass = new APIInfo()
        {
            FunctionName = "AddWorkClass",
            Description = "配置新的班別信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CLASS_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "START_TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FQueryWorkClass = new APIInfo()
        {
            FunctionName = "QueryWorkClass",
            Description = "查詢班別信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CLASS_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpdateWorkClass = new APIInfo()
        {
            FunctionName = "UpdateWorkClass",
            Description = "更新班別信息",
            Parameters = new List<APIInputInfo>()
            {     
                new APIInputInfo() {InputName = "CLASS_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NEW_CLASS_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "START_TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteWorkClass = new APIInfo()
        {
            FunctionName = "DeleteWorkClass",
            Description = "刪除班別信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CLASS_ID", InputType = "string", DefaultValue = "" }               
            },
            Permissions = new List<MESPermission>() { }
        };
        
        public WorkClassConfig()
        {
            this.Apis.Add(FAddWorkClass.FunctionName, FAddWorkClass);
            this.Apis.Add(FQueryWorkClass.FunctionName, FQueryWorkClass);
            this.Apis.Add(FUpdateWorkClass.FunctionName, FUpdateWorkClass);
            this.Apis.Add(FDeleteWorkClass.FunctionName, FDeleteWorkClass);           
        }
        public void AddWorkClass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string className = Data["CLASS_NAME"].ToString().Trim();
            string startTime = Data["START_TIME"].ToString().Trim();
            string endTime = Data["END_TIME"].ToString().Trim();
            bool isExist = false;
            OleExec oleDB = null;
            T_C_WORK_CLASS workClass = null;
            if (string.IsNullOrEmpty(className))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("CLASS NAME");
                StationReturn.Data = "";
                return;
            }
            if (string.IsNullOrEmpty(startTime))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("START TIME");
                StationReturn.Data = "";
                return;
            }
            if (string.IsNullOrEmpty(endTime))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("END TIME");
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                workClass = new T_C_WORK_CLASS(oleDB, DBTYPE);
                if (workClass.WorkClassIsExistByName(className, oleDB))
                { 
                    StationReturn.MessagePara.Add(className);
                    isExist = true;
                }
                if (workClass.TimeIsExist(startTime, oleDB))
                {
                    StationReturn.MessagePara.Add(startTime);
                    isExist = true;
                }
                if (workClass.TimeIsExist(endTime, oleDB))
                {
                    StationReturn.MessagePara.Add(endTime);
                    isExist = true;
                }
                if(isExist)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_WORK_CLASS workClassRow = (Row_C_WORK_CLASS)workClass.NewRow();
                    workClassRow.ID = workClass.GetNewID(this.BU, oleDB, DBTYPE);
                    workClassRow.NAME = className;
                    workClassRow.START_TIME = startTime;
                    workClassRow.END_TIME = endTime;
                    oleDB.ThrowSqlExeception = true;                   
                    oleDB.ExecSQL(workClassRow.GetInsertString(DBTYPE));                    
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception exctption)
            {               
                this.DBPools["SFCDB"].Return(oleDB);
                throw exctption;
            }
        }

        public void QueryWorkClass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string className = Data["CLASS_NAME"].ToString().Trim();
            OleExec sfcdb = null;
            T_C_WORK_CLASS workClass = null;
            List<C_WORK_CLASS> workClassList = new List<C_WORK_CLASS>();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                workClass = new T_C_WORK_CLASS(sfcdb, DBTYPE);
                workClassList = workClass.GetWorkClassList(sfcdb, className);
                if (workClassList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = workClassList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw exception;
            } 
            
        }

        public void UpdateWorkClass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string strID = Data["CLASS_ID"].ToString().Trim();
            string newClassName = Data["NEW_CLASS_NAME"].ToString().Trim();
            string startTime = Data["START_TIME"].ToString().Trim();
            string endTime = Data["END_TIME"].ToString().Trim();           
            OleExec oleDB = null;
            T_C_WORK_CLASS workClass = null;
            Row_C_WORK_CLASS workClassRow = null;
            if (string.IsNullOrEmpty(strID))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("CLASS ID");
                StationReturn.Data = "";
                return;
            }
            if (string.IsNullOrEmpty(startTime))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("START TIME");
                StationReturn.Data = "";
                return;
            }
            if (string.IsNullOrEmpty(endTime))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("END TIME");
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                workClass = new T_C_WORK_CLASS(oleDB, DBTYPE);
                workClassRow = (Row_C_WORK_CLASS)workClass.NewRow();
                if (!workClass.WorkClassIsExistByID(strID, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";                    
                    StationReturn.Data = "";
                }
                else if (!string.IsNullOrEmpty(newClassName) && workClass.WorkClassIsExistByName(newClassName, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(newClassName);
                    StationReturn.Data = "";
                }
                else
                {                    
                    workClassRow = (Row_C_WORK_CLASS)workClass.GetObjByID(strID, oleDB, DBTYPE);
                    if (!string.IsNullOrEmpty(newClassName))
                    {
                        workClassRow.NAME = newClassName;
                    }
                    if (!string.IsNullOrEmpty(startTime))
                    {
                        workClassRow.START_TIME = startTime;
                    }
                    if (!string.IsNullOrEmpty(endTime))
                    {
                        workClassRow.END_TIME = endTime;
                    }
                    oleDB.ThrowSqlExeception = true;                   
                    oleDB.ExecSQL(workClassRow.GetUpdateString(DBTYPE));                   
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }

        }            

        public void DeleteWorkClass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //string id = Data["CLASS_ID"].ToString().Trim();
            Newtonsoft.Json.Linq.JArray idArray = (Newtonsoft.Json.Linq.JArray)Data["CLASS_ID"];
            OleExec oleDB = null;
            T_C_WORK_CLASS workClass = null;
            Row_C_WORK_CLASS workClassRow;
            if (idArray.Count == 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("CLASS ID");
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                oleDB.ThrowSqlExeception = true;
                oleDB.BeginTrain();
                workClass = new T_C_WORK_CLASS(oleDB, DBTYPE);               
                for (int i = 0; i < idArray.Count; i++)
                {
                    workClassRow = (Row_C_WORK_CLASS)workClass.GetObjByID(idArray[i].ToString(), oleDB, DBTYPE);

                    oleDB.ExecSQL(workClassRow.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "MES00000004";
                StationReturn.Data = "";
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception exception)
            {
                oleDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }
                  
        }
   
    }
}
