using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    class Structural_Test_Yield : ReportBase
    {
        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportInput inputStationName = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput() { Name = "Type", InputType = "Select", Value = "First_Pass_Yield", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "First_Pass_Yield", "Pass_Yield" ,"True_Failure_Yield"} };
        ReportInput inputModule = new ReportInput() { Name = "Module", InputType = "Select", Value = "SKU", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "SKU", "MODULE" } };
        ReportInput inputSKU = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "Workorderno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        string sqlGetSation = "";
        public Structural_Test_Yield()
        {
            Inputs.Add(inputStartDate);
            Inputs.Add(inputEndDate);
            Inputs.Add(inputStationName);
            Inputs.Add(inputType);
            Inputs.Add(inputModule);
            Inputs.Add(inputSKU);
            Inputs.Add(inputWO);
            sqlGetSation = " select DISTINCT MES_STATION from c_temes_station_mapping where mes_station not in ('SI_V1','SI_V2') order by MES_STATION";
            Sqls.Add("GetSation", sqlGetSation);

        }
        public override void Init()
        {
            inputStartDate.Value = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd");
            inputEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd");
            inputStationName.ValueForUse = GetStation();
        }
        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string station = inputStationName.Value.ToString();
                string type = inputType.Value.ToString();
                string wo = inputWO.Value.ToString();
                string skuno = inputSKU.Value.ToString();
                string module = inputModule.Value.ToString();
                DateTime sTime = Convert.ToDateTime(inputStartDate.Value);
                DateTime eTime = Convert.ToDateTime(inputEndDate.Value).AddDays(1);
                string sValue = sTime.ToString("yyyy-MM-dd");
                string eValue = eTime.ToString("yyyy-MM-dd");
                var TeStation = ""; var MesStation = "";
                string strSku = string.Empty;
                if (wo == "")
                {
                    strSku = $@"SELECT RSK.SKUNO,TO_CHAR(REPLACE(DECODE (TO_CHAR(WM_CONCAT(RMM.CUSTPN)),'','',TO_CHAR(WM_CONCAT(RMM.CUSTPN))),',','')) BASEPID,RSK.ROUTE_ID FROM ( SELECT DISTINCT CSK.SKUNO,RSR.ROUTE_ID FROM R_TEST_JUNIPER RTJ,C_SKU CSK,R_SKU_ROUTE RSR WHERE RTJ.PART_NUMBER=CSK.SKUNO AND CSK.ID=RSR.SKU_ID";
                }
                else 
                {
                    strSku = $@"SELECT RSK.SKUNO,RMM.CUSTPN BASEPID,RSK.ROUTE_ID FROM ( SELECT DISTINCT CSK.SKUNO,RSR.ROUTE_ID FROM R_TEST_JUNIPER RTJ,C_SKU CSK,R_SKU_ROUTE RSR ,R_WO_BASE RWB WHERE RTJ.PART_NUMBER=CSK.SKUNO AND CSK.ID=RSR.SKU_ID AND CSK.SKUNO=RWB.SKUNO";
                }
                if (sValue != null && eValue != null)
                {
                    strSku = strSku + $@" AND RTJ.TATIME>TO_DATE('{sValue}','yyyy/MM/dd') AND RTJ.TATIME<TO_DATE('{eValue}','yyyy/MM/dd')";
                }
                if (skuno != "" && module == "SKU")
                {
                    strSku = strSku + $@" AND CSK.SKUNO='{skuno}'";
                }
                if (skuno != "" && module == "MODULE")
                {
                    strSku = strSku + $@" AND CSK.SKUNO LIKE '{skuno}%'";
                }
                if (station != "ALL")
                {
                    string SqlTestation = $@" SELECT DISTINCT TE_STATION,MES_STATION FROM C_TEMES_STATION_MAPPING CSM WHERE MES_STATION='{station}'  ";
                    DataTable DtTestation = new DataTable();
                    DtTestation = SFCDB.RunSelect(SqlTestation).Tables[0];
                    TeStation = DtTestation.Rows[0]["TE_STATION"].ToString();
                    MesStation = DtTestation.Rows[0]["MES_STATION"].ToString();
                    strSku = strSku + $@" AND RTJ.EVENTNAME='{TeStation}'";
                }
                else
                {
                    strSku = strSku + $@" AND RTJ.EVENTNAME !='DBG'";
                }
                if ( wo != "")
                {
                    strSku = strSku + $@" AND RWB.WORKORDERNO='{wo}'";
                }
                strSku = strSku + $@")RSK LEFT JOIN R_MODELSUBPN_MAP RMM ON RSK.SKUNO = RMM.SUBPARTNO AND RMM.FLAG=1 GROUP BY RSK.SKUNO,RMM.CUSTPN,RSK.ROUTE_ID ";
                DataTable Dtsku = SFCDB.RunSelect(strSku).Tables[0];
                RunSqls.Add(strSku);
                if (Dtsku.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    throw new Exception("No Data!");
                    //return;
                }

                List<DataTable> dtroute = new List<DataTable>();
                DataTable dtstationroute1 = new DataTable();
                string SqlStationRoute = String.Empty;

                for (int i = 0; i < Dtsku.Rows.Count; i++)
                {
                    SqlStationRoute = $@"SELECT DISTINCT MES_STATION,CRD.SEQ_NO FROM C_TEMES_STATION_MAPPING CSM,C_ROUTE_DETAIL CRD 
                                         WHERE CSM.MES_STATION=CRD.STATION_NAME AND ROUTE_ID='{Dtsku.Rows[i]["route_id"].ToString()}' and CSM.MES_STATION NOT IN ('SI_V1','SI_V2') ORDER BY CRD.SEQ_NO ";
                    dtstationroute1 = SFCDB.RunSelect(SqlStationRoute).Tables[0];
                    RunSqls.Add(SqlStationRoute);
                    dtroute.Add(dtstationroute1);
                }

                Dictionary<string, DataTable> Resdts = new Dictionary<string, DataTable>();
                Dictionary<string, DataTable> LinkTables = new Dictionary<string, DataTable>();
                var group = "";

                for (int i = 0; i < Dtsku.Rows.Count; i++)
                {
                    try
                    {
                        if (dtroute[i].Rows.Count != 0)
                        {
                            if (station == "ALL")
                            {
                                group = Dtsku.Rows[i]["ROUTE_ID"].ToString();
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
                                group = TeStation;
                            }
                            var sku = Dtsku.Rows[i]["SKUNO"].ToString();
                            DataTable resdt = new DataTable();
                            DataTable linkTable = new DataTable();
                            if (!Resdts.Keys.Contains(group))
                            {
                                Resdts.Add(group, resdt);
                                LinkTables.Add(group, linkTable);
                                resdt.Columns.Add("SKUNO");
                                linkTable.Columns.Add("SKUNO");
                                resdt.Columns.Add("BASEPID");
                                linkTable.Columns.Add("BASEPID");
                                string station_name = string.Empty;
                                if (wo != "")
                                {
                                    resdt.Columns.Add("WO");
                                    linkTable.Columns.Add("WO");
                                }
                                if (station == "ALL")
                                {
                                    for (int j = 0; j < dtroute[i].Rows.Count; j++)
                                    {
                                        station_name = dtroute[i].Rows[j]["MES_STATION"].ToString();
                                        resdt.Columns.Add(station_name + "_Test");
                                        linkTable.Columns.Add(station_name + "_Test");
                                        resdt.Columns.Add(station_name + "_Fail");
                                        linkTable.Columns.Add(station_name + "_Fail");
                                        resdt.Columns.Add(station_name + "_Yield");
                                        linkTable.Columns.Add(station_name + "_Yield");
                                    }
                                }
                                else
                                {
                                    resdt.Columns.Add(station + "_Test");
                                    linkTable.Columns.Add(station + "_Test");
                                    resdt.Columns.Add(station + "_Fail");
                                    linkTable.Columns.Add(station + "_Fail");
                                    resdt.Columns.Add(station + "_Yield");
                                    linkTable.Columns.Add(station + "_Yield");
                                }
                            }
                            resdt = Resdts[group];
                            linkTable = LinkTables[group];

                            DataRow drd = resdt.NewRow();
                            DataRow linkDataRow = linkTable.NewRow();
                            resdt.Rows.Add(drd);
                            linkTable.Rows.Add(linkDataRow);
                            drd["SKUNO"] = Dtsku.Rows[i]["SKUNO"].ToString();
                            drd["BASEPID"] = Dtsku.Rows[i]["BASEPID"].ToString();
                            if (wo != "")
                            {
                                drd["WO"] = wo;
                            }

                            //The total number of query tests
                            for (int k = 0; k < dtroute[i].Rows.Count; k++)
                            {
                                if (station != "ALL")
                                {
                                    if (station != dtroute[i].Rows[k]["MES_STATION"].ToString())
                                    {
                                        continue;
                                    }
                                }


                                string TestCount = string.Empty;
                                string linkURL = string.Empty;
                                if (wo == "")
                                {
                                    TestCount = $@"SELECT CTS.MES_STATION ,COUNT(DISTINCT SYSSERIALNO)C
                                                FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                                                WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                AND RTJ.EVENTNAME=CTS.TE_STATION 
                                                AND CSKU.ID=RSR.SKU_ID
                                                AND CRD.STATION_NAME=CTS.MES_STATION
                                                AND RTJ.SYSSERIALNO=RSN.SN
                                                /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                                AND CSKU.SKUNO = '{sku}' 
                                                AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{dtroute[i].Rows[k]["MES_STATION"]}')";
                                    linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.Structural_Test_Yield_Detail&RunFlag=1&StationName=" + dtroute[i].Rows[k]["MES_STATION"] + "&Skuno=" + sku + "";
                                }
                                else
                                {
                                    TestCount = $@"SELECT CTS.MES_STATION ,COUNT(DISTINCT SYSSERIALNO)C
                                                FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_WO_BASE RWO,R_SN RSN
                                                WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                AND RTJ.EVENTNAME=CTS.TE_STATION 
                                                AND RTJ.SYSSERIALNO=RSN.SN
                                                AND CSKU.ID=RSR.SKU_ID
                                                AND CRD.STATION_NAME=CTS.MES_STATION
                                                AND RWO.WORKORDERNO=RSN.WORKORDERNO
                                                AND RWO.SKUNO=CSKU.SKUNO
                                                /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                                AND RWO.WORKORDERNO = '{wo}' 
                                                AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{dtroute[i].Rows[k]["MES_STATION"]}')";
                                    linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.Structural_Test_Yield_Detail&RunFlag=1&StationName=" + dtroute[i].Rows[k]["MES_STATION"] + "&Workorderno=" + wo + "";
                                }
                                if (sValue != null && eValue != null)
                                {
                                    TestCount = TestCount + $@"  AND RTJ.TATIME> TO_DATE('{sValue}', 'yyyy/MM/dd') AND RTJ.TATIME<TO_DATE('{eValue}','yyyy/MM/dd')";
                                    linkURL = linkURL + "&StartDate=" + sValue + "&EndDate=" + eValue + "";
                                }
                                TestCount = TestCount + $@" GROUP BY CTS.MES_STATION";
                                linkURL = linkURL + "&Type=";
                                DataTable DtTestCont = SFCDB.RunSelect(TestCount).Tables[0];
                                RunSqls.Add(TestCount);
                                var StrFailSQL = "";
                                var StrTimeSQL = $@"";

                                if (DtTestCont.Rows.Count > 0)
                                {
                                    float TestSnCount = float.Parse(DtTestCont.Rows[0]["c"].ToString());

                                    if (sValue != null && eValue != null)
                                    {
                                        StrTimeSQL = $@" AND RTJ.TATIME> TO_DATE('{sValue}', 'yyyy/MM/dd') AND RTJ.TATIME<TO_DATE('{eValue}','yyyy/MM/dd')";
                                    }

                                    if (type == "First_Pass_Yield")
                                    {
                                        if (wo == "")
                                        {
                                            StrFailSQL = $@"SELECT COUNT(DISTINCT SS.SYSSERIALNO) c FROM (
                                                        SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                        AND RTJ.EVENTNAME=CTS.TE_STATION
                                                        AND RTJ.SYSSERIALNO=RSN.SN
                                                        AND CSKU.ID=RSR.SKU_ID
                                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                                        AND CSKU.SKUNO='{sku}' 
                                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')" + StrTimeSQL;
                                        }
                                        else
                                        {
                                            StrFailSQL = $@"SELECT COUNT(DISTINCT SS.SYSSERIALNO) c FROM (
                                                        SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,RSN.SKUNO,RSN.WORKORDERNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_WO_BASE RWO,R_SN RSN
                                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                                        AND RTJ.SYSSERIALNO=RSN.SN
                                                        AND CSKU.ID=RSR.SKU_ID
                                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                                        AND RWO.WORKORDERNO=RSN.WORKORDERNO
                                                        AND RWO.SKUNO=CSKU.SKUNO
                                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                                        AND RWO.WORKORDERNO='{wo}' 
                                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')" + StrTimeSQL;
                                        }                                        
                                        StrFailSQL = StrFailSQL + $@" )SS ,C_TEMES_STATUS_MAPPING CTSM WHERE UPPER(SS.STATUS)=UPPER(CTSM.TE_STATUS) AND  SS.RN=1 and UPPER(CTSM.MES_STATUS)='FAIL'";
                                        linkURL = linkURL + "First_Pass_Yield";
                                    }
                                    else if (type == "Pass_Yield")
                                    {
                                        if (wo == "")
                                        {
                                            StrFailSQL = $@"SELECT COUNT(DISTINCT GG.SYSSERIALNO)C FROM (
                                                        SELECT DISTINCT JJ.SKUNO, JJ.SYSSERIALNO,JJ.MES_STATION,AA.STATUS,JJ.TATIME FROM (
                                                        SELECT  CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION,MAX(TATIME) TATIME
                                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                                        AND RTJ.SYSSERIALNO=RSN.SN
                                                        AND CSKU.ID=RSR.SKU_ID
                                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                                        AND CSKU.SKUNO = '{sku}' 
                                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')" 
                                                        + StrTimeSQL + 
                                                        $@" GROUP BY RTJ.SYSSERIALNO,CSKU.SKUNO,CTS.MES_STATION)JJ
                                                                    LEFT JOIN R_TEST_JUNIPER AA ON JJ.SYSSERIALNO=AA.SYSSERIALNO AND JJ.TATIME=AA.TATIME 
                                                                    AND AA.EVENTNAME IN((SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')),
                                                                     C_TEMES_STATUS_MAPPING BB
                                                                     WHERE UPPER(AA.STATUS) = UPPER(BB.TE_STATUS)
                                                                       AND UPPER(BB.MES_STATUS) = 'FAIL'
                                                                    ) GG WHERE GG.STATUS IS NOT NULL"; 
                                        }
                                        else
                                        {
                                            StrFailSQL = $@"SELECT COUNT(DISTINCT GG.SYSSERIALNO)C FROM (
                                                        SELECT DISTINCT JJ.SKUNO, JJ.SYSSERIALNO,JJ.MES_STATION,AA.STATUS,JJ.TATIME FROM (
                                                        SELECT  CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION,MAX(TATIME) TATIME
                                                        FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_WO_BASE RWO,R_SN RSN
                                                        WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                        AND RTJ.EVENTNAME=CTS.TE_STATION 
                                                        AND RTJ.SYSSERIALNO=RSN.SN
                                                        AND CSKU.ID=RSR.SKU_ID
                                                        AND CRD.STATION_NAME=CTS.MES_STATION
                                                        AND RWO.WORKORDERNO=RSN.WORKORDERNO
                                                        AND RWO.SKUNO=CSKU.SKUNO
                                                        /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL')*/
                                                        AND RWO.WORKORDERNO = '{wo}' 
                                                        AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')" + StrTimeSQL
                                                        + $@" GROUP BY RTJ.SYSSERIALNO,CSKU.SKUNO,RSN.WORKORDERNO,CTS.MES_STATION)JJ
                                                                    LEFT JOIN R_TEST_JUNIPER AA ON JJ.SYSSERIALNO=AA.SYSSERIALNO AND JJ.TATIME=AA.TATIME 
                                                                    AND AA.EVENTNAME IN((SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')),
                                                                     C_TEMES_STATUS_MAPPING BB
                                                                     WHERE UPPER(AA.STATUS) = UPPER(BB.TE_STATUS)
                                                                       AND UPPER(BB.MES_STATUS) = 'FAIL'
                                                                    ) GG WHERE GG.STATUS IS NOT NULL";
                                        }
                                        linkURL = linkURL + "Pass_Yield";
                                    }
                                    else if (type == "True_Failure_Yield")
                                    {
                                        if (wo == "")
                                        {
                                            StrFailSQL = $@"SELECT COUNT(DISTINCT SS.SYSSERIALNO) c FROM (
                                                            SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN ,r_repair_main rrm
                                                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                            AND RTJ.EVENTNAME=CTS.TE_STATION
                                                            AND RTJ.SYSSERIALNO=RSN.SN
                                                            AND CSKU.ID=RSR.SKU_ID
                                                            AND CRD.STATION_NAME=CTS.MES_STATION
                                                            AND RTJ.SYSSERIALNO=RRM.SN
                                                            AND CTS.MES_STATION=RRM.FAIL_STATION
                                                            /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                                            AND CSKU.SKUNO='{sku}' 
                                                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')" + StrTimeSQL
                                                            + $@" )SS,C_TEMES_STATUS_MAPPING BB
                                                                     WHERE SS.RN = 1 AND UPPER(SS.STATUS)=UPPER(BB.TE_STATUS)
                                                                       and UPPER(BB.MES_STATUS) = 'FAIL'
"; 
                                        }
                                        else
                                        {
                                            StrFailSQL = $@"SELECT COUNT(DISTINCT SS.SYSSERIALNO) c FROM (
                                                            SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                                                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN ,r_repair_main rrm
                                                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                                                            AND RTJ.EVENTNAME=CTS.TE_STATION
                                                            AND RTJ.SYSSERIALNO=RSN.SN
                                                            AND CSKU.ID=RSR.SKU_ID
                                                            AND CRD.STATION_NAME=CTS.MES_STATION
                                                            AND RTJ.SYSSERIALNO=RRM.SN
                                                            AND CTS.MES_STATION=RRM.FAIL_STATION
                                                            /*AND UPPER(RTJ.STATUS) IN('PASS','FAIL') */
                                                            AND RWO.WORKORDERNO='{wo}' 
                                                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE MES_STATION='{DtTestCont.Rows[0]["MES_STATION"].ToString()}')" + StrTimeSQL
                                                            + $@" )SS,C_TEMES_STATUS_MAPPING BB
                                                                     WHERE SS.RN = 1 AND UPPER(SS.STATUS)=UPPER(BB.TE_STATUS)
                                                                       and UPPER(BB.MES_STATUS) = 'FAIL'
"; 
                                        }
                                        linkURL = linkURL + "True_Failure_Yield";
                                    }
                                    DataTable DtFail = SFCDB.RunSelect(StrFailSQL).Tables[0];
                                    Double FailSnCount = 0, aa = 1;

                                    if (DtFail.Rows.Count > 0)
                                    {
                                        FailSnCount = Double.Parse(DtFail.Rows[0]["c"].ToString());
                                        aa = (Double)((TestSnCount - FailSnCount) / TestSnCount);
                                        int bb = (int)(aa * 10000);
                                        aa = (Double)(bb * 0.0001);//保留两位小数
                                    }
                                    drd[DtTestCont.Rows[0]["MES_STATION"].ToString() + "_Test"] = TestSnCount;
                                    linkDataRow[DtTestCont.Rows[0]["MES_STATION"].ToString() + "_Test"] = (DtTestCont.Rows[0]["c"].ToString() != "0") ? (linkURL + "&Status='ALL'") : "";
                                    drd[DtTestCont.Rows[0]["MES_STATION"].ToString() + "_Fail"] = FailSnCount;
                                    linkDataRow[DtTestCont.Rows[0]["MES_STATION"].ToString() + "_Fail"] = (DtTestCont.Rows[0]["c"].ToString() != "0") ? (linkURL + "&Status='FAIL'") : "";
                                    drd[DtTestCont.Rows[0]["MES_STATION"].ToString() + "_Yield"] = aa * 100 + "%";
                                    //linkDataRow[DtTestCont.Rows[0]["EVENTNAME"].ToString() + "_Yield"] = (DtTestCont.Rows[0]["c"].ToString() != "0") ? (linkURL) : "";


                                }
                                else
                                {
                                    drd[dtroute[i].Rows[k]["MES_STATION"].ToString() + "_Test"] = 0;
                                    linkDataRow[dtroute[i].Rows[k]["MES_STATION"].ToString() + "_Test"] = "";
                                    drd[dtroute[i].Rows[k]["MES_STATION"].ToString() + "_Fail"] = 0;
                                    linkDataRow[dtroute[i].Rows[k]["MES_STATION"].ToString() + "_Fail"] = "";
                                    drd[dtroute[i].Rows[k]["MES_STATION"].ToString() + "_Yield"] = 0;
                                    linkDataRow[dtroute[i].Rows[k]["MES_STATION"].ToString() + "_Yield"] = "";
                                }

                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        throw ee;
                    }

                }

                var skus = Resdts.Keys.ToArray();

                for (int i = 0; i < skus.Length; i++)
                {
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(Resdts[skus[i]], LinkTables[skus[i]]);
                    retTab.Tittle = $@"{skus[i]}   Structural Test Yield";

                    Dictionary<string, List<TableHeader>> tmp = new Dictionary<string, List<TableHeader>>();
                    List<string> tas = new List<string>();
                    if (station == "ALL")
                    {

                      string   Sqlstation = $@" SELECT CTS.MES_STATION,CRD.SEQ_NO FROM C_ROUTE CR,C_ROUTE_DETAIL CRD,C_TEMES_STATION_MAPPING CTS where CR.ID=CRD.ROUTE_ID AND CRD.STATION_NAME=CTS.MES_STATION AND CR.ROUTE_NAME='{skus[i]}'ORDER BY CRD.SEQ_NO";
                      DataTable  dtstationroute = SFCDB.RunSelect(Sqlstation).Tables[0];
                        for (int j = 0; j < dtstationroute.Rows.Count; j++)
                        {
                            var Test = retTab.TableHeaders[0].Find(t => t.title == dtstationroute.Rows[j]["MES_STATION"] + "_Test");
                            var Fail = retTab.TableHeaders[0].Find(t => t.title == dtstationroute.Rows[j]["MES_STATION"] + "_Fail");
                            var Yield = retTab.TableHeaders[0].Find(t => t.title == dtstationroute.Rows[j]["MES_STATION"] + "_Yield");
                            if (Test != null)
                            {
                                tas.Add(dtstationroute.Rows[j]["MES_STATION"].ToString());
                                Test.colspan = 3;
                                Test.title = dtstationroute.Rows[j]["MES_STATION"].ToString();
                                Test.field = null;
                                retTab.TableHeaders[0].Remove(Fail);
                                retTab.TableHeaders[0].Remove(Yield);
                                List<TableHeader> r2 = null;

                                r2 = new List<TableHeader>();
                                TableHeader hW = new TableHeader();
                                hW.title = "Test";
                                hW.field = dtstationroute.Rows[j]["MES_STATION"] + "_Test";
                                r2.Add(hW);
                                TableHeader hF = new TableHeader();
                                hF.title = "Fail";
                                hF.field = dtstationroute.Rows[j]["MES_STATION"] + "_Fail";
                                r2.Add(hF);

                                TableHeader hR = new TableHeader();
                                hR.title = "Yield";
                                hR.field = dtstationroute.Rows[j]["MES_STATION"] + "_Yield";
                                r2.Add(hR);

                                tmp.Add(dtstationroute.Rows[j]["MES_STATION"].ToString(), r2);
                            }
                        }

                    }
                    else
                    {
                        var Test = retTab.TableHeaders[0].Find(t => t.title == MesStation + "_Test");
                        var Fail = retTab.TableHeaders[0].Find(t => t.title == MesStation + "_Fail");
                        var Yield = retTab.TableHeaders[0].Find(t => t.title == MesStation + "_Yield");
                        if (Test != null)
                        {
                            tas.Add(MesStation.ToString());
                            Test.colspan = 3;
                            Test.title = MesStation.ToString();
                            Test.field = null;
                            retTab.TableHeaders[0].Remove(Fail);
                            retTab.TableHeaders[0].Remove(Yield);
                            List<TableHeader> r2 = null;

                            r2 = new List<TableHeader>();
                            TableHeader hW = new TableHeader();
                            hW.title = "Test";
                            hW.field = MesStation + "_Test";
                            r2.Add(hW);
                            TableHeader hF = new TableHeader();
                            hF.title = "Fail";
                            hF.field = MesStation + "_Fail";
                            r2.Add(hF);

                            TableHeader hR = new TableHeader();
                            hR.title = "Yield";
                            hR.field = MesStation + "_Yield";
                            r2.Add(hR);

                            tmp.Add(MesStation.ToString(), r2);
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
            catch (Exception e) 
            { 
                throw e;
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }

        private List<string> GetStation()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dtStation = SFCDB.RunSelect(Sqls["GetSation"]).Tables[0];
                List<string> stationList = new List<string>();
                stationList.Add("ALL");

                if (dtStation.Rows.Count > 0)
                {
                    foreach (DataRow row in dtStation.Rows)
                    {
                        //stationList.Add(row["te_station"].ToString());
                        stationList.Add(row["mes_station"].ToString());
                    }
                }
                else
                {
                    throw new Exception("no station in system!");
                }
                return stationList;
            }
            catch (Exception ex)
            {
                throw ex;
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
