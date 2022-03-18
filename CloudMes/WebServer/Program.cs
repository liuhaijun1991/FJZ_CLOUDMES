using MESDBHelper;
using MESPubLab;
using System;
using System.Configuration;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WebServer
{
    static class Program
    {
        static bool debug = false;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG     
            try
            {

                var sfcdb = REGHelp.getRegValue("SFCDB");
                var apdb = REGHelp.getRegValue("APDB");

                if (sfcdb != "")
                {
                    ConnectionManager.Add("SFCDB", sfcdb);
                }

                if (apdb != "")
                {
                    ConnectionManager.Add("APDB", apdb);
                }

                if (debug)
                MessageBox.Show("1");
                string p = Environment.CurrentDirectory;
                string webpath = p.Substring(0, p.LastIndexOf("CloudMes") + 9) + "WebClient";
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings.AllKeys.Contains("WebFilePath"))
                {
                    config.AppSettings.Settings["WebFilePath"].Value = webpath + "\\DOWNLOAD";
                }
                else
                {
                    config.AppSettings.Settings.Add("WebFilePath", webpath + "\\DOWNLOAD");
                }
                config.Save();
                if (debug)
                    MessageBox.Show("1.1");
            }
            catch
            {
                if (debug)
                    MessageBox.Show("1.1.2");
            }
#endif
            #region Get WebClient Path And Set Into Config File
            try
            {
                if (debug)
                    MessageBox.Show("2");
                DirectoryEntry rootEntry = new DirectoryEntry("IIS://localhost/w3svc");
                foreach (DirectoryEntry entry in rootEntry.Children)
                {
                    if (entry.SchemaClassName.Equals("IIsWebServer", StringComparison.OrdinalIgnoreCase))
                    {
                        string path = GetWebsitePhysicalPath(entry);
                        if (path.EndsWith("WebClient"))
                        {
                            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            if (config.AppSettings.Settings.AllKeys.Contains("WebFilePath"))
                            {
                                config.AppSettings.Settings["WebFilePath"].Value = path + "\\DOWNLOAD";
                            }
                            else
                            {
                                config.AppSettings.Settings.Add("WebFilePath", path + "\\DOWNLOAD");
                            }
                            config.Save();
                            break;
                        }
                    }
                }
                if (debug)
                    MessageBox.Show("2.1");
            }
            catch
            {
                if (debug)
                    MessageBox.Show("2.1.2");
            }
            #endregion
            Application.ThreadException += Application_ThreadException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (debug)
                MessageBox.Show("3");
            try
            {
                Application.Run(new FormMain());
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        /// <summary>
        /// 得到网站的物理路径
        /// </summary>
        /// <param name="rootEntry">网站节点</param>
        /// <returns></returns>
        private static string GetWebsitePhysicalPath(DirectoryEntry rootEntry)
        {
            string physicalPath = "";
            foreach (DirectoryEntry childEntry in rootEntry.Children)
            {
                if ((childEntry.SchemaClassName == "IIsWebVirtualDir") && (childEntry.Name.ToLower() == "root"))
                {
                    if (childEntry.Properties["Path"].Value != null)
                    {
                        physicalPath = childEntry.Properties["Path"].Value.ToString();
                    }
                    else
                    {
                        physicalPath = "";
                    }
                }
            }
            return physicalPath;
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            FileStream fs = new FileStream("SystemErr.log", System.IO.FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now);
            sw.Write(e.Exception.Message);
            sw.WriteLine("--------------------------------");
            sw.Flush();
            sw.Close();
            Application.Restart();
        }
    }
}
