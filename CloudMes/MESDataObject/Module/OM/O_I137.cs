using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.OM
{
    public class O_I137
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

        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
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
