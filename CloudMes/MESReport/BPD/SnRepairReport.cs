using MESDataObject.Module;
using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BPD
{
    public class SnRepairReport:ReportBase
    {
        ReportInput SN = new ReportInput() { Name = "SN", InputType = "string", Enable = true, SendChangeEvent = false, EnterSubmit = true, Value = "", ValueForUse = null };
        OleExec SFCDB = null;
        public SnRepairReport()
        {
            this.Inputs.Add(SN);
        }

        public override void Run()
        {
            string Sn = SN.Value.ToString();
            SFCDB = DBPools["SFCDB"].Borrow();

            DataTable RepairData = SFCDB.ORM.Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE, R_REPAIR_ACTION, C_ERROR_CODE, C_ACTION_CODE, C_REASON_CODE>(
                (rrm, rrf, rra, cec, cac, crc) => new object[] { JoinType.Inner,rrm.ID == rrf.MAIN_ID,JoinType.Left,rrf.ID == rra.R_FAILCODE_ID,JoinType.Inner,rrf.FAIL_CODE == cec.ERROR_CODE,
                JoinType.Left,rra.ACTION_CODE == cac.ACTION_CODE,JoinType.Left,rra.REASON_CODE == crc.REASON_CODE})
                .Where((rrm, rrf, rra, cec, cac, crc) => rrm.SN == Sn)
                .OrderBy((rrm, rrf, rra, cec, cac, crc) => rrm.SN).OrderBy((rrm,rrf,rra,cec,cac,crc)=>rrm.FAIL_TIME)
                .Select(@"rrm.sn,rrm.skuno,rrm.workorderno,rrm.fail_line,decode(rrm.closed_flag,'0','不良品','1','正常品') product_status,
                    rrm.fail_station,rrm.fail_device,cec.chinese_description,rrf.fail_location,rrf.fail_category,rrm.fail_time,
                    decode(rrf.repair_flag,'0','待維修','1','已維修') repair_status,cac.chinese_description,crc.chinese_description,
                    rra.description,rra.tr_sn,rra.new_tr_sn,rra.repair_emp,rra.repair_time")
                .ToDataTable();

            ReportTable table = new ReportTable();
            table.LoadData(RepairData);
            Outputs.Add(table);
        }
    }
}
