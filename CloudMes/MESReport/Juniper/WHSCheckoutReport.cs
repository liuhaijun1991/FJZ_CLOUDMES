using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class WHSCheckoutReport : MESReport.ReportBase
    {

        ReportInput TR_SN = new ReportInput { 
            Name = "TR_SN", 
            InputType = "TXT", 
            Value = "", 
            Enable = true, 
            SendChangeEvent = false, 
            ValueForUse = null, 
            EnterSubmit = true 
        };

        public WHSCheckoutReport()
        {
            Inputs.Add(TR_SN);
        }
        public override void Run()
        {

            string tr_snInput = TR_SN.Value.ToString().Trim().ToUpper();
         
            string sqlselect = null;

            if (String.IsNullOrEmpty(tr_snInput))
            {
                sqlselect = $@"SELECT a.ID, a.TR_SN, a.ORIGINAL_QTY, a.CHECKOUT_QTY, a.CHECKOUT_BY, a.CHECKOUT_TIME, a.RETURN_QTY, a.RETURN_BY, a.RETURN_TIME 
                            FROM MES4.R_WHS_MATL_WIP_CTRL a ORDER BY CHECKOUT_TIME DESC ";
            }
            else
            {
                sqlselect = $@"SELECT a.ID, a.TR_SN, a.ORIGINAL_QTY, a.CHECKOUT_QTY, a.CHECKOUT_BY, a.CHECKOUT_TIME, a.RETURN_QTY, a.RETURN_BY, a.RETURN_TIME 
                             FROM MES4.R_WHS_MATL_WIP_CTRL a
                             WHERE a.TR_SN = '{TR_SN.Value.ToString().Trim().ToUpper()}'
                             ORDER BY CHECKOUT_TIME DESC ";
            }

            try
            {            
                OleExec APDB = DBPools["APDB"].Borrow();
                var res = APDB.RunSelect(sqlselect); 
                
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "WHS Checkout Report";
                Outputs.Add(retTab);
              
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {                
                //if (sfcdb != null)
                //{
                //    DBPools["SFCDB"].Return(APDB);
                //}
            }
        }
    }
}
