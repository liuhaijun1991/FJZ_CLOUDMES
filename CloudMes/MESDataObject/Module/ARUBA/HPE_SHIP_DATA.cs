using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.ARUBA
{
    public class T_HPE_SHIP_DATA : DataObjectTable
    {
        public T_HPE_SHIP_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_HPE_SHIP_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_HPE_SHIP_DATA);
            TableName = "HPE_SHIP_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_HPE_SHIP_DATA : DataObjectBase
    {
        public Row_HPE_SHIP_DATA(DataObjectInfo info) : base(info)
        {

        }
        public HPE_SHIP_DATA GetDataObject()
        {
            HPE_SHIP_DATA DataObject = new HPE_SHIP_DATA();
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.ID = this.ID;
            DataObject.F_TO_NO = this.F_TO_NO;
            DataObject.F_TO_DATE = this.F_TO_DATE;
            DataObject.F_TO_SHIPDATE = this.F_TO_SHIPDATE;
            DataObject.F_TO_DN = this.F_TO_DN;
            DataObject.F_TO_DN_LINE = this.F_TO_DN_LINE;
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
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.F_INCOTERM = this.F_INCOTERM;
            return DataObject;
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
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
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
    }
    public class HPE_SHIP_DATA
    {
        public string CREATE_EMP { get; set; }
        public string EDIT_EMP { get; set; }
        public string ID { get; set; }
        public string F_TO_NO { get; set; }
        public DateTime? F_TO_DATE { get; set; }
        public DateTime? F_TO_SHIPDATE { get; set; }
        public string F_TO_DN { get; set; }
        public string F_TO_DN_LINE { get; set; }
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
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string F_INCOTERM { get; set; }
    }
}