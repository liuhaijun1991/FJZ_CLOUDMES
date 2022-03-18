using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.JUNIPER
{
    public partial class MakePcbaWipReport_UI : UserControl
    {
        MakePcbaWipReport report;
        public MakePcbaWipReport_UI(MakePcbaWipReport obj)
        {
            report = obj;
            InitializeComponent();
        }
        CreateWO _CreateWO;
        //public CreateWO_UI(CreateWO obj_CreateWO)
        //{
        //    InitializeComponent();
        //    this.Dock = DockStyle.Fill;
        //    _CreateWO = obj_CreateWO;
        //    //txt_log.Text = _CreateWO.RES;
        //}
        private void MakePcbaWipReport_UI_Load(object sender, EventArgs e)
        {

        }
    }
}
