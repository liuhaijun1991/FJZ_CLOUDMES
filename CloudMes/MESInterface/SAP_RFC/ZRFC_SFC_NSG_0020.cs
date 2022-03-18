using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.SAP_RFC
{
    /// <summary>
    /// Use for wo to MRB BackFlush
    /// </summary>
    public class ZRFC_SFC_NSG_0020:SAP_RFC_BASE
    {
        public ZRFC_SFC_NSG_0020(string BU) : base(BU)
        {
            SetRFC_NAME("ZRFC_SFC_NSG_0020");
        }
    }
}
