using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;

namespace MESReport.BPD
{
    public class WoWipReport : ReportBase
    {
        OleExec SFCDB = null;
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "002100017350", Enable = true, SendChangeEvent = false, ValueForUse = null,EnterSubmit=true };
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        public WoWipReport()
        {
            this.Inputs.Add(WO);
            this.Inputs.Add(CloseFlag);
        }

        public override void Run()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            ReportAlart alert = new ReportAlart();
            TableColView ColView = null;
            if (WO.Value == null)
            {
                alert.Msg = "WO Can not be null";
                Outputs.Add(alert);
                return;
            }

            string wo = WO.Value.ToString();
            string closeflag = CloseFlag.Value.ToString();

            string linkURL = "/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListByWo&RunFlag=1&WO=" + wo + "&EventName=";

            DataTable dt= SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == wo).WhereIF(closeflag=="Y",t=>t.CLOSED_FLAG=="1")
                .WhereIF(closeflag=="N",t=>t.CLOSED_FLAG=="0")
                .Select("workorderno,skuno,workorder_qty,trunc(sysdate-download_date) as online_period")
                .ToDataTable();
            if (dt.Rows.Count == 0)
            {
                alert.Msg = "No Data!";
                Outputs.Add(alert);
                return;
            }

            //工單的基本數據
            TableRowView RowView = new TableRowView();

            foreach (DataColumn col in dt.Columns)
            {
                ColView = new TableColView()
                {
                    ColName=col.ColumnName,
                    Value = dt.Rows[0][col.ColumnName].ToString(),
                };
                RowView.RowData.Add(col.ColumnName, ColView);
            }
            

            //工單裡面的 SN 狀態分佈
            dt = SFCDB.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == wo && t.REPAIR_FAILED_FLAG!=null && t.REPAIR_FAILED_FLAG!="1" && t.NEXT_STATION!="REWORK")
                .Select("next_station,count(next_station) count")
                .GroupBy(t => t.NEXT_STATION).ToDataTable();


            dt = SFCDB.ORM.SqlQueryable<object>($@"select a.station_name,
                                                   case
                                                     when b.count is null then
                                                      a.count
                                                     else
                                                      b.count
                                                   end count
                                              from (select station_name, 0 count
                                                      from c_route_detail
                                                     where route_id in (select route_id
                                                                          from r_sn
                                                                         where workorderno = '{wo}' and valid_flag='1'
                                                                           and rownum = 1) order by seq_no) a,
                                                   (select next_station, count(sn) count
                                                      from r_sn
                                                     where workorderno = '{wo}' and valid_flag='1'
                                                     group by next_station) b
                                             where a.station_name = b.next_station(+)
                                            ").ToDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                ColView = new TableColView()
                {
                    ColName= dr["station_name"].ToString(),
                    Value = dr["count"].ToString(),
                    LinkType="Link",
                    LinkData= linkURL+dr["station_name"].ToString()
                };
                RowView.RowData.Add(ColView.ColName,ColView);
            }

            int RepairWip = SFCDB.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == wo && t.REPAIR_FAILED_FLAG == "1").ToList().Count;
            ColView = new TableColView()
            {
                ColName = "RepairWip",
                Value = RepairWip.ToString(),
                LinkType = "Link",
                LinkData = linkURL + "REPAIR",
                CellStyle=new Dictionary<string, object>() { { "background", "#f00" }, { "color","#fff"} }
            };

            RowView.RowData.Add("RepairWip", ColView);

            int MrbWip = SFCDB.ORM.Queryable<R_MRB>().Where(t => t.WORKORDERNO == wo).ToList().Count;
            ColView = new TableColView()
            {
                ColName = "MRBWip",
                Value = MrbWip.ToString(),
                LinkType = "Link",
                LinkData = linkURL + "MRB",
                
            };
            RowView.RowData.Add("MRBWip", ColView);

            //構建返回的表
            ReportTable table = new ReportTable();
            table.Tittle = "Wo Wip Report";
            //構建表的列，根據 RowView 的 RowData 的 Keys 屬性
            foreach (string key in RowView.RowData.Keys)
            {
                table.ColNames.Add(key);
            }
            //構建表的數據
            table.Rows.Add(RowView);
            Outputs.Add(table);

            


            if (RowView.RowData.Keys.Count > 0)
            {
                List<object> objList = new List<object>();
                pieChart pie = new pieChart();
                pie.Tittle = "工單" + wo + "WIP分佈餅狀圖";
                pie.ChartTitle = "主標題";
                pie.ChartSubTitle = "副標題";
                ChartData chartData = new ChartData();
                chartData.name = "WOLIST";
                chartData.type = ChartType.pie.ToString();
                foreach (string key in RowView.RowData.Keys)
                {
                    if (key != "WORKORDERNO" && key != "SKUNO" && key != "ONLINE_PERIOD" && key != "WORKORDER_QTY" 
                        && RowView.RowData[key].Value != "" && RowView.RowData[key].Value != "0")
                    {
                        objList.Add(new List<object> { key, Int32.Parse(RowView.RowData[key].Value) });
                    }
                }
                
                chartData.data = objList;
                chartData.colorByPoint = true;
                List<ChartData> _ChartDatas = new List<ChartData> { chartData };
                pie.ChartDatas = _ChartDatas;
                Outputs.Add(pie);
            }

        }
    }
}
