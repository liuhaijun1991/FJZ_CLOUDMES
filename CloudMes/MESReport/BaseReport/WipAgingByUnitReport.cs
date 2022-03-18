using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESReport.BaseReport
{
    public class WipAgingByUnitReport : ReportBase
    {
        ReportInput inputBU = new ReportInput { Name = "Bu", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "VERTIV", "VNDCN", "VNJUNIPER" } };
        public WipAgingByUnitReport()
        {
            Inputs.Add(inputBU);
        }
        public override void Init()
        {            
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                InitBU(SFCDB);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }
        public override void Run()
        {
            OleExec SFCDB = null;
            try
            {
                string sql = $@"";
                SFCDB = DBPools["SFCDB"].Borrow();

                string Bu = inputBU.Value.ToString().ToUpper().Trim();
                switch (Bu)
                {
                    case "VERTIV":
                        #region VERTIV
                        sql = $@"
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
                         order by next_station";
                        #endregion
                        break;
                    case "VNDCN":
                        #region VNDCN
                        sql = $@"
                                  SELECT DISTINCT DECODE(B.CURRENT_STATION,
                                                        'SMTLOADING',
                                                        'SMT-PCBA',
                                                        'SILOADING',
                                                        'SI-SYSTEM') AS WORKROUTETYPE,
                                                 C.PLANT,
                                                 F.CUSTOMER_NAME,
                                                 TO_CHAR(A.SN) AS SN,
                                                 A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                 A.SKUNO,
                                                 A.WORKORDERNO,
                                                 A.CURRENT_STATION,
                                                 A.NEXT_STATION,
                                                 TO_CHAR(ROUND(TO_NUMBER(SYSDATE - A.START_TIME))) AS AGING
                                   FROM R_SN                A,
                                        R_SN_STATION_DETAIL B,
                                        R_WO_BASE           C,
                                        C_SKU               D,
                                        C_SERIES            E,
                                        C_CUSTOMER          F
                                  WHERE A.VALID_FLAG = '1'
                                    AND A.COMPLETED_FLAG = '0'
                                    AND A.SN NOT LIKE '*%'
                                    AND A.SN NOT LIKE '#%'
                                    AND A.SN NOT LIKE 'del%'
                                    AND A.WORKORDERNO NOT LIKE '%TEST%'
                                    AND A.SN NOT LIKE '%TEST%'
                                    AND C.CLOSED_FLAG = '0'
                                    AND A.WORKORDERNO LIKE '002%'
                                    AND A.SN = B.SN
                                    AND B.CURRENT_STATION LIKE '%LOADING'
                                    AND A.WORKORDERNO = C.WORKORDERNO
                                    AND A.CURRENT_STATION NOT IN ('STOCKIN')
                                    AND A.NEXT_STATION NOT IN ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                    AND A.SKUNO = D.SKUNO
                                    AND D.C_SERIES_ID = E.ID
                                    AND F.ID = E.CUSTOMER_ID
                                  ORDER BY A.NEXT_STATION";
                        #endregion
                        break;
                    case "VNJUNIPER":
                        #region VNJUNIPER
                        sql = $@"
                                SELECT DISTINCT DECODE(B.CURRENT_STATION,
                                               'SMTLOADING',
                                               'SMT-PCBA',
                                               'SILOADING',
                                               'SI-SYSTEM') AS WORKROUTETYPE,
                                        'VJGS' AS PLANT,
                                        F.CUSTOMER_NAME,
                                        TO_CHAR(A.SN) AS SN,
                                        A.REPAIR_FAILED_FLAG REPAIRHELD,
                                        A.SKUNO,
                                        A.WORKORDERNO,
                                        A.CURRENT_STATION,
                                        A.NEXT_STATION,
                                        TO_CHAR(ROUND(TO_NUMBER(SYSDATE - A.START_TIME))) AS AGING
                          FROM R_SN A, R_SN_STATION_DETAIL B, R_WO_BASE C, C_SKU  D,C_SERIES  E, C_CUSTOMER F
                         WHERE A.VALID_FLAG = '1'
                           AND A.COMPLETED_FLAG = '0'
                           AND A.SN NOT LIKE '*%'
                           AND A.SN NOT LIKE '#%'
                           AND A.SN NOT LIKE 'del%'
                           AND C.CLOSED_FLAG = '0'
                           AND A.WORKORDERNO NOT LIKE '%TEST%'
                           AND A.SN NOT LIKE '%TEST%'
                           AND A.WORKORDERNO LIKE '002%'
                           AND A.SN = B.SN
                           AND B.CURRENT_STATION LIKE '%LOADING'
                           AND A.WORKORDERNO = C.WORKORDERNO
                           AND A.CURRENT_STATION NOT IN ('STOCKIN')
                           AND A.NEXT_STATION NOT IN ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                           AND A.SKUNO = D.SKUNO
                           AND D.C_SERIES_ID = E.ID
                           AND F.ID = E.CUSTOMER_ID
                         ORDER BY A.NEXT_STATION";
                        #endregion
                        break;
                    default:
                        //throw new Exception("BU參數錯誤! " + Bu);
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816162450" + Bu));
                }

                RunSqls.Add(sql);
                DataSet res = SFCDB.RunSelect(sql);

                ReportTable retTab = new ReportTable();
                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "Wip  Aging By Unit";
                Outputs.Add(retTab);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }
        public override void DownFile()
        {
            OleExec SFCDB = null;
            try
            {
                string sql = $@"";
                SFCDB = DBPools["SFCDB"].Borrow();

                string Bu = inputBU.Value.ToString().ToUpper().Trim();
                switch (Bu)
                {
                    case "VERTIV":
                        #region VERTIV
                        sql = $@"
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
                            and c.closed_flag = '0'
                            and A.workorderno not like '%TEST%'
                            AND A.SN NOT LIKE '%TEST%'
                            AND A.workorderno LIKE '002%'
                            and a.sn = b.sn
                            and b.current_station like '%LOADING'
                            and a.workorderno = c.workorderno
                            and a.current_station not in ('STOCKIN')
                            and a.next_station not in ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                            and not exists (select 1 from r_sn_lock l where  l.lock_status = '1' and l.sn=a.sn and l.lock_reason  like '%LLT%')
                            order by a.next_station";
                        #endregion
                        break;
                    case "VNDCN":
                        #region VNDCN
                        sql = $@"
                                  SELECT DISTINCT DECODE(B.CURRENT_STATION,
                                                        'SMTLOADING',
                                                        'SMT-PCBA',
                                                        'SILOADING',
                                                        'SI-SYSTEM') AS WORKROUTETYPE,
                                                 C.PLANT,
                                                 F.CUSTOMER_NAME,
                                                 TO_CHAR(A.SN) AS SN,
                                                 A.REPAIR_FAILED_FLAG REPAIRHELD,
                                                 A.SKUNO,
                                                 A.WORKORDERNO,
                                                 A.CURRENT_STATION,
                                                 A.NEXT_STATION,
                                                 TO_CHAR(ROUND(TO_NUMBER(SYSDATE - A.START_TIME))) AS AGING
                                   FROM R_SN                A,
                                        R_SN_STATION_DETAIL B,
                                        R_WO_BASE           C,
                                        C_SKU               D,
                                        C_SERIES            E,
                                        C_CUSTOMER          F
                                  WHERE A.VALID_FLAG = '1'
                                    AND A.COMPLETED_FLAG = '0'
                                    AND A.SN NOT LIKE '*%'
                                    AND A.SN NOT LIKE '#%'
                                    AND A.SN NOT LIKE 'del%'
                                    AND A.WORKORDERNO NOT LIKE '%TEST%'
                                    AND A.SN NOT LIKE '%TEST%'
                                    AND C.CLOSED_FLAG = '0'
                                    AND A.WORKORDERNO LIKE '002%'
                                    AND A.SN = B.SN
                                    AND B.CURRENT_STATION LIKE '%LOADING'
                                    AND A.WORKORDERNO = C.WORKORDERNO
                                    AND A.CURRENT_STATION NOT IN ('STOCKIN')
                                    AND A.NEXT_STATION NOT IN ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                                    AND A.SKUNO = D.SKUNO
                                    AND D.C_SERIES_ID = E.ID
                                    AND F.ID = E.CUSTOMER_ID
                                  ORDER BY A.NEXT_STATION";
                        #endregion
                        break;
                    case "VNJUNIPER":
                        #region VNJUNIPER
                        sql = $@"
                                SELECT DISTINCT DECODE(B.CURRENT_STATION,
                                               'SMTLOADING',
                                               'SMT-PCBA',
                                               'SILOADING',
                                               'SI-SYSTEM') AS WORKROUTETYPE,
                                        'VJGS' AS PLANT,
                                        F.CUSTOMER_NAME,
                                        TO_CHAR(A.SN) AS SN,
                                        A.REPAIR_FAILED_FLAG REPAIRHELD,
                                        A.SKUNO,
                                        A.WORKORDERNO,
                                        A.CURRENT_STATION,
                                        A.NEXT_STATION,
                                        TO_CHAR(ROUND(TO_NUMBER(SYSDATE - A.START_TIME))) AS AGING
                          FROM R_SN A, R_SN_STATION_DETAIL B, R_WO_BASE C, C_SKU  D,C_SERIES  E, C_CUSTOMER F
                         WHERE A.VALID_FLAG = '1'
                           AND A.COMPLETED_FLAG = '0'
                           AND A.SN NOT LIKE '*%'
                           AND A.SN NOT LIKE '#%'
                           AND A.SN NOT LIKE 'del%'
                           AND C.CLOSED_FLAG = '0'
                           AND A.WORKORDERNO NOT LIKE '%TEST%'
                           AND A.SN NOT LIKE '%TEST%'
                           AND A.WORKORDERNO LIKE '002%'
                           AND A.SN = B.SN
                           AND B.CURRENT_STATION LIKE '%LOADING'
                           AND A.WORKORDERNO = C.WORKORDERNO
                           AND A.CURRENT_STATION NOT IN ('STOCKIN')
                           AND A.NEXT_STATION NOT IN ('SHIPFINISH', 'JOBFINISH', 'SHIPOUT')
                           AND A.SKUNO = D.SKUNO
                           AND D.C_SERIES_ID = E.ID
                           AND F.ID = E.CUSTOMER_ID
                         ORDER BY A.NEXT_STATION";
                        #endregion
                        break;
                    default:
                        //throw new Exception("BU參數錯誤! " + Bu);
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816162450" + Bu));
                }

                RunSqls.Add(sql);
                DataTable resDT = SFCDB.RunSelect(sql).Tables[0];
                if (resDT.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }

                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(resDT);
                string fileName = "WipAgingUnitReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }
        public void InitBU(OleExec DB)
        {
            DataTable dt = new DataTable();
            List<string> allBu = new List<string>();
            T_C_BU bu = new T_C_BU(DB, DB_TYPE_ENUM.Oracle);
            dt = bu.GetAllBu(DB);
            foreach (DataRow dr in dt.Rows)
            {
                allBu.Add(dr["BU"].ToString());
            }
            inputBU.ValueForUse = allBu;
            inputBU.Value = allBu[0];
        }
    }
}
