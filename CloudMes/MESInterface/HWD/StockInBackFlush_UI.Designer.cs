namespace MESInterface.HWD
{
    partial class StockInBackFlush_UI
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.DGVPostData = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.DGVMessage = new System.Windows.Forms.DataGridView();
            this.btnGetData = new System.Windows.Forms.Button();
            this.btnCallRFC = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVPostData)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(673, 151);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.tabPage1.Controls.Add(this.DGVPostData);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(665, 125);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // DGVPostData
            // 
            this.DGVPostData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVPostData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGVPostData.Location = new System.Drawing.Point(3, 3);
            this.DGVPostData.Name = "DGVPostData";
            this.DGVPostData.RowTemplate.Height = 24;
            this.DGVPostData.Size = new System.Drawing.Size(659, 119);
            this.DGVPostData.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.tabPage2.Controls.Add(this.DGVMessage);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(451, 125);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            // 
            // DGVMessage
            // 
            this.DGVMessage.AllowUserToOrderColumns = true;
            this.DGVMessage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGVMessage.Location = new System.Drawing.Point(3, 3);
            this.DGVMessage.Name = "DGVMessage";
            this.DGVMessage.RowTemplate.Height = 24;
            this.DGVMessage.Size = new System.Drawing.Size(445, 119);
            this.DGVMessage.TabIndex = 0;
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(10, 169);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(75, 23);
            this.btnGetData.TabIndex = 1;
            this.btnGetData.Text = "收集數據";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // btnCallRFC
            // 
            this.btnCallRFC.Location = new System.Drawing.Point(109, 169);
            this.btnCallRFC.Name = "btnCallRFC";
            this.btnCallRFC.Size = new System.Drawing.Size(75, 23);
            this.btnCallRFC.TabIndex = 2;
            this.btnCallRFC.Text = "CallRFC";
            this.btnCallRFC.UseVisualStyleBackColor = true;
            this.btnCallRFC.Click += new System.EventHandler(this.btnCallRFC_Click);
            // 
            // StockInBackFlush_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCallRFC);
            this.Controls.Add(this.btnGetData);
            this.Controls.Add(this.tabControl1);
            this.Name = "StockInBackFlush_UI";
            this.Size = new System.Drawing.Size(690, 219);
            this.Load += new System.EventHandler(this.StockInBackFlush_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGVPostData)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGVMessage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView DGVPostData;
        private System.Windows.Forms.DataGridView DGVMessage;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.Button btnCallRFC;
    }
}