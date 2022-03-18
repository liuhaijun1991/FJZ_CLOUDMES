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
    public class SkuAllWipReport : ReportBase
    {
        DataTable dtStation, dtSku, dtHold, dtOrt,dtSample, dtFail, dtB73F, dtB74F;
        ReportInput inputSKU = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SkuAllWipReport()
        {
            Inputs.Add(inputSKU);
        }
        public override void Run()
        {
            string sku = inputSKU.Value.ToString();
            string sqlSku;
            string sqlB73F;
            string sqlB74F;
            string sqlOrt;
            string sqlHold;
            string sqlSample;
            Dictionary<string, List<string>> objValue = new Dictionary<string, List<string>>();
            DataTable dt = new DataTable();
            DataRow dr;
            IEnumerable<string> skuRow ;
            DataTable dc = new DataTable();
            List<string> list = new List<string>();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sqlStation = $@"select * from (select CONTROL_VALUE,to_number(control_level) as control_level from  c_control where control_name = 'SFC_ALL_SKU_WIP_ROUTE' and control_type = 'ROUTE')  order by control_level";

                #region sqlSku：查詢機種當前WIP數量及所在工站，包含沒掃維修及掃了維修的數據方便後面計算

                string sqlcustomer = $@"select*From c_customer";
                dc = SFCDB.RunSelect(sqlcustomer).Tables[0];
                sqlSku = $@"
                select *
                    from (select ccs.CUSTOMER_NAME PRODUCTNAME,
                                csc.SERIES_NAME CUSTPARTDESC,
                                a.skuno,
                                b.sku_qty,
                                a.route_id,
                                '' B73F,
                                '' B74F,
                                a.station_wip,
                                a.NEXT_STATION
                            from (select rwo.skuno,
                                        count(rsn.SN) station_wip,
                                        rsn.NEXT_STATION,
                                        rsn.route_id
                                    from r_sn rsn, r_wo_base rwo
                                    where rsn.workorderno = rwo.workorderno
                                    and rsn.valid_flag = 1
                                    and rsn.REPAIR_FAILED_FLAG = 0
                                    Temp_Sql
                                    group by rwo.SKUNO,
                                            rsn.route_id,
                                            --rwo.START_STATION,--這條件沒必要
                                            rsn.NEXT_STATION) a,
                                (select count(sn) sku_qty, skuno
                                    from r_sn
                                    where valid_flag = 1
                                    and substr(skuno, 0, 1) <> '#'
                                    and substr(skuno, 0, 1) <> '~'
                                    group by skuno) b,
                                (select * from c_sku) c,
                                c_series csc,
                                c_customer ccs
                            WHERE a.skuno = b.skuno
                            and b.skuno = c.skuno
                            and c.c_series_id = csc.id
                            and csc.customer_id = ccs.id
                        union all --下面的語句會根據掃入維修的SN的NEXT_STATION匯總，但是因為在外層把NEXT_STATION強制=REPAIR，這樣就會出現重複數據，所以這裡需要用 union all，不然會把重複的數據過濾掉
                        select ccs.CUSTOMER_NAME PRODUCTNAME,
                                csc.SERIES_NAME CUSTPARTDESC,
                                a.skuno,
                                b.sku_qty,
                                a.route_id,
                                '' B73F,
                                '' B74F,
                                a.station_wip,
                                'REPAIR' NEXT_STATION
                            from (select rwo.skuno,
                                        count(rsn.SN) station_wip,
                                        rsn.NEXT_STATION,
                                        rsn.route_id
                                    from r_sn rsn, r_wo_base rwo
                                    where rsn.workorderno = rwo.workorderno
                                    and rsn.valid_flag = 1
                                    and rsn.REPAIR_FAILED_FLAG = 1
                                    Temp_Sql
                                    group by rwo.SKUNO,
                                            rsn.route_id,
                                            --rwo.START_STATION,--這條件沒必要
                                            rsn.NEXT_STATION) a,
                                (select count(sn) sku_qty, skuno
                                    from r_sn
                                    where valid_flag = 1
                                    and substr(skuno, 0, 1) <> '#'
                                    and substr(skuno, 0, 1) <> '~'
                                    group by skuno) b,
                                (select * from c_sku) c,
                                c_series csc,
                                c_customer ccs
                            WHERE a.skuno = b.skuno
                            and b.skuno = c.skuno
                            and c.c_series_id = csc.id
                            and csc.customer_id = ccs.id)
                    order by skuno";
                sqlB73F = $@"select a.skuno,count(a.sn) station_wip From r_sn a where a.CURRENT_STATION ='STOCKIN' and a.valid_flag = 1
                                and not exists (select 1 from  r_sn_station_detail b where a.sn = b.sn and b.station_name ='CBS')
                                /*and not exists (select 1 from r_sn_kp c where a.sn = c.value )*/
                                and not exists (select 1 from r_sn_kp c where a.sn = c.value and c.sn<>c.value) /*Juniper 存在Siloading時自己綁自己的情況,這種不算*/
                                Temp_Sql
                                group by a.skuno";
                sqlB74F = $@"select a.skuno,count(a.sn) station_wip from r_sn a where a.valid_flag =1 and a.current_station in ('CBS','CBS2') AND a.next_station = 'SHIPOUT' and a.shipped_flag = 0
                            Temp_Sql
                            group by a.skuno ";
                sqlOrt = $@"select count(1) ORT ,skuno From r_sn where sn in(
                                SELECT sn  fROM R_LOT_DETAIL where lot_id in (select id From r_lot_status where SAMPLE_STATION = 'ORT') and substr(sn, 0, 1) <> '*' and substr(sn, 0, 1) <> '#'
                                union all
								select SN From r_ort r where ortevent='ORTIN' 
								AND NOT EXISTS( SELECT*fROM R_ORT o WHERE r.sn=o.sn  and o.ortevent='ORTOUT'))
                                and VALID_FLAG = 1 
                                Temp_Sql 
                                group by skuno 
                                ORDER BY skuno";
                sqlHold = $@"select count(1) HOLD_WIP ,skuno from r_sn where sn in (
                                    select sn from(select row_number() over(partition by  sn ORDER BY LOCK_TIME desc) rn, sn, lock_time from r_sn_lock where LOCK_STATUS = 1 ))
                                    and valid_flag=1 --只取有效數據
                                    Temp_Sql
                                    group by skuno
                                    order by skuno ";
                sqlSample = $@"select count(sn) SAMPLEQTY ,'0' CriticalBonepile,skuno From r_sn where sn in(
                                    SELECT sn  fROM R_LOT_DETAIL where lot_id in (select id From r_lot_status where SAMPLE_STATION <> 'ORT') 
                                    and substr(sn, 0, 1) <> '*' and substr(sn, 0, 1) <> '#'and substr(sn, 0, 1) <> '~' )
                                    and VALID_FLAG = 1 
                                    Temp_Sql
                                    group by skuno 
                                    ORDER BY skuno 
                                     ";

                #endregion

                #region sqlFail：查詢機種SN維修數量，包含產線掃Fail,CheckIn,Checkout,Inline,Rewip

                string sqlFail = $@"
                select t1.skuno,
                       t1.fail_Total,
                       nvl(t2.checkin, 0) check_in,
                       nvl(t3.checkout, 0) check_out,
                       nvl(t4.inline, 0) in_line,
                       nvl(t5.rewip, 0) re_wip       
                  from (select a.skuno, nvl(count(/*distinct*/ a.sn), 0) fail_Total /*機種在生產過程中所有掃Fail的次數,不會去重*//*同一個SN先後掃入維修，也算次數，所以去掉distinct*/
                          from r_repair_main a
                         where 1 = 1
                           --and a.fail_station <> 'COSMETIC-FAILURE' /*外觀不良掃進維修也算次數*/
                           and substr(a.sn, 0, 1) not in ('#', '*', '~')
                           and substr(a.sn, 0, 2) not in ('RW')
                         group by a.skuno) t1
                  left join (select a.skuno, nvl(count(/*distinct*/ a.sn), 0) checkin /*機種在生產過程中所有掃Fail並掃CheckIn的次數(不管有沒沒有掃CheckOut),不會去重*//*同一個SN先後掃入維修，也算次數，所以去掉distinct*/
                               from r_repair_main a
                              where 1 = 1
                                --and a.fail_station <> 'COSMETIC-FAILURE' /*外觀不良掃進維修也算次數*/
                                and substr(a.sn, 0, 1) not in ('#', '*', '~')
                                and substr(a.sn, 0, 2) not in ('RW')
                                and exists (select 1 from r_repair_transfer b where a.id = b.repair_main_id)
                              group by a.skuno) t2
                    on t1.skuno = t2.skuno
                  left join (select a.skuno, nvl(count(/*distinct*/ a.sn), 0) checkout /*機種在生產過程中所有掃Fail並掃CheckIn且掃CheckOut的次數,不會去重*//*同一個SN先後掃入維修，也算次數，所以去掉distinct*/
                               from r_repair_main a
                              where 1 = 1
                                --and a.fail_station <> 'COSMETIC-FAILURE' /*外觀不良掃進維修也算次數*/
                                and substr(a.sn, 0, 1) not in ('#', '*', '~')
                                and substr(a.sn, 0, 2) not in ('RW')
                                and exists (select 1 from r_repair_transfer b where a.id = b.repair_main_id and b.closed_flag = '1')
                              group by a.skuno) t3
                    on t1.skuno = t3.skuno
                  left join (select a.skuno, nvl(count(/*distinct*/ a.sn), 0) inline /*機種SN當前還在維修且未掃CheckIn的個數*//*同一個SN先後掃入維修，也算次數，所以去掉distinct*/
                               from r_sn a left join r_repair_main b on a.sn = b.sn
                                --and b.fail_station <> 'COSMETIC-FAILURE' /*外觀不良掃進維修也算次數*/
                                and substr(b.sn, 0, 1) not in ('#', '*', '~')
                                and substr(b.sn, 0, 2) not in ('RW')      
                              where 1 = 1 
                                and a.repair_failed_flag = 1 
                                and not exists (select 1 from r_repair_transfer c where b.id = c.repair_main_id)
                              group by a.skuno) t4
                    on t1.skuno = t4.skuno
                  left join (select a.skuno, nvl(count(/*distinct*/ a.sn), 0) rewip /*機種SN當前還在維修且已掃CheckIn但沒掃CheckOut的個數*//*同一個SN先後掃入維修，也算次數，所以去掉distinct*/
                               from r_sn a left join r_repair_main b on a.sn = b.sn
                                --and b.fail_station <> 'COSMETIC-FAILURE' /*外觀不良掃進維修也算次數*/
                                and substr(b.sn, 0, 1) not in ('#', '*', '~')
                                and substr(b.sn, 0, 2) not in ('RW')                
                              where 1 = 1
                                and a.repair_failed_flag = 1
                                and a.valid_flag = 1 /*只取有效數據*/
                                and b.id in (select repair_main_id from r_repair_transfer where closed_flag = '0')
                              group by a.skuno) t5
                    on t1.skuno = t5.skuno
                 where substr(t1.skuno, 0, 1) not in ('#', '*', '~')
                   and t1.fail_Total > 0
                   Temp_Sql
                 order by t1.skuno";

                #endregion
    
                if (sku != "ALL" && sku != "")
                {
                    sqlSku = sqlSku.Replace("Temp_Sql", $@" and rsn.skuno = '{sku}'");
                    sqlHold = sqlHold.Replace("Temp_Sql", $@" and skuno = '{sku}'");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", $@" and skuno = '{sku}'");
                    sqlSample = sqlSample.Replace("Temp_Sql", $@" and skuno = '{sku}'");
                    sqlFail = sqlFail.Replace("Temp_Sql", $@"and t1.skuno = '{sku}'");
                    sqlB73F = sqlB73F.Replace("Temp_Sql", $@" and a.skuno = '{sku}'");
                    sqlB74F = sqlB74F.Replace("Temp_Sql", $@" and a.skuno = '{sku}'");
                }
                else
                {
                    sqlSku = sqlSku.Replace("Temp_Sql", "");
                    sqlHold = sqlHold.Replace("Temp_Sql", "");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", "");
                    sqlSample = sqlSample.Replace("Temp_Sql", "");
                    sqlFail = sqlFail.Replace("Temp_Sql", "");
                    sqlB73F = sqlB73F.Replace("Temp_Sql", " ");
                    sqlB74F = sqlB74F.Replace("Temp_Sql", " ");
                }

                dtStation = SFCDB.RunSelect(sqlStation).Tables[0];
                dtSku = SFCDB.RunSelect(sqlSku).Tables[0];
                dtHold = SFCDB.RunSelect(sqlHold).Tables[0];
                dtOrt = SFCDB.RunSelect(sqlOrt).Tables[0];
                dtSample = SFCDB.RunSelect(sqlSample).Tables[0];
                dtFail = SFCDB.RunSelect(sqlFail).Tables[0];
                dtB73F = SFCDB.RunSelect(sqlB73F).Tables[0];
                dtB74F = SFCDB.RunSelect(sqlB74F).Tables[0];
                if (dtSku.Rows.Count == 0 || dtStation.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                else
                {
                    var data = dtSku.AsEnumerable();
                    skuRow = data.Select(d => d.Field<string>("SKUNO")).Distinct();                        
                    var sRoute = data.Select(d => d.Field<string>("ROUTE_ID")).Distinct();

                    //根據機種WIP表中路由ID(去重)循環取得路由NAME並添加到字典中，格式：<string, List<string>>
                    foreach (var routeName in sRoute)
                    {
                        List<string> c_ROUTE_s = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == routeName).Select(r=>r.STATION_NAME).ToList();
                        if (objValue.ContainsKey(routeName))
                        {
                            continue;
                        }
                        else
                        {   if (c_ROUTE_s.Count!=0) {
                                objValue.Add(routeName, c_ROUTE_s);
                            }                            
                        }
                    }
                    //循環機種WIP表中列數將列名添加到DataTable
                    for (var i = 0; i < dtSku.Columns.Count; i++)
                    {
                        dt.Columns.Add(dtSku.Columns[i].ColumnName.ToString(), typeof(string));
                    }
                    //循環工站將工站名添加到DataTable
                    for (var i = 0; i < dtStation.Rows.Count; i++)
                    {
                        dt.Columns.Add(dtStation.Rows[i]["CONTROL_VALUE"].ToString(), typeof(string));
                    }
                    //循環HOLD表中列數將列名添加到DataTable
                    for (var i = 0; i < dtHold.Columns.Count; i++)
                    {
                        if (dtHold.Columns[i].ColumnName.ToString() != "SKUNO")
                        {
                            dt.Columns.Add(dtHold.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    //循環ORT表中列數將列名添加到DataTable
                    for (var i = 0; i < dtOrt.Columns.Count; i++)
                    {
                        if (dtOrt.Columns[i].ColumnName.ToString() != "SKUNO")
                        {
                            dt.Columns.Add(dtOrt.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    //循環SAMPLE表中列數將列名添加到DataTable
                    for (var i = 0; i < dtSample.Columns.Count; i++)
                    {
                        if (dtSample.Columns[i].ColumnName.ToString() != "SKUNO")
                        {
                            dt.Columns.Add(dtSample.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    //循環FAIL表中列數將列名添加到DataTable
                    for (var i = 0; i < dtFail.Columns.Count; i++)
                    {
                        if (dtFail.Columns[i].ColumnName.ToString() != "SKUNO")
                        {
                            dt.Columns.Add(dtFail.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }

                    if (dt.Columns.Contains("STATION_WIP")) dt.Columns.Remove("STATION_WIP");
                    if (dt.Columns.Contains("NEXT_STATION")) dt.Columns.Remove("NEXT_STATION");
                    //根據機種WIP表中機種號(去重)循環將機種號寫入DataTable
                    foreach (var s in skuRow)
                    {
                        dr = dt.NewRow();
                        dr["SKUNO"] = s;
                        dt.Rows.Add(dr);
                    }
                }
                //循環更新DataTable中基礎欄位信息
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtSku.Rows.Count; k++)
                    {
                        if (dt.Rows[j]["SKUNO"].ToString() == dtSku.Rows[k]["SKUNO"].ToString())
                        {
                            dt.Rows[j]["PRODUCTNAME"] = dtSku.Rows[k]["PRODUCTNAME"].ToString();
                            dt.Rows[j]["CUSTPARTDESC"] = dtSku.Rows[k]["CUSTPARTDESC"].ToString();
                            dt.Rows[j]["SKU_QTY"] = dtSku.Rows[k]["SKU_QTY"].ToString();
                            dt.Rows[j]["ROUTE_ID"] = dtSku.Rows[k]["ROUTE_ID"].ToString();

                            //循環更新DataTable中對應工站WIP數量
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                //其他工站：直接取工單WIP表中工站名相等的WIP數量
                                if (dt.Columns[t].ColumnName.ToString() == dtSku.Rows[k]["NEXT_STATION"].ToString())
                                {
                                    dt.Rows[j][t] = dtSku.Rows[k]["STATION_WIP"].ToString();
                                }
                                //LOADING(包含SMT&SI)：機種WIP中工站WIP數量之和
                                if (dt.Columns[t].ColumnName.ToString().EndsWith("LOADING"))
                                {
                                    var wipSum = dtSku.Select(" SKUNO= '" + dtSku.Rows[k]["SKUNO"].ToString() + "'").AsEnumerable().Sum(it => Convert.ToInt32(it["STATION_WIP"].ToString())); //.Sum("STATION_WIP").Compute("sum(STATION_WIP)", "TRUE");
                                    dt.Rows[j][t] = Convert.ToInt32(dtSku.Rows[k]["SKU_QTY"].ToString()) - wipSum;
                                }
                            }
                        }
                    }
                }

                //循環更新DataTable中B73F數量
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtB73F.Rows.Count; k++)
                    {
                        //沒人說得清楚為什麼要單獨串R_PRE_WO_HEAD，因語句查詢無數據屏蔽掉
                        //if (dc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
                        //{
                        //    if (dt.Rows[j]["GROUPID"].ToString() == dtB73F.Rows[k]["GROUPID"].ToString())
                        //    {
                        //        for (var t = 0; t < dt.Columns.Count; t++)
                        //        {
                        //            if (dt.Columns[t].ColumnName.ToString() == "B73F")
                        //            {
                        //                dt.Rows[j][t] = dtB73F.Rows[k]["STATION_WIP"].ToString();
                        //            }
                        //        }
                        //    }

                        //}
                        //else
                        //{
                            if (dt.Rows[j]["SKUNO"].ToString() == dtB73F.Rows[k]["SKUNO"].ToString())
                            {
                                for (var t = 0; t < dt.Columns.Count; t++)
                                {
                                    if (dt.Columns[t].ColumnName.ToString() == "B73F")
                                    {
                                        dt.Rows[j][t] = dtB73F.Rows[k]["STATION_WIP"].ToString();
                                    }
                                }
                            }
                        //}
                      
                    }
                }

                //循環更新DataTable中B74F數量
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtB74F.Rows.Count; k++)
                    {
                        //沒人說得清楚為什麼要單獨串R_PRE_WO_HEAD，因語句查詢無數據屏蔽掉
                        //if (dc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
                        //{
                        //    if (dt.Rows[j]["GROUPID"].ToString() == dtB74F.Rows[k]["GROUPID"].ToString())
                        //    {
                        //        for (var t = 0; t < dt.Columns.Count; t++)
                        //        {
                        //            if (dt.Columns[t].ColumnName.ToString() == "B74F")
                        //            {
                        //                dt.Rows[j][t] = dtB74F.Rows[k]["STATION_WIP"].ToString();
                        //            }
                        //        }
                        //    }

                        //}
                        //else
                        //{
                            if (dt.Rows[j]["SKUNO"].ToString() == dtB74F.Rows[k]["SKUNO"].ToString())
                            {
                                for (var t = 0; t < dt.Columns.Count; t++)
                                {
                                    if (dt.Columns[t].ColumnName.ToString() == "B74F")
                                    {
                                        dt.Rows[j][t] = dtB74F.Rows[k]["STATION_WIP"].ToString();
                                    }
                                }
                            }
                        //}
                    }
                }

                //循環更新DataTable中HOLD數量
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtHold.Rows.Count; k++)
                    {
                       
                        
                        if (dt.Rows[j]["SKUNO"].ToString() == dtHold.Rows[k]["SKUNO"].ToString())
                        {
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                if (dt.Columns[t].ColumnName.ToString() == "HOLD_WIP")
                                {
                                    dt.Rows[j][t] = dtHold.Rows[k]["HOLD_WIP"].ToString();
                                }
                            }
                        }
                        
                            
                    }
                }

                //循環更新DataTable中ORT數量
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

                            }
                        }

                        
                
                    }
                }

                //循環更新DataTable中SAMPLE數量
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtSample.Rows.Count; k++)
                    {
                        
                       
                        if (dt.Rows[j]["SKUNO"].ToString() == dtSample.Rows[k]["SKUNO"].ToString())
                        {
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                if (dt.Columns[t].ColumnName.ToString() == "SAMPLEQTY")
                                {
                                    dt.Rows[j][t] = dtSample.Rows[k]["SAMPLEQTY"].ToString() == "" ? "0" : dtSample.Rows[k]["SAMPLEQTY"].ToString();
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "CRITICALBONEPILE")
                                {
                                    dt.Rows[j][t] = dtSample.Rows[k]["CRITICALBONEPILE"].ToString();
                                }
                            }
                        }
                        
                            
                    }
                }

                //循環更新DataTable中FAIL數量
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
                                else if (dt.Columns[t].ColumnName.ToString() == "IN_LINE")
                                {
                                    dt.Rows[j][t] = dtFail.Rows[k]["IN_LINE"].ToString() ?? "0";
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
            Outputs.Add(objValue);
        }
    }
}
