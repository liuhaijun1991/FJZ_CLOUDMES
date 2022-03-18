using System;
using MESDBHelper;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class YieldReportJNPR : ReportBase
    {
        ReportInput inputTestType = new ReportInput() { Name = "TestType", InputType = "Select", Value = "Structural", Enable = true, SendChangeEvent = true, ValueForUse = new string[] { "Structural", "Functional" } };
        ReportInput inputDateType = new ReportInput() { Name = "DateType", InputType = "Select", Value = "ByDay", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ByDay", "ByWeeks", "ByMonth" } };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "2021-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = false, SendChangeEvent = false, ValueForUse = null };


        public YieldReportJNPR()
        {

            Inputs.Add(inputTestType);
            Inputs.Add(inputDateType);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
            Inputs.Add(inputSkuno);
            Inputs.Add(inputStation);

            string sqlGetStation = $@"SELECT DISTINCT NEXT_sTATION AS station_name
                                    FROM SFCRUNTIME.R_SN_STATION_DETAIL C
                                    WHERE EXISTS (SELECT 1 FROM SFCBASE.C_TEMES_STATION_MAPPING
                                                        WHERE C.NEXT_STATION = MES_STATION) AND (WORKORDERNO LIKE '0091%' OR WORKORDERNO LIKE '0093%') ORDER BY NEXT_STATION ASC";
            Sqls.Add("GetStation", sqlGetStation);
        }

        public override void Init()
        {
            inputDateFrom.Value = DateTime.Now.AddDays(-7);//.ToString("yyyy-MM-dd");
            inputDateTo.Value = DateTime.Now;//.ToString("yyyy-MM-dd");

            inputStation.ValueForUse = null;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtStation = SFCDB.RunSelect(Sqls["GetStation"]).Tables[0];

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

            ChangeTestType();
        }


        public override void InputChangeEvent()
        {
            base.InputChangeEvent();
            ChangeTestType();
        }

        private void ChangeTestType()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            #region Init Input Data List
            try
            {
                if (inputTestType.Value.ToString() == "Structural")
                {

                    inputStation.ValueForUse = null;

                    DataTable dtStation = SFCDB.RunSelect(Sqls["GetStation"]).Tables[0];

                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }

                    List<string> stationList = new List<string>();
                    //stationList.Add("ALL");

                    foreach (DataRow row in dtStation.Rows)
                    {
                        stationList.Add(row["station_name"].ToString());
                    }
                    inputStation.Value = stationList[0];
                    inputStation.ValueForUse = stationList;


                }

                if (inputTestType.Value.ToString() == "Functional")
                {
                    string sqlGetFuncStation = $@"SELECT DISTINCT NEXT_sTATION AS station_name
                                    FROM SFCRUNTIME.R_SN_STATION_DETAIL C
                                    WHERE EXISTS (SELECT 1 FROM SFCBASE.C_TEMES_STATION_MAPPING
                                                        WHERE C.NEXT_STATION = MES_STATION) AND (WORKORDERNO LIKE '0092%' OR WORKORDERNO LIKE '0094%' OR WORKORDERNO LIKE '006%' or WORKORDERNO LIKE '0095%') ORDER BY NEXT_STATION ASC";
                    Sqls.Add("GetFuncStation", sqlGetFuncStation);

                    inputStation.ValueForUse = null;
                    DataTable dtStation = SFCDB.RunSelect(Sqls["GetFuncStation"]).Tables[0];
                    List<string> stationList = new List<string>();
                    //stationList.Add("ALL");
                    stationList.Add("VI");
                    stationList.Add("VI1");
                    stationList.Add("VI2");

                    foreach (DataRow row in dtStation.Rows)
                    {
                        stationList.Add(row["station_name"].ToString());
                    }

                    inputStation.Value = stationList[0];
                    inputStation.ValueForUse = stationList;


                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            #endregion
        }

        //----------------------------------------------------------------NEW-----------------------------------------
        public override void Run()
        {
            if (inputDateFrom.Value == null || inputDateTo.Value == null)
            {
                throw new Exception("Please Input DateFrom And DateTo!");
            }
            string skuno = inputSkuno.Value.ToString();
            string station = inputStation.Value.ToString();
            //niem-jk 2021-01-30 convert datetime for use many region
            string dateFrom = Convert.ToDateTime(inputDateFrom.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");
            string dateTo = Convert.ToDateTime(inputDateTo.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");
            string dateType = inputDateType.Value.ToString();
            string tempSql, skuSql;


            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                string bu = SFCDB.ORM.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();

                string sqlCount = bu == "VNDCN" ? "COUNT(distinct sn)" : bu == "VNJUNIPER" ? "COUNT(distinct sn)" : "count(*)";

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

                //if (station == "SMT1" || station == "SMT2" )
                //{
                //    tempSql = $@" {skuSql}  and station_name='{station}' and edit_time between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";
                //}
                //else
                //{
                    tempSql = $@" sn in (select sn from r_sn where {skuSql} ) and messtation='{station}' and edit_time between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";
                //}
                string runSql = "";
                switch (dateType)
                {

                    case "ByHours":
                        //if (station == "SMT1" || station == "SMT2" )
                        //{
                        //    runSql = $@"select to_char(edit_time,'YYYY/MM/DD HH24') as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                        //        from r_sn_station_detail where {tempSql} group by to_char(edit_time,'YYYY/MM/DD HH24'),repair_failed_flag ORDER BY datetime";
                        //    //
                        //}
                        //else
                        //{
                            runSql = $@"select to_char(starttime,'YYYY/MM/DD HH24') as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM/DD HH24'),state ORDER BY datetime";
                        //}

                        break;
                    case "ByDay":
                        //if (station == "SMT1" || station == "SMT2" )
                        //{
                        //    runSql = $@"select to_char(edit_time,'YYYY/MM/DD') as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                        //        from r_sn_station_detail where {tempSql} group by to_char(edit_time,'YYYY/MM/DD'),repair_failed_flag ORDER BY datetime";

                        //}
                        //else
                        //{
                            runSql = $@"SELECT TO_CHAR (EDIT_TIME, 'YYYY/MM/DD')  AS DATETIME, {sqlCount} AS QTY,  DECODE (state,  'P', 'PASS',  'F', 'FAIL',  state) AS STATUS FROM ( 
                                      SELECT SN, STATE, EDIT_TIME, MESSTATION, ROW_NUMBER() OVER (PARTITION BY SN ORDER BY EDIT_TIME ASC) AS RN FROM SFCRUNTIME.R_TEST_RECORD
                                       WHERE {tempSql}) WHERE RN = 1 AND STATE <> 'INCOMPLETE' GROUP BY TO_CHAR (EDIT_TIME, 'YYYY/MM/DD'), STATE ORDER BY DATETIME ASC";

                        //}
                        break;
                    case "ByWeeks":
                        //if (station == "SMT1" || station == "SMT2" )
                        //{
                        //    runSql = $@"select to_char(edit_time,'IW') || 'Week' as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                        //        from r_sn_station_detail where {tempSql} group by to_char(edit_time,'IW'),repair_failed_flag ORDER BY datetime";
                        //}
                        //else
                        //{
                            runSql = $@"SELECT TO_CHAR (edit_time, 'IW') || 'Week' AS datetime, {sqlCount} AS qty, DECODE (state,  'P', 'PASS',  'F', 'FAIL',  state) AS status
                                        FROM (SELECT SN, STATE, EDIT_TIME, MESSTATION, ROW_NUMBER () OVER (PARTITION BY SN ORDER BY EDIT_TIME ASC) AS RN
                                        FROM SFCRUNTIME.R_TEST_RECORD WHERE {tempSql}) WHERE RN = 1 GROUP BY TO_CHAR (edit_time, 'IW'), state ORDER BY datetime";
                        //}
                        break;
                    case "ByMonth":
                        //if (station == "SMT1" || station == "SMT2" )
                        //{
                        //    runSql = $@"select to_char(edit_time,'YYYY/MM') as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                        //        where {tempSql} group by to_char(edit_time,'YYYY/MM'),repair_failed_flag  ORDER BY datetime";
                        //}
                        //else
                        //{
                            //runSql = $@"select to_char(starttime,'YYYY/MM') as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                //from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM'),state ORDER BY datetime";

                            runSql = $@"  SELECT TO_CHAR (EDIT_TIME, 'YYYY/MM') AS DATETIME, {sqlCount} AS QTY, DECODE (STATE,  'P', 'PASS',  'F', 'FAIL',  STATE) AS STATUS FROM 
                                    ( SELECT SN, STATE, EDIT_TIME, MESSTATION, ROW_NUMBER () OVER (PARTITION BY SN ORDER BY EDIT_TIME ASC) AS RN FROM R_TEST_RECORD 
                                    WHERE {tempSql}) WHERE RN = 1 GROUP BY TO_CHAR (EDIT_TIME, 'YYYY/MM'), STATE ORDER BY DATETIME";
                        //}
                        break;
                    case "ByQuarter":
                        //if (station == "SMT1" || station == "SMT2" )
                        //{
                        //    runSql = $@"select to_char(edit_time,'q') || 'Quarter' as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                        //        where {tempSql} group by to_char(edit_time,'q'),repair_failed_flag  ORDER BY datetime";
                        //}
                        //else
                        //{
                            runSql = $@"select to_char(starttime,'q') || 'Quarter' as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'q'),state ORDER BY datetime";
                        //}
                        break;
                    case "ByYears":
                        //if (station == "SMT1" || station == "SMT2" )
                        //{
                        //    runSql = $@"select to_char(edit_time,'YYYY') || 'Year' as datetime ,{sqlCount} as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                        //        where {tempSql} group by to_char(edit_time,'YYYY'),repair_failed_flag  ORDER BY datetime";
                        //}
                        //else
                        //{
                            runSql = $@"select to_char(starttime,'YYYY') || 'Year' as datetime ,{sqlCount} as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY'),state ORDER BY datetime";
                        //}
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
                FPYTable.Columns.Add(new DataColumn("Station"));
                FPYTable.Columns.Add(new DataColumn("Item\\Date"));
                FPYLinkTable.Columns.Add(new DataColumn("Item\\Date"));
                FPYLinkTable.Columns.Add(new DataColumn("Station"));

                List<object> passData = new List<object>();
                List<object> failData = new List<object>();
                List<object> totalData = new List<object>();

                List<object> fpyData = new List<object>();
                //List<object> avgData = new List<object>();
                List<object> goalsData = new List<object>();

                //COLUMNS ARE FILLED BY DATE WITH listFPY
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

                int summTotal = totalData.Sum(x => Convert.ToInt32(x));
                int summFail = failData.Sum(x => Convert.ToInt32(x));
                int summPass = passData.Sum(x => Convert.ToInt32(x));
                double summFPY = Math.Round((Convert.ToDouble(summPass) * 100) / Convert.ToDouble(summTotal), 2);


                #region Show Table
                List<ItemModel> skuList = new List<ItemModel>();
                skuList.Add(new ItemModel { Name = "Station", TempName = station });
                List<ItemModel> listItem = new List<ItemModel>();
                listItem.Add(new ItemModel { Name = "TotalQty", TempName = "Total Qty" });
                listItem.Add(new ItemModel { Name = "PassQty", TempName = "Pass Qty" });
                listItem.Add(new ItemModel { Name = "FailQty", TempName = "Fail Qty" });
                listItem.Add(new ItemModel { Name = "Goals", TempName = "Goal (%)" });
                listItem.Add(new ItemModel { Name = "FPYRate", TempName = "FPY Rate(%)" });
                DataRow dr, linkDr, stationDr;
                string linkUrl = $@"Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.FPYReportDetail&RunFlag=1&Skuno={skuno}&Station={station}&DateType={dateType}&DateFrom={dateFrom}&DateTo={dateTo}";
                //stationDr = FPYTable.NewRow();
                //stationDr["Station"] = station;
                //stationDr[FPYTable.Columns[0].ColumnName] = station;
                //FPYTable.Rows.Add(stationDr);
                //FPYLinkTable.Rows.Add("");
                foreach (ItemModel im in listItem) //Rows
                {
                    dr = FPYTable.NewRow();
                    linkDr = FPYLinkTable.NewRow();
                    dr["Item\\Date"] = im.TempName;
                    linkDr["Item\\Date"] = "";

                    //if (listItem[0])
                    //{
                    //dr["Station"] = im.TempName;
                    dr[FPYTable.Columns[0].ColumnName] = station;


                    //FPYTable.mer
                    //}

                    //FPYTable.Rows.Add(dr);
                    //FPYLinkTable.Rows.Add("");

                    for (int k = 1; k < FPYTable.Columns.Count; k++)
                    {
                        foreach (FPYReportModel f in listFPY)
                        {
                            if (FPYTable.Columns[k].ColumnName == f.Date)
                            {
                                switch (im.Name)
                                {
                                    case "TotalQty":
                                        dr[FPYTable.Columns[k].ColumnName] = f.TotalQty.ToString();
                                        linkDr[FPYTable.Columns[k].ColumnName] = "";
                                        break;
                                    case "PassQty":
                                        dr[FPYTable.Columns[k].ColumnName] = f.PassQty.ToString();
                                        linkDr[FPYTable.Columns[k].ColumnName] = f.PassQty == 0 ? "" : linkUrl + $@"&Status=PASS&Date={FPYTable.Columns[k].ColumnName}";
                                        break;
                                    case "FailQty":
                                        dr[FPYTable.Columns[k].ColumnName] = f.FailQty.ToString();
                                        //linkDr[FPYTable.Columns[k].ColumnName] = f.FailQty == 0 ? "" : linkUrl + $@"&&Status=FAIL&&Date=";
                                        linkDr[FPYTable.Columns[k].ColumnName] = f.FailQty == 0 ? "" : linkUrl + $@"&Status=FAIL&Date={FPYTable.Columns[k].ColumnName}";
                                        break;
                                    case "Goals":
                                        dr[FPYTable.Columns[k].ColumnName] = f.Goals.ToString();
                                        linkDr[FPYTable.Columns[k].ColumnName] = "";
                                        break;
                                    case "FPYRate":
                                        dr[FPYTable.Columns[k].ColumnName] = f.FPYRate;
                                        linkDr[FPYTable.Columns[k].ColumnName] = "";
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    FPYTable.Rows.Add(dr);
                    FPYLinkTable.Rows.Add(linkDr);
                    //dr["FPY Summary"] = summTotal;
                    //linkDr["FPY Summary"] = "";
                }

                //Summary Column
                FPYTable.Columns.Add(new DataColumn("FPY Summary"));
                FPYLinkTable.Columns.Add(new DataColumn("FPY Summary"));
                FPYTable.Rows[0]["FPY Summary"] = summTotal;
                FPYTable.Rows[1]["FPY Summary"] = summPass;
                FPYTable.Rows[2]["FPY Summary"] = summFail;
                FPYTable.Rows[3]["FPY Summary"] = "98";
                FPYTable.Rows[4]["FPY Summary"] = summFPY;


                ReportTable retTab = new ReportTable();
                retTab.LoadData(FPYTable, FPYLinkTable);
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
