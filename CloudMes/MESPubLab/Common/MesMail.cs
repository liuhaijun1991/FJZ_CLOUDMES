using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MESPubLab.LotusNotesService;

namespace MESPubLab.Common
{
    public class MesMail
    {
        public MesMail()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public class MailObj
        {
            string mailTitle;
            public string MailTitle
            {
                get
                {
                    if (string.IsNullOrEmpty(mailTitle))
                        throw new Exception("MailTo is not null!");
                    return mailTitle;
                }
                set
                {
                    this.mailTitle = value;
                }
            }

            public string MailBody { get; set; }
            string[] mailTo;
            public string[] MailTo
            {
                get
                {
                    if (mailTo == null || mailTo.Count() == 0)
                        throw new Exception("MailTo is not null!");
                    return mailTo;
                }
                set
                {
                    this.mailTo = value;
                }
            }
            public string MailSave { get; set; }
            public string MailAttachment { get; set; }
            public List<MailBodyCode> Mailbodycodes { get; set; }
        }

        public class MailBodyCode
        {
            public string sendName;
            public string sendContext1;
            public string sendContext2;
            public DataSet dataSet;
        }

        public static string ConverDataToHtml(List<MailBodyCode> mailBodyCodeList, string mailTo)
        {
            /*Old code
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
                                font-family: sans - serif;
                                font-style: normal;
                                background - color: #f5fafe;
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
                                background-repeat: repeat - x;
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
                                            <th class = ""title"" colspan = " + dataTable.Columns.Count + @">" + dataTable.TableName + "</th></tr><tr>";
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
                strhtml += @"<br/><div>" + mailBodyCode.sendContext2 + "</div>";
            }

            strhtml += $@" <br/><br/><div>Send list:<br/>{mailTo}</div>";
            strhtml += @" <script type=""text/javascript""> function btnLink(strLink) { window.open(strLink); } </script > </body></html>";

            return strhtml;
        }

        public object SendNotes(MailObj obj)
        {
            try
            {
                return new LotusNotesService.LotusNotesService().SendNotesMail(new LotusNotesService.LotusMail()
                {
                    MailTitle = obj.MailTitle,
                    MailBody = new Func<string>(() =>
                    {
                        if (obj.Mailbodycodes != null && obj.Mailbodycodes.Count > 0)
                        {
                            return ConverDataToHtml(obj.Mailbodycodes, new Func<string>(() =>
                            {
                                var res = string.Empty;
                                foreach (var item in obj.MailTo)
                                    res += $@"{item},";
                                return res;
                            })());
                        }
                        else
                            return obj.MailBody;
                    })(),
                    MailTo = obj.MailTo,
                    MailAttachment = obj.MailAttachment,
                    MailSave = false
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

}