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
    public class SkuWipReport:ReportBase
    {
        OleExec SFCDB = null;
        ReportInput SKUNO = new ReportInput() { InputType = "Autocomplete", Name = "SKUNO", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        public SkuWipReport()
        {
            this.Inputs.Add(SKUNO);
        }

        public string ChangeStationName(string OldStationName)
        {
            string NewStationName = OldStationName;
            if (OldStationName.Equals("STOCKIN"))
            {
                NewStationName = "S101";
            }
            else if (OldStationName.Contains("CBS"))
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
            string SkuNo = SKUNO.Value.ToString();
            SFCDB = DBPools["SFCDB"].Borrow();
            List<string> Wos = new List<string>();
            List<string> Stations = new List<string>();
            List<string> TempRoutes = new List<string>();
            string WolinkURL = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.WoWipReport&RunFlag=1&CloseFlag=0&WO=";
            string CountlinkURL = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.StationWipReport&RunFlag=1&WO=";

            List<R_WO_BASE> WoBases = SFCDB.ORM.Queryable<R_WO_BASE>().OrderBy(t=>t.RELEASE_DATE).Where(t => t.CLOSED_FLAG == "0" && t.SKUNO == SkuNo).ToList();
            foreach (R_WO_BASE wb in WoBases)
            {
                if(!Wos.Contains(wb.WORKORDERNO))
                {
                    Wos.Add(wb.WORKORDERNO);
                }
                if (!TempRoutes.Contains(wb.ROUTE_ID))
                {
                    TempRoutes.Add(wb.ROUTE_ID);
                    SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == wb.ROUTE_ID)
                        .OrderBy(t => t.SEQ_NO).ToList()
                        .ForEach(t => {
                            if (!Stations.Contains(t.STATION_NAME))
                                Stations.Add(t.STATION_NAME);
                        });
                }
            }

            //填充首行
            DataTable WipData = new DataTable();
            WipData.Columns.Add("WorkOrder");
            WipData.Columns.Add("ScheduleDate");
            foreach (string Station in Stations)
            {
                WipData.Columns.Add(ChangeStationName(Station));
            }
            WipData.Columns.Add("JOBFINISH");
            WipData.Columns.Add("Repair");
            WipData.Columns.Add("F106");
            WipData.Columns.Add("REWORK");
            WipData.Columns.Add("RMA");
            WipData.PrimaryKey = new DataColumn[] { WipData.Columns["WorkOrder"] };

            //填充首列
            foreach (string Wo in Wos)
            {
                DataRow dr = WipData.NewRow();
                dr["WorkOrder"] = Wo;
                WipData.Rows.Add(dr);
            }

            //所有工單 WIP 分佈圖
            DataTable WoData = SFCDB.ORM.Queryable<R_WO_BASE, R_SN>((rwb, rs) => rwb.WORKORDERNO == rs.WORKORDERNO)
                .Where((rwb, rs) => rwb.SKUNO == SkuNo && rwb.CLOSED_FLAG == "0" && rs.VALID_FLAG == "1"
                && rs.SHIPPED_FLAG != "1" && rs.REPAIR_FAILED_FLAG != "1" && rs.SCRAPED_FLAG != "1")
                .GroupBy((rwb,rs)=>rwb.WORKORDERNO).GroupBy((rwb,rs)=>rs.NEXT_STATION).GroupBy((rwb,rs)=>rwb.RELEASE_DATE)
                .Select("rwb.workorderno,rwb.release_date,next_station,count(rs.sn) as count").ToDataTable();

            DataTable ReworkData= SFCDB.ORM.Queryable<R_SN>().Where(rs => rs.VALID_FLAG == "1" && rs.SHIPPED_FLAG != "1" && rs.REPAIR_FAILED_FLAG != "1" && rs.SCRAPED_FLAG != "1" && rs.NEXT_STATION == "REWORK")
                .GroupBy(rs => rs.WORKORDERNO)
                .Select("workorderno,count(sn) as count").ToDataTable();
            //repair 數據

            //repair 數據
            DataTable RepairData = SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((rs, rwb) => rs.WORKORDERNO == rwb.WORKORDERNO)
                .Where((rs, rwb) => rwb.CLOSED_FLAG == "0" && rs.VALID_FLAG == "1" && rs.REPAIR_FAILED_FLAG == "1" && rwb.SKUNO==SkuNo)
                .GroupBy((rw, rwb) => rwb.WORKORDERNO).Select("rwb.workorderno,count(rs.sn) as count").ToDataTable();


            //所有機種數據填充
            foreach (DataRow dr in WoData.Rows)
            {
                DataRow WipDr = WipData.Rows.Find(dr["workorderno"]);
                if (dr["next_station"].ToString()!="REWORK" && WipData.Columns.Contains(ChangeStationName(dr["next_station"].ToString())))
                {
                    WipDr[ChangeStationName(dr["next_station"].ToString())] = dr["count"];
                }
                WipDr["ScheduleDate"] = dr["RELEASE_DATE"];
            }
            //Rework 數據填充
            foreach (DataRow dr in ReworkData.Rows)
            {
                DataRow ReworkDr = WipData.Rows.Find(dr["WorkOrderNo"]);
                if (ReworkDr == null)
                {
                    ReworkDr = WipData.NewRow();
                    ReworkDr["workorder"] = dr["workorderno"];
                    ReworkDr["ScheduleDate"] = "";
                    WipData.Rows.Add(ReworkDr);
                }
                ReworkDr["REWORK"] = dr["count"];
            }

            //repair 數據填充
            foreach (DataRow dr in RepairData.Rows)
            {
                DataRow RepairDr = WipData.Rows.Find(dr["WorkOrderNo"]);
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

            bool emptyRow = true;
            for (int i = WipData.Rows.Count - 1; i >= 0; i--)
            {
                emptyRow = true;
                foreach (DataColumn dc in WipData.Columns)
                {
                    if (dc.ColumnName != "WorkOrder" && dc.ColumnName!= "ScheduleDate" && WipData.Rows[i][dc.ColumnName].ToString() != "0")
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
            table.Tittle = SKUNO.Value + "所有在線工單 WIP 分佈";
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
                        ColName = dc.ColumnName,
                        Value = dr[dc.ColumnName].ToString()
                    };
                    if (dc.ColumnName.Equals("WorkOrder"))
                    {
                        ColView.LinkType = "Link";
                        ColView.LinkData = WolinkURL + dr["WorkOrder"];
                    }
                    else
                    {
                        if (!dc.ColumnName.Equals("ScheduleDate")&&ColView.Value != "0")
                        {
                            ColView.LinkType = "Link";
                            ColView.LinkData=CountlinkURL + dr["WorkOrder"] + "&StationName=" + dc.ColumnName+"&Skuno="+SkuNo;
                            ColView.CellStyle = new Dictionary<string, object>() { { "background", "#98FB98" }, { "color", "#000" } };
                        }
                    }
                    TempRowView.RowData.Add(ColView.ColName, ColView);
                }
                table.Rows.Add(TempRowView);
            }

            Outputs.Add(table);



            #region 折線圖

            List<string> Categories = new List<string>();
            List<ChartData> ChartDatas = new List<ChartData>();
            foreach (DataRow dr in WipData.Rows)
            {
                List<object> Data = new List<object>();
                ChartData ChartData = new ChartData();
                ChartData.name = dr["WorkOrder"].ToString();
                ChartData.type = ChartType.line.ToString();
                foreach (DataColumn dc in WipData.Columns)
                {
                    if (dc.ColumnName != "WorkOrder" && dc.ColumnName!= "ScheduleDate")
                    {
                        if (!Categories.Contains(dc.ColumnName))
                        {
                            Categories.Add(dc.ColumnName);
                        }
                        Data.Add(new columnData() { name = dc.ColumnName, y = double.Parse(dr[dc.ColumnName].ToString()) });
                    }
                }
                ChartData.data = Data;
                ChartDatas.Add(ChartData);
            }

            LineChart chart = new LineChart();
            chart.Tittle = "各工單 WIP 分佈折線圖";
            chart.ChartTitle = "主标题";
            chart.ChartSubTitle = "副标题";
            XAxis _XAxis = new XAxis();
            _XAxis.Title = "工站";
            _XAxis.XAxisType = XAxisType.BarChart;
            _XAxis.Categories = Categories;
            chart.XAxis = _XAxis;
            chart.Tooltip = "%";

            Yaxis _YAxis = new Yaxis();
            _YAxis.Title = "WIP 分佈數";
            chart.YAxis = _YAxis;
            PlotOptions PlotOptions = new PlotOptions();
            PlotOptions.type = PlotType.intdata;
            PlotOptions.pointInterval = 1;
            PlotOptions.pointStartIntdata = 0;
            //PlotOptions.pointStartDateTime = DateTime.Now;
            //PlotOptions.pointInterval = 3600000;
            chart.Plot = PlotOptions;

            List<ChartData> _ChartDatas = ChartDatas;
            chart.ChartDatas = _ChartDatas;
            Outputs.Add(chart);
            #endregion
        }
    }
}
