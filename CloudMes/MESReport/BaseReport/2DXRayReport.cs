using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class _2DXRayReport:ReportBase
    {
        ReportInput Skuno = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Sn = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Line = new ReportInput { Name = "LINE", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Align = new ReportInput { Name = "ALIGN", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "<1/4", ">=1/4" } };
        ReportInput Other = new ReportInput { Name = "OTHER", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "YES", "NO" } };
        ReportInput Short = new ReportInput { Name = "SHORT", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "YES", "NO" } };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        string sqlRun = "";
        public _2DXRayReport() {

            Inputs.Add(Skuno);
            Inputs.Add(Sn);
            Inputs.Add(Line);
            Inputs.Add(Align);
            Inputs.Add(Other);
            Inputs.Add(Short);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            sqlRun = "select distinct linename from c_2dxrayline order by linename asc ";
            Sqls.Add("GetLine", sqlRun);
        }
        public override void Init()
        {
            StartTime.Value = DateTime.Now.AddDays(-1);
            EndTime.Value = DateTime.Now;
            Line.ValueForUse = GetLine();
            //base.Init();
        }

        private List<string> GetLine()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtStation = SFCDB.RunSelect(Sqls["GetLine"]).Tables[0];
            List<string> lineList = new List<string>();
            lineList.Add("ALL");
            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            if (dtStation.Rows.Count > 0)
            {
                foreach (DataRow row in dtStation.Rows)
                {
                    lineList.Add(row["linename"].ToString());
                }
            }
            else
            {
                throw new Exception("no line in system!");
            }
            return lineList;
        }
        public override void Run()
        {

            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            string skuno = Skuno.Value.ToString();
            string sn = Sn.Value.ToString();
            string line = Line.Value.ToString();
            string align = Align.Value.ToString();
            string other = Other.Value.ToString();
            string shot = Short.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string sqlRun = $@"select skuno,sn,BGA_LOCATION bga,status,misalignment align,void,replace(isshort,0,'NO') shot,replace(other,0,'NO') other,
                            customer BUName, linename line,edit_emp,edit_time from r_2dxray where 1=1 
                            temp_skuno
                            temp_sn
                            temp_line
                            temp_align
                            temp_other
                            temp_shot
                            and edit_time BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') 
                            and TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS') ORDER BY edit_time ";
            if (skuno != "ALL" && skuno != "")
            {
                sqlRun = sqlRun.Replace("temp_skuno", $@" and skuno = '{skuno}'");
            }
            else {
                sqlRun = sqlRun.Replace("temp_skuno", "");
            }
            if (sn != "")
            {
                sqlRun = sqlRun.Replace("temp_sn", $@" and sn = '{sn}'");
            }
            else
            {
                sqlRun = sqlRun.Replace("temp_sn", "");
            }
            if (line != "ALL" && line != "")
            {
                sqlRun = sqlRun.Replace("temp_line", $@" and linename = '{line}'");
            }
            else
            {
                sqlRun = sqlRun.Replace("temp_line", "");
            }
            if (align != "ALL" && align != "")
            {
                sqlRun = sqlRun.Replace("temp_align", $@" and misalignment = '{align}'");
            }
            else
            {
                sqlRun = sqlRun.Replace("temp_align", "");
            }
            if (other != "ALL" && other != "")
            {
                if (other == "YES")
                {
                    other = "1";
                }
                else {
                    other = "0";
                }
                sqlRun = sqlRun.Replace("temp_other", $@" and other = '{other}'");
            }
            else
            {
                sqlRun = sqlRun.Replace("temp_other", "");
            }
            if (shot != "ALL" && shot != "")
            {
                if (shot == "YES")
                {
                    shot = "1";
                }
                else {
                    shot = "0";
                }
                sqlRun = sqlRun.Replace("temp_shot", $@" and isshort = '{shot}'");
            }
            else
            {
                sqlRun = sqlRun.Replace("temp_shot", "");
            }
            try {
                DataTable res = SFCDB.ExecuteDataTable(sqlRun, CommandType.Text);
                if (res.Rows.Count == 0)
                {
                    throw new Exception("No data");
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res, null);
                retTab.Tittle = "2DXRayReport";
                //retTab.ColNames.RemoveAt(0);//為什麼要刪除?
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);

            } catch (Exception ex) {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}
