using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    class MajorRepairReport : ReportBase
    {
        ReportInput inputPSN = new ReportInput() { Name = "ParentSN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputRSN = new ReportInput() { Name = "RepairSN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput repairStatus = new ReportInput() { Name = "Status", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "WO Assigned", "Waiting WO" } };
        
        public MajorRepairReport()
        {
            Inputs.Add(inputPSN);
            Inputs.Add(inputRSN);
            Inputs.Add(repairStatus);

        }

        public override void Init()
        {
            base.Init();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
           

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            List<string> stationList = new List<string>();
            stationList.Add("ALL");
        }
        public override void Run()
        {
            try
            {
                base.Run();
                string repSN = inputRSN.Value.ToString().Trim().ToUpper();
                string parSN = inputPSN.Value.ToString().Trim().ToUpper();
                string status = repairStatus.Value.ToString();
                string runSql = "";

                if (status == "ALL")
                {
                    runSql = $@"SELECT 
                                RR.PARENT_SN, 
                                RR.REPAIRED_SN,
                                CASE RR.WO_FLAG 
                                 WHEN '0' THEN 'NOT ASSIGNED YET'
                                 ELSE (SELECT WORKORDERNO FROM SFCRUNTIME.R_WO_BASE WHERE ID = RR.WORKORDERNO_ID)
                                END NEW_REPAIRED_SN_WO,
                                RA.ACTION_CODE, 
                                RA.FAIL_LOCATION, 
                                RA.REASON_CODE ROOT_CAUSE, 
                                RA.DESCRIPTION, 
                                RA.REPAIR_EMP, 
                                RA.REPAIR_TIME 
                        FROM SFCRUNTIME.R_REPAIR_PCBA_RELATIONSHIP RR
                        JOIN SFCRUNTIME.R_REPAIR_ACTION RA
                        ON RR.REPAIR_ACTION_ID = RA.ID";
                }
                if (status == "WO Assigned")
                {
                    runSql = $@"SELECT RR.PARENT_SN, 
                                       RR.REPAIRED_SN, 
                                       RW.WORKORDERNO NEW_REPAIRED_SN_WO, 
                                       RA.ACTION_CODE, 
                                       RA.FAIL_LOCATION, 
                                       RA.REASON_CODE ROOT_CAUSE, 
                                       RA.DESCRIPTION, 
                                       RA.REPAIR_EMP, 
                                       RA.REPAIR_TIME 
                                FROM SFCRUNTIME.R_REPAIR_PCBA_RELATIONSHIP RR
                                JOIN SFCRUNTIME.R_REPAIR_ACTION RA
                                ON RR.REPAIR_ACTION_ID = RA.ID
                                JOIN SFCRUNTIME.R_WO_BASE RW
                                ON RR.WORKORDERNO_ID = RW.ID";
                }
                if (status == "Waiting WO")
                {
                    runSql += $@" SELECT RR.ID,
                                   RR.PARENT_SN, 
                                   RR.REPAIRED_SN,
                                   'WAITING REPAIR CHECKOUT' NEW_REPAIRED_SN_WO,
                                   RA.ACTION_CODE, 
                                   RA.FAIL_LOCATION, 
                                   RA.REASON_CODE ROOT_CAUSE, 
                                   RA.DESCRIPTION, 
                                   RA.REPAIR_EMP, 
                                   RA.REPAIR_TIME 
                            FROM SFCRUNTIME.R_REPAIR_PCBA_RELATIONSHIP RR
                            JOIN SFCRUNTIME.R_REPAIR_ACTION RA
                            ON RR.REPAIR_ACTION_ID = RA.ID
                            AND RR.WO_FLAG = '0'";
                }



                if (!String.IsNullOrEmpty(repSN))
                {
                    runSql += $@" AND RR.REPAIRED_SN = '{repSN}'";
                };

                if (!String.IsNullOrEmpty(parSN))
                {
                    runSql += $@" AND RR.PARENT_SN = '{parSN}'";
                };


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
