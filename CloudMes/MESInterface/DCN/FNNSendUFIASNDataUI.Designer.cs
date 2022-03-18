
namespace MESInterface.DCN
{
    partial class FNNSendUFIASNDataUI
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
            this.SuspendLayout();
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "YYYY/MM/DD HH:mm:ss";
            this.dtpFrom.Location = new System.Drawing.Point(83, 40);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(183, 22);
            this.dtpFrom.TabIndex = 0;
            // 
            // labelFrom
            // 
            this.labelFrom.AutoSize = true;
            this.labelFrom.Location = new System.Drawing.Point(43, 46);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(33, 12);
            this.labelFrom.TabIndex = 1;
            this.labelFrom.Text = "Form:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(85, 81);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // rbOneDay
            // 
            this.rbOneDay.AutoSize = true;
            this.rbOneDay.Checked = true;
            this.rbOneDay.Location = new System.Drawing.Point(44, 10);
            this.rbOneDay.Name = "rbOneDay";
            this.rbOneDay.Size = new System.Drawing.Size(84, 16);
            this.rbOneDay.TabIndex = 3;
            this.rbOneDay.TabStop = true;
            this.rbOneDay.Text = "SendOneDay";
            this.rbOneDay.UseVisualStyleBackColor = true;
            // 
            // rbMultipleDay
            // 
            this.rbMultipleDay.AutoSize = true;
            this.rbMultipleDay.Location = new System.Drawing.Point(201, 10);
            this.rbMultipleDay.Name = "rbMultipleDay";
            this.rbMultipleDay.Size = new System.Drawing.Size(104, 16);
            this.rbMultipleDay.TabIndex = 4;
            this.rbMultipleDay.TabStop = true;
            this.rbMultipleDay.Text = "SendMultipleDay";
            this.rbMultipleDay.UseVisualStyleBackColor = true;
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(289, 45);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(21, 12);
            this.labelTo.TabIndex = 6;
            this.labelTo.Text = "To:";
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "YYYY/MM/DD HH:mm:ss";
            this.dtpTo.Location = new System.Drawing.Point(324, 39);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(183, 22);
            this.dtpTo.TabIndex = 5;
            // 
            // labelMsg
            // 
            this.labelMsg.AutoSize = true;
            this.labelMsg.Location = new System.Drawing.Point(48, 119);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Size = new System.Drawing.Size(0, 12);
            this.labelMsg.TabIndex = 7;

            this.timerSend.Interval = 1000;

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelMsg);
            this.Controls.Add(this.labelTo);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.rbMultipleDay);
            this.Controls.Add(this.rbOneDay);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.labelFrom);
            this.Controls.Add(this.dtpFrom);
            this.Name = "FNNSendUFIASNDataUI";
            this.Size = new System.Drawing.Size(800, 450);
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
    }
}