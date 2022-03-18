using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Vertiv
{
    public class TestYieldAlert : ReportBase
    {
        public override void Init()
        {
            base.Init();
        }
        public override void Run()
        {
            base.Run();
        }
        public override void DownFile()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DateTime sysdate = SFCDB.ORM.GetDate();              
                DateTime beginTime = sysdate.AddHours(-4);
                BaseReport.TestReportBySN testReport = new BaseReport.TestReportBySN();
                Predicate<ReportInput> startDate = t => t.Name == "StartDate";
                testReport.Inputs.Find(startDate).Value = beginTime;
                Predicate<ReportInput> endDate = t => t.Name == "EndDate";
                testReport.Inputs.Find(endDate).Value = sysdate;
                DataTable dt = testReport.GetSendAlertEmail(SFCDB);
                if(dt.Rows.Count>0)
                {
                    string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                    string fileName = "YieldAlart_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    Outputs.Add(new ReportFile(fileName, content));
                }
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
