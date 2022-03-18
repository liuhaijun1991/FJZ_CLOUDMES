using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_CUSTOMER_FILE_FORMAT : DataObjectTable
    {
        public T_C_CUSTOMER_FILE_FORMAT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CUSTOMER_FILE_FORMAT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CUSTOMER_FILE_FORMAT);
            TableName = "C_CUSTOMER_FILE_FORMAT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_CUSTOMER_FILE_FORMAT : DataObjectBase
    {
        public Row_C_CUSTOMER_FILE_FORMAT(DataObjectInfo info) : base(info)
        {

        }
        public C_CUSTOMER_FILE_FORMAT GetDataObject()
        {
            C_CUSTOMER_FILE_FORMAT DataObject = new C_CUSTOMER_FILE_FORMAT();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.FIELD_NAME = this.FIELD_NAME;
            DataObject.DISPLAY_NAME = this.DISPLAY_NAME;
            DataObject.FIELD_DESCRIPTION = this.FIELD_DESCRIPTION;
            DataObject.DATA_TYPE = this.DATA_TYPE;
            DataObject.REQUIRED = this.REQUIRED;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_NAME = this.EDIT_NAME;
            DataObject.SEQ = this.SEQ;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
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
        public string FIELD_NAME
        {
            get
            {
                return (string)this["FIELD_NAME"];
            }
            set
            {
                this["FIELD_NAME"] = value;
            }
        }
        public string DISPLAY_NAME
        {
            get
            {
                return (string)this["DISPLAY_NAME"];
            }
            set
            {
                this["DISPLAY_NAME"] = value;
            }
        }
        public string FIELD_DESCRIPTION
        {
            get
            {
                return (string)this["FIELD_DESCRIPTION"];
            }
            set
            {
                this["FIELD_DESCRIPTION"] = value;
            }
        }
        public string DATA_TYPE
        {
            get
            {
                return (string)this["DATA_TYPE"];
            }
            set
            {
                this["DATA_TYPE"] = value;
            }
        }
        public string REQUIRED
        {
            get
            {
                return (string)this["REQUIRED"];
            }
            set
            {
                this["REQUIRED"] = value;
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
        public string EDIT_NAME
        {
            get
            {
                return (string)this["EDIT_NAME"];
            }
            set
            {
                this["EDIT_NAME"] = value;
            }
        }
        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
    }
    public class C_CUSTOMER_FILE_FORMAT
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string BU { get; set; }
        public string FILE_NAME { get; set; }
        public string FIELD_NAME { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string FIELD_DESCRIPTION { get; set; }
        public string DATA_TYPE { get; set; }
        public string REQUIRED { get; set; }
        public double? VALID_FLAG { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_NAME { get; set; }
        public double? SEQ { get; set; }        
    }
}