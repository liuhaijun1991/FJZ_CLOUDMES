using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.ProccessAlert
{
    public class AlertTypeBase
    {
        public virtual string NAME { get; }
        public virtual void Run()
        { }
    }
}
