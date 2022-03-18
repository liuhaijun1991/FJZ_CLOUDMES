using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    //SN 信息報表
    public class SNListReport : ReportBase
    {
        ReportInput TYPE = new ReportInput { Name = "TYPE", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = true, ValueForUse = null };
        //ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = true, ValueForUse = null, RefershType= "EveryTime" };
        ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput WO = new ReportInput { Name = "WO", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = true, ValueForUse = null, RefershType = "EveryTime" };
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput STATION = new ReportInput { Name = "STATION", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = true, ValueForUse = null, RefershType = "EveryTime" };
        ReportInput STATUS = new ReportInput { Name = "STATUS", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "PROD", "FAIL", "REPAIR", "RMB", "REWORK", "FG", "SHIPPED", "SILVER_WIP", "SUPER_MARKET", "CWIP" } };
        public SNListReport()
        {
            Inputs.Add(TYPE);
            Inputs.Add(SKUNO);
            Inputs.Add(WO);
            Inputs.Add(STATION);
            Inputs.Add(STATUS);
        }

        public override void Init()
        {
            base.Init();
            if (!this.isCallBack)
            {
                InitInputData();
            }
        }

        public override void Run()
        {

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                //string sql = "SELECT * FROM R_SN S WHERE VALID_FLAG = 1 AND 1=1 {0}";
                string sql = "SELECT * FROM R_SN S WHERE  1=1 {0}";
                string whrstr = "";
                if (TYPE.Value != null && TYPE.Value.ToString() != "" && TYPE.Value.ToString() != "ALL")
                {
                    whrstr = string.Format("AND SKUNO IN(SELECT SKUNO FROM C_SKU WHERE SKU_TYPE='{0}')", TYPE.Value.ToString());
                }
                if (SKUNO.Value != null && SKUNO.Value.ToString() != "" && SKUNO.Value.ToString() != "ALL")
                {
                    whrstr += string.Format(" AND SKUNO='{0}'", SKUNO.Value.ToString().ToUpper().Trim());
                }
                if (WO.Value != null && WO.Value.ToString() != "" && WO.Value.ToString() != "ALL")
                {
                    whrstr += string.Format(" AND WORKORDERNO='{0}'", WO.Value.ToString().ToUpper().Trim());
                }

                if (STATION.Value != null && STATION.Value.ToString() != "" && STATION.Value.ToString() != "ALL")
                {
                    whrstr += string.Format(" AND NEXT_STATION='{0}'", STATION.Value.ToString());
                }
                var status = STATUS.Value.ToString();
                switch (status)
                {
                    case "PROD":
                        //whrstr += " AND S.VALID_FLAG = 1 AND S.COMPLETED_FLAG=0 AND S.SHIPPED_FLAG=0 AND S.REPAIR_FAILED_FLAG=0 ";
                        //whrstr += " AND S.VALID_FLAG = 1 AND S.COMPLETED_FLAG=1 AND S.SHIPPED_FLAG=0 AND S.REPAIR_FAILED_FLAG=0 ";
                        whrstr += " AND S.VALID_FLAG = 1  AND S.SHIPPED_FLAG=0 ";
                        break;
                    case "FAIL":
                        //whrstr += $@"  AND S.VALID_FLAG = 1 
                        //               AND S.COMPLETED_FLAG = 0
                        //               AND S.SHIPPED_FLAG = 0
                        //               AND S.REPAIR_FAILED_FLAG = 1
                        //               AND EXISTS
                        //               (SELECT *
                        //                      FROM R_REPAIR_MAIN RM
                        //                     WHERE RM.SN = S.SN
                        //                       AND S.WORKORDERNO = RM.WORKORDERNO
                        //                       AND RM.CLOSED_FLAG = 0
                        //                       AND NOT EXISTS (SELECT *
                        //                              FROM R_REPAIR_TRANSFER RT
                        //                             WHERE RM.ID = RT.REPAIR_MAIN_ID))";
                        whrstr += $@"  AND  EXISTS(SELECT * FROM R_TEST_RECORD T WHERE T.R_SN_ID = S.ID AND T.MESSTATION = S.NEXT_STATION AND T.STATE = 'FAIL' ) 
                                       AND (S.REPAIR_FAILED_FLAG <> 1 OR S.REPAIR_FAILED_FLAG IS NULL)";
                        break;
                    case "REPAIR":
                        //whrstr += $@"  AND S.VALID_FLAG = 1 
                        //               AND S.COMPLETED_FLAG = 0
                        //               AND S.SHIPPED_FLAG = 0
                        //               AND S.REPAIR_FAILED_FLAG = 1
                        //               AND EXISTS
                        //               (SELECT *
                        //                      FROM R_REPAIR_MAIN RM
                        //                     WHERE RM.SN = S.SN
                        //                       AND S.WORKORDERNO = RM.WORKORDERNO
                        //                       AND RM.CLOSED_FLAG = 0
                        //                       AND EXISTS (SELECT *
                        //                              FROM R_REPAIR_TRANSFER RT
                        //                             WHERE RM.ID = RT.REPAIR_MAIN_ID))";
                        whrstr += $@"  AND S.VALID_FLAG='1' 
                                       AND S.repair_failed_flag = 1
                                       AND NOT EXISTS(SELECT * FROM R_MRB B WHERE S.SN = B.SN AND B.REWORK_WO IS NULL)";
                        break;
                    case "MRB":
                        whrstr += $@"  AND S.VALID_FLAG = 1 
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION='REWORK'
                                       AND EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = B.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)";
                        break;
                    case "REWORK":
                        whrstr += $@"  AND S.VALID_FLAG = 0 
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION='REWORK'
                                       AND EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = S.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)";
                        break;
                    case "FG":
                        string strsql = $@"select * from r_wo_base rw,c_sku csk where rw.skuno=csk.skuno and rw.workorderno='{WO.Value.ToString()}' and csk.sku_type in('PCBA','MODEL')";
                        DataSet DataTab = SFCDB.RunSelect(strsql);
                        if (DataTab.Tables.Count > 0)
                        {
                            whrstr += $@"  AND S.VALID_FLAG = 1 
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION<>'REWORK'
                                       AND NOT EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = S.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)";
                        }
                        else
                        {
                            whrstr += $@"  AND S.VALID_FLAG = 1 
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.STOCK_STATUS = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION<>'REWORK'
                                       AND NOT EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = S.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)";
                        }
                        break;
                    case "SHIPPED":
                        whrstr += " AND S.VALID_FLAG <> 0 AND S.COMPLETED_FLAG=1 AND S.SHIPPED_FLAG=1 AND S.REPAIR_FAILED_FLAG=0 ";
                        break;
                    case "transformation":
                        DateTime today = SFCDB.ORM.GetDate();
                        whrstr += $@" AND S.VALID_FLAG <> 0 and  s.shipped_flag=1
                                               AND EXISTS
                                             (SELECT *
                                                      FROM R_SN_KP KP
                                                     WHERE KP.VALUE = S.SN
                                                       AND KP.VALID_FLAG = '1'
                                                   
                                                       AND KP.EDIT_TIME BETWEEN
                                                           TO_DATE('{today.AddDays(-1).ToString("yyyy/MM/dd")} 06:00:00', 'YYYY/MM/DD HH24:MI:SS') AND
                                                           TO_DATE('{today.ToString("yyyy/MM/dd")} 05:59:59', 'YYYY/MM/DD HH24:MI:SS'))
                                            ";
                        break;
                    case "SILVER_WIP":
                        strsql = $@"select * from r_wo_base rw,c_sku csk where rw.skuno=csk.skuno and rw.workorderno='{WO.Value.ToString()}' and csk.sku_type in('PCBA','MODEL')";
                        DataTab = SFCDB.RunSelect(strsql);
                        if (DataTab.Tables[0].Rows.Count > 0)
                        {
                            whrstr += $@"  AND S.VALID_FLAG <> 0 
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION<>'REWORK'
                                       AND NOT EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = S.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)
                                        and EXISTS
                                       (
                                           select * from r_juniper_silver_wip p where p.sn = s.sn and p.state_flag = 1
                                       ) ";
                        }
                        else
                        {
                            whrstr += $@"  AND S.VALID_FLAG <> 0
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.STOCK_STATUS = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION<>'REWORK'
                                       AND NOT EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = S.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)";
                        }
                        break;
                    case "SUPER_MARKET":
                        strsql = $@"select * from r_wo_base rw,c_sku csk where rw.skuno=csk.skuno and rw.workorderno='{WO.Value.ToString()}' and csk.sku_type in('PCBA')";
                        var strsql2 = $@"select * from r_wo_base rw,c_sku csk where rw.skuno=csk.skuno and rw.workorderno='{WO.Value.ToString()}' and csk.sku_type in('MODEL')";
                        DataTab = SFCDB.RunSelect(strsql);
                        var DataTab2 = SFCDB.RunSelect(strsql2);
                        if (DataTab.Tables[0].Rows.Count > 0)
                        {
                            whrstr += $@"  AND S.VALID_FLAG = 1 
                                    AND S.COMPLETED_FLAG = 1
                                    AND S.SHIPPED_FLAG = 0
                                    AND S.NEXT_STATION<>'REWORK'
                                    AND NOT EXISTS(SELECT*
                                            FROM R_MRB B
                                            WHERE B.SN = S.SN
                                            AND B.WORKORDERNO = S.WORKORDERNO)
                                    AND NOT EXISTS
                                    (
                                        select * from r_juniper_silver_wip p where p.sn = s.sn and p.state_flag = 1
                                    ) ";

                        }
                        else if (DataTab2.Tables[0].Rows.Count > 0)
                        {
                            whrstr += $@"  AND S.VALID_FLAG = 1 
                                    AND S.COMPLETED_FLAG = 1
                                    AND S.SHIPPED_FLAG = 0
                                    AND S.NEXT_STATION<>'REWORK'
                                    and EXISTS (select * from r_supermarket mm where mm.r_sn_id = s.id and mm.status = 1 )
                                    AND NOT EXISTS(SELECT*
                                            FROM R_MRB B
                                            WHERE B.SN = S.SN
                                            AND B.WORKORDERNO = S.WORKORDERNO)
                                    AND NOT EXISTS
                                    (
                                        select * from r_juniper_silver_wip p where p.sn = s.sn and p.state_flag = 1
                                    ) ";
                        }
                        else
                        {
                            whrstr += $@"  AND S.VALID_FLAG = 1 
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.STOCK_STATUS = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION<>'REWORK'
                                       AND NOT EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = S.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)";
                        }
                        break;
                    case "JWIP":
                        strsql = $@"select * from r_wo_base rw,c_sku csk where rw.skuno=csk.skuno and rw.workorderno='{WO.Value.ToString()}' and csk.sku_type in('MODEL')";
                        DataTab = SFCDB.RunSelect(strsql);
                        if (DataTab.Tables[0].Rows.Count > 0)
                        {
                            whrstr += $@"  AND S.VALID_FLAG <> 0 
                                    AND S.COMPLETED_FLAG = 1
                                    AND S.SHIPPED_FLAG = 0
                                    AND S.NEXT_STATION<>'REWORK'
                                    and not EXISTS (select * from r_supermarket mm where mm.r_sn_id = s.id and mm.status = 1 )
                                    AND NOT EXISTS(SELECT*
                                            FROM R_MRB B
                                            WHERE B.SN = S.SN
                                            AND B.WORKORDERNO = S.WORKORDERNO)
                                    AND NOT EXISTS
                                    (
                                        select * from r_juniper_silver_wip p where p.sn = s.sn and p.state_flag = 1
                                    ) ";

                        }
                        else
                        {
                            whrstr += $@"  AND S.VALID_FLAG = 1 
                                       AND S.COMPLETED_FLAG = 1
                                       AND S.STOCK_STATUS = 1
                                       AND S.SHIPPED_FLAG = 0
                                       AND S.NEXT_STATION<>'REWORK'
                                       AND NOT EXISTS(SELECT*
                                              FROM R_MRB B
                                             WHERE B.SN = S.SN
                                               AND B.WORKORDERNO = S.WORKORDERNO)";
                        }
                        break;
                    case "CWIP":
                        whrstr += $@" and s.completed_flag=1 
                                        and s.shipped_flag = 0
                                        and s.next_station <> 'REWORK'
                                        and s.valid_flag <> 0
                                        and exists(select* from  r_supermarket m
                                        where s.id = m.r_sn_id
                                        and m.status = 0) 
                                        and EXISTS
                                        (SELECT*
                                                FROM R_WO_BASE WW
                                                WHERE WW.WORKORDERNO = S.WORKORDERNO
                                                AND EXISTS (SELECT*
                                                        FROM C_SKU K
                                                        WHERE WW.SKUNO = K.SKUNO
                                                        AND K.SKU_TYPE = 'MODEL'))
                                        and not exists(select *
                                                from r_supermarket rm
                                                where s.id = rm.r_sn_id
                                                and rm.status = 1)
                                        and not exists(select * from r_juniper_silver_wip w where s.sn = w.sn and w.state_flag = 0)";
                        break;
                    default:
                        break;
                }
                sql = string.Format(sql, whrstr);
                DataSet res = SFCDB.RunSelect(sql);
                ReportTable retTab = new ReportTable();

                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string wolinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&CloseFlag=ALL&WO=" + row["workorderno"].ToString();
                    string snlinkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["SN"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "WORKORDERNO")
                        {
                            linkRow[dc.ColumnName] = wolinkURL;
                        }
                        else if (dc.ColumnName.ToString().ToUpper() == "SN")
                        {
                            linkRow[dc.ColumnName] = snlinkURL;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }

                retTab.LoadData(res.Tables[0], linkTable);
                retTab.Tittle = "SN List";
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

        public override void InputChangeEvent()
        {
            base.InputChangeEvent();
            InitInputData();
        }

        private void InitInputData()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            #region Init Input Data List
            try
            {
                string sqlSkuType = "SELECT DISTINCT SKU_TYPE FROM C_SKU";
                string sqlSku = "SELECT SKUNO FROM C_SKU WHERE 1=1 ";
                string sqlWO = "SELECT WORKORDERNO FROM R_WO_BASE WHERE 1=1 ";
                string sqlStation = "SELECT DISTINCT STATION_NAME FROM C_ROUTE_DETAIL WHERE ROUTE_ID IN(SELECT DISTINCT ROUTE_ID FROM R_WO_BASE) ";

                if (TYPE.Value != null && TYPE.Value.ToString() != "" && TYPE.Value.ToString() != "ALL")
                {
                    sqlSku += $@" AND SKU_TYPE='{TYPE.Value}'";
                    sqlWO += string.Format(" AND SKUNO IN({0})", sqlSku);
                    sqlStation += string.Format(" AND ROUTE_ID IN(SELECT ROUTE_ID FROM R_WO_BASE WHERE SKUNO IN({0}))", sqlSku);
                }

                if (SKUNO.Value != null && SKUNO.Value.ToString() != "" && SKUNO.Value.ToString() != "ALL")
                {
                    sqlWO += $@" AND SKUNO='{SKUNO.Value}'";
                    sqlStation += string.Format(" AND ROUTE_ID IN(SELECT ROUTE_ID FROM R_WO_BASE WHERE SKUNO ='{0}')", SKUNO.Value.ToString());
                }

                if (WO.Value != null && WO.Value.ToString() != "" && WO.Value.ToString() != "ALL")
                {
                    sqlStation += $@" AND ROUTE_ID IN(SELECT ROUTE_ID FROM R_WO_BASE WHERE WORKORDERNO ='{WO.Value}')";
                }

                var skutype = SFCDB.ORM.Ado.SqlQuery<string>(sqlSkuType);
                skutype.Insert(0, "ALL");
                TYPE.ValueForUse = skutype;

                var skulist = SFCDB.ORM.Ado.SqlQuery<string>(sqlSku);
                skulist.Insert(0, "ALL");
                SKUNO.ValueForUse = skulist;

                var wolist = SFCDB.ORM.Ado.SqlQuery<string>(sqlWO);
                wolist.Insert(0, "ALL");
                WO.ValueForUse = wolist;

                var stationlist = SFCDB.ORM.Ado.SqlQuery<string>(sqlStation);
                stationlist.Insert(0, "ALL");
                STATION.ValueForUse = stationlist;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            #endregion
        }

    }
}
