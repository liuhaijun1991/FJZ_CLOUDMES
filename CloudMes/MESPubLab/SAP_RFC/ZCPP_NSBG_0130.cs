using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    /// <summary>
    ///ZCPP_NSBG_0130 的摘要说明 :Pto開Wo
    /// </summary>
    public class ZCPP_NSBG_0130 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0130(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0130");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="PartNo"></param>
        /// <param name="orderType">FSJ ORA ORDER TYPE PTO IS ZH13</param>
        /// <param name="Wo"></param>
        /// <param name="Qty"></param>
        /// <param name="Po"></param>
        /// <param name="WoStartDate"></param>
        public void SetValue(string Plant, string PartNo, string orderType,string Wo, string Qty, string Po, string WoStartDate)
        {
            this.ClearValues();
            SetValue("RLSED", "X");
            _Tables["ZWO_HEAD"].Clear();
            DataRow dr = _Tables["ZWO_HEAD"].NewRow();
            dr["WERKS"] = Plant;
            dr["MATNR"] = PartNo;
            dr["AUART"] = orderType;
            dr["AUFNR"] = Wo;
            dr["GAMNG"] = Qty;
            dr["ABLAD"] = Po;
            dr["GSTRP"] = WoStartDate;
            _Tables["ZWO_HEAD"].Rows.Add(dr);
        }
    }
}