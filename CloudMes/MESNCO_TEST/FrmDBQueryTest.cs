using MesReportCenter.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESNCO_TEST
{
    public partial class FrmDBQueryTest : Form
    {
        public FrmDBQueryTest()
        {
            InitializeComponent();
        }

        private void FrmDBQueryTest_Load(object sender, EventArgs e)
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            




        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //var a = new List<DBQueryPara>();
            //var b = new BindingList<DBQueryPara>();
            //b.Add(new DBQueryPara() { Column = "AAA" });
            //dataGridView1.AutoGenerateColumns = true;
            //dataGridView1.DataSource = b;
            //dataGridView1.Refresh();
            DBQuery qurey = new DBQuery();
            qurey.UseTables.Add(new DBQueryTable() { Table = DBTable.GetTestTable_R_SN(), Alias = "R_SN" });
            qurey.UseTables.Add(new DBQueryTable() { Table = DBTable.GetTestTable_R_WO_BASE(), Alias = "R_WO_BASE" });
            //qurey.UseTables.Add(new DBQueryTable() { Table = DBTable.GetTestTable_R_WO_BASE(), Alias = "R_WO_BASE1" });

            JoinType j = new JoinType();
            j.Type = "inner join";
            j.Table1 = qurey.UseTables[0].Alias;
            j.Table2 = qurey.UseTables[1].Alias;

            JoinData d = new JoinData();
            d.Col1 = "WORKORDERNO";
            d.Col2 = "WORKORDERNO";
            d.Operator = "=";

            j.Data.Add(d);
            d = new JoinData();
            d.Col1 = "PLANT";
            d.Col2 = "PLANT";
            d.Operator = "=";

            j.Data.Add(d);
            qurey.Joins.Add(j);


           

            qurey.Paras.Add(new DBQueryPara() { Column = "WORKORDERNO", Alias = "WO",
                Table = "R_WO_BASE", Output = true, Filter = "='002510053564'" , GroupBy = "Where"
            });
            qurey.Paras.Add(new DBQueryPara() { Column = "SN", Alias = "SyserialNO", Table = "R_SN", Output = true });
            qurey.Paras.Add(new DBQueryPara() { Column = "WORKORDERNO", Alias = "WORKORDERNO1", Table = "R_WO_BASE", Output = false,
                SortType = "DESC" , SortOrder = 1 });
            qurey.Paras.Add(new DBQueryPara() { Column = "SN", Alias = "SyserialNO", Table = "R_SN", Output = true , GroupBy = "COUNT"});

            var b = new BindingList<DBQueryPara>(qurey.Paras);
           
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = b;
            dataGridView1.Refresh();


            SQLServerFunction f = new SQLServerFunction();

            textBox1.Text = f.Select(qurey) + "\r\n" + 
                f.From(qurey) + "\r\n"+
                f.Where(qurey) + "\r\n" + 
                f.GroupBy(qurey) + "\r\n" + 
                f.Having(qurey) + "\r\n" + 
                f.OrderBy(qurey);
        }
    }
}
