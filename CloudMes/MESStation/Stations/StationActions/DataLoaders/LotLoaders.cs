using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class LotLoaders
    {
        /// <summary>
        /// 根據SN對象及傳入的工站加載LOT對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetLotInfoBySNAndStationLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //用於往下一個Action傳遞數據
            MESStationSession sessionLotObject = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLotObject == null)
            {
                sessionLotObject = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionLotObject);
            }
            string station = Paras[2].VALUE;
            if (string.IsNullOrEmpty(station))
            {
                station = Station.StationName;
            }
            SN snObject = (SN)SNSession.Value;
            T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            R_LOT_STATUS lotObject = t_r_lot_status.GetLotBySNAndStation(snObject.SerialNo, station, Station.SFCDB);
            if (lotObject != null)
            {
                sessionLotObject.Value = lotObject;
            }
            else
            {
                sessionLotObject.Value = null;
            }            
        }


        /// <summary>
        /// 把LOT 的,LOT NO,LOT QTY,REJECT QTY,SAMPLE QTY,PASS QTY,FAIL QTY加載到StationSession中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LotDetailLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 7)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionLotObj = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionLotObj == null )
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //用於輸出LOT NO
            MESStationSession sessionLotNo = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLotNo == null)
            {
                sessionLotNo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionLotNo);
            }
            //用於輸出LOT QTY
            MESStationSession sessionLotQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionLotQty == null)
            {
                sessionLotQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionLotQty);
            }
            //用於輸出REJECT QTY
            MESStationSession sessionRejectQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionRejectQty == null)
            {
                sessionRejectQty = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = "", SessionKey = Paras[3].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionRejectQty);
            }
            //用於輸出SAMPLE QTY
            MESStationSession sessionSampleQty = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (sessionSampleQty == null)
            {
                sessionSampleQty = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = "", SessionKey = Paras[4].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionSampleQty);
            }
            //用於輸出PASS QTY
            MESStationSession sessionPassQty = Station.StationSession.Find(s => s.MESDataType == Paras[5].SESSION_TYPE && s.SessionKey == Paras[5].SESSION_KEY);
            if (sessionPassQty == null)
            {
                sessionPassQty = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = "", SessionKey = Paras[5].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionPassQty);
            }
            //用於輸出FAIL QTY
            MESStationSession sessionFailQty = Station.StationSession.Find(s => s.MESDataType == Paras[6].SESSION_TYPE && s.SessionKey == Paras[6].SESSION_KEY);
            if (sessionFailQty == null)
            {
                sessionFailQty = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = "", SessionKey = Paras[6].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionFailQty);
            }

            LotNo lotObj = null;
            if (sessionLotObj.Value is LotNo && sessionLotObj.Value != null)
            {
                lotObj = (LotNo)sessionLotObj.Value;
                sessionLotNo.Value = lotObj.LOT_NO;
                sessionLotQty.Value = lotObj.LOT_QTY;
                sessionRejectQty.Value = lotObj.REJECT_QTY;
                sessionSampleQty.Value = lotObj.SAMPLE_QTY;
                sessionPassQty.Value = lotObj.PASS_QTY;
                sessionFailQty.Value = lotObj.FAIL_QTY;
            }
            else if (sessionLotObj.Value is string && sessionLotObj.Value != null)
            {
                lotObj = new LotNo();
                lotObj.Init(sessionLotObj.Value.ToString(), "", Station.SFCDB);
                sessionLotNo.Value = lotObj.LOT_NO;
                sessionLotQty.Value = lotObj.LOT_QTY;
                sessionRejectQty.Value = lotObj.REJECT_QTY;
                sessionSampleQty.Value = lotObj.SAMPLE_QTY;
                sessionPassQty.Value = lotObj.PASS_QTY;
                sessionFailQty.Value = lotObj.FAIL_QTY;
            }
            else if (sessionLotObj.Value is R_LOT_STATUS && sessionLotObj.Value != null)
            {
                R_LOT_STATUS lotStatus = (R_LOT_STATUS)sessionLotObj.Value;
                sessionLotNo.Value = lotStatus.LOT_NO;
                sessionLotQty.Value = lotStatus.LOT_QTY;
                sessionRejectQty.Value = lotStatus.REJECT_QTY;
                sessionSampleQty.Value = lotStatus.SAMPLE_QTY;
                sessionPassQty.Value = lotStatus.PASS_QTY;
                sessionFailQty.Value = lotStatus.FAIL_QTY;
            }
        }
    }
}
