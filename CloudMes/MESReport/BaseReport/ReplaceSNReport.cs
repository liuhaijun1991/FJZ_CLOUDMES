using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class ReplaceSNReport : ReportBase
    {
        ReportInput Sn = new ReportInput() { Name = "SN", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public ReplaceSNReport()
        {
            Inputs.Add(Sn);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }
        public override void Init()
        {
            StartTime.Value = DateTime.Now.AddDays(-10);
            EndTime.Value = DateTime.Now;
        }
        public override void Run()
        {
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sql = $@"select OLD_SN,NEW_SN,EDIT_EMP,EDIT_TIME From r_replace_sn where EDIT_TIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{evalue}','YYYY/MM/DD HH24:MI:SS') ";
                if (Sn.Value.ToString() != "ALL"){
                    sql = sql + $@" and NEW_SN  = '{Sn.Value.ToString()}' or OLD_SN = '{Sn.Value.ToString()}' ";
                }
                sql = sql + $@" order by EDIT_TIME DESC" ;
                RunSqls.Add(sql);
                DataSet res = SFCDB.RunSelect(sql);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "Replace SN Report";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }

    }
}
