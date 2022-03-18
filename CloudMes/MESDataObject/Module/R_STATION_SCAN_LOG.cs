using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_STATION_SCAN_LOG : DataObjectTable
    {
        public T_R_STATION_SCAN_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_STATION_SCAN_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_STATION_SCAN_LOG);
            TableName = "R_STATION_SCAN_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_STATION_SCAN_LOG : DataObjectBase
    {
        public Row_R_STATION_SCAN_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_STATION_SCAN_LOG GetDataObject()
        {
            R_STATION_SCAN_LOG DataObject = new R_STATION_SCAN_LOG();
            DataObject.DEFECT1 = this.DEFECT1;
            DataObject.DEFECT2 = this.DEFECT2;
            DataObject.REMARK = this.REMARK;
            DataObject.SCANKEY = this.SCANKEY;
            DataObject.ERRMSG = this.ERRMSG;
            DataObject.STATION = this.STATION;
            DataObject.LINE = this.LINE;
            DataObject.IP = this.IP;
            DataObject.EMPNO = this.EMPNO;
            DataObject.ACTIONNAME = this.ACTIONNAME;
            DataObject.CLASSNAME = this.CLASSNAME;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            return DataObject;
        }
        public string DEFECT1
        {
            get
            {
                return (string)this["DEFECT1"];
            }
            set
            {
                this["DEFECT1"] = value;
            }
        }
        public string DEFECT2
        {
            get
            {
                return (string)this["DEFECT2"];
            }
            set
            {
                this["DEFECT2"] = value;
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
    public class R_STATION_SCAN_LOG
    {
        public string DEFECT1 { get; set; }
        public string DEFECT2 { get; set; }
        public string REMARK { get; set; }
        public string SCANKEY { get; set; }
        public string ERRMSG { get; set; }
        public string STATION { get; set; }
        public string LINE { get; set; }
        public string IP { get; set; }
        public string EMPNO { get; set; }
        public string ACTIONNAME { get; set; }
        public string CLASSNAME { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}