using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESReport.DCN
{
    /// <summary>
    /// 維修WIP報表For DCN
    /// </summary>
    public class RepairWipReport : ReportBase
    {
        ReportInput Customer = new ReportInput() { Name = "Customer", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput Series = new ReportInput() { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput StartDay = new ReportInput() { Name = "StartDay", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndDay = new ReportInput() { Name = "EndDay", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairWipReport()
        {
            Inputs.Add(Customer);
            Inputs.Add(Series);
            Inputs.Add(SkuNo);
            Inputs.Add(StartDay);
            Inputs.Add(EndDay);
        }

        public override void Init()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {               
                InitCustomer(SFCDB);
                InitSeries(SFCDB);
                InitSkuNo(SFCDB);
                StartDay.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                EndDay.Value = DateTime.Now.ToString("yyyy-MM-dd");               
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public override void Run()
        {
            string customer = Customer.Value.ToString();
            string series = Series.Value.ToString();
            string skuNo = SkuNo.Value.ToString();
            string startDay = Convert.ToDateTime(StartDay.Value).ToString("yyyy-MM-dd");
            string endDay = Convert.ToDateTime(EndDay.Value).ToString("yyyy-MM-dd");
            string endDayForCount = Convert.ToDateTime(EndDay.Value).AddDays(1).ToString("yyyy-MM-dd");

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                #region 檢查時間區間再取天數List
                List<string> dayList = new List<string>();
                string diffSql = $@"
                    select to_date('{endDay}','yyyy-MM-dd') - to_date('{startDay}','yyyy-MM-dd') diff from dual
                    union all
                    select to_date('{endDay}','yyyy-MM-dd') - to_date(to_char(sysdate, 'yyyy-MM-dd'),'yyyy-MM-dd') diff from dual
                    union all
                    select to_date('{startDay}','yyyy-MM-dd') - to_date('2020-01-01','yyyy-MM-dd') diff from dual";
                DataTable diffDT = SFCDB.RunSelect(diffSql).Tables[0];

                if (Convert.ToInt32(diffDT.Rows[0]["DIFF"]) < 0)
                {
                    throw new Exception("起始時間不能大於截止時間,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816162751"));
                }
                else if (Convert.ToInt32(diffDT.Rows[0]["DIFF"]) > 10)
                {
                    throw new Exception("查詢時間必須在10天以內,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816163017"));
                }
                else if (Convert.ToInt32(diffDT.Rows[1]["DIFF"]) > 0)
                {
                    throw new Exception("查詢時間不能大於當前時間,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816163032"));
                }
                else if (Convert.ToInt32(diffDT.Rows[2]["DIFF"]) < 0)
                {
                    throw new Exception("查詢時間不能小於時間:2020-01-01,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816163046"));
                }
                if (Convert.ToDateTime(startDay) >= DateTime.Now)
                {
                    throw new Exception("起始時間不能大於當前時間,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816163058"));
                }
                if (Convert.ToDateTime(endDay) >= DateTime.Now)
                {
                    throw new Exception("截止時間不能大於當前時間,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816163110"));
                }
                for (int i = 0; i < Convert.ToInt32(diffDT.Rows[0]["DIFF"]) + 1; i++)
                {
                    string day = Convert.ToDateTime(startDay).AddDays(i).ToString("yyyy-MM-dd");
                    dayList.Add(day);
                }
                #endregion

                #region CheckIn&CheckOut的SQL語句
                string inSql = $@"
                    select distinct d.customer_name,c.series_name,a.workorderno,
                                    a.skuno,a.sn,a.line_name,a.station_name,a.in_time,
                                    a.in_send_emp,a.in_receive_emp,a.closed_flag
                        from r_repair_transfer a
                        left join c_sku b
                        on a.skuno = b.skuno
                        left join c_series c
                        on b.c_series_id = c.id
                        left join c_customer d
                        on c.customer_id = d.id
                        where a.sn not like 'RW%'
                        and substr(a.sn, 1, 1) not in ('*', '#', '~')
                        and a.in_time between to_date('{startDay}', 'yyyy-mm-dd') and
                            to_date('{endDayForCount}', 'yyyy-mm-dd')
                        TEMP_CUSTSQL
                        TEMP_SERISQL
                        TEMP_SKUSQL
                        order by a.sn, a.in_time";
                if (customer != "ALL")
                {
                    inSql = inSql.Replace("TEMP_CUSTSQL", $@" and d.customer_name = '{customer}' ");
                }
                else
                {
                    inSql = inSql.Replace("TEMP_CUSTSQL", " ");
                }
                if (series != "ALL")
                {
                    inSql = inSql.Replace("TEMP_SERISQL", $@" and c.series_name = '{series}' ");
                }
                else
                {
                    inSql = inSql.Replace("TEMP_SERISQL", " ");
                }
                if (skuNo != "ALL")
                {
                    inSql = inSql.Replace("TEMP_SKUSQL", $@" and a.skuno = '{skuNo}' ");
                }
                else
                {
                    inSql = inSql.Replace("TEMP_SKUSQL", " ");
                }

                string outSql = inSql.Replace("a.in_", "a.out_");
                #endregion
                
                #region 取dayList中每一天的CheckIn&CheckOut值
                DataTable inDT = SFCDB.RunSelect(inSql).Tables[0];
                DataTable outDT = SFCDB.RunSelect(outSql).Tables[0];
                if (inDT.Rows.Count == 0 || outDT.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }

                List<object> inDatas = new List<object>();
                List<object> outDatas = new List<object>();
                for (int j = 0; j < dayList.Count; j++)
                {
                    DataRow[] inRow = inDT.Select($@"in_time >= '{Convert.ToDateTime(dayList[j])}' and in_time < '{Convert.ToDateTime(dayList[j]).AddDays(1)}'");
                    DataRow[] outRow = outDT.Select($@"out_time >= '{Convert.ToDateTime(dayList[j])}' and out_time < '{Convert.ToDateTime(dayList[j]).AddDays(1)}'");
                    
                    inDatas.Add(inRow.Length);
                    outDatas.Add(outRow.Length);
                }
                #endregion

                #region 所有查詢起始時間之前未完成維修SQL語句
                //DCN的SP裡面就這麼寫的時間條件:取查詢時間之前
                string totalSql = $@"
                    select t1.*,t2.next_station,t2.from_storage,
                           t2.to_storage,t2.create_emp mrbby,t2.create_time,sysdate
                      from (select distinct d.customer_name,c.series_name,a.workorderno,a.skuno,
                                            a.id,a.sn,a.fail_station,a.create_time,a.closed_flag
                              from r_repair_main a
                              left join c_sku b
                                on a.skuno = b.skuno
                              left join c_series c
                                on b.c_series_id = c.id
                              left join c_customer d
                                on c.customer_id = d.id
                             where a.closed_flag = 0
                               and a.sn not like 'RW%'
                               and substr(a.sn, 1, 1) not in ('*', '#', '~')
                               and a.create_time < to_date('{startDay}', 'yyyy-mm-dd')) t1
                      left join (select distinct a.workorderno,a.sn,a.next_station,a.from_storage,
                                                 a.to_storage,a.create_emp,a.create_time
                                   from r_mrb a, r_repair_main b
                                  where a.sn = b.sn
                                    and a.rework_wo is null
                                    and b.closed_flag = 0
                                    and b.sn not like 'RW%'
                                    and substr(b.sn, 1, 1) not in ('*', '#', '~')
                                    and b.create_time < to_date('{startDay}', 'yyyy-mm-dd')) t2
                        on t1.sn = t2.sn
                       and t1.workorderno = t2.workorderno
                     where 1 = 1 TEMP_CUSTSQL TEMP_SERISQL TEMP_SKUSQL";
                if (customer != "ALL")
                {
                    totalSql = totalSql.Replace("TEMP_CUSTSQL", $@" and t1.customer_name = '{customer}' ");
                }
                else
                {
                    totalSql = totalSql.Replace("TEMP_CUSTSQL", " ");
                }
                if (series != "ALL")
                {
                    totalSql = totalSql.Replace("TEMP_SERISQL", $@" and t1.series_name = '{series}' ");
                }
                else
                {
                    totalSql = totalSql.Replace("TEMP_SERISQL", " ");
                }
                if (skuNo != "ALL")
                {
                    totalSql = totalSql.Replace("TEMP_SKUSQL", $@" and t1.skuno = '{skuNo}' ");
                }
                else
                {
                    totalSql = totalSql.Replace("TEMP_SKUSQL", " ");
                }
                #endregion

                #region 取自定義Table各欄位值
                DataTable totalDT = SFCDB.RunSelect(totalSql).Tables[0];
                int totalQty = totalDT.Rows.Count;    //Total數
                int scrapQty = 0;                     //報廢板數,MES端的R_MRB表沒有REASON_CODE='USELESS',先暫記0
                int B89YQty = 0;                      //異常板數,MES端的R_MRB表沒有REASON_CODE='B89Y',先暫記0
                int B89NQty = 0;                      //難板數,MES端的R_MRB表沒有REASON_CODE='B89N',先暫記0
                int B78MQty = totalDT.Select("to_storage='B78M'").Length;//待判倉未報廢數,其實還需要加一個條件:reason_code<>'USELESS'
                int B79MQty = totalDT.Select("to_storage='B79M'").Length;//盤點倉未報廢數,其實還需要加一個條件:reason_code<>'USELESS'

                string inLineSql = totalSql + " and not exists (select 1 from r_repair_transfer t3 where t1.sn = t3.sn and t1.id = t3.repair_main_id) ";
                int inLineQty = SFCDB.RunSelect(inLineSql).Tables[0].Rows.Count;//在產線數
                int reWipQty = totalQty - scrapQty - B89YQty - B78MQty - inLineQty;//REWip數:Total數量-報廢板-異常板-待判倉-InLine
                string lastInSql = inSql.Replace($@"a.in_time between to_date('{startDay}', 'yyyy-mm-dd') and to_date('{endDayForCount}', 'yyyy-mm-dd')",
                    $@"a.in_time < to_date('{startDay}', 'yyyy-mm-dd')");
                string lastOutSql = lastInSql.Replace("a.in_", "a.out_");
                DataTable lastInDT = SFCDB.RunSelect(lastInSql).Tables[0];
                DataTable lastOutDT = SFCDB.RunSelect(lastOutSql).Tables[0];
                int lastInOutQty = lastInDT.Rows.Count - lastOutDT.Rows.Count;//開始日期前CheckIn-Out數
                int inOutQty = inDT.Rows.Count - outDT.Rows.Count;//查詢時間內CheckIn-Out數
                int allWipQty = reWipQty + lastInOutQty + inOutQty;//所有維修WIP數
                #endregion
                
                #region 自定義WipTable
                DataTable wipDT = new DataTable();
                //wipDT.Columns.Add("Fail總數");
                //wipDT.Columns.Add("報廢板數");
                //wipDT.Columns.Add("異常板數(B89Y)");
                //wipDT.Columns.Add("難板數(B89N)");
                //wipDT.Columns.Add("B78M");
                //wipDT.Columns.Add("B79M");
                //wipDT.Columns.Add("INLINE");
                //wipDT.Columns.Add("實際維修WIP數");
                //wipDT.Columns.Add("開始日期前CheckIn-Out數");
                //wipDT.Columns.Add("查詢時間內CheckIn-Out數");
                //wipDT.Columns.Add("所有維修WIP數");

                wipDT.Columns.Add("Fail Qty");
                wipDT.Columns.Add("Scrap Qty");
                wipDT.Columns.Add("Abnormal Qty(B89Y)");
                wipDT.Columns.Add("Difficult Qty(B89N)");
                wipDT.Columns.Add("B78M");
                wipDT.Columns.Add("B79M");
                wipDT.Columns.Add("INLINE");
                wipDT.Columns.Add("Actual Repair WIP Qty");
                wipDT.Columns.Add("Before Start date CheckIn-Out Qty");
                wipDT.Columns.Add("Within query time CheckIn-Out Qty");
                wipDT.Columns.Add("All Repair WIP Qty");

                DataRow wipRow = wipDT.NewRow();
                //wipRow["Fail總數"] = totalQty;
                //wipRow["報廢板數"] = scrapQty;
                //wipRow["異常板數(B89Y)"] = B89YQty;
                //wipRow["難板數(B89N)"] = B89NQty;
                //wipRow["B78M"] = B78MQty;
                //wipRow["B79M"] = B79MQty;
                //wipRow["INLINE"] = inLineQty;
                //wipRow["實際維修WIP數"] = reWipQty;
                //wipRow["開始日期前CheckIn-Out數"] = lastInOutQty;
                //wipRow["查詢時間內CheckIn-Out數"] = inOutQty;
                //wipRow["所有維修WIP數"] = allWipQty;

                wipRow["Fail Qty"] = totalQty;
                wipRow["Scrap Qty"] = scrapQty;
                wipRow["Abnormal Qty(B89Y)"] = B89YQty;
                wipRow["Difficult Qty(B89N)"] = B89NQty;
                wipRow["B78M"] = B78MQty;
                wipRow["B79M"] = B79MQty;
                wipRow["INLINE"] = inLineQty;
                wipRow["Actual Repair WIP Qty"] = reWipQty;
                wipRow["Before Start date CheckIn-Out Qty"] = lastInOutQty;
                wipRow["Within query time CheckIn-Out Qty"] = inOutQty;
                wipRow["All Repair WIP Qty"] = allWipQty;
                wipDT.Rows.Add(wipRow);
                #endregion

                ReportTable retTab = new ReportTable();
                retTab.LoadData(wipDT, null);
                retTab.Tittle = "Repair Wip Report - Table Report";
                Outputs.Add(retTab);

                #region 柱狀圖參數屬性
                columnChart columnChart = new columnChart();
                List<ChartData> columnList = new List<ChartData>
                {
                    new ChartData { name = "CheckIn", data = inDatas, type = ChartType.column.ToString() },
                    new ChartData { name = "CheckOut", data = outDatas, type = ChartType.column.ToString() }
                };
                XAxis columnXAxis = new XAxis
                {
                    Title = "",
                    XAxisType = XAxisType.initValue,
                    Categories = dayList
                };
                Yaxis columnYAxis = new Yaxis
                {
                    Title = "Qty(pcs)"
                };
                columnChart.XAxis = columnXAxis;
                columnChart.YAxis = columnYAxis;
                columnChart.ChartDatas = columnList;
                #endregion

                Outputs.Add(columnChart);

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }

        public override void InputChangeEvent()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string customer = Customer.Value.ToString();
                if (customer != "ALL")
                {
                    ChangeCustomer(customer, SFCDB);
                }
                else
                {
                    InitSeries(SFCDB);
                }

                string series = Series.Value.ToString();
                if (series != "ALL")
                {
                    ChangeSeries(series, SFCDB);
                }
                else
                {
                    InitSkuNo(SFCDB);
                }

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }

        private void InitCustomer(OleExec db)
        {
            List<string> custList = new List<string>();
            T_C_CUSTOMER T_CCUST = new T_C_CUSTOMER(db, DB_TYPE_ENUM.Oracle);
            DataTable custDT = T_CCUST.GetCustomer(null, db);
            custList.Add("ALL");
            foreach (DataRow dr in custDT.Rows)
            {
                custList.Add(dr["CUSTOMER_NAME"].ToString());
            }
            Customer.ValueForUse = null;
            Customer.ValueForUse = custList;

        }

        private void InitSeries(OleExec db)
        {
            List<string> seriesList = new List<string>();
            T_C_SERIES T_CSERIES = new T_C_SERIES(db, DB_TYPE_ENUM.Oracle);
            DataTable seriesDT = T_CSERIES.GetQueryAll(null, null, db);
            seriesList.Add("ALL");
            foreach (DataRow dr in seriesDT.Rows)
            {
                seriesList.Add(dr["SERIES_NAME"].ToString());
            }
            Series.ValueForUse = null;
            Series.ValueForUse = seriesList;
        }

        private void InitSkuNo(OleExec db)
        {
            List<string> skuList = new List<string>();
            T_C_SKU T_CSKU = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
            DataTable skuDT = T_CSKU.GetALLSkuno(db);
            skuList.Add("ALL");
            foreach (DataRow dr in skuDT.Rows)
            {
                skuList.Add(dr["SKUNO"].ToString());
            }
            SkuNo.ValueForUse = null;
            SkuNo.ValueForUse = skuList;
        }

        private void ChangeCustomer(string customer, OleExec db)
        {
            List<string> seriesList = new List<string>();
            string seriesSql = $@"select distinct series_name from c_series where customer_id in (select id from c_customer where customer_name='{customer}') order by series_name";
            DataTable seriesDT = db.RunSelect(seriesSql).Tables[0];
            if (seriesDT.Rows.Count == 0)
            {
                throw new Exception("This Customer:" + customer + " Has No Config In Table:C_SERIES!");
            }
            seriesList.Add("ALL");
            foreach (DataRow dr in seriesDT.Rows)
            {
                seriesList.Add(dr["SERIES_NAME"].ToString());
            }
            Series.ValueForUse = null;
            Series.ValueForUse = seriesList;
        }

        private void ChangeSeries(string series, OleExec db)
        {
            List<string> skuList = new List<string>();
            string skuSql = $@"select distinct skuno from c_sku where c_series_id in (select id from c_series where series_name = '{series}') order by skuno";
            DataTable skuDT = db.RunSelect(skuSql).Tables[0];
            if (skuDT.Rows.Count == 0)
            {
                throw new Exception("This Series:" + series + " Has No Config In Table:C_SKU!");
            }
            skuList.Add("ALL");
            foreach (DataRow dr in skuDT.Rows)
            {
                skuList.Add(dr["SKUNO"].ToString());
            }
            SkuNo.ValueForUse = null;
            SkuNo.ValueForUse = skuList;
        }
    }
}
