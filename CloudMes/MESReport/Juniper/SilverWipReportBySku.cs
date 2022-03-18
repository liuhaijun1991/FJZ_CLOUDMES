using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class SilverWipReportBySku : MESReport.ReportBase
    {
        ReportInput SKU = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        public SilverWipReportBySku()
        {
            //Outputs.Add(SKU);
            Inputs.Add(SKU);
        }

        public override void Run()
        {
            var strSql = $@"select w.sn , w.start_time,w.test_hours , round (sysdate - w.start_time,2) 
                            from R_JUNIPER_SILVER_WIP w 
                            where skuno='{SKU.Value}' and state_flag = '1' order by w.start_time";
            var sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                var res = sfcdb.RunSelect(strSql);
                ReportTable retTab = new ReportTable();
                DataTable linkTable = new DataTable();
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL1 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["sn"].ToString();
                    var linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "SN")
                        {
                            linkRow[dc.ColumnName] = linkURL1;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }

                retTab.LoadData(res.Tables[0], linkTable);
                retTab.Tittle = "SilverWip Report";
                Outputs.Add(retTab);
            }
            catch (Exception ee)
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }

        }

    }
}
