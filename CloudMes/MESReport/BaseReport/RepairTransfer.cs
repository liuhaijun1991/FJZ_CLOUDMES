using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public  class RepairTransfer:ReportBase
    {

        
        ReportInput testType = new ReportInput() { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL","STRUCTURAL", "FUNCTIONAL"} };
        ReportInput InputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Wo = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null};
        ReportInput InputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput InputStatus = new ReportInput() { Name = "Status", InputType = "Select", Value = "RepairWip", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "WaitingCheckIn", "RepairWip", "AlreadyCheckOut"} };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairTransfer()
        {
            Inputs.Add(testType);
            Inputs.Add(InputSN);
            Inputs.Add(Wo);
            Inputs.Add(InputSku);
            Inputs.Add(InputStatus);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);

        }
        public override void Init()
        {
            try
            {
                StartTime.Value = DateTime.Now.AddDays(-7);
                EndTime.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Run()
        {
            // base.Run();
            string type = testType.Value.ToString();
            string WoObj = Wo.Value.ToString();
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            string sn = InputSN.Value.ToString();
            string skuno = InputSku.Value.ToString();
            string status = InputStatus.Value.ToString();
            DataTable dt = null;
            ReportTable retTab = new ReportTable();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try {
                string strSql = "";
                if (status == "WaitingCheckIn")
                {
                    strSql = $@"SELECT A.SN, A.FAIL_STATION, A.SKUNO, R.WORKORDERNO, A.FAIL_TIME, A.FAIL_EMP, 'WaitingCheckIn' STATUS
                                   FROM R_REPAIR_MAIN A, SFCRUNTIME.R_SN R
                                  WHERE A.SN = R.SN 
                                  AND R.VALID_FLAG = '1'
                                    AND R.REPAIR_FAILED_FLAG = '1'
                                    AND A.ID NOT IN
                                        (SELECT REPAIR_MAIN_ID FROM R_REPAIR_TRANSFER)";
                }
                else
                {
                    strSql = $@"select a.sn,a.station_name,a.skuno,a.workorderno,in_time,in_send_emp,in_receive_emp,out_time,out_send_emp,out_receive_emp,decode(closed_flag,1,'AlreadyCheckOut',0,'RepairWip','ERROR') as status from r_repair_transfer a,r_sn b where a.sn=b.sn and b.valid_flag='1' ";
                }
                
                if (sn != "")
                {
                    strSql = strSql + $@" and a.sn = '{sn}'";
                }
                else
                {
                    if (WoObj != "")
                    {
                        strSql = strSql + $@" and a.workorderno = '{WoObj}'";
                    }
                    else
                    {
                        if (skuno != "")
                        {
                            strSql = strSql + $@" and a.skuno = '{skuno}'";
                        }
                    }
                }
                if (status == "AlreadyCheckOut")
                {
                    strSql = strSql + $@" and a.closed_flag = 1";
                }
                else if (status == "RepairWip")
                {
                    strSql = strSql + $@" and a.closed_flag = 0";
                }
                string strStation = "";
                string strTime = "";
                if (status == "WaitingCheckIn")
                {
                    strSql = strSql + $@" AND A.CREATE_TIME BETWEEN TO_DATE('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                AND TO_DATE('{evalue}','YYYY/MM/DD HH24:MI:SS') ";
                    strStation = "FAIL_STATION";
                    strTime = "a.CREATE_TIME";
                }
                else
                {
                    strSql = strSql + $@" AND a.IN_TIME BETWEEN TO_DATE('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                AND TO_DATE('{evalue}','YYYY/MM/DD HH24:MI:SS') ";
                    strStation = "STATION_NAME";
                    strTime = "a.IN_TIME";
                }

                if (type != "ALL")
                {
                    strSql = strSql + $@" AND {strStation} IN (SELECT MES_STATION FROM SFCBASE.C_TEMES_STATION_MAPPING WHERE TEGROUP = '{type}') 
                                        ORDER BY {strTime}";
                }
                else
                {
                    strSql = strSql + $@" ORDER BY {strTime}";
                }



                dt = SFCDB.RunSelect(strSql).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                retTab.LoadData(dt, null);
                retTab.Tittle = "RepairTransferWip";
                Outputs.Add(retTab);
            } catch (Exception ex) {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }
    }
}
