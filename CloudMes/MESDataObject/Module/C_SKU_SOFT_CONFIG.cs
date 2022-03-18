using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MESDataObject.Module
{
    public class T_C_SKU_SOFT_CONFIG : DataObjectTable
    {
        public T_C_SKU_SOFT_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_SOFT_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_SOFT_CONFIG);
            TableName = "C_SKU_SOFT_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_SKU_SOFT_CONFIG : DataObjectBase
    {
        public Row_C_SKU_SOFT_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_SOFT_CONFIG GetDataObject()
        {
            C_SKU_SOFT_CONFIG DataObject = new C_SKU_SOFT_CONFIG();
            DataObject.ID = this.ID;
            DataObject.P_NO = this.P_NO;
            DataObject.P_VERSION = this.P_VERSION;
            DataObject.SOFT_ITEM_CODE = this.SOFT_ITEM_CODE;
            DataObject.SOFT_REVISION = this.SOFT_REVISION;
            DataObject.SOFT_LOCATION = this.SOFT_LOCATION;
            DataObject.SOFT_NUM = this.SOFT_NUM;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.CUSTOMER_KP_NO = this.CUSTOMER_KP_NO;
            DataObject.CUSTOMER_KP_NO_VER = this.CUSTOMER_KP_NO_VER;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string P_NO
        {
            get
            {
                return (string)this["P_NO"];
            }
            set
            {
                this["P_NO"] = value;
            }
        }
        public string P_VERSION
        {
            get
            {
                return (string)this["P_VERSION"];
            }
            set
            {
                this["P_VERSION"] = value;
            }
        }
        public string SOFT_ITEM_CODE
        {
            get
            {
                return (string)this["SOFT_ITEM_CODE"];
            }
            set
            {
                this["SOFT_ITEM_CODE"] = value;
            }
        }
        public string SOFT_REVISION
        {
            get
            {
                return (string)this["SOFT_REVISION"];
            }
            set
            {
                this["SOFT_REVISION"] = value;
            }
        }
        public string SOFT_LOCATION
        {
            get
            {
                return (string)this["SOFT_LOCATION"];
            }
            set
            {
                this["SOFT_LOCATION"] = value;
            }
        }
        public double? SOFT_NUM
        {
            get
            {
                return (double?)this["SOFT_NUM"];
            }
            set
            {
                this["SOFT_NUM"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string DATA4
        {
            get
            {
                return (string)this["DATA4"];
            }
            set
            {
                this["DATA4"] = value;
            }
        }
        public string CUSTOMER_KP_NO
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO"];
            }
            set
            {
                this["CUSTOMER_KP_NO"] = value;
            }
        }
        public string CUSTOMER_KP_NO_VER
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO_VER"];
            }
            set
            {
                this["CUSTOMER_KP_NO_VER"] = value;
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
        public string EDIT_TIME
        {
            get
            {
                return (string)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class C_SKU_SOFT_CONFIG
    {
        public string ID { get; set; }
        public string P_NO{ get; set; }
        public string P_VERSION{ get; set; }
        public string SOFT_ITEM_CODE{ get; set; }
        public string SOFT_REVISION{ get; set; }
        public string SOFT_LOCATION{ get; set; }
        public double? SOFT_NUM{ get; set; }
        public string DATA1{ get; set; }
        public string DATA2{ get; set; }
        public string DATA3{ get; set; }
        public string DATA4{ get; set; }
        public string CUSTOMER_KP_NO{ get; set; }
        public string CUSTOMER_KP_NO_VER{ get; set; }
        public string EDIT_EMP{ get; set; }
        public string EDIT_TIME{ get; set; }
    }
}