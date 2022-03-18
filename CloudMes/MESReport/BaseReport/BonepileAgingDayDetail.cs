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
    public class BonepileAgingDayDetail: ReportBase
    {
        ReportInput inputDate = new ReportInput() { Name = "Date", InputType = "String", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput() { Name = "Type", InputType = "String", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public BonepileAgingDayDetail()
        {
            Inputs.Add(inputDate);
            Inputs.Add(inputType);
        }

        public override void Init()
        {
            base.Init();
        }
        public override void Run()
        {
            string date = inputDate.Value.ToString();
            string type = inputType.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable data = SFCDB.ORM.Queryable<R_SN_LOG>()
                    .Where(r => r.LOGTYPE == "BonepileAgingDayReport" && r.DATA3 == date)
                    .WhereIF(!type.Equals("ALL"), r => r.DATA1 == type).Select(r => new { r.SN, Type = r.DATA1, AgingDays = r.DATA2, Day = r.DATA3 }).ToDataTable();
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(data, null);
                reportTable.Tittle = $@"BonepileAgingDayDetail";
                Outputs.Add(reportTable);
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
    }
}
