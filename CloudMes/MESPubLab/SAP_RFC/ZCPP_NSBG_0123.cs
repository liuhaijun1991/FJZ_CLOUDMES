using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0123 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0123(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0123");
        }
        public void SetValue(DataTable in_table)
        {
            ClearValues();
            try
            {
                _Tables["OUT_TAB"].Clear();
                _Tables["IN_TAB"].Clear();
            }
            catch { }
            DataRow dr;
            try
            {
                foreach (DataRow row in in_table.Rows)
                {                   
                    dr = _Tables["IN_TAB"].NewRow();
                    dr["WERKS"] = row["WERKS"];
                    dr["AUART"] = row["AUART"];
                    dr["MATNR"] = row["MATNR"];
                    dr["GAMNG"] = row["GAMNG"];
                    dr["GSTRP"] = row["GSTRP"];
                    dr["GLTRP"] = row["GLTRP"];
                    dr["ABLAD"] = row["ABLAD"];
                    _Tables["IN_TAB"].Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
