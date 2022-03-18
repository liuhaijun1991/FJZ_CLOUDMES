
using System;

namespace MESDataObject.Module.Juniper
{
    public class R_JUNIPER_SW
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string PARENTPN { get; set; }
        public string MODELPN { get; set; }
        public string SWTYPE { get; set; }
        public string SWVERSION { get; set; }
        public string SWPN { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}