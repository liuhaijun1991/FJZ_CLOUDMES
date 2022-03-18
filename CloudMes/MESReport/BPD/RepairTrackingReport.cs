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
    class RepairTrackingReport : ReportBase
    {
        OleExec SFCDB = null;
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01 08:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = DateTime.Now, Enable = true, SendChangeEvent = false, ValueForUse = null };
        public RepairTrackingReport()
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
            

            //dt =SFCDB.ORM.Queryable<R_REPAIR_TRANSFER, R_REPAIR_MAIN, R_WO_BASE>((rrt, rrm, rwb) => rrt.REPAIR_MAIN_ID == rrm.ID && rrt.WORKORDERNO == rwb.WORKORDERNO)
            //    .WhereIF(SN.Value != null && !SN.Value.ToString().Equals(""), (rrt, rrm, rwb) => rrt.SN == SN.Value.ToString())
            //    .WhereIF(SKUNO.Value!=null&&!SKUNO.Value.ToString().Equals(""),(rrt,rrm,rwb)=>rrt.SKUNO==SKUNO.Value.ToString())
            //    .Where((rrt,rrm,rwb)=>rrt.IN_TIME>starttime&&rrt.IN_TIME<endtime&&rrt.OUT_TIME==null)
            // .OrderBy((rrm, rrf, rra, cec, cac, crc) => rrm.SN).OrderBy((rrm,rrf,rra,cec,cac,crc)=>rrm.FAIL_TIME)
            //.Select(@"rrm.sn,rrm.skuno,rrm.workorderno,rrm.fail_line,decode(rrm.closed_flag,'0','不良品','1','正常品') product_status,
            //    rrm.fail_station,rrm.fail_device,cec.chinese_description,rrf.fail_location,rrf.fail_category,rrm.fail_time,
            //    decode(rrf.repair_flag,'0','待維修','1','已維修') repair_status,cac.chinese_description,crc.chinese_description,
            //    rra.description,rra.tr_sn,rra.new_tr_sn,rra.repair_emp,rra.repair_time")
            //    .ToDataTable();


           try {
                string sql = $@"SELECT C.SKU_SERIES 客戶,
                               A.SKUNO PN1,
                               C.SKUNO || ' ' || C.SKU_VER PN2,
                               IN_TIME CHECKINDATE,
                               A.SN,
                               B.FAIL_STATION,
                               ROUND((SYSDATE - IN_TIME), 2) DelayDays,
                               SYSDATE Today,
                               a.in_receive_emp AS EMP_NO,
                               D.EMP_NAME
                          FROM R_REPAIR_TRANSFER A, R_REPAIR_MAIN B, R_WO_BASE C,C_USER D
                         where A.REPAIR_MAIN_ID = B.ID
                           AND A.WORKORDERNO = C.WORKORDERNO
                           AND A.IN_RECEIVE_EMP=D.EMP_NO
                           AND A.IN_TIME BETWEEN TO_DATE('" + starttime+ "', 'yyyy-mm-dd hh24:mi:ss') AND TO_DATE('"+ endtime + "', 'yyyy-mm-dd hh24:mi:ss')"+
                          "AND A.OUT_TIME IS NULL";
                if (SN.Value != null && SN.Value.ToString() != "") { sql += $@" AND A.SN= '{sn}'"; }
                if (SKUNO.Value != null && SKUNO.Value.ToString() != "") { sql += $@" AND A.SKUNO='{sku}'"; }
                sql += " order by 7 desc";
                SFCDB = DBPools["SFCDB"].Borrow();
                dt = SFCDB.RunSelect(sql).Tables[0];
                // dt = SFCDB.RunSelect(sql).Tables[0];
                ReportTable retTab = new ReportTable();
           

                retTab.LoadData(dt);
                retTab.Tittle = "待維修";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception){
                DBPools["SFCDB"].Return(SFCDB);
            }
            }
    }
  
}
