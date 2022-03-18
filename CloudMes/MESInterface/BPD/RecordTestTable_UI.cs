using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.BPD
{
    public partial class RecordTestTable_UI : UserControl
    {
        RecordTestTable obj_record_test = null;
        public RecordTestTable_UI(RecordTestTable rtt)
        {
            InitializeComponent();
            this.obj_record_test = rtt;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            obj_record_test.Start();
        }
    }
}
