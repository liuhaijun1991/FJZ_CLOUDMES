using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ECN_FILE : DataObjectTable
    {
        public T_R_ECN_FILE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ECN_FILE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ECN_FILE);
            TableName = "R_ECN_FILE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ECN_FILE : DataObjectBase
    {
        public Row_R_ECN_FILE(DataObjectInfo info) : base(info)
        {

        }
        public R_ECN_FILE GetDataObject()
        {
            R_ECN_FILE DataObject = new R_ECN_FILE();
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.ECN_NUMBER = this.ECN_NUMBER;
            DataObject.ECN_DATE = this.ECN_DATE;
            return DataObject;
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
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
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string ECN_NUMBER
        {
            get
            {
                return (string)this["ECN_NUMBER"];
            }
            set
            {
                this["ECN_NUMBER"] = value;
            }
        }
        public DateTime? ECN_DATE
        {
            get
            {
                return (DateTime?)this["ECN_DATE"];
            }
            set
            {
                this["ECN_DATE"] = value;
            }
        }
    }
    public class R_ECN_FILE
    {
        public string VALID_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string ECN_NUMBER { get; set; }
        public DateTime? ECN_DATE { get; set; }
    }
}