using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using System.Data;

namespace MESReport.BPD
{
    public class SKUYeildReport:ReportBase
    {
        OleExec SFCDB = null;
        ReportInput Start_time = new ReportInput() { Name = "Start_time", InputType = "TXT", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput End_time = new ReportInput() { Name = "End_time", InputType = "TXT", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput LINE = new ReportInput { Name = "LINE", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL" } };
        ReportInput STATION = new ReportInput { Name = "STATION", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL" } };
        ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL" } };

        public SKUYeildReport()
        {
            Inputs.Add(Start_time);
            Inputs.Add(End_time);
            Inputs.Add(LINE);
            Inputs.Add(STATION);
            Inputs.Add(SKUNO);
        }

        public override void Init()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            Start_time.Value = DateTime.Now.ToString("yyyy/MM/dd");
            End_time.Value = DateTime.Now.ToString("yyyy/MM/dd");
            ((List<string>)LINE.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_LINE>().OrderBy(t=>t.LINE_NAME).Select(t => t.LINE_NAME).ToList());

            ((List<string>)STATION.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_STATION>().OrderBy(t=>t.STATION_NAME).Select(t => t.STATION_NAME).ToList());

            ((List<string>)SKUNO.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_SKU>().OrderBy(t=>t.SKUNO).Select(t => t.SKUNO).ToList());
        }
        
        public override void Run()
        {
            DataTable dt = null;

            DateTime startDate = DateTime.Parse(DateTime.Parse(Start_time.Value.ToString()).ToString("yyyy/MM/dd"));
            //DateTime startDate = DateTime.Parse(Start_time.Value.ToString());
            DateTime endDate = DateTime.Parse(DateTime.Parse(End_time.Value.ToString()).ToString("yyyy/MM/dd"));

            //DateTime endDate = DateTime.Parse(End_time.Value.ToString());
            string station = STATION.Value.ToString();
            string line = LINE.Value.ToString();
            string sku = SKUNO.Value.ToString();
            dt = SFCDB.ORM.Queryable<R_YIELD_RATE_DETAIL>() 
                .WhereIF(line != "ALL", t => t.PRODUCTION_LINE == line)
                .WhereIF(station != "ALL", t => t.STATION_NAME == station)
                .WhereIF(sku != "ALL", t => t.SKUNO == sku)
                .Where(t => t.WORK_DATE>= startDate && t.WORK_DATE<= endDate)
                .Select(@"WORK_DATE,
                        WORK_TIME,
                        PRODUCTION_LINE,
                        STATION_NAME,
                        WORKORDERNO,
                        SKUNO,
                        EDIT_EMP,
                        SUM(TOTAL_FRESH_BUILD_QTY) NUMS,
                        SUM(TOTAL_FRESH_PASS_QTY) PASS,
                        SUM(TOTAL_FRESH_FAIL_QTY) FAIL,
                        CASE  SUM(TOTAL_FRESH_BUILD_QTY) WHEN 0 THEN '0' ELSE ROUND(SUM(TOTAL_FRESH_PASS_QTY)/SUM(TOTAL_FRESH_BUILD_QTY),4)*100||'%' END YIELD")
                .GroupBy(@"WORK_DATE,WORK_TIME,PRODUCTION_LINE,STATION_NAME,WORKORDERNO,SKUNO,EDIT_EMP")
                .OrderBy(@"WORK_DATE,WORK_TIME")
                .ToDataTable();
            
            ReportTable retTab = new ReportTable();
            DataTable yieldTable = new DataTable();
            DataRow linkRow;
            foreach (DataColumn column in dt.Columns)
            {
                yieldTable.Columns.Add(column.ColumnName);
            }
            foreach (DataRow row in dt.Rows)
            {
                //string SKU = row["SKUNO"].ToString();
                //string STATION = row["STATION_NAME"].ToString();
                string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.DefectiveReport&RunFlag=1&=ALL&SKUNO=" + row["SKUNO"].ToString() + "&STATION=" + row["STATION_NAME"].ToString() + "";
                linkRow = yieldTable.NewRow();
                foreach (DataColumn dc in yieldTable.Columns)
                {
                    if (dc.ColumnName.ToString().ToUpper()=="FAIL")
                    {
                        linkRow[dc.ColumnName] = linkURL;
                    }
                    else
                    {
                        linkRow[dc.ColumnName] = "";
                    }
                }
                yieldTable.Rows.Add(linkRow);
            }
            retTab.LoadData(dt,yieldTable);
            retTab.Tittle="Yeild Report";
            Outputs.Add(retTab);

            
        }
    }
}
