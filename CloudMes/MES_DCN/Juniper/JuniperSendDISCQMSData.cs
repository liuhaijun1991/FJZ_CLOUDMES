using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.ModuleHelp;
using MESDBHelper;
using MESPubLab.Common;
using Renci.SshNet;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;

namespace MES_DCN.Juniper
{
    public class JuniperSendDISCQMSData
    {
        private string _mesdbstr, _apdbstr, _bustr, _filepath, _filebackuppath, _remotepath, _keypath, _plant;

        #region B2B test sftp
        //private string CONST_SFTPHost = "10.191.23.14";
        //private string CONST_SFTPPort = "443";
        //private string CONST_SFTPLogin = "JuniperSFCTest";
        //private string CONST_SFTPPassword = "52V1nR3S!";
        #endregion

        #region B2B pro sftp
        private string CONST_SFTPHost = "10.191.23.14";
        private string CONST_SFTPPort = "5022";
        private string CONST_SFTPLogin = "JuniperSFCProd";
        private string CONST_SFTPPassword = "41hbx77w@";
        #endregion

        const string CONST_SUPPLIER = "FOXCONN";

        private string traceFileNameColumn = "TRANSACTION_DATA/SUPPLIER_DATA/raw/FOXCONN/component_trace/";
        private string testFileNameColumn = "TRANSACTION_DATA/SUPPLIER_DATA/raw/FOXCONN/test/";
        private string defectiveFileNameColumn = "TRANSACTION_DATA/SUPPLIER_DATA/raw/FOXCONN/defective/";
        private string repairFileNameColumn = "TRANSACTION_DATA/SUPPLIER_DATA/raw/FOXCONN/repair/";
        private string manufacturingFileNameColumn = "TRANSACTION_DATA/SUPPLIER_DATA/raw/FOXCONN/manufacturing/";
        public JuniperSendDISCQMSData(string mesdbstr, string apdbstr, string bustr, string filepath, string filebackpath, string remotepath, string keypath, string plant, bool IsPROD)
        {
            _mesdbstr = mesdbstr;
            _apdbstr = apdbstr;
            _bustr = bustr;
            _filepath = filepath;
            _filebackuppath = filebackpath;
            _remotepath = remotepath;
            _keypath = keypath;
            _plant = plant;
            if (!IsPROD)
            {
                CONST_SFTPPort = "443";
                CONST_SFTPLogin = "JuniperSFCTest";
                CONST_SFTPPassword = "52V1nR3S!";
            }
        }

        public void Build(string filetype, DateTime collectDate, ref Dictionary<string, FuncExecRes> listresult, bool bSend = true)
        {
            try
            {
                //date = Convert.ToDateTime("2021-04-01 08:00:00");
                SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keypath, KeyType.RSA_PRIVATE);
                var typeList = filetype.Split(',').ToList();

                SqlSugarClient APDB = OleExec.GetSqlSugarClient(_apdbstr, false);
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
                R_DISC_HEAD disc_head = null;
                string key = collectDate.ToString("yyyy-MM-dd");
                string fileName = $"{collectDate.ToString("yyyyMMddHHmmss")}.csv";
                if (bSend)
                {
                    SFCDB.Updateable<R_DISC_HEAD>().SetColumns(r => new R_DISC_HEAD { VALID_FLAG = 0 }).Where(r => r.DISC_KEY == key && r.PLANT == _plant).ExecuteCommand();
                }
                List<R_DISC_HEAD> list_head = new List<R_DISC_HEAD>();
                foreach (var type in typeList)
                {
                    disc_head = new R_DISC_HEAD();
                    disc_head.ID = MesDbBase.GetNewID<R_DISC_REPAIR>(SFCDB, _bustr);
                    disc_head.DISC_KEY = key;
                    if (type == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_TRAC_{fileName}";
                    }
                    else if (type == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_TEST_{fileName}";
                    }
                    else if (type == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_DTRC_{fileName}";
                    }
                    else if (type == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_MREP_{fileName}";
                    }
                    else if (type == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_MANF_{fileName}";
                    }
                    else
                    {
                        throw new Exception($@"Input type error!");
                    }
                    disc_head.COLLECT_FLAG = "0";
                    disc_head.CONVERT_FLAG = "0";
                    disc_head.SEND_FLAG = "0";
                    disc_head.VALID_FLAG = bSend ? 1 : 2;
                    disc_head.CREATE_TIME = SFCDB.GetDate();
                    disc_head.EDIT_TIME = SFCDB.GetDate();
                    disc_head.DISC_TYPE = type;
                    disc_head.PLANT = _plant;
                    SFCDB.Insertable<R_DISC_HEAD>(disc_head).ExecuteCommand();
                    list_head.Add(disc_head);
                }

                CollectingData(SFCDB, APDB, collectDate, list_head);

                FuncExecRes funcResult = null;
                foreach (var headObj in list_head)
                {
                    DiscBase disc = null;
                    funcResult = null;
                    try
                    {
                        if (headObj.DISC_TYPE == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                        {
                            disc = new TraceDisc();
                        }
                        else if (headObj.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                        {
                            disc = new TestDisc();
                        }
                        else if (headObj.DISC_TYPE == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                        {
                            disc = new DefectDisc();
                        }
                        else if (headObj.DISC_TYPE == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                        {
                            disc = new RepairDisc();
                        }
                        else if (headObj.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                        {
                            disc = new MFGDisc();
                        }
                        else
                        {
                            continue;
                        }
                        disc.bSend = bSend;
                        disc.sfcdb = SFCDB;
                        disc.sftpHelp = sftpHelp;
                        disc.localFilePath = _filepath;
                        disc.remotePath = _remotepath;
                        disc.backupPath = _filebackuppath;
                        disc.DiscHeadObj = headObj;
                        disc.FileFullName = $@"{_filepath}{headObj.DISC_FILE}";
                        funcResult = ConvertToFile(disc);
                    }
                    catch (Exception ex)
                    {
                        funcResult = new FuncExecRes() { IsSuccess = false, ErrorMessage = ex.Message, ErrorException = ex };
                    }
                    if (funcResult != null)
                    {
                        listresult.Add(headObj.DISC_TYPE, funcResult);
                    }
                }
                try
                {
                    sftpHelp.Disconnect();
                }
                catch (Exception)
                {
                }
                try
                {
                    SFCDB.Dispose();
                    APDB.Dispose();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BuildTask(string filetype, DateTime collectDate, ref Dictionary<string, FuncExecRes> listResult, bool bSend = true)
        {
            try
            {
                //date = Convert.ToDateTime("2021-04-01 08:00:00");
                SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keypath, KeyType.RSA_PRIVATE);
                var typeList = filetype.Split(',').ToList();

                SqlSugarClient APDB = OleExec.GetSqlSugarClient(_apdbstr, false);
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
                R_DISC_HEAD disc_head = null;
                DateTime loadDate = SFCDB.GetDate();
                string key = collectDate.ToString("yyyy-MM-dd");
                string fileName = $"{collectDate.ToString("yyyyMMddHHmmss")}.csv";
                if (bSend)
                {
                    SFCDB.Updateable<R_DISC_HEAD>().SetColumns(r => new R_DISC_HEAD { VALID_FLAG = 0 }).Where(r => r.DISC_KEY == key && r.PLANT == _plant).ExecuteCommand();
                }
                List<R_DISC_HEAD> list_head = new List<R_DISC_HEAD>();
                foreach (var type in typeList)
                {
                    disc_head = new R_DISC_HEAD();
                    disc_head.ID = MesDbBase.GetNewID<R_DISC_REPAIR>(SFCDB, _bustr);
                    disc_head.DISC_KEY = key;
                    if (type == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_TRAC_{fileName}";
                    }
                    else if (type == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_TEST_{fileName}";
                    }
                    else if (type == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_DTRC_{fileName}";
                    }
                    else if (type == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_MREP_{fileName}";
                    }
                    else if (type == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                    {
                        disc_head.DISC_FILE = $@"{_plant}_MANF_{fileName}";
                    }
                    else
                    {
                        throw new Exception($@"Input type error!");
                    }
                    disc_head.COLLECT_FLAG = "0";
                    disc_head.CONVERT_FLAG = "0";
                    disc_head.SEND_FLAG = "0";
                    disc_head.VALID_FLAG = bSend ? 1 : 2;
                    disc_head.CREATE_TIME = SFCDB.GetDate();
                    disc_head.EDIT_TIME = SFCDB.GetDate();
                    disc_head.DISC_TYPE = type;
                    disc_head.PLANT = _plant;
                    SFCDB.Insertable<R_DISC_HEAD>(disc_head).ExecuteCommand();
                    list_head.Add(disc_head);
                }

                CollectingDataTask(SFCDB, APDB, collectDate, list_head);

                foreach (var headObj in list_head)
                {
                    DiscBase disc = null;
                    if (headObj.DISC_TYPE == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                    {
                        disc = new TraceDisc();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                    {
                        disc = new TestDisc();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                    {
                        disc = new DefectDisc();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                    {
                        disc = new RepairDisc();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                    {
                        disc = new MFGDisc();
                    }
                    else
                    {
                        continue;
                    }
                    disc.bSend = bSend;
                    disc.sfcdb = OleExec.GetSqlSugarClient(_mesdbstr, false);
                    disc.sftpHelp = sftpHelp;
                    disc.localFilePath = _filepath;
                    disc.remotePath = _remotepath;
                    disc.backupPath = _filebackuppath;
                    disc.DiscHeadObj = headObj;
                    disc.FileFullName = $@"{_filepath}{headObj.DISC_FILE}";
                    listResult.Add(disc.DiscHeadObj.DISC_TYPE, ConvertToFile(disc));
                }
                try
                {
                    sftpHelp.Disconnect();
                }
                catch (Exception)
                {
                }
                try
                {
                    SFCDB.Dispose();
                    APDB.Dispose();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CollectingData(SqlSugarClient SFCDB, SqlSugarClient APDB, DateTime collectedDate, List<R_DISC_HEAD> list_head)
        {
            string collectedDateStr = collectedDate.ToString("yyyy/MM/dd").Replace('-', '/');
            List<R_SN_STATION_DETAIL> listDetail = SFCDB.Ado.SqlQuery<R_SN_STATION_DETAIL>($@"
                                    select *
                                      from r_sn_station_detail a
                                     where a.edit_time between
                                           to_date('{collectedDateStr} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                           to_date('{collectedDateStr} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                       and exists
                                     (select * from c_station b where a.station_name = b.station_name) order by edit_time");
            DateTime loadDate = SFCDB.GetDate();
            foreach (var item in listDetail)
            {
                CollectingPassStationData(SFCDB, APDB, loadDate, collectedDateStr, item, list_head);
            }

            CollectingRepairData(SFCDB, APDB, loadDate, collectedDateStr, list_head);
            list_head.ForEach(r =>
            {
                r.COLLECT_FLAG = "1";
            });
            SFCDB.Updateable<R_DISC_HEAD>(list_head).ExecuteCommand();
        }
        public void CollectingDataTask(SqlSugarClient SFCDB, SqlSugarClient APDB, DateTime collectedDate, List<R_DISC_HEAD> list_head)
        {
            string collectedDateStr = collectedDate.ToString("yyyy/MM/dd").Replace('-', '/');
            List<R_SN_STATION_DETAIL> listDetail = SFCDB.Ado.SqlQuery<R_SN_STATION_DETAIL>($@"
                                    select DISTINCT SN,SKUNO,WORKORDERNO, STATION_NAME,START_TIME
                                      from r_sn_station_detail a
                                     where a.edit_time between
                                           to_date('{collectedDateStr} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                           to_date('{collectedDateStr} 23:59:59', 'yyyy/mm/dd hh24:mi:ss') AND VALID_FLAG=1 
                                       and exists
                                     (select * from c_station b where a.station_name = b.station_name) order by START_TIME ");
            DateTime loadDate = SFCDB.GetDate();
            int totalTask = 10;
            int taskExecuteData = listDetail.Count % totalTask == 0 ? listDetail.Count / totalTask : listDetail.Count / totalTask + 1;
            List<Task> listTask = new List<Task>();
            for (int t = 0; t < totalTask; t++)
            {
                List<R_SN_STATION_DETAIL> listDetailTemp = new List<R_SN_STATION_DETAIL>();
                if (t * taskExecuteData < listDetail.Count)
                {
                    listDetailTemp = listDetail.Skip(t * taskExecuteData).Take(taskExecuteData).ToList();
                }
                else
                {
                    listDetailTemp = listDetail.Take(listDetail.Count - t * taskExecuteData).ToList();
                }
                if (listDetailTemp.Count == 0)
                {
                    break;
                }
                
                Task task = new Task(() =>
                {
                    DiscTaskObj taskData = new DiscTaskObj()
                    {
                        SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false),
                        APDB = OleExec.GetSqlSugarClient(_apdbstr, false),
                        LoadDate = loadDate,
                        CollectedDate = collectedDateStr,
                        Data = listDetailTemp,
                        HeadList = list_head
                    };
                    TaskAction(taskData);
                });
                task.Start();
                //TaskFactory taskFactory = new TaskFactory();
                //Task task = taskFactory.StartNew((data) =>
                //{
                //    try
                //    {
                //        DiscTaskObj inputData = (DiscTaskObj)data;
                //        foreach (var item in (List<R_SN_STATION_DETAIL>)inputData.Data)
                //        {
                //            CollectingPassStationData(inputData.SFCDB, inputData.APDB, inputData.LoadDate, inputData.CollectedDate, item, inputData.HeadList);
                //        }
                //        try
                //        {
                //            inputData.SFCDB.Dispose();
                //            inputData.APDB.Dispose();
                //        }
                //        catch (Exception)
                //        {
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        try
                //        {
                //            R_MES_LOG log = new R_MES_LOG();
                //            log.ID = MesDbBase.GetNewID<R_MES_LOG>(OleExec.GetSqlSugarClient(_mesdbstr, false), _bustr);
                //            log.PROGRAM_NAME = "JuniperDiscData";
                //            log.CLASS_NAME = "MES_DCN.Juniper.JuniperSendDISCQMSData";
                //            log.FUNCTION_NAME = "CollectingPassStationData";
                //            log.LOG_MESSAGE = "TaskFactory Error";
                //            log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                //            log.DATA1 = collectedDateStr;
                //            //log.DATA2 = sn.STATION_NAME;
                //            log.EDIT_EMP = "Interface";
                //            log.EDIT_TIME = SFCDB.GetDate();
                //            SFCDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                //        }
                //        catch (Exception)
                //        {
                //        }
                //    }


                //}, taskData);
                listTask.Add(task);
            }
            Task.WaitAll(listTask.ToArray());

            CollectingRepairData(SFCDB, APDB, loadDate, collectedDateStr, list_head);
            list_head.ForEach(r => {
                r.COLLECT_FLAG = "1";
            });
            SFCDB.Updateable<R_DISC_HEAD>(list_head).ExecuteCommand();
        }

        public void TaskAction(DiscTaskObj inputData)
        {
            try
            {                
                foreach (var item in (List<R_SN_STATION_DETAIL>)inputData.Data)
                {
                    CollectingPassStationData(inputData.SFCDB, inputData.APDB, inputData.LoadDate, inputData.CollectedDate, item, inputData.HeadList);
                }
                try
                {
                    inputData.SFCDB.Dispose();
                    inputData.APDB.Dispose();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                try
                {
                    SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
                    R_MES_LOG log = new R_MES_LOG();
                    log.ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB, _bustr);
                    log.PROGRAM_NAME = "JuniperDiscData";
                    log.CLASS_NAME = "MES_DCN.Juniper.JuniperSendDISCQMSData";
                    log.FUNCTION_NAME = "CollectingPassStationData";
                    log.LOG_MESSAGE = "TaskFactory Error";
                    log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                    log.DATA1 = inputData.CollectedDate;
                    log.DATA2 = inputData.LoadDate.ToString("yyyyy/MM/dd HH:mm:ss");
                    log.EDIT_EMP = "Interface";
                    log.EDIT_TIME = SFCDB.GetDate();
                    SFCDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                }
                catch (Exception)
                {
                }
            }
        }

        public FuncExecRes ConvertToFile(DiscBase discObj)
        {
            FuncExecRes funcResult = null;
            try
            {
                discObj.GetSendDataList();
                if (discObj.DiscHeadObj.DISC_TYPE == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                {
                    discObj.SaveCSVFile<R_DISC_TRACE>();
                }
                else if (discObj.DiscHeadObj.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                {
                    discObj.SaveCSVFile<R_DISC_TEST>();
                }
                else if (discObj.DiscHeadObj.DISC_TYPE == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                {
                    discObj.SaveCSVFile<R_DISC_DEFECT>();
                }
                else if (discObj.DiscHeadObj.DISC_TYPE == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                {
                    discObj.SaveCSVFile<R_DISC_REPAIR>();
                }
                else if (discObj.DiscHeadObj.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                {
                    discObj.SaveCSVFile<R_DISC_MFG>();
                }
                else
                {
                    throw new Exception($"DISC TYPE ERROR.");
                }

                discObj.UpdateConvertFlag();
                discObj.CompressFile();
                discObj.UploadSFTP();
                discObj.UpdateSendFlag();
                discObj.UpdateHeadSendFlag();
                discObj.DoBackups();
                funcResult = new FuncExecRes() { IsSuccess = true, ErrorMessage = "OK", ErrorException = null };
            }
            catch (Exception ex)
            {
                funcResult = new FuncExecRes() { IsSuccess = false, ErrorMessage = ex.Message, ErrorException = ex };
            }
            return funcResult;
        }

        public void CollectingPassStationData(SqlSugarClient SFCDB, SqlSugarClient APDB, DateTime loadDate, string collectedDate, R_SN_STATION_DETAIL sn, List<R_DISC_HEAD> headList)
        {
            try
            {
                var agile = SFCDB.Queryable<MESDataObject.Module.OM.O_AGILE_ATTR>().Where(r => r.ITEM_NUMBER == sn.SKUNO && r.PLANT == SqlFunc.Replace(_plant, "FXN", ""))
                    .OrderBy(r => r.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                DateTime sysdate = SFCDB.GetDate();
                #region R_DISC_TRACE    
                R_DISC_HEAD traceHead = headList.Find(r => r.DISC_TYPE == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description);
                if (traceHead != null)
                {
                    var list_trace = new List<R_DISC_TRACE>();
                    R_DISC_TRACE traceObj = null;

                    switch (sn.STATION_NAME)
                    {
                        case "SMT1":
                        case "SMT2":
                            //case "SMTLOADING":
                            //SMT1,SMT2
                            var process = sn.STATION_NAME == "SMT1" ? "B" : sn.STATION_NAME == "SMT2" ? "T" : "D";
                            var allpart = GetAllpartData(APDB, sn.SN, process);
                            for (int i = 0; i < allpart.Rows.Count; i++)
                            {
                                traceObj = new R_DISC_TRACE();
                                traceObj.ID = MesDbBase.GetNewID<R_DISC_TRACE>(SFCDB, _bustr);
                                traceObj.SUPPLIER = CONST_SUPPLIER;
                                traceObj.SUPPLIER_SITE = _plant;
                                #region 以下欄位為要抓取的數據
                                traceObj.PART_NUMBER = agile != null ? agile.CUSTPARTNO : sn.SKUNO; //與LABEL上的 Part number是否一致
                                traceObj.CM_ODM_PARTNUMBER = agile != null ? agile.CUSTPARTNO : sn.SKUNO;
                                traceObj.SERIAL_NUMBER = sn.SN;
                                traceObj.PART_NUMBER_REVISION = agile != null ? agile.REV : "";//與LABEL上的 REV是否一致
                                traceObj.SHOP_FLOOR_ORDER_NUMBER = sn.WORKORDERNO;//富士康工單?SALESORDERNUMBER

                                traceObj.COMPONENT_PART_NUMBER = allpart.Rows[i]["KP_NO"].ToString();// KP 和料表
                                traceObj.CM_COMPONENT_PART_NUMBER = allpart.Rows[i]["KP_NO"].ToString();//COMPONENT_PART_NUMBER
                                traceObj.ASSEMBLY_STATION = sn.STATION_NAME;//MES STATION
                                traceObj.ASSEMBLY_STATION_DESCRIPTION = sn.STATION_NAME;//MES STATION
                                traceObj.LOCATION = allpart.Rows[i]["LOCATION"].ToString();//                        
                                traceObj.VENDOR = allpart.Rows[i]["MFR_NAME"].ToString();//供應名
                                traceObj.MPN = allpart.Rows[i]["MFR_KP_NO"].ToString();//MPN
                                traceObj.DATE_CODE = allpart.Rows[i]["DATE_CODE"].ToString();
                                traceObj.LOT_CODE = allpart.Rows[i]["LOT_CODE"].ToString();
                                traceObj.SERIAL_NUMBER_CHILD = "";

                                traceObj.ECID = "";//固定空值
                                traceObj.QTY = "1";//固定1
                                traceObj.LOAD_DATE = Convert.ToDateTime(loadDate.ToString("yyyy-MM-dd"));//上傳日期//上料時間或綁定時間
                                #endregion
                                traceObj.FILE_NAME = $@"{traceFileNameColumn}{traceHead.DISC_FILE}";
                                traceObj.CREATE_TIME = sysdate;
                                traceObj.SENT_FLAG = 0;
                                traceObj.VALID_FLAG = 1;
                                traceObj.HEAD_ID = traceHead.ID;
                                list_trace.Add(traceObj);
                            }
                            break;
                        default:
                            //KP
                            //以後需要傳的KP要QE到網頁上配置
                            var kp_list = SFCDB.Ado.SqlQuery<R_SN_KP>($@"
                                    select *
                                      from r_sn_kp a
                                     where sn = '{sn.SN}'
                                       and a.edit_time between
                                           to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                           to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                       and a.valid_flag = '1'
                                        and a.PARTNO='{sn.SKUNO}'
                                       and a.STATION='{sn.STATION_NAME}'
                                        and a.STATION NOT IN ('SMTLOADING','MATL_LINK')
                                       and not exists
                                     (select *
                                              from r_disc_trace b
                                             where b.serial_number = a.sn
                                               and b.assembly_station = a.station
                                               and b.create_time between
                                                   to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                                   to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                               and b.sent_flag = '0')
                                    union 
                                    select *
                                      from r_sn_kp a
                                     where sn = '{sn.SN}'
                                       and a.edit_time between
                                           to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                           to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                       and a.valid_flag = '1'
                                       and a.SCANTYPE = 'APTRSN'
                                       and a.STATION='{sn.STATION_NAME}'
                                        and a.STATION<>'SMTLOADING'
                                       and not exists
                                     (select *
                                              from r_disc_trace b
                                             where b.serial_number = a.sn
                                               and b.assembly_station = a.station
                                               and b.create_time between
                                                   to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                                   to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                               and b.sent_flag = '0')
                                    ");
                            list_trace = new List<R_DISC_TRACE>();
                            foreach (var kp in kp_list)
                            {

                                var disc_Kp = SFCDB.Ado.SqlQuery<R_SN_KP>($@" select * From r_sn_kp where sn='{sn.SN}' and partno in(
                                                select MPN From r_sn_Kp where sn='{sn.SN}'  and  partno='{kp.PARTNO}') and VALID_FLAG=1 and STATION='MATL_LINK' ");


                                traceObj = new R_DISC_TRACE();
                                traceObj.ID = MesDbBase.GetNewID<R_DISC_TRACE>(SFCDB, _bustr);
                                traceObj.SUPPLIER = CONST_SUPPLIER;
                                traceObj.SUPPLIER_SITE = _plant;
                                #region 以下欄位為要抓取的數據，待編寫邏輯
                                traceObj.PART_NUMBER = agile != null ? agile.CUSTPARTNO : sn.SKUNO; //與LABEL上的 Part number是否一致
                                traceObj.CM_ODM_PARTNUMBER = agile != null ? agile.CUSTPARTNO : sn.SKUNO;
                                traceObj.SERIAL_NUMBER = sn.SN;
                                traceObj.PART_NUMBER_REVISION = agile != null ? agile.REV : "";//與LABEL上的 REV是否一致
                                traceObj.SHOP_FLOOR_ORDER_NUMBER = sn.WORKORDERNO;//富士康工單?SALESORDERNUMBER
                                traceObj.COMPONENT_PART_NUMBER = kp.PARTNO;// KP 和料表
                                traceObj.CM_COMPONENT_PART_NUMBER = kp.PARTNO;//COMPONENT_PART_NUMBER
                                traceObj.ASSEMBLY_STATION = kp.STATION;//MES STATION
                                traceObj.ASSEMBLY_STATION_DESCRIPTION = kp.STATION;//MES STATION
                                traceObj.VENDOR = "";//供應名
                                traceObj.DATE_CODE = "";
                                traceObj.LOT_CODE = "";
                                traceObj.SERIAL_NUMBER_CHILD = kp.VALUE;//KP value
                                traceObj.LOCATION = kp.LOCATION;//                        
                                traceObj.MPN = kp.MPN;//MPN
                                traceObj.ECID = "";//固定空值
                                traceObj.QTY = "1";//固定1
                                traceObj.LOAD_DATE = Convert.ToDateTime(loadDate.ToString("yyyy-MM-dd"));//上傳日期//上料時間或綁定時間
                                #endregion
                                traceObj.FILE_NAME = $@"{traceFileNameColumn}{traceHead.DISC_FILE}";
                                traceObj.CREATE_TIME = sysdate;
                                traceObj.SENT_FLAG = 0;
                                traceObj.VALID_FLAG = 1;
                                traceObj.HEAD_ID = traceHead.ID;
                                if (kp.SCANTYPE == "APTRSN")
                                {
                                    var TrsnData = GetAllpartTrSnData(APDB, kp.EXVALUE1);
                                    traceObj.VENDOR = TrsnData.Rows[0]["MFR_NAME"].ToString();//供應名
                                    traceObj.DATE_CODE = TrsnData.Rows[0]["DATE_CODE"].ToString();
                                    traceObj.LOT_CODE = TrsnData.Rows[0]["LOT_CODE"].ToString();
                                    traceObj.SERIAL_NUMBER_CHILD = kp.EXVALUE1;//KP value
                                    traceObj.MPN = TrsnData.Rows[0]["MFR_KP_NO"].ToString();

                                }
                                else if (disc_Kp.Count > 0)
                                {

                                    var GetVendor = SFCDB.Ado.SqlQuery<R_SN_KP>($@"select*From r_sn_kp where sn='{disc_Kp[0].VALUE}' and KP_NAME='TR_SN' and SCANTYPE='VENDOR'");
                                    var GetDateCode = SFCDB.Ado.SqlQuery<R_SN_KP>($@"select*From r_sn_kp where sn='{disc_Kp[0].VALUE}' and KP_NAME='TR_SN'  and SCANTYPE='DATE_CODE'");

                                    traceObj.VENDOR = GetVendor[0].VALUE.ToString();//供應名
                                    traceObj.DATE_CODE = GetDateCode[0].VALUE;
                                    traceObj.LOT_CODE = GetDateCode[0].EXVALUE1.ToString();
                                    traceObj.MPN = GetVendor[0].MPN.ToString();
                                    traceObj.COMPONENT_PART_NUMBER = kp.MPN;
                                    traceObj.CM_COMPONENT_PART_NUMBER = kp.MPN;
                                    traceObj.LOCATION = "";


                                }
                                else if (kp.STATION.Equals("SUPPLIER-SN"))
                                {
                                    var allpart_smtloading = GetAllpartData(APDB, sn.SN, "D");
                                    traceObj.VENDOR = allpart_smtloading.Rows[0]["MFR_NAME"].ToString();//供應名
                                    traceObj.MPN = allpart_smtloading.Rows[0]["MFR_KP_NO"].ToString();//MPN
                                    traceObj.DATE_CODE = allpart_smtloading.Rows[0]["DATE_CODE"].ToString();
                                    traceObj.LOT_CODE = allpart_smtloading.Rows[0]["LOT_CODE"].ToString();
                                }

                                list_trace.Add(traceObj);


                            }
                            break;
                    }

                    SFCDB.Insertable<R_DISC_TRACE>(list_trace).ExecuteCommand();
                }
                #endregion

                #region R_DISC_TEST
                R_DISC_HEAD testHead = headList.Find(r => r.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description);
                if (testHead != null)
                {
                    var list_test = new List<R_DISC_TEST>();
                    R_DISC_TEST testObj = null;
                    switch (sn.STATION_NAME)
                    {
                        default:
                            var oranfestation = sn.STATION_NAME;

                            if (sn.STATION_NAME == "SMT1")
                            {
                                sn.STATION_NAME = "AOI1";
                            }
                            else if (sn.STATION_NAME == "SMT2")
                            {
                                sn.STATION_NAME = "AOI2";
                            }



                            //var test_list = SFCDB.Queryable<R_TEST_JUNIPER>()
                            //    .Where(r => r.SYSSERIALNO == sn.SN && r.EVENTNAME == sn.STATION_NAME && SqlFunc.Subqueryable<R_DISC_TEST>()
                            //    .Where(t => t.SERIAL_NUMBER == r.SYSSERIALNO
                            //    && t.PHASE == r.PHASE
                            //    && t.PART_NUMBER_REVISION == r.PART_NUMBER_REVISION
                            //    && t.UNIQUE_TEST_ID == r.UNIQUE_TEST_ID
                            //    && t.TEST_STEP == r.TEST_STEP
                            //    && t.TEST_CYCLE_TEST_LOOP == r.TEST_CYCLE_TEST_LOOP
                            //    && t.CAPTURE_TIME == r.CAPTURE_TIME
                            //    && t.TEST_RESULT == r.TEST_RESULT
                            //    && t.FAILCODE == r.FAILCODE
                            //    && t.TEST_STATION_NUMBER == r.TEST_STATION_NUMBER
                            //    && t.TEST_STATION_NAME == r.TEST_STATION_NAME).NotAny())
                            //    .Select(r =>
                            //    new {
                            //        r.PART_NUMBER,
                            //        r.CM_ODM_PARTNUMBER,
                            //        r.SYSSERIALNO,
                            //        r.SERIAL_NUMBER,
                            //        r.PHASE,
                            //        r.PART_NUMBER_REVISION,
                            //        r.UNIQUE_TEST_ID,
                            //        r.TEST_START_TIMESTAMP,
                            //        r.TEST_STEP,
                            //        r.TEST_CYCLE_TEST_LOOP,
                            //        r.CAPTURE_TIME,
                            //        r.TEST_RESULT,
                            //        r.FAILCODE,
                            //        r.TEST_STATION_NUMBER,
                            //        r.TEST_STATION_NAME,
                            //        r.LOAD_DATE
                            //    }).Distinct().ToList();

                            var test_List = SFCDB.Ado.SqlQuery<R_TEST_JUNIPER>($@"
                                             SELECT  r.PART_NUMBER,
                                                    r.CM_ODM_PARTNUMBER,
                                                    r.SYSSERIALNO,
                                                    r.SERIAL_NUMBER,
                                                    r.PHASE,
                                                    r.PART_NUMBER_REVISION,
                                                    r.UNIQUE_TEST_ID,
                                                    r.TEST_START_TIMESTAMP,
                                                    r.TEST_STEP,
                                                    r.TEST_CYCLE_TEST_LOOP,
                                                    r.CAPTURE_TIME,
                                                    r.TEST_RESULT,
                                                    r.FAILCODE,
                                                    r.TEST_STATION_NUMBER,
                                                    r.TEST_STATION_NAME,
                                                    r.LOAD_DATE, r.MFG_TEST_LOG 
                                            fROM R_TEST_JUNIPER r where r.sysserialno='{sn.SN}' and r.EVENTNAME='{sn.STATION_NAME}' 
                                                and  TATIME  between
                                                   to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                                   to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                             and not exists(select*From r_disc_test t where t.SERIAL_NUMBER = r.SYSSERIALNO
                                                                    and  t.PHASE = r.PHASE
                                                                    and  t.PART_NUMBER_REVISION = r.PART_NUMBER_REVISION
                                                                    and  t.UNIQUE_TEST_ID = r.UNIQUE_TEST_ID                       
                                                                    and  t.TEST_STEP = r.TEST_STEP
                                                                    and  t.TEST_CYCLE_TEST_LOOP = r.TEST_CYCLE_TEST_LOOP
                                                                    and  t.CAPTURE_TIME = r.CAPTURE_TIME
                                                                    and  t.TEST_RESULT = r.TEST_RESULT
                                                                    and  t.FAILCODE = r.FAILCODE
                                                                    and  t.TEST_STATION_NUMBER = r.TEST_STATION_NUMBER
                                                                    and  t.TEST_STATION_NAME = r.TEST_STATION_NAME )
                                            ");

                            //var test_list = SFCDB.Queryable<R_TEST_JUNIPER>().Where(r => r.SYSSERIALNO == sn.SN && r.TEST_STATION_NAME == sn.STATION_NAME && r.TESTDATE < sn.EDIT_TIME && r.TESTDATE>sn.START_TIME).ToList();

                            foreach (var test in test_List)
                            {
                                var Test_Phase = SFCDB.Ado.SqlQuery<R_WO_TYPE>($@"select a.* From r_wo_type a, r_sn b where   b.sn='{sn.SN}' and b.workorderno like a.PREFIX||'%' and b.VALID_FLAG=1");
                                testObj = new R_DISC_TEST();
                                testObj.ID = MesDbBase.GetNewID<R_DISC_TEST>(SFCDB, _bustr);
                                testObj.SUPPLIER = CONST_SUPPLIER;
                                testObj.SUPPLIER_SITE = _plant;
                                #region 以下欄位為要抓取的數據，待編寫邏輯
                                testObj.PART_NUMBER = test.PART_NUMBER;
                                testObj.CM_ODM_PARTNUMBER = test.CM_ODM_PARTNUMBER;
                                //testObj.SERIAL_NUMBER = test.SERIAL_NUMBER;
                                testObj.SERIAL_NUMBER = test.SYSSERIALNO;
                                testObj.PHASE = Test_Phase[0].PRODUCT_TYPE == "GA" ? "Production" : Test_Phase[0].PRODUCT_TYPE == "NPI" ? "NPI" : test.PHASE;
                                testObj.PART_NUMBER_REVISION = test.PART_NUMBER_REVISION;
                                testObj.UNIQUE_TEST_ID = test.UNIQUE_TEST_ID;
                                testObj.TEST_START_TIMESTAMP = string.IsNullOrEmpty(test.TEST_START_TIMESTAMP) ? test.TEST_START_TIMESTAMP : ConvertTimeSpanToString(test.TEST_START_TIMESTAMP);
                                testObj.TEST_STEP = test.TEST_STEP == "" ? sn.STATION_NAME : test.TEST_STEP;
                                testObj.TEST_CYCLE_TEST_LOOP = test.TEST_CYCLE_TEST_LOOP;
                                testObj.CAPTURE_TIME = test.CAPTURE_TIME;
                                testObj.TEST_RESULT = test.TEST_RESULT;
                                testObj.FAILCODE = test.FAILCODE;
                                testObj.TEST_STATION_NUMBER = test.TEST_STATION_NUMBER;
                                testObj.TEST_STATION_NAME = test.TEST_STATION_NAME;
                                #endregion
                                testObj.FILE_NAME = $@"{testFileNameColumn}{testHead.DISC_FILE}";
                                testObj.CREATE_TIME = sysdate;
                                testObj.SENT_FLAG = 0;
                                testObj.VALID_FLAG = 1;
                                testObj.HEAD_ID = testHead.ID;
                                testObj.LOAD_DATE = test.LOAD_DATE;
                                testObj.MFG_TEST_LOG = test.MFG_TEST_LOG;//disc test data need this cloumn 2022-01-13
                                list_test.Add(testObj);
                            }
                            break;
                            sn.STATION_NAME = oranfestation;
                    }
                    SFCDB.Insertable<R_DISC_TEST>(list_test).ExecuteCommand();


                }
                #endregion

                #region R_DISC_MFG
                R_DISC_HEAD mfgHead = headList.Find(r => r.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description);
                if (mfgHead != null)
                {
                    //var list_mfg = new List<R_DISC_MFG>();
                    R_DISC_MFG mfgObj = null;
                    mfgObj = new R_DISC_MFG();
                    mfgObj.ID = MesDbBase.GetNewID<R_DISC_MFG>(SFCDB, _bustr);
                    mfgObj.SUPPLIER = CONST_SUPPLIER;
                    mfgObj.SUPPLIER_SITE = _plant;
                    #region 以下欄位為要抓取的數據，
                    mfgObj.PART_NUMBER = agile != null ? agile.CUSTPARTNO : sn.SKUNO;
                    mfgObj.SERIAL_NUMBER = sn.SN;
                    mfgObj.PART_NUMBER_REVISION = agile != null ? agile.REV : "";
                    mfgObj.SHOP_FLOOR_ORDER_NUMBER = sn.WORKORDERNO;
                    mfgObj.ROUTING_STEP_NUMBER = sn.STATION_NAME; //工站
                    mfgObj.WORK_STATION = sn.STATION_NAME;//工站
                    mfgObj.WORK_STATION_DESCRIPTION = sn.STATION_NAME;//工站
                    mfgObj.START_DATE_TIME = sn.START_TIME;
                    mfgObj.END_DATE_TIME = sn.START_TIME;
                    mfgObj.LOAD_DATE = Convert.ToDateTime(loadDate.ToString("yyyy-MM-dd"));
                    #endregion
                    mfgObj.FILE_NAME = $@"{manufacturingFileNameColumn}{mfgHead.DISC_FILE}";
                    mfgObj.CREATE_TIME = sysdate;
                    mfgObj.SENT_FLAG = 0;
                    mfgObj.VALID_FLAG = 1;
                    mfgObj.HEAD_ID = mfgHead.ID;
                    SFCDB.Insertable<R_DISC_MFG>(mfgObj).ExecuteCommand();
                    //list_mfg.Add(mfgObj);
                }
                #endregion
            }
            catch (Exception ex)
            {
                try
                {
                    R_MES_LOG log = new R_MES_LOG();
                    log.ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB, _bustr);
                    log.PROGRAM_NAME = "JuniperDiscData";
                    log.CLASS_NAME = "MES_DCN.Juniper.JuniperSendDISCQMSData";
                    log.FUNCTION_NAME = "CollectingPassStationData";
                    log.LOG_MESSAGE = "Juniper Send DISCQMS Data";
                    log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                    log.DATA1 = sn.SN;
                    log.DATA2 = sn.STATION_NAME;
                    log.EDIT_EMP = "Interface";
                    log.EDIT_TIME = SFCDB.GetDate();
                    SFCDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                }
                catch (Exception)
                {
                }
            }
        }

        public void CollectingRepairData(SqlSugarClient SFCDB, SqlSugarClient APDB, DateTime loadDate, string collectedDate, List<R_DISC_HEAD> headList)
        {
            DateTime sysdate = SFCDB.GetDate();
            var list_repair = new List<R_DISC_REPAIR>();
            R_DISC_REPAIR repairObj = null;

            var list_defect = new List<R_DISC_DEFECT>();
            R_DISC_DEFECT defectObj = null;
            R_DISC_HEAD repairHead = headList.Find(r => r.DISC_TYPE == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description);
            R_DISC_HEAD defectHead = headList.Find(r => r.DISC_TYPE == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description);
            #region repair && deffect
            var repair_list = SFCDB.Ado.GetDataTable($@"
                            select rrm.id,
                                   rsn.sn,
                                   rsn.workorderno,
                                   rsn.skuno,
                                   rrm.fail_station,
                                   rrm.fail_device,
                                   to_char(rrm.fail_time,'yyyy/mm/dd hh24:mi:ss') fail_time,
                                   rrf.fail_code,
                                   rrf.fail_location,
                                   rrf.fail_category,
                                   rrf.fail_process,
                                   rrf.description,
                                   rfa.action_code,
                                   rfa.REASON_CODE,
                                   rfa.NEW_KP_NO,
                                   rfa.NEW_DATE_CODE,
                                   rfa.NEW_LOT_CODE,
                                   rfa.compomentid,
                                   rfa.mfr_name,
                                   rfa.mpn,
                                   rfa.description as action_desc,
                                   rfa.keypart_sn as keypart_sn,
								   rfa.solution,
								   rfa.section_id,
								   rfa.description as REPAIR_COMMENTS,
                                   rrm.closed_flag
                              from r_sn              rsn,
                                   r_repair_main     rrm,
                                   r_repair_failcode rrf,
                                   r_repair_action   rfa
                             where rsn.valid_flag = '1'
                               and rsn.sn = rrm.sn
                               and rrm.closed_flag = '1'
                               and rrm.id = rrf.repair_main_id
                               and rrf.id = rfa.repair_failcode_id
                               and rrm.edit_time between
                               to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                               to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')");

            foreach (DataRow row in repair_list.Rows)
            {
                string mainid = row["ID"].ToString();
                string sn = row["SN"].ToString();
                string skuno = row["SKUNO"].ToString();
                string wo = row["WORKORDERNO"].ToString();
                string fail_station = row["FAIL_STATION"].ToString();
                string fail_device = row["FAIL_DEVICE"].ToString();
                string fail_time = row["FAIL_TIME"].ToString();
                string fail_code = row["FAIL_CODE"].ToString();
                string fail_location = row["FAIL_LOCATION"].ToString();
                string fail_category = row["FAIL_CATEGORY"].ToString();
                string fail_process = row["FAIL_PROCESS"].ToString();
                string fail_description = row["DESCRIPTION"].ToString();
                string REASON_CODE = row["REASON_CODE"].ToString();
                string kp_no = row["NEW_KP_NO"].ToString();
                string date_code = row["NEW_DATE_CODE"].ToString();
                string lot_code = row["NEW_LOT_CODE"].ToString();
                string compomentid = row["COMPOMENTID"].ToString();
                string mfr_name = row["MFR_NAME"].ToString();
                string mpn = row["MPN"].ToString();
                string action_desc = row["ACTION_DESC"].ToString();
                string keypart_sn = row["KEYPART_SN"].ToString();
                string solution = row["SOLUTION"].ToString();
                string section_id = row["SECTION_ID"].ToString();
                string REPAIR_COMMENTS = row["REPAIR_COMMENTS"].ToString();
                string closed_flag = row["closed_flag"].ToString();

                if (fail_station == "SMT1")
                {
                    fail_station = "AOI1";
                }
                else if (fail_station == "SMT2")
                {
                    fail_station = "AOI2";
                }
                var agile = SFCDB.Queryable<MESDataObject.Module.OM.O_AGILE_ATTR>().Where(r => r.ITEM_NUMBER == skuno && r.PLANT == SqlFunc.Replace(_plant, "FXN", ""))
                .OrderBy(r => r.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();

                var RepairMain_ex = SFCDB.Ado.GetDataTable($@"select VALUE From R_REPAIR_MAIN_EX where MAIN_ID='{mainid}' and name='UNIQUE_TEST_ID'");

                R_TEST_JUNIPER test_fail = null;

                if (RepairMain_ex.Rows.Count > 0)
                {
                    test_fail = SFCDB.Queryable<R_TEST_JUNIPER>().Where(r => r.SYSSERIALNO == sn
                    && SqlFunc.ToUpper(r.STATUS) == "FAIL" && r.TEST_STATION_NAME == fail_station && r.UNIQUE_TEST_ID == RepairMain_ex.Rows[0]["VALUE"].ToString())
                    .OrderBy(r => r.LASTEDITDT, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                }
                if (string.IsNullOrEmpty(kp_no))
                {
                    var allpartData = GetAllpartDataByLocation(APDB, sn, fail_location);
                    if (allpartData.Rows.Count > 0)
                    {
                        kp_no = allpartData.Rows[0]["KP_NO"].ToString();
                    }
                }

                #region R_DISC_REPAIR
                if (repairHead != null)
                {
                    try
                    {
                        repairObj = new R_DISC_REPAIR();
                        repairObj.ID = MesDbBase.GetNewID<R_DISC_REPAIR>(SFCDB, _bustr);
                        repairObj.SUPPLIER = CONST_SUPPLIER;
                        repairObj.SUPPLIER_SITE = _plant;
                        #region 以下欄位為要抓取的數據，待編寫邏輯
                        repairObj.PART_NUMBER = agile != null ? agile.CUSTPARTNO : skuno;
                        repairObj.CM_ODM_PARTNUMBER = agile != null ? agile.CUSTPARTNO : skuno;
                        repairObj.SERIAL_NUMBER = sn;
                        repairObj.PART_NUMBER_REVISION = agile != null ? agile.REV : "";

                        if (test_fail != null)
                        {
                            repairObj.UNIQUE_TEST_ID = test_fail.UNIQUE_TEST_ID;
                            repairObj.TEST_START_TIMESTAMP = string.IsNullOrEmpty(test_fail.TEST_START_TIMESTAMP) ? test_fail.TEST_START_TIMESTAMP : ConvertTimeSpanToString(test_fail.TEST_START_TIMESTAMP);
                            repairObj.TEST_STEP = test_fail.TEST_STEP;
                            repairObj.TEST_CYCLE_TEST_LOOP = test_fail.TEST_CYCLE_TEST_LOOP;
                            repairObj.MFG_TEST_LOG = test_fail.MFG_TEST_LOG;//disc repair data need this cloumn 2022-01-24
                        }
                        repairObj.DEFECT_CLASSIFICATION = solution;//維修人員輸入
                        repairObj.LOCATION = fail_location;
                        repairObj.COMPONENT_PART_NUMBER = kp_no;
                        repairObj.CM_ODM_COMPONENT_ID = kp_no;
                        repairObj.DEFECT_TYPE = section_id;//維修人員輸入
                        repairObj.DEFECT_DESCRIPTION = fail_description;
                        repairObj.REPAIR_STATION = fail_station;
                        repairObj.REPAIR_STATION_NAME = fail_station;
                        repairObj.REPAIR_STATUS = closed_flag;//0 待維修,1 維修結束，
                        repairObj.REPAIR_COMMENTS = REPAIR_COMMENTS;//維修人員 輸入 備註

                        repairObj.LOAD_DATE = Convert.ToDateTime(loadDate.ToString("yyyy-MM-dd"));
                        #endregion
                        repairObj.FILE_NAME = $@"{repairFileNameColumn}{repairHead.DISC_FILE}";
                        repairObj.CREATE_TIME = sysdate;
                        repairObj.SENT_FLAG = 0;
                        repairObj.VALID_FLAG = 1;
                        repairObj.HEAD_ID = repairHead.ID;
                        //list_repair.Add(repairObj);
                        SFCDB.Insertable<R_DISC_REPAIR>(repairObj).ExecuteCommand();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            R_MES_LOG log = new R_MES_LOG();
                            log.ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB, _bustr);
                            log.PROGRAM_NAME = "JuniperDiscData";
                            log.CLASS_NAME = "MES_DCN.Juniper.JuniperSendDISCQMSData";
                            log.FUNCTION_NAME = "Insert R_DISC_REPAIR";
                            log.LOG_MESSAGE = "Juniper Send DISCQMS Data";
                            log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                            log.DATA1 = sn;
                            log.DATA2 = fail_station;
                            log.EDIT_EMP = "Interface";
                            log.EDIT_TIME = SFCDB.GetDate();
                            SFCDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                #endregion

                #region R_DISC_DEFECT
                if (defectHead != null&& REASON_CODE == "E206"||fail_code.Contains("FUNC"))
                {
                    try
                    {
                        defectObj = new R_DISC_DEFECT();
                        defectObj.ID = MesDbBase.GetNewID<R_DISC_DEFECT>(SFCDB, _bustr);
                        defectObj.SUPPLIER = CONST_SUPPLIER;
                        defectObj.SUPPLIER_SITE = _plant;
                        #region 以下欄位為要抓取的數據，待編寫邏輯
                        defectObj.PART_NUMBER = agile != null ? agile.CUSTPARTNO : skuno;
                        defectObj.CM_ODM_PARTNUMBER = agile != null ? agile.CUSTPARTNO : skuno;
                        defectObj.SERIAL_NUMBER = sn;
                        defectObj.PART_NUMBER_REVISION = agile != null ? agile.REV : "";
                        if (test_fail != null)
                        {
                            defectObj.UNIQUE_TEST_ID = test_fail.UNIQUE_TEST_ID;
                            defectObj.TEST_START_TIMESTAMP = string.IsNullOrEmpty(test_fail.TEST_START_TIMESTAMP) ? test_fail.TEST_START_TIMESTAMP : ConvertTimeSpanToString(test_fail.TEST_START_TIMESTAMP);
                            defectObj.TEST_STEP = test_fail.TEST_STEP;
                            defectObj.TEST_CYCLE_TEST_LOOP = test_fail.TEST_CYCLE_TEST_LOOP;
                        }
                        defectObj.LOCATION = fail_location;
                        defectObj.COMPONENT_PART_NUMBER = kp_no;
                        defectObj.CM_ODM_COMPONENTID = kp_no;
                        defectObj.DEFECT_DESCRIPTION = fail_description;// action_desc;
                        defectObj.VENDOR = mfr_name;
                        defectObj.MPN = mpn;
                        defectObj.DATE_CODE = date_code;
                        defectObj.LOT_CODE = lot_code;
                        defectObj.SERIAL_NUMBER_CHILD = keypart_sn;
                        defectObj.SHOP_FLOOR_ORDER_NUMBER = wo;
                        defectObj.ECID = "";//默認空值
                        defectObj.LOAD_DATE = Convert.ToDateTime(sysdate.ToString("yyyy-MM-dd"));
                        #endregion
                        defectObj.FILE_NAME = $@"{defectiveFileNameColumn}{defectHead.DISC_FILE}";
                        defectObj.CREATE_TIME = sysdate;
                        defectObj.SENT_FLAG = 0;
                        defectObj.VALID_FLAG = 1;
                        defectObj.HEAD_ID = defectHead.ID;
                        SFCDB.Insertable<R_DISC_DEFECT>(defectObj).ExecuteCommand();
                        //list_defect.Add(defectObj);       
                    }
                    catch (Exception ex)
                    {
                        R_MES_LOG log = new R_MES_LOG();
                        log.ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB, _bustr);
                        log.PROGRAM_NAME = "JuniperDiscData";
                        log.CLASS_NAME = "MES_DCN.Juniper.JuniperSendDISCQMSData";
                        log.FUNCTION_NAME = "Insert R_DISC_REPAIR";
                        log.LOG_MESSAGE = "Juniper Send DISCQMS Data";
                        log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                        log.DATA1 = sn;
                        log.DATA2 = fail_station;
                        log.EDIT_EMP = "Interface";
                        log.EDIT_TIME = SFCDB.GetDate();
                        SFCDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                    }
                }
                #endregion                
            }
            #endregion
        }
        public DataTable GetAllpartDataByLocation(SqlSugarClient APDB, string sn, string location)
        {
            var sql = $@"select distinct b.KP_NO
                             from mes4.R_TR_PRODUCT_DETAIL a
                            join mes4.R_TR_CODE_DETAIL b on a.tr_code=b.tr_code
                            left join mes1.C_SMT_AP_LOCATION c on b.smt_code=c.smt_code and b.kp_no=c.kp_no
                            left join mes1.c_mfr_config e on b.mfr_code=e.mfr_code
                            where a.p_sn='{sn}' and c.LOCATION='{location}'
                            group by b.DATE_CODE, b.LOT_CODE, b.MFR_KP_NO, b.KP_NO, c.LOCATION, b.mfr_code,e.mfr_name
                            order by kp_no";
            return APDB.Ado.GetDataTable(sql);
        }
        public DataTable GetAllpartData(SqlSugarClient APDB, string sn, string process)
        {
            string sql_process = "";
            // sql_process = process == "B" ? $@" and b.process_flag in ('{process}')" : process == "T" ?$@" and b.process_flag in ('{process}')": $@" and b.process_flag in ('B','{process}')";
            switch (process)
            {
                case "B":
                    //SMT1
                    sql_process = $@" and b.process_flag = '{process}'";
                    break;
                case "T":
                    //SMT2
                    sql_process = $@" and b.process_flag = '{process}'";
                    break;
                case "D":
                    //SMTLOADING
                    sql_process = $@" and b.process_flag = '{process}' and b.station like '%LOADING%' and b.tr_code like 'S%'";
                    break;
                default:
                    throw new Exception("process error");
            }

            var sql = $@"select distinct  b.DATE_CODE, b.LOT_CODE, b.MFR_KP_NO, b.KP_NO, c.LOCATION,b.mfr_code,e.mfr_name
                             from mes4.R_TR_PRODUCT_DETAIL a
                            join mes4.R_TR_CODE_DETAIL b on a.tr_code=b.tr_code
                            left join mes1.C_SMT_AP_LOCATION c on b.smt_code=c.smt_code and b.kp_no=c.kp_no
                            left join mes1.c_mfr_config e on b.mfr_code=e.mfr_code
                            where a.p_sn='{sn}' and b.station not like '%AP%'  {sql_process}
                            group by b.DATE_CODE, b.LOT_CODE, b.MFR_KP_NO, b.KP_NO, c.LOCATION, b.mfr_code,e.mfr_name
                            order by kp_no";
            return APDB.Ado.GetDataTable(sql);
        }
        public DataTable GetAllpartTrSnData(SqlSugarClient APDB, string TRSN)
        {
            var sql = $@"select a.DATE_CODE,a.LOT_CODE ,b.MFR_NAME,a.MFR_KP_NO From mes4.r_tr_Sn a,mes1.c_mfr_config b where a.tr_sn='{TRSN}' and a.mfr_Code=b.mfr_code";
            return APDB.Ado.GetDataTable(sql);
        }
        public void GetTestWaitSendData(SqlSugarClient SFCDB, DateTime date, List<R_DISC_HEAD> list_head)
        {
            DateTime sysdate = SFCDB.GetDate();
            string str_date = date.AddDays(-1).ToString("yyyy/MM/dd").Replace('-', '/');

            var list_trace = new List<R_DISC_TRACE>();
            R_DISC_TRACE traceObj = null;

            var list_test = new List<R_DISC_TEST>();
            R_DISC_TEST testObj = null;

            var list_repair = new List<R_DISC_REPAIR>();
            R_DISC_REPAIR repairObj = null;

            var list_defect = new List<R_DISC_DEFECT>();
            R_DISC_DEFECT defectObj = null;

            var list_mfg = new List<R_DISC_MFG>();
            R_DISC_MFG mfgObj = null;
            #region 測試數據 R_DISC_TRACE
            for (var i = 0; i < 5; i++)
            {
                traceObj = new R_DISC_TRACE();
                traceObj.ID = MesDbBase.GetNewID<R_DISC_TRACE>(SFCDB, _bustr);
                traceObj.SUPPLIER = CONST_SUPPLIER;
                traceObj.SUPPLIER_SITE = _plant;
                #region 以下欄位為要抓取的數據，待編寫邏輯
                traceObj.PART_NUMBER = "711-065484";//客戶 PCBA 板子料號
                traceObj.CM_ODM_PARTNUMBER = "711-065484";//PCBA 板子料號
                traceObj.SERIAL_NUMBER = "CARL7610";
                traceObj.PART_NUMBER_REVISION = "14";//PCBA 板子版本
                traceObj.SHOP_FLOOR_ORDER_NUMBER = "100000000000";//富士康工單
                traceObj.COMPONENT_PART_NUMBER = "320-000328";// KP 和料表
                traceObj.CM_COMPONENT_PART_NUMBER = "320-000328JNE";//COMPONENT_PART_NUMBER
                traceObj.ASSEMBLY_STATION = "M130";//MES STATION
                traceObj.ASSEMBLY_STATION_DESCRIPTION = "COMPENT TRACKING TOP";//MES STATION
                traceObj.LOCATION = "D6611";//
                traceObj.VENDOR = "ON SEMICONDUCTOR";//供應名
                traceObj.MPN = "MMBD914LT1G";//MPN
                traceObj.DATE_CODE = "2008";
                traceObj.LOT_CODE = "MQK082706";
                traceObj.SERIAL_NUMBER_CHILD = "";//KP value
                traceObj.ECID = "";//固定空值
                traceObj.QTY = "1";//固定1
                traceObj.LOAD_DATE = Convert.ToDateTime(sysdate.ToString("yyyy-MM-dd"));//上傳日期//上料時間或綁定時間
                #endregion
                traceObj.FILE_NAME = "";
                traceObj.CREATE_TIME = sysdate;
                traceObj.SENT_FLAG = 0;
                traceObj.VALID_FLAG = 1;
                list_trace.Add(traceObj);
            }
            SFCDB.Insertable<R_DISC_TRACE>(list_trace).ExecuteCommand();
            #endregion

            #region 測試數據 R_DISC_TEST
            for (var i = 0; i < 5; i++)
            {
                testObj = new R_DISC_TEST();
                testObj.ID = MesDbBase.GetNewID<R_DISC_TEST>(SFCDB, _bustr);
                testObj.SUPPLIER = CONST_SUPPLIER;
                testObj.SUPPLIER_SITE = _plant;
                #region 以下欄位為要抓取的數據，待編寫邏輯
                testObj.PART_NUMBER = "711-065484";
                testObj.CM_ODM_PARTNUMBER = "711-065484-CMRE";
                testObj.SERIAL_NUMBER = "CARL7610";
                testObj.PHASE = "";
                testObj.PART_NUMBER_REVISION = "14";
                testObj.UNIQUE_TEST_ID = "51200000000000000";
                testObj.TEST_START_TIMESTAMP = "";
                testObj.TEST_STEP = "ICT";
                testObj.TEST_CYCLE_TEST_LOOP = "1";
                testObj.CAPTURE_TIME = sysdate;
                testObj.TEST_RESULT = "Fail";
                testObj.FAILCODE = "91053";
                testObj.TEST_STATION_NUMBER = "M401";
                testObj.TEST_STATION_NAME = "ICT";
                testObj.LOAD_DATE = Convert.ToDateTime(sysdate.ToString("yyyy-MM-dd"));
                testObj.MFG_TEST_LOG = "463411138100370f034a449";//test data
                #endregion
                testObj.FILE_NAME = "";
                testObj.CREATE_TIME = sysdate;
                testObj.SENT_FLAG = 0;
                testObj.VALID_FLAG = 1;
                list_test.Add(testObj);
            }
            SFCDB.Insertable<R_DISC_TEST>(list_test).ExecuteCommand();
            #endregion

            #region 測試數據  R_DISC_DEFECT          
            for (var i = 0; i < 5; i++)
            {
                defectObj = new R_DISC_DEFECT();
                defectObj.ID = MesDbBase.GetNewID<R_DISC_DEFECT>(SFCDB, _bustr);
                defectObj.SUPPLIER = CONST_SUPPLIER;
                defectObj.SUPPLIER_SITE = _plant;
                #region 以下欄位為要抓取的數據，待編寫邏輯
                defectObj.PART_NUMBER = "611-107327";
                defectObj.CM_ODM_PARTNUMBER = "611-107327-DNPT";
                defectObj.SERIAL_NUMBER = "611-107327062101190020";
                defectObj.PART_NUMBER_REVISION = "6";
                defectObj.UNIQUE_TEST_ID = "11200000000000000";
                defectObj.TEST_START_TIMESTAMP = "";
                defectObj.TEST_STEP = "ICT";
                defectObj.TEST_CYCLE_TEST_LOOP = "1";
                defectObj.LOCATION = "D6008";
                defectObj.COMPONENT_PART_NUMBER = "320-067448";
                defectObj.CM_ODM_COMPONENTID = "320-067448JNE";
                defectObj.DEFECT_DESCRIPTION = "Upside Down/ Overturn";
                defectObj.VENDOR = "KINGBRIGHT";
                defectObj.MPN = "KPBA-3010DGSYKC";
                defectObj.DATE_CODE = "2035";
                defectObj.LOT_CODE = "MST2080112-001A";
                defectObj.SERIAL_NUMBER_CHILD = "";
                defectObj.SHOP_FLOOR_ORDER_NUMBER = "100000000000";
                defectObj.ECID = "";
                defectObj.LOAD_DATE = Convert.ToDateTime(sysdate.ToString("yyyy-MM-dd"));
                #endregion
                defectObj.FILE_NAME = "";
                defectObj.CREATE_TIME = sysdate;
                defectObj.SENT_FLAG = 0;
                defectObj.VALID_FLAG = 1;
                list_defect.Add(defectObj);
            }
            SFCDB.Insertable<R_DISC_DEFECT>(list_defect).ExecuteCommand();
            #endregion

            #region 測試數據 R_DISC_MFG           
            for (var i = 0; i < 5; i++)
            {
                mfgObj = new R_DISC_MFG();
                mfgObj.ID = MesDbBase.GetNewID<R_DISC_MFG>(SFCDB, _bustr);
                mfgObj.SUPPLIER = CONST_SUPPLIER;
                mfgObj.SUPPLIER_SITE = _plant;
                #region 以下欄位為要抓取的數據，待編寫邏輯
                mfgObj.PART_NUMBER = "711-065484";
                mfgObj.SERIAL_NUMBER = "CARL7610";
                mfgObj.PART_NUMBER_REVISION = "14";
                mfgObj.SHOP_FLOOR_ORDER_NUMBER = "100004190949";
                mfgObj.ROUTING_STEP_NUMBER = "5DX"; //工站
                mfgObj.WORK_STATION = "M136";//工站
                mfgObj.WORK_STATION_DESCRIPTION = "JNPR TOP 5DX";//工站
                mfgObj.START_DATE_TIME = sysdate;
                mfgObj.END_DATE_TIME = sysdate;
                mfgObj.LOAD_DATE = Convert.ToDateTime(sysdate.ToString("yyyy-MM-dd"));
                #endregion
                mfgObj.FILE_NAME = "";
                mfgObj.CREATE_TIME = sysdate;
                mfgObj.SENT_FLAG = 0;
                mfgObj.VALID_FLAG = 1;
                list_mfg.Add(mfgObj);
            }
            SFCDB.Insertable<R_DISC_MFG>(list_mfg).ExecuteCommand();
            #endregion

            #region 測試數據 R_DISC_REPAIR
            for (var i = 0; i < 5; i++)
            {
                repairObj = new R_DISC_REPAIR();
                repairObj.ID = MesDbBase.GetNewID<R_DISC_REPAIR>(SFCDB, _bustr);
                repairObj.SUPPLIER = CONST_SUPPLIER;
                repairObj.SUPPLIER_SITE = _plant;
                #region 以下欄位為要抓取的數據，待編寫邏輯
                repairObj.PART_NUMBER = "711-065484";
                repairObj.CM_ODM_PARTNUMBER = "711-065484-CMRE";
                repairObj.SERIAL_NUMBER = "CARL7610";
                repairObj.PART_NUMBER_REVISION = "14";
                repairObj.UNIQUE_TEST_ID = "42162001210206900";
                repairObj.TEST_START_TIMESTAMP = "";
                repairObj.TEST_STEP = "ICT";
                repairObj.TEST_CYCLE_TEST_LOOP = "1";
                repairObj.DEFECT_CLASSIFICATION = "";
                repairObj.LOCATION = "D6008";
                repairObj.COMPONENT_PART_NUMBER = "320-067448";
                repairObj.CM_ODM_COMPONENT_ID = "320-067448JNE";
                repairObj.DEFECT_TYPE = "placement";
                repairObj.DEFECT_DESCRIPTION = "Upside Down/ Overturn";
                repairObj.REPAIR_STATION = "772M";
                repairObj.REPAIR_STATION_NAME = "JNPR REWORK 1";
                repairObj.REPAIR_STATUS = "Fixed";
                repairObj.REPAIR_COMMENTS = "";
                repairObj.LOAD_DATE = Convert.ToDateTime(sysdate.ToString("yyyy-MM-dd"));
                #endregion
                repairObj.FILE_NAME = "";
                repairObj.CREATE_TIME = sysdate;
                repairObj.SENT_FLAG = 0;
                repairObj.VALID_FLAG = 1;
                list_repair.Add(repairObj);
            }
            SFCDB.Insertable<R_DISC_REPAIR>(list_repair).ExecuteCommand();
            #endregion

            list_head.ForEach(r =>
            {
                r.COLLECT_FLAG = "1";
            });
            SFCDB.Updateable<R_DISC_HEAD>(list_head).ExecuteCommand();
        }

        public void SFTPTest()
        {
            SFTPHelper sftpHelp = null;
            List<SFTPHelper> list = new List<SFTPHelper>();
            List<Task> listTask = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keypath, KeyType.RSA_PRIVATE);
                TaskFactory taskFactory = new TaskFactory();
                listTask.Add(taskFactory.StartNew((data) =>
                {
                    try
                    {
                        var sftp = (SFTPHelper)data;
                        sftp.Connect();
                        var kk = sftp.GetFileList("FJZ", "");
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }, sftpHelp));
            }
            Task.WaitAll(listTask.ToArray());
            #region 多綫程生成文件 有問題SFT                   
            //TaskFactory taskFactory = new TaskFactory();
            //Task task = taskFactory.StartNew(
            //        (data) =>
            //        {
            //            DiscBase discData = (DiscBase)data;
            //            try
            //            {                                  
            //                ConvertToFile(discData);                                   
            //                try
            //                {
            //                    discData.sfcdb.Dispose();
            //                }
            //                catch (Exception)
            //                {
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MesLog.Info($"{discData.FileName} convert to file error. {ex.Message}");
            //            }                               
            //        }, disc);

            //taskList.Add(task);
            //Task.WaitAll(taskList.ToArray());
            #endregion
        }

        public void TraceDiscTest()
        {

        }

        public string ConvertTimeSpanToString(string timeSpan)
        {
            DateTime date;
            bool bTime = DateTime.TryParse(timeSpan, out date);
            if (bTime)
                return timeSpan;
            TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            DateTime dateTime = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            double _timeSpan = Convert.ToDouble(timeSpan);
            if (timeSpan.Length == 10)
            {
                dateTime = startTime.AddMinutes(_timeSpan);
            }
            else if (timeSpan.Length == 13)
            {
                dateTime = startTime.AddMilliseconds(_timeSpan);
            }
            else if (timeSpan.Length == 15)
            {
                dateTime = startTime.AddMilliseconds(_timeSpan / 100);
            }
            else if (timeSpan.Length == 18)
            {
                dateTime = startTime.AddMilliseconds(_timeSpan / 10000);
            }
            else
            {
                throw new Exception("TEST_START_TIMESTAMP Error!");
            }
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    public enum DiscFileType
    {
        /// <summary>
        /// component_trace
        /// </summary>
        [EnumExtensions.EnumValueAttribute("TRACE")]
        TRACE,
        /// <summary>
        /// test
        /// </summary>
        [EnumExtensions.EnumValueAttribute("TEST")]
        TEST,
        /// <summary>
        /// defective
        /// </summary>       
        [EnumExtensions.EnumValueAttribute("DEFECT")]
        DEFECT,
        /// <summary>
        /// manufacturing
        /// </summary>
        [EnumExtensions.EnumValueAttribute("MFG")]
        MFG,
        /// <summary>
        /// Repair
        /// </summary>
        [EnumExtensions.EnumValueAttribute("REPAIR")]
        REPAIR
    }

    public class DiscBase
    {

        public SqlSugarClient sfcdb;
        public SFTPHelper sftpHelp;
        public string localFilePath;
        public string backupPath;
        public string remotePath;
        public string plant;
        public bool bSend = true;

        public R_DISC_HEAD DiscHeadObj { get; set; }
        public object SendList { get; set; }
        public string FileFullName { get; set; }
        public string GZFileName { get; set; }

        public void SaveCSVFile<T>()
        {
            try
            {
                List<T> list = (List<T>)SendList;
                List<string> no_sent_column = new List<string>() { "ID", "CREATE_TIME", "SENT_FLAG", "SENT_TIME", "HEAD_ID", "VALID_FLAG", "MFG_TEST_LOG" };
                Dictionary<string, string> dict_trace = new Dictionary<string, string>() {
                    { "SERIAL_NUMBER_CHILD","SERIAL_NUMBER_CHILD"},
                    { "ECID","Ecid(ASIC)"}
                };
                Dictionary<string, Dictionary<string, string>> dict_change_column = new Dictionary<string, Dictionary<string, string>>()
                {
                    {
                        "TRACE",new Dictionary<string, string>(){
                            { "SERIAL_NUMBER_CHILD","Serial_Number(Child)"},
                            { "ECID","Ecid(ASIC)"}
                        }
                    },
                    {
                        "TEST",new Dictionary<string, string>(){
                            { "FAILCODE","Failcode_(Error Code)"}
                        }
                    },
                    {
                        "DEFECT",new Dictionary<string, string>(){
                            { "SERIAL_NUMBER_CHILD","SERIAL_NUMBER_CHILD"}
                        }
                    },
                    {
                        "MFG",new Dictionary<string, string>(){
                            { "ROUTING_STEP_NUMBER","Routing Step#"},
                            { "WORK_STATION","Work Station"},
                            { "WORK_STATION_DESCRIPTION","Work Station Description"},
                            { "START_DATE_TIME","Start Date and Time"},
                            { "END_DATE_TIME","End Date and Time"},
                        }
                    },
                };

                Type type = typeof(T);
                PropertyInfo[] column_list = type.GetProperties();
                column_list = column_list.Where(c => !no_sent_column.Contains(c.Name)).ToArray();
                using (var sw = new StreamWriter(new FileStream(FileFullName, FileMode.Create), Encoding.GetEncoding("UTF-8")))
                {
                    StringBuilder column = new StringBuilder();
                    StringBuilder value = new StringBuilder();
                    foreach (var c in column_list)
                    {
                        var type_change = dict_change_column.Where(d => d.Key == DiscHeadObj.DISC_TYPE.ToUpper());
                        if (type_change.Count() > 0)
                        {
                            var name = type_change.FirstOrDefault().Value.Where(f => f.Key == c.Name);
                            if (name.Count() > 0)
                            {
                                column.Append(name.FirstOrDefault().Value);
                            }
                            else
                            {
                                column.Append(c.Name);
                            }

                        }
                        else
                        {
                            column.Append(c.Name);
                        }
                        //column.Append(",");
                        column.Append("|");
                    }
                    column.Remove(column.Length - 1, 1);
                    sw.WriteLine(column);
                    foreach (var l in list)
                    {
                        var t = typeof(T).GetFields();
                        value.Remove(0, value.Length);
                        foreach (var v in column_list)
                        {
                            string temp = "";
                            if (v.GetValue(l) != null)
                            {
                                var column_value = v.GetValue(l);
                                if (v.Name == "LOAD_DATE")
                                {
                                    temp = ((DateTime)column_value).ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    temp = string.IsNullOrEmpty(column_value.ToString()) ? "" : column_value.ToString();
                                }
                            }
                            //value.Append(CSVHandlerStr(temp));
                            //value.Append(",");
                            value.Append(temp);
                            value.Append("|");
                        }
                        value.Remove(value.Length - 1, 1);
                        sw.WriteLine(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CompressFile()
        {
            if (bSend)
            {
                using (FileStream originalFileStream = new FileStream($"{localFilePath}{DiscHeadObj.DISC_FILE}", FileMode.Open))
                {
                    using (FileStream compressedFileStream = File.Create($"{localFilePath}{DiscHeadObj.DISC_FILE}.gz"))
                    {
                        using (GZipStream compressioonStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressioonStream);
                            GZFileName = $"{DiscHeadObj.DISC_FILE}.gz";
                        }
                    }
                }
                File.Delete($"{localFilePath}{DiscHeadObj.DISC_FILE}");
            }
        }
        public void UploadSFTP()
        {
            if (bSend)
            {
                //sftpHelp.Put($@"{localFilePath}{FileName}", $@"{remotePath}{DiscHeadObj.DISC_FILE}");
                sftpHelp.Put($@"{localFilePath}{GZFileName}", $@"{remotePath}{GZFileName}");
            }
        }
        public void DoBackups()
        {
            try
            {
                if (bSend)
                {
                    if (!Directory.Exists(backupPath))
                    {
                        Directory.CreateDirectory(backupPath);
                    }
                    if (File.Exists($@"{backupPath}{GZFileName}"))
                    {
                        File.Move($@"{backupPath}{GZFileName}", $@"{backupPath}backup_{DateTime.Now.ToString("yyyyMMddhhmmss")}_{GZFileName}");
                        File.Move($@"{localFilePath}{GZFileName}", $@"{backupPath}{GZFileName}");
                        Thread.Sleep(200);
                    }
                    else
                    {
                        File.Move($@"{localFilePath}{GZFileName}", $@"{backupPath}{GZFileName}");
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    string pathCSV = $@"{localFilePath}\CSV";
                    if (!Directory.Exists(pathCSV))
                    {
                        Directory.CreateDirectory(pathCSV);
                    }
                    pathCSV = $@"{pathCSV}\";
                    if (File.Exists($@"{pathCSV}{DiscHeadObj.DISC_FILE}"))
                    {
                        File.Move($@"{pathCSV}{DiscHeadObj.DISC_FILE}", $@"{pathCSV}backup_{DateTime.Now.ToString("yyyyMMddhhmmss")}_{DiscHeadObj.DISC_FILE}");
                        File.Move($@"{localFilePath}{DiscHeadObj.DISC_FILE}", $@"{pathCSV}{DiscHeadObj.DISC_FILE}");
                        Thread.Sleep(200);
                    }
                    else
                    {
                        File.Move($@"{localFilePath}{DiscHeadObj.DISC_FILE}", $@"{pathCSV}{DiscHeadObj.DISC_FILE}");
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateConvertFlag()
        {
            DiscHeadObj.CONVERT_FLAG = "1";
            DiscHeadObj.EDIT_TIME = sfcdb.GetDate();
            sfcdb.Updateable<R_DISC_HEAD>(DiscHeadObj).ExecuteCommand();
        }
        public void UpdateHeadSendFlag()
        {
            if (bSend)
            {
                DiscHeadObj.SEND_FLAG = "1";
                DiscHeadObj.EDIT_TIME = sfcdb.GetDate();
                sfcdb.Updateable<R_DISC_HEAD>(DiscHeadObj).ExecuteCommand();
            }
        }
        public virtual void GetSendDataList()
        {
            throw new NotImplementedException();
        }
        public virtual void UpdateSendFlag()
        {
            throw new NotImplementedException();
        }
    }

    public class TraceDisc : DiscBase
    {

        public override void GetSendDataList()
        {
            List<R_DISC_TRACE> list = sfcdb.Queryable<R_DISC_TRACE>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            this.SendList = list;
        }
        public override void UpdateSendFlag()
        {
            if (bSend)
            {
                List<R_DISC_TRACE> list = (List<R_DISC_TRACE>)SendList;
                if (list != null && list.Count > 0)
                {
                    DateTime uploadTime = sfcdb.GetDate();
                    list.ForEach(l =>
                    {
                        l.SENT_FLAG = 1;
                        l.SENT_TIME = uploadTime;
                    });
                    sfcdb.Updateable<R_DISC_TRACE>(list).ExecuteCommand();
                }
            }
        }
    }
    public class TestDisc : DiscBase
    {
        public override void GetSendDataList()
        {
            List<R_DISC_TEST> list = sfcdb.Queryable<R_DISC_TEST>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            this.SendList = list;
        }
        public override void UpdateSendFlag()
        {
            if (bSend)
            {
                List<R_DISC_TEST> list = (List<R_DISC_TEST>)SendList;
                if (list != null && list.Count > 0)
                {
                    DateTime uploadTime = sfcdb.GetDate();
                    list.ForEach(l =>
                    {
                        l.SENT_FLAG = 1;
                        l.SENT_TIME = uploadTime;
                    });
                    sfcdb.Updateable<R_DISC_TEST>(list).ExecuteCommand();
                }
            }
        }
    }
    public class DefectDisc : DiscBase
    {
        public override void GetSendDataList()
        {
            List<R_DISC_DEFECT> list = sfcdb.Queryable<R_DISC_DEFECT>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            this.SendList = list;
        }
        public override void UpdateSendFlag()
        {
            if (bSend)
            {
                List<R_DISC_DEFECT> list = (List<R_DISC_DEFECT>)SendList;
                if (list != null && list.Count > 0)
                {
                    DateTime uploadTime = sfcdb.GetDate();
                    list.ForEach(l =>
                    {
                        l.SENT_FLAG = 1;
                        l.SENT_TIME = uploadTime;
                    });
                    sfcdb.Updateable<R_DISC_DEFECT>(list).ExecuteCommand();
                }
            }
        }
    }
    public class RepairDisc : DiscBase
    {
        public override void GetSendDataList()
        {
            List<R_DISC_REPAIR> list = sfcdb.Queryable<R_DISC_REPAIR>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            this.SendList = list;
        }
        public override void UpdateSendFlag()
        {
            if (bSend)
            {
                List<R_DISC_REPAIR> list = (List<R_DISC_REPAIR>)SendList;
                if (list != null && list.Count > 0)
                {
                    DateTime uploadTime = sfcdb.GetDate();
                    list.ForEach(l =>
                    {
                        l.SENT_FLAG = 1;
                        l.SENT_TIME = uploadTime;
                    });
                    sfcdb.Updateable<R_DISC_REPAIR>(list).ExecuteCommand();
                }
            }
        }
    }
    public class MFGDisc : DiscBase
    {
        public override void GetSendDataList()
        {
            List<R_DISC_MFG> list = sfcdb.Queryable<R_DISC_MFG>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            this.SendList = list;
        }
        public override void UpdateSendFlag()
        {
            if (bSend)
            {
                List<R_DISC_MFG> list = (List<R_DISC_MFG>)SendList;
                if (list != null && list.Count > 0)
                {
                    DateTime uploadTime = sfcdb.GetDate();
                    list.ForEach(l =>
                    {
                        l.SENT_FLAG = 1;
                        l.SENT_TIME = uploadTime;
                    });
                    sfcdb.Updateable<R_DISC_MFG>(list).ExecuteCommand();
                }
            }
        }
    }

    public class DiscTaskObj
    {
        public SqlSugarClient SFCDB { get; set; }
        public SqlSugarClient APDB { get; set; }
        public DateTime LoadDate { get; set; }
        public string CollectedDate { get; set; }
        public object Data { get; set; }
        public List<R_DISC_HEAD> HeadList { get; set; }
    }
}
