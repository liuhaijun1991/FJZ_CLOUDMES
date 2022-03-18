using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNByLineOutReport : ReportBase
    {
        ReportInput SKUNO = new ReportInput { Name = "skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput STATION_NAME = new ReportInput { Name = "station_name", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput LINE = new ReportInput { Name = "line", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput StartTime = new ReportInput { Name = "StartTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput { Name = "EndTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput PASS = new ReportInput() { Name = "PASS", InputType = "TXT", Value = "" };
        public SNByLineOutReport()
        {
            Inputs.Add(SKUNO);
            Inputs.Add(STATION_NAME);
            Inputs.Add(LINE);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            Inputs.Add(PASS);
        }
        public override void Run()
        {
            DataSet ds = null;
            string sku_no = SKUNO.Value.ToString();
            string station = STATION_NAME.Value.ToString();
            string line = LINE.Value.ToString();
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            string pass = PASS.Value.ToString();
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            string sqlline = string.Empty;
            ReportTable retTab = new ReportTable();
            DataTable linkTable = new DataTable();
            try
            {
                sqlline = $@"select sn,station_name,line,EDIT_TIME from r_sn_station_detail where VALID_FLAG =1 and  REPAIR_FAILED_FLAG ='{pass}' 
                    and skuno = '{sku_no}' and station_name ='{station}' and line = '{line}' AND EDIT_TIME BETWEEN TO_DATE('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                AND TO_DATE('{evalue}','YYYY/MM/DD HH24:MI:SS') ";
                ds = sfcdb.RunSelect(sqlline);
                if (ds.Tables.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("no data");
                    Outputs.Add(alart);
                    return;
                }
                if (sfcdb !=null) {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                retTab.LoadData(ds.Tables[0],null);
                retTab.Tittle = "Line Output";
                Outputs.Add(retTab);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(sfcdb);
                throw exception;
            }
        }
    }
}
