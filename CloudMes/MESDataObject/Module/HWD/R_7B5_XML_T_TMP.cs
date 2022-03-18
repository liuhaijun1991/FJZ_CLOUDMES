using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_XML_T_TMP : DataObjectTable
    {
        public T_R_7B5_XML_T_TMP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_XML_T_TMP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_XML_T_TMP);
            TableName = "R_7B5_XML_T_TMP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int DeleteByTaskNO(OleExec sfcdb, string task_no)
        {
            return sfcdb.ORM.Deleteable<R_7B5_XML_T_TMP>().Where(r => r.TASK_NO == task_no).ExecuteCommand();
        }
    }
    public class Row_R_7B5_XML_T_TMP : DataObjectBase
    {
        public Row_R_7B5_XML_T_TMP(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_XML_T_TMP GetDataObject()
        {
            R_7B5_XML_T_TMP DataObject = new R_7B5_XML_T_TMP();
            DataObject.ITEM = this.ITEM;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.EMS_CODE = this.EMS_CODE;
            DataObject.TASK_CHANGE_NO = this.TASK_CHANGE_NO;
            DataObject.CANCEL_FLAG = this.CANCEL_FLAG;
            DataObject.PO_FLAG = this.PO_FLAG;
            DataObject.PLAN_QTY = this.PLAN_QTY;
            DataObject.PLAN_FLAG = this.PLAN_FLAG;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.MODEL = this.MODEL;
            DataObject.PRODUCT_LINE = this.PRODUCT_LINE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.CREATE_WO_FLAG = this.CREATE_WO_FLAG;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.TRANSFER_FLAG = this.TRANSFER_FLAG;
            DataObject.PUBLISH_TIME = this.PUBLISH_TIME;
            DataObject.COMPONENT_REMARK = this.COMPONENT_REMARK;
            DataObject.COMPONENT_QTY = this.COMPONENT_QTY;
            DataObject.COMPONENT_VERSION = this.COMPONENT_VERSION;
            DataObject.COMPONENT = this.COMPONENT;
            DataObject.TASK_NOREMARK = this.TASK_NOREMARK;
            DataObject.COMPLETE_DATE = this.COMPLETE_DATE;
            DataObject.START_DATE = this.START_DATE;
            DataObject.RELEASE_DATE = this.RELEASE_DATE;
            DataObject.QTY = this.QTY;
            DataObject.ROHS = this.ROHS;
            DataObject.ITEM_VERSION = this.ITEM_VERSION;
            DataObject.CHANGE_INFORMATION = this.CHANGE_INFORMATION;
            DataObject.PO_SUPPLY_INFORMATION = this.PO_SUPPLY_INFORMATION;
            DataObject.SUPPLY_TYPE = this.SUPPLY_TYPE;
            DataObject.TASK_CHANGE_CONFIRM = this.TASK_CHANGE_CONFIRM;
            return DataObject;
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
        public string EMS_CODE
        {
            get
            {
                return (string)this["EMS_CODE"];
            }
            set
            {
                this["EMS_CODE"] = value;
            }
        }
        public string TASK_CHANGE_NO
        {
            get
            {
                return (string)this["TASK_CHANGE_NO"];
            }
            set
            {
                this["TASK_CHANGE_NO"] = value;
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
        public string PO_FLAG
        {
            get
            {
                return (string)this["PO_FLAG"];
            }
            set
            {
                this["PO_FLAG"] = value;
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
        public string PLAN_FLAG
        {
            get
            {
                return (string)this["PLAN_FLAG"];
            }
            set
            {
                this["PLAN_FLAG"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
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
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }
        public string CREATE_WO_FLAG
        {
            get
            {
                return (string)this["CREATE_WO_FLAG"];
            }
            set
            {
                this["CREATE_WO_FLAG"] = value;
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
        public string TRANSFER_FLAG
        {
            get
            {
                return (string)this["TRANSFER_FLAG"];
            }
            set
            {
                this["TRANSFER_FLAG"] = value;
            }
        }
        public DateTime? PUBLISH_TIME
        {
            get
            {
                return (DateTime?)this["PUBLISH_TIME"];
            }
            set
            {
                this["PUBLISH_TIME"] = value;
            }
        }
        public string COMPONENT_REMARK
        {
            get
            {
                return (string)this["COMPONENT_REMARK"];
            }
            set
            {
                this["COMPONENT_REMARK"] = value;
            }
        }
        public double? COMPONENT_QTY
        {
            get
            {
                return (double?)this["COMPONENT_QTY"];
            }
            set
            {
                this["COMPONENT_QTY"] = value;
            }
        }
        public string COMPONENT_VERSION
        {
            get
            {
                return (string)this["COMPONENT_VERSION"];
            }
            set
            {
                this["COMPONENT_VERSION"] = value;
            }
        }
        public string COMPONENT
        {
            get
            {
                return (string)this["COMPONENT"];
            }
            set
            {
                this["COMPONENT"] = value;
            }
        }
        public string TASK_NOREMARK
        {
            get
            {
                return (string)this["TASK_NOREMARK"];
            }
            set
            {
                this["TASK_NOREMARK"] = value;
            }
        }
        public DateTime? COMPLETE_DATE
        {
            get
            {
                return (DateTime?)this["COMPLETE_DATE"];
            }
            set
            {
                this["COMPLETE_DATE"] = value;
            }
        }
        public DateTime? START_DATE
        {
            get
            {
                return (DateTime?)this["START_DATE"];
            }
            set
            {
                this["START_DATE"] = value;
            }
        }
        public string RELEASE_DATE
        {
            get
            {
                return (string)this["RELEASE_DATE"];
            }
            set
            {
                this["RELEASE_DATE"] = value;
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
        public string PO_SUPPLY_INFORMATION
        {
            get
            {
                return (string)this["PO_SUPPLY_INFORMATION"];
            }
            set
            {
                this["PO_SUPPLY_INFORMATION"] = value;
            }
        }
        public string SUPPLY_TYPE
        {
            get
            {
                return (string)this["SUPPLY_TYPE"];
            }
            set
            {
                this["SUPPLY_TYPE"] = value;
            }
        }
        public string TASK_CHANGE_CONFIRM
        {
            get
            {
                return (string)this["TASK_CHANGE_CONFIRM"];
            }
            set
            {
                this["TASK_CHANGE_CONFIRM"] = value;
            }
        }
    }
    public class R_7B5_XML_T_TMP
    {
        public string ITEM { get; set; }
        public string TASK_NO { get; set; }
        public string EMS_CODE { get; set; }
        public string TASK_CHANGE_NO { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string PO_FLAG { get; set; }
        public double? PLAN_QTY { get; set; }
        public string PLAN_FLAG { get; set; }
        public string CATEGORY { get; set; }
        public string MODEL { get; set; }
        public string PRODUCT_LINE { get; set; }
        public string DESCRIPTION { get; set; }
        public string LOT_NO { get; set; }
        public string CREATE_WO_FLAG { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string TRANSFER_FLAG { get; set; }
        public DateTime? PUBLISH_TIME { get; set; }
        public string COMPONENT_REMARK { get; set; }
        public double? COMPONENT_QTY { get; set; }
        public string COMPONENT_VERSION { get; set; }
        public string COMPONENT { get; set; }
        public string TASK_NOREMARK { get; set; }
        public DateTime? COMPLETE_DATE { get; set; }
        public DateTime? START_DATE { get; set; }
        public string RELEASE_DATE { get; set; }
        public double? QTY { get; set; }
        public string ROHS { get; set; }
        public string ITEM_VERSION { get; set; }
        public string CHANGE_INFORMATION { get; set; }
        public string PO_SUPPLY_INFORMATION { get; set; }
        public string SUPPLY_TYPE { get; set; }
        public string TASK_CHANGE_CONFIRM { get; set; }
    }
}