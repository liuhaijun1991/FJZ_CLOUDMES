using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BRCM_PICK2SHIP_DATA : DataObjectTable
    {
        public T_R_BRCM_PICK2SHIP_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BRCM_PICK2SHIP_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BRCM_PICK2SHIP_DATA);
            TableName = "R_BRCM_PICK2SHIP_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_BRCM_PICK2SHIP_DATA : DataObjectBase
    {
        public Row_R_BRCM_PICK2SHIP_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_BRCM_PICK2SHIP_DATA GetDataObject()
        {
            R_BRCM_PICK2SHIP_DATA DataObject = new R_BRCM_PICK2SHIP_DATA();
            DataObject.FILENAME = this.FILENAME;
            DataObject.ID = this.ID;
            DataObject.RECORD_TYPE = this.RECORD_TYPE;
            DataObject.CM_CODE = this.CM_CODE;
            DataObject.DELIVERY_NAME = this.DELIVERY_NAME;
            DataObject.SALES_ORDER_NUMBER = this.SALES_ORDER_NUMBER;
            DataObject.SALES_ORDER_LINE_NUMBER = this.SALES_ORDER_LINE_NUMBER;
            DataObject.SHIP_METHOD = this.SHIP_METHOD;
            DataObject.INCOTERM = this.INCOTERM;
            DataObject.SHIPMENT_DATE = this.SHIPMENT_DATE;
            DataObject.WAYBILL_NUMBER = this.WAYBILL_NUMBER;
            DataObject.SHIPPING_LPN = this.SHIPPING_LPN;
            DataObject.BOX_CODE = this.BOX_CODE;
            DataObject.BOX_WEIGHT = this.BOX_WEIGHT;
            DataObject.ITEM = this.ITEM;
            DataObject.LOT_NUMBER = this.LOT_NUMBER;
            DataObject.SHIPPED_QUANTITY = this.SHIPPED_QUANTITY;
            DataObject.LOT_LPN = this.LOT_LPN;
            DataObject.SERIAL_NUMBER = this.SERIAL_NUMBER;
            DataObject.CUSTOMER_SERIAL_NUMBER = this.CUSTOMER_SERIAL_NUMBER;
            DataObject.DEPARTMENT_CODE = this.DEPARTMENT_CODE;
            DataObject.COMMENT1 = this.COMMENT1;
            DataObject.RESERVED_COLUMNS1 = this.RESERVED_COLUMNS1;
            DataObject.RESERVED_COLUMNS2 = this.RESERVED_COLUMNS2;
            DataObject.RESERVED_COLUMNS3 = this.RESERVED_COLUMNS3;
            DataObject.RESERVED_COLUMNS4 = this.RESERVED_COLUMNS4;
            DataObject.RESERVED_COLUMNS5 = this.RESERVED_COLUMNS5;
            DataObject.RESERVED_COLUMNS6 = this.RESERVED_COLUMNS6;
            DataObject.RESERVED_COLUMNS7 = this.RESERVED_COLUMNS7;
            DataObject.RESERVED_COLUMNS8 = this.RESERVED_COLUMNS8;
            DataObject.RESERVED_COLUMNS9 = this.RESERVED_COLUMNS9;
            DataObject.RESERVED_COLUMNS10 = this.RESERVED_COLUMNS10;
            DataObject.RESERVED_COLUMNS11 = this.RESERVED_COLUMNS11;
            DataObject.RESERVED_COLUMNS12 = this.RESERVED_COLUMNS12;
            DataObject.RESERVED_COLUMNS13 = this.RESERVED_COLUMNS13;
            DataObject.RESERVED_COLUMNS14 = this.RESERVED_COLUMNS14;
            DataObject.RESERVED_COLUMNS15 = this.RESERVED_COLUMNS15;
            DataObject.RESERVED_COLUMNS16 = this.RESERVED_COLUMNS16;
            DataObject.RESERVED_COLUMNS17 = this.RESERVED_COLUMNS17;
            DataObject.RESERVED_COLUMNS18 = this.RESERVED_COLUMNS18;
            DataObject.RESERVED_COLUMNS19 = this.RESERVED_COLUMNS19;
            DataObject.RESERVED_COLUMNS20 = this.RESERVED_COLUMNS20;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
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
        public string RECORD_TYPE
        {
            get
            {
                return (string)this["RECORD_TYPE"];
            }
            set
            {
                this["RECORD_TYPE"] = value;
            }
        }
        public string CM_CODE
        {
            get
            {
                return (string)this["CM_CODE"];
            }
            set
            {
                this["CM_CODE"] = value;
            }
        }
        public string DELIVERY_NAME
        {
            get
            {
                return (string)this["DELIVERY_NAME"];
            }
            set
            {
                this["DELIVERY_NAME"] = value;
            }
        }
        public string SALES_ORDER_NUMBER
        {
            get
            {
                return (string)this["SALES_ORDER_NUMBER"];
            }
            set
            {
                this["SALES_ORDER_NUMBER"] = value;
            }
        }
        public string SALES_ORDER_LINE_NUMBER
        {
            get
            {
                return (string)this["SALES_ORDER_LINE_NUMBER"];
            }
            set
            {
                this["SALES_ORDER_LINE_NUMBER"] = value;
            }
        }
        public string SHIP_METHOD
        {
            get
            {
                return (string)this["SHIP_METHOD"];
            }
            set
            {
                this["SHIP_METHOD"] = value;
            }
        }
        public string INCOTERM
        {
            get
            {
                return (string)this["INCOTERM"];
            }
            set
            {
                this["INCOTERM"] = value;
            }
        }
        public string SHIPMENT_DATE
        {
            get
            {
                return (string)this["SHIPMENT_DATE"];
            }
            set
            {
                this["SHIPMENT_DATE"] = value;
            }
        }
        public string WAYBILL_NUMBER
        {
            get
            {
                return (string)this["WAYBILL_NUMBER"];
            }
            set
            {
                this["WAYBILL_NUMBER"] = value;
            }
        }
        public string SHIPPING_LPN
        {
            get
            {
                return (string)this["SHIPPING_LPN"];
            }
            set
            {
                this["SHIPPING_LPN"] = value;
            }
        }
        public string BOX_CODE
        {
            get
            {
                return (string)this["BOX_CODE"];
            }
            set
            {
                this["BOX_CODE"] = value;
            }
        }
        public string BOX_WEIGHT
        {
            get
            {
                return (string)this["BOX_WEIGHT"];
            }
            set
            {
                this["BOX_WEIGHT"] = value;
            }
        }
        public string ITEM
        {
            get
            {
                return (string)this["ITEM"];
            }
            set
            {
                this["ITEM"] = value;
            }
        }
        public string LOT_NUMBER
        {
            get
            {
                return (string)this["LOT_NUMBER"];
            }
            set
            {
                this["LOT_NUMBER"] = value;
            }
        }
        public string SHIPPED_QUANTITY
        {
            get
            {
                return (string)this["SHIPPED_QUANTITY"];
            }
            set
            {
                this["SHIPPED_QUANTITY"] = value;
            }
        }
        public string LOT_LPN
        {
            get
            {
                return (string)this["LOT_LPN"];
            }
            set
            {
                this["LOT_LPN"] = value;
            }
        }
        public string SERIAL_NUMBER
        {
            get
            {
                return (string)this["SERIAL_NUMBER"];
            }
            set
            {
                this["SERIAL_NUMBER"] = value;
            }
        }
        public string CUSTOMER_SERIAL_NUMBER
        {
            get
            {
                return (string)this["CUSTOMER_SERIAL_NUMBER"];
            }
            set
            {
                this["CUSTOMER_SERIAL_NUMBER"] = value;
            }
        }
        public string DEPARTMENT_CODE
        {
            get
            {
                return (string)this["DEPARTMENT_CODE"];
            }
            set
            {
                this["DEPARTMENT_CODE"] = value;
            }
        }
        public string COMMENT1
        {
            get
            {
                return (string)this["COMMENT1"];
            }
            set
            {
                this["COMMENT1"] = value;
            }
        }
        public string RESERVED_COLUMNS1
        {
            get
            {
                return (string)this["RESERVED_COLUMNS1"];
            }
            set
            {
                this["RESERVED_COLUMNS1"] = value;
            }
        }
        public string RESERVED_COLUMNS2
        {
            get
            {
                return (string)this["RESERVED_COLUMNS2"];
            }
            set
            {
                this["RESERVED_COLUMNS2"] = value;
            }
        }
        public string RESERVED_COLUMNS3
        {
            get
            {
                return (string)this["RESERVED_COLUMNS3"];
            }
            set
            {
                this["RESERVED_COLUMNS3"] = value;
            }
        }
        public string RESERVED_COLUMNS4
        {
            get
            {
                return (string)this["RESERVED_COLUMNS4"];
            }
            set
            {
                this["RESERVED_COLUMNS4"] = value;
            }
        }
        public string RESERVED_COLUMNS5
        {
            get
            {
                return (string)this["RESERVED_COLUMNS5"];
            }
            set
            {
                this["RESERVED_COLUMNS5"] = value;
            }
        }
        public string RESERVED_COLUMNS6
        {
            get
            {
                return (string)this["RESERVED_COLUMNS6"];
            }
            set
            {
                this["RESERVED_COLUMNS6"] = value;
            }
        }
        public string RESERVED_COLUMNS7
        {
            get
            {
                return (string)this["RESERVED_COLUMNS7"];
            }
            set
            {
                this["RESERVED_COLUMNS7"] = value;
            }
        }
        public string RESERVED_COLUMNS8
        {
            get
            {
                return (string)this["RESERVED_COLUMNS8"];
            }
            set
            {
                this["RESERVED_COLUMNS8"] = value;
            }
        }
        public string RESERVED_COLUMNS9
        {
            get
            {
                return (string)this["RESERVED_COLUMNS9"];
            }
            set
            {
                this["RESERVED_COLUMNS9"] = value;
            }
        }
        public string RESERVED_COLUMNS10
        {
            get
            {
                return (string)this["RESERVED_COLUMNS10"];
            }
            set
            {
                this["RESERVED_COLUMNS10"] = value;
            }
        }
        public string RESERVED_COLUMNS11
        {
            get
            {
                return (string)this["RESERVED_COLUMNS11"];
            }
            set
            {
                this["RESERVED_COLUMNS11"] = value;
            }
        }
        public string RESERVED_COLUMNS12
        {
            get
            {
                return (string)this["RESERVED_COLUMNS12"];
            }
            set
            {
                this["RESERVED_COLUMNS12"] = value;
            }
        }
        public string RESERVED_COLUMNS13
        {
            get
            {
                return (string)this["RESERVED_COLUMNS13"];
            }
            set
            {
                this["RESERVED_COLUMNS13"] = value;
            }
        }
        public string RESERVED_COLUMNS14
        {
            get
            {
                return (string)this["RESERVED_COLUMNS14"];
            }
            set
            {
                this["RESERVED_COLUMNS14"] = value;
            }
        }
        public string RESERVED_COLUMNS15
        {
            get
            {
                return (string)this["RESERVED_COLUMNS15"];
            }
            set
            {
                this["RESERVED_COLUMNS15"] = value;
            }
        }
        public string RESERVED_COLUMNS16
        {
            get
            {
                return (string)this["RESERVED_COLUMNS16"];
            }
            set
            {
                this["RESERVED_COLUMNS16"] = value;
            }
        }
        public string RESERVED_COLUMNS17
        {
            get
            {
                return (string)this["RESERVED_COLUMNS17"];
            }
            set
            {
                this["RESERVED_COLUMNS17"] = value;
            }
        }
        public string RESERVED_COLUMNS18
        {
            get
            {
                return (string)this["RESERVED_COLUMNS18"];
            }
            set
            {
                this["RESERVED_COLUMNS18"] = value;
            }
        }
        public string RESERVED_COLUMNS19
        {
            get
            {
                return (string)this["RESERVED_COLUMNS19"];
            }
            set
            {
                this["RESERVED_COLUMNS19"] = value;
            }
        }
        public string RESERVED_COLUMNS20
        {
            get
            {
                return (string)this["RESERVED_COLUMNS20"];
            }
            set
            {
                this["RESERVED_COLUMNS20"] = value;
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
    }
    public class R_BRCM_PICK2SHIP_DATA
    {
        public string FILENAME { get; set; }
        public string ID { get; set; }
        public string RECORD_TYPE { get; set; }
        public string CM_CODE { get; set; }
        public string DELIVERY_NAME { get; set; }
        public string SALES_ORDER_NUMBER { get; set; }
        public string SALES_ORDER_LINE_NUMBER { get; set; }
        public string SHIP_METHOD { get; set; }
        public string INCOTERM { get; set; }
        public string SHIPMENT_DATE { get; set; }
        public string WAYBILL_NUMBER { get; set; }
        public string SHIPPING_LPN { get; set; }
        public string BOX_CODE { get; set; }
        public string BOX_WEIGHT { get; set; }
        public string ITEM { get; set; }
        public string LOT_NUMBER { get; set; }
        public string SHIPPED_QUANTITY { get; set; }
        public string LOT_LPN { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string CUSTOMER_SERIAL_NUMBER { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string COMMENT1 { get; set; }
        public string RESERVED_COLUMNS1 { get; set; }
        public string RESERVED_COLUMNS2 { get; set; }
        public string RESERVED_COLUMNS3 { get; set; }
        public string RESERVED_COLUMNS4 { get; set; }
        public string RESERVED_COLUMNS5 { get; set; }
        public string RESERVED_COLUMNS6 { get; set; }
        public string RESERVED_COLUMNS7 { get; set; }
        public string RESERVED_COLUMNS8 { get; set; }
        public string RESERVED_COLUMNS9 { get; set; }
        public string RESERVED_COLUMNS10 { get; set; }
        public string RESERVED_COLUMNS11 { get; set; }
        public string RESERVED_COLUMNS12 { get; set; }
        public string RESERVED_COLUMNS13 { get; set; }
        public string RESERVED_COLUMNS14 { get; set; }
        public string RESERVED_COLUMNS15 { get; set; }
        public string RESERVED_COLUMNS16 { get; set; }
        public string RESERVED_COLUMNS17 { get; set; }
        public string RESERVED_COLUMNS18 { get; set; }
        public string RESERVED_COLUMNS19 { get; set; }
        public string RESERVED_COLUMNS20 { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}