using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
namespace MESReport.HWT
{
  
    public class OutputReport : ReportBase
    {
        
        ReportInput Station = new ReportInput() { Name = "EventPoint", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "AOI1", "AOI2", "VI1", "VI2" } };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "D32S1", "D32S2" } };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public OutputReport()
        {
    
            Inputs.Add(Station);
            Inputs.Add(SkuNo);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }

        public override void Init()
        {
            try
            {
                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                InitStation(SFCDB);
                InitSkuno(SFCDB);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Run()
        {

           
            string station = Station.Value.ToString();
            string Skuno = SkuNo.Value.ToString();
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            string sqlline = "";
            DataTable linkTable = new DataTable();
            DataTable dt = null;
            //DataRow linkDataRow = null;
            ReportTable retTab = new ReportTable();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {

                if (Skuno == "ALL" && station == "ALL")
                {
                    sqlline = $@"SELECT a.skuno AS skuno,
                                a.Station_Name  AS eventpoint,COUNT(a.Sn) AS total,
                                nvl(DECODE(a.Station_Name, 'AOI2', B_POINT, 'AOI4', T_POINT, 0), 0) as face_POINT,
                                nvl(DECODE(a.Station_Name,'AOI2', (c.TOTAL_POINT),'AOI4',(c.TOTAL_POINT),0),0) as SUM_POINT, nvl(DECODE(a.Station_Name,
                               'AOI2', COUNT(a.SN) * (c.B_POINT),'AOI4',COUNT(a.SN) * (c.T_POINT), 0),0) as TOTAL_POINT,
                                COUNT(B.SN) AS LOCK_TOTAL, COUNT(A.SKUNO) - COUNT(b.SN) DN_CREATE_QTY
                                FROM r_sn_station_detail a, (SELECT *FROM r_sn_lock WHERE LOCK_STATION = 'SHIPPING' AND LOCK_STATUS = 1) b,
                                c_sku_point_config c WHERE a.valid_flag = 1  and c.skuno(+) = a.skuno
                                and a.SN = b.SN(+) and A.EDIT_TIME >= to_date('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                and A.EDIT_TIME <= to_date('{evalue}', 'YYYY/MM/DD HH24:MI:SS')
                                GROUP BY a.skuno, A.STATION_NAME, c.B_POINT, c.T_POINT, TOTAL_POINT
                                ORDER BY a.skuno, A.STATION_NAME";
                }
                else if (Skuno == "ALL")
                {
                    if (station == "SMT產出點數")
                    {
                        sqlline = $@"select SKUNO, EVENTPOINT,
                                    sum(TOTAL) as TOTAL,sum(TOTAL_POINT) as TOTAL_POINT,
                                    sum(LOCK_TOTAL) as LOCK_TOTAL,sum(DN_CREATE_QTY) as DN_CREATE_QTY
                                    from (SELECT a.skuno AS skuno,DECODE(a.station_name,'AOI2', 'SMT產出點數', 'AOI4','SMT產出點數', a.station_name) AS eventpoint,
                                   COUNT(a.sn) AS total,nvl(DECODE(a.station_name, 'AOI2', B_POINT, 'AOI4', T_POINT, 0), 0) as face_POINT,
                                   nvl(DECODE(a.station_name, 'AOI2',(c.TOTAL_POINT), 'AOI4',(c.TOTAL_POINT), 0), 0) as SUM_POINT,
                                   nvl(DECODE(a.station_name,'AOI2', COUNT(a.sn) * (c.b_POINT),'AOI4', COUNT(a.sn) * (c.T_POINT), 0),0) as TOTAL_POINT,
                                   COUNT(B.sn) AS LOCK_TOTAL, COUNT(a.sn) - COUNT(b.sn) DN_CREATE_QTY
                                  FROM r_sn_station_detail a, (SELECT * FROM r_Sn_Lock WHERE lock_Station = 'SHIPPING' AND lock_STATUS = 1) b,
                                  c_sku_point_config c WHERE a.valid_flag = 1 and c.skuno(+) = a.skuno and a.sn = b.sn(+)
                                   and a.station_name in ('AOI2', 'AOI4') and a.edit_time >= to_date('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   and a.edit_time <= to_date('{evalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   GROUP BY a.skuno, a.station_name, c.B_POINT, c.T_POINT, TOTAL_POINT
                                   ORDER BY a.skuno, a.station_name) GROUP BY SKUNO, EVENTPOINT";
                    }
                    else
                    {
                        sqlline = $@"SELECT a.skuno AS skuno, a.station_name AS eventpoint, COUNT(a.sn) AS total,
                                   nvl(DECODE(a.station_name, 'AOI2', B_POINT, 'AOI4', T_POINT, 0), 0) as face_POINT,
                                   nvl(DECODE(a.station_name,'AOI2', (c.TOTAL_POINT), 'AOI4',(c.TOTAL_POINT), 0), 0) as SUM_POINT,
                                   nvl(DECODE(a.station_name,'AOI2', COUNT(a.sn) * (c.b_POINT), 'AOI4',COUNT(a.sn) * (c.T_POINT), 0),0) as TOTAL_POINT,
                                   COUNT(B.sn) AS LOCK_TOTAL, COUNT(a.sn) - COUNT(b.sn) DN_CREATE_QTY
                                   FROM r_sn_station_detail a, (SELECT * FROM r_sn_lock WHERE lock_station = 'SHIPPING' AND lock_STATUS = 1) b, c_sku_point_config c
                                   WHERE a.valid_flag = 1 and c.skuno(+) = a.skuno and a.sn = b.sn(+) and a.station_name ='{station}'
                                   and a.edit_time >= to_date('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   and a.edit_time <= to_date('{evalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   GROUP BY a.skuno, a.station_name, c.B_POINT, c.T_POINT, TOTAL_POINT ORDER BY a.skuno, a.station_name";

                    }

                }
                else if (station == "ALL")
                {
                    sqlline = $@"SELECT a.skuno AS skuno, a.station_name AS eventpoint, COUNT(a.sn) AS total,
                                   nvl(DECODE(a.station_name, 'AOI2', B_POINT, 'AOI4', T_POINT, 0), 0) as face_POINT,
                                   nvl(DECODE(a.station_name,'AOI2', (c.TOTAL_POINT), 'AOI4',(c.TOTAL_POINT), 0), 0) as SUM_POINT,
                                   nvl(DECODE(a.station_name,'AOI2', COUNT(a.sn) * (c.b_POINT), 'AOI4',COUNT(a.sn) * (c.T_POINT), 0),0) as TOTAL_POINT,
                                   COUNT(B.sn) AS LOCK_TOTAL, COUNT(a.sn) - COUNT(b.sn) DN_CREATE_QTY
                                   FROM r_sn_station_detail a, (SELECT * FROM r_sn_lock WHERE lock_station = 'SHIPPING' AND lock_STATUS = 1) b, c_sku_point_config c
                                   WHERE a.valid_flag = 1 and c.skuno(+) = a.skuno and a.sn = b.sn(+) and a.skuno='{Skuno}'
                                   and a.edit_time >= to_date('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   and a.edit_time <= to_date('{evalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   GROUP BY a.skuno, a.station_name, c.B_POINT, c.T_POINT, TOTAL_POINT ORDER BY a.skuno, a.station_name";
                }
                else if (Skuno != "ALL" && station == "SMT產出點數")
                {
                    sqlline = $@"select SKUNO,EVENTPOINT,sum(TOTAL) as TOTAL, sum(TOTAL_POINT) as TOTAL_POINT,
                                sum(LOCK_TOTAL) as LOCK_TOTAL, sum(DN_CREATE_QTY) as DN_CREATE_QTY
                                from (SELECT a.skuno AS skuno, DECODE(a.station_name, 'AOI2','SMT產出點數',
                                'AOI4','SMT產出點數', a.station_name) AS eventpoint, COUNT(a.sn) AS total,
                                nvl(DECODE(a.station_name, 'AOI2', B_POINT, 'AOI4', T_POINT, 0), 0) as face_POINT,
                                nvl(DECODE(a.station_name,'AOI2',(c.TOTAL_POINT),'AOI4',(c.TOTAL_POINT),0),0) as SUM_POINT,
                                nvl(DECODE(a.station_name,'AOI2', COUNT(a.sn) * (c.B_POINT),'AOI4',COUNT(a.sn) * (c.T_POINT), 0),0) as TOTAL_POINT,
                                COUNT(B.sn) AS LOCK_TOTAL,COUNT(a.sn) - COUNT(b.sn) DN_CREATE_QTY
                                FROM r_sn_station_detail a,(SELECT * FROM r_Sn_Lock WHERE EVENTPOINT = 'SHIPPING' AND STATUS = 1) b, c_sku_point_config c
                                WHERE a.eventpass = 1 and c.skuno(+) = a.skuno and a.sn = b.sn(+) and a.skuno ='{Skuno}'
                                and a.station_name in ('AOI2', 'AOI4')and a.edit_time >= to_date('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                and a.edit_time <= to_date('{evalue}', 'YYYY/MM/DD HH24:MI:SS')
                                GROUP BY a.skuno, a.station_name, c.B_POINT, c.T_POINT, TOTAL_POINT
                                ORDER BY a.skuno, a.station_name) GROUP BY SKUNO, EVENTPOINT";
                }
                else
                {
                    sqlline = $@"SELECT a.skuno AS skuno, a.station_name AS eventpoint, COUNT(a.sn) AS total,
                                   nvl(DECODE(a.station_name, 'AOI2', B_POINT, 'AOI4', T_POINT, 0), 0) as face_POINT,
                                   nvl(DECODE(a.station_name,'AOI2', (c.TOTAL_POINT), 'AOI4',(c.TOTAL_POINT), 0), 0) as SUM_POINT,
                                   nvl(DECODE(a.station_name,'AOI2', COUNT(a.sn) * (c.b_POINT), 'AOI4',COUNT(a.sn) * (c.T_POINT), 0),0) as TOTAL_POINT,
                                   COUNT(B.sn) AS LOCK_TOTAL, COUNT(a.sn) - COUNT(b.sn) DN_CREATE_QTY
                                   FROM r_sn_station_detail a, (SELECT * FROM r_sn_lock WHERE lock_station = 'SHIPPING' AND lock_STATUS = 1) b, c_sku_point_config c
                                   WHERE a.valid_flag = 1 and c.skuno(+) = a.skuno and a.sn = b.sn(+) and a.skuno='{Skuno}' and a.station_name='{station}'
                                   and a.edit_time >= to_date('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   and a.edit_time <= to_date('{evalue}', 'YYYY/MM/DD HH24:MI:SS')
                                   GROUP BY a.skuno, a.station_name, c.B_POINT, c.T_POINT, TOTAL_POINT ORDER BY a.skuno, a.station_name";
                }

                dt = SFCDB.RunSelect(sqlline).Tables[0];  
                retTab.LoadData(dt, null);
                retTab.Tittle = "Output Report";
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

        public void InitStation(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            T_C_ROUTE_DETAIL S = new T_C_ROUTE_DETAIL(db, DB_TYPE_ENUM.Oracle);
            dt = S.GetALLStation(db);
            station.Add("ALL");
            station.Add("SMT產出點數");
            foreach (DataRow dr in dt.Rows)
            {
                station.Add(dr["station_name"].ToString());

            }
            Station.ValueForUse = station;
        }

        public void InitSkuno(OleExec db)
        {
            List<string> Skuno = new List<string>();
            DataTable dt = new DataTable();
            T_C_SKU S = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
            dt = S.GetALLSkuno(db);
            Skuno.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                Skuno.Add(dr["SKUNO"].ToString());

            }
            SkuNo.ValueForUse = Skuno;
        }

        // GetAllLine

    }
}
