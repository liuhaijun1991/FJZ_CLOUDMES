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
    public class StorageItemConfig : MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetStorageByItemNameAndItemName = new APIInfo()
        {
            FunctionName = "GetStorageByItemNameAndItemName",
            Description = "通過ItemName和ItemSon獲取StorageItem（倉碼）",
            Parameters = new List<APIInputInfo>(){
                 new APIInputInfo() { InputName = "ItemName" },
                 new APIInputInfo() { InputName = "ItemSon"}
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetStorageByItemName = new APIInfo()
        {
            FunctionName = "GetStorageByItemName",
            Description = "通過ItemName獲取StorageItem（倉碼）",
            Parameters = new List<APIInputInfo>(){new APIInputInfo() { InputName = "ItemName" }},
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddNewStorageItem = new APIInfo()
        {
            FunctionName = "AddNewStorageItem",
            Description = "添加StorageItem",
            Parameters = new List<APIInputInfo>(){
                 new APIInputInfo() { InputName = "ItemName" },
                 new APIInputInfo() { InputName = "ItemSon"},
                 new APIInputInfo() { InputName = "StorageCode"}
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteById = new APIInfo()
        {
            FunctionName = "DeleteById",
            Description = "通過Id刪除StorageItem",
            Parameters = new List<APIInputInfo>(){
                 new APIInputInfo() { InputName = "Id" }
            },
            Permissions = new List<MESPermission>()
        };
        #endregion 方法信息集合 end
        public StorageItemConfig()
        {
            this.Apis.Add(FGetStorageByItemNameAndItemName.FunctionName, FGetStorageByItemNameAndItemName);
            this.Apis.Add(FGetStorageByItemName.FunctionName, FGetStorageByItemName);
            this.Apis.Add(FAddNewStorageItem.FunctionName, FAddNewStorageItem);
            this.Apis.Add(FDeleteById.FunctionName, FDeleteById);
        }
        /// <summary>
        /// 通過ItemName和ItemSon獲取倉碼
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetStorageByItemNameAndItemSon(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<string> StorageList = new List<string>();
                string ItemName = Data["ItemName"].ToString();
                string ItemSon = Data["ItemSon"].ToString();
                T_C_STORAGE_ITEM TC_STORAGE_ITEM = new T_C_STORAGE_ITEM(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                StorageList = TC_STORAGE_ITEM.GetByItemNameAndItemSon(this.BU, this.LoginUser.EMP_NO, ItemName, ItemSon, sfcdb);
                StorageList.Insert(0, "");
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
        public void GetStorageByItemName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<string> StorageList = new List<string>();
                string ItemName = Data["ItemName"].ToString();               
                T_C_STORAGE_ITEM TC_STORAGE_ITEM = new T_C_STORAGE_ITEM(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                StorageList = TC_STORAGE_ITEM.GetByItemName(ItemName, sfcdb);
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
        /// <summary>
        /// 添加StorageItem
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddNewStorageItem(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();              
                string ItemName = Data["ItemName"].ToString().Trim();
                string ItemSon = Data["ItemSon"].ToString().Trim();
                string StorageCode = Data["StorageCode"].ToString().Trim();
                if (ItemName.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ItemName");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (ItemSon.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ItemSon");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (StorageCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("StorageCode");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                T_C_STORAGE_ITEM TC_STORAGE_ITEM = new T_C_STORAGE_ITEM(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_STORAGE_CODE TC_STORAGE_CODE = new T_C_STORAGE_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
               List<C_STORAGE_CODE> GetSTORAGE_CODEList = new List<C_STORAGE_CODE>();
                C_STORAGE_ITEM GetSTORAGE_ITEM = new C_STORAGE_ITEM();
                GetSTORAGE_CODEList = TC_STORAGE_CODE.GetByStorageCode(StorageCode, sfcdb);
                if (GetSTORAGE_CODEList == null || GetSTORAGE_CODEList.Count() <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add("StorageCode:" + StorageCode);
                }
                else
                {
                    GetSTORAGE_ITEM = TC_STORAGE_ITEM.GetByItemNameAndItemSonAndStorageCode(ItemName, ItemSon, StorageCode,sfcdb);
                    if (GetSTORAGE_ITEM != null)
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000008";
                        StationReturn.MessagePara.Add("ItemName:" + ItemName+ ",ItemSon:"+ ItemSon + ",StorageCode:"+ StorageCode);
                    }
                    else
                    {
                        C_STORAGE_ITEM NewStorageItem = new C_STORAGE_ITEM();
                        NewStorageItem.ID = TC_STORAGE_ITEM.GetNewID(BU,sfcdb);
                        NewStorageItem.ITEM_NAME = ItemName;
                        NewStorageItem.ITEM_SON = ItemSon;
                        NewStorageItem.STORAGE_CODE = StorageCode;
                        NewStorageItem.EDIT_EMP =LoginUser.EMP_NO;
                        NewStorageItem.EDIT_TIME =GetDBDateTime();
                        int result=TC_STORAGE_ITEM.Add(NewStorageItem,sfcdb);
                        if (result > 0)
                        {
                            StationReturn.Data = NewStorageItem;
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            StationReturn.MessageCode = "MES00000001";
                        }
                        else
                        {
                            StationReturn.Data = "";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000021";
                            StationReturn.MessagePara.Add("C_STORAGE_ITEM");
                        }
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
        /// 通過Id刪除
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeleteById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STORAGE_ITEM TC_STORAGE_ITEM = new T_C_STORAGE_ITEM(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_STORAGE_ITEM GetC_STORAGE_ITEM = new C_STORAGE_ITEM();
                string strId = Data["Id"].ToString();
                GetC_STORAGE_ITEM = TC_STORAGE_ITEM.GetById(strId, sfcdb);
                if (GetC_STORAGE_ITEM != null)
                {
                    int result = TC_STORAGE_ITEM.DeleteById(strId, sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Data = strId;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000023";
                        StationReturn.MessagePara.Add("C_STORAGE_ITEM");
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
    }
}
