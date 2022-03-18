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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.Management;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckLocker
    {
        /// <summary>
        /// 檢查SN/PanelSN是否被鎖
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLockedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //input test
            //string inputValue = Input.Value.ToString();
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");
            //if (snSession == null)
            //{
            //    throw new MESReturnMessage("SN加載異常");
            //}
            //SN sn = (SN) snSession.Value;
            LogicObject.SN SN = null;
            if (snSession.Value is SN)
            {
                SN = (SN)snSession.Value;
            }
            else
            {
                SN = new SN(snSession.Value.ToString(), Station.SFCDB, Station.DBType);
            }

            SN.LockCheck(Station.SFCDB);

            //OleExec sfcdb = Station.SFCDB;
            ////R_SN_LOCK r_sn_lock = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySN(sfcdb, sn.SerialNo, Station.StationName);//sn.SerialNo,sn.CurrentStation
            //R_SN_LOCK r_sn_lock = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySN(sfcdb, Input.Value.ToString(), Station.StationName);//sn.SerialNo,sn.CurrentStation
            //if (r_sn_lock != null)
            //{
            //    Station.AddMessage("MES00000044", new string[] { "SN", r_sn_lock.SN, r_sn_lock.LOCK_EMP }, StationMessageState.Fail);
            //    //return;
            //    throw new MESReturnMessage("SN被鎖定");
            //}

        }

        /// <summary>
        /// 檢查線體是否被鎖
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LineLockedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //GET LINE
            string LineName = Station.Line;
            string CurrentStation = Station.StationName;

            if (string.IsNullOrEmpty(LineName))
            {
                //throw new MESReturnMessage("LINE線體加載異常");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140405"));
            }
            OleExec sfcdb = Station.SFCDB;
            R_SN_LOCK r_sn_lock = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySN(sfcdb, LineName, CurrentStation);//sn.SerialNo,sn.CurrentStation
            if (r_sn_lock != null)
            {
                Station.AddMessage("MES00000044", new string[] { "LINE", r_sn_lock.SN, r_sn_lock.LOCK_EMP, r_sn_lock.LOCK_REASON }, StationMessageState.Fail);
                //return;
                throw new MESReturnMessage("線體被鎖定");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140523"));
            }

        }


        /// <summary>
        /// 檢查 SN 上料是否齊全
        /// 兩個參數，SN 對象，WO 對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTMaterialChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 1)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { });
                throw new MESReturnMessage(ErrMesg);
            }
            OleExec apdb = Station.DBS["APDB"].Borrow();

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            SN sn = (SN)SNSession.Value;
            AP_DLL APDLL = new AP_DLL();

            string result = "";// APDLL.CheckSmtMaterial(sn.SerialNo, Station.StationName, apdb);
            if (Station.BU.Equals("FJZ"))
            {
                if (Station.StationName == "SMT1" || Station.StationName == "AOI1")
                {
                    result = APDLL.CheckSmtMaterial(sn.SerialNo, "AOI2", apdb);
                }
                else if (Station.StationName == "VI")
                {
                    result = APDLL.CheckSmtMaterial(sn.SerialNo, "VI", apdb);
                }
                else
                {
                    result = APDLL.CheckSmtMaterial(sn.SerialNo, "AOI4", apdb);
                }

            }
            else
            {
                result = APDLL.CheckSmtMaterial(sn.SerialNo, Station.StationName, apdb);
            }


            if (!result.Equals("OK"))
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20181228163221", new string[] { result });
                throw new MESReturnMessage(ErrMesg);
            }

            Station.AddMessage("MSGCODE20180906202300", new string[] { sn.SerialNo }, StationMessageState.Message);
            Station.DBS["APDB"].Return(apdb);




        }

        /// <summary>
        /// 檢查當前工單是否上料齊套
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTAPMaterialChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //return;
            //input test
            //string inputValue = Input.Value.ToString();
            MESStationSession WO_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WO_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "WO" }));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            MESStationSession StationName_Session = null;
            if (Paras.Count == 3)
            {
                StationName_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            }

            string wo = WO_Session.Value.ToString();
            //Modify By LLF 2018-01-26 SN_Session.Value是對象，取InputValue
            //string sn = SN_Session.Value.ToString();
            string sn = SN_Session.InputValue.ToString();
            string line = Station.Line;
            string stationName = Station.StationName;
            if (StationName_Session != null)
            {
                stationName = StationName_Session.Value.ToString();
            }

            OleExec apdb = Station.APDB; //apdbPool.Borrow();
            string msg = "";
            if (Station.BU.Equals("FJZ"))
            {
                AP_DLL ap = new AP_DLL();
                if (stationName == "SMT1" || stationName == "AOI1")
                {
                    msg = ap.CheckSmtMaterial(sn, "AOI2", apdb);
                }
                else if (stationName == "VI" || stationName == "PTH")
                {
                    msg = ap.CheckSmtMaterial(sn, "VI", apdb);
                }
                else if (stationName == "PRESS-FIT")
                {
                    msg = ap.CheckSmtMaterial(sn, "VI2", apdb);
                }
                else
                {
                    msg = ap.CheckSmtMaterial(sn, "AOI4", apdb);
                }

            }
            else
            {
                OleDbParameter[] paras = new OleDbParameter[] {
                new OleDbParameter("G_PSN", sn),
                new OleDbParameter("G_WO", wo),
                new OleDbParameter("G_STATION", line),
                new OleDbParameter("G_EVENT", stationName),
                new OleDbParameter(":RES", OleDbType.VarChar, 800)
                };
                paras[4].Direction = ParameterDirection.Output;
                msg = apdb.ExecProcedureNoReturn("MES1.CMC_INSERTDATA_SP", paras);
            }

            if (msg.ToUpper().IndexOf("OK") == 0)
            {
                Station.AddMessage("MES00000047", new string[] { "wo" }, StationMessageState.Pass);//wo
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000046", new string[] { msg }));
            }
        }

        /// <summary>
        /// 檢查Data是否在鎖記錄中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckerDataIsLocked(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession dataSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (dataSession == null || dataSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            List<R_SN_LOCK> locks = Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where((r) => r.SN == dataSession.Value.ToString() && r.LOCK_STATUS == "1").ToList();
            if (locks.Count > 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000044", new string[] { locks[0].TYPE, dataSession.Value.ToString(), locks[0].LOCK_EMP }));
            }

        }
        /// <summary>
        /// 檢查Panel是否被鎖定
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelLockedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            try
            {
                LogicObject.Panel objPannel = null;
                if (sessionPanel.Value is Panel)
                {
                    objPannel = (Panel)sessionPanel.Value;
                }
                else
                {
                    objPannel = new Panel(sessionPanel.Value.ToString(), Station.SFCDB, Station.DBType);
                }
                objPannel.LockCheck(Station.SFCDB, Station.StationName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 檢查TEST HIPOT 及 BURNIN  Status是否Fail的次数>=2
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TestStatusLockedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sWO == null || sWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            OleExec sfcdb = null;
            WorkOrder wo = (WorkOrder)sWO.Value;
            T_R_SN_LOCK t_r_sn_lock = null;
            Row_R_SN_LOCK rowSNLock = null;
            string stationName = Station.StationName;
            DateTime? dateTime = null;
            DateTime date = Station.GetDBDateTime();
            try
            {
                sfcdb = Station.SFCDB;
                t_r_sn_lock = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle);
                List<R_TEST_DETAIL_VERTIV> tCount = null;
                var unTime = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(it => it.WORKORDERNO == wo.WorkorderNo && it.TYPE == "WO" && it.LOCK_STATUS == "0" && it.LOCK_STATION == stationName).OrderBy(it => it.UNLOCK_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                if (unTime == null)
                {
                    tCount = sfcdb.ORM.Queryable<R_TEST_DETAIL_VERTIV, R_SN>((rtd, rsn) => rtd.SN == rsn.SN).Where((rtd, rsn) => rsn.WORKORDERNO == wo.WorkorderNo && rsn.VALID_FLAG == "1" && rtd.STATE == "FAIL" && rtd.STATION == rsn.NEXT_STATION && rtd.STATION == stationName).Select((rtd, rsn) => rtd).ToList();
                }
                else
                {
                    tCount = sfcdb.ORM.Queryable<R_TEST_DETAIL_VERTIV, R_SN>((rtd, rsn) => rtd.SN == rsn.SN).Where((rtd, rsn) => rsn.WORKORDERNO == wo.WorkorderNo && rsn.VALID_FLAG == "1" && rtd.STATE == "FAIL" && rtd.STATION == rsn.NEXT_STATION && rtd.STATION == stationName && rtd.CREATETIME > unTime.UNLOCK_TIME).Select((rtd, rsn) => rtd).ToList();
                }

                if (tCount.Count >= 2)
                {
                    var sCount = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(it => it.WORKORDERNO == wo.WorkorderNo && it.TYPE == "WO" && it.LOCK_STATUS == "1" && it.LOCK_STATION == stationName).ToList();
                    if (sCount.Count > 0)
                    {
                        //throw new MESReturnMessage($@"{wo.WorkorderNo} ->{stationName} 鎖定,請聯繫QE處理！");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140545", new string[] { wo.WorkorderNo, stationName }));
                    }
                    else
                    {
                        sCount = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(it => it.WORKORDERNO == wo.WorkorderNo && it.TYPE == "WO" && it.LOCK_STATUS == "0" && it.LOCK_STATION == stationName).ToList();
                        if (sCount.Count > 0)
                        {
                            sfcdb.ORM.Updateable<R_SN_LOCK>().UpdateColumns(it => new R_SN_LOCK() { LOCK_STATUS = "1", UNLOCK_REASON = "", LOCK_TIME = date, UNLOCK_EMP = "", UNLOCK_TIME = dateTime }).Where(it => it.WORKORDERNO == wo.WorkorderNo && it.TYPE == "WO" && it.LOCK_STATUS == "0" && it.LOCK_STATION == stationName).ExecuteCommand();
                        }
                        else
                        {
                            rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                            rowSNLock.ID = t_r_sn_lock.GetNewID(Station.BU, sfcdb);
                            rowSNLock.WORKORDERNO = wo.WorkorderNo;
                            rowSNLock.TYPE = "WO";
                            rowSNLock.LOCK_STATION = stationName;
                            rowSNLock.LOCK_REASON = stationName + ",測試中出現測試不良大於等於2PCS鎖定";
                            rowSNLock.LOCK_STATUS = "1";
                            rowSNLock.LOCK_EMP = "SYSTEM";
                            rowSNLock.LOCK_TIME = Station.GetDBDateTime();
                            sfcdb.ThrowSqlExeception = true;
                            sfcdb.ExecSQL(rowSNLock.GetInsertString(DB_TYPE_ENUM.Oracle));
                            sfcdb.CommitTrain();
                            //throw new MESReturnMessage($@"{stationName}-->測試中出現不良大於等於2PCS鎖定,請聯繫QE處理！");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140612", new string[] { stationName }));
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
        /// Lock Check
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LockChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionData = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionData == null || sessionData.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var param = sessionData.Value.ToString();
            if (Paras[0].VALUE == "" || Paras[0].VALUE == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " Value" }));
            }
            var paramType = Paras[0].VALUE.Trim().ToUpper();
            var Alert = "FALSE";
            if (Paras.Count > 1 && Paras[1].SESSION_TYPE.ToString() == "AlertWindow")
            {
                Alert = Paras[1].VALUE.ToString().ToUpper();
            }
            List<R_SN_LOCK> locks = new List<R_SN_LOCK>();
            try
            {
                locks = LockManager.CheckLock(param, paramType, Station.StationName, Station.SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (locks.Count > 0)
            {
                var emp_lock_info = Station.SFCDB.ORM.Queryable<C_USER>().Where(u => u.EMP_NO == locks[0].LOCK_EMP).First();

                string emp_lock = emp_lock_info != null ? emp_lock_info.DPT_NAME.Trim().ToUpper() + ": " + emp_lock_info.EMP_NAME.Trim().ToUpper() : locks[0].LOCK_EMP;

                if (Alert == "TRUE")
                {
                    //Station.AddMessage("MES00000044",
                    //new string[] { locks[0].TYPE, locks[0].WORKORDERNO, locks[0].LOCK_EMP },
                    Station.AddMessage("MSGCODE20210628172337",
                    new string[] { locks[0].TYPE, locks[0].WORKORDERNO, emp_lock, locks[0].LOCK_REASON },
                        MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail,
                        MESPubLab.MESStation.MESReturnView.Station.StationMessageDisplayType.Swal
                    );
                }
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000044", new string[] { locks[0].TYPE, sessionData.Value.ToString(), locks[0].LOCK_EMP }));
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210628172337", new string[] { locks[0].TYPE, sessionData.Value.ToString(), emp_lock, locks[0].LOCK_REASON }));
            }

        }

    }
}
