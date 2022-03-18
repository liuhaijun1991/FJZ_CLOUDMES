using System.Threading;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESPubLab.Common;
using MESStation.LogicObject;
using SqlSugar;
using System.Collections.Generic;
using System.IO;
using MESDataObject.Common;
using MESPubLab.MesBase;
using MES_DCN.Broadcom;
using CsvHelper;
using System.Globalization;
using DbType = SqlSugar.DbType;
using System.Collections;
using MESDataObject.Module.OM;
using MESDataObject.Module.Juniper;
using static MESJuniper.Base.SynAck;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Constants;
using HWDNNSFCBase;
using System;

namespace MESInterface.DCN
{
    public class DcnAgineData : taskBase
    {
        public bool IsRuning = false;
        string dbstr, bustr, filepath, filebackpath;
        OleExec db = null;
        OleExec ldb = null;
        public override void init()
        {
            try
            {
                dbstr = ConfigGet("DB");
                filepath = ConfigGet("FILEPATH");
                filebackpath = ConfigGet("FILEBACKPATH");
            }
            catch (Exception)
            {
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
                MesLog.Info("Start");
                GetDcnAgingData();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }

        void GetDcnAgingData()
        {
            var target = new List<DcnAgingData>();
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("DCN_SUGAR_SQLSERVER", false, DbType.SqlServer))
            {
                //var res = db.Ado.GetDataTable($@"
                //                                select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                //                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                //                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                //                                from mfworkstatus a, mfworkorder b ,mfsysevent c 
                //                                where a.completed='0' 
                //                                and a.workorderno=b.workorderno 
                //                                and c.sysserialno=a.sysserialno 
                //                                AND b.closed=0
                //                                and c.eventname in ('SMTLOADING','SILOADING')
                //                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                //                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                //                                and a.factoryid in ('NBEA')
                //                                AND A.workorderno LIKE '002%'
                //                                AND A.sysserialno NOT LIKE '~%'
                //                                order by REPAIRHELD;
                //                                ");
                var res = db.Ado.GetDataTable($@"
                                                    select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                                                from mfworkstatus a, mfworkorder b ,(select * from (
                                                select *,ROW_NUMBER() over(partition by sysserialno,eventname order by scandatetime desc) as sortnum from mfsysevent where eventname in ('SMTLOADING','SILOADING')   ) cc
                                                where sortnum=1) c 
                                                                                                where a.completed='0' 
                                                                                                and a.workorderno=b.workorderno 
                                                                                                and c.sysserialno=a.sysserialno 
                                                                                                AND b.closed=0
                                                                                               -- and c.eventname in ('SMTLOADING','SILOADING')
                                                                                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                                                                                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                                                                                                and a.factoryid in ('NBEA')
                                                                                                AND A.workorderno LIKE '002%'
                                                                                                AND A.sysserialno NOT LIKE '~%'
                                                                                                order by REPAIRHELD;
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("DCN_ORACLE_SQLSERVER", false, DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"
                                               select distinct B.WorkRouteType ,B.factoryid AS PLANT,CONVERT(CHAR,A.sysserialno) AS SN, 
                                                CONVERT(CHAR,a.repairheld)  REPAIRHELD,B.skuno SKUNO,B.workorderno AS WORKORDERNO,A.currentevent CURRENT_STATION,
                                                A.nextevent NEXT_STATION,  CONVERT(CHAR,DATEDIFF(DD,C.scandatetime,GETDATE())) AS AGING   
                                                from mfworkstatus a, mfworkorder b ,(select * from (
                                                select *,ROW_NUMBER() over(partition by sysserialno,eventname order by scandatetime desc) as sortnum from mfsysevent where eventname in ('SMTLOADING','SILOADING')   ) cc
                                                where sortnum=1) c 
                                                                                                where a.completed='0' 
                                                                                                and a.workorderno=b.workorderno 
	                                                                                            AND b.closed=0
                                                                                                and c.sysserialno=a.sysserialno 
                                                                                               -- and c.eventname in ('SMTLOADING','SILOADING')
                                                                                                and a.currentevent not in ('CBS','SHIPOUT','START','STOCKIN')
                                                                                                and a.nextevent not in ('SHIPFINISH','JOBFINISH')
                                                                                                and a.factoryid in ('NOEA')
                                                                                                AND b.workorderno NOT LIKE '%999%'
                                                                                                AND A.sysserialno NOT LIKE '~%'
                                                                                                order by AGING
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VNODB", false))
            {
                var res = db.Ado.GetDataTable($@"
                                              select distinct decode(B.current_station,
                                                          'SMTLOADING',
                                                          'SMT-PCBA',
                                                          'SILOADING',
                                                          'SI-SYSTEM') AS WorkRouteType,
                                                   c.plant,
                                                    to_char(A.SN) as SN,
                                                   A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                   A.SKUNO,
                                                   A.WORKORDERNO,
                                                   A.CURRENT_STATION,
                                                   A.NEXT_STATION,
                                                   TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                              from r_sn a, r_sn_station_detail b, r_wo_base c
                                             where a.valid_flag = '1'
                                               and a.completed_flag = '0'
                                               and A.sn not like '*%'
                                               and A.sn not like '#%'
                                               and A.sn not like 'del%'
                                               and A.workorderno not like '%TEST%'
                                               AND A.SN NOT LIKE '%TEST%'
                                               and c.closed_flag = '0'
                                               AND A.workorderno LIKE '002%'
                                               and a.sn = b.sn
                                               and b.current_station like '%LOADING'
                                               and a.workorderno = c.workorderno
                                               and a.current_station not in ('STOCKIN')
                                               and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                             order by a.next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("VERTIVODB", false))
            {
                var res = db.Ado.GetDataTable($@"                                            
                                            select *
                                              from (select distinct decode(B.current_station,
                                                                           'SMTLOADING',
                                                                           'SMT-PCBA',
                                                                           'SILOADING',
                                                                           'SI-SYSTEM') AS WorkRouteType,
                                                                    c.plant,
                                                                    to_char(A.SN) as SN,
                                                                    A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                                    A.SKUNO,
                                                                    A.WORKORDERNO,
                                                                    A.CURRENT_STATION,
                                                                    A.NEXT_STATION,
                                                                    TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                                      from r_sn a, r_sn_station_detail b, r_wo_base c
                                                     where a.valid_flag = '1'
                                                       and a.completed_flag = '0'
                                                       and A.sn not like '*%'
                                                       and A.sn not like '#%'
                                                       and A.sn not like 'del%'
                                                       and c.closed_flag = '0'
                                                       and A.workorderno not like '%TEST%'
                                                       AND A.SN NOT LIKE '%TEST%'
                                                       AND A.workorderno LIKE '002%'
                                                       AND WO_TYPE = 'REGULAR'
                                                       and a.sn = b.sn
                                                       and b.current_station like '%LOADING'
                                                       and a.workorderno = c.workorderno
                                                       and a.current_station not in ('STOCKIN')
                                                       and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                                       and not exists (select 1
                                                              from r_sn_lock l
                                                             where l.lock_status = '1'
                                                               and l.sn = a.sn
                                                               and l.lock_reason like '%LLT%')
                                                    --order by a.next_station
                                                    union all
                                                    select distinct decode(B.current_station,
                                                                           'SMTLOADING',
                                                                           'SMT-PCBA',
                                                                           'SILOADING',
                                                                           'SI-SYSTEM') AS WorkRouteType,
                                                                    c.plant,
                                                                    to_char(A.SN) as SN,
                                                                    A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                                    A.SKUNO,
                                                                    A.WORKORDERNO,
                                                                    A.CURRENT_STATION,
                                                                    A.NEXT_STATION,
                                                                    TO_CHAR(ROUND(TO_NUMBER(sysdate - bc.edit_time))) as aging
                                                      from r_sn a,
                                                           (SELECT *
                                                              FROM (SELECT A.*,
                                                                           RANK() OVER(PARTITION BY SN ORDER BY EDIT_TIME DESC) NUMT
                                                                      FROM r_sn_station_detail A
                                                                     WHERE CURRENT_STATION = 'REWORK') BB
                                                             WHERE BB.NUMT = 1) bc,
                                                           r_sn_station_detail B,
                                                           r_wo_base c
                                                     where a.valid_flag = '1'
                                                       and a.completed_flag = '0'
                                                       and A.sn not like '*%'
                                                       and A.sn not like '#%'
                                                       and A.sn not like 'del%'
                                                       and c.closed_flag = '0'
                                                       and A.workorderno not like '%TEST%'
                                                       AND A.SN NOT LIKE '%TEST%'
                                                       AND A.workorderno LIKE '002%'
                                                       AND WO_TYPE = 'REWORK'
                                                       and a.sn = b.sn
                                                       and b.current_station like '%LOADING'
                                                       and a.workorderno = c.workorderno
                                                       and a.current_station not in ('STOCKIN')
                                                       and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                                       and not exists (select 1
                                                              from r_sn_lock l
                                                             where l.lock_status = '1'
                                                               and l.sn = a.sn
                                                               and l.lock_reason like '%LLT%')
                                                       and b.sn = bc.sn)
                                             order by next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }

            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("FVNJNPODB", false))
            {
                var res = db.Ado.GetDataTable($@"
                                             select distinct decode(B.current_station,
                                                          'SMTLOADING',
                                                          'SMT-PCBA',
                                                          'SILOADING',
                                                          'SI-SYSTEM') AS WorkRouteType,
                                                   'VJGS' as plant,
                                                to_char(A.SN) as SN,
                                                   A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                   A.SKUNO,
                                                   A.WORKORDERNO,
                                                   A.CURRENT_STATION,
                                                   A.NEXT_STATION,
                                                   TO_CHAR(ROUND(TO_NUMBER(sysdate - A.START_TIME))) as aging
                                              from r_sn a, r_sn_station_detail b, r_wo_base c
                                             where a.valid_flag = '1'
                                               and a.completed_flag = '0'
                                               and A.sn not like '*%'
                                               and A.sn not like '#%'
                                               and A.sn not like 'del%'
                                               and c.closed_flag = '0'
                                               and A.workorderno not like '%TEST%'
                                               AND A.SN NOT LIKE '%TEST%'
                                               AND A.workorderno LIKE '002%'
                                               and a.sn = b.sn
                                               and b.current_station like '%LOADING'
                                               and a.workorderno = c.workorderno
                                               and a.current_station not in ('STOCKIN')
                                               and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                             order by a.next_station
                                                ");
                target.AddRange(ObjectDataHelper.FromTable<DcnAgingData>(res));
            }

            using (var db = MESDBHelper.OleExec.GetSqlSugarClient("DCN_SUGAR_SQLSERVER", false, DbType.SqlServer))
            {
                var res = db.Ado.UseTran(()=> {
                    db.Deleteable<DcnAgingData>().Where(t=>1==1).ExecuteCommand();
                    db.Insertable(target).ExecuteCommand();
                });
            }

            //var key = DateTime.Now.ToString("yyyyMMddhhmmss");
            //string filename = $@"C:\\Users\\G6001953.NN\\Desktop\\DCNAGING\\{key}.csv";
            ////scfile($@"C:\\Users\\G6001953\\Desktop\\DCNAGING", key, target);
            //ExcelHelp.ExportCsv(target, filename);
        }

    }

    class DcnAgingData
    {
        public string WorkRouteType { get; set; }
        public string PLANT { get; set; }
        public string SN { get; set; }
        public string REPAIRHELD { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string AGING { get; set; }
    }

}
