using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
   public class ZCMM_NSBG_0013 : SAP_RFC_BASE
    {
        public ZCMM_NSBG_0013(string BU):base(BU)
        {
            SetRFC_NAME("ZCMM_NSBG_0013");
        }
        public void SetValue(string TO, string FromLocation,string ToLocation, string Plant, string PostDate, DataTable dt)
        {
            this.ClearValues();
            //SetValue("DOC_DATE", "00000000");
            SetValue("GMCODE", "04");
            SetValue("HEADER_TXT", TO);
            SetValue("I_LOCATION", ToLocation);
            SetValue("MOVE_TYPE", "311");
            if (ToLocation != null)
            {
                SetValue("O_LOCATION", FromLocation);
            }
            SetValue("PLANT", Plant);
            SetValue("PSTNG_DATE", PostDate);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow zdr = _Tables["IN_TAB"].NewRow();
                zdr["MATNR"] = dt.Rows[i]["GROUPID"];
                zdr["MENGE"] = dt.Rows[i]["QUANTITY"];
                //zdr["RSNUM"] = "0000000000";
                //zdr["RSPOS"] = "0000";
                _Tables["IN_TAB"].Rows.Add(zdr);
            }
        }
    }
}
