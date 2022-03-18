using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.JUNIPER
{
    public partial class I054AckProcess_UI : UserControl
    {
        I054AckProcess i054;
        public I054AckProcess_UI(I054AckProcess _i054)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            i054 = _i054;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
