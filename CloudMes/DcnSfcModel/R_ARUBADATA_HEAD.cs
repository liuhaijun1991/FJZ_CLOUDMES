using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DcnSfcModel
{
    [Serializable]
    public class R_ARUBADATA_HEAD
    {
        public R_ARUBADATA_HEAD()
        {

        }
        public string ID { get; set; }
        public string DATEKEY { get; set; }
        public string DATETYPE { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string FILENAME { get; set; }
        public string HEADERRECORD { get; set; }
        public string TRAILERRECORD { get; set; }
        public string SEQ { get; set; }
        public string DATATYPE { get; set; }
        public string GET { get; set; }
        public string CONVERT { get; set; }
        public string SEND { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
    }
}
