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
    public partial class MrbBackFlush_UI : UserControl
    {
        MrbBackFlush mrbBackFlush = null;
        public MrbBackFlush_UI(MrbBackFlush obj)
        {
            InitializeComponent();
            mrbBackFlush = obj;
        }

        private void MrbBackFlush_UI_Load(object sender, EventArgs e)
        {
            labelMbrBackFlush.Text = "BU:" + mrbBackFlush.BU + "  ";
            labelMbrBackFlush.Text += "DB:" + mrbBackFlush.DB + "  ";
            labelMbrBackFlush.Text += "Plant:" + mrbBackFlush.Plant + "\r\n\r\n";
            labelMbrBackFlush.Text += "ConfigFile:" + mrbBackFlush.configFile + "  ";
            labelMbrBackFlush.Text += "Section:" + mrbBackFlush.Section + "\r\n\r\n";
            labelMbrBackFlush.Text += "RFC:ZRFC_SFC_NSG_0020";
        }
    }
}
