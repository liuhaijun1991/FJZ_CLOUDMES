using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SO : DataObjectTable
    {
        public T_R_SO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SO);
            TableName = "R_SO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SO : DataObjectBase
    {
        public Row_R_SO(DataObjectInfo info) : base(info)
        {

        }
        public R_SO GetDataObject()
        {
            R_SO DataObject = new R_SO();
            DataObject.ID = this.ID;
            DataObject.SO_NO = this.SO_NO;
            DataObject.BILL_TO_CODE = this.BILL_TO_CODE;
            DataObject.STATUS = this.STATUS;
            DataObject.ENABLE_DATE_FROM = this.ENABLE_DATE_FROM;
            DataObject.ENABLE_DATE_TO = this.ENABLE_DATE_TO;
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
        public string SO_NO
        {
            get
            {
                return (string)this["SO_NO"];
            }
            set
            {
                this["SO_NO"] = value;
            }
        }
        public string BILL_TO_CODE
        {
            get
            {
                return (string)this["BILL_TO_CODE"];
            }
            set
            {
                this["BILL_TO_CODE"] = value;
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
    public class R_SO
    {
        public string ID { get; set; }
        public string SO_NO { get; set; }
        public string BILL_TO_CODE { get; set; }
        public string STATUS { get; set; }
        public DateTime? ENABLE_DATE_FROM { get; set; }
        public DateTime? ENABLE_DATE_TO { get; set; }
        public string EXT_KEY1 { get; set; }
        public string EXT_VALUE1 { get; set; }
        public string EXT_KEY2 { get; set; }
        public string EXT_VALUE2 { get; set; }
        public string EXT_KEY3 { get; set; }
        public string EXT_VALUE3 { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
    }
    public class R_SO_Status
    {
        public static string OPEN = "OPEN";
        public static string FINISH = "FINISH";
        public static string CANCEL = "CANCEL";
    }

}