using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNFailDetailReportBySKU : ReportBase
    {
        ReportInput inputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputLine = new ReportInput() { Name = "LINE", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput FromDay = new ReportInput() { Name = "FromDay", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ToDay = new ReportInput() { Name = "ToDay", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Type = new ReportInput() { Name = "Type", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Series = new ReportInput() { Name = "Series", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //VNQE 阮文論要求增加工單 20211019
        ReportInput Wo = new ReportInput() { Name = "Wo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SNFailDetailReportBySKU()
        {
            Inputs.Add(inputSku);
            //Inputs.Add(inputLine);
            Inputs.Add(FromDay);
            Inputs.Add(ToDay);
            Inputs.Add(Type);
            Inputs.Add(Series);
            Inputs.Add(Station);
            Inputs.Add(Wo);
        }
        public override void Run()
        {
            //base.Run();
            string sku = inputSku.Value.ToString();
            string fromDay = FromDay.Value.ToString().Replace("%20", " ");
            string toDay = ToDay.Value.ToString().Replace("%20", " ");
            string sqlRun = string.Empty;
            //string line = inputLine.Value.ToString();
            string type = Type.Value.ToString();
            string series = Series.Value.ToString();
            string station = Station.Value.ToString();
            string wo = Wo.Value.ToString();
            //add by TRANPHUONG 25/02/2021
            string finalSKU = string.Empty;
            if (sku.IndexOf(',') != -1)
            {
                List<string> result = sku.Split(new char[] { ',' }).ToList();
                string t = string.Empty;
                for (int i = 0; i < result.Count; i++)
                {
                    t += "'" + result[i].ToString() + "',";
                }
                finalSKU = t.Remove(t.Length - 1, 1);
            }
            else
            {
                finalSKU = "'" + sku + "'";
            }
            //end
            string finalWO = string.Empty;
            if (wo.IndexOf(',') != -1)
            {
                List<string> result = wo.Split(new char[] { ',' }).ToList();
                string t = string.Empty;
                for (int i = 0; i < result.Count; i++)
                {
                    t += "'" + result[i].ToString() + "',";
                }
                finalWO = t.Remove(t.Length - 1, 1);
            }
            else
            {
                finalWO = "'" + wo + "'";
            }
            DataTable dt = null;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                if (type == "INPUT")
                {
                    #region Input SQL
                    sqlRun = $@"
                    select t4.customer_name,
                           t1.skuno,
                           t1.workorderno,
                           t1.sn,
                           t1.current_station,
                           t1.next_station,
                           t1.repair_failed_flag
                      from r_sn t1, c_sku t2, c_series t3, c_customer t4
                     where 1 = 1
                       and t1.skuno = t2.skuno
                       and t2.c_series_id = t3.id
                       and t3.customer_id = t4.id
                       Temp_Series_Sql
                       and sn in
                           (select b.sn
                              from r_sn_station_detail b
                             where b.skuno in ({finalSKU})
                               Temp_Station_Sql
                               Temp_Wo_Sql
                               and b.valid_flag = 1
                               and b.edit_time between
                                   to_date('{fromDay}', 'yyyy-MM-dd hh24:mi:ss') and
                                   to_date('{toDay}', 'yyyy-MM-dd hh24:mi:ss'))
                       and valid_flag = 1
                     order by t4.customer_name, t1.skuno, t1.sn";
                    #endregion

                    string tempStation = string.Empty;
                    if (!string.IsNullOrEmpty(station))
                    {
                        //tempStation = $@" and b.current_station = '{station}' ";//明明超鏈接上層查詢數據取的是station_name，怎麼點擊進來取的是current_station？Edit By ZHB 2021-09-24
                        tempStation = $@" and b.station_name = '{station}' ";
                    }
                    string tempSeries = string.Empty;
                    if (!string.IsNullOrEmpty(series))
                    {
                        tempSeries = $@" and t4.customer_name = '{series}' ";
                    }
                    string tempWo = string.Empty;
                    if (!string.IsNullOrEmpty(wo))
                    {
                        if (wo.IndexOf(',') != -1)
                            tempWo = $@" and b.workorderno in ({wo}) ";
                        else
                            tempWo = $@" and b.workorderno = '{wo}' ";
                    }
                    sqlRun = sqlRun.Replace("Temp_Station_Sql", tempStation);
                    sqlRun = sqlRun.Replace("Temp_Series_Sql", tempSeries);
                    sqlRun = sqlRun.Replace("Temp_Wo_Sql", tempWo);
                }
                else if (type == "FAILTOTAL")
                {
                    #region FailTotal SQL
                    sqlRun = $@"
                    select t3.series,
                           t3.skuno,
                           t3.workorderno,
                           t3.fail_station,
                           t3.fail_line,
                           t3.sn,
                           t3.SymptomDesc,
                           t3.scanLocation,
                           t3.fail_time,
                           t5.english_description FailureDesc,
                           t6.english_description ActionDesc,
                           t4.fail_location repairLocation,
                           t4.repair_time,
                           case nvl(t7.sn, 'Fail') when 'Fail' then 'Fail' else 'Pass' end status,
                           nvl(t4.repair_emp, t3.edit_emp) repair_emp,t8.sn as X8_SN,
                           t4.keypart_sn  kp_sn,
                           t9.SKUNO kp_pn
                      from (select t5.customer_name series,
                                   t1.skuno,
                                   t6.workorderno,
                                   t1.fail_station,
                                   t1.fail_line,
                                   t1.sn,
                                   t2.description SymptomDesc,
                                   case t1.skuno when '23-0000028-01' then 'U108' when '23-0000037-01' then 'U109' when '23-0000036-01' then 'U110' else t2.fail_location end scanLocation,
                                   t1.fail_time,
                                   t2.fail_code,
                                   t2.id,
                                   t1.edit_emp
                              from r_repair_main     t1,
                                   r_repair_failcode t2,
                                   c_sku             t3,
                                   c_series          t4,
                                   c_customer        t5,
                                   r_sn              t6
                             where 1 = 1
                               and t1.id = t2.repair_main_id
                               and t1.skuno = t3.skuno
                               and t3.c_series_id = t4.id
                               and t4.customer_id = t5.id
                               and t1.sn = t6.sn
                               and t6.valid_flag = 1
                               Temp_Series_Sql
                               and (t1.sn, t1.fail_station, t1.fail_time) in
                                   (select a.sn,
                                           a.next_station fail_station,
                                           a.edit_time    fail_time
                                      from r_sn_station_detail a
                                     where 1 = 1
                                       and a.valid_flag = 1
                                       and a.repair_failed_flag = 1
                                       and a.skuno in ({finalSKU})
                                       Temp_Station_Sql
                                       Temp_Wo_Sql
                                       and a.edit_time between to_date('{fromDay}', 'yyyy-MM-dd hh24:mi:ss') and to_date('{toDay}', 'yyyy-MM-dd hh24:mi:ss'))) t3
                      left join r_repair_action t4
                        on t3.id = t4.repair_failcode_id
                      left join c_error_code t5
                        on t4.reason_code = t5.error_code
                      left join c_action_code t6
                        on t4.action_code = t6.action_code
                      left join r_sn_station_detail t7
                        on t3.sn = t7.sn
                       and t3.fail_station = t7.current_station
                       and t7.valid_flag = 1
                      left join r_sn_kp t8
                      on t3.sn=t8.value 
                       left join r_sn t9
                      on t4.keypart_sn=t9.sn
                     where 1 = 1
                     order by t3.series, t3.skuno, t3.sn, t3.fail_time";
                    #endregion

                    string tempStation = string.Empty;
                    if (!string.IsNullOrEmpty(station))
                    {
                        tempStation = $@" and a.station_name = '{station}' ";
                    }
                    string tempSeries = string.Empty;
                    if (!string.IsNullOrEmpty(series))
                    {
                        tempSeries = $@" and t5.customer_name = '{series}' ";
                    }
                    string tempWo = string.Empty;
                    if (!string.IsNullOrEmpty(wo))
                    {
                        if (wo.IndexOf(',') != -1)
                            tempWo = $@" and a.workorderno in ({wo}) ";
                        else
                            tempWo = $@" and a.workorderno = '{wo}' ";
                    }
                    sqlRun = sqlRun.Replace("Temp_Station_Sql", tempStation);
                    sqlRun = sqlRun.Replace("Temp_Series_Sql", tempSeries);
                    sqlRun = sqlRun.Replace("Temp_Wo_Sql", tempWo);
                }
                ReportTable reportTable = new ReportTable();
                dt = SFCDB.RunSelect(sqlRun).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "SN Status";
                Outputs.Add(reportTable);

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
