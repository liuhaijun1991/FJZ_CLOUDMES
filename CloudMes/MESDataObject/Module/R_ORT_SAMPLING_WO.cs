using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORT_SAMPLING_WO : DataObjectTable
    {
        public T_R_ORT_SAMPLING_WO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORT_SAMPLING_WO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORT_SAMPLING_WO);
            TableName = "R_ORT_SAMPLING_WO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORT_SAMPLING_WO : DataObjectBase
    {
        public Row_R_ORT_SAMPLING_WO(DataObjectInfo info) : base(info)
        {

        }
        public R_ORT_SAMPLING_WO GetDataObject()
        {
            R_ORT_SAMPLING_WO DataObject = new R_ORT_SAMPLING_WO();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WOQTY = this.WOQTY;
            DataObject.SAMPLING_TOTAL = this.SAMPLING_TOTAL;
            DataObject.SAMPLING_RATE = this.SAMPLING_RATE;
            DataObject.SEQNO = this.SEQNO;
            DataObject.SAMPLING_SEQNO = this.SAMPLING_SEQNO;
            DataObject.SAMPLING_FLAG = this.SAMPLING_FLAG;
            DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
            DataObject.STATION = this.STATION;
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
        public double? WOQTY
        {
            get
            {
                return (double?)this["WOQTY"];
            }
            set
            {
                this["WOQTY"] = value;
            }
        }
        public double? SAMPLING_TOTAL
        {
            get
            {
                return (double?)this["SAMPLING_TOTAL"];
            }
            set
            {
                this["SAMPLING_TOTAL"] = value;
            }
        }
        public double? SAMPLING_RATE
        {
            get
            {
                return (double?)this["SAMPLING_RATE"];
            }
            set
            {
                this["SAMPLING_RATE"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public double? SAMPLING_SEQNO
        {
            get
            {
                return (double?)this["SAMPLING_SEQNO"];
            }
            set
            {
                this["SAMPLING_SEQNO"] = value;
            }
        }
        public string SAMPLING_FLAG
        {
            get
            {
                return (string)this["SAMPLING_FLAG"];
            }
            set
            {
                this["SAMPLING_FLAG"] = value;
            }
        }
        public string SAMPLING_TYPE
        {
            get
            {
                return (string)this["SAMPLING_TYPE"];
            }
            set
            {
                this["SAMPLING_TYPE"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
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
    public class R_ORT_SAMPLING_WO
    {
        public string ID;
        public string WO;
        public string SKUNO;
        public double? WOQTY;
        public double? SAMPLING_TOTAL;
        public double? SAMPLING_RATE;
        public double? SEQNO;
        public double? SAMPLING_SEQNO;
        public string SAMPLING_FLAG;
        public string SAMPLING_TYPE;
        public string STATION;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}