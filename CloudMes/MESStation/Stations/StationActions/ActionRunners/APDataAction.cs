using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESStation.LogicObject;

using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;
using MESStation.Stations.StationActions.DataLoaders;
using System.Data.OleDb;
using MESPubLab.MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class APDataAction
    {
        /// <summary>
        /// HWD Allpart鋼網計數，add by LLF 2018-01-29
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void APStencilUpdateCountAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {


            OleExec apdb = Station.APDB;
            string Psn = Input.Value.ToString();
            string Line = Station.Line;
            string StationName = Station.StationName;
         
            OleDbParameter[] StencilSP = new OleDbParameter[4];
            StencilSP[0] = new OleDbParameter("IN_P_SN", Psn);
            StencilSP[1] = new OleDbParameter("IN_STATION", StationName);
            StencilSP[2] = new OleDbParameter("IN_LINE", Line);
            StencilSP[3] = new OleDbParameter();
            StencilSP[3].Size = 1000;
            StencilSP[3].ParameterName = "RES";
            StencilSP[3].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.CHECK_STENCIL_COUNT_UPDATE", StencilSP);
            if (result == "OK")
            {
                //apdbPool.Return(apdb);
                Station.AddMessage("MES00000062", new string[] { Psn }, StationMessageState.Pass);
            }
            else
            {
                //apdbPool.Return(apdb);
                throw new Exception(result);
            }            
        }

        public static void APPanelSNReplaceAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string PanelSN;
            SN SNObj = new SN();

            string StrSN = "";
            R_PANEL_SN Psn = null;

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            PanelSN = PanelSession.InputValue.ToString();
            Psn = SNObj.GetPanelVirtualSN(PanelSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            StrSN = SNSession.Value.ToString();



            OleExec apdb = Station.APDB;
            //Psn = PanelSession.InputValue.ToString();

            OleDbParameter[] StencilSP = new OleDbParameter[4];
            StencilSP[0] = new OleDbParameter("G_PANEL", "");
            StencilSP[1] = new OleDbParameter("G_PSN", StrSN);
            StencilSP[2] = new OleDbParameter();
            StencilSP[2].Size = 1000;
            StencilSP[2].ParameterName = "RES";
            StencilSP[2].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.Z_PANEL_REPLACE_SP", StencilSP);
            if (result == "OK")
            {
                //apdbPool.Return(apdb);
                Station.AddMessage("MES00000062", new string[] { "" }, StationMessageState.Pass);
            }
            else
            {
                //apdbPool.Return(apdb);
                throw new Exception(result);
            }
        }

        /// HWD PTH Allpart扣料，add by LLF 2018-02-19
        /// </summary>
        public static void APAssignMaterialPTHAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
//            OleExecPool apdbPool = null;
            OleExec apdb = null;

            try
            {

                apdb = Station.APDB;
                string Line = Station.Line;
                string StationName = string.Empty;
                string ErrMessage = string.Empty;
                string StrWO = string.Empty;
                string StationNum = string.Empty;
                string TRCode = string.Empty;
                string SN = string.Empty;

                var pr = Paras.Find(t => t.SESSION_TYPE == "CLEARFLAG");
                MESStationSession CLEARFLAG_Session = Station.StationSession.Find(t => t.MESDataType == "CLEARFLAG" && t.SessionKey == "1");
                if (CLEARFLAG_Session != null && pr != null)
                {
                    if (CLEARFLAG_Session.Value.ToString() != pr.VALUE)
                    {
                        return;
                    }
                }


                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                SN = SNSession.Value.ToString();

                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (WOSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                StrWO = WOSession.Value.ToString();

                MESStationSession StationNumSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (StationNumSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                StationNum = StationNumSession.Value.ToString();
                var pStation = Paras.Find(t => t.SESSION_TYPE == "PTHSTATION");

                if (pStation == null)
                {
                    StationName = Station.Line + Station.StationName + StationNum;
                }
                else
                {
                    StationName = Station.Line + pStation.VALUE + StationNum;
                }

                MESStationSession PTHTRCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (PTHTRCodeSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }

                TRCode = PTHTRCodeSession.Value.ToString();

                OleDbParameter[] AssignMaterialPTHSP = new OleDbParameter[6]; 
                AssignMaterialPTHSP[0] = new OleDbParameter("g_type", "0010");
                AssignMaterialPTHSP[1] = new OleDbParameter("g_wo", StrWO);
                AssignMaterialPTHSP[2] = new OleDbParameter("g_station", StationName);
                AssignMaterialPTHSP[3] = new OleDbParameter("g_tr_code", TRCode);
                AssignMaterialPTHSP[4] = new OleDbParameter("g_psn", SN);
                AssignMaterialPTHSP[5] = new OleDbParameter();
                AssignMaterialPTHSP[5].Size = 1000;
                AssignMaterialPTHSP[5].ParameterName = "RES";
                AssignMaterialPTHSP[5].Direction = System.Data.ParameterDirection.Output;
                string result = apdb.ExecProcedureNoReturn("MES1.assign_material_pth", AssignMaterialPTHSP);
                if (result.IndexOf("OK") >= 0)
                {
                    OleDbParameter[] AssignMaterialPTH0020 = new OleDbParameter[6];
                    AssignMaterialPTH0020[0] = new OleDbParameter("g_type", "0020");
                    AssignMaterialPTH0020[1] = new OleDbParameter("g_wo", StrWO);
                    AssignMaterialPTH0020[2] = new OleDbParameter("g_station", StationName);
                    AssignMaterialPTH0020[3] = new OleDbParameter("g_tr_code", TRCode);
                    AssignMaterialPTH0020[4] = new OleDbParameter("g_psn", SN);
                    AssignMaterialPTH0020[5] = new OleDbParameter();
                    AssignMaterialPTH0020[5].Size = 1000;
                    AssignMaterialPTH0020[5].ParameterName = "RES";
                    AssignMaterialPTH0020[5].Direction = System.Data.ParameterDirection.Output;
                    result = apdb.ExecProcedureNoReturn("MES1.assign_material_pth", AssignMaterialPTH0020);
                    //apdbPool.Return(apdb);
                    if (result.IndexOf("OK") == -1)
                    {
                        throw new Exception(result);
                    }
                }
                else
                {
                    //apdbPool.Return(apdb);
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                //if (apdb != null)
                //{
                //    //apdbPool.Return(apdb);
                //}
                throw new Exception(ex.Message.ToString());
            }

        }
        /// HWD PTH Allpart扣料，add by LLF 2018-02-19

        /// <summary>
        /// 更新TR_SN數據加載，查詢R_TR_SN,R_TR_SN_WIP的數據保存到Dictionary<string_Datarow>中,key為表名 "R_TR_SN","R_TR_SN_WIP"
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRSNDataSessionUpdateAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Dictionary<string, DataRow> APInfo = new Dictionary<string, DataRow>();
            string strTRSN = "";
            string ErrMessage = "";
            OleExec apdb = null;
            int LinkQty = 0;
            int TrSNExtQty = 0;
            if (Paras.Count < 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            

            if (TRSN_Session == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            strTRSN = TRSN_Session.InputValue.ToString();

            MESStationSession TRSNExtQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TRSNExtQty_Session == null)
            {

                TRSNExtQty_Session = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNExtQty_Session);
                TRSNExtQty_Session.Value = 0;
            }

            MESStationSession TRSNPcbSku_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TRSNPcbSku_Session == null)
            {

                TRSNPcbSku_Session = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNPcbSku_Session);
                TRSNPcbSku_Session.Value = "";
            }

            MESStationSession LinkQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (LinkQty_Session != null)
            {
                int.TryParse(LinkQty_Session.Value.ToString(), out LinkQty);
            }

            //獲取ALLPART數據
            AP_DLL APDLL = new AP_DLL();
            try
            {
                apdb = Station.APDB;

                List<DataRow> TRSNWIPlist = APDLL.R_TR_SN_WIP_GetBYTR_SN(strTRSN, apdb);
                if (TRSNWIPlist.Count > 0)
                {
                    TRSNExtQty_Session.Value = TRSNWIPlist[0]["EXT_QTY"];
                    TRSNPcbSku_Session.Value = TRSNWIPlist[0]["KP_NO"];
                }
                else
                {
                    TRSNExtQty_Session.Value = 0;
                }

                //Station.DBS["APDB"].Return(apdb);

                int.TryParse(TRSNExtQty_Session.Value.ToString(), out TrSNExtQty);
                if (TrSNExtQty < LinkQty)
                {
                    MESStationInput StationInput = Station.Inputs.Find(t => t.DisplayName == "TR_SN");
                    StationInput.Enable = true;
                    Station.NextInput = StationInput;
                }
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    //Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }
        }

        /// <summary>
        /// ALLPART补资料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void APInsertDataAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.APDB;
            string Station_Name = Station.StationName;
            string ErrMessage = String.Empty;
            SN SN = null;
            WorkOrder StrWO = null;
            R_AP_TEMP rAp;
            try
            {
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                SN = (SN)SNSession.Value;

                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (WOSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                StrWO =(WorkOrder)WOSession.Value;

                MESStationSession StationNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (StationNOSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                rAp = (R_AP_TEMP)StationNOSession.Value;

                OleDbParameter[] para = new OleDbParameter[]
                {
                new OleDbParameter(":g_psn",OleDbType.VarChar,300),
                new OleDbParameter(":g_wo",OleDbType.VarChar,300),
                new OleDbParameter(":g_station",OleDbType.VarChar,300),
                new OleDbParameter(":g_event",OleDbType.VarChar,300),
                new OleDbParameter(":res",OleDbType.VarChar,500)
                };
                para[0].Value = SN.SerialNo;
                para[1].Value = StrWO.WorkorderNo;
                para[2].Value = rAp.DATA2;
                para[3].Value = Station_Name;
                para[4].Direction = ParameterDirection.Output;
                DB.ExecProcedureNoReturn("MES1.cmc_insertdata_sp", para);
                string sct = para[4].Value.ToString();
                if (sct.Substring(0, 2).ToUpper() != "OK")
                {
                    if (StrWO.WorkorderNo.Substring(0, 6) != "002315" || StrWO.WorkorderNo.Substring(0, 6) != "002258" || StrWO.WorkorderNo.Substring(0, 6) != "002260" || StrWO.WorkorderNo.Substring(0, 6) != "000099")
                    {
                        throw new Exception("Allpart  Err:" + sct);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Allpart  Err:" + ex.Message);
            }
        }

        /// <summary>
        /// 檢查處理虛擬條碼
        /// 暫時不用
        /// add by hgb 2019.05.29
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckAndInsertVirtualSn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            try
            {
                if (Paras.Count != 3)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                string SN = SNSession.Value.ToString().Trim().ToUpper();


                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (WOSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                string WO = WOSession.Value.ToString().Trim().ToUpper();

                if (WO.Substring(0, 6) == "009999" || WO.Substring(0, 6) == "002163" || WO.Substring(0, 6) == "002164" || WO.Substring(0, 6) == "002159" || WO.Substring(0, 6) == "002159")
                {
                    goto over_allparts_check;//不檢查ALLPART
                }

                MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (SKUSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                string SKUNO = SKUSession.Value.ToString().Trim().ToUpper();

                AP_DLL ApDll = new AP_DLL();
                if (ApDll.CheckIsNeedCheckVirtualSn(SKUNO, Station.APDB))//是否配置需要檢查虛擬條碼
                {
                    if (WO.Substring(0, 2) == "99")//虛擬工單
                    {
                        ApDll.CheckVirtualSnIsFihishScan(SN, SKUNO, Station.StationName, Station.IP, WO, Station.APDB);//檢查上一輪是否有掃完                       
                    }
                    else //正常工單
                    {
                        ApDll.CheckVirtualSnIsScan(SN, SKUNO, Station.StationName, Station.IP, WO, Station.SFCDB, Station.APDB);//檢查是否已掃描虛擬條碼
                    }


                    string result = ApDll.CHECK_PSN_MATERIAL_AOI_NEW(SN, Station.StationName, Station.Line, Station.IP, WO, Station.APDB);

                    if (result.IndexOf("OK") < 0)
                    {
                        throw new Exception(result);
                    }

                }

            over_allparts_check:;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }

        /// <summary>
        /// PanelSN重工更新Allpart表mes4.r_sn_link的工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void APPanelSNReworkUpdateRLinkSnAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string sql = string.Empty;
            string ErrMessage = "";
            string PanelSN;
            string Wo;
            int result = 0;
            OleExec APDB = null;

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            PanelSN = PanelSession.InputValue.ToString();

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Wo = WOSession.Value.ToString();
            //StrWO = (WorkOrder)WOSession.Value;

            try
            {
                APDB = Station.APDB;
                sql = $@"UPDATE MES4.R_SN_LINK R SET R.WO='{Wo}' WHERE R.PANEL_NO='{PanelSN}'";
                result = APDB.ExecSqlNoReturn(sql, null);
                //if (result <= 0)
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { " MES4.R_SN_LINK R  WHERE R.PANEL_NO= " + PanelSN, "UPDATE" }));
                //}
            }
            catch (MESReturnMessage ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// SN重工更新Allpart表mes4.r_sn_link的工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void APSNReworkUpdateRLinkSnAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string sql = string.Empty;
            string ErrMessage = "";
            string Sn;
            string Wo;
            int result = 0;
            OleExec APDB = null;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Sn = SNSession.InputValue.ToString();

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Wo = WOSession.Value.ToString();
            //if (WOSession is WorkOrder)
            //{
            //    Wo = ((WorkOrder)WOSession.Value).WorkorderNo;
            //}
            //StrWO = (WorkOrder)WOSession.Value;

            try
            {
                APDB = Station.APDB;
                sql = $@"UPDATE MES4.R_SN_LINK R SET R.WO='{Wo}' WHERE R.P_SN='{Sn}'";
                result = APDB.ExecSqlNoReturn(sql, null);
                //if (result <= 0)
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { " MES4.R_SN_LINK R  WHERE R.P_SN= " + Sn, "UPDATE" }));
                //}
            }
            catch (MESReturnMessage ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Tool 載具掃描記錄 MES4.R_FIXTURE_DETAIL
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void APRFixtureDetailAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string sql = string.Empty;
            string ErrMessage = "";
            string Sn;
            //string Wo;
            //int result = 0;
            OleExec APDB = null;

            try
            {
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                Sn = SNSession.InputValue.ToString();
                string line = Station.Line;
                string empno = Station.LoginUser.EMP_NO;
                string stationname = Station.StationName;

                APDB = Station.APDB;
                AP_DLL ApDll = new AP_DLL();
                ApDll.AP_R_FIXTURE_DETAIL_EVENT(Sn, line, stationname, empno, APDB);
            }
            catch (MESReturnMessage ex)
            {
                throw ex;
            }

        }

        public static void SentDataToHWAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn_new = string.Empty;
            string sn_old = string.Empty;

            if (Paras.Count != 2)
            {               
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession oldSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (oldSNSession == null || oldSNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession newSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (newSNSession == null || newSNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }            

            var OldObj = oldSNSession.Value;
            if (OldObj is string)
            {
                sn_old = oldSNSession.Value.ToString();                
            }
            else
            {               
                sn_old = ((SN)oldSNSession.Value).SerialNo;
            }

            var NewObj = newSNSession.Value;
            if (NewObj is string)
            {
                sn_new = newSNSession.Value.ToString();
            }
            else
            {
                sn_new = ((SN)newSNSession.Value).SerialNo;
            }
            
            DataTable dt = null;
            string result = "";
            int res = 0;
            bool bResult;
            //主板過BIP
            string sql = $@"select * from mes4.r_mes_ift01_seq where lot_id = '{sn_old}'";
            if (Station.APDB.ExecuteDataTable(sql, CommandType.Text, null).Rows.Count > 0)
            {
                //sql = $@"update  mes4.r_mes_ift01_seq set lot_id = '{sn_new}', EDIT_FLAG = '0' where lot_id = '{sn_old}'";
                sql = $@"insert into  mes4.r_mes_ift01_seq(lot_id,edit_time,edit_flag) values('{sn_new}',sysdate,'0')";
                result = Station.APDB.ExecSQL(sql);
                bResult = int.TryParse(result, out res);
                if (!bResult)
                {
                    throw new Exception(result);
                }
            }
            else
            {
                //是否是副板，副板過LINK再綁
                sql = $@"select * from c_keypart z where z.station_name = 'LINK'   and z.seq_no > 10
                            and exists (select 1 from r_sn b where z.skuno = b.skuno and  b.sn = '{sn_old}' and b.valid_flag = '1')";
                if (!(Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null).Rows.Count > 0))
                {
                    sql = $@"select * from r_sn_station_detail where sn='{sn_new}' and station_name = 'BIP'";
                    if (Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null).Rows.Count > 0)
                    {
                        sql = $@"insert into mes4.r_mes_ift01_seq(LOT_ID,EDIT_TIME,EDIT_FLAG) values('{sn_new}',sysdate,'0')";
                        result = Station.APDB.ExecSQL(sql);
                        bResult = int.TryParse(result, out res);
                        if (!bResult)
                        {
                            throw new Exception(result);
                        }
                    }
                }
            }

            //過PTH
            sql = $@"select * from mes4.r_mes_ift01_pthseq where lot_id = '{sn_old}'";
            if (Station.APDB.ExecuteDataTable(sql, CommandType.Text, null).Rows.Count > 0)
            {
                //sql = $@"update  mes4.r_mes_ift01_pthseq set lot_id = '{sn_new}', EDIT_FLAG = '0' where lot_id = '{sn_old}'";
                sql = $@"insert into mes4.r_mes_ift01_pthseq(LOT_ID,EDIT_TIME,EDIT_FLAG) values('{sn_new}',sysdate,'0')";
                result = Station.APDB.ExecSQL(sql);
                bResult = int.TryParse(result, out res);
                if (!bResult)
                {
                    throw new Exception(result);
                }
            }
            else
            {
                sql = $@"select * from r_sn_station_detail where sn='{sn_new}' and station_name = 'LINK'";
                if (Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null).Rows.Count > 0)
                {
                    sql = $@"insert into mes4.r_mes_ift01_pthseq(LOT_ID,EDIT_TIME,EDIT_FLAG) values('{sn_new}',sysdate,'0')";
                    result = Station.APDB.ExecSQL(sql);
                    bResult = int.TryParse(result, out res);
                    if (!bResult)
                    {
                        throw new Exception(result);
                    }
                }
            }

            //1. LINK or BIP scan kp 換SN
            sql = $@"select * from mes4.r_mes_ift01_subseq where lot_id = '{sn_old}'";
            if (Station.APDB.ExecuteDataTable(sql, CommandType.Text, null).Rows.Count > 0)
            {
                //sql = $@"update  mes4.r_mes_ift01_subseq set lot_id = '{sn_new}', EDIT_FLAG = '0' where lot_id = '{sn_old}'";
                sql = $@"insert into mes4.r_mes_ift01_subseq(lot_id, sub_lot_id, edit_time, edit_flag) 
                        select '{sn_new}' as lot_id,sub_lot_id,sysdate as edit_time,'0' as edit_flag from mes4.r_mes_ift01_subseq where lot_id='{sn_old}'";
                result = Station.APDB.ExecSQL(sql);
                bResult = int.TryParse(result, out res);
                if (!bResult)
                {
                    throw new Exception(result);
                }
            }
            else
            {
                sql = $@"SELECT A.SN, A.KEYPART_SN
                          FROM R_SN_KEYPART_DETAIL A
                         WHERE A.STATION_NAME = 'LINK'
                           AND A.SEQ_NO > 10
                           AND A.KEYPART_SN LIKE 'DW%'
                           AND A.SN = '{sn_new}'
                        UNION
                        SELECT A.SN, A.VALUE KEYPART_SN
                          FROM R_SN_KP A
                         WHERE STATION = 'BIP'
                           AND EXISTS (SELECT 1 FROM R_SN WHERE SN = A.VALUE)
                           AND LENGTH(A.SN) <= 25
                           AND A.SN = '{sn_new}'";
                dt = Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                foreach (DataRow r in dt.Rows)
                {
                    sql = $@"insert into mes4.r_mes_ift01_subseq(lot_id, sub_lot_id, edit_time, edit_flag) values('{r["SN"].ToString()}','{r["KEYPART_SN"].ToString()}',sysdate,'0')";
                    result = Station.APDB.ExecSQL(sql);
                    bResult = int.TryParse(result, out res);
                    if (!bResult)
                    {
                        throw new Exception(result);
                    }
                }
            }

            //2. LINK or BIP scan kp 換KEKPART
            sql = $@"select * from mes4.r_mes_ift01_subseq where sub_lot_id = '{sn_old}'";
            if (Station.APDB.ExecuteDataTable(sql, CommandType.Text, null).Rows.Count > 0)
            {
                //sql = $@"update  mes4.r_mes_ift01_subseq set sub_lot_id = '{sn_new}', EDIT_FLAG = '0' where sub_lot_id = '{sn_old}'";
                sql = $@"insert into mes4.r_mes_ift01_subseq(lot_id, sub_lot_id, edit_time, edit_flag) 
                        select '{sn_new}' as lot_id,sub_lot_id,sysdate as edit_time,'0' as edit_flag from mes4.r_mes_ift01_subseq where lot_id='{sn_old}'";
                result = Station.APDB.ExecSQL(sql);
                bResult = int.TryParse(result, out res);
                if (!bResult)
                {
                    throw new Exception(result);
                }
            }
            else
            {
                sql = $@"SELECT A.SN, A.KEYPART_SN
                          FROM R_SN_KEYPART_DETAIL A
                         WHERE A.STATION_NAME = 'LINK'
                           AND A.SEQ_NO > 10
                           AND A.KEYPART_SN LIKE 'DW%'
                           AND A.KEYPART_SN = '{sn_new}'
                        UNION
                        SELECT A.SN, A.VALUE KEYPART_SN
                          FROM R_SN_KP A
                         WHERE STATION = 'BIP'
                           AND EXISTS (SELECT 1 FROM R_SN WHERE SN = A.VALUE)
                           AND LENGTH(A.SN) <= 25
                           AND A.VALUE = '{sn_new}'";
                dt = Station.SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                foreach (DataRow r in dt.Rows)
                {
                    sql = $@"insert into mes4.r_mes_ift01_subseq(lot_id, sub_lot_id, edit_time, edit_flag) values('{r["SN"].ToString()}','{r["KEYPART_SN"].ToString()}',sysdate,'0')";
                    result = Station.APDB.ExecSQL(sql);
                    bResult = int.TryParse(result, out res);
                    if (!bResult)
                    {
                        throw new Exception(result);
                    }
                }
            }
        }
    }
}
