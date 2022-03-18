using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
namespace MESDataObject.Module.Juniper
{
    public class T_R_I137_ITEM : DataObjectTable
    {
        public T_R_I137_ITEM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I137_ITEM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I137_ITEM);
            TableName = "R_I137_ITEM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I137_ITEM : DataObjectBase
    {
        public Row_R_I137_ITEM(DataObjectInfo info) : base(info)
        {

        }
        public R_I137_ITEM GetDataObject()
        {
            R_I137_ITEM DataObject = new R_I137_ITEM();
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.TRANID = this.TRANID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.PONUMBER = this.PONUMBER;
            DataObject.ITEM = this.ITEM;
            DataObject.PARENTITEMID = this.PARENTITEMID;
            DataObject.TYPECODE = this.TYPECODE;
            DataObject.ITEMCHANGEINDICATOR = this.ITEMCHANGEINDICATOR;
            DataObject.LASTCHANGEDATE = this.LASTCHANGEDATE;
            DataObject.BLOCKEDINDICATOR = this.BLOCKEDINDICATOR;
            DataObject.LINESHIPMETHOD = this.LINESHIPMETHOD;
            DataObject.SALESORDERLINEITEM = this.SALESORDERLINEITEM;
            DataObject.SOID = this.SOID;
            DataObject.MATERIALID = this.MATERIALID;
            DataObject.SOQTY = this.SOQTY;
            DataObject.TAAINDICATOR = this.TAAINDICATOR;
            DataObject.SWTYPE = this.SWTYPE;
            DataObject.SWVERSION = this.SWVERSION;
            DataObject.SWPARTNUMBER = this.SWPARTNUMBER;
            DataObject.ITEMAGENCYID = this.ITEMAGENCYID;
            DataObject.JNP_PLANT = this.JNP_PLANT;
            DataObject.SALESORDERHOLD = this.SALESORDERHOLD;
            DataObject.PN = this.PN;
            DataObject.CUSTPRODID = this.CUSTPRODID;
            DataObject.PRODUCTFAMILY = this.PRODUCTFAMILY;
            DataObject.PACKOUTLABEL = this.PACKOUTLABEL;
            DataObject.COUNTRYSPECIFICLABEL = this.COUNTRYSPECIFICLABEL;
            DataObject.CARTONLABEL1 = this.CARTONLABEL1;
            DataObject.CARTONLABEL2 = this.CARTONLABEL2;
            DataObject.CUSTOMERPN = this.CUSTOMERPN;
            DataObject.MATERIALNUMBER = this.MATERIALNUMBER;
            DataObject.LINEQUANTITY = this.LINEQUANTITY;
            DataObject.SOLINE = this.SOLINE;
            DataObject.RMQCOMMITDATE = this.RMQCOMMITDATE;
            DataObject.DELIVERYGROUP = this.DELIVERYGROUP;
            DataObject.CLASSIFICATIONCODE = this.CLASSIFICATIONCODE;
            DataObject.TRANSFERLOCATIONNAME = this.TRANSFERLOCATIONNAME;
            DataObject.NETPRICE = this.NETPRICE;
            DataObject.UNITCODE = this.UNITCODE;
            DataObject.BASEQUANTITY = this.BASEQUANTITY;
            DataObject.DELIVERYSTARTDATE = this.DELIVERYSTARTDATE;
            DataObject.CHANGEREQUESTEDDATE = this.CHANGEREQUESTEDDATE;
            DataObject.CUSTREQSHIPDATE = this.CUSTREQSHIPDATE;
            DataObject.DELIVERYPRIORITY = this.DELIVERYPRIORITY;
            DataObject.DELIVERYSCHEDULINGSTATUS = this.DELIVERYSCHEDULINGSTATUS;
            DataObject.SCHEDULINGSTATUS = this.SCHEDULINGSTATUS;
            DataObject.QUANTITYCODE = this.QUANTITYCODE;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
            }
        }
        public string F_PLANT
        {
            get
            {
                return (string)this["F_PLANT"];
            }
            set
            {
                this["F_PLANT"] = value;
            }
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
        public string MESSAGEID
        {
            get
            {
                return (string)this["MESSAGEID"];
            }
            set
            {
                this["MESSAGEID"] = value;
            }
        }
        public string PONUMBER
        {
            get
            {
                return (string)this["PONUMBER"];
            }
            set
            {
                this["PONUMBER"] = value;
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
        public string PARENTITEMID
        {
            get
            {
                return (string)this["PARENTITEMID"];
            }
            set
            {
                this["PARENTITEMID"] = value;
            }
        }
        public string TYPECODE
        {
            get
            {
                return (string)this["TYPECODE"];
            }
            set
            {
                this["TYPECODE"] = value;
            }
        }
        public string ITEMCHANGEINDICATOR
        {
            get
            {
                return (string)this["ITEMCHANGEINDICATOR"];
            }
            set
            {
                this["ITEMCHANGEINDICATOR"] = value;
            }
        }
        public DateTime? LASTCHANGEDATE
        {
            get
            {
                return (DateTime?)this["LASTCHANGEDATE"];
            }
            set
            {
                this["LASTCHANGEDATE"] = value;
            }
        }
        public string BLOCKEDINDICATOR
        {
            get
            {
                return (string)this["BLOCKEDINDICATOR"];
            }
            set
            {
                this["BLOCKEDINDICATOR"] = value;
            }
        }
        public string LINESHIPMETHOD
        {
            get
            {
                return (string)this["LINESHIPMETHOD"];
            }
            set
            {
                this["LINESHIPMETHOD"] = value;
            }
        }
        public string SALESORDERLINEITEM
        {
            get
            {
                return (string)this["SALESORDERLINEITEM"];
            }
            set
            {
                this["SALESORDERLINEITEM"] = value;
            }
        }
        public string SOID
        {
            get
            {
                return (string)this["SOID"];
            }
            set
            {
                this["SOID"] = value;
            }
        }
        public string MATERIALID
        {
            get
            {
                return (string)this["MATERIALID"];
            }
            set
            {
                this["MATERIALID"] = value;
            }
        }
        public string SOQTY
        {
            get
            {
                return (string)this["SOQTY"];
            }
            set
            {
                this["SOQTY"] = value;
            }
        }
        public string TAAINDICATOR
        {
            get
            {
                return (string)this["TAAINDICATOR"];
            }
            set
            {
                this["TAAINDICATOR"] = value;
            }
        }
        public string SWTYPE
        {
            get
            {
                return (string)this["SWTYPE"];
            }
            set
            {
                this["SWTYPE"] = value;
            }
        }
        public string SWVERSION
        {
            get
            {
                return (string)this["SWVERSION"];
            }
            set
            {
                this["SWVERSION"] = value;
            }
        }
        public string SWPARTNUMBER
        {
            get
            {
                return (string)this["SWPARTNUMBER"];
            }
            set
            {
                this["SWPARTNUMBER"] = value;
            }
        }
        public string ITEMAGENCYID
        {
            get
            {
                return (string)this["ITEMAGENCYID"];
            }
            set
            {
                this["ITEMAGENCYID"] = value;
            }
        }
        public string JNP_PLANT
        {
            get
            {
                return (string)this["JNP_PLANT"];
            }
            set
            {
                this["JNP_PLANT"] = value;
            }
        }
        public string SALESORDERHOLD
        {
            get
            {
                return (string)this["SALESORDERHOLD"];
            }
            set
            {
                this["SALESORDERHOLD"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string CUSTPRODID
        {
            get
            {
                return (string)this["CUSTPRODID"];
            }
            set
            {
                this["CUSTPRODID"] = value;
            }
        }
        public string PRODUCTFAMILY
        {
            get
            {
                return (string)this["PRODUCTFAMILY"];
            }
            set
            {
                this["PRODUCTFAMILY"] = value;
            }
        }
        public string PACKOUTLABEL
        {
            get
            {
                return (string)this["PACKOUTLABEL"];
            }
            set
            {
                this["PACKOUTLABEL"] = value;
            }
        }
        public string COUNTRYSPECIFICLABEL
        {
            get
            {
                return (string)this["COUNTRYSPECIFICLABEL"];
            }
            set
            {
                this["COUNTRYSPECIFICLABEL"] = value;
            }
        }
        public string CARTONLABEL1
        {
            get
            {
                return (string)this["CARTONLABEL1"];
            }
            set
            {
                this["CARTONLABEL1"] = value;
            }
        }
        public string CARTONLABEL2
        {
            get
            {
                return (string)this["CARTONLABEL2"];
            }
            set
            {
                this["CARTONLABEL2"] = value;
            }
        }
        public string CUSTOMERPN
        {
            get
            {
                return (string)this["CUSTOMERPN"];
            }
            set
            {
                this["CUSTOMERPN"] = value;
            }
        }
        public string MATERIALNUMBER
        {
            get
            {
                return (string)this["MATERIALNUMBER"];
            }
            set
            {
                this["MATERIALNUMBER"] = value;
            }
        }
        public string LINEQUANTITY
        {
            get
            {
                return (string)this["LINEQUANTITY"];
            }
            set
            {
                this["LINEQUANTITY"] = value;
            }
        }
        public string SOLINE
        {
            get
            {
                return (string)this["SOLINE"];
            }
            set
            {
                this["SOLINE"] = value;
            }
        }
        public string RMQCOMMITDATE
        {
            get
            {
                return (string)this["RMQCOMMITDATE"];
            }
            set
            {
                this["RMQCOMMITDATE"] = value;
            }
        }
        public string DELIVERYGROUP
        {
            get
            {
                return (string)this["DELIVERYGROUP"];
            }
            set
            {
                this["DELIVERYGROUP"] = value;
            }
        }
        public string CLASSIFICATIONCODE
        {
            get
            {
                return (string)this["CLASSIFICATIONCODE"];
            }
            set
            {
                this["CLASSIFICATIONCODE"] = value;
            }
        }
        public string TRANSFERLOCATIONNAME
        {
            get
            {
                return (string)this["TRANSFERLOCATIONNAME"];
            }
            set
            {
                this["TRANSFERLOCATIONNAME"] = value;
            }
        }
        public string NETPRICE
        {
            get
            {
                return (string)this["NETPRICE"];
            }
            set
            {
                this["NETPRICE"] = value;
            }
        }
        public string UNITCODE
        {
            get
            {
                return (string)this["UNITCODE"];
            }
            set
            {
                this["UNITCODE"] = value;
            }
        }
        public string BASEQUANTITY
        {
            get
            {
                return (string)this["BASEQUANTITY"];
            }
            set
            {
                this["BASEQUANTITY"] = value;
            }
        }
        public DateTime? DELIVERYSTARTDATE
        {
            get
            {
                return (DateTime?)this["DELIVERYSTARTDATE"];
            }
            set
            {
                this["DELIVERYSTARTDATE"] = value;
            }
        }
        public string CHANGEREQUESTEDDATE
        {
            get
            {
                return (string)this["CHANGEREQUESTEDDATE"];
            }
            set
            {
                this["CHANGEREQUESTEDDATE"] = value;
            }
        }
        public DateTime? CUSTREQSHIPDATE
        {
            get
            {
                return (DateTime?)this["CUSTREQSHIPDATE"];
            }
            set
            {
                this["CUSTREQSHIPDATE"] = value;
            }
        }
        public string DELIVERYPRIORITY
        {
            get
            {
                return (string)this["DELIVERYPRIORITY"];
            }
            set
            {
                this["DELIVERYPRIORITY"] = value;
            }
        }
        public string DELIVERYSCHEDULINGSTATUS
        {
            get
            {
                return (string)this["DELIVERYSCHEDULINGSTATUS"];
            }
            set
            {
                this["DELIVERYSCHEDULINGSTATUS"] = value;
            }
        }
        public string SCHEDULINGSTATUS
        {
            get
            {
                return (string)this["SCHEDULINGSTATUS"];
            }
            set
            {
                this["SCHEDULINGSTATUS"] = value;
            }
        }
        public string QUANTITYCODE
        {
            get
            {
                return (string)this["QUANTITYCODE"];
            }
            set
            {
                this["QUANTITYCODE"] = value;
            }
        }
        public string QUANTITY
        {
            get
            {
                return (string)this["QUANTITY"];
            }
            set
            {
                this["QUANTITY"] = value;
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
    public class R_I137_ITEM
    {
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string PARENTITEMID { get; set; }
        public string TYPECODE { get; set; }
        public string ITEMCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATE { get; set; }
        public string BLOCKEDINDICATOR { get; set; }
        public string LINESHIPMETHOD { get; set; }
        public string SALESORDERLINEITEM { get; set; }
        public string SOID { get; set; }
        public string MATERIALID { get; set; }
        public string SOQTY { get; set; }
        public string TAAINDICATOR { get; set; }
        public string SWTYPE { get; set; }
        public string SWVERSION { get; set; }
        public string SWPARTNUMBER { get; set; }
        public string ITEMAGENCYID { get; set; }
        public string JNP_PLANT { get; set; }
        public string SALESORDERHOLD { get; set; }
        public string PN { get; set; }
        public string CUSTPRODID { get; set; }
        public string PRODUCTFAMILY { get; set; }
        public string PACKOUTLABEL { get; set; }
        public string COUNTRYSPECIFICLABEL { get; set; }
        public string CARTONLABEL1 { get; set; }
        public string CARTONLABEL2 { get; set; }
        public string CUSTOMERPN { get; set; }
        public string MATERIALNUMBER { get; set; }
        public string LINEQUANTITY { get; set; }
        public string SOLINE { get; set; }
        public string RMQCOMMITDATE { get; set; }
        public string DELIVERYGROUP { get; set; }
        public string CLASSIFICATIONCODE { get; set; }
        public string TRANSFERLOCATIONNAME { get; set; }
        public string NETPRICE { get; set; }
        public string UNITCODE { get; set; }
        public string BASEQUANTITY { get; set; }
        public DateTime? DELIVERYSTARTDATE { get; set; }
        public string CHANGEREQUESTEDDATE { get; set; }
        public DateTime? CUSTREQSHIPDATE { get; set; }
        public string DELIVERYPRIORITY { get; set; }
        public string DELIVERYSCHEDULINGSTATUS { get; set; }
        public string SCHEDULINGSTATUS { get; set; }
        public string QUANTITYCODE { get; set; }
        public string QUANTITY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}
