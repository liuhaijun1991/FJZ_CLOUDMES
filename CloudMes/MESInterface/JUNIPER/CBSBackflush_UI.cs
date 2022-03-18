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
    public partial class CBSBackflush_UI : UserControl
    {
        CBSBackflush I;
        public CBSBackflush_UI( )
        {
            InitializeComponent();
        }
        public CBSBackflush_UI(CBSBackflush bk)
        {
            InitializeComponent();
            I = bk;
        }

        private void CBSBackflush_UI_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            I.Start();
        }
    }
}
