using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_PO_HEAD : DataObjectTable
    {
        public T_PO_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_PO_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_PO_HEAD);
            TableName = "PO_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_PO_HEAD : DataObjectBase
    {
        public Row_PO_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public PO_HEAD GetDataObject()
        {
            PO_HEAD DataObject = new PO_HEAD();
            DataObject.FLAG = this.FLAG;
            DataObject.PLANT = this.PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.BILLTOCOMPANY = this.BILLTOCOMPANY;
            DataObject.BILLTOCOMPANYNAME = this.BILLTOCOMPANYNAME;
            DataObject.BUYERCITY = this.BUYERCITY;
            DataObject.BUYERCOUNTRYCODE = this.BUYERCOUNTRYCODE;
            DataObject.BUYERREGIONCODE = this.BUYERREGIONCODE;
            DataObject.BUYERSTREET = this.BUYERSTREET;
            DataObject.BUYERPOSTALCODE = this.BUYERPOSTALCODE;
            DataObject.BUYER = this.BUYER;
            DataObject.CASHDISCOUNTTERMS = this.CASHDISCOUNTTERMS;
            DataObject.CHANGEINDICATOR = this.CHANGEINDICATOR;
            DataObject.CREATIONDATETIME = this.CREATIONDATETIME;
            DataObject.PO = this.PO;
            DataObject.LASTCHANGEDATETIME = this.LASTCHANGEDATETIME;
            DataObject.PODOCTYPE = this.PODOCTYPE;
            DataObject.ORDERREASON = this.ORDERREASON;
            DataObject.SUPPLIER = this.SUPPLIER;
            DataObject.SHIPPINGNOTE = this.SHIPPINGNOTE;
            DataObject.SHIPPMETHOD = this.SHIPPMETHOD;
            DataObject.SOFRTCARRIER = this.SOFRTCARRIER;
            DataObject.SPLPROCIND = this.SPLPROCIND;
            DataObject.SHIPTONAME = this.SHIPTONAME;
            DataObject.SHIPCITYNAME = this.SHIPCITYNAME;
            DataObject.SHIPPHONE = this.SHIPPHONE;
            DataObject.SHIPCOUNTRYCODE = this.SHIPCOUNTRYCODE;
            DataObject.SHIPHOUSEID = this.SHIPHOUSEID;
            DataObject.SHIPREGIONCODE = this.SHIPREGIONCODE;
            DataObject.SHIPSTREETNAME = this.SHIPSTREETNAME;
            DataObject.SHIPSTREETPOSTALCODE = this.SHIPSTREETPOSTALCODE;
            DataObject.SHIPCONTACTNAME = this.SHIPCONTACTNAME;
            DataObject.SHIPEMAIL = this.SHIPEMAIL;
            DataObject.SHIPFAX = this.SHIPFAX;
            DataObject.CUSTOMERPONUMBER = this.CUSTOMERPONUMBER;
            DataObject.FREIGHTTERM = this.FREIGHTTERM;
            DataObject.INCO1 = this.INCO1;
            DataObject.INCO2 = this.INCO2;
            DataObject.SHIPTOID = this.SHIPTOID;
            DataObject.SALESORDERNUMBER = this.SALESORDERNUMBER;
            DataObject.SALESPERSON = this.SALESPERSON;
            DataObject.SODATE = this.SODATE;
            DataObject.COMPLETEDELIVERY = this.COMPLETEDELIVERY;
            DataObject.SOLDTOCUSTOMERNAME = this.SOLDTOCUSTOMERNAME;
            DataObject.SOLDTOCITYNAME = this.SOLDTOCITYNAME;
            DataObject.SOLDTOCOUNTRYCODE = this.SOLDTOCOUNTRYCODE;
            DataObject.SOLDTOHOUSEID = this.SOLDTOHOUSEID;
            DataObject.SOLDTOREGIONCODE = this.SOLDTOREGIONCODE;
            DataObject.SOLDTOSTREETPOSTALCODE = this.SOLDTOSTREETPOSTALCODE;
            DataObject.SOLDTOCUSTOMERNUMBER = this.SOLDTOCUSTOMERNUMBER;
            DataObject.SCHEDULINGSTATUS = this.SCHEDULINGSTATUS;
            DataObject.RMQQUOTE = this.RMQQUOTE;
            DataObject.RMQPO = this.RMQPO;
            DataObject.SOLDTOSTREETNAME = this.SOLDTOSTREETNAME;
            return DataObject;
        }
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
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
        public string BUYERCITY
        {
            get
            {
                return (string)this["BUYERCITY"];
            }
            set
            {
                this["BUYERCITY"] = value;
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
        public string BUYERSTREET
        {
            get
            {
                return (string)this["BUYERSTREET"];
            }
            set
            {
                this["BUYERSTREET"] = value;
            }
        }
        public string BUYERPOSTALCODE
        {
            get
            {
                return (string)this["BUYERPOSTALCODE"];
            }
            set
            {
                this["BUYERPOSTALCODE"] = value;
            }
        }
        public string BUYER
        {
            get
            {
                return (string)this["BUYER"];
            }
            set
            {
                this["BUYER"] = value;
            }
        }
        public string CASHDISCOUNTTERMS
        {
            get
            {
                return (string)this["CASHDISCOUNTTERMS"];
            }
            set
            {
                this["CASHDISCOUNTTERMS"] = value;
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
        public string CREATIONDATETIME
        {
            get
            {
                return (string)this["CREATIONDATETIME"];
            }
            set
            {
                this["CREATIONDATETIME"] = value;
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
        public string LASTCHANGEDATETIME
        {
            get
            {
                return (string)this["LASTCHANGEDATETIME"];
            }
            set
            {
                this["LASTCHANGEDATETIME"] = value;
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
        public string ORDERREASON
        {
            get
            {
                return (string)this["ORDERREASON"];
            }
            set
            {
                this["ORDERREASON"] = value;
            }
        }
        public string SUPPLIER
        {
            get
            {
                return (string)this["SUPPLIER"];
            }
            set
            {
                this["SUPPLIER"] = value;
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
        public string SHIPTONAME
        {
            get
            {
                return (string)this["SHIPTONAME"];
            }
            set
            {
                this["SHIPTONAME"] = value;
            }
        }
        public string SHIPCITYNAME
        {
            get
            {
                return (string)this["SHIPCITYNAME"];
            }
            set
            {
                this["SHIPCITYNAME"] = value;
            }
        }
        public string SHIPPHONE
        {
            get
            {
                return (string)this["SHIPPHONE"];
            }
            set
            {
                this["SHIPPHONE"] = value;
            }
        }
        public string SHIPCOUNTRYCODE
        {
            get
            {
                return (string)this["SHIPCOUNTRYCODE"];
            }
            set
            {
                this["SHIPCOUNTRYCODE"] = value;
            }
        }
        public string SHIPHOUSEID
        {
            get
            {
                return (string)this["SHIPHOUSEID"];
            }
            set
            {
                this["SHIPHOUSEID"] = value;
            }
        }
        public string SHIPREGIONCODE
        {
            get
            {
                return (string)this["SHIPREGIONCODE"];
            }
            set
            {
                this["SHIPREGIONCODE"] = value;
            }
        }
        public string SHIPSTREETNAME
        {
            get
            {
                return (string)this["SHIPSTREETNAME"];
            }
            set
            {
                this["SHIPSTREETNAME"] = value;
            }
        }
        public string SHIPSTREETPOSTALCODE
        {
            get
            {
                return (string)this["SHIPSTREETPOSTALCODE"];
            }
            set
            {
                this["SHIPSTREETPOSTALCODE"] = value;
            }
        }
        public string SHIPCONTACTNAME
        {
            get
            {
                return (string)this["SHIPCONTACTNAME"];
            }
            set
            {
                this["SHIPCONTACTNAME"] = value;
            }
        }
        public string SHIPEMAIL
        {
            get
            {
                return (string)this["SHIPEMAIL"];
            }
            set
            {
                this["SHIPEMAIL"] = value;
            }
        }
        public string SHIPFAX
        {
            get
            {
                return (string)this["SHIPFAX"];
            }
            set
            {
                this["SHIPFAX"] = value;
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
        public string SODATE
        {
            get
            {
                return (string)this["SODATE"];
            }
            set
            {
                this["SODATE"] = value;
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
        public string SOLDTOCUSTOMERNAME
        {
            get
            {
                return (string)this["SOLDTOCUSTOMERNAME"];
            }
            set
            {
                this["SOLDTOCUSTOMERNAME"] = value;
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
        public string SOLDTOCUSTOMERNUMBER
        {
            get
            {
                return (string)this["SOLDTOCUSTOMERNUMBER"];
            }
            set
            {
                this["SOLDTOCUSTOMERNUMBER"] = value;
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
        public string RMQQUOTE
        {
            get
            {
                return (string)this["RMQQUOTE"];
            }
            set
            {
                this["RMQQUOTE"] = value;
            }
        }
        public string RMQPO
        {
            get
            {
                return (string)this["RMQPO"];
            }
            set
            {
                this["RMQPO"] = value;
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
    }
    public class PO_HEAD
    {
        public string FLAG { get; set; }
        public string PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string BILLTOCOMPANY { get; set; }
        public string BILLTOCOMPANYNAME { get; set; }
        public string BUYERCITY { get; set; }
        public string BUYERCOUNTRYCODE { get; set; }
        public string BUYERREGIONCODE { get; set; }
        public string BUYERSTREET { get; set; }
        public string BUYERPOSTALCODE { get; set; }
        public string BUYER { get; set; }
        public string CASHDISCOUNTTERMS { get; set; }
        public string CHANGEINDICATOR { get; set; }
        public string CREATIONDATETIME { get; set; }
        public string PO { get; set; }
        public string LASTCHANGEDATETIME { get; set; }
        public string PODOCTYPE { get; set; }
        public string ORDERREASON { get; set; }
        public string SUPPLIER { get; set; }
        public string SHIPPINGNOTE { get; set; }
        public string SHIPPMETHOD { get; set; }
        public string SOFRTCARRIER { get; set; }
        public string SPLPROCIND { get; set; }
        public string SHIPTONAME { get; set; }
        public string SHIPCITYNAME { get; set; }
        public string SHIPPHONE { get; set; }
        public string SHIPCOUNTRYCODE { get; set; }
        public string SHIPHOUSEID { get; set; }
        public string SHIPREGIONCODE { get; set; }
        public string SHIPSTREETNAME { get; set; }
        public string SHIPSTREETPOSTALCODE { get; set; }
        public string SHIPCONTACTNAME { get; set; }
        public string SHIPEMAIL { get; set; }
        public string SHIPFAX { get; set; }
        public string CUSTOMERPONUMBER { get; set; }
        public string FREIGHTTERM { get; set; }
        public string INCO1 { get; set; }
        public string INCO2 { get; set; }
        public string SHIPTOID { get; set; }
        public string SALESORDERNUMBER { get; set; }
        public string SALESPERSON { get; set; }
        public string SODATE { get; set; }
        public string COMPLETEDELIVERY { get; set; }
        public string SOLDTOCUSTOMERNAME { get; set; }
        public string SOLDTOCITYNAME { get; set; }
        public string SOLDTOCOUNTRYCODE { get; set; }
        public string SOLDTOHOUSEID { get; set; }
        public string SOLDTOREGIONCODE { get; set; }
        public string SOLDTOSTREETPOSTALCODE { get; set; }
        public string SOLDTOCUSTOMERNUMBER { get; set; }
        public string SCHEDULINGSTATUS { get; set; }
        public string RMQQUOTE { get; set; }
        public string RMQPO { get; set; }
        public string SOLDTOSTREETNAME { get; set; }
    }
}