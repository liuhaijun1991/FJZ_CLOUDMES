using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESHelper
{
    public partial class FrmInput : Form
    {
        static string _Value;
        public FrmInput()
        {
            InitializeComponent();
        }

        private void FrmInput_Load(object sender, EventArgs e)
        {

        }

        public static string GetValue(string Tittle)
        {
            _Value = "";
            FrmInput I = new FrmInput();
            I.Text = Tittle;
            I.Show();
            I.Visible = false;
            I.ShowDialog();
            return _Value;
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\n' || e.KeyChar == '\r')
            {
                _Value = txtInput.Text;
                this.Close();

            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _Value = txtInput.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _Value = "";
            this.Close();
        }

        private void FrmInput_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                this.TopMost = true;
            }
        }
    }
}
