using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Vertiv
{
    class SkuWipByDateReport : ReportBase
    {
        DataTable dtStation, dtSku, dtHold, dtOrt, dtFail;
        ReportInput inputSKU = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput FromDay = new ReportInput() { Name = "FromDay", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ToDate = new ReportInput() { Name = "ToDay", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SkuWipByDateReport()
        {
            Inputs.Add(inputSKU);
            Inputs.Add(inputStation);
            Inputs.Add(FromDay);
            Inputs.Add(ToDate);
        }
        public override void Init()
        {
            try
            {
                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                FromDay.Value = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                ToDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }
        public override void Run()
        {
            string sku = inputSKU.Value.ToString();
            string station = inputStation.Value.ToString();
            string fromDate = Convert.ToDateTime(FromDay.Value).ToString("yyyy-MM-dd");
            string toDate = Convert.ToDateTime(ToDate.Value).ToString("yyyy-MM-dd");
            Dictionary<string, DataTable> openWith = new Dictionary<string, DataTable>();
            DataTable dt = new DataTable();
            DataRow dr;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            // base.Run();
            try
            {
                string sqlStation = $@"select * from  c_control where control_name = 'SFC_WIP_ROUTE' and control_type = 'ROUTE'  order by control_level";
                string sqlSku = $@"select ccs.description,a.skuno,b.sku_qty,a.station_wip,a.CURRENT_STATION from 
                                   (select rwo.skuno,count(rsn.SN) station_wip,rsn.CURRENT_STATION from r_sn rsn, r_wo_base rwo where rsn.workorderno = rwo.workorderno
                                   and rsn.valid_flag = 1 and rsn.REPAIR_FAILED_FLAG = 0
                                   and rsn.edit_time between to_date('{fromDate}', 'yyyy-mm-dd') and
                                   to_date('{toDate}', 'yyyy-mm-dd')
                                    Temp_Sql
                                    Temp_Station
                                    group by rwo.SKUNO,
                                    rwo.START_STATION,rsn.CURRENT_STATION ) a ,(select sum(workorder_qty) sku_qty,skuno from r_wo_base  group by  skuno) b ,(select * from c_sku ) c
                                    ,c_series csc,c_customer ccs
                                    WHERE a.skuno = b.skuno and b.skuno = c.skuno and c.c_series_id = csc.id and csc.customer_id = ccs.id
                                    order by a.skuno";


                string sqlHold = $@"select count(1) Hold ,skuno from r_sn where sn in (
                                    select sn from(select row_number() over(partition by  sn ORDER BY LOCK_TIME desc) rn, sn, lock_time from r_sn_lock where LOCK_STATUS = 1 ))
                                    Temp_Sql
                                    group by skuno
                                    order by skuno ";
                string sqlOrt = $@"select count(1) ORT ,'1' CriticalBonepile,skuno From r_sn where sn in(
                                SELECT sn  fROM R_LOT_DETAIL where lot_id in (select id From r_lot_status where SAMPLE_STATION = 'ORT') and substr(sn, 0, 1) <> '*' and substr(sn, 0, 1) <> '#')
                                and VALID_FLAG = 1 
                                Temp_Sql 
                                group by skuno 
                                ORDER BY skuno";

                string sqlFail = $@"select t1.skuno,
                                         t1.fail_Total,
                                         nvl(t2.checkin, 0) check_in,
                                         nvl(t3.checkout, 0) check_out,
                                         nvl(t4.inline, 0) in_line,
                                         nvl(t5.rewip, 0) re_wip
                                    from (select a.skuno, nvl(count(distinct a.sn), 0) fail_Total
                                            from r_repair_main a
                                           where 1=1
                                             and a.fail_station <> 'COSMETIC-FAILURE'
                                             and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2) <> 'RW%'
                                           group by a.skuno) t1
                                    left join (select b.skuno, nvl(count(distinct b.sn), 0) checkin
                                                 from r_repair_main a, r_repair_transfer b
                                                where 1=1 and a.id = b.repair_main_id
                                                  and a.fail_station <> 'COSMETIC-FAILURE'
                                                  and substr(a.sn, 0, 1)<> 'RW%' and substr(a.sn, 0, 1) <> '#'
                                                group by b.skuno) t2
                                      on t1.skuno = t2.skuno
                                    left join (select b.skuno, nvl(count(distinct b.sn), 0) checkout
                                                 from r_repair_main a, r_repair_transfer b
                                                where 1=1 and  a.id = b.repair_main_id
                                                  and b.closed_flag = 1
                                                  and a.fail_station <> 'COSMETIC-FAILURE'
                                                  and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2)<> 'RW%'
                                                group by b.skuno) t3
                                      on t1.skuno = t3.skuno
                                    left join (select a.skuno, nvl(count(distinct a.sn), 0) inline
                                                 from r_repair_main a
                                                where 1=1
                                                  and a.id not in
                                                      (select repair_main_id from r_repair_transfer)
                                                  and a.fail_station <> 'COSMETIC-FAILURE'
                                                  and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2)<> 'RW%'
                                                group by a.skuno) t4
                                      on t1.skuno = t4.skuno
                                    left join (select a.skuno, nvl(count(distinct a.sn), 0) rewip
                                                 from r_repair_main a
                                                where 1=1
                                                  and a.id in (select repair_main_id
                                                                 from r_repair_transfer
                                                                where closed_flag = 0)
                                                  and a.fail_station <> 'COSMETIC-FAILURE'
                                                  and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2)<> 'RW%'
                                                group by a.skuno) t5
                                      on t1.skuno = t5.skuno
                                   where substr(t1.skuno,0,1) <> '*' and substr(t1.skuno,0,1) <> '~'
                                     and t1.fail_Total > 0
                                    Temp_Sql
                                   order by t1.skuno";

                if (sku != "ALL" && sku != "")
                {
                    sqlSku = sqlSku.Replace("Temp_Sql", $@" and rsn.skuno = '{sku}'");
                    sqlHold = sqlHold.Replace("Temp_Sql", $@" and skuno = '{sku}'");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", $@" and skuno = '{sku}'");
                    sqlFail = sqlFail.Replace("Temp_Sql", $@"and t1.skuno = '{sku}'");
                }
                else
                {
                    sqlSku = sqlSku.Replace("Temp_Sql", "");
                    sqlHold = sqlHold.Replace("Temp_Sql", "");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", "");
                    sqlFail = sqlFail.Replace("Temp_Sql", "");
                }
                if (station != "ALL" && station != "")
                {
                    sqlSku = sqlSku.Replace("Temp_Station", $@" and rsn.CURRENT_STATION = '{station}'");
                }
                else
                {
                    sqlSku = sqlSku.Replace("Temp_Station", "");
                }
                dtStation = SFCDB.RunSelect(sqlStation).Tables[0];
                dtSku = SFCDB.RunSelect(sqlSku).Tables[0];
                dtHold = SFCDB.RunSelect(sqlHold).Tables[0];
                dtOrt = SFCDB.RunSelect(sqlOrt).Tables[0];
                dtFail = SFCDB.RunSelect(sqlFail).Tables[0];
                if (dtSku.Rows.Count == 0 || dtStation.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                else
                {
                    var data = dtSku.AsEnumerable();
                    var skuRow = data.Select(d => d.Field<string>("SKUNO")).Distinct();
                    for (var i = 0; i < dtSku.Columns.Count; i++)
                    {
                        dt.Columns.Add(dtSku.Columns[i].ColumnName.ToString(), typeof(string));
                    }
                    for (var i = 0; i < dtStation.Rows.Count; i++)
                    {
                        dt.Columns.Add(dtStation.Rows[i]["CONTROL_VALUE"].ToString(), typeof(string));
                    }
                    for (var i = 0; i < dtHold.Columns.Count; i++)
                    {
                        if (dtHold.Columns[i].ColumnName.ToString() != "SKUNO")
                        {
                            dt.Columns.Add(dtHold.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    for (var i = 0; i < dtOrt.Columns.Count; i++)
                    {
                        if (dtOrt.Columns[i].ColumnName.ToString() != "SKUNO")
                        {
                            dt.Columns.Add(dtOrt.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    for (var i = 0; i < dtFail.Columns.Count; i++)
                    {
                        if (dtFail.Columns[i].ColumnName.ToString() != "SKUNO")
                        {
                            dt.Columns.Add(dtFail.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }

                    if (dt.Columns.Contains("STATION_WIP")) dt.Columns.Remove("STATION_WIP");
                    if (dt.Columns.Contains("CURRENT_STATION")) dt.Columns.Remove("CURRENT_STATION");
                    foreach (var s in skuRow)
                    {
                        dr = dt.NewRow();
                        dr["SKUNO"] = s;
                        dt.Rows.Add(dr);
                    }
                }

                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtSku.Rows.Count; k++)
                    {
                        if (dt.Rows[j]["SKUNO"].ToString() == dtSku.Rows[k]["SKUNO"].ToString())
                        {
                            dt.Rows[j]["DESCRIPTION"] = dtSku.Rows[k]["DESCRIPTION"].ToString();
                            dt.Rows[j]["SKU_QTY"] = dtSku.Rows[k]["SKU_QTY"].ToString();
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                if (dt.Columns[t].ColumnName.ToString() == dtSku.Rows[k]["CURRENT_STATION"].ToString())
                                {
                                    dt.Rows[j][t] = dtSku.Rows[k]["STATION_WIP"].ToString();
                                }
                            }
                        }
                    }
                }

                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtHold.Rows.Count; k++)
                    {
                        if (dt.Rows[j]["SKUNO"].ToString() == dtHold.Rows[k]["SKUNO"].ToString())
                        {
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                if (dt.Columns[t].ColumnName.ToString() == "HOLD")
                                {
                                    dt.Rows[j][t] = dtHold.Rows[k]["HOLD"].ToString();
                                }
                            }
                        }
                    }
                }
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtOrt.Rows.Count; k++)
                    {
                        if (dt.Rows[j]["SKUNO"].ToString() == dtOrt.Rows[k]["SKUNO"].ToString())
                        {
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                if (dt.Columns[t].ColumnName.ToString() == "ORT")
                                {
                                    dt.Rows[j][t] = dtOrt.Rows[k]["ORT"].ToString() == "" ? "0" : dtOrt.Rows[k]["ORT"].ToString();
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "CRITICALBONEPILE")
                                {
                                    dt.Rows[j][t] = dtOrt.Rows[k]["CRITICALBONEPILE"].ToString();
                                }
                            }
                        }
                    }
                }
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtFail.Rows.Count; k++)
                    {
                        if (dt.Rows[j]["SKUNO"].ToString() == dtFail.Rows[k]["SKUNO"].ToString())
                        {
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                if (dt.Columns[t].ColumnName.ToString() == "FAIL_TOTAL")
                                {
                                    dt.Rows[j][t] = dtFail.Rows[k]["FAIL_TOTAL"].ToString() ?? "0";
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "IN_LINE")
                                {
                                    dt.Rows[j][t] = dtFail.Rows[k]["IN_LINE"].ToString() ?? "0";
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "CHECK_IN")
                                {
                                    dt.Rows[j][t] = dtFail.Rows[k]["CHECK_IN"].ToString() ?? "0";
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "CHECK_OUT")
                                {
                                    dt.Rows[j][t] = dtFail.Rows[k]["CHECK_OUT"].ToString() ?? "0";
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "RE_WIP")
                                {
                                    dt.Rows[j][t] = dtFail.Rows[k]["RE_WIP"].ToString() ?? "0";
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

            Outputs.Add(dt);
        }
    }
}
