using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;

namespace MESReport.Juniper
{
    public class JuniperCooReport : ReportBase
    {
        ReportInput TONO = new ReportInput { Name = "TO_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public JuniperCooReport()
        {
            Inputs.Add(TONO);
        }
        public override void Run()
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            string runSql = "", runSql1 = "";

            string Strtono = TONO.Value.ToString().Trim();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            try
            {
                if (Strtono == "" || Strtono == null)
                {
                    runSql = $@"SELECT DISTINCT *
                                  FROM (SELECT B.INVOICENO as TONO,
                                               B.WORKORDERNO,
                                               B.SKUNO,
                                               CASE WHEN C.CUSTPID=I.PN THEN C.CUSTPID
                                                 WHEN C.CUSTPID<>I.PN THEN I.PN 
                                               END CUSTPID,
                                               B.GROUPID,
                                               TRIM(A.DESCRIPTION) AS DESCRIPTION,
                                               B.QUANTITY,
                                               B.CARTONNO,
                                               B.GROSSWEIGHT,
                                               B.COO,
                                               B.TRAILERNUMBER,
                                               B.PALLETID,
                                               B.PO_NUMBER,
                                               B.SALESORDER as DNNO,
                                               B.LASTEDITDT,
                                               B.LASTEDITBY
                                          FROM (select *
                                                  from o_agile_attr
                                                 where rowid in (select max(rowid)
                                                                   from o_agile_attr
                                                                  where DESCRIPTION is not null
                                                                  group by item_number)) A,
                                               (SELECT D.ROWID FF, D.* FROM R_JUNIPER_MFPACKINGLIST D) B,
                                               O_ORDER_MAIN C,
                                               O_I137_ITEM I
                                         WHERE A.item_number = B.SKUNO
                                           AND A.DESCRIPTION is not null
                                           AND C.ITEMID = I.ID
                                           AND B.WORKORDERNO = C.PREWO)
                                 ORDER BY TONO desc";

                    
                }
                else
                {
                    runSql = $@"SELECT *
                                  FROM (SELECT B.INVOICENO as TONO,
                                               B.WORKORDERNO,
                                               B.SKUNO,
                                               CASE WHEN C.CUSTPID=I.PN THEN C.CUSTPID
                                                 WHEN C.CUSTPID<>I.PN THEN I.PN 
                                               END CUSTPID,
                                               B.GROUPID,
                                               TRIM(A.DESCRIPTION) AS DESCRIPTION,
                                               B.QUANTITY,
                                               B.CARTONNO,
                                               B.GROSSWEIGHT,
                                               B.COO,
                                               B.TRAILERNUMBER,
                                               B.PALLETID,
                                               B.PO_NUMBER,
                                               B.SALESORDER as DNNO,
                                               B.LASTEDITDT,
                                               B.LASTEDITBY
                                          FROM (select *
                                                  from o_agile_attr
                                                 where rowid in (select max(rowid)
                                                                   from o_agile_attr
                                                                  where DESCRIPTION is not null
                                                                  group by item_number)) A,
                                               (SELECT D.ROWID FF, D.* FROM R_JUNIPER_MFPACKINGLIST D) B,
                                               O_ORDER_MAIN C,
                                               O_I137_ITEM I
                                         WHERE B.INVOICENO = '{Strtono}'
                                           AND A.item_number = B.SKUNO
                                           AND C.ITEMID = I.ID
                                           AND B.WORKORDERNO = C.PREWO
                                           AND A.DESCRIPTION is not null
                                         ORDER BY B.LASTEDITDT)";

                   
                }



                dt = SFCDB.RunSelect(runSql).Tables[0];

                DataTable ret = new DataTable();
                ret.Columns.Add("SKUNO");
                ret.Columns.Add("CUSTPID");
                ret.Columns.Add("DESCRIPTION");
                ret.Columns.Add("Coo-1");
                ret.Columns.Add("Coo-2");
                ret.Columns.Add("Coo-3");
                ret.Columns.Add("Coo-4");
                ret.Columns.Add("Coo-5");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var cr = dt.Rows[i];
                    DataRow dr = null;
                    try
                    {
                        dr = ret.Select($@"SKUNO='{cr["SKUNO"].ToString()}'").First();
                    }
                    catch
                    { }
                    if (dr == null)
                    {
                        dr = ret.NewRow();
                        ret.Rows.Add(dr);
                        dr["SKUNO"] = cr["SKUNO"].ToString();
                        dr["CUSTPID"] = cr["CUSTPID"].ToString();
                        dr["DESCRIPTION"] = cr["DESCRIPTION"].ToString();
                        dr["Coo-1"] = cr["COO"].ToString();

                    }
                    else
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            if (dr[$@"Coo-{j}"] != DBNull.Value)
                            {
                                if (dr[$@"Coo-{j}"].ToString() == cr["COO"].ToString())
                                { break; }
                            }
                            else
                            {
                                dr[$@"Coo-{j}"] = cr["COO"].ToString();
                                break;
                            }
                        }
                    }
                }


                if (ret.Rows.Count == 0 )
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                //ReportTable reportTable1 = new ReportTable();
                reportTable.LoadData(ret, null);
                //reportTable1.LoadData(dt1, null);
                reportTable.Tittle = "Juniper Coo Data";
                //reportTable1.Tittle = "Juniper TruckLoad SHIP Data";
                //reportTable1.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);
                //Outputs.Add(reportTable1);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }
}
