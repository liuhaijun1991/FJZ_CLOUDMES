using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp.Server;
using WebServer.SocketService;
using System.Threading;
using MESPubLab.MESStation.LogicObject;
using System.Windows.Forms.DataVisualization.Charting;
using MESPubLab;
using MESDBHelper;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace WebServer
{
    public partial class FormMain : Form
    {
        static WebSocketServer SFCSocket;
        static string format = "ClientCount:{0} || Client:{1}:{2} || ID:{3} >> Message:{3}";
        ServerSession SelectSession = null;
        delegate void OncallDelegate(Report report, CallHis his);

        bool debug = false;
        public static int index = 0;

        void showCallInfo(Report report, CallHis his)
        {
            lock (treeView2)
            {
                if (index > 2000)
                {
                    index = 0;
                }
                index++;
                TreeNode T = new TreeNode(his.ClientIP + ":" + his.ClientPort + ">>"+ index.ToString()+">>" + his.APIName);
                T.Tag = his;
                treeView2.Nodes.Insert(0, T);
                if (treeView2.Nodes.Count >= 21)
                {
                    treeView2.Nodes.RemoveAt(20);
                }
                treeView2.Refresh();

                if (txtShowResponse.Text == "")
                {
                    try
                    {
                        txtShowResponse.Text = his.Response;
                        labCallInfo.Text = "StartTime:" + his.StartTime.ToString("HH:mm:ss");
                    }
                    catch
                    { }
                }
            }
        }

        public void OnCall(Report report, CallHis his)
        {
            OncallDelegate a = new OncallDelegate(showCallInfo);
            lock (this)
            {
                this.Invoke(a, new object[] { report, his });
            }
        }

        private void initChartCall()
        {
           
        }


        public FormMain() 
        {
            if (debug)
                MessageBox.Show("F1");
            InitializeComponent();

            labPath.Text = AppDomain.CurrentDomain.BaseDirectory;

            txtSFCDB.Text = REGHelp.getRegValue("SFCDB");
            txtAPDB.Text = REGHelp.getRegValue("APDB");
            txtWeb_Download_Path.Text = REGHelp.getRegValue("WebFilePath");

            if (!string.IsNullOrWhiteSpace(txtSFCDB.Text))
            {
                ConnectionManager.Add("SFCDB", txtSFCDB.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtAPDB.Text))
            {
                ConnectionManager.Add("APDB", txtAPDB.Text);
            }
            /*
            if (cbxUseSSL.Checked == true)
            {
                REGHelp.setRegKey("UseSSL", "TRUE");
            }
            else
            {
                REGHelp.setRegKey("UseSSL", "FALSE");
            }
            REGHelp.setRegKey("CertificateFile", txtCertificateFile.Text);
            REGHelp.setRegKey("Certificatekey", txtCertificatekey.Text);
            REGHelp.setRegKey("SslProtocols", txtSslProtocols.Text);
            */
            txtCertificateFile.Text = REGHelp.getRegValue("CertificateFile");
            txtCertificatekey.Text = REGHelp.getRegValue("Certificatekey");
            txtSslProtocols.Text = REGHelp.getRegValue("SslProtocols");
            var strUseSSL = REGHelp.getRegValue("UseSSL");
            if (strUseSSL != null && strUseSSL == "TRUE")
            {
                cbxUseSSL.Checked = true;
            }
            else
            {
                cbxUseSSL.Checked = false;
            }
            

            DateTime StartTime = DateTime.Now;
            Thread T = new Thread(new ThreadStart(ServiceStart));
            if (debug)
                MessageBox.Show("F1.2");
            //T.Start();
            var serverport = int.Parse(ConfigurationManager.AppSettings["ServerPort"].ToString());
            Console.WriteLine("Start");

            if (strUseSSL.Equals("TRUE"))
            {
                ////SFCSocket = new WebSocketServer("wss://mis-g6001953.nn.cloudnsbg.efoxconn.com:2130");
                //SFCSocket = new WebSocketServer(serverport, true);
                //SFCSocket.SslConfiguration.EnabledSslProtocols = (SslProtocols)Enum.Parse(typeof(SslProtocols), ConfigurationManager.AppSettings["SslProtocols"].ToString());
                ////SFCSocket.SslConfiguration.ServerCertificate = new X509Certificate2("testca.pfx", "asd");

                ////SFCSocket = new WebSocketServer("wss://nntestmes.cnsbg.efoxconn.com:2130");
                //SFCSocket.SslConfiguration.ServerCertificate = new X509Certificate2(ConfigurationManager.AppSettings["CertificateFile"].ToString(),
                //    ConfigurationManager.AppSettings["Certificatekey"].ToString());
                ////SFCSocket = new WebSocketServer("wss://localhost:2130");
                ////SFCSocket.SslConfiguration.ServerCertificate = new X509Certificate2("D:/ca/test.pfx", "qwe");
                ////SFCSocket.SslConfiguration.ClientCertificateValidationCallback =
                ////  (sender, certificate, chain, sslPolicyErrors) =>
                ////  {
                ////      return true; // If the server certificate is valid.
                ////  };
                ////SFCSocket = new WebSocketServer(System.Net.IPAddress.Any, serverport);
                SFCSocket = new WebSocketServer(serverport, true);
                SFCSocket.SslConfiguration.EnabledSslProtocols = (SslProtocols)Enum.Parse(typeof(SslProtocols), txtSslProtocols.Text);
                SFCSocket.SslConfiguration.ServerCertificate = new X509Certificate2(txtCertificateFile.Text,
                    txtCertificatekey.Text);
            }
            else
                SFCSocket = new WebSocketServer(serverport);
            SFCSocket.Log.Level = WebSocketSharp.LogLevel.Error;
            SFCSocket.WaitTime = TimeSpan.FromSeconds(2);
            SFCSocket.AddWebSocketService<Report>("/ReportService", new Func<Report>(NewReportService));
            Report.ServerForm = this;
            SFCSocket.Start();
            if (debug)
                MessageBox.Show("F1.3");


        }
        static void ServiceStart()
        { 
            //Console.WriteLine("Start");
            SFCSocket = new WebSocketServer(System.Net.IPAddress.Any, 2130);
            SFCSocket.Log.Level = WebSocketSharp.LogLevel.Error;
            SFCSocket.WaitTime = TimeSpan.FromSeconds(2);
            SFCSocket.AddWebSocketService<Report>("/ReportService", new Func<Report>(NewReportService));

            SFCSocket.Start();
        }
        static void killLogusers(string Token)
        {
            if (Report.LoginUsers.ContainsKey(Token))
            {
                Report.LoginUsers.Remove(Token);
                MESStation.Stations.CallStation.logout(Token);

                Console.WriteLine($@"Token:{Token} have killed");
            }
            else
            {
                Console.WriteLine($@"Token:{Token} not find");
            }
            GC.Collect(2);
        }

        private static Report NewReportService()
        {
            Report _ReportService = new Report();
            _ReportService.OnSocketOpen += _ReportService_OnSocketOpen;
            _ReportService.OnSocketClose += _ReportService_OnSocketClose;
            _ReportService.OnSocketError += _ReportService_OnSocketError;
            _ReportService.OnSocketMessage += _ReportService_OnSocketMessage;
            return _ReportService;
        }

        private static void  _ReportService_OnSocketMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
           
        }

        private static void _ReportService_OnSocketOpen(object sender, EventArgs e)
        {
           
        }

        private static void _ReportService_OnSocketClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            try
            {
                Report s = (Report)sender;
                if (s.Token != null)
                {
                    killLogusers(s.Token);
                }
                for (int i = 0; i < s.UseDbs.Count; i++)
                {
                    try
                    {
                        s.UseDbs[i].RollbackTrain();
                    }
                    catch
                    { }
                }
                
                s.UseDbs.Clear();
                
            }
            catch (Exception ee)
            {

            }
            

        }
        private static void _ReportService_OnSocketError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
           
        }

        void UpdateControl(string msg, int count)
        {
           
        }

        private  void FormMain_Load(object sender, EventArgs e)
        {
            initChartCall();
            this.Text = "CloundMES start at:" + DateTime.Now.ToString();



        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var keys = Report.LoginUsers.Keys.ToList();
            treeView1.Nodes.Clear();
            int count = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                var session = Report.LoginUsers[keys[i]];
                var Text = session.user.EMP_NO + ":" + session.serverObj.ClientIP;
                txtSelect.Text = txtSelect.Text.Trim();

                if (txtSelect.Text != "" && Text.IndexOf(txtSelect.Text) == -1)
                {
                    continue;
                }

                TreeNode n = new TreeNode();
                n.Text = Text;
                count++;

                n.Tag = keys[i];
                treeView1.Nodes.Add(n);
                n.Name = "USER";
            }
            GroupSession.Text = $@"Session({count})";
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode n = e.Node;
            try
            {
                string key = (string)n.Tag;
                ServerSession session = Report.LoginUsers[key];
                SelectSession = session;
                FrmViewCallHis f = new FrmViewCallHis(session.serverObj);

                f.Show();
            }
            catch
            { }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode n = e.Node;

            try
            {
                string key = (string)n.Tag;
                ServerSession session = Report.LoginUsers[key];
                SelectSession = session;
            }
            catch
            {
                return;
            }

            if (n.Name == "USER")
            {
                n.Nodes.Clear();
                var keys = MESStation.Stations.CallStation.StationPool.Keys.ToList();
                string Token = n.Tag.ToString();
                var k = keys.FindAll(t => t.StartsWith(Token));
                for (int i = 0; i < k.Count; i++)
                {
                    TreeNode n1 = new TreeNode();
                    n1.Name = "STATION";
                    n1.Text = k[i].Substring(Token.Length);
                    n1.Tag = k[i];
                    n.Nodes.Add(n1);
                }
                n.Expand();

            }
        }

        private void btnKillSession_Click(object sender, EventArgs e)
        {
            try
            {
                SelectSession.serverObj.Close();
                button1_Click(button1, new EventArgs());
            }
            catch
            { }
        }

        private void btnKillIPSession_Click(object sender, EventArgs e)
        {
            var keys = Report.LoginUsers.Keys.ToList();
            treeView1.Nodes.Clear();
            int count = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                var session = Report.LoginUsers[keys[i]];
                var Text = session.user.EMP_NO + ":" + session.serverObj.ClientIP;
                txtSelect.Text = txtSelect.Text.Trim();

                if (txtSelect.Text != "" && session.serverObj.ClientIP.IndexOf(txtKillIPSession.Text) == 0)
                {
                    session.serverObj.Close();
                    continue;
                }

                TreeNode n = new TreeNode();
                n.Text = Text;
                count++;

                n.Tag = keys[i];
                treeView1.Nodes.Add(n);
                n.Name = "USER";
            }
            GroupSession.Text = $@"Session({count})";
        }

        private void btnKillUserSession_Click(object sender, EventArgs e)
        {
            var keys = Report.LoginUsers.Keys.ToList();
            treeView1.Nodes.Clear();
            int count = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                var session = Report.LoginUsers[keys[i]];
                var Text = session.user.EMP_NO + ":" + session.serverObj.ClientIP;
                txtSelect.Text = txtSelect.Text.Trim();

                if (txtSelect.Text != "" && session.user.EMP_NO.IndexOf(txtKillUserSession.Text) == 0)
                {
                    session.serverObj.Close();
                    continue;
                }

                TreeNode n = new TreeNode();
                n.Text = Text;
                count++;

                n.Tag = keys[i];
                treeView1.Nodes.Add(n);
                n.Name = "USER";
            }
            GroupSession.Text = $@"Session({count})";
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        CallHis his = null;

        private void treeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            his = (CallHis)e.Node.Tag;
            txtShowCall.Text = his.Request.ToString();
            txtShowResponse.Text = his.Response;
            labCallInfo.Text = "StartTime:" + his.StartTime.ToString("HH:mm:ss");
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
            Application.Exit();
        }

        float x = 0;
        Random r = new Random();
        private void TimerGetServerInfo_Tick(object sender, EventArgs e)
        {
            try
            {
                var MemorySize = GC.GetTotalMemory(false);
                labServerInfo.Text = "内存占用:" + (MemorySize / 1024 / 1024).ToString() + "MB" + "\r\n";
                string poolINF = "";
                if (Report.APDBPool != null)
                {
                    poolINF += $@"APDB:{Report.APDBPool.PoolBorrowed} / {Report.APDBPool.PoolRemain + Report.APDBPool.PoolBorrowed} " + "\r\n";
                }
                if (Report.SFCDBPool != null)
                {
                    poolINF += $@"SFCDB:{Report.SFCDBPool.PoolBorrowed} / {Report.SFCDBPool.PoolRemain + Report.SFCDBPool.PoolBorrowed} " + "\r\n";
                }
                poolINF += $@"Login Token:{Report.LoginUsers.Keys.Count}" + "\r\n";
                labServerInfo.Text += poolINF;
                ListFunctionNoRet.DataSource = null;
                ListFunctionNoRet.Refresh();
                ListFunctionNoRetAP.DataSource = null;
                ListFunctionNoRetAP.Refresh();
                //Report.SFCDBPool.BorrowTimeOutFunction.Add("dfdfdf"+r.Next(0,10).ToString());
                if (Report.SFCDBPool != null)
                {
                    ListFunctionNoRet.DataSource = Report.SFCDBPool.BorrowTimeOutFunction;
                }
                ListFunctionNoRet.Refresh();
                if (Report.APDBPool != null)
                {
                    ListFunctionNoRetAP.DataSource = Report.APDBPool.BorrowTimeOutFunction;
                }
                ListFunctionNoRetAP.Refresh();


                listSFCLend.DataSource = null;
                listSFCLend.Refresh();
                if (Report.SFCDBPool != null)
                {
                    listSFCLend.DataSource = Report.SFCDBPool.GetBorrowFunction();
                }
                listSFCLend.Refresh();

                listAPLend.DataSource = null;
                listAPLend.Refresh();
                if (Report.SFCDBPool != null)
                {
                    listAPLend.DataSource = Report.APDBPool.GetBorrowFunction();
                }
                listAPLend.Refresh();

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnGCStart_Click(object sender, EventArgs e)
        {
            GC.Collect(2);
        }

        private void btnCleanDBPool_Click(object sender, EventArgs e)
        {
            try
            {
                Report.APDBPool.CleanAll();
            }
            catch
            { }
            try
            {
                Report.SFCDBPool.CleanAll();
            }
            catch
            { }
        }

        private void btnRegSave_Click(object sender, EventArgs e)
        {
            REGHelp.setRegKey("SFCDB", txtSFCDB.Text);
            REGHelp.setRegKey("APDB", txtAPDB.Text);
            REGHelp.setRegKey("WebFilePath",txtWeb_Download_Path.Text);
            if (cbxUseSSL.Checked == true)
            {
                REGHelp.setRegKey("UseSSL", "TRUE");
            }
            else
            {
                REGHelp.setRegKey("UseSSL", "FALSE");
            }
            REGHelp.setRegKey("CertificateFile", txtCertificateFile.Text);
            REGHelp.setRegKey("Certificatekey", txtCertificatekey.Text);
            REGHelp.setRegKey("SslProtocols", txtSslProtocols.Text);

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
    public class SessionIten
    {
        public string key;
        public object Value;
    }
}
