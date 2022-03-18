using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_VT_COMMIT : DataObjectTable
    {
        public T_R_VT_COMMIT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_VT_COMMIT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_VT_COMMIT);
            TableName = "R_VT_COMMIT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_VT_COMMIT : DataObjectBase
    {
        public Row_R_VT_COMMIT(DataObjectInfo info) : base(info)
        {

        }
        public R_VT_FORECAST_COMMIT GetDataObject()
        {
            R_VT_FORECAST_COMMIT DataObject = new R_VT_FORECAST_COMMIT();
            DataObject.ID = this.ID;
            DataObject.COMMIT_ID = this.COMMIT_ID;
            DataObject.CUSTOMER = this.CUSTOMER;
            DataObject.SUPPLIER = this.SUPPLIER;
            DataObject.CUSTOMER_ITEM_NAME = this.CUSTOMER_ITEM_NAME;
            DataObject.SUPPLIER_ITEM_NAME = this.SUPPLIER_ITEM_NAME;
            DataObject.SITE_NAME = this.SITE_NAME;
            DataObject.SUPPLIER_SITE_NAME = this.SUPPLIER_SITE_NAME;
            DataObject.DATA_MEASURE = this.DATA_MEASURE;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.COMMIT_DATE = this.COMMIT_DATE;
            DataObject.FLEXATTR_STRING_PIT_01 = this.FLEXATTR_STRING_PIT_01;
            DataObject.FLEXATTR_STRING_PIT_02 = this.FLEXATTR_STRING_PIT_02;
            DataObject.FLEXATTR_STRING_PIT_03 = this.FLEXATTR_STRING_PIT_03;
            DataObject.FLEXATTR_STRING_PIT_04 = this.FLEXATTR_STRING_PIT_04;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.SEND_TIME = this.SEND_TIME;
            DataObject.SEND_FILE_NAME = this.SEND_FILE_NAME;
            DataObject.CREATED_EMP = this.CREATED_EMP;
            DataObject.CREATED_TIME = this.CREATED_TIME;            
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
        public string COMMIT_ID
        {
            get
            {
                return (string)this["COMMIT_ID"];
            }
            set
            {
                this["COMMIT_ID"] = value;
            }
        }
        public string CUSTOMER
        {
            get
            {
                return (string)this["CUSTOMER"];
            }
            set
            {
                this["CUSTOMER"] = value;
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
        public string CUSTOMER_ITEM_NAME
        {
            get
            {
                return (string)this["CUSTOMER_ITEM_NAME"];
            }
            set
            {
                this["CUSTOMER_ITEM_NAME"] = value;
            }
        }
        public string SUPPLIER_ITEM_NAME
        {
            get
            {
                return (string)this["SUPPLIER_ITEM_NAME"];
            }
            set
            {
                this["SUPPLIER_ITEM_NAME"] = value;
            }
        }
        public string SITE_NAME
        {
            get
            {
                return (string)this["SITE_NAME"];
            }
            set
            {
                this["SITE_NAME"] = value;
            }
        }
        public string SUPPLIER_SITE_NAME
        {
            get
            {
                return (string)this["SUPPLIER_SITE_NAME"];
            }
            set
            {
                this["SUPPLIER_SITE_NAME"] = value;
            }
        }
        public string DATA_MEASURE
        {
            get
            {
                return (string)this["DATA_MEASURE"];
            }
            set
            {
                this["DATA_MEASURE"] = value;
            }
        }
        public double? QUANTITY
        {
            get
            {
                return (double?)this["QUANTITY"];
            }
            set
            {
                this["QUANTITY"] = value;
            }
        }
        public string COMMIT_DATE
        {
            get
            {
                return (string)this["COMMIT_DATE"];
            }
            set
            {
                this["COMMIT_DATE"] = value;
            }
        }
        public string FLEXATTR_STRING_PIT_01
        {
            get
            {
                return (string)this["FLEXATTR_STRING_PIT_01"];
            }
            set
            {
                this["FLEXATTR_STRING_PIT_01"] = value;
            }
        }
        public string FLEXATTR_STRING_PIT_02
        {
            get
            {
                return (string)this["FLEXATTR_STRING_PIT_02"];
            }
            set
            {
                this["FLEXATTR_STRING_PIT_02"] = value;
            }
        }
        public string FLEXATTR_STRING_PIT_03
        {
            get
            {
                return (string)this["FLEXATTR_STRING_PIT_03"];
            }
            set
            {
                this["FLEXATTR_STRING_PIT_03"] = value;
            }
        }
        public string FLEXATTR_STRING_PIT_04
        {
            get
            {
                return (string)this["FLEXATTR_STRING_PIT_04"];
            }
            set
            {
                this["FLEXATTR_STRING_PIT_04"] = value;
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
        public double? SEND_FLAG
        {
            get
            {
                return (double?)this["SEND_FLAG"];
            }
            set
            {
                this["SEND_FLAG"] = value;
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
        public string SEND_FILE_NAME
        {
            get
            {
                return (string)this["SEND_FILE_NAME"];
            }
            set
            {
                this["SEND_FILE_NAME"] = value;
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
    }
    public class R_VT_FORECAST_COMMIT
    {
        public string ID { get; set; }
        public string COMMIT_ID { get; set; }
        public string CUSTOMER { get; set; }
        public string SUPPLIER { get; set; }
        public string CUSTOMER_ITEM_NAME { get; set; }
        public string SUPPLIER_ITEM_NAME { get; set; }
        public string SITE_NAME { get; set; }
        public string SUPPLIER_SITE_NAME { get; set; }
        public string DATA_MEASURE { get; set; }
        public double? QUANTITY { get; set; }
        public string COMMIT_DATE { get; set; }
        public string FLEXATTR_STRING_PIT_01 { get; set; }
        public string FLEXATTR_STRING_PIT_02 { get; set; }
        public string FLEXATTR_STRING_PIT_03 { get; set; }
        public string FLEXATTR_STRING_PIT_04 { get; set; }
        public double? VALID_FLAG { get; set; }
        public double? SEND_FLAG { get; set; }
        public DateTime? SEND_TIME { get; set; }
        public string SEND_FILE_NAME { get; set; }
        public string CREATED_EMP { get; set; }
        public DateTime? CREATED_TIME { get; set; }
    }
}