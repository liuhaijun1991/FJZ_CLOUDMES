using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class WashPcbReport : ReportBase
    {
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput woObj = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput snObj = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput lineObj = new ReportInput { Name = "LINE", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public WashPcbReport()
        {
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            Inputs.Add(woObj);
            Inputs.Add(snObj);
            Inputs.Add(lineObj);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            DateTime stime = Convert.ToDateTime(startTime.Value);
            DateTime etime = Convert.ToDateTime(endTime.Value);
            string dateFrom = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string dateTO = etime.ToString("yyyy/MM/dd HH:mm:ss");
            string sn = snObj.Value.ToString();
            string wo = woObj.Value.ToString();
            string line = lineObj.Value.ToString();
            string runSql = $@"select  distinct Pan.Panel Panel_Sn,SN.WORKORDERNO,SN.SKUNO,IO.DATA1 Reason,IO.DATA2 Location,IO.DATA3 Line,IO.EDIT_EMP Check_In_Emp,IO.EDIT_TIME Check_In_Time,
                             LAST_EDITBY Check_OUT_Emp ,LAST_EDIT_TIME Check_OUT_TIME from r_io_head IO,r_panel_sn Pan,r_sn Sn where IO.EDIT_TIME BETWEEN TO_DATE ('{dateFrom}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{dateTO}','YYYY/MM/DD HH24:MI:SS') and io.sn=Pan.Panel and pan.sn=Sn.sn and sn.valid_flag='1'";

            if (sn != "")
            {
                runSql = runSql + $@" and (IO.sn='{sn}') order by io.edit_time desc";
            }
            if (wo != "")
            {
                runSql = runSql + $@" and sn.workorderno='{wo}'order by io.edit_emp ,io.edit_time desc";
            }
            if (line != "")
            {
                runSql = runSql + $@" and IO.DATA3='{line}'order by io.edit_emp ,io.edit_time desc";
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
                reportTable.Tittle = "WashPcb Detail";
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
