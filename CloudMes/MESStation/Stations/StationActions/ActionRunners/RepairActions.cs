using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Data.OleDb;
using MESDataObject.Common;
using static MESDataObject.Common.EnumExtensions;
using MESDataObject.Module.Juniper;

//using System.Transactions;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class RepairActions
    {
       
        //產品維修CheckIn Action
        public static void SNInRepairAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSendEmp = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSendEmp == null || sessionSendEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionReceiveEmp = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionReceiveEmp == null || sessionReceiveEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            string repairNo = "";
            string repairWorkorderNo = "";
            string repairSkuNo = "";
            string repairCurrentStation = "";

            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                repairNo = snObject.SerialNo;
                repairWorkorderNo = snObject.WorkorderNo;
                repairSkuNo = snObject.SkuNo;
                repairCurrentStation = snObject.CurrentStation;
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                repairNo = panelObject.PanelNo;
                panelObject.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                repairWorkorderNo = panelObject.PanelSnList.FirstOrDefault().WORKORDERNO;
                repairSkuNo = panelObject.PanelSnList.FirstOrDefault().SKUNO;
                repairCurrentStation = panelObject.PanelSnList.FirstOrDefault().CURRENT_STATION;
            }
            else
            {
                throw new Exception("Input Type Error");
            }

            T_R_REPAIR_MAIN rRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            List<R_REPAIR_MAIN> RepariMainList = rRepairMain.GetRepairMainBySN(Station.SFCDB, repairNo);
            R_REPAIR_MAIN rMain = RepariMainList.Where(r => r.CLOSED_FLAG == "0").FirstOrDefault();  // Find(r => r.CLOSED_FLAG == "0");
            if (rMain != null)
            {
                T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
                Row_R_REPAIR_TRANSFER rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.NewRow();
                rowTransfer.ID = rTransfer.GetNewID(Station.BU, Station.SFCDB);
                rowTransfer.REPAIR_MAIN_ID = rMain.ID;
                rowTransfer.IN_SEND_EMP = sessionSendEmp.Value.ToString();
                rowTransfer.IN_RECEIVE_EMP = sessionReceiveEmp.Value.ToString();
                rowTransfer.IN_TIME = Station.GetDBDateTime();
                rowTransfer.SN = repairNo;
                rowTransfer.LINE_NAME = Station.Line;
                rowTransfer.STATION_NAME = repairCurrentStation;
                rowTransfer.WORKORDERNO = repairWorkorderNo;
                rowTransfer.SKUNO = repairSkuNo;
                rowTransfer.CLOSED_FLAG = "0";
                rowTransfer.CREATE_TIME = Station.GetDBDateTime();
                rowTransfer.DESCRIPTION = "";
                rowTransfer.EDIT_TIME = Station.GetDBDateTime();
                rowTransfer.EDIT_EMP = sessionReceiveEmp.Value.ToString();
                string strRet = (Station.SFCDB).ExecSQL(rowTransfer.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000037", new string[] { "INSET R_REPAIR_TRANSFER" }, StationMessageState.Pass);
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { repairNo, "CLOSED" }));
            }
        }

        //產品維修CheckOut Action
        public static void SNOutRepairAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSendEmp = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSendEmp == null || sessionSendEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionReceiveEmp = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionReceiveEmp == null || sessionReceiveEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            string repairNo = "";
            List<R_SN> listSNObject = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);

            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                repairNo = snObject.SerialNo;
                listSNObject.Add(t_r_sn.LoadData(snObject.SerialNo, Station.SFCDB));
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                repairNo = panelObject.PanelNo;
                panelObject.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                listSNObject = panelObject.PanelSnList;
            }
            else
            {
                throw new Exception("Input Type Error");
            }

            T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_TRANSFER rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.NewRow();


            List<R_REPAIR_TRANSFER> transferList = rTransfer.GetLastRepairedBySN(repairNo, Station.SFCDB);
            R_REPAIR_TRANSFER rRepairTransfer = transferList.Where(r => r.CLOSED_FLAG == "0").FirstOrDefault();//TRANSFER表 1 表示不良
            if (rRepairTransfer != null)
            {
                rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.GetObjByID(rRepairTransfer.ID, Station.SFCDB);
                rowTransfer.CLOSED_FLAG = "1";
                rowTransfer.OUT_TIME = Station.GetDBDateTime();
                rowTransfer.OUT_SEND_EMP = sessionSendEmp.Value.ToString();
                rowTransfer.OUT_RECEIVE_EMP = sessionReceiveEmp.Value.ToString();

                string strRet = (Station.SFCDB).ExecSQL(rowTransfer.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000035", new string[] { strRet }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000025", new string[] { "REPAIR TRANSFER" }, StationMessageState.Pass);
                }
                foreach (R_SN sn in listSNObject)
                {
                    Row_R_SN rowSN = (Row_R_SN)t_r_sn.GetObjByID(sn.ID, Station.SFCDB);
                    rowSN.REPAIR_FAILED_FLAG = "0";
                    strRet = (Station.SFCDB).ExecSQL(rowSN.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    if (Convert.ToInt32(strRet) > 0)
                    {
                        Station.AddMessage("MES00000035", new string[] { strRet }, StationMessageState.Pass);
                    }
                    else
                    {
                        Station.AddMessage("MES00000025", new string[] { "R_SN" }, StationMessageState.Pass);
                    }
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { repairNo, "abnormal" }));
            }
        }

        public static void SNFailAction_Old(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string R_SN_STATION_DETAIL_ID = "";
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                SNLoadPoint = new MESStationSession() { MESDataType = "SN", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(SNLoadPoint);
            }

            //獲取頁面傳過來的數據
            string failCode = Station.Inputs.Find(s => s.DisplayName == "Fail_Code").Value.ToString();
            string failLocation = Station.Inputs.Find(s => s.DisplayName == "Fail_Location").Value.ToString();
            string failProcess = Station.Inputs.Find(s => s.DisplayName == "Fail_Process").Value.ToString();
            string failDescription = Station.Inputs.Find(s => s.DisplayName == "Description").Value.ToString();
            string strSn = Input.Value.ToString();

            OleExec oleDB = null;
            oleDB = Station.SFCDB;
            //oleDB = this.DBPools["SFCDB"].Borrow();
            oleDB.BeginTrain();  //以下執行 要么全成功，要么全失敗

            //更新R_SN REPAIR_FAILED_FLAG=’1’
            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_SN rrSn = (Row_R_SN)rSn.NewRow();
            R_SN r = rSn.GetDetailBySN(strSn, Station.SFCDB);
            rrSn = (Row_R_SN)rSn.GetObjByID(r.ID, Station.SFCDB);
            rrSn.REPAIR_FAILED_FLAG = "1";
            string strRet = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(strRet) > 0))
            {
                throw new Exception("update repair failed flag error!");
            }

            //新增一筆FAIL記錄到R_SN_STATION_DETAIL
            T_R_SN_STATION_DETAIL rSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_SN_STATION_DETAIL_ID = rSnStationDetail.GetNewID(Station.BU, Station.SFCDB);
            string detailResult = rSnStationDetail.AddDetailToRSnStationDetail(R_SN_STATION_DETAIL_ID, rrSn.GetDataObject(), Station.Line, Station.StationName, Station.StationName, Station.SFCDB);
            if (!(Convert.ToInt32(detailResult) > 0))
            {
                throw new Exception("Insert sn station detail error!");
            }

            //新增一筆到R_REPAIR_MAIN
            T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();
            rRepairMain.ID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
            rRepairMain.SN = strSn;
            rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
            rRepairMain.SKUNO = rrSn.SKUNO;
            rRepairMain.FAIL_LINE = Station.Line;
            rRepairMain.FAIL_STATION = Station.StationName;
            rRepairMain.FAIL_EMP = Station.User.EMP_NO;
            rRepairMain.FAIL_TIME = Station.GetDBDateTime();
            rRepairMain.CLOSED_FLAG = "0";
            string insertResult = (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(insertResult) > 0))
            {
                throw new Exception("Insert repair main error!");
            }

            //新增一筆到R_REPAIR_FAILCODE
            T_R_REPAIR_FAILCODE tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
            rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
            rRepairFailCode.REPAIR_MAIN_ID = rRepairMain.ID;
            rRepairFailCode.SN = strSn;
            rRepairFailCode.FAIL_CODE = failCode;
            rRepairFailCode.FAIL_EMP = Station.User.EMP_NO;
            rRepairFailCode.FAIL_TIME = DateTime.Now;
            rRepairFailCode.CREATE_TIME = rRepairFailCode.FAIL_TIME;
            rRepairFailCode.FAIL_CATEGORY = "SYMPTON";
            rRepairFailCode.FAIL_LOCATION = failLocation;
            rRepairFailCode.FAIL_PROCESS = failProcess;
            rRepairFailCode.DESCRIPTION = failDescription;
            rRepairFailCode.REPAIR_FLAG = "0";
            string strResult = (Station.SFCDB).ExecSQL(rRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(strResult) > 0))
            {
                throw new Exception("Insert repair failcode error!");
            }

            oleDB.CommitTrain();

            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
        }

        public static void SNFailAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            Int16 FailCount = 0;
            string StrSn = "";
            string R_SN_STATION_DETAIL_ID = "";
            List<Dictionary<string, string>> FailList = null;

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailListSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[3].VALUE, SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                //      Station.StationSession.Add(StatusSession);
                if (StatusSession.Value == null)
                {
                    StatusSession.Value = "FAIL";
                }
                Station.StationSession.Add(StatusSession);
            }
            MESStationSession ClearInputSession = null;
            if (Paras.Count >= 5)
            {
                ClearInputSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[4].SESSION_TYPE) && t.SessionKey.Equals(Paras[4].SESSION_KEY));
                if (ClearInputSession == null)
                {
                    ClearInputSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                    Station.StationSession.Add(ClearInputSession);
                }
            }

            FailCount = Convert.ToInt16(FailCountSession.Value.ToString());
            FailList = (List<Dictionary<string, string>>)FailListSession.Value;

            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            DateTime FailTime = rSn.GetDBDateTime(Station.SFCDB);
            if (FailList.Count >= FailCount && FailCount != 0) //允許掃描多個Fail
            {
                StrSn = SNSession.InputValue.ToString();
                string repairMainId = "";
                for (int i = 0; i < FailList.Count; i++)
                {
                    //獲取頁面傳過來的數據
                    string failCode = FailList[i]["FailCode"].ToString();
                    string failLocation = FailList[i]["FailLocation"].ToString();
                    string failProcess = FailList[i]["FailProcess"].ToString();
                    string failDescription = FailList[i]["FailDesc"].ToString();

                    OleExec oleDB = null;
                    oleDB = Station.SFCDB;
                    //oleDB.BeginTrain();  //以下執行 要么全成功，要么全失敗
                    T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();

                    Row_R_SN rrSn = (Row_R_SN)rSn.NewRow();
                    if (i == 0)
                    {
                        //更新R_SN REPAIR_FAILED_FLAG=’1’
                        R_SN r = rSn.GetDetailBySN(StrSn, Station.SFCDB);
                        rrSn = (Row_R_SN)rSn.GetObjByID(r.ID, Station.SFCDB);
                        rrSn.REPAIR_FAILED_FLAG = "1";
                        //AOI工站不入维修
                        if (Station.StationName == "AOI1" || Station.StationName == "AOI2")
                        {
                            rrSn.REPAIR_FAILED_FLAG = "0";
                        }

                        rrSn.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rrSn.EDIT_TIME = FailTime;
                        string strRet = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
                        if (!(Convert.ToInt32(strRet) > 0))
                        {
                            throw new Exception("update repair failed flag error!");
                        }

                        //新增一筆FAIL記錄到R_SN_STATION_DETAIL
                        T_R_SN_STATION_DETAIL rSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        R_SN_STATION_DETAIL_ID = rSnStationDetail.GetNewID(Station.BU, Station.SFCDB);
                        // string detailResult = rSnStationDetail.AddDetailToRSnStationDetail(R_SN_STATION_DETAIL_ID,rrSn.GetDataObject(),Station.Line,Station.StationName,Station.StationName,Station.SFCDB);
                        string detailResult = rSnStationDetail.AddDetailToBipStationFailDetail(
                               R_SN_STATION_DETAIL_ID, rrSn.GetDataObject(), Station.Line, Station.StationName,
                               Station.StationName, Station.SFCDB, "1");
                        if (!(Convert.ToInt32(detailResult) > 0))
                        {
                            throw new Exception("Insert sn station detail error!");
                        }

                        //新增一筆到R_REPAIR_MAIN 
                        repairMainId = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                        rRepairMain.ID = repairMainId;
                        rRepairMain.SN = StrSn;
                        rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
                        rRepairMain.SKUNO = rrSn.SKUNO;
                        rRepairMain.FAIL_LINE = Station.Line;
                        rRepairMain.FAIL_STATION = Station.StationName;
                        rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                        //rRepairMain.FAIL_TIME = Station.GetDBDateTime();//Mpdofy by LLF 2018-03-17
                        rRepairMain.FAIL_TIME = FailTime;
                        rRepairMain.CREATE_TIME = Station.GetDBDateTime();
                        rRepairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rRepairMain.EDIT_TIME = Station.GetDBDateTime();
                        rRepairMain.CLOSED_FLAG = "0";
                        string insertResult = (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
                        if (!(Convert.ToInt32(insertResult) > 0))
                        {
                            throw new Exception("Insert repair main error!");
                        }
                        //關聯Juniper測試記錄
                        if (Station.BU == "FJZ" || Station.BU == "VNJUNIPER")
                        {
                            try
                            {
                                //No need SMT=>AOI in this action,cause R_TEST_JUNIPER & R_TEST_RECORD has already insert OK, if change SMT to AOI, it will not get fail record data. 2021-11-17 edit
                                //if (Station.StationName == "SMT1")
                                //{
                                //    Station.StationName = "AOI1";
                                //}
                                //else if (Station.StationName == "SMT2")
                                //{
                                //    Station.StationName = "AOI2";
                                //}

                                //Station.SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.MES_STATION == Station.StationName).Select(t=>t.).ToList();
                                //var failrecord = Station.SFCDB.ORM.Queryable<R_TEST_JUNIPER>()
                                //    .Where(t => t.EVENTNAME == Station.StationName && t.STATUS == "FAIL").OrderBy(t => t.TATIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                                //缺少SN條件, 導致取到的信息與SN不匹配
                                var failrecord = Station.SFCDB.ORM.Queryable<R_TEST_RECORD, R_TEST_JUNIPER>((tr, tj) => tr.ID == tj.R_TEST_RECORD_ID)
                                    .Where((tr, tj) => tr.MESSTATION == Station.StationName && tr.STATE == "FAIL" && tr.SN == StrSn)
                                    .OrderBy((tr, tj) => tj.TATIME, SqlSugar.OrderByType.Desc).Select((tr, tj) => tj).ToList().FirstOrDefault();
                                if (failrecord != null)
                                {
                                    R_REPAIR_MAIN_EX ex = new R_REPAIR_MAIN_EX()
                                    {
                                        ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_REPAIR_MAIN_EX"),
                                        MAIN_ID = rRepairMain.ID,
                                        NAME = "UNIQUE_TEST_ID",
                                        VALUE = failrecord.UNIQUE_TEST_ID,
                                    };
                                    Station.SFCDB.ORM.Insertable(ex).ExecuteCommand();
                                    ex = new R_REPAIR_MAIN_EX()
                                    {
                                        ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_REPAIR_MAIN_EX"),
                                        MAIN_ID = rRepairMain.ID,
                                        NAME = "JUNIPER_TEST_ID",
                                        VALUE = failrecord.ID,
                                    };
                                    Station.SFCDB.ORM.Insertable(ex).ExecuteCommand();
                                }

                            }
                            catch
                            { }
                        }


                    }

                    ////新增一筆到R_REPAIR_MAIN
                    //T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    //Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();
                    //rRepairMain.ID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                    //rRepairMain.SN = StrSn;
                    //rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
                    //rRepairMain.SKUNO = rrSn.SKUNO;
                    //rRepairMain.FAIL_LINE = Station.Line;
                    //rRepairMain.FAIL_STATION = Station.StationName;
                    //rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                    //rRepairMain.FAIL_TIME = FailTime.ToString();
                    //rRepairMain.CLOSED_FLAG = "0";
                    //string insertResult = (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
                    //if (!(Convert.ToInt32(insertResult) > 0))
                    //{
                    //    throw new Exception("Insert repair main error!");
                    //}

                    //新增一筆到R_REPAIR_FAILCODE
                    T_R_REPAIR_FAILCODE tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
                    rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
                    rRepairFailCode.REPAIR_MAIN_ID = repairMainId;
                    rRepairFailCode.SN = StrSn;
                    rRepairFailCode.FAIL_CODE = failCode;
                    rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
                    rRepairFailCode.FAIL_TIME = FailTime;
                    rRepairFailCode.FAIL_CATEGORY = "SYMPTOM";
                    rRepairFailCode.FAIL_LOCATION = failLocation;
                    rRepairFailCode.FAIL_PROCESS = failProcess;
                    rRepairFailCode.DESCRIPTION = failDescription;
                    rRepairFailCode.REPAIR_FLAG = "0";
                    rRepairFailCode.CREATE_TIME = Station.GetDBDateTime();
                    rRepairFailCode.EDIT_EMP = Station.LoginUser.EMP_NO;
                    rRepairFailCode.EDIT_TIME = Station.GetDBDateTime();

                    string strResult = (Station.SFCDB).ExecSQL(rRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (!(Convert.ToInt32(strResult) > 0))
                    {
                        throw new Exception("Insert repair failcode error!");
                    }

                    //oleDB.CommitTrain();
                }
                if (ClearInputSession != null)
                {
                    ClearInputSession.Value = "true";
                }
                else
                {
                    ((List<Dictionary<string, string>>)FailListSession.Value).Clear();
                }
                Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
            }
            else
            {
                Station.NextInput = Station.FindInputByName("Location");
                Station.AddMessage("MES00000162", new string[] { StrSn, FailCount.ToString(), FailList.Count.ToString() }, StationMessageState.Message);
            }
        }
        public static void FailStopLineAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            Int16 FailCount = 0;
            string StrSn = "";
            List<Dictionary<string, string>> FailList = null;

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailListSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            FailCount = Convert.ToInt16(FailCountSession.Value.ToString());
            FailList = (List<Dictionary<string, string>>)FailListSession.Value;

            if (FailList.Count >= FailCount && FailCount != 0)
            {
                StrSn = SNSession.InputValue.ToString();
                List<string> fcode = new List<string>();
                for (int i = 0; i < FailList.Count; i++)
                {
                    fcode.Add(FailList[i]["FailCode"].ToString());
                }

                var stoplinecode = Station.SFCDB.ORM.Queryable<C_ERROR_CODE>()
                    .Where(t => fcode.Contains(t.ERROR_CODE) && t.ERROR_CATEGORY == "STOPLINE")
                    .ToList();

                if (stoplinecode.Count > 0)
                {
                    var fc = "";
                    for (int i = 0; i < stoplinecode.Count; i++)
                    {
                        fc += stoplinecode[i].ERROR_CODE.ToString() + ",";
                    }
                    fc = fc.Substring(0, fc.Length - 1);

                    R_SN SN = Station.SFCDB.ORM.Queryable<R_SN>()
                        .Where(t => t.SN == StrSn && t.VALID_FLAG == "1")
                        .First();

                    R_SN_LOCK locker = new R_SN_LOCK()
                    {
                        ID = MESDataObject.MesDbBase.GetNewID<R_SN_LOCK>(Station.SFCDB.ORM, Station.BU),
                        TYPE = "WO",
                        WORKORDERNO = SN.WORKORDERNO,
                        LOCK_STATION = "ALL",
                        LOCK_REASON = "Fail Stop Line:" + fc,
                        LOCK_STATUS = "1",
                        LOCK_TIME = DateTime.Now,
                        LOCK_EMP = "System",
                        LOCK_LOT = ""
                    };
                    Station.SFCDB.ORM.Insertable<R_SN_LOCK>(locker).ExecuteCommand();
                    Station.AddMessage("MSGCODE20210609091614", new string[] { fc, SN.WORKORDERNO }, StationMessageState.Alert, StationMessageDisplayType.Swal);
                }
            }
        }

        public static void FailCountByLocationAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";            
            string StrSn = "";

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            StrSn = "EAAA0838"; //SNSession.InputValue.ToString().Trim().ToUpper();

            List<R_REPAIR_FAILCODE> fails = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>()
                                         .Where(rf => rf.SN.Trim().ToUpper() == StrSn).ToList();

            var failCountByLoc = fails
                .GroupBy(rf => new { rf.SN, rf.F_LOCATION })
                .Select(nr => new
                {
                    ID = nr.Key,
                    //SN = ,
                    FAIL_CODE_LOC = nr.Key.F_LOCATION,
                    FAIL_COUNT_LOC = nr.Count()
                }).Distinct();

            /*
            var failCountByLoc = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>()
                .Where(rf => rf.SN.Trim().ToUpper() == StrSn)
                .GroupBy(rf => new { rf.SN, rf.FAIL_CODE })
                .Select(nr => new
                {
                    SN = nr.Key.SN,
                    FAIL_CODE_LOC = nr.Key.FAIL_CODE,
                    FAIL_COUNT_LOC = nr.Distinct().Count()
                })
                .FirstOrDefault();
                */

            if (failCountByLoc == null)
                return;
            /*
            if (failCountByLoc.FAIL_COUNT_LOC >= 2)
            {                
                string lockReason = MESReturnMessage.GetMESReturnMessage("MSGCODE20210825165016", new string[] { StrSn, failCountByLoc.FAIL_CODE_LOC + " FailCount: " + failCountByLoc.FAIL_COUNT_LOC.ToString() });

                R_SN_LOCK locker = new R_SN_LOCK()
                {
                    ID = MESDataObject.MesDbBase.GetNewID<R_SN_LOCK>(Station.SFCDB.ORM, Station.BU),
                    TYPE = "SN",
                    SN = StrSn,
                    //WORKORDERNO = SN.WORKORDERNO,
                    LOCK_STATION = "ALL",
                    LOCK_REASON = lockReason,
                    LOCK_STATUS = "1",
                    LOCK_TIME = DateTime.Now,
                    LOCK_EMP = "System",
                    LOCK_LOT = ""
                };
                               
                Station.SFCDB.ORM.Insertable<R_SN_LOCK>(locker).ExecuteCommand();
               
                Station.AddMessage("MSGCODE20210825165016", new string[] { StrSn, failCountByLoc.FAIL_CODE_LOC + " FailCount: " + failCountByLoc.FAIL_COUNT_LOC.ToString() }, StationMessageState.Alert, StationMessageDisplayType.Swal);


            }*/

        }

        public static void HWDBIPSNFailAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string StrSn = "";
            string APVirtualSn = "";
            string VirtualSn = "";
            AP_DLL APObj = new AP_DLL();
            OleExec APDB = null;
            R_PANEL_SN PANELObj = null;
            Int16 FailCount = 0;
            List<Dictionary<string, string>> FailList = null;
            string R_SN_STATION_DETAIL_ID = "";
            StringBuilder ReturnMessage = new StringBuilder();
            string RepairmainID = "";//add by LLF 2018-04-12

            if (Paras.Count < 5)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailListSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession PanelVitualSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (PanelVitualSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[4].VALUE, SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value.ToString() == "")
                {
                    StatusSession.Value = "FAIL";
                }
            }

            MESStationSession ClearInputSession = null;
            if (Paras.Count >= 6)
            {
                ClearInputSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[5].SESSION_TYPE) && t.SessionKey.Equals(Paras[5].SESSION_KEY));
                if (ClearInputSession == null)
                {
                    ClearInputSession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY };
                    Station.StationSession.Add(ClearInputSession);
                }
            }

            FailCount = Convert.ToInt16(FailCountSession.Value.ToString());
            FailList = (List<Dictionary<string, string>>)FailListSession.Value;

            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            DateTime FailTime = rSn.GetDBDateTime(Station.SFCDB);
            StrSn = SNSession.Value.ToString();
            if (FailList.Count >= FailCount && FailCount != 0) //允許掃描多個Fail
            {
                #region 業務邏輯

                OleExec oleDB = null;
                oleDB = Station.SFCDB;
                //oleDB = this.DBPools["SFCDB"].Borrow();
                //oleDB.BeginTrain(); 
                //OleExecPool APDBPool = Station.DBS["APDB"];
                //APDB = APDBPool.Borrow();
                //APDB.BeginTrain();
                APDB = Station.APDB;
                try
                {
                    Row_R_SN rrSn = (Row_R_SN)rSn.NewRow();
                    for (int i = 0; i < FailList.Count; i++)
                    {
                        //獲取頁面傳過來的數據
                        string failCode = FailList[i]["FailCode"].ToString();
                        string failLocation = FailList[i]["FailLocation"].ToString();
                        string failProcess = FailList[i]["FailProcess"].ToString();
                        string failDescription = FailList[i]["FailDesc"].ToString();



                        //黃楊盛 2018年4月24日14:14:28 模擬自動做維修的動作,修正時間
                        //更新R_SN REPAIR_FAILED_FLAG=’1’
                        //modify by ZGJ 2018-03-22 BIP Fail 的產品自動清除待維修狀態，但是記錄不良

                        PANELObj = (R_PANEL_SN)PanelVitualSNSession.Value;
                        if (i == 0)
                        {
                            VirtualSn = PANELObj.SN.ToString();
                            R_SN r = rSn.GetDetailBySN(VirtualSn, Station.SFCDB);
                            rrSn = (Row_R_SN)rSn.GetObjByID(r.ID, Station.SFCDB);
                            //rrSn.REPAIR_FAILED_FLAG = "0";
                            rrSn.REPAIR_FAILED_FLAG = "1";
                            rrSn.SN = StrSn;

                            C_ROUTE_DETAIL routeDetail = TCRD.GetStationRoute(rrSn.ROUTE_ID, Station.StationName, Station.SFCDB);
                            if (routeDetail.STATION_TYPE.ToUpper() == "JOBFINISH")
                            {
                                //MODIFY by fgg 2019.08.19 BIP工站為JOBFINISH工站 再改COMPLETED=1,以避免BIP FAIL 后再入MRB，就不會拋賬到工單，只會轉倉到對應的MRB倉碼，進而導致工單拋賬異常
                                rrSn.COMPLETED_FLAG = "1";
                                rrSn.COMPLETED_TIME = Station.GetDBDateTime();
                            }
                            rrSn.EDIT_EMP = Station.LoginUser.EMP_NO; //add by LLF 2018-03-17
                            rrSn.EDIT_TIME = FailTime; //add by LLF 2018-03-17
                            string strRet = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
                            if (!(Convert.ToInt32(strRet) > 0))
                            {
                                throw new MESReturnMessage("update repair failed flag error!");
                            }

                            //Update r_sn_kp add by fgg 2018.05.23
                            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            t_r_sn_kp.UpdateSNBySnId(r.ID, StrSn, Station.LoginUser.EMP_NO, Station.SFCDB);
                            // Update R_PANEL_SN
                            T_R_PANEL_SN RPanelSN = new T_R_PANEL_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            Row_R_PANEL_SN Row_Panel = (Row_R_PANEL_SN)RPanelSN.NewRow();
                            Row_Panel = (Row_R_PANEL_SN)RPanelSN.GetObjByID(PANELObj.ID, Station.SFCDB);
                            Row_Panel.SN = StrSn;
                            strRet = (Station.SFCDB).ExecSQL(Row_Panel.GetUpdateString(DB_TYPE_ENUM.Oracle));
                            if (!(Convert.ToInt32(strRet) > 0))
                            {
                                throw new MESReturnMessage("update r_panel_sn error!");
                            }

                            //Update AP 

                            //黄杨盛 2018年4月14日09:10:50 修正不能超过9连板的情况.同时加上不支持3位数连板的约束
                            //APVirtualSn = PANELObj.PANEL + "0" + PANELObj.SEQ_NO.ToString();
                            if (PANELObj.SEQ_NO > 99)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000226",
                                    new string[] { DB_TYPE_ENUM.Oracle.ToString() }));
                            }
                            APVirtualSn = PANELObj.PANEL + Convert.ToInt16(PANELObj.SEQ_NO).ToString("00");


                            string result = APObj.APUpdatePanlSN(APVirtualSn, StrSn, APDB);
                            //APDBPool.Return(APDB);
                            if (!result.Equals("OK"))
                            {
                                //2018年4月24日13:56:14 黃楊盛 
                                //throw new MESReturnMessage("already be binded to other serial number");
                                throw new MESReturnMessage(result);
                            }


                            //新增一筆FAIL記錄到R_SN_STATION_DETAIL
                            T_R_SN_STATION_DETAIL rSnStationDetail =
                                new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            //Add by LLF 2018-03-19
                            R_SN_STATION_DETAIL_ID = rSnStationDetail.GetNewID(Station.BU, Station.SFCDB);
                            //string detailResult = rSnStationDetail.AddDetailToRSnStationDetail(R_SN_STATION_DETAIL_ID,rrSn.GetDataObject(), Station.Line, Station.StationName, Station.StationName, Station.SFCDB);
                            string detailResult = rSnStationDetail.AddDetailToBipStationFailDetail(
                                R_SN_STATION_DETAIL_ID, rrSn.GetDataObject(), Station.Line, Station.StationName,
                                Station.StationName, Station.SFCDB, "1");
                            if (!(Convert.ToInt32(detailResult) > 0))
                            {
                                throw new MESReturnMessage("Insert sn station detail error!");
                            }

                            //update R_SN_STATION_DETAIL 
                            rSnStationDetail.UpdateRSnStationDetailBySNID(StrSn, PANELObj.SN, Station.SFCDB);

                            //新增一筆到R_REPAIR_MAIN
                            T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();

                            RepairmainID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                            //rRepairMain.ID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                            rRepairMain.ID = RepairmainID;
                            rRepairMain.SN = StrSn;
                            rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
                            rRepairMain.SKUNO = rrSn.SKUNO;
                            rRepairMain.FAIL_LINE = Station.Line;
                            rRepairMain.FAIL_STATION = Station.StationName;
                            rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                            //rRepairMain.FAIL_TIME = Station.GetDBDateTime();//Modify by LLF 2018-03-17
                            rRepairMain.FAIL_TIME = FailTime; //Modify by LLF 2018-03-17
                            rRepairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rRepairMain.EDIT_TIME = Station.GetDBDateTime();
                            rRepairMain.CLOSED_FLAG = "1";
                            string insertResult =
                                (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
                            if (!(Convert.ToInt32(insertResult) > 0))
                            {
                                throw new Exception("Insert repair main error!");
                            }
                        }

                        //新增一筆到R_REPAIR_FAILCODE
                        T_R_REPAIR_FAILCODE tRepairFailCode =
                            new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
                        rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
                        //rRepairFailCode.REPAIR_MAIN_ID = rRepairMain.ID; //Modify by LLF 2018-04-11 多筆FAIL 取1一個RepairMainID
                        rRepairFailCode.REPAIR_MAIN_ID = RepairmainID;
                        rRepairFailCode.SN = StrSn;
                        rRepairFailCode.FAIL_CODE = failCode;
                        rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
                        rRepairFailCode.FAIL_TIME = FailTime;
                        rRepairFailCode.CREATE_TIME = rRepairFailCode.FAIL_TIME;
                        rRepairFailCode.FAIL_CATEGORY = "SYMPTOM";
                        rRepairFailCode.FAIL_LOCATION = failLocation;
                        rRepairFailCode.FAIL_PROCESS = failProcess;
                        rRepairFailCode.DESCRIPTION = failDescription;
                        rRepairFailCode.REPAIR_FLAG = "1";
                        rRepairFailCode.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rRepairFailCode.EDIT_TIME = Station.GetDBDateTime();

                        string strResult =
                            (Station.SFCDB).ExecSQL(rRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
                        if (!(Convert.ToInt32(strResult) > 0))
                        {
                            throw new MESReturnMessage("Insert repair failcode error!");
                        }


                        //oleDB.CommitTrain();

                        ReturnMessage.Append(failDescription).Append("|");
                    }


                    //黃楊盛 2018年4月24日14:11:42 做一筆出來的記錄
                    R_SN snOut = rSn.GetDetailBySN(StrSn, Station.SFCDB);
                    rrSn = (Row_R_SN)rSn.GetObjByID(snOut.ID, Station.SFCDB);
                    rrSn.CURRENT_STATION = rrSn.NEXT_STATION;
                    rrSn.NEXT_STATION = rSn.GetNextStation(snOut.ROUTE_ID, snOut.NEXT_STATION, oleDB);
                    rrSn.REPAIR_FAILED_FLAG = "0";
                    rrSn.SN = StrSn;
                    rrSn.EDIT_EMP = Station.LoginUser.EMP_NO;
                    rrSn.EDIT_TIME = FailTime;
                    var count = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    if (!(Convert.ToInt32(count) > 0))
                    {
                        throw new MESReturnMessage("update rsn failed flag error!");
                    }

                    // 方國剛 2018.05.02 11:45:30
                    // 因拋賬計算過站數量時，不計算REPAIR_FAILED_FLAG=1的數量，故BIP Fail 的產品自動清除待維修狀態后再在過站記錄表記錄一筆REPAIR_FAILED_FLAG=0的記錄
                    T_R_SN_STATION_DETAIL rSnStationDetailRepaired = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    R_SN_STATION_DETAIL_ID = rSnStationDetailRepaired.GetNewID(Station.BU, Station.SFCDB);
                    string detailResultRepaired = rSnStationDetailRepaired.AddDetailToBipStationFailDetail(
                        R_SN_STATION_DETAIL_ID, rrSn.GetDataObject(), Station.Line, Station.StationName,
                        Station.StationName, Station.SFCDB, "0");
                    if (!(Convert.ToInt32(detailResultRepaired) > 0))
                    {
                        throw new MESReturnMessage("Insert sn station detail error!");
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        //APDB.RollbackTrain();
                        //oleDB.RollbackTrain();
                    }
                    catch (Exception)
                    {
                        ;
                    }

                    throw new MESReturnMessage(e.Message + e.StackTrace);
                }
                finally
                {
                    //APDBPool.Return(APDB);
                }



                #endregion
                //add by zgj 2018-03-14
                //當記錄完當前 SN 不良後，清除保存在 session 中的不良信息
                //((List<Dictionary<string, string>>)FailListSession.Value).Clear();

                if (ClearInputSession != null)
                {
                    ClearInputSession.Value = "true";
                }

                ReturnMessage.Remove(ReturnMessage.Length - 1, 1);
                Station.NextInput = Station.FindInputByName("PanelSn");
                Station.AddMessage("MES00000158", new string[] { StrSn, ReturnMessage.ToString() }, StationMessageState.Pass);
            }
            else
            {
                Station.NextInput = Station.FindInputByName("Location");
                Station.AddMessage("MES00000162", new string[] { StrSn, FailCount.ToString(), FailList.Count.ToString() }, StationMessageState.Message);
            }
        }

        /// <summary>
        /// add by 黃天柱 2018.10.24
        /// 應對 BPD 提出的單個 FailCode 不良可以輸入多次維修動作，因此將 插入數據到 R_REPAIR_ACTION、更新 R_REPAIR_FAILCODE 以及 更新 產品狀態三個功能分開
        /// 每次輸入 Description 后回車就會調用不良維修動作的記錄，全部記錄完后點擊 Save Action 就會更新 R_REPAIR_FAILCODE 中的記錄
        /// 最後點擊 Save 的時候就完成整個產品的維修，更新 R_REPAIR_MAIN 以及 R_SN
        ///PCBA單個維修動作完成Action 
        public static void RecordRepairAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            string result = "";
            string failCodeID = "";
            string actionCode = "";
            string rootCause = "";
            string process = "";
            string location = "";
            string section = "";
            string repairItem = "";
            string repairItemSon = "";
            string pcbaSN = "";
            //string returnStation = "";
            //string currentStation = "";

            string tr_sn = "";
            string kp_no = "";
            string mfr_name = "";
            string date_code = "";
            string lot_code = "";

            string description = "";

            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_ACTION RepairRow = (Row_R_REPAIR_ACTION)RepairAction.NewRow();

            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            Station.SFCDB.ThrowSqlExeception = true;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                //獲取到 SN 對象
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SnObject = (SN)SNSession.Value;

                MESStationSession FailCodeIDSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (FailCodeIDSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else if (FailCodeIDSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                failCodeID = FailCodeIDSession.Value.ToString();

                if (failCodeID != "")
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(failCodeID, Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, failCodeID }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }

                MESStationSession ActionCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (ActionCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                else if (ActionCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                actionCode = ActionCodeSession.Value.ToString();

                MESStationSession PCBASNSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (PCBASNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else if (PCBASNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                pcbaSN = PCBASNSession.Value.ToString();


                MESStationSession RootCauseSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (RootCauseSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                else if (RootCauseSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                rootCause = RootCauseSession.Value.ToString();


                MESStationSession ProcessSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
                if (ProcessSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                else if (ProcessSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                process = ProcessSession.Value.ToString();

                MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
                if (LocationSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                else if (LocationSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                location = LocationSession.Value.ToString();

                MESStationSession SectionSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
                if (SectionSession != null && SectionSession.Value != null)
                {
                    section = SectionSession.Value.ToString();
                }

                MESStationSession TR_SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
                MESStationSession KPNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[11].SESSION_KEY);
                MESStationSession MFRNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[12].SESSION_KEY);
                MESStationSession DateCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[11].SESSION_TYPE && t.SessionKey == Paras[13].SESSION_KEY);
                MESStationSession LotCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[12].SESSION_TYPE && t.SessionKey == Paras[14].SESSION_KEY);

                //如果有輸入ALLPART條碼,則取ALLPART條碼對應的料號、廠商、DateCode、LotCode，沒有則取輸入的值
                if (TR_SNSession != null)
                {
                    if (TR_SNSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                    }
                    DataRow tr_snRow = (DataRow)TR_SNSession.Value;
                    tr_sn = tr_snRow["TR_SN"].ToString();
                    kp_no = tr_snRow["CUST_KP_NO"].ToString();
                    mfr_name = tr_snRow["MFR_KP_NO"].ToString();
                    date_code = tr_snRow["DATE_CODE"].ToString();
                    lot_code = tr_snRow["Lot_Code"].ToString();
                }
                else
                {
                    if (KPNOSession != null && KPNOSession.Value != null)
                    {
                        kp_no = KPNOSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "KP_NO") != null)
                    {
                        kp_no = Station.Inputs.Find(input => input.DisplayName == "KP_NO").Value.ToString();
                    }

                    if (MFRNameSession != null && MFRNameSession.Value != null)
                    {
                        mfr_name = MFRNameSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "MFR_Name") != null)
                    {
                        mfr_name = Station.Inputs.Find(input => input.DisplayName == "MFR_Name").Value.ToString();
                    }

                    if (DateCodeSession != null && DateCodeSession.Value != null)
                    {
                        date_code = DateCodeSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "Date_Code") != null)
                    {
                        date_code = Station.Inputs.Find(input => input.DisplayName == "Date_Code").Value.ToString();
                    }

                    if (LotCodeSession != null && LotCodeSession.Value != null)
                    {
                        lot_code = LotCodeSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "Lot_Code") != null)
                    {
                        lot_code = Station.Inputs.Find(input => input.DisplayName == "Lot_Code").Value.ToString();
                    }
                }

                MESStationInput DescriptionSession = Station.Inputs.Find(t => t.DisplayName == Paras[13].SESSION_TYPE);
                if (DescriptionSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[13].SESSION_TYPE + Paras[13].SESSION_KEY }));
                }
                else if (DescriptionSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[13].SESSION_TYPE + Paras[13].SESSION_KEY }));
                }
                description = DescriptionSession.Value.ToString();

                RepairRow.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                RepairRow.REPAIR_FAILCODE_ID = failCodeID;
                RepairRow.SN = SnObject.SerialNo;
                RepairRow.ACTION_CODE = actionCode;
                RepairRow.SECTION_ID = section;
                RepairRow.PROCESS = process;
                RepairRow.ITEMS_ID = repairItem;
                RepairRow.ITEMS_SON_ID = repairItemSon;
                RepairRow.REASON_CODE = rootCause;
                RepairRow.DESCRIPTION = description;
                RepairRow.FAIL_LOCATION = location;
                RepairRow.FAIL_CODE = FailCodeRow.FAIL_CODE;
                RepairRow.KEYPART_SN = pcbaSN;
                RepairRow.NEW_KEYPART_SN = "";
                RepairRow.TR_SN = tr_sn;
                RepairRow.KP_NO = kp_no;
                RepairRow.MFR_NAME = mfr_name;
                RepairRow.DATE_CODE = date_code;
                RepairRow.LOT_CODE = lot_code;
                RepairRow.REPAIR_EMP = Station.LoginUser.EMP_NO;
                RepairRow.REPAIR_TIME = Station.GetDBDateTime();
                RepairRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                RepairRow.EDIT_TIME = Station.GetDBDateTime();

                result = (Station.SFCDB).ExecSQL(RepairRow.GetInsertString(DB_TYPE_ENUM.Oracle));//插入一條action記錄
                if (!(Convert.ToInt32(result) > 0))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "REPAIR_ACTION" }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// add by 黃天柱 2018.10.25
        /// 單個 FailCode 的所有維修動作完成後，更新 R_REPAIR_FAILCODE 中的 Repair_flag 為 1 以及 R_REPAIR_MAIN 中的 Closed_Flag 為1
        public static void UpdateFailRecordAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            String failCodeID = "";
            string updateSql = "";
            SN SnObject = null;

            MESStationSession FailCodeIDSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCodeIDSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else if (FailCodeIDSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            failCodeID = FailCodeIDSession.Value.ToString();

            if (failCodeID != "")
            {
                FailCodeRow = RepairFailcode.GetByFailCodeID(failCodeID, Station.SFCDB);
                if (FailCodeRow == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, failCodeID }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }


            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;


            Row_R_REPAIR_FAILCODE FRow = (Row_R_REPAIR_FAILCODE)RepairFailcode.GetObjByID(failCodeID, Station.SFCDB);
            FRow.REPAIR_FLAG = "1";  //執行完維修動作後更新R_REPAIR_FAILCODE   FLAG=1 
            FRow.EDIT_TIME = Station.GetDBDateTime();
            FRow.EDIT_EMP = Station.LoginUser.EMP_NO;
            updateSql = FRow.GetUpdateString(Station.DBType);
            Station.SFCDB.ExecSQL(updateSql);
            for (int i = 1; i < Paras.Count; i++)
            {
                Station.StationSession.Remove(Station.StationSession.Find(s => s.MESDataType == Paras[i].SESSION_TYPE && s.SessionKey == Paras[i].SESSION_KEY));
            }
            foreach (MESStationInput input in Station.Inputs)
            {
                input.Value = null;
            }

            T_R_REPAIR_MAIN REPAIRMAIN = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_MAIN ROWMAIN = (Row_R_REPAIR_MAIN)REPAIRMAIN.GetObjByID(FRow.REPAIR_MAIN_ID, Station.SFCDB);
            ROWMAIN.CLOSED_FLAG = "1";
            ROWMAIN.EDIT_TIME = Station.GetDBDateTime();
            string UPDATEMAIN = ROWMAIN.GetUpdateString(Station.DBType);
            Station.SFCDB.ExecSQL(UPDATEMAIN);
            Station.AddMessage("MES00000105", new string[] { SnObject.SerialNo, failCodeID }, StationMessageState.Pass);
        }

        /// add by 黃天柱 2018.10.25
        /// 所有的 FailCode 完成后，更新 R_SN 中的記錄，表示產品已是正常產品，可以進入產線繼續生產
        /// 產品的狀態變更：
        ///     （1）如果產品的流程裡面包括 3DX 和 ICT
        ///         1）如果當前站位是 ICT
        ///             如果不良代碼是 C_CONTROL 表中CONTROL_NAME 是 SPECIAL_REPAIR_RETURN_STATION 記錄里 CONTROL_VALUE 部分的值（連接器不良），那麼產品維修完成后就
        ///             拿到 3DX 重測，否則就回到 ICT 重測
        ///         2）如果不是 ICT，那麼就按正常返修即可。
        ///     （2）如果產品的流程裡面不同時包括 3DX 和 ICT，那麼就按正常返修即可。
        /// 
        public static void ReturnToStationAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ReturnStation = "";
            string CurrentStation = "";
            string FailCodeId = string.Empty;
            R_REPAIR_FAILCODE FailCode = null;
            T_R_REPAIR_FAILCODE TRRF = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            MESStationSession ReturnStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);


            SN SnObject = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;

            MESStationSession FailCodeIDSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCodeIDSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else if (FailCodeIDSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            FailCodeId = FailCodeIDSession.Value.ToString();
            if (FailCodeId != "")
            {
                FailCode = TRRF.GetFailCodeById(FailCodeId, Station.SFCDB);
                if (FailCode == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, FailCodeId }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            ReturnStation = TCRD.GetSpecialReturnStation(SnObject.RouteID, SnObject.NextStation, FailCode.FAIL_CODE, Station.SFCDB);
            List<C_ROUTE_DETAIL> stationList = TCRD.GetLastStations(SnObject.RouteID, ReturnStation, Station.SFCDB);
            if (stationList.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903091835", new string[] { ReturnStation }));
            }
            CurrentStation = stationList[stationList.Count - 1].STATION_NAME;
            DateTime edtiTime = Station.GetDBDateTime();

            var i = Station.SFCDB.ORM.Updateable<R_SN>().UpdateColumns(rsn => new R_SN
            {
                CURRENT_STATION = CurrentStation,
                NEXT_STATION = ReturnStation,
                EDIT_EMP = Station.LoginUser.EMP_NO,
                EDIT_TIME = edtiTime
            }).Where(rsn => rsn.ID == SnObject.ID).ExecuteCommand();
            if (i <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
            }
        }

        /// add by fgg 2018.6.15
        ///PCBA單個維修動作完成Action 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PCBARepairSaveAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //SN SnObject = null;
            string updateSql = null;
            string failCodeID = "";
            string actionCode = "";
            string rootCause = "";
            string process = "";
            string location = "";
            string section = "";
            string repairItem = "";
            string repairItemSon = "";
            string pcbaSN = "";
            string returnStation = "";
            string currentStation = "";

            string tr_sn = "";
            string kp_no = "";
            string mfr_name = "";
            string date_code = "";
            string lot_code = "";

            string description = "";

            string repair_no = "";

            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_ACTION RepairRow = (Row_R_REPAIR_ACTION)RepairAction.NewRow();
            T_C_REPAIR_ITEMS TTRepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RepairItemsRow;
            T_C_REPAIR_ITEMS_SON HHRepairItemSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS_SON RepairItemSonRow;
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> listSN = new List<R_SN>();

            Station.SFCDB.ThrowSqlExeception = true;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                //獲取到 SN 對象
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                if (SNSession.Value.GetType() == typeof(SN))
                {
                    LogicObject.SN snObject = (LogicObject.SN)SNSession.Value;
                    repair_no = snObject.SerialNo;
                    listSN.Add(t_r_sn.LoadData(repair_no, Station.SFCDB));
                }
                else if (SNSession.Value.GetType() == typeof(Panel))
                {
                    Panel panelObject = (Panel)SNSession.Value;
                    repair_no = panelObject.PanelNo;
                    listSN = panelObject.GetSnDetail(repair_no, Station.SFCDB, Station.DBType);
                }

                MESStationSession FailCodeIDSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (FailCodeIDSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else if (FailCodeIDSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                failCodeID = FailCodeIDSession.Value.ToString();

                if (failCodeID != "")
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(failCodeID, Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { repair_no, failCodeID }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }

                MESStationSession ActionCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (ActionCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                else if (ActionCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                actionCode = ActionCodeSession.Value.ToString();

                MESStationSession PCBASNSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (PCBASNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else if (PCBASNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                pcbaSN = PCBASNSession.Value.ToString();


                MESStationSession RootCauseSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (RootCauseSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                else if (RootCauseSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                rootCause = RootCauseSession.Value.ToString();


                MESStationSession ProcessSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
                if (ProcessSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                else if (ProcessSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                process = ProcessSession.Value.ToString();

                MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
                if (LocationSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                else if (LocationSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                location = LocationSession.Value.ToString();

                MESStationSession SectionSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
                if (SectionSession != null && SectionSession.Value != null)
                {
                    section = SectionSession.Value.ToString();
                }

                MESStationSession RepairItemSession = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
                if (RepairItemSession != null)
                {
                    repairItem = RepairItemSession.Value.ToString();
                    if (!string.IsNullOrEmpty(repairItem))
                    {
                        RepairItemsRow = TTRepairItems.GetIDByItemName(repairItem, Station.SFCDB);
                        repairItem = RepairItemsRow.ID;
                    }
                }

                MESStationSession RepairItemSonSession = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
                if (RepairItemSonSession != null)
                {
                    repairItemSon = RepairItemSonSession.Value.ToString();
                    if (!string.IsNullOrEmpty(repairItemSon))
                    {
                        RepairItemSonRow = HHRepairItemSon.GetIDByItemsSon(repairItemSon, Station.SFCDB);
                        repairItemSon = RepairItemSonRow.ID;
                    }
                }

                MESStationSession TR_SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
                MESStationSession KPNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[11].SESSION_TYPE && t.SessionKey == Paras[11].SESSION_KEY);
                MESStationSession MFRNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[12].SESSION_TYPE && t.SessionKey == Paras[12].SESSION_KEY);
                MESStationSession DateCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[13].SESSION_TYPE && t.SessionKey == Paras[13].SESSION_KEY);
                MESStationSession LotCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[14].SESSION_TYPE && t.SessionKey == Paras[14].SESSION_KEY);

                //如果有輸入ALLPART條碼,則取ALLPART條碼對應的料號、廠商、DateCode、LotCode，沒有則取輸入的值
                if (TR_SNSession != null)
                {
                    if (TR_SNSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                    }
                    DataRow tr_snRow = (DataRow)TR_SNSession.Value;
                    tr_sn = tr_snRow["TR_SN"].ToString();
                    kp_no = tr_snRow["CUST_KP_NO"].ToString();
                    mfr_name = tr_snRow["MFR_KP_NO"].ToString();
                    date_code = tr_snRow["DATE_CODE"].ToString();
                    lot_code = tr_snRow["Lot_Code"].ToString();
                }
                else
                {
                    if (KPNOSession != null && KPNOSession.Value != null)
                    {
                        kp_no = KPNOSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "KP_NO") != null)
                    {
                        kp_no = Station.Inputs.Find(input => input.DisplayName == "KP_NO").Value.ToString();
                    }

                    if (MFRNameSession != null && MFRNameSession.Value != null)
                    {
                        mfr_name = MFRNameSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "MFR_Name") != null)
                    {
                        mfr_name = Station.Inputs.Find(input => input.DisplayName == "MFR_Name").Value.ToString();
                    }

                    if (DateCodeSession != null && DateCodeSession.Value != null)
                    {
                        date_code = DateCodeSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "Date_Code") != null)
                    {
                        date_code = Station.Inputs.Find(input => input.DisplayName == "Date_Code").Value.ToString();
                    }

                    if (LotCodeSession != null && LotCodeSession.Value != null)
                    {
                        lot_code = LotCodeSession.Value.ToString();
                    }
                    else if (Station.Inputs.Find(input => input.DisplayName == "Lot_Code") != null)
                    {
                        lot_code = Station.Inputs.Find(input => input.DisplayName == "Lot_Code").Value.ToString();
                    }
                }

                MESStationInput DescriptionSession = Station.Inputs.Find(t => t.DisplayName == Paras[15].SESSION_TYPE);
                if (DescriptionSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                else if (DescriptionSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                description = DescriptionSession.Value.ToString();

                MESStationSession ReturnStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[16].SESSION_TYPE && t.SessionKey == Paras[16].SESSION_KEY);
                if (ReturnStationSession != null && ReturnStationSession.Value != null && ReturnStationSession.Value.ToString() != "")
                {
                    returnStation = ReturnStationSession.Value.ToString();
                    List<C_ROUTE_DETAIL> stationList = t_c_route_detail.GetLastStations(listSN.FirstOrDefault().ROUTE_ID, returnStation, Station.SFCDB);
                    if (stationList.Count == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903091835", new string[] { returnStation }));
                    }
                    currentStation = stationList[stationList.Count - 1].STATION_NAME;
                    DateTime edtiTime = Station.GetDBDateTime();
                    foreach (R_SN sn in listSN)
                    {
                        if (sn.NEXT_STATION == "REWORK" || sn.SCRAPED_FLAG == "1")
                        {
                            continue;
                        }
                        var i = Station.SFCDB.ORM.Updateable<R_SN>().UpdateColumns(rsn => new R_SN
                        {
                            CURRENT_STATION = currentStation,
                            NEXT_STATION = returnStation,
                            EDIT_EMP = Station.LoginUser.EMP_NO,
                            EDIT_TIME = edtiTime
                        }).Where(rsn => rsn.ID == sn.ID).ExecuteCommand();
                        if (i <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                        }

                        T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                        table.RecordPassStationDetail(new List<R_SN>() { sn }, "Repair", "RETURN", $"{sn.NEXT_STATION}->{returnStation}", Station.BU, Station.SFCDB);
                    }
                }

                RepairRow.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                RepairRow.REPAIR_FAILCODE_ID = failCodeID;
                RepairRow.SN = repair_no;
                RepairRow.ACTION_CODE = actionCode;
                RepairRow.SECTION_ID = section;
                RepairRow.PROCESS = process;
                RepairRow.ITEMS_ID = repairItem;
                RepairRow.ITEMS_SON_ID = repairItemSon;
                RepairRow.REASON_CODE = rootCause;
                RepairRow.DESCRIPTION = description;
                RepairRow.FAIL_LOCATION = location;
                RepairRow.FAIL_CODE = FailCodeRow.FAIL_CODE;
                RepairRow.KEYPART_SN = pcbaSN;
                RepairRow.NEW_KEYPART_SN = "";
                RepairRow.TR_SN = tr_sn;
                RepairRow.KP_NO = kp_no;
                RepairRow.MFR_NAME = mfr_name;
                RepairRow.DATE_CODE = date_code;
                RepairRow.LOT_CODE = lot_code;
                RepairRow.REPAIR_EMP = Station.LoginUser.EMP_NO;
                RepairRow.REPAIR_TIME = Station.GetDBDateTime();
                RepairRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                RepairRow.EDIT_TIME = Station.GetDBDateTime();

                string StrRes = Station.SFCDB.ExecSQL(RepairRow.GetInsertString(Station.DBType));
                if (StrRes == "1")
                {
                    Row_R_REPAIR_FAILCODE FRow = (Row_R_REPAIR_FAILCODE)RepairFailcode.GetObjByID(failCodeID, Station.SFCDB);
                    FRow.REPAIR_FLAG = "1";  //執行完維修動作後更新R_REPAIR_FAILCODE   FLAG=1 
                    FRow.EDIT_TIME = Station.GetDBDateTime();
                    updateSql = FRow.GetUpdateString(Station.DBType);
                    Station.SFCDB.ExecSQL(updateSql);
                    //for (int i = 1; i < Paras.Count; i++)
                    //{
                    //    Station.StationSession.Remove(Station.StationSession.Find(s => s.MESDataType == Paras[i].SESSION_TYPE && s.SessionKey == Paras[i].SESSION_KEY));
                    //}
                    //foreach (MESStationInput input in Station.Inputs)
                    //{
                    //    input.Value = null;
                    //}
                    Station.AddMessage("MES00000105", new string[] { repair_no, failCodeID }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000083", new string[] { repair_no, failCodeID }, StationMessageState.Fail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///PCBA維修產品所有維修動作完成Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFinishAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //SN SnObject = null;
            string UpdateSql = "";
            string result = "";
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_ACTION RepairRow = (Row_R_REPAIR_ACTION)RepairAction.NewRow();
            T_R_REPAIR_MAIN RMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            List<R_REPAIR_MAIN> RepairMainInfo = new List<R_REPAIR_MAIN>();
            List<R_REPAIR_FAILCODE> FailCodeInfo = new List<R_REPAIR_FAILCODE>();
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string DeviceName = Station.StationName;

            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //SnObject = (SN)SNSession.Value;
            List<R_SN> listSN = new List<R_SN>();
            string repair_no = "";
            if (SNSession.Value.GetType() == typeof(SN))
            {
                LogicObject.SN snObject = (LogicObject.SN)SNSession.Value;
                repair_no = snObject.SerialNo;
                listSN.Add(table.LoadData(repair_no, Station.SFCDB));
            }
            else if (SNSession.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)SNSession.Value;
                repair_no = panelObject.PanelNo;
                listSN = panelObject.GetSnDetail(repair_no, Station.SFCDB, Station.DBType);
            }


            //獲取 DEVICE1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            try
            {
                RepairMainInfo = RMain.GetRepairMainBySN(Station.SFCDB, repair_no).FindAll(r => r.CLOSED_FLAG == "0");
                if (RepairMainInfo == null || RepairMainInfo.Count == 0)
                {
                    return;
                }

                //foreach (R_REPAIR_MAIN rrm in RepairMainInfo)
                //{
                //    FailCodeInfo = RepairFailcode.CheckSNRepairFinishAction(Station.SFCDB, SnObject.SerialNo, rrm.ID);
                //    if (FailCodeInfo.Count != 0)
                //    {
                //        ///未维修完成的无法update repair_main 表信息
                //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000106", new string[] { SnObject.SerialNo, rrm.ID }));
                //    }
                //    //執行完所有的維修動作後才能更新R_REPAIR_MAIN  FLAG=1 
                //    Row_R_REPAIR_MAIN FRow = (Row_R_REPAIR_MAIN)RMain.GetObjByID(rrm.ID, Station.SFCDB);
                //    FRow.CLOSED_FLAG = "1";
                //    FRow.EDIT_TIME = Station.GetDBDateTime();
                //    UpdateSql = FRow.GetUpdateString(Station.DBType);
                //    result = Station.SFCDB.ExecSQL(UpdateSql);
                //    if (Convert.ToInt32(result) <= 0)
                //    {
                //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "REPAIR MAIN" }));
                //    }
                //}
                //table.RecordPassStationDetail(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);   //添加过站记录

                //foreach (R_Station_Output output in Station.StationOutputs)
                //{
                //    if (Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY) != null)
                //    {
                //        Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                //    }
                //}
                //foreach (MESStationInput input in Station.Inputs)
                //{
                //    if (Station.StationSession.Find(s => s.MESDataType == input.DisplayName) != null)
                //    {
                //        Station.StationSession.Find(s => s.MESDataType == input.DisplayName).Value = "";
                //    }
                //    input.Value = "";
                //}

                if (RepairMainInfo.Count > 1)
                {
                    //維修主表有多條為維修完成的記錄
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180621185023", new string[] { repair_no }));
                }

                FailCodeInfo = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where((t) => t.MAIN_ID == RepairMainInfo[0].ID && t.SN == repair_no && t.REPAIR_FLAG == "0").ToList();
                if (FailCodeInfo.Count != 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000106", new string[] { repair_no, FailCodeInfo[0].ID })); ///未维修完成的无法update repair_main 表信息
                }
                else
                {
                    foreach (R_SN r_sn in listSN)
                    {
                        table.RecordPassStationDetail(r_sn.SN, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);   //添加过站记录
                    }
                    //執行完所有的維修動作後才能更新R_REPAIR_MAIN FLAG=1
                    Row_R_REPAIR_MAIN FRow = (Row_R_REPAIR_MAIN)RMain.GetObjByID(RepairMainInfo[0].ID, Station.SFCDB);
                    FRow.CLOSED_FLAG = "1";
                    FRow.EDIT_TIME = Station.GetDBDateTime();
                    UpdateSql = FRow.GetUpdateString(Station.DBType);
                    result = Station.SFCDB.ExecSQL(UpdateSql);
                    if (Convert.ToInt32(result) > 0)
                    {
                        foreach (R_Station_Output output in Station.StationOutputs)
                        {
                            if (Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY) != null)
                            {
                                Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                            }
                        }

                        foreach (MESStationInput input in Station.Inputs)
                        {
                            if (Station.StationSession.Find(s => s.MESDataType == input.DisplayName) != null)
                            {
                                Station.StationSession.Find(s => s.MESDataType == input.DisplayName).Value = "";
                            }
                            input.Value = "";
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "REPAIR MAIN" }));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// SN掃入維修ByFailCode
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFailByFailCodeAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            if (sessionFailCode == null || sessionFailCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionFailDescription = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionFailDescription == null)
            {
                sessionFailDescription = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionFailDescription);
                if (sessionFailDescription.Value == null)
                {
                    sessionFailDescription.Value = ((C_ERROR_CODE)sessionFailCode.Value).ENGLISH_DESC;
                }
            }
            var isCheck = true;
            var isCheckTestRecord = Paras.Find(t => t.SESSION_TYPE == "CHECK_TEST_RECORD_FLAG");
            if (isCheckTestRecord == null || isCheckTestRecord.VALUE == null || isCheckTestRecord.VALUE.ToUpper() == "TRUE")
            {
                isCheck = true;
            }
            else
            {
                isCheck = false;
            }

            //如果是BPD 的話，第四個參數是不良位置            
            if (Station.LoginUser.BU.Equals("BPD"))
            {
                sessionFailLocation = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (sessionFailLocation == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
            }

            //VNDCN Ask Location at SMT1/SMT2/PTH/PTH1/PTH2/5DX Add By ZHB 2020年9月8日08:59:20
            if (Station.LoginUser.BU.Equals("VNDCN") && ("SMT1,SMT2,PTH,PTH1,PTH2,5DX").IndexOf(Station.StationName) > 0)
            {
                sessionFailLocation = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (sessionFailLocation == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
            }

            T_R_TEST_RECORD RTR = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
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
            string sn_id = r_sn.ID;
            //如果是測試工站,則SN在該工站的最後一筆測試記錄是FAIL才能掃入維修，外觀不良另外處理
            bool IsTestStation = Station.SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(r => r.MES_STATION == Station.StationName).Any();
            if (IsTestStation && isCheck)
            {
                R_TEST_RECORD testRecord = Station.SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(r => r.MESSTATION == Station.StationName && r.R_SN_ID == sn_id)
                    .OrderBy(r => r.ENDTIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                if (testRecord != null)
                {
                    if (testRecord.STATE != "FAIL" || testRecord.STATE == "F")
                    {
                        throw new Exception($@"{r_sn.SN} Last Test State Is Not Fail!");
                    }
                    if (testRecord.EDIT_TIME < r_sn.START_TIME)
                    {
                        throw new Exception($@"{r_sn.SN} Test Time Before Loading Time!");
                    }
                }
                else
                {
                    throw new Exception($@"No Test Record!");
                }
            }

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

                if (Station.BU == "FJZ" || Station.BU == "VNJUNIPER")
                {
                    try
                    {
                        var stationname = Station.StationName;
                        if (stationname == "SMT1")
                        {
                            stationname = "AOI1";
                        }
                        else if (stationname == "SMT2")
                        {
                            stationname = "AOI2";
                        }
                        //Station.SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.MES_STATION == Station.StationName).Select(t=>t.).ToList();
                        var failrecord = Station.SFCDB.ORM.Queryable<R_TEST_JUNIPER>()
                            .Where(t => t.EVENTNAME == stationname && t.STATUS == "FAIL").OrderBy(t => t.TATIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                        if (failrecord != null)
                        {
                            R_REPAIR_MAIN_EX ex = new R_REPAIR_MAIN_EX()
                            {
                                ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_REPAIR_MAIN_EX"),
                                MAIN_ID = rRepairMain.ID,
                                NAME = "UNIQUE_TEST_ID",
                                VALUE = failrecord.UNIQUE_TEST_ID,
                            };
                            Station.SFCDB.ORM.Insertable(ex).ExecuteCommand();
                            ex = new R_REPAIR_MAIN_EX()
                            {
                                ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_REPAIR_MAIN_EX"),
                                MAIN_ID = rRepairMain.ID,
                                NAME = "JUNIPER_TEST_ID",
                                VALUE = failrecord.ID,
                            };
                            Station.SFCDB.ORM.Insertable(ex).ExecuteCommand();
                        }

                    }
                    catch
                    { }
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
            rRepairFailCode.DESCRIPTION = sessionFailDescription.Value.ToString();
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
        }

        /// <summary>
        /// Panel掃入維修ByFailCode
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelFailByFailCodeAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionFailLocation = null;
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionFailCode == null || sessionFailCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionFailDescription = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionFailDescription == null)
            {
                sessionFailDescription = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionFailDescription);
                if (sessionFailDescription.Value == null)
                {
                    sessionFailDescription.Value = ((C_ERROR_CODE)sessionFailCode.Value).ENGLISH_DESC;
                }
            }

            //如果是BPD 的話，第四個參數是不良位置
            if (Station.LoginUser.BU.Equals("BPD"))
            {
                sessionFailLocation = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (sessionFailLocation == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
            }

            T_R_TEST_RECORD RTR = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_STATION_DETAIL rSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_FAILCODE tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();
            Row_R_SN row_r_sn = (Row_R_SN)t_r_sn.NewRow();
            C_ERROR_CODE failCodeObject = (C_ERROR_CODE)sessionFailCode.Value;
            string result = "";
            string repairMainID = "";
            string wo = "";
            string sku = "";
            Panel panelObject = (Panel)sessionPanel.Value;
            List<R_SN> listSNObject = panelObject.GetSnDetail(panelObject.PanelNo, Station.SFCDB, Station.DBType);
            foreach (R_SN r_sn in panelObject.PanelSnList)
            {
                //更新R_SN REPAIR_FAILED_FLAG=’1’
                wo = r_sn.WORKORDERNO;
                sku = r_sn.SKUNO;
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
            }
            List<R_REPAIR_MAIN> repairList = tRepairMain.GetRepairListSNAndStation(Station.SFCDB, panelObject.PanelNo, Station.StationName, "0");
            if (repairList == null || repairList.Count == 0)
            {
                //新增一筆到R_REPAIR_MAIN
                repairMainID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                rRepairMain.ID = repairMainID;
                rRepairMain.SN = panelObject.PanelNo;
                rRepairMain.WORKORDERNO = wo;
                rRepairMain.SKUNO = sku;
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
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219085857", new string[] { panelObject.PanelNo, Station.StationName }));
            }

            if (tRepairFailCode.FailCodeIsExist(Station.SFCDB, panelObject.PanelNo, repairMainID, failCodeObject.ERROR_CODE))
            {
                // SN:{0}在工站{1}已經錄入不良代碼{2},請不要重複錄入
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219090050", new string[] { panelObject.PanelNo, Station.StationName, failCodeObject.ERROR_CODE }));
            }
            //新增一筆到R_REPAIR_FAILCODE         
            Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
            rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
            rRepairFailCode.REPAIR_MAIN_ID = repairMainID;
            rRepairFailCode.SN = panelObject.PanelNo;
            rRepairFailCode.FAIL_CODE = failCodeObject.ERROR_CODE;
            rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
            rRepairFailCode.FAIL_TIME = Station.GetDBDateTime();
            rRepairFailCode.FAIL_CATEGORY = failCodeObject.ERROR_CATEGORY;
            rRepairFailCode.FAIL_LOCATION = sessionFailLocation == null ? "" : sessionFailLocation.Value.ToString();
            rRepairFailCode.FAIL_PROCESS = "";
            rRepairFailCode.DESCRIPTION = sessionFailDescription.Value.ToString();
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
        }




        public static void AddMultipleFailAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession FailIndex = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (FailIndex == null || FailIndex.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "FailIndex" }));
            }
            MESStationSession FailCount = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCount == null || FailCount.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "FailCount" }));
            }

            var FIndex = int.Parse(FailIndex.Value.ToString());
            var FCount = int.Parse(FailCount.Value.ToString());
            if (FIndex < (FCount - 1))
            {
                FailIndex.Value = FIndex + 1;
                MESStationInput next = Station.Inputs.Find(t => t.DisplayName == Paras[2].VALUE.ToString());
                Station.NextInput = next;
                List<MESStationInput> inputs = Station.Inputs.FindAll(t => t.SeqNo >= next.SeqNo);
                for (int i = 0; i < inputs.Count; i++)
                {
                    inputs[i].Value = "";
                }
                return;
            }


            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190318140829", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            List<MESStationSession> sessionFailCode = Station.StationSession.FindAll(t => t.MESDataType == Paras[4].SESSION_TYPE);
            if (sessionFailCode.Count < FCount)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190318140829", new string[] { Paras[4].SESSION_TYPE }));
            }
            List<MESStationSession> sessionFailDescription = Station.StationSession.FindAll(t => t.MESDataType == Paras[5].SESSION_TYPE);
            if (sessionFailDescription.Count < FCount)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190318140829", new string[] { Paras[5].SESSION_TYPE }));
            }

            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_STATION_DETAIL rSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_FAILCODE tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_REPAIR_MAIN rRepairMain = new R_REPAIR_MAIN();
            Row_R_SN row_r_sn = (Row_R_SN)t_r_sn.NewRow();
            int result = 0;
            string repairMainID = "";
            string strsn = sessionSN.Value.ToString();
            //更新R_SN REPAIR_FAILED_FLAG=’1’
            OleExec DB = Station.SFCDB;
            R_SN sn = DB.ORM.Queryable<R_SN>().Where((s) => s.SN == strsn && s.VALID_FLAG == "1").First();
            sn.REPAIR_FAILED_FLAG = "1";
            sn.EDIT_EMP = Station.LoginUser.EMP_NO;
            sn.EDIT_TIME = Station.GetDBDateTime();
            result = DB.ORM.Updateable<R_SN>(sn).ExecuteCommand();
            if (result == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
            }

            //新增一筆FAIL記錄到R_SN_STATION_DETAIL
            R_SN_STATION_DETAIL snd = new R_SN_STATION_DETAIL();
            snd.ID = rSnStationDetail.GetNewID(Station.BU, Station.SFCDB);
            snd.R_SN_ID = sn.ID;
            snd.SN = sn.SN;
            snd.SKUNO = sn.SKUNO;
            snd.WORKORDERNO = sn.WORKORDERNO;
            snd.PLANT = sn.PLANT;
            snd.CLASS_NAME = Common.GetWorkClass(DB);
            snd.ROUTE_ID = sn.ROUTE_ID;
            snd.LINE = Station.Line;
            snd.STARTED_FLAG = sn.STARTED_FLAG;
            snd.START_TIME = sn.START_TIME;
            snd.PACKED_FLAG = sn.PACKED_FLAG;
            snd.PACKED_TIME = sn.PACKDATE;
            snd.COMPLETED_FLAG = sn.COMPLETED_FLAG;
            snd.COMPLETED_TIME = sn.COMPLETED_TIME;
            snd.SHIPPED_FLAG = sn.SHIPPED_FLAG;
            snd.SHIPDATE = sn.SHIPDATE;
            snd.REPAIR_FAILED_FLAG = sn.REPAIR_FAILED_FLAG;
            snd.CURRENT_STATION = sn.CURRENT_STATION;
            snd.NEXT_STATION = sn.NEXT_STATION;
            snd.PO_NO = sn.PO_NO;
            snd.CUST_ORDER_NO = sn.CUST_ORDER_NO;
            snd.CUST_PN = sn.CUST_PN;
            snd.BOXSN = sn.BOXSN;
            snd.DEVICE_NAME = Station.DisplayName;
            snd.STATION_NAME = Station.StationName;
            snd.SCRAPED_FLAG = sn.SCRAPED_FLAG;
            snd.SCRAPED_TIME = sn.SCRAPED_TIME;
            snd.PRODUCT_STATUS = sn.PRODUCT_STATUS;
            snd.REWORK_COUNT = sn.REWORK_COUNT;
            snd.VALID_FLAG = sn.VALID_FLAG;
            snd.EDIT_EMP = sn.EDIT_EMP;
            snd.EDIT_TIME = sn.EDIT_TIME;
            result = DB.ORM.Insertable<R_SN_STATION_DETAIL>(snd).ExecuteCommand();
            if (result == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "STATION_DETAIL" }));
            }
            DateTime createTime = Station.GetDBDateTime();
            List<R_REPAIR_MAIN> repairList = tRepairMain.GetRepairListSNAndStation(Station.SFCDB, sn.SN, Station.StationName, "0");
            if (repairList == null || repairList.Count == 0)
            {
                //新增一筆到R_REPAIR_MAIN
                repairMainID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                rRepairMain.ID = repairMainID;
                rRepairMain.SN = sn.SN;
                rRepairMain.WORKORDERNO = sn.WORKORDERNO;
                rRepairMain.SKUNO = sn.SKUNO;
                rRepairMain.FAIL_LINE = Station.Line;
                rRepairMain.FAIL_STATION = Station.StationName;
                rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                rRepairMain.FAIL_TIME = Station.GetDBDateTime();
                rRepairMain.CREATE_TIME = createTime;
                rRepairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                rRepairMain.EDIT_TIME = Station.GetDBDateTime();
                rRepairMain.CLOSED_FLAG = "0";
                result = DB.ORM.Insertable<R_REPAIR_MAIN>(rRepairMain).ExecuteCommand();
                if (!(Convert.ToInt32(result) > 0))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "REPAIR_MAIN" }));
                }
            }
            else if (repairList.Count == 1)
            {
                repairMainID = repairList[0].ID;
                createTime = repairList[0].CREATE_TIME.Value;
            }
            else
            {
                // SN:{0}在工站{1}有多筆未維修記錄
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219085857", new string[] { sn.SN, Station.StationName }));
            }

            //新增一筆到R_REPAIR_FAILCODE     
            for (int i = 0; i < FCount; i++)
            {
                C_ERROR_CODE failcode = (C_ERROR_CODE)sessionFailCode[i].Value;
                if (tRepairFailCode.FailCodeIsExist(Station.SFCDB, sn.SN, repairMainID, failcode.ERROR_CODE))
                {
                    // SN:{0}在工站{1}已經錄入不良代碼{2},請不要重複錄入
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219090050", new string[] { sn.SN, Station.StationName, failcode.ERROR_CODE }));
                }

                R_REPAIR_FAILCODE rRepairFailCode = new R_REPAIR_FAILCODE();
                rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
                rRepairFailCode.MAIN_ID = repairMainID;
                rRepairFailCode.SN = sn.SN;
                rRepairFailCode.FAIL_CODE = failcode.ERROR_CODE;
                rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
                rRepairFailCode.FAIL_TIME = Station.GetDBDateTime();
                rRepairFailCode.F_CATEGORY = failcode.ERROR_CATEGORY;
                rRepairFailCode.F_LOCATION = "";
                rRepairFailCode.F_PROCESS = "";
                rRepairFailCode.DESCRIPTION = failcode.ENGLISH_DESC;
                rRepairFailCode.REPAIR_FLAG = "0";
                rRepairFailCode.CREATE_TIME = createTime;
                rRepairFailCode.EDIT_EMP = Station.LoginUser.EMP_NO;
                rRepairFailCode.EDIT_TIME = Station.GetDBDateTime();
                result = DB.ORM.Insertable<R_REPAIR_FAILCODE>(rRepairFailCode).ExecuteCommand();
                if (!(Convert.ToInt32(result) > 0))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "FAILCODE" }));
                }
            }
            for (int i = 0; i < Station.Inputs.Count; i++)
            {
                Station.Inputs[i].Value = "";
            }
            Station.NextInput = Station.Inputs[0];
            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
        }

        //產品維修CheckIn Action ByPassWord 
        public static void SNInByPassWordRepairAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSendEmp = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSendEmp == null || sessionSendEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionSendPW = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSendPW == null || sessionSendPW.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionReceiveEmp = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionReceiveEmp == null || sessionReceiveEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession sessionReceivePW = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionReceivePW == null || sessionReceivePW.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE }));
            }

            T_c_user t_c_user = new T_c_user(Station.SFCDB, Station.DBType);
            Row_c_user rowSendUser = t_c_user.getC_Userbyempno(sessionSendEmp.Value.ToString(), Station.SFCDB, Station.DBType);
            if (rowSendUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionSendEmp.Value.ToString() }));
            }
            if (!rowSendUser.EMP_PASSWORD.Equals(sessionSendPW.Value.ToString()))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180813154717", new string[] { sessionSendEmp.Value.ToString() }));
            }

            Row_c_user rowReceiveUser = t_c_user.getC_Userbyempno(sessionReceiveEmp.Value.ToString(), Station.SFCDB, Station.DBType);
            if (rowReceiveUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionReceiveEmp.Value.ToString() }));
            }
            if (!rowReceiveUser.EMP_PASSWORD.Equals(sessionReceivePW.Value.ToString()))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180813154717", new string[] { sessionReceivePW.Value.ToString() }));
            }
            //SN snObject = (SN)sessionSN.Value;
            string repairNo = "";
            string repairWorkorderNo = "";
            string repairSkuNo = "";
            string repairCurrentStation = "";
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);

            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                repairNo = snObject.SerialNo;
                repairWorkorderNo = snObject.WorkorderNo;
                repairSkuNo = snObject.SkuNo;
                repairCurrentStation = snObject.NextStation;
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                repairNo = panelObject.PanelNo;
                panelObject.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                repairWorkorderNo = panelObject.PanelSnList.FirstOrDefault().WORKORDERNO;
                repairSkuNo = panelObject.PanelSnList.FirstOrDefault().SKUNO;
                repairCurrentStation = panelObject.PanelSnList.FirstOrDefault().NEXT_STATION;
            }
            else if (sessionSN.Value.GetType() == typeof(string))
            {
                R_SN objSN = TRS.LoadData(sessionSN.Value.ToString(), Station.SFCDB);
                if (objSN != null)
                {
                    repairNo = objSN.SN;
                    repairWorkorderNo = objSN.WORKORDERNO;
                    repairSkuNo = objSN.SKUNO;
                    repairCurrentStation = objSN.NEXT_STATION;
                }
                else if (TRPS.CheckPanelExist(sessionSN.Value.ToString(), Station.SFCDB))//輸入的是一個Panel
                {
                    Panel panelO = new Panel(sessionSN.Value.ToString(), Station.SFCDB, Station.DBType);
                    repairNo = panelO.PanelNo;
                    panelO.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                    repairWorkorderNo = panelO.PanelSnList.FirstOrDefault().WORKORDERNO;
                    repairSkuNo = panelO.PanelSnList.FirstOrDefault().SKUNO;
                    repairCurrentStation = panelO.PanelSnList.FirstOrDefault().NEXT_STATION;
                }
                else
                {
                    throw new Exception("Input Type Error");
                }
            }
            else
            {
                throw new Exception("Input Type Error");
            }

            T_R_REPAIR_MAIN rRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            List<R_REPAIR_MAIN> RepariMainList = rRepairMain.GetRepairMainBySN(Station.SFCDB, repairNo);
            R_REPAIR_MAIN rMain = RepariMainList.Where(r => r.CLOSED_FLAG == "0").FirstOrDefault();  // Find(r => r.CLOSED_FLAG == "0");
            if (rMain != null)
            {
                T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
                Row_R_REPAIR_TRANSFER rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.NewRow();
                rowTransfer.ID = rTransfer.GetNewID(Station.BU, Station.SFCDB);
                rowTransfer.REPAIR_MAIN_ID = rMain.ID;
                rowTransfer.IN_SEND_EMP = sessionSendEmp.Value.ToString();
                rowTransfer.IN_RECEIVE_EMP = sessionReceiveEmp.Value.ToString();
                rowTransfer.IN_TIME = Station.GetDBDateTime();
                rowTransfer.SN = repairNo;
                rowTransfer.LINE_NAME = Station.Line;
                rowTransfer.STATION_NAME = Station.BU == "VNDCN" ? rMain.FAIL_STATION : repairCurrentStation;//NormalBonepile中會用掃Fail工站判斷是否CheckIn/Out，假如是COSMETIC-FAILURE掃的Fail，這裡選擇SN下一站就不合適了
                rowTransfer.WORKORDERNO = repairWorkorderNo;
                rowTransfer.SKUNO = repairSkuNo;
                rowTransfer.CLOSED_FLAG = "0";
                rowTransfer.CREATE_TIME = Station.GetDBDateTime();
                rowTransfer.DESCRIPTION = "";
                rowTransfer.EDIT_TIME = Station.GetDBDateTime();
                rowTransfer.EDIT_EMP = sessionReceiveEmp.Value.ToString();
                string strRet = (Station.SFCDB).ExecSQL(rowTransfer.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000037", new string[] { "INSET R_REPAIR_TRANSFER" }, StationMessageState.Pass);
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { repairNo, "CLOSED" }));
            }
        }

        //產品維修CheckOut Action ByPassWord 
        public static void SNOutByPassWordRepairAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSendEmp = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSendEmp == null || sessionSendEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionSendPW = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSendPW == null || sessionSendPW.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionReceiveEmp = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionReceiveEmp == null || sessionReceiveEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            MESStationSession sessionReceivePW = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionReceivePW == null || sessionReceivePW.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE }));
            }

            T_c_user t_c_user = new T_c_user(Station.SFCDB, Station.DBType);
            Row_c_user rowSendUser = t_c_user.getC_Userbyempno(sessionSendEmp.Value.ToString(), Station.SFCDB, Station.DBType);
            if (rowSendUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionSendEmp.Value.ToString() }));
            }
            if (!rowSendUser.EMP_PASSWORD.Equals(sessionSendPW.Value.ToString()))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180813154717", new string[] { sessionSendEmp.Value.ToString() }));
            }

            Row_c_user rowReceiveUser = t_c_user.getC_Userbyempno(sessionReceiveEmp.Value.ToString(), Station.SFCDB, Station.DBType);
            if (rowReceiveUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionReceiveEmp.Value.ToString() }));
            }
            if (!rowReceiveUser.EMP_PASSWORD.Equals(sessionReceivePW.Value.ToString()))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180813154717", new string[] { sessionReceivePW.Value.ToString() }));
            }

            //SN snObject = (SN)sessionSN.Value;
            string repairNo = "";
            List<R_SN> listSNObject = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                repairNo = snObject.SerialNo;
                listSNObject.Add(t_r_sn.LoadData(snObject.SerialNo, Station.SFCDB));
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                repairNo = panelObject.PanelNo;
                panelObject.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                listSNObject = panelObject.PanelSnList;
            }
            else if (sessionSN.Value.GetType() == typeof(string))
            {
                R_SN objSN = TRS.LoadData(sessionSN.Value.ToString(), Station.SFCDB);
                if (objSN != null)
                {
                    repairNo = objSN.SN;
                    listSNObject.Add(t_r_sn.LoadData(repairNo, Station.SFCDB));
                }
                else if (TRPS.CheckPanelExist(sessionSN.Value.ToString(), Station.SFCDB))//輸入的是一個Panel
                {
                    Panel panelO = new Panel(sessionSN.Value.ToString(), Station.SFCDB, Station.DBType);
                    repairNo = panelO.PanelNo;

                    panelO.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                    listSNObject = panelO.PanelSnList;
                }
                else
                {
                    throw new Exception("Input Type Error");
                }
            }
            else
            {
                throw new Exception("Input Type Error");
            }


            T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_TRANSFER rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.NewRow();
            //T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);

            List<R_REPAIR_TRANSFER> transferList = rTransfer.GetLastRepairedBySN(repairNo, Station.SFCDB);
            R_REPAIR_TRANSFER rRepairTransfer = transferList.Where(r => r.CLOSED_FLAG == "0").FirstOrDefault();//TRANSFER表 1 表示不良
            if (rRepairTransfer != null)
            {
                rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.GetObjByID(rRepairTransfer.ID, Station.SFCDB);
                rowTransfer.CLOSED_FLAG = "1";
                rowTransfer.OUT_TIME = Station.GetDBDateTime();
                rowTransfer.OUT_SEND_EMP = sessionSendEmp.Value.ToString();
                rowTransfer.OUT_RECEIVE_EMP = sessionReceiveEmp.Value.ToString();

                string strRet = (Station.SFCDB).ExecSQL(rowTransfer.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000035", new string[] { strRet }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000025", new string[] { "REPAIR TRANSFER" }, StationMessageState.Pass);
                }
                foreach (R_SN sn in listSNObject)
                {
                    Row_R_SN rowSN = (Row_R_SN)t_r_sn.GetObjByID(sn.ID, Station.SFCDB);
                    rowSN.REPAIR_FAILED_FLAG = "0";
                    strRet = (Station.SFCDB).ExecSQL(rowSN.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    if (Convert.ToInt32(strRet) > 0)
                    {
                        Station.AddMessage("MES00000035", new string[] { strRet }, StationMessageState.Pass);
                    }
                    else
                    {
                        Station.AddMessage("MES00000025", new string[] { "R_SN" }, StationMessageState.Pass);
                    }
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { repairNo, "abnormal" }));
            }
        }

        /// <summary>
        /// Add New Fail Code
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddNewFailCodeAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
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
            if (sessionFailCode == null || sessionFailCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string result;
            SN snObject = (SN)sessionSN.Value;
            C_ERROR_CODE errorCodeObject = (C_ERROR_CODE)sessionFailCode.Value;
            Station.SFCDB.ThrowSqlExeception = true;
            T_R_REPAIR_FAILCODE tRepairFailCode = null;
            Row_R_REPAIR_FAILCODE rRepairFailCode = null;
            List<R_REPAIR_MAIN> repairMainObject = new List<R_REPAIR_MAIN>();
            T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);

            repairMainObject = t_r_repair_main.GetRepairMainBySN(Station.SFCDB, snObject.SerialNo).FindAll(r => r.CLOSED_FLAG == "0");
            if (repairMainObject == null || repairMainObject.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", snObject.SerialNo }));
            }
            if (repairMainObject.Count > 1)
            {
                //維修主表有多條為維修完成的記錄
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180621185023", new string[] { snObject.SerialNo }));
            }

            //repairMainObject = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN>().Where(r => r.CLOSED_FLAG == "0" && r.SN == snObject.SerialNo).ToList().FirstOrDefault();

            //新增一筆到R_REPAIR_FAILCODE           
            tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
            rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
            rRepairFailCode.REPAIR_MAIN_ID = repairMainObject[0].ID;
            rRepairFailCode.SN = snObject.SerialNo;
            rRepairFailCode.FAIL_CODE = errorCodeObject.ERROR_CODE;
            rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
            rRepairFailCode.FAIL_TIME = Station.GetDBDateTime();
            rRepairFailCode.FAIL_CATEGORY = errorCodeObject.ERROR_CATEGORY;
            rRepairFailCode.FAIL_LOCATION = "";
            rRepairFailCode.FAIL_PROCESS = "";
            rRepairFailCode.DESCRIPTION = errorCodeObject.CHINESE_DESC;
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


        }


        /// <summary>
        /// 保存替换记录
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairSaveReplace(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            string failCodeID = "";
            string actionCode = "";
            string errorCode = "";
            string rootCause = "";

            string returnStation = "";
            string currentStation = "";

            string Partno = "";
            string OriginalSN = "";
            string ReplaceSN = "";

            DateTime edtiTime = Station.GetDBDateTime();

            string description = "";

            R_REPAIR_FAILCODE FailCodeRow;
            T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            R_REPAIR_ACTION action = new R_REPAIR_ACTION();
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            Station.SFCDB.ThrowSqlExeception = true;

            //added portion to remove test file from Oracle server vince_20191111
            //int remove_flag = Delete("C:\\Users\\vince_wu\\Desktop\\MFG2Project\\Oracle", "/Vince_Test_FTP", "TEST.SNBOM", "10.18.142.111", "22", "Vwu", "14days");
            //downloadFile();
            //string host = @"10.18.142.111";
            //int port = 22;
            //string username = "Vwu";
            //string password = @"14days";
            //string sftpPath = "Vince_Test_FTP";
            //string SNTestReslut = "TestAgain.txt";
            string host = @"10.18.155.17";
            int port = 22;
            string username = "bomuser";
            string password = @"4bomuser!";
            string sftpPath = @"/var/apache2/htdocs/FOCSFC/BOM";

            //Loop to find test result file and delete from TDMS server if needed vince_20191205
            //List<SftpFile> fl = GetFileToDelete(host, port, username, password, sftpPath, SNTestReslut);

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                //獲取到 SN 對象
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SnObject = (SN)SNSession.Value;

                MESStationSession FailCodeIDSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (FailCodeIDSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else if (FailCodeIDSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                failCodeID = FailCodeIDSession.Value.ToString();

                if (failCodeID != "")
                {
                    FailCodeRow = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where((f) => f.ID == failCodeID).ToList().First<R_REPAIR_FAILCODE>();
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, failCodeID }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }

                MESStationSession ActionCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (ActionCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                else if (ActionCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                actionCode = ActionCodeSession.Value.ToString();

                MESStationSession ErrorCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (ErrorCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else if (ErrorCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                errorCode = ErrorCodeSession.Value.ToString();

                MESStationSession RootCauseSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (RootCauseSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                else if (RootCauseSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                rootCause = RootCauseSession.Value.ToString();

                if (actionCode == "A12")
                {
                    MESStationSession PartNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
                    if (PartNoSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                    }
                    else if (PartNoSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                    }
                    Partno = PartNoSession.Value.ToString();

                    MESStationSession OriginalSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
                    if (OriginalSNSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                    }
                    else if (OriginalSNSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                    }
                    OriginalSN = OriginalSNSession.Value.ToString();

                    MESStationSession ReplaceSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
                    if (ReplaceSNSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
                    }
                    else if (ReplaceSNSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
                    }
                    ReplaceSN = ReplaceSNSession.Value.ToString();

                    MESStationSession KPRULE = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
                    if (KPRULE == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                    }
                    else if (KPRULE.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                    }

                    MESStationSession MPNRULE = Station.StationSession.Find(t => t.MESDataType == Paras[11].SESSION_TYPE && t.SessionKey == Paras[11].SESSION_KEY);
                    if (MPNRULE == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                    }
                    else if (MPNRULE.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                    }

                    MESStationSession ReplaceMPN = Station.StationSession.Find(t => t.MESDataType == Paras[12].SESSION_TYPE && t.SessionKey == Paras[12].SESSION_KEY);
                    if (OriginalSN != null && ReplaceSN != null)
                    {
                        List<R_SN_KP> kps = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((r) => r.VALUE == OriginalSN && r.VALID_FLAG == 1).ToList();
                        if (kps.Count > 0)
                        {
                            R_SN_KP kp = kps[0];
                            kp.VALUE = ReplaceSN;
                            kp.REGEX = KPRULE.InputValue;
                            kp.EDIT_EMP = Station.LoginUser.EMP_NO;
                            kp.EDIT_TIME = edtiTime;
                            var i = Station.SFCDB.ORM.Updateable<R_SN_KP>(kp).ExecuteCommand();

                            if (ReplaceMPN != null && ReplaceMPN.Value != null)
                            {
                                i = Station.SFCDB.ORM.Updateable<R_SN_KP>().UpdateColumns(rsn => new R_SN_KP
                                {
                                    VALUE = ReplaceMPN.Value.ToString(),
                                    REGEX = MPNRULE.InputValue,
                                    EDIT_EMP = Station.LoginUser.EMP_NO,
                                    EDIT_TIME = edtiTime
                                }).Where(rsn => rsn.R_SN_ID == kp.R_SN_ID && rsn.PARTNO == kp.PARTNO && rsn.ITEMSEQ == kp.ITEMSEQ && rsn.DETAILSEQ == (kp.DETAILSEQ - 1) && (rsn.SCANTYPE == "MPN" || rsn.SCANTYPE == "PN") && (rsn.EXVALUE1 == kp.EXVALUE1 || SqlSugar.SqlFunc.IsNullOrEmpty(rsn.EXVALUE1))).ExecuteCommand();
                            }
                            /*Mark by James*/
                            //if (i > 0)
                            //{
                            //    T_R_SN_KP tkp = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            //    kp.ID = tkp.GetNewID(Station.BU, Station.SFCDB);
                            //    kp.VALUE = ReplaceSN;
                            //    kp.EDIT_EMP = Station.LoginUser.EMP_NO;
                            //    kp.EDIT_TIME = edtiTime;
                            //    var n = Station.SFCDB.ORM.Insertable<R_SN_KP>(kp).ExecuteCommand();
                            //    if (n < 0)
                            //    {
                            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { " Replace KP"}));
                            //    }
                            //}
                            //else
                            //{
                            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000197", new string[] { " R_SN_KP", "Original KP" }));
                            //}
                        }
                    }
                }
                else
                {
                    ReplaceSN = "";
                    OriginalSN = "";
                    Partno = "";
                }
                MESStationSession DescriptionSession = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
                if (DescriptionSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                else if (DescriptionSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                description = DescriptionSession.Value.ToString();

                MESStationSession ReturnStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
                if (ReturnStationSession != null && ReturnStationSession.Value != null && ReturnStationSession.Value.ToString() != "")
                {
                    returnStation = ReturnStationSession.Value.ToString();
                    List<C_ROUTE_DETAIL> stationList = t_c_route_detail.GetLastStations(SnObject.RouteID, returnStation, Station.SFCDB);
                    if (stationList.Count == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903091835", new string[] { returnStation }));
                    }
                    currentStation = stationList[stationList.Count - 1].STATION_NAME;
                    R_SN sn = Station.SFCDB.ORM.Queryable<R_SN>().Where(s => s.ID == SnObject.ID).First();
                    sn.CURRENT_STATION = currentStation;
                    sn.NEXT_STATION = returnStation;
                    sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                    sn.EDIT_TIME = edtiTime;
                    var i = Station.SFCDB.ORM.Updateable<R_SN>(sn).ExecuteCommand();
                    if (i <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                    }
                }

                action.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                action.R_FAILCODE_ID = failCodeID;
                action.SN = SnObject.SerialNo;
                action.ACTION_CODE = actionCode;

                action.REASON_CODE = rootCause;
                action.DESCRIPTION = description;
                action.FAIL_CODE = FailCodeRow.FAIL_CODE;
                action.KEYPART_SN = OriginalSN;
                action.NEW_KEYPART_SN = ReplaceSN;
                action.KP_NO = Partno;
                action.NEW_KP_NO = Partno;

                action.REPAIR_EMP = Station.LoginUser.EMP_NO;
                action.REPAIR_TIME = Station.GetDBDateTime();
                action.EDIT_EMP = Station.LoginUser.EMP_NO;
                action.EDIT_TIME = Station.GetDBDateTime();

                int Res = Station.SFCDB.ORM.Insertable<R_REPAIR_ACTION>(action).ExecuteCommand();
                if (Res == 1)
                {
                    R_REPAIR_FAILCODE FRow = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where((f) => f.ID == failCodeID).ToList().First<R_REPAIR_FAILCODE>();
                    FRow.REPAIR_FLAG = "1";  //執行完維修動作後更新R_REPAIR_FAILCODE   FLAG=1 
                    FRow.EDIT_TIME = Station.GetDBDateTime();
                    Station.SFCDB.ORM.Updateable<R_REPAIR_FAILCODE>(FRow).ExecuteCommand();

                    Station.AddMessage("MES00000105", new string[] { SnObject.SerialNo, failCodeID }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000083", new string[] { SnObject.SerialNo, failCodeID }, StationMessageState.Fail);
                }

                //vince_20191022 added logic to check and update HIPOT and tesing result ---start
                //T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                R_SN R_SN_Data = t_r_sn.LoadSN(SnObject.SerialNo, Station.SFCDB);
                string str_skuno = R_SN_Data.SKUNO;
                MESDataObject.Module.T_R_WO_BASE O_TWO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE row_r_wo_base = O_TWO.LoadWorkorder(R_SN_Data.WORKORDERNO, Station.SFCDB);
                string plantO = row_r_wo_base.PLANT;
                if (plantO == "TOGA")
                {
                    T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
                    var SnRemark = "--" + SnObject.SerialNo;
                    if (ReturnStationSession.Value.ToString() == "HIPOT")
                    {
                        //if return to Hipot, reverse all including Hipot FSC, FST, UPGRADEKIT test record vince_20191008
                        Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.STATE.ToUpper() == "PASS" && t.SN == SnObject.SerialNo).ExecuteCommand();
                        //delete SNBOM file from TDMS server vince_20191205
                        string SNBOMSN = SnObject.SerialNo.ToString() + ".BOM";
                        DeleteFile(host, port, username, password, sftpPath, SNBOMSN);
                    }
                    T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                    List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.ORAGetPreviousByRouteId(SnObject.RouteID, SnObject.NextStation, Station.SFCDB);
                    var stationList = RouteDetails.Select(t => t.STATION_NAME).ToArray();
                    T_C_SKU t_c_sku = new T_C_SKU(Station.SFCDB, Station.DBType);
                    string PF = t_c_sku.GetSku(SnObject.SkuNo, Station.SFCDB).SKU_NAME;
                    if (stationList.Contains("SFT") && ReturnStationSession.Value.ToString() == "HIPOT")
                    {
                        // if 2C, need to update both BBT for chassis and SFT for SM
                        //get SM SNs from 2C chassis
                        T_R_SN_KP TRKP = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                        List<R_SN_KP> SMODSN = TRKP.GetSMODSN(SnObject.SerialNo, Station.SFCDB);

                        Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "SFT" && t.STATE.ToUpper() == "PASS" && t.SN == SnObject.SerialNo).ExecuteCommand();
                        if (PF.Contains("2C")) //should alwasy get 2 SMODs
                        {
                            foreach (var a in SMODSN)
                            {
                                var SMSnRemark = "--" + a.VALUE;
                                Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SMSnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "SFT" && t.STATE.ToUpper() == "PASS" && t.SN == a.VALUE).ExecuteCommand();

                                //delete SNBOM file from TDMS server vince_20191205
                                string SNBOMSN = a.SN.ToString() + ".BOM";
                                DeleteFile(host, port, username, password, sftpPath, SNBOMSN);
                            }
                            //for 2C chassis
                            Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "BBT" && t.STATE.ToUpper() == "PASS" && t.SN == SnObject.SerialNo).ExecuteCommand();
                        }
                    }

                }
                //vince_20191022 end
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Add download and delete test result file funtion connect to oracle server vince_20191108
        public static void downloadFile()
        {
            string host = @"10.18.142.111";
            string username = "Vwu";
            string password = @"14days";

            // Path to file on SFTP server
            string pathRemoteFile = "/Vince_Test_FTP/TEST.SNBOM";
            // Path where the file should be saved once downloaded (locally)
            string pathLocalFile = "C:\\Users\\vince_wu\\Desktop\\TestSFTP\\TEST.SNBOM";

            using (SftpClient sftp = new SftpClient(host, username, password))
            {
                try
                {
                    sftp.Connect();

                    Console.WriteLine("Downloading {0}", pathRemoteFile);

                    using (Stream fileStream = File.OpenWrite(pathLocalFile))
                    {
                        sftp.DownloadFile(pathRemoteFile, fileStream);
                    }

                    sftp.Disconnect();
                }
                catch (Exception er)
                {
                    Console.WriteLine("An exception has been caught " + er.ToString());
                }
            }
        }

        public static void DeleteFile(string host, int port, string username, string password, string sftpPath, string SNBOMSN)
        {
            try
            {
                using (SftpClient sftpClient = new SftpClient(host, port, username, password))
                {
                    sftpPath = sftpPath + "/" + SNBOMSN;
                    sftpClient.Connect();
                    sftpClient.DeleteFile(sftpPath);
                    sftpClient.Disconnect();
                }
            }
            catch (Exception er)
            {
                Console.WriteLine("An exception has been caught " + er.ToString());
            }
        }
        public static List<Renci.SshNet.Sftp.SftpFile> GetFileToDelete(string host, int port, string username, string password, string sftpPath, string SNTestReslut)
        {
            List<Renci.SshNet.Sftp.SftpFile> fList = new List<Renci.SshNet.Sftp.SftpFile>();
            try
            {
                using (SftpClient sftpClient = new SftpClient(host, port, username, password))
                {
                    sftpClient.Connect();
                    var fileList = sftpClient.ListDirectory(sftpPath);

                    // Display all the files.
                    //var files = sftpClient.ListDirectory("TestAgain.txt");
                    foreach (var f in fileList)
                    {
                        if (f.Name != "." && f.Name != "..")
                        {
                            // Get folders
                            if (f.IsDirectory)
                            {
                                SftpFile sftpDir = f;
                                IEnumerable<SftpFile> fl = GetFilesRecur(sftpClient, sftpDir, SNTestReslut);
                                foreach (var DeleteFile in fl)
                                {
                                    fList.Add(DeleteFile);
                                    if (DeleteFile.FullName.ToString().Contains(SNTestReslut))
                                    {
                                        string DeleteFilePath = DeleteFile.FullName.ToString();
                                        sftpClient.DeleteFile(DeleteFilePath);
                                    }
                                }

                            }
                            if (f.IsRegularFile)
                            {
                                fList.Add(f);
                            }
                        }
                    }
                    sftpClient.Disconnect();
                }
            }
            catch (Exception er)
            {
                Console.WriteLine("An exception has been caught " + er.ToString());
            }
            return fList;
        }

        private static IEnumerable<SftpFile> GetFilesRecur(SftpClient ssh, SftpFile sftpDir, string SNTestReslut)
        {
            if (!sftpDir.IsDirectory)
            {
                return new[] { sftpDir };
            }
            var fl = new List<SftpFile>();
            foreach (var sftpFile in ssh.ListDirectory(sftpDir.FullName))
            {
                if (sftpFile.IsRegularFile)
                {
                    if (sftpFile.FullName.ToString().Contains(SNTestReslut))
                    {
                        fl.Add(sftpFile);
                    }
                }
                else if (sftpFile.IsDirectory && sftpFile.Name != "." && sftpFile.Name != "..")
                {
                    fl.AddRange(GetFilesRecur(ssh, sftpFile, SNTestReslut));
                }
            }
            return fl;
        }

        /// <summary>
        /// PCBA維修SaveReplace動作
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PCBARepairSaveReplaceAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            try
            {
                #region 1.獲取Session中的對象
                //獲取SN對象
                MESStationSession sSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (sSN == null || sSN.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SN objSN = (SN)sSN.Value;

                //獲取FailCodeID對象
                MESStationSession sFailCodeID = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (sFailCodeID == null || sFailCodeID.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                string failCodeID = sFailCodeID.Value.ToString();

                //獲取ActionCode對象
                MESStationSession sActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (sActionCode == null || sActionCode.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                string actionCode = sActionCode.Value.ToString();

                //獲取PCBASN對象
                MESStationSession sPCBASN = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                //獲取Location對象
                MESStationSession sLocation = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                //獲取Category對象
                MESStationSession sCategory = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
                //獲取RootCause對象
                MESStationSession sRootCause = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
                if (sRootCause == null || sRootCause.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                string rootCause = sRootCause.Value.ToString();

                //獲取KPNO對象
                MESStationSession sKPNO = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
                //獲取Process對象
                MESStationSession sProcess = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
                //獲取IsPCBA對象
                MESStationSession sIsPCBA = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
                //獲取OriginalCSN對象
                MESStationSession sOriginalCSN = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
                //獲取ReplaceCSN對象
                MESStationSession sReplaceCSN = Station.StationSession.Find(t => t.MESDataType == Paras[11].SESSION_TYPE && t.SessionKey == Paras[11].SESSION_KEY);
                //獲取OldTRSN對象
                MESStationSession sOldTRSN = Station.StationSession.Find(t => t.MESDataType == Paras[12].SESSION_TYPE && t.SessionKey == Paras[12].SESSION_KEY);
                //獲取TRSN對象
                MESStationSession sTRSN = Station.StationSession.Find(t => t.MESDataType == Paras[13].SESSION_TYPE && t.SessionKey == Paras[13].SESSION_KEY);
                //獲取NewDC對象
                MESStationSession sNewDC = Station.StationSession.Find(t => t.MESDataType == Paras[14].SESSION_TYPE && t.SessionKey == Paras[14].SESSION_KEY);
                //獲取NewLC對象
                MESStationSession sNewLC = Station.StationSession.Find(t => t.MESDataType == Paras[15].SESSION_TYPE && t.SessionKey == Paras[15].SESSION_KEY);
                //獲取NewMfrCode對象
                MESStationSession sNewMfrCode = Station.StationSession.Find(t => t.MESDataType == Paras[16].SESSION_TYPE && t.SessionKey == Paras[16].SESSION_KEY);
                //獲取NewMfrName對象
                MESStationSession sNewMfrName = Station.StationSession.Find(t => t.MESDataType == Paras[17].SESSION_TYPE && t.SessionKey == Paras[17].SESSION_KEY);
                //獲取ReturnStation對象
                MESStationSession sReturnStation = Station.StationSession.Find(t => t.MESDataType == Paras[18].SESSION_TYPE && t.SessionKey == Paras[18].SESSION_KEY);
                if (sReturnStation == null || sReturnStation.Value.ToString() == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[18].SESSION_TYPE + Paras[18].SESSION_KEY }));
                }
                string returnStation = sReturnStation.Value.ToString();
                //獲取CompomentID對象
                MESStationSession sCompomentID = Station.StationSession.Find(t => t.MESDataType == Paras[19].SESSION_TYPE && t.SessionKey == Paras[19].SESSION_KEY);
                //獲取OldMPN對象
                MESStationSession sOldMPN = Station.StationSession.Find(t => t.MESDataType == Paras[20].SESSION_TYPE && t.SessionKey == Paras[20].SESSION_KEY);
                //獲取NewMPN對象
                MESStationSession sNewMPN = Station.StationSession.Find(t => t.MESDataType == Paras[21].SESSION_TYPE && t.SessionKey == Paras[21].SESSION_KEY);
                #endregion

                #region 2.空值判斷
                string kpNo = string.Empty;
                string location = string.Empty;
                string originalCSN = string.Empty;
                string replaceCSN = string.Empty;
                if (actionCode == "A12" || rootCause == "E206")
                {
                    if (sLocation == null || string.IsNullOrEmpty(sLocation.Value.ToString()))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                    }
                    location = sLocation.Value.ToString();

                    if (sKPNO == null || string.IsNullOrEmpty(sKPNO.Value.ToString()))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
                    }
                    kpNo = sKPNO.Value.ToString();

                    if (sOriginalCSN == null || string.IsNullOrEmpty(sOriginalCSN.Value.ToString()))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                    }
                    originalCSN = sOriginalCSN.Value.ToString();

                    if (sReplaceCSN == null || string.IsNullOrEmpty(sReplaceCSN.Value.ToString()))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { Paras[14].SESSION_TYPE + Paras[14].SESSION_KEY }));
                    }
                    replaceCSN = sReplaceCSN.Value.ToString();
                }
                #endregion

                #region 3.如果TR_SN有值,取Allpart中TR_SN的信息
                string sql = string.Format(@"
                        SELECT A.TR_SN, A.CUST_KP_NO, A.MFR_CODE, B.MFR_NAME, A.DATE_CODE, A.LOT_CODE
                          FROM MES4.R_TR_SN A, MES1.C_MFR_CONFIG B WHERE A.MFR_CODE = B.MFR_CODE");
                string oldMfrCode = string.Empty;
                string oldMfrName = string.Empty;
                string oldDateCode = string.Empty;
                string oldLotCode = string.Empty;
                string mfrCode = string.Empty;
                string mfrName = string.Empty;
                string dateCode = string.Empty;
                string lotCode = string.Empty;
                if (sOldTRSN != null && sOldTRSN.Value != null)
                {
                    sql += string.Format(@" AND TR_SN = '{0}'", sOldTRSN.Value.ToString());
                    DataTable dt = Station.APDB.ExecuteDataTable(sql, CommandType.Text, null);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        oldMfrCode = dt.Rows[i]["MFR_CODE"].ToString();
                        oldMfrName = dt.Rows[i]["MFR_NAME"].ToString();
                        oldDateCode = dt.Rows[i]["DATE_CODE"].ToString();
                        oldLotCode = dt.Rows[i]["LOT_CODE"].ToString();
                    }
                }
                if (sTRSN != null && sTRSN.Value != null)
                {
                    sql += string.Format(@" AND TR_SN = '{0}'", sTRSN.Value.ToString());
                    DataTable dt = Station.APDB.ExecuteDataTable(sql, CommandType.Text, null);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        mfrCode = dt.Rows[i]["MFR_CODE"].ToString();
                        mfrName = dt.Rows[i]["MFR_NAME"].ToString();
                        dateCode = dt.Rows[i]["DATE_CODE"].ToString();
                        lotCode = dt.Rows[i]["LOT_CODE"].ToString();
                    }
                }
                else
                {
                    mfrCode = (sNewMfrCode == null || sNewMfrCode.Value == null) ? "" : sNewMfrCode.Value.ToString();
                    mfrName = (sNewMfrName == null || sNewMfrName.Value == null) ? "" : sNewMfrName.Value.ToString();
                    dateCode = (sNewDC == null || sNewDC.Value == null) ? "" : sNewDC.Value.ToString();
                    lotCode = (sNewLC == null || sNewLC.Value == null) ? "" : sNewLC.Value.ToString();
                }
                #endregion

                R_REPAIR_MAIN objRRM = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.SN == objSN.SerialNo && t.CLOSED_FLAG == "0").ToList().FirstOrDefault();
                C_ERROR_CODE objCEC = Station.SFCDB.ORM.Queryable<C_ERROR_CODE>().Where(t => t.ERROR_CODE == rootCause).ToList().FirstOrDefault();
                R_SN_KP objRSK = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == objSN.SerialNo && t.PARTNO == kpNo && t.EXVALUE1 == location).ToList().FirstOrDefault();
                var checkFlag = (objRSK != null && (objRSK.STATION == "PRE-ASSY" || objRSK.STATION.Contains("PRESS-FIT"))) ? "0" : "1";

                #region 3.更新Allpart替換信息[MES4.R_KP_REPLACE/MES4.R_TR_PRODUCT_DETAIL.REPLACE_FLAG]
                if (rootCause == "E206" && (sIsPCBA != null && sIsPCBA.Value.ToString() == "YES"))
                {
                    OleDbParameter[] spParam = new OleDbParameter[19];
                    spParam[0] = new OleDbParameter("MYPSN", objSN.SerialNo);
                    spParam[1] = new OleDbParameter("MYWO", objSN.WorkorderNo);
                    spParam[2] = new OleDbParameter("MYSTATION", objRRM.FAIL_STATION);
                    spParam[3] = new OleDbParameter("MYLOCATION", location);
                    spParam[4] = new OleDbParameter("MYTRSN", (sOldTRSN == null || sOldTRSN.Value == null) ? "" : sOldTRSN.Value.ToString());
                    spParam[5] = new OleDbParameter("NEWTRSN", (sTRSN == null || sTRSN.Value == null) ? "" : sTRSN.Value.ToString());
                    spParam[6] = new OleDbParameter("REPLACESN", "");
                    spParam[7] = new OleDbParameter("NEWKPSN", "");
                    spParam[8] = new OleDbParameter("OLDKPSN", "");
                    spParam[9] = new OleDbParameter("MYEMP", Station.LoginUser.EMP_NO);
                    spParam[10] = new OleDbParameter("P_NO", objSN.SkuNo);
                    spParam[11] = new OleDbParameter("FAIL_SYMPTOM", objCEC.ENGLISH_DESC);
                    spParam[12] = new OleDbParameter("ID_NO", Station.LoginUser.EMP_NO);
                    spParam[13] = new OleDbParameter("G_ERROR_CODE", objCEC.ERROR_CODE);
                    spParam[14] = new OleDbParameter("G_Failure_PN", objSN.SkuNo);
                    spParam[15] = new OleDbParameter("G_Failure_SN", objSN.SerialNo);
                    spParam[16] = new OleDbParameter("G_FAIL_ID", "");
                    spParam[17] = new OleDbParameter("CHECK_FLAG", checkFlag);
                    spParam[18] = new OleDbParameter();
                    spParam[18].Size = 1000;
                    spParam[18].ParameterName = "RES";
                    spParam[18].Direction = ParameterDirection.Output;
                    string result = Station.APDB.ExecProcedureNoReturn("MES1.Z_INSERT_KP_REPLACE_UPDATE", spParam);
                    if (!result.ToUpper().StartsWith("OK"))
                    {
                        //throw new Exception("執行Allpart SP報錯:" + result);
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151804", new string[] { result }));
                    }
                }
                #endregion

                //取得R_REPAIR_MAIN.FAIL_STATION、R_REPAIR_FAILCODE.FAIL_CODE
                var failList = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE>((rm, rf) => rm.ID == rf.MAIN_ID)
                    .Where((rm, rf) => rf.REPAIR_FLAG == "0" && rf.ID == failCodeID && rf.SN == objSN.SerialNo).Select((rm, rf) => new { rm.FAIL_STATION, rf.FAIL_CODE }).ToList();
                if (failList.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { objSN.SerialNo, failCodeID }));
                }
                #region 4.寫入R_REPAIR_ACTION表
                T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
                Row_R_REPAIR_ACTION ActionRow = (Row_R_REPAIR_ACTION)RepairAction.NewRow();
                ActionRow.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                ActionRow.REPAIR_FAILCODE_ID = failCodeID;
                ActionRow.SN = objSN.SerialNo;
                ActionRow.ACTION_CODE = actionCode;
                ActionRow.PROCESS = (sProcess == null || sProcess.Value == null) ? "" : sProcess.Value.ToString();
                ActionRow.REASON_CODE = rootCause;
                ActionRow.DESCRIPTION = objCEC.ENGLISH_DESC;
                ActionRow.FAIL_LOCATION = (sLocation == null || sLocation.Value == null) ? "" : sLocation.Value.ToString();
                ActionRow.FAIL_CODE = failList[0].FAIL_CODE;
                ActionRow.KEYPART_SN = (sPCBASN == null || sPCBASN.Value == null) ? "" : sPCBASN.Value.ToString();
                ActionRow.NEW_KEYPART_SN = "";
                ActionRow.KP_NO = (sKPNO == null || sKPNO.Value == null) ? "" : sKPNO.Value.ToString();
                ActionRow.TR_SN = (sOldTRSN == null || sOldTRSN.Value == null) ? "" : sOldTRSN.Value.ToString();
                ActionRow.MFR_CODE = oldMfrCode;
                ActionRow.MFR_NAME = oldMfrName;
                ActionRow.DATE_CODE = oldDateCode;
                ActionRow.LOT_CODE = oldLotCode;
                ActionRow.NEW_KP_NO = (sKPNO == null || sKPNO.Value == null) ? "" : sKPNO.Value.ToString();
                ActionRow.NEW_TR_SN = (sTRSN == null || sTRSN.Value == null) ? "" : sTRSN.Value.ToString();
                ActionRow.NEW_MFR_CODE = mfrCode;
                ActionRow.NEW_MFR_NAME = mfrName;
                ActionRow.NEW_DATE_CODE = dateCode;
                ActionRow.NEW_LOT_CODE = lotCode;
                ActionRow.REPAIR_EMP = Station.LoginUser.EMP_NO;
                ActionRow.REPAIR_TIME = Station.GetDBDateTime();
                ActionRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                ActionRow.EDIT_TIME = Station.GetDBDateTime();
                ActionRow.COMPOMENTID = (sCompomentID == null || sCompomentID.Value == null) ? "" : sCompomentID.Value.ToString();//Add By ZHB 20200805
                ActionRow.MPN = (sOldMPN == null || sOldMPN.Value == null) ? "" : sOldMPN.Value.ToString();//Add By ZHB 20200814
                ActionRow.NEW_MPN = (sNewMPN == null || sNewMPN.Value == null) ? "" : sNewMPN.Value.ToString();//Add By ZHB 20200814
                string res = Station.SFCDB.ExecSQL(ActionRow.GetInsertString(Station.DBType));
                if (res != "1")
                {
                    throw new Exception(res);
                }
                #endregion

                #region 5.執行完維修動作後更新R_REPAIR_FAILCODE.REPAIR_FLAG=1
                T_R_REPAIR_FAILCODE T_RFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
                Row_R_REPAIR_FAILCODE R_RFailCode = (Row_R_REPAIR_FAILCODE)T_RFailCode.GetObjByID(failCodeID, Station.SFCDB);
                R_RFailCode.REPAIR_FLAG = "1";
                R_RFailCode.EDIT_TIME = Station.GetDBDateTime();
                var r = Station.SFCDB.ExecSQL(R_RFailCode.GetUpdateString(Station.DBType));
                if (Convert.ToInt32(r) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_REPAIR_FAILCODE.REPAIR_FLAG=1" }));
                }
                #endregion

                //取得R_SN對象
                R_SN objRS = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == objSN.SerialNo && t.VALID_FLAG == "1").ToList().FirstOrDefault();
                #region 6.更新下一站:DCN只有當SN的當前站是特殊工站才會更新下一站
                if (objRS.NEXT_STATION != returnStation)
                {
                    //修復個BUG 只改下一站不改當前站 wuqing 20201118
                    string CurrentStation = "";
                    Route routeDetail = new Route(objRS.ROUTE_ID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    List<string> snStationList = new List<string>();
                    List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                    RouteDetail R = routeDetailList.Where(rr => rr.STATION_NAME == returnStation).FirstOrDefault();

                    CurrentStation = routeDetailList.Where(rr => rr.SEQ_NO == (R.SEQ_NO - 10)).FirstOrDefault().STATION_NAME;


                    var a = Station.SFCDB.ORM.Updateable<R_SN>().SetColumns(rsn => new R_SN
                    {
                        CURRENT_STATION = CurrentStation,
                        NEXT_STATION = returnStation,
                        EDIT_EMP = Station.LoginUser.EMP_NO,
                        EDIT_TIME = Station.GetDBDateTime()
                    }).Where(rsn => rsn.ID == objRS.ID).ExecuteCommand();
                    if (a <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN.NEXT_STATION" }));
                    }

                    T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                    table.RecordPassStationDetail(new List<R_SN>() { objRS }, "Repair", "RETURN", $"{objRS.NEXT_STATION}->{returnStation}",  Station.BU, Station.SFCDB);

                }
                #endregion

                #region 7.如果機種已配置REPAIR_LOCK信息且SN連續3次測試FAIL,則鎖住
                C_CONTROL objCC = Station.SFCDB.ORM.Queryable<C_CONTROL>()
                    .Where(t => t.CONTROL_NAME == "REPAIR_LOCK" && t.CONTROL_VALUE.Contains(objRS.SKUNO) && t.CONTROL_VALUE.Contains(objCEC.ENGLISH_DESC)).ToList().FirstOrDefault();
                if (objCC != null)
                {
                    bool passFlag = false;
                    List<R_TEST_RECORD> objRTR = Station.SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == objSN.SerialNo).OrderBy(t => t.STARTTIME).Take(3).ToList();
                    for (int i = 0; i < objRTR.Count; i++)
                    {
                        if (objRTR[i].STATE == "PASS")
                        {
                            passFlag = true;
                        }
                    }
                    if (!passFlag)
                    {
                        R_SN_LOCK objRSL = new R_SN_LOCK();
                        objRSL = Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN == objSN.SerialNo && t.LOCK_STATION == "" && t.LOCK_STATUS == "1").ToList().FirstOrDefault();
                        if (objRSL == null)
                        {
                            objRSL.ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_SN_LOCK");
                            objRSL.SN = objSN.SerialNo;
                            objRSL.LOCK_EMP = Station.LoginUser.EMP_NO;
                            objRSL.LOCK_REASON = $@"'{objRS.SKUNO}'/Repairaction:(NTF OR RETEST)/Test fail 3 times continuously";
                            objRSL.LOCK_STATUS = "1";
                            objRSL.LOCK_TIME = DateTime.Now;
                            objRSL.TYPE = "SN";
                            objRSL.LOCK_STATION = failList[0].FAIL_STATION;
                            Station.SFCDB.ORM.Insertable(objRSL).ExecuteCommand();
                        }
                    }
                }
                #endregion

                for (int i = 1; i < Paras.Count; i++)
                {
                    Station.StationSession.Remove(Station.StationSession.Find(s => s.MESDataType == Paras[i].SESSION_TYPE && s.SessionKey == Paras[i].SESSION_KEY));
                }
                foreach (MESStationInput input in Station.Inputs)
                {
                    input.Value = null;
                }
                Station.AddMessage("MSGCODE20200324084605", new string[] { objSN.SerialNo }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PCBA維修Save結束動作
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PCBARepairSaveFinishAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            try
            {
                //獲取SN對象
                MESStationSession sSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (sSN == null || sSN.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SN objSN = (SN)sSN.Value;

                T_R_SN T_RSN = new T_R_SN(Station.SFCDB, Station.DBType);
                T_R_REPAIR_MAIN T_RMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
                List<R_REPAIR_MAIN> objRRMList = new List<R_REPAIR_MAIN>();
                List<R_REPAIR_FAILCODE> objRRFList = new List<R_REPAIR_FAILCODE>();
                T_C_ROUTE_DETAIL T_CRDetail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);

                objRRMList = T_RMain.GetRepairMainBySN(Station.SFCDB, objSN.SerialNo).FindAll(r => r.CLOSED_FLAG == "0");
                if (objRRMList == null || objRRMList.Count == 0)
                {
                    //維修主表無信息
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { objSN.SerialNo, "CLOSED_FLAG=0" }));
                }
                else if (objRRMList.Count > 1)
                {
                    //維修主表有多條為維修完成的記錄
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180621185023", new string[] { objSN.SerialNo }));
                }

                objRRFList = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.MAIN_ID == objRRMList[0].ID && t.SN == objSN.SerialNo && t.REPAIR_FLAG == "0").ToList();
                if (objRRFList.Count != 0)
                {
                    //未維修完成的無法更新R_REPAIR_MAIN表信息
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000106", new string[] { objSN.SerialNo, objRRFList[0].ID }));
                }

                //執行完所有的維修動作後才能更新R_REPAIR_MAIN  FLAG=1 
                Row_R_REPAIR_MAIN R_RMain = (Row_R_REPAIR_MAIN)T_RMain.GetObjByID(objRRMList[0].ID, Station.SFCDB);
                R_RMain.CLOSED_FLAG = "1";
                R_RMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                R_RMain.EDIT_TIME = Station.GetDBDateTime();
                var result = Station.SFCDB.ExecSQL(R_RMain.GetUpdateString(Station.DBType));
                if (Convert.ToInt32(result) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_REPAIR_MAIN.CLOSED_FLAG=1" }));
                }
                else
                {
                    //清空Input Output Session
                    foreach (MESStationInput input in Station.Inputs)
                    {
                        if (Station.StationSession.Find(s => s.MESDataType == input.DisplayName) != null)
                        {
                            Station.StationSession.Find(s => s.MESDataType == input.DisplayName).Value = "";
                        }
                        input.Value = "";
                    }
                    foreach (R_Station_Output output in Station.StationOutputs)
                    {
                        if (Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY) != null)
                        {
                            Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                        }
                    }
                }

                //添加過站記錄
                T_RSN.RecordPassStationDetail(objSN.SerialNo, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Add Panel Fail Data
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddPanelFailDataAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionSNNumber = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSNNumber == null || sessionSNNumber.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession sessionLocation = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionLocation == null || sessionLocation.Value == null)
            {
                //是否必填
                if (!string.IsNullOrEmpty(Paras[3].VALUE) && Paras[3].VALUE.ToUpper().Trim() == "TRUE")
                {
                    throw new MESReturnMessage($@"Please input {Paras[3].SESSION_TYPE},{ Paras[3].SESSION_KEY}");
                }
            }

            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionFailCode == null || sessionFailCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }
            int result = 0;
            string failStation = sessionStation.Value.ToString();
            string failSNNumber = sessionSNNumber.Value.ToString();
            string failLocation = (sessionLocation == null || sessionLocation.Value == null) ? "" : sessionLocation.Value.ToString();
            Panel failPanel = (Panel)sessionPanel.Value;
            C_ERROR_CODE failCode = (C_ERROR_CODE)sessionFailCode.Value;
            T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            T_R_REPAIR_FAILCODE t_r_repair_failcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            R_WO_BASE woObject = t_r_wo_base.GetWoByWoNo(failPanel.Workorderno, Station.SFCDB);
            R_REPAIR_MAIN repairMain = t_r_repair_main.GetRepairListSNAndStation(Station.SFCDB, failPanel.PanelNo, failStation, "0").FirstOrDefault();
            if (repairMain == null)
            {
                List<R_SN> listSNObject = failPanel.GetSnDetail(failPanel.PanelNo, Station.SFCDB, Station.DBType);
                string line_name = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(r => r.R_SN_ID == listSNObject.FirstOrDefault().ID)
                    .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault().LINE;
                foreach (R_SN r_sn in listSNObject)
                {
                    //更新R_SN REPAIR_FAILED_FLAG=’1’                    
                    r_sn.REPAIR_FAILED_FLAG = "1";
                    r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                    r_sn.EDIT_TIME = Station.GetDBDateTime();
                    result = t_r_sn.Update(r_sn, Station.SFCDB);
                    if (result == 0)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                    }

                    //新增一筆FAIL記錄到R_SN_STATION_DETAIL
                    string re = t_r_sn_station_detail.AddDetailToRSnStationDetail(t_r_sn_station_detail.GetNewID(Station.BU, Station.SFCDB),
                        r_sn, line_name, failStation, failStation, Station.SFCDB);
                    if (!(Convert.ToInt32(re) > 0))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "STATION_DETAIL" }));
                    }
                }

                repairMain = new R_REPAIR_MAIN();
                repairMain.ID = t_r_repair_main.GetNewID(Station.BU, Station.SFCDB);
                repairMain.SN = failPanel.PanelNo;
                repairMain.WORKORDERNO = woObject.WORKORDERNO;
                repairMain.SKUNO = woObject.SKUNO;
                repairMain.FAIL_LINE = line_name;
                repairMain.FAIL_STATION = failStation;
                repairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                repairMain.FAIL_TIME = Station.GetDBDateTime();
                repairMain.CREATE_TIME = repairMain.FAIL_TIME;
                repairMain.CLOSED_FLAG = "0";
                repairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                repairMain.EDIT_TIME = repairMain.FAIL_TIME;
                result = t_r_repair_main.Insert(repairMain, Station.SFCDB);
                if (result == 0)
                {
                    throw new MESReturnMessage("Save Repair Main Fail!");
                }
            }

            bool bExist = false;
            if (string.IsNullOrEmpty(failLocation))
            {
                bExist = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>()
                .Where(r => r.MAIN_ID == repairMain.ID && r.REPAIR_FLAG == "0" && r.SN == repairMain.SN
                    && r.FAIL_CODE == failCode.ERROR_CODE && r.DESCRIPTION == failSNNumber && SqlSugar.SqlFunc.IsNullOrEmpty(r.F_LOCATION))
                 .Any();
            }
            else
            {
                bExist = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>()
                    .Where(r => r.MAIN_ID == repairMain.ID && r.REPAIR_FLAG == "0" && r.SN == repairMain.SN
                        && r.FAIL_CODE == failCode.ERROR_CODE && r.DESCRIPTION == failSNNumber && r.F_LOCATION == failLocation)
                     .Any();
            }
            if (bExist)
            {
                throw new MESReturnMessage($@"Fail Code:{failCode.ERROR_CODE},Fail Location:{failLocation},SN Number:{failSNNumber},Already Exist!");
            }
            R_REPAIR_FAILCODE repairFailCode = new R_REPAIR_FAILCODE();
            repairFailCode.ID = t_r_repair_failcode.GetNewID(Station.BU, Station.SFCDB);
            repairFailCode.MAIN_ID = repairMain.ID;
            repairFailCode.SN = repairMain.SN;
            repairFailCode.REPAIR_FLAG = "0";
            repairFailCode.FAIL_CODE = failCode.ERROR_CODE;
            repairFailCode.F_LOCATION = failLocation;
            repairFailCode.DESCRIPTION = failSNNumber;
            repairFailCode.FAIL_TIME = Station.GetDBDateTime();
            repairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
            repairFailCode.CREATE_TIME = repairFailCode.FAIL_TIME;
            repairFailCode.EDIT_TIME = repairFailCode.FAIL_TIME;
            repairFailCode.EDIT_EMP = repairFailCode.FAIL_EMP;
            repairFailCode.F_CATEGORY = failCode.ERROR_CODE.StartsWith("S")
                ? R_REPAIR_FAILCODE_CATEGORY.SYMPTOM.Ext<EnumValueAttribute>().Description
                : R_REPAIR_FAILCODE_CATEGORY.DEFECT.Ext<EnumValueAttribute>().Description;
            result = t_r_repair_failcode.Save(Station.SFCDB, repairFailCode);
            if (result == 0)
            {
                throw new MESReturnMessage("Save Repair Code Fail!");
            }
            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
        }

        /// <summary>
        /// Repair Save Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairSaveAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionRepairFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionRepairFailCode == null || sessionRepairFailCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionActionCode == null || sessionActionCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionRootCause = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            MESStationSession sessionLocation = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            MESStationSession sessionProcess = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            MESStationSession sessionDescription = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            MESStationSession sessionRepairFinishFlag = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (sessionRepairFinishFlag == null)
            {
                sessionRepairFinishFlag = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, SessionKey = Paras[6].SESSION_KEY };
                Station.StationSession.Add(sessionRepairFinishFlag);
            }
            T_R_REPAIR_ACTION t_r_repair_action = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            string repair_fail_code_id = sessionRepairFailCode.Value.ToString();
            C_ACTION_CODE actionObj = (C_ACTION_CODE)sessionActionCode.Value;
            R_REPAIR_ACTION repairAction = null;

            string root_cause = "";
            R_REPAIR_FAILCODE failObj = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(r => r.ID == repair_fail_code_id).ToList().FirstOrDefault();
            if (failObj == null)
            {
                throw new MESReturnMessage($@"[{repair_fail_code_id}]Repair Fial Code Not Exists!");
            }
            if (sessionRootCause != null && sessionRootCause.Value != null)
            {
                if (sessionRootCause.Value is C_ERROR_CODE)
                {
                    //root_cause = ((C_ERROR_CODE)sessionRootCause.Value).CHINESE_DESC;
                    root_cause = ((C_ERROR_CODE)sessionRootCause.Value).ERROR_CODE;
                }
                else if (sessionRootCause.Value is C_REASON_CODE)
                {
                    //root_cause = ((C_REASON_CODE)sessionRootCause.Value).CHINESE_DESCRIPTION;
                    root_cause = ((C_REASON_CODE)sessionRootCause.Value).REASON_CODE;
                }
                else
                {
                    root_cause = sessionRootCause.Value.ToString();
                }
            }
            root_cause = string.IsNullOrEmpty(root_cause) ? failObj.FAIL_CODE : root_cause;
            string fail_location = (sessionLocation == null || sessionLocation.Value == null) ? failObj.F_LOCATION : sessionLocation.Value.ToString();
            string process = (sessionProcess == null || sessionProcess.Value == null) ? "" : sessionProcess.Value.ToString();
            string description = (sessionDescription == null || sessionDescription.Value == null) ? "" : sessionDescription.Value.ToString();
            int result = 0;
            DateTime sysdate = Station.GetDBDateTime();
            sessionRepairFinishFlag.Value = "0";
            failObj.REPAIR_FLAG = "1";
            failObj.EDIT_EMP = Station.LoginUser.EMP_NO;
            failObj.EDIT_TIME = sysdate;
            result = Station.SFCDB.ORM.Updateable<R_REPAIR_FAILCODE>(failObj).Where(r => r.ID == failObj.ID).ExecuteCommand();
            if (result == 0)
            {
                throw new MESReturnMessage("Update Repari Fial Code Error!");
            }
            repairAction = new R_REPAIR_ACTION();
            repairAction.ID = t_r_repair_action.GetNewID(Station.BU, Station.SFCDB);
            repairAction.R_FAILCODE_ID = failObj.ID;
            repairAction.ACTION_CODE = actionObj.ACTION_CODE;
            repairAction.SN = failObj.SN;
            repairAction.PROCESS = process;
            repairAction.DESCRIPTION = description;
            repairAction.REASON_CODE = root_cause;
            repairAction.FAIL_CODE = failObj.FAIL_CODE;
            repairAction.FAIL_LOCATION = fail_location;
            repairAction.REPAIR_EMP = Station.LoginUser.EMP_NO;
            repairAction.REPAIR_TIME = sysdate;
            repairAction.EDIT_EMP = repairAction.REPAIR_EMP;
            repairAction.EDIT_TIME = sysdate;
            repairAction.FAIL_TIME = failObj.FAIL_TIME;
            result = Station.SFCDB.ORM.Insertable<R_REPAIR_ACTION>(repairAction).ExecuteCommand();
            if (result == 0)
            {
                throw new MESReturnMessage("Save Repari Action Error!");
            }
            List<R_REPAIR_FAILCODE> list = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(r => r.MAIN_ID == failObj.MAIN_ID && r.REPAIR_FLAG == "0").ToList();
            if (list.Count == 0)
            {
                R_REPAIR_MAIN repairMain = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN>().Where(r => r.ID == failObj.MAIN_ID).ToList().FirstOrDefault();
                if (repairMain == null)
                {
                    throw new MESReturnMessage("Repair Main Not Exist!");
                }
                repairMain.CLOSED_FLAG = "1";
                repairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                repairMain.EDIT_TIME = sysdate;
                result = Station.SFCDB.ORM.Updateable<R_REPAIR_MAIN>(repairMain).Where(r => r.ID == repairMain.ID).ExecuteCommand();
                if (result == 0)
                {
                    throw new MESReturnMessage("Update Repari Main Error!");
                }
                sessionRepairFinishFlag.Value = "1";
                Station.StationMessages.Add(
                    new StationMessage { State = StationMessageState.Pass, Message = "OK" }
                    );
            }
        }
        /// <summary>
        /// Repair Finish Update R_SN REPAIR_FAILED_FLAG Action 
        /// Used in conjunction with RepairSaveAction
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairFinishUpdateSNFlagAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //獲取到 SN 對象
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionRepairFinishFlag = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionRepairFinishFlag == null || sessionRepairFinishFlag.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            int result = 0;
            DateTime sysdate = Station.GetDBDateTime();
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            List<R_SN> listSN = new List<R_SN>();
            bool bRepairFinish = sessionRepairFinishFlag.Value.ToString() == "1" ? true : false;
            if (!bRepairFinish)
            {
                return;
            }
            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                listSN.Add(t_r_sn.LoadData(snObject.SerialNo, Station.SFCDB));
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                listSN = panelObject.GetSnDetail(panelObject.PanelNo, Station.SFCDB, Station.DBType);
            }
            if (listSN.Count == 0)
            {
                throw new MESReturnMessage("SN OR Panel Not Exists!");
            }

            foreach (R_SN r_sn in listSN)
            {
                r_sn.REPAIR_FAILED_FLAG = "0";
                r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                r_sn.EDIT_TIME = sysdate;
                result = t_r_sn.Update(r_sn, Station.SFCDB);
                if (result == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                }

                //新增一筆FAIL記錄到R_SN_STATION_DETAIL
                string re = t_r_sn_station_detail.AddDetailToRSnStationDetail(t_r_sn_station_detail.GetNewID(Station.BU, Station.SFCDB),
                    r_sn, Station.Line, "Repair", "Repair", Station.SFCDB);
                if (!(Convert.ToInt32(re) > 0))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "STATION_DETAIL" }));
                }
                Station.StationMessages.Add(
                   new StationMessage { State = StationMessageState.Pass, Message = "OK" }
                   );
            }
        }

        public static void FailByCodeProcessLocationAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionFailCode == null || sessionFailCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionProcess = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionProcess == null || sessionProcess.Value == null)
            {
                //是否必填
                if (!string.IsNullOrEmpty(Paras[2].VALUE) && Paras[2].VALUE.ToUpper().Trim() == "TRUE")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }
            MESStationSession sessionLocation = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionLocation == null || sessionLocation.Value == null)
            {
                //是否必填
                if (!string.IsNullOrEmpty(Paras[3].VALUE) && Paras[3].VALUE.ToUpper().Trim() == "TRUE")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
            }

            SN snObj = (SN)sessionSN.Value;
            C_ERROR_CODE failCodeObj = (C_ERROR_CODE)sessionFailCode.Value;
            string failProcess = (sessionProcess == null || sessionProcess.Value == null) ? "" : sessionProcess.Value.ToString();
            string failLocation = (sessionLocation == null || sessionLocation.Value == null) ? "" : sessionLocation.Value.ToString();
            int result = 0;
            DateTime sysdate = Station.GetDBDateTime();

            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            T_R_REPAIR_FAILCODE t_r_repair_failcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);

            //更新R_SN REPAIR_FAILED_FLAG=’1’
            result = Station.SFCDB.ORM.Updateable<R_SN>()
                .UpdateColumns(r => new R_SN { REPAIR_FAILED_FLAG = "1", EDIT_EMP = Station.LoginUser.EMP_NO, EDIT_TIME = sysdate })
                .Where(r => r.ID == snObj.ID).ExecuteCommand();
            if (result == 0)
            {
                throw new MESReturnMessage("Update Repair Failed Flag Error!");
            }

            R_SN r_sn = t_r_sn.GetRowById(snObj.ID, Station.SFCDB);
            //新增一筆FAIL記錄到R_SN_STATION_DETAIL
            string re = t_r_sn_station_detail.AddDetailToRSnStationDetail(t_r_sn_station_detail.GetNewID(Station.BU, Station.SFCDB),
                r_sn, Station.Line, Station.StationName, Station.StationName, Station.SFCDB);
            if (!(Convert.ToInt32(re) > 0))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "STATION_DETAIL" }));
            }
            //新增一筆到R_REPAIR_MAIN
            R_REPAIR_MAIN repairMain = new R_REPAIR_MAIN();
            repairMain.ID = t_r_repair_main.GetNewID(Station.BU, Station.SFCDB);
            repairMain.SN = snObj.SerialNo;
            repairMain.WORKORDERNO = snObj.WorkorderNo;
            repairMain.SKUNO = snObj.SkuNo;
            repairMain.FAIL_LINE = Station.Line;
            repairMain.FAIL_STATION = Station.StationName;
            repairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
            repairMain.FAIL_TIME = sysdate;
            repairMain.CREATE_TIME = sysdate;
            repairMain.CLOSED_FLAG = "0";
            repairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
            repairMain.EDIT_TIME = sysdate;
            result = t_r_repair_main.Insert(repairMain, Station.SFCDB);
            if (result == 0)
            {
                throw new MESReturnMessage("Save Repair Main Fail!");
            }
            //新增一筆到R_REPAIR_FAILCODE
            R_REPAIR_FAILCODE repairFailCode = new R_REPAIR_FAILCODE();
            repairFailCode.ID = t_r_repair_failcode.GetNewID(Station.BU, Station.SFCDB);
            repairFailCode.MAIN_ID = repairMain.ID;
            repairFailCode.SN = repairMain.SN;
            repairFailCode.REPAIR_FLAG = "0";
            repairFailCode.FAIL_CODE = failCodeObj.ERROR_CODE;
            repairFailCode.F_LOCATION = failLocation;
            repairFailCode.F_PROCESS = failProcess;
            repairFailCode.DESCRIPTION = "";
            repairFailCode.FAIL_TIME = sysdate;
            repairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
            repairFailCode.CREATE_TIME = sysdate;
            repairFailCode.EDIT_TIME = sysdate;
            repairFailCode.EDIT_EMP = repairFailCode.FAIL_EMP;
            result = t_r_repair_failcode.Save(Station.SFCDB, repairFailCode);
            if (result == 0)
            {
                throw new MESReturnMessage("Save Repair Code Fail!");
            }
            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Pass);
        }
    
        public static void RepairCheckout2Action(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {            

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }           

            //SN snObject = (SN)sessionSN.Value;
            string repairNo = "";
            List<R_SN> listSNObject = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                repairNo = snObject.SerialNo;
                listSNObject.Add(t_r_sn.LoadData(snObject.SerialNo, Station.SFCDB));
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                repairNo = panelObject.PanelNo;
                panelObject.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                listSNObject = panelObject.PanelSnList;
            }
            else if (sessionSN.Value.GetType() == typeof(string))
            {
                R_SN objSN = TRS.LoadData(sessionSN.Value.ToString(), Station.SFCDB);
                if (objSN != null)
                {
                    repairNo = objSN.SN;
                    listSNObject.Add(t_r_sn.LoadData(repairNo, Station.SFCDB));
                }
                else if (TRPS.CheckPanelExist(sessionSN.Value.ToString(), Station.SFCDB))//輸入的是一個Panel
                {
                    Panel panelO = new Panel(sessionSN.Value.ToString(), Station.SFCDB, Station.DBType);
                    repairNo = panelO.PanelNo;

                    panelO.GetSnDetail(repairNo, Station.SFCDB, Station.DBType);
                    listSNObject = panelO.PanelSnList;
                }
                else
                {
                    throw new Exception("Input Type Error");
                }
            }
            else
            {
                throw new Exception("Input Type Error");
            }


            T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            DateTime sysdate = Station.SFCDB.ORM.GetDate();         

            List<R_REPAIR_TRANSFER> transferList = rTransfer.GetLastRepairedBySN(repairNo, Station.SFCDB);
            R_REPAIR_TRANSFER rRepairTransfer = transferList.Where(r => r.CLOSED_FLAG == "0" 
            && SqlSugar.SqlFunc.ToUpper(r.OUT_RECEIVE_EMP)== "WAIT_CHECKOUT2"
            && !SqlSugar.SqlFunc.IsNullOrEmpty(r.OUT_TIME) && !SqlSugar.SqlFunc.IsNullOrEmpty(r.OUT_SEND_EMP)).FirstOrDefault();//TRANSFER表 1 表示不良

            if (rRepairTransfer != null)
            {               
                rRepairTransfer.CLOSED_FLAG = "1";
                rRepairTransfer.OUT_RECEIVE_EMP = Station.LoginUser.EMP_NO;
                rRepairTransfer.EDIT_TIME = sysdate;
                rRepairTransfer.EDIT_EMP = Station.LoginUser.EMP_NO;

                int res = Station.SFCDB.ORM.Updateable<R_REPAIR_TRANSFER>(rRepairTransfer).Where(r => r.ID == rRepairTransfer.ID).ExecuteCommand();
                if (res > 0)
                {
                    Station.AddMessage("MES00000035", new string[] { res.ToString() }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000025", new string[] { "REPAIR TRANSFER" }, StationMessageState.Pass);
                }
                foreach (R_SN snObj in listSNObject)
                {                    
                    snObj.REPAIR_FAILED_FLAG = "0";
                    snObj.EDIT_TIME = sysdate;
                    snObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                    res = Station.SFCDB.ORM.Updateable<R_SN>(snObj).Where(r => r.ID == snObj.ID).ExecuteCommand();
                    if (res > 0)
                    {
                        Station.AddMessage("MES00000035", new string[] { res.ToString() }, StationMessageState.Pass);
                    }
                    else
                    {
                        Station.AddMessage("MES00000025", new string[] { "R_SN" }, StationMessageState.Pass);
                    }
                }
            }
            else
            {
                throw new MESReturnMessage($@"{repairNo} not in [CHECKOUT2]");
            }
        }

        public static void RePairOffLine(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessioncode = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessioncode == null || sessioncode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            string sn = sessionSN.InputValue.ToString();
            string errorcode = sessioncode.InputValue.ToString();
            T_C_ERROR_CODE c_errorcode = new T_C_ERROR_CODE(Station.SFCDB, Station.DBType);
            if (errorcode != "S315" && errorcode != "S316")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211104021359", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (!c_errorcode.CheckErrorCodeByErrorCode(Station.SFCDB, errorcode))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211104021448", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            try
            {
                T_C_SKU c_SKU = new T_C_SKU(Station.SFCDB, Station.DBType);
                T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
                R_SN_LOG check_log;
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                R_SN r_sn;
                r_sn = new R_SN();
                r_sn = t_r_sn.GetDetailBySN(sn, Station.SFCDB);
                if (!c_SKU.CheckSkuNETGEAR(sn, Station.SFCDB))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211105072040", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                check_log = new R_SN_LOG();
                check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                check_log.SNID = r_sn.ID;
                check_log.SN = sn;
                check_log.LOGTYPE = "Repair_offline";
                check_log.DATA1 = "MESStation.Stations";
                check_log.DATA2 = "StationActions.ActionRunners";
                check_log.DATA3 = "RepairActions";
                check_log.DATA4 = "ManegeErrorCodeOffForSkuSMT";
                check_log.DATA5 = errorcode;
                check_log.DATA6 = errorcode;
                check_log.DATA7 = "0";
                check_log.FLAG = "0";
                check_log.CREATETIME = Station.GetDBDateTime();
                check_log.CREATEBY = Station.LoginUser.EMP_NO;
                int rs = Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                if (rs == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_SN_LOG" }));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
