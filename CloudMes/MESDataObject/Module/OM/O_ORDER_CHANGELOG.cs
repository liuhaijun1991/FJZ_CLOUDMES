using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_ORDER_CHANGELOG : DataObjectTable
    {
        public T_O_ORDER_CHANGELOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_ORDER_CHANGELOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_ORDER_CHANGELOG);
            TableName = "O_ORDER_CHANGELOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_ORDER_CHANGELOG : DataObjectBase
    {
        public Row_O_ORDER_CHANGELOG(DataObjectInfo info) : base(info)
        {

        }
        public O_ORDER_CHANGELOG GetDataObject()
        {
            O_ORDER_CHANGELOG DataObject = new O_ORDER_CHANGELOG();
            DataObject.ID = this.ID;
            DataObject.UPOID = this.UPOID;
            DataObject.MAINID = this.MAINID;
            DataObject.CHANGEITEMID = this.CHANGEITEMID;
            DataObject.SOURCEITEMID = this.SOURCEITEMID;
            DataObject.CHANGETYPE = this.CHANGETYPE;
            DataObject.CURRENTREVSION = this.CURRENTREVSION;
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
        public string MAINID
        {
            get
            {
                return (string)this["MAINID"];
            }
            set
            {
                this["MAINID"] = value;
            }
        }
        public string CHANGEITEMID
        {
            get
            {
                return (string)this["CHANGEITEMID"];
            }
            set
            {
                this["CHANGEITEMID"] = value;
            }
        }
        public string SOURCEITEMID
        {
            get
            {
                return (string)this["SOURCEITEMID"];
            }
            set
            {
                this["SOURCEITEMID"] = value;
            }
        }
        public string CHANGETYPE
        {
            get
            {
                return (string)this["CHANGETYPE"];
            }
            set
            {
                this["CHANGETYPE"] = value;
            }
        }
        public string CURRENTREVSION
        {
            get
            {
                return (string)this["CURRENTREVSION"];
            }
            set
            {
                this["CURRENTREVSION"] = value;
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
    public class O_ORDER_CHANGELOG
    {
        public string ID { get; set; }
        public string UPOID { get; set; }
        public string MAINID { get; set; }
        public string CHANGEITEMID { get; set; }
        public string SOURCEITEMID { get; set; }
        public string CHANGETYPE { get; set; }
        public string CURRENTREVSION { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string VERSIONLOG { get; set; }        
    }
}