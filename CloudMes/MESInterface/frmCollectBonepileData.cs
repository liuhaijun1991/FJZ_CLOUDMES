using MESDBHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface
{
    public partial class frmCollectBonepileData : Form
    {       
        public frmCollectBonepileData()
        {
            InitializeComponent();
        }               

        private void frmCollectBonepileData_Load(object sender, EventArgs e)
        { 
            OleExec SFCDB = null;
            try
            {
                RecodeLocalLog("Begin");
                string db_string= HWDNNSFCBase.ConfigFile.ReadIniData("CollectBonepileData", "DB", "", @".\config.ini");
                string type = HWDNNSFCBase.ConfigFile.ReadIniData("CollectBonepileData", "TYPE", "", @".\config.ini");
                string bu = HWDNNSFCBase.ConfigFile.ReadIniData("CollectBonepileData", "BU", "", @".\config.ini");
                SFCDB = new OleExec(db_string, false);
                MESStation.Config.BonepileConfig bonepile = new MESStation.Config.BonepileConfig();
                bonepile.BonepileSummaryReport(SFCDB, bu, "ALL", type, null);              
                RecodeLocalLog("End");
                RecodeLocalLog("");
            }
            catch (Exception ex)
            {
                RecodeLocalLog("Error:" + ex.Message);
            }
            finally
            {
                this.Close();
            }
        }

        private void RecodeLocalLog(string msg)
        {
            string logPath = System.IO.Directory.GetCurrentDirectory() + "\\log\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string logFile = System.IO.Directory.GetCurrentDirectory() + "\\log\\" + "CollectBonepileData.log";
            FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
            //通过指定字符编码方式可以实现对汉字的支持，否则在用记事本打开查看会出现乱码            
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Current);
            if (msg == "")
            {
                sw.WriteLine();
            }
            else
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + ": " + msg);
            }
            sw.Flush();
            sw.Close();
        }
    }
}
