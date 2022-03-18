using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class JuniperKanBan : ReportBase
    {

        ReportInput WIPTYPE = new ReportInput { Name = "WIPTYPE", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "PTH", "MA" } };
        public JuniperKanBan()
        {
            Inputs.Add(WIPTYPE);
        }

        public override void Init()
        {
            base.Init();

            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public override void Run()
        {
            try
            {
                base.Run();
                string WIP_TYPE = WIPTYPE.Value.ToString();
                string runSql = $@"";
                if (WIP_TYPE == "PTH")
                {
                      runSql = "SELECT *\n" +
                                "  FROM (select SKUNO, NEXT_STATION, COUNT(1) QTY\n" +
                                "          from r_sn A\n" +
                                "         where next_station in ('FQA', 'PTH')\n" +
                                "           AND A.VALID_FLAG = 1\n" +
                                "           AND A.REPAIR_FAILED_FLAG = 0\n" +
                                "           AND A.SKUNO IN\n" +
                                "               (SELECT SKUNO FROM C_SKU WHERE SKU_TYPE IN ('PCBA'))\n" +
                                "         GROUP BY SKUNO, NEXT_STATION) B\n" +
                                "PIVOT(SUM(QTY)\n" +
                                "   FOR NEXT_STATION IN('FQA' as \"FQA\",\n" +
                                "                       'PTH' as \"PTH\"))";
                }
                else 
                {
                     runSql = "select * from (" +
                        "select SKUNO,NEXT_STATION,COUNT(*) QTY " +
                        "from r_sn where next_station in('PRESS-FIT','5DX2','ICT','FQA') " +
                        "AND VALID_FLAG=1 " +
                        "and REPAIR_FAILED_FLAG=0 " +
                        "AND SKUNO IN(SELECT SKUNO FROM C_SKU WHERE SKU_TYPE='PCBA')" +
                        "GROUP BY SKUNO,NEXT_STATION " +
                        "UNION" +
                        " SELECT R.SKUNO,'711 SM' NEXT_STATION,COUNT(*) QTY  FROM R_SN R WHERE " +
                        " R.VALID_FLAG=1 " +
                        "AND R.COMPLETED_FLAG=1 " +
                        "AND R.SHIPPED_FLAG !=1" +
                        "AND R.SKUNO IN(SELECT SKUNO FROM C_SKU WHERE SKU_TYPE='PCBA')" +
                        "GROUP BY R.SKUNO)b " +
                        "PIVOT(SUM(QTY)" +
                        "FOR NEXT_STATION IN('PRESS-FIT' as \"PRESS-FIT\",\n" +
                        " '5DX2' as \"5DX2\",\n" +
                        " 'ICT' as \"ICT\",\n" +
                        " 'FQA' as \"FQA\",\n" +
                        " '711 SM' as \"711 SM\"))";
                }

                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                try
                {
                    RunSqls.Add(runSql);
                    DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    ReportTable retTab = new ReportTable();
                    
                    retTab.LoadData(dt, null);
                    retTab.Tittle = "Juniper";
                    retTab.FixedHeader = true;
                    retTab.pagination = false;
                    Outputs.Add(retTab);
                }
                catch (Exception exception)
                {
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    Outputs.Add(new ReportAlart(exception.Message));
                }
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}