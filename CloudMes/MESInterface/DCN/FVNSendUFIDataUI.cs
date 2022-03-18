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
    public partial class FVNSendUFIDataUI  : UserControl
    {
        FVNSendUFIDataTaskSyn sendUFI = null;

        public string strAction = "";
        public FVNSendUFIDataUI(FVNSendUFIDataTaskSyn sendObj)
        {
            InitializeComponent();
            sendUFI = sendObj;
        }
        int seconds = 0;
        private void btnSend_Click(object sender, EventArgs e)
        {
            btnSend.Enabled = false;            
            this.labelMsg.Text = "Sending data......";
            this.labelMsg.ForeColor = Color.Blue;
            timerSend.Enabled = true;
            timerSend.Tick += TimerSend_Tick;
            seconds = 0;
            strAction = "Sending";            
            DateTime StartDate = Convert.ToDateTime(dtpFrom.Value.ToString("yyyy/MM/dd") + " 00:00:00");
            DateTime EndDate = Convert.ToDateTime(dtpTo.Value.ToString("yyyy/MM/dd") + " 23:59:59");
            Task task = new Task(() => {
                string msg = "OK";
                try
                {
                    if (EndDate > StartDate)
                    {                        
                        sendUFI.SendData(StartDate, EndDate);                        
                    }
                    else
                    {
                        msg = "From date is less than To date.";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }                
                MethodInvoker m = () =>
                {
                    btnSend.Enabled = true;                    
                    this.labelMsg.Text = $"Start Time:{StartDate};End Time:{EndDate};Msg:{msg}";
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
    }
}
