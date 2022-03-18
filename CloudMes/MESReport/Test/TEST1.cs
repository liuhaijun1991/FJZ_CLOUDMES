using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.Test
{
    public class TEST1 : ReportBase
    {
         ReportInput WO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "002520000001", Enable = true, SendChangeEvent = false, ValueForUse = null };
         ReportInput Station = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] {"ALL", "AOI1","AOI2","VI1","VI2" }  };
         ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
         ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public TEST1()
        {
            Inputs.Add(WO);
            Inputs.Add(Station);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);

            string strGetWoSN = @"select * from r_sn where workorderno = '{0}' and rownum < 30 ";
            Sqls.Add("strGetWoSN", strGetWoSN);
        }

        public override void Init()
        {
            StartTime.Value = DateTime.Now.AddDays(-1);
            EndTime.Value = DateTime.Now;

        }

        public override void Run()
        {
            if (WO.Value == null)
            {
                throw new Exception("WO Can not be null");
            }
            string runSql = string.Format(Sqls["strGetWoSN"], WO.Value.ToString());
            RunSqls.Add(runSql);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet res = SFCDB.RunSelect(runSql);
                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);

                retTab.Tittle = "SN List";

                retTab.ColNames.RemoveAt(0);


                // piechart
                pieChart retChart_pie = new pieChart();
                retChart_pie.GetSample();
                Outputs.Add(retChart_pie);
                //linechart
                LineChart retChart_line = new LineChart();
                retChart_line.GetSample1();
                LineChart retChart_spline = new LineChart();
                retChart_spline.GetSample2();
                LineChart retChart_area = new LineChart();
                retChart_area.GetSample3();
                //columnChart
                columnChart retChart_column = new columnChart();
                retChart_column.GetSample1();

                Outputs.Add(retChart_column);
                Outputs.Add(retChart_line);
                Outputs.Add(retChart_spline);
                Outputs.Add(retChart_area);
                Outputs.Add(retTab);
               
                DBPools["SFCDB"].Return(SFCDB);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }


    }
}
