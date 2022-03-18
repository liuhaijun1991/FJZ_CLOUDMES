using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BPD
{
    public class WipReport : ReportBase
    {
        OleExec SFCDB = null;
        public WipReport()
        { }

        public string ChangeStationName(string OldStationName)
        {
            string NewStationName = OldStationName;
            if (OldStationName.Equals("STOCKIN"))
            {
                NewStationName = "S101";
            }
            else if (OldStationName.Equals("CBS"))
            {
                NewStationName = "F101";
            }
            else if (OldStationName.Contains("B29"))
            {
                NewStationName = "F106";
            }
            return NewStationName;
        }

        public override void Run()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            string linkURL = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.SkuWipReport&RunFlag=1&SKUNO=";
            List<R_WO_BASE> WoBases = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.CLOSED_FLAG == "0").ToList();
            List<string> Skus = new List<string>();
            List<string> Stations = new List<string>();
            List<string> TempRoute = new List<string>();
            List<C_ROUTE_DETAIL> Details = new List<C_ROUTE_DETAIL>();

            foreach (R_WO_BASE wb in WoBases)
            {
                if (!Skus.Contains(wb.SKUNO))
                {
                    Skus.Add(wb.SKUNO);
                }
                if (!TempRoute.Contains(wb.ROUTE_ID))
                {
                    TempRoute.Add(wb.ROUTE_ID);
                    SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == wb.ROUTE_ID).OrderBy(t=>t.SEQ_NO).ToList().ForEach(t=> { if (!Stations.Contains(t.STATION_NAME)) Stations.Add(t.STATION_NAME); }); 
                }
            }

            //填充首行
            DataTable WipData = new DataTable();
            WipData.Columns.Add("SkuNo");
            foreach (string Station in Stations)
            {
                
                WipData.Columns.Add(ChangeStationName(Station));   
            }
            WipData.Columns.Add("JOBFINISH");
            WipData.Columns.Add("Repair");
            WipData.Columns.Add("F106");
            WipData.Columns.Add("JOBSTOCKIN");
            WipData.Columns.Add("REWORK");
            WipData.Columns.Add("RMA");
            WipData.PrimaryKey = new DataColumn[] { WipData.Columns["SkuNo"] };

            //填充首列
            foreach (string Sku in Skus)
            {
                DataRow dr = WipData.NewRow();
                dr["SkuNo"] = Sku;
                WipData.Rows.Add(dr);
            }
            DataRow TotalRow = WipData.NewRow();
            TotalRow["SkuNo"] = "Total";
            WipData.Rows.Add(TotalRow);

            //所有機種所有站位分佈
            DataTable SkuData = SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((rs, rwb) => rs.WORKORDERNO == rwb.WORKORDERNO)
                .Where((rs, rwb) => rwb.CLOSED_FLAG == "0" && rs.VALID_FLAG == "1" 
                && rs.SHIPPED_FLAG != "1" && rs.REPAIR_FAILED_FLAG != "1" && rs.SCRAPED_FLAG != "1")
                .GroupBy((rs,rwb)=>rwb.SKUNO).GroupBy((rs,rwb)=>rs.NEXT_STATION)
                .Select("rwb.skuno,rs.next_station,count(rs.sn) as count").ToDataTable();

            DataTable ReworkData = SFCDB.ORM.Queryable<R_SN>().Where(rs => rs.VALID_FLAG == "1" && rs.SHIPPED_FLAG != "1" && rs.REPAIR_FAILED_FLAG != "1" && rs.SCRAPED_FLAG != "1" && rs.NEXT_STATION == "REWORK")
                .GroupBy(rs => rs.SKUNO)
                .Select("skuno,count(sn) as count").ToDataTable();
            //repair 數據
            DataTable RepairData = SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((rs, rwb) => rs.WORKORDERNO == rwb.WORKORDERNO)
                .Where((rs, rwb) => rwb.CLOSED_FLAG == "0" && rs.VALID_FLAG == "1" && rs.REPAIR_FAILED_FLAG == "1")
                .GroupBy((rw, rwb) => rwb.SKUNO).Select("rwb.skuno,count(rs.sn) as count").ToDataTable();
            
            //所有機種數據填充
            foreach (DataRow dr in SkuData.Rows)
            {
                DataRow WipDr = WipData.Rows.Find(dr["skuno"]);
                if (dr["next_station"].ToString() != "REWORK" && WipData.Columns.Contains(ChangeStationName(dr["next_station"].ToString())))
                {
                    WipDr[ChangeStationName(dr["next_station"].ToString())] = dr["count"];
                }
            }

            foreach (DataRow dr in ReworkData.Rows)
            {
                DataRow ReworkDr = WipData.Rows.Find(dr["skuno"]);
                if (ReworkDr != null)
                {
                    ReworkDr["REWORK"] = dr["count"];
                }
            }

            //repair 數據填充
            foreach (DataRow dr in RepairData.Rows)
            {
                DataRow RepairDr = WipData.Rows.Find(dr["skuno"]);
                if (RepairDr != null)
                {
                    RepairDr["Repair"] = dr["count"];
                }
            }

            //空白部分填充 0
            foreach (DataRow dr in WipData.Rows)
            {
                foreach (DataColumn dc in WipData.Columns)
                {
                    if (dr[dc.ColumnName] is DBNull)
                    {
                        dr[dc.ColumnName] = "0";
                    }
                }
            }

            //total 數據
            foreach (DataColumn dc in WipData.Columns)
            {
                if (!dc.ColumnName.Equals("SkuNo"))
                {
                    int total = 0;
                    foreach (DataRow dr in WipData.Rows)
                    {
                        total += Int32.Parse(dr[dc.ColumnName].ToString());
                    }
                    TotalRow[dc.ColumnName] = total;
                }
            }

            bool emptyRow = true;

            for (int i = WipData.Rows.Count - 1; i >= 0; i--)
            {
                emptyRow = true;
                foreach (DataColumn dc in WipData.Columns)
                {
                    if (dc.ColumnName != "SkuNo" && WipData.Rows[i][dc.ColumnName].ToString()!="0")
                    {
                        emptyRow = false;
                        break;
                    }
                }
                if (emptyRow)
                {
                    WipData.Rows.RemoveAt(i);
                }
            }     

            ReportTable table = new ReportTable();
            table.Tittle = "存在在線工單的機種 WIP 分佈表";
            TableRowView RowView = new TableRowView();
            TableColView ColView = null;
            foreach (DataColumn col in WipData.Columns)
            {
                table.ColNames.Add(col.ColumnName);
            }

            //填充主要數據部分
            foreach (DataRow dr in WipData.Rows)
            {
                TableRowView TempRowView = new TableRowView();
                foreach (DataColumn dc in WipData.Columns)
                {
                    ColView = new TableColView()
                    {
                        ColName=dc.ColumnName,
                        Value=dr[dc.ColumnName].ToString()
                    };
                    if (dc.ColumnName.Equals("SkuNo") && !dr[dc.ColumnName].Equals("Total"))
                    {
                        ColView.LinkType = "Link";
                        ColView.LinkData = linkURL + dr["skuno"];
                    }
                    
                    else if (!dc.ColumnName.Equals("SkuNo") && !dr[dc.ColumnName].Equals("Total") && !dr[dc.ColumnName].Equals("0"))
                    {
                        ColView.CellStyle = new Dictionary<string, object>() { { "background", "#98FB98" }, { "color", "#000" } };
                    }

                    TempRowView.RowData.Add(ColView.ColName, ColView);
                }
                table.Rows.Add(TempRowView);
            }

            Outputs.Add(table);

            #region 柱狀圖
            List<string> Categories = new List<string>();
            List<object> Data = new List<object>();

            foreach (DataColumn dc in WipData.Columns)
            {
                if (dc.ColumnName != "SkuNo")
                {
                    Categories.Add(dc.ColumnName);
                    DataRow dr = WipData.Rows.Find("Total");
                    Data.Add(new columnData() { name = dc.ColumnName, y = double.Parse(dr[dc.ColumnName].ToString()) });
                }
            }

            columnChart cChart = new columnChart();
            cChart.Tittle = "所有機種 WIP 分佈圖";
            cChart.ChartTitle = "主标题";
            cChart.ChartSubTitle = "副标题";
            XAxis _cXAxis = new XAxis();
            _cXAxis.Title = "工站";
            _cXAxis.XAxisType = XAxisType.BarChart;
            //不可以省略，表示X軸上面顯示的内容，如果沒有設定的話，就會變成 0，1，2，3，4 這些毫無意義的數據
            _cXAxis.Categories = Categories;

            cChart.XAxis = _cXAxis;
            cChart.Tooltip = "%";

            Yaxis _cYAxis = new Yaxis();
            _cYAxis.Title = "分佈數";
            cChart.YAxis = _cYAxis;

            ChartData cChartData = new ChartData();
            cChartData.name = "各工站 WIP 數量";
            cChartData.type = ChartType.column.ToString();
            cChartData.colorByPoint = true;
            cChartData.data = Data;
            List<ChartData> _cChartDatas = new List<ChartData> { cChartData };
            cChart.ChartDatas = _cChartDatas;
            Outputs.Add(cChart);
            #endregion
        }
    }
}
