using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.SAP_RFC
{
    /*
      O_ERROR	CHAR		Variable part of a message
     O_ERRORMESSAGE	CHAR		Variable part of a message
     O_FLAG	CHAR		Flag Material for Deletion at Client Level
     O_FLAG1	CHAR		Flag Material for Deletion at Client Level
     O_MESSAGE	CHAR		Variable part of a message
     O_MESSAGE1	CHAR		List processing, contents of selected line
     I_AUFNR	CHAR		Order Number
     I_BUDAT	DATE	SY-DATUM	Operation Number
     I_LMNGA	BCD		Yield currently to be confirmed
     I_STATION	CHAR		Operation Number
      */
  public  class ZRFC_SFC_NSG_0022 : SAP_RFC_BASE
    {
        /// <summary>
        /// Rework Call RFC
        /// </summary>
        public ZRFC_SFC_NSG_0022(string BuName): base(BuName)
        {
            SetRFC_NAME("ZRFC_SFC_NSG_0022");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="POSTDATE"></param>
        /// <param name="QTY"></param>
        /// <param name="SAP_STATION_CODE"></param>
        public void SetValue(string WO, string POSTDATE,
            string QTY, string SAP_STATION_CODE)
        {
            ClearValues();

            SetValue("I_AUFNR", WO);
            SetValue("I_BUDAT", POSTDATE);
            //SetValue("I_FLAG", confirmed_flag);
            //SetValue("I_LGORT_TO", storge);
            SetValue("I_LMNGA", QTY);
            SetValue("I_STATION", SAP_STATION_CODE);

        }


    }
}
