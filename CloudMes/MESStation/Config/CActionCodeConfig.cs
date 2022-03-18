using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESDBHelper;

namespace MESStation.Config
{
  public  class CActionCodeConfig : MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetByActionCode = new APIInfo()
        {
            FunctionName = "GetByActionCode",
            Description = "通過ActionCode獲取ActionCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "ActionCode" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGeActionCodeById = new APIInfo()
        {
            FunctionName = "GeActionCodeById",
            Description = "通過Id獲取ActionCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Id" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetActionCodeByFuzzySearch = new APIInfo()
        {
            FunctionName = "GetActionCodeByFuzzySearch",
            Description = "通過模糊查找（不區分大小寫）獲取ActionCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SearchValue" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddNewActionCode = new APIInfo()
        {
            FunctionName = "AddNewActionCode",
            Description = "添加新的ActionCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "ActionCode" },
                new APIInputInfo() { InputName = "EnglishDescription" },
                new APIInputInfo() { InputName = "ChineseDescription" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateActionCodeById = new APIInfo()
        {
            FunctionName = "UpdateActionCodeById",
            Description = "通過Id更新ActionCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "Id" },
                new APIInputInfo() { InputName = "ActionCode" },
                new APIInputInfo() { InputName = "EnglishDescription" },
                new APIInputInfo() { InputName = "ChineseDescription" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllActionCode = new APIInfo()
        {
            FunctionName = "GetAllActionCode",
            Description = "獲取所有ActionCode信息",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteActionCodeById = new APIInfo()
        {
            FunctionName = "DeleteActionCodeById",
            Description = "通過Id刪除ActionCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "Id" }
            },
            Permissions = new List<MESPermission>()
        };
        #endregion 方法信息集合
        public CActionCodeConfig()
        {
            this.Apis.Add(FGetByActionCode.FunctionName, FGetByActionCode);
            this.Apis.Add(FGeActionCodeById.FunctionName, FGeActionCodeById);
            this.Apis.Add(FGetActionCodeByFuzzySearch.FunctionName, FGetActionCodeByFuzzySearch);
            this.Apis.Add(FAddNewActionCode.FunctionName, FAddNewActionCode);
            this.Apis.Add(FUpdateActionCodeById.FunctionName, FUpdateActionCodeById);
            this.Apis.Add(FGetAllActionCode.FunctionName, FGetAllActionCode);
            this.Apis.Add(FDeleteActionCodeById.FunctionName, FDeleteActionCodeById);
        }
        public void GetByActionCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ACTION_CODE SelectActionCode = new C_ACTION_CODE();
                string ActionCode = Data["ActionCode"].ToString();
                SelectActionCode = TC_ACTION_CODE.GetByActionCode(ActionCode, sfcdb);
                if (SelectActionCode != null)
                {
                    StationReturn.Data = SelectActionCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(ActionCode);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void GeActionCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ACTION_CODE SelectActionCode = new C_ACTION_CODE();
                string ActionCodeId = Data["Id"].ToString();
                SelectActionCode = TC_ACTION_CODE.GetByid(ActionCodeId, sfcdb);
                if (SelectActionCode != null)
                {
                    StationReturn.Data = SelectActionCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(ActionCodeId);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void GetActionCodeByFuzzySearch(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
               List<C_ACTION_CODE> SelectActionCode = new List<C_ACTION_CODE>();
                string SearchValue = Data["SearchValue"].ToString().ToUpper();
                SelectActionCode = TC_ACTION_CODE.GetByFuzzySearch(SearchValue, sfcdb);
                if (SelectActionCode != null)
                {
                    StationReturn.Data = SelectActionCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(SearchValue);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void AddNewActionCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ACTION_CODE SelectActionCode = new C_ACTION_CODE();
                string ActionCode = Data["ActionCode"].ToString().Trim();
                string EDescription = Data["EnglishDescription"].ToString().Trim();
                string CDescription = Data["ChineseDescription"].ToString().Trim();
                if (ActionCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ActionCode");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (EDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("EnglishDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (CDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ChineseDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                SelectActionCode = TC_ACTION_CODE.GetByActionCode(ActionCode, sfcdb);
                if (SelectActionCode == null)
                {
                    C_ACTION_CODE NewActionCode = new C_ACTION_CODE();
                    NewActionCode.ID = TC_ACTION_CODE.GetNewID(BU, sfcdb);
                    NewActionCode.ACTION_CODE = ActionCode;
                    NewActionCode.ENGLISH_DESC = EDescription;
                    NewActionCode.CHINESE_DESC = CDescription;
                    NewActionCode.EDIT_EMP = LoginUser.EMP_NO;
                    NewActionCode.EDIT_TIME = GetDBDateTime();
                    int result = TC_ACTION_CODE.AddNewActionCode(NewActionCode, sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Data = NewActionCode;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.MessagePara.Add(ActionCode);
                    }
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(ActionCode);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void UpdateActionCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ACTION_CODE SelectActionCode = new C_ACTION_CODE();
                string strId = Data["Id"].ToString().Trim();
                string ActionCode = Data["ActionCode"].ToString().Trim();
                string EDescription = Data["EnglishDescription"].ToString().Trim();
                string CDescription = Data["ChineseDescription"].ToString().Trim();
                if (ActionCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ActionCode");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (EDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("EnglishDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (CDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ChineseDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                SelectActionCode = TC_ACTION_CODE.GetByid(strId, sfcdb);
                if (SelectActionCode == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add("Id:" + strId);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;

                }
                SelectActionCode = TC_ACTION_CODE.GetByActionCode(ActionCode, sfcdb);
                if (SelectActionCode != null && SelectActionCode.ID != strId)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(ActionCode);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                C_ACTION_CODE NewActionCode = new C_ACTION_CODE();
                NewActionCode.ID = strId;
                NewActionCode.ACTION_CODE = ActionCode;
                NewActionCode.ENGLISH_DESC = EDescription;
                NewActionCode.CHINESE_DESC = CDescription;
                NewActionCode.EDIT_EMP = LoginUser.EMP_NO;
                NewActionCode.EDIT_TIME = GetDBDateTime();
                int result = TC_ACTION_CODE.UpdateById(NewActionCode, sfcdb);
                if (result > 0)
                {
                    StationReturn.Data = NewActionCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.MessagePara.Add(ActionCode);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void GetAllActionCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_ACTION_CODE> SelectActionCode = new List<C_ACTION_CODE>();              
                SelectActionCode = TC_ACTION_CODE.GetAllActionCode(sfcdb);               
                StationReturn.Data = SelectActionCode;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";               
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetAllActionCodeString(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<string> SelectActionCode  = TC_ACTION_CODE.GetAllActionCode(sfcdb).Select(t=>t.ACTION_CODE).ToList();
                StationReturn.Data = SelectActionCode;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void DeleteActionCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string strId = Data["Id"].ToString().Trim();
                T_C_ACTION_CODE TC_ACTION_CODE = new T_C_ACTION_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ACTION_CODE SelectActionCode = new C_ACTION_CODE();
                SelectActionCode = TC_ACTION_CODE.GetByid(strId,sfcdb);
                if (SelectActionCode == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(strId);

                }
                else
                {
                    int result = TC_ACTION_CODE.DeleteById(strId, sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000023";
                        StationReturn.MessagePara.Add(strId);
                    }
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    }
}
