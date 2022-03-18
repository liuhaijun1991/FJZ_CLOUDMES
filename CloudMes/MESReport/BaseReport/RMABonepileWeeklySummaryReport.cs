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
    public class RMABonepileWeeklySummaryReport : ReportBase
    {
        ReportInput inputFromYear = new ReportInput() { Name = "FromYear", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputFromWeek = new ReportInput() { Name = "FromWeek", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputToYear = new ReportInput() { Name = "ToYear", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputToWeek = new ReportInput() { Name = "ToWeek", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCategory = new ReportInput() { Name = "Category", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Function", "Cosmetic" } };
        ReportInput inputSeries = new ReportInput() { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSubSeries = new ReportInput() { Name = "SubSeries", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputProduct = new ReportInput() { Name = "Product", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputIsHard = new ReportInput() { Name = "是否難板", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Yes", "No" } };
        ReportInput inputIsHard = new ReportInput() { Name = "IsHard", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Yes", "No" } };

        public RMABonepileWeeklySummaryReport()
        {
            Inputs.Add(inputFromYear);
            Inputs.Add(inputFromWeek);
            Inputs.Add(inputToYear);
            Inputs.Add(inputToWeek);
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
                string sql = "select to_char(sysdate,'iw') as week from dual";
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                string week = dt.Rows[0][0].ToString();
                List<string> listWeek = new List<string>();
                for (int w = 1; w < int.Parse(week); w++)
                {
                    listWeek.Add(w.ToString());
                }
                inputFromWeek.ValueForUse = listWeek;
                inputFromWeek.Value = week;
                inputToWeek.ValueForUse = listWeek;
                inputToWeek.Value = week;
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
            string fromYear = inputFromYear.Value.ToString();
            string fromWeek = inputFromWeek.Value.ToString();
            string toYear = inputToYear.Value.ToString();
            string toWeek = inputToWeek.Value.ToString();
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
                    throw new Exception("查詢起始時間的年份不能大於截止時間的年份,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816163124"));
                }
                if (int.Parse(fromYear) == int.Parse(toYear))
                {
                    if (int.Parse(fromWeek) > int.Parse(toWeek))
                    {
                        throw new Exception("查詢起始時間的周別不能大於截止時間的周別,請確認!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816165504"));
                    }
                }
                BonepileSummaryReportBase rb = new BonepileSummaryReportBase();
                rb.Type = "WK";
                rb.DataClass = "RMA";
                rb.FromYear = Convert.ToInt32(fromYear);
                rb.FromWeek = Convert.ToInt32(fromWeek);
                rb.ToYear = Convert.ToInt32(toYear);
                rb.ToWeek = Convert.ToInt32(toWeek);
                rb.Category = category;
                rb.Series = series;
                rb.SubSeries = subSeries;
                rb.Product = product;
                rb.IsHard = isHard;
                ReportTable reportTable = rb.GetReportTable(SFCDB);
                reportTable.Tittle = $@"RMABonepileWeeklySummaryReport";
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

        public override void DownFile()
        {
            base.DownFile();
        }
    }
}
