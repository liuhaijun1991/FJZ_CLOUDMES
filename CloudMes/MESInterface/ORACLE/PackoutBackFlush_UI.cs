using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.ORACLE
{
    public partial class PackoutBackFlush_UI : UserControl
    {
        PackoutBackFlush PackoutBackFlush = null;
        public PackoutBackFlush_UI(PackoutBackFlush obj)
        {
            InitializeComponent();
            PackoutBackFlush = obj;
        }

        private void PackoutBackFlush_UI_Load(object sender, EventArgs e)
        {
            labelPackoutBackFlush.Text = "BU:" + PackoutBackFlush.BU + "  ";
            labelPackoutBackFlush.Text += "DB:" + PackoutBackFlush.DB + "  ";
            labelPackoutBackFlush.Text += "Plant:" + PackoutBackFlush.Plant + "\r\n\r\n";
            labelPackoutBackFlush.Text += "ConfigFile:" + PackoutBackFlush.configFile + "  ";
            labelPackoutBackFlush.Text += "Section:" + PackoutBackFlush.Section + "\r\n\r\n";
            labelPackoutBackFlush.Text += "RFC:ZRFC_SFC_NSGT_0002";
        }
    }
}
