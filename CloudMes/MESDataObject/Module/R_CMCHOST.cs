using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CMCHOST : DataObjectTable
    {
        public T_R_CMCHOST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CMCHOST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CMCHOST);
            TableName = "R_CMCHOST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_CMCHOST : DataObjectBase
    {
        public Row_R_CMCHOST(DataObjectInfo info) : base(info)
        {

        }
        public R_CMCHOST GetDataObject()
        {
            R_CMCHOST DataObject = new R_CMCHOST();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.IP = this.IP;
            DataObject.HOST_NAME = this.HOST_NAME;
            DataObject.ID = this.ID;
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
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
        public string HOST_NAME
        {
            get
            {
                return (string)this["HOST_NAME"];
            }
            set
            {
                this["HOST_NAME"] = value;
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
    public class R_CMCHOST
    {
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string DESCRIPTION { get; set; }
        public string IP { get; set; }
        public string HOST_NAME { get; set; }
        public string ID { get; set; }
    }
}