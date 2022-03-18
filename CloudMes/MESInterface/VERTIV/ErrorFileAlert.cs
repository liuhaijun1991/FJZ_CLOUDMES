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
    public class ErrorFileAlert :  taskBase
    {
        public string errorFilePath = "";
        public string mailToStr = "";
        public string mailTitle = "";        
        public override void init()
        {
            mailToStr = ConfigGet("MailTo");
            errorFilePath = ConfigGet("ErrorFilePath");
            mailTitle = ConfigGet("MailTitle");            
        }
        public override void Start()
        {
            Func<IPAddress, bool> func = e => e.AddressFamily.ToString() == "InterNetwork";
            IPAddress ipAddress= Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(func);
            string ip = ipAddress != null ? ipAddress.ToString() : "";            
            string[] mailTo = mailToStr.Split(',');
            try
            {
                string mailContent = $@"IP:{ip};</br>CreatBy:Interface ErrorFileAlert;</br>Error File List:</br>";
                DirectoryInfo dirInfo = new DirectoryInfo(errorFilePath);
                var fileList = dirInfo.GetFiles();
                foreach (FileInfo file in fileList)
                {
                    mailContent += $@"{file.FullName};</br>";
                }
                NotesService.LotusMail mail = new NotesService.LotusMail() { 
                    MailTitle=$@"{mailTitle}",
                    MailBody=mailContent,
                    MailTo=mailTo,
                    MailSave=false
                };
                if(fileList.Length>0)
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
    }
}
