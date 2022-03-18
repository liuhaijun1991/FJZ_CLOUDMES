using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESHelper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。

        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    MessageBox.Show(args[i]);
                }
                //meshelper://127.0.0.1:2130,PRINT,PRINT,FJZ
                var pattern = "meshelper://([\\w,.,:]+),(\\w+),(\\w+),(\\w+)/";
                //var m = Regex.Match("meshelper://127.0.0.1:2130,PRINT,PRINT,FJZ/", pattern);
                var m = Regex.Match(args[0], pattern);

                MessageBox.Show(m.Success.ToString());
                if (m.Success)
                {
                    Application.Run(new Helper(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value));
                }
                else
                {
                    Application.Run(new Helper());
                }

            }
            else
            {
                Application.Run(new Helper());
            }
            
            
        }
    }
}
