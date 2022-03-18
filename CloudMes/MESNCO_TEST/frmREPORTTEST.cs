using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MESDBHelper;
using MESReport;
using MESReport.BaseReport;


namespace MESNCO_TEST
{
    public partial class frmREPORTTEST : Form
    {
        public frmREPORTTEST()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SkuWoReportNew r = new SkuWoReportNew();
            r.DBPools = new Dictionary<string, OleExecPool>();
            OleExecPool p = new OleExecPool("Data Source = 10.120.246.160:1527 / VNMESODB; User ID = TEST; Password = SFCTEST");
            p.MaxPoolSize = 1;
            p.MinPoolSize = 1;
            
            r.DBPools.Add("SFCDB", p);

            r.DownFile();

            ReportFile f = (ReportFile)r.Outputs.Find(t => t.GetType() == typeof(ReportFile));
            if (f != null)
            {
                var b64 = f.FileContent.ToString();
                var bytes = System.Convert.FromBase64String(b64);
                
                System.IO.FileStream fs = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory+"\\" + f.FileName, System.IO.FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

            }

        }
    }
}
