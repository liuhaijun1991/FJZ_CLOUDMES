using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module
{
    public class R_DASHBOARDTRACKHISDATA
    {
        public string TRACKDATE { get; set; }
        public string TRACK { get; set; }
        public string TRACKTYPE { get; set; }
        public string BU { get; set; }
        public string DATA { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}
