using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESReport.BaseReport
{
    //維修WIP 報表
   public  class RepairWipReport:ReportBase
    {

        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Event = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "AOI1", "AOI2", "VI1", "VI2" } };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };

        public RepairWipReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            Inputs.Add(Event);
            Inputs.Add(SkuNo);
        }
        public override void Init()
        {
            // StartTime.Value = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss");
            //  EndTime.Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                InitSkuno(SFCDB);
                InitStation(SFCDB);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public override void Run()
        {
            DateTime stime =Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string runSql = $@"SELECT  SKUNO,FAIL_STATION,COUNT(*) QTY
                               FROM R_REPAIR_MAIN  WHERE closed_flag = 0
                               AND CREATE_TIME BETWEEN TO_DATE ('{svalue}',
                               'YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{evalue}',
                               'YYYY/MM/DD HH24:MI:SS') ";

                if (Event.Value.ToString() != "ALL")
                {
                    runSql += $@"and FAIL_STATION = '{ Event.Value.ToString()}'";
                }
                if (SkuNo.Value.ToString() != "ALL")
                {
                    runSql += $@"and FAIL_STATION = '{SkuNo.Value.ToString()}'";
                }
                runSql = runSql + " GROUP BY SKUNO,FAIL_STATION";
                RunSqls.Add(runSql);
                DataSet res = SFCDB.RunSelect(runSql);
                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "Repair wip";
                Outputs.Add(retTab);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }
        }

        public void InitSkuno(OleExec db)
        {
            List<string> skuno = new List<string>();
            DataTable dt = new DataTable();
            T_C_SKU sku = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
            dt = sku.GetALLSkuno(db);
            skuno.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                skuno.Add(dr["SKUNO"].ToString());

            }
            SkuNo.ValueForUse = skuno;

        }

        public void InitStation(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            T_C_ROUTE_DETAIL S = new T_C_ROUTE_DETAIL(db, DB_TYPE_ENUM.Oracle);
            dt = S.GetALLStation(db);
            station.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                station.Add(dr["station_name"].ToString());

            }
            Event.ValueForUse = station;
        }
    }
}
