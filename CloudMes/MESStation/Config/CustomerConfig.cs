using MESDataObject.Module;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using System.Data;

namespace MESStation.Config
{
    public class CustomerConfig : MesAPIBase
    {
        protected APIInfo FAddNewCustomer = new APIInfo()
        {
            FunctionName = "AddNewCustomer",
            Description = "配置新的物料信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CUSTOMER_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DESCRIPTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FQueryCustomer = new APIInfo()
        {
            FunctionName = "QueryCustomer",
            Description = "查詢物料信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CUSTOMER_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpdateCustomer = new APIInfo()
        {
            FunctionName = "UpdateCustomer",
            Description = "更新物料信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CUSTOMER_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NEW_CUSTOMER_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DESCRIPTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeteleCustomer = new APIInfo()
        {
            FunctionName = "DeteleCustomer",
            Description = "刪除物料信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CUSTOMER_ID", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };


        public CustomerConfig()
        {
            this.Apis.Add(FAddNewCustomer.FunctionName, FAddNewCustomer);
            this.Apis.Add(FQueryCustomer.FunctionName, FQueryCustomer);
            this.Apis.Add(FUpdateCustomer.FunctionName, FUpdateCustomer);
            this.Apis.Add(FDeteleCustomer.FunctionName, FDeteleCustomer);
        }

        public void AddNewCustomer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string strbu = BU;
            string customer_name = Data["CUSTOMER_NAME"].ToString().Trim();
            string description = Data["DESCRIPTION"].ToString().Trim();           
            OleExec oleDB = null;
            T_C_CUSTOMER customer = null;
            if (string.IsNullOrEmpty(customer_name))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("CUSTOMER NAME");
                StationReturn.Data = "";
                return;
            }
            if (string.IsNullOrEmpty(description))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("DESCRIPTION");
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                customer = new T_C_CUSTOMER(oleDB, DBTYPE);
                if (customer.CustomerIsExist(oleDB, strbu, customer_name))
                {   
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_CUSTOMER customerRow = (Row_C_CUSTOMER)customer.NewRow();
                    customerRow.ID = customer.GetNewID(this.BU, oleDB, DBTYPE);
                    customerRow.BU = strbu;
                    customerRow.CUSTOMER_NAME = customer_name;
                    customerRow.DESCRIPTION = description;
                    oleDB.ThrowSqlExeception = true;                                       
                    oleDB.ExecSQL(customerRow.GetInsertString(DBTYPE));                   
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

        public void QueryCustomer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string strbu = BU;
            string customer_name = Data["CUSTOMER_NAME"].ToString().Trim();            
            OleExec sfcdb = null;
            T_C_CUSTOMER customer = null;
            Dictionary<string, string> paras =new Dictionary<string, string>();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                paras.Add("BU",strbu);
                if (!string.IsNullOrEmpty(customer_name))
                {
                    paras.Add("CUSTOMER_NAME", customer_name);
                }
                customer = new T_C_CUSTOMER(sfcdb, DBTYPE);
                List<C_CUSTOMER> costomerList = customer.GetCustomerList(paras, sfcdb);
                if (costomerList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = costomerList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch(Exception exception)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw exception;
            }           
        }

        public void UpdateCustomer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string strbu = BU;            
            string id = Data["CUSTOMER_ID"].ToString().Trim();
            string new_customer_name = Data["NEW_CUSTOMER_NAME"].ToString().Trim();
            string description = Data["DESCRIPTION"].ToString().Trim();
            OleExec oleDB = null;
            T_C_CUSTOMER customer = null;
            Row_C_CUSTOMER customerRow = null;
            if (string.IsNullOrEmpty(id))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("CUSTOMER ID");
                StationReturn.Data = "";
                return;
            }
            if (string.IsNullOrEmpty(description))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("DESCRIPTION");
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();               
                customer = new T_C_CUSTOMER(oleDB, DBTYPE);
                customerRow = (Row_C_CUSTOMER)customer.NewRow();
                if (!customer.CustomerIsExist(oleDB, id))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";                    
                    StationReturn.Data = "";
                }
                else if (!string.IsNullOrEmpty(new_customer_name) && customer.CustomerIsExist(oleDB, strbu, new_customer_name))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(new_customer_name);
                    StationReturn.Data = "";
                }
                else
                {                    
                    customerRow = (Row_C_CUSTOMER)customer.GetObjByID(id, oleDB, DBTYPE);
                    if (!string.IsNullOrEmpty(new_customer_name))
                    {
                        customerRow.CUSTOMER_NAME = new_customer_name;
                    }
                    if (!string.IsNullOrEmpty(description))
                    {
                        customerRow.DESCRIPTION = description;
                    }
                    oleDB.ThrowSqlExeception = true;                    
                    oleDB.ExecSQL(customerRow.GetUpdateString(DBTYPE));                    
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
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

        public void DeteleCustomer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string strbu = BU;
            Newtonsoft.Json.Linq.JArray idArray = (Newtonsoft.Json.Linq.JArray)Data["CUSTOMER_ID"];            
            OleExec oleDB = null;
            T_C_CUSTOMER customer = null;
            Row_C_CUSTOMER customerRow = null;
            if (idArray.Count == 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("CUSTOMER ID");
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                customer = new T_C_CUSTOMER(oleDB, DBTYPE);
                oleDB.ThrowSqlExeception = true;
                oleDB.BeginTrain();
                for (int i = 0; i < idArray.Count; i++)
                {
                    customerRow = (Row_C_CUSTOMER)customer.GetObjByID(idArray[i].ToString(), oleDB, DBTYPE);
                    oleDB.ExecSQL(customerRow.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
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
