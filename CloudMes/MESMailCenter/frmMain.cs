using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MESMailCenter
{
    public partial class frmMain : Form
    {
        Configs config = new Configs();
        Dictionary<string, ProcessManagedItem> SQLAlarmManagedItems = new Dictionary<string, ProcessManagedItem>();
        //SQLAlarmProcess CurrentItem = null;
        Process CurrentItem = null;
        public frmMain()  
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
            Init();

        }
        void Init()
        {
            dgTaskView.Rows.Clear();
            try
            {
                try
                {
                    config.ReadXml(".\\SQLTaskConfig.xml");
                }
                catch
                {

                }
                //DataRow dr1 = config.THAlertConfig.NewRow();
                //dr1["Name"] = "HWD_TH_ALERT";
                //dr1["TimeSpan"] = "3600000";
                //dr1["DBKEY"] = "TTT";
                //dr1["Pactory"] = "WDN1";
                //config.THAlertConfig.Rows.Add(dr1);
                //config.WriteXml(".\\SQLTaskConfig.xml", XmlWriteMode.WriteSchema);
                //return;

                List<SQLAlarmProcess> SQLAlarms = new List<SQLAlarmProcess>();
               
                //List<THAlertProccess> THAlert = new List<THAlertProccess>();
                //foreach (DataRow dr in config.THAlertConfig.Rows)
                //{
                //    THAlertProccess process = new THAlertProccess(dr);
                //    THAlert.Add(process);
                //    HWDNNSFCBase.ProcessManagedItem ManagedItem = null;
                //    try
                //    {
                //        ManagedItem = new HWDNNSFCBase.ProcessManagedItem(process, int.Parse(process._TimeSpan));
                //    }
                //    catch
                //    {
                //        ManagedItem = new HWDNNSFCBase.ProcessManagedItem(process, 9999);
                //        ManagedItem.ManagerType = ProccessManagerTypeEnum.TimeList;

                //        string[] TIMES = process._TimeSpan.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //        for (int i = 0; i < TIMES.Length; i++)
                //        {
                //            try
                //            {
                //                ManagedItem.StartList.Add(TIMES[i]);
                //            }
                //            catch
                //            { }
                //        }

                //    }
                //    SQLAlarmManagedItems.Add(process.ToString(), ManagedItem);
                //    ManagedItem.RunningStateChange += new HWDNNSFCBase.ProcessManagedItem.RunningStateChangeHandler(ManagedItem_RunningStateChange);
                //    dgTaskView.Rows.Add();
                //    int rindex = dgTaskView.Rows.Count - 1;
                //    dgTaskView.Rows[rindex].Cells[0].Value = process;
                //    dgTaskView.Rows[rindex].Cells[1].Value = process._TimeSpan;
                //    dgTaskView.Rows[rindex].Cells[3].Value = "N";

                //}
                foreach (DataRow dr in config.SQLTaskConfig.Rows)
                {
                    SQLAlarmProcess process = new SQLAlarmProcess(dr);
                    SQLAlarms.Add(process);
                    ProcessManagedItem ManagedItem = null;
                    try
                    {
                        ManagedItem = new ProcessManagedItem(process, int.Parse( process._TimeSpan));
                    }
                    catch
                    {
                        ManagedItem = new ProcessManagedItem(process, 9999);
                        ManagedItem.ManagerType = ProccessManagerTypeEnum.TimeList;
                        
                        string[] TIMES = process._TimeSpan.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < TIMES.Length; i++)
                        {
                            try
                            {
                                ManagedItem.StartList.Add(TIMES[i]);
                            }
                            catch
                            { }
                        }

                    }
                    SQLAlarmManagedItems.Add(process.ToString(), ManagedItem);
                    ManagedItem.RunningStateChange += new ProcessManagedItem.RunningStateChangeHandler(ManagedItem_RunningStateChange);
                    dgTaskView.Rows.Add();
                    int rindex = dgTaskView.Rows.Count - 1;
                    dgTaskView.Rows[rindex].Cells[0].Value = process;
                    dgTaskView.Rows[rindex].Cells[1].Value =process._TimeSpan;
                    dgTaskView.Rows[rindex].Cells[3].Value = "N";

                }
                
                foreach (DataRow dr in config.SendReportFileConfig.Rows)
                {
                    SendReportProcess process = new SendReportProcess(dr);
                    //SQLAlarms.Add(process);
                    ProcessManagedItem ManagedItem = null;
                    try
                    {
                        ManagedItem = new ProcessManagedItem(process, int.Parse(process._TimeSpan));
                    }
                    catch
                    {
                        ManagedItem = new ProcessManagedItem(process, 9999);
                        ManagedItem.ManagerType = ProccessManagerTypeEnum.TimeList;

                        string[] TIMES = process._TimeSpan.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < TIMES.Length; i++)
                        {
                            try
                            {
                                ManagedItem.StartList.Add(TIMES[i]);
                            }
                            catch
                            { }
                        }

                    }
                    SQLAlarmManagedItems.Add(process.ToString(), ManagedItem);
                    ManagedItem.RunningStateChange += new ProcessManagedItem.RunningStateChangeHandler(ManagedItem_RunningStateChange);
                    dgTaskView.Rows.Add();
                    int rindex = dgTaskView.Rows.Count - 1;
                    dgTaskView.Rows[rindex].Cells[0].Value = process;
                    dgTaskView.Rows[rindex].Cells[1].Value = process._TimeSpan;
                    dgTaskView.Rows[rindex].Cells[3].Value = "N";

                }
                

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        void ManagedItem_RunningStateChange(object sender, RunningStateChangeEventArgs ev)
        {
            if (ev.Message == "成功執行")
            {
                for (int i = 0; i < dgTaskView.Rows.Count; i++)
                {
                    if (dgTaskView.Rows[i].Cells[0].Value == ev.process)
                    {
                        dgTaskView.Rows[i].Cells[2].Value = DateTime.Now;
                    }
                }
            }
            if (ev.Message == "調用Start")
            {
                for (int i = 0; i < dgTaskView.Rows.Count; i++)
                {
                    if (dgTaskView.Rows[i].Cells[0].Value == ev.process)
                    {
                        dgTaskView.Rows[i].Cells[3].Value = "Y";
                    }
                }
            }
            if (ev.Message == "調用Stop")
            {
                for (int i = 0; i < dgTaskView.Rows.Count; i++)
                {
                    if (dgTaskView.Rows[i].Cells[0].Value == ev.process)
                    {
                        dgTaskView.Rows[i].Cells[3].Value = "N";
                    }
                }
            }
            
            if (ev.Message.IndexOf("執行出現異常") >=0)
            {
                for (int i = 0; i < dgTaskView.Rows.Count; i++)
                {
                    if (dgTaskView.Rows[i].Cells[0].Value == ev.process)
                    {
                        dgTaskView.Rows[i].Cells[3].Value = "N";
                        dgTaskView.Rows[i].Cells[4].Value = ev.Message;
                    }
                }
            }

        }
        void startAll()
        {
            foreach (ProcessManagedItem item in SQLAlarmManagedItems.Values)
            {
                item.Start();
            }
        }
        void stopAll()
        {
            foreach (ProcessManagedItem item in SQLAlarmManagedItems.Values)
            {
                item.Stop();
            }
        }
        public void ConfigsEx()
        {
            Configs s = new Configs();
            DataRow dr = s.SQLTaskConfig.NewRow();
            dr["Name"] = "三個月未異動物料報警";
            dr["TimeSpan"] = 86400000;
            dr["SQL"] = " SELECT TR_SN \"料盤號\",\n" +
            "         CUST_KP_NO \"料號\",\n" +
            "         DATE_CODE,\n" +
            "         LOT_CODE,\n" +
            "         EXT_QTY \"數量\",\n" +
            "         STANDARD_DC \"標準date code\",\n" +
            "         START_TIME \"進料日期\",\n" +
            "         DECODE (LOCATION_FLAG,  '1', 'KITTING',  '0', 'WHS') \"位置\"\n" +
            "    FROM mes4.r_tr_sn a\n" +
            "   WHERE     (   (    location_flag = '1'\n" +
            "                  AND kitting_flag = 'a'\n" +
            "                  AND work_flag = '0')\n" +
            "              OR EXISTS\n" +
            "                    (SELECT 1\n" +
            "                       FROM mes4.r_whs_location b\n" +
            "                      WHERE     b.LOCATION IN ('0P3G', '003G')\n" +
            "                            AND a.tr_sn = b.tr_sn))\n" +
            "         AND start_time < SYSDATE - 90\n" +
            "         AND (end_time < SYSDATE - 90 OR end_time IS NULL)\n" +
            "ORDER BY \"位置\", CUST_KP_NO, \"進料日期\"";
            dr["DBKey"] = "HWDNEWDB";
            dr["MailTo"] = "HWD-SFC-01";
            dr["UseExcel"] = true;
            dr["NoRecordSendMail"] = false;
            s.SQLTaskConfig.Rows.Add(dr);

            Configs.DTSTaskConfigRow dr1 = s.DTSTaskConfig.NewDTSTaskConfigRow();
            dr1.Name = "DTS1";
            dr1.TimeSpan = "3600000";
            dr1.DBKey1 = "ORGDB";
            dr1.DBKey2 = "OBJDB";
            dr1.BeforeInsertDB1SQL = "update t1";
            dr1.BeforeInsertDB2SQL = "update t2";
            dr1.SelectSQL = "select t1";
            dr1.InsertTableName = "T2";
            dr1.AfterInsertDB1SQL = "update t1";
            dr1.AfterInsertDB2SQL = "update t2";
            dr1.DB1TYPE = "ORA";
            dr1.DB2TYPE = "SQLSERVER";

            s.DTSTaskConfig.Rows.Add(dr1);
            s.WriteXml(".\\EX_SQLTaskConfig.xml", XmlWriteMode.WriteSchema);
        }

        private void dgTaskView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //dgTaskView.s
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            startAll();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            stopAll();
        }

        private void dgTaskView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                CurrentItem = ( Process)dgTaskView.Rows[e.RowIndex].Cells[0].Value;
                toolStripLabel1.Text = "任務:" + CurrentItem.ToString();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ProcessManagedItem MI = this.SQLAlarmManagedItems[CurrentItem.ToString()];
            MI.Start();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ProcessManagedItem MI = this.SQLAlarmManagedItems[CurrentItem.ToString()];
            MI.Stop();
        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ProcessManagedItem MI = this.SQLAlarmManagedItems[CurrentItem.ToString()];
            for (int i = 0; i < dgTaskView.Rows.Count; i++)
            {
                if (dgTaskView.Rows[i].Cells[0].Value == CurrentItem)
                {
                    dgTaskView.Rows[i].Cells[2].Value = DateTime.Now.ToString();
                    dgTaskView.Rows[i].Cells[4].Value = "手動執行";
                    MI.Run();
                }
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ConfigsEx();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            frmConfigEdit c = new frmConfigEdit();
            c.ShowDialog();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            config.Clear();
            foreach (ProcessManagedItem mi in SQLAlarmManagedItems.Values)
            {
                mi.Stop();
            }
            SQLAlarmManagedItems.Clear();
            Init();
        }

        private void dgTaskView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {

        }
    }
}
