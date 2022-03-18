using MESDataObject;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESStation.Config.Vertiv
{
    public class CStockConfig : MesAPIBase
    {
        protected APIInfo FGetAllStock = new APIInfo()
        {
            FunctionName = "GetAllStock",
            Description = "獲取所有儲位信息",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetStockByValue = new APIInfo()
        {
            FunctionName = "GetStockByValue",
            Description = "通過輸入值查找（不區分大小寫）獲取儲位信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SearchValue" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetStockByValues = new APIInfo()
        {
            FunctionName = "GetStockByValues",
            Description = "通過輸入值查找（不區分大小寫）獲取儲位信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Section" }, new APIInputInfo() { InputName = "Location" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteStockById = new APIInfo()
        {
            FunctionName = "DeleteStockById",
            Description = "通過Id刪除儲位信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Ids" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUploadStockConfig = new APIInfo
        {
            FunctionName = "UploadStockConfig",
            Description = "上傳儲位信息",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewStockConfig = new APIInfo()
        {
            FunctionName = "AddNewStockConfig",
            Description = "添加新的儲位信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "Section" },
                new APIInputInfo() { InputName = "Location" },
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FSelectList = new APIInfo()
        {
            FunctionName = "SelectList",
            Description = "獲取R_Stock_Record",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "Location", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Value", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "SkuNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Workorder", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Station", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "FromDate", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "ToDate", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateList = new APIInfo()
        {
            FunctionName = "UpdateList",
            Description = "修改R_Stock_Record儲位",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Location", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        public CStockConfig()
        {
            this.Apis.Add(FGetAllStock.FunctionName, FGetAllStock);
            this.Apis.Add(FGetStockByValue.FunctionName, FGetStockByValue);
            this.Apis.Add(FGetStockByValues.FunctionName, FGetStockByValues);
            this.Apis.Add(FDeleteStockById.FunctionName, FDeleteStockById);
            this.Apis.Add(FUploadStockConfig.FunctionName, FUploadStockConfig);
            this.Apis.Add(FAddNewStockConfig.FunctionName, FAddNewStockConfig);
            this.Apis.Add(FSelectList.FunctionName, FSelectList);
            this.Apis.Add(FUpdateList.FunctionName, FUpdateList);
        }
        public void GetAllStock(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_STOCK_CONFIG> stockList = new List<C_STOCK_CONFIG>();
                stockList = _STOCK.GetAllStock(SFCDB);
                StationReturn.Data = stockList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void GetStockByValue(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_STOCK_CONFIG> stockList = new List<C_STOCK_CONFIG>();
                string searchValue = Data["SearchValue"].ToString().ToUpper();
                stockList = _STOCK.GetStockByValue(searchValue, SFCDB);
                if (stockList != null)
                {
                    StationReturn.Data = stockList;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(searchValue);
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void GetStockByValues(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_STOCK_CONFIG> stockList = new List<C_STOCK_CONFIG>();
                string section = Data["Section"].ToString().ToUpper();
                string location = Data["Location"].ToString().ToUpper();
                stockList = _STOCK.GetStockByValue(section, location, SFCDB);
                if (stockList != null)
                {
                    StationReturn.Data = stockList;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(section + "|" + location);
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void DeleteStockById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string strIds = Data["Ids"].ToString().Trim();
                string[] arrIds = strIds.Split(',');
                for (int i = 0; i < arrIds.Length; i++)
                {
                    T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    C_STOCK_CONFIG stock = new C_STOCK_CONFIG();
                    stock = _STOCK.GetStockByid(arrIds[i], SFCDB);
                    if (stock == null)
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000007";
                        StationReturn.MessagePara.Add(strIds);

                    }
                    else
                    {
                        int result = _STOCK.DeleteStockById(stock.ID, SFCDB);
                        if (result <= 0)
                        {
                            StationReturn.Data = "";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000023";
                            StationReturn.MessagePara.Add(stock.LOCATION);
                        }
                    }
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void UploadStockConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string dataList = Data["DataList"].ToString();                
                Newtonsoft.Json.Linq.JArray dataArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(dataList);
                for (int i = 0; i < dataArray.Count; i++)
                {
                    string section = dataArray[i]["SECTION"].ToString();
                    string location = dataArray[i]["LOCATION"].ToString();
                    T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(SFCDB, DBTYPE);
                    Row_C_STOCK_CONFIG row_Stock = null;
                    List<C_STOCK_CONFIG> stockList = _STOCK.GetStockByValue(section, location, SFCDB);
                    if (stockList != null)
                    {
                        throw new Exception("附件中 SECTION：[" + section + "]， LOCATION：[" + location + "]已存在數據庫中！");
                    }

                    #region 寫入 C_STOCK_CONFIG 表
                    row_Stock = (Row_C_STOCK_CONFIG)_STOCK.NewRow();
                    row_Stock.ID = _STOCK.GetNewID(BU, SFCDB);
                    row_Stock.SECTION = section;
                    row_Stock.LOCATION = location;
                    row_Stock.EDIT_EMP = LoginUser.EMP_NO;
                    row_Stock.EDIT_TIME = _STOCK.GetDBDateTime(SFCDB);
                    SFCDB.ExecSQL(row_Stock.GetInsertString(DBTYPE));
                    #endregion
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MSGCODE20210814165555";
                SFCDB.CommitTrain();
            }
            catch (Exception e)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void AddNewStockConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string section = Data["Section"].ToString().Trim();
                string location = Data["Location"].ToString().Trim();
                T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_STOCK_CONFIG> stockList = _STOCK.GetStockByValue(section, location, SFCDB);
                if (stockList == null)
                {
                    C_STOCK_CONFIG stock = new C_STOCK_CONFIG
                    {
                        ID = _STOCK.GetNewID(BU, SFCDB),
                        SECTION = section,
                        LOCATION = location,
                        EDIT_EMP = LoginUser.EMP_NO,
                        EDIT_TIME = GetDBDateTime()
                    };
                    int res = _STOCK.AddNewStockConfig(stock, SFCDB);
                    if (res > 0)
                    {
                        StationReturn.Data = stock;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.MessagePara.Add(section + "|" + location);
                    }
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(section + "|" + location);
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void SelectList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string location = Data["Location"].ToString().Trim();
                string value = Data["Value"].ToString().Trim();
                string skuNo = Data["SkuNo"].ToString().Trim();
                string wo = Data["Workorder"].ToString().Trim();
                string station = Data["Station"].ToString().Trim();
                string fromDate = Data["FromDate"].ToString().Trim();
                string toDate = Data["ToDate"].ToString().Trim();

                string sqlTime = $@"t.EDIT_TIME >= to_date('{fromDate}', 'yyyy/mm/dd') and t.EDIT_TIME < to_date('{toDate}', 'yyyy/mm/dd') + 1";
                DataTable dt = SFCDB.ORM.Queryable<R_STOCK_RECORD>("t").Where(t => t.STATUS == "1")
                    .WhereIF(!string.IsNullOrEmpty(location), t => t.LOCATION == location)
                    .WhereIF(!string.IsNullOrEmpty(value), t => t.VALUE == value)
                    .WhereIF(!string.IsNullOrEmpty(skuNo), t => t.SKUNO == skuNo)
                    .WhereIF(!string.IsNullOrEmpty(wo), t => t.WORKORDERNO == wo)
                    .WhereIF(!string.IsNullOrEmpty(station), t => t.STATION == station)
                    .Where(sqlTime)
                    .Select(t => new { t.ID, t.LOCATION, t.VALUE, t.SKUNO, t.WORKORDERNO, t.STATION, t.STATUS, t.EDIT_TIME, t.EDIT_EMP })
                    .OrderBy(t => t.EDIT_TIME)
                    .Take(500)
                    .ToDataTable();
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }

                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void UpdateList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string id = Data["ID"].ToString().Trim();
                string location = Data["Location"].ToString().Trim();
                T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(SFCDB, DB_TYPE_ENUM.Oracle);
                List<C_STOCK_CONFIG> stockList = _STOCK.GetStockByValue("WHS", location, SFCDB);
                if (stockList == null)
                {
                    //throw new Exception(" 掃描儲位 " + location + " 無效!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164041", new string[] { location }));
                }

                var res = SFCDB.ORM.Updateable<R_STOCK_RECORD>().SetColumns(t => new R_STOCK_RECORD()
                {
                    LOCATION = location,
                    EDIT_TIME = DateTime.Now,
                    EDIT_EMP = LoginUser.EMP_NO
                }).Where(t => t.ID == id).ExecuteCommand();
                if (res > 0)
                {
                    StationReturn.MessageCode = "MES00000003";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.MessagePara = new List<object>() { "" };
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }                
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    }
}
