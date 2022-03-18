using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_VT_ORDER_COMMIT : DataObjectTable
    {
        public T_R_VT_ORDER_COMMIT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_VT_ORDER_COMMIT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_VT_ORDER_COMMIT);
            TableName = "R_VT_ORDER_COMMIT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_VT_ORDER_COMMIT : DataObjectBase
    {
        public Row_R_VT_ORDER_COMMIT(DataObjectInfo info) : base(info)
        {

        }
        public R_VT_ORDER_COMMIT GetDataObject()
        {
            R_VT_ORDER_COMMIT DataObject = new R_VT_ORDER_COMMIT();
            DataObject.ID = this.ID;
            DataObject.VT_ORDER_ID = this.VT_ORDER_ID;
            DataObject.COMMIT_DETAIL = this.COMMIT_DETAIL;
            DataObject.COMMIT_EMP = this.COMMIT_EMP;
            DataObject.COMMIT_TIME = this.COMMIT_TIME;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.SEND_TIME = this.SEND_TIME;
            DataObject.SEND_FILE = this.SEND_FILE;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string VT_ORDER_ID
        {
            get
            {
                return (string)this["VT_ORDER_ID"];
            }
            set
            {
                this["VT_ORDER_ID"] = value;
            }
        }
        public string COMMIT_DETAIL
        {
            get
            {
                return (string)this["COMMIT_DETAIL"];
            }
            set
            {
                this["COMMIT_DETAIL"] = value;
            }
        }
        public string COMMIT_EMP
        {
            get
            {
                return (string)this["COMMIT_EMP"];
            }
            set
            {
                this["COMMIT_EMP"] = value;
            }
        }
        public DateTime? COMMIT_TIME
        {
            get
            {
                return (DateTime?)this["COMMIT_TIME"];
            }
            set
            {
                this["COMMIT_TIME"] = value;
            }
        }
        public double? SEND_FLAG
        {
            get
            {
                return (double?)this["SEND_FLAG"];
            }
            set
            {
                this["SEND_FLAG"] = value;
            }
        }
        public DateTime? SEND_TIME
        {
            get
            {
                return (DateTime?)this["SEND_TIME"];
            }
            set
            {
                this["SEND_TIME"] = value;
            }
        }
        public string SEND_FILE
        {
            get
            {
                return (string)this["SEND_FILE"];
            }
            set
            {
                this["SEND_FILE"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        
    }
    public class R_VT_ORDER_COMMIT
    {
        public string ID { get; set; }
        public string VT_ORDER_ID { get; set; }
        public string COMMIT_DETAIL { get; set; }
        public string COMMIT_EMP { get; set; }
        public DateTime? COMMIT_TIME { get; set; }
        public double? SEND_FLAG { get; set; }
        public DateTime? SEND_TIME { get; set; }
        public string SEND_FILE { get; set; }

        public string VALID_FLAG { get; set; }
    }
}