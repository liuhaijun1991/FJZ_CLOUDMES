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
    public class T_R_VT_FORECAST : DataObjectTable
    {
        public T_R_VT_FORECAST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_VT_FORECAST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_VT_FORECAST);
            TableName = "R_VT_FORECAST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_VT_FORECAST : DataObjectBase
    {
        public Row_R_VT_FORECAST(DataObjectInfo info) : base(info)
        {

        }
        public R_VT_FORECAST GetDataObject()
        {
            R_VT_FORECAST DataObject = new R_VT_FORECAST();
            DataObject.ID = this.ID;
            DataObject.CUSTOMER = this.CUSTOMER;
            DataObject.SUPPLIER = this.SUPPLIER;
            DataObject.CUSTOMER_ITEM_NAME = this.CUSTOMER_ITEM_NAME;
            DataObject.SUPPLIER_ITEM_NAME = this.SUPPLIER_ITEM_NAME;
            DataObject.SITE_NAME = this.SITE_NAME;
            DataObject.SUPPLIER_SITE_NAME = this.SUPPLIER_SITE_NAME;
            DataObject.DATA_MEASURE = this.DATA_MEASURE;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.FORECAST_DATE = this.FORECAST_DATE;
            DataObject.FLEXATTR_STRING_PIT_01 = this.FLEXATTR_STRING_PIT_01;
            DataObject.FLEXATTR_STRING_PIT_02 = this.FLEXATTR_STRING_PIT_02;
            DataObject.FLEXATTR_STRING_PIT_03 = this.FLEXATTR_STRING_PIT_03;
            DataObject.FLEXATTR_STRING_PIT_04 = this.FLEXATTR_STRING_PIT_04;
            DataObject.CREATED_EMP = this.CREATED_EMP;
            DataObject.CREATED_TIME = this.CREATED_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.LOT_NO = this.LOT_NO;
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
        public string FORECAST_DATE
        {
            get
            {
                return (string)this["FORECAST_DATE"];
            }
            set
            {
                this["FORECAST_DATE"] = value;
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
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }        
    }
    public class R_VT_FORECAST
    {
        public string ID { get; set; }
        public string CUSTOMER { get; set; }
        public string SUPPLIER { get; set; }
        public string CUSTOMER_ITEM_NAME { get; set; }
        public string SUPPLIER_ITEM_NAME { get; set; }
        public string SITE_NAME { get; set; }
        public string SUPPLIER_SITE_NAME { get; set; }
        public string DATA_MEASURE { get; set; }
        public double? QUANTITY { get; set; }
        public string FORECAST_DATE { get; set; }
        public string FLEXATTR_STRING_PIT_01 { get; set; }
        public string FLEXATTR_STRING_PIT_02 { get; set; }
        public string FLEXATTR_STRING_PIT_03 { get; set; }
        public string FLEXATTR_STRING_PIT_04 { get; set; }
        public string CREATED_EMP { get; set; }
        public DateTime? CREATED_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public double? VALID_FLAG { get; set; }
        public string FILE_NAME { get; set; }
        public string LOT_NO { get; set; }
    }

    public enum ForecastValid
    {
        [EnumName("Invalid")]
        [EnumValue("0")]
        Invalid,

        [EnumName("Valid")]
        [EnumValue("1")]
        Valid
    }
}