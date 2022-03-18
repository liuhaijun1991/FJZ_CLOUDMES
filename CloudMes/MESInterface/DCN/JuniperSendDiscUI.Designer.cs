
namespace MESInterface.DCN
{
    partial class JuniperSendDiscUI
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
            this.components = new System.ComponentModel.Container();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.labelFrom = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.rbOneDay = new System.Windows.Forms.RadioButton();
            this.rbMultipleDay = new System.Windows.Forms.RadioButton();
            this.labelTo = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.labelMsg = new System.Windows.Forms.Label();
            this.timerSend = new System.Windows.Forms.Timer(this.components);
            this.btnSaveCSV = new System.Windows.Forms.Button();
            this.gbRebulid = new System.Windows.Forms.GroupBox();
            this.btnRebulid = new System.Windows.Forms.Button();
            this.labelRebulidTo = new System.Windows.Forms.Label();
            this.dtpRebulidTo = new System.Windows.Forms.DateTimePicker();
            this.labelRebulidFrom = new System.Windows.Forms.Label();
            this.dtpRebulidFrom = new System.Windows.Forms.DateTimePicker();
            this.rbREPAIR = new System.Windows.Forms.RadioButton();
            this.rbMFG = new System.Windows.Forms.RadioButton();
            this.rbDEFECT = new System.Windows.Forms.RadioButton();
            this.rbTEST = new System.Windows.Forms.RadioButton();
            this.rbTRACE = new System.Windows.Forms.RadioButton();
            this.gbRebulid.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "YYYY/MM/DD HH:mm:ss";
            this.dtpFrom.Location = new System.Drawing.Point(83, 43);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(183, 20);
            this.dtpFrom.TabIndex = 0;
            // 
            // labelFrom
            // 
            this.labelFrom.AutoSize = true;
            this.labelFrom.Location = new System.Drawing.Point(43, 50);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(33, 13);
            this.labelFrom.TabIndex = 1;
            this.labelFrom.Text = "Form:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(85, 88);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 25);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // rbOneDay
            // 
            this.rbOneDay.AutoSize = true;
            this.rbOneDay.Checked = true;
            this.rbOneDay.Location = new System.Drawing.Point(44, 11);
            this.rbOneDay.Name = "rbOneDay";
            this.rbOneDay.Size = new System.Drawing.Size(89, 17);
            this.rbOneDay.TabIndex = 3;
            this.rbOneDay.TabStop = true;
            this.rbOneDay.Text = "SendOneDay";
            this.rbOneDay.UseVisualStyleBackColor = true;
            // 
            // rbMultipleDay
            // 
            this.rbMultipleDay.AutoSize = true;
            this.rbMultipleDay.Location = new System.Drawing.Point(201, 11);
            this.rbMultipleDay.Name = "rbMultipleDay";
            this.rbMultipleDay.Size = new System.Drawing.Size(105, 17);
            this.rbMultipleDay.TabIndex = 4;
            this.rbMultipleDay.TabStop = true;
            this.rbMultipleDay.Text = "SendMultipleDay";
            this.rbMultipleDay.UseVisualStyleBackColor = true;
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(289, 49);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(23, 13);
            this.labelTo.TabIndex = 6;
            this.labelTo.Text = "To:";
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "YYYY/MM/DD HH:mm:ss";
            this.dtpTo.Location = new System.Drawing.Point(324, 42);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(183, 20);
            this.dtpTo.TabIndex = 5;
            // 
            // labelMsg
            // 
            this.labelMsg.AutoSize = true;
            this.labelMsg.Location = new System.Drawing.Point(48, 129);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Size = new System.Drawing.Size(26, 13);
            this.labelMsg.TabIndex = 7;
            this.labelMsg.Text = "msg";
            // 
            // timerSend
            // 
            this.timerSend.Interval = 1000;
            // 
            // btnSaveCSV
            // 
            this.btnSaveCSV.Location = new System.Drawing.Point(191, 88);
            this.btnSaveCSV.Name = "btnSaveCSV";
            this.btnSaveCSV.Size = new System.Drawing.Size(75, 25);
            this.btnSaveCSV.TabIndex = 8;
            this.btnSaveCSV.Text = "SaveCSV";
            this.btnSaveCSV.UseVisualStyleBackColor = true;
            this.btnSaveCSV.Click += new System.EventHandler(this.btnSaveCSV_Click);
            // 
            // gbRebulid
            // 
            this.gbRebulid.Controls.Add(this.btnRebulid);
            this.gbRebulid.Controls.Add(this.labelRebulidTo);
            this.gbRebulid.Controls.Add(this.dtpRebulidTo);
            this.gbRebulid.Controls.Add(this.labelRebulidFrom);
            this.gbRebulid.Controls.Add(this.dtpRebulidFrom);
            this.gbRebulid.Controls.Add(this.rbREPAIR);
            this.gbRebulid.Controls.Add(this.rbMFG);
            this.gbRebulid.Controls.Add(this.rbDEFECT);
            this.gbRebulid.Controls.Add(this.rbTEST);
            this.gbRebulid.Controls.Add(this.rbTRACE);
            this.gbRebulid.Location = new System.Drawing.Point(45, 166);
            this.gbRebulid.Name = "gbRebulid";
            this.gbRebulid.Size = new System.Drawing.Size(485, 202);
            this.gbRebulid.TabIndex = 9;
            this.gbRebulid.TabStop = false;
            this.gbRebulid.Text = "Rebulid .gz file";
            // 
            // btnRebulid
            // 
            this.btnRebulid.Enabled = false;
            this.btnRebulid.Location = new System.Drawing.Point(18, 142);
            this.btnRebulid.Name = "btnRebulid";
            this.btnRebulid.Size = new System.Drawing.Size(75, 25);
            this.btnRebulid.TabIndex = 11;
            this.btnRebulid.Text = "Rebulid";
            this.btnRebulid.UseVisualStyleBackColor = true;
            this.btnRebulid.Visible = false;
            this.btnRebulid.Click += new System.EventHandler(this.btnRebulid_Click);
            // 
            // labelRebulidTo
            // 
            this.labelRebulidTo.AutoSize = true;
            this.labelRebulidTo.Location = new System.Drawing.Point(256, 95);
            this.labelRebulidTo.Name = "labelRebulidTo";
            this.labelRebulidTo.Size = new System.Drawing.Size(23, 13);
            this.labelRebulidTo.TabIndex = 10;
            this.labelRebulidTo.Text = "To:";
            // 
            // dtpRebulidTo
            // 
            this.dtpRebulidTo.CustomFormat = "YYYY/MM/DD HH:mm:ss";
            this.dtpRebulidTo.Location = new System.Drawing.Point(291, 89);
            this.dtpRebulidTo.Name = "dtpRebulidTo";
            this.dtpRebulidTo.Size = new System.Drawing.Size(143, 20);
            this.dtpRebulidTo.TabIndex = 9;
            // 
            // labelRebulidFrom
            // 
            this.labelRebulidFrom.AutoSize = true;
            this.labelRebulidFrom.Location = new System.Drawing.Point(16, 96);
            this.labelRebulidFrom.Name = "labelRebulidFrom";
            this.labelRebulidFrom.Size = new System.Drawing.Size(33, 13);
            this.labelRebulidFrom.TabIndex = 8;
            this.labelRebulidFrom.Text = "Form:";
            // 
            // dtpRebulidFrom
            // 
            this.dtpRebulidFrom.CustomFormat = "YYYY/MM/DD HH:mm:ss";
            this.dtpRebulidFrom.Location = new System.Drawing.Point(56, 90);
            this.dtpRebulidFrom.Name = "dtpRebulidFrom";
            this.dtpRebulidFrom.Size = new System.Drawing.Size(137, 20);
            this.dtpRebulidFrom.TabIndex = 7;
            // 
            // rbREPAIR
            // 
            this.rbREPAIR.AutoSize = true;
            this.rbREPAIR.Location = new System.Drawing.Point(398, 40);
            this.rbREPAIR.Name = "rbREPAIR";
            this.rbREPAIR.Size = new System.Drawing.Size(65, 17);
            this.rbREPAIR.TabIndex = 4;
            this.rbREPAIR.TabStop = true;
            this.rbREPAIR.Text = "REPAIR";
            this.rbREPAIR.UseVisualStyleBackColor = true;
            // 
            // rbMFG
            // 
            this.rbMFG.AutoSize = true;
            this.rbMFG.Location = new System.Drawing.Point(298, 40);
            this.rbMFG.Name = "rbMFG";
            this.rbMFG.Size = new System.Drawing.Size(48, 17);
            this.rbMFG.TabIndex = 3;
            this.rbMFG.TabStop = true;
            this.rbMFG.Text = "MFG";
            this.rbMFG.UseVisualStyleBackColor = true;
            // 
            // rbDEFECT
            // 
            this.rbDEFECT.AutoSize = true;
            this.rbDEFECT.Location = new System.Drawing.Point(199, 40);
            this.rbDEFECT.Name = "rbDEFECT";
            this.rbDEFECT.Size = new System.Drawing.Size(67, 17);
            this.rbDEFECT.TabIndex = 2;
            this.rbDEFECT.TabStop = true;
            this.rbDEFECT.Text = "DEFECT";
            this.rbDEFECT.UseVisualStyleBackColor = true;
            // 
            // rbTEST
            // 
            this.rbTEST.AutoSize = true;
            this.rbTEST.Location = new System.Drawing.Point(117, 40);
            this.rbTEST.Name = "rbTEST";
            this.rbTEST.Size = new System.Drawing.Size(53, 17);
            this.rbTEST.TabIndex = 1;
            this.rbTEST.TabStop = true;
            this.rbTEST.Text = "TEST";
            this.rbTEST.UseVisualStyleBackColor = true;
            // 
            // rbTRACE
            // 
            this.rbTRACE.AutoSize = true;
            this.rbTRACE.Checked = true;
            this.rbTRACE.Location = new System.Drawing.Point(16, 40);
            this.rbTRACE.Name = "rbTRACE";
            this.rbTRACE.Size = new System.Drawing.Size(61, 17);
            this.rbTRACE.TabIndex = 0;
            this.rbTRACE.TabStop = true;
            this.rbTRACE.Text = "TRACE";
            this.rbTRACE.UseVisualStyleBackColor = true;
            // 
            // JuniperSendDiscUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbRebulid);
            this.Controls.Add(this.btnSaveCSV);
            this.Controls.Add(this.labelMsg);
            this.Controls.Add(this.labelTo);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.rbMultipleDay);
            this.Controls.Add(this.rbOneDay);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.labelFrom);
            this.Controls.Add(this.dtpFrom);
            this.Name = "JuniperSendDiscUI";
            this.Size = new System.Drawing.Size(800, 488);
            this.gbRebulid.ResumeLayout(false);
            this.gbRebulid.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label labelFrom;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RadioButton rbOneDay;
        private System.Windows.Forms.RadioButton rbMultipleDay;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label labelMsg;
        private System.Windows.Forms.Timer timerSend;
        private System.Windows.Forms.Button btnSaveCSV;
        private System.Windows.Forms.GroupBox gbRebulid;
        private System.Windows.Forms.Button btnRebulid;
        private System.Windows.Forms.Label labelRebulidTo;
        private System.Windows.Forms.DateTimePicker dtpRebulidTo;
        private System.Windows.Forms.Label labelRebulidFrom;
        private System.Windows.Forms.DateTimePicker dtpRebulidFrom;
        private System.Windows.Forms.RadioButton rbREPAIR;
        private System.Windows.Forms.RadioButton rbMFG;
        private System.Windows.Forms.RadioButton rbDEFECT;
        private System.Windows.Forms.RadioButton rbTEST;
        private System.Windows.Forms.RadioButton rbTRACE;
    }
}