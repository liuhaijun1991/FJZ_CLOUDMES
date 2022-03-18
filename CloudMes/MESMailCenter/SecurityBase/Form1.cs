using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.IO;
using System.Security.Cryptography;
using SecurityBase;

namespace MESMailCenter
{
    public partial class Form1 : Form
    {
        byte[] IV;
        byte[] Key;
        System.Security.Cryptography.SymmetricAlgorithm Crypt;
        byte[] data;
        //Encoder Enc = UTF8Encoding.UTF8;
        //Decoder Dec = UTF8Encoding.

        ToBase64Transform ToBase64 = new ToBase64Transform();
        FromBase64Transform FromBase64 = new FromBase64Transform();
        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Crypt = SymmetricAlgorithm.Create("3DES");
            Crypt = TripleDES.Create();
            byte[] salt = new byte[8];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(salt);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes("test", salt);
            Crypt.Key = pdb.GetBytes(24);
            Crypt.IV = new byte[8];
            Key = Crypt.Key;
            IV = Crypt.IV;
            //Base64.


        }

        private void button2_Click(object sender, EventArgs e)
        {
            MemoryStream s = new MemoryStream();
            CryptoStream cs = new CryptoStream(s, ToBase64, CryptoStreamMode.Write);
            cs = new CryptoStream(cs, Crypt.CreateEncryptor(), CryptoStreamMode.Write);
            StreamWriter w = new StreamWriter(cs);
            w.Write(textBox1.Text);
            w.Flush();
            cs.FlushFinalBlock();
            data = new byte[s.Length];
            s.Position = 0;
            s.Read(data,0,(int)s.Length);
            //textBox2.Text = BitConverter.ToString(data);
            s.Position = 0;
            StreamReader r = new StreamReader(s);
            textBox2.Text = r.ReadToEnd();
            //BitConverter.
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MemoryStream s = new MemoryStream((new UTF8Encoding()).GetBytes(textBox2.Text));
            s.Position = 0;
            CryptoStream cs = new CryptoStream(s, FromBase64, CryptoStreamMode.Read);
            cs = new CryptoStream(cs, Crypt.CreateDecryptor(), CryptoStreamMode.Read);
            StreamReader r = new StreamReader(cs);
            string t = r.ReadToEnd();
            byte[] bytes = (new UTF8Encoding()).GetBytes(t);
            textBox3.Text = t;// +"|" + BitConverter.ToString(bytes);

        }
    }
}
