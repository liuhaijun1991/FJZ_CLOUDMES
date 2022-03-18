using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0005NE: SAP_RFC_BASE
    {
        public ZCPP_NSBG_0005NE(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0005NE");
        }
        public void SetValue(string Plant, string GroupID, string orderType, string Wo, string Qty, string So, string WoStartDate, DataTable dt)
        {
            this.ClearValues();
            SetValue("PLANT", Plant);
            SetValue("NOCREATE", "X");
            SetValue("NOCHKWO", "X");
            SetValue("RLSED", "");
            // _Tables["ZWO_HEAD"].Clear();
            DataRow dr = _Tables["ZWO_HEADER"].NewRow();
            dr["WERKS"] = Plant;
            dr["MATNR"] = GroupID;
            dr["AUART"] = orderType;
            dr["AUFNR"] = Wo;
            dr["GAMNG"] = Qty;
            dr["ABLAD"] = So;
            dr["GSTRP"] = WoStartDate;
            _Tables["ZWO_HEADER"].Rows.Add(dr);
            _Tables["ZWO_ITEM"].Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow zdr = _Tables["ZWO_ITEM"].NewRow();
                zdr["SEQNO"] = dt.Rows.Count;
                zdr["AUFNR"] = Wo;
                zdr["IDNRK"] = dt.Rows[i]["partno"].ToString();
                zdr["MENGE"] = dt.Rows[i]["RequestQTY"].ToString();                
                _Tables["ZWO_ITEM"].Rows.Add(zdr);
            }
        }

    }
}
