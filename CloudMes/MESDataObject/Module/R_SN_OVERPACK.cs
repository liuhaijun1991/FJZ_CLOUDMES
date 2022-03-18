using MESDBHelper;
using System;

namespace MESDataObject.Module
{
    public class T_R_SN_OVERPACK : DataObjectTable
    {
        public T_R_SN_OVERPACK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_OVERPACK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_OVERPACK);
            TableName = "R_SN_OVERPACK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SN_OVERPACK : DataObjectBase
    {
        public Row_R_SN_OVERPACK(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_OVERPACK GetDataObject()
        {
            R_SN_OVERPACK DataObject = new R_SN_OVERPACK();
            DataObject.ID = this.ID;
            DataObject.DN_NO = this.DN_NO;
            DataObject.DN_LINE = this.DN_LINE;
            DataObject.PACK_NO = this.PACK_NO;
            DataObject.SN = this.SN;
            DataObject.SN_ID = this.SN_ID;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EXT_KEY1 = this.EXT_KEY1;
            DataObject.EXT_VALUE1 = this.EXT_VALUE1;
            DataObject.EXT_KEY2 = this.EXT_KEY2;
            DataObject.EXT_VALUE2 = this.EXT_VALUE2;
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
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string DN_LINE
        {
            get
            {
                return (string)this["DN_LINE"];
            }
            set
            {
                this["DN_LINE"] = value;
            }
        }
        public double? PACK_NO
        {
            get
            {
                return (double?)this["PACK_NO"];
            }
            set
            {
                this["PACK_NO"] = value;
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
        public string SN_ID
        {
            get
            {
                return (string)this["SN_ID"];
            }
            set
            {
                this["SN_ID"] = value;
            }
        }
        public double? VALID_FLAG
        {
            get
            {
                return (double?)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
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
    }
    public class R_SN_OVERPACK
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string DN_NO { get; set; }
        public string DN_LINE { get; set; }
        public double? PACK_NO { get; set; }
        public string SN { get; set; }
        public string SN_ID { get; set; }
        public double? VALID_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EXT_KEY1 { get; set; }
        public string EXT_VALUE1 { get; set; }
        public string EXT_KEY2 { get; set; }
        public string EXT_VALUE2 { get; set; }
    }
}