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
    public class T_R_VT_ORDER : DataObjectTable
    {
        public T_R_VT_ORDER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_VT_ORDER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_VT_ORDER);
            TableName = "R_VT_ORDER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_VT_ORDER : DataObjectBase
    {
        public Row_R_VT_ORDER(DataObjectInfo info) : base(info)
        {

        }
        public R_VT_ORDER GetDataObject()
        {
            R_VT_ORDER DataObject = new R_VT_ORDER();
            DataObject.ID = this.ID;
            DataObject.ORDER_NUMBER = this.ORDER_NUMBER;
            DataObject.ORDER_LINE_ID = this.ORDER_LINE_ID;
            DataObject.PROMISE_ID = this.PROMISE_ID;
            DataObject.ACTION = this.ACTION;
            DataObject.ORDER_DETAIL = this.ORDER_DETAIL;
            DataObject.CREATED_EMP = this.CREATED_EMP;
            DataObject.CREATED_TIME = this.CREATED_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.STATUS = this.STATUS;
            DataObject.SCHEDULE_ID = this.SCHEDULE_ID;
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
        public string ORDER_LINE_ID
        {
            get
            {
                return (string)this["ORDER_LINE_ID"];
            }
            set
            {
                this["ORDER_LINE_ID"] = value;
            }
        }
        public string PROMISE_ID
        {
            get
            {
                return (string)this["PROMISE_ID"];
            }
            set
            {
                this["PROMISE_ID"] = value;
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
        public byte[] ORDER_DETAIL
        {
            get
            {
                return (byte[])this["ORDER_DETAIL"];
            }
            set
            {
                this["ORDER_DETAIL"] = value;
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
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public double? VALID_FLAG
        {
            get
            {
                return (double?)this["VALID_FLAG"];
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
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }

        public string SCHEDULE_ID {
            get
            {
                return (string)this["SCHEDULE_ID"];
            }
            set
            {
                this["SCHEDULE_ID"] = value;
            }
        }
    }
    public class R_VT_ORDER
    {
        public string ID { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string ORDER_LINE_ID { get; set; }
        public string PROMISE_ID { get; set; }
        public string ACTION { get; set; }
        public byte[] ORDER_DETAIL { get; set; }
        public string CREATED_EMP { get; set; }
        public DateTime? CREATED_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public double? VALID_FLAG { get; set; }
        public string FILE_NAME { get; set; }
        public string STATUS { get; set; }

        public string SCHEDULE_ID { get; set; }
    }

    public enum OrderAction
    {
        [EnumName("Null")]
        [EnumValue("0")]
        Null,

        [EnumName("InsertOrUpdate")]
        [EnumValue("1")]
        InsertOrUpdate,

        [EnumName("Accept")]
        [EnumValue("2")]
        Accept,

        [EnumName("Update")]
        [EnumValue("3")]
        Update,

        [EnumName("Reject")]
        [EnumValue("4")]
        Reject,

        [EnumName("Cancelled")]
        [EnumValue("5")]
        Cancelled,

        [EnumName("Close")]
        [EnumValue("6")]
        Close,

        [EnumName("New")]
        [EnumValue("7")]
        New,
        
        [EnumName("Open")]
        [EnumValue("8")]
        Open

    }
    public enum OrderValidFlag
    {
        [EnumName("Invalid")]
        [EnumValue("0")]
        Invalid,

        [EnumName("Valid")]
        [EnumValue("1")]
        Valid

        //[EnumName("WaitComfirm")]
        //[EnumValue("2")]
        //WaitComfirm
    }
    public enum OrderStatus
    {
        [EnumName("WaitForCommit")]
        [EnumValue("0")]
        WaitForCommit,

        [EnumName("WaitForSendCommitFile")]
        [EnumValue("1")]
        WaitForSendCommitFile,

        [EnumName("WaitForCreatShipment")]
        [EnumValue("2")]
        WaitForCreatShipment,

        [EnumName("WaitForSendShipment")]
        [EnumValue("3")]
        WaitForSendShipment,

        [EnumName("Closed")]
        [EnumValue("4")]
        Closed,

        [EnumName("Reject")]
        [EnumValue("5")]
        Reject,

        [EnumName("WaitForE2openCommit")]
        [EnumValue("6")]
        WaitForE2openCommit,

        [EnumName("Cancelled")]
        [EnumValue("7")]
        Cancelled,

        [EnumName("CancelASN")]
        [EnumValue("8")]
        CancelASN
    }
}