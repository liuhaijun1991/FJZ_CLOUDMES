using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPORTCENTER : DataObjectTable
    {
        public T_R_REPORTCENTER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPORTCENTER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPORTCENTER);
            TableName = "R_REPORTCENTER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_REPORTCENTER : DataObjectBase
    {
        public Row_R_REPORTCENTER(DataObjectInfo info) : base(info)
        {

        }
        public R_REPORTCENTER GetDataObject()
        {
            R_REPORTCENTER DataObject = new R_REPORTCENTER();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.DATA = this.DATA;
            DataObject.PARENTKEY = this.PARENTKEY;
            DataObject.DATATYPE = this.DATATYPE;
            DataObject.KEY = this.KEY;
            DataObject.CONFIGTYPE = this.CONFIGTYPE;
            DataObject.ID = this.ID;
            return DataObject;
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
        public string DATA
        {
            get
            {
                return (string)this["DATA"];
            }
            set
            {
                this["DATA"] = value;
            }
        }
        public string PARENTKEY
        {
            get
            {
                return (string)this["PARENTKEY"];
            }
            set
            {
                this["PARENTKEY"] = value;
            }
        }
        public string DATATYPE
        {
            get
            {
                return (string)this["DATATYPE"];
            }
            set
            {
                this["DATATYPE"] = value;
            }
        }
        public string KEY
        {
            get
            {
                return (string)this["KEY"];
            }
            set
            {
                this["KEY"] = value;
            }
        }
        public string CONFIGTYPE
        {
            get
            {
                return (string)this["CONFIGTYPE"];
            }
            set
            {
                this["CONFIGTYPE"] = value;
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
    public class R_REPORTCENTER
    {
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
        public string DATA;
        public string PARENTKEY;
        public string DATATYPE;
        public string KEY;
        public string CONFIGTYPE;
        public string ID;
    }
}