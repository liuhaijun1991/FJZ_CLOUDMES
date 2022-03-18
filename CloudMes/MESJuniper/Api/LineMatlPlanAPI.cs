using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.Json;
using MESJuniper.Model;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject;
using System.Data;

namespace MESJuniper.Api
{
    public class LineMatlPlanAPI : MesAPIBase
    {
        protected APIInfo _SaveLinePlan = new APIInfo
        {
            FunctionName = "SaveLinePlan",
            Description = "SaveLinePlan",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "Stock_Location", InputType = "string" },
                new APIInputInfo() { InputName = "DATE", InputType = "datetime" },
                new APIInputInfo() { InputName = "JsonData", InputType = "string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _GetLinePlan = new APIInfo
        {
            FunctionName = "GetLinePlan",
            Description = "GetLinePlan",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "Stock_Location", InputType = "string" },
                new APIInputInfo() { InputName = "DATE", InputType = "datetime" },
                //new APIInputInfo() { InputName = "JsonData", InputType = "string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetGroupName = new APIInfo()
        {
            FunctionName = "GetGroupName",
            Description = "GetGroupName",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetGroupLine = new APIInfo()
        {
            FunctionName = "GetGroupLine",
            Description = "GetGroupLine",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "GroupName", InputType = "string" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetPlanWO = new APIInfo()
        {
            FunctionName = "GetPlanWO ",
            Description = "GetPlanWO ",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo _MatlIssue = new APIInfo()
        {
            FunctionName = "MatlIssue",
            Description = "MatlIssue",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "JsonData", InputType = "string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetPlanAndStock = new APIInfo
        {
            FunctionName = "GetPlanAndStock",
            Description = "GetPlanAndStock",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "Stock_Location", InputType = "string" },
                new APIInputInfo() { InputName = "DATE", InputType = "datetime" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FMatlCheckoutChecker = new APIInfo
        {
            FunctionName = "MatlCheckoutChecker",
            Description = "MatlCheckoutChecker",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="STOCK_LOCATION",InputType="string" },
                new APIInputInfo(){ InputName="DATE",InputType="string" },
                new APIInputInfo(){ InputName="TIME",InputType="string" },
                new APIInputInfo(){ InputName="LINE",InputType="string" },
                new APIInputInfo() { InputName = "PN", InputType = "string" },
                new APIInputInfo {InputName ="QTY",InputType="string"}
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FMatlReturnChecker = new APIInfo
        {
            FunctionName = "MatlReturnChecker",
            Description = "MatlReturnChecker",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "PN", InputType = "string" },
                new APIInputInfo(){ InputName="STOCK_LOCATION",InputType="string" },
                new APIInputInfo {InputName ="QTY",InputType="string"}
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FReservationChecker = new APIInfo
        {
            FunctionName = "ReservationChecker",
            Description = "ReservationChecker",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="WO",InputType="string" },
                new APIInputInfo() { InputName = "PN", InputType = "string" }               
               
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetCheckoutDetail = new APIInfo
        {
            FunctionName = "GetCheckoutDetail",
            Description = "GetCheckoutDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="STOCK_LOCATION",InputType="string" },
                new APIInputInfo() { InputName = "DATE", InputType = "string" }

            },
            Permissions = new List<MESPermission>()
        };
        
        public LineMatlPlanAPI()
        {
            this.Apis.Add(_SaveLinePlan.FunctionName, _SaveLinePlan);
            this.Apis.Add(_GetLinePlan.FunctionName, _GetLinePlan);
            this.Apis.Add(FGetGroupName.FunctionName, FGetGroupName);
            this.Apis.Add(FGetGroupLine.FunctionName, FGetGroupLine);
            this.Apis.Add(FGetPlanWO.FunctionName, FGetPlanWO);
            this.Apis.Add(_MatlIssue.FunctionName, _MatlIssue);
            this.Apis.Add(FGetPlanAndStock.FunctionName, FGetPlanAndStock);
            this.Apis.Add(FMatlCheckoutChecker.FunctionName, FMatlCheckoutChecker);
            this.Apis.Add(FMatlReturnChecker.FunctionName, FMatlReturnChecker);
            this.Apis.Add(FReservationChecker.FunctionName, FReservationChecker);
            this.Apis.Add(FGetCheckoutDetail.FunctionName, FGetCheckoutDetail);
        }

        public void MatlIssue(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();           
            try
            {                
                var data = Data["JsonData"].ToObject<R_JNP_LINE_STOCK_DETAIL>();
                var stock = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK>().Where(t => t.STOCK_LOCATION == data.STOCK_LOCATION && t.PN == data.PN).First();                
                if (stock == null)
                {
                    stock = new R_JNP_LINE_STOCK()
                    {
                        ID = MesDbBase.GetNewID(SFCDB.ORM, this.BU, "R_JNP_LINE_STOCK"),
                        PN = data.PN,
                        QTY = 0,
                        STOCK_LOCATION = data.STOCK_LOCATION,
                            
                    };
                    SFCDB.ORM.Insertable(stock).ExecuteCommand();
                }
                stock.QTY = stock.QTY + data.QTY;
                stock.EDIT_EMP = LoginUser.EMP_NO;
                stock.EDIT_TIME = DateTime.Now;
                SFCDB.ORM.Updateable(stock).Where(r => r.ID == stock.ID).ExecuteCommand();

                data.ID = MesDbBase.GetNewID(SFCDB.ORM, this.BU, "R_JNP_LINE_STOCK_DETAIL");
                data.EDIT_EMP = LoginUser.EMP_NO;
                data.EDIT_TIME = DateTime.Now;
                SFCDB.ORM.Insertable(data).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetLinePlan(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();            
            try
            {                
                var date = Data["DATE"].ToObject<DateTime>();
                var stockLocation = Data["Stock_Location"].ToObject<string>();
                string dateStr = date.ToString("yyyyMMdd");
                var name = stockLocation + "_" + dateStr;
                var type = "LineMatlPlan";
                //var extdata = JsonSave.GetFromDB<List<LineMatlPlanAPI>>(name, type, SFCDB);
                var  ret = JsonSave.GetFromDB<List<LINE_MATL_PLAN>>( name, type,  SFCDB);
                if (ret != null)
                {
                    List<object> oList = new List<object>();
                    foreach (var item in ret)
                    {
                        bool bCheckout = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK_DETAIL>()
                            .Any(r => r.OPTION_TYPE == 0 && r.STOCK_LOCATION == stockLocation && r.PN == item.PN && r.DETAIL2 == item.Time && r.DETAIL3 == item.LINE && r.DETAIL4 == dateStr);
                        oList.Add(
                            new
                            {
                                LINE = item.LINE,
                                WO = item.WO,
                                QTY = item.QTY,
                                PN = item.PN,
                                Time = item.Time,
                                Checkout = bCheckout
                            });
                    }
                    StationReturn.Data = oList;
                }
                else
                {
                    StationReturn.Data = new List<LINE_MATL_PLAN>(); 
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                StationReturn.Data = new List<LINE_MATL_PLAN>();
            }
            finally
            {
                
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void SaveLinePlan(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var data = Data["JsonData"].ToObject<List< LINE_MATL_PLAN>>();
                var date = Data["DATE"].ToObject<DateTime>();
                var stockLocation = Data["Stock_Location"].ToObject<string>();

                var name = stockLocation + "_"+ date.ToString("yyyyMMdd");
                var type = "LineMatlPlan";
                var groupByTime = data.GroupBy(r => r.Time);
                foreach (var item in groupByTime)
                {
                    int totalQty = 0;
                    item.ToList().ForEach(r => {
                        totalQty += Convert.ToInt32(r.QTY);
                    });
                    if (totalQty > 1200 && item.Select(r => r).ToList().Count() >1)
                    {
                        throw new Exception($@"The total Qty of {item.Key}'s is over 1,200");
                    }
                }                
                //var extdata = JsonSave.GetFromDB<List<LineMatlPlanAPI>>(name, type, SFCDB);
                JsonSave.SaveToDB(data, name, type, LoginUser.EMP_NO, SFCDB, BU, true);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetGroupName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> groupName = new List<string>();
            try
            {
                T_R_FUNCTION_CONTROL funControl = new T_R_FUNCTION_CONTROL(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                groupName = funControl.GetListByFcv("STOCK_GROUP", "STOCK_GROUP", SFCDB).Select(r => r.VALUE).Distinct().ToList();                
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                StationReturn.Data = groupName;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetGroupLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> groupLine = new List<string>();
            try
            {
                if(Data["GroupName"]==null)
                {
                    throw new Exception("Please input GroupName.");
                }
                string groupName = Data["GroupName"].ToString();
                T_R_FUNCTION_CONTROL funControl = new T_R_FUNCTION_CONTROL(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);                
                groupLine = funControl.GetListByFcv("STOCK_GROUP", "STOCK_GROUP", groupName, SFCDB).Select(r => r.EXTVAL).Distinct().ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                StationReturn.Data = groupLine;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetPlanWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<R_WO_BASE> woList = new List<R_WO_BASE>();
            try
            {
                woList = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.CLOSED_FLAG == "0").ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                StationReturn.Data = woList;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetPlanAndStock(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();            
            try
            {                
                var DATE = Data["DATE"].ToObject<DateTime>();
                string dateStr = DATE.ToString("yyyyMMdd");
                var stockLocation = Data["Stock_Location"].ToObject<string>();
                var name = stockLocation + "_" + dateStr;
                var type = "LineMatlPlan";
                List<LINE_MATL_PLAN> list = JsonSave.GetFromDB<List<LINE_MATL_PLAN>>(name, type, SFCDB);
                if(list==null)
                {
                    throw new Exception($@"{stockLocation},{dateStr},No Plan.");
                }
                if (list.Count > 0)
                {
                    List<object> oList = new List<object>();
                    var pnList = list.Select(r => new { r.Time, r.PN, r.LINE }).Distinct().ToList();
                    foreach (var l in pnList)
                    {
                        //var sumCheckOut = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK_DETAIL>()
                        //    .Where(r => r.OPTION_TYPE == 0 && r.STOCK_LOCATION == Stock_Location && r.PN == l.PN && r.DETAIL1 == l.WO && r.DETAIL2 == l.Time && r.DETAIL3 == l.LINE)
                        //    .Select(r => SqlSugar.SqlFunc.AggregateSum(r.QTY)).ToList().FirstOrDefault();
                        var sumCheckOut = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK_DETAIL>()
                            .Where(r => r.OPTION_TYPE == 0 && r.STOCK_LOCATION == stockLocation && r.PN == l.PN && r.DETAIL2 == l.Time && r.DETAIL3 == l.LINE && r.DETAIL4== dateStr)
                            .Select(r => SqlSugar.SqlFunc.AggregateSum(r.QTY)).ToList().FirstOrDefault();
                        //var stockQty = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK>().Where(r => r.PN == l.PN && r.STOCK_LOCATION== stockLocation).Select(r => r.QTY).ToList().FirstOrDefault();
                        string Sqlstock = $@"select STOCK_LOCATION,SUM(QTY) QTY from R_JNP_LINE_STOCK where STOCK_LOCATION='{stockLocation}'GROUP BY STOCK_LOCATION";
                        DataTable dt = SFCDB.RunSelect(Sqlstock).Tables[0];

                        double tQty = list.Where(r => r.LINE == l.LINE && r.PN == l.PN && r.Time == l.Time).Sum(r => Convert.ToDouble(r.QTY));
                        double dQty = sumCheckOut == null ? 0 : (double)sumCheckOut;
                        string wQty = dt.Rows[0]["QTY"].ToString() == null ? "0" : dt.Rows[0]["QTY"].ToString();
                        //double wQty = dt.Rows[0]["QTY"] == null ? 0 : (double)dt.Rows[0]["QTY"];


                        oList.Add(
                            new {
                                Time=l.Time,
                                LINE = l.LINE,
                                QTY = tQty, 
                                PN = l.PN, 
                                DeliveryQTY = dQty, 
                                WipQTY = wQty 
                            });
                    }
                    StationReturn.Data = oList;
                }               
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {                
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    
        public void MatlCheckoutChecker(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            
            try
            {
                var stockLocation = Data["STOCK_LOCATION"].ToObject<string>();
                var date = Data["DATE"].ToObject<DateTime>();
                var time= Data["TIME"].ToObject<string>();
                var line= Data["LINE"].ToObject<string>();
                var pn = Data["PN"].ToObject<string>();                
                var checkoutQty = Data["CHECKOUT_QTY"].ToObject<double>();
                var requestQty = Data["REQUEST_QTY"].ToObject<double>(); 
                if (string.IsNullOrEmpty(stockLocation))
                {
                    throw new Exception("Please input STOCK_LOCATION");
                }               
                if (string.IsNullOrEmpty(pn))
                {
                    throw new Exception("Please input PN");
                }                
                if (checkoutQty == 0)
                {
                    throw new Exception($"Checkout qty is {checkoutQty}");
                }
                string dateStr= date.ToString("yyyyMMdd");
                var name = stockLocation + "_" + dateStr;
                var type = "LineMatlPlan";
                List<LINE_MATL_PLAN> list = JsonSave.GetFromDB<List<LINE_MATL_PLAN>>(name, type, SFCDB);

                var checkList = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK_DETAIL>()
                    .Where(r => r.STOCK_LOCATION == stockLocation && r.PN == pn && r.OPTION_TYPE == 0 && r.DETAIL2 == time && r.DETAIL3 == line && r.DETAIL4 == dateStr)
                    .ToList();
                var checkoutTotal = checkList.Select(r => r.QTY).Sum()+ checkoutQty;                
                if (checkoutTotal > requestQty)
                {
                    throw new Exception($"Total checkout qty [{checkoutTotal}] more then request qty [{requestQty}]");
                }
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {               
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void MatlReturnChecker(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {
                var pn = Data["PN"].ToObject<string>();
                var stockLocation = Data["STOCK_LOCATION"].ToObject<string>();
                var qty = Data["QTY"].ToObject<double>();
                if (string.IsNullOrEmpty(pn))
                {
                    throw new Exception("Please input PN");
                }
                if (string.IsNullOrEmpty(stockLocation))
                {
                    throw new Exception("Please input STOCK_LOCATION");
                }
                if (qty == 0)
                {
                    throw new Exception($"Return qty is {qty}");
                }
                var stockQty = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK>()
                    .Where(r => r.PN == pn && r.STOCK_LOCATION == stockLocation).Select(r => SqlSugar.SqlFunc.AggregateSum(r.QTY))
                    .ToList().FirstOrDefault();
                if (stockQty == null)
                {
                    throw new Exception($"{pn} have not been check out to {stockLocation}");
                }
                if (qty > stockQty)
                {
                    throw new Exception($"Return qty [{qty}] more then stock qty [{stockQty}]");
                }
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void ReservationChecker(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn) 
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {
                var woList = Data["WO"].ToArray();
                if (woList.Length == 0)
                {
                    throw new Exception("Please input WO");
                }
                List<object> list = new List<object>();
                foreach (var item in woList)
                {
                    string workorder = item.ToString();
                    var woObj = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == workorder).ToList().FirstOrDefault();
                    if (woObj == null)
                    {
                        throw new Exception($"The WO [{workorder}] is not release in MES.");
                    }
                    var autoKpList = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(workorder, "JuniperAutoKPConfig", SFCDB);
                    if (autoKpList == null || autoKpList.Count == 0)
                    {
                        throw new Exception("Get PN from Auto KP error.");
                    }
                    var pn = autoKpList.Select(r => r.PN_7XX).First();
                    list.Add(new { WO = workorder, QTY = woObj.WORKORDER_QTY, PN = pn });
                }
                StationReturn.Data = list;
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    
        public void GetCheckoutDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var stockLocation = Data["STOCK_LOCATION"].ToObject<string>();
                var date = Data["DATE"].ToObject<DateTime>();                
                if (string.IsNullOrEmpty(stockLocation))
                {
                    throw new Exception("Please input STOCK_LOCATION");
                }                
                string dateStr = date.ToString("yyyyMMdd");
               
                var checkList = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK_DETAIL>()
                    .Where(r => r.STOCK_LOCATION == stockLocation && r.OPTION_TYPE == 0 && r.DETAIL4 == dateStr)
                    .ToList();
                
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = checkList;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetWipSummaryDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var stockLocation = Data["STOCK_LOCATION"].ToObject<string>();
                var date = Data["DATE"].ToObject<DateTime>();
                if (string.IsNullOrEmpty(stockLocation))
                {
                    throw new Exception("Please input STOCK_LOCATION");
                }
                string dateStr = date.ToString("yyyyMMdd");

                var checkList = SFCDB.ORM.Queryable<R_JNP_LINE_STOCK>()
                    .Where(r => r.STOCK_LOCATION == stockLocation && r.QTY>0)
                    .ToList();

                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = checkList;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
