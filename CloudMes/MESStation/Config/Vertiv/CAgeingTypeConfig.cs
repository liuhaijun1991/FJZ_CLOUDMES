using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.Vertiv
{
    public class CAgeingTypeConfig : MesAPIBase
    {
        protected APIInfo FGetAgeingTypeList = new APIInfo()
        {
            FunctionName = "GetAgeingTypeList",
            Description = "Get ageing type list by type",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Type", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddNewAgeingType = new APIInfo()
        {
            FunctionName = "AddNewAgeingType",
            Description = "add new ageing type",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Type", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Area_Code", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Max_Time", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Max_Percentage", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Min_Time", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Min_Percentage", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FModifyAgeingType = new APIInfo()
        {
            FunctionName = "ModifyAgeingType",
            Description = "modify ageing type",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Area_Code", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Max_Time", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Max_Percentage", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Min_Time", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Min_Percentage", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteAgeingType = new APIInfo()
        {
            FunctionName = "DeleteAgeingType",
            Description = "delete ageing type by id",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddNewWoAgeingByShippingAddress = new APIInfo()
        {
            FunctionName = "AddNewWoAgeingByShippingAddress",
            Description = "Add new wo ageing by shipping address",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AreaCode", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddNewWoAgeingByAgeingType = new APIInfo()
        {
            FunctionName = "AddNewWoAgeingByAgeingType",
            Description = "Add new wo ageing by ageing type",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AgeingType", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAllShippingAddress = new APIInfo()
        {
            FunctionName = "GetAllShippingAddress",
            Description = "Get all shipping address",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAllShippingCode = new APIInfo() {
            FunctionName = "GetAllShippingCode",
            Description = "Get all shipping code",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetWoControlList = new APIInfo()
        {
            FunctionName = "GetWoControlList",
            Description = "Get wo control list",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteWoAgeingByID = new APIInfo()
        {
            FunctionName = "DeleteWoAgeingByID",
            Description = "delete wo ageing control by id",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public CAgeingTypeConfig()
        {
            this.Apis.Add(FGetAgeingTypeList.FunctionName, FGetAgeingTypeList);
            this.Apis.Add(FAddNewAgeingType.FunctionName, FAddNewAgeingType);
            this.Apis.Add(FModifyAgeingType.FunctionName, FModifyAgeingType);
            this.Apis.Add(FDeleteAgeingType.FunctionName, FDeleteAgeingType);
            this.Apis.Add(FAddNewWoAgeingByShippingAddress.FunctionName, FAddNewWoAgeingByShippingAddress);
            this.Apis.Add(FAddNewWoAgeingByAgeingType.FunctionName, FAddNewWoAgeingByAgeingType);
            this.Apis.Add(FGetWoControlList.FunctionName, FGetWoControlList);
            this.Apis.Add(FDeleteWoAgeingByID.FunctionName, FDeleteWoAgeingByID);
            this.Apis.Add(FGetAllShippingAddress.FunctionName, FGetAllShippingAddress);
        }

        public void GetAgeingTypeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AGEING_TYPE ageingType = null;
            T_C_SHIPPING_ADDRESS t_c_shiping_address = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                ageingType = new T_C_AGEING_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                t_c_shiping_address = new T_C_SHIPPING_ADDRESS(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_AGEING_TYPE> list = ageingType.GetAgeingTypeList(sfcdb, (Data["Type"].ToString().ToUpper()).Trim());
                if (list.Count > 0)
                {
                    foreach (C_AGEING_TYPE c in list)
                    {
                        c.AGEING_AREA_CODE = t_c_shiping_address.GetAddressByAreaCode(sfcdb, c.AGEING_AREA_CODE);
                    }
                    StationReturn.MessageCode = "MSGCODE20210814161629";
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
        public void AddNewAgeingType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AGEING_TYPE ageingType = null;
            OleExec sfcdb = null;
            try
            {
                int result;
                string type = Data["Type"].ToString().Trim().ToUpper();
                string areaCode = Data["Area_Code"].ToString().Trim().ToUpper();
                string maxTime = Data["Max_Time"].ToString().Trim().ToUpper();
                string maxPercentage = Data["Max_Percentage"].ToString().Trim().ToUpper();
                string minTime = Data["Min_Time"].ToString().Trim().ToUpper();
                string minPercentage = Data["Min_Percentage"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                ageingType = new T_C_AGEING_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                if (ageingType.TypeIsExist(sfcdb, type, areaCode))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { type }));
                }

                result = ageingType.AddNewAgeingType(this.BU, type, areaCode, maxTime, maxPercentage, minTime, minPercentage, LoginUser.EMP_NO, sfcdb);
                this.DBPools["SFCDB"].Return(sfcdb);
                if (result > 0)
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
        public void ModifyAgeingType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AGEING_TYPE ageingType = null;
            OleExec sfcdb = null;
            try
            {
                int result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                string areaCode = Data["Area_Code"].ToString().Trim().ToUpper();
                string maxTime = Data["Max_Time"].ToString().Trim().ToUpper();
                string maxPercentage = Data["Max_Percentage"].ToString().Trim().ToUpper();
                string minTime = Data["Min_Time"].ToString().Trim().ToUpper();
                string minPercentage = Data["Min_Percentage"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                ageingType = new T_C_AGEING_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                result = result = ageingType.UpdateAgeingTypeById(id, areaCode, maxTime, maxPercentage, minTime, minPercentage, LoginUser.EMP_NO, sfcdb);
                if (result > 0)
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
        public void DeleteAgeingType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AGEING_TYPE ageingType = null;
            OleExec sfcdb = null;
            try
            {
                int result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                ageingType = new T_C_AGEING_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                result = ageingType.DeleteAgeingTypeById(id, sfcdb);
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

        public void GetWoControlList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_AGEING woAgeing = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                woAgeing = new T_R_WO_AGEING(sfcdb, DB_TYPE_ENUM.Oracle);
                var list = woAgeing.GetWoAgeingList(sfcdb, (Data["WO"].ToString().ToUpper()).Trim());
                if (list != null)
                {
                    StationReturn.MessageCode = "MSGCODE20210814161629";
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
        public void AddNewWoAgeingByShippingAddress(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_AGEING woAgeing = null;
            R_WO_AGEING ageingObject = null;
            C_AGEING_TYPE typeObject = null;
            T_C_AGEING_TYPE t_c_ageing_type= null;
            T_C_SHIPPING_ADDRESS t_c_shipping_address = null;
            C_SHIPPING_ADDRESS c_shipping_address = null;
            T_R_WO_BASE t_r_wo_base = null;
            R_WO_BASE r_wo_base = null;
            OleExec sfcdb = null;
            double maxQty;
            double minQty;
            try
            {
                int result = 0;
                string wo = Data["WO"].ToString().Trim().ToUpper();
                string shippingArea = Data["SHIPPING_AREA"].ToString().Trim().ToUpper();                
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                woAgeing = new T_R_WO_AGEING(sfcdb, DB_TYPE_ENUM.Oracle);
                t_r_wo_base = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                r_wo_base = t_r_wo_base.GetWoByWoNo(wo, sfcdb);
                if (r_wo_base == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { wo }));
                }
                if (r_wo_base.CLOSED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000100"));
                }
                t_c_shipping_address = new T_C_SHIPPING_ADDRESS(sfcdb, DB_TYPE_ENUM.Oracle);
                c_shipping_address = t_c_shipping_address.GetShippingAddressByShippingArea(sfcdb, shippingArea);
                if (c_shipping_address == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026095501", new string[] { shippingArea }));
                }

                ageingObject = woAgeing.GetWoAgeingObject(sfcdb, r_wo_base.WORKORDERNO);
                if (ageingObject == null)
                {
                    result = woAgeing.AddNewWoAgeingAreaCode(sfcdb, BU, r_wo_base.WORKORDERNO, r_wo_base.SKUNO, c_shipping_address.SHIPPING_AREA, LoginUser.EMP_NO);
                }
                else if (ageingObject.AGEING_TYPE == null || ageingObject.AGEING_TYPE.ToString() == "")
                {
                    result = woAgeing.UpdateAreaCodeByWO(sfcdb, r_wo_base.WORKORDERNO, c_shipping_address.SHIPPING_ADDRESS);
                }
                else
                {
                    t_c_ageing_type = new T_C_AGEING_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                    typeObject = t_c_ageing_type.GetObjectByTypeAndAddress(sfcdb, ageingObject.AGEING_TYPE, c_shipping_address.SHIPPING_AREA);
                    if (typeObject == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026095637", new string[] { c_shipping_address.SHIPPING_ADDRESS, ageingObject.AGEING_TYPE }));
                    }
                    maxQty = Math.Ceiling((double)r_wo_base.WORKORDER_QTY * Convert.ToDouble(typeObject.MAX_AGEING_PERCENTAGE));
                    minQty = (double)r_wo_base.WORKORDER_QTY - maxQty;
                    result = woAgeing.ModifyWoAgeingAreaCodeById(sfcdb, ageingObject.ID, c_shipping_address.SHIPPING_AREA, typeObject.MAX_AGEING_TIME, maxQty.ToString(),
                        typeObject.MIN_AGEING_TIME, minQty.ToString(), LoginUser.EMP_NO);
                }

                this.DBPools["SFCDB"].Return(sfcdb);
                if (result > 0)
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
        public void AddNewWoAgeingByAgeingType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_AGEING woAgeing = null;
            R_WO_AGEING ageingObject = null;
            C_AGEING_TYPE typeObject = null;
            T_C_AGEING_TYPE t_c_ageing_type = null;
            T_R_WO_BASE t_r_wo_base = null;
            R_WO_BASE r_wo_base = null;
            OleExec sfcdb = null;
            T_C_SHIPPING_ADDRESS t_c_shipping_address = null;
            C_SHIPPING_ADDRESS c_shipping_address = null;
            double maxQty;
            double minQty;
            try
            {
                int result = 0;
                string wo = Data["WO"].ToString().Trim().ToUpper();
                string type = Data["Type"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                woAgeing = new T_R_WO_AGEING(sfcdb, DB_TYPE_ENUM.Oracle);
                t_r_wo_base = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                r_wo_base = t_r_wo_base.GetWoByWoNo(wo, sfcdb);
                if (r_wo_base == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { wo }));
                }
                if (r_wo_base.CLOSED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000100")); 
                }
                ageingObject = woAgeing.GetWoAgeingObject(sfcdb, r_wo_base.WORKORDERNO);
                if (ageingObject == null)
                {
                    result = woAgeing.AddNewWoAgeingType(sfcdb, BU, r_wo_base.WORKORDERNO, r_wo_base.SKUNO, type, LoginUser.EMP_NO);
                }
                else if (ageingObject.AGEING_AREA_CODE == null || ageingObject.AGEING_AREA_CODE.ToString() == "")
                {
                    result = woAgeing.UpdateAgeingTypeByWO(sfcdb, r_wo_base.WORKORDERNO, type);
                }
                else
                {
                    t_c_shipping_address = new T_C_SHIPPING_ADDRESS(sfcdb, DB_TYPE_ENUM.Oracle);
                    c_shipping_address = t_c_shipping_address.GetShippingAddressByShippingArea(sfcdb, ageingObject.AGEING_AREA_CODE);
                    if (c_shipping_address == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026095501",new string[] { ageingObject.AGEING_AREA_CODE }));
                    }
                    t_c_ageing_type = new T_C_AGEING_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                    typeObject = t_c_ageing_type.GetObjectByTypeAndAddress(sfcdb,type, c_shipping_address.SHIPPING_AREA);
                    if (typeObject == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026095637", new string[] {c_shipping_address.SHIPPING_ADDRESS, type}));
                    }
                    maxQty = Math.Ceiling((double)r_wo_base.WORKORDER_QTY * Convert.ToDouble(typeObject.MAX_AGEING_PERCENTAGE));
                    minQty = (double)r_wo_base.WORKORDER_QTY - maxQty;
                    result = woAgeing.ModifyWoAgeingTypeById(sfcdb, ageingObject.ID, typeObject.AGEING_TYPE, typeObject.MAX_AGEING_TIME, maxQty.ToString(),
                        typeObject.MIN_AGEING_TIME, minQty.ToString(), LoginUser.EMP_NO);
                }

                this.DBPools["SFCDB"].Return(sfcdb);
                if (result > 0)
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
        public void DeleteWoAgeingByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_AGEING ageingControl = null;
            OleExec sfcdb = null;
            try
            {
                int result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                ageingControl = new T_R_WO_AGEING(sfcdb, DB_TYPE_ENUM.Oracle);
                result = ageingControl.DeleteWoAgeingById(sfcdb,id);
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
        public void GetAllShippingAddress(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SHIPPING_ADDRESS shippingAddress = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                shippingAddress = new T_C_SHIPPING_ADDRESS(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SHIPPING_ADDRESS> list = shippingAddress.GetShippingAddressList(sfcdb);
                if (list.Count > 0)
                {  
                    StationReturn.MessageCode = "MSGCODE20210814161629";
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
        public void GetAllShippingCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SHIPPING_ADDRESS shippingAddress = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                shippingAddress = new T_C_SHIPPING_ADDRESS(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SHIPPING_ADDRESS> list = shippingAddress.GetShippingAddressList(sfcdb);
                if (list.Count > 0)
                {
                    List<Dictionary<string, string>> showList = new List<Dictionary<string, string>>();
                    Dictionary<string, string> areaCode = new Dictionary<string, string>(); ;
                    foreach (var c in list)
                    {
                        if (areaCode.ContainsKey(c.AREA_CODE))
                        {
                            areaCode[c.AREA_CODE] = areaCode[c.AREA_CODE] + "," + c.SHIPPING_ADDRESS;
                        }
                        else
                        {
                            areaCode.Add(c.AREA_CODE, c.SHIPPING_ADDRESS);
                        }
                    }
                    foreach (var t in areaCode)
                    {
                        Dictionary<string, string> address = new Dictionary<string, string>();
                        address.Add("AREA_CODE", t.Key);
                        address.Add("SHIPPING_ADDRESS", t.Value);
                        showList.Add(address);
                    }

                    StationReturn.MessageCode = "MSGCODE20210814161629";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = showList;
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
    }
}
