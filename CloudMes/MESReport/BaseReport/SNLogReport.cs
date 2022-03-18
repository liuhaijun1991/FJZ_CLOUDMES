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
    public class SNLogReport:ReportBase
    {
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SNLogReport()
        {
            Inputs.Add(inputSN);
        }

        public override void Init()
        {
            //base.Init();
        }
        public override void Run()
        {
            base.Run();
            string sn = inputSN.Value.ToString().Trim();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.SN == sn).ToDataTable();
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.ColNames.RemoveAt(0);
                reportTable.Tittle = "SNLog";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart rAlart = new ReportAlart(ex.Message);
                Outputs.Add(rAlart);
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
