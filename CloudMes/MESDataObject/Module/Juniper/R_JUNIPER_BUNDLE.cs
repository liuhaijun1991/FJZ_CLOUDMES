using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.Juniper
{
    public class R_JUNIPER_BUNDLE
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string BNDL_NO { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string PONUMBER { get; set; }
        public string POLINE { get; set; }
        public string SALEORDER { get; set; }
        public string SOITEM { get; set; }
        public string SOID { get; set; }
        public string SCANBY { get; set; }
        public DateTime? BNDLDATETIME { get; set; }
        public string VALID_FLAG { get; set; }
    }
}
