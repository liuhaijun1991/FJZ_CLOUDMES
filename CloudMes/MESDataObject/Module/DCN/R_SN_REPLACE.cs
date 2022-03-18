using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SN_REPLACE : DataObjectTable
    {
        public T_R_SN_REPLACE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_REPLACE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_REPLACE);
            TableName = "R_SN_REPLACE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SN_REPLACE : DataObjectBase
    {
        public Row_R_SN_REPLACE(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_REPLACE GetDataObject()
        {
            R_SN_REPLACE DataObject = new R_SN_REPLACE();
            DataObject.ID = this.ID;
            DataObject.LINKTYPE = this.LINKTYPE;
            DataObject.NEWSN = this.NEWSN;
            DataObject.OLDSN = this.OLDSN;
            DataObject.BOXSN = this.BOXSN;
            DataObject.STATION = this.STATION;
            DataObject.REMARK = this.REMARK;
            DataObject.FLAG = this.FLAG;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
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
        public string LINKTYPE
        {
            get
            {
                return (string)this["LINKTYPE"];
            }
            set
            {
                this["LINKTYPE"] = value;
            }
        }
        public string NEWSN
        {
            get
            {
                return (string)this["NEWSN"];
            }
            set
            {
                this["NEWSN"] = value;
            }
        }
        public string OLDSN
        {
            get
            {
                return (string)this["OLDSN"];
            }
            set
            {
                this["OLDSN"] = value;
            }
        }
        public string BOXSN
        {
            get
            {
                return (string)this["BOXSN"];
            }
            set
            {
                this["BOXSN"] = value;
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
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
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
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
    }
    public class R_SN_REPLACE
    {
        public string ID { get; set; }
        public string LINKTYPE { get; set; }
        public string NEWSN { get; set; }
        public string OLDSN { get; set; }
        public string BOXSN { get; set; }
        public string STATION { get; set; }
        public string REMARK { get; set; }
        public string FLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}
