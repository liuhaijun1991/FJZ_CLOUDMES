using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp.Server;
using WebServer.SocketService;
using System.Threading;
using MESPubLab.MESStation.LogicObject;

namespace WebServer
{
    public partial class FrmViewCallHis : Form
    {
        Report ShowObj;
        public FrmViewCallHis(Report _Data)
        {
            InitializeComponent();
            ShowObj = _Data;
            txtRecordCount.Text = ShowObj.RecordHisCount.ToString();
            
        }

        private void FrmViewCallHis_Load(object sender, EventArgs e)
        {
            listHis.Items.Clear();
            for (int i = 0; i < ShowObj.HIS.Count;i++)
            {
                listHis.Items.Add(ShowObj.HIS[i]);
            }
            ShowObj.CallFinishEvent += ShowObj_CallFinishEvent;

        }

        delegate void reNewFrmDelg();
        private void ShowObj_CallFinishEvent(Report sender, CallHis his)
        {

            this.Invoke(new reNewFrmDelg ( reNewForm));
        }

        void reNewForm()
        {
            listHis.Items.Clear();
            for (int i = 0; i < ShowObj.HIS.Count; i++)
            {
                listHis.Items.Add(ShowObj.HIS[i]);
            }
            if (chkAutoRun.Checked == true)
            {
                reNewForm(ShowObj.HIS[ShowObj.HIS.Count - 1]);
            }
        }

        void reNewForm(CallHis his)
        {
            
            LabState.Text = $@"Class:{his.APIClass}.{his.APIName} {"\r\n"}UseTime:{(his.EndTime - his.StartTime).TotalSeconds} \r\nStartTime:{his.StartTime}\r\nEndTime:{his.EndTime}";
            txtRequest.Text = his.Request.ToString();
            txtResponse.Text = his.Response;
            
        }

        private void listHis_Click(object sender, EventArgs e)
        {
            try
            {
                reNewForm((CallHis)listHis.SelectedItem);
            }
            catch
            {

            }
        }

        private void FrmViewCallHis_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShowObj.CallFinishEvent -= ShowObj_CallFinishEvent;
        }

        private void txtRecordCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                try
                {
                    int count = int.Parse(txtRecordCount.Text);
                    ShowObj.RecordHisCount = count;
                }
                catch
                {
                    txtRecordCount.Text = ShowObj.RecordHisCount.ToString();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
