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

using MESDBHelper;
using SqlSugar;
using MESPubLab.MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class HWD_LOT_ACTION
    {
        public static void LOTNOINPUT(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            string LOTNO = Input.Value.ToString().ToUpper();
            T_R_LOT_STATUS TRLS = new T_R_LOT_STATUS(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            var LOT_COUNT_session = Station.StationSession.Find(t => t.MESDataType == "LOT_COUNT" && t.SessionKey == "1");
            if (LOT_COUNT_session == null)
            {
                LOT_COUNT_session = new MESStationSession() { MESDataType = "LOT_COUNT", SessionKey = "1", Value = 0 };
                Station.StationSession.Add(LOT_COUNT_session);
            }
            var Lot = TRLS.GetByLotNo(LOTNO, DB);
            if (Lot.ID == null || Lot.ID == "")
            {
                throw new Exception($@"LotNo '{LOTNO}' is not exten");
            }
            if (Lot.CLOSED_FLAG == "1")
            {
                throw new Exception($@"LotNo '{LOTNO}' is closed");
            }
            if (Lot.SAMPLE_STATION != Station.DisplayName)
            {
                throw new Exception($@"LotNo '{LOTNO}' is use to '{Lot.SAMPLE_STATION}'");
            }
                var LOT_session = Station.StationSession.Find(t => t.MESDataType == "LOTNO" && t.SessionKey == "1");
            if (LOT_session == null)
            {
                LOT_session = new MESStationSession() { MESDataType = "LOTNO", SessionKey = "1" };
                Station.StationSession.Add(LOT_session);
            }
            
            LOT_session.Value = LOTNO;
            int count = DB.ORM.Queryable<R_LOT_DETAIL>().Where(t => t.LOT_ID == Lot.ID).Count();
            LOT_COUNT_session.Value = count;

            Station.NextInput = Station.Inputs[1];
        }
        public static void SNInputToLot(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            string strSql = "";
            var LOT_session = Station.StationSession.Find(t => t.MESDataType == "LOTNO" && t.SessionKey == "1");
            if (LOT_session == null)
            {
                LOT_session = new MESStationSession() { MESDataType = "LOTNO", SessionKey = "1" };
                Station.StationSession.Add(LOT_session);
            }
            var LOT_COUNT_session = Station.StationSession.Find(t => t.MESDataType == "LOT_COUNT" && t.SessionKey == "1");
            if (LOT_COUNT_session == null)
            {
                LOT_COUNT_session = new MESStationSession() { MESDataType = "LOT_COUNT", SessionKey = "1" , Value=0};
                Station.StationSession.Add(LOT_COUNT_session);
            }

            var SN_Session = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");

            SN sn = (SN)SN_Session.Value;
            if (sn.CompletedFlag != "1")
            {
                throw new Exception($@"'{sn.SerialNo}'  is not  Completed");
            }
            string lotno = "";//LOT_session.Value.ToString();
            T_R_LOT_STATUS TRLS = new T_R_LOT_STATUS(DB, DB_TYPE_ENUM.Oracle);
            Row_R_LOT_STATUS RRLS = (Row_R_LOT_STATUS)TRLS.NewRow();
            if (LOT_session.Value == null)
            {
                RRLS.ID = TRLS.GetNewID(Station.BU, DB);
                RRLS.SKUNO = sn.SkuNo;
                RRLS.LOT_QTY = 0;
                RRLS.LOT_STATUS_FLAG = "0";
                RRLS.SAMPLE_STATION = Station.DisplayName;
                RRLS.CLOSED_FLAG = "0";
                RRLS.LOT_NO = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN("HWDLOT", DB);
                RRLS.EDIT_TIME = DateTime.Now;
                RRLS.EDIT_EMP = Station.LoginUser.EMP_NO;
                DB.ExecSQL(RRLS.GetInsertString(DB_TYPE_ENUM.Oracle));
                RRLS.AcceptChange();
                lotno = RRLS.LOT_NO;
            } else
            {
                lotno = LOT_session.Value.ToString();
                strSql = $@"select * from R_LOT_STATUS where lot_no = '{lotno}'";
                var d = DB.RunSelect(strSql);
                RRLS.loadData(d.Tables[0].Rows[0]);
            }
            LOT_session.Value = lotno;
            if (RRLS.CLOSED_FLAG == "1")
            {
                throw new Exception($@"LotNo '{RRLS.LOT_NO}' is closed");
            }


            strSql = $@"select l.lot_no from r_lot_status l inner join r_lot_detail d on 
l.lot_no = d.lot_id  where sn = '{sn.SerialNo}' and l.lot_no like 'LOT-%' and d.status = '0'";
            
            var data = DB.RunSelect(strSql);
            if (data.Tables[0].Rows.Count > 0)
            {
                throw new Exception($@"'{sn.SerialNo}'  is in lot'{data.Tables[0].Rows[0]["lot_no"].ToString()}'");
            }
            if (sn.SkuNo != RRLS.SKUNO)
            {
                throw new Exception($@"'{sn.SerialNo}': '{sn.SkuNo}'  but lot is '{RRLS.SKUNO}'");
            }

            T_R_LOT_DETAIL TRLD = new T_R_LOT_DETAIL(DB, DB_TYPE_ENUM.Oracle);
            R_LOT_DETAIL R = new R_LOT_DETAIL();

            R.ID = TRLD.GetNewID(Station.BU, DB);
            R.LOT_ID = RRLS.LOT_NO;
            R.SN = sn.SerialNo;
            R.STATUS = "0";
            R.EDIT_EMP = Station.LoginUser.EMP_NO;
            R.EDIT_TIME = DateTime.Now;
            DB.ORM.Insertable<R_LOT_DETAIL>(R).ExecuteCommand();

            T_R_SN TRS = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            TRS.RecordPassStationDetail(sn.SerialNo, Station.Line, Station.StationName, Station.DisplayName, Station.BU, DB);

            int count = DB.ORM.Queryable<R_LOT_DETAIL>().Where(t => t.LOT_ID == R.LOT_ID).Count();
            RRLS.LOT_QTY = count;
            RRLS.EDIT_TIME = DateTime.Now;
            RRLS.EDIT_EMP = Station.LoginUser.EMP_NO;
            DB.ExecSQL(RRLS.GetUpdateString(DB_TYPE_ENUM.Oracle));
            RRLS.AcceptChange();
            LOT_COUNT_session.Value = count;
            Station.NextInput = Station.Inputs[1];

        }

        public static void LotClose(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            string strSql = "";
            var LOT_session = Station.StationSession.Find(t => t.MESDataType == "LOTNO" && t.SessionKey == "1");
            if (LOT_session == null)
            {
                LOT_session = new MESStationSession() { MESDataType = "LOTNO", SessionKey = "1" };
                Station.StationSession.Add(LOT_session);
            }
            T_R_LOT_STATUS TRLS = new T_R_LOT_STATUS(DB, DB_TYPE_ENUM.Oracle);
            Row_R_LOT_STATUS RRLS = (Row_R_LOT_STATUS)TRLS.NewRow();
            string lotno = "";
            if (LOT_session.Value == null)
            {
                return;
            }
            else
            {
                lotno = LOT_session.Value.ToString();
                strSql = $@"select * from R_LOT_STATUS where lot_no = '{lotno}'";
                var d = DB.RunSelect(strSql);
                RRLS.loadData(d.Tables[0].Rows[0]);
            }
            RRLS.CLOSED_FLAG = "1";
            RRLS.EDIT_EMP = Station.LoginUser.EMP_NO;
            RRLS.EDIT_TIME = DateTime.Now;
            RRLS.LOT_QTY = DB.ORM.Queryable<R_LOT_DETAIL>().Where(t => t.LOT_ID == lotno).Count();
            DB.ExecSQL(RRLS.GetUpdateString(DB_TYPE_ENUM.Oracle));

            Station.StationSession.Clear();
            Station.NextInput = Station.Inputs[0];
        }

        public static void OutLot(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            //string strSql = "";
            var SN_Session = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");
            SN sn = (SN)SN_Session.Value;

            var LotStates = DB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>
                ((lot, detail) => lot.LOT_NO == detail.LOT_ID)
                .Where((lot, detail) => detail.SN == sn.SerialNo && detail.STATUS == "0" && SqlFunc.StartsWith(lot.LOT_NO, "LOT-"))
                .Select((lot, detail) => new { LOT = lot, Detail = detail }).ToList();

            
            if (LotStates.Count == 0)
            {
                throw new Exception($@"{sn.SerialNo } not in Lot");
            }
            T_R_SN TRS = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);

            for (int i = 0; i < LotStates.Count; i++)
            {
                var LS = LotStates[i];
                LS.Detail.STATUS = "1";
                LS.Detail.EDIT_EMP = Station.LoginUser.EMP_NO;
                LS.Detail.EDIT_TIME = DateTime.Now;
                DB.ORM.Updateable<R_LOT_DETAIL>(LS.Detail).Where(t => t.ID == LS.Detail.ID).ExecuteCommand();
                TRS.RecordPassStationDetail(sn.SerialNo, Station.Line, Station.StationName, LS.LOT.SAMPLE_STATION, Station.BU, DB);
                Station.StationMessages.Add(new StationMessage()
                { Message = $@"'{sn.SerialNo}' Scanout Lot'{LS.LOT.LOT_NO}','{LS.LOT.SAMPLE_STATION}'", State= StationMessageState.Message });
                
            }



        }



    }
}
