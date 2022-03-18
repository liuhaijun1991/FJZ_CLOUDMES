using MESCMCHost.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using static MESCMCHost.CMC503Scanda;
using System.Threading;

namespace MESCMCHost
{
    public partial class frmMain : Form
    {
        CMC503_UI CurrUI = null;
        string MESConnStr = "";
        string HOST_NAME = "";
        string MES_USER = "";
        string MES_PWD = "";
        //string Token = "";
        string ServerIP = "";
        string Point = "";
        MESAPIClient MESAPI;
        List<CMC503Scanda> ScandaList = new List<CMC503Scanda>();
        JToken HOSTINFO;


        public frmMain()
        {
            MESConnStr = ConfigurationManager.AppSettings["MES_API"];
            HOST_NAME = ConfigurationManager.AppSettings["CMCHOSTNAME"];
            MES_USER = ConfigurationManager.AppSettings["MES_USER"];
            MES_PWD = ConfigurationManager.AppSettings["MES_PWD"];
            ServerIP = ConfigurationManager.AppSettings["HostIp"]; ;
            Point = ConfigurationManager.AppSettings["Point"]; ;
            InitializeComponent();
            MESAPI = new MESAPIClient(MESConnStr, MES_USER, MES_PWD);
            try
            {
                MESAPI.Connect();
                MESAPI.Login();
                this.Text = HOST_NAME +":已能连接到MES API服务器!";
            }
            catch
            {  
                //MessageBox.Show("不能连接到MES API服务器!");
                this.Text = HOST_NAME + ":不能连接到MES API服务器!";
                //Application.Exit();
                return;
            }

            var hostinfo = MESAPI.GetHostInfo(HOST_NAME);
            this.Text = hostinfo["Data"]["HOST"]["HOST_NAME"].ToString();
            HOSTINFO = hostinfo["Data"];
            renewCmcUI();

        }

        void renewCmcUI()
        {
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                try
                {
                    ((CMC503_UI)c).scanda.FreeMe();
                }
                catch
                { }
            }
            this.flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < HOSTINFO["CMCS"].Count(); i++)
            {
                //CMC503_UI ui = new CMC503_UI();
                CMC503Scanda scanda = new CMC503Scanda(MESConnStr);
                scanda.IP = HOSTINFO["CMCS"][i]["IP"].ToString();
                scanda.Line = HOSTINFO["CMCS"][i]["LINE"].ToString();
                scanda.Station = HOSTINFO["CMCS"][i]["STATION_NAME"].ToString();

                scanda.MAIN_UI.Click += cmC503_UI1_Click;
                this.flowLayoutPanel1.Controls.Add(scanda.MAIN_UI);
                ScandaList.Add(scanda);

                if (HOSTINFO["CMCS"][i]["AUTO_CONNECT"].ToString().ToUpper().Trim() == "TRUE")
                {
                    scanda.isConnectToCMC = true;
                }
            }
        }


        private void MESWebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

       

        private void MESWebSocket_OnClose(object sender, CloseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void cmC503_UI1_Load(object sender, EventArgs e)
        {
            CMC503Scanda test = new CMC503Scanda(MESConnStr);
            test.IP = "10.120.156.50";
            test.Line = "B223FC10";
            test.Station = "TEST";
            cmC503_UI1.scanda = test;
            //cmC503_UI2.Click += cmC503_UI1_Click;
        }

        private void cmC503_UI1_Click(object sender, EventArgs e)
        {
            if (CurrUI != null)
            {
                CurrUI.BackColor = System.Drawing.SystemColors.Control;
            }

            CurrUI = (CMC503_UI)sender;
            CurrUI.BackColor = System.Drawing.SystemColors.ActiveCaption;
            //CurrUI.scanda.txtLog;
            txtLog.Text = CurrUI.scanda.txtLog;
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < ScandaList.Count; i++)
            {
                ScandaList[i].FreeMe();


            }
            ScandaList.Clear();
            MESAPI.DisConnect();

            foreach (Control c in flowLayoutPanel1.Controls)
            {
                try
                {
                    ((CMC503_UI)c).scanda.FreeMe();
                }
                catch
                { }
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                txtLog.Text = CurrUI.scanda.txtLog;
            }
            catch
            { }
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                MESAPI.Connect();
                MESAPI.Login();
                this.Text = HOST_NAME + ":已能连接到MES API服务器!";
            }
            catch
            {
                MessageBox.Show("不能连接到MES API服务器!");
                this.Text = HOST_NAME + ":不能连接到MES API服务器!";
                //Application.Exit();
                return;
            }

            var hostinfo = MESAPI.GetHostInfo(HOST_NAME);
            this.Text = hostinfo["Data"]["HOST"]["HOST_NAME"].ToString();
            HOSTINFO = hostinfo["Data"];
            renewCmcUI();
        }
    }
}
