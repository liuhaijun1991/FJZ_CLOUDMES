using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_DN_SO : DataObjectTable
    {
        public T_R_DN_SO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DN_SO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DN_SO);
            TableName = "R_DN_SO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_DN_SO : DataObjectBase
    {
        public Row_R_DN_SO(DataObjectInfo info) : base(info)
        {

        }
        public R_DN_SO GetDataObject()
        {
            R_DN_SO DataObject = new R_DN_SO();
            DataObject.ID = this.ID;
            DataObject.DN_NO = this.DN_NO;
            DataObject.DN_LINE = this.DN_LINE;
            DataObject.DN_SKUNO = this.DN_SKUNO;
            DataObject.DN_QTY = this.DN_QTY;
            DataObject.SO_NO = this.SO_NO;
            DataObject.SO_LINE_SEQ = this.SO_LINE_SEQ;
            DataObject.SO_LINE_QTY = this.SO_LINE_QTY;
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
        public string DN_SKUNO
        {
            get
            {
                return (string)this["DN_SKUNO"];
            }
            set
            {
                this["DN_SKUNO"] = value;
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
        public string SO_LINE_SEQ
        {
            get
            {
                return (string)this["SO_LINE_SEQ"];
            }
            set
            {
                this["SO_LINE_SEQ"] = value;
            }
        }
        public double? SO_LINE_QTY
        {
            get
            {
                return (double?)this["SO_LINE_QTY"];
            }
            set
            {
                this["SO_LINE_QTY"] = value;
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
    public class R_DN_SO
    {
        public string ID { get; set; }
        public string DN_NO { get; set; }
        public string DN_LINE { get; set; }
        public string DN_SKUNO { get; set; }
        public double? DN_QTY { get; set; }
        public string SO_NO { get; set; }
        public string SO_LINE_SEQ { get; set; }
        public double? SO_LINE_QTY { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
    }
}