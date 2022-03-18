using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNKPReport : ReportBase
    {
        ReportInput woObj = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput snObj = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SNKPReport()
        {
            Inputs.Add(woObj);
            Inputs.Add(snObj);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            //base.Run();
            string sn = snObj.Value.ToString();
            string wo = woObj.Value.ToString();
            string runSql = $@" select sn.skuno,sn.workorderno,sn.sn,kp.value,kp.partno,kp.kp_name,kp.mpn,kp.scantype,kp.itemseq,
                                kp.scanseq,kp.detailseq,kp.station,kp.valid_flag,kp.EXKEY1,kp.EXVALUE1,kp.EXKEY2,kp.EXVALUE2,kp.LOCATION,kp.edit_time,kp.edit_emp from  r_sn_kp kp, r_sn sn
                                 where kp.sn = sn.sn and sn.valid_flag = '1' and kp.valid_flag='1' ";
            if (sn == "" && wo == "")
            {
                ReportAlart alart = new ReportAlart("Please input a sn or wo");
                Outputs.Add(alart);
                return;
            }
            if (sn != "")
            {
                runSql = runSql + $@" and (sn.sn='{sn}' or sn.boxsn='{sn}') ";
            }
            if (wo != "")
            {
                runSql = runSql + $@" and sn.workorderno='{wo}' ";
            }
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            DataTable dt = new DataTable();
            try
            {
                dt = sfcdb.RunSelect(runSql).Tables[0];
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                if (dt.Rows.Count == 0)
                {
                   throw new Exception("NO data");                   
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "SN keypart detail";               
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}
