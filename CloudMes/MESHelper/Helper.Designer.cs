namespace MESHelper
{
    partial class Helper
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Helper));
            this.bnt_SaveSetting = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_tx = new System.Windows.Forms.TabPage();
            this.cmb_bu = new System.Windows.Forms.ComboBox();
            this.lab_bu = new System.Windows.Forms.Label();
            this.lab_TestServer = new System.Windows.Forms.Label();
            this.txt_Password = new System.Windows.Forms.TextBox();
            this.txt_UserName = new System.Windows.Forms.TextBox();
            this.txt_MESServer = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.lab_UserName = new System.Windows.Forms.Label();
            this.lab_MESServer = new System.Windows.Forms.Label();
            this.txtServicePort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tab_print = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.cb_isLocalPath = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.bnt_localPathChose = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtLocalPath = new System.Windows.Forms.TextBox();
            this.cbx_Printer4 = new System.Windows.Forms.ComboBox();
            this.cbx_Printer1 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ZebraPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbx_Printer3 = new System.Windows.Forms.ComboBox();
            this.cbx_Printer2 = new System.Windows.Forms.ComboBox();
            this.tab_comport = new System.Windows.Forms.TabPage();
            this.labWeight = new System.Windows.Forms.Label();
            this.btnOpenCom = new System.Windows.Forms.Button();
            this.Lab_ComData = new System.Windows.Forms.Label();
            this.Txt_WeightRegex = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cbx_comportlist = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_BaudRate = new System.Windows.Forms.TextBox();
            this.cbx_WeighterType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.TxtTmpdir = new System.Windows.Forms.TextBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.txtDownFile = new System.Windows.Forms.TextBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.bnt_Stop = new System.Windows.Forms.Button();
            this.lab_status = new System.Windows.Forms.Label();
            this.bnt_run = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.MenuStrip_RightBnt = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ts_Option = new System.Windows.Forms.ToolStripMenuItem();
            this.ts_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.ts_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.ts_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tab_tx.SuspendLayout();
            this.tab_print.SuspendLayout();
            this.tab_comport.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.MenuStrip_RightBnt.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnt_SaveSetting
            // 
            this.bnt_SaveSetting.Location = new System.Drawing.Point(379, 31);
            this.bnt_SaveSetting.Name = "bnt_SaveSetting";
            this.bnt_SaveSetting.Size = new System.Drawing.Size(103, 25);
            this.bnt_SaveSetting.TabIndex = 28;
            this.bnt_SaveSetting.Text = "保存設置/Save";
            this.bnt_SaveSetting.UseVisualStyleBackColor = true;
            this.bnt_SaveSetting.Click += new System.EventHandler(this.bnt_SaveSetting_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tab_tx);
            this.tabControl1.Controls.Add(this.tab_print);
            this.tabControl1.Controls.Add(this.tab_comport);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(520, 354);
            this.tabControl1.TabIndex = 27;
            // 
            // tab_tx
            // 
            this.tab_tx.Controls.Add(this.cmb_bu);
            this.tab_tx.Controls.Add(this.lab_bu);
            this.tab_tx.Controls.Add(this.lab_TestServer);
            this.tab_tx.Controls.Add(this.txt_Password);
            this.tab_tx.Controls.Add(this.txt_UserName);
            this.tab_tx.Controls.Add(this.txt_MESServer);
            this.tab_tx.Controls.Add(this.label15);
            this.tab_tx.Controls.Add(this.lab_UserName);
            this.tab_tx.Controls.Add(this.lab_MESServer);
            this.tab_tx.Controls.Add(this.txtServicePort);
            this.tab_tx.Controls.Add(this.label1);
            this.tab_tx.Location = new System.Drawing.Point(4, 22);
            this.tab_tx.Name = "tab_tx";
            this.tab_tx.Padding = new System.Windows.Forms.Padding(3);
            this.tab_tx.Size = new System.Drawing.Size(512, 328);
            this.tab_tx.TabIndex = 0;
            this.tab_tx.Text = "通訊設置";
            this.tab_tx.ToolTipText = "Communication Settings";
            this.tab_tx.UseVisualStyleBackColor = true;
            // 
            // cmb_bu
            // 
            this.cmb_bu.FormattingEnabled = true;
            this.cmb_bu.Location = new System.Drawing.Point(91, 68);
            this.cmb_bu.Name = "cmb_bu";
            this.cmb_bu.Size = new System.Drawing.Size(121, 21);
            this.cmb_bu.TabIndex = 13;
            // 
            // lab_bu
            // 
            this.lab_bu.AutoSize = true;
            this.lab_bu.Location = new System.Drawing.Point(61, 73);
            this.lab_bu.Name = "lab_bu";
            this.lab_bu.Size = new System.Drawing.Size(25, 13);
            this.lab_bu.TabIndex = 12;
            this.lab_bu.Text = "BU:";
            // 
            // lab_TestServer
            // 
            this.lab_TestServer.AutoSize = true;
            this.lab_TestServer.Location = new System.Drawing.Point(91, 224);
            this.lab_TestServer.Name = "lab_TestServer";
            this.lab_TestServer.Size = new System.Drawing.Size(0, 13);
            this.lab_TestServer.TabIndex = 11;
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(91, 186);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(140, 20);
            this.txt_Password.TabIndex = 9;
            this.txt_Password.Text = "PRINT";
            this.txt_Password.UseSystemPasswordChar = true;
            // 
            // txt_UserName
            // 
            this.txt_UserName.Location = new System.Drawing.Point(91, 146);
            this.txt_UserName.Name = "txt_UserName";
            this.txt_UserName.Size = new System.Drawing.Size(140, 20);
            this.txt_UserName.TabIndex = 8;
            this.txt_UserName.Text = "PRINT";
            // 
            // txt_MESServer
            // 
            this.txt_MESServer.Location = new System.Drawing.Point(91, 106);
            this.txt_MESServer.Multiline = true;
            this.txt_MESServer.Name = "txt_MESServer";
            this.txt_MESServer.Size = new System.Drawing.Size(275, 24);
            this.txt_MESServer.TabIndex = 7;
            this.txt_MESServer.Text = "10.221.86.122:2130";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(34, 190);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Password:";
            // 
            // lab_UserName
            // 
            this.lab_UserName.AutoSize = true;
            this.lab_UserName.Location = new System.Drawing.Point(26, 151);
            this.lab_UserName.Name = "lab_UserName";
            this.lab_UserName.Size = new System.Drawing.Size(63, 13);
            this.lab_UserName.TabIndex = 5;
            this.lab_UserName.Text = "User Name:";
            // 
            // lab_MESServer
            // 
            this.lab_MESServer.AutoSize = true;
            this.lab_MESServer.Location = new System.Drawing.Point(21, 112);
            this.lab_MESServer.Name = "lab_MESServer";
            this.lab_MESServer.Size = new System.Drawing.Size(67, 13);
            this.lab_MESServer.TabIndex = 4;
            this.lab_MESServer.Text = "MES Server:";
            // 
            // txtServicePort
            // 
            this.txtServicePort.Location = new System.Drawing.Point(91, 28);
            this.txtServicePort.Name = "txtServicePort";
            this.txtServicePort.Size = new System.Drawing.Size(55, 20);
            this.txtServicePort.TabIndex = 2;
            this.txtServicePort.Text = "2600";
            this.txtServicePort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_BaudRate_KeyPress);
            this.txtServicePort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtServicePort_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Local Port:";
            // 
            // tab_print
            // 
            this.tab_print.Controls.Add(this.label14);
            this.tab_print.Controls.Add(this.cb_isLocalPath);
            this.tab_print.Controls.Add(this.label13);
            this.tab_print.Controls.Add(this.bnt_localPathChose);
            this.tab_print.Controls.Add(this.label12);
            this.tab_print.Controls.Add(this.label3);
            this.tab_print.Controls.Add(this.label11);
            this.tab_print.Controls.Add(this.txtLocalPath);
            this.tab_print.Controls.Add(this.cbx_Printer4);
            this.tab_print.Controls.Add(this.cbx_Printer1);
            this.tab_print.Controls.Add(this.label10);
            this.tab_print.Controls.Add(this.label2);
            this.tab_print.Controls.Add(this.txt_ZebraPort);
            this.tab_print.Controls.Add(this.label7);
            this.tab_print.Controls.Add(this.label9);
            this.tab_print.Controls.Add(this.label8);
            this.tab_print.Controls.Add(this.cbx_Printer3);
            this.tab_print.Controls.Add(this.cbx_Printer2);
            this.tab_print.Location = new System.Drawing.Point(4, 22);
            this.tab_print.Name = "tab_print";
            this.tab_print.Padding = new System.Windows.Forms.Padding(3);
            this.tab_print.Size = new System.Drawing.Size(512, 328);
            this.tab_print.TabIndex = 1;
            this.tab_print.Text = "打印機設置";
            this.tab_print.ToolTipText = "Printer settings";
            this.tab_print.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label14.Location = new System.Drawing.Point(218, 187);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(169, 13);
            this.label14.TabIndex = 35;
            this.label14.Text = "PALLET LIST列印到四號打印機";
            // 
            // cb_isLocalPath
            // 
            this.cb_isLocalPath.AutoSize = true;
            this.cb_isLocalPath.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_isLocalPath.Location = new System.Drawing.Point(17, 24);
            this.cb_isLocalPath.Name = "cb_isLocalPath";
            this.cb_isLocalPath.Size = new System.Drawing.Size(122, 17);
            this.cb_isLocalPath.TabIndex = 12;
            this.cb_isLocalPath.Text = "使用本地模板路徑";
            this.cb_isLocalPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_isLocalPath.UseVisualStyleBackColor = true;
            this.cb_isLocalPath.Click += new System.EventHandler(this.cb_isLocalPath_CheckStateChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label13.Location = new System.Drawing.Point(218, 157);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(155, 13);
            this.label13.TabIndex = 34;
            this.label13.Text = "PALLET標籤列印到三號標籤";
            // 
            // bnt_localPathChose
            // 
            this.bnt_localPathChose.Location = new System.Drawing.Point(308, 54);
            this.bnt_localPathChose.Name = "bnt_localPathChose";
            this.bnt_localPathChose.Size = new System.Drawing.Size(75, 25);
            this.bnt_localPathChose.TabIndex = 10;
            this.bnt_localPathChose.Text = "選擇";
            this.bnt_localPathChose.UseVisualStyleBackColor = true;
            this.bnt_localPathChose.Click += new System.EventHandler(this.bnt_localPathChose_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label12.Location = new System.Drawing.Point(218, 127);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(172, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "CARTON標籤列印到二號打印機";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "本地模板:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label11.Location = new System.Drawing.Point(218, 96);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(175, 13);
            this.label11.TabIndex = 32;
            this.label11.Text = "一般標籤默認列印到一號打印機";
            // 
            // txtLocalPath
            // 
            this.txtLocalPath.Location = new System.Drawing.Point(91, 54);
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.Size = new System.Drawing.Size(196, 20);
            this.txtLocalPath.TabIndex = 8;
            // 
            // cbx_Printer4
            // 
            this.cbx_Printer4.FormattingEnabled = true;
            this.cbx_Printer4.Location = new System.Drawing.Point(91, 185);
            this.cbx_Printer4.Name = "cbx_Printer4";
            this.cbx_Printer4.Size = new System.Drawing.Size(121, 21);
            this.cbx_Printer4.TabIndex = 31;
            this.cbx_Printer4.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer4_SelectedIndexChanged);
            // 
            // cbx_Printer1
            // 
            this.cbx_Printer1.FormattingEnabled = true;
            this.cbx_Printer1.Location = new System.Drawing.Point(91, 91);
            this.cbx_Printer1.Name = "cbx_Printer1";
            this.cbx_Printer1.Size = new System.Drawing.Size(121, 21);
            this.cbx_Printer1.TabIndex = 21;
            this.cbx_Printer1.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer1_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(17, 190);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "四號打印機:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "一號打印機:";
            // 
            // txt_ZebraPort
            // 
            this.txt_ZebraPort.Location = new System.Drawing.Point(106, 213);
            this.txt_ZebraPort.Name = "txt_ZebraPort";
            this.txt_ZebraPort.Size = new System.Drawing.Size(45, 20);
            this.txt_ZebraPort.TabIndex = 28;
            this.txt_ZebraPort.Text = "LPT1";
            this.txt_ZebraPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_ZebraPort_KeyUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 127);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "二號打印機:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 218);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(86, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Zebra打印端口:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "三號打印機:";
            // 
            // cbx_Printer3
            // 
            this.cbx_Printer3.FormattingEnabled = true;
            this.cbx_Printer3.Location = new System.Drawing.Point(91, 154);
            this.cbx_Printer3.Name = "cbx_Printer3";
            this.cbx_Printer3.Size = new System.Drawing.Size(121, 21);
            this.cbx_Printer3.TabIndex = 26;
            this.cbx_Printer3.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer3_SelectedIndexChanged);
            // 
            // cbx_Printer2
            // 
            this.cbx_Printer2.FormattingEnabled = true;
            this.cbx_Printer2.Location = new System.Drawing.Point(91, 122);
            this.cbx_Printer2.Name = "cbx_Printer2";
            this.cbx_Printer2.Size = new System.Drawing.Size(121, 21);
            this.cbx_Printer2.TabIndex = 25;
            this.cbx_Printer2.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer2_SelectedIndexChanged);
            // 
            // tab_comport
            // 
            this.tab_comport.Controls.Add(this.labWeight);
            this.tab_comport.Controls.Add(this.btnOpenCom);
            this.tab_comport.Controls.Add(this.Lab_ComData);
            this.tab_comport.Controls.Add(this.Txt_WeightRegex);
            this.tab_comport.Controls.Add(this.label16);
            this.tab_comport.Controls.Add(this.cbx_comportlist);
            this.tab_comport.Controls.Add(this.label5);
            this.tab_comport.Controls.Add(this.txt_BaudRate);
            this.tab_comport.Controls.Add(this.cbx_WeighterType);
            this.tab_comport.Controls.Add(this.label6);
            this.tab_comport.Controls.Add(this.label4);
            this.tab_comport.Location = new System.Drawing.Point(4, 22);
            this.tab_comport.Name = "tab_comport";
            this.tab_comport.Size = new System.Drawing.Size(512, 328);
            this.tab_comport.TabIndex = 2;
            this.tab_comport.Text = "COM口設置";
            this.tab_comport.ToolTipText = "COM Port Settings";
            this.tab_comport.UseVisualStyleBackColor = true;
            // 
            // labWeight
            // 
            this.labWeight.AutoSize = true;
            this.labWeight.Location = new System.Drawing.Point(236, 164);
            this.labWeight.Name = "labWeight";
            this.labWeight.Size = new System.Drawing.Size(41, 13);
            this.labWeight.TabIndex = 24;
            this.labWeight.Text = "label17";
            // 
            // btnOpenCom
            // 
            this.btnOpenCom.Location = new System.Drawing.Point(254, 23);
            this.btnOpenCom.Name = "btnOpenCom";
            this.btnOpenCom.Size = new System.Drawing.Size(75, 25);
            this.btnOpenCom.TabIndex = 23;
            this.btnOpenCom.Text = "Open";
            this.btnOpenCom.UseVisualStyleBackColor = true;
            this.btnOpenCom.Click += new System.EventHandler(this.btnOpenCom_Click);
            // 
            // Lab_ComData
            // 
            this.Lab_ComData.AutoSize = true;
            this.Lab_ComData.Location = new System.Drawing.Point(23, 164);
            this.Lab_ComData.Name = "Lab_ComData";
            this.Lab_ComData.Size = new System.Drawing.Size(41, 13);
            this.Lab_ComData.TabIndex = 22;
            this.Lab_ComData.Text = "label17";
            // 
            // Txt_WeightRegex
            // 
            this.Txt_WeightRegex.Location = new System.Drawing.Point(76, 119);
            this.Txt_WeightRegex.Name = "Txt_WeightRegex";
            this.Txt_WeightRegex.Size = new System.Drawing.Size(225, 20);
            this.Txt_WeightRegex.TabIndex = 21;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(29, 122);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 13);
            this.label16.TabIndex = 20;
            this.label16.Text = "Regex:";
            // 
            // cbx_comportlist
            // 
            this.cbx_comportlist.FormattingEnabled = true;
            this.cbx_comportlist.Location = new System.Drawing.Point(79, 23);
            this.cbx_comportlist.Name = "cbx_comportlist";
            this.cbx_comportlist.Size = new System.Drawing.Size(121, 21);
            this.cbx_comportlist.TabIndex = 19;
            this.cbx_comportlist.SelectedIndexChanged += new System.EventHandler(this.cbx_comportlist_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "波特率:";
            // 
            // txt_BaudRate
            // 
            this.txt_BaudRate.Location = new System.Drawing.Point(79, 55);
            this.txt_BaudRate.MaxLength = 5;
            this.txt_BaudRate.Name = "txt_BaudRate";
            this.txt_BaudRate.Size = new System.Drawing.Size(121, 20);
            this.txt_BaudRate.TabIndex = 16;
            this.txt_BaudRate.Text = "9600";
            this.txt_BaudRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_BaudRate_KeyPress);
            this.txt_BaudRate.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_BaudRate_KeyUp);
            // 
            // cbx_WeighterType
            // 
            this.cbx_WeighterType.FormattingEnabled = true;
            this.cbx_WeighterType.Items.AddRange(new object[] {
            "穩定傳輸",
            "連續傳輸",
            "帶ST標記",
            "不帶ST標記",
            "次數:重量+單位",
            "Net         0.4570 kg",
            "(\\\\d.\\\\d) kg G",
            "Regex",
            "Input"});
            this.cbx_WeighterType.Location = new System.Drawing.Point(79, 87);
            this.cbx_WeighterType.Name = "cbx_WeighterType";
            this.cbx_WeighterType.Size = new System.Drawing.Size(121, 21);
            this.cbx_WeighterType.TabIndex = 17;
            this.cbx_WeighterType.SelectedIndexChanged += new System.EventHandler(this.cbx_WeighterType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "傳輸類型:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "連接端口:";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TxtTmpdir);
            this.tabPage1.Controls.Add(this.btnPrint);
            this.tabPage1.Controls.Add(this.txtDownFile);
            this.tabPage1.Controls.Add(this.btnDown);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(512, 328);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "DownLoadLabel";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // TxtTmpdir
            // 
            this.TxtTmpdir.Location = new System.Drawing.Point(16, 92);
            this.TxtTmpdir.Name = "TxtTmpdir";
            this.TxtTmpdir.Size = new System.Drawing.Size(348, 20);
            this.TxtTmpdir.TabIndex = 3;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(282, 61);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 25);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "PrintLabel";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // txtDownFile
            // 
            this.txtDownFile.Location = new System.Drawing.Point(16, 18);
            this.txtDownFile.Name = "txtDownFile";
            this.txtDownFile.Size = new System.Drawing.Size(260, 20);
            this.txtDownFile.TabIndex = 1;
            this.txtDownFile.Text = "Juniper_TCIF child_Label.lab";
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(282, 18);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 25);
            this.btnDown.TabIndex = 0;
            this.btnDown.Text = "DownLoad";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtLog);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(512, 328);
            this.tabPage2.TabIndex = 4;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(506, 322);
            this.txtLog.TabIndex = 0;
            // 
            // bnt_Stop
            // 
            this.bnt_Stop.Location = new System.Drawing.Point(212, 31);
            this.bnt_Stop.Name = "bnt_Stop";
            this.bnt_Stop.Size = new System.Drawing.Size(107, 25);
            this.bnt_Stop.TabIndex = 26;
            this.bnt_Stop.Text = "Stop";
            this.bnt_Stop.UseVisualStyleBackColor = true;
            this.bnt_Stop.Click += new System.EventHandler(this.bnt_Stop_Click);
            // 
            // lab_status
            // 
            this.lab_status.AutoSize = true;
            this.lab_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_status.ForeColor = System.Drawing.Color.Black;
            this.lab_status.Location = new System.Drawing.Point(12, 17);
            this.lab_status.Name = "lab_status";
            this.lab_status.Size = new System.Drawing.Size(157, 39);
            this.lab_status.TabIndex = 25;
            this.lab_status.Text = "開始偵聽";
            // 
            // bnt_run
            // 
            this.bnt_run.Location = new System.Drawing.Point(212, 31);
            this.bnt_run.Name = "bnt_run";
            this.bnt_run.Size = new System.Drawing.Size(75, 25);
            this.bnt_run.TabIndex = 24;
            this.bnt_run.Text = "Run";
            this.bnt_run.UseVisualStyleBackColor = true;
            this.bnt_run.Click += new System.EventHandler(this.bnt_run_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.MenuStrip_RightBnt;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Socket Service";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // MenuStrip_RightBnt
            // 
            this.MenuStrip_RightBnt.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ts_Option,
            this.ts_Start,
            this.ts_Stop,
            this.ts_Exit});
            this.MenuStrip_RightBnt.Name = "MenuStrip_RightBnt";
            this.MenuStrip_RightBnt.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.MenuStrip_RightBnt.Size = new System.Drawing.Size(99, 92);
            this.MenuStrip_RightBnt.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuStrip_RightBnt_ItemClicked);
            // 
            // ts_Option
            // 
            this.ts_Option.Name = "ts_Option";
            this.ts_Option.Size = new System.Drawing.Size(98, 22);
            this.ts_Option.Text = "設置";
            // 
            // ts_Start
            // 
            this.ts_Start.Name = "ts_Start";
            this.ts_Start.Size = new System.Drawing.Size(98, 22);
            this.ts_Start.Text = "開始";
            // 
            // ts_Stop
            // 
            this.ts_Stop.Name = "ts_Stop";
            this.ts_Stop.Size = new System.Drawing.Size(98, 22);
            this.ts_Stop.Text = "停止";
            // 
            // ts_Exit
            // 
            this.ts_Exit.Name = "ts_Exit";
            this.ts_Exit.Size = new System.Drawing.Size(98, 22);
            this.ts_Exit.Text = "退出";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bnt_Stop);
            this.panel1.Controls.Add(this.bnt_run);
            this.panel1.Controls.Add(this.bnt_SaveSetting);
            this.panel1.Controls.Add(this.lab_status);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 268);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(520, 86);
            this.panel1.TabIndex = 29;
            // 
            // Helper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 354);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Helper";
            this.Text = "Mes Helper";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HelperForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Helper_FormClosed);
            this.Load += new System.EventHandler(this.Helper_Load);
            this.SizeChanged += new System.EventHandler(this.HelperForm_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tab_tx.ResumeLayout(false);
            this.tab_tx.PerformLayout();
            this.tab_print.ResumeLayout(false);
            this.tab_print.PerformLayout();
            this.tab_comport.ResumeLayout(false);
            this.tab_comport.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.MenuStrip_RightBnt.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnt_SaveSetting;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_tx;
        private System.Windows.Forms.TextBox txtServicePort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tab_print;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cb_isLocalPath;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button bnt_localPathChose;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtLocalPath;
        private System.Windows.Forms.ComboBox cbx_Printer4;
        private System.Windows.Forms.ComboBox cbx_Printer1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_ZebraPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbx_Printer3;
        private System.Windows.Forms.ComboBox cbx_Printer2;
        private System.Windows.Forms.TabPage tab_comport;
        private System.Windows.Forms.ComboBox cbx_comportlist;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_BaudRate;
        private System.Windows.Forms.ComboBox cbx_WeighterType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bnt_Stop;
        private System.Windows.Forms.Label lab_status;
        private System.Windows.Forms.Button bnt_run;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.ContextMenuStrip MenuStrip_RightBnt;
        private System.Windows.Forms.ToolStripMenuItem ts_Option;
        private System.Windows.Forms.ToolStripMenuItem ts_Start;
        private System.Windows.Forms.ToolStripMenuItem ts_Stop;
        private System.Windows.Forms.ToolStripMenuItem ts_Exit;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label lab_UserName;
        private System.Windows.Forms.Label lab_MESServer;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_Password;
        private System.Windows.Forms.TextBox txt_UserName;
        private System.Windows.Forms.TextBox txt_MESServer;
        private System.Windows.Forms.Label lab_TestServer;
        private System.Windows.Forms.ComboBox cmb_bu;
        private System.Windows.Forms.Label lab_bu;
        private System.Windows.Forms.TextBox Txt_WeightRegex;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label Lab_ComData;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.TextBox txtDownFile;
        private System.Windows.Forms.Button btnOpenCom;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.TextBox TxtTmpdir;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labWeight;
    }
}

