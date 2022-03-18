using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_SHIP : DataObjectTable
    {
        public T_R_7B5_SHIP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_SHIP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_SHIP);
            TableName = "R_7B5_SHIP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_7B5_SHIP> GetObjByTaskAndItme(OleExec sfcdb, string task_no, string hh_item)
        {
            return sfcdb.ORM.Queryable<R_7B5_SHIP>().Where(r => r.TASK_NO == task_no && r.HH_ITEM == hh_item).ToList();
        }

        public int UpdateTotalPlanQtyByTaskAndItem(OleExec sfcdb, string task_no, string hh_item,double qty)
        {
            return sfcdb.ORM.Updateable<R_7B5_SHIP>()
                .UpdateColumns(r => new R_7B5_SHIP { TOTAL_PLAN_QTY = r.TOTAL_PLAN_QTY + qty })
                .Where(r => r.TASK_NO == task_no && r.HH_ITEM == hh_item).ExecuteCommand();
        }
    }
    public class Row_R_7B5_SHIP : DataObjectBase
    {
        public Row_R_7B5_SHIP(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_SHIP GetDataObject()
        {
            R_7B5_SHIP DataObject = new R_7B5_SHIP();
            DataObject.BUFFER_REMARK = this.BUFFER_REMARK;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.TOTAL_PLAN_QTY = this.TOTAL_PLAN_QTY;
            DataObject.BUFFER_QTY = this.BUFFER_QTY;
            DataObject.SHIPPED_QTY = this.SHIPPED_QTY;
            DataObject.RECEIVE_DATE = this.RECEIVE_DATE;
            DataObject.TASK_QTY = this.TASK_QTY;
            DataObject.HW_ITEM = this.HW_ITEM;
            DataObject.HH_ITEM = this.HH_ITEM;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.MODEL = this.MODEL;
            return DataObject;
        }
        public string BUFFER_REMARK
        {
            get
            {
                return (string)this["BUFFER_REMARK"];
            }
            set
            {
                this["BUFFER_REMARK"] = value;
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
        public double? TOTAL_PLAN_QTY
        {
            get
            {
                return (double?)this["TOTAL_PLAN_QTY"];
            }
            set
            {
                this["TOTAL_PLAN_QTY"] = value;
            }
        }
        public double? BUFFER_QTY
        {
            get
            {
                return (double?)this["BUFFER_QTY"];
            }
            set
            {
                this["BUFFER_QTY"] = value;
            }
        }
        public double? SHIPPED_QTY
        {
            get
            {
                return (double?)this["SHIPPED_QTY"];
            }
            set
            {
                this["SHIPPED_QTY"] = value;
            }
        }
        public DateTime? RECEIVE_DATE
        {
            get
            {
                return (DateTime?)this["RECEIVE_DATE"];
            }
            set
            {
                this["RECEIVE_DATE"] = value;
            }
        }
        public double? TASK_QTY
        {
            get
            {
                return (double?)this["TASK_QTY"];
            }
            set
            {
                this["TASK_QTY"] = value;
            }
        }
        public string HW_ITEM
        {
            get
            {
                return (string)this["HW_ITEM"];
            }
            set
            {
                this["HW_ITEM"] = value;
            }
        }
        public string HH_ITEM
        {
            get
            {
                return (string)this["HH_ITEM"];
            }
            set
            {
                this["HH_ITEM"] = value;
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
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
            }
        }
    }
    public class R_7B5_SHIP
    {
        public string BUFFER_REMARK { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
        public double? TOTAL_PLAN_QTY { get; set; }
        public double? BUFFER_QTY { get; set; }
        public double? SHIPPED_QTY { get; set; }
        public DateTime? RECEIVE_DATE { get; set; }
        public double? TASK_QTY { get; set; }
        public string HW_ITEM { get; set; }
        public string HH_ITEM { get; set; }
        public string TASK_NO { get; set; }
        public string MODEL { get; set; }
    }
}