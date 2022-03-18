using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_C_ITEM_BOM : DataObjectTable
    {
        public T_C_ITEM_BOM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ITEM_BOM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ITEM_BOM);
            TableName = "C_ITEM_BOM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public string GET_SKU_7B5_LEVEL(OleExec sfcdb, string skuno)
        {
            C_ITEM_BOM itemBom = sfcdb.ORM.Queryable<C_ITEM_BOM>().Where(r => r.BOM_ID == skuno).ToList().FirstOrDefault();
            if (itemBom == null)
            {
                return "NO BOM";
            }
            else
            {
                if (string.IsNullOrEmpty(itemBom.PARENT_KPNO))
                {
                    return "SON";
                }
                else
                {
                    return "FATHER";
                }
            }
        }
    }
    public class Row_C_ITEM_BOM : DataObjectBase
    {
        public Row_C_ITEM_BOM(DataObjectInfo info) : base(info)
        {

        }
        public C_ITEM_BOM GetDataObject()
        {
            C_ITEM_BOM DataObject = new C_ITEM_BOM();
            DataObject.ID = this.ID;
            DataObject.HW_ITEM = this.HW_ITEM;
            DataObject.BOM_ID = this.BOM_ID;
            DataObject.PARENT_KPNO = this.PARENT_KPNO;
            DataObject.SON_KPNO = this.SON_KPNO;
            DataObject.STANDARD_QTY = this.STANDARD_QTY;
            DataObject.ROHS = this.ROHS;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string BOM_ID
        {
            get
            {
                return (string)this["BOM_ID"];
            }
            set
            {
                this["BOM_ID"] = value;
            }
        }
        public string PARENT_KPNO
        {
            get
            {
                return (string)this["PARENT_KPNO"];
            }
            set
            {
                this["PARENT_KPNO"] = value;
            }
        }
        public string SON_KPNO
        {
            get
            {
                return (string)this["SON_KPNO"];
            }
            set
            {
                this["SON_KPNO"] = value;
            }
        }
        public double? STANDARD_QTY
        {
            get
            {
                return (double?)this["STANDARD_QTY"];
            }
            set
            {
                this["STANDARD_QTY"] = value;
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
    }
    public class C_ITEM_BOM
    {
        public string ID { get; set; }
        public string HW_ITEM { get; set; }
        public string BOM_ID { get; set; }
        public string PARENT_KPNO { get; set; }
        public string SON_KPNO { get; set; }
        public double? STANDARD_QTY { get; set; }
        public string ROHS { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}