using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class TestReportFromLlt : ReportBase
    {

        ReportInput inputSKU = new ReportInput() { Name = "skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "sn", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportTable reportTable = null;
        public TestReportFromLlt()
        {
            Inputs.Add(inputSKU);
            Inputs.Add(inputSN);
        }
        public override void Init()
        {
            reportTable = new ReportTable();
            reportTable.ColNames.Add("WORKORDERNO");
            reportTable.ColNames.Add("SKUNO");
            reportTable.ColNames.Add("SN");
            reportTable.ColNames.Add("STATUS");
            reportTable.ColNames.Add("STATION");
            reportTable.ColNames.Add("BURNIN_TIME");
            reportTable.ColNames.Add("TOTALTIME");
            reportTable.ColNames.Add("CREATEBY");
            reportTable.ColNames.Add("CREATETIME");
            Outputs.Add(new ReportColumns(reportTable.ColNames));
            PaginationServer = false;
        }

        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string skuno = inputSKU.Value.ToString();
                string _SN = inputSN.Value.ToString();
                DataTable showTable = new DataTable();

                GetRunSql(skuno, _SN);
                showTable = SFCDB.RunSelect(Sqls["RunSql"]).Tables[0];
                if (showTable.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }

                //DataTable linkTable = new DataTable();
                //foreach (var c in showTable.Columns)
                //{
                //    linkTable.Columns.Add(c.ToString());
                //}
                reportTable.LoadData(showTable, null);
                reportTable.Tittle = "LLTSN TEST REPORT";
                Outputs.Add(reportTable);
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
            string skuno = inputSKU.Value.ToString();
            string _SN = inputSN.Value.ToString();
            GetRunSql(skuno, _SN);
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

        private void GetRunSql(string skuno, string sn)
        {

            string runSql = $@"select bb.*,aa.TotalTIME from (
                            select k.workorderno,k.skuno,w.SN,w.STATUS,w.MESSTATION as STATION,CREATEBY,CREATETIME,BURNIN_TIME from r_llt_test w,r_sn k
                            where w.sn=k.sn and k.valid_flag=1 ";
            string tempSql = $@" )bb left join (" +
                            "select SN,SUM(BURNIN_TIME) TotalTIME " +
                            " from R_LLT_TEST group by SN)aa on bb.sn=aa.sn order by aa.sn";
            if (skuno != "")
            {
                runSql = runSql + $@" and k.skuno='" + skuno + "' ";
            }
            if (sn != "")
            {
                runSql = runSql + $@" and k.sn='" + sn + "' ";
            }
            if (skuno != "" && sn != "")
            {
                runSql = runSql + $@" and k.skuno='" + skuno + "' and k.sn= '" + sn + "' ";
            }
            runSql = runSql + tempSql;


            RunSqls.Add(runSql);
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
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }

    public class LLTestObject
    {
        public string Workorderno { get; set; }
        public string Skuno { get; set; }
        public string Model { get; set; }
        public string SN { get; set; }
        public string Status { get; set; }
        public string Station { get; set; }
        public string BURNIN_TIME { get; set; }
        public string TotalTIME { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
