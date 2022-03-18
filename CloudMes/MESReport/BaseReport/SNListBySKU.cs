using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNListBySKU : ReportBase
    {

        ReportInput inputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputNextName = new ReportInput() { Name = "NextName", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputRoute = new ReportInput() { Name = "RouteID", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput FromDay = new ReportInput() { Name = "FromDay", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ToDay = new ReportInput() { Name = "ToDay", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SNListBySKU()
        {
            Inputs.Add(inputSku);
            Inputs.Add(inputNextName);
            Inputs.Add(inputRoute);
            Inputs.Add(FromDay);
            Inputs.Add(ToDay);
        }
        public override void Run()
        {
            //base.Run();
            string sku = inputSku.Value.ToString();
            string fromDay = FromDay.Value.ToString();
            string toDay = ToDay.Value.ToString();
            string nextName = inputNextName.Value.ToString().ToUpper();
            string routeName = inputRoute.Value.ToString();
            string sqlRun = string.Empty;
            string Temp_fromDay = string.Empty;
            string Temp_toDay = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataTable dc = new DataTable();
            DataRow linkRow = null;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string sqlcustomer = $@"select*From c_customer";
            dc = SFCDB.RunSelect(sqlcustomer).Tables[0];
            if (fromDay.Contains("%20")) fromDay = fromDay.Replace("%20", " ");

            if (toDay.Contains("%20")) toDay = toDay.Replace("%20", " ");

            if (fromDay.Length < 11)
            {
                Temp_fromDay = $@"to_date('{fromDay}', 'yyyy-mm-dd')";
            }
            else
            {
                Temp_fromDay = $@"to_date('{fromDay}', 'yyyy-mm-dd hh24:mi:ss')";
            }
            if (toDay.Length < 11)
            {
                Temp_toDay = $@"to_date('{toDay}', 'yyyy-mm-dd')";
            }
            else
            {
                Temp_toDay = $@"to_date('{toDay}', 'yyyy-mm-dd hh24:mi:ss')";
            }
         
            try
            {
                if (nextName.Equals("ORT"))
                {
                    sqlRun = $@"select c.sn,c.current_station,c.next_station,c.edit_time
                                 from r_lot_detail a, r_lot_status b, r_sn c
                                where 1=1 and (c.skuno='{sku}' or c.workorderno='{sku}')
                                  and c.sn=a.sn
                                  and c.valid_flag='1'
                                  and substr(a.sn, 0, 1) not in ('*','#','~')
                                  and a.lot_id = b.id
                                  and b.sample_station='ORT' order by c.edit_time ";
                }

                else if (nextName.Equals("SKU_QTY"))
                {
                    sqlRun = $@"select sn, current_station, next_station, edit_time from r_sn where 1 = 1 and(skuno = '{sku}' or workorderno = '{sku}') and valid_flag = '1' and substr(skuno,0,1) not in ('*', '#', '~')";   
                }
                else if (nextName.Equals("HOLD_WIP"))
                {
                    sqlRun = $@"select sn,current_station,next_station,edit_time from r_sn where 1=1 and sn in (
                                select sn from (select row_number() over(partition by sn order by lock_time desc ) rn ,sn,lock_time from r_sn_lock where lock_status=1) a where a.rn =1) 
                                   and (skuno='{sku}' or workorderno ='{sku}') and valid_flag='1' order by edit_time ";
                }
                else if (nextName.Equals("FAIL_TOTAL"))
                {
                    sqlRun = $@"select sn,current_station,next_station,edit_time from r_sn where sn in (select a.sn
                                  from r_repair_main a
                                 where 1=1
                                  -- and a.fail_station <> 'COSMETIC-FAILURE'
                                   and substr(a.sn, 0, 1) not in ('*','#','~') and substr(a.sn, 0, 2) <> 'RW' and (a.skun ='{sku}' or a.workorderno='{sku}')) and valid_flag='1'
                                 order by sn ";
                }
                else if (nextName.Equals("CHECK_IN"))
                {
                    sqlRun = $@"select sn,current_station,next_station,edit_time from r_sn where sn in ( select a.sn from r_repair_main a, r_repair_transfer b
                                 where 1=1 and a.id = b.repair_main_id
                                   --and a.fail_station <> 'COSMETIC-FAILURE'
                                   and substr(a.sn, 0, 2) <> 'RW' and substr(a.sn, 0, 1) not in ('*','#','~')
                                   and (a.skuno='{sku}' or a.workorderno='{sku}') ) and valid_flag='1' order by sn ";
                }
                else if (nextName.Equals("CHECK_OUT"))
                {
                    sqlRun = $@"select sn,current_station,next_station,edit_time from r_sn where 1 =1  and sn in (select a.sn
                                  from r_repair_main a, r_repair_transfer b
                                 where 1=1 and  a.id = b.repair_main_id
                                   and b.closed_flag = '1'
                                  -- and a.fail_station <> 'COSMETIC-FAILURE'
                                   and substr(a.sn, 0, 2) <> 'RW' and  substr(a.sn, 0, 1) not in ('*','#','~')
                                   and (a.skuno='{sku}' or a.workorderno='{sku}')) and valid_flag='1' order by sn ";
                }
                else if (nextName.Equals("IN_LINE"))
                {
                    sqlRun = $@"select sn,current_station,next_station,edit_time from r_sn where 1=1 and sn in (select a.sn
                                  from r_repair_main a
                                 where 1=1
                                   and a.id not in (select repair_main_id from r_repair_transfer)
                                   --and a.fail_station <> 'COSMETIC-FAILURE'
                                   and substr(a.sn, 0, 2) <> 'RW' and substr(a.sn, 0, 1) not in ('*','#','~') and (a.skuno='{sku}' or a.workorderno='{sku}')) and valid_flag='1' 
                                 order by sn";
                }
                else if (nextName.Equals("RE_WIP"))
                {
                    sqlRun = $@"select sn,current_station,next_station,edit_time from r_sn where 1=1 and sn in  (select a.sn
                                  from r_repair_main a
                                 where 1=1
                                   and a.id in (select repair_main_id
                                  from r_repair_transfer where closed_flag='0')
                                   --and a.fail_station <> 'COSMETIC-FAILURE'
                                   and substr(a.sn, 0, 2) <> 'RW' and substr(a.sn, 0, 1) not in ('*','#','~') and (a.skuno='{sku}' or a.workorderno='{sku}'))
                                   and valid_flag='1' order by sn ";
                } else if (nextName.Equals("B73F"))
                {
                    sqlRun = $@"select a.sn,a.current_station,a.next_station,a.edit_time from r_sn a where a.current_station='STOCKIN' and a.valid_flag='1'
                            and not exists (select 1 from r_sn_station_detail b where a.sn=b.sn and b.station_name='CBS')
                            /*and not exists (select 1 from r_sn_kp c where a.sn=c.value)*/
                            and not exists (select 1 from r_sn_kp c where a.sn = c.value and c.sn<>c.value) /*Juniper 存在Siloading時自己綁自己的情況,這種不算*/
                            and a.skuno='{sku}' order by a.sn ";
                }
                else if (nextName.Equals("B74F"))
                {
                    sqlRun = $@"select a.sn,a.next_station as station,a.edit_time from r_sn a where a.valid_flag ='1' and a.current_station in ('CBS','CBS2') and a.next_station='SHIPOUT' and a.shipped_flag='0'
                                and (a.skuno='{sku}' or a.workorderno='{sku}') order by a.sn ";
                }
                else if (nextName.Equals("WORKORDER_QTY"))
                {
                    sqlRun = $@"select a.sn,a.current_station,case when a.repair_failed_flag='0' then a.next_station when a.repair_failed_flag='1' then 'REPAIR' end as next_station,a.edit_time,b.panel  from r_sn a left join r_panel_sn b on a.sn=b.sn where 1=1 
                                and (a.skuno='{sku}' or a.workorderno='{sku}') /*and a.valid_flag='1'*/ and substr(a.sn, 0, 1) not in ('*','#','~') order by a.sn";
                }
                else if (nextName.Equals("SAMPLEQTY"))
                {
                    sqlRun = $@"select sn,current_station,next_station,edit_time from r_sn where sn in(
                                select sn from r_lot_detail where lot_id in (select id from r_lot_status where sample_station <> 'ORT') 
                                and substr(sn, 0, 1) not in ('*','#','~')) and valid_flag='1' and skuno='{sku}' order by sn ";
                }
                else
                {
                    if (nextName.Equals("REWORK"))
                    {
                        sqlRun = $@"select a.sn,a.current_station,a.next_station,a.edit_time,b.panel from r_sn a left join r_panel_sn b on a.sn=b.sn where 1=1 and (a.skuno ='{sku}' or a.workorderno='{sku}') and a.repair_failed_flag='0' 
                                    and a.next_station='{nextName}' and valid_flag='0' Temp_sql order by a.sn ";
                        if ((fromDay == null && toDay == null) || (fromDay == "" && toDay == ""))
                        {
                            sqlRun = sqlRun.Replace("Temp_sql", "");
                        }
                        else
                        {
                            sqlRun = sqlRun.Replace("Temp_sql", $@" and a.edit_time between {Temp_fromDay} and {Temp_toDay} ");
                        }
                    }
                    else
                    {
                        sqlRun = $@"select a.sn,a.current_station,a.next_station,a.edit_time,b.panel from r_sn a left join r_panel_sn b on a.sn=b.sn where 1=1 and (a.skuno='{sku}' or a.workorderno='{sku}') and a.repair_failed_flag ='0'
                                    and a.next_station='{nextName}' and a.route_id = '{routeName}' and a.valid_flag='1' Temp_sql order by a.sn";
                        if ((fromDay == null && toDay == null) || (fromDay == "" && toDay == ""))
                        {
                            sqlRun = sqlRun.Replace("Temp_sql", "");
                        }
                        else
                        {
                            sqlRun = sqlRun.Replace("Temp_sql", $@" and a.edit_time between {Temp_fromDay} and {Temp_toDay} ");
                        }
                    }
                }

                RunSqls.Add(sqlRun);
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                DBPools["SFCDB"].Return(SFCDB);
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("CURRENT_STATION");
                linkTable.Columns.Add("NEXT_STATION");
                linkTable.Columns.Add("EDIT_TIME");
                linkTable.Columns.Add("PANEL");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + snListTable.Rows[i]["SN"].ToString();
                    linkRow["CURRENT_STATION"] = "";
                    linkRow["NEXT_STATION"] = "";
                    linkRow["EDIT_TIME"] = "";
                    linkRow["PANEL"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(snListTable, linkTable);
                reportTable.Tittle = "SNList";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
