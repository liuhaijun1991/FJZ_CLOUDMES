using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class R_DASHBOARDTRACKLIST
    {
        public string TRACK { get; set; }
        public string TRACKTYPE { get; set; }
        public double? SEQ { get; set; }
        public string PROCESS { get; set; }
        public string BU { get; set; }
        public string MEASURE { get; set; }
        public string DATASOURCE { get; set; }
        public string REPORT { get; set; }
        public string OWNER { get; set; }
        public string GOAL { get; set; }
        public string LINKPAGE { get; set; }
        public string PARAMS { get; set; }
        public string REALTIMESQL { get; set; }
    }
}

