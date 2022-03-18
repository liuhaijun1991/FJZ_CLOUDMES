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
    public class FVNSendUFIDataTaskSyn: taskBase
    {
        private bool IsRuning = false;
        private string mesdbstr, custdbstr, plantstr, taskNum, ip;        

        public override void init()
        {
            try
            {
                //apdbstr = ConfigGet("APDB");
                mesdbstr = ConfigGet("MESDB");
                custdbstr = ConfigGet("CUSTDB");
                plantstr = ConfigGet("PLANT");               
                taskNum = ConfigGet("TASK_NUM");
                //csvFilePath = $@"{filepath}";
                Output.UI = new FVNSendUFIDataUI(this);

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
                SqlSugar.SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesdbstr, false);
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
                Run(StartDate, EndDate, taskNum);
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
                SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesdbstr, false);
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
                throw new Exception($@"Class:MES_DCN.Ufispace.SendUfiDataProcess;Function:Run;Error Msg: {ex.Message}");
            }
        }
        
        public DataTable GetSNDetail(SqlSugarClient SFCDB, string collectedStartDateStr, string collectedEndDateStr)
        {
            try
            {


                return SFCDB.Ado.GetDataTable($@" SELECT 
                                                    case when INSTR(UPPER(n.ROUTE_NAME), 'PCBA') > 0 then 'PCBA'
                                                            when INSTR(UPPER(n.ROUTE_NAME), 'VANILLA') > 0 then 'VANILLA'
                                                            when (INSTR(UPPER(n.ROUTE_NAME), 'PACK') > 0 or INSTR(UPPER(n.ROUTE_NAME), 'CTO') > 0) then 'CTO'
                                                    end as skutype,
                                                    m.SN,
                                                    m.SKUNO,
                                                    m.WORKORDERNO,
                                                    CASE WHEN m.REPAIR_FAILED_FLAG = 0 THEN 1 ELSE 0 END AS EVENTPASS,
                                                    m.STATION_NAME,
                                                    m.PRODUCT_STATUS,
                                                    TO_CHAR(m.EDIT_TIME, 'yyyy-mm-dd hh24:mi:ss') EDIT_TIME
                                                from r_sn_station_detail m,
                                                    (SELECT DISTINCT C.SKUNO, d.route_name
                                                        FROM C_CUSTOMER A, C_SERIES B, C_SKU C, C_ROUTE D, R_SKU_ROUTE E
                                                        WHERE A.CUSTOMER_NAME = 'UFI'
                                                        AND A.ID = B.CUSTOMER_ID
                                                        AND B.ID = C.C_SERIES_ID
                                                        AND C.ID = E.SKU_ID
                                                        AND E.ROUTE_ID = D.ID) n
                                                where m.skuno = n.skuno
                                                and exists
                                                (select 1
                                                        from r_sn rsn
                                                        where m.sn = rsn.sn
                                                        and rsn.valid_flag = 1)
                                                and m.EDIT_TIME BETWEEN 
                                                TO_DATE('{collectedStartDateStr}','YYYY-MM-DD HH24:MI:SS') AND
                                                TO_DATE('{collectedEndDateStr}','YYYY-MM-DD HH24:MI:SS') ");

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


                return SFCDB.Ado.GetDataTable($@"   select x.sn, x.value, x.partno, x.station, x.location, TO_CHAR(X.EDIT_TIME, 'yyyy-mm-dd hh24:mi:ss') EDIT_TIME
                                                      from r_sn m, r_sn_kp x
                                                        where exists (select 1
                                                                from (SELECT DISTINCT C.SKUNO
                                                                        FROM C_CUSTOMER  A,
                                                                            C_SERIES    B,
                                                                            C_SKU       C
                                                                        WHERE A.CUSTOMER_NAME = 'UFI'
                                                                        AND A.ID = B.CUSTOMER_ID
                                                                        AND B.ID = C.C_SERIES_ID                                                                        
                                                                   ) n
                                                                where m.skuno = n.skuno)
                                                        and m.valid_flag = 1
                                                        and m.sn = x.sn
                                                        and x.valid_flag = 1
                                                        and x.value is not null
                                                        and x.EDIT_TIME BETWEEN 
                                                        TO_DATE('{collectedStartDateStr}','YYYY-MM-DD HH24:MI:SS') AND
                                                        TO_DATE('{collectedEndDateStr}','YYYY-MM-DD HH24:MI:SS') ");
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


                return SFCDB.Ado.GetDataTable($@" SELECT   CASE WHEN INSTR(UPPER(T.ROUTE_NAME), 'PCBA') > 0 THEN 'PCBA'
                                                                WHEN INSTR(UPPER(T.ROUTE_NAME), 'VANILLA') > 0 THEN 'VANILLA'
                                                                WHEN (INSTR(UPPER(T.ROUTE_NAME), 'PACK') > 0 OR INSTR(UPPER(T.ROUTE_NAME), 'CTO') > 0) THEN 'CTO'
                                                           END AS SKUTYPE,
                                                           F.SN,
                                                           F.WORKORDERNO,
                                                           F.SKUNO,
                                                           F.FAIL_STATION,
                                                           F.FAIL_TIME,
                                                           TO_CHAR(F.EDIT_TIME, 'yyyy-mm-dd hh24:mi:ss') EDIT_TIME,
                                                           G.FAIL_CODE,
                                                           G.FAIL_LOCATION,
                                                           G.DESCRIPTION DEFECT_DESCRIPTION,
                                                           H.DESCRIPTION SYMPTOM_DESCRIPTION
                                                      FROM R_REPAIR_MAIN F
                                                      LEFT JOIN R_REPAIR_FAILCODE G
                                                        ON F.SN = G.SN
                                                       AND F.ID = G.REPAIR_MAIN_ID
                                                      LEFT JOIN R_REPAIR_ACTION H
                                                        ON G.SN = H.SN
                                                       AND H.REPAIR_FAILCODE_ID = G.ID,
                                                     ((SELECT DISTINCT C.SKUNO, D.ROUTE_NAME
                                                               FROM C_CUSTOMER A, C_SERIES B, C_SKU C, C_ROUTE D, R_SKU_ROUTE E
                                                              WHERE A.CUSTOMER_NAME = 'UFI'
                                                                AND A.ID = B.CUSTOMER_ID
                                                                AND B.ID = C.C_SERIES_ID
                                                                AND C.ID = E.SKU_ID
                                                                AND E.ROUTE_ID = D.ID)) T
                                                     WHERE F.SKUNO = T.SKUNO
                                                       AND F.EDIT_TIME BETWEEN
                                                            TO_DATE('{collectedStartDateStr}','YYYY-MM-DD HH24:MI:SS') AND
                                                            TO_DATE('{collectedEndDateStr}','YYYY-MM-DD HH24:MI:SS') ");
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
                                    var checkpcba = CUSTDB.Ado.GetDataTable($@" select 1 from pcba(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),edit_time,121)='{r["EDIT_TIME"].ToString()}'  ");
                                    if (checkpcba.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@" insert into pcba(upload_time,plant,sn,skuno,workorderno,eventpass,station_name,product_status,edit_time) 
                                                                      values(convert(varchar(19),GETDATE(),121),'{plantstr}','{r["SN"].ToString()}','{r["SKUNO"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["EVENTPASS"].ToString()}','{r["STATION_NAME"].ToString()}','{r["PRODUCT_STATUS"].ToString()}','{r["EDIT_TIME"].ToString()}') ");

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
                                    var checkvanilla = CUSTDB.Ado.GetDataTable($@" select 1 from vanilla(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),edit_time,121)='{r["EDIT_TIME"].ToString()}' ");
                                    if (checkvanilla.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@" insert into vanilla(upload_time,plant,sn,skuno,workorderno,eventpass,station_name,product_status,edit_time) 
                                                                          values(convert(varchar(19),GETDATE() ,121),'{plantstr}','{r["SN"].ToString()}','{r["SKUNO"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["EVENTPASS"].ToString()}','{r["STATION_NAME"].ToString()}','{r["PRODUCT_STATUS"].ToString()}','{r["EDIT_TIME"].ToString()}' ) ");

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
                                    var checkvanilla = CUSTDB.Ado.GetDataTable($@" select 1 from cto(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),edit_time,121)='{r["EDIT_TIME"].ToString()}'  ");
                                    if (checkvanilla.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@" insert into cto(upload_time,plant,sn,skuno,workorderno,eventpass,station_name,product_status,edit_time) 
                                                                      values(convert(varchar(19),GETDATE() ,121),'{plantstr}','{r["SN"].ToString()}','{r["SKUNO"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["EVENTPASS"].ToString()}','{r["STATION_NAME"].ToString()}','{r["PRODUCT_STATUS"].ToString()}','{r["EDIT_TIME"].ToString()}') ");

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
                                    var checkpcba = CUSTDB.Ado.GetDataTable($@" select 1 from pcba_re(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),FAIL_TIME,121)='{r["FAIL_TIME"].ToString()}' ");
                                    if (checkpcba.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@"   insert into pcba_re(upload_time,plant,sn,workorderno,skuno,fail_station,fail_time,edit_time,fail_code,fail_location,defect_description,symptom_description)
                                                                            values (convert(varchar(19),GETDATE() ,121),'{plantstr}','{r["SN"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["SKUNO"].ToString()}','{r["fail_station"].ToString()}', 
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
                                                                      where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),fail_time,121)='{r["fail_time"].ToString()}' ");

                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Update Success");
                                    }
                                }
                                break;
                            case "VANILLA":
                                {
                                    var checkvanilla = CUSTDB.Ado.GetDataTable($@" select 1 from vanilla_re(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),FAIL_TIME,121)='{r["FAIL_TIME"].ToString()}' ");
                                    if (checkvanilla.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@"   insert into vanilla_re(upload_time,plant,sn,workorderno,skuno,fail_station,fail_time,edit_time,fail_code,fail_location,defect_description,symptom_description)
                                                                            values (convert(varchar(19),GETDATE() ,121),'{plantstr}','{r["SN"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["SKUNO"].ToString()}','{r["fail_station"].ToString()}', 
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
                                                                      where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),fail_time,121)='{r["fail_time"].ToString()}' ");

                                        WriteSNLog(SFCDB, r["SN"].ToString(), r["SKUTYPE"].ToString(), "SNRepair", r["WORKORDERNO"].ToString(), r["fail_station"].ToString(), r["EDIT_TIME"].ToString(), "Y", "Update Success");
                                    }
                                }
                                break;
                            case "CTO":
                                {
                                    var checkpcto = CUSTDB.Ado.GetDataTable($@" select 1 from cto_re(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and convert(varchar(19),FAIL_TIME,121)='{r["FAIL_TIME"].ToString()}' ");
                                    if (checkpcto.Rows.Count == 0)
                                    {
                                        try
                                        {
                                            CUSTDB.Ado.ExecuteCommand($@"   insert into cto_re(upload_time,plant,sn,workorderno,skuno,fail_station,fail_time,edit_time,fail_code,fail_location,defect_description,symptom_description)
                                                                            values (convert(varchar(19),GETDATE() ,121),'{plantstr}','{r["SN"].ToString()}','{r["WORKORDERNO"].ToString()}','{r["SKUNO"].ToString()}','{r["fail_station"].ToString()}', 
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
                                                                      where plant='{plantstr}' and sn='{r["SN"].ToString()}'  and convert(varchar(19),fail_time,121)='{r["fail_time"].ToString()}'");

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
                        //var checklink = CUSTDB.Ado.GetDataTable($@" select 1 from link(nolock) where plant='{plantstr}' and sn='{r["SN"].ToString()}' and partno='{r["partno"].ToString()}'  ");
                        if (checklink.Rows.Count == 0)
                        {
                            try
                            {
                                CUSTDB.Ado.ExecuteCommand($@"   insert into link(upload_time,plant,sn,value,partno,station,location,edit_time)
                                                                values(convert(varchar(19),GETDATE(),121),'{plantstr}','{r["SN"].ToString()}','{r["value"].ToString()}','{r["partno"].ToString()}','{r["station"].ToString()}','{r["location"].ToString()}','{r["EDIT_TIME"].ToString()}' ) ");

                                WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", r["location"].ToString(), r["station"].ToString(), r["Edit_time"].ToString(), "Y", "Send Success");
                            }
                            catch (Exception ex)
                            {
                                WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", r["location"].ToString(), r["station"].ToString(), r["Edit_time"].ToString(), "N", ex.Message);
                            }
                            //try
                            //{
                            //    CUSTDB.Ado.ExecuteCommand($@"   insert into link(upload_time,plant,sn,value,partno,station,edit_time)
                            //                                    values(convert(varchar(19),GETDATE(),121),'{plantstr}','{r["SN"].ToString()}','{r["value"].ToString()}','{r["partno"].ToString()}','{r["station"].ToString()}','{r["EDIT_TIME"].ToString()}' ) ");

                            //    WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", "", r["station"].ToString(), r["Edit_time"].ToString(), "Y", "Send Success");
                            //}
                            //catch (Exception ex)
                            //{
                            //    WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", "", r["station"].ToString(), r["Edit_time"].ToString(), "N", ex.Message);
                            //}

                        }
                        else
                        {
                            CUSTDB.Ado.ExecuteCommand($@" update link set value='{r["value"].ToString()}', edit_time='{r["Edit_time"].ToString()}' where plant='{plantstr}' and sn='{r["SN"].ToString()}' and partno='{r["partno"].ToString()}' and location='{r["location"].ToString()}'");
                            
                            WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", r["location"].ToString(), r["station"].ToString(), r["Edit_time"].ToString(), "Y", "Update Success");
                            //CUSTDB.Ado.ExecuteCommand($@" update link set value='{r["value"].ToString()}', edit_time='{r["Edit_time"].ToString()}' where plant='{plantstr}' and sn='{r["SN"].ToString()}' and partno='{r["partno"].ToString()}' ");
                            //WriteSNLog(SFCDB, r["SN"].ToString(), "CTO", "SNLink", "", r["station"].ToString(), r["Edit_time"].ToString(), "Y", "Update Success");
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
                var lasttime = SFCDB.Ado.GetDataTable($@" select max(a.edit_time) edit_time from r_mes_log a where a.program_name='MESInterface' and  a.class_name='MESInterface.DCN.FVNSendUFIDataTaskSyn' and a.function_name='WriteApLog' ");
                if (lasttime.Rows.Count == 0)
                {
                    SqlSugarClient LogDB = SFCDB;
                    R_MES_LOG log = new R_MES_LOG();
                    log.ID = MesDbBase.GetNewID<R_SN_LOG>(LogDB, "VNDCN");
                    log.PROGRAM_NAME = "MESInterface";
                    log.CLASS_NAME = "MESInterface.DCN.FVNSendUFIDataTaskSyn";
                    log.FUNCTION_NAME = "WriteApLog";
                    log.LOG_MESSAGE = "RunTime";
                    log.EDIT_EMP = ip;
                    log.EDIT_TIME = LogDB.GetDate();
                    LogDB.Insertable<R_MES_LOG>(log).ExecuteCommand();

                    return SFCDB.GetDate().AddDays(-1);
                }
                else
                {
                    return Convert.ToDateTime(lasttime.Rows[0]["edit_time"].ObjToString("yyyy/MM/dd hh24:mi:ss"));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Get LastRunTime Fail;Fail Msg:{ex.Message}");
            }
        }

        public void WriteApLog(SqlSugarClient SFCDB, DateTime EndTime, string ip)
        {
            try
            {
                SqlSugarClient LogDB = SFCDB;
                R_MES_LOG log = new R_MES_LOG();
                log.ID = MesDbBase.GetNewID<R_SN_LOG>(LogDB, "VNDCN");
                log.PROGRAM_NAME = "MESInterface";
                log.CLASS_NAME = "MESInterface.DCN.FVNSendUFIDataTaskSyn";
                log.FUNCTION_NAME = "WriteApLog";
                log.LOG_MESSAGE = "RunTime";
                log.EDIT_EMP = ip;
                log.EDIT_TIME = EndTime;
                LogDB.Insertable<R_MES_LOG>(log).ExecuteCommand();
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
                R_SN_LOG log = new R_SN_LOG();
                log.ID = MesDbBase.GetNewID<R_SN_LOG>(LogDB, "VNDCN");
                log.SN = SN;
                log.LOGTYPE = "SendUfiSFCDataProcess";
                log.DATA1 = Skutype;
                log.DATA2 = Datatype;
                log.DATA3 = str;
                log.DATA4 = Station_Name;
                log.DATA5 = Edit_Time;
                log.FLAG = Flag;
                log.DATA9 = Status;
                log.CREATETIME = LogDB.GetDate();
                log.CREATEBY = "Interface";
                LogDB.Insertable<R_SN_LOG>(log).ExecuteCommand();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Wirte log error;Error Msg:{ex.Message}");
            }
        }
    }
}
