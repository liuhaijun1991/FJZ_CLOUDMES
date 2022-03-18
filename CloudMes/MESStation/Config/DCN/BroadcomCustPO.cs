using MESDataObject.Module.DCN;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.DCN
{
    public class BroadcomCustPO
    {

    }
    public class BroadcomCustPOLine
    {
        public static Dictionary<string, string> CsvMapping = new Dictionary<string, string>() {
            {"V1",  "ORGANIZATION"},
            {"V2",  "CM_CODE"},
            {"V3",  "DEPARTMENT"},
            {"V4",  "SALES_CHANNEL"},
            {"V5",  "GOVERNMENT_USER"},
            {"V6",  "UNIT_PRICE"},
            {"V7",  "ORDER_LINE_TYPE"},
            {"V8",  "UOM"},
            {"V9",  "REGION"},
            {"V10", "SALES_ORDER_NUMBER"},
            {"V11", "SALES_ORDER_LINE"},
            {"V12", "VERSION"},
            {"V13", "PART_NUMBER"},
            {"V14", "ITEM_DESCRIPTION"},
            {"V15", "SHIPPABLE_FLAG"},
            {"V16", "BUNDLE_PART"},
            {"V17", "FULFILLMENT_SET"},
            {"V18", "CUSTOMER_ITEM"},
            {"V19", "CUSTOMER_NAME"},
            {"V20", "CUSTOMER_PO_NUMBER"},
            {"V21", "ORDER_STATUS"},
            {"V22", "HOLD_NAME"},
            {"V23", "BOOKED_DATE"},
            {"V24", "CUSTOMER_REQUESTED_DATE"},
            {"V25", "TRANSIT_TIME"},
            {"V26", "PROMISE_DATE"},
            {"V27", "SUGGESTED_SHIP_DATE"},
            {"V28", "SCHEDULE_SHIP_DATE"},
            {"V29", "AUTO_MANUAL_ACK"},
            {"V30", "CM_COMMIT_DATE"},
            {"V31", "COMMENTS"},
            {"V32", "ORDERED_QUANTITY"},
            {"V33", "SPECIAL_INSTRUCTION"},
            {"V34", "SHIP_TO_ID"},
            {"V35", "SHIPPING_METHOD"},
            {"V36", "INCO_TERM"},
            {"V37", "SHIPPING_ZONE"},
            {"V38", "SELLING_ENTITY_COMPANY_NAME"},
            {"V39", "SELLING_ENTITY_ADDRESS1"},
            {"V40", "SELLING_ENTITY_ADDRESS2"},
            {"V41", "SELLING_ENTITY_ADDRESS3"},
            {"V42", "SELLING_ENTITY_ADDRESS4"},
            {"V43", "SELLING_ENTITY_CITY"},
            {"V44", "SELLING_ENTITY_STATE"},
            {"V45", "SELLING_ENTITY_POSTAL_CODE"},
            {"V46", "SELLING_ENTITY_COUNTRY"},
            {"V47", "SHIP_FROM_COMPANY_NAME"},
            {"V48", "SHIP_FROM_ADDRESS1"},
            {"V49", "SHIP_FROM_ADDRESS2"},
            {"V50", "SHIP_FROM_ADDRESS3"},
            {"V51", "SHIP_FROM_ADDRESS4"},
            {"V52", "SHIP_FROM_CITY"},
            {"V53", "SHIP_FROM_STATE"},
            {"V54", "SHIP_FROM_POSTAL_CODE"},
            {"V55", "SHIP_FROM_COUNTRY"},
            {"V56", "SHIP_TO_COMPANY_NAME"},
            {"V57", "SHIP_TO_ADDRESS1"},
            {"V58", "SHIP_TO_ADDRESS2"},
            {"V59", "SHIP_TO_ADDRESS3"},
            {"V60", "SHIP_TO_ADDRESS4"},
            {"V61", "SHIP_TO_CITY"},
            {"V62", "SHIP_TO_STATE"},
            {"V63", "SHIP_TO_POSTAL_CODE"},
            {"V64", "SHIP_TO_COUNTRY"},
            {"V65", "BILL_TO_COMPANY_NAME"},
            {"V66", "BILL_TO_ADDRESS1"},
            {"V67", "BILL_TO_ADDRESS2"},
            {"V68", "BILL_TO_ADDRESS3"},
            {"V69", "BILL_TO_ADDRESS4"},
            {"V70", "BILL_TO_CITY"},
            {"V71", "BILL_TO_STATE"},
            {"V72", "BILL_TO_POSTAL_CODE"},
            {"V73", "BILL_TO_COUNTRY"},
            {"V74", "SHIP_TO_HTS"},
            {"V75", "US_ECCN"},
            {"V76", "US_LICENSE"},
            {"V77", "ECCN"},
            {"V78", "LICENSE"},
            {"V79", "SHIPPING_NOTE"},
            {"V80", "PLANNER_CODE"},
            {"V81", "PLANNER_EMAIL_ADDRESS"},
            {"V82", "LINE_BOOK_DATE"},
            {"V83", "LINE_LAST_UPDATE_DATE"}
         };
        public string ORGANIZATION { get; set; }
        public string CM_CODE { get; set; }
        public string DEPARTMENT { get; set; }
        public string SALES_CHANNEL { get; set; }
        public string GOVERNMENT_USER { get; set; }
        public string UNIT_PRICE { get; set; }
        public string ORDER_LINE_TYPE { get; set; }
        public string UOM { get; set; }
        public string REGION { get; set; }
        public string SALES_ORDER_NUMBER { get; set; }
        public string SALES_ORDER_LINE { get; set; }
        public string VERSION { get; set; }
        public string PART_NUMBER { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string SHIPPABLE_FLAG { get; set; }
        public string BUNDLE_PART { get; set; }
        public string FULFILLMENT_SET { get; set; }
        public string CUSTOMER_ITEM { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_PO_NUMBER { get; set; }
        public string ORDER_STATUS { get; set; }
        public string HOLD_NAME { get; set; }
        public string BOOKED_DATE { get; set; }
        public string CUSTOMER_REQUESTED_DATE { get; set; }
        public string TRANSIT_TIME { get; set; }
        public string PROMISE_DATE { get; set; }
        public string SUGGESTED_SHIP_DATE { get; set; }
        public string SCHEDULE_SHIP_DATE { get; set; }
        public string AUTO_MANUAL_ACK { get; set; }
        public string CM_COMMIT_DATE { get; set; }
        public string COMMENTS { get; set; }
        public string ORDERED_QUANTITY { get; set; }
        public string SPECIAL_INSTRUCTION { get; set; }
        public string SHIP_TO_ID { get; set; }
        public string SHIPPING_METHOD { get; set; }
        public string INCO_TERM { get; set; }
        public string SHIPPING_ZONE { get; set; }
        public string SELLING_ENTITY_COMPANY_NAME { get; set; }
        public string SELLING_ENTITY_ADDRESS1 { get; set; }
        public string SELLING_ENTITY_ADDRESS2 { get; set; }
        public string SELLING_ENTITY_ADDRESS3 { get; set; }
        public string SELLING_ENTITY_ADDRESS4 { get; set; }
        public string SELLING_ENTITY_CITY { get; set; }
        public string SELLING_ENTITY_STATE { get; set; }
        public string SELLING_ENTITY_POSTAL_CODE { get; set; }
        public string SELLING_ENTITY_COUNTRY { get; set; }
        public string SHIP_FROM_COMPANY_NAME { get; set; }
        public string SHIP_FROM_ADDRESS1 { get; set; }
        public string SHIP_FROM_ADDRESS2 { get; set; }
        public string SHIP_FROM_ADDRESS3 { get; set; }
        public string SHIP_FROM_ADDRESS4 { get; set; }
        public string SHIP_FROM_CITY { get; set; }
        public string SHIP_FROM_STATE { get; set; }
        public string SHIP_FROM_POSTAL_CODE { get; set; }
        public string SHIP_FROM_COUNTRY { get; set; }
        public string SHIP_TO_COMPANY_NAME { get; set; }
        public string SHIP_TO_ADDRESS1 { get; set; }
        public string SHIP_TO_ADDRESS2 { get; set; }
        public string SHIP_TO_ADDRESS3 { get; set; }
        public string SHIP_TO_ADDRESS4 { get; set; }
        public string SHIP_TO_CITY { get; set; }
        public string SHIP_TO_STATE { get; set; }
        public string SHIP_TO_POSTAL_CODE { get; set; }
        public string SHIP_TO_COUNTRY { get; set; }
        public string BILL_TO_COMPANY_NAME { get; set; }
        public string BILL_TO_ADDRESS1 { get; set; }
        public string BILL_TO_ADDRESS2 { get; set; }
        public string BILL_TO_ADDRESS3 { get; set; }
        public string BILL_TO_ADDRESS4 { get; set; }
        public string BILL_TO_CITY { get; set; }
        public string BILL_TO_STATE { get; set; }
        public string BILL_TO_POSTAL_CODE { get; set; }
        public string BILL_TO_COUNTRY { get; set; }
        public string SHIP_TO_HTS { get; set; }
        public string US_ECCN { get; set; }
        public string US_LICENSE { get; set; }
        public string ECCN { get; set; }
        public string LICENSE { get; set; }
        public string SHIPPING_NOTE { get; set; }
        public string PLANNER_CODE { get; set; }
        public string PLANNER_EMAIL_ADDRESS { get; set; }
        public string LINE_BOOK_DATE { get; set; }
        public string LINE_LAST_UPDATE_DATE { get; set; }

        public BroadcomCustPOLine() { }
        public BroadcomCustPOLine(DataRow dr )
        {
            Type t = typeof(BroadcomCustPOLine);
            var keys = CsvMapping.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                var csvcol = CsvMapping[keys[i]];
                t.GetProperty( csvcol ).SetValue(this, dr[csvcol].ToString());
            }
        }
        public BroadcomCustPOLine(BROADCOM_CSV_DETAIL dr)
        {
            Type t = typeof(BroadcomCustPOLine);
            Type t2 = dr.GetType();
            var keys = CsvMapping.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                try
                {
                    var csvcol = CsvMapping[keys[i]];
                    var value = t2.GetProperty(keys[i]).GetValue(dr)?.ToString();
                    t.GetProperty(csvcol).SetValue(this, value);
                }
                catch(Exception ee)
                {
                    throw ee;
                }
            }
        }

    }
}
