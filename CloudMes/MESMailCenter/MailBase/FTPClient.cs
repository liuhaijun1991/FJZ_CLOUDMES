using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace MESMailCenter
{
    class FTPClient
    {
        string ftpServerIP;
        string ftpUserID;
        string ftpPassword;
        FtpWebRequest reqFTP;
        public void Connecttest(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            // 根据uri?建FtpWebRequest?象
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP));
            // 指定?据???型
            reqFTP.UseBinary = true;
            // ftp用?名和密?
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }
        #region ?接
        /// <summary>
        /// ?接
        /// </summary>
        /// <param name="path"></param>
        private void Connect(String path)//?接ftp
        {
            // 根据uri?建FtpWebRequest?象
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
            // 指定?据???型
            reqFTP.UseBinary = true;
            // ftp用?名和密?
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }
        #endregion

        #region ftp登?信息
        /// <summary>
        /// ftp登?信息
        /// </summary>
        /// <param name="ftpServerIP">ftpServerIP</param>
        /// <param name="ftpUserID">ftpUserID</param>
        /// <param name="ftpPassword">ftpPassword</param>
        public void FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }
        #endregion

        #region ?取文件列表
        /// <summary>
        /// ?取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="WRMethods"></param>
        /// <returns></returns>
        private string[] GetFileList(string path, string WRMethods)//上面的代?示例了如何?ftp服?器上?得文件列表
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            try
            {
                Connect(path);
                reqFTP.Method = WRMethods;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);//中文文件名
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                downloadFiles = null;
                return downloadFiles;
            }
        }
        public string[] GetFileList(string path)//上面的代?示例了如何?ftp服?器上?得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory);
        }
        public string[] GetFileList()//上面的代?示例了如何?ftp服?器上?得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectory);
        }
        #endregion
        #region 上?文件
        /// <summary>
        /// 上?文件
        /// </summary>
        /// <param name="filename"></param>
        public bool Upload(string filename, string path, out string errorinfo) //上面的代???了?ftp服?器上?文件的功能
        {
            path = path.Replace("\\", "/");
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + path + "/" + fileInf.Name;
            Connect(uri);//?接         
            // 默??true，?接不?被??
            // 在一?命令之后被?行
            reqFTP.KeepAlive = false;
            // 指定?行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 上?文件?通知服?器文件的大小
            reqFTP.ContentLength = fileInf.Length;
            // ??大小?置?kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打?一?文件流(System.IO.FileStream) 去?上?的文件
            FileStream fs = fileInf.OpenRead();
            try
            {
                // 把上?的文件?入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次?文件流的kb
                contentLen = fs.Read(buff, 0, buffLength);
                // 流?容?有?束
                while (contentLen != 0)
                {
                    // 把?容?file stream ?入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // ????流
                strm.Close();
                fs.Close();
                errorinfo = "完成";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},無法完成上傳", ex.Message);
                return false;
            }
        }
        #endregion
        #region ??文件
        /// <summary>
        /// ??文件
        /// </summary>
        /// <param name="filename"></param>
        public bool Upload(string filename, long size, string path, out string errorinfo) //上面的代???了?ftp服?器上?文件的功能
        {
            path = path.Replace("\\", "/");
            FileInfo fileInf = new FileInfo(filename);
            //string uri = "ftp://" + path + "/" + fileInf.Name;
            string uri = "ftp://" + path;
            Connect(uri);//?接         
            // 默??true，?接不?被??
            // 在一?命令之后被?行
            reqFTP.KeepAlive = false;
            // 指定?行什么命令         
            reqFTP.Method = WebRequestMethods.Ftp.AppendFile;
            // 上?文件?通知服?器文件的大小
            reqFTP.ContentLength = fileInf.Length;
            // ??大小?置?kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打?一?文件流(System.IO.FileStream) 去?上?的文件
            FileStream fs = fileInf.OpenRead();
            try
            {
                StreamReader dsad = new StreamReader(fs);
                fs.Seek(size, SeekOrigin.Begin);
                // 把上?的文件?入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次?文件流的kb
                contentLen = fs.Read(buff, 0, buffLength);
                // 流?容?有?束
                while (contentLen != 0)
                {
                    // 把?容?file stream ?入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // ????流
                strm.Close();
                fs.Close();
                errorinfo = "完成";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},無法完成上傳", ex.Message);
                return false;
            }
        }
        #endregion
        #region 下?文件
        /// <summary>
        /// 下?文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="errorinfo"></param>
        /// <returns></returns>
        public bool Download(string ftpfilepath, string filePath, string fileName, out string errorinfo)////上面的代???了?ftp服?器下?文件的功能
        {
            try
            {
                filePath = filePath.Replace("我的電腦\\", "");
                String onlyFileName = Path.GetFileName(fileName);
                string newFileName = filePath + onlyFileName;
                if (File.Exists(newFileName))
                {
                    errorinfo = string.Format("本地文件{0}已存在,?法下?", newFileName);
                    return false;
                }
                ftpfilepath = ftpfilepath.Replace("\\", "/");
                string url = "ftp://" + ftpfilepath;
                Connect(url);//?接 
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                FileStream outputStream = new FileStream(newFileName, FileMode.Create);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                errorinfo = "";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},無法下載", ex.Message);
                return false;
            }
        }
        #endregion
        #region ?除文件
        /// <summary>
        /// ?除文件
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteFileName(string fileName)
        {
            try
            {
                FileInfo fileInf = new FileInfo(fileName);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//?接         
                // 默??true，?接不?被??
                // 在一?命令之后被?行
                reqFTP.KeepAlive = false;
                // 指定?行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "?除??");
            }
        }
        #endregion
        #region 在ftp上?建目?
        /// <summary>
        /// 在ftp上?建目?
        /// </summary>
        /// <param name="dirName"></param>
        public void MakeDir(string dirName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//?接      
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region ?除ftp上目?
        /// <summary>
        /// ?除ftp上目?
        /// </summary>
        /// <param name="dirName"></param>
        public void delDir(string dirName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//?接      
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region ?得ftp上文件大小
        /// <summary>
        /// ?得ftp上文件大小
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public long GetFileSize(string filename)
        {
            long fileSize = 0;
            filename = filename.Replace("\\", "/");
            try
            {
                // FileInfo fileInf = new FileInfo(filename);
                //string uri1 = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                // string uri = filename;
                string uri = "ftp://" + filename;
                Connect(uri);//?接      
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                fileSize = response.ContentLength;
                response.Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
            return fileSize;
        }
        #endregion
        #region ftp上文件改名
        /// <summary>
        /// ftp上文件改名
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        public void Rename(string currentFilename, string newFilename)
        {
            try
            {
                FileInfo fileInf = new FileInfo(currentFilename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//?接
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                //Stream ftpStream = response.GetResponseStream();
                //ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region ?得文件明晰
        /// <summary>
        /// ?得文件明晰
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesDetailList()
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }
        /// <summary>
        /// ?得文件明晰
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFilesDetailList(string path)
        {
            path = path.Replace("\\", "/");
            return GetFileList("ftp://" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }
        #endregion

    }
}
