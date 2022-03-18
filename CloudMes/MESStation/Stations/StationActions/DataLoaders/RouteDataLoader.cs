using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class RouteDataLoader
    {
        /// <summary>
        /// 加載REWORK工站ROUTE LIST站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RouteDetailDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            List<string> NoCheckStation = new List<string>();
            MESStationSession StationSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (StationSave == null)
            {
                StationSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationSave);
            }
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder ObjWo = (WorkOrder)WoSession.Value;

            List<R_Station_Action_Para> NoCheckSession = Paras.FindAll(t => t.SESSION_TYPE.Equals("NOCHECK"));
            
            try
            {
                MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "StationName");
                List<object> snStationList = I.DataForUse;
                snStationList.Clear();
                snStationList.Add(""); ///BY SDL  加載頁面默認賦予空值,操作員必須點選其他有內容選項

                //if (Station.BU == "VERTIV" && ObjWo.START_STATION != "SILOADING" && ObjWo.START_STATION != "SMTLOADING")
                if (Station.BU != "HWD" && ObjWo.START_STATION != "SILOADING" && ObjWo.START_STATION != "SMTLOADING")
                {
                    //VT 如果生管在手動轉重工工單時配置了起始工站，且起始工站不是LOADING工站，則掃入這個工單的SN從生管指定的工站開始重工 //DCN 也加這個邏輯                 
                    snStationList.Add(ObjWo.START_STATION);
                }
                else
                {
                    Route routeDetail = new Route(ObjWo.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                    RouteDetail h = routeDetailList.Find(t => t.STATION_NAME == ObjWo.START_STATION);
                    if (h == null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000204", new string[] { ObjWo.WorkorderNo, ObjWo.START_STATION });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    for (int i = 0; i < routeDetailList.Count; i++)
                    {
                        if (routeDetailList[i].SEQ_NO >= h.SEQ_NO)
                        {
                            snStationList.Add(routeDetailList[i].STATION_NAME);
                            if (routeDetailList[i].DIRECTLINKLIST != null)
                            {
                                foreach (var item in routeDetailList[i].DIRECTLINKLIST)
                                {
                                    snStationList.Add(item.STATION_NAME);
                                }
                            }
                        }
                    }
                    for (int i = 0; i < NoCheckSession.Count; i++)
                    {
                        snStationList.Remove(NoCheckSession[i].VALUE.ToString());
                    }
                }
                StationSave.Value = snStationList;
                Station.AddMessage("MES00000029", new string[] { "RouteID :", ObjWo.RouteID }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}
