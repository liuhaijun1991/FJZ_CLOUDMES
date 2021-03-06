using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace MESPubLab.Common
{
    public class FTPHelp
    {
        //string ftpRemotePath;
        string ftpUserID;
        string ftpPassword;
        string ftpURI;
        

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        public FTPHelp(string _ftpURI, string FtpUserID, string FtpPassword)
        {
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            ftpURI = _ftpURI;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="filename"></param>
        public void Upload(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            string uri = ftpURI + fileInf.Name;
            FtpWebRequest reqFTP;

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                try
                {
                    strm.Flush();
                }
                catch 
                {                    
                }
                strm.Close();
                try
                {
                    strm.Dispose();
                }
                catch
                {
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally {
                try
                {
                    fs.Close();
                }
                catch
                {
                }
                try
                {
                    fs.Dispose();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public void Download(string filePath, string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response;
                try
                {
                    response = (FtpWebResponse)reqFTP.GetResponse();
                }
                catch (Exception)
                {
                    outputStream.Flush();
                    outputStream.Close();
                    outputStream.Dispose();
                    throw;
                }
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                outputStream.Flush();
                try
                {
                    ftpStream.Close();
                    response.Close();
                }
                catch
                {
                }
                try
                {
                    ftpStream.Dispose();
                    outputStream.Dispose();
                }
                catch
                {
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally { 
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        public void Delete(string fileName)
        {
            try
            {
                string uri = ftpURI +"/"+ fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Proxy = null;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderName"></param>
        public void RemoveDirectory(string folderName)
        {
            try
            {
                string uri = ftpURI + folderName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取当前目录下明细(包含文件和文件夹)
        /// </summary>
        /// <returns></returns>
        public StringCollection GetFilesDetailList()
        {
            StringCollection downloadFiles = new StringCollection(); ;
            try
            {
                StringBuilder result = new StringBuilder();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                
                string line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                string[] _fl= result.ToString().Split('\n');
                for (int i = 0; i < _fl.Length; i++)
                {
                    downloadFiles.Add(_fl[i]);
                }
                return downloadFiles;
            }
            catch (Exception)
            {
                downloadFiles = null;
                throw;
            }
        }

        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <returns></returns>
        public StringCollection GetFileList(string mask)
        {
            StringCollection downloadFiles = new StringCollection();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                reqFTP.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

                string line = reader.ReadLine();
                if (line == null)
                {
                    return new StringCollection();
                }
                bool isHtml = false;
                //這裡會碰到兩種返回的結果，一種HTML代碼，一種是文本，HTML需要處理
                if (line.ToUpper().Contains("HTML"))
                {
                    isHtml = true;
                }
                while (line != null)
                {
                    //HTML的文件部分結束后有一個HR標籤，這個標籤後面的代碼不解析
                    if (line.ToUpper().StartsWith("<HR")) 
                    {
                        isHtml = true;
                    }
                    if (isHtml)
                    {
                        if (line.ToUpper().Contains("<PRE>"))
                        {
                            isHtml = false;
                        }
                    }
                    else
                    {
                        if (mask.Trim() != string.Empty)
                        {
                            string[] masks = mask.Split('*');
                            string mask_ = line;
                            bool flag = true;
                            for (int i = 0; i < masks.Length; i++)
                            {
                                if (!mask_.Contains(masks[i]))
                                {
                                    flag = false;
                                    break;
                                }
                                mask_ = mask_.Remove(0, mask_.IndexOf(masks[i]) + masks[i].Length);
                            }
                            if (flag)
                            {
                                if (line.ToUpper().Contains("HREF=\""))
                                {
                                    string fn = line.Substring(line.IndexOf("HREF=\"") + 6, line.IndexOf("\">") - line.IndexOf("HREF=\"") - 6);
                                    if (!fn.Contains(".."))
                                    {
                                        downloadFiles.Add(fn);
                                    }
                                }
                                else
                                {
                                    downloadFiles.Add(line);
                                }
                            }
                        }
                        else
                        {
                            if (line.ToUpper().Contains("HREF=\""))
                            {
                                string fn = line.Substring(line.IndexOf("HREF=\"") + 6, line.IndexOf("\">") - line.IndexOf("HREF=\"") - 6);
                                if (!fn.Contains(".."))
                                {
                                    downloadFiles.Add(fn);
                                }
                            }
                            else
                            {
                                downloadFiles.Add(line);
                            }
                        }
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();                
                return downloadFiles;
            }
            catch (Exception)
            {
                throw;                
            }
        }

        /// <summary>
        /// 获取当前目录下所有的文件夹列表(仅文件夹)
        /// </summary>
        /// <returns></returns>
        public StringCollection GetDirectoryList()
        {
            StringCollection drectory = GetFilesDetailList();
            StringCollection m = new StringCollection();
            foreach (string str in drectory)
            {
                int dirPos = str.IndexOf("<DIR>");
                if (dirPos > 0)
                {
                    /*判断 Windows 风格*/
                    m.Add(str.Substring(dirPos + 5).Trim());
                }
                else if (str.Trim().Substring(0, 1).ToUpper() == "D")
                {
                    /*判断 Unix 风格*/
                    string dir = str.Substring(54).Trim();
                    if (dir != "." && dir != "..")
                    {
                        m.Add(dir);
                    }
                }
            }
            return m;
        }

        /// <summary>
        /// 判断当前目录下指定的子目录是否存在
        /// </summary>
        /// <param name="RemoteDirectoryName">指定的目录名</param>
        public bool DirectoryExist(string RemoteDirectoryName)
        {
            StringCollection dirList = new StringCollection();
            try
            {            
                dirList = GetDirectoryList();
            }
            catch (Exception)
            {
                throw;
            }
            foreach (string str in dirList)
            {
                if (str.Trim() == RemoteDirectoryName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前目录下指定的文件是否存在
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        public bool FileExist(string RemoteFileName)
        {
            StringCollection fileList = new StringCollection();
            try
            {
                fileList = GetFileList("*.*");
            }
            catch (Exception)
            {
                throw;
            }
            foreach (string str in fileList)
            {
                if (str.Trim() == RemoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirName"></param>
        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取指定文件大小
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                ftpStream.Close();
                response.Close();
                return fileSize;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 改名
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        public void ReName(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        public void MovieFile(string currentFilename, string newDirectory)
        {
            try
            {
                ReName(currentFilename, newDirectory);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 切换当前目录
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <param name="IsRoot">true 绝对路径   false 相对路径</param>
        //public void GotoDirectory(string DirectoryName, bool IsRoot)
        //{
        //    if (IsRoot)
        //    {
        //        ftpRemotePath = DirectoryName;
        //    }
        //    else
        //    {
        //        ftpRemotePath += DirectoryName + "/";
        //    }
        //    ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
        //}
        
    }
}
