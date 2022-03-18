using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{

    /*
    CONSIGN	CHAR		"X" Indicator for including consign stock
	PLANT	CHAR	MBGA	Plant
	IN_LOC	TABLE	TABLES PARAMETER IN_LOC=TABLE 	Material Stock
	IN_MAT	TABLE	TABLES PARAMETER IN_MAT=TABLE 	Material list
	OUT_STOCK	TABLE	TABLES PARAMETER OUT_STOCK=TABLE  
    */

    /// <summary>
    /// 查SAP庫存
    /// </summary>
    public class ZCMM_NSBG_0025 : SAP_RFC_BASE
    {
        public ZCMM_NSBG_0025(string BU) : base(BU)
        {
            SetRFC_NAME("ZCMM_NSBG_0025");
        }

        public void SetValue( List<string> LOCATION = null)
        {
            this._Tables["IN_LOC"].Clear();
            this._Tables["IN_MAT"].Clear();
            this._Tables["OUT_STOCK"].Clear();
            if (LOCATION != null)
            {
                for (int i = 0; i < LOCATION.Count; i++)
                {

                }

            }
        }
    }
}
