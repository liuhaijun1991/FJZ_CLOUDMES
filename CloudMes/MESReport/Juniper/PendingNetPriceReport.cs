using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class PendingNetPriceReport : MESReport.ReportBase
    {
        ReportInput inputType = new ReportInput() { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL", "NeverShipped", "Shipped" } };
        public PendingNetPriceReport()
        {
            Inputs.Add(inputType);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                ReportTable reportTable = new ReportTable();
                DataTable dt = GetData(SFCDB);
                if (PaginationServer)
                {
                    reportTable.MakePagination(dt, null, PageNumber, PageSize);
                }
                else
                {
                    reportTable.LoadData(dt, null);
                }
                reportTable.Tittle = "Double Check Report";
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
        private DataTable GetData(OleExec SFCDB )
        {
            DataTable dt = new DataTable();
            try
            {
                string type = inputType.Value.ToString();
                string sql = "SELECT GROUPID,count(wo)\n" +
                "  FROM R_PRE_WO_HEAD\n" +
                " WHERE GROUPID IN\n" +
                "       (SELECT GROUPID\n" +
                "          FROM R_PRE_WO_HEAD O\n" +
                "         WHERE 1=1\n" +
                "           {0}\n" +
                "           AND GROUPID IS NOT NULL)\n" +
                "   AND GROUPID IS NOT NULL\n" +
                "  GROUP BY GROUPID \n" +
                "  ORDER BY GROUPID";

                switch (type)
                {
                    case "NeverShipped":
                        sql = string.Format(sql, "AND WO IN (SELECT PREWO FROM O_ORDER_MAIN WHERE FINALASN = '0')");
                        break;
                    case "Shipped":
                        sql = string.Format(sql, "AND WO IN (SELECT PREWO FROM O_ORDER_MAIN WHERE FINALASN <> '0')");
                        break;
                    default:
                        sql = string.Format(sql, "");
                        break;
                }
                dt = SFCDB.ORM.Ado.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public override void DownFile()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = GetData(SFCDB);
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "GroupID_" + inputType.Value.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
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



    }
}
