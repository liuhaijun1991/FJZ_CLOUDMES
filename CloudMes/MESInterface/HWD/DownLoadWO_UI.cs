using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.HWD
{
	public partial class DownLoadWO_UI : UserControl
	{
        DownLoadWO obj_down_wo = null;
		public DownLoadWO_UI(DownLoadWO obj_down)
		{
			InitializeComponent();
			obj_down_wo = obj_down;
        }

		private void DownLoadWO_UI_Load(object sender, EventArgs e)
		{
			lbConfigWO.Text = "BU:" + obj_down_wo.BU + "  ";
			lbConfigWO.Text += "DB:" + obj_down_wo.DB + "  ";
			lbConfigWO.Text += "CUST:" + obj_down_wo.CUST + "  ";
			lbConfigWO.Text += "Plant:" + obj_down_wo.Plant + "\r\n\r\n";
			lbConfigWO.Text += "ConfigFile:" + obj_down_wo.configFile + "  ";
			lbConfigWO.Text += "Section:" + obj_down_wo.Section + "\r\n\r\n";
			lbConfigWO.Text += "RFC:ZRFC_SFC_NSG_0001B";
		}
		private void btnDownLoadWO_Click(object sender, EventArgs e)
		{
            try
            {
                obj_down_wo.downLoadDate = dateTimePickerWO.Value.ToString("yyyy-MM-dd");
                obj_down_wo.Start();
                obj_down_wo.downLoadDate = "";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
		}

	}
}
