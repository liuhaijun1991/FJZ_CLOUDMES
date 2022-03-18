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
    public class WHSReport : ReportBase
    {
        ReportInput inputSn = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStatus = new ReportInput { Name = "Status", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "IN", "OUT" } };

        public WHSReport()
        {
            Inputs.Add(inputSn);
            Inputs.Add(inputSku);
            Inputs.Add(inputStatus);
        }

        public override void Run()
        {
            try
            {
                base.Run();
                string sn = inputSn.Value.ToString().ToUpper().Trim();
                string sku = inputSku.Value.ToString().ToUpper().Trim();
                string status = inputStatus.Value.ToString();

                string runSql = $@"SELECT * FROM SFCRUNTIME.R_WHS_SN
                                    WHERE 1 = 1";

                if (!string.IsNullOrEmpty(sn))
                {
                    runSql += $@" AND SN = '{sn}'";
                }

                if (!string.IsNullOrEmpty(sku))
                {
                    runSql += $@" AND SKUNO = '{sku}'";
                }

                switch (status)
                {
                    case "IN":
                        runSql += $@" AND STATUS = 1";
                        break;
                    case "OUT":
                        runSql += $@" AND STATUS = 0";
                        break;
                    case "ALL":
                    default:
                        break;
                }

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
                    reportTableReverse.Tittle = "Warehouse Report";
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
