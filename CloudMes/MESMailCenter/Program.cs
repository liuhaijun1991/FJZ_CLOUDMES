using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MESMailCenter
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new frmMain());
                //Application.Run(new FrmComfig());
                //Application.Run(new CryptoForm());
                //Application.Run(new Form1());
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
    }
}
