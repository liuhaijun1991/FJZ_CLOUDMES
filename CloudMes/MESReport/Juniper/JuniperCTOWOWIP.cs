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

namespace MESReport.Juniper
{
    public class JuniperCTOWOWIP : ReportBase
    {
        //ReportInput inputSku = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput WIPTYPE = new ReportInput { Name = "WIPTYPE", InputType = "Select", Value = "CTO", Enable = true, SendChangeEvent = false, ValueForUse = new string[] {  "BTS", "CTO" } };
        [JsonIgnore]
        [ScriptIgnore]
        public OleExec SFCDB = null;
        public JuniperCTOWOWIP()
        {
            //this.Inputs.Add(inputSku);
            this.Inputs.Add(WIPTYPE);
        }
        public override void Run()
        {
            string sqlSku = "";
            string sqlRoute = "";
            bool isborrow = false;
            string WIP_TYPE = WIPTYPE.Value.ToString();
            string SqlFun = $@" ";
            string sqlWoWipCount = "";
            if (SFCDB == null)
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                isborrow = true;
            }
            if (WIP_TYPE == "CTO")
            {
                 SqlFun = $@" FUNCTIONNAME='CTOWOWIP'";
            }
            else
            {
                 SqlFun = $@" FUNCTIONNAME='BTSWOWIP'";
            }
            if (WIP_TYPE == "CTO")
            {
                sqlRoute = $@"SELECT *
                                FROM (SELECT a.*,
                                            ROW_NUMBER() OVER(PARTITION BY STATION_NAME, SKU_TYPE ORDER BY SEQ_NO DESC) numbs
                                        FROM (SELECT DISTINCT CASE
                                                                WHEN R.STATION_NAME LIKE '%LOADING%' THEN
                                                                'LOADING'
                                                                ELSE R.STATION_NAME
                                                            END STATION_NAME,
                                                            S.SKU_TYPE,R.SEQ_NO
                                                FROM C_SKU S, C_ROUTE_DETAIL R, R_SKU_ROUTE SR,R_WO_BASE RW
                                                WHERE S.ID = SR.SKU_ID AND S.SKUNO=RW.SKUNO AND R.STATION_NAME NOT IN ('SILOADING')
                                                AND RW.WORKORDERNO IN (SELECT VALUE FROM R_FUNCTION_CONTROL WHERE {SqlFun} AND CATEGORY='WO'AND CONTROLFLAG='Y' AND FUNCTIONTYPE='NOSYSTEM')
                                                AND R.ROUTE_ID = SR.ROUTE_ID
                                            ) a)
                                WHERE numbs = 1
                                ORDER BY SKU_TYPE, SEQ_NO,STATION_NAME";
            }
            else
            {
                sqlRoute = $@"SELECT *
                                FROM (SELECT a.*,
                                            ROW_NUMBER() OVER(PARTITION BY STATION_NAME, SKU_TYPE ORDER BY SEQ_NO DESC) numbs
                                        FROM (SELECT DISTINCT CASE
                                                                WHEN R.STATION_NAME LIKE '%LOADING%' THEN
                                                                'LOADING'
                                                                ELSE R.STATION_NAME
                                                            END STATION_NAME,
                                                            S.SKU_TYPE,R.SEQ_NO
                                                FROM C_SKU S, C_ROUTE_DETAIL R, R_SKU_ROUTE SR,R_WO_BASE RW
                                                WHERE S.ID = SR.SKU_ID AND S.SKUNO=RW.SKUNO 
                                                AND RW.WORKORDERNO IN (SELECT VALUE FROM R_FUNCTION_CONTROL WHERE {SqlFun} AND CATEGORY='WO'AND CONTROLFLAG='Y' AND FUNCTIONTYPE='NOSYSTEM')
                                                AND R.ROUTE_ID = SR.ROUTE_ID
                                            ) a)
                                WHERE numbs = 1
                                ORDER BY SKU_TYPE, SEQ_NO,STATION_NAME";
            }


            try
            {
                RunSqls.Add(sqlSku);
                RunSqls.Add(sqlRoute);
                DataTable dtRoute = SFCDB.RunSelect(sqlRoute).Tables[0];
                DataTable reportTable = new DataTable();
                DataTable linkTable = new DataTable();
                reportTable.Columns.Add("BUILDDATE");
                reportTable.Columns.Add("SO");
                reportTable.Columns.Add("SHIPDATE");
                reportTable.Columns.Add("SKU");
                if (WIP_TYPE == "BTS")
                {
                    reportTable.Columns.Add("TLA");
                    reportTable.Columns.Add("GROUP");
                }
                reportTable.Columns.Add("WO");
                reportTable.Columns.Add("QTY");
                if (WIP_TYPE == "CTO")
                {
                    reportTable.Columns.Add("SN");
                }
                for (int i = 0; i < dtRoute.Rows.Count; i++)
                {
                    if (!reportTable.Columns.Contains(dtRoute.Rows[i]["STATION_NAME"].ToString()))
                    {
                        reportTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString());
                        linkTable.Columns.Add(dtRoute.Rows[i]["STATION_NAME"].ToString());
                    }
                }
                reportTable.Columns.Add("SHIPFINISH");
                reportTable.Columns.Add("REMARK");
                linkTable.Columns.Add("SO");
                linkTable.Columns.Add("BUILDDATE");
                linkTable.Columns.Add("SHIPDATE");
                linkTable.Columns.Add("SKU");
                if (WIP_TYPE == "BTS")
                {
                    linkTable.Columns.Add("TLA");
                    linkTable.Columns.Add("GROUP");
                }
                linkTable.Columns.Add("WO");
                linkTable.Columns.Add("QTY");
                if (WIP_TYPE == "CTO")
                {
                    linkTable.Columns.Add("SN");
                }
                linkTable.Columns.Add("SHIPFINISH");
                linkTable.Columns.Add("REMARK");

                DataRow reportRow;
                DataRow linkDataRow;
                if (WIP_TYPE == "CTO")
                {
                    sqlWoWipCount = $@"SELECT DISTINCT JJ.SO,'' AS TLA,''GROUP1,
                                           JJ.BUILDDATE,
                                           JJ.SHIPDATE,
                                           JJ.PID SKU,
                                           JJ.WO,
                                           JJ.QTY,
                                           SN.SN,
                                           SN.NEXT_STATION,
                                          CASE
                                              WHEN SN.NEXT_STATION = 'SHIPFINISH' THEN
                                              'COMPLETE'
                                              ELSE
                                              'OPEN'
                                          END AS STATUS,EX.VALUE REMARK,
                                       ROUND((SYSDATE -NVL(RR.EDIT_TIME,SYSDATE))*24 ,1)  WAIT_H
                                      FROM (SELECT OM.PID,FC.ID,
                                                  FC.EXTVAL BUILDDATE,
                                                  TO_CHAR(OM.DELIVERY, 'YYYY/MM/DD') SHIPDATE,
                                                  IH.SALESORDERNUMBER || '_' || II.SALESORDERLINEITEM SO,
                                                  OM.PREWO WO,
                                                  OM.QTY
                                              FROM O_ORDER_MAIN OM, O_I137_ITEM II, O_I137_HEAD IH,R_FUNCTION_CONTROL FC
                                              WHERE OM.ORIGINALITEMID = II.ID AND OM.PREWO=FC.VALUE
                                              AND II.TRANID = IH.TRANID
                                              AND FC.FUNCTIONNAME= 'CTOWOWIP'
                                              AND FC.CATEGORY='WO'
                                              AND FC.CONTROLFLAG='Y'
                                              AND FC.FUNCTIONTYPE='NOSYSTEM') JJ
                                      LEFT JOIN R_SN SN
                                      ON SN.WORKORDERNO = JJ.WO  AND SN.VALID_FLAG !=0
                                      left join R_FUNCTION_CONTROL_EX EX ON JJ.ID=EX.DETAIL_ID AND EX.SEQ_NO=1
                                      left join R_SN_STATION_DETAIL RR ON 
                                      RR.R_SN_ID=SN.ID 
                                      AND RR.SN=SN.SN 
                                      AND SN.NEXT_STATION=RR.NEXT_STATION
                                      ORDER BY JJ.BUILDDATE,JJ.SHIPDATE, JJ.PID, JJ.WO";
                }
                else
                {
                    sqlWoWipCount = $@"SELECT DISTINCT AA.SO, AA.BUILDDATE,
                                        AA.SHIPDATE,
                                        AA.SKU,
                                        AA.WO,
                                        AA.QTY,
                                        AA.TLA,
                                        AA.NEXT_STATION,COUNT(AA.NEXT_STATION) SUMQTY ,AA.STATUS,RF.VALUE REMARK,RFF.VALUE  GROUP1  FROM (
                            SELECT JJ.SKU,
                           JJ.BUILDDATE,
                                        JJ.SHIPDATE,
                                        JJ.SO,
                                        FE.VALUE TLA,
                                        FE.VALUE REMARK,
                                        JJ.WO,
                                        JJ.QTY,
                                        SN.NEXT_STATION,JJ.ID,
                                        CASE
                                            WHEN SN.NEXT_STATION = 'SHIPFINISH' THEN
                                            'COMPLETE'
                                            ELSE
                                            'OPEN'
                                        END AS STATUS
                                    FROM(SELECT OM.PID SKU, FC.ID,
                                                FC.EXTVAL  BUILDDATE,
                                                TO_CHAR(OM.DELIVERY, 'YYYY/MM/DD') SHIPDATE,
                                                IH.SALESORDERNUMBER || '_' || II.SALESORDERLINEITEM SO,
                                                OM.PREWO WO,
                                                CAST(OM.QTY AS VARCHAR2(50)) QTY
                                            FROM O_ORDER_MAIN OM, O_I137_ITEM II, O_I137_HEAD IH, R_FUNCTION_CONTROL FC
                                            WHERE OM.ORIGINALITEMID = II.ID AND OM.PREWO = FC.VALUE
                                              AND II.TRANID = IH.TRANID
                                              AND FC.FUNCTIONNAME = 'BTSWOWIP'
                                              AND FC.CATEGORY = 'WO'
                                              AND FC.CONTROLFLAG = 'Y'
                                              AND FC.FUNCTIONTYPE = 'NOSYSTEM'
                                              UNION
                                              SELECT WO.SKUNO SKU ,FC.ID ,FC.EXTVAL  BUILDDATE,'' SHIPDATE,
                                              '' SO, WO.WORKORDERNO WO,CAST( WO.WORKORDER_QTY AS VARCHAR2(50)) QTY
                                              FROM R_WO_BASE WO,R_FUNCTION_CONTROL FC
                                              WHERE WO.WORKORDERNO=FC.VALUE
                                              AND FC.FUNCTIONNAME = 'BTSWOWIP'
                                              AND WO.SKUNO LIKE '%-FVN'
                                              AND FC.CATEGORY = 'WO'
                                              AND FC.CONTROLFLAG = 'Y'
                                              AND FC.FUNCTIONTYPE = 'NOSYSTEM') JJ
                                      LEFT JOIN R_SN SN ON SN.WORKORDERNO = JJ.WO AND SN.VALID_FLAG != 0  LEFT JOIN R_FUNCTION_CONTROL_EX FE ON JJ.ID = FE.DETAIL_ID AND FE.SEQ_NO = 1
                                    )AA LEFT JOIN R_FUNCTION_CONTROL_EX RF ON AA.ID = RF.DETAIL_ID AND RF.SEQ_NO = 2
                                    LEFT JOIN R_FUNCTION_CONTROL_EX RFF ON AA.ID = RFF.DETAIL_ID AND RF.SEQ_NO = 3
                                    GROUP BY AA.SKU,AA.TLA,
                                        AA.BUILDDATE,
                                        AA.SHIPDATE,
                                        AA.SO,
                                        AA.WO,
                                        AA.QTY,
                                        AA.NEXT_STATION,AA.STATUS ,RF.VALUE,RFF.VALUE
                                    ORDER BY AA.BUILDDATE,AA.SHIPDATE, AA.SKU, AA.WO";
                }
                DataTable DtSqlWip = SFCDB.RunSelect(sqlWoWipCount).Tables[0];
                var sqlWoWipCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlWoWipCount);
                var  wolist = (from d in DtSqlWip.AsEnumerable()
                                select new
                                {
                                    WO = d.Field<string>("WO"),
                                    QTY = d.Field<string>("QTY"),
                                    SKU = d.Field<string>("SKU"),
                                    BUILDDATE = d.Field<string>("BUILDDATE"),
                                    TLA = d.Field<string>("TLA"),
                                    SHIPDATE = d.Field<string>("SHIPDATE"),
                                    SO = d.Field<string>("SO"),
                                    STATUS = d.Field<string>("STATUS"),
                                    REMARK = d.Field<string>("REMARK"),
                                    GROUP = d.Field<string>("GROUP1")
                                }).Distinct().ToList();
                if (WIP_TYPE == "CTO")
                {
                    for (int i = 0; i < wolist.Count; i++)
                    {
                        if (wolist[i].STATUS != "COMPLETE")
                        {
                            var qty = Convert.ToDouble(wolist[i].QTY);
                            var wos = DtSqlWip.Select($@"WO='{wolist[i].WO}'");
                            var pos = DtSqlWip.Rows.IndexOf(wos[0]);
                            if (wos.Length < qty)
                            {
                                for (int x = 0; x < qty - wos.Length; x++)
                                {
                                    var newrow = DtSqlWip.NewRow();
                                    newrow["SKU"] = wos[0]["SKU"];
                                    newrow["BUILDDATE"] = wos[0]["BUILDDATE"];
                                    newrow["SHIPDATE"] = wos[0]["SHIPDATE"];
                                    newrow["SO"] = wos[0]["SO"];
                                    newrow["WO"] = wos[0]["WO"];
                                    newrow["QTY"] = wos[0]["QTY"];
                                    newrow["SN"] = "";
                                    newrow["NEXT_STATION"] = "";
                                    newrow["REMARK"] = wos[0]["REMARK"];
                                    DtSqlWip.Rows.InsertAt(newrow, pos);
                                }
                            }
                        }
                    }

                    for (int j = 0; j < DtSqlWip.Rows.Count; j++)
                    {
                        try
                        {
                            if (DtSqlWip.Rows[j]["STATUS"].ToString() != "COMPLETE")
                            {
                                reportRow = reportTable.NewRow();
                                linkDataRow = linkTable.NewRow();
                                reportRow["SO"] = DtSqlWip.Rows[j]["SO"] == null ? "" : DtSqlWip.Rows[j]["SO"];
                                reportRow["BUILDDATE"] = DtSqlWip.Rows[j]["BUILDDATE"] == null ? "" : DateTime.Parse(DtSqlWip.Rows[j]["BUILDDATE"].ToString()).ToString("yyyy/MM/dd"); 
                                reportRow["SHIPDATE"] = DtSqlWip.Rows[j]["SHIPDATE"] == null ? "" : DtSqlWip.Rows[j]["SHIPDATE"];
                                reportRow["SKU"] = DtSqlWip.Rows[j]["SKU"] == null ? "" : DtSqlWip.Rows[j]["SKU"];
                                reportRow["WO"] = DtSqlWip.Rows[j]["WO"] == null ? "" : DtSqlWip.Rows[j]["WO"];
                                reportRow["QTY"] = DtSqlWip.Rows[j]["QTY"] == null ? "" : DtSqlWip.Rows[j]["QTY"].ToString().TrimEnd('0').TrimEnd('.'); 
                                reportRow["SN"] = DtSqlWip.Rows[j]["SN"] == null ? "" : DtSqlWip.Rows[j]["SN"];
                                #region STATION WIP
                                if (!string.IsNullOrEmpty(DtSqlWip.Rows[j]["SN"].ToString()) && DtSqlWip.Rows[j]["SN"].ToString() != "")
                                {  //modify by hgb 2022.03.16 
                                    //reportRow[DtSqlWip.Rows[j]["NEXT_STATION"].ToString()] = DtSqlWip.Rows[j]["NEXT_STATION"].ToString() == "" ? "OK" : "Process";
                                    string h = DtSqlWip.Rows[j]["WAIT_H"].ToString() ;
                                    reportRow[DtSqlWip.Rows[j]["NEXT_STATION"].ToString()] = h=="0"?"0.1":h+"H";
                                }
                                #endregion
                                reportRow["REMARK"] = DtSqlWip.Rows[j]["REMARK"] == null ? "" : DtSqlWip.Rows[j]["REMARK"];
                                reportTable.Rows.Add(reportRow);
                                linkTable.Rows.Add(linkDataRow);
                            }
                        }
                        catch (Exception ee)
                        {

                        }
                    }
                }
                else 
                {
                    for (int i = 0; i < wolist.Count; i++)
                    {
                        try
                        {
                            if (wolist[i].STATUS != "COMPLETE")
                            {
                                reportRow = reportTable.NewRow();
                                linkDataRow = linkTable.NewRow();
                                reportRow["SO"] = wolist[i].SO == null ? "" : wolist[i].SO;
                                reportRow["BUILDDATE"] = wolist[i].BUILDDATE == null ? "" : DateTime.Parse(wolist[i].BUILDDATE).ToString("yyyy/MM/dd");
                                reportRow["SHIPDATE"] = wolist[i].SHIPDATE == null ? "" : wolist[i].SHIPDATE;
                                reportRow["TLA"] = wolist[i].TLA == null ? "" : wolist[i].TLA;
                                reportRow["SKU"] = wolist[i].SKU == null ? "" : wolist[i].SKU;
                                reportRow["WO"] = wolist[i].WO == null ? "" : wolist[i].WO;
                                reportRow["QTY"] = wolist[i].QTY == null ? "" : wolist[i].QTY.ToString().TrimEnd('0').TrimEnd('.');

                                var wos = DtSqlWip.Select($@"WO='{wolist[i].WO}'");
                                var data = sqlWoWipCounts.FindAll(t => t.WO == wos[0]["WO"].ToString());

                                #region STATION WIP
                                for (int j = 0; j < data.Count; j++)
                                {
                                    if (!string.IsNullOrEmpty(data[j].NEXT_STATION))
                                    {
                                        if (data[j].NEXT_STATION.Contains("LOADING"))
                                        {
                                            reportRow["LOADING"] = data[j].SUMQTY;
                                        }
                                        else
                                        {
                                            reportRow[data[j].NEXT_STATION] = data[j].SUMQTY;
                                        }

                                    }
                                }
                                #endregion

                                string sqlLoadingCount = $@"SELECT WORKORDERNO WO, COUNT(*) QTY
                                                      FROM R_SN S WHERE S.WORKORDERNO='{wolist[i].WO}'
                                                           AND S.NEXT_STATION NOT LIKE 'REVERSE%'
                                                           AND S.SN NOT LIKE '~%'
                                                     GROUP BY WORKORDERNO";
                                var loadingCounts = SFCDB.ORM.Ado.SqlQuery<StationData>(sqlLoadingCount);
                                var loadingCount = loadingCounts.Find(t => t.WO == wolist[i].WO);
                                Double WoSum = Convert.ToDouble(wolist[i].QTY);
                                Double loadings = loadingCount == null ? 0 : Convert.ToDouble(loadingCount.QTY);
                                reportRow["LOADING"] = WoSum - loadings;
                                reportRow["REMARK"] = wolist[i].REMARK == null ? "" : wolist[i].REMARK;
                                reportRow["GROUP"] = wolist[i].GROUP == null ? "" : wolist[i].GROUP;
                                reportTable.Rows.Add(reportRow);
                                linkTable.Rows.Add(linkDataRow);
                            }
                        }
                        catch (Exception ee)
                        {
                            throw ee;
                        }
                    }
                }
                    ReportTable report = new ReportTable();
                    report.LoadData(reportTable, linkTable);
                    report.SearchCol.Add("WO");
                    report.SearchCol.Add("SKU");
                    report.FixedCol = 5;
                    report.FixedHeader = true;
                    report.pagination = false;
                    report.Tittle = "WO WIP";
                    //report.Rows.Add(totalRow);
                    var today = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));
                    for (int i = 0; i < report.Rows.Count; i++)
                    {
                        if (report.Rows[i]["SHIPDATE"].Value != "")
                        {
                            var shipdate = Convert.ToDateTime(report.Rows[i]["SHIPDATE"].Value);
                            try
                            {
                                var offset = DateTime.Compare(today, shipdate);
                                if (offset > 0)
                                {
                                    report.Rows[i]["WO"].CellStyle = new Dictionary<string, object>()
                                    {
                                        { "background", "#FF1100" },
                                        { "color", "#000" }
                                    };
                                }
                                else if (offset == 0)
                                {
                                    report.Rows[i]["WO"].CellStyle = new Dictionary<string, object>()
                                    {
                                        { "background", "#FFFF00" },
                                        { "color", "#000000" }
                                    };
                                }
                                else
                                {
                                    report.Rows[i]["WO"].CellStyle = new Dictionary<string, object>()
                                    {
                                        { "background", "#00FF00" },
                                        { "color", "#000" }
                                    };
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
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

        class StationData
        {
            public string SKUNO { get; set; }
            public string SHIPDATE { get; set; }
            public string SO { get; set; }
            public string WO { get; set; }
            public string QTY { get; set; }
            public string SUMQTY { get; set; }
            public string SN { get; set; }
            public string NEXT_STATION { get; set; }
            public string STATUS { get; set; }
           
        }
    }
}
