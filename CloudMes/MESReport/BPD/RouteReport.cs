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
    public class RouteReport : ReportBase
    {
        ReportInput ROUTENAME = new ReportInput { Name = "ROUTENAME", InputType = "TXT", Value = "40-1000803-05", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };


        public RouteReport()
        {
            this.Inputs.Add(ROUTENAME);
        }

        public override void Run()
        {
            ReportAlart alert = new ReportAlart();
            if (ROUTENAME.Value == null)
            {
                alert.Msg = "route_name Can not be null ";
                Outputs.Add(alert);
                return;
            }

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string route_name = ROUTENAME.Value.ToString();

            ReportTable reTab = new ReportTable();
            DataTable strGetRoute = SFCDB.ORM.SqlQueryable<object>($@"SELECT B.ROUTE_NAME, A.ROUTE_ID, A.STATION_NAME, A.RETURN_FLAG
                  FROM C_ROUTE_DETAIL A, C_ROUTE B
                 WHERE A.ROUTE_ID = B.ID
                   AND B.ROUTE_NAME='{route_name}' order by a.seq_no").ToDataTable();
            ReportTable retTab = new ReportTable();
            retTab.LoadData(strGetRoute);
            retTab.Tittle = "Route DETAIL";
            Outputs.Add(retTab);
        }
    }


}
