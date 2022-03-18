using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module.Juniper
{
    public class T_R_I137 : DataObjectTable
    {
        public T_R_I137(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I137(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I137);
            TableName = "R_I137".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I137 : DataObjectBase
    {
        public Row_R_I137(DataObjectInfo info) : base(info)
        {

        }
        public R_I137 GetDataObject()
        {
            R_I137 DataObject = new R_I137();
            DataObject.INCO1 = this.INCO1;
            DataObject.INCO2 = this.INCO2;
            DataObject.SOLDTOID = this.SOLDTOID;
            DataObject.SOLDTOCOMPANY = this.SOLDTOCOMPANY;
            DataObject.SOLDTOCOUNTRYCODE = this.SOLDTOCOUNTRYCODE;
            DataObject.SOLDTOREGIONCODE = this.SOLDTOREGIONCODE;
            DataObject.SOLDTOSTREETPOSTALCODE = this.SOLDTOSTREETPOSTALCODE;
            DataObject.SOLDTOCITYNAME = this.SOLDTOCITYNAME;
            DataObject.SOLDTOSTREETNAME = this.SOLDTOSTREETNAME;
            DataObject.SOLDTOHOUSEID = this.SOLDTOHOUSEID;
            DataObject.SOLDTOPERSONNAME = this.SOLDTOPERSONNAME;
            DataObject.BILLTOID = this.BILLTOID;
            DataObject.BILLTOCOMPANY = this.BILLTOCOMPANY;
            DataObject.BILLTOCOUNTRYCODE = this.BILLTOCOUNTRYCODE;
            DataObject.BILLTOREGIONCODE = this.BILLTOREGIONCODE;
            DataObject.BILLTOSTREETPOSTALCODE = this.BILLTOSTREETPOSTALCODE;
            DataObject.BILLTOCITYNAME = this.BILLTOCITYNAME;
            DataObject.BILLTOSTREETNAME = this.BILLTOSTREETNAME;
            DataObject.BILLTOHOUSEID = this.BILLTOHOUSEID;
            DataObject.BILLTOPERSONNAME = this.BILLTOPERSONNAME;
            DataObject.BUYERPARTYID = this.BUYERPARTYID;
            DataObject.BUYERCOUNTRYCODE = this.BUYERCOUNTRYCODE;
            DataObject.BUYERREGIONCODE = this.BUYERREGIONCODE;
            DataObject.BUYERSTREETPOSTALCODE = this.BUYERSTREETPOSTALCODE;
            DataObject.BUYERCITYNAME = this.BUYERCITYNAME;
            DataObject.BUYERSTREETNAME = this.BUYERSTREETNAME;
            DataObject.BUYERDEVIATINGFULLNAME = this.BUYERDEVIATINGFULLNAME;
            DataObject.BUYEREMAILURI = this.BUYEREMAILURI;
            DataObject.SELLERPARTYID = this.SELLERPARTYID;
            DataObject.ECO_FCO = this.ECO_FCO;
            DataObject.PAYMENTTERM = this.PAYMENTTERM;
            DataObject.ACTIONCODE = this.ACTIONCODE;
            DataObject.ITEM = this.ITEM;
            DataObject.PARENTITEMID = this.PARENTITEMID;
            DataObject.ITEMCHANGEINDICATOR = this.ITEMCHANGEINDICATOR;
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
            DataObject.CURRENCYCODE = this.CURRENCYCODE;
            DataObject.NETPRICE = this.NETPRICE;
            DataObject.UNITCODE = this.UNITCODE;
            DataObject.BASEQUANTITY = this.BASEQUANTITY;
            DataObject.PODELIVERYDATE = this.PODELIVERYDATE;
            DataObject.CHANGEREQUESTEDDATE = this.CHANGEREQUESTEDDATE;
            DataObject.CUSTREQSHIPDATE = this.CUSTREQSHIPDATE;
            DataObject.DELIVERYPRIORITY = this.DELIVERYPRIORITY;
            DataObject.LINESCHEDULINGSTATUS = this.LINESCHEDULINGSTATUS;
            DataObject.QUANTITYCODE = this.QUANTITYCODE;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.COMPONENTID = this.COMPONENTID;
            DataObject.COMCUSTPRODID = this.COMCUSTPRODID;
            DataObject.COMSALESORDERLINEITEM = this.COMSALESORDERLINEITEM;
            DataObject.LINEUOM = this.LINEUOM;
            DataObject.COMPONENTQTY = this.COMPONENTQTY;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
            DataObject.MFLAG = this.MFLAG;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.TRANID = this.TRANID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            //DataObject.HEADERCREATIONDATE = this.HEADERCREATIONDATE;
            DataObject.SENDERID = this.SENDERID;
            DataObject.RECIPIENTID = this.RECIPIENTID;
            DataObject.REPLENISHMENTORDERID = this.REPLENISHMENTORDERID;
            DataObject.SHMENTORDERCREATIONDATE = this.SHMENTORDERCREATIONDATE;
            DataObject.POBILLTOCOMPANY = this.POBILLTOCOMPANY;
            DataObject.BILLTOCOMPANYNAME = this.BILLTOCOMPANYNAME;
            DataObject.SOLDBYORG = this.SOLDBYORG;
            DataObject.RMQQUOTENUMBER = this.RMQQUOTENUMBER;
            DataObject.RMQPONUMBER = this.RMQPONUMBER;
            DataObject.PODOCTYPE = this.PODOCTYPE;
            DataObject.POCHANGEINDICATOR = this.POCHANGEINDICATOR;
            //DataObject.LASTCHANGEDATE = this.LASTCHANGEDATE;
            DataObject.SALESORDERREFERENCEID = this.SALESORDERREFERENCEID;
            DataObject.PROCESSINGTYPECODE = this.PROCESSINGTYPECODE;
            DataObject.COMPLETEDELIVERY = this.COMPLETEDELIVERY;
            DataObject.SHIPPINGNOTE = this.SHIPPINGNOTE;
            DataObject.SHIPPMETHOD = this.SHIPPMETHOD;
            DataObject.SOFRTCARRIER = this.SOFRTCARRIER;
            DataObject.SPLPROCIND = this.SPLPROCIND;
            DataObject.SHIPTOID = this.SHIPTOID;
            DataObject.SALESORDERNUMBER = this.SALESORDERNUMBER;
            DataObject.HEADERSCHEDULINGSTATUS = this.HEADERSCHEDULINGSTATUS;
            DataObject.SALESPERSON = this.SALESPERSON;
            DataObject.SODATE = this.SODATE;
            DataObject.SHIPTOCOMPANY = this.SHIPTOCOMPANY;
            DataObject.SHIPTOCOUNTRYCODE = this.SHIPTOCOUNTRYCODE;
            DataObject.SHIPTOREGIONCODE = this.SHIPTOREGIONCODE;
            DataObject.SHIPTOSTREETPOSTALCODE = this.SHIPTOSTREETPOSTALCODE;
            DataObject.SHIPTOCITYNAME = this.SHIPTOCITYNAME;
            DataObject.SHIPTOSTREETNAME = this.SHIPTOSTREETNAME;
            DataObject.SHIPTOHOUSEID = this.SHIPTOHOUSEID;
            DataObject.SHIPTOCONTACTPHONE = this.SHIPTOCONTACTPHONE;
            DataObject.SHIPTODEVIATINGFULLNAME = this.SHIPTODEVIATINGFULLNAME;
            DataObject.SHIPTOEMAILURI = this.SHIPTOEMAILURI;
            DataObject.SHIPTOFAXL = this.SHIPTOFAXL;
            DataObject.CUSTOMERPONUMBER = this.CUSTOMERPONUMBER;
            DataObject.FREIGHTTERM = this.FREIGHTTERM;
            return DataObject;
        }
        public string INCO1
        {
            get
            {
                return (string)this["INCO1"];
            }
            set
            {
                this["INCO1"] = value;
            }
        }
        public string INCO2
        {
            get
            {
                return (string)this["INCO2"];
            }
            set
            {
                this["INCO2"] = value;
            }
        }
        public string SOLDTOID
        {
            get
            {
                return (string)this["SOLDTOID"];
            }
            set
            {
                this["SOLDTOID"] = value;
            }
        }
        public string SOLDTOCOMPANY
        {
            get
            {
                return (string)this["SOLDTOCOMPANY"];
            }
            set
            {
                this["SOLDTOCOMPANY"] = value;
            }
        }
        public string SOLDTOCOUNTRYCODE
        {
            get
            {
                return (string)this["SOLDTOCOUNTRYCODE"];
            }
            set
            {
                this["SOLDTOCOUNTRYCODE"] = value;
            }
        }
        public string SOLDTOREGIONCODE
        {
            get
            {
                return (string)this["SOLDTOREGIONCODE"];
            }
            set
            {
                this["SOLDTOREGIONCODE"] = value;
            }
        }
        public string SOLDTOSTREETPOSTALCODE
        {
            get
            {
                return (string)this["SOLDTOSTREETPOSTALCODE"];
            }
            set
            {
                this["SOLDTOSTREETPOSTALCODE"] = value;
            }
        }
        public string SOLDTOCITYNAME
        {
            get
            {
                return (string)this["SOLDTOCITYNAME"];
            }
            set
            {
                this["SOLDTOCITYNAME"] = value;
            }
        }
        public string SOLDTOSTREETNAME
        {
            get
            {
                return (string)this["SOLDTOSTREETNAME"];
            }
            set
            {
                this["SOLDTOSTREETNAME"] = value;
            }
        }
        public string SOLDTOHOUSEID
        {
            get
            {
                return (string)this["SOLDTOHOUSEID"];
            }
            set
            {
                this["SOLDTOHOUSEID"] = value;
            }
        }
        public string SOLDTOPERSONNAME
        {
            get
            {
                return (string)this["SOLDTOPERSONNAME"];
            }
            set
            {
                this["SOLDTOPERSONNAME"] = value;
            }
        }
        public string BILLTOID
        {
            get
            {
                return (string)this["BILLTOID"];
            }
            set
            {
                this["BILLTOID"] = value;
            }
        }
        public string BILLTOCOMPANY
        {
            get
            {
                return (string)this["BILLTOCOMPANY"];
            }
            set
            {
                this["BILLTOCOMPANY"] = value;
            }
        }
        public string BILLTOCOUNTRYCODE
        {
            get
            {
                return (string)this["BILLTOCOUNTRYCODE"];
            }
            set
            {
                this["BILLTOCOUNTRYCODE"] = value;
            }
        }
        public string BILLTOREGIONCODE
        {
            get
            {
                return (string)this["BILLTOREGIONCODE"];
            }
            set
            {
                this["BILLTOREGIONCODE"] = value;
            }
        }
        public string BILLTOSTREETPOSTALCODE
        {
            get
            {
                return (string)this["BILLTOSTREETPOSTALCODE"];
            }
            set
            {
                this["BILLTOSTREETPOSTALCODE"] = value;
            }
        }
        public string BILLTOCITYNAME
        {
            get
            {
                return (string)this["BILLTOCITYNAME"];
            }
            set
            {
                this["BILLTOCITYNAME"] = value;
            }
        }
        public string BILLTOSTREETNAME
        {
            get
            {
                return (string)this["BILLTOSTREETNAME"];
            }
            set
            {
                this["BILLTOSTREETNAME"] = value;
            }
        }
        public string BILLTOHOUSEID
        {
            get
            {
                return (string)this["BILLTOHOUSEID"];
            }
            set
            {
                this["BILLTOHOUSEID"] = value;
            }
        }
        public string BILLTOPERSONNAME
        {
            get
            {
                return (string)this["BILLTOPERSONNAME"];
            }
            set
            {
                this["BILLTOPERSONNAME"] = value;
            }
        }
        public string BUYERPARTYID
        {
            get
            {
                return (string)this["BUYERPARTYID"];
            }
            set
            {
                this["BUYERPARTYID"] = value;
            }
        }
        public string BUYERCOUNTRYCODE
        {
            get
            {
                return (string)this["BUYERCOUNTRYCODE"];
            }
            set
            {
                this["BUYERCOUNTRYCODE"] = value;
            }
        }
        public string BUYERREGIONCODE
        {
            get
            {
                return (string)this["BUYERREGIONCODE"];
            }
            set
            {
                this["BUYERREGIONCODE"] = value;
            }
        }
        public string BUYERSTREETPOSTALCODE
        {
            get
            {
                return (string)this["BUYERSTREETPOSTALCODE"];
            }
            set
            {
                this["BUYERSTREETPOSTALCODE"] = value;
            }
        }
        public string BUYERCITYNAME
        {
            get
            {
                return (string)this["BUYERCITYNAME"];
            }
            set
            {
                this["BUYERCITYNAME"] = value;
            }
        }
        public string BUYERSTREETNAME
        {
            get
            {
                return (string)this["BUYERSTREETNAME"];
            }
            set
            {
                this["BUYERSTREETNAME"] = value;
            }
        }
        public string BUYERDEVIATINGFULLNAME
        {
            get
            {
                return (string)this["BUYERDEVIATINGFULLNAME"];
            }
            set
            {
                this["BUYERDEVIATINGFULLNAME"] = value;
            }
        }
        public string BUYEREMAILURI
        {
            get
            {
                return (string)this["BUYEREMAILURI"];
            }
            set
            {
                this["BUYEREMAILURI"] = value;
            }
        }
        public string SELLERPARTYID
        {
            get
            {
                return (string)this["SELLERPARTYID"];
            }
            set
            {
                this["SELLERPARTYID"] = value;
            }
        }
        public string ECO_FCO
        {
            get
            {
                return (string)this["ECO_FCO"];
            }
            set
            {
                this["ECO_FCO"] = value;
            }
        }
        public string PAYMENTTERM
        {
            get
            {
                return (string)this["PAYMENTTERM"];
            }
            set
            {
                this["PAYMENTTERM"] = value;
            }
        }
        public string ACTIONCODE
        {
            get
            {
                return (string)this["ACTIONCODE"];
            }
            set
            {
                this["ACTIONCODE"] = value;
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
        public DateTime? PODELIVERYDATE
        {
            get
            {
                return (DateTime?)this["PODELIVERYDATE"];
            }
            set
            {
                this["PODELIVERYDATE"] = value;
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
        public string LINESCHEDULINGSTATUS
        {
            get
            {
                return (string)this["LINESCHEDULINGSTATUS"];
            }
            set
            {
                this["LINESCHEDULINGSTATUS"] = value;
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
        public string COMPONENTID
        {
            get
            {
                return (string)this["COMPONENTID"];
            }
            set
            {
                this["COMPONENTID"] = value;
            }
        }
        public string COMCUSTPRODID
        {
            get
            {
                return (string)this["COMCUSTPRODID"];
            }
            set
            {
                this["COMCUSTPRODID"] = value;
            }
        }
        public string COMSALESORDERLINEITEM
        {
            get
            {
                return (string)this["COMSALESORDERLINEITEM"];
            }
            set
            {
                this["COMSALESORDERLINEITEM"] = value;
            }
        }
        public string LINEUOM
        {
            get
            {
                return (string)this["LINEUOM"];
            }
            set
            {
                this["LINEUOM"] = value;
            }
        }
        public string COMPONENTQTY
        {
            get
            {
                return (string)this["COMPONENTQTY"];
            }
            set
            {
                this["COMPONENTQTY"] = value;
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
        public string MFLAG
        {
            get
            {
                return (string)this["MFLAG"];
            }
            set
            {
                this["MFLAG"] = value;
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
        public DateTime? HEADERCREATIONDATE
        {
            get
            {
                return (DateTime?)this["HEADERCREATIONDATE"];
            }
            set
            {
                this["HEADERCREATIONDATE"] = value;
            }
        }
        public string SENDERID
        {
            get
            {
                return (string)this["SENDERID"];
            }
            set
            {
                this["SENDERID"] = value;
            }
        }
        public string RECIPIENTID
        {
            get
            {
                return (string)this["RECIPIENTID"];
            }
            set
            {
                this["RECIPIENTID"] = value;
            }
        }
        public string REPLENISHMENTORDERID
        {
            get
            {
                return (string)this["REPLENISHMENTORDERID"];
            }
            set
            {
                this["REPLENISHMENTORDERID"] = value;
            }
        }
        public DateTime? SHMENTORDERCREATIONDATE
        {
            get
            {
                return (DateTime?)this["SHMENTORDERCREATIONDATE"];
            }
            set
            {
                this["SHMENTORDERCREATIONDATE"] = value;
            }
        }
        public string POBILLTOCOMPANY
        {
            get
            {
                return (string)this["POBILLTOCOMPANY"];
            }
            set
            {
                this["POBILLTOCOMPANY"] = value;
            }
        }
        public string BILLTOCOMPANYNAME
        {
            get
            {
                return (string)this["BILLTOCOMPANYNAME"];
            }
            set
            {
                this["BILLTOCOMPANYNAME"] = value;
            }
        }
        public string SOLDBYORG
        {
            get
            {
                return (string)this["SOLDBYORG"];
            }
            set
            {
                this["SOLDBYORG"] = value;
            }
        }
        public string RMQQUOTENUMBER
        {
            get
            {
                return (string)this["RMQQUOTENUMBER"];
            }
            set
            {
                this["RMQQUOTENUMBER"] = value;
            }
        }
        public string RMQPONUMBER
        {
            get
            {
                return (string)this["RMQPONUMBER"];
            }
            set
            {
                this["RMQPONUMBER"] = value;
            }
        }
        public string PODOCTYPE
        {
            get
            {
                return (string)this["PODOCTYPE"];
            }
            set
            {
                this["PODOCTYPE"] = value;
            }
        }
        public string POCHANGEINDICATOR
        {
            get
            {
                return (string)this["POCHANGEINDICATOR"];
            }
            set
            {
                this["POCHANGEINDICATOR"] = value;
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
        public string SALESORDERREFERENCEID
        {
            get
            {
                return (string)this["SALESORDERREFERENCEID"];
            }
            set
            {
                this["SALESORDERREFERENCEID"] = value;
            }
        }
        public string PROCESSINGTYPECODE
        {
            get
            {
                return (string)this["PROCESSINGTYPECODE"];
            }
            set
            {
                this["PROCESSINGTYPECODE"] = value;
            }
        }
        public string COMPLETEDELIVERY
        {
            get
            {
                return (string)this["COMPLETEDELIVERY"];
            }
            set
            {
                this["COMPLETEDELIVERY"] = value;
            }
        }
        public string SHIPPINGNOTE
        {
            get
            {
                return (string)this["SHIPPINGNOTE"];
            }
            set
            {
                this["SHIPPINGNOTE"] = value;
            }
        }
        public string SHIPPMETHOD
        {
            get
            {
                return (string)this["SHIPPMETHOD"];
            }
            set
            {
                this["SHIPPMETHOD"] = value;
            }
        }
        public string SOFRTCARRIER
        {
            get
            {
                return (string)this["SOFRTCARRIER"];
            }
            set
            {
                this["SOFRTCARRIER"] = value;
            }
        }
        public string SPLPROCIND
        {
            get
            {
                return (string)this["SPLPROCIND"];
            }
            set
            {
                this["SPLPROCIND"] = value;
            }
        }
        public string SHIPTOID
        {
            get
            {
                return (string)this["SHIPTOID"];
            }
            set
            {
                this["SHIPTOID"] = value;
            }
        }
        public string SALESORDERNUMBER
        {
            get
            {
                return (string)this["SALESORDERNUMBER"];
            }
            set
            {
                this["SALESORDERNUMBER"] = value;
            }
        }
        public string HEADERSCHEDULINGSTATUS
        {
            get
            {
                return (string)this["HEADERSCHEDULINGSTATUS"];
            }
            set
            {
                this["HEADERSCHEDULINGSTATUS"] = value;
            }
        }
        public string SALESPERSON
        {
            get
            {
                return (string)this["SALESPERSON"];
            }
            set
            {
                this["SALESPERSON"] = value;
            }
        }
        public DateTime? SODATE
        {
            get
            {
                return (DateTime?)this["SODATE"];
            }
            set
            {
                this["SODATE"] = value;
            }
        }
        public string SHIPTOCOMPANY
        {
            get
            {
                return (string)this["SHIPTOCOMPANY"];
            }
            set
            {
                this["SHIPTOCOMPANY"] = value;
            }
        }
        public string SHIPTOCOUNTRYCODE
        {
            get
            {
                return (string)this["SHIPTOCOUNTRYCODE"];
            }
            set
            {
                this["SHIPTOCOUNTRYCODE"] = value;
            }
        }
        public string SHIPTOREGIONCODE
        {
            get
            {
                return (string)this["SHIPTOREGIONCODE"];
            }
            set
            {
                this["SHIPTOREGIONCODE"] = value;
            }
        }
        public string SHIPTOSTREETPOSTALCODE
        {
            get
            {
                return (string)this["SHIPTOSTREETPOSTALCODE"];
            }
            set
            {
                this["SHIPTOSTREETPOSTALCODE"] = value;
            }
        }
        public string SHIPTOCITYNAME
        {
            get
            {
                return (string)this["SHIPTOCITYNAME"];
            }
            set
            {
                this["SHIPTOCITYNAME"] = value;
            }
        }
        public string SHIPTOSTREETNAME
        {
            get
            {
                return (string)this["SHIPTOSTREETNAME"];
            }
            set
            {
                this["SHIPTOSTREETNAME"] = value;
            }
        }
        public string SHIPTOHOUSEID
        {
            get
            {
                return (string)this["SHIPTOHOUSEID"];
            }
            set
            {
                this["SHIPTOHOUSEID"] = value;
            }
        }
        public string SHIPTOCONTACTPHONE
        {
            get
            {
                return (string)this["SHIPTOCONTACTPHONE"];
            }
            set
            {
                this["SHIPTOCONTACTPHONE"] = value;
            }
        }
        public string SHIPTODEVIATINGFULLNAME
        {
            get
            {
                return (string)this["SHIPTODEVIATINGFULLNAME"];
            }
            set
            {
                this["SHIPTODEVIATINGFULLNAME"] = value;
            }
        }
        public string SHIPTOEMAILURI
        {
            get
            {
                return (string)this["SHIPTOEMAILURI"];
            }
            set
            {
                this["SHIPTOEMAILURI"] = value;
            }
        }
        public string SHIPTOFAXL
        {
            get
            {
                return (string)this["SHIPTOFAXL"];
            }
            set
            {
                this["SHIPTOFAXL"] = value;
            }
        }
        public string CUSTOMERPONUMBER
        {
            get
            {
                return (string)this["CUSTOMERPONUMBER"];
            }
            set
            {
                this["CUSTOMERPONUMBER"] = value;
            }
        }
        public string FREIGHTTERM
        {
            get
            {
                return (string)this["FREIGHTTERM"];
            }
            set
            {
                this["FREIGHTTERM"] = value;
            }
        }
    }
    public class R_I137
    {
        public string INCO1 { get; set; }
        public string INCO2 { get; set; }
        public string SOLDTOID { get; set; }
        public string SOLDTOCOMPANY { get; set; }
        public string SOLDTOCOUNTRYCODE { get; set; }
        public string SOLDTOREGIONCODE { get; set; }
        public string SOLDTOSTREETPOSTALCODE { get; set; }
        public string SOLDTOCITYNAME { get; set; }
        public string SOLDTOSTREETNAME { get; set; }
        public string SOLDTOHOUSEID { get; set; }
        public string SOLDTOPERSONNAME { get; set; }
        public string BILLTOID { get; set; }
        public string BILLTOCOMPANY { get; set; }
        public string BILLTOCOUNTRYCODE { get; set; }
        public string BILLTOREGIONCODE { get; set; }
        public string BILLTOSTREETPOSTALCODE { get; set; }
        public string BILLTOCITYNAME { get; set; }
        public string BILLTOSTREETNAME { get; set; }
        public string BILLTOHOUSEID { get; set; }
        public string BILLTOPERSONNAME { get; set; }
        public string BUYERPARTYID { get; set; }
        public string BUYERCOUNTRYCODE { get; set; }
        public string BUYERREGIONCODE { get; set; }
        public string BUYERSTREETPOSTALCODE { get; set; }
        public string BUYERCITYNAME { get; set; }
        public string BUYERSTREETNAME { get; set; }
        public string BUYERDEVIATINGFULLNAME { get; set; }
        public string BUYEREMAILURI { get; set; }
        public string SELLERPARTYID { get; set; }
        public string ECO_FCO { get; set; }
        public string PAYMENTTERM { get; set; }
        public string ACTIONCODE { get; set; }
        public string ITEM { get; set; }
        public string PARENTITEMID { get; set; }
        public string ITEMCHANGEINDICATOR { get; set; }
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
        public string CURRENCYCODE { get; set; }
        public string NETPRICE { get; set; }
        public string UNITCODE { get; set; }
        public string BASEQUANTITY { get; set; }
        public DateTime? PODELIVERYDATE { get; set; }
        public string CHANGEREQUESTEDDATE { get; set; }
        public DateTime? CUSTREQSHIPDATE { get; set; }
        public string DELIVERYPRIORITY { get; set; }
        public string LINESCHEDULINGSTATUS { get; set; }
        public string QUANTITYCODE { get; set; }
        public string QUANTITY { get; set; }
        public string COMPONENTID { get; set; }
        public string COMCUSTPRODID { get; set; }
        public string COMSALESORDERLINEITEM { get; set; }
        public string LINEUOM { get; set; }
        public string COMPONENTQTY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public string MFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? HEADERCREATIONDATETIME { get; set; }
        public string SENDERID { get; set; }
        public string RECIPIENTID { get; set; }
        public string REPLENISHMENTORDERID { get; set; }
        public DateTime? SHMENTORDERCREATIONDATE { get; set; }
        public string POBILLTOCOMPANY { get; set; }
        public string BILLTOCOMPANYNAME { get; set; }
        public string SOLDBYORG { get; set; }
        public string RMQQUOTENUMBER { get; set; }
        public string RMQPONUMBER { get; set; }
        public string PODOCTYPE { get; set; }
        public string POCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
        public string SALESORDERREFERENCEID { get; set; }
        public string PROCESSINGTYPECODE { get; set; }
        public string COMPLETEDELIVERY { get; set; }
        public string SHIPPINGNOTE { get; set; }
        public string SHIPPMETHOD { get; set; }
        public string SOFRTCARRIER { get; set; }
        public string SPLPROCIND { get; set; }
        public string SHIPTOID { get; set; }
        public string SALESORDERNUMBER { get; set; }
        public string HEADERSCHEDULINGSTATUS { get; set; }
        public string SALESPERSON { get; set; }
        public DateTime? SODATE { get; set; }
        public string SHIPTOCOMPANY { get; set; }
        public string SHIPTOCOUNTRYCODE { get; set; }
        public string SHIPTOREGIONCODE { get; set; }
        public string SHIPTOSTREETPOSTALCODE { get; set; }
        public string SHIPTOCITYNAME { get; set; }
        public string SHIPTOSTREETNAME { get; set; }
        public string SHIPTOHOUSEID { get; set; }
        public string SHIPTOCONTACTPHONE { get; set; }
        public string SHIPTODEVIATINGFULLNAME { get; set; }
        public string SHIPTOEMAILURI { get; set; }
        public string SHIPTOFAXL { get; set; }
        public string CUSTOMERPONUMBER { get; set; }
        public string FREIGHTTERM { get; set; }
    }

    [SqlSugar.SugarTable("jnp.tb_i137")]
    public class B2B_R_I137
    {
        public string INCO1 { get; set; }
        public string INCO2 { get; set; }
        public string SOLDTOID { get; set; }
        public string SOLDTOCOMPANY { get; set; }
        public string SOLDTOCOUNTRYCODE { get; set; }
        public string SOLDTOREGIONCODE { get; set; }
        public string SOLDTOSTREETPOSTALCODE { get; set; }
        public string SOLDTOCITYNAME { get; set; }
        public string SOLDTOSTREETNAME { get; set; }
        public string SOLDTOHOUSEID { get; set; }
        public string SOLDTOPERSONNAME { get; set; }
        public string BILLTOID { get; set; }
        public string BILLTOCOMPANY { get; set; }
        public string BILLTOCOUNTRYCODE { get; set; }
        public string BILLTOREGIONCODE { get; set; }
        public string BILLTOSTREETPOSTALCODE { get; set; }
        public string BILLTOCITYNAME { get; set; }
        public string BILLTOSTREETNAME { get; set; }
        public string BILLTOHOUSEID { get; set; }
        public string BILLTOPERSONNAME { get; set; }
        public string BUYERPARTYID { get; set; }
        public string BUYERCOUNTRYCODE { get; set; }
        public string BUYERREGIONCODE { get; set; }
        public string BUYERSTREETPOSTALCODE { get; set; }
        public string BUYERCITYNAME { get; set; }
        public string BUYERSTREETNAME { get; set; }
        public string BUYERDEVIATINGFULLNAME { get; set; }
        public string BUYEREMAILURI { get; set; }
        public string SELLERPARTYID { get; set; }
        public string ECO_FCO { get; set; }
        public string PAYMENTTERM { get; set; }
        public string ACTIONCODE { get; set; }
        public string ITEM { get; set; }
        public string PARENTITEMID { get; set; }
        public string ITEMCHANGEINDICATOR { get; set; }
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
        public string CURRENCYCODE { get; set; }
        public string NETPRICE { get; set; }
        public string UNITCODE { get; set; }
        public string BASEQUANTITY { get; set; }
        public DateTime? PODELIVERYDATE { get; set; }
        public string CHANGEREQUESTEDDATE { get; set; }
        public DateTime? CUSTREQSHIPDATE { get; set; }
        public string DELIVERYPRIORITY { get; set; }
        public string LINESCHEDULINGSTATUS { get; set; }
        public string QUANTITYCODE { get; set; }
        public string QUANTITY { get; set; }
        public string COMPONENTID { get; set; }
        public string COMCUSTPRODID { get; set; }
        public string COMSALESORDERLINEITEM { get; set; }
        public string LINEUOM { get; set; }
        public string COMPONENTQTY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public string F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? HEADERCREATIONDATETIME { get; set; }
        public string SENDERID { get; set; }
        public string RECIPIENTID { get; set; }
        public string REPLENISHMENTORDERID { get; set; }
        public DateTime? SHMENTORDERCREATIONDATE { get; set; }
        public string POBILLTOCOMPANY { get; set; }
        public string BILLTOCOMPANYNAME { get; set; }
        public string SOLDBYORG { get; set; }
        public string RMQQUOTENUMBER { get; set; }
        public string RMQPONUMBER { get; set; }
        public string PODOCTYPE { get; set; }
        public string POCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
        public string SALESORDERREFERENCEID { get; set; }
        public string PROCESSINGTYPECODE { get; set; }
        public string COMPLETEDELIVERY { get; set; }
        public string SHIPPINGNOTE { get; set; }
        public string SHIPPMETHOD { get; set; }
        public string SOFRTCARRIER { get; set; }
        public string SPLPROCIND { get; set; }
        public string SHIPTOID { get; set; }
        public string SALESORDERNUMBER { get; set; }
        public string HEADERSCHEDULINGSTATUS { get; set; }
        public string SALESPERSON { get; set; }
        public DateTime? SODATE { get; set; }
        public string SHIPTOCOMPANY { get; set; }
        public string SHIPTOCOUNTRYCODE { get; set; }
        public string SHIPTOREGIONCODE { get; set; }
        public string SHIPTOSTREETPOSTALCODE { get; set; }
        public string SHIPTOCITYNAME { get; set; }
        public string SHIPTOSTREETNAME { get; set; }
        public string SHIPTOHOUSEID { get; set; }
        public string SHIPTOCONTACTPHONE { get; set; }
        public string SHIPTODEVIATINGFULLNAME { get; set; }
        public string SHIPTOEMAILURI { get; set; }
        public string SHIPTOFAXL { get; set; }
        public string CUSTOMERPONUMBER { get; set; }
        public string FREIGHTTERM { get; set; }
    }

}