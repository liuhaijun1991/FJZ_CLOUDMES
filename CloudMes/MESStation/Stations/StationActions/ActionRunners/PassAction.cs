using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using System.Data;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDBHelper;
using MESPubLab.MesClient;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class PassAction
    {
        public static void ValueCompareAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            
            string ErrMessage = string.Empty;
            string AutoEvent = string.Empty;

            if (Paras.Count != 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "2", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取到 value1 對象
            MESStationSession Value1Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Value1Session == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //獲取到 value2 對象
            MESStationSession Value2Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Value1Session == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取到 message 對象
            string message = Paras[2].VALUE;

            if (Value1Session.Value.ToString() != Value2Session.Value.ToString())
            {
                throw new Exception(message);
            }

            
        }

        public static void AutoPassStation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string ErrMessage = string.Empty;
            string AutoEvent = string.Empty;

            if (Paras.Count != 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "2", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            if (SNSession.Value.GetType() == typeof(SN))
            {
                SnObject = (SN)SNSession.Value;
            }
            else
            {
                SnObject = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            //獲取參數中配置的需要自動過站的工站
            AutoEvent = Paras[1].VALUE.ToString();
            if (string.IsNullOrEmpty(AutoEvent))
            {
                throw new MESReturnMessage($@"AutoEvent Config Error, Please Confirm Station Setting!");
            }

            //只有這個工站是路由中最後一個工站才能自動PASS
            string MaxStation = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>()
                .Where(t => t.ROUTE_ID == SnObject.RouteID)
                .OrderBy(t => t.SEQ_NO, SqlSugar.OrderByType.Desc)
                .Select(t => t.STATION_NAME).First();
            //調用檢查之前先提交之前SN過站Action的數據
            //Station.SFCDB.CommitTrain();//試一下不提交事務能不能重新加載到前面Action做的Update信息
            //然後重新加載SN對象
            var rSN = table.LoadSN(SnObject.SerialNo, Station.SFCDB);
            if (rSN.NEXT_STATION == AutoEvent && rSN.NEXT_STATION == MaxStation)
            {
                //調用過站邏輯
                table.PassStation(SnObject.SerialNo, Station.Line, AutoEvent, "AUTOPASS", Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
            }
        }

        public static void AutoInsertWaiBao(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

        }


        public static void AutoPassStationWaiBao(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string station = string.Empty;
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string newSNID = table.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
            R_SN obj = new R_SN();
            MESStationInput input = Station.Inputs.Find(t => t.DisplayName == "SN");
            if (input == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (SNSession == null)
            //{
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
            //    throw new MESReturnMessage(ErrMessage);
            //}
            //MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            //if (SkuSession == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            //}
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            WorkOrder objWorkorder = new WorkOrder();
            objWorkorder = (WorkOrder)WOSession.Value;
            objWorkorder.Init(WOSession.Value.ToString(), Station.SFCDB);
            string WO = WOSession.Value.ToString().Trim().ToUpper();
            //string SKUNO = SkuSession.Value.ToString().Trim().ToUpper();
            obj.ID = newSNID;
            obj.SN = input.Value.ToString();
            obj.SKUNO = objWorkorder.SkuNO;
            obj.WORKORDERNO = WO;
            obj.PLANT = objWorkorder.PLANT;
            obj.ROUTE_ID = objWorkorder.RouteID;
            obj.STARTED_FLAG = "1";
            obj.PACKED_FLAG = "0";
            obj.COMPLETED_FLAG = "0";
            obj.SHIPPED_FLAG = "0";
            obj.REPAIR_FAILED_FLAG = "0";
            obj.CURRENT_STATION = "SMTLOADING";
            obj.NEXT_STATION = "SMTLOADING";
            obj.CUST_PN = objWorkorder.SkuNO;
            obj.VALID_FLAG = "1";
            obj.EDIT_EMP = "SYSTEM";
            obj.START_TIME = GetDBDateTime(Station.SFCDB);
            obj.EDIT_TIME = GetDBDateTime(Station.SFCDB);
            int flag = table.Insert(obj, Station.SFCDB);
            if (flag == 1)
            {
                Station.AddMessage("MES00000029", new string[] { "Insert R_SN OK", input.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "Insert R_SN FAIL" }));
            }
            // SN SnObject = new SN();

            //if (SNSession.Value.GetType() == typeof(SN))
            //{
            //    SnObject = (SN)SNSession.Value;
            //}
            //else
            //{
            //    SnObject = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //}
            string line = Station.DisplayOutput.Find(t => t.Name == "LINE").Value.ToString();
            //for (int i = 1; i < Paras.Count; i++)
            //{
            //    station = Paras[i].VALUE.Trim().ToUpper();
            //    table.RecordPassStationDetail(obj, line, station, station ,Station.BU, Station.SFCDB);
            //switch (station)
            //{
            //    case "SMTLOADING":
            //        table.RecordPassStationDetail(obj, "", "SMTLOADING", "SMTLOADING", Station.BU, Station.SFCDB);
            //        break;
            //    case "SMT2":
            //        table.RecordPassStationDetail(obj, "", "SMT2", "SMT2", Station.BU, Station.SFCDB);
            //        break;
            //    case "VI":
            //        table.RecordPassStationDetail(obj, "", "VI", "VI", Station.BU, Station.SFCDB);
            //        break;
            //    case "MIN-POT":
            //        table.RecordPassStationDetail(obj, "", "MIN-POT", "MIN-POT", Station.BU, Station.SFCDB);
            //        break;
            //    case "ICT":
            //        table.RecordPassStationDetail(obj, "", "ICT", "ICT", Station.BU, Station.SFCDB);
            //        break;
            //    default:
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259"));
            //}
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            List<C_ROUTE_DETAIL> listRoute = TCRD.GetByRouteIdOrderBySEQASC(objWorkorder.RouteID, Station.SFCDB);
            List<string> listStation = listRoute.Select(r => r.STATION_NAME).Distinct().ToList();
            foreach (var lst in listRoute)
            {
                string MaxStation = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>()
                .Where(t => t.ROUTE_ID == objWorkorder.RouteID)
                .OrderBy(t => t.SEQ_NO, SqlSugar.OrderByType.Desc)
                .Select(t => t.STATION_NAME).First();
                var rSN = table.LoadSN(input.Value.ToString(), Station.SFCDB);
                if (rSN.NEXT_STATION == lst.STATION_NAME && rSN.NEXT_STATION != MaxStation &&  rSN.NEXT_STATION != "ICT" && rSN.NEXT_STATION != "STOCKIN" && rSN.NEXT_STATION != "PRE-ASSY")
                {
                    table.PassStation(input.Value.ToString(), Station.Line, lst.STATION_NAME, "AUTOPASS_WAIBAO", Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
                }
            }


            // }
        }
        public static DateTime GetDBDateTime(OleExec SFCDB)
        {
            string strSql = "select sysdate from dual";
            DateTime DBTime = (DateTime)SFCDB.ExecSelectOneValue(strSql);
            return DBTime;
        }
        /// <summary>
        /// 處理SN狀態/記錄過站記錄/統計良率 for TCQS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TCQSPassStationAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string RepairMainID = "";
            SN SnObject = null;
            C_ERROR_CODE failCodeObject = null;
            T_R_SN TRSN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_TCQS_YIELD_RATE_DETAIL TRTCQS = new T_R_TCQS_YIELD_RATE_DETAIL(Station.SFCDB, Station.DBType);
            R_TCQS_YIELD_RATE_DETAIL RTCQS = new R_TCQS_YIELD_RATE_DETAIL();
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            //獲取DB時間,所有數據更新使用同一時間
            DateTime DT = Station.GetDBDateTime();

            if (Paras.Count != 7)
            {
                //參數不正確：配置表中参数不够，应该为 {0} 个，实际只有 {1} 个！
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "7", Paras.Count.ToString() });
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
                //如果該工站沒有設定默認狀態，則默認為PASS
                if (StatusSession.Value == null ||
                    (StatusSession.Value != null && StatusSession.Value.ToString() == ""))
                {
                    StatusSession.Value = "PASS";
                }
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
            MESStationSession TCQSSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (TCQSSession == null)
            {
                //无法获取到 {0} 的数据，请检查工站配置！
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            else
            {
                RTCQS = (R_TCQS_YIELD_RATE_DETAIL)TCQSSession.Value;
            }

            //FailCode,是否有掃描不良代碼
            MESStationSession FailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (FailCodeSession == null)
            {
                //如果沒有，則創建一個該工站的FailCodeSession,且SN狀態默認為該Action中設定的狀態Value = Paras[1].VALUE
                FailCodeSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[1].VALUE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailCodeSession);

            }

            MESStationSession StationNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (StationNoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20180920151156", new string[] { });
                throw new MESReturnMessage(ErrMessage);

            }

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (NextStationSession == null)
            {

            }
            //如果該工站沒有FailCode，則默認為PASS
            if (FailCodeSession.Value == null ||
                (FailCodeSession.Value != null && FailCodeSession.Value.ToString() == ""))
            {
                if (StatusSession.Value.ToString() == "FAIL")
                {
                    //結果為Fail,必須有FailCode
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000284", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                //結果為Fail,FailCodeSession.Value不能有值
                if (StatusSession.Value.ToString() == "PASS")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000285", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                else
                {
                    //獲取FailCode對象
                    failCodeObject = (C_ERROR_CODE)FailCodeSession.Value;
                }
            }

            //如果過站,則按MES原邏輯處理
            if (RTCQS.TOTAL_FRESH_BUILD_QTY > 0 || RTCQS.TOTAL_REWORK_BUILD_QTY > 0)
            {
                if (RTCQS.TOTAL_FRESH_PASS_QTY + RTCQS.TOTAL_REWORK_PASS_QTY > 0)   //Pass過站處理
                {
                    string NextStation = "JOBFINISH";
                    TRSN.PassStation(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, StatusSession.Value.ToString(), Station.LoginUser.EMP_NO, Station.SFCDB);
                    Station.AddMessage("MSGCODE20180529165023", new string[] { "PASS,", SnObject.SerialNo, NextStation }, StationMessageState.CMCMessage);
                }
                else   //Fail過站過理，R_Repair_Main時間與R_SN_Detail時間保持一致用RTCQS.DT
                {
                    RepairMainID = TRTCQS.SNFailByFailCode(RTCQS, failCodeObject, SnObject.SerialNo, Station.BU, StatusSession.Value.ToString(), Station.LoginUser.EMP_NO, Station.SFCDB);
                }
                TRSN.RecordYieldRate(SnObject.WorkorderNo, (double)(RTCQS.TOTAL_FRESH_BUILD_QTY + RTCQS.TOTAL_REWORK_BUILD_QTY), SnObject.SerialNo, StatusSession.Value.ToString(), Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                TRSN.RecordUPH(SnObject.WorkorderNo, (double)(RTCQS.TOTAL_FRESH_BUILD_QTY + RTCQS.TOTAL_REWORK_BUILD_QTY), SnObject.SerialNo, StatusSession.Value.ToString(), Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            }
            //所有測試記錄，都要統計在TCQS的三個表中(TCQS_Yield/Tmp_ATEData/R_SN_Detail)
            ErrMessage = TRTCQS.RecordTCQSYieldRate(RTCQS, SnObject.SerialNo, Station.BU, Station.SFCDB);
            Station.AddMessage("MES00000150", new string[] { SnObject.SerialNo, "TCQS Yield Rate" }, StationMessageState.Pass);

            //如果做過站處理,則提示用戶需要重測
            if (RTCQS.TOTAL_FRESH_BUILD_QTY + RTCQS.TOTAL_REWORK_BUILD_QTY == 0)
            {
                Station.AddMessage("MES00000283", new string[] { SnObject.SerialNo, StatusSession.Value.ToString() }, StationMessageState.Debug);
            }

            //清除FailCodeSession&StatusSession很重要:過站了,清除本次Error信息,避免下一槍掃直接SN時仍以Fail處理(在CMC上)
            //FailCodeSession.Value = null;
            //StatusSession.Value ="";

            //
            //R_AP_TEMP ap = (R_AP_TEMP)StationNoSession.  ;
            //T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB,DB_TYPE_ENUM.Oracle);
            //tap.PassDeleteAp(Station.SFCDB,ap.DATA2);
        }

    }
}
