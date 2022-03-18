using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.Common;
using SqlSugar;
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
using static MESDataObject.Common.EnumExtensions;

namespace MES_DCN.Juniper
{
    public class DISCQMSDataNew
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

        public DISCQMSDataNew(string mesdbstr, string apdbstr, string bustr, string filepath, string filebackpath, string remotepath, string keypath, string plant, bool IsPROD)
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

        public void Run(string filetype, DateTime collectedDate,string taskNum, bool IsSend = true) 
        {
            try
            {
                SqlSugarClient APDB = OleExec.GetSqlSugarClient(_apdbstr, false);
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
                List<R_DISC_HEAD> headList = MakeDiscHead(SFCDB, filetype, collectedDate, IsSend);
                string collectedDateStr = collectedDate.ToString("yyyy/MM/dd").Replace('-', '/');
                List<R_SN_STATION_DETAIL> snDetailList = GetSnDetailList(SFCDB, collectedDateStr);
                DateTime loadDate = SFCDB.GetDate();
                
                int totalTask = 0;
                if (string.IsNullOrEmpty(taskNum))
                {
                    totalTask = 10;
                }
                else
                {
                    try
                    {
                        Int32.TryParse(taskNum, out totalTask);
                    }
                    catch (Exception)
                    {
                        totalTask = 10;
                    }
                }
                if (snDetailList.Count > totalTask && snDetailList.Count > 100 && totalTask != 1)
                {
                    #region Multiple Task TRACE,TEST,MFG
                    MesLog.Info("Begin CollectingPassTask ...");
                    int taskExecuteData = snDetailList.Count % totalTask == 0 ? snDetailList.Count / totalTask : snDetailList.Count / totalTask + 1;
                    List<Task> collectTaskList = new List<Task>();
                    for (int t = 0; t < totalTask; t++)
                    {
                        List<R_SN_STATION_DETAIL> detailTemppList = snDetailList.Skip(t * taskExecuteData).Take(taskExecuteData).ToList();
                        Task collectTask = new Task(() =>
                        {
                            DiscTaskObj taskData = new DiscTaskObj()
                            {
                                SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false),
                                APDB = OleExec.GetSqlSugarClient(_apdbstr, false),                                
                                LoadDate = loadDate,
                                CollectedDate = collectedDateStr,
                                Data = detailTemppList,
                                HeadList = headList
                            };
                            CollectingPassTaskAction(taskData);
                        });
                        collectTask.Start();
                        collectTaskList.Add(collectTask);
                    }
                    Task.WaitAll(collectTaskList.ToArray());
                    MesLog.Info("End CollectingPassTask ...");
                    #endregion
                }
                else
                {
                    MesLog.Info("Begin CollectingPassStationData ...");
                    foreach (var snDetail in snDetailList)
                    {
                        CollectingPassStationData(SFCDB, APDB, loadDate, collectedDateStr, snDetail, headList);
                    }
                    MesLog.Info("End CollectingPassStationData ...");
                }

                MesLog.Info("Begin CollectingRepairData ...");
                SqlSugarClient APDB_RE = OleExec.GetSqlSugarClient(_apdbstr, false);
                SqlSugarClient SFCDB_RE = OleExec.GetSqlSugarClient(_mesdbstr, false);                
                CollectingRepairData(SFCDB_RE, APDB_RE, loadDate, collectedDateStr, headList);
                MesLog.Info("End CollectingRepairData ...");

                MesLog.Info("Begin UpdateCollectFlag ...");
                SqlSugarClient SFCDB_HE = OleExec.GetSqlSugarClient(_mesdbstr, false);                
                UpdateCollectFlag(SFCDB_HE, headList);
                MesLog.Info("End UpdateCollectFlag ...");

                MesLog.Info("Begin ConvertCSVAction ...");
                //List<Task> convertTaskList = new List<Task>();
                List<DiscDataBase> discBaseList = new List<DiscDataBase>();
                foreach (var headObj in headList)
                {
                    DiscDataBase discData = null;
                    if (headObj.DISC_TYPE == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new TraceDiscData();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new TestDiscData();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new DefectDiscData();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new RepairDiscData();
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new MFGDiscData();
                    }
                    else
                    {
                        continue;
                    }
                    discData.IsSend = IsSend;
                    discData.SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
                    discData.LocalFilePath = _filepath;
                    discData.BackupPath = _filebackuppath;
                    discData.DiscHeadObj = headObj;
                    discData.FileFullName = $@"{_filepath}{headObj.DISC_FILE}";
                    discData.BU = _bustr;
                    discBaseList.Add(discData);

                    try
                    {
                        ConvertCSVAction(discData);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            SqlSugarClient LogDB = SFCDB_HE;// OleExec.GetSqlSugarClient(_mesdbstr, false);
                            R_MES_LOG log = new R_MES_LOG();
                            log.ID = MesDbBase.GetNewID<R_MES_LOG>(LogDB, _bustr);
                            log.PROGRAM_NAME = "JuniperDiscData";
                            log.CLASS_NAME = "MES_DCN.Juniper.DISCQMSDataNew";
                            log.FUNCTION_NAME = "Multiple Task ConvertCSVAction";
                            log.LOG_MESSAGE = " Multiple Task Running ConvertCSVAction Fail";
                            log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                            log.DATA1 = headObj.DISC_TYPE;
                            log.EDIT_EMP = "Interface";
                            log.EDIT_TIME = LogDB.GetDate();
                            LogDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    //Task convertTask = new Task(() =>
                    //{
                    //    try
                    //    {
                    //        ConvertCSVAction(discData);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        try
                    //        {
                    //            SqlSugarClient LogDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
                    //            R_MES_LOG log = new R_MES_LOG();
                    //            log.ID = MesDbBase.GetNewID<R_MES_LOG>(LogDB, _bustr);
                    //            log.PROGRAM_NAME = "JuniperDiscData";
                    //            log.CLASS_NAME = "MES_DCN.Juniper.DISCQMSDataNew";
                    //            log.FUNCTION_NAME = "Multiple Task ConvertCSVAction";
                    //            log.LOG_MESSAGE = " Multiple Task Running ConvertCSVAction Fail";
                    //            log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                    //            log.DATA1 = headObj.DISC_TYPE;
                    //            log.EDIT_EMP = "Interface";
                    //            log.EDIT_TIME = LogDB.GetDate();
                    //            LogDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                    //        }
                    //        catch (Exception)
                    //        {
                    //        }
                    //    }

                    //});
                    //convertTask.Start();
                    //convertTaskList.Add(convertTask);
                }
                //Task.WaitAll(convertTaskList.ToArray());
                MesLog.Info("End ConvertCSVAction ...");

                if (IsSend)
                {
                    UploadToSftp(discBaseList);
                }
                else
                {
                    
                    foreach (var item in discBaseList)
                    {
                        MesLog.Info($@"Begin {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} UpdateSendFlag ...");
                        item.UpdateSendFlag();
                        MesLog.Info($@"End {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} UpdateSendFlag ...");
                    }                   
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Class:MES_DCN.Juniper.DISCQMSDataNew;Function:Run;Error Msg: {ex.Message}");
            }
        }

        public List<R_DISC_HEAD> MakeDiscHead(SqlSugarClient SFCDB, string filetype, DateTime collectDate,bool IsSend)
        {
            try
            {
                var typeList = filetype.Split(',').ToList();
                R_DISC_HEAD discHead = null;
                DateTime loadDate = SFCDB.GetDate();
                string key = collectDate.ToString("yyyy-MM-dd");
                string fileName = $"{collectDate.ToString("yyyyMMddHHmmss")}.csv";
                if (IsSend)
                {
                    SFCDB.Updateable<R_DISC_HEAD>().SetColumns(r => new R_DISC_HEAD { VALID_FLAG = 0 }).Where(r => r.DISC_KEY == key && r.PLANT == _plant).ExecuteCommand();
                }
                List<R_DISC_HEAD> headList = new List<R_DISC_HEAD>();
                foreach (var type in typeList)
                {
                    discHead = new R_DISC_HEAD();
                    discHead.ID = MesDbBase.GetNewID<R_DISC_REPAIR>(SFCDB, _bustr);
                    discHead.DISC_KEY = key;
                    if (type == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                    {
                        discHead.DISC_FILE = $@"{_plant}_TRAC_{fileName}";
                    }
                    else if (type == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                    {
                        discHead.DISC_FILE = $@"{_plant}_TEST_{fileName}";
                    }
                    else if (type == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                    {
                        discHead.DISC_FILE = $@"{_plant}_DTRC_{fileName}";
                    }
                    else if (type == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                    {
                        discHead.DISC_FILE = $@"{_plant}_MREP_{fileName}";
                    }
                    else if (type == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                    {
                        discHead.DISC_FILE = $@"{_plant}_MANF_{fileName}";
                    }
                    else
                    {
                        throw new Exception($@"Input type error!");
                    }
                    discHead.COLLECT_FLAG = "0";
                    discHead.CONVERT_FLAG = "0";
                    discHead.SEND_FLAG = "0";
                    discHead.VALID_FLAG = IsSend ? 1 : 2;
                    discHead.CREATE_TIME = SFCDB.GetDate();
                    discHead.EDIT_TIME = SFCDB.GetDate();
                    discHead.DISC_TYPE = type;
                    discHead.PLANT = _plant;
                    SFCDB.Insertable<R_DISC_HEAD>(discHead).ExecuteCommand();
                    headList.Add(discHead);
                }
                return headList;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Running MakeDiscHead Fail;Fail Msg:{ex.Message}");
            }
        }

        public List<R_SN_STATION_DETAIL> GetSnDetailList(SqlSugarClient SFCDB,string collectedDateStr)
        {
            try
            {
                return SFCDB.Ado.SqlQuery<R_SN_STATION_DETAIL>($@"
                                    select DISTINCT SN,SKUNO,WORKORDERNO, STATION_NAME,EDIT_TIME
                                      from r_sn_station_detail a
                                     where a.edit_time between
                                           to_date('{collectedDateStr} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                           to_date('{collectedDateStr} 23:59:59', 'yyyy/mm/dd hh24:mi:ss') AND VALID_FLAG=1 
                                       and exists
                                     (select * from c_station b where a.station_name = b.station_name) order by EDIT_TIME ");
            }
            catch (Exception ex)
            {
                throw new Exception($@"Running GetSnDetailList Fail;Fail Msg:{ex.Message}");
            }            
        }
       
        public void CollectingPassTaskAction(DiscTaskObj inputData)
        {
            try
            {
                foreach (var item in (List<R_SN_STATION_DETAIL>)inputData.Data)
                {
                    CollectingPassStationData(inputData.SFCDB, inputData.APDB, inputData.LoadDate, inputData.CollectedDate, item, inputData.HeadList);
                }
                //try
                //{
                //    inputData.SFCDB.Dispose();
                //    inputData.APDB.Dispose();
                //}
                //catch (Exception)
                //{
                //}
            }
            catch (Exception ex)
            {
                try
                {
                    SqlSugarClient LogDB = inputData.SFCDB;// OleExec.GetSqlSugarClient(_mesdbstr, false);
                    R_MES_LOG log = new R_MES_LOG();
                    log.ID = MesDbBase.GetNewID<R_MES_LOG>(LogDB, _bustr);
                    log.PROGRAM_NAME = "JuniperDiscData";
                    log.CLASS_NAME = "MES_DCN.Juniper.DISCQMSDataNew";
                    log.FUNCTION_NAME = "CollectingPassTaskAction";
                    log.LOG_MESSAGE = "Running CollectingPassTaskAction Fail";
                    log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                    log.DATA1 = inputData.CollectedDate;
                    log.DATA2 = inputData.LoadDate.ToString("yyyyy/MM/dd HH:mm:ss");
                    log.EDIT_EMP = "Interface";
                    log.EDIT_TIME = LogDB.GetDate();
                    LogDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                }
                catch (Exception)
                {
                }
            }
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
                                    ");
                            list_trace = new List<R_DISC_TRACE>();
                            foreach (var kp in kp_list)
                            {

                                var disc_Kp = SFCDB.Ado.SqlQuery<R_SN_KP>($@" select * From r_sn_kp where sn='{sn.SN}'and VALID_FLAG=1 and STATION='MATL_LINK' ");
                                

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
                                if (kp.KP_NAME == "AutoKP")
                                {
                                    //吧这些值清空
                                    traceObj.VENDOR = "";//供應名
                                    traceObj.DATE_CODE ="";
                                    traceObj.LOT_CODE = "";
                                    traceObj.MPN = "";
                                    traceObj.COMPONENT_PART_NUMBER = kp.MPN;
                                    traceObj.CM_COMPONENT_PART_NUMBER = kp.MPN;
                                    traceObj.LOCATION = "";
                                    traceObj.SERIAL_NUMBER_CHILD = "";
                                    if (disc_Kp.Count == 0 && kp.PARTNO != sn.SKUNO)
                                    {
                                        continue;
                                    }

                                    
                                }

                                bool send = true;

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

                                    traceObj.VENDOR = GetVendor?[0].VALUE.ToString();//供應名
                                    traceObj.DATE_CODE = GetDateCode?[0].VALUE;
                                    traceObj.LOT_CODE = GetDateCode?[0].EXVALUE1.ToString();
                                    traceObj.MPN = GetVendor?[0].MPN.ToString();
                                    traceObj.COMPONENT_PART_NUMBER = kp.MPN;
                                    traceObj.CM_COMPONENT_PART_NUMBER = kp.MPN;
                                    traceObj.LOCATION = "";
                                    traceObj.SERIAL_NUMBER_CHILD = "";

                                }
                                else if (kp.STATION.Equals("SUPPLIER-SN"))
                                {
                                    var allpart_smtloading = GetAllpartData(APDB, sn.SN, "D");
                                    traceObj.VENDOR = allpart_smtloading?.Rows[0]["MFR_NAME"].ToString();//供應名
                                    traceObj.MPN = allpart_smtloading?.Rows[0]["MFR_KP_NO"].ToString();//MPN
                                    traceObj.DATE_CODE = allpart_smtloading?.Rows[0]["DATE_CODE"].ToString();
                                    traceObj.LOT_CODE = allpart_smtloading?.Rows[0]["LOT_CODE"].ToString();
                                }
                                else if (disc_Kp.Count == 0)
                                {
                                    var strsql = $@"select * from r_sn_kp where sn in 
                                                        (
                                                        select value from r_sn_kp where r_sn_id ='{kp.R_SN_ID}'
                                                        )
                                                        and kp_name = 'LINK_TR_SN' and valid_flag=1";
                                    var disc_Kp1 = SFCDB.Ado.SqlQuery<R_SN_KP>(strsql).ToList();
                                    for (int j = 0; j < disc_Kp1.Count; j++)
                                    {
                                        var GetVendor = SFCDB.Ado.SqlQuery<R_SN_KP>($@"select*From r_sn_kp where sn='{disc_Kp1[0].VALUE}' and KP_NAME='TR_SN' and SCANTYPE='VENDOR' and valid_flag=1");
                                        var GetDateCode = SFCDB.Ado.SqlQuery<R_SN_KP>($@"select*From r_sn_kp where sn='{disc_Kp1[0].VALUE}' and KP_NAME='TR_SN'  and SCANTYPE='DATE_CODE' and valid_flag=1");
                                        var GetPno = SFCDB.Ado.SqlQuery < R_SN_KP > ($@"select * from r_sn_kp where sn='{kp.SN}' and  value = '{disc_Kp1[0].SN}' and kp_name = 'AutoKP_SAP_HB' and valid_flag=1");
                                        var nt = traceObj.Clone();
                                        nt.ID = MesDbBase.GetNewID<R_DISC_TRACE>(SFCDB, _bustr);
                                        nt.SERIAL_NUMBER_CHILD = disc_Kp1[j].SN;
                                        nt.DATE_CODE = GetDateCode[0]?.VALUE;
                                        nt.LOT_CODE = GetDateCode[0]?.EXVALUE1.ToString();
                                        nt.MPN = GetVendor[0]?.MPN.ToString();
                                        //nt.COMPONENT_PART_NUMBER = GetPno[0]?.PARTNO;
                                        //nt.CM_COMPONENT_PART_NUMBER = GetPno[0]?.PARTNO;
                                        nt.COMPONENT_PART_NUMBER = traceObj.COMPONENT_PART_NUMBER;
                                        nt.CM_COMPONENT_PART_NUMBER = traceObj.CM_COMPONENT_PART_NUMBER;
                                        nt.LOCATION = "";
                                        nt.VENDOR = GetVendor[0].VALUE;
                                        list_trace.Add(nt);
                                    }
                                    send = false;

                                }
                                if (send)
                                {
                                    list_trace.Add(traceObj);
                                }


                            }
                            break;
                    }

                    SFCDB.Insertable<R_DISC_TRACE>(list_trace).ExecuteCommand();
                }
                #endregion

                

                #region R_DISC_MFG
                R_DISC_HEAD mfgHead = headList.Find(r => r.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description);
                if (mfgHead != null)
                {
                    var woObj = SFCDB.Queryable<R_SAP_AS_BOM>().Where(r => r.WO == sn.WORKORDERNO).OrderBy(r=>r.CREATETIME,OrderByType.Desc).ToList().FirstOrDefault();
                    string rev = woObj != null ? woObj.PARENTREV : "";
                    if(string.IsNullOrEmpty(rev))
                    {
                        rev = agile != null ? agile.REV : "";
                    }
                    //var list_mfg = new List<R_DISC_MFG>();
                    R_DISC_MFG mfgObj = null;
                    mfgObj = new R_DISC_MFG();
                    mfgObj.ID = MesDbBase.GetNewID<R_DISC_MFG>(SFCDB, _bustr);
                    mfgObj.SUPPLIER = CONST_SUPPLIER;
                    mfgObj.SUPPLIER_SITE = _plant;
                    #region 以下欄位為要抓取的數據，
                    mfgObj.PART_NUMBER = agile != null ? agile.CUSTPARTNO : sn.SKUNO;
                    mfgObj.SERIAL_NUMBER = sn.SN;
                    mfgObj.PART_NUMBER_REVISION = rev;//03.03 modify by G6004623  // agile != null ? agile.REV : "";
                    mfgObj.SHOP_FLOOR_ORDER_NUMBER = sn.WORKORDERNO;
                    mfgObj.ROUTING_STEP_NUMBER = sn.STATION_NAME; //工站
                    mfgObj.WORK_STATION = sn.STATION_NAME;//工站
                    mfgObj.WORK_STATION_DESCRIPTION = sn.STATION_NAME;//工站
                    mfgObj.START_DATE_TIME = sn.EDIT_TIME;
                    mfgObj.END_DATE_TIME = sn.EDIT_TIME;
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
                    SqlSugarClient LogDB = SFCDB;// OleExec.GetSqlSugarClient(_mesdbstr, false);
                    log.ID = MesDbBase.GetNewID<R_MES_LOG>(LogDB, _bustr);
                    log.PROGRAM_NAME = "JuniperDiscData";
                    log.CLASS_NAME = "MES_DCN.Juniper.DISCQMSDataNew";
                    log.FUNCTION_NAME = "CollectingPassStationData";
                    log.LOG_MESSAGE = "Running CollectingPassStationData Fail";
                    log.LOG_SQL = ex.Message.Length > 1000 ? ex.Message.Substring(0, 950) : ex.Message;
                    log.DATA1 = sn.SN;
                    log.DATA2 = sn.STATION_NAME;
                    log.EDIT_EMP = "Interface";
                    log.EDIT_TIME = LogDB.GetDate();
                    LogDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                }
                catch (Exception)
                {
                }
            }
        }
        public void CollectingRepairData(SqlSugarClient SFCDB, SqlSugarClient APDB, DateTime loadDate, string collectedDate, List<R_DISC_HEAD> headList)
        {
            try
            {
                DateTime sysdate = SFCDB.GetDate();
                #region R_DISC_TEST
                R_DISC_HEAD testHead = headList.Find(r => r.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description);
                if (testHead != null)
                {
                    var list_test = new List<R_DISC_TEST>();
                    R_DISC_TEST testObj = null;

                    var test_List = SFCDB.Ado.SqlQuery<R_TEST_JUNIPER>($@"
                                            SELECT distinct r.PART_NUMBER,
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
                                        fROM R_TEST_JUNIPER r where 
                                            r.TATIME  between to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss') 
                                            and r.EVENTNAME not in('ICT','AOI1','5DX','5DX1','AOI2','5DX2')
                                          and r.FILE_NAME='Y' and r.STATUS in('Pass','PASS','FAIL','Fail','V1','V2','ABORT', 'INCOMPLETE','Incomplete','Abort') and r.part_number is not null
                                         and r.TEST_STEP not in('DBG')
                                        and r.CM_ODM_PARTNUMBER is not null 
                                        and r.CM_ODM_PARTNUMBER not like '%BUILT%'
                                        and r.sysserialno not in (select distinct sn from SFCRUNTIME.R_JUNIPER_SILVER_WIP WHERE STATE_FLAG = '1')
                                        and not exists(select*From r_disc_test t where t.SERIAL_NUMBER = r.SYSSERIALNO
                                                                and  t.PART_NUMBER  =r.PART_NUMBER
                                                                and  t.UNIQUE_TEST_ID = r.UNIQUE_TEST_ID                       
                                                                and  t.TEST_STEP = r.TEST_STEP
                                                                and  t.TEST_CYCLE_TEST_LOOP = r.TEST_CYCLE_TEST_LOOP
                                                                and  t.TEST_RESULT = r.TEST_RESULT
                                                                and  t.TEST_STATION_NUMBER = r.TEST_STATION_NUMBER
                                                                and  t.TEST_STATION_NAME = r.TEST_STATION_NAME )
                                        union all
                                                SELECT  distinct r.PART_NUMBER,
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
                                        fROM R_TEST_JUNIPER r where 
                                           r.TATIME  between to_date('{collectedDate} 00:00:00', 'yyyy/mm/dd hh24:mi:ss') and to_date('{collectedDate} 23:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                         and r.EVENTNAME  in('ICT','AOI1','5DX','5DX1','AOI2','5DX2')   and r.STATUS in('Pass','PASS','FAIL','Fail','ABORT', 'INCOMPLETE','Incomplete','Abort')
                                            and r.part_number is not null
                                 and r.CM_ODM_PARTNUMBER is not null
                                 and r.TEST_STEP not in('DBG')
                                 AND not exists(select*From r_disc_test t where t.SERIAL_NUMBER = r.SYSSERIALNO
                                                                and  t.TEST_STATION_NAME = r.TEST_STATION_NAME AND t.TEST_RESULT='PASS' ) 
                                 and not exists(select*From r_disc_test t where t.SERIAL_NUMBER = r.SYSSERIALNO
                                                                and  t.PART_NUMBER  =r.PART_NUMBER
                                                                and  t.UNIQUE_TEST_ID = r.UNIQUE_TEST_ID                       
                                                                and  t.TEST_STEP = r.TEST_STEP
                                                                and  t.TEST_CYCLE_TEST_LOOP = r.TEST_CYCLE_TEST_LOOP
                                                                and  t.TEST_RESULT = r.TEST_RESULT
                                                                and  t.TEST_STATION_NUMBER = r.TEST_STATION_NUMBER
                                                                and  t.TEST_STATION_NAME = r.TEST_STATION_NAME )");
                    foreach (var test in test_List)
                    {
                        //711 PN only send 711-052892(SFB2)  711-044466(SFB)
                        bool is711 = SFCDB.Queryable<R_SN>().Any(r => r.SN == test.SYSSERIALNO && r.VALID_FLAG == "1" && SqlFunc.StartsWith(r.SKUNO, "711"));
                        if(is711)
                        {
                            bool bSendPN = SFCDB.Queryable<R_SN>().Any(r => r.SN == test.SYSSERIALNO && r.VALID_FLAG == "1"
                            && SqlFunc.Subqueryable<R_F_CONTROL>().Where(f => f.VALUE == r.SKUNO && f.FUNCTIONNAME == "DISC_DATA"
                            && f.CATEGORY == "DISC_TEST_PN" && f.CONTROLFLAG == "Y" && f.FUNCTIONTYPE == "NOSYSTEM").Any());
                            if(!bSendPN)
                            {
                                continue;
                            }
                        }
                        var Test_Phase = SFCDB.Ado.SqlQuery<R_WO_TYPE>($@"select a.* From r_wo_type a, r_sn b where   b.sn='{test.SYSSERIALNO}' and b.workorderno like a.PREFIX||'%' and b.VALID_FLAG=1");
                        string phase = "Production", v_PNO = test.PART_NUMBER;

                        if (Test_Phase.Count > 0)
                        {
                            switch (Test_Phase[0].PRODUCT_TYPE)
                            {
                                case "GA":
                                    phase = "Production";
                                    break;
                                case "NPI":
                                    phase = "NPI";
                                    break;
                                default:
                                    phase = "Production";
                                    break;
                            }
                        }

                        //Validate Part Number
                        if ((v_PNO.StartsWith("MX") || v_PNO.StartsWith("EX")) && v_PNO.Length <= 6)
                        {
                            var objSN = SFCDB.Queryable<R_SN>().Where(S => S.SN == test.SYSSERIALNO && S.VALID_FLAG != "0").OrderBy(S => S.EDIT_TIME, OrderByType.Desc).First();
                            if (objSN != null)
                            {
                                v_PNO = objSN.SKUNO.ToString();
                            }
                        }

                        if (SFCDB.Queryable<O_AGILE_ATTR>().Where(o => o.ITEM_NUMBER == v_PNO).Any())
                        {
                            testObj = new R_DISC_TEST();
                            testObj.ID = MesDbBase.GetNewID<R_DISC_TEST>(SFCDB, _bustr);
                            testObj.SUPPLIER = CONST_SUPPLIER;
                            testObj.SUPPLIER_SITE = _plant;
                            #region 以下欄位為要抓取的數據，待編寫邏輯
                            testObj.PART_NUMBER = v_PNO;
                            testObj.CM_ODM_PARTNUMBER = test.CM_ODM_PARTNUMBER;
                            testObj.SERIAL_NUMBER = test.SERIAL_NUMBER;
                            //testObj.SERIAL_NUMBER = test.SYSSERIALNO;
                            //testObj.PHASE =  Test_Phase[0].PRODUCT_TYPE == "GA" ? "Production" : Test_Phase[0].PRODUCT_TYPE == "NPI" ? "NPI" : test.PHASE;
                            testObj.PHASE = phase;
                            testObj.PART_NUMBER_REVISION = test.PART_NUMBER_REVISION;
                            testObj.UNIQUE_TEST_ID = test.UNIQUE_TEST_ID;
                            testObj.TEST_START_TIMESTAMP = string.IsNullOrEmpty(test.TEST_START_TIMESTAMP) ? test.TEST_START_TIMESTAMP : ConvertTimeSpanToString(test.TEST_START_TIMESTAMP);
                            testObj.TEST_STEP = test.TEST_STEP == "" ? test.TEST_STATION_NAME : test.TEST_STEP;
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
                            testObj.MFG_TEST_LOG = test.MFG_TEST_LOG;//disc test data need this cloumn 2022-01-13
                            list_test.Add(testObj);
                            
                        }
                        
                    }
                    SFCDB.Insertable<R_DISC_TEST>(list_test).ExecuteCommand();
                }

                #endregion
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
                                   rrm.workorderno,
                                   rrm.skuno,
                                   rrm.fail_station,
                                   rrm.fail_device,
                                   to_char(rrm.fail_time,'yyyy/mm/dd hh24:mi:ss') fail_time,
                                   rfa.fail_code,
                                   rfa.fail_location,
                                   rrf.fail_category,
                                   rrf.fail_process,
                                   rrf.description ,
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
								   case when rfa.solution='NULL' then '' else rfa.solution end solution,
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
                        && SqlFunc.ToUpper(r.STATUS) == "FAIL" && r.EVENTNAME == fail_station && r.UNIQUE_TEST_ID == RepairMain_ex.Rows[0]["VALUE"].ToString())
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
                                repairObj.TEST_START_TIMESTAMP = repairObj.TEST_START_TIMESTAMP.Replace('/', '-');//強制轉換，免得TE又上傳格式錯誤又搞得雞飛狗跳的 2021-12-10
                                repairObj.TEST_STEP = test_fail.TEST_STEP;
                                repairObj.TEST_CYCLE_TEST_LOOP = test_fail.TEST_CYCLE_TEST_LOOP;
                                repairObj.MFG_TEST_LOG = test_fail.MFG_TEST_LOG;//disc repair data need this cloumn 2022-01-24
                            }
                            repairObj.DEFECT_CLASSIFICATION = solution;//維修人員輸入
                            repairObj.LOCATION = fail_location;
                            repairObj.COMPONENT_PART_NUMBER = kp_no;
                            repairObj.CM_ODM_COMPONENT_ID = kp_no;
                            repairObj.DEFECT_TYPE = section_id;//維修人員輸入
                            repairObj.DEFECT_DESCRIPTION = section_id == "NDF" ? section_id : REPAIR_COMMENTS; // repairObj.DEFECT_DESCRIPTION = fail_description; Changed to REPAIR_COMMENTS due to Chris Langley Requirement 01/31/22
                            repairObj.REPAIR_STATION = fail_station;
                            repairObj.REPAIR_STATION_NAME = fail_station;
                            repairObj.REPAIR_STATUS = closed_flag;//0 待維修,1 維修結束，
                            repairObj.REPAIR_COMMENTS = fail_description;//維修人員 輸入 備註 repairObj.REPAIR_COMMENTS = REPAIR_COMMENTS;  Changed to fail_description due to Chris Langley Requirement 01/31/22

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
                                log.CLASS_NAME = "MES_DCN.Juniper.DISCQMSDataNew";
                                log.FUNCTION_NAME = "CollectingRepairData";
                                log.LOG_MESSAGE = "Running CollectingRepairData Fail,Insert R_DISC_REPAIR Error";
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
                    if (defectHead != null || SFCDB.Queryable<C_ERROR_CODE>().Where(err => err.ERROR_CODE == REASON_CODE && err.DISC_DEFFECT == "YES").Any())
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
                            defectObj.DEFECT_DESCRIPTION = "Defect Component";// action_desc;
                            defectObj.VENDOR = mfr_name;
                            defectObj.MPN = mpn;
                            defectObj.DATE_CODE = date_code;
                            defectObj.LOT_CODE = lot_code;
                            defectObj.SERIAL_NUMBER_CHILD = keypart_sn== sn ? "": keypart_sn;
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
                            log.CLASS_NAME = "MES_DCN.Juniper.DISCQMSDataNew";
                            log.FUNCTION_NAME = "CollectingRepairData";
                            log.LOG_MESSAGE = "Running CollectingRepairData Fail,Insert R_DISC_DEFECT Error";
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
            catch (Exception ex)
            {
                throw new Exception($@"Running CollectingRepairData Fail;Fail Msg:{ex.Message}");
            }
        }
        public void UpdateCollectFlag(SqlSugarClient SFCDB, List<R_DISC_HEAD> headList) 
        {
            headList.ForEach(r => {
                r.COLLECT_FLAG = "1";
            });
            SFCDB.Updateable<R_DISC_HEAD>(headList).ExecuteCommand();
        }
        public void ConvertCSVAction(DiscDataBase discObj)
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
                return;
            }

            discObj.UpdateHeadConvertFlag();           
        }
        public void UploadToSftp(List<DiscDataBase> listDiscBase)
        {
            try
            {
                SFTPHelper sftpHelp = new SFTPHelper(CONST_SFTPHost, CONST_SFTPPort, CONST_SFTPLogin, CONST_SFTPPassword, _keypath, KeyType.RSA_PRIVATE);
                foreach (var item in listDiscBase)
                {
                    MesLog.Info($@"Begin {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} CompressFile ...");
                    item.CompressFile();
                    MesLog.Info($@"End {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} CompressFile ...");

                    MesLog.Info($@"Begin {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} sftpHelp.Put ...");
                    sftpHelp.Put($@"{item.LocalFilePath}{item.GZFileName}", $@"{_remotepath}{item.GZFileName}");
                    MesLog.Info($@"End {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} sftpHelp.Put ...");

                    MesLog.Info($@"Begin {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} UpdateSendFlag ...");
                    item.UpdateSendFlag();
                    MesLog.Info($@"End {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} UpdateSendFlag ...");

                    MesLog.Info($@"Begin {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} UpdateHeadSendFlag ...");
                    item.UpdateHeadSendFlag();
                    MesLog.Info($@"End {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} UpdateHeadSendFlag ...");

                    MesLog.Info($@"Begin {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} DoBackups ...");
                    item.DoBackups();
                    MesLog.Info($@"End {item.DiscHeadObj.DISC_TYPE},{item.DiscHeadObj.ID} DoBackups ...");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Running UploadToSftp Fail;Fail Msg:{ex.Message}");
            }
            
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
    
    
        public void RebulidGZFile(DateTime fromDate,DateTime toDate, string discType)
        {
            SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
            int days = (toDate - fromDate).Days;
            string discKey = "";
            if (days >= 0)
            {  
                for (int i = 0; i <= days; i++)
                {
                    discKey = fromDate.AddDays(i).ToString("yyyy-MM-dd");
                    R_DISC_HEAD headObj = SFCDB.Queryable<R_DISC_HEAD>().Where(r => r.DISC_TYPE == discType && r.DISC_KEY == discKey && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                    if(headObj==null)
                    {
                        continue;
                    }
                    DiscDataBase discData = null;
                    if (headObj.DISC_TYPE == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new TraceDiscData();
                        if (SFCDB.Queryable<R_DISC_TRACE>().Any(r => r.HEAD_ID == headObj.ID))
                        {
                            discData.SendDataList = SFCDB.Queryable<R_DISC_TRACE>().Where(r => r.HEAD_ID == headObj.ID).ToList();
                        }
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new TestDiscData();
                        if (SFCDB.Queryable<R_DISC_TEST>().Any(r => r.HEAD_ID == headObj.ID))
                        {
                            discData.SendDataList = SFCDB.Queryable<R_DISC_TEST>().Where(r => r.HEAD_ID == headObj.ID).ToList();
                        }
                       
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new DefectDiscData();
                        if (SFCDB.Queryable<R_DISC_DEFECT>().Any(r => r.HEAD_ID == headObj.ID))
                        {
                            discData.SendDataList = SFCDB.Queryable<R_DISC_DEFECT>().Where(r => r.HEAD_ID == headObj.ID).ToList();
                        }
                        
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new RepairDiscData();
                        if (SFCDB.Queryable<R_DISC_REPAIR>().Any(r => r.HEAD_ID == headObj.ID))
                        {
                            discData.SendDataList = SFCDB.Queryable<R_DISC_REPAIR>().Where(r => r.HEAD_ID == headObj.ID).ToList();
                        }
                        
                    }
                    else if (headObj.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                    {
                        discData = new MFGDiscData();
                        if (SFCDB.Queryable<R_DISC_MFG>().Any(r => r.HEAD_ID == headObj.ID))
                        {
                            discData.SendDataList = SFCDB.Queryable<R_DISC_MFG>().Where(r => r.HEAD_ID == headObj.ID).ToList();
                        }
                        
                    }
                    else
                    {
                        continue;
                    } 
                    if(discData.SendDataList==null)
                    {
                        continue;
                    }
                    discData.IsSend = true;
                    discData.SFCDB = OleExec.GetSqlSugarClient(_mesdbstr, false);
                    discData.LocalFilePath = _filepath;
                    discData.BackupPath = _filebackuppath;
                    discData.DiscHeadObj = headObj;
                    discData.FileFullName = $@"{_filepath}{headObj.DISC_FILE}";
                    discData.BU = _bustr;

                    if (discData.DiscHeadObj.DISC_TYPE == DiscFileType.TRACE.Ext<EnumValueAttribute>().Description)
                    {
                        discData.SaveCSVFile<R_DISC_TRACE>();
                    }
                    else if (discData.DiscHeadObj.DISC_TYPE == DiscFileType.TEST.Ext<EnumValueAttribute>().Description)
                    {
                        discData.SaveCSVFile<R_DISC_TEST>();
                    }
                    else if (discData.DiscHeadObj.DISC_TYPE == DiscFileType.DEFECT.Ext<EnumValueAttribute>().Description)
                    {
                        discData.SaveCSVFile<R_DISC_DEFECT>();
                    }
                    else if (discData.DiscHeadObj.DISC_TYPE == DiscFileType.REPAIR.Ext<EnumValueAttribute>().Description)
                    {
                        discData.SaveCSVFile<R_DISC_REPAIR>();
                    }
                    else if (discData.DiscHeadObj.DISC_TYPE == DiscFileType.MFG.Ext<EnumValueAttribute>().Description)
                    {
                        discData.SaveCSVFile<R_DISC_MFG>();
                    }
                    else
                    {
                        return;
                    }

                    discData.CompressFile();
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }

    public class DiscDataBase
    {
        public string BackupPath { get; set; }
        public string LocalFilePath { get; set; }
        public bool IsSend { get; set; } = true;
        public SqlSugarClient SFCDB { get; set; }
        public R_DISC_HEAD DiscHeadObj { get; set; }
        public object SendDataList { get; set; }
        public string FileFullName { get; set; }
        public string GZFileName { get; set; }
        //public double SendFlag { get { return IsSend ? 1 : 2; } }
        public string BU { get; set; }
        public virtual void GetSendDataList()
        {
            throw new NotImplementedException();
        }
        public void SaveCSVFile<T>()
        {
            try
            {
                List<T> list = (List<T>)SendDataList;
                List<string> no_sent_column = new List<string>() { "ID", "CREATE_TIME", "SENT_FLAG", "SENT_TIME", "HEAD_ID", "VALID_FLAG" ,"MFG_TEST_LOG" };                
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
            if (IsSend)
            {
                using (FileStream originalFileStream = new FileStream($"{LocalFilePath}{DiscHeadObj.DISC_FILE}", FileMode.Open))
                {
                    using (FileStream compressedFileStream = File.Create($"{LocalFilePath}{DiscHeadObj.DISC_FILE}.gz"))
                    {
                        using (GZipStream compressioonStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressioonStream);
                            GZFileName = $"{DiscHeadObj.DISC_FILE}.gz";
                        }
                    }
                }
                File.Delete($"{LocalFilePath}{DiscHeadObj.DISC_FILE}");
            }
        }
        public virtual void UpdateSendFlag()
        {
            throw new NotImplementedException();
        }
        public void UpdateHeadConvertFlag()
        {
            DiscHeadObj.CONVERT_FLAG = "1";
            DiscHeadObj.EDIT_TIME = SFCDB.GetDate();
            SFCDB.Updateable<R_DISC_HEAD>(DiscHeadObj).ExecuteCommand();
        }
        public void UpdateHeadSendFlag()
        {
            if (IsSend)
            {
                DiscHeadObj.SEND_FLAG = "1";
                DiscHeadObj.EDIT_TIME = SFCDB.GetDate();
                SFCDB.Updateable<R_DISC_HEAD>(DiscHeadObj).ExecuteCommand();
            }
        }
        public void DoBackups()
        {
            try
            {
                if (IsSend)
                {
                    if (!Directory.Exists(BackupPath))
                    {
                        Directory.CreateDirectory(BackupPath);
                    }
                    if (File.Exists($@"{BackupPath}{GZFileName}"))
                    {
                        File.Move($@"{BackupPath}{GZFileName}", $@"{BackupPath}backup_{DateTime.Now.ToString("yyyyMMddhhmmss")}_{GZFileName}");
                        File.Move($@"{LocalFilePath}{GZFileName}", $@"{BackupPath}{GZFileName}");
                        Thread.Sleep(200);
                    }
                    else
                    {
                        File.Move($@"{LocalFilePath}{GZFileName}", $@"{BackupPath}{GZFileName}");
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    string pathCSV = $@"{LocalFilePath}\CSV";
                    if (!Directory.Exists(pathCSV))
                    {
                        Directory.CreateDirectory(pathCSV);
                    }
                    pathCSV = $@"{pathCSV}\";
                    if (File.Exists($@"{pathCSV}{DiscHeadObj.DISC_FILE}"))
                    {
                        File.Move($@"{pathCSV}{DiscHeadObj.DISC_FILE}", $@"{pathCSV}backup_{DateTime.Now.ToString("yyyyMMddhhmmss")}_{DiscHeadObj.DISC_FILE}");
                        File.Move($@"{LocalFilePath}{DiscHeadObj.DISC_FILE}", $@"{pathCSV}{DiscHeadObj.DISC_FILE}");
                        Thread.Sleep(200);
                    }
                    else
                    {
                        File.Move($@"{LocalFilePath}{DiscHeadObj.DISC_FILE}", $@"{pathCSV}{DiscHeadObj.DISC_FILE}");
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ValueIsNull<T>(List<string> fieldList, T obj)
        {
            bool bResult = false;
            string nullFieldName = "";
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();
            PropertyInfo[] checkField = propertyInfos.Where(c => fieldList.Contains(c.Name.ToUpper())).ToArray();

            PropertyInfo[] snProperty = propertyInfos.Where(p => p.Name.ToUpper() == "SERIAL_NUMBER").ToArray();
            string sn = snProperty.First().GetValue(obj).ToString();
            PropertyInfo[] idProperty = propertyInfos.Where(p => p.Name.ToUpper() == "ID").ToArray();
            string id= idProperty.First().GetValue(obj).ToString();

            foreach (var property in checkField)
            {
                if (property.GetValue(obj) == null)
                {
                    bResult = true;
                    nullFieldName = property.Name;
                    break;
                }
            }
            if (bResult)
            {
                ValueErrorLog("ValueIsNull", DiscHeadObj.DISC_TYPE, nullFieldName, sn, id, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
            }
            return bResult;
        }
        private void SaveCSVFile<T>(Dictionary<string, string> dicReplaceColumn, List<T> valueList)
        {
            try
            {
                List<string> noSentColumn = new List<string>() { "ID", "CREATE_TIME", "SENT_FLAG", "SENT_TIME", "HEAD_ID", "VALID_FLAG" };
                Type type = typeof(T);
                PropertyInfo[] columnList = type.GetProperties();
                columnList = columnList.Where(c => !noSentColumn.Contains(c.Name)).ToArray();
                using (var sw = new StreamWriter(new FileStream(FileFullName, FileMode.Create), Encoding.GetEncoding("UTF-8")))
                {
                    StringBuilder column = new StringBuilder();
                    StringBuilder value = new StringBuilder();

                    foreach (var c in columnList)
                    {
                        var name = dicReplaceColumn.Where(d => d.Key == c.Name);
                        if (name.Count() > 0)
                        {
                            column.Append(name.FirstOrDefault().Value);
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
                    foreach (var l in valueList)
                    {
                        value.Remove(0, value.Length);
                        foreach (var v in columnList)
                        {
                            string temp = "";
                            if (v.GetValue(l) != null)
                            {
                                var columnValue = v.GetValue(l);
                                if (v.Name == "LOAD_DATE")
                                {
                                    temp = ((DateTime)columnValue).ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    temp = string.IsNullOrEmpty(columnValue.ToString()) ? "" : columnValue.ToString();
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

        public void ValueErrorLog(string functionName,string msg, string logSql,string data1,string data2,string data3,string data4)
        {
            try
            {
                R_MES_LOG log = new R_MES_LOG();
                log.ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB, BU);
                log.PROGRAM_NAME = "JuniperDiscData";
                log.CLASS_NAME = "MES_DCN.Juniper.DiscDataBase";
                log.FUNCTION_NAME = functionName;
                log.LOG_MESSAGE = msg;
                log.LOG_SQL = logSql;
                log.DATA1 = data1;
                log.DATA2 = data2;
                log.DATA3 = data3;
                log.DATA4 = data4;
                log.EDIT_EMP = "Interface";
                log.EDIT_TIME = SFCDB.GetDate();
                SFCDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
            }
            catch (Exception ex)
            {
                try
                {
                    R_MES_LOG log = new R_MES_LOG();
                    log.ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB, BU);
                    log.PROGRAM_NAME = "JuniperDiscData";
                    log.CLASS_NAME = "MES_DCN.Juniper.DiscDataBase";
                    log.FUNCTION_NAME = "ValueErrorLog";
                    log.LOG_MESSAGE = $"Running ValueErrorLog Fail,{msg}";
                    log.LOG_SQL = ex.Message;
                    log.DATA1 = data1;
                    log.DATA2 = data2;
                    log.EDIT_EMP = "Interface";
                    log.EDIT_TIME = SFCDB.GetDate();
                    SFCDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
                }
                catch (Exception)
                {

                }
            }
        }
    }

    public class TraceDiscData : DiscDataBase
    {

        public override void GetSendDataList()
        {
            //this.SendDataList = SFCDB.Queryable<R_DISC_TRACE>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();            
            //var list = SFCDB.Queryable<R_DISC_TRACE>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();            
            var list = SFCDB.Queryable<R_DISC_TRACE>().Where(r => r.HEAD_ID == DiscHeadObj.ID).ToList();
            list.RemoveAll((traceObj) =>
            {
                return ErrorData(traceObj);
            });
            this.SendDataList = list;
        }
        public override void UpdateSendFlag()
        {
            #region more time
            //List<R_DISC_TRACE> list = (List<R_DISC_TRACE>)SendDataList;
            //if (list != null && list.Count > 0)
            //{
            //    DateTime sendTime = SFCDB.GetDate();
            //    list.ForEach(l =>
            //    {
            //        l.SENT_FLAG = SendFlag;
            //        l.SENT_TIME = sendTime;
            //    });
            //    SFCDB.Updateable<R_DISC_TRACE>(list).ExecuteCommand();
            //}
            #endregion

            if(DiscHeadObj!=null)
            {
                SFCDB.Ado.ExecuteCommand($@" update r_disc_trace set sent_flag='2',sent_time=sysdate where head_id='{DiscHeadObj.ID}' ");
            }            
        }
        private bool ErrorData(R_DISC_TRACE traceObj)
        {
            var notNullFiled = new List<string>() {
                "PART_NUMBER",
                "CM_ODM_PARTNUMBER",
                "SERIAL_NUMBER",
                "SHOP_FLOOR_ORDER_NUMBER",
                "COMPONENT_PART_NUMBER",
                "CM_COMPONENT_PART_NUMBER",
                "ASSEMBLY_STATION",
                "ASSEMBLY_STATION_DESCRIPTION",
                "VENDOR",
                "MPN",
                "DATE_CODE"};           
            return ValueIsNull(notNullFiled, traceObj);
        }
    }
    public class TestDiscData : DiscDataBase
    {
        public override void GetSendDataList()
        {
            //this.SendDataList = SFCDB.Queryable<R_DISC_TEST>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();            
            //var list = SFCDB.Queryable<R_DISC_TEST>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            var list = SFCDB.Queryable<R_DISC_TEST>().Where(r => r.HEAD_ID == DiscHeadObj.ID ).ToList();
            list.RemoveAll((testObj) =>
            {
                return ErrorData(testObj);
            });
            this.SendDataList = list;
        }
        public override void UpdateSendFlag()
        {
            #region more time
            //List<R_DISC_TEST> list = (List<R_DISC_TEST>)SendDataList;
            //if (list != null && list.Count > 0)
            //{
            //    DateTime sendTime = SFCDB.GetDate();
            //    list.ForEach(l =>
            //    {
            //        l.SENT_FLAG = SendFlag;
            //        l.SENT_TIME = sendTime;
            //    });
            //    SFCDB.Updateable<R_DISC_TEST>(list).ExecuteCommand();
            //}
            #endregion

            if (DiscHeadObj != null)
            {
                SFCDB.Ado.ExecuteCommand($@" update r_disc_test set sent_flag='2',sent_time=sysdate where head_id='{DiscHeadObj.ID}' ");
            }
        }
        private bool ErrorData(R_DISC_TEST testObj)
        {
            List<string> notNullFiled = new List<string>()
            {
                "PART_NUMBER",
                "CM_ODM_PARTNUMBER",
                "SERIAL_NUMBER",
                "PHASE",
                "UNIQUE_TEST_ID",
                "TEST_START_TIMESTAMP",
                "TEST_CYCLE_TEST_LOOP",
                "CAPTURE_TIME",
                "TEST_RESULT"
            };            
            if(ValueIsNull(notNullFiled, testObj))
            {
                return true;
            }
            List<string> phase = new List<string>() { "NPI", "Production" };
            if(!phase.Contains(testObj.PHASE))
            {
                ValueErrorLog("PHASE ERROR", DiscHeadObj.DISC_TYPE, 
                    $"PHASE can only be NPI or Production;[{testObj.PHASE}]", 
                    testObj.SERIAL_NUMBER, testObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }
            DateTime testStartTime;
            if(!DateTime.TryParse(testObj.TEST_START_TIMESTAMP, out testStartTime))
            {
                ValueErrorLog("TEST_START_TIMESTAMP ERROR", DiscHeadObj.DISC_TYPE, 
                    $"TEST_START_TIMESTAMP can only be date time;[{testObj.TEST_START_TIMESTAMP}] ", 
                    testObj.SERIAL_NUMBER, testObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }

            int testLoop;
            if (!Int32.TryParse(testObj.TEST_CYCLE_TEST_LOOP, out testLoop))
            {
                ValueErrorLog("TEST_CYCLE_TEST_LOOP ERROR", DiscHeadObj.DISC_TYPE,
                    $"TEST_CYCLE_TEST_LOOP can only be a number;[{testObj.TEST_CYCLE_TEST_LOOP}] ",
                    testObj.SERIAL_NUMBER, testObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }
            if (DateTime.Compare((DateTime)testObj.CAPTURE_TIME, testStartTime)<0)
            {
                ValueErrorLog("CAPTURE_TIME ERROR", DiscHeadObj.DISC_TYPE, 
                    $"CAPTURE_TIME[{((DateTime)testObj.CAPTURE_TIME).ToString("yyyy/MM/dd HH:mm:ss")}] is more than TEST_START_TIMESTAMP [{testStartTime.ToString("yyyy/MM/dd HH:mm:ss")}]", 
                    testObj.SERIAL_NUMBER, testObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }
            //if(!testObj.TEST_RESULT.ToUpper().Equals("PASS")&&!testObj.TEST_RESULT.ToUpper().Equals("FAIL"))
            //{
            //    ValueErrorLog("TEST_RESULT ERROR", DiscHeadObj.DISC_TYPE, 
            //        $"TEST_RESULT can only be PASS or FAIL;[{testObj.TEST_RESULT}]",
            //        testObj.SERIAL_NUMBER, testObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
            //    return true;
            //}
            if (!testObj.TEST_RESULT.ToUpper().Equals("PASS") && !testObj.TEST_RESULT.ToUpper().Equals("FAIL") && !testObj.TEST_RESULT.ToUpper().Equals("INCOMPLETE") && !testObj.TEST_RESULT.ToUpper().Equals("ABORT"))
            {
                ValueErrorLog("TEST_RESULT ERROR", DiscHeadObj.DISC_TYPE,
                    $"TEST_RESULT can only be PASS, FAIL, ABORT OR INCOMPLETE;[{testObj.TEST_RESULT}]",
                    testObj.SERIAL_NUMBER, testObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }
            return false;
        }
    }
    public class DefectDiscData : DiscDataBase
    {
        public override void GetSendDataList()
        {
            //this.SendDataList = SFCDB.Queryable<R_DISC_DEFECT>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();            
            //var list = SFCDB.Queryable<R_DISC_DEFECT>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            var list = SFCDB.Queryable<R_DISC_DEFECT>().Where(r => r.HEAD_ID == DiscHeadObj.ID).ToList();
            list.RemoveAll((defectObj) =>
            {
                return ErrorData(defectObj);
            });
            this.SendDataList = list;
        }
        public override void UpdateSendFlag()
        {
            #region more time
            //List<R_DISC_DEFECT> list = (List<R_DISC_DEFECT>)SendDataList;
            //if (list != null && list.Count > 0)
            //{
            //    DateTime sendTime = SFCDB.GetDate();
            //    list.ForEach(l =>
            //    {
            //        l.SENT_FLAG = SendFlag;
            //        l.SENT_TIME = sendTime;
            //    });
            //    SFCDB.Updateable<R_DISC_DEFECT>(list).ExecuteCommand();
            //}
            #endregion

            if (DiscHeadObj != null)
            {
                SFCDB.Ado.ExecuteCommand($@" update r_disc_defect set sent_flag='2',sent_time=sysdate where head_id='{DiscHeadObj.ID}' ");
            }
        }
        private bool ErrorData(R_DISC_DEFECT defectObj)
        {
            List<string> notNullFiled = new List<string>()
            {
                "PART_NUMBER",
                "CM_ODM_PARTNUMBER",
                "SERIAL_NUMBER",
                "UNIQUE_TEST_ID",
                "TEST_START_TIMESTAMP",
                "TEST_CYCLE_TEST_LOOP",
                "LOCATION",
                "COMPONENT_PART_NUMBER",
                "CM_ODM_COMPONENTID",
                "DEFECT_DESCRIPTION",
                "VENDOR",
                "MPN",
                "DATE_CODE",
                "SHOP_FLOOR_ORDER_NUMBER"               
            };
            if(ValueIsNull(notNullFiled, defectObj))
            {
                return true;
            }
            DateTime testStartTime;
            if (!DateTime.TryParse(defectObj.TEST_START_TIMESTAMP, out testStartTime))
            {
                ValueErrorLog("TEST_START_TIMESTAMP ERROR", DiscHeadObj.DISC_TYPE, 
                    $"TEST_START_TIMESTAMP can only be date time [{defectObj.TEST_START_TIMESTAMP}]",
                    defectObj.SERIAL_NUMBER, defectObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }
            int testLoop;
            if (!Int32.TryParse(defectObj.TEST_CYCLE_TEST_LOOP, out testLoop))
            {
                ValueErrorLog("TEST_CYCLE_TEST_LOOP ERROR", DiscHeadObj.DISC_TYPE, 
                    $"TEST_CYCLE_TEST_LOOP can only be a number [{defectObj.TEST_CYCLE_TEST_LOOP}]", 
                    defectObj.SERIAL_NUMBER, defectObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }

            if(!defectObj.DEFECT_DESCRIPTION.Equals("Defect Component"))
            {
                ValueErrorLog("DEFECT_DESCRIPTION ERROR", DiscHeadObj.DISC_TYPE, 
                    $"DEFECT_DESCRIPTION can only be Defect Component [{defectObj.DEFECT_DESCRIPTION}]",
                    defectObj.SERIAL_NUMBER, defectObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }

            return false;
        }
    }
    public class RepairDiscData : DiscDataBase
    {
        public override void GetSendDataList()
        {
            //this.SendDataList = SFCDB.Queryable<R_DISC_REPAIR>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();           
            //var list = SFCDB.Queryable<R_DISC_REPAIR>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            var list = SFCDB.Queryable<R_DISC_REPAIR>().Where(r => r.HEAD_ID == DiscHeadObj.ID).ToList();
            list.RemoveAll((repairObj) =>
            {
                return ErrorData(repairObj);
            });
            this.SendDataList = list;
        }
        public override void UpdateSendFlag()
        {
            #region more time
            //List<R_DISC_REPAIR> list = (List<R_DISC_REPAIR>)SendDataList;
            //if (list != null && list.Count > 0)
            //{
            //    DateTime sendTime = SFCDB.GetDate();
            //    list.ForEach(l =>
            //    {
            //        l.SENT_FLAG = SendFlag;
            //        l.SENT_TIME = sendTime;
            //    });
            //    SFCDB.Updateable<R_DISC_REPAIR>(list).ExecuteCommand();
            //}
            #endregion

            if (DiscHeadObj != null)
            {
                SFCDB.Ado.ExecuteCommand($@" update r_disc_repair set sent_flag='2',sent_time=sysdate where head_id='{DiscHeadObj.ID}' ");
            }
        }
        private bool ErrorData(R_DISC_REPAIR repairObj)
        {
            List<string> notNullFiled = new List<string>()
            {
                "PART_NUMBER",
                "CM_ODM_PARTNUMBER",
                "SERIAL_NUMBER",
                "UNIQUE_TEST_ID",
                "TEST_START_TIMESTAMP",
                "TEST_CYCLE_TEST_LOOP",
                "DEFECT_TYPE"
            };
            if (ValueIsNull(notNullFiled, repairObj))
            {
                return true;
            }
            DateTime testStartTime;
            if (!DateTime.TryParse(repairObj.TEST_START_TIMESTAMP, out testStartTime))
            {
                ValueErrorLog("TEST_START_TIMESTAMP ERROR", DiscHeadObj.DISC_TYPE, 
                    $"TEST_START_TIMESTAMP can only be date time [{repairObj.TEST_START_TIMESTAMP}] ", 
                    repairObj.SERIAL_NUMBER, repairObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }
            int testLoop;
            if (!Int32.TryParse(repairObj.TEST_CYCLE_TEST_LOOP, out testLoop))
            {
                ValueErrorLog("TEST_CYCLE_TEST_LOOP ERROR", DiscHeadObj.DISC_TYPE,
                    $"TEST_CYCLE_TEST_LOOP can only be a number [{repairObj.TEST_CYCLE_TEST_LOOP}]", 
                    repairObj.SERIAL_NUMBER, repairObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }

            List<string> defectTypeList = new List<string>() { "COMPONENT", "NDF", "PLACEMENT", "SOLDER" };
            if (!defectTypeList.Contains(repairObj.DEFECT_TYPE))
            {
                ValueErrorLog("DEFECT_TYPE ERROR", DiscHeadObj.DISC_TYPE, 
                    $"DEFECT_TYPE can only be Component or NDF or Placement or Solder [{repairObj.DEFECT_TYPE}]", 
                    repairObj.SERIAL_NUMBER, repairObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
                return true;
            }
            //默認為空，只有當"DEFECT_TYPE"為NDF時，"DEFECT_CLASSIFICATION"才能為NTF
            //這個已經在維修那裡做卡關了
            //if (repairObj.DEFECT_CLASSIFICATION.Equals("NTF") && !repairObj.DEFECT_TYPE.Equals("NDF"))
            //{
            //    ValueErrorLog("DEFECT_CLASSIFICATION ERROR", DiscHeadObj.DISC_TYPE, 
            //        $"DEFECT_CLASSIFICATION can only be NTF when DEFECT_TYPE is NDF [{repairObj.DEFECT_CLASSIFICATION},{repairObj.DEFECT_TYPE}]", 
            //        repairObj.SERIAL_NUMBER, repairObj.ID, DiscHeadObj.ID, DiscHeadObj.DISC_KEY);
            //    return true;
            //}
            return false;
        }
    }
    public class MFGDiscData : DiscDataBase
    {
        public override void GetSendDataList()
        {
            //this.SendDataList = SFCDB.Queryable<R_DISC_MFG>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();            
            //var list = SFCDB.Queryable<R_DISC_MFG>().Where(r => r.HEAD_ID == DiscHeadObj.ID && r.SENT_FLAG == 0 && r.VALID_FLAG == 1).ToList();
            var list = SFCDB.Queryable<R_DISC_MFG>().Where(r => r.HEAD_ID == DiscHeadObj.ID).ToList();
            list.RemoveAll((mfgObj) =>
            {                
                return ErrorData(mfgObj);
            });
            this.SendDataList = list;
        }
        public override void UpdateSendFlag()
        {
            #region more time
            //List<R_DISC_MFG> list = (List<R_DISC_MFG>)SendDataList;
            //if (list != null && list.Count > 0)
            //{
            //    DateTime sendTime = SFCDB.GetDate();
            //    list.ForEach(l =>
            //    {
            //        l.SENT_FLAG = SendFlag;
            //        l.SENT_TIME = sendTime;
            //    });
            //    SFCDB.Updateable<R_DISC_MFG>(list).ExecuteCommand();
            //}
            #endregion

            if (DiscHeadObj != null)
            {
                SFCDB.Ado.ExecuteCommand($@" update r_disc_mfg set sent_flag='2',sent_time=sysdate where head_id='{DiscHeadObj.ID}' ");
            }
        }
        private bool ErrorData(R_DISC_MFG mfgObj)
        {
            List<string> notNullFiled = new List<string>()
            {
                "PART_NUMBER",
                "SERIAL_NUMBER",
                "SHOP_FLOOR_ORDER_NUMBER",
                "ROUTING_STEP_NUMBER",
                "WORK_STATION",
                "WORK_STATION_DESCRIPTION",
                "START_DATE_TIME",
                "END_DATE_TIME"
            };
            if(ValueIsNull(notNullFiled, mfgObj))
            {
                return true;
            }
            if (DateTime.Compare((DateTime)mfgObj.START_DATE_TIME, (DateTime)mfgObj.END_DATE_TIME) < 0)
            {
                ValueErrorLog("CAPTURE_TIME ERROR", DiscHeadObj.DISC_TYPE, 
                    $"START_DATE_TIME[{((DateTime)mfgObj.START_DATE_TIME).ToString("yyyy/MM/dd HH:mm:ss")}] is more than END_DATE_TIME[{((DateTime)mfgObj.END_DATE_TIME).ToString("yyyy/MM/dd HH:mm:ss")}]",
                    mfgObj.SERIAL_NUMBER, mfgObj.ID,DiscHeadObj.ID,DiscHeadObj.DISC_KEY);
                return true;
            }
            return false;
        }
    }
}
