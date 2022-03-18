using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    /*
     	O_FLAG	CHAR		
	O_MESSAGE	CHAR		
	O_VBELN	CHAR		
	  P_LGORT	CHAR		
	  P_POSNR	NUM		
	  P_QTY	BCD		
	  P_VBELN	CHAR		
	P_VSTEL	CHAR		
	  P_WADAT	DATE		
         */
    public class ZRFC_NSG_SD_0005B : SAP_RFC_BASE
    {
        public string O_FLAG
        {
            get
            {
                return GetValue("O_FLAG");
            }
        }
        public string O_MESSAGE
        {
            get
            {
                return GetValue("O_MESSAGE");
            }
        }
        public string O_VBELN
        {
            get
            {
                return GetValue("O_VBELN");
            }
        }
        public ZRFC_NSG_SD_0005B(string BU) : base(BU)
        {
            SetRFC_NAME("ZRFC_NSG_SD_0005B");
        }
        public void SetValue(string SHIPPOINT, string SONO, string SOITEN_SEQ, int QTY, string LOCATION, DateTime Date)
        {
            ClearValues();
            SetValue("P_VSTEL", SHIPPOINT);
            SetValue("P_VBELN", SONO);
            SetValue("P_POSNR", SOITEN_SEQ);
            SetValue("P_QTY", QTY.ToString());
            SetValue("P_LGORT", LOCATION);
            SetValue("P_WADAT", Date.ToString("yyyy-MM-dd"));
            
        }
    }
}
