using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PARTNO_MAPPING : DataObjectTable
    {
        public T_C_PARTNO_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PARTNO_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PARTNO_MAPPING);
            TableName = "C_PARTNO_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_PARTNO_MAPPING : DataObjectBase
    {
        public Row_C_PARTNO_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public C_PARTNO_MAPPING GetDataObject()
        {
            C_PARTNO_MAPPING DataObject = new C_PARTNO_MAPPING();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CTYPE = this.CTYPE;
            DataObject.VALUE1 = this.VALUE1;
            DataObject.VALUE2 = this.VALUE2;
            DataObject.VALUE3 = this.VALUE3;
            DataObject.VALUE4 = this.VALUE4;
            DataObject.VALUE5 = this.VALUE5;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
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
        public string CTYPE
        {
            get
            {
                return (string)this["CTYPE"];
            }
            set
            {
                this["CTYPE"] = value;
            }
        }
        public string VALUE1
        {
            get
            {
                return (string)this["VALUE1"];
            }
            set
            {
                this["VALUE1"] = value;
            }
        }
        public string VALUE2
        {
            get
            {
                return (string)this["VALUE2"];
            }
            set
            {
                this["VALUE2"] = value;
            }
        }
        public string VALUE3
        {
            get
            {
                return (string)this["VALUE3"];
            }
            set
            {
                this["VALUE3"] = value;
            }
        }
        public string VALUE4
        {
            get
            {
                return (string)this["VALUE4"];
            }
            set
            {
                this["VALUE4"] = value;
            }
        }
        public string VALUE5
        {
            get
            {
                return (string)this["VALUE5"];
            }
            set
            {
                this["VALUE5"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
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
    public class C_PARTNO_MAPPING
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string CTYPE { get; set; }
        public string VALUE1 { get; set; }
        public string VALUE2 { get; set; }
        public string VALUE3 { get; set; }
        public string VALUE4 { get; set; }
        public string VALUE5 { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}