namespace MESInterface.HWD
{
    partial class Analyse_7B5XML_UI
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
            this.labelMessage = new System.Windows.Forms.Label();
            this.labelReturnMsg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelFtp = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSavePath = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelBackupPath = new System.Windows.Forms.Label();
            this.labelErrorPath = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelSAPRFC = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(64, 138);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(47, 12);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "Message:";
            // 
            // labelReturnMsg
            // 
            this.labelReturnMsg.Location = new System.Drawing.Point(124, 138);
            this.labelReturnMsg.Name = "labelReturnMsg";
            this.labelReturnMsg.Size = new System.Drawing.Size(603, 131);
            this.labelReturnMsg.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(90, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "FTP:";
            // 
            // labelFtp
            // 
            this.labelFtp.AutoSize = true;
            this.labelFtp.Location = new System.Drawing.Point(125, 33);
            this.labelFtp.Name = "labelFtp";
            this.labelFtp.Size = new System.Drawing.Size(42, 12);
            this.labelFtp.TabIndex = 3;
            this.labelFtp.Text = "ftp路徑";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(64, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Save Path:";
            // 
            // labelSavePath
            // 
            this.labelSavePath.AutoSize = true;
            this.labelSavePath.Location = new System.Drawing.Point(124, 54);
            this.labelSavePath.Name = "labelSavePath";
            this.labelSavePath.Size = new System.Drawing.Size(190, 12);
            this.labelSavePath.TabIndex = 5;
            this.labelSavePath.Text = "XML 從ftp上下載下來后保存的路徑";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Backup Path:";
            // 
            // labelBackupPath
            // 
            this.labelBackupPath.AutoSize = true;
            this.labelBackupPath.Location = new System.Drawing.Point(125, 74);
            this.labelBackupPath.Name = "labelBackupPath";
            this.labelBackupPath.Size = new System.Drawing.Size(126, 12);
            this.labelBackupPath.TabIndex = 7;
            this.labelBackupPath.Text = "XML解析后的備份路徑";
            // 
            // labelErrorPath
            // 
            this.labelErrorPath.AutoSize = true;
            this.labelErrorPath.Location = new System.Drawing.Point(125, 95);
            this.labelErrorPath.Name = "labelErrorPath";
            this.labelErrorPath.Size = new System.Drawing.Size(150, 12);
            this.labelErrorPath.TabIndex = 9;
            this.labelErrorPath.Text = "解析異常的XML的存放路徑";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(63, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "SAP RFC:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(61, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "Error Path:";
            // 
            // labelSAPRFC
            // 
            this.labelSAPRFC.AutoSize = true;
            this.labelSAPRFC.Location = new System.Drawing.Point(124, 113);
            this.labelSAPRFC.Name = "labelSAPRFC";
            this.labelSAPRFC.Size = new System.Drawing.Size(108, 12);
            this.labelSAPRFC.TabIndex = 11;
            this.labelSAPRFC.Text = "Call SAP 調用的RFC";
            // 
            // Analyse_7B5XML_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelSAPRFC);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelErrorPath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelBackupPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelSavePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelFtp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelReturnMsg);
            this.Controls.Add(this.labelMessage);
            this.Name = "Analyse_7B5XML_UI";
            this.Size = new System.Drawing.Size(800, 321);
            this.Load += new System.EventHandler(this.Analyse_7B5XML_UI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label labelReturnMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelFtp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSavePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelBackupPath;
        private System.Windows.Forms.Label labelErrorPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelSAPRFC;
    }
}