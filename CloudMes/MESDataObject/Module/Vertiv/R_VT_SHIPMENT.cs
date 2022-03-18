using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_VT_SHIPMENT : DataObjectTable
    {
        public T_R_VT_SHIPMENT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_VT_SHIPMENT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_VT_SHIPMENT);
            TableName = "R_VT_SHIPMENT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_VT_SHIPMENT : DataObjectBase
    {
        public Row_R_VT_SHIPMENT(DataObjectInfo info) : base(info)
        {

        }
        public R_VT_SHIPMENT GetDataObject()
        {
            R_VT_SHIPMENT DataObject = new R_VT_SHIPMENT();
            DataObject.ID = this.ID;
            DataObject.SHIPMENT_ID = this.SHIPMENT_ID;
            DataObject.ORDER_ID = this.ORDER_ID;
            DataObject.DN_NO = this.DN_NO;
            DataObject.DN_LINE = this.DN_LINE;
            DataObject.SHIPMENT_DETAIL = this.SHIPMENT_DETAIL;
            DataObject.ACTION = this.ACTION;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.CREATED_EMP = this.CREATED_EMP;
            DataObject.CREATED_TIME = this.CREATED_TIME;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.SEND_EMP = this.SEND_EMP;
            DataObject.SEND_TIME = this.SEND_TIME;
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
        public string ORDER_ID
        {
            get
            {
                return (string)this["ORDER_ID"];
            }
            set
            {
                this["ORDER_ID"] = value;
            }
        }
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string DN_LINE
        {
            get
            {
                return (string)this["DN_LINE"];
            }
            set
            {
                this["DN_LINE"] = value;
            }
        }
        public byte[] SHIPMENT_DETAIL
        {
            get
            {
                return (byte[])this["SHIPMENT_DETAIL"];
            }
            set
            {
                this["SHIPMENT_DETAIL"] = value;
            }
        }
        public string ACTION
        {
            get
            {
                return (string)this["ACTION"];
            }
            set
            {
                this["ACTION"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
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
        public string CREATED_EMP
        {
            get
            {
                return (string)this["CREATED_EMP"];
            }
            set
            {
                this["CREATED_EMP"] = value;
            }
        }
        public DateTime? CREATED_TIME
        {
            get
            {
                return (DateTime?)this["CREATED_TIME"];
            }
            set
            {
                this["CREATED_TIME"] = value;
            }
        }
        public string SEND_FLAG
        {
            get
            {
                return (string)this["SEND_FLAG"];
            }
            set
            {
                this["SEND_FLAG"] = value;
            }
        }
        public string SEND_EMP
        {
            get
            {
                return (string)this["SEND_EMP"];
            }
            set
            {
                this["SEND_EMP"] = value;
            }
        }
        public DateTime? SEND_TIME
        {
            get
            {
                return (DateTime?)this["SEND_TIME"];
            }
            set
            {
                this["SEND_TIME"] = value;
            }
        }
    }
    public class R_VT_SHIPMENT
    {
        public string ID { get; set; }
        public string SHIPMENT_ID { get; set; }
        public string ORDER_ID { get; set; }
        public string DN_NO { get; set; }
        public string DN_LINE { get; set; }

        public byte[] SHIPMENT_DETAIL { get; set; }
        public string ACTION { get; set; }
        public string VALID_FLAG { get; set; }
        public string FILE_NAME { get; set; }
        public string CREATED_EMP { get; set; }
        public DateTime? CREATED_TIME { get; set; }
        public string SEND_FLAG { get; set; }
        public string SEND_EMP { get; set; }
        public DateTime? SEND_TIME { get; set; }       
    }

    public enum ShipmentAction
    {
        [EnumName("InsertOrUpdate")]
        [EnumValue("0")]
        InsertOrUpdate,

        [EnumName("SuppUpdate")]
        [EnumValue("1")]
        SuppUpdate,

        [EnumName("CarrierUpdate")]
        [EnumValue("2")]
        CarrierUpdate,

        [EnumName("Deliver")]
        [EnumValue("3")]
        Deliver,

        [EnumName("Cancel")]
        [EnumValue("4")]
        Cancel
    }
}