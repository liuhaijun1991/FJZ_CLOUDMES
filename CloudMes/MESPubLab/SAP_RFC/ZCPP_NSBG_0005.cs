using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0005 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0005(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0005");
        }
        public void SetValue(string ASNNUMBER, string FromLocation, string Plant, string PostDate, string GROUPID, string QUANTITY)
        {
            this.ClearValues();
            SetValue("I_LOCATION", FromLocation);
            SetValue("HEADER_TXT", ASNNUMBER);
            SetValue("PLANT", Plant);
            SetValue("MOVE_TYPE", "251");
            SetValue("PSTNG_DATE", PostDate);
            _Tables["ITAB_IN01"].Rows.Clear();
            DataRow zdr = _Tables["ITAB_IN01"].NewRow();
            zdr["MATERIAL"] = GROUPID;
            zdr["ENTRY_QNT"] = QUANTITY;
            _Tables["ITAB_IN01"].Rows.Add(zdr);

        }
    }
}
