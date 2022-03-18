using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SN_LINK : DataObjectTable
    {
        public T_R_SN_LINK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_LINK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_LINK);
            TableName = "R_SN_LINK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SN_LINK : DataObjectBase
    {
        public Row_R_SN_LINK(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_LINK GetDataObject()
        {
            R_SN_LINK DataObject = new R_SN_LINK();
            DataObject.ID = this.ID;
            DataObject.LINKTYPE = this.LINKTYPE;
            DataObject.MODEL = this.MODEL;
            DataObject.SN = this.SN;
            DataObject.CSN = this.CSN;
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
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
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
        public string CSN
        {
            get
            {
                return (string)this["CSN"];
            }
            set
            {
                this["CSN"] = value;
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
    public class R_SN_LINK
    {
        public string ID { get; set; }
        public string LINKTYPE { get; set; }
        public string MODEL { get; set; }
        public string SN { get; set; }
        public string CSN { get; set; }
        public string VALIDFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}