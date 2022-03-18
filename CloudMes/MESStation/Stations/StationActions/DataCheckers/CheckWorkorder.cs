using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.OM;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static MESDataObject.Constants.PublicConstants;

namespace MESStation.Stations.StationActions.DataCheckers
{
    class CheckWorkorder
    {
        /// <summary>
        /// 檢查工單數據是否存在
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void WoDataCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            if (Paras.Count != 1)
            {

                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            //CHECK  Workorder是否存在        
            T_R_WO_BASE TRWO = new T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_R_WO_BASE ROWWO;
            string WO = Input.Value.ToString();
            try
            {
                ROWWO = TRWO.GetWo(WO, Station.SFCDB);
                s.Value = WO;
                s.InputValue = Input.Value.ToString();
                s.ResetInput = Input;

                //modify by LLF 2018-02-02
                //Station.AddMessage("MES00000029", new string[] { "Workorder", WO}, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000029", new string[] { "Workorder", WO });
                throw new MESReturnMessage(ErrMessage);
            }
            catch (Exception ex)
            {
                //modify by LLF 2018-02-02
                //ex.InnerException.Message;
                //string msgCode = ex.Message;
                //Station.AddMessage(msgCode, new string[] { "Workorder:" + WO }, StationMessageState.Fail);
                throw new MESReturnMessage(ex.Message);

            }

        }

        /// <summary>
        /// 檢查工單狀態必須Release&Start
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void SkuFromWODataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }

            //marked by ZGJ 2018-03-15
            //單從這個方法的功能上（這個方法的功能定義為檢查工單的狀態，但是方法名卻像是從工單加載機種）看，
            //沒有必要使用 SKU session
            //MESStationSession SKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (SKU == null)
            //{
            //    SKU = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(SKU);
            //}

            MESStationSession TWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TWO == null)
            {
                TWO = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TWO);
            }

            //CHECK  Workorder是否Release&Start 
            WorkOrder WorkorderInfo = new WorkOrder();
            //string WoNum = TWO.InputValue;
            var obj_wo = TWO.Value;

            //string WoNum = TWO.Value.ToString();
            try
            {
                //add by ZGJ 2018-03-15
                //檢查工單時，之前的步驟中可能就已經把工單實例放在 WO1 中，所以這裡判斷，如果已經是工單實例，
                //那麼就直接賦值，否則進行加載
                if (obj_wo is string)
                {
                    WorkorderInfo.Init(obj_wo.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                }
                else if (obj_wo is WorkOrder)
                {
                    WorkorderInfo = (WorkOrder)obj_wo;
                }
                //WorkorderInfo.Init(WoNum, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(WorkorderInfo.CLOSED_FLAG) || WorkorderInfo.CLOSED_FLAG == "1")   //null or 1代表工單已經關閉，0代表工單開啟
                {
                    //Modify by LLF 2018-02-02 
                    //Station.AddMessage("MES00000041", new string[] { "WO:" + WorkorderInfo.WorkorderNo }, StationMessageState.Fail);
                    //return;
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041", new string[] { WorkorderInfo.WorkorderNo });
                    throw new MESReturnMessage(ErrMessage);
                }

                if (WorkorderInfo.RELEASE_DATE == null)
                {
                    //Modify by LLF 2018-02-02 
                    //Station.AddMessage("MES00000042", new string[] { "WO:" + WorkorderInfo.WorkorderNo }, StationMessageState.Fail);
                    //return;
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000042", new string[] { WorkorderInfo.WorkorderNo });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            catch (Exception ex)
            {
                //Modify by LLF 2018-02-02 
                //ex.InnerException.Message;
                //string msgCode = ex.Message;
                //Station.AddMessage(msgCode, new string[] { "Workorder:" + WorkorderInfo.WorkorderNo }, StationMessageState.Fail);
                throw new MESReturnMessage(ex.Message);

            }
        }

        //工單狀態檢查
        public static void WoStateDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            ////ADD BY  SDL  20180316
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession WoLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoLoadPoint == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            WorkOrder ObjWO = (WorkOrder)WoLoadPoint.Value;



            ////ADD BY  SDL  20180316
            //CHECK  Workorder是否存在 
            string ErrMessage = "";

            T_R_WO_BASE TRWO = new T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_R_WO_BASE ROWWO;
            T_R_SN rSn = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            // string WO = Input.Value.ToString();   by sdl  20180316
            string WO = ObjWO.WorkorderNo;
            try
            {
                //List<R_SN> snList = rSn.GetRSNbyWo(WO, Station.SFCDB);
                ROWWO = TRWO.GetWo(WO, Station.SFCDB);
                R_WO_BASE woBase = ROWWO.GetDataObject();
                WorkOrder ObjWorkorder = new WorkOrder();
                //if (snList!=null)
                //{
                //    foreach (var item in snList)
                //    {
                //        if (woBase.ROUTE_ID != item.ROUTE_ID)
                //        {
                //            //throw new Exception("SN RouteID不唯一!");
                //            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000167", new string[] { item.SN });
                //            throw new MESReturnMessage(ErrMessage);
                //        }
                //    }
                //}



                if (woBase.CLOSED_FLAG == 1.ToString())
                {
                    // throw new Exception("ClosedFlag=1!");
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000168", new string[] { woBase.WORKORDERNO });
                    throw new MESReturnMessage(ErrMessage);
                }

                if ((woBase.START_STATION == null || woBase.START_STATION == "N/A") && woBase.WO_TYPE == "REWORK")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000203", new string[] { woBase.WORKORDERNO });
                    throw new MESReturnMessage(ErrMessage);
                }

                if (woBase.FINISHED_QTY > woBase.WORKORDER_QTY)
                {
                    //  throw new Exception("FinishQty>WorkOrderQty!");
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000169", new string[] { woBase.WORKORDERNO });
                    throw new MESReturnMessage(ErrMessage);
                }
                //add by fgg 檢查工單是否被鎖定在當前工站
                T_R_SN_LOCK r_sn_lock = new T_R_SN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                R_SN_LOCK snLock = r_sn_lock.GetLockObject("", "WO", "", woBase.WORKORDERNO, "", Station.StationName, Station.SFCDB);
                if (snLock != null && snLock.LOCK_STATUS == "1")
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { woBase.WORKORDERNO, snLock.LOCK_EMP, snLock.LOCK_REASON }));
                }

                Station.StationSession.Add(WoLoadPoint);
                ObjWorkorder.Init(WO, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                WoLoadPoint.Value = ObjWorkorder;
                WoLoadPoint.InputValue = Input.Value.ToString();
                WoLoadPoint.ResetInput = Input;
                WoLoadPoint.SessionKey = "1";
                WoLoadPoint.MESDataType = "WO";
                Station.AddMessage("MES00000029", new string[] { "Workorder", WO }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.05.12
        /// 工單投入檢查 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WOInputDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 4)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionWOQty = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_TYPE);
            if (sessionWOQty == null)
            {
                sessionWOQty = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionWOQty);
            }

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_TYPE);
            if (sessionInputQty == null)
            {
                sessionInputQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionInputQty);
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_TYPE);
            if (sessionExtQty == null)
            {
                sessionExtQty = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionExtQty);
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                objWorkorder.Init(sessionWO.Value.ToString(), Station.SFCDB);
                //工單鎖定檢查
                var holdobj = Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == objWorkorder.WorkorderNo && t.TYPE == "WO" 
                && (t.LOCK_STATION==Station.StationName||t.LOCK_STATION.ToUpper()=="ALL")
                && t.LOCK_STATUS == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                if (holdobj != null)
                    throw new Exception($@"Wo: {holdobj.WORKORDERNO} is Locked!reason: {holdobj.LOCK_REASON}");

                //投錯工站
                if (!objWorkorder.START_STATION.Equals(stationName))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000246", new string[] { stationName, objWorkorder.START_STATION }));
                }
                //工單關節
                if (objWorkorder.CLOSED_FLAG.Equals("1"))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000100"));
                }
                //已經投滿
                if (objWorkorder.INPUT_QTY >= objWorkorder.WORKORDER_QTY)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000247", new string[] { objWorkorder.WorkorderNo }));
                }
                sessionWOQty.Value = objWorkorder.WORKORDER_QTY;
                sessionInputQty.Value = objWorkorder.INPUT_QTY;
                sessionExtQty.Value = objWorkorder.WORKORDER_QTY - objWorkorder.INPUT_QTY;

                Station.AddMessage("MES00000029", new string[] { "Workorder", objWorkorder.WorkorderNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.05.12
        /// Kiểm tra công lệnh waibao
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WOWaiBaoInputDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 4)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionWOQty = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_TYPE);
            if (sessionWOQty == null)
            {
                sessionWOQty = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionWOQty);
            }

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_TYPE);
            if (sessionInputQty == null)
            {
                sessionInputQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionInputQty);
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_TYPE);
            if (sessionExtQty == null)
            {
                sessionExtQty = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionExtQty);
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                objWorkorder.Init(sessionWO.Value.ToString(), Station.SFCDB);
                
                //工單關節
                if (objWorkorder.CLOSED_FLAG.Equals("1"))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000100"));
                }
                //已經投滿
                if (objWorkorder.INPUT_QTY >= objWorkorder.WORKORDER_QTY)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000247", new string[] { objWorkorder.WorkorderNo }));
                }
                sessionWOQty.Value = objWorkorder.WORKORDER_QTY;
                sessionInputQty.Value = objWorkorder.INPUT_QTY;
                sessionExtQty.Value = objWorkorder.WORKORDER_QTY - objWorkorder.INPUT_QTY;

                Station.AddMessage("MES00000029", new string[] { "Workorder", objWorkorder.WorkorderNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.05.12
        /// 工單類型檢查 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WOTypeDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                if (objWorkorder.WO_TYPE.Equals("REWORK") || objWorkorder.WO_TYPE.Equals("REWORK_NPI"))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000249", new string[] { objWorkorder.WorkorderNo, objWorkorder.WO_TYPE }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 20190513 Patty added 
        /// 工單類型檢查 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WONewPNDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                T_R_WO_BOM t_r_wo_bom = new T_R_WO_BOM(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<R_WO_BOM> WO_BOM = t_r_wo_bom.ReturnNewPNfromBOM(sessionWO.Value.ToString(), Station.SFCDB);
                if (WO_BOM.Count != 0 && objWorkorder.SKU_NAME != "X7-2C" && objWorkorder.SKU_NAME != "E1-2C" && objWorkorder.SKU_NAME != "E2-2C")
                {
                    //return the first PN to let PE to setup 
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000212", new string[] { WO_BOM[0].MATNR.ToString() }));

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 簡單比對工單剩餘未使用條碼數和申請打印數量
        /// 兩個參數，一個 COUNT，一個 RESTRANGE
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PrintCountChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 2)
            {
                //參數不夠，至少兩個
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { });
                throw new MESReturnMessage(ErrMesg);
            }
            MESStationSession CountSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (CountSession == null)
            {
                //未輸入打印數量
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180904194912", new string[] { });
                throw new MESReturnMessage(ErrMesg);
            }

            MESStationSession RestSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (RestSession == null)
            {
                //未加載剩餘數量
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180904195016", new string[] { });
                throw new MESReturnMessage(ErrMesg);
            }

            if (Int32.Parse(CountSession.Value.ToString()) > Int32.Parse(RestSession.Value.ToString()))
            {
                //打印數量超過未使用數量
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180904195124", new string[] { });
                throw new MESReturnMessage(ErrMesg);
            }
            else
            {
                RestSession.Value = Int32.Parse(RestSession.Value.ToString()) - Int32.Parse(CountSession.Value.ToString());
            }
            Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);

        }

        /// <summary>
        /// Check if WO object is locked
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WOLockChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            var Alert = "FALSE";
            MESStationSession sessionAlert = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionAlert != null)
            {
                Alert = sessionAlert.Value.ToString().ToUpper();
            }
            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                var lockObj = sfcdb.ORM.Queryable<R_SN_LOCK>()
                    .Where(r => r.TYPE == "WO"
                        && r.WORKORDERNO == objWorkorder.WorkorderNo
                        && (r.LOCK_STATION == Station.StationName || r.LOCK_STATION == "ALL")
                        && r.LOCK_STATUS == "1")
                    .ToList()
                    .FirstOrDefault();
                if (lockObj != null)
                {
                    if (Alert == "TRUE")
                    {
                        Station.AddMessage("MES00000044",
                            new string[] { "WO", lockObj.WORKORDERNO, lockObj.LOCK_EMP },
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail,
                            MESPubLab.MESStation.MESReturnView.Station.StationMessageDisplayType.Swal
                        );
                    }
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000044", new string[] { "WO", lockObj.WORKORDERNO, lockObj.LOCK_EMP }));
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
            }
        }



        /// <summary>
        /// Check Print QTY
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WOPrintQtyChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionQTY = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionQTY == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            WorkOrder objWorkorder = new WorkOrder();
            objWorkorder = (WorkOrder)sessionWO.Value;
            var p = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(r => r.WORKORDERNO == objWorkorder.WorkorderNo && r.STATION_NAME == "KIT_PRINT").ToList();
            if (objWorkorder.WORKORDER_QTY < (p.Count + int.Parse(sessionQTY.Value.ToString())))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200408083238", new string[] { (objWorkorder.WORKORDER_QTY - p.Count).ToString(), sessionQTY.Value.ToString() }));
            }
        }

        public static void JuniperPacktypeChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNORuleSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNORuleSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession PacktypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PacktypeSession == null)
            {
                return;
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            var strSku = SkuSession.Value.ToString();
            var strPT = PackNORuleSession.Value.ToString();
            if (PacktypeSession.Value.ToString() == "Single pack")
            {
                var strR = $@"^(\S+):\((\d+)\)$";
                var R = Regex.Match(strPT, strR);
                if (R.Success == true)
                {
                    var Pno = R.Groups[1].Value.ToString();
                    if (Station.SFCDB.ORM.Queryable<O_SKU_PACKAGE>().Where(t => t.SKUNO == strSku && t.PARTNO == Pno && t.SCENARIO == "CartonOverpack").Any())
                    {
                        return;
                    }
                    else
                    {
                        throw new Exception("It must use CartonOverpack .Pls Change.");
                    }
                }
                else
                {
                    throw new Exception($@"ERR:{strPT}");
                }
            }
            else if (PacktypeSession.Value.ToString() == "Bulk pack")
            {
                var strR = $@"^(\S+):\((\d+)\)$";
                var R = Regex.Match(strPT, strR);
                if (R.Success == true)
                {
                    var Pno = R.Groups[1].Value.ToString();
                    if (Station.SFCDB.ORM.Queryable<O_SKU_PACKAGE>().Where(t => t.SKUNO == strSku && t.PARTNO == Pno && t.SCENARIO == "Multipack").Any())
                    {
                        return;
                    }
                    else
                    {
                        throw new Exception("It must use Multipack .Pls Change.");
                    }
                }
                else
                {
                    throw new Exception($@"ERR:{strPT}");
                }

            }



        }

        /// <summary>
        /// 檢查打印數量
        /// </summary>
        public static void PrintQtyChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionQTY = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionQTY == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string stationName = Paras[2].VALUE.ToString().ToUpper().Trim();
            if (string.IsNullOrEmpty(stationName))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            WorkOrder objWO = new WorkOrder();
            objWO = (WorkOrder)sessionWO.Value;
            switch (stationName)
            {
                case "PRINT_MAC":
                    var p = SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(r => r.EXT1 == objWO.WorkorderNo && r.EXT3 == stationName).ToList();
                    if (objWO.WORKORDER_QTY < (p.Count + int.Parse(sessionQTY.Value.ToString())))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200408083238", new string[] { (objWO.WORKORDER_QTY - p.Count).ToString(), sessionQTY.Value.ToString() }));
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// MAC補打檢查
        /// </summary>
        public static void RePrintMacChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionMac = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionMac == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            WorkOrder objWO = new WorkOrder();
            objWO = (WorkOrder)sessionWO.Value;
            string mac = sessionMac.Value.ToString();
            bool hasLog = SFCDB.ORM.Queryable<R_PRINT_LOG>().Where(t => t.SN == mac && t.CTYPE == "MAC").Any();
            if (!hasLog)
            {
                //throw new Exception(mac + " 不存在，請確認！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164455", new string[] { mac }));
            }
            bool woFlag = SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.VALUE == mac && t.EXT1 == objWO.WorkorderNo).Any();
            if (!hasLog)
            {
                //throw new Exception(objWO.WorkorderNo + " 不存在MAC:[" + mac + "]，請確認！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164738", new string[] { objWO.WorkorderNo, mac }));
            }
        }
        /// <summary>
        /// JuniperWO NOt in MRB
        /// </summary>
        public static void JuniperWONotINMRB(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SN objSN = new SN();
            objSN = (SN)sessionSN.Value;

            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            WorkOrder objWO = new WorkOrder();
            objWO = (WorkOrder)sessionWO.Value;
           
            bool bol = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(x => x.PREWO == objWO.WorkorderNo).Any();
            if (bol) {
                //throw new Exception("SN : "+objSN.SerialNo+"->Juniper WO " + objWO.WorkorderNo + " 不允许入MRB！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164810", new string[] { objSN.SerialNo, objWO.WorkorderNo }));
            } 
        }
    }
}
