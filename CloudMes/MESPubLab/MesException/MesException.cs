using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MesException
{
    public class MesException : ApplicationException
    {
        public MesException(string message) : base(message)
        {
        }
    }
}
