using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject;
using MESDBHelper;

namespace MESReport.Juniper
{
    public class JuniperToReport : ReportBase
    {
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = DateTime.Now.AddDays(-1), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = DateTime.Now, Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput TONO = new ReportInput { Name = "TO_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public JuniperToReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            Inputs.Add(TONO);
        }

        public override void Run()
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            string runSql = "", runSql1="";
            string st = Convert.ToDateTime(StartTime.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");
            string et = Convert.ToDateTime(EndTime.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");
            string Strtono = TONO.Value.ToString().ToUpper().Trim();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            try
            {
                if (Strtono=="" || Strtono==null)
                {
                    runSql = $@"SELECT DISTINCT *
                                  FROM (SELECT B.INVOICENO as TONO,H.SALESORDERNUMBER AS SO,
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
                                               (SELECT D.ROWID FF, D.* FROM R_JUNIPER_MFPACKINGLIST D WHERE D.LASTEDITDT > TO_DATE('{st}','yyyy/mm/dd hh24:mi:ss') and D.LASTEDITDT < TO_DATE('{et}','yyyy/mm/dd hh24:mi:ss')) B,
                                               O_ORDER_MAIN C,
                                               O_I137_ITEM I,O_I137_head H
                                         WHERE A.item_number = B.SKUNO AND I.TRANID=H.TRANID
                                           AND A.DESCRIPTION is not null
                                           AND C.ITEMID = I.ID
                                           AND B.WORKORDERNO = C.PREWO)
                                 ORDER BY TONO desc";

                    runSql1 = $@"SELECT DISTINCT *
                                  FROM (SELECT JNPB2B.SALESORDER AS DeliveryAgile,
                                               I137H.SALESORDERNUMBER AS SalesOrder,
                                               I137H.SHIPTOCOMPANY AS ShiptoName,
                                               I137H.SHIPTOCITYNAME || '' || I137H.SHIPTOREGIONCODE || ' / ' ||
                                               I137H.SHIPTOCOUNTRYCODE AS ShiptoCountry,
                                               I137I.SALESORDERLINEITEM AS SalesOrderLine,
                                               I137H.CUSTOMERPONUMBER as PO,
                                               I137I.PN as PartNumber,
                                               JNPB2B.TRAILERNUMBER as TrackingNumber,
                                               JNPB2B.INVOICENO as FJZInvoice,
                                               JNPB2B.CARTONNO as BundleName,
                                               TO_CHAR(JNPB2B.LASTEDITDT, 'MM/DD/YYYY') as ShippingDate,
                                               JNPB2B.GROSSWEIGHT AS NetWt
                                          FROM O_I137_HEAD I137H,
                                               O_I137_ITEM I137I,
                                               O_ORDER_MAIN O,
                                               (SELECT TT.ASNNUMBER, translate(TT.DELIVERYNUMBER using char_cs) DELIVERYNUMBER
                                                  FROM (SELECT a.*,
                                                               ROW_NUMBER() OVER(PARTITION BY ASNNUMBER ORDER BY createtime DESC) numbs
                                                          FROM R_I282 a
                                                         WHERE DELIVERYNUMBER IN
                                                               (SELECT DISTINCT SALESORDER
                                                                  FROM (SELECT INVOICENO,
                                                                               TRAILERNUMBER,
                                                                               SKUNO,
                                                                               WORKORDERNO,
                                                                               GROUPID,
                                                                               SALESORDER,
                                                                               SUM(CARTONNO) AS CARTONNO,
                                                                               SUM(GROSSWEIGHT),
                                                                               MAX(LASTEDITDT) AS LASTEDITDT
                                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                                         WHERE LASTEDITDT > TO_DATE('{st}','yyyy/mm/dd hh24:mi:ss') and LASTEDITDT < TO_DATE('{et}','yyyy/mm/dd hh24:mi:ss')
                                                                         GROUP BY INVOICENO,
                                                                                  TRAILERNUMBER,
                                                                                  SKUNO,
                                                                                  WORKORDERNO,
                                                                                  GROUPID,
                                                                                  SALESORDER,
                                                                                  LASTEDITDT))) TT
                                                 WHERE TT.numbs = 1
                                                union
                                                SELECT BB.ASNNUMBER,
                                                       translate(BB.DELIVERYNUMBER using char_cs) AS DELIVERYNUMBER
                                                  FROM (SELECT b.*,
                                                               ROW_NUMBER() OVER(PARTITION BY ASNNUMBER ORDER BY createtime DESC) numbs
                                                          FROM R_JNP_DOA_SHIPMENTS_ACK b
                                                         WHERE DELIVERYNUMBER IN
                                                               (SELECT DISTINCT SALESORDER
                                                                  FROM (SELECT INVOICENO,
                                                                               TRAILERNUMBER,
                                                                               SKUNO,
                                                                               WORKORDERNO,
                                                                               GROUPID,
                                                                               SALESORDER,
                                                                               SUM(CARTONNO) AS CARTONNO,
                                                                               SUM(GROSSWEIGHT),
                                                                               MAX(LASTEDITDT) AS LASTEDITDT
                                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                                         WHERE LASTEDITDT > TO_DATE('{st}','yyyy/mm/dd hh24:mi:ss') and LASTEDITDT < TO_DATE('{et}','yyyy/mm/dd hh24:mi:ss')
                                                                         GROUP BY INVOICENO,
                                                                                  TRAILERNUMBER,
                                                                                  SKUNO,
                                                                                  WORKORDERNO,
                                                                                  GROUPID,
                                                                                  SALESORDER,
                                                                                  LASTEDITDT))) BB
                                                 WHERE BB.numbs = 1) I282,
                                               (SELECT INVOICENO,
                                                       TRAILERNUMBER,
                                                       SKUNO,
                                                       WORKORDERNO,
                                                       GROUPID,
                                                       SALESORDER,
                                                       SUM(CARTONNO) AS CARTONNO,
                                                       SUM(GROSSWEIGHT) AS GROSSWEIGHT,
                                                       MAX(LASTEDITDT) AS LASTEDITDT
                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                 GROUP BY INVOICENO,
                                                          TRAILERNUMBER,
                                                          SKUNO,
                                                          WORKORDERNO,
                                                          GROUPID,
                                                          SALESORDER) JNPB2B
                                         WHERE JNPB2B.SALESORDER = I282.DELIVERYNUMBER
                                           AND I137H.TRANID = I137I.TRANID
                                           AND I137I.ID = O.ITEMID
                                           AND O.PREASN = I282.ASNNUMBER
                                           AND O.PREWO = JNPB2B.WORKORDERNO
                                           AND JNPB2B.SKUNO = O.PID)";
                }
                else
                {
                    runSql = $@"SELECT *
                                  FROM (SELECT B.INVOICENO as TONO,H.SALESORDERNUMBER AS SO,
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
                                               O_I137_ITEM I,O_I137_head H
                                         WHERE B.INVOICENO = '{Strtono}' AND I.TRANID=H.TRANID
                                           AND A.item_number = B.SKUNO
                                           AND C.ITEMID = I.ID
                                           AND B.WORKORDERNO = C.PREWO
                                           AND A.DESCRIPTION is not null
                                         ORDER BY B.LASTEDITDT)";

                    runSql1 = $@"SELECT DISTINCT *
                                  FROM (SELECT JNPB2B.SALESORDER AS DeliveryAgile,
                                               I137H.SALESORDERNUMBER AS SalesOrder,
                                               I137H.SHIPTOCOMPANY AS ShiptoName,
                                               I137H.SHIPTOCITYNAME || '' || I137H.SHIPTOREGIONCODE || ' / ' ||
                                               I137H.SHIPTOCOUNTRYCODE AS ShiptoCountry,
                                               I137I.SALESORDERLINEITEM AS SalesOrderLine,
                                               I137H.CUSTOMERPONUMBER as PO,
                                               I137I.PN as PartNumber,
                                               JNPB2B.TRAILERNUMBER as TrackingNumber,
                                               JNPB2B.INVOICENO as FJZInvoice,
                                               JNPB2B.CARTONNO as BundleName,
                                               TO_CHAR(JNPB2B.LASTEDITDT, 'MM/DD/YYYY') as ShippingDate,
                                               JNPB2B.GROSSWEIGHT AS NetWt
                                          FROM O_I137_HEAD I137H,
                                               O_I137_ITEM I137I,
                                               O_ORDER_MAIN O,
                                               (SELECT TT.ASNNUMBER, translate(TT.DELIVERYNUMBER using char_cs) DELIVERYNUMBER
                                                  FROM (SELECT a.*,
                                                               ROW_NUMBER() OVER(PARTITION BY ASNNUMBER ORDER BY createtime DESC) numbs
                                                          FROM R_I282 a
                                                         WHERE DELIVERYNUMBER IN
                                                               (SELECT DISTINCT SALESORDER
                                                                  FROM (SELECT INVOICENO,
                                                                               TRAILERNUMBER,
                                                                               SKUNO,
                                                                               WORKORDERNO,
                                                                               GROUPID,
                                                                               SALESORDER,
                                                                               SUM(CARTONNO) AS CARTONNO,
                                                                               SUM(GROSSWEIGHT),
                                                                               MAX(LASTEDITDT) AS LASTEDITDT
                                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                                         WHERE INVOICENO = '{Strtono}'
                                                                         GROUP BY INVOICENO,
                                                                                  TRAILERNUMBER,
                                                                                  SKUNO,
                                                                                  WORKORDERNO,
                                                                                  GROUPID,
                                                                                  SALESORDER,
                                                                                  LASTEDITDT))) TT
                                                 WHERE TT.numbs = 1
                                                union
                                                SELECT BB.ASNNUMBER,
                                                       translate(BB.DELIVERYNUMBER using char_cs) AS DELIVERYNUMBER
                                                  FROM (SELECT b.*,
                                                               ROW_NUMBER() OVER(PARTITION BY ASNNUMBER ORDER BY createtime DESC) numbs
                                                          FROM R_JNP_DOA_SHIPMENTS_ACK b
                                                         WHERE DELIVERYNUMBER IN
                                                               (SELECT DISTINCT SALESORDER
                                                                  FROM (SELECT INVOICENO,
                                                                               TRAILERNUMBER,
                                                                               SKUNO,
                                                                               WORKORDERNO,
                                                                               GROUPID,
                                                                               SALESORDER,
                                                                               SUM(CARTONNO) AS CARTONNO,
                                                                               SUM(GROSSWEIGHT),
                                                                               MAX(LASTEDITDT) AS LASTEDITDT
                                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                                         WHERE INVOICENO = '{Strtono}'
                                                                         GROUP BY INVOICENO,
                                                                                  TRAILERNUMBER,
                                                                                  SKUNO,
                                                                                  WORKORDERNO,
                                                                                  GROUPID,
                                                                                  SALESORDER,
                                                                                  LASTEDITDT))) BB
                                                 WHERE BB.numbs = 1) I282,
                                               (SELECT INVOICENO,
                                                       TRAILERNUMBER,
                                                       SKUNO,
                                                       WORKORDERNO,
                                                       GROUPID,
                                                       SALESORDER,
                                                       SUM(CARTONNO) AS CARTONNO,
                                                       SUM(GROSSWEIGHT) AS GROSSWEIGHT,
                                                       MAX(LASTEDITDT) AS LASTEDITDT
                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                 WHERE INVOICENO = '{Strtono}'
                                                 GROUP BY INVOICENO,
                                                          TRAILERNUMBER,
                                                          SKUNO,
                                                          WORKORDERNO,
                                                          GROUPID,
                                                          SALESORDER) JNPB2B
                                         WHERE JNPB2B.SALESORDER = I282.DELIVERYNUMBER
                                           AND I137H.TRANID = I137I.TRANID
                                           AND I137I.ID = O.ITEMID
                                           AND O.PREASN = I282.ASNNUMBER
                                           AND O.PREWO = JNPB2B.WORKORDERNO
                                           AND JNPB2B.SKUNO = O.PID)";
                }
            
                dt = SFCDB.RunSelect(runSql).Tables[0];
                dt1 = SFCDB.RunSelect(runSql1).Tables[0];
                if (dt.Rows.Count == 0&& dt1.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                ReportTable reportTable1 = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable1.LoadData(dt1, null);
                reportTable.Tittle = "Juniper TruckLoad TONO Data";
                reportTable1.Tittle = "Juniper TruckLoad SHIP Data";
                //reportTable1.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);
                Outputs.Add(reportTable1);
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
