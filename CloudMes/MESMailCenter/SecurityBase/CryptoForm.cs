using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using SecurityBase;

namespace MESMailCenter
{
    public partial class CryptoForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public CryptoForm()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] key = BytesIO.FromBase64String(txtEnCodeKey.Text);
                txtCryptoedValue.Text = BytesIO.ToBase64String(CryptMain.Encode(txtValue.Text, comboBox1.Text, key));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SymmetricAlgorithm crypto = CryptMain.GetEncode(comboBox1.Text);
                byte[] key = crypto.Key;
                txtEnCodeKey.Text = txtDeCodeKey.Text = BytesIO.ToBase64String(key);
                txtKeyL.Text = txtDeCodeKey.Text.Length.ToString() + "字符 <> " + (key.Length * 8).ToString() + "Bit";
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                txtValue.Text = CryptMain.Decode(txtCryptoedValue.Text, comboBox1.Text, BytesIO.FromBase64String(txtDeCodeKey.Text));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
    }
}
