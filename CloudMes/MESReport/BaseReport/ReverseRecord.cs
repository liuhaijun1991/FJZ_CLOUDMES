using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class ReverseRecord : ReportBase
    {
        ReportInput inputSearch = new ReportInput() { Name = "SearchBy", InputType = "Select", Value = "SN", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "SN", "WO" } };
        ReportInput inputValue = new ReportInput() { Name = "Value", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEmployee = new ReportInput() { Name = "Employee", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "2020-01-01", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = "2020-02-01", Enable = false, SendChangeEvent = false, ValueForUse = null };

        public ReverseRecord()
        {
            Inputs.Add(inputSearch);
            Inputs.Add(inputValue);
            Inputs.Add(inputEmployee);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
        }
        public override void Init()
        {
            base.Init();
            inputDateFrom.Value = DateTime.Now.AddDays(-1);
            inputDateTo.Value = DateTime.Now;
        }
        public override void Run()
        {
            try
            {
                base.Run();
                string type = inputSearch.Value.ToString();
                string value = inputValue.Value.ToString().ToUpper().Trim();
                string employee = inputEmployee.Value.ToString().Trim();
                string dateFrom = Convert.ToDateTime(inputDateFrom.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");
                string dateTo = Convert.ToDateTime(inputDateTo.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");

                string runSql = $@"SELECT EDIT_EMP AS Employee, DATA1 AS ReverseType, DATA2 AS Value, LOG_SQL AS Reason, EDIT_TIME AS ReverseTime FROM SFCRUNTIME.R_MES_LOG
                                    WHERE FUNCTION_NAME = 'ReverseJuniperByType'
                                    AND DATA1 = '{type}'";
                if (!string.IsNullOrEmpty(value))
                {
                    runSql += $@" AND DATA2 = '{value}'";
                }
                if (!string.IsNullOrEmpty(employee))
                {
                    runSql += $@" AND EDIT_EMP = '{employee}'";
                }
                runSql += $@" AND EDIT_TIME BETWEEN TO_DATE('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ORDER BY EDIT_TIME DESC";

                OleExec sfcdb = DBPools["SFCDB"].Borrow();
                try
                {
                    RunSqls.Add(runSql);
                    DataTable dt = sfcdb.RunSelect(runSql).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("No Data!");
                    }

                    if (sfcdb != null)
                    {
                        DBPools["SFCDB"].Return(sfcdb);
                    }

                    ReportTable reportTableReverse = new ReportTable();
                    reportTableReverse.LoadData(dt, null);
                    reportTableReverse.Tittle = "Reverse Log";
                    Outputs.Add(reportTableReverse);
                }
                catch (Exception exception)
                {
                    if (sfcdb != null)
                    {
                        DBPools["SFCDB"].Return(sfcdb);
                    }
                    Outputs.Add(new ReportAlart(exception.Message));
                }
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}
