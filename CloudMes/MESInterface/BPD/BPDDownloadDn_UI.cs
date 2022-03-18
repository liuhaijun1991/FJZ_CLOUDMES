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
    public partial class BPDDownloadDn_UI : UserControl
    {
        BPDDownloadDn obj_down_dn = null;
        public BPDDownloadDn_UI(BPDDownloadDn obj_down)
        {
            InitializeComponent();
            obj_down_dn = obj_down;
        }

        private void DownDN_UI_Load(object sender, EventArgs e)
        {
            label1.Text = "BU:" + obj_down_dn.bu + "  ";
            label1.Text += "DB:" + obj_down_dn.dbstr + "  ";
            label1.Text += "CUST:" + obj_down_dn.cust + "  ";
            label1.Text += "Plant:" + obj_down_dn.plant + "\r\n";
            label1.Text += "configFile:" + obj_down_dn.configFile + "  ";
            label1.Text += "Section:" + obj_down_dn.Section + "\r\n";
            label1.Text += "RFC:ZRFC_SFC_NSG_0003E";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            obj_down_dn.dnBase.DnCreatDateTime = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            obj_down_dn.Start();
            obj_down_dn.dnBase.DnCreatDateTime = "";
        }
    }
}
