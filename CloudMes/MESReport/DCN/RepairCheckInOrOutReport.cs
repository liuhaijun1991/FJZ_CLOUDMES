using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    /// <summary>
    /// 創建初衷是為了下載指定時間內所有CheckIn或者Out數據
    /// </summary>
    public class RepairCheckInOrOutReport : ReportBase
    {
        ReportInput CheckType = new ReportInput() { Name = "CheckType", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput Customer = new ReportInput() { Name = "Customer", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput Series = new ReportInput() { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairCheckInOrOutReport()
        {
            Inputs.Add(CheckType);
            Inputs.Add(Customer);
            Inputs.Add(Series);
            Inputs.Add(SkuNo);
            Inputs.Add(StartDate);
            Inputs.Add(EndDate);
        }

        public override void Init()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                InitCustomer(SFCDB);
                InitSeries(SFCDB);
                InitSkuNo(SFCDB);
                StartDate.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                EndDate.Value = DateTime.Now.ToString("yyyy-MM-dd");                

                List<string> typeList = new List<string>();
                typeList.Add("CHECKIN");
                typeList.Add("CHECKOUT");
                CheckType.ValueForUse = typeList;

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }

        public override void Run()
        {
            DateTime sTime = Convert.ToDateTime(StartDate.Value);
            DateTime eTime = Convert.ToDateTime(EndDate.Value).AddDays(1);
            string sValue = sTime.ToString("yyyy-MM-dd");
            string eValue = eTime.ToString("yyyy-MM-dd");
            string checkType = CheckType.Value.ToString();
            string customer = Customer.Value.ToString();
            string series = Series.Value.ToString();
            string skuNo = SkuNo.Value.ToString();

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                #region SQL語句
                string sql = $@"
                    select distinct '{checkType}' datatype,'{sValue}' datadate,
                                    d.customer_name customer,c.series_name series,
                                    a.skuno,a.workorderno,a.sn,a.station_name station,a.in_time,
                                    a.in_send_emp,a.in_receive_emp,a.out_time,a.out_send_emp,
                                    a.out_receive_emp,case a.closed_flag when '1' then 'True' when '0' then 'False' end closed
                        from r_repair_transfer a
                        left join c_sku b
                        on a.skuno = b.skuno
                        left join c_series c
                        on b.c_series_id = c.id
                        left join c_customer d
                        on c.customer_id = d.id
                        where a.sn not like 'RW%'
                        and substr(a.sn, 1, 1) not in ('*', '#', '~')
                        and a.in_time between to_date('{sValue}', 'yyyy-mm-dd') and
                            to_date('{eValue}', 'yyyy-mm-dd')
                        TEMP_CUSTSQL
                        TEMP_SERISQL
                        TEMP_SKUSQL
                        order by a.sn, a.in_time";
                if (customer != "ALL")
                {
                    sql = sql.Replace("TEMP_CUSTSQL", $@" and d.customer_name = '{customer}' ");
                }
                else
                {
                    sql = sql.Replace("TEMP_CUSTSQL", " ");
                }
                if (series != "ALL")
                {
                    sql = sql.Replace("TEMP_SERISQL", $@" and c.series_name = '{series}' ");
                }
                else
                {
                    sql = sql.Replace("TEMP_SERISQL", " ");
                }
                if (skuNo != "ALL")
                {
                    sql = sql.Replace("TEMP_SKUSQL", $@" and a.skuno = '{skuNo}' ");
                }
                else
                {
                    sql = sql.Replace("TEMP_SKUSQL", " ");
                }

                if (checkType == "CHECKOUT")
                {
                    sql = sql.Replace("a.in_time between", "a.out_time between");
                }
                #endregion

                DataTable resDT = SFCDB.RunSelect(sql).Tables[0];
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resDT, null);
                retTab.Tittle = $@"Repair {checkType} Report";
                Outputs.Add(retTab);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
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
