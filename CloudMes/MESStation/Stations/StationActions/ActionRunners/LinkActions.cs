using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDBHelper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    class LinkActions
    {
        public static void SNLinkPassAction_Old(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SN = null;
            string ErrMessage = string.Empty;
            string wo = Station.Inputs.Find(s => s.DisplayName == "WO").Value.ToString();
            string sn = Station.Inputs.Find(s => s.DisplayName == "SUB_SN").Value.ToString();
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150601"));
            }
            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SN = (SN)SNLoadPoint.Value;

            T_R_SN TRS = new T_R_SN(Station.SFCDB,DB_TYPE_ENUM.Oracle);
            Row_R_SN RRS = (Row_R_SN)TRS.NewRow();
            T_R_SN_STATION_DETAIL tr_sd = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_SN_STATION_DETAIL rr_sd = (Row_R_SN_STATION_DETAIL)tr_sd.NewRow();

            RRS = TRS.getR_SNbySN(sn, Station.SFCDB);
            RRS.CURRENT_STATION = Station.StationName;
            RRS.NEXT_STATION = TRS.GetNextStation(SN.RouteID,Station.StationName,Station.SFCDB);
            RRS.VALID_FLAG = "0";
            RRS.EDIT_EMP = Station.LoginUser.EMP_NO;
            RRS.EDIT_TIME = DateTime.Now;
            if (RRS.NEXT_STATION.ToUpper() == "JOBFINISH")
            {
                RRS.COMPLETED_FLAG = "1";
                RRS.COMPLETED_TIME= DateTime.Now;
            }
            Station.SFCDB.ExecSQL(RRS.GetUpdateString(DB_TYPE_ENUM.Oracle));
            TRS.RecordPassStationDetail(sn, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
        }

        public static void SNLinkSubSNKPAction_Old(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder wO = null;
            //SN snob = null;
            string ErrMessage = string.Empty;
            string wo = Station.Inputs.Find(s => s.DisplayName == "WO").Value.ToString();
            string sn = Station.Inputs.Find(s => s.DisplayName == "SUB_SN").Value.ToString();
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession WOLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOLoadPoint == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            wO = (WorkOrder)WOLoadPoint.Value;
            //snob.Load(sn,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            SN snob = new SN(sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (snob == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN Point" }));
            }

            T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB,DB_TYPE_ENUM.Oracle);
            T_R_SN_KEYPART_DETAIL T_kd = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_KEYPART> KEYPARTS = tck.GetKeypartListByWOAndStation(Station.SFCDB,wO.WorkorderNo,Station.StationName);
            if (KEYPARTS.Count > 0)
            {
                T_kd.INSN_KEYPART_DETAIL(Station.SFCDB, Station.BU, snob.ID, snob.SerialNo, sn, Station.StationName, KEYPARTS[0].PART_NO, KEYPARTS[0].SEQ_NO, KEYPARTS[0].CATEGORY_NAME, KEYPARTS[0].CATEGORY, Station.LoginUser.EMP_NO);
                Station.AddMessage("MES00000180", new string[] { "SUB_SN", sn  }, StationMessageState.Pass);
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { sn });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void SNLinkMainSNKPAction_Old(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder wO = null;
            //SN Sub_SN = null;
            //SN Min_Sn = null;
            SN Sub_SN = new SN();
            SN Min_Sn = new SN();
            string ErrMessage = string.Empty;
            string wo = Station.Inputs.Find(s => s.DisplayName == "WO").Value.ToString();
            string s_sn = Station.Inputs.Find(s => s.DisplayName == "SUB_SN").Value.ToString();
            string m_sn = Station.Inputs.Find(s => s.DisplayName == "MIN_SN").Value.ToString();
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession WOLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOLoadPoint == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            wO = (WorkOrder)WOLoadPoint.Value;
            Sub_SN.Load(s_sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Min_Sn.Load(m_sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_KEYPART_DETAIL T_kd = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_KEYPART> KEYPARTS = tck.GetKeypartListByWOAndStation(Station.SFCDB, wO.WorkorderNo, Station.StationName);
            List<R_SN_KEYPART_DETAIL> KEYPARTD = T_kd.GetKeypartBySub_Sn(Station.SFCDB, s_sn,Station.StationName);
            if (KEYPARTS.Count >= 2)
            {
                if (KEYPARTS.Count > KEYPARTD.Count)
                {
                    int i = KEYPARTD.Count;
                    T_kd.INSN_KEYPART_DETAIL(Station.SFCDB, Station.BU, Sub_SN.ID, Sub_SN.SerialNo, Min_Sn.SerialNo, Station.StationName, KEYPARTS[i].PART_NO, KEYPARTS[i].SEQ_NO, KEYPARTS[i].CATEGORY_NAME, KEYPARTS[i].CATEGORY, Station.LoginUser.EMP_NO);
                }
                Station.AddMessage("MES00000180", new string[] { "MAIN_SN", m_sn }, StationMessageState.Pass);
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { Min_Sn.SerialNo });
                throw new MESReturnMessage(ErrMessage);
            }
        }


        public static void SNLinkSubSNKPAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> KPList_Temp = new List<Dictionary<string, string>>();
            Dictionary<string, string> DicKP = new Dictionary<string, string>();
            List<C_KEYPART> SubKP = new List<C_KEYPART>();

            //WorkOrder WO = null;
            SN Sn = null;
            string ErrMessage = string.Empty;
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SubSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SubSNSession == null)
            {
                SubSNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SubSNSession);
            }

            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SubKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (KPListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            Sn = (SN)SubSNSession.Value;
            SubKP = (List<C_KEYPART>)SubKPSession.Value;

            DicKP["SN"] = Sn.SerialNo;
            DicKP["SN_ID"] = Sn.ID;
            DicKP["KEYPART_SN"] = Sn.SerialNo;
            DicKP["KP_SN_ID"] = Sn.ID;
            DicKP["PART_NO"] = Sn.SkuNo;
            DicKP["SEQ_NO"] = SubKP[0].SEQ_NO.ToString();
            DicKP["CATEGORY_NAME"] = SubKP[0].CATEGORY_NAME;
            DicKP["CATEGORY"] = SubKP[0].CATEGORY;
            DicKP["KP_TYPE"] = "SUB_SN";

            KPList.Add(DicKP);
            KPListSession.Value = KPList;
            //MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            //if (SubSNSession == null)
            //{
            //    SubSNSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(SubSNSession);
            //}

            //wO = (WorkOrder)WOLoadPoint.Value;
            //snob.Load(sn,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            //SN snob = new SN(sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //if (snob == null)
            //{
            //    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN Point" }));
            //}
            //T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //T_R_SN_KEYPART_DETAIL T_kd = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //List<C_KEYPART> KEYPARTS = tck.GetKeypartListByWOAndStation(Station.SFCDB, wO.WorkorderNo, Station.StationName);
            //if (KEYPARTS.Count > 0)
            //{
            //    T_kd.INSN_KEYPART_DETAIL(Station.SFCDB, Station.BU, snob.ID, snob.SerialNo, sn, Station.StationName, KEYPARTS[0].PART_NO, KEYPARTS[0].SEQ_NO, KEYPARTS[0].CATEGORY_NAME, Station.LoginUser.EMP_NO);
            //    Station.AddMessage("MES00000180", new string[] { "SUB_SN", sn }, StationMessageState.Pass);
            //}
            //else
            //{
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { sn });
            //    throw new MESReturnMessage(ErrMessage);
            //}
        }

        public static void SNLinkMainSNKPAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> KPList_Temp = new List<Dictionary<string, string>>();
            Dictionary<string, string> DicKP = new Dictionary<string, string>();
            List<C_KEYPART> MainKP = null;
            C_KEYPART MainKP_Temp = null;

            //WorkOrder WO = null;
            SN MainSnObj = null;
            SN SubSnObj = null;
            //string SubSn= "";
            string ErrMessage = string.Empty;
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SubSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SubSNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession MainSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (MainSNSession == null)
            {
                MainSNSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainSNSession);
            }

            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (MainKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (KPListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MainSnObj = (SN)MainSNSession.Value;
            SubSnObj = (SN)SubSNSession.Value;
            MainKP = (List<C_KEYPART>)MainKPSession.Value;
            MainKP_Temp = MainKP.Find(c => c.PART_NO == MainSnObj.SkuNo);

            if (MainKP_Temp == null)
            {
                //throw new Exception("MainSN 料號與配置的料號不一致!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150651"));
            }

            KPList = (List<Dictionary<string, string>>)KPListSession.Value;
            //SubSn = KPList.Find(kp => kp["KP_TYPE"].ToString() == "SUB_SN")["SUB_SN"].ToString();
            DicKP["SN"] = SubSnObj.SerialNo;
            DicKP["SN_ID"] = SubSnObj.ID;
            DicKP["KEYPART_SN"] = MainSnObj.SerialNo;
            DicKP["KP_SN_ID"] = MainSnObj.ID;
            DicKP["PART_NO"] = MainSnObj.SkuNo;
            DicKP["SEQ_NO"] = MainKP_Temp.SEQ_NO.ToString();
            DicKP["CATEGORY_NAME"] = MainKP_Temp.CATEGORY_NAME;
            DicKP["CATEGORY"] = MainKP_Temp.CATEGORY;
            DicKP["KP_TYPE"] = "MAIN_SN";

            KPList.Add(DicKP);
            KPListSession.Value = KPList;
        }

        public static void SNLinkPassAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string SubSn = "";
            SN SubSNObj = new SN();
            SN SnObj = new SN();
            WorkOrder WO = new WorkOrder();
            T_R_SN Table_R_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL Table_SnDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN_KEYPART_DETAIL Table_R_Keypart = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, Station.DBType);
            string ErrMessage = string.Empty;
            List<C_KEYPART> SubKPList = new List<C_KEYPART>();
            List<C_KEYPART> MainKPList = new List<C_KEYPART>();
            string StrNextStation = "";
            string Status = "";
            R_SN R_Sn = null;
            T_R_WO_BASE WoTable = null; //add by LLF
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            string result = "";
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150733"));
            }

            MESStationSession SubSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SubSNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SubKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (MainKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (KPListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (NextStationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE,  SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    StatusSession.Value = "PASS";
                }
            }
            Status = StatusSession.Value.ToString();

            MESStationSession ClearFlagGSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (ClearFlagGSession == null)
            {
                ClearFlagGSession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(ClearFlagGSession);
            }
            ClearFlagGSession.Value = "false";

            StrNextStation = SnObj.StringListToString((List<string>)NextStationSession.Value);
            //StrNextStation = NextStationSession.Value.ToString();
            SubKPList =(List<C_KEYPART>)SubKPSession.Value;
            MainKPList = (List<C_KEYPART>)MainKPSession.Value;
            KPList = (List<Dictionary<string, string>>)KPListSession.Value;
            SubSNObj = (SN)SubSNSession.Value;
            WO = (WorkOrder)WOSession.Value;
            SubSn = SubSNObj.SerialNo;
            //R_Sn = Table_R_SN.GetById(SubSNObj.ID, Station.SFCDB);
            if (SubKPList.Count + MainKPList.Count == KPList.Count)
            {
                Table_R_SN.InsertLinkSN(SubSn, WO.WorkorderNo, WO.SkuNO, WO.RouteID, WO.KP_LIST_ID, Station.StationName, StrNextStation, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB, SubSNObj.Plant);
                Table_R_SN.UpdateSNKeyparStatus(SubSNObj.ID, Station.LoginUser.EMP_NO,"0", Station.SFCDB);
                R_Sn = Table_R_SN.GetDetailBySN(SubSNObj.SerialNo, Station.SFCDB);

                Table_R_SN.UpdateLinkKPID(SubSNObj.ID, SubSNObj.SerialNo, R_Sn.ID, Station.SFCDB);
                //更新Main KP SN
                foreach (Dictionary<string,string> DicMainKP in KPList)
                {
                    int SeqNo = Convert.ToInt16(DicMainKP["SEQ_NO"]);
                    if (DicMainKP["KP_TYPE"] == "MAIN_SN")
                    {
                        Table_R_SN.UpdateSNKeyparStatus(DicMainKP["KP_SN_ID"], Station.LoginUser.EMP_NO, "1", Station.SFCDB);
                    }
                    //写KP
                    Table_R_Keypart.INSN_KEYPART_DETAIL(Station.SFCDB, Station.BU, R_Sn.ID, DicMainKP["SN"], DicMainKP["KEYPART_SN"], Station.StationName, DicMainKP["PART_NO"], SeqNo, DicMainKP["CATEGORY_NAME"], DicMainKP["CATEGORY"], Station.LoginUser.EMP_NO);
                }
                
                
                //寫過站記錄
                result = Table_R_SN.LinkPassStationDetail(R_Sn, WO.WorkorderNo, WO.SkuNO, WO.RouteID, Station.Line, Station.StationName, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                if (Convert.ToInt32(result) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "STATION DETAIL" }));
                }
                //add by LLF
                WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
                result = WoTable.AddCountToWo(WO.WorkorderNo, 1, Station.SFCDB); // 更新 R_WO_BASE 中的數據
                if (Convert.ToInt32(result) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "WO QTY" }));
                }
                //寫良率，UPH
                result = Table_R_SN.RecordUPH(WO.WorkorderNo, 1, SubSn, Status, Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                if (Convert.ToInt32(result) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "UPH" }));
                }
                result = Table_R_SN.RecordYieldRate(WO.WorkorderNo, 1, SubSn, Status, Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                if (Convert.ToInt32(result) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "YIELD" }));
                }

                SN SN = new SN();
                SN.Load(R_Sn.SN, Station.SFCDB,DB_TYPE_ENUM.Oracle);
                SN.LockCheck(Station.SFCDB);
                KPListSession.Value = null;
                ClearFlagGSession.Value = "true";
                Station.AddMessage("MES00000195", new string[] { SubSn, StrNextStation }, StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 将 SN 与 Keypart 的对应关系插入到 R_SN_KEYPART_DETAIL 表中
        /// 如果Link满了之后就将 SN 过站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnLinkKeypartAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN Sn = (SN)SnSession.Value;

            MESStationSession KeypartSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (KeypartSnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            string KeypartSn = KeypartSnSession.Value.ToString();

            MESStationSession CurrentKeypartSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (CurrentKeypartSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[20].SESSION_TYPE }));
            }
            string CurrentKeypart = CurrentKeypartSession.Value.ToString();

            MESStationSession KeypartListSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (CurrentKeypartSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            List<KEYPARTLIST> KeypartList = (List<KEYPARTLIST>)KeypartListSession.Value;

            T_R_SN_KEYPART_DETAIL TRSKD = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN TRS = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            List<R_SN_KEYPART_DETAIL> ScanedKeypart = TRSKD.GetKeypartsBySN(Sn.SerialNo, Station.StationName, Station.SFCDB);
            double? SeqNo = 10;
            if (ScanedKeypart.Count > 0)
            {
                SeqNo = ScanedKeypart.Last().SEQ_NO + 10;
            }


            KEYPARTLIST Keypart = KeypartList.Find(t => t.CATEGORY_NAME.Equals(CurrentKeypart));

            R_SN_KEYPART_DETAIL KeypartDetail = new R_SN_KEYPART_DETAIL();
            KeypartDetail.R_SN_ID = Sn.ID;
            KeypartDetail.SN = Sn.SerialNo;
            KeypartDetail.KEYPART_SN = KeypartSn;
            KeypartDetail.STATION_NAME = Station.StationName;
            KeypartDetail.PART_NO = Keypart.PART_NO;
            KeypartDetail.SEQ_NO = SeqNo;
            KeypartDetail.CATEGORY_NAME = CurrentKeypart;
            KeypartDetail.CATEGORY = CurrentKeypart;
            KeypartDetail.VALID = "1";
            KeypartDetail.CREATE_EMP = Station.LoginUser.EMP_NO;
            KeypartDetail.EDIT_EMP = Station.LoginUser.EMP_NO;

            int result = (new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle)).InsertKeypartDetail( KeypartDetail, Station.BU, Station.SFCDB);
            if (result > 0)
            {
                ScanedKeypart = TRSKD.GetKeypartsBySN(Sn.SerialNo, Station.StationName, Station.SFCDB);
                List<string> Categorys = ScanedKeypart.Select(t => t.CATEGORY_NAME).Distinct().ToList();
                if (Categorys.Count.Equals(KeypartList.Count) && ScanedKeypart.FindAll(t=>t.CATEGORY_NAME==CurrentKeypart).Count==Int32.Parse(KeypartList.Find(t=>t.CATEGORY_NAME==CurrentKeypart).QTY))
                {
                    TRS.PassStation(Sn.SerialNo, Station.Line, Station.StationName, Station.StationName, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
                    //TRS.RecordPassStationDetail(Sn.SerialNo, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
                    TRS.RecordUPH(Sn.WorkorderNo, 1, Sn.SerialNo, "PASS", Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                    TRS.RecordYieldRate(Sn.WorkorderNo, 1, Sn.SerialNo, "PASS", Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                    //Station.NextInput = Station.FindInputByName("MainSN");
                }
            }

        }

        /// <summary>
        /// HWD 退LINK
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWDCancelLinkAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObj = (SN)sessionSN.Value;
            T_R_SN_KEYPART_DETAIL t_r_sn_keypart_detail = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, Station.DBType);
            List<R_SN_KEYPART_DETAIL> listKPDetail = t_r_sn_keypart_detail.GetKeypartsBySN(snObj.SerialNo, Station.StationName, Station.SFCDB);

            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            List<R_SN_KP> listKp = t_r_sn_kp.GetKPRecordBySnStation(snObj.SerialNo, Station.StationName, Station.SFCDB);
            
            R_SN kpSN = null;
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            int result = 0;
            DateTime sysdate = Station.GetDBDateTime();
            string user = Station.LoginUser.EMP_NO;
            foreach (var d in listKPDetail)
            {
                if (d.KEYPART_SN == snObj.SerialNo)
                {
                    kpSN = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == d.KEYPART_SN && r.SHIPPED_FLAG == "1" && r.VALID_FLAG == "0" && r.NEXT_STATION == "JOBFINISH")
                        .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                }
                else
                {
                    kpSN = t_r_sn.LoadSN(d.KEYPART_SN, Station.SFCDB);
                }
                if (kpSN != null)
                {
                    kpSN.SHIPPED_FLAG = "0";
                    kpSN.VALID_FLAG = "1";
                    kpSN.EDIT_TIME = sysdate;
                    kpSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                    result = t_r_sn.Update(kpSN, Station.SFCDB);
                    if (result == 0)
                    {
                        throw new Exception($@"{d.KEYPART_SN},Update R_SN Fail!");
                    }
                }
                d.VALID = "0";                
                result = t_r_sn_keypart_detail.Update(Station.SFCDB,d);
                if (result == 0)
                {
                    throw new Exception($@"{d.KEYPART_SN},Update R_SN_KEYPART Fail!");
                }
            }

            foreach (var k in listKp)
            {
                kpSN = t_r_sn.LoadSN(k.VALUE, Station.SFCDB);
                if (kpSN != null)
                {
                    kpSN.SHIPPED_FLAG = "0";                   
                    kpSN.EDIT_TIME = sysdate;
                    kpSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                    result = t_r_sn.Update(kpSN, Station.SFCDB);
                    if (result == 0)
                    {
                        throw new Exception($@"{k.VALUE},Update R_SN Fail!");
                    }
                }
                k.VALID_FLAG = 0;
                result = t_r_sn_kp.UpdateObj(Station.SFCDB, k);
                if (result == 0)
                {
                    throw new Exception($@"{k.VALUE},Update R_SN_KP Fail!");
                }
            }

            R_SN r_sn = t_r_sn.LoadSN(snObj.SerialNo, Station.SFCDB);
            string re = t_r_sn.RecordPassStationDetail(r_sn, Station.Line, "CancelLink", "CancelLink", Station.BU,Station.SFCDB);
            if (Convert.ToInt32(re) == 0)
            {
                throw new Exception($@"{snObj.SerialNo},Insert R_SN_STATION_DETAIL Fail!");
            }

            result = Station.SFCDB.ORM.Updateable<R_SN>().UpdateColumns(r => new R_SN { VALID_FLAG = "0", EDIT_TIME = sysdate, EDIT_EMP = user })
                .Where(r => r.ID == snObj.ID).ExecuteCommand();
            if (result == 0)
            {
                throw new Exception($@"{snObj.SerialNo},Update R_SN Fail!");
            }

            result = Station.SFCDB.ORM.Updateable<R_WO_BASE>().UpdateColumns(r => new R_WO_BASE { INPUT_QTY = r.INPUT_QTY - 1 })
                .Where(r => r.WORKORDERNO == snObj.WorkorderNo).ExecuteCommand();
            if (result == 0)
            {
                throw new Exception($@"{snObj.WorkorderNo},Update R_WO_BASE Fail!");
            }

        }
    }
}
