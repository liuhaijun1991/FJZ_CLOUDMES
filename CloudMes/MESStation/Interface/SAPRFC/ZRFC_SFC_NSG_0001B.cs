using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Interface.SAPRFC
{
    public class ZRFC_SFC_NSG_0001B : SAP_RFC_BASE
    {
        public ZRFC_SFC_NSG_0001B(string BU)
            : base(BU)
        {
            this.SetRFC_NAME("ZRFC_SFC_NSG_0001B");
        }
    }
}
