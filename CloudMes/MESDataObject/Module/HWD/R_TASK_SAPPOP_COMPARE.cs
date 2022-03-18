using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module.HWD
{
    public class T_r_task_sappop_compare : DataObjectTable
    {
        public T_r_task_sappop_compare(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_r_task_sappop_compare(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_r_task_sappop_compare);
            TableName = "r_task_sappop_compare".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

       
    }
    public class Row_r_task_sappop_compare : DataObjectBase
    {
        public Row_r_task_sappop_compare(DataObjectInfo info) : base(info)
        {

        }
        public R_TASK_SAPPOP_COMPARE GetDataObject()
        {
            R_TASK_SAPPOP_COMPARE DataObject = new R_TASK_SAPPOP_COMPARE();
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA1 = this.DATA1;
            DataObject.MEMO = this.MEMO;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.DIFF_REMARK = this.DIFF_REMARK;
            DataObject.RESULT = this.RESULT;
            DataObject.POP_NUM = this.POP_NUM;
            DataObject.VERSION_NUM = this.VERSION_NUM;
            DataObject.VERSION_DATE = this.VERSION_DATE;
            DataObject.UNIT_POP = this.UNIT_POP;
            DataObject.QTY_POP = this.QTY_POP;
            DataObject.HW_ITEM_POP = this.HW_ITEM_POP;
            DataObject.QTY_SAP = this.QTY_SAP;
            DataObject.WO_UNIT_SAP = this.WO_UNIT_SAP;
            DataObject.HW_ITEM_SAP = this.HW_ITEM_SAP;
            DataObject.FOX_ITEM_SAP = this.FOX_ITEM_SAP;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.TASK_NO = this.TASK_NO;
            return DataObject;
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string MEMO
        {
            get
            {
                return (string)this["MEMO"];
            }
            set
            {
                this["MEMO"] = value;
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
        public string DIFF_REMARK
        {
            get
            {
                return (string)this["DIFF_REMARK"];
            }
            set
            {
                this["DIFF_REMARK"] = value;
            }
        }
        public string RESULT
        {
            get
            {
                return (string)this["RESULT"];
            }
            set
            {
                this["RESULT"] = value;
            }
        }
        public string POP_NUM
        {
            get
            {
                return (string)this["POP_NUM"];
            }
            set
            {
                this["POP_NUM"] = value;
            }
        }
        public double? VERSION_NUM
        {
            get
            {
                return (double?)this["VERSION_NUM"];
            }
            set
            {
                this["VERSION_NUM"] = value;
            }
        }
        public string VERSION_DATE
        {
            get
            {
                return (string)this["VERSION_DATE"];
            }
            set
            {
                this["VERSION_DATE"] = value;
            }
        }
        public double? UNIT_POP
        {
            get
            {
                return (double?)this["UNIT_POP"];
            }
            set
            {
                this["UNIT_POP"] = value;
            }
        }
        public double? QTY_POP
        {
            get
            {
                return (double?)this["QTY_POP"];
            }
            set
            {
                this["QTY_POP"] = value;
            }
        }
        public string HW_ITEM_POP
        {
            get
            {
                return (string)this["HW_ITEM_POP"];
            }
            set
            {
                this["HW_ITEM_POP"] = value;
            }
        }
        public double? QTY_SAP
        {
            get
            {
                return (double?)this["QTY_SAP"];
            }
            set
            {
                this["QTY_SAP"] = value;
            }
        }
        public double? WO_UNIT_SAP
        {
            get
            {
                return (double?)this["WO_UNIT_SAP"];
            }
            set
            {
                this["WO_UNIT_SAP"] = value;
            }
        }
        public string HW_ITEM_SAP
        {
            get
            {
                return (string)this["HW_ITEM_SAP"];
            }
            set
            {
                this["HW_ITEM_SAP"] = value;
            }
        }
        public string FOX_ITEM_SAP
        {
            get
            {
                return (string)this["FOX_ITEM_SAP"];
            }
            set
            {
                this["FOX_ITEM_SAP"] = value;
            }
        }
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
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
    public class R_TASK_SAPPOP_COMPARE
    {
      
        public string TASK_NO { get; set; }
        public string SKUNO { get; set; }
        public string WO { get; set; }
        public string FOX_ITEM_SAP { get; set; }
        public string HW_ITEM_SAP { get; set; }
        public double? WO_UNIT_SAP { get; set; }
        public double? QTY_SAP { get; set; }
        public string HW_ITEM_POP { get; set; }
        public double? QTY_POP { get; set; }
        public double? UNIT_POP { get; set; }
        public string VERSION_DATE { get; set; }
        public double? VERSION_NUM { get; set; }
        public string POP_NUM { get; set; }
        public string RESULT { get; set; }
        public string DIFF_REMARK { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string MEMO { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
    }
}
