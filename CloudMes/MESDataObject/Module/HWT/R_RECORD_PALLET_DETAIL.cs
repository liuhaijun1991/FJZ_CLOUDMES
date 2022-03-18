using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_R_RECORD_PALLET_DETAIL : DataObjectTable
    {
        public T_R_RECORD_PALLET_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RECORD_PALLET_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RECORD_PALLET_DETAIL);
            TableName = "R_RECORD_PALLET_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        
        public int Save(OleExec sfcdb, R_RECORD_PALLET_DETAIL detailObj)
        {
            return sfcdb.ORM.Insertable<R_RECORD_PALLET_DETAIL>(detailObj).ExecuteCommand();
        }
    }
    public class Row_R_RECORD_PALLET_DETAIL : DataObjectBase
    {
        public Row_R_RECORD_PALLET_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_RECORD_PALLET_DETAIL GetDataObject()
        {
            R_RECORD_PALLET_DETAIL DataObject = new R_RECORD_PALLET_DETAIL();
            DataObject.ID = this.ID;
            DataObject.PALLET_NO = this.PALLET_NO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKUVERISON = this.SKUVERISON;
            DataObject.ACTION_TYPE = this.ACTION_TYPE;
            DataObject.EVENTNAME = this.EVENTNAME;
            DataObject.TASK_FLAG = this.TASK_FLAG;
            DataObject.TASK_TIME = this.TASK_TIME;
            DataObject.DATA1 = this.DATA1;
            DataObject.LASTEDIT = this.LASTEDIT;
            DataObject.LASTBY = this.LASTBY;
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
        public string PALLET_NO
        {
            get
            {
                return (string)this["PALLET_NO"];
            }
            set
            {
                this["PALLET_NO"] = value;
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
        public string SKUVERISON
        {
            get
            {
                return (string)this["SKUVERISON"];
            }
            set
            {
                this["SKUVERISON"] = value;
            }
        }
        public string ACTION_TYPE
        {
            get
            {
                return (string)this["ACTION_TYPE"];
            }
            set
            {
                this["ACTION_TYPE"] = value;
            }
        }
        public string EVENTNAME
        {
            get
            {
                return (string)this["EVENTNAME"];
            }
            set
            {
                this["EVENTNAME"] = value;
            }
        }
        public string TASK_FLAG
        {
            get
            {
                return (string)this["TASK_FLAG"];
            }
            set
            {
                this["TASK_FLAG"] = value;
            }
        }
        public DateTime? TASK_TIME
        {
            get
            {
                return (DateTime?)this["TASK_TIME"];
            }
            set
            {
                this["TASK_TIME"] = value;
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
        public DateTime? LASTEDIT
        {
            get
            {
                return (DateTime?)this["LASTEDIT"];
            }
            set
            {
                this["LASTEDIT"] = value;
            }
        }
        public string LASTBY
        {
            get
            {
                return (string)this["LASTBY"];
            }
            set
            {
                this["LASTBY"] = value;
            }
        }
    }
    public class R_RECORD_PALLET_DETAIL
    {
        public string ID { get; set; }
        public string PALLET_NO { get; set; }
        public string SKUNO { get; set; }
        public string SKUVERISON { get; set; }
        public string ACTION_TYPE { get; set; }
        public string EVENTNAME { get; set; }
        public string TASK_FLAG { get; set; }
        public DateTime? TASK_TIME { get; set; }
        public string DATA1 { get; set; }
        public DateTime? LASTEDIT { get; set; }
        public string LASTBY { get; set; }
    }
}