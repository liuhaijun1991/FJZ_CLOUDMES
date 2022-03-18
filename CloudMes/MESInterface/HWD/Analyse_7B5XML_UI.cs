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
    public partial class Analyse_7B5XML_UI :  UserControl
    {
        Analyse_7B5XML objAnalyse7B5;
        public Analyse_7B5XML_UI(Analyse_7B5XML _objAnalyse7B5)
        {
            InitializeComponent();
            objAnalyse7B5 = _objAnalyse7B5;
        }

        private void Analyse_7B5XML_UI_Load(object sender, EventArgs e)
        {
            labelFtp.Text = objAnalyse7B5.ftpPath;
            labelSavePath.Text = objAnalyse7B5.savePath;
            labelBackupPath.Text = objAnalyse7B5.backupPath;
            labelErrorPath.Text = objAnalyse7B5.errorPath;
            labelSAPRFC.Text = objAnalyse7B5.saprfc;
            labelMessage.Text = objAnalyse7B5.message;
        }
    }
}
