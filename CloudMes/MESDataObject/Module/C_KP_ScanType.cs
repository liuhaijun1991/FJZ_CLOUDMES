using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_ScanType : DataObjectTable
    {
        public T_C_KP_ScanType(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_ScanType(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_ScanType);
            TableName = "C_KP_ScanType".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_KP_ScanType : DataObjectBase
    {
        public Row_C_KP_ScanType(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_ScanType GetDataObject()
        {
            C_KP_ScanType DataObject = new C_KP_ScanType();
            DataObject.ID = this.ID;
            DataObject.TYPENAME = this.TYPENAME;
            DataObject.PARTNO = this.PARTNO;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string TYPENAME
        {
            get
            {
                return (string)this["TYPENAME"];
            }
            set
            {
                this["TYPENAME"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
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
    }
    public class C_KP_ScanType
    {
        public string ID{get;set;}
        public string TYPENAME{get;set;}
        public string PARTNO{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}