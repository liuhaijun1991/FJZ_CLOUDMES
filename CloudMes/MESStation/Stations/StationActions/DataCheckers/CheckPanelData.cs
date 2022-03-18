using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckPanelData
    {

        public static void PanelStateDatachecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string P_sn = Input.Value.ToString();
            string ErrMessage = string.Empty;
            int RepairedCount = 0;
            int CompletedCount = 0;
            int ShippedCount = 0;
            int TotalCount = 0;

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (PanelSession == null)
            //{
            //    PanelSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(PanelSession);
            //}
            //else
            //{
            //    if (PanelSession.Value != null && PanelSession.Value.ToString().Length > 0)
            //    {
            //        P_sn = PanelSession.Value.ToString();
            //    }
            //    else
            //    {
            //        P_sn = Input.Value.ToString();
            //    }
            //}

            Panel p = new Panel();
            //lock check
            var locks = Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN == P_sn && t.TYPE == "PANEL" && t.LOCK_STATUS == "1").ToList();

            if (locks.Count > 0)
            {
                throw new Exception($@"PANEL:{P_sn} Locked ! {locks[0].LOCK_REASON}");
            }


            List<R_SN> r_sn = p.GetPanel(P_sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            //changed into follow one by zgj 2018-03-14
            if (r_sn != null && r_sn.Count > 0)
            {
                TotalCount = r_sn.Count;
                foreach (R_SN sn in r_sn)
                {
                    if (sn.REPAIR_FAILED_FLAG.Equals("1"))
                    {
                        RepairedCount++;
                    }
                    if (sn.COMPLETED_FLAG.Equals("1") || sn.PACKED_FLAG.Equals("1"))
                    {
                        CompletedCount++;
                    }
                    if (sn.SHIPPED_FLAG.Equals("1"))
                    {
                        ShippedCount++;
                    }
                }
                if (RepairedCount.Equals(TotalCount))
                {
                    Station.AddMessage("MES00000068", new string[] { P_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000068",
                                    new string[] { P_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (CompletedCount.Equals(TotalCount))
                {
                    Station.AddMessage("MES00000069", new string[] { P_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000069",
                                    new string[] { P_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (ShippedCount.Equals(TotalCount))
                {
                    Station.AddMessage("MES00000070", new string[] { P_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000070",
                                    new string[] { P_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
                Station.AddMessage("MES00000067", new string[] { "PanleSN" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                Station.AddMessage("MES00000007", new string[] { "PanleSN" }, StationMessageState.Fail);
            }
            //if (r_sn != null)
            //{
            //    if (r_sn[0].REPAIR_FAILED_FLAG == "1")
            //    {
            //        Station.AddMessage("MES00000068", new string[] { P_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000068",
            //                        new string[] { P_sn });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //    if (r_sn[0].COMPLETED_FLAG == "1" || r_sn[0].PACKED_FLAG == "1")
            //    {
            //        Station.AddMessage("MES00000069", new string[] { P_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000069",
            //                        new string[] { P_sn });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //    if (r_sn[0].SHIPPED_FLAG == "1")
            //    {
            //        Station.AddMessage("MES00000070", new string[] { P_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000070",
            //                        new string[] { P_sn });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //    Station.AddMessage("MES00000067", new string[] { "PanleSN" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            //}


        }

        /// <summary>
        /// 檢查當前站是否在下一站的清單中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// add by LLF  2018-01-27 
        public static void NextStationDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<string> ListNextStation = new List<string>();
            SN SNObj = new SN();
            string StrStation = "";

            MESStationSession NextStation_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (NextStation_Session == null)
            {
                //Station.AddMessage("MES00000135", new string[] { "ListNextStation--Function:NextStationDataChecker" }, StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000135", new string[] { "ListNextStation--Function:NextStationDataChecker" }));
            }

            ListNextStation = (List<string>)NextStation_Session.Value;
            StrStation = SNObj.StringListToString(ListNextStation);
            if (!ListNextStation.Contains(Station.StationName))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000136", new string[] { StrStation }));
            }
        }

        public static void PanelRuleDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //input test
            //string inputValue = Input.Value.ToString();
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PanelSession == null)
            {
                //throw new MESReturnMessage("Panel加載異常");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152959"));

            }
            Station.AddMessage("OK", new string[] { "ok" }, StationMessageState.Pass);
        }

        public static void PanelVirtualSNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = "";
            bool CheckFlag = false;
            SN SNObj = new SN();
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            PanelSN = PanelSession.InputValue.ToString();
            CheckFlag=SNObj.CheckPanelVirtualSNExist(PanelSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (!CheckFlag)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000139", new string[] { PanelSN }));
            }
        }

        /// <summary>
        /// 檢查該 Panel 是不是在指定站位
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPanelInSmtLoading(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSn = string.Empty;
            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            string ErrMessage = string.Empty;
            string UniqueStation = string.Empty;

            if (Paras.Count < 1)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMessage);
            }

            PanelSn = Input.Value.ToString();
            UniqueStation = Paras[0].VALUE;
            if(UniqueStation.Length==0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000050");
                throw new MESReturnMessage(ErrMessage);
            }

            if (!RPanelSn.GetPanelUniqueStation(PanelSn, Station.SFCDB).ToUpper().Equals(UniqueStation))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000206", new string[] { PanelSn, UniqueStation });
                throw new MESReturnMessage(ErrMessage);
            }

        }

        /// <summary>
        /// 檢查panel對象中的SN的NextStation是否等於當前工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanleNextStationChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPanle = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanle == null || sessionPanle.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel panel = new Panel();

            Panel panleObject = (Panel)sessionPanle.Value;
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            //List<R_SN> snList = panel.GetSnDetail(panleObject.PanelNo, Station.SFCDB, Station.DBType)操妈蛋
            List<R_SN> snList = panel.GetSnDetail(panleObject.PanelNo, Station.SFCDB, Station.DBType).FindAll(t => t.ID == t.SN);
            foreach (R_SN sn in snList)
            {
                if (!t_c_route_detail.StationInRoute(sn.ROUTE_ID, Station.StationName, Station.SFCDB))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180621172210", new string[] { Station.StationName, sn.SN }));
                }
                if (!sn.NEXT_STATION.Equals(Station.StationName))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000136", new string[] { sn.NEXT_STATION ,Station.StationName}));
                }
            }
            
        }

        /// <summary>
        /// Panel SN status flag checker
        /// 檢查指定Panel SN Flag 
        /// 傳入一個 Panel 對象 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanleSNStatusChecker(MESStationBase Station,MESStationInput Input,List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            string statusFlag = string.Empty;
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel ObjPanel = (Panel)panelSession.Value;            
            foreach (R_PANEL_SN panel in ObjPanel.PanelCollection)
            {
                SN sn = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(sn.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                }
                for (int i = 1; i < Paras.Count; i++)
                {
                    statusFlag = Paras[i].VALUE.Trim().ToUpper();
                    switch (statusFlag)
                    {
                        case "PACKED_FLAG":
                            //已經包裝
                            if (sn.PackedFlag != null && sn.PackedFlag.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005155848", new string[] { sn.SerialNo }));
                            }
                            break;
                        case "COMPLETED_FLAG":
                            //已經完工
                            if (sn.CompletedFlag != null && sn.CompletedFlag.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160344", new string[] { sn.SerialNo }));
                            }
                            break;
                        case "SHIPPED_FLAG":
                            //已經出貨
                            if (sn.ShippedFlag != null && sn.ShippedFlag.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { sn.SerialNo }));
                            }
                            break;
                        case "REPAIR_FAILED_FLAG":
                            //待維修
                            if (sn.RepairFailedFlag != null && sn.RepairFailedFlag.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160551", new string[] { sn.SerialNo }));
                            }
                            break;
                        case "SCRAPED_FLAG":
                            //已報廢
                            if (sn.ScrapedFlag != null && sn.ScrapedFlag.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160653", new string[] { sn.SerialNo }));
                            }
                            break;
                        case "VALID_FLAG":
                            //無效的
                            if (sn.ValidFlag != null && sn.ValidFlag.Equals("0"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154143", new string[] { sn.SerialNo }));
                            }
                            break;
                        case "STOCK_STATUS":
                            //已入庫
                            if (sn.StockStatus != null && sn.StockStatus.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005161120", new string[] { sn.SerialNo }));
                            }
                            break;
                        default:
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259"));
                    }
                }
            }
        }

        /// <summary>
        /// check next station is equal the paras value
        /// 檢查Panel SN的Nextstation是否等於傳入的值,等於就報錯
        /// 傳入一個 Panel 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelSNNextStationValueChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel ObjPanel = (Panel)panelSession.Value;
            foreach (R_PANEL_SN panel in ObjPanel.PanelCollection)
            {
                SN snObject = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(snObject.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                }
                for (int i = 1; i < Paras.Count; i++)
                {
                    if (snObject.NextStation.ToUpper() == Paras[i].VALUE.Trim().ToUpper())
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154006", new string[] { snObject.SerialNo, snObject.NextStation }));
                    }
                }
            }
        }
        /// <summary>
        /// check next station is equal the paras value
        /// 檢查Panel SN的Nextstation是否不等於傳入的值,不等於就報錯
        /// 傳入一個 Panel 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelSNNextStationNotEqualChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel ObjPanel = (Panel)panelSession.Value;
            foreach (R_PANEL_SN panel in ObjPanel.PanelCollection)
            {
                SN snObject = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(snObject.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                }
                for (int i = 1; i < Paras.Count; i++)
                {
                    if (snObject.NextStation.ToUpper() != Paras[i].VALUE.Trim().ToUpper())
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154006", new string[] { snObject.SerialNo, snObject.NextStation }));
                    }
                }
            }
        }
        public static void PanelPassStationCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSn = string.Empty;
            string CurrentStation = string.Empty;
            string ErrMessage = string.Empty;
            string StationNext = string.Empty;
            bool Result = false;
            SN SNObj = new SN();
            T_R_PANEL_SN R_PANEL_SN = new T_R_PANEL_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (Paras.Count <= 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
           
            MESStationSession PanelSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE  });
                throw new MESReturnMessage(ErrMessage);
            }
            PanelSn = PanelSnSession.InputValue.ToString();
            MESStationSession StationNextList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StationNextList == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE });
                throw new MESReturnMessage(ErrMessage);
            }
            StationNext = SNObj.StringListToString((List<string>)StationNextList.Value);
            Result =  R_PANEL_SN.CheckPanelBySNStatus(Station.SFCDB, PanelSn, StationNext);
            if (Result) {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20191202175100", new string[] { Paras[0].SESSION_TYPE});
                throw new MESReturnMessage(ErrMessage);
            }

        }
        /// <summary>
        ///  檢查Panel SN是否掃入Stock
        ///  傳入一個 Panel 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelSNInJobstocChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {           
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel ObjPanel = (Panel)panelSession.Value;
            T_R_STOCK t_r_stock = new T_R_STOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            bool isStock;
            foreach (R_PANEL_SN panel in ObjPanel.PanelCollection)
            {
                SN snObject = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(snObject.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                }
                isStock = t_r_stock.IsStockIn(snObject.SerialNo, snObject.WorkorderNo, Station.SFCDB);
                if (snObject.StockStatus != null && snObject.StockStatus.Equals("1") && isStock)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005161120", new string[] { snObject.SerialNo }));
                }
            }
        }
        /// <summary>
        /// Panel重工檢查新舊工單的版本是否一致
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputPanelWorkorderVerionChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            Panel objPanel = (Panel)panelSession.Value;
            WorkOrder newWOObject = (WorkOrder)woSession.Value;
            WorkOrder oldWOObject = new WorkOrder();
            oldWOObject.Init(objPanel.Workorderno, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (oldWOObject == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { objPanel.Workorderno }));
            }
            if (string.IsNullOrEmpty(newWOObject.SKU_VER))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102153958", new string[] { newWOObject.WorkorderNo }));
            }
            if (string.IsNullOrEmpty(oldWOObject.SKU_VER))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102153958", new string[] { oldWOObject.WorkorderNo }));
            }
            if (newWOObject.SKU_VER != oldWOObject.SKU_VER)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102153958", new string[] { oldWOObject.WorkorderNo, newWOObject.WorkorderNo }));
            }
        }
        /// <summary>
        /// Panel重工打回工站檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelReworkStationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {            
            List<R_MRB> GetMRBList = new List<R_MRB>();
            R_MRB New_R_MRB = new R_MRB();
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);

            if (Paras.Count != 3)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession stationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (stationSession == null|| stationSession.Value==null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            try
            {
                WorkOrder woObject = (WorkOrder)woSession.Value;
                Panel objPanel = (Panel)panelSession.Value;
                string nextStation = stationSession.Value.ToString();
                foreach (R_PANEL_SN panel in objPanel.PanelCollection)
                {
                    SN snObject = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    if (string.IsNullOrEmpty(snObject.SerialNo))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                    }
                    GetMRBList = TR_MRB.GetMrbInformationBySN(snObject.SerialNo, Station.SFCDB);

                    if (GetMRBList == null || GetMRBList.Count == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "R_MRB:" + snObject.SerialNo }));
                    }
                    Route routeDetail = new Route(woObject.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                    RouteDetail h = routeDetailList.Find(t => t.STATION_NAME == GetMRBList[0].NEXT_STATION || t.STATION_TYPE == GetMRBList[0].NEXT_STATION);
                    if (h == null)   //R_MRB next_station欄位的值
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000205", new string[] { snObject.SerialNo, woObject.WorkorderNo }));
                    }

                    RouteDetail g = routeDetailList.Find(t => t.STATION_NAME == nextStation);
                    if (g == null)  //REWORK選擇的要打回工站
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { nextStation }));
                    }

                    if (g.SEQ_NO > h.SEQ_NO)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000211", new string[] { }));
                    }
                }
                Station.AddMessage("MES00000026", new string[] { },StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 根據SN對象檢查該SN所處的PANEL是否分板OK
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPanelIsBIPOKBySNObj(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN objSN = (SN)sessionSN.Value;
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            if (TRPS.CheckPanelIsBIPOKBySN(Station.SFCDB, objSN.SerialNo))
            {
                //throw new Exception(objSN.SerialNo+" 對應的PANEL沒有分板完,請回BIP工站進行分板");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155253", new string[] { objSN.SerialNo }));

            }
        }

        public static void PanelNextStationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            string stationName = sessionStation.Value.ToString();
            Panel ObjPanel = (Panel)panelSession.Value;
            foreach (R_PANEL_SN panel in ObjPanel.PanelCollection)
            {
                SN snObject = new SN(panel.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(snObject.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + panel.SN }));
                }
                if (snObject.NextStation.ToUpper() != stationName.ToUpper())
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154006", new string[] { snObject.SerialNo, snObject.NextStation }));
                }
            }
        }
    
        /// <summary>
        /// 檢查是否有進行過分板
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelSplitCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {           
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel ObjPanel = (Panel)panelSession.Value;
            bool bSplit = Station.SFCDB.ORM.Queryable<R_SN, R_PANEL_SN>((s, p) => s.SN == p.SN).Any((s, p) => p.PANEL == ObjPanel.PanelNo && s.ID != s.SN);
            if(bSplit)
            {
                throw new MESReturnMessage($@"{ObjPanel.PanelNo} already split.");
            }
        }
    }
}
