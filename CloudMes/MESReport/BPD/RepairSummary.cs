using System.Linq;
using System.Text;
using System.Data;
using MESDBHelper;
using System;
using MESDataObject.Module;
using System.Threading.Tasks;

namespace MESReport.BPD
{
   public class RepairSummary:MESReport.BaseReport
    {
        OleExec SFCDB = null;
        ReportInput StartTime = new ReportInput { Name = "StartTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput { Name = "EndTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Skuno = new ReportInput { Name = "Skuno", InputType = "Select", Value = "", Enable = true, ValueForUse = null };
        ReportInput Station = new ReportInput { Name = "Station", InputType = "Select", Value = "", Enable = true,ValueForUse = null };

        public RepairSummary()
        {
            this.Inputs.Add(StartTime);
            this.Inputs.Add(EndTime);
            this.Inputs.Add(Skuno);
            this.Inputs.Add(Station);
        }


        public override void Init()
        {
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                List<object> Stations= new List<object>() { ""};
                
                Stations.Add(SFCDB.ORM.Queryable<C_ROUTE_DETAIL, R_WO_BASE>());
                //Stations.AddRange(SFCDB.ORM.Queryable<C_ROUTE_DETAIL,R_WO_BASE>((a,b)=>a.ROUTE_ID==b.ROUTE_ID)
                //    .OrderBy((a,b)=>b.WORKORDERNO)
                //    .OrderBy(a=>a.STATION_NAME)
                //    .Select((a,b)=>a.STATION_NAME)
                //    .ToList());
                //Station.ValueForUse = Stations;
            }
            catch (Exception e)
            {
                throw e;
            }    
        }
    }
}
