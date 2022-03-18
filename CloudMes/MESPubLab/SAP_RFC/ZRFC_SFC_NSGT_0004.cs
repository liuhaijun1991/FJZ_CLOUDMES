using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
   public class ZRFC_SFC_NSGT_0004 : SAP_RFC_BASE
    {
        public ZRFC_SFC_NSGT_0004(string BU):base(BU)
        {
            SetRFC_NAME("ZRFC_SFC_NSGT_0004");
        }
        public void SetValue(string Wo, string Qty, string WoPostDate)
        {
            this.ClearValues();
            SetValue("I_AUFNR", Wo);
            SetValue("I_LMNGA", Qty);
            SetValue("I_BUDAT", WoPostDate);
        }
    }
}
