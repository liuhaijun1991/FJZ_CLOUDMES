using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.Vertiv;
using System.Data;
using MESDataObject.Common;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckRepairFail
    {
        /// <summary>
        /// 維修輸入SN Fail狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFailChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            //input test
            /*string inputValue = Input.Value.ToString();
            if (string.IsNullOrEmpty(inputValue))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN輸入值" }));
            }
            SN sn = new SN(inputValue, sfcdb, DB_TYPE_ENUM.Oracle);*/

            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE
                           && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                }
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            SN sn = (SN) SN_Session.Value;
            
            if (sn.RepairFailedFlag == "0")
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    try
                    {
                        Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                    }
                    catch
                    {
                    }
                }
                //正常品
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000078", new string[] { sn.SerialNo }));
            }
            List<R_REPAIR_MAIN> repairMains = new T_R_REPAIR_MAIN(sfcdb, DB_TYPE_ENUM.Oracle).GetRepairMainBySN(sfcdb, sn.SerialNo);
            if (repairMains == null || repairMains.Count == 0)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                }
                //無維修主檔信息
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", sn.SerialNo }));
            }
            R_REPAIR_MAIN rm = repairMains.Find(r => r.CLOSED_FLAG == "0");
            List<R_REPAIR_FAILCODE> rf = sfcdb.ORM.Queryable<R_REPAIR_FAILCODE>().Where(f => f.SN == sn.SerialNo && f.REPAIR_FLAG == "0").ToList();
            if (rm == null && rf.Count > 0)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    try
                    {
                        Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                    }
                    catch
                    {
                    }
                }
                //無維修主檔信息
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", sn.SerialNo }));
            }
            else if(rm == null)
            {
                //產品已維修，R_SN.REPAIR_FAILED_FLAG為1時，需要check out ---ADD BY LJD 2019-3-27 10:25:56
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190327103914", new string[] { "Repiar CHECK_OUT"}));
            }
            //foreach (R_REPAIR_MAIN rm in repairMains)
            //{
            //    //存在closed_flag=0
            //    if (rm.CLOSED_FLAG != "0")
            //    {
            //        foreach (R_Station_Output output in Station.StationOutputs)
            //        {
            //            Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
            //        }
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000097", new string[] {"SN", rm.SN }));
            //    }
            //}
            Station.AddMessage("MES00000046", new string[] { "OK" }, StationMessageState.Pass);
        }
        /// <summary>
        /// SN 或 Panel Fail狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNOrPanelRepairFailChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionInput == null || sessionInput.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            string input_value = sessionInput.Value.ToString();
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN TRPS=new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            R_SN objSN = null;
            
            List<R_SN> listSN = new List<R_SN>();

            //輸入的是一個SN
            objSN = TRS.LoadData(input_value, Station.SFCDB);
            if (objSN != null)
            {
                listSN.Add(objSN);
                SN objSNNew = new SN(objSN.SN, Station.SFCDB, Station.DBType);
                sessionInput.Value = objSNNew;
            }
            else if (TRPS.CheckPanelExist(input_value, Station.SFCDB))//輸入的是一個Panel
            {
                listSN = TRPS.GetSn(input_value, Station.SFCDB);
                Panel panelObject = new Panel(input_value, Station.SFCDB, Station.DBType);               
                sessionInput.Value = panelObject;
            }
            if (listSN.Count == 0)
            {
                sessionInput.Value = "";
                throw new Exception("Please Input A SN Or Panel");
            }
            foreach (R_SN sn in listSN)
            {
                if (sn.REPAIR_FAILED_FLAG.Equals("0"))
                {
                    sessionInput.Value = "";
                    //正常品
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000078", new string[] { input_value }));
                }
            }

            List<R_REPAIR_MAIN> repairMains = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle).GetRepairMainBySN(Station.SFCDB, input_value);
            if (repairMains == null || repairMains.Count == 0)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                }
                //無維修主檔信息
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", input_value }));
            }
            R_REPAIR_MAIN rm = repairMains.Find(r => r.CLOSED_FLAG == "0");

            List<R_REPAIR_FAILCODE> rf = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(f => f.SN == input_value && f.REPAIR_FLAG == "0").ToList();
            if (rm == null && rf.Count > 0)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    try
                    {
                        Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                    }
                    catch
                    {
                    }
                }
                //無維修主檔信息
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", input_value }));
            }
            else if (rm == null)
            {
                //產品已維修，R_SN.REPAIR_FAILED_FLAG為1時，需要check out ---ADD BY LJD 2019-3-27 10:25:56
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190327103914", new string[] { "Repiar CHECK_OUT" }));
            }            
            Station.AddMessage("MES00000046", new string[] { "OK" }, StationMessageState.Pass);

        }

        /// <summary>
        /// 維修輸入SN Fail次數管控
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairCountChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //string inputValue = Input.Value.ToString();

            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            SN sn = (SN) SN_Session.Value;
            string skuno = null;
            //Paras: SESSION_TYPE='SKU'  SESSION_KEY='1'  VALUE='0,1'
            switch (Paras[1].VALUE)
            {
                case "0":
                    skuno = sn.SkuNo;
                    break;
                default:
                    skuno = "ALL";
                    break;
            }

            OleExec sfcdb = Station.SFCDB;
            C_REPAIR_DAY repairDay = new T_C_REPAIR_DAY(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySkuno(sfcdb, skuno);
            if (repairDay != null)
            {
                //repair_count
                if (repairDay.REPAIR_COUNT == 3)
                {
                    Station.AddMessage("MES00000087", new string[] { repairDay.REPAIR_COUNT.ToString(), "請注意" }, StationMessageState.Message);
                }
                if (repairDay.REPAIR_COUNT > 3)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000087", new string[] { repairDay.REPAIR_COUNT.ToString(), "已鎖定" }));
                }
            }
        }

        public static void RepairPCBASNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "PCBASNSession" }));
            }
            MESStationSession PCBASNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PCBASNSession == null || PCBASNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "PCBASNSession" }));
            }
            try
            {
                if (SNSession.Value.GetType() == typeof(SN))
                {
                    LogicObject.SN snObject = (LogicObject.SN)SNSession.Value;
                    if (snObject.SerialNo != PCBASNSession.Value.ToString() && snObject.BoxSN != PCBASNSession.Value.ToString())
                    {
                        T_R_SN_KP t_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                        if (!t_sn_kp.KpIsLinkBySN(snObject.ID, PCBASNSession.Value.ToString(), Station.SFCDB))
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616081316", new string[] { PCBASNSession.Value.ToString(), snObject.SerialNo }));
                        }
                    }
                }
                else if (SNSession.Value.GetType() == typeof(Panel))
                {
                    Panel panelObject = (Panel)SNSession.Value;
                    T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
                    R_SN sn = TRS.LoadData(PCBASNSession.Value.ToString(), Station.SFCDB);
                    List<R_SN> listSN = panelObject.GetSnDetail(panelObject.PanelNo, Station.SFCDB, Station.DBType);
                    if (panelObject.PanelNo != PCBASNSession.Value.ToString() && !listSN.Contains(sn))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616081316", new string[] { PCBASNSession.Value.ToString(), panelObject.PanelNo }));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// REPAIR_CHECK_IN狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairInStatusChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }            
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string input_value = "";
            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                input_value = snObject.SerialNo;
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                input_value = panelObject.PanelNo;
            }
            else
            {
                input_value = sessionSN.Value.ToString();
            }
            T_R_REPAIR_TRANSFER t_r_repair = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            if (t_r_repair.SNIsRepairIn(input_value, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154230", new string[] { input_value })); 
            }
        }
        
        /// <summary>
        /// REPAIR_CHECK_IN權限檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairInEmpChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string type = Paras[0].VALUE.ToString().ToUpper();
            if (type == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionEmp = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionEmp == null || sessionEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            //Vertiv SE&RE黃克喜要求只有指定人員才能掃入和接收REPAIR_CHECK_IN 
            T_c_user t_c_uer = new T_c_user(Station.SFCDB, Station.DBType);
            Row_c_user rowUser = t_c_uer.getC_Userbyempno(sessionEmp.Value.ToString(), Station.SFCDB, Station.DBType);           
            T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, Station.DBType);
            string[] inEmp = t_c_control.GetControlByName("REPAIR_CHECK_IN_SEND", Station.SFCDB).CONTROL_VALUE.Split(',');
            string[] receiveEmp = t_c_control.GetControlByName("REPAIR_CHECK_IN_RECEIVE", Station.SFCDB).CONTROL_VALUE.Split(',');
            List<string> inEmpList = new List<string>(inEmp);
            List<string> receiveList = new List<string>(receiveEmp);
            if (rowUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionEmp.Value.ToString() }));
            }
            if (type == "SEND")
            {
                if (inEmpList.Find(s => s == rowUser.EMP_NO) == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154648", new string[] { rowUser.EMP_NO }));
                }
            }
            else if(type == "RECEIVE")
            {
                if (receiveList.Find(s => s == rowUser.EMP_NO) == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154947", new string[] { rowUser.EMP_NO }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { "Input" })); 
            }
        }

        /// <summary>
        /// REPAIR_CHECK_OUT狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairOutStatusChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }           
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            //SN snObject = (SN)sessionSN.Value;
            string input_value = "";
            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                input_value = snObject.SerialNo;
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                input_value = panelObject.PanelNo;
            }
            else
            {
                input_value = sessionSN.Value.ToString();
            }
            T_R_REPAIR_TRANSFER t_r_repair = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            if (!t_r_repair.SNIsRepairIn(input_value, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154342", new string[] { input_value })); 
            }

            T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            if(!t_r_repair_main.SNIsRepaired(input_value, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { input_value })); 
            }
        }

        /// <summary>
        /// Panel REPAIR_CHECK_OUT狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelRepairOutStatusChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            Panel panelObject = (Panel)sessionPanel.Value;
            T_R_REPAIR_TRANSFER t_r_repair = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            if (!t_r_repair.SNIsRepairIn(panelObject.PanelNo, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154342", new string[] { panelObject.PanelNo }));
            }

            T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            if (!t_r_repair_main.SNIsRepaired(panelObject.PanelNo, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { panelObject.PanelNo }));
            }
        }

        /// <summary>
        /// REPAIR_CHECK_OUT權限檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairOutEmpChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string type = Paras[0].VALUE.ToString().ToUpper();
            if (type == "")
            { 
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionEmp = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionEmp == null || sessionEmp.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            //Vertiv SE&RE黃克喜要求只有指定人員才能掃入和接收REPAIR_CHECK_IN 
            T_c_user t_c_uer = new T_c_user(Station.SFCDB, Station.DBType);
            Row_c_user rowUser = t_c_uer.getC_Userbyempno(sessionEmp.Value.ToString(), Station.SFCDB, Station.DBType);
            T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, Station.DBType);
            string[] inEmp = t_c_control.GetControlByName("REPAIR_CHECK_OUT_SEND", Station.SFCDB).CONTROL_VALUE.Split(',');
            string[] receiveEmp = t_c_control.GetControlByName("REPAIR_CHECK_OUT_RECEIVE", Station.SFCDB).CONTROL_VALUE.Split(',');
            List<string> inEmpList = new List<string>(inEmp);
            List<string> receiveList = new List<string>(receiveEmp);
            if (rowUser == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { sessionEmp.Value.ToString() }));
            }
            if (type == "SEND")
            {
                if (inEmpList.Find(s => s == rowUser.EMP_NO) == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619155133", new string[] { rowUser.EMP_NO }));
                }
            }
            else if (type == "RECEIVE")
            {
                if (receiveList.Find(s => s == rowUser.EMP_NO) == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154947", new string[] { rowUser.EMP_NO }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { "Input" }));
            }
        }

        /// <summary>
        /// 檢查SN是否有掃REPAIR_CHECK_IN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairInChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            //SN snObject = (SN)sessionSN.Value;
            string input_value = "";
            if (sessionSN.Value.GetType() == typeof(SN))
            {
                SN snObject = (SN)sessionSN.Value;
                input_value = snObject.SerialNo;
            }
            else if (sessionSN.Value.GetType() == typeof(Panel))
            {
                Panel panelObject = (Panel)sessionSN.Value;
                input_value = panelObject.PanelNo;
            }
            else
            {
                input_value = sessionSN.Value.ToString();
            }

            T_R_REPAIR_TRANSFER t_r_repair = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            if (!t_r_repair.SNIsRepairIn(input_value, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154342", new string[] { input_value }));
            }
        }

        /// <summary>
        /// 檢查Panel是否有掃REPAIR_CHECK_IN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelRepairInChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel panelObject = (Panel)sessionPanel.Value;
            T_R_REPAIR_TRANSFER t_r_repair = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);
            if (!t_r_repair.SNIsRepairIn(panelObject.PanelNo, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154342", new string[] { panelObject.PanelNo }));
            }
        }

        /// <summary>
        /// SN掃入維修檢查Completed,JOBFINISH,REWORK,RepairFailedFlag,ShippedFlag
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNInRepariChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObject = (SN)sessionSN.Value;
            if (snObject.CompletedFlag.Equals("1"))
            {  
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000069", new string[] { snObject.SerialNo }));
            }
            if (snObject.NextStation.Equals("JOBFINISH"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000069", new string[] { snObject.SerialNo }));
            }
            if (snObject.NextStation.Equals("REWORK"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180927150610", new string[] { snObject.SerialNo }));
            }
            if (snObject.RepairFailedFlag.Equals("1"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000068", new string[] { snObject.SerialNo }));
            }
            if (snObject.ShippedFlag.Equals("1"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000070", new string[] { snObject.SerialNo }));
            }
            //check r_repair_main
            T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            if (!t_r_repair_main.SNIsRepaired(snObject.SerialNo, Station.SFCDB))
            {
                throw new Exception($@"{snObject.SerialNo},The Last Repair Not Close(closed=0)");
            }
            //check r_repair_transfer 
            T_R_REPAIR_TRANSFER t_r_repair_transfer = new T_R_REPAIR_TRANSFER(Station.SFCDB,Station.DBType);
            if (t_r_repair_transfer.SNIsRepairIn(snObject.SerialNo, Station.SFCDB))
            {
                throw new Exception($@"{snObject.SerialNo},The Last Check In Not Check Out(closed=0)");
            }
        }

        /// <summary>
        /// Panel掃入維修檢查每個SN Completed,JOBFINISH,REWORK,RepairFailedFlag,ShippedFlag
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelInRepariChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            Panel panelObject = (Panel)sessionPanel.Value;
            List<R_SN> listSN= panelObject.GetSnDetail(panelObject.PanelNo, Station.SFCDB, Station.DBType);
            foreach (R_SN sn in listSN)
            {
                if (sn.COMPLETED_FLAG.Equals("1"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000069", new string[] { sn.SN }));
                }
                if (sn.NEXT_STATION.Equals("JOBFINISH"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000069", new string[] { sn.SN }));
                }
                if (sn.NEXT_STATION.Equals("REWORK"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180927150610", new string[] { sn.SN }));
                }
                if (sn.REPAIR_FAILED_FLAG.Equals("1"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000068", new string[] { sn.SN }));
                }
                if (sn.SHIPPED_FLAG.Equals("1"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000070", new string[] { sn.SN }));
                }
            }
        }

        /// <summary>
        /// Panel SN In Repair Main Checker
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelRepariMainChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {            
            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            Panel panelObject = (Panel)sessionPanel.Value;
            string failStation = sessionStation.Value.ToString();
            R_REPAIR_MAIN repairMain = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN>()
                .Where(r => r.SN == panelObject.PanelNo && r.FAIL_STATION != failStation && r.CLOSED_FLAG == "0").ToList().FirstOrDefault();
            if (repairMain != null)
            {
                throw new MESReturnMessage($@"Panel:{panelObject.PanelNo},Fail Station:{failStation},Not Repair!");
            }
           
        }

        public static void FailCodeRepairInputChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }

            MESStationSession sessionFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionFailCode == null || sessionFailCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            SN snObject = (SN)sessionSN.Value;
            C_ERROR_CODE errorCodeObject = (C_ERROR_CODE)sessionFailCode.Value;
            OleExec DB = Station.SFCDB;
            DB.ThrowSqlExeception = true;
            List<R_REPAIR_MAIN> repairMainObject = new List<R_REPAIR_MAIN>();
            R_REPAIR_FAILCODE failCodeObject = null;
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
            //repairMainObject = DB.ORM.Queryable<R_REPAIR_MAIN>().Where(r => r.CLOSED_FLAG == "0" && r.SN == snObject.SerialNo).ToList();
            //if (repairMainObject == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", snObject.SerialNo }));
            //}

            failCodeObject = DB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(r => r.MAIN_ID == repairMainObject[0].ID
              && r.SN == snObject.SerialNo && r.FAIL_CODE == errorCodeObject.ERROR_CODE
              && SqlSugar.SqlFunc.Subqueryable<R_REPAIR_ACTION>().Where(a => a.R_FAILCODE_ID == r.ID && a.SN == r.SN).NotAny()).ToList().FirstOrDefault();
            if (failCodeObject != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180929183305", new string[] { "SN", errorCodeObject.ERROR_CODE }));               
            }
        }

        /// <summary>
        /// 維修檢查SN是否是RMA產品
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairSNRMAChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
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

            T_R_RMA_BONEPILE t_r_rma = new T_R_RMA_BONEPILE(Station.SFCDB, Station.DBType);
            //判斷SN是否屬於RMA產品
            if (t_r_rma.RmaBonepileIsOpen(Station.SFCDB, objSN.SerialNo))
            {
                Station.StationSession.Find(s => s.MESDataType == "FAILCODE" && s.SessionKey == "1").Value = "";
                Station.StationSession.Find(s => s.MESDataType == "REPAIRACTION" && s.SessionKey == "1").Value = "";
                Station.StationSession.Find(s => s.MESDataType == "SNKP" && s.SessionKey == "1").Value = "";
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200304102250", new string[] { objSN.SerialNo }));//产品 {0} 已经入 RMA，请使用RMA-Repair模块！
            }
        }

        /// <summary>
        /// 維修檢查SN狀態:前綴,出貨
        /// 已有的Checker要麼不符合要求要麼僅符合部分但不好輕易修改,因此新建個Checker
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairSNStatusChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
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

            #region 判斷SN前綴
            if (objSN.SerialNo.StartsWith("WAX") || objSN.SerialNo.StartsWith("WCL") || objSN.SerialNo.StartsWith("2102"))
            {
                UIInputData O = new UIInputData()
                {
                    Timeout = 600000,
                    UIArea = new string[] { "25%", "28%" },
                    IconType = IconType.None,
                    Message = "確認",
                    Tittle = "",
                    Type = UIInputType.Confirm,
                    Name = "",
                    ErrMessage = "請點擊確認按鈕！"
                };
                O.OutInputs.Add(new DisplayOutPut() { Name = "溫馨提示：", DisplayType = UIOutputType.TextArea.ToString(), Value = "先組好CPU Board，再組電池作業！" });
                O.GetUiInput(Station.API, UIInput.Normal, Station);
            }
            #endregion

            #region 判斷SN是否出貨
            if (objSN.ShippedFlag == "1")
            {
                Station.StationSession.Find(s => s.MESDataType == "FAILCODE" && s.SessionKey == "1").Value = "";
                Station.StationSession.Find(s => s.MESDataType == "REPAIRACTION" && s.SessionKey == "1").Value = "";
                Station.StationSession.Find(s => s.MESDataType == "SNKP" && s.SessionKey == "1").Value = "";
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { objSN.SerialNo }));//{0} 已經出貨
            }
            #endregion
        }

        /// <summary>
        /// 檢查SN維修管控次數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RepairCountChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            #region 獲取SN對象 & 維修次數對象
            MESStationSession sSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sSN == null || sSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN objSN = (SN)sSN.Value;

            MESStationSession sRepairCount = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sRepairCount == null || sRepairCount.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            #endregion
            
            T_R_REPAIR_MAIN TRRM = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            R_REPAIR_MAIN rmObject = TRRM.GetSNBySN(objSN.SerialNo, Station.SFCDB);  //獲取R_REPAIR_MAIN對象

            //獲取SN在Fail工站最大維修次數管控對象
            C_REPAIR_SN_CONTROL rSnControl = Station.SFCDB.ORM.Queryable<C_REPAIR_SN_CONTROL>()
                .Where(t => t.SN == objSN.SerialNo && t.STATION == rmObject.FAIL_STATION && t.REPAIRCOUNT > 0)
                .ToList().FirstOrDefault();

            int repairCount = int.Parse(sRepairCount.Value.ToString());   //獲取維修次數
            //如果SN未配置Fail工站的最大維修次數
            if (rSnControl == null)
            {
                //判斷是否Fail工站屬於特殊管控維修次數工站，如果不屬於則按正常流程卡關(都是3次)
                C_CONTROL rMaxControl = Station.SFCDB.ORM.Queryable<C_CONTROL>()
                    .Where(t => t.CONTROL_NAME == "REPAIR_MAXTIMES" && t.CONTROL_TYPE == "STATION" && t.CONTROL_VALUE.Contains(rmObject.FAIL_STATION))
                    .ToList().FirstOrDefault();
                if (rMaxControl != null)
                {
                    if (repairCount >= int.Parse(rMaxControl.CONTROL_LEVEL))
                    {
                        Station.StationSession.Find(s => s.MESDataType == "FAILCODE" && s.SessionKey == "1").Value = "";
                        Station.StationSession.Find(s => s.MESDataType == "REPAIRACTION" && s.SessionKey == "1").Value = "";
                        Station.StationSession.Find(s => s.MESDataType == "SNKP" && s.SessionKey == "1").Value = "";
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200305093313", new string[] { objSN.SerialNo, rmObject.FAIL_STATION, repairCount.ToString() }));
                    }  
                }
                else
                {
                    if (repairCount >= 3)
                    {
                        Station.StationSession.Find(s => s.MESDataType == "FAILCODE" && s.SessionKey == "1").Value = "";
                        Station.StationSession.Find(s => s.MESDataType == "REPAIRACTION" && s.SessionKey == "1").Value = "";
                        Station.StationSession.Find(s => s.MESDataType == "SNKP" && s.SessionKey == "1").Value = "";
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200305093313", new string[] { objSN.SerialNo, rmObject.FAIL_STATION, "3" }));
                    }
                }
            }
            else
            {
                if (repairCount >= rSnControl.REPAIRCOUNT)
                {
                    Station.StationSession.Find(s => s.MESDataType == "FAILCODE" && s.SessionKey == "1").Value = "";
                    Station.StationSession.Find(s => s.MESDataType == "REPAIRACTION" && s.SessionKey == "1").Value = "";
                    Station.StationSession.Find(s => s.MESDataType == "SNKP" && s.SessionKey == "1").Value = "";
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200305094711", new string[] { objSN.SerialNo, rmObject.FAIL_STATION, rSnControl.REPAIRCOUNT.ToString() }));
                } 
            }

            //維修次數提示
            if (repairCount >= 1)
            {
                UIInputData O = new UIInputData()
                {
                    Timeout = 600000,
                    UIArea = new string[] { "25%", "28%" },
                    IconType = IconType.None,
                    Message = "確認",
                    Tittle = "",
                    Type = UIInputType.Confirm,
                    Name = "",
                    ErrMessage = "請點擊確認按鈕！"
                };
                O.OutInputs.Add(new DisplayOutPut() { Name = "溫馨提示：", DisplayType = UIOutputType.TextArea.ToString(), Value = "產品已維修 " + repairCount.ToString() + " 次！" });
                O.GetUiInput(Station.API, UIInput.Normal, Station);
            }
        }
        public static void IsRMA(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras) {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sSN == null || sSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN objSN = (SN)sSN.Value;  //獲取SN對象
            T_R_RMA_BONEPILE t_r_rma = new T_R_RMA_BONEPILE(Station.SFCDB, Station.DBType);
            //判斷SN是否屬於RMA產品
            if (t_r_rma.RmaBonepileIsOpen(Station.SFCDB, objSN.SerialNo))
            {
               
                throw new MESReturnMessage(objSN.baseSN+":  FAIL");
            }
        }

        /// <summary>
        /// 同工單(連續20pcs有5pcs Fail)或者(連續3pcs同一FailCode和Location Fail)鎖定,儘生效於SMT Fail工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SmtFailLockChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            //取得控制開關信息判斷是否開啟卡關 & 當前FAIL工站是否SMT1/SMT2
            var isCTL = Station.SFCDB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "SMTFAILLOCK" && t.CONTROL_TYPE == "SMT-WO" && t.CONTROL_VALUE == "Y").Any();
            if (isCTL && Station.StationName.StartsWith("SMT"))
            {

                var _SN = sessionSN.Value.ToString();
                //取得SN在此工站本次掃Fail的過站記錄
                var failRecord = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                    .Where(t => t.SN == _SN && t.VALID_FLAG == "1" && t.STATION_NAME == Station.StationName && t.REPAIR_FAILED_FLAG == "1")
                    .OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                if (failRecord != null)
                {
                    var t_snLog = new T_R_SN_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                    //判斷掃Fail SN對應工單是否已被記錄在R_SN_LOG表且狀態=Y
                    var snLog = Station.SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.LOGTYPE == "SMTWOLOCK" && t.FLAG == "Y" && t.DATA1 == failRecord.STATION_NAME && t.DATA2 == failRecord.WORKORDERNO)
                        //.OrderBy(t => Convert.ToDateTime(t.DATA4), SqlSugar.OrderByType.Desc).First();
                        .OrderBy(t => t.DATA4, SqlSugar.OrderByType.Desc).First();
                    if (snLog == null)
                    {
                        #region 寫入R_SN_LOG表
                        //記錄第一筆FAIL資料:把工單和時間寫入,方便下次解鎖後重新計算開始時間=failtime                
                        snLog = new R_SN_LOG
                        {
                            ID = t_snLog.GetNewID(Station.BU, Station.SFCDB),
                            SNID = failRecord.R_SN_ID,
                            SN = failRecord.SN,
                            LOGTYPE = "SMTWOLOCK",//類型
                            DATA1 = failRecord.STATION_NAME,//工站
                            DATA2 = failRecord.WORKORDERNO,//工單
                            DATA3 = failRecord.SKUNO,//機種
                            DATA4 = failRecord.EDIT_TIME.ToString(),//掃Fail時間
                            DATA5 = "初始寫入",//備註
                            FLAG = "Y",//初始寫入
                            CREATETIME = Station.GetDBDateTime(),
                            CREATEBY = Station.LoginUser.EMP_NO
                        };
                        var result = t_snLog.Save(Station.SFCDB, snLog);
                        if (result == 0)
                        {
                            //throw new MESReturnMessage("Save Sn Log Fail![初始寫入]");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155340"));
                        }
                        #endregion
                    }

                    //取得初始寫入時間點之後該工單所有SN在此工站的過站記錄，只按時間倒序取20筆
                    var snRecordList = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                        .Where(t => t.VALID_FLAG == "1" && t.WORKORDERNO == failRecord.WORKORDERNO && t.STATION_NAME == failRecord.STATION_NAME && t.EDIT_TIME >= Convert.ToDateTime(snLog.DATA4))
                        .OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();

                    var snRecordList_20 = snRecordList.Take(20).ToList();
                    var failRecordList = snRecordList_20.FindAll(t => t.REPAIR_FAILED_FLAG == "1").ToList();

                    //如果failRecordList.Count >= 5，說明初始寫入掃Fail信息後在此工站掃Fail數量已經大於等於5，則鎖定
                    if (failRecordList.Count >= 5)
                    {
                        #region 更新R_SN_LOG表                        
                        snLog.FLAG = "N";
                        snLog.DATA5 = "連續20PCS有5PCS FAIL,R_SN_LOCK鎖定!";
                        snLog.CREATETIME = Station.GetDBDateTime();
                        snLog.CREATEBY = "SYSTEM";
                        t_snLog.Update(Station.SFCDB, snLog);
                        #endregion

                        //取得5pcs 掃Fail的SN過站記錄對象
                        var failRecord_5 = failRecordList.Take(5).ToList();
                        for (int i = 0; i < failRecord_5.Count; i++)
                        {
                            #region 寫入R_SN_LOCK表,按WO鎖但記錄SN
                            var t_snLock = new T_R_SN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            var snLock = new R_SN_LOCK
                            {
                                ID = t_snLock.GetNewID(Station.BU, Station.SFCDB),
                                SN = failRecord_5[i].SN,
                                TYPE = "WO",
                                WORKORDERNO = failRecord_5[i].WORKORDERNO,
                                LOCK_STATION = failRecord_5[i].STATION_NAME,
                                LOCK_STATUS = "1",
                                LOCK_REASON = "連續20PCS有5PCS FAIL,請找QE確認!",
                                LOCK_EMP = Station.LoginUser.EMP_NO,
                                LOCK_TIME = Station.GetDBDateTime()
                            };
                            var result = Station.SFCDB.ORM.Insertable(snLock).ExecuteCommand();
                            if (result == 0)
                            {
                                //throw new MESReturnMessage("Insert R_SN_LOCK Fail![連續20PCS有5PCS FAIL]");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155515"));
                            }
                            #endregion
                        }

                        //throw new MESReturnMessage("該工單已連續20PCS就有5PCS FAIL,請找QE確認!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155537"));
                    }
                    
                    var snRecordList_3 = snRecordList.Take(3).ToList();
                    failRecordList = snRecordList_3.FindAll(t => t.REPAIR_FAILED_FLAG == "1").ToList();

                    //如果failRecordList.Count >= 3，說明初始寫入掃Fail信息後在此工站連續掃Fail數量等於3，則判斷是否同一FailCode&FailLocation
                    if (failRecordList.Count >= 3)
                    {
                        //先把3pcs SN單獨拿出來
                        var snFail_3 = string.Empty;
                        for (int i = 0; i < failRecordList.Count; i++)
                        {
                            snFail_3 = snFail_3 + failRecordList[i].SN + ",";
                        }
                        snFail_3 = snFail_3.Substring(0, snFail_3.Length - 1);

                        //取得3pcs SN的FailCode信息並去重
                        var distinctRecord = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE, R_REPAIR_MAIN>((t1, t2) => t1.MAIN_ID == t2.ID && t1.FAIL_TIME == t2.FAIL_TIME && t2.FAIL_STATION == Station.StationName)
                            .Where((t1, t2) => snFail_3.Contains(t1.SN)).GroupBy((t1, t2) => new { t1.FAIL_CODE, t1.F_LOCATION, t1.DESCRIPTION }).Select((t1, t2) => new { t1.FAIL_CODE, t1.F_LOCATION, t1.DESCRIPTION }).ToList();
                        
                        //如果3個SN的Fail記錄根據FailCode和FailLocation去重後只剩1筆，說明是同一FailCode和FailLocation
                        if (distinctRecord.Count == 1)
                        {
                            #region 更新R_SN_LOG表                            
                            snLog.FLAG = "N";
                            snLog.DATA5 = "連續3PCS同一FAILCODE FAIL,R_SN_LOCK鎖定!";
                            snLog.CREATETIME = Station.GetDBDateTime();
                            snLog.CREATEBY = "SYSTEM";
                            t_snLog.Update(Station.SFCDB, snLog);
                            #endregion

                            for (int i = 0; i < failRecordList.Count; i++)
                            {
                                var failInfo = Station.SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.SN == failRecordList[i].SN && t.FAIL_TIME == failRecordList[i].EDIT_TIME)
                                .Select(t => new { t.FAIL_CODE, t.F_LOCATION, t.DESCRIPTION }).First();

                                #region 寫入R_SN_LOCK表,按WO鎖但記錄SN
                                var t_snLock = new T_R_SN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                                var snLock = new R_SN_LOCK
                                {
                                    ID = t_snLock.GetNewID(Station.BU, Station.SFCDB),
                                    SN = failRecordList[i].SN,
                                    TYPE = "WO",
                                    WORKORDERNO = failRecordList[i].WORKORDERNO,
                                    LOCK_STATION = failRecordList[i].STATION_NAME,
                                    LOCK_STATUS = "1",
                                    LOCK_REASON = "連續3PCS同一FAILCODE&LOCATION FAIL,請找QE確認!",
                                    LOCK_EMP = Station.LoginUser.EMP_NO,
                                    LOCK_TIME = Station.GetDBDateTime()
                                };
                                var result = Station.SFCDB.ORM.Insertable(snLock).ExecuteCommand();
                                if (result == 0)
                                {
                                    //throw new MESReturnMessage("Insert R_SN_LOCK Fail![連續3PCS同一FAILCODE&LOCATION FAIL]");
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155553"));
                                }
                                #endregion
                            }

                            //throw new MESReturnMessage("該工單已連續3PCS FAIL且同一FAILCODE和位置, 請找QE確認!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155611"));
                        }
                    }
                }
            }
        }
    
        public static void CosmeticFailCheckTestRecord(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string sn = "";
            if(snSession.Value is string)
            {
                sn = snSession.Value.ToString().ToUpper();
            }
            else
            {
                sn= ((SN)snSession.Value).SerialNo;
            }
            var lastTestRecord = Station.SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(r => r.SN == sn).OrderBy(r => r.ENDTIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (lastTestRecord != null)
            {
                if ((lastTestRecord.STATE.ToUpper().Equals("FAIL") || lastTestRecord.STATE.ToUpper().Equals("F")) && !lastTestRecord.MESSTATION.Equals(Station.StationName))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20220216092040", new string[] { sn, lastTestRecord.MESSTATION }));
                }
            }
        }
    }
}
