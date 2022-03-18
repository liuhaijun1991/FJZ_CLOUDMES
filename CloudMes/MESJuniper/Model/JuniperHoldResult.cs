using MESDataObject.Module.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESJuniper.Model
{
    public class JuniperHoldResult
    {
        public bool HoldFlag;
        public string HoldReason;
        public ENUM_O_ORDER_HOLD_CONTROLTYPE ControlType;
    }
}
