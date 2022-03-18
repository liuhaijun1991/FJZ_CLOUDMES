using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="RepairTopReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// RepairTopPieBySku 以餅圖的方式顯示某一系列的前10不良代碼
    /// </summary>
    public class RepairTopPieBySku: ReportBase
    {       
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput skuName = new ReportInput() { Name = "SkuName", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairTopPieBySku()
        {
            Inputs.Add(skuName);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
        }
        public override void Init()
        {
            //startTime.Value = DateTime.Now.AddDays(-365).ToString("yyyy-MM-dd") + " 08:00:00";
            //endTime.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 08:00:00";
        }

        public override void Run()
        {
            DateTime dateFrom = DateTime.Now.AddDays(-730);
            DateTime dateTO = DateTime.Now;
            DataTable dtRepairTop = new DataTable();
            string _skuName = "";
            string sqlRun = "";
            int col;
            //dateFrom = (DateTime)startTime.Value;
            //dateTO = (DateTime)endTime.Value;
            if (startTime.Value == null || string.IsNullOrEmpty(startTime.Value.ToString()))
            {
                throw new Exception("Start time can not be null!");
            }
            else
            {
                dateFrom = Convert.ToDateTime(startTime.Value) ;
            }
            if (endTime.Value == null || string.IsNullOrEmpty(endTime.Value.ToString()))
            {
                throw new Exception("End time can not be null!");
            }
            else
            {
                dateTO = (DateTime)endTime.Value;
            }
            if (skuName.Value == null || string.IsNullOrEmpty(skuName.Value.ToString()))
            {
                throw new Exception("Sku name can not be null!");
            }
            else
            {
                _skuName = skuName.Value.ToString();
            }

            sqlRun = $@"select c.fail_code, d.sku_name , count(1) count
                          from r_repair_main a
                           inner join r_sn b
                              on a.sn = b.sn
                           inner join r_repair_failcode c
                              on c.sn = a.sn
                             and c.edit_time = a.edit_time
                           inner join c_sku d
                              on d.skuno = b.skuno
                           where a.closed_flag = 0
                             and d.sku_name='{_skuName}'
                             and a.edit_time between
                                 to_date('{dateFrom.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy-MM-dd hh24:mi:ss') and
                                 to_date('{dateTO.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy-MM-dd hh24:mi:ss')
                             --and c.fail_category = 'DEFECT'
                           group by d.sku_name, c.fail_code
                           order by c.fail_code, count(1) desc";

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
                if (dtRepairTop.Rows.Count > 0)
                {                    
                    DataRow[] rows = dtRepairTop.Select("sku_name='" + _skuName + "'", "count desc");
                    List<object> objList = new List<object>();
                    pieChart pie = new pieChart();
                    //pie.Tittle = dateFrom.ToString("yyyy-MM-dd HH:mm:ss") + "至" + dateTO.ToString("yyyy-MM-dd HH:mm:ss") + "  " + _skuName + "不良TOP10餅狀圖";
                    //pie.Tittle = dateFrom.ToString("f").Replace(":", "時") + "分至" + dateTO.ToString("f").Replace(":", "時") + "分" + _skuName + "不良TOP10餅狀圖";
                    pie.Tittle = dateFrom.ToString("f") + "~" + dateTO.ToString("f") + _skuName + "Bad top 10 pie chart ";
                    //pie.ChartTitle = "主標題";
                    //pie.ChartSubTitle = "副標題";
                    pie.ChartTitle = "Main title ";
                    pie.ChartSubTitle = "Sub title";
                    ChartData chartData = new ChartData();
                    chartData.name = _skuName;
                    chartData.type = ChartType.pie.ToString();
                    col = 0;
                    for (int j = 0; j < rows.Length; j++)
                    {
                        col++;
                        if (col < 11)
                        {
                            objList.Add(new List<object> { rows[j]["fail_code"].ToString(), Convert.ToInt64(rows[j]["count"].ToString()) });
                        }                        
                    }
                    chartData.data = objList;
                    chartData.colorByPoint = true;
                    List<ChartData> _ChartDatas = new List<ChartData> { chartData };
                    pie.ChartDatas = _ChartDatas;
                    Outputs.Add(pie);
                }               
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
