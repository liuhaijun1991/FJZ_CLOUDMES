using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_WO_TEMP : DataObjectTable
    {
        public T_R_7B5_WO_TEMP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_WO_TEMP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_WO_TEMP);
            TableName = "R_7B5_WO_TEMP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable GetRecently200(OleExec sfcdb)
        {
            string sql = $@" SELECT v_task_no, hh_item, qty  FROM r_7b5_wo_temp WHERE create_time > SYSDATE - 200 ";
            return sfcdb.RunSelect(sql).Tables[0];
        }

        public DataTable GetRecentlyByTask(OleExec sfcdb,string v_task_no)
        {
            string sql = $@" SELECT v_task_no, hh_item, qty  FROM r_7b5_wo_temp WHERE v_task_no='{v_task_no}' ";
            return sfcdb.RunSelect(sql).Tables[0];
        }
    }
    public class Row_R_7B5_WO_TEMP : DataObjectBase
    {
        public Row_R_7B5_WO_TEMP(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_WO_TEMP GetDataObject()
        {
            R_7B5_WO_TEMP DataObject = new R_7B5_WO_TEMP();
            DataObject.TASK_NO_LEVEL = this.TASK_NO_LEVEL;
            DataObject.SUGGEST_QTY = this.SUGGEST_QTY;
            DataObject.CREAT_WO_QTY = this.CREAT_WO_QTY;
            DataObject.MODEL = this.MODEL;
            DataObject.ZNP195_MESSAGE = this.ZNP195_MESSAGE;
            DataObject.WO_TYPE = this.WO_TYPE;
            DataObject.ZNP195_FLAG = this.ZNP195_FLAG;
            DataObject.ITEM_VERSION = this.ITEM_VERSION;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.WO_REMARK = this.WO_REMARK;
            DataObject.MAIN_WO_FLAG = this.MAIN_WO_FLAG;
            DataObject.CHANGE_INFORMATION = this.CHANGE_INFORMATION;
            DataObject.COMPLETE_DATE = this.COMPLETE_DATE;
            DataObject.START_DATE = this.START_DATE;
            DataObject.ROHS = this.ROHS;
            DataObject.QTY = this.QTY;
            DataObject.HW_ITEM = this.HW_ITEM;
            DataObject.HH_ITEM = this.HH_ITEM;
            DataObject.V_TASK_NO = this.V_TASK_NO;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.PRODUCT_LINE = this.PRODUCT_LINE;
            DataObject.SAP_FACTORY = this.SAP_FACTORY;
            DataObject.TASK_NO_USE = this.TASK_NO_USE;
            DataObject.TASK_NO_TYPE = this.TASK_NO_TYPE;
            DataObject.RECEIVE_DATE = this.RECEIVE_DATE;
            return DataObject;
        }
        public string TASK_NO_LEVEL
        {
            get
            {
                return (string)this["TASK_NO_LEVEL"];
            }
            set
            {
                this["TASK_NO_LEVEL"] = value;
            }
        }
        public double? SUGGEST_QTY
        {
            get
            {
                return (double?)this["SUGGEST_QTY"];
            }
            set
            {
                this["SUGGEST_QTY"] = value;
            }
        }
        public double? CREAT_WO_QTY
        {
            get
            {
                return (double?)this["CREAT_WO_QTY"];
            }
            set
            {
                this["CREAT_WO_QTY"] = value;
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
        public string ZNP195_MESSAGE
        {
            get
            {
                return (string)this["ZNP195_MESSAGE"];
            }
            set
            {
                this["ZNP195_MESSAGE"] = value;
            }
        }
        public string WO_TYPE
        {
            get
            {
                return (string)this["WO_TYPE"];
            }
            set
            {
                this["WO_TYPE"] = value;
            }
        }
        public string ZNP195_FLAG
        {
            get
            {
                return (string)this["ZNP195_FLAG"];
            }
            set
            {
                this["ZNP195_FLAG"] = value;
            }
        }
        public string ITEM_VERSION
        {
            get
            {
                return (string)this["ITEM_VERSION"];
            }
            set
            {
                this["ITEM_VERSION"] = value;
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
        public string WO_REMARK
        {
            get
            {
                return (string)this["WO_REMARK"];
            }
            set
            {
                this["WO_REMARK"] = value;
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
        public string CHANGE_INFORMATION
        {
            get
            {
                return (string)this["CHANGE_INFORMATION"];
            }
            set
            {
                this["CHANGE_INFORMATION"] = value;
            }
        }
        public string COMPLETE_DATE
        {
            get
            {
                return (string)this["COMPLETE_DATE"];
            }
            set
            {
                this["COMPLETE_DATE"] = value;
            }
        }
        public string START_DATE
        {
            get
            {
                return (string)this["START_DATE"];
            }
            set
            {
                this["START_DATE"] = value;
            }
        }
        public string ROHS
        {
            get
            {
                return (string)this["ROHS"];
            }
            set
            {
                this["ROHS"] = value;
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
        public string PRODUCT_LINE
        {
            get
            {
                return (string)this["PRODUCT_LINE"];
            }
            set
            {
                this["PRODUCT_LINE"] = value;
            }
        }
        public string SAP_FACTORY
        {
            get
            {
                return (string)this["SAP_FACTORY"];
            }
            set
            {
                this["SAP_FACTORY"] = value;
            }
        }
        public string TASK_NO_USE
        {
            get
            {
                return (string)this["TASK_NO_USE"];
            }
            set
            {
                this["TASK_NO_USE"] = value;
            }
        }
        public string TASK_NO_TYPE
        {
            get
            {
                return (string)this["TASK_NO_TYPE"];
            }
            set
            {
                this["TASK_NO_TYPE"] = value;
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
    }
    public class R_7B5_WO_TEMP
    {
        public string TASK_NO_LEVEL { get; set; }
        public double? SUGGEST_QTY { get; set; }
        public double? CREAT_WO_QTY { get; set; }
        public string MODEL { get; set; }
        public string ZNP195_MESSAGE { get; set; }
        public string WO_TYPE { get; set; }
        public string ZNP195_FLAG { get; set; }
        public string ITEM_VERSION { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string WO_REMARK { get; set; }
        public string MAIN_WO_FLAG { get; set; }
        public string CHANGE_INFORMATION { get; set; }
        public string COMPLETE_DATE { get; set; }
        public string START_DATE { get; set; }
        public string ROHS { get; set; }
        public double? QTY { get; set; }
        public string HW_ITEM { get; set; }
        public string HH_ITEM { get; set; }
        public string V_TASK_NO { get; set; }
        public string TASK_NO { get; set; }
        public string PRODUCT_LINE { get; set; }
        public string SAP_FACTORY { get; set; }
        public string TASK_NO_USE { get; set; }
        public string TASK_NO_TYPE { get; set; }
        public DateTime? RECEIVE_DATE { get; set; }
    }
}