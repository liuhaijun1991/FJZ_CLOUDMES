using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0140 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0140(string BU)
        : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0140");
        }

        public void SetValues(string WO)
        {
            this.ClearValues();
            this.SetValue("IN_AUFNR", WO);
        }

    }
}
