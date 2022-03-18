using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESInterface
{
    public class EmailObj
    {
        public string mailFrom;
        public string mailTo;
        public string mailCC;
        public string mailSubject;
        public string mailBody;
        public string mailPriority;
        public string IsBodyHtml;
        public Dictionary<string, FileStream> Attachments = new Dictionary<string, FileStream>();
        
    }
}
