using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CMCHOST_DETAIL : DataObjectTable
    {
        public T_R_CMCHOST_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CMCHOST_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CMCHOST_DETAIL);
            TableName = "R_CMCHOST_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_CMCHOST_DETAIL : DataObjectBase
    {
        public Row_R_CMCHOST_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_CMCHOST_DETAIL GetDataObject()
        {
            R_CMCHOST_DETAIL DataObject = new R_CMCHOST_DETAIL();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CMCTYPE = this.CMCTYPE;
            DataObject.IP = this.IP;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.LINE = this.LINE;
            DataObject.HOST_ID = this.HOST_ID;
            DataObject.ID = this.ID;
            DataObject.AUTO_CONNECT = this.AUTO_CONNECT;
            return DataObject;
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
        public string CMCTYPE
        {
            get
            {
                return (string)this["CMCTYPE"];
            }
            set
            {
                this["CMCTYPE"] = value;
            }
        }
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string HOST_ID
        {
            get
            {
                return (string)this["HOST_ID"];
            }
            set
            {
                this["HOST_ID"] = value;
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
        public string AUTO_CONNECT
        {
            get
            {
                return (string)this["AUTO_CONNECT"];
            }
            set
            {
                this["AUTO_CONNECT"] = value;
            }
        }
    }
    public class R_CMCHOST_DETAIL
    {
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string CMCTYPE { get; set; }
        public string IP { get; set; }
        public string STATION_NAME { get; set; }
        public string LINE { get; set; }
        public string HOST_ID { get; set; }
        public string ID { get; set; }
        public string AUTO_CONNECT { get; set; }
    }
}