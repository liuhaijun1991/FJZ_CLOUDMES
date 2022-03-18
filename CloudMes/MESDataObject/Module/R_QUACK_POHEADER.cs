using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_POHEADER : DataObjectTable
    {
        public T_R_QUACK_POHEADER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_POHEADER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_POHEADER);
            TableName = "R_QUACK_POHEADER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_QUACK_POHEADER GetPO(string PO, OleExec DB, DB_TYPE_ENUM DBType)
        {
           return DB.ORM.Queryable<R_QUACK_POHEADER>().Where(t => t.PO_NO == PO).ToList().FirstOrDefault();
        }
    }
    public class Row_R_QUACK_POHEADER : DataObjectBase
    {
        public Row_R_QUACK_POHEADER(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_POHEADER GetDataObject()
        {
            R_QUACK_POHEADER DataObject = new R_QUACK_POHEADER();
            DataObject.ID = this.ID;
            DataObject.PO_NO = this.PO_NO;
            DataObject.VENDOR_CODE = this.VENDOR_CODE;
            DataObject.PO_TYPE = this.PO_TYPE;
            DataObject.CREATE_DATE = this.CREATE_DATE;
            DataObject.INCO1 = this.INCO1;
            DataObject.ZTERM = this.ZTERM;
            DataObject.LOEKZ = this.LOEKZ;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.ERDAT = this.ERDAT;
            DataObject.ERZET = this.ERZET;
            DataObject.USER_NAME = this.USER_NAME;
            DataObject.UDATE = this.UDATE;
            DataObject.UTIME = this.UTIME;
            DataObject.SUBMI = this.SUBMI;
            DataObject.UNSEZ = this.UNSEZ;
            DataObject.KUNNR = this.KUNNR;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string VENDOR_CODE
        {
            get
            {
                return (string)this["VENDOR_CODE"];
            }
            set
            {
                this["VENDOR_CODE"] = value;
            }
        }
        public string PO_TYPE
        {
            get
            {
                return (string)this["PO_TYPE"];
            }
            set
            {
                this["PO_TYPE"] = value;
            }
        }
        public DateTime? CREATE_DATE
        {
            get
            {
                return (DateTime?)this["CREATE_DATE"];
            }
            set
            {
                this["CREATE_DATE"] = value;
            }
        }
        public string INCO1
        {
            get
            {
                return (string)this["INCO1"];
            }
            set
            {
                this["INCO1"] = value;
            }
        }
        public string ZTERM
        {
            get
            {
                return (string)this["ZTERM"];
            }
            set
            {
                this["ZTERM"] = value;
            }
        }
        public string LOEKZ
        {
            get
            {
                return (string)this["LOEKZ"];
            }
            set
            {
                this["LOEKZ"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? ERDAT
        {
            get
            {
                return (DateTime?)this["ERDAT"];
            }
            set
            {
                this["ERDAT"] = value;
            }
        }
        public DateTime? ERZET
        {
            get
            {
                return (DateTime?)this["ERZET"];
            }
            set
            {
                this["ERZET"] = value;
            }
        }
        public string USER_NAME
        {
            get
            {
                return (string)this["USER_NAME"];
            }
            set
            {
                this["USER_NAME"] = value;
            }
        }
        public DateTime? UDATE
        {
            get
            {
                return (DateTime?)this["UDATE"];
            }
            set
            {
                this["UDATE"] = value;
            }
        }
        public DateTime? UTIME
        {
            get
            {
                return (DateTime?)this["UTIME"];
            }
            set
            {
                this["UTIME"] = value;
            }
        }
        public string SUBMI
        {
            get
            {
                return (string)this["SUBMI"];
            }
            set
            {
                this["SUBMI"] = value;
            }
        }
        public string UNSEZ
        {
            get
            {
                return (string)this["UNSEZ"];
            }
            set
            {
                this["UNSEZ"] = value;
            }
        }
        public string KUNNR
        {
            get
            {
                return (string)this["KUNNR"];
            }
            set
            {
                this["KUNNR"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_QUACK_POHEADER
    {
        public string ID { get; set; }
        public string PO_NO { get; set; }
        public string VENDOR_CODE { get; set; }
        public string PO_TYPE { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string INCO1 { get; set; }
        public string ZTERM { get; set; }
        public string LOEKZ { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? ERDAT { get; set; }
        public DateTime? ERZET { get; set; }
        public string USER_NAME { get; set; }
        public DateTime? UDATE { get; set; }
        public DateTime? UTIME { get; set; }
        public string SUBMI { get; set; }
        public string UNSEZ { get; set; }
        public string KUNNR { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}