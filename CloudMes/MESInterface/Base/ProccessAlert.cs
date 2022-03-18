using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.Base
{
    public class ProccessAlert: taskBase
    {
        public override void init()
        {
            Output.UI = new ProccessAlert_UI(this);
        }
        public override void Start()
        {
            
        }
        public override string ToString()
        {
            return "排程监控";
        }

    }
}
