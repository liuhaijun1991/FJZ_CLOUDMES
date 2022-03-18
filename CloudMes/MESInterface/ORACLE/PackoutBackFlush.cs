using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.SAP_RFC;
using MESPubLab.MESInterface;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using System.Data;
using MESPubLab;

namespace MESInterface.ORACLE
{
    public class PackoutBackFlush: taskBase
    {
        #region 外部調用時，請給以下賦值 
        public ZRFC_SFC_NSGT_0002 ZRFC_SFC_NSGT_0002;
        //Add by James Zhu 10/30/2019
        public ZRFC_SFC_NSGT_0004 ZRFC_SFC_NSGT_0004;
        public OleExec SFCDB = null;
        public string BU = "";
        public string Plant = "";
        public string DB = "";
        public T_R_SYNC_LOCK synLock;
        public T_R_SN R_SN;
        public T_H_MRB_GT H_MRB_GT;
        public string ip = "";
        #endregion

        private bool IsRuning = false;
        private string lockIp = "";
        private string postDate;
        private string sql = "";
        private string logMessage = "";
        private DataTable MRBTable;
        private List<R_SN> SNList;
        private Row_R_SN rowR_SN;
        private Row_H_MRB_GT rowH_MRBGT;

        public override void init()
        {
            //base.init();
            try
            {
                BU = ConfigGet("BU");
                Plant = ConfigGet("PLANT");
                DB = ConfigGet("DB");
                SFCDB = new OleExec(DB, false);
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                R_SN = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
                H_MRB_GT = new T_H_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                ZRFC_SFC_NSGT_0004 = new ZRFC_SFC_NSGT_0004(BU);
                ZRFC_SFC_NSGT_0002 = new ZRFC_SFC_NSGT_0002(BU);
               
                Output.UI = new PackoutBackFlush_UI(this);
            }
            catch (Exception ex)
            {
                throw new Exception("Init PackoutBackFlush fail," + ex.Message);
            }
        }

        public override void Start()
        {
            //base.Start();
            IsRuning = synLock.IsLock("HWD_WOToMRBBackFlush", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception("HWD WOToPackoutBackFlush interface is running on " + lockIp + ",Please try again later");
            }

            try
            {
                synLock.SYNC_Lock(BU, ip, "HWD_WOToMRBBackFlush", "HWD_WOToMRBBackFlush", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
                SFCDB.CommitTrain();
                DoMrbBackFlush();
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                throw new Exception("Start PackoutBackFlush Fail" + ex.Message);
            }
            synLock.SYNC_UnLock(BU, ip, "HWD_WOToMRBBackFlush", "HWD_WOToMRBBackFlush", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
            SFCDB.CommitTrain();
        }

        private void DoMrbBackFlush()
        {
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                //月結不給拋賬
                throw new Exception("This time is monthly,can't BackFlush");
            }
            else
            {
                WOToPackoutBackFlush();
            }
        }

        private void WOToPackoutBackFlush()
        {
            SNList = new List<R_SN>();
            MRBTable = new DataTable();

            //sql = $@" update r_mrb_gt set sap_flag = '3' where (sap_flag = '2' or sap_flag = '0') and zcpp_flag = 0   and sap_station_code is not null  ";
            //SFCDB.ExecSQL(sql);
            // Changed by James Zhu 10/24/2019 for reconfig w/o
           // sql = $@" select * from R_SN where completed_flag = 1 and stock_status is null and next_station = 'JOBFINISH' and workorderno not like 'SM%' and workorderno not like 'CMOD%' ";
            sql = $@" SELECT  A.SN,A.SKUNO,A.COMPLETED_TIME,A.WORKORDERNO,CASE B.AUART WHEN 'TOG5' THEN 'REWORK' WHEN' TOG6' THEN 'REWORK' ELSE 'REGUALR' END PRODUCT_STATUS   FROM ( select  SN,SKUNO, COMPLETED_TIME, CASE WHEN EXISTS(SELECT * FROM R_WO_BASE WHERE WORKORDERNO=R_SN.WORKORDERNO  ";
            sql = sql + " AND ROHS IS NOT NULL) THEN (SELECT CASE WHEN LENGTH(ROHS)<12 THEN  SUBSTR('0000'||ROHS,-12) ELSE ROHS END FROM R_WO_BASE WHERE WORKORDERNO=R_SN.WORKORDERNO ) ELSE WORKORDERNO END WORKORDERNO from R_SN where completed_flag = 1  and stock_status is null and next_station = 'JOBFINISH' and workorderno not like 'SM%' and workorderno not like 'CMOD%') A LEFT JOIN R_WO_HEADER B  ON INSTR(B.AUFNR,A.WORKORDERNO)>0";
            MRBTable = SFCDB.RunSelect(sql).Tables[0];
            if (MRBTable != null && MRBTable.Rows.Count > 0)
            {
                foreach (DataRow row in MRBTable.Rows)
                {
                    rowR_SN = (Row_R_SN)R_SN.NewRow();
                    rowR_SN.loadData(row);
                    SNList.Add(rowR_SN.GetDataObject());
                }

                //sql = " update R_SN set stock_status = '1' where sap_flag = '3' and zcpp_flag = 0  and sap_station_code is not null   ";
                //SFCDB.ExecSQL(sql);
            }

            postDate = InterfacePublicValues.GetPostResumeDate(SFCDB);

            if (SNList != null && SNList.Count > 0)
            {
                foreach (R_SN R_SN in SNList)
                {
                    if (R_SN.PRODUCT_STATUS == "REWORK")
                    {
                        rowR_SN = null;
                        rowH_MRBGT = null;
                        logMessage = "";
                        ZRFC_SFC_NSGT_0004.SetValue("I_AUFNR", R_SN.WORKORDERNO);                     
                        ZRFC_SFC_NSGT_0004.SetValue("I_BUDAT", postDate);                       
                        ZRFC_SFC_NSGT_0004.SetValue("I_LMNGA", "1");
                      
                        try
                        {
                            ZRFC_SFC_NSGT_0004.CallRFC();
                        }
                        catch (Exception ex)
                        {
                            WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", R_SN.WORKORDERNO + ";" + ip + ";" + ex.ToString(), "", "interface");
                            continue;
                        }
                        // zrfc_sfc_nsg_0020 中包含三個動作101，521，轉倉，flag，flag1，flag2 一次對應這三個動作
                        // flag，flag1，flag2 這幾個flag 0表示OK，1表示false 
                        logMessage = " M:" + ZRFC_SFC_NSGT_0004.GetValue("O_MESSAGE")                                      
                                        + " E:" + ZRFC_SFC_NSGT_0004.GetValue("O_ERROR");

                        //rowR_SN = (Row_R_SN)R_SN.GetObjByID(r_mrb_gt.ID, SFCDB);
                        //rowR_SN.SAP_MESSAGE = logMessage;
                        if (ZRFC_SFC_NSGT_0004.GetValue("O_FLAG") == "0")
                        {
                            try
                            {
                                sql = " update R_SN set stock_status = 1, stock_in_time =TO_DATE('" + System.DateTime.Now.ToString() + "', 'MM/DD/YY HH:MI:SS AM')  where SN  ='" + R_SN.SN + "'";
                                SFCDB.ExecSQL(sql);
                            }
                            catch (Exception ex)
                            {

                                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", R_SN.WORKORDERNO + ";" + ip + ";update r_sn fail," + ex.Message, "", "interface");
                            }
                        }
                    }
                    else
                    {
                        rowR_SN = null;
                        rowH_MRBGT = null;
                        logMessage = "";
                        ZRFC_SFC_NSGT_0002.SetValue("I_AUFNR", R_SN.WORKORDERNO);
                        ZRFC_SFC_NSGT_0002.SetValue("I_BUDAT", postDate);                      
                        ZRFC_SFC_NSGT_0002.SetValue("I_LMNGA", "1");                      
                        try
                        {
                            ZRFC_SFC_NSGT_0002.CallRFC();
                        }
                        catch (Exception ex)
                        {
                            WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", R_SN.WORKORDERNO + ";" + ip + ";" + ex.ToString(), "", "interface");
                            continue;
                        }
                        // zrfc_sfc_nsg_0020 中包含三個動作101，521，轉倉，flag，flag1，flag2 一次對應這三個動作
                        // flag，flag1，flag2 這幾個flag 0表示OK，1表示false 
                        logMessage = " M:" + ZRFC_SFC_NSGT_0002.GetValue("O_MESSAGE")                                     
                                        + " E:" + ZRFC_SFC_NSGT_0002.GetValue("O_ERROR");

                        //rowR_SN = (Row_R_SN)R_SN.GetObjByID(r_mrb_gt.ID, SFCDB);
                        //rowR_SN.SAP_MESSAGE = logMessage;
                        if (ZRFC_SFC_NSGT_0002.GetValue("O_FLAG") == "0")
                        {
                            try
                            {
                                sql = " update R_SN set stock_status = 1, stock_in_time =TO_DATE('" + System.DateTime.Now.ToString() + "', 'MM/DD/YY HH:MI:SS AM')  where SN  ='" + R_SN.SN + "'";
                                SFCDB.ExecSQL(sql);
                            }
                            catch (Exception ex)
                            {

                                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", R_SN.WORKORDERNO + ";" + ip + ";update r_sn fail," + ex.Message, "", "interface");
                            }
                        }
                    }
                    //try
                    //{
                    //    SFCDB.ExecSQL(rowR_SN.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    //}
                    //catch (Exception ex)
                    //{
                    //    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", R_SN.WORKORDERNO + ";" + ip + ";update r_mrb_gt fail," + ex.Message, "", "interface");
                    //}

                    try
                    {

                        rowH_MRBGT = (Row_H_MRB_GT)H_MRB_GT.NewRow();
                        rowH_MRBGT.ID = H_MRB_GT.GetNewID(BU, SFCDB);
                        rowH_MRBGT.WORKORDERNO = R_SN.WORKORDERNO;
                        rowH_MRBGT.SAP_STATION_CODE = R_SN.SN;
                        //rowH_MRBGT.SAP_STATION_CODE = r_mrb_gt.SAP_STATION_CODE;
                        //rowH_MRBGT.FROM_STORAGE = r_mrb_gt.FROM_STORAGE;
                        //rowH_MRBGT.TO_STORAGE = r_mrb_gt.TO_STORAGE;
                        //rowH_MRBGT.TOTAL_QTY = r_mrb_gt.TOTAL_QTY;
                        //rowH_MRBGT.CONFIRMED_FLAG = r_mrb_gt.CONFIRMED_FLAG;
                        //rowH_MRBGT.ZCPP_FLAG = r_mrb_gt.ZCPP_FLAG;
                        //rowH_MRBGT.SAP_FLAG = r_mrb_gt.SAP_FLAG;
                        rowH_MRBGT.SKUNO = R_SN.SKUNO;
                        rowH_MRBGT.SAP_MESSAGE = logMessage;
                        rowH_MRBGT.EDIT_EMP = "interface";
                        rowH_MRBGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowH_MRBGT.GetInsertString(DB_TYPE_ENUM.Oracle));
                        //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush" , r_mrb_gt.WORKORDERNO + ";" + ip + ";" + logMessage, "", "interface");
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", R_SN.WORKORDERNO + ";" + ip + ";inert into h_mrb_gt fail," + ex.Message, "", "interface");
                    }
                    SFCDB.CommitTrain();
                }
            }
        }       
    }
}

