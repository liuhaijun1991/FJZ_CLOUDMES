using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MESReport.BaseReport
{
    public class KITTINGCALLMATERIAL : ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        string sqlRun = "";
        public KITTINGCALLMATERIAL()
        {

            Inputs.Add(WO);
            
        }
        public override void Init()
        {
            //base.Init()
           
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
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public override void Run()
        {
            string Wo = WO.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string sqlRun;
            sqlRun = $@"";
            try
            {
                DataTable res = SFCDB.ExecuteDataTable(sqlRun, CommandType.Text);
                if (res.Rows.Count == 0)
                {
                    throw new Exception("No data");
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res, null);
                retTab.Tittle = "KITTINGCALLMATERIALREPORT";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}
