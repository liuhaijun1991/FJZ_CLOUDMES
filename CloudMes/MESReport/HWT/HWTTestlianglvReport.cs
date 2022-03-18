using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;

namespace MESReport.HWT
{
    public class HWTTestlianglvReport : ReportBase
    {
        ReportInput Station = new ReportInput()
        {
            Name = "Station",
            InputType = "Select",
            SendChangeEvent = false,
            ValueForUse = new string[] { "ICT", "FT2_0", "FT2_ST" },
            Value = "ICT"

        };
        public HWTTestlianglvReport()
        {
            
            this.Inputs.Add(Station);

        }
        public override void Run()
        {
            OleExec db = new OleExec("Data Source = 10.120.232.130:1527 / HWODB; User ID = HWUSER; Password = HWTEST");
            string strSql = $@"select UPHDATE,HOURPERIOD,sum(INPUTUNIT) as sum_input,sum(OUTPUTUNIT) as sum_output,
trunc(sum(OUTPUTUNIT)/sum(INPUTUNIT),2) ICT_FPY from sfcuphratedetail where EVENTPOINT='ICT' and LASTEDITDT>sysdate-10/24 
group by UPHDATE,HOURPERIOD order by 1,2";
            var data1 = db.RunSelect(strSql);
            strSql = $@"select PRODUCTIONLINE,sum(sum_input) sum_input,sum(sum_output) sum_output,trunc(sum(sum_output)/sum(sum_input),4) ICT_FPY from 
(select HOURPERIOD,PRODUCTIONLINE,sum(INPUTUNIT) as sum_input,sum(OUTPUTUNIT) as sum_output,
trunc(sum(OUTPUTUNIT)/sum(INPUTUNIT),2) ICT_FPY from sfcuphratedetail where EVENTPOINT='ICT' and LASTEDITDT>sysdate-1/24 
group by HOURPERIOD,PRODUCTIONLINE ) group by PRODUCTIONLINE order by 1";
            var data2 = db.RunSelect(strSql);
            strSql = $@"select UPHDATE,sum(INPUTUNIT) as sum_input,sum(OUTPUTUNIT) as PASSQTY, sum(INPUTUNIT) - sum(OUTPUTUNIT) as FAILQTY,
trunc(sum(OUTPUTUNIT)/sum(INPUTUNIT),2) ICT_FPY from sfcuphratedetail where EVENTPOINT='ICT' and LASTEDITDT>sysdate-31
group by UPHDATE order by 1";
            var data3 = db.RunSelect(strSql);
            strSql = $@"select a.FAILCODE,count(*) as Fail_QTY from sfcrepairfailcode a,sfcrepairmain b where a.sysserialno=b.sysserialno and 
b.FAILUREEVENTPOINT='ICT'
and b.REPAIRDATE=to_char(sysdate,'yyyy-mm-dd') and a.FAILCATEGORY='DEFECT' group by a.FAILCODE order by 2 desc";
            var data4 = db.RunSelect(strSql);

            ReportTable retTab = new ReportTable();
            retTab.LoadData(data1.Tables[0], null);
            retTab.Tittle = $@"過去10個小時每個小時的ICT良率";
            //retTab.Tittle = "A";
            //Outputs.Add(retTab);

            EChartBase Chart1 = new EChartBase();
            Chart1.title = new
            {
                text = $@"過去10個小時每個小時的ICT良率"
            };
            for (int i = 0; i < data1.Tables[0].Rows.Count; i++)
            {
                GAUGE_Chart_Data C = new GAUGE_Chart_Data();
                C.min = 90;
                C.max = 100;
                C.splitNumber = 1;
                C.endAngle = -45;
                C.radius = "80%";
                C.name = data1.Tables[0].Rows[i]["HOURPERIOD"].ToString();
                C.axisLine = new
                {            // 坐???
                    lineStyle = new
                    {       // ?性lineStyle控制???式
                        width = 4,
                        color = new List<object>()
                        {  new List<object>() { 0.5, "#FF0000"  }, new List<object>() { 1, "#00FF00" } }
                    }
                };
                C.detail = new
                {
                    fontSize = 18,
                    color = $@"#fff",
                    offsetCenter = new List<string> { "0", "70%" },
                    formatter = new
                    {
                        formatter = "{value}%"
                    }
                };
                int j = i + 1;
                C.center = new List<string>() { $@"{i * 10 + 5}%", $@"60%" };

                float Value = float.Parse(data1.Tables[0].Rows[i]["ICT_FPY"].ToString());
                C.data = new List<object>() { new { value = Value * 100 , name = C.name } };
                Chart1.series.Add(C);
            }
            Chart1.ContainerID = "TopZone";
            Outputs.Add(Chart1);

           
            List<string> lines = new List<string>();
            Bar_Chart_Data bardata1 = new Bar_Chart_Data();
            bardata1.label = new
            {
                show = true,
                position = new List<string> { "100%", "50%" },
                distance = 8,
                color = "#fff",
                fontSize = ""
            };
            
            for (int i = 0; i < data2.Tables[0].Rows.Count; i++)
            {
                lines.Add(data2.Tables[0].Rows[i]["PRODUCTIONLINE"].ToString());
                var val = double.Parse(data2.Tables[0].Rows[i]["ICT_FPY"].ToString());
                var c = "#f00";
                if (val > 0.95)
                {
                    c = "#0f0";
                }
                object v = new { value = val * 100, itemStyle=new { color = c } ,label= new { color = c,
                    formatter = $@"{val * 100}%"
                } };
                bardata1.data.Add(v);
            }

            EChartBase Chart2 = new EChartBase();
            Chart2.title = new
            {
                text = $@"當前1個小時內ICT每個機台_線體的良率",
                color = $@"#fff",
                fontSize = $@"12"
            };
            Chart2.xAxis = new {
                type = "value",
                formatter = new
                {
                    formatter = "{value}%"
                },
                axisLabel=new
                {
                    formatter = "{value}%"
                }
        };
            Chart2.yAxis = new
            {
                type = "category",
                data = lines,
                
            };
            Chart2.series.Add(bardata1);
            Chart2.ContainerID = "LeftZone";
            Outputs.Add(Chart2);

            retTab = new ReportTable();
            retTab.LoadData(data2.Tables[0], null);
            retTab.Tittle = $@"當前1個小時內ICT每個機台_線體的良率";
            //retTab.Tittle = "B";
            // Outputs.Add(retTab);

            //-----------------------------------------------------------------------------------------
            EChartBase Chart3 = new EChartBase();
            Chart3.title = new
            {
                text = $@"過去31天的ICT良率"
            };

            List<object> yAxis = new List<object>();
            yAxis.Add(new
            {
                type = "value",
                name = "數量",


            });
            yAxis.Add(new
            {
                type = "value",
                name = "良率",
                axisLabel = new
                {
                    formatter = "{value} %"
                }
            });
            Chart3.legend = new
            {
                data = new List<string>() { "PASS", "FAIL" , "良率" }
            };
            Chart3.yAxis = yAxis;
            //Chart3.xAxis = new { type = "value" };
            List<object> xAxis = new List<object>();
            List<string> XBar = new List<string>();
            xAxis.Add(new
            {
                type = "category",
                data = XBar,
                axisPointer = new
                {
                    type = "shadow"
                }

            });
            Chart3.xAxis = xAxis;
            Bar_Chart_Data PassData = new Bar_Chart_Data();
            Bar_Chart_Data FailData = new Bar_Chart_Data();
            Bar_Chart_Data RData = new Bar_Chart_Data();

            PassData.stack = "Total";
            FailData.stack = "Total";
            PassData.itemStyle = new
            {
                color = "#0f0"
            };
            FailData.itemStyle = new
            {
                color = "#f00"
            };
            PassData.name = "PASS";
            FailData.name = "FAIL";
            RData.name = "良率";
            RData.yAxisIndex = 1;
            RData.type = "line";
            for (int i = 0; i < data3.Tables[0].Rows.Count; i++)
            {
                DataRow dr = data3.Tables[0].Rows[i];
                XBar.Add(dr["UPHDATE"].ToString());
                PassData.data.Add(int.Parse(dr["PASSQTY"].ToString()));
                FailData.data.Add(int.Parse(dr["FAILQTY"].ToString()));
                RData.data.Add(double.Parse(dr["ICT_FPY"].ToString()) * 100);
            }
            Chart3.series.Add(PassData);
            Chart3.series.Add(FailData);
            Chart3.series.Add(RData);

            Chart3.ContainerID = "RightBottom";
            Outputs.Add(Chart3);

            //----------------------------------------------------------------------------------------


            retTab = new ReportTable();
            retTab.LoadData(data3.Tables[0], null);
            retTab.Tittle = $@"按天顯示過去31天的ICT良率";
            //retTab.Tittle = "C";
            //Outputs.Add(retTab);

            List<string> lines2 = new List<string>();
            Bar_Chart_Data bardata2 = new Bar_Chart_Data();
            for (int i = 0; i < data4.Tables[0].Rows.Count; i++)
            {
                lines2.Add(data4.Tables[0].Rows[i]["FAILCODE"].ToString());
                double v = double.Parse(data4.Tables[0].Rows[i]["Fail_QTY"].ToString());
                bardata2.data.Add(v);
            }

            EChartBase Chart4 = new EChartBase();
            Chart4.title = new
            {
                text = $@"按不良代碼顯示ICT當天的維修數據"
            };
            Chart4.yAxis = new { type = "value" };
            Chart4.xAxis = new
            {
                type = "category",
                data = lines2,
                
            };
            Chart4.series.Add(bardata2);
            Chart4.ContainerID = "RightTop";
            Outputs.Add(Chart4);

            retTab = new ReportTable();
            retTab.LoadData(data4.Tables[0], null);
            retTab.Tittle = $@"按不良代碼顯示ICT當天的維修數據";
            //retTab.Tittle = "D";
            //Outputs.Add(retTab);
        }
    }
}
