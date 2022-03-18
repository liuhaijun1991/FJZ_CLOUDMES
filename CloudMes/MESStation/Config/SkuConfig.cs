using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using WebServer.SocketService;


namespace MESStation.Config
{
    public class SkuConfig : MesAPIBase
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

        private APIInfo _CopySku = new APIInfo()
        {
            FunctionName = "CopySku",
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

        /// <summary>
        /// add by Taylor 2018年9月4日16:10:29
        /// </summary>
        private APIInfo _GetAllSkuList = new APIInfo()
        {
            FunctionName = "GetAllSkuList",
            Description = "获取所有机种清单",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        #endregion

        public SkuConfig()
        {
            this.Apis.Add(AllSKU.FunctionName, AllSKU);
            this.Apis.Add(SkuByName.FunctionName, SkuByName);
            this.Apis.Add(_AddSku.FunctionName, _AddSku);
            this.Apis.Add(_UpdateSku.FunctionName, _UpdateSku);
            this.Apis.Add(_DeleteSku.FunctionName, _DeleteSku);
            this.Apis.Add(_DeleteSkuById.FunctionName, _DeleteSkuById);
            this.Apis.Add(_GetSkuByRouteId.FunctionName, _GetSkuByRouteId);
            this.Apis.Add(_GetAllSkuList.FunctionName, _GetAllSkuList);
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
            T_C_SKU Table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                //SkuList = Table.GetAllSku(sfcdb);
                var SkuCList = Table.GetSKUList(sfcdb);                
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

        public void GetSingleSkuByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                string SkuName = Data["Sku_Name"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var res = sfcdb.ORM.Queryable<C_SKU>().Where(x => x.SKUNO == SkuName).ToList();
                if (res.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(res.Count().ToString());
                    StationReturn.Data = res;
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
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

        public void UpdateSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
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
                if (this.BU.ToUpper() == "HWD")
                {
                    //int insertResult = HWD.InsertOldDB.InsertSfccodelike(Sku.SKUNO, Sku.SKU_TYPE, Sku.SKU_NAME, Sku.SKU_NAME,
                    //     Sku.DESCRIPTION, Sku.VERSION, Sku.CUST_PARTNO, "", LoginUser.EMP_NO);
                    //if (insertResult <= 0)
                    //{
                    //    throw new Exception("Insert sfccodelike fail!");
                    //}
                    //if (Sku.SKU_TYPE.Equals("MODEL"))
                    //{
                        OleExec apdb = this.DBPools["APDB"].Borrow();
                        HWD.InsertOldDB.InsertProductionConfig(apdb, Sku.SKUNO, Sku.VERSION, Sku.SKU_NAME, Sku.CUST_PARTNO, LoginUser.EMP_NO);
                        this.DBPools["APDB"].Return(apdb);

                    //}

                }
                if (!sfcdb.ORM.Queryable<C_SKU>().Any(t => t.ID == Sku.ID && t.SKUNO == Sku.SKUNO))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MSGCODE20180906104529";
                    return;
                }

                Sku.EDIT_EMP = LoginUser.EMP_NO;
                result = Table.UpdateSku(BU, Sku, "UPDATE", GetDBDateTime(), out SkuId, sfcdb);

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
            C_SKU Sku = null;
            string result = string.Empty;
            StringBuilder SkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuObject = Data["SkuObject"].ToString();
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                if (this.BU.ToUpper() == "HWD")
                {
                    //int insertResult = HWD.InsertOldDB.InsertSfccodelike(Sku.SKUNO, Sku.SKU_TYPE, Sku.SKU_NAME, Sku.SKU_NAME,
                    //     Sku.DESCRIPTION, Sku.VERSION, Sku.CUST_PARTNO, "", LoginUser.EMP_NO);
                    //if (insertResult <= 0)
                    //{
                    //    throw new Exception("Insert sfccodelike fail!");
                    //}
                    //if (Sku.SKU_TYPE.Equals("MODEL"))
                    //{
                        OleExec apdb = this.DBPools["APDB"].Borrow();
                        HWD.InsertOldDB.InsertProductionConfig(apdb, Sku.SKUNO, Sku.VERSION, Sku.SKU_NAME, Sku.CUST_PARTNO, LoginUser.EMP_NO);
                        this.DBPools["APDB"].Return(apdb);
                    //}
                }
                if (sfcdb.ORM.Queryable<C_SKU>().Any(t => t.SKUNO == Sku.SKUNO))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MSGCODE20180906104037";
                    return;
                }

                Sku.EDIT_EMP = LoginUser.EMP_NO;
                result = Table.UpdateSku(BU, Sku, "ADD", GetDBDateTime(), out SkuId, sfcdb);

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
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void CopySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            string SkuObject = string.Empty;
            C_SKU Sku = null;
            string result = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                var skudata = Data["SkuObject"];
                var cpfrom = skudata["COPYFROMSKU"];
                string CopyFrom = cpfrom.ToString();
                //cpfrom.Remove();
                SkuObject = skudata.ToString();
                Sku = (C_SKU)JsonConvert.Deserialize(SkuObject, typeof(C_SKU));
                var x = sfcdb.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == Sku.SKUNO && t.VERSION == Sku.VERSION).ToList();
                if (x.Count > 0)
                {
                    //機種已存在，不可複製
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000151";
                    StationReturn.MessagePara.Add(Sku.SKUNO);
                    StationReturn.MessagePara.Add(Sku.VERSION);
                    StationReturn.Data = Sku.SKUNO;
                    return;
                }
                if (this.BU.ToUpper() == "HWD")
                {
                    //int insertResult = HWD.InsertOldDB.InsertSfccodelike(Sku.SKUNO, Sku.SKU_TYPE, Sku.SKU_NAME, Sku.SKU_NAME,
                    //     Sku.DESCRIPTION, Sku.VERSION, Sku.CUST_PARTNO, "", LoginUser.EMP_NO);
                    //if (insertResult <= 0)
                    //{
                    //    throw new Exception("Insert sfccodelike fail!");
                    //}
                    //if (Sku.SKU_TYPE.Equals("MODEL"))
                    // {
                    OleExec apdb = this.DBPools["APDB"].Borrow();
                    HWD.InsertOldDB.InsertProductionConfig(apdb, Sku.SKUNO, Sku.VERSION, Sku.SKU_NAME, Sku.CUST_PARTNO, LoginUser.EMP_NO);
                    this.DBPools["APDB"].Return(apdb);
                    //}
                }
                string id = Table.GetNewID(BU, sfcdb);
                Sku.ID = id;
                Sku.EDIT_TIME = DateTime.Now;
                Sku.EDIT_EMP = LoginUser.EMP_NO;

                int n = sfcdb.ORM.Insertable<C_SKU>(Sku).ExecuteCommand();
                CopyOption(Sku.SKUNO, CopyFrom, sfcdb);
                if (n > 0)
                {
                    //更新成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = id;
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
            }
            finally {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }

        public void CopyOption(string SKU, string FromSKU, OleExec DB)
        {
            int n = 0;
            try
            {
                DateTime DT = DateTime.Now;
                C_SKU sku = DB.ORM.Queryable<C_SKU>().Where((s) => s.SKUNO == SKU).First();
                C_SKU Fsku = DB.ORM.Queryable<C_SKU>().Where((s) => s.SKUNO == FromSKU).First();
                //Copy Sku detail
                List<C_SKU_DETAIL> skudetail = DB.ORM.Queryable<C_SKU_DETAIL>().Where((t) => t.SKUNO == Fsku.SKUNO).ToList();
                T_C_SKU_DETAIL tcsd = new T_C_SKU_DETAIL(DB, DB_TYPE_ENUM.Oracle);
                for (int i = 0; i < skudetail.Count; i++)
                {
                    skudetail[i].ID = tcsd.GetNewID(BU, DB);
                    skudetail[i].SKUNO = sku.SKUNO;
                    skudetail[i].EDIT_TIME = DT;
                    skudetail[i].EDIT_EMP = LoginUser.EMP_NO;
                }
                if (skudetail.Count > 0)
                {
                    n = DB.ORM.Insertable<C_SKU_DETAIL>(skudetail).ExecuteCommand();
                }

                //Copy Route;
                List<R_SKU_ROUTE> routes = DB.ORM.Queryable<R_SKU_ROUTE>().Where((t) => t.SKU_ID == Fsku.ID).ToList();
                T_R_SKU_ROUTE trsr = new T_R_SKU_ROUTE(DB, DB_TYPE_ENUM.Oracle);
                for (int i = 0; i < routes.Count; i++)
                {
                    routes[i].ID = trsr.GetNewID(BU, DB);
                    routes[i].SKU_ID = sku.ID;
                    routes[i].EDIT_TIME = DT;
                    routes[i].EDIT_EMP = LoginUser.EMP_NO;
                }
                if (routes.Count > 0)
                {
                    n = DB.ORM.Insertable<R_SKU_ROUTE>(routes).ExecuteCommand();
                }
                //Copy point
                List<C_SAP_STATION_MAP> points = DB.ORM.Queryable<C_SAP_STATION_MAP>().Where((t) => t.SKUNO == FromSKU).ToList();
                T_C_SAP_STATION_MAP tcssm = new T_C_SAP_STATION_MAP(DB, DB_TYPE_ENUM.Oracle);
                for (int i = 0; i < points.Count; i++)
                {
                    points[i].ID = tcssm.GetNewID(BU, DB);
                    points[i].SKUNO = SKU;
                    points[i].EDIT_TIME = DT;
                    points[i].EDIT_EMP = LoginUser.EMP_NO;
                }
                if (points.Count > 0)
                {
                    n = DB.ORM.Insertable<C_SAP_STATION_MAP>(points).ExecuteCommand();
                }

                //Copy packing
                List<C_PACKING> packings = DB.ORM.Queryable<C_PACKING>().Where((p) => p.SKUNO == FromSKU).ToList();
                T_C_PACKING tcp = new T_C_PACKING(DB, DB_TYPE_ENUM.Oracle);
                for (int i = 0; i < packings.Count; i++)
                {
                    packings[i].ID = tcp.GetNewID(BU, DB);
                    packings[i].SKUNO = SKU;
                    packings[i].EDIT_TIME = DT;
                    packings[i].EDIT_EMP = LoginUser.EMP_NO;
                }
                if (packings.Count > 0)
                {
                    n = DB.ORM.Insertable<C_PACKING>(packings).ExecuteCommand();
                }

                //Copy label
                List<C_SKU_Label> labels = DB.ORM.Queryable<C_SKU_Label>().Where((l) => l.SKUNO == FromSKU).ToList();
                T_C_SKU_Label tcsl = new T_C_SKU_Label(DB, DB_TYPE_ENUM.Oracle);
                for (int i = 0; i < labels.Count; i++)
                {
                    labels[i].ID = tcsl.GetNewID(BU, DB);
                    labels[i].SKUNO = SKU;
                    labels[i].EDIT_TIME = DT;
                    labels[i].EDIT_EMP = LoginUser.EMP_NO;
                }
                if (labels.Count > 0)
                {
                    n = DB.ORM.Insertable<C_SKU_Label>(labels).ExecuteCommand();
                }

                //Copy AQL
                List<C_SKU_AQL> aqls = DB.ORM.Queryable<C_SKU_AQL>().Where((l) => l.SKUNO == FromSKU).ToList();
                T_C_SKU_AQL tcsa = new T_C_SKU_AQL(DB, DB_TYPE_ENUM.Oracle);
                for (int i = 0; i < aqls.Count; i++)
                {
                    aqls[i].ID = tcsa.GetNewID(BU, DB);
                    aqls[i].SKUNO = SKU;
                    aqls[i].EDIT_TIME = DT;
                    aqls[i].EDIT_EMP = LoginUser.EMP_NO;
                }
                if (aqls.Count > 0)
                {
                    n = DB.ORM.Insertable<C_SKU_AQL>(aqls).ExecuteCommand();
                }
            }
            catch (Exception)
            {
                throw;
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
                result = Table.UpdateSku(BU, Sku, "DELETE", GetDBDateTime(), out SkuId, sfcdb);

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
                result = Table.UpdateSku(BU, Sku, "DELETE", GetDBDateTime(), out strSkuId, sfcdb);

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

        public void GetAllSkuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> skulist = new List<string>();

        }

        public void HWDGetLast100DaySkuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;            
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBTYPE);
                System.Data.DataTable dt = t_c_sku.HWDGetLast100DaySkuList(sfcdb);
                if (dt.Rows.Count == 0)
                {                   
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {                   
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count.ToString());
                    StationReturn.Data = dt;
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

        public void GetWeightConfigBySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var skuno = Data["Skuno"].ToString();
                var res = sfcdb.ORM.Queryable<C_WEIGHT>().Where(t => t.SKUNO == skuno).ToList();
                if (res.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = res;
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void RecordWeightConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var obj = Data.ToObject<C_WEIGHT>();
                var res = 0;
                if (obj.ID != null&& obj.ID!="")
                {
                    obj.EDITTIME = DateTime.Now;
                    obj.EDITBY = this.LoginUser.EMP_NO;
                    res = sfcdb.ORM.Updateable(obj).ExecuteCommand();
                }
                else
                {

                    var objexists = sfcdb.ORM.Queryable<C_WEIGHT>().Where(t => t.SKUNO == obj.SKUNO && t.TYPE == obj.TYPE && t.STATION==obj.STATION && (
                                                                                   t.TYPE == "SKUNO" ||
                                                                                   ("MPN,PACKAGE".Contains(t.TYPE) &&
                                                                                    t.PARTNO == obj.PARTNO &&
                                                                                    t.MPN == obj.MPN))
                    ).Any();
                    if (objexists)
                        throw  new Exception($@"The same data already exists!");
                    obj.ID = MesDbBase.GetNewID(sfcdb.ORM, this.BU, "C_WEIGHT");
                    obj.CREATETIME = DateTime.Now;
                    obj.CREATEBY = this.LoginUser.EMP_NO;
                    res = sfcdb.ORM.Insertable(obj).ExecuteCommand();
                }
                if (res > 0)
                {
                    //添加成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(res);
                }
                else
                {
                    //沒有添加任何數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.MessagePara.Add(res);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        /// <summary>
        /// 獲取所有機種配置的所有Label信息
        /// </summary>
        /// <param name="StationReturn"></param>
        public void GetAllSkuLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var skuLabelList = sfcdb.ORM.Queryable<C_SKU, C_SKU_Label, R_Label>((CS, CSL, RL) => CS.SKUNO == CSL.SKUNO && CSL.LABELNAME == RL.LABELNAME)
                    .OrderBy((CS, CSL, RL)=>CS.SKUNO, SqlSugar.OrderByType.Asc)
                    .OrderBy((CS, CSL, RL) => CSL.STATION, SqlSugar.OrderByType.Asc).OrderBy((CS, CSL, RL) => CSL.SEQ, SqlSugar.OrderByType.Asc)
                    .Select((CS, CSL, RL) => new { CSL.ID, CSL.SKUNO, CSL.STATION, CSL.SEQ, CSL.QTY, CSL.LABELNAME, CSL.LABELTYPE, RL.R_FILE_NAME, CSL.EDIT_EMP, CSL.EDIT_TIME }).ToList();
                if (skuLabelList.Count() == 0)
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
                    StationReturn.MessagePara.Add(skuLabelList.Count().ToString());
                    StationReturn.Data = skuLabelList;
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

        public void GetSkuDetailConfigList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<object> ret = new List<object>();
                var config = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => SqlSugar.SqlFunc.StartsWith(t.SKUNO, "CONFIG")).ToList();
                for (int i = 0; i < config.Count; i++)
                {
                    var name = config[i].SKUNO.Remove(0, 7);
                    var value = config[i].VALUE != null ? config[i].VALUE.Split(new char[] { ':' } ): new string[] { "",""};
                    var exd = config[i].EXTEND != null ? config[i].EXTEND.Split(new char[] { ':' }) : new string[] { "", "" };
                    var bt = config[i].BASETEMPLATE != null ? config[i].BASETEMPLATE.Split(new char[] { ':' }) : new string[] { "", "" };
                    var stn = config[i].STATION_NAME != null ? config[i].STATION_NAME.Split(new char[] { ':' }) : new string[] { "", "" };

                    var t = new
                    {
                        NAME = name,
                        ID = config[i].ID,
                        CATEGORY = config[i].CATEGORY,
                        CATEGORY_NAME = config[i].CATEGORY_NAME,
                        VALUE = new { value = value.Length>1?value[1]:null, placeholder = value.Length>0? value[0]:null },
                        EXTEND = new { value = exd.Length > 1 ? exd[1] : null, placeholder = exd.Length > 0 ? exd[0] : null },
                        BASETEMPLATE = new { value = bt.Length > 1 ? bt[1] : null, placeholder = bt.Length > 0 ? bt[0] : null },
                        STATION_NAME = new { value = stn.Length > 1 ? stn[1] : null, placeholder = stn.Length > 0 ? stn[0] : null },
                    };
                    ret.Add(t);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                //StationReturn.MessageCode = "MES00000034";
                StationReturn.Data = ret;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }


        }


    }
}
