namespace MESInterface.HWD
{
    partial class MrbBackFlush_UI
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
            this.labelMbrBackFlush = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelMbrBackFlush
            // 
            this.labelMbrBackFlush.AutoSize = true;
            this.labelMbrBackFlush.Location = new System.Drawing.Point(33, 39);
            this.labelMbrBackFlush.Name = "labelMbrBackFlush";
            this.labelMbrBackFlush.Size = new System.Drawing.Size(33, 12);
            this.labelMbrBackFlush.TabIndex = 0;
            this.labelMbrBackFlush.Text = "label1";
            // 
            // MrbBackFlush_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelMbrBackFlush);
            this.Name = "MrbBackFlush_UI";
            this.Size = new System.Drawing.Size(699, 190);
            this.Load += new System.EventHandler(this.MrbBackFlush_UI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMbrBackFlush;
    }
}