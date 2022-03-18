using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_PLAN_TASK : DataObjectTable
    {
        public T_R_7B5_PLAN_TASK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_PLAN_TASK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_PLAN_TASK);
            TableName = "R_7B5_PLAN_TASK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_7B5_PLAN_TASK : DataObjectBase
    {
        public Row_R_7B5_PLAN_TASK(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_PLAN_TASK GetDataObject()
        {
            R_7B5_PLAN_TASK DataObject = new R_7B5_PLAN_TASK();
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.CANCEL_FLAG = this.CANCEL_FLAG;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.PLAN_DT = this.PLAN_DT;
            DataObject.PLAN_QTY = this.PLAN_QTY;
            DataObject.ITEM = this.ITEM;
            DataObject.TASK_NO = this.TASK_NO;
            return DataObject;
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
        public string CANCEL_FLAG
        {
            get
            {
                return (string)this["CANCEL_FLAG"];
            }
            set
            {
                this["CANCEL_FLAG"] = value;
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
        public DateTime? PLAN_DT
        {
            get
            {
                return (DateTime?)this["PLAN_DT"];
            }
            set
            {
                this["PLAN_DT"] = value;
            }
        }
        public double? PLAN_QTY
        {
            get
            {
                return (double?)this["PLAN_QTY"];
            }
            set
            {
                this["PLAN_QTY"] = value;
            }
        }
        public string ITEM
        {
            get
            {
                return (string)this["ITEM"];
            }
            set
            {
                this["ITEM"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
            }
        }
    }
    public class R_7B5_PLAN_TASK
    {
        public DateTime? LASTEDITDT { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? PLAN_DT { get; set; }
        public double? PLAN_QTY { get; set; }
        public string ITEM { get; set; }
        public string TASK_NO { get; set; }
    }
}