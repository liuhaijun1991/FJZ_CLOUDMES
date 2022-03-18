using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Interface.SAPRFC
{
    /// <summary>
    /// Use for wo to MRB BackFlush
    /// </summary>
    public class ZRFC_SFC_NSG_0020:SAP_RFC_BASE
    {
        /************************************************************************
            O_ERROR	CHAR		Variable part of a message
            O_ERRORMESSAGE	CHAR		Variable part of a message
            O_FLAG	CHAR		Flag Material for Deletion at Client Level
            O_FLAG1	CHAR		Flag Material for Deletion at Client Level
            O_FLAG2	CHAR		Internal
            O_MESSAGE	CHAR		Variable part of a message
            O_MESSAGE1	CHAR		List processing, contents of selected line
            O_MESSAGE2	CHAR		List processing, contents of selected line
            I_AUFNR	CHAR		Order Number
            I_BUDAT	DATE	SY-DATUM	Posting date
            I_FLAG	CHAR		Flag Material for Deletion at Client Level
            I_LGORT_TO	CHAR		Storage location
            I_LMNGA	BCD		Yield currently to be confirmed
            I_STATION	CHAR		Operation Number
         *************************************************************************/
        public ZRFC_SFC_NSG_0020(string BU) : base(BU)
        {
            SetRFC_NAME("ZRFC_SFC_NSG_0020");
        }
    }
}
