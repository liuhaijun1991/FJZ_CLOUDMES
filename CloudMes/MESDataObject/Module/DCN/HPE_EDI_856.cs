using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_HPE_EDI_856 : DataObjectTable
    {
        public T_HPE_EDI_856(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_HPE_EDI_856(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_HPE_EDI_856);
            TableName = "HPE_EDI_856".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_HPE_EDI_856 : DataObjectBase
    {
        public Row_HPE_EDI_856(DataObjectInfo info) : base(info)
        {

        }
        public HPE_EDI_856 GetDataObject()
        {
            HPE_EDI_856 DataObject = new HPE_EDI_856();
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.F_FILENAME = this.F_FILENAME;
            DataObject.F_TO_NO = this.F_TO_NO;
            DataObject.F_TO_DATE = this.F_TO_DATE;
            DataObject.F_TO_SHIPDATE = this.F_TO_SHIPDATE;
            DataObject.F_TO_DN = this.F_TO_DN;
            DataObject.F_TO_DN_LINE = this.F_TO_DN_LINE;
            DataObject.F_TO_PKG_QTY = this.F_TO_PKG_QTY;
            DataObject.F_TO_NETWEIGHT = this.F_TO_NETWEIGHT;
            DataObject.F_TO_GROSSWEIGHT = this.F_TO_GROSSWEIGHT;
            DataObject.F_TO_VOLUME = this.F_TO_VOLUME;
            DataObject.F_TO_TRAILERNO = this.F_TO_TRAILERNO;
            DataObject.F_CARRIER_TYPE = this.F_CARRIER_TYPE;
            DataObject.F_CARRIER_CODE = this.F_CARRIER_CODE;
            DataObject.F_CARRIER_TRAN_TYPE = this.F_CARRIER_TRAN_TYPE;
            DataObject.F_CARRIER_REF_NO = this.F_CARRIER_REF_NO;
            DataObject.F_CARRIER_TRAILER_NO = this.F_CARRIER_TRAILER_NO;
            DataObject.F_ST_NAME = this.F_ST_NAME;
            DataObject.F_ST_CONTACT = this.F_ST_CONTACT;
            DataObject.F_ST_CONTACT_MAIL = this.F_ST_CONTACT_MAIL;
            DataObject.F_ST_CUSTOMERCODE = this.F_ST_CUSTOMERCODE;
            DataObject.F_ST_ADDRESS = this.F_ST_ADDRESS;
            DataObject.F_ST_CITY = this.F_ST_CITY;
            DataObject.F_ST_POSTCODE = this.F_ST_POSTCODE;
            DataObject.F_ST_STATE_CODE = this.F_ST_STATE_CODE;
            DataObject.F_ST_COUNTRY_CODE = this.F_ST_COUNTRY_CODE;
            DataObject.F_PO_NO = this.F_PO_NO;
            DataObject.F_PO_LINE_NO = this.F_PO_LINE_NO;
            DataObject.F_PO_LINE_QTY = this.F_PO_LINE_QTY;
            DataObject.F_PO_DATE = this.F_PO_DATE;
            DataObject.F_PKG_ID = this.F_PKG_ID;
            DataObject.F_PKG_QTY = this.F_PKG_QTY;
            DataObject.F_PKG_GROSS_WEIGHT = this.F_PKG_GROSS_WEIGHT;
            DataObject.F_PKG_DIMENSION = this.F_PKG_DIMENSION;
            DataObject.F_ITEM_MPN = this.F_ITEM_MPN;
            DataObject.F_ITEM_CPN = this.F_ITEM_CPN;
            DataObject.F_ITEM_SN = this.F_ITEM_SN;
            DataObject.F_ITEM_COO = this.F_ITEM_COO;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
            DataObject.F_INCOTERM = this.F_INCOTERM;
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
        public double? F_ID
        {
            get
            {
                return (double?)this["F_ID"];
            }
            set
            {
                this["F_ID"] = value;
            }
        }
        public string F_FILENAME
        {
            get
            {
                return (string)this["F_FILENAME"];
            }
            set
            {
                this["F_FILENAME"] = value;
            }
        }
        public string F_TO_NO
        {
            get
            {
                return (string)this["F_TO_NO"];
            }
            set
            {
                this["F_TO_NO"] = value;
            }
        }
        public DateTime? F_TO_DATE
        {
            get
            {
                return (DateTime?)this["F_TO_DATE"];
            }
            set
            {
                this["F_TO_DATE"] = value;
            }
        }
        public DateTime? F_TO_SHIPDATE
        {
            get
            {
                return (DateTime?)this["F_TO_SHIPDATE"];
            }
            set
            {
                this["F_TO_SHIPDATE"] = value;
            }
        }
        public string F_TO_DN
        {
            get
            {
                return (string)this["F_TO_DN"];
            }
            set
            {
                this["F_TO_DN"] = value;
            }
        }
        public string F_TO_DN_LINE
        {
            get
            {
                return (string)this["F_TO_DN_LINE"];
            }
            set
            {
                this["F_TO_DN_LINE"] = value;
            }
        }
        public string F_TO_PKG_QTY
        {
            get
            {
                return (string)this["F_TO_PKG_QTY"];
            }
            set
            {
                this["F_TO_PKG_QTY"] = value;
            }
        }
        public string F_TO_NETWEIGHT
        {
            get
            {
                return (string)this["F_TO_NETWEIGHT"];
            }
            set
            {
                this["F_TO_NETWEIGHT"] = value;
            }
        }
        public string F_TO_GROSSWEIGHT
        {
            get
            {
                return (string)this["F_TO_GROSSWEIGHT"];
            }
            set
            {
                this["F_TO_GROSSWEIGHT"] = value;
            }
        }
        public string F_TO_VOLUME
        {
            get
            {
                return (string)this["F_TO_VOLUME"];
            }
            set
            {
                this["F_TO_VOLUME"] = value;
            }
        }
        public string F_TO_TRAILERNO
        {
            get
            {
                return (string)this["F_TO_TRAILERNO"];
            }
            set
            {
                this["F_TO_TRAILERNO"] = value;
            }
        }
        public string F_CARRIER_TYPE
        {
            get
            {
                return (string)this["F_CARRIER_TYPE"];
            }
            set
            {
                this["F_CARRIER_TYPE"] = value;
            }
        }
        public string F_CARRIER_CODE
        {
            get
            {
                return (string)this["F_CARRIER_CODE"];
            }
            set
            {
                this["F_CARRIER_CODE"] = value;
            }
        }
        public string F_CARRIER_TRAN_TYPE
        {
            get
            {
                return (string)this["F_CARRIER_TRAN_TYPE"];
            }
            set
            {
                this["F_CARRIER_TRAN_TYPE"] = value;
            }
        }
        public string F_CARRIER_REF_NO
        {
            get
            {
                return (string)this["F_CARRIER_REF_NO"];
            }
            set
            {
                this["F_CARRIER_REF_NO"] = value;
            }
        }
        public string F_CARRIER_TRAILER_NO
        {
            get
            {
                return (string)this["F_CARRIER_TRAILER_NO"];
            }
            set
            {
                this["F_CARRIER_TRAILER_NO"] = value;
            }
        }
        public string F_ST_NAME
        {
            get
            {
                return (string)this["F_ST_NAME"];
            }
            set
            {
                this["F_ST_NAME"] = value;
            }
        }
        public string F_ST_CONTACT
        {
            get
            {
                return (string)this["F_ST_CONTACT"];
            }
            set
            {
                this["F_ST_CONTACT"] = value;
            }
        }
        public string F_ST_CONTACT_MAIL
        {
            get
            {
                return (string)this["F_ST_CONTACT_MAIL"];
            }
            set
            {
                this["F_ST_CONTACT_MAIL"] = value;
            }
        }
        public string F_ST_CUSTOMERCODE
        {
            get
            {
                return (string)this["F_ST_CUSTOMERCODE"];
            }
            set
            {
                this["F_ST_CUSTOMERCODE"] = value;
            }
        }
        public string F_ST_ADDRESS
        {
            get
            {
                return (string)this["F_ST_ADDRESS"];
            }
            set
            {
                this["F_ST_ADDRESS"] = value;
            }
        }
        public string F_ST_CITY
        {
            get
            {
                return (string)this["F_ST_CITY"];
            }
            set
            {
                this["F_ST_CITY"] = value;
            }
        }
        public string F_ST_POSTCODE
        {
            get
            {
                return (string)this["F_ST_POSTCODE"];
            }
            set
            {
                this["F_ST_POSTCODE"] = value;
            }
        }
        public string F_ST_STATE_CODE
        {
            get
            {
                return (string)this["F_ST_STATE_CODE"];
            }
            set
            {
                this["F_ST_STATE_CODE"] = value;
            }
        }
        public string F_ST_COUNTRY_CODE
        {
            get
            {
                return (string)this["F_ST_COUNTRY_CODE"];
            }
            set
            {
                this["F_ST_COUNTRY_CODE"] = value;
            }
        }
        public string F_PO_NO
        {
            get
            {
                return (string)this["F_PO_NO"];
            }
            set
            {
                this["F_PO_NO"] = value;
            }
        }
        public string F_PO_LINE_NO
        {
            get
            {
                return (string)this["F_PO_LINE_NO"];
            }
            set
            {
                this["F_PO_LINE_NO"] = value;
            }
        }
        public string F_PO_LINE_QTY
        {
            get
            {
                return (string)this["F_PO_LINE_QTY"];
            }
            set
            {
                this["F_PO_LINE_QTY"] = value;
            }
        }
        public DateTime? F_PO_DATE
        {
            get
            {
                return (DateTime?)this["F_PO_DATE"];
            }
            set
            {
                this["F_PO_DATE"] = value;
            }
        }
        public string F_PKG_ID
        {
            get
            {
                return (string)this["F_PKG_ID"];
            }
            set
            {
                this["F_PKG_ID"] = value;
            }
        }
        public string F_PKG_QTY
        {
            get
            {
                return (string)this["F_PKG_QTY"];
            }
            set
            {
                this["F_PKG_QTY"] = value;
            }
        }
        public string F_PKG_GROSS_WEIGHT
        {
            get
            {
                return (string)this["F_PKG_GROSS_WEIGHT"];
            }
            set
            {
                this["F_PKG_GROSS_WEIGHT"] = value;
            }
        }
        public string F_PKG_DIMENSION
        {
            get
            {
                return (string)this["F_PKG_DIMENSION"];
            }
            set
            {
                this["F_PKG_DIMENSION"] = value;
            }
        }
        public string F_ITEM_MPN
        {
            get
            {
                return (string)this["F_ITEM_MPN"];
            }
            set
            {
                this["F_ITEM_MPN"] = value;
            }
        }
        public string F_ITEM_CPN
        {
            get
            {
                return (string)this["F_ITEM_CPN"];
            }
            set
            {
                this["F_ITEM_CPN"] = value;
            }
        }
        public string F_ITEM_SN
        {
            get
            {
                return (string)this["F_ITEM_SN"];
            }
            set
            {
                this["F_ITEM_SN"] = value;
            }
        }
        public string F_ITEM_COO
        {
            get
            {
                return (string)this["F_ITEM_COO"];
            }
            set
            {
                this["F_ITEM_COO"] = value;
            }
        }
        public DateTime? F_LASTEDITDT
        {
            get
            {
                return (DateTime?)this["F_LASTEDITDT"];
            }
            set
            {
                this["F_LASTEDITDT"] = value;
            }
        }
        public string F_INCOTERM
        {
            get
            {
                return (string)this["F_INCOTERM"];
            }
            set
            {
                this["F_INCOTERM"] = value;
            }
        }
    }
    public class HPE_EDI_856
    {
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string F_FILENAME { get; set; }
        public string F_TO_NO { get; set; }
        public DateTime? F_TO_DATE { get; set; }
        public DateTime? F_TO_SHIPDATE { get; set; }
        public string F_TO_DN { get; set; }
        public string F_TO_DN_LINE { get; set; }
        public string F_TO_PKG_QTY { get; set; }
        public string F_TO_NETWEIGHT { get; set; }
        public string F_TO_GROSSWEIGHT { get; set; }
        public string F_TO_VOLUME { get; set; }
        public string F_TO_TRAILERNO { get; set; }
        public string F_CARRIER_TYPE { get; set; }
        public string F_CARRIER_CODE { get; set; }
        public string F_CARRIER_TRAN_TYPE { get; set; }
        public string F_CARRIER_REF_NO { get; set; }
        public string F_CARRIER_TRAILER_NO { get; set; }
        public string F_ST_NAME { get; set; }
        public string F_ST_CONTACT { get; set; }
        public string F_ST_CONTACT_MAIL { get; set; }
        public string F_ST_CUSTOMERCODE { get; set; }
        public string F_ST_ADDRESS { get; set; }
        public string F_ST_CITY { get; set; }
        public string F_ST_POSTCODE { get; set; }
        public string F_ST_STATE_CODE { get; set; }
        public string F_ST_COUNTRY_CODE { get; set; }
        public string F_PO_NO { get; set; }
        public string F_PO_LINE_NO { get; set; }
        public string F_PO_LINE_QTY { get; set; }
        public DateTime? F_PO_DATE { get; set; }
        public string F_PKG_ID { get; set; }
        public string F_PKG_QTY { get; set; }
        public string F_PKG_GROSS_WEIGHT { get; set; }
        public string F_PKG_DIMENSION { get; set; }
        public string F_ITEM_MPN { get; set; }
        public string F_ITEM_CPN { get; set; }
        public string F_ITEM_SN { get; set; }
        public string F_ITEM_COO { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public string F_INCOTERM { get; set; }
    }
}