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
    class QueryWoBySkuReport : ReportBase
    {
        OleExec SFCDB = null;

        ReportInput Skuno = new ReportInput() { InputType = "Autocomplete", Name = "Skuno", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno",EnterSubmit=true, Enable=true, APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };

        public QueryWoBySkuReport()
        {
            this.Inputs.Add(Skuno);
        }

        public override void Run()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            ReportTable table = new ReportTable();
            table.Tittle = "工單列表";

            DataTable dt = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.CLOSED_FLAG == "0").WhereIF(Skuno.Value!=null,t=>t.SKUNO==Skuno.Value.ToString()).OrderBy(t=>t.SKUNO).OrderBy(t => t.RELEASE_DATE)
                .Select("workorderno,skuno,release_date,workorder_qty,finished_qty").ToDataTable();
            table.LoadData(dt);
            this.Outputs.Add(table);
        }
    }
}
