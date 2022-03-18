using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC 
{
    class ZCPP_NSBG_0038 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0038(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0038");
        }

        /// <summary>
        /// ADD BY Winster-温欢,FJZ UNPACK RFC FOR JUNIPER</summary>
        /// <param name="WERKS">SAP Plant Code厂別</param>
        /// <param name="CARTON">MES carton ID卡通號</param>
        /// <param name="AUFNR">WO工單</param>
        /// <param name="MATNR">group ID</param>
        /// <param name="PKDATE">PKDATE: carton 打包日期</param>
        /// <param name="PKTIME">PKTIME: carton 打包時間</param>
        /// <param name="LMNGA">WO 數量</param>
        /// <param name="ISM02">工時</param>
        /// <param name="ABLAD">客人PO</param>
        public void SetValue(string WERKS, string CARTON, string AUFNR, string MATNR, string PKDATE, string PKTIME,string LMNGA, string ISM02, string ABLAD)
        {
            this.ClearValues();
            this.SetValue("WERKS", WERKS);
            this.SetValue("CARTON", CARTON);
            this.SetValue("AUFNR", AUFNR);
            this.SetValue("MATNR", MATNR);
            this.SetValue("PKDATE", PKDATE);
            this.SetValue("PKTIME", PKTIME);
            this.SetValue("LMNGA", LMNGA);
            this.SetValue("ISM02", ISM02);
            this.SetValue("ABLAD", ABLAD);
        }

    }
}
