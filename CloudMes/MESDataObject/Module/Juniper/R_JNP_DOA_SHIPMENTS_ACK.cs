using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.Juniper
{
    public class R_JNP_DOA_SHIPMENTS_ACK
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string ASNNUMBER { get; set; }
        public string FILE_PO { get; set; }
        public string FILE_PO_LINE { get; set; }
        public double? FILE_QTY { get; set; }
        
        [SqlSugar.SugarColumn(ColumnName ="MODEL_NAME")]
        public string MODEL { get; set; }
        public string SERIAL { get; set; }
        public string DELIVERYNUMBER { get; set; }
        public string DNLINE { get; set; }
        public string EQUIPMENT { get; set; }
        public string USER_STATUS { get; set; }
        public string IB_DELIVERY { get; set; }
        public string MESSAGE_CODE { get; set; }
        public string MESSAGE_TEXT { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }

    }
}
