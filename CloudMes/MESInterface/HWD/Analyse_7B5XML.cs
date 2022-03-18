using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESDBHelper;
using MESInterface.Common;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MESInterface.HWD
{
    public class Analyse_7B5XML : taskBase
    {
        public string ftpPath = "";
        public string ftpUser = "";
        public string ftpPwd = "";
        public string savePath = "";
        public string backupPath = "";
        public string errorPath = "";
        public string saprfc = "ZCPP_NSBG_0121";
        public string message = "";
        public string dbConnet = "";
        public string apConnet = "";
        public string hwtConnet = "";
        public string ip = "";
        public OleExec SFCDB = null;
        public OleExec APDB = null;
        public OleExec HWT_SFCDB = null;
        public T_R_SYNC_LOCK TRSL;
        public DB_TYPE_ENUM DBTYPE = DB_TYPE_ENUM.Oracle;
        public string[] analyMail;
        public string[] qiTaoMail;

        private bool IsRuning;
        private string BU = "HWD";
        private clsFTP cf;
        private FileHelp fileHelp;
        private string todayDir = "";
        private int pass_qty = 0;
        private int fail_qty = 0;
        private int total_qty = 0;
        private string strpass = "";
        private string strfail = ""; 

        public override void init()
        {
            //base.init();
            Output.UI = new Analyse_7B5XML_UI(this);
            try
            {
                ZCPP_NSBG_0121 zcpp_nsbg_0121 = new ZCPP_NSBG_0121(BU);
                ftpPath = ConfigGet("FTP_PATH");
                ftpUser = ConfigGet("FTP_USER");
                ftpPwd = ConfigGet("FTP_PWD");
                savePath = ConfigGet("SAVE_PATH");
                backupPath = ConfigGet("BACKUP_PATH");
                errorPath = ConfigGet("ERROR_PATH");
                analyMail = ConfigGet("ANALY_MAIL").Split(',');
                qiTaoMail = ConfigGet("QITAO_MAIL").Split(',');
                dbConnet = ConfigGet("DB");
                apConnet = ConfigGet("APDB");
                hwtConnet = ConfigGet("HWTDB");
                fileHelp = new FileHelp();
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Output.UI.Refresh();
            }
        }
        public override void Start()
        {
            //base.Start();
            APDB = new OleExec(apConnet, false);
            SFCDB = new OleExec(dbConnet, false);
            HWT_SFCDB = new OleExec(hwtConnet, false);
            TRSL = new T_R_SYNC_LOCK(SFCDB, DBTYPE);
            string lockIp = "";
            todayDir = DateTime.Now.ToString("yyyyMMdd");
            pass_qty = 0;
            fail_qty = 0;
            total_qty = 0;
            strpass = "";
            strfail = "";

            IsRuning = TRSL.IsLock("HWD_ANALYSE_7B5XML", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception("HWD ANALYSE 7B5XML Interface Is Running On " + lockIp + ",Please Try Again Later!");
            }
            TRSL.SYNC_Lock(BU, ip, "HWD_ANALYSE_7B5XML", "HWD_ANALYSE_7B5XML", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
            try
            {
                //UpdateWOType();
                //1.從ftp上下載xml文件到本機  
                RecodeLocalLog("");
                RecodeLocalLog("Begin Download XML");
                DownXML();
                RecodeLocalLog("End Download XML");

                //2.解析xml文件
                RecodeLocalLog("Begin Analyse XML");
                GetFileList(savePath, backupPath, errorPath);
                RecodeLocalLog("End Analyse XML");

                //3.Call SAP
                RecodeLocalLog("Begin Call SAP");
                CallSAP();
                RecodeLocalLog("End Call SAP");
            }
            catch (Exception ex)
            {
                message = ex.Message;
                //Output.UI.Refresh();
                throw ex;
            }
            finally
            {
                TRSL.SYNC_UnLock(BU, ip, "HWD_ANALYSE_7B5XML", "HWD_ANALYSE_7B5XML", "interface", SFCDB, DBTYPE);
            }
        }
        /// <summary>
        /// 從FTP上下載XML到本地電腦
        /// </summary>
        private void DownXML()
        {
            try
            {                
                //cf = new clsFTP(new Uri("FTP://10.120.198.96/HWD7B5XML/"), "HWD7B5", "HWD7B5");
                cf = new clsFTP(new Uri(ftpPath), ftpUser, ftpPwd);
                FileStruct[] listAll = cf.ListFiles();
                foreach (FileStruct file1 in listAll)
                {
                    try
                    {
                        if (file1.Name.Substring(file1.Name.Length - 3, 3) == "xml")
                        {
                            if (!File.Exists(savePath + "\\" + file1.Name))
                            {
                                cf.DownloadFile(file1.Name, savePath);                                
                                if (File.Exists(savePath + "\\" + file1.Name))
                                {
                                    RecodeLocalLog("XML File:" + file1.Name + " Download OK!");
                                    cf.DeleteFile(file1.Name);                                   
                                    RecodeLocalLog("XML File:" + file1.Name + " Delete OK!");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        RecodeLocalLog("XML File:" + file1.Name + " Download Or Delete Fail:" + ex.Message);
                    }                    
                }
            }
            catch (Exception ex)
            {               
                RecodeLocalLog("Open FTP Fail:" + ex.Message);
            }
        }

        private void GetFileList(string strCurDir, string strBakDir, string strErrorDir)
        {
            string SourceFull_FileName = "";
            string FileName = "";
            string FileExt = "";         
            long FileSize;
            FileInfo fi;
            DirectoryInfo dir;
            DateTime FileModify;
            DirectoryInfo dirInfo = new DirectoryInfo(strCurDir);

            foreach (FileSystemInfo fsi in dirInfo.GetFileSystemInfos())
            {
                try
                {
                    if (fsi is FileInfo)
                    {
                        fi = (FileInfo)fsi;
                        //取得文件名
                        FileName = fi.Name;
                        //取得文件的扩展名
                        FileExt = fi.Extension;
                        //取得文件的大小
                        FileSize = fi.Length;
                        //取得文件的最后修改时间
                        FileModify = fi.LastWriteTime;
                        //獲取完整的文件名與目錄
                        SourceFull_FileName = strCurDir + "\\" + FileName;
                        total_qty++;
                        //解析XML文件
                        XML_Analyse(SourceFull_FileName, FileName, strCurDir, strBakDir, strErrorDir);
                    }
                    else
                    {
                        dir = (DirectoryInfo)fsi;
                        //取得目录名
                        FileName = dir.Name;
                        //取得目录的最后修改时间
                        FileModify = dir.LastWriteTime;
                        //设置文件的扩展名为"文件夹"
                        FileExt = "文件夹";
                        //获取文件夹路径
                        GetFileList(strCurDir + "\\" + dir.Name, strBakDir, strErrorDir);
                    }
                }
                catch (Exception ex)
                {
                    RecodeLocalLog("GetFileList Error:" + FileName + "," + ex.Message);
                }
            }
        }

        //解析XML主函數
        private void XML_Analyse(string FullFileName, string FileName, string strCurDir, string strBakDir, string strErrorDir)
        {
            XmlDocument xml = new XmlDocument();
            //Task no 公共的值
            string Ems_Code = "";
            string WO = "";
            string WO_Item = "";
            string Supply_Type = "";
            string Po_Supply_Information = "";
            string Change_Information = "";
            string Version = "";
            string ROHS = "";
            string Qty = "";
            string Complete_Date = "";
            string Start_Date = "";
            string Publish_Time = "";
            string Release_Date = "";
            string Remark = "";
            //Task no 非公共的值
            string StrComponent = "";
            string Component_Version = "";
            string Component_Qty = "";
            string Component_Remark = "";
            string Product_line = "";
            string Model = "";
            string Category = "";
            string Description = "";
            int index = 0;

            string strsql = "";
            string strResult = "";
            string DestFullFileName = "";
            string DestFullDir = "";

            try
            {

                xml.Load(FullFileName);

                //////////*******下面开始循环读取xml文件信息********/
                //獲取同一個Task No公共變量值部份，
                foreach (XmlNode node in xml.ChildNodes)
                {

                    if (node.Name == "Pip7B5NotifyOfManufacturingWorkOrder")
                    {
                        foreach (XmlNode node1 in node.ChildNodes)
                        {
                            if (node1.Name == "toRole")
                            {
                                foreach (XmlNode node2 in node1.ChildNodes)
                                {
                                    if (node2.Name == "PartnerRoleDescription")
                                    {
                                        foreach (XmlNode node3 in node2.ChildNodes)
                                        {
                                            if (node3.Name == "ContactInformation")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "contactName")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "FreeFormText")
                                                            {
                                                                Ems_Code = node5.InnerText;
                                                            }

                                                        }

                                                    }
                                                }

                                            }
                                        }

                                    }

                                }
                            }
                            else if (node1.Name == "WorkOrder")
                            {
                                foreach (XmlNode node2 in node1.ChildNodes)
                                {
                                    if (node2.Name == "comment")
                                    {
                                        foreach (XmlNode node3 in node2.ChildNodes)
                                        {
                                            if (node3.Name == "FreeFormText")
                                            {
                                                Change_Information = node3.InnerText;
                                            }
                                        }
                                    }
                                    else if (node2.Name == "WorkOrderNumber")
                                    {
                                        WO = node2.InnerText;
                                    }
                                    else if (node2.Name == "RevisionNumber")
                                    {
                                        Version = node2.InnerText;
                                        //index = Version.IndexOf("%");
                                        //Description = Version.Substring(index + 1, Version.Length - index - 1);
                                        //Version = Version.Substring(0, index);
                                    }
                                    else if (node2.Name == "WorkOrderLineItems")
                                    {
                                        foreach (XmlNode node3 in node2.ChildNodes)
                                        {
                                            if (node3.Name == "comment")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "FreeFormText")
                                                    {
                                                        Remark = node4.InnerText;

                                                        index = Remark.IndexOf("%");
                                                        //Product_line = Remark.Substring(0, index);
                                                        Product_line = ToTraditional(Remark.Substring(0, index));
                                                        Remark = Remark.Substring(index + 1, Remark.Length - index - 1);

                                                        index = Remark.IndexOf("%");
                                                        Model = Remark.Substring(0, index);
                                                        Remark = Remark.Substring(index + 1, Remark.Length - index - 1);

                                                        index = Remark.IndexOf("%");
                                                        Category = Remark.Substring(0, index);
                                                        Remark = Remark.Substring(index + 1, Remark.Length - index - 1);
                                                    }
                                                }
                                            }
                                            else if (node3.Name == "CustomerProfile")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "lineItemQuantity")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "ProductQuantity")
                                                            {
                                                                Qty = node5.InnerText;
                                                            }
                                                        }
                                                    }
                                                    else if (node4.Name == "requestedEvent")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "DateStamp")
                                                            {
                                                                Complete_Date = node5.InnerText;
                                                            }
                                                        }
                                                    }
                                                    else if (node4.Name == "requestedStartDate")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "DateStamp")
                                                            {
                                                                Start_Date = node5.InnerText;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (node3.Name == "dateCode")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "FreeFormText")
                                                    {
                                                        Publish_Time = node4.InnerText;
                                                    }
                                                }
                                            }
                                            else if (node3.Name == "DocumentReference")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "DateTimeStamp")
                                                    {
                                                        Release_Date = node4.InnerText;
                                                    }
                                                }
                                            }
                                            else if (node3.Name == "SpecialProcessingInformation")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "specialInstructions")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "FreeFormText")
                                                            {
                                                                ROHS = node5.InnerText;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (node3.Name == "TargetDevice")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "customerTargetDevice")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "ProductIdentification")
                                                            {
                                                                foreach (XmlNode node6 in node5.ChildNodes)
                                                                {
                                                                    if (node6.Name == "PartnerProductIdentification")
                                                                    {
                                                                        foreach (XmlNode node7 in node6.ChildNodes)
                                                                        {
                                                                            if (node7.Name == "ProprietaryProductIdentifier")
                                                                            {
                                                                                WO_Item = node7.InnerText;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                    else if (node4.Name == "targetDeviceLotNumber")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "ProprietaryLotIdentifier")
                                                            {
                                                                Supply_Type = node5.InnerText;
                                                            }
                                                        }
                                                    }
                                                    else if (node4.Name == "customerLotNumber")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "ProprietaryLotIdentifier")
                                                            {
                                                                Po_Supply_Information = node5.InnerText;
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }


                                    }
                                }
                            }

                        }
                    }

                }
                //FORMAT DATE CHAR
                Start_Date = Start_Date.Replace("T", "");
                Start_Date = Start_Date.Replace("Z", "");

                Complete_Date = Complete_Date.Replace("T", "");
                Complete_Date = Complete_Date.Replace("Z", "");

                //delete task no R_7B5_XML_T_tmp data 
                SFCDB.ORM.Deleteable<R_7B5_XML_T_TMP>().Where(r => r.TASK_NO == WO).ExecuteCommand();

                //獲取同一個Task No不同變量值部份，主要是Component相關信息
                foreach (XmlNode node in xml.ChildNodes)
                {

                    if (node.Name == "Pip7B5NotifyOfManufacturingWorkOrder")
                    {
                        foreach (XmlNode node1 in node.ChildNodes)
                        {
                            if (node1.Name == "WorkOrder")
                            {
                                foreach (XmlNode node2 in node1.ChildNodes)
                                {
                                    if (node2.Name == "WorkOrderLineItems")
                                    {
                                        foreach (XmlNode node3 in node2.ChildNodes)
                                        {
                                            if (node3.Name == "SourceDevice")
                                            {
                                                foreach (XmlNode node4 in node3.ChildNodes)
                                                {
                                                    if (node4.Name == "ProductIdentification")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "PartnerProductIdentification")
                                                            {
                                                                foreach (XmlNode node6 in node5.ChildNodes)
                                                                {
                                                                    if (node6.Name == "ProprietaryProductIdentifier")
                                                                    {
                                                                        StrComponent = node6.InnerText;
                                                                    }
                                                                    else if (node6.Name == "revisionIdentifier")
                                                                    {
                                                                        foreach (XmlNode node7 in node6.ChildNodes)
                                                                        {
                                                                            if (node7.Name == "FreeFormText")
                                                                            {
                                                                                Component_Version = node7.InnerText;
                                                                                index = Component_Version.IndexOf("%");
                                                                                //Description = Component_Version.Substring(index + 1, Component_Version.Length - index - 1);
                                                                                Description = ToTraditional(Component_Version.Substring(index + 1, Component_Version.Length - index - 1));
                                                                                Component_Version = Component_Version.Substring(0, index);
                                                                            }

                                                                        }

                                                                    }
                                                                }

                                                            }
                                                        }

                                                    }
                                                    else if (node4.Name == "SourceLot")
                                                    {
                                                        foreach (XmlNode node5 in node4.ChildNodes)
                                                        {
                                                            if (node5.Name == "LotQuantity")
                                                            {
                                                                foreach (XmlNode node6 in node5.ChildNodes)
                                                                {
                                                                    if (node6.Name == "waferQuantity")
                                                                    {
                                                                        foreach (XmlNode node7 in node6.ChildNodes)
                                                                        {
                                                                            if (node7.Name == "ProductQuantity")
                                                                            {
                                                                                Component_Qty = node7.InnerText;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else if (node5.Name == "mappingFileName")
                                                            {
                                                                foreach (XmlNode node6 in node5.ChildNodes)
                                                                {
                                                                    if (node6.Name == "FreeFormText")
                                                                    {
                                                                        Component_Remark = node6.InnerText;
                                                                    }
                                                                }

                                                            }
                                                        }

                                                    }
                                                }

                                                //獲取到一行COMPONENT數據插入一行,insert data to R_7B5_XML_T_tmp
                                                strsql = "insert into  R_7B5_XML_T_tmp (EMS_CODE,TASK_NO,ITEM,SUPPLY_TYPE,PO_SUPPLY_INFORMATION," +
                                                       "CHANGE_INFORMATION ,ITEM_VERSION,ROHS,QTY,RELEASE_DATE,START_DATE,COMPLETE_DATE,TASK_NOREMARK," +
                                                       "COMPONENT,COMPONENT_VERSION,COMPONENT_QTY,COMPONENT_REMARK,PUBLISH_TIME ,LASTEDITDT,LASTEDITBY, " +
                                                       "Description,Product_line,Model,Category)  " +
                                                       "values ('" + Ems_Code + "','" + WO + "', '" + WO_Item + "', '" + Supply_Type + "'," +
                                                       "'" + Po_Supply_Information + "', '" + Change_Information + "', '" + Version + "', '" + ROHS + "'," +
                                                       "" + Qty + ", '" + Release_Date + "',to_date('" + Start_Date + "','YYYY/MM/DD HH24:MI:SS')+1/3," +
                                                       "to_date('" + Complete_Date + "','YYYY/MM/DD HH24:MI:SS')+1/3, '" + Remark + "'," +
                                                       "'" + StrComponent + "', '" + Component_Version + "'," + Component_Qty + "," +
                                                       "'" + Component_Remark + "',to_date('" + Publish_Time + "','YYYY/MM/DD HH24:MI:SS'),sysdate,'ADMIN'," +
                                                       "'" + Description + "', '" + Product_line + "', '" + Model + "', '" + Category + "') ";
                                                try
                                                {
                                                    SFCDB.ExecSQL(strsql);
                                                }
                                                catch (Exception ex)
                                                {                                                    
                                                    RecodeLocalLog("XML File:" + FileName + "Insert Into R_7B5_XML_T_TMP Fail," + WO + ";" + ex);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }               
                try
                {
                    MESStation.Config.HWD.C7B5Function.SaveCheck(SFCDB, APDB, DBTYPE, WO);
                    //獲取需要搬移的目錄                    
                    DestFullDir = strBakDir;
                    strResult = "OK";
                }
                catch (Exception ex)
                {
                    strResult = ex.Message;
                    DestFullDir = strErrorDir;
                }
                //記錄SP log
                //Recode_log(FileName, strResult);
                RecodeLocalLog("XML File:" + FileName + ";SaveCheck:" + strResult);
                //每天生成一個文件夾                
                DestFullDir = DestFullDir + "\\" + todayDir;
                //CHECK 移轉目錄是否存在，沒有則建立
                if (!Directory.Exists(DestFullDir))
                {
                    Directory.CreateDirectory(DestFullDir);
                }
                DestFullFileName = DestFullDir + "\\" + FileName;

                //搬移前check是否已存在此文件,
                if (File.Exists(DestFullFileName))
                {
                    DestFullFileName = DestFullFileName + '_' + DateTime.Now.ToString("yyyyMMddHHmmss"); 
                }
                //搬移文件
                File.Move(FullFileName, DestFullFileName);
                RecodeLocalLog("XML File:" + FileName + "Move To " + DestFullDir);
                RecodeLocalLog("XML File:" + FileName + "Analyse OK! ");
                pass_qty++;
                strpass = strpass + FileName + ";";
            }
            catch (Exception ex)
            {
                DestFullDir = strErrorDir;
                //每天生成一個文件夾               
                DestFullDir = DestFullDir + "\\" + todayDir;
                //CHECK 移轉目錄是否存在，沒有則建立
                if (!Directory.Exists(DestFullDir))
                {
                    Directory.CreateDirectory(DestFullDir);
                }
                DestFullFileName = DestFullDir + "\\" + FileName;
                //搬移前check是否已存在此文件,
                if (File.Exists(DestFullFileName))
                {
                    DestFullFileName = DestFullFileName + '_' + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                //搬移文件
                File.Move(FullFileName, DestFullFileName);
                RecodeLocalLog("XML File:" + FileName + "Move To " + DestFullDir);
                RecodeLocalLog("XML File:" + FileName + "Analyse Fail! " + ex.Message);
                fail_qty++;
                strfail = strfail + FileName + ";";
            }
        }

        private string ToTraditional(string source)
        {
            string target = new string(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
            return target;
        }
        private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
        private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int Locale, int dwMapFlags, string lpsrcStr, int cchSrc, [Out] string lpDdstStr, int cchDest);

        private void CallSAP()
        {
            try
            {
                string saveMsg = "", countMsg = "";
                try
                {
                    MESStation.Config.HWD.C7B5Function.SaveWOQty(SFCDB, DBTYPE,ip, ref saveMsg);
                }
                catch (Exception ex)
                {
                    saveMsg = ex.Message;
                }
                if (saveMsg != "")
                {
                    RecodeLocalLog("SaveMsg Error:" + saveMsg);
                }
                else
                {
                    RecodeLocalLog("SaveMsg OK!");
                }
                try
                {
                    MESStation.Config.HWD.C7B5Function.CountQty(SFCDB, DBTYPE, ref countMsg);
                }
                catch (Exception ex)
                {
                    countMsg = ex.Message;
                }
                if (countMsg != "")
                {
                    RecodeLocalLog("CountMsg Error:" + countMsg);
                }
                else
                {
                    RecodeLocalLog("CountMsg OK!");
                }
                //非正常任務令手工拋轉
                string sql = " select A.receive_date,A.task_no_type,A.task_no_use,A.sap_factory,A.v_task_no,A.HH_ITEM,A.hw_item,A.QTY,TO_DATE(A.complete_date,'YYYY/MM/DD') complete_date,"
                           + " A.task_no,A.main_wo_flag,a.TASK_NO_LEVEL,b.model  as b_model from r_7b5_wo_temp a,r_7b5_xml_t b where a.ZNP195_FLAG='N' and a.task_no=b.task_no"
                           + " and  (left(a.TASK_NO,2)='BS' OR SUBSTR(a.TASK_NO,3,1)='Z') and b.DESCRIPTION not like '%啞機%' and a.CREATE_TIME>sysdate-30 ";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                string result = "";
                string message = "";
                string ZNP195_result = "";
                string sqlMail = "";
                ZCPP_NSBG_0121 zcpp_nsbg_0121 = new ZCPP_NSBG_0121(BU);
                DataTable in_tab = new DataTable();
                DataTable out_tab = new DataTable();
                in_tab.Columns.Add("POSDT");
                in_tab.Columns.Add("TSORT");
                in_tab.Columns.Add("TSORU");
                in_tab.Columns.Add("TSORL");
                in_tab.Columns.Add("WERKS");
                in_tab.Columns.Add("EXTWG");
                in_tab.Columns.Add("TSORNUM");
                in_tab.Columns.Add("MATNR");
                in_tab.Columns.Add("HWDMAT");
                in_tab.Columns.Add("MAKTX");
                in_tab.Columns.Add("REQTY");
                in_tab.Columns.Add("REQDT");
                in_tab.Columns.Add("MEINS");
                in_tab.Columns.Add("HWPONU");
                in_tab.Columns.Add("FOXDES");
                in_tab.Columns.Add("HWDDES");
                in_tab.Columns.Add("REMARK");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            in_tab.Clear();
                            DataRow dr = in_tab.NewRow();
                            dr["POSDT"] = DateTime.Parse(dt.Rows[i]["receive_date"].ToString().ToUpper().Trim()).ToString("yyyyMMdd");
                            dr["TSORT"] = dt.Rows[i]["task_no_type"].ToString().ToUpper().Trim();
                            dr["TSORU"] = dt.Rows[i]["task_no_use"].ToString().ToUpper().Trim();
                            dr["TSORL"] = dt.Rows[i]["task_no_level"].ToString().ToUpper().Trim();
                            dr["WERKS"] = dt.Rows[i]["sap_factory"].ToString().ToUpper().Trim();
                            dr["EXTWG"] = dt.Rows[i]["b_model"].ToString().ToUpper().Trim();
                            dr["TSORNUM"] = dt.Rows[i]["v_task_no"].ToString().ToUpper().Trim();
                            dr["MATNR"] = dt.Rows[i]["HH_ITEM"].ToString().ToUpper().Trim();
                            dr["HWDMAT"] = dt.Rows[i]["hw_item"].ToString().ToUpper().Trim();
                            dr["MAKTX"] = "";
                            dr["REQTY"] = dt.Rows[i]["QTY"].ToString().ToUpper().Trim();
                            dr["REQDT"] = DateTime.Parse(dt.Rows[i]["complete_date"].ToString().ToUpper().Trim()).ToString("yyyyMMdd");
                            dr["MEINS"] = "";
                            dr["HWPONU"] = "";
                            //有子任務令備註原任務令;
                            if (!dt.Rows[i]["task_no"].ToString().ToUpper().Trim().Equals(dt.Rows[i]["v_task_no"].ToString().ToUpper().Trim()))
                            {
                                dr["FOXDES"] = dt.Rows[i]["task_no"].ToString().ToUpper().Trim();
                            }
                            else if (dt.Rows[i]["task_no"].ToString().ToUpper().Trim().Substring(3, 1) == "S")
                            {
                                dr["FOXDES"] = dt.Rows[i]["task_no"].ToString().ToUpper().Trim().Substring(0, 3) + "6" + dt.Rows[i]["task_no"].ToString().ToUpper().Trim().Substring(4, 7);
                            }
                            else
                            {
                                dr["FOXDES"] = "SFCUPLOAD";
                            }
                            dr["HWDDES"] = "";
                            if (dt.Rows[i]["main_wo_flag"].ToString().ToUpper().Trim().Equals("Y"))
                            {
                                dr["REMARK"] = "1";
                            }
                            else
                            {
                                dr["REMARK"] = "";
                            }
                            in_tab.Rows.Add(dr);
                            zcpp_nsbg_0121.SetValue(in_tab);
                            zcpp_nsbg_0121.CallRFC();
                            out_tab = zcpp_nsbg_0121.GetTableValue("OUT_TAB");
                            result = out_tab.Rows[0]["FLAG"].ToString();
                            message = out_tab.Rows[0]["MESS"].ToString();
                            if (result == "S")
                            {
                                sql = " UPDATE r_7b5_wo_temp SET ZNP195_MESSAGE='" + message + "',ZNP195_FLAG='Y' WHERE TASK_NO='" + dt.Rows[i]["task_no"].ToString().ToUpper().Trim() + "'"
                                    + " AND HH_ITEM='" + dt.Rows[i]["HH_ITEM"].ToString().ToUpper().Trim() + "' ";

                            }
                            else
                            {
                                sql = " UPDATE r_7b5_wo_temp SET ZNP195_MESSAGE='" + message + "' WHERE TASK_NO='" + dt.Rows[i]["task_no"].ToString().ToUpper().Trim() + "'"
                                    + " AND HH_ITEM='" + dt.Rows[i]["HH_ITEM"].ToString().ToUpper().Trim() + "' ";
                                ZNP195_result = ZNP195_result + dt.Rows[i]["v_task_no"].ToString() + "," + message + ";";
                            }
                            SFCDB.ExecSQL(sql);
                        }
                        catch (Exception ex)
                        {
                            ZNP195_result = ZNP195_result + dt.Rows[i]["v_task_no"].ToString() + "," + ex.Message + ";";
                        }
                    }
                }

                if (ZNP195_result.Length > 0)
                {
                    ZNP195_result = "以下任務令UPLOAD ZNP195失敗:" + ZNP195_result;
                    RecodeLocalLog("ZNP195 Result:" + ZNP195_result);
                }
                else
                {
                    ZNP195_result = "";
                    RecodeLocalLog("ZNP195 Result:OK!");
                }
                string mail_subject = "";
                string mail_content = "";
                if (total_qty != 0)
                {
                    mail_subject = "HW拋轉任務令 " + total_qty + "筆,解析成功 " + pass_qty + "筆,解析失敗 " + fail_qty + "筆";
                    mail_content = "解析成功的文件:" + strpass + "\r\n解析失敗的文件:" + strfail + "\r\n"
                        + (saveMsg == "" ? "" : "匹配富士康料號錯誤:" + saveMsg) + "\r\n" + ZNP195_result;
                    mail_content = mail_content.Length > 3500 ? mail_content.Substring(3500) : mail_content;
                    sqlMail = "INSERT INTO r_mail_t (mail_id,mail_to,mail_subject,mail_sequence,mail_content,mail_date) VALUES "
                        + "('HWD_7B5','mis-dcn-sfc@mail.foxconn.com,cony.by.wei@mail.foxconn.com,mary.mx.yin@mail.foxconn.com,rachle.qm.lei@mail.foxconn.com,jack.l.wu@mail.foxconn.com,"
                        + "jie-lin.qiu@mail.foxconn.com,hwd-nn-pesys@mail.foxconn.com','" + mail_subject + "','0','" + mail_content + "',sysdate)";
                    HWT_SFCDB.ExecSQL(sqlMail);
                    //if (analyMail.Length > 0)
                    //{
                    //    NotesService.LotusMail aMail = new NotesService.LotusMail()
                    //    {
                    //        MailTitle = mail_subject,
                    //        MailBody = mail_content,
                    //        MailTo = analyMail,
                    //        MailSave = false
                    //    };
                    //    SendMail(aMail);
                    //}                      
                }
                else
                {
                    mail_subject = (saveMsg == "" ? "" : "匹配富士康料號錯誤!") + (ZNP195_result == "" ? "" : "任務令UPLOAD ZNP195錯誤!");
                    mail_content = (saveMsg == "" ? "" : "匹配富士康料號錯誤:" + saveMsg) + "\r\n" + (ZNP195_result == "" ? "" : "任務令UPLOAD ZNP195錯誤!" + ZNP195_result);
                    mail_content = mail_content.Length > 3500 ? mail_content.Substring(3500) : mail_content;
                    if (mail_subject != "")
                    {
                        sqlMail = "INSERT INTO r_mail_t (mail_id,mail_to,mail_subject,mail_sequence,mail_content,mail_date) VALUES "
                            + "('HWD_7B5','mis-dcn-sfc@mail.foxconn.com,cony.by.wei@mail.foxconn.com,mary.mx.yin@mail.foxconn.com,rachle.qm.lei@mail.foxconn.com,jack.l.wu@mail.foxconn.com,"
                            + "jie-lin.qiu@mail.foxconn.com,hwd-nn-pesys@mail.foxconn.com','" + mail_subject + "','0','" + mail_content + "',sysdate)";
                        HWT_SFCDB.ExecSQL(sqlMail);
                        //if (analyMail.Length > 0)
                        //{
                        //    NotesService.LotusMail aMail = new NotesService.LotusMail()
                        //    {
                        //        MailTitle = mail_subject,
                        //        MailBody = mail_content,
                        //        MailTo = analyMail,
                        //        MailSave = false
                        //    };
                        //    SendMail(aMail);
                    }
                }               

                sql = "SELECT * FROM R_7B5_XML_T WHERE TASK_CHANGE_NO='與TE確認備品是否齊套' AND TRANSFER_FLAG='N' AND LASTEDITDT>SYSDATE-1/24";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mail_subject = "任務令" + dt.Rows[i]["task_no"].ToString() + "，機種 " + dt.Rows[i]["model"].ToString() + ",料號" + dt.Rows[i]["ITEM"].ToString() + "，請TE確認備品是否齊套";
                    mail_content = "";
                    RecodeLocalLog(mail_subject);
                    sqlMail = "INSERT INTO r_mail_t (mail_id,mail_to,mail_subject,mail_sequence,mail_content,mail_date) VALUES"
                        + " ('HWD_7B5','mis-dcn-sfc@mail.foxconn.com,shan-zhao.lv@mail.foxconn.com,yang.y.shi@mail.foxconn.com,hwd-nn-equipment@mail.foxconn.com,"
                        + "robert.lx.yu@mail.foxconn.com','" + mail_subject + "','0','" + mail_content + "',sysdate)";
                    HWT_SFCDB.ExecSQL(sqlMail);

                    //if (qiTaoMail.Length > 0)
                    //{
                    //    NotesService.LotusMail qMail = new NotesService.LotusMail()
                    //    {
                    //        MailTitle = mail_subject,
                    //        MailBody = mail_content,
                    //        MailTo = analyMail,
                    //        MailSave = false
                    //    };
                    //    SendMail(qMail);
                    //}
                }
            }
            catch (Exception ex)
            {
                RecodeLocalLog("CallSAP Error:" + ex.Message);
            }
        }

        private void SendMail(NotesService.LotusMail mail)
        {
            try
            {
                NotesService.LotusNotesService ln = new NotesService.LotusNotesService();
                ln.SendNotesMail(mail);
            }
            catch (Exception ex)
            {
                RecodeLocalLog("Mail:" + ex.Message);
            }
        }             

        private void WriteAP()
        {
            MESDataObject.Module.HWD.T_R_7B5_XML_T TRXT = new MESDataObject.Module.HWD.T_R_7B5_XML_T(SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_SKU TCS = new T_C_SKU(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SKU_ROUTE TRSR = new T_R_SKU_ROUTE(SFCDB, DB_TYPE_ENUM.Oracle);
            string sql = "";
            SFCDB.ThrowSqlExeception = true;
            APDB.ThrowSqlExeception = true;
            List<string> listTask = new List<string>() { "" };
            foreach (string task_no in listTask)
            {
                try
                {
                    MESDataObject.Module.HWD.R_7B5_XML_T objXMLT = TRXT.GetObjectByTaskNO(SFCDB, task_no);
                    if (objXMLT == null)
                    {
                        return;
                    }
                    List<C_SKU> listSku = SFCDB.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, "A")
                       && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && r.SKU_NAME == objXMLT.MODEL).ToList();

                    if (listSku.Count > 0 && !objXMLT.DESCRIPTION.Contains("板") && !objXMLT.DESCRIPTION.Contains("組件") && !objXMLT.DESCRIPTION.Contains("啞機") && !objXMLT.DESCRIPTION.Contains("元件"))
                    {
                        string sku_id = TCS.GetNewID("HWD", SFCDB);
                        string route_id = TRSR.GetNewID("HWD", SFCDB);
                        string temp_skuno = "A" + objXMLT.ITEM + "-A";

                        listSku = SFCDB.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == temp_skuno).ToList();
                        if (listSku.Count == 0)
                        {
                            sql = $@"INSERT INTO C_SKU (id, bu, skuno, version, sku_name, c_series_id, cust_partno, cust_sku_code, sn_rule, panel_rule, description, edit_emp, edit_time, sku_type, aqltype )
                              VALUES  ('{sku_id}', 'HWD', 'A{objXMLT.ITEM}-A', '01', '{objXMLT.MODEL}', 'HWD000000000000000000000000000001', 
                              '{objXMLT.ITEM}', '', '****************', '', '****************', 'SYSTEM', SYSDATE, 'MODEL', 'AQL_0.25')";
                            SFCDB.ExecSQL(sql);

                            sql = $@"INSERT INTO R_SKU_ROUTE(ID, ROUTE_ID, SKU_ID, DEFAULT_FLAG, EDIT_TIME, EDIT_EMP)
                                    VALUES('{route_id}', 'HWD0000000000000000000000000000T9', '{sku_id}', '', SYSDATE, 'SYSTEM')";
                            SFCDB.ExecSQL(sql);
                        }
                        temp_skuno = objXMLT.ITEM + "-A";
                        listSku = SFCDB.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == temp_skuno).ToList();
                        if (listSku.Count == 0)
                        {
                            sku_id = TCS.GetNewID("HWD", SFCDB);
                            route_id = TRSR.GetNewID("HWD", SFCDB);
                            sql = $@"INSERT INTO C_SKU (id, bu, skuno, version, sku_name, c_series_id, cust_partno, cust_sku_code, sn_rule, panel_rule, description, edit_emp, edit_time, sku_type, aqltype )
                              VALUES  ('{sku_id}', 'HWD', '{objXMLT.ITEM}-A', '01', '{objXMLT.MODEL}', 'HWD000000000000000000000000000001', 
                              '{objXMLT.ITEM}', '', '****************', '', '****************', 'SYSTEM', SYSDATE, 'MODEL', 'AQL_0.25')";
                            SFCDB.ExecSQL(sql);

                            sql = $@"INSERT INTO R_SKU_ROUTE (ID, ROUTE_ID, SKU_ID, DEFAULT_FLAG, EDIT_TIME, EDIT_EMP)
                                    VALUES('{route_id}', 'HWD0000000000000000000000000000T9', '{sku_id}', '', SYSDATE, 'SYSTEM')";
                            SFCDB.ExecSQL(sql);
                        }

                        sql = $@"select count(*) as row_count from mes1.c_product_config WHERE p_no ='A{objXMLT.ITEM}-A' ";
                        DataTable dtAllPart = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                        if (dtAllPart != null && dtAllPart.Rows[0][0].ToString() == "0")
                        {
                            sql = $@"INSERT INTO  mes1.c_product_config (cust_code,bu_code,p_no,p_version,p_desc,p_type,sn_len,pth_flag,wo_pno,process_type,
                                 edit_time,edit_emp,weld_qty,panel_type,link_qty,process_flag,memo,data1,data2) VALUES 
                                  ('HUAWEI','HWD','A{objXMLT.ITEM}-A','01','{objXMLT.MODEL}','OEM','11','1','','0',SYSDATE,'SYSTEM','1','0','1','T','','','Y') ";
                            APDB.ExecSQL(sql);
                        }

                        sql = $@"select count(*) as row_count from mes1.c_product_config WHERE p_no ='{objXMLT.ITEM}-A' ";
                        dtAllPart = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                        if (dtAllPart != null && dtAllPart.Rows[0][0].ToString() == "0")
                        {
                            sql = $@"INSERT INTO  mes1.c_product_config (cust_code,bu_code,p_no,p_version,p_desc,p_type,sn_len,pth_flag,wo_pno,process_type,
                                 edit_time,edit_emp,weld_qty,panel_type,link_qty,process_flag,memo,data1,data2) VALUES 
                                  ('HUAWEI','HWD','{objXMLT.ITEM}-A','01','{objXMLT.MODEL}','OEM','11','1','','0',SYSDATE,'SYSTEM','1','0','1','T','','','Y') ";
                            APDB.ExecSQL(sql);
                        }
                    }

                }
                catch (Exception ex)
                {
                    RecodeLocalLog("ERROR:" + task_no + ";" + ex.Message);
                }
            }
        }

        private void RecodeLocalLog(string msg)
        {
            string logPath = System.IO.Directory.GetCurrentDirectory() + "\\log\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string logFile= System.IO.Directory.GetCurrentDirectory() + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
            //通过指定字符编码方式可以实现对汉字的支持，否则在用记事本打开查看会出现乱码            
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Current);
            if (msg == "")
            {
                sw.WriteLine();
            }
            else
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + ": " + msg);
            }
            sw.Flush();
            sw.Close();
        }

        private void UpdateWOType()
        {
            string[] arrayThirdWord = new string[] { "G", "F", "N" };
            string[] arraySecondWord = new string[] { "R", "S", "T", "W", "V" };

            string sql = "select * from r_7b5_wo_temp where wo_type is null and RECEIVE_DATE>sysdate-10";

            DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                string wo_task_no = dt.Rows[i]["TASK_NO"].ToString();
                string thirdWord = wo_task_no.Substring(2, 1);
                string secondWord = wo_task_no.Substring(1, 1);
                string wo_type = "";
                string skuno = dt.Rows[i]["HH_ITEM"].ToString();
                C_SKU objSku = SFCDB.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == skuno).ToList().FirstOrDefault();
                if (objSku == null)
                {
                    throw new Exception(skuno + " Not Setting In C_SKU!");
                }

                if (wo_task_no.StartsWith("BS"))
                {

                    wo_type = "ZWSW";
                }
                else
                {

                    if (arraySecondWord.Contains(secondWord) && thirdWord == "Z")
                    {
                        if (objSku.SKU_TYPE == "PCBA")
                        {
                            wo_type = "ZESW";
                        }
                        else
                        {
                            wo_type = "ZESD";
                        }
                    }

                    if (!arraySecondWord.Contains(secondWord) && thirdWord == "Z")
                    {
                        if (objSku.SKU_TYPE == "PCBA")
                        {
                            wo_type = "ZESW";
                        }
                        else
                        {
                            wo_type = "ZWSD";
                        }
                    }
                    if (thirdWord == "G" || thirdWord == "F")
                    {
                        wo_type = "ZCS4";
                    }
                }
                sql = $@" update r_7b5_wo_temp  set WO_TYPE='{wo_type}' where task_no='{wo_task_no}' and HH_ITEM='{skuno}'";
                SFCDB.ExecSQL(sql);
            }
        }
    }
}
