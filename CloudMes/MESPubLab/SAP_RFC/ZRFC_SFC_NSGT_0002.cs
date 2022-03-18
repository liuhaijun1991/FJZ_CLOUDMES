using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZRFC_SFC_NSGT_0002 : SAP_RFC_BASE
    {
        public ZRFC_SFC_NSGT_0002(string BU) : base(BU)
        {
            SetRFC_NAME("ZRFC_SFC_NSGT_0002");
        }
        public void SetValue(string Wo, string Qty, string WoPostDate)
        {
            this.ClearValues();
            SetValue("I_AUFNR", Wo);
            SetValue("I_LMNGA", Qty);
            SetValue("I_BUDAT", WoPostDate);
        }
        public void SetValue(string Wo, string SAPStationCode, string QTY, string PostDate,string ConfirmationText)
        {
            this.ClearValues();
            SetValue("I_AUFNR", Wo);
            SetValue("I_STATION", SAPStationCode);
            SetValue("I_LMNGA", QTY);
            SetValue("I_BUDAT", PostDate);
            SetValue("I_LTXA1", ConfirmationText);

        }
    }
}

