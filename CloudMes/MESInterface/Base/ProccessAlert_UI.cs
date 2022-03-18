using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MESDBHelper;
using MESDataObject.Module;
using System.Threading;

namespace MESInterface.Base
{
    public partial class ProccessAlert_UI : UserControl
    {
        ProccessAlert proccess;
        DataTable Task = new DataTable();
        DataTable Detail = new DataTable();
        string db;
        OleExec DB;
        string MONITOR_NAME;

        Thread AutoThread;
        List<ProccessCkeckBase> SelectChecks;

        public ProccessAlert_UI(ProccessAlert _proccess)
        {
            proccess = _proccess;
            InitializeComponent();
            Task.Columns.Add("Name");
            Task.Columns.Add("Status");
            Task.Columns.Add("Data", typeof(object));
            Task.Columns.Add("Detail",typeof(object));
            Detail.Columns.Add("Name");
            Detail.Columns.Add("LastTime");
            Detail.Columns.Add("NextTime");
            Detail.Columns.Add("Status");
            Detail.Columns.Add("Message");
            dgvTask.DataSource = Task;
            dgvTask.Columns[2].Visible = false;
            dgvTask.Columns[3].Visible = false;
            dgvTaskDetail.DataSource = Detail;
            db = proccess.ConfigGet("DB");
            Init();

        }

        void Init()
        {
            lock (Task)
            {
                if (AutoThread != null)
                {
                    try
                    {
                        AutoThread.Abort();
                    }
                    catch
                    { }
                    AutoThread = null;
                }

                Task.Rows.Clear();
                Detail.Rows.Clear();
                DB = new OleExec(db, true);
                MONITOR_NAME = proccess.ConfigGet("MONITOR_NAME");
                var Proccess = DB.ORM.Queryable<C_PROCCESS_ALERT>().Where(t => t.MONITOR_NAME == MONITOR_NAME).ToList();

                for (int i = 0; i < Proccess.Count; i++)
                {
                    var dr = Task.NewRow();
                    dr["Name"] = Proccess[i].PROCCESS_NAME;
                    dr["Status"] = "正常";
                    dr["Data"] = Proccess[i];
                    var PL = DB.ORM.Queryable<C_PROCCESS_CHECK>().Where(t => t.PROCCESS_NAME == Proccess[i].PROCCESS_NAME).ToList();
                    List<ProccessCkeckBase> l = new List<ProccessCkeckBase>();
                    dr["Detail"] = l;
                    for (int j = 0; j < PL.Count; j++)
                    {
                        var p = ProccessCkeckBase.GetProccessCkeck(PL[j],DB);
                        if (PL[j].RUN_TIME_DATA == null || PL[j].RUN_TIME_DATA.Trim() == "")
                        {
                            p.runtime = new RunTimeData();
                            p.runtime.ProccessName = Proccess[i].PROCCESS_NAME;
                        }
                        l.Add(p);
                    }

                    Task.Rows.Add(dr);
                }
                AutoThread = new Thread(new ThreadStart(AutoThreadRun));
            }
            AutoThread.Start();
        }

        void AutoThreadRun()
        {
            while (true)
            {
                Thread.Sleep(1000);
                lock (Task)
                {
                    for (int i = 0; i < Task.Rows.Count; i++)
                    {
                        Task.Rows[i]["Status"] = "正常";
                        object t1 = Task.Rows[i]["Detail"];
                        List<ProccessCkeckBase> pcl = (List<ProccessCkeckBase>)(t1);
                        for (int j = 0; j < pcl.Count; j++)
                        {
                            if (pcl[j].runtime.NextStartTime > DateTime.Now)
                            {
                                continue;
                            }
                            try
                            {
                                pcl[j].Run(pcl[j].runtime.ProccessName, pcl[j].Config, DB);
                                if (int.Parse(pcl[i].runtime.Alert_LV) > 0)
                                {
                                    Task.Rows[j]["Status"] = "异常";
                                }
                                //Task.Rows[j]["Message"] = pcl[i].runtime.Message;
                            }
                            catch
                            {

                            }
                        }

                    }
                    //发送预警信息
                    for (int i = 0; i < Task.Rows.Count; i++)
                    {
                        object t1 = Task.Rows[i]["Detail"];
                        List<ProccessCkeckBase> pcl = (List<ProccessCkeckBase>)(t1);
                        C_PROCCESS_ALERT CPA = (C_PROCCESS_ALERT)Task.Rows[i]["Data"];
                        for (int j = 0; j < pcl.Count; j++)
                        {
                            if (pcl[j].runtime.SMS_FLAG == "0")
                            {
                                var SMS = new MESPubLab.MESInterface.SMS.FoxLHSMSWebService();
                                SMS.Init(null);
                                string PN = "";
                                if (pcl[j].runtime.Alert_LV == "1")
                                {
                                    PN = CPA.LV1_SMS;
                                }
                                else if (pcl[j].runtime.Alert_LV == "2")
                                {
                                    PN = CPA.LV2_SMS;
                                }
                                else if (pcl[j].runtime.Alert_LV == "3")
                                {
                                    PN = CPA.LV3_SMS;
                                }
                                var pns = PN.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int k = 0; k < pns.Length; k++)
                                {
                                    SMS.Send(pns[k], pcl[j].runtime.Message);
                                }
                                pcl[j].runtime.SMS_FLAG = "1";
                            }
                            
                        }

                    }
                }
            }
        }

        private void ProccessAlert_UI_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        void RenewUI()
        {
            lock (SelectChecks)
            {
                if (SelectChecks != null)
                {
                    Detail.Rows.Clear();
                    for (int i = 0; i < SelectChecks.Count; i++)
                    {
                        var dr = Detail.NewRow();
                        dr["Name"] = SelectChecks[i].Name;
                        dr["LastTime"] = SelectChecks[i].runtime.LastStartTime;
                        dr["NextTime"] = SelectChecks[i].runtime.NextStartTime;
                        if (int.Parse(SelectChecks[i].runtime.Alert_LV) > 0)
                        {
                            dr["Status"] = "异常";
                        }
                        else
                        {
                            dr["Status"] = "正常";
                        }
                        dr["Message"] = SelectChecks[i].runtime.Message;
                        Detail.Rows.Add(dr);
                    }
                }
            }
        }

        private void dgvTask_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTask.SelectedRows.Count > 0)
            {
                var dr = ((System.Data.DataRowView)(dgvTask.SelectedRows[0].DataBoundItem)).Row;
                if (SelectChecks == null)
                {
                    SelectChecks = (List<ProccessCkeckBase>)dr["Detail"];
                }
                else
                {
                    lock (SelectChecks)
                    {

                        SelectChecks = (List<ProccessCkeckBase>)dr["Detail"];
                    }
                }
                RenewUI();
                dgvTaskDetail.Refresh();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RenewUI();
        }

        private void dgvTaskDetail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public class RunTimeData
    {
        public DateTime? LastStartTime;
        public DateTime? NextStartTime;
        public string Alert_LV = "0";
        public string Message;
        public string RuntimeState;
        public string SMS_FLAG;
        public string MAIL_FLAG;
        public List<string> Data=new List<string>();
        public DateTime? AlertTime;
        public string ProccessName;
    }
    public class CheckConfigBase
    {
        /// <summary>
        /// 排程運行間隔
        /// </summary>
        public int TimeSpan = 3600;
        public int AlertLV = 1;
        public int AutoResetHouts = 24;
    }
}
