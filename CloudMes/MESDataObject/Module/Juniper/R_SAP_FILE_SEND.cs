using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP_FILE_SEND : DataObjectTable
    {
        public T_R_SAP_FILE_SEND(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_FILE_SEND(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_FILE_SEND);
            TableName = "R_SAP_FILE_SEND".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_FILE_SEND : DataObjectBase
    {
        public Row_R_SAP_FILE_SEND(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_FILE_SEND GetDataObject()
        {
            R_SAP_FILE_SEND DataObject = new R_SAP_FILE_SEND();
            DataObject.ID = this.ID;
            DataObject.FILE_ID = this.FILE_ID;
            DataObject.GET_DATA = this.GET_DATA;
            DataObject.GET_TIME = this.GET_TIME;
            DataObject.LOCAL_FILE_PATH = this.LOCAL_FILE_PATH;
            DataObject.SEND_DATA = this.SEND_DATA;
            DataObject.SEND_TIME = this.SEND_TIME;
            DataObject.REMOTE_FILE_PATH = this.REMOTE_FILE_PATH;
            DataObject.TYPE = this.TYPE;
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
        public string FILE_ID
        {
            get
            {
                return (string)this["FILE_ID"];
            }
            set
            {
                this["FILE_ID"] = value;
            }
        }
        public string GET_DATA
        {
            get
            {
                return (string)this["GET_DATA"];
            }
            set
            {
                this["GET_DATA"] = value;
            }
        }
        public DateTime? GET_TIME
        {
            get
            {
                return (DateTime?)this["GET_TIME"];
            }
            set
            {
                this["GET_TIME"] = value;
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
        public string SEND_DATA
        {
            get
            {
                return (string)this["SEND_DATA"];
            }
            set
            {
                this["SEND_DATA"] = value;
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
    public class R_SAP_FILE_SEND
    {
        public string ID { get; set; }
        public string FILE_ID { get; set; }
        public string GET_DATA { get; set; }
        public DateTime? GET_TIME { get; set; }
        public string LOCAL_FILE_PATH { get; set; }
        public string SEND_DATA { get; set; }
        public DateTime? SEND_TIME { get; set; }
        public string REMOTE_FILE_PATH { get; set; }
        public string TYPE { get; set; }
        public string MEMO { get; set; }
    }
}