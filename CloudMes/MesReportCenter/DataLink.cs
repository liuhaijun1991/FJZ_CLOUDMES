using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesReportCenter
{
    public class DataLink
    {
        public string key;
        public string LinkType;
    }

    public class DataBaseLink:DataLink
    {
        public string DBType = "";
        public string ConnectString = "";
        public DataBaseLink()
        {
            LinkType = "DataBase";
        }
    }
}
