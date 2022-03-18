using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.SAP_RFC;
using MESPubLab.MESInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab;

namespace MESInterface.HWD
{
    public class StockInBackFlush : taskBase
    {
        private DateTime startTime;
        private DateTime endTime;
        private DateTime dateTimeNow;
        private bool IsRuning = false;
        private bool BackFlushOK = true;
        private string lockIp = "";
        private string logMsg = "";
        private string sapMsg = "";
        private List<R_SAP_TEMP> backFlushList;
        public DataTable stockInTable;

        public string ip = "";
        public string BU = "";
        public string Plant = "";
        public string DB = "";
        public OleExec SFCDB = null;
        public ZCMM_NSBG_0051 zcmm_nsbg_0051 = null;
        public T_R_SYNC_LOCK synLock;
        public T_R_SAP_TEMP t_r_sap_temp;
        public Row_R_SAP_TEMP r_sap_temp;
        public T_R_SAPLOG t_r_saplog;
        public Row_R_SAPLOG r_saplog;
        public T_C_CONTROL t_c_control;
        public T_R_SN_STATION_DETAIL t_r_sn_station_detail;

        public override void init()
        {
            try
            {
                BU = ConfigGet("BU");
                Plant = ConfigGet("PLANT");
                DB = ConfigGet("DB");
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                SFCDB = new OleExec(DB, false);
                zcmm_nsbg_0051 = new ZCMM_NSBG_0051(BU);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                t_r_sap_temp = new T_R_SAP_TEMP(SFCDB, DB_TYPE_ENUM.Oracle);
                t_r_saplog = new T_R_SAPLOG(SFCDB, DB_TYPE_ENUM.Oracle);
                t_c_control = new T_C_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
                t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                Output.UI = new StockInBackFlush_UI(this);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public override void Start()
        {
            try
            {
                GetBackFlustData();
                CallRFCBackFlush();               
            }
            catch (Exception ex)
            {
                throw new Exception("Start StockInBackFlush Fail" + ex.Message);
            }

        }

        public void GetBackFlustData()
        {
            SFCDB.BeginTrain();
            try
            {
                C_CONTROL control = t_c_control.GetControlByName("HWD_STOCKIN_TIME", SFCDB);
                if (control.CONTROL_VALUE == "" && string.IsNullOrEmpty(control.CONTROL_VALUE))
                {
                    throw new Exception("Get last run time fail ");
                }
                startTime = Convert.ToDateTime(control.CONTROL_VALUE);
                endTime = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                stockInTable = t_r_sn_station_detail.GetStockInQtyByTime(startTime,endTime,SFCDB);
                if (stockInTable != null)
                {
                    foreach (DataRow dr in stockInTable.Rows)
                    {
                        r_sap_temp = (Row_R_SAP_TEMP)t_r_sap_temp.NewRow();
                        r_sap_temp.ID = t_r_sap_temp.GetNewID(BU, SFCDB);
                        r_sap_temp.SKUNO = dr["skuno"].ToString();
                        r_sap_temp.TYPE = "011G To 016G";
                        r_sap_temp.QTY = double.Parse(dr["qty"].ToString());
                        r_sap_temp.FROM_STORAGE = "011G";
                        r_sap_temp.TO_STORAGE = "016G";
                        r_sap_temp.SAP_FLAG = "0";
                        r_sap_temp.FAIL_COUNT = 0;
                        r_sap_temp.EDIT_EMP = "interface";
                        r_sap_temp.EDIT_TIME = endTime;
                        SFCDB.ExecSQL(r_sap_temp.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }
                Row_C_CONTROL rowControl = (Row_C_CONTROL)t_c_control.GetObjByID(control.ID, SFCDB);
                rowControl.CONTROL_VALUE = endTime.ToString("yyyy/MM/dd HH: mm: ss");
                rowControl.EDIT_EMP = "intrerface";
                rowControl.EDIT_TIME = endTime;
                SFCDB.ExecSQL(rowControl.GetUpdateString(DB_TYPE_ENUM.Oracle));
                SFCDB.CommitTrain();
            }
            catch (Exception exception)
            {
                SFCDB.RollbackTrain();
                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.StockInBackFlush", "GetBackFlustData", ip + ";" + exception.Message, "", "interface");
                StockInBackFlush_UI.OutPutMessage("", exception.Message, false);
            }
        }

        public void CallRFCBackFlush()
        {
            IsRuning = synLock.IsLock("HWD_StockInBackFlush", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception("HWD StockInBackFlush interface is running on " + lockIp + ",Please try again later");
            }
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                //月結不給拋賬
                throw new Exception("This time is monthly,can't BackFlush");
            }

            synLock.SYNC_Lock(BU, ip, "HWD_StockInBackFlush", "HWD_StockInBackFlush", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
            SFCDB.CommitTrain();
            backFlushList = t_r_sap_temp.GetStockInBackFlushList(SFCDB);
            dateTimeNow = DateTime.Parse(InterfacePublicValues.GetPostDate(SFCDB));
            if (backFlushList != null)
            {
                foreach(R_SAP_TEMP temp in backFlushList)
                {
                    logMsg = "";
                    sapMsg = "";
                    r_sap_temp = null;
                    zcmm_nsbg_0051.SetValue(Plant, temp.SKUNO, temp.FROM_STORAGE, temp.TO_STORAGE, temp.QTY.ToString(), temp.TYPE, dateTimeNow, "311");
                    zcmm_nsbg_0051.CallRFC();

                    r_sap_temp = (Row_R_SAP_TEMP)t_r_sap_temp.GetObjByID(temp.ID, SFCDB);
                    if (zcmm_nsbg_0051.GetValue("O_FLAG") == "1")
                    {
                        //失敗
                        sapMsg = " T:" + dateTimeNow.ToString("yyyy/MM/dd HH:mm:ss") + "M1:" + zcmm_nsbg_0051.GetValue("O_MESSAGE");
                        try
                        {
                            sapMsg += " M2:" + zcmm_nsbg_0051.GetTableValue("OUT_TAB").Rows[0]["MESSAGE"].ToString();
                        }
                        catch
                        {
                            sapMsg += " M2:null";
                        } 
                        r_sap_temp.FAIL_COUNT = r_sap_temp.FAIL_COUNT + 1;
                        if (r_sap_temp.FAIL_COUNT >= 100)
                        {
                            r_sap_temp.SAP_FLAG = "3";
                        }
                        BackFlushOK = false;
                    }
                    else
                    {
                        //成功
                        sapMsg = "OK:" + zcmm_nsbg_0051.GetValue("O_MBLNR") + " T:" + dateTimeNow.ToString("yyyy/MM/dd HH:mm:ss");
                        r_sap_temp.EDIT_TIME = dateTimeNow;
                        r_sap_temp.SAP_FLAG = "2";
                        BackFlushOK = true;
                    }
                    SFCDB.ExecSQL(r_sap_temp.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    logMsg = temp.SKUNO + ":" + temp.QTY.ToString() + ":" + sapMsg;
                    // write mes log
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.StockInBackFlush", "CallRFCB", ip + ";" + logMsg, "", "interface");

                    // write r_saplog
                    try
                    {
                        r_saplog = (Row_R_SAPLOG)t_r_saplog.NewRow();
                        r_saplog.ID = t_r_saplog.GetNewID(BU, SFCDB);
                        r_saplog.WORKTIME = temp.EDIT_TIME;
                        r_saplog.SKUNO = temp.SKUNO;
                        r_saplog.QTY = temp.QTY;
                        r_saplog.ERRORTYPE = "STOCKIN";
                        r_saplog.ERRORMESSAGE = sapMsg;
                        SFCDB.ExecSQL(r_saplog.GetInsertString(DB_TYPE_ENUM.Oracle));
                        StockInBackFlush_UI.OutPutMessage(temp.SKUNO + ":" + temp.QTY.ToString(), sapMsg, BackFlushOK);
                    }
                    catch (Exception ex)
                    {
                        logMsg = temp.SKUNO + ":" + temp.QTY + ";insert into r_saplog fail" + ex.Message;
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.StockInBackFlush", "CallRFCB", ip + ";" + logMsg, "", "interface");
                    }
                    SFCDB.CommitTrain();
                }
            }
            synLock.SYNC_UnLock(BU, ip, "HWD_StockInBackFlush", "HWD_StockInBackFlush", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
            SFCDB.CommitTrain();
        }
    }
}
