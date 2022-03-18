using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.DCN
{
    public class BroadcomPackingList
    {
        public string SALES_ORDER_NUMBER { get; set; }
        public string SHIP_DATE { get; set; }
        public string CUSTOMER_PO_NUMBER { get; set; }
        public string DN_NO { get; set; }
        public string SKUNO { get; set; }
        public string BILL_TO_COMPANY_NAME { get; set; }
        public string BILL_TO_ADDRESS1 { get; set; }
        public string BILL_TO_ADDRESS2 { get; set; }
        public string BILL_TO_ADDRESS3 { get; set; }
        public string BILL_TO_ADDRESS4 { get; set; }
        public string BILL_TO_CITY { get; set; }
        public string BILL_TO_STATE { get; set; }
        public string BILL_TO_POSTAL_CODE { get; set; }
        public string BILL_TO_COUNTRY { get; set; }
        public string SHIPPING_NOTE { get; set; }
        public string SHIP_TO_COMPANY_NAME { get; set; }
        public string SHIP_TO_ADDRESS1 { get; set; }
        public string SHIP_TO_ADDRESS2 { get; set; }
        public string SHIP_TO_ADDRESS3 { get; set; }
        public string SHIP_TO_ADDRESS4 { get; set; }
        public string SHIP_TO_CITY { get; set; }
        public string SHIP_TO_STATE { get; set; }
        public string SHIP_TO_POSTAL_CODE { get; set; }
        public string SHIP_TO_COUNTRY { get; set; }
        public string SHIPPING_METHOD { get; set; }
        public string INCO_TERM { get; set; }
        public string FobCode { get; set; }
        public string SPECIAL_INSTRUCTION { get; set; }
        public string Solineno { get; set; }
        public string Total_Volumetric_Weight { get; set; }
        public string Total_Volumetric_Weight_Sum { get; set; }
        public string Country_of_Origin { get; set; }     
        public string Itemdesc { get; set; }
        public string CUSTOMER_ITEM { get; set; }
        public string PCS_NT { get; set; }
        public string PAGE { get; set; }
        public string ALLPAGE { get; set; }
        public List<PalletObj> PalletList { get; set; }
        public string TotalPackages { get; set; }
        public string TotalQuantityShipped { get; set; }
        public string UOM { get; set; }
        public string TotalGrossWeight { get; set; }
        public string TotalNetWeight { get; set; }
        
    }

    public class PalletObj
    {
        public string Line { get; set; }
        public string ItemNumber { get; set; }
        public string BoxLPNNumber { get; set; }
        public string Packages { get; set; }
        public string QuantityShipped { get; set; }
        public string UOM { get; set; }
        public string BoxWeight { get; set; }
        public string NetWeightUnit { get; set; }
        public string VolumetricWeight { get; set; }
        public string CountryOfOrigin { get; set; }
        public List<string> SNList { get; set; }
    }
}
