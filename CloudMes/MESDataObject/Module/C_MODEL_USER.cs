using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_MODEL_USER : DataObjectTable
    {
        public T_C_MODEL_USER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MODEL_USER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MODEL_USER);
            TableName = "C_MODEL_USER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_MODEL_USER : DataObjectBase
    {
        public Row_C_MODEL_USER(DataObjectInfo info) : base(info)
        {

        }
        public C_MODEL_USER GetDataObject()
        {
            C_MODEL_USER DataObject = new C_MODEL_USER();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.TYPE_ID = this.TYPE_ID;
            DataObject.USER_ID = this.USER_ID;
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
        public string TYPE_ID
        {
            get
            {
                return (string)this["TYPE_ID"];
            }
            set
            {
                this["TYPE_ID"] = value;
            }
        }
        public string USER_ID
        {
            get
            {
                return (string)this["USER_ID"];
            }
            set
            {
                this["USER_ID"] = value;
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
    public class C_MODEL_USER
    {
        public string ID{get;set;}
        public string USER_ID{get;set;}
        public string TYPE_ID{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}