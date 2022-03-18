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

namespace MESInterface.HWD
{
    public class MrbBackFlush:taskBase
    {
        #region 外部調用時，請給以下賦值 
        public ZRFC_SFC_NSG_0020 zrfc_sfc_nsg_0020;       
        public OleExec SFCDB = null;        
        public string BU = "";
        public string Plant = "";
        public string DB = "";
        public T_R_SYNC_LOCK synLock;
        public T_R_MRB_GT R_MRB_GT;
        public T_H_MRB_GT H_MRB_GT;
        public string ip = "";
        #endregion

        private bool IsRuning = false;
        private string lockIp = "";
        private string postDate;
        private string sql = "";
        private string logMessage = "";       
        private DataTable MRBTable;
        private List<R_MRB_GT> MRBGTList;
        private Row_R_MRB_GT rowR_MRBGT;
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
                R_MRB_GT = new T_R_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                H_MRB_GT = new T_H_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                zrfc_sfc_nsg_0020 = new ZRFC_SFC_NSG_0020(BU);
                Output.UI = new MrbBackFlush_UI(this);
            }
            catch(Exception ex)
            {
                throw new Exception("Init MrbBackFlus fail," + ex.Message);
            }
        }

        public override void Start()
        {
            //base.Start();
            IsRuning = synLock.IsLock("HWD_WOToMRBBackFlush", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception("HWD WOToMRBBackFlush interface is running on " + lockIp + ",Please try again later");
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
                throw new Exception("Start MrbBackFlush Fail" + ex.Message);
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
                WOToMRBBackFlush();               
            }
        }

        private void WOToMRBBackFlush()
        {
            MRBGTList = new List<R_MRB_GT>();
            MRBTable = new DataTable();

            if (BU.Equals("BPD"))
            {
                //之前拋帳出現工單虧損的重新拋
                sql = $@"update r_mrb_gt set sap_flag='0' where sap_flag='4' and zcpp_flag='0' and sap_station_code is not null and sap_message like '%Deficit%'";
                SFCDB.ExecSQL(sql);
            }
            
            sql = $@" update r_mrb_gt set sap_flag = '2' where  sap_flag = '0' and zcpp_flag = 0   and sap_station_code is not null  ";
            SFCDB.ExecSQL(sql);
            SFCDB.CommitTrain();

            sql = $@" select * from r_mrb_gt where sap_flag = 2 and zcpp_flag = 0  and sap_station_code is not null   ";
            MRBTable = SFCDB.RunSelect(sql).Tables[0];
            if (MRBTable != null && MRBTable.Rows.Count > 0)
            {
                foreach(DataRow row in MRBTable.Rows)
                {
                    rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.NewRow();
                    rowR_MRBGT.loadData(row);
                    MRBGTList.Add(rowR_MRBGT.GetDataObject());
                }

                //sql = " update r_mrb_gt set sap_flag = '4' where sap_flag = '3' and zcpp_flag = 0  and sap_station_code is not null   ";
                //SFCDB.ExecSQL(sql);
            }

            postDate = InterfacePublicValues.GetPostDate(SFCDB);

            if (MRBGTList != null && MRBGTList.Count > 0)
            {
                foreach (R_MRB_GT r_mrb_gt in MRBGTList)
                {
                    rowR_MRBGT = null;
                    rowH_MRBGT = null;
                    logMessage = "";
                    zrfc_sfc_nsg_0020.SetValue("I_AUFNR", r_mrb_gt.WORKORDERNO);
                    zrfc_sfc_nsg_0020.SetValue("I_BUDAT", postDate);
                    zrfc_sfc_nsg_0020.SetValue("I_FLAG", r_mrb_gt.CONFIRMED_FLAG);
                    zrfc_sfc_nsg_0020.SetValue("I_LGORT_TO", r_mrb_gt.TO_STORAGE);
                    zrfc_sfc_nsg_0020.SetValue("I_LMNGA", r_mrb_gt.TOTAL_QTY.ToString());
                    zrfc_sfc_nsg_0020.SetValue("I_STATION", r_mrb_gt.SAP_STATION_CODE);
                    rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.GetObjByID(r_mrb_gt.ID, SFCDB);
                    try
                    {
                        //默認拋賬是成功的,因為如果RFC返回的信息如有變動，而拋賬程序沒有及時更新的話，就可能會導致重複拋賬
                        rowR_MRBGT.SAP_FLAG = "1";
                        zrfc_sfc_nsg_0020.CallRFC();
                    }
                    catch (Exception ex)
                    {                        
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush" , r_mrb_gt.WORKORDERNO + ";" + ip + ";" + ex.ToString(), "", "interface");
                        //continue;
                    }
                    // zrfc_sfc_nsg_0020 中包含三個動作101，521，轉倉，flag，flag1，flag2 一次對應這三個動作
                    // flag，flag1，flag2 這幾個flag 0表示OK，1表示false 
                    logMessage = "Date:"+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" M:" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE")
                                   + " M1:" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE1")
                                   + " M2" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE2");
                   
                    rowR_MRBGT.SAP_MESSAGE = logMessage;
                    //if (zrfc_sfc_nsg_0020.GetValue("O_FLAG") == "0")
                    //{                                            
                    //    rowR_MRBGT.SAP_MESSAGE = logMessage;
                    //}                   
                    try
                    {
                        SFCDB.ExecSQL(rowR_MRBGT.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", r_mrb_gt.WORKORDERNO + ";" + ip + ";update r_mrb_gt fail," + ex.Message, "", "interface");
                    }
                    try
                    {
                        
                        rowH_MRBGT = (Row_H_MRB_GT)H_MRB_GT.NewRow();
                        rowH_MRBGT.ID = H_MRB_GT.GetNewID(BU, SFCDB);
                        rowH_MRBGT.WORKORDERNO = r_mrb_gt.WORKORDERNO;
                        rowH_MRBGT.SAP_STATION_CODE = r_mrb_gt.SAP_STATION_CODE;
                        rowH_MRBGT.FROM_STORAGE = r_mrb_gt.FROM_STORAGE;
                        rowH_MRBGT.TO_STORAGE = r_mrb_gt.TO_STORAGE;
                        rowH_MRBGT.TOTAL_QTY = r_mrb_gt.TOTAL_QTY;
                        rowH_MRBGT.CONFIRMED_FLAG = r_mrb_gt.CONFIRMED_FLAG;
                        rowH_MRBGT.ZCPP_FLAG = r_mrb_gt.ZCPP_FLAG;
                        rowH_MRBGT.SAP_FLAG = rowR_MRBGT.SAP_FLAG;
                        rowH_MRBGT.SKUNO = r_mrb_gt.SKUNO;
                        rowH_MRBGT.SAP_MESSAGE = logMessage;
                        rowH_MRBGT.EDIT_EMP = "interface";
                        rowH_MRBGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowH_MRBGT.GetInsertString(DB_TYPE_ENUM.Oracle));
                        //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush" , r_mrb_gt.WORKORDERNO + ";" + ip + ";" + logMessage, "", "interface");
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush" , r_mrb_gt.WORKORDERNO + ";" + ip + ";inert into h_mrb_gt fail," + ex.Message, "", "interface");
                    }
                    SFCDB.CommitTrain();
                }
            }
        }
                
    }
}
