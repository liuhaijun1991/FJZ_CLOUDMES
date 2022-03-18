using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PACKING_SIZE : DataObjectTable
    {
        public T_C_PACKING_SIZE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PACKING_SIZE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PACKING_SIZE);
            TableName = "C_PACKING_SIZE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_PACKING_SIZE : DataObjectBase
    {
        public Row_C_PACKING_SIZE(DataObjectInfo info) : base(info)
        {

        }
        public C_PACKING_SIZE GetDataObject()
        {
            C_PACKING_SIZE DataObject = new C_PACKING_SIZE();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PALLET_SIZE = this.PALLET_SIZE;
            DataObject.CARTON_SIZE = this.CARTON_SIZE;
            DataObject.BOX_SIZE = this.BOX_SIZE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string PALLET_SIZE
        {
            get
            {
                return (string)this["PALLET_SIZE"];
            }
            set
            {
                this["PALLET_SIZE"] = value;
            }
        }
        public string CARTON_SIZE
        {
            get
            {
                return (string)this["CARTON_SIZE"];
            }
            set
            {
                this["CARTON_SIZE"] = value;
            }
        }
        public string BOX_SIZE
        {
            get
            {
                return (string)this["BOX_SIZE"];
            }
            set
            {
                this["BOX_SIZE"] = value;
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
    }
    public class C_PACKING_SIZE
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string PALLET_SIZE { get; set; }
        public string CARTON_SIZE { get; set; }
        public string BOX_SIZE { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}