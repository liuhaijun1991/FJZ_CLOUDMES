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
    public class WoWipStatusReportDCN : ReportBase
    {
        DataTable dtStation, dtWO, dtHold, dtOrt, dtFail;
        string[] defined = { "FGI", "STORAGE" };
        ReportInput inputWO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput skuNo = new ReportInput { Name = "SKU", InputType = "TextArea", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public WoWipStatusReportDCN()
        {
            Inputs.Add(inputWO);
            Inputs.Add(skuNo);
        }
        public override void Run()
        {
            string wo = inputWO.Value.ToString().Trim();
            string sku = skuNo.Value.ToString().Trim();
            sku = $@"'{sku.Replace("\n", "',\n'").Replace(",''", "").Trim()}'";
            Dictionary<string, DataTable> openWith = new Dictionary<string, DataTable>();
            Dictionary<string, List<string>> objValue = new Dictionary<string, List<string>>();
            DataTable dt = new DataTable();
            DataRow dr;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {

                string sqlStation = $@"select * from  c_control where control_name = 'SFC_WIP_ROUTE' and control_type = 'ROUTE' AND CONTROL_DESC='WOWIPSTATUSREPORT' order by control_level";
                string sqlStationName = @"SELECT DISTINCT *
                                            FROM (SELECT CONTROL_VALUE, MAX(SEQ_NO) AS SEQ_NO
                                                    FROM (SELECT 'JOBFINISH' AS CONTROL_VALUE, 200 AS SEQ_NO
                                                            FROM DUAL
                                                        UNION
                                                        SELECT 'SHIPFINISH' AS CONTROL_VALUE, 201 AS SEQ_NO
                                                            FROM DUAL
                                                        UNION
                                                        SELECT T2.STATION_NAME AS CONTROL_VALUE, T2.SEQ_NO AS SEQ_NO
                                                            FROM SFCRUNTIME.R_WO_BASE T1
                                                            INNER JOIN SFCBASE.C_ROUTE_DETAIL T2
                                                            ON T1.ROUTE_ID = T2.ROUTE_ID
                                                            WHERE 1 = 1 sql_station)
                                                    GROUP BY CONTROL_VALUE)
                                            ORDER BY SEQ_NO ASC";

                #region 新SQL 莫言要求工單release沒生產也要顯示工單出來 20201023
                string sqlWO = $@"select *
                                   from (select ccs.CUSTOMER_NAME PRODUCTNAME,
                                                rwo.WORKORDERNO,
                                                rwo.SKUNO,
                                                csc.SERIES_NAME CUSTPARTDESC,
                                                rwo.WORKORDER_QTY,
                                                rwo.DOWNLOAD_DATE StartDate,
                                                count(rsn.SN) station_wip,
                                                rsn.route_id,
                                                rsn.NEXT_STATION,
                                                rwo.SKU_VER
                                           from r_sn       rsn,
                                                r_wo_base  rwo,
                                                c_sku      csk,
                                                c_series   csc,
                                                c_customer ccs
                                          where rsn.workorderno = rwo.workorderno
                                            and rsn.REPAIR_FAILED_FLAG = 0
                                            and rwo.closed_flag = '0'
                                            and rwo.skuno = csk.skuno
                                            and csk.c_series_id = csc.id
                                            and csc.customer_id = ccs.id
                                            Temp_Sql
                                          group by ccs.CUSTOMER_NAME,
                                                   rwo.WORKORDERNO,
                                                   rwo.SKUNO,
                                                   csc.SERIES_NAME,
                                                   rwo.WORKORDER_QTY,
                                                   rwo.DOWNLOAD_DATE,
                                                   rsn.route_id,
                                                   rsn.NEXT_STATION,
                                                   rwo.SKU_VER
                                         union all
                                         select ccs.CUSTOMER_NAME PRODUCTNAME,
                                                rwo.WORKORDERNO,
                                                rwo.SKUNO,
                                                csc.SERIES_NAME CUSTPARTDESC,
                                                rwo.WORKORDER_QTY,
                                                rwo.DOWNLOAD_DATE StartDate,
                                                count(rsn.SN) station_wip,
                                                rsn.route_id,
                                                'REPAIR' NEXT_STATION,
                                                rwo.SKU_VER
                                           from r_sn       rsn,
                                                r_wo_base  rwo,
                                                c_sku      csk,
                                                c_series   csc,
                                                c_customer ccs
                                          where rsn.workorderno = rwo.workorderno
                                            and rsn.REPAIR_FAILED_FLAG = 1
                                            and rwo.closed_flag = '0'
                                            and rwo.skuno = csk.skuno
                                            and csk.c_series_id = csc.id
                                            and csc.customer_id = ccs.id
                                            Temp_Sql
                                          group by ccs.CUSTOMER_NAME,
                                                   rwo.WORKORDERNO,
                                                   rwo.SKUNO,
                                                   csc.SERIES_NAME,
                                                   rwo.WORKORDER_QTY,
                                                   rwo.DOWNLOAD_DATE,
                                                   rsn.route_id,
                                                   rsn.NEXT_STATION,
                                                   rwo.SKU_VER
                                         union all
                                         select ccs.CUSTOMER_NAME PRODUCTNAME,
                                                rwo.WORKORDERNO,
                                                rwo.SKUNO,
                                                csc.SERIES_NAME CUSTPARTDESC,
                                                rwo.WORKORDER_QTY,
                                                rwo.DOWNLOAD_DATE StartDate,
                                                count(null) station_wip,
                                                '999' route_id,
                                                '' NEXT_STATION,
                                                rwo.SKU_VER
                                           from r_wo_header rwh,
                                                r_wo_base   rwo,
                                                c_sku       csk,
                                                c_series    csc,
                                                c_customer  ccs
                                          WHERE rwh.AUFNR = rwo.workorderno
                                            and rwo.closed_flag = '0'
                                            AND rwo.WORKORDER_QTY <> '0'
                                            and rwo.skuno = csk.skuno
                                            and csk.c_series_id = csc.id
                                            and csc.customer_id = ccs.id
                                            AND NOT EXISTS
                                          (SELECT * FROM r_sn rs WHERE rs.workorderno = rwh.aufnr)
                                          Temp_Sql
                                          group by ccs.CUSTOMER_NAME,
                                                   rwo.WORKORDERNO,
                                                   rwo.SKUNO,
                                                   csc.SERIES_NAME,
                                                   rwo.WORKORDER_QTY,
                                                   rwo.DOWNLOAD_DATE,
                                                   rwo.SKU_VER)
                                  order by skuno";
                #endregion


                string sqlHold = $@"select count(1) Hold_WIP ,workorderno from r_sn where sn in (
                                    select sn from (select row_number() over(partition by  sn ORDER BY LOCK_TIME desc ) rn ,sn,lock_time from r_sn_lock where LOCK_STATUS=1 ) a  where  a.rn =1) and valid_flag = 1
                                    Temp_Sql
                                    group by WORKORDERNO ORDER BY WORKORDERNO ";
                string sqlOrt = $@" select count(1) ORT ,'0' TEMPWIP, '0' CriticalBonepile,workorderno from r_sn where sn in (
                                    select sn from (select row_number() over(partition by a.sn order by a.EDIT_TIME desc) rn,a.sn
                                    from r_lot_detail a, r_lot_status b where 1=1 and a.lot_id = b.id
                                        and b.sample_station = 'ORT' and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 1) <> '*'and  substr(a.sn, 0, 1) <> '~')) and VALID_FLAG =1 
                                    Temp_Sql
                                    group by WORKORDERNO ORDER BY WORKORDERNO";

                #region sqlFail：查詢工單SN維修數量，包含產線掃Fail,CheckIn,Checkout,Inline,Rewip

                #region 舊SQL
                //string sqlFail = $@"select t1.workorderno,
                //                         t1.fail_Total,
                //                         nvl(t2.checkin, 0) check_in,
                //                         nvl(t3.checkout, 0) check_out,
                //                         nvl(t5.rewip, 0) re_wip,
                //                         nvl(t4.inline, 0) in_line
                //                    from (select a.workorderno, nvl(count(distinct a.sn), 0) fail_Total
                //                            from r_repair_main a
                //                           where 1=1
                //                             and a.fail_station <> 'COSMETIC-FAILURE'
                //                             and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2) <> 'RW'
                //                           group by a.workorderno) t1
                //                    left join (select b.workorderno, nvl(count(distinct b.sn), 0) checkin
                //                                 from r_repair_main a, r_repair_transfer b
                //                                where 1=1 and a.id = b.repair_main_id
                //                                  and a.fail_station <> 'COSMETIC-FAILURE'
                //                                  and substr(a.sn, 0, 2)<> 'RW' and substr(a.sn, 0, 1) <> '#'
                //                                group by b.workorderno) t2
                //                      on t1.workorderno = t2.workorderno
                //                    left join (select b.workorderno, nvl(count(distinct b.sn), 0) checkout
                //                                 from r_repair_main a, r_repair_transfer b
                //                                where 1=1 and  a.id = b.repair_main_id
                //                                  and b.closed_flag = 1
                //                                  and a.fail_station <> 'COSMETIC-FAILURE'
                //                                  and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2)<> 'RW'
                //                                group by b.workorderno) t3
                //                      on t1.workorderno = t3.workorderno
                //                    left join (select a.workorderno, nvl(count(distinct a.sn), 0) inline
                //                                 from r_repair_main a
                //                                where 1=1
                //                                  and a.id not in
                //                                      (select repair_main_id from r_repair_transfer)
                //                                  and a.fail_station <> 'COSMETIC-FAILURE'
                //                                  and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2)<> 'RW'
                //                                group by a.workorderno) t4
                //                      on t1.workorderno = t4.workorderno
                //                    left join (select a.workorderno, nvl(count(distinct a.sn), 0) rewip
                //                                 from r_repair_main a
                //                                where 1=1
                //                                  and a.id in (select repair_main_id
                //                                                 from r_repair_transfer
                //                                                where closed_flag = 0)
                //                                  and a.fail_station <> 'COSMETIC-FAILURE'
                //                                  and substr(a.sn, 0, 1) <> '#' and substr(a.sn, 0, 2)<> 'RW'
                //                                group by a.workorderno) t5
                //                      on t1.workorderno = t5.workorderno
                //                   where substr(t1.workorderno,0,1) <> '*' and substr(t1.workorderno,0,1) <> '~'
                //                     and t1.fail_Total > 0
                //                     Temp_Sql
                //                   order by t1.workorderno";
                #endregion
                #region 新SQL
                string sqlFail = $@"select t1.workorderno,
                       t1.fail_Total,
                       nvl(t2.checkin, 0) check_in,
                       nvl(t3.checkout, 0) check_out,
                       nvl(t4.inline, 0) in_line,
                       nvl(t5.rewip, 0) re_wip
                  from (select a.workorderno,a.skuno, nvl(count(distinct a.sn), 0) fail_Total /*工單在生產過程中所有掃Fail的次數,不會去重*/
                          from r_repair_main a
                         where 1 = 1
                           and a.fail_station <> 'COSMETIC-FAILURE'
                           and substr(a.sn, 0, 1) not in ('#', '*', '~')
                           and substr(a.sn, 0, 2) not in ('RW')
                         group by a.workorderno,a.skuno) t1
                  left join (select a.workorderno,a.skuno, nvl(count(distinct a.sn), 0) checkin /*工單在生產過程中所有掃Fail並掃CheckIn的次數(不管有沒沒有掃CheckOut),不會去重*/
                               from r_repair_main a
                              where 1 = 1
                                and a.fail_station <> 'COSMETIC-FAILURE'
                                and substr(a.sn, 0, 1) not in ('#', '*', '~')
                                and substr(a.sn, 0, 2) not in ('RW')
                                and exists (select 1 from r_repair_transfer b where a.id = b.repair_main_id)
                              group by a.workorderno,a.skuno) t2
                    on t1.workorderno = t2.workorderno
                  left join (select a.workorderno,a.skuno, nvl(count(distinct a.sn), 0) checkout /*工單在生產過程中所有掃Fail並掃CheckIn且掃CheckOut的次數,不會去重*/
                               from r_repair_main a
                              where 1 = 1
                                and a.fail_station <> 'COSMETIC-FAILURE'
                                and substr(a.sn, 0, 1) not in ('#', '*', '~')
                                and substr(a.sn, 0, 2) not in ('RW')
                                and exists (select 1 from r_repair_transfer b where a.id = b.repair_main_id and b.closed_flag = '1')                
                              group by a.workorderno,a.skuno) t3
                    on t1.workorderno = t3.workorderno
                  left join (select a.workorderno,a.skuno, nvl(count(distinct a.sn), 0) inline /*工單SN當前還在維修且未掃CheckIn的個數*/
                               from r_sn a left join r_repair_main b on a.sn = b.sn
                                --and b.fail_station <> 'COSMETIC-FAILURE'        /*PM統計總數有差異先把這個條件拿掉 WUQING 20201130 */
                                and substr(b.sn, 0, 1) not in ('#', '*', '~')
                                and substr(b.sn, 0, 2) not in ('RW')      
                              where 1 = 1 
                                and a.repair_failed_flag = '1'
                                and not exists (select 1 from r_repair_transfer c where b.id = c.repair_main_id)
                              group by a.workorderno,a.skuno) t4
                    on t1.workorderno = t4.workorderno
                  left join (select a.workorderno,a.skuno, nvl(count(distinct a.sn), 0) rewip /*工單SN當前還在維修且已掃CheckIn但沒掃CheckOut的個數*/
                               from r_sn a left join r_repair_main b on a.sn = b.sn
                               -- and b.fail_station <> 'COSMETIC-FAILURE'  /*PM統計總數有差異先把這個條件拿掉 WUQING 20201130 */
                                and substr(b.sn, 0, 1) not in ('#', '*', '~')
                                and substr(b.sn, 0, 2) not in ('RW')                
                              where 1 = 1
                                and a.repair_failed_flag = '1'
                                and b.id in (select repair_main_id from r_repair_transfer where closed_flag = '0')             
                              group by a.workorderno,a.skuno) t5
                    on t1.workorderno = t5.workorderno
                 where substr(t1.workorderno, 0, 1) not in ('#', '*', '~')
                   and t1.fail_Total > 0
                   Temp_Sql
                 order by t1.workorderno";
                #endregion

                #endregion

                if (!"ALL".Equals(wo) && wo.Length > 0 && "ALL".Equals(sku.Replace("'","")))
                {
                    sqlStationName = sqlStationName.Replace("sql_station", $@" AND T1.WORKORDERNO = '{wo}'");
                    sqlWO = sqlWO.Replace("Temp_Sql", $@" and rwo.WORKORDERNO ='{wo}'");
                    sqlHold = sqlHold.Replace("Temp_Sql", $@" and WORKORDERNO ='{wo}'");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", $@" and WORKORDERNO ='{wo}'");
                    sqlFail = sqlFail.Replace("Temp_Sql", $@" and t1.WORKORDERNO ='{wo}'");
                    dtStation = SFCDB.RunSelect(sqlStationName).Tables[0];
                }
                else if (!"ALL".Equals(wo) && wo.Length > 0 && !"ALL".Equals(sku.Replace("'", "")))
                {
                    sqlStationName = sqlStationName.Replace("sql_station", $@" AND T1.WORKORDERNO = '{wo}' AND T1.SKUNO IN ({sku})");
                    sqlWO = sqlWO.Replace("Temp_Sql", $@" and rwo.WORKORDERNO ='{wo}' and rwo.SKUNO IN ({sku})");
                    sqlHold = sqlHold.Replace("Temp_Sql", $@" and WORKORDERNO ='{wo}' and SKUNO IN ({sku})");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", $@" and WORKORDERNO ='{wo}' and SKUNO IN ({sku})");
                    sqlFail = sqlFail.Replace("Temp_Sql", $@" and t1.WORKORDERNO ='{wo}' and t1.SKUNO IN ({sku})");
                    dtStation = SFCDB.RunSelect(sqlStationName).Tables[0];
                }
                else if ("ALL".Equals(wo) && !"ALL".Equals(sku.Replace("'", "")))
                {
                    sqlStationName = sqlStationName.Replace("sql_station", $@" AND T1.SKUNO IN ({sku})");
                    sqlWO = sqlWO.Replace("Temp_Sql", $@" and rwo.SKUNO IN ({sku})");
                    sqlHold = sqlHold.Replace("Temp_Sql", $@" and SKUNO IN ({sku})");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", $@" and SKUNO IN ({sku})");
                    sqlFail = sqlFail.Replace("Temp_Sql", $@" and t1.SKUNO IN ({sku})");
                    dtStation = SFCDB.RunSelect(sqlStationName).Tables[0];
                }
                else
                {
                    sqlStationName = sqlStationName.Replace("sql_station", $@"");
                    sqlWO = sqlWO.Replace("Temp_Sql", "");
                    sqlHold = sqlHold.Replace("Temp_Sql", "");
                    sqlOrt = sqlOrt.Replace("Temp_Sql", "");
                    sqlFail = sqlFail.Replace("Temp_Sql", "");
                    dtStation = SFCDB.RunSelect(sqlStationName).Tables[0];
                }
                dtWO = SFCDB.RunSelect(sqlWO).Tables[0];
                dtHold = SFCDB.RunSelect(sqlHold).Tables[0];
                dtOrt = SFCDB.RunSelect(sqlOrt).Tables[0];
                dtFail = SFCDB.RunSelect(sqlFail).Tables[0];
                if (dtWO.Rows.Count == 0 || dtStation.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                else
                {
                    var data = dtWO.AsEnumerable();
                    var woRow = data.Select(d => d.Field<string>("WORKORDERNO")).Distinct();
                    var sRoute = data.Select(d => d.Field<string>("ROUTE_ID")).Distinct();

                    //根據工單WIP表中路由ID(去重)循環取得路由NAME並添加到字典中，格式：<string, List<string>>
                    foreach (var routeName in sRoute)
                    {
                        List<string> c_ROUTE_s = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == routeName).Select(r => r.STATION_NAME).ToList();
                        if (objValue.ContainsKey(routeName))
                        {
                            continue;
                        }
                        else
                        {
                            if (c_ROUTE_s.Count != 0)
                            {
                                objValue.Add(routeName, c_ROUTE_s);
                            }
                        }
                    }
                    //循環工單WIP表中列數將列名添加到DataTable
                    for (var i = 0; i < dtWO.Columns.Count; i++)
                    {
                        dt.Columns.Add(dtWO.Columns[i].ColumnName.ToString(), typeof(string));
                    }
                    //循環工站將工站名添加到DataTable
                    for (var i = 0; i < dtStation.Rows.Count; i++)
                    {
                        dt.Columns.Add(dtStation.Rows[i]["CONTROL_VALUE"].ToString(), typeof(string));
                    }
                    //循環HOLD表中列數將列名添加到DataTable
                    for (var i = 0; i < dtHold.Columns.Count; i++)
                    {
                        if (dtHold.Columns[i].ColumnName.ToString() != "WORKORDERNO")
                        {
                            dt.Columns.Add(dtHold.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    //循環ORT表中列數將列名添加到DataTable
                    for (var i = 0; i < dtOrt.Columns.Count; i++)
                    {
                        if (dtOrt.Columns[i].ColumnName.ToString() != "WORKORDERNO")
                        {
                            dt.Columns.Add(dtOrt.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    //循環FAIL表中列數將列名添加到DataTable
                    for (var i = 0; i < dtFail.Columns.Count; i++)
                    {
                        if (dtFail.Columns[i].ColumnName.ToString() != "WORKORDERNO")
                        {
                            dt.Columns.Add(dtFail.Columns[i].ColumnName.ToString(), typeof(string));
                        }
                    }
                    //循環自定義數組將列名添加到DataTable
                    for (var i = 0; i < defined.Length; i++)
                    {
                        dt.Columns.Add(defined[i].ToString(), typeof(string));
                    }

                    if (dt.Columns.Contains("STATION_WIP")) dt.Columns.Remove("STATION_WIP");
                    if (dt.Columns.Contains("NEXT_STATION")) dt.Columns.Remove("NEXT_STATION");

                    //根據工單WIP表中工單號(去重)循環將工單號寫入DataTable
                    foreach (var s in woRow)
                    {
                        dr = dt.NewRow();
                        dr["WORKORDERNO"] = s;
                        dr["SKUNO"] = "";
                        dt.Rows.Add(dr);
                    }
                }

                //循環工單WIP表行數按機種將當前表當前機種內容添加到字典中，格式：<string, DataTable>
                for (var j = 0; j < dtWO.Rows.Count; j++)
                {
                    if (openWith.ContainsKey(dtWO.Rows[j]["SKUNO"].ToString()))
                    {
                        continue;
                    }
                    else
                    {
                        openWith.Add(dtWO.Rows[j]["SKUNO"].ToString(), dtWO.Select("SKUNO='" + dtWO.Rows[j]["SKUNO"].ToString() + "'").CopyToDataTable());
                    }
                }

                //循環存放機種和該機種對應工單WIP信息的字典
                foreach (var s in openWith)
                {
                    //再循環DataTable行數
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        //再循環字典中存放機種內容的表的行數，更新DataTable中每一行的基礎欄位
                        for (var j = 0; j < s.Value.Rows.Count; j++)
                        {
                            if (s.Value.Rows[j]["WORKORDERNO"].ToString() == dt.Rows[i]["WORKORDERNO"].ToString())
                            {
                                dt.Rows[i]["PRODUCTNAME"] = s.Value.Rows[j]["PRODUCTNAME"].ToString();
                                dt.Rows[i]["WORKORDERNO"] = s.Value.Rows[j]["WORKORDERNO"].ToString();
                                dt.Rows[i]["SKUNO"] = s.Value.Rows[j]["SKUNO"].ToString();
                                dt.Rows[i]["CUSTPARTDESC"] = s.Value.Rows[j]["CUSTPARTDESC"].ToString();
                                dt.Rows[i]["WORKORDER_QTY"] = s.Value.Rows[j]["WORKORDER_QTY"].ToString();
                                dt.Rows[i]["STARTDATE"] = s.Value.Rows[j]["STARTDATE"].ToString();
                                dt.Rows[i]["ROUTE_ID"] = s.Value.Rows[j]["ROUTE_ID"].ToString();
                                dt.Rows[i]["SKU_VER"] = s.Value.Rows[j]["SKU_VER"].ToString();
                                dt.Rows[i]["FGI"] = "0";
                                dt.Rows[i]["STORAGE"] = "0";
                                //dt.Rows[i]["I_SHIPMENT"] = "0";
                                //dt.Rows[i]["SHIPMENT"] = "0";
                                break;
                            }
                        }
                    }
                }

                //循環更新DataTable中HOLD數量
                for (var j = 0; j < dt.Rows.Count; j++)
                {
                    for (var k = 0; k < dtHold.Rows.Count; k++)
                    {
                        if (dt.Rows[j]["WORKORDERNO"].ToString() == dtHold.Rows[k]["WORKORDERNO"].ToString())
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
                        if (dt.Rows[j]["WORKORDERNO"].ToString() == dtOrt.Rows[k]["WORKORDERNO"].ToString())
                        {
                            for (var t = 0; t < dt.Columns.Count; t++)
                            {
                                if (dt.Columns[t].ColumnName.ToString() == "ORT")
                                {
                                    dt.Rows[j][t] = dtOrt.Rows[k]["ORT"].ToString() == "" ? "0" : dtOrt.Rows[k]["ORT"].ToString();
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "TEMPWIP")
                                {
                                    dt.Rows[j][t] = dtOrt.Rows[k]["TEMPWIP"].ToString();
                                }
                                else if (dt.Columns[t].ColumnName.ToString() == "CRITICALBONEPILE")
                                {
                                    dt.Rows[j][t] = dtOrt.Rows[k]["CRITICALBONEPILE"].ToString();
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
                        if (dt.Rows[j]["WORKORDERNO"].ToString() == dtFail.Rows[k]["WORKORDERNO"].ToString())
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

                //循環更新DataTable中工站WIP數量
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    foreach (var s in openWith)
                    {
                        for (var j = 0; j < s.Value.Rows.Count; j++)
                        {
                            if (s.Value.Rows[j]["WORKORDERNO"].ToString() == dt.Rows[i]["WORKORDERNO"].ToString())
                            {
                                for (var k = 0; k < dt.Columns.Count; k++)
                                {
                                    //其他工站：直接取工單WIP表中工站名相等的WIP數量
                                    if (dt.Columns[k].ColumnName.ToString() == s.Value.Rows[j]["NEXT_STATION"].ToString())
                                    {
                                        dt.Rows[i][k] = s.Value.Rows[j]["STATION_WIP"].ToString();
                                    }
                                    //LOADING(包含SMT&SI)：工單WIP中工站WIP數量之和
                                    if (dt.Columns[k].ColumnName.ToString().EndsWith("LOADING"))
                                    {
                                        var wipSum = s.Value.Select(" WORKORDERNO='" + dt.Rows[i]["WORKORDERNO"].ToString()+"'").AsEnumerable().Sum(t => Convert.ToInt32(t["STATION_WIP"].ToString())); //.Sum("STATION_WIP").Compute("sum(STATION_WIP)", "TRUE");
                                        dt.Rows[i][k] = Convert.ToInt32(s.Value.Rows[j]["WORKORDER_QTY"].ToString()) - wipSum;
                                    }
                                    //STORAGE：工單中下一站為JOBFINISH的數量
                                    if (dt.Columns[k].ColumnName.ToString().Equals("STORAGE"))
                                    {
                                        var jobfinishnum = s.Value.Select(" WORKORDERNO='" + dt.Rows[i]["WORKORDERNO"].ToString() + "' AND NEXT_STATION='JOBFINISH'");
                                        if (jobfinishnum.Length > 0)
                                            dt.Rows[i][k] = jobfinishnum[0]["STATION_WIP"].ToString();
                                    }
                                    //FGI：工單中下一站為JOBFINISH或REWORK的數量之和
                                    if (dt.Columns[k].ColumnName.ToString().Equals("FGI"))
                                    {
                                        //var jobfinishnum = s.Value.Select(" WORKORDERNO= " + dt.Rows[i]["WORKORDERNO"].ToString() +
                                        //                                  " and NEXT_STATION='JOBFINISH'");
                                        //if (jobfinishnum.Length > 0)
                                        //    dt.Rows[i][k] = jobfinishnum[0]["STATION_WIP"].ToString();
                                        //var reworknum = s.Value.Select(" WORKORDERNO= " + dt.Rows[i]["WORKORDERNO"].ToString() +
                                        //                                  " and NEXT_STATION='REWORK'");
                                        //if (reworknum.Length > 0)
                                        //    dt.Rows[i][k] = Convert.ToInt32(dt.Rows[i][k].ToString())+Convert.ToInt32(reworknum[0]["STATION_WIP"].ToString());

                                        //上面語句會循環給dt.Rows[i][k]賦值，直接改成下面寫法 Edit By ZHB 2020年9月7日14:11:59
                                        dt.Rows[i][k] = s.Value.Select(" WORKORDERNO='" + dt.Rows[i]["WORKORDERNO"].ToString() + "' AND NEXT_STATION IN ('JOBFINISH','REWORK')").Sum(t => Convert.ToInt32(t["STATION_WIP"].ToString()));
                                    }
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
