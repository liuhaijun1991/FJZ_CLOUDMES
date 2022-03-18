using MESDataObject.Module;
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
    public class NormalBonepileWeeklySummaryReport : ReportBase
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
        ReportInput inputIsHard = new ReportInput() { Name = "BadBoard", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Yes", "No" } };

        public NormalBonepileWeeklySummaryReport()
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
                listYear.Add(year);
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
                listWeek.Add(week);
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
            base.Run();
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
                    //throw new Exception("查詢起始時間的年份不能大於截止時間的年份,請確認!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816152948"));
                }
                if (int.Parse(fromYear) == int.Parse(toYear))
                {
                    if (int.Parse(fromWeek) > int.Parse(toWeek))
                    {
                        //throw new Exception("查詢起始時間的周別不能大於截止時間的周別,請確認!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816152948"));
                    }
                }
                BonepileSummaryReportBase rb = new BonepileSummaryReportBase();
                rb.Type = "WK";
                rb.DataClass = "Normal";
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
                reportTable.Tittle = $@"NN VERTIV NormalBonepileWeeklySummaryReport";
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

    public class BonepileSummaryReportBase
    {
        public string Type { get; set; }
        public string DataClass { get; set; }
        public int FromYear { get; set; }
        public int FromWeek { get; set; }
        public int ToYear { get; set; }
        public int ToWeek { get; set; }
        public string Category { get; set; }
        public string RMA { get; set; }
        public string Series { get; set; }
        public string SubSeries { get; set; }
        public string Product { get; set; }
        public string IsHard { get; set; }
        public string LinkUrl { get; set; }//$@"Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.FPYReportDetail&RunFlag=1

        public ReportTable GetReportTable(OleExec SFCDB)
        {
            ReportTable reportTable = new ReportTable();
            MESDataObject.Module.T_R_BONEPILE_BASIC t_r_bonepile_basic = new MESDataObject.Module.T_R_BONEPILE_BASIC(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Dictionary<string, object> dic = t_r_bonepile_basic.GetBonepileSummaryReportData(SFCDB, Type, DataClass,
                FromYear, FromWeek, ToYear, ToWeek, Category, RMA, Series, SubSeries, Product, IsHard);

            Dictionary<string, object> dicDate = (Dictionary<string, object>)dic["Date"];
            DataTable table1 = (DataTable)dic["DATA_1"];
            DataTable table2 = (DataTable)dic["DATA_2"];

            string from_year = dicDate["YEAR_1"].ToString();
            string to_year = dicDate["YEAR_2"].ToString();
            string link_url = "";
            List<int> listWeek1 = (List<int>)dicDate["LIST_WEEK_1"];
            List<int> listWeek2 = (List<int>)dicDate["LIST_WEEK_2"];

            if (string.IsNullOrEmpty(LinkUrl))
            {
                link_url = LinkUrl;
                if (string.IsNullOrEmpty(Category))
                {
                    link_url += $@"&Category ={Category}";                    
                }
                if (string.IsNullOrEmpty(RMA))
                {
                    link_url += $@"&RMA ={RMA}";
                }
                if (string.IsNullOrEmpty(Series))
                {
                    link_url += $@"&Series ={Series}";
                }
                if (string.IsNullOrEmpty(SubSeries))
                {
                    link_url += $@"&SubSeries ={SubSeries}";
                }
                if (string.IsNullOrEmpty(Product))
                {
                    link_url += $@"&Product ={Product}";
                }
                if (string.IsNullOrEmpty(IsHard))
                {
                    link_url += $@"&IsHard ={IsHard}";
                }
            }

            //前面兩個固定列 --begin
            int fixedColum = 2;
            reportTable.ColNames.Add("1");
            reportTable.ColNames.Add("2");
            //前面兩個固定列 --end
            TableRowView rowView = new TableRowView();
            TableColView colView = new TableColView();
            int colName = 2;
            int weekDataCol = 0;
            Dictionary<string, object> dData = new Dictionary<string, object>();

            #region Add Week Data
            BonepileSummaryBase sBase = null;
            if (FromYear == ToYear)
            {
                foreach (var w in listWeek1)
                {
                    sBase = new BonepileSummaryBase();
                    colName++;
                    weekDataCol++;
                    sBase.GetWeekData(table1, FromYear.ToString(), w.ToString());
                    dData.Add(colName.ToString(), sBase);
                    reportTable.ColNames.Add(colName.ToString());
                }
            }
            else
            {
                foreach (var w in listWeek1)
                {
                    sBase = new BonepileSummaryBase();
                    colName++;
                    weekDataCol++;
                    sBase.GetWeekData(table1, FromYear.ToString(), w.ToString());
                    dData.Add(colName.ToString(), sBase);
                    reportTable.ColNames.Add(colName.ToString());
                }
                foreach (var w2 in listWeek2)
                {
                    sBase = new BonepileSummaryBase();
                    colName++;
                    weekDataCol++;
                    sBase.GetWeekData(table1, ToYear.ToString(), w2.ToString());
                    dData.Add(colName.ToString(), sBase);
                    reportTable.ColNames.Add(colName.ToString());
                }
            }
            #endregion

            #region Add Aging Data
            List<string> listAging = new List<string> { "A", "B", "C", "D", "E" };
            int agingDataCol = listAging.Count;
            foreach (string a in listAging)
            {
                colName++;
                sBase = new BonepileSummaryBase();
                sBase.NewBonepileQty = 0;
                sBase.ClosedBonepileQty = 0;
                sBase.OpenBonepileQty = 0;
                sBase.OpenBonepileAmount = 0;
                switch (a)
                {
                    case "A":
                        sBase.Title = "0-30";
                        sBase.GetAgingData(table2, a);
                        break;
                    case "B":
                        sBase.Title = "31-60";
                        sBase.GetAgingData(table2, a);
                        break;
                    case "C":
                        sBase.Title = "61-90";
                        sBase.GetAgingData(table2, a);
                        break;
                    case "D":
                        sBase.Title = "91-120";
                        sBase.GetAgingData(table2, a);
                        break;
                    case "E":
                        sBase.Title = ">120";
                        sBase.GetAgingData(table2, a);
                        break;
                }
                dData.Add(colName.ToString(), sBase);
                reportTable.ColNames.Add(colName.ToString());
            }
            #endregion

            #region Drow Table                
            for (int i = 1; i <= 6; i++)
            {
                //第i行 i ROW
                rowView = new TableRowView();
                foreach (string col_name in reportTable.ColNames)
                {
                    //第col_name列 col_Name COL
                    colView = new TableColView();
                    colView.ColName = col_name;
                    if (i == 1)
                    {
                        colView.CellStyle = new Dictionary<string, object>() {
                                { "background", "#cccccc" },
                                { "color", "#000" },
                                { "font-size", "15px" },
                                { "font-weight","bold"}
                            };
                        if (col_name == "1")
                        {
                            colView.Value = "Category";
                            colView.ColSpan = 2;
                            colView.RowSpan = 2;
                        }
                        else if (col_name == "3")
                        {
                            colView.Value = Type == "WK" ? "Year/Week" : "Year/Month";
                            colView.ColSpan = weekDataCol;
                        }
                        else if (col_name == (weekDataCol + fixedColum + 1).ToString())
                        {
                            colView.Value = "Bonepile Aging";
                            colView.ColSpan = agingDataCol;
                        }
                        else
                        {
                            colView.ColSpan = 0;
                            colView.RowSpan = 0;
                        }
                    }
                    else if (i == 2)
                    {
                        colView.CellStyle = new Dictionary<string, object>() {
                                { "background", "#cccccc" },
                                { "color", "#000" },
                                { "font-size", "15px" },
                                { "font-weight","bold"}
                            };
                        if (col_name == "1" || col_name == "2")
                        {
                            colView.ColSpan = 0;
                            colView.RowSpan = 0;
                        }
                        else
                        {
                            colView.Value = ((BonepileSummaryBase)dData[col_name]).Title;
                        }
                    }
                    else
                    {
                        colView.CellStyle = new Dictionary<string, object>() {
                                { "background", "#ffff99" },
                                { "color", "#000" },
                                { "font-weight","bold"}
                            };
                        if (col_name == "1")
                        {
                            if (i == 3)
                            {
                                //colView.Value = "CriticalBonepile";
                                colView.Value = "NormalBonepile";
                                colView.RowSpan = 4;
                            }
                            else
                            {
                                colView.RowSpan = 0;
                                colView.Value = "";
                            }
                        }
                        else if (col_name == "2")
                        {
                            switch (i)
                            {
                                case 3:
                                    colView.Value = "New Bonepile Qty (pcs)";
                                    break;
                                case 4:
                                    colView.Value = "Closed Bonepile Qty (pcs)";
                                    break;
                                case 5:
                                    colView.Value = "Open Bonepile Qty (pcs)";
                                    break;
                                case 6:
                                    colView.Value = "Open Bonepile Amount (M$)";
                                    break;
                            }

                        }
                        else
                        {
                            switch (i)
                            {
                                case 3:
                                    colView.Value = ((BonepileSummaryBase)dData[col_name]).NewBonepileQty.ToString();
                                    if (((BonepileSummaryBase)dData[col_name]).NewBonepileQty > 0)
                                    {
                                        colView.LinkType = "Link";
                                        colView.LinkData = link_url;
                                    }
                                    break;
                                case 4:
                                    colView.Value = ((BonepileSummaryBase)dData[col_name]).ClosedBonepileQty.ToString();
                                    if (((BonepileSummaryBase)dData[col_name]).ClosedBonepileQty > 0)
                                    {
                                        colView.LinkType = "Link";
                                        colView.LinkData = link_url;
                                    }
                                    break;
                                case 5:
                                    colView.Value = ((BonepileSummaryBase)dData[col_name]).OpenBonepileQty.ToString();
                                    if (((BonepileSummaryBase)dData[col_name]).OpenBonepileQty > 0)
                                    {
                                        colView.LinkType = "Link";
                                        colView.LinkData = link_url;
                                    }
                                    break;
                                case 6:
                                    //colView.Value = ((BonepileSummaryBase)dData[col_name]).OpenBonepileAmount.ToString();
                                    //這個的數據來源是r_normal_bonepile 的price,價格的配置是在r_pn_master_data
                                    colView.Value = (System.Math.Round(((BonepileSummaryBase)dData[col_name]).OpenBonepileAmount / 1000000, 2)).ToString("0.00");
                                    //if (((BonepileSummaryBase)dData[col_name]).OpenBonepileAmount > 0)
                                    //{
                                    //    colView.LinkType = "Link";
                                    //    colView.LinkData = "LinkUrl";
                                    //}
                                    break;
                            }
                        }
                    }
                    rowView.RowData.Add(colView.ColName, colView);
                }
                reportTable.Rows.Add(rowView);
            }
            #endregion
            return reportTable;
        }

        public class BonepileSummaryBase
        {
            public string Title { get; set; }
            public double NewBonepileQty { get; set; }
            public double ClosedBonepileQty { get; set; }
            public double OpenBonepileQty { get; set; }
            public double OpenBonepileAmount { get; set; }

            public void GetWeekData(DataTable dt, string year, string week)
            {
                Title = year + "/" + week;
                NewBonepileQty = 0;
                ClosedBonepileQty = 0;
                OpenBonepileQty = 0;
                OpenBonepileAmount = 0;
                DataRow[] drWeek = dt.Select($@"YEAR='{year}' And WEEK_OR_MONTH='{week}'");
                foreach (DataRow dr in drWeek)
                {
                    double qty = Convert.ToDouble(dr["QTY"].ToString());
                    switch (dr["STATUS"].ToString())
                    {
                        case "OpenAmount":
                            OpenBonepileAmount = qty;
                            break;
                        case "Open":
                            OpenBonepileQty = qty;
                            break;
                        case "New":
                            NewBonepileQty = qty;
                            break;
                        case "Closed":
                            ClosedBonepileQty = qty;
                            break;
                        default:
                            break;
                    }
                }
            }

            public void GetAgingData(DataTable dt, string aging)
            {
                NewBonepileQty = 0;
                ClosedBonepileQty = 0;
                OpenBonepileQty = 0;
                OpenBonepileAmount = 0;
                DataRow[] drWeek = dt.Select($@"AGING='{aging}'");
                foreach (DataRow dr in drWeek)
                {
                    double qty = (dr["QTY"] == null || dr["QTY"].ToString() == "") ? 0 : Convert.ToDouble(dr["QTY"].ToString());
                    switch (dr["STATUS"].ToString())
                    {
                        case "OpenAmount":
                            OpenBonepileAmount = qty;
                            break;
                        case "Open":
                            OpenBonepileQty = qty;
                            break;
                        case "New":
                            NewBonepileQty = qty;
                            break;
                        case "Closed":
                            ClosedBonepileQty = qty;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
