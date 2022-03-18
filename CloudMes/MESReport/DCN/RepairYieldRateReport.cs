using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESReport.DCN
{
    /// <summary>
    /// 維修良率報表For DCN
    /// </summary>
    public class RepairYieldRateReport : MesAPIBase
    {
        protected APIInfo FGetReport = new APIInfo()
        {
            FunctionName = "GetReport",
            Description = "獲取維修良率報表數據",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "RepairEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DateFrom", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DateTo", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetSubReport = new APIInfo()
        {
            FunctionName = "GetSubReport",
            Description = "獲取維修良率子報表數據",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "RepairEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DateFrom", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DateTo", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetDetailReport = new APIInfo()
        {
            FunctionName = "GetDetailReport",
            Description = "獲取維修良率子報表詳細數據",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "RepairEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DateFrom", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DateTo", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public RepairYieldRateReport()
        {
            this.Apis.Add(FGetReport.FunctionName, FGetReport);
            this.Apis.Add(FGetSubReport.FunctionName, FGetSubReport);
            this.Apis.Add(FGetDetailReport.FunctionName, FGetDetailReport);
        }

        public void GetReport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string repairEmp = Data["RepairEmp"].ToString();
                string dateFrom = Data["DateFrom"].ToString();
                string dateTo = Data["DateTo"].ToString();

                #region FirstRate & SecondRate & ThirdRate 舊語句
                //string firstRateSql = $@"
                //    select tt.edit_emp, 
                //           nvl(qty, 0) || '/' || (nvl(qty, 0) + nvl(failqty, 0)) || '=' || round(nvl(qty, 0) / (nvl(qty, 0) + nvl(failqty, 0)), 4) * 100 || '%' passedrate 
                //     from (select b.edit_emp, count(a.sn) qty
                //             from (select count(sn) times, sn, fail_station from r_repair_main group by sn, fail_station) a, r_repair_main b, r_sn c
                //            where a.sn = b.sn and a.sn = c.sn and a.times = 1 and b.closed_flag = 1 and c.valid_flag = 1 
                //              and b.fail_station <> c.next_station and a.fail_station = b.fail_station
                //              and substr(b.sn, 0, 1) not in ('~','*','#') TEMP_EMPSQL
                //              and b.edit_time between to_date('{dateFrom}','yyyy-mm-dd hh24:mi') and to_date('{dateTo}','yyyy-mm-dd hh24:mi')
                //            group by b.edit_emp
                //    ) tt left join (
                //           select b.edit_emp, count(a.sn) failqty
                //             from (select count(sn) times, sn, fail_station from r_repair_main group by sn, fail_station) a, r_repair_main b
                //            where a.sn = b.sn and a.times = 1 and b.closed_flag = 1
                //              and a.fail_station = b.fail_station
                //              and substr(a.sn, 0, 1) not in ('~','*','#')
                //              and exists (select * from r_sn_station_detail c where a.sn = c.sn and a.fail_station = c.station_name and c.repair_failed_flag = 1 and b.edit_time < c.edit_time)
                //              TEMP_EMPSQL
                //              and b.edit_time between to_date('{dateFrom}','yyyy-mm-dd hh24:mi') and to_date('{dateTo}','yyyy-mm-dd hh24:mi')
                //            group by b.edit_emp
                //    ) failtt on tt.edit_emp = failtt.edit_emp";

                //string secondRateSql = firstRateSql.Replace("a.times = 1", "a.times = 2");
                //string thirdRateSql = firstRateSql.Replace("a.times = 1", "a.times = 3");
                #endregion

                #region  FirstRate & SecondRate & ThirdRate  新語句 wuqing 20201211 
                string firstRateSql = $@"
                    select tt.edit_emp, 
                           nvl(qty, 0) || '/' || (nvl(total, 0)) || '=' || to_char(round(nvl(qty, 0) / (nvl(total, 0)), 4) * 100,'fm999990.00') || '%' passedrate 
                     from (select b.edit_emp, count(a.sn) qty
                             from (select count(sn) times, sn, fail_station from r_repair_main group by sn, fail_station) a, r_repair_main b, r_sn c
                            where a.sn = b.sn and a.sn = c.sn and a.times = 1 and b.closed_flag = 1 and c.valid_flag = 1 
                              and b.fail_station <> c.next_station and a.fail_station = b.fail_station
                              and substr(b.sn, 0, 1) not in ('~','*','#') TEMP_EMPSQL
                              and b.edit_time between to_date('{dateFrom}','yyyy-mm-dd hh24:mi') and to_date('{dateTo}','yyyy-mm-dd hh24:mi')
                            group by b.edit_emp
                    ) tt left join (
                        select b.edit_emp, count(b.sn) total from r_repair_main b where b.closed_flag = 1
                         and substr(b.sn, 0, 1) not in ('~', '*', '#') TEMP_EMPSQL
                           and b.edit_time between to_date('{dateFrom}', 'yyyy-mm-dd hh24:mi') and to_date('{dateTo}', 'yyyy-mm-dd hh24:mi')
                           group by b.edit_emp
                    ) total on tt.edit_emp = total.edit_emp";

                string secondRateSql = firstRateSql.Replace("a.times = 1", "a.times = 2");
                string thirdRateSql = firstRateSql.Replace("a.times = 1", "a.times = 3");
                #endregion

                #region 拼接SQL語句
                string runSql = $@"
                    select t1.edit_emp ""RepairEmp"", 
                           nvl(total, 0) ""RepairQty"", 
                           nvl(t2.passedrate, '0.00%') ""FirstRate"", 
                           nvl(t3.passedrate, '0.00%') ""SecondRate"", 
                           nvl(t4.passedrate, '0.00%') ""ThirdRate""
                      from (
                    select b.edit_emp, count(b.sn) total from r_repair_main b where b.closed_flag = 1
                       and substr(b.sn, 0, 1) not in ('~', '*', '#') TEMP_EMPSQL
                       and b.edit_time between to_date('{dateFrom}', 'yyyy-mm-dd hh24:mi') and to_date('{dateTo}', 'yyyy-mm-dd hh24:mi')
                     group by b.edit_emp
                    ) t1 left join(";
                runSql += firstRateSql + ") t2 on t1.edit_emp = t2.edit_emp left join (";
                runSql += secondRateSql + ") t3 on t1.edit_emp = t3.edit_emp left join (";
                runSql += thirdRateSql + ") t4 on t1.edit_emp = t4.edit_emp order by 1";

                if (!string.IsNullOrEmpty(repairEmp))
                {
                    runSql = runSql.Replace("TEMP_EMPSQL", $" and b.edit_emp = '{repairEmp}' ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_EMPSQL", "");
                }
                #endregion

                DataTable runDT = SFCDB.RunSelect(runSql).Tables[0];
                float total = 0;
                float Num_FirstRate = 0;
                float Num_SecondRate = 0;
                float Num_ThirdRate = 0;
                //String testing = "text that i am looking for";
                //Console.Write(testing.IndexOf("looking") + Environment.NewLine);
                //Console.WriteLine(testing.Substring(0, testing.IndexOf("looking")));
                for (int i = 0; i < runDT.Rows.Count; i++)
                {
                    string FirstRate = runDT.Rows[i][2].ToString();
                    string SecondRate = runDT.Rows[i][3].ToString();
                    string ThirdRate = runDT.Rows[i][4].ToString();
                    total += int.Parse(runDT.Rows[i][1].ToString());
                    if (FirstRate != "0.00%")
                    {
                        Num_FirstRate += float.Parse(FirstRate.Substring(0, FirstRate.IndexOf("/")));
                    }
                    if (SecondRate != "0.00%")
                    {
                        Num_SecondRate += float.Parse(SecondRate.Substring(0, SecondRate.IndexOf("/")));
                    }
                    if (ThirdRate != "0.00%")
                    {
                        Num_ThirdRate += float.Parse(ThirdRate.Substring(0, ThirdRate.IndexOf("/")));
                    }
                }

                var totalRow = runDT.NewRow();
                totalRow[0] = "Total";
                totalRow[1] = total;
                if (Num_FirstRate != 0) totalRow[2] = Num_FirstRate + "/" + total + "=" + (Num_FirstRate / total) * 100 + "%"; else totalRow[2] = "0.00%";
                if (Num_SecondRate != 0) totalRow[3] = Num_SecondRate + "/" + total + "=" + (Num_SecondRate / total) * 100 + "%"; else totalRow[3] = "0.00 %";
                if (Num_ThirdRate != 0) totalRow[4] = Num_ThirdRate + "/" + total + "=" + (Num_ThirdRate / total) * 100 + "%"; else totalRow[4] = "0.00 %";
                //totalRow[5] = (Math.Round(((double)totalFial / totalInput) * 100, 2)).ToString() + "%";
                //totalRow[5] = (Math.Round(((double)(totalInput - totalFial) / totalInput) * 100, 2)).ToString() + "%";;
                if (runDT.Rows.Count > 0)
                    runDT.Rows.Add(totalRow);

                if (runDT.Rows.Count > 0)
                {
                    StationReturn.Message = "Get success !";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = runDT;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception ex)
            {
                StationReturn.Message = ex.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetSubReport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string repairEmp = Data["RepairEmp"].ToString();
                string dateFrom = Data["DateFrom"].ToString();
                string dateTo = Data["DateTo"].ToString();

                #region FirstRate & SecondRate & ThirdRate SQL語句
                string firstRateSql = $@"
                    select tt.edit_time day_id, 
                           nvl(qty, 0) || '/' || (nvl(qty, 0) + nvl(failqty, 0)) || '=' || round(nvl(qty, 0) / nvl(qty, 0) + nvl(failqty, 0), 4) * 100 || '%' rate 
                     from (select to_char(b.edit_time, 'yyyy-mm-dd') edit_time, count(a.sn) qty
                             from (select count(sn) times, sn, fail_station from r_repair_main group by sn, fail_station) a, r_repair_main b, r_sn c
                            where a.sn = b.sn and a.sn = c.sn and a.times = 1 and b.closed_flag = 1 and c.valid_flag = 1 
                              and b.fail_station <> c.next_station and a.fail_station = b.fail_station
                              and substr(b.sn, 0, 1) not in ('~','*','#') and b.edit_emp = '{repairEmp}'
                              and b.edit_time between to_date('{dateFrom}','yyyy-mm-dd hh24:mi') and to_date('{dateTo}','yyyy-mm-dd hh24:mi')
                            group by to_char(b.edit_time, 'yyyy-mm-dd')
                    ) tt left join (
                           select to_char(b.edit_time, 'yyyy-mm-dd') edit_time, count(a.sn) failqty
                             from (select count(sn) times, sn, fail_station from r_repair_main group by sn, fail_station) a, r_repair_main b
                            where a.sn = b.sn and a.times = 1 and b.closed_flag = 1
                              and a.fail_station = b.fail_station
                              and substr(a.sn, 0, 1) not in ('~','*','#')
                              and exists (select * from r_sn_station_detail c where a.sn = c.sn and a.fail_station = c.station_name and c.repair_failed_flag = 1 and b.edit_time < c.edit_time)
                              and b.edit_emp = '{repairEmp}'
                              and b.edit_time between to_date('{dateFrom}','yyyy-mm-dd hh24:mi') and to_date('{dateTo}','yyyy-mm-dd hh24:mi')
                            group by to_char(b.edit_time, 'yyyy-mm-dd')
                    ) failtt on tt.edit_time = failtt.edit_time";

                string secondRateSql = firstRateSql.Replace("a.times = 1", "a.times = 2");
                string thirdRateSql = firstRateSql.Replace("a.times = 1", "a.times = 3");
                #endregion

                #region 拼接SQL語句
                string dayFrom = Convert.ToDateTime(dateFrom).ToString("yyyy-MM-dd");
                string dayTo = Convert.ToDateTime(dateTo).AddDays(1).ToString("yyyy-MM-dd");
                string runSql = $@"
                    select t1.day_id ""Day"", 
                           nvl(ext1.total, '0') ""Total"",
                           nvl(t2.rate, '0.00%') ""FirstRate"",
                           nvl(t3.rate, '0.00%') ""SecondRate"", 
                           nvl(t4.rate, '0.00%') ""ThirdRate""
                      from (
                    select to_char(to_date('{dayFrom}', 'yyyy-mm-dd') + rownum - 1, 'yyyy-mm-dd') day_id from dual
                     connect by rownum <= to_date('{dayTo}', 'yyyy-mm-dd') - to_date('{dayFrom}', 'yyyy-mm-dd')
                    ) t1 left join(
                    select to_char(b.edit_time, 'yyyy-mm-dd') day_id, count(b.sn) total from r_repair_main b where b.closed_flag = 1
                       and substr(b.sn, 0, 1) not in ('~', '*', '#') and b.edit_emp = '{repairEmp}'
                       and b.edit_time between to_date('{dateFrom}', 'yyyy-mm-dd hh24:mi') and to_date('{dateTo}', 'yyyy-mm-dd hh24:mi')
                     group by to_char(b.edit_time, 'yyyy-mm-dd')
                    ) ext1 on t1.day_id = ext1.day_id left join (";
                runSql += firstRateSql + ") t2 on t1.day_id = t2.day_id left join (";
                runSql += secondRateSql + ") t3 on t1.day_id = t3.day_id left join (";
                runSql += thirdRateSql + ") t4 on t1.day_id = t4.day_id order by 1";
                #endregion

                DataTable runDT = SFCDB.RunSelect(runSql).Tables[0];
                if (runDT.Rows.Count > 0)
                {
                    StationReturn.Message = "Get success !";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = runDT;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception ex)
            {
                StationReturn.Message = ex.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetDetailReport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string repairEmp = Data["RepairEmp"].ToString();
                string dateFrom = Data["DateFrom"].ToString();
                string dateTo = Data["DateTo"].ToString();

                string runSql = $@"
                    select a.skuno,a.fail_station station,a.sn,b.fail_code,b.description fail_desc,c.fail_location location,a.fail_time,
                           c.description action_desc,case when a.fail_station = d.next_station then 'Fail' else 'Pass' end status, c.repair_emp,c.repair_time
                      from r_repair_main a, r_repair_failcode b, r_repair_action c, r_sn d
                     where a.id = b.repair_main_id
                       and a.sn = b.sn and b.id = c.repair_failcode_id and b.sn = c.sn
                       and a.sn = d.sn and d.valid_flag = 1
                       and a.closed_flag = 1
                       and substr(a.sn, 0, 1) not in ('~', '*', '#')
                       and a.edit_emp = '{repairEmp}'
                       and a.edit_time between
                           to_date('{dateFrom}', 'yyyy-mm-dd hh24:mi') and
                           to_date('{dateTo}', 'yyyy-mm-dd hh24:mi')
                     order by a.sn";

                DataTable runDT = SFCDB.RunSelect(runSql).Tables[0];
                if (runDT.Rows.Count > 0)
                {
                    StationReturn.Message = "Get success !";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = runDT;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception ex)
            {
                StationReturn.Message = ex.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
