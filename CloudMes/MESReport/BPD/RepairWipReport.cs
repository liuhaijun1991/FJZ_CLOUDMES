using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Data;
using MESDBHelper;
using System.Data.OleDb;

namespace MESReport.BPD
{
    class RepairWipReport : ReportBase
    {
        ReportInput StartTime = new ReportInput { Name = "Start_time", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput { Name = "End_time", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Flag = new ReportInput { Name = "Flag", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairWipReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            Inputs.Add(Flag);
        }

        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dt = new DataTable();
            string sql = string.Empty;
            string Stime = StartTime.Value.ToString();
            string ETime = EndTime.Value.ToString();
            OleDbParameter[] paramet = new OleDbParameter[] { };
            try
            {
                if (Flag.Value.ToString().ToUpper().Equals("ONCHECKIN"))
                {
                    sql = @"select a.SN,a.workorderno,a.skuno,b.fail_code,a.fail_line,a.fail_station, a.fail_emp, a.create_time, a.closed_flag, b.repair_flag  from r_repair_main a, r_repair_failcode b where a.id not in (select repair_main_id from r_repair_transfer)  and a.id = b.repair_main_id and a.create_time between to_date('"+ Stime + "', 'yyyy-mm-dd hh24:mi:ss') and to_date('" + ETime + "', 'yyyy-mm-dd hh24:mi:ss')";                   
                }
                if (Flag.Value.ToString().ToUpper().Equals("CHECKIN"))
                {
                    sql = @"select a.SN,a.workorderno, a.skuno, b.fail_code, a.fail_station, c.in_time,c.in_send_emp, c.in_receive_emp,a.fail_line, a.fail_emp, a.create_time, a.closed_flag from r_repair_main a, r_repair_failcode b, r_repair_transfer c where a.id = b.repair_main_id  and a.id = c.repair_main_id and a.closed_flag = '0' and a.create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";
                }
                if (Flag.Value.ToString().ToUpper().Equals("REPAIRED"))
                {
                    sql = @"select a.SN,a.workorderno, a.skuno, b.fail_code, a.fail_station, c.in_time,c.in_send_emp, c.in_receive_emp,a.fail_line, a.fail_emp, a.create_time, a.closed_flag,b.repair_flag from r_repair_main a, r_repair_failcode b, r_repair_transfer c where a.id = b.repair_main_id  and a.id = c.repair_main_id and a.closed_flag = '1' and a.create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";

                }
                
                if (Flag.Value.ToString().ToUpper().Equals("ONCHECKOUT"))
                {
                    sql = @"select a.SN,a.workorderno, a.skuno, b.fail_code, a.fail_station, c.in_time,c.out_time,c.in_send_emp, c.in_receive_emp,a.fail_line, a.fail_emp, a.create_time, a.closed_flag  checkout_flag,b.repair_flag from r_repair_main a, r_repair_failcode b, r_repair_transfer c where a.id = b.repair_main_id  and a.id = c.repair_main_id and a.closed_flag = '1' and c.out_time is  null and a.create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";

                }
                if (Flag.Value.ToString().ToUpper().Equals("CHECKOUT"))
                {
                    sql = @"select a.SN,a.workorderno, a.skuno, b.fail_code, a.fail_station, c.in_time,c.out_time,c.in_send_emp, c.in_receive_emp,a.fail_line, a.fail_emp, a.create_time, a.closed_flag  checkout_flag,b.repair_flag from r_repair_main a, r_repair_failcode b, r_repair_transfer c where a.id = b.repair_main_id  and a.id = c.repair_main_id and a.closed_flag = '1' and c.out_time is not null and a.create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";
                }
               
                dt = SFCDB.ExecuteDataSet(sql, CommandType.Text, paramet).Tables[0];
              
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, dt);
                Outputs.Add(reportTable);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw e;
            }
            
        }

    }

    
}
