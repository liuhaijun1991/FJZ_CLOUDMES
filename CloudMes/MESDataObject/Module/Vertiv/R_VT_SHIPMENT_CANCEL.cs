using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_VT_SHIPMENT_CANCEL : DataObjectTable
    {
        public T_R_VT_SHIPMENT_CANCEL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_VT_SHIPMENT_CANCEL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_VT_SHIPMENT_CANCEL);
            TableName = "R_VT_SHIPMENT_CANCEL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_VT_SHIPMENT_CANCEL : DataObjectBase
    {
        public Row_R_VT_SHIPMENT_CANCEL(DataObjectInfo info) : base(info)
        {

        }
        public R_VT_SHIPMENT_CANCEL GetDataObject()
        {
            R_VT_SHIPMENT_CANCEL DataObject = new R_VT_SHIPMENT_CANCEL();
            DataObject.CREATED_TIME = this.CREATED_TIME;
            DataObject.CREATED_EMP = this.CREATED_EMP;
            DataObject.NEW_ID = this.NEW_ID;
            DataObject.CANCEL_ID = this.CANCEL_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public DateTime? CREATED_TIME
        {
            get
            {
                return (DateTime?)this["CREATED_TIME"];
            }
            set
            {
                this["CREATED_TIME"] = value;
            }
        }
        public string CREATED_EMP
        {
            get
            {
                return (string)this["CREATED_EMP"];
            }
            set
            {
                this["CREATED_EMP"] = value;
            }
        }
        public string NEW_ID
        {
            get
            {
                return (string)this["NEW_ID"];
            }
            set
            {
                this["NEW_ID"] = value;
            }
        }
        public string CANCEL_ID
        {
            get
            {
                return (string)this["CANCEL_ID"];
            }
            set
            {
                this["CANCEL_ID"] = value;
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
    }
    public class R_VT_SHIPMENT_CANCEL
    {
        public DateTime? CREATED_TIME { get; set; }
        public string CREATED_EMP { get; set; }
        public string NEW_ID { get; set; }
        public string CANCEL_ID { get; set; }
        public string ID { get; set; }
    }
}