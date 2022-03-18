using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESNCO
{
    public class MESAPIData
    {
        public string Token { get; set; }
        public string Class { get; set; }
        public string Function { get; set; }
        public object Data { get; set; }
        public string MessageID { get; set; }
        public string ClientID { get; set; }
    }
}
