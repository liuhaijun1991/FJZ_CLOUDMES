using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.ALLPART
{
    [SqlSugar.SugarTable("MES4.R_TR_SN")]
    public class R_TR_SN
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string TR_SN { get; set; }
        public string DOC_NO { get; set; }
        public string DOC_FLAG { get; set; }
        public string CUST_KP_NO { get; set; }
        public string MFR_KP_NO { get; set; }
        public string MFR_CODE { get; set; }
        public string DATE_CODE { get; set; }
        public string LOT_CODE { get; set; }
        public int QTY { get; set; }
        public int EXT_QTY { get; set; }
        public string LOCATION_FLAG { get; set; }
        public string WORK_FLAG { get; set; }
        public DateTime? START_TIME { get; set; }
        public DateTime? END_TIME { get; set; }
        public string EMP_NO { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string KP_TYPE { get; set; }
        public string TR_SN_DESC { get; set; }
        public string LTB_FLAG { get; set; }
        public string KP_ROHS_STATUS { get; set; }
        public string KITTING_FLAG { get; set; }
        public string KITTING_WO { get; set; }
        public string SAP_STOCK { get; set; }
        public string FLOOR { get; set; }
        public string ACTUAL_KP_NO { get; set; }
        public string WAFER_NO { get; set; }
        public string STANDARD_DC { get; set; }
        public string OVER_TIME { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public string DATA6 { get; set; }
        public double? LCR_VALUE { get; set; }
        public string KP_VERSION { get; set; }
        public string AUTO_FLAG { get; set; }
        public string RECEIVE_TIMES { get; set; }
    }
}
