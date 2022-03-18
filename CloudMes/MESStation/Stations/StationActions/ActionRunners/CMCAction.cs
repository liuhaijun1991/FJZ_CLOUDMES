using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject;
using MESDataObject.Common;
using System.Data;
using MESDBHelper;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using MESPubLab.Json;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class CMCAction
    {
        /// <summary>
        /// insert ICT 測試Data
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InsertICTTestDataAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            MESStationSession TestDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TestDataSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            Config.CMC.DataItem dataItem = (Config.CMC.DataItem)TestDataSession.Value;

            //T_ict_detail _Ict_Detail= new T_ict_detail(DB, DB_TYPE_ENUM.Oracle);
            //Row_ict_detail R_Ict_Detail = (Row_ict_detail)_Ict_Detail.NewRow();
            T_ICT_HEADER t_ICT = new T_ICT_HEADER(DB, DB_TYPE_ENUM.Oracle);
            Row_ICT_HEADER row_ICT_HEADER = (Row_ICT_HEADER)t_ICT.NewRow();

            row_ICT_HEADER.SYSSERIALNO = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "SN");
            row_ICT_HEADER.ICTDATETIME = DateTime.Now.ToString("yyyyMMddHHmmss");
            row_ICT_HEADER.MODEL = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "SKUNO");
            row_ICT_HEADER.ICT_NO = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "STATION_NO");
            row_ICT_HEADER.ICTDATE = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            row_ICT_HEADER.ICTTIME = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
            row_ICT_HEADER.RESULT = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "STATUS") == "PASS" ? "P" : "F";
            row_ICT_HEADER.RETEST = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "STATUS") == "PASS" ? "0" : "1";
            row_ICT_HEADER.WORKSCHEDULE = "1";
            row_ICT_HEADER.OP = "";
            row_ICT_HEADER.DETAILRESULT = dataItem.DATA;
            row_ICT_HEADER.STATION_TYPE = Station.StationName;
            DB.ExecSQL(row_ICT_HEADER.GetInsertString(DB_TYPE_ENUM.Oracle));
            if (Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "STATUS") != "PASS")
            {
                T_ict_detail t_Ict_Detail = new T_ict_detail(DB, DB_TYPE_ENUM.Oracle);
                Row_ict_detail row_Ict_Detail = (Row_ict_detail)t_Ict_Detail.NewRow();
                string var_ictheader = "Board: " + Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "SKUNO");
                var_ictheader += "      " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "|";
                var_ictheader += "Test NO:" + Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "STATION_NO") + "|";
                var_ictheader += "Board NO:" + Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "SN") + "|";
                var_ictheader += "Error Code:" + Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "FAILCODE");

                row_Ict_Detail.SYSSERIALNO = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "SN");
                row_Ict_Detail.ICTDATETIME = DateTime.Now.ToString("yyyyMMddHHmmss");
                row_Ict_Detail.ICTHEADER = var_ictheader;
                row_Ict_Detail.ICTDETAIL = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "FAILCODE");
                row_Ict_Detail.ICTDATE = DateTime.Now.ToString("yyyyMMdd");
                row_Ict_Detail.STATION_TYPE = Station.StationName;

                DB.ExecSQL(row_Ict_Detail.GetInsertString(DB_TYPE_ENUM.Oracle));
            }


        }


        /// <summary>
        /// insert ICT Fail IN REPAIR
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ICTFailByFailCodeAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionFailLocation = null;
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionFailCode == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession TestDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TestDataSession == null)
            {
                TestDataSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(TestDataSession);
            }

            MESStation.Config.CMC.DataItem dataItem = (MESStation.Config.CMC.DataItem)TestDataSession.Value;
            if (dataItem.keyValuePairs.Where(s => s.Key == "STATUS").ToArray()[0].Value == "FAIL")
            {


                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_SN_STATION_DETAIL rSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_REPAIR_FAILCODE tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();
                Row_R_SN row_r_sn = (Row_R_SN)t_r_sn.NewRow();
                C_ERROR_CODE failCodeObject = (C_ERROR_CODE)sessionFailCode.Value;
                string result = "";
                string repairMainID = "";

                //更新R_SN REPAIR_FAILED_FLAG=’1’
                R_SN r_sn = t_r_sn.GetDetailBySN(sessionSN.Value.ToString(), Station.SFCDB);
                row_r_sn = (Row_R_SN)t_r_sn.GetObjByID(r_sn.ID, Station.SFCDB);
                row_r_sn.REPAIR_FAILED_FLAG = "1";
                row_r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                row_r_sn.EDIT_TIME = Station.GetDBDateTime();
                result = (Station.SFCDB).ExecSQL(row_r_sn.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (!(Convert.ToInt32(result) > 0))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                }

                //新增一筆FAIL記錄到R_SN_STATION_DETAIL
                result = rSnStationDetail.AddDetailToRSnStationDetail(rSnStationDetail.GetNewID(Station.BU, Station.SFCDB),
                    row_r_sn.GetDataObject(), Station.Line, Station.StationName, Station.StationName, Station.SFCDB);
                if (!(Convert.ToInt32(result) > 0))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "STATION_DETAIL" }));
                }

                List<R_REPAIR_MAIN> repairList = tRepairMain.GetRepairListSNAndStation(Station.SFCDB, r_sn.SN, Station.StationName, "0");
                if (repairList == null || repairList.Count == 0)
                {
                    //新增一筆到R_REPAIR_MAIN
                    repairMainID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                    rRepairMain.ID = repairMainID;
                    rRepairMain.SN = r_sn.SN;
                    rRepairMain.WORKORDERNO = r_sn.WORKORDERNO;
                    rRepairMain.SKUNO = r_sn.SKUNO;
                    rRepairMain.FAIL_LINE = Station.Line;
                    rRepairMain.FAIL_STATION = Station.StationName;
                    rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                    rRepairMain.FAIL_TIME = Station.GetDBDateTime();
                    rRepairMain.CREATE_TIME = Station.GetDBDateTime();
                    rRepairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                    rRepairMain.EDIT_TIME = Station.GetDBDateTime();
                    rRepairMain.CLOSED_FLAG = "0";
                    result = (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (!(Convert.ToInt32(result) > 0))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "REPAIR_MAIN" }));
                    }
                }
                else if (repairList.Count == 1)
                {
                    repairMainID = repairList[0].ID;
                }
                else
                {
                    // SN:{0}在工站{1}有多筆未維修記錄
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219085857", new string[] { r_sn.SN, Station.StationName }));
                }

                if (tRepairFailCode.FailCodeIsExist(Station.SFCDB, r_sn.SN, repairMainID, failCodeObject.ERROR_CODE))
                {
                    // SN:{0}在工站{1}已經錄入不良代碼{2},請不要重複錄入
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219090050", new string[] { r_sn.SN, Station.StationName, failCodeObject.ERROR_CODE }));
                }
                //新增一筆到R_REPAIR_FAILCODE         
                Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
                rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
                rRepairFailCode.REPAIR_MAIN_ID = repairMainID;
                rRepairFailCode.SN = r_sn.SN;
                rRepairFailCode.FAIL_CODE = failCodeObject.ERROR_CODE;
                rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
                rRepairFailCode.FAIL_TIME = Station.GetDBDateTime();
                rRepairFailCode.FAIL_CATEGORY = failCodeObject.ERROR_CATEGORY;
                rRepairFailCode.FAIL_LOCATION = sessionFailLocation == null ? "" : sessionFailLocation.Value.ToString();
                rRepairFailCode.FAIL_PROCESS = "";
                rRepairFailCode.DESCRIPTION = failCodeObject.ERROR_CODE;
                rRepairFailCode.REPAIR_FLAG = "0";
                rRepairFailCode.CREATE_TIME = Station.GetDBDateTime();
                rRepairFailCode.EDIT_EMP = Station.LoginUser.EMP_NO;
                rRepairFailCode.EDIT_TIME = Station.GetDBDateTime();
                result = (Station.SFCDB).ExecSQL(rRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (!(Convert.ToInt32(result) > 0))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "FAILCODE" }));
                }
                Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
                Station.NextInput = Station.Inputs[0];
            }
        }

        public static void TestOneDataReturn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession Skusession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Skusession == null || Skusession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string SNStr = Input.Value.ToString();
            SKU c_SKU = (SKU)Skusession.Value;
            string ReturnMessage = "1>>SERIALNO=" + SNStr + ",SKUNO=" + c_SKU.SkuNo + "#OK,UNIT STATUS IS VALID";
            MESPubLab.MESStation.MESReturnView.Station.StationMessage msg = new MESPubLab.MESStation.MESReturnView.Station.StationMessage();
            msg.Message = ReturnMessage;
            msg.State = StationMessageState.CMCMessage;
            Station.StationMessages.Add(msg);
        }

        public static void TesttWODataReturn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionStation_NO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation_NO == null || sessionStation_NO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession FCSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FCSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null || StatusSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }


            string SNStr = Input.Value.ToString();
            string ReturnMessage = "";
            C_ERROR_CODE code = (C_ERROR_CODE)FCSession.Value;
            if (StatusSession.Value.ToString() == "PASS")
            {
                ReturnMessage = "2>>SERIALNO=" + SNStr + "#OK,UNIT  IS PASS! ";
            }
            else
            {
                ReturnMessage = "2>>SERIALNO=" + SNStr + "#FAIL,FAILCODE=" + code.ERROR_CODE + "!";
            }
            JsonSave.DeleteFromDB(sessionStation_NO.Value.ToString(), SNStr, Station.SFCDB);
            MESPubLab.MESStation.MESReturnView.Station.StationMessage msg = new MESPubLab.MESStation.MESReturnView.Station.StationMessage();
            msg.Message = ReturnMessage;
            msg.State = StationMessageState.CMCMessage;
            Station.StationMessages.Add(msg);
        }

        public static void CM_TestData_Analysis(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionStation_NO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation_NO == null || sessionStation_NO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSKU == null || sessionSKU.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession custsnSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (custsnSession == null)
            {
                custsnSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(custsnSession);
            }

            MESStationSession SN2Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (SN2Session == null)
            {
                SN2Session = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(SN2Session);
            }

            MESStationSession SITESession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (SITESession == null)
            {
                SITESession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY };
                Station.StationSession.Add(SITESession);
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, SessionKey = Paras[6].SESSION_KEY };
                Station.StationSession.Add(StatusSession);
            }

            MESStationSession FCSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (FCSession == null)
            {
                FCSession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, SessionKey = Paras[7].SESSION_KEY };
                Station.StationSession.Add(FCSession);
            }

            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            if (DeviceSession == null)
            {
                DeviceSession = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, SessionKey = Paras[8].SESSION_KEY };
                Station.StationSession.Add(DeviceSession);
            }

            StatusSession.Value = null;
            FCSession.Value = null;
            SKU sKU = (SKU)sessionSKU.Value;
            WorkOrder wo = (WorkOrder)sessionWO.Value;
            string CHASSISSN = "", PCBASN = "", CODENAME = "", HFCMAC = "", MTAMAC = "", ETHERNETMAC = "", ATOM = "",
                BOOT_ROM = "", HW_REV = "", SW_REV = "", SID = "", WANMAC = "", WAN2MAC = "", WLAN24MAC = "",
                WLAN50MAC = "", L2SD = "", SSID24 = "", SSID50 = "", NETWORK_KEY = "", USERNAME = "", NETWORK_NAME = "",
                WPS_PIN = "", PASSWORD = "", CUST_REV = "", MOCAMAC = "", PSK = "", ADMINPW = "", TEST_LOG = "", SITE = "",
                TEST_VERSION = "", SN2 = "", RGLANMAC = "", RGWANMAC = "";
            string SNStr = Input.Value.ToString();

            MESStation.Config.CMC.TwoDataItem TDataItem =
                    JsonSave.GetFromDB<MESStation.Config.CMC.TwoDataItem>(sessionStation_NO.Value.ToString(), "2", SNStr, Station.SFCDB);
            T_R_CUSTSN_T tcust = new T_R_CUSTSN_T(Station.SFCDB, DB_TYPE_ENUM.Oracle);


            // T_R_SN_KP Key = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_KP KEYPART = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            T_C_SERIES sERIES = new T_C_SERIES(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_LOCK sN_LOCK = new T_R_SN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_SEQNO sEQNO = new T_C_SEQNO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            CODENAME = sERIES.GetDetailById(Station.SFCDB, sKU.CSeriesId).SERIES_NAME;
            R_CUSTSN_T r_CUSTSN_T = tcust.GetTestDataBySn(SNStr, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_CUSTSN_T tempcust = new R_CUSTSN_T();
            tempcust.SERIAL_NUMBER = SNStr;
            tempcust.MO_NUMBER = sKU.SkuNo;
            tempcust.GROUP_NAME = Station.StationName;
            CHASSISSN = TDataItem.Data.Find(t => t.IndexOf("CHASSISSN=") >= 0);
            if (CHASSISSN == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN1 != "")
                {
                    CHASSISSN = r_CUSTSN_T.SSN1;
                }
                else
                {
                    CHASSISSN = "";
                }
            }
            else
            {
                {
                    CHASSISSN = TDataItem.Data[0].Split('=')[1];
                    if (CHASSISSN == "")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "CHASSISSN= " }));
                    }
                }
            }
            tempcust.SSN1 = CHASSISSN;

            PCBASN = TDataItem.Data.Find(t => t.IndexOf("ISN=") >= 0);
            if (PCBASN == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN2 != "")
                {
                    PCBASN = r_CUSTSN_T.SSN2;
                }
                else
                {
                    PCBASN = "";
                }
            }
            else
            {
                PCBASN = TDataItem.Data[1].Split('=')[1];
                if (PCBASN == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "ISN= " }));
                }
                string PCBASN1 = KEYPART.GetPcbaSn(SNStr, Station.SFCDB).VALUE;
                if (PCBASN1 != PCBASN && CODENAME.IndexOf("TM1602") >= 0)
                {
                    sN_LOCK.AddNewLock(Station.BU, sEQNO.GetLotno("LOCKNO", Station.SFCDB), "S/N", SNStr, wo.WorkorderNo, Station.StationName, "LOCKED BY SN: 第二步TE傳的ISN: " + PCBASN1 + "與前段SN: " + PCBASN + "不一致,測試站為:" + sessionStation_NO.Value.ToString(), Station.User.EMP_NO, Station.SFCDB);
                }
            }
            tempcust.SSN2 = PCBASN;

            HFCMAC = TDataItem.Data.Find(t => t.IndexOf("HFCMAC=") >= 0);
            if (HFCMAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC1 != "")
                {
                    HFCMAC = r_CUSTSN_T.MAC1;
                }
                else
                {
                    HFCMAC = "";
                }
            }
            else
            {
                HFCMAC = TDataItem.Data[2].Split('=')[1];
                if (HFCMAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "HFCMAC= " }));
                }
            }
            tempcust.MAC1 = HFCMAC;

            MTAMAC = TDataItem.Data.Find(t => t.IndexOf("MTAMAC=") >= 0);
            if (MTAMAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC2 != "")
                {
                    MTAMAC = r_CUSTSN_T.MAC2;
                }
                else
                {
                    MTAMAC = "";
                }
            }
            else
            {
                MTAMAC = TDataItem.Data[3].Split('=')[1];
                if (MTAMAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "MTAMAC= " }));
                }
            }
            tempcust.MAC2 = MTAMAC;

            ETHERNETMAC = TDataItem.Data.Find(t => t.IndexOf("ETHERNETMAC=") >= 0);
            if (ETHERNETMAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC5 != "")
                {
                    ETHERNETMAC = r_CUSTSN_T.MAC5;
                }
                else
                {
                    ETHERNETMAC = "";
                }
            }
            else
            {
                ETHERNETMAC = TDataItem.Data[4].Split('=')[1];
                if (ETHERNETMAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "ETHERNETMAC= " }));
                }
            }
            tempcust.MAC5 = ETHERNETMAC;

            ATOM = TDataItem.Data.Find(t => t.IndexOf("ATOM=") >= 0);
            if (ATOM == "")
            {
                if (CODENAME.IndexOf("TG") != 0)
                {
                    if (r_CUSTSN_T != null && r_CUSTSN_T.MAC10 != "")
                    {
                        ATOM = r_CUSTSN_T.MAC10;
                    }
                    else
                    {
                        ATOM = "";
                    }
                }
                else
                {
                    if (r_CUSTSN_T != null && r_CUSTSN_T.MAC16 != "")
                    {
                        ATOM = r_CUSTSN_T.MAC16;
                    }
                    else
                    {
                        ATOM = "";
                    }
                }
            }
            else
            {
                ATOM = TDataItem.Data[5].Split('=')[1];
                if (ATOM == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "ATOM= " }));
                }
            }

            if (CODENAME.IndexOf("TG") != 0)
            {
                tempcust.MAC10 = ATOM;
            }
            else
            {
                tempcust.MAC16 = ATOM;
            }

            BOOT_ROM = TDataItem.Data.Find(t => t.IndexOf("BOOT_ROM=") >= 0);
            if (BOOT_ROM == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN10 != "")
                {
                    BOOT_ROM = r_CUSTSN_T.SSN10;
                }
                else
                {
                    BOOT_ROM = "";
                }
            }
            else
            {
                BOOT_ROM = TDataItem.Data[6].Split('=')[1];
                if (BOOT_ROM == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "BOOT_ROM= " }));
                }
            }
            tempcust.SSN10 = BOOT_ROM;

            HW_REV = TDataItem.Data.Find(t => t.IndexOf("HW_REV=") >= 0);
            if (HW_REV == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN8 != "")
                {
                    HW_REV = r_CUSTSN_T.SSN8;
                }
                else
                {
                    HW_REV = "";
                }
            }
            else
            {
                HW_REV = TDataItem.Data[7].Split('=')[1];
                if (HW_REV == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "HW_REV= " }));
                }
            }
            tempcust.SSN8 = HW_REV;

            SW_REV = TDataItem.Data.Find(t => t.IndexOf("SW_REV=") >= 0);
            if (SW_REV == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN9 != "")
                {
                    SW_REV = r_CUSTSN_T.SSN9;
                }
                else
                {
                    SW_REV = "";
                }
            }
            else
            {
                SW_REV = TDataItem.Data[8].Split('=')[1];
                if (SW_REV == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "SW_REV= " }));
                }
            }
            tempcust.SSN9 = SW_REV;

            SID = TDataItem.Data.Find(t => t.IndexOf("SID=") >= 0);
            if (SID == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN7 != "")
                {
                    SID = r_CUSTSN_T.SSN7;
                }
                else
                {
                    SID = "";
                }
            }
            else
            {
                SID = TDataItem.Data[9].Split('=')[1];
                if (SID == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "SID= " }));
                }
            }
            tempcust.SSN7 = SID;

            WANMAC = TDataItem.Data.Find(t => t.IndexOf("WANMAC=") >= 0);
            if (WANMAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC6 != "")
                {
                    WANMAC = r_CUSTSN_T.MAC6;
                }
                else
                {
                    WANMAC = "";
                }
            }
            else
            {
                WANMAC = TDataItem.Data[10].Split('=')[1];
                if (WANMAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "WANMAC= " }));
                }
            }
            tempcust.MAC6 = WANMAC;

            WAN2MAC = TDataItem.Data.Find(t => t.IndexOf("WAN2MAC=") >= 0);
            if (WAN2MAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC7 != "")
                {
                    WAN2MAC = r_CUSTSN_T.MAC7;
                }
                else
                {
                    WAN2MAC = "";
                }
            }
            else
            {
                WAN2MAC = TDataItem.Data[11].Split('=')[1];
                if (WAN2MAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "WAN2MAC= " }));
                }
            }
            tempcust.MAC7 = WAN2MAC;

            WLAN24MAC = TDataItem.Data.Find(t => t.IndexOf("WLAN24MAC=") >= 0);
            if (WLAN24MAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC8 != "")
                {
                    WLAN24MAC = r_CUSTSN_T.MAC8;
                }
                else
                {
                    WLAN24MAC = "";
                }
            }
            else
            {
                WLAN24MAC = TDataItem.Data[12].Split('=')[1];
                if (WLAN24MAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "WLAN24MAC= " }));
                }
            }
            tempcust.MAC8 = WLAN24MAC;

            WLAN50MAC = TDataItem.Data.Find(t => t.IndexOf("WLAN50MAC=") >= 0);
            if (WLAN50MAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC9 != "")
                {
                    WLAN50MAC = r_CUSTSN_T.MAC9;
                }
                else
                {
                    WLAN50MAC = "";
                }
            }
            else
            {
                WLAN50MAC = TDataItem.Data[13].Split('=')[1];
                if (WLAN50MAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "WLAN50MAC= " }));
                }
            }
            tempcust.MAC9 = WLAN50MAC;

            L2SD = TDataItem.Data.Find(t => t.IndexOf("L2SD=") >= 0);
            if (L2SD == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN12 != "")
                {
                    L2SD = r_CUSTSN_T.SSN12;
                }
                else
                {
                    L2SD = "";
                }
            }
            else
            {
                L2SD = TDataItem.Data[14].Split('=')[1];
                if (L2SD == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "L2SD= " }));
                }
            }
            tempcust.SSN12 = L2SD;

            SSID24 = TDataItem.Data.Find(t => t.IndexOf("SSID24=") >= 0);
            if (SSID24 == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN11 != "")
                {
                    SSID24 = r_CUSTSN_T.SSN11;
                }
                else
                {
                    SSID24 = "";
                }
            }
            else
            {
                SSID24 = TDataItem.Data[15].Split('=')[1];
                if (SSID24 == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "SSID24= " }));
                }
            }
            tempcust.SSN11 = SSID24;

            SSID50 = TDataItem.Data.Find(t => t.IndexOf("SSID50=") >= 0);
            if (SSID50 == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC12 != "")
                {
                    SSID50 = r_CUSTSN_T.MAC12;
                }
                else
                {
                    SSID50 = "";
                }
            }
            else
            {
                SSID50 = TDataItem.Data[16].Split('=')[1];
                if (SSID50 == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "SSID50= " }));
                }
            }
            tempcust.MAC12 = SSID50;

            NETWORK_KEY = TDataItem.Data.Find(t => t.IndexOf("NETWORK_KEY=") >= 0);
            if (NETWORK_KEY == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC14 != "")
                {
                    NETWORK_KEY = r_CUSTSN_T.MAC14;
                }
                else
                {
                    NETWORK_KEY = "";
                }
            }
            else
            {
                NETWORK_KEY = TDataItem.Data[17].Split('=')[1];
                if (NETWORK_KEY == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "NETWORK_KEY= " }));
                }
            }
            tempcust.MAC14 = NETWORK_KEY;

            USERNAME = TDataItem.Data.Find(t => t.IndexOf("USERNAME=") >= 0);
            if (USERNAME == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN13 != "")
                {
                    USERNAME = r_CUSTSN_T.SSN13;
                }
                else
                {
                    USERNAME = "";
                }
            }
            else
            {
                USERNAME = TDataItem.Data[18].Split('=')[1];
                if (USERNAME == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "USERNAME= " }));
                }
            }
            tempcust.SSN13 = USERNAME;

            NETWORK_NAME = TDataItem.Data.Find(t => t.IndexOf("NETWORK_NAME=") >= 0);
            if (NETWORK_NAME == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC13 != "")
                {
                    NETWORK_NAME = r_CUSTSN_T.MAC13;
                }
                else
                {
                    NETWORK_NAME = "";
                }
            }
            else
            {
                NETWORK_NAME = TDataItem.Data[19].Split('=')[1];
                if (NETWORK_NAME == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "NETWORK_NAME= " }));
                }
            }
            tempcust.MAC13 = NETWORK_NAME;

            WPS_PIN = TDataItem.Data.Find(t => t.IndexOf("WPS_PIN=") >= 0);
            if (WPS_PIN == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC3 != "")
                {
                    WPS_PIN = r_CUSTSN_T.MAC3;
                }
                else
                {
                    WPS_PIN = "";
                }
            }
            else
            {
                WPS_PIN = TDataItem.Data[20].Split('=')[1];
                if (WPS_PIN == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "WPS_PIN= " }));
                }
            }
            tempcust.MAC3 = WPS_PIN;

            PASSWORD = TDataItem.Data.Find(t => t.IndexOf("PASSWORD=") >= 0);
            if (PASSWORD == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC15 != "")
                {
                    PASSWORD = r_CUSTSN_T.MAC15;
                }
                else
                {
                    PASSWORD = "";
                }
            }
            else
            {
                PASSWORD = TDataItem.Data[21].Split('=')[1];
                if (PASSWORD == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "PASSWORD= " }));
                }
            }
            tempcust.MAC15 = PASSWORD;

            CUST_REV = TDataItem.Data.Find(t => t.IndexOf("CUST_REV=") >= 0);
            if (CUST_REV == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC16 != "")
                {
                    CUST_REV = r_CUSTSN_T.MAC16;
                }
                else
                {
                    CUST_REV = "";
                }
            }
            else
            {
                CUST_REV = TDataItem.Data[22].Split('=')[1];
                if (CUST_REV == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "CUST_REV= " }));
                }
            }
            tempcust.MAC16 = CUST_REV;

            MOCAMAC = TDataItem.Data.Find(t => t.IndexOf("MOCAMAC=") >= 0);
            if (MOCAMAC == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.MAC17 != "")
                {
                    MOCAMAC = r_CUSTSN_T.MAC17;
                }
                else
                {
                    MOCAMAC = "";
                }
            }
            else
            {
                MOCAMAC = TDataItem.Data[23].Split('=')[1];
                if (MOCAMAC == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "MOCAMAC= " }));
                }
            }
            tempcust.MAC17 = MOCAMAC;

            PSK = TDataItem.Data.Find(t => t.IndexOf("PSK=") >= 0);
            if (PSK == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN3 != "")
                {
                    PSK = r_CUSTSN_T.SSN3;
                }
                else
                {
                    PSK = "";
                }
            }
            else
            {
                PSK = TDataItem.Data[24].Split('=')[1];
                if (PSK == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "PSK= " }));
                }
            }
            tempcust.SSN3 = PSK;

            ADMINPW = TDataItem.Data.Find(t => t.IndexOf("ADMINPW=") >= 0);
            if (ADMINPW == "")
            {
                if (r_CUSTSN_T != null && r_CUSTSN_T.SSN4 != "")
                {
                    ADMINPW = r_CUSTSN_T.SSN4;
                }
                else
                {
                    ADMINPW = "";
                }
            }
            else
            {
                ADMINPW = TDataItem.Data[25].Split('=')[1];
                if (ADMINPW == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "ADMINPW= " }));
                }
            }
            tempcust.SSN4 = ADMINPW;

            TEST_LOG = TDataItem.Data.Find(t => t.IndexOf("TEST_LOG=") >= 0);
            if (TEST_LOG != "")
            {
                TEST_LOG = TDataItem.Data[26].Split('=')[1];
                if (TEST_LOG == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000001", new string[] { "TEST_LOG= " }));
                }
            }
            tempcust.MAC11 = TEST_LOG;

            //机台编号
            SITE = TDataItem.Data.Find(t => t.IndexOf("SITE=") >= 0);
            if (SITE != "")
            {
                SITE = TDataItem.Data[27].Split('=')[1];
            }

            TEST_VERSION = TDataItem.Data.Find(t => t.IndexOf("TEST_VERSION=") >= 0);
            if (TEST_VERSION != "")
            {
                TEST_VERSION = TDataItem.Data[28].Split('=')[1];
            }
            tempcust.MAC19 = TEST_VERSION;

            SN2 = TDataItem.Data.Find(t => t.IndexOf("SN2=") >= 0);
            if (SN2 != "")
            {
                SN2 = TDataItem.Data[29].Split('=')[1];
            }

            RGLANMAC = TDataItem.Data.Find(t => t.IndexOf("RGLANMAC=") >= 0);
            if (RGLANMAC != "")
            {
                RGLANMAC = TDataItem.Data[30].Split('=')[1];
            }
            tempcust.SSN5 = RGLANMAC;

            RGWANMAC = TDataItem.Data.Find(t => t.IndexOf("RGWANMAC=") >= 0);
            if (RGWANMAC != "")
            {
                RGWANMAC = TDataItem.Data[31].Split('=')[1];
            }
            tempcust.SSN6 = RGWANMAC;
            //if (TDataItem.Status.ToUpper() == "PASS")
            //{
            //    if (r_CUSTSN_T.SSN1 != "")
            //    {
            //        if (tcust.CheckSSNorMAC(SNStr, "SSN1", CHASSISSN, Station.SFCDB) == 1)
            //        {
            //            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000002", new string[] { "CHASSISSN:" + CHASSISSN }));
            //        }
            //        if (r_CUSTSN_T.SSN1 != CHASSISSN)
            //        {
            //            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000003", new string[] {  CHASSISSN , SNStr }));
            //        }
            //    }
            //}
            tempcust.SSN22 = TDataItem.TestDataString;
            tempcust.IN_STATION_TIME = Station.GetDBDateTime();

            SN2Session.Value = SN2;
            SITESession.Value = SITE;
            if (TDataItem.Status.ToUpper() != "PASS")
            {

                T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE failCodeObject = t_c_error_code.GetByErrorCode(TDataItem.Status.ToUpper(), Station.SFCDB);

                if (failCodeObject == null)
                {
                    failCodeObject = new C_ERROR_CODE();
                    failCodeObject.ID = t_c_error_code.GetNewID(Station.BU, Station.SFCDB);
                    failCodeObject.ERROR_CODE = TDataItem.Status.ToUpper();
                    failCodeObject.ENGLISH_DESC = TDataItem.Status.ToUpper();
                    failCodeObject.CHINESE_DESC = TDataItem.Status.ToUpper();
                    failCodeObject.EDIT_EMP = Station.LoginUser.EMP_NO;
                    failCodeObject.EDIT_TIME = Station.GetDBDateTime();
                    failCodeObject.ERROR_CATEGORY = "";
                    t_c_error_code.AddNewErrorCode(failCodeObject, Station.SFCDB);
                    FCSession.Value = failCodeObject;
                }
                else
                {
                    FCSession.Value = failCodeObject;
                }
                StatusSession.Value = "FAIL";
                tempcust.SSN21 = "FAIL";
            }
            else
            {
                StatusSession.Value = TDataItem.Status;
                tempcust.SSN21 = TDataItem.Status;
            }
            custsnSession.Value = tempcust;

            MESStation.Config.CMC.OneDataItem oneDataItem =
                      JsonSave.GetFromDB<MESStation.Config.CMC.OneDataItem>(sessionStation_NO.Value.ToString(), "1", SNStr, Station.SFCDB);

            string Device = oneDataItem.ONE[oneDataItem.ONE.Count - 2];
            DeviceSession.Value = Device;
        }

        public static void InsertR_Custsn_T(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionStation_NO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation_NO == null || sessionStation_NO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession custsnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (custsnSession == null)
            {
                custsnSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(custsnSession);
            }
            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            string SNStr = Input.Value.ToString();

            T_R_CUSTSN_T tcust = new T_R_CUSTSN_T(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_CUSTSN_T r_CUSTSN_T = tcust.GetTestDataBySn(SNStr, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_CUSTSN_T RRcust = (Row_R_CUSTSN_T)tcust.NewRow();

            T_R_CUSTSN_DETAIL_T tcustdetail = new T_R_CUSTSN_DETAIL_T(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_CUSTSN_DETAIL_T RRcustdetail = (Row_R_CUSTSN_DETAIL_T)tcustdetail.NewRow();



            //MESStation.Config.CMC.TwoDataItem TDataItem =
            //        JsonSave.GetFromDB<MESStation.Config.CMC.TwoDataItem>(sessionStation_NO.Value.ToString(), "2", SNStr, Station.SFCDB);
            R_CUSTSN_T tempcust = (R_CUSTSN_T)custsnSession.Value;


            //實例化反射對象
            Type RRtype = RRcust.GetType();
            System.Reflection.PropertyInfo[] field = RRtype.GetProperties();
            Type ctype = tempcust.GetType();
            System.Reflection.PropertyInfo[] field1 = ctype.GetProperties();
            Type RRtypedetail = RRcustdetail.GetType();
            System.Reflection.PropertyInfo[] fielddetail = RRtypedetail.GetProperties();

            try
            {
                //利用反射將測試解析數據對Row_R_CUSTSN_T對象賦值
                foreach (System.Reflection.PropertyInfo pro1 in field1)
                {
                    foreach (System.Reflection.PropertyInfo pro in field)
                    {
                        if (pro1.Name == pro.Name)//對比兩個對象屬性名一致則進行賦值
                        {
                            pro.SetValue(RRcust, pro1.GetValue(tempcust));
                        }
                    }
                }

                //利用反射將測試解析數據對Row_R_CUSTSN_DETAIL_T對象賦值
                foreach (System.Reflection.PropertyInfo pro1 in field1)
                {
                    foreach (System.Reflection.PropertyInfo prode in fielddetail)
                    {
                        if (pro1.Name == prode.Name)//對比兩個對象屬性名一致則進行賦值
                        {
                            prode.SetValue(RRcustdetail, pro1.GetValue(tempcust));
                        }
                    }
                }

                if (r_CUSTSN_T != null && StatusSession.Value.ToString() == "PASS") //Insert R_Custsn_T
                {
                    Station.SFCDB.ExecSQL(RRcust.GetUpdateString(0));//如果數據庫內有數據則Update R_Custsn_T
                }
                else if (r_CUSTSN_T == null && StatusSession.Value.ToString() == "PASS")
                {
                    Station.SFCDB.ExecSQL(RRcust.GetInsertString(0));//反之Insert R_Custsn_T
                }

                Station.SFCDB.ExecSQL(RRcustdetail.GetInsertString(0)); //Insert R_Custsn_Detail_T
            }
            catch
            {
                //throw new MESReturnMessage("insert C_custsn_t or C_custsn_detail_t  失敗！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110609", new string[] { "insert C_custsn_t or C_custsn_detail_t" }));
            }
            //RRcust.SERIAL_NUMBER = tempcust.SERIAL_NUMBER;
            //RRcust.MO_NUMBER = tempcust.MO_NUMBER;
            //RRcust.GROUP_NAME = tempcust.GROUP_NAME;
            //RRcust.SSN1 = tempcust.SSN1;
            //RRcust.SSN2 = tempcust.SSN2;
            //RRcust.SSN3 = tempcust.SSN3;
            //RRcust.SSN4 = tempcust.SSN4;
            //RRcust.SSN5 = tempcust.SSN5;
            //RRcust.SSN6 = tempcust.SSN6;
            //RRcust.SSN7 = tempcust.SSN7;
            //RRcust.SSN8 = tempcust.SSN8;
            //RRcust.SSN9 = tempcust.SSN9;
            //RRcust.SSN10 = tempcust.SSN10;
            //RRcust.SSN11 = tempcust.SSN11;
            //RRcust.SSN12 = tempcust.SSN12;
            //RRcust.SSN13 = tempcust.SSN13;
            //RRcust.SSN14 = tempcust.SSN14;
            //RRcust.SSN15 = tempcust.SSN15;
            //RRcust.SSN16 = tempcust.SSN16;
            //RRcust.SSN17 = tempcust.SSN17;
            //RRcust.SSN18 = tempcust.SSN18;
            //RRcust.SSN19 = tempcust.SSN19;
            //RRcust.SSN20 = tempcust.SSN20;
            //RRcust.SSN21 = tempcust.SSN21;
            //RRcust.SSN22 = tempcust.SSN22;
            //RRcust.MAC1 = tempcust.MAC1;
            //RRcust.MAC2 = tempcust.MAC2;
            //RRcust.MAC3 = tempcust.MAC3;
            //RRcust.MAC4 = tempcust.MAC4;
            //RRcust.MAC5 = tempcust.MAC5;
            //RRcust.MAC6 = tempcust.MAC6;
            //RRcust.MAC7 = tempcust.MAC7;
            //RRcust.MAC8 = tempcust.MAC8;
            //RRcust.MAC9 = tempcust.MAC9;
            //RRcust.MAC10 = tempcust.MAC10;
            //RRcust.MAC11 = tempcust.MAC11;
            //RRcust.MAC12 = tempcust.MAC12;
        }

        /// <summary>
        /// ForCMC 提供SN基本過站&寫過站記錄&LOADING SN過站流程
        /// SN對象(必須),WO對象(非必須,如果為空則使用SN的WO直接轉)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LinkKeyPartSnAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //獲主板SN對象
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            //取KeyPart對象
            MESStationSession sessionSnKpScanList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            //抓取KeyPart掃描List,如果需要掃KeyPart則檢查KeyPart有沒有掃完如果掃完則執行過站,否則不執行PASS過站Action.
            List<Row_R_SN_KP> kpScanList = new List<Row_R_SN_KP>();
            if (sessionSnKpScanList != null)
            {
                if (sessionSnKpScanList.Value != null)
                {
                    kpScanList = (List<Row_R_SN_KP>)sessionSnKpScanList.Value;
                }
            }

            if (kpScanList.Count > 0)
            {
                string r_sn_id = "";
                LogicObject.SN sn = new SN();
                if (sessionSN.Value is SN)
                {
                    r_sn_id = ((SN)sessionSN.Value).baseSN.ID;
                }
                else
                {
                    r_sn_id = (sn.LoadSN(sessionSN.Value.ToString(), Station.SFCDB)).ID;
                }

                T_R_SN_KP TRKP = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_SN_KP> r_sn_kp_list = TRKP.GetKPRecordBySnIDStation(r_sn_id, Station.StationName, Station.SFCDB);
                //保險起見檢查一次掃描的KeyPart和LOADING預寫進R_SN_KP的記錄對不對得上.
                var checkkp = (from m in r_sn_kp_list
                               from n in kpScanList
                               where m.SN == n.SN && m.STATION == n.STATION && m.ITEMSEQ == n.ITEMSEQ && m.SCANSEQ == n.SCANSEQ && m.DETAILSEQ == n.DETAILSEQ && m.SCANTYPE == n.SCANTYPE && m.REGEX == n.REGEX
                               select new { ID = m.ID, ITEMSEQ = m.ITEMSEQ, SCANSEQ = m.SCANSEQ, DETAILSEQ = m.DETAILSEQ, VALUE = n.VALUE, MPN = n.MPN, PARTNO = n.PARTNO }).ToList();
                if (checkkp.Count() == r_sn_kp_list.Count)
                {
                    for (int i = 0; i < checkkp.Count; i++)
                    {
                        Row_R_SN_KP item = (Row_R_SN_KP)TRKP.GetObjByID(checkkp[i].ID, Station.SFCDB);
                        if (item.ITEMSEQ == checkkp[i].ITEMSEQ && item.SCANSEQ == checkkp[i].SCANSEQ && item.DETAILSEQ == checkkp[i].DETAILSEQ)
                        {
                            item.VALUE = checkkp[i].VALUE;
                            item.MPN = checkkp[i].MPN;
                            item.PARTNO = checkkp[i].PARTNO;
                            item.EDIT_TIME = DateTime.Now;
                            item.EDIT_EMP = Station.LoginUser.EMP_NO;
                        }
                        Station.SFCDB.ExecSQL(item.GetUpdateString(DB_TYPE_ENUM.Oracle));
                        item.AcceptChange();
                    }
                }
                else
                {
                    //返回異常
                    //throw new MESReturnMessage($@"Update R_SN_KP failed!KeyPartScanQty:{checkkp.Count()}<>KeyPartQty:{r_sn_kp_list.Count} ERROR!");
                    MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115020", new string[] { checkkp.Count().ToString(), r_sn_kp_list.Count.ToString() });

                }
            }
        }

        /// <summary>
        /// ForCMC 提供SN LOADING過站流程
        /// SN對象(必須),WO對象(非必須,如果為空則使用SN的WO直接轉)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLoadingPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //獲主板SN對象
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            //取工單對象
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            WorkOrder objWorkorder = sessionWO.Value as WorkOrder;
            if (objWorkorder == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            //加載FailCode
            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            C_ERROR_CODE failCodeObject = null;
            if (sessionFailCode != null)
                failCodeObject = sessionFailCode.Value as C_ERROR_CODE;

            //STATUS,方便寫良率和UPH使用,以及過站使能
            MESStationSession sessionStatus = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionStatus == null)
            {
                sessionStatus = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[3].VALUE, SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionStatus);
            }

            //用戶掃了ERRORCODE就是掃FAIL,不執行Action.
            if (failCodeObject != null)
            {
                sessionStatus.Value = "FAIL";
            }
            else
            {
                sessionStatus.Value = "PASS";
            }

            if (sessionStatus.Value.ToString().ToUpper() == "PASS")
            {
                Dictionary<string, object> dicNextStation;
                string nextStation = "";
                int result;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                objWorkorder = (WorkOrder)sessionWO.Value;
                string WorkorderNo = objWorkorder.WorkorderNo;
                dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, Station.SFCDB);
                nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();

                #region 寫主表R_SN
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                R_SN r_sn = new R_SN();

                string strSysSN = (string)sessionSN.Value;
                if (string.IsNullOrEmpty(strSysSN))//通常都不會報這個錯,如果報了那就是工站配置錯了.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));

                r_sn.SN = strSysSN;
                r_sn.ID = t_r_sn.GetNewID(Station.BU, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                r_sn.SKUNO = objWorkorder.SkuNO;
                r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                r_sn.PLANT = objWorkorder.PLANT;
                r_sn.ROUTE_ID = objWorkorder.RouteID;
                r_sn.STARTED_FLAG = "1";
                r_sn.START_TIME = Station.GetDBDateTime();
                r_sn.PACKED_FLAG = "0";
                r_sn.COMPLETED_FLAG = "0";
                r_sn.SHIPPED_FLAG = "0";
                r_sn.REPAIR_FAILED_FLAG = "0";
                r_sn.SCRAPED_FLAG = "0";
                r_sn.CURRENT_STATION = Station.StationName;
                r_sn.NEXT_STATION = nextStation;
                r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                r_sn.CUST_PN = objWorkorder.CUST_PN;
                r_sn.VALID_FLAG = "1";
                r_sn.STOCK_STATUS = "0";
                r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                r_sn.EDIT_TIME = Station.GetDBDateTime();
                result = t_r_sn.AddNewSN(r_sn, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + r_sn.SN, "ADD" }));
                }
                #endregion

                #region 寫綁定表R_SN_KP
                T_C_KP_LIST c_kp_list = new T_C_KP_LIST(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (objWorkorder.KP_LIST_ID != null && objWorkorder.KP_LIST_ID.ToString() != "")
                {
                    if (!c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, Station.SFCDB))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { objWorkorder.WorkorderNo + " KP_LIST_ID" }));
                    }
                    SN snObject = new SN();
                    snObject.InsertR_SN_KP(objWorkorder, r_sn, Station.SFCDB, Station, DB_TYPE_ENUM.Oracle);
                }
                else
                {
                    if (c_kp_list.GetListIDBySkuno(objWorkorder.SkuNO, Station.SFCDB).Count != 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181101091946", new string[] { objWorkorder.SkuNO, objWorkorder.WorkorderNo }));
                    }
                }
                #endregion

                #region 更新工單投入數量
                result = Convert.ToInt32(t_r_wo_base.AddCountToWo(objWorkorder.WorkorderNo, 1, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + objWorkorder.WorkorderNo, "UPDATE" }));
                }
                #endregion

                #region 寫過站記錄表R_SN_STATION_DETAIL
                t_r_sn.RecordPassStationDetail(r_sn, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
                #endregion
            }
        }

        /// <summary>
        /// ForCMC,掃描步驟判斷,掃描流程控制,判斷掃入的SN該由哪個輸入框處理.
        /// 只支持基本過站,掃描進維修
        /// 輸入框項目定義：公共输入框->工单输入->FAILCODE->主板SN掃描->KEYPART條碼掃描->+B條碼一槍LOADING
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnScanSILoadingAssyPandingAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            Dictionary<string, string> dic = (Dictionary<string, string>)Input.Value;
            string strinput = dic["SN"];
            string station_no = dic["Station_No"];
            string modelType = string.Empty;

            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            //取StationNo
            MESStationSession sessionStationNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStationNo == null)
            {
                sessionStationNo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = station_no };
                Station.StationSession.Add(sessionStationNo);
            }
            //取ModelType
            MESStationSession sessionModelType = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionModelType == null)
            {
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
                modelType = sessionModelType.Value != null ? (string)sessionModelType.Value : "";

            T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_AP_TEMP ap = tap.GetMaxByStation_no(Station.SFCDB, station_no);

            T_C_ERROR_CODE tcec = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            bool IsErrorCode = tcec.CheckErrorCodeByErrorCode(Station.SFCDB, strinput);

            string strStationName = tap.GetSfcStation(Station.SFCDB, station_no);
            if (ap.DATA4 == "STATION_NAME")//掃Undo后的第一槍,如果是SILOADING/SMTLOADING則第一槍必須是掃工單,非SILOADING/SMTLOADING第一槍有可能是掃ERRORCODE或者掃主板SN.
            {

                if (strStationName.EndsWith("LOADING"))//LOADING工站初始第一槍必須掃工單.
                {
                    //指定由工單INPUT框處理掃描數據W/O
                    //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[1].DisplayName, strinput, Station.Inputs[3].DisplayName);
                    tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[1].DisplayName, strinput, Station.Inputs[3].DisplayName, 0, "", "W/O", "");
                }
                else//普通條碼一槍過站
                {
                    if (IsErrorCode)
                    {
                        //指定由ERROR CODE INPUT框處理掃描數據ERROR CODE
                        //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[2].DisplayName, strinput, Station.Inputs[3].DisplayName);
                        tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[2].DisplayName, strinput, Station.Inputs[3].DisplayName, 0, "", "ERROR CODE", "");
                    }
                    else
                    {
                        //指定由主板SN INPUT框處理掃描數據S/N
                        //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[3].DisplayName);
                        tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[3].DisplayName, 0, "", "S/N", "");
                    }
                }
            }
            else if (ap.DATA4 == Station.Inputs[1].DisplayName)//如果上一槍掃的是工單,那麼這一槍在SILOADING/SMTLOADING工站應該掃主板SN開始Loading,在非SILOADING/SMTLOADING工站掃的有可能是ERRORCODE或者掃主板SN過站.
            {
                if (strStationName.EndsWith("LOADING"))
                {
                    tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[4].DisplayName, 0, "", "S/N", "");
                }
                else //非Loading工站一槍過站或Fail
                {
                    if (IsErrorCode)
                    {
                        //指定由ERROR CODE INPUT框處理掃描數據ERROR CODE
                        //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[2].DisplayName, strinput, Station.Inputs[3].DisplayName);
                        tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[2].DisplayName, strinput, Station.Inputs[3].DisplayName, 0, "", "ERROR CODE", "");
                    }
                    else
                    {
                        //指定由主板SN INPUT框處理掃描數據S/N
                        //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[3].DisplayName);
                        tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[3].DisplayName, 0, "", "S/N", "");
                    }
                }
            }
            else if (ap.DATA4 == Station.Inputs[2].DisplayName) //如果上一槍掃的是ERROR_CODE,那麼這一槍必須是主板SN.這裡需要改改 20200717
            {
                //指定由主板SN INPUT框處理掃描數據
                //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[3].DisplayName);
                tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[3].DisplayName, 0, "", "S/N", "");
            }
            else if (ap.DATA4 == Station.Inputs[3].DisplayName) //如果上一槍掃的是主板SN,那麼這一槍必然是掃KeyPart.(掃完主板條碼都沒過站請記錄,說明肯定有KeyPart沒掃完)
            {
                //指定由KeyPartSn INPUT框處理掃描數據
                //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[4].DisplayName, strinput, Station.Inputs[4].DisplayName);
                tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[4].DisplayName, strinput, Station.Inputs[4].DisplayName, 0, "KEYPART", "", "");
            }
            else if (ap.DATA4 == Station.Inputs[4].DisplayName) //如果上一槍掃的是KeyPartSN,KeyPart沒掃完繼續掃KeyPart.
            {
                //指定由KeyPartSn INPUT框處理掃描數據
                //tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[4].DisplayName, strinput, Station.Inputs[4].DisplayName);
                tap.InsertApTemp(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[4].DisplayName, strinput, Station.Inputs[4].DisplayName, 0, "KEYPART", "", "");
            }
            else //不知道用戶掃了什麼,默認用戶掃的是主板SN,但要在NextScan加個Ex區分
            {
                //UnException.
                tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[3].DisplayName, strinput, Station.Inputs[3].DisplayName + "_Ex");
            }
            sessionStationNo.Value = station_no;
        }

        public static void SnScanSILoadingAssyCheckAndCallInput(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strModelType = "", strStationNo = "", rsnid = ""; bool runType = false; int CallInput = 999; string InputString = ""; string strSystemSn = "";
            if (Paras.Count != 6)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //取StationNo
            MESStationSession sessionStationNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStationNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (sessionStationNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            strStationNo = sessionStationNo.Value.ToString();
            //取主板SN
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else if (sessionSn.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (sessionSn.Value is SN)
                {
                    strSystemSn = ((SN)sessionSn.Value).SerialNo;
                }
                else
                {
                    strSystemSn = sessionSn.Value as String;
                    if (string.IsNullOrEmpty(strSystemSn))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
            }

            //取ModelType
            MESStationSession sessionModelType = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionModelType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                strModelType = sessionModelType.Value as string;
            }

            //SN KeyPart清單,必須設定.
            List<Row_R_SN_KP> kpScanList = new List<Row_R_SN_KP>();
            MESStationSession sessionKpScanList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionKpScanList == null)
            {
                sessionKpScanList = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, InputValue = Input.Value.ToString(), ResetInput = Input };
                Station.StationSession.Add(sessionKpScanList);
            }
            else if (sessionKpScanList.Value != null)
            {
                kpScanList = sessionKpScanList.Value as List<Row_R_SN_KP>;
            }

            //檢查由CheckKp方法緩存的KeyPartCheckList裡面的主板條碼是否當前的主板SN是否一致,如果不一致則報錯通知用戶重新掃描
            if (kpScanList.Where(p => p.SN != strSystemSn).Any())
            {
                //throw new MESReturnMessage("KeyPart掃描失敗!原因:發現條碼與KeyPart記錄不符,請掃Undo重新掃描!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115358"));                
            }

            string CheckType = Paras[4].VALUE as String;
            if (string.IsNullOrEmpty(CheckType))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            C_ERROR_CODE failcode = null;
            if (sessionFailCode != null)
            {
                failcode = sessionFailCode.Value as C_ERROR_CODE;
            }

            //放在KPSN Input最後面用於檢查條碼是否需要綁KeyPart.不需要綁KeyPart則直接調用過站Input過站.
            if (CheckType == "KEYPART")
            {
                //如果不需要綁KeyPart則直接走過站Input
                if (kpScanList.Count == 0)
                {
                    runType = true;
                    CallInput = 5;
                }
                //如果有KeyPart且KeyPart已掃完則走主板條碼Input重新檢查主板條碼狀態.
                if (!kpScanList.Where(p => p.VALUE == "" || p.VALUE == null).Any())
                {
                    if (strModelType.IndexOf("022") >= 0 && Station.StationName.EndsWith("LOADING"))
                    {
                        //加B條碼LOADING不用再重複檢查主板狀態,直接過站.
                        //runType = true;
                        //CallInput = 5;
                        if (kpScanList.Count > 1)
                        {
                            //重新檢查主板SN的狀態.
                            runType = true;
                            CallInput = 3;
                        }
                        else
                        {
                            //加B條碼在SILOADING不需要綁其它條碼,則直接過站.
                            runType = true;
                            CallInput = 5;
                        }
                    }
                    else
                    {
                        //重新檢查主板SN的狀態.
                        runType = true;
                        CallInput = 3;
                    }
                }
            }
            //放在主板條碼Input後面,用於檢查
            if (CheckType == "PASSSTATION")
            {
                if (failcode == null)
                {
                    if (Station.StationName.EndsWith("LOADING"))
                    {
                        //ModeType配置022,屬於一槍過站類型
                        if (strModelType.IndexOf("022") >= 0)
                        {
                            if (kpScanList.Count == 0)
                            {
                                //配了022ModelType的機種,SILOADING的KeyPart必須設定!!!
                                throw new MESReturnMessage("ModeType022,SILOADING keypart item cannot be null!");
                            }
                            if (kpScanList.Where(p => p.VALUE == "" || p.VALUE == null).Any())
                            {
                                //加B條碼沒綁完KeyPart則走KeyPart Input,綁PCBA SN
                                runType = true;
                                CallInput = 4;
                            }
                            else
                            {
                                //加B條碼Loading還綁其它條碼的情況
                                runType = true;
                                CallInput = 5;
                            }
                        }
                        else
                        {
                            if (kpScanList.Count > 0)//沒配022,標準的掃一槍後段板掃一槍前段板Link過站.
                            {
                                if (!kpScanList.Where(p => p.VALUE == "" || p.VALUE == null).Any())
                                {
                                    runType = true;
                                    CallInput = 5;
                                }
                            }
                            else//針對非廠內生產板,在Loading工站不用綁KeyPart,直接Load進系統. 
                            {
                                runType = true;
                                CallInput = 5;
                            }
                        }
                    }
                    else
                    {
                        if (kpScanList.Count == 0)
                        {
                            /* 已經注釋掉,在前面加載完KeyPart進kpScanList后就不用再重新加載檢查了.
                            //沒做CheckKP,則檢查條碼在當前站需不要掃KeyPart.
                            R_SN rsn = new R_SN();
                            SN sn = new SN();
                            rsn = sn.LoadSN(strSystemSn, Station.SFCDB);
                            rsnid = rsn.ID;
                            T_R_SN_KP TRKP = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(rsnid, Station.StationName, Station.SFCDB).Where(p => p.VALUE == "" || p.VALUE == null).ToList();
                            if (snkp.Count == 0)
                            {
                                //當前工站不需要掃KeyPart,則直接調用過站Input過站.
                                runType = true;
                                CallInput = 5;
                            }
                            */
                            //當前工站不需要掃KeyPart,則直接調用過站Input過站.
                            runType = true;
                            CallInput = 5;
                        }
                        else
                        {
                            if (!kpScanList.Where(p => p.VALUE == "" || p.VALUE == null).Any())
                            {
                                //KeyPart掃完了,調用過站Input
                                runType = true;
                                CallInput = 5;
                            }
                        }
                    }
                }
                else
                {
                    runType = true;
                    CallInput = 6;
                }
            }

            if (runType)
            {
                try
                {
                    T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    R_AP_TEMP ap = tap.GetMaxByStation_no(Station.SFCDB, strStationNo);
                    string strSysSn = tap.GetSysserialNo(Station.SFCDB, strStationNo);
                    //Call KeyPart Input R_AP_TEMP的記錄必須要寫,KeyPart會依據R_AP_TEMP的記錄檢查掃描當前掃描的是哪一槍. 
                    //3:SN狀態檢查INPUT 4:KEYPART INPUT 5:過站INPUT
                    switch (CallInput)
                    {
                        case 3: tap.InsertApTemp(Station.SFCDB, strStationNo, ap.DATA3 + 1, Station.Inputs[CallInput].DisplayName, strSysSn, Station.Inputs[CallInput].DisplayName, 0, "CALLINPUT", "S/N CHECK", ""); Station.Inputs[CallInput].Value = strSysSn; break;
                        case 5: tap.InsertApTemp(Station.SFCDB, strStationNo, ap.DATA3 + 1, Station.Inputs[CallInput].DisplayName, strSysSn, Station.Inputs[CallInput].DisplayName, 0, "CALLINPUT", "S/N PASS", ""); Station.Inputs[CallInput].Value = strSysSn; break;
                        case 4: tap.InsertApTemp(Station.SFCDB, strStationNo, ap.DATA3 + 1, Station.Inputs[CallInput].DisplayName, strSysSn, Station.Inputs[CallInput].DisplayName, 0, "KEYPART", "", ""); Station.Inputs[CallInput].Value = strSysSn; break;
                        case 6: tap.InsertApTemp(Station.SFCDB, strStationNo, ap.DATA3 + 1, Station.Inputs[CallInput].DisplayName, strSysSn, Station.Inputs[CallInput].DisplayName, 0, "CALLINPUT", "S/N FAIL", ""); Station.Inputs[CallInput].Value = strSysSn; break;
                    }
                    Station.Inputs[CallInput].Run();
                }
                catch (Exception ex)
                {
                    //tap.FailDeleteAp(Station.SFCDB, strStationNo);
                    List<StationMessage> msglist = Station.StationMessages.Where(p => p.State == StationMessageState.CMCMessage).ToList();
                    foreach (StationMessage msg in msglist)
                    {
                        Station.StationMessages.Remove(msg);
                    }
                    throw new MESReturnMessage(ex.Message);
                }
            }
        }

        /// <summary>
        /// For CMC,條碼過站&條碼掃描回傳結果到CMC顯示和清除Session
        /// 這個方法只執行基本的CMC回傳顯示操作和過站處理R_AP_TEMP表記錄,如果想要處理
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnScanSILoadingAssyReturnMessage(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strMessage = "", strStatus = "", strStationName = "", strStationNo = "", strSql = "";

            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionStationNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStationNo == null)
            {
                //无法获取到 {0} 的数据，请检查工站配置！
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                strStationNo = (string)sessionStationNo.Value;
                if (string.IsNullOrEmpty(strStationNo))
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            MESStationSession sessionStatus = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionStatus == null)
            {
                strStatus = "";
            }
            else
            {
                strStatus = (string)sessionStatus.Value;
            }

            List<Row_R_SN_KP> KpScanList = null;
            if (Paras.Count > 4)
            {
                MESStationSession sessionKpScanList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (sessionKpScanList != null)
                {
                    KpScanList = sessionKpScanList.Value as List<Row_R_SN_KP>;
                }
            }

            C_ERROR_CODE failcode = null;
            if (Paras.Count > 5)
            {
                MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (sessionFailCode != null)
                {
                    failcode = sessionFailCode.Value as C_ERROR_CODE;
                }
            }

            T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_AP_TEMP> listaptemp = tap.GetRecordByStationNo(Station.SFCDB, strStationNo);
            strStationName = listaptemp.Where(p => p.DATA4 == "STATION_NAME").Select(p => p.DATA5).FirstOrDefault();

            #region 回傳作業員下一槍應該掃什麼.
            //條碼PASS/FAIL過站回傳：OK UNIT AUDIT PASSED,NEXTEVENT [NextEvent]
            //條碼KEYPART掃描回傳：OK KEYPART SCAN OK,NEXT SCAN [NextKeyPartScanType]
            //Input掃描完成提示：OK [CURRENTINPUT] SCAN OK,NEXT SCAN [NEXTINPUT]
            if (strStatus == "PASS") //SN PASS回傳
            {
                string strSystemSn = "";
                if (sessionSn == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else if (sessionSn.Value is SN)
                {
                    strSystemSn = ((SN)sessionSn.Value).SerialNo;
                }
                else
                {
                    strSystemSn = sessionSn.Value as String;
                }
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                R_SN r_sn = t_r_sn.LoadData(strSystemSn, Station.SFCDB);
                strMessage += "OK UNIT AUDIT PASSED,NEXTEVENT [" + r_sn.NEXT_STATION + "]";
            }
            else if (strStatus == "FAIL")//SN FAIL回傳
            {
                strMessage += "OK,SN AUDIT FAIL";
            }
            else //多槍掃描回傳.
            {
                R_AP_TEMP maxscan = listaptemp[listaptemp.Count - 1];
                if (KpScanList != null && failcode == null)
                {
                    if (KpScanList != null)
                    {
                        if (KpScanList.Count == 1)
                        {
                            strMessage += "OK SN SCAN OK,NEXT SCAN [" + KpScanList[0].KP_NAME + "]";
                        }
                        else if (KpScanList.Count > 1)
                        {
                            Row_R_SN_KP nextscan = KpScanList.Where(p => p.VALUE == null || p.VALUE == "").OrderBy(p => p.ITEMSEQ).ThenBy(p => p.SCANSEQ).FirstOrDefault();
                            strMessage += "OK KEYPART SCAN OK,NEXT SCAN [" + nextscan.KP_NAME + "]";
                        }
                        else //No KeyPart Scan
                        {
                            strMessage += "OK KEYPART SCAN OK,NEXT SCAN [" + maxscan.DATA10 + "]";
                        }
                    }
                }
                else
                {
                    strMessage += "OK " + maxscan.DATA4 + " SCAN OK,NEXT SCAN [" + maxscan.DATA6 + "]";
                }
            }
            #endregion

            #region 條碼PASS/FAIL過站清除R_AP_TEMP的數據
            if (strStatus == "PASS")//PASS過站清理數據
            {
                if (strStationName.EndsWith("LOADING"))
                {
                    //Loading工站清除除工單工站基礎數據(EMP/LINE_NAME/STATION_NAME/<WORKORDERNO>)之外的記錄.
                    strSql = "Delete from r_ap_temp where data2='" + strStationNo + "' and data3>4";
                }
                else
                {
                    //非Loading工站清除除工站基礎數據(EMP/LINE_NAME/STATION_NAME)之外的記錄.
                    strSql = "Delete from r_ap_temp where data2='" + strStationNo + "' and data3>3";
                }
                Station.SFCDB.ExecuteNonQuery(strSql, CommandType.Text, null);
            }
            else if (strStatus == "FAIL")//FAIL過站清理數據
            {
                if (strStationName.EndsWith("LOADING"))
                {
                    //Loading工站清除除工單工站基礎數據(EMP/LINE_NAME/STATION_NAME/<WORKORDERNO>)之外的記錄.
                    strSql = "Delete from r_ap_temp where data2='" + strStationNo + "' and data3>4";
                }
                else
                {
                    //非Loading工站清除除工站基礎數據(EMP/LINE_NAME/STATION_NAME)之外的記錄.
                    strSql = "Delete from r_ap_temp where data2='" + strStationNo + "' and data3>3";
                }
                Station.SFCDB.ExecuteNonQuery(strSql, CommandType.Text, null);
            }
            else
            {
            }
            #endregion

            //回傳信息給CMC顯示,StationMessageState類型必須是StationMessageState.CMCMessage
            List<StationMessage> msglist = Station.StationMessages.Where(p => p.State == StationMessageState.CMCMessage).ToList();
            foreach (StationMessage msg in msglist)
            {
                Station.StationMessages.Remove(msg);
            }
            Station.AddMessage("MSGCODE20200707084548", new string[] { strMessage }, StationMessageState.CMCMessage);
        }

        /// <summary>
        /// ForCMC 提供SN基本過站&寫過站記錄&LOADING SN過站流程
        /// SN對象(必須),WO對象(非必須,如果為空則使用SN的WO直接轉),此方法已經停用,并拆分.
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNPassStationBaseAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder objWorkorder = new WorkOrder();
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM sfcdbType = Station.DBType;
            C_ERROR_CODE failCodeObject = null;
            string strR_SN_ID = string.Empty;
            string DeviceName = string.Empty;
            string ModelType = string.Empty;

            if (Paras.Count != 7)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "6", Paras.Count.ToString() }));
            }

            //獲主板SN對象
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            //取工單對象
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            objWorkorder = sessionWO.Value as WorkOrder;
            if (objWorkorder == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            //取ModelType
            MESStationSession sessionModelType = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionModelType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            ModelType = sessionModelType.Value.ToString();

            //取KeyPart對象
            MESStationSession sessionSnKpScanList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);

            //取DeviceName
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else //Add by LLF 2018-02-05
            {
                DeviceName = Station.StationName;
            }

            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionFailCode != null)
                failCodeObject = sessionFailCode.Value as C_ERROR_CODE;

            //STATUS,方便寫良率和UPH使用,以及過站使能
            MESStationSession sessionStatus = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (sessionStatus == null)
            {
                sessionStatus = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[6].VALUE, SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionStatus);
            }
            else if (sessionStatus.Value == null)
            {
                sessionStatus.Value = "PASS";
            }

            //抓取KeyPart掃描List,如果需要掃KeyPart則檢查KeyPart有沒有掃完如果掃完則執行過站,否則不執行PASS過站Action.
            List<Row_R_SN_KP> kpScanList = new List<Row_R_SN_KP>();
            if (sessionSnKpScanList == null)
            {
                sessionStatus.Value = "PASS";
            }
            else
            {
                if (sessionSnKpScanList.Value == null)
                {
                    sessionStatus.Value = "PASS";
                }
                else
                {
                    kpScanList = (List<Row_R_SN_KP>)sessionSnKpScanList.Value;
                    if (!kpScanList.Where(p => p.VALUE == "").Any())
                    {
                        sessionStatus.Value = "PASS";
                    }
                    else
                        sessionStatus.Value = "";
                }
            }

            //用戶掃了ERRORCODE就是掃FAIL,不執行本PASS過站Action.
            if (failCodeObject != null)
            {
                sessionStatus.Value = "FAIL";
            }

            if (sessionStatus.Value.ToString().ToUpper() == "PASS")
            {
                if (Station.StationName.EndsWith("LOADING"))//Loading過站,寫R_SN,R_SN_KP,R_SN_STATION_DETAIL
                {
                    Dictionary<string, object> dicNextStation;
                    string nextStation = "";
                    int result;
                    T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, sfcdbType);
                    T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, sfcdbType);
                    objWorkorder = (WorkOrder)sessionWO.Value;
                    string WorkorderNo = objWorkorder.WorkorderNo;
                    dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, sfcdb);
                    nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();

                    #region 寫主表R_SN
                    T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                    R_SN r_sn = new R_SN();

                    if (string.IsNullOrEmpty(sessionSN.Value.ToString()))//通常都不會報這個錯,如果報了那就是工站配置錯了.
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));

                    //if (ModelType.IndexOf("022") >0)//配置ModelTYPE022的對條碼加B作為後段主板條碼.
                    //    r_sn.SN = sessionSN.Value.ToString()+"B";
                    //else
                    r_sn.SN = sessionSN.Value.ToString();

                    r_sn.ID = t_r_sn.GetNewID(Station.BU, sfcdb, sfcdbType);
                    r_sn.SKUNO = objWorkorder.SkuNO;
                    r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                    r_sn.PLANT = objWorkorder.PLANT;
                    r_sn.ROUTE_ID = objWorkorder.RouteID;
                    r_sn.STARTED_FLAG = "1";
                    r_sn.START_TIME = Station.GetDBDateTime();
                    r_sn.PACKED_FLAG = "0";
                    r_sn.COMPLETED_FLAG = "0";
                    r_sn.SHIPPED_FLAG = "0";
                    r_sn.REPAIR_FAILED_FLAG = "0";
                    r_sn.CURRENT_STATION = Station.StationName;
                    r_sn.NEXT_STATION = nextStation;
                    r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                    r_sn.CUST_PN = objWorkorder.CUST_PN;
                    r_sn.VALID_FLAG = "1";
                    r_sn.STOCK_STATUS = "0";
                    r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                    r_sn.EDIT_TIME = Station.GetDBDateTime();
                    result = t_r_sn.AddNewSN(r_sn, sfcdb);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + r_sn.SN, "ADD" }));
                    }
                    #endregion

                    #region 寫綁定表R_SN_KP
                    T_C_KP_LIST c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
                    if (objWorkorder.KP_LIST_ID != null && objWorkorder.KP_LIST_ID.ToString() != "")
                    {
                        if (!c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, sfcdb))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { objWorkorder.WorkorderNo + " KP_LIST_ID" }));
                        }
                        SN snObject = new SN();
                        snObject.InsertR_SN_KP(objWorkorder, r_sn, sfcdb, Station, sfcdbType);
                    }
                    else
                    {
                        if (c_kp_list.GetListIDBySkuno(objWorkorder.SkuNO, sfcdb).Count != 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181101091946", new string[] { objWorkorder.SkuNO, objWorkorder.WorkorderNo }));
                        }
                    }
                    #endregion

                    #region 更新工單投入數量
                    result = Convert.ToInt32(t_r_wo_base.AddCountToWo(objWorkorder.WorkorderNo, 1, Station.SFCDB));
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + objWorkorder.WorkorderNo, "UPDATE" }));
                    }
                    #endregion

                    #region 寫過站記錄表R_SN_STATION_DETAIL
                    t_r_sn.RecordPassStationDetail(r_sn, Station.Line, Station.StationName, Station.StationName, Station.BU, sfcdb);
                    #endregion

                    strR_SN_ID = r_sn.ID;
                }
                else//掃描SN PASS標準過站,更新R_SN狀態
                {
                    LogicObject.SN sn = new SN();
                    if (sessionSN.Value is SN)
                        sn = (SN)sessionSN.Value;
                    else
                        sn.LoadSN(sessionSN.Value.ToString(), Station.SFCDB);

                    strR_SN_ID = sn.ID;
                    //普通過站更新主表R_SN狀態和寫過站記錄R_SN_STATION_DETAIL
                    T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                    table.PassStation(sn.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, sessionStatus.Value.ToString(), Station.LoginUser.EMP_NO, Station.SFCDB);
                    Station.AddMessage("MES00000063", new string[] { sn.SerialNo }, StationMessageState.Pass);
                }

                #region 有KeyPart的更新綁定關係表R_SN_KP
                if (kpScanList.Count > 0)
                {
                    T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
                    List<R_SN_KP> r_sn_kp_list = TRKP.GetKPRecordBySnIDStation(strR_SN_ID, Station.StationName, sfcdb);
                    //保險起見檢查一次掃描的KeyPart和LOADING預寫進R_SN_KP的記錄對不對得上.
                    var checkkp = (from m in r_sn_kp_list
                                   from n in kpScanList
                                   where m.SN == n.SN && m.STATION == n.STATION && m.ITEMSEQ == n.ITEMSEQ && m.SCANSEQ == n.SCANSEQ && m.DETAILSEQ == n.DETAILSEQ && m.SCANTYPE == n.SCANTYPE && m.REGEX == n.REGEX
                                   select new { ID = m.ID, ITEMSEQ = m.ITEMSEQ, SCANSEQ = m.SCANSEQ, DETAILSEQ = m.DETAILSEQ, VALUE = n.VALUE, MPN = n.MPN, PARTNO = n.PARTNO }).ToList();
                    if (checkkp.Count() == r_sn_kp_list.Count)
                    {
                        for (int i = 0; i < checkkp.Count; i++)
                        {
                            Row_R_SN_KP item = (Row_R_SN_KP)TRKP.GetObjByID(checkkp[i].ID, sfcdb);
                            if (item.ITEMSEQ == checkkp[i].ITEMSEQ && item.SCANSEQ == checkkp[i].SCANSEQ && item.DETAILSEQ == checkkp[i].DETAILSEQ)
                            {
                                item.VALUE = checkkp[i].VALUE;
                                item.MPN = checkkp[i].MPN;
                                item.PARTNO = checkkp[i].PARTNO;
                                item.EDIT_TIME = DateTime.Now;
                                item.EDIT_EMP = Station.LoginUser.EMP_NO;
                            }
                            sfcdb.ExecSQL(item.GetUpdateString(DB_TYPE_ENUM.Oracle));
                            item.AcceptChange();
                        }
                    }
                    else
                    {
                        //返回異常 Update R_SN_KP failed!KeyPartScanQty:{0}<>KeyPartQty:{1} ERROR!
                        //throw new MESReturnMessage($@"Update R_SN_KP failed!KeyPartScanQty:{checkkp.Count()}<>KeyPartQty:{r_sn_kp_list.Count} ERROR!");
                        MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115020", new string[] { checkkp.Count().ToString(), r_sn_kp_list.Count.ToString() });
                    }
                }
                #endregion
            }
        }
    }
}
