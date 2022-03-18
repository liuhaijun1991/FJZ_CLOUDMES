using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZRFC_SFC_NSG_0001HW : SAP_RFC_BASE
    {
        /*
         ZRFC_SFC_NSG_0001HW
         AUFNRF  CHAR    Order Number
         AUFNRT  CHAR    Order Number
         COUNT  INT4  14  Natural number
         CUST  CHAR  'Cisco'  External material group
         FLAG  CHAR    Flag = 'X',down Phant component ver.
         PLANT  CHAR  'ACEA'  Plant
         RLDATE  DATE    Creation date of the change document
         SCHEDULED_DATE  DATE  SY-DATUM  Date and time, current (application server) date
         WO_HEADER	TABLE		SFC <=> SAP_PO  Output Table
         WO_ITEM	TABLE		PO detail       Output Table
         WO_TEXT	TABLE		                Output Table
        */
        /// <summary>
        /// 獲取指定工單的SAP需求等信息
        /// </summary>
        public ZRFC_SFC_NSG_0001HW(string BU) : base(BU)
        {
            SetRFC_NAME("ZRFC_SFC_NSG_0001HW");
        }

        public void SetValue(string AUFNRF,string AUFNRT,string COUNT,string CUST,string FLAG,string PLANT,string RLDATE,string SCHEDULED_DATE)
        {
            this.ClearValues();
            this.SetValue("AUFNRF", AUFNRF);
            this.SetValue("AUFNRT", AUFNRT);
            this.SetValue("COUNT", COUNT);
            this.SetValue("CUST", CUST);
            this.SetValue("FLAG", FLAG);
            this.SetValue("PLANT", PLANT);
            this.SetValue("RLDATE", RLDATE);
            this.SetValue("SCHEDULED_DATE", SCHEDULED_DATE);
        }
    }
}
