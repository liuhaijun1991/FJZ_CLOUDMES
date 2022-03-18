using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESInterface.HWD.BackFlush;

namespace MESInterface.HWD
{
    /// <summary>
    /// 
    /// </summary>
    public partial class frmBackFlush :UserControl
    {
        BackFlushHelp BackFlush;
        delegate void AddDataGridDelegate(DataGridView dgv, DataTable dt);
        delegate void SetCtrlEnableDelegate(Control C, bool b);
        delegate void ProcessRateDelegate( ProgressBar bar,Control labcl,int AllCount, int CerCount);       
        void ProcessRate(string BarName,string labNmame,int AllCount, int CerCount)
        {
            Control barcl = GetControlByName(this.Controls, BarName);
            Control lacl = GetControlByName(this.Controls, labNmame);
            if (barcl != null&& lacl!=null)
            {
                ProcessRateDelegate PBdelegete = delegate (ProgressBar PBar, Control labcl, int allcount,int curcont)
                {
                    PBar.Maximum = allcount;
                    PBar.Value = curcont;
                    labcl.Text = curcont.ToString() + "/" + allcount.ToString();
                };
                this.Invoke(PBdelegete, new object[] {(ProgressBar)barcl, lacl, AllCount, CerCount});
            }
           
        }
        void SetCtrlEnable(string ControlName, bool b)
        {
            Control cl = GetControlByName(this.Controls, ControlName);
            if (cl != null)
            {
                SetCtrlEnableDelegate seCdelegete = delegate (Control CL, bool B)
                  {
                      CL.Enabled = B;
                  };
                this.Invoke(seCdelegete, new object[] { cl, b });
            }
        }       
        void AddDataGrid(string DataGridViewName , DataTable dt)
        {
            Control cl = GetControlByName(this.Controls, DataGridViewName);
            if (cl != null)
            {
                AddDataGridDelegate dgdelegete = delegate (DataGridView dg, DataTable ta)
                {
                    dg.DataSource = ta;
                };
                this.Invoke(dgdelegete, new object[] { ((DataGridView)cl), dt });
            }
        }
        private Control GetControlByName(Control.ControlCollection Controls,string ControlName)
        {
            Control FindCl=null;
            foreach (Control cl in Controls)
            {
                if (cl.Name == ControlName)
                {
                    FindCl = cl;
                    break;
                }
                else
                {
                    if (cl.HasChildren)
                    {
                        FindCl= GetControlByName(cl.Controls,ControlName);
                        if (FindCl != null)
                        {
                            break;
                        }
                    }
                }
            }
            return FindCl;
        }
        public frmBackFlush(BackFlushHelp BackFlushs)
        {
            InitializeComponent();
            BackFlush = BackFlushs;
        }
        void BackFlushStartGetPosteData(object para)
        {
            try
            {
                BackFlush.GetSAPWaitForBackFlushWo();
                BackFlush.ToBackFlushCheck();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        void BackFlushCallRFC(object para)
        {
            try
            {
                BackFlush.CallRfcBackFlush();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private void frmBackFlush_Load(object sender, EventArgs e)
        {
            btnCallRFC.Enabled = false;
            BackFlush.processRateDelegate = new BackFlushHelp.ProcessRateDelegate(ProcessRate);
            BackFlush.addDataGridDelegate = new BackFlushHelp.AddDataGridDelegate(AddDataGrid);
            BackFlush.setCtrlEnableDelegate = new BackFlushHelp.SetCtrlEnableDelegate(SetCtrlEnable);
           
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                btnCallRFC.Enabled = false;
                System.Threading.Thread T = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(BackFlushStartGetPosteData));
                T.Start();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnCallRFC_Click(object sender, EventArgs e)
        {
            try
            {
                System.Threading.Thread T = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(BackFlushCallRFC));
                T.Start();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnAutoRun_Click(object sender, EventArgs e)
        {
            //if (btnAutoRun.Text == "自動執行")
            //{
            //    if (TIMER == null)
            //    {
            //        System.Threading.TimerCallback A = new System.Threading.TimerCallback(AutoRun);
            //        TIMER = new System.Threading.Timer(A);
            //        TIMER.Change(Int32.Parse(UpDownAutoTime.Value.ToString()) * 1000, Int32.Parse(UpDownAutoTime.Value.ToString()) * 1000);
            //        btnAutoRun.Text = "停止執行";
                    
            //    }
            //}
            //else
            //{
            //    btnAutoRun.Text = "自動執行";
            //    try
            //    {
            //        CurThread.Abort();
            //    }
            //    catch
            //    { }
            //    TIMER.Change(System.Threading.Timeout.Infinite, Int32.Parse(UpDownAutoTime.Value.ToString()) * 1000);
            //}
        }

     
    }
}
