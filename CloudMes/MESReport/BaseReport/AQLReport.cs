using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class AQLReport : ReportBase
    {

        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput skuInput = new ReportInput
        {
            Name = "SKUNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput Style = new ReportInput()
        {
            Name = "STYLE",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            // ValueForUse = new string[] { "ALL", "待抽检", "PASS", "FAIL" }
            ValueForUse = new string[] { "ALL", "AlreadyTest", "NoTest" }
        };
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Series = new ReportInput { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        string sqlRun = "";
        public AQLReport()
        {

            Inputs.Add(WO);
            Inputs.Add(skuInput);
            Inputs.Add(Style);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            Inputs.Add(Series);
        }
        public override void Init()
        {
            //base.Init()
            startTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            endTime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                InitSeries(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public override void Run()
        {
            string Wo = WO.Value.ToString();
            string SKUNO = skuInput.Value.ToString();
            string STYLE = Style.Value?.ToString();
            string series = Series.Value.ToString();
            DateTime startDT = Convert.ToDateTime(startTime.Value);
            DateTime endDT = Convert.ToDateTime(endTime.Value);
            string dateFrom = startDT.ToString("yyyy/MM/dd HH:mm:ss");
            string dateTo = endDT.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string sqlRun;
            //string sqlRun = $@"SELECT B.WORKORDERNO,B.SKUNO,B.SN, A.CREATETIME 抽測時間,
            //                       CASE
            //                         WHEN C.SN IS NULL THEN
            //                          '沒有測試'
            //                         ELSE
            //                          '已經測試'
            //                       END 是否被AQL測試,
            //                       C.STARTTIME 開始測試時間,
            //                       C.ENDTIME 結束測試時間,
            //                       C.STATE 測試結果
            //                  FROM R_SN_LOG A
            //                  LEFT JOIN R_SN B
            //                    ON A.SN = B.SN
            //                  LEFT JOIN R_TEST_RECORD C
            //                    ON C.SN = A.SN
            //                   AND C.MESSTATION = 'AQL'
            //                 WHERE 1 = 1
            //                   temp_wo
            //                   AND A.LOGTYPE = 'CHARGE-SAMPLE'
            //                   AND A.FLAG = 'Y'
            //                 ORDER BY A.CREATETIME";
            // 中文翻譯成英文--廖東林--20210816
            if (series == "ALL")
            {
                sqlRun = $@"SELECT B.WORKORDERNO,B.SKUNO,B.SN, A.CREATETIME TestTime,
                                   CASE
                                     WHEN C.SN IS NULL THEN
                                      'NoTest'
                                     ELSE
                                      'AlreadyTest'
                                   END AQLResult,
                                   C.STARTTIME StartTestTime,
                                   C.ENDTIME EndTestTime,
                                   C.STATE TestResult
                              FROM R_SN_LOG A
                              LEFT JOIN R_SN B
                                ON A.SN = B.SN
                              LEFT JOIN R_TEST_RECORD C
                                ON C.SN = A.SN
                               AND C.MESSTATION = 'AQL'
                             WHERE 1 = 1
                               AND A.LOGTYPE = 'CHARGE-SAMPLE'
                               AND A.FLAG = 'Y' ";

                if (Wo != "ALL" && Wo != "")
                {
                    sqlRun += $@" and B.WORKORDERNO='{Wo}'";
                }

                if (SKUNO != "ALL" && SKUNO != "")
                {
                    sqlRun += $@" and b.skuno='{SKUNO}'";
                }

                if (STYLE != "ALL" && STYLE != "")
                {
                    if (STYLE == "AlreadyTest")
                    {
                        sqlRun += $@" AND B.SN  IN (SELECT SN FROM R_TEST_RECORD WHERE   MESSTATION='AQL')";
                    }
                    else
                    {
                        sqlRun += $@" AND B.SN NOT IN (SELECT SN FROM R_TEST_RECORD WHERE   MESSTATION='AQL')";
                    }
                }

                try
                {
                    DataTable res = SFCDB.ExecuteDataTable(sqlRun, CommandType.Text);
                    if (res.Rows.Count == 0)
                    {
                        throw new Exception("No data");
                    }
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(res, null);
                    retTab.Tittle = "AQLReport";
                    Outputs.Add(retTab);
                    DBPools["SFCDB"].Return(SFCDB);
                }
                catch (Exception ex)
                {
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    ReportAlart alart = new ReportAlart(ex.Message);
                    Outputs.Add(alart);
                }
            }
            else
            {
                sqlRun = $@"   SELECT b.SKUNO, COUNT(case when a.STATE = 'PASS' then a.sn else null end) AS PASS,
                           COUNT(case when a.STATE = 'FAIL' then a.sn else null end) AS FAIL
                             FROM SFCRUNTIME.R_TEST_RECORD a, r_sn b WHERE a.SN=b.SN AND a.SN IN (
                           SELECT SN FROM r_sn WHERE SKUNO IN (
                           SELECT  SKUNO FROM c_sku WHERE C_SERIES_ID IN ( 
                           SELECT ID FROM SFCBASE.C_SERIES WHERE CUSTOMER_ID IN (
                           SELECT ID FROM C_CUSTOMER WHERE CUSTOMER_NAME ='{series}'))) AND VALID_FLAG=1) AND a.MESSTATION = 'AQL' AND a.EDIT_TIME between
                            TO_DATE('{dateFrom}', 'yyyy/MM/dd hh24:mi:ss') and
                            TO_DATE('{dateTo}', 'yyyy/MM/dd hh24:mi:ss')  GROUP BY  b.SKUNO ";

                try
                {
                    DataRow linkDataRow = null;
                    DataTable linkTable = new DataTable();
                    DataTable res = SFCDB.ExecuteDataTable(sqlRun, CommandType.Text);
                    if (res.Rows.Count == 0)
                    {
                        throw new Exception("No data");
                    }
                    var totalInput = 0;
                    var totalFial = 0;
                    for (int i = 0; i < res.Rows.Count; i++)
                    {
                        // totalInput += Convert.ToInt32(dt.Rows[i]["投入"]);


                        totalInput += Convert.ToInt32(res.Rows[i]["PASS"]);

                        // totalFial += Convert.ToInt32(dt.Rows[i]["不良總數"]);

                        totalFial += Convert.ToInt32(res.Rows[i]["FAIL"]);
                    }
                    string allSKU = string.Empty;
                    for (int t = 0; t < res.Rows.Count; t++)
                    {
                        allSKU += res.Rows[t]["SKUNO"].ToString();
                        allSKU += ",";
                    }
                    var totalRow = res.NewRow();
                    //totalRow[0] = "總數";
                    totalRow[0] = "Total";
                    totalRow[1] = totalInput.ToString();
                    totalRow[2] = totalFial.ToString();
                    if (res.Rows.Count > 0)
                        res.Rows.Add(totalRow);
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("PASS");
                    linkTable.Columns.Add("FAIL");
                    for (int t = 0; t < res.Rows.Count; t++)
                    {
                        linkDataRow = linkTable.NewRow();
                        linkDataRow["PASS"] = res.Rows[t]["PASS"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SnAQLReport&RunFlag=1&SKU=" + res.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + res.Columns[1].ToString() + "&Type=INPUT";
                        linkDataRow["FAIL"] = res.Rows[t]["FAIL"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SnAQLReport&RunFlag=1&SKU=" + res.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + res.Columns[2].ToString() + "&Type=INPUT";
                        if (res.Rows[t]["SKUNO"].ToString() == "Total")
                        {
                            linkDataRow["PASS"] = res.Rows[t]["PASS"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SnAQLReport&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + res.Columns[1].ToString() + "&Type=INPUT";
                            linkDataRow["FAIL"] = res.Rows[t]["FAIL"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SnAQLReport&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + res.Columns[2].ToString() + "&Type=INPUT";
                        }
                        linkTable.Rows.Add(linkDataRow);
                    }
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(res, linkTable);
                    retTab.Tittle = "AQLReport";
                    Outputs.Add(retTab);
                    DBPools["SFCDB"].Return(SFCDB);
                }
                catch (Exception ex)
                {
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    ReportAlart alart = new ReportAlart(ex.Message);
                    Outputs.Add(alart);
                }
            }

        }
        public void InitSeries(OleExec db)
        {
            List<string> series = new List<string>();
            DataTable dt = new DataTable();
            string sql = $@"SELECT CUSTOMER_NAME FROM C_CUSTOMER ORDER BY CUSTOMER_NAME";
            dt = db.ExecSelect(sql).Tables[0];
            series.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                series.Add(dr["CUSTOMER_NAME"].ToString());
            }
            Series.ValueForUse = series;
        }
    }
}
