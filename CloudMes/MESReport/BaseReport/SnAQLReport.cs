using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SnAQLReport : ReportBase
    {
        ReportInput inputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput FromDay = new ReportInput() { Name = "FromDay", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ToDay = new ReportInput() { Name = "ToDay", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Series = new ReportInput() { Name = "Series", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SnAQLReport()
        {
            Inputs.Add(inputSku);
            Inputs.Add(FromDay);
            Inputs.Add(ToDay);
            Inputs.Add(Station);
            Inputs.Add(Series);

        }
        public override void Run()
        {
            //base.Run();
            string sku = inputSku.Value.ToString();
            string fromDay = FromDay.Value.ToString().Replace("%20", " ");
            string toDay = ToDay.Value.ToString().Replace("%20", " ");
            string sqlRun = string.Empty;
            //string line = inputLine.Value.ToString();
            string station = Station.Value.ToString();
            string series = Series.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string finalSKU = string.Empty;
            if (sku.IndexOf(',') != -1)
            {
                List<string> result = sku.Split(new char[] { ',' }).ToList();
                string t = string.Empty;
                for (int i = 0; i < result.Count; i++)
                {
                    t += "'" + result[i].ToString() + "',";
                }
                finalSKU = t.Remove(t.Length - 1, 1);
            }
            else
            {
                finalSKU = $@"'{sku}'";
            }
            sqlRun = $@"  SELECT  a.sn,b.SKUNO,b.WORKORDERNO,a.STATE,a.TESTATION, a.EDIT_EMP,b.CURRENT_STATION,b.NEXT_STATION   FROM SFCRUNTIME.R_TEST_RECORD a, r_sn b WHERE a.SN=b.SN AND a.SN IN (
                           SELECT SN FROM r_sn WHERE SKUNO IN (
                           SELECT  SKUNO FROM c_sku WHERE skuno in({finalSKU}) AND C_SERIES_ID IN ( 
                           SELECT ID FROM SFCBASE.C_SERIES WHERE CUSTOMER_ID IN (
                           SELECT ID FROM C_CUSTOMER WHERE CUSTOMER_NAME ='{series}'))) AND VALID_FLAG=1) AND a.R_SN_ID IS NOT null AND a.MESSTATION = 'AQL' AND a.EDIT_TIME between
                            TO_DATE('{fromDay}', 'YYYY/MM/DD hh24:mi:ss') and
                            TO_DATE('{toDay}', 'YYYY/MM/DD hh24:mi:ss') AND a.STATE = '{station}' ";

            try
            {
                DataTable res = SFCDB.ExecuteDataTable(sqlRun, CommandType.Text);
                if (res.Rows.Count == 0)
                {
                    throw new Exception("No data");
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res, null);
                retTab.Tittle = "SnAQLReport";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
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
