using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;

namespace MESStation.Config
{
    public class SkuVerMappingSetting : MesAPIBase
    {
        protected APIInfo FGetMappingList = new APIInfo()
        {
            FunctionName = "GetMappingList",
            Description = "Get sku version mapping list by skuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },                
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewMapping = new APIInfo()
        {
            FunctionName = "AddNewMapping",
            Description = "add a new version mapping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FOX_SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FOX_VER1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FOX_VER2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_VER", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FModifyMapping = new APIInfo()
        {
            FunctionName = "ModifyMapping",
            Description = "Modify sku version mapping by skuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CUST_SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FOX_VER1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FOX_VER2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_VER", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteMapping = new APIInfo()
        {
            FunctionName = "DeleteMapping",
            Description = "delete sku version mapping list by id",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        public SkuVerMappingSetting() {
            this.Apis.Add(FGetMappingList.FunctionName, FGetMappingList);
            this.Apis.Add(FAddNewMapping.FunctionName, FAddNewMapping);
            this.Apis.Add(FModifyMapping.FunctionName, FModifyMapping);
            this.Apis.Add(FDeleteMapping.FunctionName, FDeleteMapping);
        }

        public void GetMappingList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_VER_MAPPING skuVerMapping = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                skuVerMapping = new T_C_SKU_VER_MAPPING(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SKU_VER_MAPPING> list = skuVerMapping.GetMappingListBySku((Data["SKUNO"].ToString()).Trim(), sfcdb);

                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void AddNewMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_VER_MAPPING skuVerMapping = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string foxSkuno= Data["FOX_SKUNO"].ToString().Trim().ToUpper();
                string custSkuno = Data["CUST_SKUNO"].ToString().Trim().ToUpper();
                string foxVer1 = Data["FOX_VER1"].ToString().Trim().ToUpper();
                string foxVer2 = Data["FOX_VER2"].ToString().Trim().ToUpper();
                string custVer = Data["CUST_VER"].ToString().Trim().ToUpper();
                string errorStr = foxSkuno + ":" + foxVer1;
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                skuVerMapping = new T_C_SKU_VER_MAPPING(sfcdb, DB_TYPE_ENUM.Oracle);
                if(skuVerMapping.MappingIsExistBySku(foxSkuno, foxVer1,sfcdb))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { errorStr }));
                }
                Row_C_SKU_VER_MAPPING rowMapping = (Row_C_SKU_VER_MAPPING)skuVerMapping.NewRow();
                rowMapping.ID = skuVerMapping.GetNewID(this.BU,sfcdb);
                rowMapping.FOX_SKUNO = foxSkuno;
                rowMapping.FOX_VERSION1 = foxVer1;
                rowMapping.FOX_VERSION2 = foxVer2;
                rowMapping.CUSTOMER_SKUNO = custSkuno;
                rowMapping.CUSTOMER_VERSION = custVer;
                rowMapping.EDIT_EMP = this.LoginUser.EMP_NO;
                rowMapping.EDIT_TIME = GetDBDateTime();
                result =sfcdb.ExecSQL(rowMapping.GetInsertString(this.DBTYPE));

                if (Convert.ToInt32(result) > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.MessageCode = "MES00000021";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000021";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";               
            }
        }

        public void ModifyMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_VER_MAPPING skuVerMapping = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                string foxSkuno = Data["FOX_SKUNO"].ToString().Trim().ToUpper();
                string custSkuno = Data["CUST_SKUNO"].ToString().Trim().ToUpper();
                string foxVer1 = Data["FOX_VER1"].ToString().Trim().ToUpper();
                string foxVer2 = Data["FOX_VER2"].ToString().Trim().ToUpper();
                string custVer = Data["CUST_VER"].ToString().Trim().ToUpper();
                string errorStr = foxSkuno + ":" + foxVer1;
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                skuVerMapping = new T_C_SKU_VER_MAPPING(sfcdb, DB_TYPE_ENUM.Oracle);
                if (skuVerMapping.MappingIsExistBySkuEditMode(foxSkuno, foxVer1, id, sfcdb))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { errorStr }));
                }
                Row_C_SKU_VER_MAPPING rowMapping = (Row_C_SKU_VER_MAPPING)skuVerMapping.GetObjByID(id, sfcdb);
                rowMapping.FOX_VERSION1 = foxVer1;
                rowMapping.FOX_VERSION2 = foxVer2;
                rowMapping.CUSTOMER_SKUNO = custSkuno;
                rowMapping.CUSTOMER_VERSION = custVer;
                rowMapping.EDIT_EMP = this.LoginUser.EMP_NO;
                rowMapping.EDIT_TIME = GetDBDateTime();
                result = sfcdb.ExecSQL(rowMapping.GetUpdateString(this.DBTYPE));

                if (Convert.ToInt32(result) > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(Convert.ToInt32(result));
                    StationReturn.MessageCode = "MES00000035"; 
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Message = e.Message;
                StationReturn.MessageCode = "MES00000025";
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
        }

        public void DeleteMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_VER_MAPPING skuVerMapping = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();                
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                skuVerMapping = new T_C_SKU_VER_MAPPING(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SKU_VER_MAPPING rowMapping = (Row_C_SKU_VER_MAPPING)skuVerMapping.GetObjByID(id, sfcdb);               
                result = sfcdb.ExecSQL(rowMapping.GetDeleteString(this.DBTYPE));

                if (Convert.ToInt32(result) > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.MessageCode = "MES00000004";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.MessageCode = "MES00000023";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000023";
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
        }
    }
}
