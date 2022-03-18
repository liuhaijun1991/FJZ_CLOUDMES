using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MESPubLab.SAP_RFC
{
    /// <summary>
    /// 提供查詢當前在制工單的拋帳情況
    /// </summary>
    public class ZRFC_SFC_NSG_0023T : SAP_RFC_BASE
    {

        /*
         	CUST	CHAR	'ALL'	External material group
	        PLANT	CHAR	'ACEA'	plant
	        ZSFC23C	TABLE		Catch the operation of order
         */
        /// <summary>
        /// 
        /// </summary>
        public ZRFC_SFC_NSG_0023T(string BuName) : base(BuName)
        {
            SetRFC_NAME("ZRFC_SFC_NSG_0023T");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlantCode"></param>
        public void SetRFCValue(string PlantCode,string CUST="ALL")
        {
            this.ClearValues();
            this.SetValue("CUST", CUST);
            this.SetValue("PLANT", PlantCode);
        }

        public DataTable RETTABLE
        {
            get
            {
                return this.GetTableValue("ZSFC23C");
            }
        }
    }
}
