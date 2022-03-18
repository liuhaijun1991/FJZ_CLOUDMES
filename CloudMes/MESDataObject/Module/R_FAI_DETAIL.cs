using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_FAI_DETAIL : DataObjectTable
    {
        public T_R_FAI_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FAI_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FAI_DETAIL);
            TableName = "R_FAI_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_FAI_DETAIL : DataObjectBase
    {
        public Row_R_FAI_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_FAI_DETAIL GetDataObject()
        {
            R_FAI_DETAIL DataObject = new R_FAI_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SEQNO = this.SEQNO;
            DataObject.FAISTATIONID = this.FAISTATIONID;
            DataObject.ITEM = this.ITEM;
            DataObject.STATUS = this.STATUS;
            DataObject.FILENAME = this.FILENAME;
            DataObject.FAITIME = this.FAITIME;
            DataObject.FAIBY = this.FAIBY;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string SEQNO
        {
            get
            {
                return (string)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string FAISTATIONID
        {
            get
            {
                return (string)this["FAISTATIONID"];
            }
            set
            {
                this["FAISTATIONID"] = value;
            }
        }
        public string ITEM
        {
            get
            {
                return (string)this["ITEM"];
            }
            set
            {
                this["ITEM"] = value;
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
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public DateTime? FAITIME
        {
            get
            {
                return (DateTime?)this["FAITIME"];
            }
            set
            {
                this["FAITIME"] = value;
            }
        }
        public string FAIBY
        {
            get
            {
                return (string)this["FAIBY"];
            }
            set
            {
                this["FAIBY"] = value;
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
    public class R_FAI_DETAIL
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SEQNO { get; set; }
        public string FAISTATIONID { get; set; }
        public string ITEM { get; set; }
        public string STATUS { get; set; }
        public string FILENAME { get; set; }
        public DateTime? FAITIME { get; set; }
        public string FAIBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}