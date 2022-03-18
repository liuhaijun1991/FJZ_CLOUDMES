using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.HWD
{
    public class SN_KEYPART_DETAIL : ReportBase
    {
        ReportInput inputWO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputKeypartSN = new ReportInput() { Name = "KeypartSN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputPartNO = new ReportInput() { Name = "PartNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SN_KEYPART_DETAIL()
        {
            this.Inputs.Add(inputWO);
            this.Inputs.Add(inputSN);
            this.Inputs.Add(inputKeypartSN);
            this.Inputs.Add(inputPartNO);
        }
        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            string wo = inputWO.Value.ToString().Trim().ToUpper();
            string sn = inputSN.Value.ToString().Trim().ToUpper();
            string keypartSN = inputKeypartSN.Value.ToString().Trim().ToUpper();
            string partNO = inputPartNO.Value.ToString().Trim().ToUpper();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                if (string.IsNullOrEmpty(wo) && string.IsNullOrEmpty(sn) && string.IsNullOrEmpty(keypartSN))
                {
                    throw new Exception("Please input WO or SN or KeypartSN");
                }
                string sql = $@"select distinct a.skuno,a.workorderno,a.sn,b.keypart_sn,b.station_name,b.part_no,b.seq_no,b.category_name,b.category,b.create_emp,b.create_time
                             from r_sn a,r_SN_KEYPART_DETAIL b,c_keypart c where a.sn=b.sn and a.skuno=c.skuno and c.part_no=b.part_no";
                if (!string.IsNullOrEmpty(wo))
                {
                    sql += $@" and a.sn in (select sn from r_sn where workorderno='{wo}' and valid_flag='1') ";
                }
                if (!string.IsNullOrEmpty(sn))
                {
                    sql += $@" and a.sn ='{sn}' ";
                }
                if (!string.IsNullOrEmpty(keypartSN))
                {
                    sql += $@" and b.keypart_sn ='{keypartSN}' ";
                }
                if (!string.IsNullOrEmpty(partNO))
                {
                    sql += $@" and b.part_no='{partNO}' ";
                }

                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No Data.");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "SN_KEYPART_DETAIL";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
