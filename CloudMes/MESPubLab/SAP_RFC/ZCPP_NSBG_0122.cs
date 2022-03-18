using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0122 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0122(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0122");
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
                    dr["MANDT"] = row["MANDT"];
                    dr["BATNO"] = row["BATNO"];
                    dr["BSTKD"] = row["BSTKD"];
                    dr["MABNR"] = row["MABNR"];
                    dr["KWMENG"] = row["KWMENG"];
                    dr["VRKME"] = row["VRKME"];
                    dr["KBETR"] = row["KBETR"];
                    dr["KONWA"] = row["KONWA"];
                    dr["KPEIN"] = row["KPEIN"];
                    dr["KMEIN"] = row["KMEIN"];
                    dr["ERFDT"] = row["ERFDT"];
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
