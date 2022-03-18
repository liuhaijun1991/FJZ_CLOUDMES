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
   public class StorageCodeConfig: MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetAllStorage = new APIInfo()
        {
            FunctionName = "GetAllStorage",
            Description = "獲取所有的StorageCode（倉碼）",
            Parameters = new List<APIInputInfo>(){},
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetStorageByPlant = new APIInfo()
        {
            FunctionName = "GetStorageByPlant",
            Description = "通過廠別獲取StorageCode（倉碼）",
            Parameters = new List<APIInputInfo>(){new APIInputInfo() { InputName = "Plant" }},
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetStorageById = new APIInfo()
        {
            FunctionName = "GetStorageById",
            Description = "通過Id獲取StorageCode（倉碼）",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Id" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetStorageByStorageCode = new APIInfo()
        {
            FunctionName = "GetStorageByStorageCode",
            Description = "通過StorageCode獲取StorageCode信息（倉碼）",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "StorageCode" } },
            Permissions = new List<MESPermission>()
        };
        
        protected APIInfo FAddNewStorage = new APIInfo()
        {
            FunctionName = "AddNewStorage",
            Description = "添加StorageCode（倉碼）",
            Parameters = new List<APIInputInfo>(){
                 new APIInputInfo() { InputName = "Plant" },
                 new APIInputInfo() { InputName = "StorageCode" },
                 new APIInputInfo() { InputName = "Descriptions" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateStorageById = new APIInfo()
        {
            FunctionName = "UpdateStorageById",
            Description = "更新StorageCode（倉碼）",
            Parameters = new List<APIInputInfo>(){
                 new APIInputInfo() { InputName = "Id" },
                 new APIInputInfo() { InputName = "Plant" },
                 new APIInputInfo() { InputName = "StorageCode" },
                 new APIInputInfo() { InputName = "Descriptions" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteByIdArray = new APIInfo()
        {
            FunctionName = "DeleteByIdArray",
            Description = "通過Id數組刪除StorageCode（倉碼）",
            Parameters = new List<APIInputInfo>(){new APIInputInfo() { InputName = "IdArray" }},
            Permissions = new List<MESPermission>()
        };
        #endregion 方法信息集合 end
        public StorageCodeConfig()
        {
            this.Apis.Add(FGetAllStorage.FunctionName, FGetAllStorage);
            this.Apis.Add(FGetStorageByPlant.FunctionName, FGetStorageByPlant);
            this.Apis.Add(FGetStorageById.FunctionName, FGetStorageById);            
            this.Apis.Add(FGetStorageByStorageCode.FunctionName, FGetStorageByStorageCode);
            this.Apis.Add(FAddNewStorage.FunctionName, FAddNewStorage);
            this.Apis.Add(FUpdateStorageById.FunctionName, FUpdateStorageById);
            this.Apis.Add(FDeleteByIdArray.FunctionName, FDeleteByIdArray);         
        }     
        public void GetAllStorage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<C_STORAGE_CODE> StorageList = new List<C_STORAGE_CODE>();
                T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                StorageList = TC_STORAGE_CODE.GetAll(sfcdb);
                StationReturn.Data = StorageList;
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
        public void GetStorageByPlant(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string strPlant = Data["Plant"].ToString();
                List<C_STORAGE_CODE> StorageList = new List<C_STORAGE_CODE>();
                T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                StorageList = TC_STORAGE_CODE.GetByPlant(strPlant, sfcdb);
                StationReturn.Data = StorageList;
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
        public void GetStorageById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string strId = Data["Id"].ToString();
                C_STORAGE_CODE Storage = new C_STORAGE_CODE();
                T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                Storage = TC_STORAGE_CODE.GetById(strId, sfcdb);
                StationReturn.Data = Storage;
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
        public void GetStorageByStorageCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string StorageCode = Data["StorageCode"].ToString();
                List<C_STORAGE_CODE> Storage = new List<C_STORAGE_CODE>();
                T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                Storage = TC_STORAGE_CODE.GetByStorageCode(StorageCode, sfcdb);
                StationReturn.Data = Storage;
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
        public void AddNewStorage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                string strPlant = Data["Plant"].ToString().Trim();
                string strStorageCode = Data["StorageCode"].ToString().Trim();
                string strDescription = Data["Descriptions"].ToString().Trim();
                if (strPlant.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("Plant");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (strStorageCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("StorageCode");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (TC_STORAGE_CODE.GetByPlantAndStorageCode(strPlant, strStorageCode, sfcdb) != null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add("Plant:" + strPlant + ",StorageCode:" + strStorageCode);
                }
                else
                {
                    C_STORAGE_CODE NewStorageCode = new C_STORAGE_CODE();
                    NewStorageCode.ID = TC_STORAGE_CODE.GetNewID(BU, sfcdb);
                    NewStorageCode.PLANT = strPlant;
                    NewStorageCode.STORAGE_CODE = strStorageCode;
                    NewStorageCode.DESCRIPTION = strDescription;
                    NewStorageCode.EDIT_EMP = LoginUser.EMP_NO;
                    NewStorageCode.EDIT_TIME = GetDBDateTime();
                    int result = TC_STORAGE_CODE.Add(NewStorageCode, sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Data = NewStorageCode;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.MessagePara.Add("C_STORAGE_CODE");
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
        public void UpdateStorageById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_STORAGE_CODE GetC_STORAGE_CODE = new C_STORAGE_CODE();
                string strId = Data["Id"].ToString();
                string strPlant = Data["Plant"].ToString().Trim();
                string strStorageCode = Data["StorageCode"].ToString().Trim();
                string strDescription = Data["Descriptions"].ToString().Trim();
                if (strPlant.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("Plant");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (strStorageCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("StorageCode");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                GetC_STORAGE_CODE = TC_STORAGE_CODE.GetById(strId, sfcdb);
                if (GetC_STORAGE_CODE != null)
                {
                    GetC_STORAGE_CODE = TC_STORAGE_CODE.GetByPlantAndStorageCode(strPlant, strStorageCode, sfcdb);
                    if (GetC_STORAGE_CODE != null && GetC_STORAGE_CODE.ID != strId)
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000007";
                        StationReturn.MessagePara.Add("Plant:" + strPlant + ",StorageCode:" + strStorageCode);
                    }
                    else
                    {
                        C_STORAGE_CODE NewStorageCode = new C_STORAGE_CODE();
                        NewStorageCode.ID = strId;
                        NewStorageCode.PLANT = strPlant;
                        NewStorageCode.STORAGE_CODE = strStorageCode;
                        NewStorageCode.DESCRIPTION = strDescription;
                        NewStorageCode.EDIT_EMP = LoginUser.EMP_NO;
                        NewStorageCode.EDIT_TIME = GetDBDateTime();
                        int result = TC_STORAGE_CODE.UpdateById(NewStorageCode, sfcdb);
                        if (result > 0)
                        {
                            StationReturn.Data = NewStorageCode;
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            StationReturn.MessageCode = "MES00000001";
                        }
                        else
                        {
                            StationReturn.Data = "";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000025";
                            StationReturn.MessagePara.Add("C_STORAGE_CODE");
                        }
                    }
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add("Id:" + strId);
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
        public void DeleteByIdArray(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
          {
              OleExec sfcdb = null;
              try
              {
                  sfcdb = this.DBPools["SFCDB"].Borrow();
                  T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                  C_STORAGE_CODE GetC_STORAGE_CODE = new C_STORAGE_CODE();
                string strIdArray = Data["IdArray"].ToString().Trim();
                strIdArray = strIdArray.Replace("[","").Replace("]","");
                strIdArray = strIdArray.Replace("\"","");
                  string[]IdArray = strIdArray.Replace("\r\n","").Split(',');
                  bool isError = false;
                if (IdArray == null || IdArray.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("IdArray");
                }
                else
                {
                    isError = false;
                    sfcdb.BeginTrain();
                    for (int i = 0; i < IdArray.Length; i++)
                    {                       
                        GetC_STORAGE_CODE = TC_STORAGE_CODE.GetById(IdArray[i].Trim(), sfcdb);
                        if (GetC_STORAGE_CODE != null)
                        {
                            int result = TC_STORAGE_CODE.DeleteById(IdArray[i].Trim(), sfcdb);
                            if (result<= 0)
                            {
                                StationReturn.Data = IdArray[i].Trim();
                                StationReturn.Status = StationReturnStatusValue.Fail;
                                StationReturn.MessageCode = "MES00000023";
                                StationReturn.MessagePara.Add("C_STORAGE_CODE");
                                isError = true;
                                break;
                            }
                        }
                        else
                        {
                            StationReturn.Data = "";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000007";
                            StationReturn.MessagePara.Add("Id:" + IdArray[i].Trim());
                            isError = true;
                            break;
                        }
                    }
                    if (isError)
                    {
                        sfcdb.RollbackTrain();
                    }
                    else
                    {
                        sfcdb.CommitTrain();
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
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
