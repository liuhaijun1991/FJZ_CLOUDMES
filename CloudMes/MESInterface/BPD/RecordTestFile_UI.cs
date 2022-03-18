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
    public partial class RecordTestFile_UI : UserControl
    {
        RecordTestFile obj_record_test = null;
        public RecordTestFile_UI(RecordTestFile rtf)
        {
            InitializeComponent();
            obj_record_test = rtf;
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            obj_record_test.Start();
        }
    }
}
