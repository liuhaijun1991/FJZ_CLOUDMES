using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MESMailCenter
{
    public class MailHelper
    {
        public MailHelper()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
         
        public class MailBodyCode
        {
            public string alertName;
            public string sendName;
            public string sendContext1;
            public string sendContext2;
            public DataSet dataSet;
            public string toMailList;
        }

        public class MailStruct
        {
            public string from { get; set; }
            public string to { get; set; }
            public string cc { get; set; }
            public string bcc { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
        }

        public static string ConverDataToHtml(List<MailBodyCode> mailBodyCodeList)
        {
            /* Old code
            string strhtml = @"<!DOCTYPE html> 
                        <html> 
                        <head> 
                        <meta charset=""utf-8"" \/> 
                        <title></title> 
                        <style>
                        body,table{ 
                        font-size:12px; 
                        } 
                        table{ 
                        table-layout:fixed; 
                        empty-cells:show; 
                        border-collapse: collapse; 
                        margin:0 auto; 
                        } 
                        td{ 
                        height:30px; 
                        } 
                        h1,h2,h3{ 
                        font-size:12px; 
                        margin:0; 
                        padding:0; 
                        } 
                        .table{ 
                        border:1px solid #cad9ea; 
                        color:#666; 
                        } 
                        .table th { 
                        background-repeat:repeat-x; 
                        height:30px; 
                        } 
                        .table td,.table th{ 
                        border:1px solid #cad9ea; 
                        padding:0 1em 0; 
                        } 
                        .table tr.alter{ 
                        background-color:#f5fafe; 
                        } 
                        </style>
                        </head>                   
                        <body> ";
            */
            string strhtml = @"<!DOCTYPE html>
                <html>
                    <head>
                        <meta charset=""utf-8""/>
                        <title></title>
                        <style>
                            body, table {
                                font-size: 12px;
                            }
                            table {
                                table-layout: fixed;
                                empty-cells: show;
                                border-style: solid;
                                border-width: medium thin;
                                border-color: #adadad;
                                border-collapse: collapse;
                                margin: 0 auto;
                                font-family: sans-serif;
                                font-style: normal;
                                background-color: #f5fafe;
                                color: #000;
                            }
                            thead {
                                font-size: 14px;
                                color:#FFF;
                                background-color:#0000a8;
                            }
                            .title {
                                font-size: 30px;
                                font-weight: bold;
                                height: 45px;
                            }
                            td, th {
                                background-repeat: repeat-x;
                                height: 30px;
                                padding: 0 1em 0;
                                border-style: solid;
                                border-width: medium thin;
                                border-color: #adadad;
                                border-collapse: collapse;
                            }
                        </style>
                    </head>
                    <body>";
            foreach (var mailBodyCode in mailBodyCodeList)
            {
                strhtml += @"<div>" + mailBodyCode.sendName + "</div><br/>";
                strhtml += @"<div>" + mailBodyCode.sendContext1 + "</div>";

                if (mailBodyCode.dataSet != null)
                {
                    foreach (DataTable dataTable in mailBodyCode.dataSet.Tables)
                    {
                        if (dataTable.Rows.Count > 0)
                        {
                            /******** HEADER ********/

                            /* Old code
                                // strhtml += @"   <br/><br/><center><font style=""font-size:48px; font-weight:bold;font-family:標楷體; font-style:normal;"">" + dataTable.TableName + "</font></center>";
                                strhtml += @" <br/><br/> <div>     <table class=""table"" border=""1"" cellpadding=""1"" cellspacing=""1"" bordercolor=""#660000"" style=""border-collapse:collapse;"" rules=""none"" >";
                                // strhtml += @" <tr style=""font-size: 20px;color:#000000;background-color:#d9d2e9""><th >" + mailBodyCode.dataTable.TableName + "</th></tr>";
                                strhtml += @" <thead>";
                                strhtml += @" <tr style=""font-size: 14px;color:#000000;background-color:#d9d2e9""><th colspan=" + dataTable.Columns.Count + @"><font style=""font-size:48px; font-weight:bold;font-family:標楷體; font-style:normal;"">" + dataTable.TableName + "</font></th></tr>";
                                strhtml += @"<tr style=""font-size: 14px;color:#000000;background-color:#d9d2e9"">";
                            */
                            strhtml += @"<br/><br/>
                            <div>
                                <table cellpadding = ""1"" cellspacing = ""1"" rules = ""none"">
                                    <thead>
                                        <tr>
                                            <th class = ""title"" colspan = " + dataTable.Columns.Count + @">" + mailBodyCode.alertName.Trim() + "</th></tr><tr>";
                            foreach (DataColumn coulumn in dataTable.Columns)
                            {
                                strhtml += @"<th>" + coulumn.ColumnName + "</th>";
                            }
                            strhtml += @"</tr></thead>";

                            /******** DATA ROWS ********/
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                strhtml += @"<tr>";
                                foreach (DataColumn coulumn in dataTable.Columns)
                                {
                                    //添加按鈕;
                                    if (coulumn.ColumnName.IndexOf("_btnlink") > -1)
                                    {
                                        strhtml += @"<td>";
                                        strhtml += $@"<input type = ""button"" value = ""{coulumn.ColumnName}"" onclick=""btnLink('{ dr[coulumn.ColumnName].ToString()}')"" />";
                                        strhtml += @"</td>";
                                    }
                                    else
                                    {
                                        strhtml += @"<td>" + dr[coulumn.ColumnName].ToString() + "</td>";
                                    }
                                }
                                strhtml += @"</tr>";
                            }
                            strhtml += @"</table></div>";
                        }
                    }
                }
                strhtml += @"<br/><div>" + mailBodyCode.sendContext2 + "</div>";
            }

            strhtml += $@" <br/><br/><div>Send list:<br/>{mailBodyCodeList[0].toMailList}</div>";
            strhtml += @" <script type=""text/javascript""> function btnLink(strLink) { window.open(strLink); } </script > </body></html>";

            return strhtml;
        }

        public object Send(object value)
        {
            MailStruct ms = (MailStruct)value;
            SMTPService.SmtpServiceSoapClient mail = new SMTPService.SmtpServiceSoapClient();
            SMTPService.MailStruct mst = new SMTPService.MailStruct();

            mst.from = ms.from;
            mst.mailto = ms.to;
            mst.cc = ms.cc;
            mst.bcc = ms.bcc;
            mst.body = ms.body;
            mst.subject = ms.subject;

            SMTPService.ContainsKey key = new SMTPService.ContainsKey();
            return mail.SendMail(ref key, mst);
        }

        public object Send(string from, string to, string cc, string bcc, string subject, string body)
        {
            var value = new MailStruct();
            value.from = from;
            value.to = to;
            value.cc = cc;
            value.bcc = bcc;
            value.subject = subject;
            value.body = body;
            return Send(value);
        }

        public object SendNotes(object value)
        {
            MailStruct ms = (MailStruct)value;
            lotusNotesService.LotusMail mail = new lotusNotesService.LotusMail()
            {
                MailTo = new string[] { ms.to },
                MailTitle = ms.subject,
                MailAttachment = "",
                MailBody = ms.body,
                MailSave = false
            };
            lotusNotesService.LotusNotesServiceSoapClient send = new lotusNotesService.LotusNotesServiceSoapClient();
            return send.SendNotesMail(mail);
        }
    }
}
