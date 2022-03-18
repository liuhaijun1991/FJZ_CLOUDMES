using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MESMailCenter
{
    public partial class FrmComfig : Form
    {
        DataSet ConnData;
        /// <summary>
        /// 
        /// </summary>
        public FrmComfig()
        {
            InitializeComponent();
        }

        private void FrmComfig_Load(object sender, EventArgs e)
        {
            //Dictionary<string, string> aa = new Dictionary<string, string>();
            //DataSet 
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonLoadConn_Click(object sender, EventArgs e)
        {
            ConnData = new DataSet("ConnData");
            try
            {
                ConnData.ReadXml("DataBase.xml");
            }
            catch
            {
                DataTable dt = ConnData.Tables.Add("TConnString");
                dt.Columns.Add("ConnName");
                dt.Columns.Add("ConnString");
                dt.Columns.Add("IsCrypt");
            }
            GViewConns.DataSource = ConnData.Tables["TConnString"];

        }

        private void GViewConns_Click(object sender, EventArgs e)
        {

        }

        private void GViewConns_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void GViewConns_RowDividerDoubleClick(object sender, DataGridViewRowDividerDoubleClickEventArgs e)
        {
            
        }

        private void GViewConns_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)GViewConns.DataSource;
                DataRow dr = dt.Rows[e.RowIndex];
                txtConnName.Text = dr["ConnName"].ToString();
                if (Boolean.Parse( dr["IsCrypt"].ToString()))
                {
                    txtConnString.Text = SecurityBase.CryptMain.Decode(
                        dr["ConnString"].ToString(),
                        ConnectionManager.CryptName,
                        SecurityBase.BytesIO.FromBase64String(ConnectionManager.CryptKEY));
                }
                else
                {
                    txtConnString.Text = dr["ConnString"].ToString();
                }
                chkIsCrypt.Checked = Boolean.Parse(dr["IsCrypt"].ToString());
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void buttonAddConnString_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)GViewConns.DataSource;
                DataRow dr = dt.NewRow();
                dr["ConnName"] = txtConnName.Text;
                if (chkIsCrypt.Checked)
                {
                    byte[] cryptKey = SecurityBase.BytesIO.FromBase64String(ConnectionManager.CryptKEY);
                    byte[] crypted = SecurityBase.CryptMain.Encode(txtConnString.Text, ConnectionManager.CryptName, cryptKey);
                    dr["ConnString"] = SecurityBase.BytesIO.ToBase64String(crypted);
                }
                else
                {
                    dr["ConnString"] = txtConnString.Text;
                }
                dr["IsCrypt"] = chkIsCrypt.Checked;
                dt.Rows.Add(dr);

                
            }
            catch
            { }
        }

        private void buttonDelConnString_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)GViewConns.DataSource;
                dt.Rows.RemoveAt(GViewConns.SelectedRows[0].Index);
            }
            catch (Exception ee)
            {
                //MessageBox.Show(ee.Message);
            }
        }

        private void buttonSaveConn_Click(object sender, EventArgs e)
        {
            if (GViewConns.DataSource != null)
            {
                this.ConnData.WriteXml("DataBase.xml");
            }
            else
            {
                MessageBox.Show("Haven't init !");
            }
        }

        ProcessManager PM = new ProcessManager();
        private void buttonTaskTest_Click(object sender, EventArgs e)
        {
            PM.Add("TEST", new ProcessManagedItem(new ProcessTest(), 5 * 1000));
        }
        LogManager LM = new LogManager();
        private void buttonLogTest_Click(object sender, EventArgs e)
        {
            try
            {
                LM.AddLog("TEST");
            }
            catch
            { }
            LM.Write("TEST", "hellow World !");
            LM.Write("TEST", "hellow World !!");
            LM.Write("TEST", "hellow World !!!");
        }
    }
}
