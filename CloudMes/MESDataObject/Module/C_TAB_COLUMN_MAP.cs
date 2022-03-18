using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_TAB_COLUMN_MAP : DataObjectTable
    {
        public T_C_TAB_COLUMN_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_TAB_COLUMN_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_TAB_COLUMN_MAP);
            TableName = "C_TAB_COLUMN_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_C_TAB_COLUMN_MAP GetTableColumnMap(string Table_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from C_TAB_COLUMN_MAP where tab_name='{Table_Name}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_TAB_COLUMN_MAP ret = (Row_C_TAB_COLUMN_MAP)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }
    }
    public class Row_C_TAB_COLUMN_MAP : DataObjectBase
    {
        public Row_C_TAB_COLUMN_MAP(DataObjectInfo info) : base(info)
        {

        }
        public C_TAB_COLUMN_MAP GetDataObject()
        {
            C_TAB_COLUMN_MAP DataObject = new C_TAB_COLUMN_MAP();
            DataObject.ID = this.ID;
            DataObject.TAB_NAME = this.TAB_NAME;
            DataObject.TAB_COLUMN = this.TAB_COLUMN;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string TAB_NAME
        {
            get
            {
                return (string)this["TAB_NAME"];
            }
            set
            {
                this["TAB_NAME"] = value;
            }
        }
        public string TAB_COLUMN
        {
            get
            {
                return (string)this["TAB_COLUMN"];
            }
            set
            {
                this["TAB_COLUMN"] = value;
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
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class C_TAB_COLUMN_MAP
    {
        public string ID{get;set;}
        public string TAB_NAME{get;set;}
        public string TAB_COLUMN{get;set;}
        public string DESCRIPTION{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime EDIT_TIME{get;set;}
    }
}