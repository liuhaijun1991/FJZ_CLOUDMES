using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using MESPubLab.MESStation.MESReturnView;
using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class PanelActions
    {
        /// <summary>
        /// Panel 投入
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelInputAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder WorkOrder = null;
            T_R_SN SnTable = null;
            T_R_WO_BASE WoTable = null;
            T_R_PANEL_SN PanelTable = null;
            double LinkQty = 0d;
            string PanelSn = string.Empty;
            List<R_SN> SNs = null;
            R_SN OriginalSN = null;
            List<string> SNIds = null;
            List<R_PANEL_SN> RPanelSNs = new List<R_PANEL_SN>();
            SN SNObj = new SN();
            string NextStation = "";
            int SeqNo = 1;
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            if (Paras.Count != 5)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "4", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //Panel1
            MESStationSession PanelSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Wo1
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //LinkQty1
            MESStationSession LinkCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (LinkCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            MESStationSession sessionWOInputQty = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionWOInputQty == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            try
            {
                PanelSn = PanelSnSession.Value.ToString();
                WorkOrder = (WorkOrder)WoSession.Value;
                LinkQty = double.Parse(LinkCountSession.Value.ToString());
                SNs = new List<R_SN>((int)LinkQty);

                for (int i = 1; i <= LinkQty; i++)
                {
                    OriginalSN = new R_SN();
                    OriginalSN.SKUNO = WorkOrder.SkuNO;
                    OriginalSN.WORKORDERNO = WorkOrder.WorkorderNo;
                    OriginalSN.PLANT = WorkOrder.PLANT;
                    OriginalSN.ROUTE_ID = WorkOrder.RouteID;
                    OriginalSN.STARTED_FLAG = "1";
                    OriginalSN.PACKED_FLAG = "0";
                    OriginalSN.COMPLETED_FLAG = "0";
                    OriginalSN.SHIPPED_FLAG = "0";
                    OriginalSN.REPAIR_FAILED_FLAG = "0";
                    OriginalSN.CURRENT_STATION = Station.StationName;
                    OriginalSN.CUST_PN = WorkOrder.CUST_PN;
                    OriginalSN.SCRAPED_FLAG = "0";
                    OriginalSN.PRODUCT_STATUS = "FRESH";
                    OriginalSN.REWORK_COUNT = 0d;
                    OriginalSN.VALID_FLAG = "1";
                    OriginalSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                    SNs.Add(OriginalSN);
                }

                SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
                SNIds = SnTable.AddToRSn(SNs, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB); //批量插入到 R_SN 中
                foreach (string SNId in SNIds)
                {
                    R_PANEL_SN RPanelSN = new R_PANEL_SN();
                    RPanelSN.SN = SNId;
                    RPanelSN.PANEL = PanelSn;
                    RPanelSN.WORKORDERNO = WorkOrder.WorkorderNo;
                    RPanelSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                    RPanelSN.SEQ_NO = SeqNo++;
                    RPanelSNs.Add(RPanelSN);
                }
                PanelTable = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                PanelTable.AddSnToPanel(RPanelSNs, Station.BU, Station.SFCDB); //批量插入到 R_PANEL_SN 中

                #region 寫入 r_sn_kp add by fgg 2018.5.23
                T_C_KP_LIST c_kp_list = new T_C_KP_LIST(Station.SFCDB, Station.DBType);
                if (WorkOrder.KP_LIST_ID != "" && c_kp_list.KpIDIsExist(WorkOrder.KP_LIST_ID, Station.SFCDB))
                {
                    SN snObject = new SN();
                    foreach (R_SN r_sn in SNs)
                    {
                        snObject.InsertR_SN_KP(WorkOrder, r_sn, Station.SFCDB, Station, Station.DBType);
                    }
                }
                #endregion

                //調用 SN 過站 插入記錄到過站記錄中 未完待續
                WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
                WoTable.AddCountToWo(WorkOrder.WorkorderNo, LinkQty, Station.SFCDB); // 更新 R_WO_BASE 中的數據

                Route routeDetail = new Route(WorkOrder.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<string> snStationList = new List<string>();
                List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == Station.StationName).FirstOrDefault();
                string nextStation1 = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
                snStationList.Add(nextStation1);
                if (R.DIRECTLINKLIST != null)
                {
                    foreach (var item in R.DIRECTLINKLIST)
                    {
                        snStationList.Add(item.STATION_NAME);
                    }
                }

                Row_R_WO_BASE newRowWo = WoTable.LoadWorkorder(WorkOrder.WorkorderNo, Station.SFCDB);
                sessionWOInputQty.Value = newRowWo.INPUT_QTY;

                NextStation = SNObj.StringListToString(snStationList);
                Station.AddMessage("MES00000055", new string[] { PanelSnSession.Value.ToString(), NextStation }, StationMessageState.Pass); //回饋消息到前台
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void PanelLogAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            try
            {
                string strSn = Input.Value.ToString();
                SN sn = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_SN_STATION_DETAIL cStation = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_SN_STATION_DETAIL rStation = (Row_R_SN_STATION_DETAIL)cStation.NewRow();
                rStation.ID = sn.ID;
                rStation.NEXT_STATION = sn.NextStation;
                rStation.SKUNO = sn.SkuNo;
                rStation.WORKORDERNO = sn.WorkorderNo;
                rStation.ROUTE_ID = sn.RouteID;
                rStation.PLANT = sn.Plant;
                rStation.STARTED_FLAG = sn.StartedFlag;
                rStation.START_TIME = (DateTime)sn.StartTime;
                rStation.PACKED_FLAG = sn.PackedFlag;
                rStation.COMPLETED_FLAG = sn.CompletedFlag;
                rStation.COMPLETED_TIME = sn.CompletedTime ?? DateTime.Now;
                rStation.SHIPPED_FLAG = sn.ShippedFlag;
                rStation.SHIPDATE = sn.ShipDate ?? DateTime.Now;
                rStation.REPAIR_FAILED_FLAG = sn.RepairFailedFlag;
                rStation.CURRENT_STATION = sn.CurrentStation;
                rStation.KP_LIST_ID = sn.KeyPartList[0].ID;
                rStation.PO_NO = sn.PONO;
                rStation.CUST_ORDER_NO = sn.CustomerOrderNo;
                rStation.CUST_PN = "";
                rStation.BOXSN = sn.BoxSN;
                rStation.SCRAPED_FLAG = sn.ScrapedFlag;
                rStation.SCRAPED_TIME = sn.ScrapedTime ?? DateTime.Now;
                rStation.PRODUCT_STATUS = sn.ProductStatus;
                rStation.REWORK_COUNT = sn.ReworkCount;
                rStation.VALID_FLAG = sn.ValidFlag;
                rStation.EDIT_EMP = "";
                rStation.EDIT_TIME = DateTime.Now;
                string strRet = Station.SFCDB.ExecSQL(rStation.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
                }
                else
                {
                    throw new Exception("ERROR!");
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void PanelSnInputAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            MESStationInput input = null;
            string strPanel = Input.Value.ToString();
            Panel panel = new Panel();
            panel.Init(strPanel, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Hashtable tempTable = new Hashtable();
            tempTable.Add("BU", Station.BU);
            tempTable.Add("Panel", panel.PanelNo);
            tempTable.Add("WO", s.Value);
            tempTable.Add("User", Station.User.EMP_NO);
            Boolean b = panel.CreatePanel(tempTable, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (b == false)
            {
                Station.AddMessage("MES00000029", new string[] { "Panel", strPanel.ToString() }, StationMessageState.Fail);
            }
            input = Station.FindInputByName("Panel");
            Station.NextInput = input;
            Station.AddMessage("MES00000029", new string[] { "Panel", strPanel.ToString() }, StationMessageState.Pass);

            string strSql = "update r_sn set current_station=:CurrentStation,next_station=:nextStation where sn=:sn";
            int count = Station.SFCDB.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[3] { new System.Data.OleDb.OleDbParameter("CurrentStation", Station.StationName), new System.Data.OleDb.OleDbParameter("nextStation", "Insp"), new System.Data.OleDb.OleDbParameter("sn", strPanel) });
        }

        /// <summary>
        /// 補 Allpart 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddAPRecordsAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string Message = string.Empty;
            //modify by LLF 2018-01-27
            //R_SN SN = null;
            string StrSN = "";
            string StrWo = "";
            string StrStation = "";

            if (Paras.Count <= 1)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "1", Paras.Count.ToString() });
                throw new MESReturnMessage(Message);
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //SN = (R_SN)SnSession.Value;
            StrSN = SnSession.InputValue.ToString();

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //SN = (R_SN)SnSession.Value;
            StrWo = WoSession.Value.ToString();
            StrStation = Station.StationName;

            Message = table.AddAPRecords(StrSN, StrWo, StrStation, Station.Line, Station.SFCDB);
            if (Message.Equals("OK"))
            {
                Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
            }
            else
            {
                throw new MESReturnMessage(Message);
            }
        }


        /// <summary>
        /// 補 SMTLOADING 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddSMTLoadingRecordsAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string Message = string.Empty;
            //R_PANEL_SN PanelSN = null;
            string StrPanel = "";
            string StrWO = "";
            string StrTrCode = "";
            Dictionary<string, DataRow> TrSnTable = null;
            string Process = string.Empty;
            double LinkQty = 0d;
            string MacAddress = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 7)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(Message);
            }


            //獲取 R_PANEL_SN 對象
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //PanelSN = (R_PANEL_SN)PanelSession.Value;
            StrPanel = PanelSession.Value.ToString();
            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            //獲取面別
            Process = Paras[2].VALUE.ToString();

            //獲取連板數
            MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (LinkQtySession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            LinkQty = double.Parse(LinkQtySession.Value.ToString());

            //獲取 MAC 地址
            MacAddress = Paras[4].VALUE.ToString();

            //add by LLF 2017-01-24 begin
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WoSession.Value.ToString();

            MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);

            if (TrCodeSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }

            StrTrCode = TrCodeSession.Value.ToString();

            //Message = table.AddSMTLoadingRecords(PanelSN.WORKORDERNO, StrPanel, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, Station.SFCDB);
            try
            {

                apdb = Station.APDB;
                Message = table.AddSMTLoadingRecords(StrWO, StrPanel, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, StrTrCode, "T", apdb);
                if (Message.Equals("OK"))
                {
                    Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
                    //Station.DBS["APDB"].Return(apdb);
                }
                else
                {
                    //Station.DBS["APDB"].Return(apdb);
                    throw new MESReturnMessage(Message);
                }
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    //Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }
        }

        /// <summary>
        /// 補 連板 SMTLOADING 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddMULTISMTLoadingRecordsAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string Message = string.Empty;
            //R_PANEL_SN PanelSN = null;
            string StrSN = "";
            string StrWO = "";
            string StrTrCode = "";
            Dictionary<string, DataRow> TrSnTable = null;
            string Process = string.Empty;
            double LinkQty = 0d;
            string MacAddress = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 8)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "8", Paras.Count.ToString() });
                throw new MESReturnMessage(Message);
            }


            //獲取 R_PANEL_SN 對象
            MESStationSession SNlSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNlSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //PanelSN = (R_PANEL_SN)PanelSession.Value;
            StrSN = SNlSession.Value.ToString();
            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            //獲取面別
            Process = Paras[2].VALUE.ToString();

            

            //獲取 MAC 地址
            MacAddress = Paras[3].VALUE.ToString();

            //add by LLF 2017-01-24 begin
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WoSession.Value.ToString();

            MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);

            if (TrCodeSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrTrCode = TrCodeSession.Value.ToString();

            //獲取連板數
            MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (LinkQtySession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            LinkQty = double.Parse(LinkQtySession.Value.ToString());

            MESStationSession SNsSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (SNsSession == null)
            {
                SNsSession = new MESStationSession() { SessionKey = Paras[7].SESSION_KEY, MESDataType = Paras[7].SESSION_TYPE , Value = new List<string>()};
                Station.StationSession.Add(SNsSession);
            }
            List<string> SNs = (List<string>)SNsSession.Value;
            if (SNs.Contains(StrSN))
            {
                throw new Exception($@"Don't Scan san SN({StrSN}) agant!");
            }
            else
            {
                SNs.Add(StrSN);
            }
            if (SNs.Count < LinkQty)
            {
                Station.NextInput = Input;
                Station.CurrActionRrturn = StationActionReturn.PassStopRunNext;
                return;
            }

            string Ext_QTY = "";
            //Message = table.AddSMTLoadingRecords(PanelSN.WORKORDERNO, StrPanel, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, Station.SFCDB);
            try
            {

                apdb = Station.APDB;
                if (Station.BU.Contains("FJZ"))
                {
                    MacAddress = Station.IP;
                    AP_DLL AP = new AP_DLL();
                    AP.LH_NSDI_GetAPSNCode(MacAddress, apdb, DB_TYPE_ENUM.Oracle);
                    for (int i = 0; i < SNs.Count; i++)
                    {
                        Message = table.AddSMTLoadingRecordsNSDIAP(StrWO, SNs[i], MacAddress, apdb, ref Ext_QTY);
                    }
                }
                else
                {
                    Message = table.AddSMTLoadingRecords(StrWO, StrSN, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, 1, Station.LoginUser.EMP_NO, MacAddress, StrTrCode, "N", apdb);
                }
                if (Message.StartsWith("OK"))
                {
                    Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
                    //Station.DBS["APDB"].Return(apdb);
                }
                else
                {
                    //Station.DBS["APDB"].Return(apdb);
                    throw new MESReturnMessage(Message);
                }
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    //Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }
        }

        /// <summary>
        /// 補 单板 SMTLOADING 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddSingleSMTLoadingRecordsAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string Message = string.Empty;
            //R_PANEL_SN PanelSN = null;
            string StrSN = "";
            string StrWO = "";
            string StrTrCode = "";
            Dictionary<string, DataRow> TrSnTable = null;
            string Process = string.Empty;
            //double LinkQty = 0d;
            string MacAddress = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 6)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(Message);
            }


            //獲取 R_PANEL_SN 對象
            MESStationSession SNlSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNlSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //PanelSN = (R_PANEL_SN)PanelSession.Value;
            StrSN = SNlSession.Value.ToString();
            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            //獲取面別
            Process = Paras[2].VALUE.ToString();

            //獲取連板數
            //MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            //if (LinkQtySession == null)
            //{
            //    Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
            //    throw new MESReturnMessage(Message);
            //}
            //LinkQty = double.Parse(LinkQtySession.Value.ToString());

            //獲取 MAC 地址
            MacAddress = Paras[3].VALUE.ToString();

            //add by LLF 2017-01-24 begin
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WoSession.Value.ToString();

            MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
           
                if (TrCodeSession == null)
                {
                    Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY });
                    throw new MESReturnMessage(Message);
                }
                StrTrCode = TrCodeSession.Value.ToString();

            string Ext_QTY = "";
            //Message = table.AddSMTLoadingRecords(PanelSN.WORKORDERNO, StrPanel, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, Station.SFCDB);
            try
            {

                apdb = Station.APDB;
                if (Station.BU.Contains("FJZ"))
                {
                    MacAddress = Station.IP;
                    AP_DLL AP = new AP_DLL();
                    AP.LH_NSDI_GetAPSNCode(MacAddress, apdb, DB_TYPE_ENUM.Oracle);
                    Message = table.AddSMTLoadingRecordsNSDIAP(StrWO, StrSN, MacAddress, apdb,ref Ext_QTY);
                }
                else
                {
                    Message = table.AddSMTLoadingRecords(StrWO, StrSN, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, 1, Station.LoginUser.EMP_NO, MacAddress, StrTrCode, "N", apdb);
                }
                if (Message.StartsWith("OK"))
                {
                    Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
                    //Station.DBS["APDB"].Return(apdb);
                }
                else
                {
                    //Station.DBS["APDB"].Return(apdb);
                    throw new MESReturnMessage(Message);
                }
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    //Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }
        }

        /// <summary>
        /// 補連板 SMTLOADING 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddMultiSNSMTLoadingRecordsAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string Message = string.Empty;
            List<string> StrSns = null;
            string StrWO = "";
            string StrTrCode = "";
            Dictionary<string, DataRow> TrSnTable = null;
            string Process = string.Empty;
            double LinkQty = 0d;
            string MacAddress = string.Empty;
            OleExec apdb = null;

            //獲取 多個 SN 對象
            MESStationSession SNsSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNsSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrSns = (List<string>)SNsSession.Value;
            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            //獲取面別
            Process = Paras[2].VALUE.ToString();

            //獲取連板數
            //MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            //if (LinkQtySession == null)
            //{
            //    Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
            //    throw new MESReturnMessage(Message);
            //}
            //LinkQty = double.Parse(LinkQtySession.Value.ToString());



            //獲取 MAC 地址
            MacAddress = Paras[4].VALUE.ToString();

            //獲取工單
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            if (WoSession.Value is WorkOrder)
            {
                StrWO = ((WorkOrder)WoSession.Value).WorkorderNo;
            }
            else
            {
                StrWO = WoSession.Value.ToString();
            }

            MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);

            if (TrCodeSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }

            StrTrCode = TrCodeSession.Value.ToString();
            try
            {
                apdb = Station.APDB;

                AP_DLL ap_dll = new AP_DLL();
                string linkQty = ap_dll.AP_GET_LINKQTY(StrWO, Station.APDB);
                string PanelCode=ap_dll.GetNextPanelCode(apdb);
                Message = table.AddSMTLoadingRecords(StrWO, StrSns, PanelCode, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, double.Parse(linkQty), Station.LoginUser.EMP_NO, MacAddress, StrTrCode, apdb);
                //Station.DBS["APDB"].Return(apdb);
                if (Message.Equals("OK"))
                {
                    Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
                }
                else
                {
                    throw new MESReturnMessage(Message);
                }
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    //Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }
        }



        /// <summary>
        /// Panel 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelPassStationAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string PanelSn = string.Empty;
            string Status = string.Empty;
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            string StationNext = "";
            SN SNObj = new SN();

            if (Paras.Count <= 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession PanelSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Modify By LLF 2018-01-26 SN_Session.Value是對象，取InputValue
            //PanelSn = PanelSnSession.Value.ToString();
            PanelSn = PanelSnSession.InputValue.ToString();

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StatusSession == null)
            {
                //Modify by LLF 2018-01-27 Value默認Pass
                //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                //throw new MESReturnMessage(ErrMessage);
                StatusSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input, Value = Paras[1].VALUE };
                Station.StationSession.Add(StatusSession);
            }
            Status = StatusSession.Value.ToString();

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else//Add by LLF 2018-01-27 DeviceName 默認為工站名稱
            {
                DeviceName = Station.StationName.ToString();
            }

            MESStationSession StationNextList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            StationNext = SNObj.StringListToString((List<string>)StationNextList.Value);

            table.PanelPassStation(PanelSn, Station.Line, Station.StationName, DeviceName, Station.BU, Status, Station.LoginUser.EMP_NO, Station.SFCDB);
            Station.AddMessage("MES00000064", new string[] { Station.StationName, StationNext }, StationMessageState.Pass);
        }

        /// <summary>
        /// FQC Lot 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LotPassStationAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            T_R_LOT_STATUS table = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Row_R_Lot_Status = null;
            string SerialNo = string.Empty;
            string LotNo = string.Empty;
            string Status = string.Empty;
            string[] FailInfos = new string[3];
            string DeviceName = string.Empty;

            if (Paras.Count <=0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SnSession.Value.ToString();
            //SerialNo = ((R_SN)SnSession.Value).SN;

            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LotNoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            LotNo = ((R_LOT_STATUS)LotNoSession.Value).LOT_NO.ToString();

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                //MODIFY by LLF 2018-02-24
                //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                //throw new MESReturnMessage(ErrMessage);
                Status = "PASS";
            }
            //Status = StatusSession.Value.ToString();

            MESStationSession FailInfoSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (FailInfoSession != null)
            {
                Status = "FAIL";
                FailInfos = (string[])FailInfoSession.Value;
            }
            //FailInfos[0] = "TEST";
            //FailInfos[1] = "TEST_CODE";
            //FailInfos[2] = "Just for test";

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else
            {
                DeviceName = Station.StationName;
            }

            MESStationSession SessionAQLTYPE = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            MESStationSession SessionLotQTY = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            MESStationSession SessionSAMPLEQTY = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            MESStationSession SessionREJECTQTY = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            MESStationSession SessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
            MESStationSession SessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
            if (SessionAQLTYPE == null)
            {
                SessionAQLTYPE = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionAQLTYPE);
            }

            if (SessionLotQTY == null)
            {
                SessionLotQTY = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotQTY);
            }

            if (SessionSAMPLEQTY == null)
            {
                SessionSAMPLEQTY = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSAMPLEQTY);
            }

            if (SessionREJECTQTY == null)
            {
                SessionREJECTQTY = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionREJECTQTY);
            }

            if (SessionPassQty == null)
            {
                SessionPassQty = new MESStationSession() { MESDataType = Paras[9].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[9].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionPassQty);
            }

            if (SessionFailQty == null)
            {
                SessionFailQty = new MESStationSession() { MESDataType = Paras[10].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[10].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionFailQty);
            }

            Station.SFCDB.ThrowSqlExeception = true;
            try
            {
                table.LotPassStation(SerialNo, LotNo, Status, Station.LoginUser.EMP_NO, Station.StationName, DeviceName, Station.Line, Station.BU, Station.SFCDB, FailInfos);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Row_R_Lot_Status = table.GetByLotNo(LotNo, Station.SFCDB);
            if (Row_R_Lot_Status != null)
            {
                SessionAQLTYPE.Value = Row_R_Lot_Status.AQL_TYPE;
                SessionLotQTY.Value = Row_R_Lot_Status.LOT_QTY;
                SessionSAMPLEQTY.Value = Row_R_Lot_Status.SAMPLE_QTY;
                SessionREJECTQTY.Value = Row_R_Lot_Status.REJECT_QTY;
                SessionPassQty.Value = Row_R_Lot_Status.PASS_QTY;
                SessionFailQty.Value = Row_R_Lot_Status.FAIL_QTY;
            }
            //table.LotPassStation(Input.Value.ToString(), "LOT_123456", "PASS", Station.LoginUser.EMP_NO, Station.StationName, Station.Line, Station.BU, Station.SFCDB, FailInfos);
            //table.LotPassStation("SN2222", "LOT_123456", "FAIL", Station.LoginUser.EMP_NO, Station.StationName, Station.Line, Station.BU, Station.SFCDB, FailInfos);
            Station.AddMessage("MES00000065", new string[] { LotNo, SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 分板過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SplitsPassStationAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string PanelSn = string.Empty;
            string SerialNo = string.Empty;
            string Status = string.Empty;
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            R_PANEL_SN PANELObj = null;

            if (Station.Line.Trim() == "")
            {
                throw new Exception("Line Cann't be null !");
            }
            if (Paras.Count != 5)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            PanelSn = PanelSession.InputValue.ToString();
            //PanelSn = ((R_PANEL_SN)PanelSession.Value).PANEL;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SNSession.Value.ToString();
            //SerialNo = ((R_SN)SNSession.Value).SN;

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[2].VALUE, SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value.ToString() == "")
                {
                    StatusSession.Value = "PASS";
                }
            }
            Status = StatusSession.Value.ToString();

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else
            {
                DeviceName = Station.StationName;
            }

            //add by LLF 2018-03-28
            MESStationSession PanelVitualSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (PanelVitualSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            PANELObj = (R_PANEL_SN)PanelVitualSNSession.Value;

            Station.SFCDB.ThrowSqlExeception = true;
            try
            {
                table.SplitsPassStation(PanelSn, Station.Line, Station.StationName, DeviceName, Station.BU, SerialNo, Status, Station.LoginUser.EMP_NO, Station.SFCDB, Station.APDB, PANELObj);
                //table.SplitsPassStation("P147852", Input.Value.ToString(), "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //检查是否BIP完成
            string strsql = $@"select count(1) from r_sn where sn in (
select sn from r_panel_sn where panel = '{PanelSn}' ) 
and id = sn";
            int C = int.Parse(Station.SFCDB.ExecSelectOneValue(strsql).ToString());
            if (C > 0)
            {
                Station.NextInput = Input;
                //Station.StationMessages.Add(new StationMessage { Message = $@"还有{C}PCS未分版", State = StationMessageState.Message });
                Station.StationMessages.Add(new StationMessage { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151624", new string[] { C.ToString() }), State = StationMessageState.Message });
            }
            else
            {
                Station.NextInput = Station.Inputs[0];
                //Input.Value = "";
                //add by LLF 2018-04-04
                //Station.StationMessages.Add(new StationMessage { Message = $@"请输入新Panel", State = StationMessageState.Message });
                Station.AddMessage("MES00000222", new string[] { PanelSn }, StationMessageState.Alert); //回饋消息到前台
            }


            //Modify by LLF 2018-04-04
            //Station.AddMessage("MES00000148", new string[] { PanelSn, SerialNo }, StationMessageState.Pass); //回饋消息到前台
           // Station.AddMessage("MES00000148", new string[] { PanelSn, SerialNo }, StationMessageState.Alert); //回饋消息到前台

        }

        public static void InLotPassAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            T_R_LOT_STATUS Table_Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Row_R_Lot_Status = null;
            string SerialNo = string.Empty;
            string LotNo = string.Empty;
            string LotID = string.Empty;
            string AQL_TYPE = string.Empty;
            string NewLotFlag = "0";
            SN SNObj = new SN();
            R_SN RSNObj = new R_SN();
            LotNo LotObj = new LotNo();

            if (Paras.Count <= 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SNObj = (SN)SnSession.Value;

            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LotNoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession NewLotFlagSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NewLotFlagSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            NewLotFlag = NewLotFlagSession.Value.ToString();

            MESStationSession AQLTYPESession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (AQLTYPESession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            AQL_TYPE = AQLTYPESession.Value.ToString();

            //
            MESStationSession SessionLotQTY = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            MESStationSession SessionSAMPLEQTY = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            MESStationSession SessionREJECTQTY = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            MESStationSession SessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            MESStationSession SessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
           
            if (SessionLotQTY == null)
            {
                SessionLotQTY = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotQTY);
            }

            if (SessionSAMPLEQTY == null)
            {
                SessionSAMPLEQTY = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSAMPLEQTY);
            }

            if (SessionREJECTQTY == null)
            {
                SessionREJECTQTY = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionREJECTQTY);
            }

            if (SessionPassQty == null)
            {
                SessionPassQty = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionPassQty);
            }

            if (SessionFailQty == null)
            {
                SessionFailQty = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionFailQty);
            }

            //

            if (NewLotFlag == "1")
            {
                LotNo = LotNoSession.Value.ToString();
            }
            else
            {
                LotObj = (LotNo)LotNoSession.Value;
                LotID = LotObj.ID;
                LotNo = LotObj.LOT_NO;
            }

            RSNObj = SNObj.LoadSN(SNObj.SerialNo, Station.SFCDB);
            Table_Lot_Status.InLotPassStation(NewLotFlag, RSNObj, LotNo, LotID, Station.StationName, Station.LoginUser.EMP_NO, AQL_TYPE, Station.Line, Station.BU, Station.SFCDB);

            Row_R_Lot_Status = Table_Lot_Status.GetByLotNo(LotNo, Station.SFCDB);
            if (Row_R_Lot_Status != null)
            {
                AQLTYPESession.Value = Row_R_Lot_Status.AQL_TYPE;
                SessionLotQTY.Value = Row_R_Lot_Status.LOT_QTY;
                SessionSAMPLEQTY.Value = Row_R_Lot_Status.SAMPLE_QTY;
                SessionREJECTQTY.Value = Row_R_Lot_Status.REJECT_QTY;
                SessionPassQty.Value = Row_R_Lot_Status.PASS_QTY;
                SessionFailQty.Value = Row_R_Lot_Status.FAIL_QTY;
            }

            Station.AddMessage("MES00000157", new string[] { SNObj.SerialNo, LotNo }, StationMessageState.Pass); //回饋消息到前台
        }

        public static void LotCloseAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string StrLotNo = "";
            LotNo LotObj = new LotNo();
            T_R_LOT_STATUS Table_Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Row_Lot_Status = null;

            if (Paras.Count <= 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "2", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LotNoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession NewLotFlagSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NewLotFlagSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (NewLotFlagSession.Value.ToString() == "0")
            {
                LotObj = (LotNo)LotNoSession.Value;
                StrLotNo = LotObj.LOT_NO;
            }
            else
            {
                StrLotNo = LotNoSession.Value.ToString();
            }

            Row_Lot_Status=Table_Lot_Status.GetByInput(StrLotNo, "LOT_NO", Station.SFCDB);

            if (Row_Lot_Status == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000161", new string[] { });
                throw new MESReturnMessage(ErrMessage);
            }

            if (Row_Lot_Status.CLOSED_FLAG == "1")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000092", new string[] { Row_Lot_Status.LOT_NO });
                throw new MESReturnMessage(ErrMessage);
            }

            Row_R_LOT_STATUS Rows = (Row_R_LOT_STATUS)Table_Lot_Status.GetObjByID(Row_Lot_Status.ID, Station.SFCDB);
            Rows.CLOSED_FLAG = "1";
            Rows.EDIT_TIME = Station.GetDBDateTime();
            Rows.EDIT_EMP = Station.LoginUser.EMP_NO;
            Station.SFCDB.ExecSQL(Rows.GetUpdateString(Station.DBType));

            Station.AddMessage("MES00000155", new string[] { Row_Lot_Status.LOT_NO }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 刪條碼
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void DeletePanel(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = string.Empty;
            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            int result = 0;
            string ErrMessage = string.Empty;
            T_R_WO_BASE Wo = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN Panel = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            List<R_PANEL_SN> Panels = new List<R_PANEL_SN>();
            
            PanelSN = Input.Value.ToString();
            Panels = Panel.GetPanel(PanelSN, Station.SFCDB);
            
            if (Panels.Count != 0)
            {
                RPanelSn.RecordPanelStationDetail(PanelSN, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB, Station.DBType);
                result = RPanelSn.SetPanelInValid(PanelSN, Station.SFCDB, Station.DBType);
                if (result == 0)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "Panel#Valid_Flag" });
                    throw new MESReturnMessage(ErrMessage);
                }

                Wo.AddCountToWo(Panels[0].WORKORDERNO, Convert.ToDouble("-" + result), Station.SFCDB);
                Station.AddMessage("MES00000207", new string[] { PanelSN}, StationMessageState.Pass);
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000038", new string[] { PanelSN });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        /// <summary>
        /// Panel 退 SMTLOADING
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReturnSMTLoadingAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = string.Empty;
            string WO = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            //Panel pa=(Panel)sessionPanel.Value;
            WO = sessionWO.Value.ToString();
            PanelSN = ((Panel)sessionPanel.Value).PanelNo;

            if (string.IsNullOrEmpty(PanelSN) || string.IsNullOrEmpty(WO))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            if (!RPanelSn.CheckPanelExist(PanelSN,Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            if (!RPanelSn.GetPanelCurrentStation(PanelSN,Station.SFCDB).Equals("SMTLOADING"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000261", new string[] { Paras[0].SESSION_TYPE }));
            }

            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> snList = t_r_sn.GetRSNbyPsn(PanelSN, Station.SFCDB);
            foreach(R_SN r_sn in snList)
            {
                Row_R_SN rowSN = (Row_R_SN)t_r_sn.GetObjByID(r_sn.ID, Station.SFCDB);
                rowSN.SN = "#" + rowSN.SN;
                rowSN.SKUNO = "#" + rowSN.SKUNO;
                rowSN.WORKORDERNO = "#" + rowSN.WORKORDERNO;
                rowSN.VALID_FLAG = "0";
                rowSN.EDIT_TIME = t_r_sn.GetDBDateTime(Station.SFCDB);
                rowSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                Station.SFCDB.ExecSQL(rowSN.GetUpdateString(Station.DBType));
            }

            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            List<R_SN_STATION_DETAIL> snDetailList = t_r_sn_station_detail.GetSNStationDetailByPanel(PanelSN, Station.SFCDB);
            foreach (R_SN_STATION_DETAIL r_detail_sn in snDetailList)
            {
                Row_R_SN_STATION_DETAIL rowDetailSN = (Row_R_SN_STATION_DETAIL)t_r_sn_station_detail.GetObjByID(r_detail_sn.ID, Station.SFCDB);
                rowDetailSN.R_SN_ID = "#" + rowDetailSN.R_SN_ID;
                rowDetailSN.SN = "#" + rowDetailSN.SN;
                rowDetailSN.SKUNO = "#" + rowDetailSN.SKUNO;
                rowDetailSN.WORKORDERNO = "#" + rowDetailSN.WORKORDERNO;
                Station.SFCDB.ExecSQL(rowDetailSN.GetUpdateString(Station.DBType));
            }

            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            t_r_wo_base.AddCountToWo(WO, - snList.Count, Station.SFCDB);

            T_R_PANEL_SN t_r_panel_sn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            List<R_PANEL_SN> panelSnList = t_r_panel_sn.GetPanel(PanelSN, Station.SFCDB);
            foreach(R_PANEL_SN r_panel_sn in panelSnList)
            {
                Row_R_PANEL_SN rowPanelSN = (Row_R_PANEL_SN)t_r_panel_sn.GetObjByID(r_panel_sn.ID, Station.SFCDB);
                rowPanelSN.SN = "#" + rowPanelSN.SN;
                rowPanelSN.PANEL = "#" + rowPanelSN.PANEL;
                rowPanelSN.WORKORDERNO = "#" + rowPanelSN.WORKORDERNO;
                Station.SFCDB.ExecSQL(rowPanelSN.GetUpdateString(Station.DBType));
            }

            AP_DLL APDLL = new AP_DLL();
            apdb = Station.APDB;
            APDLL.APUpdateUndoSmtloading(PanelSN, apdb);

        }

        /// <summary>
        /// Panel 入MRB
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelMrbPassAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {    
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession toStorageSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (toStorageSession == null || toStorageSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (toStorageSession.Value.ToString().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            Panel ObjPanel = (Panel)panelSession.Value;
            string toStorage = toStorageSession.Value.ToString();
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_MRB t_r_mrb = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_MRB_GT t_r_mrb_gt = new T_R_MRB_GT(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_SAP_STATION_MAP sapMap = new T_C_SAP_STATION_MAP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_MRB mrbObj = null;
            R_MRB_GT gtObj = null;
            int result;
            string sapStationCode = "";
            string confirmedFlag;
            bool isSame = false;
            foreach (R_PANEL_SN panel in ObjPanel.PanelCollection)
            {
                SN snObject = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(snObject.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                }
                //SN如果已經完工，Confirmed_Flag=1，否則Confirmed_Flag=0
                if (snObject.CompletedFlag != null && snObject.CompletedFlag == "1")
                {
                    confirmedFlag = "1";
                }
                else
                {
                    confirmedFlag = "0";
                }
                //寫入R_MRB
                mrbObj = new R_MRB();
                mrbObj.ID = t_r_mrb.GetNewID(Station.BU, Station.SFCDB);
                mrbObj.SN = snObject.SerialNo;
                mrbObj.WORKORDERNO = snObject.WorkorderNo;
                mrbObj.NEXT_STATION = snObject.NextStation;
                mrbObj.SKUNO = snObject.SkuNo;
                mrbObj.FROM_STORAGE = "";
                mrbObj.TO_STORAGE = toStorage;
                mrbObj.REWORK_WO = "";//空
                mrbObj.CREATE_EMP = Station.LoginUser.EMP_NO;
                mrbObj.CREATE_TIME = Station.GetDBDateTime();
                mrbObj.MRB_FLAG = "1";
                mrbObj.SAP_FLAG = "0";
                mrbObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                mrbObj.EDIT_TIME = Station.GetDBDateTime();
                result = t_r_mrb.Insert(mrbObj, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + snObject.SerialNo, "ADD" }));
                }
                //寫入R_MRB_GT
                gtObj = t_r_mrb_gt.GetByWOAndSAPFlageIsZero(snObject.WorkorderNo, Station.SFCDB);
                if (gtObj != null)
                {
                    gtObj.FROM_STORAGE = (gtObj.FROM_STORAGE == null || gtObj.FROM_STORAGE.Trim().Length <= 0) ? "" : gtObj.FROM_STORAGE;
                    gtObj.TO_STORAGE = (gtObj.TO_STORAGE == null || gtObj.TO_STORAGE.Trim().Length <= 0) ? "" : gtObj.TO_STORAGE;
                    gtObj.CONFIRMED_FLAG = (gtObj.CONFIRMED_FLAG == null || gtObj.CONFIRMED_FLAG.Trim().Length <= 0) ? "" : gtObj.CONFIRMED_FLAG;
                    if (gtObj.FROM_STORAGE == mrbObj.FROM_STORAGE && gtObj.TO_STORAGE == mrbObj.TO_STORAGE && gtObj.CONFIRMED_FLAG == confirmedFlag)
                    {
                        isSame = true;
                        //result = t_r_mrb_gt.updateTotalQTYAddOne(snObject.WorkorderNo, Station.LoginUser.EMP_NO,Station.SFCDB);
                        result = t_r_mrb_gt.updateTotalQTYAddOne(snObject.WorkorderNo, Station.LoginUser.EMP_NO, confirmedFlag, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB_GT:" + snObject.SerialNo, "UPDATE" }));
                        }
                    }
                }
                if (!isSame)
                {
                    gtObj = new R_MRB_GT();
                    gtObj.ID = t_r_mrb_gt.GetNewID(Station.BU, Station.SFCDB);
                    gtObj.WORKORDERNO = snObject.WorkorderNo;
                    sapStationCode = sapMap.GetMAXSAPStationCodeBySku(snObject.SkuNo, Station.SFCDB);
                    if (sapStationCode == "")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000224", new string[] { snObject.SkuNo }));
                    }
                    gtObj.SAP_STATION_CODE = sapStationCode;
                    gtObj.FROM_STORAGE = "";
                    gtObj.TO_STORAGE = toStorage;
                    gtObj.TOTAL_QTY = 1;
                    gtObj.CONFIRMED_FLAG = confirmedFlag;
                    gtObj.ZCPP_FLAG = "0";//單板入MRB
                    gtObj.SAP_FLAG = "0";//0待拋,1已拋
                    gtObj.SKUNO = snObject.SkuNo;
                    gtObj.SAP_MESSAGE = "";
                    gtObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                    gtObj.EDIT_TIME = Station.GetDBDateTime();
                    result = t_r_mrb_gt.Add(gtObj, Station.SFCDB);
                }
                //更改SN狀態
                result = t_r_sn.SN_Mrb_Pass_actionNotUpdateCompleted(snObject.ID, Station.LoginUser.EMP_NO, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + snObject.SerialNo, "UPDATE" }));
                }
                //寫入過站記錄
                var snobj = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.ID == snObject.ID).ToList().FirstOrDefault();
                result = Convert.ToInt32(t_r_sn.RecordPassStationDetail(snobj, Station.Line, Station.StationName, "", Station.BU, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + snObject.SerialNo, "ADD" }));
                }
                //更新工單數據
                result = t_r_wo_base.UpdateFINISHEDQTYAddOne(snObject.WorkorderNo, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + snObject.SerialNo, "UPDATE" }));
                }
                t_r_wo_base.UpdateWoCloseFlag(snObject.WorkorderNo, Station.SFCDB); 
            }
        }
        /// <summary>
        /// Panel入MRB寫入過站記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WritePanelMrbRecordsAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession toStorageSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (toStorageSession == null || toStorageSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (toStorageSession.Value.ToString().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            Panel ObjPanel = (Panel)panelSession.Value;
            string toStorage = toStorageSession.Value.ToString();
            int result = 0;
            T_R_SN_STATION_DETAIL TRSS = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_WO_BASE wo = t_r_wo_base.GetWoByWoNo(ObjPanel.Workorderno, Station.SFCDB);
            if (wo == null)
            {               
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { ObjPanel.Workorderno }));
            }
            R_SN_STATION_DETAIL detail = new R_SN_STATION_DETAIL();
            detail.ID = TRSS.GetNewID(Station.BU, Station.SFCDB);
            detail.R_SN_ID = ObjPanel.PanelNo;
            detail.SN = ObjPanel.PanelNo;
            detail.WORKORDERNO = ObjPanel.Workorderno;
            detail.SKUNO = wo.SKUNO;
            detail.PLANT = wo.PLANT;
            detail.STATION_NAME = Station.StationName;
            detail.NEXT_STATION = toStorage;
            detail.EDIT_EMP = Station.LoginUser.EMP_NO;
            detail.EDIT_TIME = TRSS.GetDBDateTime(Station.SFCDB);
            result = TRSS.SaveStationDetail(detail, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + ObjPanel.PanelNo, "Insert" }));
            }
        }

        public static void PanelReworkPassAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            //獲取到Panel 對象
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            //獲取工單對象
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession nextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (nextStationSession == null || nextStationSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            try
            {
                int result;
                Panel panelObj = (Panel)panelSession.Value;
                WorkOrder woObject = (WorkOrder)woSession.Value;
                string reworkStation = nextStationSession.Value.ToString().Trim().ToUpper();
                T_R_PANEL_SN t_r_panel_sn = new T_R_PANEL_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_MRB t_r_mrb = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //修改r_panel_sn中的工單
                //result = t_r_panel_sn.UpdateWOByPanel(panelObj.PanelCollection[0].WORKORDERNO, woObject.WorkorderNo, panelObj.PanelCollection[0].PANEL, Station.SFCDB);
                //if (result <= 0)
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_PANEL_SN:" + panelObj.PanelNo, "UPDATE" }));
                //}
                //更新工單投入數量
                result = Convert.ToInt32(t_r_wo_base.AddCountToWo(woObject.WorkorderNo, panelObj.PanelCollection.Count, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + woObject.WorkorderNo, "UPDATE" }));
                }
                foreach (R_PANEL_SN panel in panelObj.PanelCollection)
                {
                    SN snObject = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    if (string.IsNullOrEmpty(snObject.SerialNo))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                    }                    
                    //更新SN的Valid_Flag=0
                    result = t_r_sn.updateValid_Flag(snObject.ID, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + snObject.SerialNo, "UPDATE" }));
                    }
                    //R_SN新加一筆記錄新增一筆SN 與重工工單記錄
                    R_SN r_sn = new R_SN();
                    r_sn = t_r_sn.GetById(snObject.ID, Station.SFCDB);
                    r_sn.ID = t_r_sn.GetNewID(Station.BU, Station.SFCDB);
                    r_sn.SN = r_sn.ID;
                    r_sn.WORKORDERNO = woObject.WorkorderNo;
                    r_sn.CURRENT_STATION = "REWORK";
                    r_sn.NEXT_STATION = reworkStation;
                    r_sn.ROUTE_ID = woObject.RouteID;
                    r_sn.VALID_FLAG = "1";
                    r_sn.COMPLETED_FLAG = "0";
                    r_sn.PRODUCT_STATUS = "REWORK";
                    if (r_sn.REWORK_COUNT == null)
                    {
                        r_sn.REWORK_COUNT = 1;
                    }
                    else
                    {
                        r_sn.REWORK_COUNT = r_sn.REWORK_COUNT + 1;
                    }
                    r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                    r_sn.EDIT_TIME = Station.GetDBDateTime();
                    result = t_r_sn.AddNewSN(r_sn, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + r_sn.SN, "ADD" }));
                    }
                    //更新r_MRB
                    result = t_r_mrb.OutMrbUpdate(woObject.WorkorderNo, Station.LoginUser.EMP_NO, snObject.SerialNo, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + snObject.SerialNo, "UPDATE" }));
                    }
                    //過站記錄加一筆TR_SN_STATION_DETAIL  
                    result = Convert.ToInt32(t_r_sn.RecordPassStationDetail(r_sn, Station.Line, Station.StationName, "", Station.BU, Station.SFCDB));
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + r_sn.SN, "ADD" }));
                    }
                    //更新舊的虛擬條碼的過站記錄中的SN為最新的SN
                    result = Station.SFCDB.ORM.Updateable<R_SN_STATION_DETAIL>().UpdateColumns(
                        r => new R_SN_STATION_DETAIL()
                        {
                            SN = r_sn.SN
                        }).Where(r => r.SN == snObject.SerialNo).ExecuteCommand();
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + r_sn.SN, "UPDATE" }));
                    }
                    //更改KP
                    result = Station.SFCDB.ORM.Updateable<R_SN_KP>().UpdateColumns(
                        r => new R_SN_KP()
                        {
                            R_SN_ID = r_sn.ID,
                            SN = r_sn.SN
                        }).Where(r => r.R_SN_ID == snObject.ID).ExecuteCommand();

                    //更改PANEL中的SN和工單
                    result = Station.SFCDB.ORM.Updateable<R_PANEL_SN>().UpdateColumns(
                        r => new R_PANEL_SN()
                        {                            
                            SN = r_sn.SN,
                            WORKORDERNO=r_sn.WORKORDERNO
                        }).Where(r => r.SN == snObject.SerialNo).ExecuteCommand();
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_PANEL_SN:" + snObject.SerialNo, "UPDATE" }));
                    }

                }
                Station.AddMessage("MES00000063", new string[] { panelObj.PanelNo }, StationMessageState.Pass); //回饋消息到前台
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public static void PanelReworkAddSapStationRecord(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionReworkStation = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionReworkStation == null || sessionReworkStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            WorkOrder woObject = (WorkOrder)sessionWO.Value;            
            Panel panelObj = (Panel)panelSession.Value;
            string reworkStation = sessionReworkStation.Value.ToString();
            string controlName = Paras[3].VALUE.Trim();
            string controlCategory = Paras[4].VALUE.Trim();
            if (controlName == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            if (controlCategory == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            List<R_F_CONTROL> listControl = t_r_function_control.GetListByFcv(controlName, controlCategory, Station.SFCDB);
            bool bBackflush = listControl.Where(r => woObject.SkuNO.StartsWith(r.VALUE)).Any();
            if (bBackflush)
            {
                T_C_SAP_STATION_MAP t_c_sap_station_map = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
                List<C_SAP_STATION_MAP> listMap = t_c_sap_station_map.GetSAPStationMapBySkuOrderBySAPCodeASC(woObject.SkuNO, Station.SFCDB);
                if (listMap.Count == 0)
                {
                    throw new MESReturnMessage(woObject.SkuNO + " Not Setting SAP BackFlush Station!");
                }
                string backflushStation = listMap.FirstOrDefault().STATION_NAME;
                if (backflushStation != reworkStation)
                {

                    T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                    SN snObject = new SN(panelObj.PanelCollection.FirstOrDefault().SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    List<C_ROUTE_DETAIL> listRoute = t_c_route_detail.GetByRouteIdOrderBySEQASC(snObject.RouteID, Station.SFCDB);
                    if (listRoute.Count == 0)
                    {
                        throw new MESReturnMessage("Get SN Route Detail Error!");
                    }
                    C_ROUTE_DETAIL backflushStationDetail = listRoute.Find(r => r.STATION_NAME == backflushStation);
                    if (backflushStationDetail == null)
                    {
                        throw new MESReturnMessage("Backflush Station Not In SN Route");
                    }
                    C_ROUTE_DETAIL reworkStationDetail = listRoute.Find(r => r.STATION_NAME == reworkStation);
                    if (reworkStationDetail == null)
                    {
                        throw new MESReturnMessage("Rework Station Not In SN Route");
                    }
                    if (backflushStationDetail.SEQ_NO < reworkStationDetail.SEQ_NO)
                    {
                        T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);                       
                        foreach (var panel in panelObj.PanelCollection)
                        {
                            SN snOb = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            t_r_sn.RecordPassStationDetail(snOb.SerialNo, Station.Line, backflushStation, backflushStation, Station.BU, Station.SFCDB);
                        }                        
                    }
                }
            }

        }
        private static MESStationSession GetStationSession(MESStationBase Station, string SessionType,string SessionKey)
        {
            MESStationSession session = Station.StationSession.Find(t => t.MESDataType == SessionType && t.SessionKey == SessionKey);
            if(session==null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { SessionType + SessionKey }));

            }
            return session;
        }

        /// <summary>
        /// 將Panel和SN Link在一起
        /// 三個參數：
        /// 1.panel sn 對應的 R_SN 表的對象
        /// 2.LINK_QTY 連板數
        /// 3.SNS 輸入的 SN List 集合
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelLinkSNsAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var PanelSession = GetStationSession(Station, Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY);
            var Panel = (SN)PanelSession.Value;
            var LinkQtySession = GetStationSession(Station, Paras[1].SESSION_TYPE, Paras[1].SESSION_KEY);
            var LinkQty = (int)LinkQtySession.Value;
            var SnsSession = GetStationSession(Station, Paras[2].SESSION_TYPE, Paras[2].SESSION_KEY);
            var SnList = (List<string>)SnsSession.Value;
            //if(SnList.Count!=LinkQty)
            //{
            //    throw new MESReturnMessage("The count of input sn is not equal to the link qty");
            //}

            //記錄到 R_PANEL_SN 中
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            List<R_PANEL_SN> PanelSns = new List<R_PANEL_SN>();
            List<R_SN> Sns = new List<R_SN>();
            int i = 1;
            SnList.ForEach(sn =>
            {
                R_PANEL_SN PanelSn = null;
                if ((PanelSn=TRPS.GetPanelBySn(sn, Station.SFCDB)) == null)
                {
                    PanelSns.Add(new R_PANEL_SN()
                    {
                        PANEL = Panel.SerialNo,
                        SN = sn,
                        SEQ_NO = i,
                        WORKORDERNO = Panel.WorkorderNo,
                        EDIT_EMP = Station.LoginUser.EMP_NO
                    });
                    i++;

                    Sns.Add(new R_SN()
                    {
                        SN = sn,
                        SKUNO = Panel.SkuNo,
                        WORKORDERNO = Panel.WorkorderNo,
                        PLANT = Panel.Plant,
                        ROUTE_ID = Panel.RouteID,
                        STARTED_FLAG = "1",
                        PACKED_FLAG = "0",
                        COMPLETED_FLAG = "0",
                        SHIPPED_FLAG = "0",
                        REPAIR_FAILED_FLAG = "0",
                        CURRENT_STATION = Panel.NextStation,
                        CUST_PN = Panel.CustomerPartNo,
                        SCRAPED_FLAG = "0",
                        PRODUCT_STATUS = "FRESH",
                        REWORK_COUNT = 0,
                        VALID_FLAG = "1",
                        EDIT_EMP = Station.LoginUser.EMP_NO,

                    });
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200325140815",new string[] { sn,PanelSn.PANEL }));
                }
            });
            TRPS.AddSnToPanel(PanelSns, Station.BU, Station.SFCDB);
            //根據 Panel 對象創建多個SN對象，插入到數據庫中
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            TRS.AddToRSn(Sns, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
            //記錄到 mes4.r_sn_link 中
            AP_DLL ApDLL = new AP_DLL();
            var result = ApDLL.InsertRSNLink(Panel.SerialNo, Sns.Select(t=>t.SN).ToList(), Panel.WorkorderNo, Station.LoginUser.EMP_NO,Station.APDB);


            //同時更新 Panel 對象的Valid_Flag為0
            if (Sns.Count > 0)
            {
                TRS.UpdateSNValid(Panel.SerialNo, Station.SFCDB, Station.DBType);
                //更新工單的投入數量，減去 panel 的投入1再加上子板的數量 LinkQty
                T_R_WO_BASE TRWB = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
                TRWB.UpdateWOINPUTQTY(Panel.WorkorderNo, Station.SFCDB, Sns.Count - 1);
            }

            

            SnsSession.Value = new List<string>();
        }

    }
}
