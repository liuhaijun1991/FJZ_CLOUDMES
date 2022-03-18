namespace MESInterface.ORACLE
{
    partial class DownloadWO_UI
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
            this.lbConfigWO = new System.Windows.Forms.Label();
            this.dateTimePickerWO = new System.Windows.Forms.DateTimePicker();
            this.btnDownLoadWO = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbConfigWO
            // 
            this.lbConfigWO.AutoSize = true;
            this.lbConfigWO.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbConfigWO.Location = new System.Drawing.Point(35, 26);
            this.lbConfigWO.Name = "lbConfigWO";
            this.lbConfigWO.Size = new System.Drawing.Size(41, 13);
            this.lbConfigWO.TabIndex = 0;
            this.lbConfigWO.Text = "label1";
            // 
            // dateTimePickerWO
            // 
            this.dateTimePickerWO.Location = new System.Drawing.Point(38, 128);
            this.dateTimePickerWO.Name = "dateTimePickerWO";
            this.dateTimePickerWO.Size = new System.Drawing.Size(153, 22);
            this.dateTimePickerWO.TabIndex = 1;
            // 
            // btnDownLoadWO
            // 
            this.btnDownLoadWO.Location = new System.Drawing.Point(247, 128);
            this.btnDownLoadWO.Name = "btnDownLoadWO";
            this.btnDownLoadWO.Size = new System.Drawing.Size(75, 23);
            this.btnDownLoadWO.TabIndex = 2;
            this.btnDownLoadWO.Text = "DownLoadWO";
            this.btnDownLoadWO.UseVisualStyleBackColor = true;
            this.btnDownLoadWO.Click += new System.EventHandler(this.btnDownLoadWO_Click);
            // 
            // DownloadWO_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 261);
            this.Name = "DownloadWO_UI";
            this.Text = "DownloadWO_UI";
            this.Controls.Add(this.btnDownLoadWO);
            this.Controls.Add(this.dateTimePickerWO);
            this.Controls.Add(this.lbConfigWO);
            this.Size = new System.Drawing.Size(599, 392);
            this.Load += new System.EventHandler(this.DownLoadWO_UI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();


        }

        #endregion
        private System.Windows.Forms.Label lbConfigWO;
        private System.Windows.Forms.DateTimePicker dateTimePickerWO;
        private System.Windows.Forms.Button btnDownLoadWO;
    }
}