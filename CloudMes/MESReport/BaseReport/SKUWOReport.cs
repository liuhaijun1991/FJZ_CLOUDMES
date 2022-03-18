using MESDBHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MESReport.BaseReport
{
    public class SKUWOReport : ReportBase
    {
        ReportInput inputSku = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput { Name = "TYPE", InputType = "Select", Value = "PCBA", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "PCBA", "MODEL", "CTO", "OPTICS", "DOF", "VIRTUAL" } };
        [JsonIgnore]
        [ScriptIgnore]
        public OleExec SFCDB = null;//DBPools["SFCDB"].Borrow();
        public SKUWOReport()
        {
            this.Inputs.Add(inputSku);
            this.Inputs.Add(inputType);
        }
        public override void Run()
        {
            string sqlSku = "";
            string sqlRoute = "";
            string sqlsku1 = "";
            if (inputSku.Value == null || inputSku.Value.ToString() == "")
            {
                sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, trunc ( sysdate - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w WHERE 1=1  ";
            }
            else
            {
                sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, trunc ( sysdate - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w 
                            where skuno='{inputSku.Value.ToString().Trim().ToUpper()}'";
                sqlsku1 = $@" AND SKUNO ='{inputSku.Value}'";
            }

            if (inputType.Value != null && inputType.Value.ToString() != "")
            {
                sqlSku += $@" AND EXISTS (SELECT *
                                  FROM C_SKU S
                                 WHERE W.SKUNO = S.SKUNO
                                   AND S.SKU_TYPE = '{inputType.Value}')";
                sqlRoute = $@"SELECT *
                                  FROM (SELECT a.*,
                                               ROW_NUMBER() OVER(PARTITION BY STATION_NAME, SKU_TYPE ORDER BY SEQ_NO DESC) numbs
                                          FROM (SELECT DISTINCT CASE
                                                                  WHEN R.STATION_NAME LIKE '%LOADING%' THEN
                                                                   'LOADING'
                                                                  ELSE R.STATION_NAME
                                                                END STATION_NAME,
                                                                R.SEQ_NO,
                                                                S.SKU_TYPE
                                                  FROM C_SKU S, C_ROUTE_DETAIL R, R_SKU_ROUTE SR
                                                 WHERE S.ID = SR.SKU_ID
                                                   {sqlsku1}
                                                   AND R.ROUTE_ID = SR.ROUTE_ID
                                                   AND S.SKU_TYPE = '{inputType.Value}'
                                                ) a)
                                 WHERE numbs = 1
                                 ORDER BY SKU_TYPE, SEQ_NO";
            }

            var wolinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&CloseFlag=N&WO=";
            var snlinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListReport&RunFlag=1";

            bool isborrow = false;

            if (SFCDB == null)
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                isborrow = true;
            }

            try
            {
                RunSqls.Add(sqlSku);
                RunSqls.Add(sqlRoute);
                DataTable dtWO = SFCDB.RunSelect(sqlSku).Tables[0];
                DataTable dtRoute = SFCDB.RunSelect(sqlRoute).Tables[0];
                if (dtWO.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                DataTable reportTable = new DataTable();
                DataTable linkTable = new DataTable();
                reportTable.Columns.Add("WORKORDERNO");
                reportTable.Columns.Add("CLOSED_FLAG");
                reportTable.Columns.Add("SKUNO");
                reportTable.Columns.Add("DAYS");
                reportTable.Columns.Add("VER");
                reportTable.Columns.Add("QTY", typeof(int));
                reportTable.Columns.Add("BALANCE", typeof(int));
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("CLOSED_FLAG");
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("DAYS");
                linkTable.Columns.Add("VER");
                linkTable.Columns.Add("QTY");
                linkTable.Columns.Add("BALANCE");
                for (int i = 0; i < dtRoute.Rows.Count; i++)
                {
                    if (dtRoute.Rows[i]["STATION_NAME"].ToString()=="FPCTEST"|| dtRoute.Rows[i]["STATION_NAME"].ToString() == "PICTEST")
                    {
                        if (!reportTable.Columns.Contains("FCT"))
                        {
                            reportTable.Columns.Add("FCT", typeof(int));
                            linkTable.Columns.Add("FCT");
                        }
                    }
                    else
                    {
                        reportTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString(), typeof(int));
                        linkTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString());
                    }
                }
                reportTable.Columns.Add("FailWip", typeof(int));
                reportTable.Columns.Add("RepairWip", typeof(int));
                reportTable.Columns.Add("MRB", typeof(int));
                reportTable.Columns.Add("REWORK", typeof(int));
                reportTable.Columns.Add("ORT", typeof(int));
                reportTable.Columns.Add("WHS", typeof(int));
                reportTable.Columns.Add("SHIPPED", typeof(int));
                linkTable.Columns.Add("FailWip");
                linkTable.Columns.Add("RepairWip");
                linkTable.Columns.Add("MRB");
                linkTable.Columns.Add("REWORK");
                linkTable.Columns.Add("ORT");
                linkTable.Columns.Add("WHS");
                linkTable.Columns.Add("SHIPPED");
                DataRow reportRow;
                DataRow linkDataRow;
                string wo;
                string sqlFailCount;
                string sqlRepairCount;
                string sqlMrbCount;
                string sqlOrtCount;
                string workorderQty;

                string skusql = "";
                if (inputSku.Value != null && inputSku.Value.ToString() != "" && inputSku.Value.ToString() != "ALL")
                {
                    skusql = $@" AND WW.SKUNO='{inputSku.Value.ToString().Trim().ToUpper()}'";
                }

                string sqlSNCount = $@"SELECT W.WORKORDERNO, R.STATION_NAME NEXT_STATION, COUNT(S.SN) QTY
                                          FROM (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE 1 = 1 
                                                   {skusql}
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{inputType.Value}')) W
                                          LEFT JOIN C_ROUTE_DETAIL R
                                            ON R.ROUTE_ID = W.ROUTE_ID
                                          LEFT JOIN R_SN S
                                            ON W.WORKORDERNO = S.WORKORDERNO
                                           AND R.STATION_NAME = S.NEXT_STATION
                                           AND S.REPAIR_FAILED_FLAG = 0
                                         GROUP BY W.WORKORDERNO, R.STATION_NAME";

                var dtSNCount = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlSNCount).ToList();

                sqlFailCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                                      FROM R_REPAIR_MAIN RM
                                     WHERE NOT EXISTS
                                     (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                                       AND NOT EXISTS
                                     (SELECT *
                                              FROM R_MRB R
                                             WHERE RM.SN = R.SN
                                               AND R.WORKORDERNO = RM.WORKORDERNO)
                                       AND RM.SN IN
                                           (SELECT SN
                                              FROM R_SN S
                                             WHERE S.REPAIR_FAILED_FLAG = 1
                                               AND EXISTS
                                             (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}'))
        
                                            )
                                       AND RM.CLOSED_FLAG = 0
                                     GROUP BY WORKORDERNO";
                var failcounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlFailCount);

                sqlRepairCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                                      FROM R_REPAIR_MAIN RM
                                     WHERE EXISTS
                                     (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                                       AND NOT EXISTS
                                     (SELECT *
                                              FROM R_MRB R
                                             WHERE RM.SN = R.SN
                                               AND R.WORKORDERNO = RM.WORKORDERNO)
                                       AND RM.SN IN
                                           (SELECT SN
                                              FROM R_SN S
                                             WHERE S.REPAIR_FAILED_FLAG = 1
                                               AND EXISTS
                                             (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}')))
                                       AND RM.CLOSED_FLAG = 0
                                     GROUP BY WORKORDERNO";
                var repairCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlRepairCount);

                sqlMrbCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                                  FROM R_MRB B
                                 WHERE EXISTS (SELECT *
                                          FROM R_WO_BASE WW
                                         WHERE WW.WORKORDERNO = B.WORKORDERNO 
                                         {skusql}
                                           AND EXISTS (SELECT *
                                                  FROM C_SKU K
                                                 WHERE WW.SKUNO = K.SKUNO
                                                   AND K.SKU_TYPE = '{inputType.Value}'))
                                   AND REWORK_WO IS NULL
                                 GROUP BY WORKORDERNO";
                var mrbCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlMrbCount);

                string sqlReworkCount = $@"SELECT WORKORDERNO, COUNT(NEXT_STATION) QTY
                                              FROM R_SN S
                                             WHERE NEXT_STATION = 'REWORK'
                                               AND EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}'))
                                               AND NOT EXISTS (SELECT *
                                                      FROM R_MRB B
                                                     WHERE S.SN = B.SN
                                                       AND B.REWORK_WO IS NULL
                                                       AND B.WORKORDERNO = S.WORKORDERNO)
                                             GROUP BY WORKORDERNO";
                var reworkCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlReworkCount);

                sqlOrtCount = $@"SELECT S.WORKORDERNO,COUNT(1) QTY
                                      FROM R_LOT_DETAIL A, R_LOT_STATUS B, R_SN S
                                     WHERE B.ID = A.LOT_ID
                                       AND A.SN = S.SN
                                       AND B.SAMPLE_STATION IN ('ORT', 'ORT-FT2')
                                       AND S.VALID_FLAG = 1
                                       AND NOT EXISTS
                                     (SELECT A2.SN
                                              FROM R_TEST_RECORD A2
                                             WHERE A2.SN = A.SN
                                               AND A2.TESTATION IN ('ORT', 'ORT-FT2')
                                               AND A2.STATE = 'PASS'
                                               AND A2.ENDTIME > (SELECT MAX(CREATE_DATE)
                                                                   FROM R_LOT_DETAIL A3
                                                                  WHERE A3.SN = A.SN))
                                       AND B.SKUNO = S.WORKORDERNO
                                       AND EXISTS (SELECT *
                                              FROM R_WO_BASE WW
                                             WHERE WW.WORKORDERNO = S.WORKORDERNO
                                               {skusql}
                                               AND EXISTS (SELECT *
                                                      FROM C_SKU K
                                                     WHERE WW.SKUNO = K.SKUNO
                                                       AND K.SKU_TYPE = '{inputType.Value}'))
                                       AND S.SHIPPED_FLAG = 0
                                    GROUP BY S.WORKORDERNO";
                var ortCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlOrtCount);

                string sqlLoadingCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                              FROM R_SN S
                                             WHERE EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}'))
                                             GROUP BY WORKORDERNO";
                var loadingCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlLoadingCount);

                string sqlWHSCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                   {skusql}
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{inputType.Value}'))
                                           AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.NEXT_STATION <> 'REWORK'
                                         GROUP BY WORKORDERNO";
                var WHSCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlWHSCount);

                string sqlScraped = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                   {skusql}
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{inputType.Value}'))
                                           AND S.SCRAPED_FLAG='1'
                                           AND S.VALID_FLAG='1'
                                         GROUP BY WORKORDERNO";
                var dtScraped = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlScraped);

                string sqlShippedCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                              FROM R_SN S
                                             WHERE EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}'))
                                               AND S.SHIPPED_FLAG = 1
                                             GROUP BY WORKORDERNO";
                var ShippedCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlShippedCount);
                foreach (DataRow row in dtWO.Rows)
                {
                    try
                    {
                        //loadingNum = 0;
                        //mrbNum = 0;
                        //repairNum = 0;
                        wo = row["WORKORDERNO"] == null ? "" : row["WORKORDERNO"].ToString();
                        workorderQty = row["WORKORDER_QTY"] == null ? "0" : row["WORKORDER_QTY"].ToString();
                        reportRow = reportTable.NewRow();
                        linkDataRow = linkTable.NewRow();
                        reportRow["WORKORDERNO"] = wo;
                        reportRow["CLOSED_FLAG"] = row["CLOSED_FLAG"] == null ? "" : row["CLOSED_FLAG"].ToString(); ;
                        linkDataRow["WORKORDERNO"] = wolinkURL + wo;
                        reportRow["SKUNO"] = row["SKUNO"] == null ? "" : row["SKUNO"].ToString();
                        reportRow["DAYS"] = row["DAYS"] == null ? "" : row["DAYS"].ToString();
                        reportRow["VER"] = row["SKU_VER"] == null ? "N/A" : row["SKU_VER"].ToString();
                        reportRow["QTY"] = workorderQty;
                        #region STATION WIP
                        var data = dtSNCount.FindAll(t => t.WORKORDERNO == wo);
                        for (int i = 0; i < data.Count; i++)
                        {
                            try
                            {
                                if (data[i].NEXT_STATION.Contains("LOADING"))
                                {
                                    reportRow["LOADING"] = data[i].QTY;
                                }
                                else
                                {
                                    if (data[i].NEXT_STATION == "FPCTEST" || data[i].NEXT_STATION == "PICTEST")
                                    {
                                        int t = 0;
                                        int.TryParse(reportRow["FCT"].ToString(), out t);
                                        reportRow["FCT"] = t + data[i].QTY;
                                        linkDataRow["FCT"] = snlinkURL + "&WO=" + wo + "&STATION=" + data[i].NEXT_STATION + "&STATUS=PROD";
                                    }
                                    else
                                    {
                                        reportRow[data[i].NEXT_STATION] = data[i].QTY;
                                        linkDataRow[data[i].NEXT_STATION] = snlinkURL + "&WO=" + wo + "&STATION=" + data[i].NEXT_STATION + "&STATUS=PROD";
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        #endregion

                        #region Fail WIP
                        var failcount = failcounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["FailWip"] = (failcount == null ? 0 : failcount.QTY);
                        linkDataRow["FailWip"] = snlinkURL + "&WO=" + wo + "&STATUS=FAIL";
                        #endregion

                        #region Repiar WIP
                        var repairCount = repairCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["RepairWip"] = repairCount == null ? 0 : repairCount.QTY;
                        linkDataRow["RepairWip"] = snlinkURL + "&WO=" + wo + "&STATUS=REPAIR";
                        #endregion

                        #region MRB & REWORK
                        var mrbCount = mrbCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["MRB"] = mrbCount == null ? 0 : mrbCount.QTY;
                        linkDataRow["MRB"] = snlinkURL + "&WO=" + wo + "&STATUS=MRB";

                        var reworkCount = reworkCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["REWORK"] = reworkCount == null ? 0 : reworkCount.QTY;
                        linkDataRow["REWORK"] = snlinkURL + "&WO=" + wo + "&STATUS=REWORK";

                        Double reworkSum = reworkCount == null ? 0 : reworkCount.QTY;
                        try
                        {
                            reworkSum = Convert.ToInt64(reworkSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region ORT數據匯總
                        var ortCount = ortCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["ORT"] = ortCount == null ? 0 : ortCount.QTY;
                        #endregion

                        #region WHS
                        var WHSCount = WHSCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["WHS"] = WHSCount == null ? 0 : WHSCount.QTY;
                        linkDataRow["WHS"] = snlinkURL + "&WO=" + wo + "&STATUS=FG";
                        #endregion


                        #region Scraped
                        var ScrapedCount = dtScraped.Find(t => t.WORKORDERNO == wo);
                        double ScrapedSum = ScrapedCount == null ? 0 : ScrapedCount.QTY;
                        try
                        {
                            ScrapedSum = Convert.ToInt64(ScrapedSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region Shipped
                        var ShippedCount = ShippedCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["SHIPPED"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                        linkDataRow["SHIPPED"] = snlinkURL + "&WO=" + wo + "&STATUS=SHIPPED";

                        double ShippedSum = ShippedCount == null ? 0 : ShippedCount.QTY;
                        try
                        {
                            ShippedSum = Convert.ToInt64(ShippedSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion
                        var loadingCount = loadingCounts.Find(t => t.WORKORDERNO == wo);

                        reportRow["LOADING"] = Convert.ToInt64(workorderQty) - (loadingCount == null ? 0 : loadingCount.QTY);

                        Double WoSum = row["WORKORDER_QTY"] == null ? 0 : Convert.ToInt64(row["WORKORDER_QTY"].ToString());
                        reportRow["BALANCE"] = WoSum - ScrapedSum - reworkSum - ShippedSum;
                        reportTable.Rows.Add(reportRow);
                        linkTable.Rows.Add(linkDataRow);
                    }
                    catch (Exception ee)
                    {

                    }
                }
                DataRow dr = reportTable.NewRow();
                DataRow drl = linkTable.NewRow();
                dr["VER"] = "Total:";
                foreach (var c in reportTable.Columns)
                {
                    if (c.ToString() != "WORKORDERNO" && c.ToString() != "CLOSED_FLAG" && c.ToString() != "SKUNO" && c.ToString() != "DAYS" && c.ToString() != "VER")
                    {
                        //工站名帶"-"字符的Compute()方法會報錯，於是加個判斷換個寫法 Edit By ZHB 2020年7月10日13:30:49
                        //dr[c.ToString()] = reportTable.Compute("Sum(" + c.ToString() + ")", "");
                        var o = 0;
                        var res = true;
                        var str = c.ToString().Substring(0, 1);
                        if (!int.TryParse(str, out o))
                        {
                            res = false;
                        }
                        if (!reportTable.Columns.Contains(c.ToString()))
                        {
                            continue;
                        }

                        if (!c.ToString().Contains("-") && !c.ToString().Contains(".") && !c.ToString().Contains("_") && !res)
                        {
                            dr[c.ToString()] = reportTable.Compute("Sum(" + c.ToString() + ")", "");
                        }
                        else
                        {
                            var temp = 0;
                            for (int i = 0; i < reportTable.Rows.Count; i++)
                            {
                                if (reportTable.Rows[i][c.ToString()].ToString().Length > 0)
                                {
                                    temp = temp + Convert.ToInt32(reportTable.Rows[i][c.ToString()]);
                                }
                            }
                            dr[c.ToString()] = temp.ToString();
                        }
                    }
                }
                reportTable.Rows.Add(dr);
                linkTable.Rows.Add(drl);

                ReportTable report = new ReportTable();
                report.SearchCol.Add("WORKORDERNO");
                report.SearchCol.Add("SKUNO");
                report.FixedHeader = true;
                report.FixedCol = 5;
                report.LoadData(reportTable, linkTable);
                report.Tittle = "WO WIP";
                //report.Rows.Add(totalRow);
                Outputs.Add(report);
                
            }
            catch (Exception e)
            {
               
                ReportAlart alart = new ReportAlart(e.ToString());
                Outputs.Add(alart);
                return;
            }
            finally
            {
                if (isborrow)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }

        class StationData
        {
            public string WORKORDERNO { get; set; }
            public string NEXT_STATION { get; set; }
            public int QTY { get; set; }
        }
    }
}
