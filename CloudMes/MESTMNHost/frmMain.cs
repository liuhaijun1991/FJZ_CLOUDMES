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
using System.Net.Sockets;
using System.Net;
using System.Reflection;

namespace MESCMCHost
{
    public partial class frmMain : Form
    {
        CMC503_UI CurrUI = null;
        string MESConnStr = "";
        string HOST_NAME = "";
        string MES_USER = "";
        string MES_PWD = "";
        string Token = "";
        MESAPIClient MESAPI;
        List<CMC503Scanda> ScandaList = new List<CMC503Scanda>();
        JToken HOSTINFO;
        List<SocketSession> Sockets = new List<SocketSession>();
        Thread ConnectCMC;


        public frmMain()
        {
            FieldInfo fi = typeof(ConfigurationManager).GetField("s_initState", BindingFlags.Static);
            if (fi != null)
            {
                fi.SetValue(null, 0);
            }
            MESConnStr = ConfigurationManager.AppSettings["MES_API"];
            HOST_NAME = ConfigurationManager.AppSettings["CMCHOSTNAME"];
            //HOST_NAME = "TESTHOST1";
            MES_USER = ConfigurationManager.AppSettings["MES_USER"];
            MES_PWD = ConfigurationManager.AppSettings["MES_PWD"];
            InitializeComponent();
            MESAPI = new MESAPIClient(MESConnStr, MES_USER, MES_PWD);
            try
            {
                MESAPI.Connect();
                MESAPI.Login();
                this.Text = HOST_NAME + ":已能连接到MES API服务器!";
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
            ConnectCMC = new Thread(new ThreadStart(ClientSocket));
            ConnectCMC.Start();
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
                CMC503Scanda scanda = new CMC503Scanda();
                scanda.IP = HOSTINFO["CMCS"][i]["IP"].ToString();
                scanda.Line = HOSTINFO["CMCS"][i]["LINE"].ToString();
                scanda.Station = HOSTINFO["CMCS"][i]["STATION_NAME"].ToString();
                scanda.MESAPI = new MESAPIClient(MESConnStr);
                scanda.MESAPI.MES_USER = HOSTINFO["CMCS"][i]["ID"].ToString();
                scanda.MESAPI.MES_PWD = "11111";
                scanda.MESAPI.LoginTest();
                scanda.InitStation();
                //Sockets.Add(new SocketSession(null, scanda.MESAPI.Token, scanda.IP));
                scanda.MAIN_UI.Click += cmC503_UI1_Click;
                this.flowLayoutPanel1.Controls.Add(scanda.MAIN_UI);
                ScandaList.Add(scanda);

                //if (HOSTINFO["CMCS"][i]["AUTO_CONNECT"].ToString().ToUpper().Trim() == "TRUE")
                //{ 
                //    scanda.isConnectToCMC = true;
                //}
            }
        }

        public void ClientSocket()
        {
            int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Point"]);
            //int Port = Convert.ToInt32("1501");
            string IpTemp = "";
            IPEndPoint IPT = new IPEndPoint(IPAddress.Any, Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(IPT);
            try
            {
                socket.Listen(0);
                while (true)
                {
                    Socket sok = socket.Accept();
                    IpTemp = GetStringIp(sok);
                    if (ScandaList.Find(s => s.IP == IpTemp) != null && ScandaList.Find(s => s.IP == IpTemp).ClientSocket == null)
                    {
                        foreach (CMC503Scanda item in ScandaList)
                        {
                            if (item.IP == IpTemp)
                            {
                                item.ClientSocket = sok;
                            }
                        }
                    }
                }
            }
            catch
            { }
        }



        public string GetStringIp(Socket _socket)
        {
            string Ip = ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
            return Ip;
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
