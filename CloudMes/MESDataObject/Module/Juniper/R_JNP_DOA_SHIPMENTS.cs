using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MESDataObject.Module.Juniper
{
    public class R_JNP_DOA_SHIPMENTS
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string FILE_NAME { get; set; }
        public string PART_NUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string CARTON_ID { get; set; }
        public string MIST_CLAIM_CODE { get; set; }
        public string ETH_MAC { get; set; }
        public string MIST_PALLET_ID { get; set; }
        public string INVOICE_NO { get; set; }
        public string MFG_DATE { get; set; }
        public string HW_REVISION { get; set; }
        public string PO_NUMBER { get; set; }
        public string PO_LINE_NO { get; set; }
        public double? SHIPPED_QTY { get; set; }
        public string COO { get; set; }
        public string BUILD_SITE { get; set; }
        public string STATUS { get; set; }
        public string ROHS2 { get; set; }
        public string MEANS_OF_TRANSPORT { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string ASNNUMBER { get; set; }
        public double? FILE_FLAG { get; set; }
        public DateTime? FILE_TIME { get; set; }
        public double? SEND_FLAG { get; set; }
        public DateTime? SEND_TIME { get; set; }
    }
}