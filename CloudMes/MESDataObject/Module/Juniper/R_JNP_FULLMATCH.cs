using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JNP_FULLMATCH : DataObjectTable
    {
        public T_R_JNP_FULLMATCH(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_FULLMATCH(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_FULLMATCH);
            TableName = "R_JNP_FULLMATCH".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_FULLMATCH : DataObjectBase
    {
        public Row_R_JNP_FULLMATCH(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_FULLMATCH GetDataObject()
        {
            R_JNP_FULLMATCH DataObject = new R_JNP_FULLMATCH();
            DataObject.ID = this.ID;
            DataObject.PARENTPN = this.PARENTPN;
            DataObject.BASETYPE = this.BASETYPE;
            DataObject.SLOTTYPE = this.SLOTTYPE;
            DataObject.BLACKPN = this.BLACKPN;
            DataObject.QTY = this.QTY;
            DataObject.VALIDFLAG = this.VALIDFLAG;
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
        public string PARENTPN
        {
            get
            {
                return (string)this["PARENTPN"];
            }
            set
            {
                this["PARENTPN"] = value;
            }
        }
        public string BASETYPE
        {
            get
            {
                return (string)this["BASETYPE"];
            }
            set
            {
                this["BASETYPE"] = value;
            }
        }
        public string SLOTTYPE
        {
            get
            {
                return (string)this["SLOTTYPE"];
            }
            set
            {
                this["SLOTTYPE"] = value;
            }
        }
        public string BLACKPN
        {
            get
            {
                return (string)this["BLACKPN"];
            }
            set
            {
                this["BLACKPN"] = value;
            }
        }
        public string QTY
        {
            get
            {
                return (string)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
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
    public class R_JNP_FULLMATCH
    {
        [SqlSugar.SugarColumn(IsIdentity =true)]
        public string ID { get; set; }
        public string PARENTPN { get; set; }
        public string BASETYPE { get; set; }
        public string SLOTTYPE { get; set; }
        public string BLACKPN { get; set; }
        public string QTY { get; set; }
        public string VALIDFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}