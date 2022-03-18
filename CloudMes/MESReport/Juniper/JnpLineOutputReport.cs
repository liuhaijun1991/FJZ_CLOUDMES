using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;

namespace MESReport.Juniper
{
    public class JnpLineOutputReport: ReportBase
    {
        ReportInput Bu = new ReportInput() { Name = "Bu", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "HWD", "DCN" } };
        ReportInput Skuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "AOI1", "AOI2", "VI1", "VI2" } };
        ReportInput Line = new ReportInput() { Name = "Line", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "D32S1", "D32S2" } };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public JnpLineOutputReport()
        {
            Inputs.Add(Bu);
            Inputs.Add(Skuno);
            Inputs.Add(Station);
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
                InitBu(SFCDB);
                InitStation(SFCDB);
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

            string bu = Bu.Value.ToString();
            string station = Station.Value.ToString();
            string skuno = Skuno.Value.ToString();
            string line = Line.Value.ToString();
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            DataTable linkTable = new DataTable();
            DataTable dt = null;
            DataRow linkDataRow = null;
            ReportTable retTab = new ReportTable();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sqlline = $@"select line, skuno,sku_name, GROUPID,STATION_NAME,sum(EVENTPASS) pass ,sum(REPAIR_FAILED_FLAG) fail,'{svalue}' as StartTime,'{evalue}' as EndTime from(  
                               select a.line, a.skuno,b.sku_name,GROUPID, a.STATION_NAME, a.REPAIR_FAILED_FLAG INPUTUNIT,CASE WHEN a.VALID_FLAG = '1' AND a.REPAIR_FAILED_FLAG = '0' THEN 1 ELSE 0 END AS EVENTPASS, a.REPAIR_FAILED_FLAG
                    from r_sn_station_detail a left join (select distinct skuno,sku_name from c_sku) b on a.skuno=b.skuno  LEFT JOIN R_WO_GROUPID C ON A.WORKORDERNO=C.WO where 1=1 AND (A.SKUNO NOT LIKE '#%' AND A.SKUNO NOT LIKE '*%')";
                string sqlfiter = "";
                if (!string.IsNullOrEmpty(skuno))
                {
                    sqlfiter = sqlfiter + $@" and a.skuno = '{skuno}'";
                }
                if (line != "ALL")
                {
                    sqlfiter = sqlfiter + $@" and a.line = '{line}'";
                }
                if (station != "ALL")
                {
                    sqlfiter = sqlfiter + $@"AND a.STATION_NAME = '{station}'";
                }

                sqlline = sqlline + sqlfiter + $@"AND a.EDIT_TIME BETWEEN TO_DATE('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                AND TO_DATE('{evalue}','YYYY/MM/DD HH24:MI:SS'))
                                GROUP BY line, skuno,sku_name,GROUPID, STATION_NAME order by line";

                
                // DataSet res = SFCDB.RunSelect(sqlline);
                dt = SFCDB.RunSelect(sqlline).Tables[0];
                var e = SFCDB.ExecSQL(sqlline);
                linkTable.Columns.Add("line");
                linkTable.Columns.Add("skuno");
                linkTable.Columns.Add("sku_name");
                linkTable.Columns.Add("GROUPID");
                linkTable.Columns.Add("CURRENT_STATION");
                if (bu == "VERTIV") linkTable.Columns.Add("UPH"); //VERTIV多加個UPH
                linkTable.Columns.Add("pass");
                linkTable.Columns.Add("fail");
                linkTable.Columns.Add("StartTime");
                linkTable.Columns.Add("EndTime");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    linkDataRow = linkTable.NewRow();
                    linkDataRow["line"] = "";
                    linkDataRow["skuno"] = "";
                    linkDataRow["sku_name"] = "";
                    linkDataRow["GROUPID"] = "";
                    linkDataRow["CURRENT_STATION"] = "";
                    linkDataRow["StartTime"] = "";
                    linkDataRow["EndTime"] = "";
                    linkDataRow["pass"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNByLineOutReport&RunFlag=1&skuno=" + dt.Rows[i]["skuno"].ToString() + "&station_name=" + dt.Rows[i]["station_name"].ToString()
                        + "&line=" + dt.Rows[i]["line"].ToString() + "&StartTime=" + svalue + "&EndTime=" + evalue + "&PASS=" + "0";
                    linkDataRow["fail"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNByLineOutReport&RunFlag=1&skuno=" + dt.Rows[i]["skuno"].ToString() + "&station_name=" + dt.Rows[i]["station_name"].ToString()
                        + "&line=" + dt.Rows[i]["line"].ToString() + "&StartTime=" + svalue + "&EndTime=" + evalue + "&PASS=" + "1"; ;
                    linkTable.Rows.Add(linkDataRow);
                }
                //retTab.LoadData(res.Tables[0], null);
                retTab.LoadData(dt, linkTable);
                retTab.Tittle = "Line Output";
                Outputs.Add(retTab);
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ee;
            }

        }

        public void InitBu(OleExec db)
        {
            DataTable dt = new DataTable();
            List<string> allbu = new List<string>();
            T_C_BU bu = new T_C_BU(db, DB_TYPE_ENUM.Oracle);
            dt = bu.GetAllBu(db);
            allbu.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                allbu.Add(dr["BU"].ToString());
            }
            Bu.ValueForUse = allbu;
        }

        public void InitStation(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            T_C_ROUTE_DETAIL S = new T_C_ROUTE_DETAIL(db, DB_TYPE_ENUM.Oracle);
            dt = S.GetALLStation(db);
            station.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                station.Add(dr["station_name"].ToString());

            }
            Station.ValueForUse = station;
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
