using System;

namespace MESDataObject.Module.Juniper
{
    public class R_SAP_HB
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string WO { get; set; }
        public string PN { get; set; }
        public string USAGE { get; set; }
        public string PARENTPN { get; set; }
        public string CUSTPN { get; set; }
        public string CUSTPARENTPN { get; set; }
        public string CLEI1 { get; set; }
        public string CLEI2 { get; set; }
        public string SPARTDESC { get; set; }
        public string PPARTDESC { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string PNREV { get; set; }
        public string WASTAGE { get; set; }
        public string HBREV { get; set; }
    }
}