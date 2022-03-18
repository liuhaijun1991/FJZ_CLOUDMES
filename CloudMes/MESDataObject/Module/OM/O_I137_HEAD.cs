using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.OM
{
    public class O_I137_HEAD
    {
        public string MFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }

        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? HEADERCREATIONDATETIME { get; set; }
        public string SENDERID { get; set; }
        public string RECIPIENTID { get; set; }
        public string PONUMBER { get; set; }
        public DateTime? SHMENTORDERCREATIONDATE { get; set; }
        public string POBILLTOCOMPANY { get; set; }
        public string BILLTOCOMPANYNAME { get; set; }
        public string SOLDBYORG { get; set; }
        public string RMQQUOTENUMBER { get; set; }
        public string RMQPONUMBER { get; set; }
        public string PODOCTYPE { get; set; }
        public string POCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
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
        public string VERSION { get; set; }
        public string VENDORID { get; set; }
        public string ECO_FCO { get; set; }
        public string PAYMENTTERM { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
    }


    [SqlSugar.SugarTable("O_I137_HEAD")]
    public class I137_H
    {
        public string MFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }

        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? HEADERCREATIONDATETIME { get; set; }
        public string SENDERID { get; set; }
        public string RECIPIENTID { get; set; }
        public string PONUMBER { get; set; }
        public DateTime? SHMENTORDERCREATIONDATE { get; set; }
        public string POBILLTOCOMPANY { get; set; }
        public string BILLTOCOMPANYNAME { get; set; }
        public string SOLDBYORG { get; set; }
        public string RMQQUOTENUMBER { get; set; }
        public string RMQPONUMBER { get; set; }
        public string PODOCTYPE { get; set; }
        public string POCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
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
        public string VERSION { get; set; }
        public string VENDORID { get; set; }        
        public string ECO_FCO { get; set; }
        public string PAYMENTTERM { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
    }


    [SqlSugar.SugarTable("jnp.TB_I137head")]
    public class B2B_I137_H
    {
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? HEADERCREATIONDATETIME { get; set; }
        public string SENDERID { get; set; }
        public string RECIPIENTID { get; set; }
        public string PONUMBER { get; set; }
        public DateTime? SHMENTORDERCREATIONDATE { get; set; }
        public string POBILLTOCOMPANY { get; set; }
        public string BILLTOCOMPANYNAME { get; set; }
        public string SOLDBYORG { get; set; }
        public string RMQQUOTENUMBER { get; set; }
        public string RMQPONUMBER { get; set; }
        public string PODOCTYPE { get; set; }
        public string POCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
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
        public string VENDORID { get; set; }
        public string PAYMENTTERM { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
    }
    
    public enum ENUM_I137_PoDocType
    {
        [EnumValue("ZDOA")]
        ZDOA,
        [EnumValue("ZIO")]
        ZIO,
        [EnumValue("ZSOD")]
        ZSOD,
        [EnumValue("ZRMQ")]
        ZRMQ,
        [EnumValue("IDOA")]
        IDOA
    }
}