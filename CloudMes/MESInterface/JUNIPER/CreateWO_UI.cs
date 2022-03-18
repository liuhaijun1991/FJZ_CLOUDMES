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
    public partial class CreateWO_UI : UserControl
    {
        CreateWO _CreateWO;
        public CreateWO_UI(CreateWO obj_CreateWO)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            _CreateWO = obj_CreateWO;
            //txt_log.Text = _CreateWO.RES;
        }

    }
}
