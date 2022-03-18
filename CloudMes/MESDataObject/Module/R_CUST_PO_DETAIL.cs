using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CUST_PO_DETAIL : DataObjectTable
    {
        public T_R_CUST_PO_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CUST_PO_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CUST_PO_DETAIL);
            TableName = "R_CUST_PO_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_CUST_PO_DETAIL : DataObjectBase
    {
        public Row_R_CUST_PO_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_CUST_PO_DETAIL GetDataObject()
        {
            R_CUST_PO_DETAIL DataObject = new R_CUST_PO_DETAIL();
            DataObject.ID = this.ID;
            DataObject.R_CUST_PO_ID = this.R_CUST_PO_ID;
            DataObject.CUST_PO_NO = this.CUST_PO_NO;
            DataObject.LINE_NO = this.LINE_NO;
            DataObject.CUST_SKUNO = this.CUST_SKUNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.QTY = this.QTY;
            DataObject.DN_QTY = this.DN_QTY;
            DataObject.SHIPED_QTY = this.SHIPED_QTY;
            DataObject.ENABLE_DATE_FROM = this.ENABLE_DATE_FROM;
            DataObject.ENABLE_DATE_TO = this.ENABLE_DATE_TO;
            DataObject.NEED_BY_DATE = this.NEED_BY_DATE;
            DataObject.STATUS = this.STATUS;
            DataObject.EXT_KEY1 = this.EXT_KEY1;
            DataObject.EXT_VALUE1 = this.EXT_VALUE1;
            DataObject.EXT_KEY2 = this.EXT_KEY2;
            DataObject.EXT_VALUE2 = this.EXT_VALUE2;
            DataObject.EXT_KEY3 = this.EXT_KEY3;
            DataObject.EXT_VALUE3 = this.EXT_VALUE3;
            DataObject.EDIT_DATE = this.EDIT_DATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string R_CUST_PO_ID
        {
            get
            {
                return (string)this["R_CUST_PO_ID"];
            }
            set
            {
                this["R_CUST_PO_ID"] = value;
            }
        }
        public string CUST_PO_NO
        {
            get
            {
                return (string)this["CUST_PO_NO"];
            }
            set
            {
                this["CUST_PO_NO"] = value;
            }
        }
        public string LINE_NO
        {
            get
            {
                return (string)this["LINE_NO"];
            }
            set
            {
                this["LINE_NO"] = value;
            }
        }
        public string CUST_SKUNO
        {
            get
            {
                return (string)this["CUST_SKUNO"];
            }
            set
            {
                this["CUST_SKUNO"] = value;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public double? DN_QTY
        {
            get
            {
                return (double?)this["DN_QTY"];
            }
            set
            {
                this["DN_QTY"] = value;
            }
        }
        public double? SHIPED_QTY
        {
            get
            {
                return (double?)this["SHIPED_QTY"];
            }
            set
            {
                this["SHIPED_QTY"] = value;
            }
        }
        public DateTime? ENABLE_DATE_FROM
        {
            get
            {
                return (DateTime?)this["ENABLE_DATE_FROM"];
            }
            set
            {
                this["ENABLE_DATE_FROM"] = value;
            }
        }
        public DateTime? ENABLE_DATE_TO
        {
            get
            {
                return (DateTime?)this["ENABLE_DATE_TO"];
            }
            set
            {
                this["ENABLE_DATE_TO"] = value;
            }
        }
        public DateTime? NEED_BY_DATE
        {
            get
            {
                return (DateTime?)this["NEED_BY_DATE"];
            }
            set
            {
                this["NEED_BY_DATE"] = value;
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
        public string EXT_KEY1
        {
            get
            {
                return (string)this["EXT_KEY1"];
            }
            set
            {
                this["EXT_KEY1"] = value;
            }
        }
        public string EXT_VALUE1
        {
            get
            {
                return (string)this["EXT_VALUE1"];
            }
            set
            {
                this["EXT_VALUE1"] = value;
            }
        }
        public string EXT_KEY2
        {
            get
            {
                return (string)this["EXT_KEY2"];
            }
            set
            {
                this["EXT_KEY2"] = value;
            }
        }
        public string EXT_VALUE2
        {
            get
            {
                return (string)this["EXT_VALUE2"];
            }
            set
            {
                this["EXT_VALUE2"] = value;
            }
        }
        public string EXT_KEY3
        {
            get
            {
                return (string)this["EXT_KEY3"];
            }
            set
            {
                this["EXT_KEY3"] = value;
            }
        }
        public string EXT_VALUE3
        {
            get
            {
                return (string)this["EXT_VALUE3"];
            }
            set
            {
                this["EXT_VALUE3"] = value;
            }
        }
        public DateTime? EDIT_DATE
        {
            get
            {
                return (DateTime?)this["EDIT_DATE"];
            }
            set
            {
                this["EDIT_DATE"] = value;
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
    }
    public class R_CUST_PO_DETAIL
    {
        public string ID { get; set; }
        public string R_CUST_PO_ID { get; set; }
        public string CUST_PO_NO { get; set; }
        public string LINE_NO { get; set; }
        public string CUST_SKUNO { get; set; }
        public string SKUNO { get; set; }
        public double? QTY { get; set; }
        public double? DN_QTY { get; set; }
        public double? SHIPED_QTY { get; set; }
        public DateTime? ENABLE_DATE_FROM { get; set; }
        public DateTime? ENABLE_DATE_TO { get; set; }
        public DateTime? NEED_BY_DATE { get; set; }
        public string STATUS { get; set; }
        public string EXT_KEY1 { get; set; }
        public string EXT_VALUE1 { get; set; }
        public string EXT_KEY2 { get; set; }
        public string EXT_VALUE2 { get; set; }
        public string EXT_KEY3 { get; set; }
        public string EXT_VALUE3 { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
        public string PO_FILE_TYPE { get; set; }
        public string PO_FILE_DESC { get; set; }
        public string PO_FILE_ID { get; set; }
    }
}