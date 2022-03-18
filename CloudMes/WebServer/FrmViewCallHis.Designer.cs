namespace WebServer
{
    partial class FrmViewCallHis
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupList = new System.Windows.Forms.GroupBox();
            this.listHis = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupEdit = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRecordCount = new System.Windows.Forms.TextBox();
            this.LabState = new System.Windows.Forms.Label();
            this.chkAutoRun = new System.Windows.Forms.CheckBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.request = new System.Windows.Forms.GroupBox();
            this.txtRequest = new System.Windows.Forms.TextBox();
            this.response = new System.Windows.Forms.GroupBox();
            this.txtResponse = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.request.SuspendLayout();
            this.response.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(975, 522);
            this.splitContainer1.SplitterDistance = 191;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupList
            // 
            this.groupList.Controls.Add(this.listHis);
            this.groupList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupList.Location = new System.Drawing.Point(0, 0);
            this.groupList.Name = "groupList";
            this.groupList.Size = new System.Drawing.Size(191, 522);
            this.groupList.TabIndex = 0;
            this.groupList.TabStop = false;
            this.groupList.Text = "List";
            // 
            // listHis
            // 
            this.listHis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listHis.FormattingEnabled = true;
            this.listHis.ItemHeight = 12;
            this.listHis.Location = new System.Drawing.Point(3, 18);
            this.listHis.Name = "listHis";
            this.listHis.Size = new System.Drawing.Size(185, 501);
            this.listHis.TabIndex = 0;
            this.listHis.Click += new System.EventHandler(this.listHis_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupEdit);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(780, 522);
            this.splitContainer2.SplitterDistance = 134;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupEdit
            // 
            this.groupEdit.Controls.Add(this.button1);
            this.groupEdit.Controls.Add(this.label1);
            this.groupEdit.Controls.Add(this.txtRecordCount);
            this.groupEdit.Controls.Add(this.LabState);
            this.groupEdit.Controls.Add(this.chkAutoRun);
            this.groupEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupEdit.Location = new System.Drawing.Point(0, 0);
            this.groupEdit.Name = "groupEdit";
            this.groupEdit.Size = new System.Drawing.Size(780, 134);
            this.groupEdit.TabIndex = 0;
            this.groupEdit.TabStop = false;
            this.groupEdit.Text = "操作";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(371, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(139, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "记录数量";
            // 
            // txtRecordCount
            // 
            this.txtRecordCount.Location = new System.Drawing.Point(198, 16);
            this.txtRecordCount.Name = "txtRecordCount";
            this.txtRecordCount.Size = new System.Drawing.Size(121, 22);
            this.txtRecordCount.TabIndex = 2;
            this.txtRecordCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRecordCount_KeyPress);
            // 
            // LabState
            // 
            this.LabState.AutoSize = true;
            this.LabState.Location = new System.Drawing.Point(17, 48);
            this.LabState.Name = "LabState";
            this.LabState.Size = new System.Drawing.Size(33, 12);
            this.LabState.TabIndex = 1;
            this.LabState.Text = "label1";
            // 
            // chkAutoRun
            // 
            this.chkAutoRun.AutoSize = true;
            this.chkAutoRun.Location = new System.Drawing.Point(19, 18);
            this.chkAutoRun.Name = "chkAutoRun";
            this.chkAutoRun.Size = new System.Drawing.Size(72, 16);
            this.chkAutoRun.TabIndex = 0;
            this.chkAutoRun.Text = "自动刷新";
            this.chkAutoRun.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.request);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.response);
            this.splitContainer3.Size = new System.Drawing.Size(780, 384);
            this.splitContainer3.SplitterDistance = 311;
            this.splitContainer3.TabIndex = 0;
            // 
            // request
            // 
            this.request.Controls.Add(this.txtRequest);
            this.request.Dock = System.Windows.Forms.DockStyle.Fill;
            this.request.Location = new System.Drawing.Point(0, 0);
            this.request.Name = "request";
            this.request.Size = new System.Drawing.Size(311, 384);
            this.request.TabIndex = 0;
            this.request.TabStop = false;
            this.request.Text = "请求";
            // 
            // txtRequest
            // 
            this.txtRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRequest.Location = new System.Drawing.Point(3, 18);
            this.txtRequest.Multiline = true;
            this.txtRequest.Name = "txtRequest";
            this.txtRequest.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRequest.Size = new System.Drawing.Size(305, 363);
            this.txtRequest.TabIndex = 0;
            // 
            // response
            // 
            this.response.Controls.Add(this.txtResponse);
            this.response.Dock = System.Windows.Forms.DockStyle.Fill;
            this.response.Location = new System.Drawing.Point(0, 0);
            this.response.Name = "response";
            this.response.Size = new System.Drawing.Size(465, 384);
            this.response.TabIndex = 0;
            this.response.TabStop = false;
            this.response.Text = "返回";
            // 
            // txtResponse
            // 
            this.txtResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResponse.Location = new System.Drawing.Point(3, 18);
            this.txtResponse.Multiline = true;
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResponse.Size = new System.Drawing.Size(459, 363);
            this.txtResponse.TabIndex = 1;
            // 
            // FrmViewCallHis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 522);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmViewCallHis";
            this.Text = "FrmViewCallHis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmViewCallHis_FormClosing);
            this.Load += new System.EventHandler(this.FrmViewCallHis_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupList.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupEdit.ResumeLayout(false);
            this.groupEdit.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.request.ResumeLayout(false);
            this.request.PerformLayout();
            this.response.ResumeLayout(false);
            this.response.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupList;
        private System.Windows.Forms.ListBox listHis;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupEdit;
        private System.Windows.Forms.Label LabState;
        private System.Windows.Forms.CheckBox chkAutoRun;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox request;
        private System.Windows.Forms.TextBox txtRequest;
        private System.Windows.Forms.GroupBox response;
        private System.Windows.Forms.TextBox txtResponse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRecordCount;
        private System.Windows.Forms.Button button1;
    }
}