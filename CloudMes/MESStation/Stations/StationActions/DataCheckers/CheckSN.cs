using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject.Module.Vertiv;
using MESStation.LogicObject;
using MESStation.HateEmsGetDataService;
using System.Text.RegularExpressions;
using MESDBHelper;
using MESPubLab.MESStation.SNMaker;
using MESDataObject.Common;
using MESDataObject.Module.DCN;
using MESDataObject.Module.OM;
using MESDataObject.Module.Juniper;
using static MESDataObject.Common.EnumExtensions;
using MESJuniper.OrderManagement;
using MESStation.Management;
using MESPubLab.MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckSN
    {
        #region 参考这样的写法，可以免去很多无用功 add by LJD 2019-5-1 10:41:41
        /// <summary>
        /// 傳入一個 SN 對象,檢查 SN 是否已經包裝
        /// 通过性可控，既未包装时可通过或包装后可通过
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnPackStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            SN sn = (SN)SNSession.Value;
            msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102500", new string[] { sn.SerialNo, "PACKED" });
            if (Paras[0].VALUE.ToUpper() == "TRUE")
            {
                if (sn.PackedFlag.Equals("1"))
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                    {
                        Message = msg,
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message
                    });
                }
                else
                {
                    //产品{0}未{1}！
                    msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102559", new string[] { sn.SerialNo, "PACKED" });
                    throw new MESReturnMessage(msg);
                }
            }
            else
            {
                if (sn.PackedFlag.Equals("1"))
                {
                    //已經包裝
                    throw new MESReturnMessage(msg);
                }
            }
        }

        /// <summary>
        /// 傳入一個 SN 對象,檢查 SN 是否已經完工
        /// 通过性可控，既未完工时可通过或完工后可通过
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnCompleteStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            SN sn = (SN)SNSession.Value;
            //产品{0}已经{1}!  Unit {0} has been {1}!
            msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102500", new string[] { sn.SerialNo, "Completed" });
            if (Paras[0].VALUE.ToUpper() == "TRUE")
            {
                if (sn.CompletedFlag.Equals("1"))
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                    {
                        Message = msg,
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message
                    });
                }
                else
                {
                    //产品{0}未{1}！
                    msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102559", new string[] { sn.SerialNo, "Completed" });
                    throw new MESReturnMessage(msg);
                }
            }
            else
            {
                if (sn.CompletedFlag.Equals("1"))
                {
                    throw new MESReturnMessage(msg);
                }
            }
        }

        /// <summary>
        /// 傳入一個 SN 對象,檢查 SN 是否已經出货
        /// 通过性可控，既未出货时可通过或出货后可通过
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnShipStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            SN sn = (SN)SNSession.Value;
            //产品{0}已经{1}!  Unit {0} has been {1}!
            msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102500", new string[] { sn.SerialNo, "Shiped" });
            if (Paras[0].VALUE.ToUpper() == "TRUE")
            {
                if (sn.ShippedFlag.Equals("1"))
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                    {
                        Message = msg,
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message
                    });
                }
                else
                {
                    //产品{0}未{1}！
                    msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102559", new string[] { sn.SerialNo, "Shiped" });
                    throw new MESReturnMessage(msg);
                }
            }
            else
            {
                if (sn.ShippedFlag.Equals("1"))
                {
                    throw new MESReturnMessage(msg);
                }
            }
        }

        /// <summary>
        /// 傳入一個 SN 對象,檢查 SN 是否已經包裝
        /// 通过性可控，既未包装时可通过或包装后可通过
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnRepairStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            SN sn = (SN)SNSession.Value;
            //产品{0}已经{1}!  Unit {0} has been {1}!
            msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102500", new string[] { sn.SerialNo, "Repaired" });
            if (Paras[0].VALUE.ToUpper() == "TRUE")
            {
                if (sn.RepairFailedFlag.Equals("1"))
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                    {
                        Message = msg,
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message
                    });
                }
                else
                {
                    //产品{0}未{1}！
                    msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102559", new string[] { sn.SerialNo, "Repaired" });
                    throw new MESReturnMessage(msg);
                }
            }
            else
            {
                if (sn.RepairFailedFlag.Equals("1"))
                {
                    throw new MESReturnMessage(msg);
                }
            }
        }

        /// <summary>
        /// 傳入一個 SN 對象,檢查 SN 是否已經报废
        /// 通过性可控，既未报废时可通过或报废后可通过
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnScrapedStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            SN sn = (SN)SNSession.Value;
            //产品{0}已经{1}!  Unit {0} has been {1}!
            msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102500", new string[] { sn.SerialNo, "Scraped" });
            if (Paras[0].VALUE.ToUpper() == "TRUE")
            {
                if (sn.ScrapedFlag.Equals("1"))
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                    {
                        Message = msg,
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message
                    });
                }
                else
                {
                    //产品{0}未{1}！
                    msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190501102559", new string[] { sn.SerialNo, "Scraped" });
                    throw new MESReturnMessage(msg);
                }
            }
            else
            {
                if (sn.ScrapedFlag.Equals("1"))
                {
                    throw new MESReturnMessage(msg);
                }
            }
        }
        #endregion

        /// <summary>
        /// Check SN Has I054 ACK From Juniper
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">R_SN Session Param</param>
        public static void CheckSnI054ACK(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            SN sn = (SN)SNSession.Value;
            JuniperOmBase.JuniperI054AckCheck(sn.SerialNo, Station.SFCDB.ORM);
        }

        /// <summary>
        /// Check SN Has I244 From Juniper
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">R_SN Session Param</param>
        public static void CheckSnI244(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            SN sn = (SN)SNSession.Value;
            JuniperOmBase.JuniperI244Check(sn.SerialNo, Station.SFCDB.ORM);
        }


        /// <summary>
        /// Check Pallet'SN Has I244 From Juniper
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">R_SN Session Param</param>
        public static void CheckSnInPalletI244(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession palletSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (palletSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "Pallet" });
                throw new MESReturnMessage(msg);
            }
            var packno = palletSession.Value.ToString();
            var snlist = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((PL, CT, SP, SN) => PL.ID == CT.PARENT_PACK_ID && CT.ID == SP.PACK_ID && SP.SN_ID == SN.ID)
                .Where((PL, CT, SP, SN) => PL.PACK_NO == packno)
                .Select((PL, CT, SP, SN) => SN.SN)
                .ToList();
            for (int i = 0; i < snlist.Count; i++)
            {
                JuniperOmBase.JuniperI244Check(snlist[i], Station.SFCDB.ORM);
            }
        }

        /// <summary>
        /// Check ParentSN has Pass Record 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">R_SN Session Param</param>
        public static void CheckParentSnTestRecord(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "WO" });
                throw new MESReturnMessage(msg);
            }
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SnSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }

            var StrSN = SnSession.Value.ToString();
            var workorder = (WorkOrder)WoSession.Value;
            var GetRoute = Station.SFCDB.ORM.Queryable<C_ROUTE>().Where(r => r.ID == workorder.RouteID).First();
            T_R_FUNCTION_CONTROL t_R_FUNCTION = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            var rfc = t_R_FUNCTION.GetListByFcv("CHECK_PARENTSN_TEST_RECORD", "ROUTE", GetRoute.ROUTE_NAME, Station.SFCDB);
            for (int i = 0; i < rfc.Count(); i++)
            {
                T_R_TEST_RECORD t_R_TEST = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
                var rt = t_R_TEST.GetLastTestRecord(StrSN, rfc[i].EXTVAL, Station.SFCDB);
                if (rt == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190604110249"));//NO TEST RECORD
                }
                else if (rt.STATE != "PASS")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { StrSN, rfc[i].EXTVAL }));//{0} can't pass in the last time on {1}!
                }
            }
        }

        /// <summary>
        /// check PanelSn ,長度是否等於8，並且以PN開頭，是否已經投入使用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelSNInputRuleChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string PanelSNNO = Input.Value.ToString();
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            PanelSNNO = Input.Value.ToString();
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                s.Value = PanelSNNO;
                s.InputValue = PanelSNNO;
                s.ResetInput = Input;
                Station.StationSession.Add(s);
            }
            else
            {
                s.InputValue = PanelSNNO;
                s.ResetInput = Input;
                s.Value = PanelSNNO;
            }
            if (PanelSNNO.Length == 8 && ((Station.BU == "HWD" && PanelSNNO.Substring(0, 2) == "PN") || (Station.BU == "VERTIV" && PanelSNNO.Substring(0, 2) == "PV")))
            {
                T_R_PANEL_SN TR_PanelSN = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                ////判斷是否投入使用                
                if (TR_PanelSN.CheckPanelExist(PanelSNNO, Station.SFCDB))
                {
                    //Station.AddMessage("MES00000040", new string[] { "PanelSN:" + PanelSNNO }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { "PanelSN:" + PanelSNNO }));
                }
                else
                {
                    Station.AddMessage("MES00000029", new string[] { "PanelSN", PanelSNNO }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
            }
            else
            {
                //Station.AddMessage("MES00000022", new string[] { "PanelSN:" + PanelSNNO }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000022", new string[] { "PanelSN:" + PanelSNNO }));
            }
        }

        /// <summary>
        /// Check Time Pass Pre-Assy > 1H
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNCheckPassTimePreAssy(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Input.Value.ToString().Length == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_C_SKU_DETAIL LockPress = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            if (LockPress.LockPreAsyy(Input.Value.ToString(), Station.SFCDB))
            {
                R_SN sn = TRS.GetSN(Input.Value.ToString(), Station.SFCDB);
                if (sn.NEXT_STATION == Station.StationName && (sn.CURRENT_STATION == "PRE-ASSY"|| sn.CURRENT_STATION == "VI") && Station.GetDBDateTime().AddHours(-1) < sn.EDIT_TIME)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210925181247"));
                }
            }
        }

        /// <summary>
        /// SN狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNInputStatusChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder Wo;
            //int Linkqty;
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }

            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                SNLoadPoint = new MESStationSession() { MESDataType = "SN", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(SNLoadPoint);
            }

            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }

            //MESStationSession LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            //if (LinkQty == null)
            //{
            //    throw new Exception("RepairFlag=1 error!");
            //}
            //Linkqty = Convert.ToUInt16(LinkQty.Value.ToString());

            //SNLoadPoint.Value = "";
            //Station.StationSession.Add(SNLoadPoint);
            SN sn;
            if (SNLoadPoint.Value == null)
            {
                string snStr = Input.Value.ToString();
                sn = new SN(snStr, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            else if (SNLoadPoint.Value is string)
            {
                sn = new SN(SNLoadPoint.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNLoadPoint.Value;
            }


            //if (Station.StationName != sn.NextStation)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("產品不在當前工站！！"));
            //}

            if (sn.RepairFailedFlag == 1.ToString())
            {
                List<R_REPAIR_FAILCODE> rf = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(f => f.SN == sn.SerialNo && f.REPAIR_FLAG == "0").ToList();
                if (rf.Count > 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190327103914", new string[] { "REPAIR" }));
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190327103914", new string[] { "REPAIR CHECK OUT" }));
                }
                throw new Exception("RepairFlag=1 error!");
            }
            List<R_SN_PASS> checkPassStation = Station.SFCDB.ORM.Queryable<R_SN_PASS>().Where(t => t.PASS_STATION == "ORT" && t.SN == sn.SerialNo && t.TYPE == "BYPASSORT" && t.STATUS == "1").ToList();
            if (checkPassStation.Count == 0)
            {
                List<R_ORT_ALERT> roa = Station.SFCDB.ORM.Queryable<R_ORT_ALERT>().Where(t => t.SN == sn.SerialNo).ToList();
                List<R_ORT> ro = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == sn.SerialNo).ToList();
                List<R_ORT> rot = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == sn.SerialNo && t.ORTEVENT == "ORTOUT").ToList();
                if (roa.Count > 0 && ro.Count == 0)
                {
                    //throw new Exception($@"{sn.SerialNo}被抽中ORT，請掃描ORTIN!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163005", new string[] { sn.SerialNo }));
                }
                if (ro.Count > 0 && rot.Count == 0)
                {
                    //throw new Exception($@"{sn.SerialNo}被掃入ORT，請維護ORT!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163042", new string[] { sn.SerialNo }));
                }
            }
            //STOCKIN暂时不卡
            //if (sn.CompletedFlag == 1.ToString())
            //{
            //    throw new Exception("CompleteFlag=1 error!");
            //}
            if (sn.PackedFlag == 1.ToString())
            {
                throw new Exception("PackFlag=1 error!");
            }
            if (sn.ShippedFlag == 1.ToString())
            {
                throw new Exception("ShipFlag=1 error!");
            }
            //2022-02-25--被danmacia 屏蔽掉過，現在恢復--03-15-G6007338
            if (sn.CurrentStation == "MRB")
            {
                throw new Exception("MRB error!");
            }

            //if (Wo.INPUT_QTY + Linkqty > Wo.WORKORDER_QTY)
            //{
            //    throw new Exception("Input_qty  + Linkqty > WORKORDER_QTY !");
            //}
            Station.AddMessage("MES00000067", new string[] { sn.SerialNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 檢查掃入SN是否已經Load入系統.
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadingSNIsExistCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn = "";
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                sn = Input.Value.ToString();
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = sn, SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                sn = InputSN_Session.Value as string;
                if (string.IsNullOrEmpty(sn))
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            T_R_SN tr_sn = new T_R_SN(Station.SFCDB, Station.DBType);

            if (tr_sn.CheckSNExists(sn, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { sn }));
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }

        }

        /// <summary>
        /// add SN status check for reopen function vince_20200228
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNReopenStatusChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder Wo;
            //int Linkqty;
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }

            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                SNLoadPoint = new MESStationSession() { MESDataType = "SN", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(SNLoadPoint);
            }

            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }

            SN sn;
            if (SNLoadPoint.Value == null)
            {
                string snStr = Input.Value.ToString();
                sn = new SN(snStr, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            else if (SNLoadPoint.Value is string)
            {
                sn = new SN(SNLoadPoint.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNLoadPoint.Value;
            }

            if (sn.CompletedFlag != 1.ToString())
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000168_1", new string[] { Input.Value.ToString() }));
            }

            Station.AddMessage("MES00000067", new string[] { sn.SerialNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 2019-01-14 Patty added for REPRINT function station check
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintStationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);


            if (SSN == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string inputValue = SSN.Value.ToString();
            SN SNObj = null;
            bool isPass = false;
            string passStation = "";
            try
            {
                SNObj = new SN(inputValue, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(SNObj.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + inputValue }));
                }
                //2C check
                T_R_WO_BASE O_TWO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE Row_R_WO_BASE = (Row_R_WO_BASE)O_TWO.NewRow();
                Row_R_WO_BASE = O_TWO.LoadWorkorder(SNObj.WorkorderNo, Station.SFCDB);
                string str_PType = Row_R_WO_BASE.SKU_SERIES;

                switch (Station.StationName)
                {
                    case "REPRINT_ASSY1":
                        passStation = "ASSY1";
                        break;
                    case "REPRINT_ASSY6":
                        passStation = "ASSY6";
                        break;
                    case "REPRINT_PACKOUT":
                        passStation = "PACKOUT";
                        break;
                    case "REPRINT_SIDE": //20190306 patty added : for X8-8
                        passStation = "ASSY1";
                        //20190306 patty added for X8-8 reprint station validaiton
                        if (!str_PType.Contains("-8"))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000268", new string[] { inputValue }));
                        }

                        break;
                    default:
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { " passStation: " + passStation }));
                }


                T_R_SN_STATION_DETAIL Table_R_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);

                isPass = Table_R_SN_STATION_DETAIL.HadWriteIntoDetail(inputValue, passStation, Station.SFCDB);

                if (!isPass)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180813114004", new string[] { inputValue, passStation }));
                }


                Station.AddMessage("MES00000067", new string[] { inputValue }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Patty added: SILOADING check without printing or change status
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ORACLE_SNInputActionCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN SnTable = null;
            T_R_WO_BASE WoTable = null;
            T_R_WO_BOM t_r_wo_bom = new T_R_WO_BOM(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            WorkOrder WorkOrder = null;
            if (Paras.Count != 1)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession Wo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder = (WorkOrder)Wo.Value;
            bool Woexists = t_r_wo_bom.CheckWOExist(Wo.Value.ToString(), Station.SFCDB);
            //get Sku
            SKU sku = new SKU();
            sku.Init(WorkOrder.SkuNO, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //get SNRule name
            string SNRULE = sku.SnRule;
            //Make WO SNs
            List<string> SNS = new List<string>();

            Station.SFCDB.BeginTrain();

            for (int i = 0; i < WorkOrder.WORKORDER_QTY - WorkOrder.INPUT_QTY; i++)
            {
                SNS.Add(SNmaker.GetNextSN(SNRULE, Station.DBS["SFCDB"]));
            }
            if (SNS.Count == 0)
            {
                Station.SFCDB.RollbackTrain();
                throw new Exception("Workorder is Full !");
            }


            //input SNsToWo
            List<R_SN> SNs = new List<R_SN>();
            for (int i = 0; i < SNS.Count; i++)
            {
                var SN = new R_SN();
                SN.SN = SNS[i];
                SN.SKUNO = WorkOrder.SkuNO;
                SN.WORKORDERNO = WorkOrder.WorkorderNo;
                SN.PLANT = WorkOrder.PLANT;
                SN.ROUTE_ID = WorkOrder.RouteID;
                SN.STARTED_FLAG = "1";
                SN.PACKED_FLAG = "0";
                SN.COMPLETED_FLAG = "0";
                SN.SHIPPED_FLAG = "0";
                SN.REPAIR_FAILED_FLAG = "0";
                SN.CURRENT_STATION = Station.StationName;
                SN.CUST_PN = WorkOrder.CUST_PN;
                SN.SCRAPED_FLAG = "0";
                SN.PRODUCT_STATUS = "FRESH";
                SN.REWORK_COUNT = 0d;
                SN.VALID_FLAG = "1";
                SN.EDIT_EMP = Station.LoginUser.EMP_NO;
                SNs.Add(SN);
            }
            SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
            //SnTable.AddToRSn(SNs, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB); // 插入到 R_SN 表中

            WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            //WoTable.AddCountToWo(WorkOrder.WorkorderNo, SNs.Count, Station.SFCDB); //更新工單投入數量
            //KP Extent
            MESStation.LogicObject.SN SNO = new SN();
            for (int i = 0; i < SNs.Count; i++)
            {

                try
                {
                    if (WorkOrder.SKU_NAME == "X7-2C" || WorkOrder.SKU_NAME == "ODA_HA" || WorkOrder.SKU_NAME == "BDA_ATO" || WorkOrder.SKU_NAME == "BDA_ATO" || WorkOrder.SKU_NAME == "ORACLE_RACK" || WorkOrder.SKU_NAME == "E1-2C" || WorkOrder.SKU_NAME == "E2-2C" || (WorkOrder.PRODUCTION_TYPE == "PTO" && WorkOrder.SKU_NAME == "X8-8") || (WorkOrder.PRODUCTION_TYPE == "PTO" && WorkOrder.SKU_NAME == "ODA_X8-2"))
                    {
                        SNO.InsertR_SN_KP(WorkOrder, SNs[i], Station.SFCDB, Station, DB_TYPE_ENUM.Oracle);

                    }
                    else
                    {
                        if (!Woexists)
                        {
                            throw new Exception("Workorder missing WO BOM !");
                        }
                        SNO.ORAInsertR_SN_KP(WorkOrder, SNs[i], Station.SFCDB, Station, DB_TYPE_ENUM.Oracle);
                    }

                }
                catch (Exception ex)
                {
                    Station.SFCDB.RollbackTrain();
                    throw new Exception(ex.Message);
                }



            }
            Station.SFCDB.RollbackTrain();

        }

        /// <summary>
        /// 檢查 SN 是否已經包裝、入庫、出貨、待維修、報廢
        /// 傳入一個 SN 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnStatusChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 1)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { });
                throw new MESReturnMessage(ErrMesg);
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(ErrMesg);
            }
            SN sn = (SN)SNSession.Value;
            if (sn.PackedFlag.Equals("1"))
            {
                //已經包裝
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180906161840", new string[] { sn.SerialNo, "PACKED" });
                throw new MESReturnMessage(ErrMesg);
            }
            if (sn.CompletedFlag.Equals("1"))
            {
                //已經入庫
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180906161840", new string[] { sn.SerialNo, "STOCKED" });
                throw new MESReturnMessage(ErrMesg);
            }
            if (sn.ShippedFlag.Equals("1"))
            {
                //已經出貨
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180906161840", new string[] { sn.SerialNo, "SHIPPED" });
                throw new MESReturnMessage(ErrMesg);
            }
            if (sn.RepairFailedFlag.Equals("1"))
            {
                //待維修
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180906161840", new string[] { sn.SerialNo, "REPAIRING" });
                throw new MESReturnMessage(ErrMesg);
            }
            if (sn.ScrapedFlag.Equals("1"))
            {
                //已報廢
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180906161840", new string[] { sn.SerialNo, "SCRAPED" });
                throw new MESReturnMessage(ErrMesg);
            }
            //Station.AddMessage("", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);

        }

        public static void CheckMrbToStorage(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            string To_Storage = string.Empty;

            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession ToStorageSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (ToStorageSession == null || ToStorageSession.Value == null || ToStorageSession.Value.ToString().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                To_Storage = ToStorageSession.Value.ToString();
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN sn = (SN)SNSession.Value;
            var sqllock = $@"select * from r_sn_lock lk,r_sn sn,r_packing pk where lk.sn=sn.workorderno and sn.skuno=pk.skuno and (lk.sn='{sn}' or sn.sn='{sn}'or pk.pack_no='{sn}')and lk.LOCK_STATUS='1' ";
            DataTable dlock = Station.SFCDB.ExecSelect(sqllock, null).Tables[0];

            var sqlwotype = $@"select * from r_wo_type where prefix =(select substr(workorderno,1,6) from r_sn where (sn='{sn}'or boxsn='{sn}'))";
            DataTable dwotype = Station.SFCDB.ExecSelect(sqlwotype, null).Tables[0];
            DataRow aa = dwotype.Rows[0];

            var sqlcategory = $@"select * from r_sn_keypart_detail where KEYPART_SN='{sn}'";
            DataTable dcategory = Station.SFCDB.ExecSelect(sqlcategory, null).Tables[0];

            var sqlshipsn = $@"select * from r_ship_detail where sn='{sn}'";
            DataTable dship = Station.SFCDB.ExecSelect(sqlshipsn, null).Tables[0];

            var sqlpl = $@"select count(rs.sn) countrssn from r_packing rp,r_packing rpk,r_sn_packing rsp,r_sn rs where rp.id=rpk.parent_pack_id and rsp.pack_id=rpk.id and rsp.sn_id=rs.id and rp.pack_no='{sn}'";
            DataTable dpl = Station.SFCDB.ExecSelect(sqlpl, null).Tables[0];
            DataRow bb = dwotype.Rows[0];

            var sqlrmacount = $@"select count(rm.sn) countrmasn from r_packing rp,r_packing rpk,r_sn_packing rsp,r_sn rs,r_mrb rm where rp.id=rpk.parent_pack_id and rsp.pack_id=rpk.id and rsp.sn_id=rs.id and rs.sn=rm.sn and rp.pack_no='{sn}'";
            DataTable dramcount = Station.SFCDB.ExecSelect(sqlrmacount, null).Tables[0];
            DataRow cc = dwotype.Rows[0];

            for (int i = 1; i < Paras.Count; i++)
            {
                switch (To_Storage)
                {
                    case "027M":
                        if (aa["PRODUCT_TYPE"].ToString() != "RMA")
                        {
                            //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("此序列號不符合入027M的條件,必須是RMA產品!!"));
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160601"));
                        }
                        break;
                    case "029M":
                        if (dlock.Rows.Count > 0)
                        {
                            //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該工單‘" + dlock.Rows[0]["sn.workorderno"] + "’被鎖原因:'" + dlock.Rows[0]["lk.LOCK_REASON"] + "',請通知PE/PQE確認處理!"));
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162000", new string[] { dlock.Rows[0]["sn.workorderno"].ToString(), dlock.Rows[0]["lk.LOCK_REASON"].ToString() }));
                        }
                        if (dcategory.Rows.Count > 0)
                        {
                            //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("重工須以最高父項入庫!!"));
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162130"));
                        }
                        if (dship.Rows.Count > 0)
                        {
                            //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該產品已經掃SHIPPING,不允許進入MRB!!"));
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162148"));
                        }
                        break;
                }

                if (dpl.Rows.Count >= 1)
                {
                    if ((int)bb["countrssn"] == (int)cc["countrmasn"])
                    {
                        //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該棧板已經在MRB倉!"));
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162203"));
                    }
                }
                else if (dpl.Rows.Count == 0 && sn.ToString().Substring(0, 2) == "PL")
                {
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該棧板沒有產品!"));
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162222"));
                }
            }
        }

        /// <summary>
        /// 檢查 工單是否需要做FAI首件確認
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnIsFai(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;

            string NewSn = Input.Value.ToString();
            SN sn = (SN)SNSession.Value;
            T_R_FAI_STATION TRFD = new T_R_FAI_STATION(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_FAI RFAI = new T_R_FAI(SFCDB, DB_TYPE_ENUM.Oracle);
            R_FAI rfai = new R_FAI();

            string wo = sn.WorkorderNo;

            //判斷該工單是否需要做FAI，並且是否做過FAI
            if (!RFAI.CheckWoHaveDoneFai(sn.WorkorderNo, Station.StationName, SFCDB))
            {
                //rfai = RFAI.GetRemakbyid(sn.ID, SFCDB);

                UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "EmpNO", Tittle = "FAI Check", Type = UIInputType.String, Name = "EmpNO", ErrMessage = "Cancel Check" };
                //I.OutInputs.Add(new DisplayOutPut() { Name = "Description", DisplayType = EnumHelper.GetEnumName(UIOutputType.Text), Value = rfai.REMARK });
                var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);

                T_c_user t_c_user = new T_c_user(Station.SFCDB, Station.DBType);
                Row_c_user rowSendUser = t_c_user.getC_Userbyempno(ret.ToString(), Station.SFCDB, Station.DBType);

                if (rowSendUser == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { ret.ToString(), ret.ToString() }));
                }

                UIInputData O = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "PassWord", Tittle = "FAI Check", Type = UIInputType.Password, Name = "PassWord", ErrMessage = "Cancel Check" };
                O.OutInputs.Add(new DisplayOutPut() { Name = "EmpNO", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = ret.ToString() });
                var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station);

                Row_c_user rowSendUserandPass = t_c_user.getC_Userbyempnoandpass(ret.ToString(), Station.BU, ret1.ToString(), Station.SFCDB, Station.DBType);
                if (rowSendUserandPass == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200706140440", new string[] { ret.ToString(), ret.ToString() }));
                }

                UIInputData Q = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "REMARK", Tittle = "FAI Check", Type = UIInputType.Password, Name = "REMARK", ErrMessage = "Cancel Check" };
                Q.OutInputs.Add(new DisplayOutPut() { Name = "EmpNO", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = ret.ToString() });
                Q.OutInputs.Add(new DisplayOutPut() { Name = "PassWord", DisplayType = UIOutputType.Password.Ext<EnumNameAttribute>().Description, Value = ret1.ToString() });
                var ret2 = Q.GetUiInput(Station.API, UIInput.Normal, Station);

                if (ret2 == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { ret.ToString(), ret.ToString() }));
                }

                RFAI.RecordFAIBySN(NewSn, wo, ret2.ToString(), Station.StationName, Station.LoginUser.EMP_NO, Station.SFCDB);
            }
        }
        /// <summary>
        /// HWD檢查 SN在AOI工站是否有超過兩個小時，或者是否有提前首件確認
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// <summary>
        public static void CheckFaiSn2H(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            OleExec SFCDB = Station.SFCDB;
            string PanelNo = string.Empty;
            string Wo = string.Empty; string ErrMessage = string.Empty;

            string Newpanel = Input.Value.ToString();
            Panel panel = new Panel();
            T_R_FAI RFAI = new T_R_FAI(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_LOCK LockWo = new T_R_SN_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_SN_LOCK rowLockWo = (Row_R_SN_LOCK)LockWo.NewRow();

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            panel = (Panel)PanelSession.Value;
            PanelNo = PanelSession.InputValue.ToString();
            Wo = panel.Workorderno;
            var route = Station.SFCDB.ORM.Queryable<R_SN, R_PANEL_SN, C_ROUTE>((rs, ps, cr) => rs.WORKORDERNO == ps.WORKORDERNO && rs.SN == ps.SN && rs.ROUTE_ID == cr.ID).Where((rs, ps, cr) => ps.PANEL == PanelNo)
                       .Select((rs, ps, cr) => cr).ToList().FirstOrDefault();


            var ControlExist = Station.SFCDB.ORM.Queryable<C_CONTROL>().Any(t => t.CONTROL_NAME == "'FAI_CONTROL_ROUTE'" && t.CONTROL_VALUE == route.ROUTE_NAME);


            //增加開關，不卡此邏輯機種路由的直接關掉，與BIP同步
            if (!ControlExist)
            {
                //判斷該工單是否在BIP工站有記錄時間，且並未在AOI做過首件確認
                if (!RFAI.CheckWoHaveRecorfromBIP(Wo, Station.StationName, SFCDB))
                {
                    //判斷是否大於兩小時未做FAI
                    if (!RFAI.CheckSnHave2HoursFai(Wo, Station.StationName, SFCDB))
                    {
                        //未做首件確認直接鎖定所有工站
                        Station.SFCDB.BeginTrain();
                        rowLockWo = (Row_R_SN_LOCK)LockWo.NewRow();
                        rowLockWo.ID = LockWo.GetNewID(Station.BU, SFCDB);
                        rowLockWo.TYPE = "WO";
                        rowLockWo.WORKORDERNO = Wo;
                        rowLockWo.LOCK_STATION = "ALL";
                        rowLockWo.LOCK_STATUS = "1";
                        rowLockWo.LOCK_REASON = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812143603", new string[] { Wo });
                        //MSGCODE20210812143603
                        //"工單→" + Wo + ",未做工單首件確認";
                        rowLockWo.LOCK_TIME = System.DateTime.Now;
                        rowLockWo.LOCK_EMP = Station.LoginUser.EMP_NO.ToString();
                        SFCDB.ThrowSqlExeception = true;
                        SFCDB.ExecSQL(rowLockWo.GetInsertString(DB_TYPE_ENUM.Oracle));
                        SFCDB.CommitTrain();
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20200407201644", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
            }
        }

        public static void HWTSNReworkchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Mrb_WorkWO = "";
            string Sn = string.Empty;
            string Skuno_Route = string.Empty;
            if (Paras.Count < 3)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NextStationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            T_R_SN R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_WO_BASE R_WO_BASE = new T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            // 防止SN是臨技路由人重工報錯,如果有配置臨技路由則取工單路由
            Mrb_WorkWO = R_SN.GetDetailBySN(SnSession.Value.ToString(), Station.SFCDB).WORKORDERNO;
            string Sn_route = R_SN.GetDetailBySN(SnSession.Value.ToString(), Station.SFCDB).ROUTE_ID;
            string MRB_Wo_route = R_WO_BASE.GetWoByWoNo(Mrb_WorkWO, Station.SFCDB).ROUTE_ID;
            if (Sn_route != MRB_Wo_route)
            {
                Skuno_Route = MRB_Wo_route;
            }
            else
            {
                Skuno_Route = Sn_route;
            }
            T_R_SN_STATION_DETAIL R_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_ROUTE_DETAIL c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_r_task_order_sn r_task_order_sn = new T_r_task_order_sn(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_MRB R_MRB = new T_R_MRB(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_RELATIONDATA_DETAIL R_RELATIONDATA_DETAIL = new T_R_RELATIONDATA_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            string NextStation = NextStationSession.InputValue.ToString();
            string RetureSn = R_SN_STATION_DETAIL.CheckRetureOrnot(SnSession.Value.ToString(), Station.SFCDB).STATION_NAME;//檢查是否有SN退站
            string snevenpoit = c_route_detail.GetSnStation(Skuno_Route, NextStation, Station.SFCDB).Equals("STATION_NAME").ToString();//取出入重工工站的前一工站
            string PassSn = R_SN_STATION_DETAIL.GetSntationPassDetail(SnSession.Value.ToString(), snevenpoit, Station.SFCDB).SN;//確保最後一次過站是Pass的
            string PassRmaSn = R_SN_STATION_DETAIL.GetRMASntationPassDetail(SnSession.Value.ToString(), NextStation, Station.SFCDB).SN;//前一工站沒有過站PASS的記錄,則查詢當前工站有沒PASS的記錄
            string PassMrbSn = R_SN_STATION_DETAIL.GetMrbSntationPassDetail(SnSession.Value.ToString(), NextStation, Station.SFCDB).SN;//針對外廠RMA板第一次入工單后沒有任何的過站記錄
            string QuitWo = R_SN_STATION_DETAIL.GetQuitSntationPassDetail(WoSession.Value.ToString(), Station.SFCDB).WORKORDERNO;//檢查是否有入報廢工單維護

            string ReturnStation = R_SN_STATION_DETAIL.GetReturnStation(SnSession.Value.ToString(), Station.SFCDB).Equals("ReturnStation").ToString();//獲取退站的工站


            //string RelationSn = R_RELATIONDATA_DETAIL.CheckSnExists(SnSession.Value.ToString(), Station.SFCDB).ToString();


            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            T_R_MES_LOG mesLog = new T_R_MES_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
            //string id = mesLog.GetNewID("ORACLE", sfcdb);
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

            //檢查SN是否有過退站
            if (RetureSn == null)
            {
                //取出入重工工站的前一工站
                if (snevenpoit != null)
                {
                    //確保最後一次過站是Pass的
                    if (PassSn == null)
                    {
                        //若前一工站沒有過站PASS的記錄,則查詢當前工站有沒PASS的記錄(針對外場RMA板再次入RMA工單的情況)
                        if (PassRmaSn == null)
                        {
                            //此種情況是針對外廠RMA板第一次入工單后沒有任何的過站記錄,遇盤點掃MRB后再次入RMA工單掃不進工單
                            if (PassMrbSn == null)
                            {
                                //如果有維護報廢工單就不卡過站記錄
                                if (QuitWo == null)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190829100604", new string[] { NextStation + snevenpoit }));
                                }
                                else
                                {
                                    sfcdb.BeginTrain();
                                    rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, sfcdb);
                                    rowMESLog.PROGRAM_NAME = "CloudMES";
                                    rowMESLog.CLASS_NAME = "RETURN";
                                    rowMESLog.FUNCTION_NAME = "RMA入報廢工單記錄";
                                    rowMESLog.LOG_MESSAGE = "RMA入報廢工單記錄:SN→" + SnSession.Value.ToString() + ",WO→" + WoSession.Value.ToString();
                                    rowMESLog.LOG_SQL = "";
                                    rowMESLog.EDIT_EMP = "OracleStation";
                                    rowMESLog.EDIT_TIME = System.DateTime.Now;
                                    rowMESLog.DATA1 = SnSession.Value.ToString();
                                    rowMESLog.DATA2 = WoSession.Value.ToString();
                                    sfcdb.ThrowSqlExeception = true;
                                    sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                    sfcdb.CommitTrain();
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("獲取工站錯誤!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162248"));
                }
            }
            else
            {
                string GetSnstationDetail = c_route_detail.GetSnstationDetail(Skuno_Route, ReturnStation, NextStation, Station.SFCDB).STATION_NAME;
                if (GetSnstationDetail != null)
                {
                    string CheckSnAfterReturn = R_SN_STATION_DETAIL.CheckSnAfterReturn(SnSession.Value.ToString(), snevenpoit, Station.SFCDB).STATION_NAME;//獲取退站的工站

                    if (CheckSnAfterReturn == null)
                    {
                        //如果有維護報廢工單就不卡過站記錄
                        if (QuitWo == null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190829100604", new string[] { NextStation + snevenpoit }));
                        }
                    }
                }
            }

            T_R_WO_TYPE WOType = new T_R_WO_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
            string ReWoRohs = R_WO_BASE.GetWoByWoNo(WoSession.Value.ToString(), Station.SFCDB).ROHS;
            string MrbWoRohs = R_WO_BASE.GetMrbWoByRohs(SnSession.Value.ToString(), Station.SFCDB).ROHS;

            //除了RMA工單，其他工單都要卡ROHS屬性一致
            if (!WOType.IsTypeInput("RMA", SnSession.Value.ToString().Substring(0, 6), sfcdb))
            {
                if (MrbWoRohs != null && MrbWoRohs != ReWoRohs)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830113926", new string[] { ReWoRohs + MrbWoRohs }));
                }
            }

            string ReWoCustVER = R_WO_BASE.GetWoByWoNo(WoSession.Value.ToString(), Station.SFCDB).SKU_VER.Substring(1, 1);
            string MrbCustVER = R_WO_BASE.GetMrbWoByRohs(SnSession.Value.ToString(), Station.SFCDB).SKU_VER.Substring(1, 1);

            if (MrbCustVER != null && MrbCustVER != ReWoCustVER)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830140709", new string[] { ReWoRohs + MrbWoRohs }));
            }

            string ReSkuno = R_WO_BASE.GetWoByWoNo(WoSession.Value.ToString(), Station.SFCDB).SKUNO;
            string MrbSkuno = R_WO_BASE.GetMrbWoByRohs(SnSession.Value.ToString(), Station.SFCDB).SKUNO;

            if (ReSkuno != MrbSkuno)
            {
                if (WoSession.Value.ToString().Substring(1, 6) == "002163" || WoSession.Value.ToString().Substring(1, 6) == "002164")
                {
                    if (ReSkuno.Length > 10)
                    {
                        if (ReSkuno.Substring(ReSkuno.Length - 1, 2) == "A3")
                        {
                            string aa = string.Empty;
                            string bb = string.Empty;
                            if (ReSkuno.Substring(1, 1) == "T" || ReSkuno.Substring(1, 1) == "Q" || ReSkuno.Substring(1, 1) == "R" || ReSkuno.Substring(1, 1) == "N")
                            {
                                aa = "Y";
                            }
                            if (MrbSkuno.Substring(1, 1) == "F" || MrbSkuno.Substring(1, 1) == "C" || MrbSkuno.Substring(1, 1) == "D" || MrbSkuno.Substring(1, 1) == "G" || MrbSkuno.Substring(1, 1) == "L")
                            {
                                bb = "Y";
                            }
                            if (aa != bb)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830160027", new string[] { ReSkuno + MrbSkuno }));
                            }
                        }
                        else if (ReSkuno.Substring(ReSkuno.Length - 8, 8) != MrbSkuno.Substring(MrbSkuno.Length - 8, 8))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830160027", new string[] { ReSkuno + MrbSkuno }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830160027", new string[] { ReSkuno + MrbSkuno }));
                    }
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190830160525");
                    throw new MESReturnMessage(errMsg);
                }
            }
            if (R_RELATIONDATA_DETAIL.CheckSnExists(SnSession.Value.ToString(), Station.SFCDB))
            {
                bool isupdatesnstatus1 = false;
                bool isupdatesnstatus2 = false;
                isupdatesnstatus1 = R_RELATIONDATA_DETAIL.UpdateMrbSnStatus1(SnSession.Value.ToString(), Skuno_Route, NextStation, Station.SFCDB);
                if (!isupdatesnstatus1)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190902100626");
                    throw new MESReturnMessage(errMsg);
                }
                isupdatesnstatus2 = R_RELATIONDATA_DETAIL.UpdateMrbSnStatus2(SnSession.Value.ToString(), Skuno_Route, NextStation, Station.SFCDB);
                if (!isupdatesnstatus2)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190902101444");
                    throw new MESReturnMessage(errMsg);
                }
            }
        }

        /// <summary>
        /// 檢查投入SN是否重碼,　SN 不能存在R_SN Table中,,PanelSN不能存在r_panel_sn table中
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:"WO",SESSION_KEY:"1",VALUE:""}</param>
        public static void InputSNDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN;
            bool isUsed = false;
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            strNewSN = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = strNewSN;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = strNewSN;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = strNewSN;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = strNewSN;
            }

            if (Paras[1].VALUE.ToString() == "0")//0是SN
            {
                T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
                isUsed = TR_SN.IsUsed(strNewSN, Station.SFCDB);
            }
            else if (Paras[1].VALUE.ToString() == "1")//1是PanelSN
            {
                T_R_PANEL_SN TR_PANEL_SN = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                isUsed = TR_PANEL_SN.CheckPanelExist(strNewSN, Station.SFCDB);
            }
            else
            {
                //throw new Exception("SNType is undefined !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000020", new string[] { "SNType", "0/1" }));
            }

            if (isUsed)
            {
                //Station.AddMessage("MES00000040", new string[] { strNewSN }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { strNewSN }));
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { strNewSN }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { strNewSN }));
            }
        }

        public static void CheckDuplicateByInputSN(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN;
            bool isUsed = false;
            // int snType = 0;//0是SN，1是PanelSN
            MESStationSession NewSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //浑身长满生殖器的鱼 MarkBy LJD 2020-7-7 09:37:49
            //if (NewSnSession != null)
            //{
            //    Station.StationSession.Remove(NewSnSession);
            //}
            //NewSnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Input.Value.ToString() };
            //Station.StationSession.Add(NewSnSession);
            //浑身长满生殖器的鱼

            strNewSN = NewSnSession.Value.ToString();

            if (Paras[1].VALUE.ToString() == "0")//0是SN
            {
                T_R_SN TR_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                isUsed = TR_SN.IsUsed(strNewSN, Station.SFCDB);
            }
            else if (Paras[1].VALUE.ToString() == "1")//1是PanelSN
            {
                T_R_PANEL_SN TR_PANEL_SN = new T_R_PANEL_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                isUsed = TR_PANEL_SN.CheckPanelExist(strNewSN, Station.SFCDB);
            }
            else
            {
                throw new Exception("SNType is undefined !");
            }

            if (isUsed)
            {
                //Station.AddMessage("MES00000040", new string[] { strNewSN }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                string ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { strNewSN });
                throw new MESReturnMessage(ErrMessage);
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { strNewSN }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// SN区间管控,依据机种系列或机种所配置的ModelTYPE判断并檢查投入SN是否處於工單區間(R_WO_Region)
        /// 2018/10/16 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void WoSNRangecheckerByModelTYPE(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN = "";
            WorkOrder wo;
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            strNewSN = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = strNewSN;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = strNewSN;
                InputSN_Session.InputValue = strNewSN;
                InputSN_Session.ResetInput = Input;
            }
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    wo = (WorkOrder)Wo_Session.Value;
                    if (wo.WorkorderNo == null || wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }

            T_C_SKU csku = new T_C_SKU(Station.SFCDB, Station.DBType);
            string modeltype = csku.GetModelTypeBySku(Station.SFCDB, wo.SkuNO);
            string skuroute = wo.RouteID;

            if (modeltype.IndexOf("007") >= 0 || modeltype.IndexOf("020") >= 0 || modeltype.IndexOf("022") >= 0 || modeltype.IndexOf("027") >= 0)
            {
                Station.AddMessage("MES00000001", new string[] { strNewSN }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                T_R_WO_REGION TR_WO_REGION = new T_R_WO_REGION(Station.SFCDB, Station.DBType);
                bool isInWoRange = TR_WO_REGION.CheckSNInWoRange(strNewSN, wo.WorkorderNo, Station.SFCDB, Station.BU);
                if (isInWoRange)
                {
                    Station.AddMessage("MES00000001", new string[] { strNewSN }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000056", new string[] { strNewSN, wo.WorkorderNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000056", new string[] { strNewSN, wo.WorkorderNo }));
                }
            }
        }

        /// <summary>
        /// 檢查投入SN是否處於工單區間,SN 要在工單的SN區間中(R_WO_Region)
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void WoSNRangeDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN = "";
            WorkOrder wo;
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            strNewSN = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = strNewSN;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = strNewSN;
                InputSN_Session.InputValue = strNewSN;
                InputSN_Session.ResetInput = Input;
            }
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    wo = (WorkOrder)Wo_Session.Value;
                    if (wo.WorkorderNo == null || wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            T_R_WO_REGION TR_WO_REGION = new T_R_WO_REGION(Station.SFCDB, Station.DBType);
            bool isInWoRange = TR_WO_REGION.CheckSNInWoRange(strNewSN, wo.WorkorderNo, Station.SFCDB, Station.BU);
            if (isInWoRange)
            {
                Station.AddMessage("MES00000001", new string[] { strNewSN }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                Station.AddMessage("MES00000056", new string[] { strNewSN, wo.WorkorderNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000056", new string[] { strNewSN, wo.WorkorderNo }));
            }
        }

        /// <summary>
        /// 檢查主板SN是否屬於指定的WO.
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">
        public static void InputSnBelongWoDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session != null)
            {
                SN ObjSN = (SN)SessionSN.Value;

                string ChkWo = Wo_Session.Value.ToString();
                string SnWo = ObjSN.WorkorderNo;

                if (ChkWo != SnWo)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181221170924", new string[] { ObjSN.SerialNo, ChkWo }));
                }
            }

        }

        /// <summary>
        /// 1.序號長度必須相同，且起始序號必須小于結束序號
        /// 2.序號必須在工單區間管控範圍內(R_WO_Region)
        /// 3.本次Loading的序號不能已經存在
        /// 2018/1/23 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void WoLotSNRangeDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strStartSN = "";
            string strEndSN = "";
            WorkOrder Wo;

            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_KEY + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }

            //Start SN必須加載
            MESStationSession StartSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StartSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (StartSN_Session.Value != null)
                {
                    strStartSN = StartSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            //End SN必須加載
            MESStationSession EndSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (EndSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                if (EndSN_Session.Value != null)
                {
                    strEndSN = EndSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }
            //1.序號長度必須相同，且起始序號必須小于結束序號
            if (strStartSN.Length != strEndSN.Length)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000117", new string[] { strStartSN, strEndSN }));
            }

            if (String.Compare(strStartSN, strEndSN) == 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000118", new string[] { strStartSN, strEndSN }));
            }
            //2.序號必須在工單區間管控範圍內(R_WO_Region)
            T_R_WO_REGION TR_WO_REGION = new T_R_WO_REGION(Station.SFCDB, Station.DBType);
            bool isInWoRange = TR_WO_REGION.CheckLotSNInWoRange(strStartSN, strEndSN, Wo.WorkorderNo, Station.SFCDB);
            if (isInWoRange == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000119", new string[] { strStartSN, strEndSN, Wo.WorkorderNo }));
            }
            // 3.本次Loading的序號不能已經存在
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            bool RangeIsUsed = TR_SN.SNRangeIsUsed(strStartSN, strEndSN, Station.SFCDB);
            if (RangeIsUsed == true)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000120", new string[] { strStartSN, strEndSN }));
            }

        }

        /// <summary>
        /// 1.判斷條碼后4位是否符合34進制編碼規則,ModelType=016
        /// 2.依據條碼后4位34進制編碼規則算出區間內有多少個SN:Var_Qty
        /// 3.本次Loading數量不能超過工單未Loading的總數量
        /// 2018/1/24 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void SNRange34HQtyDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strStartSN = "";
            string strEndSN = "";
            string strModelType = "";
            WorkOrder Wo;

            if (Paras.Count != 4)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }

            //Start SN必須加載
            MESStationSession StartSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StartSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (StartSN_Session.Value != null)
                {
                    strStartSN = StartSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            //End SN必須加載
            MESStationSession EndSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (EndSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                if (EndSN_Session.Value != null)
                {
                    strEndSN = EndSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }

            //ModelType必須加載
            MESStationSession ModelTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (ModelTypeSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            else
            {
                if (ModelTypeSession.Value != null)
                {
                    strModelType = ModelTypeSession.Value.ToString();
                }
                else
                {
                    strModelType = " ";
                }
            }

            if (strModelType.IndexOf("016") >= 0)
            {
                //1.判斷條碼后4位是否符合34進制編碼規則
                bool SnRuleFlag = Regex.IsMatch(strStartSN.Substring(strStartSN.Length - 4, 4), "[0-9A-HJ-MP-Z]{4}");
                if (SnRuleFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strStartSN, "[0-9A-HJ-MP-Z]{4}" }));
                }

                SnRuleFlag = Regex.IsMatch(strEndSN.Substring(strEndSN.Length - 4, 4), "[0-9A-HJ-MP-Z]{4}");
                if (SnRuleFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strEndSN, "[0-9A-HJ-MP-Z]{4}" }));
                }

                if (strStartSN.Substring(0, strStartSN.Length - 4) != strEndSN.Substring(0, strEndSN.Length - 4))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000126", new string[] { strStartSN, strEndSN }));
                }

                //2.依據條碼后4位34進制編碼規則算出區間內有多少個SN:Var_Qty
                T_R_WO_REGION TR_WO_REGION = new T_R_WO_REGION(Station.SFCDB, Station.DBType);
                int SnRangQty = TR_WO_REGION.GetQtyBy34HSNRange(strStartSN.Substring(strStartSN.Length - 4, 4), strEndSN.Substring(strEndSN.Length - 4, 4), Station.SFCDB);
                if (SnRangQty <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000123", new string[] { strStartSN, strEndSN }));
                }

                // 3.本次Loading數量不能超過工單未Loading的總數量
                if (Wo.WORKORDER_QTY < Wo.INPUT_QTY + SnRangQty)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000124", new string[] { SnRangQty.ToString(), (Wo.WORKORDER_QTY - Wo.INPUT_QTY).ToString() }));
                }
                //3.1 同時加載進制及數量
                MESStationSession DecimalType = Station.StationSession.Find(t => t.MESDataType == "DecimalType" && t.SessionKey == "1");
                if (DecimalType == null)
                {
                    DecimalType = new MESStationSession() { MESDataType = "DecimalType", SessionKey = "1", ResetInput = Input, InputValue = "34H" };
                    Station.StationSession.Add(DecimalType);
                }
                DecimalType.Value = "34H";

                MESStationSession LotQty = Station.StationSession.Find(t => t.MESDataType == "LotQty" && t.SessionKey == "1");
                if (LotQty == null)
                {
                    LotQty = new MESStationSession() { MESDataType = "LotQty", SessionKey = "1", ResetInput = Input, InputValue = SnRangQty.ToString() };
                    Station.StationSession.Add(LotQty);
                }
                LotQty.Value = SnRangQty.ToString();
            }
        }

        /// <summary>
        /// 1.判斷條碼后4位是否符合10進制編碼規則,ModelType=013
        /// 2.依據條碼后4位10進制編碼規則算出區間內有多少個SN:Var_Qty
        /// 3.本次Loading數量不能超過工單未Loading的總數量
        /// 2018/1/25 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void SNRange10HQtyDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strStartSN = "";
            string strEndSN = "";
            string strModelType = "";
            WorkOrder Wo;

            if (Paras.Count != 4)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }

            //Start SN必須加載
            MESStationSession StartSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StartSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (StartSN_Session.Value != null)
                {
                    strStartSN = StartSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            //End SN必須加載
            MESStationSession EndSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (EndSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                if (EndSN_Session.Value != null)
                {
                    strEndSN = EndSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }
            //ModelType必須加載
            MESStationSession ModelTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (ModelTypeSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            else
            {
                if (ModelTypeSession.Value != null)
                {
                    strModelType = ModelTypeSession.Value.ToString();
                }
                else
                {
                    strModelType = " ";
                }
            }

            if (strModelType.IndexOf("013") >= 0)
            {
                //1.判斷條碼后4位是否符合10進制編碼規則
                bool SnRuleFlag = Regex.IsMatch(strStartSN.Substring(strStartSN.Length - 4, 4), "[0-9]{4}");
                if (SnRuleFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strStartSN, "[0-9]{4}" }));
                }

                SnRuleFlag = Regex.IsMatch(strEndSN.Substring(strEndSN.Length - 4, 4), "[0-9]{4}");
                if (SnRuleFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strEndSN, "[0-9]{4}" }));
                }

                if (strStartSN.Substring(0, strStartSN.Length - 4) != strEndSN.Substring(0, strEndSN.Length - 4))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000126", new string[] { strStartSN, strEndSN }));
                }

                //2.依據條碼后4位10進制編碼規則算出區間內有多少個SN:Var_Qty
                int SnRangQty = Convert.ToInt32(strEndSN.Substring(strEndSN.Length - 4, 4)) - Convert.ToInt32(strStartSN.Substring(strStartSN.Length - 4, 4)) + 1;
                if (SnRangQty <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000123", new string[] { strStartSN, strEndSN }));
                }
                // 3.本次Loading數量不能超過工單未Loading的總數量
                if (Wo.WORKORDER_QTY < Wo.INPUT_QTY + SnRangQty)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000124", new string[] { SnRangQty.ToString(), (Wo.WORKORDER_QTY - Wo.INPUT_QTY).ToString() }));
                }
                //3.1 同時加載進制及數量
                MESStationSession DecimalType = Station.StationSession.Find(t => t.MESDataType == "DecimalType" && t.SessionKey == "1");
                if (DecimalType == null)
                {
                    DecimalType = new MESStationSession() { MESDataType = "DecimalType", SessionKey = "1", ResetInput = Input, InputValue = "10H" };
                    Station.StationSession.Add(DecimalType);
                }
                DecimalType.Value = "10H";

                MESStationSession LotQty = Station.StationSession.Find(t => t.MESDataType == "LotQty" && t.SessionKey == "1");
                if (LotQty == null)
                {
                    LotQty = new MESStationSession() { MESDataType = "LotQty", SessionKey = "1", ResetInput = Input, InputValue = SnRangQty.ToString() };
                    Station.StationSession.Add(LotQty);
                }
                LotQty.Value = SnRangQty.ToString();
            }
        }

        /// <summary>
        /// 檢查該 SN 是否已經被列印過，如果有的話，那麼就可以進行補印
        /// 需要一個參數 REPRINT_SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNPrintedChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 1)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMesg);
            }
            MESStationSession ReprintSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ReprintSnSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "ReprintSN" });
                throw new MESReturnMessage(ErrMesg);
            }
            string ReprintSn = ReprintSnSession.Value.ToString();
            if (ReprintSn.Length > 0)
            {
                T_R_WO_REGION_DETAIL T_RWRD = new T_R_WO_REGION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (T_RWRD.CheckSNExist(ReprintSn, Station.SFCDB))
                {
                    if (T_RWRD.CheckSNHasPrinted(ReprintSn, Station.SFCDB))
                    {
                        Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
                    }
                    else
                    {
                        ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180905110005", new string[] { ReprintSn });
                        throw new MESReturnMessage(ErrMesg);

                    }
                }
                else
                {
                    ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180905105828", new string[] { ReprintSn });
                    throw new MESReturnMessage(ErrMesg);
                }
            }
            else
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "ReprintSN" });
                throw new MESReturnMessage(ErrMesg);
            }
        }

        /// <summary>
        /// 當前狀態不能為MRB
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMrbchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            if (Paras.Count != 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Serial Number" });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSN = (SN)SessionSN.Value;

            if (ObjSN.CurrentStation == "MRB")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000066",
                                new string[] { ObjSN.SerialNo, ObjSN.CurrentStation });
                throw new MESReturnMessage(ErrMessage);
            }


            //add by 張官軍  2018-03-15
            //增加檢查邏輯
            //加载 session 中的工单
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Work Order" }));
            }
            var wo_session = WoSession.Value;
            WorkOrder wo = new WorkOrder();
            if (wo_session is string)
            {
                wo.Init(wo_session.ToString(), Station.SFCDB);
            }
            else if (wo_session is WorkOrder)
            {
                wo = (WorkOrder)WoSession.Value;
            }


            //判斷物料對應的機種是否存在
            T_C_SKU SkuTable = new T_C_SKU(Station.SFCDB, Station.DBType);
            if (SkuTable.GetSkuBySkuno(ObjSN.SkuNo, Station.SFCDB) == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000172", new string[] { wo.SkuNO, wo.SKU_VER });
                throw new MESReturnMessage(ErrMessage);
            }

            ////判斷該工單是否有該物料的需求
            //T_R_WO_ITEM ItemTable = new T_R_WO_ITEM(Station.SFCDB, Station.DBType);
            //if (!ItemTable.CheckExist(wo.WorkorderNo, ObjSN.SkuNo, Station.SFCDB))
            //{
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000196", new string[] { wo.WorkorderNo, ObjSN.SkuNo });
            //    throw new MESReturnMessage(ErrMessage);
            //}

            T_R_MRB MrbTable = new T_R_MRB(Station.SFCDB, Station.DBType);
            //判斷物料SN是否有MRB過	
            if (MrbTable.HadMrbed(ObjSN.SerialNo, Station.SFCDB))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000182", new string[] { ObjSN.SerialNo });
                throw new MESReturnMessage(ErrMessage);
            }

            //首先查看全局開關，是否要做後續檢測
            //如果有，則判斷該機種是否有設定不需要檢查 TS101 站位測試記錄
            //如果沒有設定就再判斷是否有報廢
            //如果也沒有，則判斷在 HW 測試系統中是否有測試 TS101 或者配置在 SFCCODELIKEDETAIL 中的站位的測試記錄
            if (MrbTable.GetMRBControl(Station.SFCDB))    //查看全局開關
            {
                if (!MrbTable.SkuCheckTS101(ObjSN.SkuNo, Station.SFCDB))    //判斷是否需要檢查 TS101
                {
                    if (!MrbTable.IsPreScrap(ObjSN.SerialNo, Station.SFCDB))    //判斷是否有報廢
                    {
                        if (!MrbTable.HasHWTest(ObjSN.SerialNo, Station.SFCDB))    //判斷是否有測試記錄
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000181", new string[] { ObjSN.SerialNo });
                            throw new MESReturnMessage(ErrMessage);
                        }
                    }
                }
            }

            Station.AddMessage("MES00000067", new string[] { ObjSN.SerialNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);


        }

        /// <summary>
        /// 檢查工單需求里是否包含SN的料號
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMrbWoRequest(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Serial Number" });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSN = (SN)SessionSN.Value;

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Work Order" }));
            }
            var wo_session = WoSession.Value;
            WorkOrder wo = new WorkOrder();
            if (wo_session is string)
            {
                wo.Init(wo_session.ToString(), Station.SFCDB);
            }
            else if (wo_session is WorkOrder)
            {
                wo = (WorkOrder)WoSession.Value;
            }
            //判斷該工單是否有該物料的需求
            T_R_WO_ITEM ItemTable = new T_R_WO_ITEM(Station.SFCDB, Station.DBType);
            if (!ItemTable.CheckExist(wo.WorkorderNo, ObjSN.SkuNo, Station.SFCDB))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000196", new string[] { wo.WorkorderNo, ObjSN.SkuNo });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        /// <summary>
        /// 產品是Fail Check
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFailchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string RepairFlag = string.Empty;
            T_R_REPAIR_MAIN Grepair = new T_R_REPAIR_MAIN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_MAIN> RepairMainList = new List<R_REPAIR_MAIN>();

            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                //throw new Exception("请输入SN!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162335"));
            }
            else
            {
                SN ObjSN = (SN)SessionSN.Value;
                if (ObjSN.RepairFailedFlag == "1")  /// R_SN  RepairFailedFlag  欄位 1表示fail  0表示pass 
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000071",
                                    new string[] { ObjSN.SerialNo });
                    throw new MESReturnMessage(ErrMessage);
                }
                RepairMainList = Grepair.GetRepairMainBySN(Station.SFCDB, ObjSN.SerialNo);

                if (RepairMainList.Count != 0)
                {
                    if (RepairMainList[0].CLOSED_FLAG == "0")    /// R_REPAIR_MAIN  close_flag  欄位 1表示pass  0表示fail
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000071",
                                       new string[] { ObjSN.SerialNo });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }

                Station.AddMessage("MES00000067", new string[] { ObjSN.SerialNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);

            }
        }

        /// <summary>
        /// 產品LOT Status狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLotStatuschecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string RepairFlag = string.Empty;
            LotNo ObjLot = new LotNo();
            Row_R_LOT_STATUS GetLotNo;
            T_R_LOT_DETAIL RowLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count <= 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession SessionNewLotFlag = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (SessionNewLotFlag == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (SessionSN == null)
            {
                //throw new Exception("请输入SN!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162335"));
            }
            else
            {
                SN ObjSN = (SN)SessionSN.Value;

                GetLotNo = RowLotStatus.GetLotBySN(ObjSN.SerialNo, Station.SFCDB);
                if (GetLotNo != null)
                {
                    //modify by LLF 2018-02-22 
                    //ObjLot.Init(GetLotNo.LOT_NO, Station.SFCDB);
                    ObjLot.Init(GetLotNo.LOT_NO, ObjSN.SerialNo, Station.SFCDB);

                    if (ObjLot.CLOSED_FLAG == "0") ///該批次未被關閉，系統報錯產品已入批次
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000080",
                                    new string[] { ObjSN.SerialNo, ObjLot.LOT_NO });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    //Marked by LLF 2018-02-07
                    //if (ObjSN.NextStation == "SMT_FQC")///SN待過工站不為SMT_FQC, 系統報錯產品不需要掃入批次
                    //{
                    //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000066",
                    //                new string[] { ObjSN.SerialNo, ObjSN.NextStation });
                    //    throw new MESReturnMessage(ErrMessage);
                    //}
                    if (ObjLot.CLOSED_FLAG == "1" && ObjLot.LOT_STATUS_FLAG == "0") ///該批次關閉,且LotStatusFlag 为0，系統報錯產品已入批次，处于待抽驗狀態
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000160",
                                    new string[] { ObjSN.SerialNo, ObjLot.LOT_NO });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    if (ObjLot.SAMPLING == "4")//SN是狀態為：R_LOT_DETAIL.Sampling=4,則報錯，需解鎖,
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000081",
                                    new string[] { ObjSN.SerialNo });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }

                Station.AddMessage("MES00000067", new string[] { ObjSN.SerialNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// SN產品LOT Status狀態是否為鎖定狀態
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFQCLotLockchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string SnOrLotno = string.Empty;
            //string Lotno = "";
            LotNo ObjLot = new LotNo();
            Row_R_LOT_STATUS GetLotNoBySN;
            Row_R_LOT_STATUS GetLotNoByLot;
            T_R_LOT_DETAIL RowLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count <= 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SessionSNorLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSNorLotNo == null)
            {
                //throw new Exception("请输入SN !");
                SessionSNorLotNo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSNorLotNo);
            }
            SnOrLotno = Input.Value.ToString();

            GetLotNoBySN = RowLotStatus.GetLotBySN(SnOrLotno, Station.SFCDB);
            GetLotNoByLot = RowLotStatus.GetByInput(SnOrLotno, "LOT_NO", Station.SFCDB);
            //add by LLF 2018-02-19
            if (GetLotNoBySN == null && GetLotNoByLot == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000143",
                                new string[] { SnOrLotno });
                throw new MESReturnMessage(ErrMessage);
            }

            //add by LLF 2018-02-19

            if (GetLotNoBySN != null)  ///COUNT=0 說明輸入的是LOTNO號，否則輸入的為SN
            {
                //modify by LLF 2018-02-22
                //ObjLot.Init(GetLotNo.LOT_NO, Station.SFCDB);
                ObjLot.Init(GetLotNoBySN.LOT_NO, SnOrLotno, Station.SFCDB);

                if (ObjLot.CLOSED_FLAG == "0") ///該批次未被關閉
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000080",
                                new string[] { SnOrLotno, ObjLot.LOT_NO });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (ObjLot.SAMPLING != "4")//該SN  R_LOT_DETAIL SAMPLING=4 處於鎖定狀態
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000114",
                                new string[] { SnOrLotno });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                //modify by LLF 2018-02-22
                //ObjLot.Init(SnOrLotno,"", Station.SFCDB);
                //add by LLF 2018-02-19
                if (GetLotNoByLot.CLOSED_FLAG == "0") ///該批次未被關閉，系統報錯產品已入批次
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000080",
                                new string[] { SnOrLotno, ObjLot.LOT_NO });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (!RowLotDetail.CheckLotNoDetailStatus(GetLotNoByLot.ID, Station.SFCDB))
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000114",
                                new string[] { SnOrLotno });
                    throw new MESReturnMessage(ErrMessage);
                }

            }

            SessionSNorLotNo.Value = SnOrLotno;
            Station.AddMessage("MES00000067", new string[] { SnOrLotno }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 產品掃描Rework SN Check
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// 1.	當前狀態要為MRB
        ///2.	SN 工單不能與當前重工工單一致
        public static void SNReworkchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            //string Sn = "";
            //string Sn_Satation = "";
            //string Sn_Wo = "";
            //MESStationSession SnInput = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");
            //if (SnInput == null)
            //{
            //    //Station.AddMessage("MES00000076", new string[] { "Sn", Sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    SN ObjSn = new SN();
            //    ObjSn = (SN)SnInput.Value;
            //    Sn = ObjSn.SerialNo;
            //    Sn_Satation = ObjSn.CurrentStation;
            //    Sn_Wo = ObjSn.WorkorderNo;
            //}
            string Rework_WO = "";
            string Sn = Input.Value.ToString();
            T_R_SN R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string Sn_Satation = R_SN.GetDetailBySN(Sn, Station.SFCDB).CURRENT_STATION;
            string Sn_Wo = R_SN.GetDetailBySN(Sn, Station.SFCDB).WORKORDERNO;
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_RMA_BONEPILE t_r_rma_bonepile = new T_R_RMA_BONEPILE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            bool isRMAopen = t_r_rma_bonepile.RmaBonepileIsOpen(Station.SFCDB, Sn);

            if (WoInput == null)
            {
                Station.AddMessage("MES00000050", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                return;
            }
            else
            {
                WorkOrder ObjWo = new WorkOrder();
                ObjWo = (WorkOrder)WoInput.Value;
                Rework_WO = ObjWo.WorkorderNo;
            }
            //object next_station = (SN)Station.StationSession[1].Value;
            // add by fgg 2018.05.03 RMA 入RMA后掃重工
            if (Sn_Satation != "MRB" && Sn_Satation != "RMA" && !Sn_Satation.Contains("B29"))
            {
                //Station.AddMessage("MES00000076", new string[] { Sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000076", new string[] { Sn }));
            }
            else if (Sn_Wo == Rework_WO)
            {
                //  Station.AddMessage("MES00000077", new string[] { Sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000077", new string[] { Sn }));
            }
            //防止RMA 品入正常重工工單 add by wyb 2021年11月10日
            else if (isRMAopen)
            {
                var isRmaWo = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == Rework_WO && SqlSugar.SqlFunc.Contains(t.WO_TYPE, "RMA")).Any();//前提是配工單類型表時要統一使用WO_TYPE 包含 RMA   NNVT VNDCN JZDCN
                if (!isRmaWo)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211110155122"));
                }
            }
            else
            {
                //MESStationSession SNRework = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                //if (SNRework == null)
                //{
                //    SNRework = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //    Station.StationSession.Add(SNRework);
                //}
                //SNRework.Value = Sn;
                Station.AddMessage("MES00000101", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 輸入工單的版本與SN 版本對比檢查
        /// SN 工單版本與輸入工單版本比較，不相同，則報錯，相同通過；
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">SESSION_TYPE:"SN",SESSION_TYPE:"WO"</param>
        public static void InputWoVerSNVerchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            string Sn = Input.Value.ToString();
            string WoInput_Ver = "";//取輸入工單的料號版本
            string WoFromSn_Ver = "";//取輸入SN的工單料號版本
                                     //MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");//配WoFromSNDataloader方法，取WorkOrder對象
                                     //if (WoFromSn == null)
                                     //{
                                     //    //Station.AddMessage("MES00000076", new string[] { "SnInput", SnInput }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                                     //}
                                     //else
                                     //{
                                     //    WorkOrder ObjWoFromSn = new WorkOrder();
                                     //    ObjWoFromSn = (WorkOrder)WoFromSn.Value;
                                     //    WoFromSn_Ver = ObjWoFromSn.SKU_VER;
                                     //}
            T_R_SN R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string Sn_Wo = R_SN.GetDetailBySN(Sn, Station.SFCDB).WORKORDERNO;
            T_R_WO_BASE R_WO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            WoFromSn_Ver = R_WO.LoadWorkorder(Sn_Wo, Station.SFCDB).SKU_VER;
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);//配WoDataloader方法，取WorkOrder對象
            if (WoInput == null)
            {
                Station.AddMessage("MES00000050", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                return;
            }
            else
            {
                WorkOrder ObjWoInput = new WorkOrder();
                ObjWoInput = (WorkOrder)WoInput.Value;
                WoInput_Ver = ObjWoInput.SKU_VER;
            }
            if (WoFromSn_Ver == null || WoInput_Ver == null)
            {
                Station.AddMessage("MES00000006", new string[] { "SKU_VER" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else if (WoFromSn_Ver != WoInput_Ver)
            {
                Station.AddMessage("MES00000084", new string[] { "Input workorder", "Input sn" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (WoFromSn == null)
                {
                    WoFromSn = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(WoFromSn);
                }
                WoFromSn.Value = Sn;
                Station.AddMessage("MES00000085", new string[] { "Input workorder", "Input sn" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 輸入SN料號與工單料號對比檢查
        /// 將SN 對應工單的料號與輸入工單的料號進行比較，不相同，則報錯，相同，則通過；
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">SESSION_TYPE:"SN",SESSION_TYPE:"WO"</param>
        public static void InputSNSkuWoSkuchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            string Sn = Input.Value.ToString();
            string WoInput_Skuno = "";//取輸入工單的料號版本
            string WoFromSn_Skuno = "";//取輸入SN的工單料號版本
                                       //MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");//配WoFromSNDataloader方法，取WorkOrder對象
                                       //if (WoFromSn == null)
                                       //{
                                       //    //Station.AddMessage("MES00000076", new string[] { "SnInput", SnInput }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                                       //}
                                       //else
                                       //{
                                       //    WorkOrder ObjWoFromSn = new WorkOrder();
                                       //    ObjWoFromSn = (WorkOrder)WoFromSn.Value;
                                       //    WoFromSn_Ver = ObjWoFromSn.SKU_VER;
                                       //}
            T_R_SN T_R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_CONTROL TCC = new T_C_CONTROL(Station.SFCDB, Station.DBType);
            R_SN R_SN = T_R_SN.GetDetailBySN(Sn, Station.SFCDB);
            string Sn_Wo = R_SN.WORKORDERNO;
            //T_R_WO_BASE R_WO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //Row_R_WO_BASE R_WO_BASE = R_WO.LoadWorkorder(Sn_Wo, Station.SFCDB);
            WoFromSn_Skuno = R_SN.SKUNO;// R_WO_BASE.SKUNO;
            string WoInput_Wo = "";
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");//配WoDataloader方法，取WorkOrder對象
            if (WoInput == null)
            {
                //Station.AddMessage("MES00000050", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050", new string[] { }));

            }
            else
            {
                WorkOrder ObjWoInput = new WorkOrder();
                ObjWoInput = (WorkOrder)WoInput.Value;
                WoInput_Skuno = ObjWoInput.SkuNO;
                WoInput_Wo = ObjWoInput.WorkorderNo;
            }
            if (WoFromSn_Skuno == null || WoInput_Skuno == null)
            {
                //Station.AddMessage("MES00000006", new string[] { "SKUNO" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else if (WoFromSn_Skuno != WoInput_Skuno)
            {
                bool Valid = false;

                List<C_CONTROL> controls = TCC.GetControlList("SKUNO_CAN_REWORK_MAPPING", Station.SFCDB);
                foreach (C_CONTROL control in controls)
                {
                    if (control.CONTROL_VALUE.Contains(WoFromSn_Skuno) && control.CONTROL_VALUE.Contains(WoInput_Skuno))
                    {
                        Valid = true;
                        break;
                    }
                }
                List<R_F_CONTROL> list_control = new List<R_F_CONTROL>();
                T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
                list_control = t_r_function_control.GetListByFcv("REWORK_TO_NEW_SKUNO", "SKUNO", Station.SFCDB);
                if (list_control.Exists(r => r.VALUE == WoInput_Skuno && r.EXTVAL == WoFromSn_Skuno))
                {
                    Valid = true;
                }
                if (!Valid)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000095", new string[] { ((WorkOrder)WoInput.Value).WorkorderNo, R_SN.SN }));
                }
                //Station.AddMessage("MES00000095", new string[] { "Input workorder", "Input sn" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                //MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                //if (WoFromSn == null)
                //{
                //    WoFromSn = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //    Station.StationSession.Add(WoFromSn);
                //}
                //WoFromSn.Value = Sn;
                //Station.AddMessage("MES00000096", new string[] { "Input workorder", "Input sn" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        public static void ScanSNSkuWoSkuchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
                //throw new Exception("參數數量不正確!");
            }
            string Sn = Input.Value.ToString();
            string WoInput_Skuno = "";//取輸入工單的料號版本
            string WoFromSn_Skuno = "";//取輸入SN的工單料號版本

            T_R_SN T_R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_SKU_DETAIL TCSD = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            R_SN R_SN = T_R_SN.GetDetailBySN(Sn, Station.SFCDB);
            string Sn_Wo = R_SN.WORKORDERNO;
            //T_R_WO_BASE R_WO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle)
            //Row_R_WO_BASE R_WO_BASE = R_WO.LoadWorkorder(Sn_Wo, Station.SFCDB);
            WoFromSn_Skuno = R_SN.SKUNO;// R_WO_BASE.SKUNO;
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");//配WoDataloader方法，取WorkOrder對象
            if (WoInput == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050", new string[] { }));

            }
            else
            {
                WorkOrder ObjWoInput = new WorkOrder();
                ObjWoInput = (WorkOrder)WoInput.Value;
                WoInput_Skuno = ObjWoInput.SkuNO;
            }
            if (WoFromSn_Skuno == null || WoInput_Skuno == null)
            {
                //Station.AddMessage("MES00000006", new string[] { "SKUNO" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else if (WoFromSn_Skuno != WoInput_Skuno)
            {
                bool Valid = false;



                if (TCSD.CheckReworkMapping("REWORK_MAPPING", WoInput_Skuno, ((WorkOrder)WoInput.Value).WorkorderNo, Station.SFCDB))
                {

                    Valid = true;
                }

                if (!Valid)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000095", new string[] { ((WorkOrder)WoInput.Value).WorkorderNo, R_SN.SN }));
                }

            }
            else
            {

            }
        }

        /// <summary>
        /// 產品掃描Rework WO Check
        /// 1.	存在workordertype 為Rework的
        /// 2.	要滿足inputqty </ workorderqty 
        /// 3.	要滿足Closed=0
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReworkWOchecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            string Wo = "";
            string wo_type = "";
            double? workorderqty = 0;
            double? inputqty = 0;
            string closed_flag = "";
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");
            if (WoInput == null)
            {
                Station.AddMessage("MES00000050", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                return;
            }
            else
            {
                WorkOrder ObjWoInput = new WorkOrder();
                ObjWoInput = (WorkOrder)WoInput.Value;
                Wo = ObjWoInput.WorkorderNo.ToUpper();
                wo_type = ObjWoInput.WO_TYPE.ToUpper();
                workorderqty = (double?)ObjWoInput.WORKORDER_QTY;
                inputqty = (double?)ObjWoInput.INPUT_QTY;
                closed_flag = ObjWoInput.CLOSED_FLAG;
            }
            if (!wo_type.Contains("REWORK") && !wo_type.Contains("RMA"))
            {
                //Station.AddMessage("MES00000098", new string[] { "REWORK" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000098", new string[] { "REWORK" });
                throw new MESReturnMessage(ErrMessage);
            }
            else if (inputqty >= workorderqty)
            {
                //Station.AddMessage("MES00000099", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000099", new string[] { });
                throw new MESReturnMessage(ErrMessage);
            }
            else if (closed_flag != "0")
            {
                //Station.AddMessage("MES00000100", new string[] { "" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000100", new string[] { "" });
                throw new MESReturnMessage(ErrMessage);
            }
            else
            {
                MESStationSession WoChecked = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (WoChecked == null)
                {
                    WoChecked = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(WoChecked);
                }
                // WoChecked.Value = Wo;
                Station.AddMessage("MES00000101", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        public static void SNInRepairchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Re_sn = Input.Value.ToString();
            string ErrMessage = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }
            T_R_REPAIR_TRANSFER trt = new T_R_REPAIR_TRANSFER(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_TRANSFER> list = trt.GetReSNbysn(Re_sn, Station.SFCDB);
            if (list != null)
            {
                if (list[0].IN_TIME != null && list[0].OUT_TIME == null && list[0].CLOSED_FLAG == "0")
                {
                    Station.AddMessage("MES00000046", new string[] { Re_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000007", new string[] { Re_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                    new string[] { Re_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000007", new string[] { Re_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                new string[] { Re_sn });
                throw new MESReturnMessage(ErrMessage);
            }

        }

        public static void SNOutRepairchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Re_sn = Input.Value.ToString();
            string ErrMessage = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }
            T_R_REPAIR_TRANSFER trt = new T_R_REPAIR_TRANSFER(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_MAIN trm = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_TRANSFER> list = trt.GetReSNbysn(Re_sn, Station.SFCDB);
            if (list != null)
            {
                if (list[0].IN_TIME != null && list[0].OUT_TIME == null && list[0].CLOSED_FLAG == "0")
                {
                    List<R_REPAIR_MAIN> listmain = trm.GetRepairMainBySN(Station.SFCDB, Re_sn);
                    R_REPAIR_MAIN Re_main = listmain.Find(s => s.CLOSED_FLAG == "1" && s.ID == list[0].REPAIR_MAIN_ID);
                    if (Re_main != null)
                    {
                        Station.AddMessage("MES00000046", new string[] { Re_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                    }
                    else
                    {
                        Station.AddMessage("MES00000007", new string[] { Re_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                        new string[] { Re_sn });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    Station.AddMessage("MES00000007", new string[] { Re_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                    new string[] { Re_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000007", new string[] { Re_sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                new string[] { Re_sn });
                throw new MESReturnMessage(ErrMessage);
            }

        }

        /// <summary>
        /// 檢查 SN 所在的測試站位是否存在
        /// 張官軍 2018/01/18
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNCallHWWSChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string SerialNumber = string.Empty;
            string SettingStation = string.Empty;
            T_C_SKU_DETAIL SkuDetail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNumber = ((SN)SNSession.Value).SerialNo;
            var SNobj = (SN)SNSession.Value;


            var SettingStationObj = SkuDetail.GetSkuDetail(Station.StationName, "CHECK_TEST", ((SN)SNSession.Value).SkuNo, Station.SFCDB);
            if (SettingStationObj != null)
            {
                HateEmsData data = new HateEmsData();

                data.MesWebProxy = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProxyIP"];
                data.MesWebProxyPort = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProxyPort"];
                data.UserName = System.Configuration.ConfigurationManager.AppSettings["HWMesWebUserName"];
                data.Factory = System.Configuration.ConfigurationManager.AppSettings["HWMesWebFactory"];
                data.ProcStep = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProcStep"];// "1";
                data.Barcode = SerialNumber;
                data.Operation = System.Configuration.ConfigurationManager.AppSettings["HWMesWebOperation"];//"111";
                data.BarcodeType = System.Configuration.ConfigurationManager.AppSettings["HWMesWebBarcodeType"];// "LOT_ID"; 
                data.Service = System.Configuration.ConfigurationManager.AppSettings["HWMesWebService"]; //"GET_PRODUCT_INFO_EMS_BY_SN";
                data.Language = Convert.ToUInt16(System.Configuration.ConfigurationManager.AppSettings["HWMesWebLanguage"].ToString());//1;

                var result = (hateEmsGetDataServiceOut)HateEmsCaller.EmsService(data);

                SettingStation = SettingStationObj.VALUE.Trim().ToString();

                if (result != null)
                {
                    if (result.operation.ToUpper().Trim().Equals(SettingStation))
                    {
                        Station.AddMessage("MES00000109", new string[] { result.emsOrderId.ToUpper().Trim() },
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                    }
                    else
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000108", new string[] {
                        result.operation.ToUpper().Trim(),SettingStation,result.emsOrderId.ToUpper().Trim() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000107", new string[] { SerialNumber });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000109", new string[] { },
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        public static void SNCallHWWSCheckerByRoute(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string SerialNumber = string.Empty;
            string SettingStation = string.Empty;
            T_C_SKU_DETAIL SkuDetail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);

            List<string> CheckValue = new List<string>();
            var CheckValue1 = Paras.FindAll(t => t.SESSION_TYPE != null && t.SESSION_TYPE.ToUpper() == "CHECKVALUE");
            for (int i = 0; i < CheckValue1.Count; i++)
            {
                CheckValue.Add(CheckValue1[i].VALUE);
                SettingStation += CheckValue1[i].VALUE + ",";
            }

            if (CheckValue.Count == 0)
            {
                CheckValue.Add("TS179");
            }


            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var routes = Paras.FindAll(t => t.SESSION_TYPE != null && t.SESSION_TYPE.ToUpper() == "ROUTENAME");


            SerialNumber = ((SN)SNSession.Value).SerialNo;
            var SNobj = (SN)SNSession.Value;


            var route = routes.Find(t => t.VALUE == SNobj.Route.ROUTE_NAME);

            // var SettingStationObj = SkuDetail.GetSkuDetail(Station.StationName, "CHECK_TEST", ((SN)SNSession.Value).SkuNo, Station.SFCDB);
            if (route != null)
            {
                HateEmsData data = new HateEmsData();

                data.MesWebProxy = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProxyIP"];
                data.MesWebProxyPort = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProxyPort"];
                data.UserName = System.Configuration.ConfigurationManager.AppSettings["HWMesWebUserName"];
                data.Factory = System.Configuration.ConfigurationManager.AppSettings["HWMesWebFactory"];
                data.ProcStep = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProcStep"];// "1";
                data.Barcode = SerialNumber;
                data.Operation = System.Configuration.ConfigurationManager.AppSettings["HWMesWebOperation"];//"111";
                data.BarcodeType = System.Configuration.ConfigurationManager.AppSettings["HWMesWebBarcodeType"];// "LOT_ID"; 
                data.Service = System.Configuration.ConfigurationManager.AppSettings["HWMesWebService"]; //"GET_PRODUCT_INFO_EMS_BY_SN";
                data.Language = Convert.ToUInt16(System.Configuration.ConfigurationManager.AppSettings["HWMesWebLanguage"].ToString());//1;

                var result = (hateEmsGetDataServiceOut)HateEmsCaller.EmsService(data);


                //SettingStation = CheckValue;

                if (result != null)
                {
                    var operation = "";
                    if (result.operation != null)
                    {
                        operation = result.operation.ToUpper().Trim();
                    }
                    if (CheckValue.Contains(operation))//(result.operation.ToUpper().Trim().Equals(SettingStation))
                    {
                        Station.AddMessage("MES00000109", new string[] { result.emsOrderId.ToUpper().Trim() },
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                    }
                    else
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000108", new string[] {
                        result.operation.ToUpper().Trim(),SettingStation,result.emsOrderId.ToUpper().Trim() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000107", new string[] { SerialNumber });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000109", new string[] { },
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        public static void SNCallHWWSCheckerByRoutePlus3_0(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            // SNCallHWWSCheckerByRoute HW升級Plus 3.0後的調用方法
            string ErrMessage = string.Empty;
            string SerialNumber = string.Empty;
            string SettingStation = string.Empty;
            T_C_SKU_DETAIL SkuDetail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);

            List<string> CheckValue = new List<string>();
            var CheckValue1 = Paras.FindAll(t => t.SESSION_TYPE != null && t.SESSION_TYPE.ToUpper() == "CHECKVALUE");
            for (int i = 0; i < CheckValue1.Count; i++)
            {
                CheckValue.Add(CheckValue1[i].VALUE);
                SettingStation += CheckValue1[i].VALUE + ",";
            }

            if (CheckValue.Count == 0)
            {
                CheckValue.Add("TS179");
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var routes = Paras.FindAll(t => t.SESSION_TYPE != null && t.SESSION_TYPE.ToUpper() == "ROUTENAME");
            SerialNumber = ((SN)SNSession.Value).SerialNo;
            var SNobj = (SN)SNSession.Value;
            var route = routes.Find(t => t.VALUE == SNobj.Route.ROUTE_NAME);

            // var SettingStationObj = SkuDetail.GetSkuDetail(Station.StationName, "CHECK_TEST", ((SN)SNSession.Value).SkuNo, Station.SFCDB);
            if (route != null)
            {
                string Url = System.Configuration.ConfigurationManager.AppSettings["HWMesWebServicePlus3"];
                getProductInfoAutoData data = new getProductInfoAutoData
                {
                    siteId = System.Configuration.ConfigurationManager.AppSettings["HWMesWebSiteId"],
                    barcodeType = System.Configuration.ConfigurationManager.AppSettings["HWMesWebBarcodeTypePlus3"],
                    barcode = SerialNumber
                };
                Newtonsoft.Json.Linq.JObject json = HateEmsCaller.PostWebServices(data, Url);
                //SettingStation = CheckValue;
                if (json != null)
                {
                    var operation = json["oper"].ToString().ToUpper().Trim();
                    if (CheckValue.Contains(operation))
                    {
                        Station.AddMessage("MES00000109", new string[] { json["emsOrderId"].ToString().ToUpper().Trim() },
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                    }
                    else
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000108", new string[] {
                        json["oper"].ToString().ToUpper().Trim(),SettingStation,json["emsOrderId"].ToString().ToUpper().Trim() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000107", new string[] { SerialNumber });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000109", new string[] { },
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        public static void InputSNExistChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            R_SN R_Sn = null;
            //modify by LLF 2018-02-06 
            //StrSN = Station.Inputs[1].ToString();
            StrSN = Input.Value.ToString();
            SN SNObj = new SN();
            R_Sn = SNObj.LoadSN(StrSN, Station.SFCDB);

            //modify by ZGJ 2018-03-15
            //下面的判斷錯誤且多餘
            if (R_Sn != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            }

            //if (Paras.Count > 0)
            //{
            //    if (R_Sn == null)
            //    {

            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            //    }
            //}
            //else
            //{
            //    if (R_Sn != null)
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            //    }
            //}
        }
        public static void CheckSnExists(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            R_SN R_Sn = null;
            StrSN = Input.Value.ToString();
            SN SNObj = new SN();
            R_Sn = SNObj.CheckSn(StrSN, Station.SFCDB);
            if (R_Sn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180911024017", new string[] { StrSN }));
            }

            //modify by ZGJ 2018-03-15
            //下面的判斷錯誤且多餘

        }


        /// <summary>
        /// 檢查Panel表中是否存在該SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> paras)
        {
            string StrSn = "";
            R_SN R_Sn = null;
            StrSn = Input.Value.ToString();
            //SN SNobj = new SN();

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == paras[0].SESSION_TYPE && t.SessionKey == paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { paras[0].SESSION_TYPE + paras[0].SESSION_KEY }));
            }
            SN SNobj = (SN)SNSession.Value;
            T_R_SILVER_ROTATION SnDetailTable = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            var aa = SnDetailTable.GetRotationBySNCheckStatus(SNobj.SerialNo, Station.SFCDB);
            R_Sn = SNobj.CheckSnStatus(SNobj.SerialNo, Station.SFCDB);
            if (R_Sn == null || aa != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180911024017", new string[] { StrSn }));
            }
        }
        public static void InputSNExistPanelChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            R_PANEL_SN R_Panel_SN = null;
            string StrSN = Input.Value.ToString();
            SN SNObj = new SN();
            R_Panel_SN = SNObj.LoadPanelBySN(StrSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (R_Panel_SN != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            }
        }

        /// <summary>
        /// 檢查UNLINK工站 SN状态是否为MRB  
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UnlinkSNStatusChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSn = (SN)SNSession.Value;

            if (ObjSn.CurrentStation != "MRB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000076", new string[] { ObjSn.BoxSN }));
            }
        }
        public static void SNRuleChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string SNRuleName = "";
            string StrSN = "";
            bool CheckRuleFlag = false;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var obj = SNSession.Value;
            if (obj is SN)
            {
                StrSN = ((SN)SNSession.Value).SerialNo;
            }
            else if (obj is string)
            {
                StrSN = SNSession.Value.ToString();
            }

            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            SKU SkuObj = (SKU)SKUSession.Value;
            SNRuleName = SkuObj.SnRule.ToString();
            SN SNObj = new SN();
            CheckRuleFlag = SNObj.CheckSNRule(StrSN, SNRuleName, Station.SFCDB, DB_TYPE_ENUM.Oracle);
        }

        public static void HWDSNStockINChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;

            if (ObjSN.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            }

            if (ObjSN.NextStation.IndexOf("JOBFINISH") < 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            }

            if (ObjSN.StockStatus == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000145", new string[] { ObjSN.SerialNo }));
            }
        }
        /// <summary>
        /// Vertiv Stockin 檢查SN狀態是否完工
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void VertivSNStockINChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;

            if (ObjSN.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            }

            //if (ObjSN.NextStation.IndexOf("JOBFINISH") < 0)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            //}

            if (ObjSN.StockStatus == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000145", new string[] { ObjSN.SerialNo }));
            }
        }
        /// <summary>
        /// 產品未完工狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNNoCompleteChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;

            if (ObjSN.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            }
        }
        
        public static void CheckOrtInSample(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;
            List<R_ORT_ALERT> roa = Station.SFCDB.ORM.Queryable<R_ORT_ALERT>().Where(t => t.SN == ObjSN.SerialNo).ToList();
            if (roa.Count == 0)
            {
               throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200831111728", new string[] { ObjSN.SerialNo }));
            }
        }
        public static void CheckBrcmTestTimes(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;
            DateTime? LastOne = null;
            DateTime? LastSecond = null;

            var cs = Station.SFCDB.ORM.Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER>((RS, CS, CSS, CC) => RS.SKUNO == CS.SKUNO && CS.C_SERIES_ID == CSS.ID && CC.ID == CSS.CUSTOMER_ID).
                 Where((RS, CS, CSS, CC) => RS.SN == ObjSN.SerialNo && RS.VALID_FLAG == "1" && CC.CUSTOMER_NAME == "BROADCOM").Select((RS, CS, CSS, CC) => RS).ToList();
            if (cs.Count > 0)
            {
                List<R_TEST_RECORD> roa = Station.SFCDB.ORM.Queryable<R_TEST_RECORD, C_TEMES_STATION_MAPPING, R_SN>
                    ((RT, CT, RS) => RT.TESTATION == CT.MES_STATION && RS.SN == RT.SN).
                    Where((RT, CT, RS) => RT.SN == ObjSN.SerialNo && CT.TE_STATION == Station.StationName
                    && RS.VALID_FLAG == "1" && RT.STATE == "PASS").OrderBy((RT, CT, RS) => RT.EDIT_TIME, SqlSugar.OrderByType.Desc).Select((RT, CT, RS) => RT).ToList();
                if (roa.Count > 1 && roa.Count <= 10)
                {
                    LastOne = roa[0].EDIT_TIME;
                    LastSecond = roa[1].EDIT_TIME;
                    TimeSpan duration = Convert.ToDateTime(LastOne) - Convert.ToDateTime(LastSecond);
                    if (duration.Hours < 10)
                    {
                        //throw new MESReturnMessage($@"最後一筆'{ Station.StationName}'測試PASS時間與上次PASS時間間隔未超10小時，請確認！");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163108", new string[] { Station.StationName }));
                    }
                }
                else if (roa.Count > 6)
                {
                    //throw new MESReturnMessage($@"該SN:'{ObjSN.SerialNo}'的 '{ Station.StationName}'PASS次數超6次，請確認！0");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163215", new string[] { ObjSN.SerialNo, Station.StationName }));
                }
            }
        }
        public static void CheckOrtScanStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;
            string sn = ObjSN.ToString();
            List<R_ORT> roa = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == ObjSN.SerialNo).ToList();

            string StrSql = $@" SELECT
	                                b.VALUE
                                FROM
	                                R_FUNCTION_CONTROL a,
	                                R_FUNCTION_CONTROL_EX b
                                WHERE
	                                a.ID = b.DETAIL_ID
	                                AND a.FUNCTIONNAME = 'ORT_SET'
	                                AND a.CATEGORY = 'ORT_SET'
	                                AND a.VALUE = SUBSTR('{sn}', 1, 3) AND b.VALUE IS NOT NULL";
            DataTable Otd = Station.SFCDB.RunSelect(StrSql).Tables[0];

            var ORTOUT = roa.FindAll(t => "ORTOUT".Equals(t.ORTEVENT));
            if (roa.Count > 0)
            {
                if (ORTOUT.Count == 0)
                {
                    if (roa.FindAll(t => "ORTIN".Equals(t.ORTEVENT)).Count == 0)
                    {
                        //throw new MESReturnMessage($@"ERR: SN未掃描 ORTIN !");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162423"));

                    }
                    if (roa.FindAll(t => "ORT10".Equals(t.ORTEVENT)).Count > 0 && Station.StationName != "ORTOUT")
                    {
                        //throw new MESReturnMessage($@"ERR: SN已在 ORT 工站測滿10天, 請掃描ORTOUT! !");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162514"));

                    }
                    if (Station.StationName == "ORTOUT" && roa.FindAll(t => "ORT10".Equals(t.ORTEVENT)).Count == 0 && Otd.Rows.Count == 0)
                    {
                        //throw new MESReturnMessage($@"ERR: SN在 ORT 工站未測滿10天, 不允許掃描ORTOUT! !");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162533"));

                    }
                    //Check data test ort follow day for sku Ufi .
                    if (Otd.Rows.Count != 0)
                    {
                        if ((roa.Count - 1).ToString() != Otd.Rows[0]["VALUE"].ToString() && Otd.Rows.Count > 0 && Station.StationName == "ORTOUT")
                        {
                            throw new MESReturnMessage($@"ERR: SN在 ORT 工站未測滿{Otd.Rows[0]["VALUE"].ToString()}天, 不允許掃描ORTOUT! ! ");
                        }
                    }
                }
                else
                {
                    if (Station.StationName == "ORTOUT")
                    {
                        //throw new MESReturnMessage($@"ERR: SN已經掃描ORTOUT,不能重複測試ORT! !");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162551"));

                    }
                }

            }
            else
            {
                //throw new MESReturnMessage($@"ERR: 該SN未掃入ORT或已被清出!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162609"));

            }
        }


        /// <summary>
        /// 最後一筆過站記錄為當前工站檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLastStationPassChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            T_R_SN_STATION_DETAIL Table_R_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            StrSN = SNSession.Value.ToString();

            R_Sn_Station_Detail = Table_R_SN_STATION_DETAIL.GetSNLastPassStationDetail(StrSN, Station.SFCDB);

            if (R_Sn_Station_Detail != null)
            {
                if (R_Sn_Station_Detail.STATION_NAME == Station.StationName)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000188", new string[] { }));
                }
            }
        }

        public static void SNLastRITPassChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            T_R_TEST_RECORD Table_R_TEST_RECORD = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
            R_TEST_RECORD R_Sn_Test_Detail = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            StrSN = SNSession.Value.ToString();

            R_Sn_Test_Detail = Table_R_TEST_RECORD.GetSNLastRITPassDetail(StrSN, Station.SFCDB);


            if (R_Sn_Test_Detail == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180921064621", new string[] { }));
            }
        }
        public static void CheckSnTestTime(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrSn = "";
            T_R_SILVER_ROTATION Tabel_R_SILVER_ROTATION = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            R_SILVER_ROTATION R_Sn_Test_Time = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            StrSn = SNSession.Value.ToString();
            R_Sn_Test_Time = Tabel_R_SILVER_ROTATION.GetCheckSnTestTime(StrSn, Station.SFCDB);
            if (R_Sn_Test_Time != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180921072853", new string[] { }));
            }
        }
        public static void CheckSnOrtExist(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Sn = "";
            T_R_LOT_DETAIL Lot_Tabel = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            R_LOT_DETAIL OrtSn = null;
            T_R_TEST_RECORD Test_Tabel = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
            R_TEST_RECORD TestData = null;
            T_R_SN_STATION_DETAIL Station_Tabel = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            R_SN_STATION_DETAIL Station_Detail = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;
            Sn = ObjSN.SerialNo;

            OrtSn = Lot_Tabel.CheckSN(Sn, Station.SFCDB);
            if (OrtSn != null)
            {
                TestData = Test_Tabel.GetSNLastORTPassDetail(Sn, OrtSn.CREATE_DATE.ToString(), Station.SFCDB);
                Station_Detail = Station_Tabel.GetSNORTPassStationDetail(Sn, OrtSn.CREATE_DATE.ToString(), Station.SFCDB);
                if (TestData == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190610104854", new string[] { }));
                }
                if (Station_Detail == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190610144611", new string[] { }));
                }
            }
        }

        public static void CheckSnORTTestTime(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            String StrSn = "";
            T_R_TEST_DETAIL_VERTIV Table_R_TEST_DETAIL_VERTIV = new T_R_TEST_DETAIL_VERTIV(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_TEST_DETAIL_VERTIV r_sn_test_time = null;
            R_TEST_DETAIL_VERTIV SnOrNot = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            StrSn = SNSession.Value.ToString();
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SN ObjSN = (SN)SNSession.Value;
            if (ObjSN.NextStation != "CARTON")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { ObjSN.SerialNo, ObjSN.CurrentStation }));
            }

            SnOrNot = Table_R_TEST_DETAIL_VERTIV.GetTestByOrtSn(Station.SFCDB, StrSn, Station.DisplayName);
            if (SnOrNot == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190604140336", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            r_sn_test_time = Table_R_TEST_DETAIL_VERTIV.GETCheckSnORTTestTime(Station.SFCDB, StrSn, Station.DisplayName);
            if (r_sn_test_time == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190604110249", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (Station.StationName == "ORT")//ORT-FT2不需要測試24H
            {
                r_sn_test_time = Table_R_TEST_DETAIL_VERTIV.CheckORT24HOrNot(Station.SFCDB, StrSn, Station.DisplayName);
                if (r_sn_test_time == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190612113333", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }
        }

        public static void LotDetailSNStatusChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string StrStation = Station.StationName;
            LotNo ObjLot = new LotNo();
            bool MultiStatus = false;
            Row_R_LOT_STATUS GetLotNo;
            T_R_LOT_DETAIL RowLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (SessionSN == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            else
            {
                SN ObjSN = (SN)SessionSN.Value;
                if (!string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    StrStation = Paras[0].VALUE.ToString();
                }
                GetLotNo = RowLotStatus.GetLotBySNForInLot(ObjSN.SerialNo, Station.SFCDB);
                if (GetLotNo != null)
                {
                    MultiStatus = RowLotDetail.CheckLotDetailSNStatus(GetLotNo.ID, StrStation, Station.SFCDB);
                    if (MultiStatus)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000202", new string[] { GetLotNo.LOT_NO }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619102029", new string[] { ObjSN.SerialNo }));
                }
            }
        }

        public static void SNRMAChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            R_SN r_sn = null;
            SN snObj = new SN();
            r_sn = snObj.LoadSN(SNSession.Value.ToString(), Station.SFCDB);
            if (r_sn != null && r_sn.CURRENT_STATION == "RMA")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000237", new string[] { r_sn.BOXSN }));
            }
        }

        /// <summary>
        /// 檢查測試工站及之前的測試站的最後一次測試記錄是否PASS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNTestChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count < 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionByPass = null;
            if (Paras.Count > 1)
            {
                sessionByPass = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            }
            var AllowByPass = true;
            if (sessionByPass == null || sessionByPass.Value == null)
            {
                AllowByPass = true;
            }
            else if (sessionByPass.Value.ToString() == "TRUE")
            {
                AllowByPass = true;
            }
            else
            {
                AllowByPass = false;
            }

            SN snObj = new SN();
            snObj = (SN)sessionSN.Value;
            T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
            t_r_test_record.CheckAllTestBySNStation(snObj.ID, Station.StationName, Station.BU, AllowByPass, sfcdb);
        }

        /// <summary>
        /// 檢查路由之外的抽測工站是否測試OK
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OutStationTestChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count < 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SNsession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNsession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObj = new SN(SNsession.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            //R_SN SerialNo = new R_SN();
            //SerialNo = snObj.LoadSN(snObj.SerialNo, Station.SFCDB);
            if (Paras.Count > 1)
            {
                for (int i = 1; i < Paras.Count; i++)
                {
                    string SampleStation = Paras[i].VALUE.ToString();
                    //判斷SN是否被抽中
                    var SnInSample = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((rls, rld) => rls.LOT_NO == rld.LOT_ID)
                        .Where((rls, rld) => rld.STATUS == "0" && rld.SN == snObj.SerialNo && rls.SAMPLE_STATION == SampleStation).Select((rls, rld) => rld).ToList();
                    //判斷是否有PASS的測試數據
                    var HaveTest = Station.SFCDB.ORM.Queryable<R_TEST_JUNIPER>().Where(r => r.SYSSERIALNO == snObj.SerialNo && r.EVENTNAME == SampleStation && r.STATUS == "PASS" && r.PART_NUMBER == snObj.SkuNo).ToList();

                    if (SnInSample.Count > 0)
                    {
                        if (HaveTest.Count > 0)
                        {
                            Station.SFCDB.ORM.Updateable<R_LOT_DETAIL>().SetColumns(r => new R_LOT_DETAIL
                            {
                                STATUS = "1",
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now
                            }).Where(r => r.SN == snObj.SerialNo && r.WORKORDERNO == snObj.WorkorderNo && r.STATUS == "0").ExecuteCommand();
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210928160151", new string[] { snObj.SerialNo, SampleStation }));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 檢查測試工站之前的測試站的最後一次測試記錄是否PASS,不包括當前站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNTestCheckStationBefore(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;

            try
            {
                T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                SN snObj = new SN(Input.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                #region 檢查當前工站之前的所有測試工站的最後一筆測試記錄是否PASS By Eden 2018/05/16
                List<C_ROUTE_DETAIL> cRouteDetailList = t_c_route_detail.GetTestStationByNameBefor(sfcdb, snObj.RouteID, stationName);
                List<R_TEST_RECORD> td = t_r_test_record.GetTestDataByTimeBefor(sfcdb, snObj.SerialNo, snObj.StartTime ?? DateTime.Now);
                foreach (var item in cRouteDetailList)
                {
                    if (td.FindAll(t => t.MESSTATION == item.STATION_NAME && t.STATE.Equals("PASS")).Count == 0 && item.STATION_NAME != stationName)
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { snObj.SerialNo, item.STATION_NAME }));
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 檢查測試工站的最後一次測試記錄是否FAIL,
        /// TE要求測試工站掃入維修時，當前測試工站的最後一次測試記錄必須是FAIL
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNTestFailChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {
                T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                SN snObj = new SN();
                snObj = (SN)sessionSN.Value;
                bool test = t_r_test_record.CheckTestBySNAndStation(snObj.ID, stationName, sfcdb);
                if (test)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180604155322", new string[] { snObj.SerialNo, stationName }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SNIsExistCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            sn = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
            }

            T_R_SN tr_sn = new T_R_SN(Station.SFCDB, Station.DBType);

            if (tr_sn.CheckSNExists(sn, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { sn }));
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }

        }
        public static void SNLockCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {

                SN snObj = new SN();
                snObj = (SN)sessionSN.Value;
                string sn = snObj.baseSN.SN;
                string strSql = $@" SELECT*FROM r_sn_lock WHERE SN  IN( SELECT {sn}  FROM DUAL
                                                         UNION SELECT value FROM r_sn_kp WHERE sn={sn}
                                                         UNION SELECT value FROM r_sn_kp WHERE sn IN(
                                                         SELECT value FROM r_sn_kp WHERE sn={sn})) AND LOCK_STATUS='1' ";
                DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    //throw new MESReturnMessage($@"ERR: 輸入的SN當階或上階已經被HOLD,請聯繫QE確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162626"));

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SN_COSMETIC_FAILURE_Check(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {

                SN snObj = new SN();
                snObj = (SN)sessionSN.Value;
                string sn = snObj.baseSN.SN;
                var RS = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.VALID_FLAG == "1" && t.STATION_NAME == "COSMETIC-FAILURE").OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                if (RS != null)
                {
                    DateTime Cosmeticdate = (DateTime)RS.EDIT_TIME;
                    List<R_SN_STATION_DETAIL> rsd = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().
                        Where(t => t.SN == sn && t.STATION_NAME == "FQA1" && t.VALID_FLAG == "1" && t.EDIT_TIME < Cosmeticdate).ToList();
                    if (rsd.Count > 0)
                    {
                        Cosmeticdate = Cosmeticdate.AddHours(-8);
                        List<R_TEST_BRCD> rtb = Station.SFCDB.ORM.Queryable<R_TEST_BRCD>().
                            Where(t => t.SYSSERIALNO == sn && t.EVENTNAME == "FQA1" && t.STATUS == "PASS" && t.TESTDATE > Cosmeticdate).ToList();
                        if (rtb.Count == 0)
                        {
                            //throw new MESReturnMessage($@"ERR: 有掃描外觀不良,QE要求管控,請重新測試FQA1!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162646"));

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SNRestoreCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            sn = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                //InputSN_Session.Value = sn;
                //InputSN_Session.ResetInput = Input;
                //InputSN_Session.InputValue = sn;
                sn = InputSN_Session.Value.ToString();
            }
            List<string> CheckSnRoute = new List<string>();
            R_TEST_BRCD rtr = Station.SFCDB.ORM.Queryable<R_TEST_BRCD>().Where(t => t.SYSSERIALNO == sn && t.EVENTNAME.Contains("RESTORE")).ToList().FirstOrDefault();
            CheckSnRoute = Station.SFCDB.ORM.Queryable<R_SN, C_ROUTE_DETAIL>((RS, CRD) => RS.ROUTE_ID == CRD.ROUTE_ID).
                    Where((RS, CRD) => RS.SN == sn && CRD.STATION_NAME == "FINALTEST").Select((RS, CRD) => RS.SN).ToList();
            if (rtr != null && CheckSnRoute.Count > 0)
            {
                List<R_TEST_BRCD> Rtest = Station.SFCDB.ORM.Queryable<R_TEST_BRCD>().Where(t => t.SYSSERIALNO == sn && t.EVENTNAME == "FINALTEST" && t.STATUS == "PASS" && t.TESTDATE > rtr.TESTDATE).ToList();
                if (Rtest.Count == 0)
                {
                    //throw new MESReturnMessage($@"This SSN {sn}有還原過OS,沒有重測FINAL-TEST,請確認! ");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163259", new string[] { sn }));


                }
            }

            //add in session agian by zhb 20200708
            if (InputSN_Session.Value.GetType().Name.ToUpper() == "STRING")
            {
                SN SNObj = new SN(sn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                InputSN_Session.Value = SNObj;
            }
        }
        public static void SNCHECKWWN(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SKU sku = new SKU();
            SKU SkuObj = (SKU)SKUSession.Value;
            SN SNObj = (SN)SNSession.Value;
            string skuno = SkuObj.SkuNo;
            string Sn = SNObj.baseSN.SN;
            List<R_SN_PASS> CheckWwn = Station.SFCDB.ORM.Queryable<R_SN_PASS>().Where(t => t.TYPE == "CHECKWWN" && SqlSugar.SqlFunc.StartsWith(t.WORKORDERNO, skuno)).ToList();
            List<R_SN_PASS> CheckMac = Station.SFCDB.ORM.Queryable<R_SN_PASS>().Where(t => t.TYPE == "CHECKMAC" && SqlSugar.SqlFunc.StartsWith(t.WORKORDERNO, skuno)).ToList();
            if (CheckWwn.Count > 0)
            {
                List<WWN_DATASHARING> WWN = Station.SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.VSSN == Sn && t.WWN == "").ToList();
                if (WWN.Count > 0)
                {
                    throw new MESReturnMessage($@" ERROR:This {Sn} not exists in wwn_datasharing or WWN is null! ");
                }
            }
            if (CheckMac.Count > 0)
            {
                List<WWN_DATASHARING> WWN = Station.SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.VSSN == Sn && t.MAC == "").ToList();
                if (WWN.Count > 0)
                {
                    throw new MESReturnMessage($@" ERROR:This {Sn} not exists in wwn_datasharing or MAC is null! ");
                }
            }

        }
        public static void SNWaitReplaceCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            sn = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
            }
            List<R_SN_REPLACE> RWait = Station.SFCDB.ORM.Queryable<R_SN_REPLACE>().Where(t => t.OLDSN == sn && t.LINKTYPE == "WaitReplace").ToList();
            if (RWait.Count > 0)
            {
                //throw new MESReturnMessage($@" 該{sn}狀態為待替換,請聯繫QE/PE確認! ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163328", new string[] { sn }));

            }
        }
        public static void CHARGE_SAMPLE_StationCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            //這麼寫的話後面的Action獲取到的Session.Value就是String類型,無法直接轉換成SN對象,所以屏蔽此段換一種寫法 Edit By ZHB 20200728
            //string sn;
            //sn = Input.Value.ToString();
            //MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (InputSN_Session == null)
            //{
            //    InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            //    InputSN_Session.Value = sn;
            //    InputSN_Session.ResetInput = Input;
            //    InputSN_Session.InputValue = sn;
            //    Station.StationSession.Add(InputSN_Session);
            //}
            //else
            //{
            //    InputSN_Session.Value = sn;
            //    InputSN_Session.ResetInput = Input;
            //    InputSN_Session.InputValue = sn;
            //}

            //獲取到 SN 對象
            SN SnObject = null;
            string ErrMessage = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SnObject = (SN)SNSession.Value;
            string sn = SnObject.SerialNo;

            List<R_SN_LOG> RWait = Station.SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.LOGTYPE == "CHARGE-SAMPLE" && t.SN == sn && t.FLAG == "Y").ToList();
            if (RWait.Count > 0)
            {
                var DisTest = Station.SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == sn && t.TESTATION == "Dis-charge").OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).First();
                var ReTest = Station.SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == sn && t.TESTATION == "Re-charge").OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).First();
                if (DisTest != null)
                {
                    if (DisTest.STATE != "PASS")
                    {
                        //throw new MESReturnMessage($@" 該{sn}被抽測Dis-charge,但最後一筆測試記錄非PASS，請重新測試! ");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163355", new string[] { sn }));

                    }
                }
                else
                {
                    //throw new MESReturnMessage($@" 該{sn}被抽測Dis-charge,但無測試記錄，請測試! ");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163422", new string[] { sn }));

                }
                if (ReTest != null)
                {
                    if (ReTest.STATE != "PASS")
                    {
                        //throw new MESReturnMessage($@" 該{sn}被抽測Re-charge,但最後一筆測試記錄非PASS，請重新測試! ");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163443", new string[] { sn }));

                    }
                }
                else
                {
                    //throw new MESReturnMessage($@" 該{sn}被抽測Re-charge,但無測試記錄，請測試! ");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163514", new string[] { sn }));

                }
            }



        }


        /// <summary>
        /// 檢查當前Pack狀態(包裝里SN是否狀態一致),當前工站是否待OBA
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackSnStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            var checkStation = "";
            try
            {
                checkStation = Paras[1].VALUE;
            }
            catch
            {
                checkStation = Station.StationName;
            }


            string Pack_NO = PackNoSession.Value.ToString();
            var sfcdb = Station.SFCDB;
            var pack = sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == Pack_NO).First();
            if (pack == null)
            {
                throw new Exception($@"'{Pack_NO}' not exist");
            }

            if (pack.PACK_TYPE == "PALLET")
            {

                T_R_SN_PACKING T_RSnPacking = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);


                if (T_RSnPacking.CheckPackCurrentStatus(Station.SFCDB, checkStation, PackNoSession.Value.ToString()))
                    throw new Exception(PackNoSession.Value.ToString() + " Pallet already passed " + Paras[1].VALUE + " Station");
                else if (!T_RSnPacking.CheckPackSnStatus(Station.SFCDB, checkStation, PackNoSession.Value.ToString()))
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528095410", new string[] { PackNoSession.Value.ToString(), Paras[1].VALUE }));
            }
            else
            {
                var wipstations = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING>((s, p) => s.ID == p.SN_ID)
                    .Where((s, p) => p.PACK_ID == pack.ID)
                    .Select((s, p) => s.NEXT_STATION).Distinct().ToList();
                if (wipstations.Count == 1)
                {
                    if (wipstations[0] != checkStation)
                    {
                        throw new Exception($@"The unions wip in {wipstations[0]}");
                    }
                    else
                    {
                        //throw new Exception($@"Not All unions wip in {checkStation}");
                    }
                }
                else
                {
                    throw new Exception($@"Not All unions wip in {checkStation}");
                }


            }
        }

        public static void CheckPalletPartno(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionPackno = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackno == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            string strSn = sessionSn.Value.ToString();
            string strPackno = sessionPackno.Value.ToString();
            string strSql = $@" SELECT*FROM r_sn WHERE sn='{strSn}' AND VALID_FLAG='1' AND (skuno LIKE '%-100NES' OR skuno LIKE '%-10000S')";
            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            List<string> CheckPalletSn = new List<string>();
            if (dt.Rows.Count > 0)
            {
                CheckPalletSn = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING>((RP, RA, RS) => RP.ID == RA.PARENT_PACK_ID && RA.ID == RS.PACK_ID).
                  Where((RP, RA, RS) => RP.PACK_NO == strPackno.ToString()).Select((RP, RA, RS) => RP.PACK_NO).ToList();
                if (CheckPalletSn.Count == 0)
                {
                    UIInputData I = new UIInputData() { Timeout = 50000, IconType = IconType.None, Message = "Scan", Tittle = "棧板料號確認", Type = UIInputType.String, Name = "FIXTURENO", ErrMessage = "Cancel Check" };
                    I.OutInputs.Add(new DisplayOutPut() { Name = "STATION", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = "請確認出貨計劃中的出貨地選擇棧板料號，出美國使用003-001，非美國使用004-002" });
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);
                    string empNO = ret.ToString();
                    List<C_USER> U = Station.SFCDB.ORM.Queryable<C_USER>().Where(t => t.EMP_NO == empNO).ToList();
                    if (U.Count == 0)
                    {
                        //throw new MESReturnMessage($@"'確認失敗，請重新確認！ ");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162709"));

                    }
                }
            }
        }

        public static void CheckWoPalletPartno(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionPackno = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackno == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            SN snObj = new SN();
            snObj = (SN)sessionSn.Value;
            string strPackno = sessionPackno.Value.ToString();
            string strSn = snObj.baseSN.SN;
            string strSku = snObj.baseSN.SKUNO;
            string strWo = snObj.baseSN.WORKORDERNO;
            string strSql = $@"select*From c_sku where c_series_id in(select id From c_series where series_name='D-LINK ODM') and skuno='{strSku}'";
            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                List<R_FAI> RF = Station.SFCDB.ORM.Queryable<R_FAI>().Where(t => t.FAITYPE == "WORKORDERNO" && t.WORKORDERNO == strPackno).ToList();
                if (RF.Count == 0)
                {
                    var PalletPartno = Station.SFCDB.ORM.Queryable<R_WO_ITEM>().Where(t => t.AUFNR == strWo && (t.MATNR.Contains("BPALLET") || t.MATNR.Contains("3B1501400")) && t.PARTS != "0").OrderBy(t => t.MATNR).ToList().FirstOrDefault();
                    string strPartno = PalletPartno.MATNR;
                    UIInputData I = new UIInputData() { Timeout = 5000, IconType = IconType.None, Message = "Scan", Tittle = "棧板料號確認", Type = UIInputType.String, Name = "PACKNO", ErrMessage = "Cancel Check" };
                    I.OutInputs.Add(new DisplayOutPut() { Name = "STATION", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = "工單棧板料號:" + strPartno + "請掃描棧板物料SN" });
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);
                    if (strPartno != ret.ToString())
                    {
                        //throw new MESReturnMessage($@"'掃描內容錯誤，請重新確認！ ");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162727"));
                    }
                }
            }
        }

        public static void CheckMacInPackout(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }


            SN snObj = new SN();
            snObj = (SN)sessionSn.Value;

            string strSn = snObj.baseSN.SN;
            string strSku = snObj.baseSN.SKUNO;

            List<R_F_CONTROL> rfc = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.VALUE == strSku && t.FUNCTIONNAME == "CHECKMAC").ToList();
            if (rfc.Count > 0)
            {
                var PalletPartno = Station.SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.MAC_BLOCK_SIZE != 0 && (t.CSSN == strSn || t.VSSN == strSn)).OrderBy(t => t.MAC).ToList().FirstOrDefault();
                string strMac = PalletPartno.MAC;
                UIInputData I = new UIInputData() { Timeout = 5000, IconType = IconType.None, Message = "Scan", Tittle = "CHECKMAC", Type = UIInputType.String, Name = "MAC", ErrMessage = "Cancel Check" };
                I.OutInputs.Add(new DisplayOutPut() { Name = "STATION", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = "請掃描MAC Address:" });
                var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);
                if (strMac != ret.ToString())
                {
                    //throw new MESReturnMessage($@"'Fail:掃描內容錯誤，請重新確認！ ");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162727"));
                }

            }
        }
        public static void CheckSNPack(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            //if (Paras.Count != 2)
            //{
            //    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            //}

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionPackno = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackno == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            var SameWo = Paras.Find(t => t.SESSION_TYPE == "SAMEWO");

            SN snObj = new SN();
            snObj = (SN)sessionSn.Value;
            string strPackno = ((MESStation.Packing.PalletBase)(sessionPackno.Value)).DATA.PACK_NO;
            string strSn = snObj.baseSN.SN;
            string strSku = snObj.baseSN.SKUNO;

            string strSql = $@"select*From c_sku where c_series_id in(select id From c_series where series_name like 'NETGEAR%') 
                              and skuno='{strSku}'and not exists (select*From r_rma_bonepile where sn='{strSn}')";
            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var aa1 = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, R_WO_BASE>
                    ((A, B, C, D, E) => A.ID == B.PARENT_PACK_ID && B.ID == C.PACK_ID && C.SN_ID == D.ID && D.WORKORDERNO == E.WORKORDERNO).
                    Where((A, B, C, D, E) => A.PACK_NO == strPackno).OrderBy((A, B, C, D, E) => E.PRODUCTION_TYPE)
                    .Select((A, B, C, D, E) => new { D, E }).ToList();
                var aa = aa1.FirstOrDefault();
                string PRODUCTION_TYPE = aa.E.PRODUCTION_TYPE;
                string r_sn = aa.D.SN;
                var bb = Station.SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((rs, web) => rs.WORKORDERNO == web.WORKORDERNO).Where((rs, web) => rs.SN == strSn && web.PRODUCTION_TYPE == PRODUCTION_TYPE).ToList();
                if (bb == null)
                {
                    //throw new MESReturnMessage($@"'NPI產品不能與GA產品混包! ");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162746"));
                }
                string subSql = $@" select distinct substr(sn,0,4) From (select d.sn From r_packing a, r_packing b,r_sn_packing c,r_sn d
                                 where a.id=b.parent_pack_id and b.id=c.pack_id and c.sn_id=d.id and a.pack_no='{strPackno}'
                                 union select sn from r_sn where sn='{strSn}')";
                DataTable dtsub = Station.SFCDB.RunSelect(subSql).Tables[0];
                if (dtsub.Rows.Count > 1)
                {
                    //throw new MESReturnMessage($@"'NETGEAR產品前綴不一樣不能混包!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162839"));
                }

            }
            if (SameWo != null)
            {
                var aa2 = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, R_WO_BASE>
               ((A, B, C, D, E) => A.ID == B.PARENT_PACK_ID && B.ID == C.PACK_ID && C.SN_ID == D.ID && D.WORKORDERNO == E.WORKORDERNO).
               Where((A, B, C, D, E) => A.PACK_NO == strPackno)
               .Select((A, B, C, D, E) => E.WORKORDERNO).Distinct().ToList();
                for (int i = 0; i < aa2.Count; i++)
                {
                    if (aa2[i] != snObj.baseSN.WORKORDERNO)
                    {
                        throw new MESReturnMessage($@"Can't Mix diff WO union");
                    }
                }
            }



        }
        public static void CheckAssyTime(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            //MESStationSession sessionPackno = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            //if (sessionPackno == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            //}

            SN snObj = new SN();
            snObj = (SN)sessionSn.Value;
            //string strPackno = sessionPackno.Value.ToString();
            string strSn = snObj.baseSN.SN;
            string strRouteid = snObj.baseSN.ROUTE_ID;

            var rfc = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().
                Where(t => t.VALUE == strRouteid && t.FUNCTIONNAME == "CHECKTIME").ToList().FirstOrDefault();
            if (rfc != null)
            {
                string strstation = rfc.EXTVAL;
                string strsql = $@"select * from R_SN_STATION_DETAIL where sn='{strSn}' and station_name like 'MC%'
                                and valid_flag='1' and edit_time>sysdate-1/2";
                DataTable dt = Station.SFCDB.RunSelect(strsql).Tables[0];
                string strtest = $@"select*From R_SN_STATION_DETAIL where sn='{strSn}' and station_name like 'MC%'
                                    and valid_flag='1' and edit_time<(select max(testdate) From r_test_brcd 
                                    where sysserialno='{strSn}' and eventname='{strstation}' and status='PASS' )";
                DataTable dttest = Station.SFCDB.RunSelect(strtest).Tables[0];
                if (dt.Rows.Count == 0 && dttest.Rows.Count == 0)
                {
                    //throw new MESReturnMessage($@"Err: 距離上次組裝 MC已經過了12小時, 請重測{strstation}工站 - 01!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163539", new string[] { strstation }));

                }

            }
        }
        public static void CheckDLinkSn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            //MESStationSession sessionPackno = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            //if (sessionPackno == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            //}

            SN snObj = new SN();
            snObj = (SN)sessionSn.Value;
            //string strPackno = sessionPackno.Value.ToString();
            string strSn = snObj.baseSN.SN;
            string strSku = snObj.baseSN.SKUNO;
            string strRouteid = snObj.baseSN.ROUTE_ID;
            string strWo = snObj.baseSN.WORKORDERNO;

            string strSql = $@"select*From c_sku where c_series_id in(select id From c_series where series_name ='D-LINK ODM') 
                              and skuno='{strSku}'";
            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0 && strRouteid.Contains("CTO") == true && strWo.PadLeft(6) != "002299")
            {
                string strsqlSn_min = $@"select*From r_Sn where workorderno='{strWo}' 
                                    and PACKED_FLAG='0' and VALID_FLAG='1' and STARTED_FLAG='1' and SN>'{strSn}' order by SN";
                DataTable dtSn_min = Station.SFCDB.RunSelect(strsqlSn_min).Tables[0];
                if (dtSn_min.Rows.Count > 0)
                {
                    //throw new MESReturnMessage($@" D-LINK 機種，請掃最小SN:{dtSn_min.Rows[0]["SN"].ToString()} ");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163608", new string[] { dtSn_min.Rows[0]["SN"].ToString() }));

                }

            }
            else
            {
                string strSqlFqa = $@"selecT*From R_SN_STATION_DETAIL where sn='{strSn}' and station_name='FQA1'
                                    and valid_flag='1'and edit_time<(select max(edit_time) From R_SN_STATION_DETAIL where sn='{strSn}' and station_name='COSMETIC-FAILURE'
                                    and valid_flag='1') ";
                DataTable dtfqa = Station.SFCDB.RunSelect(strSqlFqa).Tables[0];
                if (dtfqa.Rows.Count > 0)
                {
                    string strSqlTest = $@"select*From r_test_brcd where sysserialno='{strSn}' and eventname='FQA1' and status='PASS'and testdate>(select max(edit_time) From R_SN_STATION_DETAIL where sn='{strSn}' and station_name='COSMETIC-FAILURE'
                                    and valid_flag='1') ";
                    DataTable DtTest = Station.SFCDB.RunSelect(strSqlTest).Tables[0];
                    if (DtTest.Rows.Count == 0)
                    {
                        //throw new MESReturnMessage($@"ERR: 有掃描外觀不良,QE要求管控,請重新測試FQA1 ");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163643"));

                    }

                }
            }
        }




        /// <summary>
        /// 檢查SN對象集合中是否已綁定Keypart
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSNObjectListIsLink(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionInputString = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionInputString == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            if (sessionInputString.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionInputType = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionInputType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionInputType.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                string inputType = sessionInputType.Value.ToString();
                string inputString = sessionInputString.Value.ToString();
                List<R_SN> snList = new List<R_SN>();
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                if (inputType.Equals("SN"))
                {
                    T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                    snList.Add(t_r_sn.LoadSN(inputString, Station.SFCDB));
                }
                else if (inputType.Equals("PANEL"))
                {
                    T_R_PANEL_SN t_r_panel_sn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                    snList = t_r_panel_sn.GetValidSnByPanel(inputString, Station.SFCDB);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259"));
                }
                foreach (R_SN r_sn in snList)
                {
                    if (t_r_sn_kp.CheckLinkBySNID(r_sn.ID, Station.SFCDB))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094344", new string[] { r_sn.SN }));
                    }
                }
                Station.AddMessage("MES00000001", new string[] { sessionInputString.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查SN是否在該批次內
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSNInLot(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_SN rRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> rSnList = rRSn.GetSnByLotNoWithOba(Dis_LotNo.Value.ToString(), Station.SFCDB);
            //{0}內Sn{1}下一站為{2};
            if (rSnList.FindAll(t => !t.NEXT_STATION.Equals("OBA") && !t.NEXT_STATION.Equals("OBA2")).Count > 0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529165023", new string[] { Dis_LotNo.Value.ToString(), rSnList.Find(t => !t.NEXT_STATION.Equals("OBA")).SN, rSnList.Find(t => !t.NEXT_STATION.Equals("OBA")).NEXT_STATION }));
            //SN:{0}不在LOT:{1}內
            if (rSnList.FindAll(t => t.SN.Equals(snSession.Value.ToString())).Count == 0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529165225", new string[] { snSession.Value.ToString(), Dis_LotNo.Value.ToString() }));
        }


        /// <summary>
        /// 檢查當前SN狀態是否已抽檢過;
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnStatusInOba(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_LOT_DETAIL tRLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            List<R_LOT_DETAIL> rLotDetail = tRLotDetail.GetLotDetailByLotNo(Dis_LotNo.Value.ToString(), Station.SFCDB);
            //Sn{0}已經抽檢過,狀態為{1}請檢查!
            if (rLotDetail.FindAll(t => t.SN.Equals(snSession.Value.ToString())).Count > 0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529172003", new string[] { rLotDetail.Find(t => t.SN.Equals(snSession.Value.ToString())).SN, rLotDetail.Find(t => t.SN.Equals(snSession.Value.ToString())).STATUS.Equals("1") ? "PASS" : "FAIL" }));
        }

        public static void CheckSNInFQCLot(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionLot = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionLot == null || sessionLot.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            R_LOT_STATUS objLot = (R_LOT_STATUS)sessionLot.Value;
            SN objSN = (SN)sessionSN.Value;
            T_R_LOT_DETAIL t_r_lot_detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            if (!t_r_lot_detail.CheckSNInLot(objLot.ID, objSN.SerialNo, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619102422", new string[] { objSN.SerialNo, objLot.LOT_NO }));
            }
        }

        /// <summary>
        /// 檢查SN對象NextStation是否等於當前工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNNextStationChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObject = (SN)sessionSN.Value;
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            if (!t_c_route_detail.StationInRoute(snObject.RouteID, Station.StationName, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180621172210", new string[] { Station.StationName, snObject.SerialNo }));
            }
            if (!snObject.NextStation.Equals(Station.StationName))
            {
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000136", new string[] { snObject.NextStation }));
                #region 20191209 By Rito Add Route有跳站的邏輯 原本只有上面一行代碼
                //先獲取SN對應RouteID
                string VarRouteID = snObject.RouteID;
                //再獲取SN當前工站
                string VarCurrentStation = snObject.CurrentStation;
                List<C_ROUTE_DETAIL> RouteDetailByDirectLinkID = null;
                //根據SN對應RouteID和SN當前工站判斷是否有跳站,如果沒有則報錯
                RouteDetailByDirectLinkID = t_c_route_detail.GetRouteDetailByDirectLinkID(VarRouteID, VarCurrentStation, Station.SFCDB);
                if (RouteDetailByDirectLinkID != null)
                {
                    //RouteDetailByDirectLinkID[0].STATION_NAME
                    C_ROUTE_DETAIL nextStation = RouteDetailByDirectLinkID.Find(t => t.STATION_NAME == Station.StationName);
                    if (nextStation == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000136", new string[] { snObject.NextStation }));
                    }
                    else
                    {
                        //當判斷到是跳站後,更新r_sn的CurrentStation和NextStation,目的是不變更updatestatus的邏輯
                        T_R_SN RSN = new T_R_SN(Station.SFCDB, Station.DBType);
                        int result = RSN.TiaoZhanUpdateCurrentNextStation(snObject.ID, snObject.NextStation, Station.StationName, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + snObject.SerialNo, "UPDATE" }));
                        }
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000136", new string[] { snObject.NextStation }));
                }
                #endregion
            }
        }

        /// <summary>
        /// 檢查SN當前工站的上一個工站是否有過站記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLastStationDetailChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var sessionObj = snSession.Value;
            SN snObj = null;
            R_SN r_sn = null;
            string sn;
            bool isPass;
            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            if (sessionObj is string)
            {
                sn = sessionObj.ToString();
            }
            else if (typeof(SN) == sessionObj.GetType())
            {
                snObj = (SN)sessionObj;
                sn = snObj.SerialNo;
            }
            else if (typeof(R_SN) == sessionObj.GetType())
            {
                r_sn = (R_SN)sessionObj;
                sn = r_sn.SN;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { sessionObj.ToString() }));
            }
            isPass = t_r_sn_station_detail.TheLastStationIsPass(sn, Station.StationName, Station.SFCDB);
            if (!isPass)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180810135622", new string[] { sn, Station.StationName }));
            }
        }

        /// <summary>
        /// 檢查SN是否有寫過站記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WriteIntoDetailChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var sessionObj = snSession.Value;
            SN snObj = null;
            R_SN r_sn = null;
            string sn;
            bool isPass;
            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            if (sessionObj is string)
            {
                sn = sessionObj.ToString();
            }
            else if (typeof(SN) == sessionObj.GetType())
            {
                snObj = (SN)sessionObj;
                sn = snObj.SerialNo;
            }
            else if (typeof(R_SN) == sessionObj.GetType())
            {
                r_sn = (R_SN)sessionObj;
                sn = r_sn.SN;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { sessionObj.ToString() }));
            }
            isPass = t_r_sn_station_detail.HadWriteIntoDetail(sn, Station.StationName, Station.SFCDB);
            if (!isPass)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180813114004", new string[] { sn, Station.StationName }));
            }
        }

        /// <summary>
        /// 處理SN狀態/記錄過站記錄/統計良率 for TCQS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckTCQSTest(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_TCQS_YIELD_RATE_DETAIL TRTCQS = new T_R_TCQS_YIELD_RATE_DETAIL(Station.SFCDB, Station.DBType);
            R_TCQS_YIELD_RATE_DETAIL RTYRD = new R_TCQS_YIELD_RATE_DETAIL();
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            //獲取DB時間,所有數據更新使用同一時間
            DateTime DT = Station.GetDBDateTime();

            if (Paras.Count != 4)
            {
                //參數不正確：配置表中参数不够，应该为 {0} 个，实际只有 {1} 个！
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                //无法获取到 {0} 的数据，请检查工站配置！
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SnObject = (SN)SNSession.Value;

            //STATUS,方便過站處理/寫良率和UPH使用
            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StatusSession == null)
            {
                //如果沒有，則創建一個該工站的StatusSession,且SN狀態默認為該Action中設定的狀態Value = Paras[1].VALUE
                StatusSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[1].VALUE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
            }
            //如果該工站沒有設定默認狀態，則默認為PASS
            if (StatusSession.Value == null ||
                (StatusSession.Value != null && StatusSession.Value.ToString() == ""))
            {
                StatusSession.Value = "PASS";
            }

            //Device:站點名稱
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else //如果站點名稱不存在,則默認為工站名稱
            {
                DeviceName = Station.StationName;
            }

            //TCQSRecord:TCQS良率統計記錄
            MESStationSession TCQSSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TCQSSession == null)
            {
                TCQSSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, InputValue = Input.Value.ToString() };
                Station.StationSession.Add(TCQSSession);
            }


            //處理SN狀態/記錄過站記錄/統計良率
            try
            {
                RTYRD = TRTCQS.CheckTCQSTest(SnObject.SerialNo, SnObject.SkuNo, SnObject.WorkorderNo, Station.Line, Station.StationName, DeviceName, Station.BU, StatusSession.Value.ToString(), Station.LoginUser.EMP_NO, DT, Station.SFCDB);
                TCQSSession.Value = RTYRD;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check if the number of replacements exceeds the specified number of times
        /// 檢查替換次數是否有超過指定次數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NumberOfReplacementsChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var sessionObj = snSession.Value;
            int number = Convert.ToInt32(Paras[1].VALUE.ToString());
            SN snObj = null;
            R_SN r_sn = null;
            string sn;
            bool isExceeds;
            T_R_REPLACE_SN t_r_replace_sn = new T_R_REPLACE_SN(Station.SFCDB, Station.DBType);
            if (sessionObj is string)
            {
                sn = sessionObj.ToString();
            }
            else if (typeof(SN) == sessionObj.GetType())
            {
                snObj = (SN)sessionObj;
                sn = snObj.SerialNo;
            }
            else if (typeof(R_SN) == sessionObj.GetType())
            {
                r_sn = (R_SN)sessionObj;
                sn = r_sn.SN;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { sessionObj.ToString() }));
            }

            isExceeds = t_r_replace_sn.IsExceeds(sn, number, Station.SFCDB);
            int replaceCount = t_r_replace_sn.GetReplaceCount(sn, Station.SFCDB);
            if (!isExceeds)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180917114539", new string[] { sn, number.ToString(), replaceCount.ToString() }));
            }
        }

        /// <summary>
        /// 檢查是否存在 DOM 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNDomRecordChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_TEST_RECORD Recorder = new T_R_TEST_RECORD(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            string SerialNo = string.Empty;

            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }


            if (SnSession.Value is SN)
            {
                SerialNo = ((SN)SnSession.Value).SerialNo;
            }

            T_C_ROUTE_DETAIL routedetail = new T_C_ROUTE_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);


            if (routedetail.CheckPaste(((SN)SnSession.Value).RouteID, Station.SFCDB))
            {
                if (!Recorder.CheckDom(SerialNo, Station.SFCDB))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180922082635", new string[] { SerialNo }));
                }
            }
        }

        /// <summary>
        /// 包裝之前檢查最後一個測試站的最後一筆記錄必須要 PASS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLastTestRecordChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            T_C_STATION TCS = new T_C_STATION(Station.SFCDB, Station.DBType);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_TEST_RECORD TRTR = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL TRSSD = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage(""));
            }

            SN sn = null;
            if (SnSession.Value is SN)
            {
                sn = (SN)SnSession.Value;
            }
            else
            {
                sn = new SN();
                sn.Load(SnSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            List<C_STATION> TestStations = TCS.GetTestStations(Station.SFCDB);
            List<C_ROUTE_DETAIL> RouteDetails = TCRD.GetByRouteIdOrderBySeqDesc(sn.RouteID, Station.SFCDB);
            foreach (C_ROUTE_DETAIL RD in RouteDetails)
            {
                if (TestStations.Any(t => t.STATION_NAME == RD.STATION_NAME))
                {
                    //拿到 SN 對應站位的過站記錄，判斷過站記錄的 DEVICE_NAME 是不是等於 WebClient，如果是的話，表示這條記錄是員工在客戶端手動過站的，如果不是，DEVICE_NAME 應該
                    //是真正測試的機台名，不會是 WebClient
                    R_SN_STATION_DETAIL RSnStationDetail = TRSSD.GetDetailBySnAndStation(sn.SerialNo, RD.STATION_NAME, Station.SFCDB);
                    if (RSnStationDetail == null || (RSnStationDetail != null && !RSnStationDetail.DEVICE_NAME.Equals("WebClient")))
                    {
                        R_TEST_RECORD TestRecord = TRTR.GetLastTestRecord(sn.SerialNo, RD.STATION_NAME, Station.SFCDB);
                        if (TestRecord == null || (TestRecord != null && TestRecord.STATE.ToUpper() != "PASS"))
                        {
                            R_PANEL_SN Panel = TRPS.GetPanelBySn(sn.SerialNo, Station.SFCDB);
                            if (Panel == null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { sn.SerialNo, RD.STATION_NAME }));
                            }
                            else
                            {
                                TestRecord = TRTR.GetLastTestRecord(Panel.PANEL, RD.STATION_NAME, Station.SFCDB);
                                if (TestRecord == null || (TestRecord != null && TestRecord.STATE.ToUpper() != "PASS"))
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { sn.SerialNo, RD.STATION_NAME }));
                                }
                            }

                        }
                    }
                    break;
                }
            }


        }

        /// <summary>
        /// 檢查陪測SN狀態
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNOutRotationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN snObj = (SN)snSession.Value;
            Station.SFCDB.ThrowSqlExeception = true;
            T_R_SILVER_ROTATION t_r_silver_rotation = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            T_R_SILVER_ROTATION_DETAIL t_r_silver_rotation_detail = new T_R_SILVER_ROTATION_DETAIL(Station.SFCDB, Station.DBType);
            R_SILVER_ROTATION rotation = t_r_silver_rotation.GetRotationBySN(snObj.SerialNo, Station.SFCDB);
            if (rotation == null)
            {
                //不是陪測SN，請先掃入 
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180925082506", new string[] { snObj.SerialNo }));
            }
            if (rotation.STATUS == "1")
            {
                //已經掃出 
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180926075538", new string[] { snObj.SerialNo }));
            }
            if (t_r_silver_rotation_detail.CSNIsNotEndRotation(snObj.SerialNo, Station.SFCDB))
            {
                //還在陪測中 
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180925082833", new string[] { snObj.SerialNo }));
            }
        }

        public static void SNOutWashPcbChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string P_sn = Input.Value.ToString();
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //SN snObj = (SN)snSession.Value;
            Station.SFCDB.ThrowSqlExeception = true;
            T_R_IO_HEAD t_R_IO_HEAD = new T_R_IO_HEAD(Station.SFCDB, Station.DBType);
            R_IO_HEAD WashPcb = t_R_IO_HEAD.GetWashPsbBySN(P_sn, Station.SFCDB);
            if (Station.DisplayName == "WASHPCB_OUT")
            {
                if (WashPcb == null)
                {
                    //未掃入洗版
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181122163231", new string[] { P_sn }));
                }
                if (WashPcb.IOSTATUS == "1")
                {
                    //已經掃出
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181122163438", new string[] { P_sn }));
                }
            }
            else if (Station.DisplayName == "WASHPCB_IN")
            {
                if (WashPcb != null && WashPcb.IOSTATUS == "0")
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181122163642", new string[] { Input.Value.ToString() }));
                }
            }
            else
            {

                if (WashPcb != null && WashPcb.IOSTATUS == "0")
                {
                    //未掃出洗版
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181122163642", new string[] { Input.Value.ToString() }));
                }
            }
        }



        /// <summary>
        /// check next station is equal the paras value
        /// 檢查SN的Nextstation是否等於傳入的值
        /// 傳入一個 SN 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNNextStationValueChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObject = (SN)sessionSN.Value;
            for (int i = 1; i < Paras.Count; i++)
            {
                if (snObject.NextStation.ToUpper() == Paras[i].VALUE.Trim().ToUpper())
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154006", new string[] { snObject.SerialNo, snObject.NextStation }));
                }
            }
        }

        /// <summary>
        /// SN status flag checker
        /// 檢查指定SN Flag 
        /// 傳入一個 SN 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnStatusFlagChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            string statusFlag = string.Empty;
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN sn = (SN)SNSession.Value;
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

        /// <summary>
        /// List SN status flag checker
        /// 檢查指定SN Flag 
        /// 傳入一個 SN 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ListSnStatusFlagChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            string statusFlag = string.Empty;
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            List<R_SN> snList = new List<R_SN>();
            string lstSN = SNSession.Value.ToString();
            lstSN = $@"'{lstSN.Replace("\n", "',\n'")}'";
            if (lstSN.Length == 0)
            {
                throw new MESReturnMessage("Please Input list SN!");
            }
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            snList = t_r_sn.GetSnListByListSN(lstSN, Station.SFCDB);
            for (int i = 1; i < Paras.Count; i++)
            {
                foreach (R_SN lstSn in snList)
                {
                    statusFlag = Paras[i].VALUE.Trim().ToUpper();
                    switch (statusFlag)
                    {
                        case "PACKED_FLAG":
                            //已經包裝
                            if (lstSn.PACKED_FLAG != null && lstSn.PACKED_FLAG.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005155848", new string[] { lstSn.SN }));
                            }
                            break;
                        case "COMPLETED_FLAG":
                            //已經完工
                            if (lstSn.COMPLETED_FLAG != null && lstSn.COMPLETED_FLAG.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160344", new string[] { lstSn.SN }));
                            }
                            break;
                        case "SHIPPED_FLAG":
                            //已經出貨
                            if (lstSn.SHIPPED_FLAG != null && lstSn.SHIPPED_FLAG.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { lstSn.SN }));
                            }
                            break;
                        case "REPAIR_FAILED_FLAG":
                            //待維修
                            if (lstSn.REPAIR_FAILED_FLAG != null && lstSn.REPAIR_FAILED_FLAG.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160551", new string[] { lstSn.SN }));
                            }
                            break;
                        case "SCRAPED_FLAG":
                            //已報廢
                            if (lstSn.SCRAPED_FLAG != null && lstSn.SCRAPED_FLAG.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160653", new string[] { lstSn.SN }));
                            }
                            break;
                        case "VALID_FLAG":
                            //無效的
                            if (lstSn.VALID_FLAG != null && lstSn.VALID_FLAG.Equals("0"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154143", new string[] { lstSn.SN }));
                            }
                            break;
                        case "STOCK_STATUS":
                            //已入庫
                            if (lstSn.STOCK_STATUS != null && lstSn.STOCK_STATUS.Equals("1"))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005161120", new string[] { lstSn.SN }));
                            }
                            break;
                        default:
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259"));
                    }
                }
            }

        }


        /// <summary>
        /// BPD SILOADING SN中的機種版本與 工單中KEYPARTS 的機種版本是否一致
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNKeypartChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_C_KEYPART TCK = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_SKU TCS = new T_C_SKU(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SKU_ROUTE TRSR = new T_R_SKU_ROUTE(Station.SFCDB, Station.DBType);
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            string Sn = string.Empty;
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (SnSession.Value is SN)
            {
                Sn = ((SN)SnSession.Value).SerialNo;
            }
            else
            {
                Sn = SnSession.Value.ToString();
            }
            WorkOrder WO = (WorkOrder)WoSession.Value;
            List<C_KEYPART> Keypart = TCK.GetKeyPartBywo(WO.WorkorderNo, Station.SFCDB);
            C_SKU Sku = TCS.GetCSKUBySn(Sn, Station.SFCDB);

            //檢查產品狀態
            R_SN RSN = TRS.GetSN(Sn, Station.SFCDB);
            if (RSN.COMPLETED_FLAG != "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181120095746", new string[] { Sn, "Completed" }));
            }
            if (RSN.REPAIR_FAILED_FLAG == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160551", new string[] { Sn }));
            }
            if (RSN.SCRAPED_FLAG == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180906161840", new string[] { Sn, "Scrap" }));
            }

            //檢查是否配置 轉換對應關係
            bool Valid = Keypart.Any(t => t.PART_NO == Sku.SKUNO && t.PART_NO_VER == Sku.VERSION && t.STATION_NAME == Station.StationName);
            if (!Valid)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141356", new string[] { WO.SkuNO, Sku.SKUNO }));
            }
            else
            {
                //判断工單第一站是否是當前站位
                string station = TRSR.GetFirstStationBySku(WO.SkuNO, WO.SKU_VER, Station.SFCDB);
                if (station.Equals("ERROR"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { WO.WorkorderNo }));
                }
                else if (!station.Equals(Station.StationName))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000112", new string[] { WO.WorkorderNo, Station.StationName }));
                }
            }
        }

        /// <summary>
        /// 檢查PTH PFB PFT 等站位料件是否上齊全
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void StationKpCompletedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Sn = string.Empty;
            string KpStation = string.Empty;
            AP_DLL ApDll = new AP_DLL();

            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            if (SnSession.Value is SN)
            {
                Sn = ((SN)SnSession.Value).SerialNo;
            }
            else
            {
                Sn = SnSession.Value.ToString();
            }

            KpStation = Paras[1].VALUE;

            MESDBHelper.OleExec ApDB = Station.APDB;
            if (!ApDll.CheckStationKp(Sn, KpStation, ApDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181016135517", new string[] { Sn, Station.StationName }));
            }



        }

        /// <summary>
        /// 检查输入的 SN 是否可以入 B29
        /// SN 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void B29ValidChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            T_R_MRB TRM = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_KEYPART_DETAIL TRSKD = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession Snsession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Snsession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            SN Sn = null;
            if (Snsession.Value is SN)
            {
                Sn = (SN)Snsession.Value;
            }
            else
            {
                Sn = new SN();
                Sn.Load(Snsession.Value.ToString(), Station.SFCDB, Station.DBType);
            }
            WorkOrder wo = null;
            if (WoSession.Value is WorkOrder)
            {
                wo = (WorkOrder)WoSession.Value;
            }
            else
            {
                wo = new WorkOrder();
                wo.Initwo(WoSession.Value.ToString(), Station.SFCDB, Station.DBType);
            }
            //if (Sn.CompletedFlag == "1")
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005161120", new string[] { Sn.SerialNo }));
            //}
            //檢查輸入的工單和產品實際工單是否一致
            if (Sn.WorkorderNo != wo.WorkorderNo)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190813101324", new string[] { Sn.SerialNo, wo.WorkorderNo, Sn.WorkorderNo }));
            }
            if (TRM.HadMrbed(Sn.SerialNo, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { Sn.SerialNo }));
            }
            if (TRSKD.IsLinked(Sn.SerialNo, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181022114912", new string[] { Sn.SerialNo }));
            }

        }

        /// <summary>
        /// 檢查輸入的 SN 是否存在 B29 記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void B29ExistChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_MRB TRM = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string Sn = string.Empty;
            if (SnSession.Value is SN)
            {
                Sn = ((SN)SnSession.Value).SerialNo;
            }
            else
            {
                Sn = SnSession.Value.ToString();
            }

            if (TRM.GetMrbBySnAndStation(Sn, Station.StationName, Station.SFCDB).Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181022144317", new string[] { Sn }));
            }


        }

        /// <summary>
        /// VERTIV檢查輸入的工單是否是用於版本升級后的替換工單及SN是否有替換記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void VertivReplaceSNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SN snObj = (SN)snSession.Value;
            WorkOrder woObj = (WorkOrder)woSession.Value;
            T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_REPLACE_SN t_r_replace_sn = new T_R_REPLACE_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_MRB t_r_mrb = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            bool IsControlWO = t_c_control.ValueIsExist("REPLACE_WO", woObj.WorkorderNo, Station.SFCDB);
            bool IsReplaceSN = t_r_replace_sn.NewSNIsReplace(snObj.SerialNo, Station.SFCDB);
            bool IsReworked = t_r_mrb.IsReworked(snObj.SerialNo, Station.SFCDB);

            if (IsControlWO)
            {
                if (!IsReplaceSN)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181220111025", new string[] { woObj.WorkorderNo, snObj.SerialNo }));
                }
            }
            else
            {
                if (IsReplaceSN && !IsReworked)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181220143330", new string[] { snObj.SerialNo, woObj.WorkorderNo }));
                }
            }
        }

        public static void SNInJobstocChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            string statusFlag = string.Empty;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN sn = (SN)SNSession.Value;
            T_R_STOCK t_r_stock = new T_R_STOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            bool isStock = t_r_stock.IsStockIn(sn.SerialNo, sn.WorkorderNo, Station.SFCDB);
            if (sn.StockStatus != null && sn.StockStatus.Equals("1") && isStock)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005161120", new string[] { sn.SerialNo }));
            }
        }

        /// <summary>
        /// SN重工檢查新舊工單的版本是否一致
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSNWorkorderVerionChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            SN snObject = (SN)snSession.Value;
            WorkOrder newWOObject = (WorkOrder)woSession.Value;
            WorkOrder oldWOObject = new WorkOrder();
            oldWOObject.Init(snObject.WorkorderNo, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_REPLACE_SN t_r_replace_sn = new T_R_REPLACE_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            //List<R_F_CONTROL> list_control = new List<R_F_CONTROL>();
            //list_control = t_r_function_control.GetListByFcv("REWORK_TO_NEW_SKUNO", "SKUNO", Station.SFCDB);
            //if (list_control.Exists(r => r.VALUE == newWOObject.SkuNO && r.EXTVAL == snObject.SkuNo))
            //{
            //    return;
            //}
            //add wo condition: input rework_wo must be same as config rework_wo. asked by PE 楊大饒 2022-01-27
            List<R_FUNCTION_CONTROL_NewList> list_control = new List<R_FUNCTION_CONTROL_NewList>();
            list_control = t_r_function_control.Get1ExListbyVarValue("REWORK_TO_NEW_SKUNO", "SKUNO", newWOObject.SkuNO, snObject.SkuNo, Station.SFCDB);
            if (list_control.Exists(r => r.EXTVAL2 == newWOObject.WorkorderNo))
            {
                return;
            }

            bool IsReplaceSN = t_r_replace_sn.NewSNIsReplace(snObject.SerialNo, Station.SFCDB);
            if (oldWOObject == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { snObject.WorkorderNo }));
            }

            //添加RMA工單不管控版本邏輯
            if (newWOObject.WO_TYPE.Contains("RMA") && (Station.BU == "VNDCN" || Station.BU == "VNJUNIPER"))
            {
                return;
            }
            if (string.IsNullOrEmpty(newWOObject.SKU_VER))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102153958", new string[] { newWOObject.WorkorderNo }));
            }
            if (string.IsNullOrEmpty(oldWOObject.SKU_VER))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102153958", new string[] { oldWOObject.WorkorderNo }));
            }

            if (!IsReplaceSN && newWOObject.SKU_VER != oldWOObject.SKU_VER)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141356", new string[] { oldWOObject.WorkorderNo, newWOObject.WorkorderNo }));
            }
        }

        public static void CheckRmaValid(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            SKU sku = (SKU)skuSession.Value;


            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            //string replacesn = string.Empty;
            // if (snSession.Value.ToString().StartsWith("HSH")) { replacesn = snSession.Value.ToString().Replace("HSH","XSH"); }
            // R_SN sn = TRS.GetSN(replacesn, Station.SFCDB);
            R_SN sn = TRS.GetSN(snSession.Value.ToString(), Station.SFCDB);

            if (sn != null && !sn.ID.StartsWith(snSession.Value.ToString()) && !sn.NEXT_STATION.StartsWith("SHIP"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190425203910", new string[] { sn.SN }));
            }
            if (sn != null && !sn.SKUNO.Substring(0, sn.SKUNO.LastIndexOf("-")).Equals(sku.SkuNo.Substring(0, sku.SkuNo.LastIndexOf("-"))))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190425204057", new string[] { sn.SN, sn.SKUNO, sku.SkuNo }));
            }
            //如果在 R_SN 中有記錄，而且不存在 id starts with sn（這類記錄是從老系統搬過來的記錄），并且沒有出貨記錄的話，那麽就會報錯
            if (Station.SFCDB.ORM.Queryable<R_SN>().Any(t => t.SN == snSession.Value.ToString()) && !Station.SFCDB.ORM.Queryable<R_SN>().Any(t => t.ID.StartsWith(snSession.Value.ToString()) && t.SN == snSession.Value.ToString()) && !Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Any(t => t.SN == snSession.Value.ToString()))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190226162330", new string[] { snSession.Value.ToString() }));
            }

        }

        /// <summary>
        /// 檢查KIT是否已點擊SMTLOADING start 
        /// r_sn_station_detail.started_flag=0說明未點startadd by hgb 2019.05.23
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnSmtLoadingStart(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            string StrSN = "";
            T_R_SN_STATION_DETAIL Table_R_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            StrSN = SNSession.Value.ToString();

            R_Sn_Station_Detail = Table_R_SN_STATION_DETAIL.GetSNSmtLoadingStart(StrSN, Station.SFCDB);

            if (R_Sn_Station_Detail != null)
            {
                if (R_Sn_Station_Detail.STARTED_FLAG == "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091216", new string[] { R_Sn_Station_Detail.WORKORDERNO }));
                }
            }


        }

        /// <summary>
        /// KITTING PRINT
        /// r_sn_station_detail.started_flag=0說明未點startadd by hgb 2019.05.23
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnKittingPrintLoadingStart(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string msg = string.Empty;
            string StrSN = "";
            T_R_SN_STATION_DETAIL Table_R_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            MESStationInput input = Station.Inputs.Find(t => t.DisplayName == "SN");
            if (input == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            string WO = sessionWO.Value.ToString();

            string sku = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO).Select(t => t.SKUNO).First().ToString();
            StrSN = input.Value.ToString();
            var sds = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.WORKORDERNO == WO && t.SN == StrSN && t.STATION_NAME == "KIT_PRINT").ToList().FirstOrDefault();

            if (sds == null)
            {
                throw new Exception($@"SN:{StrSN} không được in ở KITTING hoặc không chính xác!");
            }


        }


        /// <summary>
        /// 檢查SN當前工站的上一站是否有測試PASS記錄,傳入SN,station
        /// 檢查SN當前工站的上一站是ICT是否有pass記錄 
        /// sfcruntime.r_test_record by hgb 2019.05.23
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnStationHavePass(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string LastStationName = Paras[1].VALUE;
            try
            {
                SN snObj = new SN();
                snObj = (SN)sessionSN.Value;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                T_R_WO_TYPE WOType = new T_R_WO_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);

                //除了RMA工單，其他工單都要卡是否有ICT記錄
                if (!WOType.IsTypeInput("RMA", snObj.WorkorderNo.Substring(0, 6), sfcdb))
                {
                    //存在當前工站的上一站式ict(傳入ICT),則進一步檢查ICT工站是否有pass記錄
                    if (t_c_route_detail.CheckLaststationIs(snObj.RouteID, Station.StationName, LastStationName, Station.SFCDB))
                    {
                        T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                        bool test = t_r_test_record.CheckTestBySNAndStation2(snObj.ID, LastStationName, sfcdb);
                        if (!test)//沒有pass記錄報錯
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091217", new string[] { }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        /// <summary>
        /// 檢查ECN簽核情況        
        /// add by hgb 2019.05.28
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckEcn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string WO = sessionWO.Value.ToString();
            try
            {
                T_R_ECN t_r_ecn = new T_R_ECN(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //檢查ECN是否存在
                if (t_r_ecn.CheckEcnExist("WO", WO, Station.SFCDB))
                {
                    Row_R_ECN R = t_r_ecn.GetEcn("WO", WO, Station.SFCDB);
                    T_R_SIGN t_r_sign = new T_R_SIGN(Station.SFCDB, DB_TYPE_ENUM.Oracle);


                    //檢查ECN PE是否已簽核
                    if (!t_r_sign.CheckIsSign("PE", R.LOT_NO, "ECNCHECK", Station.SFCDB))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091221", new string[] { WO, "PE" }));
                    }

                    //檢查ECN PQE是否已簽核
                    if (!t_r_sign.CheckIsSign("PQE", R.LOT_NO, "ECNCHECK", Station.SFCDB))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091221", new string[] { WO, "PQE" }));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查FT第一道工序
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckFT_First_One(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string WO = sessionWO.Value.ToString();
            WorkOrder ojbWO = (WorkOrder)sessionWO.Value;
            string RouteID = ojbWO.RouteID;
            string Sku = ojbWO.SkuNO;
            //DataTable dt, dtft;
            int counts;

            try
            {
                OleExec sfcdb = Station.SFCDB;
                DB_TYPE_ENUM sfcdbType = Station.DBType;

                //string strSql = $@"select * from c_route_detail where route_id='{RouteID}' and station_name like'FT1%'";
                //dt = sfcdb.RunSelect(strSql).Tables[0];
                //if (dt.Rows.Count > 0)
                //{
                //    strSql = $@"select * from SFCBASE.C_SKU_FT_FIRST_CONFIG t where skuno='{Sku}' and FT1 is not null";
                //    dtft = sfcdb.RunSelect(strSql).Tables[0];
                //    if (dtft.Rows.Count == 0)
                //    {
                //        throw new Exception("該機種路由存在FT1工站但未配置FT1的第一道工序，請聯繫TE進行配置后再LOADING！");
                //    }
                //}

                counts = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == RouteID && t.STATION_NAME.StartsWith("FT1")).ToList().Count;
                if (counts > 0)
                {
                    counts = Station.SFCDB.ORM.Queryable<c_sku_ft_first_config>().Where(t => t.SKUNO == Sku && t.FT1 != null).ToList().Count;
                    if (counts == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190826152329", new string[] { Sku }));
                        //throw new Exception("該機種:{0}路由存在FT1工站但未配置FT1的第一道工序，請聯繫TE進行配置后再LOADING！");
                    }
                }

                //strSql = $@"select * from c_route_detail where route_id='{RouteID}' and station_name like'FT2%'";
                //dt = new DataTable();
                //dt = sfcdb.RunSelect(strSql).Tables[0];
                //if (dt.Rows.Count > 0)
                //{
                //    strSql = $@"select * from SFCBASE.C_SKU_FT_FIRST_CONFIG t where skuno='{Sku}' and FT2 is not null";
                //    dtft = new DataTable();
                //    dtft = sfcdb.RunSelect(strSql).Tables[0];
                //    if (dtft.Rows.Count == 0)
                //    {
                //        throw new Exception("該機種路由存在FT2工站但未配置FT2的第一道工序，請聯繫TE進行配置后再LOADING！");
                //    }
                //}
                counts = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == RouteID && t.STATION_NAME.StartsWith("FT2")).ToList().Count;
                if (counts > 0)
                {
                    counts = Station.SFCDB.ORM.Queryable<c_sku_ft_first_config>().Where(t => t.SKUNO == Sku && t.FT2 != null).ToList().Count;
                    if (counts == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190826152929", new string[] { Sku }));
                        //throw new Exception("該機種{0}路由存在ORT工站但未配置ORT工序，請聯繫TE進行配置后再LOADING！");
                    }
                }

                //strSql = $@"select * from c_route_detail where route_id='{RouteID}' and station_name like'FT3%'";
                //dt = new DataTable();
                //dt = sfcdb.RunSelect(strSql).Tables[0];
                //if (dt.Rows.Count > 0)
                //{
                //    strSql = $@"select * from SFCBASE.C_SKU_FT_FIRST_CONFIG t where skuno='{Sku}' and FT3 is not null";
                //    dtft = new DataTable();
                //    dtft = sfcdb.RunSelect(strSql).Tables[0];
                //    if (dtft.Rows.Count == 0)
                //    {
                //        throw new Exception("該機種路由存在FT3工站但未配置FT3的第一道工序，請聯繫TE進行配置后再LOADING！");
                //    }
                //}
                counts = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == RouteID && t.STATION_NAME.StartsWith("FT3")).ToList().Count;
                if (counts > 0)
                {
                    counts = Station.SFCDB.ORM.Queryable<c_sku_ft_first_config>().Where(t => t.SKUNO == Sku && t.FT3 != null).ToList().Count;
                    if (counts == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190826153445", new string[] { Sku }));
                        //throw new Exception("該機種{0}路由存在ORT工站但未配置ORT工序，請聯繫TE進行配置后再LOADING！");
                    }
                }

                counts = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == RouteID && t.STATION_NAME.Substring(0, 3) == "ORT").ToList().Count;
                if (counts > 0)
                {
                    counts = Station.SFCDB.ORM.Queryable<c_sku_ft_first_config>().Where(t => t.SKUNO == Sku && t.ORT != null).ToList().Count;
                    if (counts == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190826094318", new string[] { Sku }));
                        //throw new Exception("該機種{0}路由存在ORT工站但未配置ORT工序，請聯繫TE進行配置后再LOADING！");
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 待測2DX/5DX/NORMAL_STATION   
        /// add by hgb 2019.05.28
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void Check2dx5dxNormal_station(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string SN = sessionSN.Value.ToString();
            try
            {
                T_R_2DX5DX_SAMPLING_SN t_r_2dx_sn = new T_R_2DX5DX_SAMPLING_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                //待測2DX/5DX/NORMAL_STATION
                t_r_2dx_sn.CheckSampling(SN, "2", "2DX", Station.SFCDB);
                t_r_2dx_sn.CheckSampling(SN, "2", "5DX", Station.SFCDB);
                t_r_2dx_sn.CheckSampling(SN, "2", "NORMAL_STATION", Station.SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 檢查100%抽測ORT的PASS記錄   
        /// add by hgb 2019.07.30
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckOrtPassRecord(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM sfcdbType = Station.DBType;
            T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
            R_SN r_sn = new R_SN();
            r_sn = t_r_sn.LoadData(sessionSn.Value.ToString(), sfcdb);

            string strSql = $@"
                SELECT *
                 FROM c_ort_sampling_sku
                WHERE sampling_type IN
                      (SELECT TYPE FROM c_ort_sampling WHERE SAMPLING_QTY = 100)
                 AND skuno = '{r_sn.SKUNO}' ";

            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                strSql = $@"
                SELECT *
                  FROM r_ort_sampling_wo
                 WHERE WO =  '{r_sn.WORKORDERNO}' ";

                dt = Station.SFCDB.RunSelect(strSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    strSql = $@"
                SELECT round (sampling_total / WOQTY * 100,2)
                  FROM r_ort_sampling_wo
                 WHERE WO =  '{r_sn.WORKORDERNO}' ";

                    dt = Station.SFCDB.RunSelect(strSql).Tables[0];
                    string aa = dt.Rows[0][0].ToString();

                    if (Convert.ToDouble(dt.Rows[0][0].ToString()) >= 100)//--說明為百分之百抽測
                    {
                        T_R_WO_TYPE WOType = new T_R_WO_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);

                        //除了RMA工單，其他工單都要卡是否有ort記錄
                        if (!WOType.IsTypeInput("RMA", r_sn.WORKORDERNO.Substring(0, 6), sfcdb))
                        {
                            T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                            bool test = t_r_test_record.CheckTestBySNAndStation2(r_sn.ID, "ORT", sfcdb);
                            if (!test)//沒有pass記錄報錯
                            {
                                //throw new MESReturnMessage($@"機種{r_sn.SKUNO}抽測比例為百分之百,此SN沒有ORT測試PASS記錄,請重測ORT");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163703", new string[] { r_sn.SKUNO }));


                            }

                        }
                    }



                }

            }

            strSql = $@"
                                SELECT *
                      FROM R_SN_KP
                     WHERE SN = '{r_sn.SN}'
                       AND VALUE LIKE '*%' ";

            dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //throw new MESReturnMessage($@"'SN 在R_SN_KPL表中有包含*號的Keypart,請退回到ASSY,重新過組裝 ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163739"));

            }

        }



        /// <summary>
        /// 第二道測試失敗返回FT2_01  
        /// add by hgb 2019.07.30
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FtFailReturn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }


            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM sfcdbType = Station.DBType;
            T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
            R_SN r_sn = new R_SN();
            r_sn = t_r_sn.LoadData(sessionSN.Value.ToString(), sfcdb);
            Station.SFCDB.BeginTrain();
            try
            {
                T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                //有配置 第二道測試失敗返回FT2_01
                if (t_c_control.ValueIsExist("FtFailReturn", r_sn.SKUNO, sfcdb))
                {
                    string strSql = $@"
                  SELECT *
          FROM r_ft_logdata
         WHERE SN = '{r_sn.BOXSN}'
           AND tps_name = 'ITEF_TEST_ATP'
           AND RESULT = 'PASS' ";

                    DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
                    //返回FT2_01
                    if (dt.Rows.Count == 0)
                    {
                        Route routeDetail = new Route(r_sn.ROUTE_ID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                        List<string> snStationList = new List<string>();
                        List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                        RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == "FT2_01").FirstOrDefault();
                        string LastStation = string.Empty;

                        if (R == null)
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180724154541", new string[] { Station.StationName }));
                        if (routeDetailList.Where(r => r.SEQ_NO < R.SEQ_NO).LastOrDefault() == null)//當前工站為最後一個工站時
                        {
                            LastStation = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO).LastOrDefault().STATION_TYPE;
                        }
                        else
                        {
                            LastStation = routeDetailList.Where(r => r.SEQ_NO < R.SEQ_NO).LastOrDefault().STATION_NAME;
                        }


                        strSql = $@"
                 UPDATE R_SN
             SET   CURRENT_STATION='{LastStation}',NEXT_STATION ='FT2_01'  
           WHERE SN = '{r_sn.SN}' ";

                        Station.SFCDB.ExecSQL(strSql);
                        Station.SFCDB.CommitTrain();

                        //throw new MESReturnMessage($@"此SN在FT2_01未通過第二道測試，請重測FT2_01！");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163803"));

                    }


                }
            }
            catch (Exception ex)
            {
                Station.SFCDB.RollbackTrain();
                throw ex;
            }

        }

        /// <summary>
        /// 檢查RMA維修記錄  
        /// add by hgb 2019.08.06
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckRmaRepairRecord(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }


            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM sfcdbType = Station.DBType;
            T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
            R_SN r_sn = new R_SN();
            r_sn = t_r_sn.LoadData(sessionSN.Value.ToString(), sfcdb);

            try
            {
                T_R_WO_TYPE WOType = new T_R_WO_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);

                //卡RMA
                if (WOType.IsTypeInput("RMA", r_sn.WORKORDERNO.Substring(0, 6), sfcdb))
                {
                    T_R_RMA_DETAIL t_r_rma_detail = new T_R_RMA_DETAIL(sfcdb, sfcdbType);
                    t_r_rma_detail.IsExistsRmaRepairRecord(r_sn.SN, sfcdb);
                    t_r_rma_detail.IsConfirmRmaRepairRecord(r_sn.SN, sfcdb);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 檢查輸入的機種和SN機種是否一樣
        /// add by hgb 2019.08.06
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckInputSkuAndSnSKU(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSkuInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSkuInput == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }


            try
            {
                if (sessionSkuInput.Value.ToString() != sessionSku.Value.ToString())
                {
                    //throw new MESReturnMessage($@"您輸入的機種{sessionSkuInput.Value.ToString()}和SN機種{sessionSku.Value.ToString()}不一樣，請重新輸入！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163828", new string[] { sessionSkuInput.Value.ToString(), sessionSku.Value.ToString() }));

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 檢查輸入的SN是不是不虛擬的條碼
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputValueIsVirtualChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var sessionObj = snSession.Value;
            SN snObj = null;
            R_SN r_sn = null;
            string sn;
            if (sessionObj is string)
            {
                sn = sessionObj.ToString();
            }
            else if (typeof(SN) == sessionObj.GetType())
            {
                snObj = (SN)sessionObj;
                sn = snObj.SerialNo;
            }
            else if (typeof(R_SN) == sessionObj.GetType())
            {
                r_sn = (R_SN)sessionObj;
                sn = r_sn.SN;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { sessionObj.ToString() }));
            }
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            R_SN objSN = TRS.LoadData(sn, Station.SFCDB);
            string wip_group = objSN.NEXT_STATION;
            if (wip_group == "SHIPOUT")
            {
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210804083021", new string[] { sessionObj.ToString() }));
                throw new Exception("This is" + sn + "status " + wip_group + " not CHECKIN MRB.");
            }
            if (objSN.ID == objSN.SN)
            {
                throw new Exception("Please input actual sn!");
            }
        }
        /// <summary>
        /// 檢查SN的客戶料號是否與輸入的機種一致
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNCustNoChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SKU sku = new SKU();
            SKU SkuObj = (SKU)SKUSession.Value;
            SN SNObj = (SN)SNSession.Value;
            SKU newSkuObj = sku.Init(SNObj.SkuNo, Station.SFCDB, Station.DBType);
            if (!SkuObj.CustPartNo.Equals(newSkuObj.CustPartNo))
            {
                throw new Exception("the Cust NO of" + SNObj.SerialNo + " not equals " + SkuObj.SkuNo);
            }
        }

        /// <summary>
        /// 檢查SN最後一筆過站記錄是否是傳入工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNPassStationChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (sessionStation.Value.ToString() == "")
            {
                return;
            }
            bool bPassStation = true;
            string sn = "";
            T_R_SN_STATION_DETAIL TRSSD = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN objSN = (SN)sessionSN.Value;
                sn = objSN.SerialNo;
                bPassStation = TRSSD.GetDetailBySnAndStation(objSN.SerialNo, sessionStation.Value.ToString(), Station.SFCDB) == null ? false : true;
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                List<R_SN> listSN = panelObject.GetSnDetail(panelObject.PanelNo, Station.SFCDB, Station.DBType);
                foreach (R_SN snObj in listSN)
                {
                    bPassStation = TRSSD.GetDetailBySnAndStation(snObj.SN, sessionStation.Value.ToString(), Station.SFCDB) == null ? false : true;
                    if (!bPassStation)
                    {
                        sn = snObj.SN;
                        break;
                    }
                }
            }
            if (!bPassStation)
            {
                string msg = sn + " not pass " + sessionStation.Value.ToString();
                sessionStation.Value = "";
                throw new MESReturnMessage(msg);
            }
        }

        /// <summary>
        /// 判斷產品是否可以打不良（針對自動測試站位，並且在 R_TEST_RECORD裡面有測試記錄的）
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnCanRepairChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_TEST_RECORD RTR = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //R_SN Sn = (R_SN)SNSession.Value;
            SN Sn = (SN)SNSession.Value;
            bool IsTest = RTR.CheckStationIsTest(Sn.SkuNo, Station.StationName, Station.SFCDB);
            //獲取這個 SN 在這個站位的最後一筆測試記錄
            R_TEST_RECORD record = RTR.GetLastTestRecord(Sn.SerialNo, Station.StationName, Station.SFCDB);
            //如果判斷得到這個機種這個站位應該就是測試站位的話
            if (IsTest)
            {
                //測試站位沒有測試記錄，但是嘗試打不良，就不允許
                if (record != null)
                {
                    //測試記錄存在但是不等於 FAIL 則也不允許
                    if (!record.STATE.Equals("FAIL"))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616102950", new string[] { Sn.SerialNo }));
                    }
                    else
                    {
                        //測試記錄存在且是 FAIL 但是 R_SN 中 REPAIR_FAILED_FLAG 狀態不為 1 也不允許打不良
                        if (!Sn.RepairFailedFlag.Equals("1"))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616102950", new string[] { Sn.SerialNo }));
                        }
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616102950", new string[] { Sn.SerialNo }));
                }
            }

        }


        /// <summary>
        /// 用來檢查用戶是否有這個操作的權限
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UserPrivilegeChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }

            string PrivilegeName = Paras[0].VALUE;
            T_C_PRIVILEGE TCP = new T_C_PRIVILEGE(Station.SFCDB, Station.DBType);
            T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(Station.SFCDB, Station.DBType);
            T_C_ROLE_PRIVILEGE TCRR = new T_C_ROLE_PRIVILEGE(Station.SFCDB, Station.DBType);
            C_PRIVILEGE Privilege = TCP.GetPrivilegeByName(PrivilegeName, Station.SFCDB);

            if (Privilege != null)
            {
                if (!TCUP.CheckpPivilegeByName(Station.SFCDB, PrivilegeName, Station.LoginUser.EMP_NO))
                {
                    if (!TCRR.CheckUserHasPrivilege(Station.LoginUser.EMP_NO, PrivilegeName, Station.SFCDB))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20191026133633", new string[] { Station.LoginUser.EMP_NO, Privilege.PRIVILEGE_DESC }));
                    }
                }
            }

        }


        /// <summary>
        /// 根據配置檢查SN是否能在當前工站掃描FAIL
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ScanFailStationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null || snSession.Value == null || snSession.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SN snObject = (SN)snSession.Value;
            //T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            R_F_CONTROL controlObj = null;
            //C_SKU_DETAIL controlDetail;
            string catgory = "";
            for (int i = 1; i < Paras.Count; i++)
            {
                if (Paras[i].VALUE.Trim().ToUpper() == "")
                {
                    continue;
                }
                catgory = Paras[i].VALUE.Trim().ToUpper();
                //controlDetail = t_c_sku_detail.GetSkuDetail(snObject.SkuNo, catgory, catgory, Station.SFCDB);
                controlObj = t_r_function_control.GetControl(Station.SFCDB, catgory, catgory, snObject.SkuNo);
                if (controlObj != null && controlObj.EXTVAL.Trim() != "" && controlObj.EXTVAL.Trim().ToUpper() == snObject.NextStation.ToUpper())
                {
                    throw new MESReturnMessage($@"Can't Scan Fail On Current Station {snObject.NextStation},Only Scan Pass,Please Scan Fail On {controlObj.EXTVAL.Trim()}");
                }
            }
        }

        /// <summary>
        /// 根據配置檢查SN是否能在當前工站掃描PASS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ScanPassStationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null || snSession.Value == null || snSession.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SN snObject = (SN)snSession.Value;
            //T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            //C_SKU_DETAIL controlDetail;       
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            R_F_CONTROL controlObj = null;
            R_TEST_RECORD testObject = null;
            string catgory = "";
            for (int i = 1; i < Paras.Count; i++)
            {
                if (Paras[i].VALUE.Trim().ToUpper() == "")
                {
                    continue;
                }
                catgory = Paras[i].VALUE.Trim().ToUpper();
                controlObj = t_r_function_control.GetControl(Station.SFCDB, catgory, catgory, snObject.SkuNo);
                //controlDetail = t_c_sku_detail.GetSkuDetail(snObject.SkuNo, catgory, catgory, Station.SFCDB);
                if (controlObj != null && controlObj.EXTVAL.Trim() != "")
                {
                    testObject = Station.SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(r => r.SN == snObject.SerialNo && r.MESSTATION == controlObj.EXTVAL.Trim())
                        .OrderBy(r => r.STARTTIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                    if (testObject != null && (testObject.STATE == "F" || testObject.STATE == "FAIL"))
                    {
                        throw new MESReturnMessage($@"{snObject.SerialNo} Last Test Fail On {controlObj.EXTVAL.Trim()},Please Scan Fail On {Station.StationName}!");
                    }
                }
            }
        }
        public static void SNTestIfByPassChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {
                T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                SN snObj = new SN();
                snObj = (SN)sessionSN.Value;
                #region 檢查當前工站之前的所有測試工站的最後一筆測試記錄是否PASS By Eden 2018/05/16
                List<C_ROUTE_DETAIL> cRouteDetailList = t_c_route_detail.GetTestStationByNameBefor(sfcdb, snObj.RouteID, stationName);
                List<R_TEST_RECORD> td = t_r_test_record.GetTestDataByTimeBefor(sfcdb, snObj.SerialNo, snObj.StartTime ?? DateTime.Now);
                List<R_SN_PASS> rsp = GetPassStation(sfcdb, snObj);
                T_R_MES_LOG mesLog = new T_R_MES_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

                foreach (var item in cRouteDetailList)
                {
                    //  檢查當前工站 是否 by passStation  r_sn_pass
                    if (td.FindAll(t => t.MESSTATION == item.STATION_NAME && t.STATE.Equals("PASS")).Count == 0 && rsp.FindAll(r => r.PASS_STATION == item.STATION_NAME).Count == 0)
                    {
                        sfcdb.BeginTrain();
                        rowMESLog.ID = mesLog.GetNewID(Station.BU, sfcdb);
                        rowMESLog.PROGRAM_NAME = "CloudMES";
                        rowMESLog.CLASS_NAME = "Check SN";
                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                        rowMESLog.LOG_MESSAGE = "Test Pass Data not exists for " + snObj.SerialNo + " in station " + item.STATION_NAME;
                        rowMESLog.LOG_SQL = "";
                        rowMESLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                        rowMESLog.DATA1 = snObj.SerialNo;
                        sfcdb.ThrowSqlExeception = true;
                        sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                        sfcdb.CommitTrain();
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { snObj.SerialNo, item.STATION_NAME }));
                    }
                }

                #endregion
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void GLUEChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            OleExec apdb = null;
            string sql = "";
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null || sessionSKU.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SKU skuO = (SKU)sessionSKU.Value;
            R_F_CONTROL rfc = GetFunctionControl(sfcdb, skuO.SkuNo, Station.DisplayName.ToString(), Paras[1].VALUE);
            if (rfc != null)
            {

                UIInputData O = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "", Tittle = "請輸入膠水ALLPART條碼", Type = UIInputType.String, Name = "IMEI", ErrMessage = "未輸入膠水ALLPART條碼" };
                //O.OutInputs.Add(new DisplayOutPut() { Name = "PCBA S/N", DisplayType = UIOutputType.Password.ToString(), Value = "465136N+2009RR01F5" });
                //O.OutInputs.Add(new DisplayOutPut() { Name = "MAC S/N", DisplayType = UIOutputType.Text.ToString(), Value = "01:B0:L4:58:12" });
                AP_DLL APDLL = new AP_DLL();
                try
                {
                    apdb = Station.APDB;
                    DataTable dt = null;
                    var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
                    {
                        sql = $@"select * from mes4.r_tr_sn where cust_kp_no = '{rfc.CATEGORYDEC}' and  tr_sn ='{res.ToString()}' ";

                        dt = apdb.ExecSelect(sql).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            sql = $@"select trunc(sysdate -max(move_time),1) as date1 from mes4.r_kitting_scan_detail where tr_sn = '{res.ToString()}' and move_type = 'c'";
                            DataTable res1 = apdb.ExecSelect(sql).Tables[0];
                            if (res1.Rows[0]["date1"].ToString() != "")
                            {
                                if (Convert.ToDouble(res1.Rows[0]["date1"].ToString()) > 7.0)
                                {
                                    O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20200423085626", new string[] { res.ToString() });
                                    return false;
                                }
                            }
                            else
                            {
                                O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20200423085051", new string[] { res.ToString() });
                                return false;
                            }
                            return true;

                        }
                        else
                            O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20200424140818", new string[] { rfc.CATEGORYDEC, res.ToString() });
                        return false;
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public static void fixtureOfflineRecordChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            OleExec APDB = Station.APDB;
            string sql = "";

            bool checkFlag = true;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            WorkOrder wo = (WorkOrder)SNSession.Value;
            T_R_SN_STATION_DETAIL TRSNSD = new T_R_SN_STATION_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
            R_SN_STATION_DETAIL Rsnsd = TRSNSD.GetLastStationByWo(wo.WorkorderNo, Station.StationName, SFCDB);
            try
            {

                sql = "SELECT SYSDATE FROM DUAL";
                DateTime DBTime = (DateTime)SFCDB.ExecSelectOneValue(sql);
                DateTime EndTime;

                if (Rsnsd != null)
                {
                    DateTime? LastStationTime = Rsnsd.EDIT_TIME;
                    if (LastStationTime <= DBTime.AddMinutes(-30))
                    {
                        DataTable dt = null;
                        sql = $@"SELECT MAX(OFFLINE_TIME) OFFLINE_TIME FROM MES4.R_FIXTURE_SCAN_DETAIL 
                            WHERE OUT_LINE='{Station.Line}'
                            AND FIXTURE_TYPE='鋼網' 
                            AND OUT_SATION='{Station.StationName}'
                            AND OFFLINE_TIME IS NOT NULL";
                        dt = APDB.ExecSelect(sql).Tables[0];
                        if (dt.Rows[0]["OFFLINE_TIME"].ToString() != "")
                        {
                            EndTime = Convert.ToDateTime(dt.Rows[0]["OFFLINE_TIME"]);
                            if (LastStationTime <= EndTime)
                            {
                                checkFlag = false;
                            }

                        }
                    }
                    else
                    {
                        checkFlag = false;
                    }
                }
                else
                {
                    checkFlag = false;
                }
                if (checkFlag)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200505151149", new string[] { Station.Line }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void ComparisonChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string Sn = Input.Value.ToString();
            if (Sn == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            bool bol = sfcdb.ORM.Queryable<R_SN_KP, R_SN_KP>((k, k1) => k.VALUE == k1.SN).Where((k, k1) => k.VALID_FLAG == 1 && k1.VALID_FLAG == 1 && k.SN == Sn && k.KP_NAME == "MODEL").Select((k, k1) => k).Any();
            if (bol)
            {
                R_SN_KP LinkSN = sfcdb.ORM.Queryable<R_SN_KP, R_SN_KP>((K, k1) => K.SN == k1.VALUE).Where((k, k1) => k1.SN == Sn && k.KP_NAME == "ERICSSON" && k.VALID_FLAG == 1 && k1.VALID_FLAG == 1).Select((k, k1) => k).ToList().FirstOrDefault();
                if (LinkSN != null)
                {
                    UIInputData O = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "", Tittle = "輸入ERICSSON 條碼", Type = UIInputType.String, Name = "IMEI", ErrMessage = "請輸入ERICSSON 條碼" };
                    var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
                    {
                        if (LinkSN.VALUE.ToString() == res.ToString())
                        {
                            return true;
                        }
                        else
                            //O.CBMessage = $@"驗證失敗，{LinkSN.SN} 不存在=>{res} ERICSSON條碼,請重試!";
                            O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163930", new string[] { LinkSN.SN, res });
                        return false;
                    });
                    UIInputData A = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "", Tittle = "輸入ERICSSON 條碼後11位", Type = UIInputType.String, Name = "IMEI", ErrMessage = "請輸入ERICSSON 條碼後11位" };
                    var ret2 = A.GetUiInput(Station.API, UIInput.Normal, Station, (res2) =>
                    {
                        if (ret1.ToString().Substring(ret1.ToString().Length - 11, 11) == res2)
                        {
                            return true;
                        }
                        else
                            //A.CBMessage = $@"驗證失敗，ERICSSON條碼{ret1.ToString()} 後11位與=>{res2} 不一致,請重試!";
                            A.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164010", new string[] { ret1.ToString(), res2 });
                        return false;
                    });
                }
                else
                {
                    //throw new MESReturnMessage($@"此 {Sn} 不存在KP");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164053", new string[] { Sn }));

                }
            }
            else
            {
                List<R_SN_KP> LinkSN = sfcdb.ORM.Queryable<R_SN_KP>().Where((k) => k.SN == Sn && k.VALID_FLAG == 1).ToList();
                if (LinkSN != null)
                {
                    var LinVal = LinkSN.FindAll(t => t.KP_NAME == "PACK" && t.VALID_FLAG == 1).FirstOrDefault();
                    var LinEri = LinkSN.FindAll(t => t.KP_NAME == "ERICSSON" && t.VALID_FLAG == 1).FirstOrDefault();
                    if (LinVal != null)
                    {
                        UIInputData O = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "", Tittle = "輸入PACK 條碼", Type = UIInputType.String, Name = "IMEI", ErrMessage = "請輸入PACK 條碼" };
                        var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
                        {
                            if (LinVal.VALUE.ToString() == res.ToString())
                            {
                                return true;
                            }
                            else
                                O.CBMessage = $@"驗證失敗，{LinVal.SN} 不存在=> {res} PACK 條碼,請重試!";
                            return false;
                        });
                        if (LinEri != null)
                        {
                            UIInputData A = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "", Tittle = "輸入ERICSSON 條碼", Type = UIInputType.String, Name = "IMEI", ErrMessage = "請輸入ERICSSON條碼" };
                            var ret2 = A.GetUiInput(Station.API, UIInput.Normal, Station, (res2) =>
                            {
                                if (LinEri.VALUE.ToString() == res2)
                                {
                                    return true;
                                }
                                else
                                    A.CBMessage = $@"驗證失敗，ERICSSON條碼 {LinEri.VALUE.ToString()} 與=> 輸入的 {res2} 不一致,請重試!";
                                return false;
                            });
                            UIInputData B = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "", Tittle = "輸入ERICSSON 條碼 后11位", Type = UIInputType.String, Name = "IMEI", ErrMessage = "請輸入ERICSSON 后11位" };
                            var ret3 = B.GetUiInput(Station.API, UIInput.Normal, Station, (res3) =>
                            {
                                if (ret2.ToString().Substring(ret2.ToString().Length - 11, 11) == res3)
                                {
                                    return true;
                                }
                                else
                                    B.CBMessage = $@"驗證失敗，ERICSSON條碼 {ret2.ToString()} 後11位與=> 輸入的 {res3} 不一致,請重試!";
                                return false;
                            });
                        }
                        else
                        {
                            //throw new MESReturnMessage("不存在 KP類型是 ‘PACK’ 的KP");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162915"));

                        }
                    }
                    else
                    {
                        //throw new MESReturnMessage("不存在 KP類型是 ‘ERICSSON’ 的KP");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162947"));

                    }

                }
                else
                {
                    //throw new MESReturnMessage($@"此 {Sn} 不存在KP");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164053", new string[] { Sn }));

                }
            }
        }

        public static void CarrierLinkChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string sql = "";
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN sn = (SN)SNSession.Value;
            T_R_CARRIER_LINK TRCL = new T_R_CARRIER_LINK(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_CARRIER_SKUNO_LINK TRCKL = new T_R_CARRIER_SKUNO_LINK(SFCDB, DB_TYPE_ENUM.Oracle);
            try
            {
                int ValidDays = 1;
                sql = "SELECT SYSDATE FROM DUAL";
                DateTime DBTime = (DateTime)SFCDB.ExecSelectOneValue(sql);
                bool checkRecord = TRCL.CheckLink(sn.SerialNo, DBTime, ValidDays, SFCDB);
                bool checkSku = TRCKL.checkExistBySkuno(sn.SkuNo, SFCDB);
                if (!checkRecord && checkSku)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200508104036", new string[] { sn.SerialNo, ValidDays.ToString() + "Day(s)" }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void VIStationChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            OleExec SFCDB = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string sql = "SELECT SYSDATE-1/3 FROM DUAL";
            DateTime DBTime = (DateTime)SFCDB.ExecSelectOneValue(sql);
            SN sn = (SN)SNSession.Value;
            bool bol = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn.SerialNo && t.VALID_FLAG == "1" && (t.STATION_NAME == "VI1" || t.STATION_NAME == "VI2" || t.STATION_NAME == "BAKE") && t.EDIT_TIME >= DBTime).Any();
            if (!bol)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200605142328"));
            }
        }
        public static void BakeStationChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            OleExec SFCDB = Station.SFCDB;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            //MESStationSession stSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            //if (stSession == null || stSession.Value == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            //}
            var stationName = Paras[1].VALUE.ToString();
            SN sn = (SN)SNSession.Value;
            bool bol = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn.SerialNo && t.VALID_FLAG == "1" && t.NEXT_STATION == stationName).Any();
            if (!bol)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200605171029", new string[] { stationName }));
            }
        }
        public static void DoubleChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (packSession == null || packSession.Value == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            //}
            //string pack = packSession.Value.ToString();
            string pack = Input.Value.ToString();
            T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_PACKING r_packing = t_r_packing.GetPackingByPackNo(pack, Station.SFCDB);
            if (r_packing == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { pack }));
            }

            bool bol = IsBool(SFCDB, pack);

            if (bol)
            {

                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200622140355"));
            }

        }
        public static void FWChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            bool bol = false;
            bool bol2 = false;
            OleExec SFCDB = Station.SFCDB;
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN sn = (SN)SNSession.Value;

            if (Paras[1].VALUE.Trim().ToUpper() == "NO_CHECK_FIRMWARE")
            {
                bol = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONTYPE == "NO_CHECK_FIRMWARE" && t.FUNCTIONNAME == "NO_CHECK_FIRMWARE" && t.VALUE == sn.SkuNo).Any();
            }

            if (Paras[2].VALUE.Trim().ToUpper() == "NO_CHECK_SOFTWARE")
            {
                bol2 = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONTYPE == "NO_CHECK_SOFTWARE" && t.FUNCTIONNAME == "NO_CHECK_SOFTWARE" && t.VALUE == sn.SkuNo).Any();
            }

            if (bol)
            {
                bol = SFCDB.ORM.Queryable<C_KP_List_Item, C_KP_List_Item_Detail>((CI, CID) => CI.ID == CID.ITEM_ID).Where((CI, CID) => CID.SCANTYPE == "FIRMWARE P/N" && CI.LIST_ID == sn.KP_LIST_ID).Any();
                if (!bol)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200623085605"));
                }
            }

            if (bol2)
            {
                bol2 = SFCDB.ORM.Queryable<C_KP_List_Item, C_KP_List_Item_Detail>((CI, CID) => CI.ID == CID.ITEM_ID).Where((CI, CID) => CID.SCANTYPE == "SOFTWARE P/N" && CI.LIST_ID == sn.KP_LIST_ID).Any();
                if (!bol2)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200623085741"));
                }
            }

        }
        public static void FAIChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            bool bol = false;
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession skuNoSession = Station.StationSession.Find(t => t.MESDataType == "SKUNO" && t.SessionKey == "1");
            OleExec SFCDB = Station.SFCDB;
            var skuNo = skuNoSession.Value.ToString();
            if (string.IsNullOrEmpty(skuNo))
            {
                throw new Exception("Can not get skuno!");
            }

            SKU sku = new SKU();
            sku = sku.Init(skuNo, SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (sku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { skuNo }));
            }

            string[] valus = new string[Paras.Count];
            for (int i = 0; i < valus.Length; i++)
            {
                valus[i] = Paras[i].VALUE.ToString();
            }

            bol = SFCDB.ORM.Queryable<R_FAI>().Where(r => r.FAITYPE == "SKUNO" && r.STATUS == "1" && r.SKU_VER == sku.Version && r.SKUNO == sku.SkuNo).Any();
            if (!bol)
            {

                var customerList = SFCDB.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((cs, css, cc) => cs.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID)
                    .Where((cs, css, cc) => cs.SKUNO == sku.SkuNo).Select((cs, css, cc) => cc.CUSTOMER_NAME).ToList();

                foreach (var val in valus)
                {
                    if (customerList != null && customerList.Contains(val))
                    {
                        bol = true;
                    }
                }
                //bol = SFCDB.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((cs, css, cc) => cs.C_SERIES_ID == css.ID && css.CUSTOMER_ID == cc.ID)
                //    .Where((cs, css, cc) => cs.SKUNO == sku.SkuNo && SqlSugar.SqlFunc.ContainsArray(valus, cc.DESCRIPTION)).Any();
                if (!bol)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200623093757", new string[] { sku.SkuNo }));
                }
            }

        }
        public static List<R_SN_PASS> GetPassStation(OleExec DB, SN sn)
        {

            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(DB, DB_TYPE_ENUM.Oracle);
            R_LOT_STATUS getLotNo;

            getLotNo = RowLotStatus.GetLotBySNAndWo(sn.SerialNo, sn.WorkorderNo, DB);
            if (getLotNo != null)
            {
                return DB.ORM.Queryable<R_SN_PASS>().Where(t => (t.WORKORDERNO == sn.WorkorderNo || t.WORKORDERNO == sn.SkuNo || t.SN == sn.SerialNo || t.LOTNO == getLotNo.LOT_NO) && t.STATUS == "1").ToList();
            }
            else
            {
                return DB.ORM.Queryable<R_SN_PASS>().Where(t => (t.WORKORDERNO == sn.WorkorderNo || t.WORKORDERNO == sn.SkuNo || t.SN == sn.SerialNo) && t.STATUS == "1").ToList();
            }
        }
        public static R_F_CONTROL GetFunctionControl(OleExec ole, string SKU, string Station, string type)
        {

            return ole.ORM.Queryable<R_F_CONTROL>().Where(t => t.VALUE == SKU && t.EXTVAL == Station && t.FUNCTIONNAME == type && t.CATEGORY == type).OrderBy(t => t.EDITTIME).ToList().FirstOrDefault();
        }
        public static bool IsBool(OleExec ole, string Packno)
        {
            string sql = "";
            bool bol = false;
            bol = ole.ORM.Queryable<R_PACKING, R_SKU_ROUTE, C_SKU, C_ROUTE_DETAIL>((rp, rsr, cs, crd) => rp.SKUNO == cs.SKUNO && cs.ID == rsr.SKU_ID && rsr.ROUTE_ID == crd.ROUTE_ID)
                .Where((rp, rsr, cs, crd) => rp.PACK_NO == Packno && crd.STATION_NAME == "DELIVERY").Select((rp, rsr, cs) => cs).Any();
            if (bol)
            {
                //bol = ole.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN,R_SN_STATION_DETAIL>((RP, RPG, RSP, RS,RSSD) => RP.ID == RPG.PARENT_PACK_ID && RPG.ID == RSP.PACK_ID && RSP.SN_ID == RS.ID&&RS.SN ==RSSD.SN)
                //       .Where((RP, RPG, RSP, RS,RSSD) => RP.PACK_NO == Packno && RS.VALID_FLAG == "1" &&RSSD.STATION_NAME == "DOUBLECHECK").Select((RP, RPG, RSP, RS,RSSD) => RS.SN ).Any();

                sql = $@"SELECT RS.SN
                              fROM R_PACKING           RP,
                                   R_PACKING           RPG,
                                   R_SN_PACKING        RSP,
                                   R_SN                RS
                             WHERE RP.ID = RPG.PARENT_PACK_ID
                               AND RPG.ID = RSP.PACK_ID
                               AND RSP.SN_ID = RS.ID
                               AND RP.PACK_NO = '{Packno}'
                               AND RS.VALID_FLAG = 1
                               AND RS.SN NOT IN (
                               SELECT RS.SN
                              fROM R_PACKING           RP,
                                   R_PACKING           RPG,
                                   R_SN_PACKING        RSP,
                                   R_SN                RS,
                                   R_SN_STATION_DETAIL RSSD
                             WHERE RP.ID = RPG.PARENT_PACK_ID
                               AND RPG.ID = RSP.PACK_ID
                               AND RSP.SN_ID = RS.ID
                               AND RS.SN = RSSD.SN
                               AND RP.PACK_NO = '{Packno}'
                               AND RS.VALID_FLAG = 1
                               AND RSSD.STATION_NAME = 'DOUBLECHECK'
                               )";

                if (ole.ExecSelect(sql).Tables[0].Rows.Count > 0)
                {
                    bol = true;
                }
            }

            return bol;
        }
        public static void XRayChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            //獲取 panel sn
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string panelSn = panelSession.InputValue.ToString();
            //獲取 RESULT
            MESStationSession result_Session = Station.StationSession.Find(t => t.MESDataType == "XRAYSESSION" && t.SessionKey == "1");
            if (result_Session == null || result_Session.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "XRAYSESSION" }));
            }
            string result = result_Session.Value.ToString();
            //獲取 REMARK
            MESStationSession remark_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (remark_Session == null || remark_Session.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            string remark = remark_Session.Value.ToString();
            //獲取工單
            WorkOrder wo = new WorkOrder();
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");
            if (Wo_Session != null)
            {
                if (Wo_Session.Value != null)
                {
                    wo = (WorkOrder)Wo_Session.Value;
                    if (wo.WorkorderNo == null || wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            // 插入掃XRAY記錄
            try
            {
                string xrayID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_XRAY_HEAD_HWD");
                //插入：R_XRAY_HEAD_HWD
                Station.SFCDB.ORM.Insertable(new R_XRAY_HEAD_HWD()
                {
                    ID = xrayID,
                    RESULT = result,
                    REMARK = remark,
                    EDIT_EMP = Station.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now
                }).ExecuteCommand();
                //插入 panelSN
                Station.SFCDB.ORM.Insertable(new R_XRAY_DETAIL_HWD()
                {
                    ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_XRAY_DETAIL_HWD"),
                    XRAYID = xrayID,
                    SNID = panelSn,
                    WO = wo.WorkorderNo,
                    STATION = Station.StationName,
                    LINE = Station.Line,
                    SKUNO = wo.SkuNO,
                    EDIT_EMP = Station.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now
                }).ExecuteCommand();
                // 工單解鎖
                if (result == "PASS")
                {
                    var s1 = Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == wo.WorkorderNo && t.LOCK_STATUS == "1" && t.LOCK_REASON == "MISSXRAYTEST").ToList();
                    if (s1.Count > 0)
                    {
                        var u = Station.SFCDB.ORM.Updateable<R_SN_LOCK>()
                            .UpdateColumns(t => new R_SN_LOCK
                            {
                                LOCK_STATUS = "0",
                                UNLOCK_REASON = remark,
                                UNLOCK_EMP = Station.LoginUser.EMP_NO,
                                UNLOCK_TIME = DateTime.Now
                            })
                            .Where(t => t.WORKORDERNO == wo.WorkorderNo && t.LOCK_REASON == "MISSXRAYTEST" && t.LOCK_STATUS == "1")
                            .ExecuteCommand();
                        Station.AddMessage("MES00000262", new string[] { "Unlocked !!" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage("Error：" + ex.Message);
            }
        }

        public static void HWDReplaceSNCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObj = (SN)snSession.Value;
            //bool isKPPartNo = Station.SFCDB.ORM.Queryable<C_KEYPART>().Where(r => r.PART_NO == snObj.SkuNo).Any();
            bool isKPPartNo = false;
            string sql = "";
            sql = $@"select * from C_KEYPART where PART_NO='{snObj.SkuNo}'";
            DataTable dt = Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                isKPPartNo = true;
            }
            if (!isKPPartNo)
            {
                isKPPartNo = Station.SFCDB.ORM.Queryable<C_KP_List_Item>().Where(r => r.KP_PARTNO == snObj.SkuNo).Any();
            }
            if (isKPPartNo)
            {
                throw new Exception($@"{snObj.SkuNo} Is A Keypart Part No.");
            }
            sql = $@"select * from r_sn_station_detail where sn='{snObj.SerialNo}' and station_name='BIP' 
                    and  (to_date(to_char(sysdate,'yyyy/mm/dd') ,'yyyy/mm/dd')-to_date(to_char(edit_time,'yyyy/mm/dd') ,'yyyy/mm/dd')) >50
                    and edit_time=(select max(edit_time) from r_sn_station_detail where sn='{snObj.SerialNo}' and station_name='BIP' )";
            dt = Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                throw new Exception($@"{snObj.SkuNo} Pass BIP Over 50 Day!");
            }

            sql = $@"select distinct sn from (select old_sn as sn from r_replace_sn where old_sn in (
                     select sn from r_sn where workorderno in (select workorderno from r_sn where sn='{snObj.SerialNo}'))
                     union
                     select new_sn as sn from r_replace_sn where new_sn in (select sn from r_sn where workorderno in (
                        select workorderno from r_sn where sn='{snObj.SerialNo}')))";
            dt = Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 30)
            {
                throw new Exception($@"{snObj.WorkorderNo} Already Replace SN Over 30 PCS!");
            }
        }

        /// <summary>
        /// HWD 替換次數的卡關由PE配置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWDNumberOfReplacementsChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string type = Paras[1].VALUE;
            var sessionObj = snSession.Value;
            int number = 1;
            SN snObj = null;
            R_SN r_sn = null;
            string sn;
            bool isExceeds;
            T_R_REPLACE_SN t_r_replace_sn = new T_R_REPLACE_SN(Station.SFCDB, Station.DBType);
            if (sessionObj is string)
            {
                sn = sessionObj.ToString();
            }
            else if (typeof(SN) == sessionObj.GetType())
            {
                snObj = (SN)sessionObj;
                sn = snObj.SerialNo;
            }
            else if (typeof(R_SN) == sessionObj.GetType())
            {
                r_sn = (R_SN)sessionObj;
                sn = r_sn.SN;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { sessionObj.ToString() }));
            }
            T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, Station.DBType);

            C_CONTROL control = null;
            if (type.ToUpper() == "NEW")
            {
                control = t_c_control.GetControlByName("REPLACE_TIME_NEW_SN", Station.SFCDB);
            }
            else if (type.ToUpper() == "OLD")
            {
                control = t_c_control.GetControlByName("REPLACE_TIME_OLD_SN", Station.SFCDB);
            }
            if (control != null && !string.IsNullOrEmpty(control.CONTROL_VALUE))
            {
                bool bSettingNumber = Int32.TryParse(control.CONTROL_VALUE, out number);
                number = bSettingNumber ? number : 1;
            }
            isExceeds = t_r_replace_sn.IsExceeds(sn, number, Station.SFCDB);
            int replaceCount = t_r_replace_sn.GetReplaceCount(sn, Station.SFCDB);
            if (!isExceeds)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180917114539", new string[] { sn, number.ToString(), replaceCount.ToString() }));
            }
        }

        /// <summary>
        /// 檢查SN當前的工站是否跟Action配置的一致，不一致則報錯
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CurrentStationEqualsParaChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSn = (SN)SNSession.Value;
            List<R_Station_Action_Para> listException = Paras.FindAll(t => t.SESSION_TYPE.Equals("CurrentStation"));
            List<string> exceptionStation = listException.Select(r => r.VALUE.Trim()).ToList<string>();
            if (exceptionStation.Count == 0)
            {
                return;
            }
            string currentSation = "";
            foreach (string s in exceptionStation)
            {
                currentSation = currentSation + s + ",";
            }
            if (!exceptionStation.Contains(ObjSn.CurrentStation))
            {
                SNSession.Value = null;
                throw new MESReturnMessage($@"CurrentStation[{ObjSn.CurrentStation}] Not In [{currentSation}]");
            }
        }

        /// <summary>
        /// 檢查SN的OBA測試記錄
        /// Netgear機種需要有Pass記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNObaTestRecordChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() }));
            }
            string msg = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                msg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" });
                throw new MESReturnMessage(msg);
            }
            string station = Paras[1].VALUE.ToString().Trim().ToUpper();

            SN snObj = (SN)SNSession.Value;
            string customerName = Station.SFCDB.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((sku, series, customer) => sku.C_SERIES_ID == series.ID && series.CUSTOMER_ID == customer.ID)
                .Where((sku, series, customer) => sku.SKUNO == snObj.SkuNo).Select((sku, series, customer) => customer.CUSTOMER_NAME).First();

            //檢查是否配置不需檢查有無OBA測試記錄
            var needCheck = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "NO_CHECK_OBATEST" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE == "NOSYSTEM")
                .Where(t => t.VALUE == snObj.SkuNo).Any();
            if ((customerName.Contains("NETGEAR") || customerName.Contains("ARUBA")) && !needCheck)
            {
                T_R_TEST_RECORD T_RTR = new T_R_TEST_RECORD(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                R_TEST_RECORD rtrObj = T_RTR.GetLastTestRecord(snObj.SerialNo, station, Station.SFCDB);
                if (rtrObj == null)
                {
                    throw new Exception($@"SN:{snObj.SerialNo} Has no Test {station}, Please Confirm!");
                }
                else if (!rtrObj.STATE.Contains("PASS"))
                {
                    throw new Exception($@"SN:{snObj.SerialNo} Has no Test {station} Pass, Please Confirm!");
                }
            }
        }

        public static void SmtChangeLineFAI(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SKU sku = (SKU)SNSession.Value;
            try
            {
                bool checker = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                    .Where(it => it.STATION_NAME == Station.StationName && it.LINE == Station.Line && it.LINE != "Line1" && it.SKUNO == sku.SkuNo)
                    .Any();

                if (!checker)
                {
                    UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "EmpNO", Tittle = "CFT FAI Check", Type = UIInputType.String, Name = "EmpNO", ErrMessage = "Cancel Check" };
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);

                    T_c_user t_c_user = new T_c_user(Station.SFCDB, Station.DBType);
                    Row_c_user rowSendUser = t_c_user.getC_Userbyempno(ret.ToString(), Station.SFCDB, Station.DBType);

                    if (rowSendUser == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { ret.ToString(), ret.ToString() }));
                    }

                    UIInputData O = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "PassWord", Tittle = "CFT FAI Check", Type = UIInputType.Password, Name = "PassWord", ErrMessage = "Cancel Check" };
                    O.OutInputs.Add(new DisplayOutPut() { Name = "EmpNO", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = ret.ToString() });
                    var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station);

                    Row_c_user rowSendUserandPass = t_c_user.getC_Userbyempnoandpass(ret.ToString(), Station.BU, ret1.ToString(), Station.SFCDB, Station.DBType);
                    if (rowSendUserandPass == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200706140440", new string[] { ret.ToString(), ret.ToString() }));
                    }
                    T_R_MES_LOG mesLog = new T_R_MES_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
                    SFCDB.BeginTrain();
                    rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, SFCDB);
                    rowMESLog.PROGRAM_NAME = "CloudMES";
                    rowMESLog.CLASS_NAME = "CheckSN";
                    rowMESLog.FUNCTION_NAME = "SmtChangeLineFAI";
                    rowMESLog.LOG_MESSAGE = "首件確認工號:" + ret + ";線體:" + Station.Line + ";工站:" + Station.StationName + ";機種:" + sku.SkuNo;
                    rowMESLog.LOG_SQL = "";
                    rowMESLog.EDIT_EMP = "OracleStation";
                    rowMESLog.EDIT_TIME = System.DateTime.Now;
                    rowMESLog.DATA1 = sku.SkuNo;
                    rowMESLog.DATA2 = "";
                    SFCDB.ThrowSqlExeception = true;
                    SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                    SFCDB.CommitTrain();

                }

            }

            catch (Exception e)
            {
                throw e;
            }
            finally { }
        }

        public static void SkuVerChangeFAI(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            bool checker = true;

            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            try
            {
                WorkOrder wo = (WorkOrder)SNSession.Value;
                checker = SFCDB.ORM.Queryable<R_FAI>()
                    .Where(it => it.FAITYPE == "SkuVerChange"
                    && it.SKUNO == wo.SkuNO
                    && it.SKU_VER == wo.SKU_VER
                    && it.STATUS == "1"
                    && it.REMARK == Station.StationName)
                    .Any();

                if (!checker)
                {
                    UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "EmpNO", Tittle = "SkuVer Change FAI", Type = UIInputType.String, Name = "EmpNO", ErrMessage = "Cancel Check" };
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);

                    T_c_user t_c_user = new T_c_user(Station.SFCDB, Station.DBType);
                    Row_c_user rowSendUser = t_c_user.getC_Userbyempno(ret.ToString(), Station.SFCDB, Station.DBType);

                    if (rowSendUser == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { ret.ToString(), ret.ToString() }));
                    }

                    UIInputData O = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "PassWord", Tittle = "SkuVer Change FAI", Type = UIInputType.Password, Name = "PassWord", ErrMessage = "Cancel Check" };
                    O.OutInputs.Add(new DisplayOutPut() { Name = "EmpNO", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = ret.ToString() });
                    var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station);

                    Row_c_user rowSendUserandPass = t_c_user.getC_Userbyempnoandpass(ret.ToString(), Station.BU, ret1.ToString(), Station.SFCDB, Station.DBType);
                    if (rowSendUserandPass == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200706140440", new string[] { ret.ToString(), ret.ToString() }));
                    }

                    T_R_FAI trfai = new T_R_FAI(SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_FAI rowRFAI = (Row_R_FAI)trfai.NewRow();
                    SFCDB.BeginTrain();
                    rowRFAI.ID = trfai.GetNewID(Station.LoginUser.BU, SFCDB);
                    rowRFAI.FAITYPE = "SkuVerChange";
                    rowRFAI.SKUNO = wo.SkuNO;
                    rowRFAI.SKU_VER = wo.SKU_VER;
                    rowRFAI.STATUS = "1";
                    rowRFAI.REMARK = Station.StationName;
                    rowRFAI.CREATETIME = Station.GetDBDateTime();
                    rowRFAI.CREATEBY = Station.LoginUser.EMP_NO;
                    rowRFAI.EDITTIME = Station.GetDBDateTime();
                    rowRFAI.EDITBY = ret.ToString();

                    SFCDB.ThrowSqlExeception = true;
                    SFCDB.ExecSQL(rowRFAI.GetInsertString(DB_TYPE_ENUM.Oracle));
                    SFCDB.CommitTrain();
                }

            }

            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// LLT抽測
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSNisLLTTest(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            try
            {
                var sn = SNSession.Value;
                SN snObj = (SN)SNSession.Value;
                var sku = snObj.SkuNo;
                // Count 為在第2500的倍數+1 PCS
                // X 為定值2500
                // Y 為2500的倍數+1 PCS 除以2500的小數點 
                //int Count;
                double x = 2500;
                double z = 0.0004;

                if (!sku.Contains("VT0120"))
                {
                    T_R_LLT Trllt = new T_R_LLT(SFCDB, DB_TYPE_ENUM.Oracle);
                    int Count = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Count((r) => r.SKUNO == sku && r.STATION_NAME == Station.StationName && r.VALID_FLAG == "1");
                    //int total = Count;
                    //Count = Count + 1;
                    double v = Count / x;
                    //// W 為 暫定30，就是最大工單數量30*2500=75000 PCS，若工單數量大於這個數，則需要再在W這個數組內加數量
                    //int[] w = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
                    var xx = (int)Math.Round((double)Count / 2500) + 1;
                    for (int yy = 1; yy < xx + 1; yy++)
                    {
                        if (v == yy + z)
                        {
                            Row_R_LLT RowLLT = (Row_R_LLT)Trllt.NewRow();
                            SFCDB.BeginTrain();
                            RowLLT.ID = Trllt.GetNewID(Station.LoginUser.BU, SFCDB);
                            RowLLT.R_SN_ID = snObj.ID;
                            RowLLT.SN = sn.ToString();
                            RowLLT.STATUS = "0";//0="掃進",1="測試中",2="掃出",3="取消",
                            RowLLT.STATION = "LLT";
                            RowLLT.SUPPLEMENT = "N";
                            RowLLT.INTIME = System.DateTime.Now;
                            RowLLT.CREATETIME = System.DateTime.Now;

                            SFCDB.ThrowSqlExeception = true;
                            SFCDB.ExecSQL(RowLLT.GetInsertString(DB_TYPE_ENUM.Oracle));
                            SFCDB.CommitTrain();
                            return;
                        }

                        //增加取消抽測之後補抽邏輯

                        //計算取消抽中的SN數量
                        int CTotal = Station.SFCDB.ORM.Queryable<R_LLT, R_SN>((RL, RS) => RL.R_SN_ID == RS.ID).Where((RL, RS) => RS.SKUNO == sku && RL.STATUS == "3").Select((RL, RS) => RL.SN).ToList().Count;
                        //計算補抽的數量
                        int STotal = Station.SFCDB.ORM.Queryable<R_LLT, R_SN>((RL, RS) => RL.R_SN_ID == RS.ID).Where((RL, RS) => RS.SKUNO == sku && RL.SUPPLEMENT == "Y").Select((RL, RS) => RL.SN).ToList().Count;
                        if (CTotal > 0)//只有發現有取消過記錄才會往這裡跑
                        {
                            if (CTotal != STotal)
                            {
                                Row_R_LLT RowLLT = (Row_R_LLT)Trllt.NewRow();
                                SFCDB.BeginTrain();
                                RowLLT.ID = Trllt.GetNewID(Station.LoginUser.BU, SFCDB);
                                RowLLT.R_SN_ID = snObj.ID;
                                RowLLT.SN = sn.ToString();
                                RowLLT.STATUS = "0";//0="掃進",1="測試中",2="掃出",3="取消",
                                RowLLT.STATION = "LLT";
                                RowLLT.SUPPLEMENT = "Y";
                                RowLLT.INTIME = System.DateTime.Now;
                                RowLLT.CREATETIME = System.DateTime.Now;

                                SFCDB.ThrowSqlExeception = true;
                                SFCDB.ExecSQL(RowLLT.GetInsertString(DB_TYPE_ENUM.Oracle));
                                SFCDB.CommitTrain();
                                return;
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// LLT掃進掃出狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LLTStatusCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            try
            {
                SN snObj = (SN)SNSession.Value;
                //string sn = SNSession.Value.ToString();
                var sn = snObj.SerialNo;
                //只有取消或者掃出的SN才能過站
                var STotal = Station.SFCDB.ORM.Queryable<R_LLT>().Where(RL => RL.SN == sn && (RL.STATUS == "0" || RL.STATUS == "1")).ToList().FirstOrDefault();
                if (STotal != null)
                {
                    throw new Exception($@"SN:{sn} Is LLT test,Pls Check!");

                }
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public static void LastJobStockinStationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObj = (SN)sessionSN.Value;
            C_ROUTE_DETAIL stock_station = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>()
                .Where(r => r.ROUTE_ID == snObj.RouteID && r.STATION_TYPE == "JOBSTOCKIN").OrderBy(r => r.SEQ_NO, SqlSugar.OrderByType.Desc)
                .ToList().FirstOrDefault();
            if (stock_station == null)
            {
                throw new Exception("This SN Route Not Have JOBSTOCKIN");
            }
            R_SN_STATION_DETAIL last_stock_station = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                .Where(r => r.SN == snObj.SerialNo && r.STATION_NAME == stock_station.STATION_NAME)
                .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (last_stock_station == null)
            {
                throw new Exception($@"Not Pass The JOBSTOCKIN Statioin[{stock_station.STATION_NAME}]");
            }
            if (last_stock_station.REPAIR_FAILED_FLAG == "1")
            {
                throw new Exception("The Last JOBSTOCKIN Station Is Fail!");
            }

        }

        /// <summary>
        /// SE機種有充放電抽測邏輯，被抽測到的SN需要檢查有無放電測試記錄(充電沒有測試記錄)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNChargeRecordChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //獲取SN Session
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }

            //獲取SN 對象
            SN snObj = null;
            if (SNSession.Value.GetType() == typeof(SN))
            {
                snObj = (SN)SNSession.Value;
            }
            else
            {
                snObj = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            //獲取SN所有LOG
            T_R_SN_LOG _LOG = new T_R_SN_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_LOG> rsLogs = _LOG.GetLogBySN(Station.SFCDB, snObj.SerialNo);
            if (rsLogs != null && rsLogs.Count > 0)
            {
                //檢查SN是否被抽測充放電LOG
                R_SN_LOG chargeLog = rsLogs.Find(t => t.LOGTYPE == "CHARGE-SAMPLE" && t.FLAG == "Y");
                if (chargeLog != null)
                {
                    //如果SN被抽測充放電，則檢查是否有充放電測試記錄(充電、放電工站為線外測試工站，且充電無測試記錄，放電有測試記錄且TE傳工站名為AQL)                    
                    T_R_TEST_RECORD _RECORD = new T_R_TEST_RECORD(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    bool hasCharge = _RECORD.CheckTestRecord(snObj.SerialNo, "AQL", Station.SFCDB);
                    if (!hasCharge)
                    {
                        //throw new Exception($@"SN:{snObj.SerialNo} 被抽測充放電但沒有放電測試記錄,請確認!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164119", new string[] { snObj.SerialNo }));

                    }
                }
            }
        }

        /// <summary>
        /// 檢查條碼是否是由KITTING列印
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNKitPrintLabelChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            //獲取SN Session
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            string WO = sessionWO.Value.ToString();

            string sn = SNSession.Value.ToString();

            string sku = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO).Select(t => t.SKUNO).First().ToString();


            List<R_F_CONTROL> rfc = new List<R_F_CONTROL>();
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            rfc = t_r_function_control.GetListByFcv("SkipFunction", "SkipKitprintSNChecker", sku, Station.SFCDB);

            if (rfc.Count() == 0)
            {
                var sds = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.WORKORDERNO == WO && t.SN == sn && t.STATION_NAME == "KIT_PRINT").ToList().FirstOrDefault();

                if (sds == null)
                {
                    //throw new Exception($@"SN:{sn} 不是KIT打印的條碼(或工單不正確),請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164234", new string[] { sn }));

                }
            }


        }
        public static void CheckRSnLinkExists(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            //獲取SN Session
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }

            string sn = SNSession.Value.ToString();

            if (Station.SFCDB.ORM.Queryable<R_SN_LINK>().Where(t => t.CSN == sn && t.VALIDFLAG == "1").Count() > 0)
            {
                throw new Exception($@"SN:{sn} has been Loading(r_sn_link), please check!");
            }
        }
        public static void CheckSNKeypartStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            //獲取SN Session
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }

            R_SN snObj = null;
            if (SNSession.Value is string)
            {
                snObj = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == SNSession.Value.ToString() && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (snObj == null)
                {
                    throw new Exception($@"{SNSession.Value.ToString()} Not Exists!");
                }
            }
            else if (SNSession.Value is SN)
            {
                string sn = ((SN)SNSession.Value).SerialNo;
                snObj = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == sn && r.VALID_FLAG == "1").ToList().FirstOrDefault();
            }
            else if (SNSession.Value is R_SN)
            {
                snObj = ((R_SN)SNSession.Value);
            }
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            List<R_SN_KP> listKP = new List<R_SN_KP>();
            t_r_sn_kp.GetSnKP(Station.SFCDB, snObj.SN, listKP);

            var selfKp = listKP.FindAll(r => r.SN == snObj.SN);
            var kpkp = listKP.FindAll(r => r.SN != snObj.SN);
            if (selfKp.Count > 0)
            {
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                var lastStationList = t_c_route_detail.GetLastStations(snObj.ROUTE_ID, snObj.CURRENT_STATION, Station.SFCDB).Select(r => r.STATION_NAME);
                foreach (var kp in selfKp)
                {
                    if (string.IsNullOrEmpty(kp.VALUE) && lastStationList.Contains(kp.STATION))
                    {
                        throw new Exception($@"Parent SN {kp.SN} Missing Keypart In {kp.STATION},{kp.SCANTYPE},{kp.KP_NAME}!");
                    }
                    R_SN valueSN = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == kp.VALUE && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                    if (valueSN != null)
                    {
                        if (valueSN.NEXT_STATION != "JOBFINISH")
                        {
                            throw new Exception($@"Keypart SN {kp.VALUE} Not JOBFINISH,Next Station Is {valueSN.NEXT_STATION}!");
                        }
                        var kl = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.VALUE == kp.VALUE && r.VALID_FLAG == 1).ToList();
                        if (kl.Count > 1)
                        {
                            throw new Exception($@"{kp.VALUE} Repeat Binding!");
                        }
                    }
                }
            }
            if (kpkp.Count > 0)
            {
                foreach (var k in kpkp)
                {
                    if (string.IsNullOrEmpty(k.VALUE))
                    {
                        throw new Exception($@"Keypart SN {k.SN} Missing Keypart In {k.STATION},{k.SCANTYPE},{k.KP_NAME}!");
                    }
                    R_SN valueSN = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == k.VALUE && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                    if (valueSN != null)
                    {
                        if (valueSN.NEXT_STATION != "JOBFINISH")
                        {
                            throw new Exception($@"Keypart SN {k.VALUE} Not JOBFINISH,Next Station Is {valueSN.NEXT_STATION}!");
                        }
                        var kl = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.VALUE == k.VALUE && r.VALID_FLAG == 1).ToList();
                        if (kl.Count > 1)
                        {
                            throw new Exception($@"{k.VALUE} Repeat Binding!");
                        }
                    }
                }
            }
        }

        public static void JuniperDoubleCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (skuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKU" }));
            }
            MESStationSession packTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (packTypeSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "PackType" }));
            }
            SN snObj = (SN)SNSession.Value;
            SKU skuObj = (SKU)skuSession.Value;
            string packType = packTypeSession.Value.ToString();
            DateTime sysdate = Station.SFCDB.ORM.GetDate();
            T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
            R_SN_LOG check_log = null;
            T_R_FUNCTION_CONTROL contrl = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            List<R_F_CONTROL> check_sn_list = contrl.GetListByFcv("JuniperDoubleCheck", "CheckSN", skuObj.SkuNo, Station.StationName, Station.SFCDB);
            //var packType2 = Station.SFCDB.ORM.Queryable<R_SN, O_ORDER_MAIN>((r, o) => r.WORKORDERNO == o.PREWO).Where((r, o) => r.SN == snObj.SerialNo && o.OFFERINGTYPE == "Fixed Nonstockable Bundle").Select((r, o) => o).ToList();
            //var cs = Station.SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_SN>((C, K, S) => C.ID == K.C_SERIES_ID && K.SKUNO == S.SKUNO)
            //        .Where((C, K, S) => S.SN == snObj.SerialNo)
            //        .Select((C, K, S) => C)
            //        .First();
            //if (check_sn_list.Count > 0 && packType == "Single pack" && cs.SERIES_NAME == "Juniper-Optics")
            if (check_sn_list.Count > 0 && packType == "Single pack")
            {
                UIInputData O = new UIInputData() { };
                O.Timeout = 3000000;
                O.IconType = IconType.Warning;
                O.Type = UIInputType.String;
                O.Tittle = $@"DoubleCheck";
                O.ErrMessage = "No input";
                O.Message = "SN";
                O.Name = "SN";
                O.UIArea = new string[] { "40%", "70%" };
                O.OutInputs.Add(new DisplayOutPut
                {
                    Name = "SKUNO",
                    Value = snObj.SkuNo,
                    DisplayType = UIOutputType.Text.ToString()
                });
                while (true)
                {
                    var input_sn = O.GetUiInput(Station.API, UIInput.CanPrint, Station);
                    Station.LabelPrint.Clear();
                    Station.LabelPrints.Clear();
                    Station.LabelStillPrint.Clear();
                    if (input_sn == null)
                    {
                        O.CBMessage = "Please Scan SN";
                    }
                    else
                    {
                        string check_sn = input_sn.ToString();
                        if (string.IsNullOrEmpty(check_sn))
                        {
                            O.CBMessage = "Please Scan SN";
                        }
                        else if (!check_sn.StartsWith("S"))
                        {
                            O.CBMessage = $@"{check_sn} Not Starts With S";
                        }
                        else if (check_sn.Substring(1) != snObj.SerialNo)
                        {
                            O.CBMessage = $@"{check_sn}  Is Not Pass SN";
                        }
                        else if (check_sn.Equals("No input"))
                        {
                            throw new Exception("User Cancel");
                        }
                        else
                        {
                            check_log = new R_SN_LOG();
                            check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                            check_log.SNID = snObj.ID;
                            check_log.SN = snObj.SerialNo;
                            check_log.LOGTYPE = "JuniperDoubleCheck";
                            check_log.DATA1 = "CheckSN";
                            check_log.DATA2 = snObj.SkuNo;
                            check_log.DATA3 = check_sn;
                            check_log.DATA4 = Station.StationName;
                            check_log.FLAG = "1";
                            check_log.CREATETIME = sysdate;
                            check_log.CREATEBY = Station.LoginUser.EMP_NO;
                            Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                            break;
                        }
                    }
                }
            }
            List<R_F_CONTROL> check_kp_list = contrl.GetListByFcv("JuniperDoubleCheck", "CheckKeypart", skuObj.SkuNo, Station.StationName, Station.SFCDB);
            foreach (var f in check_kp_list)
            {
                var parno_ex = Station.SFCDB.ORM.Queryable<R_F_CONTROL_EX>().Where(r => r.DETAIL_ID == f.ID && r.NAME == "PARTNO").ToList().FirstOrDefault();
                if (parno_ex == null)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(parno_ex.VALUE))
                {
                    continue;
                }
                var packType_ex = Station.SFCDB.ORM.Queryable<R_F_CONTROL_EX>().Where(r => r.DETAIL_ID == f.ID && r.NAME == "PACK_TYPE" && SqlSugar.SqlFunc.Trim(r.VALUE) == packType)
                    .ToList().FirstOrDefault();
                if (packType_ex == null)
                {
                    continue;
                }
                var list_kp = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == snObj.SerialNo && r.VALID_FLAG == 1 && r.PARTNO == parno_ex.VALUE)
                    .OrderBy(r => r.VALUE, SqlSugar.OrderByType.Asc).ToList();
                if (list_kp.Count == 0)
                {
                    throw new Exception($@"{parno_ex.VALUE} not exists in keypart,please call QE and PE to check keypart and double check setting.");
                }
                UIInputData O = new UIInputData() { };
                O.Timeout = 3000000;
                O.IconType = IconType.Warning;
                O.Type = UIInputType.String;
                O.Tittle = "DoubleCheck";
                O.ErrMessage = "No input";
                O.UIArea = new string[] { "40%", "70%" };
                foreach (var k in list_kp)
                {
                    O.OutInputs.Clear();
                    O.Message = "Keypart";
                    O.Name = "Value";
                    O.CBMessage = "";
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "PARTNO",
                        Value = k.PARTNO,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "KP_NAME",
                        Value = k.KP_NAME,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "MPN",
                        Value = k.MPN,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "SCANTYPE",
                        Value = k.SCANTYPE,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    while (true)
                    {
                        var input_sn = O.GetUiInput(Station.API, UIInput.CanPrint, Station);
                        Station.LabelPrint.Clear();
                        Station.LabelPrints.Clear();
                        Station.LabelStillPrint.Clear();
                        if (input_sn == null)
                        {
                            O.CBMessage = $@"Please Scan {k.PARTNO}";
                        }
                        else
                        {
                            string check_sn = input_sn.ToString().Trim();
                            if (string.IsNullOrEmpty(check_sn))
                            {
                                O.CBMessage = $@"Please Scan {k.PARTNO}";
                            }
                            else if (!check_sn.Equals(k.VALUE))
                            {
                                O.CBMessage = $@"{check_sn} not exists in keypart";
                            }
                            else if (check_sn.Equals("No input"))
                            {
                                throw new Exception("User Cancel");
                            }
                            else
                            {
                                check_log = new R_SN_LOG();
                                check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                                check_log.SNID = snObj.ID;
                                check_log.SN = snObj.SerialNo;
                                check_log.LOGTYPE = "JuniperDoubleCheck";
                                check_log.DATA1 = "CheckKeypart";
                                check_log.DATA2 = k.PARTNO;
                                check_log.DATA3 = check_sn;
                                check_log.DATA4 = Station.StationName;
                                check_log.FLAG = "1";
                                check_log.CREATETIME = sysdate;
                                check_log.CREATEBY = Station.LoginUser.EMP_NO;
                                Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void RepairCheckOutDoubleCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN snObj = (SN)SNSession.Value;

            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            List<R_SN_KP> list_kp = new List<R_SN_KP>();
            t_r_sn_kp.GetSnKP(Station.SFCDB, snObj.SerialNo, list_kp);
            List<R_SN_KP> list_pcba = list_kp.FindAll(r => r.SCANTYPE.StartsWith("PCBA"));
            DateTime sysdate = Station.SFCDB.ORM.GetDate();
            T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
            R_SN_LOG check_log = null;
            foreach (var k in list_pcba)
            {
                UIInputData O = new UIInputData() { };
                O.Timeout = 3000000;
                O.IconType = IconType.Warning;
                O.Type = UIInputType.String;
                O.Tittle = "RepairCheckOutDoubleCheck";
                O.ErrMessage = "No input";
                O.UIArea = new string[] { "40%", "70%" };
                O.OutInputs.Clear();
                O.Message = "Keypart";
                O.Name = "Value";
                O.CBMessage = "";
                O.OutInputs.Add(new DisplayOutPut
                {
                    Name = "PARTNO",
                    Value = k.PARTNO,
                    DisplayType = UIOutputType.Text.ToString()
                });
                O.OutInputs.Add(new DisplayOutPut
                {
                    Name = "KP_NAME",
                    Value = k.KP_NAME,
                    DisplayType = UIOutputType.Text.ToString()
                });
                O.OutInputs.Add(new DisplayOutPut
                {
                    Name = "MPN",
                    Value = k.MPN,
                    DisplayType = UIOutputType.Text.ToString()
                });
                O.OutInputs.Add(new DisplayOutPut
                {
                    Name = "SCANTYPE",
                    Value = k.SCANTYPE,
                    DisplayType = UIOutputType.Text.ToString()
                });
                while (true)
                {
                    var input_sn = O.GetUiInput(Station.API, UIInput.Normal, Station);
                    if (input_sn == null)
                    {
                        O.CBMessage = $@"Please Scan {k.PARTNO}";
                    }
                    else
                    {
                        string check_sn = input_sn.ToString().Trim();
                        if (string.IsNullOrEmpty(check_sn))
                        {
                            O.CBMessage = $@"Please Scan {k.PARTNO}";
                        }
                        else if (!check_sn.Equals(k.VALUE))
                        {
                            O.CBMessage = $@"{check_sn} not exists in keypart";
                        }
                        else if (check_sn.Equals("No input"))
                        {
                            throw new Exception("User Cancel");
                        }
                        else
                        {
                            check_log = new R_SN_LOG();
                            check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                            check_log.SNID = snObj.ID;
                            check_log.SN = snObj.SerialNo;
                            check_log.LOGTYPE = "RepairCheckOut";
                            check_log.DATA1 = "CheckPCBA";
                            check_log.DATA2 = k.PARTNO;
                            check_log.DATA3 = check_sn;
                            check_log.DATA4 = k.KP_NAME;
                            check_log.DATA5 = k.SCANTYPE;
                            check_log.DATA6 = k.STATION;
                            check_log.FLAG = "1";
                            check_log.CREATETIME = sysdate;
                            check_log.CREATEBY = Station.LoginUser.EMP_NO;
                            Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                            break;
                        }
                    }
                }
            }
        }

        public static void SilverRotationCheckInChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN snObj = (SN)snSession.Value;
            T_R_SILVER_ROTATION t_r_silver_rotation = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            var rotation = t_r_silver_rotation.GetRotationBySN(snObj.SerialNo, Station.SFCDB);
            if (rotation != null)
            {
                throw new Exception($@"{snObj.SerialNo} Already Check In Silver Rotation System!");
            }
            if (snObj.CompletedFlag.Equals("0"))
            {
                throw new Exception($@"{snObj.SerialNo} Not Complete!");
            }
        }
        public static void SilverRotationCheckOutChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN snObj = (SN)snSession.Value;
            T_R_SILVER_ROTATION t_r_silver_rotation = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            var rotation = t_r_silver_rotation.GetRotationBySN(snObj.SerialNo, Station.SFCDB);
            if (rotation == null)
            {
                throw new Exception($@"{snObj.SerialNo} Not In Silver Rotation System!");
            }
            if (rotation.ROTATION_FLAG.Equals(1))
            {
                throw new Exception($@"{snObj.SerialNo} Is Testing In Silver Rotation System!");
            }
        }

        public static void SilverRotationSNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN snObj = (SN)snSession.Value;
            T_R_SILVER_ROTATION t_r_silver_rotation = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            var rotation = t_r_silver_rotation.GetRotationBySN(snObj.SerialNo, Station.SFCDB);
            if (rotation != null)
            {
                //1. check out checker
                if (rotation.STATUS.Equals(0))
                {
                    throw new MESReturnMessage($@"{snObj.SerialNo} is in Silver Rotation System,please check out!");
                }
                //2. test record
                //1)Interface card Process Routing lasted RIT test record must be pass
                //2)Chassis base Process Routing  lasted SAT test record must be pass

            }
        }
        public static void JuniperOpticsMatlLinkChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (skuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKU" }));
            }
            string inputSn = snSession.Value.ToString();
            SKU skuObj = (SKU)skuSession.Value;
            var Series = Station.SFCDB.ORM.Queryable<C_SKU, C_SERIES>((cs, css) => cs.C_SERIES_ID == css.ID).Where((cs, css) => cs.SKUNO == skuObj.SkuNo).Select((cs, css) => css).ToList();
            if (Series[0].SERIES_NAME == "Juniper-Optics")
            {
                var Matl_link = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == inputSn && t.STATION == "MATL_LINK" && t.VALID_FLAG == 1).ToList();
                if (Matl_link.Count == 0)
                {
                    throw new MESReturnMessage($@"Juniper-Optics{skuObj.SkuNo} is SN {inputSn}  has not scanned the MATL_LINK .Please Scan the MATL_LINK first before operation!");
                }
            }

        }

        public static void MaterialLockChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null || WoSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string wo = WoSession.Value.ToString().Trim();
            //检查物料被鎖
            var PnLock = Station.SFCDB.ORM.Queryable<R_SN_LOCK, O_ORDER_MAIN, O_ORDER_OPTION>
                ((SL, OM,  OO) => SL.WORKORDERNO == OO.FOXPN  && OM.ID==OO.MAINID)
                .Where((SL, OM, OO) => OM.PREWO == wo
                && (SL.LOCK_STATION == Station.StationName || SL.LOCK_STATION == "ALL")
                && SL.LOCK_STATUS == "1"
                && SL.TYPE == "PN"
                ).Distinct().Select((SL, OM, OO) => new {
                    LOCK_REASON = SL.LOCK_REASON,
                    LOCK_EMP = SL.LOCK_EMP,
                    MaterialNo = SL.WORKORDERNO
                }).ToList();
            for (int j = 0; j < PnLock.Count(); j++)
            {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { PnLock[j].MaterialNo, PnLock[j].LOCK_EMP, PnLock[j].LOCK_REASON }));
            }

        }

        public static void SNLoadingChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn;
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            sn = Input.Value.ToString().Trim().ToUpper();
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null)
            {
                snSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                snSession.Value = sn;
                snSession.ResetInput = Input;
                snSession.InputValue = sn;
                Station.StationSession.Add(snSession);
            }
            else
            {
                snSession.Value = sn;
                snSession.ResetInput = Input;
                snSession.InputValue = sn;
            }
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string wo = "";
            string skuno = "";

            if (woSession.Value is WorkOrder)
            {
                skuno = ((WorkOrder)woSession.Value).SkuNO;
            }
            else
            {
                wo = woSession.Value.ToString().Trim();
                var woObj = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == wo).ToList().FirstOrDefault();
                if (woObj == null)
                {
                    throw new MESReturnMessage($"{wo} not exist.");
                }
                skuno = woObj.SKUNO;
            }

            if (Station.BU.Contains("FJZ") || Station.BU.Contains("FVN"))
            {
                // Check if sn has not OUT_SILVER_WIP
                var InSw = Station.SFCDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SN == sn && t.STATE_FLAG == "1").First();
                if (InSw != null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211105151002", new string[] { sn }));
                }
                // Check if sn has not SUPERMARKET_OUT
                var rsn = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
                if (rsn != null)
                {
                    var InSM = Station.SFCDB.ORM.Queryable<R_SUPERMARKET>().Where(t => t.R_SN_ID == rsn.ID && t.STATUS == "1").First();
                    if (InSM != null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20220201160325", new string[] { sn }));
                    }
                }
            }

            T_R_SN tr_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            var snObj = t_r_sn.LoadSN(sn, Station.SFCDB);
            if (snObj != null)
            {
                wo = woSession.Value.ToString().Trim();
                var ObjSku = Station.SFCDB.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == skuno).ToList().FirstOrDefault();
                var Snobjworkorder = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == wo).ToList().FirstOrDefault();
                var Oldworkorder = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == snObj.WORKORDERNO).ToList().FirstOrDefault();
                T_R_LINK_CONTROL t_r_link_control = new T_R_LINK_CONTROL(Station.SFCDB, Station.DBType);
                var linkObj = t_r_link_control.GetControlList("SKU", skuno, null, snObj.SKUNO, null, "LOADING_KEEP_SN", Station.SFCDB);
                if (ObjSku.SKU_TYPE == "PCBA" || ObjSku.SKU_TYPE == "MODEL")
                {
                    linkObj = t_r_link_control.GetControlList("SKU", skuno, Snobjworkorder.SKU_VER, snObj.SKUNO, Oldworkorder.SKU_VER, "LOADING_KEEP_SN", Station.SFCDB);
                }
                if (linkObj.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES0000156", new string[] { sn }));
                }
                else
                {
                    var locks = LockManager.CheckLock(snObj.SN, "SN", Station.StationName, Station.SFCDB);
                    if (locks.Count > 0)
                    {
                        string message_lock = null;

                        foreach (R_SN_LOCK item in locks)
                        {
                            var emp_lock_info = Station.SFCDB.ORM.Queryable<C_USER>().Where(u => u.EMP_NO == item.LOCK_EMP).First();

                            string emp_lock = emp_lock_info != null ? emp_lock_info.DPT_NAME.Trim().ToUpper() + ": " + emp_lock_info.EMP_NAME.Trim().ToUpper() : item.LOCK_EMP;

                            if (!string.IsNullOrEmpty(message_lock))
                                message_lock += "; \n";

                            message_lock += MESReturnMessage.GetMESReturnMessage("MSGCODE20210628172337", new string[] { item.TYPE, item.TYPE == "SN" ? item.SN : item.WORKORDERNO, emp_lock, item.LOCK_REASON });

                        }

                        throw new MESReturnMessage(message_lock);
                    }

                    if (!string.IsNullOrEmpty(snObj.SHIPPED_FLAG) && snObj.SHIPPED_FLAG.Equals("1"))
                    {
                        //throw new MESReturnMessage($"{snObj.SN} already shipped.");

                        if (Station.BU == "VNJUNIPER")
                        {
                            //檢查本階和上階是否有實際出貨記錄
                            var isShipped = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(t => t.SN == snObj.SN).Any();
                            var linkSnShipDetail = Station.SFCDB.ORM.Queryable<R_SN_KP, R_SHIP_DETAIL>((sk, sd) => sk.SN == sd.SN).Where((sk, sd) => sk.VALID_FLAG == 1 && sk.VALUE == snObj.SN).Select((sk, sd) => new { sd }).ToList();
                            if (isShipped || linkSnShipDetail.Count > 0)
                            {
                                throw new MESReturnMessage($"{snObj.SN} already shipped.");
                            }
                            //VN存在已經被綁定(EBAA****被RA***綁定)但需要空機構出貨的情況, 客人要求使用EBAA****類型的SN傳送I054, 因此可不卡SHIPFLAG=1 Asked By PE譚義康 2021-11-13
                            var linkControlObj = t_r_link_control.GetControlList("SKU", skuno, null, snObj.SKUNO, null, "LOADING_NOCHECK_SHIPFLAG", Station.SFCDB);
                            if (linkControlObj.Count == 0)
                            {
                                throw new MESReturnMessage($"{snObj.SN} already shipped.");
                            }
                        }
                        else 
                        {
                            throw new MESReturnMessage($"{snObj.SN} already shipped.");
                        }
                    }
                    if (!string.IsNullOrEmpty(snObj.SCRAPED_FLAG) && snObj.SCRAPED_FLAG.Equals("1"))
                    {
                        throw new MESReturnMessage($"{snObj.SN} already scraped.");
                    }
                    if (!string.IsNullOrEmpty(snObj.REPAIR_FAILED_FLAG) && snObj.REPAIR_FAILED_FLAG.Equals("1"))
                    {
                        throw new MESReturnMessage($"{snObj.SN} wait to repair.");
                    }
                    if (!string.IsNullOrEmpty(snObj.COMPLETED_FLAG) && !snObj.COMPLETED_FLAG.ToString().Equals("1"))
                    {
                        throw new MESReturnMessage($"{snObj.SN} not finish.");
                    }
                    if (snObj.NEXT_STATION.ToUpper().Equals("REWORK".ToUpper()))
                    {
                        throw new MESReturnMessage($"{snObj.SN} next station is rework.");
                    }
                }
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }

        }

        /// <summary>
        /// 檢查工站過站間隔時間
        /// </summary>
        public static void StationIntervalsChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SN snObj = (SN)snSession.Value;
            string station = Paras[1].VALUE.ToString();//間隔工站
            string intervals = Paras[2].VALUE.ToString();//間隔時間
            try
            {
                double intervalHours = double.Parse(intervals);
            }
            catch (Exception ex)
            {
                //throw new Exception("間隔時間格式錯誤!" + ex.Message);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164341", new string[] { ex.Message }));

            }

            //判斷過站SN對應機種是否配置卡關
            string partNo = Station.SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(c => c.FUNCTIONNAME == "Intervals_Hours" && c.CONTROLFLAG == "Y" && c.FUNCTIONTYPE == "NOSYSTEM")
                .Where(c => c.VALUE == snObj.SkuNo).Select(c => c.EXTVAL).ToList().FirstOrDefault();
            if (!string.IsNullOrEmpty(partNo))
            {
                //判斷過站SN是否綁定了卡關中配置的PartNo以及過站記錄是否包含需要檢查的工站
                var detailObj = Station.SFCDB.ORM.Queryable<R_SN_KP, R_SN_STATION_DETAIL>((k, d) => k.VALUE == d.SN)
                    .Where((k, d) => k.VALID_FLAG == 1 && k.SN == snObj.SerialNo && k.PARTNO == partNo)
                    .Where((k, d) => d.STATION_NAME == station).OrderBy((k, d) => d.EDIT_TIME, SqlSugar.OrderByType.Desc)
                    .Select((k, d) => d).ToList().FirstOrDefault();
                if (detailObj != null)
                {
                    DateTime editTime = (DateTime)detailObj.EDIT_TIME;
                    //如果當前時間 - 過站時間 < 需要間隔的時間(小時)
                    if ((DateTime.Now - editTime).TotalHours < double.Parse(intervals))
                    {
                        //throw new Exception($@"距離綁定的 {partNo} 過 {station} 工站時間不足 {intervals} 小時!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164412", new string[] { partNo, station, intervals }));

                    }
                }
            }
        }

        /// <summary>
        /// SNList StatusFlag Checker
        /// 檢查指定SN Flag 
        /// 傳入一個 SNList 對象
        /// </summary>
        public static void SNListStatusFlagChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            string statusFlag = string.Empty;
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession SNListSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            List<SN> snListObject = (List<SN>)SNListSession.Value;
            for (int j = 0; j < snListObject.Count; j++)
            {
                SN sn = snListObject[j];
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
        /// SNList檢查當前狀態不能為MRB
        /// </summary>
        public static void SNListMRBChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            if (Paras.Count != 1)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SessionSNList = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSNList == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Serial Number" });
                throw new MESReturnMessage(ErrMessage);
            }
            List<SN> snListObject = (List<SN>)SessionSNList.Value;
            for (int i = 0; i < snListObject.Count; i++)
            {
                SN ObjSN = snListObject[i];
                if (ObjSN.CurrentStation == "MRB")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { ObjSN.SerialNo, ObjSN.CurrentStation });
                    throw new MESReturnMessage(ErrMessage);
                }

                //判斷物料SN是否有MRB過	
                T_R_MRB MrbTable = new T_R_MRB(Station.SFCDB, Station.DBType);
                if (MrbTable.HadMrbed(ObjSN.SerialNo, Station.SFCDB))
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000182", new string[] { ObjSN.SerialNo });
                    throw new MESReturnMessage(ErrMessage);
                }
            }

            Station.AddMessage("MES00000067", new string[] { "" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }
        /// <summary>
        ///check 5pcs every week
        /// </summary>
        public static void JuniperGlueChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage, SQL = string.Empty;
            if (Paras.Count != 1)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            SN snObj = (SN)SNSession.Value;
            SQL = $@"SELECT  TO_CHAR(SYSDATE,'iw') AS CONTROL_TIME FROM DUAL";
            DataTable iwTime = Station.SFCDB.ExecSelect(SQL, null).Tables[0];
            var isGlue = Station.SFCDB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "GLUE_CONTROL" && t.CONTROL_TYPE == "GLUE_CHECK" && t.CONTROL_VALUE == "Y").Any();
            if (!isGlue)
            {
                return;
            }
            SQL = $@"select* from r_sn_station_detail where SKUNO = '{snObj.SkuNo}' AND  STATION_NAME = '{Station.DisplayName.ToString()}' and to_char(EDIT_TIME,'iw') ='{iwTime.Rows[0]["CONTROL_TIME"].ToString()}'";
            DataTable dt = Station.SFCDB.ExecSelect(SQL, null).Tables[0];
            T_R_SN_LOG RSNLOG = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
            R_SN_LOG _log = null;
            if (dt.Rows.Count < 5)
            {
                UIInputData O = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "30%", "45%" }, Message = "", Tittle = "Coverage inspection Check", Type = UIInputType.String, Name = "Result", ErrMessage = "Coverage inspection Check and SCAN 'PASS'" };
                var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station);
                if (ret1.ToString().ToUpper() == "PASS")
                {
                    UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "EmpNO", Tittle = "Coverage inspection Check", Type = UIInputType.String, Name = "EmpNO", ErrMessage = "Cancel Check" };
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);
                    T_c_user t_c_user = new T_c_user(Station.SFCDB, Station.DBType);
                    Row_c_user rowSendUser = t_c_user.getC_Userbyempno(ret.ToString(), Station.SFCDB, Station.DBType);

                    if (rowSendUser == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { ret.ToString(), ret.ToString() }));
                    }

                    UIInputData x = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "PassWord", Tittle = "Coverage inspection Check", Type = UIInputType.Password, Name = "PassWord", ErrMessage = "Cancel Check" };
                    O.OutInputs.Add(new DisplayOutPut() { Name = "EmpNO", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = ret.ToString() });
                    var ret2 = x.GetUiInput(Station.API, UIInput.Normal, Station);

                    Row_c_user rowSendUserandPass = t_c_user.getC_Userbyempnoandpass(ret.ToString(), Station.BU, ret2.ToString(), Station.SFCDB, Station.DBType);
                    if (rowSendUserandPass == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200706140440", new string[] { ret.ToString(), ret.ToString() }));
                    }
                    _log = new R_SN_LOG
                    {
                        ID = RSNLOG.GetNewID(Station.BU, Station.SFCDB),
                        SNID = snObj.ID,
                        SN = snObj.SerialNo,
                        LOGTYPE = "GLUE_CONTROL",
                        DATA1 = "GLUE_CHECK",
                        DATA2 = Station.DisplayName,
                        DATA3 = "COVERAGE_INSPECTION",
                        FLAG = "1",
                        CREATETIME = Station.SFCDB.ORM.GetDate(),
                        CREATEBY = Station.LoginUser.EMP_NO
                    };
                    Station.SFCDB.ORM.Insertable<R_SN_LOG>(_log).ExecuteCommand();
                }
                else { O.CBMessage = "Please Coverage inspection Check and  SCAN PASS "; }

            }
            SQL = $@"SELECT * FROM C_WORK_CLASS A,R_SN_STATION_DETAIL B  WHERE TO_DATE(TO_CHAR(SYSDATE,'HH24:MI:SS'),'HH24:MI:SS')  BETWEEN TO_DATE(A.START_TIME,'HH24:MI:SS') AND TO_DATE(A.END_TIME,'HH24:MI:SS')
           AND A.NAME = B.CLASS_NAME AND  B.SKUNO ='{snObj.SkuNo}' AND  B.STATION_NAME ='{Station.DisplayName}' AND B.VALID_FLAG =1 AND  TO_CHAR(B.EDIT_TIME,'YYYYMMDD') = TO_CHAR(SYSDATE,'YYYYMMDD')";
            dt = Station.SFCDB.ExecSelect(SQL, null).Tables[0];
            if (dt.Rows.Count < 3)
            {
                UIInputData a = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "", Tittle = "Torque check ", Type = UIInputType.String, Name = "Result", ErrMessage = "SCAN 'PASS'" };
                var rets = a.GetUiInput(Station.API, UIInput.Normal, Station);
                if (rets.ToString().ToUpper() == "PASS")
                {
                    _log = new R_SN_LOG
                    {
                        ID = RSNLOG.GetNewID(Station.BU, Station.SFCDB),
                        SNID = snObj.ID,
                        SN = snObj.SerialNo,
                        LOGTYPE = "GLUE_CONTROL",
                        DATA1 = "GLUE_CHECK",
                        DATA2 = Station.DisplayName,
                        DATA3 = "TORQUE",
                        FLAG = "1",
                        CREATETIME = Station.SFCDB.ORM.GetDate(),
                        CREATEBY = Station.LoginUser.EMP_NO
                    };
                    Station.SFCDB.ORM.Insertable<R_SN_LOG>(_log).ExecuteCommand();
                }
                else { a.CBMessage = "Please Torque check and SCAN PASS"; }
            }
            var iCount = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.WORKORDERNO == snObj.WorkorderNo && t.STATION_NAME == Station.DisplayName && t.VALID_FLAG == "1").ToList();
            if (iCount.Count < 1)
            {
                UIInputData a = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "", Tittle = "Dry Erase test check ", Type = UIInputType.String, Name = "Result", ErrMessage = "SCAN 'PASS'" };
                var res = a.GetUiInput(Station.API, UIInput.Normal, Station);
                if (res.ToString().ToUpper() == "PASS")
                {
                    _log = new R_SN_LOG
                    {
                        ID = RSNLOG.GetNewID(Station.BU, Station.SFCDB),
                        SNID = snObj.ID,
                        SN = snObj.SerialNo,
                        LOGTYPE = "GLUE_CONTROL",
                        DATA1 = "GLUE_CHECK",
                        DATA2 = Station.DisplayName,
                        DATA3 = "DRY_ERASE_TEST",
                        FLAG = "1",
                        CREATETIME = Station.SFCDB.ORM.GetDate(),
                        CREATEBY = Station.LoginUser.EMP_NO
                    };
                    Station.SFCDB.ORM.Insertable<R_SN_LOG>(_log).ExecuteCommand();
                }
                else
                {
                    a.CBMessage = "Please Dry Erase test check and SCAN PASS";
                }
            }
        }


        public static void KPSNTestChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count < 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionKPSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionKPSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionByPass = null;
            if (Paras.Count > 1)
            {
                sessionByPass = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            }
            var AllowByPass = true;
            if (sessionByPass == null || sessionByPass.Value == null)
            {
                AllowByPass = true;
            }
            else if (sessionByPass.Value.ToString() == "TRUE")
            {
                AllowByPass = true;
            }
            else
            {
                AllowByPass = false;
            }
            var sn = sessionKPSN.Value.ToString();
            var snObj = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
            var endStation = sfcdb.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == snObj.ROUTE_ID && t.STATION_TYPE == "JOBFINISH").First();
            T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
            t_r_test_record.CheckAllTestBySNStation(snObj.ID, endStation.STATION_NAME, Station.BU, AllowByPass, sfcdb);
        }

        public static void PassStationTimeControl(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN snObj = (SN)sessionSN.Value;
            snObj.PassStationTimeControl(Station.SFCDB.ORM, Station.StationName);
        }

        public static void DoubleCheckOBASample(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (skuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKU" }));
            }
            SN snObj = (SN)SNSession.Value;
            string skuno = skuSession.Value.ToString();

            DateTime sysdate = Station.SFCDB.ORM.GetDate();
            T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
            R_SN_LOG check_log = null;
            T_R_FUNCTION_CONTROL contrl = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);

            List<R_F_CONTROL> check_kp_list = contrl.GetListByFcv("OBASAMPLE_CHECKKP", "SKU_STATION", skuno, Station.StationName, Station.SFCDB);
            foreach (var f in check_kp_list)
            {
                var parno_ex = Station.SFCDB.ORM.Queryable<R_F_CONTROL_EX>().Where(r => r.DETAIL_ID == f.ID && r.NAME == "STATION_DOUBLECHECK").ToList().FirstOrDefault();
                if (parno_ex == null)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(parno_ex.VALUE))
                {
                    continue;
                }
                var list_kp = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == snObj.SerialNo && r.VALID_FLAG == 1 && r.STATION == parno_ex.VALUE)
                    .OrderBy(r => r.ITEMSEQ, SqlSugar.OrderByType.Asc).ToList();
                if (list_kp.Count == 0)
                {
                    throw new Exception($@"{parno_ex.VALUE} not exists in keypart, please call QE to check keypart and double check setting.");
                }
                UIInputData O = new UIInputData() { };
                O.Timeout = 3000000;
                O.IconType = IconType.Warning;
                O.Type = UIInputType.String;
                O.Tittle = "DoubleCheck";
                O.ErrMessage = "No input";
                O.UIArea = new string[] { "40%", "70%" };
                foreach (var k in list_kp)
                {
                    O.OutInputs.Clear();
                    O.Message = "Keypart";
                    O.Name = "Value";
                    O.CBMessage = "";
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "PARTNO",
                        Value = k.PARTNO,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "KP_NAME",
                        Value = k.KP_NAME,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "MPN",
                        Value = k.MPN,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    O.OutInputs.Add(new DisplayOutPut
                    {
                        Name = "SCANTYPE",
                        Value = k.SCANTYPE,
                        DisplayType = UIOutputType.Text.ToString()
                    });
                    while (true)
                    {
                        var input_sn = O.GetUiInput(Station.API, UIInput.CanPrint, Station);
                        Station.LabelPrint.Clear();
                        Station.LabelPrints.Clear();
                        Station.LabelStillPrint.Clear();
                        if (input_sn == null)
                        {
                            O.CBMessage = $@"Please Scan {k.PARTNO}";
                        }
                        else
                        {
                            string check_sn = input_sn.ToString().Trim();
                            if (string.IsNullOrEmpty(check_sn))
                            {
                                O.CBMessage = $@"Please Scan {k.PARTNO}";
                            }
                            else if (!check_sn.Equals(k.VALUE))
                            {
                                O.CBMessage = $@"{check_sn} not exists in keypart";
                            }
                            else if (check_sn.Equals("No input"))
                            {
                                throw new Exception("User Cancel");
                            }
                            else
                            {
                                check_log = new R_SN_LOG();
                                check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                                check_log.SNID = snObj.ID;
                                check_log.SN = snObj.SerialNo;
                                check_log.LOGTYPE = "OBADoubleCheck";
                                check_log.DATA1 = "CheckKeypart";
                                check_log.DATA2 = k.PARTNO;
                                check_log.DATA3 = check_sn;
                                check_log.DATA4 = Station.StationName;
                                check_log.FLAG = "1";
                                check_log.CREATETIME = sysdate;
                                check_log.CREATEBY = Station.LoginUser.EMP_NO;
                                Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void SilverWipInChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var sfcdb = Station.SFCDB;
            var strSN = SNSession.Value.ToString();
            var strsku = SKUSession.Value.ToString();

            var sn = sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SN == strSN).First();

            if (sn != null)
            {
                if (sn.SKUNO != strsku)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124144628", new string[] { strSN, strsku });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (sn.STATE_FLAG == "0")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124144715", new string[] { strSN });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124144814", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void SilverWipOutChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var sfcdb = Station.SFCDB;
            var strSN = SNSession.Value.ToString();

            var sn = sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SN == strSN).First();

            if (sn != null)
            {
                if (sn.STATE_FLAG == "1")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211105151002", new string[] { strSN });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
        }

        public static void SuperMarkCheckOutChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var sfcdb = Station.SFCDB;
            var smObj = new R_SUPERMARKET();
            var strSN = SNSession.Value.ToString();
            var snObj = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();

            if (snObj != null)
            {
                smObj = sfcdb.ORM.Queryable<R_SUPERMARKET>().Where(t => t.R_SN_ID == snObj.ID && t.STATUS == "1").ToList().FirstOrDefault();                
            }
            else
            {
                smObj = sfcdb.ORM.Queryable<R_SUPERMARKET>().Where(t => t.R_SN_ID == strSN && t.STATUS == "1").ToList().FirstOrDefault();                
            }
            if (smObj != null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220212085815", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void LastSNScanSOLineChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession CTNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (CTNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "CTN" }));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            string packNo = CTNSession.Value.ToString();
            SN snObj = (SN)SNSession.Value;
                        
            T_R_PACKING _PACKING = new T_R_PACKING(Station.SFCDB, Station.DBType);
            var isClosed = _PACKING.CheckCloseByPackno(packNo, Station.SFCDB);//檢查Carton是否已關閉
            if (isClosed)
            {
                List<R_SN> snList = new List<R_SN>();
                _PACKING.GetSNByPackNo(packNo, ref snList, Station.SFCDB);//獲取Carton內SNList
                var waitScanSNList = snList.FindAll(t => t.NEXT_STATION == Station.StationName);//獲取Carton SNList中待掃描當前工站的SNList
                //如果待掃描SNList只有一個SN且SN=當前掃描的SN, 表示當前掃描的SN是該Carton最後一個SN
                if (waitScanSNList.Count == 1)
                {
                    if (waitScanSNList[0].SN == snObj.SerialNo)
                    {
                        //獲取SN對應工單的PO&POLINE信息
                        //var orderList = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((om, ii, ih) => 
                        //    new object[] { SqlSugar.JoinType.Inner, om.ITEMID == ii.ID, SqlSugar.JoinType.Inner,  ii.TRANID == ih.TRANID })
                        //    .Where((om, ii, ih) => om.PREWO == snObj.WorkorderNo).Select((om, ii, ih) => new { ii, ih }).ToList();
                        //var itemID = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == snObj.WorkorderNo).Select(t => t.ITEMID).ToList().FirstOrDefault();
                        //var itemData = Station.SFCDB.ORM.Queryable<O_I137_ITEM>().Where(t => t.ID == itemID).Select(t=> new { t.TRANID, t.SALESORDERLINEITEM }).ToList().FirstOrDefault();
                        //var headSONumber = Station.SFCDB.ORM.Queryable<O_I137_HEAD>().Where(t => t.TRANID == itemData.TRANID).Select(t=> t.SALESORDERNUMBER).ToList().FirstOrDefault();
                        //啥幾把玩意, 用SQLSugar就報錯, 直接上SQL吧
                        var sql = $@"select ih.SALESORDERNUMBER, ii.SALESORDERLINEITEM from o_order_main om, o_i137_item ii, o_i137_head ih where om.itemid=ii.id and ii.tranid=ih.tranid and om.prewo='{snObj.WorkorderNo}'";
                        var dt = Station.SFCDB.RunSelect(sql).Tables[0];

                        #region 掃描SO.SOLINE
                        UIInputData O = new UIInputData() { };
                        O.Timeout = 3000000;
                        O.IconType = IconType.Warning;
                        O.Type = UIInputType.String;
                        O.UIArea = new string[] { "40%", "40%" };
                        O.Tittle = $@"ScanSOAndSOLine";
                        O.ErrMessage = "No input";
                        O.Message = "SOAndSOLINE";
                        O.Name = "SOAndSOLINE";
                        while (true)
                        {
                            var input_soline = O.GetUiInput(Station.API, UIInput.Normal, Station);
                            if (input_soline == null)
                            {
                                O.CBMessage = "Please Scan SO And SOLine";
                            }
                            else
                            {
                                string check_soline = input_soline.ToString();
                                if (string.IsNullOrEmpty(check_soline))
                                {
                                    O.CBMessage = "Please Scan SO And SOLine";
                                }
                                else if (!check_soline.StartsWith("1K"))
                                {
                                    O.CBMessage = $@"{check_soline} Not Starts With 1K";
                                }
                                else if (check_soline.IndexOf('.') < 0)
                                {
                                    O.CBMessage = $@"{check_soline} Format Not Macth With [1K********.****]";
                                }
                                else if (check_soline.Equals("No input"))
                                {
                                    throw new Exception("User Cancel");
                                }
                                else
                                {
                                    var soStr = check_soline.Substring(2, check_soline.Replace("1K","").IndexOf('.'));//獲取PO(去掉前兩位的1K)
                                    var lineStr = check_soline.Substring(check_soline.IndexOf('.') + 1);//獲取POLINE
                                    if (!dt.Rows[0]["SALESORDERNUMBER"].ToString().EndsWith(soStr) || !dt.Rows[0]["SALESORDERLINEITEM"].ToString().EndsWith(lineStr))
                                    {
                                        O.CBMessage = $@"Scan Not Match SO.SOLine Of WO:{snObj.WorkorderNo}";
                                    }
                                    else
                                    {
                                        #region 掃描QTY
                                        O.CBMessage = "";
                                        O.Message = "QTY";
                                        O.Name = "QTY";
                                        O.OutInputs.Clear();
                                        while (true)
                                        {
                                            var input_qty = O.GetUiInput(Station.API, UIInput.Normal, Station);
                                            if (input_qty == null)
                                            {
                                                O.CBMessage = "Please Scan QTY";
                                            }
                                            else
                                            {
                                                string check_qty = input_qty.ToString().Replace("Q", "");
                                                if (string.IsNullOrEmpty(check_qty))
                                                {
                                                    O.CBMessage = "Please Scan QTY";
                                                }
                                                else if (check_qty.Equals("No input"))
                                                {
                                                    throw new Exception("User Cancel");
                                                }
                                                else
                                                {
                                                    if (int.Parse(check_qty) != snList.Count)
                                                    {
                                                        O.CBMessage = $@"Scan QTY Not Equal To Carton Qty: {snList.Count}";
                                                    }
                                                    else
                                                    {
                                                        #region 掃描任意SN
                                                        O.CBMessage = "";
                                                        O.Message = "SN";
                                                        O.Name = "SN";
                                                        while (true)
                                                        {
                                                            var input_sn = O.GetUiInput(Station.API, UIInput.Normal, Station);
                                                            if (input_sn == null)
                                                            {
                                                                O.CBMessage = "Please Scan SN";
                                                            }
                                                            else
                                                            {
                                                                string check_sn = input_sn.ToString().Replace("S", "");
                                                                if (string.IsNullOrEmpty(check_sn))
                                                                {
                                                                    O.CBMessage = "Please Scan SN";
                                                                }
                                                                else if (check_sn.Equals("No input"))
                                                                {
                                                                    throw new Exception("User Cancel");
                                                                }
                                                                else
                                                                {
                                                                    if (!snList.Any(t => t.SN == check_sn))
                                                                    {
                                                                        O.CBMessage = $@"Scan SN Not In This Carton: {packNo}";
                                                                    }
                                                                    else
                                                                    {
                                                                        O.CBMessage = "";
                                                                        return;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        public static void SupermarketInSNCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var sfcdb = Station.SFCDB;
            var strSN = SNSession.Value.ToString();

            var sn = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").First();

            if (string.IsNullOrEmpty(strSN))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220308080242", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }

            if (sn == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220308080241", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }

            if (sn.SKUNO.StartsWith("711"))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220308080244", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }

            var silver_wip = sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.STATE_FLAG == "1" && t.SN == strSN).Any();

            if (silver_wip)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220308080243", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }

            if (sn.CURRENT_STATION == "MRB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { strSN }));
            }
            if (sn.COMPLETED_FLAG == "0")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211118115948", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }
            if (sn.SHIPPED_FLAG == "1")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }

            var snSM = sfcdb.ORM.Queryable<R_SUPERMARKET>().Where(t => sn.ID == t.R_SN_ID && t.STATUS == "1").First();
            if (snSM != null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124122733", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }

            Station.AddMessage("MSGCODE20220308080240", new string[] { strSN }, StationMessageState.UserDefined);

        }

        public static void SupermarketInSN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;

            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var sfcdb = Station.SFCDB;
            var strSKU = SKUSession.Value.ToString();
            var strSN = SNSession.Value.ToString();

            var sn = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").First();

            if (sn != null)
            {
                if (sn.SKUNO == strSKU)
                {
                    if (sn.SKUNO.StartsWith("711"))
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220308080244", new string[] { strSN });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    if (sn.CURRENT_STATION == "MRB")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { strSN }));
                    }
                    if (sn.COMPLETED_FLAG == "0")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211118115948", new string[] { strSN });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    if (sn.SHIPPED_FLAG == "1")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { strSN });
                        throw new MESReturnMessage(ErrMessage);
                    }

                    var snSM = sfcdb.ORM.Queryable<R_SUPERMARKET>().Where(t => sn.ID == t.R_SN_ID && t.STATUS == "1").First();
                    if (snSM != null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124122733", new string[] { strSN });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211124144628", new string[] { strSN, strSKU });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else if (Station.BU == "VNJUNIPER")
            {
                //FVN BuyPart SM CheckIn Logic By PE 譚義康 2022-02-11
                T_R_FUNCTION_CONTROL t_R_FUNCTION = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
                var rfc = t_R_FUNCTION.GetListByFcv("CHECK_BUYPART_SN_TEST_RECORD", "PARTNO", strSKU, Station.SFCDB);
                if (rfc.Count != 0)
                {
                    T_R_TEST_RECORD t_R_TEST = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
                    var rt = t_R_TEST.GetLastTestRecord(strSN, rfc[0].EXTVAL, Station.SFCDB);
                    if (rt == null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20190604110249");
                        throw new MESReturnMessage(ErrMessage);
                    }
                    else if (rt.STATE != "PASS")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { strSN, rfc[0].EXTVAL });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { strSN });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void CheckOrtInSample_SkuBROADCOM(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;
            string SN = ObjSN.ToString();
            //CHECK SKU BROADCOM
            try
            {
                List<R_F_CONTROL> listort = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "TEST_ALL_ORT" && t.CATEGORY == "TEST_ALL_ORT" && t.VALUE == ObjSN.SkuNo).ToList();
                if (listort.Count == 1)
                {
                    var sn = Station.SFCDB.ORM.Queryable<R_ORT_ALERT>().Where(t => t.SN == SN).First();
                    if (sn == null)
                    {
                        T_R_ORT_ALERT TAL = new T_R_ORT_ALERT(Station.SFCDB, Station.DBType);
                        R_ORT_ALERT SN_ORT_ALERT = new R_ORT_ALERT()
                        {
                            ID = TAL.GetNewID(Station.BU, Station.SFCDB),
                            SKUNO = ObjSN.SkuNo,
                            SN = SN,
                            SNID = ObjSN.ID,
                            //RAL.SCANREASON = "該產品已經標識為待掃描ORTIN.";
                            SCANREASON = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152456"),
                            ALERT_FLAG = 1,
                            CONTROLBY = "SYSTEM",
                            CONTROLDT = Station.GetDBDateTime(),
                            SCANBY = "SYSTEM",
                            SCANDT = Station.GetDBDateTime()

                        };
                        int flag = Station.SFCDB.ORM.Insertable(SN_ORT_ALERT).ExecuteCommand();
                        if (flag < 1)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { $@"Aruba CTO SN:{sn} Add Record Fail!" }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public static void ReTestOrtWhenFail(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        //{
        //    MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
        //    if (SNSession == null)
        //    {
        //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
        //    }
        //    SN ObjSN = (SN)SNSession.Value;

        //    List<R_ORT> RO = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == ObjSN.SerialNo && t.ORTEVENT == "ORTIN").ToList();
        //    if (RO.Count > 0)
        //    {
        //        DateTime? OrtInTime = RO[0].WORKTIME;
        //        List<R_TEST_BRCD> r_s = Station.SFCDB.ORM.Queryable<R_TEST_BRCD>().Where(t => t.SYSSERIALNO == ObjSN.SerialNo
        //              && t.EVENTNAME == "POST-ORT" && t.STATUS != "PASS" && t.TATIME > OrtInTime).ToList();

        //        List<R_SN_LOG> log = Station.SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.SN == ObjSN.SerialNo && t.SNID == ObjSN.ID && t.LOGTYPE == "RE_TEST_ORT" && t.DATA1 == "ORT" && t.FLAG == "1").ToList();
        //        if (log.Count == 0)
        //        {
        //            string UPDATE = $@"UPDATE R_ORT SET ID='#'||ID,SN='#'||SN, SNID='#'||SNID, WORKORDERNO='#'||WORKORDERNO, SKUNO='#'||SKUNO WHERE SN='{ObjSN.SerialNo}'";
        //            int i = Station.SFCDB.ExecuteNonQuery(UPDATE, CommandType.Text, null);

        //            T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
        //            R_SN_LOG check_log = new R_SN_LOG();
        //            check_log = new R_SN_LOG();
        //            check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
        //            check_log.SNID = ObjSN.ID;
        //            check_log.SN = ObjSN.SerialNo;
        //            check_log.LOGTYPE = "RE_TEST_ORT";
        //            check_log.DATA1 = "ORT";
        //            check_log.FLAG = "1";
        //            check_log.CREATETIME = Station.GetDBDateTime();
        //            check_log.CREATEBY = Station.LoginUser.EMP_NO;
        //            int rs = Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
        //            if (rs == 0)
        //            {
        //                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_SN_LOG" }));
        //            }
        //        }
        //    }
        //}
    }
}