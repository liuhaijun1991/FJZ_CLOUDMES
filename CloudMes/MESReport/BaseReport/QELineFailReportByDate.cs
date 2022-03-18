using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class QELineFailReportByDate : ReportBase
    {
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Series = new ReportInput { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput Station = new ReportInput { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };

        public QELineFailReportByDate()
        {
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            Inputs.Add(Series);
            Inputs.Add(Station);
        }
        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            endTime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                InitSeries(SFCDB);
                InitStation(SFCDB);

                //DBPools["SFCDB"].Return(SFCDB);
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
            DateTime startDT = Convert.ToDateTime(startTime.Value);
            DateTime endDT = Convert.ToDateTime(endTime.Value);
            string dateFrom = startDT.ToString("yyyy-MM-dd HH:mm:ss");
            string dateTo = endDT.ToString("yyyy-MM-dd HH:mm:ss");
            string series = Series.Value.ToString();
            string station = Station.Value.ToString();
            if (series.Equals("ALL"))
                throw new Exception($"Please Choose Series!");
            //if (station.Equals("ALL"))
            //    throw new Exception($"Please Choose Station!");

            //string sqlRun = $@"
            //    select series,
            //        skuno,
            //        station,
            //        Total 投入,
            //        Fail 不良總數,
            //        round(((Total-Fail) / Total) * 100, 2) ||'%' 良率
            //    /*from (select aa.series,aa.skuno,aa.station,aa.total,bb.fail From (select d.customer_name series,
            //                a.skuno,
            //                a.station_name station,
            //                count(distinct a.sn) Total
            //            from r_sn_station_detail a, c_sku b, c_series c, c_customer d
            //            where 1 = 1
            //            and a.skuno = b.skuno
            //            and b.c_series_id = c.id
            //            and c.customer_id = d.id
            //            and a.valid_flag = 1
            //            and a.line <> 'Replace'
            //            and a.line is not null
            //            and substr(a.sn, 0, 1) not in ('*', '#', '~')
            //            and a.edit_time between
            //                to_date('{dateFrom}', 'yyyy-MM-dd hh24:mi:ss') and
            //                to_date('{dateTo}', 'yyyy-MM-dd hh24:mi:ss')   
            //            Temp_Station_Sql
            //            Temp_Series_Sql
            //          group by d.customer_name, a.skuno, a.station_name
            //          order by d.customer_name, a.skuno, a.station_name)aa,(select distinct d.customer_name series,
            //                a.skuno,
            //                a.station_name station,
            //                   case when EXISTS(SELECT*fROM R_SN_STATION_DETAIL B WHERE a.SN=B.SN AND B.REPAIR_FAILED_FLAG=1 and a.edit_time=b.edit_time and a.station_name=b.station_name) 
            //                  then (SELECT COUNT(DISTINCT B.SN)  fROM R_SN_STATION_DETAIL B WHERE a.SN=B.SN AND B.REPAIR_FAILED_FLAG=1)
            //              ELSE 0 end as Fail
            //            from r_sn_station_detail a, c_sku b, c_series c, c_customer d
            //            where 1 = 1
            //            and a.skuno = b.skuno
            //            and b.c_series_id = c.id
            //            and c.customer_id = d.id
            //            and a.valid_flag = 1
            //            and a.line <> 'Replace'
            //            and a.line is not null
            //            and substr(a.sn, 0, 1) not in ('*', '#', '~')
            //            and a.edit_time between
            //                to_date('{dateFrom}', 'yyyy-MM-dd hh24:mi:ss') and
            //                to_date('{dateTo}', 'yyyy-MM-dd hh24:mi:ss')   
            //            Temp_Station_Sql
            //            Temp_Series_Sql) bb where aa.SKUNO=bb.skuno and aa.STATION=bb.STATION)
            //    order by skuno
            //select series,
            //        skuno,
            //        station,
            //        Total 投入,
            //        Fail 不良總數,
            //        round(((Total-Fail) / Total) * 100, 2) ||'%' 良率 */
            //    from (select d.customer_name series,
            //                a.skuno,
            //                a.station_name station,
            //                count(distinct a.sn) Total,
            //                /*sum(case when a.repair_failed_flag = 0 then 0 when a.repair_failed_flag = 1 then 1 end) as Fail*/
            //                count(distinct(case when a.repair_failed_flag = 1 then a.sn else null end)) as Fail
            //            from r_sn_station_detail a, c_sku b, c_series c, c_customer d
            //            where 1 = 1
            //            and a.skuno = b.skuno
            //            and b.c_series_id = c.id
            //            and c.customer_id = d.id
            //            and a.valid_flag = 1
            //            and a.line <> 'Replace'
            //            and a.line is not null
            //            and substr(a.sn, 0, 1) not in ('*', '#', '~')
            //            and a.edit_time between
            //                to_date('{dateFrom}', 'yyyy-MM-dd hh24:mi:ss') and
            //                to_date('{dateTo}', 'yyyy-MM-dd hh24:mi:ss')   
            //            Temp_Station_Sql
            //            Temp_Series_Sql
            //          group by d.customer_name, a.skuno, a.station_name
            //          order by d.customer_name, a.skuno, a.station_name)
            //    order by skuno ";

            //VNQE 阮文論要求增加工單 20211019
            string sqlRun = null;
            if (series == "BROADCOM" || series == "NETGEAR" || series == "SE")
            {
                sqlRun = $@"
                      select series,
                    skuno,
                    station,
                    Total InputQty,
                    Fail FailTotal,
                    round(((Total-Fail) / Total) * 100, 2) ||'%' YieldRatef
                from (select d.customer_name series,
                            a.skuno,
                            a.station_name station,
                          count(distinct a.sn) Total,
                            /*sum(case when a.repair_failed_flag = 0 then 0 when a.repair_failed_flag = 1 then 1 end) as Fail*/
                            count(distinct(case when a.repair_failed_flag = 1 then a.sn else null end)) as Fail
                        from r_sn_station_detail a, c_sku b, c_series c, c_customer d
                        where 1 = 1
                        and a.skuno = b.skuno
                        and b.c_series_id = c.id
                        and c.customer_id = d.id
                        and a.valid_flag = 1
                        and a.line <> 'Replace'
                        and a.line is not null
                        and substr(a.sn, 0, 1) not in ('*', '#', '~')
                        and a.edit_time between
                              to_date('{dateFrom}', 'yyyy-MM-dd hh24:mi:ss') and
                            to_date('{dateTo}', 'yyyy-MM-dd hh24:mi:ss')   
                        Temp_Station_Sql
                        Temp_Series_Sql 
                      group by d.customer_name, a.skuno, a.station_name
                      order by d.customer_name, a.skuno, a.station_name)
                order by skuno  ";

                string tempStation = string.Empty;
                if (station != "ALL")
                {
                    tempStation = $@" and a.station_name = '{station}' ";
                }
                string tempSeries = string.Empty;
                if (series != "ALL")
                {
                    tempSeries = $@" and d.customer_name = '{series}' ";
                }
                sqlRun = sqlRun.Replace("Temp_Station_Sql", tempStation);
                sqlRun = sqlRun.Replace("Temp_Series_Sql", tempSeries);

                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = null;
                DataRow linkDataRow = null;
                DataTable linkTable = new DataTable();
                try
                {
                    dt = SFCDB.RunSelect(sqlRun).Tables[0];
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }

                    //增加一行總數                
                    var totalInput = 0;
                    var totalFial = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        // totalInput += Convert.ToInt32(dt.Rows[i]["投入"]);


                        totalInput += Convert.ToInt32(dt.Rows[i]["InputQty"]);

                        // totalFial += Convert.ToInt32(dt.Rows[i]["不良總數"]);

                        totalFial += Convert.ToInt32(dt.Rows[i]["FailTotal"]);
                    }
                    string allSKU = string.Empty;
                    for (int t = 0; t < dt.Rows.Count; t++)
                    {
                        allSKU += dt.Rows[t]["SKUNO"].ToString();
                        allSKU += ",";
                    }
                    //VNQE 阮文論要求增加工單 20211019

                    var totalRow = dt.NewRow();
                    //totalRow[0] = "總數";
                    totalRow[0] = "Total";
                    totalRow[1] = "-";

                    totalRow[2] = "-";//VNQE 阮文論要求增加工單 20211019
                    totalRow[3] = totalInput.ToString();
                    totalRow[4] = totalFial.ToString();
                    //totalRow[5] = (Math.Round(((double)totalFial / totalInput) * 100, 2)).ToString() + "%";
                    totalRow[5] = (Math.Round(((double)(totalInput - totalFial) / totalInput) * 100, 2)).ToString() + "%";
                    if (dt.Rows.Count > 0)
                        dt.Rows.Add(totalRow);

                    linkTable.Columns.Add("SERIES");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("STATION");
                    //linkTable.Columns.Add("WORKORDERNO");//VNQE 阮文論要求增加工單 20211019
                    //linkTable.Columns.Add("投入");
                    linkTable.Columns.Add("InputQty");

                    // linkTable.Columns.Add("不良總數");
                    linkTable.Columns.Add("FailTotal");

                    //linkTable.Columns.Add("不良率");

                    //linkTable.Columns.Add("良率
                    linkTable.Columns.Add("YieldRate");

                    for (int t = 0; t < dt.Rows.Count; t++)
                    {
                        linkDataRow = linkTable.NewRow();
                        linkDataRow["SERIES"] = "";
                        linkDataRow["SKUNO"] = "";
                        linkDataRow["STATION"] = "";

                        if (series == "ALL" && station != "ALL")
                        {

                            //linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=INPUT";
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=INPUT";
                        }
                        else if (series != "ALL" && station == "ALL")
                        {
                            // linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=INPUT";
                            //linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=INPUT";

                            //查詢結果是根據機種和工站匯總的，因此點擊每一行傳過去的值必定包含機種和工站  Edit By ZHB 2021-09-24
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + dt.Rows[t]["STATION"].ToString() + "&Type=INPUT";
                        }
                        else if (series != "ALL" && station != "ALL" && dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {
                            // linkDataRow["不良總數"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) +  "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=INPUT";
                        }
                        else if (series != "ALL" && station != "ALL" && !dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {
                            //linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=INPUT";
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=INPUT";
                        }

                        if (series == "ALL" && station != "ALL")
                        {

                            // linkDataRow["不良總數"] = dt.Rows[t]["不良總數"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=FAILTOTAL";
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=FAILTOTAL";
                        }
                        else if (series != "ALL" && station == "ALL")
                        {

                            //linkDataRow["不良總數"] = dt.Rows[t]["不良總數"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=FAILTOTAL";
                            //linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=FAILTOTAL";

                            //查詢結果是根據機種和工站匯總的，因此點擊每一行傳過去的值必定包含機種和工站  Edit By ZHB 2021-09-24
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + dt.Rows[t]["STATION"].ToString() + "&Type=FAILTOTAL";
                        }
                        else if (series != "ALL" && station != "ALL" && dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {
                            // linkDataRow["不良總數"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1)  + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                        }
                        else if (series != "ALL" && station != "ALL" && !dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {

                            // linkDataRow["不良總數"] = dt.Rows[t]["不良總數"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                        }

                        linkTable.Rows.Add(linkDataRow);
                    }

                    ReportTable reportTable = new ReportTable();
                    reportTable.LoadData(dt, linkTable);
                    reportTable.Tittle = "QE_Report_ByDate";
                    Outputs.Add(reportTable);
                }
                catch (Exception exception)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                    throw exception;
                }
            }
            else
            {
                sqlRun = $@"
                select series,
                    skuno,
                    station,
                    workorderno,
                    Total InputQty,
                    Fail FailTotal,
                    round(((Total-Fail) / Total) * 100, 2) ||'%' YieldRate
                /*from (select aa.series,aa.skuno,aa.station,aa.total,bb.fail From (select d.customer_name series,
                            a.skuno,
                            a.station_name station,
                            count(distinct a.sn) Total
                        from r_sn_station_detail a, c_sku b, c_series c, c_customer d
                        where 1 = 1
                        and a.skuno = b.skuno
                        and b.c_series_id = c.id
                        and c.customer_id = d.id
                        and a.valid_flag = 1
                        and a.line <> 'Replace'
                        and a.line is not null
                        and substr(a.sn, 0, 1) not in ('*', '#', '~')
                        and a.edit_time between
                            to_date('{dateFrom}', 'yyyy-MM-dd hh24:mi:ss') and
                            to_date('{dateTo}', 'yyyy-MM-dd hh24:mi:ss')   
                        Temp_Station_Sql
                        Temp_Series_Sql
                      group by d.customer_name, a.skuno, a.station_name
                      order by d.customer_name, a.skuno, a.station_name)aa,(select distinct d.customer_name series,
                            a.skuno,
                            a.station_name station,
                               case when EXISTS(SELECT*fROM R_SN_STATION_DETAIL B WHERE a.SN=B.SN AND B.REPAIR_FAILED_FLAG=1 and a.edit_time=b.edit_time and a.station_name=b.station_name) 
                              then (SELECT COUNT(DISTINCT B.SN)  fROM R_SN_STATION_DETAIL B WHERE a.SN=B.SN AND B.REPAIR_FAILED_FLAG=1)
                          ELSE 0 end as Fail
                        from r_sn_station_detail a, c_sku b, c_series c, c_customer d
                        where 1 = 1
                        and a.skuno = b.skuno
                        and b.c_series_id = c.id
                        and c.customer_id = d.id
                        and a.valid_flag = 1
                        and a.line <> 'Replace'
                        and a.line is not null
                        and substr(a.sn, 0, 1) not in ('*', '#', '~')
                        and a.edit_time between
                            to_date('{dateFrom}', 'yyyy-MM-dd hh24:mi:ss') and
                            to_date('{dateTo}', 'yyyy-MM-dd hh24:mi:ss')   
                        Temp_Station_Sql
                        Temp_Series_Sql) bb where aa.SKUNO=bb.skuno and aa.STATION=bb.STATION)
                order by skuno
            select series,
                    skuno,
                    station,
                    Total InputQty,
                    Fail FailTotal,
                    round(((Total-Fail) / Total) * 100, 2) ||'%' YieldRate */
                from (select d.customer_name series,
                            a.skuno,
                            a.station_name station,
                            a.workorderno,
                            count(distinct a.sn) Total,
                            /*sum(case when a.repair_failed_flag = 0 then 0 when a.repair_failed_flag = 1 then 1 end) as Fail*/
                            count(distinct(case when a.repair_failed_flag = 1 then a.sn else null end)) as Fail
                        from r_sn_station_detail a, c_sku b, c_series c, c_customer d
                        where 1 = 1
                        and a.skuno = b.skuno
                        and b.c_series_id = c.id
                        and c.customer_id = d.id
                        and a.valid_flag = 1
                        and a.line <> 'Replace'
                        and a.line is not null
                        and substr(a.sn, 0, 1) not in ('*', '#', '~')
                        and a.edit_time between
                            to_date('{dateFrom}', 'yyyy-MM-dd hh24:mi:ss') and
                            to_date('{dateTo}', 'yyyy-MM-dd hh24:mi:ss')   
                        Temp_Station_Sql
                        Temp_Series_Sql
                      group by d.customer_name, a.skuno, a.station_name, a.workorderno
                      order by d.customer_name, a.skuno, a.station_name, a.workorderno)
                order by skuno ";

                string tempStation = string.Empty;
                if (station != "ALL")
                {
                    tempStation = $@" and a.station_name = '{station}' ";
                }
                string tempSeries = string.Empty;
                if (series != "ALL")
                {
                    tempSeries = $@" and d.customer_name = '{series}' ";
                }
                sqlRun = sqlRun.Replace("Temp_Station_Sql", tempStation);
                sqlRun = sqlRun.Replace("Temp_Series_Sql", tempSeries);

                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = null;
                DataRow linkDataRow = null;
                DataTable linkTable = new DataTable();
                try
                {
                    dt = SFCDB.RunSelect(sqlRun).Tables[0];
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }

                    //增加一行總數                
                    var totalInput = 0;
                    var totalFial = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        // totalInput += Convert.ToInt32(dt.Rows[i]["投入"]);


                        totalInput += Convert.ToInt32(dt.Rows[i]["InputQty"]);

                        // totalFial += Convert.ToInt32(dt.Rows[i]["不良總數"]);

                        totalFial += Convert.ToInt32(dt.Rows[i]["FailTotal"]);
                    }
                    string allSKU = string.Empty;
                    for (int t = 0; t < dt.Rows.Count; t++)
                    {
                        allSKU += dt.Rows[t]["SKUNO"].ToString();
                        allSKU += ",";
                    }
                    //VNQE 阮文論要求增加工單 20211019
                    string allWO = string.Empty;
                    for (int t = 0; t < dt.Rows.Count; t++)
                    {
                        allWO += dt.Rows[t]["WORKORDERNO"].ToString();
                        allWO += ",";
                    }
                    var totalRow = dt.NewRow();
                    //totalRow[0] = "總數";
                    totalRow[0] = "Total";
                    totalRow[1] = "-";
                    totalRow[2] = "-";
                    totalRow[3] = "-";//VNQE 阮文論要求增加工單 20211019
                    totalRow[4] = totalInput.ToString();
                    totalRow[5] = totalFial.ToString();
                    //totalRow[5] = (Math.Round(((double)totalFial / totalInput) * 100, 2)).ToString() + "%";
                    totalRow[6] = (Math.Round(((double)(totalInput - totalFial) / totalInput) * 100, 2)).ToString() + "%";
                    if (dt.Rows.Count > 0)
                        dt.Rows.Add(totalRow);

                    linkTable.Columns.Add("SERIES");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("STATION");
                    linkTable.Columns.Add("WORKORDERNO");//VNQE 阮文論要求增加工單 20211019
                                                         //linkTable.Columns.Add("投入");
                    linkTable.Columns.Add("InputQty");

                    // linkTable.Columns.Add("不良總數");
                    linkTable.Columns.Add("FailTotal");

                    //linkTable.Columns.Add("不良率");

                    //linkTable.Columns.Add("良率
                    linkTable.Columns.Add("YieldRate");

                    for (int t = 0; t < dt.Rows.Count; t++)
                    {
                        linkDataRow = linkTable.NewRow();
                        linkDataRow["SERIES"] = "";
                        linkDataRow["SKUNO"] = "";
                        linkDataRow["STATION"] = "";
                        if (series == "ALL" && station == "ALL")
                        {

                            //linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Type=INPUT";
                            //linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Type=INPUT";

                            //查詢結果是根據機種和工站匯總的，因此點擊每一行傳過去的值必定包含機種和工站  Edit By ZHB 2021-09-24
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + dt.Rows[t]["STATION"].ToString() + "&Type=INPUT";
                        }
                        else if (series == "ALL" && station != "ALL")
                        {

                            //linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=INPUT";
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=INPUT";
                        }
                        else if (series != "ALL" && station == "ALL")
                        {
                            // linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=INPUT";
                            //linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=INPUT";

                            //查詢結果是根據機種和工站匯總的，因此點擊每一行傳過去的值必定包含機種和工站  Edit By ZHB 2021-09-24
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + dt.Rows[t]["STATION"].ToString() + "&Type=INPUT";
                        }
                        else if (series != "ALL" && station != "ALL" && !dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {
                            //linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=INPUT";
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=INPUT";
                        }
                        //add by TRANPHUONG 25/02/2021
                        else if (series != "ALL" && station != "ALL" && dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {
                            //linkDataRow["投入"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=INPUT";
                            linkDataRow["InputQty"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&Wo=" + allWO.Remove(allWO.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=INPUT";
                        }
                        //end

                        if (series == "ALL" && station == "ALL")
                        {

                            //linkDataRow["不良總數"] = dt.Rows[t]["不良總數"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Type=FAILTOTAL";
                            //linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Type=FAILTOTAL";

                            //查詢結果是根據機種和工站匯總的，因此點擊每一行傳過去的值必定包含機種和工站  Edit By ZHB 2021-09-24
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + dt.Rows[t]["STATION"].ToString() + "&Type=FAILTOTAL";
                        }
                        else if (series == "ALL" && station != "ALL")
                        {

                            // linkDataRow["不良總數"] = dt.Rows[t]["不良總數"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=FAILTOTAL";
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Station=" + station + "&Type=FAILTOTAL";
                        }
                        else if (series != "ALL" && station == "ALL")
                        {

                            //linkDataRow["不良總數"] = dt.Rows[t]["不良總數"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=FAILTOTAL";
                            //linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Type=FAILTOTAL";

                            //查詢結果是根據機種和工站匯總的，因此點擊每一行傳過去的值必定包含機種和工站  Edit By ZHB 2021-09-24
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + dt.Rows[t]["STATION"].ToString() + "&Type=FAILTOTAL";
                        }
                        else if (series != "ALL" && station != "ALL" && !dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {

                            // linkDataRow["不良總數"] = dt.Rows[t]["不良總數"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                            linkDataRow["FailTotal"] = dt.Rows[t]["FailTotal"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Wo=" + dt.Rows[t]["WORKORDERNO"].ToString() + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                        }
                        //add by TRANPHUONG 25/02/2021
                        else if (series != "ALL" && station != "ALL" && dt.Rows[t]["SKUNO"].ToString().Equals("-"))
                        {
                            // linkDataRow["不良總數"] = dt.Rows[t]["投入"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                            linkDataRow["FailTotal"] = dt.Rows[t]["InputQty"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNFailDetailReportBySKU&RunFlag=1&SKU=" + allSKU.Remove(allSKU.Length - 1, 1) + "&Wo=" + allWO.Remove(allWO.Length - 1, 1) + "&FromDay=" + dateFrom + "&ToDay=" + dateTo + "&Series=" + series + "&Station=" + station + "&Type=FAILTOTAL";
                        }
                        //end

                        //linkDataRow["良率"] = "";
                        linkTable.Rows.Add(linkDataRow);
                    }

                    ReportTable reportTable = new ReportTable();
                    reportTable.LoadData(dt, linkTable);
                    reportTable.Tittle = "QE_Report_ByDate";
                    Outputs.Add(reportTable);
                }
                catch (Exception exception)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                    throw exception;
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

        public void InitStation(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            string sql = $@"SELECT DISTINCT STATION_NAME FROM C_ROUTE_DETAIL ORDER BY STATION_NAME";
            dt = db.ExecSelect(sql).Tables[0];
            station.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                station.Add(dr["STATION_NAME"].ToString());
            }
            Station.ValueForUse = station;
        }
    }
}
