using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="HWDLineFailReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// HWDLineFailReport
    /// </summary>
    public class HWDLineFailReport : ReportBase
    {
        //ReportInput inputLine = new ReportInput() { Name = "線別", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputStation = new ReportInput() { Name = "工位", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputSkuname = new ReportInput() { Name = "機種名", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputSku = new ReportInput() { Name = "料號", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportInput inputLine = new ReportInput() { Name = "LineType", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "WorkStation", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuname = new ReportInput() { Name = "SkunoName", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSku = new ReportInput() { Name = "ItemNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };



        public HWDLineFailReport()
        {
            Inputs.Add(inputLine);
            Inputs.Add(inputStation);
            Inputs.Add(inputSkuname);
            Inputs.Add(inputSku);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
        }

        public override void Init()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                startTime.Value = DateTime.Now.AddDays(-1);
                endTime.Value = DateTime.Now;
                List<string> lineList = new List<string>();
                lineList.Add("ALL");
                List<string> stationList = new List<string>();
                stationList.Add("ALL");
                string sql = "select distinct value from r_function_control where functionname='PanelFailStation' and controlflag = 'Y' and functiontype = 'NOSYSTEM'";
                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    stationList.Add(dr["VALUE"].ToString());
                }
                inputStation.ValueForUse = stationList;

                sql = "select distinct line_name from c_line";
                dt = SFCDB.RunSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    lineList.Add(dr["LINE_NAME"].ToString());
                }
                inputLine.ValueForUse = lineList;

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
          

        }

        public object GetChartDataSourse(string BTime,string ETime,DataTable dt)
        {
            columnChart retChart_column = new columnChart();
            retChart_column.Tittle = "HWDLineFailReport";
            //retChart_column.ChartTitle = "HWD"+ BTime +"-"+ ETime + "生產投入統計圖";
            retChart_column.ChartTitle = "HWD" + BTime + "-" + ETime + "Productin input statistical circle";

            // retChart_column.ChartSubTitle = "線別/機種產出趨勢圖";
            retChart_column.ChartSubTitle = "LineType/Skuno output trend circle";
            XAxis _XAxis = new XAxis();
            // _XAxis.Title = "機種";
            _XAxis.Title = "SKuno";
            //_XAxis.Categories = new List<string> { "B32S1","B32S2","B32S3","B32S4"};
            //_XAxis.XAxisType = XAxisType.datetime;
            retChart_column.XAxis = _XAxis;
            retChart_column.Tooltip = "Pic";

            Yaxis _YAxis = new Yaxis();
            // _YAxis.Title = "投入數";
            _YAxis.Title = "Number of input";
            retChart_column.YAxis = _YAxis;

            ChartData ChartData1 = new ChartData();
            //ChartData1.name = "HWD 產出統計";
            ChartData1.name = "HWD Output statistics";
            ChartData1.type = ChartType.column.ToString();
            ChartData1.colorByPoint = true;
            List<object> chartDataSourse = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                columnData columnData = new columnData();
               // columnData.name = dt.Rows[i]["料號"].ToString();
                columnData.name = dt.Rows[i]["ItemNo"].ToString();
                //columnData.y = Convert.ToInt32(dt.Rows[i]["投入總數"]);
                columnData.y = Convert.ToInt32(dt.Rows[i]["Total_input"]);
                chartDataSourse.Add(columnData);
            }
            ChartData1.data = chartDataSourse;
            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            retChart_column.ChartDatas = _ChartDatas;
            return retChart_column;
        }

        public override void Run()
        {           
            DateTime startDT = (DateTime)startTime.Value;
            DateTime endDT = (DateTime)endTime.Value;
            string dateFrom = $@"to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            string dateTO = $@"to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            string line = inputLine.Value.ToString();
            string sku_name = inputSkuname.Value.ToString();
            string sku = inputSku.Value.ToString();
            string station = inputStation.Value.ToString();
            string sql_line = "";
            string sql_fail_line = "";
            string sql_sku = "";
            string sql_fail_sku = "";
            string sql_sku_name = "";
            string sql_station = "";
            string sql_fail_station = "";

            if (!string.IsNullOrEmpty(line) && line.ToUpper() != "ALL")
            {
                sql_line = $@" and a.line='{line}'";
                sql_fail_line = $@" and rm.fail_line='{line}'";
            }
            if (!string.IsNullOrEmpty(sku))
            {
                sql_sku = $@" and a.skuno='{sku}'";
                sql_fail_sku = $@" and rm.skuno='{sku}'";
            }
            if (!string.IsNullOrEmpty(station) && station.ToUpper() != "ALL")
            {
                sql_station = $@" and a.station_name='{station}'";
                sql_fail_station = $@" and rm.fail_station ='{station}'";
            }
            else
            {
                sql_station = $@" and a.station_name in (select distinct value from r_function_control where functionname='PanelFailStation' 
                         and controlflag='Y' and functiontype='NOSYSTEM')";
                sql_fail_station = $@" and rm.fail_station in (select distinct value from r_function_control where functionname='PanelFailStation' 
                         and controlflag='Y' and functiontype='NOSYSTEM')";
            }
            if (!string.IsNullOrEmpty(sku_name))
            {
                sql_sku_name = $@" and s.sku_name='{sku_name}'";
            }

            //string sqlRun = $@"select input.line as 線別,input.skuno as 料號,input.input_qty as 投入總數,
            //            decode(fail.fail_qty,null,0,fail.fail_qty) as 不良總數 ,
            //            decode(fail.fail_qty,null,0||'%' ,to_char(round(fail.fail_qty/input.input_qty * 100, 2),'fm9999990.9999')||'%' ) as 不良率
            //             from (select a.skuno,a.line,count(distinct a.sn) as input_qty from r_sn_station_detail a where
            //            a.station_name='VI1'and a.edit_time between to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss')
            //            and to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss') group by a.skuno, a.line) input 
            //            left join (select skuno,fail_line,count(*) as fail_qty  from (
            //            select rm.skuno,rm.fail_line,rm.sn,count(distinct ra.description) from r_repair_main rm,r_repair_failcode ra 
            //            where rm.id=ra.repair_main_id and rm.fail_station='VI1'
            //            and rm.fail_time between to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss')
            //            and to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss') 
            //            group by rm.skuno,rm.fail_line,rm.sn) group by skuno,fail_line) fail 
            //            on input.skuno=fail.skuno and input.line=fail.fail_line";

            //string sqlRun = $@"select input.line as 線別,input.station_name 工位,input.sku_name as 機種名,input.skuno as 料號,input.input_qty as 投入總數,
            //            decode(fail.fail_qty,null,0,fail.fail_qty) as 不良總數 ,
            //            decode(fail.fail_qty,null,0||'%' ,to_char(round(fail.fail_qty/input.input_qty * 100, 2),'fm9999990.9999')||'%' ) as 不良率
            //             from (select  s.sku_name, a.skuno,a.line,a.station_name,count(distinct a.sn) as input_qty from r_sn_station_detail a,c_sku s where
            //            1=1 and  s.skuno=a.skuno  {sql_line} {sql_sku} {sql_sku_name} {sql_station}
            //            and a.edit_time between to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss')
            //            and to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss') group by a.skuno,s.sku_name, a.line,a.station_name) input 
            //            left join (select skuno,fail_line,fail_station,sum(fail_qty) as fail_qty  from (
            //            select s.sku_name,rm.skuno,rm.fail_line, rm.fail_station ,rm.sn,count(distinct ra.description) as fail_qty from r_repair_main rm,r_repair_failcode ra,c_sku s 
            //            where rm.id=ra.repair_main_id and rm.skuno=s.skuno {sql_fail_station} {sql_fail_line} {sql_fail_sku} {sql_sku_name}
            //            and rm.fail_time between to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss')
            //            and to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss') 
            //            group by s.sku_name,rm.skuno,rm.fail_line,rm.sn, rm.fail_station) group by sku_name,skuno,fail_line,fail_station) fail 
            //            on input.skuno=fail.skuno and input.line=fail.fail_line and input.station_name=fail.fail_station";

            string sqlRun = $@"select input.line as LineType,input.station_name WorkStation,input.sku_name as SkunoName,input.skuno as ItemNo,input.input_qty as Total_input,
                        decode(fail.fail_qty,null,0,fail.fail_qty) as Bad_qty ,
                        decode(fail.fail_qty,null,0||'%' ,to_char(round(fail.fail_qty/input.input_qty * 100, 2),'fm9999990.9999')||'%' ) as Defective_Rate
                         from (select  s.sku_name, a.skuno,a.line,a.station_name,count(distinct a.sn) as input_qty from r_sn_station_detail a,c_sku s where
                        1=1 and  s.skuno=a.skuno  {sql_line} {sql_sku} {sql_sku_name} {sql_station}
                        and a.edit_time between to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss')
                        and to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss') group by a.skuno,s.sku_name, a.line,a.station_name) input 
                        left join (select skuno,fail_line,fail_station,sum(fail_qty) as fail_qty  from (
                        select s.sku_name,rm.skuno,rm.fail_line, rm.fail_station ,rm.sn,count(distinct ra.description) as fail_qty from r_repair_main rm,r_repair_failcode ra,c_sku s 
                        where rm.id=ra.repair_main_id and rm.skuno=s.skuno {sql_fail_station} {sql_fail_line} {sql_fail_sku} {sql_sku_name}
                        and rm.fail_time between to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss')
                        and to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss') 
                        group by s.sku_name,rm.skuno,rm.fail_line,rm.sn, rm.fail_station) group by sku_name,skuno,fail_line,fail_station) fail 
                        on input.skuno=fail.skuno and input.line=fail.fail_line and input.station_name=fail.fail_station";

            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsLineFial = SFCDB.RunSelect(sqlRun);
                if (dsLineFial.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                
                DataTable linkDt = new DataTable();
                //linkDt.Columns.Add("線別");
                //linkDt.Columns.Add("工位");
                //linkDt.Columns.Add("機種名");
                //linkDt.Columns.Add("料號");
                ////linkDt.Columns.Add("投入總數");
                //linkDt.Columns.Add("不良總數");
                //linkDt.Columns.Add("不良率");


                linkDt.Columns.Add("LineType");
                linkDt.Columns.Add("WorkStation");
                linkDt.Columns.Add("SkunoName");
                linkDt.Columns.Add("ItemNo");
                linkDt.Columns.Add("Total_input");
                linkDt.Columns.Add("Bad_qty");
                linkDt.Columns.Add("Defective_Rate");
                foreach (DataRow row in dsLineFial.Tables[0].Rows)
                {
                    DataRow r = linkDt.NewRow();
                    //string link_url = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.HWDLineFailSNDetail&RunFlag=1";
                    //link_url=link_url + $@"&SKU={row["料號"].ToString()}&SKU_NAME={row["機種名"].ToString()}&LINE={row["線別"].ToString()}&STATION={row["工位"].ToString()}";
                    //link_url = link_url + $@"&StartTime={startDT.ToString("yyyy/MM/dd HH:mm:ss")}&EndTime={endDT.ToString("yyyy/MM/dd HH:mm:ss")}";
                    //r["線別"] = "";
                    //r["工位"] = "";
                    //r["機種名"] = "";
                    //r["料號"] = "";
                    //r["投入總數"] = "";
                    //r["不良總數"] = row["不良總數"].ToString() == "0" ? "" : link_url;
                    //r["不良率"] = "";

                    string link_url = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.HWDLineFailSNDetail&RunFlag=1";
                    link_url = link_url + $@"&SKU={row["ItemNo"].ToString()}&SKU_NAME={row["SkunoName"].ToString()}&LINE={row["LineType"].ToString()}&STATION={row["WorkStation"].ToString()}";
                    link_url = link_url + $@"&StartTime={startDT.ToString("yyyy/MM/dd HH:mm:ss")}&EndTime={endDT.ToString("yyyy/MM/dd HH:mm:ss")}";
                    r["LineType"] = "";
                    r["WorkStation"] = "";
                    r["SkunoName"] = "";
                    r["ItemNo"] = "";
                    r["Total_input"] = "";
                    r["Bad_qty"] = row["Bad_qty"].ToString() == "0" ? "" : link_url;
                    r["Defective_Rate"] = "";


                    linkDt.Rows.Add(r);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dsLineFial.Tables[0], linkDt);

                reportTable.Tittle = "LineFailTable";
                Outputs.Add(reportTable);
                if (dsLineFial.Tables[0].Rows.Count > 0)
                    Outputs.Add(GetChartDataSourse(startTime.Value.ToString(), endTime.Value.ToString(), dsLineFial.Tables[0]));
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
