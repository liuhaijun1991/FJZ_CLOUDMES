using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MESNCO;


namespace MESNCO_TEST
{
    public partial class Form1 : Form
    {
        IStationObj I = new StationObj();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = I.SetConnPara("ws://127.0.0.1:2130/ReportService", "F1324041", "F1324041");
                
            }
            catch (Exception)
            {
                //textBox1.Text = ee.Message;
            }
            
           
            //I.SetStation("AOI", "LINE1");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = I.SetStation("FI", "LINE1");
                
            }
            catch (Exception)
            {
                //textBox2.Text = ee.Message;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                textBox3.Text = I.StationInput("SN", "12345567");
                //textBox3.Text = "OK";
            }
            catch (Exception)
            {
                //textBox3.Text = ee.Message;
            }
        }
    }
}
