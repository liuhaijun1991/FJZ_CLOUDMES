namespace MESInterface.DCN
{
    partial class BroadComMds_UI
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
            this.dateTimePickerWO = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // dateTimePickerWO
            // 
            this.dateTimePickerWO.Location = new System.Drawing.Point(35, 78);
            this.dateTimePickerWO.Name = "dateTimePickerWO";
            this.dateTimePickerWO.Size = new System.Drawing.Size(153, 22);
            this.dateTimePickerWO.TabIndex = 2;
            // 
            // BroadComMds_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dateTimePickerWO);
            this.Name = "BroadComMds_UI";
            this.Size = new System.Drawing.Size(626, 332);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerWO;
    }
}
