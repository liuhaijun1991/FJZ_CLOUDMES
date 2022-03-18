using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;

namespace MESReport.BaseReport
{
    public class SkuWoReportNew : ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SKUNO = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "N", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        ReportInput SKUTYPE = new ReportInput { Name = "SKUTYPE", InputType = "Select", Value = "PCBA", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "PCBA", "MODEL", "CTO", "OPTICS", "DOF", "VIRTUAL" } };
        ReportInput GROUP_TYPE = new ReportInput { Name = "GROUP_TYPE", InputType = "Select", Value = "ROUTE", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ROUTE", "SKU" } };
        ReportInput SPECIAL = new ReportInput { Name = "SPECIAL", InputType = "Select", Value = "NA", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "NA", "SHOW_MRR_ONLY" } };
        List<string> TestStations = new List<string>() { "ICT","5DX","5DX1","5DX2", "PRESS-FIT","ROBAT-RX","SI-V1","SI-V2","PICTEST","RIT","FST_SPARES","FPCTEST","SLT","SCBTEST","SAT","BFT1","BFT2","RE_FCT","RE_FCT2","SWTEST"};
        List<string> MRR_SKUS
            = new List<string>() {"711-057161","711-057163","711-032349","711-032349","711-032349",
                "711-072924","711-041855","711-055267","711-052892","711-054743","711-054743","711-063413","711-055085","711-072922","711-032384","711-067370",
                "711-077341","711-067372","711-054388","711-084004","711-049978","711-046531","711-077329","711-068803",
                "750-062243", "750-061489", "750-101855", "750-073435", "750-038768", "750-065925", "750-065926", "750-064569",
                "750-078633", "750-070395", "750-063414", "750-067371", "750-052893", "750-043596", "750-073159", "750-099871", "750-049979", "750-083999",
                "750-077340", "750-067373" ,"750-044130","750-073160","750-049457","750-046532","750-061262","750-077330"};



        public SkuWoReportNew()
        {
            Inputs.Add(WO);
            Inputs.Add(SKUNO);
            Inputs.Add(SKUTYPE);
            Inputs.Add(GROUP_TYPE);
            Inputs.Add(CloseFlag);
            Inputs.Add(SPECIAL);
            //  string strGetSn = @"SELECT * FROM R_SN WHERE SN='{0}' OR BOXSN='{0}'";
            //   Sqls.Add("strGetSN", strGetSn);

        }

        public override void Run()
        {
            try
            {
                Dictionary<string, DataTable> Resdts1 = new Dictionary<string, DataTable>();
                Dictionary<string, DataTable> LinkTables1 = new Dictionary<string, DataTable>();
                GetData(ref Resdts1, ref LinkTables1);
                var skuss = Resdts1.Keys.ToArray();

                for (int i = 0; i < skuss.Length; i++)
                {
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(Resdts1[skuss[i]], LinkTables1[skuss[i]]);
                    retTab.Tittle = $@"{skuss[i]} WO WIP";

                    Dictionary<string, List<TableHeader>> tmp = new Dictionary<string, List<TableHeader>>();
                    List<string> tas = new List<string>();
                    for (int j = 0; j < TestStations.Count; j++)
                    {
                        var w = retTab.TableHeaders[0].Find(t => t.title == TestStations[j] + "_W");
                        var f = retTab.TableHeaders[0].Find(t => t.title == TestStations[j] + "_F");
                        var r = retTab.TableHeaders[0].Find(t => t.title == TestStations[j] + "_R");
                        if (w != null)
                        {
                            tas.Add(TestStations[j]);
                            w.colspan = 3;
                            w.title = TestStations[j];
                            w.field = null;
                            retTab.TableHeaders[0].Remove(f);
                            retTab.TableHeaders[0].Remove(r);
                            List<TableHeader> r2 = null;

                            r2 = new List<TableHeader>();
                            //r2 = retTab.TableHeaders[1];
                            TableHeader hW = new TableHeader();
                            hW.title = "W";
                            hW.field = TestStations[j] + "_W";
                            r2.Add(hW);
                            TableHeader hF = new TableHeader();
                            hF.title = "F";
                            hF.field = TestStations[j] + "_F";
                            r2.Add(hF);

                            TableHeader hR = new TableHeader();
                            hR.title = "R";
                            hR.field = TestStations[j] + "_R";
                            r2.Add(hR);

                            tmp.Add(TestStations[j], r2);
                        }
                    }

                    for (int j = 0; j < retTab.TableHeaders[0].Count; j++)
                    {
                        if (tas.Contains(retTab.TableHeaders[0][j].title))
                        {
                            if (retTab.TableHeaders.Count == 1)
                            {
                                var r2 = new List<TableHeader>();
                                retTab.TableHeaders.Add(r2);
                            }
                            var ll = tmp[retTab.TableHeaders[0][j].title];
                            for (int k = 0; k < ll.Count; k++)
                            {
                                retTab.TableHeaders[1].Add(ll[k]);
                            }
                        }
                    }


                    if (retTab.TableHeaders.Count > 1)
                    {
                        for (int j = 0; j < retTab.TableHeaders[0].Count; j++)
                        {
                            if (retTab.TableHeaders[0][j].colspan == 0)
                            {
                                retTab.TableHeaders[0][j].rowspan = 2;
                            }
                        }
                    }
                    retTab.FixedHeader = true;
                    retTab.FixedCol = 1;
                    if (SPECIAL.Value.ToString() != "NA")
                    {
                        retTab.pagination = false;
                    }

                    Outputs.Add(retTab);
                }
                
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message)); 
            }
            return;
            #region delete code
            if (WO.Value == null)
            {
                throw new Exception("WO Can not be null");
            }
            //   string runSql = string.Format(Sqls["strGetSN"], WO.Value.ToString());
            //    RunSqls.Add(runSql);
            string wo = WO.Value.ToString();
            string sku = SKUNO.Value.ToString();
            string skutype = SKUTYPE.Value.ToString();
            string group_type = GROUP_TYPE.Value.ToString();
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
                if (dcc.Rows[0]["CUSTOMER_NAME"].ToString().ToUpper() == "JUNIPER")
                {
                    Sqlwo = $@"SELECT rwb.workorderno ,rwb.skuno,rph.GROUPID, rwb.ROUTE_ID,rwb.WORKORDER_QTY , trunc ( sysdate - rwb.DOWNLOAD_DATE) DAYS,rwb.INPUT_QTY,rwb.FINISHED_QTY,rwb.SKU_VER 
                                FROM R_WO_BASE rwb left join r_pre_wo_head  rph on rwb.workorderno=rph.wo  where 1=1  ";
                    if (wo != "")
                    {
                        Sqlwo += $@" and rwb.WORKORDERNO = '{wo}' ";
                    }
                    if (sku != "")
                    {
                        Sqlwo += $@" and rwb.skuno = '{sku}' ";
                    }
                    if (skutype != "ALL")
                    {
                        Sqlwo += $@"and (select sku_type from c_sku sk where sk.skuno = rwb.skuno) = '{skutype}'";
                    }


                }
                else
                {
                    Sqlwo = $@"SELECT workorderno ,skuno,ROUTE_ID,WORKORDER_QTY , trunc ( sysdate - DOWNLOAD_DATE) DAYS,INPUT_QTY,FINISHED_QTY,SKU_VER FROM R_WO_BASE where 
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

                Sqlwo += " order by rwb.skuno";

                DataTable dtwo = SFCDB.RunSelect(Sqlwo).Tables[0];
                RunSqls.Add(Sqlwo);
                if (dtwo.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    throw new Exception("No Data!");
                    //return;
                }

                List<DataTable> dtroute = new List<DataTable>();
                //string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[0]["route_id"].ToString()}' order by  seq_no";
                for (int i = 0; i < dtwo.Rows.Count; i++)
                {
                    string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[i]["route_id"].ToString()}' order by  seq_no";
                    DataTable dtroute1 = SFCDB.RunSelect(SqlRoute).Tables[0];
                    RunSqls.Add(SqlRoute);
                    dtroute.Add(dtroute1);
                }

                //string SqlStationRoute = $@" SELECT distinct next_station FROM r_sn where REPAIR_FAILED_FLAG <> 1 and(COMPLETED_FLAG = 0 or NEXT_STATION = 'JOBFINISH') and  workorderno = '{wo}'
                //                            MINUS
                //                            select STATION_NAME from c_route_detail where ROUTE_ID='{dtwo.Rows[0]["route_id"].ToString()}'  ";

                List<DataTable> dtstationroute = new List<DataTable>();
                for (int i = 0; i < dtwo.Rows.Count; i++)
                {
                    string SqlStationRoute = $@" SELECT distinct next_station FROM r_sn where REPAIR_FAILED_FLAG <> 1 and valid_flag !=0  and  workorderno = '{dtwo.Rows[0]["workorderno"].ToString()}'
                                            MINUS
                                            select STATION_NAME from c_route_detail where ROUTE_ID='{dtwo.Rows[0]["route_id"].ToString()}'  ";
                    var dtstationroute1 = SFCDB.RunSelect(SqlStationRoute).Tables[0];
                    RunSqls.Add(SqlStationRoute);
                    dtstationroute.Add(dtstationroute1);
                }

                Dictionary<string, DataTable> Resdts = new Dictionary<string, DataTable>();
                Dictionary<string, DataTable> LinkTables = new Dictionary<string, DataTable>();

                var group = "";


                for (int i = 0; i < dtwo.Rows.Count; i++)
                {
                    try
                    {
                        if (group_type == "ROUTE")
                        {
                            group = dtwo.Rows[i]["ROUTE_ID"].ToString();
                            //var rs = SFCDB.RunSelect($@"select * from C_ROUTE where id ='{group}'");
                            var rs = SFCDB.ORM.Queryable<C_ROUTE>().Where(t => t.ID == group).First();
                            if (rs == null)
                            {
                                continue;
                            }
                            else
                            {
                                group = rs.ROUTE_NAME;
                            }

                        }
                        else
                        {
                            group = dtwo.Rows[i]["SKUNO"].ToString();
                        }
                        var skuno = dtwo.Rows[i]["SKUNO"].ToString();
                        sku = skuno;
                        DataTable resdt = null;
                        DataTable linkTable = null;
                        if (!Resdts.Keys.Contains(group))
                        {
                            resdt = new DataTable();
                            linkTable = new DataTable();
                            Resdts.Add(group, resdt);
                            LinkTables.Add(group, linkTable);
                            if (dcc.Rows[0]["CUSTOMER_NAME"].ToString().ToUpper() == "JUNIPER")
                            {
                                resdt.Columns.Add("WORKORDERNO");
                                resdt.Columns.Add("SKUNO");
                                resdt.Columns.Add("GROUPID");
                                resdt.Columns.Add("DAYS");
                                resdt.Columns.Add("VER");
                                resdt.Columns.Add("QTY");
                                resdt.Columns.Add("BALANCE");

                                linkTable.Columns.Add("WORKORDERNO");
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
                                resdt.Columns.Add("SKUNO");
                                resdt.Columns.Add("DAYS");
                                resdt.Columns.Add("VER");
                                resdt.Columns.Add("QTY");
                                resdt.Columns.Add("BALANCE");

                                linkTable.Columns.Add("WORKORDERNO");
                                linkTable.Columns.Add("SKUNO");
                                linkTable.Columns.Add("DAYS");
                                linkTable.Columns.Add("VER");
                                linkTable.Columns.Add("QTY");
                                linkTable.Columns.Add("BALANCE");
                            }


                            for (int j = 0; j < dtroute[i].Rows.Count; j++)
                            {
                                var station = dtroute[i].Rows[j]["STATION_NAME"].ToString();
                                //resdt.Columns.Add(dtroute[i].Rows[j]["STATION_NAME"].ToString());
                                //linkTable.Columns.Add(dtroute[i].Rows[j]["STATION_NAME"].ToString());

                                if (!TestStations.Contains(station))
                                {
                                    resdt.Columns.Add(station);
                                    linkTable.Columns.Add(station);
                                }
                                else
                                {
                                    resdt.Columns.Add(station + "_W");
                                    linkTable.Columns.Add(station + "_W");
                                    resdt.Columns.Add(station + "_F");
                                    linkTable.Columns.Add(station + "_F");
                                    resdt.Columns.Add(station + "_R");
                                    linkTable.Columns.Add(station + "_R");
                                }

                            }

                            for (int j = 0; j < dtstationroute[i].Rows.Count; j++)
                            {
                                if (dtstationroute[i].Rows[0]["next_station"].ToString().Equals("JOBFINISH") || string.IsNullOrEmpty(dtstationroute[i].Rows[0]["next_station"].ToString()))
                                    continue;
                                if (!TestStations.Contains(dtstationroute[i].Rows[i]["next_station"].ToString()))
                                {
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString());
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString());
                                }
                                else
                                {
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_W");
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_W");
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_F");
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_F");
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_R");
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_R");
                                }
                            }

                            //   resdt.Columns.Add("STOCKIN");
                            //   resdt.Columns.Add("JOBFINISH");
                            //resdt.Columns.Add("NA");
                            resdt.Columns.Add("RepairWip");
                            resdt.Columns.Add("MRB");
                            resdt.Columns.Add("REWORK");
                            resdt.Columns.Add("ORT");
                            resdt.Columns.Add("SCRAPED");
                            resdt.Columns.Add("JOBFINISH");

                            //linkTable.Columns.Add("NA");
                            linkTable.Columns.Add("RepairWip");
                            linkTable.Columns.Add("MRB");
                            linkTable.Columns.Add("REWORK");
                            linkTable.Columns.Add("ORT");
                            linkTable.Columns.Add("SCRAPED");
                            linkTable.Columns.Add("JOBFINISH");
                        }

                        wo = dtwo.Rows[i]["WorkOrderNo"].ToString();
                        resdt = Resdts[group];
                        linkTable = LinkTables[group];

                        DataRow drd = resdt.NewRow();
                        DataRow linkDataRow = linkTable.NewRow();
                        resdt.Rows.Add(drd);
                        linkTable.Rows.Add(linkDataRow);
                        if (dcc.Rows[0]["CUSTOMER_NAME"].ToString().ToUpper() == "JUNIPER")
                        {
                            drd["WORKORDERNO"] = wo;
                            drd["SKUNO"] = dtwo.Rows[i]["Skuno"].ToString();
                            drd["GROUPID"] = dtwo.Rows[i]["GROUPID"].ToString();
                            drd["DAYS"] = dtwo.Rows[i]["DAYS"].ToString();
                            drd["VER"] = dtwo.Rows[i]["SKU_VER"].ToString();
                            drd["QTY"] = dtwo.Rows[i]["WORKORDER_QTY"].ToString();
                        }
                        else
                        {
                            drd["WORKORDERNO"] = wo;
                            drd["SKUNO"] = dtwo.Rows[i]["Skuno"].ToString();
                            drd["DAYS"] = dtwo.Rows[i]["DAYS"].ToString();
                            drd["VER"] = dtwo.Rows[i]["SKU_VER"].ToString();
                            drd["QTY"] = dtwo.Rows[i]["WORKORDER_QTY"].ToString();
                        }

                        linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListByWo&RunFlag=1&WO=" + wo + "&EventName=";
                        //string Sqlsncount = $@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where  (REPAIR_FAILED_FLAG <> 1 or REPAIR_FAILED_FLAG is null)
                        //                 and  workorderno = '{wo}'  and NEXT_STATION <>'REWORK' group by NEXT_STATION";
                        string Sqlsncount = $@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where   workorderno = '{wo}'  and NEXT_STATION <>'REWORK' and VALID_FLAG !=0 group by NEXT_STATION";
                        long loadingNum = 0;
                        //long mrbNum = 0;
                        //long OrtNum = 0;
                        //long repairNum = 0;
                        DataTable dtsncont = SFCDB.RunSelect(Sqlsncount).Tables[0];
                        RunSqls.Add(Sqlsncount);
                        for (int j = 0; j < dtsncont.Rows.Count; j++)
                        {
                            if (!TestStations.Contains(dtsncont.Rows[j]["NEXT_STATION"].ToString()))
                            {
                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString()] = dtsncont.Rows[j]["c"].ToString();
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString()] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";

                            }
                            else
                            {
                                var wipcount = int.Parse(dtsncont.Rows[j]["c"].ToString());
                                var failsncontsql = $@"select count(1) 
                                                        from r_sn s 
                                                            where s.workorderno = '{wo}' and s.valid_flag !=0
                                                            and s.next_station = '{dtsncont.Rows[j]["NEXT_STATION"].ToString()}' 
                                                            and  exists(select * from r_test_record t where t.R_sn_ID = s.id and t.messtation = s.next_station and t.state = 'FAIL' ) 
                                                            and (s.REPAIR_FAILED_FLAG <> 1 or s.REPAIR_FAILED_FLAG is null)";

                                DataTable failsncont = SFCDB.RunSelect(failsncontsql).Tables[0];
                                var failcount = int.Parse(failsncont.Rows[0][0].ToString());
                                var repsncontsql = $@"select count(1) from r_sn s where s.workorderno = '{wo}' and s.next_station = '{dtsncont.Rows[j]["NEXT_STATION"].ToString()}' and s.REPAIR_FAILED_FLAG = 1 ";
                                DataTable repsncont = SFCDB.RunSelect(repsncontsql).Tables[0];
                                var repcount = int.Parse(repsncont.Rows[0][0].ToString());


                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_W"] = wipcount - failcount - repcount;
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_W"] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";
                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_F"] = failcount;
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_F"] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";
                                
                                
                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_R"] = repcount;
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_R"] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";


                            }
                        }

                        string SqlRepairCount = $@" select count(1) repaircount from r_sn a where a.REPAIR_FAILED_FLAG = 1 and a.workorderno = '{wo}' and not exists (select * from r_mrb b where a.sn=b.sn and rework_wo is null )";
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
                        double MrbNum = Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());

                        //ORT數據匯總
                        string SqlOrtCount = $@"SELECT COUNT(SN) ortcount fROM (select C.SN  from r_lot_detail a, r_lot_status b,r_sn c
                                where  b.id = a.lot_id and a.sn=c.sn
                                and b.SAMPLE_STATION in('ORT','ORT-FT2')
                                and c.valid_flag=1 and c.next_station not IN ('JOBFINISH','REWORK','SHIPFINISH')--('JOBFINISH','REWORK')
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
                        double scrapedQty = Convert.ToInt64(dtScraped.Rows[0]["scraped_count"].ToString());
                        drd["SCRAPED"] = dtScraped.Rows[0]["scraped_count"].ToString();
                        linkDataRow["SCRAPED"] = (dtScraped.Rows[0]["scraped_count"].ToString() != "0") ? (linkURL + "SCRAPED") : "";

                        //loadingNum = loadingNum + Convert.ToInt64(dtrepaircont.Rows[0]["repaircount"].ToString()) + Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());

                        string sqlReworkCount = $@"select  count(NEXT_STATION) reworkcount from r_sn a where NEXT_STATION='REWORK' and  workorderno = '{wo}' 
                                         and not exists(select * from r_mrb b where a.sn=b.sn and b.rework_wo is null and b.workorderno='{wo}') ";
                        DataTable dtReworkCont = SFCDB.RunSelect(sqlReworkCount).Tables[0];
                        drd["REWORK"] = dtReworkCont.Rows[0]["reworkcount"].ToString();
                        double reworkNum = Convert.ToInt64(dtReworkCont.Rows[0]["reworkcount"].ToString());

                        string sqlLoadingCount = $@"select  count(*) loadingcount from r_sn  where workorderno = '{wo}' ";
                        DataTable dtLoadingCont = SFCDB.RunSelect(sqlLoadingCount).Tables[0];
                        loadingNum = Convert.ToInt64(dtLoadingCont.Rows[0]["loadingcount"].ToString());

                        double woqty = Convert.ToInt64(dtwo.Rows[i]["WORKORDER_QTY"].ToString());
                        double jbFinishQty = 0;
                        try
                        {
                            jbFinishQty = Convert.ToInt64(drd["JOBFINISH"]);
                        }
                        catch
                        {
                        }
                        drd["BALANCE"] = woqty - scrapedQty - reworkNum - MrbNum - jbFinishQty;

                        foreach (DataColumn dc in resdt.Columns)
                        {
                            if (dc.ColumnName.ToString().ToUpper().IndexOf("LOADING") > 0 || dc.ColumnName.ToString().ToUpper().IndexOf("LINK") > -1)
                            {
                                drd[dc.ColumnName.ToString()] = Convert.ToInt64(dtwo.Rows[i]["WORKORDER_QTY"].ToString()) - loadingNum;//2018.9.27 add repairNum by fgg
                            }
                            if (dc.ColumnName.ToString().ToUpper() == "REWORK")
                            {
                                drd[dc.ColumnName.ToString()] = reworkNum;
                                linkDataRow[dc.ColumnName.ToString().ToUpper()] = (reworkNum != 0) ? (linkURL + dc.ColumnName.ToString().ToUpper()) : "";
                            }
                        }

                        
                    }
                    catch (Exception ee)
                    {

                    }

                }

                var skus = Resdts.Keys.ToArray();

                for (int i = 0; i < skus.Length; i++)
                {
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(Resdts[skus[i]], LinkTables[skus[i]]);
                    retTab.Tittle = $@"{skus[i]} WO WIP";

                    Dictionary<string, List<TableHeader>> tmp = new Dictionary<string, List<TableHeader>>();
                    List<string> tas = new List<string>();
                    for (int j = 0; j < TestStations.Count; j++)
                    {
                        var w = retTab.TableHeaders[0].Find(t => t.title == TestStations[j] + "_W");
                        var f = retTab.TableHeaders[0].Find(t => t.title == TestStations[j] + "_F");
                        var r = retTab.TableHeaders[0].Find(t => t.title == TestStations[j] + "_R");
                        if (w != null)
                        {
                            tas.Add(TestStations[j]);
                            w.colspan = 3;
                            w.title = TestStations[j];
                            w.field = null;
                            retTab.TableHeaders[0].Remove(f);
                            retTab.TableHeaders[0].Remove(r);
                            List<TableHeader> r2 = null;
                           
                            r2 = new List<TableHeader>();
                            //r2 = retTab.TableHeaders[1];
                            TableHeader hW = new TableHeader();
                            hW.title = "W";
                            hW.field = TestStations[j] + "_W";
                            r2.Add(hW);
                            TableHeader hF = new TableHeader();
                            hF.title = "F";
                            hF.field = TestStations[j] + "_F";
                            r2.Add(hF);
                            
                            TableHeader hR = new TableHeader();
                            hR.title = "R";
                            hR.field = TestStations[j] + "_R";
                            r2.Add(hR);

                            tmp.Add(TestStations[j], r2);
                        }
                    }

                    for (int j = 0; j < retTab.TableHeaders[0].Count; j++)
                    {
                        if (tas.Contains(retTab.TableHeaders[0][j].title))
                        {
                            if (retTab.TableHeaders.Count == 1)
                            {
                                var r2 = new List<TableHeader>();
                                retTab.TableHeaders.Add(r2);
                            }
                            var ll = tmp[retTab.TableHeaders[0][j].title];
                            for (int k = 0; k < ll.Count; k++)
                            {
                                retTab.TableHeaders[1].Add(ll[k]);
                            }
                        }
                    }


                    if (retTab.TableHeaders.Count > 1)
                    {
                        for (int j = 0; j < retTab.TableHeaders[0].Count; j++)
                        {
                            if (retTab.TableHeaders[0][j].colspan == 0)
                            {
                                retTab.TableHeaders[0][j].rowspan = 2;
                            }
                        }
                    }
                    retTab.FixedHeader = true;
                    retTab.FixedCol = 1;


                    Outputs.Add(retTab);
                }

            }
            catch(Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    try
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    catch
                    { }


                }
            }
            #endregion
        }

        public override void DownFile()
        {
            // 這個暫時沒有用
            try
            {              

                Dictionary<string, DataTable> Resdts1 = new Dictionary<string, DataTable>();
                Dictionary<string, DataTable> LinkTables1 = new Dictionary<string, DataTable>();
                GetData(ref Resdts1, ref LinkTables1);

                string content = MESPubLab.Common.ExcelHelp.SkuWoReportNewExportExcel(Resdts1, TestStations);
                string fileName = "SkuWoReportNew_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));
              
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }            
        }
        private void GetData(ref Dictionary<string, DataTable> Resdts,ref Dictionary<string, DataTable> LinkTables)
        {
            if (WO.Value == null)
            {
                throw new Exception("WO Can not be null");
            }
            //   string runSql = string.Format(Sqls["strGetSN"], WO.Value.ToString());
            //    RunSqls.Add(runSql);
            string wo = WO.Value.ToString();
            string sku = SKUNO.Value.ToString();
            string skutype = SKUTYPE.Value.ToString();
            string group_type = GROUP_TYPE.Value.ToString();
            string special = SPECIAL.Value.ToString();
            string closeflag = CloseFlag.Value.ToString();
            string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListByWo&RunFlag=1&WO=" + wo + "&EventName=";
            ;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sqlcustomer = $@"select*From c_customer";
                DataTable dcc = SFCDB.RunSelect(sqlcustomer).Tables[0];
                string Sqlwo;
                if (dcc.Rows[0]["CUSTOMER_NAME"].ToString().ToUpper() == "JUNIPER")
                {
                    Sqlwo = $@"SELECT rwb.workorderno ,rwb.skuno,rph.GROUPID, rwb.ROUTE_ID,rwb.WORKORDER_QTY , trunc ( sysdate - rwb.DOWNLOAD_DATE) DAYS,rwb.INPUT_QTY,rwb.FINISHED_QTY,rwb.SKU_VER 
                                FROM R_WO_BASE rwb left join r_pre_wo_head  rph on rwb.workorderno=rph.wo  where 1=1  ";
                    if (special == "NA")
                    {
                        if (wo != "")
                        {
                            Sqlwo += $@" and rwb.WORKORDERNO = '{wo}' ";
                        }
                        if (sku != "")
                        {
                            Sqlwo += $@" and rwb.skuno = '{sku}' ";
                        }
                        if (skutype != "ALL")
                        {
                            Sqlwo += $@"and (select sku_type from c_sku sk where sk.skuno = rwb.skuno) = '{skutype}'";
                        }
                    }
                    else if(special == "SHOW_MRR_ONLY")
                    {
                        closeflag = "ALL";
                         var skus = new StringBuilder();
                        for (int i = 0; i < MRR_SKUS.Count; i++)
                        {
                            skus.Append($@"'{MRR_SKUS[i]}'");
                            if (i != MRR_SKUS.Count - 1)
                            {
                                skus.Append ( ",");
                            }
                        }
                        Sqlwo += $@" and rwb.skuno in ({skus.ToString()})";
                    }


                }
                else
                {
                    //Sqlwo = $@"SELECT workorderno ,skuno,ROUTE_ID,WORKORDER_QTY , trunc ( sysdate - DOWNLOAD_DATE) DAYS,INPUT_QTY,FINISHED_QTY,SKU_VER FROM R_WO_BASE where 
                    //       WORKORDERNO = '{wo}' ";

                    //MODIFY BY HGB 2021.11.22 這個sql應該沒人驗證過，跑的有問題
                    Sqlwo = $@"SELECT workorderno ,skuno,ROUTE_ID,WORKORDER_QTY , trunc ( sysdate - DOWNLOAD_DATE) DAYS,INPUT_QTY,FINISHED_QTY,SKU_VER FROM R_WO_BASE rwb where 
                             rwb.WORKORDERNO not like '~%' ";
                    if (wo != "")
                    {
                        Sqlwo += $@" and rwb.WORKORDERNO = '{wo}' ";
                    }
                    if (sku != "")
                    {
                        Sqlwo += $@" and rwb.skuno = '{sku}' ";
                    }
                }

                if (closeflag == "Y")
                {
                    Sqlwo = Sqlwo + " and CLOSED_FLAG = 1";
                }
                else if (closeflag == "N")
                {
                    Sqlwo = Sqlwo + " and CLOSED_FLAG = 0";
                }

                Sqlwo += " order by rwb.skuno";

                DataTable dtwo = SFCDB.RunSelect(Sqlwo).Tables[0];
                RunSqls.Add(Sqlwo);
                if (dtwo.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    throw new Exception("No Data!");
                    //return;
                }

                List<DataTable> dtroute = new List<DataTable>();
                //string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[0]["route_id"].ToString()}' order by  seq_no";
                for (int i = 0; i < dtwo.Rows.Count; i++)
                {
                    string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[i]["route_id"].ToString()}' order by  seq_no";
                    DataTable dtroute1 = SFCDB.RunSelect(SqlRoute).Tables[0];
                    RunSqls.Add(SqlRoute);
                    dtroute.Add(dtroute1);
                }

                //string SqlStationRoute = $@" SELECT distinct next_station FROM r_sn where REPAIR_FAILED_FLAG <> 1 and(COMPLETED_FLAG = 0 or NEXT_STATION = 'JOBFINISH') and  workorderno = '{wo}'
                //                            MINUS
                //                            select STATION_NAME from c_route_detail where ROUTE_ID='{dtwo.Rows[0]["route_id"].ToString()}'  ";

                List<DataTable> dtstationroute = new List<DataTable>();
                for (int i = 0; i < dtwo.Rows.Count; i++)
                {
                    string SqlStationRoute = $@" SELECT distinct next_station FROM r_sn where REPAIR_FAILED_FLAG <> 1 and valid_flag !=0  and  workorderno = '{dtwo.Rows[0]["workorderno"].ToString()}'
                                            MINUS
                                            select STATION_NAME from c_route_detail where ROUTE_ID='{dtwo.Rows[0]["route_id"].ToString()}'  ";
                    var dtstationroute1 = SFCDB.RunSelect(SqlStationRoute).Tables[0];
                    RunSqls.Add(SqlStationRoute);
                    dtstationroute.Add(dtstationroute1);
                }
                var group = "";


                for (int i = 0; i < dtwo.Rows.Count; i++)
                {
                    try
                    {
                        if (group_type == "ROUTE")
                        {
                            group = dtwo.Rows[i]["ROUTE_ID"].ToString();
                            //var rs = SFCDB.RunSelect($@"select * from C_ROUTE where id ='{group}'");
                            var rs = SFCDB.ORM.Queryable<C_ROUTE>().Where(t => t.ID == group).First();
                            if (rs == null)
                            {
                                continue;
                            }
                            else
                            {
                                group = rs.ROUTE_NAME;
                            }

                        }
                        else
                        {
                            group = dtwo.Rows[i]["SKUNO"].ToString();
                        }
                        var skuno = dtwo.Rows[i]["SKUNO"].ToString();
                        sku = skuno;
                        DataTable resdt = null;
                        DataTable linkTable = null;
                        if (!Resdts.Keys.Contains(group))
                        {
                            resdt = new DataTable();
                            linkTable = new DataTable();
                            Resdts.Add(group, resdt);
                            LinkTables.Add(group, linkTable);
                            if (dcc.Rows[0]["CUSTOMER_NAME"].ToString().ToUpper() == "JUNIPER")
                            {
                                resdt.Columns.Add("WORKORDERNO");
                                resdt.Columns.Add("SKUNO");
                                resdt.Columns.Add("GROUPID");
                                resdt.Columns.Add("DAYS");
                                resdt.Columns.Add("VER");
                                resdt.Columns.Add("QTY");
                                resdt.Columns.Add("BALANCE");

                                linkTable.Columns.Add("WORKORDERNO");
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
                                resdt.Columns.Add("SKUNO");
                                resdt.Columns.Add("DAYS");
                                resdt.Columns.Add("VER");
                                resdt.Columns.Add("QTY");
                                resdt.Columns.Add("BALANCE");

                                linkTable.Columns.Add("WORKORDERNO");
                                linkTable.Columns.Add("SKUNO");
                                linkTable.Columns.Add("DAYS");
                                linkTable.Columns.Add("VER");
                                linkTable.Columns.Add("QTY");
                                linkTable.Columns.Add("BALANCE");
                            }


                            for (int j = 0; j < dtroute[i].Rows.Count; j++)
                            {
                                var station = dtroute[i].Rows[j]["STATION_NAME"].ToString();
                                //resdt.Columns.Add(dtroute[i].Rows[j]["STATION_NAME"].ToString());
                                //linkTable.Columns.Add(dtroute[i].Rows[j]["STATION_NAME"].ToString());

                                if (!TestStations.Contains(station))
                                {
                                    resdt.Columns.Add(station);
                                    linkTable.Columns.Add(station);
                                }
                                else
                                {
                                    resdt.Columns.Add(station + "_W");
                                    linkTable.Columns.Add(station + "_W");
                                    resdt.Columns.Add(station + "_F");
                                    linkTable.Columns.Add(station + "_F");
                                    resdt.Columns.Add(station + "_R");
                                    linkTable.Columns.Add(station + "_R");
                                }

                            }

                            for (int j = 0; j < dtstationroute[i].Rows.Count; j++)
                            {
                                if (dtstationroute[i].Rows[0]["next_station"].ToString().Equals("JOBFINISH") || dtstationroute[i].Rows[0]["next_station"].ToString().Equals("SHIPFINISH") || string.IsNullOrEmpty(dtstationroute[i].Rows[0]["next_station"].ToString()))
                                    continue;
                                if (!TestStations.Contains(dtstationroute[i].Rows[i]["next_station"].ToString()))
                                {
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString());
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString());
                                }
                                else
                                {
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_W");
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_W");
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_F");
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_F");
                                    resdt.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_R");
                                    linkTable.Columns.Add(dtstationroute[i].Rows[i]["next_station"].ToString() + "_R");
                                }
                            }

                            //   resdt.Columns.Add("STOCKIN");
                            //   resdt.Columns.Add("JOBFINISH");
                            //resdt.Columns.Add("NA");
                            resdt.Columns.Add("FailWip");
                            resdt.Columns.Add("RepairWip");
                            resdt.Columns.Add("MRB");
                            resdt.Columns.Add("REWORK");
                            resdt.Columns.Add("ORT");
                            resdt.Columns.Add("SCRAPED");
                            resdt.Columns.Add("JOBFINISH");
                            resdt.Columns.Add("WHS(SM)");
                            resdt.Columns.Add("SILVERWIP");//2021.11.23 ADD BY HGB
                            resdt.Columns.Add("TRANSFORMATION");//2021.11.23 ADD BY HGB 
                            //resdt.Columns.Add("JOBFINISH2"); //2021.11.23 ADD BY HGB


                            //linkTable.Columns.Add("NA");
                            linkTable.Columns.Add("FailWip");
                            linkTable.Columns.Add("RepairWip");
                            linkTable.Columns.Add("MRB");
                            linkTable.Columns.Add("REWORK");
                            linkTable.Columns.Add("ORT");
                            linkTable.Columns.Add("SCRAPED");
                            linkTable.Columns.Add("JOBFINISH");
                            linkTable.Columns.Add("WHS(SM)");
                            linkTable.Columns.Add("SILVERWIP");//2021.11.23 ADD BY HGB
                            linkTable.Columns.Add("TRANSFORMATION");//2021.11.23 ADD BY HGB
                            //linkTable.Columns.Add("JOBFINISH2"); //2021.11.23 ADD BY HGB
                        }

                        wo = dtwo.Rows[i]["WorkOrderNo"].ToString();
                        resdt = Resdts[group];
                        linkTable = LinkTables[group];

                        DataRow drd = resdt.NewRow();
                        DataRow linkDataRow = linkTable.NewRow();
                        resdt.Rows.Add(drd);
                        linkTable.Rows.Add(linkDataRow);
                        if (dcc.Rows[0]["CUSTOMER_NAME"].ToString().ToUpper() == "JUNIPER")
                        {
                            drd["WORKORDERNO"] = wo;
                            drd["SKUNO"] = dtwo.Rows[i]["Skuno"].ToString();
                            drd["GROUPID"] = dtwo.Rows[i]["GROUPID"].ToString();
                            drd["DAYS"] = dtwo.Rows[i]["DAYS"].ToString();
                            drd["VER"] = dtwo.Rows[i]["SKU_VER"].ToString();
                            drd["QTY"] = dtwo.Rows[i]["WORKORDER_QTY"].ToString();
                        }
                        else
                        {
                            drd["WORKORDERNO"] = wo;
                            drd["SKUNO"] = dtwo.Rows[i]["Skuno"].ToString();
                            drd["DAYS"] = dtwo.Rows[i]["DAYS"].ToString();
                            drd["VER"] = dtwo.Rows[i]["SKU_VER"].ToString();
                            drd["QTY"] = dtwo.Rows[i]["WORKORDER_QTY"].ToString();
                        }

                        linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListByWo&RunFlag=1&WO=" + wo + "&EventName=";
                        //string Sqlsncount = $@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where  (REPAIR_FAILED_FLAG <> 1 or REPAIR_FAILED_FLAG is null)
                        //                 and  workorderno = '{wo}'  and NEXT_STATION <>'REWORK' group by NEXT_STATION";
                        string Sqlsncount = $@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where   workorderno = '{wo}'  and NEXT_STATION <>'REWORK' and valid_flag !=0 group by NEXT_STATION";
                        long loadingNum = 0;
                        //long mrbNum = 0;
                        //long OrtNum = 0;
                        //long repairNum = 0;
                        DataTable dtsncont = SFCDB.RunSelect(Sqlsncount).Tables[0];
                        RunSqls.Add(Sqlsncount);
                        for (int j = 0; j < dtsncont.Rows.Count; j++)
                        {
                            if (!TestStations.Contains(dtsncont.Rows[j]["NEXT_STATION"].ToString()))
                            {
                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString()] = dtsncont.Rows[j]["c"].ToString();
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString()] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";

                            }
                            else
                            {
                                var wipcount = int.Parse(dtsncont.Rows[j]["c"].ToString());
                                //Check IN Repair But not Check out,update by LHJ
                                var failsncontsql = $@"select count(1) 
                                                        from r_sn s 
                                                            where s.workorderno = '{wo}' 
                                                            and s.next_station = '{dtsncont.Rows[j]["NEXT_STATION"].ToString()}' 
                                                            and  exists(select * from r_test_record t where t.R_sn_ID = s.id and t.messtation = s.next_station and t.state = 'FAIL' ) 
                                                            and (s.REPAIR_FAILED_FLAG <> 1 or s.REPAIR_FAILED_FLAG is null)";

                                DataTable failsncont = SFCDB.RunSelect(failsncontsql).Tables[0];
                                var failcount = int.Parse(failsncont.Rows[0][0].ToString());
                                //Check IN Repair But not Check out,update by LHJ
                                var repsncontsql = $@"select count(1) from r_sn s where s.workorderno = '{wo}' and s.next_station = '{dtsncont.Rows[j]["NEXT_STATION"].ToString()}' and s.REPAIR_FAILED_FLAG = 1 ";

                                DataTable repsncont = SFCDB.RunSelect(repsncontsql).Tables[0];
                                var repcount = int.Parse(repsncont.Rows[0][0].ToString());


                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_W"] = wipcount - failcount - repcount;
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_W"] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";
                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_F"] = failcount;
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_F"] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";


                                drd[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_R"] = repcount;
                                linkDataRow[dtsncont.Rows[j]["NEXT_STATION"].ToString() + "_R"] = (dtsncont.Rows[j]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[j]["NEXT_STATION"].ToString()) : "";


                            }
                        }
                        //sn Fail But not Check IN,update by LHJ
                        string SqlFailCount = $@" select count(1)  Failcount
                                                        from r_sn s 
                                                            where s.workorderno = '{wo}' 
                                                            and  exists(select * from r_test_record t where t.R_sn_ID = s.id and t.messtation = s.next_station and t.state = 'FAIL' ) 
                                                            and (s.REPAIR_FAILED_FLAG <> 1 or s.REPAIR_FAILED_FLAG is null)";
                        DataTable dtFailcount = SFCDB.RunSelect(SqlFailCount).Tables[0];
                        RunSqls.Add(SqlFailCount);
                        drd["FailWip"] = dtFailcount.Rows[0]["Failcount"].ToString();
                        //repairNum = Convert.ToInt64(dtrepaircont.Rows[0]["repaircount"].ToString());
                        linkDataRow["FailWip"] = (dtFailcount.Rows[0]["Failcount"].ToString() != "0") ? (linkURL + "FailWip") : "";

                        //Check IN Repair But not Check out,update by LHJ
                        string SqlRepairCount = $@" select count(1) repaircount from r_sn a where a.REPAIR_FAILED_FLAG = 1 and a.workorderno = '{wo}' and not exists (select * from r_mrb b where a.sn=b.sn and rework_wo is null )";
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
                        double MrbNum = Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());

                        //ORT數據匯總
                        string SqlOrtCount = $@"SELECT COUNT(SN) ortcount fROM (select C.SN  from r_lot_detail a, r_lot_status b,r_sn c
                                where  b.id = a.lot_id and a.sn=c.sn
                                and b.SAMPLE_STATION in('ORT','ORT-FT2')
                                and c.valid_flag=1 and c.next_station not IN ('JOBFINISH','REWORK','SHIPFINISH')--('JOBFINISH','REWORK')
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
                        double scrapedQty = Convert.ToInt64(dtScraped.Rows[0]["scraped_count"].ToString());
                        drd["SCRAPED"] = dtScraped.Rows[0]["scraped_count"].ToString();
                        linkDataRow["SCRAPED"] = (dtScraped.Rows[0]["scraped_count"].ToString() != "0") ? (linkURL + "SCRAPED") : "";

                        //loadingNum = loadingNum + Convert.ToInt64(dtrepaircont.Rows[0]["repaircount"].ToString()) + Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());

                        string sqlReworkCount = $@"select  count(NEXT_STATION) reworkcount from r_sn a where NEXT_STATION='REWORK' and  workorderno = '{wo}' 
                                         and not exists(select * from r_mrb b where a.sn=b.sn and b.rework_wo is null and b.workorderno='{wo}') ";
                        DataTable dtReworkCont = SFCDB.RunSelect(sqlReworkCount).Tables[0];
                        drd["REWORK"] = dtReworkCont.Rows[0]["reworkcount"].ToString();
                        double reworkNum = Convert.ToInt64(dtReworkCont.Rows[0]["reworkcount"].ToString());

                        string sqlLoadingCount = $@"select  count(*) loadingcount from r_sn  where workorderno = '{wo}' and (valid_flag !=0 or next_station ='REWORK')";
                        DataTable dtLoadingCont = SFCDB.RunSelect(sqlLoadingCount).Tables[0];
                        loadingNum = Convert.ToInt64(dtLoadingCont.Rows[0]["loadingcount"].ToString());

                        double woqty = Convert.ToInt64(dtwo.Rows[i]["WORKORDER_QTY"].ToString());
                        double jbFinishQty = 0;
                        try
                        {
                            jbFinishQty = Convert.ToInt64(drd["JOBFINISH"]);
                        }
                        catch
                        {
                        }
                        drd["BALANCE"] = woqty - scrapedQty - reworkNum - MrbNum - jbFinishQty;

                        foreach (DataColumn dc in resdt.Columns)
                        {
                            // CPU_LINK STATION HIDE 
                            //if (dc.ColumnName.ToString().ToUpper().IndexOf("LOADING") > 0 || dc.ColumnName.ToString().ToUpper().IndexOf("LINK") > -1)
                            if (dc.ColumnName.ToString().ToUpper().IndexOf("LOADING") > 0 )
                            {
                                try
                                {
                                    drd[dc.ColumnName.ToString()] = Convert.ToInt64(dtwo.Rows[i]["WORKORDER_QTY"].ToString()) - loadingNum;//2018.9.27 add repairNum by fgg
                                }
                                catch
                                {
                                    drd[dc.ColumnName.ToString()] = 0;
                                }
                                
                            }
                            if (dc.ColumnName.ToString().ToUpper() == "REWORK")
                            {
                                drd[dc.ColumnName.ToString()] = reworkNum;
                                linkDataRow[dc.ColumnName.ToString().ToUpper()] = (reworkNum != 0) ? (linkURL + dc.ColumnName.ToString().ToUpper()) : "";
                            }
                        }

                        #region get SILVERWIP,TRANSFORMATION,JOBFINISH(modify) 
                        //SILVERWIP陪測

                        if (dcc.Rows[0]["CUSTOMER_NAME"].ToString().ToUpper() == "JUNIPER")
                        {

                            string sqlSILVERWIPCount = $@"select  count(*) SILVERWIPCount from r_juniper_silver_wip rsw,r_sn rsn where rsw.sn=rsn.sn and rsw.skuno=rsn.skuno and rsn.workorderno='{wo}' and rsn.valid_flag=1 and rsw.STATE_FLAG=1";
                            DataTable dtSISILVERWIPCount = SFCDB.RunSelect(sqlSILVERWIPCount).Tables[0];
                            Int64 SILVERWIPCount = 0;
                            try
                            {
                                SILVERWIPCount = Convert.ToInt64(dtSISILVERWIPCount.Rows[0]["SILVERWIPCount"].ToString());
                            }
                            catch
                            {

                            }
                                
                            drd["SILVERWIP"] = SILVERWIPCount;

                            //TRANSFORMATION 當扣板出貨的
                            string sqlTRANSFORMATION = $@"SELECT count(*) TRANSFORMATIONCount FROM R_SN A WHERE A.SHIPPED_FLAG=1 AND A.VALID_FLAG=2 AND A.WORKORDERNO='{wo}'
                                AND exists (
                                SELECT 1 FROM R_SN_KP B WHERE B.VALID_FLAG=1 AND B.VALUE=A.SN) ";
                            DataTable dtTRANSFORMATIONCount = SFCDB.RunSelect(sqlTRANSFORMATION).Tables[0];

                            Int64 TRANSFORMATIONCount = 0;
                            try
                            {
                                TRANSFORMATIONCount = Convert.ToInt64(dtTRANSFORMATIONCount.Rows[0]["TRANSFORMATIONCount"].ToString());
                            }
                            catch
                            {

                            }

                            drd["TRANSFORMATION"] = dtTRANSFORMATIONCount.Rows[0]["TRANSFORMATIONCount"].ToString();

                            //drd["JOBFINISH2"] = (Convert.ToInt64(drd["JOBFINISH"]) - SILVERWIPCount - TRANSFORMATIONCount).ToString();
                            drd["WHS(SM)"] = 0;
                            try
                            {
                                drd["WHS(SM)"] = (Convert.ToInt64(drd["JOBFINISH"]) - SILVERWIPCount - TRANSFORMATIONCount).ToString();
                            }
                            catch
                            { }

                            
                        }

                        #endregion
                        
                        
                        

                    }
                    catch (Exception ee)
                    {

                    }

                }
                var keys = Resdts.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    var resdt = Resdts[keys[i]];
                    var linkTable = LinkTables[keys[i]];
                    bool haveGroupid = false;
                    for (int j = 0; j < resdt.Rows.Count; j++)
                    {
                        if (resdt.Rows[j]["GROUPID"] != null && resdt.Rows[j]["GROUPID"].ToString().Trim() != "")
                        {
                            haveGroupid = true;
                            break;
                        }
                    }

                    if (!haveGroupid)
                    {
                        resdt.Columns.Remove("GROUPID");
                        linkTable.Columns.Remove("GROUPID");

                    }

                }
                    
                    
                    

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        
    }
}
