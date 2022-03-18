using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.DCN
{
    public partial class FNNSendUFIASNDataUI  : UserControl
    {
        FNNSendUFIASNDataTaskSyn sendUFI = null;

        public string strAction = "";
        public FNNSendUFIASNDataUI(FNNSendUFIASNDataTaskSyn sendObj)
        {
            InitializeComponent();
            sendUFI = sendObj;
        }
        int seconds = 0;
        private void btnSend_Click(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            //btnSaveCSV.Enabled = false;
            this.labelMsg.Text = "Sending data......";
            this.labelMsg.ForeColor = Color.Blue;
            timerSend.Enabled = true;
            timerSend.Tick += TimerSend_Tick;
            seconds = 0;
            strAction = "Sending";
            DateTime startTime = DateTime.Now;
            Task task = new Task(() => {
            string msg = "OK";
                try
                {
                    if (rbOneDay.Checked)
                    {
                        DateTime collectDate = Convert.ToDateTime(dtpFrom.Value.ToString("yyyy/MM/dd"));
                        //System.Threading.Thread.Sleep(10000);
                        sendUFI.SendData(collectDate);                        
                    }
                    if (rbMultipleDay.Checked)
                    {
                        int days = (dtpTo.Value - dtpFrom.Value).Days;
                        if (days > 0)
                        {
                            for (int i = 0; i <= days; i++)
                            {
                                DateTime collectDate = Convert.ToDateTime(dtpFrom.Value.AddDays(i).ToString("yyyy/MM/dd"));
                                //System.Threading.Thread.Sleep(10000);
                                sendUFI.SendData(collectDate);                              
                            }
                        }
                        else if (days == 0)
                        {
                            msg = "Form date is equals to date.";
                        }
                        else
                        {
                            msg = "Form date is less than to date.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }                
                MethodInvoker m = () =>
                {
                    btnSend.Enabled = true;
                    //btnSaveCSV.Enabled = true;
                    this.labelMsg.Text = $"Start Time:{startTime.ToString("yyyy/MM/dd") + " 00:00:00"};End Time:{dtpTo.Value.ToString("yyyy/MM/dd") + " 23:59:59"};Msg:{msg}";
                    this.labelMsg.ForeColor = msg == "OK" ? Color.Green : Color.Red;
                    timerSend.Enabled = false;
                    timerSend.Tick -= TimerSend_Tick;
                };
                this.BeginInvoke(m);
            });
            task.Start();
        }

        private void TimerSend_Tick(object sender, EventArgs e)
        {
            seconds++;
            this.labelMsg.Text = $"{strAction}......{seconds.ToString()}s";
        }

        //private void btnSaveCSV_Click(object sender, EventArgs e)
        //{
        //    btnSend.Enabled = false;
        //    btnSaveCSV.Enabled = false;
        //    this.labelMsg.Text = "Saveing data......";
        //    this.labelMsg.ForeColor = Color.Blue;
        //    timerSend.Enabled = true;
        //    timerSend.Tick += TimerSend_Tick;
        //    seconds = 0;
        //    strAction = "Saveing";
        //    DateTime startTime = DateTime.Now;
        //    Task task = new Task(() => {
        //        string msg = "OK";
        //        try
        //        {
        //            if (rbOneDay.Checked)
        //            {
        //                DateTime collectDate = Convert.ToDateTime(dtpFrom.Value.ToString("yyyy/MM/dd") + " 08:00:00");
        //                //System.Threading.Thread.Sleep(10000);
        //                sendUFI.SaveCSV(collectDate);
        //            }
        //            if (rbMultipleDay.Checked)
        //            {
        //                int days = (dtpTo.Value - dtpFrom.Value).Days;
        //                if (days > 0)
        //                {
        //                    for (int i = 0; i <= days; i++)
        //                    {
        //                        DateTime collectDate = Convert.ToDateTime(dtpFrom.Value.AddDays(i).ToString("yyyy/MM/dd") + " 08:00:00");
        //                        //System.Threading.Thread.Sleep(10000);
        //                        sendUFI.SaveCSV(collectDate);
        //                    }
        //                }
        //                else if (days == 0)
        //                {
        //                    msg = "Form date is equals to date.";
        //                }
        //                else
        //                {
        //                    msg = "Form date is less than to date.";
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            msg = ex.Message;
        //        }
        //        MethodInvoker m = () =>
        //        {
        //            btnSend.Enabled = true;
        //            btnSaveCSV.Enabled = true;
        //            timerSend.Enabled = false;
        //            timerSend.Tick -= TimerSend_Tick;
        //            this.labelMsg.Text = $"CSV file save in {sendUFI.csvFilePath};Start Time:{startTime.ToString("yyyy/MM/dd HH:mm:ss")};End Time:{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")};Msg:{msg}";
        //            this.labelMsg.ForeColor = msg == "OK" ? Color.Green : Color.Red;

        //        };
        //        this.BeginInvoke(m);
        //    });
        //    task.Start();
        //}
    }
}
