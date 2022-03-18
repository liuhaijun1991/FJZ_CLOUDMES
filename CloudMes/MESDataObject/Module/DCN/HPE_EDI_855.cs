using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module.DCN
{
    public class T_HPE_EDI_855 : DataObjectTable
    {
        public T_HPE_EDI_855(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_HPE_EDI_855(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_HPE_EDI_855);
            TableName = "HPE_EDI_855".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_HPE_EDI_855 : DataObjectBase
    {
        public Row_HPE_EDI_855(DataObjectInfo info) : base(info)
        {

        }
        public HPE_EDI_855 GetDataObject()
        {
            HPE_EDI_855 DataObject = new HPE_EDI_855();
            DataObject.ID = this.ID;
            DataObject.F_SFC_ID = this.F_SFC_ID;
            DataObject.F_850_ID = this.F_850_ID;
            DataObject.F_PO = this.F_PO;
            DataObject.F_DATE = this.F_DATE;
            DataObject.F_LINE = this.F_LINE;
            DataObject.F_MPN = this.F_MPN;
            DataObject.F_CPN = this.F_CPN;
            DataObject.F_MPN_DESC = this.F_MPN_DESC;
            DataObject.F_LINE_PRICE = this.F_LINE_PRICE;
            DataObject.F_LINE_QTY = this.F_LINE_QTY;
            DataObject.F_REASON_CODE = this.F_REASON_CODE;
            DataObject.F_ACK_EDD = this.F_ACK_EDD;
            DataObject.F_ACK_ESD = this.F_ACK_ESD;
            DataObject.F_ACK_QTY = this.F_ACK_QTY;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string F_SFC_ID
        {
            get
            {
                return (string)this["F_SFC_ID"];
            }
            set
            {
                this["F_SFC_ID"] = value;
            }
        }
        public string F_850_ID
        {
            get
            {
                return (string)this["F_850_ID"];
            }
            set
            {
                this["F_850_ID"] = value;
            }
        }
        public string F_PO
        {
            get
            {
                return (string)this["F_PO"];
            }
            set
            {
                this["F_PO"] = value;
            }
        }
        public DateTime? F_DATE
        {
            get
            {
                return (DateTime?)this["F_DATE"];
            }
            set
            {
                this["F_DATE"] = value;
            }
        }
        public string F_LINE
        {
            get
            {
                return (string)this["F_LINE"];
            }
            set
            {
                this["F_LINE"] = value;
            }
        }
        public string F_MPN
        {
            get
            {
                return (string)this["F_MPN"];
            }
            set
            {
                this["F_MPN"] = value;
            }
        }
        public string F_CPN
        {
            get
            {
                return (string)this["F_CPN"];
            }
            set
            {
                this["F_CPN"] = value;
            }
        }
        public string F_MPN_DESC
        {
            get
            {
                return (string)this["F_MPN_DESC"];
            }
            set
            {
                this["F_MPN_DESC"] = value;
            }
        }
        public Double F_LINE_PRICE
        {
            get
            {
                return (Double)this["F_LINE_PRICE"];
            }
            set
            {
                this["F_LINE_PRICE"] = value;
            }
        }
        public string F_LINE_QTY
        {
            get
            {
                return (string)this["F_LINE_QTY"];
            }
            set
            {
                this["F_LINE_QTY"] = value;
            }
        }
        public string F_REASON_CODE
        {
            get
            {
                return (string)this["F_REASON_CODE"];
            }
            set
            {
                this["F_REASON_CODE"] = value;
            }
        }
        public DateTime? F_ACK_ESD
        {
            get
            {
                return (DateTime?)this["F_ACK_ESD"];
            }
            set
            {
                this["F_ACK_ESD"] = value;
            }
        }
        public DateTime? F_ACK_EDD
        {
            get
            {
                return (DateTime?)this["F_ACK_ESD"];
            }
            set
            {
                this["F_ACK_ESD"] = value;
            }
        }
        public string F_ACK_QTY
        {
            get
            {
                return (string)this["F_ACK_QTY"];
            }
            set
            {
                this["F_ACK_QTY"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class HPE_EDI_855
    {
        public string ID { get; set; }
        public string F_SFC_ID { get; set; }
        public string F_850_ID { get; set; }
        public string F_PO { get; set; }
        public DateTime? F_DATE { get; set; }
        public string F_LINE { get; set; }
        public string F_MPN { get; set; }
        public string F_CPN { get; set; }
        public string F_MPN_DESC { get; set; }
        public Double F_LINE_PRICE { get; set; }
        public string F_LINE_QTY { get; set; }
        public string F_REASON_CODE { get; set; }
        public DateTime? F_ACK_ESD { get; set; }
        public DateTime? F_ACK_EDD { get; set; }
        public string F_ACK_QTY { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }

    [SqlSugar.SugarTable("hpe.TB_EDI855")]
    public class B2B_HPE_EDI_855
    {
        public string F_ID { get; set; }
        public string F_SFC_ID { get; set; }
        public string F_850_ID { get; set; }
        public string F_PO { get; set; }
        public DateTime? F_DATE { get; set; }
        public string F_LINE { get; set; }
        public string F_MPN { get; set; }
        public string F_CPN { get; set; }
        public string F_MPN_DESC { get; set; }
        public Double F_LINE_PRICE { get; set; }
        public string F_LINE_QTY { get; set; }
        public string F_REASON_CODE { get; set; }
        public string F_ACK_ESD { get; set; }
        public DateTime? F_ACK_EDD { get; set; }
        public DateTime? F_ACK_QTY { get; set; }
        public DateTime F_SFC_DT { get; set; }
        public DateTime F_LASTEDIT_DT { get; set; }
        public string F_FILENAME { get; set; }
    }
}