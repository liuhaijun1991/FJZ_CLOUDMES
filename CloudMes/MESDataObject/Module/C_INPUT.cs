using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_Input : DataObjectTable
    {
        public T_C_Input(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_Input(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_Input);
            TableName = "C_Input".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckDataExist(string InputName, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where INPUT_NAME ='{InputName}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public List<C_Input> QueryInput(string ID, string InputName,string DisplayType, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_Input> LanguageList = new List<C_Input>();
            sql = $@"SELECT * FROM C_Input where 1=1 ";
            if (ID != "")
            {
                sql = sql + $@" AND ID = '{ID}'";
            }
            if (InputName != "")
            {
                sql = sql + $@" AND INPUT_NAME = '{InputName}'";
            }

            if (DisplayType != "")
            {
                sql = sql + $@" AND DISPLAY_TYPE = '{DisplayType}'";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateInputClass(dr));
            }
            return LanguageList;
        }
        public C_Input CreateInputClass(DataRow dr)
        {
            Row_C_Input row = (Row_C_Input)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }
    }
    public class Row_C_Input : DataObjectBase
    {
        public Row_C_Input(DataObjectInfo info) : base(info)
        {

        }
        public C_Input GetDataObject()
        {
            C_Input DataObject = new C_Input();
            DataObject.ID = this.ID;
            DataObject.INPUT_NAME = this.INPUT_NAME;
            DataObject.DISPLAY_TYPE = this.DISPLAY_TYPE;
            DataObject.DATA_SOURCE_API = this.DATA_SOURCE_API;
            DataObject.DATA_SOURCE_API_PARA = this.DATA_SOURCE_API_PARA;
            DataObject.REFRESH_TYPE = this.REFRESH_TYPE;
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
        public string INPUT_NAME
        {
            get
            {
                return (string)this["INPUT_NAME"];
            }
            set
            {
                this["INPUT_NAME"] = value;
            }
        }
        public string DISPLAY_TYPE
        {
            get
            {
                return (string)this["DISPLAY_TYPE"];
            }
            set
            {
                this["DISPLAY_TYPE"] = value;
            }
        }
        public string DATA_SOURCE_API
        {
            get
            {
                return (string)this["DATA_SOURCE_API"];
            }
            set
            {
                this["DATA_SOURCE_API"] = value;
            }
        }
        public string DATA_SOURCE_API_PARA
        {
            get
            {
                return (string)this["DATA_SOURCE_API_PARA"];
            }
            set
            {
                this["DATA_SOURCE_API_PARA"] = value;
            }
        }
        public string REFRESH_TYPE
        {
            get
            {
                return (string)this["REFRESH_TYPE"];
            }
            set
            {
                this["REFRESH_TYPE"] = value;
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
    public class C_Input
    {
        public string ID{get;set;}
        public string INPUT_NAME{get;set;}
        public string DISPLAY_TYPE{get;set;}
        public string DATA_SOURCE_API{get;set;}
        public string DATA_SOURCE_API_PARA{get;set;}
        public string REFRESH_TYPE{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}