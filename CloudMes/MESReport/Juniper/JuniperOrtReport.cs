using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    class JuniperOrtReport: ReportBase
    {

        ReportInput Sku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "5DX", "ORT" } };
        ReportInput inputStatus = new ReportInput() { Name = "Status", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "OPEN" ,"CLOSED", "CANCEL"} };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public JuniperOrtReport()
        {
            Inputs.Add(Sku);
            Inputs.Add(inputSN);
            Inputs.Add(inputStation);
            Inputs.Add(inputStatus);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }
        public override void Init()
        {
            StartTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd");
            EndTime.Value = DateTime.Now.ToString("yyyy/MM/dd");

        }
        public override void Run()
        {
            DateTime sTime = Convert.ToDateTime(StartTime.Value);
            DateTime eTime = Convert.ToDateTime(EndTime.Value).AddDays(1);
            string sValue = sTime.ToString("yyyy-MM-dd");
            string eValue = eTime.ToString("yyyy-MM-dd");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            
            try
            {
                var status = "";
                switch (inputStatus.Value)
                {
                    case "OPEN":
                        status = "0";
                        break;
                    case "CLOSED":
                        status = "1";
                        break;
                    case "CANCEL":
                        status = "2";
                        break;
                    default:
                        break;
                        
                }

                //String sql = $@"select rls.skuno,rld.workorderno,rld.sn,rls.sample_qty,rls.pass_qty ,CASE rld.status WHEN '0' then 'OPEN' WHEN '1' THEN 'CLOSED' END AS STATUS  from r_lot_status rls,r_lot_detail rld where rls.lot_no=rld.lot_id and rls.sample_station='ORT'";
                string sql = $@"
                SELECT RLS.SKUNO,
                       RLD.WORKORDERNO,
                       RLD.SN,
                       RLS.SAMPLE_QTY,
                       RLS.PASS_QTY,
                       RLS.FAIL_QTY,
                       CASE RLD.STATUS
                         WHEN '0' THEN
                          'OPEN'
                         WHEN '1' THEN
                          'CLOSED'
                         WHEN '2' THEN
                          'CANCEL'
                       END AS STATUS,
                       RLD.EDIT_EMP,
                       RLD.EDIT_TIME
                  FROM R_LOT_STATUS RLS, R_LOT_DETAIL RLD
                 WHERE RLS.LOT_NO = RLD.LOT_ID";

                if (Sku.Value.ToString() != "ALL")
                {
                    sql += $@" and rls.skuno = '{Sku.Value}'";
                }
                if (inputSN.Value.ToString() != "ALL")
                {
                    sql += $@" AND RLD.SN = '{inputSN.Value}'";
                }
                if (sValue != "" && eValue != "")
                {
                    sql += $@" and rld.CREATE_DATE>TO_DATE('{sValue}','yyyy/MM/dd') and rld.CREATE_DATE<TO_DATE('{eValue}','yyyy/MM/dd')";
                }
                if (inputStatus.Value.ToString() !="ALL")
                {
                    sql += $@" and rld.STATUS='{status}'";
                }
                if (inputStation.Value.ToString() != "ALL")
                {
                    sql += $@" and RLS.SAMPLE_STATION='{inputStation.Value}'";
                }
                sql += $@" ORDER BY SKUNO,WORKORDERNO,SN ";

                RunSqls.Add(sql);
                DataSet res = SFCDB.RunSelect(sql);
                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "ORT Report";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }


        }
    }
}
