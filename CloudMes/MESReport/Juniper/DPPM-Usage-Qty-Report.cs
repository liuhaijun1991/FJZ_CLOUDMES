using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;

namespace MESReport.Juniper
{
    class DPPM_Usage_Qty_Report : MESReport.ReportBase
    {
        //
        //ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput dateFrom = new ReportInput() { Name = "From", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput datetTo = new ReportInput() { Name = "To", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public DPPM_Usage_Qty_Report()
        {
            Inputs.Add(dateFrom);
            Inputs.Add(datetTo);
        }

        public override void Init()
        {
            dateFrom.Value = DateTime.Now.AddDays(-30).ToString("yyyy/MM/dd");
            datetTo.Value = DateTime.Now.ToString("yyyy/MM/dd");

        }


        public override void Run()
        {

            DateTime dtF = Convert.ToDateTime(dateFrom.Value);
            DateTime dtT = Convert.ToDateTime(datetTo.Value);

            //DateTime dtFPl1 = dtF.AddDays(1);
            DateTime dtTPl1 = dtT.AddDays(1);

            string dtFrom = dtF.Year.ToString() + "/" + dtF.Month.ToString() + "/" + dtF.Day.ToString();
            string dtTo = dtTPl1.Year.ToString() + "/" + dtTPl1.Month.ToString() + "/" + dtTPl1.Day.ToString();
            string shortDt = dtF.Month.ToString() + "/" + dtF.Year.ToString();

            


            string strWOQty = $@"
                            SELECT DISTINCT SND.WORKORDERNO, SND.SKUNO, WO.WORKORDER_QTY, (
                                SELECT COUNT(SN)
                                FROM SFCRUNTIME.R_SN_STATION_DETAIL
                                WHERE NEXT_STATION = 'JOBFINISH'
                                AND WORKORDERNO = SND.WORKORDERNO
                                AND EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                                AND EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd')
                            ) FINISHED_UNITS
                            FROM SFCRUNTIME.R_SN_STATION_DETAIL SND, SFCRUNTIME.R_WO_BASE WO
                            WHERE SND.WORKORDERNO = WO.WORKORDERNO
                            AND NEXT_STATION = 'JOBFINISH'
                            AND SND.EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                            AND SND.EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd')
                            AND REGEXP_LIKE(SND.WORKORDERNO, '^(0091|0093)')
                            UNION ALL
                            SELECT DISTINCT SND.WORKORDERNO, SND.SKUNO, WO.WORKORDER_QTY, (
                                SELECT COUNT(DISTINCT SN)
                                FROM SFCRUNTIME.R_SN_STATION_DETAIL
                                WHERE CURRENT_STATION = 'PACKOUT'
                                AND WORKORDERNO = SND.WORKORDERNO
                                AND EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                                AND EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd')
                            ) COMPLETED_UNITS
                            FROM SFCRUNTIME.R_SN_STATION_DETAIL SND, SFCRUNTIME.R_WO_BASE WO
                            WHERE SND.WORKORDERNO = WO.WORKORDERNO
                            AND SND.CURRENT_STATION = 'PACKOUT'
                            AND SND.EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                            AND SND.EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd')
                            AND SND.WORKORDERNO LIKE '006%'
                            UNION ALL
                            SELECT DISTINCT SND.WORKORDERNO, SND.SKUNO, WO.WORKORDER_QTY, (
                                SELECT COUNT(SN)
                                FROM SFCRUNTIME.R_SN_STATION_DETAIL
                                WHERE NEXT_sTATION = 'JOBFINISH'
                                AND WORKORDERNO = SND.WORKORDERNO
                                AND EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                                AND EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd')
                            ) FINISHED_UNITS
                            FROM SFCRUNTIME.R_SN_STATION_DETAIL SND, SFCRUNTIME.R_WO_BASE WO
                            WHERE SND.WORKORDERNO = WO.WORKORDERNO
                            AND SND.NEXT_sTATION = 'JOBFINISH'
                            AND SND.EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                            AND SND.EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd')
                            AND REGEXP_LIKE(SND.WORKORDERNO, '^(0092|0094)') ";

            string strUsageQty = $@"SELECT 'JUNIPER' CM,
                                 '{shortDt}' MONTHYEAR,
                                 kp_no JNPR_PNO,
                                 mfr_name MANUFACTURER,
                                 CUST_KP_DESC DESCRIPTION,
                                 mfr_kp_no MFR_PNO,
                                 SUM(standard_qty) AS USAGE_QTY
                            FROM(SELECT a.p_sn,
                                         a.work_time,
                                         b.p_no,
                                         b.kp_no,
                                         B.wo,
                                         b.standard_qty,
                                         b.date_code,
                                         b.lot_code,
                                         c.mfr_name,
                                         b.mfr_kp_no,
                                         d.CUST_KP_DESC
                                    FROM(SELECT *
                                            FROM mes4.r_tr_product_detail
                                           WHERE WO IN(SELECT WO
                                                          FROM MES4.R_WO_BASE)) a,
                                          mes4.r_tr_code_detail b,
                                          mes1.c_mfr_config    c,
                                          MES1.C_CUST_KP_CONFIG d
                                   WHERE a.tr_code = b.tr_code
                                         AND b.KP_NO = d.CUST_KP_NO
                                         AND b.KP_NO not like '740-%'
                                         AND(a.process_flag = b.process_flag OR b.process_flag = 'D')
                                         AND b.mfr_code = c.mfr_code
                                         AND P_SN IN
                                                 (SELECT DISTINCT(SN)
                                                    FROM SFCRUNTIME.R_SN_STATION_DETAIL@JMES
                                                   WHERE NEXT_STATION = 'JOBFINISH'
                                                         AND EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                                                         AND EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd')
                                                         AND(WORKORDERNO LIKE '0091%' OR WORKORDERNO LIKE '0093%')))
                        GROUP BY kp_no, mfr_name, mfr_kp_no, CUST_KP_DESC
                        UNION ALL
                           SELECT 'JUNIPER' CM,
                                 '{shortDt}' MONTHYEAR,  PARTNO, VENDOR_MATL_NO, KP_DESC, MPN, COUNT(*) USAGE_QTY FROM(
                                   SELECT distinct e.sn,
                                          PARTNO,
                                          (SELECT VALUE FROM R_SN_KP@JMES WHERE SN = E.VALUE AND SCANTYPE = 'VENDOR') VENDOR_MATL_NO,
                                          MPN,
                                          (SELECT CUST_KP_DESC FROM MES1.C_CUST_KP_CONFIG  WHERE CUST_KP_NO = PARTNO) KP_DESC
                                    FROM SFCRUNTIME.R_SN_STATION_dETAIL@JMES D, SFCRUNTIME.R_SN_KP@JMES E
                                   WHERE D.SN = E.SN
                                   AND D.WORKORDERNO LIKE '006%'
                                   AND D.CURRENT_sTATION = 'PACKOUT'
                                   AND E.SCANTYPE = 'TR_SN'
                                   AND D.EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                                         AND D.EDIT_TIME < TO_DATE('{dtTo}', 'yyyy/mm/dd'))
                                   GROUP BY PARTNO, VENDOR_MATL_NO, MPN, KP_DESC
                           UNION ALL
                                 SELECT 'JUNIPER' CM,
                                 '{shortDt}' MONTHYEAR, 
                                 PARTNO, '' MANUFACTURER, KP_NAME DESCRIPTION, MPN MFR_PNO, COUNT(PARTNO) USAGE_QTY FROM(
                                SELECT
                                CASE
                                    WHEN REGEXP_LIKE (SUBSTR(KP.PARTNO, 1, 3), '[A-Z]') THEN(SELECT MPN FROM SFCRUNTIME.R_SN_KP@JMES WHERE ID = KP.ID)
                                    ELSE KP.PARTNO
                                END PARTNO,
                                CASE KP.KP_NAME
                                    WHEN 'AutoKP' THEN(SELECT PARTNO FROM SFCRUNTIME.R_SN_KP@JMES WHERE ID = KP.ID)
                                    WHEN 'KEEP_SN' THEN 'Assy, PCA'
                                    ELSE KP.KP_NAME
                                END KP_NAME,
                                MPN
                                FROM SFCRUNTIME.R_SN_KP@JMES KP
                                 WHERE EDIT_TIME >= TO_DATE('{dtFrom}', 'yyyy/mm/dd')
                                                                 AND EDIT_TIME<TO_DATE ('{dtTo}', 'yyyy/mm/dd'))
                                 WHERE KP_NAME NOT IN('PCB', 'LINK_TR_SN', 'TR_SN')
                                 AND PARTNO<> MPN
                                 GROUP BY PARTNO, KP_NAME, MPN";

            OleExec SFCDB = DBPools["APDB"].Borrow();
            OleExec SFCDB2 = DBPools["SFCDB"].Borrow();

            try
            {
                DataSet res = SFCDB.RunSelect(strUsageQty);

                ReportTable retTab = new ReportTable();
                DataTable dt = res.Tables[0].Copy();

                retTab.LoadData(res.Tables[0]);

                retTab.Tittle = "DPPM " + " Usage from " + dtF.ToShortDateString().ToString() + " to " + dtT.ToShortDateString().ToString();// + dtFrom + " to "+ dtTo;
                //retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                ReportTable retTab1 = new ReportTable();
                retTab1.LoadData(res.Tables[0], null);
                retTab1.Tittle = "DPPM";

                //WO QTY

                DataSet res2 = SFCDB2.RunSelect(strWOQty);

                ReportTable retTab2 = new ReportTable();
                DataTable dt2 = res2.Tables[0].Copy();

                retTab2.LoadData(res2.Tables[0]);

                retTab2.Tittle = "WO " + " QTY from " + dtF.ToShortDateString().ToString() + " to " + dtT.ToShortDateString().ToString();// + dtFrom + " to "+ dtTo;
                //retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab2);
                ReportTable retTab3 = new ReportTable();
                retTab3.LoadData(res2.Tables[0], null);
                retTab3.Tittle = "DPPM";


                //retTab1.ColNames.RemoveAt(0);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
    }
}
