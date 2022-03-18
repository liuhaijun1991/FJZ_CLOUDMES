using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Vertiv
{
    public class StationOnLineReport: ReportBase
    {
       
        ReportInput StartDateInput = new ReportInput()
        {
            Name = "START_DATE",
            InputType = "DateTime",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput EndDateInput = new ReportInput()
        {
            Name = "END_DATE",
            InputType = "DateTime",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        public StationOnLineReport()
        {
            Inputs.Add(StartDateInput);
            Inputs.Add(EndDateInput);
        }
        public override void Init()
        {
            StartDateInput.Value = DateTime.Now.AddDays(-1);
            EndDateInput.Value = DateTime.Now;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public override void Run()
        {
            try
            {
                string StartDate = StartDateInput.Value.ToString();
                string EndDate = EndDateInput.Value.ToString();
                string runSql = $@"select Line,
                                               Station,
                                               CREATEBY       AS WorkUser,
                                               emp_name       as WorkUserName,
                                               DEFECT1,
                                               DEFECT2,
                                               CREATETIME     AS OnTime,
                                               bbb.ACTIONNAME AS OffTime
                                          from r_station_scan_log bbb, c_user b
                                         where bbb.SCANKEY = 'STATIONONLINE'
                                           AND bbb.CREATEBY = b.emp_no
                                           and bbb.CREATETIME BETWEEN
                                               TO_DATE('2022/1/23 8:00:00', 'YYYY/MM/DD HH24:MI:SS') AND
                                               TO_DATE('{EndDate}', 'YYYY/MM/DD HH24:MI:SS')
                                           and (bbb.DEFECT1 is not null  OR bbb.DEFECT2 is not null)
                                         order by bbb.line";
                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                try
                {
                    RunSqls.Add(runSql);
                    DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    ReportTable retTab = new ReportTable();

                    retTab.LoadData(dt, null);
                    retTab.Tittle = "人員上線模塊-Defect看板";
                    retTab.FixedHeader = true;
                    retTab.pagination = false;
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
