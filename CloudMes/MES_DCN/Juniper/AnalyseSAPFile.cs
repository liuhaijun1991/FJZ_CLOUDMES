using MESDataObject;
using MESDataObject.Module.Juniper;
using MESDBHelper;
using MESPubLab.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MES_DCN.Juniper
{
    public class AnalyseSAPFile
    {
        private string _mesDBStr, _buStr, _filePath, _backupPath, _errorPath, _remotePath, _keyPath, _fileType, _sendFalg, _sendFilePath, _sendRemotePath, _testKeyPath, _remotePath2;
        #region B2B SAP sftp Production
        private string CONST_SFTPHost = "10.191.23.14";
        private string CONST_SFTPPort = "5022";
        private string CONST_SFTPLogin = "JuniperSAPProd";
        private string CONST_SFTPPassword = "3327Wl1m!";
        #endregion

        #region B2B SAP sftp Test
        //private string CONST_SFTPHost_Test = "10.191.23.14";
        //private string CONST_SFTPPort_Test = "443";
        //private string CONST_SFTPLogin_Test = "JuniperSFCTest";
        //private string CONST_SFTPPassword_Test = "52V1nR3S!";
        #endregion

        private SqlSugarClient SFCDB = null;
        public AnalyseSAPFile(string mesDBStr, string buStr, string filePath, string backupPath, string errorPath, string remotePath, string keyPath, string fileType, string sendFlag, string sendFilePath, string sendRemotePath, string testKeyPath, string remotePath2)
        {
            _mesDBStr = mesDBStr;
            _buStr = buStr;
            _filePath = filePath;
            _backupPath = backupPath;
            _errorPath = errorPath;
            _remotePath = remotePath;
            _keyPath = keyPath;
            _fileType = fileType;
            _sendFalg = sendFlag;
            _sendFilePath = sendFilePath;
            _sendRemotePath = sendRemotePath;
            _testKeyPath = testKeyPath;
            _remotePath2 = remotePath2;

            SFCDB = OleExec.GetSqlSugarClient(_mesDBStr, false);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            if (!Directory.Exists(errorPath))
            {
                Directory.CreateDirectory(errorPath);
            }
            if (sendFilePath != "" && !Directory.Exists(sendFilePath))
            {
                Directory.CreateDirectory(sendFilePath);
            }
        }

        public void Run()
        {
            Download();
            switch (_fileType)
            {
                case "I605":
                    AnalyesI605();
                    if (_sendFalg == "Y")
                    {
                        GetI605APData();
                        SendI605DataToB2B();
                    }
                    DeleteI605File();
                    break;
                case "I590":
                    AnalyesI590();
                    break;
                default:
                    break;
            }
        }

        public void Download()
        {
            SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keyPath, KeyType.RSA_PRIVATE);
            var downloadFiles = sftpHelp.ListDirectory(_remotePath).Where(r => r.LastAccessTime.CompareTo(System.DateTime.Now.AddDays(-13)) >= 0).ToList();
            foreach (var file in downloadFiles)
            {
                if (!SFCDB.Queryable<R_SAP_FILE>().Any(r => r.FILE_NAME == file.Name))
                {
                    try
                    {
                        sftpHelp.Get(file.FullName, $@"{_filePath}\{file.Name}");
                    }
                    catch (Exception ex)
                    {
                        MesLog.Info($"Download {file.Name} fail.Error message:{ex.Message}");
                    }
                }
            }

            if (!string.IsNullOrEmpty(_remotePath2))
            {
                downloadFiles = sftpHelp.ListDirectory(_remotePath2).Where(r => r.LastAccessTime.CompareTo(System.DateTime.Now.AddDays(-3)) >= 0).ToList();
                foreach (var file in downloadFiles)
                {
                    if (!file.Name.ToUpper().EndsWith(".XML")) continue;
                    if (!SFCDB.Queryable<R_SAP_FILE>().Any(r => r.FILE_NAME == file.Name))
                    {
                        try
                        {
                            sftpHelp.Get(file.FullName, $@"{_filePath}\{file.Name}");
                        }
                        catch (Exception ex)
                        {
                            MesLog.Info($"Download {file.Name} fail.Error message:{ex.Message}");
                        }
                    }
                }
            }
        }

        public void AnalyesI605()
        {
            FileInfo file;
            DirectoryInfo dirInfo = new DirectoryInfo(_filePath);
            DateTime sysdate = SFCDB.GetDate();
            foreach (var item in dirInfo.GetFileSystemInfos())
            {
                if (item is FileInfo)
                {
                    file = null;
                    SFCDB.Ado.BeginTran();
                    try
                    {
                        file = (FileInfo)item;
                        //解析XML文件
                        XmlDocument xml = new XmlDocument();
                        xml.Load(file.FullName);
                        //xml.InnerXml
                        StringReader stream = null;
                        XmlTextReader reader = null;
                        DataSet xmlDS = new DataSet();
                        stream = new StringReader(xml.InnerXml);
                        reader = new XmlTextReader(stream);
                        xmlDS.ReadXml(reader);
                        reader.Close();
                        DataTable dt = xmlDS.Tables[0];
                        if (dt.Rows.Count == 0)
                        {
                            throw new Exception($@"No data in {file.Name}");
                        }
                        #region R_SAP_FILE
                        var dateKey = file.Name.Split('_');
                        R_SAP_FILE sapFile = new R_SAP_FILE();
                        sapFile.ID = MesDbBase.GetNewID<R_SAP_FILE>(SFCDB, _buStr);
                        sapFile.FILE_NAME = file.Name;
                        sapFile.REMOTE_FILE_PATH = $@"{_remotePath}/{file.Name}";
                        sapFile.LOCAL_FILE_PATH = $@"{_backupPath}\{file.Name}";
                        sapFile.TYPE = "I605";
                        sapFile.DETAIL_TABLE = "R_SAP_FILE_I605";
                        sapFile.ANALYSIS_FLAG = "N";
                        sapFile.DATA_KEY = dateKey.Length > 1 ? $@"{dateKey[0]}{dateKey[1]}" : "";
                        DateTime dataTime;
                        if (DateTime.TryParseExact(sapFile.DATA_KEY, "yyyyMMddhhmmss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dataTime))
                        {
                            sapFile.DATA_TIME = dataTime;
                        }
                        sapFile.MEMO = "SAP I605 Data";
                        sapFile.CREATE_TIME = sysdate;
                        sapFile.CREATE_NAME = "SYSTEM";
                        SFCDB.Insertable<R_SAP_FILE>(sapFile).ExecuteCommand();
                        #endregion

                        #region R_SAP_FILE_I605
                        List<R_SAP_FILE_I605> listI605 = ObjectDataHelper.FromTable<R_SAP_FILE_I605>(dt);
                        listI605.ForEach((i605) =>
                        {
                            i605.ID = MesDbBase.GetNewID<R_SAP_FILE_I605>(SFCDB, _buStr);
                            i605.FILE_ID = sapFile.ID;
                        });
                        SFCDB.Insertable<R_SAP_FILE_I605>(listI605).ExecuteCommand();
                        #endregion

                        sapFile.ANALYSIS_FLAG = "Y";
                        SFCDB.Updateable<R_SAP_FILE>(sapFile).Where(r => r.ID == sapFile.ID).ExecuteCommand();

                        SFCDB.Ado.CommitTran();
                        MoveFile(_filePath, _backupPath, file.Name);
                    }
                    catch (Exception ex)
                    {
                        SFCDB.Ado.RollbackTran();
                        MoveFile(_filePath, _errorPath, item.Name);
                        MesLog.Info($"Analyes {item.Name} fail.Error message:{ex.Message}");
                    }
                }
            }
        }

        public void MoveFile(string oldPath, string newPath, string fileName)
        {
            try
            {
                if (File.Exists($@"{newPath}\{fileName}"))
                {
                    File.Move($@"{oldPath}\{fileName}", $@"{newPath}\{DateTime.Now.ToString("yyyyMMddHHmmss")}_{fileName}");
                }
                else
                {
                    File.Move($@"{oldPath}\{fileName}", $@"{newPath}\{fileName}");
                }
            }
            catch
            {
            }
        }

        public void GetI605APData()
        {
            var rSapFile = SFCDB.Queryable<R_SAP_FILE>().Where(t => t.TYPE == "I605" && t.ANALYSIS_FLAG == "Y" && t.DATA_TIME > SFCDB.GetDate().AddDays(-14)).OrderBy(t => t.DATA_KEY).ToList();
            foreach (var r in rSapFile)
            {
                if (!SFCDB.Queryable<R_SAP_FILE_SEND>().Where(t => t.TYPE == r.TYPE && t.FILE_ID == r.ID).Any())
                {
                    MES_DCN.Juniper.JuniperI605Format I605 = new MES_DCN.Juniper.JuniperI605Format(SFCDB);
                    DataTable dt = I605.JuniperI605FormatTable(r.ID);
                    dt.Columns.Remove("ITEM");

                    // 序列化
                    XmlDocument Xdoc = new XmlDocument();
                    XmlDeclaration dec = Xdoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    Xdoc.AppendChild(dec);
                    var eld = Xdoc.CreateElement("ns0", "MT_OnHandMPN_FXCNN", "urn:juniper.net:IBP:PTP:CM:OracleSupplyCloud:OnHand:I605");
                    Xdoc.AppendChild(eld);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var el = Xdoc.CreateElement("Records");
                        eld.AppendChild(el);
                        foreach (var col in dt.Columns)
                        {
                            var eli = Xdoc.CreateElement(col.ToString());
                            if (dr[col.ToString()] != DBNull.Value)
                                eli.InnerText = dr[col.ToString()].ToString();
                            el.AppendChild(eli);
                        }
                    }
                    var f1 = $@"{_sendFilePath}\{r.FILE_NAME}";
                    Xdoc.Save(f1);

                    #region R_SAP_FILE_SEND
                    R_SAP_FILE_SEND sapFileSend = new R_SAP_FILE_SEND();
                    sapFileSend.ID = MesDbBase.GetNewID<R_SAP_FILE_SEND>(SFCDB, _buStr);
                    sapFileSend.FILE_ID = r.ID;
                    sapFileSend.GET_DATA = "Y";
                    sapFileSend.SEND_DATA = "N";
                    sapFileSend.GET_TIME = SFCDB.GetDate();
                    sapFileSend.LOCAL_FILE_PATH = f1;
                    sapFileSend.TYPE = r.TYPE;
                    sapFileSend.MEMO = "從SAP給的I605文件取得AllPart相應數據傳送給客戶";
                    SFCDB.Insertable<R_SAP_FILE_SEND>(sapFileSend).ExecuteCommand();
                    #endregion
                }
            }
        }

        public void SendI605DataToB2B()
        {
            //如果TEST_KEY_PATH有配，就送到測試環境，為空就送到正式環境
            //SFTPHelper sftpHelp = null;
            //if (string.IsNullOrEmpty(_testKeyPath))
            //{
            //    sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keyPath, KeyType.RSA_PRIVATE);
            //}
            //else
            //{
            //    sftpHelp = new SFTPHelper(CONST_SFTPHost_Test, CONST_SFTPPort_Test, CONST_SFTPLogin_Test, CONST_SFTPPassword_Test, _testKeyPath, KeyType.RSA_PRIVATE);
            //}

            int ii = 0;
            SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keyPath, KeyType.RSA_PRIVATE);
            var rSapFileSend = SFCDB.Queryable<R_SAP_FILE_SEND>().Where(t => t.TYPE == "I605" && t.GET_DATA == "Y" && t.SEND_DATA == "N").OrderBy(t => t.LOCAL_FILE_PATH, OrderByType.Desc).ToList();
            foreach (var r in rSapFileSend)
            {
                var f2 = $@"{_sendRemotePath}/{r.LOCAL_FILE_PATH.Substring(r.LOCAL_FILE_PATH.LastIndexOf("\\") + 1)}";
                if (ii == 0)
                {
                    sftpHelp.Put(r.LOCAL_FILE_PATH, f2);
                }
                else
                {
                    r.MEMO += "-Old Data No Send";
                }
                #region R_SAP_FILE_SEND
                r.SEND_DATA = "Y";
                r.SEND_TIME = SFCDB.GetDate();
                r.REMOTE_FILE_PATH = f2;
                SFCDB.Updateable<R_SAP_FILE_SEND>(r).Where(t => t.ID == r.ID).ExecuteCommand();
                #endregion
                ii++;
            }
        }

        public void AnalyesI590()
        {
            FileInfo file;
            DirectoryInfo dirInfo = new DirectoryInfo(_filePath);
            DateTime sysdate = SFCDB.GetDate();
            foreach (var item in dirInfo.GetFileSystemInfos())
            {
                if (item is FileInfo)
                {
                    file = null;
                    SFCDB.Ado.BeginTran();
                    try
                    {
                        file = (FileInfo)item;
                        //解析XML文件
                        XmlDocument xml = new XmlDocument();
                        xml.Load(file.FullName);
                        //xml.InnerXml
                        StringReader stream = null;
                        XmlTextReader reader = null;
                        DataSet xmlDS = new DataSet();
                        stream = new StringReader(xml.InnerXml);
                        reader = new XmlTextReader(stream);
                        xmlDS.ReadXml(reader);
                        reader.Close();
                        DataTable dt = xmlDS.Tables[0];
                        if (dt.Rows.Count == 0)
                        {
                            throw new Exception($@"No data in {file.Name}");
                        }

                        #region R_SAP_FILE
                        var dateKey = file.Name.Split('_');
                        R_SAP_FILE sapFile = new R_SAP_FILE();
                        sapFile.ID = MesDbBase.GetNewID<R_SAP_FILE>(SFCDB, _buStr);
                        sapFile.FILE_NAME = file.Name;
                        sapFile.REMOTE_FILE_PATH = $@"{_remotePath}/{file.Name}";
                        sapFile.LOCAL_FILE_PATH = $@"{_backupPath}\{file.Name}";
                        sapFile.TYPE = "I590";
                        sapFile.DETAIL_TABLE = "R_SAP_FILE_I590";
                        sapFile.ANALYSIS_FLAG = "N";
                        sapFile.DATA_KEY = dateKey.Length > 1 ? $@"{dateKey[0]}{dateKey[1]}" : "";
                        DateTime dataTime;
                        if (DateTime.TryParseExact(sapFile.DATA_KEY, "yyyyMMddhhmmss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dataTime))
                        {
                            sapFile.DATA_TIME = dataTime;
                        }
                        sapFile.MEMO = "SAP I590 Data";
                        sapFile.CREATE_TIME = sysdate;
                        sapFile.CREATE_NAME = "SYSTEM";
                        SFCDB.Insertable<R_SAP_FILE>(sapFile).ExecuteCommand();
                        #endregion

                        #region R_SAP_FILE_I590

                        #region 方法一
                        List<R_SAP_FILE_I590> listI590 = ObjectDataHelper.FromTable<R_SAP_FILE_I590>(dt);
                        List<R_SAP_FILE_I590> listI590Insert = new List<R_SAP_FILE_I590>();
                        List<R_SAP_FILE_I590> listI590Update = new List<R_SAP_FILE_I590>();
                        List<string> nameListAdd = new List<string>();
                        List<string> nameList = listI590.Select(i => i.ITEM_NAME).ToList().Distinct().ToList();
                        List<R_SAP_FILE_I590> listI590Table = SFCDB.Queryable<R_SAP_FILE_I590>().Where(t => IMesDbEx.OracleContain(t.ITEM_NAME, nameList)).ToList();
                        listI590.ForEach((i590) =>
                        {
                            i590.FILE_ID = sapFile.ID;
                            var existsi590 = listI590Table.FirstOrDefault(i => i.ITEM_NAME == i590.ITEM_NAME);
                            if (existsi590 != null)
                            {
                                if (!listI590Table.Any(i => i.GROUP_CODE == i590.GROUP_CODE
                                                            && i.SUPPLIER_SITE == i590.SUPPLIER_SITE
                                                            && i.CM_PART_NUMBER == i590.CM_PART_NUMBER
                                                            && i.ITEM_NAME == i590.ITEM_NAME
                                                            && i.ITEM_TYPE == i590.ITEM_TYPE
                                                            && i.UOM == i590.UOM
                                                            && i.MAKE_BUY_FLAG == i590.MAKE_BUY_FLAG
                                                            && i.PHANTOM_FLAG == i590.PHANTOM_FLAG
                                                            && i.KANBAN_FLAG == i590.KANBAN_FLAG
                                                            && i.ROP_FLAG == i590.ROP_FLAG
                                                            && i.ROP_QUANTITY == i590.ROP_QUANTITY
                                                            && i.SAFETY_STOCK == i590.SAFETY_STOCK
                                                            && i.MOQ == i590.MOQ
                                                            && i.PURCHASING_LEAD_TIME == i590.PURCHASING_LEAD_TIME
                                                            && i.MFG_CYCLE_TIME == i590.MFG_CYCLE_TIME
                                                            && i.RECOMMENDED_ORDER_QTY == i590.RECOMMENDED_ORDER_QTY
                                                            && i.ABC_CODE == i590.ABC_CODE
                                                            && i.NCNR_FLAG == i590.NCNR_FLAG
                                                            && i.CURRENT_REV == i590.CURRENT_REV
                                                            && i.CM_STD_COST == i590.CM_STD_COST
                                                            && i.ORDER_MULTIPLE == i590.ORDER_MULTIPLE
                                                            && i.CYCLE_TIME_TO_REPLENISH == i590.CYCLE_TIME_TO_REPLENISH
                                                            && i.DESCRIPTION == i590.DESCRIPTION
                                                            && i.JUNIPER_LIABILITY_INDICATOR == i590.JUNIPER_LIABILITY_INDICATOR
                                                            && i.NON_BOM_INDICATOR == i590.NON_BOM_INDICATOR))
                                {
                                    i590.ID = existsi590.ID;
                                    i590.CREATE_TIME = existsi590.CREATE_TIME;
                                    i590.LAST_UPDATE_TIME = SFCDB.GetDate();
                                    listI590Update.Add(i590);
                                }
                            }
                            else
                            {
                                if (!nameListAdd.Contains(i590.ITEM_NAME))
                                {
                                    i590.ID = MesDbBase.GetNewID<R_SAP_FILE_I590>(SFCDB, _buStr);
                                    i590.CREATE_TIME = SFCDB.GetDate();
                                    listI590Insert.Add(i590);
                                }
                            }
                            nameListAdd.Add(i590.ITEM_NAME);
                        });
                        if (listI590Insert.Count > 0) SFCDB.Insertable(listI590Insert).ExecuteCommand();
                        if (listI590Update.Count > 0) SFCDB.Updateable(listI590Update).ExecuteCommand();
                        #endregion

                        #region 方法二
                        //List<R_SAP_FILE_I590> listI590 = ObjectDataHelper.FromTable<R_SAP_FILE_I590>(dt);
                        //List<string> nameList = listI590.Select(i => i.ITEM_NAME).ToList().Distinct().ToList();
                        //List<R_SAP_FILE_I590> listI590Table = SFCDB.Queryable<R_SAP_FILE_I590>().Where(t => IMesDbEx.OracleContain(t.ITEM_NAME, nameList)).ToList();
                        //listI590.ForEach((i590) =>
                        //{
                        //    i590.ID = MesDbBase.GetNewID<R_SAP_FILE_I590>(SFCDB, _buStr);
                        //    i590.FILE_ID = sapFile.ID;
                        //    i590.CREATE_TIME = SFCDB.GetDate();
                        //    i590.LAST_UPDATE_TIME = SFCDB.GetDate();
                        //});
                        //if (listI590Table.Count > 0)
                        //{
                        //    SFCDB.Deleteable<R_SAP_FILE_I590>(listI590Table).ExecuteCommand();
                        //}
                        //SFCDB.Insertable<R_SAP_FILE_I590>(listI590).ExecuteCommand();
                        #endregion

                        #endregion

                        sapFile.ANALYSIS_FLAG = "Y";
                        SFCDB.Updateable<R_SAP_FILE>(sapFile).Where(r => r.ID == sapFile.ID).ExecuteCommand();

                        SFCDB.Ado.CommitTran();
                        MoveFile(_filePath, _backupPath, file.Name);
                    }
                    catch (Exception ex)
                    {
                        SFCDB.Ado.RollbackTran();
                        MoveFile(_filePath, _errorPath, item.Name);
                        MesLog.Info($"Analyes {item.Name} fail.Error message:{ex.Message}");
                    }
                }
            }
        }

        public void DeleteI605File()
        {
            var i605 = SFCDB.Queryable<R_SAP_FILE>().Where(t => t.TYPE == "I605" && t.DATA_TIME < SFCDB.GetDate().AddDays(-15)).ToList();
            SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keyPath, KeyType.RSA_PRIVATE);
            var downloadFiles = sftpHelp.ListDirectory(_remotePath).ToList();
            foreach (var file in downloadFiles)
            {
                if (i605.Where(t => t.FILE_NAME == file.Name).Any())
                {
                    try
                    {
                        sftpHelp.Delete(file.FullName);
                    }
                    catch (Exception ex)
                    {
                        MesLog.Info($"Delete B2B File {file.Name} fail.Error message:{ex.Message}");
                    }
                }
            }
        }

    }
}
