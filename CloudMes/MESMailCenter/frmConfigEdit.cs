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
    public partial class frmConfigEdit : Form
    {
        Configs _Cfg = new Configs();
        TreeNode _crrEditTreeNode;
        DataRow _crrEditDataRow;
        DataRow _copyRow;
        public frmConfigEdit()
        {
            InitializeComponent();
            _Cfg.Clear();
            _Cfg.ReadXml(".\\SQLTaskConfig.xml");
            treeView1.Nodes.Clear();
            for (int i = 0; i < _Cfg.SQLTaskConfig.Rows.Count; i++)
            {
                TreeNode tn = treeView1.Nodes.Add(_Cfg.SQLTaskConfig[i]["Name"].ToString());
                tn.Tag = _Cfg.SQLTaskConfig[i];
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            _Cfg.Clear();
            _Cfg.ReadXml(".\\SQLTaskConfig.xml");
            treeView1.Nodes.Clear();
            for (int i = 0; i < _Cfg.SQLTaskConfig.Rows.Count; i++)
            {
                TreeNode tn = treeView1.Nodes.Add(_Cfg.SQLTaskConfig[i]["Name"].ToString());
                tn.Tag = _Cfg.SQLTaskConfig[i];
            }
            //dgDataEdit.DataSource = _Cfg;
            cmbSelectCfg.Items.Clear();
            foreach (DataTable dt in _Cfg.Tables)
            {
                cmbSelectCfg.Items.Add(dt.TableName);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            _crrEditTreeNode = e.Node;
            _crrEditDataRow = ((DataRow)e.Node.Tag);
            toolStripLabel1.Text = _crrEditDataRow["Name"].ToString();
            editZoomInit();
        }

        void editZoomInit()
        {
            _crrEditTreeNode.Text = _crrEditDataRow["Name"].ToString();
            this.txtName.Text = _crrEditDataRow["Name"].ToString();
            this.txtTimeSpan.Text = _crrEditDataRow["TimeSpan"].ToString();
            this.txtMailTo.Text = _crrEditDataRow["MailTo"].ToString();
            this.txtDBKey.Text = _crrEditDataRow["DBKey"].ToString();
            try
            {
                this.checkUseExcel.Checked = Boolean.Parse(_crrEditDataRow["UseExcel"].ToString());
            }
            catch
            {
                this.checkUseExcel.Checked = false;
                _crrEditDataRow["UseExcel"] = false;
            }
            try
            {
                this.checkNoRecordSendMail.Checked = Boolean.Parse(_crrEditDataRow["NoRecordSendMail"].ToString());
            }
            catch
            {
                this.checkNoRecordSendMail.Checked = false;
                _crrEditDataRow["NoRecordSendMail"] = false;
            }
            try
            {
                this.checkUseWebServiceSend.Checked = Boolean.Parse(_crrEditDataRow["UseWebServiceSend"].ToString());
            }
            catch
            {
                this.checkUseWebServiceSend.Checked = false;
                _crrEditDataRow["UseWebServiceSend"] = false;
            }
            try
            {
                this.checkUseSMTP.Checked = Boolean.Parse(_crrEditDataRow["UseSMTP"].ToString());
            }
            catch
            {
                this.checkUseSMTP.Checked = false;
                _crrEditDataRow["UseSMTP"] = false;
            }
            try
            {
                this.cmbLanguage.Text = _crrEditDataRow["LANGUAGE"].ToString();
                if (this.cmbLanguage.Text == "")
                {
                    this.cmbLanguage.Text = "繁體中文";
                }
            }
            catch
            {
                this.cmbLanguage.Text = "繁體中文";
            }

            this.txtSQL.Text = _crrEditDataRow["SQL"].ToString();
            this.txtEXECSQL.Text = _crrEditDataRow["EXECSQL"].ToString();
            this.txtContext1.Text = _crrEditDataRow["Context1"].ToString();
            this.txtContext2.Text = _crrEditDataRow["Context2"].ToString();
            this.txtSMTPIP.Text = _crrEditDataRow["SMTP_IP"].ToString();
            this.txtSMTPFROM.Text = _crrEditDataRow["SMTP_FROM"].ToString();
        }

        private void txtEXECSQL_TextChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["EXECSQL"] = this.txtEXECSQL.Text;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["Name"] = this.txtName.Text;
            _crrEditTreeNode.Text = this.txtName.Text;
        }

        private void txtTimeSpan_TextChanged(object sender, EventArgs e)
        {
            //this.txtTimeSpan.Text = _crrEditDataRow["TimeSpan"].ToString();
            try
            {
                //_crrEditDataRow["TimeSpan"] = Int32.Parse(this.txtTimeSpan.Text);
                _crrEditDataRow["TimeSpan"] = this.txtTimeSpan.Text;
            }
            catch
            {
                this.txtTimeSpan.Text = _crrEditDataRow["TimeSpan"].ToString();
            }
        }

        private void txtDBKey_TextChanged(object sender, EventArgs e)
        {
            //this.txtDBKey.Text = _crrEditDataRow["DBKey"].ToString();
            _crrEditDataRow["DBKey"] = this.txtDBKey.Text;
        }

        private void txtMailTo_TextChanged(object sender, EventArgs e)
        {
            //this.txtMailTo.Text = _crrEditDataRow["MailTo"].ToString();
            //_crrEditDataRow["MailTo"] = this.txtMailTo.Text.Replace("\r","").Replace("\n","");
            _crrEditDataRow["MailTo"] = this.txtMailTo.Text;
        }

        private void checkUseExcel_CheckedChanged(object sender, EventArgs e)
        {
            //this.checkUseExcel.Checked = Boolean.Parse(_crrEditDataRow["UseExcel"].ToString());
            _crrEditDataRow["UseExcel"] = this.checkUseExcel.Checked;
        }

        private void checkNoRecordSendMail_CheckedChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["NoRecordSendMail"] = this.checkNoRecordSendMail.Checked;
        }

        private void txtSQL_TextChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["SQL"] = this.txtSQL.Text;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            _crrEditDataRow = _Cfg.SQLTaskConfig.NewRow();
            _crrEditDataRow["Name"] = "新任務";
            _crrEditTreeNode = treeView1.Nodes.Add(_crrEditDataRow["Name"].ToString());
            _crrEditTreeNode.Tag = _crrEditDataRow;
            _Cfg.SQLTaskConfig.Rows.Add(_crrEditDataRow);
            editZoomInit();

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            _copyRow = _crrEditDataRow;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            foreach (DataColumn dc in _Cfg.SQLTaskConfig.Columns)
            {
                _crrEditDataRow[dc.Caption] = _copyRow[dc.Caption];
            }
            editZoomInit();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            _Cfg.AcceptChanges();
            _Cfg.WriteXml(".\\SQLTaskConfig.xml",XmlWriteMode.DiffGram);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            _Cfg.SQLTaskConfig.Rows.Remove(_crrEditDataRow);
            if (_Cfg.SQLTaskConfig.Rows.Count == 0)
            {
                _crrEditDataRow = _Cfg.SQLTaskConfig.NewRow();
                _crrEditDataRow["Name"] = "新任務";
                _crrEditTreeNode = treeView1.Nodes.Add(_crrEditDataRow["Name"].ToString());
                _crrEditTreeNode.Tag = _crrEditDataRow;
                _Cfg.SQLTaskConfig.Rows.Add(_crrEditDataRow);
                
            }
            else
            {
                _crrEditDataRow = _Cfg.SQLTaskConfig.Rows[0];
            }
            treeView1.Nodes.Clear();
            for (int i = 0; i < _Cfg.SQLTaskConfig.Rows.Count; i++)
            {
                TreeNode tn = treeView1.Nodes.Add(_Cfg.SQLTaskConfig[i]["Name"].ToString());
                tn.Tag = _Cfg.SQLTaskConfig[i];
            }
            editZoomInit();
        }

        private void cmbSelectCfg_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgDataEdit.DataSource = _Cfg.Tables[cmbSelectCfg.Text];
            //cmbSelectCfg.Text
        }

        private void checkUseWebServiceSend_CheckedChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["UseWebServiceSend"] = this.checkUseWebServiceSend.Checked;
        }

        private void txtContext1_TextChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["Context1"] = this.txtContext1.Text;
        }

        private void txtContext2_TextChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["Context2"] = this.txtContext2.Text;
        }

       

        private void txtSMTPIP_TextChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["SMTP_IP"] = this.txtSMTPIP.Text;
        }

        private void txtSMTPFROM_TextChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["SMTP_FROM"] = this.txtSMTPFROM.Text;
        }

        private void checkUseSMTP_CheckedChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["UseSMTP"] = this.checkUseSMTP.Checked;
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _crrEditDataRow["LANGUAGE"] = this.cmbLanguage.Text;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
