using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESNCO_TEST
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new API_TEST());
            // Application.Run(new ApiTest_2());
            //Application.Run(new FrmDBQueryTest());
            Application.Run(new frmREPORTTEST());
        }
    }
}
