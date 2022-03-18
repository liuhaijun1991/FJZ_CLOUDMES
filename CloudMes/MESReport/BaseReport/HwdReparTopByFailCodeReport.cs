using MESDBHelper;
using MESReport.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="HwdReparTopByFailCodeReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// HwdReparTopByFailCodeReport
    /// </summary>
    public class HwdReparTopByFailCodeReport:ReportBase
    {
        ReportInput SelectBy = new ReportInput()
        {
            Name = "SelectBy",
            InputType = "Select",
            // Value = "日期段",
            Value = "TimeSlot",
            Enable = true,
            SendChangeEvent = false,
           // ValueForUse = new string[] { "日期段", "月", "周" }
            ValueForUse = new string[] { "TimeSlot", "Month", "Week" }
        };
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput month = new ReportInput() { Name = "Month", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput week = new ReportInput() { Name = "Week", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public HwdReparTopByFailCodeReport()
        {
            Inputs.Add(SelectBy);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            Inputs.Add(month);
            Inputs.Add(week);
        }

        public override void Init()
        {
            // if (SelectBy.Value.ToString() == "日期段")
            if (SelectBy.Value.ToString() == "TimeSlot")
            {
                startTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 08:00:00";
                endTime.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 08:00:00";
                month.Value = "";
                week.Value = "";
            }
            // else if (SelectBy.Value.ToString() == "月")
            else if (SelectBy.Value.ToString() == "Month")
            {
                startTime.Value = "";
                endTime.Value = "";
                month.Value = DateTime.Now.Month.ToString();
                week.Value = "";
            }
            // else if (SelectBy.Value.ToString() == "周")
            else if (SelectBy.Value.ToString() == "Week")
            {
                startTime.Value = "";
                endTime.Value = "";
                month.Value = "";
                week.Value = ConverDate.GetWeekFromDate(DateTime.Now);
            }
        }

        public override void Run()
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTO = DateTime.Now;
            DataTable dtFailCode = new DataTable();
            DataTable loadTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow loadTitleRow = null;
            DataRow loadDataRow = null;
            DataRow linkTitleRow = null;
            DataRow linkDataRow = null;
            string failCode = "";
            int col = 0;
            string sqlRun = "";
            // if (SelectBy.Value.ToString() == "日期段" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            if (SelectBy.Value.ToString() == "TimeSlot" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            {
                dateFrom = (DateTime)startTime.Value;
                dateTO = (DateTime)endTime.Value;
            }
            //else if (SelectBy.Value.ToString() == "月" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            else if (SelectBy.Value.ToString() == "Month" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            {
                if (string.IsNullOrEmpty(month.Value.ToString()))
                {
                    throw new Exception("Please input month");
                }
                dateFrom = DateTime.Parse(DateTime.Now.Year.ToString() + "-" + month.Value + "-" + "01 08:00:00");
                dateTO = DateTime.Parse(DateTime.Now.Year.ToString() + "-" + month.Value + "-" + "01 08:00:00").AddMonths(1);
            }
            //else if (SelectBy.Value.ToString() == "周" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            else if (SelectBy.Value.ToString() == "Week" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            {
                if (string.IsNullOrEmpty(week.Value.ToString()))
                {
                    throw new Exception("Please input week");
                }
                dateFrom = ConverDate.GetWeekStartDate(Convert.ToInt32(week.Value.ToString()));
                dateTO = dateFrom.AddDays(7);
            }
            sqlRun = $@"select c.fail_code, d.sku_name , count(1) count
                          from r_repair_main a
                         inner join r_sn b
                            on a.sn = b.sn
                         inner join r_repair_failcode c
                            on c.sn = a.sn
                           and c.create_time = a.create_time
                         inner join c_sku d
                            on d.skuno = b.skuno
                         where a.closed_flag = 1
                           and a.edit_time between
                               to_date('{dateFrom.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy-MM-dd hh24:mi:ss') and
                               to_date('{dateTO.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy-MM-dd hh24:mi:ss')
                           and c.fail_category = 'DEFECT'
                         group by d.sku_name, c.fail_code
                         order by c.fail_code, count(1) desc";

            #region 原報表查詢語句
            //select c.failcode, d.codename , count(1) count
            //  from sfcrepairmain A
            // inner join mfworkstatus B
            //    on a.sysserialno = b.sysserialno
            // inner join sfcrepairfailcode C
            //    on C.SYSSERIALNO = a.sysserialno
            //   and c.createdate = a.createdate
            // inner join sfccodelike d
            //    on d.skuno = b.skuno
            // where a.repaired = 1
            //   and a.lasteditdt between
            //       to_date('', 'yyyy-MM-dd hh24:mi:ss') and
            //       to_date('', 'yyyy-MM-dd hh24:mi:ss')
            //   and C.failcategory = 'DEFECT'
            // group by d.codename, c.failcode
            // order by c.failcode, count(1) desc
            #endregion

            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                dtFailCode = SFCDB.RunSelect(sqlRun).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                for (int top = 0; top <= 11; top++)
                {
                    if (top == 0)
                    {
                        loadTable.Columns.Add("FAIL_CODE");
                        linkTable.Columns.Add("FAIL_CODE");
                    }
                    else if (top == 1)
                    {
                        loadTable.Columns.Add("TOTAL");
                        linkTable.Columns.Add("TOTAL");
                    }
                    else
                    {
                        loadTable.Columns.Add("TOP" + (top - 1).ToString());
                        linkTable.Columns.Add("TOP" + (top - 1).ToString());
                    }
                } 

                for (int i = 0; i < dtFailCode.Rows.Count; i++)
                {
                    if (failCode != dtFailCode.Rows[i]["fail_code"].ToString())
                    {
                        failCode = dtFailCode.Rows[i]["fail_code"].ToString();
                        loadTitleRow = loadTable.NewRow();                        
                        loadDataRow = loadTable.NewRow();
                        linkTitleRow = linkTable.NewRow();
                        linkDataRow = linkTable.NewRow();
                        loadTitleRow["FAIL_CODE"] = failCode;
                        col = 0;
                        loadTable.Rows.Add(loadTitleRow);
                        loadTable.Rows.Add(loadDataRow);
                        linkTable.Rows.Add(linkTitleRow);
                        linkTable.Rows.Add(linkDataRow);
                        loadDataRow["TOTAL"] = 0;
                        linkTitleRow["FAIL_CODE"] = "MESReport.BaseReport.RepairFailCodeDetail";
                    }
                    col++;
                    if (col > 10)
                    {
                        continue;
                    }
                    loadTitleRow[col + 1] = dtFailCode.Rows[i]["sku_name"].ToString();
                    loadDataRow[col + 1] = dtFailCode.Rows[i]["count"].ToString();
                    loadDataRow["TOTAL"] = (Int32.Parse(loadDataRow["TOTAL"].ToString())) + Int32.Parse(dtFailCode.Rows[i]["count"].ToString());
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(loadTable, linkTable);
                reportTable.Tittle = "RepairTopByFailTable";                
                Outputs.Add(reportTable);                
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
