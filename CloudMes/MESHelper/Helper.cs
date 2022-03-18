using HWDNNSFCBase;
using MESHelper.Common;
using MESHelper.Plugin;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MESHelper
{
    public partial class Helper : Form
    {
        delegate void bgdg();
        static WebSocketServer SFCSocket;

        static MESAPIClient MESAPI;
        Thread ViewR;
        Logs log = new Logs();
        bool reflush = true;
        string LabelTempPath;
        string LogPath;
        string PrintFilePath;
        string TempPath;
        //MESSocketClient client;
        string _ConfigFile = Environment.CurrentDirectory + "\\Helper.ini";
        string _Section = "ServerConnection";

        bool userConfigFile = true;
        string serverIP, username, pwd, BU;


        public Helper(string _serverIP,string _username,string _pwd,string _BU)
        {
            userConfigFile = false;
            serverIP = _serverIP;
            username = _username;
            pwd = _pwd;
            BU = _BU;
            InitializeComponent();
            CheckProcess();//檢查本程序是否已經有副本打開，如果有就幹掉它！

            InitPath();//初始化临时文件夹路径

            Init(); //初始化窗体数据
            int t = 0;
            while (SocketService.MESAPI.Token == null && t < 5)
            {
                Thread.Sleep(1000);
                t++;
            }
            if (SocketService.MESAPI.Token == null)
            {
                MessageBox.Show("SocketService.MESAPI.Token == null");
                return;
            }

            bnt_run_Click(new object(), new EventArgs());//触发侦听程序开始运行

            ViewR = new Thread(ViewRefush);
            ViewR.Start();
            log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 開始偵聽");
            try
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                this.notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
            }

        }
        public Helper()
        {
            InitializeComponent();
            CheckProcess();//檢查本程序是否已經有副本打開，如果有就幹掉它！

            InitPath();//初始化临时文件夹路径

            Init(); //初始化窗体数据
            int t = 0;
            while (SocketService.MESAPI.Token == null && t < 5)
            {
                Thread.Sleep(1000);
                t++;
            }
            if (SocketService.MESAPI.Token == null)
            {
                MessageBox.Show("SocketService.MESAPI.Token == null");
                return;
            }

            bnt_run_Click(new object(), new EventArgs());//触发侦听程序开始运行

            ViewR = new Thread(ViewRefush);
            ViewR.Start();
            log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 開始偵聽");
            try
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                this.notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
            }
        }
        private void Init()
        {
            string Addr = string.Empty, user = string.Empty, bu = string.Empty;
            if (!userConfigFile)
            {
                txt_MESServer.Text = serverIP;
                txt_UserName.Text = username;
                txt_Password.Text = pwd;
                cmb_bu.Items.Add(BU);
                cmb_bu.Text = BU;
            }
            else if (File.Exists(_ConfigFile))
            {
                try
                {
                    txt_MESServer.Text = ConfigFile.ReadIniData(_Section, "ServerIP", "", _ConfigFile) + ":2130";
                    txt_UserName.Text = ConfigFile.ReadIniData(_Section, "Username", "", _ConfigFile);
                    txt_Password.Text = ConfigFile.ReadIniData(_Section, "Password", "", _ConfigFile);
                    cmb_bu.Items.Add(ConfigFile.ReadIniData(_Section, "Bu", "", _ConfigFile));
                    cmb_bu.Text = ConfigFile.ReadIniData(_Section, "Bu", "", _ConfigFile);
                }
                catch
                { }
            }
            

            try
            {
                Txt_WeightRegex.Text = getRegValue("WeightRegex");
            }
            catch
            { }

            try
            {
                cbx_comportlist.DataSource = GetComlist(true);
                try
                {
                    cbx_comportlist.SelectedIndex = 0;
                }
                catch
                { }
                if (getRegValue("ComPortName") != "")
                {
                    try
                    {
                        cbx_comportlist.SelectedItem = getRegValue("ComPortName");
                    }
                    catch
                    { }
                }
                else
                {
                    try
                    {
                        cbx_comportlist.SelectedItem = "COM1";
                    }
                    catch
                    { }
                }
                if (getRegValue("BaudRate") != "")
                {
                    txt_BaudRate.Text = getRegValue("BaudRate");
                }
                if (getRegValue("WeighterType") != "")
                {
                    cbx_WeighterType.SelectedItem = getRegValue("WeighterType");
                }
                if (getRegValue("ZebraPort") != "")
                {
                    txt_ZebraPort.Text = getRegValue("ZebraPort");
                }
                if (getRegValue("isLocalPath") != "")
                {
                    cb_isLocalPath.Checked = Convert.ToBoolean(getRegValue("isLocalPath"));
                }
                SocketService.PrinterList = new string[4];
                PrinterSettings.StringCollection collection = PrinterSettings.InstalledPrinters;
                List<string> PrinterList = new List<string>();
                foreach (string item in collection)
                {
                    cbx_Printer1.Items.Add(item);
                    cbx_Printer2.Items.Add(item);
                    cbx_Printer3.Items.Add(item);
                    cbx_Printer4.Items.Add(item);
                }
                PrintDocument doc = new PrintDocument();
                string defaultPrinter = doc.PrinterSettings.PrinterName;
                if (getRegValue("Printer1") != "")
                {
                    cbx_Printer1.SelectedItem = getRegValue("Printer1");
                }
                else
                {
                    cbx_Printer1.SelectedItem = defaultPrinter;
                }
                if (getRegValue("Printer2") != "")
                {
                    cbx_Printer2.SelectedItem = getRegValue("Printer2");
                }
                else
                {
                    cbx_Printer2.SelectedItem = defaultPrinter;
                }
                if (getRegValue("Printer3") != "")
                {
                    cbx_Printer3.SelectedItem = getRegValue("Printer3");
                }
                else
                {
                    cbx_Printer3.SelectedItem = defaultPrinter;
                }
                if (getRegValue("Printer4") != "")
                {
                    cbx_Printer4.SelectedItem = getRegValue("Printer4");
                }
                else
                {
                    cbx_Printer4.SelectedItem = defaultPrinter;
                }
                //txtDownFile
                if (getRegValue("DownFile") != "")
                {
                   txtDownFile.Text = getRegValue("DownFile");
                }
            }
            catch (Exception ex)
            {
                log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ex.Message);
            }
            InitMesClient();
        }
        private void InitPath()
        {
            if (userConfigFile)
            {
                LabelTempPath = Environment.CurrentDirectory + "\\LabelTemp";
                LogPath = Environment.CurrentDirectory + "\\Log";
                PrintFilePath = Environment.CurrentDirectory + "\\PrintFile";
                TempPath = Environment.CurrentDirectory + "\\Temp";
                TxtTmpdir.Text = LabelTempPath;
            }
            else
            {
                var tmp =System.IO.Path.GetTempPath();
                LabelTempPath = tmp + "LabelTemp";
                LogPath = tmp + "Log";
                PrintFilePath = tmp + "PrintFile";
                TempPath = tmp + "Temp";
                TxtTmpdir.Text = LabelTempPath;
            }

            SocketService.LabelTempPath = LabelTempPath;

            try { Directory.Delete(LabelTempPath, true); } catch { }
            Thread.Sleep(1000);
            try { Directory.CreateDirectory(LabelTempPath); } catch { }
            try
            {
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                if (!Directory.Exists(PrintFilePath))
                {
                    Directory.CreateDirectory(PrintFilePath);
                }
                if (!Directory.Exists(TempPath))
                {
                    Directory.CreateDirectory(TempPath);
                }
            }
            catch { }
        }
        private void InitMesClient()
        {
            string Addr = txt_MESServer.Text.Trim();
            string user = txt_UserName.Text.Trim();
            string pwd = txt_Password.Text.Trim();
            string bu = cmb_bu.SelectedText;
            //SocketService.client = new MESSocketClient(Addr, user, pwd, bu);
            SocketService.MESAPI = new MESAPIClient(Addr, user, pwd);
        }
        private void setStatus(bool isStart)
        {
            if (isStart)
            {
                ts_Start.Enabled = false;
                ts_Stop.Enabled = true;
                txtServicePort.Enabled = false;
                cb_isLocalPath.Enabled = false;
                txtLocalPath.Enabled = false;
                bnt_localPathChose.Enabled = false;
                bnt_run.Visible = false;
                bnt_Stop.Visible = true;
                bnt_SaveSetting.Enabled = false;
                lab_status.Visible = true;
                cbx_comportlist.Enabled = false;
                cbx_WeighterType.Enabled = false;
                txt_BaudRate.Enabled = false;
                cbx_Printer1.Enabled = false;
                cbx_Printer2.Enabled = false;
                cbx_Printer3.Enabled = false;
                cbx_Printer4.Enabled = false;
                txt_ZebraPort.Enabled = false;
            }
            else
            {
                ts_Start.Enabled = true;
                ts_Stop.Enabled = false;
                txtServicePort.Enabled = true;
                bnt_run.Visible = true;
                bnt_SaveSetting.Enabled = true;
                bnt_Stop.Visible = false;
                lab_status.Visible = false;
                cb_isLocalPath.Enabled = true;
                cbx_comportlist.Enabled = true;
                cbx_WeighterType.Enabled = true;
                txt_BaudRate.Enabled = true;
                cbx_Printer1.Enabled = true;
                cbx_Printer2.Enabled = true;
                cbx_Printer3.Enabled = true;
                cbx_Printer4.Enabled = true;
                txt_ZebraPort.Enabled = true;
                if (cb_isLocalPath.Checked)
                {
                    txtLocalPath.Enabled = true;
                    bnt_localPathChose.Enabled = true;
                }
                else
                {
                    txtLocalPath.Enabled = false;
                    bnt_localPathChose.Enabled = false;
                }
            }
        }
        private void bnt_run_Click(object sender, EventArgs e)
        {
            InitMesClient();//初始化MESServer的链接
            int t = 0;
            while (SocketService.MESAPI.Token == null && t < 5)
            {
                Thread.Sleep(1000);
                t++;
            }
            if (SocketService.MESAPI.Token == null)
            {
                MessageBox.Show("SocketService.MESAPI.Token == null");
                return;
            }

            try
            {
                    ViewR.Start();
            }
            catch
            {
            }
            try
            {
                setStatus(true);
                string strClientPort = txtServicePort.Text.Trim();
                SocketService.BaudRate = Convert.ToInt32(txt_BaudRate.Text);
                SocketService.ZebraPort = txt_ZebraPort.Text;
                if (strClientPort.Length == 0)
                {
                    strClientPort = "2600";
                }
                SFCSocket = new WebSocketServer(System.Net.IPAddress.Any, Convert.ToInt32(strClientPort));
                SFCSocket.AddWebSocketService<SocketService>("/MESHelper");
                SFCSocket.Start();
            }
            catch (Exception ex)
            {
                setStatus(false);
                log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 執行錯誤：" + ex.Message);
            }
        }
        private void ViewRefush()
        {
            bgdg dg = delegate ()
            {
                if (lab_status.ForeColor == Color.Red)
                {
                    lab_status.ForeColor = Color.Black;
                }
                else
                {
                    lab_status.ForeColor = Color.Red;
                }
                lab_status.Controls.Clear();
                lab_status.Refresh();
            };
            new Thread(new ThreadStart(() =>
            {
                while (reflush)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        lab_status.BeginInvoke(dg);
                    }
                    catch
                    { }
                }
            })).Start();
        }
        private void bnt_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                ViewR.Abort();
            }
            catch
            {
            }
            try
            {
                Plugin.Printer_Codesoft._app.Quit();
                Plugin.Printer_Codesoft._app = null;
            }
            catch
            { }

            try
            {
                try
                {
                    SocketService.reader.close();
                }
                catch
                {
                    SocketService.reader = null;
                }
                setStatus(false);
                SFCSocket.Stop();
                log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 停止偵聽");
            }
            catch (Exception ex)
            {
                log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 執行關閉錯誤：" + ex.Message);
            }
        }
        private void HelperForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
            }
        }
        private void txtServicePort_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtServicePort.Text.Length == 0)
            {
                txtServicePort.Text = "2130";
            }
        }
        private void bnt_localPathChose_Click(object sender, EventArgs e)
        {
            DialogResult D = folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                txtLocalPath.Text = folderBrowserDialog1.SelectedPath;
                SocketService.LocalPath = txtLocalPath.Text;
            }
        }
        private List<string> GetComlist(bool isUseReg)
        {
            List<string> list = new List<string>();
            try
            {
                if (isUseReg)
                {
                    RegistryKey RootKey = Registry.LocalMachine;
                    RegistryKey Comkey = RootKey.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");
                    String[] ComNames;
                    if (Comkey != null)
                    {
                        ComNames = Comkey.GetValueNames();

                        foreach (String ComNamekey in ComNames)
                        {
                            string TemS = Comkey.GetValue(ComNamekey).ToString();
                            list.Add(TemS);
                        }
                    }
                    else
                    {
                        list.Add("COM1");
                    }
                }
                else
                {
                    foreach (string com in System.IO.Ports.SerialPort.GetPortNames())  //自动获取串行口名称  
                    {
                        list.Add(com);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("獲取COM口列表異常:" + ex.Message);
            }
            return list;
        }
        private void cbx_WeighterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbx_WeighterType.SelectedItem.ToString() == "連續傳輸")
                {
                    SocketService.WeighterType = "1";
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "穩定傳輸")
                {
                    SocketService.WeighterType = "2";
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "帶ST標記")
                {
                    SocketService.WeighterType = "3";
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "不帶ST標記")
                {
                    SocketService.WeighterType = "4";
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "次數:重量+單位")
                {
                    SocketService.WeighterType = "5";
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "Net         0.4570 kg")
                {
                    SocketService.WeighterType = "6";
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "(\\\\d.\\\\d) kg G")
                {
                    SocketService.WeighterType = "7";
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "Regex")
                {
                    SocketService.WeighterType = "8";
                    SocketService.WeighterRegex = Txt_WeightRegex.Text;
                }
                else if (cbx_WeighterType.SelectedItem.ToString() == "Input")
                {
                    SocketService.WeighterType = "9";
                    
                }
            }
            catch (Exception ex)
            {
                log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 獲取電子稱傳輸類型錯誤:" + ex.Message);
            }
        }
        private void cbx_comportlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SocketService.ComPortName = cbx_comportlist.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" 獲取電子稱傳輸端口錯誤:" + ex.Message);
            }
        }
        private void txt_BaudRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void txt_BaudRate_KeyUp(object sender, KeyEventArgs e)
        {
            if (txt_BaudRate.Text.Length == 0)
            {
                txt_BaudRate.Text = "0";
            }
            SocketService.BaudRate = Convert.ToInt32(txt_BaudRate.Text);
        }
        private void HelperForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bnt_Stop_Click(this, new EventArgs());
            reflush = false;
            try
            {
                SFCSocket.Stop();
            }
            catch
            {
            }
            try
            {
                SFCSocket.RemoveWebSocketService("/SFCSocketService");
            }
            catch
            {
            }
            try
            {
                ViewR.DisableComObjectEagerCleanup();
            }
            catch (Exception)
            {
                throw;
            }
            SocketService.reader = null;
            SFCSocket = null;
        }
        private void CheckProcess()
        {
            System.Diagnostics.Process[] processes;
            //Get the list of current active processes.
            processes = System.Diagnostics.Process.GetProcesses();
            //Grab some basic information for each process.
            System.Diagnostics.Process process;
            for (int i = 0; i < processes.Length - 1; i++)
            {
                process = processes[i];
                if (process.ProcessName.Contains("MESHelper") && process.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
                {
                    process.Kill();
                    break;
                }
            }
        }
        private void cbx_Printer1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SocketService.PrinterList[0] = cbx_Printer1.SelectedItem.ToString();
        }
        private void cbx_Printer2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SocketService.PrinterList[1] = cbx_Printer2.SelectedItem.ToString();
        }
        private void cbx_Printer3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SocketService.PrinterList[2] = cbx_Printer3.SelectedItem.ToString();
        }
        private void cbx_Printer4_SelectedIndexChanged(object sender, EventArgs e)
        {
            SocketService.PrinterList[3] = cbx_Printer4.SelectedItem.ToString();
        }
        private void txt_ZebraPort_KeyUp(object sender, KeyEventArgs e)
        {
            SocketService.ZebraPort = txt_ZebraPort.Text;
        }
        public static void setRegKey(string Key, string Value)
        {
            Microsoft.Win32.RegistryKey HKLM = Registry.CurrentUser;
            Microsoft.Win32.RegistryKey hkSoftware = HKLM.OpenSubKey("Software", true);
            Microsoft.Win32.RegistryKey hkFoxconn = hkSoftware.CreateSubKey("Foxconn");
            Microsoft.Win32.RegistryKey hkMine = hkFoxconn.CreateSubKey("SFCSocketService");

            hkMine.SetValue(Key, Value);
        }
        public static String getRegValue(string Key)
        {
            Microsoft.Win32.RegistryKey HKLM = Registry.CurrentUser;
            Microsoft.Win32.RegistryKey hkSoftware = HKLM.OpenSubKey("Software", true);
            Microsoft.Win32.RegistryKey hkFoxconn = hkSoftware.CreateSubKey("Foxconn");
            Microsoft.Win32.RegistryKey hkMine = hkFoxconn.CreateSubKey("SFCSocketService");

            string v;
            try
            {
                v = (string)hkMine.GetValue(Key);
            }
            catch
            {
                return "";
            }
            if (v == null)
            {
                v = "";
            }
            return v;
        }
        private void bnt_SaveSetting_Click(object sender, EventArgs e)
        {
            setRegKey("LocalPath", txtLocalPath.Text);
            setRegKey("isLocalPath", cb_isLocalPath.Checked.ToString());
            setRegKey("ZebraPort", txt_ZebraPort.Text);
            setRegKey("BaudRate", txt_BaudRate.Text);
            if (cbx_Printer1.SelectedIndex >= 0)
            {
                setRegKey("Printer1", cbx_Printer1.SelectedItem.ToString());
            }
            if (cbx_Printer2.SelectedIndex >= 0)
            {
                setRegKey("Printer2", cbx_Printer2.SelectedItem.ToString());
            }
            if (cbx_Printer3.SelectedIndex >= 0)
            {
                setRegKey("Printer3", cbx_Printer3.SelectedItem.ToString());
            }
            if (cbx_Printer4.SelectedIndex >= 0)
            {
                setRegKey("Printer4", cbx_Printer4.SelectedItem.ToString());
            }
            if (cbx_comportlist.SelectedIndex >= 0)
            {
                setRegKey("ComPortName", cbx_comportlist.SelectedItem.ToString());
            }
            if (cbx_WeighterType.SelectedIndex >= 0)
            {
                setRegKey("WeighterType", cbx_WeighterType.SelectedItem.ToString());
            }
            setRegKey("WeightRegex", Txt_WeightRegex.Text);
            setRegKey("DownFile", txtDownFile.Text);
        }

        private void cb_isLocalPath_CheckStateChanged(object sender, EventArgs e)
        {
            if (cb_isLocalPath.Checked)
            {
                SocketService.isLocalPath = true;
                txtLocalPath.Enabled = true;
                bnt_localPathChose.Enabled = true;
            }
            else
            {
                SocketService.isLocalPath = false;
                txtLocalPath.Enabled = false;
                bnt_localPathChose.Enabled = false;
            }
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                this.Visible = true;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                    ts_Option.Enabled = false;
                }
                else
                {
                    this.WindowState = FormWindowState.Minimized;
                    ts_Option.Enabled = true;
                }
            }
            else
            {

            }
        }
        private void MenuStrip_RightBnt_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string ss = e.ClickedItem.Name;
            switch (e.ClickedItem.Name)
            {
                case "ts_Option":
                    this.Visible = true;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                        ts_Option.Enabled = false;
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Minimized;
                        ts_Option.Enabled = true;
                    }
                    break;
                case "ts_Start":
                    bnt_run_Click(sender, new EventArgs());
                    break;
                case "ts_Stop":
                    bnt_Stop_Click(sender, new EventArgs());
                    break;
                case "ts_Exit":
                    HelperForm_FormClosing(sender, new FormClosingEventArgs(CloseReason.ApplicationExitCall, true));
                    Application.Exit();
                    break;
                default:
                    break;
            }
        }

        private void Helper_Load(object sender, EventArgs e)
        {

        }

        private void Helper_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Plugin.Printer_Codesoft._app.Quit();
                Plugin.Printer_Codesoft._app = null;
            }
            catch
            { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Lab_ComData.Text = $@"Com:'{SocketService.ComMsg}'";
            labWeight.Text = $@"Weight:{SocketService.WeightData}";
            txtLog.Text = SocketService.Log;
        }
        string filepath;
        private void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime t = DateTime.Now;
                //SocketService.client.GetFile(txtDownFile.Text, "LABEL", GetFile_Handler);
                var fname = SocketService.MESAPI.GetFile(txtDownFile.Text, "LABEL", LabelTempPath);
                var ts = DateTime.Now - t;
                MessageBox.Show($@"OK:time use: {ts.TotalSeconds} s");
            }
            catch (Exception ee)
            {
                filepath = "";
                MessageBox.Show(ee.Message);
            }
        }

        private void GetFile_Handler(object sender, MessageEventArgs e)
        {
            WebSocket w = (WebSocket)sender;
            w.OnMessage -= GetFile_Handler;
            JObject Request = (JObject)JsonConvert.DeserializeObject(e.Data);
            if (Request["Status"].ToString() == "Pass")
            {
                JObject Data = (JObject)Request["Data"];
                var LocalFile = Data["FILENAME"].ToString();
                filepath = LabelTempPath + "\\" + Data["FILENAME"].ToString();
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                FileStream F = new FileStream(filepath, FileMode.Create);
                byte[] b = (byte[])Data["BLOB_FILE"];
                F.Write(b, 0, b.Length);
                F.Flush();
                F.Close();
            }
            else
            {
                //MessageBox.Show(Request["Message"].ToString());
                //client.Renew();
            }
            w.SyncRequest = (JObject)JsonConvert.DeserializeObject(e.Data);
        }

        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            if (SocketService.reader == null)
            {
                SocketService.reader = new Reader_COM(SocketService.ComPortName, SocketService.BaudRate, SocketService.WeighterType);
            }
            if (!SocketService.reader.COM.IsOpen)
            {
                try
                {
                    SocketService.reader.Open();
                }
                catch(Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //SocketService.client.GetFile(txtDownFile.Text, "LABEL", GetFile_Handler);
                var fname = SocketService.MESAPI.GetFile(txtDownFile.Text, "LABEL", LabelTempPath);
                filepath = LabelTempPath + "\\" + fname;
                //MessageBox.Show("OK");
            }
            catch (Exception ee)
            {
                filepath = "";
                MessageBox.Show(ee.Message);
            }
            if (filepath != "")
            {
                var _csl = new Printer_Codesoft(filepath);
                _csl.Print(1);
            }
        }
    }
}
