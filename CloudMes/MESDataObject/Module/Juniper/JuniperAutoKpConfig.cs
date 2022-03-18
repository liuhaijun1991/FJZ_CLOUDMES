using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.Juniper
{
    public class JuniperAutoKpConfig
    {
        public string PN { get; set; }
        public string PN_SERIALIZATION { get; set; }
        public string CUST_PN { get; set; }
        public string PN_7XX { get; set; }
        public string SN_RULE { get; set; }
        public float QTY { get; set; }
        public string TYPE { get; set; }
        public string REV { get; set; }
        public string CLEI_CODE { get; set; }
        public string CHAS_SN { get; set; }
    }

    public class JuniperAutoKpConfig_Extend
    {
        public string WorkOrderNo { get; set; }

        public List<JuniperAutoKpConfig> AutoKpConfig { get; set; }
    }
}
