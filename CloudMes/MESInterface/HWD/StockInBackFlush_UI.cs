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
    public partial class StockInBackFlush_UI : UserControl
    {
        StockInBackFlush stockInBackFlush = null;
        static System.Windows.Forms.DataGridView outPutDataGridView;
        public StockInBackFlush_UI(StockInBackFlush obj)
        {
            InitializeComponent();
            stockInBackFlush = obj;
        }

        private void StockInBackFlush_Load(object sender, EventArgs e)
        {
            this.DGVMessage.Rows.Clear();
            DGVMessage.Columns.Clear();
            DGVMessage.Columns.Add("INPUT", "輸入");
            DGVMessage.Columns.Add("STATUS", "狀態");
            DGVMessage.Columns.Add("OUTPUT", "OUTPUT");
            this.btnCallRFC.Enabled = false;
            this.btnGetData.Enabled = true;
            outPutDataGridView = DGVMessage;
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                this.DGVMessage.Rows.Clear();
                this.btnCallRFC.Enabled = false;
                stockInBackFlush.GetBackFlustData();
                this.DGVPostData.DataSource = stockInBackFlush.stockInTable;
                this.btnCallRFC.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCallRFC_Click(object sender, EventArgs e)
        {
            try
            {
                stockInBackFlush.CallRFCBackFlush();
                this.btnCallRFC.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void OutPutMessage(string input,string output,bool status)
        {
            outPutDataGridView.Rows.Insert(0, 1);
            outPutDataGridView.Rows[0].Cells[0].Value = input;
            outPutDataGridView.Rows[0].Cells[2].Value = output;
            if (status)
            {
                outPutDataGridView.Rows[0].Cells[1].Value = "PASS";
                outPutDataGridView.Rows[0].DefaultCellStyle.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                outPutDataGridView.Rows[0].Cells[1].Value = "FAIL";
                outPutDataGridView.Rows[0].DefaultCellStyle.BackColor = System.Drawing.Color.Brown;
            }
        }
    }
}
