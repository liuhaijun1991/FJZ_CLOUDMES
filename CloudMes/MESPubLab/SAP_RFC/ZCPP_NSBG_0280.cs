using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    /// <summary>
    /// Reverse Work Order
    /// O_FLAG	    CHAR		Flag Material for Deletion at Client Level
    /// O_MESSAGE   CHAR        Contents of a Selected List Line
    /// I_AUFNR     CHAR        Order Number
    /// I_LMNGA     BCD         Yield to Be Confirmed
    /// I_LTXA1     CHAR        Confirmation text (ZRFC_SFC_NSGT_0002.I_LTXA1)
    /// I_VORNR     CHAR        Operation/Activity Number
    /// I_WERKS     CHAR        Plant
    /// </summary>
    public class ZCPP_NSBG_0280 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0280(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0280");
        }
        public void SetValue(string Wo, string SAPStationCode, string QTY, string plant, string ConfirmationText)
        {
            this.ClearValues();
            SetValue("I_AUFNR", Wo);
            SetValue("I_VORNR", SAPStationCode);
            SetValue("I_LMNGA", QTY);
            SetValue("I_LTXA1", ConfirmationText);
            SetValue("I_WERKS", plant);
        }
    }
}

