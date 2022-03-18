namespace WebServer
{
    partial class FormMain
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.labPath = new System.Windows.Forms.Label();
            this.btnCleanDBPool = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.ListFunctionNoRet = new System.Windows.Forms.ListBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.ListFunctionNoRetAP = new System.Windows.Forms.ListBox();
            this.btnGCStart = new System.Windows.Forms.Button();
            this.labServerInfo = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCertificatekey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCertificateFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSslProtocols = new System.Windows.Forms.TextBox();
            this.cbxUseSSL = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWeb_Download_Path = new System.Windows.Forms.TextBox();
            this.btnRegSave = new System.Windows.Forms.Button();
            this.labAPDB = new System.Windows.Forms.Label();
            this.txtAPDB = new System.Windows.Forms.TextBox();
            this.labSFCDB = new System.Windows.Forms.Label();
            this.txtSFCDB = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.GroupSession = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtKillUserSession = new System.Windows.Forms.TextBox();
            this.txtKillIPSession = new System.Windows.Forms.TextBox();
            this.btnKillUserSession = new System.Windows.Forms.Button();
            this.btnKillIPSession = new System.Windows.Forms.Button();
            this.btnKillSession = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSelect = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.txtShowCall = new System.Windows.Forms.TextBox();
            this.txtShowResponse = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labCallInfo = new System.Windows.Forms.Label();
            this.TimerGetServerInfo = new System.Windows.Forms.Timer(this.components);
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.listSFCLend = new System.Windows.Forms.ListBox();
            this.listAPLend = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.GroupSession.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(972, 500);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.labPath);
            this.tabPage1.Controls.Add(this.btnCleanDBPool);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.btnGCStart);
            this.tabPage1.Controls.Add(this.labServerInfo);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(964, 474);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "服务器运行状态";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // labPath
            // 
            this.labPath.AutoSize = true;
            this.labPath.Location = new System.Drawing.Point(22, 415);
            this.labPath.Name = "labPath";
            this.labPath.Size = new System.Drawing.Size(33, 12);
            this.labPath.TabIndex = 4;
            this.labPath.Text = "label2";
            // 
            // btnCleanDBPool
            // 
            this.btnCleanDBPool.Location = new System.Drawing.Point(137, 59);
            this.btnCleanDBPool.Name = "btnCleanDBPool";
            this.btnCleanDBPool.Size = new System.Drawing.Size(75, 23);
            this.btnCleanDBPool.TabIndex = 3;
            this.btnCleanDBPool.Text = "CL_DB_Pool";
            this.btnCleanDBPool.UseVisualStyleBackColor = true;
            this.btnCleanDBPool.Click += new System.EventHandler(this.btnCleanDBPool_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tabControl2);
            this.groupBox4.Location = new System.Drawing.Point(356, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(513, 390);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "监控借出连接不归还的方法";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Controls.Add(this.tabPage7);
            this.tabControl2.Controls.Add(this.tabPage8);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(3, 18);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(507, 369);
            this.tabControl2.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ListFunctionNoRet);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(499, 343);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "SFCDB";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // ListFunctionNoRet
            // 
            this.ListFunctionNoRet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListFunctionNoRet.FormattingEnabled = true;
            this.ListFunctionNoRet.ItemHeight = 12;
            this.ListFunctionNoRet.Location = new System.Drawing.Point(3, 3);
            this.ListFunctionNoRet.Name = "ListFunctionNoRet";
            this.ListFunctionNoRet.Size = new System.Drawing.Size(493, 337);
            this.ListFunctionNoRet.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.ListFunctionNoRetAP);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(499, 343);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "APDB";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // ListFunctionNoRetAP
            // 
            this.ListFunctionNoRetAP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListFunctionNoRetAP.FormattingEnabled = true;
            this.ListFunctionNoRetAP.ItemHeight = 12;
            this.ListFunctionNoRetAP.Location = new System.Drawing.Point(3, 3);
            this.ListFunctionNoRetAP.Name = "ListFunctionNoRetAP";
            this.ListFunctionNoRetAP.Size = new System.Drawing.Size(493, 337);
            this.ListFunctionNoRetAP.TabIndex = 1;
            // 
            // btnGCStart
            // 
            this.btnGCStart.Location = new System.Drawing.Point(137, 17);
            this.btnGCStart.Name = "btnGCStart";
            this.btnGCStart.Size = new System.Drawing.Size(75, 23);
            this.btnGCStart.TabIndex = 1;
            this.btnGCStart.Text = "GCStart";
            this.btnGCStart.UseVisualStyleBackColor = true;
            this.btnGCStart.Click += new System.EventHandler(this.btnGCStart_Click);
            // 
            // labServerInfo
            // 
            this.labServerInfo.AutoSize = true;
            this.labServerInfo.Location = new System.Drawing.Point(22, 17);
            this.labServerInfo.Name = "labServerInfo";
            this.labServerInfo.Size = new System.Drawing.Size(33, 12);
            this.labServerInfo.TabIndex = 0;
            this.labServerInfo.Text = "label2";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.txtCertificatekey);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.txtCertificateFile);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtSslProtocols);
            this.tabPage2.Controls.Add(this.cbxUseSSL);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.txtWeb_Download_Path);
            this.tabPage2.Controls.Add(this.btnRegSave);
            this.tabPage2.Controls.Add(this.labAPDB);
            this.tabPage2.Controls.Add(this.txtAPDB);
            this.tabPage2.Controls.Add(this.labSFCDB);
            this.tabPage2.Controls.Add(this.txtSFCDB);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(964, 474);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "配置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "Certificatekey";
            // 
            // txtCertificatekey
            // 
            this.txtCertificatekey.Location = new System.Drawing.Point(134, 209);
            this.txtCertificatekey.Name = "txtCertificatekey";
            this.txtCertificatekey.PasswordChar = '*';
            this.txtCertificatekey.Size = new System.Drawing.Size(668, 22);
            this.txtCertificatekey.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 183);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "CertificateFile";
            // 
            // txtCertificateFile
            // 
            this.txtCertificateFile.Location = new System.Drawing.Point(134, 180);
            this.txtCertificateFile.Name = "txtCertificateFile";
            this.txtCertificateFile.Size = new System.Drawing.Size(668, 22);
            this.txtCertificateFile.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(60, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "SslProtocols";
            // 
            // txtSslProtocols
            // 
            this.txtSslProtocols.Location = new System.Drawing.Point(134, 151);
            this.txtSslProtocols.Name = "txtSslProtocols";
            this.txtSslProtocols.Size = new System.Drawing.Size(668, 22);
            this.txtSslProtocols.TabIndex = 8;
            // 
            // cbxUseSSL
            // 
            this.cbxUseSSL.AutoSize = true;
            this.cbxUseSSL.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxUseSSL.Location = new System.Drawing.Point(72, 129);
            this.cbxUseSSL.Name = "cbxUseSSL";
            this.cbxUseSSL.Size = new System.Drawing.Size(76, 16);
            this.cbxUseSSL.TabIndex = 7;
            this.cbxUseSSL.Text = "USE_SSL  ";
            this.cbxUseSSL.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Web_Download_Path";
            // 
            // txtWeb_Download_Path
            // 
            this.txtWeb_Download_Path.Location = new System.Drawing.Point(136, 96);
            this.txtWeb_Download_Path.Name = "txtWeb_Download_Path";
            this.txtWeb_Download_Path.Size = new System.Drawing.Size(668, 22);
            this.txtWeb_Download_Path.TabIndex = 5;
            // 
            // btnRegSave
            // 
            this.btnRegSave.Location = new System.Drawing.Point(837, 17);
            this.btnRegSave.Name = "btnRegSave";
            this.btnRegSave.Size = new System.Drawing.Size(75, 23);
            this.btnRegSave.TabIndex = 4;
            this.btnRegSave.Text = "SAVE";
            this.btnRegSave.UseVisualStyleBackColor = true;
            this.btnRegSave.Click += new System.EventHandler(this.btnRegSave_Click);
            // 
            // labAPDB
            // 
            this.labAPDB.AutoSize = true;
            this.labAPDB.Location = new System.Drawing.Point(89, 62);
            this.labAPDB.Name = "labAPDB";
            this.labAPDB.Size = new System.Drawing.Size(35, 12);
            this.labAPDB.TabIndex = 3;
            this.labAPDB.Text = "APDB";
            // 
            // txtAPDB
            // 
            this.txtAPDB.Location = new System.Drawing.Point(136, 59);
            this.txtAPDB.Name = "txtAPDB";
            this.txtAPDB.Size = new System.Drawing.Size(668, 22);
            this.txtAPDB.TabIndex = 2;
            // 
            // labSFCDB
            // 
            this.labSFCDB.AutoSize = true;
            this.labSFCDB.Location = new System.Drawing.Point(83, 28);
            this.labSFCDB.Name = "labSFCDB";
            this.labSFCDB.Size = new System.Drawing.Size(41, 12);
            this.labSFCDB.TabIndex = 1;
            this.labSFCDB.Text = "SFCDB";
            // 
            // txtSFCDB
            // 
            this.txtSFCDB.Location = new System.Drawing.Point(136, 19);
            this.txtSFCDB.Name = "txtSFCDB";
            this.txtSFCDB.Size = new System.Drawing.Size(668, 22);
            this.txtSFCDB.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(964, 474);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "状态追踪";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.GroupSession);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(958, 468);
            this.splitContainer1.SplitterDistance = 220;
            this.splitContainer1.TabIndex = 0;
            // 
            // GroupSession
            // 
            this.GroupSession.Controls.Add(this.treeView1);
            this.GroupSession.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupSession.Location = new System.Drawing.Point(0, 0);
            this.GroupSession.Name = "GroupSession";
            this.GroupSession.Size = new System.Drawing.Size(220, 468);
            this.GroupSession.TabIndex = 0;
            this.GroupSession.TabStop = false;
            this.GroupSession.Text = "Session";
            this.GroupSession.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 18);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(214, 447);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtKillUserSession);
            this.groupBox2.Controls.Add(this.txtKillIPSession);
            this.groupBox2.Controls.Add(this.btnKillUserSession);
            this.groupBox2.Controls.Add(this.btnKillIPSession);
            this.groupBox2.Controls.Add(this.btnKillSession);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtSelect);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(734, 468);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // txtKillUserSession
            // 
            this.txtKillUserSession.Location = new System.Drawing.Point(91, 117);
            this.txtKillUserSession.Name = "txtKillUserSession";
            this.txtKillUserSession.Size = new System.Drawing.Size(153, 22);
            this.txtKillUserSession.TabIndex = 7;
            // 
            // txtKillIPSession
            // 
            this.txtKillIPSession.Location = new System.Drawing.Point(91, 89);
            this.txtKillIPSession.Name = "txtKillIPSession";
            this.txtKillIPSession.Size = new System.Drawing.Size(153, 22);
            this.txtKillIPSession.TabIndex = 6;
            // 
            // btnKillUserSession
            // 
            this.btnKillUserSession.Location = new System.Drawing.Point(6, 118);
            this.btnKillUserSession.Name = "btnKillUserSession";
            this.btnKillUserSession.Size = new System.Drawing.Size(79, 23);
            this.btnKillUserSession.TabIndex = 5;
            this.btnKillUserSession.Text = "KillUserSession";
            this.btnKillUserSession.UseVisualStyleBackColor = true;
            this.btnKillUserSession.Click += new System.EventHandler(this.btnKillUserSession_Click);
            // 
            // btnKillIPSession
            // 
            this.btnKillIPSession.Location = new System.Drawing.Point(6, 89);
            this.btnKillIPSession.Name = "btnKillIPSession";
            this.btnKillIPSession.Size = new System.Drawing.Size(79, 23);
            this.btnKillIPSession.TabIndex = 4;
            this.btnKillIPSession.Text = "KillIPSession";
            this.btnKillIPSession.UseVisualStyleBackColor = true;
            this.btnKillIPSession.Click += new System.EventHandler(this.btnKillIPSession_Click);
            // 
            // btnKillSession
            // 
            this.btnKillSession.Location = new System.Drawing.Point(6, 60);
            this.btnKillSession.Name = "btnKillSession";
            this.btnKillSession.Size = new System.Drawing.Size(79, 23);
            this.btnKillSession.TabIndex = 3;
            this.btnKillSession.Text = "killSession";
            this.btnKillSession.UseVisualStyleBackColor = true;
            this.btnKillSession.Click += new System.EventHandler(this.btnKillSession_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(153, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "筛选";
            // 
            // txtSelect
            // 
            this.txtSelect.Location = new System.Drawing.Point(192, 20);
            this.txtSelect.Name = "txtSelect";
            this.txtSelect.Size = new System.Drawing.Size(174, 22);
            this.txtSelect.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 21);
            this.button1.TabIndex = 0;
            this.button1.Text = "刷新列表";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer2);
            this.tabPage4.Controls.Add(this.panel1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(964, 474);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "调用追踪";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 44);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer2.Size = new System.Drawing.Size(958, 427);
            this.splitContainer2.SplitterDistance = 214;
            this.splitContainer2.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.treeView2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 427);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "调用";
            // 
            // treeView2
            // 
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView2.Location = new System.Drawing.Point(3, 18);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(208, 406);
            this.treeView2.TabIndex = 0;
            this.treeView2.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView2_NodeMouseClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.splitContainer3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(740, 427);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "输入输出";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 18);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.txtShowCall);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtShowResponse);
            this.splitContainer3.Size = new System.Drawing.Size(734, 406);
            this.splitContainer3.SplitterDistance = 310;
            this.splitContainer3.TabIndex = 0;
            // 
            // txtShowCall
            // 
            this.txtShowCall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtShowCall.Location = new System.Drawing.Point(0, 0);
            this.txtShowCall.Multiline = true;
            this.txtShowCall.Name = "txtShowCall";
            this.txtShowCall.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtShowCall.Size = new System.Drawing.Size(310, 406);
            this.txtShowCall.TabIndex = 0;
            // 
            // txtShowResponse
            // 
            this.txtShowResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtShowResponse.Location = new System.Drawing.Point(0, 0);
            this.txtShowResponse.Multiline = true;
            this.txtShowResponse.Name = "txtShowResponse";
            this.txtShowResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtShowResponse.Size = new System.Drawing.Size(420, 406);
            this.txtShowResponse.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labCallInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(958, 41);
            this.panel1.TabIndex = 0;
            // 
            // labCallInfo
            // 
            this.labCallInfo.AutoSize = true;
            this.labCallInfo.Location = new System.Drawing.Point(219, 26);
            this.labCallInfo.Name = "labCallInfo";
            this.labCallInfo.Size = new System.Drawing.Size(33, 12);
            this.labCallInfo.TabIndex = 0;
            this.labCallInfo.Text = "label2";
            // 
            // TimerGetServerInfo
            // 
            this.TimerGetServerInfo.Enabled = true;
            this.TimerGetServerInfo.Interval = 1000;
            this.TimerGetServerInfo.Tick += new System.EventHandler(this.TimerGetServerInfo_Tick);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.listSFCLend);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(499, 343);
            this.tabPage7.TabIndex = 2;
            this.tabPage7.Text = "SFCDB_Lend";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.listAPLend);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(499, 343);
            this.tabPage8.TabIndex = 3;
            this.tabPage8.Text = "APDB_Lend";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // listSFCLend
            // 
            this.listSFCLend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listSFCLend.FormattingEnabled = true;
            this.listSFCLend.ItemHeight = 12;
            this.listSFCLend.Location = new System.Drawing.Point(3, 3);
            this.listSFCLend.Name = "listSFCLend";
            this.listSFCLend.Size = new System.Drawing.Size(493, 337);
            this.listSFCLend.TabIndex = 1;
            // 
            // listAPLend
            // 
            this.listAPLend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listAPLend.FormattingEnabled = true;
            this.listAPLend.ItemHeight = 12;
            this.listAPLend.Location = new System.Drawing.Point(3, 3);
            this.listAPLend.Name = "listAPLend";
            this.listAPLend.Size = new System.Drawing.Size(493, 337);
            this.listAPLend.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 500);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormMain";
            this.Text = "ClondMESServer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.GroupSession.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox GroupSession;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSelect;
        private System.Windows.Forms.Button btnKillSession;
        private System.Windows.Forms.TextBox txtKillUserSession;
        private System.Windows.Forms.TextBox txtKillIPSession;
        private System.Windows.Forms.Button btnKillUserSession;
        private System.Windows.Forms.Button btnKillIPSession;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox txtShowCall;
        private System.Windows.Forms.TextBox txtShowResponse;
        private System.Windows.Forms.Label labServerInfo;
        private System.Windows.Forms.Timer TimerGetServerInfo;
        private System.Windows.Forms.Label labCallInfo;
        private System.Windows.Forms.Button btnGCStart;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox ListFunctionNoRet;
        private System.Windows.Forms.Button btnCleanDBPool;
        private System.Windows.Forms.Button btnRegSave;
        private System.Windows.Forms.Label labAPDB;
        private System.Windows.Forms.TextBox txtAPDB;
        private System.Windows.Forms.Label labSFCDB;
        private System.Windows.Forms.TextBox txtSFCDB;
        private System.Windows.Forms.Label labPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWeb_Download_Path;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCertificatekey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCertificateFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSslProtocols;
        private System.Windows.Forms.CheckBox cbxUseSSL;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.ListBox ListFunctionNoRetAP;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.ListBox listSFCLend;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.ListBox listAPLend;
    }
}

