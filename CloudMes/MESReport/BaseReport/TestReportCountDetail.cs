using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class TestReportCountDetail : ReportBase
    {
        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStationName = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStateType = new ReportInput() { Name = "StateType", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "workorderno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSKU = new ReportInput() { Name = "skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCountFlag = new ReportInput() { Name = "CountFlag", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };

        public TestReportCountDetail()
        {
            Inputs.Add(inputStartDate);
            Inputs.Add(inputEndDate);
            Inputs.Add(inputStationName);
            Inputs.Add(inputStateType);
            Inputs.Add(inputWO);
            Inputs.Add(inputSKU);
            Inputs.Add(inputCountFlag);
        }
        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            base.Run();
            OleExec SFCDB = null;
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                string tempSql = "";
                string station = inputStationName.Value.ToString();
                string state = inputStateType.Value.ToString();
                string wo = inputWO.Value.ToString();
                string skuno = inputSKU.Value.ToString();
                string countFlag = inputCountFlag.Value.ToString();
                string runSql = $@"select b.workorderno,b.skuno, a.skuno  Model,a.sn,a.state,a.station,a.cell,a.operator,a.error_code,a.createtime 
                                    from r_test_detail_vertiv a ,r_sn b,r_test_record c where 1=1 and a.r_test_record_id=c.id  and c.r_sn_id = b.id(+) ";

                string defectiveSql = "";
                string retestSql = "";
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

                    defectiveSql = $@" and not exists
                                           (select distinct sn
                                              from  r_test_detail_vertiv t
                                                     where t.sn=a.sn              
                                                       and t.createtime between
                                                           to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss') and
                                                           to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss')
                                                       and t.station = a.station and t.state = 'PASS')";
                    retestSql = $@"and exists  (select sn, count(*)
                                          from r_test_detail_vertiv t
                                                 where t.sn = a.sn
                                                   and t.createtime between
                                                       to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss') and
                                                       to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss')
                                                   and t.station = a.station group by sn having count(*) > 1)";
                }
                else
                {
                    defectiveSql = $@" and not exists
                                           (select distinct sn
                                              from  r_test_detail_vertiv t
                                                     where t.sn=a.sn  and t.station = a.station and t.state = 'PASS')";
                    retestSql = $@"and exists  (select sn, count(*)
                                          from r_test_detail_vertiv t
                                                 where t.sn = a.sn  and t.station = a.station group by sn having count(*) > 1)";
                }
                if (station != "ALL")
                {
                    runSql = runSql + $@" and a.station='{station}' ";
                }
                if (state != "ALL")
                {
                    runSql = runSql + $@" and a.state='{state}' ";
                }
                if (wo != "ALL")
                {
                    runSql = runSql + $@" and b.workorderno = '{wo}'";
                }
                if (skuno != "ALL")
                {
                    tempSql = runSql;
                    runSql = runSql + $@" and b.skuno = '{skuno}'";
                }
                else
                {
                    tempSql = runSql;
                }
                if (countFlag == "RETEST")
                {
                    runSql = runSql + $@" {retestSql}  order by b.workorderno,a.createtime asc";
                }
                else if (countFlag == "FIRST_PASS")
                {
                    runSql = $@"select  workorderno,skuno,Model,sn,state,station,cell,operator,error_code,createtime
                                from (select workorderno,skuno,Model,sn,state,station,cell,operator,error_code,createtime,
                                                        ROW_NUMBER() over(partition by sn order by createtime asc) as rownums
                                                          from ({runSql})) where rownums = 1 and state = 'PASS'  order by workorderno,createtime asc";
                }
                else if (countFlag == "DEFECTIVE")
                {
                    runSql = runSql + $@" {defectiveSql}  order by b.workorderno,a.createtime asc ";
                }
                else
                {
                    ReportAlart alart = new ReportAlart("Count flag error");
                    Outputs.Add(alart);
                    return;
                }
                RunSqls.Add(runSql);
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dtTestReport = SFCDB.RunSelect(runSql).Tables[0];
                DataTable countTable = new DataTable();
                DataTable countLinkTable = new DataTable();
                //if (station != "ALL")
                //{
                //    string countLinkUrl = $@"Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.TestReportCountDetail&RunFlag=1&Station={station}&StateType={state}&workorderno={wo}&skuno={skuno}&StartDate={startDate}&EndDate={endDate}&CountFlag=";
                //    MakeCountTable(runSql, SFCDB, countLinkUrl, out countTable, out countLinkTable);
                //}

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                DataTable linkTable = new DataTable();
                DataRow linkRow = null;
                linkTable.Columns.Add("workorderno");
                linkTable.Columns.Add("skuno");
                linkTable.Columns.Add("Model");
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("STATE");
                linkTable.Columns.Add("STATION");
                linkTable.Columns.Add("CELL");
                linkTable.Columns.Add("OPERATOR");
                linkTable.Columns.Add("ERROR_CODE");
                linkTable.Columns.Add("CREATETIME");
                for (int i = 0; i < dtTestReport.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + dtTestReport.Rows[i]["SN"].ToString();
                    linkRow["workorderno"] = "";
                    linkRow["skuno"] = "";
                    linkRow["Model"] = "";
                    linkRow["STATE"] = "";
                    linkRow["STATION"] = "";
                    linkRow["CELL"] = "";
                    linkRow["OPERATOR"] = "";
                    linkRow["ERROR_CODE"] = "";
                    linkRow["CREATETIME"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dtTestReport, linkTable);
                reportTable.Tittle = "SN TEST REPORT";
                Outputs.Add(reportTable);

                //if (station != "ALL")
                //{
                //    ReportTable reportCountTable = new ReportTable();
                //    reportCountTable.LoadData(countTable, countLinkTable);
                //    reportCountTable.Tittle = "SN TEST Count";
                //    Outputs.Add(reportCountTable);
                //}
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

        private void MakeCountTable(string resultSql, OleExec db, string linkUrl, out DataTable countTable, out DataTable countLinkTable)
        {
            DataTable _countTable = new DataTable();
            DataTable _countLinkTable = new DataTable();
            string retestCountSql = $@"select nvl(sum(decode(test_time,1,0,test_time-1)),0) from (select sn,count(*) as test_time from ({resultSql})group by sn )";
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
            double fpy = firstPassCount / testSNCount;
            double retestRate = retestCount / testSNCount;
            double yield = goodProductCount / testSNCount;
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
    }
}
