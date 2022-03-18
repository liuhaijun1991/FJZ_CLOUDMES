using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    public class PanelSNReport : ReportBase
    {
        ReportInput PanelSN = new ReportInput { Name = "SN", InputType = "TXT", Value = "PNCCM7PV", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        public PanelSNReport()
        {
            Inputs.Add(PanelSN);
            string sql = "select SN from r_panel_sn where panel in ('{0}')";
            Sqls.Add("SQL", sql); 
        }

        public override void Run()
        {
            string panel = PanelSN.Value.ToString();
            string sql = string.Format(Sqls["SQL"], panel);

            RunSqls.Add(sql);

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet res = SFCDB.RunSelect(sql);
                DataTable linkTable = new DataTable();
                ReportTable retTab = new ReportTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["SN"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "SN")
                        {
                            linkRow[dc.ColumnName] = linkURL;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }
                retTab.LoadData(res.Tables[0], linkTable);
                Outputs.Add(retTab);
            }
            catch
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
