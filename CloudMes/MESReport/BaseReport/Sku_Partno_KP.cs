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
    public class Sku_Partno_KP : ReportBase
    {
        ReportInput inputSku = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCurrentStation = new ReportInput() { Name = "NowStation", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput inputStartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2017/02/01 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2017/02/01 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputPartno = new ReportInput() { Name = "Partno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputBinddingStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        List<R_SN_KP> listAllKP = null;
        public Sku_Partno_KP()
        {
            Inputs.Add(inputSku);
            Inputs.Add(inputCurrentStation);
            Inputs.Add(inputStartTime);
            Inputs.Add(inputEndTime);
            Inputs.Add(inputPartno);
            Inputs.Add(inputBinddingStation);
        }

        public override void Init()
        {

            OleExec SFCDB =null;
            try
            {
                inputStartTime.Value = System.DateTime.Now.AddDays(-7);
                inputEndTime.Value = DateTime.Now;
                SFCDB = DBPools["SFCDB"].Borrow();
                List<string> cStation = new List<string>();
                List<string> listTmep = SFCDB.ORM.Queryable<C_STATION>().OrderBy(r => r.STATION_NAME)
                    .Select(r => r.STATION_NAME).ToList().Distinct().ToList<string>();
                List<string> lStation = SFCDB.ORM.Queryable<C_KP_List_Item>().OrderBy(r => r.STATION)
                    .Select(r => r.STATION).ToList().Distinct().ToList<string>();
                //inputCurrentStation.ValueForUse = SFCDB.ORM.Queryable<C_STATION>().OrderBy(r => r.STATION_NAME).Select(r => r.STATION_NAME).ToList().Distinct();
                //inputBinddingStation.ValueForUse = SFCDB.ORM.Queryable<C_KP_List_Item>().OrderBy(r => r.STATION).Select(r => r.STATION).ToList().Distinct();
                cStation.Add("ALL");
                cStation.AddRange(listTmep);
                inputCurrentStation.ValueForUse = cStation;
                inputCurrentStation.Value = cStation.FirstOrDefault();
                inputBinddingStation.ValueForUse = lStation;
                inputBinddingStation.Value = lStation.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //this.DBPools["SFCDB"].Return(SFCDB);
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void Run()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DateTime stime = Convert.ToDateTime(inputStartTime.Value);
                DateTime etime = Convert.ToDateTime(inputEndTime.Value);
                string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
                string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
                string sku = inputSku.Value.ToString();
                string currentStation = inputCurrentStation.Value.ToString();
                string partno = inputPartno.Value.ToString();
                string binddingStation = inputBinddingStation.Value.ToString();

                if (sku == "")
                {
                    ReportAlart alart = new ReportAlart("Please Input Skuno!");
                    Outputs.Add(alart);
                    return;
                }
                if (partno == "")
                {
                    ReportAlart alart = new ReportAlart("Please Input Partno!");
                    Outputs.Add(alart);
                    return;
                }
                string sql = "";                
                sql = $@"select a.sn,a.skuno,a.edit_time as pass_time,a.station_name as now_station,a.workorderno,b.workorderno as current_wo,
                        a.current_station,b.next_station from r_sn_station_detail a ,r_sn b
                        where  a.skuno='{sku}' 
                        Temp_sql
                        and b.valid_flag='1' and a.sn=b.sn
                        and a.edit_time between to_date('{svalue}','yyyy/mm/dd hh24:mi:ss')
                        and to_date('{evalue}','yyyy/mm/dd hh24:mi:ss')  ";


                if (currentStation != "ALL")
                {
                    sql = sql.Replace("Temp_sql", $@" and a.Station_name = '{currentStation}' ");
                }
                else {
                    sql = sql.Replace("Temp_sql", " ");
                }
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);               
                if (dt.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }
                DataTable returnTable = new DataTable();                
                returnTable.Columns.Add(new DataColumn("SKUNO"));
                returnTable.Columns.Add(new DataColumn("SN"));
                returnTable.Columns.Add(new DataColumn("CURRENT_WO"));
                returnTable.Columns.Add(new DataColumn("CURRENT_STATION"));
                returnTable.Columns.Add(new DataColumn("NEXT_STATION"));
                returnTable.Columns.Add(new DataColumn("PARTNO"));
                returnTable.Columns.Add(new DataColumn("VALUE"));
                returnTable.Columns.Add(new DataColumn("STATION"));                
                returnTable.Columns.Add(new DataColumn("ASSY_TIME"));
                returnTable.Columns.Add(new DataColumn("NOW_STATION"));
                returnTable.Columns.Add(new DataColumn("PASS_TIME"));
                returnTable.Columns.Add(new DataColumn("WORKORDERNO"));

                DataRow dr;
                List<R_SN_KP> listTemp;
                foreach (DataRow row in dt.Rows)
                { 
                    listAllKP = new List<R_SN_KP>();
                    GetSnKP(SFCDB, row["SN"].ToString());
                    listTemp = new List<R_SN_KP>();
                    listTemp = listAllKP.FindAll(r => r.PARTNO == partno && r.STATION == binddingStation);
                    if (listTemp.Count > 0)
                    {
                        foreach (var kp in listTemp)
                        {
                            dr = returnTable.NewRow();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                dr[dc.ToString()] = row[dc.ToString()];
                            }
                            dr["PARTNO"] = kp.PARTNO;
                            dr["VALUE"] = kp.VALUE;
                            dr["STATION"] = kp.STATION;
                            dr["ASSY_TIME"] = kp.EDIT_TIME;
                            returnTable.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = returnTable.NewRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            dr[dc.ToString()] = row[dc.ToString()];
                        }
                        dr["PARTNO"] = "";
                        dr["VALUE"] = "";
                        dr["STATION"] = "";
                        dr["ASSY_TIME"] = "";
                        returnTable.Rows.Add(dr);
                    }
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(returnTable, null);
                reportTable.Tittle = "SKU_PARTNO_KP";
                Outputs.Add(reportTable);

            }
            catch (Exception exception)
            {               
                ReportAlart alart = new ReportAlart(exception.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        private void GetSnKP(MESDBHelper.OleExec DB, string sn)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB,MESDataObject.DB_TYPE_ENUM.Oracle);
            var list = t_r_sn_kp.GetKPListBYSN(sn, 1, DB);
            if (list.Count > 0)
            {
                listAllKP.AddRange(list);
                foreach (var kp in list)
                {
                    GetSnKP(DB, kp.VALUE);
                }
            }
        }
    }    

}
