using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using System.Data;

namespace MESReport.BaseReport
{
    public class ZGJ_WOReport:ReportBase
    {
        ReportInput wo = new ReportInput { Name = "輸入工單：", InputType = "TXT", Enable = true, EnterSubmit = true, Value = "002331000015", SendChangeEvent=false, ValueForUse=null };
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        public ZGJ_WOReport()
        {
            Inputs.Add(wo);
            Inputs.Add(CloseFlag);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            if (wo.Value == null || wo.Value.ToString().Equals(""))
            {
                throw new Exception("WO Can not be null");
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();

            try
            {

                //string workorderno = wo.Value.ToString();
                //string ClosedFlag = CloseFlag.Value.ToString();

                T_R_SN t = new T_R_SN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                //DataTable dt = t.GetSNByWo(workorderno,sfcdb);


                //ReportTable retTab = new ReportTable();
                //retTab.LoadData(dt, null);
                //retTab.Tittle = "ASSY Manufacture Report";
                //Outputs.Add(retTab);

                #region 折綫圖
                LineChart lChart = new LineChart();
                lChart.GetSample1();
                //lChart.Tittle = "SampleChart_折線圖";
                //lChart.ChartTitle = "主标题";
                //lChart.ChartSubTitle = "副标题";
                //XAxis _XAxis = new XAxis();
                //_XAxis.Title = "X轴标题";
                //_XAxis.XAxisType = XAxisType.datetime;
                //lChart.XAxis = _XAxis;
                //lChart.Tooltip = "%";

                //Yaxis _YAxis = new Yaxis();
                //_YAxis.Title = "Y轴标题";
                //lChart.YAxis = _YAxis;

                //ChartData lChartData = new ChartData();
                //lChartData.name = "B32S1";
                //lChartData.type = ChartType.line.ToString();

                //PlotOptions PlotOptions = new PlotOptions();
                //PlotOptions.type = PlotType.datetime;
                ////PlotOptions.pointStartDateTime = DateTime.Parse("2018-08-20 12:00:00");
                ////PlotOptions.pointInterval = 3600000;
                //lChart.Plot = PlotOptions;
                //lChartData.data = new List<object> { new List<object>{ "2018-08-20 12:00:00" ,new string[] { "11","22","33"} },
                //new List<object>{ "2018-08-20 13:00:00" ,new string[] { "111","222","333"} }} ;

                //List<ChartData> lChartDatas = new List<ChartData> { lChartData };
                //lChart.ChartDatas = lChartDatas;
                Outputs.Add(lChart);
                #endregion

                #region 曲綫圖
                LineChart spLineChart = new LineChart();
                spLineChart.Tittle = "SampleChart_曲線圖";
                spLineChart.ChartTitle = "主标题";
                spLineChart.ChartSubTitle = "副标题";
                XAxis _spXAxis = new XAxis();
                _spXAxis.Title = "X轴标题";
                _spXAxis.Categories = new string[] { "2018-10-22 10:00:00", "2018-10-22 11:00:00", "2018-10-22 12:00:00", "2018-10-22 13:00:00", "2018-10-22 14:00:00", "2018-10-22 15:00:00", "2018-10-22 16:00:00" };
                _spXAxis.XAxisType = XAxisType.datetime;
                spLineChart.XAxis = _spXAxis;
                //spLineChart.Tooltip = "%";

                Yaxis _spYAxis = new Yaxis();
                _spYAxis.Title = "Y轴标题";
                spLineChart.YAxis = _spYAxis;

                ChartData spChartData = new ChartData();
                spChartData.name = "B32S1";
                spChartData.type = ChartType.spline.ToString();

                PlotOptions spPlotOptions = new PlotOptions();
                spPlotOptions.type = PlotType.datetime;
                spPlotOptions.pointStartDateTime = DateTime.Now;
                //spPlotOptions.pointInterval = 60 * 60 * 1000;
                spLineChart.Plot = spPlotOptions;
                //日期會顯示在當鼠標經過圖標的時候
                //spChartData.data = new List<object> { new List<object> { "2018-05-29 10:00:00", 64 }, new List<object> { "2018-05-29 11:00:00", 78 }, new List<object> { "2018-05-29 12:00:00", 35 }, new List<object> { "2018-05-29 14:00:00", 235 }, new List<object> { "2018-05-29 15:00:00", 135 }, new List<object> { "2018-05-29 16:00:00", 85 }, new List<object> { "2018-05-29 17:00:00", 56 }, new List<object> { "2018-05-29 18:00:00", 15 }, new List<object> { "2018-05-29 19:00:00", 133 } };
                //spChartData.data = new List<object> { new List<object> { DateTime.Parse("2018-05-29 10:00:00"),64 } , new List<object> { DateTime.Parse("2018-05-29 11:00:00"), 78 } , new List<object> { DateTime.Parse("2018-05-29 12:00:00"), 35 } , new List<object> { DateTime.Parse("2018-05-29 14:00:00"), 235 } , new List<object> { DateTime.Parse("2018-05-29 15:00:00"), 135 } , new List<object> { DateTime.Parse("2018-05-29 16:00:00"), 85 } , new List<object> { DateTime.Parse("2018-05-29 17:00:00"), 56 } , new List<object> { DateTime.Parse("2018-05-29 18:00:00"), 15 } };
                spChartData.data = new List<object> { 64, 78, 35, 235, 29, 69, 75, 168 };
                List<ChartData> _spChartDatas = new List<ChartData> { spChartData };
                spLineChart.ChartDatas = _spChartDatas;
                Outputs.Add(spLineChart);
                #endregion

                #region 面積圖
                LineChart aLineChart = new LineChart();
                aLineChart.Tittle = "SampleChart_面積圖";
                aLineChart.ChartTitle = "主标题";
                aLineChart.ChartSubTitle = "副标题";
                XAxis _aXAxis = new XAxis();
                _aXAxis.Title = "2018/08/20 降雨分佈";
                _aXAxis.XAxisType = XAxisType.BarChart;
                //_aXAxis.Categories = new string[] { "12","13","14","15","16","17","18" };
                aLineChart.XAxis = _aXAxis;
                aLineChart.Tooltip = "%";

                Yaxis _aYAxis = new Yaxis();
                _aYAxis.Title = "降雨量";
                aLineChart.YAxis = _aYAxis;

                ChartData aChartData = new ChartData();
                aChartData.name = "降雨圖";
                aChartData.type = ChartType.area.ToString();

                PlotOptions aPlotOptions = new PlotOptions();
                aPlotOptions.type = PlotType.intdata;
                aPlotOptions.pointStartIntdata = 18;
                aPlotOptions.pointInterval = 2;
                aLineChart.Plot = aPlotOptions;
                aChartData.data = new List<object> { 198, 384, 64, 78, 35, 235, 135 };

                List<ChartData> _aChartDatas = new List<ChartData> { aChartData };
                aLineChart.ChartDatas = _aChartDatas;
                Outputs.Add(aLineChart);
                #endregion

                #region 餅狀圖
                pieChart pChart = new pieChart();
                pChart.Tittle = "餅狀圖測試 ZGJ";
                pChart.ChartTitle = "餅狀圖主標題";
                pChart.ChartSubTitle = "餅狀圖副標題";
                ChartData pChartData = new ChartData();
                pChartData.name = "餅狀圖數據";
                pChartData.type = ChartType.pie.ToString();
                pChartData.data = t.GetPieChartTestData(sfcdb);
                pChartData.colorByPoint = true;
                List<ChartData> pChartDatas = new List<ChartData> { pChartData };
                pChart.ChartDatas = pChartDatas;
                Outputs.Add(pChart);
                #endregion

                #region 柱狀圖
                columnChart cChart = new columnChart();
                cChart.Tittle = "SampleChart_柱狀圖";
                cChart.ChartTitle = "主标题";
                cChart.ChartSubTitle = "副标题";
                XAxis _cXAxis = new XAxis();
                _cXAxis.Title = "X轴标题";
                _cXAxis.XAxisType = XAxisType.BarChart;
                //不可以省略，表示X軸上面顯示的内容，如果沒有設定的話，就會變成 0，1，2，3，4 這些毫無意義的數據
                _cXAxis.Categories = new string[] { "苹果", "橘子", "梨", "葡萄", "香蕉" };

                cChart.XAxis = _cXAxis;
                cChart.Tooltip = "%";

                Yaxis _cYAxis = new Yaxis();
                _cYAxis.Title = "Y轴标题";
                cChart.YAxis = _cYAxis;

                ChartData cChartData = new ChartData();
                cChartData.name = "HWD 各線別產出";
                cChartData.type = ChartType.column.ToString();
                cChartData.colorByPoint = true;
                cChartData.data = new List<object> { new columnData() { name="苹果",y = 10 },
                new columnData() { name="橘子",y = 2 },
                new columnData() { name="梨",y = -3 },
                new columnData() { name="葡萄",y = 4 },
                new columnData() {  name="香蕉",y=0.0} };
                List<ChartData> _cChartDatas = new List<ChartData> { cChartData };
                cChart.ChartDatas = _cChartDatas;
                Outputs.Add(cChart);
                #endregion


                #region 指針式儀表圖
                //EChartBase Chart1 = new EChartBase();
                //Chart1.title = new { text = "測試指針式儀表數據" };
                //for (int i = 0; i < 5; i++)
                //{
                //    GAUGE_Chart_Data GCD = new GAUGE_Chart_Data();
                //    GCD.min = 80;
                //    GCD.max = 100;
                //    GCD.splitNumber = 2;
                //    GCD.endAngle = -45;
                //    GCD.radius = "50%";
                //    GCD.name = "第" + i + "小時";
                //    GCD.axisLine = new
                //    {
                //        lineStyle = new
                //        {
                //            width = 8,
                //            color = new List<Object>()
                //            { new List<object>() { 0.5, "#FF0000"  }, new List<object>() { 1, "#00FF00" }}
                //        }
                //    };
                //    int j = i + 1;
                //    GCD.center = new List<string>() { $@"{j * 10 + 5}%", $@"50%" };
                //    GCD.data = new List<object> { new { value = 80 + i * 5, name = GCD.name } };
                //    Chart1.series.Add(GCD);
                //}

                //Chart1.Zone_ID = "TopZone";
                //Outputs.Add(Chart1);
                #endregion

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (sfcdb != null) DBPools["SFCDB"].Return(sfcdb);
            }

        }
    }
}
