using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.SAP_RFC
{
    public class ZCMM_NSBG_0051 : SAP_RFC_BASE
    {
        public ZCMM_NSBG_0051(string BU) : base(BU)
        {
            SetRFC_NAME("ZCMM_NSBG_0051");
        }
        public void SetValue(string plant, string skuno, string from, string to, string qty, string headerText, DateTime time, string moveType)
        {
            ClearValues();
            SetValue("HEADER_TXT", headerText);
            SetValue("I_LOCATION", to);
            SetValue("O_LOCATION", from);
            SetValue("PLANT", plant);
            SetValue("PSTNG_DATE", time.ToString());
            SetValue("MOVE_TYPE", moveType);
            _Tables["IN_TAB"].Clear();
            DataRow dr = _Tables["IN_TAB"].NewRow();
            dr[1] = skuno;
            dr[5] = qty;
            _Tables["IN_TAB"].Rows.Add(dr);
            try
            {
                _Tables["OUT_TAB"].Clear();
            }
            catch
            {
                
            }

        }
    }
}
