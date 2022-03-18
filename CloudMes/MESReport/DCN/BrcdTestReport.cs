using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class BrcdTestReport : ReportBase
    {
        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStationName = new ReportInput() { Name = "MesStation", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStateType = new ReportInput() { Name = "StateType", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "PASS", "FAIL" } };
        ReportInput inputWO = new ReportInput() { Name = "workorderno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSKU = new ReportInput() { Name = "skuno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        string sqlGetSation = "";
        ReportTable reportTable = null;
        public BrcdTestReport()
        {
            Inputs.Add(inputStartDate);
            Inputs.Add(inputEndDate);
            Inputs.Add(inputSN);
            Inputs.Add(inputStationName);
            Inputs.Add(inputStateType);
            Inputs.Add(inputWO);
            Inputs.Add(inputSKU);

            sqlGetSation = "select distinct mes_station from c_temes_station_mapping order by mes_station";
            Sqls.Add("GetSation", sqlGetSation);
        }
        public override void Init()
        {
            //base.Init();
            inputStartDate.Value = DateTime.Now.AddDays(-1);
            inputEndDate.Value = DateTime.Now;
            inputStationName.ValueForUse = GetStation();
            reportTable = new ReportTable();
            reportTable.ColNames.Add("SKUNO");
            reportTable.ColNames.Add("WORKORDERNO");
            reportTable.ColNames.Add("SN");
            reportTable.ColNames.Add("TESTATION");
            reportTable.ColNames.Add("MESSTATION");
            reportTable.ColNames.Add("DEVICE");
            reportTable.ColNames.Add("STATUS");
            //reportTable.ColNames.Add("TEST_STARTTIME");
            //reportTable.ColNames.Add("TEST_ENDTIME");
            reportTable.ColNames.Add("EDIT_TIME");            
            reportTable.ColNames.Add("TESTDATE");
            reportTable.ColNames.Add("TATIME");
            reportTable.ColNames.Add("PARTNO");
            reportTable.ColNames.Add("SYMPTOM");
            reportTable.ColNames.Add("PCBASN");
            reportTable.ColNames.Add("PCBAPN");
            reportTable.ColNames.Add("VPN");
            reportTable.ColNames.Add("LASTEDITDT");
            reportTable.ColNames.Add("LASTEDITBY");
            reportTable.ColNames.Add("FAILURECODE");
            reportTable.ColNames.Add("TRAY_SN");
            reportTable.ColNames.Add("TESTERNO");
            reportTable.ColNames.Add("TEMP4");
            reportTable.ColNames.Add("TEMP5");           
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
                    sql = $@"select  SKUNO,WORKORDERNO,SN,TESTATION,MESSTATION,DEVICE,/*TEST_STARTTIME,TEST_ENDTIME,*/EDIT_TIME,
                                TESTDATE,STATUS,PARTNO,SYMPTOM,PCBASN,PCBAPN,VPN,LASTEDITDT,LASTEDITBY,FAILURECODE,TRAY_SN,TESTERNO,TEMP4,TEMP5,TATIME
                                    from (select temp.*,rownum as rn from ({Sqls["RunSql"]} ) temp )
                                    where rn> ({PageNumber} - 1) * {PageSize} and rn<= {PageNumber} * {PageSize}";          
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
                    string countLinkUrl = $@"Link#/FunctionPage/Report/Report.html?ClassName=MESReport.DCN.BrcdTestReportCountDetail&RunFlag=1&Station={station}&StateType={state}&workorderno={wo}&skuno={skuno}&StartDate={startDate}&EndDate={endDate}&CountFlag=";
                    MakeCountTable(Sqls["RunSql"], SFCDB, countLinkUrl, out countTable, out countLinkTable);
                    //MakeCountTableByDataTable(SFCDB.RunSelect(Sqls["RunSql"]).Tables[0], SFCDB, countLinkUrl, out countTable, out countLinkTable);
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                
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
                string fileName = "BrcdTestReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
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
        private void GetRunSql(string sn, string station, string state, string wo, string skuno)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            
            string runSql = $@"select SN.SKUNO,SN.WORKORDERNO,SN.SN,TRECORD.TESTATION,TRECORD.MESSTATION,TRECORD.DEVICE,/*TRECORD.STARTTIME as TEST_STARTTIME,TRECORD.ENDTIME as TEST_ENDTIME,*/TRECORD.EDIT_TIME,
                                tbrcd.TESTDATE,tbrcd.STATUS,tbrcd.PARTNO,tbrcd.SYMPTOM,tbrcd.PCBASN,tbrcd.PCBAPN,tbrcd.VPN,tbrcd.LASTEDITDT,tbrcd.LASTEDITBY,
                                tbrcd.FAILURECODE,tbrcd.TRAY_SN,tbrcd.TESTERNO,tbrcd.TEMP4,tbrcd.TEMP5,tbrcd.TATIME from r_sn sn,r_test_record trecord,r_test_brcd tbrcd
                                where sn.id=trecord.r_sn_id and trecord.id=tbrcd.r_test_record_id";
           
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
                runSql = runSql + $@" and trecord.edit_time  between to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') 
                                and  to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') ";               
            }

            if (sn != "")
            {
                runSql = runSql + $@" and sn.sn='{sn}' ";              
            }
            if (station != "ALL" && !string.IsNullOrEmpty(station))
            {
                runSql = runSql + $@" and TRECORD.messtation='{station}' ";               
            }
            if (state != "ALL" && !string.IsNullOrEmpty(state))
            {
                runSql = runSql + $@" and tbrcd.status='{state}' ";              
            }
            if (wo != "ALL" && !string.IsNullOrEmpty(wo))
            {
                runSql = runSql + $@" and sn.workorderno = '{wo}'";
            }
            if (skuno != "ALL" &&!string.IsNullOrEmpty(skuno))
            {
                runSql = runSql + $@" and sn.skuno = '{skuno}'";
            }            
            runSql += " order by sn.workorderno,trecord.EDIT_TIME asc";            
            RunSqls.Add(runSql);
            Sqls.Add("RunSql", runSql);           
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
            string firstPassCountSql = $@"select count(*) from (select workorderno,skuno,sn,STATUS,MESSTATION,edit_time,
                                                        ROW_NUMBER() over(partition by sn order by edit_time asc) as rownums
                                                          from ({resultSql})) where rownums = 1 and STATUS = 'PASS'";
            string goodProductCountSql = $@"select count(distinct sn) from ({resultSql}) mm where mm.STATUS = 'PASS' ";

            string retestSNCountSql = $@"SELECT COUNT(*) from ( select count(distinct sn) from ({resultSql}) aa where 1=1 group by aa.sn having count(aa.MESSTATION)>1)";

            double retestCount = Convert.ToDouble(db.RunSelect(retestCountSql).Tables[0].Rows[0][0].ToString());
            double retestSNCount = Convert.ToDouble(db.RunSelect(retestSNCountSql).Tables[0].Rows[0][0].ToString());
            double testCount = Convert.ToDouble(db.RunSelect(testCountSql).Tables[0].Rows[0][0].ToString());
            double firstPassCount = Convert.ToDouble(db.RunSelect(firstPassCountSql).Tables[0].Rows[0][0].ToString());
            double goodProductCount = Convert.ToDouble(db.RunSelect(goodProductCountSql).Tables[0].Rows[0][0].ToString());
            double testSNCount = testCount - retestCount;
            double fpy = testSNCount == 0 ? 0 : firstPassCount / testSNCount;
            double retestRate = testSNCount == 0 ? 0 : retestCount / testSNCount;
            double yield = testSNCount == 0 ? 0 : goodProductCount / testSNCount;
            _countTable.Columns.Add("Retest times");//("重測次數");
            _countTable.Columns.Add("Retest SN counts"); //("重測SN個數");
            _countTable.Columns.Add("Total tests"); //("總測試次數");
            _countTable.Columns.Add("Total tested SN"); //("測試SN總數");
            _countTable.Columns.Add("First pass counts"); //("首次PASS數");
            _countTable.Columns.Add("Direct rate（FPY）"); //("直通率（FPY）");
            _countTable.Columns.Add("Retest rate"); //("重測率");
            _countTable.Columns.Add("Good product"); //("良品");
            _countTable.Columns.Add("Yield rate"); //("良品率");
            _countTable.Columns.Add("rejects"); //("不良品");
            _countTable.Columns.Add("Defective rate"); //("不良品率");
            DataRow countRow = _countTable.NewRow();
            //countRow["重測次數"] = retestCount;
            //countRow["重測SN個數"] = retestSNCount;
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
            countRow["Retest times"] = retestCount;
            countRow["Retest SN counts"] = retestSNCount;
            countRow["Total tests"] = testCount;
            countRow["Total tested SN"] = testSNCount;
            //抓來彈jj
           // countRow["First pass number"] = firstPassCount;
            countRow["First pass counts"] = firstPassCount;
            countRow["Direct rate（FPY）"] = Math.Round(fpy * 100, 2) + "%";
            countRow["Retest rate"] = Math.Round(retestRate * 100, 2) + "%";
            countRow["Good product"] = goodProductCount;
            countRow["Yield rate"] = Math.Round(yield * 100, 2) + "%";
            countRow["rejects"] = testSNCount - goodProductCount;
            countRow["Defective rate"] = (goodProductCount == 0 ? 0 : Math.Round((testSNCount - goodProductCount) / testSNCount * 100, 2)) + "%";
            _countTable.Rows.Add(countRow);

            _countLinkTable.Columns.Add("Retest times");//("重測次數");
            _countLinkTable.Columns.Add("Retest SN counts"); //("重測SN個數");
            _countLinkTable.Columns.Add("Total tests"); //("總測試次數");
            _countLinkTable.Columns.Add("Total tested SN"); //("測試SN總數");
            _countLinkTable.Columns.Add("First pass counts"); //("首次PASS數");
            _countLinkTable.Columns.Add("Direct rate（FPY）"); //("直通率（FPY）");
            _countLinkTable.Columns.Add("Retest rate"); //("重測率");
            _countLinkTable.Columns.Add("Good product"); //("良品");
            _countLinkTable.Columns.Add("Yield rate"); //("良品率");
            _countLinkTable.Columns.Add("rejects"); //("不良品");
            _countLinkTable.Columns.Add("Defective rate"); //("不良品率");

            DataRow countLinkRow = _countLinkTable.NewRow();
            //countLinkRow["重測次數"] = retestCount == 0 ? "" : linkUrl + "RETEST";
            ////countLinkRow["重測SN個數"] = retestCount == 0 ? "" : linkUrl + "RETEST";
            //countLinkRow["總測試次數"] = "";
            //countLinkRow["測試SN總數"] = "";
            //countLinkRow["首次PASS數"] = firstPassCount == 0 ? "" : linkUrl + "FIRST_PASS";
            //countLinkRow["直通率（FPY）"] = "";
            //countLinkRow["重測率"] = "";
            //countLinkRow["良品"] = "";
            //countLinkRow["良品率"] = "";
            //countLinkRow["不良品"] = (testSNCount - goodProductCount) == 0 ? "" : linkUrl + "DEFECTIVE";
            //countLinkRow["不良品率"] = "";

            countLinkRow["Retest times"] = retestCount == 0 ? "" : linkUrl + "RETEST";
            //countLinkRow["重測SN個數"] = retestCount == 0 ? "" : linkUrl + "RETEST";
            countLinkRow["Total tests"] = "";
            countLinkRow["Total tested SN"] = "";
            countLinkRow["First pass counts"] = firstPassCount == 0 ? "" : linkUrl + "FIRST_PASS";
            countLinkRow["Direct rate（FPY）"] = "";
            countLinkRow["Retest rate"] = "";
            countLinkRow["Good product"] = "";
            countLinkRow["Yield rate"] = "";
            countLinkRow["rejects"] = (testSNCount - goodProductCount) == 0 ? "" : linkUrl + "DEFECTIVE";
            countLinkRow["Defective rate"] = "";
            _countLinkTable.Rows.Add(countLinkRow);

            countTable = _countTable;
            countLinkTable = _countLinkTable;
        }

    }
}
