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
    public partial class I054CaptrueProcess_UI : UserControl
    {
        I054CaptrueProcess i054;
        public I054CaptrueProcess_UI(I054CaptrueProcess _i054)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            i054 = _i054;
        }
    }
}
