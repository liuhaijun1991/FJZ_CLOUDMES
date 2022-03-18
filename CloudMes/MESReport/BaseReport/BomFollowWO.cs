using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;

namespace MESReport.BaseReport
{
    class BomFollowWO: ReportBase
    {
        ReportInput WO = new ReportInput { Name = "SearchByWO", InputType = "TextArea", Value = "8C3BAD6AFC58", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput cc = new ReportInput { Name = "SearchByWO", InputType = "Radio", Value = "8C3BAD6AFC58", Enable = true, SendChangeEvent = false, ValueForUse = null };

        //ReportInput SKU = new ReportInput { Name = "SearchBySku", InputType = "TextArea", Value = "8C3BAD6AFC58", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public BomFollowWO()
        {
            Inputs.Add(WO);
            //Inputs.Add(cc);
        }
        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string wo = WO.Value.ToString().Trim();
                string prewo = wo.Replace("\n", "','");
                //string runSql = $@"SELECT a.wo, a.partno, a.requestqty, b.PID, b.CUSTPN FROM (SELECT * FROM R_PRE_WO_DETAIL  WHERE WO IN ('1=1') )a  LEFT JOIN R_PRE_WO_HEAD b ON a.WO= b.WO ";
                string runSql1 = $@"SELECT a.wo, a.partno, a.requestqty, b.PID, b.CUSTPN FROM (SELECT * FROM R_PRE_WO_DETAIL  WHERE WO IN ('{prewo}') )a  LEFT JOIN R_PRE_WO_HEAD b ON a.WO= b.WO ";
                //RunSqls.Add(runSql);
                DataSet res = SFCDB.RunSelect(runSql1);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "Bom Report";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }

        }
    }
}
