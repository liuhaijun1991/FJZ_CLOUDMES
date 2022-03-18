using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;
using MESDataObject.Module;

namespace MESReport.BPD
{
    public class SNWipReport : ReportBase
    {
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "FXS224101FQ", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };

        public SNWipReport()
        {
            this.Inputs.Add(SN);
        }

        public override void Run()
        {
            ReportAlart alert = new ReportAlart();
            if (SN.Value ==null)
            {
                alert.Msg = "SN Can not be null";
                Outputs.Add(alert);
                return;
            }
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string sn = SN.Value.ToString();
            ReportTable retTab = new ReportTable();
            DataTable strGetSN = SFCDB.ORM.Queryable<R_SN, C_ROUTE>((rs, cr) => rs.ROUTE_ID == cr.ID)
                .Where((rs, cr) =>rs.SN == sn || rs.BOXSN==sn )
                .Select(@"sn,skuno,workorderno,plant,cr.route_name,current_station,next_station,started_flag,start_time,completed_flag,
                                completed_time,packed_flag,packdate,shipped_flag,shipdate,repair_failed_flag,po_no,cust_order_no,cust_pn,boxsn,
                                scraped_flag,scraped_time,product_status,rework_count,valid_flag,stock_status,stock_in_time,rs.edit_time").ToDataTable();
            DataTable strGetSnStaion = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().OrderBy(t=>t.EDIT_TIME)
                .Where(ssd => ssd.SN == sn).ToDataTable();
            DataTable strGetKeySn = SFCDB.ORM.Queryable<R_SN_KEYPART_DETAIL, R_SN>((sk, rs) => sk.R_SN_ID == rs.ID)
                .Where((sk, rs) => rs.SN == sn || rs.BOXSN == sn)
                .Select((sk, rs) => sk).ToDataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow;

            #region SN的wip信息
            foreach (DataColumn column in strGetSN.Columns)
            {
                linkTable.Columns.Add(column.ColumnName);
            }
            foreach (DataRow row in strGetSN.Rows)
            {
                string routeURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.RouteReport&RunFlag=1&=ALL&ROUTENAME=" + row["route_name"].ToString();
                string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.WoWipReport&RunFlag=1&=ALL&WO=" + row["workorderno"].ToString();
                linkRow = linkTable.NewRow();
                foreach (DataColumn dc in linkTable.Columns)
                {
                    if (dc.ColumnName.ToString().ToUpper()== "WORKORDERNO")
                    {
                        linkRow[dc.ColumnName] = linkURL;
                    }
                    else if (dc.ColumnName.ToString().ToUpper() == "ROUTE_NAME")
                    {
                        linkRow[dc.ColumnName] = routeURL;
                    }
                    else
                    {
                        linkRow[dc.ColumnName] = "";
                    }
                }
                linkTable.Rows.Add(linkRow);
            }
            #endregion
            retTab.LoadData(strGetSN,linkTable);
            retTab.Tittle = "SN WIPDETAIL";
            Outputs.Add(retTab);

            ReportTable retTab1 = new ReportTable();
            retTab1.LoadData(strGetSnStaion);
            retTab1.Tittle = "SN STATION DETAIL";
            Outputs.Add(retTab1);

            ReportTable retTab2 = new ReportTable();
            retTab2.LoadData(strGetKeySn);
            retTab2.Tittle = "SN keypart DETAIL";
            Outputs.Add(retTab2);

        }
    }  
}
