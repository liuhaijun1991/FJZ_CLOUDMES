namespace MESInterface.JUNIPER
{
    partial class frmBackFlush
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
            this.dgSAPDATA = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgBACKFLUSH = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dgRET = new System.Windows.Forms.DataGridView();
            this.labBackFlushProgress = new System.Windows.Forms.Label();
            this.BackFlushProgressBar = new System.Windows.Forms.ProgressBar();
            this.btnCallRFC = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSAPDATA)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBACKFLUSH)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRET)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(765, 278);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgSAPDATA);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(757, 252);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SAPData";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgSAPDATA
            // 
            this.dgSAPDATA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgSAPDATA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSAPDATA.Location = new System.Drawing.Point(3, 3);
            this.dgSAPDATA.Name = "dgSAPDATA";
            this.dgSAPDATA.RowTemplate.Height = 24;
            this.dgSAPDATA.Size = new System.Drawing.Size(752, 246);
            this.dgSAPDATA.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgBACKFLUSH);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(757, 271);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "BACKFLUSH";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgBACKFLUSH
            // 
            this.dgBACKFLUSH.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgBACKFLUSH.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgBACKFLUSH.Location = new System.Drawing.Point(3, 3);
            this.dgBACKFLUSH.Name = "dgBACKFLUSH";
            this.dgBACKFLUSH.RowTemplate.Height = 24;
            this.dgBACKFLUSH.Size = new System.Drawing.Size(751, 262);
            this.dgBACKFLUSH.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgRET);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(745, 303);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "RET";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgRET
            // 
            this.dgRET.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgRET.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRET.Location = new System.Drawing.Point(0, 1);
            this.dgRET.Name = "dgRET";
            this.dgRET.RowTemplate.Height = 24;
            this.dgRET.Size = new System.Drawing.Size(745, 299);
            this.dgRET.TabIndex = 0;
            // 
            // labBackFlushProgress
            // 
            this.labBackFlushProgress.AutoSize = true;
            this.labBackFlushProgress.Location = new System.Drawing.Point(334, 29);
            this.labBackFlushProgress.Name = "labBackFlushProgress";
            this.labBackFlushProgress.Size = new System.Drawing.Size(20, 12);
            this.labBackFlushProgress.TabIndex = 3;
            this.labBackFlushProgress.Text = "0/0";
            // 
            // BackFlushProgressBar
            // 
            this.BackFlushProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackFlushProgressBar.Location = new System.Drawing.Point(336, 3);
            this.BackFlushProgressBar.Maximum = 500;
            this.BackFlushProgressBar.Name = "BackFlushProgressBar";
            this.BackFlushProgressBar.Size = new System.Drawing.Size(402, 23);
            this.BackFlushProgressBar.TabIndex = 2;
            // 
            // btnCallRFC
            // 
            this.btnCallRFC.Location = new System.Drawing.Point(166, 3);
            this.btnCallRFC.Name = "btnCallRFC";
            this.btnCallRFC.Size = new System.Drawing.Size(75, 23);
            this.btnCallRFC.TabIndex = 1;
            this.btnCallRFC.Text = "拋帳";
            this.btnCallRFC.UseVisualStyleBackColor = true;
            this.btnCallRFC.Click += new System.EventHandler(this.btnCallRFC_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(28, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "搜集數據";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.BackFlushProgressBar);
            this.panel1.Controls.Add(this.labBackFlushProgress);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.btnCallRFC);
            this.panel1.Location = new System.Drawing.Point(3, 285);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(766, 44);
            this.panel1.TabIndex = 4;
            // 
            // frmBackFlush
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmBackFlush";
            this.Size = new System.Drawing.Size(769, 329);
            this.Load += new System.EventHandler(this.frmBackFlush_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSAPDATA)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgBACKFLUSH)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgRET)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgSAPDATA;
        private System.Windows.Forms.DataGridView dgBACKFLUSH;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dgRET;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnCallRFC;
        private System.Windows.Forms.Label labBackFlushProgress;
        private System.Windows.Forms.ProgressBar BackFlushProgressBar;
        private System.Windows.Forms.Panel panel1;
    }
}