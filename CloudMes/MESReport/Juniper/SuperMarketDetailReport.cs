using System;
using System.Data;
using MESDBHelper;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace MESReport.Juniper
{
    class SuperMarketDetailReport : MESReport.ReportBase
    {
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput SKU = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput inputStatus = new ReportInput { Name = "Status", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "WIP", "AlreadyOUT" } };

        private string SN_F = null;
        private string SKU_F = null;
        private string State_flag = null;
        public SuperMarketDetailReport()
        {
            Inputs.Add(SN);
            Inputs.Add(SKU);
            Inputs.Add(inputStatus);
        }

        public override void Run()
        {
            try
            {
                SN_F = SN.Value == null ? SN.Value.ToString() : SN.Value.ToString().Trim().ToUpper();
                SKU_F = SKU.Value == null ? SKU.Value.ToString() : SKU.Value.ToString().Trim().ToUpper();
                State_flag = inputStatus.Value == null ? inputStatus.Value.ToString() : inputStatus.Value.ToString().Trim().ToUpper();

                var strSqlSummary = $@"SELECT SN.SKUNO,
                                           COUNT(SN.SN) TOTAL  
                                    FROM SFCRUNTIME.R_SUPERMARKET SM
                                    INNER JOIN SFCRUNTIME.R_SN SN
                                    ON SM.R_SN_ID = SN.ID
                                    WHERE SM.STATUS = '1'
                                    GROUP BY SN.SKUNO
                                    ORDER BY TOTAL DESC";

                var strSql = $@"
                    SELECT SN.SN, 
                           SN.SKUNO,
                           CASE SM.STATUS
                                WHEN '1' THEN 'WIP'
                              ELSE
                                'ALREADY OUT'
                           END STATUS,
                           SM.IN_TIME, 
                           SM.IN_BY, 
                           SM.OUT_TIME, 
                           SM.OUT_BY, 
                           CASE SM.STATUS
                                WHEN '1' THEN ROUND(SYSDATE - SM.IN_TIME)
                              ELSE
                                ROUND(SM.OUT_TIME - SM.IN_TIME)
                            END SM_AGING
                    FROM SFCRUNTIME.R_SUPERMARKET SM
                    INNER JOIN SFCRUNTIME.R_SN SN
                    ON SM.R_SN_ID = SN.ID";

                if (!String.IsNullOrEmpty(SN_F) & String.IsNullOrEmpty(SKU_F))
                    strSql += $@" and SN.SN = '{SN_F}' ";
                else if (String.IsNullOrEmpty(SN_F) & !String.IsNullOrEmpty(SKU_F))
                    strSql += $@" and SN.SKUNO = '{SKU_F}' ";
                else if (!String.IsNullOrEmpty(SN_F) & !String.IsNullOrEmpty(SKU_F))
                    strSql += $@" and SN.SN = '{SN_F}' and SN.SKUNO = '{SKU_F}' ";

                if (State_flag == "WIP")
                    strSql += $@" AND SM.STATUS = '1' ";
                else if (State_flag == "ALREADYOUT")
                    strSql += $@" AND SM.STATUS = '0' ";

                strSql += " ORDER BY SM.IN_TIME DESC";

                var sfcdb = DBPools["SFCDB"].Borrow();

                try
                {
                    this.Layout.Add(
                    new ReportLayout[2]{
                        new ReportLayout() {ID="GroupByLayout", Scale=5},
                        new ReportLayout() {ID="PieLayout",Scale=7 }
                    });
                    //添加容器第二行
                    this.Layout.Add(
                        new ReportLayout[1]{
                        new ReportLayout() {ID="SNDetail", Scale=12}
                        });



                    //----------------------------------------------SN DETAIL
                    var res = sfcdb.RunSelect(strSql);
                    ReportTable retTab = new ReportTable();
                    retTab.ContainerID = "SNDetail";
                    retTab.Tittle = $@"Supermarket {State_flag} SN Detail Report";

                    DataTable linkTable = new DataTable();
                    foreach (DataColumn column in res.Tables[0].Columns)
                    {
                        linkTable.Columns.Add(column.ColumnName);
                    }
                    foreach (DataRow row in res.Tables[0].Rows)
                    {
                        string linkURL1 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["sn"].ToString();
                        var linkRow = linkTable.NewRow();
                        foreach (DataColumn dc in linkTable.Columns)
                        {
                            if (dc.ColumnName.ToString().ToUpper() == "SN")
                            {
                                linkRow[dc.ColumnName] = linkURL1;
                            }
                            //else if (dc.ColumnName.ToString().ToUpper() == "SKUNO")
                            //{
                            //    linkRow[dc.ColumnName] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.SilverWipReport&RunFlag=1&SKU=" + row["skuno"].ToString();
                            //}
                            else
                            {
                                linkRow[dc.ColumnName] = "";
                            }
                        }
                        linkTable.Rows.Add(linkRow);
                    }

                    retTab.LoadData(res.Tables[0], linkTable);
                    Outputs.Add(retTab);

                    if (State_flag != "ALREADYOUT")
                    {
                        //---------------------------------------------Group By
                        var res2 = sfcdb.RunSelect(strSqlSummary);
                        ReportTable retTab2 = new ReportTable();
                        retTab2.ContainerID = "GroupByLayout";
                        retTab2.Tittle = "SKU- WIP Summary";

                        DataTable linkTable2 = new DataTable();
                        foreach (DataColumn column in res2.Tables[0].Columns)
                        {
                            linkTable2.Columns.Add(column.ColumnName);
                        }


                        retTab2.LoadData(res2.Tables[0]);
                        Outputs.Add(retTab2);

                        //----------------------------------------------Pie Chart
                        string columnName = "";
                        List<object> objList = new List<object>();
                        pieChart pie = new pieChart();
                        pie.Tittle = "WIP Distribution";//分佈餅狀圖
                        pie.ChartTitle = "Main title";//主標題
                        pie.ChartSubTitle = "Subtitle";//副標題
                        ChartData chartData = new ChartData();
                        pie.ContainerID = "PieLayout";
                        chartData.name = "WOLIST";
                        chartData.type = ChartType.pie.ToString();
                        for (int j = 0; j < res2.Tables[0].Rows.Count; j++)
                        {
                            foreach (DataColumn column in res2.Tables[0].Columns)
                            {
                                columnName = column.ColumnName.ToString().ToUpper();
                                if (columnName != "SKUNO")
                                {
                                    var qty = Convert.ToInt64(res2.Tables[0].Rows[j][columnName].ToString());
                                    objList.Add(new List<object> { res2.Tables[0].Rows[j]["SKUNO"].ToString() + " (" + qty.ToString() + ")", qty }) ;
                                }
                            }
                        }
                        chartData.data = objList;
                        chartData.colorByPoint = true;
                        List<ChartData> _ChartDatas = new List<ChartData> { chartData };
                        pie.ChartDatas = _ChartDatas;
                        Outputs.Add(pie);
                    }
                }
                catch (Exception ex)
                {
                    ReportAlart alart = new ReportAlart(ex.Message);
                    Outputs.Add(alart);
                }
                finally
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }

        }
    }
}
