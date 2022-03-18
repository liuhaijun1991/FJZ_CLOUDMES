using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class SilverWipReport : MESReport.ReportBase
    {
        ReportInput SKU = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };

        public SilverWipReport()
        {
            Inputs.Add(SKU);
        }

        public override void Run()
        {
            var strSql = $@"select 
                        sku.skuno,     
                        (select count(1) from R_JUNIPER_SILVER_WIP where skuno= sku.skuno and sysdate - start_time <= 10 and state_flag = '1' ) ""10Day"",
                        (select count(1) from R_JUNIPER_SILVER_WIP where skuno = sku.skuno and sysdate - start_time > 10 and sysdate - start_time <= 30 and state_flag = '1') ""10-30Day"",
                        (select count(1) from R_JUNIPER_SILVER_WIP where skuno = sku.skuno and sysdate -start_time > 30 and sysdate -start_time <= 60 and state_flag = '1' ) ""30-60Day"",
                        (select count(1) from R_JUNIPER_SILVER_WIP where skuno = sku.skuno and sysdate -start_time > 60 and sysdate -start_time <= 90 and state_flag = '1' ) ""60-90Day"",
                        (select count(1) from R_JUNIPER_SILVER_WIP where skuno = sku.skuno and sysdate -start_time > 90  and state_flag = '1' ) ""90+Day"",
                        (select count(SN) from R_JUNIPER_SILVER_WIP c where SKUNO = sku.SKUNO) TOTAL_IN_WIP, sku.VALUE MAX_IN_WIP,
                        sku.EXTEND MAX_WIP_DAYS
                    from C_SKU_DETAIL sku
                    WHERE CATEGORY = 'JUNIPER' and CATEGORY_NAME = 'SilverWip' and sku.skuno != 'CONFIG:JuniperSilverWip' ";

            if (!String.IsNullOrEmpty(SKU.Value.ToString()))
                strSql += $@" and sku.SKUNO = '{SKU.Value.ToString().Trim().ToUpper()}' ";

            strSql += " order by sku.SKUNO ";

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
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
                    //string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.SilverWipReportBySku&RunFlag=1&SKU=" + row["skuno"].ToString();
                    string linkSKU = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.SilverWipDetailReport&RunFlag=1&SKU=" + row["skuno"].ToString();

                    var linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "SKUNO")
                        {
                            linkRow[dc.ColumnName] = linkSKU;
                        }
                        else if (dc.ColumnName.ToString().ToUpper() == "TOTAL_IN_WIP")
                        {
                            linkRow[dc.ColumnName] = linkSKU;
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
