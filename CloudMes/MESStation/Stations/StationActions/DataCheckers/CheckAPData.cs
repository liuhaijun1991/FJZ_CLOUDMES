using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDataObject;
using MESDBHelper;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.Stations.StationActions.DataLoaders;
using System.Data.OleDb;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckAPData
    {
        /// <summary>
        /// SMTLoadingTRSN狀態檢查,
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTTRSNStateDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //return;
            int EXTQTY = 0;//TRSN剩餘數量
            string TRSN_WORKFLAG = "0";//上線標誌
            string TRSNWIP_WORKFLAG = "0";//上線標誌
            string TRSN_LOCATION_FLAG = "";//上線標誌2
            string WIPSKU = "";//r_tr_sn_wip 表中的料號
            List<string> LINKSKU = new List<string>();//r_pcba_link表中的料號
            int LINKQTY = 0;//連板數量          
            Dictionary<string, List<DataRow>> APInfo = new Dictionary<string, List<DataRow>>();
            Dictionary<string, DataRow> TRInfo = new Dictionary<string, DataRow>();
            string strTRSN = "";
            string StrTrSNExtQTY = "";  //add by LLF 2018-03
            int TrSN_EXTQTY = 0;//TRSN剩餘數量,add by LLF 2018-03

            if (Paras.Count != 5)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (TRSN_Session.Value != null)
                {
                    //重新加載                  
                    APInfoDataloader.TRSNDataloader(Station, TRSN_Session.ResetInput, new List<R_Station_Action_Para>() { Paras[0] });
                    TRInfo = (Dictionary<string, DataRow>)TRSN_Session.Value;
                    if (TRInfo.Keys.Contains("R_TR_SN"))
                    {
                        if (TRInfo["R_TR_SN"] == null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                        }
                        else
                        {
                            strTRSN = TRInfo["R_TR_SN"]["TR_SN"].ToString();
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                    }
                    if (TRInfo.Keys.Contains("R_TR_SN_WIP"))
                    {
                        if (TRInfo["R_TR_SN_WIP"] == null)
                        {
                            // throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY +" T_R_SN_WIP"}));
                            //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN_WIP" }));
                            //Add by LLF 2018-03
                            StrTrSNExtQTY = TRInfo["R_TR_SN"]["EXT_QTY"].ToString().Trim();
                            StrTrSNExtQTY = (StrTrSNExtQTY == "") ? "0" : StrTrSNExtQTY;
                            TrSN_EXTQTY = Convert.ToInt32(StrTrSNExtQTY);
                            if (TrSN_EXTQTY <= 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000073", new string[] { strTRSN }));
                            }

                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN_WIP" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN_WIP" }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }

            MESStationSession APCONFIG_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (APCONFIG_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (APCONFIG_Session.Value != null)
                {
                    APInfo = (Dictionary<string, List<DataRow>>)APCONFIG_Session.Value;
                    if (APInfo.Keys.Contains("R_PCBA_LINK"))
                    {
                        if (APInfo["R_PCBA_LINK"] == null || APInfo["R_PCBA_LINK"].Count <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " R_PCBA_LINK" }));
                        }
                        
                    }
                    else if (APInfo.Keys.Contains("C_STATION_KP"))
                    {
                        if (APInfo["C_STATION_KP"] == null || APInfo["C_STATION_KP"].Count <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " C_STATION_KP" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " R_PCBA_LINK" }));
                    }
                    if (APInfo.Keys.Contains("C_PRODUCT_CONFIG"))
                    {
                        if (APInfo["C_PRODUCT_CONFIG"] == null || APInfo["C_PRODUCT_CONFIG"].Count <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " C_PRODUCT_CONFIG" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " C_PRODUCT_CONFIG" }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            MESStationSession LinkQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (LinkQTY_Session == null)
            {
                LinkQTY_Session = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(LinkQTY_Session);
            }
            MESStationSession TRSNEXTQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (TRSNEXTQTY_Session == null)
            {
                TRSNEXTQTY_Session = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNEXTQTY_Session);
            }
            MESStationSession TRSNPcbSku_Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (TRSNPcbSku_Session == null)
            {
                TRSNPcbSku_Session = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNPcbSku_Session);
            }
            DataRow R_TR_SN_Row = TRInfo["R_TR_SN"];
            DataRow R_TR_SN_WIP_Row = TRInfo["R_TR_SN_WIP"];
            string strextQTY = R_TR_SN_WIP_Row["EXT_QTY"].ToString().Trim();
            strextQTY = (strextQTY == "") ? "0" : strextQTY;
            EXTQTY = Convert.ToInt32(strextQTY);
            TRSNEXTQTY_Session.Value = EXTQTY;
            TRSN_WORKFLAG = (R_TR_SN_Row["WORK_FLAG"] == null) ? "" : R_TR_SN_Row["WORK_FLAG"].ToString().Trim();
            TRSN_LOCATION_FLAG = (R_TR_SN_Row["LOCATION_FLAG"] == null) ? "" : R_TR_SN_Row["LOCATION_FLAG"].ToString().Trim();
            TRSNWIP_WORKFLAG = (R_TR_SN_WIP_Row["WORK_FLAG"] == null) ? "" : R_TR_SN_WIP_Row["WORK_FLAG"].ToString().Trim();
            WIPSKU = R_TR_SN_WIP_Row["KP_NO"].ToString();
            List<DataRow> R_PCBA_LINK_Row_List;
            try
            {
                R_PCBA_LINK_Row_List = APInfo["R_PCBA_LINK"];
            }
            catch
            {
                R_PCBA_LINK_Row_List = new List<DataRow>();
            }
            List<DataRow> C_PRODUCT_CONFIG_Row_List = APInfo["C_PRODUCT_CONFIG"];
            TRSNPcbSku_Session.Value = WIPSKU; //add by LLF 2018-03
            string kpno = string.Empty;
            if (Station.BU.Equals("BPD") ||Station.BU.Contains("HWT") || Station.BU.Contains("FJZ"))
            {
                kpno = "KP_NO";

            }
            else
            {
                kpno = "PCBA_SKUNO";
            }
            
            foreach (DataRow pcbLingRow in R_PCBA_LINK_Row_List)
            {
                LINKSKU.Add(pcbLingRow[kpno].ToString());
            }
            string strLinkQTY = "";
            if (C_PRODUCT_CONFIG_Row_List[0].Table.Columns.Contains("LINK_QTY"))
            {
                strLinkQTY = C_PRODUCT_CONFIG_Row_List[0]["LINK_QTY"].ToString().Trim();
            }

            strLinkQTY = (strLinkQTY == "") ? "0" : strLinkQTY;

            if (Station.BU.Equals("FJZ"))
            {
                strLinkQTY = APInfo["C_STATION_KP"][0]["LINK_FLAG"].ToString();
            }
            LINKQTY = Convert.ToInt32(strLinkQTY);
            LinkQTY_Session.Value = LINKQTY;
            try
            {
                if (EXTQTY <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000073", new string[] { R_TR_SN_Row["TR_SN"].ToString() }));
                }
                if (TRSN_WORKFLAG != "0" || TRSNWIP_WORKFLAG != "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000074", new string[] { R_TR_SN_Row["TR_SN"].ToString() }));
                }
                if (TRSN_LOCATION_FLAG != "2")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000140", new string[] { R_TR_SN_Row["TR_SN"].ToString(), TRSN_LOCATION_FLAG }));
                }
                if (LINKSKU.Count > 0 && !LINKSKU.Contains(WIPSKU))
                {
                    string strLinkSku = "";
                    for (int i = 0; i < LINKSKU.Count; i++)
                    {
                        if (strLinkSku == "")
                        {
                            strLinkSku = strLinkSku + LINKSKU[i];
                        }
                        else
                        {
                            strLinkSku = strLinkSku + "," + LINKSKU[i];
                        }
                    }
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000075", new string[] { R_TR_SN_Row["TR_SN"].ToString(), WIPSKU, strLinkSku }));
                }
                else
                {
                    if (Station.BU.Equals("FJZ"))
                    {
                        var strKPNO = APInfo["C_STATION_KP"][0]["KP_NO"].ToString();
                        if (strKPNO != WIPSKU)
                        {
                            //throw new Exception($@"The PCB PNO'{WIPSKU}' is diffident from config '{strKPNO}' C_STATION_KP");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104109", new string[] { WIPSKU,strKPNO }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TRSNEXTQTY_Session.Value = 0;
                LinkQTY_Session.Value = 0;
                throw ex;
            }
            Station.AddMessage("MES00000001", new string[] { R_TR_SN_Row["TR_SN"].ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);

        }
        /// <summary>
        /// 連板數量檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        //Add by LLF 2018-01-26 Begin
        public static void SMTLoadingLinkQtyChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
            {
            try
            {
                MESStationSession AP_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (AP_LinkQty == null)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000133", new string[] { }));
                }

                if (Convert.ToInt16(AP_LinkQty.Value.ToString()) <= 0)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000137", new string[] { AP_LinkQty.Value.ToString() }));
                }

                MESStationSession Input_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (Input_LinkQty == null)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000132", new string[] { }));
                }

                if (Convert.ToInt16(Input_LinkQty.Value.ToString()) > Convert.ToInt16(AP_LinkQty.Value.ToString()))
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000131", new string[] { Input_LinkQty.Value.ToString(), AP_LinkQty.Value.ToString() }));
                }
                Station.Inputs[3].Enable = true;
                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        //Add by LLF 2018-01-26 End

        /// <summary>
        /// 檢查當前工單是否上料齊套
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTPanelNoCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WO_Session = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");
            if (WO_Session == null)
            {
                Station.AddMessage("MES00000007", new string[] { "WO" }, StationMessageState.Fail);
                return;
            }
            string wo = WO_Session.Value.ToString();


            OleExec apdb = Station.APDB;
            //string msg = apdb.ExecProcedureNoReturn("", null);
            if (apdb != null)
            {

            }

        }

        /// <summary>
        /// 檢查當前工單的錫膏是否上線:
        ///HWD CHECK&補Allparts錫膏資料（連板&非連板均調用該SP）
        ///MES1.CHECK_SOLDER_INSERTDATA(Panelno,Nextevent,L_tmp_line,)
        ///var_message返回 OK則OK, 反之，throw(ErrorMessage)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTSolderDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //Marked by LLF 2018-01-29
            //if (Paras.Count == 0)
            //{
            //    throw new Exception("參數數量不正確!");
            //}

            OleExec apdb = Station.APDB;
            string PsnInsert = Input.Value.ToString();
            string Line = Station.Line;
            List<R_SN> ListRsn = new List<R_SN>();
            T_R_SN RSn = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            ListRsn = RSn.GetRSNbyPsn(PsnInsert, Station.SFCDB);
            //Modify BY LLF 20108-01-29,應該是獲取當前工站
            //string Next_Station = ListRsn[0].NEXT_STATION;
            string StationName = Station.StationName;
            OleDbParameter[] SolderSP = new OleDbParameter[4];
            SolderSP[0] = new OleDbParameter("G_PSN", PsnInsert);
            SolderSP[1] = new OleDbParameter("G_EVENTNAME", StationName);
            SolderSP[2] = new OleDbParameter("G_LINE", Line);
            SolderSP[3] = new OleDbParameter();
            SolderSP[3].Size = 1000;
            SolderSP[3].ParameterName = "RES";
            SolderSP[3].Direction = System.Data.ParameterDirection.Output;
            SolderSP[3].Size = 200;
            string result = apdb.ExecProcedureNoReturn("MES1.CHECK_SOLDER_INSERTDATA", SolderSP);
            if (result == "OK")
            {

                Station.AddMessage("MES00000062", new string[] { PsnInsert }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {

                throw new Exception(result);
            }
        }

        /// <summary>
        /// 檢查當前工單的鋼網是否上線
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTStencilDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }


            OleExec apdb = Station.APDB;

            if (apdb != null)
            {

            }
        }
        /// <summary>
        /// HWD Allparts AOI測試資料檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// MES1.CHECK_AOI_STATUS@mbdallpart(VAR_PANELNO,var_nextevent,var_productionline,var_LASTEDITBY,var_message )
        /// (G_SYSSERIALNO IN VARCHAR2,
        public static void AOITestAPDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            OleExec apdb = Station.APDB;
            string Psn = Input.Value.ToString();
            string Line = Station.Line;
            string StationName = Station.StationName;
            string EMP_NO = Station.LoginUser.EMP_NO;

            OleDbParameter[] StencilSP = new OleDbParameter[5];
            StencilSP[0] = new OleDbParameter("G_SYSSERIALNO", Psn);
            StencilSP[1] = new OleDbParameter("G_EVENTNAME", StationName);
            StencilSP[2] = new OleDbParameter("G_LINE_NAME", Line);
            StencilSP[3] = new OleDbParameter("G_EMP", EMP_NO);
            StencilSP[4] = new OleDbParameter();
            StencilSP[4].Size = 1000;
            StencilSP[4].ParameterName = "RES";
            StencilSP[4].Direction = System.Data.ParameterDirection.Output;
            //string result = apdb.ExecProcedureNoReturn("MES1.CHECK_AOI_STATUS@mbdallpart", StencilSP);
            string result = apdb.ExecProcedureNoReturn("MES1.CHECK_AOI_STATUS", StencilSP);
            if (result == "OK")
            {

                Station.AddMessage("MES00000062", new string[] { Psn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {

                throw new Exception(result);
            }
        }

        /// <summary>
        /// SMT Loading 檢查allpart條碼的數量是否大於輸入的link數量,AllPart條碼的數量是否是連半數的整倍數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTLoadingCheckLinkQtyAndTRQty(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                int trsnQty = 0;
                Dictionary<string, DataRow> TRInfo = new Dictionary<string, DataRow>();
                if (Paras.Count != 2)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (TRSN_Session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else
                {
                    if (TRSN_Session.Value != null)
                    {
                        //重新加載                  
                        APInfoDataloader.TRSNDataloader(Station, TRSN_Session.ResetInput, new List<R_Station_Action_Para>() { Paras[0] });
                        TRInfo = (Dictionary<string, DataRow>)TRSN_Session.Value;
                        if (TRInfo.Keys.Contains("R_TR_SN"))
                        {
                            if (TRInfo["R_TR_SN"] == null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                            }
                            else
                            {
                                trsnQty = Convert.ToInt32(TRInfo["R_TR_SN"]["QTY"].ToString());
                            }
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                        }

                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }

                MESStationSession Input_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (Input_LinkQty == null)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000132", new string[] { }));
                }

                //AllPart 條碼的數量不能小於輸入的連半數
                if (Convert.ToInt32(Input_LinkQty.Value.ToString()) > trsnQty)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000239", new string[] { Input_LinkQty.Value.ToString(), trsnQty.ToString() }));
                }

                //MESStationSession AP_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                //if (AP_LinkQty == null)
                //{
                //    Station.Inputs[2].Value = "";
                //    Station.Inputs[3].Enable = false;
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000133", new string[] { }));
                //}

                //if (Convert.ToInt32(AP_LinkQty.Value.ToString()) <= 0)
                //{
                //    Station.Inputs[2].Value = "";
                //    Station.Inputs[3].Enable = false;
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000137", new string[] { AP_LinkQty.Value.ToString() }));
                //}
                ////AllPart 條碼的數量必須是連半數的整倍數
                //if (trsnQty % Convert.ToInt32(AP_LinkQty.Value.ToString()) != 0)
                //{
                //    Station.Inputs[2].Value = "";
                //    Station.Inputs[3].Enable = false;
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000241", new string[] { AP_LinkQty.Value.ToString() }));
                //}

                Station.Inputs[3].Enable = true;
                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        /// <summary>
        /// CHECK_PSN_MATERIAL_PTH
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPSNMeaterialPTHAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec apdb = null;
            try
            {
                apdb = Station.APDB;
                string StationLine = string.Empty;
                string StationNum = string.Empty;
                string SN = string.Empty;

                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                if (SNSession.Value.GetType() == typeof(SN))
                {
                    SN = ((SN)SNSession.Value).SerialNo;
                }
                else if (SNSession.Value.GetType() == typeof(Panel))
                {
                    SN = ((Panel)SNSession.Value).PanelNo;
                }
                else
                {
                    SN = SNSession.Value.ToString().Trim().ToUpper();
                }

                MESStationSession StationNumSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (StationNumSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                StationNum = StationNumSession.Value.ToString();
                var pStation = Paras.Find(t => t.SESSION_TYPE == "PTHSTATION");
                if (pStation == null)
                {
                    StationLine = Station.Line + Station.StationName + StationNum;
                }
                else
                {
                    StationLine = Station.Line + pStation.VALUE + StationNum;
                }

                OleDbParameter[] CheckPSNMeaterialPTHSP = new OleDbParameter[4];
                CheckPSNMeaterialPTHSP[0] = new OleDbParameter("G_PSN", SN);
                CheckPSNMeaterialPTHSP[1] = new OleDbParameter("G_EVENT", Station.StationName);
                CheckPSNMeaterialPTHSP[2] = new OleDbParameter("G_LINE", StationLine);
                CheckPSNMeaterialPTHSP[3] = new OleDbParameter();
                CheckPSNMeaterialPTHSP[3].Size = 1000;
                CheckPSNMeaterialPTHSP[3].ParameterName = "RES";
                CheckPSNMeaterialPTHSP[3].Direction = System.Data.ParameterDirection.Output;
                string result = apdb.ExecProcedureNoReturn("MES1.CHECK_PSN_MATERIAL_PTH", CheckPSNMeaterialPTHSP);
                if (result.IndexOf("OK") < 0)
                {
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }

        /// <summary>
        /// 參考VT 卡PTH是否上料 只取一個參數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPSNMeaterialPTHAction_DCN(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec apdb = null;
            OleExec sfcdb = null;
            try
            {
                apdb = Station.APDB;
                sfcdb = Station.SFCDB;
                string StationLine = string.Empty;
                string StationNum = string.Empty;
                string SN = string.Empty;

                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SN = SNSession.Value.ToString().Trim().ToUpper();

                //MESStationSession StationNumSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                //if (StationNumSession == null)
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                //}
                //StationNum = StationNumSession.Value.ToString();
                //var pStation = Paras.Find(t => t.SESSION_TYPE == "PTHSTATION");
                //if (pStation == null)
                //{
                //    StationLine = Station.Line + Station.StationName + StationNum;
                //}
                //else
                //{
                //    StationLine = Station.Line + pStation.VALUE + StationNum;
                //}

                //QE有在機種擴展配置配置就不卡
                string strSql = $@" select *
                                        from c_sku_detail
                                       where category = 'NOCHECK_PTH'
                                         and skuno in (select skuno from r_sn where sn = '{SN}')";
                DataTable res = sfcdb.ExecSelect(strSql).Tables[0];

                if (res.Rows.Count == 0)
                {
                    OleDbParameter[] CheckPSNMeaterialPTHSP = new OleDbParameter[4];
                    CheckPSNMeaterialPTHSP[0] = new OleDbParameter("G_PSN", SN);
                    CheckPSNMeaterialPTHSP[1] = new OleDbParameter("G_EVENT", Station.StationName);
                    CheckPSNMeaterialPTHSP[2] = new OleDbParameter("G_LINE", StationLine);
                    CheckPSNMeaterialPTHSP[3] = new OleDbParameter();
                    CheckPSNMeaterialPTHSP[3].Size = 1000;
                    CheckPSNMeaterialPTHSP[3].ParameterName = "RES";
                    CheckPSNMeaterialPTHSP[3].Direction = System.Data.ParameterDirection.Output;
                    string result = apdb.ExecProcedureNoReturn("MES1.CHECK_PSN_MATERIAL_PTH", CheckPSNMeaterialPTHSP);
                    if (result.IndexOf("OK") < 0)
                    {
                        throw new Exception(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }


        /// <summary>
        /// HWT過站檢查allpart資料
        /// add by hgb 2019.05.23,輸入SN,工站，腺體，ip,工單
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void checkPsnMaterialAoiAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            try
            {
                if (Paras.Count != 2)
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
                AP_DLL ApDll = new AP_DLL();
                string result = ApDll.CHECK_PSN_MATERIAL_AOI_NEW(SN, Station.StationName, Station.Line, Station.IP, WO, Station.APDB);

                if (result.IndexOf("OK") < 0)
                {
                    throw new Exception(result);
                }
            over_allparts_check:;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }


        /// <summary>
        /// HWT檢查是否需要上輔料
        /// add by hgb 2019.05.28,工站,工單
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void checkSfcCheckAssistKpAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                if (Paras.Count != 1)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (WOSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                string WO = WOSession.Value.ToString().Trim().ToUpper();


                T_R_WO_TYPE WOType = new T_R_WO_TYPE(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //除了RMA工單，其他工單都要卡是否需要上輔料
                if (!WOType.IsTypeInput("RMA", WO.Substring(0, 6), Station.SFCDB))
                {
                    AP_DLL ApDll = new AP_DLL();
                    string result = ApDll.SFC_CHECK_ASSIST_KP(Station.StationName, WO, Station.APDB);
                    if (result.IndexOf("OK") < 0)
                    {
                        throw new Exception(result);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }


        /// <summary>
        /// 檢查虛擬條碼
        /// add by hgb 2019.05.29
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckVirtualSn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
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
                if (ApDll.CheckIsNeedCheckVirtualSn( SKUNO, Station.APDB))//是否配置需要檢查虛擬條碼
                {
                    if (WO.Substring(0, 2) == "99")//虛擬工單
                    {
                        ApDll.CheckVirtualSnIsFihishScan(SN, SKUNO, Station.StationName, Station.IP, WO, Station.APDB);//檢查上一輪是否有掃完                       
                    }
                    else //正常工單
                    {
                        ApDll.CheckVirtualSnIsScan(SN, SKUNO, Station.StationName, Station.IP, WO, Station.SFCDB, Station.APDB);//檢查是否已掃描虛擬條碼
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
        /// 按工單檢查輔料是否上線
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckAssistKpByWOChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            WorkOrder ObjWorkorder = (WorkOrder)WOSession.Value;
            if (ObjWorkorder == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { WOSession.Value.ToString().Trim() }));
            }

            AP_DLL ApDll = new AP_DLL();
            string result = ApDll.SFC_CHECK_ASSIST_KP(Station.StationName, ObjWorkorder.WorkorderNo, Station.APDB);
            if (result.IndexOf("OK") < 0)
            {
                throw new Exception(result);
            }
        }

        /// <summary>
        /// SMTLOADING檢查TRSN信息:料號與料號PCB料號是否一致,是否備入工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTTRSNExtendDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            #region 獲取TRSN對象
            MESStationSession sTRSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sTRSN == null)
            {

                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            #endregion

            #region 獲取WO對象
            MESStationSession sWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            #endregion

            #region 獲取APConfig對象
            MESStationSession sAPConfig = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sAPConfig == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            #endregion
            
            OleExec APDB = null;
            AP_DLL APDLL = new AP_DLL();
            List<DataRow> woList = new List<DataRow>();
            Dictionary<string, DataRow> TRInfo = new Dictionary<string, DataRow>();
            Dictionary<string, List<DataRow>> APInfo = new Dictionary<string, List<DataRow>>();

            try
            {
                APDB = Station.APDB;
                TRInfo = (Dictionary<string, DataRow>)sTRSN.Value;
                if (TRInfo.Keys.Contains("R_TR_SN"))
                {
                    if (TRInfo["R_TR_SN"] == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                    }
                }
                //檢查料號與料號PCB料號是否一致
                APInfo = (Dictionary<string, List<DataRow>>)sAPConfig.Value;
                if (APInfo.Keys.Contains("C_STATION_KP"))
                {
                    if (APInfo["C_STATION_KP"][0]["KP_NO"].ToString() != TRInfo["R_TR_SN"]["CUST_KP_NO"].ToString())
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200506141212", new string[] { TRInfo["R_TR_SN"]["TR_SN"].ToString() }));
                    }
                }
                //檢查是否備入工單
                if (sWO.Value.ToString() != TRInfo["R_TR_SN"]["KITTING_WO"].ToString())
                {
                    woList = APDLL.GetRVWOByWO(sWO.Value.ToString(), APDB);
                    if (woList.Count <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200506142949", new string[] { TRInfo["R_TR_SN"]["TR_SN"].ToString(), sWO.Value.ToString() }));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void UFGLUEDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            string kp = "''";
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }


            MESStationSession sWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            WorkOrder wo = (WorkOrder)sWO.Value;
            OleExec APDB = null;
            string strSql = $@"select * From mes1.c_station_kp where P_NO = '{wo.SkuNO}' AND P_VERSION ='{wo.SKU_VER}' AND STATION_NAME = 'UF'";

            try
            {
                APDB = Station.APDB;
                DataTable res = APDB.ExecSelect(strSql).Tables[0];
                if (res.Rows.Count > 0)
                {
                    if (res.Rows.Count > 2)
                    {
                        //throw new MESReturnMessage("配置膠水料號数量错误，請找PE確認");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105228"));

                    }

                    else {
                        foreach (DataRow row in res.Rows) {
                            kp += ",'"+row["KP_NO"].ToString()+"'";
                        }
                        
                    }

                }
                else
                {
                    //throw new MESReturnMessage("沒有配置膠水料號，請找PE確認");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105325"));

                }

                MESStationSession TempSession = Station.StationSession.Find(t => t.MESDataType == "KP_NO" && t.SessionKey == "1");
                if (TempSession == null)
                {
                    TempSession = new MESStationSession() { MESDataType = "KP_NO", SessionKey = "1", Value = kp };
                    Station.StationSession.Add(TempSession);
                }
                else
                {
                    TempSession.Value = kp;
                }
                MESStationSession rowSession = Station.StationSession.Find(t => t.MESDataType == "ROWS" && t.SessionKey == "1");
                if (rowSession == null)
                {
                    rowSession = new MESStationSession() { MESDataType = "ROWS", SessionKey = "1", Value = res.Rows.Count };
                    Station.StationSession.Add(rowSession);
                }
                else
                {
                    rowSession.Value = res.Rows.Count;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void UFTRSNDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string kpNO = string.Empty;
            int rows = 0;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sKp = Station.StationSession.Find(it=>it.MESDataType=="KP_NO"&& it.SessionKey == "1");
            if (sKp == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "KP_NO"}));
            }
            else {
                kpNO = sKp.Value.ToString();
            }

            if (Input.DisplayName == "TR_SN")
            {

                string sTRsn = Input.Value.ToString();
                MESStationSession sRow = Station.StationSession.Find(it => it.MESDataType == "ROWS" && it.SessionKey == "1");
                if (sRow == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "ROWS" }));
                }
                else
                {
                    rows = Convert.ToInt32(sRow.Value.ToString());
                }

                WorkOrder wo = (WorkOrder)sWO.Value;
                OleExec APDB = null;
                string strSql = $@"select * from mes4.r_solder_detail where tr_sn  = '{sTRsn}' and REMAK is null";

                try
                {
                    APDB = Station.APDB;
                    DataTable res = APDB.ExecSelect(strSql).Tables[0];
                    if (res.Rows.Count > 0)
                    {
                        strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{sTRsn}' and WO ='{wo.WorkorderNo}' and KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-46/24";
                    }
                    else {
                        strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{sTRsn}' and WO ='{wo.WorkorderNo}' and KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-8/24";
                    }
                    res = APDB.ExecSelect(strSql).Tables[0];
                    if (res.Rows.Count > 0)
                    {
                        if (rows == 1)
                        {
                            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == "SN");

                            if (snInput != null)
                            {
                                snInput.Enable = true;
                                Station.NextInput = snInput;
                            }
                        }
                        else if (rows == 2)
                        {
                            MESStationInput sn1Input = Station.Inputs.Find(t => t.DisplayName == "TR_SN1");
                            if (sn1Input != null)
                            {
                                sn1Input.Enable = true;
                                Station.NextInput = sn1Input;
                            }
                        }

                    }
                    else
                    {
                        //throw new MESReturnMessage($@"{sTRsn}->沒有發料到{wo.WorkorderNo} 的 {Station.StationName} 工站/或已用完/或已超46H(2次回溫不能超8H),請確認");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105407", new string[] { sTRsn, wo.WorkorderNo, Station.StationName }));
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else {

                string sTRsn = Input.Value.ToString();
                WorkOrder wo = (WorkOrder)sWO.Value;
                OleExec APDB = null;
                string strSql = $@"select * from mes4.r_solder_detail where tr_sn  = '{sTRsn}' and REMAK is null";
                try
                {
                    APDB = Station.APDB;
                    DataTable res = APDB.ExecSelect(strSql).Tables[0];
                    if (res.Rows.Count > 0)
                    {
                        strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{sTRsn}' and WO ='{wo.WorkorderNo}' and KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-46/24";
                    }
                    else
                    {
                        strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{sTRsn}' and WO ='{wo.WorkorderNo}' KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-8/24";
                    }
                    res = APDB.ExecSelect(strSql).Tables[0];
                    if (res.Rows.Count == 0)
                    {
                        //throw new MESReturnMessage($@"{sTRsn}->沒有發料到{wo.WorkorderNo} 的 {Station.StationName} 工站/或已用完/或已超46H(2次回溫不能超8H),請確認");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105407", new string[] { sTRsn, wo.WorkorderNo, Station.StationName }));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
        }

        public static void UFSNDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int rows = 0;
            string kpNO = string.Empty;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sRow = Station.StationSession.Find(it => it.MESDataType == "ROWS" && it.SessionKey == "1");
            if (sRow == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "ROWS" }));
            }
            else
            {
                rows = Convert.ToInt32(sRow.Value.ToString());
            }
            MESStationSession sKp = Station.StationSession.Find(it => it.MESDataType == "KP_NO" && it.SessionKey == "1");
            if (sKp == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "KP_NO" }));
            }
            else
            {
                kpNO = sKp.Value.ToString();
            }
            WorkOrder wo = (WorkOrder)sWO.Value;
            OleExec APDB = null;
            try
            {
                var trSN = Station.Inputs.Find(it => it.DisplayName == "TR_SN").Value.ToString();
                var trSN1 = Station.Inputs.Find(it => it.DisplayName == "TR_SN1").Value.ToString();
                APDB = Station.APDB;
                string strSql = $@"select * from mes4.r_solder_detail where tr_sn  = '{trSN}' and REMAK is null";
                DataTable res = APDB.ExecSelect(strSql).Tables[0];
                if (res.Rows.Count > 0)
                {
                    strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{trSN}' and WO ='{wo.WorkorderNo}' and KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-46/24";
                }
                else
                {
                    strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{trSN}' and WO ='{wo.WorkorderNo}' AND  KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-8/24";
                }
                res = APDB.ExecSelect(strSql).Tables[0];
                if (rows == 1)
                {
                    if (res.Rows.Count == 0)
                    {
                        //throw new MESReturnMessage($@"{trSN}->沒有發料到{wo.WorkorderNo} 的 {Station.StationName} 工站/或已用完/或已超46H(2次回溫不能超8H),請確認");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105407", new string[] { trSN, wo.WorkorderNo, Station.StationName }));
                    }
                }
                else if (rows == 2)
                {
                    string strSql1 = $@"select * from mes4.r_solder_detail where tr_sn  = '{trSN1}' and REMAK is null";
                    DataTable res1 = APDB.ExecSelect(strSql1).Tables[0];

                    if (res1.Rows.Count > 0)
                    {
                        strSql1 = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{trSN1}' and WO ='{wo.WorkorderNo}' and KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-46/24";
                    }
                    else
                    {
                        strSql1 = $@"select * from mes4.r_tr_sn_wip where tr_sn = '{trSN1}' and WO ='{wo.WorkorderNo}' KP_NO in  ({kpNO}) and WORK_TIME >SYSDATE-8/24";
                    }
                    res1 = APDB.ExecSelect(strSql1).Tables[0];

                    if (res.Rows.Count == 0 || res1.Rows.Count == 0)
                    {
                        //throw new MESReturnMessage($@"{trSN} 或/ {trSN1}->沒有發料到{wo.WorkorderNo} 的 {Station.StationName} 工站/或已用完/或已超46H(2次回溫不能超8H),請確認");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105657", new string[] { trSN, wo.WorkorderNo, Station.StationName }));
                    }
                }
                else {
                    //throw new MESReturnMessage($@"配置膠水料號數據異常,請聯繫PE分析");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110125", new string[] { Paras[0].SESSION_TYPE }));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查PCB板子LOADING時間是否大於1小時  add by wuqing 2021年3月18日15:10:03
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPCBA_LOADING_TIME(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec apdb = null;
            OleExec sfcdb = null;
            try
            {
                apdb = Station.APDB;
                sfcdb = Station.SFCDB;
                string TRSN = string.Empty;
                MESStationSession sWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (sWO == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                MESStationSession TRSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (TRSNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                TRSN = TRSNSession.InputValue.ToString().Trim().ToUpper();

                WorkOrder wo = (WorkOrder)sWO.Value;

                OleDbParameter[] CheckPSNMeaterialPTHSP = new OleDbParameter[4];
                CheckPSNMeaterialPTHSP[0] = new OleDbParameter("G_TRSN", TRSN);
                CheckPSNMeaterialPTHSP[1] = new OleDbParameter("G_WO", wo.WorkorderNo);
                CheckPSNMeaterialPTHSP[2] = new OleDbParameter("G_EVENT",Station.StationName);
                CheckPSNMeaterialPTHSP[3] = new OleDbParameter();
                CheckPSNMeaterialPTHSP[3].Size = 1000;
                CheckPSNMeaterialPTHSP[3].ParameterName = "RES";
                CheckPSNMeaterialPTHSP[3].Direction = System.Data.ParameterDirection.Output;
                string result = apdb.ExecProcedureNoReturn("MES1.CHECK_TRSN_LOADING_TIME", CheckPSNMeaterialPTHSP);
                if (result.IndexOf("OK") < 0)
                {
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }

        /// <summary>
        /// 用於檢查是否是HW推送過來的鎖定信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckHW3Lock(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            OleDbParameter[] parameters = new OleDbParameter[2];
            parameters[0] = new OleDbParameter("G_SN", SNSession.Value);
            parameters[0].OleDbType = OleDbType.VarChar;
            parameters[0].Direction = ParameterDirection.Input;

            parameters[1] = new OleDbParameter("RES", OleDbType.VarChar);
            parameters[1].Direction = ParameterDirection.Output;
            parameters[1].Size = 500;

            var result = Station.APDB.ExecProcedureNoReturn("MES1.CHECK_LOCK", parameters);
            if (result is null || result != "OK")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211004123054", new string[] { result }));
            }
        }
    }
}