using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class ASSYReport : ReportBase
    {
        ReportInput WoInput = new ReportInput()
        {
            Name = "WO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        public ASSYReport()
        {
            this.Inputs.Add(WoInput);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            string inputValue = WoInput.Value.ToString();
            if (string.IsNullOrEmpty(inputValue))
            {
                return;
            }
            DataTable dt = null;
            DataTable dt2 = null;
            //DataTable dt3 = null;
            string workorderno = null;
            string route_id = null;
            string skuno = null;
            string release = null;
            string workorderqty = null;
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            //OleExec apdb = DBPools["APDB"].Borrow();
            try
            {
                string logicSQL = null;
                //string baseSQL = $@"select wo.cust_order_no 工令,wo.workorderno 工單,wo.skuno 機種,wo.sku_ver 版本,wo.workorder_qty 總數,wo.release_date,'' 預計完工日,wo.finished_qty 完工數,
                //        (case when wo.workorder_qty = 0 or wo.workorder_qty is null then 0 else round(nvl(wo.finished_qty, 0)*100/wo.workorder_qty, 2) end) as 完工率, 
                //        wo.route_id 
                //        from r_wo_base wo where 1=1 ";

                string baseSQL = $@"select wo.cust_order_no ,wo.workorderno ,wo.skuno ,wo.sku_ver Version,wo.workorder_qty TotalQty,wo.release_date,'' PredictedTime,wo.finished_qty,
                        (case when wo.workorder_qty = 0 or wo.workorder_qty is null then 0 else round(nvl(wo.finished_qty, 0)*100/wo.workorder_qty, 2) end) as CompletionRate, 
                        wo.route_id 
                        from r_wo_base wo where 1=1 ";

                baseSQL += $@"and wo.workorderno='{inputValue.Replace("'", "''")}'";

                dt = sfcdb.RunSelect(baseSQL).Tables[0];
                dt.Columns.Add("OnlineTime");
                dt.Columns.Add("OnlineAlert");
                dt.Columns.Add("OfflinePoint");
                dt.Columns.Add("OfflineTime");
                dt.Columns.Add("OfflineQty");
                dt.Columns.Add("OfflineAlert");
                foreach (DataRow dr in dt.Rows)
                {
                    //workorderno = dr["工單"].ToString();
                    //route_id = dr["route_id"].ToString();
                    //skuno = dr["機種"].ToString();
                    //release = dr["release_date"].ToString();
                    //workorderqty = dr["總數"].ToString();

                    workorderno = dr["workorderno"].ToString();
                    route_id = dr["route_id"].ToString();
                    skuno = dr["skuno"].ToString();
                    release = dr["release_date"].ToString();
                    workorderqty = dr["TotalQty"].ToString();

                    #region
                    logicSQL = $@"select min(start_time) onlinetime from r_sn_station_detail 
                        where workorderno='{workorderno}' ";
                    dt2 = sfcdb.RunSelect(logicSQL).Tables[0];
                    if (dt2.Rows.Count > 0)
                    {
                        string onlinetime = dt2.Rows[0][0].ToString();
                    }
                    else
                    {

                    }

                    #endregion
                }

               
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, null);
                retTab.Tittle = "ASSY Manufacture Report";
                Outputs.Add(retTab);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (sfcdb != null) DBPools["SFCDB"].Return(sfcdb);
            }
          
        }

    }
}
