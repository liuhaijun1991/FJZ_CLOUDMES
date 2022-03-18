using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESReport.BaseReport
{
    public class QueryModel : ReportBase
    {
        ReportInput SKUNO = new ReportInput() { Name = "SKUNO", InputType = "Autocomplete", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = false };
        ReportInput GROUP = new ReportInput() { Name = "GROUP", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL", "CBS", "CBS2", "SHIPOUT", "SHIPFINISH" }, EnterSubmit = false };
        ReportInput WH_NAME = new ReportInput { Name = "WH_NAME", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput CheckPO = new ReportInput { Name = "Check_PO", InputType = "Select", Value = "No", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "Yes", "No" }, EnterSubmit = false };
        ReportInput PO_NO = new ReportInput() { Name = "PO_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = false };
        ReportInput CHECKDATE = new ReportInput { Name = "Check_Time", InputType = "Select", Value = "No", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "Yes", "No" }, EnterSubmit = false };
        ReportInput STARTTIME = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2021-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ENDTIME = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2021-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public QueryModel()
        {
            Inputs.Add(SKUNO);
            Inputs.Add(GROUP);
            Inputs.Add(WH_NAME);
            Inputs.Add(CheckPO);
            Inputs.Add(PO_NO);
            Inputs.Add(CHECKDATE);
            Inputs.Add(STARTTIME);
            Inputs.Add(ENDTIME);
        }
        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                STARTTIME.Value = DateTime.Now;
                ENDTIME.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public override void Run()
        {
            bool countFlag = false;
            OleExec SFCDB = null;
            SFCDB = DBPools["SFCDB"].Borrow();
            string bu = SFCDB.ORM.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();
            string sql = "";
            string sql1 = "";
            string sql2 = "";
            string sql3 = "";
            string Skuno = SKUNO.Value.ToString();
            string Station = GROUP.Value.ToString();
            string Wh_name = WH_NAME.Value.ToString();
            string checkpo = CheckPO.Value.ToString();
            string po_no = PO_NO.Value.ToString();
            string time = CHECKDATE.Value.ToString();
            DateTime startDT = Convert.ToDateTime(STARTTIME.Value);
            DateTime endDT = Convert.ToDateTime(ENDTIME.Value);
            endDT = endDT.AddDays(1);
            string dateFrom = startDT.ToString("yyyy/MM/dd HH:mm:ss");
            string dateTo = endDT.ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                if (Station == "ALL")
                {
                    throw new Exception($"Please Choose Station!");
                }
                if (bu == "VNDCN")
                {
                    if ((Skuno == "ALL") && (Wh_name == "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        sql1 = $@"  SELECT  SN.SKUNO, COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET , WH.WH_NAME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID";
                        sql2 = $@"GROUP BY SN.SKUNO,WH.WH_NAME
                                                 UNION 
                                  SELECT  SN.SKUNO, COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,'' WH_NAME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN
			                                     WHERE SN.SKUNO NOT IN( SELECT  SN.SKUNO
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID ";
                        sql3 = $@"GROUP BY SN.SKUNO,WH.WH_NAME)
                                                 AND PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.SKUNO NOT LIKE '#%'
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND SN.SHIPPED_FLAG=0";
                        if (time == "Yes")
                        {
                            sql1 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            sql2 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            sql3 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                        }
                        else if (time == "No")
                        {
                            sql1 += $@" ";
                            sql2 += $@" ";
                            sql3 += $@" ";
                        }
                        sql = sql1 + sql2 + sql3 + $@" GROUP BY SN.SKUNO";
                    }
                    else if ((Skuno == "ALL") && (Wh_name != "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        sql = $@" SELECT  SN.SKUNO, COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET , WH.WH_NAME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND WH.WH_NAME='{Wh_name}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID";
                        if (time == "Yes")
                        {
                            sql += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                        }
                        else if (time == "No")
                        {
                            sql += $@" ";
                        }
                        sql += $@" GROUP BY SN.SKUNO,WH.WH_NAME";
                    }
                    else if ((Skuno != "ALL") && (Wh_name == "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        sql1 = $@"  SELECT  SN.SKUNO, COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET , WH.WH_NAME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND SN.SKUNO ='{Skuno}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID";
                        sql2 = $@"GROUP BY SN.SKUNO,WH.WH_NAME
                                                 UNION 
                                  SELECT  SN.SKUNO, COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,'' WH_NAME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN
			                                     WHERE SN.SKUNO NOT IN( SELECT  SN.SKUNO
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND SN.SKUNO ='{Skuno}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID ";
                        sql3 = $@"GROUP BY SN.SKUNO,WH.WH_NAME)
                                                 AND PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.SKUNO NOT LIKE '#%'
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND SN.SKUNO ='{Skuno}'
                                                 AND SN.SHIPPED_FLAG=0";
                        if (time == "Yes")
                        {
                            sql1 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            sql2 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            sql3 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                        }
                        else if (time == "No")
                        {
                            sql1 += $@" ";
                            sql2 += $@" ";
                            sql3 += $@" ";
                        }
                        sql = sql1 + sql2 + sql3 + $@" GROUP BY SN.SKUNO";
                    }
                    else if ((Skuno != "ALL") && (Wh_name != "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        sql = $@" SELECT  SN.SKUNO, COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET , WH.WH_NAME
			                                     FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH ,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO
			                                     WHERE PL.id = PX.PARENT_PACK_ID(+)
			                                     AND PX.id = SP.PACK_ID(+)
			                                     AND SP.SN_ID = SN.ID
                                                 AND SN.NEXT_STATION ='{Station}'
                                                 AND WH.WH_NAME='{Wh_name}'
                                                 AND SN.SKUNO ='{Skuno}'
                                                 AND WH.COL_SIZE=1
                                                 AND PO.PALLET_NO= PL.PACK_NO
                                                 AND PO.OUT_FLAG=0
                                                 AND PO.WH_ID=WH.WH_ID";
                        if (time == "Yes")
                        {
                            sql += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                        }
                        else if (time == "No")
                        {
                            sql += $@" ";
                        }
                        sql += $@"GROUP BY SN.SKUNO,WH.WH_NAME";
                    }
                    RunSqls.Add(sql);
                    DataTable dt = null;
                    DataRow linkDataRow = null;
                    DataTable linkTable = new DataTable();
                    try
                    {
                        dt = SFCDB.RunSelect(sql).Tables[0];
                        if (SFCDB != null)
                        {
                            DBPools["SFCDB"].Return(SFCDB);
                        }

                        //增加一行總數                
                        var totalSN = 0;
                        var totalPallet = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            totalSN += Convert.ToInt32(dt.Rows[i]["TOTAL_SN"]);
                            totalPallet += Convert.ToInt32(dt.Rows[i]["TOTAL_PALLET"]);
                        }
                        string allSKU = string.Empty;
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            allSKU += dt.Rows[t]["SKUNO"].ToString();
                            allSKU += ",";
                        }
                        string allWH = string.Empty;
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            allWH += dt.Rows[t]["WH_NAME"].ToString();
                            allWH += ",";
                        }
                        var totalRow = dt.NewRow();
                        totalRow[0] = "TOTAL";
                        totalRow[1] = totalSN.ToString();
                        totalRow[2] = totalPallet.ToString();
                        totalRow[3] = "-";
                        if (dt.Rows.Count > 0)
                            dt.Rows.Add(totalRow);
                        linkTable.Columns.Add("SKUNO");
                        linkTable.Columns.Add("TOTAL_SN");
                        linkTable.Columns.Add("TOTAL_PALLET");
                        linkTable.Columns.Add("WH_NAME");
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            linkDataRow = linkTable.NewRow();
                            linkDataRow["SKUNO"] = "";
                            linkDataRow["WH_NAME"] = "";
                            linkDataRow["TOTAL_SN"] = dt.Rows[t]["TOTAL_SN"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNPLDetailBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Station=" + Station + "&Type=TOTAL_SN";
                            linkDataRow["TOTAL_PALLET"] = dt.Rows[t]["TOTAL_PALLET"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNPLDetailBySKU&RunFlag=1&SKU=" + dt.Rows[t]["SKUNO"].ToString() + "&Station=" + Station + "&Type=TOTAL_PALLET";
                            linkTable.Rows.Add(linkDataRow);
                        }
                        ReportTable reportTable = new ReportTable();
                        reportTable.LoadData(dt, linkTable);
                        reportTable.Tittle = "Query Model";
                        Outputs.Add(reportTable);
                    }
                    catch (Exception exception)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                        throw exception;
                    }
                }
                else if (bu == "VNJUNIPER")
                {
                    if ((Skuno == "ALL") && (Wh_name == "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        if (checkpo == "No" && po_no == "")
                        {
                            sql1 = $@"SELECT W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO, WH.WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0";
                            sql2 = $@"  AND PO.WH_ID=WH.WH_ID GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO
                                      UNION
                                  SELECT W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO,'' WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE W.GROUPID NOT IN ( SELECT  W.GROUPID 
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID";
                            sql3 = $@"GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO)
                                        AND PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO";
                            if (time == "Yes")
                            {
                                sql1 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                                sql2 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                                sql3 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            }
                            else if (time == "No")
                            {
                                sql1 += $@" ";
                                sql2 += $@" ";
                                sql3 += $@" ";
                            }
                            sql = sql1 + sql2 + sql3 + $@"GROUP BY W.GROUPID,SN.WORKORDERNO,O.PONO";
                        }
                        else if (checkpo == "Yes" && po_no != "")
                        {
                            sql1 = $@"SELECT W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO, WH.WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                         AND O.PONO='{po_no}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0";
                            sql2 = $@"  AND PO.WH_ID=WH.WH_ID GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO
                                      UNION
                                  SELECT W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO,'' WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE W.GROUPID NOT IN ( SELECT  W.GROUPID 
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND O.PONO='{po_no}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID";
                            sql3 = $@"GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO)
                                        AND PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND O.PONO='{po_no}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO";
                            if (time == "Yes")
                            {
                                sql1 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                                sql2 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                                sql3 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            }
                            else if (time == "No")
                            {
                                sql1 += $@" ";
                                sql2 += $@" ";
                                sql3 += $@" ";
                            }
                            sql = sql1 + sql2 + sql3 + $@"GROUP BY W.GROUPID,SN.WORKORDERNO,O.PONO";
                        }
                    }
                    else if ((Skuno != "ALL") && (Wh_name == "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        if (checkpo == "No" && po_no == "")
                        {
                            sql1 = $@"SELECT W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO, WH.WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND W.GROUPID='{Skuno}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0";
                            sql2 = $@"  AND PO.WH_ID=WH.WH_ID GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO
                                      UNION
                                  SELECT W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO,'' WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE W.GROUPID NOT IN ( SELECT  W.GROUPID 
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND W.GROUPID='{Skuno}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID";
                            sql3 = $@"GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO)
                                        AND PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND W.GROUPID='{Skuno}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO";
                            if (time == "Yes")
                            {
                                sql1 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                                sql2 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                                sql3 += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            }
                            else if (time == "No")
                            {
                                sql1 += $@" ";
                                sql2 += $@" ";
                                sql3 += $@" ";
                            }
                            sql = sql1 + sql2 + sql3 + $@"GROUP BY W.GROUPID,SN.WORKORDERNO,O.PONO";
                        }
                    }
                    else if ((Skuno != "ALL") && (Wh_name != "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        if (checkpo == "No" && po_no == "")
                        {
                            sql = $@" SELECT  W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO, WH.WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND W.GROUPID='{Skuno}'
                                        AND WH.WH_NAME='{Wh_name}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0
                                        AND PO.WH_ID=WH.WH_ID ";
                            if (time == "Yes")
                            {
                                sql += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            }
                            else if (time == "No")
                            {
                                sql += $@" ";
                            }
                            sql += $@"GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO ";
                        }
                    }
                    else if ((Skuno == "ALL") && (Wh_name != "ALL") && (Station != "ALL"))
                    {
                        countFlag = true;
                        if (checkpo == "No" && po_no == "")
                        {
                            sql = $@" SELECT  W.GROUPID , COUNT( DISTINCT SN.SN) as Total_SN,count( DISTINCT PL.PACK_NO) as TOTAL_PALLET ,SN.WORKORDERNO,O.PONO, WH.WH_NAME
			                            FROM SFCRUNTIME.R_PACKING PL,SFCRUNTIME.R_PACKING PX, R_SN_PACKING SP,SFCRUNTIME.R_SN SN,SFCBASE.C_WAREHOUSE_CONFIG_T WH,SFCBASE.C_WAREHOUSE_PALLET_POSITION_T PO,O_ORDER_MAIN O,R_WO_GROUPID W
			                            WHERE PL.id = PX.PARENT_PACK_ID(+)
			                            AND PX.id = SP.PACK_ID(+)
			                            AND SP.SN_ID = SN.ID
                                        AND SN.NEXT_STATION='{Station}'
                                        AND WH.WH_NAME='{Wh_name}'
                                        AND O.PREWO=sn.WORKORDERNO
                                        AND W.WO=O.PREWO
                                        AND WH.COL_SIZE=1
                                        AND PO.PALLET_NO= PL.PACK_NO
                                        AND PO.OUT_FLAG=0";
                            if (time == "Yes")
                            {
                                sql += $@" AND SN.EDIT_TIME  between to_date('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') and to_date('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ";
                            }
                            else if (time == "No")
                            {
                                sql += $@" ";
                            }
                            sql += $@"GROUP BY W.GROUPID,SN.WORKORDERNO,WH.WH_NAME,O.PONO";
                        }
                    }
                    RunSqls.Add(sql);
                    DataTable dt = null;
                    DataRow linkDataRow = null;
                    DataTable linkTable = new DataTable();
                    try
                    {
                        dt = SFCDB.RunSelect(sql).Tables[0];
                        if (SFCDB != null)
                        {
                            DBPools["SFCDB"].Return(SFCDB);
                        }
                        var totalSN = 0;
                        var totalPallet = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            totalSN += Convert.ToInt32(dt.Rows[i]["TOTAL_SN"]);
                            totalPallet += Convert.ToInt32(dt.Rows[i]["TOTAL_PALLET"]);
                        }
                        string allSKU = string.Empty;
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            allSKU += dt.Rows[t]["GROUPID"].ToString();
                            allSKU += ",";
                        }
                        string allWO = string.Empty;
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            allWO += dt.Rows[t]["WORKORDERNO"].ToString();
                            allWO += ",";
                        }
                        string allPO = string.Empty;
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            allPO += dt.Rows[t]["PONO"].ToString();
                            allPO += ",";
                        }
                        string allWH = string.Empty;
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            allWH += dt.Rows[t]["WH_NAME"].ToString();
                            allWH += ",";
                        }
                        var totalRow = dt.NewRow();
                        totalRow[0] = "Total";
                        totalRow[1] = totalSN.ToString();
                        totalRow[2] = totalPallet.ToString();
                        totalRow[3] = "-";
                        totalRow[4] = "-";
                        totalRow[5] = "-";
                        if (dt.Rows.Count > 0)
                            dt.Rows.Add(totalRow);
                        linkTable.Columns.Add("GROUPID");
                        linkTable.Columns.Add("TOTAL_SN");
                        linkTable.Columns.Add("TOTAL_PALLET");
                        linkTable.Columns.Add("WORKORDERNO");
                        linkTable.Columns.Add("PONO");
                        linkTable.Columns.Add("WH_NAME");
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            linkDataRow = linkTable.NewRow();
                            linkDataRow["GROUPID"] = "";
                            linkDataRow["WORKORDERNO"] = "";
                            linkDataRow["PONO"] = "";
                            linkDataRow["WH_NAME"] = "";
                            linkDataRow["TOTAL_SN"] = dt.Rows[t]["TOTAL_SN"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNPLDetailBySKU&RunFlag=1&SKU=" + dt.Rows[t]["GROUPID"].ToString() + "&Station=" + Station + "&Type=TOTAL_SN";
                            linkDataRow["TOTAL_PALLET"] = dt.Rows[t]["TOTAL_PALLET"].ToString() == "0" ? "" : "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNPLDetailBySKU&RunFlag=1&SKU=" + dt.Rows[t]["GROUPID"].ToString() + "&Station=" + Station + "&Type=TOTAL_PALLET";
                            linkTable.Rows.Add(linkDataRow);
                        }
                        ReportTable reportTable = new ReportTable();
                        reportTable.LoadData(dt, linkTable);
                        reportTable.Tittle = "Query Model";
                        Outputs.Add(reportTable);
                    }
                    catch (Exception exception)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                        throw exception;
                    }
                }
            }
            catch (Exception e)
            {
                Outputs.Add(new ReportAlart(e.Message));
            }
        }
    }
}
