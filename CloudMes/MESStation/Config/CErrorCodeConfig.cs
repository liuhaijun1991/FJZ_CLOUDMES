using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESDBHelper;
using Newtonsoft.Json.Linq;

namespace MESStation.Config
{
 public class CErrorCodeConfig :MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetByErrorCode = new APIInfo()
        {
            FunctionName = "GetByErrorCode",
            Description = "通過ErrorCode獲取ErrorCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "ErrorCode" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetErrorCodeById = new APIInfo()
        {
            FunctionName = "GetErrorCodeById",
            Description = "通過Id獲取ErrorCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Id" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetErrorCodeByFuzzySearch = new APIInfo()
        {
            FunctionName = "GetErrorCodeByFuzzySearch",
            Description = "通過模糊查找（不區分大小寫）獲取ErrorCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SearchValue" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddNewErrorCode = new APIInfo()
        {
            FunctionName = "AddNewErrorCode",
            Description = "添加新的ErrorCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "ErrorCode" },
                new APIInputInfo() { InputName = "EnglishDescription" },
                new APIInputInfo() { InputName = "ChineseDescription" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateErrorCodeById = new APIInfo()
        {
            FunctionName = "UpdateErrorCodeById",
            Description = "通過Id更新ErrorCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "Id" },
                new APIInputInfo() { InputName = "ErrorCode" },
                new APIInputInfo() { InputName = "EnglishDescription" },
                new APIInputInfo() { InputName = "ChineseDescription" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllErrorCode = new APIInfo()
        {
            FunctionName = "GetAllErrorCode",
            Description = "獲取所有ErrorCode信息",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetAllErrorCodeString = new APIInfo()
        {
            FunctionName = "GetAllErrorCodeString",
            Description = "獲取所有ErrorCode 字符串表示",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteErrorCodeById = new APIInfo()
        {
            FunctionName = "DeleteErrorCodeById",
            Description = "通過Id刪除ErrorCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "Id" }
            },
            Permissions = new List<MESPermission>()
        };
      
        #endregion 方法信息集合
        public CErrorCodeConfig()
        {
            this.Apis.Add(FGetByErrorCode.FunctionName, FGetByErrorCode);
            this.Apis.Add(FGetErrorCodeById.FunctionName, FGetErrorCodeById);
            this.Apis.Add(FGetErrorCodeByFuzzySearch.FunctionName, FGetErrorCodeByFuzzySearch);
            this.Apis.Add(FAddNewErrorCode.FunctionName, FAddNewErrorCode);
            this.Apis.Add(FUpdateErrorCodeById.FunctionName, FUpdateErrorCodeById);
            this.Apis.Add(FGetAllErrorCode.FunctionName, FGetAllErrorCode);
            this.Apis.Add(FGetAllErrorCodeString.FunctionName, FGetAllErrorCodeString);
            this.Apis.Add(FDeleteErrorCodeById.FunctionName, FDeleteErrorCodeById);
        }
        public void GetByErrorCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb,MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string ErrorCode = Data["ErrorCode"].ToString();
                SelectErrorCode = TC_ERROR_CODE.GetByErrorCode(ErrorCode, sfcdb);
                if (SelectErrorCode != null)
                {
                    StationReturn.Data = SelectErrorCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(ErrorCode);
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
        public void GetErrorCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string ErrorCodeId = Data["Id"].ToString();
                SelectErrorCode = TC_ERROR_CODE.GetByid(ErrorCodeId, sfcdb);
                if (SelectErrorCode != null)
                {
                    StationReturn.Data = SelectErrorCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(ErrorCodeId);
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
        public void GetErrorCodeByFuzzySearch(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
               List<C_ERROR_CODE> SelectErrorCode = new List<C_ERROR_CODE>();
                string SearchValue = Data["SearchValue"].ToString().ToUpper();
                SelectErrorCode = TC_ERROR_CODE.GetByFuzzySearch(SearchValue, sfcdb);
                if (SelectErrorCode != null)
                {
                    StationReturn.Data = SelectErrorCode;
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
        public void AddNewErrorCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string ErrorCode = Data["ErrorCode"].ToString().Trim();
                string EDescription = Data["EnglishDescription"].ToString().Trim();
                string CDescription = Data["ChineseDescription"].ToString().Trim();
                string ErrorCategory = Data["ErrorCategory"].ToString().Trim();
                bool isDiscDeffect = Data.SelectToken("isDisc").Value<bool>();

                if (ErrorCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ErrorCode");
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
                SelectErrorCode = TC_ERROR_CODE.GetByErrorCode(ErrorCode, sfcdb);
                if (SelectErrorCode == null)
                {
                    C_ERROR_CODE NewErrorCode = new C_ERROR_CODE();
                    NewErrorCode.ID = TC_ERROR_CODE.GetNewID(BU,sfcdb);
                    NewErrorCode.ERROR_CODE = ErrorCode;
                    NewErrorCode.ENGLISH_DESC = EDescription;
                    NewErrorCode.CHINESE_DESC = CDescription;
                    NewErrorCode.EDIT_EMP = LoginUser.EMP_NO;
                    NewErrorCode.EDIT_TIME = GetDBDateTime();
                    NewErrorCode.ERROR_CATEGORY = ErrorCategory;
                    if (isDiscDeffect)
                    {
                        NewErrorCode.DISC_DEFFECT = "YES";
                    }
                    else
                    {
                        NewErrorCode.DISC_DEFFECT = "NO";
                    }
                    int result = TC_ERROR_CODE.AddNewErrorCode(NewErrorCode,sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Data = NewErrorCode;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.MessagePara.Add(ErrorCode);
                    }
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(ErrorCode);
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
        public void UpdateErrorCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string strId = Data["Id"].ToString().Trim();
                string ErrorCode = Data["ErrorCode"].ToString().Trim();
                string EDescription = Data["EnglishDescription"].ToString().Trim();
                string CDescription = Data["ChineseDescription"].ToString().Trim();
                string ErrorCategory = Data["ErrorCategory"].ToString().Trim();
                bool isDiscDeffect = Data.SelectToken("isDisc").Value<bool>();

                if (ErrorCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ErrorCode");
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
                SelectErrorCode= TC_ERROR_CODE.GetByid(strId, sfcdb);
                if (SelectErrorCode == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add("Id:"+strId);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;

                }
                SelectErrorCode = TC_ERROR_CODE.GetByErrorCode(ErrorCode, sfcdb);
                if (SelectErrorCode != null && SelectErrorCode.ID != strId)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(ErrorCode);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }               
                C_ERROR_CODE NewErrorCode = new C_ERROR_CODE();
                NewErrorCode.ID =strId;
                NewErrorCode.ERROR_CODE = ErrorCode;
                NewErrorCode.ENGLISH_DESC = EDescription;
                NewErrorCode.CHINESE_DESC = CDescription;
                NewErrorCode.EDIT_EMP = LoginUser.EMP_NO;
                NewErrorCode.EDIT_TIME = GetDBDateTime();
                NewErrorCode.ERROR_CATEGORY = ErrorCategory;
                if (isDiscDeffect)
                {
                    NewErrorCode.DISC_DEFFECT = "YES";
                }
                    else
                {
                    NewErrorCode.DISC_DEFFECT = "NO";
                }
                int result = TC_ERROR_CODE.UpdateById(NewErrorCode, sfcdb);

                if (result > 0)
                {
                    StationReturn.Data = NewErrorCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.MessagePara.Add(ErrorCode);
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
        public void GetAllErrorCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_ERROR_CODE> SelectErrorCode = new List<C_ERROR_CODE>();
                SelectErrorCode = TC_ERROR_CODE.GetAllErrorCode(sfcdb);
                StationReturn.Data = SelectErrorCode;
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

        public void GetAllErrorCodeString(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<string> ErrorCodeList = TC_ERROR_CODE.GetAllErrorCode(sfcdb).Select(t => t.ERROR_CODE).ToList();
                StationReturn.Data = ErrorCodeList;
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

        public void DeleteErrorCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string strId = Data["Id"].ToString().Trim();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                SelectErrorCode = TC_ERROR_CODE.GetByid(strId, sfcdb);
                if (SelectErrorCode == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(strId);

                }
                else
                {
                    int result = TC_ERROR_CODE.DeleteById(strId, sfcdb);
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

        /// <summary>
        /// 上傳配置Excel文檔
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UploadErrorCodeExcel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                var successCount = 0;
                var failCount = 0;
                var SnExist = 0;
                string _SnExist ="";
                string data = Data["DataList"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);

                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                for (int i = 0; i < array.Count; i++)
                {
                 
                    string ErrorCode = array[i]["ERROR_CODE"].ToString().Trim();
                    string EDescription = array[i]["ENGLISH_DESCRIPTION"].ToString().Trim();
                    string CDescription = array[i]["CHINESE_DESCRIPTION"].ToString().Trim();

                    if (ErrorCode == "" || ErrorCode == null|| EDescription==""|| EDescription== null|| CDescription == "" || CDescription == null)
                    {
                        failCount += 1;
                        continue;
                        
                    }
   
                    SelectErrorCode = TC_ERROR_CODE.GetByErrorCode(ErrorCode, sfcdb);
                    if (SelectErrorCode == null)
                    {
                        C_ERROR_CODE NewErrorCode = new C_ERROR_CODE();
                        NewErrorCode.ID = TC_ERROR_CODE.GetNewID(BU, sfcdb);
                        NewErrorCode.ERROR_CODE = ErrorCode;
                        NewErrorCode.ENGLISH_DESC = EDescription;
                        NewErrorCode.CHINESE_DESC = CDescription;
                        NewErrorCode.EDIT_EMP = LoginUser.EMP_NO;
                        NewErrorCode.EDIT_TIME = GetDBDateTime();
                        int result = TC_ERROR_CODE.AddNewErrorCode(NewErrorCode, sfcdb);
                        if (result > 0)
                        {
                           
                            successCount += 1;
                        }
                        else
                        {
                            
                            failCount += 1;
                        }
                    }
                    else
                    {
                     
                        failCount += 1;
                        SnExist += 1;
                        _SnExist += ErrorCode+",";

                    }

                }
                StationReturn.Data = "";
                if (SnExist > 0)
                {
                    StationReturn.Message = _SnExist+ "already exist!" + "上傳成功[" + successCount.ToString() + "]筆,失敗[" + failCount.ToString() + "]筆!";
                }
                else
                {
                    StationReturn.Message = "上傳成功[" + successCount.ToString() + "]筆,失敗[" + failCount.ToString() + "]筆!";
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
