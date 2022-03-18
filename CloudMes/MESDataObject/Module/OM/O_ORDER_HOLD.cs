using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.OM
{
    public class T_O_ORDER_HOLD : DataObjectTable
    {
        public T_O_ORDER_HOLD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_ORDER_HOLD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_ORDER_HOLD);
            TableName = "O_ORDER_HOLD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_ORDER_HOLD : DataObjectBase
    {
        public Row_O_ORDER_HOLD(DataObjectInfo info) : base(info)
        {

        }
        public O_ORDER_HOLD GetDataObject()
        {
            O_ORDER_HOLD DataObject = new O_ORDER_HOLD();
            DataObject.ID = this.ID;
            DataObject.ITEMID = this.ITEMID;
            DataObject.UPOID = this.UPOID;
            DataObject.HOLDFLAG = this.HOLDFLAG;
            DataObject.HOLDREASON = this.HOLDREASON;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string ITEMID
        {
            get
            {
                return (string)this["ITEMID"];
            }
            set
            {
                this["ITEMID"] = value;
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
        public string HOLDFLAG
        {
            get
            {
                return (string)this["HOLDFLAG"];
            }
            set
            {
                this["HOLDFLAG"] = value;
            }
        }
        public string HOLDREASON
        {
            get
            {
                return (string)this["HOLDREASON"];
            }
            set
            {
                this["HOLDREASON"] = value;
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
    public class O_ORDER_HOLD
    {
        public string ID { get; set; }
        public string ITEMID { get; set; }
        public string UPOID { get; set; }
        public string HOLDFLAG { get; set; }
        public string HOLDREASON { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }

}