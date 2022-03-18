using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SN_DETAIL : DataObjectTable
    {
        public T_R_SN_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_DETAIL);
            TableName = "R_SN_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SN_DETAIL : DataObjectBase
    {
        public Row_R_SN_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_DETAIL GetDataObject()
        {
            R_SN_DETAIL DataObject = new R_SN_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.LINE = this.LINE;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.DEVICE_NAME = this.DEVICE_NAME;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.RESULT_FLAG = this.RESULT_FLAG;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
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
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
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
        public string DEVICE_NAME
        {
            get
            {
                return (string)this["DEVICE_NAME"];
            }
            set
            {
                this["DEVICE_NAME"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string RESULT_FLAG
        {
            get
            {
                return (string)this["RESULT_FLAG"];
            }
            set
            {
                this["RESULT_FLAG"] = value;
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
    public class R_SN_DETAIL
    {
        public string ID;
        public string SN;
        public string SKUNO;
        public string WORKORDERNO;
        public string LINE;
        public string STATION_NAME;
        public string DEVICE_NAME;
        public string CLASS_NAME;
        public string RESULT_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}