using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MESPubLab.SAP_RFC
{
    /// <summary>
    ///ZCPP_NSBG_0129 的摘要说明:Ato開Wo
    /// </summary>
    public class ZCPP_NSBG_0129 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0129(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0129");
        }

        /// <summary>
        /// dt-->第一列為料號,第二列為數量;
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="GroupID"></param>
        /// <param name="orderType">FSJ ORA ORDER TYPE ATO IS ZH12</param>
        /// <param name="Wo"></param>
        /// <param name="Qty"></param>
        /// <param name="So"></param>
        /// <param name="WoStartDate"></param>
        /// <param name="dt"></param>
        public void SetValue(string Plant, string GroupID, string orderType,string Wo, string Qty, string So, string WoStartDate, DataTable dt)
        {
            this.ClearValues();
            SetValue("PLANT", Plant);
            SetValue("RLSED", "X");
            _Tables["ZWO_HEAD"].Clear();
            DataRow dr = _Tables["ZWO_HEAD"].NewRow();
            dr["WERKS"] = Plant;
            dr["MATNR"] = GroupID;
            dr["AUART"] = orderType;
            dr["AUFNR"] = Wo;
            dr["GAMNG"] = Qty;
            dr["ABLAD"] = So;
            dr["GSTRP"] = WoStartDate;
            _Tables["ZWO_HEAD"].Rows.Add(dr);
            _Tables["ZWO_ITEM"].Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow zdr = _Tables["ZWO_ITEM"].NewRow();
                zdr["AUFNR"] = Wo;
                zdr["IDNRK"] = dt.Rows[i][0].ToString();
                zdr["BDMNG"] = dt.Rows[i][1].ToString();
                _Tables["ZWO_ITEM"].Rows.Add(zdr);
            }
        }

        /// <summary>
        /// dt-->第一列為料號,第二列為數量;
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="GroupID"></param>
        /// <param name="orderType">FSJ ORA ORDER TYPE ATO IS ZH12</param>
        /// <param name="Wo"></param>
        /// <param name="Qty"></param>
        /// <param name="So"></param>
        /// <param name="WoStartDate"></param>
        /// <param name="dt"></param>
        public void SetValue(string Plant, string GroupID, string orderType, string Wo, string Qty, string So, string WoStartDate, List<object> dt)
        {
            this.ClearValues();
            SetValue("PLANT", Plant);
            SetValue("RLSED", "X");
            _Tables["ZWO_HEAD"].Clear();
            DataRow dr = _Tables["ZWO_HEAD"].NewRow();
            dr["WERKS"] = Plant;
            dr["MATNR"] = GroupID;
            dr["AUART"] = orderType;
            dr["AUFNR"] = Wo;
            dr["GAMNG"] = Qty;
            dr["ABLAD"] = So;
            dr["GSTRP"] = WoStartDate;
            _Tables["ZWO_HEAD"].Rows.Add(dr);
            _Tables["ZWO_ITEM"].Clear();
            foreach (var item in dt)
            {
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(item);                
                DataRow zdr = _Tables["ZWO_ITEM"].NewRow();
                zdr["AUFNR"] = Wo;
                zdr["IDNRK"] = pdc.Find("PARTNO", true).GetValue(item).ToString(); 
                zdr["BDMNG"] = pdc.Find("TOTREQUESTQTY", true).GetValue(item).ToString();
                _Tables["ZWO_ITEM"].Rows.Add(zdr);
            }
        }
    }
}