using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_STATION_SCAN_LOG : DataObjectTable
    {
        public T_C_STATION_SCAN_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_STATION_SCAN_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_STATION_SCAN_LOG);
            TableName = "C_STATION_SCAN_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_STATION_SCAN_LOG : DataObjectBase
    {
        public Row_C_STATION_SCAN_LOG(DataObjectInfo info) : base(info)
        {

        }
        public C_STATION_SCAN_LOG GetDataObject()
        {
            C_STATION_SCAN_LOG DataObject = new C_STATION_SCAN_LOG();
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.ENABLEFLAG = this.ENABLEFLAG;
            DataObject.EXCEPTIONFLAG = this.EXCEPTIONFLAG;
            DataObject.CLASSNAME = this.CLASSNAME;
            DataObject.ACTIONNAME = this.ACTIONNAME;
            DataObject.EMPNO = this.EMPNO;
            DataObject.IP = this.IP;
            DataObject.LINE = this.LINE;
            DataObject.STATION = this.STATION;
            DataObject.ERRMSG = this.ERRMSG;
            DataObject.SCANKEY = this.SCANKEY;
            DataObject.ID = this.ID;
            return DataObject;
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
        public string EXCEPTIONFLAG
        {
            get
            {
                return (string)this["EXCEPTIONFLAG"];
            }
            set
            {
                this["EXCEPTIONFLAG"] = value;
            }
        }
        public string CLASSNAME
        {
            get
            {
                return (string)this["CLASSNAME"];
            }
            set
            {
                this["CLASSNAME"] = value;
            }
        }
        public string ACTIONNAME
        {
            get
            {
                return (string)this["ACTIONNAME"];
            }
            set
            {
                this["ACTIONNAME"] = value;
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
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string ERRMSG
        {
            get
            {
                return (string)this["ERRMSG"];
            }
            set
            {
                this["ERRMSG"] = value;
            }
        }
        public string SCANKEY
        {
            get
            {
                return (string)this["SCANKEY"];
            }
            set
            {
                this["SCANKEY"] = value;
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
    }
    public class C_STATION_SCAN_LOG
    {
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string ENABLEFLAG { get; set; }
        public string EXCEPTIONFLAG { get; set; }
        public string CLASSNAME { get; set; }
        public string ACTIONNAME { get; set; }
        public string EMPNO { get; set; }
        public string IP { get; set; }
        public string LINE { get; set; }
        public string STATION { get; set; }
        public string ERRMSG { get; set; }
        public string SCANKEY { get; set; }
        public string ID { get; set; }

        public static List<C_STATION_SCAN_LOG> getCStationScanLogs(OleExec DB)
        {
           return DB.ORM.Queryable<C_STATION_SCAN_LOG>().ToList();
        }
    }
}