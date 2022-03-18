using System;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_USER_FUNCTION : DataObjectTable
    {
        public T_C_USER_FUNCTION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_USER_FUNCTION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_USER_FUNCTION);
            TableName = "C_USER_FUNCTION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_USER_FUNCTION : DataObjectBase
    {
        public Row_C_USER_FUNCTION(DataObjectInfo info) : base(info)
        {

        }
        public C_USER_FUNCTION GetDataObject()
        {
            C_USER_FUNCTION DataObject = new C_USER_FUNCTION();
            DataObject.ID = this.ID;
            DataObject.FUNCTIONNAME = this.FUNCTIONNAME;
            DataObject.USERID = this.USERID;
            DataObject.ROLE = this.ROLE;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string FUNCTIONNAME
        {
            get
            {
                return (string)this["FUNCTIONNAME"];
            }
            set
            {
                this["FUNCTIONNAME"] = value;
            }
        }
        public string USERID
        {
            get
            {
                return (string)this["USERID"];
            }
            set
            {
                this["USERID"] = value;
            }
        }
        public string ROLE
        {
            get
            {
                return (string)this["ROLE"];
            }
            set
            {
                this["ROLE"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
    }
    public class C_USER_FUNCTION
    {
        public string ID { get; set; }
        public string FUNCTIONNAME { get; set; }
        public string USERID { get; set; }
        public string ROLE { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}