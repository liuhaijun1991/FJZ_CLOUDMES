using DcnSfcModel;
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESInterface.DCN
{
    public class FNNSendUFIDataTaskSyn: taskBase
    {
        private bool IsRuning = false;
        private string mesdbstr, custdbstr, plantstr, taskNum ,ip;
        //private string mesdbstr, apdbstr, bustr, keypath, filetype, filepath, filebackpath, remotepath, plant,taskNum;
        //private bool IsPROD = true;
        //public string csvFilePath = "";

        public override void init()
        {
            try
            {
                //apdbstr = ConfigGet("APDB");
                mesdbstr = ConfigGet("MESDB");
                custdbstr = ConfigGet("CUSTDB");
                plantstr = ConfigGet("PLANT");               
                taskNum = ConfigGet("TASK_NUM");                
                Output.UI = new FNNSendUFIDataUI(this);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }      

        public override void Start()
        {
            try
            {
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesdbstr, false, SqlSugar.DbType.SqlServer);
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                DateTime StartDate = GetLastRunTime(SFCDB);
                DateTime EndDate = SFCDB.GetDate();               
                SendData(StartDate, EndDate);
                WriteApLog(SFCDB, EndDate, ip);
                SFCDB.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        
        public void SendData(DateTime StartDate, DateTime EndDate)
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes> listresult = new Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes>();
            try
            {
                MesLog.Info("Start");
                if (plantstr == "")
                {
                    throw new Exception("Please setting PLANT");
                }                
                Run(StartDate,EndDate, taskNum);
            }
            catch (Exception ex)
            {
                MesLog.Info($@"Error:{ex.Message}");
                throw ex;
            }
            finally
            {
                try
                {
                    foreach (var r in listresult)
                    {
                        string log = $@"{r.Key} send ";
                        if (r.Value.IsSuccess)
                        {
                            log += $@"success";
                        }
                        else
                        {
                            log += $@"fail;Error message:{r.Value.ErrorMessage}";
                        }
                        MesLog.Info(log);
                    }
                }
                catch (Exception)
                {
                }
                MesLog.Info("End");
                IsRuning = false;
            }
        }

        public void Run(DateTime StartDate, DateTime EndDate, string taskNum)
        {
            try
            {
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesdbstr, false, SqlSugar.DbType.SqlServer);
                SqlSugarClient CUSTDB = OleExec.GetSqlSugarClient(custdbstr, false, SqlSugar.DbType.SqlServer);
                string collectedStartDateStr = StartDate.ToString().Replace('-', '/');
                string collectedEndDateStr = EndDate.ToString().Replace('-', '/');
                DataTable SNDetail = GetSNDetail(SFCDB, collectedStartDateStr, collectedEndDateStr);
                DataTable SNLink = GetSNLink(SFCDB, collectedStartDateStr, collectedEndDateStr);
                DataTable SNRepair = GetSNRepair(SFCDB, collectedStartDateStr, collectedEndDateStr);
                InsertSNDetail(SFCDB, CUSTDB, SNDetail);
                InsertSNLink(SFCDB, CUSTDB, SNLink);
                InsertSNRepair(SFCDB, CUSTDB, SNRepair);
                

            }
            catch (Exception ex)
            {
                throw new Exception($@"Class:FNNSendUFIDataTaskSyn;Function:Run;Error Msg: {ex.Message}");
            }
        }

        //public DataTable GetDataList(SqlSugarClient SFCDB, string collectedDateStr)
        public DataTable GetSNDetail(SqlSugarClient SFCDB, string collectedStartDateStr, string collectedEndDateStr)
        {
            try
            {


                return SFCDB.Ado.GetDataTable($@" select case when charindex('PCBA', UPPER(b.SfcRoute)) > 0 then 'PCBA'
                                                         when charindex('VANILLA', UPPER(b.SfcRoute)) > 0 then 'VANILLA'
                                                         when charindex('CTO', UPPER(b.SfcRoute)) > 0 then 'CTO'
                                                       end as skutype,
                                                       a.sysserialno SN,
                                                       a.SKUNO,
                                                       a.WORKORDERNO,
                                                       a.EVENTPASS,
                                                       a.eventname Station_name,
                                                       a.productstatus PRODUCT_STATUS,
                                                       a.scandatetime EDIT_TIME
                                                  from mfsysevent a(nolock),
                                                       (select distinct n.skuno, n.SfcRoute
                                                          from mmprodmaster m, sfcCodeLike n
                                                         where m.SourceType = 'UFISPACE'
                                                           and m.partname = n.SkuNo) b
                                                 where a.Skuno = b.SkuNo
                                                   and exists
                                                 (select 1 from mfworkstatus c where a.sysserialno = c.sysserialno)
                                                   and a.sysserialno not like '[~#*R]'
                                                   and a.scandatetime > '{collectedStartDateStr}' 
                                                   and a.scandatetime < '{collectedEndDateStr}' ");
            }
            catch (Exception ex)
            {
                throw new Exception($@"Running Collect SNdetail Data Fail;Fail Msg:{ex.Message}");
            }
        }

        public DataTable GetSNLink(SqlSugarClient SFCDB, string collectedStartDateStr, string collectedEndDateStr)
        {
            try
            {


                return SFCDB.Ado.GetDataTable($@"   select a.sysserialno SN,
                                                           a.cserialno   Value,
                                                           a.partno,
                                                           a.eventpoint  station,
                                                           a.eeecode     location,
                                                           a.scandt      Edit_Time
                                                      from mfsyscserial a
                                                     where exists (select 1
                                                              from ((select v.sysserialno
                                                                       from mmprodmaster m, sfcCodeLike n, mfsysproduct v
                                                                      where m.SourceType = 'UFISPACE'
                                                                        and m.partname = n.SkuNo                                                                      
                                                                        and n.skuno = v.skuno)) t
                                                             where a.sysserialno = t.sysserialno)
		                                                     and a.sysserialno not like '[~#*R]%'
                                                        and a.scandt > '{collectedStartDateStr}' 
                                                        and a.scandt < '{collectedEndDateStr}' ");
            }
            catch (Exception ex)
            {
                throw new Exception($@"Running Collect SNLink Data Fail;Fail Msg:{ex.Message}");
            }
        }

        public DataTable GetSNRepair(SqlSugarClient SFCDB, string collectedStartDateStr, string collectedEndDateStr)
        {
            try
            {


                return SFCDB.Ado.GetDataTable($@" SELECT CASE WHEN CHARINDEX('PCBA', UPPER(E.SFCROUTE)) > 0 THEN 'PCBA'
                                                              WHEN CHARINDEX('VANILLA', UPPER(E.SFCROUTE)) > 0 THEN 'VANILLA'
                                                              WHEN CHARINDEX('CTO', UPPER(E.SFCROUTE)) > 0 THEN 'CTO'
                                                        END AS SKUTYPE,
                                                        A.SYSSERIALNO AS SN,
                                                        A.WORKORDERNO,
                                                        D.SKUNO,
                                                        A.FAILURESTATION AS FAIL_STATION,
                                                        B.FAILTIME AS FAIL_TIME,
                                                        B.LASTEDITDT AS EDIT_TIME,
                                                        B.FAILCODE AS FAIL_CODE,
                                                        B.FAILLOCATION AS FAIL_LOCATION,
                                                        C.DESCRIPTION AS DEFECT_DESCRIPTION,
                                                        B.DESCRIPTION AS SYMPTOM_DESCRIPTION
                                                    FROM SFCREPAIRMAIN A
                                                    LEFT JOIN SFCREPAIRFAILCODE B
                                                    ON A.SYSSERIALNO = B.SYSSERIALNO
                                                    AND A.CREATEDATE = B.CREATEDATE
                                                    AND B.FAILCATEGORY = 'SYMPTOM'
                                                    LEFT JOIN SFCREPAIRFAILCODE C
                                                    ON A.SYSSERIALNO = C.SYSSERIALNO
                                                    AND A.CREATEDATE = C.CREATEDATE
                                                    AND C.FAILCATEGORY = 'DEFECT', MFSYSPRODUCT D,
                                                    (SELECT DISTINCT N.SKUNO, N.SFCROUTE
                                                            FROM MMPRODMASTER M, SFCCODELIKE N
                                                            WHERE M.SOURCETYPE = 'UFISPACE'
                                                            AND M.PARTNAME = N.SKUNO) E
                                                    WHERE A.SYSSERIALNO = D.SYSSERIALNO
                                                    AND D.SKUNO = E.SKUNO
                                                    AND A.SYSSERIALNO NOT LIKE '[R~#*]%'
                                                    AND B.LASTEDITDT > '{collectedStartDateStr}' 
                                                    AND B.LASTEDITDT < '{collectedEndDateStr}' ");
            }
            catch (Exception ex)
            {
                throw new Exception($@"Running Collect SNRepair Data Fail;Fail Msg:{ex.Message}");
            }
        }                     

        public void InsertSNDetail(SqlSugarClient SFCDB, SqlSugarClient CUSTDB, DataTable SNDetail)
        {
            if (SNDetail != null)
            {
                try
                {
                    foreach (DataRow r in SNDetail.Rows)
                    {
                        switch (r["skutype"].ToString())
                        {
                            case "PCBA":
                                {
                                    var checkpcba = CUSTDB.Ado.GetDataTable($@" select 1 from pcba(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and edit_time='{r["EDIT_TIME"].ToString()}'  ");
                                    if (checkpcba.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@" insert into pcba(upload_time,plant,sn,skuno,workorderno,eventpass,station_name,product_status,edit_time) 
                                                                      values(GETDATE(),'{plantstr}','{r["SN"].ToString()}','{r["SKUNO"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["EVENTPASS"].ToString()}','{r["STATION_NAME"].ToString()}','{r["PRODUCT_STATUS"].ToString()}','{r["EDIT_TIME"].ToString()}') ");

                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Send Success");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "N", ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Already Exists");
                                    }
                                }
                                break;
                            case "VANILLA":
                                {
                                    var checkvanilla = CUSTDB.Ado.GetDataTable($@" select 1 from vanilla(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and edit_time='{r["EDIT_TIME"].ToString()}' ");
                                    if (checkvanilla.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@" insert into vanilla(upload_time,plant,sn,skuno,workorderno,eventpass,station_name,product_status,edit_time) 
                                                                          values(GETDATE(),'{plantstr}','{r["SN"].ToString()}','{r["SKUNO"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["EVENTPASS"].ToString()}','{r["STATION_NAME"].ToString()}','{r["PRODUCT_STATUS"].ToString()}','{r["EDIT_TIME"].ToString()}' ) ");

                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Send Success");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "N", ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Already Exists");
                                    }
                                }
                                break;
                            case "CTO":
                                {
                                    var checkvanilla = CUSTDB.Ado.GetDataTable($@" select 1 from cto(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and edit_time='{r["EDIT_TIME"].ToString()}'  ");
                                    if (checkvanilla.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@" insert into cto(upload_time,plant,sn,skuno,workorderno,eventpass,station_name,product_status,edit_time) 
                                                                      values(GETDATE(),'{plantstr}','{r["SN"].ToString()}','{r["SKUNO"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["EVENTPASS"].ToString()}','{r["STATION_NAME"].ToString()}','{r["PRODUCT_STATUS"].ToString()}','{r["EDIT_TIME"].ToString()}') ");

                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Send Success");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "N", ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNDetail", r["WORKORDERNO"].ToString(), r["STATION_NAME"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Already Exists");
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($@"insert sndetail data error, Msg: {ex.Message}");
                }
            }

        }

        public void InsertSNRepair(SqlSugarClient SFCDB, SqlSugarClient CUSTDB, DataTable SNRepair)
        {
            if (SNRepair != null)
            {
                try
                {
                    foreach (DataRow r in SNRepair.Rows)
                    {
                        switch (r["skutype"].ToString())
                        {
                            case "PCBA":
                                {
                                    var checkpcba = CUSTDB.Ado.GetDataTable($@" select 1 from pcba_re(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and FAIL_TIME='{r["FAIL_TIME"].ToString()}' ");
                                    if (checkpcba.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@"   insert into pcba_re(upload_time,plant,sn,workorderno,skuno,fail_station,fail_time,edit_time,fail_code,fail_location,defect_description,symptom_description)
                                                                            values(GETDATE(),'{plantstr}','{r["SN"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["SKUNO"].ToString()}','{r["fail_station"].ToString()}', 
                                                                                    '{r["fail_time"].ToString()}','{r["edit_time"].ToString()}','{r["fail_code"].ToString()}','{r["fail_location"].ToString()}','{r["defect_description"].ToString()}','{r["symptom_description"].ToString()}') ");

                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["fail_time"].ToString(), "Y", "Send Success");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["fail_time"].ToString(), "N", ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        CUSTDB.Ado.ExecuteCommand($@" update pcba_re set edit_time='{r["edit_time"].ToString()}', fail_code='{r["fail_code"].ToString()}', fail_location='{r["fail_location"].ToString()}',defect_description='{r["defect_description"].ToString()}',symptom_description='{r["symptom_description"].ToString()}'
                                                                      where plant='{plantstr}' and sn='{r["SN"].ToString()}' and fail_time='{r["fail_time"].ToString()}' ");

                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Update Success");
                                    }
                                }
                                break;
                            case "VANILLA":
                                {
                                    var checkvanilla = CUSTDB.Ado.GetDataTable($@" select 1 from vanilla_re(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and FAIL_TIME='{r["FAIL_TIME"].ToString()}' ");
                                    if (checkvanilla.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@"   insert into vanilla_re(upload_time,plant,sn,workorderno,skuno,fail_station,fail_time,edit_time,fail_code,fail_location,defect_description,symptom_description)
                                                                            values(GETDATE(),'{plantstr}','{r["SN"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["SKUNO"].ToString()}','{r["fail_station"].ToString()}', 
                                                                                    '{r["fail_time"].ToString()}','{r["edit_time"].ToString()}','{r["fail_code"].ToString()}','{r["fail_location"].ToString()}','{r["defect_description"].ToString()}','{r["symptom_description"].ToString()}') ");

                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["fail_time"].ToString(), "Y", "Send Success");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["fail_time"].ToString(), "N", ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        CUSTDB.Ado.ExecuteCommand($@" update vanilla_re set edit_time='{r["edit_time"].ToString()}', fail_code='{r["fail_code"].ToString()}', fail_location='{r["fail_location"].ToString()}',defect_description='{r["defect_description"].ToString()}',symptom_description='{r["symptom_description"].ToString()}'  
                                                                      where plant='{plantstr}' and sn='{r["SN"].ToString()}' and fail_time='{r["fail_time"].ToString()}' ");

                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Update Success");
                                    }
                                }
                                break;
                            case "CTO":
                                {
                                    var checkpcto = CUSTDB.Ado.GetDataTable($@" select 1 from cto_re(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and FAIL_TIME='{r["FAIL_TIME"].ToString()}' ");
                                    if (checkpcto.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@"   insert into cto_re(upload_time,plant,sn,workorderno,skuno,fail_station,fail_time,edit_time,fail_code,fail_location,defect_description,symptom_description)
                                                                            values(GETDATE(),'{plantstr}','{r["SN"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["SKUNO"].ToString()}','{r["fail_station"].ToString()}', 
                                                                                    '{r["fail_time"].ToString()}' ,'{r["edit_time"].ToString()}','{r["fail_code"].ToString()}','{r["fail_location"].ToString()}','{r["defect_description"].ToString()}','{r["symptom_description"].ToString()}') ");

                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["fail_time"].ToString(), "Y", "Send Success");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["fail_time"].ToString(), "N", ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        CUSTDB.Ado.ExecuteCommand($@" update cto_re set edit_time='{r["edit_time"].ToString()}', fail_code='{r["fail_code"].ToString()}', fail_location='{r["fail_location"].ToString()}',defect_description='{r["defect_description"].ToString()}',symptom_description='{r["symptom_description"].ToString()}'  
                                                                      where plant='{plantstr}' and sn='{r["SN"].ToString()}'  and fail_time='{r["fail_time"].ToString()}'");

                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Update Success");
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($@"insert snrepair data error, Msg: {ex.Message}");
                }
            }

        }

        public void InsertSNLink(SqlSugarClient SFCDB, SqlSugarClient CUSTDB, DataTable SNLink)
        {
            if (SNLink != null)
            {
                try
                {
                    foreach (DataRow r in SNLink.Rows)
                    {
                        var checklink = CUSTDB.Ado.GetDataTable($@" select 1 from link(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and partno='{r["partno"].ToString()}' and location='{r["location"].ToString()}'  ");
                        if (checklink.Rows.Count == 0)
                        {
                            try
                            {
                                CUSTDB.Ado.ExecuteCommand($@"   insert into link(upload_time,plant,sn,value,partno,station,location,edit_time)
                                                                values(GETDATE(),'{plantstr}','{r["SN"].ToString()}','{r["value"].ToString()}','{r["partno"].ToString()}','{r["station"].ToString()}','{r["location"].ToString()}','{r["EDIT_TIME"].ToString()}' ) ");

                                WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", r["location"].ToString(), r["station"].ToString(), r["Edit_time"].ToString(), "Y", "Send Success");
                            }
                            catch (Exception ex)
                            {
                                WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", r["location"].ToString(), r["station"].ToString(), r["Edit_time"].ToString(), "N", ex.Message);
                            }
                        }
                        else
                        {
                            CUSTDB.Ado.ExecuteCommand($@" update link set value='{r["value"].ToString()}',edit_time='{r["Edit_time"].ToString()}' where plant='{plantstr}' and sn='{r["SN"].ToString()}' and partno='{r["partno"].ToString()}' and location='{r["location"].ToString()}'  ");
                            WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", r["location"].ToString(), r["station"].ToString(), r["Edit_time"].ToString(), "Y", "Update Success");
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($@"insert snlink data error, Msg: {ex.Message}");
                }
            }

        }

        public DateTime GetLastRunTime(SqlSugarClient SFCDB)
        {
            try
            {
                var lasttime= SFCDB.Ado.GetDataTable($@" select top 1 time from applog(nolock) where appname='FNNSendUfiSFCData' order by time desc ");
                if (lasttime.Rows.Count == 0)
                {
                    SqlSugarClient LogDB = SFCDB;
                    AppLog log = new AppLog();                    
                    log.AppName = "FNNSendUfiSFCData";
                    log.MsgType = "Log";
                    log.Message = "RunTime";
                    log.Time = LogDB.GetDate();
                    log.AddValue = ip;
                    LogDB.Insertable<AppLog>(log).ExecuteCommand();

                    return SFCDB.GetDate().AddDays(-1);
                }
                else
                {
                    return Convert.ToDateTime(lasttime.Rows[0]["time"].ObjToString("yyyy/MM/dd hh24:mi:ss"));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Get LastRunTime Fail;Fail Msg:{ex.Message}");
            }
        }

        public void WriteApLog(SqlSugarClient SFCDB,DateTime EndTime,string ip)
        {
            try
            {
                SqlSugarClient LogDB = SFCDB;
                AppLog log = new AppLog();
                log.AppName = "FNNSendUfiSFCData";
                log.MsgType = "Log";
                log.Message = "RunTime";
                //log.Time = Convert.ToDateTime(EndTime);
                log.Time = EndTime;
                log.AddValue = ip;
                LogDB.Insertable<AppLog>(log).ExecuteCommand();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Wirte Aplog error;Error Msg:{ex.Message}");
            }
        }

        public void WriteSNLog(SqlSugarClient SFCDB, string SN, string Skutype, string Datatype, string str, string Station_Name, string Edit_Time, string Flag, string Status)
        {
            try
            {
                SqlSugarClient LogDB = SFCDB;
                ServiceLog log = new ServiceLog();
                //log.ID = MesDbBase.GetNewID<R_SN_LOG>(LogDB, "VNDCN");
                log.FunctionType = "FNNSendUFISFCData";
                log.LastEditTime = LogDB.GetDate();
                log.CurrentEditTime = LogDB.GetDate();
                log.NextEditTime = LogDB.GetDate();
                log.Data1 = SN;
                log.Data2 = Skutype;
                log.Data3 = Datatype;
                log.Data4 = str;
                log.Data5 = Station_Name;
                log.Data6 = Edit_Time;
                log.Data7 = Status;
                log.CurrentEditTime = LogDB.GetDate();
                log.Data9 = Flag;
                LogDB.Insertable<ServiceLog>(log).ExecuteCommand();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Wirte SNLog error;Error Msg:{ex.Message}");
            }
        }
    }
}
