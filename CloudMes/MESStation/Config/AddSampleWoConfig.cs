
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
   public class AddSampleWoConfig: MesAPIBase
    {
        protected APIInfo FQueryWoBySku = new APIInfo()
        {
            FunctionName = "QueryWoBySku",
            Description = "Query Wo By Sku",
            Parameters = new List<APIInputInfo>()

            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        private Row_C_ROUTE rowRoute;
        private List<C_ROUTE_DETAIL> routeDetailList;
        private List<string> keypartIDList;
        public T_C_ROUTE C_ROUTE ;
        public T_C_ROUTE_DETAIL RouteDetail;
        public T_C_KP_LIST t_c_kp_list;
       
        
        public void QueryWoBySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = null;
            T_R_WO_BASE cSkuWo = null;
            List<R_WO_BASE> cSkuWoList = new List<R_WO_BASE>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuWo = new T_R_WO_BASE(oleDB, DBTYPE);
                cSkuWoList = cSkuWo.GetSampleWOBySku(oleDB,Sku);
                if (cSkuWoList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(cSkuWoList.Count);
                    StationReturn.Data = cSkuWoList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void AddSampleWoWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            C_SKU SkuObj = null;
            string Sku = Data["SKUNO"].ToString().Trim();
            string Qty = Data["WOQTY"].ToString().Trim();
            //2020.10.12 modify by fgg 應VT PE要求所有機種都可以創建虛擬工單 
            //if (!Sku.StartsWith("XN")) {
            //    StationReturn.Status = StationReturnStatusValue.Fail;
            //    StationReturn.MessageCode = "MSGCODE20190111084817";
            //    return;
            //}
            OleExec oleDB = null;
            T_R_WO_BASE cSkuWo = null;
            T_C_SKU cSku = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSku = new T_C_SKU(oleDB, DBTYPE);
                if (!cSku.SkuNoIsExist(Sku, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000245";
                    StationReturn.MessagePara = new List<object>() { Sku };
                }
                else
                {
                    cSkuWo = new T_R_WO_BASE(oleDB, DBTYPE);
                    
                    SkuObj = cSku.GetSku(Sku, oleDB);
                    C_ROUTE = new T_C_ROUTE(oleDB, DBTYPE);
                    rowRoute = (Row_C_ROUTE)C_ROUTE.GetRouteBySkuno(SkuObj.ID, oleDB, DBTYPE);
                    RouteDetail = new T_C_ROUTE_DETAIL(oleDB, DBTYPE);
                    routeDetailList = RouteDetail.GetByRouteIdOrderBySEQASC(rowRoute.ID, oleDB);
                    if (routeDetailList == null || routeDetailList.Count == 0)
                    {
                        string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112458", new string[] { rowRoute.ID });
                        throw new Exception(errMessage);
                    }
                    t_c_kp_list = new T_C_KP_LIST(oleDB, DBTYPE);
                    keypartIDList = t_c_kp_list.GetListIDBySkuno(SkuObj.SKUNO, oleDB);
                    if (keypartIDList.Count > 0 && keypartIDList.Count != 1)
                    {
                        string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112748", new string[] { SkuObj.SKUNO });
                        throw new Exception(errMessage);
                    }
                    //獲取廠別 Add By ZHB 20200713
                    var plantList = oleDB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "GET_PLANT" && t.CONTROL_TYPE == "PLANT").Select(t => t.CONTROL_VALUE).ToList();
                    var plant = plantList != null && plantList.Count > 0 ? plantList[0].ToString() : "";
                    //獲取系列 Add By ZHB 20200713
                    var seriesList = oleDB.ORM.Queryable<C_SERIES>().Where(t => t.ID == SkuObj.C_SERIES_ID).Select(t => t.SERIES_NAME).ToList();
                    var series = seriesList != null && seriesList.Count > 0 ? seriesList[0].ToString() : "";

                    T_C_SEQNO T_C_Seqno = new T_C_SEQNO(oleDB, DBTYPE);
                    Row_R_WO_BASE rowRwoBase = (Row_R_WO_BASE)cSkuWo.NewRow();
                    rowRwoBase.ID = cSkuWo.GetNewID(this.BU, oleDB, DBTYPE);
                    rowRwoBase.WORKORDERNO = T_C_Seqno.GetLotno("SAMPLEWO", oleDB);
                    rowRwoBase.PLANT = plant;
                    rowRwoBase.RELEASE_DATE = GetDBDateTime();
                    rowRwoBase.DOWNLOAD_DATE = GetDBDateTime();
                    rowRwoBase.PRODUCTION_TYPE = "BTO";
                    rowRwoBase.WO_TYPE = "REGULAR";
                    rowRwoBase.SKUNO = Sku;
                    rowRwoBase.SKU_VER = SkuObj.VERSION;
                    rowRwoBase.SKU_SERIES = series;
                    rowRwoBase.SKU_NAME = SkuObj.SKU_NAME;
                    rowRwoBase.SKU_DESC = SkuObj.DESCRIPTION;
                    rowRwoBase.CUST_PN = SkuObj.CUST_PARTNO;
                    rowRwoBase.CUST_PN_VER = "";
                    rowRwoBase.CUSTOMER_NAME = SkuObj.CUST_SKU_CODE;
                    rowRwoBase.ROUTE_ID = rowRoute.ID;
                    rowRwoBase.START_STATION = routeDetailList[0].STATION_NAME;
                    rowRwoBase.KP_LIST_ID = (keypartIDList != null && keypartIDList.Count > 0) ? keypartIDList[0] : "";
                    rowRwoBase.CLOSED_FLAG = "0";
                    rowRwoBase.WORKORDER_QTY = Convert.ToDouble(Qty);
                    rowRwoBase.INPUT_QTY = 0;
                    rowRwoBase.FINISHED_QTY = 0;
                    rowRwoBase.SCRAPED_QTY = 0;
                    rowRwoBase.STOCK_LOCATION = "";
                    rowRwoBase.PO_NO = "";
                    rowRwoBase.CUST_ORDER_NO = "";
                    rowRwoBase.ROHS = "R6";
                    rowRwoBase.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowRwoBase.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowRwoBase.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

    }
}
