using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class NoLinkReport : ReportBase
    {
        ReportInput inputWo = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSku = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public NoLinkReport()
        {
            this.Inputs.Add(inputWo);
            this.Inputs.Add(inputSku);
        }

        public override void Run()
        {
            base.Run();
            string wo = inputWo.Value == null ? "" : inputWo.Value.ToString().Trim().ToUpper();
            string skuno = inputSku.Value == null ? "" : inputSku.Value.ToString().Trim().ToUpper();
            string sqlWO = "";
            string sqlSkuno = "";
            string sqlRun = "";
            DataTable reportTable;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                if (string.IsNullOrEmpty(wo) && string.IsNullOrEmpty(skuno))
                {
                    ReportAlart alart = new ReportAlart("Please input wo or sku");
                    Outputs.Add(alart);
                    return;
                }
                if (!string.IsNullOrEmpty(wo))
                {
                    sqlWO = $@" and rsn.workorderno='{wo}' ";
                }
                if (!string.IsNullOrEmpty(skuno))
                {
                    sqlSkuno = $@" and sku.skuno='{skuno}' ";
                }
                try
                {
                    sqlRun = $@"select rsn.skuno,rsn.workorderno,rsn.sn,rsn.current_station,rsn.next_station,rsn.completed_flag,rsn.completed_time
                             from r_sn rsn where rsn.next_station='JOBFINISH' and rsn.valid_flag='1' and rsn.skuno in (
                            select sku.skuno from c_sku sku,c_kp_list_item item where sku.skuno=item.kp_partno {sqlSkuno})
                            and not exists (select * from r_sn_kp rsnkp where rsn.sn=rsnkp.value)
                            and not exists (select * from r_ship_detail ship where ship.sn=rsn.sn) {sqlWO}
                            union
                            select rsn.skuno,rsn.workorderno,rsn.sn,rsn.current_station,rsn.next_station,rsn.completed_flag,rsn.completed_time
                             from r_sn rsn where rsn.next_station='JOBFINISH' and rsn.valid_flag='1' and rsn.skuno in (
                            select sku.skuno from c_sku sku,r_sn_keypart_detail item where sku.skuno=item.part_no  {sqlSkuno})
                            and not exists (select * from r_sn_keypart_detail rsnkp where rsn.sn=rsnkp.keypart_sn)
                            and not exists (select * from r_ship_detail ship where ship.sn=rsn.sn) {sqlWO}";
                    reportTable = SFCDB.RunSelect(sqlRun).Tables[0];
                }
                catch (Exception)
                {
                    sqlRun = $@"select rsn.skuno,rsn.workorderno,rsn.sn,rsn.current_station,rsn.next_station,rsn.completed_flag,rsn.completed_time
                                 from r_sn rsn where rsn.next_station='JOBFINISH' and rsn.valid_flag='1' and rsn.shipped_flag<>'1' and rsn.skuno in (
                                select sku.skuno from c_sku sku,c_kp_list_item item where sku.skuno=item.kp_partno  {sqlSkuno} )
                                and not exists (select * from r_sn_kp rsnkp where rsn.sn=rsnkp.value) {sqlWO}
                                union
                                select rsn.skuno,rsn.workorderno,rsn.sn,rsn.current_station,rsn.next_station,rsn.completed_flag,rsn.completed_time
                                 from r_sn rsn where rsn.next_station='JOBFINISH' and rsn.valid_flag='1' and rsn.shipped_flag<>'1' and rsn.skuno in (
                                select sku.skuno from c_sku sku,r_sn_keypart_detail item where sku.skuno=item.part_no  {sqlSkuno} )
                                and not exists (select * from r_sn_keypart_detail rsnkp where rsn.sn=rsnkp.keypart_sn) {sqlWO}";
                    reportTable = SFCDB.RunSelect(sqlRun).Tables[0];
                }
                DBPools["SFCDB"].Return(SFCDB);
                if (reportTable.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Date!");
                    Outputs.Add(alart);
                    return;
                }
                DataTable linkTable = reportTable.Clone();
                DataRow linkRow;
                foreach (DataRow row in reportTable.Rows)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["SN"].ToString();
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable report = new ReportTable();
                report.LoadData(reportTable, linkTable);
                report.Tittle = "No Link Report";
                Outputs.Add(report);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message.ToString());
                Outputs.Add(alart);
            }
            finally {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
