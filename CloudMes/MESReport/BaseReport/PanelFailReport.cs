using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class PanelFailReport: ReportBase
    {
        ReportInput inputPanel = new ReportInput() { Name = "Panel", InputType = "TXT", Value = "PNCG5E43", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportTable reportTable = null;
        public PanelFailReport()
        {
            Inputs.Add(inputPanel);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);                  
        }

        public override void Init()
        {
            //base.Init();
            reportTable = new ReportTable();
            reportTable.ColNames = new List<string> {"FAIL_PANEL","WO","SKUNO","FAIL_STATION","SN_NUMBER","FAIL_LOCATION","FAIL_CODE","FAIL_DESC", "FAIL_TIME",
                "FAIL_EMP","PROCESS","ACTION_CODE", "ACTION_DESC","REPAIR_TIME","REPAIR_EMP"};
            Outputs.Add(new ReportColumns(reportTable.ColNames));
            PaginationServer = true;
        }

        public override void Run()
        {
            //base.Run();           
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = GetData(SFCDB);
                if (PaginationServer)
                {
                    reportTable.MakePagination(dt, null, PageNumber, PageSize);
                }
                else
                {
                    reportTable.LoadData(dt, null);
                }
                reportTable.Tittle = "Panel Fail Report";
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
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = GetData(SFCDB);
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "PanelFail_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
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
            string panel = inputPanel.Value.ToString();
            string panel_sql = "", date_sql = "";
            if (!string.IsNullOrEmpty(panel))
            {
                panel_sql = $@" and m.sn='{panel}'"; 
            }
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                if (Convert.ToInt64(Convert.ToDateTime(dateFrom).ToString("yyyyMMdd")) > Convert.ToInt64(Convert.ToDateTime(dateTo).ToString("yyyyMMdd")))
                {
                    throw new Exception("Date From不能大於Date To!");
                }
                date_sql = $@" and m.fail_time between to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss') 
                            and to_date('{dateTo}','yyyy/mm/dd hh24:mi:ss')";
            }
            string sql = $@"select m.sn as FAIL_PANEL,m.workorderno as WO,m.skuno,m.fail_station,f.description as sn_number,f.fail_location,
                            f.fail_code,e.chinese_description as FAIL_DESC,f.edit_time as FAIL_TIME, f.edit_emp as FAIL_EMP,f.fail_process,
                            a.action_code,ca.chinese_description as ACTION_DESC, a.edit_time as REPAIR_TIME,a.edit_emp as REPAIR_EMP
                             from r_repair_main m 
                            left join r_repair_failcode f on m.id=f.repair_main_id 
                            left join c_error_code e on e.error_code=f.fail_code 
                            left join r_repair_action a on f.id=a.repair_failcode_id
                            left join c_action_code ca on ca.action_code=a.action_code
                            where m.sn not like 'DW%'  {panel_sql} {date_sql} ";
            DataTable dt= SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count == 0)
            {
                throw new Exception("No Data!");
            }
            return dt;
        }        
    }
}
