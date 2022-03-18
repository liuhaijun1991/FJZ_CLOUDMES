using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MESPubLab.MESStation;

namespace MESReport.BaseReport
{
    public class SkuWipReport : ReportBase
    {
        ReportInput inputSku = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput { Name = "TYPE", InputType = "Select", Value = "PCBA", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "PCBA", "MODEL", "CTO", "OPTICS", "DOF", "VIRTUAL" } };
        [JsonIgnore]
        [ScriptIgnore]
        public OleExec SFCDB = null;
        public SkuWipReport()
        {
            this.Inputs.Add(inputSku);
            this.Inputs.Add(inputType);
        }
        public override void Run()
        {
            string sqlSku = "";
            string sqlRoute = "";
            string sqlsku1 = "";
            if (inputSku.Value == null || inputSku.Value.ToString() == "")
            {

                sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, 
                            trunc ( sysdate - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w 
                            WHERE  substr(w.workorderno,1,4) not IN ('006A','0091','0092','0093','0094')";
                if (LoginBU.Contains("FJZ"))
                {
                    sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, 
                            trunc ( sysdate - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w 
                            WHERE  substr(w.workorderno,1,4) IN ('006A','0091','0092','0093','0094')";
                }
            }
            else
            {
                sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, 
                            TRUNC ( SYSDATE - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID 
                            FROM R_WO_BASE W WHERE  SKUNO='{inputSku.Value.ToString().Trim().ToUpper()}' 
                            AND SUBSTR(W.workorderno,1,4) not IN ('006A','0091','0092','0093','0094') ";

                if (LoginBU.Contains("FJZ"))
                {
                    sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, 
                            TRUNC ( SYSDATE - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID 
                            FROM R_WO_BASE W WHERE  SKUNO='{inputSku.Value.ToString().Trim().ToUpper()}' 
                            AND SUBSTR(W.workorderno,1,4)  IN ('006A','0091','0092','0093','0094') ";
                }

                sqlsku1 = $@" AND SKUNO ='{inputSku.Value}'";
            }

            if (inputType.Value != null && inputType.Value.ToString() != "")
            {
                sqlSku += $@" AND EXISTS (SELECT *
                                  FROM C_SKU S
                                 WHERE W.SKUNO = S.SKUNO
                                   AND S.SKU_TYPE = '{inputType.Value}')";
                sqlRoute = $@"SELECT *
                                  FROM (SELECT a.*,
                                               ROW_NUMBER() OVER(PARTITION BY STATION_NAME, SKU_TYPE ORDER BY SEQ_NO DESC) numbs
                                          FROM (SELECT DISTINCT CASE
                                                                  WHEN R.STATION_NAME LIKE '%LOADING%' THEN
                                                                   'LOADING'
                                                                  ELSE R.STATION_NAME
                                                                END STATION_NAME,
                                                                case when STATION_NAME ='FQA' and S.SKU_TYPE = 'PCBA'  then 45
                                                                      when STATION_NAME ='5DX' and S.SKU_TYPE = 'PCBA' then 50
                                                                         when STATION_NAME ='FLYING_PROBE' and S.SKU_TYPE = 'PCBA' then 150
                                                                         when STATION_NAME ='ICT' and S.SKU_TYPE = 'PCBA' then 160
                                                                         when STATION_NAME ='STOCKIN' and S.SKU_TYPE = 'PCBA' then 170 
                                                                        when STATION_NAME ='FQA' and S.SKU_TYPE = 'MODEL' then 61 else  R.SEQ_NO end  SEQ_NO,
                                                                S.SKU_TYPE
                                                  FROM C_SKU S, C_ROUTE_DETAIL R, R_SKU_ROUTE SR
                                                 WHERE S.ID = SR.SKU_ID
                                                   {sqlsku1}
                                                   AND R.ROUTE_ID = SR.ROUTE_ID
                                                   AND S.SKU_TYPE = '{inputType.Value}'
                                                   --AND SUBSTR(S.SKUNO,1,3) IN('711','740','750','760')
                                                ) a)
                                 WHERE numbs = 1
                                 ORDER BY SKU_TYPE, SEQ_NO,STATION_NAME";
            }

            var wolinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&CloseFlag=N&WO=";
            var snlinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListReport&RunFlag=1";

            bool isborrow = false;
            if (SFCDB == null)
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                isborrow = true;
            }
            sqlSku += " order by w.skuno ";
            try
            {
                RunSqls.Add(sqlSku);
                RunSqls.Add(sqlRoute);
                DataTable dtWO = SFCDB.RunSelect(sqlSku).Tables[0];
                DataTable dtRoute = SFCDB.RunSelect(sqlRoute).Tables[0];
                if (dtWO.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                DataTable reportTable = new DataTable();
                DataTable linkTable = new DataTable();
                reportTable.Columns.Add("WORKORDERNO");
                reportTable.Columns.Add("CLOSED_FLAG");
                reportTable.Columns.Add("SKUNO");
                reportTable.Columns.Add("DAYS");
                reportTable.Columns.Add("VER");
                reportTable.Columns.Add("QTY", typeof(int));
                reportTable.Columns.Add("BALANCE", typeof(int));
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("CLOSED_FLAG");
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("DAYS");
                linkTable.Columns.Add("VER");
                linkTable.Columns.Add("QTY");
                linkTable.Columns.Add("BALANCE");

                if (inputType.Value.Equals("CTO"))//add by hgb 2022.03.09 CTO_KIT_NEW data
                {
                    reportTable.Columns.Add("KIT");
                    linkTable.Columns.Add("KIT");
                }


                for (int i = 0; i < dtRoute.Rows.Count; i++)
                {
                    //if (dtRoute.Rows[i]["STATION_NAME"].ToString() == "FPCTEST" || dtRoute.Rows[i]["STATION_NAME"].ToString() == "PICTEST")
                    //{
                    //    if (!reportTable.Columns.Contains("FCT"))
                    //    {
                    //        reportTable.Columns.Add("FCT", typeof(int));
                    //        linkTable.Columns.Add("FCT");
                    //    }
                    //}
                    //else
                    //{
                    //    reportTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString(), typeof(int));
                    //    linkTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString());
                    //}
                    if (!reportTable.Columns.Contains(dtRoute.Rows[i]["STATION_NAME"].ToString()))
                    {
                        if (!(reportTable.Columns.Contains("WAREHOUSE-SN-OUT") && inputType.Value.Equals("MODEL")))//add by hgb 2022.03.14 
                        {
                            reportTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString(), typeof(int));
                            linkTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString());
                        }
                    }
                }
                reportTable.Columns.Add("FailWip", typeof(int));
                reportTable.Columns.Add("RepairWip", typeof(int));
                reportTable.Columns.Add("MRB", typeof(int));
                reportTable.Columns.Add("REWORK", typeof(int));
                reportTable.Columns.Add("ORT", typeof(int));
                //WHS 改爲SM supermartet
                if (inputType.Value.Equals("MODEL"))
                {
                    //add by hgb 2022.03.14 
                    reportTable.Columns.Add("WHS", typeof(int));
                    reportTable.Columns.Add("JWIP", typeof(int));
                    reportTable.Columns.Add("750 SM", typeof(int));
                    reportTable.Columns.Add("750 SHIPPED", typeof(int));
                }
                else if (inputType.Value.Equals("PCBA"))
                {
                    reportTable.Columns.Add("711 SM", typeof(int));
                    reportTable.Columns.Add("711 SHIPPED", typeof(int));
                }
                else if (inputType.Value.Equals("DOF"))
                {
                    reportTable.Columns.Add("WHS", typeof(int));
                    reportTable.Columns.Add("SHIP-OUT", typeof(int));
                }
                else
                {
                    reportTable.Columns.Add("WHS", typeof(int));
                    reportTable.Columns.Add("SHIPPED", typeof(int));
                }

                if (inputType.Value.Equals("MODEL"))
                {
                    reportTable.Columns.Add("SilverWip", typeof(int));
                    reportTable.Columns.Add("CWIP", typeof(int));
                    reportTable.Columns.Add("Transformation", typeof(int));
                    reportTable.Columns.Add("Bind->MODEL", typeof(int));
                    reportTable.Columns.Add("Bind->FVN", typeof(int));
                    reportTable.Columns.Add("Bind->BTS", typeof(int));
                    reportTable.Columns.Add("Bind->CTO", typeof(int));
                }


                linkTable.Columns.Add("FailWip");
                linkTable.Columns.Add("RepairWip");
                linkTable.Columns.Add("MRB");
                linkTable.Columns.Add("REWORK");
                linkTable.Columns.Add("ORT");
                //WHS 改爲SM supermarket
                if (inputType.Value.Equals("MODEL"))
                {
                    //add by hgb 2022.03.14 
                    linkTable.Columns.Add("WHS");
                    //linkTable.Columns.Add("WHS(SM)");
                    linkTable.Columns.Add("JWIP");
                    linkTable.Columns.Add("750 SM");
                    linkTable.Columns.Add("750 SHIPPED");
                }
                else if (inputType.Value.Equals("PCBA"))
                {
                    //linkTable.Columns.Add("WHS");
                    linkTable.Columns.Add("711 SM");
                    linkTable.Columns.Add("711 SHIPPED");
                }
                else if (inputType.Value.Equals("DOF"))
                {
                    linkTable.Columns.Add("WHS");
                    linkTable.Columns.Add("SHIP-OUT");
                }
                else
                {
                    linkTable.Columns.Add("WHS");
                    linkTable.Columns.Add("SHIPPED");
                }

                if (inputType.Value.Equals("MODEL"))
                {
                    linkTable.Columns.Add("SilverWip");
                    linkTable.Columns.Add("CWIP");
                    linkTable.Columns.Add("Transformation");
                    linkTable.Columns.Add("Bind->MODEL");
                    linkTable.Columns.Add("Bind->FVN");
                    linkTable.Columns.Add("Bind->BTS");
                    linkTable.Columns.Add("Bind->CTO");
                }

                DataRow reportRow;
                DataRow linkDataRow;
                string wo;
                string sqlFailCount;
                string sqlRepairCount;
                string sqlMrbCount;
                string sqlOrtCount;
                string workorderQty;

                //add by hgb 2022.03.09 CTO_KIT_NEW data 
                string sqlcotkit = $@"   SELECT WO WORKORDERNO,
                                            'CTO_KIT' NEXT_STATION,
                                            0 QTY,
                                            SCANQTY || '/' || ORDERQTY PERCENT
                                       FROM (SELECT WO, ORDERQTY, COUNT(SN) SCANQTY
                                               FROM (SELECT A.WO, A.ORDERQTY, SN
                                                       FROM (SELECT WO, SUM(ORDERQTY) ORDERQTY
                                                               FROM R_SAP_PODETAIL A, C_SKU B, R_WO_BASE C
                                                              WHERE A.PN = B.SKUNO
                                                                AND A.WO = C.WORKORDERNO
                                                                AND C.PRODUCTION_TYPE = 'CTO'
                                                                AND B.SKU_TYPE <> 'VIRTUAL'
                                                                AND C.CLOSED_FLAG = 0
                                                              GROUP BY WO) A
                                                       LEFT JOIN R_JNP_PD_KIT_DETAIL B
                                                         ON A.WO = B.WO)
                                              GROUP BY WO, ORDERQTY) A
                                    ";

                var dtcotkitCount = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlcotkit).ToList();
                //end add by hgb 2022.03.09 CTO_KIT_NEW data

                string skusql = "";
                if (inputSku.Value != null && inputSku.Value.ToString() != "" && inputSku.Value.ToString() != "ALL")
                {
                    skusql = $@" AND WW.SKUNO='{inputSku.Value.ToString().Trim().ToUpper()}'";
                }

                string sqlSNCount = $@"SELECT W.WORKORDERNO, R.STATION_NAME NEXT_STATION, COUNT(S.SN) QTY
                                          FROM (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE 1 = 1 
                                                   {skusql}
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{inputType.Value}')) W
                                          LEFT JOIN C_ROUTE_DETAIL R
                                            ON R.ROUTE_ID = W.ROUTE_ID
                                          LEFT JOIN R_SN S
                                            ON W.WORKORDERNO = S.WORKORDERNO
                                           AND R.STATION_NAME = S.NEXT_STATION
                                          --AND S.REPAIR_FAILED_FLAG = 0
                                           AND S.VALID_FLAG<>0
                                         GROUP BY W.WORKORDERNO, R.STATION_NAME";

                var dtSNCount = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlSNCount).ToList();

                #region The last Test record is sqlFailCount So delete code
                //sqlFailCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                //                      FROM R_REPAIR_MAIN RM
                //                     WHERE NOT EXISTS
                //                     (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                //                       AND NOT EXISTS
                //                     (SELECT *
                //                              FROM R_MRB R
                //                             WHERE RM.SN = R.SN
                //                               AND R.WORKORDERNO = RM.WORKORDERNO)
                //                       AND RM.SN IN
                //                           (SELECT SN
                //                              FROM R_SN S
                //                             WHERE S.REPAIR_FAILED_FLAG = 1
                //                               AND S.VALID_FLAG<>0
                //                               AND EXISTS
                //                             (SELECT *
                //                                      FROM R_WO_BASE WW
                //                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                //                                       {skusql}
                //                                       AND EXISTS (SELECT *
                //                                              FROM C_SKU K
                //                                             WHERE WW.SKUNO = K.SKUNO
                //                                               AND K.SKU_TYPE = '{inputType.Value}'))

                //                            )
                //                       AND RM.CLOSED_FLAG = '0'
                //                     GROUP BY WORKORDERNO";
                #endregion

                sqlFailCount = $@"SELECT WORKORDERNO,COUNT(1) QTY
                                    FROM R_SN S 
                                      WHERE  EXISTS(SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}')) 
                                        --AND  EXISTS(SELECT * FROM R_TEST_RECORD T WHERE T.R_SN_ID = S.ID AND T.MESSTATION = S.NEXT_STATION AND T.STATE = 'FAIL' ) 
                                               AND EXISTS (SELECT *
                                  FROM (SELECT C.*,
                                               ROW_NUMBER() OVER(PARTITION BY C.SN, C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS
                                          FROM R_TEST_RECORD C
                                         /*WHERE S.SN = C.SN
                                           AND S.NEXT_STATION = C.MESSTATION*/)
                                 WHERE NUMS = '1'
                                   AND STATE = 'FAIL' AND S.NEXT_STATION = MESSTATION AND S.SN = SN) /*這兩個字段放內層會導致字段未定義錯誤,還是放外層吧*/                                
                        AND (S.REPAIR_FAILED_FLAG <> 1 OR S.REPAIR_FAILED_FLAG IS NULL) GROUP BY WORKORDERNO";
                var failcounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlFailCount);
                #region Scan Fail and Waite scan Check repair out is sqlRepairCount
                //sqlRepairCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                //                      FROM R_REPAIR_MAIN RM
                //                     WHERE EXISTS
                //                     (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                //                       AND NOT EXISTS
                //                     (SELECT *
                //                              FROM R_MRB R
                //                             WHERE RM.SN = R.SN
                //                               AND R.WORKORDERNO = RM.WORKORDERNO)
                //                       AND RM.SN IN
                //                           (SELECT SN
                //                              FROM R_SN S
                //                             WHERE S.REPAIR_FAILED_FLAG = 1
                //                               AND S.VALID_FLAG<>0
                //                               AND EXISTS
                //                             (SELECT *
                //                                      FROM R_WO_BASE WW
                //                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                //                                       {skusql}
                //                                       AND EXISTS (SELECT *
                //                                              FROM C_SKU K
                //                                             WHERE WW.SKUNO = K.SKUNO
                //                                               AND K.SKU_TYPE = '{inputType.Value}')))
                //                       AND RM.CLOSED_FLAG = '0'
                //                     GROUP BY WORKORDERNO";
                #endregion

                sqlRepairCount = $@"SELECT WORKORDERNO, COUNT(1) QTY FROM R_SN S WHERE S.REPAIR_FAILED_FLAG = 1 AND 
                                    EXISTS(SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}')) 
                                    AND NOT EXISTS (SELECT * FROM R_MRB B WHERE S.SN=B.SN AND REWORK_WO IS NULL )
                                    and s.next_station <> 'REWORK'
                                    GROUP BY WORKORDERNO";
                var repairCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlRepairCount);

                sqlMrbCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                                  FROM R_MRB B
                                 WHERE EXISTS (SELECT *
                                          FROM R_WO_BASE WW
                                         WHERE WW.WORKORDERNO = B.WORKORDERNO 
                                         {skusql}
                                           AND EXISTS (SELECT *
                                                  FROM C_SKU K
                                                 WHERE WW.SKUNO = K.SKUNO
                                                   AND K.SKU_TYPE = '{inputType.Value}'))
                                   AND REWORK_WO IS NULL
                                 GROUP BY WORKORDERNO";
                var mrbCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlMrbCount);

                string sqlReworkCount = $@"SELECT WORKORDERNO, COUNT(NEXT_STATION) QTY
                                              FROM R_SN S
                                             WHERE NEXT_STATION = 'REWORK'
                                               AND EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}'))
                                               AND NOT EXISTS (SELECT *
                                                      FROM R_MRB B
                                                     WHERE S.SN = B.SN
                                                       AND B.REWORK_WO IS NULL
                                                       AND B.WORKORDERNO = S.WORKORDERNO)
                                             GROUP BY WORKORDERNO";
                var reworkCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlReworkCount);

                sqlOrtCount = $@"SELECT S.WORKORDERNO,COUNT(1) QTY
                                      FROM R_LOT_DETAIL A, R_LOT_STATUS B, R_SN S
                                     WHERE B.ID = A.LOT_ID
                                       AND A.SN = S.SN
                                       AND B.SAMPLE_STATION IN ('ORT', 'ORT-FT2')
                                       --AND S.VALID_FLAG = 1
                                       AND S.VALID_FLAG<>0
                                       AND NOT EXISTS
                                     (SELECT A2.SN
                                              FROM R_TEST_RECORD A2
                                             WHERE A2.SN = A.SN
                                               AND A2.TESTATION IN ('ORT', 'ORT-FT2')
                                               AND A2.STATE = 'PASS'
                                               AND A2.ENDTIME > (SELECT MAX(CREATE_DATE)
                                                                   FROM R_LOT_DETAIL A3
                                                                  WHERE A3.SN = A.SN))
                                       AND B.SKUNO = S.WORKORDERNO
                                       AND EXISTS (SELECT *
                                              FROM R_WO_BASE WW
                                             WHERE WW.WORKORDERNO = S.WORKORDERNO
                                               {skusql}
                                               AND EXISTS (SELECT *
                                                      FROM C_SKU K
                                                     WHERE WW.SKUNO = K.SKUNO
                                                       AND K.SKU_TYPE = '{inputType.Value}'))
                                       AND S.SHIPPED_FLAG = 0
                                    GROUP BY S.WORKORDERNO";
                var ortCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlOrtCount);

                string sqlLoadingCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                              FROM R_SN S
                                             WHERE EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}'))
                                                   --AND S.VALID_FLAG<>0
                                                   AND S.NEXT_STATION NOT LIKE 'REVERSE%'
                                             GROUP BY WORKORDERNO";
                var loadingCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlLoadingCount);

                string sqlWHSCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                   {skusql}
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{inputType.Value}'))
                                           AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG<>0
                                           AND S.NEXT_STATION <> 'REWORK'
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                         GROUP BY WORKORDERNO";
                var WHSCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlWHSCount);
                string sqlSMCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                   {skusql}
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{inputType.Value}'))
                                           and EXISTS (select * from r_supermarket mm where mm.r_sn_id = s.id and mm.status = 1 )
                                           AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG<>0
                                           AND S.NEXT_STATION <> 'REWORK'
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                         GROUP BY WORKORDERNO";
                var SMCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlSMCount);
                string sqlScraped = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                   {skusql}
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{inputType.Value}'))
                                           AND S.SCRAPED_FLAG='1'
                                           AND S.VALID_FLAG<>0
                                         GROUP BY WORKORDERNO";
                var dtScraped = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlScraped);

                string sqlShippedCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                              FROM R_SN S
                                             WHERE EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       {skusql}
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{inputType.Value}'))
                                               AND (S.SHIPPED_FLAG = 1 AND (
                                                 EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                               or EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                               or EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                                      ))
                                            and s.next_station <> 'REWORK'
                                             GROUP BY WORKORDERNO";
                var ShippedCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlShippedCount);


                #region 計算MODEL 特殊欄位
                string sqlSilverWip = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                        FROM R_SN S
                                        WHERE EXISTS
                                        (SELECT *
                                                FROM R_WO_BASE WW
                                                WHERE WW.WORKORDERNO = S.WORKORDERNO
                                                {skusql}
                                                AND EXISTS (SELECT *
                                                        FROM C_SKU K
                                                        WHERE WW.SKUNO = K.SKUNO
                                                        AND K.SKU_TYPE = '{inputType.Value}'))
                                        AND EXISTS (select * from r_juniper_silver_wip sw where sw.sn=s.sn and sw.skuno=s.skuno and state_flag=1)
                                        AND S.VALID_FLAG<>0
                                        AND S.COMPLETED_FLAG = 1
                                        AND S.SHIPPED_FLAG = 0
                                        AND S.VALID_FLAG <> 0
                                        AND S.NEXT_STATION <> 'REWORK'
                                        and not EXISTS (select *
                                                    from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                        and not EXISTS (select *
                                                    from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                        and not EXISTS (select *
                                                    from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                        GROUP BY WORKORDERNO";
                var silverWipCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlSilverWip);

                string sqlOutSM = $@"select workorderno,count(*) qty
                                    from r_sn s 
                                    where completed_flag=1 
                                    and shipped_flag=0 
                                    and next_station <>'REWORK'
                                    and s.valid_flag<>0 
                                    and exists(select * from  r_supermarket m
                                    where s.id = m.r_sn_id
                                    and m.status = 0) --and s.workorderno='009200000340'
                                    and EXISTS
                                    (SELECT *
                                            FROM R_WO_BASE WW
                                            WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                            AND EXISTS (SELECT *
                                                    FROM C_SKU K
                                                    WHERE WW.SKUNO = K.SKUNO
                                                    AND K.SKU_TYPE = '{inputType.Value}'))
                                    and not exists (select *
                                            from r_supermarket rm
                                            where s.id = rm.r_sn_id
                                            and rm.status = 1)
                                    and not exists (select * from r_juniper_silver_wip w where s.sn=w.sn and w.state_flag=0) 
                                    group by workorderno";
                var sqlOutSMs = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlOutSM);

                DateTime today = SFCDB.ORM.GetDate();
                var sqlTransformation = $@"select workorderno,count(*) qty
                                            from r_sn sn
                                            where valid_flag <> 0 and  shipped_flag=1
                                            and EXISTS
                                            (SELECT *
                                                    FROM R_WO_BASE WW
                                                    WHERE WW.WORKORDERNO = SN.WORKORDERNO
                                                    AND EXISTS (SELECT *
                                                            FROM C_SKU K
                                                            WHERE WW.SKUNO = K.SKUNO
                                                            AND K.SKU_TYPE = '{inputType.Value}'))
                                            and exists
                                            (select *
                                                    from r_sn_kp kp
                                                    where kp.value = sn.sn
                                                    and kp.valid_flag = '1'  

                                                    and kp.edit_time between
                                                        to_date('{today.AddDays(-1).ToString("yyyy/MM/dd")} 06:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                                        to_date('{today.ToString("yyyy/MM/dd")} 05:59:59', 'yyyy/mm/dd hh24:mi:ss'))
                                        group by workorderno";
                var transformationCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlTransformation);

                var sqltransfor = $@"select a.workorderno,count(distinct a.sn) qty from (
                                Select sn.sn,sn.workorderno,sn.skuno From r_sn sn ,r_sn_kp kp 
                                where sn.sn=kp.value 
                                and sn.valid_flag<>0 
                                and sn.shipped_flag=1 
                                and kp.valid_flag=1   
                                and kp.edit_time
                                between to_date('{today.AddDays(-1).ToString("yyyy/MM/dd")} 06:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                to_date('{today.ToString("yyyy/MM/dd")} 05:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                and EXISTS
                                (SELECT *
                                        FROM R_WO_BASE WW
                                        WHERE WW.WORKORDERNO = SN.WORKORDERNO
                                            AND EXISTS (SELECT *
                                                FROM C_SKU K
                                                WHERE WW.SKUNO = K.SKUNO
                                                    AND K.SKU_TYPE = 'MODEL')))a";

                var sqlModeltransfor = sqltransfor + $@"
                                    left join   (select * from (select rr.sn,'' csn,rr.workorderno,rr.skuno,rr.valid_flag from r_sn rr,c_sku ck 
                                                where rr.skuno=ck.skuno and ck.sku_type='MODEL' and rr.skuno not like '%FVN' and rr.valid_flag !=0 )rp
                                                union 
                                                select so.sn,kk.value csn,so.workorderno,so.skuno,so.valid_flag from r_sn so,c_sku bb, r_sn_kp kk 
                                                where so.skuno=bb.skuno and so.sn=kk.sn  and bb.sku_type='MODEL' 
                                                and kk.valid_flag=1 and so.valid_flag !=0 and substr(so.skuno,length(so.skuno)-3,3) not in('FVN') and upper(substr( kk.kp_name,1,6)) not in( 'AUTOKP')
                                                )b
                                                on (a.sn=b.sn or a.sn=b.csn) and a.workorderno !=b.workorderno where b.workorderno is not null
                                    group by a.workorderno";
                var ModeltransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlModeltransfor);

                var sqlFvntransfor = sqltransfor + $@"
                                left join r_sn b on a.sn=b.sn and b.skuno like '%-FVN' where b.workorderno is not null
                                group by a.workorderno";
                var FvntransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlFvntransfor);

                var sqlBtstrafor = sqltransfor + $@"
                                left join (select el.sn,kl.value,kl.kp_name,el.workorderno,el.skuno from o_order_main ol,r_sn el ,r_sn_kp kl 
                                            where ol.prewo=el.workorderno and el.sn=kl.sn and kl.valid_flag=1 and ol.potype='BTS')b 
                                on a.sn=b.value where b.workorderno is not null
                                group by a.workorderno";
                var BtstransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlBtstrafor);

                var sqlCTOtransfor = sqltransfor + $@"
                                left join (select ee.sn,kk.value,kk.kp_name,ee.workorderno,ee.skuno from o_order_main oo,r_sn ee ,r_sn_kp kk 
                                            where oo.prewo=ee.workorderno and ee.sn=kk.sn and kk.valid_flag=1 and oo.potype='CTO')b 
                                on a.sn=b.value and upper(b.kp_name) LIKE 'AUTOKP%' where b.workorderno is not null
                                group by a.workorderno";
                var CtotransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlCTOtransfor);
                #endregion

                foreach (DataRow row in dtWO.Rows)
                {
                    try
                    {
                        wo = row["WORKORDERNO"] == null ? "" : row["WORKORDERNO"].ToString();
                        workorderQty = row["WORKORDER_QTY"] == null ? "0" : row["WORKORDER_QTY"].ToString();
                        var sqty = ShippedCounts.Find(t => t.WORKORDERNO == wo);
                        //update LHJ 2021年12月23日 
                        //if (sqty != null && sqty.QTY == Convert.ToDouble(workorderQty))
                        //{
                        //    continue;//not show shipped finish wo
                        //}
                        reportRow = reportTable.NewRow();
                        linkDataRow = linkTable.NewRow();
                        reportRow["WORKORDERNO"] = wo;
                        reportRow["CLOSED_FLAG"] = row["CLOSED_FLAG"] == null ? "" : row["CLOSED_FLAG"].ToString(); ;
                        linkDataRow["WORKORDERNO"] = wolinkURL + wo;
                        reportRow["SKUNO"] = row["SKUNO"] == null ? "" : row["SKUNO"].ToString();
                        reportRow["DAYS"] = row["DAYS"] == null ? "" : row["DAYS"].ToString();
                        reportRow["VER"] = row["SKU_VER"] == null ? "N/A" : row["SKU_VER"].ToString();
                        reportRow["QTY"] = workorderQty;


                        #region   add by hgb 2022.03.09 CTO_KIT_NEW data
                        var cotkitdata = dtcotkitCount.FindAll(t => t.WORKORDERNO == wo);
                        if (cotkitdata.Count > 0)
                        {

                            if (!string.IsNullOrEmpty(cotkitdata[0].NEXT_STATION))
                            {
                                if (cotkitdata[0].NEXT_STATION.Contains("CTO_KIT"))
                                {
                                    reportRow["KIT"] = cotkitdata[0].PERCENT;
                                    string cto_kitlinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.R_JNP_PD_KIT_MAIN_Report&RunFlag=1";
                                    linkDataRow["KIT"] = cto_kitlinkURL + "&WO=" + wo;
                                }
                            }
                        }


                        #endregion   add by hgb 2022.03.09 CTO_KIT_NEW data


                        #region STATION WIP
                        var data = dtSNCount.FindAll(t => t.WORKORDERNO == wo);
                        for (int i = 0; i < data.Count; i++)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(data[i].NEXT_STATION))
                                {
                                    if (data[i].NEXT_STATION.Contains("LOADING"))
                                    {
                                        reportRow["LOADING"] = data[i].QTY;
                                    }
                                    else if (data[i].NEXT_STATION == "WAREHOUSE-SN-OUT" && inputType.Value.Equals("MODEL"))
                                    {
                                        //add by hgb 2022.03.14 
                                        reportRow["WHS"] = data[i].QTY;
                                        linkDataRow["WHS"] = snlinkURL + "&WO=" + wo + "&STATION=" + data[i].NEXT_STATION + "&STATUS=PROD";
                                    }
                                    else
                                    {
                                        //if (data[i].NEXT_STATION == "FPCTEST" || data[i].NEXT_STATION == "PICTEST")
                                        //{
                                        //    int t = 0;
                                        //    int.TryParse(reportRow["FCT"].ToString(), out t);
                                        //    reportRow["FCT"] = t + data[i].QTY;
                                        //    linkDataRow["FCT"] = snlinkURL + "&WO=" + wo + "&STATION=" + data[i].NEXT_STATION + "&STATUS=PROD";
                                        //}
                                        //else
                                        //{
                                        //    reportRow[data[i].NEXT_STATION] = data[i].QTY;
                                        //    linkDataRow[data[i].NEXT_STATION] = snlinkURL + "&WO=" + wo + "&STATION=" + data[i].NEXT_STATION + "&STATUS=PROD";
                                        //}
                                        reportRow[data[i].NEXT_STATION] = data[i].QTY;
                                        linkDataRow[data[i].NEXT_STATION] = snlinkURL + "&WO=" + wo + "&STATION=" + data[i].NEXT_STATION + "&STATUS=PROD";
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        #endregion

                        #region Fail WIP
                        var failcount = failcounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["FailWip"] = (failcount == null ? 0 : failcount.QTY);
                        linkDataRow["FailWip"] = snlinkURL + "&WO=" + wo + "&STATUS=FAIL";
                        #endregion

                        #region Repiar WIP
                        var repairCount = repairCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["RepairWip"] = repairCount == null ? 0 : repairCount.QTY;
                        linkDataRow["RepairWip"] = snlinkURL + "&WO=" + wo + "&STATUS=REPAIR";
                        #endregion

                        #region MRB & REWORK
                        var mrbCount = mrbCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["MRB"] = mrbCount == null ? 0 : mrbCount.QTY;
                        linkDataRow["MRB"] = snlinkURL + "&WO=" + wo + "&STATUS=MRB";
                        Double MrbSum = mrbCount == null ? 0 : mrbCount.QTY;

                        var reworkCount = reworkCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["REWORK"] = reworkCount == null ? 0 : reworkCount.QTY;
                        linkDataRow["REWORK"] = snlinkURL + "&WO=" + wo + "&STATUS=REWORK";

                        Double reworkSum = reworkCount == null ? 0 : reworkCount.QTY;
                        try
                        {
                            reworkSum = Convert.ToInt64(reworkSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region ORT數據匯總
                        var ortCount = ortCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["ORT"] = ortCount == null ? 0 : ortCount.QTY;
                        #endregion

                        #region WHS & silverWip
                        double silverWipSum = 0;
                        var WHSCount = WHSCounts.Find(t => t.WORKORDERNO == wo);
                        var SMCount = SMCounts.Find(t => t.WORKORDERNO == wo);

                        var OutSmCount = sqlOutSMs.Find(t => t.WORKORDERNO == wo);
                        //reportRow["WHS"] = WHSCount == null ? 0 : WHSCount.QTY;
                        //linkDataRow["WHS"] = snlinkURL + "&WO=" + wo + "&STATUS=FG";
                        if (inputType.Value.Equals("MODEL"))
                        {
                            var silverWip = silverWipCounts.Find(t => t.WORKORDERNO == wo);
                            reportRow["SilverWip"] = silverWip == null ? 0 : silverWip.QTY;
                            linkDataRow["SilverWip"] = snlinkURL + "&WO=" + wo + "&STATUS=SILVER_WIP"; ;

                            silverWipSum = silverWip == null ? 0 : silverWip.QTY;

                            var CWIP = OutSmCount == null ? 0 : OutSmCount.QTY;
                            reportRow["CWIP"] = CWIP;
                            linkDataRow["CWIP"] = snlinkURL + "&WO=" + wo + "&STATUS=CWIP";
                            if (!reportRow["SKUNO"].ToString().EndsWith("FVN"))
                            {
                                var transformation = transformationCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Transformation"] = transformation == null ? 0 : transformation.QTY;
                                linkDataRow["Transformation"] = snlinkURL + "&WO=" + wo + "&STATUS=transformation";

                                var Modeltransfor = ModeltransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->MODEL"] = Modeltransfor == null ? 0 : Modeltransfor.QTY;

                                var Fvntransfor = FvntransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->FVN"] = Fvntransfor == null ? 0 : Fvntransfor.QTY;
                                //linkDataRow["Ship->FVN"] = snlinkURL + "&WO=" + wo + "&STATUS=transformation";

                                var Btstransfor = BtstransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->BTS"] = Btstransfor == null ? 0 : Btstransfor.QTY;
                                //linkDataRow["Ship->BTS"] = snlinkURL + "&WO=" + wo + "&STATUS=transformation";

                                var Ctotransformation = CtotransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->CTO"] = Ctotransformation == null ? 0 : Ctotransformation.QTY;
                                //linkDataRow["Ship->CTO"] = snlinkURL + "&WO=" + wo + "&STATUS=transformation";
                            }

                            var SM = SMCount == null ? 0 : SMCount.QTY;
                            var JWIP = (WHSCount == null ? 0 : WHSCount.QTY - silverWipSum) - SM;

                            reportRow["JWIP"] = JWIP;
                            linkDataRow["JWIP"] = snlinkURL + "&WO=" + wo + "&STATUS=JWIP";
                            reportRow["750 SM"] = SM;//WHSCount == null ? 0 : WHSCount.QTY - silverWipSum;
                            linkDataRow["750 SM"] = snlinkURL + "&WO=" + wo + "&STATUS=SUPER_MARKET";


                        }
                        else if (inputType.Value.Equals("PCBA"))
                        {
                            reportRow["711 SM"] = WHSCount == null ? 0 : WHSCount.QTY;
                            linkDataRow["711 SM"] = snlinkURL + "&WO=" + wo + "&STATUS=FG";
                        }
                        else
                        {
                            reportRow["WHS"] = WHSCount == null ? 0 : WHSCount.QTY;
                            linkDataRow["WHS"] = snlinkURL + "&WO=" + wo + "&STATUS=FG";
                        }
                        double WhsdSum = WHSCount == null ? 0 : WHSCount.QTY - silverWipSum;
                        try
                        {
                            WhsdSum = Convert.ToInt64(WhsdSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region Scraped
                        var ScrapedCount = dtScraped.Find(t => t.WORKORDERNO == wo);
                        double ScrapedSum = ScrapedCount == null ? 0 : ScrapedCount.QTY;
                        try
                        {
                            ScrapedSum = Convert.ToInt64(ScrapedSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region Shipped
                        var ShippedCount = ShippedCounts.Find(t => t.WORKORDERNO == wo);


                        if (inputType.Value.Equals("MODEL"))
                        {
                            reportRow["750 SHIPPED"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                            linkDataRow["750 SHIPPED"] = snlinkURL + "&WO=" + wo + "&STATUS=SHIPPED";
                        }
                        else if (inputType.Value.Equals("PCBA"))
                        {
                            reportRow["711 SHIPPED"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                            linkDataRow["711 SHIPPED"] = snlinkURL + "&WO=" + wo + "&STATUS=SHIPPED";
                        }
                        else if (inputType.Value.Equals("DOF"))
                        {
                            reportRow["SHIP-OUT"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                            linkDataRow["SHIP-OUT"] = snlinkURL + "&WO=" + wo + "&STATUS=SHIPPED";
                        }
                        else
                        {
                            reportRow["SHIPPED"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                            linkDataRow["SHIPPED"] = snlinkURL + "&WO=" + wo + "&STATUS=SHIPPED";
                        }

                        double ShippedSum = ShippedCount == null ? 0 : ShippedCount.QTY;
                        try
                        {
                            ShippedSum = Convert.ToInt64(ShippedSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        var loadingCount = loadingCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["LOADING"] = Convert.ToInt64(workorderQty) - (loadingCount == null ? 0 : loadingCount.QTY);
                        Double WoSum = row["WORKORDER_QTY"] == null ? 0 : Convert.ToInt64(row["WORKORDER_QTY"].ToString());
                        if (inputType.Value.Equals("MODEL"))
                        {
                            reportRow["BALANCE"] = WoSum - ScrapedSum - MrbSum - reworkSum - ShippedSum - WhsdSum - silverWipSum;
                        }
                        else
                        {
                            reportRow["BALANCE"] = WoSum - ScrapedSum - MrbSum - reworkSum - WhsdSum - ShippedSum;
                        }

                        if (reportRow["CLOSED_FLAG"].ToString() == "1")
                        {
                            var BALANCE_qty = double.Parse(reportRow["BALANCE"].ToString());
                            var LOADING_qty = double.Parse(reportRow["LOADING"].ToString()); //(double)reportRow["LOADING"];
                            var QTY_qty = double.Parse(reportRow["QTY"].ToString()); //(double)reportRow["QTY"];
                            if (BALANCE_qty > 0)
                            {
                                reportRow["QTY"] = QTY_qty - LOADING_qty;
                                reportRow["LOADING"] = 0;
                                reportRow["BALANCE"] = BALANCE_qty - LOADING_qty;
                            }
                        }


                        //if (inputType.Value.Equals("DOF"))
                        //{
                        //    DataRow[] fstRow = fstSparesTable.Rows.Count > 0 ? fstSparesTable.Select($@" MAIN_ITEM = '{reportRow["SKUNO"].ToString()}'") : null;
                        //    if(fstRow != null)
                        //    {
                        //        if(fstRow.Length > 0)
                        //        {
                        //            reportRow["FST_SPARES"] = Convert.ToInt32(fstRow[0]["QTY"].ToString());
                        //        }
                        //        else
                        //        {
                        //            reportRow["FST_SPARES"] = 0;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        reportRow["FST_SPARES"] = 0;

                        //    }
                        //    linkDataRow["FST_SPARES"] = "";
                        //}
                        reportTable.Rows.Add(reportRow);
                        linkTable.Rows.Add(linkDataRow);
                    }
                    catch (Exception ee)
                    {

                    }
                }
                DataRow dr = reportTable.NewRow();
                DataRow drl = linkTable.NewRow();
                dr["VER"] = "Total:";
                foreach (var c in reportTable.Columns)
                {
                    //add KIT by hgb 2022.03.09
                    if (c.ToString() != "KIT" && c.ToString() != "WORKORDERNO" && c.ToString() != "CLOSED_FLAG" && c.ToString() != "SKUNO" && c.ToString() != "DAYS" && c.ToString() != "VER")
                    {
                        //工站名帶"-"字符的Compute()方法會報錯，於是加個判斷換個寫法 Edit By ZHB 2020年7月10日13:30:49
                        //dr[c.ToString()] = reportTable.Compute("Sum(" + c.ToString() + ")", "");
                        var o = 0;
                        var res = true;
                        var str = c.ToString().Substring(0, 1);
                        if (!int.TryParse(str, out o))
                        {
                            res = false;
                        }
                        if (!reportTable.Columns.Contains(c.ToString()))
                        {
                            continue;
                        }

                        if (!c.ToString().Contains("-") && !c.ToString().Contains(".") && !c.ToString().Contains("_") && !c.ToString().Contains("(") && !c.ToString().Contains(")") && !res)
                        {
                            dr[c.ToString()] = reportTable.Compute("Sum(" + c.ToString() + ")", "");
                        }
                        else
                        {
                            var temp = 0;
                            for (int i = 0; i < reportTable.Rows.Count; i++)
                            {
                                if (reportTable.Rows[i][c.ToString()].ToString().Length > 0)
                                {
                                    temp = temp + Convert.ToInt32(reportTable.Rows[i][c.ToString()]);
                                }
                            }
                            dr[c.ToString()] = temp.ToString();
                        }
                    }
                }
                reportTable.Rows.Add(dr);
                linkTable.Rows.Add(drl);

                ReportTable report = new ReportTable();
                report.LoadData(reportTable, linkTable);
                report.SearchCol.Add("WORKORDERNO");
                report.SearchCol.Add("SKUNO");
                report.FixedHeader = true;
                report.FixedCol = 5;
                report.Tittle = "WO WIP";
                //report.Rows.Add(totalRow);
                Outputs.Add(report);

            }
            catch (Exception e)
            {

                ReportAlart alart = new ReportAlart(e.ToString());
                Outputs.Add(alart);
                return;
            }
            finally
            {
                if (isborrow)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }

        List<string> redColumns = new List<string>();// { "FAILWIP", "REPAIRWIP", "MRB", "REWORK" };
        #region 711 color
        // SMT #7030a0
        List<string> pcbaSMTColumns = new List<string>();// { "LOADING", "SUPPLIER-SN",  "SMT1", "SMT2", "FQA"};
        // PTH #ddebf7        
        List<string> pcbaPTHColumns = new List<string>();// {  "PTH", "PRESS-FIT", "5DX2", "CPU_LINK", "FQA2" };
        // ICT #c6e0b4  
        List<string> pcbaICTColumns = new List<string>();// { "FLYING_PROBE", "ICT", "STOCKIN" };
        #endregion
        #region 750 color 
        List<string> modelYellowColumns = new List<string>();// { "LOADING", "SI_V1", "SI_V2", "MA1", "MA2", "VI", "FQA" };
        List<string> modeBlueColumns = new List<string>();// { "ADCTEST", "BFT1","FPCTEST","PICTEST", "RE_FCT","SAT","SCBTEST", "SAT2", "RE_FCT2","BFT2","SLT","SWTEST","RIT","FST_SPARES","FQA2" };


        #endregion

        Dictionary<string, List<string>> dicHighPriority = new Dictionary<string, List<string>>();
        //Dictionary<string, List<string>> dicHighPriority = new Dictionary<string, List<string>>()
        //{
        //    {"SCEB2",new List<string>(){ "SCBE2-MX-S", "750-062572", "711-062571" } },
        //    {"SCEB3",new List<string>(){ "SCBE3-MX-S", "750-070866", "711-070865" } },
        //    {"MPC7E-MRATE",new List<string>(){"MPC7E-MRATE","MPC7E-MRATE-RTU", "750-136059", "711-098840", "711-054743" } },
        //    {"MPC7-10G 1",new List<string>(){ "MPC7E-10G",  "MPC7E-10G-RTU", "750-053323","711-053322","711-054743","711-055619"} },
        //    {"MPC7-10G 2",new List<string>(){ "MPC7E-10G-RTU",  "MPC7E-10G", "750-136058","711-098703","711-054743","711-055619" } },
        //    {"MPC10E-15C-X",new List<string>(){ "MPC10E-15C-X", "750-070395", "711-070394" } },
        //    {"MPC10E-10C-X",new List<string>(){ "MPC10E-10C-X", "750-078633", "711-070394" } },

        //};

        //HGB ADD KIT 2022.03.11
        List<string> notUse = new List<string>() { "KIT", "WORKORDERNO", "CLOSED_FLAG", "DAYS", "VER" };

        public override void DownFile()
        {
            try
            {
                //請注意目前這個生成Excel的邏輯僅適用于墨西哥Juniper
                var db = DBPools["SFCDB"].Borrow();
                var controlList = db.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "SkuWipReport_StationColor").ToList();
                redColumns = controlList.FindAll(r => r.CONTROL_TYPE == "RED_COLUMNS").OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE.Trim()).ToList();
                pcbaSMTColumns = controlList.FindAll(r => r.CONTROL_TYPE == "PCBA_SMT_COLUMNS").OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE.Trim()).ToList();
                pcbaPTHColumns = controlList.FindAll(r => r.CONTROL_TYPE == "PCBA_PTH_COLUMNS").OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE.Trim()).ToList();
                pcbaICTColumns = controlList.FindAll(r => r.CONTROL_TYPE == "PCBA_ICT_COLUMNS").OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE.Trim()).ToList();
                modelYellowColumns = controlList.FindAll(r => r.CONTROL_TYPE == "MODEL_YELLOW_COLUMNS").OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE.Trim()).ToList();
                modeBlueColumns = controlList.FindAll(r => r.CONTROL_TYPE == "MODE_BLUE_COLUMNS").OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE.Trim()).ToList();
                DBPools["SFCDB"].Return(db);

                DataTable pcbaData = GetDataByType("PCBA");
                DataTable modelData = GetDataByType("MODEL");
                DataTable backlogData = GetBacklog();
                DataTable pcbaSkuData = GetPcbaSkuData(pcbaData);
                DataTable modelSkuData = GetModelSkuData(modelData);
                DataTable firstPCBAData = new DataTable();
                DataTable secondPCBAData = new DataTable();
                DataTable firstModelData = new DataTable();
                DataTable secondModelData = new DataTable();
                DataTable firstDOFData = new DataTable();
                DataTable secondDOFData = new DataTable();
                DataTable firstCTOData = new DataTable();
                DataTable secondCTOData = new DataTable();

                DataTable testInTable = new DataTable();
                DataTable testOutTable = new DataTable();
                List<string> inOutStation = new List<string>();
                DataTable dofData = GetDataByType("DOF");
                DataTable dofSkuData = GetDofSkuData(dofData);
                DataTable ctoData = GetDataByType("CTO");
                DataTable ctoSkuData = GetDofSkuData(ctoData);

                GetHistoryData(pcbaSkuData, modelSkuData, dofSkuData, ctoSkuData, ref firstPCBAData, ref secondPCBAData, ref firstModelData, ref secondModelData,
                    ref firstDOFData, ref secondDOFData, ref firstCTOData, ref secondCTOData,
                    ref testInTable, ref testOutTable, ref inOutStation);

                string content = MESPubLab.Common.ExcelHelp.FJZSkuWipReportExportExcel(pcbaData, modelData, backlogData, pcbaSkuData, modelSkuData,
                    pcbaSMTColumns, pcbaICTColumns, pcbaPTHColumns, redColumns, modelYellowColumns, modeBlueColumns,
                    dicHighPriority, firstPCBAData, secondPCBAData, firstModelData, secondModelData, firstDOFData, secondDOFData, firstCTOData, secondCTOData,
                    testInTable, testOutTable, inOutStation, dofData, dofSkuData, ctoData, ctoSkuData);
                string fileName = "SkuWipReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));
            }
            catch (Exception e)
            {
                Outputs.Add(new ReportAlart(e.ToString()));
            }
        }
        public DataTable GetDataByType(string type)
        {
            string sqlSku = "";
            string sqlRoute = "";
            string DebugCode = "";
            //sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, 
            //            TRUNC ( SYSDATE - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w 
            //            WHERE  SUBSTR(w.workorderno,1,4) IN ('006A','0091','0092','0093','0094') 
            //            AND EXISTS (SELECT * FROM C_SKU S WHERE W.SKUNO = S.SKUNO AND S.SKU_TYPE = '{type}')  
            //            AND EXISTS (SELECT * FROM R_SN R WHERE W.WORKORDERNO=R.WORKORDERNO AND R.SHIPPED_FLAG=0)
            //            ORDER BY W.SKUNO";
            sqlSku = $@"SELECT SKUNO, WORKORDERNO ,CLOSED_FLAG,WORKORDER_QTY,START_STATION, 
                        TRUNC ( SYSDATE - DOWNLOAD_DATE) DAYS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w 
                        WHERE  SUBSTR(w.workorderno,1,4) IN ('006A','0091','0092','0093','0094') 
                        AND EXISTS (SELECT * FROM C_SKU S WHERE W.SKUNO = S.SKUNO AND S.SKU_TYPE = '{type}')  
                        ORDER BY W.SKUNO";

            sqlRoute = $@"SELECT *
                                  FROM (SELECT a.*,
                                               ROW_NUMBER() OVER(PARTITION BY STATION_NAME, SKU_TYPE ORDER BY SEQ_NO DESC) numbs
                                          FROM (SELECT DISTINCT CASE
                                                                  WHEN R.STATION_NAME LIKE '%LOADING%' THEN
                                                                   'LOADING'
                                                                  ELSE R.STATION_NAME
                                                                END STATION_NAME,
                                                                case when STATION_NAME ='FQA' and S.SKU_TYPE = 'PCBA'  then 45
                                                                         when STATION_NAME ='5DX' and S.SKU_TYPE = 'PCBA' then 50
                                                                         when STATION_NAME ='FLYING_PROBE' and S.SKU_TYPE = 'PCBA' then 150
                                                                         when STATION_NAME ='ICT' and S.SKU_TYPE = 'PCBA' then 160
                                                                         when STATION_NAME ='STOCKIN' and S.SKU_TYPE = 'PCBA' then 170 
                                                                            when STATION_NAME ='FQA' and S.SKU_TYPE = 'MODEL' then 61 else  R.SEQ_NO end  SEQ_NO,
                                                                S.SKU_TYPE
                                                  FROM C_SKU S, C_ROUTE_DETAIL R, R_SKU_ROUTE SR
                                                 WHERE S.ID = SR.SKU_ID                                                  
                                                   AND R.ROUTE_ID = SR.ROUTE_ID
                                                   AND S.SKU_TYPE = '{type}'
                                                   --AND SUBSTR(S.SKUNO,1,3) IN('711','740','750','760')
                                                ) a)
                                 WHERE numbs = 1
                                 ORDER BY SKU_TYPE, SEQ_NO,STATION_NAME";


            SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DebugCode = "1";
                RunSqls.Add(sqlSku);
                RunSqls.Add(sqlRoute);
                DataTable dtWO = SFCDB.RunSelect(sqlSku).Tables[0];
                DataTable dtRoute = SFCDB.RunSelect(sqlRoute).Tables[0];
                if (dtWO.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                DataTable reportTable = new DataTable();

                reportTable.Columns.Add("WORKORDERNO");
                reportTable.Columns.Add("CLOSED_FLAG");
                reportTable.Columns.Add("SKUNO");
                reportTable.Columns.Add("DAYS");
                reportTable.Columns.Add("VER");
                reportTable.Columns.Add("QTY", typeof(int));
                reportTable.Columns.Add("BALANCE", typeof(int));

                if (type.Equals("CTO"))//add by hgb 2022.03.09 CTO_KIT_NEW data
                {
                    reportTable.Columns.Add("KIT");
                }

                //if (type.Equals("DOF"))
                //{
                //    DebugCode = "1.1";
                //    if (!reportTable.Columns.Contains("FST_SPARES"))
                //    {
                //        reportTable.Columns.Add("FST_SPARES", typeof(int));
                //    }
                //}
                for (int i = 0; i < dtRoute.Rows.Count; i++)
                {
                    //if (dtRoute.Rows[i]["STATION_NAME"].ToString() == "FPCTEST" || dtRoute.Rows[i]["STATION_NAME"].ToString() == "PICTEST")
                    //{
                    //    if (!reportTable.Columns.Contains("FCT"))
                    //    {
                    //        reportTable.Columns.Add("FCT", typeof(int));                            
                    //    }
                    //}
                    //else
                    //{
                    //    reportTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString(), typeof(int));

                    //}
                    if (!reportTable.Columns.Contains(dtRoute.Rows[i]["STATION_NAME"].ToString()))
                    {
                        if (!(reportTable.Columns.Contains("WAREHOUSE-SN-OUT") && inputType.Value.Equals("MODEL")))//add by hgb 2022.03.14 
                        {
                            reportTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString(), typeof(int)); 
                        }
                    }
                }
                reportTable.Columns.Add("FailWip", typeof(int));
                reportTable.Columns.Add("RepairWip", typeof(int));
                reportTable.Columns.Add("MRB", typeof(int));
                reportTable.Columns.Add("REWORK", typeof(int));
                reportTable.Columns.Add("ORT", typeof(int));
                // whs 改為SM supermarket
                if (type.Equals("MODEL"))
                {
                    //add by hgb 2022.03.14 
                    reportTable.Columns.Add("WHS", typeof(int));
                    reportTable.Columns.Add("JWIP", typeof(int));
                    reportTable.Columns.Add("750 SM", typeof(int));
                    reportTable.Columns.Add("750 SHIPPED", typeof(int));
                }
                else if (type.Equals("PCBA"))
                {
                    reportTable.Columns.Add("711 SM", typeof(int));
                    reportTable.Columns.Add("711 SHIPPED", typeof(int));
                }
                else if (type.Equals("DOF"))
                {
                    reportTable.Columns.Add("WHS", typeof(int));
                    reportTable.Columns.Add("SHIP-OUT", typeof(int));
                }
                else
                {
                    reportTable.Columns.Add("WHS", typeof(int));
                    reportTable.Columns.Add("SHIPPED", typeof(int));
                }
                if (type.Equals("MODEL"))
                {
                    reportTable.Columns.Add("SilverWip", typeof(int));
                    reportTable.Columns.Add("CWIP", typeof(int));
                    reportTable.Columns.Add("Transformation", typeof(int));
                    reportTable.Columns.Add("Bind->MODEL", typeof(int));
                    reportTable.Columns.Add("Bind->FVN", typeof(int));
                    reportTable.Columns.Add("Bind->BTS", typeof(int));
                    reportTable.Columns.Add("Bind->CTO", typeof(int));

                }

                DataRow reportRow;
                string wo;
                string sqlFailCount;
                string sqlRepairCount;
                string sqlMrbCount;
                string sqlOrtCount;
                string workorderQty;


                //add by hgb 2022.03.09 CTO_KIT_NEW data 
                string sqlcotkit = $@"   SELECT WO WORKORDERNO,
                                            'CTO_KIT' NEXT_STATION,
                                            0 QTY,
                                            SCANQTY || '/' || ORDERQTY PERCENT
                                       FROM (SELECT WO, ORDERQTY, COUNT(SN) SCANQTY
                                               FROM (SELECT A.WO, A.ORDERQTY, SN
                                                       FROM (SELECT WO, SUM(ORDERQTY) ORDERQTY
                                                               FROM R_SAP_PODETAIL A, C_SKU B, R_WO_BASE C
                                                              WHERE A.PN = B.SKUNO
                                                                AND A.WO = C.WORKORDERNO
                                                                AND C.PRODUCTION_TYPE = 'CTO'
                                                                AND B.SKU_TYPE <> 'VIRTUAL'
                                                                AND C.CLOSED_FLAG = 0
                                                              GROUP BY WO) A
                                                       LEFT JOIN R_JNP_PD_KIT_DETAIL B
                                                         ON A.WO = B.WO)
                                              GROUP BY WO, ORDERQTY) A
                                    ";

                var dtcotkitCount = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlcotkit).ToList();
                // end add by hgb 2022.03.09 CTO_KIT_NEW data 


                string sqlSNCount = $@"SELECT W.WORKORDERNO, R.STATION_NAME NEXT_STATION, COUNT(S.SN) QTY
                                          FROM (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE 1 = 1                                                    
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{type}')) W
                                          LEFT JOIN C_ROUTE_DETAIL R
                                            ON R.ROUTE_ID = W.ROUTE_ID
                                          LEFT JOIN R_SN S
                                            ON W.WORKORDERNO = S.WORKORDERNO
                                           AND R.STATION_NAME = S.NEXT_STATION
                                           --AND S.REPAIR_FAILED_FLAG = 0
                                           AND S.VALID_FLAG<>0
                                         GROUP BY W.WORKORDERNO, R.STATION_NAME";

                var dtSNCount = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlSNCount).ToList();
                DebugCode = "10";
                #region The last Test record is sqlFailCount So delete code
                //sqlFailCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                //                      FROM R_REPAIR_MAIN RM
                //                     WHERE NOT EXISTS
                //                     (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                //                       AND NOT EXISTS
                //                     (SELECT *
                //                              FROM R_MRB R
                //                             WHERE RM.SN = R.SN
                //                               AND R.WORKORDERNO = RM.WORKORDERNO)
                //                       AND RM.SN IN
                //                           (SELECT SN
                //                              FROM R_SN S
                //                             WHERE S.REPAIR_FAILED_FLAG = 1
                //                               AND S.VALID_FLAG<>0
                //                               AND EXISTS
                //                             (SELECT *
                //                                      FROM R_WO_BASE WW
                //                                     WHERE WW.WORKORDERNO = S.WORKORDERNO                                                        
                //                                       AND EXISTS (SELECT *
                //                                              FROM C_SKU K
                //                                             WHERE WW.SKUNO = K.SKUNO
                //                                               AND K.SKU_TYPE = '{type}'))

                //                            )
                //                       AND RM.CLOSED_FLAG = '0'
                //                     GROUP BY WORKORDERNO";
                #endregion

                sqlFailCount = $@"SELECT WORKORDERNO,COUNT(1) QTY
                                    FROM R_SN S 
                                      WHERE  EXISTS(SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{type}')) 
                                        --AND  EXISTS(SELECT * FROM R_TEST_RECORD T WHERE T.R_SN_ID = S.ID AND T.MESSTATION = S.NEXT_STATION AND T.STATE = 'FAIL' ) 
                                         AND EXISTS (SELECT *
                                  FROM (SELECT C.*,
                                               ROW_NUMBER() OVER(PARTITION BY C.SN, C.MESSTATION ORDER BY C.EDIT_TIME DESC) NUMS
                                          FROM R_TEST_RECORD C
                                         WHERE S.SN = C.SN
                                           AND S.NEXT_STATION = C.MESSTATION)
                                 WHERE NUMS = '1'
                                   AND STATE = 'FAIL')    
AND (S.REPAIR_FAILED_FLAG <> 1 OR S.REPAIR_FAILED_FLAG IS NULL) GROUP BY WORKORDERNO";
                var failcounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlFailCount);

                #region Scan Fail and Waite scan Check repair out is sqlRepairCount So delete code
                //sqlRepairCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                //                      FROM R_REPAIR_MAIN RM
                //                     WHERE EXISTS
                //                     (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                //                       AND NOT EXISTS
                //                     (SELECT *
                //                              FROM R_MRB R
                //                             WHERE RM.SN = R.SN
                //                               AND R.WORKORDERNO = RM.WORKORDERNO)
                //                       AND RM.SN IN
                //                           (SELECT SN
                //                              FROM R_SN S
                //                             WHERE S.REPAIR_FAILED_FLAG = 1
                //                               AND S.VALID_FLAG<>0
                //                               AND EXISTS
                //                             (SELECT *
                //                                      FROM R_WO_BASE WW
                //                                     WHERE WW.WORKORDERNO = S.WORKORDERNO                                                      
                //                                       AND EXISTS (SELECT *
                //                                              FROM C_SKU K
                //                                             WHERE WW.SKUNO = K.SKUNO
                //                                               AND K.SKU_TYPE = '{type}')))
                //                       AND RM.CLOSED_FLAG = '0'
                //                     GROUP BY WORKORDERNO";
                #endregion

                sqlRepairCount = $@"SELECT WORKORDERNO, COUNT(1) QTY FROM R_SN S WHERE S.REPAIR_FAILED_FLAG = 1 AND 
                                    EXISTS(SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{type}')) 
                                    AND NOT EXISTS (SELECT * FROM R_MRB B WHERE S.SN=B.SN AND REWORK_WO IS NULL )
                                    and s.next_station <> 'REWORK'
                                    GROUP BY WORKORDERNO";
                var repairCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlRepairCount);

                sqlMrbCount = $@"SELECT WORKORDERNO, COUNT(1) QTY
                                  FROM R_MRB B
                                 WHERE EXISTS (SELECT *
                                          FROM R_WO_BASE WW
                                         WHERE WW.WORKORDERNO = B.WORKORDERNO
                                           AND EXISTS (SELECT *
                                                  FROM C_SKU K
                                                 WHERE WW.SKUNO = K.SKUNO
                                                   AND K.SKU_TYPE = '{type}'))
                                   AND REWORK_WO IS NULL
                                 GROUP BY WORKORDERNO";
                var mrbCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlMrbCount);

                string sqlReworkCount = $@"SELECT WORKORDERNO, COUNT(NEXT_STATION) QTY
                                              FROM R_SN S
                                             WHERE NEXT_STATION = 'REWORK'
                                               AND EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO                                                       
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{type}'))
                                               AND NOT EXISTS (SELECT *
                                                      FROM R_MRB B
                                                     WHERE S.SN = B.SN
                                                       AND B.REWORK_WO IS NULL
                                                       AND B.WORKORDERNO = S.WORKORDERNO)
                                             GROUP BY WORKORDERNO";
                var reworkCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlReworkCount);

                sqlOrtCount = $@"SELECT S.WORKORDERNO,COUNT(1) QTY
                                      FROM R_LOT_DETAIL A, R_LOT_STATUS B, R_SN S
                                     WHERE B.ID = A.LOT_ID
                                       AND A.SN = S.SN
                                       AND B.SAMPLE_STATION IN ('ORT', 'ORT-FT2')
                                       --AND S.VALID_FLAG = 1
                                       AND S.VALID_FLAG<>0 
                                       AND NOT EXISTS
                                     (SELECT A2.SN
                                              FROM R_TEST_RECORD A2
                                             WHERE A2.SN = A.SN
                                               AND A2.TESTATION IN ('ORT', 'ORT-FT2')
                                               AND A2.STATE = 'PASS'
                                               AND A2.ENDTIME > (SELECT MAX(CREATE_DATE)
                                                                   FROM R_LOT_DETAIL A3
                                                                  WHERE A3.SN = A.SN))
                                       AND B.SKUNO = S.WORKORDERNO
                                       AND EXISTS (SELECT *
                                              FROM R_WO_BASE WW
                                             WHERE WW.WORKORDERNO = S.WORKORDERNO                                              
                                               AND EXISTS (SELECT *
                                                      FROM C_SKU K
                                                     WHERE WW.SKUNO = K.SKUNO
                                                       AND K.SKU_TYPE = '{type}'))
                                       AND S.SHIPPED_FLAG = 0
                                    GROUP BY S.WORKORDERNO";
                var ortCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlOrtCount);

                string sqlLoadingCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                              FROM R_SN S
                                             WHERE EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO                                                       
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{type}'))
                                                    --AND S.VALID_FLAG<>0
                                                AND S.NEXT_STATION NOT LIKE 'REVERSE%'
                                             GROUP BY WORKORDERNO";
                var loadingCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlLoadingCount);

                string sqlWHSCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO                                                   
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{type}'))
                                           AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG<>0
                                           AND S.NEXT_STATION <> 'REWORK'
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                         GROUP BY WORKORDERNO";
                var WHSCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlWHSCount);

                string sqlSMCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO                                                   
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{type}'))
                                           and EXISTS (select * from r_supermarket mm where mm.r_sn_id = s.id and mm.status = 1 )
                                           AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG<>0
                                           AND S.NEXT_STATION <> 'REWORK'
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                         GROUP BY WORKORDERNO";
                var SMCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlSMCount);

                string sqlOutSM = $@"select workorderno,count(*) qty
                                    from r_sn s 
                                    where completed_flag=1 
                                    and shipped_flag=0 
                                    and next_station <>'REWORK'
                                    and s.valid_flag<>0 
                                    and exists(select * from  r_supermarket m
                                    where s.id = m.r_sn_id
                                    and m.status = 0) --and s.workorderno='009200000340'
                                    and EXISTS
                                    (SELECT *
                                            FROM R_WO_BASE WW
                                            WHERE WW.WORKORDERNO = S.WORKORDERNO 
                                            AND EXISTS (SELECT *
                                                    FROM C_SKU K
                                                    WHERE WW.SKUNO = K.SKUNO
                                                    AND K.SKU_TYPE = '{inputType.Value}'))
                                    and not exists (select *
                                            from r_supermarket rm
                                            where s.id = rm.r_sn_id
                                            and rm.status = 1)
                                    and not exists (select * from r_juniper_silver_wip w where s.sn=w.sn and w.state_flag=0) 
                                    group by workorderno";
                var sqlOutSMs = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlOutSM);

                string sqlScraped = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO                                                  
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{type}'))
                                           AND S.SCRAPED_FLAG='1'
                                           AND S.VALID_FLAG<>0
                                         GROUP BY WORKORDERNO";
                var dtScraped = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlScraped);

                string sqlShippedCount = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                              FROM R_SN S
                                             WHERE EXISTS (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = S.WORKORDERNO                                                       
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{type}'))
                                              AND (S.SHIPPED_FLAG = 1 and (
                                                EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                               or EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                               or EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                                      ))
                                            and s.next_station <> 'REWORK'
                                             GROUP BY WORKORDERNO";
                var ShippedCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlShippedCount);

                string sqlSilverWip = $@"SELECT WORKORDERNO, COUNT(*) QTY
                                          FROM R_SN S
                                         WHERE EXISTS
                                         (SELECT *
                                                  FROM R_WO_BASE WW
                                                 WHERE WW.WORKORDERNO = S.WORKORDERNO                                                  
                                                   AND EXISTS (SELECT *
                                                          FROM C_SKU K
                                                         WHERE WW.SKUNO = K.SKUNO
                                                           AND K.SKU_TYPE = '{type}'))
                                         AND EXISTS (select * from r_juniper_silver_wip sw where sw.sn=s.sn and sw.skuno=s.skuno and state_flag=1)
                                        AND S.VALID_FLAG<>0 
                                        AND S.COMPLETED_FLAG = 1
                                           AND S.SHIPPED_FLAG = 0
                                           AND S.VALID_FLAG <> 0
                                           AND S.NEXT_STATION <> 'REWORK'
                                            and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.exvalue1 = s.id )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.kp_name like 'AutoKP%' )
                                           and not EXISTS (select *
                                                      from r_sn_kp kp  where kp.value = s.sn and kp.valid_flag = 1 and kp.partno = s.skuno )
                                        GROUP BY WORKORDERNO";
                var silverWipCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlSilverWip);

                DateTime today = SFCDB.ORM.GetDate();
                var sqlTransformation = $@"select workorderno,count(*) qty
                                              from r_sn sn
                                             where valid_flag <> 0 and  shipped_flag=1
                                               and EXISTS
                                             (SELECT *
                                                      FROM R_WO_BASE WW
                                                     WHERE WW.WORKORDERNO = SN.WORKORDERNO
                                                       AND EXISTS (SELECT *
                                                              FROM C_SKU K
                                                             WHERE WW.SKUNO = K.SKUNO
                                                               AND K.SKU_TYPE = '{type}'))
                                               and exists
                                             (select *
                                                      from r_sn_kp kp
                                                     where kp.value = sn.sn
                                                       and kp.valid_flag = '1' and (sn.skuno=kp.partno or sn.skuno = kp.mpn)
                                                       and kp.edit_time between
                                                           to_date('{today.AddDays(-1).ToString("yyyy/MM/dd")} 06:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                                           to_date('{today.ToString("yyyy/MM/dd")} 05:59:59', 'yyyy/mm/dd hh24:mi:ss'))
                                            group by workorderno";
                var transformationCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlTransformation);

                //var sqltransfor = $@"select a.workorderno,count(distinct a.sn) qty from (
                //                    Select * From r_sn sn where sn.valid_flag<>0 and sn.shipped_flag=1
                //                    and sn.shipdate 
                //                    between to_date('{today.AddDays(-1).ToString("yyyy/MM/dd")} 06:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                //                    to_date('{today.ToString("yyyy/MM/dd")} 05:59:59', 'yyyy/mm/dd hh24:mi:ss')
                //                    and EXISTS
                //                    (SELECT *
                //                            FROM R_WO_BASE WW
                //                           WHERE WW.WORKORDERNO = SN.WORKORDERNO
                //                             AND EXISTS (SELECT *
                //                                    FROM C_SKU K
                //                                   WHERE WW.SKUNO = K.SKUNO
                //                                     AND K.SKU_TYPE = 'MODEL')))a";

                var sqltransfor = $@"select a.workorderno,count(distinct a.sn) qty from (
                                    Select sn.sn,sn.workorderno,sn.skuno From r_sn sn ,r_sn_kp kp 
                                    where sn.sn=kp.value 
                                    and sn.valid_flag<>0 
                                    and sn.shipped_flag=1 
                                    and kp.valid_flag=1   
                                    and kp.edit_time
                                    between to_date('{today.AddDays(-1).ToString("yyyy/MM/dd")} 06:00:00', 'yyyy/mm/dd hh24:mi:ss') and
                                    to_date('{today.ToString("yyyy/MM/dd")} 05:59:59', 'yyyy/mm/dd hh24:mi:ss')
                                    and EXISTS
                                    (SELECT *
                                            FROM R_WO_BASE WW
                                           WHERE WW.WORKORDERNO = SN.WORKORDERNO
                                             AND EXISTS (SELECT *
                                                    FROM C_SKU K
                                                   WHERE WW.SKUNO = K.SKUNO
                                                     AND K.SKU_TYPE = 'MODEL')))a";

                var sqlModeltransfor = sqltransfor + $@"
                                      left join   (select * from (select rr.sn,'' csn,rr.workorderno,rr.skuno,rr.valid_flag from r_sn rr,c_sku ck 
                                                  where rr.skuno=ck.skuno and ck.sku_type='MODEL' and rr.skuno not like '%FVN' and rr.valid_flag !=0 )rp
                                                  union 
                                                  select so.sn,kk.value csn,so.workorderno,so.skuno,so.valid_flag from r_sn so,c_sku bb, r_sn_kp kk 
                                                  where so.skuno=bb.skuno and so.sn=kk.sn  and bb.sku_type='MODEL' 
                                                  and kk.valid_flag=1 and so.valid_flag !=0 and substr(so.skuno,length(so.skuno)-3,3) not in('FVN') and upper(substr( kk.kp_name,1,6)) not in( 'AUTOKP')
                                                  )b
                                                  on (a.sn=b.sn or a.sn=b.csn) and a.workorderno !=b.workorderno where b.workorderno is not null
                                      group by a.workorderno";
                var ModeltransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlModeltransfor);

                var sqlFvntransfor = sqltransfor + $@"
                                    left join r_sn b on a.sn=b.sn and b.skuno like '%-FVN' where b.workorderno is not null
                                    group by a.workorderno";
                var FvntransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlFvntransfor);

                var sqlBtstrafor = sqltransfor + $@"
                                    left join (select el.sn,kl.value,kl.kp_name,el.workorderno,el.skuno from o_order_main ol,r_sn el ,r_sn_kp kl 
                                               where ol.prewo=el.workorderno and el.sn=kl.sn and kl.valid_flag=1 and ol.potype='BTS')b 
                                    on a.sn=b.value where b.workorderno is not null
                                    group by a.workorderno";
                var BtstransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlBtstrafor);

                var sqlCTOtransfor = sqltransfor + $@"
                                    left join (select ee.sn,kk.value,kk.kp_name,ee.workorderno,ee.skuno from o_order_main oo,r_sn ee ,r_sn_kp kk 
                                               where oo.prewo=ee.workorderno and ee.sn=kk.sn and kk.valid_flag=1 and oo.potype='CTO')b 
                                    on a.sn=b.value and upper(b.kp_name) LIKE 'AUTOKP%' where b.workorderno is not null
                                    group by a.workorderno";
                var CtotransforCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlCTOtransfor);

                //DataTable fstSparesTable = new DataTable();
                //if (type.Equals("DOF"))
                //{
                //    fstSparesTable = SFCDB.ORM.Ado.GetDataTable($@"select main_item,sub_item,count(*) as qty from (
                //                        select rlc.main_item,rlc.sub_item,sn.workorderno,rtr.sn,rtr.state,rtr.edit_time,
                //                        row_number() over(partition by rtr.sn order by rtr.edit_time desc ) as rownums  
                //                        from r_test_record rtr,r_sn sn,r_link_control rlc,c_sku k where 
                //                        rtr.sn=sn.sn and sn.skuno=rlc.sub_item and rlc.main_item  =k.skuno
                //                        and rtr.messtation='FST_SPARES'
                //                        and sn.SHIPPED_FLAG=0 and sn.valid_flag=1
                //                        and rlc.control_type='SKU' and rlc.category='LOADING_KEEP_SN' 
                //                        AND K.SKU_TYPE = '{type}') where rownums=1 and (state='PASS' or state='Pass' or state='P')
                //                        group by main_item,sub_item");
                //}
                DebugCode = "30";
                foreach (DataRow row in dtWO.Rows)
                {
                    try
                    {
                        wo = row["WORKORDERNO"] == null ? "" : row["WORKORDERNO"].ToString();
                        workorderQty = row["WORKORDER_QTY"] == null ? "0" : row["WORKORDER_QTY"].ToString();
                        reportRow = reportTable.NewRow();
                        foreach (DataColumn cc in reportTable.Columns)
                        {
                            reportRow[cc.ColumnName] = 0;
                        }

                        reportRow["WORKORDERNO"] = wo;
                        reportRow["CLOSED_FLAG"] = row["CLOSED_FLAG"] == null ? "" : row["CLOSED_FLAG"].ToString();
                        reportRow["SKUNO"] = row["SKUNO"] == null ? "" : row["SKUNO"].ToString();
                        reportRow["DAYS"] = row["DAYS"] == null ? "" : row["DAYS"].ToString();
                        reportRow["VER"] = row["SKU_VER"] == null ? "N/A" : row["SKU_VER"].ToString();
                        reportRow["QTY"] = workorderQty;
                        #region STATION WIP
                        var data = dtSNCount.FindAll(t => t.WORKORDERNO == wo);
                        for (int i = 0; i < data.Count; i++)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(data[i].NEXT_STATION))
                                {
                                    if (data[i].NEXT_STATION.Contains("LOADING"))
                                    {
                                        reportRow["LOADING"] = data[i].QTY;
                                    }
                                    else if (data[i].NEXT_STATION == "WAREHOUSE-SN-OUT"&& type.Equals("MODEL"))
                                    {
                                        //add by hgb 2022.03.14 
                                        reportRow["WHS"] = data[i].QTY;
                                     }
                                    else
                                    {
                                        //if (data[i].NEXT_STATION == "FPCTEST" || data[i].NEXT_STATION == "PICTEST")
                                        //{
                                        //    int t = 0;
                                        //    int.TryParse(reportRow["FCT"].ToString(), out t);
                                        //    reportRow["FCT"] = t + data[i].QTY;
                                        //}
                                        //else
                                        //{
                                        //    reportRow[data[i].NEXT_STATION] = data[i].QTY;
                                        //}
                                        reportRow[data[i].NEXT_STATION] = data[i].QTY;
                                    }
                                }
                                else
                                {

                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        #endregion

                        #region   add by hgb 2022.03.09 CTO_KIT_NEW data
                        if (type.Equals("CTO"))//add by hgb 2022.03.09 CTO_KIT_NEW data
                        {

                            var cotkitdata = dtcotkitCount.FindAll(t => t.WORKORDERNO == wo);
                            if (cotkitdata.Count > 0)
                            {

                                if (!string.IsNullOrEmpty(cotkitdata[0].NEXT_STATION))
                                {
                                    if (cotkitdata[0].NEXT_STATION.Contains("CTO_KIT"))
                                    {
                                        reportRow["KIT"] = cotkitdata[0].PERCENT;
                                    }
                                }
                            }
                        }

                        #endregion   add by hgb 2022.03.09 CTO_KIT_NEW data

                        #region Fail WIP
                        var failcount = failcounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["FailWip"] = (failcount == null ? 0 : failcount.QTY);
                        #endregion

                        #region Repiar WIP
                        var repairCount = repairCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["RepairWip"] = repairCount == null ? 0 : repairCount.QTY;
                        #endregion

                        #region MRB & REWORK
                        var mrbCount = mrbCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["MRB"] = mrbCount == null ? 0 : mrbCount.QTY;
                        Double MrbSum = mrbCount == null ? 0 : mrbCount.QTY;
                        var reworkCount = reworkCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["REWORK"] = reworkCount == null ? 0 : reworkCount.QTY;
                        Double reworkSum = reworkCount == null ? 0 : reworkCount.QTY;
                        try
                        {
                            reworkSum = Convert.ToInt64(reworkSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region ORT數據匯總
                        var ortCount = ortCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["ORT"] = ortCount == null ? 0 : ortCount.QTY;
                        #endregion

                        #region WHS & silverWip
                        double silverWipSum = 0;
                        var WHSCount = WHSCounts.Find(t => t.WORKORDERNO == wo);
                        var SMCount = SMCounts.Find(t => t.WORKORDERNO == wo);
                        var OutSmCount = sqlOutSMs.Find(t => t.WORKORDERNO == wo);
                        if (type.Equals("MODEL"))
                        {
                            var silverWip = silverWipCounts.Find(t => t.WORKORDERNO == wo);
                            reportRow["SilverWip"] = silverWip == null ? 0 : silverWip.QTY;
                            silverWipSum = silverWip == null ? 0 : silverWip.QTY;
                            var CWIP = OutSmCount == null ? 0 : OutSmCount.QTY;
                            reportRow["CWIP"] = CWIP;
                            if (!reportRow["SKUNO"].ToString().EndsWith("FVN"))
                            {

                                var transformation = transformationCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Transformation"] = transformation == null ? 0 : transformation.QTY;

                                var Modeltransfor = ModeltransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->MODEL"] = Modeltransfor == null ? 0 : Modeltransfor.QTY;

                                var Fvntransfor = FvntransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->FVN"] = Fvntransfor == null ? 0 : Fvntransfor.QTY;

                                var Btstransfor = BtstransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->BTS"] = Btstransfor == null ? 0 : Btstransfor.QTY;

                                var Ctotransformation = CtotransforCounts.Find(t => t.WORKORDERNO == wo);
                                reportRow["Bind->CTO"] = Ctotransformation == null ? 0 : Ctotransformation.QTY;

                            }
                            var SM = SMCount == null ? 0 : SMCount.QTY;
                            var JWIP = (WHSCount == null ? 0 : WHSCount.QTY - silverWipSum) - SM;
                            reportRow["JWIP"] = JWIP;
                            reportRow["750 SM"] = SM;//WHSCount == null ? 0 : WHSCount.QTY - silverWipSum;
                        }
                        else if (type.Equals("PCBA"))
                        {
                            reportRow["711 SM"] = WHSCount == null ? 0 : WHSCount.QTY;
                        }
                        else
                        {
                            reportRow["WHS"] = WHSCount == null ? 0 : WHSCount.QTY;
                        }
                        double WhsdSum = WHSCount == null ? 0 : WHSCount.QTY - silverWipSum;
                        try
                        {
                            WhsdSum = Convert.ToInt64(WhsdSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region Scraped
                        var ScrapedCount = dtScraped.Find(t => t.WORKORDERNO == wo);
                        double ScrapedSum = ScrapedCount == null ? 0 : ScrapedCount.QTY;
                        try
                        {
                            ScrapedSum = Convert.ToInt64(ScrapedSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        #region Shipped
                        var ShippedCount = ShippedCounts.Find(t => t.WORKORDERNO == wo);
                        if (type.Equals("MODEL"))
                        {
                            reportRow["750 SHIPPED"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                        }
                        else if (type.Equals("PCBA"))
                        {
                            reportRow["711 SHIPPED"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                        }
                        else if (type.Equals("DOF"))
                        {
                            reportRow["SHIP-OUT"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                        }
                        else
                        {
                            reportRow["SHIPPED"] = ShippedCount == null ? 0 : ShippedCount.QTY;
                        }

                        double ShippedSum = ShippedCount == null ? 0 : ShippedCount.QTY;
                        try
                        {
                            ShippedSum = Convert.ToInt64(ShippedSum.ToString());
                        }
                        catch
                        {

                        }
                        #endregion

                        var loadingCount = loadingCounts.Find(t => t.WORKORDERNO == wo);
                        reportRow["LOADING"] = Convert.ToInt64(workorderQty) - (loadingCount == null ? 0 : loadingCount.QTY);
                        Double WoSum = row["WORKORDER_QTY"] == null ? 0 : Convert.ToInt64(row["WORKORDER_QTY"].ToString());
                        if (type.Equals("MODEL"))
                        {
                            reportRow["BALANCE"] = WoSum - ScrapedSum - MrbSum - reworkSum - ShippedSum - WhsdSum - silverWipSum;
                        }
                        else
                        {
                            reportRow["BALANCE"] = WoSum - ScrapedSum - MrbSum - reworkSum - WhsdSum - ShippedSum;
                        }

                        if (reportRow["CLOSED_FLAG"].ToString() == "1")
                        {
                            var BALANCE_qty = double.Parse(reportRow["BALANCE"].ToString());
                            var LOADING_qty = double.Parse(reportRow["LOADING"].ToString()); //(double)reportRow["LOADING"];
                            var QTY_qty = double.Parse(reportRow["QTY"].ToString()); //(double)reportRow["QTY"];
                            if (BALANCE_qty > 0)
                            {
                                reportRow["QTY"] = QTY_qty - LOADING_qty;
                                reportRow["LOADING"] = 0;
                                reportRow["BALANCE"] = BALANCE_qty - LOADING_qty;
                            }
                        }

                        //if (type.Equals("DOF"))
                        //{
                        //    DataRow[] fstRow = fstSparesTable.Rows.Count > 0 ? fstSparesTable.Select($@" MAIN_ITEM = '{reportRow["SKUNO"].ToString()}'") : null;
                        //    if (fstRow != null)
                        //    {
                        //        if (fstRow.Length > 0)
                        //        {
                        //            reportRow["FST_SPARES"] = Convert.ToInt32(fstRow[0]["QTY"].ToString());
                        //        }
                        //        else
                        //        {
                        //            reportRow["FST_SPARES"] = 0;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        reportRow["FST_SPARES"] = 0;

                        //    }
                        //}
                        reportTable.Rows.Add(reportRow);
                    }
                    catch (Exception ee)
                    {

                    }
                }
                DataRow dr = reportTable.NewRow();
                dr["VER"] = "Total:";
                foreach (var c in reportTable.Columns)
                {
                    //add KIT by hgb 2022.03.09
                    if (c.ToString() != "KIT" && c.ToString() != "WORKORDERNO" && c.ToString() != "CLOSED_FLAG" && c.ToString() != "SKUNO" && c.ToString() != "DAYS" && c.ToString() != "VER")
                    {
                        //工站名帶"-"字符的Compute()方法會報錯，於是加個判斷換個寫法 Edit By ZHB 2020年7月10日13:30:49
                        //dr[c.ToString()] = reportTable.Compute("Sum(" + c.ToString() + ")", "");
                        var o = 0;
                        var res = true;
                        var str = c.ToString().Substring(0, 1);
                        if (!int.TryParse(str, out o))
                        {
                            res = false;
                        }
                        if (!reportTable.Columns.Contains(c.ToString()))
                        {
                            continue;
                        }

                        if (!c.ToString().Contains("-") && !c.ToString().Contains(".") && !c.ToString().Contains("_") && !c.ToString().Contains("(") && !c.ToString().Contains(")") && !res)
                        {
                            dr[c.ToString()] = reportTable.Compute("Sum(" + c.ToString() + ")", "");
                        }
                        else
                        {
                            var temp = 0;
                            for (int i = 0; i < reportTable.Rows.Count; i++)
                            {
                                if (reportTable.Rows[i][c.ToString()].ToString().Length > 0)
                                {
                                    temp = temp + Convert.ToInt32(reportTable.Rows[i][c.ToString()]);
                                }
                            }
                            dr[c.ToString()] = temp.ToString();
                        }
                    }
                }
                reportTable.Rows.Add(dr);
                DebugCode = "40";
                #region 加一行以上面結果最後一行為計算基礎的統計行  do the calculation based on the last row of reportTable
                DataRow lastRow = reportTable.Rows[reportTable.Rows.Count - 1];
                DataRow totalDR = reportTable.NewRow();
                if (type.Equals("PCBA"))
                {
                    int totalSMT = 0;
                    foreach (var smt in pcbaSMTColumns)
                    {
                        if (!lastRow.Table.Columns.Contains(smt))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(lastRow[smt].ToString()))
                        {
                            try
                            {
                                totalSMT += Convert.ToInt32(lastRow[smt].ToString());
                            }
                            catch
                            { }
                        }
                    }
                    totalDR[pcbaSMTColumns.Last()] = totalSMT;

                    int totalPTH = 0;
                    foreach (var pth in pcbaPTHColumns)
                    {
                        if (!lastRow.Table.Columns.Contains(pth))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(lastRow[pth].ToString()))
                        {
                            try
                            {
                                totalPTH += Convert.ToInt32(lastRow[pth].ToString());
                            }
                            catch
                            { }
                        }
                    }
                    totalDR[pcbaPTHColumns.Last()] = totalPTH;
                    DebugCode = "50";
                    int totalICT = 0;
                    foreach (var ict in pcbaICTColumns)
                    {
                        if (!lastRow.Table.Columns.Contains(ict))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(lastRow[ict].ToString()))
                        {
                            try
                            {
                                totalICT += Convert.ToInt32(lastRow[ict].ToString());
                            }
                            catch
                            { }
                        }
                    }
                    totalDR[pcbaICTColumns.Last()] = totalICT;

                    int totalBonepile = 0;
                    foreach (var red in redColumns)
                    {
                        if (!lastRow.Table.Columns.Contains(red))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(lastRow[red].ToString()))
                        {
                            try
                            {
                                totalBonepile += Convert.ToInt32(lastRow[red].ToString());
                            }
                            catch
                            { }
                        }
                    }
                    totalDR[redColumns.Last()] = totalBonepile;
                    reportTable.Rows.Add(totalDR);
                }
                else if (type.Equals("MODEL"))
                {
                    DebugCode = "60";
                    int totalYellow = 0;
                    foreach (var yellow in modelYellowColumns)
                    {
                        if (!lastRow.Table.Columns.Contains(yellow))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(lastRow[yellow].ToString()))
                        {
                            try
                            {
                                totalYellow += Convert.ToInt32(lastRow[yellow].ToString());
                            }
                            catch
                            { }
                        }
                    }
                    totalDR[modelYellowColumns.Last()] = totalYellow;

                    int totalBlue = 0;
                    foreach (var blue in modeBlueColumns)
                    {
                        if (!lastRow.Table.Columns.Contains(blue))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(lastRow[blue].ToString()))
                        {
                            try
                            {
                                totalBlue += Convert.ToInt32(lastRow[blue].ToString());
                            }
                            catch
                            { }
                        }
                    }
                    totalDR[modeBlueColumns.Last()] = totalBlue;

                    int totalRed = 0;
                    foreach (var red in redColumns)
                    {
                        if (!lastRow.Table.Columns.Contains(red))
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(lastRow[red].ToString()))
                        {
                            try
                            {
                                totalRed += Convert.ToInt32(lastRow[red].ToString());
                            }
                            catch
                            { }
                        }
                    }
                    totalDR[redColumns.Last()] = totalRed;
                    reportTable.Rows.Add(totalDR);
                }
                DebugCode = "70";
                #endregion

                return reportTable;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public DataTable GetBacklog()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DateTime today = SFCDB.ORM.GetDate();
                DataTable dt = new DataTable();
                dt.Columns.Add("PARTNO");
                Dictionary<string, DataTable> dictionary = new Dictionary<string, DataTable>();
                for (int i = 0; i < 5; i++)
                {
                    DateTime startDate = today.AddDays(6 * i + i);
                    DateTime endDate = today.AddDays(6 * (i + 1) + i);
                    string sql = "";
                    string week = "";
                    if (i < 4)
                    {
                        sql = $@"select partno,sum(d.requestqty) qty  from r_pre_wo_detail d,o_order_main m,r_wo_base w where 
                                     d.wo = m.prewo and w.workorderno = m.prewo
                                     and m.cancel = 0 and w.closed_flag = 0
                                     and partnotype not in ('PACKAGE', 'COUNTRYLABEL', 'BASE', 'COMPONENTS')
                                     and partno like '7%'
                                     and m.delivery between to_date('{startDate.ToString("yyyy-MM-dd")}', 'yyyy-mm-dd') 
                                     and to_date('{endDate.ToString("yyyy-MM-dd")}','yyyy-mm-dd')
                                     group by partno";
                        week = $@"{(i + 1).ToString()}th({startDate.ToString("dd/MM/yyyy")}-{endDate.ToString("dd/MM/yyyy")})";
                    }
                    else
                    {
                        sql = $@"select partno,sum(d.requestqty) qty  from r_pre_wo_detail d,o_order_main m,r_wo_base w where 
                                     d.wo = m.prewo and w.workorderno = m.prewo
                                     and m.cancel = 0 and w.closed_flag = 0
                                     and partnotype not in ('PACKAGE', 'COUNTRYLABEL', 'BASE', 'COMPONENTS')
                                     and partno like '7%'
                                     and m.delivery > to_date('{startDate.ToString("yyyy-MM-dd")}', 'yyyy-mm-dd')                                     
                                     group by partno";
                        week = $@"{(i + 1).ToString()}th(After {startDate.ToString("dd/MM/yyyy")})";
                    }
                    DataTable data = SFCDB.ORM.Ado.GetDataTable(sql);
                    dictionary.Add(week, data);
                    dt.Columns.Add(week);
                }
                dt.Columns.Add("TOTAL");
                List<string> list = new List<string>();
                foreach (var dic in dictionary)
                {
                    list.AddRange(dic.Value.AsEnumerable().Select(d => d.Field<string>("PARTNO")).Distinct().ToList());
                }
                List<string> partnoList = list.Distinct().OrderBy(r => r).ToList();
                foreach (var partno in partnoList)
                {
                    DataRow row = dt.NewRow();
                    row["PARTNO"] = partno;
                    int partnoTotal = 0;
                    foreach (var dic in dictionary)
                    {
                        row[dic.Key] = dic.Value.Select($@" PARTNO= '{partno}'").Length > 0 ? dic.Value.Select($@" PARTNO= '{partno}'")[0][1].ToString() : "0";
                        partnoTotal += Convert.ToInt32(row[dic.Key].ToString());
                    }
                    row["TOTAL"] = partnoTotal;
                    dt.Rows.Add(row);
                }
                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public DataTable GetPcbaSkuData(DataTable pcbaWOData)
        {

            DataTable skuData = new DataTable();
            foreach (DataColumn column in pcbaWOData.Columns)
            {
                if (!notUse.Contains(column.ColumnName))
                {
                    skuData.Columns.Add(column.ColumnName);
                }
            }

            List<string> skuList = pcbaWOData.AsEnumerable().Select(d => d.Field<string>("SKUNO")).Distinct().ToList().OrderBy(r => r).ToList();
            foreach (string sku in skuList)
            {
                if (string.IsNullOrEmpty(sku))
                {
                    continue;
                }
                DataRow[] skuTotal = pcbaWOData.Select($@" SKUNO= '{sku}'");
                DataRow newRow = skuData.NewRow();
                foreach (DataColumn col in skuData.Columns)
                {
                    if (col.ColumnName.Equals("SKUNO"))
                    {
                        newRow[col.ColumnName] = sku;
                    }
                    else
                    {
                        int total = 0;
                        foreach (DataRow row in skuTotal)
                        {
                            total += Convert.ToInt32(row[col.ColumnName].ToString());
                        }
                        newRow[col.ColumnName] = total;
                    }
                }
                skuData.Rows.Add(newRow);
            }
            #region total row
            DataRow totalRow = skuData.NewRow();
            foreach (DataColumn col in skuData.Columns)
            {
                if (col.ColumnName.Equals("SKUNO"))
                {
                    totalRow[col.ColumnName] = "Total:";
                }
                else
                {

                    int total = 0;
                    for (int i = 0; i < skuData.Rows.Count; i++)
                    {
                        if (skuData.Rows[i][col.ColumnName].ToString().Length > 0)
                        {
                            total += Convert.ToInt32(skuData.Rows[i][col.ColumnName]);
                        }
                    }
                    totalRow[col.ColumnName] = total;
                }
            }
            skuData.Rows.Add(totalRow);
            #endregion
            #region color row
            DataRow colorRow = skuData.NewRow();
            int totalPurple = 0;
            foreach (var purple in pcbaSMTColumns)
            {
                if (!totalRow.Table.Columns.Contains(purple))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(totalRow[purple].ToString()))
                {
                    try
                    {
                        totalPurple += Convert.ToInt32(totalRow[purple].ToString());
                    }
                    catch
                    { }
                }
            }
            colorRow[pcbaSMTColumns.Last()] = totalPurple;

            int totalLightBlue = 0;
            foreach (var Lightblue in pcbaPTHColumns)
            {
                if (!totalRow.Table.Columns.Contains(Lightblue))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(totalRow[Lightblue].ToString()))
                {
                    try
                    {
                        totalLightBlue += Convert.ToInt32(totalRow[Lightblue].ToString());
                    }
                    catch
                    { }
                }
            }
            colorRow[pcbaPTHColumns.Last()] = totalLightBlue;

            int totalLightgreen = 0;
            foreach (var Lightgreen in pcbaICTColumns)
            {
                if (!totalRow.Table.Columns.Contains(Lightgreen))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(totalRow[Lightgreen].ToString()))
                {
                    try
                    {
                        totalLightgreen += Convert.ToInt32(totalRow[Lightgreen].ToString());
                    }
                    catch
                    { }
                }
            }
            colorRow[pcbaICTColumns.Last()] = totalLightgreen;

            int totalRed = 0;
            foreach (var red in redColumns)
            {
                if (!totalRow.Table.Columns.Contains(red))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(totalRow[red].ToString()))
                {

                    totalRed += Convert.ToInt32(totalRow[red].ToString());
                }
            }
            colorRow[redColumns.Last()] = totalRed;
            skuData.Rows.Add(colorRow);
            #endregion

            return skuData;
        }

        public DataTable GetModelSkuData(DataTable modelWOData)
        {
            DataTable skuData = new DataTable();
            foreach (DataColumn column in modelWOData.Columns)
            {
                if (!notUse.Contains(column.ColumnName))
                {
                    skuData.Columns.Add(column.ColumnName);
                }
            }

            List<string> skuList = modelWOData.AsEnumerable().Select(d => d.Field<string>("SKUNO")).Distinct().ToList().OrderBy(r => r).ToList();
            foreach (string sku in skuList)
            {
                if (string.IsNullOrEmpty(sku))
                {
                    continue;
                }
                DataRow[] skuTotal = modelWOData.Select($@" SKUNO= '{sku}'");
                DataRow newRow = skuData.NewRow();
                foreach (DataColumn col in skuData.Columns)
                {
                    if (col.ColumnName.Equals("SKUNO"))
                    {
                        newRow[col.ColumnName] = sku;
                    }
                    else
                    {
                        int total = 0;
                        foreach (DataRow row in skuTotal)
                        {
                            total += Convert.ToInt32(row[col.ColumnName].ToString());
                        }
                        newRow[col.ColumnName] = total;
                    }
                }
                skuData.Rows.Add(newRow);
            }
            #region total row
            DataRow totalRow = skuData.NewRow();
            foreach (DataColumn col in skuData.Columns)
            {
                if (col.ColumnName.Equals("SKUNO"))
                {
                    totalRow[col.ColumnName] = "Total:";
                }
                else
                {

                    int total = 0;
                    for (int i = 0; i < skuData.Rows.Count; i++)
                    {
                        if (skuData.Rows[i][col.ColumnName].ToString().Length > 0)
                        {
                            total += Convert.ToInt32(skuData.Rows[i][col.ColumnName]);
                        }
                    }
                    totalRow[col.ColumnName] = total;
                }
            }
            skuData.Rows.Add(totalRow);
            #endregion
            #region color row
            DataRow colorRow = skuData.NewRow();
            int totalYellow = 0;
            foreach (var yellow in modelYellowColumns)
            {
                if (!string.IsNullOrEmpty(totalRow[yellow].ToString()))
                {
                    totalYellow += Convert.ToInt32(totalRow[yellow].ToString());
                }
            }
            colorRow[modelYellowColumns.Last()] = totalYellow;

            int totalBlue = 0;
            foreach (var blue in modeBlueColumns)
            {
                if (!string.IsNullOrEmpty(totalRow[blue].ToString()))
                {
                    totalBlue += Convert.ToInt32(totalRow[blue].ToString());
                }
            }
            colorRow[modeBlueColumns.Last()] = totalBlue;

            int totalRed = 0;
            foreach (var red in redColumns)
            {
                if (!string.IsNullOrEmpty(totalRow[red].ToString()))
                {
                    totalRed += Convert.ToInt32(totalRow[red].ToString());
                }
            }
            colorRow[redColumns.Last()] = totalRed;
            skuData.Rows.Add(colorRow);
            #endregion

            return skuData;
        }

        public DataTable GetDofSkuData(DataTable dofWOData)
        {
            DataTable skuData = new DataTable();
            foreach (DataColumn column in dofWOData.Columns)
            {
                if (!notUse.Contains(column.ColumnName))
                {
                    skuData.Columns.Add(column.ColumnName);
                }
            }

            List<string> skuList = dofWOData.AsEnumerable().Select(d => d.Field<string>("SKUNO")).Distinct().ToList().OrderBy(r => r).ToList();
            foreach (string sku in skuList)
            {
                if (string.IsNullOrEmpty(sku))
                {
                    continue;
                }
                DataRow[] skuTotal = dofWOData.Select($@" SKUNO= '{sku}'");
                DataRow newRow = skuData.NewRow();
                foreach (DataColumn col in skuData.Columns)
                {
                    if (col.ColumnName.Equals("SKUNO"))
                    {
                        newRow[col.ColumnName] = sku;
                    }
                    else
                    {
                        int total = 0;
                        foreach (DataRow row in skuTotal)
                        {
                            total += Convert.ToInt32(row[col.ColumnName].ToString());
                        }
                        newRow[col.ColumnName] = total;
                    }
                }
                skuData.Rows.Add(newRow);
            }
            #region total row
            DataRow totalRow = skuData.NewRow();
            foreach (DataColumn col in skuData.Columns)
            {
                if (col.ColumnName.Equals("SKUNO"))
                {
                    totalRow[col.ColumnName] = "Total:";
                }
                else
                {

                    int total = 0;
                    for (int i = 0; i < skuData.Rows.Count; i++)
                    {
                        if (skuData.Rows[i][col.ColumnName].ToString().Length > 0)
                        {
                            total += Convert.ToInt32(skuData.Rows[i][col.ColumnName]);
                        }
                    }
                    totalRow[col.ColumnName] = total;
                }
            }
            skuData.Rows.Add(totalRow);
            #endregion    
            return skuData;
        }
        public DataTable GetHistoryData(DataTable pcbaSkuData, DataTable modelSkuData, DataTable dofSkuData, DataTable ctoSkuData,
            ref DataTable firstPCBAData, ref DataTable secondPCBAData, ref DataTable firstModelData, ref DataTable secondModelData,
            ref DataTable firstDOFData, ref DataTable secondDOFData, ref DataTable firstCTOData, ref DataTable secondCTOData,
            ref DataTable inTable, ref DataTable outTable, ref List<string> testStation)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                //save data to json
                DateTime now = db.ORM.GetDate();
                int clock = now.TimeOfDay.Hours;
                string typeName = "SkuWipReport";
                string pcbaDataName = $@"PCBA_SKU_DATA_{now.ToString("yyyyMMdd")}";
                string modelDataName = $@"MODEL_SKU_DATA_{now.ToString("yyyyMMdd")}";
                //string pcbaDataName = $@"PCBA_SKU_DATA_{now.ToString("20211214")}";
                //string modelDataName = $@"MODEL_SKU_DATA_{now.ToString("20211214")}";

                string dofDataName = $@"DOF_SKU_DATA_{now.ToString("yyyyMMdd")}";
                string ctoDataName = $@"CTO_SKU_DATA_{now.ToString("yyyyMMdd")}";
                if (clock >= 6)
                {

                    if (!db.ORM.Queryable<R_JSON>().Any(t => t.NAME == pcbaDataName && t.TYPE == typeName))
                    {
                        JsonSave.SaveToDB(pcbaSkuData, pcbaDataName, typeName, "SYSTEM", db, "FJZ", true);
                    }
                    if (!db.ORM.Queryable<R_JSON>().Any(t => t.NAME == modelDataName && t.TYPE == typeName))
                    {
                        JsonSave.SaveToDB(modelSkuData, modelDataName, typeName, "SYSTEM", db, "FJZ", true);
                    }

                    if (!db.ORM.Queryable<R_JSON>().Any(t => t.NAME == dofDataName && t.TYPE == typeName))
                    {
                        JsonSave.SaveToDB(dofSkuData, dofDataName, typeName, "SYSTEM", db, "FJZ", true);
                    }
                    if (!db.ORM.Queryable<R_JSON>().Any(t => t.NAME == ctoDataName && t.TYPE == typeName))
                    {
                        JsonSave.SaveToDB(ctoSkuData, ctoDataName, typeName, "SYSTEM", db, "FJZ", true);
                    }
                }
                string firstPCBAName = $@"PCBA_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                string firstModelName = $@"MODEL_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                string firstDOFName = $@"DOF_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                string firstCTOName = $@"CTO_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                if (clock < 6)
                {
                    firstPCBAName = $@"PCBA_SKU_DATA_{now.AddDays(-2).ToString("yyyyMMdd")}";
                    firstModelName = $@"MODEL_SKU_DATA_{now.AddDays(-2).ToString("yyyyMMdd")}";
                    dofDataName = $@"DOF_SKU_DATA_{now.AddDays(-2).ToString("yyyyMMdd")}";
                    ctoDataName = $@"CTO_SKU_DATA_{now.AddDays(-2).ToString("yyyyMMdd")}";

                    pcbaDataName = $@"PCBA_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                    modelDataName = $@"MODEL_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                    firstDOFName = $@"DOF_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                    firstCTOName = $@"CTO_SKU_DATA_{now.AddDays(-1).ToString("yyyyMMdd")}";
                }

                firstPCBAData = JsonSave.GetFromDB<DataTable>(firstPCBAName, typeName, db);
                secondPCBAData = JsonSave.GetFromDB<DataTable>(pcbaDataName, typeName, db);

                firstModelData = JsonSave.GetFromDB<DataTable>(firstModelName, typeName, db);
                secondModelData = JsonSave.GetFromDB<DataTable>(modelDataName, typeName, db);

                firstDOFData = JsonSave.GetFromDB<DataTable>(firstDOFName, typeName, db);
                secondDOFData = JsonSave.GetFromDB<DataTable>(dofDataName, typeName, db);

                firstCTOData = JsonSave.GetFromDB<DataTable>(firstCTOName, typeName, db);
                secondCTOData = JsonSave.GetFromDB<DataTable>(ctoDataName, typeName, db);


                firstPCBAData = firstPCBAData == null ? new DataTable() : firstPCBAData;
                secondPCBAData = secondPCBAData == null ? new DataTable() : secondPCBAData;
                firstModelData = firstModelData == null ? new DataTable() : firstModelData;
                secondModelData = secondModelData == null ? new DataTable() : secondModelData;

                firstDOFData = firstDOFData == null ? new DataTable() : firstDOFData;
                secondDOFData = secondDOFData == null ? new DataTable() : secondDOFData;
                firstCTOData = firstCTOData == null ? new DataTable() : firstCTOData;
                secondCTOData = secondCTOData == null ? new DataTable() : secondCTOData;

                firstPCBAData.TableName = firstPCBAName;
                secondPCBAData.TableName = pcbaDataName;
                firstModelData.TableName = firstModelName;
                secondModelData.TableName = modelDataName;

                firstDOFData.TableName = firstDOFName;
                secondDOFData.TableName = dofDataName;
                firstCTOData.TableName = firstCTOName;
                secondCTOData.TableName = ctoDataName;

                dicHighPriority = new Dictionary<string, List<string>>();
                var categoryList = SFCDB.ORM.Queryable<R_F_CONTROL>()
                    .Where(r => r.FUNCTIONNAME == "HighPrioritySku" && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "SYSTEM")
                    .OrderBy(r => r.CATEGORYDEC, SqlSugar.OrderByType.Asc)
                    .Select(r => r.CATEGORY).ToList().Distinct().ToList();
                foreach (var category in categoryList)
                {
                    var sList = SFCDB.ORM.Queryable<R_F_CONTROL>()
                        .Where(r => r.FUNCTIONNAME == "HighPrioritySku" && r.CATEGORY == category && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM")
                        .OrderBy(r => r.EXTVAL, SqlSugar.OrderByType.Asc).Select(r => r.VALUE).ToList().Distinct()
                        .ToList();
                    dicHighPriority.Add(category, sList);
                }

                List<string> skuList = new List<string>();
                foreach (var item in dicHighPriority)
                {
                    skuList.AddRange((List<string>)item.Value);
                }
                skuList = skuList.Distinct().ToList();

                Juniper.TEST_IN_OUT_Report test_in_out = new Juniper.TEST_IN_OUT_Report();
                test_in_out.DBPools = this.DBPools;
                test_in_out.Init();
                test_in_out.skuList = skuList;
                test_in_out.Inputs.Find(r => r.Name == "Type").Value = "ALL";
                test_in_out.Inputs.Find(r => r.Name == "GroupBy").Value = "SKUNO";
                test_in_out.GetInAndOutData(SFCDB, ref inTable, ref outTable);
                testStation = test_in_out.stationList;



            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }

            return null;
        }

        class StationData
        {
            internal string wo;

            public string WORKORDERNO { get; set; }
            public string NEXT_STATION { get; set; }
            public int QTY { get; set; }

            public string PERCENT { get; set; }
        }
    }
}
