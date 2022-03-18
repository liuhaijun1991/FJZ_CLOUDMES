using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;

namespace MESHelper.Plugin
{
    public class Printer_Doc
    {
        public string FilePath { get; set; }
        public string PrinterName { get; set; }
        PrintDocument pd;
        public Printer_Doc(string _Path)
        {
            if (!File.Exists(_Path))
            {
                //throw new Exception("找不到文檔");
                throw new Exception("File is not exists!");
            }
            else
            {
                FilePath = _Path;
            }
        }

        public void Print()
        {
            pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = PrinterName;
            Process process = new Process();
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = true;
            startinfo.Verb = "Print";
            startinfo.CreateNoWindow = true;
            startinfo.WindowStyle = ProcessWindowStyle.Maximized;
            startinfo.Arguments = @"/p/h " + FilePath + "/ \"" + pd.PrinterSettings.PrinterName + "\"";
            startinfo.FileName = FilePath;
            process.StartInfo = startinfo;
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
