using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_WO : DataObjectTable
    {
        public T_R_7B5_WO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_WO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_WO);
            TableName = "R_7B5_WO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_7B5_WO : DataObjectBase
    {
        public Row_R_7B5_WO(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_WO GetDataObject()
        {
            R_7B5_WO DataObject = new R_7B5_WO();
            DataObject.RELEASEDDATE = this.RELEASEDDATE;
            DataObject.FINISHQTY = this.FINISHQTY;
            DataObject.CREATE_BY = this.CREATE_BY;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.WO_STATUS = this.WO_STATUS;
            DataObject.DELETE_FLAG = this.DELETE_FLAG;
            DataObject.LOAD_QTY = this.LOAD_QTY;
            DataObject.SAP_MESSAGE = this.SAP_MESSAGE;
            DataObject.SAP_WO = this.SAP_WO;
            DataObject.MAIN_WO_FLAG = this.MAIN_WO_FLAG;
            DataObject.WO_QTY = this.WO_QTY;
            DataObject.TASK_QTY = this.TASK_QTY;
            DataObject.HW_ITEM = this.HW_ITEM;
            DataObject.HH_ITEM = this.HH_ITEM;
            DataObject.V_TASK_NO = this.V_TASK_NO;
            DataObject.TASK_NO = this.TASK_NO;
            return DataObject;
        }
        public DateTime? RELEASEDDATE
        {
            get
            {
                return (DateTime?)this["RELEASEDDATE"];
            }
            set
            {
                this["RELEASEDDATE"] = value;
            }
        }
        public double? FINISHQTY
        {
            get
            {
                return (double?)this["FINISHQTY"];
            }
            set
            {
                this["FINISHQTY"] = value;
            }
        }
        public string CREATE_BY
        {
            get
            {
                return (string)this["CREATE_BY"];
            }
            set
            {
                this["CREATE_BY"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public string WO_STATUS
        {
            get
            {
                return (string)this["WO_STATUS"];
            }
            set
            {
                this["WO_STATUS"] = value;
            }
        }
        public string DELETE_FLAG
        {
            get
            {
                return (string)this["DELETE_FLAG"];
            }
            set
            {
                this["DELETE_FLAG"] = value;
            }
        }
        public double? LOAD_QTY
        {
            get
            {
                return (double?)this["LOAD_QTY"];
            }
            set
            {
                this["LOAD_QTY"] = value;
            }
        }
        public string SAP_MESSAGE
        {
            get
            {
                return (string)this["SAP_MESSAGE"];
            }
            set
            {
                this["SAP_MESSAGE"] = value;
            }
        }
        public string SAP_WO
        {
            get
            {
                return (string)this["SAP_WO"];
            }
            set
            {
                this["SAP_WO"] = value;
            }
        }
        public string MAIN_WO_FLAG
        {
            get
            {
                return (string)this["MAIN_WO_FLAG"];
            }
            set
            {
                this["MAIN_WO_FLAG"] = value;
            }
        }
        public double? WO_QTY
        {
            get
            {
                return (double?)this["WO_QTY"];
            }
            set
            {
                this["WO_QTY"] = value;
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
        public string V_TASK_NO
        {
            get
            {
                return (string)this["V_TASK_NO"];
            }
            set
            {
                this["V_TASK_NO"] = value;
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
    public class R_7B5_WO
    {
        public DateTime? RELEASEDDATE { get; set; }
        public double? FINISHQTY { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string WO_STATUS { get; set; }
        public string DELETE_FLAG { get; set; }
        public double? LOAD_QTY { get; set; }
        public string SAP_MESSAGE { get; set; }
        public string SAP_WO { get; set; }
        public string MAIN_WO_FLAG { get; set; }
        public double? WO_QTY { get; set; }
        public double? TASK_QTY { get; set; }
        public string HW_ITEM { get; set; }
        public string HH_ITEM { get; set; }
        public string V_TASK_NO { get; set; }
        public string TASK_NO { get; set; }
    }
}