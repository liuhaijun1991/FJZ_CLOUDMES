using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class HWDxrayReport : ReportBase
    {
        ReportInput Wo = new ReportInput() { Name = "Wo", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Skuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Line = new ReportInput() { Name = "Line", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "B32S1", "B32S2" } };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public HWDxrayReport()
        {
            Inputs.Add(Wo);
            Inputs.Add(Skuno);
            Inputs.Add(Line);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }

        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                StartTime.Value = DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd 08:00:00")) ? DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd 08:00:00")) : DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd 20:00:00"));
                EndTime.Value = DateTime.Now;
                InitLine(SFCDB);
                //DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public override void Run()
        {

            string wo = Wo.Value.ToString();
            string skuno = Skuno.Value.ToString();
            string line = Line.Value.ToString();
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            DataTable dt = null;
            ReportTable retTab = new ReportTable();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {

                string sqlRet = $@"SELECT A.SNID PANELSN,A.WO,A.STATION,A.LINE,A.SKUNO,B.RESULT,B.REMARK,A.EDIT_EMP,A.EDIT_TIME FROM R_XRAY_DETAIL_HWD A,R_XRAY_HEAD_HWD B WHERE 1 =1 AND A.XRAYID = B.ID ";
                if (wo != "ALL" && wo != "")
                {
                    sqlRet = sqlRet + $@" AND A.WO = '{wo}'";
                }
                if (skuno != "ALL" && skuno != "")
                {
                    sqlRet = sqlRet + $@" AND A.SKUNO = '{skuno}'";
                }
                if (line != "ALL" && line != "")
                {
                    sqlRet = sqlRet + $@" AND A.LINE = '{line}'";
                }
                sqlRet = sqlRet + $@" AND A.EDIT_TIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') 
                            and TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS') ORDER BY A.SKUNO,A.WO,A.EDIT_TIME ";
                dt = SFCDB.RunSelect(sqlRet).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (dt.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }
                retTab.LoadData(dt, null);
                retTab.Tittle = "XRAY Report";
                Outputs.Add(retTab);

            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }

        }
        public void InitLine(OleExec db)
        {
            List<string> line = new List<string>();
            DataTable dt = new DataTable();
            T_C_LINE S = new T_C_LINE(db, DB_TYPE_ENUM.Oracle);
            dt = S.GetAllLine(db);
            line.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                line.Add(dr["line_name"].ToString());

            }
            Line.ValueForUse = line;
        }
    }
}
