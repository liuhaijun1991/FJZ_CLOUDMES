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
    public class BonepileReport : ReportBase
    {
        ReportInput inputSku = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput { Name = "TYPE", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "SMT", "BGA", "REPAIR", "WIP" } };
        [JsonIgnore]
        [ScriptIgnore]
        public OleExec SFCDB = null;//DBPools["SFCDB"].Borrow();
        public BonepileReport()
        {
            this.Inputs.Add(inputSku);
            this.Inputs.Add(inputType);
        }
        public override void Run()
        {
            string SQLSKU = "";
            string SQLTYPE = "";
            if (inputSku.Value != null && inputSku.Value.ToString() != "")
            {
                SQLSKU = $@" AND SKUNO='{inputSku.Value}'";
            }
            else
            {
                SQLSKU = $@"";
            }

            if (inputType.Value != null && inputType.Value.ToString() != "" && inputType.Value.ToString() != "ALL")
            {
                SQLTYPE = $@" AND FAILTYPE='{inputType.Value}'";
            }
            else
            {
                SQLTYPE = $@"";
            }

            string sqlBPDetail = $@"SELECT *
                                      FROM (SELECT SN.SKUNO,
                                                   SN.WORKORDERNO,
                                                   SN.SN,
                                                   SN.NEXT_STATION,
                                                   RM.FAIL_STATION,
                                                   RM.FAIL_TIME,
                                                   RMF.FAIL_STATION FirstFailStation,
                                                   RMF.FAIL_TIME FirstFailTime,
                                                   RT.IN_TIME,
                                                   RT.OUT_TIME,
                                                   CASE
                                                     WHEN BGA.REPAIR_ID IS NULL AND
                                                          RM.FAIL_STATION IN ('SMT1',
                                                                              'SMT2',
                                                                              'AOI1',
                                                                              'AOI2',
                                                                              'AOI3',
                                                                              'AOI4',
                                                                              'ICT',
                                                                              '5DX') THEN
                                                      'SMT'
                                                     WHEN BGA.REPAIR_ID IS NOT NULL THEN
                                                      'BGA'
                                                     ELSE
                                                      'NONAL'
                                                   END FAILTYPE,
                                                   DECODE(RT.CLOSED_FLAG, NULL, 'FAIL', 0, 'REPAIR', 'WIP') STATUS,
                                                   DECODE(RT.CLOSED_FLAG,
                                                          NULL,
                                                          CEIL((SYSDATE - RM.FAIL_TIME) * 24),
                                                          0,
                                                          CEIL((SYSDATE - RT.IN_TIME) * 24),
                                                          1,
                                                          CEIL((SYSDATE - RT.OUT_TIME) * 24)) StatusAgingHours,
                                                   CEIL((SYSDATE - RM.FAIL_TIME) * 24) TotalAgingHours
                                              FROM R_SN SN
                                              LEFT JOIN (SELECT *
                                                          FROM (SELECT RMT.*,
                                                                       ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME DESC) numbs
                                                                  FROM R_REPAIR_MAIN RMT
                                                                 WHERE EXISTS
                                                                 (SELECT *
                                                                          FROM R_SN SNT
                                                                         WHERE SNT.SN = RMT.SN
                                                                           AND SNT.WORKORDERNO = RMT.WORKORDERNO
                                                                           AND (SNT.REPAIR_FAILED_FLAG = 1 OR
                                                                               (EXISTS (SELECT *
                                                                                           FROM R_REPAIR_MAIN RMTT
                                                                                          WHERE SNT.SN = RMTT.SN
                                                                                            AND SNT.WORKORDERNO =
                                                                                                RMTT.WORKORDERNO
                                                                                            AND SNT.NEXT_STATION =
                                                                                                RMTT.FAIL_STATION) AND
                                                                                SNT.REPAIR_FAILED_FLAG = 0))))
                                                         WHERE numbs = 1) RM
                                                ON SN.SN = RM.SN
                                               AND SN.WORKORDERNO = RM.WORKORDERNO
                                              LEFT JOIN R_REPAIR_TRANSFER RT
                                                ON RM.ID = RT.REPAIR_MAIN_ID
                                              LEFT JOIN (SELECT *
                                                          FROM (SELECT RMT.*,
                                                                       ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME ASC) numbs
                                                                  FROM R_REPAIR_MAIN RMT
                                                                 WHERE EXISTS
                                                                 (SELECT *
                                                                          FROM R_SN SNT
                                                                         WHERE SNT.SN = RMT.SN
                                                                           AND SNT.WORKORDERNO = RMT.WORKORDERNO
                                                                           AND (SNT.REPAIR_FAILED_FLAG = 1 OR
                                                                               (EXISTS (SELECT *
                                                                                           FROM R_REPAIR_MAIN RMTT
                                                                                          WHERE SNT.SN = RMTT.SN
                                                                                            AND SNT.WORKORDERNO =
                                                                                                RMTT.WORKORDERNO
                                                                                            AND SNT.NEXT_STATION =
                                                                                                RMTT.FAIL_STATION) AND
                                                                                SNT.REPAIR_FAILED_FLAG = 0))))
                                                         WHERE numbs = 1) RMF
                                                ON SN.SN = RMF.SN
                                               AND SN.WORKORDERNO = RMF.WORKORDERNO
                                              LEFT JOIN SFCRUNTIME.R_BGA_DETAIL BGA
                                                ON RM.ID = BGA.REPAIR_ID
                                             WHERE SN.VALID_FLAG='1' 
                                                AND (SCRAPED_FLAG<>'1' OR SCRAPED_FLAG IS NULL) 
                                                AND (SN.REPAIR_FAILED_FLAG = 1 
                                                        OR EXISTS
                                                         (SELECT *
                                                                  FROM R_REPAIR_MAIN RMTT
                                                                 WHERE SN.SN = RMTT.SN
                                                                   AND SN.WORKORDERNO = RMTT.WORKORDERNO
                                                                   AND SN.NEXT_STATION = RMTT.FAIL_STATION))) T
                                     WHERE 1 = 1
                                           {SQLSKU}
                                           {SQLTYPE}
                                     ORDER BY TOTALAGINGHOURS,SKUNO,WORKORDERNO,STATUS";

            string sqlPBGroupby = "SELECT FAILTYPE AS \"Fail Type\",\n" +
"                                           Status,\n" +
"                                           SUM(NA) As \"<3H\",\n" +
"                                           SUM(H48)As \"3H-2D\",\n" +
"                                           SUM(H120) As \"2D-5D\",\n" +
"                                           SUM(MH120) As \">5D\"\n" +
"                                      FROM (SELECT FAILTYPE,\n" +
"                                                   STATUS,\n" +
"                                                   DECODE(AGINGTYPE, '-', QTY) NA,\n" +
"                                                   DECODE(AGINGTYPE, '<48', QTY) H48,\n" +
"                                                   DECODE(AGINGTYPE, '>48-120', QTY) H120,\n" +
"                                                   DECODE(AGINGTYPE, '>120', QTY) MH120\n" +
"                                              FROM (SELECT AGINGTYPE, FAILTYPE, STATUS, COUNT(1) QTY\n" +
"                                                      FROM (SELECT T.*,\n" +
"                                                                   CASE\n" +
"                                                                     WHEN TotalAgingHours <= 3 THEN\n" +
"                                                                      '-'\n" +
"                                                                     WHEN TotalAgingHours > 3 AND\n" +
"                                                                          TotalAgingHours < 48 THEN\n" +
"                                                                      '<48'\n" +
"                                                                     WHEN TotalAgingHours >= 48 AND\n" +
"                                                                          TotalAgingHours < 120 THEN\n" +
"                                                                      '>48-120'\n" +
"                                                                     WHEN TotalAgingHours >= 120 THEN\n" +
"                                                                      '>120'\n" +
"                                                                   END AgingType\n" +
"                                                              FROM (SELECT SN.SKUNO,\n" +
"                                                                           SN.WORKORDERNO,\n" +
"                                                                           SN.SN,\n" +
"                                                                           SN.NEXT_STATION,\n" +
"                                                                           RM.FAIL_STATION,\n" +
"                                                                           RM.FAIL_TIME,\n" +
"                                                                           RMF.FAIL_STATION FirstFailStation,\n" +
"                                                                           RMF.FAIL_TIME FirstFailTime,\n" +
"                                                                           RT.IN_TIME,\n" +
"                                                                           RT.OUT_TIME,\n" +
"                                                                           CASE\n" +
"                                                                             WHEN BGA.REPAIR_ID IS NOT NULL THEN\n" +
"                                                                              'BGA'\n" +
"                                                                             WHEN BGA.REPAIR_ID IS NULL AND\n" +
"                                                                                  RM.FAIL_STATION IN\n" +
"                                                                                  ('SMT1',\n" +
"                                                                                   'SMT2',\n" +
"                                                                                   'AOI1',\n" +
"                                                                                   'AOI2',\n" +
"                                                                                   'AOI3',\n" +
"                                                                                   'AOI4',\n" +
"                                                                                   'ICT',\n" +
"                                                                                   '5DX',\n" +
"                                                                                   '5DX2') THEN\n" +
"                                                                              'SMT'\n" +
"                                                                             ELSE\n" +
"                                                                              'FUNCTIONAL'\n" +
"                                                                           END FAILTYPE,\n" +
"                                                                           DECODE(RT.CLOSED_FLAG,\n" +
"                                                                                  NULL,\n" +
"                                                                                  'FAIL',\n" +
"                                                                                  0,\n" +
"                                                                                  'REPAIR',\n" +
"                                                                                  'WIP') STATUS,\n" +
"                                                                           DECODE(RT.CLOSED_FLAG,\n" +
"                                                                                  NULL,\n" +
"                                                                                  CEIL((SYSDATE - RM.FAIL_TIME) * 24),\n" +
"                                                                                  0,\n" +
"                                                                                  CEIL((SYSDATE - RT.IN_TIME) * 24),\n" +
"                                                                                  1,\n" +
"                                                                                  CEIL((SYSDATE - RT.OUT_TIME) * 24)) StatusAgingHours,\n" +
"                                                                           CEIL((SYSDATE - RM.FAIL_TIME) * 24) TotalAgingHours\n" +
"                                                                      FROM R_SN SN\n" +
"                                                                      LEFT JOIN (SELECT *\n" +
"                                                                                  FROM (SELECT RMT.*,\n" +
"                                                                                               ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME DESC) numbs\n" +
"                                                                                          FROM R_REPAIR_MAIN RMT\n" +
"                                                                                         WHERE EXISTS\n" +
"                                                                                         (SELECT *\n" +
"                                                                                                  FROM R_SN SNT\n" +
"                                                                                                 WHERE SNT.SN =\n" +
"                                                                                                       RMT.SN\n" +
"                                                                                                   AND SNT.WORKORDERNO =\n" +
"                                                                                                       RMT.WORKORDERNO\n" +
"                                                                                                   AND (SNT.REPAIR_FAILED_FLAG = 1 OR\n" +
"                                                                                                       (EXISTS\n" +
"                                                                                                        (SELECT *\n" +
"                                                                                                            FROM R_REPAIR_MAIN RMTT\n" +
"                                                                                                           WHERE SNT.SN =\n" +
"                                                                                                                 RMTT.SN\n" +
"                                                                                                             AND SNT.WORKORDERNO =\n" +
"                                                                                                                 RMTT.WORKORDERNO\n" +
"                                                                                                             AND SNT.NEXT_STATION =\n" +
"                                                                                                                 RMTT.FAIL_STATION) AND\n" +
"                                                                                                        SNT.REPAIR_FAILED_FLAG = 0))))\n" +
"                                                                                 WHERE numbs = 1) RM\n" +
"                                                                        ON SN.SN = RM.SN\n" +
"                                                                       AND SN.WORKORDERNO = RM.WORKORDERNO\n" +
"                                                                      LEFT JOIN R_REPAIR_TRANSFER RT\n" +
"                                                                        ON RM.ID = RT.REPAIR_MAIN_ID\n" +
"                                                                      LEFT JOIN (SELECT *\n" +
"                                                                                  FROM (SELECT RMT.*,\n" +
"                                                                                               ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME ASC) numbs\n" +
"                                                                                          FROM R_REPAIR_MAIN RMT\n" +
"                                                                                         WHERE EXISTS\n" +
"                                                                                         (SELECT *\n" +
"                                                                                                  FROM R_SN SNT\n" +
"                                                                                                 WHERE SNT.SN =\n" +
"                                                                                                       RMT.SN\n" +
"                                                                                                   AND SNT.WORKORDERNO =\n" +
"                                                                                                       RMT.WORKORDERNO\n" +
"                                                                                                   AND (SNT.REPAIR_FAILED_FLAG = 1 OR\n" +
"                                                                                                       (EXISTS\n" +
"                                                                                                        (SELECT *\n" +
"                                                                                                            FROM R_REPAIR_MAIN RMTT\n" +
"                                                                                                           WHERE SNT.SN =\n" +
"                                                                                                                 RMTT.SN\n" +
"                                                                                                             AND SNT.WORKORDERNO =\n" +
"                                                                                                                 RMTT.WORKORDERNO\n" +
"                                                                                                             AND SNT.NEXT_STATION =\n" +
"                                                                                                                 RMTT.FAIL_STATION) AND\n" +
"                                                                                                        SNT.REPAIR_FAILED_FLAG = 0))))\n" +
"                                                                                 WHERE numbs = 1) RMF\n" +
"                                                                        ON SN.SN = RMF.SN\n" +
"                                                                       AND SN.WORKORDERNO = RMF.WORKORDERNO\n" +
"                                                                      LEFT JOIN SFCRUNTIME.R_BGA_DETAIL BGA\n" +
"                                                                        ON RM.ID = BGA.REPAIR_ID\n" +
"                                                                     WHERE SN.VALID_FLAG='1' AND (SCRAPED_FLAG<>'1' OR SCRAPED_FLAG IS NULL) AND (SN.REPAIR_FAILED_FLAG = 1\n" +
"                                                                        OR EXISTS\n" +
"                                                                     (SELECT *\n" +
"                                                                              FROM R_REPAIR_MAIN RMTT\n" +
"                                                                             WHERE SN.SN = RMTT.SN\n" +
"                                                                               AND SN.WORKORDERNO = RMTT.WORKORDERNO\n" +
"                                                                               AND SN.NEXT_STATION =\n" +
"                                                                                   RMTT.FAIL_STATION))) T\n" +
"                                                             WHERE 1 = 1\n" +
"                                                                {0}\n" +
"                                                                {1}\n" +
"                                                            ) TT\n" +
"                                                     GROUP BY AGINGTYPE, FAILTYPE, STATUS) T)\n" +
"                                     GROUP BY FAILTYPE, STATUS  ORDER BY FAILTYPE, STATUS ASC";
            sqlPBGroupby = string.Format(sqlPBGroupby, SQLSKU, SQLTYPE);

            var snlinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=";

            bool isborrow = false;

            if (SFCDB == null)
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                isborrow = true;
            }

            try
            {
                RunSqls.Add(sqlBPDetail);
                RunSqls.Add(sqlPBGroupby);
                DataTable reportTable = SFCDB.ORM.Ado.GetDataTable(sqlBPDetail);
                DataTable groupData = SFCDB.ORM.Ado.GetDataTable(sqlPBGroupby);
                var chartDate = SFCDB.ORM.SqlQueryable<BPChartData>(sqlPBGroupby).ToList();
                if (reportTable.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                DataTable linkTable = new DataTable();
                for (int i = 0; i < reportTable.Columns.Count; i++)
                {
                    linkTable.Columns.Add(reportTable.Columns[i].ColumnName);
                }

                DataRow linkDataRow;
                foreach (DataRow row in reportTable.Rows)
                {
                    try
                    {
                        linkDataRow = linkTable.NewRow();
                        linkDataRow["SN"] = snlinkURL + row["SN"].ToString();
                        linkTable.Rows.Add(linkDataRow);
                    }
                    catch (Exception ee)
                    {

                    }
                }

                //添加容器第一行
                this.Layout.Add(
                    new ReportLayout[3]{
                        new ReportLayout() {ID="GroupByLayout", Scale=4},
                        new ReportLayout() {ID="PieLayout",Scale=4 },
                        new ReportLayout() {ID="Pie2Layout",Scale=4 }
                    });
                //添加容器第二行
                this.Layout.Add(
                    new ReportLayout[1]{
                        new ReportLayout() {ID="BonepileDetail", Scale=12}
                    });

                ReportTable reportGroupby = new ReportTable();
                //指定報表輸出的容器，把Layout中容器ID賦值給ContainerID
                reportGroupby.ContainerID = "GroupByLayout";
                reportGroupby.Tittle = "Bone Pile Aging Hours";
                reportGroupby.pagination = false;
                reportGroupby.LoadData(groupData);
                Outputs.Add(reportGroupby);

                if (groupData.Rows.Count > 0)
                {

                    var objList = new List<object>();
                    pieChart pie = new pieChart();
                    pie.ContainerID = "PieLayout";
                    pie.Tittle = "Status Distribution";//分佈餅狀圖
                    pie.ChartTitle = "Main title";//主標題
                    pie.ChartSubTitle = "Subtitle";//副標題

                    ChartData chartData = new ChartData();
                    chartData.name = "BonepileStatus";
                    chartData.type = ChartType.pie.ToString();
                    chartData.size = "60%";
                    var wip = 0;
                    var repair = 0;
                    var fail = 0;
                    for (int i = 0; i < groupData.Rows.Count; i++)
                    {
                        int value = 0;
                        foreach (DataColumn item in groupData.Columns)
                        {
                            var columnName = item.ColumnName.ToString().ToUpper();
                            if (columnName != "STATUS" && columnName != "FAIL TYPE" && groupData.Rows[i][columnName].ToString() != "" && groupData.Rows[i][columnName].ToString() != "0")
                            {
                                var v2 = 0;
                                int.TryParse(groupData.Rows[i][item.ColumnName].ToString(), out v2);
                                value += v2;
                            }
                        }
                        switch (groupData.Rows[i]["STATUS"].ToString())
                        {
                            case "WIP":
                                wip += value;
                                break;
                            case "FAIL":
                                fail += value;
                                break;
                            case "REPAIR":
                                repair += value;
                                break;
                            default:
                                break;
                        }
                    }

                    chartData.data = new List<object> { new List<object> { "WIP", wip }, new List<object> { "FAIL", fail }, new List<object> { "REPAIR", repair } };
                    chartData.colorByPoint = true;


                    ChartData chartData2 = new ChartData();
                    chartData2.name = "BonepileType";
                    chartData2.type = ChartType.pie.ToString();
                    chartData2.size = "80%";
                    chartData2.innerSize = "40%";
                    chartData2.data = new List<object> { new List<object> { "FUNCTIONAL", 50 }, new List<object> { "SMT", 100 } };
                    chartData2.colorByPoint = true;

                    List<ChartData> _ChartDatas = new List<ChartData> { chartData, chartData2 };
                    pie.ChartDatas = _ChartDatas;
                    Outputs.Add(pie);
                }

                if (reportTable.Rows.Count > 0)
                {
                    var objList = new List<object>();
                    pieChart pie = new pieChart();
                    pie.ContainerID = "Pie2Layout";
                    pie.Tittle = "Fail Station Distribution";//分佈餅狀圖
                    pie.ChartTitle = "Main title";//主標題
                    pie.ChartSubTitle = "Subtitle";//副標題

                    ChartData chartData = new ChartData();
                    chartData.name = "Bonepile";
                    chartData.type = ChartType.pie.ToString();

                    Dictionary<string, int> groupbyStation = new Dictionary<string, int>();

                    for (int i = 0; i < reportTable.Rows.Count; i++)
                    {
                        if (groupbyStation.ContainsKey(reportTable.Rows[i]["FAIL_STATION"].ToString()))
                        {
                            groupbyStation[reportTable.Rows[i]["FAIL_STATION"].ToString()] += 1;
                        }
                        else
                        {
                            groupbyStation.Add(reportTable.Rows[i]["FAIL_STATION"].ToString(), 1);
                        }
                    }
                    var gpstation = new List<object>();
                    foreach (var item in groupbyStation.Keys)
                    {
                        gpstation.Add(new List<object> { item, groupbyStation[item] });
                    }
                    chartData.data = gpstation;
                    chartData.colorByPoint = true;
                    List<ChartData> _ChartDatas = new List<ChartData> { chartData };
                    pie.ChartDatas = _ChartDatas;
                    Outputs.Add(pie);
                }

                ReportTable report = new ReportTable();
                report.ContainerID = "BonepileDetail";
                report.Tittle = "Bone Pile Detail";
                report.FixedHeader = true;
                report.SearchCol.Add("WORKORDERNO");
                report.SearchCol.Add("SKUNO");
                report.SearchCol.Add("SN");
                report.LoadData(reportTable, linkTable);

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
    }
    class BPChartData
    {
        public string AGINGTYPE { get; set; }
        public string FAILTYPE { get; set; }
        public string STATUS { get; set; }
        public string QTY { get; set; }
    }
}
