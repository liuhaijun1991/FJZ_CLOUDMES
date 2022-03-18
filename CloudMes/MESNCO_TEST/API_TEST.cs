using MESNCO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MESDataObject.Module.DCN;

namespace MESNCO_TEST
{
    public partial class API_TEST : Form
    {
        IStationObj I = new StationObj();

        public API_TEST()
        {
            
            InitializeComponent();
        }

        private void btnCallSetConnPara_Click(object sender, EventArgs e)
        {
            txtCallSetConnParaReturn.Text = I.SetConnPara(txtMESConnStr.Text,TxtMES_USER.Text,txtMES_PWD.Text);

        }

        private void btnCallSetStation_Click(object sender, EventArgs e)
        {
            txtCallSetStationReturn.Text = I.SetStation(TxtStation.Text, txtLine.Text);
            if (txtCallSetStationReturn.Text == "OK")
            {
                txtCallSetStationObj.Text = I.GetCurrStationObj();
            }
            else
            {
                txtCallSetStationObj.Text = "";
            }
        }

        private void btnCallStationInput_Click(object sender, EventArgs e)
        {
            txtCallStationInputReturn.Text = I.StationInput(txtInputName.Text, txtInputValue.Text);
            txtCallStationInputObj.Text = I.GetCurrStationObj();
            
        }

        private void btnGetSNStationKPList_Click(object sender, EventArgs e)
        {
            try
            {
                txtGetSNStationKPListReturn.Text = I.GetSNStationKPList(txtKPLSN.Text);
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnCallScanKPTest_Click(object sender, EventArgs e)
        {
            txtScanKPTestReturn.Text = I.ScanKPTest(txtScanKPTestSN.Text, txtScanKPTestKP_ITEM.Text);
        }

        private void btnScanKP_Click(object sender, EventArgs e)
        {
            txtScanKPReturn.Text = I.ScanKP(txtScanKPSN.Text, txtScanKPKP_ITEM.Text);
        }

        private void btnCallGetSNKP_Click(object sender, EventArgs e)
        {
            txtRetGetSNKP.Text = I.GetSNKP(txtGetSNKPSN.Text);
        }

        private void btnCallGetWWN_Click(object sender, EventArgs e)
        {
            var strret = I.GetWWN_Datasharing(txtWWNWSN.Text, txtWWNVSSN.Text, txtWWNCSSN.Text);
            if (strret.StartsWith("ERROR"))
            {
                MessageBox.Show(strret);
                return;
            }
            var wwnList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WWN_DATASHARING>>(strret);
            dgvWWN.DataSource = wwnList;
        }

        private void btnUpdateWWN_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvWWN.SelectedRows.Count == 0)
                {
                    return;
                }
                WWN_DATASHARING item = (WWN_DATASHARING)dgvWWN.SelectedRows[0].DataBoundItem;
                List<WWN_DATASHARING> updateItems = new List<WWN_DATASHARING>();
                updateItems.Add(item);
                var Jstring = Newtonsoft.Json.JsonConvert.SerializeObject(updateItems, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                var strret = I.UpdateWWN_Datasharing(Jstring);
                if (strret.StartsWith("ERROR"))
                {
                    MessageBox.Show(strret);
                    return;
                }
            }
            catch
            { }
        }

        private void QueryObj_Call_Click(object sender, EventArgs e)
        {
            //var selectObj = Newtonsoft.Json.JsonConvert.DeserializeObject(QueryObj_Select.Text);
            var ret = I.GetSQLQUERY(QueryObj_Select.Text, QueryObj_SN.Text);
            QueryObj_ret.Text = ret;
        }

        private void btnCall_GetSNFromOldDB_Click(object sender, EventArgs e)
        {
            var ret = I.GetSNFromOldDB(textSN_GetSNFromOldDB.Text);
            textRet_GetSNFromOldDB.Text = ret;
        }

        private void btnCallR_TEST_BRCD_Click(object sender, EventArgs e)
        {
            var ret = I.UploadDCNTestData(txtR_TEST_BRCD_UPDATE.Text);
            txtR_TEST_BRCD_RET.Text = ret;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ret = I.TE_PASSSTATION(textSN_TE_PASSSTATION.Text, textStationNane_TE_PASSSTATION.Text);
            txtTE_PASSSTATION.Text = ret;
        }

        private void btnUploadJuniper_Click(object sender, EventArgs e)
        {
            var ret = I.UpdateJuniperTest(tbInputJuniper.Text);
            tbOutputJuniper.Text = ret;
        }

        private void btnUploadJSnList_Click(object sender, EventArgs e)
        {
            var ret = I.UpdateJuniperSnList(tbInputJSnList.Text);
            tbOutputJSnList.Text = ret;
        }

        private void btnOpenApiTest2_Click(object sender, EventArgs e)
        {
            ApiTest_2 api2 = new ApiTest_2();
            api2.ShowDialog();
            
        }
    }
}
