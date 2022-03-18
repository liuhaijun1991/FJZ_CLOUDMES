using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.PublicBackflush
{
    public partial class SemifinishedBackflush_UI : UserControl
    {
        public SemifinishedBackflush_UI()
        {
            InitializeComponent();
        }

        SemifinishedBackflush semifinishedBackflush = null;
        public SemifinishedBackflush_UI(SemifinishedBackflush objectReturn)
        {
            InitializeComponent();
            semifinishedBackflush = objectReturn;
        }

        private void SemifinishedBackflush_UI_Load(object sender, EventArgs e)
        {
            labelMsg.Text = "BU:" + semifinishedBackflush.BU + "  ";
            labelMsg.Text += "DB:" + semifinishedBackflush.DB + "  ";
            labelMsg.Text += "FROMSTORAGE:" + semifinishedBackflush.fromStorage + "  ";
            labelMsg.Text += "TOSTORAGE:" + semifinishedBackflush.toStorage + "  ";
            labelMsg.Text += "Plant:" + semifinishedBackflush.Plant + "\r\n\r\n";
            labelMsg.Text += "FROMSTORAGE:" + semifinishedBackflush.fromStorage + "  ";
            labelMsg.Text += "TOSTORAGE:" + semifinishedBackflush.toStorage + "\r\n\r\n";
            labelMsg.Text += "ConfigFile:" + semifinishedBackflush.configFile + "  ";
            labelMsg.Text += "Section:" + semifinishedBackflush.Section + "\r\n\r\n";
            labelMsg.Text += "RFC:ZRFC_SFC_NSG_0011";
        }
    }
}
