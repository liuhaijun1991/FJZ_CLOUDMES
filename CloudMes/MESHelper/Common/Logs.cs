using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MESHelper.Common
{
    class Logs
    {
        string FileNamePath;
        StreamWriter W;   
        public Logs()
        {
            FileNamePath = Environment.CurrentDirectory;  
        }

        public string WritePrintFile(string Zbl,string number)
        {
            try
            {
                if (!Directory.Exists(FileNamePath + "\\PrintFile"))
                {
                    System.IO.Directory.CreateDirectory(FileNamePath + "\\PrintFile");
                }
                string FilePath = FileNamePath + "\\PrintFile\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + number + ".TXT";
                W = new StreamWriter(FilePath, false);
                W.WriteLine(Zbl);
                W.Flush();
                W.Close();
                return FilePath;
            }
            catch(Exception ex)
            {
                throw ex;
                return "ERROR:" + ex.Message;
            }
        }

        public string WriteLog(string Msg)
        {
            try
            {
                if (!Directory.Exists(FileNamePath + "\\Log"))
                {
                    System.IO.Directory.CreateDirectory(FileNamePath + "\\Log");
                }
                string FilePath = FileNamePath + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + ".TXT";
                W = new StreamWriter(FilePath, true);
                W.WriteLine(Msg);
                W.Flush();
                W.Close();
                return FilePath;
            }
            catch(Exception ex)
            {
                return "ERROR:" + ex.Message;
            }
        }

        public string WriteFile(string path,string Msg)
        {
            try
            {
                W = new StreamWriter(path, false);
                W.WriteLine(Msg);
                W.Flush();
                W.Close();
                return path;
            }
            catch (Exception ex)
            {
                return "ERROR:" + ex.Message;
            }
        }

    }
}
