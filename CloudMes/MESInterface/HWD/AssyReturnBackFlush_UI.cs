using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.HWD
{
    public partial class AssyReturnBackFlush_UI : UserControl
    {
        AssyReturnBackFlush assyReturnBackFlush = null;
        public AssyReturnBackFlush_UI(AssyReturnBackFlush objectReturn)
        {
            InitializeComponent();
            assyReturnBackFlush = objectReturn;
        }

        private void AssyReturnBackFlush_UI_Load(object sender, EventArgs e)
        {
            labelMbrBackFlush.Text = "BU:" + assyReturnBackFlush.BU + "  ";
            labelMbrBackFlush.Text += "DB:" + assyReturnBackFlush.DB + "  ";
            labelMbrBackFlush.Text += "Plant:" + assyReturnBackFlush.Plant + "\r\n\r\n";
            labelMbrBackFlush.Text += "ConfigFile:" + assyReturnBackFlush.configFile + "  ";
            labelMbrBackFlush.Text += "Section:" + assyReturnBackFlush.Section + "\r\n\r\n";
            labelMbrBackFlush.Text += "RFC:ZCPP_NSBG_0091";
        }
    }
}
