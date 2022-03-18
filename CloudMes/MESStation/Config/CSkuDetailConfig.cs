using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class CSkuDetailConfig : MesAPIBase
    {
        protected APIInfo FAddCSkuDetail = new APIInfo()
        {
            FunctionName = "AddCSkuDetail",
            Description = "添加CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VALUE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTEND", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VERSION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BASETEMPLATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STATION_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCSkuDetail = new APIInfo()
        {
            FunctionName = "DeleteCSkuDetail",
            Description = "刪除CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCSkuDetailByIDList = new APIInfo()
        {
            FunctionName = "DeleteCSkuDetailByIDList",
            Description = "刪除CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID_List", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectCSkuDetail = new APIInfo()
        {
            FunctionName = "SelectCSkuDetail",
            Description = "查詢CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUdateCSkuDetail = new APIInfo()
        {
            FunctionName = "UdateCSkuDetail",
            Description = "更新CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VALUE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTEND", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VERSION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BASETEMPLATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STATION_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CSkuDetailConfig()
        {
            this.Apis.Add(FAddCSkuDetail.FunctionName, FAddCSkuDetail);
            this.Apis.Add(FDeleteCSkuDetail.FunctionName, FDeleteCSkuDetail);
            this.Apis.Add(FDeleteCSkuDetailByIDList.FunctionName, FDeleteCSkuDetailByIDList);
            this.Apis.Add(FUdateCSkuDetail.FunctionName, FUdateCSkuDetail);
            this.Apis.Add(FSelectCSkuDetail.FunctionName, FSelectCSkuDetail);
        }

        public void AddCSkuDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_DETAIL cSkuDetail = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSkuDetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SKU_DETAIL r = (Row_C_SKU_DETAIL)cSkuDetail.NewRow();
                r.ID = cSkuDetail.GetNewID(this.BU, sfcdb);
                r.SKUNO = (Data["SKUNO"].ToString()).Trim();
                r.CATEGORY = (Data["CATEGORY"].ToString()).Trim();
                r.CATEGORY_NAME = (Data["CATEGORY_NAME"].ToString()).Trim();
                r.VALUE = Data["VALUE"].ToString();
                r.EXTEND = (Data["EXTEND"].ToString()).Trim();
                r.VERSION = (Data["VERSION"].ToString()).Trim();
                r.BASETEMPLATE = (Data["BASETEMPLATE"].ToString()).Trim();
                r.STATION_NAME = (Data["STATION_NAME"].ToString()).Trim();
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "添加成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteCSkuDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_DETAIL cSkuDetail = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSkuDetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                int strRet = cSkuDetail.DeleteSkuDetail((Data["ID"].ToString()).Trim(), sfcdb);
                if (strRet > 0)
                {
                    StationReturn.Message = "刪除成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteCSkuDetailByIDList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                JToken OBJ = Data["ID_List"];
                List<string> ids = OBJ.ToObject<List<string>>();
                int res = sfcdb.ORM.Deleteable<C_SKU_DETAIL>(ids).ExecuteCommand();
                if (res > 0)
                {
                    StationReturn.Message = "刪除成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void UdateCSkuDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_DETAIL cSkuDetail = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSkuDetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                C_SKU_DETAIL r = cSkuDetail.getC_SKU_DETAILbyID((Data["ID"].ToString()).Trim(), sfcdb);
                //r.SKUNO = (Data["SKUNO"].ToString()).Trim();
                r.CATEGORY = (Data["CATEGORY"].ToString()).Trim();
                r.CATEGORY_NAME = (Data["CATEGORY_NAME"].ToString()).Trim();
                r.VALUE = (Data["VALUE"].ToString()).Trim();
                r.EXTEND = (Data["EXTEND"].ToString()).Trim();
                r.VERSION = (Data["VERSION"].ToString()).Trim();
                r.BASETEMPLATE = (Data["BASETEMPLATE"].ToString()).Trim();
                r.STATION_NAME = (Data["STATION_NAME"].ToString()).Trim();
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                int strRet = cSkuDetail.AddOrUpdateSkuDetail("UPDATE", r, sfcdb);


                if (strRet > 0)
                {
                    StationReturn.Message = "添加成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void SelectCSkuDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_DETAIL cSkuDetail = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSkuDetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SKU_DETAIL> list = cSkuDetail.GetSkuDetailBySkuno((Data["SKUNO"].ToString()).Trim(), sfcdb);
                StationReturn.Message = "获取成功！！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = list;
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

        public void CheckSkunoEx(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU cSku = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                if (cSku.CheckSku((Data["SKUNO"].ToString()).Trim(), sfcdb))
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "SKU不存在！";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
    }
}
