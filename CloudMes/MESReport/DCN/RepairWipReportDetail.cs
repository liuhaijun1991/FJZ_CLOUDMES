using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESReport.DCN
{
    /// <summary>
    /// 維修WIP Detail報表For DCN
    /// </summary>
    public class RepairWipReportDetail : MesAPIBase
    {
        protected APIInfo _GetWipReportDetail = new APIInfo()
        {
            FunctionName = "GetWipReportDetail",
            Description = "獲取WipByDay報表詳細數據",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Date", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };

        public RepairWipReportDetail()
        {
            this.Apis.Add(_GetWipReportDetail.FunctionName, _GetWipReportDetail);
        }

        public void GetWipReportDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string dateFrom = Data["Date"].ToString();
                string dateTo = Convert.ToDateTime(dateFrom).AddDays(1).ToString("yyyy-MM-dd");
                string type = Data["Type"].ToString().ToUpper();
                string customer = Data["Customer"].ToString().ToUpper();
                string series = Data["Series"].ToString().ToUpper();
                string skuNo = Data["SkuNo"].ToString().ToUpper();

                #region SQL語句
                string sql = $@"
                    select distinct '{type}' datatype,'{dateFrom}' datadate,
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
                        and a.in_time between to_date('{dateFrom}', 'yyyy-mm-dd') and
                            to_date('{dateTo}', 'yyyy-mm-dd')
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

                if (type == "CHECKOUT")
                {
                    sql = sql.Replace("a.in_time between", "a.out_time between");
                }
                #endregion

                DataTable resDT = SFCDB.RunSelect(sql).Tables[0];
                if (resDT.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resDT, null);
                retTab.Tittle = "Repair Wip By Day - Detail Report";

                
                StationReturn.Data = retTab;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    }
}
