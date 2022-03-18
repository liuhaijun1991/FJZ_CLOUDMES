using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0593 : SAP_RFC_BASE
    {

        public ZCPP_NSBG_0593(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0593");
        }

        public void SetValue(string I_BASDAY, string I_PLSCN, string I_WERKS)
        {
            this.ClearValues();
            this.SetValue("I_BASDAY", I_BASDAY);
            this.SetValue("I_PLSCN", I_PLSCN);
            this.SetValue("I_WERKS", I_WERKS);
        }
        
    }
}
