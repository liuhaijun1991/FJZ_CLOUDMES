using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport
{
    public class ReportChart:ReportOutputBase
    {
        public string ContainerID;
        public String OutputType
        {
            get
            {
                return "ReportChart";
            }
        }
        public string Tittle;
        public string ChartTitle;
        public string ChartSubTitle;
        //public XAxis XAxis;
        //public Yaxis YAxis;
        public List<ChartData> ChartDatas;

        //public void GetSample()
        //{
        //    this.Tittle = "SampleChart";
        //    this.ChartTitle = "主標題";
        //    this.ChartSubTitle = "副標題";
        //    XAxis _XAxis = new XAxis();
        //    _XAxis.Title = "X軸標題";
        //    _XAxis.XAxisType = XAxisType.datetime;
        //    this.XAxis = _XAxis;

        //    Yaxis _YAxis = new Yaxis();
        //    _YAxis.Title = "Y軸標題";
        //    this.YAxis = _YAxis;

        //    ChartData ChartData1 = new ChartData();
        //    ChartData1.name = "B32S1";
        //    ChartData1.type = ChartType.spline.ToString();
        //    //ChartData1.data =new List<object> { new List<string> { "64","2018-01-29 10:00:00"}, new List<string> { "78", "2018-01-29 11:00:00" }, new List<string> { "35", "2018-01-29 12:00:00" } };
        //    ChartData1.data = new List<object> { new List<int> { 64, 40 }, new List<int> { 78, 32 }, new List<int> { 35, 15 } };
        //    //ChartData ChartData2 = new ChartData();
        //    //ChartData2.name = "B32S2";
        //    //ChartData2.type = ChartType.column.ToString();
        //    //ChartData2.data = new List<object> { new List<object> { 95, "2018-01-29 15:00:00" }, new List<object> { 46, "2018-01-29 17:00:00" }, new List<object> { 135, "2018-01-29 18:00:00" } };

        //    List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
        //    this.ChartDatas = _ChartDatas;
        //}



    }

    public class LineChart : ReportChart
    {
        public string Type = "lineChart";
        public XAxis XAxis;
        public Yaxis YAxis;
        public PlotOptions Plot;
        /// <summary>
        ///  鼠标移到线上显示的单位;
        /// </summary>
        public string Tooltip;
        #region LineSample_1:给定开始时间和时间间隔;
        /// <summary>
        /// 折線圖-给定开始时间和时间间隔;
        /// </summary>
        public void GetSample1()
        {
            this.Tittle = "SampleChart_折線圖";
            this.ChartTitle = "主标题";
            this.ChartSubTitle = "副标题";
            XAxis _XAxis = new XAxis();
            _XAxis.Title = "X轴标题";
            _XAxis.XAxisType = XAxisType.datetime;
            this.XAxis = _XAxis;
            this.Tooltip = "%";

            Yaxis _YAxis = new Yaxis();
            _YAxis.Title = "Y轴标题";
            this.YAxis = _YAxis;

            ChartData ChartData1 = new ChartData();
            ChartData1.name = "B32S1";
            ChartData1.type = ChartType.line.ToString();

            PlotOptions PlotOptions = new PlotOptions();
            PlotOptions.type = PlotType.datetime;
            PlotOptions.pointStartDateTime = DateTime.Now;
            PlotOptions.pointInterval = 3600000;
            this.Plot = PlotOptions; ChartData1.data = new List<object> { 96, 88, 179, 139, 254, 34, 101, 459, 179, 341, 578 };

            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            this.ChartDatas = _ChartDatas;
        }
        #endregion

        #region LineSample_2:时间在键值对里;如["2018-01-30 22:23:45",65.3]
        /// <summary>
        /// 曲線圖-时间在键值对里
        /// </summary>
        public void GetSample2()
        {
            this.Tittle = "SampleChart_曲線圖";
            this.ChartTitle = "主标题";
            this.ChartSubTitle = "副标题";
            XAxis _XAxis = new XAxis();
            _XAxis.Title = "X轴标题";
            _XAxis.XAxisType = XAxisType.datetime;
            this.XAxis = _XAxis;
            this.Tooltip = "%";

            Yaxis _YAxis = new Yaxis();
            _YAxis.Title = "Y轴标题";
            this.YAxis = _YAxis;

            ChartData ChartData1 = new ChartData();
            ChartData1.name = "B32S1";
            ChartData1.type = ChartType.spline.ToString();

            PlotOptions PlotOptions = new PlotOptions();
            PlotOptions.type = PlotType.datetime;
            this.Plot = PlotOptions;ChartData1.data = new List<object> { new List<object> { "2018-01-29 10:00:00",64 }, new List<object> {  "2018-01-29 11:00:00",78 }, new List<object> { "2018-01-29 12:00:00",35 }, new List<object> { "2018-01-29 14:00:00", 235 }, new List<object> { "2018-01-29 15:00:00", 135 }, new List<object> { "2018-01-29 16:00:00", 85 }, new List<object> { "2018-01-29 17:00:00", 56 }, new List<object> { "2018-01-29 18:00:00", 15 }, new List<object> { "2018-01-29 19:00:00", 133 } };

            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            this.ChartDatas = _ChartDatas;
        }
        #endregion

        #region LineSample_3:时间在键值对里;如["2018-01-30 22:23:45",65.3]
        /// <summary>
        /// 面積圖-时间在键值对里
        /// </summary>
        public void GetSample3()
        {
            this.Tittle = "SampleChart_面積圖";
            this.ChartTitle = "主标题";
            this.ChartSubTitle = "副标题";
            XAxis _XAxis = new XAxis();
            _XAxis.Title = "X轴标题";
            _XAxis.XAxisType = XAxisType.datetime;
            this.XAxis = _XAxis;
            this.Tooltip = "%";

            Yaxis _YAxis = new Yaxis();
            _YAxis.Title = "Y轴标题";
            this.YAxis = _YAxis;

            ChartData ChartData1 = new ChartData();
            ChartData1.name = "B32S1";
            ChartData1.type = ChartType.area.ToString();

            PlotOptions PlotOptions = new PlotOptions();
            PlotOptions.type = PlotType.datetime;
            this.Plot = PlotOptions; ChartData1.data = new List<object> { new List<object> { "2018-01-29 08:00:00", 24 }, new List<object> { "2018-01-29 09:00:00", 384 }, new List<object> { "2018-01-29 10:00:00", 64 }, new List<object> { "2018-01-29 11:00:00", 78 }, new List<object> { "2018-01-29 12:00:00", 35 }, new List<object> { "2018-01-29 14:00:00", 235 }, new List<object> { "2018-01-29 15:00:00", 135 }, new List<object> { "2018-01-29 16:00:00", 85 }, new List<object> { "2018-01-29 17:00:00", 56 }, new List<object> { "2018-01-29 18:00:00", 15 }, new List<object> { "2018-01-29 19:00:00", 133 } };

            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            this.ChartDatas = _ChartDatas;
        }
        #endregion
    }

    public class pieChart:ReportChart
    {
        public string Type = "pieChart";
        #region 饼图Sample
        public void GetSample()
        {
            this.Tittle = "SampleChart_餅狀圖";
            this.ChartTitle = "主標題";
            this.ChartSubTitle = "副標題";

            ChartData ChartData1 = new ChartData();
            ChartData1.name = "B32S1";
            ChartData1.type = ChartType.pie.ToString();
            ChartData1.data = new List<object> { new List<object>{ "MRB", 6.3 }, new List<object> { "SMTLOADING", 48.7 }, new List<object> { "AOI2", 45.0 }, new List<object> { "BIP", 15.0 }, new List<object> { "VI", 13.0 } };
            ChartData1.colorByPoint = true;
            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            this.ChartDatas = _ChartDatas;
        }
        #endregion
    }

    public class columnChart: ReportChart
    {
        public string Type = "columnChart";
        public XAxis XAxis;
        public Yaxis YAxis;
        public string Tooltip;
        #region LineSample_1:给定开始时间和时间间隔;
        /// <summary>
        /// 折線圖-给定开始时间和时间间隔;
        /// </summary>
        public void GetSample1()
        {
            this.Tittle = "SampleChart_柱狀圖";
            this.ChartTitle = "主标题";
            this.ChartSubTitle = "副标题";
            XAxis _XAxis = new XAxis();
            _XAxis.Title = "X轴标题";
            _XAxis.XAxisType = XAxisType.datetime;
            this.XAxis = _XAxis;
            this.Tooltip = "%";

            Yaxis _YAxis = new Yaxis();
            _YAxis.Title = "Y轴标题";
            this.YAxis = _YAxis;

            ChartData ChartData1 = new ChartData();
            ChartData1.name = "HWD 各線別產出";
            ChartData1.type = ChartType.column.ToString();
            ChartData1.colorByPoint = true;
            ChartData1.data = new List<object> {  new columnData() { name = "B32S1",y=132 }, new columnData() { name = "B32S2", y = 245 }, new columnData() { name = "B32S3", y = 45 }, new columnData() { name = "B32S4", y = 315 }, new columnData() { name = "B32S5", y = 35 }, new columnData() { name = "B32S6", y = 183 }  };

            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            this.ChartDatas = _ChartDatas;
        }
        #endregion 

    }

    public class columnData
    {
        public string name;
        public double y;
    }

    public class PlotOptions
    {
        /// <summary>
        /// 时间间隔(ms)
        /// </summary>
        public int pointInterval;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime pointStartDateTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public int pointStartIntdata;
        /// <summary>
        /// Type为data
        /// </summary>
        public PlotType type= PlotType.intdata;
    }

    public enum PlotType
    {
        datetime,intdata
    }
    public class XAxis
    {
        /// <summary>
        /// X軸標題
        /// </summary>
        public string Title;
        /// <summary>
        /// x軸類型
        /// </summary>
        public XAxisType XAxisType;
        /// <summary>
        /// 當XAxisType=BarChart =>categories= ['苹果', '香蕉', '橙子']
        /// XAxisType=initValue =>categories= 63       
        /// </summary>
        public object Categories;
    }

    public class Yaxis
    {
        public string Title;
    }

    /// <summary>
    /// ChartType 圖標類型
    /// seriesName 該組數據名稱,用于顯示圖例
    /// Data -當XAxis.XAxisType in (initValue,BarChart)時為一維數組;反之為二維數組;
    /// </summary>
    public class ChartData
    {
        public string type = ChartType.spline.ToString();
        public string name;
        public List<object> data;
        /// <summary>
        /// 圖形是否自動變色
        /// </summary>
        public bool colorByPoint;
        public string size;
        public string innerSize;
    }

    /// <summary>
    /// ['bar',條狀圖],['column',柱狀圖],['pie',餅狀圖],['area',面積圖],['line',折線圖],['spline',曲線圖],
    /// </summary>
    public enum ChartType
    {
        bar,column,pie,area,line,spline
        //bar=0,
        //column=1,
        //pie =2,
        //area =3,
        //line =4,
        //spline =5
    }

    /// <summary>
    /// datetime:時間類型
    /// initValue:初始值:給定初始值,X軸STEP加1;
    /// XY:鍵值類型,X軸即為給定數據
    /// BarChart:X軸分類,例如['蘋果','香蕉','橙子']
    /// </summary>
    public enum XAxisType
    {
        datetime = 0,
        initValue =1,
        XY =2,
        BarChart =3
    }
    public class EChartBase
    {
        /// <summary>
        /// 顯示頁面上的區域ID
        /// </summary>
        public string ContainerID = null;
        public string Lib = "EChart";
        public object title;
        public object tooltip;
        public object toolbox;
        public object legend;
        public object grid;
        public object calculable;
        public object xAxis;
        public object yAxis;
        public object Options;
        public List<object> series = new List<object>();
    }

    public class Bar_Chart_Data
    {
        public string name = "";
        public string type = "bar";
        public List<object> data = new List<object>();
        public int yAxisIndex = 0;
        public string stack = null;
        public object itemStyle;
        public object label;

    }

    /// <summary>
    /// 指針式儀表數據
    /// </summary>
    public class GAUGE_Chart_Data
    {
        public string Lib = "EChart";
        public string name = "轉速";
        public string type = "gauge";
        public List<string> center = new List<string>() { "25%", "55%" };  // 默?全局居中
        public string radius = "50%";
        public int min = 0;
        public int max = 7;
        public int endAngle = 0;
        public int splitNumber = 7;
        //public object axisLine = new {            // 坐???
        //    lineStyle = new {       // ?性lineStyle控制???式
        //        width = 8
        //    }
        //};
        public object axisLine = new
        {            // 坐???
            lineStyle = new
            {       // ?性lineStyle控制???式
                width = 10,
                color = new List<object>()
                        { new List<object>() { 0.2, "#000" }, new List<object>() { 0.8, "#63869e" }, new List<object>() { 1, "#c23531" } }
            }
        };
        public object axisTick = new {            // 坐??小??
            length = 12,        // ?性length控制??
            lineStyle = new {       // ?性lineStyle控制???式
                color = "auto"
            }
        };
        public object splitLine = new {           // 分隔?
                    length =20,         // ?性length控制??
                    lineStyle= new {       // ?性lineStyle（??lineStyle）控制???式
                        color= "auto"
                    }
                };
        public object pointer = new {
            width = 5
        };
        public object title = new {
            offsetCenter = new List<string>(){ "0", "-30%" }, // x, y，?位px
        };
        public object detail = new {
            textStyle = new {       // 其余?性默?使用全局文本?式，??TEXTSTYLE
                fontWeight = "bolder"
            }
        };
        public object data = new List<object>() { new { value = 1.5, name = "x1000 r/min" } };
    }
    
}
