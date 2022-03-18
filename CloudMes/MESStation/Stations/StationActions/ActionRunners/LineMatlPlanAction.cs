using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESPubLab.Json;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class LineMatlPlanAction
    {
        public static void LoadingDeductsMaterial(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (woSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            WorkOrder woObj = (WorkOrder)woSession.Value;
            DateTime sysdate = Station.SFCDB.ORM.GetDate();
            var checkoutList = Station.SFCDB.ORM.Queryable<R_JNP_LINE_STOCK_DETAIL>()
                .Where(r => r.DETAIL1 == woObj.WorkorderNo && r.OPTION_TYPE == 0 && r.DETAIL3 == Station.Line).ToList();
            if (checkoutList.Count > 0)
            {                
                foreach (var checkoutObj in checkoutList)
                {
                    var stock = Station.SFCDB.ORM.Queryable<R_JNP_LINE_STOCK>().Where(t => t.STOCK_LOCATION == checkoutObj.STOCK_LOCATION && t.PN == checkoutObj.PN).First();
                    if (stock == null) 
                    {
                        throw new MESReturnMessage($"Material [{checkoutObj.PN}] not in {checkoutObj.STOCK_LOCATION},Please to do the checkout.");
                    }
                    if (stock.QTY == 0)
                    {
                        throw new MESReturnMessage($"Material [{checkoutObj.PN}] is used up");
                    }
                    stock.QTY = stock.QTY - 1;
                    stock.EDIT_EMP = Station.LoginUser.EMP_NO;
                    stock.EDIT_TIME = sysdate;
                    Station.SFCDB.ORM.Updateable(stock).Where(r => r.ID == stock.ID).ExecuteCommand();

                    var stockDetail = new R_JNP_LINE_STOCK_DETAIL();
                    stockDetail.ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_JNP_LINE_STOCK_DETAIL");
                    stockDetail.STOCK_LOCATION = checkoutObj.STOCK_LOCATION;
                    stockDetail.PN = checkoutObj.PN;
                    stockDetail.QTY = -1;
                    stockDetail.OPTION_TYPE = 1;
                    stockDetail.FROM_LOCATION = "CWIP";
                    stockDetail.TO_LOCATION = "WO";
                    stockDetail.SAP_FLAG = 0;
                    stockDetail.DETAIL1 = woObj.WorkorderNo;
                    stockDetail.DETAIL2 = snSession.InputValue;
                    stockDetail.DETAIL3 = Station.Line;
                    stockDetail.EDIT_EMP = Station.LoginUser.EMP_NO;
                    stockDetail.EDIT_TIME = sysdate;
                    Station.SFCDB.ORM.Insertable(stockDetail).ExecuteCommand();
                }
            }
            else
            {
                var autoKpList = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(woObj.WorkorderNo, "JuniperAutoKPConfig",Station.SFCDB);
                if (autoKpList.Count > 0)
                {
                    var lineList = Station.SFCDB.ORM.Queryable<R_F_CONTROL>()
                        .Where(r => r.FUNCTIONNAME == "STOCK_GROUP" && r.CATEGORY == "STOCK_GROUP" && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.EXTVAL == Station.Line)
                        .ToList();
                    string stockLocation = lineList.FirstOrDefault()?.VALUE;
                    foreach (var item in autoKpList)
                    {
                        var stock = Station.SFCDB.ORM.Queryable<R_JNP_LINE_STOCK>().Where(t => t.STOCK_LOCATION == stockLocation && t.PN == item.PN_7XX).First();
                        if (stock == null)
                        {
                            stock = new R_JNP_LINE_STOCK()
                            {
                                ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_JNP_LINE_STOCK"),
                                PN = item.PN_7XX,
                                QTY = 0,
                                STOCK_LOCATION = stockLocation,

                            };
                            Station.SFCDB.ORM.Insertable(stock).ExecuteCommand();
                        }
                        stock.QTY = stock.QTY - 1;
                        stock.EDIT_EMP = Station.LoginUser.EMP_NO;
                        stock.EDIT_TIME = sysdate;
                        Station.SFCDB.ORM.Updateable(stock).Where(r => r.ID == stock.ID).ExecuteCommand();

                        var stockDetail = new R_JNP_LINE_STOCK_DETAIL();
                        stockDetail.ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_JNP_LINE_STOCK_DETAIL");
                        stockDetail.STOCK_LOCATION = stockLocation;
                        stockDetail.PN = item.PN_7XX;
                        stockDetail.QTY = -1;
                        stockDetail.OPTION_TYPE = 1;
                        stockDetail.FROM_LOCATION = "CWIP";
                        stockDetail.TO_LOCATION = "WO";
                        stockDetail.SAP_FLAG = 0;
                        stockDetail.DETAIL1 = woObj.WorkorderNo;
                        stockDetail.DETAIL2 = snSession.InputValue;
                        stockDetail.DETAIL3 = Station.Line;
                        stockDetail.EDIT_EMP = Station.LoginUser.EMP_NO;
                        stockDetail.EDIT_TIME = sysdate;
                        Station.SFCDB.ORM.Insertable(stockDetail).ExecuteCommand();
                    }
                }
            }
        }
    }
}
