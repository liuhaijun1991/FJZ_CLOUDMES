using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_R_WH_STOCK_LIST : DataObjectTable
    {
        public T_R_WH_STOCK_LIST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WH_STOCK_LIST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WH_STOCK_LIST);
            TableName = "R_WH_STOCK_LIST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_WH_STOCK_LIST> GetStockListByTypeAndArea(OleExec sfcdb, string type, string area)
        {
            return sfcdb.ORM.Queryable<R_WH_STOCK_LIST>().Where(r => r.TYPE == type && r.AREA == area).ToList();
        }
    }
    public class Row_R_WH_STOCK_LIST : DataObjectBase
    {
        public Row_R_WH_STOCK_LIST(DataObjectInfo info) : base(info)
        {

        }
        public R_WH_STOCK_LIST GetDataObject()
        {
            R_WH_STOCK_LIST DataObject = new R_WH_STOCK_LIST();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.TYPE = this.TYPE;
            DataObject.REMARK = this.REMARK;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.ID = this.ID;
            DataObject.LOCATION = this.LOCATION;
            DataObject.AREA = this.AREA;
            DataObject.DEFAULT_QTY = this.DEFAULT_QTY;
            DataObject.CURRENT_QTY = this.CURRENT_QTY;
            DataObject.LESS_QTY = this.LESS_QTY;
            return DataObject;
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
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
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
        public string AREA
        {
            get
            {
                return (string)this["AREA"];
            }
            set
            {
                this["AREA"] = value;
            }
        }
        public double? DEFAULT_QTY
        {
            get
            {
                return (double?)this["DEFAULT_QTY"];
            }
            set
            {
                this["DEFAULT_QTY"] = value;
            }
        }
        public double? CURRENT_QTY
        {
            get
            {
                return (double?)this["CURRENT_QTY"];
            }
            set
            {
                this["CURRENT_QTY"] = value;
            }
        }
        public double? LESS_QTY
        {
            get
            {
                return (double?)this["LESS_QTY"];
            }
            set
            {
                this["LESS_QTY"] = value;
            }
        }
    }
    public class R_WH_STOCK_LIST
    {
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string TYPE { get; set; }
        public string REMARK { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string ID { get; set; }
        public string LOCATION { get; set; }
        public string AREA { get; set; }
        public double? DEFAULT_QTY { get; set; }
        public double? CURRENT_QTY { get; set; }
        public double? LESS_QTY { get; set; }
    }
}