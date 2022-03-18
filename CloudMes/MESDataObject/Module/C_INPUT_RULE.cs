using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_INPUT_RULE : DataObjectTable
    {
        public T_C_INPUT_RULE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_INPUT_RULE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_INPUT_RULE);
            TableName = "C_INPUT_RULE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckDataExist(string PageName, string InputName, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM C_INPUT_RULE where page_name='{PageName}' AND INPUT_NAME='{InputName}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public List<C_INPUT_RULE> QueryInputRule(string PageName, string InputName, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_INPUT_RULE> LanguageList = new List<C_INPUT_RULE>();
            sql = $@"SELECT * FROM C_INPUT_RULE where 1=1 ";
            if (PageName != "")
            {
                sql = sql + $@" AND page_name = '{PageName}'";
            }
            if (InputName != "")
            {
                sql = sql + $@" AND INPUT_NAME = '{InputName}'";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }

        public Dictionary<String, String> GetInputRuleList(string PageName, string InputName, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Dictionary<String, String> rulelist = new Dictionary<String, String>();
            sql = $@"SELECT * FROM C_INPUT_RULE where 1=1 ";
            if (PageName != "")
            {
                sql = sql + $@" AND page_name = '{PageName}'";
            }
            if (InputName != "")
            {
                sql = sql + $@" AND INPUT_NAME = '{InputName}'";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                rulelist.Add(dr["INPUT_NAME"].ToString(), dr["EXPRESSION"].ToString());
            }
            return rulelist;
        }
        public C_INPUT_RULE CreateLanguageClass(DataRow dr)
        {
            Row_C_INPUT_RULE row = (Row_C_INPUT_RULE)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }
    }
    public class Row_C_INPUT_RULE : DataObjectBase
    {
        public Row_C_INPUT_RULE(DataObjectInfo info) : base(info)
        {

        }
        public C_INPUT_RULE GetDataObject()
        {
            C_INPUT_RULE DataObject = new C_INPUT_RULE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.PAGE_NAME = this.PAGE_NAME;
            DataObject.INPUT_NAME = this.INPUT_NAME;
            DataObject.EXPRESSION = this.EXPRESSION;
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
        public string SYSTEM_NAME
        {
            get
            {
                return (string)this["SYSTEM_NAME"];
            }
            set
            {
                this["SYSTEM_NAME"] = value;
            }
        }
        public string PAGE_NAME
        {
            get
            {
                return (string)this["PAGE_NAME"];
            }
            set
            {
                this["PAGE_NAME"] = value;
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
        public string EXPRESSION
        {
            get
            {
                return (string)this["EXPRESSION"];
            }
            set
            {
                this["EXPRESSION"] = value;
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
    public class C_INPUT_RULE
    {
        public string ID{get;set;}
        public string SYSTEM_NAME{get;set;}
        public string PAGE_NAME{get;set;}
        public string INPUT_NAME{get;set;}
        public string EXPRESSION{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}