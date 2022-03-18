using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
namespace MESDataObject.Module.Juniper
{
    public class T_R_I137_HEAD : DataObjectTable
    {
        public T_R_I137_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I137_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I137_HEAD);
            TableName = "R_I137_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I137_HEAD : DataObjectBase
    {
        public Row_R_I137_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_I137_HEAD GetDataObject()
        {
            R_I137_HEAD DataObject = new R_I137_HEAD();
            DataObject.SOLDTOHOUSEID = this.SOLDTOHOUSEID;
            DataObject.SOLDTOPERSONNAME = this.SOLDTOPERSONNAME;
            DataObject.BILLTOID = this.BILLTOID;
            DataObject.BILLTOADDRESS = this.BILLTOADDRESS;
            DataObject.BILLTOCOUNTRYCODE = this.BILLTOCOUNTRYCODE;
            DataObject.BILLTOREGIONCODE = this.BILLTOREGIONCODE;
            DataObject.BILLTOSTREETPOSTALCODE = this.BILLTOSTREETPOSTALCODE;
            DataObject.BILLTOCITYNAME = this.BILLTOCITYNAME;
            DataObject.BILLTOSTREETNAME = this.BILLTOSTREETNAME;
            DataObject.BILLTOHOUSEID = this.BILLTOHOUSEID;
            DataObject.BILLTOPERSONNAME = this.BILLTOPERSONNAME;
            DataObject.BUYERPARTYAGENCYID = this.BUYERPARTYAGENCYID;
            DataObject.BUYERPARTYID = this.BUYERPARTYID;
            DataObject.BUYERCOUNTRYCODE = this.BUYERCOUNTRYCODE;
            DataObject.BUYERREGIONCODE = this.BUYERREGIONCODE;
            DataObject.BUYERSTREETPOSTALCODE = this.BUYERSTREETPOSTALCODE;
            DataObject.BUYERCITYNAME = this.BUYERCITYNAME;
            DataObject.BUYERSTREETNAME = this.BUYERSTREETNAME;
            DataObject.BUYERDEVIATINGFULLNAME = this.BUYERDEVIATINGFULLNAME;
            DataObject.BUYEREMAILURI = this.BUYEREMAILURI;
            DataObject.BUYERURIDEFAULTINDICATOR = this.BUYERURIDEFAULTINDICATOR;
            DataObject.SELLERPARTYAGENCYID = this.SELLERPARTYAGENCYID;
            DataObject.SELLERPARTYID = this.SELLERPARTYID;
            DataObject.BUYERREFERENCENOTE = this.BUYERREFERENCENOTE;
            DataObject.DESCRI = this.DESCRI;
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
            DataObject.HEADERCREATIONDATE = this.HEADERCREATIONDATE;
            DataObject.SENDERID = this.SENDERID;
            DataObject.RECIPIENTID = this.RECIPIENTID;
            DataObject.PONUMBER = this.PONUMBER;
            DataObject.SHMENTORDERCREATIONDATE = this.SHMENTORDERCREATIONDATE;
            DataObject.BILLTOCOMPANY = this.BILLTOCOMPANY;
            DataObject.BILLTOCOMPANYNAME = this.BILLTOCOMPANYNAME;
            DataObject.SOLDBYORG = this.SOLDBYORG;
            DataObject.RMQQUOTENUMBER = this.RMQQUOTENUMBER;
            DataObject.RMQPONUMBER = this.RMQPONUMBER;
            DataObject.PODOCTYPE = this.PODOCTYPE;
            DataObject.POCHANGEINDICATOR = this.POCHANGEINDICATOR;
            DataObject.LASTCHANGEDATE = this.LASTCHANGEDATE;
            DataObject.SALESORDERREFERENCEID = this.SALESORDERREFERENCEID;
            DataObject.PROCESSINGTYPECODE = this.PROCESSINGTYPECODE;
            DataObject.SODELCOMPL = this.SODELCOMPL;
            DataObject.SHIPPINGNOTE = this.SHIPPINGNOTE;
            DataObject.SHIPPMETHOD = this.SHIPPMETHOD;
            DataObject.SOFRTCARRIER = this.SOFRTCARRIER;
            DataObject.SPLPROCIND = this.SPLPROCIND;
            DataObject.SHIPTOID = this.SHIPTOID;
            DataObject.SALESORDERNUMBER = this.SALESORDERNUMBER;
            DataObject.HEADERSCHEDULINGSTATUS = this.HEADERSCHEDULINGSTATUS;
            DataObject.SALESPERSON = this.SALESPERSON;
            DataObject.SODATE = this.SODATE;
            DataObject.SHIPTOADDRESS = this.SHIPTOADDRESS;
            DataObject.SHIPTOCOUNTRYCODE = this.SHIPTOCOUNTRYCODE;
            DataObject.SHIPTOREGIONCODE = this.SHIPTOREGIONCODE;
            DataObject.SHIPTOSTREETPOSTALCODE = this.SHIPTOSTREETPOSTALCODE;
            DataObject.SHIPTOCITYNAME = this.SHIPTOCITYNAME;
            DataObject.SHIPTOSTREETNAME = this.SHIPTOSTREETNAME;
            DataObject.SHIPTOHOUSEID = this.SHIPTOHOUSEID;
            DataObject.SHIPTOCONTACTPHONE = this.SHIPTOCONTACTPHONE;
            DataObject.SHIPTODEVIATINGFULLNAME = this.SHIPTODEVIATINGFULLNAME;
            DataObject.SHIPTOEMAILURI = this.SHIPTOEMAILURI;
            DataObject.SHIPTOURIDEFAULTINDICATOR = this.SHIPTOURIDEFAULTINDICATOR;
            DataObject.SHIPTOFAXL = this.SHIPTOFAXL;
            DataObject.CUSTOMERPONUMBER = this.CUSTOMERPONUMBER;
            DataObject.FREIGHTTERM = this.FREIGHTTERM;
            DataObject.INCO1 = this.INCO1;
            DataObject.INCO2 = this.INCO2;
            DataObject.SOLDTOID = this.SOLDTOID;
            DataObject.SOLDTOADDRESS = this.SOLDTOADDRESS;
            DataObject.SOLDTOCOUNTRYCODE = this.SOLDTOCOUNTRYCODE;
            DataObject.SOLDTOREGIONCODE = this.SOLDTOREGIONCODE;
            DataObject.SOLDTOSTREETPOSTALCODE = this.SOLDTOSTREETPOSTALCODE;
            DataObject.SOLDTOCITYNAME = this.SOLDTOCITYNAME;
            DataObject.SOLDTOSTREETNAME = this.SOLDTOSTREETNAME;
            return DataObject;
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
        public string BILLTOADDRESS
        {
            get
            {
                return (string)this["BILLTOADDRESS"];
            }
            set
            {
                this["BILLTOADDRESS"] = value;
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
        public string BUYERPARTYAGENCYID
        {
            get
            {
                return (string)this["BUYERPARTYAGENCYID"];
            }
            set
            {
                this["BUYERPARTYAGENCYID"] = value;
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
        public string BUYERURIDEFAULTINDICATOR
        {
            get
            {
                return (string)this["BUYERURIDEFAULTINDICATOR"];
            }
            set
            {
                this["BUYERURIDEFAULTINDICATOR"] = value;
            }
        }
        public string SELLERPARTYAGENCYID
        {
            get
            {
                return (string)this["SELLERPARTYAGENCYID"];
            }
            set
            {
                this["SELLERPARTYAGENCYID"] = value;
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
        public string BUYERREFERENCENOTE
        {
            get
            {
                return (string)this["BUYERREFERENCENOTE"];
            }
            set
            {
                this["BUYERREFERENCENOTE"] = value;
            }
        }
        public string DESCRI
        {
            get
            {
                return (string)this["DESCRI"];
            }
            set
            {
                this["DESCRI"] = value;
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
        public string SODELCOMPL
        {
            get
            {
                return (string)this["SODELCOMPL"];
            }
            set
            {
                this["SODELCOMPL"] = value;
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
        public string SHIPTOADDRESS
        {
            get
            {
                return (string)this["SHIPTOADDRESS"];
            }
            set
            {
                this["SHIPTOADDRESS"] = value;
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
        public string SHIPTOURIDEFAULTINDICATOR
        {
            get
            {
                return (string)this["SHIPTOURIDEFAULTINDICATOR"];
            }
            set
            {
                this["SHIPTOURIDEFAULTINDICATOR"] = value;
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
        public string SOLDTOADDRESS
        {
            get
            {
                return (string)this["SOLDTOADDRESS"];
            }
            set
            {
                this["SOLDTOADDRESS"] = value;
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
    }
    public class R_I137_HEAD
    {
        public string SOLDTOHOUSEID { get; set; }
        public string SOLDTOPERSONNAME { get; set; }
        public string BILLTOID { get; set; }
        public string BILLTOADDRESS { get; set; }
        public string BILLTOCOUNTRYCODE { get; set; }
        public string BILLTOREGIONCODE { get; set; }
        public string BILLTOSTREETPOSTALCODE { get; set; }
        public string BILLTOCITYNAME { get; set; }
        public string BILLTOSTREETNAME { get; set; }
        public string BILLTOHOUSEID { get; set; }
        public string BILLTOPERSONNAME { get; set; }
        public string BUYERPARTYAGENCYID { get; set; }
        public string BUYERPARTYID { get; set; }
        public string BUYERCOUNTRYCODE { get; set; }
        public string BUYERREGIONCODE { get; set; }
        public string BUYERSTREETPOSTALCODE { get; set; }
        public string BUYERCITYNAME { get; set; }
        public string BUYERSTREETNAME { get; set; }
        public string BUYERDEVIATINGFULLNAME { get; set; }
        public string BUYEREMAILURI { get; set; }
        public string BUYERURIDEFAULTINDICATOR { get; set; }
        public string SELLERPARTYAGENCYID { get; set; }
        public string SELLERPARTYID { get; set; }
        public string BUYERREFERENCENOTE { get; set; }
        public string DESCRI { get; set; }
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
        public DateTime? HEADERCREATIONDATE { get; set; }
        public string SENDERID { get; set; }
        public string RECIPIENTID { get; set; }
        public string PONUMBER { get; set; }
        public DateTime? SHMENTORDERCREATIONDATE { get; set; }
        public string BILLTOCOMPANY { get; set; }
        public string BILLTOCOMPANYNAME { get; set; }
        public string SOLDBYORG { get; set; }
        public string RMQQUOTENUMBER { get; set; }
        public string RMQPONUMBER { get; set; }
        public string PODOCTYPE { get; set; }
        public string POCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATE { get; set; }
        public string SALESORDERREFERENCEID { get; set; }
        public string PROCESSINGTYPECODE { get; set; }
        public string SODELCOMPL { get; set; }
        public string SHIPPINGNOTE { get; set; }
        public string SHIPPMETHOD { get; set; }
        public string SOFRTCARRIER { get; set; }
        public string SPLPROCIND { get; set; }
        public string SHIPTOID { get; set; }
        public string SALESORDERNUMBER { get; set; }
        public string HEADERSCHEDULINGSTATUS { get; set; }
        public string SALESPERSON { get; set; }
        public DateTime? SODATE { get; set; }
        public string SHIPTOADDRESS { get; set; }
        public string SHIPTOCOUNTRYCODE { get; set; }
        public string SHIPTOREGIONCODE { get; set; }
        public string SHIPTOSTREETPOSTALCODE { get; set; }
        public string SHIPTOCITYNAME { get; set; }
        public string SHIPTOSTREETNAME { get; set; }
        public string SHIPTOHOUSEID { get; set; }
        public string SHIPTOCONTACTPHONE { get; set; }
        public string SHIPTODEVIATINGFULLNAME { get; set; }
        public string SHIPTOEMAILURI { get; set; }
        public string SHIPTOURIDEFAULTINDICATOR { get; set; }
        public string SHIPTOFAXL { get; set; }
        public string CUSTOMERPONUMBER { get; set; }
        public string FREIGHTTERM { get; set; }
        public string INCO1 { get; set; }
        public string INCO2 { get; set; }
        public string SOLDTOID { get; set; }
        public string SOLDTOADDRESS { get; set; }
        public string SOLDTOCOUNTRYCODE { get; set; }
        public string SOLDTOREGIONCODE { get; set; }
        public string SOLDTOSTREETPOSTALCODE { get; set; }
        public string SOLDTOCITYNAME { get; set; }
        public string SOLDTOSTREETNAME { get; set; }
    }
}