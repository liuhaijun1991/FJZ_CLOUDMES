using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESReport.DCN
{
    /// <summary>
    /// 維修WIP狀態報表For DCN
    /// </summary>
    public class RepairWipStatus : ReportBase
    {
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairWipStatus()
        {
            Inputs.Add(SkuNo);
            Inputs.Add(StartDate);
            Inputs.Add(EndDate);
        }

        public override void Init()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {                
                InitSkuNo(SFCDB);
                StartDate.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                EndDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
                DBPools["SFCDB"].Return(SFCDB);
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

        public override void Run()
        {
            DateTime sTime = Convert.ToDateTime(StartDate.Value);
            DateTime eTime = Convert.ToDateTime(EndDate.Value).AddDays(1);
            string sValue = sTime.ToString("yyyy-MM-dd");
            string eValue = eTime.ToString("yyyy-MM-dd");
            string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.DCN.RepairWipStatusDetail&RunFlag=1&StartDate=" + sValue + "&EndDate=" + eValue + "&SkuNo=";

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                #region SQL語句
                string runSql = $@"
                    select t1.skuno,
                           t1.customer_name,
                           t1.failTotal,
                           nvl(t2.checkin, 0) checkin,
                           nvl(t3.checkout, 0) checkout,
                           nvl(t4.inline, 0) inline,
                           nvl(t5.rewip, 0) rewip
                      from (select a.skuno, nvl(t6.customer_name, 'N/A') customer_name, nvl(count(a.sn), 0) failTotal
                              from r_repair_main a
                              left join (select distinct b.skuno, d.customer_name
                                          from c_sku b, c_series c, c_customer d
                                         where b.c_series_id = c.id
                                           and c.customer_id = d.id) t6
                                on a.skuno = t6.skuno
                             where a.create_time between
                                   to_date('{sValue}', 'yyyy-mm-dd') and
                                   to_date('{eValue}', 'yyyy-mm-dd')
                               and a.fail_station <> 'COSMETIC-FAILURE'
                               and a.sn not like 'RW%'
                               TEMP_SQL
                             group by a.skuno, t6.customer_name) t1
                      left join (select b.skuno, nvl(count(b.sn), 0) checkin
                                   from r_repair_main a, r_repair_transfer b
                                  where a.id = b.repair_main_id
                                    and a.create_time between
                                        to_date('{sValue}', 'yyyy-mm-dd') and
                                        to_date('{eValue}', 'yyyy-mm-dd')
                                    and a.fail_station <> 'COSMETIC-FAILURE'
                                    and a.sn not like 'RW%'
                                    TEMP_SQL
                                  group by b.skuno) t2
                        on t1.skuno = t2.skuno
                      left join (select b.skuno, nvl(count(b.sn), 0) checkout
                                   from r_repair_main a, r_repair_transfer b
                                  where a.id = b.repair_main_id
                                    and a.create_time between
                                        to_date('{sValue}', 'yyyy-mm-dd') and
                                        to_date('{eValue}', 'yyyy-mm-dd')
                                    and b.closed_flag = 1
                                    and a.fail_station <> 'COSMETIC-FAILURE'
                                    and a.sn not like 'RW%'
                                    TEMP_SQL
                                  group by b.skuno) t3
                        on t1.skuno = t3.skuno
                      left join (select a.skuno, nvl(count(a.sn), 0) inline
                                   from r_repair_main a
                                  where a.create_time between
                                        to_date('{sValue}', 'yyyy-mm-dd') and
                                        to_date('{eValue}', 'yyyy-mm-dd')
                                    and a.id not in
                                        (select repair_main_id from r_repair_transfer)
                                    and a.fail_station <> 'COSMETIC-FAILURE'
                                    and a.sn not like 'RW%'
                                    TEMP_SQL
                                  group by a.skuno) t4
                        on t1.skuno = t4.skuno
                      left join (select a.skuno, nvl(count(a.sn), 0) rewip
                                   from r_repair_main a
                                  where a.create_time between
                                        to_date('{sValue}', 'yyyy-mm-dd') and
                                        to_date('{eValue}', 'yyyy-mm-dd')
                                    and a.id in (select repair_main_id
                                                   from r_repair_transfer
                                                  where closed_flag = 0)
                                    and a.fail_station <> 'COSMETIC-FAILURE'
                                    and a.sn not like 'RW%'
                                    TEMP_SQL
                                  group by a.skuno) t5
                        on t1.skuno = t5.skuno
                     where t1.skuno not like '*%'
                       and t1.failTotal > 0
                     order by t1.skuno";
                #endregion

                if (SkuNo.Value.ToString() != "ALL")
                {
                    runSql = runSql.Replace("TEMP_SQL", $@" and a.skuno = '{SkuNo.Value.ToString()}' ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_SQL", " ");
                }
                RunSqls.Add(runSql);
                DataTable resDT = SFCDB.RunSelect(runSql).Tables[0];

                DataTable linkTable = new DataTable();
                DataRow linkDataRow = null;
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("CUSTOMER_NAME");
                linkTable.Columns.Add("FAILTOTAL");
                linkTable.Columns.Add("CHECKIN");
                linkTable.Columns.Add("CHECKOUT");
                linkTable.Columns.Add("INLINE");
                linkTable.Columns.Add("REWIP");
                
                for (int i = 0; i < resDT.Rows.Count; i++)
                {
                    linkDataRow = linkTable.NewRow();
                    //跳轉的頁面鏈接
                    linkDataRow["SKUNO"] = "";
                    linkDataRow["CUSTOMER_NAME"] = "";
                    linkDataRow["FAILTOTAL"] = "";
                    linkDataRow["CHECKIN"] = "";
                    linkDataRow["CHECKOUT"] = "";
                    if (Convert.ToInt32(resDT.Rows[i]["INLINE"]) > 0)
                    {
                        linkDataRow["INLINE"] = linkURL + resDT.Rows[i]["SKUNO"].ToString() + "&Type=INLINE";
                    }
                    else
                    {
                        linkDataRow["INLINE"] = "";
                    }
                    if (Convert.ToInt32(resDT.Rows[i]["REWIP"]) > 0)
                    {
                        linkDataRow["REWIP"] = linkURL + resDT.Rows[i]["SKUNO"].ToString() + "&Type=REWIP";
                    }
                    else
                    {
                        linkDataRow["REWIP"] = "";
                    }
                    linkTable.Rows.Add(linkDataRow);
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(resDT, linkTable);
                retTab.Tittle = "Repair Wip Status";
                Outputs.Add(retTab);

                DBPools["SFCDB"].Return(SFCDB);
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

        public void InitSkuNo(OleExec db)
        {
            List<string> skuList = new List<string>();
            T_C_SKU T_CSKU = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
            DataTable skuDT = T_CSKU.GetALLSkuno(db);
            skuList.Add("ALL");
            foreach (DataRow dr in skuDT.Rows)
            {
                skuList.Add(dr["SKUNO"].ToString());
            }
            SkuNo.ValueForUse = skuList;
        }
    }
}
