using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.Common;
using MESPubLab.MESStation;
using MESStation.Interface.Vertiv;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Config.Vertiv
{
    public class VertivPOApi : MesAPIBase
    {
        protected APIInfo FGetAllOrderAction = new APIInfo()
        {
            FunctionName = "GetAllOrderAction",
            Description = "Get All Order Action",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllOrderStatus = new APIInfo()
        {
            FunctionName = "GetAllOrderStatus",
            Description = "Get All Order Status",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllShipmentAction = new APIInfo()
        {
            FunctionName = "GetAllShipmentAction",
            Description = "Get All Shipment Action",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetOrderByActionAndStatus = new APIInfo()
        {
            FunctionName = "GetOrderByActionAndStatus",
            Description = "Get VT Order By Action Status",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="Action",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="Status",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="Valid",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="PO",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="LINE",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetVTOrderDetail = new APIInfo()
        {
            FunctionName = "GetVTOrderDetail",
            Description = "Get VT Order Detail",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FInsertOrUpdateOrder = new APIInfo()
        {
            FunctionName = "InsertOrUpdateOrder",
            Description = "Insert Or Update Order",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAcceptOrder = new APIInfo()
        {
            FunctionName = "AcceptOrder",
            Description = "Accept Order",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateOrder = new APIInfo()
        {
            FunctionName = "UpdateOrder",
            Description = "Update Order",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FRejectOrder = new APIInfo()
        {
            FunctionName = "RejectOrder",
            Description = "Reject Order",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCancelOrder = new APIInfo()
        {
            FunctionName = "CancelOrder",
            Description = "Cancel Order",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCloseOrder = new APIInfo()
        {
            FunctionName = "CloseOrder",
            Description = "Close Order",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetVTOneOrderByID = new APIInfo()
        {
            FunctionName = "GetVTOneOrderByID",
            Description = "GetVTOneOrder",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="OrderId",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="LineId",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="PromisseId",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FEditPoForIT = new APIInfo()
        {
            FunctionName = "EditPoForIT",
            Description = "EditPoForIT",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="OrderId",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="OrderDetail",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetOrderByByPO = new APIInfo()
        {
            FunctionName = "GetOrderByByPO",
            Description = "GetOrderByByPO",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="PO",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetShipmentListByOrderId = new APIInfo()
        {
            FunctionName = "GetShipmentListByOrderId",
            Description = "GetShipmentListByOrderId",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="OrderId",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetShipmentById = new APIInfo()
        {
            FunctionName = "GetShipmentById",
            Description = "GetShipmentById",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDownloadByPO = new APIInfo()
        {
            FunctionName = "DownloadByPO",
            Description = "DownloadByPO",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="PoList",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdatePromiseData = new APIInfo()
        {
            FunctionName = "UpdatePromiseData",
            Description = "UpdatePromiseData",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="PromiseDate",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="PromisedShipmentDate",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FSplitPO = new APIInfo()
        {
            FunctionName = "SplitPO",
            Description = "SplitPO",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="FirstPromiseQty",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="FirstPromiseDate",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="FirstPromisedShipmentDate",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="SecondPromiseQty",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="SecondPromiseDate",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="SecondPromisedShipmentDate",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetForecastFileList = new APIInfo()
        {
            FunctionName = "GetForecastFileList",
            Description = "GetForecastFileList",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetCommitListOrderId = new APIInfo()
        {
            FunctionName = "GetCommitListOrderId",
            Description = "GetCommitListOrderId",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="OrderId",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCompareForecast = new APIInfo()
        {
            FunctionName = "CompareForecast",
            Description = "CompareForecast",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){
                    InputName="LotNoList",
                    InputType="string",
                    DefaultValue=""
                }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCompareAndDownloadForecast = new APIInfo()
        {
            FunctionName = "CompareAndDownloadForecast",
            Description = "CompareAndDownloadForecast",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){
                    InputName="LotNoList",
                    InputType="string",
                    DefaultValue=""
                }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCancelShipment = new APIInfo()
        {
            FunctionName = "CancelShipment",
            Description = "CancelShipment",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FSplitPOByList = new APIInfo() { 
            FunctionName= "SplitPOByList",
            Description= "SplitPOByList",
            Parameters=new List<APIInputInfo>() { 
                new APIInputInfo(){InputName="ID",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="SplitList",InputType="string",DefaultValue=""}
            },
            Permissions=new List<MESPermission>()
        };
        public VertivPOApi()
        {
            this.Apis.Add(FGetAllOrderAction.FunctionName, FGetAllOrderAction);
            this.Apis.Add(FGetAllOrderStatus.FunctionName, FGetAllOrderStatus);
            this.Apis.Add(FGetAllShipmentAction.FunctionName, FGetAllShipmentAction);
            this.Apis.Add(FGetOrderByActionAndStatus.FunctionName, FGetOrderByActionAndStatus);
            this.Apis.Add(FGetVTOrderDetail.FunctionName, FGetVTOrderDetail);
            this.Apis.Add(FInsertOrUpdateOrder.FunctionName, FInsertOrUpdateOrder);
            this.Apis.Add(FAcceptOrder.FunctionName, FAcceptOrder);
            this.Apis.Add(FUpdateOrder.FunctionName, FUpdateOrder);
            this.Apis.Add(FRejectOrder.FunctionName, FRejectOrder);
            this.Apis.Add(FCancelOrder.FunctionName, FCancelOrder);
            this.Apis.Add(FCloseOrder.FunctionName, FCloseOrder);
            this.Apis.Add(FGetVTOneOrderByID.FunctionName, FGetVTOneOrderByID);
            this.Apis.Add(FEditPoForIT.FunctionName, FEditPoForIT);
            this.Apis.Add(FGetShipmentListByOrderId.FunctionName, FGetShipmentListByOrderId);
            this.Apis.Add(FGetShipmentById.FunctionName, FGetShipmentById);
            this.Apis.Add(FDownloadByPO.FunctionName, FDownloadByPO);
            this.Apis.Add(FUpdatePromiseData.FunctionName, FUpdatePromiseData);
            this.Apis.Add(FSplitPO.FunctionName, FSplitPO);
            this.Apis.Add(FGetForecastFileList.FunctionName, FGetForecastFileList);
            this.Apis.Add(FGetCommitListOrderId.FunctionName, FGetCommitListOrderId);
            this.Apis.Add(FCompareForecast.FunctionName, FCompareForecast);
            this.Apis.Add(FCompareAndDownloadForecast.FunctionName, FCompareAndDownloadForecast);
            this.Apis.Add(FCancelShipment.FunctionName, FCancelShipment);
        }

        public void GetAllOrderAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            List<string> list = Enum.GetNames(typeof(OrderAction)).ToList();
            list.Add("All");
            StationReturn.Data = list;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        public void GetAllOrderValid(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            List<string> list = new List<string>();
            list.Add("All");
            list.AddRange(Enum.GetNames(typeof(OrderValidFlag)).ToList());
            StationReturn.Data = list;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        public void GetAllOrderStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            List<string> list = new List<string>();
            list.Add("All");
            list.AddRange(Enum.GetNames(typeof(OrderStatus)).ToList());
            StationReturn.Data = list;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        public void GetForecastValid(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            List<string> list = Enum.GetNames(typeof(ForecastValid)).ToList();
            list.Sort((x, y) => -x.CompareTo(y));
            list.AddRange(new List<string>() { "All" });
            StationReturn.Data = list;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        public void GetAllShipmentAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            List<string> list = Enum.GetNames(typeof(ShipmentAction)).ToList();
            list.Add("All");
            StationReturn.Data = list;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
        public void GetOrderByActionAndStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string action = Data["Action"] == null ? "All" : Data["Action"].ToString().Trim();
                string valid = Data["Valid"] == null ? "All" : Data["Valid"].ToString().Trim();
                string status = Data["Status"] == null ? "All" : Data["Status"].ToString().Trim();
                string po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                string line = Data["POLine"] == null ? "" : Data["POLine"].ToString().Trim();
                List<R_VT_ORDER> orderList = new List<R_VT_ORDER>();

                double validFlag = 0;
                List<string> flagList = Enum.GetNames(typeof(OrderValidFlag)).ToList();
                foreach (var flag in flagList)
                {
                    if (valid.Equals(flag))
                    {
                        var eFlag = (OrderValidFlag)Enum.Parse(typeof(OrderValidFlag), flag);
                        validFlag = Convert.ToDouble(eFlag.ExtValue());
                        break;
                    }

                }
                string statusFlag = "All";
                List<string> statusList = Enum.GetNames(typeof(OrderStatus)).ToList();
                foreach (var s in statusList)
                {
                    if (status.Equals(s))
                    {
                        var sFlag = (OrderStatus)Enum.Parse(typeof(OrderStatus), s);
                        statusFlag = sFlag.ExtValue();
                        break;
                    }

                }

                orderList = SFCDB.ORM.Queryable<R_VT_ORDER>()
                    //.Where(r=>r.ORDER_NUMBER== "4500230047")
                    .WhereIF(!action.ToUpper().Equals("ALL") && !action.Equals(EnumExtensions.ExtName(OrderAction.Null)), r => r.ACTION == action)
                    .WhereIF(action.Equals(EnumExtensions.ExtName(OrderAction.Null)), r => SqlSugar.SqlFunc.IsNullOrEmpty(r.ACTION))
                    .WhereIF(!valid.ToUpper().Equals("ALL"), r => r.VALID_FLAG == validFlag)
                    .WhereIF(!status.ToUpper().Equals("ALL"), r => r.STATUS == statusFlag)
                    .WhereIF(!string.IsNullOrEmpty(po), r => SqlSugar.SqlFunc.Contains(r.ORDER_NUMBER, po))
                    .WhereIF(!string.IsNullOrEmpty(line), r => r.ORDER_LINE_ID == line)
                    .OrderBy(r => r.ORDER_NUMBER, SqlSugar.OrderByType.Desc).OrderBy(r => r.ORDER_LINE_ID).OrderBy(r => r.SCHEDULE_ID).OrderBy(r=>r.PROMISE_ID)
                    .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc)
                    .OrderBy(r => r.CREATED_TIME, SqlSugar.OrderByType.Desc).ToList();

                List<object> returnList = new List<object>();
                foreach (var item in orderList)
                {
                    ORDER_DETAIL_VT detail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(item.ORDER_DETAIL));
                    string commitFile = "";
                    string shipmentFile = "";
                    string shipmentDN = "";
                    string pgiDate = "";
                    double shipmentTotal = 0;
                    double balanceQty = 0;

                    var commit = SFCDB.ORM.Queryable<R_VT_ORDER_COMMIT>().Where(r => r.VT_ORDER_ID == item.ID && r.VALID_FLAG == "1" && (r.SEND_FILE != "null" || !SqlFunc.IsNullOrEmpty(r.SEND_FILE)))
                        .OrderBy(r => r.SEND_TIME, OrderByType.Desc).ToList().FirstOrDefault();

                    var sql = SFCDB.ORM.Queryable<R_VT_ORDER_COMMIT>().Where(r => r.VT_ORDER_ID == item.ID && r.VALID_FLAG == "1").ToSql();
                    commitFile = commit == null ? "" : commit.SEND_FILE;
                    string commitFileSendTime = commit == null ? "" : (commit.SEND_TIME == null ? "" : Convert.ToDateTime(commit.SEND_TIME).ToString("yyyy/MM/dd HH:mm:ss"));
                    var shipmentList = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == item.ID && r.VALID_FLAG == "1").ToList();
                    if (shipmentList.Count > 0)
                    {
                        shipmentFile = shipmentList.OrderByDescending(r => r.CREATED_TIME).FirstOrDefault().FILE_NAME;

                        foreach (var shipment in shipmentList)
                        {
                            string detailStr = Encoding.Unicode.GetString(shipment.SHIPMENT_DETAIL);
                            SHIPMENT_DETAIL_VT shipmentDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(detailStr);
                            shipmentTotal = shipmentTotal + Convert.ToDouble(shipmentDetail.ShippedQuantity);
                            shipmentDN += $@",{shipment.DN_NO}";
                            var dnObj = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(r => r.DN_NO == shipment.DN_NO && r.DN_LINE == shipment.DN_LINE).ToList().FirstOrDefault();
                            pgiDate = (dnObj != null && dnObj.GTDATE != null) ? ((DateTime)dnObj.GTDATE).ToString("yyyy/MM/dd HH:mm:ss") : "";
                        }
                        shipmentDN = shipmentDN.Length > 1 ? shipmentDN.Substring(1) : shipmentDN;
                    }
                    if (!string.IsNullOrWhiteSpace(detail.PromiseQty))
                    {
                        try
                        {
                            balanceQty = Convert.ToDouble(detail.PromiseQty) - shipmentTotal;
                        }
                        catch (Exception)
                        {
                            throw new Exception($@"ID:{item.ID},PromiseQty error.");
                        }

                    }

                    #region Conver data
                    string requestDate = "";
                    string originalRequestDate = "";
                    string poCreationDate = "";
                    string promiseDate = "";
                    string promisedShipmentDate = "";
                    string requestedShipDate = "";
                    try
                    {
                        requestDate = string.IsNullOrWhiteSpace(detail.RequestDate) ? detail.RequestDate : Convert.ToDateTime(detail.RequestDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        requestDate = detail.RequestDate;
                    }
                    try
                    {
                        originalRequestDate = string.IsNullOrWhiteSpace(detail.FlexAttrDateRequest1) ? detail.FlexAttrDateRequest1 : Convert.ToDateTime(detail.FlexAttrDateRequest1).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        originalRequestDate = detail.FlexAttrDateRequest1;
                    }
                    try
                    {
                        promiseDate = string.IsNullOrWhiteSpace(detail.PromiseDate) ? detail.PromiseDate : Convert.ToDateTime(detail.PromiseDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        promiseDate = detail.PromiseDate;
                    }
                    try
                    {
                        promisedShipmentDate = string.IsNullOrWhiteSpace(detail.PromisedShipmentDate) ? detail.PromisedShipmentDate : Convert.ToDateTime(detail.PromisedShipmentDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        promisedShipmentDate = detail.PromisedShipmentDate;
                    }
                    try
                    {
                        poCreationDate = string.IsNullOrWhiteSpace(detail.OrderCreationDate) ? detail.OrderCreationDate : Convert.ToDateTime(detail.OrderCreationDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        poCreationDate = detail.OrderCreationDate;
                    }
                    try
                    {
                        requestedShipDate = string.IsNullOrWhiteSpace(detail.RequestedShipDate) ? detail.OrderCreationDate : Convert.ToDateTime(detail.RequestedShipDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        requestedShipDate = detail.RequestedShipDate;
                    }

                    #endregion


                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("ID", item.ID);
                    dic.Add("VALID_FLAG", item.VALID_FLAG);
                    dic.Add("Purchase Order No", detail.OrderNumber);
                    dic.Add("Line ID", detail.OrderLineId);
                    dic.Add("Schedule ID", detail.OrderRequestScheduleId);
                    dic.Add("Promise ID", detail.OrderPromiseId);
                    dic.Add("RevNumber", detail.RevNumber);
                    dic.Add("Customer Item No", detail.CustomerItemName);
                    dic.Add("Unit Price", detail.UnitPrice);
                    dic.Add("Supplier Item No", detail.SupplierItemName);
                    dic.Add("Request Qty", detail.RequestQty);
                    dic.Add("Promise Qty", detail.PromiseQty);
                    dic.Add("Shipped Qty", ChangeToF4(shipmentTotal.ToString()));
                    if (!string.IsNullOrWhiteSpace(detail.PromiseQty))
                    {
                        dic.Add("Balance Qty", ChangeToF4(balanceQty.ToString()));
                    }
                    else
                    {
                        dic.Add("Balance Qty", "");
                    }
                    //dic.Add("Request Date", requestDate);
                    //dic.Add("Original Request Date", originalRequestDate);
                    dic.Add("Requested Ship Date", requestedShipDate);
                    dic.Add("PO Creation Date", poCreationDate);
                    //dic.Add("Promise Date", promiseDate);
                    dic.Add("Promised Ship Date", promisedShipmentDate);
                    dic.Add("DN#", shipmentDN);
                    dic.Add("PGI Date", pgiDate);

                    //dic.Add("Request Date", detail.RequestDate);                   
                    //dic.Add("Original Request Date", detail.FlexAttrDateRequest1);
                    //dic.Add("PO Creation Date", detail.OrderCreationDate);
                    //dic.Add("Promise Date", detail.PromiseDate);
                    //dic.Add("Promised Ship Date", detail.PromisedShipmentDate);

                    //Shipping Mode
                    //Bill To
                    //Bill To Address Descriptor
                    //In coterms

                    dic.Add("Action", item.ACTION);
                    dic.Add("Shipping Mode", detail.FlexAttrStringRequest11);
                    dic.Add("Bill To", detail.BillTo);
                    dic.Add("Ship To Site", detail.CustomerSite);
                    //dic.Add("Bill To Address Descriptor", detail.BillToAddressDescriptor);
                    dic.Add("In coterms", detail.InCoTerms);
                    dic.Add("Status", ((OrderStatus)Enum.Parse(typeof(OrderStatus), item.STATUS)).ExtName());
                    dic.Add("File Name", item.FILE_NAME);
                    dic.Add("Created Time", item.CREATED_TIME);
                    dic.Add("Commit File", commitFile);

                    dic.Add("Send Tiem", commitFileSendTime);
                    dic.Add("Last Edit Emp", item.EDIT_EMP);
                    dic.Add("Last Edit Time", item.EDIT_TIME);
                    returnList.Add(dic);
                }

                StationReturn.Data = returnList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void GetOrderByByPO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                if (string.IsNullOrWhiteSpace(po))
                {
                    throw new Exception("Please input PO");
                }

                List<R_VT_ORDER> orderList = new List<R_VT_ORDER>();

                orderList = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => SqlSugar.SqlFunc.Contains(r.ORDER_NUMBER, po))
                    .OrderBy(r => r.ORDER_NUMBER, SqlSugar.OrderByType.Desc).OrderBy(r => r.ORDER_LINE_ID).OrderBy(r => r.SCHEDULE_ID)
                    .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc)
                    .OrderBy(r => r.CREATED_TIME, SqlSugar.OrderByType.Desc).ToList();

                List<object> returnList = new List<object>();
                foreach (var item in orderList)
                {
                    ORDER_DETAIL_VT detail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(item.ORDER_DETAIL));
                    string commitFile = "";
                    string shipmentFile = "";
                    if (!item.STATUS.Equals("0") && !item.STATUS.Equals("5"))
                    {
                        var commit = SFCDB.ORM.Queryable<R_VT_ORDER_COMMIT>().Where(r => r.VT_ORDER_ID == item.ID && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                        commitFile = commit == null ? "" : commit.SEND_FILE;
                    }
                    if (!item.STATUS.Equals("0") && !item.STATUS.Equals("1") && !item.STATUS.Equals("5"))
                    {
                        var shipment = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == item.ID && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                        shipmentFile = shipment == null ? "" : shipment.FILE_NAME;
                    }

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("ID", item.ID);
                    dic.Add("VALID_FLAG", item.VALID_FLAG);
                    dic.Add("Purchase Order No", detail.OrderNumber);
                    dic.Add("Line ID", detail.OrderLineId);
                    dic.Add("Schedule ID", detail.OrderRequestScheduleId);
                    dic.Add("Promise ID", detail.OrderPromiseId);
                    dic.Add("RevNumber", detail.RevNumber);
                    dic.Add("Request Qty", detail.RequestQty);
                    //dic.Add("Request Date", requestDate);
                    dic.Add("Request Date", detail.RequestDate);
                    //dic.Add("Original Request Date", originalRequestDate);
                    dic.Add("Original Request Date", detail.FlexAttrDateRequest1);
                    //dic.Add("PO Creation Date", poCreationDate);
                    dic.Add("PO Creation Date", detail.OrderCreationDate);
                    dic.Add("Promise Qty", detail.PromiseQty);
                    //dic.Add("Promise Date", promiseDate);
                    dic.Add("Promise Date", detail.PromiseDate);
                    //dic.Add("Promised Ship Date", promisedShipmentDate);
                    dic.Add("Promised Ship Date", detail.PromisedShipmentDate);
                    dic.Add("Action", item.ACTION);
                    dic.Add("Customer Item No", detail.CustomerItemName);
                    dic.Add("Shipping Mode", detail.FlexAttrStringRequest11);
                    dic.Add("Unit Price", detail.UnitPrice);
                    dic.Add("Supplier Item No", detail.SupplierItemName);
                    dic.Add("Bill To", detail.BillTo);
                    dic.Add("Bill To Address Descriptor", detail.BillToAddressDescriptor);
                    dic.Add("In coterms", detail.InCoTerms);
                    dic.Add("Status", ((OrderStatus)Enum.Parse(typeof(OrderStatus), item.STATUS)).ExtName());
                    dic.Add("File Name", item.FILE_NAME);
                    dic.Add("Created Time", item.CREATED_TIME);
                    dic.Add("Commit File", commitFile);
                    dic.Add("Shipment File", shipmentFile);
                    dic.Add("Edit Emp", item.EDIT_EMP);
                    dic.Add("Edit Time", item.EDIT_TIME);
                    returnList.Add(dic);
                }

                StationReturn.Data = returnList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void GetVTOrderDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                string orderId = Data["ID"].ToString();
                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }
                if (order.ORDER_DETAIL.Length == 0)
                {
                    throw new Exception("Get order detail fail.");
                }
                string str = System.Text.Encoding.Unicode.GetString(order.ORDER_DETAIL);
                var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(str);
                #region 轉換時間                
                //try
                //{
                //    orderDetail.PromiseDate = Convert.ToDateTime(orderDetail.PromiseDate).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{
                //}
                //try
                //{
                //    orderDetail.PromisedShipmentDate = Convert.ToDateTime(orderDetail.PromisedShipmentDate).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{
                //}
                //try
                //{
                //    orderDetail.RequestDate = Convert.ToDateTime(orderDetail.RequestDate).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{
                //}
                //try
                //{
                //    orderDetail.OrderCreationDate = Convert.ToDateTime(orderDetail.OrderCreationDate).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{
                //}
                //try
                //{
                //    orderDetail.RequestedShipDate = Convert.ToDateTime(orderDetail.RequestedShipDate).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{
                //}
                //try
                //{
                //    orderDetail.FlexAttrDateRequest1 = Convert.ToDateTime(orderDetail.FlexAttrDateRequest1).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{
                //}
                #endregion
                List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.ORM.Queryable<C_CUSTOMER_FILE_FORMAT>().Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                    .OrderBy(c => c.SEQ).ToList();
                if (fieldList.Count == 0)
                {
                    throw new Exception("MTIMDiscreteOrder not setting C_CUSTOMER_FILE_FORMAT");
                }
                object showItem = new Func<ORDER_DETAIL_VT, object>((d) =>
                  {
                      Dictionary<string, string> dic = new Dictionary<string, string>();
                      foreach (var field in fieldList)
                      {
                          var propertyValue = d.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(d);
                          var value = propertyValue == null ? "" : propertyValue.ToString();
                          dic.Add(field.DISPLAY_NAME.Trim(), value);
                      }
                      return dic;

                  })(orderDetail);

                StationReturn.Data = showItem;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void GetVTOneOrderByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                var orderObj = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == id).ToList().FirstOrDefault();

                var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(orderObj.ORDER_DETAIL));
                //try
                //{
                //    orderDetail.PromiseDate = Convert.ToDateTime(orderDetail.PromiseDate).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{

                //}
                //try
                //{
                //    orderDetail.PromisedShipmentDate = Convert.ToDateTime(orderDetail.PromisedShipmentDate).ToString("yyyy/MM/dd HH:mm:ss");
                //}
                //catch (Exception)
                //{

                //}

                List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.ORM.Queryable<C_CUSTOMER_FILE_FORMAT>().Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                   .OrderBy(c => c.SEQ).ToList();
                if (fieldList.Count == 0)
                {
                    throw new Exception("MTIMDiscreteOrder not setting C_CUSTOMER_FILE_FORMAT");
                }
                object showItem = new Func<ORDER_DETAIL_VT, object>((d) =>
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (var field in fieldList)
                    {
                        var propertyValue = d.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(d);
                        var value = propertyValue == null ? "" : propertyValue.ToString();
                        dic.Add(field.DISPLAY_NAME.Trim(), value);
                    }
                    return dic;

                })(orderDetail);

                StationReturn.Data = new { OrderId = orderObj.ID, OrderDetail = showItem };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void InsertOrUpdateOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                //暂时不需要Foxconn做
                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void AcceptOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                string orderId = Data["ID"].ToString();
                string oldAction = "";
                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }
                if (order.ORDER_DETAIL.Length == 0)
                {
                    throw new Exception("Get order detail fail.");
                }

                DateTime sysdate = SFCDB.ORM.GetDate();

                string detailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);

                if (order.ACTION == EnumExtensions.ExtName(OrderAction.Reject) || orderDetail.Action == EnumExtensions.ExtName(OrderAction.Reject))
                {
                    throw new Exception($@"This Purchase Order No.[{ orderDetail.OrderNumber }],Line ID:[{orderDetail.OrderLineId}],Promise Id:[{orderDetail.OrderPromiseId}],Already Reject");
                }

                CheckOrderStatus(order);

                oldAction = order.ACTION;
                order.ACTION = EnumExtensions.ExtName(OrderAction.Accept);
                orderDetail.Action = order.ACTION;
                detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetail, Newtonsoft.Json.Formatting.Indented);
                order.ORDER_DETAIL = Encoding.Unicode.GetBytes(detailStr);
                order.EDIT_TIME = sysdate;
                order.EDIT_EMP = LoginUser.EMP_NO;
                order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();

                string commitDetial = $@"Action:{order.ACTION};"
                               + $@"Purchase Order No.:{ order.ORDER_NUMBER};"
                               + $@"Line ID:{order.ORDER_LINE_ID};"
                               + $@"Promise Id:{orderDetail.OrderPromiseId};"
                               + $@"Promise Qty:{orderDetail.PromiseQty};";
                SaveCommitOrderInfo(SFCDB, order, sysdate, commitDetial);

                StationReturn.Data = $@"{order.ACTION} Order OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void UpdateOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                #region input value not null
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["PromiseQty"] == null)
                {
                    throw new Exception("Please input PromiseQty");
                }
                if (Data["PromiseDate"] == null)
                {
                    throw new Exception("Please input PromiseDate");
                }
                if (Data["PromisedShipmentDate"] == null)
                {
                    throw new Exception("Please input PromisedShipmentDate");
                }
                #endregion

                string orderId = Data["ID"].ToString();
                string pQty = Data["PromiseQty"].ToString();
                string pDate = Data["PromiseDate"].ToString();
                string pShipmentDate = Data["PromisedShipmentDate"].ToString();

                double promiseQty = 0;
                string promiseDate = "";
                string promisedShipmentDate = "";
                #region Convert data
                try
                {
                    promiseQty = Convert.ToDouble(pQty);
                }
                catch (Exception)
                {
                    throw new Exception("PromiseQty is not a number.");
                }
                try
                {
                    promiseDate = (Convert.ToDateTime(pDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("PromiseDate is not a date time.");
                }
                try
                {
                    promisedShipmentDate = (Convert.ToDateTime(pShipmentDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("PromisedShipmentDate is not a date time.");
                }
                if (promiseQty <= 0)
                {
                    throw new Exception($@"PromiseQty error [{promiseQty}].");
                }
                #endregion

                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }
                if (order.ORDER_DETAIL.Length == 0)
                {
                    throw new Exception("Get order detail fail.");
                }
                CheckOrderStatus(order);

                T_R_VT_ORDER t_r_vt_order = new T_R_VT_ORDER(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

                DateTime sysdate = SFCDB.ORM.GetDate();
                string oldAction = order.ACTION;

                string detailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);
                if (promiseQty > Convert.ToDouble(orderDetail.PromiseQty))
                {
                    throw new Exception("Input PromiseQty larger than order PromiseQty.");
                }
                if (promiseQty > Convert.ToDouble(orderDetail.RequestQty))
                {
                    throw new Exception("PromiseQty larger than RequestQty.");
                }
                if (order.ACTION == EnumExtensions.ExtName(OrderAction.Reject) || orderDetail.Action == EnumExtensions.ExtName(OrderAction.Reject))
                {
                    throw new Exception($@"This Purchase Order No.[{ orderDetail.OrderNumber }],Line ID:[{orderDetail.OrderLineId}],Promise Id:[{orderDetail.OrderPromiseId}],Already Reject");
                }
                order.ACTION = EnumExtensions.ExtName(OrderAction.Update);
                orderDetail.Action = order.ACTION;
                orderDetail.PromiseDate = promiseDate;
                orderDetail.PromisedShipmentDate = promisedShipmentDate;
                string orderCommitDetial = "";

                if (promiseQty < Convert.ToDouble(orderDetail.PromiseQty))
                {
                    #region Get max OrderPromiseId
                    //newOrderDetail.OrderPromiseId
                    var list = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == order.ORDER_NUMBER && r.ORDER_LINE_ID == order.ORDER_LINE_ID).ToList();
                    List<ORDER_DETAIL_VT> detailList = new List<ORDER_DETAIL_VT>();
                    foreach (var o in list)
                    {
                        var temp = Encoding.Unicode.GetString(o.ORDER_DETAIL);
                        detailList.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(temp));
                    }
                    var maxPromiseId = detailList.Max(r => Convert.ToInt32(r.OrderPromiseId));
                    #endregion


                    ORDER_DETAIL_VT newOrderDetail = ObjectDataHelper.Mapper<ORDER_DETAIL_VT, ORDER_DETAIL_VT>(orderDetail);
                    newOrderDetail.PromiseQty = ChangeToF4(promiseQty.ToString());
                    newOrderDetail.OrderPromiseId = (maxPromiseId + 1).ToString();
                    string newDetailStr = Newtonsoft.Json.JsonConvert.SerializeObject(newOrderDetail, Newtonsoft.Json.Formatting.Indented);
                    R_VT_ORDER newOrder = new R_VT_ORDER();
                    newOrder.ID = t_r_vt_order.GetNewID(BU, SFCDB);
                    newOrder.ORDER_NUMBER = order.ORDER_NUMBER;
                    newOrder.ORDER_LINE_ID = order.ORDER_LINE_ID;
                    newOrder.ACTION = order.ACTION;
                    newOrder.PROMISE_ID = newOrderDetail.OrderPromiseId;
                    newOrder.SCHEDULE_ID = order.SCHEDULE_ID;
                    newOrder.ORDER_DETAIL = Encoding.Unicode.GetBytes(newDetailStr);
                    newOrder.CREATED_EMP = LoginUser.EMP_NO;
                    newOrder.CREATED_TIME = sysdate;
                    newOrder.EDIT_EMP = LoginUser.EMP_NO;
                    newOrder.EDIT_TIME = sysdate;
                    newOrder.VALID_FLAG = 1;
                    newOrder.FILE_NAME = order.FILE_NAME;
                    newOrder.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                    SFCDB.ORM.Insertable<R_VT_ORDER>(newOrder).ExecuteCommand();

                    string newOrderCommitDetial = $@"UpdatePrimiseQty;Action:{order.ACTION};"
                                                + $@"Purchase Order No.:{ newOrder.ORDER_NUMBER};"
                                                + $@"Line ID:{newOrder.ORDER_LINE_ID};"
                                                + $@"Promise Id:{newOrderDetail.OrderPromiseId};"
                                                + $@"Promise Qty:{newOrderDetail.PromiseQty};"
                                                + $@"Input Promise Qty:{promiseQty}."
                                                + $@"Promise Date:{newOrderDetail.PromiseDate};"
                                                + $@"Promised Ship Date:{newOrderDetail.PromisedShipmentDate};";

                    SaveCommitOrderInfo(SFCDB, newOrder, sysdate, newOrderCommitDetial);

                    orderDetail.PromiseQty = ChangeToF4((Convert.ToDouble(orderDetail.PromiseQty) - promiseQty).ToString());

                    orderCommitDetial = $@"UpdatePrimiseQty;Action:{order.ACTION};"
                        + $@"Purchase Order No.:{ order.ORDER_NUMBER};"
                        + $@"Line ID:{order.ORDER_LINE_ID};"
                        + $@"Promise Id:{orderDetail.OrderPromiseId};"
                        + $@"Promise Qty:{orderDetail.PromiseQty};"
                        + $@"Input Promise Qty:{promiseQty}."
                        + $@"Promise Date:{orderDetail.PromiseDate};"
                        + $@"Promised Ship Date:{orderDetail.PromisedShipmentDate};";
                }
                else
                {
                    //promiseQty數量不變，則認爲只改時間而已
                    orderCommitDetial = $@"UpdatePrimiseTime;Action:{order.ACTION};"
                            + $@"Purchase Order No.:{ order.ORDER_NUMBER};"
                            + $@"Line ID:{order.ORDER_LINE_ID};"
                            + $@"Promise Id:{orderDetail.OrderPromiseId};"
                            + $@"Promise Qty:{orderDetail.PromiseQty};"
                            + $@"Input Promise Qty:{promiseQty}."
                            + $@"Promise Date:{orderDetail.PromiseDate};"
                            + $@"Promised Ship Date:{orderDetail.PromisedShipmentDate};";
                }

                detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetail, Newtonsoft.Json.Formatting.Indented);

                order.ORDER_DETAIL = Encoding.Unicode.GetBytes(detailStr);
                order.EDIT_TIME = sysdate;
                order.EDIT_EMP = LoginUser.EMP_NO;
                order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();


                SaveCommitOrderInfo(SFCDB, order, sysdate, orderCommitDetial);

                StationReturn.Data = $@"{order.ACTION} Order OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void UpdatePromiseData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                #region input value not null
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["PromiseDate"] == null)
                {
                    throw new Exception("Please input PromiseDate");
                }
                if (Data["PromisedShipmentDate"] == null)
                {
                    throw new Exception("Please input PromisedShipmentDate");
                }
                #endregion

                string orderId = Data["ID"].ToString();
                string pDate = Data["PromiseDate"].ToString();
                string pShipmentDate = Data["PromisedShipmentDate"].ToString();

                string promiseDate = "";
                string promisedShipmentDate = "";
                #region Convert data                
                try
                {
                    promiseDate = (Convert.ToDateTime(pDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("PromiseDate is not a date time.");
                }
                try
                {
                    promisedShipmentDate = (Convert.ToDateTime(pShipmentDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("PromisedShipmentDate is not a date time.");
                }
                if ((Convert.ToDateTime(pShipmentDate) - DateTime.Now).Days < 0)
                {
                    throw new Exception("Promised Ship Date Error.Promise Ship date cannot be in the past.");
                }
                #endregion

                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }
                if (order.ORDER_DETAIL.Length == 0)
                {
                    throw new Exception("Get order detail fail.");
                }
                CheckOrderStatus(order);

                T_R_VT_ORDER t_r_vt_order = new T_R_VT_ORDER(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string oldAction = order.ACTION;
                DateTime sysdate = SFCDB.ORM.GetDate();

                string detailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);
                if (order.ACTION == EnumExtensions.ExtName(OrderAction.Reject) || orderDetail.Action == EnumExtensions.ExtName(OrderAction.Reject))
                {
                    throw new Exception($@"This Purchase Order No.[{ orderDetail.OrderNumber }],Line ID:[{orderDetail.OrderLineId}],Promise Id:[{orderDetail.OrderPromiseId}],Already Reject");
                }
                string oldPromiseDate = orderDetail.PromiseDate;
                string oldPromisedShipmentDate = orderDetail.PromisedShipmentDate;
                order.ACTION = EnumExtensions.ExtName(OrderAction.Update);
                orderDetail.Action = order.ACTION;
                orderDetail.PromiseDate = promiseDate;
                orderDetail.PromisedShipmentDate = promisedShipmentDate;
                string orderCommitDetial = "";

                orderCommitDetial = $@"UpdatePrimiseTime;Action:{order.ACTION};"
                        + $@"Order Id:{ order.ID};"
                        + $@"Purchase Order No.:{ order.ORDER_NUMBER};"
                        + $@"Line ID:{order.ORDER_LINE_ID};"
                        + $@"Schedule Id:{order.SCHEDULE_ID};"
                        + $@"Old Action:{oldAction};"
                        + $@"Old Promise Date:{oldPromiseDate};"
                        + $@"Old Promised Ship Date:{oldPromisedShipmentDate};"
                        + $@"New Promise Date:{orderDetail.PromiseDate};"
                        + $@"New Promised Ship Date:{orderDetail.PromisedShipmentDate};";

                detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetail, Newtonsoft.Json.Formatting.Indented);

                order.ORDER_DETAIL = Encoding.Unicode.GetBytes(detailStr);
                order.EDIT_TIME = sysdate;
                order.EDIT_EMP = LoginUser.EMP_NO;
                order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();

                SaveCommitOrderInfo(SFCDB, order, sysdate, orderCommitDetial);

                StationReturn.Data = $@"{order.ACTION} Order OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void SplitPO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                #region input value not null
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["FirstPromiseQty"] == null)
                {
                    throw new Exception("Please input FirstPromiseQty");
                }
                if (Data["FirstPromiseDate"] == null)
                {
                    throw new Exception("Please input FirstPromiseDate");
                }
                if (Data["FirstPromisedShipmentDate"] == null)
                {
                    throw new Exception("Please input FirstPromisedShipmentDate");
                }
                if (Data["SecondPromiseQty"] == null)
                {
                    throw new Exception("Please input SecondPromiseQty");
                }
                if (Data["SecondPromiseDate"] == null)
                {
                    throw new Exception("Please input SecondPromiseDate");
                }
                if (Data["SecondPromisedShipmentDate"] == null)
                {
                    throw new Exception("Please input SecondPromisedShipmentDate");
                }
                #endregion

                string orderId = Data["ID"].ToString();
                string fPromiseQty = Data["FirstPromiseQty"].ToString();
                string fPromiseDate = Data["FirstPromiseDate"].ToString();
                string fPromiseShipmentDate = Data["FirstPromisedShipmentDate"].ToString();
                string sPromiseQty = Data["SecondPromiseQty"].ToString();
                string sPromiseDate = Data["SecondPromiseDate"].ToString();
                string sPromiseShipmentDate = Data["SecondPromisedShipmentDate"].ToString();

                double firstPromiseQty = 0;
                string firstPromiseDate = "";
                string firstPromiseShipmentDate = "";
                double secondPromiseQty = 0;
                string secondPromiseDate = "";
                string secondPromiseShipmentDate = "";
                #region Convert data
                try
                {
                    firstPromiseQty = Convert.ToDouble(fPromiseQty);
                }
                catch (Exception)
                {
                    throw new Exception("FirstPromiseQty is not a number.");
                }
                try
                {
                    firstPromiseDate = (Convert.ToDateTime(fPromiseDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("FirstPromiseDate is not a date time.");
                }
                try
                {
                    firstPromiseShipmentDate = (Convert.ToDateTime(fPromiseShipmentDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("FirstPromisedShipmentDate is not a date time.");
                }
                if (firstPromiseQty <= 0)
                {
                    throw new Exception($@"PromiseQty error [{firstPromiseQty}].");
                }

                try
                {
                    secondPromiseQty = Convert.ToDouble(sPromiseQty);
                }
                catch (Exception)
                {
                    throw new Exception("SecondPromiseQty is not a number.");
                }
                try
                {
                    secondPromiseDate = (Convert.ToDateTime(sPromiseDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("SecondPromiseDate is not a date time.");
                }
                try
                {
                    secondPromiseShipmentDate = (Convert.ToDateTime(sPromiseShipmentDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                }
                catch (Exception)
                {
                    throw new Exception("SecondPromisedShipmentDate is not a date time.");
                }
                if (secondPromiseQty <= 0)
                {
                    throw new Exception($@"PromiseQty error [{secondPromiseQty}].");
                }

                if ((Convert.ToDateTime(fPromiseShipmentDate) - DateTime.Now).Days < 0)
                {
                    throw new Exception("First Promised Ship Date Error.Promise Ship date cannot be in the past.");
                }
                if ((Convert.ToDateTime(sPromiseShipmentDate) - DateTime.Now).Days < 0)
                {
                    throw new Exception("Second Promised Ship Date Error.Promise Ship date cannot be in the past.");
                }
                #endregion

                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }
                if (order.ORDER_DETAIL.Length == 0)
                {
                    throw new Exception("Get order detail fail.");
                }
                CheckOrderStatus(order);

                T_R_VT_ORDER t_r_vt_order = new T_R_VT_ORDER(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                DateTime sysdate = SFCDB.ORM.GetDate();
                string oldAction = order.ACTION;
                string oldDetailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(oldDetailStr);
                if (firstPromiseQty + secondPromiseQty != Convert.ToDouble(orderDetail.PromiseQty))
                {
                    throw new Exception("(FirstPromiseQty + SecondPromiseQty) more than order PromiseQty.");
                }
                if (firstPromiseQty + secondPromiseQty != Convert.ToDouble(orderDetail.RequestQty))
                {
                    throw new Exception("(FirstPromiseQty + SecondPromiseQty) more than order RequestQty.");
                }
                if (order.ACTION == EnumExtensions.ExtName(OrderAction.Reject) || orderDetail.Action == EnumExtensions.ExtName(OrderAction.Reject))
                {
                    throw new Exception($@"This Purchase Order No.[{ orderDetail.OrderNumber }],Line ID:[{orderDetail.OrderLineId}],Promise Id:[{orderDetail.OrderPromiseId}],Already Reject");
                }
                string oldPromiseQty = orderDetail.PromiseQty;
                string oldPromiseDate = orderDetail.PromiseDate;
                string oldPromisedShipmentDate = orderDetail.PromisedShipmentDate;

                #region first
                order.ACTION = EnumExtensions.ExtName(OrderAction.Update);
                orderDetail.Action = order.ACTION;
                orderDetail.PromiseQty = ChangeToF4(firstPromiseQty.ToString());
                orderDetail.PromiseDate = firstPromiseDate;
                orderDetail.PromisedShipmentDate = firstPromiseShipmentDate;
                oldDetailStr = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetail, Newtonsoft.Json.Formatting.Indented);
                order.ORDER_DETAIL = Encoding.Unicode.GetBytes(oldDetailStr);
                order.EDIT_TIME = sysdate;
                order.EDIT_EMP = LoginUser.EMP_NO;
                order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();

                string orderCommitDetial = $@"UpdatePrimiseQty;Action:{order.ACTION};"
                        + $@"Order Id:{ order.ID};"
                        + $@"Purchase Order No.:{ order.ORDER_NUMBER};"
                        + $@"Line ID:{order.ORDER_LINE_ID};"
                        + $@"Schedule Id:{order.SCHEDULE_ID};"
                        + $@"Old Action:{oldAction};"
                        + $@"Old Promise Qty:{oldPromiseQty};"
                        + $@"Old Promise Date:{oldPromiseDate};"
                        + $@"Old Promised Ship Date:{oldPromisedShipmentDate};"
                        + $@"New Promise Qty:{orderDetail.PromiseQty};"
                        + $@"New Promise Date:{orderDetail.PromiseDate };"
                        + $@"New Promised Ship Date:{orderDetail.PromisedShipmentDate};";
                SaveCommitOrderInfo(SFCDB, order, sysdate, orderCommitDetial);
                #endregion

                #region second
                //Get max OrderPromiseId newOrderDetail.OrderPromiseId
                var list = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == order.ORDER_NUMBER && r.ORDER_LINE_ID == order.ORDER_LINE_ID && r.SCHEDULE_ID == order.SCHEDULE_ID).ToList();
                List<ORDER_DETAIL_VT> detailList = new List<ORDER_DETAIL_VT>();
                foreach (var o in list)
                {
                    var temp = Encoding.Unicode.GetString(o.ORDER_DETAIL);
                    detailList.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(temp));
                }
                var maxPromiseId = detailList.Max(r => Convert.ToInt32(r.OrderPromiseId));

                ORDER_DETAIL_VT newOrderDetail = ObjectDataHelper.Mapper<ORDER_DETAIL_VT, ORDER_DETAIL_VT>(orderDetail);
                newOrderDetail.PromiseQty = ChangeToF4(secondPromiseQty.ToString());
                newOrderDetail.OrderPromiseId = (maxPromiseId + 1).ToString();
                newOrderDetail.PromiseDate = secondPromiseDate;
                newOrderDetail.PromisedShipmentDate = secondPromiseShipmentDate;

                string newDetailStr = Newtonsoft.Json.JsonConvert.SerializeObject(newOrderDetail, Newtonsoft.Json.Formatting.Indented);
                R_VT_ORDER newOrder = new R_VT_ORDER();
                newOrder.ID = t_r_vt_order.GetNewID(BU, SFCDB);
                newOrder.ORDER_NUMBER = order.ORDER_NUMBER;
                newOrder.ORDER_LINE_ID = order.ORDER_LINE_ID;
                newOrder.ACTION = order.ACTION;
                newOrder.PROMISE_ID = newOrderDetail.OrderPromiseId;
                newOrder.SCHEDULE_ID = order.SCHEDULE_ID;
                newOrder.ORDER_DETAIL = Encoding.Unicode.GetBytes(newDetailStr);
                newOrder.CREATED_EMP = LoginUser.EMP_NO;
                newOrder.CREATED_TIME = sysdate;
                newOrder.EDIT_EMP = LoginUser.EMP_NO;
                newOrder.EDIT_TIME = sysdate;
                newOrder.VALID_FLAG = 1;
                newOrder.FILE_NAME = order.FILE_NAME;
                newOrder.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                SFCDB.ORM.Insertable<R_VT_ORDER>(newOrder).ExecuteCommand();

                string newOrderCommitDetial = $@"UpdatePrimiseQty;Action:{newOrder.ACTION};"
                                        + $@"Order Id:{ newOrder.ID};"
                                        + $@"Purchase Order No.:{ newOrder.ORDER_NUMBER};"
                                        + $@"Line ID:{newOrder.ORDER_LINE_ID};"
                                        + $@"Schedule Id:{newOrder.SCHEDULE_ID};"
                                        + $@"Old Action:{oldAction};"
                                        + $@"Old Promise Qty:0;"
                                        + $@"Old Promise Date:null;"
                                        + $@"Old Promised Ship Date:null;"
                                        + $@"New Promise Qty:{secondPromiseQty};"
                                        + $@"New Promise Date:{secondPromiseDate};"
                                        + $@"New Promised Ship Date:{secondPromiseShipmentDate};";
                SaveCommitOrderInfo(SFCDB, newOrder, sysdate, newOrderCommitDetial);
                #endregion

                StationReturn.Data = $@"{order.ACTION} Order OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void SplitPOByList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {               
                #region input value not null
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["SplitList"] == null)
                {
                    throw new Exception("Please input SplitList");
                }                
                #endregion

                string orderId = Data["ID"].ToString();
                Newtonsoft.Json.Linq.JToken splitList = Data["SplitList"];
                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }
                if (order.ORDER_DETAIL.Length == 0)
                {
                    throw new Exception("Get order detail fail.");
                }
                CheckOrderStatus(order);                
                string oldDetailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                ORDER_DETAIL_VT oldOrderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(oldDetailStr);
                if (order.ACTION == EnumExtensions.ExtName(OrderAction.Reject) || oldOrderDetail.Action == EnumExtensions.ExtName(OrderAction.Reject))
                {
                    throw new Exception($@"This Purchase Order No.[{ oldOrderDetail.OrderNumber }],Line ID:[{oldOrderDetail.OrderLineId}],Promise Id:[{oldOrderDetail.OrderPromiseId}],Already Reject");
                }
                double totalQty = splitList.Select(r => r["PromiseQty"]).Sum(r => Convert.ToDouble(r.ToString()));
                if (!totalQty.Equals(Convert.ToDouble(oldOrderDetail.PromiseQty)))
                {
                    throw new Exception($@"Total split qty [{totalQty}] not equals old promise qty [{oldOrderDetail.PromiseQty}].");
                }
                T_R_VT_ORDER t_r_vt_order = new T_R_VT_ORDER(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                DateTime sysdate = SFCDB.ORM.GetDate();
                string splitToPromiseID = "";
                foreach (var item in splitList)
                {
                    double promiseQty = 0;
                    string sPromisedShipDate = item["PromisedShipDate"].ToString();
                    string seq = item["NO"].ToString();
                    string promiseShipmentDate = "";
                    string promiseId = "";
                    try
                    {
                        promiseQty = Convert.ToDouble(item["PromiseQty"].ToString());
                    }
                    catch (Exception)
                    {
                        throw new Exception($@"{seq} PromiseQty is not a number.");
                    }                    
                    try
                    {
                        promiseShipmentDate = (Convert.ToDateTime(sPromisedShipDate)).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                    }
                    catch (Exception)
                    {
                        throw new Exception($@"{seq} PromisedShipDate is not a date time.");
                    }
                    if (promiseQty <= 0)
                    {
                        throw new Exception($@"{seq} PromiseQty error [{promiseQty}].");
                    }
                    if ((Convert.ToDateTime(sPromisedShipDate) - DateTime.Now).Days < 0)
                    {
                        throw new Exception($@"{seq} PromisedShipDate Error.Promise Ship date cannot be in the past.");
                    }

                    #region new order
                    if (seq.Equals("1"))
                    {
                        promiseId = order.PROMISE_ID;
                    }
                    else
                    {
                        //Get max OrderPromiseId newOrderDetail.OrderPromiseId
                        var list = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == order.ORDER_NUMBER && r.ORDER_LINE_ID == order.ORDER_LINE_ID && r.SCHEDULE_ID == order.SCHEDULE_ID).ToList();
                        List<ORDER_DETAIL_VT> detailList = new List<ORDER_DETAIL_VT>();
                        foreach (var o in list)
                        {
                            var temp = Encoding.Unicode.GetString(o.ORDER_DETAIL);
                            detailList.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(temp));
                        }
                        var maxPromiseId = detailList.Max(r => Convert.ToInt32(r.OrderPromiseId));
                        promiseId= (maxPromiseId + 1).ToString();
                    }
                    ORDER_DETAIL_VT newOrderDetail = ObjectDataHelper.Mapper<ORDER_DETAIL_VT, ORDER_DETAIL_VT>(oldOrderDetail);
                    newOrderDetail.PromiseQty = ChangeToF4(promiseQty.ToString());
                    newOrderDetail.OrderPromiseId = promiseId;                   
                    newOrderDetail.PromisedShipmentDate = promiseShipmentDate;
                    newOrderDetail.Action = EnumExtensions.ExtName(OrderAction.Update);

                    string newDetailStr = Newtonsoft.Json.JsonConvert.SerializeObject(newOrderDetail, Newtonsoft.Json.Formatting.Indented);
                    R_VT_ORDER newOrder = new R_VT_ORDER();
                    newOrder.ID = t_r_vt_order.GetNewID(BU, SFCDB);
                    newOrder.ORDER_NUMBER = order.ORDER_NUMBER;
                    newOrder.ORDER_LINE_ID = order.ORDER_LINE_ID;
                    newOrder.ACTION = EnumExtensions.ExtName(OrderAction.Update);
                    newOrder.PROMISE_ID = newOrderDetail.OrderPromiseId;
                    newOrder.SCHEDULE_ID = order.SCHEDULE_ID;
                    newOrder.ORDER_DETAIL = Encoding.Unicode.GetBytes(newDetailStr);
                    newOrder.CREATED_EMP = LoginUser.EMP_NO;
                    newOrder.CREATED_TIME = sysdate;
                    newOrder.EDIT_EMP = LoginUser.EMP_NO;
                    newOrder.EDIT_TIME = sysdate;
                    newOrder.VALID_FLAG = 1;
                    newOrder.FILE_NAME = order.FILE_NAME;
                    newOrder.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                    SFCDB.ORM.Insertable<R_VT_ORDER>(newOrder).ExecuteCommand();

                    string newOrderCommitDetial = $@"UpdatePrimiseQty;Split PO;Action:{newOrder.ACTION};"                                            
                                            + $@"Purchase Order No.:{ newOrder.ORDER_NUMBER};"
                                            + $@"Line ID:{newOrder.ORDER_LINE_ID};"
                                            + $@"Schedule Id:{newOrder.SCHEDULE_ID};"
                                            + $@"Old Order Id:{ order.ID};"
                                            + $@"Old Action:{order.ACTION};"
                                            + $@"Old Promise Qty:{oldOrderDetail.PromiseQty};"                                          
                                            + $@"Old Promised Ship Date:{oldOrderDetail.PromisedShipmentDate};"
                                            + $@"New Order Id:{ newOrder.ID};"
                                            + $@"New Action:{newOrder.ACTION};"
                                            + $@"New Promise Qty:{promiseQty};"                                           
                                            + $@"New Promised Ship Date:{promiseShipmentDate};";
                    SaveCommitOrderInfo(SFCDB, newOrder, sysdate, newOrderCommitDetial);
                    #endregion

                    splitToPromiseID += $@",{newOrder.PROMISE_ID}";
                }
                splitToPromiseID = splitToPromiseID.Length > 0 ? splitToPromiseID.Substring(1) : splitToPromiseID;
                order.VALID_FLAG = 0;
                order.EDIT_EMP = LoginUser.EMP_NO;
                order.EDIT_TIME = sysdate;
                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r=>r.ID==order.ID).ExecuteCommand();               
                SaveCommitOrderInfo(SFCDB, order, sysdate, $@"Split PO;Old PromiseID [{order.PROMISE_ID}] Split To [{splitToPromiseID}]", false);
                SFCDB.CommitTrain();
                StationReturn.Data = $@"{ EnumExtensions.ExtName(OrderAction.Update)} Order OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }            
        }
        public void RejectOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["ReasonForReject"] == null)
                {
                    throw new Exception("Please input ReasonForReject");
                }
                string orderId = Data["ID"].ToString();
                string remark = Data["Remark"] == null ? "" : Data["Remark"].ToString();
                string reasonForReject = Data["ReasonForReject"].ToString();
                if (string.IsNullOrWhiteSpace(remark))
                {
                    throw new Exception("Please input remark.");
                }
                if (remark.Length > 140)
                {
                    throw new Exception("Remark more char");
                }
                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }
                if (order.ORDER_DETAIL.Length == 0)
                {
                    throw new Exception("Get order detail fail.");
                }

                CheckOrderStatus(order);

                DateTime sysdate = SFCDB.ORM.GetDate();
                string oldAction = order.ACTION;

                string detailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);
                if (order.ACTION == EnumExtensions.ExtName(OrderAction.Reject) || orderDetail.Action == EnumExtensions.ExtName(OrderAction.Reject))
                {
                    throw new Exception($@"This Purchase Order No.[{ orderDetail.OrderNumber }],Line ID:[{orderDetail.OrderLineId}],Promise Id:[{orderDetail.OrderPromiseId}],Already Reject");
                }

                order.ACTION = EnumExtensions.ExtName(OrderAction.Reject);
                orderDetail.Action = order.ACTION;
                orderDetail.LineNotesToCustomer = remark;
                orderDetail.FlexAttrStringRequest9 = reasonForReject;
                detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetail, Newtonsoft.Json.Formatting.Indented);
                order.ORDER_DETAIL = Encoding.Unicode.GetBytes(detailStr);
                order.EDIT_TIME = sysdate;
                order.EDIT_EMP = LoginUser.EMP_NO;
                order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile);
                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();

                string commitDetial = $@"Action:{order.ACTION};"
                                               + $@"Order Id:{ order.ID};"
                                               + $@"Purchase Order No.:{ order.ORDER_NUMBER};"
                                               + $@"Line ID:{order.ORDER_LINE_ID};"
                                               + $@"Schedule Id:{order.SCHEDULE_ID};"
                                               + $@"Old Action:{oldAction};";

                SaveCommitOrderInfo(SFCDB, order, sysdate, commitDetial);

                StationReturn.Data = $@"{order.ACTION} Order OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void CancelOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                //暂时不需要Foxconn做
                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void CloseOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                //暂时不需要Foxconn做
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void EditVTOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["OrderId"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["OrderDetail"] == null)
                {
                    throw new Exception("Please input OrderDetail");
                }
                string orderId = Data["OrderId"].ToString();

                Newtonsoft.Json.Linq.JArray orderArray = (Newtonsoft.Json.Linq.JArray)Data["OrderDetail"];
                if (orderArray.Count == 0)
                {
                    throw new Exception("OrderDetail is null.");
                }

                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Get order object fail.");
                }

                ORDER_DETAIL_VT orderDetail = new ORDER_DETAIL_VT();
                var formatList = SFCDB.ORM.Queryable<C_CUSTOMER_FILE_FORMAT>()
                        .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder").ToList();

                if (orderArray.Count != formatList.Count)
                {
                    //throw new Exception("傳入的DisplayName總數與設置的欄位總數不一致");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171317"));
                }
                List<string> dateList = new List<string>() { "PO Creation Date", "Request Date", "Requested Ship Date", "Original Request Date", "Promise Date", "Promised Ship Date", "Original Promise Date" };
                for (int i = 0; i < orderArray.Count; i++)
                {
                    var no = orderArray[i]["NO"].ToString().Trim();
                    var displayName = orderArray[i]["DisplayName"].ToString().Trim();
                    string value = "";
                    if (dateList.Contains(displayName))
                    {
                        value = orderArray[i]["Value"].ToString().Trim() == "" ? "" : Convert.ToDateTime(Convert.ToDateTime(orderArray[i]["Value"].ToString().Trim()).ToString("yyyy-MM-dd")).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                    }
                    else
                    {
                        value = orderArray[i]["Value"].ToString().Trim();
                    }
                    var customerFormat = formatList.Find(f => f.DISPLAY_NAME.Trim() == displayName);
                    if (customerFormat == null || string.IsNullOrWhiteSpace(customerFormat.FILE_NAME))
                    {
                        throw new Exception($@"Display name [{displayName}] not setting.");
                    }
                    foreach (PropertyInfo orderProperty in typeof(ORDER_DETAIL_VT).GetProperties())
                    {
                        if (orderProperty.Name == customerFormat.FIELD_NAME)
                        {
                            orderProperty.SetValue(orderDetail, value, null);
                            break;
                        }
                    }
                }

                string detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetail, Newtonsoft.Json.Formatting.Indented);
                order.ORDER_DETAIL = System.Text.Encoding.Unicode.GetBytes(detailStr);
                order.ORDER_NUMBER = orderDetail.OrderNumber;
                order.ORDER_LINE_ID = orderDetail.OrderLineId;
                order.PROMISE_ID = orderDetail.OrderPromiseId;
                order.ACTION = orderDetail.Action;

                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();

                StationReturn.Data = $@"OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void GetShipmentListByOrderId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                if (Data["OrderId"] == null)
                {
                    throw new Exception("OrderId is null");
                }
                string orderId = Data["OrderId"].ToString();
                if (string.IsNullOrWhiteSpace(orderId))
                {
                    throw new Exception("OrderId error.");
                }
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var orderObj = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();

                if (orderObj == null)
                {
                    throw new Exception("Order id error or order is Invalid");
                }
                var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(orderObj.ORDER_DETAIL));
                List<R_VT_SHIPMENT> shipmentList = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == orderObj.ID ).ToList();
                double shippedTotalQty = 0.00;
                List<object> list = new List<object>();
                if (shipmentList.Count > 0)
                {
                    int index = 0;
                    foreach (var sObj in shipmentList)
                    {
                        index++;
                        string dStr = Encoding.Unicode.GetString(sObj.SHIPMENT_DETAIL);
                        SHIPMENT_DETAIL_VT shipmentDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(dStr);
                        shippedTotalQty = shippedTotalQty + Convert.ToDouble(shipmentDetail.ShippedQuantity);
                        bool isCancel = false;
                        if (sObj.VALID_FLAG == "1" && shipmentDetail.Action.Equals("InsertOrUpdate") && !SFCDB.ORM.Queryable<R_VT_SHIPMENT_CANCEL>().Any(r => r.CANCEL_ID == sObj.ID))
                        {
                            isCancel = true;
                        }
                        list.Add(new
                        {
                            NO = index,
                            ID = sObj.ID,
                            DN = sObj.DN_NO,
                            ShippedQty = shipmentDetail.ShippedQuantity,
                            FileName = sObj.FILE_NAME,
                            Action = shipmentDetail.Action,
                            CreatTime = sObj.CREATED_TIME,
                            CreatBy=sObj.CREATED_EMP,
                            SendTime = sObj.SEND_TIME,
                            Validity = sObj.VALID_FLAG.Equals("1") ? "Valid" : "Invalid",
                            Cancel = isCancel
                        }) ;
                    }
                }
                double waitShippingQty = Convert.ToDouble(orderDetail.PromiseQty) - shippedTotalQty;

                StationReturn.Data = new { RequestQty = orderDetail.RequestQty, PromiseQty = orderDetail.PromiseQty, ShippedQty = ChangeToF4(shippedTotalQty.ToString()), BalanceQty = ChangeToF4(waitShippingQty.ToString()), ShipmentList = list };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void GetShipmentById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                if (Data["ID"] == null)
                {
                    throw new Exception("ID is null");
                }
                string id = Data["ID"].ToString();
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception("ID error.");
                }
                SFCDB = this.DBPools["SFCDB"].Borrow();
                R_VT_SHIPMENT shipmentObj = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ID == id ).ToList().FirstOrDefault();
                if (shipmentObj == null)
                {
                    throw new Exception($@"shipment [{id}] not exist.");
                }
                SHIPMENT_DETAIL_VT shipment = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(Encoding.Unicode.GetString(shipmentObj.SHIPMENT_DETAIL));
                List<C_CUSTOMER_FILE_FORMAT> sFormatList = SFCDB.ORM.Queryable<C_CUSTOMER_FILE_FORMAT>().Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMShipment")
                    .OrderBy(c => c.SEQ).ToList();
                if (sFormatList.Count == 0)
                {
                    throw new Exception("MTIMShipment not setting C_CUSTOMER_FILE_FORMAT");
                }
                object showItem = new Func<SHIPMENT_DETAIL_VT, object>((s) =>
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (var field in sFormatList)
                    {
                        var propertyValue = s.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(s);
                        var value = propertyValue == null ? "" : propertyValue.ToString();
                        dic.Add(field.DISPLAY_NAME.Trim(), value);
                    }
                    return dic;

                })(shipment);

                StationReturn.Data = showItem;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void CreatShipment(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();            
            try
            {
                if (Data["OrderId"] == null)
                {
                    throw new Exception("OrderId is null");
                }
                if (Data["DN"] == null)
                {
                    throw new Exception("DN is null");
                }
                //if (Data["ShipmentMode"] == null)
                //{
                //    throw new Exception("ShipmentMode is null");
                //}
                //if (Data["Carrier"] == null)
                //{
                //    throw new Exception("Carrier is null");
                //} 
               
                string orderId = Data["OrderId"].ToString();
                string dn = Data["DN"].ToString();
                string carrier = Data["Carrier"].ToString();
                string shipmentMode = Data["ShipmentMode"].ToString();

                var orderObj = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                DoCreatShipment(SFCDB.ORM, orderObj, dn, LoginUser.EMP_NO);                
                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void GetShipmentByOrderId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string orderId = Data["OrderId"] == null ? "" : Data["OrderId"].ToString();

                R_VT_SHIPMENT shipmentObj = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == orderId && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (shipmentObj == null)
                {
                    throw new Exception($@"shipment [{orderId}] not exist.");
                }
                SHIPMENT_DETAIL_VT shipment = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(Encoding.Unicode.GetString(shipmentObj.SHIPMENT_DETAIL));
                List<C_CUSTOMER_FILE_FORMAT> sFormatList = SFCDB.ORM.Queryable<C_CUSTOMER_FILE_FORMAT>().Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMShipment")
                    .OrderBy(c => c.SEQ).ToList();
                if (sFormatList.Count == 0)
                {
                    throw new Exception("MTIMShipment not setting C_CUSTOMER_FILE_FORMAT");
                }
                object showItem = new Func<SHIPMENT_DETAIL_VT, object>((s) =>
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (var field in sFormatList)
                    {
                        var propertyValue = s.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(s);
                        var value = propertyValue == null ? "" : propertyValue.ToString();
                        dic.Add(field.DISPLAY_NAME.Trim(), value);
                    }
                    return dic;

                })(shipment);

                StationReturn.Data = new { ShipmentId = shipmentObj.ID, ShipmentDetail = showItem };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void EditShipment(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["ShipmentDetail"] == null)
                {
                    throw new Exception("Please input ShipmentDetail");
                }
                string id = Data["ID"].ToString();

                var shipment = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ID == id).ToList().FirstOrDefault();
                if (shipment == null)
                {
                    throw new Exception("Get order object fail.");
                }

                Newtonsoft.Json.Linq.JArray shipmentArray = (Newtonsoft.Json.Linq.JArray)Data["ShipmentDetail"];
                if (shipmentArray.Count == 0)
                {
                    throw new Exception("shipment is null.");
                }
                SHIPMENT_DETAIL_VT shipmentDetail = new SHIPMENT_DETAIL_VT();
                var formatList = SFCDB.ORM.Queryable<C_CUSTOMER_FILE_FORMAT>()
                        .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMShipment").ToList();
                if (shipmentArray.Count != formatList.Count)
                {
                    //throw new Exception("傳入的DisplayName總數與設置的欄位總數不一致");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171317"));
                }
                for (int i = 0; i < shipmentArray.Count; i++)
                {
                    var no = shipmentArray[i]["NO"].ToString().Trim();
                    var displayName = shipmentArray[i]["DisplayName"].ToString().Trim();
                    var value = shipmentArray[i]["Value"].ToString().Trim();
                    var customerFormat = formatList.Find(f => f.DISPLAY_NAME.Trim() == displayName);
                    if (customerFormat == null || string.IsNullOrWhiteSpace(customerFormat.FILE_NAME))
                    {
                        throw new Exception($@"Display name [{displayName}] not setting.");
                    }
                    foreach (PropertyInfo pShipment in typeof(SHIPMENT_DETAIL_VT).GetProperties())
                    {
                        if (pShipment.Name == customerFormat.FIELD_NAME)
                        {
                            pShipment.SetValue(shipmentDetail, value, null);
                            break;
                        }
                    }
                }

                string detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(shipmentDetail, Newtonsoft.Json.Formatting.Indented);

                shipment.SHIPMENT_DETAIL = System.Text.Encoding.Unicode.GetBytes(detailStr);
                shipment.ACTION = shipmentDetail.Action;
                SFCDB.ORM.Updateable<R_VT_SHIPMENT>(shipment).Where(r => r.ID == shipment.ID).ExecuteCommand();

                StationReturn.Data = $@"OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void GetForecast(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string valid = Data["Valid"] == null ? "" : Data["Valid"].ToString();
                double validFlag = 1;
                List<string> validList = Enum.GetNames(typeof(ForecastValid)).ToList();
                foreach (var v in validList)
                {
                    if (valid.Equals(v))
                    {
                        var sFlag = (ForecastValid)Enum.Parse(typeof(ForecastValid), v);
                        validFlag = Convert.ToDouble(sFlag.ExtValue());
                        break;
                    }

                }
                List<object> returnList = new List<object>();
                var forecastList = SFCDB.ORM.Queryable<R_VT_FORECAST>()
                    //.Where(r=>r.SUPPLIER_ITEM_NAME == "VT01202021")
                    .WhereIF(!valid.ToUpper().Equals("ALL"), r => r.VALID_FLAG == validFlag).ToList();

                List<string> dateList = forecastList.OrderBy(r => r.FORECAST_DATE).Select(r => r.FORECAST_DATE).Distinct().ToList();
                var lotList = forecastList.OrderBy(r => r.SUPPLIER_ITEM_NAME).Select(r => new { r.LOT_NO, r.VALID_FLAG }).Distinct().ToList();
                foreach (var lot in lotList)
                {
                    var skuList = forecastList.Where(r => r.LOT_NO == lot.LOT_NO && r.VALID_FLAG == lot.VALID_FLAG)
                        .Select(r => r.SUPPLIER_ITEM_NAME).Distinct().ToList();
                    foreach (var sku in skuList)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("LOT_NO", lot.LOT_NO);
                        dic.Add("VALID_FLAG", lot.VALID_FLAG);
                        dic.Add("SKU", sku);
                        foreach (var date in dateList)
                        {
                            //var item = forecastList.Find(r => r.LOT_NO == lot.LOT_NO && r.VALID_FLAG == lot.VALID_FLAG && r.SUPPLIER_ITEM_NAME == sku && r.FORECAST_DATE == date);
                            //if (item == null)
                            //{
                            //    continue;
                            //    //dic.Add((Convert.ToDateTime(date)).ToString("yyyy/MM/dd"), new
                            //    //{
                            //    //    ID = "",
                            //    //    QTY = 0
                            //    //});
                            //}
                            //else
                            //{
                            //    dic.Add((Convert.ToDateTime(date)).ToString("yyyy/MM/dd"), new
                            //    {
                            //        ID = item.ID,
                            //        QTY = item.QUANTITY
                            //    });
                            //}

                            int qty = 0;

                            List<R_VT_FORECAST> items = forecastList.Where(r => r.LOT_NO == lot.LOT_NO && r.VALID_FLAG == lot.VALID_FLAG && r.SUPPLIER_ITEM_NAME == sku && r.FORECAST_DATE == date)
                                .ToList<R_VT_FORECAST>();
                            if (items.Count > 0)
                            {
                                //Dictionary<string, object> dicItem = new Dictionary<string, object>();
                                //foreach (var item in items)
                                //{
                                //    dicItem.Add(item.ID, new
                                //    {
                                //        SITE_NAME = item.SITE_NAME,
                                //        QTY = item.QUANTITY
                                //    });
                                //}
                                //dic.Add((Convert.ToDateTime(date)).ToString("yyyy/MM/dd"), dicItem.ToList());

                                foreach (var item in items)
                                {
                                    if (item.QUANTITY != null)
                                    {
                                        qty = qty + (int)item.QUANTITY;
                                    }
                                }
                                dic.Add((Convert.ToDateTime(date)).ToString("yyyy/MM/dd"), qty);
                            }
                            else
                            {
                                dic.Add((Convert.ToDateTime(date)).ToString("yyyy/MM/dd"), 0);
                            }

                        }
                        returnList.Add(dic);
                    }
                }
                StationReturn.Data = returnList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void EditForecast(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                R_SYNC_LOCK runingObj = SFCDB.ORM.Queryable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME.Equals("SendForecast")).ToList().FirstOrDefault();
                if (runingObj != null)
                {
                    throw new Exception($@"{runingObj.LOCK_IP} is sending forecast,Please try again later");
                }
                if (Data["ID"] == null)
                {
                    throw new Exception("Please input ID");
                }
                if (Data["NewQty"] == null)
                {
                    throw new Exception("Please input New Qty");
                }
                string forecastId = Data["ID"].ToString();
                double newQty;

                if (!double.TryParse(Data["NewQty"].ToString(), out newQty))
                {
                    throw new Exception("Please input a number");
                }
                if (newQty < 0)
                {
                    throw new Exception("New Qty error.");
                }
                var forecast = SFCDB.ORM.Queryable<R_VT_FORECAST>().Where(r => r.ID == forecastId && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (forecast == null)
                {
                    throw new Exception("Get forecast object fail.");
                }
                double? oldQty = forecast.QUANTITY;
                forecast.QUANTITY = newQty;
                forecast.EDIT_EMP = LoginUser.EMP_NO;
                forecast.EDIT_TIME = SFCDB.ORM.GetDate();
                SFCDB.ORM.Updateable<R_VT_FORECAST>(forecast).Where(r => r.ID == forecast.ID).ExecuteCommand();

                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                R_MES_LOG logObj = new R_MES_LOG();
                logObj.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                logObj.PROGRAM_NAME = "VertivPOApi";
                logObj.CLASS_NAME = "MESStation.Config.Vertiv.VertivPOApi";
                logObj.FUNCTION_NAME = "EditForecast";
                logObj.LOG_MESSAGE = $@"Old Qty:{oldQty};New Qty:{newQty}";
                logObj.DATA1 = forecast.ID;
                logObj.DATA2 = forecast.SUPPLIER_ITEM_NAME;
                logObj.DATA3 = forecast.FORECAST_DATE;
                logObj.EDIT_TIME = forecast.EDIT_TIME;
                logObj.EDIT_EMP = LoginUser.EMP_NO;
                t_r_mes_log.InsertkpsnLog(logObj, SFCDB);

                StationReturn.Data = $@"OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void GetForecastEditLog(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var logList = SFCDB.ORM.Queryable<R_MES_LOG>().Where(r => r.PROGRAM_NAME == "VertivPOApi" &&
                  r.CLASS_NAME == "MESStation.Config.Vertiv.VertivPOApi" && r.FUNCTION_NAME == "EditForecast")
                    .Select(r => new { ID = r.DATA1, SKU = r.DATA2, DATE = r.DATA3, MSG = r.LOG_MESSAGE, r.EDIT_EMP, r.EDIT_TIME })
                    .ToList();
                StationReturn.Data = logList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void CommitForecast(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                CommitForecast commit = new CommitForecast($@"{System.IO.Directory.GetCurrentDirectory()}\Forecast", "outbound", BU, SFCDB.ORM, this.IP);
                commit.Run();
                StationReturn.Data = $@"OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetWaitComfirmOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string orderNo = Data["OrderNo"] == null ? "" : Data["OrderNo"].ToString();
                string lineId = Data["LineId"] == null ? "" : Data["LineId"].ToString();
                string promiseId = Data["PromiseId"] == null ? "" : Data["PromiseId"].ToString();
                List<R_VT_ORDER> orderList = new List<R_VT_ORDER>();

                orderList = SFCDB.ORM.Queryable<R_VT_ORDER>()
                    .Where(r => r.ORDER_NUMBER == orderNo && r.ORDER_LINE_ID == lineId && r.PROMISE_ID == promiseId && r.VALID_FLAG == 2)
                    .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();

                List<C_CUSTOMER_FILE_FORMAT> formatList = SFCDB.ORM.Queryable<C_CUSTOMER_FILE_FORMAT>().Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                   .OrderBy(c => c.SEQ).ToList();
                if (formatList.Count == 0)
                {
                    throw new Exception("MTIMDiscreteOrder not setting C_CUSTOMER_FILE_FORMAT");
                }

                List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();
                foreach (var item in orderList)
                {
                    ORDER_DETAIL_VT detail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(item.ORDER_DETAIL));
                    #region Conver data
                    //try
                    //{
                    //    detail.RequestDate = string.IsNullOrWhiteSpace(detail.RequestDate) ? detail.RequestDate : Convert.ToDateTime(detail.RequestDate).ToString("yyyy/MM/dd HH:mm:ss");
                    //}
                    //catch (Exception)
                    //{
                    //}
                    //try
                    //{
                    //    detail.FlexAttrDateRequest1 = string.IsNullOrWhiteSpace(detail.FlexAttrDateRequest1) ? detail.FlexAttrDateRequest1 : Convert.ToDateTime(detail.FlexAttrDateRequest1).ToString("yyyy/MM/dd HH:mm:ss");
                    //}
                    //catch (Exception)
                    //{
                    //}
                    //try
                    //{
                    //    detail.PromiseDate = string.IsNullOrWhiteSpace(detail.PromiseDate) ? detail.PromiseDate : Convert.ToDateTime(detail.PromiseDate).ToString("yyyy/MM/dd HH:mm:ss");
                    //}
                    //catch (Exception)
                    //{
                    //}
                    //try
                    //{
                    //    detail.PromisedShipmentDate = string.IsNullOrWhiteSpace(detail.PromisedShipmentDate) ? detail.PromisedShipmentDate : Convert.ToDateTime(detail.PromisedShipmentDate).ToString("yyyy/MM/dd HH:mm:ss");
                    //}
                    //catch (Exception)
                    //{
                    //}
                    //try
                    //{
                    //    detail.OrderCreationDate = string.IsNullOrWhiteSpace(detail.OrderCreationDate) ? detail.OrderCreationDate : Convert.ToDateTime(detail.OrderCreationDate).ToString("yyyy/MM/dd HH:mm:ss");
                    //}
                    //catch (Exception)
                    //{
                    //}
                    #endregion

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("ID", item.ID);
                    new Action<ORDER_DETAIL_VT>((d) =>
                    {
                        foreach (var field in formatList)
                        {
                            var propertyValue = d.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(d);
                            var value = propertyValue == null ? "" : propertyValue.ToString();
                            dic.Add(field.DISPLAY_NAME.Trim(), value);
                        }
                    })(detail);

                    dic.Add("File Name", item.FILE_NAME);
                    dic.Add("Created Time", item.CREATED_TIME);
                    dic.Add("Edit Emp", item.EDIT_EMP);
                    dic.Add("Edit Time", item.EDIT_TIME);
                    returnList.Add(dic);
                }

                List<string> keyList = formatList.Select(r => r.DISPLAY_NAME).ToList();
                List<string> noSameList = new List<string>();
                foreach (var key in keyList)
                {
                    var keyValueList = returnList.SelectMany(r => r.Where(k => k.Key == key)).ToList();
                    var valueList = keyValueList.Select(v => v.Value).ToList().Distinct().ToList();
                    if (valueList.Count > 1)
                    {
                        noSameList.Add(key);
                        foreach (var item in returnList)
                        {
                            //標記值不同的欄位
                            item[key] = new { IsSame = false, Value = item[key] };
                        }
                    }
                }

                //值不同的欄位移到前面去顯示
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (var item in returnList)
                {
                    Dictionary<string, object> newDic = new Dictionary<string, object>();
                    foreach (var ns in noSameList)
                    {
                        var tt = item.First(r => r.Key == ns);
                        newDic.Add(tt.Key, tt.Value);
                        item.Remove(ns);
                    }
                    foreach (var s in item)
                    {
                        newDic.Add(s.Key, s.Value);
                    }
                    list.Add(newDic);
                }

                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void ComfirmOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string orderId = Data["OrderId"] == null ? "" : Data["OrderId"].ToString();
                var order = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId).ToList().FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("Input OrderId error.");
                }
                //List<R_VT_ORDER> orderList = SFCDB.ORM.Queryable<R_VT_ORDER>()
                //    .Where(r => r.ORDER_NUMBER == order.ORDER_NUMBER && r.ORDER_LINE_ID == order.ORDER_LINE_ID && r.PROMISE_ID == order.PROMISE_ID && r.VALID_FLAG == 2)
                //    .ToList();
                //if (orderList.Count < 2)
                //{
                //    throw new Exception($@"ORDER_NUMBER:{order.ORDER_NUMBER};ORDER_LINE_ID:{ order.ORDER_LINE_ID };PROMISE_ID:{order.PROMISE_ID};VALID_FLAG:2; Only one.");
                //}
                DateTime systemdate = SFCDB.ORM.GetDate();
                order.VALID_FLAG = 1;
                order.EDIT_TIME = systemdate;
                order.EDIT_EMP = LoginUser.EMP_NO;
                SFCDB.ORM.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();

                SFCDB.ORM.Updateable<R_VT_ORDER>().SetColumns(r => new R_VT_ORDER {
                    VALID_FLAG = 0,
                    EDIT_TIME = systemdate,
                    EDIT_EMP = LoginUser.EMP_NO
                }).Where(r => r.ORDER_NUMBER == order.ORDER_NUMBER && r.ORDER_LINE_ID == order.ORDER_LINE_ID && r.PROMISE_ID == order.PROMISE_ID && r.VALID_FLAG == 2)
                .ExecuteCommand();

                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void CombinePO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["MainId"] == null)
                {
                    throw new Exception("Please input MainId");
                }
                if (Data["OtherId"] == null)
                {
                    throw new Exception("Please input OtherId");
                }
                string id = Data["MainId"].ToString();
                Newtonsoft.Json.Linq.JArray otherId = (Newtonsoft.Json.Linq.JArray)Data["OtherId"];
                var mainOrder = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == id && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (mainOrder == null)
                {
                    throw new Exception("Id error.");
                }
                if (mainOrder.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Closed)))
                {
                    throw new Exception($@"Purchase Order No.:{mainOrder.ORDER_NUMBER},Line ID:{mainOrder.ORDER_LINE_ID};Promise ID:{mainOrder.PROMISE_ID};Already closed");
                }
                if (mainOrder.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Reject)))
                {
                    throw new Exception($@"Purchase Order No.:{mainOrder.ORDER_NUMBER},Line ID:{mainOrder.ORDER_LINE_ID};Promise ID:{mainOrder.PROMISE_ID};Already reject");
                }
                if (mainOrder.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.WaitForSendShipment)))
                {
                    throw new Exception($@"Purchase Order No.:{mainOrder.ORDER_NUMBER},Line ID:{mainOrder.ORDER_LINE_ID};Promise ID:{mainOrder.PROMISE_ID};Already creat shipment");
                }
                var mainOrderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(mainOrder.ORDER_DETAIL));
                DateTime sysdate = SFCDB.ORM.GetDate();
                string logMes = $@"Main ID:{mainOrder.ID},Main Promise ID:{mainOrder.PROMISE_ID},Main Promise QTY: {mainOrderDetail.PromiseQty};";
                for (int i = 0; i < otherId.Count; i++)
                {
                    var oId = otherId[i]["ID"].ToString().Trim();
                    var otherOrder = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == oId).ToList().FirstOrDefault();
                    if (otherOrder == null)
                    {
                        throw new Exception($@"Purchase Order No.:{otherOrder.ORDER_NUMBER},Line ID:{otherOrder.ORDER_LINE_ID};Promise ID:{otherOrder.PROMISE_ID};is Invalid");
                    }
                    if (otherOrder.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Closed)))
                    {
                        throw new Exception($@"Purchase Order No.:{otherOrder.ORDER_NUMBER},Line ID:{otherOrder.ORDER_LINE_ID};Promise ID:{otherOrder.PROMISE_ID};Already closed");
                    }
                    if (otherOrder.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Reject)))
                    {
                        throw new Exception($@"Purchase Order No.:{otherOrder.ORDER_NUMBER},Line ID:{otherOrder.ORDER_LINE_ID};Promise ID:{otherOrder.PROMISE_ID};Already reject");
                    }
                    if (otherOrder.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.WaitForSendShipment)))
                    {
                        throw new Exception($@"Purchase Order No.:{otherOrder.ORDER_NUMBER},Line ID:{otherOrder.ORDER_LINE_ID};Promise ID:{otherOrder.PROMISE_ID};Already creat shipment");
                    }

                    var otherOrderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(otherOrder.ORDER_DETAIL));
                    logMes = logMes + $@"Other ID:{otherOrder.ID},Other Promise ID:{otherOrder.PROMISE_ID},Other Promise QTY: {otherOrderDetail.PromiseQty};";
                    mainOrderDetail.PromiseQty = ChangeToF4((Convert.ToDouble(mainOrderDetail.PromiseQty) + Convert.ToDouble(otherOrderDetail.PromiseQty)).ToString());
                    otherOrderDetail.PromiseQty = "0.0000";

                    string otherStr = Newtonsoft.Json.JsonConvert.SerializeObject(otherOrderDetail, Newtonsoft.Json.Formatting.Indented);

                    otherOrder.ORDER_DETAIL = System.Text.Encoding.Unicode.GetBytes(otherStr);
                    otherOrder.VALID_FLAG = 0;
                    otherOrder.EDIT_EMP = LoginUser.EMP_NO;
                    otherOrder.EDIT_TIME = sysdate;
                    SFCDB.ORM.Updateable<R_VT_ORDER>(otherOrder).Where(r => r.ID == otherOrder.ID).ExecuteCommand();
                }
                string mainStr = Newtonsoft.Json.JsonConvert.SerializeObject(mainOrderDetail, Newtonsoft.Json.Formatting.Indented);
                mainOrder.ORDER_DETAIL = System.Text.Encoding.Unicode.GetBytes(mainStr);
                mainOrder.EDIT_EMP = LoginUser.EMP_NO;
                mainOrder.EDIT_TIME = sysdate;
                SFCDB.ORM.Updateable<R_VT_ORDER>(mainOrder).Where(r => r.ID == mainOrder.ID).ExecuteCommand();

                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                R_MES_LOG logObj = new R_MES_LOG();
                logObj.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                logObj.PROGRAM_NAME = "VertivPOApi";
                logObj.CLASS_NAME = "MESStation.Config.Vertiv.VertivPOApi";
                logObj.FUNCTION_NAME = "CombinePO";
                logObj.LOG_MESSAGE = logMes;
                logObj.DATA1 = mainOrder.ORDER_NUMBER;
                logObj.DATA2 = mainOrder.ORDER_LINE_ID;
                logObj.EDIT_TIME = sysdate;
                logObj.EDIT_EMP = LoginUser.EMP_NO;
                t_r_mes_log.InsertkpsnLog(logObj, SFCDB);

                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetWaitCombinePO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                string getOther = Data["GetOther"] == null ? "" : Data["GetOther"].ToString();

                var mainOrder = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == id && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (mainOrder == null)
                {
                    throw new Exception("Id error.");
                }
                List<R_VT_ORDER> orderList = new List<R_VT_ORDER>();
                if (!getOther.ToUpper().Equals("YES"))
                {
                    orderList.Add(mainOrder);
                }
                else
                {
                    orderList = SFCDB.ORM.Queryable<R_VT_ORDER>()
                        .Where(r => r.ORDER_NUMBER == mainOrder.ORDER_NUMBER && r.ORDER_LINE_ID == mainOrder.ORDER_LINE_ID
                        && r.VALID_FLAG == 1 && !r.ID.Equals(mainOrder.ID)
                        && (!r.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Closed)))
                        && !r.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Reject)) && !r.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.WaitForSendShipment)))
                        .ToList();
                }
                List<object> returnList = new List<object>();
                foreach (var orderObj in orderList)
                {
                    var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(orderObj.ORDER_DETAIL));
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("ID", orderObj.ID);
                    dic.Add("Purchase Order No", orderDetail.OrderNumber);
                    dic.Add("Line ID", orderDetail.OrderLineId);
                    dic.Add("Promise ID", orderDetail.OrderPromiseId);
                    dic.Add("Request Qty", orderDetail.RequestQty);
                    //dic.Add("Request Date", orderDetail.RequestDate);
                    //dic.Add("Original Request Date", orderDetail.FlexAttrDateRequest1);
                    //dic.Add("PO Creation Date", orderDetail.OrderCreationDate);
                    dic.Add("Promise Qty", orderDetail.PromiseQty);
                    dic.Add("Promise Date", orderDetail.PromiseDate);
                    dic.Add("Promised Ship Date", orderDetail.PromisedShipmentDate);
                    //dic.Add("Action", orderObj.ACTION);
                    //dic.Add("Customer Item No", orderDetail.CustomerItemName);
                    //dic.Add("Shipping Mode", orderDetail.FlexAttrStringRequest11);
                    //dic.Add("Unit Price", orderDetail.UnitPrice);
                    //dic.Add("Supplier Item No", orderDetail.SupplierItemName);
                    //dic.Add("Bill To", orderDetail.BillTo);
                    //dic.Add("Bill To Address Descriptor", orderDetail.BillToAddressDescriptor);
                    //dic.Add("In coterms", orderDetail.InCoTerms);
                    returnList.Add(dic);
                }
                StationReturn.Data = returnList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void DownloadByPO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                Newtonsoft.Json.Linq.JArray arrayPo = (Newtonsoft.Json.Linq.JArray)Data["PoList"];
                List<R_VT_ORDER> orderList = new List<R_VT_ORDER>();
                List<string> listPo = new List<string>();
                for (int i = 0; i < arrayPo.Count; i++)
                {
                    string po = arrayPo[i].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(po) && !listPo.Contains(po))
                    {
                        listPo.Add(po);
                    }
                }
                orderList = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => listPo.Contains(r.ORDER_NUMBER) && r.VALID_FLAG == 1)
                    .OrderBy(r => r.ORDER_NUMBER).OrderBy(r => r.ORDER_LINE_ID).OrderBy(r => r.SCHEDULE_ID).ToList();
                #region 構造用於生成excel 的table
                DataTable poTable = new DataTable("PoList");
                poTable.Columns.Add("Purchase Order No");
                poTable.Columns.Add("Line ID");
                poTable.Columns.Add("Schedule ID");
                poTable.Columns.Add("Promise ID");
                poTable.Columns.Add("RevNumber");
                poTable.Columns.Add("Request Qty");
                poTable.Columns.Add("Promise Qty");
                poTable.Columns.Add("Shipped Qty");
                poTable.Columns.Add("Balance Qty");
                //poTable.Columns.Add("Request Date");
                //poTable.Columns.Add("Original Request Date");
                poTable.Columns.Add("Requested Ship Date");
                poTable.Columns.Add("PO Creation Date");
                //poTable.Columns.Add("Promise Date");
                poTable.Columns.Add("Promised Ship Date");
                poTable.Columns.Add("DN#");
                poTable.Columns.Add("PGI Date");
                poTable.Columns.Add("Action");
                poTable.Columns.Add("Customer Item No");
                poTable.Columns.Add("Shipping Mode");
                poTable.Columns.Add("Unit Price");
                poTable.Columns.Add("Supplier Item No");
                poTable.Columns.Add("Bill To");
                poTable.Columns.Add("Ship To Site");
                //poTable.Columns.Add("Bill To Address Descriptor");
                poTable.Columns.Add("In coterms");
                poTable.Columns.Add("Status");
                poTable.Columns.Add("File Name");
                poTable.Columns.Add("Created Time");
                poTable.Columns.Add("Commit File");
                poTable.Columns.Add("Send Tiem");
                poTable.Columns.Add("Last Edit Emp");
                poTable.Columns.Add("Last Edit Time");
                #endregion
                foreach (var item in orderList)
                {
                    ORDER_DETAIL_VT detail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(item.ORDER_DETAIL));
                    string commitFile = "";
                    string shipmentFile = "";
                    string shipmentDN = "";
                    string pgiDate = "";
                    double shipmentTotal = 0;
                    double balanceQty = 0;

                    var commit = SFCDB.ORM.Queryable<R_VT_ORDER_COMMIT>().Where(r => r.VT_ORDER_ID == item.ID && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                    var sql = SFCDB.ORM.Queryable<R_VT_ORDER_COMMIT>().Where(r => r.VT_ORDER_ID == item.ID && r.VALID_FLAG == "1").ToSql();
                    commitFile = commit == null ? "" : commit.SEND_FILE;
                    string commitFileSendTime = commit == null ? "" : (commit.SEND_TIME == null ? "" : Convert.ToDateTime(commit.SEND_TIME).ToString("yyyy/MM/dd HH:mm:ss"));
                    var shipmentList = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == item.ID && r.VALID_FLAG == "1").ToList();
                    if (shipmentList.Count > 0)
                    {
                        shipmentFile = shipmentList.OrderByDescending(r => r.CREATED_TIME).FirstOrDefault().FILE_NAME;

                        foreach (var shipment in shipmentList)
                        {
                            string detailStr = Encoding.Unicode.GetString(shipment.SHIPMENT_DETAIL);
                            SHIPMENT_DETAIL_VT shipmentDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(detailStr);
                            shipmentTotal = shipmentTotal + Convert.ToDouble(shipmentDetail.ShippedQuantity);
                            shipmentDN = $@",{shipment.DN_NO}";
                            var dnObj = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(r => r.DN_NO == shipment.DN_NO && r.DN_LINE == shipment.DN_LINE).ToList().FirstOrDefault();
                            pgiDate = (dnObj != null && dnObj.GTDATE != null) ? ((DateTime)dnObj.GTDATE).ToString("yyyy/MM/dd HH:mm:ss") : "";
                        }
                        shipmentDN = shipmentDN.Length > 1 ? shipmentDN.Substring(1) : shipmentDN;
                    }
                    if (!string.IsNullOrWhiteSpace(detail.PromiseQty))
                    {
                        try
                        {
                            balanceQty = Convert.ToDouble(detail.PromiseQty) - shipmentTotal;
                        }
                        catch (Exception)
                        {
                            throw new Exception($@"ID:{item.ID},PromiseQty error.");
                        }

                    }

                    #region Conver data
                    string requestDate = "";
                    string originalRequestDate = "";
                    string poCreationDate = "";
                    string promiseDate = "";
                    string promisedShipmentDate = "";
                    string requestedShipDate = "";
                    try
                    {
                        requestDate = string.IsNullOrWhiteSpace(detail.RequestDate) ? detail.RequestDate : Convert.ToDateTime(detail.RequestDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        requestDate = detail.RequestDate;
                    }
                    try
                    {
                        originalRequestDate = string.IsNullOrWhiteSpace(detail.FlexAttrDateRequest1) ? detail.FlexAttrDateRequest1 : Convert.ToDateTime(detail.FlexAttrDateRequest1).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        originalRequestDate = detail.FlexAttrDateRequest1;
                    }
                    try
                    {
                        promiseDate = string.IsNullOrWhiteSpace(detail.PromiseDate) ? detail.PromiseDate : Convert.ToDateTime(detail.PromiseDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        promiseDate = detail.PromiseDate;
                    }
                    try
                    {
                        promisedShipmentDate = string.IsNullOrWhiteSpace(detail.PromisedShipmentDate) ? detail.PromisedShipmentDate : Convert.ToDateTime(detail.PromisedShipmentDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        promisedShipmentDate = detail.PromisedShipmentDate;
                    }
                    try
                    {
                        poCreationDate = string.IsNullOrWhiteSpace(detail.OrderCreationDate) ? detail.OrderCreationDate : Convert.ToDateTime(detail.OrderCreationDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        poCreationDate = detail.OrderCreationDate;
                    }
                    try
                    {
                        requestedShipDate = string.IsNullOrWhiteSpace(detail.RequestedShipDate) ? detail.OrderCreationDate : Convert.ToDateTime(detail.RequestedShipDate).ToString("yyyy/MM/dd");
                    }
                    catch (Exception)
                    {
                        requestedShipDate = detail.RequestedShipDate;
                    }
                    #endregion

                    DataRow row = poTable.NewRow();
                    row["Purchase Order No"] = detail.OrderNumber;
                    row["Line ID"] = detail.OrderLineId;
                    row["Schedule ID"] = detail.OrderRequestScheduleId;
                    row["Promise ID"] = detail.OrderPromiseId;
                    row["RevNumber"] = detail.RevNumber;

                    row["Customer Item No"] = detail.CustomerItemName;
                    row["Unit Price"] = detail.UnitPrice;
                    row["Supplier Item No"] = detail.SupplierItemName;

                    row["Request Qty"] = detail.RequestQty;
                    row["Promise Qty"] = detail.PromiseQty;
                    row["Shipped Qty"] = ChangeToF4(shipmentTotal.ToString());
                    if (!string.IsNullOrWhiteSpace(detail.PromiseQty))
                    {
                        row["Balance Qty"] = ChangeToF4(balanceQty.ToString());
                    }
                    else
                    {
                        row["Balance Qty"] = "";
                    }
                    //row["Request Date"] = requestDate;
                    //row["Original Request Date"] = originalRequestDate;
                    row["Requested Ship Date"] = requestedShipDate;
                    row["PO Creation Date"] = poCreationDate;
                    //row["Promise Date"] = promiseDate;
                    row["Promised Ship Date"] = promisedShipmentDate;
                    row["DN#"] = shipmentDN;
                    row["PGI Date"] = pgiDate;
                    row["Action"] = item.ACTION;
                    row["Shipping Mode"] = detail.FlexAttrStringRequest11;
                    row["Bill To"] = detail.BillTo;
                    row["Ship To Site"] = detail.CustomerSite;
                    //row["Bill To Address Descriptor"] = detail.BillToAddressDescriptor;
                    row["In coterms"] = detail.InCoTerms;
                    row["Status"] = ((OrderStatus)Enum.Parse(typeof(OrderStatus), item.STATUS)).ExtName();
                    row["File Name"] = item.FILE_NAME;
                    row["Created Time"] = item.CREATED_TIME;
                    row["Commit File"] = commitFile;
                    row["Send Tiem"] = commitFileSendTime;
                    row["DN#"] = shipmentDN;
                    row["Last Edit Emp"] = item.EDIT_EMP;
                    row["Last Edit Time"] = item.EDIT_TIME;
                    poTable.Rows.Add(row);
                }
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(poTable);
                string fileName = "PoList_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                StationReturn.Data = new { fileName = fileName, fileContent = content };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void GetForecastFileList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                //var fileList = SFCDB.ORM.Queryable<R_VT_FORECAST>().Select(r => new { FileName = r.FILE_NAME, DownloadTime = r.CREATED_TIME })
                //    .Distinct().OrderBy(r=>r.DownloadTime,OrderByType.Desc).ToList();
                var fileList = SFCDB.ORM.Ado.GetDataTable($@"select rownum as no,lot_no,file_name,DOWNLOAD_TIME from (
                                select  distinct lot_no, file_name,created_time as download_time from r_vt_forecast order by created_time desc)");
                StationReturn.Data = fileList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void GetCommitListByOrderId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                if (Data["OrderId"] == null)
                {
                    throw new Exception("OrderId is null");
                }
                string orderId = Data["OrderId"].ToString();
                if (string.IsNullOrWhiteSpace(orderId))
                {
                    throw new Exception("OrderId error.");
                }
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var orderObj = SFCDB.ORM.Queryable<R_VT_ORDER>().Where(r => r.ID == orderId).ToList().FirstOrDefault();

                if (orderObj == null)
                {
                    throw new Exception("Order id error or order is Invalid");
                }
                var list = SFCDB.ORM.Queryable<R_VT_ORDER_COMMIT>().Where(r => r.VT_ORDER_ID == orderId)
                    .OrderBy(r => r.SEND_TIME, OrderByType.Desc)
                    .Select(r => new { r.COMMIT_DETAIL, r.COMMIT_EMP, r.COMMIT_TIME, r.SEND_FLAG, r.SEND_FILE, r.SEND_TIME })
                    .ToList();

                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void CompareForecast(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var lotNoArray = Data["LotNoList"].ToArray();
                if (lotNoArray.Length == 0)
                {
                    throw new Exception("Please input LotNo");
                }
                List<string> lotList = new List<string>();
                for (int i = 0; i < lotNoArray.Length; i++)
                {
                    lotList.Add(lotNoArray[i].ToString());
                }
                StationReturn.Data = CompareForecastByLotNo(SFCDB, lotList);
                //StationReturn.Data = CompareLastTowForecast(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public void CompareAndDownloadForecast(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var lotNoArray = Data["LotNoList"].ToArray();
                if (lotNoArray.Length == 0)
                {
                    throw new Exception("Please input LotNo");
                }
                List<string> lotList = new List<string>();
                for (int i = 0; i < lotNoArray.Length; i++)
                {
                    lotList.Add(lotNoArray[i].ToString());
                }
                DataTable compareData = CompareForecastByLotNo(SFCDB, lotList);
                compareData.TableName = "ForecastCompare";

                string content = MESPubLab.Common.ExcelHelp.MakeVTForecastCompare(compareData, ForecastCompareColumns);
                #region Spire.Xls.Workbook 免費版導出的Excel行數好像有限制，只能導出200行
                //System.IO.MemoryStream ms = new System.IO.MemoryStream();                
                //using (Spire.Xls.Workbook dwonloadFile = new Spire.Xls.Workbook())
                //{
                //    dwonloadFile.Worksheets[0].Name = "ForecastCompare";
                //    for (int i = 0; i < compareData.Rows.Count; i++)
                //    {
                //        for (int j = 0; j < compareData.Columns.Count; j++)
                //        {
                //            dwonloadFile.Worksheets[0].Range[i + 1, j + 1].Text = compareData.Columns[j].ColumnName;
                //            dwonloadFile.Worksheets[0].Range[i + 1, j + 1].Style.HorizontalAlignment = Spire.Xls.HorizontalAlignType.Center;
                //        }
                //    }
                //    System.Drawing.ColorConverter colorConver = new System.Drawing.ColorConverter();

                //    for (int i = 0; i < compareData.Rows.Count; i++)
                //    {
                //        for (int j = 0; j < compareData.Columns.Count; j++)
                //        {
                //            int rowIndex = i + 2;
                //            int columnIncex = j + 1;
                //            if (!ForecastCompareColumns.Contains(compareData.Columns[j].ColumnName))
                //            {
                //                ForecastQty data = Newtonsoft.Json.JsonConvert.DeserializeObject<ForecastQty>(compareData.Rows[i][j].ToString());
                //                dwonloadFile.Worksheets[0].Range[rowIndex, columnIncex].NumberValue = Convert.ToDouble(data.Qty.ToString());
                //                if (!data.IsSame)
                //                {
                //                    dwonloadFile.Worksheets[0].Range[rowIndex, columnIncex].Style.Font.Color = System.Drawing.Color.Red;
                //                    dwonloadFile.Worksheets[0].Range[rowIndex, columnIncex].Style.Interior.Color = (System.Drawing.Color)colorConver.ConvertFromString("#f5eb8f");
                //                }
                //            }
                //            else
                //            {
                //                dwonloadFile.Worksheets[0].Range[rowIndex, columnIncex].Text = compareData.Rows[i][j].ToString();
                //            }
                //            dwonloadFile.Worksheets[0].Range[rowIndex, columnIncex].Style.HorizontalAlignment = Spire.Xls.HorizontalAlignType.Center;
                //        }
                //    }
                //    dwonloadFile.Worksheets[0].AllocatedRange.AutoFitColumns();
                //    //var tttt = dwonloadFile.Worksheets[0].Rows.Count();
                //    //dwonloadFile.SaveToFile($@"D:\ForecastCompare_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
                //    dwonloadFile.SaveToStream(ms);
                //    //var ttftt = dwonloadFile.Worksheets[0].Rows.Count();
                //}
                //byte[] bytes = ms.ToArray();
                //ms.Close();
                //ms.Dispose();
                //string content = Convert.ToBase64String(bytes); 
                #endregion 
                string fileName = "ForecastCompare_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                StationReturn.Data = new { fileName = fileName, fileContent = content };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";                

            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        
        public void CancelShipment(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {               
                var id = Data["ID"].ToString();
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception("Please input ID");
                }
                var sObj = SFCDB.ORM.Queryable<R_VT_SHIPMENT>().Where(r => r.ID == id).ToList().FirstOrDefault();
                if (sObj == null)
                {
                    throw new Exception($@"This shipment [ID:{id}] not exist.");
                }
                DateTime systemdate = SFCDB.ORM.GetDate();
                string dStr = Encoding.Unicode.GetString(sObj.SHIPMENT_DETAIL);
                SHIPMENT_DETAIL_VT shipmentDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(dStr);
                shipmentDetail.Action = "Cancel";
                shipmentDetail.ShipmentId = $@"{shipmentDetail.RefdOrderNumber}-{systemdate.ToString("yyMMddHHmmss")}";

                string detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(shipmentDetail, Newtonsoft.Json.Formatting.Indented);
                R_VT_SHIPMENT shipmentObj = new R_VT_SHIPMENT();
                shipmentObj.ID = MesDbBase.GetNewID<R_VT_SHIPMENT>(SFCDB.ORM, this.BU);
                shipmentObj.SHIPMENT_ID = shipmentDetail.ShipmentId;
                shipmentObj.ORDER_ID = sObj.ORDER_ID;
                shipmentObj.DN_NO = sObj.DN_NO;
                shipmentObj.DN_LINE = sObj.DN_LINE;
                shipmentObj.ACTION = shipmentDetail.Action;
                shipmentObj.CREATED_EMP = LoginUser.EMP_NO;
                shipmentObj.CREATED_TIME = systemdate;
                shipmentObj.VALID_FLAG = "1";
                shipmentObj.SEND_FLAG = "0";
                shipmentObj.SHIPMENT_DETAIL = System.Text.Encoding.Unicode.GetBytes(detailStr);
                SFCDB.ORM.Insertable<R_VT_SHIPMENT>(shipmentObj).ExecuteCommand();

                R_VT_SHIPMENT_CANCEL cObj = new R_VT_SHIPMENT_CANCEL();
                cObj.ID= MesDbBase.GetNewID<R_VT_SHIPMENT_CANCEL>(SFCDB.ORM, this.BU);
                cObj.CANCEL_ID = sObj.ID;
                cObj.NEW_ID = shipmentObj.ID;
                cObj.CREATED_EMP = LoginUser.EMP_NO;
                cObj.CREATED_TIME = systemdate;
                SFCDB.ORM.Insertable<R_VT_SHIPMENT_CANCEL>(cObj).ExecuteCommand();

                sObj.VALID_FLAG = "0";
                SFCDB.ORM.Updateable<R_VT_SHIPMENT>(sObj).Where(r=>r.ID== sObj.ID) .ExecuteCommand();

                SFCDB.ORM.Updateable<R_VT_ORDER>()
                    .SetColumns(r => new R_VT_ORDER
                    {
                        STATUS = EnumExtensions.ExtValue(OrderStatus.CancelASN),
                        EDIT_EMP = LoginUser.EMP_NO,
                        EDIT_TIME = systemdate
                    })
                    .Where(r => r.ID == sObj.ORDER_ID).ExecuteCommand();
                
                SFCDB.CommitTrain();
                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";            

            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        private int SaveCommitOrderInfo(OleExec SFCDB, R_VT_ORDER orderObj, DateTime sysdate, string commitDetail,bool bToSend=true)
        {
            T_R_VT_ORDER_COMMIT t_order_commit = new T_R_VT_ORDER_COMMIT(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            SFCDB.ORM.Updateable<R_VT_ORDER_COMMIT>()
                .SetColumns(r => r.VALID_FLAG == "0")
                .Where(r => r.VT_ORDER_ID == orderObj.ID && r.SEND_FLAG == 0 && r.VALID_FLAG == "1").ExecuteCommand();
            R_VT_ORDER_COMMIT orderCommit = new R_VT_ORDER_COMMIT();
            orderCommit.ID = t_order_commit.GetNewID(BU, SFCDB);
            orderCommit.VT_ORDER_ID = orderObj.ID;
            orderCommit.COMMIT_TIME = sysdate;
            orderCommit.COMMIT_EMP = LoginUser.EMP_NO;
            orderCommit.COMMIT_DETAIL = commitDetail;
            orderCommit.SEND_FLAG = bToSend ? 0 : 1;
            orderCommit.VALID_FLAG = "1";
            return SFCDB.ORM.Insertable<R_VT_ORDER_COMMIT>(orderCommit).ExecuteCommand();
        }

        public void CheckOrderStatus(R_VT_ORDER order)
        {
            if (order.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.WaitForSendShipment)))
            {
                throw new Exception("Already creat shipment,now is waiting for send shipment file.");
            }
            if (order.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Closed)))
            {
                throw new Exception("Already closed.");
            }
            if (order.STATUS.Equals(EnumExtensions.ExtName(OrderStatus.Reject)))
            {
                throw new Exception("Already reject.");
            }
        }

        public string ChangeToF4(string number)
        {
            string n = string.Empty;
            if (!string.IsNullOrEmpty(number) &&
                (System.Text.RegularExpressions.Regex.IsMatch(number, @"^[1-9]\d*|0$") ||
                System.Text.RegularExpressions.Regex.IsMatch(number, @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$")))
            {
                n = Convert.ToDecimal(number).ToString("F4");
            }
            else
            {
                n = "0.0000";
            }
            return n;
        }

        public string StripHTML(string html)
        {
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            string output = regex.Replace(html, "");
            output = output.Replace("<", "");
            output = output.Replace(">", "");
            output = output.Replace("&nbsp;", "");
            return output;
        }

        public void DoCreatShipment(SqlSugarClient DB, R_VT_ORDER orderObj, string dn, string emp)
        {
            DB.Ado.BeginTran();
            try
            {
                if (orderObj == null)
                {
                    throw new Exception("Order id error or order is Invalid");
                }
                if (orderObj.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForSendShipment))
                {
                    throw new Exception("Is wait for send shipment file,please check.");
                }
                var orderCommit = DB.Queryable<R_VT_ORDER_COMMIT>().Where(r => r.VT_ORDER_ID == orderObj.ID && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (orderCommit == null)
                {
                    throw new Exception($@"Purchase Order No.:{orderObj.ORDER_NUMBER},Line ID:{orderObj.ORDER_LINE_ID};Promise ID:{orderObj.PROMISE_ID};Not commit.");
                }
                if (orderCommit.SEND_FLAG == 0)
                {
                    throw new Exception($@"Purchase Order No.:{orderObj.ORDER_NUMBER},Line ID:{orderObj.ORDER_LINE_ID};Promise ID:{orderObj.PROMISE_ID};Commit file not send to B2B.");
                }
                if (orderCommit.SEND_TIME == null)
                {
                    throw new Exception($@"Purchase Order No.:{orderObj.ORDER_NUMBER},Line ID:{orderObj.ORDER_LINE_ID};Promise ID:{orderObj.PROMISE_ID};Commit file send time error. ");
                }

                TimeSpan ts = DB.GetDate() - (DateTime)orderCommit.SEND_TIME;
                //傳輸確認文件后，要等一天后才能生成shipment文件
                //2021.12.15 由原來的一天改成2個小時
                //2021.12.29 由原來的2個小時改成一天
                if (ts.Days < 1)
                {
                    throw new Exception($@"Please create the shipmemnt after commit file has been sent over 1 day. ");
                }

                var dnObj = DB.Queryable<R_DN_STATUS>().Where(r => r.DN_NO == dn).ToList().FirstOrDefault();
                if (dnObj == null)
                {
                    throw new Exception($@"{dn} not exist.");
                }

                var shipedList = DB.Queryable<R_SHIP_DETAIL>().Where(r => r.DN_NO == dnObj.DN_NO && r.DN_LINE == dnObj.DN_LINE).ToList();
                if (shipedList.Count == 0)
                {
                    throw new Exception($@"{dn} not shipping.");
                }

                var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(orderObj.ORDER_DETAIL));
                if (orderDetail.Action.Equals(EnumExtensions.ExtName(OrderAction.Close)))
                {
                    throw new Exception("Already closed.");
                }
                if (orderDetail.Action.Equals(EnumExtensions.ExtName(OrderAction.Reject)))
                {
                    throw new Exception("Already reject.");
                }
                if (!dnObj.SKUNO.Trim().StartsWith(orderDetail.SupplierItemName))
                {
                    throw new Exception($@"Supplier Item No [{orderDetail.SupplierItemName}] not match skuno[{dnObj.SKUNO}].");
                }

                #region 同一PO同一條LINE相同料號和數量promise date早的優先生成ASN
                List<ORDER_DETAIL_VT> detailList = new List<ORDER_DETAIL_VT>();
                var orderList = DB.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == orderObj.ORDER_NUMBER && r.ORDER_LINE_ID == orderObj.ORDER_LINE_ID && r.VALID_FLAG == 1).ToList();
                foreach (var o in orderList)
                {
                    var oDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(o.ORDER_DETAIL));
                    if (oDetail.PromiseQty.Equals(orderDetail.PromiseQty) && oDetail.SupplierItemName.Equals(orderDetail.SupplierItemName))
                    {
                        detailList.Add(oDetail);
                    }
                }
                ORDER_DETAIL_VT fPromise = detailList
                    .Find(r => !r.OrderNumber.Equals(orderDetail.OrderNumber)
                    && !r.SupplierItemName.Equals(orderDetail.SupplierItemName)
                    && Convert.ToDateTime(r.PromiseDate) < Convert.ToDateTime(orderDetail.PromiseDate));
                if (fPromise != null)
                {
                    throw new Exception($@"PO:{fPromise.OrderNumber},
                                        Line:{fPromise.OrderLineId},
                                        ScheduleId:{fPromise.OrderRequestScheduleId},
                                        PromiseId:{fPromise.OrderPromiseId},
                                        PromiseDate:{fPromise.PromiseDate};
                                        Promise date as early as {orderDetail.PromiseDate};Please create ASN for the first promise date.");
                }
                #endregion

                var isUsed = DB.Queryable<R_VT_SHIPMENT>()
                    .Where(r => r.DN_NO == dnObj.DN_NO && !SqlFunc.Subqueryable<R_VT_SHIPMENT_CANCEL>().Where(c => c.CANCEL_ID == r.ID).Any())
                    .Any();
                if (isUsed)
                {
                    throw new Exception($@"DN [{dnObj.DN_NO}] already create shipment.");
                }

                List<R_VT_SHIPMENT> shipmentList = DB.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == orderObj.ID && r.VALID_FLAG == "1").ToList();

                double shippedTotalQty = 0.00;
                if (shipmentList.Count > 0)
                {
                    foreach (var sObj in shipmentList)
                    {
                        string dStr = Encoding.Unicode.GetString(sObj.SHIPMENT_DETAIL);
                        SHIPMENT_DETAIL_VT shipmentDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(dStr);
                        shippedTotalQty = shippedTotalQty + Convert.ToDouble(shipmentDetail.ShippedQuantity);
                    }
                }
                //dnObj.QTY = 5;
                double waitShippingQty = Convert.ToDouble(orderDetail.PromiseQty) - shippedTotalQty;
                if (!Convert.ToDouble(shipedList.Count).Equals(waitShippingQty) || !Convert.ToDouble(dnObj.QTY).Equals(waitShippingQty))
                {
                    throw new Exception($@"Dn qty [{dnObj.QTY}] or shipped qty [{shipedList.Count}] not match balance qty [{waitShippingQty}].");
                }

                DateTime systemdate = DB.GetDate();
                SHIPMENT_DETAIL_VT shipment = new SHIPMENT_DETAIL_VT();
                shipment.ShipmentId = $@"{orderDetail.OrderNumber}-{systemdate.ToString("yyMMddHHmmss")}";//待定
                #region 先從PO中對應的FIELD_NAME或DISPLAY_NAME取值
                var orderType = orderDetail.GetType();//获得类型  
                var shipmentType = typeof(SHIPMENT_DETAIL_VT);
                List<C_CUSTOMER_FILE_FORMAT> sFormatList = DB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                   .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMShipment")
                  .OrderBy(c => c.SEQ).ToList();

                if (sFormatList.Count == 0)
                {
                    throw new Exception("MTIMShipment not setting C_CUSTOMER_FILE_FORMAT");
                }

                List<C_CUSTOMER_FILE_FORMAT> oFormatList = DB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                    .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                    .ToList();

                foreach (PropertyInfo pOrder in orderType.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo pShipment in shipmentType.GetProperties())
                    {
                        if (pShipment.Name == pOrder.Name && pShipment.PropertyType == pOrder.PropertyType)//判断属性名是否相同  
                        {
                            pShipment.SetValue(shipment, pOrder.GetValue(orderDetail, null), null);//获得s对象属性的值复制给d对象的属性  
                        }
                        else
                        {
                            var sFormat = sFormatList.Find(s => s.FIELD_NAME == pShipment.Name);
                            var oFormat = oFormatList.Find(s => s.FIELD_NAME == pOrder.Name);
                            if (sFormat != null && oFormat != null && sFormat.DISPLAY_NAME == oFormat.DISPLAY_NAME)
                            {
                                pShipment.SetValue(shipment, pOrder.GetValue(orderDetail, null), null);//获得s对象属性的值复制给d对象的属性  
                            }
                        }
                        //只按DISPLAY_NAME來取值
                        //var sFormat = sFormatList.Find(s => s.FIELD_NAME == pShipment.Name);
                        //var oFormat = oFormatList.Find(s => s.FIELD_NAME == pOrder.Name);
                        //if (sFormat != null && oFormat != null && sFormat.DISPLAY_NAME == oFormat.DISPLAY_NAME)
                        //{
                        //    pShipment.SetValue(shipment, pOrder.GetValue(orderDetail, null), null);//获得s对象属性的值复制给d对象的属性  
                        //}
                    }
                }
                #endregion

                #region 以下為固定值
                shipment.SupplierAddressDescriptor = "";
                shipment.SupplierAddressCountry = "";
                shipment.SupplierAddressCity = "";
                shipment.SupplierAddressState = "";
                shipment.ShipmentCreationDate = systemdate.ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                shipment.ShipmentCreatorCode = "Foxconn01";
                shipment.ShipmentStatus = "";
                shipment.ShipmentDate = Convert.ToDateTime(dnObj.CREATETIME).ToString("yyyy-MM-ddTHH:mm:ss+ffff");
                shipment.PlannedDeliveryDate = "";
                shipment.ETAAtFinalSite = "";
                shipment.CarrierName = "VT nominated FWD";
                shipment.ShipmentMode = "VT instructed way";
                shipment.CustomerItemName = orderDetail.CustomerItemName;
                //Weight UOM 固定為空
                shipment.FlexAttrStringHeader8 = "";
                shipment.ShipToLocationType = "Site";
                shipment.CountryOfOrigin = "China";
                shipment.ShipFromAddressCity = "";
                shipment.ShipFromAddressCountry = "";
                shipment.BillToName = "";
                shipment.ShipmentLineItemId = dnObj.DN_NO;
                shipment.ShipmentLineStatus = "";
                shipment.Action = "InsertOrUpdate";
                shipment.ShippedQuantity = ChangeToF4(dnObj.QTY.ToString());// orderDetail.PromiseQty;
                shipment.RefdOrderType = "DiscreteOrder";
                shipment.RefdOrderNumber = orderDetail.OrderNumber;
                shipment.RefdOrderLineId = orderDetail.OrderLineId;
                shipment.RefdOrderScheduleId = orderDetail.OrderRequestScheduleId;
                shipment.RefdOrderPromiseScheduleId = orderDetail.OrderPromiseId;
                shipment.FlexAttrIntDelivery3 = "";
                shipment.FlexAttrIntDelivery4 = "";
                #endregion


                string detailStr = Newtonsoft.Json.JsonConvert.SerializeObject(shipment, Newtonsoft.Json.Formatting.Indented);
                R_VT_SHIPMENT shipmentObj = new R_VT_SHIPMENT();
                shipmentObj.ID = MesDbBase.GetNewID<R_VT_SHIPMENT>(DB, this.BU);
                shipmentObj.SHIPMENT_ID = shipment.ShipmentId;
                shipmentObj.ORDER_ID = orderObj.ID;
                shipmentObj.DN_NO = dnObj.DN_NO;
                shipmentObj.DN_LINE = dnObj.DN_LINE;
                shipmentObj.ACTION = shipment.Action;
                shipmentObj.CREATED_EMP = emp;
                shipmentObj.CREATED_TIME = systemdate;
                shipmentObj.VALID_FLAG = "1";
                shipmentObj.SEND_FLAG = "0";
                shipmentObj.SHIPMENT_DETAIL = System.Text.Encoding.Unicode.GetBytes(detailStr);
                DB.Insertable<R_VT_SHIPMENT>(shipmentObj).ExecuteCommand();

                orderObj.EDIT_TIME = systemdate;
                orderObj.EDIT_EMP = emp;
                orderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForSendShipment);
                DB.Updateable<R_VT_ORDER>(orderObj).Where(r => r.ID == orderObj.ID).ExecuteCommand();
                DB.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                DB.Ado.RollbackTran();
                throw ex;
            }

        }

        List<string> ForecastCompareColumns = new List<string>() { "LOT_NO", "FILE_NAME", "SITE_NAME","SKUNO" };
        public DataTable CompareLastTowForecast(OleExec SFCDB)
        {

            DateTime now = SFCDB.ORM.GetDate();
            var fileNameList = SFCDB.ORM.Queryable<R_VT_FORECAST>().Where(r => r.CREATED_TIME > now.AddDays(-60))
               .OrderBy(r => r.CREATED_TIME, OrderByType.Desc).Select(r => r.FILE_NAME).ToList();
            fileNameList = fileNameList.Distinct().ToList();
            if (fileNameList.Count == 0)
            {
                throw new Exception("No data.");
            }
            string firstFileName = "";
            string firstLotNo = "";
            string secondFileName = "";
            string secondLotNo = "";
            string sql = "";
            DataTable tableFileName = new DataTable();
            if (fileNameList.Count == 1)
            {
                firstFileName = secondFileName = fileNameList[0];
                sql = $@"select * from (
                        select lot_no, file_name, row_number() over(partition by file_name order by CREATED_TIME desc) as rownumber from(
                         select distinct lot_no, file_name, created_time from r_vt_forecast where file_name = '{fileNameList[0]}'
                         )) where rownumber = 1 ";
                tableFileName = SFCDB.ORM.Ado.GetDataTable(sql);
                firstLotNo = secondLotNo = tableFileName.Rows[0]["LOT_NO"].ToString();


            }
            else
            {
                firstFileName = fileNameList[0];
                secondFileName = fileNameList[1];
                sql = $@"select * from (
                        select lot_no, file_name, row_number() over(partition by file_name order by CREATED_TIME desc) as rownumber from(
                         select distinct lot_no, file_name, created_time from r_vt_forecast where file_name = '{fileNameList[0]}'
                         )) where rownumber = 1 ";
                tableFileName = SFCDB.ORM.Ado.GetDataTable(sql);
                firstLotNo = tableFileName.Rows[0]["LOT_NO"].ToString();
                sql = $@"select * from (
                        select lot_no, file_name, row_number() over(partition by file_name order by CREATED_TIME desc) as rownumber from(
                         select distinct lot_no, file_name, created_time from r_vt_forecast where file_name = '{fileNameList[1]}'
                         )) where rownumber = 1 ";
                tableFileName = SFCDB.ORM.Ado.GetDataTable(sql);
                secondLotNo = tableFileName.Rows[0]["LOT_NO"].ToString();
            }

            List<R_VT_FORECAST> firstList = SFCDB.ORM.Queryable<R_VT_FORECAST>().Where(r => r.LOT_NO == firstLotNo).ToList();
            List<R_VT_FORECAST> secondList = SFCDB.ORM.Queryable<R_VT_FORECAST>().Where(r => r.LOT_NO == secondLotNo).ToList();

            var firstSiteList = firstList.Select(r => r.SITE_NAME).Distinct().ToList();
            var secondSiteList = secondList.Select(r => r.SITE_NAME).Distinct().ToList();
            var siteList = new List<string>();
            siteList.AddRange(firstSiteList);
            siteList.AddRange(secondSiteList);
            siteList = siteList.Distinct().OrderBy(r => r).ToList();

            var firstDateList = firstList.Select(r => r.FORECAST_DATE).Distinct().ToList();
            var secondDateList = secondList.Select(r => r.FORECAST_DATE).Distinct().ToList();
            var dateList = new List<string>();
            dateList.AddRange(firstDateList);
            dateList.AddRange(secondDateList);
            dateList = dateList.Distinct().OrderBy(r => r).ToList();

            var firstSkuList = firstList.Select(r => r.SUPPLIER_ITEM_NAME).Distinct().ToList();
            var secondSkuList = secondList.Select(r => r.SUPPLIER_ITEM_NAME).Distinct().ToList();
            var skuList = new List<string>();
            skuList.AddRange(firstSkuList);
            skuList.AddRange(secondSkuList);
            skuList = skuList.Distinct().OrderBy(r => r).ToList();
            DataTable showTable = new DataTable();
            foreach (var item in ForecastCompareColumns)
            {
                showTable.Columns.Add(item);
            }
            foreach (var date in dateList)
            {
                showTable.Columns.Add((Convert.ToDateTime(date)).ToString("yyyy/MM/dd"));
            }
            foreach (var site in siteList)
            {
                foreach (var skuno in skuList)
                {
                    DataRow firstRow = showTable.NewRow();
                    DataRow secondRow = showTable.NewRow();
                    firstRow["SITE_NAME"] = site;
                    firstRow["LOT_NO"] = firstLotNo;
                    firstRow["FILE_NAME"] = firstFileName;
                    firstRow["SKUNO"] = skuno;

                    secondRow["SITE_NAME"] = site;
                    secondRow["LOT_NO"] = secondLotNo;
                    secondRow["FILE_NAME"] = secondFileName;
                    secondRow["SKUNO"] = skuno;
                    foreach (var date in dateList)
                    {
                        var first = firstList.Find(r => r.SITE_NAME == site && r.SUPPLIER_ITEM_NAME == skuno && r.FORECAST_DATE == date);
                        var second = secondList.Find(r => r.SITE_NAME == site && r.SUPPLIER_ITEM_NAME == skuno && r.FORECAST_DATE == date);
                        firstRow[(Convert.ToDateTime(date)).ToString("yyyy/MM/dd")] = first == null ? 0 : first.QUANTITY;
                        secondRow[(Convert.ToDateTime(date)).ToString("yyyy/MM/dd")] = second == null ? 0 : second.QUANTITY;
                    }

                    showTable.Rows.Add(secondRow);
                    showTable.Rows.Add(firstRow);
                }
            }
            return showTable;
        }

        public DataTable CompareForecastByLotNo(OleExec SFCDB, List<string> lotNoList)
        {
            Dictionary<string, List<R_VT_FORECAST>> dicCompareData = new Dictionary<string, List<R_VT_FORECAST>>();
            foreach (var lotNo in lotNoList)
            {
                dicCompareData.Add(lotNo, SFCDB.ORM.Queryable<R_VT_FORECAST>().Where(r => r.LOT_NO == lotNo).ToList());
            }
            var siteList = new List<string>();
            var dateList = new List<string>();
            var skuList = new List<string>();
            foreach (var compareData in dicCompareData)
            {
                var lotSite = compareData.Value.Select(r => r.SITE_NAME).Distinct().ToList();
                siteList.AddRange(lotSite);
                var lotData = compareData.Value.Select(r => r.FORECAST_DATE).Distinct().ToList();
                dateList.AddRange(lotData);
                var lotSku = compareData.Value.Select(r => r.SUPPLIER_ITEM_NAME).Distinct().ToList();
                skuList.AddRange(lotSku);
            }
            siteList = siteList.Distinct().OrderBy(r => r).ToList();
            dateList = dateList.Distinct().OrderBy(r => r).ToList();
            skuList = skuList.Distinct().OrderBy(r => r).ToList();

            DataTable showTable = new DataTable();
            showTable.TableName = "ForecastCompare";
            foreach (var item in ForecastCompareColumns)
            {
                showTable.Columns.Add(item);
            }
            foreach (var date in dateList)
            {
                showTable.Columns.Add((Convert.ToDateTime(date)).ToString("yyyy/MM/dd"));
            }
            foreach (var site in siteList)
            {
                foreach (var skuno in skuList)
                {
                    foreach (var compare in dicCompareData)
                    {
                        DataRow row = showTable.NewRow();
                        row["SITE_NAME"] = site;
                        row["LOT_NO"] = compare.Key;
                        row["FILE_NAME"] = compare.Value.First().FILE_NAME;
                        row["SKUNO"] = skuno;

                        //var tt = compare.Value.Select(r=>r.QUANTITY).Sum();

                        foreach (var date in dateList)
                        {
                            var dateQty = compare.Value.Find(r => r.SITE_NAME == site && r.SUPPLIER_ITEM_NAME == skuno && r.FORECAST_DATE == date);
                            row[(Convert.ToDateTime(date)).ToString("yyyy/MM/dd")] = dateQty == null ? 0 : dateQty.QUANTITY;
                        }
                        showTable.Rows.Add(row);
                    }
                }
            }
            
            foreach (var site in siteList)
            {
                foreach (var skuno in skuList)
                {
                    var qtyRow = showTable.AsEnumerable()
                        .Where(d => d.Field<string>("SKUNO") == skuno && d.Field<string>("SITE_NAME") == site)
                        .ToList();
                    foreach (var date in dateList)
                    {
                        var dateStr = (Convert.ToDateTime(date)).ToString("yyyy/MM/dd");
                        var dateRow = qtyRow.Select(d => d.Field<string>(dateStr));
                        var allValue = qtyRow.Select(d => d.Field<string>(dateStr)).Distinct().ToList();
                        if (allValue.Count == 1)
                        {
                            foreach (var row in qtyRow)
                            {
                                row[dateStr] = Newtonsoft.Json.JsonConvert.SerializeObject(new ForecastQty { IsSame = true, Qty = row[dateStr].ToString() });
                            }
                        }
                        else
                        {
                            foreach (var row in qtyRow)
                            {
                                row[dateStr] = Newtonsoft.Json.JsonConvert.SerializeObject(new ForecastQty { IsSame = false, Qty = row[dateStr].ToString() });
                            }
                        }
                    }
                }
            }

            return showTable;
        }
    }

    public class ForecastQty
    {
        public bool IsSame { get; set; }
        public string Qty  { get; set; }
    }
}

