using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using System.Data;
using System.Reflection;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module.Juniper;
using MESJuniper.Base;
using SqlSugar;
using MESDataObject.Module.OM;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.Common;
using Newtonsoft.Json.Linq;
using static MESJuniper.Base.SynAck;
using static MESDataObject.Common.EnumExtensions;
using System.Threading;
using MESPubLab.Json;
using MESPubLab.MesException;
using System.Web;
using MESPubLab.MesBase;

namespace MESJuniper.Api
{
    public class OrderManager : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
        protected APIInfo FGetOrderList = new APIInfo
        {
            FunctionName = "GetOrderList",
            Description = "Get Order List",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetSingleOrderInfo = new APIInfo
        {
            FunctionName = "GetSingleOrderInfo",
            Description = "GetSingleOrderInfo",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetStatusMaps = new APIInfo
        {
            FunctionName = "GetStatusMaps",
            Description = "GetStatusMaps",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetPreI138 = new APIInfo
        {
            FunctionName = "GetPreI138",
            Description = "GetPreI138",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetI138ReasonCode = new APIInfo
        {
            FunctionName = "GetI138ReasonCode",
            Description = "GetI138ReasonCode",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddI138Data = new APIInfo
        {
            FunctionName = "AddI138Data",
            Description = "AddI138Data",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGet138List = new APIInfo
        {
            FunctionName = "Get138List",
            Description = "Get138List",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUpload138List = new APIInfo
        {
            FunctionName = "Upload138List",
            Description = "Upload138List",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetOrderAlartList = new APIInfo
        {
            FunctionName = "GetOrderAlartList",
            Description = "GetOrderAlartList",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetAgileList = new APIInfo
        {
            FunctionName = "GetAgileList",
            Description = "GetAgileList",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetPreSap = new APIInfo
        {
            FunctionName = "GetPreSap",
            Description = "GetPreSap",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetPreWo = new APIInfo
        {
            FunctionName = "GetPreWo",
            Description = "GetPreWo",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetItem054 = new APIInfo
        {
            FunctionName = "GetItem054",
            Description = "GetItem054",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetItem138 = new APIInfo
        {
            FunctionName = "GetItem138",
            Description = "GetItem138",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGet137HeadByPo = new APIInfo
        {
            FunctionName = "Get137HeadByPo",
            Description = "Get137HeadByPo",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGet137DataByTranID = new APIInfo
        {
            FunctionName = "Get137DataByTranID",
            Description = "Get137DataByTranID",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGet139and282ByMainid = new APIInfo
        {
            FunctionName = "Get139and282ByMainid",
            Description = "Get139and282ByMainid",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGet244ByMainid = new APIInfo
        {
            FunctionName = "Get244ByMainid",
            Description = "Get244ByMainid",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetOrderSnByMainid = new APIInfo
        {
            FunctionName = "GetOrderSnByMainid",
            Description = "GetOrderSnByMainid",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetOrderListByPOList = new APIInfo
        {
            FunctionName = "GetOrderListByPOList",
            Description = "GetOrderListByPOList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="POLIST",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FResetPo = new APIInfo
        {
            FunctionName = "ResetPo",
            Description = "ResetPo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="UPOIDLIST",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetWOAutoKPList = new APIInfo
        {
            FunctionName = "GetWOAutoKPList",
            Description = "GetWOAutoKPList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="WO",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateWOAutoKP = new APIInfo
        {
            FunctionName = "UpdateWOAutoKP",
            Description = "UpdateWOAutoKP",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="OLD_ROW",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="NEW_ROW",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FResetOrderWo = new APIInfo
        {
            FunctionName = "ResetOrderWo",
            Description = "ResetOrderWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="UPOIDLIST",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetDoaOrder = new APIInfo
        {
            FunctionName = "GetDoaOrder",
            Description = "GetDoaOrder",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCreateDoaOrder = new APIInfo
        {
            FunctionName = "CreateDoaOrder",
            Description = "CreateDoaOrder",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetSkuPlantCode = new APIInfo
        {
            FunctionName = "GetSkuPlantCode",
            Description = "GetSkuPlantCode",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCreateNewSkuPlantCode = new APIInfo
        {
            FunctionName = "CreateNewSkuPlantCode",
            Description = "CreateNewSkuPlantCode",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDelSkuPlantCode = new APIInfo
        {
            FunctionName = "DelSkuPlantCode",
            Description = "DelSkuPlantCode",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddReplacePnByPoLine = new APIInfo
        {
            FunctionName = "AddReplacePnByPoLine",
            Description = "AddReplacePnByPoLine",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="POID",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="NEW_ROW",InputType="string",DefaultValue="" }                
            },
            Permissions = new List<MESPermission>()
        };

        
        public OrderManager()
        {
            this.Apis.Add(FGetOrderList.FunctionName, FGetOrderList);
            this.Apis.Add(FGetSingleOrderInfo.FunctionName, FGetSingleOrderInfo);
            this.Apis.Add(FGetPreI138.FunctionName, FGetPreI138);
            this.Apis.Add(FGetI138ReasonCode.FunctionName, FGetI138ReasonCode);
            this.Apis.Add(FAddI138Data.FunctionName, FAddI138Data);
            this.Apis.Add(FGetStatusMaps.FunctionName, FGetStatusMaps);
            this.Apis.Add(FGet138List.FunctionName, FGet138List);
            this.Apis.Add(FUpload138List.FunctionName, FUpload138List);
            this.Apis.Add(FGetOrderAlartList.FunctionName, FGetOrderAlartList);
            this.Apis.Add(FGetAgileList.FunctionName, FGetAgileList);
            this.Apis.Add(FGetPreSap.FunctionName, FGetPreSap);
            this.Apis.Add(FGetPreWo.FunctionName, FGetPreWo);
            this.Apis.Add(FGetItem054.FunctionName, FGetItem054);
            this.Apis.Add(FGetItem138.FunctionName, FGetItem138);
            this.Apis.Add(FGet137HeadByPo.FunctionName, FGet137HeadByPo);
            this.Apis.Add(FGet137DataByTranID.FunctionName, FGet137DataByTranID);
            this.Apis.Add(FGet139and282ByMainid.FunctionName, FGet139and282ByMainid);
            this.Apis.Add(FGet244ByMainid.FunctionName, FGet244ByMainid);
            this.Apis.Add(FGetOrderSnByMainid.FunctionName, FGetOrderSnByMainid);
            this.Apis.Add(FGetOrderListByPOList.FunctionName, FGetOrderListByPOList);
            this.Apis.Add(FResetPo.FunctionName, FResetPo);
            this.Apis.Add(FGetWOAutoKPList.FunctionName, FGetWOAutoKPList);
            this.Apis.Add(FUpdateWOAutoKP.FunctionName, FUpdateWOAutoKP);
            this.Apis.Add(FResetOrderWo.FunctionName, FResetOrderWo);
            this.Apis.Add(FGetDoaOrder.FunctionName, FGetDoaOrder);
            this.Apis.Add(FGetSkuPlantCode.FunctionName, FGetSkuPlantCode);
            this.Apis.Add(FCreateNewSkuPlantCode.FunctionName, FCreateNewSkuPlantCode);
            this.Apis.Add(FDelSkuPlantCode.FunctionName, FDelSkuPlantCode);
        }

        public void ResetPo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var upoidlist = Data["UPOIDLIST"].ToString().ToUpper().Trim();
                var targetlist = upoidlist.Split(',');
                int resnum = 0, restolnum = 0;
                var resstr = "";
                foreach (var iupoid in targetlist)
                {
                    var upoid = iupoid.Trim();
                    restolnum++;
                    if (upoid.Length == 0)
                        continue;
                    var res = sfcdb.ORM.Ado.UseTran(() =>
                      {
                          var mainobj = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.UPOID == upoid).ToList().FirstOrDefault();
                          var poid = mainobj?.ID;
                          var wo = mainobj?.PREWO;
                          if (mainobj == null)
                              return;
                          sfcdb.ORM.Deleteable<O_ORDER_MAIN_H>().Where(t => t.UPOID == upoid).ExecuteCommand();
                          sfcdb.ORM.Deleteable<O_ORDER_HOLD>().Where(t => t.UPOID == upoid).ExecuteCommand();
                          sfcdb.ORM.Deleteable<O_ORDER_CHANGELOG>().Where(t => t.UPOID == upoid).ExecuteCommand();
                          sfcdb.ORM.Updateable<O_EXCEPTION_DATA>().SetColumns(t => new O_EXCEPTION_DATA() { STATUS = JuniperErrStatus.Close.ExtValue() }).Where(t => t.UPOID == upoid).ExecuteCommand();
                          sfcdb.ORM.Deleteable<O_ORDER_MAIN>().Where(t => t.UPOID == upoid).ExecuteCommand();
                          sfcdb.ORM.Updateable<O_I137_ITEM>().SetColumns(t => new O_I137_ITEM() { MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue() }).Where(t => SqlFunc.MergeString(t.PONUMBER, t.ITEM) == upoid).ExecuteCommand();
                          if (poid != null)
                          {
                              sfcdb.ORM.Deleteable<R_PRE_WO_HEAD>().Where(t => t.MAINID == poid).ExecuteCommand();
                              sfcdb.ORM.Deleteable<O_ORDER_OPTION>().Where(t => t.MAINID == poid).ExecuteCommand();
                              sfcdb.ORM.Deleteable<O_PO_STATUS>().Where(t => t.POID == poid).ExecuteCommand();
                          }
                          if (wo != null)
                          {
                              sfcdb.ORM.Deleteable<R_PRE_WO_DETAIL>().Where(t => t.WO == wo).ExecuteCommand();
                              sfcdb.ORM.Deleteable<R_WO_GROUPID>().Where(t => t.WO == wo).ExecuteCommand();
                              sfcdb.ORM.Deleteable<R_SAP_AS_BOM>().Where(t => t.WO == wo).ExecuteCommand();
                              sfcdb.ORM.Deleteable<R_SAP_HB>().Where(t => t.WO == wo).ExecuteCommand();
                              sfcdb.ORM.Deleteable<R_SAP_PODETAIL>().Where(t => t.WO == wo).ExecuteCommand();
                              sfcdb.ORM.Updateable<R_ORDER_WO>().SetColumns(t => new R_ORDER_WO() { VALID = MesBool.No.ExtValue() }).Where(t => t.WO == wo).ExecuteCommand();
                          }
                          resstr += upoid + ",";
                          resnum++;
                      });
                    if (!res.IsSuccess)
                        throw res.ErrorException;
                }
                StationReturn.Data = $@"Tolnum: {restolnum} ,success num: {resnum}, detail: {resstr}";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void ResetOrderWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var upoidlist = Data["UPOIDLIST"].ToString().ToUpper().Trim();
                var targetlist = upoidlist.Split(',');
                int resnum = 0, restolnum = 0;
                var resstr = "";
                foreach (var upoid in targetlist)
                {
                    restolnum++;
                    if (upoid.Length == 0)
                        continue;
                    var res = sfcdb.ORM.Ado.UseTran(() =>
                    {
                        var mainobj = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.UPOID == upoid).ToList().FirstOrDefault();
                        var poid = mainobj?.ID;
                        var wo = mainobj?.PREWO;
                        if (mainobj == null)
                            return;
                        sfcdb.ORM.Deleteable<O_ORDER_MAIN_H>().Where(t => t.UPOID == upoid).ExecuteCommand();
                        sfcdb.ORM.Deleteable<O_ORDER_HOLD>().Where(t => t.UPOID == upoid).ExecuteCommand();
                        sfcdb.ORM.Deleteable<O_ORDER_CHANGELOG>().Where(t => t.UPOID == upoid).ExecuteCommand();
                        sfcdb.ORM.Updateable<O_EXCEPTION_DATA>().SetColumns(t => new O_EXCEPTION_DATA() { STATUS = JuniperErrStatus.Close.ExtValue() }).Where(t => t.UPOID == upoid).ExecuteCommand();
                        sfcdb.ORM.Deleteable<O_ORDER_MAIN>().Where(t => t.UPOID == upoid).ExecuteCommand();
                        sfcdb.ORM.Updateable<O_I137_ITEM>().SetColumns(t => new O_I137_ITEM() { MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue() }).Where(t => SqlFunc.MergeString(t.PONUMBER, t.ITEM) == upoid).ExecuteCommand();
                        if (poid != null)
                        {
                            sfcdb.ORM.Deleteable<R_PRE_WO_HEAD>().Where(t => t.MAINID == poid).ExecuteCommand();
                            sfcdb.ORM.Deleteable<O_ORDER_OPTION>().Where(t => t.MAINID == poid).ExecuteCommand();
                            sfcdb.ORM.Deleteable<O_PO_STATUS>().Where(t => t.POID == poid).ExecuteCommand();
                        }
                        if (wo != null)
                        {
                            sfcdb.ORM.Deleteable<R_PRE_WO_DETAIL>().Where(t => t.WO == wo).ExecuteCommand();
                            sfcdb.ORM.Deleteable<R_WO_GROUPID>().Where(t => t.WO == wo).ExecuteCommand();
                            sfcdb.ORM.Deleteable<R_SAP_AS_BOM>().Where(t => t.WO == wo).ExecuteCommand();
                            sfcdb.ORM.Deleteable<R_SAP_HB>().Where(t => t.WO == wo).ExecuteCommand();
                            sfcdb.ORM.Deleteable<R_SAP_PODETAIL>().Where(t => t.WO == wo).ExecuteCommand();
                            sfcdb.ORM.Updateable<R_ORDER_WO>().SetColumns(t => new R_ORDER_WO() { VALID = MesBool.No.ExtValue() }).Where(t => t.WO == wo).ExecuteCommand();
                        }
                        resstr += upoid + ",";
                        resnum++;
                    });
                    if (!res.IsSuccess)
                        throw res.ErrorException;
                }
                StationReturn.Data = $@"Tolnum: {restolnum} ,success num: {resnum}, detail: {resstr}";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetOrderList_FileStr(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {           
            try
            {
                var plant = Data["plant"].ToString().Trim();
                var potype = Data["potype"].ToString().Trim();         
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(GerOrderListData(plant, potype, ""));
                string fileName = "JnpOrderListInfos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                StationReturn.Data = new { content = content, fileName= fileName };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
        }

        List<OrderModle> GerOrderListData(string plant,string potype, JToken jpostatus)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var postatus = jpostatus.ToString().Trim();
                JArray array = postatus == "" ? new JArray("ALL") : (JArray)jpostatus;

                string[] poStatusArray = new Func<string[]>(() =>
                {
                    string[] temp = new string[array.Count];
                    for (int i = 0; i < array.Count; i++)
                    {
                        temp[i] = array[i].ToString();
                    }
                    return temp;
                })();

                sfcdb.ThrowSqlExeception = true;
                var orderList = sfcdb.ORM.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J, O_ORDER_HOLD, I137_I, I137_H, O_EXCEPTION_DATA, R_WO_GROUPID>((m, s, j, h, i, oh, e, f) =>
                       new object[] { JoinType.Left,m.ID == s.POID  && s.VALIDFLAG == MesBool.Yes.ExtValue() ,JoinType.Left,s.STATUSID==j.NAME,JoinType.Left,m.ITEMID==h.ITEMID,
                          JoinType.Left, m.ITEMID == i.ID,JoinType.Left, i.TRANID==oh.TRANID, JoinType.Left,m.UPOID==e.UPOID && e.STATUS == JuniperErrStatus.Open.ExtValue(),
                      JoinType.Left,m.PREWO == f.WO})
                    .Where((m, s, j, h, i, oh, e, f) => m.CUSTOMER == Customer.JUNIPER.ExtValue())
                    .WhereIF(plant != "ALL", (m, s, j, h, i, oh, e, f) => m.PLANT == plant)
                    .WhereIF(potype != "ALL", (m, s, j, h, i, oh, e, f) => m.POTYPE == potype)
                    .WhereIF((poStatusArray.Length == 1 && poStatusArray[0] == "Special"), (m, s, j, h, i, oh, e, f) =>
                        oh.SHIPPINGNOTE.ToUpper().Contains("DEV-")
                        || oh.SHIPPINGNOTE.ToUpper().Contains("ECO-")
                        || oh.SHIPPINGNOTE.ToUpper().Contains("MPM")
                        || oh.SHIPPINGNOTE.ToUpper().Contains("DEVIATION")
                        || (new string[] { "Z09", "Z10", "ZA3" }).Contains(oh.ECO_FCO))
                    .WhereIF((poStatusArray.Length == 1 && poStatusArray[0] != "ALL" && poStatusArray[0] != "Special"), (m, s, j, h, i, oh, e, f) => SqlFunc.ContainsArray(poStatusArray, j.DESCRIPTION))
                    .WhereIF((poStatusArray.Length != 1), (m, s, j, h, i, oh, e, f) => SqlFunc.ContainsArray(poStatusArray, j.DESCRIPTION))
                        .OrderBy((m, s, j, h, i, oh, e, f) => m.PONO, OrderByType.Asc)
                    .Select((m, s, j, h, i, oh, e, f) =>
                        new OrderModle
                        {
                            ID = m.ID,
                            PLANT = m.PLANT,
                            HEADERSCHSTATUS = oh.HEADERSCHEDULINGSTATUS,
                            LINESCHSTATUS = i.LINESCHEDULINGSTATUS,
                            COMPLETEDELIVER = oh.COMPLETEDELIVERY,
                            UPOID = m.UPOID,
                            POTYPE = m.POTYPE,
                            PONO = m.PONO,
                            POLINE = m.POLINE,
                            VERSION = m.VERSION,
                            POSTATUS = j.DESCRIPTION,
                            HOLD = h.HOLDREASON,
                            QTY = m.QTY,
                            UNITPRICE = m.UNITPRICE,
                            PREWO = m.PREWO,
                            GROUPID = f.GROUPID,
                            PID = m.PID,
                            JUNIPERPID = i.PN,
                            DELIVERY = m.DELIVERY,
                            CRSD = i.CUSTREQSHIPDATE,
                            SO = oh.SALESORDERNUMBER,
                            SOLN = i.SALESORDERLINEITEM,
                            USERITEMTYPE = m.USERITEMTYPE,
                            OFFERINGTYPE = m.OFFERINGTYPE,
                            LASTCHANGETIME = m.LASTCHANGETIME,
                            COMPLETED = m.COMPLETED,
                            COMPLETIME = m.COMPLETIME,
                            CLOSED = m.CLOSED,
                            CLOSETIME = m.CLOSETIME,
                            CANCEL = m.CANCEL,
                            CANCELTIME = m.CANCELTIME,
                            CUSTOMSW = i.SWTYPE,
                            ECO_FCO = oh.ECO_FCO,
                            COUNTRYSPECIFICLABE = i.COUNTRYSPECIFICLABEL,
                            CARTONLABEL1 = i.CARTONLABEL1,
                            CARTONLABEL2 = i.CARTONLABEL2,
                            PACKOUTLABEL = i.PACKOUTLABEL,
                            CREATETIME = m.CREATETIME,
                            EDITTIME = m.EDITTIME,
                            ORIGINALID = m.ORIGINALID,
                            ORIGINALITEMID = m.ORIGINALITEMID,
                            ITEMID = m.ITEMID,
                            ORDERTYPE = m.ORDERTYPE,
                            EXCEPTIONINFO = e.EXCEPTIONINFO
                        }).PartitionBy(m => m.UPOID).Distinct().ToList();                

                var plantobj = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "SYSPLANT" && t.FUNCTIONTYPE == "NOSYSTEM").ToList().FirstOrDefault();
                var threads = 20;
                var count = orderList.Count;
                var mCount = count % threads;
                var eachCount = (count - mCount) / threads;
                var manualEvents = new List<ManualResetEvent>();
                var res = new List<OrderModle>();
                for (int i = 0; i < threads; i++)
                {
                    ManualResetEvent mre = new ManualResetEvent(false);
                    manualEvents.Add(mre);
                    var orders = new List<OrderModle>();
                    if (i == threads - 1)
                    {
                        orders = orderList.GetRange(i * eachCount, eachCount + mCount);
                    }
                    else
                    {
                        orders = orderList.GetRange(i * eachCount, eachCount);
                    }
                    res.AddRange(orders);
                    ThreadPool.QueueUserWorkItem((a) =>
                    {
                        TT(orders, plantobj);
                        ManualResetEvent e = (ManualResetEvent)a;
                        e.Set();
                    }, mre);
                }
                WaitHandle.WaitAll(manualEvents.ToArray());//watting all thread
                return res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        
        public void GetOrderList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {          
            try
            {
                var plant = Data["plant"].ToString().Trim();
                var potype = Data["potype"].ToString().Trim();
                StationReturn.Data = GerOrderListData(plant, potype, Data["postatus"]);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
        }

        //public void GetOrderList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        //{
        //    OleExec sfcdb = null;
        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        var plant = Data["plant"].ToString().Trim();
        //        var potype = Data["potype"].ToString().Trim();
        //        var postatus = Data["postatus"].ToString().Trim();
        //        JArray array = Data["postatus"].ToString().Trim() == "" ? new JArray("ALL") : (JArray)Data["postatus"];

        //        string[] poStatusArray = new Func<string[]>(() =>
        //        {
        //            string[] temp = new string[array.Count];
        //            for (int i = 0; i < array.Count; i++)
        //            {
        //                temp[i] = array[i].ToString();
        //            }
        //            return temp;
        //        })();

        //        sfcdb.ThrowSqlExeception = true;
        //        var orderList = sfcdb.ORM.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J, O_ORDER_HOLD, I137_I, I137_H, O_EXCEPTION_DATA, R_WO_GROUPID>((m, s, j, h, i, oh, e, f) =>
        //               new object[] { JoinType.Left,m.ID == s.POID  && s.VALIDFLAG == MesBool.Yes.ExtValue() ,JoinType.Left,s.STATUSID==j.NAME,JoinType.Left,m.ITEMID==h.ITEMID,
        //                  JoinType.Left, m.ITEMID == i.ID,JoinType.Left, i.TRANID==oh.TRANID, JoinType.Left,m.UPOID==e.UPOID && e.STATUS == JuniperErrStatus.Open.ExtValue(),
        //              JoinType.Left,m.PREWO == f.WO})
        //            .Where((m, s, j, h, i, oh, e, f) => m.CUSTOMER == Customer.JUNIPER.ExtValue())
        //            .WhereIF(plant != "ALL", (m, s, j, h, i, oh, e, f) => m.PLANT == plant)
        //            .WhereIF(potype != "ALL", (m, s, j, h, i, oh, e, f) => m.POTYPE == potype)
        //            .WhereIF((poStatusArray.Length == 1 && poStatusArray[0] == "Special"), (m, s, j, h, i, oh, e, f) => 
        //                oh.SHIPPINGNOTE.ToUpper().Contains("DEV-")
        //                ||oh.SHIPPINGNOTE.ToUpper().Contains("ECO-")
        //                ||oh.SHIPPINGNOTE.ToUpper().Contains("MPM")
        //                ||oh.SHIPPINGNOTE.ToUpper().Contains("DEVIATION")
        //                ||(new string[] {"Z09","Z10","ZA3" }).Contains(oh.ECO_FCO))
        //            .WhereIF((poStatusArray.Length == 1 && poStatusArray[0] != "ALL" && poStatusArray[0]!= "Special"), (m, s, j, h, i, oh, e, f) => SqlFunc.ContainsArray(poStatusArray, j.DESCRIPTION))
        //            .WhereIF((poStatusArray.Length != 1), (m, s, j, h, i, oh, e, f) => SqlFunc.ContainsArray(poStatusArray, j.DESCRIPTION))
        //                .OrderBy((m, s, j, h, i, oh, e, f) => m.PONO, OrderByType.Asc)
        //            .Select((m, s, j, h, i, oh, e, f) =>
        //                new OrderModle
        //                {
        //                    ID = m.ID,
        //                    PLANT = m.PLANT,
        //                    HEADERSCHSTATUS = oh.HEADERSCHEDULINGSTATUS,
        //                    LINESCHSTATUS = i.LINESCHEDULINGSTATUS,
        //                    COMPLETEDELIVER = oh.COMPLETEDELIVERY,
        //                    UPOID = m.UPOID,
        //                    POTYPE = m.POTYPE,
        //                    PONO = m.PONO,
        //                    POLINE = m.POLINE,
        //                    VERSION = m.VERSION,
        //                    POSTATUS = j.DESCRIPTION,
        //                    HOLD = h.HOLDREASON,
        //                    QTY = m.QTY,
        //                    UNITPRICE = m.UNITPRICE,
        //                    PREWO = m.PREWO,
        //                    GROUPID = f.GROUPID,
        //                    PID = m.PID,
        //                    JUNIPERPID = i.PN,
        //                    DELIVERY = m.DELIVERY,
        //                    CRSD = i.CUSTREQSHIPDATE,
        //                    SO = oh.SALESORDERNUMBER,
        //                    SOLN = i.SALESORDERLINEITEM,
        //                    USERITEMTYPE = m.USERITEMTYPE,
        //                    OFFERINGTYPE = m.OFFERINGTYPE,
        //                    LASTCHANGETIME = m.LASTCHANGETIME,
        //                    COMPLETED = m.COMPLETED,
        //                    COMPLETIME = m.COMPLETIME,
        //                    CLOSED = m.CLOSED,
        //                    CLOSETIME = m.CLOSETIME,
        //                    CANCEL = m.CANCEL,
        //                    CANCELTIME = m.CANCELTIME,
        //                    CUSTOMSW = i.SWTYPE,
        //                    ECO_FCO = oh.ECO_FCO,
        //                    COUNTRYSPECIFICLABE = i.COUNTRYSPECIFICLABEL,
        //                    CARTONLABEL1 = i.CARTONLABEL1,
        //                    CARTONLABEL2 = i.CARTONLABEL2,
        //                    PACKOUTLABEL = i.PACKOUTLABEL,
        //                    CREATETIME = m.CREATETIME,
        //                    EDITTIME = m.EDITTIME,
        //                    ORIGINALID = m.ORIGINALID,
        //                    ORIGINALITEMID = m.ORIGINALITEMID,
        //                    ITEMID = m.ITEMID,
        //                    ORDERTYPE = m.ORDERTYPE,
        //                    EXCEPTIONINFO = e.EXCEPTIONINFO
        //                }).PartitionBy(m => m.UPOID).Distinct().ToList();

        //        //if (plant != "ALL" && !string.IsNullOrEmpty(plant))
        //        //    orderList = orderList.Where(t => t.PLANT == plant).ToList();W
        //        //if (potype != "ALL" && !string.IsNullOrEmpty(potype))
        //        //    orderList = orderList.Where(t => t.POTYPE == potype).ToList();
        //        //if (postatus != "ALL" && !string.IsNullOrEmpty(postatus))
        //        //    orderList = orderList.Where(t => t.POSTATUS == postatus).ToList();

        //        var plantobj = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "SYSPLANT" && t.FUNCTIONTYPE == "NOSYSTEM").ToList().FirstOrDefault();
        //        var threads = 20;
        //        var count = orderList.Count;
        //        var mCount = count % threads;
        //        var eachCount = (count - mCount) / threads;
        //        var manualEvents = new List<ManualResetEvent>();
        //        var res = new List<OrderModle>();
        //        for (int i = 0; i < threads; i++)
        //        {
        //            ManualResetEvent mre = new ManualResetEvent(false);
        //            manualEvents.Add(mre);
        //            var orders = new List<OrderModle>();
        //            if (i == threads - 1)
        //            {
        //                orders = orderList.GetRange(i * eachCount, eachCount + mCount);
        //            }
        //            else
        //            {
        //                orders = orderList.GetRange(i * eachCount, eachCount);
        //            }
        //            res.AddRange(orders);
        //            ThreadPool.QueueUserWorkItem((a) =>
        //            {
        //                TT(orders, plantobj);
        //                ManualResetEvent e = (ManualResetEvent)a;
        //                e.Set();
        //            }, mre);
        //        }
        //        WaitHandle.WaitAll(manualEvents.ToArray());//watting all thread
        //        StationReturn.Data = res;
        //        StationReturn.Status = StationReturnStatusValue.Pass;
        //        StationReturn.MessageCode = "MES00000001";
        //    }
        //    catch (Exception exception)
        //    {
        //        StationReturn.Status = StationReturnStatusValue.Fail;
        //        StationReturn.MessageCode = "MES00000037";
        //        StationReturn.MessagePara.Add(exception.Message);
        //        StationReturn.Data = exception.Message;
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}

        private void TT(List<OrderModle> orderList, R_F_CONTROL plantobj)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                foreach (var order in orderList)
                {
                    string pcode = plantobj != null ? plantobj.VALUE : "";
                    var plantcode = sfcdb.ORM.Queryable<R_SKU_PLANT>().Where(r => r.FOXCONN == order.PID).ToList().FirstOrDefault();
                    var i138sub = sfcdb.ORM.Queryable<O_I138>().Where(t => t.PONUMBER == order.PONO && t.POITEMNUMBER == order.POLINE).OrderBy(t => t.CREATETIME, OrderByType.Desc).ToList().FirstOrDefault();
                    DateTime? Last_138_submission = null;
                    if (plantcode != null && !string.IsNullOrEmpty(plantcode.PLANTCODE))
                    {
                        pcode = plantcode.PLANTCODE;
                    }
                    if (i138sub != null)
                    {
                        Last_138_submission = (DateTime)i138sub.CREATETIME;
                    }
                    order.LAST138SUBMISSION = Last_138_submission;
                    order.PLANTCODE = pcode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetOrderAlartList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var potype = Data["potype"].ToString().Trim();
                var postatus = Data["postatus"].ToString().Trim();
                sfcdb.ThrowSqlExeception = true;
                var orderAlartList = sfcdb.ORM.Queryable<O_EXCEPTION_DATA>().Where(t => t.STATUS != JuniperErrStatus.Ongoing.ExtValue()).ToList();

                if (potype != "ALL" && !string.IsNullOrEmpty(potype))
                    orderAlartList = orderAlartList.Where(t => t.EXCEPCODE == potype).ToList();
                if (postatus != "ALL" && !string.IsNullOrEmpty(postatus))
                    orderAlartList = orderAlartList.Where(t => t.STATUS == postatus).ToList();
                StationReturn.Data = orderAlartList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetStatusMaps(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var res = sfcdb.ORM.Queryable<O_PO_STATUS_MAP_J>().Select(t => t.DESCRIPTION).ToList().Distinct();
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Upload138List(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var predata = Data["predata"].ToList();
                var reasonmapinfo = sfcdb.ORM.Queryable<O_I138_REASON>().ToList();
                var uploadtable = new DataTable();
                var blockdates = new List<DateTime>();

                #region JNP_SYS_MAINTENANCE
                var currentDay = MesDbBase.GetOraDbTime(sfcdb.ORM);
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                DateTime currenttimewithpst = TimeZoneInfo.ConvertTime(currentDay, timeZoneInfo);
                var naintentime = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "JNP_SYS_MAINTENANCE" && t.CATEGORY == "MAINTENANCE_TIME" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE != "SYSTEM").ToList().FirstOrDefault();
                if (currenttimewithpst > Convert.ToDateTime(naintentime.VALUE) && currenttimewithpst < Convert.ToDateTime(naintentime.EXTVAL))
                    throw new Exception($@"Jnp System Maintenance at {naintentime.VALUE} - {naintentime.EXTVAL}");
                var blockdateobj = sfcdb.ORM.Queryable<R_F_CONTROL>()
                    .Where(t => t.FUNCTIONNAME == "JNP_COMMITDATE_BLOCK" && t.CATEGORY == "BLOCKDATE" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE != "SYSTEM")
                    .ToList().FirstOrDefault();
                if (blockdateobj != null)
                    blockdateobj.VALUE.Split(',').ToList().ForEach(p => { blockdates.Add(Convert.ToDateTime(p)); });
                #endregion

                foreach (var item in predata)
                {
                    foreach (var jToken in item.Children())
                    {
                        var jProperty = jToken as JProperty;
                        uploadtable.Columns.Add(jProperty.Name.ToString());
                    }
                    break;
                }
                uploadtable.Columns.Add("JNPPLANT");
                uploadtable.Columns.Add("VENDORID");
                uploadtable.Columns.Add("I137ID");
                uploadtable.Columns.Add("STATUS");
                uploadtable.Columns.Add("MSG");
                uploadtable.Columns.Add("SALESORDERNUMBER");

                uploadtable.Columns.Add("SKU");
                uploadtable.Columns.Add("SALESORDER");
                uploadtable.Columns.Add("SALESORDERITEM");
                uploadtable.Columns.Add("QTY");
                uploadtable.Columns.Add("PN");
                #region valid data
                foreach (var item in predata)
                {
                    var itemrow = uploadtable.NewRow();
                    try
                    {
                        foreach (var jToken in item.Children())
                        {
                            var jProperty = jToken as JProperty;
                            itemrow[jProperty.Name.ToString()] = item[jProperty.Name.ToString()].ToString().Trim();
                            if (itemrow[jProperty.Name.ToString()].ToString().Trim().Equals(""))
                                throw new Exception($@"{jProperty.Name.ToString()} value is empty,pls check!");
                        }
                    }
                    catch (Exception e)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = e.ToString();
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    var po = itemrow["PO"].ToString();
                    var poline = itemrow["ITEM"].ToString();
                    var itemmain = sfcdb.ORM.Queryable<I137_I, I137_H>((i, h) => i.TRANID == h.TRANID).Where((i, h) => i.PONUMBER == po && i.ITEM == poline
                                  && (i.MFLAG == ENUM_I137_H_STATUS.CHECK_PASS.ExtValue() || i.MFLAG == ENUM_I137_H_STATUS.RELEASE.ExtValue()))
                        .OrderBy((i, h) => i.LASTCHANGEDATETIME, OrderByType.Desc).Select((i, h) => new { i, h }).ToList().FirstOrDefault();

                    if (itemmain == null)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = $@"Without this POITEM: {po}-{poline},pls check!";
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }

                    var holdobj = sfcdb.ORM.Queryable<O_ORDER_HOLD, O_ORDER_MAIN>((h, m) => h.ITEMID == m.ITEMID).Where((h, m) => h.HOLDFLAG == MesBool.Yes.ExtValue()
                                && m.PONO == po && m.POLINE == poline).Select((h, m) => h).ToList().FirstOrDefault();
                    if (holdobj != null && holdobj.HOLDREASON.ToUpper().Contains("CREDIT"))
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = $@"Order Status on Hold hodereason: {holdobj.HOLDREASON}, pls check!";
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }

                    #region pre asn check..
                    var mainorder = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == po && t.POLINE == poline).ToList().FirstOrDefault();
                    if (mainorder == null)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = $@"Without this POITEM: {po}-{poline} in O_ORDER_MAIN, pls check!";
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    if (mainorder != null && mainorder.PREASN != ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue())
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = $@"Order: {po} {poline} already sent Pre-Ship ASN!";
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    #endregion

                    itemrow["PN"] = itemmain.i.PN;
                    itemrow["JNPPLANT"] = itemmain.h.RECIPIENTID;
                    itemrow["VENDORID"] = itemmain.h.VENDORID;
                    itemrow["I137ID"] = itemmain.i.ID;
                    itemrow["SALESORDERNUMBER"] = itemmain.h.SALESORDERNUMBER;
                    //item["SKU"] = itemmain.i.PN;
                    itemrow["SKU"] = mainorder.PID;
                    itemrow["SALESORDER"] = itemmain.h.SALESORDERNUMBER;
                    itemrow["SALESORDERITEM"] = itemmain.i.SALESORDERLINEITEM;
                    try
                    {
                        var qty = double.Parse(itemmain.i.QUANTITY);
                    }
                    catch (Exception e)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = $@"Qty must be Greater than 0!";
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    itemrow["QTY"] = double.Parse(itemmain.i.QUANTITY);


                    try
                    {
                        //var qty = int.Parse(item["QTY"].ToString());
                        var qty = int.Parse(itemrow["QTY"].ToString());
                        if (qty == 0)
                            throw new Exception("Qty>0!");
                        if (double.Parse(itemmain.i.QUANTITY) < qty)
                        {
                            itemrow["STATUS"] = "Check Fail";
                            itemrow["MSG"] = $@"Qty: {qty} must be <= Order Qty {itemmain.i.QUANTITY}!";
                            uploadtable.Rows.Add(itemrow);
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = $@"Qty must be Greater than 0!";
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    try
                    {
                        var delivery = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString());
                        var currentobj = reasonmapinfo.Where(t => t.REASONCODE == itemrow["REASONCODE"].ToString()).FirstOrDefault();
                        if (currentobj == null)
                        {
                            itemrow["STATUS"] = "Check Fail";
                            itemrow["MSG"] = $@"reasoncode:{ itemrow["REASONCODE"].ToString() } is not exists!";
                            uploadtable.Rows.Add(itemrow);
                            continue;
                        }

                        #region block commit time for jnp sys maintens
                        if (blockdates.Contains(delivery))
                        {
                            itemrow["STATUS"] = "Check Fail";
                            itemrow["MSG"] = $@"blockreason: {blockdateobj.EXTVAL},pls check!";
                            uploadtable.Rows.Add(itemrow);
                            continue;
                        }
                        #endregion
                        //if (itemmain.h.POCHANGEINDICATOR.Equals(Juniper137PoType.New.ExtValue()) && currentobj.CODETYPE != ENUM_I138_TYPE.NEWSCH.ExtValue())
                        //{
                        //    itemrow["STATUS"] = "Check Fail";
                        //    itemrow["MSG"] = $@"Order is newly schedule, pls use 1st scheduling reason code!";
                        //    uploadtable.Rows.Add(itemrow);
                        //    continue;
                        //}
                        //else
                        //{

                        //    //switch ((ENUM_I138_TYPE)Enum.Parse(typeof(ENUM_I138_TYPE), currentobj.CODETYPE))
                        //    //{
                        //    //    //case ENUM_I138_TYPE.NEWSCH:
                        //    //    //    if (!itemmain.h.POCHANGEINDICATOR.Equals(Juniper137PoType.New.ExtValue()))
                        //    //    //    {
                        //    //    //        itemrow["STATUS"] = "Check Fail";
                        //    //    //        itemrow["MSG"] = $@"Order is newly schedule, pls use 1st scheduling reason code!";
                        //    //    //        uploadtable.Rows.Add(itemrow);
                        //    //    //        continue;
                        //    //    //    }
                        //    //    //    break;
                        //    //    case ENUM_I138_TYPE.PULLIN:
                        //    //        if (itemmain.i.PODELIVERYDATE <= delivery)
                        //    //        {
                        //    //            itemrow["STATUS"] = "Check Fail";
                        //    //            itemrow["MSG"] = $@"Reason Code belongs to Pull in, new commit date must be earlier than current commit date in system!";
                        //    //            uploadtable.Rows.Add(itemrow);
                        //    //            continue;
                        //    //        }
                        //    //        break;
                        //    //    case ENUM_I138_TYPE.PUSHOUT:
                        //    //        if (itemmain.i.PODELIVERYDATE >= delivery)
                        //    //        {
                        //    //            itemrow["STATUS"] = "Check Fail";
                        //    //            itemrow["MSG"] = $@"Reason Code belongs to Push Out, new commit date must be later than current commit date in system!";
                        //    //            uploadtable.Rows.Add(itemrow);
                        //    //            continue;
                        //    //        }
                        //    //        break;
                        //    //}
                        //}

                        ///// Risk not check
                        //if (reasonmapinfo.Where(t => t.REASONCODE == itemrow["REASONCODE"].ToString() && t.CODETYPE != ENUM_I138_TYPE.RISK.ExtValue()).Count() > 0)
                        //{
                        //    /// 1st sch check
                        //    if (itemmain.h.POCHANGEINDICATOR.Equals(Juniper137PoType.New.ExtValue())
                        //        && reasonmapinfo.Where(t => t.REASONCODE == itemrow["REASONCODE"].ToString() && t.CODETYPE == ENUM_I138_TYPE.NEWSCH.ExtValue()).Count() == 0)
                        //    {
                        //        itemrow["STATUS"] = "Check Fail";
                        //        itemrow["MSG"] = $@"Order is newly schedule, pls use 1st scheduling reason code!";
                        //        uploadtable.Rows.Add(itemrow);
                        //        continue;
                        //    }
                        //    /// pull in check
                        //    else if (itemmain.i.PODELIVERYDATE > delivery
                        //        && reasonmapinfo.Where(t => t.REASONCODE == itemrow["REASONCODE"].ToString() && t.CODETYPE == ENUM_I138_TYPE.PULLIN.ExtValue()).Count() == 0)
                        //    {
                        //        itemrow["STATUS"] = "Check Fail";
                        //        itemrow["MSG"] = $@"Reason Code belongs to Pull in, new commit date must be earlier than current commit date in system!";
                        //        uploadtable.Rows.Add(itemrow);
                        //        continue;
                        //    }
                        //    /// push out check
                        //    if (itemmain.i.PODELIVERYDATE < delivery
                        //        && reasonmapinfo.Where(t => t.REASONCODE == itemrow["REASONCODE"].ToString() && t.CODETYPE == ENUM_I138_TYPE.PUSHOUT.ExtValue()).Count() == 0)
                        //    {
                        //        itemrow["STATUS"] = "Check Fail";
                        //        itemrow["MSG"] = $@"Reason Code belongs to Push Out, new commit date must be later than current commit date in system!";
                        //        uploadtable.Rows.Add(itemrow);
                        //        continue;
                        //    }
                        //}

                        /// 如果用戶commit 的日期比 CRSD 更早, 請提示: "Commit Date<CRSD" (但不要卡住)
                        if (itemmain.i.CUSTREQSHIPDATE > delivery)
                        {
                            itemrow["STATUS"] = "success!";
                            itemrow["MSG"] = $@"Commit Date < CRSD!";
                            uploadtable.Rows.Add(itemrow);
                            continue;
                        }

                        /// 如果用戶commit 的日期是星期天, 請提示 "Commit Date = Sunday" (但不要卡住) 操作系統時間格式不一樣會不會？！
                        if (delivery.DayOfWeek.ToString() == "Sunday")
                        {
                            itemrow["STATUS"] = "success!";
                            itemrow["MSG"] = $@"Commit Date = Sunday!";
                            uploadtable.Rows.Add(itemrow);
                            continue;
                        }

                        //need commit for same so with BNDL
                        //if (mainorder.USERITEMTYPE == ENUM_O_ORDER_MAIN_POTYPE.BNDL.ExtValue())
                        if (mainorder.OFFERINGTYPE == ENUM_O_AGILE_ATTR_OFFERINGTYPE.Fixed_Nonstockable_Bundle.ExtValue())
                        {
                            sfcdb.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((m, i, h) => m.ITEMID == i.ID && i.TRANID == h.TRANID).Where((m, i, h) => i.SOID == itemmain.i.SOID && h.SALESORDERNUMBER == itemmain.h.SALESORDERNUMBER
                                && m.USERITEMTYPE == ENUM_O_ORDER_MAIN_POTYPE.BNDL.ExtValue() && m.CANCEL == MesBool.No.ExtValue()).Select((m, i) => m).ToList()
                                .ForEach(p =>
                                {
                                    if (!predata.Any(t => t["PO"].ToString() == p.PONO && t["ITEM"].ToString() == p.POLINE && Convert.ToDateTime(t["DELIVERY(YYYY-MM-DD)"].ToString()) == delivery))
                                        throw new MesException($@"Delivery date must be consistent for BNDL with the same SOID ! (PO:{ p.PONO},POITEM:{p.POLINE})");
                                });
                        }
                    }
                    catch (Exception e)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = e.GetType() == typeof(MesException) ? e.Message : $@"deliveryDate format is Err,pls check!";
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    itemrow["STATUS"] = "success!";
                    itemrow["MSG"] = "";
                    //1.If the PO-LN is on HOLD when upload the PO-LN for commit, fail the upload
                    //Error Msg: Order is on CREDIT HOLD ONLY

                    //2.If PO - LN or Sales Order + SO Item ID already send pre - Ship ASN, fail commit.
                    //Error Msg: PO - LN already sent Pre - Ship ASN
                    uploadtable.Rows.Add(itemrow);
                }
                #endregion
                #region check pass record
                var checkres = uploadtable.Select(" STATUS ='Check Fail' ").Count() == 0;
                if (checkres && uploadtable.Rows.Count > 0)
                {
                    var plant = uploadtable.Rows[0]["VENDORID"].ToString().Equals(JuniperB2BPlantCode.FVN.ExtValue()) ? JuniperB2BPlantCode.FVN.ToString() : JuniperB2BPlantCode.FJZ.ToString();
                    var polist = uploadtable.Copy().DefaultView.ToTable(true, new string[] { "PO" });
                    //var ctranid =$@"{plant}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
                    var commit138list = new List<O_I138>();
                    var commit138Record = new List<O_I138_RECORD>();
                    var dbres = sfcdb.ORM.Ado.UseTran(() =>
                    {
                        foreach (DataRow dr in polist.Rows)
                        {
                            var poitems = uploadtable.Select($@"PO={dr["PO"].ToString()}");
                            var TranIDTemp = $@"{plant}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
                            foreach (DataRow item in poitems)
                            {
                                #region 避免重複
                                var isexiststranid = sfcdb.ORM.Queryable<O_I138>().Any(t => t.TRANID == TranIDTemp && t.PONUMBER != item["PO"].ToString()) ||
                                commit138list.Any(t => t.TRANID == TranIDTemp && t.PONUMBER != item["PO"].ToString());
                                while (isexiststranid)
                                {
                                    Thread.Sleep(50);
                                    TranIDTemp = $@"{plant}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
                                    isexiststranid = sfcdb.ORM.Queryable<O_I138>().Any(t => t.TRANID == TranIDTemp && t.PONUMBER != item["PO"].ToString()) ||
                                                    commit138list.Any(t => t.TRANID == TranIDTemp && t.PONUMBER != item["PO"].ToString());
                                }
                                #endregion
                                var ID = MesDbBase.GetNewID<O_ORDER_CHANGELOG>(sfcdb.ORM, Customer.JUNIPER.ExtValue());
                                commit138list.Add(new O_I138()
                                {
                                    ID = ID,
                                    CREATIONDATE = DateTime.Now,
                                    CREATETIME = DateTime.Now,
                                    PONUMBER = item["PO"].ToString(),
                                    POCREATIONDATE = DateTime.Now,
                                    JNP_PLANT = item["JNPPLANT"].ToString(),
                                    VENDORID = item["VENDORID"].ToString(),
                                    POITEMNUMBER = item["ITEM"].ToString(),
                                    PN = item["PN"].ToString(),
                                    NOTE = item["REASONCODE"].ToString(),
                                    DELIVERYSTARTDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                                    DELIVERYENDDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                                    SHIPPINGSTARTDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                                    SHIPPINGENDDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                                    QUANTITY = item["QTY"].ToString(),
                                    TRANID = TranIDTemp,//$@"{plant}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                                    CREATEBY = this.LoginUser.EMP_NO,
                                    SALESORDERNUMBER = item["SALESORDERNUMBER"].ToString()
                                });
                                commit138Record.Add(new O_I138_RECORD()
                                {
                                    ID = MesDbBase.GetNewID<O_I138_RECORD>(sfcdb.ORM, Customer.JUNIPER.ExtValue()),
                                    UPOID = $@"{item["PO"].ToString()}{item["ITEM"].ToString()}",
                                    I138ID = ID,
                                    I137ID = item["I137ID"].ToString(),
                                    I138CREATETIME = DateTime.Now,
                                    CREATETIME = DateTime.Now
                                });
                            }
                        }
                        sfcdb.ORM.Insertable(commit138list).ExecuteCommand();
                        sfcdb.ORM.Insertable(commit138Record).ExecuteCommand();


                        ////同一PO TRANID要一樣 donald要求
                        //var TranIDTemp = $@"{plant}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
                        //var PoTemp = "";
                        //uploadtable.DefaultView.Sort = "PO,ITEM";
                        //DataTable uploadtableTemp = uploadtable.DefaultView.ToTable();

                        //foreach (DataRow item in uploadtableTemp.Rows)
                        //{
                        //    if (PoTemp != item["PO"].ToString())
                        //    {
                        //        TranIDTemp = $@"{plant}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
                        //    }
                        //    PoTemp = item["PO"].ToString();

                        //    var ID = MesDbBase.GetNewID<O_ORDER_CHANGELOG>(sfcdb.ORM, Customer.JUNIPER.ExtValue());
                        //    sfcdb.ORM.Insertable(new O_I138()
                        //    {
                        //        ID = ID,
                        //        CREATIONDATE = DateTime.Now,
                        //        CREATETIME = DateTime.Now,
                        //        PONUMBER = item["PO"].ToString(),
                        //        POCREATIONDATE = DateTime.Now,
                        //        JNP_PLANT = item["JNPPLANT"].ToString(),
                        //        VENDORID = item["VENDORID"].ToString(),
                        //        POITEMNUMBER = item["ITEM"].ToString(),
                        //        PN = item["PN"].ToString(),
                        //        NOTE = item["REASONCODE"].ToString(),
                        //        DELIVERYSTARTDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                        //        DELIVERYENDDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                        //        SHIPPINGSTARTDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                        //        SHIPPINGENDDATE = Convert.ToDateTime(item["DELIVERY(YYYY-MM-DD)"].ToString()),
                        //        QUANTITY = item["QTY"].ToString(),
                        //        TRANID = TranIDTemp,//$@"{plant}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}",
                        //        CREATEBY = this.LoginUser.EMP_NO,
                        //        SALESORDERNUMBER = item["SALESORDERNUMBER"].ToString()
                        //    }).ExecuteCommand();
                        //    sfcdb.ORM.Insertable(new O_I138_RECORD()
                        //    {
                        //        ID = MesDbBase.GetNewID<O_I138_RECORD>(sfcdb.ORM, Customer.JUNIPER.ExtValue()),
                        //        UPOID = $@"{item["PO"].ToString()}{item["ITEM"].ToString()}",
                        //        I138ID = ID,
                        //        I137ID = item["I137ID"].ToString(),
                        //        I138CREATETIME = DateTime.Now,
                        //        CREATETIME = DateTime.Now
                        //    }).ExecuteCommand();
                        //    Thread.Sleep(50);
                        //}
                    });
                    if (dbres.IsSuccess)
                    {
                        StationReturn.Data = uploadtable;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                }
                else
                {
                    StationReturn.Data = uploadtable;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(" Valid I138 Data Fail!");
                }
                #endregion
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Get138List(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var plant = Data["plant"].ToString().Trim();

                var res = sfcdb.ORM.Queryable<O_I138, O_B2B_ACK>((o, a) => new object[] { JoinType.Left, o.TRANID == a.TRANID && a.F_MSG_TYPE == JuniperErrType.I138.ExtValue() }).OrderBy((o, a) => o.CREATETIME, OrderByType.Desc)
                    .Select((o, a) => new
                    {
                        o.PONUMBER,
                        o.JNP_PLANT,
                        o.VENDORID,
                        o.POITEMNUMBER,
                        o.PN,
                        o.SALESORDERNUMBER,
                        REASONCODE = o.NOTE,
                        o.DELIVERYSTARTDATE,
                        o.QUANTITY,
                        o.TRANID,
                        o.CREATIONDATE,
                        o.CREATEBY,
                        a.EXCEPTIONTYPE,
                        a.F_MSG,
                        a.FILENAME
                    })
                        .ToList().Distinct();
                if (plant != "ALL" && !string.IsNullOrEmpty(plant))
                    res = res.Where(t => t.VENDORID == plant).ToList();
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetSingleOrderInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var poid = Data["POID"].ToString().Trim();
                var ordermain = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID.ToString() == poid).ToList().FirstOrDefault();

                var i = sfcdb.ORM.Queryable<I137_I>().Where(t => t.ID == ordermain.ITEMID).ToList().FirstOrDefault();
                var h = sfcdb.ORM.Queryable<I137_H>().Where(t => t.TRANID == i.TRANID).ToList().FirstOrDefault();
                var d = sfcdb.ORM.Queryable<I137_D>().Where(t => t.TRANID == i.TRANID && t.PONUMBER == i.PONUMBER && t.ITEM == i.ITEM).ToList();
                var o = sfcdb.ORM.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == ordermain.ID).ToList();

                //var resobj = sfcdb.ORM.Queryable<I137_H, I137_I, I137_D>((h, i, d) => new object[] { JoinType.Inner, i.TRANID == h.TRANID, JoinType.Left, i.TRANID == d.TRANID } ).Where((h, i, d) => i.ID == ordermain.ITEMID)
                //    .Select((h, i, d) => new { h, i, d }).ToList().FirstOrDefault();

                var res = new object[]
                 {
                       ObjectDataHelper.FormatTableSingleRowViewData(h,6),
                       ObjectDataHelper.FormatTableSingleRowViewData(i,6),
                       d,
                       o
                 };

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
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
        public void GetPreI138(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var HeadInfo = sfcdb.ORM.Queryable<R_PO, PO_HEAD>((rp, ph) => rp.MESSAGEID == ph.MESSAGEID).Where((rp, ph) => rp.ID == poid)
                    .Select((rp, ph) => ph).ToList().FirstOrDefault();
                var ItemInfo = sfcdb.ORM.Queryable<R_PO, PO_HEAD, PO_ITEM>((rp, ph, pi) => rp.MESSAGEID == ph.MESSAGEID && ph.FILENAME == pi.FILENAME)
                    .Where((rp, ph) => rp.ID == poid).Select((rp, ph, pi) => pi).ToList().FirstOrDefault();
                var LineInfo = sfcdb.ORM.Queryable<R_PO, PO_HEAD, PO_LINE>((rp, ph, pl) => new object[] {
                    SqlSugar.JoinType.Inner,rp.MESSAGEID==ph.MESSAGEID,
                    SqlSugar.JoinType.Left,ph.FILENAME==pl.FILENAME }).Where((rp, ph, pl) => rp.ID == poid)
                    .Select((rp, ph, pl) => pl).ToList();
                StationReturn.Data = new PO_CONFIRM_HEAD
                {
                    PO = HeadInfo.PO,
                    POITEM = ItemInfo.LINENO,
                    VENDORID = HeadInfo.SUPPLIER,
                    PN = ItemInfo.SKU,
                    PLANT = HeadInfo.PLANT,
                    UNITCODE = "EA",
                    SO = ItemInfo.SO,
                    SOITEM = ItemInfo.SOLINE,
                    QTY = ItemInfo.QTY
                };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetI138ReasonCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var reasonCodeList = sfcdb.ORM.Queryable<R_JUNIPER_I138_REASONCODE>().OrderBy(t => t.ID, OrderByType.Asc).ToList();
                StationReturn.Data = reasonCodeList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void AddI138Data(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                PO_CONFIRM_HEAD model = Data["model"].ToObject<PO_CONFIRM_HEAD>();
                var poConfirmHeadIsExsit = sfcdb.ORM.Queryable<PO_CONFIRM_HEAD>()
                    .Any(t => t.PO == model.PO && t.POITEM == model.POITEM);
                if (poConfirmHeadIsExsit)
                    throw new Exception($@"PO:{model.PO}/ITEM:{model.POITEM} already Commit!");
                var poConfirmHeadList = sfcdb.ORM.Queryable<PO_CONFIRM_HEAD>().OrderBy(t => t.MESSAGEID, OrderByType.Desc).ToList();
                if (poConfirmHeadList?.Count > 0)
                    model.MESSAGEID = (Convert.ToInt32(poConfirmHeadList.FirstOrDefault().MESSAGEID) + 1).ToString();
                else
                    model.MESSAGEID = "1000000000";
                model.CREATIONDATETIME = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                model.DELIVERYDATE = Convert.ToDateTime(model.DELIVERYDATE).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                model.LOAD_CDB_FLAG = "N";
                model.CREATEDT = DateTime.Now.ToString();
                model.STD_OUT_FILE_NAME = $@"FNN_J_I138_PO_ITEM_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xml";
                sfcdb.ORM.Insertable(model).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetAgileList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var orderAlartList = sfcdb.ORM.Queryable<O_AGILE_ATTR>().PartitionBy(t => t.ACTIVED == MesBool.Yes.ExtValue()).ToList();
                StationReturn.Data = orderAlartList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetPreSap(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var ordermain = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID.ToString() == poid).ToList().FirstOrDefault();
                if (!string.IsNullOrEmpty(ordermain.PREWO))
                {
                    var podetail = sfcdb.ORM.Queryable<R_SAP_PODETAIL>().Where(t => t.WO == ordermain.PREWO).ToList();
                    var asbom = sfcdb.ORM.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == ordermain.PREWO).ToList();
                    var hb = sfcdb.ORM.Queryable<R_SAP_HB>().Where(t => t.WO == ordermain.PREWO).ToList();
                    StationReturn.Data = new { podetail, asbom, hb };
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetPreWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var ordermain = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID.ToString() == poid).ToList().FirstOrDefault();
                if (!string.IsNullOrEmpty(ordermain.PREWO))
                {
                    var head = sfcdb.ORM.Queryable<R_PRE_WO_HEAD>().Where(t => t.WO == ordermain.PREWO).ToList();
                    var detail = sfcdb.ORM.Queryable<R_PRE_WO_DETAIL>().Where(t => t.WO == ordermain.PREWO).ToList();
                    StationReturn.Data = new { head, detail };
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetItem054(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var ordermain = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID.ToString() == poid).ToList().FirstOrDefault();
                if (!string.IsNullOrEmpty(ordermain.PREWO))
                {
                    var I054 = sfcdb.ORM.Queryable<R_I054, R_I054, R_SN>((m, mm, s) => mm.TRANID == m.TRANID && mm.PARENTSN == s.SN && s.VALID_FLAG == MesBool.Yes.ExtValue())
                        .Where((m, mm, s) => s.WORKORDERNO == ordermain.PREWO)
                        .Select((m, mm, s) => m)
                        .OrderBy(m => m.ID)
                        .Distinct()
                        .ToList();
                    var I054ack = sfcdb.ORM.Queryable<R_I054, R_SN, R_I054_ACK>((m, s, a) => m.PARENTSN == s.SN && s.VALID_FLAG == MesBool.Yes.ExtValue() && m.TRANID == a.TRANID)
                        .Where((m, s, a) => s.WORKORDERNO == ordermain.PREWO).Select((m, s, a) => a).ToList().Distinct();
                    StationReturn.Data = new { I054, I054ack };
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetItem138(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var ordermain = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID.ToString() == poid).ToList().FirstOrDefault();
                var res = sfcdb.ORM.Queryable<O_I138, O_B2B_ACK>((o, a) => new object[] { JoinType.Left, o.TRANID == a.TRANID && a.F_MSG_TYPE == JuniperErrType.I138.ExtValue() })
                    .Where((o, a) => o.PONUMBER == ordermain.PONO && o.POITEMNUMBER == ordermain.POLINE).OrderBy((o, a) => o.CREATETIME, OrderByType.Desc)
                    .Select((o, a) => new
                    {
                        o.PONUMBER,
                        o.JNP_PLANT,
                        o.VENDORID,
                        o.POITEMNUMBER,
                        o.PN,
                        o.SALESORDERNUMBER,
                        REASONCODE = o.NOTE,
                        o.DELIVERYSTARTDATE,
                        o.QUANTITY,
                        o.TRANID,
                        o.CREATIONDATE,
                        o.CREATEBY,
                        a.EXCEPTIONTYPE,
                        a.F_MSG,
                        a.FILENAME
                    }).ToList().Distinct();
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Get137HeadByPo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var ordermain = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID.ToString() == poid).ToList().FirstOrDefault();
                var orderList = sfcdb.ORM.Queryable<O_I137_HEAD>().Where(t => t.PONUMBER == ordermain.PONO).OrderBy(t => SqlFunc.ToInt32(t.VERSION)).ToList();
                StationReturn.Data = orderList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Get137DataByTranID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var tranid = Data["tranid"].ToString().Trim();
                var item = Data["item"].ToString().Trim();
                var i = sfcdb.ORM.Queryable<I137_I>().Where(t => t.TRANID == tranid && t.ITEM == item).ToList().FirstOrDefault();
                var h = sfcdb.ORM.Queryable<I137_H>().Where(t => t.TRANID == tranid).ToList().FirstOrDefault();
                var d = sfcdb.ORM.Queryable<I137_D>().Where(t => t.TRANID == tranid && t.ITEM == item).ToList();
                var res = new object[]
                 {
                       ObjectDataHelper.FormatTableSingleRowViewData(h,4),
                       ObjectDataHelper.FormatTableSingleRowViewData(i,4),
                       d
                 };

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
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
        public void Get139and282ByMainid(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var I139 = sfcdb.ORM.Queryable<O_ORDER_MAIN, R_I139>((m, i9) => m.PONO == i9.PONUMBER && m.POLINE == i9.ITEM).Where((m, i9) => m.ID == poid).Select((m, i9) => i9).ToList();
                var I282 = sfcdb.ORM.Queryable<O_ORDER_MAIN, R_I139, R_I282>((m, i9, i2) => m.PONO == i9.PONUMBER && m.POLINE == i9.ITEM && i9.ASNNUMBER == i2.ASNNUMBER).Where((m, i9, i2) => m.ID == poid).Select((m, i9, i2) => i2)
                    .Distinct().ToList();

                StationReturn.Data = new
                {
                    I139,
                    I282
                };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Get244ByMainid(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();

                var orderMain = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID == poid).First();
                //var I244 = sfcdb.ORM.Queryable<O_ORDER_MAIN, R_SN, R_I244>((m, s, r) => m.PREWO == s.WORKORDERNO && s.SN == r.PARENTSN)
                //.Where((m, s, r) => m.ID == poid && s.VALID_FLAG == MesBool.Yes.ExtValue()).Select((m, s, r) => r).Distinct().ToList();

                //update by LHJ request by pohhong 2022.3.3
                //List<MESDataObject.Module.Juniper.R_I244> I244 = sfcdb.ORM.Queryable<I137_I, O_I137_HEAD, MESDataObject.Module.Juniper.R_I244, MESDataObject.Module.Juniper.R_I244>
                //            ((ii, ih, ri, rm) => ii.TRANID == ih.TRANID && SqlSugar.SqlFunc.EndsWith(ii.SALESORDERLINEITEM, ri.SALESORDERLINENUMBER)
                //            //update by pohhong 2022.2.25
                //            && ri.MESSAGEID == rm.MESSAGEID
                //            //&& SqlSugar.SqlFunc.EndsWith(ih.SALESORDERNUMBER, ri.SALESORDERNUMBER) && (ri.PNTYPE == "Parent" || SqlFunc.IsNullOrEmpty(ri.PARENTSN)))
                //            && SqlSugar.SqlFunc.EndsWith(ih.SALESORDERNUMBER, ri.SALESORDERNUMBER))
                //            .Where((ii, ih, ri, rm) => ii.ID == orderMain.ITEMID).Select((ii, ih, ri, rm) => rm).ToList();


                //List<MESDataObject.Module.Juniper.R_I244> I244 = sfcdb.ORM.Queryable<O_ORDER_MAIN, MESDataObject.Module.R_SN, MESDataObject.Module.Juniper.R_I244>
                //    ((oo, rs, ri) => oo.PREWO == rs.WORKORDERNO && rs.SN == ri.PARENTSN).Where((oo, rs, ri) => oo.ID == poid).Select((oo, rs, ri) => ri).ToList();

                List<MESDataObject.Module.Juniper.R_I244> I244 = sfcdb.ORM.Queryable<I137_I, O_I137_HEAD, MESDataObject.Module.Juniper.R_I244, MESDataObject.Module.Juniper.R_I244>
                            ((ii, ih, ri, rm) => ii.TRANID == ih.TRANID && ri.MESSAGEID == rm.MESSAGEID
                            && (ii.SALESORDERLINEITEM.Substring(2, 20) == ri.SALESORDERLINENUMBER || ii.SALESORDERLINEITEM.Substring(1, 20) == ri.SALESORDERLINENUMBER)
                            && ih.SALESORDERNUMBER.Substring(2, 20) == ri.SALESORDERNUMBER
                            )
                            .Where((ii, ih, ri, rm) => ii.ID == orderMain.ORIGINALITEMID).Select((ii, ih, ri, rm) => rm).Distinct().ToList();

                StationReturn.Data = I244;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetOrderSnByMainid(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var snlist = sfcdb.ORM.Queryable<O_ORDER_MAIN, R_SN>((m, s) => m.PREWO == s.WORKORDERNO).Where((m, s) => m.ID == poid && s.VALID_FLAG == MesBool.Yes.ExtValue()).Select((m, s) => s).Distinct().ToList();
                StationReturn.Data = snlist;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetOrderListByPOList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var po = Data["POLIST"].ToString().Trim();
                if (string.IsNullOrEmpty(po))
                {
                    throw new Exception("Please input compare PO.");
                }
                string[] poArry = po.Split(',');
                for (int i = 0; i < poArry.Length; i++)
                {
                    poArry[i] = poArry[i].Trim();
                }


                var orderList = sfcdb.ORM.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J, O_ORDER_HOLD, I137_I, I137_H, O_EXCEPTION_DATA, R_WO_GROUPID>((m, s, j, h, i, oh, e, f) =>
                       new object[] { JoinType.Left,m.ID == s.POID  &&
                        s.VALIDFLAG == MesBool.Yes.ExtValue() ,JoinType.Left,s.STATUSID==j.NAME,JoinType.Left,m.ITEMID==h.ITEMID, JoinType.Left, m.ITEMID == i.ID, JoinType.Left, i.TRANID==oh.TRANID, JoinType.Left,m.UPOID==e.UPOID &&
                    e.STATUS == JuniperErrStatus.Open.ExtValue(),JoinType.Left,m.PREWO == f.WO })
                    .Where((m, s, j, h, i, oh, e, f) =>
                        m.CUSTOMER == Customer.JUNIPER.ExtValue() && SqlFunc.ContainsArray(poArry, m.PONO))
                        .OrderBy((m, s, j, h, i, oh, e, f) => m.PONO, OrderByType.Asc)
                    .Select((m, s, j, h, i, oh, e, f) =>
                        new
                        {
                            m.ID,
                            m.PLANT,
                            oh.HEADERSCHEDULINGSTATUS,
                            i.LINESCHEDULINGSTATUS,
                            oh.COMPLETEDELIVERY,
                            m.UPOID,
                            m.POTYPE,
                            m.PONO,
                            m.POLINE,
                            m.VERSION,
                            POSTATUS = j.DESCRIPTION,
                            HOLD = h.HOLDREASON,
                            m.QTY,
                            m.UNITPRICE,
                            m.PREWO,
                            f.GROUPID,
                            m.PID,
                            m.CUSTPID,
                            m.DELIVERY,
                            i.CUSTREQSHIPDATE,
                            SO = oh.SALESORDERNUMBER,
                            i.SALESORDERLINEITEM,
                            m.USERITEMTYPE,
                            m.OFFERINGTYPE,
                            m.LASTCHANGETIME,
                            m.COMPLETED,
                            m.COMPLETIME,
                            m.CLOSED,
                            m.CLOSETIME,
                            m.CANCEL,
                            m.CANCELTIME,
                            i.SWTYPE,
                            oh.ECO_FCO,
                            i.COUNTRYSPECIFICLABEL,
                            i.CARTONLABEL1,
                            i.CARTONLABEL2,
                            i.PACKOUTLABEL,
                            m.CREATETIME,
                            m.EDITTIME,
                            m.ORIGINALID,
                            m.ORIGINALITEMID,
                            m.ITEMID,
                            e.EXCEPTIONINFO,
                            i.PN,
                            m.ORDERTYPE
                        }).PartitionBy(m => m.UPOID).Distinct().ToList();

                List<OrderModle> list = new List<OrderModle>();
                foreach (var order in orderList)
                {
                    var plantobj = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "SYSPLANT" && t.FUNCTIONTYPE == "NOSYSTEM").ToList().FirstOrDefault();
                    string pcode = plantobj != null ? plantobj.VALUE : "";
                    var plantcode = sfcdb.ORM.Queryable<R_SKU_PLANT>().Where(r => r.JUNIPER == order.PID).ToList().FirstOrDefault();
                    if (plantcode != null && !string.IsNullOrEmpty(plantcode.PLANTCODE))
                    {
                        pcode = plantcode.PLANTCODE;
                    }
                    var i138sub = sfcdb.ORM.Queryable<O_I138>().Where(t => t.PONUMBER == order.PONO && t.POITEMNUMBER == order.POLINE).OrderBy(t => t.CREATETIME, OrderByType.Desc).ToList().FirstOrDefault();
                    DateTime? Last_138_submission = null;
                    if (i138sub != null)
                    {
                        Last_138_submission = (DateTime)i138sub.CREATETIME;
                    }
                    list.Add(
                        new OrderModle
                        {
                            ID = order.ID,
                            PLANT = order.PLANT,
                            HEADERSCHSTATUS = order.HEADERSCHEDULINGSTATUS,
                            LINESCHSTATUS = order.LINESCHEDULINGSTATUS,
                            COMPLETEDELIVER = order.COMPLETEDELIVERY,
                            LAST138SUBMISSION = Last_138_submission,
                            UPOID = order.UPOID,
                            POTYPE = order.POTYPE,
                            PONO = order.PONO,
                            POLINE = order.POLINE,
                            VERSION = order.VERSION,
                            POSTATUS = order.POSTATUS,
                            HOLD = order.HOLD,
                            QTY = order.QTY,
                            UNITPRICE = order.UNITPRICE,
                            PREWO = order.PREWO,
                            JUNIPERPID = order.PN,
                            //JUNIPERPID = order.CUSTPID,
                            GROUPID = order.GROUPID,
                            PLANTCODE = pcode,
                            PID = order.PID,
                            DELIVERY = order.DELIVERY,
                            CRSD = order.CUSTREQSHIPDATE,
                            SO = order.SO,
                            SOLN = order.SALESORDERLINEITEM,
                            USERITEMTYPE = order.USERITEMTYPE,
                            OFFERINGTYPE = order.OFFERINGTYPE,
                            LASTCHANGETIME = order.LASTCHANGETIME,
                            COMPLETED = order.COMPLETED,
                            COMPLETIME = order.COMPLETIME,
                            CLOSED = order.CLOSED,
                            CLOSETIME = order.CLOSETIME,
                            CANCEL = order.CANCEL,
                            CANCELTIME = order.CANCELTIME,
                            CUSTOMSW = order.SWTYPE,
                            ECO_FCO = order.ECO_FCO,
                            COUNTRYSPECIFICLABE = order.COUNTRYSPECIFICLABEL,
                            CARTONLABEL1 = order.CARTONLABEL1,
                            CARTONLABEL2 = order.CARTONLABEL2,
                            PACKOUTLABEL = order.PACKOUTLABEL,
                            CREATETIME = order.CREATETIME,
                            EDITTIME = order.EDITTIME,
                            ORIGINALID = order.ORIGINALID,
                            ORIGINALITEMID = order.ORIGINALITEMID,
                            ITEMID = order.ITEMID,
                            EXCEPTIONINFO = order.EXCEPTIONINFO,
                            ORDERTYPE=order.ORDERTYPE
                        });
                }

                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetWOAutoKPList(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var wo = Data["WO"].ToString().Trim();
                var list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(wo, "JuniperAutoKPConfig", sfcdb);
                if (list == null || list.Count == 0)
                {
                    throw new Exception("No data.");
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("WO");
                dt.Columns.Add("PN");
                dt.Columns.Add("PN_SERIALIZATION");
                dt.Columns.Add("CUST_PN");
                dt.Columns.Add("PN_7XX");
                dt.Columns.Add("SN_RULE");
                dt.Columns.Add("QTY");
                dt.Columns.Add("TYPE");
                dt.Columns.Add("REV");
                dt.Columns.Add("CLEI_CODE");
                dt.Columns.Add("CHAS_SN");
                foreach (var v in list)
                {
                    DataRow r = dt.NewRow();
                    r["WO"] = wo;
                    r["PN"] = v.PN;
                    r["PN_SERIALIZATION"] = v.PN_SERIALIZATION;
                    r["CUST_PN"] = v.CUST_PN;
                    r["PN_7XX"] = v.PN_7XX;
                    r["SN_RULE"] = v.SN_RULE;
                    r["QTY"] = v.QTY;
                    r["TYPE"] = v.TYPE;
                    r["REV"] = v.REV;
                    r["CLEI_CODE"] = v.CLEI_CODE;
                    r["CHAS_SN"] = v.CHAS_SN;
                    dt.Rows.Add(r);
                }

                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void UpdateWOAutoKP(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var oldRow = Data["OLD_ROW"].ToString().Trim();
                var newRow = Data["NEW_ROW"].ToString().Trim();
                string wo = Data["OLD_ROW"]["WO"].ToString().Trim();
                bool bLoading = sfcdb.ORM.Queryable<R_SN>().Any(r => r.WORKORDERNO == wo);
                if (sfcdb.ORM.Queryable<R_SN>().Any(r => r.WORKORDERNO == wo))
                {
                    //throw new Exception($"{wo} already loading.");
                }

                var list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(wo, "JuniperAutoKPConfig", sfcdb);
                JuniperAutoKpConfig oldObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JuniperAutoKpConfig>(oldRow);
                JuniperAutoKpConfig newObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JuniperAutoKpConfig>(newRow);

                List<JuniperAutoKpConfig> edit = string.IsNullOrEmpty(oldObj.PN) ? list : list.Where(k => k.PN == oldObj.PN).ToList();
                edit = string.IsNullOrEmpty(oldObj.PN_SERIALIZATION) ? edit : edit.Where(k => k.PN_SERIALIZATION == oldObj.PN_SERIALIZATION).ToList();
                edit = string.IsNullOrEmpty(oldObj.CUST_PN) ? edit : edit.Where(k => k.CUST_PN == oldObj.CUST_PN).ToList();
                edit = string.IsNullOrEmpty(oldObj.PN_7XX) ? edit : edit.Where(k => k.PN_7XX == oldObj.PN_7XX).ToList();
                edit = string.IsNullOrEmpty(oldObj.SN_RULE) ? edit : edit.Where(k => k.SN_RULE == oldObj.SN_RULE).ToList();
                edit = string.IsNullOrEmpty(oldObj.TYPE) ? edit : edit.Where(k => k.TYPE == oldObj.TYPE).ToList();
                edit = string.IsNullOrEmpty(oldObj.REV) ? edit : edit.Where(k => k.REV == oldObj.REV).ToList();
                edit = string.IsNullOrEmpty(oldObj.CLEI_CODE) ? edit : edit.Where(k => k.CLEI_CODE == oldObj.CLEI_CODE).ToList();
                edit = string.IsNullOrEmpty(oldObj.CHAS_SN) ? edit : edit.Where(k => k.CHAS_SN == oldObj.CHAS_SN).ToList();
                edit = edit.Where(k => k.QTY == oldObj.QTY).ToList();
                if (edit.FirstOrDefault() != null)
                {
                    //list.Remove(edit.FirstOrDefault());
                    edit.FirstOrDefault().PN = newObj.PN;
                    edit.FirstOrDefault().PN_SERIALIZATION = newObj.PN_SERIALIZATION;
                    edit.FirstOrDefault().CUST_PN = newObj.CUST_PN;
                    edit.FirstOrDefault().PN_7XX = newObj.PN_7XX;
                    edit.FirstOrDefault().SN_RULE = newObj.SN_RULE;
                    edit.FirstOrDefault().TYPE = newObj.TYPE;
                    edit.FirstOrDefault().REV = newObj.REV;
                    edit.FirstOrDefault().CLEI_CODE = newObj.CLEI_CODE;
                    edit.FirstOrDefault().CHAS_SN = newObj.CHAS_SN;
                    edit.FirstOrDefault().QTY = newObj.QTY;

                    var vers = EcnFunction.GetUsefulVer(edit.FirstOrDefault().PN_7XX, sfcdb);
                    if (!vers.Contains(edit.FirstOrDefault().REV))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("REV_OF_PARTNO_ERROR", new string[]
                        { edit.FirstOrDefault().PN_7XX, edit.FirstOrDefault().REV }));
                    }

                    var cleis = EcnFunction.GetUsefulCLEICode(edit.FirstOrDefault().PN, sfcdb);
                    if (!cleis.Contains(edit.FirstOrDefault().CLEI_CODE))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("CLEI_OF_PARTNO_ERROR", new string[]
                        { edit.FirstOrDefault().PN, edit.FirstOrDefault().CLEI_CODE }));
                    }

                    JsonSave.SaveToDB(list, wo, "JuniperAutoKPConfig", LoginUser.EMP_NO, sfcdb, BU, true);
                }

                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetDoaOrder(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var list = sfcdb.ORM.Queryable<I137_I, I137_H>((i, h) => h.FILENAME == i.FILENAME).Where((i, h) => h.PODOCTYPE == ENUM_I137_PoDocType.IDOA.ExtValue())
                 .OrderBy((i, h) => h.LASTCHANGEDATETIME, OrderByType.Desc)
                 .Select((i, h) => new { i.PONUMBER, i.ITEM, i.ACTIONCODE, i.PN, i.SOQTY, i.BASEQUANTITY, h.SALESORDERNUMBER, i.SALESORDERLINEITEM, i.SOID, h.HEADERSCHEDULINGSTATUS, i.LINESCHEDULINGSTATUS, h.COMPLETEDELIVERY, i.SALESORDERHOLD, h.ECO_FCO, i.CARTONLABEL1, i.CARTONLABEL2, i.CREATETIME })
                 .PartitionBy(i => new { i.PONUMBER, i.ITEM }).Distinct().ToList();
                if (list == null || list.Count == 0)
                    throw new Exception("No data.");
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void CreateDoaOrder(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var EIMILFGI_MILPITAS = Newtonsoft.Json.JsonConvert.DeserializeObject<O_I137_HEAD>($@"{{""SHIPTOID"":""Juniper Networks"",
                                        ""SHIPTOCOMPANY"":""EIMILFGI C/O Expeditors International"",
                                        ""SHIPTOSTREETNAME"":""1075 Montague Expressway"",
                                        ""SHIPTOCITYNAME"":""MILPITAS"",
                                        ""SHIPTOREGIONCODE"":""CA"",
                                        ""SHIPTOSTREETPOSTALCODE"":""95035"",
                                        ""SHIPTOCOUNTRYCODE"":""US""}}");
                var EIAMSBONDFGI_AMSTERDAM = Newtonsoft.Json.JsonConvert.DeserializeObject<O_I137_HEAD>($@"{{""SHIPTOID"":""Juniper Networks"",
                                        ""SHIPTOCOMPANY"":""EIAMSBONDFGI"",
                                        ""SHIPTOSTREETNAME"":""c/o Logwise Casablancaweg 8-10"",
                                        ""SHIPTOCITYNAME"":""AMSTERDAM"",
                                        ""SHIPTOREGIONCODE"":""NH"",
                                        ""SHIPTOSTREETPOSTALCODE"":""1047 HP"",
                                        ""SHIPTOCOUNTRYCODE"":""NL""}}");
                var EIHKFGI_HONG_KONG = Newtonsoft.Json.JsonConvert.DeserializeObject<O_I137_HEAD>($@"{{""SHIPTOID"":""Juniper Networks"",
                                        ""SHIPTOCOMPANY"":""EIHKFGI   BV"",
                                        ""SHIPTOSTREETNAME"":""G/F, Tradeport Logistics Centre,"",
                                        ""SHIPTOCITYNAME"":""21 Chun Yue Road"",
                                        ""SHIPTOREGIONCODE"":""HKIA"",
                                        ""SHIPTOSTREETPOSTALCODE"":""LANTAU"",
                                        ""SHIPTOCOUNTRYCODE"":""HK""}}");
                var PONUMBER = Data["PONUMBER"].ToString().Trim();
                var ITEM = Data["ITEM"].ToString().Trim();
                var NETPRICE = new Func<string>(()=> {
                    try { Convert.ToDecimal(Data["NETPRICE"].ToString().Trim()); }
                    catch { throw new Exception($@"NETPRICE is Err!"); }
                    return Data["NETPRICE"].ToString().Trim();
                })(); 

                var PN = Data["PN"].ToString().Trim();
                var ACTIONCODE = Data["POCANCEL"].ToString().Trim() == "True" ? ENUM_I137_Actioncode_Type.Cancel.ExtValue() : ENUM_I137_Actioncode_Type.Change.ExtValue();
                var BASEQUANTITY = Data["BASEQUANTITY"].ToString().Trim();
                //var COMPLETEDELIVERY = Data["COMPLETEDELIVERY"].ToString().Trim();
                var COMPLETEDELIVERY = "NA";
                //var HEADERSCHEDULINGSTATUS = Data["HEADERSCHEDULINGSTATUS"].ToString().Trim();
                var HEADERSCHEDULINGSTATUS = "CSC";
                //var LINESCHEDULINGSTATUS = Data["LINESCHEDULINGSTATUS"].ToString().Trim();
                var LINESCHEDULINGSTATUS = "SC";
                var SALESORDERNUMBER = PONUMBER;
                var SALESORDERLINEITEM = ITEM;
                //var SALESORDERNUMBER = Data["SALESORDERNUMBER"].ToString().Trim();
                //var SALESORDERLINEITEM = Data["SALESORDERLINEITEM"].ToString().Trim();
                //var SOID = Data["SOID"].ToString().Trim();
                var SOID = "000000";
                //var SALESORDERHOLD = Data["SALESORDERHOLD"].ToString().Trim();
                var SALESORDERHOLD = "NA,NA,NA,NA,NA,NA";
                //var ECO_FCO = Data["ECO_FCO"].ToString().Trim();
                //var ECO_FCO = "";
                //var CARTONLABEL1 = Data["CARTONLABEL1"].ToString().Trim();
                //var CARTONLABEL2 = Data["CARTONLABEL2"].ToString().Trim();
                var ADDRESSCOMMIT = Data["ADDRESSCOMMIT"].ToString().Trim();

                O_I137_HEAD ADDRESSOBJ = new Func<O_I137_HEAD>(() =>
                {
                    switch (ADDRESSCOMMIT)
                    {
                        case "EIMILFGI (MILPITAS)": return EIMILFGI_MILPITAS;
                        case "EIAMSBONDFGI (AMSTERDAM)": return EIAMSBONDFGI_AMSTERDAM;
                        case "EIHKFGI (HONG KONG)": return EIHKFGI_HONG_KONG;
                        default: return null;
                    }
                })();
                if (ADDRESSOBJ == null)
                    throw new Exception($@"address is null!");
                var PODELIVERYDATE = Convert.ToDateTime(Data["PODELIVERYDATE"].ToString().Trim());
                T_C_SEQNO t_c_seqno = new T_C_SEQNO(sfcdb, DB_TYPE_ENUM.Oracle);
                var FID = t_c_seqno.GetLotno("JNP137ID", sfcdb);
                var TRANID = $@"DOA{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}{DateTime.Now.Millisecond}";
                var FILENAME = $@"{TRANID}.xml";
                var SITE = ((JuniperB2BPlantCode)Enum.Parse(typeof(JuniperB2BPlantCode), JuniperBase.GetCurrentSite(sfcdb.ORM))).ExtName();
                var VENDORID = ((JuniperB2BPlantCode)Enum.Parse(typeof(JuniperB2BPlantCode), JuniperBase.GetCurrentSite(sfcdb.ORM))).ExtValue();
                var Exists = sfcdb.ORM.Queryable<I137_I>().Where(t => t.PONUMBER == PONUMBER && t.ITEM == ITEM).OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Desc).First();
                var REV = Exists == null ? 0 : Exists.VERSION==null? 1: int.Parse(Exists.VERSION) + 1;
                var POCHANGEINDICATOR = Exists == null ? "I" : "U";
                var LASTCHANGEDATETIME = DateTime.Now;
                var res = sfcdb.ORM.Ado.UseTran(() =>
                {
                    sfcdb.ORM.Insertable(new I137_H()
                    {
                        ID = MesDbBase.GetNewID<O_I137_HEAD>(sfcdb.ORM, Customer.JUNIPER.ExtValue()),
                        F_ID = Convert.ToDouble(FID),
                        TRANID = TRANID,
                        F_PLANT = "BTS",
                        FILENAME = FILENAME,
                        MESSAGEID = TRANID,
                        HEADERCREATIONDATETIME = DateTime.Now,
                        RECIPIENTID = SITE,
                        PONUMBER = PONUMBER,
                        RMQQUOTENUMBER = "NA",
                        RMQPONUMBER = "NA",
                        PODOCTYPE = ENUM_I137_PoDocType.IDOA.ExtValue(),
                        POCHANGEINDICATOR = POCHANGEINDICATOR,
                        LASTCHANGEDATETIME = LASTCHANGEDATETIME,
                        VENDORID = VENDORID,
                        COMPLETEDELIVERY = COMPLETEDELIVERY,
                        SALESORDERNUMBER = SALESORDERNUMBER,
                        HEADERSCHEDULINGSTATUS = HEADERSCHEDULINGSTATUS,
                        SELLERPARTYID = VENDORID,
                        F_LASTEDITDT = DateTime.Now,
                        MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue(),
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now,
                        VERSION = REV.ToString(),
                        SHIPTOID = ADDRESSOBJ.SHIPTOID,
                        SHIPTOCOMPANY = ADDRESSOBJ.SHIPTOCOMPANY,
                        SHIPTOSTREETNAME = ADDRESSOBJ.SHIPTOSTREETNAME,
                        SHIPTOCITYNAME = ADDRESSOBJ.SHIPTOCITYNAME,
                        SHIPTOREGIONCODE = ADDRESSOBJ.SHIPTOREGIONCODE,
                        SHIPTOSTREETPOSTALCODE = ADDRESSOBJ.SHIPTOSTREETPOSTALCODE,
                        SHIPTOCOUNTRYCODE = ADDRESSOBJ.SHIPTOCOUNTRYCODE
                    }).ExecuteCommand();
                    sfcdb.ORM.Insertable(new I137_I()
                    {
                        ID = MesDbBase.GetNewID<O_I137_ITEM>(sfcdb.ORM, Customer.JUNIPER.ExtValue()),
                        F_ID = Convert.ToDouble(FID),
                        TRANID = TRANID,
                        F_PLANT = "BTS",
                        FILENAME = FILENAME,
                        MESSAGEID = TRANID,
                        PONUMBER = PONUMBER,
                        SOQTY = Convert.ToDouble(BASEQUANTITY).ToString("0.000"),
                        ITEM = ITEM,
                        ACTIONCODE = ACTIONCODE,
                        ITEMCHANGEINDICATOR = POCHANGEINDICATOR,
                        LASTCHANGEDATETIME = LASTCHANGEDATETIME,
                        SALESORDERLINEITEM = SALESORDERLINEITEM,
                        SOID = SOID,
                        JNP_PLANT = SITE,
                        SALESORDERHOLD = SALESORDERHOLD,
                        NETPRICE = NETPRICE,
                        PN = PN,
                        BASEQUANTITY = Convert.ToDouble(BASEQUANTITY).ToString("0.000"),
                        PODELIVERYDATE = PODELIVERYDATE,
                        PACKOUTLABEL = "UPC",
                        CUSTREQSHIPDATE = PODELIVERYDATE,
                        LINESCHEDULINGSTATUS = LINESCHEDULINGSTATUS,
                        QUANTITY = Convert.ToDouble(BASEQUANTITY).ToString("0.000"),
                        F_LASTEDITDT = DateTime.Now,
                        MFLAG = ENUM_I137_H_STATUS.CHECK_PASS.ExtValue(),
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now
                    }).ExecuteCommand();
                });
                if (!res.IsSuccess)
                    throw res.ErrorException;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Get137HByPoLine(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var ponumber = Data["PONUMBER"].ToString();
                var item = Data["ITEM"].ToString();
                var objs = sfcdb.ORM.Queryable<I137_H, I137_I>((h, i) => h.FILENAME == i.FILENAME).Where((h, i) => h.PONUMBER == ponumber && i.ITEM == item)
                    .OrderBy((h, i) => i.EDITTIME, OrderByType.Desc).Select((h, i) => new
                    {
                        i.PONUMBER,
                        h.SALESORDERNUMBER,
                        h.PODOCTYPE,
                        h.HEADERSCHEDULINGSTATUS,
                        h.COMPLETEDELIVERY,
                        h.ECO_FCO,
                        i.PN,
                        i.BASEQUANTITY,
                        i.SALESORDERLINEITEM,
                        i.PODELIVERYDATE,
                        i.SOID,
                        i.LINESCHEDULINGSTATUS,
                        i.SALESORDERHOLD,
                        i.CARTONLABEL1,
                        i.CARTONLABEL2,
                        i.ACTIONCODE
                    }).ToList();
                var obj = objs.FirstOrDefault();
                if (obj != null && obj.PODOCTYPE != ENUM_I137_PoDocType.IDOA.ExtValue())
                    throw new Exception($@"PONUMBER IS EXISTS!: {ponumber} IS DOF ORDER!");
                if (objs.Any(t => t.ACTIONCODE == ENUM_I137_Actioncode_Type.Cancel.ExtValue()))
                    throw new Exception($@"Order IS Cancel!: {ponumber} ,Item:{item} !");
                StationReturn.Data = obj;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        
        public void CreateNewSkuPlantCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var model = Data.ToObject<R_SKU_PLANT>();
                model.ID = MesDbBase.GetNewID<R_SKU_PLANT>(SFCDB.ORM, this.BU);
                model.CREATETIME = DateTime.Now;
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    if (!SFCDB.ORM.Queryable<R_SKU_PLANT>().Any(t => t.FOXCONN == model.FOXCONN || t.JUNIPER == model.JUNIPER))
                        SFCDB.ORM.Insertable(model).ExecuteCommand();
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
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

        public void GetSkuPlantCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var res = sfcdb.ORM.Queryable<R_SKU_PLANT>().ToList();
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void DelSkuPlantCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string[] ids = (string[])JsonConvert.Deserialize(Data["Ids"].ToString(), typeof(string[]));
                foreach (var item in ids)
                    oleDB.ORM.Deleteable<R_SKU_PLANT>().Where(t => t.ID == item.ToString()).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "Ok";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }

        public void GetBomByPo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var res = sfcdb.ORM.Queryable<O_ORDER_MAIN,R_PRE_WO_DETAIL,R_JNP_REPLACEPN>((m,w,r)=>
                new object[] { JoinType.Inner,m.PREWO ==w.WO ,JoinType.Left,m.UPOID==r.UPOID && r.VALIDFLAG == MesBool.Yes.ExtValue() && w.PARTNO == r.PARTNO})
                    .Where((m, w, r) => m.ID == poid).Select((m, w, r) =>new { m.UPOID,w.PARTNO,r.REPLACEPN,w.REQUESTQTY,w.PARTNOTYPE,r.ID }).ToList();
                if(res.Count==0)
                    res = sfcdb.ORM.Queryable<O_ORDER_MAIN, R_JNP_REPLACEPN>((m, r) => m.UPOID == r.UPOID).Where((m, r) => m.ID == poid).Select((m, r) => new { m.UPOID,r.PARTNO, r.REPLACEPN, REQUESTQTY = "", PARTNOTYPE = "", r.ID }).ToList();

                StationReturn.Data = res;                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void AddReplacePnByPoLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();                
                var poid = Data["POID"].ToString().Trim();
                var id = Data["ID"].ToString().Trim();
                var partno = Data["PARTNO"].ToString().Trim();
                var replacepn = Data["REPLACEPN"].ToString().Trim();
                var mainobj = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID == poid).ToList().FirstOrDefault();
                if (mainobj == null)
                    throw new Exception($@"PO is not exists!");
                var currentobjbyid = sfcdb.ORM.Queryable<R_JNP_REPLACEPN>().Where(t => t.ID == id && t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                var currentobjbypn = sfcdb.ORM.Queryable<R_JNP_REPLACEPN>().Where(t => t.PARTNO == partno && t.UPOID == mainobj.UPOID && t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();

                var res = sfcdb.ORM.Ado.UseTran(()=> {
                    if (currentobjbyid != null)
                    {
                        currentobjbyid.VALIDFLAG = MesBool.No.ExtValue();
                        sfcdb.ORM.Updateable(currentobjbyid).ExecuteCommand();
                    }
                    if (currentobjbypn != null)
                    {
                        currentobjbypn.VALIDFLAG = MesBool.No.ExtValue();
                        sfcdb.ORM.Updateable(currentobjbypn).ExecuteCommand();
                    }

                    sfcdb.ORM.Insertable(new R_JNP_REPLACEPN()
                    {
                        ID = MesDbBase.GetNewID<R_JNP_REPLACEPN>(sfcdb.ORM, Customer.JUNIPER.ExtValue()),
                        UPOID = mainobj.UPOID,
                        POID = mainobj.ID,
                        PARTNO = partno,
                        REPLACEPN = replacepn,
                        VALIDFLAG = MesBool.Yes.ExtValue(),
                        CREATETIME = DateTime.Now,
                        CREATEBY = this.LoginUser.EMP_NO
                    }).ExecuteCommand();
                });
                if (!res.IsSuccess)
                    throw new Exception(res.ErrorMessage);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }        
        public void DelReplacePnByPoLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var id = Data["ID"].ToString().Trim();
                sfcdb.ORM.Updateable<R_JNP_REPLACEPN>().SetColumns(t=>new R_JNP_REPLACEPN() { VALIDFLAG=MesBool.No.ExtValue()}).Where(t => t.ID == id).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void ResetPoStatusToPreWoGanerate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var poid = Data["POID"].ToString().Trim();
                var main = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.ID == poid).ToList().FirstOrDefault();
                var postatusobj = sfcdb.ORM.Queryable<O_PO_STATUS>().Where(t => t.POID == main.ID && t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                var iscrtd = JuniperBase.CheckCrtdStatusWithOrder(sfcdb.ORM, main, postatusobj);
                var iswaitcrtd = JnpConst.JnpWaitCrtdWoInSap.Contains(postatusobj.STATUSID);
                if (iscrtd || iswaitcrtd)
                {
                    var res = sfcdb.ORM.Ado.UseTran(()=> {
                        if (iscrtd)
                        {
                            var tecores = JuniperBase.TecoSapWo(this.BU, main.PREWO);
                            if (!tecores.issuccess)
                                throw new Exception(tecores.msg);
                        }
                        sfcdb.ORM.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == main.ID).ExecuteCommand();
                        sfcdb.ORM.Insertable(new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(sfcdb.ORM, Customer.JUNIPER.ExtValue()),
                            STATUSID = ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue(),
                            VALIDFLAG = MesBool.Yes.ExtValue(),
                            CREATETIME = DateTime.Now,
                            CREATEBY = LoginUser.EMP_NO,
                            EDITTIME = DateTime.Now,
                            EDITBY = LoginUser.EMP_NO,
                            POID = main.ID
                        }).ExecuteCommand();
                    });
                }
                else
                    throw new Exception("order has been produced . pls reverse wo and try again!"); 

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetBlackControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var res = sfcdb.ORM.Queryable<R_JNP_FULLMATCH>().Where(t=>t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().Distinct();
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void NewBlackControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            var model = Data.ToObject<R_JNP_FULLMATCH>();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                if (sfcdb.ORM.Queryable<R_JNP_FULLMATCH>().Any(t => t.PARENTPN == model.PARENTPN && t.SLOTTYPE == model.SLOTTYPE && t.BASETYPE == model.BASETYPE))
                    throw new Exception("ParentPn's SlotType is Allready exists!");

                var res = sfcdb.ORM.Insertable(new R_JNP_FULLMATCH()
                {
                    ID = MesDbBase.GetNewID<R_JNP_FULLMATCH>(sfcdb.ORM, this.BU),
                    PARENTPN = model.PARENTPN,
                    BASETYPE = model.BASETYPE,
                    SLOTTYPE = model.SLOTTYPE,
                    BLACKPN = model.BLACKPN,
                    QTY = model.QTY,
                    VALIDFLAG = MesBool.Yes.ExtValue(),
                    CREATETIME = DateTime.Now,
                    CREATEBY = this.LoginUser.EMP_NO
                }).ExecuteCommand();

                StationReturn.Message = "Add Success";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void DelBlackControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var ids = Data["Ids"].ToList();
                var res = sfcdb.ORM.Ado.UseTran(()=> {
                    foreach (string item in ids)                    
                        sfcdb.ORM.Updateable<R_JNP_FULLMATCH>().SetColumns(t => new R_JNP_FULLMATCH() { VALIDFLAG = MesBool.No.ExtValue(),EDITTIME =DateTime.Now,EDITBY = this.LoginUser.EMP_NO }).Where(t => t.ID == item).ExecuteCommand();                    
                });
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void UploadBlackControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    throw new Exception("Please Input Excel Data");
                }
                if (Data["FileName"] == null)
                {
                    throw new Exception("Please Input FileName");
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));
                //throw new Exception($@"上傳的Excel中沒有內容!");                

                string result = "";

                #region 写入数据库
                var targetlist = ObjectDataHelper.FromTable<R_JNP_FULLMATCH>(dt);
                var cols = dt.Columns;
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataColumn col in cols)
                    {
                        if( dr[col.ColumnName].ToString().Trim().Equals(""))
                            throw new Exception($@"{col.ColumnName} value is empty,pls check!");
                    }
                }

                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    foreach (var item in targetlist)
                    {
                        item.ID = MesDbBase.GetNewID<R_JNP_FULLMATCH>(SFCDB.ORM, this.BU);
                        item.VALIDFLAG = MesBool.Yes.ExtValue();
                        item.CREATETIME = DateTime.Now;
                        item.CREATEBY = this.LoginUser.EMP_NO;
                        var exists = SFCDB.ORM.Queryable<R_JNP_FULLMATCH>().Where(t => t.PARENTPN == item.PARENTPN && t.SLOTTYPE == item.SLOTTYPE && t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                        if(exists!=null)
                            SFCDB.ORM.Updateable<R_JNP_FULLMATCH>().SetColumns(t => new R_JNP_FULLMATCH() { VALIDFLAG = MesBool.No.ExtValue(), EDITTIME = DateTime.Now, EDITBY = this.LoginUser.EMP_NO }).Where(t => t.ID == exists.ID).ExecuteCommand();
                    }
                    SFCDB.ORM.Insertable(targetlist).ExecuteCommand();
                });

                if (res.IsSuccess)
                    result = "All Upload OK ! ";
                else
                    throw res.ErrorException;
                #endregion

                StationReturn.Message = result;
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }
        //public void EditBlackControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    OleExec sfcdb = null;
        //    var model = Data.ToObject<R_JNP_FULLMATCH>();
        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        if (!sfcdb.ORM.Queryable<R_JNP_FULLMATCH>().Any(t => t.PARENTPN == model.PARENTPN && t.SLOTTYPE == model.SLOTTYPE))
        //            throw new Exception("ParentPn's SlotType is not exists!");
        //        //model.EDITTIME =
        //        var res = sfcdb.ORM.Updateable(new R_JNP_FULLMATCH()
        //        {
        //            ID = MesDbBase.GetNewID<R_JNP_FULLMATCH>(sfcdb.ORM, this.BU),
        //            PARENTPN = model.PARENTPN,
        //            SLOTTYPE = model.SLOTTYPE,
        //            QTY = model.QTY,
        //            PARTNO = model.PARTNO,
        //            VALIDFLAG = MesBool.Yes.ExtValue(),
        //            CREATETIME = DateTime.Now,
        //            CREATEBY = this.LoginUser.EMP_NO
        //        }).ExecuteCommand();

        //        StationReturn.Message = "Add Success";
        //        StationReturn.Status = StationReturnStatusValue.Pass;
        //        StationReturn.Data = "";
        //    }
        //    catch (Exception e)
        //    {
        //        StationReturn.Message = e.Message;
        //        StationReturn.Status = StationReturnStatusValue.Fail;
        //        StationReturn.Data = "";
        //    }
        //    finally
        //    {
        //        DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}


        public void GetOrderHisByWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var wo = Data["WO"].ToString().Trim();
                var swo = sfcdb.ORM.Queryable<R_PRE_WO_HEAD,R_PRE_WO_HEAD>((r1,r2)=>r1.PONO==r2.PONO && r1.POLINE==r2.POLINE).Where((r1, r2) =>r1.WO==wo)
                    .OrderBy((r1, r2) =>r2.CREATETIME,OrderByType.Desc)
                    .Select((r1, r2) => r2).ToList();
                if (swo.Count==0)
                    throw new Exception("no data!");
                var main = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == swo.FirstOrDefault().PONO && t.POLINE == swo.FirstOrDefault().POLINE).ToList().FirstOrDefault();
                var res = new List<object>();                
                res.Add(new { PoNumber= main.PONO,Poline= main.POLINE,WorkOrderNo= main.PREWO,CreateTime=main.EDITTIME,Status = "Actived" });
                foreach (var item in swo)                
                    if(item.WO != main.PREWO)
                        res.Add(new { PoNumber = item.PONO, Poline = item.POLINE, WorkOrderNo = item.WO, CreateTime=item.CREATETIME, Status = "Invalid" });                
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }

    public class OrderModle
    {
        [ExcelIgone]
        public string ID { get; set; }
        public string PLANT { get; set; }
        public string HEADERSCHSTATUS { get; set; }
        public string LINESCHSTATUS { get; set; }
        public string COMPLETEDELIVER { get; set; }
        public DateTime? LAST138SUBMISSION { get; set; }
        public string UPOID { get; set; }
        public string POTYPE { get; set; }
        public string PONO { get; set; }
        public string POLINE { get; set; }
        public string VERSION { get; set; }
        public string POSTATUS { get; set; }
        public string HOLD { get; set; }
        public string QTY { get; set; }
        public string UNITPRICE { get; set; }
        public string PREWO { get; set; }
        public string JUNIPERPID { get; set; }
        public string GROUPID { get; set; }
        public string PLANTCODE { get; set; }
        public string PID { get; set; }
        public DateTime? DELIVERY { get; set; }
        public DateTime? CRSD { get; set; }
        public string SO { get; set; }
        public string SOLN { get; set; }
        public string USERITEMTYPE { get; set; }
        public string OFFERINGTYPE { get; set; }
        public string LASTCHANGETIME { get; set; }
        public string COMPLETED { get; set; }
        public DateTime? COMPLETIME { get; set; }
        public string CLOSED { get; set; }
        public DateTime? CLOSETIME { get; set; }
        public string CANCEL { get; set; }
        public DateTime? CANCELTIME { get; set; }
        public string CUSTOMSW { get; set; }
        public string ECO_FCO { get; set; }
        public string COUNTRYSPECIFICLABE { get; set; }
        public string CARTONLABEL1 { get; set; }
        public string CARTONLABEL2 { get; set; }
        public string PACKOUTLABEL { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
        [ExcelIgone]
        public string ORIGINALID { get; set; }
        [ExcelIgone]
        public string ORIGINALITEMID { get; set; }
        [ExcelIgone]
        public string ITEMID { get; set; }
        public string EXCEPTIONINFO { get; set; }
        public string ORDERTYPE { get; set; }
    }
}
