using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Net.Mail;

namespace MESMailCenter
{
    class SQLAlarmProcess: Process
    {
        string _name;
        string _strSql;
        OleExec DBExec;
        string[] _mailList;
        string[] _mailTittle;
        bool useExcelFile;
        bool _useWebServiceSend;
        public string _TimeSpan;
        string _DBName;
        string _strExecSQL = "";
        DataSet _Data;
        LogManagedItem Log;
        bool _NoRecordSendMail;
        string _context1;
        string _context2;
        bool useSMTP;
        string SMTP_IP;
        string SMTP_FROM;
        string LANGUAGE;
        public override void Start()
        {

            DBExec = new OleExec(_DBName, true);
            try
            { _Data.Clear(); }
            catch
            { }

            try
            {

                if (_strSql.Trim() != "")
                {
                    RunSendMail();
                }
                if (_strExecSQL.Trim() != "")
                {
                    RunExecSQL();
                }
            }
            catch (Exception ee)
            {
                DBExec.FreeMe();
                throw ee;
            }
            DBExec.FreeMe();
        }
        public override string ToString()
        {
            return _name;
        }
        public SQLAlarmProcess(DataRow configItem )
        {
            try
            {
                _name = (string)configItem["Name"];
                _TimeSpan = configItem["TimeSpan"].ToString();
                _strSql = (string)configItem["SQL"];
                _DBName = (string)configItem["DBKey"];
                //_mailList = ((string)configItem["MailTo"]).Split(new char[] { ',' });
                _mailList = (GetMailList((string)configItem["MailTo"])).Split(new char[] { ',',';' });
                useExcelFile = (bool)configItem["UseExcel"];
                _NoRecordSendMail = (bool)configItem["NoRecordSendMail"];
                _useWebServiceSend =(bool)configItem["UseWebServiceSend"];
                _strExecSQL = (string)configItem["EXECSQL"];
                _context1 = (string)configItem["Context1"];
                _context2 = (string)configItem["Context2"];
                //DBExec = new OleExec(_DBName, true);
                Log = new LogManagedItem(_name, LogMode.File);
                useSMTP = (bool)configItem["UseSMTP"];
                SMTP_IP = (string)configItem["SMTP_IP"];
                SMTP_FROM = (string)configItem["SMTP_FROM"];
                LANGUAGE = (string)configItem["LANGUAGE"];

            }
            catch(Exception EE )
            {
                throw EE;
            } 
            
        }

        void RunSendMail()
        {
            _Data = DBExec.RunSelect(_strSql);
            string tittle = _name + " - [本次無資料!]";
            /*
                繁體中文
                English
                简体中文
             */
            if (LANGUAGE == "简体中文")
            {
                tittle = _name + " - [本次无资料!]";
            }

            if (LANGUAGE == "English")
            {
                tittle = _name + " - [No Data!]";
            }

            string FileName = "";
            if (_Data.Tables[0].Rows.Count > 0 )
            {
                if (Directory.Exists(".\\SendFiles"))
                {
                    FileName = _name + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";

                    string strVer = MESMailCenter.Excel.GetVer();
                    string[] vers = strVer.Split(new char[] { '.' });
                    try
                    {
                        int intVer = Int32.Parse(vers[0]);
                        if (intVer <= 11)
                        {
                            if (FileName.ToUpper().EndsWith("XLSX"))
                            {
                                FileName = FileName.Substring(0, FileName.Length - 1);
                            }
                        }
                        else
                        {
                            if (FileName.ToUpper().EndsWith("XLS"))
                            {
                                FileName = FileName + "x";
                            }
                        }
                    }
                    catch
                    { }


                    List<DataTable> datas = new List<DataTable>();
                    datas.Add(_Data.Tables[0]);

                    System.IO.Directory.CreateDirectory(".\\SendFiles");
                    //throw new Exception("It haven't Logs dir !");

                    //AppDomain.CurrentDomain.BaseDirectory
                    FileName = AppDomain.CurrentDomain.BaseDirectory + "SendFiles\\" + FileName;
                    if (useExcelFile)
                    {
                        MESMailCenter.Excel.CreateExcelFile(datas, FileName);
                    }
                    
                }
            }
            else
            {
                if (!_NoRecordSendMail)
                {
                    Log.Write("本次無資料");
                    return;
                }
            }
            tittle = _name + " - [本次共" + _Data.Tables[0].Rows.Count.ToString() + "筆資料]";
            if (LANGUAGE == "简体中文")
            {
                tittle = _name + " - [本次共" + _Data.Tables[0].Rows.Count.ToString() + "笔资料]";
            }

            if (LANGUAGE == "English")
            {
                tittle = _name + $@" - [Total No. of Record:{_Data.Tables[0].Rows.Count} ]";
            }
            string body="";
            if (!useExcelFile)
            { 
                for(int i = 0 ;i<_Data.Tables.Count;i++)
                {
                    body += "Table" + i.ToString() + ":\r\n";
                    DataTable tab = _Data.Tables[i];
                    body += "\n";
                    for (int j = 0; j < tab.Columns.Count; j++)
                    {
                        body +="" + tab.Columns[j].ColumnName + "\t";
                    }
                    body += "\r\n";
                    for (int k = 0; k < tab.Rows.Count; k++)
                    {
                        body += "";
                        for (int Q = 0; Q < tab.Columns.Count; Q++)
                        {
                            body += "" + tab.Rows[k][Q].ToString() + "\t";
                        }
                        body += "\r\n";
                    }

                }
                FileName = "";
                body += "-----------------\r\n";
            }

            body += "系統自動發出的郵件,請不要直接回覆!\r\n本郵件向以下郵箱發送:";
            //foreach (string str in _mailList)
            string Mailto;
            string MailList = "";
            foreach (string str in _mailList)
            {
                Mailto = str;
                Mailto = Mailto.Replace("\r\n", "").ToString();
                body += "\r\n" + Mailto;
                MailList += "\r\n" + Mailto;
            }
            
            if (useSMTP)            
            {
                MailHelper.MailBodyCode mailBodyCode = new MailHelper.MailBodyCode()
                {
                    alertName = _name,
                    sendName = "",
                    sendContext1 = _context1,
                    sendContext2 = _context2,
                    dataSet = _Data,
                    toMailList = MailList,
                };

                List<MailHelper.MailBodyCode> mailBodyCodeList = new List<MailHelper.MailBodyCode>();
                mailBodyCodeList.Add(mailBodyCode);
                body = MailHelper.ConverDataToHtml(mailBodyCodeList);
                foreach (string str in _mailList)
                {
                    Mailto = str;
                    Mailto = Mailto.Replace("\r\n", "").ToString();
                    //string mesg = mail.SendNotesMail(tittle, FileName, Mailto, body, false);
                    if (!useExcelFile)
                    {
                        sendEmailSMTP(SMTP_IP, Mailto, SMTP_FROM, tittle, body);
                    }
                    else
                    { //FileName
                        sendEmailSMTP(SMTP_IP,Mailto, SMTP_FROM, tittle, body, FileName);
                    }

                    
                }
            }
            else if (!_useWebServiceSend)
            {
                SendMailByActiveNotes.SendMailClass mail = new SendMailByActiveNotes.SendMailClass();
                foreach (string str in _mailList)
                {
                    Mailto = str;
                    Mailto = Mailto.Replace("\r\n", "").ToString();
                    string mesg = mail.SendNotesMail(tittle, FileName, Mailto, body, false);
                    if (mesg != "OK")
                    {
                        Log.Write(mesg);
                    }
                }
            }
            else
            {
                MailHelper.MailBodyCode mailBodyCode = new MailHelper.MailBodyCode()
                {
                    sendName = "",
                    sendContext1 = _context1,
                    sendContext2 = _context2,
                    dataSet = _Data,
                    toMailList = MailList
                };
                List<MailHelper.MailBodyCode> mailBodyCodeList = new List<MailHelper.MailBodyCode>();
                mailBodyCodeList.Add(mailBodyCode);

                foreach (string str in _mailList)
                {
                    Mailto = str;
                    Mailto = Mailto.Replace("\r\n", "").ToString();
                    MailHelper.MailStruct ms = new MailHelper.MailStruct()
                    {
                        from = "cnsbg-sfc-alert@foxconn.com",
                        to = Mailto,
                        cc = "",
                        subject = tittle,
                        body = MailHelper.ConverDataToHtml(mailBodyCodeList)
                    };
                    MailHelper mh = new MailHelper();
                    string mesg = mh.SendNotes(ms).ToString();
                    if (mesg != "OK")
                    {
                        Log.Write(mesg);
                    }
                }
            }
            Log.Write(tittle);
        }
        public static void sendEmailSMTP(string SMTP_IP, String To, String From, String Subject, String Body, String AttachFile1 = "")
        {

            MailMessage Email = new MailMessage();

            if (AttachFile1 != "")
            {
                Attachment Attach1 = new Attachment(AttachFile1);
                Email.Attachments.Add(Attach1);
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



        void RunExecSQL()
        {
            string[] sqls = this._strExecSQL.Split(new char[]{';'});
            DBExec.BeginTrain();
            try
            {
                for (int i = 0; i < sqls.Length; i++)
                {
                    string c = DBExec.ExecSQL(sqls[i]);
                    try
                    {
                        int C = Int32.Parse(c);
                    }
                    catch
                    {
                        throw new Exception(c);
                    }
                }
            }
            catch (Exception ee)
            {
                DBExec.RollbackTrain();
                throw ee;
            }
            DBExec.CommitTrain();
        }

        private string GetMailList(string value)
        {
            string MailList = "";
            try
            {
                if (value.Trim().ToUpper().StartsWith("SELECT"))
                {
                    OleExec DBValue;
                    DBValue = new OleExec(_DBName, true);
                    DataSet dsmail = DBValue.RunSelect(value);
                    DBValue.FreeMe();
                    if (dsmail.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in dsmail.Tables[0].Rows)
                        {
                            MailList = r[0].ToString() + "," + MailList;
                        }
                        if (MailList.Length > 2) MailList = MailList.Substring(0, MailList.Length - 1);
                    }
                }
                else
                {
                    MailList = value;
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return MailList;
        }
    }
}
