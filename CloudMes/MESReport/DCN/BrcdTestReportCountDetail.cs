using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class BrcdTestReportCountDetail : ReportBase
    {
        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStationName = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStateType = new ReportInput() { Name = "StateType", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "workorderno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSKU = new ReportInput() { Name = "skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCountFlag = new ReportInput() { Name = "CountFlag", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };

        public BrcdTestReportCountDetail()
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
                string runSql = $@"select SN.SKUNO,SN.WORKORDERNO,SN.SN,TRECORD.TESTATION,TRECORD.MESSTATION,TRECORD.DEVICE,/*TRECORD.STARTTIME as TEST_STARTTIME,TRECORD.ENDTIME as TEST_ENDTIME,*/TRECORD.EDIT_TIME,
                                tbrcd.TESTDATE,tbrcd.STATUS,tbrcd.PARTNO,tbrcd.SYMPTOM,tbrcd.PCBASN,tbrcd.PCBAPN,tbrcd.VPN,tbrcd.LASTEDITDT,tbrcd.LASTEDITBY,
                                tbrcd.FAILURECODE,tbrcd.TRAY_SN,tbrcd.TESTERNO,tbrcd.TEMP4,tbrcd.TEMP5,tbrcd.TATIME from r_sn sn,r_test_record trecord,r_test_brcd tbrcd
                                where sn.id=trecord.r_sn_id and trecord.id=tbrcd.r_test_record_id";

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
                    runSql = runSql + $@" and trecord.EDIT_TIME   between to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') 
                                and  to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') ";

                    defectiveSql = $@" and not exists
                                           (select distinct sn
                                              from  r_test_record t
                                                     where t.sn=sn.sn              
                                                       and t.EDIT_TIME between
                                                           to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss') and
                                                           to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss')
                                                       and t.messtation = trecord.messtation and t.state = 'PASS')";
                    retestSql = $@"and exists  (select sn, count(*)
                                          from r_test_record t
                                                 where t.sn = sn.sn
                                                   and t.EDIT_TIME between
                                                       to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss') and
                                                       to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}', 'yyyy/mm/dd hh24:mi:ss')
                                                   and t.messtation = trecord.messtation group by sn having count(*) > 1)";
                }
                else
                {
                    defectiveSql = $@" and not exists
                                           (select distinct sn
                                              from  r_test_record t
                                                     where t.sn=sn.sn  and t.messtation = trecord.messtation and t.state = 'PASS')";
                    retestSql = $@"and exists  (select sn, count(*)
                                          from r_test_record t
                                                 where t.sn = sn.sn  and  t.messtation = trecord.messtation  group by sn having count(*) > 1)";
                }
                if (station != "ALL" && !string.IsNullOrEmpty(station))
                {
                    runSql = runSql + $@" and trecord.messtation='{station}' ";
                }
                if (state != "ALL" && !string.IsNullOrEmpty(state))
                {
                    runSql = runSql + $@" and tbrcd.STATUS='{state}' ";
                }
                if (wo != "ALL" && !string.IsNullOrEmpty(wo))
                {
                    runSql = runSql + $@" and sn.workorderno = '{wo}'";
                }
                if (skuno != "ALL" && !string.IsNullOrEmpty(skuno))
                {
                    tempSql = runSql;
                    runSql = runSql + $@" and sn.skuno = '{skuno}'";
                }
                else
                {
                    tempSql = runSql;
                }
                if (countFlag == "RETEST")
                {
                    runSql = runSql + $@" {retestSql}  order by sn.workorderno,trecord.EDIT_TIME asc";
                }
                else if (countFlag == "FIRST_PASS")
                {
                    runSql = $@"select SKUNO,WORKORDERNO,SN,TESTATION,MESSTATION,DEVICE,/*TEST_STARTTIME,TEST_ENDTIME,*/EDIT_TIME,
                                TESTDATE,STATUS,PARTNO,SYMPTOM,PCBASN,PCBAPN,VPN,LASTEDITDT,LASTEDITBY,FAILURECODE,TRAY_SN,TESTERNO,TEMP4,TEMP5,TATIME
                                from (select SKUNO,WORKORDERNO,SN,TESTATION,MESSTATION,DEVICE,/*TEST_STARTTIME,TEST_ENDTIME,*/EDIT_TIME,
                                TESTDATE,STATUS,PARTNO,SYMPTOM,PCBASN,PCBAPN,VPN,LASTEDITDT,LASTEDITBY,FAILURECODE,TRAY_SN,TESTERNO,TEMP4,TEMP5,TATIME,
                                                        ROW_NUMBER() over(partition by sn order by EDIT_TIME asc) as rownums
                                                          from ({runSql})) where rownums = 1 and STATUS = 'PASS'  order by workorderno,EDIT_TIME asc";
                }
                else if (countFlag == "DEFECTIVE")
                {
                    runSql = runSql + $@" {defectiveSql}  order by sn.workorderno,trecord.EDIT_TIME asc ";
                }
                else
                {
                    ReportAlart alart = new ReportAlart("Count flag error");
                    Outputs.Add(alart);
                    return;
                }

                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dtTestReport = SFCDB.RunSelect(runSql).Tables[0];

                if (dtTestReport.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }

                DataTable linkTable = new DataTable();
                DataRow linkRow = null;
                
                foreach (var c in dtTestReport.Columns)
                {
                    linkTable.Columns.Add(c.ToString());
                }
                for (int i = 0; i < dtTestReport.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    foreach (var c in linkTable.Columns)
                    {

                        if (c.ToString() == "SN")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + dtTestReport.Rows[i]["SN"].ToString();
                        }
                        else
                        {
                            linkRow[c.ToString()] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dtTestReport, linkTable);
                reportTable.Tittle = "SN TEST REPORT";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {               
                ReportAlart alart = new ReportAlart(exception.Message);
                Outputs.Add(alart);
            }
            finally {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

    }
}
