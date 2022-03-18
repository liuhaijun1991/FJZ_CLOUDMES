using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP_FILE : DataObjectTable
    {
        public T_R_SAP_FILE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_FILE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_FILE);
            TableName = "R_SAP_FILE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_FILE : DataObjectBase
    {
        public Row_R_SAP_FILE(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_FILE GetDataObject()
        {
            R_SAP_FILE DataObject = new R_SAP_FILE();
            DataObject.ID = this.ID;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.LOCAL_FILE_PATH = this.LOCAL_FILE_PATH;
            DataObject.REMOTE_FILE_PATH = this.REMOTE_FILE_PATH;
            DataObject.TYPE = this.TYPE;
            DataObject.DETAIL_TABLE = this.DETAIL_TABLE;
            DataObject.ANALYSIS_FLAG = this.ANALYSIS_FLAG;
            DataObject.DATA_TIME = this.DATA_TIME;
            DataObject.DATA_KEY = this.DATA_KEY;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CREATE_NAME = this.CREATE_NAME;
            DataObject.MEMO = this.MEMO;
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
        public string FILE_NAME
        {
            get
            {
                return (string)this["FILE_NAME"];
            }
            set
            {
                this["FILE_NAME"] = value;
            }
        }
        public string LOCAL_FILE_PATH
        {
            get
            {
                return (string)this["LOCAL_FILE_PATH"];
            }
            set
            {
                this["LOCAL_FILE_PATH"] = value;
            }
        }
        public string REMOTE_FILE_PATH
        {
            get
            {
                return (string)this["REMOTE_FILE_PATH"];
            }
            set
            {
                this["REMOTE_FILE_PATH"] = value;
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
        public string DETAIL_TABLE
        {
            get
            {
                return (string)this["DETAIL_TABLE"];
            }
            set
            {
                this["DETAIL_TABLE"] = value;
            }
        }
        public string ANALYSIS_FLAG
        {
            get
            {
                return (string)this["ANALYSIS_FLAG"];
            }
            set
            {
                this["ANALYSIS_FLAG"] = value;
            }
        }
        public DateTime? DATA_TIME
        {
            get
            {
                return (DateTime?)this["DATA_TIME"];
            }
            set
            {
                this["DATA_TIME"] = value;
            }
        }
        public string DATA_KEY
        {
            get
            {
                return (string)this["DATA_KEY"];
            }
            set
            {
                this["DATA_KEY"] = value;
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
        public string CREATE_NAME
        {
            get
            {
                return (string)this["CREATE_NAME"];
            }
            set
            {
                this["CREATE_NAME"] = value;
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
    }
    public class R_SAP_FILE
    {
        public string ID { get; set; }
        public string FILE_NAME { get; set; }
        public string LOCAL_FILE_PATH { get; set; }
        public string REMOTE_FILE_PATH { get; set; }
        public string TYPE { get; set; }
        public string DETAIL_TABLE { get; set; }
        public string ANALYSIS_FLAG { get; set; }
        public DateTime? DATA_TIME { get; set; }
        public string DATA_KEY { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string CREATE_NAME { get; set; }
        public string MEMO { get; set; }
    }
}