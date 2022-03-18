using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class FPYReport : ReportBase
    {

        ReportInput inputDateType = new ReportInput() { Name = "DateType", InputType = "Select", Value = "ByHours", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ByHours", "ByDay", "ByWeeks", "ByMonth", "ByQuarter","ByYears" } };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        
        public FPYReport()
        {
            Inputs.Add(inputDateType);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
            Inputs.Add(inputSkuno);
            Inputs.Add(inputStation);

            string sqlGetSkuno = $@"select distinct skuno from c_sku where id in (
                                    select sku_id from r_sku_route where route_id in (
                                    select distinct route_id from c_route_detail where station_name in (select mes_station from c_temes_station_mapping)))
                                    order by skuno";
            string sqlGetStation = $@"select distinct mes_station as station_name  from c_temes_station_mapping  order by mes_station";
            Sqls.Add("GetSkuno", sqlGetSkuno);
            Sqls.Add("GetStation", sqlGetStation);
        }

        public override void Init()
        {
            inputDateFrom.Value = DateTime.Now.AddDays(-7);//.ToString("yyyy-MM-dd");
            inputDateTo.Value = DateTime.Now;//.ToString("yyyy-MM-dd");

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtSkuno = SFCDB.RunSelect(Sqls["GetSkuno"]).Tables[0];
            DataTable dtStation = SFCDB.RunSelect(Sqls["GetStation"]).Tables[0];
            
            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            List<string> skunoList = new List<string>();
            skunoList.Add("ALL");
            foreach (DataRow row in dtSkuno.Rows)
            {
                skunoList.Add(row["skuno"].ToString());
            }
            
            List<string> stationList = new List<string>();
            stationList.Add("SMT1");
            stationList.Add("SMT2");
            stationList.Add("5DX");
            foreach (DataRow row in dtStation.Rows)
            {
                stationList.Add(row["station_name"].ToString());
            }

            inputSkuno.Value= skunoList[0];
            inputSkuno.ValueForUse = skunoList;
            inputStation.Value = stationList[0];
            inputStation.ValueForUse = stationList;
        }

        public override void Run()
        {
            if (inputDateFrom.Value == null || inputDateTo.Value == null)
            {
                throw new Exception("Please Input DateFrom And DateTo!");
            }
            string skuno = inputSkuno.Value.ToString();
            string station = inputStation.Value.ToString();
            //niem-jk 2021-01-30 convert datetime for use many region

            // hh 為12小時 HH 為24小時制

            //string dateFrom = Convert.ToDateTime(inputDateFrom.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd hh:mm:ss");

            string dateFrom = Convert.ToDateTime(inputDateFrom.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");

            //string dateTo = Convert.ToDateTime(inputDateTo.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd hh:mm:ss");

            string dateTo = Convert.ToDateTime(inputDateTo.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");


            string dateType = inputDateType.Value.ToString();
            string tempSql,skuSql;
            //string skuno = "VT03021150";
            //string station = "ICT";
            //string dateFrom = "2018/6/28";
            //string dateTo = "2018/11/28";
            //string dateType = "ByDay";
            //string tempSql = $@" skuno='{skuno}' and station_name='{station}' and to_date(to_char(edit_time,'YYYY/MM/DD'),'YYYY/MM/DD') between to_date('{dateFrom}','YYYY/MM/DD') and  to_date('{dateTo}','YYYY/MM/DD') ";

            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                string bu = SFCDB.ORM.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();

                string sqlCount = bu == "VNDCN" ? "COUNT(distinct sn)": bu == "VNJUNIPER" ? "COUNT(distinct sn)" : "count(*)";

                if (skuno.ToUpper().Equals("ALL"))
                {
                    skuSql = $@" skuno in (select distinct skuno from c_sku where id in (
                                    select sku_id from r_sku_route where route_id in (
                                    select distinct route_id from c_route_detail where station_name in (select mes_station from c_temes_station_mapping))))";
                }
                else
                {
                    skuSql = $@" skuno='{skuno}'  ";
                }
                if (station == "SMT1" || station == "SMT2" )
                {
                    tempSql = $@" {skuSql}  and station_name='{station}' and edit_time between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";
                }
                else
                {

                    //tempSql = $@" sn in (select sn from r_sn where {skuSql} ) and messtation='{station}' and starttime between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";

                    tempSql = $@" sn in (select sn from r_sn where {skuSql}   and VALID_FLAG = 1 ) and messtation='{station}' and starttime between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS')    ";
                }
                string runSql = "";
                switch (dateType)
                {

                    case "ByHours":
                        if (station == "SMT1" || station == "SMT2" )
                        {
                            runSql = $@"select to_char(edit_time,'YYYY/MM/DD HH24') as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                                from r_sn_station_detail where {tempSql} group by to_char(edit_time,'YYYY/MM/DD HH24'),repair_failed_flag ORDER BY datetime";
                            //
                        }
                        else
                        {
                            if (bu == "VNJUNIPER") //VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                            {
                                runSql = $@"select to_char(starttime,'YYYY/MM/DD HH24') as datetime,   {sqlCount} as qty, decode(state, 'P', 'PASS', 'F', 'FAIL', state) as status
                                                      from (select *
                                                              from (select a.*,
                                                                           ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                                      from r_test_record a) aa
                                                             where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) aaa
                                                     where {tempSql}
                                                     group by to_char(starttime,'YYYY/MM/DD HH24'), state
                                                     ORDER BY datetime  ";

                            }
                            else
                            {
                                runSql = $@"select to_char(starttime,'YYYY/MM/DD HH24') as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM/DD HH24'),state ORDER BY datetime";
                            }
                        }

                        break;
                    case "ByDay":
                        if (station == "SMT1" || station == "SMT2" )
                        {
                            runSql = $@"select to_char(edit_time,'YYYY/MM/DD') as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                                from r_sn_station_detail where {tempSql} group by to_char(edit_time,'YYYY/MM/DD'),repair_failed_flag ORDER BY datetime";
                        }
                        else
                        {
                            if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                            {
                                runSql = $@"select to_char(starttime,'YYYY/MM/DD') as datetime,   {sqlCount} as qty, decode(state, 'P', 'PASS', 'F', 'FAIL', state) as status
                                                      from (select *
                                                              from (select a.*,
                                                                           ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                                      from r_test_record a) aa
                                                             where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) aaa
                                                     where {tempSql}
                                                     group by to_char(starttime,'YYYY/MM/DD'), state
                                                     ORDER BY datetime  ";

                            }
                            else
                            {
                                runSql = $@"select to_char(starttime,'YYYY/MM/DD') as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM/DD'),state ORDER BY datetime";
                            }
                        }
                        break;
                    case "ByWeeks":
                        if (station == "SMT1" || station == "SMT2" )
                        {
                            runSql = $@"select to_char(edit_time,'IW') || 'Week' as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                                from r_sn_station_detail where {tempSql} group by to_char(edit_time,'IW'),repair_failed_flag ORDER BY datetime";
                        }
                        else
                        {
                            if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                            {
                                runSql = $@"select to_char(starttime,'IW') || 'Week' as datetime,   {sqlCount} as qty, decode(state, 'P', 'PASS', 'F', 'FAIL', state) as status
                                                      from (select *
                                                              from (select a.*,
                                                                           ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                                      from r_test_record a) aa
                                                             where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) aaa
                                                     where {tempSql}
                                                     group by to_char(starttime,'IW'), state
                                                     ORDER BY datetime  ";

                            }
                            else
                            {
                                runSql = $@"select to_char(starttime,'IW') || 'Week' as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'IW'),state ORDER BY datetime";
                            }
                        }
                        break;
                    case "ByMonth":
                        if (station == "SMT1" || station == "SMT2" )
                        {
                            runSql = $@"select to_char(edit_time,'YYYY/MM') as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                                where {tempSql} group by to_char(edit_time,'YYYY/MM'),repair_failed_flag  ORDER BY datetime";
                        }
                        else
                        {
                            if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                            {
                                runSql = $@"select  to_char(starttime,'YYYY/MM') as datetime,   {sqlCount} as qty, decode(state, 'P', 'PASS', 'F', 'FAIL', state) as status
                                                      from (select *
                                                              from (select a.*,
                                                                           ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                                      from r_test_record a) aa
                                                             where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) aaa
                                                     where {tempSql}
                                                     group by to_char(starttime,'YYYY/MM'), state
                                                     ORDER BY datetime  ";

                            }
                            else
                            {
                                runSql = $@"select to_char(starttime,'YYYY/MM') as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM'),state ORDER BY datetime";
                            }
                        }
                        break;
                    case "ByQuarter":
                        if (station == "SMT1" || station == "SMT2" )
                        {
                            runSql = $@"select to_char(edit_time,'q') || 'Quarter' as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                                where {tempSql} group by to_char(edit_time,'q'),repair_failed_flag  ORDER BY datetime";
                        }
                        else
                        {
                            if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                            {
                                runSql = $@"select   to_char(starttime,'q') || 'Quarter' as datetime,   {sqlCount} as qty, decode(state, 'P', 'PASS', 'F', 'FAIL', state) as status
                                                      from (select *
                                                              from (select a.*,
                                                                           ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                                      from r_test_record a) aa
                                                             where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) aaa
                                                     where {tempSql}
                                                     group by to_char(starttime,'q'), state
                                                     ORDER BY datetime  ";

                            }
                            else
                            {
                                runSql = $@"select to_char(starttime,'q') || 'Quarter' as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'q'),state ORDER BY datetime";
                            }
                        }
                        break;
                    case "ByYears":
                        if (station == "SMT1" || station == "SMT2" )
                        {
                            runSql = $@"select to_char(edit_time,'YYYY') || 'Year' as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                                where {tempSql} group by to_char(edit_time,'YYYY'),repair_failed_flag  ORDER BY datetime";
                        }
                        else
                        {
                            if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                            {
                                runSql = $@"select  to_char(starttime,'YYYY') || 'Year' as datetime,   {sqlCount} as qty, decode(state, 'P', 'PASS', 'F', 'FAIL', state) as status
                                                      from (select *
                                                              from (select a.*,
                                                                           ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                                      from r_test_record a) aa
                                                             where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) aaa
                                                     where {tempSql}
                                                     group by to_char(starttime,'YYYY'), state
                                                     ORDER BY datetime  ";

                            }
                            else
                            {
                                runSql = $@"select to_char(starttime,'YYYY') || 'Year' as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY'),state ORDER BY datetime";
                            }
                        }
                        break;
                    default:
                        throw new Exception("DateType Error!");
                }
                RunSqls.Add(runSql);

                DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("SkuNo:[" + skuno + "], Station:[" + station + "] No Data!");
                }

                //根據機種工站取得QE設置的達成率,未設置的默認98：QE謝志明要求不是所有機種達成率都是98 Edit By ZHB 2020年9月23日15:16:03
                double targetValue = 98;
                var targetSql = $@"
                select b.value
                  from r_function_control a, r_function_control_ex b
                 where a.id = b.detail_id
                   and a.functionname = 'FPYREPORT_TARGET'
                   and a.controlflag = 'Y'
                   and a.functiontype = 'NOSYSTEM'
                   and b.name = 'TARGET'
                   and a.value = '{skuno}'
                   and a.extval = '{station}'";
                var targetDt = SFCDB.RunSelect(targetSql).Tables[0];                
                if (targetDt.Rows.Count > 0)
                {
                    targetValue = !string.IsNullOrEmpty(targetDt.Rows[0][0].ToString()) ? Convert.ToDouble(targetDt.Rows[0][0]) : targetValue;
                }

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }               

                List<FPYReportModel> listFPY = new List<FPYReportModel>();
                string date = "";
                int passQty = 0;
                int failQty = 0;

                DataTable dateTimeTable = dt.DefaultView.ToTable(true, "datetime");
                DataRow[] passRow;
                DataRow[] failRow;
                foreach (DataRow row in dateTimeTable.Rows)
                {
                    date = row["datetime"].ToString();
                    passRow = dt.Select(" status= 'PASS' and datetime='" + row["datetime"].ToString() + "'");
                    failRow = dt.Select(" status= 'FAIL' and datetime='" + row["datetime"].ToString() + "'");
                    passQty = passRow.Count() > 0 ? Convert.ToInt32(passRow[0]["QTY"].ToString()) : 0;
                    failQty = failRow.Count() > 0 ? Convert.ToInt32(failRow[0]["QTY"].ToString()) : 0;
                    listFPY.Add(new FPYReportModel(date, passQty, failQty, targetValue));
                }
                DataTable FPYTable = new DataTable();
                DataTable FPYLinkTable = new DataTable();
                FPYTable.Columns.Add(new DataColumn("Item\\Date"));
                FPYLinkTable.Columns.Add(new DataColumn("Item\\Date"));
                List<object> passData = new List<object>();
                List<object> failData = new List<object>();
                List<object> totalData = new List<object>();

                List<object> fpyData = new List<object>();
                //List<object> avgData = new List<object>();
                List<object> goalsData = new List<object>();

                List<string> categories = new List<string>();           
                foreach (FPYReportModel fpy in listFPY)
                {
                    FPYTable.Columns.Add(new DataColumn(fpy.Date));
                    FPYLinkTable.Columns.Add(new DataColumn(fpy.Date));
                    passData.Add(fpy.PassQty);
                    failData.Add(fpy.FailQty);
                    totalData.Add(fpy.TotalQty);
                    categories.Add(fpy.Date);
                    fpyData.Add(fpy.FPYRate);
                    //avgData.Add(fpy.FailRate);
                    goalsData.Add(fpy.Goals);
                }                
                #region Show Table
                List<ItemModel> listItem = new List<ItemModel>();
                listItem.Add(new ItemModel { Name = "TotalQty", TempName = "Total Qty(pcs)" });
                listItem.Add(new ItemModel { Name = "PassQty", TempName = "Pass Qty(pcs)" });
                listItem.Add(new ItemModel { Name = "FailQty", TempName = "Fail Qty(pcs)" });
                listItem.Add(new ItemModel { Name = "Goals", TempName = "Goals(%)" });
                listItem.Add(new ItemModel { Name = "FPYRate", TempName = "FPY Rate(%)" });               
                DataRow dr,linkDr;
                string linkUrl = $@"Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.FPYReportDetail&RunFlag=1&Skuno={skuno}&Station={station}&DateType={dateType}&DateFrom={dateFrom}&DateTo={dateTo}";
                foreach (ItemModel im in listItem)
                {
                    dr = FPYTable.NewRow();
                    linkDr = FPYLinkTable.NewRow();
                    dr["Item\\Date"] = im.TempName;
                    linkDr["Item\\Date"] = "";
                    for (int j = 1; j < FPYTable.Columns.Count ; j++)
                    {   
                        foreach (FPYReportModel f in listFPY)
                        {
                            if (FPYTable.Columns[j].ColumnName == f.Date )
                            {
                                switch (im.Name)
                                {
                                    case "TotalQty":
                                        dr[FPYTable.Columns[j].ColumnName] = f.TotalQty.ToString();
                                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求TotalQty 也要求顯示出來
                                        {
                                            linkDr[FPYTable.Columns[j].ColumnName] = f.TotalQty == 0 ? "" : linkUrl + $@"&Status=ALL&Date={FPYTable.Columns[j].ColumnName}";
                                        }
                                        else
                                        {

                                            linkDr[FPYTable.Columns[j].ColumnName] = "";

                                        }

                                        break;
                                    case "PassQty":
                                        dr[FPYTable.Columns[j].ColumnName] = f.PassQty.ToString();
                                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求PassQty也要求顯示出來
                                        {
                                            linkDr[FPYTable.Columns[j].ColumnName] = f.PassQty == 0 ? "" : linkUrl + $@"&Status=PASS&Date={FPYTable.Columns[j].ColumnName}";
                                        }
                                        else
                                        {
                                            linkDr[FPYTable.Columns[j].ColumnName] = "";
                                        }
                                        break;
                                    case "FailQty":
                                        dr[FPYTable.Columns[j].ColumnName] = f.FailQty.ToString();
                                        //linkDr[FPYTable.Columns[j].ColumnName] = f.FailQty == 0 ? "" : linkUrl + $@"&&Status=FAIL&&Date=";
                                        linkDr[FPYTable.Columns[j].ColumnName] = f.FailQty == 0 ? "" : linkUrl + $@"&Status=FAIL&Date={FPYTable.Columns[j].ColumnName}";
                                        break;
                                    case "Goals":
                                        dr[FPYTable.Columns[j].ColumnName] = f.Goals.ToString();
                                        linkDr[FPYTable.Columns[j].ColumnName] = "";
                                        break;
                                    case "FPYRate":
                                        dr[FPYTable.Columns[j].ColumnName] = f.FPYRate;
                                        linkDr[FPYTable.Columns[j].ColumnName] = "";
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    FPYTable.Rows.Add(dr);
                    FPYLinkTable.Rows.Add(linkDr);
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(FPYTable,FPYLinkTable);
                retTab.Tittle = $@"{skuno} Daily Realtime {station} FPY Report";
                Outputs.Add(retTab);
                #endregion

                #region Show Line Chart
                List<ChartData> listLine = new List<ChartData>();
                listLine.Add(new ChartData { name = "FPY", data = fpyData, type = ChartType.line.ToString() });
                listLine.Add(new ChartData { name = "Goals", data = goalsData, type = ChartType.line.ToString() });
                //listLine.Add(new ChartData { name = "MOV AVG", data = avgData, type = ChartType.line.ToString() });
                LineChart retChart_line = new LineChart();
                XAxis lineXAxis = new XAxis();
                lineXAxis.Title = "";
                lineXAxis.XAxisType = XAxisType.datetime;
                lineXAxis.Categories = categories;
                Yaxis lineYAxis = new Yaxis();
                lineYAxis.Title = "Rate(%)";
                PlotOptions PlotOptions = new PlotOptions();
                PlotOptions.type = PlotType.datetime;
                PlotOptions.pointStartDateTime = DateTime.Now;
                PlotOptions.pointInterval = 3600000;
                retChart_line.Plot = PlotOptions;
                retChart_line.XAxis = lineXAxis;
                retChart_line.YAxis = lineYAxis;
                retChart_line.ChartDatas = listLine;
                retChart_line.Tittle = $@"{skuno} Daily Realtime {station} FPY Line Charts Report";
                Outputs.Add(retChart_line);
                #endregion

                #region Show Column Chart
                List<ChartData> listColumn = new List<ChartData>();
                listColumn.Add(new ChartData { name = "FAIL", data = failData, type = ChartType.column.ToString() });
                listColumn.Add(new ChartData { name = "PASS", data = passData, type = ChartType.column.ToString() });
                columnChart retChart_column = new columnChart();
                XAxis columnXAxis = new XAxis();
                columnXAxis.Title = "";
                columnXAxis.XAxisType = XAxisType.initValue;
                columnXAxis.Categories = categories;
                Yaxis columnYAxis = new Yaxis();
                columnYAxis.Title = "Qty(pcs)";               
                retChart_column.XAxis = columnXAxis;
                retChart_column.YAxis = columnYAxis;
                retChart_column.ChartDatas = listColumn;
                retChart_column.Tittle = $@"{skuno} Daily Realtime {station} FPY Column Charts Report";
                Outputs.Add(retChart_column);
                #endregion               
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                Outputs.Add(new ReportAlart(exception.Message));
            }

        }

        public class FPYReportModel
        {
            private string _date = "";
            private double _fpyRate;
            private double _failRate;
            private int _totalQty;
            private int _passQty = 0;
            private int _failQty = 0;
            private double _goals;

            public string Date
            {
                get { return _date; }               
            }
            public int TotalQty
            {
                get { return _totalQty; }
            }
            public int PassQty
            {
                get { return _passQty; }                
            }
            public int FailQty
            {
                get { return _failQty; }               
            }
            public double Goals
            {
                //get { return 98; }  //QE謝志明要求不是所有機種達成率都是98 Edit By ZHB 2020年9月23日15: 16:03
                get { return _goals; }
            }
            public double FPYRate
            {                
                get { return _fpyRate; }
            }
            public double FailRate
            {              
                get { return _failRate; }
            }

            public FPYReportModel(string date, int passQty, int failQty, double goals)
            {
                _date = date;
                _passQty = passQty;
                _failQty = failQty;
                _totalQty = _passQty + _failQty;
                _failRate = Math.Round(Convert.ToDouble(_failQty) / Convert.ToDouble(_totalQty) * 100, 2);
                _fpyRate = Math.Round(Convert.ToDouble(_passQty) / Convert.ToDouble(_totalQty) * 100, 2);
                _goals = goals; 
            }
        }

        public class ItemModel
        {
            public string Name { get; set; }
            public string TempName { get; set; }
        }
    }
}
