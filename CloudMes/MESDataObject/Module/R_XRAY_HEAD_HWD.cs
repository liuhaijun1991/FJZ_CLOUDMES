using MESDBHelper;
using System;

namespace MESDataObject.Module
{
    public class T_R_XRAY_HEAD_HWD : DataObjectTable
    {
        public T_R_XRAY_HEAD_HWD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
        }

        public T_R_XRAY_HEAD_HWD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_XRAY_HEAD_HWD);
            TableName = "R_XRAY_HEAD_HWD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }

    public class Row_R_XRAY_HEAD_HWD : DataObjectBase
    {
        public Row_R_XRAY_HEAD_HWD(DataObjectInfo info) : base(info)
        {
        }

        public R_XRAY_HEAD_HWD GetDataObject()
        {
            R_XRAY_HEAD_HWD DataObject = new R_XRAY_HEAD_HWD();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.REMARK = this.REMARK;
            DataObject.RESULT = this.RESULT;
            DataObject.ID = this.ID;
            return DataObject;
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

        public string RESULT
        {
            get
            {
                return (string)this["RESULT"];
            }
            set
            {
                this["RESULT"] = value;
            }
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
    }

    public class R_XRAY_HEAD_HWD
    {
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string REMARK { get; set; }
        public string RESULT { get; set; }
        public string ID { get; set; }
    }
}