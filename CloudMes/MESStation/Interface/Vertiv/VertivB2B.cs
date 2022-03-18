using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.Common;
using MESStation.Config.Vertiv;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MESStation.Interface.Vertiv
{ 
    public class VertivB2B
    {
        private string sftpHost = "10.132.48.74", sftpPort = "8022", sftpLogin = "Vertiv", sftpPassword = "65q6D0gt";
        public string localPath, remotePath, buStr, ip;

        public SqlSugarClient SFCDB = null;
        public SFTPHelper sftpHelp = null;
        public VertivB2B()
        {

        }
        public VertivB2B(string localPath, string remotePath, string buStr, SqlSugarClient sugarDB, string ip)
        {
            this.localPath = localPath;
            this.remotePath = remotePath;
            this.buStr = buStr;
            this.ip = ip;
            this.SFCDB = sugarDB;// OleExec.GetSqlSugarClient(dbStr, false);
            this.sftpHelp = new SFTPHelper(sftpHost, sftpPort, sftpLogin, sftpPassword);
            if (!Directory.Exists(this.localPath))
            {
                Directory.CreateDirectory(this.localPath);
            }
        }
        public void Download(string fileType)
        {
            localPath = localPath.EndsWith(@"\") ? localPath : $@"{localPath}\";
            //var downloadFiles = sftpHelp.ListDirectory(remotePath).Where(r => r.LastAccessTime.CompareTo(System.DateTime.Now.AddDays(-30)) >= 0 && r.Name.Contains(fileType)).ToList();            
            var downloadFiles = sftpHelp.ListDirectory(remotePath).Where(r => r.Name.Contains(fileType)).ToList().OrderBy(r => r.LastAccessTime);
            foreach (var file in downloadFiles)
            {
                try
                {
                    if (file.Name.EndsWith(".txt"))
                    {
                        sftpHelp.Get(file.FullName, $@"{localPath}{file.Name}");
                        //sftpHelp.Delete(file.FullName);
                        sftpHelp.Move(file.FullName, $@"{remotePath}/bak/" + file.Name);
                        Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    MesLog.Info($"Download {file.Name} fail.Error message:{ex.Message}");
                }
            }
        }
        public void MoveFile(string fromPath, string toPath, string fileName)
        {
            fromPath = fromPath.EndsWith(@"\") ? fromPath : $@"{fromPath}\";
            toPath = toPath.EndsWith(@"\") ? toPath : $@"{toPath}\";
            if (!Directory.Exists(toPath))
            {
                Directory.CreateDirectory(toPath);
            }
            if (File.Exists($@"{toPath}{fileName}"))
            {
                File.Move($@"{toPath}{fileName}", $@"{toPath}backup_{DateTime.Now.ToString("yyyyMMddhhmmss")}_{fileName}");
            }
            File.Move($@"{fromPath}{fileName}", $@"{toPath}{fileName}");
            Thread.Sleep(200);
        }
        public string DataTableToJson(DataTable dataTable)
        {
            System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
            foreach (DataRow dr in dataTable.Rows)
            {
                Dictionary<string, object> dRow = new Dictionary<string, object>();
                foreach (DataColumn dc in dataTable.Columns)
                {
                    dRow.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                arrayList.Add(dRow);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(arrayList, Newtonsoft.Json.Formatting.Indented);
        }
        public string DataRowToJson(DataRow row, DataColumnCollection columns)
        {
            Dictionary<string, object> dRow = new Dictionary<string, object>();
            foreach (DataColumn dc in columns)
            {
                dRow.Add(dc.ColumnName, row[dc.ColumnName]);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(dRow, Newtonsoft.Json.Formatting.Indented);
        }
        public virtual void Run()
        {
            throw new NotImplementedException();
        }
        public void UploadSftp()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(localPath);
            string tempRemotePath = remotePath.EndsWith("/") ? remotePath : $@"{remotePath}/";
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                if (file.Length > 0)
                {
                    try
                    {
                        sftpHelp.Put(file.FullName, $@"{tempRemotePath}{file.Name}");
                        MoveFile(localPath, $@"{localPath}\Backup\", file.Name);
                    }
                    catch (Exception ex)
                    {
                        MoveFile(localPath, $@"{localPath}\Error\", file.Name);
                        MesLog.Info($@"Upload to sftp [{file.FullName}] fail;Error message:{ex.Message}");
                    }

                }
            }
        }
        public void UploadSftp(string fileFullName, string fileName)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(localPath);
            string tempRemotePath = remotePath.EndsWith("/") ? remotePath : $@"{remotePath}/";
            try
            {
                sftpHelp.Put(fileFullName, $@"{tempRemotePath}{fileName}");
                MoveFile(localPath, $@"{localPath}\Backup\", fileName);
            }
            catch (Exception ex)
            {
                MoveFile(localPath, $@"{localPath}\Error\", fileName);
                MesLog.Info($@"Upload to sftp [{fileFullName}] fail;Error message:{ex.Message}");
                throw ex;
            }
        }
        public void CheckRunning(string name)
        {
            CheckRunning(SFCDB, name,ip,this.buStr);
        }
        public void CheckRunning(SqlSugarClient sfcdb, string name,string lockIp,string bu)
        {
            try
            {
                R_SYNC_LOCK runingObj = sfcdb.Queryable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME.Equals(name)).ToList().FirstOrDefault();
                if (runingObj != null)
                {
                    throw new Exception($@"{name} is running on {runingObj.LOCK_IP},Please try again later");
                }
                runingObj = new R_SYNC_LOCK();
                runingObj.ID = MesDbBase.GetNewID<R_SYNC_LOCK>(sfcdb, bu);
                runingObj.LOCK_NAME = name;
                runingObj.LOCK_KEY = "1";
                runingObj.LOCK_TIME_LONG = 5;
                runingObj.EDIT_EMP = "SYSTEM";
                runingObj.LOCK_TIME = sfcdb.GetDate();
                runingObj.LOCK_IP = lockIp;
                sfcdb.Insertable<R_SYNC_LOCK>(runingObj).ExecuteCommand();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteRunningStatus(string name)
        {
            DeleteRunningStatus(SFCDB, name,ip);
        }
        public void DeleteRunningStatus(SqlSugarClient sfcdb, string name,string lockip)
        {
            sfcdb.Deleteable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME.Equals(name) && t.LOCK_IP.Equals(lockip)).ExecuteCommand();
        }
        public bool AlreadyAnalyes(string type, string fileName)
        {
            bool alreadyAnalyes = false;
            switch (type)
            {
                case "MTIMDiscreteOrder":
                    alreadyAnalyes = SFCDB.Queryable<R_VT_ORDER>().Any(r => r.VALID_FLAG == 1 && r.FILE_NAME == fileName);
                    break;
                case "MTIMForecast":
                    alreadyAnalyes = SFCDB.Queryable<R_VT_FORECAST>().Any(r => r.VALID_FLAG == 1 && r.FILE_NAME == fileName);
                    break;
                default:
                    break;
            }
            if (alreadyAnalyes)
            {
                //已經解析過的文件不再重複解析，轉移到SAME文件夾下
                MoveFile(localPath, $@"{localPath}\Same\", fileName);
            }
            return alreadyAnalyes;
        }
    
        public static List<R_VT_ORDER> GetASNAlertPO(SqlSugarClient sfcdb)
        {
            #region 報警郵件
            //--以下PO超過3天還未生成ASN，請注意
            //  select o.id, o.order_number, o.order_line_id, o.schedule_id, o.created_time, o.edit_time send_commitfile_time, o.file_name from r_vt_order o where o.valid_flag= 1 and o.status= 2 and o.edit_time<sysdate-3

            //--以下ASN超過3天還未傳輸，請注意
            //  select s.id, s.shipment_id, s.order_id, s.dn_no, s.dn_line, s.created_time   from r_vt_shipment s where s.valid_flag=1 and send_flag=0 and s.created_time<sysdate-3

            //--以下PO超過3天還未生成Commit，請注意
            // select  o.id, o.order_number, o.order_line_id, o.schedule_id, o.created_time, o.file_name from r_vt_order o where o.valid_flag=1 and o.status=0 and o.created_time<sysdate-3

            //--以下ASN超過3天還未傳輸Commit文件，請注意
            //  select o.id, o.order_number, o.order_line_id, o.schedule_id, o.created_time, o.edit_time commit_time, o.file_name from r_vt_order o where o.valid_flag=1 and o.status=1 and o.edit_time<sysdate-3
            #endregion

            #region 以下PO超過3天還未生成ASN，請注意           
            var dnList = sfcdb.Queryable<R_DN_STATUS>().Where(r => r.DN_FLAG == "3" && r.CREATETIME > SqlFunc.GetDate().AddDays(-30)).ToList();
            List<R_VT_ORDER> alertList = new List<R_VT_ORDER>();
            foreach (var dnObj in dnList)
            {
                try
                {
                    var orderList = sfcdb.Queryable<R_VT_ORDER>()
                        .Where(r => r.ORDER_NUMBER == dnObj.PO_NO && r.VALID_FLAG == 1 && r.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment))
                        .ToList();
                    foreach (var orderObj in orderList)
                    {
                        var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(orderObj.ORDER_DETAIL));
                        if (Convert.ToDouble(orderDetail.PromiseQty).Equals(dnObj.QTY) && dnObj.SKUNO.StartsWith(orderDetail.SupplierItemName))
                        {
                            if (!alertList.Any(r => r.ID.Equals(orderObj.ID)))
                            {
                                alertList.Add(orderObj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    R_MES_LOG logObj = new R_MES_LOG();
                    logObj.ID = MesDbBase.GetNewID<R_MES_LOG>(sfcdb, "VERTIV");
                    logObj.PROGRAM_NAME = "INTERFACE";
                    logObj.CLASS_NAME = "MESStation.Interface.Vertiv.VertivB2B";
                    logObj.FUNCTION_NAME = "GetASNAlertPO";
                    logObj.LOG_MESSAGE = $@"GetASNAlertPO error:{ex.Message}";
                    logObj.DATA1 = dnObj.DN_NO;
                    logObj.DATA2 = dnObj.DN_LINE;
                    logObj.DATA3 = dnObj.PO_NO;
                    logObj.EDIT_TIME = sfcdb.GetDate();
                    logObj.EDIT_EMP = "SYSTEM";
                    sfcdb.Insertable<R_MES_LOG>(logObj).ExecuteCommand();
                }
            }
            return alertList;
            #endregion
        }

        public void AutoSkipE2openCommit(SqlSugarClient sfcdb,int hours,string ip)
        {
            try
            {
                CheckRunning(sfcdb, "AutoSkipE2openCommit", ip, "VERTIV");
                MesLog.Info($@"Begin AutoSkipE2openCommit;");               
                var orderList = sfcdb.Queryable<R_VT_ORDER>()
                    .Where(r => r.VALID_FLAG == 1 && r.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForE2openCommit))
                    .ToList();
                foreach (var order in orderList)
                {
                    DateTime sysdate = sfcdb.GetDate();
                    if (order.EDIT_TIME < sysdate.AddHours(-hours))
                    {
                        order.EDIT_EMP = "SYSTEM";
                        order.EDIT_TIME = sysdate;
                        order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment);
                        sfcdb.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();
                        SaveCommitOrderInfo(sfcdb, order, sysdate, $@"AutoSkipE2openCommit;", "VERTIV", "SYSTEM",  false);
                    }
                }

                MesLog.Info($@"End AutoSkipE2openCommit;");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteRunningStatus(sfcdb, "AutoSkipE2openCommit", ip);
            }
        }

        public int SaveCommitOrderInfo(SqlSugarClient sfcdb, R_VT_ORDER orderObj, DateTime sysdate, string commitDetail,string BU,string emp, bool bToSend = true)
        {
            sfcdb.Updateable<R_VT_ORDER_COMMIT>()
                .SetColumns(r => r.VALID_FLAG == "0")
                .Where(r => r.VT_ORDER_ID == orderObj.ID && r.SEND_FLAG == 0 && r.VALID_FLAG == "1").ExecuteCommand();
            R_VT_ORDER_COMMIT orderCommit = new R_VT_ORDER_COMMIT();
            orderCommit.ID = MesDbBase.GetNewID<R_VT_ORDER_COMMIT>(sfcdb, BU);
            orderCommit.VT_ORDER_ID = orderObj.ID;
            orderCommit.COMMIT_TIME = sysdate;
            orderCommit.COMMIT_EMP = emp;
            orderCommit.COMMIT_DETAIL = commitDetail;
            orderCommit.SEND_FLAG = bToSend ? 0 : 1;
            orderCommit.VALID_FLAG = "1";
            return sfcdb.Insertable<R_VT_ORDER_COMMIT>(orderCommit).ExecuteCommand();
        }
    }

    public class AnalyesOrder : VertivB2B
    {
        public AnalyesOrder(string localPath, string remotePath, string buStr, SqlSugarClient sugarDB, string ip) : base(localPath, remotePath, buStr, sugarDB, ip)
        {
        }

        public override void Run()
        {
            try
            {
                CheckRunning("AnalyesOrder");
                MesLog.Info($@"Begin AnalyesOrder;");
                //統一下載到本地后，再從本地一個一個讀取進行解析
                AnalyesOld();
                //這個萬一程序有異常下載后沒有解析，就不會再重新解析，所以不用
                //AnalyesNew();
                MesLog.Info($@"End AnalyesOrder;");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteRunningStatus("AnalyesOrder");
            }
            
        }
        /// <summary>
        /// 統一下載到本地后，再從本地一個一個讀取進行解析
        /// </summary>
        public void AnalyesOld()
        {
            MesLog.Info($@"Begin download MTIMDiscreteOrder;");
            Download("MTIMDiscreteOrder");
            MesLog.Info($@"End download MTIMDiscreteOrder;");

            List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>().Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                .OrderBy(c => c.SEQ)
                .ToList();
            DateTime sysdate = SFCDB.GetDate();
            DataTable dtOrder = null;
            DirectoryInfo dirInfo = new DirectoryInfo(localPath);
            var fileList = dirInfo.GetFiles().OrderBy(r => r.LastAccessTime);
            foreach (FileInfo file in fileList)
            {
                try
                {
                    bool alreadyAnalyes = AlreadyAnalyes("MTIMDiscreteOrder", file.Name);
                    if (alreadyAnalyes)
                    {
                        MesLog.Info($@"AnalyesOrder,[{file.Name}] already analyes");
                        continue;
                    }

                    var lines = File.ReadAllLines(file.FullName);
                    dtOrder = new DataTable("MTIMDiscreteOrder");
                    for (var i = 0; i < lines.Length; i++)
                    {
                        if (i == 0)
                        {
                            var columns = lines[0].Split('\t');
                            for (var c = 0; c < columns.Length; c++)
                            {
                                var field = fieldList.Find(f => f.DISPLAY_NAME.Trim() == columns[c].Replace("#Header:", ""));
                                if (field != null)
                                {
                                    dtOrder.Columns.Add(field.FIELD_NAME.Trim());
                                }
                                else if (columns[c].Replace("#Header:", "").ToString().Trim().Equals("Requested Delivery Date"))
                                {
                                    dtOrder.Columns.Add($@"RequestDate");
                                }
                                else
                                {
                                    dtOrder.Columns.Add($@"Columns{c}");
                                }
                            }
                        }
                        else
                        {
                            var rowDatas = lines[i].Split('\t');
                            DataRow dr = dtOrder.NewRow();
                            for (var r = 0; r < rowDatas.Length; r++)
                            {
                                dr[r] = rowDatas[r].Replace("#Header:", "");
                            }
                            dtOrder.Rows.Add(dr);
                        }
                    }

                    SavePOData(SFCDB, dtOrder, sysdate, file.Name);
                    MoveFile(localPath, $@"{localPath}\Backup\", file.Name);
                }
                catch (Exception ex)
                {
                    MoveFile(localPath, $@"{localPath}\Error\", file.Name);
                    MesLog.Info($@"Analyse [{file.FullName}] fail;Error:{ex.Message}");
                }
            }
        }
        /// <summary>
        /// 下載一個文件解析一個文件，解析完后再下載另一個
        /// </summary>
        public void AnalyesNew()
        {
            localPath = localPath.EndsWith(@"\") ? localPath : $@"{localPath}\";
            var downloadFiles = sftpHelp.ListDirectory(remotePath).Where(r => r.Name.Contains("MTIMDiscreteOrder")).ToList().OrderBy(r => r.LastAccessTime);

            foreach (var file in downloadFiles)
            {
                if (file.Name.EndsWith(".txt"))
                {
                    MesLog.Info($@"AnalyesOrder,Begin download [{file.Name}]");
                    try
                    {
                        sftpHelp.Get(file.FullName, $@"{localPath}{file.Name}");
                        sftpHelp.Move(file.FullName, $@"{remotePath}\bak\" + file.Name);
                        //sftpHelp.Delete(file.FullName);                        
                        MesLog.Info($"AnalyesOrder,Download OK");
                    }
                    catch (Exception ex)
                    {
                        MesLog.Info($"AnalyesOrder,Download fail.Error message:{ex.Message}");
                        continue;
                    }
                    MesLog.Info($@"AnalyesOrder,End download [{file.Name}] ");

                    Thread.Sleep(3000);

                    MesLog.Info($@"AnalyesOrder,Begin analyes [{file.Name}] ");

                    bool alreadyAnalyes = AlreadyAnalyes("MTIMDiscreteOrder", file.Name);
                    if (alreadyAnalyes)
                    {
                        MesLog.Info($@"AnalyesOrder,[{file.Name}] already analyes");
                        continue;
                    }

                    SFCDB.Ado.BeginTran();
                    try
                    {
                        List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                            .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                            .OrderBy(c => c.SEQ).ToList();
                        DateTime sysdate = SFCDB.GetDate();
                        DataTable dtOrder = null;
                        var lines = File.ReadAllLines($@"{localPath}{file.Name}");
                        dtOrder = new DataTable("MTIMDiscreteOrder");
                        for (var i = 0; i < lines.Length; i++)
                        {
                            if (i == 0)
                            {
                                var columns = lines[0].Split('\t');
                                for (var c = 0; c < columns.Length; c++)
                                {
                                    var field = fieldList.Find(f => f.DISPLAY_NAME.Trim() == columns[c].Replace("#Header:", ""));
                                    if (field != null)
                                    {
                                        dtOrder.Columns.Add(field.FIELD_NAME.Trim());
                                    }
                                    else if (columns[c].Replace("#Header:", "").ToString().Trim().Equals("Requested Delivery Date"))
                                    {
                                        dtOrder.Columns.Add($@"RequestDate");
                                    }
                                    else
                                    {
                                        dtOrder.Columns.Add($@"Columns{c}");
                                    }
                                }
                            }
                            else
                            {
                                var rowDatas = lines[i].Split('\t');
                                DataRow dr = dtOrder.NewRow();
                                for (var r = 0; r < rowDatas.Length; r++)
                                {
                                    dr[r] = rowDatas[r].Replace("#Header:", "");
                                }
                                dtOrder.Rows.Add(dr);
                            }
                        }
                        SavePOData(SFCDB, dtOrder, sysdate, file.Name);
                        SFCDB.Ado.CommitTran();
                        MoveFile(localPath, $@"{localPath}\Backup\", file.Name);

                        MesLog.Info($@"Analyse OK;");
                    }
                    catch (Exception ex)
                    {
                        SFCDB.Ado.RollbackTran();
                        MoveFile(localPath, $@"{localPath}\Error\", file.Name);
                        MesLog.Info($@"Analyse fail;Error:{ex.Message}");
                    }
                    MesLog.Info($@"AnalyesOrder,End analyes {file.Name} ");
                }
            }
        }

        private void SavePOData(SqlSugarClient db, DataTable dtOrder, DateTime sysdate, string fileName)
        {
            foreach (DataRow row in dtOrder.Rows)
            {

                R_VT_ORDER newOrderObj = new R_VT_ORDER();
                newOrderObj.ID = MesDbBase.GetNewID<R_VT_ORDER>(db, buStr);
                newOrderObj.ORDER_NUMBER = row["OrderNumber"].ToString();
                newOrderObj.ORDER_LINE_ID = row["OrderLineId"].ToString();
                newOrderObj.SCHEDULE_ID = row["OrderRequestScheduleId"].ToString();
                newOrderObj.PROMISE_ID = row["OrderPromiseId"].ToString();
                newOrderObj.ACTION = row["Action"].ToString();
                newOrderObj.ORDER_DETAIL = System.Text.Encoding.Unicode.GetBytes(DataRowToJson(row, dtOrder.Columns));
                newOrderObj.VALID_FLAG = 1;
                newOrderObj.CREATED_EMP = "SYSTEM";
                newOrderObj.CREATED_TIME = sysdate;
                newOrderObj.EDIT_EMP = "SYSTEM";
                newOrderObj.EDIT_TIME = sysdate;
                newOrderObj.FILE_NAME = fileName;

                //OrderNumber,OrderLineId,OrderRequestScheduleId構成一個唯一條件
                var orderObj = db.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == row["OrderNumber"].ToString()
                        && r.ORDER_LINE_ID == row["OrderLineId"].ToString() && r.SCHEDULE_ID == row["OrderRequestScheduleId"].ToString()
                        && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                if (orderObj != null)
                {
                    if (orderObj.STATUS == EnumExtensions.ExtValue(OrderStatus.Closed))
                    {
                        newOrderObj.VALID_FLAG = 0;
                        newOrderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.Closed);
                    }
                    else
                    {
                        var shipmentRecordList = db.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == orderObj.ID && r.VALID_FLAG == "1").ToList();
                        if (shipmentRecordList.Count == 0)
                        {
                            if (row["Action"].ToString() == "Cancelled" || row["Action"].ToString() == "Cancel")
                            {
                                newOrderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.Cancelled);
                            }
                            else
                            {
                                newOrderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCommit);
                            }
                        }
                        else
                        {
                            double promiseQty = Convert.ToDouble(row["PromiseQty"].ToString());//再次發過來的出貨數量
                            double shipmentTotal = 0;//我們已經出貨的縂數量
                            foreach (var shipment in shipmentRecordList)
                            {
                                string detailStr = Encoding.Unicode.GetString(shipment.SHIPMENT_DETAIL);
                                SHIPMENT_DETAIL_VT shipmentDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(detailStr);
                                shipmentTotal = shipmentTotal + Convert.ToDouble(shipmentDetail.ShippedQuantity);
                            }

                            if (promiseQty < shipmentTotal)
                            {
                                //如果新進來的PromiseQty小於我們已經出貨的數量就報異常
                                throw new Exception($@"File:{fileName};OrderNumber:{row["OrderNumber"].ToString()};OrderLineId:{row["OrderLineId"].ToString()};ScheduleId:{row["OrderRequestScheduleId"].ToString()};PromiseQty Error!");
                            }

                            if (shipmentTotal == promiseQty)
                            {
                                //如果新進來的PromiseQty等於我們已經出貨的數量就把狀態設置為關節
                                newOrderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.Closed);
                            }
                            else
                            {
                                //如果新進來的PromiseQty大於我們已經出貨的數量就繼承原來的狀態
                                newOrderObj.STATUS = orderObj.STATUS;
                            }
                        }

                        //把這個PO這條line對應ScheduleId舊的作廢,以最新進來的為
                        //拆分數量后，會有多條line 和ScheduleId一樣的情況
                        //orderObj.VALID_FLAG = 0;
                        //orderObj.EDIT_EMP = "SYSTEM";
                        //orderObj.EDIT_TIME = sysdate;
                        //db.Updateable<R_VT_ORDER>(orderObj).Where(r => r.ID == orderObj.ID).ExecuteCommand();
                        //拆分數量后，會有多條line 和ScheduleId一樣的情況,所以都要作廢掉
                        db.Updateable<R_VT_ORDER>()
                            .SetColumns(r => new R_VT_ORDER { VALID_FLAG = 0, EDIT_EMP = "SYSTEM", EDIT_TIME = sysdate })
                            .Where(r => r.ORDER_NUMBER == row["OrderNumber"].ToString()
                             && r.ORDER_LINE_ID == row["OrderLineId"].ToString() && r.SCHEDULE_ID == row["OrderRequestScheduleId"].ToString()
                             && r.VALID_FLAG == 1).ExecuteCommand();
                    }
                }
                else
                {  
                    if(row["Action"].ToString()== "Cancelled" || row["Action"].ToString() == "Cancel")
                    {
                        newOrderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.Cancelled);
                    }
                    else
                    {
                        newOrderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCommit);
                    }                                      
                }
                db.Insertable<R_VT_ORDER>(newOrderObj).ExecuteCommand();

                //更新這個PO同一條LINE的其他ScheduleId版本為與新進來的ScheduleId的版本一致
                List<R_VT_ORDER> otherScheduleList = db.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == row["OrderNumber"].ToString()
                                            && r.ORDER_LINE_ID == row["OrderLineId"].ToString() && r.SCHEDULE_ID != row["OrderRequestScheduleId"].ToString()
                                            && r.VALID_FLAG == 1).ToList();
                foreach (var scheduleOrder in otherScheduleList)
                {
                    if(scheduleOrder.STATUS == EnumExtensions.ExtValue(OrderStatus.Closed) 
                        || scheduleOrder.STATUS == EnumExtensions.ExtValue(OrderStatus.Reject)
                        || scheduleOrder.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForE2openCommit)
                        || scheduleOrder.STATUS == EnumExtensions.ExtValue(OrderStatus.Cancelled)
                        || scheduleOrder.STATUS == EnumExtensions.ExtValue(OrderStatus.CancelASN))
                    {
                        continue;
                    }
                    string str = System.Text.Encoding.Unicode.GetString(scheduleOrder.ORDER_DETAIL);
                    ORDER_DETAIL_VT sOrderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(str);
                    string sOldRev = sOrderDetail.RevNumber;
                    if (sOldRev != row["RevNumber"].ToString())
                    {
                        sOrderDetail.RevNumber = row["RevNumber"].ToString();
                        string newScheduleDetailStr = Newtonsoft.Json.JsonConvert.SerializeObject(sOrderDetail, Newtonsoft.Json.Formatting.Indented);
                        scheduleOrder.ORDER_DETAIL = Encoding.Unicode.GetBytes(newScheduleDetailStr);
                        db.Updateable<R_VT_ORDER>(scheduleOrder).Where(r => r.ID == scheduleOrder.ID).ExecuteCommand();

                        R_MES_LOG logObj = new R_MES_LOG();
                        logObj.ID = MesDbBase.GetNewID<R_MES_LOG>(db, buStr);
                        logObj.PROGRAM_NAME = "VertivB2B";
                        logObj.CLASS_NAME = "MESStation.Interface.Vertiv.AnalyesOrder";
                        logObj.FUNCTION_NAME = "AnalyesNew";
                        logObj.LOG_MESSAGE = $@"Old RevNumber:{sOldRev};New RevNumber:{row["RevNumber"].ToString()}";
                        logObj.LOG_SQL = $@"Change By PO:{newOrderObj.ORDER_NUMBER},Line Id:{newOrderObj.ORDER_LINE_ID},Schedule Id:{newOrderObj.SCHEDULE_ID},Promise Id:{newOrderObj.PROMISE_ID},Id:{newOrderObj.ID}";
                        logObj.DATA1 = scheduleOrder.ID;
                        logObj.DATA2 = scheduleOrder.ORDER_NUMBER;
                        logObj.DATA3 = scheduleOrder.ORDER_LINE_ID;
                        logObj.DATA4 = scheduleOrder.SCHEDULE_ID;
                        logObj.EDIT_TIME = sysdate;
                        logObj.EDIT_EMP = "SYSTEM";
                        db.Insertable<R_MES_LOG>(logObj).ExecuteCommand();
                    }
                }


                //更新這個PO未出貨(做shipment)其它LINE的版本為與新進來的LINE的版本一致
                List<R_VT_ORDER> otherLineList = db.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == row["OrderNumber"].ToString()
                                            && r.ORDER_LINE_ID != row["OrderLineId"].ToString()
                                            && r.VALID_FLAG == 1).ToList();
                foreach (var otherObj in otherLineList)
                {
                    #region 全部更新為最新版本
                    //把這個PO未出貨(做shipment)其它LINE的版本為與新進來的LINE的版本一致
                    //bool shipmentRecord = db.Queryable<R_VT_SHIPMENT>().Any(r => r.ORDER_ID == otherObj.ID && r.VALID_FLAG == "1");
                    //bool otherShipment = shipmentRecord || otherObj.STATUS.Equals(EnumExtensions.ExtValue(OrderStatus.Closed)) ? true : false;

                    //if (!otherShipment)
                    //{
                    //    string str = System.Text.Encoding.Unicode.GetString(otherObj.ORDER_DETAIL);
                    //    ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(str);
                    //    string oldRev = orderDetail.RevNumber;
                    //    if (oldRev != row["RevNumber"].ToString())
                    //    {
                    //        orderDetail.RevNumber = row["RevNumber"].ToString();
                    //        string newDetailStr = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetail, Newtonsoft.Json.Formatting.Indented);
                    //        otherObj.ORDER_DETAIL = Encoding.Unicode.GetBytes(newDetailStr);
                    //        db.Updateable<R_VT_ORDER>(otherObj).Where(r => r.ID == otherObj.ID).ExecuteCommand();

                    //        R_MES_LOG logObj = new R_MES_LOG();
                    //        logObj.ID = MesDbBase.GetNewID<R_MES_LOG>(db, buStr);
                    //        logObj.PROGRAM_NAME = "VertivB2B";
                    //        logObj.CLASS_NAME = "MESStation.Interface.Vertiv.AnalyesOrder";
                    //        logObj.FUNCTION_NAME = "AnalyesNew";
                    //        logObj.LOG_MESSAGE = $@"Old RevNumber:{oldRev};New RevNumber:{row["RevNumber"].ToString()}";
                    //        logObj.DATA1 = otherObj.ID;
                    //        logObj.DATA2 = otherObj.ORDER_NUMBER;
                    //        logObj.DATA3 = otherObj.ORDER_LINE_ID;
                    //        logObj.DATA4 = otherObj.SCHEDULE_ID;
                    //        logObj.EDIT_TIME = sysdate;
                    //        logObj.EDIT_EMP = "SYSTEM";
                    //        db.Insertable<R_MES_LOG>(logObj).ExecuteCommand();
                    //    }
                    //}
                    #endregion

                    if (otherObj.STATUS == EnumExtensions.ExtValue(OrderStatus.Closed)
                        || otherObj.STATUS == EnumExtensions.ExtValue(OrderStatus.Reject)
                        || otherObj.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForE2openCommit)
                        || otherObj.STATUS == EnumExtensions.ExtValue(OrderStatus.Cancelled)
                        || otherObj.STATUS == EnumExtensions.ExtValue(OrderStatus.CancelASN))
                    {
                        continue;
                    }

                    string str = System.Text.Encoding.Unicode.GetString(otherObj.ORDER_DETAIL);
                    ORDER_DETAIL_VT otherOrderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(str);
                    string otherOldRev = otherOrderDetail.RevNumber;
                    if (otherOldRev != row["RevNumber"].ToString())
                    {
                        otherOrderDetail.RevNumber = row["RevNumber"].ToString();
                        string newOtherDetailStr = Newtonsoft.Json.JsonConvert.SerializeObject(otherOrderDetail, Newtonsoft.Json.Formatting.Indented);
                        otherObj.ORDER_DETAIL = Encoding.Unicode.GetBytes(newOtherDetailStr);
                        db.Updateable<R_VT_ORDER>(otherObj).Where(r => r.ID == otherObj.ID).ExecuteCommand();

                        R_MES_LOG logObj = new R_MES_LOG();
                        logObj.ID = MesDbBase.GetNewID<R_MES_LOG>(db, buStr);
                        logObj.PROGRAM_NAME = "VertivB2B";
                        logObj.CLASS_NAME = "MESStation.Interface.Vertiv.AnalyesOrder";
                        logObj.FUNCTION_NAME = "AnalyesNew";
                        logObj.LOG_MESSAGE = $@"Old RevNumber:{otherOldRev};New RevNumber:{row["RevNumber"].ToString()}";
                        logObj.LOG_SQL = $@"Change By PO:{newOrderObj.ORDER_NUMBER},Line Id:{newOrderObj.ORDER_LINE_ID},Schedule Id:{newOrderObj.SCHEDULE_ID},Promise Id:{newOrderObj.PROMISE_ID},Id:{newOrderObj.ID}";
                        logObj.DATA1 = otherObj.ID;
                        logObj.DATA2 = otherObj.ORDER_NUMBER;
                        logObj.DATA3 = otherObj.ORDER_LINE_ID;
                        logObj.DATA4 = otherObj.SCHEDULE_ID;
                        logObj.EDIT_TIME = sysdate;
                        logObj.EDIT_EMP = "SYSTEM";
                        db.Insertable<R_MES_LOG>(logObj).ExecuteCommand();
                    }
                }
            }
        }

        public void UpdateScheduleId(SqlSugarClient db)
        {
            var list = db.Queryable<R_VT_ORDER>().ToList();
            foreach (var item in list)
            {
                if(string.IsNullOrWhiteSpace(item.SCHEDULE_ID))
                {
                    string str = System.Text.Encoding.Unicode.GetString(item.ORDER_DETAIL);
                    ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(str);
                    item.SCHEDULE_ID = orderDetail.OrderRequestScheduleId;
                    db.Updateable<R_VT_ORDER>(item).Where(r=>r.ID==item.ID).ExecuteCommand();
                }
            }
        }
}

    public class AnalyesForecast : VertivB2B
    {
        public AnalyesForecast(string localPath, string remotePath, string buStr, SqlSugarClient sugarDB, string ip) : base(localPath, remotePath, buStr, sugarDB, ip)
        {
        }

        public override void Run()
        {
            try
            {
                CheckRunning("AnalyesForecast");
                MesLog.Info($@"Begin AnalyesForecast;");
                //統一下載到本地后，再從本地一個一個讀取進行解析
                AnalyesOld();
                //這個萬一程序有異常下載后沒有解析，就不會再重新解析，所以不用
                //AnalyesNew();
                MesLog.Info($@"End AnalyesForecast;");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteRunningStatus("AnalyesForecast");
            }
            
            

        }
        /// <summary>
        /// 統一下載到本地后，再從本地一個一個讀取進行解析
        /// </summary>
        public void AnalyesOld()
        {
            MesLog.Info($@"Begin download MTIMForecast;");
            Download("MTIMForecast");
            MesLog.Info($@"End download MTIMForecast;");
            List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MtimForecast")
                .OrderBy(c => c.SEQ)
                .ToList();
            DataTable dtForecast = null;
            DirectoryInfo dirInfo = new DirectoryInfo(localPath);
            List<FileInfo> fileList = dirInfo.GetFiles().OrderBy(f => f.LastAccessTime).ToList();
            foreach (FileInfo file in fileList)
            {
                //上一次的全部無效
                SFCDB.Updateable<R_VT_FORECAST>().SetColumns(r => new R_VT_FORECAST { VALID_FLAG = 0 }).Where(r => r.VALID_FLAG == 1).ExecuteCommand();
                DateTime sysdate = SFCDB.GetDate();
                string lotNo = $@"Forecast_{sysdate.ToString("yyyyMMddHHmmss")}";
                try
                {
                    bool alreadyAnalyes = AlreadyAnalyes("MTIMForecast", file.Name);
                    if (alreadyAnalyes)
                    {
                        MesLog.Info($@"[{file.Name}] already analyes");
                        continue;
                    }
                    var lines = File.ReadAllLines(file.FullName);
                    dtForecast = new DataTable("Forecast");
                    for (var i = 0; i < lines.Length; i++)
                    {
                        if (i == 0)
                        {
                            var columns = lines[0].Split('\t');
                            for (var c = 0; c < columns.Length; c++)
                            {
                                var field = fieldList.Find(f => f.DISPLAY_NAME.Trim() == columns[c].Replace("#Header:", ""));
                                if (field != null)
                                {
                                    dtForecast.Columns.Add(field.FIELD_NAME.Trim());
                                }
                                else
                                {
                                    dtForecast.Columns.Add($@"Columns{c}");
                                }
                            }
                        }
                        else
                        {
                            var rowDatas = lines[i].Split('\t');
                            DataRow dr = dtForecast.NewRow();
                            for (var r = 0; r < rowDatas.Length; r++)
                            {
                                dr[r] = rowDatas[r].Replace("#Header:", "");
                            }
                            dtForecast.Rows.Add(dr);
                        }
                    }
                    foreach (DataRow row in dtForecast.Rows)
                    {
                        R_VT_FORECAST forecast = new R_VT_FORECAST();
                        forecast.ID = MesDbBase.GetNewID<R_VT_FORECAST>(SFCDB, buStr);
                        forecast.CUSTOMER = row["Customer"].ToString();
                        forecast.SUPPLIER = row["Supplier"].ToString();
                        forecast.CUSTOMER_ITEM_NAME = row["Customer_Item_Name"].ToString();
                        forecast.SUPPLIER_ITEM_NAME = row["Supplier_Item_Name"].ToString();
                        forecast.SITE_NAME = row["Site_Name"].ToString();
                        forecast.SUPPLIER_SITE_NAME = row["Supplier_Site_Name"].ToString();
                        forecast.DATA_MEASURE = row["Data_Measure"].ToString();
                        if (!string.IsNullOrWhiteSpace(row["Quantity"].ToString()))
                        {
                            forecast.QUANTITY = Convert.ToDouble(row["Quantity"].ToString());
                        }
                        forecast.FORECAST_DATE = row["Date"].ToString();
                        forecast.FLEXATTR_STRING_PIT_01 = row["FlexAttr_String_PIT_01"].ToString();
                        forecast.FLEXATTR_STRING_PIT_02 = row["FlexAttr_String_PIT_02"].ToString();
                        forecast.FLEXATTR_STRING_PIT_03 = row["FlexAttr_String_PIT_03"].ToString();
                        forecast.FLEXATTR_STRING_PIT_04 = row["FlexAttr_String_PIT_04"].ToString();
                        forecast.CREATED_EMP = "SYSTEM";
                        forecast.CREATED_TIME = sysdate;
                        forecast.VALID_FLAG = 1;
                        forecast.FILE_NAME = file.Name;
                        forecast.LOT_NO = lotNo;
                        SFCDB.Insertable<R_VT_FORECAST>(forecast).ExecuteCommand();
                    }
                    MoveFile(localPath, $@"{localPath}\Backup\", file.Name);
                }
                catch (Exception ex)
                {
                    MoveFile(localPath, $@"{localPath}\Error\", file.Name);
                    MesLog.Info($@"Analyse [{file.FullName}] fail;Error:{ex.Message}");
                }
            }
        }
        /// <summary>
        /// 下載一個文件解析一個文件，解析完后再下載另一個
        /// </summary>
        public void AnalyesNew()
        {
            localPath = localPath.EndsWith(@"\") ? localPath : $@"{localPath}\";
            var downloadFiles = sftpHelp.ListDirectory(remotePath).Where(r => r.Name.Contains("MTIMForecast")).ToList().OrderBy(r => r.LastAccessTime);
            foreach (var file in downloadFiles)
            {
                if (file.Name.EndsWith(".txt"))
                {
                    MesLog.Info($@"AnalyesForecast,Begin download {file.Name} ");
                    try
                    {
                        sftpHelp.Get(file.FullName, $@"{localPath}{file.Name}");
                        sftpHelp.Move(file.FullName, $@"{remotePath}\bak\" + file.Name);
                        //sftpHelp.Delete(file.FullName);
                        MesLog.Info($"AnalyesForecast,Download OK.");
                    }
                    catch (Exception ex)
                    {
                        MesLog.Info($"AnalyesForecast,Download fail.Error message:{ex.Message}");
                        continue;
                    }
                    MesLog.Info($@"AnalyesForecast,End download {file.Name} ");

                    Thread.Sleep(3000);

                    MesLog.Info($@"Begin analyes [{file.Name}] ;");
                    try
                    {
                        bool alreadyAnalyes = AlreadyAnalyes("MTIMForecast", file.Name);
                        if (alreadyAnalyes)
                        {
                            MesLog.Info($@"[{file.Name}] already analyes");
                            continue;
                        }
                        List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                            .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MtimForecast")
                            .OrderBy(c => c.SEQ)
                            .ToList();

                        //上一次的全部無效
                        SFCDB.Updateable<R_VT_FORECAST>().SetColumns(r => new R_VT_FORECAST { VALID_FLAG = 0 }).Where(r => r.VALID_FLAG == 1).ExecuteCommand();
                        DateTime sysdate = SFCDB.GetDate();
                        DataTable dtForecast = null;
                        string lotNo = $@"Forecast_{sysdate.ToString("yyyyMMddHHmmss")}";
                        var lines = File.ReadAllLines($@"{localPath}{file.Name}");
                        dtForecast = new DataTable("Forecast");
                        for (var i = 0; i < lines.Length; i++)
                        {
                            if (i == 0)
                            {
                                var columns = lines[0].Split('\t');
                                for (var c = 0; c < columns.Length; c++)
                                {
                                    var field = fieldList.Find(f => f.DISPLAY_NAME.Trim() == columns[c].Replace("#Header:", ""));
                                    if (field != null)
                                    {
                                        dtForecast.Columns.Add(field.FIELD_NAME.Trim());
                                    }
                                    else
                                    {
                                        dtForecast.Columns.Add($@"Columns{c}");
                                    }
                                }
                            }
                            else
                            {
                                var rowDatas = lines[i].Split('\t');
                                DataRow dr = dtForecast.NewRow();
                                for (var r = 0; r < rowDatas.Length; r++)
                                {
                                    dr[r] = rowDatas[r].Replace("#Header:", "");
                                }
                                dtForecast.Rows.Add(dr);
                            }
                        }
                        foreach (DataRow row in dtForecast.Rows)
                        {
                            R_VT_FORECAST forecast = new R_VT_FORECAST();
                            forecast.ID = MesDbBase.GetNewID<R_VT_FORECAST>(SFCDB, buStr);
                            forecast.CUSTOMER = row["Customer"].ToString();
                            forecast.SUPPLIER = row["Supplier"].ToString();
                            forecast.CUSTOMER_ITEM_NAME = row["Customer_Item_Name"].ToString();
                            forecast.SUPPLIER_ITEM_NAME = row["Supplier_Item_Name"].ToString();
                            forecast.SITE_NAME = row["Site_Name"].ToString();
                            forecast.SUPPLIER_SITE_NAME = row["Supplier_Site_Name"].ToString();
                            forecast.DATA_MEASURE = row["Data_Measure"].ToString();
                            if (!string.IsNullOrWhiteSpace(row["Quantity"].ToString()))
                            {
                                forecast.QUANTITY = Convert.ToDouble(row["Quantity"].ToString());
                            }
                            forecast.FORECAST_DATE = row["Date"].ToString();
                            forecast.FLEXATTR_STRING_PIT_01 = row["FlexAttr_String_PIT_01"].ToString();
                            forecast.FLEXATTR_STRING_PIT_02 = row["FlexAttr_String_PIT_02"].ToString();
                            forecast.FLEXATTR_STRING_PIT_03 = row["FlexAttr_String_PIT_03"].ToString();
                            forecast.FLEXATTR_STRING_PIT_04 = row["FlexAttr_String_PIT_04"].ToString();
                            forecast.CREATED_EMP = "SYSTEM";
                            forecast.CREATED_TIME = sysdate;
                            forecast.VALID_FLAG = 1;
                            forecast.FILE_NAME = file.Name;
                            forecast.LOT_NO = lotNo;
                            SFCDB.Insertable<R_VT_FORECAST>(forecast).ExecuteCommand();
                        }
                        MoveFile(localPath, $@"{localPath}\Backup\", file.Name);
                        MesLog.Info($@"Analyes OK;");
                    }
                    catch (Exception ex)
                    {
                        MoveFile(localPath, $@"{localPath}\Error\", file.Name);
                        MesLog.Info($@"Analyse fail;Error:{ex.Message}");
                    }
                    MesLog.Info($@"End analyes [{file.Name}];");
                }
            }
        }
    }
    public class CommitOrder : VertivB2B
    {
        public CommitOrder(string localPath, string remotePath, string buStr, SqlSugarClient sugarDB, string ip) : base(localPath, remotePath, buStr, sugarDB, ip)
        {
        }
        public override void Run()
        {
            try
            {
                CheckRunning("CommitOrder");
                MesLog.Info($@"Begin CommitOrder;");
                MakeFileByScheduleId();
                MesLog.Info($@"End CommitOrder;");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteRunningStatus("CommitOrder");
            }
            
        }
        public void MakeFileByScheduleId()
        {
            //var orderList = SFCDB.Queryable<R_VT_ORDER_COMMIT, R_VT_ORDER>((c, o) => c.VT_ORDER_ID == o.ID)
            //    .Where((c, o) => c.SEND_FLAG == 0 && c.VALID_FLAG == "1" && o.VALID_FLAG == 1 && o.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile))
            //    .Select((c, o) => new { c, o }).ToList();

            List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                .OrderBy(c => c.SEQ).ToList();
            var commitList = SFCDB.Queryable<R_VT_ORDER_COMMIT>().Where(c => c.SEND_FLAG == 0 && c.VALID_FLAG == "1").ToList();
            List<R_VT_ORDER_COMMIT> sendList = new List<R_VT_ORDER_COMMIT>();
            foreach (var commit in commitList)
            {
                SFCDB.Ado.BeginTran();
                try
                {                    
                    if (sendList.Contains(commit))
                    {
                        SFCDB.Ado.CommitTran();
                        continue;
                    }
                    R_VT_ORDER order = SFCDB.Queryable<R_VT_ORDER>()
                        .Where(o => o.ID == commit.VT_ORDER_ID && o.VALID_FLAG == 1 && o.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile))
                        .ToList().FirstOrDefault();
                    if (order == null)
                    {
                        SFCDB.Ado.CommitTran();
                        continue;
                    }
                    DateTime sysdate = SFCDB.GetDate();
                    //SupplierDUNS_VERTIVDX_MTIMDiscreteOrderSupplier_1.0_ < YYYYMMDDHHMMSSS >.txt
                    //SupplierDUNS=544734668TEST
                    string fileName = $@"544734668_VERTIVDX_MTIMDiscreteOrderSupplier_1.0_{sysdate.ToString("yyyyMMddHHmmssfff")}.txt";
                    string fileFullName = $@"{localPath}\{fileName}";

                    //如果是拆PO，則要把拆開的line 合并在一個文件發送
                    if (commit.COMMIT_DETAIL.ToUpper().StartsWith("UPDATEPRIMISEQTY"))
                    {
                        List<R_VT_ORDER> orderList = SFCDB.Queryable<R_VT_ORDER>()
                            .Where(r => r.ORDER_NUMBER == order.ORDER_NUMBER && r.ORDER_LINE_ID == order.ORDER_LINE_ID && r.SCHEDULE_ID == order.SCHEDULE_ID
                            && r.VALID_FLAG == 1 && r.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForSendCommitFile)).ToList();
                        if (orderList.Count < 2)
                        {
                            //小於2應該是異常的，先暫時不是送，先檢查原因
                            continue;
                        }
                        using (FileStream fs = new FileStream(fileFullName, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5")))
                            {
                                sw.Flush();
                                sw.BaseStream.Seek(0, SeekOrigin.Current);
                                string headerLine = "#Header:";
                                foreach (var field in fieldList)
                                {
                                    if (field == fieldList.Last())
                                    {
                                        headerLine += $@"{field.DISPLAY_NAME.Trim()}";
                                    }
                                    else
                                    {
                                        headerLine += field.DISPLAY_NAME.Trim() + "\t";
                                    }
                                }
                                sw.WriteLine(headerLine);
                                foreach (var orderObj in orderList)
                                {
                                    string detailStr = Encoding.Unicode.GetString(orderObj.ORDER_DETAIL);
                                    ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);
                                    string contentLine = "";
                                    foreach (var field in fieldList)
                                    {
                                        object o = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(orderDetail);
                                        PropertyInfo p = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim());
                                        o = p.GetValue(orderDetail);
                                        var value = o == null ? "" : o.ToString();
                                        if (field == fieldList.Last())
                                        {
                                            contentLine += $@"{value}";
                                        }
                                        else
                                        {
                                            contentLine += value + "\t";
                                        }
                                    }
                                    sw.WriteLine(contentLine);

                                    R_VT_ORDER_COMMIT commitObj = SFCDB.Queryable<R_VT_ORDER_COMMIT>()
                                        .Where(c => c.VT_ORDER_ID == orderObj.ID && c.SEND_FLAG == 0 && c.VALID_FLAG == "1")
                                        .ToList().FirstOrDefault();

                                    orderObj.EDIT_TIME = sysdate;
                                    orderObj.EDIT_EMP = "SYSTEM";
                                    if (orderObj.ACTION == EnumExtensions.ExtName(OrderAction.Update))
                                    {
                                        if (commitObj.COMMIT_DETAIL.ToUpper().StartsWith("UPDATEPRIMISETIME") || commitObj.COMMIT_DETAIL.ToUpper().StartsWith("UpdatePrimiseTime"))
                                        {
                                            //只改時間,客人不會回復確認的文件
                                            orderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment);
                                        }
                                        else
                                        {
                                            orderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForE2openCommit);
                                        }

                                    }
                                    else if (orderObj.ACTION == EnumExtensions.ExtName(OrderAction.Reject))
                                    {
                                        orderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.Reject);
                                    }
                                    else
                                    {
                                        orderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment);
                                    }
                                    SFCDB.Updateable<R_VT_ORDER>(orderObj).Where(r => r.ID == orderObj.ID).ExecuteCommand();

                                    commitObj.SEND_FLAG = 1;
                                    commitObj.SEND_TIME = sysdate;
                                    commitObj.SEND_FILE = fileName;
                                    SFCDB.Updateable<R_VT_ORDER_COMMIT>(commitObj).Where(r => r.ID == commitObj.ID).ExecuteCommand();

                                    var tempCommit = commitList.Find(r => r.ID == commitObj.ID);                                    
                                    sendList.Add(tempCommit);
                                }
                                sw.Flush();
                            }
                        }
                    }
                    else
                    {   
                        using (FileStream fs = new FileStream(fileFullName, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5")))
                            {
                                sw.Flush();
                                sw.BaseStream.Seek(0, SeekOrigin.Current);
                                string headerLine = "#Header:";
                                foreach (var field in fieldList)
                                {
                                    if (field == fieldList.Last())
                                    {
                                        headerLine += $@"{field.DISPLAY_NAME.Trim()}";
                                    }
                                    else
                                    {
                                        headerLine += field.DISPLAY_NAME.Trim() + "\t";
                                    }
                                }
                                sw.WriteLine(headerLine);

                                string detailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                                ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);
                                string contentLine = "";
                                foreach (var field in fieldList)
                                {
                                    object o = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(orderDetail);
                                    PropertyInfo p = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim());
                                    o = p.GetValue(orderDetail);
                                    var value = o == null ? "" : o.ToString();
                                    if (field == fieldList.Last())
                                    {
                                        contentLine += $@"{value}";
                                    }
                                    else
                                    {
                                        contentLine += value + "\t";
                                    }
                                }
                                sw.WriteLine(contentLine);                                                   

                                order.EDIT_TIME = sysdate;
                                order.EDIT_EMP = "SYSTEM";
                                if (order.ACTION == EnumExtensions.ExtName(OrderAction.Update))
                                {
                                    if (commit.COMMIT_DETAIL.ToUpper().StartsWith("UPDATEPRIMISETIME") || commit.COMMIT_DETAIL.ToUpper().StartsWith("UpdatePrimiseTime"))
                                    {
                                        //只改時間,客人不會回復確認的文件
                                        order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment);
                                    }
                                    else
                                    {
                                        order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForE2openCommit);
                                    }

                                }
                                else if (order.ACTION == EnumExtensions.ExtName(OrderAction.Reject))
                                {
                                    order.STATUS = EnumExtensions.ExtValue(OrderStatus.Reject);
                                }
                                else
                                {
                                    order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment);
                                }
                                SFCDB.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();

                                commit.SEND_FLAG = 1;
                                commit.SEND_TIME = sysdate;
                                commit.SEND_FILE = fileName;
                                SFCDB.Updateable<R_VT_ORDER_COMMIT>(commit).Where(r => r.ID == commit.ID).ExecuteCommand();

                                sw.Flush();
                            }
                        }                        
                    }
                    System.Threading.Thread.Sleep(2000);
                    UploadSftp(fileFullName, fileName);
                    System.Threading.Thread.Sleep(3000);
                    SFCDB.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    SFCDB.Ado.RollbackTran();
                    MesLog.Info($@"CommitOrder make file fail.Commit_id:{commit.ID};ORDER_ID:{commit.VT_ORDER_ID}; Error:{ex.Message}");
                }
            }
        }
        public void MakeFile()
        {

            var wList = SFCDB.Queryable<R_VT_ORDER_COMMIT, R_VT_ORDER>((c, o) => c.VT_ORDER_ID == o.ID)
                .Where((c, o) => c.SEND_FLAG == 0 && c.VALID_FLAG == "1")
                .GroupBy((c, o) => o.ORDER_NUMBER)
                .GroupBy((c, o) => o.ORDER_LINE_ID).Select((c, o) => new { o.ORDER_NUMBER }).Distinct().ToList();

            List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMDiscreteOrder")
                .OrderBy(c => c.SEQ).ToList();

            foreach (var w in wList)
            {
                SFCDB.Ado.BeginTran();
                try
                {
                    List<R_VT_ORDER> orderList = SFCDB.Queryable<R_VT_ORDER, R_VT_ORDER_COMMIT>((o, c) => c.VT_ORDER_ID == o.ID)
                   .Where((o, c) => o.ORDER_NUMBER == w.ORDER_NUMBER && c.SEND_FLAG == 0 && c.VALID_FLAG == "1")
                   .Select((o, c) => o).ToList();
                    DateTime sysdate = SFCDB.GetDate();

                    //SupplierDUNS_VERTIVDX_MTIMDiscreteOrderSupplier_1.0_ < YYYYMMDDHHMMSSS >.txt
                    //SupplierDUNS=544734668TEST
                    string fileName = $@"544734668_VERTIVDX_MTIMDiscreteOrderSupplier_1.0_{sysdate.ToString("yyyyMMddHHmmssfff")}.txt";
                    string fileFullName = $@"{localPath}\{fileName}";
                    using (FileStream fs = new FileStream(fileFullName, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5")))
                        {
                            sw.Flush();
                            sw.BaseStream.Seek(0, SeekOrigin.Current);
                            string headerLine = "#Header:";
                            foreach (var field in fieldList)
                            {
                                if (field == fieldList.Last())
                                {
                                    headerLine += $@"{field.DISPLAY_NAME.Trim()}";
                                }
                                else
                                {
                                    headerLine += field.DISPLAY_NAME.Trim() + "\t";
                                }
                            }
                            sw.WriteLine(headerLine);
                            foreach (var order in orderList)
                            {
                                string detailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                                ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);
                                string contentLine = "";
                                foreach (var field in fieldList)
                                {
                                    object o = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(orderDetail);
                                    PropertyInfo p = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim());
                                    o = p.GetValue(orderDetail);
                                    var value = o == null ? "" : o.ToString();
                                    if (field == fieldList.Last())
                                    {
                                        contentLine += $@"{value}";
                                    }
                                    else
                                    {
                                        contentLine += value + "\t";
                                    }
                                }
                                sw.WriteLine(contentLine);
                                SFCDB.Updateable<R_VT_ORDER_COMMIT>()
                                    .SetColumns(r => new R_VT_ORDER_COMMIT() { SEND_FLAG = 1, SEND_TIME = sysdate, SEND_FILE = fileName })
                                    .Where(r => r.VT_ORDER_ID == order.ID && r.SEND_FLAG == 0 && r.VALID_FLAG == "1")
                                    .ExecuteCommand();

                                order.EDIT_TIME = sysdate;
                                order.EDIT_EMP = "SYSTEM";
                                order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment);
                                SFCDB.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();
                            }
                            sw.Flush();
                        }
                    }
                    UploadSftp(fileFullName, fileName);
                    System.Threading.Thread.Sleep(1000);
                    SFCDB.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    SFCDB.Ado.RollbackTran();
                    MesLog.Info($@"CommitOrder make file fail.ORDER_NUMBER:{w};Error:{ex.Message}");
                }

                #region Old                
                //FileStream fs = new FileStream(saveFile, FileMode.Append, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
                //sw.Flush();
                //sw.BaseStream.Seek(0, SeekOrigin.Current);
                //try
                //{
                //    string headerLine = "#Header:";
                //    foreach (var field in fieldList)
                //    {
                //        if (field == fieldList.Last())
                //        {
                //            headerLine += $@"{field.DISPLAY_NAME.Trim()}";
                //        }
                //        else
                //        {
                //            headerLine += field.DISPLAY_NAME.Trim() + "\t";
                //        }
                //    }
                //    sw.WriteLine(headerLine);
                //    foreach (var order in orderList)
                //    {
                //        string detailStr = Encoding.Unicode.GetString(order.ORDER_DETAIL);
                //        ORDER_DETAIL_VT orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(detailStr);
                //        string contentLine = "";
                //        foreach (var field in fieldList)
                //        {
                //            object o = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(orderDetail);
                //            PropertyInfo p = orderDetail.GetType().GetProperty(field.FIELD_NAME.Trim());
                //            o = p.GetValue(orderDetail);
                //            var value = o == null ? "" : o.ToString();
                //            if (field == fieldList.Last())
                //            {
                //                contentLine += $@"{value}";
                //            }
                //            else
                //            {
                //                contentLine += value + "\t";
                //            }
                //        }
                //        sw.WriteLine(contentLine);
                //        SFCDB.Updateable<R_VT_ORDER_COMMIT>()
                //            .SetColumns(r => new R_VT_ORDER_COMMIT() { SEND_FLAG = 1, SEND_TIME = sysdate, SEND_FILE = newFileName })
                //            .Where(r => r.VT_ORDER_ID == order.ID && r.SEND_FLAG == 0 && r.VALID_FLAG == "1")
                //            .ExecuteCommand();

                //        order.EDIT_TIME = sysdate;
                //        order.EDIT_EMP = "SYSTEM";
                //        order.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitCreatShipment);
                //        SFCDB.Updateable<R_VT_ORDER>(order).Where(r => r.ID == order.ID).ExecuteCommand();
                //    }
                //    sw.Flush();
                //    sw.Close();
                //    UploadSftp(saveFile, newFileName);
                //}
                //catch (Exception ex)
                //{
                //    MesLog.Info(ex.Message);
                //    sw.Flush();
                //    sw.Close();
                //    File.Delete(saveFile);
                //}
                #endregion
            }
        }

    }
    public class CommitForecast : VertivB2B
    {
        public CommitForecast(string localPath, string remotePath, string buStr, SqlSugarClient sugarDB, string ip) : base(localPath, remotePath, buStr, sugarDB, ip)
        {
        }
        public override void Run()
        {
            try
            {
                CheckRunning("CommitForecast");
                MesLog.Info($@"Begin CommitForecast;");
                SaveData();
                MakeFile();
                MesLog.Info($@"End CommitForecast;");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteRunningStatus("CommitForecast");
            }

            
        }

        public void SaveData()
        {
            List<R_VT_FORECAST> forecastList = SFCDB.Queryable<R_VT_FORECAST>().Where(r => r.VALID_FLAG == 1).ToList();
            DateTime sysdate = SFCDB.GetDate();
            List<R_VT_FORECAST_COMMIT> commitList = new List<R_VT_FORECAST_COMMIT>();
            foreach (var item in forecastList)
            {
                SFCDB.Updateable<R_VT_FORECAST_COMMIT>().SetColumns(r => new R_VT_FORECAST_COMMIT { VALID_FLAG = 0 })
                    .Where(r => r.COMMIT_ID == item.ID).ExecuteCommand();
                R_VT_FORECAST_COMMIT commit = new R_VT_FORECAST_COMMIT();
                commit.ID = MesDbBase.GetNewID<R_VT_FORECAST>(SFCDB, buStr);
                commit.COMMIT_ID = item.ID;
                commit.CUSTOMER = item.CUSTOMER;
                commit.SUPPLIER = item.SUPPLIER;
                commit.CUSTOMER_ITEM_NAME = item.CUSTOMER_ITEM_NAME;
                commit.SUPPLIER_ITEM_NAME = item.SUPPLIER_ITEM_NAME;
                commit.SITE_NAME = item.SITE_NAME;
                commit.SUPPLIER_SITE_NAME = item.SUPPLIER_SITE_NAME;
                commit.DATA_MEASURE = "ConsumptionCommit";// item.DATA_MEASURE;
                commit.QUANTITY = item.QUANTITY;
                commit.COMMIT_DATE = item.FORECAST_DATE;
                commit.FLEXATTR_STRING_PIT_01 = item.FLEXATTR_STRING_PIT_01;
                commit.FLEXATTR_STRING_PIT_02 = item.FLEXATTR_STRING_PIT_02;
                commit.FLEXATTR_STRING_PIT_03 = item.FLEXATTR_STRING_PIT_03;
                commit.FLEXATTR_STRING_PIT_04 = item.FLEXATTR_STRING_PIT_04;
                commit.VALID_FLAG = 1;
                commit.SEND_FLAG = 0;
                commit.CREATED_EMP = "SYSTEM";
                commit.CREATED_TIME = sysdate;
                SFCDB.Insertable<R_VT_FORECAST_COMMIT>(commit).ExecuteCommand();
            }
        }

        public void MakeFile()
        {
            SFCDB.Ado.BeginTran();
            try
            {
                List<R_VT_FORECAST_COMMIT> waitSendList = SFCDB.Queryable<R_VT_FORECAST_COMMIT>().Where(r => r.VALID_FLAG == 1 && r.SEND_FLAG == 0).ToList();
                List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                    .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MtimCommit")
                    .OrderBy(c => c.SEQ).ToList();
                DateTime sysdate = SFCDB.GetDate();
                //SupplierDUNS=544734668TEST
                //SupplierDUNS_VERTIVDX_MTIMCommitSupplier_1.0_ < YYYYMMDDHHMMSSS >.txt               
                string fileName = $@"544734668_VERTIVDX_MTIMCommitSupplier_1.0_{sysdate.ToString("yyyyMMddHHmmssfff")}.txt";
                string fileFullName = $@"{localPath}\{fileName}";
                using (FileStream fs = new FileStream(fileFullName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5")))
                    {
                        //通过指定字符编码方式可以实现对汉字的支持，否则在用记事本打开查看会出现乱码            
                        sw.Flush();
                        sw.BaseStream.Seek(0, SeekOrigin.Current);
                        string headerLine = "#Header:";
                        foreach (var field in fieldList)
                        {
                            if (field == fieldList.Last())
                            {
                                headerLine += $@"{field.DISPLAY_NAME}";
                            }
                            else
                            {
                                headerLine += field.DISPLAY_NAME + "\t";
                            }
                        }

                        sw.WriteLine(headerLine);
                        var propertys = typeof(R_VT_FORECAST_COMMIT).GetProperties();
                        foreach (var item in waitSendList)
                        {
                            string contentLine = "";
                            foreach (var field in fieldList)
                            {
                                object o = item.GetType().GetProperty(field.FIELD_NAME.ToUpper()).GetValue(item, null);
                                var value = o == null ? "" : o.ToString();
                                if (field == fieldList.Last())
                                {
                                    contentLine += $@"{value}";
                                }
                                else
                                {
                                    contentLine += value + "\t";
                                }
                            }
                            sw.WriteLine(contentLine);
                            item.SEND_FLAG = 1;
                            item.SEND_TIME = sysdate;
                            item.SEND_FILE_NAME = fileName;
                            SFCDB.Updateable<R_VT_FORECAST_COMMIT>(item).Where(r => r.ID == item.ID).ExecuteCommand();
                        }
                        sw.Flush();
                    }
                }
                UploadSftp(fileFullName, fileName);
                System.Threading.Thread.Sleep(1000);
                SFCDB.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                SFCDB.Ado.RollbackTran();
                MesLog.Info($@"CommitForecast make file fail.Error:{ex.Message}");
            }
        }

    }

    public class SendShipment : VertivB2B
    {
        public SendShipment(string localPath, string remotePath, string buStr, SqlSugarClient sugarDB, string ip) : base(localPath, remotePath, buStr, sugarDB, ip)
        {
        }

        public override void Run()
        {
            try
            {
                CheckRunning("SendShipment");
                MesLog.Info($@"Begin SaveShipment;");
                //AutoCreatShipment();
                MakeFile();
                MesLog.Info($@"End SaveShipment;");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DeleteRunningStatus("SendShipment");
            }
        }
        public void AutoCreatShipment()
        {
            var dnList = SFCDB.Ado.SqlQuery<R_DN_STATUS>($@"select * from r_dn_status d where d.dn_flag='3' and d.gtdate is not null
                and not exists(select * from r_vt_shipment s where s.dn_no = d.dn_no and valid_flag = 1)                
                and d.gtdate > sysdate - 45 ");
            VertivPOApi api = new VertivPOApi();
            api.BU = "VERTIV";
            api.DBTYPE = DB_TYPE_ENUM.Oracle;
            foreach (var dnObj in dnList)
            {
                try
                {
                    var orderList = SFCDB.Queryable<R_VT_ORDER>().Where(r => r.ORDER_NUMBER == dnObj.PO_NO 
                        && r.VALID_FLAG == 1 && r.STATUS == EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment))
                        .ToList();
                    R_VT_ORDER order = null;
                    foreach (var orderObj in orderList)
                    {
                        var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(orderObj.ORDER_DETAIL));
                        if (Convert.ToDouble(orderDetail.PromiseQty).Equals(dnObj.QTY) && dnObj.SKUNO.StartsWith(orderDetail.SupplierItemName))
                        {
                            order = orderObj;
                            break;
                        }
                    }
                    if (order != null)
                    {
                        api.DoCreatShipment(SFCDB, order, dnObj.DN_NO,"SYSTEM");
                    }
                }
                catch (Exception ex)
                {
                    R_MES_LOG logObj = new R_MES_LOG();
                    logObj.ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB, "VERTIV");
                    logObj.PROGRAM_NAME = "INTERFACE";
                    logObj.CLASS_NAME = "MESStation.Interface.Vertiv.SendShipment";
                    logObj.FUNCTION_NAME = "AutoCreatShipment";
                    logObj.LOG_MESSAGE = $@"AutoCreatShipment error:{ex.Message}";
                    logObj.DATA1 = dnObj.DN_NO;
                    logObj.DATA2 = dnObj.DN_LINE;
                    logObj.DATA3 = dnObj.PO_NO;
                    logObj.EDIT_TIME = SFCDB.GetDate();
                    logObj.EDIT_EMP = "SYSTEM";
                    SFCDB.Insertable<R_MES_LOG>(logObj).ExecuteCommand();
                }                
            }
        }
        public void MakeFile()
        {

            var sList = SFCDB.Queryable<R_VT_SHIPMENT>().Where(r => r.SEND_FLAG == "0").OrderBy(r => r.CREATED_TIME, OrderByType.Asc).ToList();

            List<C_CUSTOMER_FILE_FORMAT> fieldList = SFCDB.Queryable<C_CUSTOMER_FILE_FORMAT>()
                .Where(c => c.BU == "VERTIV" && c.FILE_NAME == "MTIMShipment")
                .OrderBy(c => c.SEQ).ToList();

            foreach (var shipment in sList)
            {
                DateTime sysdate = SFCDB.GetDate();
                //SupplierDUNS_VERTIVDX_MTIMShipmentSupplier_1.0_ < YYYYMMDDHHMMSSS >.txt
                //SupplierDUNS=544734668TEST                
                string fileName = $@"544734668_VERTIVDX_MTIMShipmentSupplier_1.0_{sysdate.ToString("yyyyMMddHHmmssfff")}.txt";
                string fileFullName = $@"{localPath}\{fileName}";
                SFCDB.Ado.BeginTran();
                try
                {
                    using (FileStream fs = new FileStream(fileFullName, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5")))
                        {
                            sw.Flush();
                            sw.BaseStream.Seek(0, SeekOrigin.Current);
                            string headerLine = "#Header:";
                            foreach (var field in fieldList)
                            {
                                if (field == fieldList.Last())
                                {
                                    headerLine += $@"{field.DISPLAY_NAME.Trim()}";
                                }
                                else
                                {
                                    headerLine += field.DISPLAY_NAME.Trim() + "\t";
                                }
                            }
                            sw.WriteLine(headerLine);

                            string detailStr = Encoding.Unicode.GetString(shipment.SHIPMENT_DETAIL);
                            SHIPMENT_DETAIL_VT shipmentDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(detailStr);
                            string contentLine = "";
                            foreach (var field in fieldList)
                            {
                                object o = shipmentDetail.GetType().GetProperty(field.FIELD_NAME.Trim()).GetValue(shipmentDetail);
                                PropertyInfo p = shipmentDetail.GetType().GetProperty(field.FIELD_NAME.Trim());
                                o = p.GetValue(shipmentDetail);
                                var value = o == null ? "" : o.ToString();
                                if (field == fieldList.Last())
                                {
                                    contentLine += $@"{value}";
                                }
                                else
                                {
                                    contentLine += value + "\t";
                                }
                            }
                            sw.WriteLine(contentLine);
                            sw.Flush();
                            sw.Close();

                            shipment.SEND_TIME = sysdate;
                            shipment.SEND_EMP = "SYSTEM";
                            shipment.SEND_FLAG = "1";
                            shipment.FILE_NAME = fileName;
                            SFCDB.Updateable<R_VT_SHIPMENT>(shipment).Where(r => r.ID == shipment.ID).ExecuteCommand();


                            var orderObj = SFCDB.Queryable<R_VT_ORDER>().Where(r => r.ID == shipment.ORDER_ID && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                            var orderDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<ORDER_DETAIL_VT>(Encoding.Unicode.GetString(orderObj.ORDER_DETAIL));
                            List<R_VT_SHIPMENT> shipmentList = SFCDB.Queryable<R_VT_SHIPMENT>().Where(r => r.ORDER_ID == orderObj.ID && r.VALID_FLAG == "1").ToList();
                            double shippedTotalQty = 0.00;
                            if (shipmentList.Count > 0)
                            {
                                foreach (var sObj in shipmentList)
                                {
                                    string dStr = Encoding.Unicode.GetString(sObj.SHIPMENT_DETAIL);
                                    SHIPMENT_DETAIL_VT sDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<SHIPMENT_DETAIL_VT>(dStr);
                                    shippedTotalQty = shippedTotalQty + Convert.ToDouble(sDetail.ShippedQuantity);
                                }
                            }
                            if (Convert.ToDouble(orderDetail.PromiseQty) - shippedTotalQty == 0)
                            {
                                orderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.Closed);
                            }
                            else
                            {
                                orderObj.STATUS = EnumExtensions.ExtValue(OrderStatus.WaitForCreatShipment);
                            }
                            orderObj.EDIT_TIME = sysdate;
                            orderObj.EDIT_EMP = "SYSTEM";
                            SFCDB.Updateable<R_VT_ORDER>(orderObj).Where(r => r.ID == orderObj.ID).ExecuteCommand();
                        }
                    }
                    UploadSftp(fileFullName, fileName);
                    System.Threading.Thread.Sleep(3000);
                    SFCDB.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    SFCDB.Ado.RollbackTran();
                    MesLog.Info($@"SendShipment make file fail.Shipment id:{shipment.ID}; Error:{ex.Message}");
                }
            }
        }
    }
        
}
