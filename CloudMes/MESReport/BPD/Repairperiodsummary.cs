using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BPD
{
    class Repairperiodsummary : ReportBase
    {
   
           // ReportInput SKUNO = new ReportInput { Name = "skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
            //ReportInput STATION_NAME = new ReportInput { Name = "station_name", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
            ReportInput StartTime = new ReportInput { Name = "StartTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
            ReportInput EndTime = new ReportInput { Name = "EndTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
            public Repairperiodsummary()
            {
              //  Inputs.Add(SKUNO);
              //  Inputs.Add(STATION_NAME);
                Inputs.Add(StartTime);
                Inputs.Add(EndTime);
            }
            public override void Run()
            {                    
                DataTable loadTable = new DataTable();
                DataTable linkTable = new DataTable();
                DataRow loadTitleRow = null;
                DataRow loadDataRow = null;
                DataRow linkTitleRow = null;
                DataRow linkDataRow = null;
            
                string Stime = DateTime.Parse(StartTime.Value.ToString()).ToString("yyyyMMddhhmmss"); 
                string ETime = DateTime.Parse(EndTime.Value.ToString()).ToString("yyyyMMddhhmmss"); 
                ReportTable retTab = new ReportTable();
                OleExec SFCDB = DBPools["SFCDB"].Borrow();

                try
                {
                string sql1 = @"select count(1) oncheckin from r_repair_main  where id not in (select repair_main_id from r_repair_transfer) and create_time between to_date('"+ Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";
                string sql2 = @"select count(1) onrepair from r_repair_main  where id  in (select repair_main_id from r_repair_transfer) and closed_flag='0'  and create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";
                string sql3 = @"select count(1) repaired from r_repair_main     a,  r_repair_failcode b,   r_repair_transfer c   where a.id = b.repair_main_id  and a.id = c.repair_main_id  and a.closed_flag = '1'  and a.create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";
                string sql4 = @"select count(1) oncheckout  from r_repair_main where id in (select repair_main_id  from r_repair_transfer   where out_time is  null) and closed_flag = '1'  and create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";
                string sql5 = @"select count(1) checkout   from r_repair_main a, r_repair_failcode b, r_repair_transfer c    where a.id = b.repair_main_id and a.id = c.repair_main_id  and a.closed_flag = '1' and c.out_time is not null   and a.create_time between to_date('" + Stime + "','yyyy-mm-dd hh24:mi:ss') and  to_date('" + ETime + "','yyyy-mm-dd hh24:mi:ss')";

                OleDbParameter[] paramet = new OleDbParameter[] {  };
                    linkTable.Columns.Add("Oncheckin");
                    linkTable.Columns.Add("checkin");
                    linkTable.Columns.Add("repaired");
                    linkTable.Columns.Add("Oncheckout");
                    linkTable.Columns.Add("checkout");

                    loadTable.Columns.Add("Oncheckin");
                    loadTable.Columns.Add("checkin");
                    loadTable.Columns.Add("repaired");
                    loadTable.Columns.Add("Oncheckout");
                    loadTable.Columns.Add("checkout");


                    loadTitleRow = loadTable.NewRow();
                    loadDataRow = loadTable.NewRow();
                    linkTitleRow = linkTable.NewRow();
                    linkDataRow = linkTable.NewRow();

                    string Oncheckin = "Oncheckin";
                    loadTitleRow["Oncheckin"] = SFCDB.ExecuteDataTable(sql1, CommandType.Text, paramet).Rows[0][0].ToString();                
                    linkTitleRow["Oncheckin"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.RepairWipReport&RunFlag=1&Flag="+ Oncheckin + "&Start_time="+ Stime+"&End_time="+ETime;

                    string checkin = "checkin";
                    loadTitleRow["checkin"] = SFCDB.ExecuteDataTable(sql2, CommandType.Text, paramet).Rows[0][0].ToString();
                    linkTitleRow["checkin"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.RepairWipReport&RunFlag=1&Flag=" + checkin + "&Start_time=" + Stime + "&End_time=" + ETime;

                    string repaired = "repaired";
                    loadTitleRow["repaired"] = SFCDB.ExecuteDataTable(sql3, CommandType.Text, paramet).Rows[0][0].ToString();
                    linkTitleRow["repaired"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.RepairWipReport&RunFlag=1&Flag=" + repaired + "&Start_time=" + Stime + "&End_time=" + ETime;

                    string Oncheckout = "Oncheckout";
                    loadTitleRow["Oncheckout"] = SFCDB.ExecuteDataTable(sql4, CommandType.Text, paramet).Rows[0][0].ToString();
                    linkTitleRow["Oncheckout"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.RepairWipReport&RunFlag=1&Flag=" + Oncheckout + "&Start_time=" + Stime + "&End_time=" + ETime;

                    string checkout = "checkout";
                    loadTitleRow["checkout"] = SFCDB.ExecuteDataTable(sql5, CommandType.Text, paramet).Rows[0][0].ToString();
                    linkTitleRow["checkout"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.RepairWipReport&RunFlag=1&Flag=" + checkout + "&Start_time=" + Stime + "&End_time=" + ETime;

                    loadTable.Rows.Add(loadTitleRow);
                    loadTable.Rows.Add(loadDataRow);
                    linkTable.Rows.Add(linkTitleRow);
                    linkTable.Rows.Add(linkDataRow);

                    retTab.LoadData(loadTable, linkTable);
                    retTab.Tittle = "Line Output";
                    Outputs.Add(retTab);
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                }
                catch (Exception ee)
                {
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    throw ee;
                }
        }      
    }
}
