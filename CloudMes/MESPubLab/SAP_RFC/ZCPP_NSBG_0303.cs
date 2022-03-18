using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    public class ZCPP_NSBG_0303: SAP_RFC_BASE
    {
        public ZCPP_NSBG_0303(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0303");
        }

        public void SetValue(string RLSED, DataTable ZWO_HEADER)
        {
            ClearValues();
            try
            {
                _Tables["ZWO_HEAD"].Clear();
                _Tables["RETURN"].Clear();
            }
            catch { }
            DataRow dr;
            try
            {
                SetValue("RLSED", RLSED);
                //AUFNR ：工單號碼
                //WERKS ：廠別
                //AUART ：工單類型
                //MATNR ： PID
                //GAMNG ： Qty
                //ABLAD ： 客戶 PO 號碼
                //EXPLD ：是否展 BOM （ Y - 需要展 BOM ，N - 不展 BOM）
                foreach (DataRow row in ZWO_HEADER.Rows)
                {
                    dr = _Tables["ZWO_HEAD"].NewRow();
                    dr["AUFNR"] = row["WO"];
                    dr["WERKS"] = row["PLANT"];
                    dr["AUART"] = row["WOTYPE"];
                    dr["MATNR"] = row["PID"];
                    dr["GAMNG"] = row["QTY"];
                    dr["ABLAD"] = row["PO"];
                    dr["GSTRP"] = row["GSTRP"];
                    _Tables["ZWO_HEAD"].Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GET_NEW_ZWO_HEADER()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("WO");
            dt.Columns.Add("PLANT");
            dt.Columns.Add("WOTYPE");
            dt.Columns.Add("PID");
            dt.Columns.Add("QTY");
            dt.Columns.Add("PO");
            dt.Columns.Add("GSTRP");
            return dt;
        }
    }
}
