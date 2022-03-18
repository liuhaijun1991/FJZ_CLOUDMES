using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.HWD
{
    public class MRBASSYReturnReport: ReportBase
    {
        ReportInput inputWo = new ReportInput() { Name = "ASSY_WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportTable reportTable = null;
        public override void Init()
        {
            Inputs.Add(inputWo);
            Inputs.Add(inputSN);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
        }

        public override void Run()
        {
            OleExec SFCDB = null;
            reportTable = new ReportTable();
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = GetData(SFCDB);
                DataTable linkTable = new DataTable();
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("NEXT_STATION");
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("FROM_STORAGE");
                linkTable.Columns.Add("ASSY_WO");
                linkTable.Columns.Add("REWORK_WO");
                linkTable.Columns.Add("CREATE_EMP");
                linkTable.Columns.Add("CREATE_TIME");
                DataRow linkRow = null;
                foreach (DataRow row in dt.Rows)
                {
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn column in linkTable.Columns)
                    {                       
                        linkRow[column.ColumnName] = "";                        
                        if (column.ColumnName == "SN")
                        {
                            linkRow[column.ColumnName] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["SN"].ToString();
                        }                        
                    }                   
                    linkTable.Rows.Add(linkRow);
                }

                if (PaginationServer)
                {
                    reportTable.MakePagination(dt, linkTable, PageNumber, PageSize);
                }
                else
                {
                    reportTable.LoadData(dt, linkTable);
                }
                reportTable.Tittle = "MRB ASSY Return Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void DownFile()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = GetData(SFCDB);
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "MRBASSY_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        private DataTable GetData(OleExec SFCDB)
        {
            string dateFrom = inputDateFrom.Value.ToString();
            string dateTo = inputDateTo.Value.ToString();
            string panel = inputWo.Value.ToString();
            string sn = inputWo.Value.ToString();
            string wo_sql = "", sn_sql = "", date_sql = "";
            if (!string.IsNullOrEmpty(panel))
            {
                wo_sql = $@" and from_storage='{panel}' ";
            }
            else
            {
                wo_sql = $@" and from_storage is not null ";
            }
            if (!string.IsNullOrEmpty(sn))
            {
                wo_sql = $@" and sn='{sn}' ";
            }            
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                if (Convert.ToInt64(Convert.ToDateTime(dateFrom).ToString("yyyyMMdd")) > Convert.ToInt64(Convert.ToDateTime(dateTo).ToString("yyyyMMdd")))
                {
                    throw new Exception("Date From不能大於Date To!");
                }
                date_sql = $@" and create_time between to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss') 
                            and to_date('{dateTo}','yyyy/mm/dd hh24:mi:ss')";
            }
            string sql = $@"select SN,WORKORDERNO,NEXT_STATION,SKUNO,FROM_STORAGE as ASSY_WO,REWORK_WO,CREATE_EMP,CREATE_TIME from r_mrb where 1=1 {wo_sql} {sn_sql} {date_sql} ";
            DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("No Data!");
            }
            return dt;
        }
    }
}
