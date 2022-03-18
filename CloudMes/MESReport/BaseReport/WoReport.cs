using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    //查詢工單信息
    public class WoReport:ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL","N","Y" } };

       public WoReport()
        {
            Inputs.Add(WO);
            Inputs.Add(CloseFlag);
            //  string strGetSn = @"SELECT * FROM R_SN WHERE SN='{0}' OR BOXSN='{0}'";
            //   Sqls.Add("strGetSN", strGetSn);

        }

        public override void Run()
        {
            if (WO.Value == null)
            {
                throw new Exception("WO Can not be null");
            }
            //   string runSql = string.Format(Sqls["strGetSN"], WO.Value.ToString());
            //    RunSqls.Add(runSql);
            string wo = WO.Value.ToString().ToUpper().Trim();
           
            string columnName = "";
            string closeflag = CloseFlag.Value.ToString();
            string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListByWo&RunFlag=1&WO=" + wo + "&EventName=";
                                                 ;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sqlcustomer = $@"select*From c_customer";
                DataTable dcc = SFCDB.RunSelect(sqlcustomer).Tables[0];
                string Sqlwo;
                if (dcc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
                {
                    Sqlwo = $@"SELECT rwb.workorderno,case when rwb.closed_flag='1' then 'Y' else 'N' end as CloseFlag, rwb.skuno,rph.GROUPID, rwb.ROUTE_ID,rwb.WORKORDER_QTY , trunc ( sysdate - rwb.DOWNLOAD_DATE) DAYS,rwb.INPUT_QTY,rwb.FINISHED_QTY,rwb.SKU_VER 
                                FROM R_WO_BASE rwb left join r_pre_wo_head  rph on rwb.workorderno=rph.wo  where  rwb.WORKORDERNO = '{wo}' ";
                }
                else
                {
                    Sqlwo = $@" SELECT workorderno ,case when closed_flag=1 then 'Y' else 'N' end as CloseFlag, skuno,ROUTE_ID,WORKORDER_QTY , trunc ( sysdate - DOWNLOAD_DATE) DAYS,INPUT_QTY,FINISHED_QTY,SKU_VER FROM R_WO_BASE where 
                           WORKORDERNO = '{wo}' ";
                }
                    
                if (closeflag == "Y")
                {
                    Sqlwo = Sqlwo + " and CLOSED_FLAG = 1";
                }
                else if (closeflag == "N")
                {
                    Sqlwo = Sqlwo + " and CLOSED_FLAG = 0";
                }
                DataTable dtwo = SFCDB.RunSelect(Sqlwo).Tables[0];
                RunSqls.Add(Sqlwo);
                if (dtwo.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    throw new Exception("No Data!");
                    //return;
                }
                //string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[0]["route_id"].ToString()}' order by  seq_no";
                string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[0]["route_id"].ToString()}' order by  seq_no";
                DataTable dtroute = SFCDB.RunSelect(SqlRoute).Tables[0];
                RunSqls.Add(SqlRoute);

                //string SqlStationRoute = $@" SELECT distinct next_station FROM r_sn where REPAIR_FAILED_FLAG <> 1 and(COMPLETED_FLAG = 0 or NEXT_STATION = 'JOBFINISH') and  workorderno = '{wo}'
                //                            MINUS
                //                            select STATION_NAME from c_route_detail where ROUTE_ID='{dtwo.Rows[0]["route_id"].ToString()}'  ";

                //現在存在進維修後未修出來就直接掃MRB的情況，這裡把REPAIR_FAILED_FLAG條件去掉吧
                string SqlStationRoute = $@" SELECT distinct next_station FROM r_sn where /*REPAIR_FAILED_FLAG <> 1  and*/  workorderno = '{wo}' and next_station not like 'REVERSE%'
                                            MINUS
                                            select STATION_NAME from c_route_detail where ROUTE_ID='{dtwo.Rows[0]["route_id"].ToString()}'  ";
                DataTable dtstationroute = SFCDB.RunSelect(SqlStationRoute).Tables[0];
                RunSqls.Add(SqlStationRoute);

                DataTable resdt = new DataTable();
                DataTable linkTable = new DataTable();

                if (dcc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
                {
                    resdt.Columns.Add("WORKORDERNO");
                    resdt.Columns.Add("CLOSEFLAG");
                    resdt.Columns.Add("SKUNO");
                    resdt.Columns.Add("GROUPID");
                    resdt.Columns.Add("DAYS");
                    resdt.Columns.Add("VER");
                    resdt.Columns.Add("QTY");
                    resdt.Columns.Add("BALANCE");

                    linkTable.Columns.Add("WORKORDERNO");
                    linkTable.Columns.Add("CLOSEFLAG");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("GROUPID");
                    linkTable.Columns.Add("DAYS");
                    linkTable.Columns.Add("VER");
                    linkTable.Columns.Add("QTY");
                    linkTable.Columns.Add("BALANCE");
                }
                else
                {
                    resdt.Columns.Add("WORKORDERNO");
                    resdt.Columns.Add("CLOSEFLAG");
                    resdt.Columns.Add("SKUNO");
                    resdt.Columns.Add("DAYS");
                    resdt.Columns.Add("VER");
                    resdt.Columns.Add("QTY");
                    resdt.Columns.Add("BALANCE");

                    linkTable.Columns.Add("WORKORDERNO");
                    linkTable.Columns.Add("CLOSEFLAG");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("DAYS");
                    linkTable.Columns.Add("VER");
                    linkTable.Columns.Add("QTY");
                    linkTable.Columns.Add("BALANCE");
                }
                

                for (int i = 0; i < dtroute.Rows.Count; i++)
                {
                    resdt.Columns.Add(dtroute.Rows[i]["STATION_NAME"].ToString());
                    linkTable.Columns.Add(dtroute.Rows[i]["STATION_NAME"].ToString());
                }

                for (int i = 0; i < dtstationroute.Rows.Count; i++)
                {
                    if (dtstationroute.Rows[i]["next_station"].ToString().Equals("JOBFINISH") || string.IsNullOrEmpty(dtstationroute.Rows[i]["next_station"].ToString()))
                        continue;
                    resdt.Columns.Add(dtstationroute.Rows[i]["next_station"].ToString());
                    linkTable.Columns.Add(dtstationroute.Rows[i]["next_station"].ToString());
                }

                //   resdt.Columns.Add("STOCKIN");
                //   resdt.Columns.Add("JOBFINISH");
                //resdt.Columns.Add("NA");
                resdt.Columns.Add("RepairWip");
                resdt.Columns.Add("MRB");
                //resdt.Columns.Add("REWORK");
                resdt.Columns.Add("ORT");
                resdt.Columns.Add("SCRAPED");
                resdt.Columns.Add("JOBFINISH");

                //linkTable.Columns.Add("NA");
                linkTable.Columns.Add("RepairWip");
                linkTable.Columns.Add("MRB");
                //linkTable.Columns.Add("REWORK");
                linkTable.Columns.Add("ORT");
                linkTable.Columns.Add("SCRAPED");
                linkTable.Columns.Add("JOBFINISH");

                DataRow drd = resdt.NewRow();
                DataRow linkDataRow = linkTable.NewRow();

                if (dcc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
                {
                    drd["WorkOrderNo"] = wo;
                    drd["CloseFlag"] = dtwo.Rows[0]["CloseFlag"].ToString();
                    drd["Skuno"] = dtwo.Rows[0]["Skuno"].ToString();
                    drd["GROUPID"] = dtwo.Rows[0]["GROUPID"].ToString();
                    drd["DAYS"] = dtwo.Rows[0]["DAYS"].ToString();
                    drd["VER"] = dtwo.Rows[0]["SKU_VER"].ToString();
                    drd["QTY"] = dtwo.Rows[0]["WORKORDER_QTY"].ToString();
                }
                else
                {
                    drd["WorkOrderNo"] = wo;
                    drd["CloseFlag"] = dtwo.Rows[0]["CloseFlag"].ToString();
                    drd["Skuno"] = dtwo.Rows[0]["Skuno"].ToString();
                    drd["DAYS"] = dtwo.Rows[0]["DAYS"].ToString();
                    drd["VER"] = dtwo.Rows[0]["SKU_VER"].ToString();
                    drd["QTY"] = dtwo.Rows[0]["WORKORDER_QTY"].ToString();
                }

                //  drd["STOCKIN"]= dtwo.Rows[0]["FINISHED_QTY"].ToString();

                //string Sqlsncount =$@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where  (REPAIR_FAILED_FLAG <> 1 or REPAIR_FAILED_FLAG is null)
                //                        and(COMPLETED_FLAG = 0 or NEXT_STATION = 'JOBFINISH') 
                //                         and  workorderno = '{wo}' group by NEXT_STATION";
                //
                //string Sqlsncount = $@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where  (REPAIR_FAILED_FLAG <> 1 or REPAIR_FAILED_FLAG is null)
                //                         and  workorderno = '{wo}' and valid_flag='1' and NEXT_STATION <>'REWORK' group by NEXT_STATION";
                string Sqlsncount = $@" select a.NEXT_STATION, count(a.NEXT_STATION)c from r_sn a where  (a.REPAIR_FAILED_FLAG <> 1 or a.REPAIR_FAILED_FLAG is null)
                                         and  a.workorderno = '{wo}' and a.NEXT_STATION <>'REWORK' and a.next_station not like 'REVERSE%'  and not exists(select*From r_ort b  where a.sn=b.sn AND A.ID = B.ID) group by a.NEXT_STATION";
                long loadingNum = 0;
                //long mrbNum = 0;
                //long OrtNum = 0;
                //long repairNum = 0;
                DataTable dtsncont = SFCDB.RunSelect(Sqlsncount).Tables[0];
                RunSqls.Add(Sqlsncount);
                for (int i = 0; i < dtsncont.Rows.Count; i++)
                {
                    drd[dtsncont.Rows[i]["NEXT_STATION"].ToString()] = dtsncont.Rows[i]["c"].ToString();
                    linkDataRow[dtsncont.Rows[i]["NEXT_STATION"].ToString()] = (dtsncont.Rows[i]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[i]["NEXT_STATION"].ToString()) : "";
                    //loadingNum = loadingNum + Convert.ToInt64(dtsncont.Rows[i]["c"].ToString());
                }

                //string SqlRepairCount = $@" select count(1) repaircount from r_sn a where a.REPAIR_FAILED_FLAG = 1 and a.workorderno = '{wo}' and  not exists (select * from r_mrb b where a.sn=b.sn and rework_wo is null )";
                //不改這裡，改超鏈接進去查詢維修數量的語句
                string SqlRepairCount = $@" select count(1) repaircount from r_sn a where a.valid_flag=1 and a.REPAIR_FAILED_FLAG = 1 and a.workorderno = '{wo}' and  not exists (select * from r_mrb b where a.sn=b.sn and rework_wo is null )";
                //string SqlRepairCount = $@"select count(1) repaircount
                //                                      from r_sn a
                //                                     where a.REPAIR_FAILED_FLAG = 1
                //                                       and a.workorderno = '{wo}'
                //                                       and not exists (select *
                //                                              from r_mrb b
                //                                             where a.sn = b.sn) ";

                DataTable dtrepaircont = SFCDB.RunSelect(SqlRepairCount).Tables[0];
                RunSqls.Add(SqlRepairCount);
                drd["RepairWip"] = dtrepaircont.Rows[0]["repaircount"].ToString();
                //repairNum = Convert.ToInt64(dtrepaircont.Rows[0]["repaircount"].ToString());
                linkDataRow["RepairWip"] = (dtrepaircont.Rows[0]["repaircount"].ToString() != "0") ? (linkURL + "RepairWip") : "";

                string SqlMrbCount = $@"select count(1) mrbcount from r_mrb where workorderno = '{wo}'  and rework_wo is null";
                //string SqlMrbCount = $@"select count(1) mrbcount from r_mrb where workorderno = '{wo}' ";
                DataTable dtmrbcont = SFCDB.RunSelect(SqlMrbCount).Tables[0];
                RunSqls.Add(SqlMrbCount);
                drd["MRB"] = dtmrbcont.Rows[0]["mrbcount"].ToString();
                //mrbNum = Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());
                linkDataRow["MRB"] = (dtmrbcont.Rows[0]["mrbcount"].ToString() != "0") ? (linkURL + "MRB") : "";

                //ORT數據匯總
                string SqlOrtCount = $@"SELECT COUNT(SN) ortcount fROM (select C.SN  from r_lot_detail a, r_lot_status b,r_sn c
                                where  b.id = a.lot_id and a.sn=c.sn
                                and b.SAMPLE_STATION in('ORT','ORT-FT2')
                                and c.valid_flag=1 and c.next_station not IN ('JOBFINISH','REWORK')
                                and not EXISTS(select a2.sn from r_test_record a2 where a2.sn=a.sn and a2.testation in('ORT','ORT-FT2') and a2.state='PASS' 
                                and a2.endtime >(select max(CREATE_DATE) from r_lot_detail a3 where a3.SN=a.sn  ) ) and b.skuno ='{wo}'
								UNION ALL 
                                select  A.SN  From r_sn a,r_ort b where a.sn=b.sn  and valid_flag='1' and a.workorderno='{wo}' and b.ORTEVENT='ORTIN' AND A.workorderno=B.workorderno 
                                and NOT EXISTS(SELECT*fROM R_ORT C WHERE B.SN=C.SN AND C.ORTEVENT='ORTOUT'  ))	";
                DataTable dtOrtcont = SFCDB.RunSelect(SqlOrtCount).Tables[0];
                RunSqls.Add(SqlOrtCount);
                drd["ORT"] = dtOrtcont.Rows[0]["ortcount"].ToString();
                //OrtNum = Convert.ToInt64(dtOrtcont.Rows[0]["ortcount"].ToString());
                linkDataRow["ORT"] = (dtOrtcont.Rows[0]["ortcount"].ToString() != "0") ? (linkURL + "ORT") : "";

                string sqlScraped = $@"select count(*) as scraped_count from r_sn where workorderno='{wo}' and scraped_flag='1' and valid_flag='1'";
                DataTable dtScraped = SFCDB.RunSelect(sqlScraped).Tables[0];
                RunSqls.Add(sqlScraped);
                drd["SCRAPED"] = dtScraped.Rows[0]["scraped_count"].ToString();
                linkDataRow["SCRAPED"] = (dtScraped.Rows[0]["scraped_count"].ToString() != "0") ? (linkURL + "SCRAPED") : "";

                //loadingNum = loadingNum + Convert.ToInt64(dtrepaircont.Rows[0]["repaircount"].ToString()) + Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());

                string sqlReworkCount = $@"select  count(NEXT_STATION) reworkcount from r_sn a where NEXT_STATION='REWORK' and  workorderno = '{wo}' 
                                         and not exists(select * from r_mrb b where a.sn=b.sn and b.rework_wo is null and b.workorderno='{wo}') ";
                DataTable dtReworkCont = SFCDB.RunSelect(sqlReworkCount).Tables[0];
                //drd["REWORK"] = dtReworkCont.Rows[0]["reworkcount"].ToString();

                double woqty = Convert.ToInt64(dtwo.Rows[0]["WORKORDER_QTY"].ToString());
                double reworkNum = Convert.ToInt64(dtReworkCont.Rows[0]["reworkcount"].ToString());
                double scrapedQty = Convert.ToInt64(dtScraped.Rows[0]["scraped_count"].ToString());
                double jbFinishQty = 0;
                try
                {
                    jbFinishQty = Convert.ToInt64(drd["JOBFINISH"]);
                }
                catch
                {

                }
                drd["BALANCE"] = woqty - reworkNum - scrapedQty - jbFinishQty;

                string sqlLoadingCount = $@"select  count(*) loadingcount from r_sn  where workorderno = '{wo}' and next_station not like 'REVERSE%' ";
                DataTable dtLoadingCont = SFCDB.RunSelect(sqlLoadingCount).Tables[0];
                loadingNum = Convert.ToInt64(dtLoadingCont.Rows[0]["loadingcount"].ToString());

                foreach (DataColumn dc in resdt.Columns)
                {
                    if (dc.ColumnName.ToString().ToUpper().IndexOf("LOADING") > 0 || dc.ColumnName.ToString().ToUpper().IndexOf("LINK") > -1)
                    {
                        if(dc.ColumnName.ToString() != "CPU_LINK")
                        {
                            drd[dc.ColumnName.ToString()] = Convert.ToInt64(dtwo.Rows[0]["WORKORDER_QTY"].ToString()) - loadingNum;//2018.9.27 add repairNum by fgg
                        }
                        
                    }
                    if (dc.ColumnName.ToString().ToUpper() == "REWORK")
                    {
                        drd[dc.ColumnName.ToString()] = reworkNum;
                        linkDataRow[dc.ColumnName.ToString().ToUpper()] = (reworkNum != 0) ? (linkURL + dc.ColumnName.ToString().ToUpper()) : "";
                    }
                }

                resdt.Rows.Add(drd);
                linkTable.Rows.Add(linkDataRow);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resdt, linkTable);
                retTab.Tittle = "WO WIP";
                //retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                if (resdt.Rows.Count > 0)
                {
                    List<object> objList = new List<object>();
                    pieChart pie = new pieChart();
                    pie.Tittle = "WO " + wo + "WIP Distribution pie chart";//分佈餅狀圖
                    pie.ChartTitle = "Main title";//主標題
                    pie.ChartSubTitle = "Subtitle";//副標題
                    ChartData chartData = new ChartData();
                    chartData.name = "WOLIST";
                    chartData.type = ChartType.pie.ToString();
                    for (int j = 0; j < resdt.Rows.Count; j++)
                    {
                        foreach (DataColumn column in resdt.Columns)
                        {
                            columnName = column.ColumnName.ToString().ToUpper();
                            if (columnName != "WORKORDERNO" && columnName != "CLOSEFLAG" && columnName != "SKUNO" && columnName != "GROUPID" && columnName != "DAYS" && columnName != "QTY" && columnName != "VER" && resdt.Rows[j][columnName].ToString() != "" && resdt.Rows[j][columnName].ToString() != "0")
                            {
                                objList.Add(new List<object> { columnName, Convert.ToInt64(resdt.Rows[j][columnName].ToString()) });
                            }
                        }
                    }
                    chartData.data = objList;
                    chartData.colorByPoint = true;
                    List<ChartData> _ChartDatas = new List<ChartData> { chartData };
                    pie.ChartDatas = _ChartDatas;
                    Outputs.Add(pie);
                }
            }
            catch(Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
    }
}
