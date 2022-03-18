using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_r_task_order_sn : DataObjectTable
    {
        public T_r_task_order_sn(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_r_task_order_sn(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_r_task_order_sn);
            TableName = "r_task_order_sn".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Insert(r_task_order_sn rtos, OleExec DB)
        {
            return DB.ORM.Insertable<r_task_order_sn>(rtos).ExecuteCommand();
        }

        public bool CheckExists(string SN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM R_TASK_ORDER_SN WHERE SN = '{SN}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count == 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }
    }
    public class Row_r_task_order_sn : DataObjectBase
    {
        public Row_r_task_order_sn(DataObjectInfo info) : base(info)
        {

        }
        public r_task_order_sn GetDataObject()
        {
            r_task_order_sn DataObject = new r_task_order_sn();
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.HW_TASK_NO = this.HW_TASK_NO;
            DataObject.HW_TASK_ITEM = this.HW_TASK_ITEM;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WO = this.WO;
            DataObject.SN_STATUS = this.SN_STATUS;
            DataObject.WO_STATUS = this.WO_STATUS;
            DataObject.HW_TASK_STATUS = this.HW_TASK_STATUS;
            DataObject.MEMO = this.MEMO;
            DataObject.TASK_QTY = this.TASK_QTY;
            DataObject.TASK_SEQNO = this.TASK_SEQNO;
            DataObject.ASSIGNED_TASK = this.ASSIGNED_TASK;
            DataObject.ASSIGNED_TIME = this.ASSIGNED_TIME;
            DataObject.SHIPPING_TYPE = this.SHIPPING_TYPE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string HW_TASK_NO
        {
            get
            {
                return (string)this["HW_TASK_NO"];
            }
            set
            {
                this["HW_TASK_NO"] = value;
            }
        }
        public string HW_TASK_ITEM
        {
            get
            {
                return (string)this["HW_TASK_ITEM"];
            }
            set
            {
                this["HW_TASK_ITEM"] = value;
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
        public string SN_STATUS
        {
            get
            {
                return (string)this["SN_STATUS"];
            }
            set
            {
                this["SN_STATUS"] = value;
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
        public string HW_TASK_STATUS
        {
            get
            {
                return (string)this["HW_TASK_STATUS"];
            }
            set
            {
                this["HW_TASK_STATUS"] = value;
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
        public string TASK_QTY
        {
            get
            {
                return (string)this["TASK_QTY"];
            }
            set
            {
                this["TASK_QTY"] = value;
            }
        }
        public string TASK_SEQNO
        {
            get
            {
                return (string)this["TASK_SEQNO"];
            }
            set
            {
                this["TASK_SEQNO"] = value;
            }
        }
        public string ASSIGNED_TASK
        {
            get
            {
                return (string)this["ASSIGNED_TASK"];
            }
            set
            {
                this["ASSIGNED_TASK"] = value;
            }
        }
        public DateTime? ASSIGNED_TIME
        {
            get
            {
                return (DateTime?)this["ASSIGNED_TIME"];
            }
            set
            {
                this["ASSIGNED_TIME"] = value;
            }
        }
        public string SHIPPING_TYPE
        {
            get
            {
                return (string)this["SHIPPING_TYPE"];
            }
            set
            {
                this["SHIPPING_TYPE"] = value;
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
    public class r_task_order_sn
    {
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string ID { get; set; }
        public string SN { get; set; }
        public string HW_TASK_NO { get; set; }
        public string HW_TASK_ITEM { get; set; }
        public string SKUNO { get; set; }
        public string WO { get; set; }
        public string SN_STATUS { get; set; }
        public string WO_STATUS { get; set; }
        public string HW_TASK_STATUS { get; set; }
        public string MEMO { get; set; }
        public string TASK_QTY { get; set; }
        public string TASK_SEQNO { get; set; }
        public string ASSIGNED_TASK { get; set; }
        public DateTime? ASSIGNED_TIME { get; set; }
        public string SHIPPING_TYPE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}