using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MESMailCenter
{
    public class SendReportProcess : Process
    {
        string _name;
        string _API;
        string _UserName;
        string _PWD;
        string _ReportClass;
        string _VALUES;
        string _Tittle;
        string _STRDBCONN;

        
        string[] _MailTo;
        
        
        public string _TimeSpan;
        
        DataSet _Data;
        LogManagedItem Log;
       
       
        bool _useSMTP;
        string _SMTP_IP;
        string _SMTP_FROM;
        public SendReportProcess(DataRow configItem)
        {
            try
            {
                _name = (string)configItem["Name"];
                _TimeSpan = configItem["TimeSpan"].ToString();
                _API = (string)configItem["API"];
                _UserName = (string)configItem["UserName"];
                _PWD = (string)configItem["PWD"];
                _ReportClass = (string)configItem["ReportClass"];
                try
                {
                    _VALUES = (string)configItem["VALUES"];
                    _MailTo = ((string)configItem["MailTo"]).Split(new char[] { ',', ';' });
                    _Tittle = (string)configItem["Tittle"];
                    Log = new LogManagedItem(_name, LogMode.File);
                    _useSMTP = (bool)configItem["UseSMTP"];
                    _SMTP_IP = (string)configItem["SMTP_IP"];
                    _SMTP_FROM = (string)configItem["SMTP_FROM"];
                    //_STRDBCONN = (string)configItem["STRDBCONN"];
                }
                catch
                { }
               

            }
            catch (Exception EE)
            {
                throw EE;
            }

           

        }

        public override void Start()
        {
            MESAPIClient API = new MESAPIClient(_API, _UserName, _PWD);
            MESAPIData data = new MESAPIData();
            data.Class = "MESStation.Report.CallReport";
            data.Function = "GetReport";
            data.Data = new { ClassName = _ReportClass };

            var report = API.CallMESAPISync(data, 10000);

            MESAPIData data1 = new MESAPIData();
            data1.Class = "MESStation.Report.CallReport";
            data1.Function = "DownFile";
            data1.Data = new { ClassName = _ReportClass , Report = report["Data"] };
            var report1 = API.CallMESAPISync(data1, 100000);
            var apppath = AppDomain.CurrentDomain.BaseDirectory;
            if (!System.IO.Directory.Exists(apppath + "\\Reports"))
            {
                System.IO.Directory.CreateDirectory(apppath + "\\Reports");
            }
            List<string> files = new List<string>();
            for (int i = 0; i < report1["Data"]["Outputs"].Count(); i++)
            {
                if (report1["Data"]["Outputs"][i]["OutputType"].ToString() == "ReportFile")
                {
                    var filename = report1["Data"]["Outputs"][i]["FileName"].ToString();
                    var bytes = System.Convert.FromBase64String(report1["Data"]["Outputs"][i]["FileContent"].ToString());
                    System.IO.FileStream fs = new System.IO.FileStream(apppath + "\\Reports\\" + filename, System.IO.FileMode.Create);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                    files.Add(apppath + "\\Reports\\" + filename);
                }
            }

            if (_useSMTP)
            {
                MailHelper.MailBodyCode mailBodyCode = new MailHelper.MailBodyCode()
                {
                    alertName = _name,
                    sendName = "",
                    sendContext1 = "",
                    sendContext2 = "",
                    dataSet = _Data,
                    toMailList = "",
                };

                List<MailHelper.MailBodyCode> mailBodyCodeList = new List<MailHelper.MailBodyCode>();
                mailBodyCodeList.Add(mailBodyCode);
                var body = MailHelper.ConverDataToHtml(mailBodyCodeList);
                foreach (string str in _MailTo)
                {
                    var Mailto = str;
                    Mailto = Mailto.Replace("\r\n", "").ToString();
                    
                    sendEmailSMTP(_SMTP_IP, Mailto, _SMTP_FROM, _Tittle, body, files);
                }
            }
            else
            {
                if (files.Count > 0)
                {
                    SendMailByActiveNotes.SendMailClass mail = new SendMailByActiveNotes.SendMailClass();
                    foreach (string str in _MailTo)
                    {
                        try
                        {
                            var mailTo = str;
                            mailTo = mailTo.Replace("\r\n", "").ToString();
                            foreach (var file in files)
                            {
                                string mesg = mail.SendNotesMail(_Tittle, file, mailTo, "", false);
                                if (mesg != "OK")
                                {
                                    Log.Write(mesg);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Log.Write($"{_Tittle} send fail.error message:{ex.Message}");
                        }
                    }
                }
            }



        }

        public static void sendEmailSMTP(string SMTP_IP, String To, String From, String Subject, String Body, List< string> AttachFile1 = null)
        {

            MailMessage Email = new MailMessage();

            if (AttachFile1 != null)
            {
                for (int i = 0; i < AttachFile1.Count; i++)
                {
                    Attachment Attach1 = new Attachment(AttachFile1[i]);
                    Email.Attachments.Add(Attach1);
                }
                
            }

            Email.From = new MailAddress(From);
            Email.To.Add(To);
            Email.Subject = Subject;
            Email.IsBodyHtml = true;
            Email.Body = Body;
            Email.Priority = MailPriority.Normal;
            SmtpClient client = new SmtpClient(SMTP_IP);
            //client.Credentials = new System.Net.NetworkCredential("账号", "***密码**");
            client.Send(Email);
        }
    }
}
