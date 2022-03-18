using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    /// <summary>
    /// FVN teco   
    /// 003 =>TECO
    /// 004 =>反轉
    /// </summary>
    public class ZCPP_NSBG_0045: SAP_RFC_BASE
    {
        public ZCPP_NSBG_0045(string BU)
        : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0045");
        }

        public void SetValues(string WO, bool tecoflag)
        {
            var msgfn = "004";
            if(tecoflag)
                msgfn = "003";
            this.ClearValues();
            _Tables["WOACT"].Clear();
            DataRow dr = _Tables["WOACT"].NewRow();
            dr["AUFNR"] = WO;
            dr["MSGFN"] = msgfn;
            _Tables["WOACT"].Rows.Add(dr);
        }
    }
}
