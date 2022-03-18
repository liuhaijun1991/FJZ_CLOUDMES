using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESJuniper.Base;
using MESDataObject.Module;

namespace MESReport.Juniper
{
    public class JuniperECNDetailReport : MESReport.ReportBase
    {
        ReportInput SKU = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        //ReportInput REV = new ReportInput { Name = "REV", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        public JuniperECNDetailReport()
        {
            //Outputs.Add(SKU);
            Inputs.Add(SKU);
        }
        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                var skuno = SKU.Value.ToString();
                var Revs = EcnFunction.GetCurEcnVer(skuno, SFCDB);
                var wos = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => !Revs.Contains(t.SKU_VER)).ToList();

                var sns = SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((s, w) => s.WORKORDERNO == w.WORKORDERNO)
                    .Where((s, w) => !Revs.Contains(w.SKU_VER) && s.COMPLETED_FLAG == "1" && s.SHIPPED_FLAG == "0")
                    .Select((s, w) => s).ToDataTable();


            }
            catch
            { }

        }


    }
}
