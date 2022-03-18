using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class SmtSNFailReport : ReportBase
    {
        ReportInput Sn = new ReportInput { Name = "SN", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SmtSNFailReport()
        {
            Inputs.Add(Sn);

        }
        public override void Run()
        {

            string sn = Sn.Value.ToString();
            string sql = string.Empty;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {

                if (sn != "ALL")
                {
                    sql = $@"select a.SN,b.FAIL_LINE,a.DATA2 WO,a.DATA1 STATION,c.fail_location,c.description,a.DATA3 SKU,a.DATA5 鎖定類型,c.FAIL_TIME   
                               from r_sn_log a,r_repair_main b,r_repair_failcode c
                                where a.LOGTYPE ='SMTWOLOCK' and a.sn = b.sn and a.data1= b.FAIL_STATION and b.id = c.repair_main_id 
                                and a.sn = c.sn  and a.sn = '{sn}'  order by c.FAIL_TIME desc";
                }
                else {
                    sql = $@"select a.SN,b.FAIL_LINE,a.DATA2 WO,a.DATA1 STATION,c.fail_location,c.description,a.DATA3 SKU,a.DATA5 鎖定類型,c.FAIL_TIME   
                                from r_sn_log a,r_repair_main b,r_repair_failcode c
                                where a.LOGTYPE ='SMTWOLOCK' and a.sn = b.sn and a.data1= b.FAIL_STATION and b.id = c.repair_main_id 
                                and a.sn = c.sn   order by c.FAIL_TIME desc";
                }                
                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                if (dt.Rows.Count == 0) {
                    throw new Exception("No Data");
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt,null);
                retTab.Tittle = "Smt Fail Report";
                Outputs.Add(retTab);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }

}
