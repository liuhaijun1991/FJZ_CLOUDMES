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

    public class WoBaseReport:ReportBase
    {
        ReportInput WO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "" };
        ReportInput SKUNO = new ReportInput { InputType = "Autocomplete", Name = "SKUNO", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };


        public WoBaseReport()
        {
            this.Inputs.Add(WO);
            this.Inputs.Add(SKUNO);
        }

        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string wo = WO.Value.ToString();
            string skuno = SKUNO.Value.ToString();
            ReportTable retTab = new ReportTable();

            DataTable dt = SFCDB.ORM.Queryable<R_WO_BASE>().WhereIF(skuno != "", t => t.SKUNO == skuno).WhereIF(wo != "", t => t.WORKORDERNO == wo)
                .Select("SKUNO,WORKORDERNO,RELEASE_DATE,ROUTE_ID,START_STATION,WORKORDER_QTY,INPUT_QTY,FINISHED_QTY")
                .OrderBy("RELEASE_DATE")
                .ToDataTable();
            retTab.LoadData(dt);
            retTab.Tittle = "工单明细";      
            Outputs.Add(retTab);

        }
    }
}
