using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESReport.BaseReport
{
    public class NormalBonepileMonthlySummaryReport : ReportBase
    {
        ReportInput inputFromYear = new ReportInput() { Name = "FromYear", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputFromMonth = new ReportInput() { Name = "FromMonth", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputToYear = new ReportInput() { Name = "ToYear", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputToMonth = new ReportInput() { Name = "ToMonth", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCategory = new ReportInput() { Name = "Category", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Function", "Cosmetic" } };
        ReportInput inputSeries = new ReportInput() { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSubSeries = new ReportInput() { Name = "SubSeries", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputProduct = new ReportInput() { Name = "Product", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        // ReportInput inputIsHard = new ReportInput() { Name = "是否難板", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Yes", "No" } };
        ReportInput inputIsHard = new ReportInput() { Name = "BadBoard", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Yes", "No" } };

        public NormalBonepileMonthlySummaryReport()
        {
            Inputs.Add(inputFromYear);
            Inputs.Add(inputFromMonth);
            Inputs.Add(inputToYear);
            Inputs.Add(inputToMonth);
            Inputs.Add(inputCategory);
            Inputs.Add(inputSeries);
            Inputs.Add(inputSubSeries);
            Inputs.Add(inputProduct);
            Inputs.Add(inputIsHard);
        }

        public override void Init()
        {
            base.Init();
            OleExec SFCDB = null;
            try
            {
                #region year
                string year = System.DateTime.Now.Year.ToString();
                List<string> listYear = new List<string>();
                for (int y = 2008; y <= int.Parse(year); y++)
                {
                    listYear.Add(y.ToString());
                }
                inputFromYear.ValueForUse = listYear;
                inputFromYear.Value = year;
                inputToYear.ValueForUse = listYear;
                inputToYear.Value = year;
                #endregion
                #region week
                string sql = "select to_char(sysdate,'MM') as week from dual";
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                string week = dt.Rows[0][0].ToString();
                List<string> listWeek = new List<string>();
                for (int w = 1; w < int.Parse(week); w++)
                {
                    listWeek.Add(w.ToString());
                }
                inputFromMonth.ValueForUse = listWeek;
                inputFromMonth.Value = week;
                inputToMonth.ValueForUse = listWeek;
                inputToMonth.Value = week;
                #endregion

                sql = "select distinct series_name from c_series";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listSeries = new List<string>();
                listSeries.Add("ALL");
                for (int l = 0; l < dt.Rows.Count; l++)
                {
                    listSeries.Add(dt.Rows[l][0].ToString());
                }
                inputSeries.ValueForUse = listSeries;
                inputSubSeries.ValueForUse = listSeries;

                sql = "select distinct skuno from c_sku where skuno is not null order by skuno";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listProduct = new List<string>();
                listProduct.Add("ALL");
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    listProduct.Add(dt.Rows[n][0].ToString());
                }
                inputProduct.ValueForUse = listProduct;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void Run()
        {
            base.Run();
            string fromYear = inputFromYear.Value.ToString();
            string fromMonth = inputFromMonth.Value.ToString();
            string toYear = inputToYear.Value.ToString();
            string toMonth = inputToMonth.Value.ToString();
            string category = inputCategory.Value.ToString().ToUpper() == "ALL" ? "" : inputCategory.Value.ToString();
            string series = inputSeries.Value.ToString().ToUpper() == "ALL" ? "" : inputSeries.Value.ToString();
            string subSeries = inputSubSeries.Value.ToString().ToUpper() == "ALL" ? "" : inputSubSeries.Value.ToString();
            string product = inputProduct.Value.ToString().ToUpper() == "ALL" ? "" : inputProduct.Value.ToString();
            string isHard = inputIsHard.Value.ToString().ToUpper() == "ALL" ? "" : inputIsHard.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                if (int.Parse(fromYear) > int.Parse(toYear))
                {
                    //throw new Exception("查詢起始時間的年份不能大於截止時間的年份,請確認!");

                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816152119"));

                }
                if (int.Parse(fromYear) == int.Parse(toYear))
                {
                    if (int.Parse(fromMonth) > int.Parse(toMonth))
                    {
                        //throw new Exception("查詢起始時間的月份不能大於截止時間的月份,請確認!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816152119"));
                    }
                }
                BonepileSummaryReportBase rb = new BonepileSummaryReportBase();
                rb.Type = "MO";
                rb.DataClass = "Normal";
                rb.FromYear = Convert.ToInt32(fromYear);
                rb.FromWeek = Convert.ToInt32(fromMonth);
                rb.ToYear = Convert.ToInt32(toYear);
                rb.ToWeek = Convert.ToInt32(toMonth);
                rb.Category = category;
                rb.Series = series;
                rb.SubSeries = subSeries;
                rb.Product = product;
                rb.IsHard = isHard;
                ReportTable reportTable = rb.GetReportTable(SFCDB);
                reportTable.Tittle = $@"NN VERTIV NormalBonepileMonthlySummaryReport";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }
}
