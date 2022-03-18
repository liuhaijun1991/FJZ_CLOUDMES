using System;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Collections.Generic;

namespace MESInterface.Common
{
    public class FileHelp
    {
        public string FromPath { get; set; }
        public string ToPath { get; set; }
        public string LogPath { get; set; }
        public List<string> ExtName { get; set; }

        public FileHelp()
        {
            ExtName = new List<string>();
            FromPath = "";
            ToPath = "";
            LogPath = Environment.CurrentDirectory + "\\Log";
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }

        public FileHelp(string _FromPath, string _ToPath, List<string> _ExtName)
        {
            ExtName = _ExtName;
            FromPath = _FromPath;
            ToPath = _ToPath;
            LogPath = Environment.CurrentDirectory + "\\Log";
        }
        
        /// <summary>
        /// 獲取目?下所有文件
        /// </summary>
        /// <param name="rootdir">目?路徑</param>
        /// <returns>文件集</returns>
        public StringCollection GetAllFiles()
        {
            StringCollection result = new StringCollection();
            for (int x = 0; x < ExtName.Count; x++)
            {
                //string[] file = Directory.GetFiles(FromPath, ExtName[x]);
                string[] file = Directory.GetFiles(FromPath);
                for (int i = 0; i < file.Length; i++)
                {
                    result.Add(file[i].ToUpper());
                }
            }
            return result;
        }

        public void DeleteFolds(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            DirectoryInfo[] dirs = dir.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                string str = dirs[i].CreationTime.ToString("yyyyMMdd");
                string a = DateTime.Now.ToString("yyyyMM");

                int d = Convert.ToInt32(DateTime.Now.ToString("dd"));
                if (d > 15)
                {
                    int c = Convert.ToInt32(a);
                    int b = Convert.ToInt32(str.Substring(0, 6));

                    string file = dirPath + str;
                    if (c - b > 0)
                    {
                        DirectoryInfo di = new DirectoryInfo(file);
                        di.Delete(true);
                    }
                    str = string.Empty;
                }
            }
        }

        public void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public bool FileExists(string file)
        {
            return File.Exists(file);
        }

        public void CreatePath(string _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }
        
        public void MoveFile(string file)
        {
            string filename = file.Substring(file.LastIndexOf("\\") + 1);
            if (!Directory.Exists(ToPath))
            {
                Directory.CreateDirectory(ToPath);
            }
            if (File.Exists(ToPath + "\\" + filename))
            {
                File.Move(ToPath + "\\" + filename, ToPath + "\\" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + "_" + filename);
                Thread.Sleep(200);
            }
            else
            {
                File.Move(FromPath + "\\" + filename, ToPath + "\\" + filename);
            }
        }
        public void CreateFile(string FolderName, string FileName, string content)
        {
            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }
            if (!File.Exists(FolderName + "\\" + FileName))
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(content);
                using (var fs = new FileStream(FolderName + "\\" + FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                };
            }
        }

        public void CopyFile(string file)
        {
            string filename = file.Substring(file.LastIndexOf("\\") + 1);
            if (!Directory.Exists(ToPath))
            {
                Directory.CreateDirectory(ToPath);
            }
            if (File.Exists(ToPath + "\\" + filename))
            {
                File.Move(ToPath + "\\" + filename, ToPath + "\\" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + "_" + filename);
            }
            File.Copy(FromPath + "\\" + filename, ToPath + "\\" + filename);
        }

        public List<string> ReadFile(string file)
        {
            string filename = file.Substring(file.LastIndexOf("\\") + 1);
            List<string> Res = new List<string>();
            try
            {
                FileStream fS = new FileStream(FromPath + "\\" + filename, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader sR = new StreamReader(fS);
                string nextLine;
                while ((nextLine = sR.ReadLine()) != null)
                {
                    Res.Add(nextLine);
                }
                try
                {
                    sR.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                try
                {
                    fS.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Res;
        }

        /// <summary>
        /// ?建LOG文檔
        /// </summary>
        public void CreateLogTxt()
        {
            string FolderName = DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(LogPath + "\\" + FolderName))
            {
                Directory.CreateDirectory(LogPath + "\\" + FolderName);
            }
            if (!File.Exists(LogPath + "\\" + FolderName + "\\log.txt"))
            {
                FileStream fs = new FileStream(LogPath + "\\" + FolderName + "\\log.txt", FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// ?入LOG檔內容
        /// </summary>
        public void WriteContentToLogTxt(string content)
        {
            CreateLogTxt();
            string FolderName = DateTime.Now.ToString("yyyyMMdd");
            FileStream fs = new FileStream(LogPath + "\\" + FolderName + "\\log.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(content);
            sw.Close();
            fs.Close();
        }
    }
}