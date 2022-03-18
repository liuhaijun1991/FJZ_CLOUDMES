using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.Juniper
{
    //SN 信息報表
    public class R_JNP_PD_KIT_DETAIL_Report : ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput PARTNO = new ReportInput { Name = "PARTNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };

        public R_JNP_PD_KIT_DETAIL_Report()
        { 
            Inputs.Add(WO);
            Inputs.Add(PARTNO);
        }

        public override void Init()
        {
            base.Init(); 
        }

        public override void Run()
        {

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                
                string sql = $@"select  SKUNO, WO, PARTNO,SN , EDIT_TIME, EDIT_BY 
                       from     R_JNP_PD_KIT_DETAIL WHERE WO ='{WO.Value}' AND PARTNO='{PARTNO.Value}' and VALID_FLAG ='1' "; 
                  
                DataSet res = SFCDB.RunSelect(sql);
                ReportTable retTab = new ReportTable();
                 

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "R_JNP_PD_KIT_DETAIL";
                Outputs.Add(retTab);
            }
            catch (Exception e)
            {
                Outputs.Add(new ReportAlart(e.ToString()));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
 

    }
}
