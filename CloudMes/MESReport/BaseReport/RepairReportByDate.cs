
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class RepairReportByDate : ReportBase
    {
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairReportByDate()
        {
            Inputs.Add(startTime);
            Inputs.Add(endTime);
        }
        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            endTime.Value = DateTime.Now.ToString("yyyy-MM-dd");
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
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
            endDT = endDT.AddDays(1);
            string dateFrom = startDT.ToString("yyyy-MM-dd");
            string dateTo = endDT.ToString("yyyy-MM-dd");
            string sql = $@"with finalTable as(
                            select TO_CHAR(A.IN_TIME,'DD-MM-YYYY') AS IN_DAY,TO_CHAR(A.OUT_TIME,'DD-MM-YYYY') AS OUT_DAY from R_REPAIR_TRANSFER A
                            where out_time between to_date('{dateFrom}', 'yyyy-MM-dd') and to_date('{dateTo}', 'yyyy-MM-dd')                          
                            and in_time between to_date('{dateFrom}', 'yyyy-MM-dd') and to_date('{dateTo}', 'yyyy-MM-dd')
                            and A.sn not like 'RW%'
                            and substr(A.sn, 1, 1) not in ('*', '#', '~')
                            )select IN_DAY,OUT_DAY,count(OUT_DAY) as TotalDay from finalTable
                            group by IN_DAY,OUT_DAY
                            order by  to_date(IN_DAY, 'dd-MM-yyyy'),to_date(OUT_DAY, 'dd-MM-yyyy')";
            string sqlGetDate = $@"select Day,count(Day) as Total from(
                                    select TO_CHAR(A.IN_TIME,'DD-MM-YYYY')  as Day from R_REPAIR_TRANSFER A                           
                                    where A.in_time between to_date('{dateFrom}', 'yyyy-MM-dd') and to_date('{dateTo}', 'yyyy-MM-dd')
                                    and A.sn not like 'RW%'
                                    and substr(A.sn, 1, 1) not in ('*', '#', '~')
                                    )
                                    group by Day
                                    order by to_date(Day, 'DD-MM-YYYY')";
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dt = null;
            DataTable dtDay = null;
            DataTable linkTable = new DataTable();
            try
            {
                dtDay = SFCDB.RunSelect(sqlGetDate).Tables[0];
                dt = SFCDB.RunSelect(sql).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportTable reportTable = new ReportTable();
                reportTable = GetReportTable(dtDay, dt);
                reportTable.Tittle = "RepairReportByDate";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
        public ReportTable GetReportTable(DataTable DataDate, DataTable DataMain)
        {
            ReportTable reportTable = new ReportTable();
            reportTable.ColNames.Add("1");
            reportTable.ColNames.Add("2");

            TableRowView rowView = new TableRowView();
            TableColView colView = new TableColView();
            int colName = 2;
            int nextRowDate = 0;
            string OrderDate = "";
            Dictionary<string, object> dData = new Dictionary<string, object>();

            #region Add number column
            for (int i = 1; i <= DataDate.Rows.Count * 2; i++)
            {
                colName++;
                reportTable.ColNames.Add(colName.ToString());
            }
            #endregion

            #region Add title
            for (int i = 1; i <= DataDate.Rows.Count + 2; i++)
            {
                int nextColumn = 3;
                int nextDate = 0;
                int nextDate1 = 0;
                float sumTotal = 0;
                float Total = 0;
                bool WriteFlag = false;
                rowView = new TableRowView();
                foreach (string col_name in reportTable.ColNames)
                {
                    colView = new TableColView();
                    colView.ColName = col_name;
                    if (i == 1)
                    {
                        colView.CellStyle = new Dictionary<string, object>() {
                                { "background", "#cccccc" },
                                { "color", "#000" },
                                { "font-size", "15px" },
                                { "font-weight","bold"}
                            };
                        if (col_name == "1")
                        {
                            colView.Value = "Check In Day";
                            colView.ColSpan = 1;
                            colView.RowSpan = 2;
                        }
                        else if (col_name == "2")
                        {
                            colView.Value = "Check In Qty";
                            colView.ColSpan = 1;
                            colView.RowSpan = 2;
                        }
                        else if (int.Parse(col_name) == nextColumn)
                        {
                            colView.Value = $@"Check Out({DataDate.Rows[nextDate]["DAY"]})";
                            colView.ColSpan = 2;
                            colView.RowSpan = 1;
                            nextColumn += 2;
                            nextDate++;
                        }
                        else
                        {
                            colView.ColSpan = 0;
                            colView.RowSpan = 0;
                        }

                    }
                    else if (i == 2)
                    {
                        colView.CellStyle = new Dictionary<string, object>() {
                                { "background", "#cccccc" },
                                { "color", "#000" },
                                { "font-size", "15px" },
                                { "font-weight","bold"}
                            };
                        if (col_name == "1" || col_name == "2")
                        {
                            colView.ColSpan = 0;
                            colView.RowSpan = 0;
                        }
                        else if (int.Parse(col_name) == nextColumn)
                        {
                            colView.Value = "QTY";
                            colView.ColSpan = 1;
                            colView.RowSpan = 1;
                            nextColumn += 2;
                        }
                        else
                        {
                            colView.Value = "Rate";
                            colView.ColSpan = 1;
                            colView.RowSpan = 1;
                        }
                    }
                    else
                    {

                        if (col_name == "1" || col_name == "2")
                        {
                            if (col_name == "1")
                            {
                                OrderDate = DataDate.Rows[nextRowDate]["DAY"].ToString();
                                colView.Value = $@"{DataDate.Rows[nextRowDate]["DAY"]}";
                            }
                            else if (col_name == "2")
                            {
                                sumTotal = float.Parse(DataDate.Rows[nextRowDate]["Total"].ToString());
                                colView.Value = $@"{(int)sumTotal}";
                            }

                            colView.ColSpan = 1;
                            colView.RowSpan = 1;
                        }
                        else if (int.Parse(col_name) == nextColumn)
                        {
                            for (int h = 0; h < DataMain.Rows.Count; h++)
                            {
                                if (DataMain.Rows[h]["IN_DAY"].ToString() == OrderDate && DataMain.Rows[h]["OUT_DAY"].ToString() == DataDate.Rows[nextDate1]["DAY"].ToString())
                                {
                                    Total += float.Parse(DataMain.Rows[h]["TotalDay"].ToString());
                                    colView.Value = DataMain.Rows[h]["TotalDay"].ToString();
                                    colView.LinkType = "Link";
                                    DateTime In_Day=DateTime.ParseExact(OrderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                    DateTime Out_Day = DateTime.ParseExact(DataMain.Rows[h]["OUT_DAY"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                    colView.LinkData = "/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.RepairReportByDayInAnddayOut&RunFlag=1&InDay=" + In_Day.ToString("yyyy-MM-dd") + "&OutDay=" + Out_Day.ToString("yyyy-MM-dd"); ;
                                    WriteFlag = true;
                                    break;
                                }
                                else { colView.Value = null; }
                            }
                            colView.ColSpan = 1;
                            colView.RowSpan = 1;
                            nextColumn += 2;
                            nextDate1++;
                        }
                        else
                        {
                            if (WriteFlag)
                            {
                                decimal Rate = (decimal)(Total / sumTotal) * 100;
                                colView.Value = (Math.Round(Rate, 2)).ToString() + "%";
                                if (Rate == 100)
                                    colView.CellStyle = new Dictionary<string, object>() {
                                            { "background", "#2ECC71" },
                                            { "color", "#000" }
                                    };
                                else
                                    colView.CellStyle = new Dictionary<string, object>() {
                                            { "background", "#F4D03F" },
                                            { "color", "#000" }
                                    };
                                WriteFlag = false;
                            }
                            colView.ColSpan = 1;
                            colView.RowSpan = 1;
                        }
                        if (col_name == "2" && i < DataDate.Rows.Count + 2)
                            nextRowDate++;
                    }
                    rowView.RowData.Add(colView.ColName, colView);
                }
                #endregion

                #region Drow Table                
                reportTable.Rows.Add(rowView);
                #endregion
            }
            return reportTable;
        }
    }
}
