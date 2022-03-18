using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_5DX_FAIL_DATA : DataObjectTable
    {
        public T_R_5DX_FAIL_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_5DX_FAIL_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_5DX_FAIL_DATA);
            TableName = "R_5DX_FAIL_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_5DX_FAIL_DATA : DataObjectBase
    {
        public Row_R_5DX_FAIL_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_5DX_FAIL_DATA GetDataObject()
        {
            R_5DX_FAIL_DATA DataObject = new R_5DX_FAIL_DATA();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.POST_DATE = this.POST_DATE;
            DataObject.TOTAL_QTY = this.TOTAL_QTY;
            DataObject.TOTAL_PASS_QTY = this.TOTAL_PASS_QTY;
            DataObject.TOTAL_FAIL_QTY = this.TOTAL_FAIL_QTY;
            DataObject.TOTAL_JOIN_QTY = this.TOTAL_JOIN_QTY;
            DataObject.TOTAL_CALL_QTY = this.TOTAL_CALL_QTY;
            DataObject.TOTAL_REPAIR_QTY = this.TOTAL_REPAIR_QTY;
            DataObject.TOTAL_FALSE_QTY = this.TOTAL_FALSE_QTY;
            DataObject.FPY = this.FPY;
            DataObject.FALSE_CALL_QTY = this.FALSE_CALL_QTY;
            DataObject.REPAIR_PIN_QTY = this.REPAIR_PIN_QTY;
            DataObject.SYM_OPEN = this.SYM_OPEN;
            DataObject.SYM_TOMBS = this.SYM_TOMBS;
            DataObject.SYM_MISSING = this.SYM_MISSING;
            DataObject.SYM_SHIFT = this.SYM_SHIFT;
            DataObject.SYM_SHORT = this.SYM_SHORT;
            DataObject.SYM_LIFTED = this.SYM_LIFTED;
            DataObject.SYM_VOID = this.SYM_VOID;
            DataObject.SYM_INSUFF = this.SYM_INSUFF;
            DataObject.SYM_OTHERS = this.SYM_OTHERS;
            DataObject.SYM_TOTAL = this.SYM_TOTAL;
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
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public DateTime? POST_DATE
        {
            get
            {
                return (DateTime?)this["POST_DATE"];
            }
            set
            {
                this["POST_DATE"] = value;
            }
        }
        public double? TOTAL_QTY
        {
            get
            {
                return (double?)this["TOTAL_QTY"];
            }
            set
            {
                this["TOTAL_QTY"] = value;
            }
        }
        public double? TOTAL_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_PASS_QTY"];
            }
            set
            {
                this["TOTAL_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_FAIL_QTY"] = value;
            }
        }
        public double? TOTAL_JOIN_QTY
        {
            get
            {
                return (double?)this["TOTAL_JOIN_QTY"];
            }
            set
            {
                this["TOTAL_JOIN_QTY"] = value;
            }
        }
        public double? TOTAL_CALL_QTY
        {
            get
            {
                return (double?)this["TOTAL_CALL_QTY"];
            }
            set
            {
                this["TOTAL_CALL_QTY"] = value;
            }
        }
        public double? TOTAL_REPAIR_QTY
        {
            get
            {
                return (double?)this["TOTAL_REPAIR_QTY"];
            }
            set
            {
                this["TOTAL_REPAIR_QTY"] = value;
            }
        }
        public double? TOTAL_FALSE_QTY
        {
            get
            {
                return (double?)this["TOTAL_FALSE_QTY"];
            }
            set
            {
                this["TOTAL_FALSE_QTY"] = value;
            }
        }
        public double? FPY
        {
            get
            {
                return (double?)this["FPY"];
            }
            set
            {
                this["FPY"] = value;
            }
        }
        public double? FALSE_CALL_QTY
        {
            get
            {
                return (double?)this["FALSE_CALL_QTY"];
            }
            set
            {
                this["FALSE_CALL_QTY"] = value;
            }
        }
        public double? REPAIR_PIN_QTY
        {
            get
            {
                return (double?)this["REPAIR_PIN_QTY"];
            }
            set
            {
                this["REPAIR_PIN_QTY"] = value;
            }
        }
        public double? SYM_OPEN
        {
            get
            {
                return (double?)this["SYM_OPEN"];
            }
            set
            {
                this["SYM_OPEN"] = value;
            }
        }
        public double? SYM_TOMBS
        {
            get
            {
                return (double?)this["SYM_TOMBS"];
            }
            set
            {
                this["SYM_TOMBS"] = value;
            }
        }
        public double? SYM_MISSING
        {
            get
            {
                return (double?)this["SYM_MISSING"];
            }
            set
            {
                this["SYM_MISSING"] = value;
            }
        }
        public double? SYM_SHIFT
        {
            get
            {
                return (double?)this["SYM_SHIFT"];
            }
            set
            {
                this["SYM_SHIFT"] = value;
            }
        }
        public double? SYM_SHORT
        {
            get
            {
                return (double?)this["SYM_SHORT"];
            }
            set
            {
                this["SYM_SHORT"] = value;
            }
        }
        public double? SYM_LIFTED
        {
            get
            {
                return (double?)this["SYM_LIFTED"];
            }
            set
            {
                this["SYM_LIFTED"] = value;
            }
        }
        public double? SYM_VOID
        {
            get
            {
                return (double?)this["SYM_VOID"];
            }
            set
            {
                this["SYM_VOID"] = value;
            }
        }
        public double? SYM_INSUFF
        {
            get
            {
                return (double?)this["SYM_INSUFF"];
            }
            set
            {
                this["SYM_INSUFF"] = value;
            }
        }
        public double? SYM_OTHERS
        {
            get
            {
                return (double?)this["SYM_OTHERS"];
            }
            set
            {
                this["SYM_OTHERS"] = value;
            }
        }
        public double? SYM_TOTAL
        {
            get
            {
                return (double?)this["SYM_TOTAL"];
            }
            set
            {
                this["SYM_TOTAL"] = value;
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
    public class R_5DX_FAIL_DATA
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string STATION_NAME { get; set; }
        public DateTime? POST_DATE { get; set; }
        public double? TOTAL_QTY { get; set; }
        public double? TOTAL_PASS_QTY { get; set; }
        public double? TOTAL_FAIL_QTY { get; set; }
        public double? TOTAL_JOIN_QTY { get; set; }
        public double? TOTAL_CALL_QTY { get; set; }
        public double? TOTAL_REPAIR_QTY { get; set; }
        public double? TOTAL_FALSE_QTY { get; set; }
        public double? FPY { get; set; }
        public double? FALSE_CALL_QTY { get; set; }
        public double? REPAIR_PIN_QTY { get; set; }
        public double? SYM_OPEN { get; set; }
        public double? SYM_TOMBS { get; set; }
        public double? SYM_MISSING { get; set; }
        public double? SYM_SHIFT { get; set; }
        public double? SYM_SHORT { get; set; }
        public double? SYM_LIFTED { get; set; }
        public double? SYM_VOID { get; set; }
        public double? SYM_INSUFF { get; set; }
        public double? SYM_OTHERS { get; set; }
        public double? SYM_TOTAL { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}