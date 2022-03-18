using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module.Juniper
{
    public class T_R_DISC_HEAD : DataObjectTable
    {
        public T_R_DISC_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DISC_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DISC_HEAD);
            TableName = "R_DISC_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_DISC_HEAD : DataObjectBase
    {
        public Row_R_DISC_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_DISC_HEAD GetDataObject()
        {
            R_DISC_HEAD DataObject = new R_DISC_HEAD();
            DataObject.ID = this.ID;
            DataObject.DISC_KEY = this.DISC_KEY;
            DataObject.DISC_TYPE = this.DISC_TYPE;
            DataObject.DISC_FILE = this.DISC_FILE;
            DataObject.COLLECT_FLAG = this.COLLECT_FLAG;
            DataObject.CONVERT_FLAG = this.CONVERT_FLAG;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.CREATE_TIME = this.CREATE_TIME;
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
        public string DISC_KEY
        {
            get
            {
                return (string)this["DISC_KEY"];
            }
            set
            {
                this["DISC_KEY"] = value;
            }
        }
        public string DISC_TYPE
        {
            get
            {
                return (string)this["DISC_TYPE"];
            }
            set
            {
                this["DISC_TYPE"] = value;
            }
        }
        public string DISC_FILE
        {
            get
            {
                return (string)this["DISC_FILE"];
            }
            set
            {
                this["DISC_FILE"] = value;
            }
        }
        public string COLLECT_FLAG
        {
            get
            {
                return (string)this["COLLECT_FLAG"];
            }
            set
            {
                this["COLLECT_FLAG"] = value;
            }
        }
        public string CONVERT_FLAG
        {
            get
            {
                return (string)this["CONVERT_FLAG"];
            }
            set
            {
                this["CONVERT_FLAG"] = value;
            }
        }
        public string SEND_FLAG
        {
            get
            {
                return (string)this["SEND_FLAG"];
            }
            set
            {
                this["SEND_FLAG"] = value;
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
    public class R_DISC_HEAD
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string DISC_KEY { get; set; }
        public string DISC_TYPE { get; set; }
        public string DISC_FILE { get; set; }
        public string COLLECT_FLAG { get; set; }
        public string CONVERT_FLAG { get; set; }
        public string SEND_FLAG { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public double? VALID_FLAG { get; set; }
        public string PLANT { get; set; }
    }
}