using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class JuniperTestByTestIDReport : ReportBase
    {
        ReportInput inputTestID = new ReportInput() { Name = "TESTID", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        string sqlGetSation = "";
        ReportTable reportTable = null;
        public JuniperTestByTestIDReport()
        {
            Inputs.Add(inputTestID);
        }

        public override void Init()
        {
            //base.Init();
            reportTable = new ReportTable();

            DataTable resdt = new DataTable();
            resdt.Columns.Add("SN");
            resdt.Columns.Add("SERIAL_NUMBER");
            resdt.Columns.Add("UNIQUE_TEST_ID");
            resdt.Columns.Add("TESTATION");
            resdt.Columns.Add("MESSTATION");
            resdt.Columns.Add("DEVICE");
            resdt.Columns.Add("EDIT_TIME");
            resdt.Columns.Add("TESTDATE");
            resdt.Columns.Add("LASTEDITDT");
            resdt.Columns.Add("LASTEDITBY");
            resdt.Columns.Add("TESTERNO");
            resdt.Columns.Add("TATIME");
            resdt.Columns.Add("PART_NUMBER");
            resdt.Columns.Add("CM_ODM_PARTNUMBER");
            resdt.Columns.Add("PHASE");
            resdt.Columns.Add("PART_NUMBER_REVISION");
            resdt.Columns.Add("TEST_START_TIMESTAMP");
            resdt.Columns.Add("TEST_STEP");
            resdt.Columns.Add("TEST_CYCLE_TEST_LOOP");
            resdt.Columns.Add("CAPTURE_TIME");
            resdt.Columns.Add("TEST_RESULT");
            resdt.Columns.Add("FAILCODE");
            resdt.Columns.Add("TEST_STATION_NUMBER");
            resdt.Columns.Add("LOAD_DATE");
            resdt.Columns.Add("FILE_NAME");

            reportTable.LoadData(resdt);
        }

        public override void Run()
        {
            //base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string testid = inputTestID.Value.ToString();
                DataTable showTable = new DataTable();
                GetRunSql(testid);

                showTable = SFCDB.RunSelect(Sqls["RunSql"]).Tables[0];
                if (showTable.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
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
                        else if (c.ToString() == "SERIAL_NUMBER")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + showTable.Rows[i]["SERIAL_NUMBER"].ToString();
                        }
                        else
                        {
                            linkRow[c.ToString()] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }
                reportTable.LoadData(showTable, linkTable);
                reportTable.Tittle = "Juniper Test Record";
                reportTable.FixedHeader = true;
                reportTable.FixedCol = 3;
                reportTable.pagination = false;
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                ReportAlart alart = new ReportAlart(exception.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void DownFile()
        {
            string testid = inputTestID.Value.ToString();
            GetRunSql(testid);
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

        private void GetRunSql(string testid)
        {
            string runSql = $@"SELECT JRC.SYSSERIALNO SN,
                                   JRC.SERIAL_NUMBER,
                                   RCS.TESTATION,
                                   RCS.MESSTATION,
                                   RCS.DEVICE,
                                   RCS.EDIT_TIME,
                                   JRC.TESTDATE,
                                   JRC.STATUS,
                                   JRC.LASTEDITDT,
                                   JRC.LASTEDITBY,
                                   JRC.TESTERNO,
                                   JRC.TATIME,
                                   JRC.PART_NUMBER,
                                   JRC.CM_ODM_PARTNUMBER,
                                   JRC.SERIAL_NUMBER,
                                   JRC.PHASE,
                                   JRC.PART_NUMBER_REVISION,
                                   JRC.UNIQUE_TEST_ID,
                                   JRC.TEST_START_TIMESTAMP,
                                   JRC.TEST_STEP,
                                   JRC.TEST_CYCLE_TEST_LOOP,
                                   JRC.CAPTURE_TIME,
                                   JRC.TEST_RESULT,
                                   JRC.FAILCODE,
                                   JRC.TEST_STATION_NUMBER,
                                   JRC.LOAD_DATE,
                                   JRC.FILE_NAME
                              FROM R_TEST_JUNIPER JRC, R_TEST_RECORD RCS
                             WHERE JRC.R_TEST_RECORD_ID = RCS.ID
                               AND JRC.UNIQUE_TEST_ID = '{testid}'
                             ORDER BY JRC.TATIME";
            RunSqls.Add(runSql);
            Sqls.Add("RunSql", runSql);
        }

    }
}
