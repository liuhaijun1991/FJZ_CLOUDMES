using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class WorkorderLoader
    {
        /// <summary>
        /// 從輸入加載工單數據
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void LoadWoFromInput(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            //if (Paras.Count == 0)
            //{
            //    throw new Exception("參數數量不正確!");

            //}
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }

            //dStation.NextInput = Station.Inputs[1];
            //Station.DBS["APDB"].Borrow()
            LogicObject.WorkOrder WO = new LogicObject.WorkOrder();
            WO.Init(Input.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            s.Value = WO;
            s.InputValue = Input.Value.ToString();
            s.ResetInput = Input;
            Station.AddMessage("MES00000029", new string[] { "Workorder", WO.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        public static void WoInputCountDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            MESStationSession count = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (count == null)
            {
                count = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(count);
            }
            MESStationSession wo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (wo == null)
            {
                wo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(wo);
            }
            string workorder = Station.StationSession[1].InputValue.ToString();
            MESStation.LogicObject.WorkOrder worder = new LogicObject.WorkOrder();
            MESDataObject.Module.T_R_WO_BASE trwb = new MESDataObject.Module.T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_R_WO_BASE rrwb = (MESDataObject.Module.Row_R_WO_BASE)trwb.NewRow();
            rrwb = worder.GetWoMode(workorder, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Station.AddMessage("MES00000029", new string[] { "Workorder", wo.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);

        }

        /// <summary>
        /// 工單信息加載器,加載工單信息到指定位置,主要用來給界面做輸出. 2018/1/2 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">
        /// InputQTYSave	工單已投數量保存位置	{SESSION_TYPE:"INPUTQTY",SSION_KEY:"1",VALUE:""}
        /// FinishQTYSave 工單已完成數量保存位置 { SESSION_TYPE:"FINISHQTY",SSION_KEY:"1",VALUE:""}
        /// StationQTYSave 本工站過站數量保存位置 { SESSION_TYPE:"STATIONQTY",SSION_KEY:"1",VALUE:""}
        /// WOLoadPoint 工單的保存位置 { SESSION_TYPE:"WO",SSION_KEY:"1",VALUE:""}
        /// </param>
        public static void WoInfoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStation.LogicObject.WorkOrder wo = new MESStation.LogicObject.WorkOrder();
            string strWONO = "";
            if (Paras.Count != 5)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession InputQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputQTY_Session == null)
            {
                InputQTY_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(InputQTY_Session);
            }
            MESStationSession FinishQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FinishQTY_Session == null)
            {
                FinishQTY_Session = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FinishQTY_Session);
            }
            MESStationSession StationQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StationQTY_Session == null)
            {
                StationQTY_Session = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationQTY_Session);
            }
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (Wo_Session == null)
            {

                strWONO = Input.Value.ToString();
                Wo_Session = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Wo_Session);
            }
            strWONO = Wo_Session.Value.ToString();
            //add by LLF 2018-03-26 增加工單數量Sesstion
            MESStationSession WoQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (WoQty_Session == null)
            {
                WoQty_Session = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoQty_Session);
            }
            else
            {
                //wo = (WorkOrder)Wo_Session.Value;
                //if (wo != null && wo.WorkorderNo != null && wo.WorkorderNo.Length > 0)
                //{
                //    strWONO = wo.WorkorderNo;
                //   // wo.Init(wo.WorkorderNo, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                //}
                //else
                //{
                //    wo = new MESStation.LogicObject.WorkOrder();
                //    strWONO = Input.Value.ToString();
                //   // wo.Init(Input.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                //}
                //  Wo_Session.Value = wo;
                //strWONO = Wo_Session.Value.ToString();
                Wo_Session.InputValue = Input.Value.ToString();
                Wo_Session.ResetInput = Input;
            }
            try
            {
                wo.Init(strWONO, Station.SFCDB, Station.DBType);
                Wo_Session.Value = wo;
                InputQTY_Session.Value = wo.INPUT_QTY;
                FinishQTY_Session.Value = wo.FINISHED_QTY;
                WoQty_Session.Value = wo.WORKORDER_QTY;
                T_R_SN_STATION_DETAIL TR_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
                StationQTY_Session.Value = TR_SN_STATION_DETAIL.GetCountByWOAndStation(wo.ToString(), Station.StationName, Station.SFCDB);
                Station.AddMessage("MES00000001", new string[] { Input.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                Wo_Session.Value = null;
                InputQTY_Session.Value = 0;
                FinishQTY_Session.Value = 0;
                StationQTY_Session.Value = 0;
                WoQty_Session.Value = 0;
                throw ex;
            }


        }
        public static void WoStationEventQty(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WoPassQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoPassQty_Session == null)
            {
                WoPassQty_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoPassQty_Session);
            }
            MESStationSession WoFailQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoFailQty_Session == null)
            {
                WoFailQty_Session = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoFailQty_Session);
            }
            MESStationSession WoQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (WoQty_Session == null)
            {
                WoQty_Session = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoQty_Session);
            }
            MESStation.LogicObject.WorkOrder wo = new MESStation.LogicObject.WorkOrder();
            MESStationSession StrSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (StrSN_Session == null)
            {
                StrSN_Session = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StrSN_Session);
            }

            try
            {
                string strSN = StrSN_Session.Value.ToString();
                var WOOBJ = Station.SFCDB.ORM.Queryable<R_WO_BASE, R_SN>((W, S) => W.WORKORDERNO == S.WORKORDERNO)
                    .Where((W, S) => S.SN == strSN && S.VALID_FLAG == "1")
                    .Select((W, S) => W)
                    .ToList()
                    .FirstOrDefault();
                var PassQty = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                    .Where(d => d.WORKORDERNO == WOOBJ.WORKORDERNO && d.STATION_NAME == Station.StationName && d.REPAIR_FAILED_FLAG == "0" && d.VALID_FLAG == "1")
                    .ToList().Count;
                var FailQty = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                    .Where(d => d.WORKORDERNO == WOOBJ.WORKORDERNO && d.STATION_NAME == Station.StationName && d.REPAIR_FAILED_FLAG == "1" && d.VALID_FLAG == "1")
                    .ToList().Count;
                WoQty_Session.Value = WOOBJ.WORKORDER_QTY;
                WoPassQty_Session.Value = PassQty;
                WoFailQty_Session.Value = FailQty;
                Station.AddMessage("MES00000001", new string[] { Input.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                WoPassQty_Session.Value = 0;
                WoFailQty_Session.Value = 0;
                throw ex;
            }

        }


        /// <summary>
        /// 打印工單區間時，加載機種、工單套數以及未使用條碼數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WoRangeInfoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 1)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { });
                throw new MESReturnMessage(ErrMesg);
            }
            T_R_WO_BASE table_rwb = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WO_REGION table_rwr = new T_R_WO_REGION(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WO_REGION_DETAIL table_rwrd = new T_R_WO_REGION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //根據輸入加載工單區間等信息
            string wo = Input.Value.ToString();

            if (wo.Length != 0)
            {
                R_WO_BASE rwb = table_rwb.GetWoByWoNo(wo, Station.SFCDB);
                if (rwb != null) //判斷工單是否存在
                {
                    MESStationSession WoStringSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                    if (WoStringSession == null)
                    {
                        Station.StationSession.Add(new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = wo });
                    }
                    else
                    {
                        WoStringSession.Value = wo;
                    }
                    List<R_WO_REGION> rwrs = table_rwr.GetWObyWONO(wo, Station.SFCDB);
                    if (rwrs.Count > 0) //判斷工單是否申請區間
                    {
                        int RestRange = table_rwrd.GetRestCount(wo, Station.SFCDB);
                        if (RestRange > 0) //判斷工單區間裡面是否還有未使用的 SN
                        {
                            if (Paras.Exists(t => t.SESSION_TYPE == "SKU" && t.SESSION_KEY == "1"))
                            {
                                MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == "SKU" && t.SessionKey == "1");
                                if (SkuSession != null)
                                {
                                    SkuSession.Value = rwb.SKUNO;
                                }
                                else
                                {
                                    Station.StationSession.Add(new MESStationSession() { MESDataType = "SKU", SessionKey = "1", Value = rwb.SKUNO });
                                }
                            }
                            if (Paras.Exists(t => t.SESSION_TYPE == "WOQTY" && t.SESSION_KEY == "1"))
                            {
                                MESStationSession WOQTYSession = Station.StationSession.Find(t => t.MESDataType == "WOQTY" && t.SessionKey == "1");
                                if (WOQTYSession != null)
                                {
                                    WOQTYSession.Value = rwb.WORKORDER_QTY;
                                }
                                else
                                {
                                    Station.StationSession.Add(new MESStationSession() { MESDataType = "WOQTY", SessionKey = "1", Value = rwb.WORKORDER_QTY });
                                }
                            }
                            if (Paras.Exists(t => t.SESSION_TYPE == "RESTQTY" && t.SESSION_KEY == "1"))
                            {
                                MESStationSession RESTRANGESession = Station.StationSession.Find(t => t.MESDataType == "RESTQTY" && t.SessionKey == "1");
                                if (RESTRANGESession != null)
                                {
                                    RESTRANGESession.Value = RestRange;
                                }
                                else
                                {
                                    Station.StationSession.Add(new MESStationSession() { MESDataType = "RESTQTY", SessionKey = "1", Value = RestRange });
                                }
                            }

                            Station.AddMessage("MES00000061", new string[] { wo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                        }
                        else
                        {

                            //沒有剩餘區間可供列印
                            ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180904192654", new string[] { wo });
                            throw new MESReturnMessage(ErrMesg);
                            //Station.AddMessage("MSGCODE20180904192654", new string[] { wo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                        }
                    }
                    else
                    {
                        //沒有申請區間
                        ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180904192526", new string[] { wo });
                        throw new MESReturnMessage(ErrMesg);
                        //Station.AddMessage("MSGCODE20180904192526", new string[] { wo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    }
                }
                else
                {
                    //工單不存在
                    ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { wo });
                    throw new MESReturnMessage(ErrMesg);
                    //Station.AddMessage("MES00000164", new string[] { wo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
            }
        }

        public static void WoRangePrintPageLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Input.Value.ToString().ToUpper().Contains("REPRINT"))
            {
                Station.FindInputByName("WO").Visable = false;
                Station.FindInputByName("COUNT").Visable = false;
                Station.FindInputByName("PRINT").Visable = false;
                Station.FindInputByName("Reprint_SN").Visable = true;
                Station.FindInputByName("REPRINT").Visable = true;
            }
            else
            {
                Station.FindInputByName("WO").Visable = true;
                Station.FindInputByName("COUNT").Visable = true;
                Station.FindInputByName("PRINT").Visable = true;
                Station.FindInputByName("Reprint_SN").Visable = false;
                Station.FindInputByName("REPRINT").Visable = false;
            }
            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Message);
        }

        public static void KeypartFromWoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Wo = string.Empty;
            T_C_KEYPART TCK = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage(""));
            }

            MESStationSession KeypartSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (KeypartSession == null)
            {
                KeypartSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(KeypartSession);
            }

            if (WoSession.Value is WorkOrder)
            {
                Wo = ((WorkOrder)WoSession.Value).WorkorderNo;
            }
            else
            {
                Wo = WoSession.Value.ToString();
            }
            KeypartSession.Value = TCK.GetKeyPartByWoAndStation(Wo, Station.StationName, Station.SFCDB);

        }

        /// <summary>
        /// 加載重工工單的第一站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReworkWoFirstStationLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession ReworkWoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ReworkWoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession FirstStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FirstStationSession == null)
            {
                FirstStationSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(FirstStationSession);
            }

            WorkOrder ReworkWo = null;
            if (ReworkWoSession.Value is WorkOrder)
            {
                ReworkWo = (WorkOrder)ReworkWoSession.Value;
            }
            else
            {
                ReworkWo = new WorkOrder();
                ReworkWo.Init(ReworkWoSession.Value.ToString(), Station.SFCDB);
            }

            C_ROUTE_DETAIL RD = TCRD.GetByRouteIdOrderBySEQASC(ReworkWo.RouteID, Station.SFCDB).FirstOrDefault();
            if (RD != null)
            {
                FirstStationSession.Value = RD.STATION_NAME;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181022144524"));
            }
        }

        /// <summary>
        /// 從工單對象加載工單的 Deviation
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WoDeviationLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_WO_DEVIATION TRWD = new T_R_WO_DEVIATION(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession DeviationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DeviationSession == null)
            {
                DeviationSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(DeviationSession);
            }
            WorkOrder Wo = (WorkOrder)WoSession.Value;

            R_WO_DEVIATION Deviation = TRWD.GetDeviationByWo(Wo.WorkorderNo, Station.SFCDB);
            if (Deviation != null)
            {
                DeviationSession.Value = Deviation.DEVIATION;
            }
        }

        /// <summary>
        /// 根據工單獲取該工單內已經過了當前工站的SN數量
        /// WO  1
        /// PASSQTY 1 輸出變量
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param> 
        public static void WoAlreadyPassLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string Wo = null;
            if (WoSession.Value is WorkOrder)
            {
                Wo = ((WorkOrder)WoSession.Value).WorkorderNo;
            }
            else
            {
                Wo = WoSession.Value.ToString();
            }

            MESStationSession PassQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PassQtySession == null)
            {
                PassQtySession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(PassQtySession);
            }

            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            PassQtySession.Value = TRS.GetWoPassStationQty(Wo, Station.StationName, Station.SFCDB);

        }

        public static void JuniperPacktypeLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string Wo = null;
            if (WoSession.Value is WorkOrder)
            {
                Wo = ((WorkOrder)WoSession.Value).WorkorderNo;
            }
            else
            {
                Wo = WoSession.Value.ToString();
            }

            MESStationSession PacktypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PacktypeSession == null)
            {
                PacktypeSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(PacktypeSession);
            }
            PacktypeSession.Value = "";
            var o137 = Station.SFCDB.ORM.Queryable<MESDataObject.Module.OM.I137_I, MESDataObject.Module.OM.O_ORDER_MAIN>((i, o) => i.ID == o.ITEMID)
                .Where((i, o) => o.PREWO == Wo).Select((i, o) => i).ToList().FirstOrDefault();
            if (o137 != null)
            {

                if (string.IsNullOrEmpty(o137.CARTONLABEL2))
                {
                    PacktypeSession.Value = "Single pack";
                }
                else
                {
                    PacktypeSession.Value = "Bulk pack";
                }
            }
        }

        public static void WOGroupIdLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string Wo = null;
            if (WoSession.Value is WorkOrder)
            {
                Wo = ((WorkOrder)WoSession.Value).WorkorderNo;
            }
            else
            {
                Wo = WoSession.Value.ToString();
            }
            MESStationSession groupIdSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (groupIdSession == null)
            {
                groupIdSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(groupIdSession);
            }
            MESDataObject.Module.Juniper.T_R_PRE_WO_HEAD t_r_wo_head = new MESDataObject.Module.Juniper.T_R_PRE_WO_HEAD(Station.SFCDB, Station.DBType);
            var woObj = t_r_wo_head.GetObjectByWo(Station.SFCDB, Wo);
            groupIdSession.Value = woObj == null ? "" : woObj.GROUPID;
        }

        public static void WoLoaderStation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }
            WorkOrder ObjWorkorder = new WorkOrder();
            string WOSavePoint = Input.Value.ToString();
            try
            {
                T_C_TEMES_STATION_MAPPING t_C_TEMES_STATION_MAPPING = new T_C_TEMES_STATION_MAPPING(Station.SFCDB, Station.DBType);
                ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (ObjWorkorder == null)
                {
                    //throw new Exception("Can Not Find " + WOSavePoint + " 'Information ' !");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { WOSavePoint }));
                }
                Swo.Value = ObjWorkorder;
                Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                string msgcode = ex.Message;
                throw new MESReturnMessage("not found wo infomation");

            }
           
            T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySeqDesc(ObjWorkorder.RouteID,  Station.SFCDB);
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == "Station");
            if (RouteDetails.Count > 0)
            {

                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                foreach (C_ROUTE_DETAIL c in RouteDetails)
                {
                    //退站方法並未對SILOADING做特殊處理，就直接UPDATE當前站下一站為SILOADING，不僅沒用反而是錯的，這裡增加條件排除掉，要退SILOADING暫時只能找IT手動退 Edit By ZHB 20200924
                    var res = Station.SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>()
                        .Where(x => x.MES_STATION == c.STATION_NAME ).Any();
                    if (res)
                    {
                        stationInput.DataForUse.Add(c.STATION_NAME);
                    }
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("mes00000194", new string[] { "cc" }));
            }


            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Message);
        }
    }
}
