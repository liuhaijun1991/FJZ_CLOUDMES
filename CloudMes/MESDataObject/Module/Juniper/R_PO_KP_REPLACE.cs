using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_PO_KP_REPLACE : DataObjectTable
    {
        public T_R_PO_KP_REPLACE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PO_KP_REPLACE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PO_KP_REPLACE);
            TableName = "R_PO_KP_REPLACE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PO_KP_REPLACE : DataObjectBase
    {
        public Row_R_PO_KP_REPLACE(DataObjectInfo info) : base(info)
        {

        }
        public R_PO_KP_REPLACE GetDataObject()
        {
            R_PO_KP_REPLACE DataObject = new R_PO_KP_REPLACE();
            DataObject.ID = this.ID;
            DataObject.PONO = this.PONO;
            DataObject.POLINE = this.POLINE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.REPLACEKP = this.REPLACEKP;
            DataObject.VALIDFLAG = this.VALIDFLAG;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
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
        public string PONO
        {
            get
            {
                return (string)this["PONO"];
            }
            set
            {
                this["PONO"] = value;
            }
        }
        public string POLINE
        {
            get
            {
                return (string)this["POLINE"];
            }
            set
            {
                this["POLINE"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string REPLACEKP
        {
            get
            {
                return (string)this["REPLACEKP"];
            }
            set
            {
                this["REPLACEKP"] = value;
            }
        }
        public string VALIDFLAG
        {
            get
            {
                return (string)this["VALIDFLAG"];
            }
            set
            {
                this["VALIDFLAG"] = value;
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
    }
    public class R_PO_KP_REPLACE
    {
        public string ID { get; set; }
        public string PONO { get; set; }
        public string POLINE { get; set; }
        public string PARTNO { get; set; }
        public string REPLACEKP { get; set; }
        public string VALIDFLAG { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
    }
}