using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.Juniper
{
    public class PackingLabelValue { 
        public string DeliveryNumber { get; set; }
        public string SoldBy { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public string ShippingNotes { get; set; }
        public string ForwardingAgent { get; set; }
        public string ShipVia { get; set; }
        public string IncoTermPlace { get; set; }
        public string LVAS { get; set; }
        public string TotalWeight  { get; set; }
        public string TotalNetWeight { get; set; }
        public string TotalPieces { get; set; }
        public string TotalCartons{ get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string CustomerPO { get; set; }
        public string SalesPerson { get; set; }
        public string ContactPerson { get; set; }

        public List<PackingLabelItem> ItemList { get;set; }
    }
    public class PackingLabelItem
    {
        public string QtLn { get; set; }
        public string Ln { get; set; }
        public string ProductNumber { get; set; }
        public string CustomerPartNumber { get; set; }
        public string ProductDescription { get; set; }
        public bool BNDLParent { get; set; } = false;
        public string UoM { get; set; }
        public string SerialNumber{get;set;}
        public string OrderQty { get; set; }
        public string ShipQty { get; set; }
        public string CLEI { get; set; }
        public string CPR { get; set; }
    }
}
