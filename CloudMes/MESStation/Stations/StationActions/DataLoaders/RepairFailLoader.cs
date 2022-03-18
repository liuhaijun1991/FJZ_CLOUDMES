using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class RepairFailLoader
    {
        /// <summary>
        /// 從SN對象中加載RepairFail訊息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFailDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            //Repair Session
            MESStationSession RepairMain = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (RepairMain == null)
            {
                RepairMain = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(RepairMain);
            }
            MESStationSession RepairFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (RepairFailCode == null)
            {
                RepairFailCode = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(RepairFailCode);
            }
            string sn = SN_Session.Value.ToString();
            OleExec sfcdb = Station.SFCDB;
            //存入R_REPAIR_MAIN信息
            Dictionary<string, List<R_REPAIR_MAIN>> repairMainInfo = new Dictionary<string, List<R_REPAIR_MAIN>>();
            List<R_REPAIR_MAIN> repairMains = new T_R_REPAIR_MAIN(sfcdb, DB_TYPE_ENUM.Oracle).GetRepairMainBySN(sfcdb, sn);
            repairMainInfo.Add("R_REPAIR_MAIN", repairMains);
            RepairMain.Value = repairMainInfo;
            //存入R_REPAIR_FAILCODE信息
            Dictionary<string, List<R_REPAIR_FAILCODE>> repairFailCodeInfo = new Dictionary<string, List<R_REPAIR_FAILCODE>>();
            List<R_REPAIR_FAILCODE> failCodes = new T_R_REPAIR_FAILCODE(sfcdb, DB_TYPE_ENUM.Oracle).GetFailCodeBySN(sfcdb, sn);
            repairFailCodeInfo.Add("R_REPAIR_FAILCODE", failCodes);
            RepairFailCode.Value = repairFailCodeInfo;
        }

        /// <summary>
        /// 從SN對象中加載SN的維修訊息和FAIL訊息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FailDataForRepairDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            //Repair Session
            MESStationSession RepairMain = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (RepairMain == null)
            {
                RepairMain = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(RepairMain);
            }

            MESStationSession RepairAction = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (RepairAction == null)
            {
                RepairAction = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(RepairAction);
            }

            MESStationSession RepairReplace = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (RepairReplace == null)
            {
                RepairReplace = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(RepairReplace);
            }

            MESStationSession RepairFailCodeHis = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (RepairFailCodeHis == null)
            {
                RepairFailCodeHis = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(RepairFailCodeHis);
            }

            MESStationSession RepairFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (RepairFailCode == null)
            {
                RepairFailCode = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY };
                Station.StationSession.Add(RepairFailCode);
            }

            SN SnObject = (SN)SN_Session.Value;
            string sn = SnObject.SerialNo;
            OleExec sfcdb = Station.SFCDB;
            //存入R_REPAIR_MAIN信息
            Dictionary<string, object> repairMainInfo = new Dictionary<string, object>();
            var repairMains = sfcdb.ORM.Queryable<R_REPAIR_MAIN>().Where((m) => m.SN == sn)
                .Select((m) => new { m.CREATE_TIME, m.FAIL_LINE, m.FAIL_STATION, m.CLOSED_FLAG }).ToList();
            if (repairMains.Count == 0)
            {
                object a = new { CREATE_TIME = "", FAIL_LINE = "", FAIL_STATION = "", CLOSED_FLAG = "" };
                List<object> obj = new List<object>();
                obj.Add(a);
                repairMainInfo.Add("R_REPAIR_MAIN", obj);
            }
            else
            {
                repairMainInfo.Add("R_REPAIR_MAIN", repairMains);
            }
            RepairMain.Value = repairMainInfo;

            //存入R_REPAIR_ACTION信息
            Dictionary<string, object> rAction = new Dictionary<string, object>();
            var rActions = sfcdb.ORM.Queryable<R_REPAIR_ACTION>().Where((a) => a.SN == sn).OrderBy((a) => a.REPAIR_TIME)
                .Select((a) => new { a.ACTION_CODE, a.REPAIR_EMP, a.REPAIR_TIME }).ToList();
            if (rActions.Count == 0)
            {
                object a = new { ACTION_CODE = "", REPAIR_EMP = "", REPAIR_TIME = "" };
                List<object> obj = new List<object>();
                obj.Add(a);
                rAction.Add("R_REPAIR_ACTION", obj);
            }
            else
            {
                rAction.Add("R_REPAIR_ACTION", rActions);
            }
            RepairAction.Value = rAction;

            //存入R_REPAIR_ACTION信息
            Dictionary<string, object> rReplace = new Dictionary<string, object>();
            var sql = sfcdb.ORM.Queryable<R_REPAIR_ACTION>().Where((a) => a.SN == sn && a.NEW_KEYPART_SN != null).OrderBy((a) => a.REPAIR_TIME)
                .Select((a) => new { PartNo = a.KP_NO, OriginalSN = a.KEYPART_SN, NewPartNo = a.NEW_KP_NO, RepairSN = a.NEW_KEYPART_SN });
            var str = sql.ToSql();
            var rReplaces = sfcdb.ORM.Queryable<R_REPAIR_ACTION>().Where((a) => a.SN == sn && a.NEW_KEYPART_SN != null).OrderBy((a) => a.REPAIR_TIME)
                .Select((a) => new { PartNo = a.KP_NO, OriginalSN = a.KEYPART_SN, NewPartNo = a.NEW_KP_NO, RepairSN = a.NEW_KEYPART_SN }).ToList();
            if (rReplaces.Count == 0)
            {
                object a = new { PartNo = "", OriginalSN = "", NewPartNo = "", RepairSN = "" };
                List<object> obj = new List<object>();
                obj.Add(a);
                rAction.Add("REPAIR_REPLACE", obj);
            }
            else
            {
                rAction.Add("REPAIR_REPLACE", rReplaces);
            }
            RepairReplace.Value = rReplaces;

            //存入R_REPAIR_FAILCODE_HIS信息
            Dictionary<string, object> repairFailCodeHis = new Dictionary<string, object>();
            var failCodesHis = sfcdb.ORM.Queryable<R_REPAIR_FAILCODE>().Where((f) => f.SN == sn && f.REPAIR_FLAG == "1").OrderBy((f) => f.MAIN_ID).Select((f) => new { f.CREATE_TIME, f.FAIL_CODE, f.F_PROCESS, f.F_LOCATION, f.DESCRIPTION, f.REPAIR_FLAG }).ToList();
            if (failCodesHis.Count == 0)
            {
                object a = new { CREATE_TIME = "", FAIL_CODE = "", FAIL_PROCESS = "", FAIL_LOCATION = "", DESCRIPTION = "", REPAIR_FLAG = "" };
                List<object> obj = new List<object>();
                obj.Add(a);
                repairFailCodeHis.Add("R_REPAIR_FAILCODE_HIS", obj);
            }
            else
            {
                repairFailCodeHis.Add("R_REPAIR_FAILCODE_HIS", failCodesHis);
            }
            RepairFailCodeHis.Value = repairFailCodeHis;

            //存入R_REPAIR_FAILCODE信息
            Dictionary<string, object> repairFailCode = new Dictionary<string, object>();
            var failCodes = sfcdb.ORM.Queryable<R_REPAIR_FAILCODE>().Where((f) => f.SN == sn && f.REPAIR_FLAG == "0").OrderBy((f) => f.FAIL_TIME).Select((f) => new { f.ID, f.CREATE_TIME, f.FAIL_CODE, f.F_PROCESS, f.F_LOCATION, f.DESCRIPTION, f.REPAIR_FLAG }).ToList();
            if (failCodes.Count == 0)
            {
                object a = new { ID = "", CREATE_TIME = "", FAIL_CODE = "", FAIL_PROCESS = "", FAIL_LOCATION = "", DESCRIPTION = "", REPAIR_FLAG = "" };
                List<object> obj = new List<object>();
                obj.Add(a);
                repairFailCode.Add("R_REPAIR_FAILCODE", obj);
            }
            else
            {
                repairFailCode.Add("R_REPAIR_FAILCODE", failCodes);
            }
            RepairFailCode.Value = repairFailCode;

            var input = Station.Inputs.Find(t => t.DisplayName == Paras[5].VALUE);
            input.DataForUse.Clear();
            for (int i = 0; i < failCodes.Count(); i++)
            {
                input.DataForUse.Add(failCodes[i]);
            }
        }


        /// <summary>
        /// 加載SN的下階料號
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNKeyPartNoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            string inputName = Paras[1].VALUE;
            List<string> KP_NAME = new List<string>();
            for (int i = 2; i < Paras.Count; i++)
            {
                KP_NAME.Add(Paras[i].VALUE);
            }

            OleExec db = Station.SFCDB;
            string sn = SN_Session.Value.ToString();
            List<R_SN_KP> kps = new List<R_SN_KP>();

            List<R_SN_KP> r_sn = db.ORM.Queryable<R_SN_KP>().Where((A) => A.SN == sn && (KP_NAME.Contains(A.KP_NAME) || KP_NAME.Contains("*"))).ToList();
            kps.AddRange(r_sn);
            for (int i = 0; i < r_sn.Count; i++)
            {
                List<R_SN_KP> R = db.ORM.Queryable<R_SN_KP>().Where((A) => A.SN == r_sn[i].VALUE && (KP_NAME.Contains(A.KP_NAME) || KP_NAME.Contains("*"))).ToList();
                kps.AddRange(R);
            }
            List<string> PartNoList = kps.Select(t => t.PARTNO).Distinct().ToList();
            var input = Station.Inputs.Find(t => t.DisplayName == Paras[1].VALUE);
            input.DataForUse.Clear();
            for (int i = 0; i < PartNoList.Count(); i++)
            {
                if (!input.DataForUse.Contains(PartNoList[i]))
                {
                    input.DataForUse.Add(PartNoList[i]);
                }
            }
        }

        /// <summary>
        /// 從SN對象中加載RepairFailCode訊息   // 同上一个方法一样，只是传出数据格式变化
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFailCodeDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            //Repair Action Session
            MESStationSession RepairAction = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (RepairAction == null)
            {
                RepairAction = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(RepairAction);
            }
            MESStationSession RepairFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (RepairFailCode == null)
            {
                RepairFailCode = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(RepairFailCode);
            }
            string sn = SN_Session.Value.ToString();

            DataTable FailCodeInfo = new DataTable();
            T_R_REPAIR_FAILCODE TFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            FailCodeInfo = TFailcode.SelectFailCodeBySN(sn, Station.SFCDB, Station.DBType);
            if (FailCodeInfo.Rows.Count == 0)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    MESStationSession TempSession = Station.StationSession.Find(t => t.MESDataType == output.SESSION_TYPE && t.SessionKey == output.SESSION_KEY);
                    if (TempSession != null)
                    {
                        TempSession.Value = "";
                    }
                }
                var repairmain = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN>()
                    .Where(t => t.SN == sn && t.CLOSED_FLAG == "0")
                    .First();
                if (repairmain == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616102950", new string[] { sn }));
                }
                else
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage
                    {
                        Message = "Not Fail Record,Please Click Save Finish",
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message
                    });
                }
            }
            RepairFailCode.Value = ConvertToJson.DataTableToJson(FailCodeInfo);
            DataTable RepairActionInfo = new DataTable();
            T_R_REPAIR_ACTION TAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            RepairActionInfo = TAction.SelectRepairActionBySN(sn, Station.SFCDB, Station.DBType);
            RepairAction.Value = ConvertToJson.DataTableToJson(RepairActionInfo);
        }

        /// <summary>
        /// 掃描不良從輸入加載FAIL_CODE對象 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FailCodeDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)

        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string inputValue = Input.Value.ToString();
            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionFailCode == null)
            {
                sessionFailCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionFailCode);
            }
            else
            {
                //inputValue = sessionFailCode.Value.ToString();
                sessionFailCode.Value = null;
            }

            T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_ERROR_CODE failCodeObject = t_c_error_code.GetByErrorCode(inputValue, Station.SFCDB);

            if (failCodeObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { inputValue }));
            }
            else
            {
                sessionFailCode.Value = failCodeObject;
                Station.Inputs[Station.Inputs.Count - 1].Value = failCodeObject.ENGLISH_DESC.ToString();
            }
        }

        /// <summary>
        /// 掃描不良從輸入加載FAIL_CODE&STATUS對象 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FailCodeDataloaderForCMC(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string inputValue = Input.Value.ToString();
            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionFailCode == null)
            {
                sessionFailCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionFailCode);
            }
            else
            {
                //清空Failcode對象
                sessionFailCode.Value = null;
            }

            T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_ERROR_CODE failCodeObject = t_c_error_code.GetByErrorCode(inputValue, Station.SFCDB);

            if (failCodeObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { inputValue }));
            }
            else
            {
                sessionFailCode.Value = failCodeObject;

                //記錄或變更產品狀態為FAIL
                MESStationSession sessionSTATUS = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (sessionSTATUS == null)
                {
                    sessionSTATUS = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = "FAIL", InputValue = failCodeObject.ERROR_CODE, ResetInput = Input };
                    Station.StationSession.Add(sessionSTATUS);
                }
                else
                {
                    sessionSTATUS.Value = "FAIL";
                    sessionSTATUS.InputValue = failCodeObject.ERROR_CODE;
                }
            }

        }

        public static void FailCodeMultipleDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession FailIndex = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (FailIndex == null)
            {
                FailIndex = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = 0, ResetInput = Input };
                Station.StationSession.Add(FailIndex);
            }
            if (FailIndex.Value == null)
            {
                FailIndex.Value = 0;
            }
            MESStationSession FailCount = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCount == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "FailCount" }));
            }
            MESStationSession FailCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == FailIndex.Value.ToString());
            if (FailCode == null)
            {
                FailCode = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = FailIndex.Value.ToString(), ResetInput = Input };
                Station.StationSession.Add(FailCode);
            }
            MESStationSession FailDesc = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == FailIndex.Value.ToString());
            if (FailDesc == null)
            {
                FailDesc = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = FailIndex.Value.ToString(), ResetInput = Input };
                Station.StationSession.Add(FailDesc);
            }
            string inputValue = Input.Value.ToString();

            T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_ERROR_CODE failCodeObject = t_c_error_code.GetByErrorCode(inputValue, Station.SFCDB);

            if (failCodeObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { inputValue }));
            }
            else
            {
                FailCode.Value = failCodeObject;
                if (Paras[4].VALUE != null)
                {
                    try
                    {
                        MESStationInput next= Station.Inputs.Find(t => t.DisplayName == Paras[4].VALUE);
                        next.Value = failCodeObject.ENGLISH_DESC;
                        Station.NextInput = next;
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 從SN對象加載該SN已經維修的次數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairCountDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            if (sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            //Repair Count Session
            MESStationSession sessionRepairCount = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionRepairCount == null)
            {
                sessionRepairCount = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionRepairCount);
            }
            try
            {
                //LogicObject.SN snObject = (LogicObject.SN)sessionSN.Value;
                string repair_no = "";
                if (sessionSN.Value.GetType() == typeof(SN))
                {
                    SN snObject = (SN)sessionSN.Value;
                    repair_no = snObject.SerialNo;
                }
                else if (sessionSN.Value.GetType() == typeof(Panel))
                {
                    Panel panelObject = (Panel)sessionSN.Value;
                    repair_no = panelObject.PanelNo;
                }
                else
                {
                    repair_no = sessionSN.Value.ToString();
                }

                T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
                sessionRepairCount.Value = t_r_repair_main.GetRepairedCount(repair_no, Station.SFCDB, Station.DBType);
                sessionRepairCount.InputValue = t_r_repair_main.GetRepairedCount(repair_no, Station.SFCDB, Station.DBType).ToString();
                sessionRepairCount.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { Paras[1].SESSION_TYPE, sessionRepairCount.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 維修根據輸入的SN和Location獲取allpart信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AllpartInfoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            if (sessionSN.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }

            MESStationSession sessionLocation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLocation == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "Location" }));
            }
            if (sessionLocation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "Location" }));
            }
            if (sessionLocation.Value.ToString().ToUpper().Trim() == "NA" || sessionLocation.Value.ToString().ToUpper().Trim() == "N/A")
            {
                return;
            }
            try
            {
                System.Data.OleDb.OleDbParameter[] paras = new System.Data.OleDb.OleDbParameter[]
                {
                new System.Data.OleDb.OleDbParameter("MYPSN", sessionSN.Value.ToString()),
                new System.Data.OleDb.OleDbParameter("MYLOCATION", sessionLocation.Value.ToString()),
                new System.Data.OleDb.OleDbParameter("G_TR_SN", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_KP_NO", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_MFR_KP_NO", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_MFR_CODE", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_MFR_NAME", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_DATE_CODE", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_LOT_CODE", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_KP_DESC", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_PROCESS_FLAG", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("G_STATION", System.Data.OleDb.OleDbType.VarChar, 200),
                new System.Data.OleDb.OleDbParameter("RES", System.Data.OleDb.OleDbType.VarChar, 800)
                };
                for (int i = 2; i < paras.Length; i++)
                {
                    paras[i].Direction = ParameterDirection.Output;
                }

                Dictionary<string, object> spReturnDic = Station.APDB.ExecProcedureReturnDic("MES1.GET_KP_MESSAGE", paras);
                if (spReturnDic["RES"].ToString().Equals("OK"))
                {
                    Station.Inputs.Find(input => input.DisplayName == "KP_NO").Value = spReturnDic["G_KP_NO"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "MFR_Name").Value = spReturnDic["G_MFR_NAME"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "MFR_Code").Value = spReturnDic["G_MFR_CODE"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "Date_Code").Value = spReturnDic["G_DATE_CODE"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "Lot_Code").Value = spReturnDic["G_LOT_CODE"].ToString();
                }
                else
                {
                    throw new Exception(spReturnDic["RES"].ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 獲取ActionCode描述
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ActionCodeDescLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sActionCode == null || sActionCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sOutDesc = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sOutDesc == null)
            {
                sOutDesc = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sOutDesc);
            }
            MESStationSession sFailCodeID = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sFailCodeID == null || sFailCodeID.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            try
            {
                T_C_ACTION_CODE T_CAC = new T_C_ACTION_CODE(Station.SFCDB, Station.DBType);
                C_ACTION_CODE objCAC = T_CAC.GetByActionCode(sActionCode.Value.ToString(), Station.SFCDB);
                if (objCAC == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "ActionCode", sActionCode.Value.ToString() }));
                }
                Station.Inputs.Find(input => input.DisplayName == "ActionCodeDesc").Value = objCAC.ENGLISH_DESC;
                Station.AddMessage("MES00000029", new string[] { Paras[0].SESSION_TYPE, sActionCode.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 獲取RootCause描述
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RootCauseDescLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sActionCode == null || sActionCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sReplaceCSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);            
            MESStationSession sTRSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            MESStationSession sRootCause = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sRootCause == null || sRootCause.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            MESStationSession sOutDesc = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sOutDesc == null)
            {
                sOutDesc = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sOutDesc);
            }

            try
            {
                if (sActionCode.Value.ToString() == "A12")
                {
                    //如果ActionCode=A12的情況下必須有換料信息才可以輸入RootCause
                    if ((sReplaceCSN == null || sReplaceCSN.Value == null) && (sTRSN == null || sTRSN.Value == null))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200323082356"));
                    }
                }
                T_C_ERROR_CODE T_CEC = new T_C_ERROR_CODE(Station.SFCDB, Station.DBType);
                C_ERROR_CODE objCEC = T_CEC.GetByErrorCode(sRootCause.Value.ToString(), Station.SFCDB);
                if (objCEC == null)
                {
                    sRootCause.Value = "";
                    sRootCause.InputValue = "";
                    sRootCause.ResetInput = Input;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "RootCause", sRootCause.Value.ToString() }));
                }
                Station.Inputs.Find(input => input.DisplayName == "RootCauseDesc").Value = objCEC.ENGLISH_DESC;
                Station.AddMessage("MES00000029", new string[] { Paras[0].SESSION_TYPE, sRootCause.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 獲取TRSN的Allpart信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRSNDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sTRSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sTRSN == null || sTRSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            try
            {
                string sql = string.Format(@"
                        SELECT A.TR_SN, A.CUST_KP_NO, A.MFR_CODE, B.MFR_NAME, A.DATE_CODE, A.LOT_CODE
                          FROM MES4.R_TR_SN A, MES1.C_MFR_CONFIG B WHERE A.MFR_CODE = B.MFR_CODE AND TR_SN = '{0}'", sTRSN.Value.ToString());
                DataTable dt = Station.APDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { sTRSN.Value.ToString(), "MES4.R_TR_SN/MES1.C_MFR_CONFIG" }));
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Station.Inputs.Find(input => input.DisplayName == "NewMfrCode").Value = dt.Rows[i]["MFR_CODE"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "NewMfrName").Value = dt.Rows[i]["MFR_NAME"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "NewDC").Value = dt.Rows[i]["DATE_CODE"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "NewLC").Value = dt.Rows[i]["LOT_CODE"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 從SN對象加載是否需要傳MDS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMDSFlagLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            #region 獲取SN對象
            MESStationSession sSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sSN == null || sSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN objSN = (SN)sSN.Value;
            #endregion

            #region 新建MDSGet對象
            MESStationSession sMDSFlag = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sMDSFlag == null)
            {
                sMDSFlag = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sMDSFlag);
            }
            #endregion

            try
            {
                bool mdsFlag = true;
                T_R_REPAIR_MAIN T_RRM = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
                R_REPAIR_MAIN RRM = T_RRM.GetSNBySN(objSN.SerialNo, Station.SFCDB);
                if (RRM.FAIL_STATION == "COSMETIC-FAILURE")
                {
                    mdsFlag = false;
                }
                sMDSFlag.Value = mdsFlag.ToString();
                sMDSFlag.InputValue = mdsFlag.ToString();
                sMDSFlag.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { Paras[1].SESSION_TYPE, sMDSFlag.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 從SN對象加載R_SN_KP信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNKPInfoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            #region 獲取SN對象
            MESStationSession sSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sSN == null || sSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN objSN = (SN)sSN.Value;
            #endregion

            #region 新建SNKP對象
            MESStationSession sSNKp = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sSNKp == null)
            {
                sSNKp = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sSNKp);
            }
            #endregion

            try
            {
                DataTable snKpDT = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == objSN.ID && t.VALID_FLAG == 1).Select(t => new
                {
                    t.SN, t.VALUE, t.PARTNO, t.KP_NAME, t.MPN, t.SCANTYPE, t.ITEMSEQ, t.SCANSEQ, t.STATION, t.EXKEY1, t.EXVALUE1, t.EXKEY2, t.EXVALUE2, t.EDIT_EMP, t.EDIT_TIME
                }).OrderBy(t=>t.ITEMSEQ).OrderBy(t=>t.SCANSEQ).ToDataTable();
                if (snKpDT.Rows.Count == 0)
                {
                    //不能循環清除Output,只清除SNKpList中的信息
                    Station.StationSession.Find(s => s.MESDataType == "SNKP" && s.SessionKey == "1").Value = "";
                }
                sSNKp.Value = ConvertToJson.DataTableToJson(snKpDT);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Panel Fail Data From Panel Object
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelFailDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {            
            MESStationSession panelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (panelSession == null || panelSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession failStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            //if (failStationSession == null || panelSession.Value == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            //}
            MESStationSession failDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (failDataSession == null)
            {
                failDataSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(failDataSession);
            }
            Panel panel = (Panel)panelSession.Value;
            string failStation = (failStationSession == null || panelSession.Value == null) ? "" : failStationSession.Value.ToString();
            if (failStation == "")
            {
                var data = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE>((main, code) => main.ID == code.MAIN_ID && main.SN == code.SN)
                .Where((main, code) => main.SN == panel.PanelNo && main.WORKORDERNO == panel.Workorderno && main.CLOSED_FLAG == "0"&&code.REPAIR_FLAG=="0")
                .Select((main, code) => new { main.SN,  main.WORKORDERNO, main.SKUNO, main.FAIL_STATION, code.FAIL_CODE, code.F_LOCATION, SNNumber=code.DESCRIPTION, code.ID })
                .ToList();
                failDataSession.Value = data;
            }
            else
            {
                var data = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE>((main, code) => main.ID == code.MAIN_ID && main.SN == code.SN)
                .Where((main, code) => main.SN == panel.PanelNo && main.WORKORDERNO == panel.Workorderno && main.CLOSED_FLAG == "0" && code.REPAIR_FLAG == "0" && main.FAIL_STATION == failStation)
                .Select((main, code) => new { main.SN, main.WORKORDERNO, main.SKUNO, main.FAIL_STATION, code.FAIL_CODE, code.F_LOCATION, SNNumber = code.DESCRIPTION, code.ID })
                .ToList();
                failDataSession.Value = data;
            }            
        }
        /// <summary>
        /// Get Fail Code Info From Input Fail Code
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FailCodeInfoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {            
            string inputValue = Input.Value.ToString();
            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionFailCode == null)
            {
                sessionFailCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionFailCode);
            }
            else
            {                
                sessionFailCode.Value = null;
            }

            MESStationSession sessionEnglishDesc = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionEnglishDesc == null)
            {
                sessionEnglishDesc = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionEnglishDesc);
            }
            else
            {
                sessionEnglishDesc.Value = null;
            }
            MESStationSession sessionChineseDesc = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionChineseDesc == null)
            {
                sessionChineseDesc = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionChineseDesc);
            }
            else
            {
                sessionChineseDesc.Value = null;
            }
            List<R_Station_Action_Para> cagegoryParas = Paras.FindAll(t => t.SESSION_TYPE.Equals("ErrorCategory"));
            List<string> list = new List<string>();
            T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_ERROR_CODE failCodeObject = null;
            if (cagegoryParas.Count > 0)
            {
                list = cagegoryParas.Select(r => r.VALUE).ToList();
                failCodeObject = t_c_error_code.GetCodeByCategory(Station.SFCDB, inputValue, list);
            }
            else
            {
                failCodeObject = t_c_error_code.GetByErrorCode(inputValue, Station.SFCDB);
            }
            if (failCodeObject == null)
            {
                if (cagegoryParas.Count > 0)
                {
                    string category = "";
                    foreach (string s in list)
                    {
                        category += "," + s;
                    }
                    throw new MESReturnMessage($@"{inputValue} Not Exist Or Error Category Not In [{category.Substring(1)}]");
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { inputValue }));
                }
            }
            else
            {
                sessionFailCode.Value = failCodeObject;
                sessionEnglishDesc.Value = failCodeObject.ENGLISH_DESC.ToString();
                sessionChineseDesc.Value = failCodeObject.CHINESE_DESC.ToString();
            }
        }
        /// <summary>
        ///  Get Action Code Info From Input Action Code
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ActionCodeInfoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string inputValue = Input.Value.ToString();
            MESStationSession sessionActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionActionCode == null)
            {
                sessionActionCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionActionCode);
            }
            else
            {
                sessionActionCode.Value = null;
            }

            MESStationSession sessionEnglishDesc = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionEnglishDesc == null)
            {
                sessionEnglishDesc = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionEnglishDesc);
            }
            else
            {
                sessionEnglishDesc.Value = null;
            }
            MESStationSession sessionChineseDesc = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionChineseDesc == null)
            {
                sessionChineseDesc = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionChineseDesc);
            }
            else
            {
                sessionChineseDesc.Value = null;
            }

            T_C_ACTION_CODE t_c_action_code = new T_C_ACTION_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_ACTION_CODE actionCodeObject = t_c_action_code.GetByActionCode(inputValue, Station.SFCDB);

            if (actionCodeObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { inputValue }));
            }
            else
            {
                sessionActionCode.Value = actionCodeObject;
                sessionEnglishDesc.Value = actionCodeObject.ENGLISH_DESC.ToString();
                sessionChineseDesc.Value = actionCodeObject.CHINESE_DESC.ToString();
            }
        }
        /// <summary>
        ///  Get Action List By Repair Fail Code ID
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairActionListLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionRepairFailCodeID = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionRepairFailCodeID == null || sessionRepairFailCodeID.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }           
            MESStationSession sessionActionList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionActionList == null)
            {
                sessionActionList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(sessionActionList);
            }           
            string repair_fail_code_id = sessionRepairFailCodeID.Value.ToString();
            R_REPAIR_FAILCODE failObj = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(r => r.ID == repair_fail_code_id).ToList().FirstOrDefault();
            List<string> list = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(r => r.MAIN_ID == failObj.MAIN_ID).Select(r => r.ID).ToList();
            sessionActionList.Value = Station.SFCDB.ORM.Queryable<R_REPAIR_ACTION>().Where(r => list.Contains(r.R_FAILCODE_ID))
                .Select(r => new { r.SN, r.ACTION_CODE, r.PROCESS, r.FAIL_LOCATION, r.FAIL_CODE, r.REASON_CODE, r.DESCRIPTION, r.REPAIR_TIME }).ToList();          
        }
    }
}
