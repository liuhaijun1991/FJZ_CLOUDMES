namespace MESInterface.ORACLE
{
    partial class GetTDMSData_UI
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
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
            this.txtRes = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtRes
            // 
            this.txtRes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRes.Location = new System.Drawing.Point(0, 0);
            this.txtRes.Multiline = true;
            this.txtRes.Name = "txtRes";
            this.txtRes.Size = new System.Drawing.Size(519, 398);
            this.txtRes.TabIndex = 0;
            // 
            // FileTransfer_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtRes);
            this.Name = "FileTransfer_UI";
            this.Size = new System.Drawing.Size(519, 398);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRes;
    }
}
