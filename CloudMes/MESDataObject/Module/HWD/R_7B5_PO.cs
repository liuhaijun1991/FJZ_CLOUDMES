using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_PO : DataObjectTable
    {
        public T_R_7B5_PO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_PO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_PO);
            TableName = "R_7B5_PO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool TaskIsExist(OleExec sfcdb, string task_no)
        {
            return sfcdb.ORM.Queryable<R_7B5_PO>().Any(r => r.TASK_NO == task_no);
        }
    }
    public class Row_R_7B5_PO : DataObjectBase
    {
        public Row_R_7B5_PO(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_PO GetDataObject()
        {
            R_7B5_PO DataObject = new R_7B5_PO();
            DataObject.SAP_MESSAGE = this.SAP_MESSAGE;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.UPLOADBY = this.UPLOADBY;
            DataObject.UPLOADDT = this.UPLOADDT;
            DataObject.PRICE_UNIT = this.PRICE_UNIT;
            DataObject.BASE_QTY = this.BASE_QTY;
            DataObject.CURRENCY = this.CURRENCY;
            DataObject.PRICE = this.PRICE;
            DataObject.SALES_UNIT = this.SALES_UNIT;
            DataObject.QTY = this.QTY;
            DataObject.ITEM = this.ITEM;
            DataObject.PO_NO = this.PO_NO;
            DataObject.TASK_NO = this.TASK_NO;
            return DataObject;
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
        public string UPLOADBY
        {
            get
            {
                return (string)this["UPLOADBY"];
            }
            set
            {
                this["UPLOADBY"] = value;
            }
        }
        public DateTime? UPLOADDT
        {
            get
            {
                return (DateTime?)this["UPLOADDT"];
            }
            set
            {
                this["UPLOADDT"] = value;
            }
        }
        public string PRICE_UNIT
        {
            get
            {
                return (string)this["PRICE_UNIT"];
            }
            set
            {
                this["PRICE_UNIT"] = value;
            }
        }
        public double? BASE_QTY
        {
            get
            {
                return (double?)this["BASE_QTY"];
            }
            set
            {
                this["BASE_QTY"] = value;
            }
        }
        public string CURRENCY
        {
            get
            {
                return (string)this["CURRENCY"];
            }
            set
            {
                this["CURRENCY"] = value;
            }
        }
        public double? PRICE
        {
            get
            {
                return (double?)this["PRICE"];
            }
            set
            {
                this["PRICE"] = value;
            }
        }
        public string SALES_UNIT
        {
            get
            {
                return (string)this["SALES_UNIT"];
            }
            set
            {
                this["SALES_UNIT"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
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
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
            }
        }
    }
    public class R_7B5_PO
    {
        public string SAP_MESSAGE { get; set; }
        public string SAP_FLAG { get; set; }
        public string UPLOADBY { get; set; }
        public DateTime? UPLOADDT { get; set; }
        public string PRICE_UNIT { get; set; }
        public double? BASE_QTY { get; set; }
        public string CURRENCY { get; set; }
        public double? PRICE { get; set; }
        public string SALES_UNIT { get; set; }
        public double? QTY { get; set; }
        public string ITEM { get; set; }
        public string PO_NO { get; set; }
        public string TASK_NO { get; set; }
    }
}