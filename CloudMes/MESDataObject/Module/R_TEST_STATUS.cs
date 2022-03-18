using MESDBHelper;
using System;

namespace MESDataObject.Module
{
    public class T_R_TEST_STATUS : DataObjectTable
    {
        public T_R_TEST_STATUS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_STATUS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_STATUS);
            TableName = "R_TEST_STATUS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_TEST_STATUS : DataObjectBase
    {
        public Row_R_TEST_STATUS(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_STATUS GetDataObject()
        {
            R_TEST_STATUS DataObject = new R_TEST_STATUS();
            DataObject.MACHINE_NO = this.MACHINE_NO;
            DataObject.SLOT_NO = this.SLOT_NO;
            DataObject.STATUS = this.STATUS;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.SEQNO = this.SEQNO;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            return DataObject;
        }
        public string MACHINE_NO
        {
            get
            {
                return (string)this["MACHINE_NO"];
            }
            set
            {
                this["MACHINE_NO"] = value;
            }
        }
        public string SLOT_NO
        {
            get
            {
                return (string)this["SLOT_NO"];
            }
            set
            {
                this["SLOT_NO"] = value;
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
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string SYSSERIALNO
        {
            get
            {
                return (string)this["SYSSERIALNO"];
            }
            set
            {
                this["SYSSERIALNO"] = value;
            }
        }
    }
    public class R_TEST_STATUS
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string MACHINE_NO { get; set; }
        public string SLOT_NO { get; set; }
        public string STATUS { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
        public double? SEQNO { get; set; }
        public string SYSSERIALNO { get; set; }
    }
}