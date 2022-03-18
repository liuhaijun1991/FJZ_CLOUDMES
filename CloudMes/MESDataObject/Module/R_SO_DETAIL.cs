using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SO_DETAIL : DataObjectTable
    {
        public T_R_SO_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SO_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SO_DETAIL);
            TableName = "R_SO_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SO_DETAIL : DataObjectBase
    {
        public Row_R_SO_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SO_DETAIL GetDataObject()
        {
            R_SO_DETAIL DataObject = new R_SO_DETAIL();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.ID = this.ID;
            DataObject.R_SO_ID = this.R_SO_ID;
            DataObject.SO_NO = this.SO_NO;
            DataObject.LINE_SEQ = this.LINE_SEQ;
            DataObject.SKUNO = this.SKUNO;
            DataObject.QTY = this.QTY;
            DataObject.DN_QTY = this.DN_QTY;
            DataObject.EXT_KEY1 = this.EXT_KEY1;
            DataObject.EXT_VALUE1 = this.EXT_VALUE1;
            DataObject.EXT_KEY2 = this.EXT_KEY2;
            DataObject.EXT_VALUE2 = this.EXT_VALUE2;
            DataObject.EDIT_DATE = this.EDIT_DATE;
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
        public string R_SO_ID
        {
            get
            {
                return (string)this["R_SO_ID"];
            }
            set
            {
                this["R_SO_ID"] = value;
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
        public string LINE_SEQ
        {
            get
            {
                return (string)this["LINE_SEQ"];
            }
            set
            {
                this["LINE_SEQ"] = value;
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
    public class R_SO_DETAIL
    {
        public string EDIT_EMP { get; set; }
        public string ID { get; set; }
        public string R_SO_ID { get; set; }
        public string SO_NO { get; set; }
        public string LINE_SEQ { get; set; }
        public string SKUNO { get; set; }
        public double? QTY { get; set; }
        public double? DN_QTY { get; set; }
        public string EXT_KEY1 { get; set; }
        public string EXT_VALUE1 { get; set; }
        public string EXT_KEY2 { get; set; }
        public string EXT_VALUE2 { get; set; }
        public DateTime? EDIT_DATE { get; set; }
    }
}