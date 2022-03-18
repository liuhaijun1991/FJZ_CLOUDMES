using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_O_EXCEPTION_DATA : DataObjectTable
    {
        public T_O_EXCEPTION_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_EXCEPTION_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_EXCEPTION_DATA);
            TableName = "O_EXCEPTION_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_EXCEPTION_DATA : DataObjectBase
    {
        public Row_O_EXCEPTION_DATA(DataObjectInfo info) : base(info)
        {

        }
        public O_EXCEPTION_DATA GetDataObject()
        {
            O_EXCEPTION_DATA DataObject = new O_EXCEPTION_DATA();
            DataObject.ID = this.ID;
            DataObject.UPOID = this.UPOID;
            DataObject.ORIGINALID = this.ORIGINALID;
            DataObject.EXCTYPE = this.EXCTYPE;
            DataObject.EXCEPTIONINFO = this.EXCEPTIONINFO;
            DataObject.EXCEPCODE = this.EXCEPCODE;
            DataObject.STATUS = this.STATUS;
            DataObject.MAILFLAG = this.MAILFLAG;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string UPOID
        {
            get
            {
                return (string)this["UPOID"];
            }
            set
            {
                this["UPOID"] = value;
            }
        }
        public string ORIGINALID
        {
            get
            {
                return (string)this["ORIGINALID"];
            }
            set
            {
                this["ORIGINALID"] = value;
            }
        }
        public string EXCTYPE
        {
            get
            {
                return (string)this["EXCTYPE"];
            }
            set
            {
                this["EXCTYPE"] = value;
            }
        }
        public string EXCEPTIONINFO
        {
            get
            {
                return (string)this["EXCEPTIONINFO"];
            }
            set
            {
                this["EXCEPTIONINFO"] = value;
            }
        }
        public string EXCEPCODE
        {
            get
            {
                return (string)this["EXCEPCODE"];
            }
            set
            {
                this["EXCEPCODE"] = value;
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
        public string MAILFLAG
        {
            get
            {
                return (string)this["MAILFLAG"];
            }
            set
            {
                this["MAILFLAG"] = value;
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
    }
    public class O_EXCEPTION_DATA
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string UPOID { get; set; }
        public string ORIGINALID { get; set; }
        public string EXCTYPE { get; set; }
        public string EXCEPTIONINFO { get; set; }
        public string EXCEPCODE { get; set; }
        public string STATUS { get; set; }
        public string MAILFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public string MAILID { get; set; }
        public DateTime? EDITTIME { get; set; }
    }
}