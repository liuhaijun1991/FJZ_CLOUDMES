using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JNP_LINE_STOCK : DataObjectTable
    {
        public T_R_JNP_LINE_STOCK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_LINE_STOCK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_LINE_STOCK);
            TableName = "R_JNP_LINE_STOCK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_LINE_STOCK : DataObjectBase
    {
        public Row_R_JNP_LINE_STOCK(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_LINE_STOCK GetDataObject()
        {
            R_JNP_LINE_STOCK DataObject = new R_JNP_LINE_STOCK();
            DataObject.ID = this.ID;
            DataObject.STOCK_LOCATION = this.STOCK_LOCATION;
            DataObject.PN = this.PN;
            DataObject.QTY = this.QTY;
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
        public string STOCK_LOCATION
        {
            get
            {
                return (string)this["STOCK_LOCATION"];
            }
            set
            {
                this["STOCK_LOCATION"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
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
    public class R_JNP_LINE_STOCK
    {
        public string ID { get; set; }
        public string STOCK_LOCATION { get; set; }
        public string PN { get; set; }
        public double? QTY { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}


