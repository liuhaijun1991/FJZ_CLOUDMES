using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
namespace MESReport.BaseReport
{
   public class AgileSFCMappingReport: ReportBase
    {
        ReportInput CUST_KP_NO = new ReportInput() { Name = "CUST_KP_NO", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public AgileSFCMappingReport() {
            Inputs.Add(CUST_KP_NO);
        }
        public override void Run()
        {
            string cust_kp_no = CUST_KP_NO.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string runSql = $@"select * from r_kp_agile_sfc_mapping where 1=1 ";
            try
            {
                if (cust_kp_no != "ALL" && cust_kp_no != "")
                {

                    runSql = runSql + $@" and CUST_KP_NO = '{cust_kp_no}' ";
                }
                runSql = runSql + $@" order by edit_time desc";
                DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }

                if (dt.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt,null);
                Outputs.Add(retTab);
            } catch (Exception ex) {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;

            }
        }
    }
}
