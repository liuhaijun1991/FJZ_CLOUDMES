using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Common;
using MESDBHelper;
using MESStation.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MESStation.Management
{
    public class ClassTimeConfig : MesAPIBase
    {
        protected APIInfo FDeployNewClassTime = new APIInfo()
        {
            FunctionName = "DeployNewClassTime",
            Description = "配置新的班別信息",
            Parameters = new List<APIInputInfo>()
            {               
                new APIInputInfo() {InputName = "WORK_SECTION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "START_TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "" },              
                new APIInputInfo() {InputName = "DAY_DISTINCT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FClassTimeQuery = new APIInfo()
        {
            FunctionName = "QueryClassTime",
            Description = "按時間點或班別類型查詢班別信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WORK_SECTION", InputType = "string", DefaultValue = "" },                
                new APIInputInfo() {InputName = "DAY_DISTINCT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpdateClassTime = new APIInfo()
        {
            FunctionName = "UpdateClassTime",
            Description = "更新班別信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORK_SECTION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "START_TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DAY_DISTINCT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public ClassTimeConfig()
        {
            this.Apis.Add(FDeployNewClassTime.FunctionName, FDeployNewClassTime);
            this.Apis.Add(FClassTimeQuery.FunctionName, FClassTimeQuery);
            this.Apis.Add(FUpdateClassTime.FunctionName, FUpdateClassTime);
        }

        /// <summary>
        /// Deploy New Class Time Row
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeployNewClassTime(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESException exception = new MESException("");
            string[] msg = new string[1];
            if (string.IsNullOrEmpty(Data["WORK_SECTION"].ToString()))
            { 
                //msg[0] = "Please Input Work Section";
                //throw exception.GetMESException("", "", msg);
                throw new Exception("Please Input Work Section");                
            }
            if (string.IsNullOrEmpty(Data["START_TIME"].ToString()))
            {
                throw new Exception("Please Input Start Time");
            }
            if (string.IsNullOrEmpty(Data["END_TIME"].ToString()))
            {
                throw new Exception("Please Input End Time");
            }
            if (string.IsNullOrEmpty(Data["DAY_DISTINCT"].ToString()))
            {
                throw new Exception("Please Input Day Distinct");
            }
            string strRet = "";   
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            T_C_CLASS_TIME classTime = new T_C_CLASS_TIME(sfcdb, DB_TYPE_ENUM.Oracle);
            Row_C_CLASS_TIME r = (Row_C_CLASS_TIME)classTime.NewRow();
            //r.ID = classTime.GetNewID(this.BU,sfcdb,DB_TYPE_ENUM.Oracle);
            r.ID = classTime.GetNewID("HWD", sfcdb, DB_TYPE_ENUM.Oracle);
            r.SEQ_NO = classTime.GetTotalRows(sfcdb) + 1;
            r.WORK_SECTION = Data["WORK_SECTION"].ToString().Trim();
            r.START_TIME = Data["START_TIME"].ToString().Trim();
            r.END_TIME = Data["END_TIME"].ToString().Trim();
            if (Data["DAY_DISTINCT"].ToString().Trim() == "Shift1")
            {
                r.WORK_CLASS = ((int)WORK_CLASS.Shift1).ToString();
                r.DAY_DISTINCT = WORK_CLASS.Shift1.ToString();
            }
            else
            {
                r.WORK_CLASS = ((int)WORK_CLASS.Shift2).ToString();
                r.DAY_DISTINCT = WORK_CLASS.Shift2.ToString();
            }
            r.EDIT_EMP = this.LoginUser;
            r.EDIT_TIME = DateTime.Now;
            strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
            try
            {
                int rows = int.Parse(strRet);
                sfcdb.CommitTrain();
                StationReturn.Status = "PASS";
                StationReturn.Message = "OK,Deploy New Class Time OK!";
                StationReturn.data = rows;
            }
            catch
            {
                sfcdb.RollbackTrain();
                StationReturn.Status = "FAIL";
                StationReturn.Message = strRet;
                StationReturn.data = "";
            }
            this.DBPools["SFCDB"].Return(sfcdb);
        }
        /// <summary>
        /// Query Class Time
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void QueryClassTime(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {             
            DataTable dt = new DataTable();
            Dictionary<string, string> paras;
            if (string.IsNullOrEmpty(Data["WORK_SECTION"].ToString()) && string.IsNullOrEmpty(Data["DAY_DISTINCT"].ToString()))
            {
                //throw new Exception("Please Input Work Section Or Day Distinct");
                paras = null;
            }
            else
            {
                paras = new Dictionary<string, string>
                  {
                     {"WORK_SECTION", Data["WORK_SECTION"].ToString().Trim()},
                     { "DAY_DISTINCT", Data["DAY_DISTINCT"].ToString().Trim()}
                  };
            }          
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            T_C_CLASS_TIME classTime = new T_C_CLASS_TIME(sfcdb, DB_TYPE_ENUM.Oracle);  
            dt = classTime.GetShiftInfo(paras, sfcdb);
            if (dt != null && dt.Rows.Count > 0)
            {
                StationReturn.Status = "PASS";
                StationReturn.Message = "OK";
                ConvertToJson ctj = new ConvertToJson();
                StationReturn.data = ctj.DataTableToJson(dt);
            }
            else
            {
                StationReturn.Status = "Fail";
                StationReturn.Message = "No Date!";
                StationReturn.data = "";
            }
            this.DBPools["SFCDB"].Return(sfcdb);
        }
        /// <summary>
        /// Update Class Time
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateClassTime(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {            
            string strRet = "";           
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            T_C_CLASS_TIME classTime = new T_C_CLASS_TIME(sfcdb, DB_TYPE_ENUM.Oracle);
            Row_C_CLASS_TIME row = (Row_C_CLASS_TIME)classTime.NewRow();
            row = (Row_C_CLASS_TIME)classTime.GetObjByID(Data["ID"].ToString().Trim(), sfcdb, DB_TYPE_ENUM.Oracle);
            row.WORK_SECTION = Data["WORK_SECTION"].ToString().Trim();           
            row.START_TIME = Data["START_TIME"].ToString().Trim();
            row.END_TIME = Data["END_TIME"].ToString().Trim();
            if (Data["DAY_DISTINCT"].ToString().Trim() == ("Shift1"))
            {
                row.WORK_CLASS = ((int)WORK_CLASS.Shift1).ToString();
                row.DAY_DISTINCT = WORK_CLASS.Shift1.ToString();
            }
            else
            {
                row.WORK_CLASS = ((int)WORK_CLASS.Shift2).ToString();
                row.DAY_DISTINCT = WORK_CLASS.Shift2.ToString();
            }
            row.EDIT_EMP = this.LoginUser;
            row.EDIT_TIME = DateTime.Now;
            strRet = sfcdb.ExecSQL(row.GetUpdateString(DB_TYPE_ENUM.Oracle));
            try
            {
                int rows = int.Parse(strRet);
                sfcdb.CommitTrain();
                StationReturn.Status = "PASS";
                StationReturn.Message = "OK,Update Class Time OK!";
                StationReturn.data = rows;
            }
            catch
            {
                sfcdb.RollbackTrain();
                StationReturn.Status = "FAIL";
                StationReturn.Message = strRet;
                StationReturn.data = "";
            }
            this.DBPools["SFCDB"].Return(sfcdb);
        }
    }
}
