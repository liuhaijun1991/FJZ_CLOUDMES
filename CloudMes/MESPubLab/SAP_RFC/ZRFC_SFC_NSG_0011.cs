using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZRFC_SFC_NSG_0011 : SAP_RFC_BASE
    {
        /*
        O_DOCUMENTNO	CHAR		List processing, contents of selected line
        O_FLAG	        CHAR		Flag Material for Deletion at Client Level
        O_MESSAGE	    CHAR		List processing, contents of selected line
        CHARG	        CHAR		Document header text
        I_BKTXT	        CHAR  		Document header text
        I_BUDAT	        DATE	    SY-DATUM	Document header text
        I_ERFMG	        BCD		    Quantity in unit of entry
        I_FROM	        CHAR		Storage location
        I_MATNR	        CHAR		Material number
        I_TO	        CHAR		Storage location
        PLANT	        CHAR		Plant
        */
        public ZRFC_SFC_NSG_0011(string BU) : base(BU)
        {
            SetRFC_NAME("ZRFC_SFC_NSG_0011");
        }
    }
}
