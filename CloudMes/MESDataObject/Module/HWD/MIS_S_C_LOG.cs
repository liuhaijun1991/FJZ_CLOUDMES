using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_MIS_S_C_LOG : DataObjectTable
    {
        public T_MIS_S_C_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_MIS_S_C_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_MIS_S_C_LOG);
            TableName = "MIS_S_C_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_MIS_S_C_LOG : DataObjectBase
    {
        public Row_MIS_S_C_LOG(DataObjectInfo info) : base(info)
        {

        }
        public MIS_S_C_LOG GetDataObject()
        {
            MIS_S_C_LOG DataObject = new MIS_S_C_LOG();
            DataObject.ID = this.ID;
            DataObject.SERVERID = this.SERVERID;
            DataObject.SHADU = this.SHADU;
            DataObject.PWD = this.PWD;
            DataObject.TIMES = this.TIMES;
            DataObject.EXCEPTINFO = this.EXCEPTINFO;
            DataObject.SYSRESOURCE = this.SYSRESOURCE;
            DataObject.BAKUP = this.BAKUP;
            DataObject.CHECKEMP = this.CHECKEMP;
            DataObject.CHECKTIME = this.CHECKTIME;
            DataObject.REMARK = this.REMARK;
            DataObject.ISWSUS = this.ISWSUS;
            DataObject.ISSET = this.ISSET;
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
        public string SERVERID
        {
            get
            {
                return (string)this["SERVERID"];
            }
            set
            {
                this["SERVERID"] = value;
            }
        }
        public string SHADU
        {
            get
            {
                return (string)this["SHADU"];
            }
            set
            {
                this["SHADU"] = value;
            }
        }
        public string PWD
        {
            get
            {
                return (string)this["PWD"];
            }
            set
            {
                this["PWD"] = value;
            }
        }
        public string TIMES
        {
            get
            {
                return (string)this["TIMES"];
            }
            set
            {
                this["TIMES"] = value;
            }
        }
        public string EXCEPTINFO
        {
            get
            {
                return (string)this["EXCEPTINFO"];
            }
            set
            {
                this["EXCEPTINFO"] = value;
            }
        }
        public string SYSRESOURCE
        {
            get
            {
                return (string)this["SYSRESOURCE"];
            }
            set
            {
                this["SYSRESOURCE"] = value;
            }
        }
        public string BAKUP
        {
            get
            {
                return (string)this["BAKUP"];
            }
            set
            {
                this["BAKUP"] = value;
            }
        }
        public string CHECKEMP
        {
            get
            {
                return (string)this["CHECKEMP"];
            }
            set
            {
                this["CHECKEMP"] = value;
            }
        }
        public string CHECKTIME
        {
            get
            {
                return (string)this["CHECKTIME"];
            }
            set
            {
                this["CHECKTIME"] = value;
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
        public string ISWSUS
        {
            get
            {
                return (string)this["ISWSUS"];
            }
            set
            {
                this["ISWSUS"] = value;
            }
        }
        public string ISSET
        {
            get
            {
                return (string)this["ISSET"];
            }
            set
            {
                this["ISSET"] = value;
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
    public class MIS_S_C_LOG
    {
        public string ID { get; set; }
        public string SERVERID { get; set; }
        public string SHADU { get; set; }
        public string PWD { get; set; }
        public string TIMES { get; set; }
        public string EXCEPTINFO { get; set; }
        public string SYSRESOURCE { get; set; }
        public string BAKUP { get; set; }
        public string CHECKEMP { get; set; }
        public string CHECKTIME { get; set; }
        public string REMARK { get; set; }
        public string ISWSUS { get; set; }
        public string ISSET { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}