using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ECN_WO : DataObjectTable
    {
        public T_R_ECN_WO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ECN_WO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ECN_WO);
            TableName = "R_ECN_WO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ECN_WO : DataObjectBase
    {
        public Row_R_ECN_WO(DataObjectInfo info) : base(info)
        {

        }
        public R_ECN_WO GetDataObject()
        {
            R_ECN_WO DataObject = new R_ECN_WO();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.OLD_VERSION = this.OLD_VERSION;
            DataObject.SKUNO_73 = this.SKUNO_73;
            DataObject.VERSION_73 = this.VERSION_73;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.REMARK = this.REMARK;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
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
        public string OLD_VERSION
        {
            get
            {
                return (string)this["OLD_VERSION"];
            }
            set
            {
                this["OLD_VERSION"] = value;
            }
        }
        public string SKUNO_73
        {
            get
            {
                return (string)this["SKUNO_73"];
            }
            set
            {
                this["SKUNO_73"] = value;
            }
        }
        public string VERSION_73
        {
            get
            {
                return (string)this["VERSION_73"];
            }
            set
            {
                this["VERSION_73"] = value;
            }
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
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
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
    public class R_ECN_WO
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string OLD_VERSION { get; set; }
        public string SKUNO_73 { get; set; }
        public string VERSION_73 { get; set; }
        public string VALID_FLAG { get; set; }
        public string REMARK { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}