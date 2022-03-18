using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace MESInterface.VERTIV
{
    public class SendYieldAlertEmail : taskBase
    {
        public string dbStr = "";
        public string hour = "";
        OleExec SFCDB;
        bool isRun = false;
        public string mailToStr = "";
        public string mailTitle = "";
        public override void init()
        {
            dbStr = ConfigGet("MESDB");
            hour = ConfigGet("HOUR");
            mailToStr = ConfigGet("MailTo");
            mailTitle = ConfigGet("MailTitle");
        }
        public override void Start()
        {
            try
            {
                if(isRun)
                {
                    throw new Exception("Is running.");
                }
                isRun = true;
                SFCDB = new OleExec(dbStr, false);
                Func<IPAddress, bool> func = e => e.AddressFamily.ToString() == "InterNetwork";
                IPAddress ipAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(func);
                string ip = ipAddress != null ? ipAddress.ToString() : "";
                string[] mailTo = mailToStr.Split(',');
                DateTime sysdate = SFCDB.ORM.GetDate();
                double h = string.IsNullOrEmpty(hour) ? 4 : Convert.ToDouble(hour);
                DateTime beginTime = sysdate.AddHours(-h);
                MESReport.BaseReport.TestReportBySN testReport = new MESReport.BaseReport.TestReportBySN();
                Predicate<MESReport.ReportInput> startDate = t => t.Name == "StartDate";
                testReport.Inputs.Find(startDate).Value = beginTime;
                Predicate<MESReport.ReportInput> endDate = t => t.Name == "EndDate";
                testReport.Inputs.Find(endDate).Value = sysdate;                
                DataTable dt = testReport.GetSendAlertEmail(SFCDB);
                //良率不達標，請CFT優先分析處理
                if (dt.Rows.Count > 0)
                {                    
                    string mailContent = $@"IP:{ip};</br>CreatBy:SendYieldAlertEmail;</br></br>{mailTitle}：</br>";                   
                    string thStyle = $@"style=""padding: 0 5px; text-align:center; height:30px; background-color:#aca7a7;""";
                    string tdStyle = $@"style=""padding: 0 5px; text-align:center;""";
                    //mailContent = "";
                    mailContent += $@"<table style=""border: 1px solid #808080;"" cellspacing=""0"" cellpadding=""0"" border=""1""><thead><tr>";
                    foreach (var c in dt.Columns)
                    {
                        mailContent += $@"<th {thStyle}>{c.ToString()}</th>";
                    }
                    mailContent += "</tr></thead><tbody>";

                    foreach (DataRow row in dt.Rows)
                    {
                        if (!row["SKUNO"].ToString().Equals("ALL"))
                        {
                            mailContent += "<tr>";
                            foreach (var c in dt.Columns)
                            {
                                mailContent += $@"<td {tdStyle}>{row[c.ToString()].ToString()}</td>";
                            }
                            mailContent += "</tr>";
                        }
                    }
                    mailContent += "</tbody></table>";
                    NotesService.LotusMail mail = new NotesService.LotusMail()
                    {
                        MailTitle = $@"{mailTitle}",
                        MailBody = mailContent,
                        MailTo = mailTo,
                        MailSave = false
                    };
                    NotesService.LotusNotesService ln = new NotesService.LotusNotesService();
                    ln.SendNotesMail(mail);
                }
                System.Threading.Thread.Sleep(60000);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                isRun = false;
            }
        }
    }
}
