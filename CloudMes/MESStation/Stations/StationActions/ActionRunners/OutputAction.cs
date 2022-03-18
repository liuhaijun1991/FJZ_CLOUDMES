using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class OutputAction
    {
        /// <summary>
        /// SN 列印時輸入工單時的輸出
        /// 至少兩個參數，WO 和 SKU
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnPrintAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 2)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMesg);
            }
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006",new string[] { "WO"});
                throw new MESReturnMessage(ErrMesg);
            }
            WorkOrder Wo = (WorkOrder)WoSession.Value;

            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006",new string[] { "SKU"});
                throw new MESReturnMessage(ErrMesg);
            }
            SKU Sku = (SKU)SkuSession.Value;


            if (Paras.Exists(t => t.SESSION_TYPE == "WOQTY" && t.SESSION_KEY == "1"))
            {
                MESStationSession WOQTYSession = Station.StationSession.Find(t => t.MESDataType == "WOQTY" && t.SessionKey == "1");
                if (WOQTYSession != null)
                {
                    WOQTYSession.Value = Wo.WORKORDER_QTY;
                }
                else
                {
                    Station.StationSession.Add(new MESStationSession() { MESDataType = "WOQTY", SessionKey = "1", Value = Wo.WORKORDER_QTY });
                }
            }

            T_R_WO_REGION T_Region = new T_R_WO_REGION(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (Paras.Exists(t => t.SESSION_TYPE == "RESTQTY" && t.SESSION_KEY == "1"))
            {
                MESStationSession RESTQTYSession = Station.StationSession.Find(t => t.MESDataType == "RESTQTY" && t.SessionKey == "1");
                if (RESTQTYSession != null)
                {
                    RESTQTYSession.Value = Wo.WORKORDER_QTY-T_Region.GetWoDistributed(Wo.WorkorderNo,Station.SFCDB);
                }
                else
                {
                    Station.StationSession.Add(new MESStationSession() { MESDataType = "RESTQTY", SessionKey = "1", Value = Wo.WORKORDER_QTY - T_Region.GetWoDistributed(Wo.WorkorderNo, Station.SFCDB) });
                }
            }

            if (Paras.Exists(t => t.SESSION_TYPE == "SKUNO" && t.SESSION_KEY == "1"))
            {
                MESStationSession SKUNOSession = Station.StationSession.Find(t => t.MESDataType == "SKUNO" && t.SessionKey == "1");
                if (SKUNOSession != null)
                {
                    SKUNOSession.Value = Sku.SkuNo;
                }
                else
                {
                    Station.StationSession.Add(new MESStationSession() { MESDataType = "SKUNO", SessionKey = "1", Value = Sku.SkuNo });
                }
            }

            if (Paras.Exists(t => t.SESSION_TYPE == "SKUVER" && t.SESSION_KEY == "1"))
            {
                MESStationSession SKUVERSession = Station.StationSession.Find(t => t.MESDataType == "SKUVER" && t.SessionKey == "1");
                if (SKUVERSession != null)
                {
                    SKUVERSession.Value = Sku.Version;
                }
                else
                {
                    Station.StationSession.Add(new MESStationSession() { MESDataType = "SKUVER", SessionKey = "1", Value = Sku.Version });
                }
            }
        }

        public static void PressFitOutputAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 3)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMesg);
            }
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "WO" });
                throw new MESReturnMessage(ErrMesg);
            }
            WorkOrder Wo = (WorkOrder)WoSession.Value;

            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKU" });
                throw new MESReturnMessage(ErrMesg);
            }
            SKU Sku = (SKU)SkuSession.Value;

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SkuSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "NextStation" });
                throw new MESReturnMessage(ErrMesg);
            }
            List<string> NextStationList = (List<string>)NextStationSession.Value;


            if (Paras.Exists(t => t.SESSION_TYPE == "WORKORDER" && t.SESSION_KEY == "1"))
            {
                MESStationSession WorkOrderSession = Station.StationSession.Find(t => t.MESDataType == "WORKORDER" && t.SessionKey == "1");
                if (WorkOrderSession != null)
                {
                    WorkOrderSession.Value = Wo.WorkorderNo;
                }
                else
                {
                    Station.StationSession.Add(new MESStationSession() { MESDataType = "WORKORDER", SessionKey = "1", Value = Wo.WorkorderNo });
                }
            }

          
            if (Paras.Exists(t => t.SESSION_TYPE == "SKUNO" && t.SESSION_KEY == "1"))
            {
                MESStationSession SKUNOSession = Station.StationSession.Find(t => t.MESDataType == "SKUNO" && t.SessionKey == "1");
                if (SKUNOSession != null)
                {
                    SKUNOSession.Value = Sku.SkuNo;
                }
                else
                {
                    Station.StationSession.Add(new MESStationSession() { MESDataType = "SKUNO", SessionKey = "1", Value = Sku.SkuNo });
                }
            }

            if (Paras.Exists(t => t.SESSION_TYPE == "NEXTSTATION" && t.SESSION_KEY == "1"))
            {
                MESStationSession NEXTSTATIONSession = Station.StationSession.Find(t => t.MESDataType == "NEXTSTATION" && t.SessionKey == "1");
                if (NextStationList.Count > 0)
                {
                    if (NEXTSTATIONSession != null)
                    {
                        NEXTSTATIONSession.Value = NextStationList[0];
                    }
                    else
                    {
                        Station.StationSession.Add(new MESStationSession() { MESDataType = "NEXTSTATION", SessionKey = "1", Value = NextStationList[0] });
                    }
                }
            }
        }
    }
}
