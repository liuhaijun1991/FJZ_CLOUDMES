using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP_FILE_I590 : DataObjectTable
    {
        public T_R_SAP_FILE_I590(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_FILE_I590(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_FILE_I590);
            TableName = "R_SAP_FILE_I590".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_FILE_I590 : DataObjectBase
    {
        public Row_R_SAP_FILE_I590(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_FILE_I590 GetDataObject()
        {
            R_SAP_FILE_I590 DataObject = new R_SAP_FILE_I590();
            DataObject.ID = this.ID;
            DataObject.FILE_ID = this.FILE_ID;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.LAST_UPDATE_TIME = this.LAST_UPDATE_TIME;
            DataObject.GROUP_CODE = this.GROUP_CODE;
            DataObject.SUPPLIER_SITE = this.SUPPLIER_SITE;
            DataObject.CM_PART_NUMBER = this.CM_PART_NUMBER;
            DataObject.ITEM_NAME = this.ITEM_NAME;
            DataObject.ITEM_TYPE = this.ITEM_TYPE;
            DataObject.UOM = this.UOM;
            DataObject.MAKE_BUY_FLAG = this.MAKE_BUY_FLAG;
            DataObject.PHANTOM_FLAG = this.PHANTOM_FLAG;
            DataObject.KANBAN_FLAG = this.KANBAN_FLAG;
            DataObject.ROP_FLAG = this.ROP_FLAG;
            DataObject.ROP_QUANTITY = this.ROP_QUANTITY;
            DataObject.SAFETY_STOCK = this.SAFETY_STOCK;
            DataObject.MOQ = this.MOQ;
            DataObject.PURCHASING_LEAD_TIME = this.PURCHASING_LEAD_TIME;
            DataObject.MFG_CYCLE_TIME = this.MFG_CYCLE_TIME;
            DataObject.RECOMMENDED_ORDER_QTY = this.RECOMMENDED_ORDER_QTY;
            DataObject.ABC_CODE = this.ABC_CODE;
            DataObject.NCNR_FLAG = this.NCNR_FLAG;
            DataObject.CURRENT_REV = this.CURRENT_REV;
            DataObject.CM_STD_COST = this.CM_STD_COST;
            DataObject.ORDER_MULTIPLE = this.ORDER_MULTIPLE;
            DataObject.CYCLE_TIME_TO_REPLENISH = this.CYCLE_TIME_TO_REPLENISH;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.JUNIPER_LIABILITY_INDICATOR = this.JUNIPER_LIABILITY_INDICATOR;
            DataObject.NON_BOM_INDICATOR = this.NON_BOM_INDICATOR;
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
        public string FILE_ID
        {
            get
            {
                return (string)this["FILE_ID"];
            }
            set
            {
                this["FILE_ID"] = value;
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
        public DateTime? LAST_UPDATE_TIME
        {
            get
            {
                return (DateTime?)this["LAST_UPDATE_TIME"];
            }
            set
            {
                this["LAST_UPDATE_TIME"] = value;
            }
        }
        public string GROUP_CODE
        {
            get
            {
                return (string)this["GROUP_CODE"];
            }
            set
            {
                this["GROUP_CODE"] = value;
            }
        }
        public string SUPPLIER_SITE
        {
            get
            {
                return (string)this["SUPPLIER_SITE"];
            }
            set
            {
                this["SUPPLIER_SITE"] = value;
            }
        }
        public string CM_PART_NUMBER
        {
            get
            {
                return (string)this["CM_PART_NUMBER"];
            }
            set
            {
                this["CM_PART_NUMBER"] = value;
            }
        }
        public string ITEM_NAME
        {
            get
            {
                return (string)this["ITEM_NAME"];
            }
            set
            {
                this["ITEM_NAME"] = value;
            }
        }
        public string ITEM_TYPE
        {
            get
            {
                return (string)this["ITEM_TYPE"];
            }
            set
            {
                this["ITEM_TYPE"] = value;
            }
        }
        public string UOM
        {
            get
            {
                return (string)this["UOM"];
            }
            set
            {
                this["UOM"] = value;
            }
        }
        public string MAKE_BUY_FLAG
        {
            get
            {
                return (string)this["MAKE_BUY_FLAG"];
            }
            set
            {
                this["MAKE_BUY_FLAG"] = value;
            }
        }
        public string PHANTOM_FLAG
        {
            get
            {
                return (string)this["PHANTOM_FLAG"];
            }
            set
            {
                this["PHANTOM_FLAG"] = value;
            }
        }
        public string KANBAN_FLAG
        {
            get
            {
                return (string)this["KANBAN_FLAG"];
            }
            set
            {
                this["KANBAN_FLAG"] = value;
            }
        }
        public string ROP_FLAG
        {
            get
            {
                return (string)this["ROP_FLAG"];
            }
            set
            {
                this["ROP_FLAG"] = value;
            }
        }
        public string ROP_QUANTITY
        {
            get
            {
                return (string)this["ROP_QUANTITY"];
            }
            set
            {
                this["ROP_QUANTITY"] = value;
            }
        }
        public string SAFETY_STOCK
        {
            get
            {
                return (string)this["SAFETY_STOCK"];
            }
            set
            {
                this["SAFETY_STOCK"] = value;
            }
        }
        public string MOQ
        {
            get
            {
                return (string)this["MOQ"];
            }
            set
            {
                this["MOQ"] = value;
            }
        }
        public string PURCHASING_LEAD_TIME
        {
            get
            {
                return (string)this["PURCHASING_LEAD_TIME"];
            }
            set
            {
                this["PURCHASING_LEAD_TIME"] = value;
            }
        }
        public string MFG_CYCLE_TIME
        {
            get
            {
                return (string)this["MFG_CYCLE_TIME"];
            }
            set
            {
                this["MFG_CYCLE_TIME"] = value;
            }
        }
        public string RECOMMENDED_ORDER_QTY
        {
            get
            {
                return (string)this["RECOMMENDED_ORDER_QTY"];
            }
            set
            {
                this["RECOMMENDED_ORDER_QTY"] = value;
            }
        }
        public string ABC_CODE
        {
            get
            {
                return (string)this["ABC_CODE"];
            }
            set
            {
                this["ABC_CODE"] = value;
            }
        }
        public string NCNR_FLAG
        {
            get
            {
                return (string)this["NCNR_FLAG"];
            }
            set
            {
                this["NCNR_FLAG"] = value;
            }
        }
        public string CURRENT_REV
        {
            get
            {
                return (string)this["CURRENT_REV"];
            }
            set
            {
                this["CURRENT_REV"] = value;
            }
        }
        public string CM_STD_COST
        {
            get
            {
                return (string)this["CM_STD_COST"];
            }
            set
            {
                this["CM_STD_COST"] = value;
            }
        }
        public string ORDER_MULTIPLE
        {
            get
            {
                return (string)this["ORDER_MULTIPLE"];
            }
            set
            {
                this["ORDER_MULTIPLE"] = value;
            }
        }
        public string CYCLE_TIME_TO_REPLENISH
        {
            get
            {
                return (string)this["CYCLE_TIME_TO_REPLENISH"];
            }
            set
            {
                this["CYCLE_TIME_TO_REPLENISH"] = value;
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
        public string JUNIPER_LIABILITY_INDICATOR
        {
            get
            {
                return (string)this["JUNIPER_LIABILITY_INDICATOR"];
            }
            set
            {
                this["JUNIPER_LIABILITY_INDICATOR"] = value;
            }
        }
        public string NON_BOM_INDICATOR
        {
            get
            {
                return (string)this["NON_BOM_INDICATOR"];
            }
            set
            {
                this["NON_BOM_INDICATOR"] = value;
            }
        }
    }
    public class R_SAP_FILE_I590
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string FILE_ID { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public DateTime? LAST_UPDATE_TIME { get; set; }
        public string GROUP_CODE { get; set; }
        public string SUPPLIER_SITE { get; set; }
        public string CM_PART_NUMBER { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_TYPE { get; set; }
        public string UOM { get; set; }
        public string MAKE_BUY_FLAG { get; set; }
        public string PHANTOM_FLAG { get; set; }
        public string KANBAN_FLAG { get; set; }
        public string ROP_FLAG { get; set; }
        public string ROP_QUANTITY { get; set; }
        public string SAFETY_STOCK { get; set; }
        public string MOQ { get; set; }
        public string PURCHASING_LEAD_TIME { get; set; }
        public string MFG_CYCLE_TIME { get; set; }
        public string RECOMMENDED_ORDER_QTY { get; set; }
        public string ABC_CODE { get; set; }
        public string NCNR_FLAG { get; set; }
        public string CURRENT_REV { get; set; }
        public string CM_STD_COST { get; set; }
        public string ORDER_MULTIPLE { get; set; }
        public string CYCLE_TIME_TO_REPLENISH { get; set; }
        public string DESCRIPTION { get; set; }
        public string JUNIPER_LIABILITY_INDICATOR { get; set; }
        public string NON_BOM_INDICATOR { get; set; }
    }
}