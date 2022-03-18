using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.HWD
{
    public class HWDStockReport : ReportBase
    {
        ReportInput StockNo = new ReportInput()
        {
            Name = "StockNo",
            InputType = "Select",
            SendChangeEvent = false,
            ValueForUse = new string[] { "ALL", "12BH40" , "12JS05" },
            Value = "ALL"
            
        };
        ReportInput fromDate = new ReportInput()
        {
            Name = "From",
            InputType = "DateTime",
            //Value = "2018-02-01",
            Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        public HWDStockReport()
        {
            fromDate.Value = DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd");
            this.Inputs.Add(StockNo);
            this.Inputs.Add(fromDate);
        }
        public override void Run()
        {
            DateTime ft = (DateTime)fromDate.Value;

            var DB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string strSql = $@"SELECT rownum NO,
       SKU.CUST_PARTNO,
       SN.SN,
       S.LOT_NO,
       S.LOT_QTY,
       'XXX' WORKTIME,
       'QQQ' DAYS,
       S.SAMPLE_STATION
  FROM R_LOT_STATUS S
 INNER JOIN R_LOT_DETAIL D
    ON S.LOT_NO = D.LOT_ID
 INNER JOIN R_SN SN
    ON D.SN = SN.SN
 INNER JOIN C_SKU SKU
    ON SN.SKUNO = SKU.SKUNO
 WHERE S.LOT_NO LIKE 'LOT-%'
   AND S.AQL_TYPE IS NULL
   AND SN.VALID_FLAG = 1
   AND D.STATUS = 0
   AND S.EDIT_TIME >to_date('{ft.ToString("yyyy-MM-dd")}','yyyy-mm-dd')";
                if (StockNo.Value.ToString() != "ALL")
                {
                    strSql += $@" AND S.SAMPLE_STATION = '{StockNo.Value.ToString()}'";
                }
                strSql += " order by s.lot_no";
                var res = DB.RunSelect(strSql);
                var yearCodeMapping = Common.SNmaker.GetCodeMapping("2YEAR", DB);
                var MMCodeMapping = Common.SNmaker.GetCodeMapping("HWDSN_MM", DB);
                var DDCodeMapping = Common.SNmaker.GetCodeMapping("HWDSN_DD", DB);
                var now = DateTime.Now;
                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr = res.Tables[0].Rows[i];
                        string SN = dr["SN"].ToString();
                        string yearCode = SN.Substring(6, 2);
                        string MMCode = SN.Substring(8, 1);
                        string DDCode = SN.Substring(9, 1);
                        var year = yearCodeMapping.Find(T => T.CODEVALUE == yearCode).VALUE;
                        var MM = MMCodeMapping.Find(T => T.CODEVALUE == MMCode).VALUE;
                        var DD = DDCodeMapping.Find(T => T.CODEVALUE == DDCode).VALUE;
                        dr["WORKTIME"] = year + MM + DD;
                        dr["DAYS"] = ((int)(now - DateTime.Parse($@"{year}-{MM}-{DD}")).TotalDays).ToString();
                    }
                    catch
                    {

                    }
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "OBA REPORT";
                Outputs.Add(retTab);
            }
            catch(Exception e)
            {
                ReportAlart alart = new ReportAlart(e.Message);
                Outputs.Add(alart);
            }
            finally
            {
                DBPools["SFCDB"].Return(DB);
            }

        }

    }
}
