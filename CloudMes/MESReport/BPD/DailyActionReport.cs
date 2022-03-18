using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using System.Data;

namespace MESReport.BPD
{
    class DailyRepairActionReport : ReportBase
    {
        OleExec SFCDB = null;
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2017/02/01 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018/02/12 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };

        public DailyRepairActionReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            Inputs.Add(SN);
            Inputs.Add(SkuNo);
        }

        public override void Init()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                InitSkuno(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InitSkuno(OleExec db)
        {
            List<string> skuno = new List<string>();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string strSql = $@"select distinct skuno from c_sku order by skuno";
            try
            {
                DataTable dt = SFCDB.ExecuteDataTable(strSql, CommandType.Text);
                skuno.Add("ALL");
                foreach (DataRow dr in dt.Rows)
                {
                    skuno.Add(dr["SKUNO"].ToString());
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                SkuNo.ValueForUse = skuno;
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }


        public override void Run() {
           
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            //DataTable table1 = SFCDB.ORM.Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE, r_repair_action, R_WO_BASE, C_ERROR_CODE, C_USER>
            //  ((a, b, c, d, e, f) => a.ID == b.REPAIR_MAIN_ID && b.ID == c.REPAIR_FAILCODE_ID && a.WORKORDERNO == d.WORKORDERNO && c.REASON_CODE == e.ERROR_CODE &&
            //  c.REPAIR_EMP == f.EMP_NO && c.REPAIR_TIME > stime && c.REPAIR_TIME < etime).WhereIF(SN != null || SN.Value.ToString() != "ALL", 
            //  ((a, b, c) => c.SN == SN.Value.ToString())).WhereIF(SkuNo!=null&&SkuNo.Value.ToString()!="ALL",(a,b,c)=>a.SKUNO==SkuNo.Value.ToString()).
            //  OrderBy((a, b, c) => a.WORKORDERNO).Select((a,b,c,d,e,f)=>a.SN).ToDataTable();


            string sql = $@"select distinct (to_char(b.repair_time, 'yyyy-mm-dd')) REpairDATE,
                    a.skuno || ' ' || a.sku_ver PN,
                    a.sku_series,
                    b.sn,
                    e.fail_station STATION,
                    c.english_description,
                    b.reason_code,
                    c.chinese_description,
                    b.action_code,
                    b.fail_location,
                    '' Component1,
                    '' dc,
                    '' lc,
                    '' Vender,
                    '' mfr,
                    b.tr_sn,
                    '' Component2 ,
                   f.emp_name
              from r_wo_base a,
                   r_repair_action   b,
                   c_error_code c,
                   r_repair_failcode d,
                   r_repair_main e,
                   c_user f
             where e.id = d.repair_main_id
               and d.id = b.repair_failcode_id
               and e.workorderno = a.workorderno
               and b.reason_code = c.error_code
               and b.repair_emp = f.emp_no
               and b.repair_time between
               to_date('" + svalue + "','yyyy-mm-dd hh24:mi:ss')" +
               "and to_date('" + evalue + "','yyyy-mm-dd hh24:mi:ss')";
            try {
                if (SN.Value.ToString() != "ALL" && SN.Value.ToString() != string.Empty)
                {

                    sql += $@" and e.SN = '{ SN.Value.ToString()}'";
                }
                if (SkuNo.Value.ToString() != "ALL" && SkuNo.Value.ToString() != string.Empty)
                {
                    sql += $@" and a.skuno = '{SkuNo.Value.ToString()}'";
                }
                sql += $@" order by b.sn,a.sku_series,e.fail_station";
                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                ReportTable retTab = new ReportTable();
               
                retTab.LoadData(dt);
                retTab.Tittle = "Daily Repair Action Report";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee){
                DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }
        }


    }

  


}
