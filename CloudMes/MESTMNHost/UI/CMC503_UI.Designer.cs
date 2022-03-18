namespace MESCMCHost.UI
{
    partial class CMC503_UI
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labUser = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.labStation = new System.Windows.Forms.Label();
            this.labLine = new System.Windows.Forms.Label();
            this.labIP = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labConnectState = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labUser
            // 
            this.labUser.AutoSize = true;
            this.labUser.Location = new System.Drawing.Point(126, 54);
            this.labUser.Name = "labUser";
            this.labUser.Size = new System.Drawing.Size(26, 12);
            this.labUser.TabIndex = 9;
            this.labUser.Text = "User";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(11, 54);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Visible = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // labStation
            // 
            this.labStation.AutoSize = true;
            this.labStation.Location = new System.Drawing.Point(96, 13);
            this.labStation.Name = "labStation";
            this.labStation.Size = new System.Drawing.Size(37, 12);
            this.labStation.TabIndex = 7;
            this.labStation.Text = "Station";
            // 
            // labLine
            // 
            this.labLine.AutoSize = true;
            this.labLine.Location = new System.Drawing.Point(126, 66);
            this.labLine.Name = "labLine";
            this.labLine.Size = new System.Drawing.Size(26, 12);
            this.labLine.TabIndex = 6;
            this.labLine.Text = "Line";
            // 
            // labIP
            // 
            this.labIP.AutoSize = true;
            this.labIP.Location = new System.Drawing.Point(14, 13);
            this.labIP.Name = "labIP";
            this.labIP.Size = new System.Drawing.Size(38, 12);
            this.labIP.TabIndex = 5;
            this.labIP.Text = "0.0.0.0";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labConnectState
            // 
            this.labConnectState.AutoSize = true;
            this.labConnectState.Location = new System.Drawing.Point(16, 36);
            this.labConnectState.Name = "labConnectState";
            this.labConnectState.Size = new System.Drawing.Size(33, 12);
            this.labConnectState.TabIndex = 10;
            this.labConnectState.Text = "label1";
            // 
            // CMC503_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labConnectState);
            this.Controls.Add(this.labUser);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.labStation);
            this.Controls.Add(this.labLine);
            this.Controls.Add(this.labIP);
            this.Name = "CMC503_UI";
            this.Size = new System.Drawing.Size(234, 84);
            this.Load += new System.EventHandler(this.CMC503_UI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label labUser;
        public System.Windows.Forms.Button btnStart;
        public System.Windows.Forms.Label labStation;
        public System.Windows.Forms.Label labLine;
        public System.Windows.Forms.Label labIP;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labConnectState;
    }
}
