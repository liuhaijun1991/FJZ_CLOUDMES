using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_PO_ITEM : DataObjectTable
    {
        public T_PO_ITEM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_PO_ITEM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_PO_ITEM);
            TableName = "PO_ITEM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_PO_ITEM : DataObjectBase
    {
        public Row_PO_ITEM(DataObjectInfo info) : base(info)
        {

        }
        public PO_ITEM GetDataObject()
        {
            PO_ITEM DataObject = new PO_ITEM();
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.PO = this.PO;
            DataObject.LINENO = this.LINENO;
            DataObject.DELETIONINDICATOR = this.DELETIONINDICATOR;
            DataObject.PLANT = this.PLANT;
            DataObject.CHANGEINDICATOR = this.CHANGEINDICATOR;
            DataObject.SPL = this.SPL;
            DataObject.EMBARGO = this.EMBARGO;
            DataObject.LICENSE = this.LICENSE;
            DataObject.GTS_BLD_NO_SHIP = this.GTS_BLD_NO_SHIP;
            DataObject.CREDIT = this.CREDIT;
            DataObject.DELIVERY_BLOCK_CODE = this.DELIVERY_BLOCK_CODE;
            DataObject.CURRENCYCODE = this.CURRENCYCODE;
            DataObject.COUNTRYSPECIFICLABEL = this.COUNTRYSPECIFICLABEL;
            DataObject.CUSTOMERPRODID = this.CUSTOMERPRODID;
            DataObject.DELIVERYGROUP = this.DELIVERYGROUP;
            DataObject.SKU = this.SKU;
            DataObject.PACKOUTLABELTYPE = this.PACKOUTLABELTYPE;
            DataObject.SALESORDERHOLD = this.SALESORDERHOLD;
            DataObject.CARTONLABEL1 = this.CARTONLABEL1;
            DataObject.CARTONLABEL2 = this.CARTONLABEL2;
            DataObject.CHANGEREQUESTEDDATE = this.CHANGEREQUESTEDDATE;
            DataObject.CUSTREQSHIPDATE = this.CUSTREQSHIPDATE;
            DataObject.DELIVERYPRIORITY = this.DELIVERYPRIORITY;
            DataObject.SCHEDULINGSTATUS = this.SCHEDULINGSTATUS;
            DataObject.DELIVERYDATE = this.DELIVERYDATE;
            DataObject.QTY = this.QTY;
            DataObject.UOM = this.UOM;
            DataObject.SO = this.SO;
            DataObject.SOLINE = this.SOLINE;
            DataObject.LINESHIPMETHOD = this.LINESHIPMETHOD;
            DataObject.PRODUCTFAMILY = this.PRODUCTFAMILY;
            DataObject.HIGHERLEVELSO = this.HIGHERLEVELSO;
            DataObject.HIGHERLEVELPO = this.HIGHERLEVELPO;
            DataObject.RMQCOMMITDATE = this.RMQCOMMITDATE;
            DataObject.NETPRICE = this.NETPRICE;
            DataObject.TAA = this.TAA;
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
        public string PO
        {
            get
            {
                return (string)this["PO"];
            }
            set
            {
                this["PO"] = value;
            }
        }
        public string LINENO
        {
            get
            {
                return (string)this["LINENO"];
            }
            set
            {
                this["LINENO"] = value;
            }
        }
        public string DELETIONINDICATOR
        {
            get
            {
                return (string)this["DELETIONINDICATOR"];
            }
            set
            {
                this["DELETIONINDICATOR"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string CHANGEINDICATOR
        {
            get
            {
                return (string)this["CHANGEINDICATOR"];
            }
            set
            {
                this["CHANGEINDICATOR"] = value;
            }
        }
        public string SPL
        {
            get
            {
                return (string)this["SPL"];
            }
            set
            {
                this["SPL"] = value;
            }
        }
        public string EMBARGO
        {
            get
            {
                return (string)this["EMBARGO"];
            }
            set
            {
                this["EMBARGO"] = value;
            }
        }
        public string LICENSE
        {
            get
            {
                return (string)this["LICENSE"];
            }
            set
            {
                this["LICENSE"] = value;
            }
        }
        public string GTS_BLD_NO_SHIP
        {
            get
            {
                return (string)this["GTS_BLD_NO_SHIP"];
            }
            set
            {
                this["GTS_BLD_NO_SHIP"] = value;
            }
        }
        public string CREDIT
        {
            get
            {
                return (string)this["CREDIT"];
            }
            set
            {
                this["CREDIT"] = value;
            }
        }
        public string DELIVERY_BLOCK_CODE
        {
            get
            {
                return (string)this["DELIVERY_BLOCK_CODE"];
            }
            set
            {
                this["DELIVERY_BLOCK_CODE"] = value;
            }
        }
        public string CURRENCYCODE
        {
            get
            {
                return (string)this["CURRENCYCODE"];
            }
            set
            {
                this["CURRENCYCODE"] = value;
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
        public string CUSTOMERPRODID
        {
            get
            {
                return (string)this["CUSTOMERPRODID"];
            }
            set
            {
                this["CUSTOMERPRODID"] = value;
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
        public string SKU
        {
            get
            {
                return (string)this["SKU"];
            }
            set
            {
                this["SKU"] = value;
            }
        }
        public string PACKOUTLABELTYPE
        {
            get
            {
                return (string)this["PACKOUTLABELTYPE"];
            }
            set
            {
                this["PACKOUTLABELTYPE"] = value;
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
        public string CUSTREQSHIPDATE
        {
            get
            {
                return (string)this["CUSTREQSHIPDATE"];
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
        public string DELIVERYDATE
        {
            get
            {
                return (string)this["DELIVERYDATE"];
            }
            set
            {
                this["DELIVERYDATE"] = value;
            }
        }
        public string QTY
        {
            get
            {
                return (string)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string UOM
        {
            get
            {
                return (string)this["UOM"];
            }
            set
            {
                this["UOM"] = value;
            }
        }
        public string SO
        {
            get
            {
                return (string)this["SO"];
            }
            set
            {
                this["SO"] = value;
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
        public string HIGHERLEVELSO
        {
            get
            {
                return (string)this["HIGHERLEVELSO"];
            }
            set
            {
                this["HIGHERLEVELSO"] = value;
            }
        }
        public string HIGHERLEVELPO
        {
            get
            {
                return (string)this["HIGHERLEVELPO"];
            }
            set
            {
                this["HIGHERLEVELPO"] = value;
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
        public string TAA
        {
            get
            {
                return (string)this["TAA"];
            }
            set
            {
                this["TAA"] = value;
            }
        }
    }
    public class PO_ITEM
    {
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PO { get; set; }
        public string LINENO { get; set; }
        public string DELETIONINDICATOR { get; set; }
        public string PLANT { get; set; }
        public string CHANGEINDICATOR { get; set; }
        public string SPL { get; set; }
        public string EMBARGO { get; set; }
        public string LICENSE { get; set; }
        public string GTS_BLD_NO_SHIP { get; set; }
        public string CREDIT { get; set; }
        public string DELIVERY_BLOCK_CODE { get; set; }
        public string CURRENCYCODE { get; set; }
        public string COUNTRYSPECIFICLABEL { get; set; }
        public string CUSTOMERPRODID { get; set; }
        public string DELIVERYGROUP { get; set; }
        public string SKU { get; set; }
        public string PACKOUTLABELTYPE { get; set; }
        public string SALESORDERHOLD { get; set; }
        public string CARTONLABEL1 { get; set; }
        public string CARTONLABEL2 { get; set; }
        public string CHANGEREQUESTEDDATE { get; set; }
        public string CUSTREQSHIPDATE { get; set; }
        public string DELIVERYPRIORITY { get; set; }
        public string SCHEDULINGSTATUS { get; set; }
        public string DELIVERYDATE { get; set; }
        public string QTY { get; set; }
        public string UOM { get; set; }
        public string SO { get; set; }
        public string SOLINE { get; set; }
        public string LINESHIPMETHOD { get; set; }
        public string PRODUCTFAMILY { get; set; }
        public string HIGHERLEVELSO { get; set; }
        public string HIGHERLEVELPO { get; set; }
        public string RMQCOMMITDATE { get; set; }
        public string NETPRICE { get; set; }
        public string TAA { get; set; }
    }
}