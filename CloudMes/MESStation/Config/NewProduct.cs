using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using Newtonsoft.Json;
using MESDataObject;
using System.Reflection;

//using WebServer.SocketService;


namespace MESStation.Config
{
    public class NewProduct : MesAPIBase
    {

        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        #region ApiInfos
        private APIInfo AllSKU = new APIInfo()
        {
            FunctionName = "GetAllSku",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo AllCSKU = new APIInfo()
        {
            FunctionName = "GetAllCSku",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetSkuByRouteId = new APIInfo()
        {
            FunctionName = "GetSkuByRouteId",
            Description = "根據路由 ID 獲取對應的機種",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="RouteId",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo SkuByName = new APIInfo()
        {
            FunctionName = "GetSkuByName",
            Description = "根據機種名模獲取機種",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="Sku_Name",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _UpdateSku = new APIInfo()
        {
            FunctionName = "UpdateSku",
            Description = "修改機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _AddSku = new APIInfo()
        {
            FunctionName = "AddSku",
            Description = "添加機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSku = new APIInfo()
        {
            FunctionName = "DeleteSku",
            Description = "刪除機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSkuById = new APIInfo()
        {
            FunctionName = "DeleteSkuById",
            Description = "刪除機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuID",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        #endregion

        public NewProduct()
        {
            this.Apis.Add(AllSKU.FunctionName, AllSKU);
            this.Apis.Add(SkuByName.FunctionName, SkuByName);
            this.Apis.Add(_AddSku.FunctionName, _AddSku);
            this.Apis.Add(_UpdateSku.FunctionName, _UpdateSku);
            this.Apis.Add(_DeleteSku.FunctionName, _DeleteSku);
            this.Apis.Add(_DeleteSkuById.FunctionName, _DeleteSkuById);
            this.Apis.Add(_GetSkuByRouteId.FunctionName, _GetSkuByRouteId);
        }

        public void GetAllCSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SKU> SkuList = new List<C_SKU>();
            T_C_SKU Table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuList = Table.GetAllCSku(sfcdb);
                if (SkuList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 只顯示最近修改的 20 條機種的信息，因為一次性返回所有數據數量太大
        /// 如果在顯示出來的列表中沒有該機種，則需要輸入機種關鍵字來進行查詢
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAllSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<SkuObject> SkuList = new List<SkuObject>();
            List<C_SKU> SkuCList = new List<C_SKU>();
            T_C_SKU Table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                //SkuList = Table.GetAllSku(sfcdb);
                SkuCList = Table.GetAllCSku(sfcdb);
                if (SkuCList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuCList.Count().ToString());
                    StationReturn.Data = SkuCList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetSkuByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<SkuObject> SkuList = new List<SkuObject>();
            T_C_SKU Table = null;
            string SkuName = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuName = Data["Sku_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(SkuName))
                {
                    GetAllSku(requestValue, Data, StationReturn);
                }
                else
                {
                    SkuList = Table.GetSkuByName(SkuName, sfcdb);
                    if (SkuList.Count() == 0)
                    {
                        //沒有獲取到數據
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        //獲取成功
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(SkuList.Count().ToString());
                        StationReturn.Data = SkuList;
                        
                    }
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }

        public void GetSkuByRouteId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            OleExec sfcdb = null;
            string RouteId = string.Empty;
            List<C_SKU> SkuList = new List<C_SKU>();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                RouteId = Data["RouteId"].ToString();
                if (!string.IsNullOrEmpty(RouteId))
                {
                    SkuList = table.GetSkuListByMappingRouteID(RouteId, sfcdb);
                    if (SkuList.Count() == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(SkuList.Count().ToString());
                        StationReturn.Data = SkuList;

                    }
                }
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetSkuOtherSet(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string SkuObject = string.Empty;
            C_SKU Sku = null;
            SkuOtherSet SkuOtherSetData = new SkuOtherSet();
            T_C_SKU_DETAIL cskudetail = null;
            
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SkuObject = Data["SkuObject"].ToString();
                cskudetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                Type t = SkuOtherSetData.GetType();
                PropertyInfo[] fis = t.GetProperties();
                foreach (PropertyInfo fi in fis)
                {
                    C_SKU_DETAIL dms = cskudetail.GetSkuDetail("OSP_CONTROL", fi.Name, Sku.SKUNO, sfcdb);
                    if (dms!=null)
                        fi.SetValue(SkuOtherSetData, dms.VALUE,null);
                }
                if (SkuOtherSetData == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    //StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuOtherSetData;
                }
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }


        }
        public void GetSkuOtherSetting(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string SkuObject = string.Empty;
            C_SKU Sku = null;
            OtherSetting OtherSettingData = new OtherSetting();

            T_C_SKU_DETAIL cskudetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SkuObject = Data["SkuObject"].ToString();
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                Type t = OtherSettingData.GetType();
                PropertyInfo[] fis = t.GetProperties();
                foreach (PropertyInfo fi in fis)
                {
                    C_SKU_DETAIL dms = cskudetail.GetSkuDetail("OTHERSETTING", fi.Name, Sku.SKUNO, sfcdb);
                    if (dms!=null)
                        fi.SetValue(OtherSettingData, dms.VALUE);
                }

                if (OtherSettingData == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    //StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = OtherSettingData;
                }
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void UpdateSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            string SkuObject = string.Empty;
            string SkuDetail = string.Empty;
            C_SKU Sku = null;
            SkuOtherSet Sku_OtherSet = null;
            string result = string.Empty;
            StringBuilder SkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuObject = Data["SkuObject"].ToString();
                SkuDetail = Data["SkuDetail"].ToString();
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                Sku_OtherSet = (SkuOtherSet)JsonConvert.Deserialize(SkuDetail, typeof(SkuOtherSet));
                Sku.EDIT_EMP = LoginUser.EMP_NO;

                if (Sku.SKU_TYPE == "PCBA")
                {
                    UpdateSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADISOSP", Sku_OtherSet.RADISOSP, sfcdb);
                    UpdateSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADHIGHTEMP", Sku_OtherSet.RADHIGHTEMP, sfcdb);
                }
                else
                {
                    DeleteSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADISOSP", sfcdb);
                    DeleteSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADHIGHTEMP", sfcdb);
                }

                result = Table.UpdateSku(BU, Sku, "UPDATE", GetDBDateTime(),out SkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //更新成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = SkuId.ToString();
                }
                else
                {
                    //更新失敗
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                //不是最新的數據，返回字符串無法被 Int32.Parse 方法轉換成 int,所以出現異常
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000032";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    //數據庫執行異常
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }

        public void AddSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            string SkuObject = string.Empty;
            string SkuDetail = string.Empty;
            C_SKU Sku = null;
            SkuOtherSet Sku_OtherSet = null;
            string result = string.Empty;
            StringBuilder SkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuObject = Data["SkuObject"].ToString();
                SkuDetail = Data["SkuDetail"].ToString();
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                Sku_OtherSet = (SkuOtherSet)JsonConvert.Deserialize(SkuDetail, typeof(SkuOtherSet));
                Sku.EDIT_EMP = LoginUser.EMP_NO;
                if (Sku.SKU_TYPE == "PCBA")
                {
                    UpdateSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADISOSP", Sku_OtherSet.RADISOSP, sfcdb);
                    UpdateSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADHIGHTEMP", Sku_OtherSet.RADHIGHTEMP, sfcdb);
                }
                else
                {
                    DeleteSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADISOSP", sfcdb);
                    DeleteSkuDetail(Sku.SKUNO, "OSP_CONTROL", "RADHIGHTEMP", sfcdb);
                }

                result = Table.UpdateSku(BU, Sku, "ADD", GetDBDateTime(),out SkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //添加成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = SkuId.ToString();

                }
                else
                {
                    //沒有添加任何數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DeleteSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            string SkuObject = string.Empty;
            C_SKU Sku = null;
            string result = string.Empty;
            StringBuilder SkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuObject = Data["SkuObject"].ToString();
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                result = Table.UpdateSku(BU, Sku, "DELETE",GetDBDateTime(),out SkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = SkuId.ToString();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DeleteSkuById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            C_SKU Sku = null;
            string result = string.Empty;
            string SkuId = string.Empty;
            StringBuilder strSkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuId = Data["SkuID"][0].ToString();
                Sku = new C_SKU();
                Sku.ID = SkuId;
                result = Table.UpdateSku(BU, Sku, "DELETE",GetDBDateTime(),out strSkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //刪除成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = strSkuId.ToString();
                }
                else
                {
                    //沒有刪除任何數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void SaveOtherSetting(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            OtherSetting Table = null;
            string OtherSettingData = string.Empty;
            string SkuObject = string.Empty;
            C_SKU Sku = null;
            string strResult = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                OtherSettingData = Data["OtherSetting"].ToString();
                SkuObject = Data["SkuObject"].ToString();
                Table = (OtherSetting)JsonConvert.Deserialize(OtherSettingData, typeof(OtherSetting));
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                Sku.EDIT_EMP = LoginUser.EMP_NO;

                Type t = Table.GetType();
                PropertyInfo[] fls = t.GetProperties();
                foreach (PropertyInfo fi in fls)
                {
                    strResult = "Update " + fi.Name+"Failed!";
                    string sd = fi.GetValue(Table) == null ? "" : fi.GetValue(Table).ToString();
                    bool updFlag = UpdateSkuDetail(Sku.SKUNO, "OTHERSETTING", fi.Name, fi.GetValue(Table) ==null?"": fi.GetValue(Table).ToString(), sfcdb);
                    if (updFlag)
                        strResult = "Update Success";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara.Add("OK");
                StationReturn.Data = Sku.SKUNO;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(strResult))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = e.Message + ":" + strResult;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void GetAllSkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<string> SkuList = new List<string>();
            T_C_SKU Table = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuList = Table.GetAllSkunoList(sfcdb);
                if (SkuList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public bool UpdateSkuDetail(string skuno,string keytype ,string keyname, string keyvalue, OleExec sfcdb)
        {

            T_C_SKU_DETAIL cskudetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            C_SKU_DETAIL dms = cskudetail.GetSkuDetail(keytype, keyname, skuno, sfcdb);
            if (dms!=null)
            {
                dms.VALUE = keyvalue;
                dms.EDIT_EMP = this.LoginUser.EMP_NO;
                dms.EDIT_TIME = GetDBDateTime();
                int strRet = cskudetail.AddOrUpdateSkuDetail("UPDATE", dms, sfcdb);
                
                if (strRet > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Row_C_SKU_DETAIL r = (Row_C_SKU_DETAIL)cskudetail.NewRow();
                r.ID = cskudetail.GetNewID(this.BU, sfcdb);
                r.SKUNO = skuno;
                r.CATEGORY = keytype;
                r.CATEGORY_NAME = keyname;
                r.VALUE = keyvalue;
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DeleteSkuDetail(string skuno, string keytype, string keyname, OleExec sfcdb)
        {
            T_C_SKU_DETAIL cskudetail = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            C_SKU_DETAIL dms = cskudetail.GetSkuDetail(keytype, keyname, skuno, sfcdb);
            if (dms != null)
            {
                int strRet = cskudetail.DeleteSkuDetail(dms.ID, sfcdb);
                if (strRet > 0)
                    return true;
                else
                    return false;
            }
            return true;
        }
    }

    public class SkuOtherSet
    {
        public string RADISOSP { get; set; }
        public string RADHIGHTEMP { get; set; }
    }
    public class OtherSetting
    {
        public string BOXWT { get; set; }
        public string BOXBT { get; set; }
        public string PALLETWT { get; set; }
        public string MATERIALID { get; set; }
        public string NAKEDPALLETWT { get; set; }
        public string DNWTMARGIN { get; set; }
        public string CARTONVOLUME { get; set; }
        public string PALLETVOLUME { get; set; }
        public string ATTCASEIDPREFIX { get; set; }
        public string ATTCASEIDSUFFIX { get; set; }
        public string SLIPPAGE { get; set; }
        public string NAKEDCARTONWT { get; set; }
        public string PALLETWTRATIO { get; set; }
    }
}
