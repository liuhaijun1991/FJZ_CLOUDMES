using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module.Juniper
{
    public class T_R_TEST_JSNLIST : DataObjectTable
    {
        public T_R_TEST_JSNLIST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_JSNLIST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_JSNLIST);
            TableName = "R_TEST_JSNLIST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_TEST_JSNLIST : DataObjectBase
    {
        public Row_R_TEST_JSNLIST(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_JSNLIST GetDataObject()
        {
            R_TEST_JSNLIST DataObject = new R_TEST_JSNLIST();
            DataObject.ID = this.ID;
            DataObject.SERIALNO = this.SERIALNO;
            DataObject.STATUS = this.STATUS;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.CREAT_TIME = this.CREAT_TIME;
            DataObject.CREAT_EMP = this.CREAT_EMP;
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
        public string SERIALNO
        {
            get
            {
                return (string)this["SERIALNO"];
            }
            set
            {
                this["SERIALNO"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
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
        public DateTime? CREAT_TIME
        {
            get
            {
                return (DateTime?)this["CREAT_TIME"];
            }
            set
            {
                this["CREAT_TIME"] = value;
            }
        }
        public string CREAT_EMP
        {
            get
            {
                return (string)this["CREAT_EMP"];
            }
            set
            {
                this["CREAT_EMP"] = value;
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
    public class R_TEST_JSNLIST
    {
        public string ID { get; set; }
        public string SERIALNO { get; set; }
        public string STATUS { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? CREAT_TIME { get; set; }
        public string CREAT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}