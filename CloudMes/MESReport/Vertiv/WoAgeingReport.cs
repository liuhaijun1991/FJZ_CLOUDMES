using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Vertiv
{
    public class WoAgeingReport:ReportBase
    {
        ReportInput inputWO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuno = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public WoAgeingReport()
        {
            Inputs.Add(inputWO);
            Inputs.Add(inputSkuno);
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            //base.Run();
            string wo = inputWO.Value.ToString();
            string skuno = inputSkuno.Value.ToString();
            string runSql = "";
            string address = "";
            string addressSql = "";
            string woSql = "";
            double finishQty = 0;
            double max_time_qty = 0;
            string finishSql = "";
            DataTable dtWo = new DataTable();
            DataTable dtAddress = new DataTable();
            DataTable dtFinish = new DataTable();
            
            //if (wo == "" && skuno == "")
            //{
            //    ReportAlart alart = new ReportAlart("Please input WO or SKUNO!");
            //    Outputs.Add(alart);
            //    return;
            //}
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                woSql = " select workorderno from r_wo_base where closed_flag='0'  ";                
                if (!string.IsNullOrEmpty(wo))
                {
                    woSql = woSql + $@" and workorderno='{wo}' ";
                }
                if (!string.IsNullOrEmpty(skuno))
                {
                    woSql = woSql + $@" and skuno='{skuno}' ";
                }
                runSql = $@"select * from r_wo_ageing where  wo in ({woSql})";
                dtWo = SFCDB.RunSelect(runSql).Tables[0];
                if (dtWo.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                DataTable dtReport = new DataTable();
                DataRow drReport;
                dtReport.Columns.Add("WO");
                dtReport.Columns.Add("SKU");
                dtReport.Columns.Add("SHIPPING_ADDRESS");
                dtReport.Columns.Add("AGEING_TYPE");
                dtReport.Columns.Add("MAX_TIME");
                dtReport.Columns.Add("MAX_TIME_QTY");
                dtReport.Columns.Add("MIN_TIME");
                dtReport.Columns.Add("MIN_TIME_QTY");
                dtReport.Columns.Add("MAX_TIME_FINISHED_QTY");
                dtReport.Columns.Add("STATE");
                foreach (DataRow row in dtWo.Rows)
                {
                    address = "";
                    if (!string.IsNullOrEmpty(row["AGEING_AREA_CODE"].ToString()))
                    {
                        addressSql = $@"select * from c_shipping_address where shipping_area='{row["AGEING_AREA_CODE"].ToString()}'";
                        dtAddress = SFCDB.RunSelect(addressSql).Tables[0];
                        if (dtAddress.Rows.Count != 0)
                        {
                            address = dtAddress.Rows[0]["SHIPPING_ADDRESS"].ToString();
                        }
                    }
                    finishQty = 0;
                    max_time_qty = 0;
                    if (!string.IsNullOrEmpty(row["MAX_AGEING_TIME"].ToString()))
                    {
                        finishSql = $@"select count(distinct rtd.sn) as qty from r_test_detail_vertiv rtd,r_sn rsn where rsn.workorderno='{row["WO"].ToString()}'
                                        and rsn.sn=rtd.sn and rtd.station='BURNIN' and rtd.burnin_time='{row["MAX_AGEING_TIME"].ToString()}'";
                        dtFinish = SFCDB.RunSelect(finishSql).Tables[0];
                        if (dtFinish.Rows.Count != 0)
                        {
                            finishQty = Convert.ToDouble(dtFinish.Rows[0]["QTY"].ToString());
                        }
                        max_time_qty = Convert.ToDouble(row["MAX_TIME_QTY"].ToString());
                    }

                    drReport = dtReport.NewRow();
                    drReport["WO"] = row["WO"].ToString();
                    drReport["SKU"] = row["SKU"].ToString();
                    drReport["SHIPPING_ADDRESS"] = address;
                    drReport["AGEING_TYPE"] = row["AGEING_TYPE"].ToString();
                    drReport["MAX_TIME"] = row["MAX_AGEING_TIME"].ToString();
                    drReport["MAX_TIME_QTY"] = max_time_qty;
                    drReport["MIN_TIME"] = row["MIN_AGEING_TIME"].ToString();
                    drReport["MIN_TIME_QTY"] = row["MIN_TIME_QTY"].ToString();
                    drReport["MAX_TIME_FINISHED_QTY"] = finishQty;
                    drReport["STATE"] = (max_time_qty <= finishQty && finishQty != 0) ? "OK" : "FAIL";
                    dtReport.Rows.Add(drReport);
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportTable rt = new ReportTable();
                rt.LoadData(dtReport, null);
                rt.Tittle = "WO AGEING REPORT";               
                Outputs.Add(rt);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}
