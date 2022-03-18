using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class SilverWipDetailReport : MESReport.ReportBase
    {
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput SKU = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput inputStatus = new ReportInput { Name = "Status", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "CHECKIN", "CHECKOUT" } };

        private string SN_F = null;
        private string SKU_F = null;
        private string State_flag = null;
        public SilverWipDetailReport()
        {
            Inputs.Add(SN);
            Inputs.Add(SKU);
            Inputs.Add(inputStatus);
        }

        public override void Run()
        {
            try
            {
                SN_F = SN.Value == null ? SN.Value.ToString() : SN.Value.ToString().Trim().ToUpper();
                SKU_F = SKU.Value == null ? SKU.Value.ToString() : SKU.Value.ToString().Trim().ToUpper();
                State_flag = inputStatus.Value == null ? inputStatus.Value.ToString() : inputStatus.Value.ToString().Trim().ToUpper();

                var strSql = $@"
                    SELECT a.SN, a.SKUNO, a.TEST_HOURS, b.BASETEMPLATE MAX_TEST_HOUR,
                    round(sysdate - a.start_time,2) WIP_DAYS, b.EXTEND MAX_WIP_DAYS, 
                    (CASE 
                        WHEN a.State_flag = 1 then 'CHECKIN' 
                        WHEN a.State_flag = 0 then 'CHECKOUT'
                        ELSE 'NA'
                      END
                     ) STATUS,
                    a.IN_WIP_USER, a.START_TIME, a.OUT_WIP_USER, a.END_TIME, a.EDIT_EMP, a.EDIT_TIME
                    FROM R_JUNIPER_SILVER_WIP a LEFT JOIN (select * from C_SKU_DETAIL where CATEGORY = 'JUNIPER' and CATEGORY_NAME = 'SilverWip') b ON a.SKUNO = b.SKUNO
                    WHERE 1=1 
                ";

                if (!String.IsNullOrEmpty(SN_F) & String.IsNullOrEmpty(SKU_F))
                    strSql += $@" and a.SN = '{SN_F}' ";
                else if (String.IsNullOrEmpty(SN_F) & !String.IsNullOrEmpty(SKU_F))
                    strSql += $@" and a.SKUNO = '{SKU_F}' ";
                else if (!String.IsNullOrEmpty(SN_F) & !String.IsNullOrEmpty(SKU_F))
                    strSql += $@" and a.SN = '{SN_F}' and a.SKUNO = '{SKU_F}' ";

                if (State_flag == "CHECKIN")
                    strSql += $@" and a.State_flag = 1 ";
                else if (State_flag == "CHECKOUT")
                    strSql += $@" and a.State_flag = 0 ";

                strSql += " ORDER BY a.EDIT_TIME DESC";

                var sfcdb = DBPools["SFCDB"].Borrow();

                try
                {
                    var res = sfcdb.RunSelect(strSql);
                    ReportTable retTab = new ReportTable();
                    retTab.Tittle = "Silver WIP Detail Report";

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
                            else if(dc.ColumnName.ToString().ToUpper() == "SKUNO")
                            {
                                linkRow[dc.ColumnName] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.SilverWipReport&RunFlag=1&SKU=" + row["skuno"].ToString(); 
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
                catch (Exception ex)
                {
                    ReportAlart alart = new ReportAlart(ex.Message);
                    Outputs.Add(alart);
                }
                finally
                {
                    DBPools["SFCDB"].Return(sfcdb);
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
