using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MAILFUNC_USER : DataObjectTable
    {
        public T_R_MAILFUNC_USER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MAILFUNC_USER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MAILFUNC_USER);
            TableName = "R_MAILFUNC_USER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MAILFUNC_USER : DataObjectBase
    {
        public Row_R_MAILFUNC_USER(DataObjectInfo info) : base(info)
        {

        }
        public R_MAILFUNC_USER GetDataObject()
        {
            R_MAILFUNC_USER DataObject = new R_MAILFUNC_USER();
            DataObject.ID = this.ID;
            DataObject.FUNCTIONNAME = this.FUNCTIONNAME;
            DataObject.EMPNO = this.EMPNO;
            DataObject.GROUPNAME = this.GROUPNAME;
            DataObject.UTYPE = this.UTYPE;
            DataObject.ENABLEFLAG = this.ENABLEFLAG;
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
        public string EMPNO
        {
            get
            {
                return (string)this["EMPNO"];
            }
            set
            {
                this["EMPNO"] = value;
            }
        }
        public string GROUPNAME
        {
            get
            {
                return (string)this["GROUPNAME"];
            }
            set
            {
                this["GROUPNAME"] = value;
            }
        }
        public string UTYPE
        {
            get
            {
                return (string)this["UTYPE"];
            }
            set
            {
                this["UTYPE"] = value;
            }
        }
        public string ENABLEFLAG
        {
            get
            {
                return (string)this["ENABLEFLAG"];
            }
            set
            {
                this["ENABLEFLAG"] = value;
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
    public class R_MAILFUNC_USER
    {
        public string ID { get; set; }
        public string FUNCTIONNAME { get; set; }
        public string EMPNO { get; set; }
        public string GROUPNAME { get; set; }
        public string UTYPE { get; set; }
        public string ENABLEFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}