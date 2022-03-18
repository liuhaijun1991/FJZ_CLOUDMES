using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Vertiv
{
    public class VTSkuWipReport : ReportBase
    {
        ReportInput inputSku = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput { Name = "TYPE", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "MODEL", "PCBA" } };

        public VTSkuWipReport()
        {
            this.Inputs.Add(inputSku);
            this.Inputs.Add(inputType);
        }
        public override void Run()
        {
            string sqlSku = "";
            string sqlRoute = $@"select * from  c_control where control_name='SFC_WIP_ROUTE' and control_type='ROUTE'  order by control_level";
            if (inputSku.Value == null || inputSku.Value.ToString() == "")
            {
                sqlSku = $@"SELECT SKUNO, WORKORDERNO ,WORKORDER_QTY,START_STATION, trunc ( sysdate - DOWNLOAD_DATE) DATS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w where closed_flag='0' and substr(w.workorderno,0,1) not in ('~','*','#')";
            }
            else
            {
                sqlSku = $@"SELECT SKUNO, WORKORDERNO ,WORKORDER_QTY,START_STATION, trunc ( sysdate - DOWNLOAD_DATE) DATS,SKU_VER,INPUT_QTY,FINISHED_QTY,ROUTE_ID FROM R_WO_BASE w 
                            where closed_flag='0' and skuno='{inputSku.Value.ToString().Trim().ToUpper()}'";
            }

            if ((inputType.Value != null || inputType.Value.ToString() != "") && inputType.Value.ToString() != "ALL")
            {
                if (inputType.Value.ToString() != "ALL")
                {
                    sqlSku += $@" and (select sku_type from c_sku c where c.skuno = w.skuno ) = '{inputType.Value}'";
                }
                if (inputType.Value.ToString() == "PCBA")
                {
                    sqlRoute = $@"select * from  c_control where control_name='SFC_WIP_ROUTE_PCBA' and control_type='ROUTE'  order by control_level";
                }
                else if (inputType.Value.ToString() == "MODEL")
                {
                    sqlRoute = $@"select * from  c_control where control_name='SFC_WIP_ROUTE_MODEL' and control_type='ROUTE'  order by control_level";
                }
            }




            var wolinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&CloseFlag=N&WO=";
            var snlinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListReport&RunFlag=1";

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
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
                if (dtRoute.Rows.Count == 0)
                {
                    throw new Exception("Wip station not setting!");
                }
                DataTable reportTable = new DataTable();
                DataTable linkTable = new DataTable();
                reportTable.Columns.Add("WORKORDERNO");
                reportTable.Columns.Add("SKUNO");
                reportTable.Columns.Add("DATS");
                reportTable.Columns.Add("VER");
                reportTable.Columns.Add("QTY", typeof(int));
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("DATS");
                linkTable.Columns.Add("VER");
                linkTable.Columns.Add("QTY");
                for (int i = 0; i < dtRoute.Rows.Count; i++)
                {
                    reportTable.Columns.Add(dtRoute.Rows[i]["CONTROL_VALUE"].ToString(), typeof(int));
                    linkTable.Columns.Add(dtRoute.Rows[i]["CONTROL_VALUE"].ToString());
                }
                reportTable.Columns.Add("FailWip", typeof(int));
                reportTable.Columns.Add("RepairWip", typeof(int));
                reportTable.Columns.Add("MRB", typeof(int));
                reportTable.Columns.Add("REWORK", typeof(int));
                reportTable.Columns.Add("ORT", typeof(int));
                reportTable.Columns.Add("JOBFINISH", typeof(int));
                linkTable.Columns.Add("FailWip");
                linkTable.Columns.Add("RepairWip");
                linkTable.Columns.Add("MRB");
                linkTable.Columns.Add("REWORK");
                linkTable.Columns.Add("ORT");
                linkTable.Columns.Add("JOBFINISH");
                DataRow reportRow;
                DataRow linkDataRow;
                string wo;
                string sqlSNCount;
                string sqlFailCount;
                string sqlRepairCount;
                string sqlMrbCount;
                string sqlOrtCount;
                string workorderQty;
                long loadingNum;
                //long mrbNum;
                //long OrtNum;
                //long repairNum;
                DataTable dtSNCount;
                foreach (DataRow row in dtWO.Rows)
                {
                    try
                    {
                        //loadingNum = 0;
                        //mrbNum = 0;
                        //repairNum = 0;
                        wo = row["WORKORDERNO"] == null ? "" : row["WORKORDERNO"].ToString();
                        workorderQty = row["WORKORDER_QTY"] == null ? "0" : row["WORKORDER_QTY"].ToString();
                        reportRow = reportTable.NewRow();
                        linkDataRow = linkTable.NewRow();
                        reportRow["WORKORDERNO"] = wo;
                        linkDataRow["WORKORDERNO"] = wolinkURL + wo;
                        reportRow["SKUNO"] = row["SKUNO"] == null ? "" : row["SKUNO"].ToString();
                        reportRow["DATS"] = row["DATS"] == null ? "" : row["DATS"].ToString();
                        reportRow["VER"] = row["SKU_VER"] == null ? "N/A" : row["SKU_VER"].ToString();
                        reportRow["QTY"] = workorderQty;
                        #region STATION WIP
                        sqlSNCount = $@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where  (REPAIR_FAILED_FLAG <> 1 or REPAIR_FAILED_FLAG is null)
                                         and  workorderno = '{wo}' and NEXT_STATION<>'REWORK' group by NEXT_STATION";
                        dtSNCount = SFCDB.RunSelect(sqlSNCount).Tables[0];
                        for (int i = 0; i < dtSNCount.Rows.Count; i++)
                        {
                            reportRow[dtSNCount.Rows[i]["NEXT_STATION"].ToString()] = dtSNCount.Rows[i]["c"].ToString();
                            linkDataRow[dtSNCount.Rows[i]["NEXT_STATION"].ToString()] = snlinkURL + "&WO=" + wo + "&STATION=" + dtSNCount.Rows[i]["NEXT_STATION"].ToString();
                        }
                        #endregion

                        #region Fail WIP
                        sqlFailCount = $@"SELECT COUNT(1) QTY
                                              FROM R_REPAIR_MAIN RM
                                             WHERE NOT EXISTS
                                             (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                                               AND NOT EXISTS
                                             (SELECT *
                                                      FROM R_MRB R
                                                     WHERE RM.SN = R.SN
                                                       AND R.WORKORDERNO = '{wo}')
                                               AND RM.SN IN (SELECT SN
                                                               FROM R_SN S
                                                              WHERE S.REPAIR_FAILED_FLAG = 1
                                                                AND S.WORKORDERNO = '{wo}')
                                               AND RM.CLOSED_FLAG = 0";
                        var failcount = SFCDB.ORM.Ado.SqlQuerySingle<int>(sqlFailCount);
                        reportRow["FailWip"] = failcount;
                        linkDataRow["FailWip"] = snlinkURL + "&WO=" + wo + "&STATUS=FAIL";
                        #endregion

                        #region Repiar WIP
                        //2021.11.10 modfy by fgg 應SI 黃英 要求屏蔽下面計算邏輯，改為跟 WoReport中的計算邏輯一致
                        //sqlRepairCount = $@"SELECT COUNT(1) QTY
                        //                      FROM R_REPAIR_MAIN RM
                        //                     WHERE EXISTS
                        //                     (SELECT * FROM R_REPAIR_TRANSFER RT WHERE RM.ID = RT.REPAIR_MAIN_ID)
                        //                       AND NOT EXISTS
                        //                     (SELECT *
                        //                              FROM R_MRB R
                        //                             WHERE RM.SN = R.SN
                        //                               AND R.WORKORDERNO = '{wo}')
                        //                       AND RM.SN IN (SELECT SN
                        //                                       FROM R_SN S
                        //                                      WHERE S.REPAIR_FAILED_FLAG = 1
                        //                                        AND S.WORKORDERNO = '{wo}')
                        //                       AND RM.CLOSED_FLAG = 0";
                        //2021.11.10 modfy by fgg 應SI 黃英 要求屏蔽上面面計算邏輯，改為跟 WoReport中的計算邏輯一致

                        sqlRepairCount = $@" select count(1) repaircount from r_sn a where a.valid_flag=1 and a.REPAIR_FAILED_FLAG = 1 and a.workorderno = '{wo}' and  not exists (select * from r_mrb b where a.sn=b.sn and rework_wo is null )"; ;

                        var repairCount = SFCDB.ORM.Ado.SqlQuerySingle<int>(sqlRepairCount);
                        reportRow["RepairWip"] = repairCount;
                        linkDataRow["RepairWip"] = snlinkURL + "&WO=" + wo + "&STATUS=REPAIR";
                        #endregion

                        #region MRB & REWORK
                        sqlMrbCount = $@"SELECT COUNT(1) MRBCOUNT
                                              FROM R_MRB
                                             WHERE WORKORDERNO = '{wo}'
                                               AND REWORK_WO IS NULL";
                        var mrbCount = SFCDB.ORM.Ado.SqlQuerySingle<int>(sqlMrbCount);
                        reportRow["MRB"] = mrbCount;
                        linkDataRow["MRB"] = snlinkURL + "&WO=" + wo + "&STATUS=MRB";

                        string sqlReworkCount = $@"SELECT COUNT(NEXT_STATION) REWORKCOUNT
                                                      FROM R_SN A
                                                     WHERE NEXT_STATION = 'REWORK'
                                                       AND WORKORDERNO = '{wo}'
                                                       AND NOT EXISTS (SELECT *
                                                              FROM R_MRB B
                                                             WHERE A.SN = B.SN
                                                               AND B.REWORK_WO IS NULL
                                                               AND B.WORKORDERNO = '{wo}')";
                        var reworkCount = SFCDB.ORM.Ado.SqlQuerySingle<int>(sqlReworkCount);
                        reportRow["REWORK"] = reworkCount;
                        linkDataRow["REWORK"] = snlinkURL + "&WO=" + wo + "&STATUS=REWORK";
                        #endregion

                        #region ORT數據匯總
                        sqlOrtCount = $@"select count(1) ortcount from r_lot_detail a, r_lot_status b,r_sn c
                                    where  b.id = a.lot_id and a.sn=c.sn
                                    and b.SAMPLE_STATION in('ORT','ORT-FT2')
                                    and c.valid_flag=1
                                    and not EXISTS(select a2.sn from r_test_record a2 where a2.sn=a.sn and a2.testation in('ORT','ORT-FT2') and a2.state='PASS' 
                                    and a2.endtime >(select max(CREATE_DATE) from r_lot_detail a3 where a3.SN=a.sn  ) ) and b.skuno ='{wo}' and c.SHIPPED_FLAG=0";
                        DataTable dtOrtcont = SFCDB.RunSelect(sqlOrtCount).Tables[0];
                        reportRow["ORT"] = dtOrtcont.Rows[0]["ortcount"].ToString();
                        //OrtNum = Convert.ToInt64(dtOrtcont.Rows[0]["ortcount"].ToString());
                        #endregion

                        string sqlLoadingCount = $@"select  count(*) loadingcount from r_sn  where workorderno = '{wo}' ";
                        DataTable dtLoadingCont = SFCDB.RunSelect(sqlLoadingCount).Tables[0];
                        loadingNum = Convert.ToInt64(dtLoadingCont.Rows[0]["loadingcount"].ToString());

                        string route_id = row["ROUTE_ID"].ToString();

                        MESDataObject.Module.C_ROUTE_DETAIL firstStation = SFCDB.ORM.Queryable<MESDataObject.Module.C_ROUTE_DETAIL>()
                            .Where(r => r.ROUTE_ID == route_id).OrderBy(r => r.SEQ_NO, SqlSugar.OrderByType.Asc).ToList().FirstOrDefault();
                        if (firstStation == null)
                        {
                            //throw new Exception($@"{wo} Route Error!");
                        }
                        else
                        {
                            reportRow[firstStation.STATION_NAME] = Convert.ToInt64(workorderQty) - loadingNum;
                        }
                        //reportRow[row["START_STATION"].ToString().ToUpper()] = Convert.ToInt64(workorderQty) - loadingNum;                 
                        //取路由中的第一個工站，避免重工工單的起始工站不是路由中的第一個工站導致報表顯示異常

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
                    if (c.ToString() != "WORKORDERNO" && c.ToString() != "SKUNO" && c.ToString() != "DATS" && c.ToString() != "VER")
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

                        if (!c.ToString().Contains("-") && !c.ToString().Contains(".") && !c.ToString().Contains("_") && !res)
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
                report.Tittle = "WO WIP";
                //report.Rows.Add(totalRow);
                Outputs.Add(report);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                DBPools["SFCDB"].Return(SFCDB);
                ReportAlart alart = new ReportAlart(e.ToString());
                Outputs.Add(alart);
                return;
            }

        }
    }
}
