using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.VERTIV
{
    public class VTCreatASNAlert : taskBase
    {
        public string buStr = "";
        private string mesDBStr = "";
        SqlSugarClient SFCDB = null;
        public string mailToStr = "";
        public string mailTitle = "";

        public override void init()
        {
            mesDBStr = ConfigGet("DB");
            mailToStr = ConfigGet("MailTo");
            mailTitle = ConfigGet("MailTitle");
            SFCDB = OleExec.GetSqlSugarClient(mesDBStr, false);
        }
        public override void Start()
        {            
            Func<IPAddress, bool> func = e => e.AddressFamily.ToString() == "InterNetwork";
            IPAddress ipAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(func);
            string ip = ipAddress != null ? ipAddress.ToString() : "";
            string[] mailTo = mailToStr.Split(',');
            try
            {
                var alertList = MESStation.Interface.Vertiv.VertivB2B.GetASNAlertPO(SFCDB);
                string mailContent = $@"IP:{ip};</br>CreatBy:Interface VTCreatASNAlert;</br></br>以下PO超過3天還未生成ASN，請注意：</br>";
                //   , ID, ORDER_NUMBER, ORDER_LINE_ID, SCHEDULE_ID, CREATED_TIME, SEND_COMMITFILE_TIME, FILE_NAME
                string thStyle = $@"style='padding: 0 5px; text-align:center; height: 30px; background-color:#aca7a7;color:#ffffff;'";
                string tdStyle = $@"style='padding: 0 5px; text-align:center;'";
                mailContent += $@"<table style='border: 1px solid #808080;' cellspacing='0' cellpadding='0' border='1'><thead>
                                    <tr>
                                    <th {thStyle}>ID</th>
                                    <th {thStyle}>ORDER_NUMBER</th>
                                    <th {thStyle}>ORDER_LINE_ID</th>
                                    <th {thStyle}>SCHEDULE_ID</th>
                                    <th {thStyle}>CREATED_TIME</th>
                                    <th {thStyle}>SEND_COMMITFILE_TIME</th>
                                    <th {thStyle}>FILE_NAME</th>
                                    </tr></thead><tbody>";
                foreach (var poObj in alertList)
                {
                    mailContent += $@" <tr>
                                    <td {tdStyle}>{poObj.ID}</td>
                                    <td {tdStyle}>{poObj.ORDER_NUMBER} </td>
                                    <td {tdStyle}>{poObj.ORDER_LINE_ID}</td>
                                    <td {tdStyle}>{poObj.SCHEDULE_ID}</td>
                                    <td {tdStyle}>{poObj.CREATED_TIME}</td>
                                    <td {tdStyle}>{poObj.EDIT_TIME} </td>
                                    <td {tdStyle}>{poObj.FILE_NAME}</td>
                                    </tr>";
                }
                mailContent += "</tbody></table>";
                NotesService.LotusMail mail = new NotesService.LotusMail()
                {
                    MailTitle = $@"{mailTitle}",
                    MailBody = mailContent,
                    MailTo = mailTo,
                    MailSave = false
                };
                if (alertList.Count > 0)
                {
                    NotesService.LotusNotesService ln = new NotesService.LotusNotesService();
                    ln.SendNotesMail(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FindPoByFile()
        {
            string po = "14071094704";
            DirectoryInfo dirInfo = new DirectoryInfo($@"D:\15");
            var fileList = dirInfo.GetFiles();
            foreach (FileInfo file in fileList)
            {
                var lines = File.ReadAllText(file.FullName);
                if(lines.Contains(po))
                {
                    MESPubLab.Common.MesLog.Info($"PO File:{file.FullName} ");
                }
            }
        }
    }
}
