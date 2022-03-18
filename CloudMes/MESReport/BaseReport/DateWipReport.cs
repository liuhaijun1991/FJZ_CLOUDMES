using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class DateWipReport :ReportBase
    {
        ReportInput Sku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public DateWipReport() {
            Inputs.Add(Sku);
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
            try {
                String sql = $@"select SN,SKUNO,WORKORDERNO,START_TIME,COMPLETED_TIME,CURRENT_STATION,NEXT_STATION,EDIT_EMP,EDIT_TIME from r_sn where VALID_FLAG =1
                and START_TIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{evalue}','YYYY/MM/DD HH24:MI:SS') ";
                if (Sku.Value.ToString() != "ALL")
                {
                    sql = sql+ $@" and skuno = {Sku.Value.ToString()}";
                }
                sql = sql+ $@" order by START_TIME,WORKORDERNO,SN ";

                RunSqls.Add(sql);
                DataSet res = SFCDB.RunSelect(sql);
                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "SN Date Wip";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            } catch (Exception ex) {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
           
           
        }
        }
}
