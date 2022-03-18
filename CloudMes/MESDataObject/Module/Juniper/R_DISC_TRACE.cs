using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module.Juniper
{
    public class T_R_DISC_TRACE : DataObjectTable
    {
        public T_R_DISC_TRACE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DISC_TRACE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DISC_TRACE);
            TableName = "R_DISC_TRACE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_DISC_TRACE : DataObjectBase
    {
        public Row_R_DISC_TRACE(DataObjectInfo info) : base(info)
        {

        }
        public R_DISC_TRACE GetDataObject()
        {
            R_DISC_TRACE DataObject = new R_DISC_TRACE();
            DataObject.ID = this.ID;
            DataObject.SUPPLIER = this.SUPPLIER;
            DataObject.SUPPLIER_SITE = this.SUPPLIER_SITE;
            DataObject.PART_NUMBER = this.PART_NUMBER;
            DataObject.CM_ODM_PARTNUMBER = this.CM_ODM_PARTNUMBER;
            DataObject.SERIAL_NUMBER = this.SERIAL_NUMBER;
            DataObject.PART_NUMBER_REVISION = this.PART_NUMBER_REVISION;
            DataObject.SHOP_FLOOR_ORDER_NUMBER = this.SHOP_FLOOR_ORDER_NUMBER;
            DataObject.COMPONENT_PART_NUMBER = this.COMPONENT_PART_NUMBER;
            DataObject.CM_COMPONENT_PART_NUMBER = this.CM_COMPONENT_PART_NUMBER;
            DataObject.ASSEMBLY_STATION = this.ASSEMBLY_STATION;
            DataObject.ASSEMBLY_STATION_DESCRIPTION = this.ASSEMBLY_STATION_DESCRIPTION;
            DataObject.LOCATION = this.LOCATION;
            DataObject.VENDOR = this.VENDOR;
            DataObject.MPN = this.MPN;
            DataObject.DATE_CODE = this.DATE_CODE;
            DataObject.LOT_CODE = this.LOT_CODE;
            DataObject.SERIAL_NUMBER_CHILD = this.SERIAL_NUMBER_CHILD;
            DataObject.ECID = this.ECID;
            DataObject.QTY = this.QTY;
            DataObject.LOAD_DATE = this.LOAD_DATE;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.SENT_FLAG = this.SENT_FLAG;
            DataObject.SENT_TIME = this.SENT_TIME;
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
        public string SUPPLIER_SITE
        {
            get
            {
                return (string)this["SUPPLIER_SITE"];
            }
            set
            {
                this["SUPPLIER_SITE"] = value;
            }
        }
        public string PART_NUMBER
        {
            get
            {
                return (string)this["PART_NUMBER"];
            }
            set
            {
                this["PART_NUMBER"] = value;
            }
        }
        public string CM_ODM_PARTNUMBER
        {
            get
            {
                return (string)this["CM_ODM_PARTNUMBER"];
            }
            set
            {
                this["CM_ODM_PARTNUMBER"] = value;
            }
        }
        public string SERIAL_NUMBER
        {
            get
            {
                return (string)this["SERIAL_NUMBER"];
            }
            set
            {
                this["SERIAL_NUMBER"] = value;
            }
        }
        public string PART_NUMBER_REVISION
        {
            get
            {
                return (string)this["PART_NUMBER_REVISION"];
            }
            set
            {
                this["PART_NUMBER_REVISION"] = value;
            }
        }
        public string SHOP_FLOOR_ORDER_NUMBER
        {
            get
            {
                return (string)this["SHOP_FLOOR_ORDER_NUMBER"];
            }
            set
            {
                this["SHOP_FLOOR_ORDER_NUMBER"] = value;
            }
        }
        public string COMPONENT_PART_NUMBER
        {
            get
            {
                return (string)this["COMPONENT_PART_NUMBER"];
            }
            set
            {
                this["COMPONENT_PART_NUMBER"] = value;
            }
        }
        public string CM_COMPONENT_PART_NUMBER
        {
            get
            {
                return (string)this["CM_COMPONENT_PART_NUMBER"];
            }
            set
            {
                this["CM_COMPONENT_PART_NUMBER"] = value;
            }
        }
        public string ASSEMBLY_STATION
        {
            get
            {
                return (string)this["ASSEMBLY_STATION"];
            }
            set
            {
                this["ASSEMBLY_STATION"] = value;
            }
        }
        public string ASSEMBLY_STATION_DESCRIPTION
        {
            get
            {
                return (string)this["ASSEMBLY_STATION_DESCRIPTION"];
            }
            set
            {
                this["ASSEMBLY_STATION_DESCRIPTION"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
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
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
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
        public string LOT_CODE
        {
            get
            {
                return (string)this["LOT_CODE"];
            }
            set
            {
                this["LOT_CODE"] = value;
            }
        }
        public string SERIAL_NUMBER_CHILD
        {
            get
            {
                return (string)this["SERIAL_NUMBER_CHILD"];
            }
            set
            {
                this["SERIAL_NUMBER_CHILD"] = value;
            }
        }
        public string ECID
        {
            get
            {
                return (string)this["ECID"];
            }
            set
            {
                this["ECID"] = value;
            }
        }
        public string QTY
        {
            get
            {
                return (string)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public DateTime? LOAD_DATE
        {
            get
            {
                return (DateTime?)this["LOAD_DATE"];
            }
            set
            {
                this["LOAD_DATE"] = value;
            }
        }
        public string FILE_NAME
        {
            get
            {
                return (string)this["FILE_NAME"];
            }
            set
            {
                this["FILE_NAME"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public double? SENT_FLAG
        {
            get
            {
                return (double?)this["SENT_FLAG"];
            }
            set
            {
                this["SENT_FLAG"] = value;
            }
        }
        public DateTime? SENT_TIME
        {
            get
            {
                return (DateTime?)this["SENT_TIME"];
            }
            set
            {
                this["SENT_TIME"] = value;
            }
        }
    }
    public class R_DISC_TRACE
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SUPPLIER { get; set; }
        public string SUPPLIER_SITE { get; set; }
        public string PART_NUMBER { get; set; }
        public string CM_ODM_PARTNUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string PART_NUMBER_REVISION { get; set; }
        public string SHOP_FLOOR_ORDER_NUMBER { get; set; }
        public string COMPONENT_PART_NUMBER { get; set; }
        public string CM_COMPONENT_PART_NUMBER { get; set; }
        public string ASSEMBLY_STATION { get; set; }
        public string ASSEMBLY_STATION_DESCRIPTION { get; set; }
        public string LOCATION { get; set; }
        public string VENDOR { get; set; }
        public string MPN { get; set; }
        public string DATE_CODE { get; set; }
        public string LOT_CODE { get; set; }
        public string SERIAL_NUMBER_CHILD { get; set; }
        public string ECID { get; set; }
        public string QTY { get; set; }
        public DateTime? LOAD_DATE { get; set; }
        public string FILE_NAME { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public double? SENT_FLAG { get; set; }
        public DateTime? SENT_TIME { get; set; }
        public string HEAD_ID { get; set; }
        public double? VALID_FLAG { get; set; }

        public R_DISC_TRACE Clone()
        {
            return (R_DISC_TRACE)this.MemberwiseClone();
        }
    }
}