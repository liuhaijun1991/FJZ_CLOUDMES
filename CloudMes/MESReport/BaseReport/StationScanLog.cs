using System;
using MESDataObject.Module;
using MESDBHelper;
using System.Data;

namespace MESReport.BaseReport
{
    public class StationScanLog : ReportBase
    {
        ReportInput Sn = new ReportInput() { Name = "Sn", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Line = new ReportInput() { Name = "Line", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ST = new ReportInput() { Name = "ST", InputType = "DateTime", Value = DateTime.Now.ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ET = new ReportInput() { Name = "ET", InputType = "DateTime", Value = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        public StationScanLog()
        {
            Inputs.Add(Sn);
            Inputs.Add(Station);
            Inputs.Add(Line);
            Inputs.Add(ST);
            Inputs.Add(ET);
        }
        public override void Run()
        {

            DataRow linkDataRow = null;
            string skuno = Sn.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DateTime std = Convert.ToDateTime(ST.Value.ToString()) , etd = Convert.ToDateTime(ET.Value.ToString());
                var res = SFCDB.ORM.Queryable<R_STATION_SCAN_LOG>().WhereIF(!string.IsNullOrEmpty(Sn.Value.ToString()), t => t.SCANKEY == Sn.Value.ToString())
                    .WhereIF(!string.IsNullOrEmpty(Station.Value.ToString()), t => t.STATION == Station.Value.ToString())
                    .WhereIF(!string.IsNullOrEmpty(Line.Value.ToString()), t => t.LINE == Line.Value.ToString())
                    .WhereIF(!string.IsNullOrEmpty(ST.Value.ToString()), t => t.CREATETIME > std)
                    .WhereIF(!string.IsNullOrEmpty(ET.Value.ToString()), t => t.CREATETIME < etd).ToDataTable();
                               
          
                if (res.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(res);
                retTab.Tittle = "StationScanLog Report";
                Outputs.Add(retTab);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
