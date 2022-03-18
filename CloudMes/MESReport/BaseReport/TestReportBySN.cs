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
    // <copyright file="TestReportBySN.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-05-11 </date>
    /// <summary>
    /// TestReportBySN
    /// </summary>
    public class TestReportBySN : ReportBase
    {

        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStationName = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStateType = new ReportInput() { Name = "StateType", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "PASS", "FAIL" } };
        ReportInput inputWO = new ReportInput() { Name = "workorderno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSKU = new ReportInput() { Name = "skuno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        string sqlGetSation = "";
        ReportTable reportTable = null;
        public TestReportBySN()
        {
            Inputs.Add(inputStartDate);
            Inputs.Add(inputEndDate);
            Inputs.Add(inputSN);
            Inputs.Add(inputStationName);
            Inputs.Add(inputStateType);
            Inputs.Add(inputWO);
            Inputs.Add(inputSKU);

            sqlGetSation = "select distinct te_station from c_temes_station_mapping order by te_station";
            Sqls.Add("GetSation", sqlGetSation);
        }
        public override void Init()
        {
            //base.Init();
            inputStartDate.Value = DateTime.Now.AddDays(-1);
            inputEndDate.Value = DateTime.Now;
            inputStationName.ValueForUse = GetStation();
            reportTable = new ReportTable();
            reportTable.ColNames.Add("WORKORDERNO");
            reportTable.ColNames.Add("SKUNO");
            reportTable.ColNames.Add("MODEL");
            reportTable.ColNames.Add("SN");
            reportTable.ColNames.Add("STATE");
            reportTable.ColNames.Add("STATION");
            reportTable.ColNames.Add("CELL");
            reportTable.ColNames.Add("OPERATOR");
            reportTable.ColNames.Add("ERROR_CODE");
            reportTable.ColNames.Add("BURNIN_TIME");
            reportTable.ColNames.Add("CREATETIME");
            //reportTable.ColNames.Add("EDIT_TIME");
            //reportTable.ColNames.Add("EDIT_EMP");
            Outputs.Add(new ReportColumns(reportTable.ColNames));         
            PaginationServer = true;
        }

        public override void Run()
        {
            //base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {               
                string sn = inputSN.Value.ToString();
                string station = inputStationName.Value.ToString();
                string state = inputStateType.Value.ToString();
                string wo = inputWO.Value.ToString();
                string skuno = inputSKU.Value.ToString();
                string sql = "";
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                if (inputStartDate.Value.ToString() != "")
                {
                    startDate = (DateTime)inputStartDate.Value;
                }
                if (inputStartDate.Value.ToString() != "")
                {
                    endDate = (DateTime)inputEndDate.Value;
                }
                DataTable showTable = new DataTable();
                GetRunSql(sn, station, state, wo, skuno);
                if (PaginationServer && PageSize != 0 && PageNumber != 0)
                {                    
                    sql = $@"select count(*) from ({Sqls["RunSql"]})";                    
                    string total_rows = SFCDB.RunSelect(sql).Tables[0].Rows[0][0].ToString();
                    reportTable.TotalRows = Convert.ToInt32(total_rows);
                    if (reportTable.TotalRows == 0)
                    {
                        throw new Exception("No Data!");
                    }
                    sql = $@"select workorderno,skuno,Model,SN,STATE,STATION,CELL,OPERATOR,ERROR_CODE,BURNIN_TIME,CREATETIME 
                                    from (select temp.*,rownum as rn from ({Sqls["RunSql"]} ) temp )
                                    where rn> ({PageNumber} - 1) * {PageSize} and rn<= {PageNumber} * {PageSize}";
                    //sql = $@"select workorderno,skuno,SN,STATE,STATION,EDIT_TIME,EDIT_EMP 
                    //                from (select temp.*,rownum as rn from ({Sqls["RunSql"]} ) temp )
                    //                where rn> ({PageNumber} - 1) * {PageSize} and rn<= {PageNumber} * {PageSize}";
                    showTable = SFCDB.RunSelect(sql).Tables[0];
                    reportTable.PaginationServer = PaginationServer;
                }
                else
                {
                    showTable = SFCDB.RunSelect(Sqls["RunSql"]).Tables[0];
                    if (showTable.Rows.Count == 0)
                    {
                        throw new Exception("No Data!");
                    }
                }

                DataTable countTable = new DataTable();
                DataTable countLinkTable = new DataTable();
                if (station != "ALL")
                {
                    string countLinkUrl = $@"Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.TestReportCountDetail&RunFlag=1&Station={station}&StateType={state}&workorderno={wo}&skuno={skuno}&StartDate={startDate}&EndDate={endDate}&CountFlag=";
                    //MakeCountTable(tempSql, SFCDB, countLinkUrl, out countTable, out countLinkTable);
                    MakeCountTableByDataTable(SFCDB.RunSelect(Sqls["RunSql"]).Tables[0], SFCDB, countLinkUrl, out countTable, out countLinkTable);
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }

                //TableRowView rowView = new TableRowView();
                //TableColView colView = new TableColView();
                //for (int i = 0; i < showTable.Rows.Count; i++)
                //{
                //    rowView = new TableRowView();
                //    foreach (string col_name in reportTable.ColNames)
                //    {
                //        colView = new TableColView();
                //        colView.ColName = col_name;
                //        colView.Value = dtTestReport.Rows[i][col_name].ToString();
                //        if (col_name== "SN")
                //        {
                //            colView.LinkType = "Link";
                //            colView.LinkData = "FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + dtTestReport.Rows[i]["SN"].ToString();
                //        }
                //        rowView.RowData.Add(colView.ColName, colView);
                //    }
                //    reportTable.Rows.Add(rowView);
                //}
                DataTable linkTable = new DataTable();
                DataRow linkRow = null;             
                foreach (var c in showTable.Columns)
                {
                    linkTable.Columns.Add(c.ToString());
                }
                for (int i = 0; i < showTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    foreach (var c in linkTable.Columns)
                    {

                        if (c.ToString() == "SN")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + showTable.Rows[i]["SN"].ToString();
                        }
                        else
                        {
                            linkRow[c.ToString()] = "";
                        }
                    }                    
                    linkTable.Rows.Add(linkRow);
                }
                reportTable.LoadData(showTable, linkTable);
                reportTable.Tittle = "SN TEST REPORT";
                Outputs.Add(reportTable);
                if (station != "ALL")
                {
                    ReportTable reportCountTable = new ReportTable();
                    reportCountTable.LoadData(countTable, countLinkTable);
                    reportCountTable.Tittle = "SN TEST Count";
                    Outputs.Add(reportCountTable);
                }
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(exception.Message);
                Outputs.Add(alart);               
            }
        }

        public override void DownFile()
        {
            string sn = inputSN.Value.ToString();
            string station = inputStationName.Value.ToString();
            string state = inputStateType.Value.ToString();
            string wo = inputWO.Value.ToString();
            string skuno = inputSKU.Value.ToString();
            GetRunSql(sn, station, state, wo, skuno);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = SFCDB.RunSelect(Sqls["RunSql"]).Tables[0];
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "TestReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";               
                Outputs.Add(new ReportFile(fileName, content));             
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        private void GetRunSql(string sn,string station,string state,string wo,string skuno)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            string tempSql = "";
            //string runSql = $@" select skuno  Model,sn,state,station,cell,operator,error_code,createtime from r_test_detail_vertiv where 1=1 ";
            string runSql = $@"select b.workorderno,b.skuno, a.skuno  Model,a.sn,a.state,a.station,a.cell,a.operator,a.error_code,a.BURNIN_TIME, a.createtime 
                                    from r_test_detail_vertiv a ,r_sn b,r_test_record c where 1=1 and a.r_test_record_id=c.id  and c.r_sn_id = b.id(+) ";
            //string runSql = $@"select b.workorderno, b.skuno, b.sn, c.state, c.messtation station, c.edit_time, c.edit_emp 
            //                    from r_sn b, r_test_record c where 1 = 1 and c.r_sn_id = b.id(+) ";
            if (inputStartDate.Value.ToString() != "")
            {
                startDate = (DateTime)inputStartDate.Value;
            }
            if (inputStartDate.Value.ToString() != "")
            {
                endDate = (DateTime)inputEndDate.Value;
            }
            if (inputStartDate.Value.ToString() != "" && inputStartDate.Value.ToString() != "")
            {
                runSql = runSql + $@" and a.createtime   between to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') 
                                and  to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') ";
                //runSql = runSql + $@" and c.endtime between to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') 
                //                    and to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') ";
            }

            if (sn != "")
            {
                runSql = runSql + $@" and a.sn='{sn}' ";
                //runSql = runSql + $@" and b.sn='{sn}' ";
            }
            if (station != "ALL")
            {
                runSql = runSql + $@" and a.station='{station}' ";
                //runSql = runSql + $@" and c.messtation='{station}' ";
            }
            if (state != "ALL")
            {
                runSql = runSql + $@" and a.state='{state}' ";
                //runSql = runSql + $@" and c.state='{state}' ";
            }
            if (wo != "ALL")
            {
                runSql = runSql + $@" and b.workorderno = '{wo}'";
            }
            if (skuno != "ALL")
            {
                runSql = runSql + $@" and b.skuno = '{skuno}'";
            }
            tempSql = runSql;
            runSql += " order by b.workorderno,a.createtime asc";
            //runSql += " order by b.workorderno,c.endtime asc";
            RunSqls.Add(runSql);
            Sqls.Remove("RunSql"); 
            Sqls.Add("RunSql", runSql);
            //return SFCDB.RunSelect(runSql).Tables[0];
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
                        stationList.Add(row["te_station"].ToString());
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
            finally {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }           
        }

        private void MakeCountTable(string resultSql, OleExec db, string linkUrl, out DataTable countTable, out DataTable countLinkTable)
        {
            DataTable _countTable = new DataTable();
            DataTable _countLinkTable = new DataTable();
            string retestCountSql = $@"select nvl(sum(decode(test_time,1,0,test_time-1)), 0) from (select sn,count(*) as test_time from ({resultSql})group by sn )";
            string testCountSql = $@"select count(*) from ({resultSql})";
            string firstPassCountSql = $@"select count(*) from (select workorderno,skuno,Model,sn,state,station,cell,operator,error_code,createtime,
                                                        ROW_NUMBER() over(partition by sn order by createtime asc) as rownums
                                                          from ({resultSql})) where rownums = 1 and state = 'PASS'";
            string goodProductCountSql = $@"select count(distinct sn) from ({resultSql}) mm where mm.state = 'PASS' ";

            double retestCount = Convert.ToDouble(db.RunSelect(retestCountSql).Tables[0].Rows[0][0].ToString());
            double testCount = Convert.ToDouble(db.RunSelect(testCountSql).Tables[0].Rows[0][0].ToString());
            double firstPassCount = Convert.ToDouble(db.RunSelect(firstPassCountSql).Tables[0].Rows[0][0].ToString());
            double goodProductCount = Convert.ToDouble(db.RunSelect(goodProductCountSql).Tables[0].Rows[0][0].ToString());
            double testSNCount = testCount - retestCount;
            double fpy = testSNCount == 0 ? 0 : firstPassCount / testSNCount;
            double retestRate = testSNCount == 0 ? 0 : retestCount / testSNCount;
            double yield = testSNCount == 0 ? 0 : goodProductCount / testSNCount;
            //_countTable.Columns.Add("重測次數");
            //_countTable.Columns.Add("總測試次數");
            //_countTable.Columns.Add("測試SN總數");
            //_countTable.Columns.Add("首次PASS數");
            //_countTable.Columns.Add("直通率（FPY）");
            //_countTable.Columns.Add("重測率");
            //_countTable.Columns.Add("良品");
            //_countTable.Columns.Add("良品率");
            //_countTable.Columns.Add("不良品");
            //_countTable.Columns.Add("不良品率");
            //DataRow countRow = _countTable.NewRow();
            //countRow["重測次數"] = retestCount;
            //countRow["總測試次數"] = testCount;
            //countRow["測試SN總數"] = testSNCount;
            //countRow["首次PASS數"] = firstPassCount;
            //countRow["直通率（FPY）"] = Math.Round(fpy * 100, 2) + "%";
            //countRow["重測率"] = Math.Round(retestRate * 100, 2) + "%";
            //countRow["良品"] = goodProductCount;
            //countRow["良品率"] = Math.Round(yield * 100, 2) + "%";
            //countRow["不良品"] = testSNCount - goodProductCount;
            //countRow["不良品率"] = (goodProductCount == 0 ? 0 : Math.Round((testSNCount - goodProductCount) / testSNCount * 100, 2)) + "%";
            //_countTable.Rows.Add(countRow);

            //_countLinkTable.Columns.Add("重測次數");
            //_countLinkTable.Columns.Add("總測試次數");
            //_countLinkTable.Columns.Add("測試SN總數");
            //_countLinkTable.Columns.Add("首次PASS數");
            //_countLinkTable.Columns.Add("直通率（FPY）");
            //_countLinkTable.Columns.Add("重測率");
            //_countLinkTable.Columns.Add("良品");
            //_countLinkTable.Columns.Add("良品率");
            //_countLinkTable.Columns.Add("不良品");
            //_countLinkTable.Columns.Add("不良品率");
            //DataRow countLinkRow = _countLinkTable.NewRow();
            //countLinkRow["重測次數"] = retestCount == 0 ? "" : linkUrl + "RETEST";
            //countLinkRow["總測試次數"] = "";
            //countLinkRow["測試SN總數"] = "";
            //countLinkRow["首次PASS數"] = firstPassCount == 0 ? "" : linkUrl + "FIRST_PASS";
            //countLinkRow["直通率（FPY）"] = "";
            //countLinkRow["重測率"] = "";
            //countLinkRow["良品"] = "";
            //countLinkRow["良品率"] = "";
            //countLinkRow["不良品"] = (testSNCount - goodProductCount) == 0 ? "" : linkUrl + "DEFECTIVE";
            //countLinkRow["不良品率"] = "";

            _countTable.Columns.Add("Retest times");//重測次數
            _countTable.Columns.Add("Total tests");//總測試次數
            _countTable.Columns.Add("Total tested SN");//測試SN總數
            _countTable.Columns.Add("First pass number");//首次PASS數
            _countTable.Columns.Add("Direct rate（FPY）");//直通率（FPY）
            _countTable.Columns.Add("Retest rate");//重測率
            _countTable.Columns.Add("Good product");//良品
            _countTable.Columns.Add("Yield rate");//良品率
            _countTable.Columns.Add("rejects");//不良品
            _countTable.Columns.Add("Defective rate");//不良品率
            DataRow countRow = _countTable.NewRow();
            countRow["Retest times"] = retestCount;//重測次數
            countRow["Total tests"] = testCount;//總測試次數
            countRow["Total tested SN"] = testSNCount;//測試SN總數
            countRow["First pass number"] = firstPassCount;//首次PASS數
            countRow["Direct rate（FPY）"] = Math.Round(fpy * 100, 2) + "%";//直通率（FPY）
            countRow["Retest rate"] = Math.Round(retestRate * 100, 2) + "%"; //重測率
            countRow["Good product"] = goodProductCount;//良品
            countRow["Yield rate"] = Math.Round(yield * 100, 2) + "%";//良品率
            countRow["rejects"] = testSNCount - goodProductCount;//不良品
            countRow["Defective rate"] = Math.Round((testSNCount - goodProductCount) / testSNCount * 100, 2) + "%";//不良品率
            _countTable.Rows.Add(countRow);

            _countLinkTable.Columns.Add("Retest times");//重測次數
            _countLinkTable.Columns.Add("Total tests");//總測試次數
            _countLinkTable.Columns.Add("Total tested SN");//測試SN總數
            _countLinkTable.Columns.Add("First pass number");//首次PASS數
            _countLinkTable.Columns.Add("Direct rate（FPY）");//直通率（FPY）
            _countLinkTable.Columns.Add("Retest rate"); //重測率
            _countLinkTable.Columns.Add("Good product");//良品
            _countLinkTable.Columns.Add("Yield rate");//良品率
            _countLinkTable.Columns.Add("rejects");//不良品
            _countLinkTable.Columns.Add("Defective rate");//不良品率
            DataRow countLinkRow = _countLinkTable.NewRow();
            countLinkRow["Retest times"] = retestCount == 0 ? "" : linkUrl + "RETEST";//重測次數
            countLinkRow["Total tests"] = "";//總測試次數
            countLinkRow["Total tested SN"] = "";//測試SN總數
            countLinkRow["First pass number"] = firstPassCount == 0 ? "" : linkUrl + "FIRST_PASS";//首次PASS數
            countLinkRow["Direct rate（FPY）"] = "";//直通率（FPY）
            countLinkRow["Retest rate"] = "";//重測率
            countLinkRow["Good product"] = "";//良品
            countLinkRow["Yield rate"] = "";//良品率
            countLinkRow["rejects"] = (testSNCount - goodProductCount) == 0 ? "" : linkUrl + "DEFECTIVE";//不良品
            countLinkRow["Defective rate"] = "";//不良品率
            _countLinkTable.Rows.Add(countLinkRow);

            countTable = _countTable;
            countLinkTable = _countLinkTable;
        }

        List<string> yieldColumns = new List<string>() 
        { 
            "SKUNO", 
            "MODEL", 
            "重測次數", 
            "總測試次數", 
            "測試SN總數", 
            "首次PASS數", 
            "直通率(FPY)", 
            "重測率",
            "良品",
            "良品率",
            "不良品",
            "不良品率"
        };
        private void MakeCountTableByDataTable(DataTable dt, OleExec db, string linkUrl, out DataTable countTable, out DataTable countLinkTable)
        {
            DataTable _countTable = new DataTable();
            DataTable _countLinkTable = new DataTable();
            foreach (var columns in yieldColumns)
            {
                _countTable.Columns.Add(columns);
                _countLinkTable.Columns.Add(columns);
            }  
            List<CountObject> listAllCount = new List<CountObject>();
            foreach (DataRow row in dt.Rows)
            {
                CountObject countObj = new CountObject();
                countObj.Workorderno = row["WORKORDERNO"] == null ? "" : row["WORKORDERNO"].ToString();
                countObj.Skuno = row["SKUNO"] == null ? "" : row["SKUNO"].ToString();
                countObj.Model = row["MODEL"] == null ? "" : row["MODEL"].ToString();
                countObj.SN = row["SN"] == null ? "" : row["SN"].ToString();
                countObj.State = row["STATE"] == null ? "" : row["STATE"].ToString();
                countObj.Station = row["STATION"] == null ? "" : row["STATION"].ToString();
                countObj.Cell = row["CELL"] == null ? "" : row["CELL"].ToString();
                countObj.Operator = row["OPERATOR"] == null ? "" : row["OPERATOR"].ToString();
                countObj.ErrorCode = row["ERROR_CODE"] == null ? "" : row["ERROR_CODE"].ToString();
                countObj.CreateTime = (DateTime)row["CREATETIME"];
                listAllCount.Add(countObj);
            }
            List<string> listSku = listAllCount.Select(l => l.Skuno).Distinct().ToList();
            if (listSku.Count > 1)
            {
                listSku.Add("ALL");
            }
            List<CountObject> listCount = null;          
            foreach (string s in listSku)
            {              
                listCount = new List<CountObject>();
                if (s == "ALL")
                {
                    listCount = listAllCount;
                }
                else
                {
                    listCount = listAllCount.Where(l => l.Skuno == s).ToList();
                }
                var reTest = from l in listCount
                             group l by l.SN into r
                             select new
                             {
                                 sn = r.Key,
                                 count = r.Count()
                             };
                var firstPass = listCount.OrderBy(x => x.CreateTime).GroupBy(x => x.SN).
                    Select(
                    group => new
                    {
                        Group = group,
                        Count = group.Count()
                    })
                    .SelectMany(
                    groupWithCount => groupWithCount.Group.Select(b => b).Zip(Enumerable.Range(1, groupWithCount.Count),
                    (j, i) => new { j.SN, j.State, j.CreateTime, RowNumber = i })).Where(p => p.State == "PASS" && p.RowNumber == 1).ToList();
                var goodProduct = listCount.Where(l => l.State == "PASS").Select(l => l.SN).Distinct().ToList();

                double testCount = listCount.Count;
                double reTestCount = 0;
                double firstPassCount = firstPass.Count;
                double goodProductCount = goodProduct.Count;

                foreach (var re in reTest)
                {
                    if (re.count != 1 && re.count != 0 && re.sn != "")
                    {
                        reTestCount = reTestCount + (re.count - 1);
                    }
                }

                double testSNCount = testCount - reTestCount;
                double fpy = firstPassCount / testSNCount;
                double retestRate = reTestCount / testSNCount;
                double yield = goodProductCount / testSNCount;

                DataRow countRow = _countTable.NewRow();
                countRow["SKUNO"] = s;
                countRow["MODEL"] = s == "ALL" ? "ALL" : listCount[0].Model;
                countRow["重測次數"] = reTestCount;
                countRow["總測試次數"] = testCount;
                countRow["測試SN總數"] = testSNCount;
                countRow["首次PASS數"] = firstPassCount;
                countRow["直通率(FPY)"] = Math.Round(fpy * 100, 2) + "%";
                countRow["重測率"] = Math.Round(retestRate * 100, 2) + "%";
                countRow["良品"] = goodProductCount;
                countRow["良品率"] = Math.Round(yield * 100, 2) + "%";
                countRow["不良品"] = testSNCount - goodProductCount;
                countRow["不良品率"] = Math.Round((testSNCount - goodProductCount) / testSNCount * 100, 2) + "%";
                _countTable.Rows.Add(countRow);

                DataRow countLinkRow = _countLinkTable.NewRow();
                countLinkRow["SKUNO"] = "";
                countLinkRow["MODEL"] ="";
                countLinkRow["重測次數"] = reTestCount == 0 ? "" : (s == "ALL" ? linkUrl + "RETEST" : linkUrl + "RETEST" + "&skuno=" + s);
                countLinkRow["總測試次數"] = "";
                countLinkRow["測試SN總數"] = "";
                countLinkRow["首次PASS數"] = firstPassCount == 0 ? "" : (s == "ALL" ? linkUrl + "FIRST_PASS" : linkUrl + "FIRST_PASS" + "&skuno=" + s);
                countLinkRow["直通率(FPY)"] = "";
                countLinkRow["重測率"] = "";
                countLinkRow["良品"] = "";
                countLinkRow["良品率"] = "";
                countLinkRow["不良品"] = (testSNCount - goodProductCount) == 0 ? "" : (s == "ALL" ? linkUrl + "DEFECTIVE" : linkUrl + "DEFECTIVE" + "&skuno=" + s);
                countLinkRow["不良品率"] = "";
                _countLinkTable.Rows.Add(countLinkRow);
            }

            countTable = _countTable;
            countLinkTable = _countLinkTable;
        }       
    
        public DataTable GetSendAlertEmail(OleExec db)
        {
            try
            {
                List<C_CONTROL> controlStation = db.ORM.Queryable<C_CONTROL>()
                    .Where(r => r.CONTROL_NAME == "SendYieldAlartEmail" && r.CONTROL_TYPE == "STATION").ToList();

                DataTable excelTable = new DataTable("良品率不達標");
                excelTable.Columns.Add("Station");
                foreach (var columns in yieldColumns)
                {
                    excelTable.Columns.Add(columns);                   
                }
                foreach (var control in controlStation)
                {
                    string station = control.CONTROL_VALUE;
                    if (string.IsNullOrEmpty(station))
                    {
                        continue;
                    }
                    double yield = Convert.ToDouble(control.CONTROL_LEVEL);
                    GetRunSql("", station, "ALL", "ALL", "ALL");
                    DataTable stationTable = db.RunSelect(Sqls["RunSql"]).Tables[0];
                    DataTable yieldTable = new DataTable();
                    DataTable linkTable = new DataTable();
                    if (stationTable.Rows.Count > 0)
                    {
                        MakeCountTableByDataTable(stationTable, db, "", out yieldTable, out linkTable);
                        foreach (DataRow row in yieldTable.Rows)
                        {
                            double goodYield = Convert.ToDouble(row["良品率"].ToString().Replace("%", ""));
                            if (goodYield < yield)
                            {
                                DataRow newRow = excelTable.NewRow();
                                newRow["Station"] = station;
                                foreach (var columns in yieldColumns)
                                {
                                    newRow[columns] = row[columns];
                                }
                                excelTable.Rows.Add(newRow);
                            }
                        }
                    }

                }
                return excelTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }

    public class CountObject
    {
        public string Workorderno { get; set; }
        public string Skuno { get; set; }
        public string Model { get; set; }
        public string SN { get; set; }
        public string State { get; set; }
        public string Station { get; set; }
        public string Cell { get; set; }
        public string Operator { get; set; }
        public string ErrorCode { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
