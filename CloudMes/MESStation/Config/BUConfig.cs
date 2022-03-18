using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;


namespace MESStation.Config
{
    public class BUConfig : MesAPIBase
    {
        protected APIInfo FAddNewBU = new APIInfo()
        {
            FunctionName = "AddNewBU",
            Description = "Add a new BU",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" }                
            },
            Permissions = new List<MESPermission>() { }
        };
      
        protected APIInfo FDeleteBU= new APIInfo()
        {
            FunctionName = "DeleteBU",
            Description = "Delete a BU",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU_ID", InputType = "string", DefaultValue = "" }               
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateBU = new APIInfo()
        {
            FunctionName = "UpdateBU",
            Description = "Update BU",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NEW_BU", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FQueryBU = new APIInfo()
        {
            FunctionName = "QueryBU",
            Description = "Query BU",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public BUConfig()
        {            
            this.Apis.Add(FAddNewBU.FunctionName, FAddNewBU);
            this.Apis.Add(FDeleteBU.FunctionName, FDeleteBU);
            this.Apis.Add(FUpdateBU.FunctionName, FUpdateBU);
            this.Apis.Add(FQueryBU.FunctionName, FQueryBU);
        }

        public void AddNewBU(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string strbu = Data["BU"].ToString().Trim();
            OleExec oleDB = null;
            T_C_BU bu = null;
            Row_C_BU buRow = null;            
            try
            {               
                oleDB = this.DBPools["SFCDB"].Borrow();
                bu = new T_C_BU(oleDB, DBTYPE);
                if (bu.BUIsExist(oleDB, strbu))
                {                    
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    buRow = (Row_C_BU)bu.NewRow();
                    buRow.ID = bu.GetNewID(SystemName, oleDB, DBTYPE);
                    buRow.BU = strbu;
                    oleDB.ThrowSqlExeception = true;                   
                    oleDB.ExecSQL(buRow.GetInsertString(DBTYPE));                   
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch(Exception exception)
            {                
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }
            
        }
     
        public void DeleteBU(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["BU_ID"].ToString().Trim();
            OleExec oleDB = null;
            T_C_BU bu = null;
            Row_C_BU rowBU = null;
            if (string.IsNullOrEmpty(id))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("BU ID");
                StationReturn.Data = "";
                return;
            }
            try
            {                
                oleDB = this.DBPools["SFCDB"].Borrow();
                bu = new T_C_BU(oleDB, DBTYPE);
                oleDB.ThrowSqlExeception = true;
                rowBU = (Row_C_BU)bu.GetObjByID(id, oleDB, DBTYPE);
                oleDB.ExecSQL(rowBU.GetDeleteString(DBTYPE));
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }
            
        }

        public void UpdateBU(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["BU_ID"].ToString().Trim(); ;
            string strBU= Data["NEW_BU"].ToString().Trim();
            OleExec oleDB = null;
            T_C_BU bu = null;
            Row_C_BU rowBU = null;
            if (string.IsNullOrEmpty(id))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("BU ID");
                StationReturn.Data = "";
                return;
            }
            if (string.IsNullOrEmpty(strBU))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("NEW BU");
                StationReturn.Data = "";
                return;
            }
            try
            {                
                oleDB = this.DBPools["SFCDB"].Borrow();
                bu = new T_C_BU(oleDB, DBTYPE);
                oleDB.ThrowSqlExeception = true;
                if (!bu.BUIsExistByID(oleDB, id))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.Data = "";
                }
                else
                {
                    rowBU = (Row_C_BU)bu.GetObjByID(id,oleDB);
                    rowBU.BU = strBU;
                    oleDB.ExecSQL(rowBU.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
                }
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }
        }

        public void QueryBU(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string strBU = Data["BU"].ToString().Trim();
            OleExec oleDB = null;
            T_C_BU bu = null;
            List<C_BU> buList = new List<C_BU>();
            try
            {           
                oleDB = this.DBPools["SFCDB"].Borrow();
                bu = new T_C_BU(oleDB, DBTYPE);
                buList = bu.GetBUList(oleDB, strBU);
                if (buList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(buList.Count);
                    StationReturn.Data = buList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
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
    }
}
