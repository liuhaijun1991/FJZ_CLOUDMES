namespace MESInterface.JUNIPER
{
    partial class DownloadWO_UI
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
            this.btnDownLoadWO = new System.Windows.Forms.Button();
            this.dateTimePickerWO = new System.Windows.Forms.DateTimePicker();
            this.lbConfigWO = new System.Windows.Forms.Label();
            this.dateTimePickerWOto = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDownLoadWO
            // 
            this.btnDownLoadWO.Location = new System.Drawing.Point(316, 74);
            this.btnDownLoadWO.Name = "btnDownLoadWO";
            this.btnDownLoadWO.Size = new System.Drawing.Size(117, 23);
            this.btnDownLoadWO.TabIndex = 5;
            this.btnDownLoadWO.Text = "DownLoadWO";
            this.btnDownLoadWO.UseVisualStyleBackColor = true;
            this.btnDownLoadWO.Click += new System.EventHandler(this.btnDownLoadWO_Click);
            // 
            // dateTimePickerWO
            // 
            this.dateTimePickerWO.Location = new System.Drawing.Point(102, 74);
            this.dateTimePickerWO.Name = "dateTimePickerWO";
            this.dateTimePickerWO.Size = new System.Drawing.Size(153, 21);
            this.dateTimePickerWO.TabIndex = 4;
            // 
            // lbConfigWO
            // 
            this.lbConfigWO.AutoSize = true;
            this.lbConfigWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbConfigWO.Location = new System.Drawing.Point(40, 28);
            this.lbConfigWO.Name = "lbConfigWO";
            this.lbConfigWO.Size = new System.Drawing.Size(51, 16);
            this.lbConfigWO.TabIndex = 3;
            this.lbConfigWO.Text = "label1";
            // 
            // dateTimePickerWOto
            // 
            this.dateTimePickerWOto.Location = new System.Drawing.Point(102, 123);
            this.dateTimePickerWOto.Name = "dateTimePickerWOto";
            this.dateTimePickerWOto.Size = new System.Drawing.Size(153, 21);
            this.dateTimePickerWOto.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "form";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "to";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(316, 118);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "DL from to";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DownloadWO_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePickerWOto);
            this.Controls.Add(this.btnDownLoadWO);
            this.Controls.Add(this.dateTimePickerWO);
            this.Controls.Add(this.lbConfigWO);
            this.Name = "DownloadWO_UI";
            this.Size = new System.Drawing.Size(727, 429);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDownLoadWO;
        private System.Windows.Forms.DateTimePicker dateTimePickerWO;
        private System.Windows.Forms.Label lbConfigWO;
        private System.Windows.Forms.DateTimePicker dateTimePickerWOto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}
