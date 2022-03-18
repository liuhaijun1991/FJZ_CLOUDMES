using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BPD
{
    public class BackFlushReport: ReportBase
    {
        ReportInput Skuno = new ReportInput() { InputType = "Autocomplete", Name = "Skuno", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        ReportInput Wo = new ReportInput() { InputType = "TXT", Name = "Wo", Value = "" };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "Autocomplete", Value = "", API = "MESStation.Config.CStationConfig.GetAllStation", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        ReportInput StartDate = new ReportInput() { InputType = "TXT", Name = "StartDate", Value = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") };
        ReportInput EndDate = new ReportInput() { InputType = "TXT", Name = "EndDate", Value = DateTime.Now.ToString("yyyy/MM/dd") };
        OleExec SFCDB = null;

        public BackFlushReport()
        {
            this.Inputs.Add(Skuno);
            this.Inputs.Add(Wo);
            this.Inputs.Add(Station);
            this.Inputs.Add(StartDate);
            this.Inputs.Add(EndDate);
        }

        public override void Run()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            DateTime _StartDate = new DateTime();
            DateTime _EndDate = new DateTime();
            if (!DateTime.TryParse(StartDate.Value.ToString(), out _StartDate) || !DateTime.TryParse(EndDate.Value.ToString(), out _EndDate))
            {
                Outputs.Add(new ReportAlart("Date format is invalid!"));
                return;
            }

            DataTable dt = SFCDB.ORM.Queryable<R_BACKFLUSH_HISTORY>()
                .WhereIF(Skuno.Value.ToString().Length > 0, t => t.SKUNO == Skuno.Value.ToString())
                .WhereIF(Wo.Value.ToString().Length > 0, t => t.WORKORDERNO == Wo.Value.ToString())
                .WhereIF(Station.Value.ToString().Length > 0, t => t.SFC_STATION == Station.Value.ToString())
                .Where($@"back_date>=to_date('{StartDate.Value.ToString()}','YYYY/MM/DD') and back_date<to_date('{EndDate.Value.ToString()}','YYYY/MM/DD')")
                .Select("skuno,workorderno,sfc_station,workorder_qty,sfc_qty,last_sfc_qty,diff_qty,mrb_qty,back_date")
                .OrderBy("skuno,back_date")
                .ToDataTable();

            ReportTable rt = new ReportTable();
            rt.Tittle = "BackFlush 拋帳報表";
            rt.LoadData(dt);
            this.Outputs.Add(rt);
        }
    }
}
