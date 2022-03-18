using HWDNNSFCBase;
using MESInterface.Common;
using System;
using System.Collections.Specialized;
using System.Data;

namespace MESInterface.ORACLE
{
    public class FileChecker : taskBase
    {
        public string DB = "";

        public string sourcePath = "";
        public string sourceMode = "";
        public string sourceUser = "";
        public string sourcePWD = "";

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
                sourcePath = ConfigGet("SOURCE_PATH");
                sourceUser = ConfigGet("SOURCE_USER");
                sourcePWD = ConfigGet("SOURCE_PWD");
                sourceMode = ConfigGet("SOURCE_MODE");

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
                CheckFile();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
                IsRuning = false;
                throw ex;
            }
            finally
            {
                SFCDB.FreeMe();
            }
        }

        public void CheckFile()
        {
            StringCollection Missingfile = GetFile();//獲取文件到本地臨時目錄
            try
            {
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Start:");
                SFCDB.BeginTrain();
                dbLog.UpdateStatus("CheckOracleLabel", "START");
                if (fileCollection.Count > 0)
                {
                    foreach (string file in fileCollection)
                    {
                        string[] s = file.Split(new char[] { '_', '-' });
                        dbLog.WriterLog("CheckOracleLabel", s[2], s[3], file, "", "S", "", "N");
                        int n = UpdatePOStatus(s[2], s[3]);
                    }

                    foreach (var file in Missingfile)
                    {
                        string[] s = file.Split(new char[] { '_', '-' });
                        dbLog.WriterLog("CheckOracleLabel", s[2], s[3], file, "", "E", "Due to the lack of certain documents, resulting in the corresponding PO label download was canceled", "N");
                    }
                }
                dbLog.UpdateStatus("CheckOracleLabel", "END");
                SFCDB.CommitTrain();
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " End");
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
                dbLog.WriterLog("CheckOracleLabel", "CheckOracleLabel", "", "", "", "E", ex.Message, "N");
                throw;
            }
        }


        public int UpdatePOStatus(string PO, string Line)
        {
            string sqlStr = "UPDATE R_ORACLE_MFPRESETWOHEAD SET SAPFLAG='13' WHERE PO='{0}' AND POLine='{1}' AND SAPFLAG='12'";
            sqlStr = string.Format(sqlStr, PO, Line);
            string res = SFCDB.ExecSQL(sqlStr);
            int n = 0;
            try
            {
                n = int.Parse(res);
            }
            catch
            {
            }
            return n;
        }

        /// <summary>
        /// 返回因LABEL不齊而被過濾的記錄
        /// </summary>
        /// <returns>返回三種LABEL不齊全的記錄</returns>
        StringCollection GetFile()
        {
            StringCollection Missingfile = new StringCollection();

            if (sourceMode == "FTP")
            {
                FTPHelp FromFtp = new FTPHelp(sourcePath, sourceUser, sourcePWD);
                //過濾狀態不是待回傳LABEL狀態和不是本廠的PO
                //string sqlStr = "SELECT * FROM R_ORACLE_MFPRESETWOHEAD WHERE SAPFLAG=12";
                string sqlStr = "SELECT * FROM R_ORACLE_MFPRESETWOHEAD";
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
                        if (DT.Select("PO='" + PO + "' AND POLINE='" + Line + "'").Length > 0)
                        {
                            fileCollection.Add(item);
                        }
                    }
                    Missingfile = FileNameHandle();//過濾三種LABEL不齊全的記錄
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
