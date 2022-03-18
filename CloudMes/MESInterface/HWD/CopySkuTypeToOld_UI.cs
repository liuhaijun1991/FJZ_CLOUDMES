using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.HWD
{
    public partial class CopySkuTypeToOld_UI : UserControl
    {
        CopySkuTypeToOld obj = null;
        public CopySkuTypeToOld_UI(CopySkuTypeToOld copySku)
        {
            InitializeComponent();
            obj = copySku;
        }
        private void CopySkuTypeToOld_UI_Load(object sender, EventArgs e)
        {
            this.label2.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                output("Start copy");
                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = 100;
                this.progressBar1.Step = 1;
                timer1.Enabled = true;                
                obj.updateDate = this.dateTimePicker1.Value.ToString("yyyy-MM-dd");
                obj.Start();
                obj.updateDate = "";
                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void output(string log)
        {
            label2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss" )+"  " + log;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.progressBar1.Value < this.progressBar1.Maximum)
            {
                progressBar1.Value++;
                this.progressBar1.PerformStep();
                output("Ongoing " + (Convert.ToDouble(progressBar1.Value) / Convert.ToDouble(progressBar1.Maximum) * 100).ToString() + "%");
            }
            else
            {
                this.progressBar1.Value = this.progressBar1.Maximum;
                output("Copy end");
                timer1.Enabled = false;                
            }
        }
    }

}
