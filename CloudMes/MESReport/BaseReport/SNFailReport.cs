using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNFailReport : ReportBase
    {
        ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputRepairStatus = new ReportInput() { Name = "Status", InputType = "Select", Value = "All", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "All", "Complete", "Pending" } };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "2020-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = "2020-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SNFailReport()
        {
            Inputs.Add(inputSkuno);
            Inputs.Add(inputSN);
            Inputs.Add(inputStation);
            Inputs.Add(inputRepairStatus);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);

            string sqlGetStation = $@"SELECT DISTINCT C.NEXT_STATION AS STATION_NAME FROM SFCRUNTIME.R_SN_STATION_DETAIL C WHERE EXISTS (
                SELECT 1 FROM SFCBASE.C_TEMES_STATION_MAPPING WHERE C.NEXT_STATION = MES_STATION) ORDER BY STATION_NAME";
            Sqls.Add("GetStation", sqlGetStation);
        }

        public override void Init()
        {
            base.Init();
            inputDateFrom.Value = DateTime.Now.AddDays(-7);
            inputDateTo.Value = DateTime.Now;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtStation = SFCDB.RunSelect(Sqls["GetStation"]).Tables[0];

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            List<string> stationList = new List<string>();
            stationList.Add("ALL");
            foreach (DataRow row in dtStation.Rows)
            {
                stationList.Add(row["STATION_NAME"].ToString());
            }
            inputStation.ValueForUse = stationList;
        }
        public override void Run()
        {
            try
            {
                base.Run();
                string skuno = inputSkuno.Value == null ? inputSkuno.Value.ToString() : inputSkuno.Value.ToString().Trim().ToUpper();
                string SN = inputSN.Value.ToString().Trim().ToUpper();
                string station = inputStation.Value.ToString();
                string repairStatus = inputRepairStatus.Value.ToString();
                string dateFrom = Convert.ToDateTime(inputDateFrom.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");
                string dateTo = Convert.ToDateTime(inputDateTo.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss");

                string runSql = $@"SELECT RM.SKUNO, RM.SN, RM.WORKORDERNO, RM.FAIL_LINE, RM.FAIL_STATION, RF.FAIL_LOCATION, RM.FAIL_EMP, RM.FAIL_TIME, RF.FAIL_CODE, RF.DESCRIPTION, (CASE
                        WHEN RF.REPAIR_FLAG = 1 THEN 'Complete'
                        WHEN RF.REPAIR_FLAG = 0 THEN 'Pending'
                    END) AS REPAIR, (CASE
                        WHEN RF.REPAIR_FLAG = 1 THEN RM.EDIT_EMP
                        WHEN RF.REPAIR_FLAG = 0 THEN NULL
                    END) AS REPAIR_EMP, (CASE
                        WHEN RF.REPAIR_FLAG = 1 THEN RM.EDIT_TIME
                        WHEN RF.REPAIR_FLAG = 0 THEN NULL
                    END) AS REPAIR_TIME FROM R_REPAIR_MAIN RM INNER JOIN R_REPAIR_FAILCODE RF ON RM.ID = RF.REPAIR_MAIN_ID WHERE 1 = 1";

                if(String.IsNullOrEmpty(skuno) && !String.IsNullOrEmpty(SN))
                {
                    runSql += $@" AND RM.SN = '{SN}'";
                }
                else if (!String.IsNullOrEmpty(skuno) && String.IsNullOrEmpty(SN))
                {
                    runSql += $@" AND RM.SKUNO = '{skuno}'";
                }
                else if (!String.IsNullOrEmpty(skuno) && !String.IsNullOrEmpty(SN))
                {
                    runSql += $@" AND RM.SN = '{SN}' AND RM.SKUNO = '{skuno}'";
                }

                switch (repairStatus)
                {
                    case "All":
                        break;
                    case "Complete":
                        runSql += $@" AND REPAIR_FLAG = 1 ";
                        break;
                    case "Pending":
                        runSql += $@" AND REPAIR_FLAG = 0 ";
                        break;
                    default:
                        throw new Exception("Repair Status Error!");
                }

                if (station != "ALL")
                {
                    runSql += $@" AND RM.FAIL_STATION = '{station}'";
                }

                runSql += $@" AND RF.FAIL_TIME BETWEEN TO_DATE('{dateFrom}', 'YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{dateTo}', 'YYYY/MM/DD HH24:MI:SS') ORDER BY RM.FAIL_TIME DESC";

                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                try
                {
                    RunSqls.Add(runSql);
                    DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("No Data!");
                    }
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(dt, null);
                    retTab.Tittle = $@"{skuno} {station} SN Fail Report";
                    Outputs.Add(retTab);
                }
                catch (Exception exception)
                {
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    Outputs.Add(new ReportAlart(exception.Message));
                }
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}