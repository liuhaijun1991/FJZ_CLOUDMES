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
    
    class MrbBackFlushReport: ReportBase
    {
        ReportInput Skuno = new ReportInput() { InputType = "Autocomplete", Name = "Skuno", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        ReportInput Wo = new ReportInput() { InputType = "TXT", Name = "Wo", Value = "" };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "Autocomplete", Value = "", API = "MESStation.Config.CStationConfig.GetAllStation", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        ReportInput Successfully = new ReportInput() { Name = "Successfully", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Y", "N" } };
        ReportInput StartDate = new ReportInput() { InputType = "TXT", Name = "StartDate", Value = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") };
        ReportInput EndDate = new ReportInput() { InputType = "TXT", Name = "EndDate", Value = DateTime.Now.ToString("yyyy/MM/dd") };
        OleExec SFCDB = null;

        public MrbBackFlushReport()
        {
            this.Inputs.Add(Skuno);
            this.Inputs.Add(Wo);
            this.Inputs.Add(Station);
            this.Inputs.Add(Successfully);
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

            DataTable dt = SFCDB.ORM.Queryable<R_MRB_GT,C_SAP_STATION_MAP>((rmg,cssm)=>rmg.SKUNO==cssm.SKUNO && rmg.SAP_STATION_CODE==cssm.SAP_STATION_CODE)
                .WhereIF(Skuno.Value.ToString().Length > 0, (rmg, cssm) => rmg.SKUNO == Skuno.Value.ToString())
                .WhereIF(Wo.Value.ToString().Length>0, (rmg, cssm) => rmg.WORKORDERNO==Wo.Value.ToString())
                .WhereIF(Station.Value.ToString().Length>0, (rmg, cssm) => cssm.STATION_NAME==Station.Value.ToString())
                .WhereIF(Successfully.Value.ToString().Equals("Y"),(rmg,cssm)=>rmg.SAP_MESSAGE.ToString().Contains("posted"))
                .WhereIF(Successfully.Value.ToString().Equals("N"), (rmg, cssm) => !rmg.SAP_MESSAGE.ToString().Contains("posted") )
                .Where($@"rmg.edit_time>=to_date('{StartDate.Value.ToString()}','YYYY/MM/DD') and rmg.edit_time<to_date('{EndDate.Value.ToString()}','YYYY/MM/DD')")
                .Select("rmg.skuno,rmg.workorderno,cssm.station_name,rmg.to_storage,rmg.total_qty,rmg.confirmed_flag,decode(rmg.sap_flag,'4','Transfered','0','Waiting','2','Waiting','Unknow') status,sap_message,rmg.edit_emp,rmg.edit_time")
                .OrderBy("rmg.skuno,rmg.edit_time")
                .ToDataTable();

            ReportTable rt = new ReportTable();
            rt.Tittle = "MRB BackFlush 拋帳報表";
            rt.LoadData(dt);
            this.Outputs.Add(rt);
        }
    }
}
