namespace MESMailCenter
{
    /// <summary>
    /// 
    /// </summary>
    partial class FrmComfig
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。

        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonDelConnString = new System.Windows.Forms.Button();
            this.buttonAddConnString = new System.Windows.Forms.Button();
            this.buttonLoadConn = new System.Windows.Forms.Button();
            this.buttonUNDoSaveConn = new System.Windows.Forms.Button();
            this.buttonSaveConn = new System.Windows.Forms.Button();
            this.GViewConns = new System.Windows.Forms.DataGridView();
            this.chkIsCrypt = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtConnString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtConnName = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonTaskTest = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonLogTest = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GViewConns)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(686, 435);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonDelConnString);
            this.tabPage1.Controls.Add(this.buttonAddConnString);
            this.tabPage1.Controls.Add(this.buttonLoadConn);
            this.tabPage1.Controls.Add(this.buttonUNDoSaveConn);
            this.tabPage1.Controls.Add(this.buttonSaveConn);
            this.tabPage1.Controls.Add(this.GViewConns);
            this.tabPage1.Controls.Add(this.chkIsCrypt);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtConnString);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtConnName);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(678, 410);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "數據庫連接配置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonDelConnString
            // 
            this.buttonDelConnString.Location = new System.Drawing.Point(536, 19);
            this.buttonDelConnString.Name = "buttonDelConnString";
            this.buttonDelConnString.Size = new System.Drawing.Size(75, 23);
            this.buttonDelConnString.TabIndex = 8;
            this.buttonDelConnString.Text = "刪除";
            this.buttonDelConnString.UseVisualStyleBackColor = true;
            this.buttonDelConnString.Click += new System.EventHandler(this.buttonDelConnString_Click);
            // 
            // buttonAddConnString
            // 
            this.buttonAddConnString.Location = new System.Drawing.Point(442, 19);
            this.buttonAddConnString.Name = "buttonAddConnString";
            this.buttonAddConnString.Size = new System.Drawing.Size(75, 23);
            this.buttonAddConnString.TabIndex = 7;
            this.buttonAddConnString.Text = "添加";
            this.buttonAddConnString.UseVisualStyleBackColor = true;
            this.buttonAddConnString.Click += new System.EventHandler(this.buttonAddConnString_Click);
            // 
            // buttonLoadConn
            // 
            this.buttonLoadConn.Location = new System.Drawing.Point(14, 379);
            this.buttonLoadConn.Name = "buttonLoadConn";
            this.buttonLoadConn.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadConn.TabIndex = 6;
            this.buttonLoadConn.Text = "加載";
            this.buttonLoadConn.UseVisualStyleBackColor = true;
            this.buttonLoadConn.Click += new System.EventHandler(this.buttonLoadConn_Click);
            // 
            // buttonUNDoSaveConn
            // 
            this.buttonUNDoSaveConn.Location = new System.Drawing.Point(562, 379);
            this.buttonUNDoSaveConn.Name = "buttonUNDoSaveConn";
            this.buttonUNDoSaveConn.Size = new System.Drawing.Size(75, 23);
            this.buttonUNDoSaveConn.TabIndex = 5;
            this.buttonUNDoSaveConn.Text = "取消";
            this.buttonUNDoSaveConn.UseVisualStyleBackColor = true;
            // 
            // buttonSaveConn
            // 
            this.buttonSaveConn.Location = new System.Drawing.Point(461, 379);
            this.buttonSaveConn.Name = "buttonSaveConn";
            this.buttonSaveConn.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveConn.TabIndex = 4;
            this.buttonSaveConn.Text = "保存";
            this.buttonSaveConn.UseVisualStyleBackColor = true;
            this.buttonSaveConn.Click += new System.EventHandler(this.buttonSaveConn_Click);
            // 
            // GViewConns
            // 
            this.GViewConns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GViewConns.Location = new System.Drawing.Point(14, 119);
            this.GViewConns.Name = "GViewConns";
            this.GViewConns.RowTemplate.Height = 24;
            this.GViewConns.Size = new System.Drawing.Size(632, 236);
            this.GViewConns.TabIndex = 3;
            this.GViewConns.RowDividerDoubleClick += new System.Windows.Forms.DataGridViewRowDividerDoubleClickEventHandler(this.GViewConns_RowDividerDoubleClick);
            this.GViewConns.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.GViewConns_RowEnter);
            this.GViewConns.Click += new System.EventHandler(this.GViewConns_Click);
            this.GViewConns.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GViewConns_CellContentClick);
            // 
            // chkIsCrypt
            // 
            this.chkIsCrypt.AutoSize = true;
            this.chkIsCrypt.Location = new System.Drawing.Point(255, 24);
            this.chkIsCrypt.Name = "chkIsCrypt";
            this.chkIsCrypt.Size = new System.Drawing.Size(72, 16);
            this.chkIsCrypt.TabIndex = 2;
            this.chkIsCrypt.Text = "是否加密";
            this.chkIsCrypt.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "連接字符串";
            this.label2.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtConnString
            // 
            this.txtConnString.Location = new System.Drawing.Point(83, 61);
            this.txtConnString.Name = "txtConnString";
            this.txtConnString.Size = new System.Drawing.Size(563, 22);
            this.txtConnString.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "連接名稱";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtConnName
            // 
            this.txtConnName.Location = new System.Drawing.Point(83, 21);
            this.txtConnName.Name = "txtConnName";
            this.txtConnName.Size = new System.Drawing.Size(124, 22);
            this.txtConnName.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonTaskTest);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(678, 410);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "排程設置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonTaskTest
            // 
            this.buttonTaskTest.Location = new System.Drawing.Point(8, 24);
            this.buttonTaskTest.Name = "buttonTaskTest";
            this.buttonTaskTest.Size = new System.Drawing.Size(75, 23);
            this.buttonTaskTest.TabIndex = 0;
            this.buttonTaskTest.Text = "排程測試";
            this.buttonTaskTest.UseVisualStyleBackColor = true;
            this.buttonTaskTest.Click += new System.EventHandler(this.buttonTaskTest_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.buttonLogTest);
            this.tabPage3.Location = new System.Drawing.Point(4, 21);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(678, 410);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "日誌設置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonLogTest
            // 
            this.buttonLogTest.Location = new System.Drawing.Point(8, 32);
            this.buttonLogTest.Name = "buttonLogTest";
            this.buttonLogTest.Size = new System.Drawing.Size(75, 23);
            this.buttonLogTest.TabIndex = 0;
            this.buttonLogTest.Text = "寫日誌";
            this.buttonLogTest.UseVisualStyleBackColor = true;
            this.buttonLogTest.Click += new System.EventHandler(this.buttonLogTest_Click);
            // 
            // FrmComfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 435);
            this.Controls.Add(this.tabControl1);
            this.Name = "FrmComfig";
            this.Text = "基本配置";
            this.Load += new System.EventHandler(this.FrmComfig_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GViewConns)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtConnName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtConnString;
        private System.Windows.Forms.Button buttonUNDoSaveConn;
        private System.Windows.Forms.Button buttonSaveConn;
        private System.Windows.Forms.DataGridView GViewConns;
        private System.Windows.Forms.CheckBox chkIsCrypt;
        private System.Windows.Forms.Button buttonLoadConn;
        private System.Windows.Forms.Button buttonDelConnString;
        private System.Windows.Forms.Button buttonAddConnString;
        private System.Windows.Forms.Button buttonTaskTest;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonLogTest;
    }
}

