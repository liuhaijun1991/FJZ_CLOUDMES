using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class JuniperTestReport : ReportBase
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
        public JuniperTestReport()
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

            DataTable resdt = new DataTable();
            resdt.Columns.Add("SKUNO");
            resdt.Columns.Add("WORKORDERNO");
            resdt.Columns.Add("SN");
            resdt.Columns.Add("SERIAL_NUMBER");
            resdt.Columns.Add("UNIQUE_TEST_ID");
            resdt.Columns.Add("TESTATION");
            resdt.Columns.Add("MESSTATION");
            resdt.Columns.Add("TEST_RESULT");
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
            //resdt.Columns.Add("TEST_RESULT");TE 曾文淵要求挪到MESSTATION後面
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
                string sn = inputSN.Value.ToString();
                string station = inputStationName.Value.ToString();
                string state = inputStateType.Value.ToString();
                string wo = inputWO.Value.ToString();
                string skuno = inputSKU.Value.ToString();

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
                        else if (c.ToString() == "UNIQUE_TEST_ID")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.JuniperTestByTestIDReport&RunFlag=1&TESTID=" + showTable.Rows[i]["UNIQUE_TEST_ID"].ToString();
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
                reportTable.FixedHeader = true;
                reportTable.FixedCol = 5;
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

            string runSql = $@"SELECT SN.SKUNO,
                                   SN.WORKORDERNO,
                                   SN.SN,
                                   TRECORD.TESTATION,
                                   TRECORD.MESSTATION,
                                   TRECORD.DEVICE,
                                   TRECORD.EDIT_TIME,
                                   TJNP.TESTDATE,
                                   TJNP.STATUS,
                                   TJNP.LASTEDITDT,
                                   TJNP.LASTEDITBY,
                                   TJNP.TESTERNO,
                                   TJNP.TATIME,
                                   TJNP.PART_NUMBER,
                                   TJNP.CM_ODM_PARTNUMBER,
                                   TJNP.SERIAL_NUMBER,
                                   TJNP.PHASE,
                                   TJNP.PART_NUMBER_REVISION,
                                   TJNP.UNIQUE_TEST_ID,
                                   TJNP.TEST_START_TIMESTAMP,
                                   TJNP.TEST_STEP,
                                   TJNP.TEST_CYCLE_TEST_LOOP,
                                   TJNP.CAPTURE_TIME,
                                   TJNP.TEST_RESULT,
                                   TJNP.FAILCODE,
                                   TJNP.TEST_STATION_NUMBER,
                                   TJNP.LOAD_DATE,
                                   TJNP.FILE_NAME
                              FROM R_SN SN, R_TEST_RECORD TRECORD, R_TEST_JUNIPER TJNP
                             WHERE SN.ID = TRECORD.R_SN_ID
                               AND SN.SN = TRECORD.SN
                               AND TRECORD.ID = TJNP.R_TEST_RECORD_ID";

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
                runSql = runSql + $@" and trecord.messtation='{station}' ";
            }
            if (state != "ALL" && !string.IsNullOrEmpty(state))
            {
                runSql = runSql + $@" and tjnp.status='{state}' ";
            }
            if (wo != "ALL" && !string.IsNullOrEmpty(wo))
            {
                runSql = runSql + $@" and sn.workorderno = '{wo}'";
            }
            if (skuno != "ALL" && !string.IsNullOrEmpty(skuno))
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
