using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.Juniper
{
    public class R_GEOGRAPHIES_MAP
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string COUNTRYCODE { get; set; }
        public string COUNTRYNAME { get; set; }
        public string REGION1 { get; set; }
        public string REGION2 { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}
