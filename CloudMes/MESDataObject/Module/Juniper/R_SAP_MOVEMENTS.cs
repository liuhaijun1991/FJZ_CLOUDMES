using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP_MOVEMENTS : DataObjectTable
    {
        public T_R_SAP_MOVEMENTS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_MOVEMENTS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_MOVEMENTS);
            TableName = "R_SAP_MOVEMENTS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_MOVEMENTS : DataObjectBase
    {
        public Row_R_SAP_MOVEMENTS(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_MOVEMENTS GetDataObject()
        {
            R_SAP_MOVEMENTS DataObject = new R_SAP_MOVEMENTS();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SN = this.SN;
            DataObject.TOTAL_QTY = this.TOTAL_QTY;
            DataObject.FROM_STORAGE = this.FROM_STORAGE;
            DataObject.TO_STORAGE = this.TO_STORAGE;
            DataObject.CONFIRMED_FLAG = this.CONFIRMED_FLAG;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.SAP_MESSAGE = this.SAP_MESSAGE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.BACKFLUSH_TIME = this.BACKFLUSH_TIME;
            DataObject.SAP_STATION_CODE = this.SAP_STATION_CODE;
            DataObject.DOCUMENT_ID = this.DOCUMENT_ID;
            DataObject.STOCK_TYPE = this.STOCK_TYPE;
            DataObject.MOVEMENT_TYPE = this.MOVEMENT_TYPE;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public double? TOTAL_QTY
        {
            get
            {
                return (double?)this["TOTAL_QTY"];
            }
            set
            {
                this["TOTAL_QTY"] = value;
            }
        }
        public string FROM_STORAGE
        {
            get
            {
                return (string)this["FROM_STORAGE"];
            }
            set
            {
                this["FROM_STORAGE"] = value;
            }
        }
        public string TO_STORAGE
        {
            get
            {
                return (string)this["TO_STORAGE"];
            }
            set
            {
                this["TO_STORAGE"] = value;
            }
        }
        public string CONFIRMED_FLAG
        {
            get
            {
                return (string)this["CONFIRMED_FLAG"];
            }
            set
            {
                this["CONFIRMED_FLAG"] = value;
            }
        }
        public string SAP_FLAG
        {
            get
            {
                return (string)this["SAP_FLAG"];
            }
            set
            {
                this["SAP_FLAG"] = value;
            }
        }
        public string SAP_MESSAGE
        {
            get
            {
                return (string)this["SAP_MESSAGE"];
            }
            set
            {
                this["SAP_MESSAGE"] = value;
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
        public DateTime? BACKFLUSH_TIME
        {
            get
            {
                return (DateTime?)this["BACKFLUSH_TIME"];
            }
            set
            {
                this["BACKFLUSH_TIME"] = value;
            }
        }
        public string SAP_STATION_CODE
        {
            get
            {
                return (string)this["SAP_STATION_CODE"];
            }
            set
            {
                this["SAP_STATION_CODE"] = value;
            }
        }
        public string DOCUMENT_ID
        {
            get
            {
                return (string)this["DOCUMENT_ID"];
            }
            set
            {
                this["DOCUMENT_ID"] = value;
            }
        }
        public string STOCK_TYPE
        {
            get
            {
                return (string)this["STOCK_TYPE"];
            }
            set
            {
                this["STOCK_TYPE"] = value;
            }
        }
        public string MOVEMENT_TYPE
        {
            get
            {
                return (string)this["MOVEMENT_TYPE"];
            }
            set
            {
                this["MOVEMENT_TYPE"] = value;
            }
        }
    }
    public class R_SAP_MOVEMENTS
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string SN { get; set; }
        public double? TOTAL_QTY { get; set; }
        public string FROM_STORAGE { get; set; }
        public string TO_STORAGE { get; set; }
        public string CONFIRMED_FLAG { get; set; }
        public string SAP_FLAG { get; set; }
        public string SAP_MESSAGE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public DateTime? BACKFLUSH_TIME { get; set; }
        public string SAP_STATION_CODE { get; set; }
        public string DOCUMENT_ID { get; set; }
        public string STOCK_TYPE { get; set; }
        public string MOVEMENT_TYPE { get; set; }
    }
}