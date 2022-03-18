using HWDNNSFCBase;
using MESInterface.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using MESPubLab;

namespace MESInterface.ORACLE
{
    public class FileTransfer : taskBase
    {
        public string DB = "";

        public string sourcePath = "";
        public string sourceMode = "";
        public string sourceUser = "";
        public string sourcePWD = "";

        public string StoragePath = "";
        public string StorageMode = "";
        public string StorageUser = "";
        public string StoragePWD = "";

        public string TempPath = "";

        public bool IsRuning = false;
        OleExec SFCDB = null;
        public StringCollection fileCollection;
        public FileHelp FileLog = null;
        public Log dbLog = null;
        public string FileNameFormat = "";
        public override void init()
        {
            FileLog = new FileHelp();
            fileCollection = new StringCollection();
            try
            {
                FileLog.LogPath += "\\" + ConfigGet("NAME");
            }
            catch (Exception ex)
            {
                FileLog.LogPath += "\\Base";
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);                
            }
            try
            {
                DB = ConfigGet("DB");
                TempPath = ConfigGet("TEMP_PATH");

                sourcePath = ConfigGet("SOURCE_PATH");
                sourceUser = ConfigGet("SOURCE_USER");
                sourcePWD = ConfigGet("SOURCE_PWD");
                sourceMode = ConfigGet("SOURCE_MODE");

                StoragePath = ConfigGet("STORAGE_PATH");
                StorageUser = ConfigGet("STORAGE_USER");
                StoragePWD = ConfigGet("STORAGE_PWD");
                StorageMode= ConfigGet("STORAGE_MODE");

                FileNameFormat = ConfigGet("FILENAMEFORMAT");
                if (FileNameFormat.Trim().Length == 0)
                {
                    FileNameFormat = "*.*";
                }
                SFCDB = new OleExec(DB, false);
                dbLog = new Log(SFCDB);
            }
            catch (Exception ex)
            {
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
                throw ex;
            }
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                TranFile();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
                IsRuning = false;
                throw ex;
            }
            finally {
                SFCDB.CloseMe();
            }
        }
        public void TranFile()
        {
            StringCollection Missingfile = GetFile();//獲取文件到本地臨時目錄
            try
            {
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Start:");
                SFCDB.BeginTrain();
                dbLog.UpdateStatus("GetOracleLabel", "START");
                if (fileCollection.Count > 0)
                {
                    if (StorageMode == "FS")
                    {
                        FileHelp FileSystem = new FileHelp(TempPath, StoragePath, new List<string> { "*.*" });
                        foreach (string file in fileCollection)
                        {
                            FileSystem.MoveFile(file);

                            string[] s = file.Split(new char[] { '_', '-' });
                            dbLog.WriterLog("GetOracleLabel", s[2], s[3], file, "", "S", "", "N");
                            UpdatePOStatus(s[2], s[3]);
                            FileLog.WriteContentToLogTxt(file);
                        }
                    }
                    else if (StorageMode == "FTP")
                    {
                        FileHelp FileSystem = new FileHelp(TempPath, StoragePath, new List<string> { "*.*" });
                        FTPHelp Ftp = new FTPHelp(StoragePath, StorageUser, StoragePWD);
                        foreach (string file in fileCollection)
                        {
                            Ftp.Upload(TempPath + "\\" + file);

                            string[] s = file.Split(new char[] { '_', '-' });
                            dbLog.WriterLog("GetOracleLabel", s[2], s[3], file, "", "S", "", "N");
                            UpdatePOStatus(s[2], s[3]);
                            FileLog.WriteContentToLogTxt(file);
                        }
                        FileSystem.DeleteFolds(TempPath);
                        FileSystem.CreatePath(TempPath);
                    }

                    foreach (var file in Missingfile)
                    {
                        string[] s = file.Split(new char[] { '_', '-' });
                        dbLog.WriterLog("GetOracleLabel", s[2], s[3], file, "", "E", "Due to the lack of certain documents, resulting in the corresponding PO label download was canceled", "N");
                    }
                }
                dbLog.UpdateStatus("GetOracleLabel", "END");
                SFCDB.CommitTrain();
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " End");
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
                dbLog.WriterLog("GetOracleLabel", "GetOracleLabel", "", "", "", "E", ex.Message, "N");
                throw;
            }
        }


        public void UpdatePOStatus(string PO, string Line)
        {
            string sqlStr = "UPDATE R_ORACLE_MFPRESETWOHEAD SET StatusID='13' WHERE PO='{0}' AND Line='{1}' AND SAPFLAG='12'";
            sqlStr = string.Format(sqlStr, PO, Line);
            string res = SFCDB.ExecSQL(sqlStr);
        }

        /// <summary>
        /// 將ORACLE回傳的LABEL下載到臨時文件夾，同時返回因三種LABEL不齊而被過濾的記錄
        /// </summary>
        /// <returns>返回三種LABEL不齊全的記錄</returns>
        StringCollection GetFile()
        {
            StringCollection Missingfile = new StringCollection();

            if (sourceMode == "FTP")
            {
                FTPHelp FromFtp = new FTPHelp(sourcePath, sourceUser, sourcePWD);
                //過濾狀態不是待回傳LABEL狀態和不是本廠的PO
                string sqlStr = "SELECT * FROM R_ORACLE_MFPRESETWOHEAD WHERE SAPFLAG=12";
                DataTable DT = SFCDB.RunSelect(sqlStr).Tables[0];
                if (DT.Rows.Count > 0)
                {
                    StringCollection fileCollectionTemp = new StringCollection();
                    try
                    {
                        fileCollectionTemp = FromFtp.GetFileList(FileNameFormat);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    fileCollection = new StringCollection();
                    foreach (string item in fileCollectionTemp)
                    {
                        string[] s = item.Split(new char[] { '_', '-' });
                        string PO = s[2];
                        string Line = s[3];
                        if (DT.Select("PO='" + PO + "' AND LINE='" + Line + "'").Length > 0)
                        {
                            fileCollection.Add(item);
                        }
                    }
                    Missingfile = FileNameHandle();//過濾三種LABEL不齊全的記錄
                    if (fileCollection.Count != 0)
                    {
                        foreach (string file in fileCollection)
                        {
                            FromFtp.Download(TempPath, file);
                        }
                    }
                }
            }
            else if (sourceMode == "FS")
            {

            }
            return Missingfile;
        }
               

        /// <summary>
        /// 處理SL、PS、CI三種文件不全套的不抓取
        /// </summary>
        public StringCollection FileNameHandle()
        {
            StringCollection Temp = new StringCollection();
            for (int i = 0; i < fileCollection.Count; i++)
            {
                int count = 0;
                string mask = "*{0}*";
                string FileName = "";
                FileName = fileCollection[i].Substring(2, fileCollection[i].LastIndexOf("_"));
                mask = string.Format(mask, FileName);
                for (int x = 0; x < fileCollection.Count; x++)
                {
                    string[] masks = mask.Split('*');
                    string mask_ = fileCollection[x];
                    bool flag = true;
                    for (int n = 0; n < masks.Length; n++)
                    {
                        if (!mask_.Contains(masks[n]))
                        {
                            flag = false;
                            break;
                        }
                        mask_ = mask_.Remove(0, mask_.IndexOf(masks[n]) + masks[n].Length);
                    }
                    if (flag)
                    {
                        count++;
                    }
                }
                if (count < 2)
                {
                    Temp.Add(fileCollection[i]);
                }
            }
            for (int i = 0; i < Temp.Count; i++)
            {
                fileCollection.Remove(Temp[i]);
            }
            return Temp;
        }
    }
}
