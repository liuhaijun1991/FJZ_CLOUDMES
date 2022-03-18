using MESDBHelper;
using System;
using System.Data;

namespace MESReport.DCN
{
    /// <summary>
    /// 維修WIP狀態Detail報表For DCN
    /// </summary>
    public class RepairWipStatusDetail : ReportBase
    {
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Type = new ReportInput() { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };

        public RepairWipStatusDetail()
        {
            Inputs.Add(SkuNo);
            Inputs.Add(StartDate);
            Inputs.Add(EndDate);
            Inputs.Add(Type);
        }

        public override void Run()
        {
            string skuNo = SkuNo.Value.ToString();
            DateTime sTime = Convert.ToDateTime(StartDate.Value);
            DateTime eTime = Convert.ToDateTime(EndDate.Value);
            string sValue = sTime.ToString("yyyy/MM/dd");
            string eValue = eTime.ToString("yyyy/MM/dd");
            string type= Type.Value.ToString();
            string runSql = "";
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                #region SQL語句
                runSql = $@"
                    select a.sn,
                           a.create_time,
                           a.workorderno,
                           a.skuno,
                           a.fail_station,
                           b.fail_process,
                           b.fail_location,
                           b.fail_code,
                           b.description,
                           a.closed_flag
                      from r_repair_main a, r_repair_failcode b
                     where a.id = b.repair_main_id
                       and a.create_time between
                           to_date('{sValue}', 'yyyy/mm/dd') and
                           to_date('{eValue}', 'yyyy/mm/dd')
                       and a.skuno = '{skuNo}'
                       TEMP_SQL
                     order by a.sn, a.create_time";
                #endregion

                if (type == "INLINE")
                {
                    runSql = runSql.Replace("TEMP_SQL", " and a.id not in (select repair_main_id from r_repair_transfer) ");
                }
                else if (type == "REWIP")
                {
                    runSql = runSql.Replace("TEMP_SQL", " and a.id in (select repair_main_id from r_repair_transfer where closed_flag = 0) ");
                }

                RunSqls.Add(runSql);
                DataTable resDT = SFCDB.RunSelect(runSql).Tables[0];
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resDT, null);
                retTab.Tittle = "Repair Wip Status Detail";
                Outputs.Add(retTab);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }
    }
}
