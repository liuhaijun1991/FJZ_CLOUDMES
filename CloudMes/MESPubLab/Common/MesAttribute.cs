using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.Common
{
    public class PropertiesDesc : System.Attribute
    {
        public string desc;
        public PropertiesDesc(string description)
        {
            desc = description;
        }
    }

    public class ExcelIgone : System.Attribute
    {
        public string desc;
        public ExcelIgone()
        {
        }
    }
}
