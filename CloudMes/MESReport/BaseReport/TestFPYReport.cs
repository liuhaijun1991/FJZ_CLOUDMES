using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class TestFPYReport : ReportBase
    {
        
        ReportInput inputDateType = new ReportInput() { Name = "DateType", InputType = "TXT", Value = "ByWeeks", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput() { Name = "CType", InputType = "Select", Value = "Structural", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "Structural", "Functional" } };
        public TestFPYReport()
        {
            Inputs.Add(inputDateType);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
            Inputs.Add(inputSkuno);
            Inputs.Add(inputType);


        }
        public override void Init()
        {
            inputDateFrom.Value = DateTime.Now.AddDays(-7).ToString("yyyy/MM/dd");
            inputDateTo.Value = DateTime.Now.ToString("yyyy/MM/dd");
            string sqlGetSkuno = $@"select distinct skuno from c_sku where id in (
                                    select sku_id from r_sku_route where route_id in (
                                    select distinct route_id from c_route_detail where station_name in (select mes_station from c_temes_station_mapping)))
                                    order by skuno";
            string sqlGetStation = $@"select distinct mes_station as station_name  from c_temes_station_mapping  order by mes_station";
            Sqls.Add("GetSkuno", sqlGetSkuno);
            Sqls.Add("GetStation", sqlGetStation);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtSkuno = SFCDB.RunSelect(Sqls["GetSkuno"]).Tables[0];


            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            List<string> skunoList = new List<string>();
            skunoList.Add("ALL");
            foreach (DataRow row in dtSkuno.Rows)
            {
                skunoList.Add(row["skuno"].ToString());
            }
            inputSkuno.Value = skunoList[0];
            inputSkuno.ValueForUse = skunoList;
        }
        public override void Run()
        {
            if (inputDateFrom.Value == null || inputDateTo.Value == null)
            {
                throw new Exception("Please Input DateFrom And DateTo!");
            }
            string skuno = inputSkuno.Value.ToString();
            //string dateFrom = Convert.ToDateTime(inputDateFrom.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd hh:mm:ss");
            //string dateTo = Convert.ToDateTime(inputDateTo.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd hh:mm:ss");

            DateTime sTime = Convert.ToDateTime(inputDateFrom.Value);
            DateTime eTime = Convert.ToDateTime(inputDateTo.Value).AddDays(1);
            string dateFrom = sTime.ToString("yyyy-MM-dd");
            string dateTo = eTime.ToString("yyyy-MM-dd");

            string dateType = inputDateType.Value.ToString();
            string runSql;
            string type = inputType.Value.ToString();
            string skuSql = $@"";
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                string bu = SFCDB.ORM.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();

                string sqlCount = bu == "VNDCN" ? "COUNT(distinct sn)" : bu == "VNJUNIPER" ? "COUNT(distinct sn)" : "count(*)";
                #region Update Old Logic Requst by 沈敏志
                //if (skuno.ToUpper().Equals("ALL"))
                //{
                //    skuSql = $@" skuno in (select distinct skuno from c_sku where id in (
                //                    select sku_id from r_sku_route where route_id in (
                //                    select distinct route_id from c_route_detail where station_name in (select mes_station from c_temes_station_mapping))))";
                //}
                //else {
                //    skuSql = $@"skuno = '{skuno}'";
                //}
                //if (type == "Structual")
                //{
                //    tempSql = $@" {skuSql}  and station_name in (select mes_station from c_temes_station_mapping where TEGROUP='STRUCTURAL') and edit_time between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";
                //}
                //else
                //{

                //    tempSql = $@"  {skuSql}  and station_name in (select mes_station from c_temes_station_mapping where TEGROUP='FUNCTIONAL') and edit_time between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";
                //}

                //runSql = $@"select to_char(edit_time,'IW') || 'Week' as datetime ,{sqlCount} as qty,skuno, STATUS from (select  decode(repair_failed_flag, '0', 'PASS', '1', 'FAIL') as STATUS,  STATION_NAME,skuno,sn, edit_time,row_number() over(partition by sn,skuno,STATION_NAME order by STATION_NAME,edit_time asc ) ro From r_sn_station_detail 
                //           where VALID_FLAG =1 AND   {tempSql} ) WHERE RO = 1 group by to_char(edit_time,'IW'),STATUS,skuno ORDER BY datetime";
                #endregion
                if (!skuno.ToUpper().Equals("ALL"))
                {
                    skuSql = $@"AND CSKU.SKUNO = '{skuno}'";
                }

                runSql = $@"SELECT to_char(SS.TATIME,'IW') || 'Week' as datetime, SS.SKUNO,RM.CUSTPN BASEPID,COUNT(SS.SYSSERIALNO) QTY,SS.STATUS FROM (
                            SELECT ROW_NUMBER() OVER(PARTITION BY SYSSERIALNO,EVENTNAME ORDER BY TATIME) RN ,CSKU.SKUNO,RTJ.SYSSERIALNO,CTS.MES_STATION STATION,RTJ.STATUS,RTJ.TATIME
                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                            AND RTJ.EVENTNAME=CTS.TE_STATION 
                            AND CSKU.ID=RSR.SKU_ID
                            AND CRD.STATION_NAME=CTS.MES_STATION
                            AND RTJ.SYSSERIALNO=RSN.SN
                            AND UPPER(RTJ.STATUS) IN('PASS','FAIL') {skuSql}
                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE TEGROUP='{type.ToUpper()}') 
                            AND RTJ.TATIME> TO_DATE('{dateFrom}','YYYY/MM/DD HH24:MI:SS') AND RTJ.TATIME<TO_DATE('{dateTo}','YYYY/MM/DD  HH24:MI:SS') )SS
                            LEFT JOIN r_modelsubpn_map RM ON SS.SKUNO=RM.SUBPARTNO  AND RM.FLAG=1  WHERE SS.RN=1 AND UPPER(SS.STATUS)='FAIL'
                            GROUP BY  to_char(SS.TATIME,'IW'), SS.SKUNO,RM.CUSTPN,SS.STATUS,RM.CUSTPN
                            UNION
                            SELECT AA.datetime,AA.SKUNO,RM.CUSTPN BASEPID,SUM(AA.C) QTY,'' FROM (
                            SELECT to_char(RTJ.TATIME,'IW') || 'Week' as datetime, CSKU.SKUNO, CTS.MES_STATION ,COUNT(DISTINCT SYSSERIALNO)C
                            FROM R_TEST_JUNIPER RTJ ,C_SKU CSKU ,R_SKU_ROUTE RSR,C_ROUTE_DETAIL CRD ,C_TEMES_STATION_MAPPING CTS,R_SN RSN
                            WHERE RTJ.PART_NUMBER=CSKU.SKUNO
                            AND RTJ.EVENTNAME=CTS.TE_STATION 
                            AND CSKU.ID=RSR.SKU_ID
                            AND CRD.STATION_NAME=CTS.MES_STATION
                            AND RTJ.SYSSERIALNO=RSN.SN
                            AND UPPER(RTJ.STATUS) IN('PASS','FAIL') {skuSql}
                            AND CTS.TE_STATION IN (SELECT TE_STATION FROM C_TEMES_STATION_MAPPING WHERE TEGROUP='{type.ToUpper()}')  
                            AND RTJ.TATIME> TO_DATE('{dateFrom}','YYYY/MM/DD HH24:MI:SS') AND RTJ.TATIME<TO_DATE('{dateTo}','YYYY/MM/DD  HH24:MI:SS')GROUP BY to_char(RTJ.TATIME,'IW'), CTS.MES_STATION,CSKU.SKUNO )AA 
                            LEFT JOIN r_modelsubpn_map RM ON AA.SKUNO=RM.SUBPARTNO  AND RM.FLAG=1
                            GROUP BY AA.datetime,AA.SKUNO,RM.CUSTPN,RM.CUSTPN";

                    RunSqls.Add(runSql);

                DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception(" No Data!");
                }
                Dictionary<string, List<FPYReportModel>> objValue = new Dictionary<string, List<FPYReportModel>>();
                List<FPYReportModel> listFPY = new List<FPYReportModel>();
                List<FPYReportModel> skuFPY = new List<FPYReportModel>();
                string date = "";
                int totalQty = 0;
                //int passQty = 0;
                int failQty = 0;
                string _skuno = "";
                DataTable dateTimeTable = dt.DefaultView.ToTable(true, "datetime");
                DataRow[] TotalRow;
                //DataRow[] passRow;
                DataRow[] failRow;
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    date = dt.Rows[x]["datetime"].ToString();
                    _skuno = dt.Rows[x]["skuno"].ToString();
                    TotalRow = dt.Select(" status is null and skuno = '" + dt.Rows[x]["skuno"].ToString() + "' and datetime='" + dt.Rows[x]["datetime"].ToString() + "'");
                    //passRow = dt.Select(" status= 'PASS' and skuno = '" + dt.Rows[x]["skuno"].ToString() + "' and datetime='" + dt.Rows[x]["datetime"].ToString() + "'");
                    failRow = dt.Select(" status= 'FAIL' and skuno ='" + dt.Rows[x]["skuno"].ToString() + "' and  datetime='" + dt.Rows[x]["datetime"].ToString() + "'");
                    totalQty = TotalRow.Count() > 0 ? Convert.ToInt32(TotalRow[0]["QTY"].ToString()) : 0;
                    //passQty = passRow.Count() > 0 ? Convert.ToInt32(passRow[0]["QTY"].ToString()) : 0;
                    failQty = failRow.Count() > 0 ? Convert.ToInt32(failRow[0]["QTY"].ToString()) : 0;
                    listFPY.Add(new FPYReportModel(date, failQty, totalQty, _skuno));
                }

                DataRow linkDataRow = null;
                DataTable resdt = new DataTable();

                resdt.Columns.Add("SKUNO");
                if (type == "Functional")
                {
                    resdt.Columns.Add("BASEPID");
                }
                resdt.Columns.Add("Item\\Date");
                foreach (DataRow row in dateTimeTable.Rows)
                {
                    resdt.Columns.Add(row["datetime"].ToString());
                }

                DataTable skunoTable = dt.DefaultView.ToTable(true, "skuno", "BASEPID");
                foreach (DataRow skunoRow in skunoTable.Rows)
                {
                    if (listFPY.FindAll(x => x.Skuno == skunoRow["skuno"].ToString()).Count > 0) {

                        objValue.Add(skunoRow["skuno"].ToString(), listFPY.FindAll(x => x.Skuno == skunoRow["skuno"].ToString())) ;
                    }
                    linkDataRow = resdt.NewRow();
                    linkDataRow["Item\\Date"] = "Total Qty(pcs)";
                    linkDataRow["SKUNO"] = skunoRow["SKUNO"].ToString();
                    if (type == "Functional")
                    {
                        linkDataRow["BASEPID"] = skunoRow["BASEPID"].ToString();
                    }
                    resdt.Rows.Add(linkDataRow);

                    linkDataRow = resdt.NewRow();
                    linkDataRow["Item\\Date"] = "Pass Qty(pcs)";
                    resdt.Rows.Add(linkDataRow);

                    linkDataRow = resdt.NewRow();
                    linkDataRow["Item\\Date"] = "Fail Qty(pcs)";
                    resdt.Rows.Add(linkDataRow);

                    linkDataRow = resdt.NewRow();
                    linkDataRow["Item\\Date"] = "FPY Rate(%)";
                    resdt.Rows.Add(linkDataRow);

                }


                List<ItemModel> listItem1 = new List<ItemModel>();
                listItem1.Add(new ItemModel { Name = "TotalQty", TempName = "Total Qty(pcs)" });
                listItem1.Add(new ItemModel { Name = "PassQty", TempName = "Pass Qty(pcs)" });
                listItem1.Add(new ItemModel { Name = "FailQty", TempName = "Fail Qty(pcs)" });
                listItem1.Add(new ItemModel { Name = "FPYRate", TempName = "FPY Rate(%)" });

                for (int j = 0; j < resdt.Rows.Count; j++)
                {
                    if (resdt.Rows[j]["SKUNO"].ToString() == "")
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var item in objValue)
                        {
                            foreach (FPYReportModel fpy1 in item.Value)
                            {

                                if (resdt.Rows[j]["SKUNO"].ToString() == fpy1.Skuno)
                                {
                                    for (int k = 0; k < resdt.Columns.Count; k++)
                                    {

                                        if (resdt.Columns[k].ColumnName == fpy1.Date)
                                        {
                                            foreach (ItemModel im in listItem1)
                                            {
                                                switch (im.Name)
                                                {

                                                    case "TotalQty":
                                                        resdt.Rows[j][k] = fpy1.TotalQty;

                                                        break;
                                                    case "PassQty":
                                                        resdt.Rows[j + 1][k] = fpy1.PassQty;
                                                        break;
                                                    case "FailQty":
                                                        resdt.Rows[j + 2][k] = fpy1.FailQty;
                                                        break;
                                                    case "FPYRate":
                                                        resdt.Rows[j + 3][k] = fpy1.FPYRate + "%";
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                #region Show Table
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resdt, null);
                retTab.Tittle = "Test FPY Report";
                Outputs.Add(retTab);
                #endregion     
            }
            catch (Exception ex) {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }

        public class FPYReportModel
        {
            private string _date = "";
            private double _fpyRate;
            private double _failRate;
            private int _totalQty;
            private int _passQty = 0;
            private int _failQty = 0;
            private string _skuno;

            public string Date
            {
                get { return _date; }
            }
            public int TotalQty
            {
                get { return _totalQty; }
            }
            public int PassQty
            {
                get { return _passQty; }
            }
            public int FailQty
            {
                get { return _failQty; }
            }
            public string Skuno
            {

                get { return _skuno; }
            }
            public double FPYRate
            {
                get { return _fpyRate; }
            }
            public double FailRate
            {
                get { return _failRate; }
            }
            public FPYReportModel(string date, int failQty,int totalQty, string skuno)
            {
                _date = date;
                _passQty = totalQty - failQty;
                _failQty = failQty;
                _totalQty = totalQty;
                _failRate = Math.Round(Convert.ToDouble(_failQty) / Convert.ToDouble(_totalQty) * 100, 2);
                _fpyRate = Math.Round(Convert.ToDouble(_passQty) / Convert.ToDouble(_totalQty) * 100, 2);
                _skuno = skuno;
            }
        }

    public class ItemModel
        {
            public string Skuno { get; set; }
            public string Name { get; set; }
            public string TempName { get; set; }
        }
       
    }
}