using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESInterface.CRC
{
    public class CrcObj
    {
        public string F_SITE { get; set; }
        public string F_BU { get; set; }
        public string F_FLOOR { get; set; }
        public string F_SUBJECT { get; set; }
        public string F_PRIORITY { get; set; }
        public string F_ERRORMESSAGE { get; set; }
        public string F_DATASOURCE { get; set; }
        public string F_REPORTER { get; set; }
        public string F_CONTACTNUMBER { get; set; }
        public string F_EMAIL { get; set; }
        public string F_SENDER { get; set; }
        public string F_OWNER { get; set; }
        public string F_ESCALATION1 { get; set; }
        public string F_ESCALATION2 { get; set; }
        public string F_ESCALATION3 { get; set; }
        public string F_CASEGROUP { get; set; }
    }

    public class CrcRes
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }
}
