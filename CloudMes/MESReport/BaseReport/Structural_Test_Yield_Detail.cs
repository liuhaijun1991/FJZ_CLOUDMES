using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class Structural_Test_Yield_Detail : ReportBase
    {
        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStationName = new ReportInput() { Name = "StationName", InputType = "TXT", Value = "ALL", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput() { Name = "Type", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSKU = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "Workorderno", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStatus = new ReportInput() { Name = "Status", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };

        public Structural_Test_Yield_Detail()
        {
            Inputs.Add(inputStartDate);
            Inputs.Add(inputEndDate);
            Inputs.Add(inputStationName);
            Inputs.Add(inputType);
            Inputs.Add(inputSKU);
            Inputs.Add(inputWO);
            Inputs.Add(inputStatus);
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            string station = inputStationName.Value.ToString();
            string type = inputType.Value.ToString();
            string wo = inputWO.Value.ToString();
            string skuno = inputSKU.Value.ToString();
            string status = inputStatus.Value.ToString();
            DateTime sTime = Convert.ToDateTime(inputStartDate.Value);
            DateTime eTime = Convert.ToDateTime(inputEndDate.Value);
            string sValue = sTime.ToString("yyyy-MM-dd");
            string eValue = eTime.ToString("yyyy-MM-dd");
            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;

            string StrTimeSQL = $@"";

            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            try
            {
                if (sValue != null && eValue != null)
                {
                    StrTimeSQL = $@"  AND RTJ.TATIME> TO_DATE('{sValue}', 'yyyy/MM/dd') AND RTJ.TATIME<TO_DATE('{eValue}','yyyy/MM/dd')";
                }
                if (status.Contains("ALL"))
                {
                    if (type == "First_Pass_Yield")
                    {

                        if (wo == "")
                        {
                            sqlRun = $@"select SS.SKUNO,SS.SYSSERIALNO,SS.MES_STATION STATION,SS.STATUS,SS.TATIME  from (
                                    SELECT ROW_NUMBER() OVER(PARTITION BY RTJ.SYSSERIALNO,EVENTNAME ORDER BY RTJ.TATIME) RN, CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION,RTJ.STATUS,RTJ.TATIME 
                                    FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                                    WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                    AND RTJ.EVENTNAME=CTS.TE_STATION 
                                    AND CSKU.ID=RSR.SKU_ID
                                    AND CRD.STATION_NAME=CTS.MES_STATION
                                    AND RTJ.SYSSERIALNO=RSN.SN
                                    /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                    AND CSKU.SKUNO = '{skuno}' 
                                    AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')
                                    " + StrTimeSQL + $@")SS WHERE SS.RN=1 ORDER BY SS.SYSSERIALNO,SS.TATIME";
                        }
                        else
                        {
                            sqlRun = $@"select SS.SKUNO,SS.WORKORDERNO,SS.SYSSERIALNO,SS.MES_STATION STATION,SS.STATUS,SS.TATIME from (
                                    SELECT ROW_NUMBER() OVER(PARTITION BY RTJ.SYSSERIALNO,EVENTNAME ORDER BY RTJ.TATIME) RN, CSKU.SKUNO,RWO.WORKORDERNO,RTJ.SYSSERIALNO,CTS.MES_STATION,RTJ.STATUS,RTJ.TATIME 
                                    FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_WO_BASE RWO,R_SN RSN
                                    WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                    AND RTJ.EVENTNAME=CTS.TE_STATION 
                                    AND RTJ.SYSSERIALNO=RSN.SN
                                    AND CSKU.ID=RSR.SKU_ID
                                    AND CRD.STATION_NAME=CTS.MES_STATION
                                    AND RWO.WORKORDERNO=RSN.WORKORDERNO
                                    AND RWO.SKUNO=CSKU.SKUNO
                                    /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                    AND RWO.WORKORDERNO = '{wo}' 
                                    AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL + $@")SS 
                                    WHERE SS.RN=1 ORDER BY SS.SYSSERIALNO,SS.TATIME";
                        }
                    }
                    else if (type == "Pass_Yield")
                    {
                        if (wo == "")
                        {
                            sqlRun = $@"SELECT DISTINCT JJ.SKUNO, JJ.SYSSERIALNO,JJ.MES_STATION STATION,AA.STATUS,JJ.TATIME FROM (
                                        SELECT  CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION,MAX(TATIME) TATIME
                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                        AND CSKU.ID=RSR.SKU_ID
                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                        AND RTJ.SYSSERIALNO=RSN.SN
                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                        AND CSKU.SKUNO = '{skuno}' 
                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                        + $@"GROUP BY RTJ.SYSSERIALNO,CSKU.SKUNO,CTS.MES_STATION)JJ 
                                        LEFT JOIN R_TEST_JUNIPER AA ON JJ.SYSSERIALNO=AA.SYSSERIALNO 
                                        AND JJ.TATIME=AA.TATIME AND AA.EVENTNAME IN((SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}'))
                                        /*AND UPPER(AA.STATUS) IN('PASS','FAIL')*/ ";
                        }
                        else
                        {
                            sqlRun = $@"SELECT DISTINCT JJ.SKUNO,JJ.WORKORDERNO, JJ.SYSSERIALNO,JJ.MES_STATION STATION,AA.STATUS,JJ.TATIME FROM (
                                        SELECT  CSKU.SKUNO,RWO.WORKORDERNO,RTJ.SYSSERIALNO,CTS.MES_STATION,MAX(TATIME) TATIME
                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_WO_BASE RWO,R_SN RSN
                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                        AND RTJ.SYSSERIALNO=RSN.SN
                                        AND CSKU.ID=RSR.SKU_ID
                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                        AND RWO.WORKORDERNO=RSN.WORKORDERNO
                                        AND RWO.SKUNO=CSKU.SKUNO
                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                        AND RWO.WORKORDERNO = '{wo}' 
                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                        + $@"GROUP BY RTJ.SYSSERIALNO,CSKU.SKUNO,RWO.WORKORDERNO,CTS.MES_STATION)JJ 
                                        LEFT JOIN R_TEST_JUNIPER AA ON JJ.SYSSERIALNO=AA.SYSSERIALNO 
                                        AND JJ.TATIME=AA.TATIME AND AA.EVENTNAME IN((SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}'))
                                        /*AND UPPER(AA.STATUS) IN('PASS','FAIL')*/ ";
                        }
                    }
                    else if (type == "True_Failure_Yield")
                    {
                        if (wo == "")
                        {
                            sqlRun = $@"SELECT DISTINCT SS.SKUNO,SS.SYSSERIALNO,SS.STATION,SS.STATUS,SS.TATIME FROM (
                                                            SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN ,r_repair_main rrm
                                                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                            AND RTJ.EVENTNAME=CTS.TE_STATION
                                                            AND RTJ.SYSSERIALNO=RSN.SN
                                                            AND CSKU.ID=RSR.SKU_ID
                                                            AND CRD.STATION_NAME=CTS.MES_STATION
                                                            AND RTJ.SYSSERIALNO=RRM.SN
                                                            AND CTS.MES_STATION=RRM.FAIL_STATION
                                                            /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                                            AND CSKU.SKUNO='{skuno}' 
                                                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                            + $@" )SS WHERE SS.RN=1";
                        }
                        else
                        {
                            sqlRun = $@"SELECT DISTINCT SS.SKUNO,SS.WORKORDERNO,SS.SYSSERIALNO,SS.STATION,SS.STATUS,SS.TATIME FROM (
                                                            SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO, RWO.WORKORDERNO, RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN ,r_repair_main rrm
                                                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                            AND RTJ.EVENTNAME=CTS.TE_STATION
                                                            AND RTJ.SYSSERIALNO=RSN.SN
                                                            AND CSKU.ID=RSR.SKU_ID
                                                            AND CRD.STATION_NAME=CTS.MES_STATION
                                                            AND RTJ.SYSSERIALNO=RRM.SN
                                                            AND CTS.MES_STATION=RRM.FAIL_STATION
                                                            /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                                            AND RWO.WORKORDERNO='{wo}' 
                                                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                            + $@" )SS WHERE SS.RN=1 ";
                        }
                    }
                }
                else
                {
                    if (type == "First_Pass_Yield")
                    {
                        if (wo == "")
                        {
                            sqlRun = $@"SELECT SS.SKUNO,SS.SYSSERIALNO,SS.STATION,SS.STATUS,SS.TATIME FROM (
                                        SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                        AND CSKU.ID=RSR.SKU_ID
                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                        AND RTJ.SYSSERIALNO=RSN.SN
                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                        AND CSKU.SKUNO='{skuno}' 
                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                        + $@" )SS,
                                                C_TEMES_STATUS_MAPPING BB
                                            WHERE SS.RN = 1
                                            AND UPPER(SS.STATUS) = UPPER(BB.TE_STATUS)
                                            and UPPER(BB.MES_STATUS) = 'FAIL'";
                        }
                        else
                        {
                            sqlRun = $@"SELECT SS.SKUNO,SS.WORKORDERNO,SS.SYSSERIALNO,SS.STATION,SS.STATUS,SS.TATIME FROM (
                                        SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RWO.WORKORDERNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_WO_BASE RWO,R_SN RSN
                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                        AND RTJ.SYSSERIALNO=RSN.SN
                                        AND CSKU.ID=RSR.SKU_ID
                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                        AND RWO.WORKORDERNO=RSN.WORKORDERNO
                                        AND RWO.SKUNO=CSKU.SKUNO
                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                        AND RWO.WORKORDERNO='{wo}' 
                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                        + $@" )SS,
                                               C_TEMES_STATUS_MAPPING BB
                                         WHERE SS.RN = 1
                                           AND UPPER(SS.STATUS) = UPPER(BB.TE_STATUS)
                                           and UPPER(BB.MES_STATUS) = 'FAIL'";
                        }
                    }
                    else if (type == "Pass_Yield")
                    {
                        if (wo == "")
                        {
                            sqlRun = $@"SELECT * FROM (
                                                        SELECT DISTINCT JJ.SKUNO, JJ.SYSSERIALNO,JJ.MES_STATION STATION,AA.STATUS,JJ.TATIME FROM (
                                                        SELECT  CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION,MAX(TATIME) TATIME
                                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                                        AND CSKU.ID=RSR.SKU_ID
                                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                                        AND RTJ.SYSSERIALNO=RSN.SN
                                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                                        AND CSKU.SKUNO = '{skuno}' 
                                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                                        + $@" GROUP BY RTJ.SYSSERIALNO,CSKU.SKUNO,CTS.MES_STATION)JJ
                                                        LEFT JOIN R_TEST_JUNIPER AA ON JJ.SYSSERIALNO=AA.SYSSERIALNO AND JJ.TATIME=AA.TATIME 
                                                        AND AA.EVENTNAME IN((SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')),
                                                            C_TEMES_STATUS_MAPPING BB
                                                            WHERE UPPER(AA.STATUS) = UPPER(BB.TE_STATUS)
                                                            AND UPPER(BB.MES_STATUS) = 'FAIL'
                                                        ) GG WHERE GG.STATUS IS NOT NULL";
                        }
                        else
                        {
                            sqlRun = $@"SELECT * FROM (
                                                        SELECT DISTINCT JJ.SKUNO,JJ.WORKORDERNO, JJ.SYSSERIALNO,JJ.MES_STATION STATION,AA.STATUS,JJ.TATIME FROM (
                                                        SELECT  CSKU.SKUNO,RWO.WORKORDERNO,RTJ.SYSSERIALNO,CTS.MES_STATION,MAX(TATIME) TATIME
                                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_WO_BASE RWO,R_SN RSN
                                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                                        AND RTJ.SYSSERIALNO=RSN.SN
                                                        AND CSKU.ID=RSR.SKU_ID
                                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                                        AND RWO.WORKORDERNO=RSN.WORKORDERNO
                                                        AND RWO.SKUNO=CSKU.SKUNO
                                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                                        AND RWO.WORKORDERNO = '{wo}' 
                                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                                        + $@" GROUP BY RTJ.SYSSERIALNO,CSKU.SKUNO,RWO.WORKORDERNO,CTS.MES_STATION)JJ
                                                                LEFT JOIN R_TEST_JUNIPER AA ON JJ.SYSSERIALNO=AA.SYSSERIALNO AND JJ.TATIME=AA.TATIME 
                                                                AND AA.EVENTNAME IN((SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')),
                                                            C_TEMES_STATUS_MAPPING BB
                                                            WHERE UPPER(AA.STATUS) = UPPER(BB.TE_STATUS)
                                                            AND UPPER(BB.MES_STATUS) = 'FAIL'
                                                        ) GG WHERE GG.STATUS IS NOT NULL";
                        }
                    }
                    else if (type == "True_Failure_Yield")
                    {
                        if (wo == "")
                        {
                            sqlRun = $@"SELECT DISTINCT SS.SKUNO,SS.SYSSERIALNO,SS.STATION,SS.STATUS,SS.TATIME FROM (
                                                            SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN ,r_repair_main rrm
                                                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                            AND RTJ.EVENTNAME=CTS.TE_STATION
                                                            AND RTJ.SYSSERIALNO=RSN.SN
                                                            AND CSKU.ID=RSR.SKU_ID
                                                            AND CRD.STATION_NAME=CTS.MES_STATION
                                                            AND RTJ.SYSSERIALNO=RRM.SN
                                                            AND CTS.MES_STATION=RRM.FAIL_STATION
                                                            /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                                            AND CSKU.SKUNO='{skuno}' 
                                                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                            + $@" )SS ,
                                                   C_TEMES_STATUS_MAPPING BB
                                             WHERE SS.RN = 1
                                               AND UPPER(SS.STATUS) = UPPER(BB.TE_STATUS)
                                               and UPPER(BB.MES_STATUS) = 'FAIL'";
                        }
                        else
                        {
                            sqlRun = $@"SELECT DISTINCT SS.SKUNO,SS.WORKORDERNO,SS.SYSSERIALNO,SS.STATION,SS.STATUS,SS.TATIME FROM (
                                                            SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO, RWO.WORKORDERNO, RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN ,r_repair_main rrm
                                                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                            AND RTJ.EVENTNAME=CTS.TE_STATION
                                                            AND RTJ.SYSSERIALNO=RSN.SN
                                                            AND CSKU.ID=RSR.SKU_ID
                                                            AND CRD.STATION_NAME=CTS.MES_STATION
                                                            AND RTJ.SYSSERIALNO=RRM.SN
                                                            AND CTS.MES_STATION=RRM.FAIL_STATION
                                                            /* AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                                            AND RWO.WORKORDERNO='{wo}' 
                                                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{station}')" + StrTimeSQL
                                            + $@" )SS,
                                                C_TEMES_STATUS_MAPPING BB
                                            WHERE SS.RN = 1
                                            AND UPPER(SS.STATUS) = UPPER(BB.TE_STATUS)
                                            and UPPER(BB.MES_STATUS) = 'FAIL'";
                        }
                    }

                }
                RunSqls.Add(sqlRun);
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("SYSSERIALNO");
                linkTable.Columns.Add("STATION");
                linkTable.Columns.Add("STATUS");
                linkTable.Columns.Add("TATIME");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SKUNO"] = "";
                    linkRow["WORKORDERNO"] = "";
                    linkRow["SYSSERIALNO"] = "";
                    linkRow["STATION"] = "";
                    linkRow["STATUS"] = "";
                    linkRow["TATIME"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(snListTable, linkTable);
                reportTable.Tittle = "SNList";
                //reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);

            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
