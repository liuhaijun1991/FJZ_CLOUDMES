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
    public class O_I137_ITEM
    {
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
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string ACTIONCODE { get; set; }
        public string PARENTITEMID { get; set; }
        public string ITEMCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
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
        public string VERSION { get; set; }
        public string UPOID { get; set; }
        public string MFLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
    }

    [SqlSugar.SugarTable("O_I137_ITEM")]
    public class I137_I
    {
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
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string ACTIONCODE { get; set; }
        public string PARENTITEMID { get; set; }
        public string ITEMCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
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
        public string VERSION { get; set; }
        public string UPOID { get; set; }
        public string MFLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
    }

    [SqlSugar.SugarTable("jnp.TB_I137item")]
    public class B2B_I137_I
    {
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
        public DateTime? F_LASTEDITDT { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string ACTIONCODE { get; set; }
        public string PARENTITEMID { get; set; }
        public string ITEMCHANGEINDICATOR { get; set; }
        public DateTime? LASTCHANGEDATETIME { get; set; }
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
    }

    public enum ENUM_I137_H_STATUS
    {
        [EnumValue("0")]
        WAITCHECK,
        [EnumValue("1")]
        CHECK_PASS,
        [EnumValue("2")]
        CHECK_FAIL,
        [EnumValue("3")]
        RELEASE,
        [EnumValue("4")]
        SKIP,
        [EnumValue("5")]
        CHECK_FAIL_CLOSED,
    }

    public enum ENUM_I137_Actioncode_Type
    {
        [EnumValue("01")]
        Change,
        [EnumValue("02")]
        Cancel
    }
}