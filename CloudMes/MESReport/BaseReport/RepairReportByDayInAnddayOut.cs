using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class RepairReportByDayInAnddayOut : ReportBase
    {
        ReportInput InDay = new ReportInput() { Name = "InDay", InputType = "DateTime2", Value = "2021-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput OutDay = new ReportInput() { Name = "OutDay", InputType = "DateTime2", Value = "2021-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public RepairReportByDayInAnddayOut()
        {
            Inputs.Add(InDay);
            Inputs.Add(OutDay); ;
        }
        public override void Init()
        {
            InDay.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            OutDay.Value = DateTime.Now.ToString("yyyy-MM-dd");
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
            DateTime In_Day = Convert.ToDateTime(InDay.Value);
            DateTime Out_Day = Convert.ToDateTime(OutDay.Value);
            //Out_Day = Out_Day.AddDays(1);
            string inDay = In_Day.ToString("yyyy-MM-dd");
            string outday = Out_Day.ToString("yyyy-MM-dd");
            DataTable dt = null;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sqlRun = $@"select * from R_REPAIR_TRANSFER where TO_CHAR(in_time, 'yyyy-MM-dd')='{inDay}' and TO_CHAR(out_time, 'yyyy-MM-dd')='{outday}'";
                ReportTable reportTable = new ReportTable();
                dt = SFCDB.RunSelect(sqlRun).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "Repair By DayIn and DayOut";
                Outputs.Add(reportTable);

            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }
    }
}
