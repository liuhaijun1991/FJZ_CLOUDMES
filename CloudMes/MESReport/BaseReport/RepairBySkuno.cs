using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MESReport.BaseReport
{
    class RepairBySkuno : ReportBase
    {
        ReportInput inputSku = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairBySkuno()
        {
            this.Inputs.Add(inputSku);
        }

        public override void Run()
        {
            // base.Run();
            string Skuno = inputSku.Value.ToString();
            string strSql = null;
            DataTable dt = null;
            ReportTable retTab = new ReportTable();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {

                if (inputSku.Value == null || inputSku.Value.ToString() == "")
                {

                    strSql = $@"SELECT WORKORDERNO,SN,SKUNO, CURRENT_STATION,NEXT_STATION FROM R_SN WHERE SN IN (
                                    SELECT SN
                                      FROM (SELECT ROW_NUMBER() OVER(PARTITION BY A.SN ORDER BY A.EDIT_TIME DESC) RN, A.SN, A.EDIT_TIME
                                              FROM R_REPAIR_MAIN A
                                             WHERE A.CLOSED_FLAG = '0'
                                               AND SUBSTR(A.SN, 0, 1) <> '*'
                                               AND NOT EXISTS (SELECT 1 FROM R_REPAIR_TRANSFER B WHERE A.SN = B.SN)
                                             GROUP BY A.SN, A.EDIT_TIME) WHERE RN =1 ) AND  VALID_FLAG = 1 AND REPAIR_FAILED_FLAG =1";


                }
                else
                {
                    strSql = $@"SELECT WORKORDERNO,SN,SKUNO,CURRENT_STATION,NEXT_STATION FROM R_SN WHERE SN IN (
                                    SELECT SN
                                      FROM (SELECT ROW_NUMBER() OVER(PARTITION BY A.SN ORDER BY A.EDIT_TIME DESC) RN, A.SN, A.EDIT_TIME
                                              FROM R_REPAIR_MAIN A
                                             WHERE A.CLOSED_FLAG = '0'
                                               AND SUBSTR(A.SN, 0, 1) <> '*'
                                               AND NOT EXISTS (SELECT 1 FROM R_REPAIR_TRANSFER B WHERE A.SN = B.SN)
                                             GROUP BY A.SN, A.EDIT_TIME) WHERE RN =1 ) AND  VALID_FLAG = 1 AND REPAIR_FAILED_FLAG =1 AND SKUNO='{Skuno}'";
                }
                dt = SFCDB.RunSelect(strSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }               
                retTab.LoadData(dt, null);
                retTab.Tittle = "RepairBySkuno";
                Outputs.Add(retTab);
            }
            catch (Exception ex)
            {               
                ReportAlart alart = new ReportAlart("No Data!");
                Outputs.Add(alart);
            }
            finally {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }


    }
}
