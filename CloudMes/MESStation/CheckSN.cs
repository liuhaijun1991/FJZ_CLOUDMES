using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESStation.LogicObject;
using MESStation.HateEmsGetDataService;
using System.Net;
using System.Text.RegularExpressions;
using MESDBHelper;
using MESPubLab.MESStation.SNMaker;

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
                throw new Exception("參數數量不正確!");
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
                throw new Exception("參數數量不正確!");
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
            string snStr = Input.Value.ToString();
            SN sn = new SN(snStr, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
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
        /// 2019-01-14 Patty added for REPRINT function station check
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintStationChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
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
                SNS.Add(SNmaker.GetNextSN(SNRULE, Station.SFCDB));
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
                    if (WorkOrder.SKU_NAME == "X7-2C" || WorkOrder.SKU_NAME == "ODA_HA"  || WorkOrder.SKU_NAME == "E1-2C" || WorkOrder.SKU_NAME == "E2-2C" || (WorkOrder.PRODUCTION_TYPE == "PTO" && WorkOrder.SKU_NAME == "X8-8") || (WorkOrder.PRODUCTION_TYPE == "PTO" && WorkOrder.SKU_NAME == "ODA_X8-2"))
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
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("此序列號不符合入027M的條件,必須是RMA產品!!"));
                        }
                        break;
                    case "029M":
                        if (dlock.Rows.Count > 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該工單‘" + dlock.Rows[0]["sn.workorderno"] + "’被鎖原因:'" + dlock.Rows[0]["lk.LOCK_REASON"] + "',請通知PE/PQE確認處理!"));
                        }
                        if (dcategory.Rows.Count > 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("重工須以最高父項入庫!!"));
                        }
                        if (dship.Rows.Count > 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該產品已經掃SHIPPING,不允許進入MRB!!"));
                        }
                        break;
                }

                if (dpl.Rows.Count >= 1)
                {
                    if ((int)bb["countrssn"] == (int)cc["countrmasn"])
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該棧板已經在MRB倉!"));
                    }
                }
                else if (dpl.Rows.Count == 0 && sn.ToString().Substring(0, 2) == "PL")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("該棧板沒有產品!"));
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
                throw new Exception("參數數量不正確!");
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
            T_R_MRB R_MRB =new T_R_MRB(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
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
            if (!WOType.IsTypeInput("RMA", SnSession.Value.ToString().Substring(0,6), sfcdb))
            {
                if (MrbWoRohs !=null && MrbWoRohs != ReWoRohs) {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830113926", new string[] { ReWoRohs + MrbWoRohs }));
                }
            }

            string ReWoCustVER = R_WO_BASE.GetWoByWoNo(WoSession.Value.ToString(), Station.SFCDB).SKU_VER.Substring(1, 1);
            string MrbCustVER = R_WO_BASE.GetMrbWoByRohs(SnSession.Value.ToString(), Station.SFCDB).SKU_VER.Substring(1, 1);

            if (MrbCustVER !=null && MrbCustVER != ReWoCustVER) {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830140709", new string[] { ReWoRohs + MrbWoRohs }));
            }

            string ReSkuno = R_WO_BASE.GetWoByWoNo(WoSession.Value.ToString(), Station.SFCDB).SKUNO;
            string MrbSkuno = R_WO_BASE.GetMrbWoByRohs(SnSession.Value.ToString(), Station.SFCDB).SKUNO;

            if (ReSkuno != MrbSkuno) {
                if (WoSession.Value.ToString().Substring(1, 6) == "002163" || WoSession.Value.ToString().Substring(1, 6) == "002164") {
                    if (ReSkuno.Length >10) {
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
                    else {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190830160027", new string[] { ReSkuno + MrbSkuno }));
                    }
                }
                else {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190830160525");
                    throw new MESReturnMessage(errMsg);
                }
            }
            if (R_RELATIONDATA_DETAIL.CheckSnExists(SnSession.Value.ToString(), Station.SFCDB)) {
                bool isupdatesnstatus1 = false;
                bool isupdatesnstatus2 = false;
                isupdatesnstatus1 = R_RELATIONDATA_DETAIL.UpdateMrbSnStatus1(SnSession.Value.ToString(), Skuno_Route, NextStation, Station.SFCDB);
                if ( !isupdatesnstatus1) {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190902100626");
                    throw new MESReturnMessage(errMsg);
                }
                isupdatesnstatus2 = R_RELATIONDATA_DETAIL.UpdateMrbSnStatus2(SnSession.Value.ToString(), Skuno_Route, NextStation, Station.SFCDB);
                if (!isupdatesnstatus2) {
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
            if (NewSnSession != null)
            {
                Station.StationSession.Remove(NewSnSession);
            }
            NewSnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Input.Value.ToString() };
            Station.StationSession.Add(NewSnSession);

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
                bool isInWoRange = TR_WO_REGION.CheckSNInWoRange(strNewSN, wo.WorkorderNo, Station.SFCDB,Station.BU);
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
            bool isInWoRange = TR_WO_REGION.CheckSNInWoRange(strNewSN, wo.WorkorderNo, Station.SFCDB,Station.BU);
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
                throw new Exception("參數數量不正確!");
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new Exception("请输入SN!");
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
                throw new Exception("參數數量不正確!");
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
                throw new Exception("请输入SN!");
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
            string Lotno = "";
            LotNo ObjLot = new LotNo();
            Row_R_LOT_STATUS GetLotNoBySN;
            Row_R_LOT_STATUS GetLotNoByLot;
            T_R_LOT_DETAIL RowLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count <= 0)
            {
                throw new Exception("參數數量不正確!");
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
                throw new Exception("參數數量不正確!");
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
                throw new Exception("參數數量不正確!");
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
                throw new Exception("參數數量不正確!");
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
                throw new Exception("參數數量不正確!");
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
            if (wo_type != "REWORK" && wo_type != "RMA")
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

            string CheckValue = Paras.Find(t => t.SESSION_TYPE.ToUpper() == "CHECKVALUE")?.VALUE;
            if (CheckValue == null)
            {
                CheckValue = "TS179";
            }


            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var routes = Paras.FindAll(t => t.SESSION_TYPE.ToUpper() == "ROUTENAME");


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

                SettingStation = CheckValue;

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
            var aa = SnDetailTable.GetRotationByCSNCheckStatus(SNobj.SerialNo, Station.SFCDB);
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
                throw new Exception("參數數量不正確!");
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
            string SN = "";
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
            SN = SNSession.Value.ToString();
            OrtSn = Lot_Tabel.CheckSN(SN, Station.SFCDB);
            if (OrtSn != null)
            {
                TestData = Test_Tabel.GetSNLastORTPassDetail(SN, OrtSn.CREATE_DATE.ToString(), Station.SFCDB);
                Station_Detail = Station_Tabel.GetSNORTPassStationDetail(SN, OrtSn.CREATE_DATE.ToString(), Station.SFCDB);
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

            SnOrNot = Table_R_TEST_DETAIL_VERTIV.GetTestByOrtSn(Station.SFCDB, StrSn,Station.DisplayName);
            if (SnOrNot == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190604140336", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            r_sn_test_time = Table_R_TEST_DETAIL_VERTIV.GETCheckSnORTTestTime(Station.SFCDB, StrSn,Station.DisplayName);
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
                throw new Exception("參數數量不正確!");
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
                //List<R_TEST_RECORD> td = t_r_test_record.GetTestDataByTimeBefor(sfcdb, snObj.ID, snObj.StartTime ?? DateTime.Now);
                // check the latest test record
                List<R_TEST_RECORD> td = t_r_test_record.GetTestDataByTimeBefor(sfcdb, snObj.SerialNo, snObj.StartTime ?? DateTime.Now);

                T_R_MES_LOG mesLog = new T_R_MES_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                //string id = mesLog.GetNewID("ORACLE", sfcdb);
                Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

                foreach (var item in cRouteDetailList)
                {
                    //Add to check 2C product, Chassis BBT first and SM SFT last vince_20190318
                    if (td.FindAll(t => t.TESTATION == "BBT" && t.STATE.Equals("PASS")).Count != 0 && item.STATION_NAME.ToString() == "SFT")
                    {
                        List<R_TEST_RECORD> td_BBT = t_r_test_record.GetTestDataByChassisBBT(sfcdb, snObj.SerialNo, snObj.StartTime ?? DateTime.Now);

                        if (td_BBT.Count != 2)
                        {
                            //missing SM test data
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_01", new string[] { snObj.SerialNo, item.STATION_NAME }));
                        }
                        else
                        {
                            foreach (var SM in td_BBT)
                            {
                                if (td.FindAll(t => t.TESTATION == "BBT" && t.ENDTIME <= SM.ENDTIME).Count == 0)
                                {
                                    //writeLog for test data missing  vince_20190305
                                    sfcdb.BeginTrain();
                                    //rowMESLog.ID = id;
                                    rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, sfcdb);
                                    rowMESLog.PROGRAM_NAME = "CloudMES";
                                    rowMESLog.CLASS_NAME = "Check SN";
                                    rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                    rowMESLog.LOG_MESSAGE = "SM Test Data before Chassis " + snObj.SerialNo + " in station " + item.STATION_NAME;
                                    rowMESLog.LOG_SQL = "";
                                    rowMESLog.EDIT_EMP = "OracleStation";
                                    rowMESLog.EDIT_TIME = System.DateTime.Now;
                                    rowMESLog.DATA1 = snObj.SerialNo;
                                    sfcdb.ThrowSqlExeception = true;
                                    sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                    sfcdb.CommitTrain();

                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_02", new string[] { snObj.SerialNo, item.STATION_NAME }));
                                }
                                else if (td.FindAll(t => t.TESTATION == "BBT" && SM.STATE.Equals("PASS")).Count == 0)
                                {
                                    //writeLog for test data missing  vince_20190305
                                    sfcdb.BeginTrain();
                                    //rowMESLog.ID = id;
                                    rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, sfcdb);
                                    rowMESLog.PROGRAM_NAME = "CloudMES";
                                    rowMESLog.CLASS_NAME = "Check SN";
                                    rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                    rowMESLog.LOG_MESSAGE = "SM Test Pass Data not exists for " + snObj.SerialNo + " in station " + item.STATION_NAME;
                                    rowMESLog.LOG_SQL = "";
                                    rowMESLog.EDIT_EMP = "OracleStation";
                                    rowMESLog.EDIT_TIME = System.DateTime.Now;
                                    rowMESLog.DATA1 = snObj.SerialNo;
                                    sfcdb.ThrowSqlExeception = true;
                                    sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                    sfcdb.CommitTrain();

                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_01", new string[] { snObj.SerialNo, item.STATION_NAME }));
                                }
                            }
                        }
                    }
                    //end vince_20190318

                    //Add to check ODA_HA product, Vanilla SFT first and Chocolate FSC last vince_20190909

                    //td.FindAll(t => t.TESTATION == "BBT" && t.STATE.Equals("PASS")).Count != 0 && 
                    if (item.STATION_NAME.ToString() == "FSC")
                    {
                        T_C_SKU T_C_SKU = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                        SkuObject SKU = T_C_SKU.GetSkuBySn(snObj.SerialNo.ToString(), sfcdb);
                        T_C_SERIES t_c_series = new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle);
                        C_SERIES c_series = t_c_series.GetDetailById(sfcdb, SKU.CSeriesId);//sku.CSeriesId
                        if (c_series.SERIES_NAME.Contains("ODA_HA"))
                        {

                            List<R_TEST_RECORD> td_Choco = t_r_test_record.GetTestDataByForODAChoco(sfcdb, snObj.SerialNo, snObj.StartTime ?? DateTime.Now);
                            List<R_TEST_RECORD> td_Vanilla = t_r_test_record.GetTestDataByForODAVanilla(sfcdb, snObj.SerialNo, snObj.StartTime ?? DateTime.Now);

                            if (td_Choco.FindAll(t => t.TESTATION == "FSC" && t.STATE.Equals("PASS")).Count != 2)
                            {
                                //missing choco test data
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_03", new string[] { snObj.SerialNo, item.STATION_NAME }));
                            }
                            else if (td_Vanilla.FindAll(t => t.TESTATION == "SFT" && t.STATE.Equals("PASS")).Count != 2)
                            {
                                //missing vanilla test data
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_05", new string[] { snObj.SerialNo, item.STATION_NAME }));
                            }
                            else
                            {
                                foreach (var SM in td_Vanilla)
                                {
                                    if (td_Choco.FindAll(t => t.ENDTIME <= SM.ENDTIME && SM.STATE.Equals("PASS")).Count != 0)
                                    {
                                        //writeLog for test data missing  vince_20190305
                                        sfcdb.BeginTrain();
                                        //rowMESLog.ID = id;
                                        rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, sfcdb);
                                        rowMESLog.PROGRAM_NAME = "CloudMES";
                                        rowMESLog.CLASS_NAME = "Check SN";
                                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                        rowMESLog.LOG_MESSAGE = "Vanilla Test Data is after Chocolate " + snObj.SerialNo + " in station " + item.STATION_NAME;
                                        rowMESLog.LOG_SQL = "";
                                        rowMESLog.EDIT_EMP = "OracleStation";
                                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                                        rowMESLog.DATA1 = snObj.SerialNo;
                                        sfcdb.ThrowSqlExeception = true;
                                        sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                        sfcdb.CommitTrain();

                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_04", new string[] { snObj.SerialNo, item.STATION_NAME }));
                                    }
                                    else if (td_Choco.FindAll(t => t.TESTATION == "FSC" && SM.TESTATION == "SFT" && SM.R_SN_ID.Substring(SM.R_SN_ID.Length - 2, 2).ToString() == t.SN.Substring(t.SN.Length - 2, 2).ToString() && SM.STATE.Equals("PASS")).Count == 0)
                                    {
                                        //writeLog for test data missing  vince_20190305
                                        sfcdb.BeginTrain();
                                        //rowMESLog.ID = id;
                                        rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, sfcdb);
                                        rowMESLog.PROGRAM_NAME = "CloudMES";
                                        rowMESLog.CLASS_NAME = "Check SN";
                                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                        rowMESLog.LOG_MESSAGE = "Vanilla Test Pass Data not exists for " + snObj.SerialNo + " in station " + item.STATION_NAME;
                                        rowMESLog.LOG_SQL = "";
                                        rowMESLog.EDIT_EMP = "OracleStation";
                                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                                        rowMESLog.DATA1 = snObj.SerialNo;
                                        sfcdb.ThrowSqlExeception = true;
                                        sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                        sfcdb.CommitTrain();

                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_05", new string[] { snObj.SerialNo, item.STATION_NAME }));
                                    }
                                }
                            }
                            td = td_Choco;
                        }
                    }
                    //end vince_20190909

                    if (td.FindAll(t => t.MESSTATION == item.STATION_NAME && t.STATE.Equals("PASS")).Count == 0)
                    {
                        //writeLog for test data missing  vince_20190305
                        sfcdb.BeginTrain();
                        rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, sfcdb);
                        //rowMESLog.ID = id;
                        rowMESLog.PROGRAM_NAME = "CloudMES";
                        rowMESLog.CLASS_NAME = "Check SN";
                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                        rowMESLog.LOG_MESSAGE = "Test Pass Data not exists for " + snObj.SerialNo + " in station " + item.STATION_NAME;
                        rowMESLog.LOG_SQL = "";
                        rowMESLog.EDIT_EMP = "OracleStation";
                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                        rowMESLog.DATA1 = snObj.SerialNo;
                        sfcdb.ThrowSqlExeception = true;
                        sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                        sfcdb.CommitTrain();

                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { snObj.SerialNo, item.STATION_NAME }));
                    }
                    else
                    {
                        //writeLog for successfully pass    vince_20190305
                        //rowMESLog.ID = id;
                        rowMESLog.ID = mesLog.GetNewID(Station.LoginUser.BU, sfcdb);
                        rowMESLog.PROGRAM_NAME = "CloudMES";
                        rowMESLog.CLASS_NAME = "Check SN";
                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                        rowMESLog.LOG_MESSAGE = "Test pass successfully for " + snObj.SerialNo + " in station " + item.STATION_NAME;
                        rowMESLog.LOG_SQL = "";
                        rowMESLog.EDIT_EMP = "OracleStation";
                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                        rowMESLog.DATA1 = snObj.SerialNo;
                        sfcdb.ThrowSqlExeception = true;
                        sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }

                #endregion
            }
              
            catch (Exception ex)
            {
                throw ex;
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

        /// <summary>
        /// 檢查當前Pack狀態(包裝里SN是否狀態一致),當前工站是否待OBA
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackSnStatus(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_SN_PACKING T_RSnPacking = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);
            if (T_RSnPacking.CheckPackCurrentStatus(Station.SFCDB, Paras[1].VALUE, PackNoSession.Value.ToString()))
                throw new Exception( PackNoSession.Value.ToString()+ " 該棧板已過 " + Paras[1].VALUE + " 工站");
           else if (!T_RSnPacking.CheckPackSnStatus(Station.SFCDB, Paras[1].VALUE, PackNoSession.Value.ToString()))
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528095410", new string[] { PackNoSession.Value.ToString(), Paras[1].VALUE }));
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
            if (rSnList.FindAll(t => !t.NEXT_STATION.Equals("OBA")).Count > 0)
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
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000136", new string[] { snObject.NextStation }));
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
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { sn.SerialNo, RD.STATION_NAME }));
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
            R_SILVER_ROTATION rotation = t_r_silver_rotation.GetRotationByCSN(snObj.SerialNo, Station.SFCDB);
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
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190813101324", new string[] { Sn.SerialNo, wo.WorkorderNo,Sn.WorkorderNo}));
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
            bool IsReplaceSN = t_r_replace_sn.NewSNIsReplace(snObject.SerialNo, Station.SFCDB);
            if (oldWOObject == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { snObject.WorkorderNo }));
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
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102153958", new string[] { oldWOObject.WorkorderNo, newWOObject.WorkorderNo }));
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
            if (sn != null && !sn.SKUNO.Substring(0,sn.SKUNO.LastIndexOf("-")).Equals(sku.SkuNo.Substring(0,sku.SkuNo.LastIndexOf("-"))))
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
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091221", new string[] { WO,"PQE" }));
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
            DataTable dt, dtft;
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
                    counts = Station.SFCDB.ORM.Queryable<c_sku_ft_first_config>().Where(t => t.SKUNO == Sku && t.ORT!=null).ToList().Count;
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
                t_r_2dx_sn.CheckSampling(SN,"2","2DX",Station.SFCDB);
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
                                throw new MESReturnMessage($@"機種{r_sn.SKUNO}抽測比例為百分之百,此SN沒有ORT測試PASS記錄,請重測ORT");
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
                throw new MESReturnMessage($@"'SN 在R_SN_KPL表中有包含*號的Keypart,請退回到ASSY,重新過組裝 ");
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
                    if (dt.Rows.Count==0)
                    {
                        Route routeDetail = new Route(r_sn.ROUTE_ID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                        List<string> snStationList = new List<string>();
                        List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                        RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == "FT2_01").FirstOrDefault();
                        string LastStation=string.Empty;

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

                        throw new MESReturnMessage($@"此SN在FT2_01未通過第二道測試，請重測FT2_01！");
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
                    throw new MESReturnMessage($@"您輸入的機種{sessionSkuInput.Value.ToString()}和SN機種{sessionSku.Value.ToString()}不一樣，請重新輸入！");
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
                throw new Exception("參數數量不正確!");
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
        private class Row_R_wo_base
        {
        }

        private class Wo
        {
        }
    }
}
