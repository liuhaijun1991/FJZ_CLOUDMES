using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_2DXRAY : DataObjectTable
    {
        public T_R_2DXRAY(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2DXRAY(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2DXRAY);
            TableName = "R_2DXRAY".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_2DXRAY : DataObjectBase
    {
        public Row_R_2DXRAY(DataObjectInfo info) : base(info)
        {

        }
        public R_2DXRAY GetDataObject()
        {
            R_2DXRAY DataObject = new R_2DXRAY();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.BGA_LOCATION = this.BGA_LOCATION;
            DataObject.STATUS = this.STATUS;
            DataObject.MISALIGNMENT = this.MISALIGNMENT;
            DataObject.VOID = this.VOID;
            DataObject.ISSHORT = this.ISSHORT;
            DataObject.OTHER = this.OTHER;
            DataObject.REMARK = this.REMARK;
            DataObject.REMARK1 = this.REMARK1;
            DataObject.REMARK2 = this.REMARK2;
            DataObject.REMARK3 = this.REMARK3;
            DataObject.CUSTOMER = this.CUSTOMER;
            DataObject.LINENAME = this.LINENAME;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string BGA_LOCATION
        {
            get
            {
                return (string)this["BGA_LOCATION"];
            }
            set
            {
                this["BGA_LOCATION"] = value;
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
        public string MISALIGNMENT
        {
            get
            {
                return (string)this["MISALIGNMENT"];
            }
            set
            {
                this["MISALIGNMENT"] = value;
            }
        }
        public string VOID
        {
            get
            {
                return (string)this["VOID"];
            }
            set
            {
                this["VOID"] = value;
            }
        }
        public string ISSHORT
        {
            get
            {
                return (string)this["ISSHORT"];
            }
            set
            {
                this["ISSHORT"] = value;
            }
        }
        public string OTHER
        {
            get
            {
                return (string)this["OTHER"];
            }
            set
            {
                this["OTHER"] = value;
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
        public string REMARK1
        {
            get
            {
                return (string)this["REMARK1"];
            }
            set
            {
                this["REMARK1"] = value;
            }
        }
        public string REMARK2
        {
            get
            {
                return (string)this["REMARK2"];
            }
            set
            {
                this["REMARK2"] = value;
            }
        }
        public string REMARK3
        {
            get
            {
                return (string)this["REMARK3"];
            }
            set
            {
                this["REMARK3"] = value;
            }
        }
        public string CUSTOMER
        {
            get
            {
                return (string)this["CUSTOMER"];
            }
            set
            {
                this["CUSTOMER"] = value;
            }
        }
        public string LINENAME
        {
            get
            {
                return (string)this["LINENAME"];
            }
            set
            {
                this["LINENAME"] = value;
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
    public class R_2DXRAY
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string BGA_LOCATION { get; set; }
        public string STATUS { get; set; }
        public string MISALIGNMENT { get; set; }
        public string VOID { get; set; }
        public string ISSHORT { get; set; }
        public string OTHER { get; set; }
        public string REMARK { get; set; }
        public string REMARK1 { get; set; }
        public string REMARK2 { get; set; }
        public string REMARK3 { get; set; }
        public string CUSTOMER { get; set; }
        public string LINENAME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}