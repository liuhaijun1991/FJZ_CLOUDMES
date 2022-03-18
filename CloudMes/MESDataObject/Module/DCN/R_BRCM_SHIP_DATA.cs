using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BRCM_SHIP_DATA : DataObjectTable
    {
        public T_R_BRCM_SHIP_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BRCM_SHIP_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BRCM_SHIP_DATA);
            TableName = "R_BRCM_SHIP_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_BRCM_SHIP_DATA : DataObjectBase
    {
        public Row_R_BRCM_SHIP_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_BRCM_SHIP_DATA GetDataObject()
        {
            R_BRCM_SHIP_DATA DataObject = new R_BRCM_SHIP_DATA();
            DataObject.ID = this.ID;
            DataObject.RECORD_TYPE = this.RECORD_TYPE;
            DataObject.CM_CODE = this.CM_CODE;
            DataObject.RECORD_CREATION_DATE = this.RECORD_CREATION_DATE;
            DataObject.TRANSACTION_TYPE = this.TRANSACTION_TYPE;
            DataObject.SHIPMENT_ID = this.SHIPMENT_ID;
            DataObject.ITEM = this.ITEM;
            DataObject.UOM = this.UOM;
            DataObject.DEPARTMENT_CODE = this.DEPARTMENT_CODE;
            DataObject.COMPLETION_DATE = this.COMPLETION_DATE;
            DataObject.SHIPMENT_DATE = this.SHIPMENT_DATE;
            DataObject.SHIPMENT_NUMBER = this.SHIPMENT_NUMBER;
            DataObject.DELIVERY_DATE = this.DELIVERY_DATE;
            DataObject.DELIVERY_NOTE_NUMBER = this.DELIVERY_NOTE_NUMBER;
            DataObject.SHIP_TO_ADDRESS_CODE = this.SHIP_TO_ADDRESS_CODE;
            DataObject.SHIP_METHOD = this.SHIP_METHOD;
            DataObject.ORDER_NUMBER = this.ORDER_NUMBER;
            DataObject.LINE_NUMBER = this.LINE_NUMBER;
            DataObject.PO_NUMBER = this.PO_NUMBER;
            DataObject.PO_LINE_NUMBER = this.PO_LINE_NUMBER;
            DataObject.PO_SHIPMENT_NUMBER = this.PO_SHIPMENT_NUMBER;
            DataObject.PO_RELEASE_NUMBER = this.PO_RELEASE_NUMBER;
            DataObject.QUANTITY_COMPLETED = this.QUANTITY_COMPLETED;
            DataObject.QUANTITY_SHIPPED = this.QUANTITY_SHIPPED;
            DataObject.WAYBILL_NUMBER = this.WAYBILL_NUMBER;
            DataObject.PACKING_SLIP_NUMBER = this.PACKING_SLIP_NUMBER;
            DataObject.BILL_OF_LADING = this.BILL_OF_LADING;
            DataObject.LOT_NUMBER = this.LOT_NUMBER;
            DataObject.LOT_QUANTITY = this.LOT_QUANTITY;
            DataObject.COO = this.COO;
            DataObject.LPN = this.LPN;
            DataObject.MANUFACTURE_DATE = this.MANUFACTURE_DATE;
            DataObject.CAT = this.CAT;
            DataObject.BIN = this.BIN;
            DataObject.CUSTOM_PN = this.CUSTOM_PN;
            DataObject.REV = this.REV;
            DataObject.VENDOR = this.VENDOR;
            DataObject.COMMENT1 = this.COMMENT1;
            DataObject.TEST_PROGRAM_REVISION = this.TEST_PROGRAM_REVISION;
            DataObject.NUMBER_OF_WIPC_RECORDS = this.NUMBER_OF_WIPC_RECORDS;
            DataObject.RESERVED_COLUMNS = this.RESERVED_COLUMNS;
            DataObject.SHIPTOSITE = this.SHIPTOSITE;
            DataObject.SHIPTODEPT = this.SHIPTODEPT;
            DataObject.VENDOR_PART_NUMBER = this.VENDOR_PART_NUMBER;
            DataObject.VENDOR_LOT_NUMBER = this.VENDOR_LOT_NUMBER;
            DataObject.NC_REASON_CODE = this.NC_REASON_CODE;
            DataObject.BATCH_NOTE = this.BATCH_NOTE;
            DataObject.SUBSTRATE_ID = this.SUBSTRATE_ID;
            DataObject.SEAL_DATE = this.SEAL_DATE;
            DataObject.GOOD_DIE_QTY = this.GOOD_DIE_QTY;
            DataObject.DIE_PART_NUMBER = this.DIE_PART_NUMBER;
            DataObject.DATE_CODE = this.DATE_CODE;
            DataObject.LPN_LOT_ATTRIBUTES = this.LPN_LOT_ATTRIBUTES;
            DataObject.OUTER_LPN_NUMBER = this.OUTER_LPN_NUMBER;
            DataObject.OUTER_LPN_FLAG = this.OUTER_LPN_FLAG;
            DataObject.WAFER_BATCH_NUMBER = this.WAFER_BATCH_NUMBER;
            DataObject.COUNTRY_OF_DIFFUSION = this.COUNTRY_OF_DIFFUSION;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.FILENAME = this.FILENAME;
            return DataObject;
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
        public string RECORD_TYPE
        {
            get
            {
                return (string)this["RECORD_TYPE"];
            }
            set
            {
                this["RECORD_TYPE"] = value;
            }
        }
        public string CM_CODE
        {
            get
            {
                return (string)this["CM_CODE"];
            }
            set
            {
                this["CM_CODE"] = value;
            }
        }
        public string RECORD_CREATION_DATE
        {
            get
            {
                return (string)this["RECORD_CREATION_DATE"];
            }
            set
            {
                this["RECORD_CREATION_DATE"] = value;
            }
        }
        public string TRANSACTION_TYPE
        {
            get
            {
                return (string)this["TRANSACTION_TYPE"];
            }
            set
            {
                this["TRANSACTION_TYPE"] = value;
            }
        }
        public string SHIPMENT_ID
        {
            get
            {
                return (string)this["SHIPMENT_ID"];
            }
            set
            {
                this["SHIPMENT_ID"] = value;
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
        public string DEPARTMENT_CODE
        {
            get
            {
                return (string)this["DEPARTMENT_CODE"];
            }
            set
            {
                this["DEPARTMENT_CODE"] = value;
            }
        }
        public string COMPLETION_DATE
        {
            get
            {
                return (string)this["COMPLETION_DATE"];
            }
            set
            {
                this["COMPLETION_DATE"] = value;
            }
        }
        public string SHIPMENT_DATE
        {
            get
            {
                return (string)this["SHIPMENT_DATE"];
            }
            set
            {
                this["SHIPMENT_DATE"] = value;
            }
        }
        public string SHIPMENT_NUMBER
        {
            get
            {
                return (string)this["SHIPMENT_NUMBER"];
            }
            set
            {
                this["SHIPMENT_NUMBER"] = value;
            }
        }
        public string DELIVERY_DATE
        {
            get
            {
                return (string)this["DELIVERY_DATE"];
            }
            set
            {
                this["DELIVERY_DATE"] = value;
            }
        }
        public string DELIVERY_NOTE_NUMBER
        {
            get
            {
                return (string)this["DELIVERY_NOTE_NUMBER"];
            }
            set
            {
                this["DELIVERY_NOTE_NUMBER"] = value;
            }
        }
        public string SHIP_TO_ADDRESS_CODE
        {
            get
            {
                return (string)this["SHIP_TO_ADDRESS_CODE"];
            }
            set
            {
                this["SHIP_TO_ADDRESS_CODE"] = value;
            }
        }
        public string SHIP_METHOD
        {
            get
            {
                return (string)this["SHIP_METHOD"];
            }
            set
            {
                this["SHIP_METHOD"] = value;
            }
        }
        public string ORDER_NUMBER
        {
            get
            {
                return (string)this["ORDER_NUMBER"];
            }
            set
            {
                this["ORDER_NUMBER"] = value;
            }
        }
        public string LINE_NUMBER
        {
            get
            {
                return (string)this["LINE_NUMBER"];
            }
            set
            {
                this["LINE_NUMBER"] = value;
            }
        }
        public string PO_NUMBER
        {
            get
            {
                return (string)this["PO_NUMBER"];
            }
            set
            {
                this["PO_NUMBER"] = value;
            }
        }
        public string PO_LINE_NUMBER
        {
            get
            {
                return (string)this["PO_LINE_NUMBER"];
            }
            set
            {
                this["PO_LINE_NUMBER"] = value;
            }
        }
        public string PO_SHIPMENT_NUMBER
        {
            get
            {
                return (string)this["PO_SHIPMENT_NUMBER"];
            }
            set
            {
                this["PO_SHIPMENT_NUMBER"] = value;
            }
        }
        public string PO_RELEASE_NUMBER
        {
            get
            {
                return (string)this["PO_RELEASE_NUMBER"];
            }
            set
            {
                this["PO_RELEASE_NUMBER"] = value;
            }
        }
        public double? QUANTITY_COMPLETED
        {
            get
            {
                return (double?)this["QUANTITY_COMPLETED"];
            }
            set
            {
                this["QUANTITY_COMPLETED"] = value;
            }
        }
        public double? QUANTITY_SHIPPED
        {
            get
            {
                return (double?)this["QUANTITY_SHIPPED"];
            }
            set
            {
                this["QUANTITY_SHIPPED"] = value;
            }
        }
        public string WAYBILL_NUMBER
        {
            get
            {
                return (string)this["WAYBILL_NUMBER"];
            }
            set
            {
                this["WAYBILL_NUMBER"] = value;
            }
        }
        public string PACKING_SLIP_NUMBER
        {
            get
            {
                return (string)this["PACKING_SLIP_NUMBER"];
            }
            set
            {
                this["PACKING_SLIP_NUMBER"] = value;
            }
        }
        public string BILL_OF_LADING
        {
            get
            {
                return (string)this["BILL_OF_LADING"];
            }
            set
            {
                this["BILL_OF_LADING"] = value;
            }
        }
        public string LOT_NUMBER
        {
            get
            {
                return (string)this["LOT_NUMBER"];
            }
            set
            {
                this["LOT_NUMBER"] = value;
            }
        }
        public double? LOT_QUANTITY
        {
            get
            {
                return (double?)this["LOT_QUANTITY"];
            }
            set
            {
                this["LOT_QUANTITY"] = value;
            }
        }
        public string COO
        {
            get
            {
                return (string)this["COO"];
            }
            set
            {
                this["COO"] = value;
            }
        }
        public string LPN
        {
            get
            {
                return (string)this["LPN"];
            }
            set
            {
                this["LPN"] = value;
            }
        }
        public string MANUFACTURE_DATE
        {
            get
            {
                return (string)this["MANUFACTURE_DATE"];
            }
            set
            {
                this["MANUFACTURE_DATE"] = value;
            }
        }
        public string CAT
        {
            get
            {
                return (string)this["CAT"];
            }
            set
            {
                this["CAT"] = value;
            }
        }
        public string BIN
        {
            get
            {
                return (string)this["BIN"];
            }
            set
            {
                this["BIN"] = value;
            }
        }
        public string CUSTOM_PN
        {
            get
            {
                return (string)this["CUSTOM_PN"];
            }
            set
            {
                this["CUSTOM_PN"] = value;
            }
        }
        public string REV
        {
            get
            {
                return (string)this["REV"];
            }
            set
            {
                this["REV"] = value;
            }
        }
        public string VENDOR
        {
            get
            {
                return (string)this["VENDOR"];
            }
            set
            {
                this["VENDOR"] = value;
            }
        }
        public string COMMENT1
        {
            get
            {
                return (string)this["COMMENT1"];
            }
            set
            {
                this["COMMENT1"] = value;
            }
        }
        public string TEST_PROGRAM_REVISION
        {
            get
            {
                return (string)this["TEST_PROGRAM_REVISION"];
            }
            set
            {
                this["TEST_PROGRAM_REVISION"] = value;
            }
        }
        public double? NUMBER_OF_WIPC_RECORDS
        {
            get
            {
                return (double?)this["NUMBER_OF_WIPC_RECORDS"];
            }
            set
            {
                this["NUMBER_OF_WIPC_RECORDS"] = value;
            }
        }
        public string RESERVED_COLUMNS
        {
            get
            {
                return (string)this["RESERVED_COLUMNS"];
            }
            set
            {
                this["RESERVED_COLUMNS"] = value;
            }
        }
        public string SHIPTOSITE
        {
            get
            {
                return (string)this["SHIPTOSITE"];
            }
            set
            {
                this["SHIPTOSITE"] = value;
            }
        }
        public string SHIPTODEPT
        {
            get
            {
                return (string)this["SHIPTODEPT"];
            }
            set
            {
                this["SHIPTODEPT"] = value;
            }
        }
        public string VENDOR_PART_NUMBER
        {
            get
            {
                return (string)this["VENDOR_PART_NUMBER"];
            }
            set
            {
                this["VENDOR_PART_NUMBER"] = value;
            }
        }
        public string VENDOR_LOT_NUMBER
        {
            get
            {
                return (string)this["VENDOR_LOT_NUMBER"];
            }
            set
            {
                this["VENDOR_LOT_NUMBER"] = value;
            }
        }
        public string NC_REASON_CODE
        {
            get
            {
                return (string)this["NC_REASON_CODE"];
            }
            set
            {
                this["NC_REASON_CODE"] = value;
            }
        }
        public string BATCH_NOTE
        {
            get
            {
                return (string)this["BATCH_NOTE"];
            }
            set
            {
                this["BATCH_NOTE"] = value;
            }
        }
        public string SUBSTRATE_ID
        {
            get
            {
                return (string)this["SUBSTRATE_ID"];
            }
            set
            {
                this["SUBSTRATE_ID"] = value;
            }
        }
        public DateTime? SEAL_DATE
        {
            get
            {
                return (DateTime?)this["SEAL_DATE"];
            }
            set
            {
                this["SEAL_DATE"] = value;
            }
        }
        public double? GOOD_DIE_QTY
        {
            get
            {
                return (double?)this["GOOD_DIE_QTY"];
            }
            set
            {
                this["GOOD_DIE_QTY"] = value;
            }
        }
        public string DIE_PART_NUMBER
        {
            get
            {
                return (string)this["DIE_PART_NUMBER"];
            }
            set
            {
                this["DIE_PART_NUMBER"] = value;
            }
        }
        public string DATE_CODE
        {
            get
            {
                return (string)this["DATE_CODE"];
            }
            set
            {
                this["DATE_CODE"] = value;
            }
        }
        public string LPN_LOT_ATTRIBUTES
        {
            get
            {
                return (string)this["LPN_LOT_ATTRIBUTES"];
            }
            set
            {
                this["LPN_LOT_ATTRIBUTES"] = value;
            }
        }
        public string OUTER_LPN_NUMBER
        {
            get
            {
                return (string)this["OUTER_LPN_NUMBER"];
            }
            set
            {
                this["OUTER_LPN_NUMBER"] = value;
            }
        }
        public string OUTER_LPN_FLAG
        {
            get
            {
                return (string)this["OUTER_LPN_FLAG"];
            }
            set
            {
                this["OUTER_LPN_FLAG"] = value;
            }
        }
        public string WAFER_BATCH_NUMBER
        {
            get
            {
                return (string)this["WAFER_BATCH_NUMBER"];
            }
            set
            {
                this["WAFER_BATCH_NUMBER"] = value;
            }
        }
        public string COUNTRY_OF_DIFFUSION
        {
            get
            {
                return (string)this["COUNTRY_OF_DIFFUSION"];
            }
            set
            {
                this["COUNTRY_OF_DIFFUSION"] = value;
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
    }
    public class R_BRCM_SHIP_DATA
    {
        public string ID { get; set; }
        public string RECORD_TYPE { get; set; }
        public string CM_CODE { get; set; }
        public string RECORD_CREATION_DATE { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string SHIPMENT_ID { get; set; }
        public string ITEM { get; set; }
        public string UOM { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string COMPLETION_DATE { get; set; }
        public string SHIPMENT_DATE { get; set; }
        public string SHIPMENT_NUMBER { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string DELIVERY_NOTE_NUMBER { get; set; }
        public string SHIP_TO_ADDRESS_CODE { get; set; }
        public string SHIP_METHOD { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string LINE_NUMBER { get; set; }
        public string PO_NUMBER { get; set; }
        public string PO_LINE_NUMBER { get; set; }
        public string PO_SHIPMENT_NUMBER { get; set; }
        public string PO_RELEASE_NUMBER { get; set; }
        public double? QUANTITY_COMPLETED { get; set; }
        public double? QUANTITY_SHIPPED { get; set; }
        public string WAYBILL_NUMBER { get; set; }
        public string PACKING_SLIP_NUMBER { get; set; }
        public string BILL_OF_LADING { get; set; }
        public string LOT_NUMBER { get; set; }
        public double? LOT_QUANTITY { get; set; }
        public string COO { get; set; }
        public string LPN { get; set; }
        public string MANUFACTURE_DATE { get; set; }
        public string CAT { get; set; }
        public string BIN { get; set; }
        public string CUSTOM_PN { get; set; }
        public string REV { get; set; }
        public string VENDOR { get; set; }
        public string COMMENT1 { get; set; }
        public string TEST_PROGRAM_REVISION { get; set; }
        public double? NUMBER_OF_WIPC_RECORDS { get; set; }
        public string RESERVED_COLUMNS { get; set; }
        public string SHIPTOSITE { get; set; }
        public string SHIPTODEPT { get; set; }
        public string VENDOR_PART_NUMBER { get; set; }
        public string VENDOR_LOT_NUMBER { get; set; }
        public string NC_REASON_CODE { get; set; }
        public string BATCH_NOTE { get; set; }
        public string SUBSTRATE_ID { get; set; }
        public DateTime? SEAL_DATE { get; set; }
        public double? GOOD_DIE_QTY { get; set; }
        public string DIE_PART_NUMBER { get; set; }
        public string DATE_CODE { get; set; }
        public string LPN_LOT_ATTRIBUTES { get; set; }
        public string OUTER_LPN_NUMBER { get; set; }
        public string OUTER_LPN_FLAG { get; set; }
        public string WAFER_BATCH_NUMBER { get; set; }
        public string COUNTRY_OF_DIFFUSION { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string FILENAME { get; set; }
    }
}
