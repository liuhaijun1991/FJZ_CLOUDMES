using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_r_facility : DataObjectTable
    {
        public T_r_facility(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_r_facility(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_r_facility);
            TableName = "r_facility".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_r_facility : DataObjectBase
    {
        public Row_r_facility(DataObjectInfo info) : base(info)
        {

        }
        public r_facility GetDataObject()
        {
            r_facility DataObject = new r_facility();
            DataObject.ID = this.ID;
            DataObject.FACILITY_NAME = this.FACILITY_NAME;
            DataObject.FACILITY_NO = this.FACILITY_NO;
            DataObject.SN = this.SN;
            DataObject.STATUS = this.STATUS;
            DataObject.CUSTOMER_NAME = this.CUSTOMER_NAME;
            DataObject.REGIST_DATE = this.REGIST_DATE;
            DataObject.MT_PERIOD = this.MT_PERIOD;
            DataObject.MT_EMP = this.MT_EMP;
            DataObject.MT_COUNT = this.MT_COUNT;
            DataObject.NEXT_MT_DATE = this.NEXT_MT_DATE;
            DataObject.MT_DATE = this.MT_DATE;
            DataObject.EXKEY1 = this.EXKEY1;
            DataObject.EXVALUE1 = this.EXVALUE1;
            DataObject.EXKEY2 = this.EXKEY2;
            DataObject.EXVALUE2 = this.EXVALUE2;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_DATE = this.EDIT_DATE;
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
        public string FACILITY_NAME
        {
            get
            {
                return (string)this["FACILITY_NAME"];
            }
            set
            {
                this["FACILITY_NAME"] = value;
            }
        }
        public string FACILITY_NO
        {
            get
            {
                return (string)this["FACILITY_NO"];
            }
            set
            {
                this["FACILITY_NO"] = value;
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
        public string CUSTOMER_NAME
        {
            get
            {
                return (string)this["CUSTOMER_NAME"];
            }
            set
            {
                this["CUSTOMER_NAME"] = value;
            }
        }
        public DateTime? REGIST_DATE
        {
            get
            {
                return (DateTime?)this["REGIST_DATE"];
            }
            set
            {
                this["REGIST_DATE"] = value;
            }
        }
        public string MT_PERIOD
        {
            get
            {
                return (string)this["MT_PERIOD"];
            }
            set
            {
                this["MT_PERIOD"] = value;
            }
        }
        public string MT_EMP
        {
            get
            {
                return (string)this["MT_EMP"];
            }
            set
            {
                this["MT_EMP"] = value;
            }
        }
        public string MT_COUNT
        {
            get
            {
                return (string)this["MT_COUNT"];
            }
            set
            {
                this["MT_COUNT"] = value;
            }
        }
        public DateTime? NEXT_MT_DATE
        {
            get
            {
                return (DateTime?)this["NEXT_MT_DATE"];
            }
            set
            {
                this["NEXT_MT_DATE"] = value;
            }
        }
        public DateTime? MT_DATE
        {
            get
            {
                return (DateTime?)this["MT_DATE"];
            }
            set
            {
                this["MT_DATE"] = value;
            }
        }
        public string EXKEY1
        {
            get
            {
                return (string)this["EXKEY1"];
            }
            set
            {
                this["EXKEY1"] = value;
            }
        }
        public string EXVALUE1
        {
            get
            {
                return (string)this["EXVALUE1"];
            }
            set
            {
                this["EXVALUE1"] = value;
            }
        }
        public string EXKEY2
        {
            get
            {
                return (string)this["EXKEY2"];
            }
            set
            {
                this["EXKEY2"] = value;
            }
        }
        public string EXVALUE2
        {
            get
            {
                return (string)this["EXVALUE2"];
            }
            set
            {
                this["EXVALUE2"] = value;
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
    }
    public class r_facility
    {
        public string ID { get; set; }
        public string FACILITY_NAME { get; set; }
        public string FACILITY_NO { get; set; }
        public string SN { get; set; }
        public string STATUS { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public DateTime? REGIST_DATE { get; set; }
        public string MT_PERIOD { get; set; }
        public string MT_EMP { get; set; }
        public string MT_COUNT { get; set; }
        public DateTime? NEXT_MT_DATE { get; set; }
        public DateTime? MT_DATE { get; set; }
        public string EXKEY1 { get; set; }
        public string EXVALUE1 { get; set; }
        public string EXKEY2 { get; set; }
        public string EXVALUE2 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_DATE { get; set; }
    }
}
