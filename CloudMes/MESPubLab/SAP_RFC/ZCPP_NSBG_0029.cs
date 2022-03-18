using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.SAP_RFC
{
    class ZCPP_NSBG_0029 : SAP_RFC_BASE
    {
        public ZCPP_NSBG_0029(string BU) : base(BU)
        {
            SetRFC_NAME("ZCPP_NSBG_0029");
        }
        /// <summary>
        /// ADD BY Winster-温欢,FJZ UNPACK RFC FOR JUNIPER
        /// </summary>
        /// <param name="OLD_WO">原本要UNPACK的工单</param>
        /// <param name="OLD_CTN">要unpack 的 Carton ID</param>
        /// <param name="OLD_PACK_QTY">要unpack 的 Carton QTY</param>
        /// <param name="NEW_WO">新的PACK工单</param>
        /// <param name="NEW_CTN">重新打包的Carton ID</param>
        /// <param name="NEW_PACK_QTY">重新打包的Carton的數量</param>
        public void SetValue(string OLD_WO, string OLD_CTN, string OLD_PACK_QTY, string NEW_WO, string NEW_CTN, string NEW_PACK_QTY)
        {
            this.ClearValues();
            this.SetValue("OLD_WO", OLD_WO);
            this.SetValue("OLD_CTN", OLD_CTN);
            this.SetValue("OLD_PACK_QTY", OLD_PACK_QTY);
            this.SetValue("NEW_WO", NEW_WO);
            this.SetValue("NEW_CTN", NEW_CTN);
            this.SetValue("NEW_PACK_QTY", NEW_PACK_QTY);
        }
    }
}