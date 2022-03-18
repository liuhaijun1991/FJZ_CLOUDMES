using MESDataObject.Module;
using MESDBHelper;
using System.Collections.Generic;
using System.Data;

namespace MESReport.BPD
{
    public class SnPassStationTimeReport: ReportBase
    {
        OleExec SFCDB = null;
        ReportInput WO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "" };


        public SnPassStationTimeReport()
        {
            this.Inputs.Add(WO);
        }

        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string wo = WO.Value.ToString();
            List<string> QueryStationList = new List<string>() { };
            
            ReportTable retTab = new ReportTable();
            var StationList = SFCDB.ORM.Queryable<C_ROUTE_DETAIL, R_WO_BASE>((crd, rwb) => crd.ROUTE_ID == rwb.ROUTE_ID).Where((crd, rwb) => rwb.WORKORDERNO == wo)
                .OrderBy((crd, rwb) => crd.SEQ_NO).Select((crd, rwb) => crd.STATION_NAME).ToList();

            StationList.ForEach(t =>
            {
                QueryStationList.Add( "'" + t + "' " + t);
            });

            DataTable dt = SFCDB.ORM.Ado.GetDataTable($@"
                    select *
                    from (select sn, station_name, edit_time
                        from r_sn_station_detail
                        where workorderno = '{wo}') pivot(max(edit_time) for station_name in ({string.Join(",", QueryStationList)}))
                    order by sn", new List<SqlSugar.SugarParameter>() { });
            retTab.LoadData(dt);
            retTab.Tittle = "SN過站時間明細";
            Outputs.Add(retTab);

        }
    }
}
