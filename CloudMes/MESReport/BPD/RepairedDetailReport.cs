using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;
using MESDataObject.Module;
using System.Linq.Expressions;

namespace MESReport.BPD
{
    class RepairedDetailReport: ReportBase
    {
        OleExec SFCDB = null;
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01 08:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = DateTime.Now, Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairedDetailReport()
        {
            this.Inputs.Add(SN);
            this.Inputs.Add(SKUNO);
            this.Inputs.Add(StartTime);
            this.Inputs.Add(EndTime);
        }


        public override void Run()
        {
            DataTable dt = null;
            DateTime starttime = DateTime.Parse(StartTime.Value.ToString());
            DateTime endtime = DateTime.Parse(EndTime.Value.ToString());

            string sn = SN.Value.ToString();
            string sku = SKUNO.Value.ToString();


            try
            {
                string sql = $@"
 select d.repair_time RepairDate,
        e.sku_series 客戶,
        e.skuno || ' ' || e.sku_ver PN,
        a.sn sn,
        c.station_name station,
        f.chinese_description SYMPTOMDESC,
        d.reason_code,
        (select english_description
           from c_error_code
          where error_code = d.reason_code) ROOTCAUSE,
        d.action_code,
        d.fail_location
   from r_repair_main     a,
        r_repair_failcode b,
        r_repair_transfer c,
        r_repair_action   d,
        r_wo_base         e,
        c_error_code      f,
        c_action_code     g
  where a.id = b.repair_main_id
    and b.id = d.repair_failcode_id
    and c.repair_main_id = a.id
    and e.workorderno = a.workorderno
    and e.skuno = a.skuno
    and g.action_code = d.action_code
    and f.error_code = b.fail_code
    and d.repair_time between to_date('{starttime}','yyyy-mm-dd hh24:mi:ss')
    and  to_date('{endtime}','yyyy-mm-dd hh24:mi:ss')";
                if (SN.Value != null && SN.Value.ToString() != "") { sql += $@" AND A.SN= '{sn}'"; }
                if (SKUNO.Value != null && SKUNO.Value.ToString() != "") { sql += $@" AND A.SKUNO='{sku}'"; }
                sql += " order by 7 desc";
                SFCDB = DBPools["SFCDB"].Borrow();
                dt = SFCDB.RunSelect(sql).Tables[0];
                // dt = SFCDB.RunSelect(sql).Tables[0];
                ReportTable retTab = new ReportTable();


                retTab.LoadData(dt);
                retTab.Tittle = "維修明細";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
