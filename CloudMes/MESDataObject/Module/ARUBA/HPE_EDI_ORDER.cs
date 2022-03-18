using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.DCN.ARUBA
{
    public class HPE_EDI_ORDER
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string F_ID { get; set; }
        public string F_SITE { get; set; }
        public string F_PO { get; set; }
        public string F_PO_TYPE { get; set; }
        public DateTime F_PO_DATE { get; set; }
        public string F_PO_COMMENT { get; set; }
        public string F_COMPANYCODE { get; set; }
        public string F_INCO_TERM { get; set; }
        public string F_N1_ST { get; set; }
        public string F_N1_DA { get; set; }
        public string F_N1_BT { get; set; }
        public string F_N1_PD { get; set; }
        public string F_LINE { get; set; }
        public string F_LINE_QTY { get; set; }
        public Double F_LINE_PRICE { get; set; }
        public string F_PN { get; set; }
        public string F_PN_DESC { get; set; }
        public string PO_SCH_LINE { get; set; }
        public string F_SCH_QTY { get; set; }
        public DateTime F_SCH_DR_DATE { get; set; }
        public DateTime F_LASTEDIT_DT { get; set; }
        public string F_FILENAME { get; set; }
        public DateTime EDIT_TIME { get; set; }
        public string FLAG { get; set; }
        public string F_SHIP_MODE { get; set; }
        public string F_CARRIER { get; set; }
        public double F_LINE_LEFT_QTY { get; set; }
        public DateTime? F_SHIP_DATE { get; set; }
        public string F_PIP_TYPE { get; set; }
        public string F_PC_CODE { get; set; }
        public string F_PO_VER { get; set; }
        public string SOURCEID { get; set; }
        public string EDITYPE { get; set; }

    }
}