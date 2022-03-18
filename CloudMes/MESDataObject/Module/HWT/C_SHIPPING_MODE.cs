using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_C_SHIPPING_MODE : DataObjectTable
    {
        public T_C_SHIPPING_MODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SHIPPING_MODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SHIPPING_MODE);
            TableName = "C_SHIPPING_MODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_SHIPPING_MODE : DataObjectBase
    {
        public Row_C_SHIPPING_MODE(DataObjectInfo info) : base(info)
        {

        }
        public C_SHIPPING_MODE GetDataObject()
        {
            C_SHIPPING_MODE DataObject = new C_SHIPPING_MODE();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SHIPMODE = this.SHIPMODE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.SOURCE = this.SOURCE;
            DataObject.DEST = this.DEST;
            DataObject.PLANT = this.PLANT;
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
        public string SHIPMODE
        {
            get
            {
                return (string)this["SHIPMODE"];
            }
            set
            {
                this["SHIPMODE"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string SOURCE
        {
            get
            {
                return (string)this["SOURCE"];
            }
            set
            {
                this["SOURCE"] = value;
            }
        }
        public string DEST
        {
            get
            {
                return (string)this["DEST"];
            }
            set
            {
                this["DEST"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
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
    }
    public class C_SHIPPING_MODE
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string SHIPMODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string SOURCE { get; set; }
        public string DEST { get; set; }
        public string PLANT { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}