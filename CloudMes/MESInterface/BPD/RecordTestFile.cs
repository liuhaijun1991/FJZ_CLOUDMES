using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace MESInterface.BPD
{
    public class RecordTestFile : taskBase
    {

        public string DB = string.Empty;
        string BU = string.Empty;
        private string remoteHost;
        private string remotePath;
        private string remote3DXPath;
        private string remoteLogPath;
        private string remoteUser;
        private string remotePass;
        private int remotePort;
        private string receivePath;
        private string logFilePath;
        private OleExec SFCDB = null;
        public T_R_TEST_RECORD TestRecord = null;
        public T_R_REPAIR_MAIN RepairMain = null;
        public T_R_SN RSN = null;
        public T_C_ROUTE_DETAIL TCRD = null;
        public T_C_ROUTE_DETAIL_DIRECTLINK TCRDDirectLink = null;
        public T_R_SN_STATION_DETAIL TRSSD = null;
        private FTPClient ftpClient = null;


        public override void init()
        {
            DB = ConfigGet("DB");
            BU = ConfigGet("BU");
            remoteHost = ConfigGet("HOST");
            remotePort = Int32.Parse(ConfigGet("PORT"));
            remotePath = ConfigGet("PATH");
            remote3DXPath = ConfigGet("3DXPATH");
            remoteLogPath = ConfigGet("LOGPATH");
            remoteUser = ConfigGet("USERNAME");
            remotePass = ConfigGet("PASSWORD");
            receivePath = ConfigGet("RECEIVE_PATH");
            logFilePath = ConfigGet("LOGFILE_PATH");
            SFCDB = new OleExec(DB, true);
            Output.UI = new RecordTestFile_UI(this);
            TestRecord = new T_R_TEST_RECORD(SFCDB, DB_TYPE_ENUM.Oracle);
            RepairMain = new T_R_REPAIR_MAIN(SFCDB, DB_TYPE_ENUM.Oracle);
            TCRD = new T_C_ROUTE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
            TCRDDirectLink = new T_C_ROUTE_DETAIL_DIRECTLINK(SFCDB, DB_TYPE_ENUM.Oracle);
            RSN = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
            TRSSD = new T_R_SN_STATION_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
            ftpClient = new FTPClient(remoteHost, remotePath, remoteUser, remotePass, remotePort);
            base.init();
        }



        public override void Start()
        {
            DoSomething(SFCDB, BU, "BPD_RecordTestFile", "BPD_RecordTestFile", "Record Test File Fail", DoRecordTest);
        }

        public string GetAFileContent(string file)
        {
            string LocalFile = file.Substring(file.LastIndexOf("/") + 1).Replace("\r", "");
            StringBuilder FileContent = new StringBuilder();
            try
            {
                ftpClient.Get(file, receivePath, LocalFile);
                if (File.Exists(receivePath + LocalFile))
                {
                    BufferedStream bs = new BufferedStream(new FileStream(receivePath + LocalFile, FileMode.Open));

                    StreamReader sr = null;
                    try
                    {

                        if (bs != null)
                        {
                            sr = new StreamReader(bs);
                            string line = sr.ReadLine();
                            while (line != null)
                            {
                                FileContent.Append(FormatLine(line));
                                line = sr.ReadLine();
                            }
                        }
                    }
                    finally
                    {
                        sr.Close();
                        bs.Close();
                    }
                }
            }
            catch (Exception)
            {
                return FileContent.ToString();
            }
            return FileContent.ToString();
        }

        public string FormatLine(string line)
        {
            string[] strs = line.Split('\t');
            if (strs.Length <= 1)
            {
                strs = line.Split(',');
            }
            StringBuilder sb = new StringBuilder();
            foreach (string str in strs)
            {
                if (!str.Equals(""))
                {
                    sb.Append(str.Trim()).Append(",");
                }
            }
            return sb.Length > 0 ? sb.Replace(",", "", sb.ToString().LastIndexOf(","), 1).ToString() : "";
        }

        public string StationMapping(string OriginalStation)
        {
            if (OriginalStation.StartsWith("ROBAT_RXI"))
            {
                return "XRAY_RXI";
            }
            if (OriginalStation.StartsWith("ROBAT_S1"))
            {
                return "ICT_S1";
            }
            if (OriginalStation.Contains("3DX"))
            {
                return "XRAY_3DX";
            }
            else
            {
                return OriginalStation;
            }
        }

        public string LineMapping(string DeviceName)
        {
            switch (DeviceName.ToUpper())
            {
                case "3DX_01":
                    return "A13XRAY3DX01";
                case "3DX_02":
                    return "A13XRAY3DX02";
                case "RXI_01":
                    return "A13XRAYRXI01";
                case "RXI_02":
                    return "A13XRAYRXI02";
                case "FXSICT1":
                    return "A13ICT";
                case "S1_01":
                    return "A13ICTS101";
                case "S1_02":
                    return "A13ICTS102";
                default:
                    return DeviceName;
            }
        }

    
        public void DoRecordTest()
        {
            int count = 0;
            int success = 0;
            R_TEST_RECORD record = null;
            Dictionary<string, List<C_ROUTE_DETAIL>> routeCache = new Dictionary<string, List<C_ROUTE_DETAIL>>();

            if (!ftpClient.Connected)
            {
                ftpClient.Connect();
            }
            //ftpClient.SetTransferType(FTPClient.TransferType.Binary);
            //if (!_3DXFtpClient.Connected)
            //{
            //    _3DXFtpClient.Connect();
            //}
            List<string> FileList = ftpClient.Dir("").ToList();
            List<string> _3DXFileList = ftpClient.Dir(remote3DXPath).ToList();
            //foreach (string s in _3DXFileList)
            //{
            //    SFCDB.ORM.SqlQueryable<R_WO_TEXT>($@"insert into r_wo_text(AUFNR) values('{s}')");
            //}
            FileList.AddRange(_3DXFileList);
            for (int i = FileList.Count-1; i >=0; i--)
            {
                string file = FileList[i];
                string Sn = string.Empty;
                string Station = string.Empty;
                DateTime StartTime = DateTime.Now;
                DateTime EndTime = DateTime.Now;
                string Status = string.Empty;
                string Device = string.Empty;
                string TestInfo = string.Empty;
                string TestEmp = string.Empty;
                string LocalFile = string.Empty;

                string[] infos = null;

                if (file.Equals("") || !file.Contains(".txt") || file.Replace("\r", "").Equals(".") || file.Replace("\r", "").Equals(".."))
                    continue;

                try
                {
                    infos = GetAFileContent(file.Replace("\r", "")).Split(',');
                    count++;
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "RecordTestFile", "RecordTestFile", "RecordTestFile", $@"Read Original Message:{string.Join(",",infos)}", "", "A0225204");
                    LocalFile = file.Substring(file.LastIndexOf("/") + 1).Replace("\r", "");
                    if (infos.Length > 0)
                    {
                        if (i < FileList.Count - _3DXFileList.Count)
                        {
                            //3DX0000,1538011235,FXS22390002,73-15632-04,3DX,P,EE9,passed,0,0,A0225204
                            //ROBAT_RXI_01,1573625370,FXS234500JJ,73-15632-04,RXI_01,P,000,passed,900,0,A0245149	
                            Sn = infos[2];
                            Station = StationMapping(infos[0]);
                            StartTime = DateTime.Parse("1970-1-1 00:00:00").AddSeconds(Int32.Parse(infos[1])).AddSeconds(8 * 60 * 60);
                            EndTime = StartTime;
                            Status = infos[5].StartsWith("P") ? "PASS" : "FAIL";
                            TestInfo = infos[6];
                            TestEmp = infos[10];
                            Device = infos[4];
                        }
                        else
                        {
                            //73-15632-04 C0,FXS2244055K,PASS,3DX,3DX01,A0245787,2018/11/08-D,2018/11/08 16:11:33,2018/11/08 16:24:37,00:16:38,2018/11/08 16:28:11,2018/11/08 16:30:33,00:02:21,10464,25,268,268,
                            Sn = infos[1];
                            Station = StationMapping(infos[3]);
                            StartTime = DateTime.Parse(infos[7]);
                            EndTime = DateTime.Parse(infos[8]);
                            Status = infos[2];
                            TestEmp = infos[5];
                            Device = infos[4];
                        }
                        
                        R_SN SN = RSN.GetSN(Sn, SFCDB);
                        if (SN != null)
                        {
                            string TempFile = string.Empty;
                            
                            if (i < FileList.Count - _3DXFileList.Count)
                            {
                                TempFile = remotePath + LocalFile.Replace("\r", "");
                            }
                            else
                            {
                                TempFile = remote3DXPath + "/" + LocalFile.Replace("\r", "");
                            }
                            //如果R_SN中的NextStation不等於當前測試文件中的Station   
                            if (!SN.NEXT_STATION.Equals(Station))
                            {
                                //嘗試從緩存中加載
                                List<C_ROUTE_DETAIL> routeDetails = null;
                                if (routeCache.ContainsKey(SN.ROUTE_ID))
                                {
                                    routeDetails = routeCache[SN.ROUTE_ID];
                                }
                                else
                                {
                                    routeDetails= TCRD.GetByRouteIdOrderBySEQASC(SN.ROUTE_ID, SFCDB);
                                    routeCache.Add(SN.ROUTE_ID, routeDetails);
                                }
                                #region 20191122 Route中有跳站時根據原本的設計緊緊看NextStation,這是不嚴謹的
                                //獲取當前工站名稱
                                string VarCurrentStation = SN.CURRENT_STATION;
                                //獲取當前工站在Route中對應的ID
                                C_ROUTE_DETAIL CSID = routeDetails.Find(t => t.STATION_NAME == VarCurrentStation);
                                if (CSID != null)//肯定不空,所以不用else
                                {
                                    string VarCurrentStationRouteID = CSID.ID;
                                    //getDetailDirectLink先置空
                                    List<C_ROUTE_DETAIL_DIRECTLINK> getDetailDirectLink = null;
                                    //根據當前工站ID查C_ROUTE_DETAIL_DIRECTLINK
                                    getDetailDirectLink = TCRDDirectLink.GetByDetailId(VarCurrentStationRouteID, SFCDB);
                                    if (getDetailDirectLink == null)//說明沒有跳站
                                    {//是不是直接刪除ftp文件即可?
                                        //如果測試文件裡面的站位在流程裡面沒有找到，那麼刪除測試文件
                                        C_ROUTE_DETAIL rd = routeDetails.Find(t => t.STATION_NAME == Station);
                                        if (rd == null)
                                        {
                                            ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                        }

                                        //如果產品的 NEXT_STATION 有問題，繼續並且刪除測試文件
                                        C_ROUTE_DETAIL nextStation = routeDetails.Find(t => t.STATION_NAME == SN.NEXT_STATION);
                                        if (nextStation == null)
                                        {
                                            ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                            continue;
                                        }

                                        //如果產品的 NEXT_STATION 已經在測試文件裡面站位之後，表示產品已經過站了，則刪除文件
                                        if (rd.SEQ_NO < nextStation.SEQ_NO && TestRecord.GetLastTestRecord(SN.SN, Station, SFCDB) != null)
                                        {
                                            ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                        }
                                        //如果不需要處理，就刪掉本地的文件
                                        File.Delete(receivePath + LocalFile);
                                        continue;
                                    }
                                    else//說明有跳站
                                    {
                                        //用當前工站ID查DirectLink中的DirectLinkID
                                        C_ROUTE_DETAIL_DIRECTLINK DirectlinkID = getDetailDirectLink.Find(t => t.C_ROUTE_DETAIL_ID == VarCurrentStationRouteID);
                                        if (DirectlinkID != null)//肯定不空,所以不用else
                                        {
                                            //獲取當前工站可以跳站的工站名稱在Route中對應的ID
                                            string VarCurrentStationRouteDLID = DirectlinkID.DIRECTLINK_ROUTE_DETAIL_ID;
                                            C_ROUTE_DETAIL CStation = routeDetails.Find(t => t.ID == VarCurrentStationRouteDLID);
                                            if (CStation != null)
                                            {
                                                string RouteStationName = CStation.STATION_NAME;
                                                if (RouteStationName.Equals(Station))
                                                {
                                                    //目的是判斷到是跳站,更新r_sn的CurrentStation and NextStation,以便不變更updatestatus的邏輯
                                                    int result = RSN.TiaoZhanUpdateCurrentNextStation(SN.ID, SN.NEXT_STATION, RouteStationName, SFCDB);
                                                    if (result <= 0)
                                                    {
                                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SN.SN, "UPDATE" }));
                                                    }
                                                }
                                                else//幾乎不可能會null
                                                {
                                                    //如果測試文件裡面的站位在流程裡面沒有找到，那麼刪除測試文件
                                                    C_ROUTE_DETAIL rd = routeDetails.Find(t => t.STATION_NAME == Station);
                                                    if (rd == null)
                                                    {
                                                        ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                                    }

                                                    //如果產品的 NEXT_STATION 有問題，繼續並且刪除測試文件
                                                    C_ROUTE_DETAIL nextStation = routeDetails.Find(t => t.STATION_NAME == SN.NEXT_STATION);
                                                    if (nextStation == null)
                                                    {
                                                        ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                                        continue;
                                                    }

                                                    //如果產品的 NEXT_STATION 已經在測試文件裡面站位之後，表示產品已經過站了，則刪除文件
                                                    if (rd.SEQ_NO < nextStation.SEQ_NO && TestRecord.GetLastTestRecord(SN.SN, Station, SFCDB) != null)
                                                    {
                                                        ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                                    }
                                                    //如果不需要處理，就刪掉本地的文件
                                                    File.Delete(receivePath + LocalFile);
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion  
                                /*20191122 Rito
                                //如果測試文件裡面的站位在流程裡面沒有找到，那麼刪除測試文件
                                C_ROUTE_DETAIL rd = routeDetails.Find(t => t.STATION_NAME == Station);
                                if (rd == null)
                                {
                                    ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                }

                                //如果產品的 NEXT_STATION 有問題，繼續並且刪除測試文件
                                C_ROUTE_DETAIL nextStation = routeDetails.Find(t => t.STATION_NAME == SN.NEXT_STATION);
                                if (nextStation == null)
                                {
                                    ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                                    continue;
                                }

                                //如果產品的 NEXT_STATION 已經在測試文件裡面站位之後，表示產品已經過站了，則刪除文件
                                if (rd.SEQ_NO < nextStation.SEQ_NO  && TestRecord.GetLastTestRecord(SN.SN,Station,SFCDB)!=null)
                                {
                                    ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", "")); 
                                }

                                //如果不需要處理，就刪掉本地的文件
                                File.Delete(receivePath + LocalFile);
                                continue;20191122 Rito*/
                            }

                            try
                            {
                                record = new R_TEST_RECORD();
                                record.ID = TestRecord.GetNewID(BU, SFCDB);
                                record.R_SN_ID = SN.ID;
                                record.SN = SN.SN;
                                record.STATE = Status;
                                record.TEGROUP = Station;
                                record.TESTATION = Station;
                                record.MESSTATION = Station;
                                record.DEVICE = LineMapping(Device);
                                record.STARTTIME = StartTime;
                                record.ENDTIME = EndTime;
                                record.DETAILTABLE = $@"from test file {file}";
                                record.EDIT_EMP = TestEmp;
                                record.EDIT_TIME = TestRecord.GetDBDateTime(SFCDB);
                                record.TESTINFO = TestInfo;
                                TestRecord.InsertTestRecord(record, SFCDB);

                                ftpClient.Rename(TempFile, remoteLogPath + LocalFile.Replace("\r", ""));
                            }
                            catch (Exception e)
                            {
                                WriteLog.WriteIntoMESLog(SFCDB, BU, "RecordTestFile", "RecordTestFile", "RecordTestFile", $@"{e.Message}", "", "Interface");
                            }
                            //WriteLog.WriteIntoMESLog(SFCDB, BU, "RecordTestFile", "RecordTestFile", "RecordTestFile", $@"Insert into R_TEST_RECORD", "", "A0225204");


                            if (File.Exists(receivePath + LocalFile))
                            {
                                //如果處理完了就將下載下來的本地文件移到另外一個目錄，然後刪掉 FTP 上面的文件
                                if (!File.Exists(logFilePath + LocalFile))
                                {
                                    File.Copy(receivePath + LocalFile, logFilePath + LocalFile);
                                }

                                File.Delete(receivePath + LocalFile);
                            }

                            if (record.STATE.Equals("PASS"))
                            {   //過站
                                RSN.PassStation(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, "PASS", record.EDIT_EMP, SFCDB);
                                success++;
                            }
                            else
                            {
                                RSN.PassStation(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, "FAIL", record.EDIT_EMP, SFCDB);
                                //RSN.RecordPassStationDetail(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, SFCDB, "1");
                                //RSN.PassStation(SN.SN, record.DEVICE, record.MESSTATION, record.DEVICE, BU, record.STATE, record.EDIT_EMP, SFCDB);
                            }

                            //寫UPH，良率
                            RSN.RecordUPH(SN.WORKORDERNO, 1, SN.SN, record.STATE, record.DEVICE, record.MESSTATION, record.EDIT_EMP, BU, SFCDB);
                            RSN.RecordYieldRate(SN.WORKORDERNO, 1, SN.SN, record.STATE,record.DEVICE, record.MESSTATION, record.EDIT_EMP, BU, SFCDB);

                            
                            //WriteLog.WriteIntoMESLog(SFCDB, BU, "RecordTestFile", "RecordTestFile", "RecordTestFile", $@"{Sn} Pass Station", "", "A0225204");


                            //else
                            //{
                            //    //寫不良
                            //    SN.REPAIR_FAILED_FLAG = "1";
                            //    RSN.Update(SN, SFCDB);
                            //    RepairMain.Insert(SN, Station, record.DEVICE, record.EDIT_EMP, record.ENDTIME, BU, SFCDB);
                            //}
                        }
                        else
                        {
                            ftpClient.Rename(file, remoteLogPath + LocalFile.Replace("\r", ""));
                            if (File.Exists(receivePath + LocalFile))
                                File.Delete(receivePath + LocalFile);
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.BPD.RecordTestFile", "DoRecordTest", $@"{LocalFile}:{e.StackTrace},{e.Message}", "", "MESInterface");
                    continue;
                }
            }

            ftpClient.DisConnect();

            MESPubLab.WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.BPD.RecordTestFile", "DoRecordTest", $@"deal with {count} files and finally success record {success} files.", "", "MESInterface");
            
        }

    }
}
