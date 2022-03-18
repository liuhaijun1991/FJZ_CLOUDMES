using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_PO_ITEMS : DataObjectTable
    {
        public T_R_QUACK_PO_ITEMS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_PO_ITEMS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_PO_ITEMS);
            TableName = "R_QUACK_PO_ITEMS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_QUACK_PO_ITEMS GetRowByPoNo(string poNo,OleExec db)
        {
            return db.ORM.Queryable<R_QUACK_PO_ITEMS>().Where(t => t.PO_NO == poNo).ToList().FirstOrDefault();
        }

        public int UpdateRowByPoNo(R_QUACK_PO_ITEMS rQuackPoItemsRow, OleExec db)
        {
            return db.ORM.Updateable(rQuackPoItemsRow).Where(t => t.PO_NO == rQuackPoItemsRow.PO_NO).ExecuteCommand();
        }

    }
    public class Row_R_QUACK_PO_ITEMS : DataObjectBase
    {
        public Row_R_QUACK_PO_ITEMS(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_PO_ITEMS GetDataObject()
        {
            R_QUACK_PO_ITEMS DataObject = new R_QUACK_PO_ITEMS();
            DataObject.ID = this.ID;
            DataObject.PO_NO = this.PO_NO;
            DataObject.PO_ITEM = this.PO_ITEM;
            DataObject.PARTNO = this.PARTNO;
            DataObject.PARTDESC = this.PARTDESC;
            DataObject.PO_QTY = this.PO_QTY;
            DataObject.PLANT = this.PLANT;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.CREATE_DATE = this.CREATE_DATE;
            DataObject.CHANGE_EMP = this.CHANGE_EMP;
            DataObject.CHANGE_DATE = this.CHANGE_DATE;
            DataObject.CHANGE_TIME = this.CHANGE_TIME;
            DataObject.DELETETION = this.DELETETION;
            DataObject.PO_PRICE = this.PO_PRICE;
            DataObject.LOCATION = this.LOCATION;
            DataObject.SFC_CLOSED_FLAG = this.SFC_CLOSED_FLAG;
            DataObject.SAPGR_FLAG = this.SAPGR_FLAG;
            DataObject.SAPGR_TIME = this.SAPGR_TIME;
            DataObject.SAPST_FLAG = this.SAPST_FLAG;
            DataObject.SAPST_TIME = this.SAPST_TIME;
            DataObject.SAPGT_FLAG = this.SAPGT_FLAG;
            DataObject.SAPGT_TIME = this.SAPGT_TIME;
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
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string PO_ITEM
        {
            get
            {
                return (string)this["PO_ITEM"];
            }
            set
            {
                this["PO_ITEM"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string PARTDESC
        {
            get
            {
                return (string)this["PARTDESC"];
            }
            set
            {
                this["PARTDESC"] = value;
            }
        }
        public double? PO_QTY
        {
            get
            {
                return (double?)this["PO_QTY"];
            }
            set
            {
                this["PO_QTY"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? CREATE_DATE
        {
            get
            {
                return (DateTime?)this["CREATE_DATE"];
            }
            set
            {
                this["CREATE_DATE"] = value;
            }
        }
        public string CHANGE_EMP
        {
            get
            {
                return (string)this["CHANGE_EMP"];
            }
            set
            {
                this["CHANGE_EMP"] = value;
            }
        }
        public string CHANGE_DATE
        {
            get
            {
                return (string)this["CHANGE_DATE"];
            }
            set
            {
                this["CHANGE_DATE"] = value;
            }
        }
        public string CHANGE_TIME
        {
            get
            {
                return (string)this["CHANGE_TIME"];
            }
            set
            {
                this["CHANGE_TIME"] = value;
            }
        }
        public string DELETETION
        {
            get
            {
                return (string)this["DELETETION"];
            }
            set
            {
                this["DELETETION"] = value;
            }
        }
        public double? PO_PRICE
        {
            get
            {
                return (double?)this["PO_PRICE"];
            }
            set
            {
                this["PO_PRICE"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string SFC_CLOSED_FLAG
        {
            get
            {
                return (string)this["SFC_CLOSED_FLAG"];
            }
            set
            {
                this["SFC_CLOSED_FLAG"] = value;
            }
        }
        public string SAPGR_FLAG
        {
            get
            {
                return (string)this["SAPGR_FLAG"];
            }
            set
            {
                this["SAPGR_FLAG"] = value;
            }
        }
        public DateTime? SAPGR_TIME
        {
            get
            {
                return (DateTime?)this["SAPGR_TIME"];
            }
            set
            {
                this["SAPGR_TIME"] = value;
            }
        }
        public string SAPST_FLAG
        {
            get
            {
                return (string)this["SAPST_FLAG"];
            }
            set
            {
                this["SAPST_FLAG"] = value;
            }
        }
        public DateTime? SAPST_TIME
        {
            get
            {
                return (DateTime?)this["SAPST_TIME"];
            }
            set
            {
                this["SAPST_TIME"] = value;
            }
        }
        public string SAPGT_FLAG
        {
            get
            {
                return (string)this["SAPGT_FLAG"];
            }
            set
            {
                this["SAPGT_FLAG"] = value;
            }
        }
        public DateTime? SAPGT_TIME
        {
            get
            {
                return (DateTime?)this["SAPGT_TIME"];
            }
            set
            {
                this["SAPGT_TIME"] = value;
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
    public class R_QUACK_PO_ITEMS
    {
        public string ID { get; set; }
        public string PO_NO { get; set; }
        public string PO_ITEM { get; set; }
        public string PARTNO { get; set; }
        public string PARTDESC { get; set; }
        public double? PO_QTY { get; set; }
        public string PLANT { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string CHANGE_EMP { get; set; }
        public string CHANGE_DATE { get; set; }
        public string CHANGE_TIME { get; set; }
        public string DELETETION { get; set; }
        public double? PO_PRICE { get; set; }
        public string LOCATION { get; set; }
        public string SFC_CLOSED_FLAG { get; set; }
        public string SAPGR_FLAG { get; set; }
        public DateTime? SAPGR_TIME { get; set; }
        public string SAPST_FLAG { get; set; }
        public DateTime? SAPST_TIME { get; set; }
        public string SAPGT_FLAG { get; set; }
        public DateTime? SAPGT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}