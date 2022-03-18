using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDBHelper;
using SqlSugar;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDataObject.Module.HWD;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class APInfoDataloader
    {
        /// <summary>
        /// 加載SKU在Allpart的配置信息從Allpart系統加載mes1.c_product_config, mes4.r_pcba_link 
        /// 將查詢到的結果存入Dictionary<string_Datarow> 中, key為表名如:" c_product_config "
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SkuAPInfoDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string StrSku = "";
            string StrVer = "";
            string strBU = "";
            SKU sku = new SKU();
            OleExec apdb = null;
            List<DataRow> PCBALinkList = new List<DataRow>();
            List<DataRow> ConfigList = new List<DataRow>();

            Dictionary<string, List<DataRow>> APInfo = new Dictionary<string, List<DataRow>>();
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession APConfig_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (APConfig_Session == null)
            {
                APConfig_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(APConfig_Session);
            }

            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            //Modify by LLF 2017-01-25 For 獲取工單對象，從工單對象中獲取料號，版本
            //MESStationSession Sku_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            //if (Sku_Session == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            //}
            //else
            //{
            //    sku = (SKU)Sku_Session.Value;
            //    if (sku == null || sku.SkuNo == null || sku.SkuNo.Length <= 0)
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            //    }
            //    if (sku.Version == null || sku.Version.Length <= 0)
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " Version" }));
            //    }
            //}
            //獲取ALLPART數據 
            try
            {
                strBU = Station.BU;
                StrSku = ((MESStation.LogicObject.WorkOrder)Wo_Session.Value).SkuNO;
                StrVer = ((MESStation.LogicObject.WorkOrder)Wo_Session.Value).SKU_VER;
                apdb = Station.APDB;
                AP_DLL APDLL = new AP_DLL();
                if (strBU.Contains("HWT"))
                {
                    ConfigList = APDLL.C_Product_Config_GetBYSkuAndVerson_like(StrSku, StrVer, apdb);
                }
                else
                {
                    ConfigList = APDLL.C_Product_Config_GetBYSkuAndVerson(StrSku, StrVer, apdb);
                }
                //List<DataRow> ConfigList = APDLL.C_Product_Config_GetBYSkuAndVerson(StrSku, StrVer, apdb);


                if (ConfigList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + StrSku + ",VER:" + StrVer, "C_PRODUCT_CONFIG" }));
                }
                APInfo.Add("C_PRODUCT_CONFIG", ConfigList);
                if (Station.BU.Equals("BPD"))
                {
                    PCBALinkList = APDLL.R_PCBA_LINK_GetBYPno(StrSku, apdb);
                }
                else if (Station.BU.Contains("HWT") || Station.BU.Contains("FJZ"))
                {
                    PCBALinkList = APDLL.C_STATION_KP_GetBYPno(StrSku, StrVer, apdb);
                }
                else
                {
                    PCBALinkList = APDLL.R_PCBA_LINK_GetBYSku(StrSku, apdb);
                }

                if (PCBALinkList.Count <= 0)
                {
                    if (Station.BU.Contains("HWT") || Station.BU.Contains("FJZ"))
                    {//"SKU 沒有配置SMTLOADING 工站PNO 請找PE"
                        //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + StrSku, "C_STATION_KP" }));
                        throw new Exception($@"SKU:'{StrSku}' not configed SMTLOADING PNO(C_STATION_KP);Pls find PE");
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + StrSku, "R_PCBA_LINK" }));
                    }
                }

                if (Station.BU.Contains("FJZ"))
                {
                    APInfo.Add("C_STATION_KP", PCBALinkList);
                }
                else
                {
                    APInfo.Add("R_PCBA_LINK", PCBALinkList);
                }
                APConfig_Session.Value = APInfo;

                Station.AddMessage("MES00000001", new string[] { StrSku }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                //Modify By LLF 2018-01-25 For 料號&版本從工單對象中獲取，而不是從C_SKU 中獲取
                /* List<DataRow> ConfigList = APDLL.C_Product_Config_GetBYSkuAndVerson(sku.SkuNo, sku.Version, apdb);
                if (ConfigList.Count <= 0)
                {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + sku.SkuNo, "C_PRODUCT_CONFIG" }));
                }
                APInfo.Add("C_PRODUCT_CONFIG", ConfigList);
                List<DataRow> PCBALinkList = APDLL.R_PCBA_LINK_GetBYSku(sku.SkuNo, apdb);
                if (PCBALinkList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + sku.SkuNo, "R_PCBA_LINK" }));
                }
                APInfo.Add("R_PCBA_LINK", PCBALinkList);
                APConfig_Session.Value = APInfo;
                Station.DBS["APDB"].Return(apdb);
                Station.AddMessage("MES00000001", new string[] { sku.SkuNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                */
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {

                }
                throw ex;
            }
        }

        /// <summary>
        /// TR_SN數據加載，查詢R_TR_SN,R_TR_SN_WIP的數據保存到Dictionary<string_Datarow>中,key為表名 "R_TR_SN","R_TR_SN_WIP"
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRSNDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Dictionary<string, DataRow> APInfo = new Dictionary<string, DataRow>();
            string strTRSN = "";
            //string ErrMessage = "";
            OleExec apdb = null;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            strTRSN = Input.Value.ToString();
            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {

                TRSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSN_Session);
            }
            else
            {
                TRSN_Session.ResetInput = Input;
                TRSN_Session.InputValue = strTRSN;
            }

            //獲取ALLPART數據
            APInfo = new Dictionary<string, DataRow>();
            AP_DLL APDLL = new AP_DLL();
            try
            {
                apdb = Station.APDB;
                List<DataRow> TRSNlist = APDLL.R_TR_SN_GetBYTR_SN(strTRSN, apdb);
                if (TRSNlist.Count <= 0)
                {
                    //throw new Exception("TRSN:" + "不存在ALLPART系統R_TR_SN中");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN" }));
                }
                else
                {
                    APInfo.Add("R_TR_SN", TRSNlist[0]);
                }
                List<DataRow> TRSNWIPlist = APDLL.R_TR_SN_WIP_GetBYTR_SN(strTRSN, apdb);
                if (TRSNWIPlist.Count <= 0)
                {
                    //throw new Exception("TRSN:" + "不存在ALLPART系統R_TR_SN_WIP中");
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN_WIP" }));
                    APInfo.Add("R_TR_SN_WIP", null);
                }
                else
                {
                    APInfo.Add("R_TR_SN_WIP", TRSNWIPlist[0]);
                }
                TRSN_Session.Value = APInfo;

                Station.AddMessage("MES00000001", new string[] { TRSN_Session.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {

                }
                throw ex;
            }
        }

        /// <summary>
        /// HWT 通過機種獲取ALLPART的連板數，用於判斷下一槍是掃描SN還是LOTCODE
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LinkQTYFromSKUDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string strWO = "";

            OleExec apdb = null;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載LinkQTY
            MESStationSession LinkQTYSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LinkQTYSession == null)
            {
                LinkQTYSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(LinkQTYSession);
            }

            //strSKU= Station.StationSession[1].Value.ToString();
            strWO = Station.StationSession[0].Value.ToString();

            //SKU sku = new SKU();
            try
            {
                apdb = Station.APDB;
                AP_DLL APDLL = new AP_DLL();
                string result = APDLL.AP_GET_LINKQTY(strWO, apdb);
                if (result.Length > 2)
                {
                    throw new Exception(result);
                }
                else
                {
                    int linkqty = Convert.ToInt32(result);
                    if (linkqty == 1)
                    {
                        Station.Inputs[2].Enable = true;
                        //Station.StationMessages.Add(new StationMessage { Message = "OK!TR S/N CHECK PASS,PLEASE INPUT PCB30 SN 請掃描30位LC條碼", State = StationMessageState.Message });
                    }
                    else
                    {
                        Station.Inputs[2].Enable = false;
                    }
                    LinkQTYSession.Value = result;
                }

                //Station.StationMessages.Add(new StationMessage { Message = "OK!TR S/N CHECK PASS,PLEASE INPUT PCB30 SN 請掃描30位LC條碼", State = StationMessageState.Message });
                //Station.AddMessage("OK!TR S/N CHECK PASS,PLEASE INPUT PCB30 SN 請掃描30位LC條碼", new string[] { /*"Skuno", sku.ToString()*/ }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception result)
            {
                //Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw result;
            }
        }

        /// <summary>
        /// HWT SMTLOADING掃描ALLPART條碼CALL SP判斷
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWT_SMTLOADING_TRSN_LOAD(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //Dictionary<string, DataRow> APInfo = new Dictionary<string, DataRow>();
            string strTRSN = "", strWO = Station.StationSession[0].Value.ToString(), strIP = Station.IP.ToString(), strEMPNO = Station.LoginUser.EMP_NO.ToString();
            //string ErrMessage = "";
            OleExec apdb = null;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            strTRSN = Input.Value.ToString();
            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {

                TRSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSN_Session);
            }
            else
            {
                TRSN_Session.ResetInput = Input;
                TRSN_Session.InputValue = strTRSN;
                TRSN_Session.Value = strTRSN;
            }
            if (strWO.Substring(0, 2) != "00")
            {
                // throw new Exception("輸入工單後請回車，否則無法正確加載數據！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105536"));
            }
            //獲取ALLPART數據
            //APInfo = new Dictionary<string, DataRow>();
            AP_DLL APDLL = new AP_DLL();
            try
            {
                apdb = Station.APDB;
                string result = APDLL.AP_get_trcode(strTRSN, strWO, strIP, strEMPNO, "", apdb);
                //TRSN_Session.Value = APInfo;
                //if (Station.StationSession[2].Value.ToString() == "1")
                //{
                //    Station.StationMessages.Add(new StationMessage { Message = "請掃描30位LOT_CODE條碼", State = StationMessageState.Message });
                //}                
                //Station.AddMessage("MES00000001", new string[] { TRSN_Session.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {

                }
                throw ex;
            }
        }

        /// <summary>
        /// HWT SMTLOADING后更新ALLPART的相關資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWT_SMTLOADING_UPDATE_AP(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string strTRSN = "", strWO = "", strCODE = "", strIP = Station.IP.ToString(), strEMPNO = Station.LoginUser.EMP_NO.ToString(), strSN = "";
            OleExec apdb = null;
            if (Paras.Count != 4)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {
                //  throw new Exception("ALLPART條碼獲取失敗，請聯繫IT處理！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110332"));
            }
            else
            {
                strTRSN = TRSN_Session.InputValue.ToString();
                //strTRSN = TRSN_Session.Value.ToString();
            }

            MESStationSession WO_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WO_Session == null)
            {
                //throw new Exception("工單獲取失敗，請聯繫IT處理！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112110"));
            }
            else
            {
                //strWO = WO_Session.InputValue.ToString();
                strWO = WO_Session.Value.ToString();
            }

            MESStationSession TRCODE_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TRCODE_Session == null)
            {
              //  throw new Exception("TRCODE獲取失敗，請聯繫IT處理！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113212"));
            }
            else
            {
                strCODE = TRCODE_Session.Value.ToString();
            }

            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (SN_Session == null)
            {
                // throw new Exception("TRCODE獲取失敗，請聯繫IT處理！");

                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113212"));
            }
            else
            {
                strSN = SN_Session.Value.ToString();
            }

            AP_DLL APDLL = new AP_DLL();
            try
            {
                apdb = Station.APDB;
                string result = APDLL.AP_z_insert_panel_snlink_new(strTRSN, strWO, strIP, strEMPNO, strSN, strCODE, apdb);
                if (result.Substring(0, 2) != "OK")
                {
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                //if (apdb != null)
                //{

                //}
                throw ex;
            }
        }

        //Add by LLF 2017-01-26 Begin
        public static void TrCodeDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Process = "";
            string Message = "";
            string StrWO = "";
            string StrCode = "";
            string IP = Station.IP;
            string strBU = Station.BU;
            OleExec APDB = null;
            Dictionary<string, List<DataRow>> APInfo = new Dictionary<string, List<DataRow>>();

            Dictionary<string, DataRow> TrSnTable = null;
            T_R_SN Table = new T_R_SN(Station.SFCDB, Station.DBType);

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WoSession.Value.ToString();

            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }



            if (strBU.Contains("HWT"))
            {
                //獲取 APCONFIG 對象
                MESStationSession APCONFIG_Session = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
                if (APCONFIG_Session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                else
                {
                    if (APCONFIG_Session.Value != null)
                    {
                        APInfo = (Dictionary<string, List<DataRow>>)APCONFIG_Session.Value;
                        if (APInfo.Keys.Contains("C_PRODUCT_CONFIG"))
                        {
                            if (APInfo["C_PRODUCT_CONFIG"] == null || APInfo["C_PRODUCT_CONFIG"].Count <= 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY + " C_PRODUCT_CONFIG" }));
                            }
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY + " C_PRODUCT_CONFIG" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                    }
                }

                Process = APInfo["C_PRODUCT_CONFIG"][0]["PROCESS_FLAG"].ToString();
            }
            else
            {
                Process = Paras[2].VALUE.ToString();
            }

            //TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;
            //Process = Paras[2].VALUE.ToString();

            //IP = Paras[3].VALUE.ToString();

            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            try
            {
                APDB = Station.APDB;
                if (Station.BU.Equals("BPD"))
                {
                    StrCode = Table.GetAPTrCodes(StrWO, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, Station.LoginUser.EMP_NO, IP, APDB);
                }
                else if (Station.BU.Equals("FJZ")) {
                    AP_DLL AP = new AP_DLL();
                    StrCode = AP.LH_NSDI_GetAPTrCode(TrSnTable["R_TR_SN"]["TR_SN"].ToString(), StrWO, IP, APDB, DB_TYPE_ENUM.Oracle);
                }
                else
                {
                    StrCode = Table.GetAPTrCode(StrWO, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, Station.LoginUser.EMP_NO, IP, APDB);

                }
                if (StrCode == "")
                {
                    // throw new Exception("流水碼TR_CODE生成失敗，請聯繫IT！");

                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113345"));
                }
                MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (TrCodeSession == null)
                {
                    TrCodeSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(TrCodeSession);
                }

                TrCodeSession.Value = StrCode;
                Station.AddMessage("MES00000001", new string[] { TrCodeSession.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        //Add by LLF 2017-01-26 End

        /// <summary>
        /// TR_SN數據對象加載其中每個欄位 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadDataFromTRSNDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            object TRSN_SessionObject;
            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (TRSN_Session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (TRSN_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                TRSN_SessionObject = TRSN_Session.Value;

                //獲取TR_SN 欄位數據
                Dictionary<string, DataRow> dd = (Dictionary<string, DataRow>)TRSN_SessionObject;
                DataRow tr;
                dd.TryGetValue("R_TR_SN", out tr);

                MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "KP_NO");
                List<object> ret = I.DataForUse;
                ret.Clear();
                if (I != null)
                {
                    ret.Add(tr["CUST_KP_NO"].ToString());
                }

                MESStationInput I1 = Station.Inputs.Find(t => t.DisplayName == "MFR_Name");
                List<object> ret1 = I1.DataForUse;
                if (I1 != null)
                {
                    ret1.Add(tr["MFR_KP_NO"].ToString());
                }
                MESStationInput I2 = Station.Inputs.Find(t => t.DisplayName == "Date_Code");
                List<object> ret2 = I2.DataForUse;
                if (I2 != null)
                {
                    ret2.Add(tr["DATE_CODE"].ToString());
                }
                MESStationInput I3 = Station.Inputs.Find(t => t.DisplayName == "Lot_Code");
                List<object> ret3 = I3.DataForUse;
                if (I3 != null)
                {
                    ret3.Add(tr["LOT_CODE"].ToString());
                }
                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 維修輸入加載TR_SN的DataRow對象，并把KP_NO,MFR_Name,Date_Code,Lot_Code到加載到對應輸入框的值
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairTRSNObjDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            string inputValue = Input.Value.ToString();
            MESStationSession sessionTRSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionTRSN == null)
            {
                sessionTRSN = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionTRSN);
            }
            try
            {
                AP_DLL APDLL = new AP_DLL();
                List<DataRow> TRSNlist = APDLL.R_TR_SN_GetBYTR_SN(inputValue, Station.APDB);
                if (TRSNlist.Count == 0)
                {
                    Station.Inputs.Find(input => input.DisplayName == "KP_NO").Value = "";
                    Station.Inputs.Find(input => input.DisplayName == "MFR_Name").Value = "";
                    Station.Inputs.Find(input => input.DisplayName == "Date_Code").Value = "";
                    Station.Inputs.Find(input => input.DisplayName == "Lot_Code").Value = "";
                    Station.Inputs.Find(input => input.DisplayName == "TR_SN").Value = "";
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + inputValue, "R_TR_SN" }));
                }
                else
                {
                    Station.Inputs.Find(input => input.DisplayName == "KP_NO").Value = TRSNlist[0]["CUST_KP_NO"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "MFR_Name").Value = TRSNlist[0]["MFR_KP_NO"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "Date_Code").Value = TRSNlist[0]["DATE_CODE"].ToString();
                    Station.Inputs.Find(input => input.DisplayName == "Lot_Code").Value = TRSNlist[0]["Lot_Code"].ToString();
                    Dictionary<string, DataRow> TRSNInfo = new Dictionary<string, DataRow>();
                    TRSNInfo.Add("TR_SN", TRSNlist[0]);
                    sessionTRSN.Value = TRSNlist[0];
                    sessionTRSN.InputValue = inputValue;
                    sessionTRSN.ResetInput = Input;
                    Station.NextInput = Station.Inputs.Find(input => input.DisplayName == "Description");
                    Station.AddMessage("MES00000001", new string[] { inputValue }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 加載TR_SN的KP_NO,MFR_Name,Date_Code,Lot_Code到MESStationSession,以便輸出
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRSNInfoDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            object TRSN_SessionObject;
            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (TRSN_Session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (TRSN_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                MESStationSession sessionKPNO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (sessionKPNO == null)
                {
                    sessionKPNO = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionKPNO);
                }
                MESStationSession sessionMFRName = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (sessionMFRName == null)
                {
                    sessionMFRName = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionMFRName);
                }
                MESStationSession sessionDateCode = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (sessionDateCode == null)
                {
                    sessionDateCode = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionDateCode);
                }
                MESStationSession sessionLotCode = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (sessionLotCode == null)
                {
                    sessionLotCode = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionLotCode);
                }

                TRSN_SessionObject = TRSN_Session.Value;

                //獲取TR_SN 欄位數據
                Dictionary<string, DataRow> dd = (Dictionary<string, DataRow>)TRSN_SessionObject;
                DataRow tr;
                dd.TryGetValue("R_TR_SN", out tr);
                if (tr != null)
                {
                    sessionKPNO.Value = tr["CUST_KP_NO"].ToString();
                    sessionMFRName.Value = tr["MFR_KP_NO"].ToString();
                    sessionDateCode.Value = tr["DATE_CODE"].ToString();
                    sessionLotCode.Value = tr["Lot_Code"].ToString();
                    Station.Inputs.Find(input => input.Name == Paras[1].SESSION_TYPE).Value = tr["CUST_KP_NO"].ToString();
                    Station.Inputs.Find(input => input.Name == Paras[2].SESSION_TYPE).Value = tr["MFR_KP_NO"].ToString();
                    Station.Inputs.Find(input => input.Name == Paras[3].SESSION_TYPE).Value = tr["DATE_CODE"].ToString();
                    Station.Inputs.Find(input => input.Name == Paras[4].SESSION_TYPE).Value = tr["Lot_Code"].ToString();
                }
                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        /// <summary>
        /// 根據SN的SKUNO帶出維修LOCATION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LocationFromSNDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            List<string> LocationList = new List<string>();
            string ErrMessage = "";
            OleExec apdb = null;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSn = (SN)SNSession.Value;

            //獲取ALLPART數據
            AP_DLL APDLL = new AP_DLL();
            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Location");
            List<object> ret = I.DataForUse;
            ret.Clear();
            try
            {
                apdb = Station.APDB;

                LocationList = APDLL.GetLocationList(ObjSn.SkuNo, apdb);
                if (LocationList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { }));
                }
                else
                {
                    foreach (object item in LocationList)
                    {
                        ret.Add(item);
                    }
                }

                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {

                }
                throw ex;
            }
        }

        //Add by LLF 2018-02-19 Begin
        public static void PTHTrCodeDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Message = "";
            string StrWO = "";
            string StrCode = "";
            string StationName = string.Empty;
            string StrStationNum = string.Empty;
            OleExec APDB = null;

            T_R_SN Table = new T_R_SN(Station.SFCDB, Station.DBType);

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WOSession.Value.ToString();

            //獲取 TRSN 對象
            MESStationSession StationNumSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StationNumSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrStationNum = StationNumSession.Value.ToString();

            //StationName = Station.Line + Station.StationName + StrStationNum;
            var pStation = Paras.Find(t => t.SESSION_TYPE == "PTHSTATION");



            StrStationNum = StationNumSession.Value.ToString();

            if (pStation == null)
            {
                StationName = Station.Line + Station.StationName + StrStationNum;
            }
            else
            {
                StationName = Station.Line + pStation.VALUE + StrStationNum;
            }
            try
            {
                APDB = Station.APDB;
                StrCode = Table.GetAPPTHTrCode(StrWO, StationName, APDB);


                MESStationSession PTHTrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (PTHTrSnSession == null)
                {
                    PTHTrSnSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(PTHTrSnSession);
                }

                PTHTrSnSession.Value = StrCode;
                Station.AddMessage("MES00000001", new string[] { PTHTrSnSession.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        //Add by LLF 2018-02-19 End

        /// <summary>
        /// 加載輸入的字符串到指定的 MESStationSession
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">      
        /// </param>
        public static void GetWaitShipToData(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                var res = Station.SFCDB.ORM
                    .Queryable<R_TO_HEAD, R_TO_DETAIL, R_DN_STATUS>((rth, rtd, rds) =>
                        rth.TO_NO == rtd.TO_NO && rtd.DN_NO == rds.DN_NO && rds.DN_FLAG == "0")
                    .OrderBy((rth) => rth.TO_CREATETIME, OrderByType.Desc)
                    .GroupBy((rth, rtd, rds) => new { rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME, rds.DN_NO })
                    .Select((rth, rtd, rds) => new { rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME, rds.DN_NO }).ToList();
                //.Select("rth.to_no,wm_concat(rds.dn_no),rth.plan_startime,rth.plan_endtime,rth.to_createtime").ToList();

                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == "TO_LIST");
                s.Value = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        /// <summary>
        /// 加載待Deliver_Check數據
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">      
        /// </param>
        public static void GetWaitDeliver_CheckData(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            { 
                var res = Station.SFCDB.ORM
                .Queryable<R_SN_DELIVER_INFO>()
                .Where(t => t.CHECK_FLAG == "1" && t.NEXT_STATION == "DELIVER_CHECK" && t.VALID_FLAG == "1")                    
                .OrderBy((t) => t.CHECK_TIME, OrderByType.Desc)                
                .Select((t) => new { t.ORDERNO })               
                .ToList()
                ;

                var outlist = res.Select((t) => new { t.ORDERNO}).Distinct();
                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == "ORDER_LIST");
                s.Value = outlist;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }




        /// <summary>
        /// 加載輸入的字符串到指定的 MESStationSession
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">      
        /// </param>
        public static void GetCarrierSnLinkData(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                var res = Station.SFCDB.ORM
                    .Queryable<R_CARRIER_LINK>()
                    .Select(it => new { it.SN, it.CARRIERNO, it.EDITTIME, it.EDITBY })
                    .OrderBy(it => it.EDITTIME, OrderByType.Desc)
                    .ToList();

                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == "TO_LIST");
                s.Value = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// 虛擬工單顯示待掃描實條碼數量
        /// add by hgb 2019.05.30
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ShowWaitScanRealSnQty(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
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

                if (WO.Substring(0, 6) == "009999" || WO.Substring(0, 6) == "002163" || WO.Substring(0, 6) == "002164" || WO.Substring(0, 6) == "002159" || WO.Substring(0, 6) == "002159")
                {
                    goto over_allparts_check;//不檢查ALLPART
                }

                AP_DLL ApDll = new AP_DLL();
                string result = ApDll.ShowWaitScanRealSnQty(Station.StationName, Station.IP, WO, Station.SFCDB, Station.APDB);

                if (result.Length > 0)
                {
                    Station.AddMessage("MSGCODE20190412091220", new string[] { result }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
                }

            over_allparts_check:;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }

        /// <summary>
        /// 根據工單加載Allpart工單及機種在Allpart的配置信息[mes4.r_wo_base,mes1.c_product_config,mes4.r_pcba_link,mes1.c_station_kp]
        /// 將查詢到的結果存入Dictionary<string_Datarow> 中, key為表名如:" r_wo_base "
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetAPInfoByWODataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            #region 獲取WO對象
            MESStationSession sWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            #endregion

            #region 新建APConfig對象
            MESStationSession sAPConfig = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sAPConfig == null)
            {
                sAPConfig = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sAPConfig);
            }
            #endregion

            string strSku = "";
            string strVer = "";
            SKU skuObj = new SKU();
            OleExec APDB = null;
            List<DataRow> apWOList = new List<DataRow>();
            List<DataRow> configList = new List<DataRow>();
            List<DataRow> pcbaLinkList = new List<DataRow>();
            List<DataRow> kpConfigList = new List<DataRow>();
            Dictionary<string, List<DataRow>> APInfo = new Dictionary<string, List<DataRow>>();

            try
            {
                strSku = ((WorkOrder)sWO.Value).SkuNO;
                strVer = ((WorkOrder)sWO.Value).SKU_VER;
                APDB = Station.APDB;
                AP_DLL APDLL = new AP_DLL();

                #region Mes4.R_Wo_Base
                DataRow apWO = APDLL.GETSkuVerByWoFromAP(sWO.Value.ToString(), APDB);
                if (apWO == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "WO:" + sWO.Value.ToString(), "MES4.R_WO_BASE" }));
                }
                if (strSku != apWO["P_NO"].ToString())
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141141", new string[] { "SFC", "ALLPART" }));
                }
                //DCN的機種版本邏輯比較奇葩，1位數要+'0'，3位數要截取右邊2位數
                if (strVer != apWO["P_VERSION"].ToString() && strVer + "0" != apWO["P_VERSION"].ToString() && strVer.Substring(1) != apWO["P_VERSION"].ToString())
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141356", new string[] { "SFC", "ALLPART" }));
                }
                apWOList.Add(apWO);
                APInfo.Add("R_WO_BASE", apWOList);
                #endregion

                #region Mes1.C_Product_Config
                configList = APDLL.GetCProductConfigBySkuAndVer(strSku, strVer, APDB);
                if (configList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + strSku + ",VER:" + strVer, "MES1.C_PRODUCT_CONFIG" }));
                }
                APInfo.Add("C_PRODUCT_CONFIG", configList);
                #endregion

                #region Mes4.R_Pcba_Link
                pcbaLinkList = APDLL.R_PCBA_LINK_GetBYSku(strSku, APDB);
                if (pcbaLinkList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + strSku, "MES4.R_PCBA_LINK" }));
                }
                APInfo.Add("R_PCBA_LINK", pcbaLinkList);
                #endregion

                #region Mes1.C_Station_Kp Left Join Mes1.C_Replace_Kp
                kpConfigList = APDLL.GetAllpartKpConfigBySkuAndVer(strSku, strVer, APDB);
                if (pcbaLinkList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200505153601", new string[] { strSku, strVer }));
                }

                //SFC與ALLPART料號比對，預留

                APInfo.Add("C_STATION_KP", kpConfigList);
                #endregion

                sAPConfig.Value = APInfo;
                Station.AddMessage("MES00000001", new string[] { sWO.Value.ToString() }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 從輸入的數據（PKG_ID,二維碼）轉為ALLPART條碼
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputDataConvertToTrSn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string strTRSN = "";
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            strTRSN = Input.Value.ToString();
            string strOutputData = strTRSN;

            try
            {
                //獲取ALLPART數據
                AP_DLL APDLL = new AP_DLL();
                strOutputData = APDLL.GetR2DSNRelation(strTRSN, Station.APDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Input.Value = strOutputData;
            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {
                TRSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSN_Session);
            }
            TRSN_Session.ResetInput = Input;
            TRSN_Session.InputValue = strOutputData;
        }
    }
}
