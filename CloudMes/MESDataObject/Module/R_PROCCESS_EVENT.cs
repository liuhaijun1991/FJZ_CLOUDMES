using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PROCCESS_EVENT : DataObjectTable
    {
        public T_R_PROCCESS_EVENT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PROCCESS_EVENT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PROCCESS_EVENT);
            TableName = "R_PROCCESS_EVENT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PROCCESS_EVENT : DataObjectBase
    {
        public Row_R_PROCCESS_EVENT(DataObjectInfo info) : base(info)
        {

        }
        public R_PROCCESS_EVENT GetDataObject()
        {
            R_PROCCESS_EVENT DataObject = new R_PROCCESS_EVENT();
            DataObject.ID = this.ID;
            DataObject.PROCCESS_NAME = this.PROCCESS_NAME;
            DataObject.EVENT_TYPE = this.EVENT_TYPE;
            DataObject.MESSAGE = this.MESSAGE;
            DataObject.EVENT_LV = this.EVENT_LV;
            DataObject.RUNTIME_ID = this.RUNTIME_ID;
            DataObject.IP = this.IP;
            DataObject.STATE = this.STATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_DATE = this.EDIT_DATE;
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
        public string PROCCESS_NAME
        {
            get
            {
                return (string)this["PROCCESS_NAME"];
            }
            set
            {
                this["PROCCESS_NAME"] = value;
            }
        }
        public string EVENT_TYPE
        {
            get
            {
                return (string)this["EVENT_TYPE"];
            }
            set
            {
                this["EVENT_TYPE"] = value;
            }
        }
        public string MESSAGE
        {
            get
            {
                return (string)this["MESSAGE"];
            }
            set
            {
                this["MESSAGE"] = value;
            }
        }
        public double? EVENT_LV
        {
            get
            {
                return (double?)this["EVENT_LV"];
            }
            set
            {
                this["EVENT_LV"] = value;
            }
        }
        public string RUNTIME_ID
        {
            get
            {
                return (string)this["RUNTIME_ID"];
            }
            set
            {
                this["RUNTIME_ID"] = value;
            }
        }
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
            }
        }
        public string STATE
        {
            get
            {
                return (string)this["STATE"];
            }
            set
            {
                this["STATE"] = value;
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
        public DateTime? EDIT_DATE
        {
            get
            {
                return (DateTime?)this["EDIT_DATE"];
            }
            set
            {
                this["EDIT_DATE"] = value;
            }
        }
    }
    public class R_PROCCESS_EVENT
    {
        public string ID { get; set; }
        public string PROCCESS_NAME { get; set; }
        public string EVENT_TYPE { get; set; }
        public string MESSAGE { get; set; }
        public double? EVENT_LV { get; set; }
        public string RUNTIME_ID { get; set; }
        public string IP { get; set; }
        public string STATE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_DATE { get; set; }
    }
}