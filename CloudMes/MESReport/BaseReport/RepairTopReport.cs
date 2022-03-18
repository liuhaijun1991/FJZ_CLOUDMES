using MESDBHelper;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESReport.Common;

namespace MESReport.BaseReport
{
    // <copyright file="RepairTopReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// RepairTopReport
    /// </summary>
    public class RepairTopReport:ReportBase
    {
        //ReportInput SelectBy = new ReportInput()
        //{
        //    Name = "SelectBy",
        //    InputType = "Select",
        //    Value = "日期段",
        //    Enable = true,
        //    SendChangeEvent = false,
        //    ValueForUse = new string[] { "日期段", "月","周" }
        //};
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput month = new ReportInput() { Name = "Month", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput week = new ReportInput() { Name = "Week", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairTopReport()
        {
            //Inputs.Add(SelectBy);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            //Inputs.Add(month);
            //Inputs.Add(week);
        }

        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 08:00:00";
            endTime.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 08:00:00";
            //if (SelectBy.Value.ToString() == "日期段")
            //{
            //    startTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 08:00:00";
            //    endTime.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 08:00:00";
            //    month.Value = "";
            //    week.Value = "";
            //}
            //else if (SelectBy.Value.ToString() == "月")
            //{
            //    startTime.Value = "";
            //    endTime.Value = "";
            //    month.Value = DateTime.Now.Month.ToString();
            //    week.Value = "";
            //}
            //else if (SelectBy.Value.ToString() == "周")
            //{
            //    startTime.Value = "";
            //    endTime.Value = "";
            //    month.Value = "";
            //    week.Value = ConverDate.GetWeekFromDate(DateTime.Now);
            //}
        }      

        public override void Run()
        {
            DateTime dateFrom = (DateTime)startTime.Value;
            DateTime dateTO = (DateTime)endTime.Value;
            DataTable dtRepairTop = new DataTable();
            DataTable loadTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow loadTitleRow = null;
            DataRow loadDataRow = null;
            DataRow linkTitleRow = null;
            DataRow linkDataRow = null;
            int col = 0;
            string skuno = "";
            string sqlRun = "";
            //if (SelectBy.Value.ToString() == "日期段" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            //{
            //    dateFrom = (DateTime)startTime.Value;
            //    dateTO = (DateTime)endTime.Value;
            //}
            //else if (SelectBy.Value.ToString() == "月" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            //{
            //    if (string.IsNullOrEmpty(month.Value.ToString()))
            //    {
            //        throw new Exception("Please input month");
            //    }
            //    dateFrom = DateTime.Parse(DateTime.Now.Year.ToString() + "-" + month.Value + "-" + "01 08:00:00");
            //    dateTO = DateTime.Parse(DateTime.Now.Year.ToString() + "-" + month.Value + "-" + "01 08:00:00").AddMonths(1);
            //}
            //else if (SelectBy.Value.ToString() == "周" && !string.IsNullOrEmpty(SelectBy.Value.ToString()))
            //{
            //    if (string.IsNullOrEmpty(week.Value.ToString()))
            //    {
            //        throw new Exception("Please input week");
            //    }
            //    dateFrom = ConverDate.GetWeekStartDate(Convert.ToInt32(week.Value.ToString()));
            //    dateTO = dateFrom.AddDays(7);
            //}
            sqlRun = $@"select d.sku_name ,c.fail_code, count(1) count
                          from r_repair_main a
                           inner join r_sn b
                              on a.sn = b.sn
                           inner join r_repair_failcode c
                              on c.sn = a.sn
                             and c.edit_time = a.edit_time
                           inner join c_sku d
                              on d.skuno = b.skuno
                           where a.closed_flag = 0
                             and a.edit_time between
                                 to_date('{dateFrom.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy-MM-dd hh24:mi:ss') and
                                 to_date('{dateTO.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy-MM-dd hh24:mi:ss')
                             --and c.fail_category = 'DEFECT'
                           group by d.sku_name, c.fail_code
                           order by d.sku_name, c.fail_code, count(1) desc";

            #region 原報表查詢語句
            //select c.failcode, d.codename , count(1) count
            //    from sfcrepairmain A
            //   inner join mfworkstatus B
            //      on a.sysserialno = b.sysserialno
            //   inner join sfcrepairfailcode C
            //      on C.SYSSERIALNO = a.sysserialno
            //     and c.createdate = a.createdate
            //   inner join sfccodelike d
            //      on d.skuno = b.skuno
            //   where a.repaired = 1
            //     and a.lasteditdt between
            //         to_date('', 'yyyy-MM-dd hh24:mi:ss') and
            //         to_date('', 'yyyy-MM-dd hh24:mi:ss')
            //     and C.failcategory = 'DEFECT'
            //   group by d.codename, c.failcode
            //   order by c.failcode, count(1) desc;
            #endregion

            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                dtRepairTop = SFCDB.RunSelect(sqlRun).Tables[0];

                for (int top = 0; top <= 10; top++)
                {
                    if (top == 0)
                    {
                        loadTable.Columns.Add("SKU_NAME");
                        linkTable.Columns.Add("SKU_NAME");
                    }
                    else
                    {
                        loadTable.Columns.Add("TOP" + top.ToString());
                        linkTable.Columns.Add("TOP" + top.ToString());
                    }
                }
                
                for (int i = 0; i < dtRepairTop.Rows.Count; i++)
                {
                    //if (skuno != dtRepairTop.Rows[i]["sku_name"].ToString())
                    //{
                    //    skuno = dtRepairTop.Rows[i]["sku_name"].ToString();
                    //    loadTitleRow = loadTable.NewRow();
                    //    loadDataRow = loadTable.NewRow();
                    //    linkTitleRow = linkTable.NewRow();                        
                    //    linkDataRow = linkTable.NewRow();
                    //    loadTitleRow["sku_name"] = skuno;
                    //    linkTitleRow["sku_name"] = "MESReport.BaseReport.RepairTopPieBySku";
                    //    col = 0;
                    //    loadTable.Rows.Add(loadTitleRow);
                    //    loadTable.Rows.Add(loadDataRow);
                    //    linkTable.Rows.Add(linkTitleRow);
                    //    linkTable.Rows.Add(linkDataRow);
                    //}
                    //col++;
                    //if (col > 10)
                    //{
                    //    continue;
                    //}
                    //loadTitleRow[col] = dtRepairTop.Rows[i]["fail_code"].ToString();
                    //loadDataRow[col] = dtRepairTop.Rows[i]["count"].ToString();
                    //linkTitleRow[col] = "MESReport.BaseReport.RepairFailCodeDetail"; 

                    if (skuno != dtRepairTop.Rows[i]["sku_name"].ToString())
                    {
                        skuno = dtRepairTop.Rows[i]["sku_name"].ToString();
                        loadTitleRow = loadTable.NewRow();
                        loadDataRow = loadTable.NewRow();
                        linkTitleRow = linkTable.NewRow();
                        linkDataRow = linkTable.NewRow();
                        loadTitleRow["sku_name"] = skuno;
                        linkTitleRow["sku_name"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.RepairTopPieBySku&RunFlag=1&SkuName=" + skuno + "&StartTime="
                                                 + dateFrom.ToString("yyyy-MM-dd HH:mm:ss") + "&EndTime=" + dateTO.ToString("yyyy-MM-dd HH:mm:ss");
                        col = 0;
                        loadTable.Rows.Add(loadTitleRow);
                        loadTable.Rows.Add(loadDataRow);
                        linkTable.Rows.Add(linkTitleRow);
                        linkTable.Rows.Add(linkDataRow);

                        DataRow [] rows = dtRepairTop.Select("sku_name='"+ skuno + "'", "count desc");
                        for (int j = 0; j < rows.Length; j++)
                        {
                            col++;
                            if (col > 10)
                            {
                                continue;
                            }
                            loadTitleRow[col] = rows[j]["fail_code"].ToString();
                            loadDataRow[col] = rows[j]["count"].ToString();
                            linkTitleRow[col] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.RepairFailCodeDetail&RunFlag=1&ErrorCode=" + rows[j]["fail_code"].ToString();
                        }
                    }
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(loadTable, linkTable);                
                reportTable.Tittle = "RepairTopTable";                
                Outputs.Add(reportTable);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
