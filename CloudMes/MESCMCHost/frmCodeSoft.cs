using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESCMCHost
{
    public partial class frmCodeSoft : Form
    {
        public frmCodeSoft()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Printer_Codesoft p = new Printer_Codesoft(@"C:\D1.lab");
            p.Print();
            p.close();
        }
    }
}
