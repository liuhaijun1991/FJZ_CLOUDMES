using System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace MESHelper.Common
{
    public class FtpHelper
    {
        /// <summary>
        /// ftp方式上传 
        /// </summary>
        public static int Upload(string filePath, string filename, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            FileInfo fileInf = new FileInfo(filePath + "\\" + filename);
            string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileInf.Name));
            try
            {
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = fileInf.Length;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                FileStream fs = fileInf.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                Stream strm = reqFTP.GetRequestStream();

                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
                return 0;
                
            }
            catch
            {
                reqFTP.Abort();
                return -2;
            }
        }

        /// <summary>
        /// FTP下载 
        /// </summary>
        public static int Download(string LocalPath,string ftpPath,string fileName, string ftpServerIP,string port, string ftpUserID, string ftpPassword)
        {
            try
            {
                FileStream outputStream = new FileStream(LocalPath + "\\" + fileName, FileMode.Create);
                string url = string.Format("FTP://{0}:{1}{2}/{3}", ftpServerIP, port, ftpPath, fileName);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));                
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                //reqFTP.KeepAlive = false;
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
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                reqFTP.Abort();
                reqFTP = null;
                return 0;
            }
            catch
            {
                return -2;
            }
        }

        /// <summary>
        /// 搜索当前目录下符合条件的文件
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="mask">条件</param>
        /// <param name="ftpServerIP"></param>
        /// <param name="port"></param>
        /// <param name="ftpUserID"></param>
        /// <param name="ftpPassword"></param>
        /// <returns></returns>
        public static StringCollection GetFileList(string ftpPath, string mask, string ftpServerIP, string port, string ftpUserID, string ftpPassword)
        {
            StringCollection downloadFiles = new StringCollection();
            FtpWebRequest reqFTP;
            try
            {
                string url = string.Format("FTP://{0}:{1}{2}/", ftpServerIP, port, ftpPath);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

                string line = reader.ReadLine();
                while (line != null)
                {
                    string[] masks = mask.Split('*');
                    string mask_ = line.ToUpper();
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
                        downloadFiles.Add(line);
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                return downloadFiles;
            }
            catch (Exception)
            {
                downloadFiles = null;
                return downloadFiles;
            }
        }
    }
}
