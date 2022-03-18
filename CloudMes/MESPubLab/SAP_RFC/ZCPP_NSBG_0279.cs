using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0279 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0279(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0279");
        }
        public void PreUploadSetValueATO(string Plant, string GroupID, string orderType, string Wo, string Qty, string So, string WoStartDate, DataTable dt)
        {
            this.ClearValues();
            SetValue("PLANT", Plant);
            SetValue("NOCREATE", "X");
            SetValue("NOCHKWO", "");
            SetValue("RLSED", "");
            _Tables["ZWO_HEADER"].Clear();
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
                zdr["IDNRK"] = dt.Rows[i]["partno"].ToString().Trim() + "-A";
                zdr["MENGE"] = dt.Rows[i]["RequestQTY"].ToString();
                _Tables["ZWO_ITEM"].Rows.Add(zdr);
            }
        }


        public void PreUploadSetValuePTO(string Plant, string GroupID, string orderType, string Wo, string Qty, string So, string WoStartDate)
        {
            this.ClearValues();
            SetValue("PLANT", Plant);
            SetValue("NOCREATE", "X");
            SetValue("NOCHKWO", "");
            SetValue("RLSED", "");
            _Tables["ZWO_HEADER"].Clear();
            DataRow dr = _Tables["ZWO_HEADER"].NewRow();
            dr["WERKS"] = Plant;
            dr["MATNR"] = GroupID;
            dr["AUART"] = orderType;
            dr["AUFNR"] = Wo;
            dr["GAMNG"] = Qty;
            dr["ABLAD"] = So;
            dr["GSTRP"] = WoStartDate;
            _Tables["ZWO_HEADER"].Rows.Add(dr);
            
        }

        public void UploadSetValueATO(string Plant, string GroupID, string orderType, string Wo, string Qty, string So, string WoStartDate, DataTable dt)
        {
            this.ClearValues();
            SetValue("PLANT", Plant);
            SetValue("NOCREATE", "");
            SetValue("NOCHKWO", "");
            SetValue("RLSED", "X");
            _Tables["ZWO_HEADER"].Clear();
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
                zdr["IDNRK"] = dt.Rows[i]["partno"].ToString() + "-A";
                zdr["MENGE"] = dt.Rows[i]["RequestQTY"].ToString();
                _Tables["ZWO_ITEM"].Rows.Add(zdr);
            }
        }

        public void UploadSetValuePTO(string Plant, string GroupID, string orderType, string Wo, string Qty, string So, string WoStartDate)
        {
            this.ClearValues();
            SetValue("PLANT", Plant);
            SetValue("NOCREATE", "");
            SetValue("NOCHKWO", "");
            SetValue("RLSED", "X");
            _Tables["ZWO_HEADER"].Clear();
            DataRow dr = _Tables["ZWO_HEADER"].NewRow();
            dr["WERKS"] = Plant;
            dr["MATNR"] = GroupID;
            dr["AUART"] = orderType;
            dr["AUFNR"] = Wo;
            dr["GAMNG"] = Qty;
            dr["ABLAD"] = So;
            dr["GSTRP"] = WoStartDate;
            _Tables["ZWO_HEADER"].Rows.Add(dr);

        }
    }
}
