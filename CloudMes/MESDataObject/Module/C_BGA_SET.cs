using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_BGA_SET : DataObjectTable
    {
        public T_C_BGA_SET(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_BGA_SET(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_BGA_SET);
            TableName = "C_BGA_SET".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_BGA_SET : DataObjectBase
    {
        public Row_C_BGA_SET(DataObjectInfo info) : base(info)
        {

        }
        public C_BGA_SET GetDataObject()
        {
            C_BGA_SET DataObject = new C_BGA_SET();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.LOC_TIMES = this.LOC_TIMES;
            DataObject.TIMES = this.TIMES;
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
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string LOC_TIMES
        {
            get
            {
                return (string)this["LOC_TIMES"];
            }
            set
            {
                this["LOC_TIMES"] = value;
            }
        }
        public string TIMES
        {
            get
            {
                return (string)this["TIMES"];
            }
            set
            {
                this["TIMES"] = value;
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
    public class C_BGA_SET
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string PARTNO { get; set; }
        public string LOC_TIMES { get; set; }
        public string TIMES { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}