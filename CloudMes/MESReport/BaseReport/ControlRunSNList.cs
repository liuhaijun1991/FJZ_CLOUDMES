using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="ControlRunSNList.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>Zhang Wenxiao</author>
    // <date> 2020-05-28 </date>
    /// <summary>
    /// ControlRunSNList
    /// </summary>
    public class ControlRunSNList:ReportBase
    {
        ReportInput inputCONTROLID = new ReportInput() { Name = "CONTROLID", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputQueryTYPE = new ReportInput() { Name = "QueryTYPE", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public ControlRunSNList() {
            Inputs.Add(inputCONTROLID);
            Inputs.Add(inputQueryTYPE);
            Inputs.Add(inputStation);
        }

        public override void Init()
        {
        }

        public override void Run()
        {
         
            string CONTROLID = inputCONTROLID.Value.ToString();
            string QueryTYPE = inputQueryTYPE.Value.ToString();
            string Station= inputStation.Value.ToString();

            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataTable tempTable = new DataTable();
            ReportTable reportTable = new ReportTable();
            DataRow linkRow = null;
            DataRow tempRow = null;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {  if (QueryTYPE=="1") { 
                    sqlRun = $@"SELECT A.CONTROLID,B.SN,B.SKUNO,B.WORKORDERNO,B.NEXT_STATION,C.PANEL
                                FROM R_CONTROLRUN_DETAIL A,R_SN B
                                LEFT JOIN R_PANEL_SN C ON B.SN = C.SN
                                WHERE A.DATA = B.SN
                                AND A.VALID_FLAG = '1'
                                AND B.VALID_FLAG = '1'
                                AND A.CONTROLID = '{CONTROLID}'";
                    RunSqls.Add(sqlRun);
                    snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                    linkTable.Columns.Add("CONTROLID");
                    linkTable.Columns.Add("SN");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("WORKORDERNO");
                    linkTable.Columns.Add("NEXT_STATION");
                    linkTable.Columns.Add("PANEL");
                    for (int i = 0; i < snListTable.Rows.Count; i++)
                    {
                        linkRow = linkTable.NewRow();
                        linkRow["CONTROLID"] = "";
                        linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + snListTable.Rows[i]["SN"].ToString();
                        linkRow["SKUNO"] = "";
                        linkRow["WORKORDERNO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + snListTable.Rows[i]["WORKORDERNO"].ToString();
                        linkRow["NEXT_STATION"] = "";
                        linkRow["PANEL"] = "";
                        linkTable.Rows.Add(linkRow);
                    }
                    reportTable.LoadData(snListTable, linkTable);
                    reportTable.Tittle = "SNList";
                }
                else if (QueryTYPE == "2")
                {
                    // if sn rewrok the sn'id is change
                    //sqlRun = $@"SELECT DISTINCT A.CONTROLID,B.SN,B.SKUNO,B.WORKORDERNO,D.PANEL,B.STATION_NAME
                    //            FROM R_CONTROLRUN_DETAIL A,R_SN_STATION_DETAIL B,R_SN C
                    //            LEFT JOIN R_PANEL_SN D ON C.SN = D.SN
                    //            WHERE   A.DATA=C.SN
                    //            AND C.ID=B.R_SN_ID
                    //            AND A.VALID_FLAG = '1'
                    //            AND B.VALID_FLAG = '1'
                    //            AND C.VALID_FLAG='1'
                    //            AND B.STATION_NAME='{Station}'
                    //            AND A.CONTROLID = '{CONTROLID}'";
                    sqlRun = $@"SELECT DISTINCT A.CONTROLID,B.SN,B.SKUNO,B.WORKORDERNO,D.PANEL,B.STATION_NAME
                                FROM R_CONTROLRUN_DETAIL A,R_SN_STATION_DETAIL B,R_SN C
                                LEFT JOIN R_PANEL_SN D ON C.SN = D.SN
                                WHERE   A.DATA=C.SN
                                AND C.SN=B.SN
                                AND A.VALID_FLAG = '1'
                                AND B.VALID_FLAG = '1'
                                AND C.VALID_FLAG='1'
                                AND B.STATION_NAME='{Station}'
                                AND A.CONTROLID = '{CONTROLID}'";
                    RunSqls.Add(sqlRun);
                    snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                    linkTable.Columns.Add("CONTROLID");
                    linkTable.Columns.Add("SN");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("WORKORDERNO");
                    linkTable.Columns.Add("PANEL");
                    linkTable.Columns.Add("STATION_NAME");
                    for (int i = 0; i < snListTable.Rows.Count; i++)
                    {
                        linkRow = linkTable.NewRow();
                        linkRow["CONTROLID"] = "";
                        linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + snListTable.Rows[i]["SN"].ToString();
                        linkRow["SKUNO"] = "";
                        linkRow["WORKORDERNO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + snListTable.Rows[i]["WORKORDERNO"].ToString(); 
                        linkRow["PANEL"] = "";
                        linkRow["STATION_NAME"] = "";
                        linkTable.Rows.Add(linkRow);
                    }
                    reportTable.LoadData(snListTable, linkTable);
                    reportTable.Tittle = "StationSNList";

                }
                else if (QueryTYPE == "3")
                {
                    // if sn rewrok the sn'id is change
                    //var sqlRunFailSN = $@"SELECT DISTINCT C.SN FROM R_CONTROLRUN_DETAIL A,R_SN_STATION_DETAIL B,R_SN C
                    //                    WHERE A.DATA=C.SN
                    //                    AND C.VALID_FLAG='1'
                    //                    AND A.VALID_FLAG='1'
                    //                    AND B.VALID_FLAG='1'
                    //                    AND C.ID=B.R_SN_ID
                    //                    AND A.CONTROLID='{CONTROLID}'
                    //                    AND B.STATION_NAME='{Station}'
                    //                   AND B.REPAIR_FAILED_FLAG='1'";
                    var sqlRunFailSN = $@"SELECT DISTINCT C.SN FROM R_CONTROLRUN_DETAIL A,R_SN_STATION_DETAIL B,R_SN C
                                        WHERE A.DATA=C.SN
                                        AND C.VALID_FLAG='1'
                                        AND A.VALID_FLAG='1'
                                        AND B.VALID_FLAG='1'
                                        AND C.sn=B.sn
                                        AND A.CONTROLID='{CONTROLID}'
                                        AND B.STATION_NAME='{Station}'
                                        AND B.REPAIR_FAILED_FLAG='1'";
                    RunSqls.Add(sqlRunFailSN);
                    DataTable failSNList = SFCDB.RunSelect(sqlRunFailSN).Tables[0];
                    linkTable.Columns.Add("SN");
                    linkTable.Columns.Add("WORKORDERNO");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("FAIL_LINE");
                    linkTable.Columns.Add("FAIL_STATION");
                    linkTable.Columns.Add("FAIL_TIME");
                    linkTable.Columns.Add("FAIL_CODE");
                    linkTable.Columns.Add("DESCRIPTION");
                    linkTable.Columns.Add("FAIL_LOCATION");
                    linkTable.Columns.Add("FAILREDESC");
                    linkTable.Columns.Add("REPAIR_FLAG");
                    linkTable.Columns.Add("ACTION_CODE");
                    linkTable.Columns.Add("ACTION_NAME");
                    linkTable.Columns.Add("REASON_CODE");
                    linkTable.Columns.Add("REPAIR_LOCATION");
                    linkTable.Columns.Add("REPAIR_TIME");
                    linkTable.Columns.Add("EDIT_EMP");

                    tempTable.Columns.Add("SN");
                    tempTable.Columns.Add("WORKORDERNO");
                    tempTable.Columns.Add("SKUNO");
                    tempTable.Columns.Add("FAIL_LINE");
                    tempTable.Columns.Add("FAIL_STATION");
                    tempTable.Columns.Add("FAIL_TIME");
                    tempTable.Columns.Add("FAIL_CODE");
                    tempTable.Columns.Add("DESCRIPTION");
                    tempTable.Columns.Add("FAIL_LOCATION");
                    tempTable.Columns.Add("FAILREDESC");
                    tempTable.Columns.Add("REPAIR_FLAG");
                    tempTable.Columns.Add("ACTION_CODE");
                    tempTable.Columns.Add("ACTION_NAME");
                    tempTable.Columns.Add("REASON_CODE");
                    tempTable.Columns.Add("REPAIR_LOCATION");
                    tempTable.Columns.Add("REPAIR_TIME");
                    tempTable.Columns.Add("EDIT_EMP");

                    for (int n = 0; n < failSNList.Rows.Count; n++)
                    {
                        sqlRun = $@"SELECT A.SN,
                                       A.WORKORDERNO,
                                       A.SKUNO,
                                       B.FAIL_LINE,
                                       B.FAIL_STATION,
                                       B.FAIL_TIME,
                                       C.FAIL_CODE,
                                       c.DESCRIPTION,
                                       C.FAIL_LOCATION,
                                       E.ENGLISH_DESCRIPTION AS FAILREDESC,
                                       (CASE
                                           WHEN C.REPAIR_FLAG = '1' THEN 'PASS'
                                           WHEN C.REPAIR_FLAG = '0' THEN 'FAIL'
                                        END)
                                          REPAIR_FLAG,
                                       D.ACTION_CODE,
                                       D.DESCRIPTION AS ACTION_NAME,
                                       D.REASON_CODE,
                                       D.FAIL_LOCATION REPAIR_LOCATION,
                                       D.REPAIR_TIME,
                                       D.EDIT_EMP
                                  FROM R_SN A
                                       LEFT JOIN R_REPAIR_MAIN B
                                          ON     A.SN = B.SN
                                             AND A.WORKORDERNO = B.WORKORDERNO
                                             AND B.FAIL_STATION = '{Station}'
                                       LEFT JOIN R_REPAIR_FAILCODE C ON B.ID = C.REPAIR_MAIN_ID
                                       LEFT JOIN R_REPAIR_ACTION D ON C.ID = D.REPAIR_FAILCODE_ID
                                       LEFT JOIN C_ERROR_CODE E ON  D.REASON_CODE=E.ERROR_CODE
                                 WHERE A.VALID_FLAG = '1' AND A.SN='{failSNList.Rows[n]["SN"].ToString()}'";
                        RunSqls.Add(sqlRun);
                        snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                        for (int i = 0; i < snListTable.Rows.Count; i++)
                        {
                            linkRow = linkTable.NewRow();
                            linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + snListTable.Rows[i]["SN"].ToString();
                            linkRow["SKUNO"] = "";
                            linkRow["WORKORDERNO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + snListTable.Rows[i]["WORKORDERNO"].ToString();
                            linkRow["FAIL_LINE"] = "";
                            linkRow["FAIL_STATION"] = "";
                            linkRow["FAIL_CODE"] = "";
                            linkRow["DESCRIPTION"] = "";
                            linkRow["FAILREDESC"] = "";
                            linkRow["REPAIR_FLAG"] = "";
                            linkRow["ACTION_CODE"] = "";
                            linkRow["ACTION_NAME"] = "";
                            linkRow["REASON_CODE"] = "";
                            linkRow["FAIL_LOCATION"] = "";
                            linkRow["REPAIR_LOCATION"] = "";
                            linkRow["EDIT_EMP"] = "";
                            linkRow["REPAIR_TIME"] = "";
                            linkTable.Rows.Add(linkRow);
                            tempRow = tempTable.NewRow();
                            tempRow["SN"] = snListTable.Rows[i]["SN"];
                            tempRow["SKUNO"] = snListTable.Rows[i]["SKUNO"];
                            tempRow["WORKORDERNO"] = snListTable.Rows[i]["WORKORDERNO"];
                            tempRow["FAIL_LINE"] = snListTable.Rows[i]["FAIL_LINE"];
                            tempRow["FAIL_STATION"] = snListTable.Rows[i]["FAIL_STATION"];
                            tempRow["FAIL_CODE"] = snListTable.Rows[i]["FAIL_CODE"];
                            tempRow["DESCRIPTION"] = snListTable.Rows[i]["DESCRIPTION"];
                            tempRow["FAILREDESC"] = snListTable.Rows[i]["FAILREDESC"];
                            tempRow["REPAIR_FLAG"] = snListTable.Rows[i]["REPAIR_FLAG"];
                            tempRow["ACTION_CODE"] = snListTable.Rows[i]["ACTION_CODE"];
                            tempRow["ACTION_NAME"] = snListTable.Rows[i]["ACTION_NAME"];
                            tempRow["REASON_CODE"] = snListTable.Rows[i]["REASON_CODE"];
                            tempRow["FAIL_LOCATION"] = snListTable.Rows[i]["FAIL_LOCATION"];
                            tempRow["REPAIR_LOCATION"] = snListTable.Rows[i]["REPAIR_LOCATION"];
                            tempRow["EDIT_EMP"] = snListTable.Rows[i]["EDIT_EMP"];
                            tempRow["FAIL_TIME"] = snListTable.Rows[i]["FAIL_TIME"];
                            tempRow["REPAIR_TIME"] = snListTable.Rows[i]["REPAIR_TIME"];
                            tempTable.Rows.Add(tempRow);
                        }
                        
                    }
                    reportTable.LoadData(tempTable, linkTable);
                    reportTable.Tittle = "FailSNList";

                }
                
               
                Outputs.Add(reportTable);

            }

            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
