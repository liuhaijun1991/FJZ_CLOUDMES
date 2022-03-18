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
    public class DefectiveReport:ReportBase
    {
        OleExec SFCDB = null;
        ReportInput STATION = new ReportInput { Name = "STATION", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL" } };
        ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL" } };

        public DefectiveReport()
        {
            Inputs.Add(STATION);
            Inputs.Add(SKUNO);
        }
        public override void Init()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            ((List<string>)STATION.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_STATION>().OrderBy(t => t.STATION_NAME).Select(t => t.STATION_NAME).ToList());

            ((List<string>)SKUNO.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_SKU>().OrderBy(t => t.SKUNO).Select(t => t.SKUNO).ToList());
        }
        public override void Run()
        {
            DataTable dt = null;
            string station = STATION.Value.ToString(); 
            string sku = SKUNO.Value.ToString();
            dt = SFCDB.ORM.SqlQueryable<object>($@"SELECT A.SN,
                    B.FAIL_LOCATION,
                    C.CHINESE_DESCRIPTION,
                    C.ERROR_CATEGORY,
                    A.FAIL_TIME
                FROM R_REPAIR_MAIN A, R_REPAIR_FAILCODE B, C_ERROR_CODE C
                WHERE A.ID = B.REPAIR_MAIN_ID
                AND B.FAIL_CODE = C.ERROR_CODE
                AND A.SKUNO = '{sku}'
                AND A.FAIL_STATION = '{station}'").ToDataTable();
            ReportTable retTab = new ReportTable();
            retTab.LoadData(dt);
            retTab.Tittle = "Defective Report";
            Outputs.Add(retTab);
        }
    }
}
