using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
   public class SamplePlateFailReport:ReportBase
    {
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputLine= new ReportInput() { Name = "Line", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse =null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL",  "PRINT1", "PRINT2","AOI1", "AOI2", "VI1", "VI2" } };
        public SamplePlateFailReport() {
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            Inputs.Add(inputLine);
            Inputs.Add(inputStation);
        }
        public override void Init()
        {
            //base.Init();
            startTime.Value = DateTime.Now.AddDays(-1);
            endTime.Value = DateTime.Now;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            InitLine(SFCDB);
        }
        public override void Run()
        {
            // base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try {
                DateTime stime = Convert.ToDateTime(startTime.Value);
                DateTime etime = Convert.ToDateTime(endTime.Value);
                string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
                string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
                string runSql = $@"select rrm.SN,
                       rps.panel,
                       rrm.fail_line,
                       rrm.FAIL_TIME,
                       rrm.fail_station,
                       rrf.fail_category,
                       rrf.fail_code,
                       rrf.fail_location,
                       rrf.description
                  from r_repair_main rrm, r_repair_failcode rrf, r_panel_sn rps
                 where 1 =1
                    and rrm.sn = rps.sn(+)
                    and rrf.repair_main_id = rrm.id
                and rrm.CREATE_TIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS')  AND TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS')";
                if (inputStation.Value.ToString() != "ALL") {
                    runSql = runSql + $@" and rrm.fail_station = '{inputStation.Value.ToString()}'";
                }
                if (inputLine.Value.ToString()!="ALL") {
                    runSql = runSql + $@" and rrm.fail_line = '{inputLine.Value.ToString()}'";
                }
                runSql = runSql + $@" order by rrm.CREATE_TIME";
                DataSet res = SFCDB.RunSelect(runSql);
                if (SFCDB != null) {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (res.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "Sample Fail Report";
                Outputs.Add(retTab);
            }
            catch (Exception ex) {

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }
        public void InitLine(OleExec db)
        {
            List<string> line = new List<string>();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string strSql = $@"select distinct line_name from c_line order by line_name";
            try
            {
                DataTable dt = SFCDB.ExecuteDataTable(strSql, CommandType.Text);
                line.Add("ALL");
                foreach (DataRow dr in dt.Rows)
                {
                    line.Add(dr["line_name"].ToString());

                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                inputLine.ValueForUse = line;
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
    }
}
