using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_LANGUAGE : DataObjectTable
    {
        public T_C_LANGUAGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_LANGUAGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_LANGUAGE);
            TableName = "C_LANGUAGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 獲取網站語言種類
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetLanguageType(OleExec DB)
        {
            //string sql = string.Empty;
            //DataTable dt = new DataTable();
            //Dictionary<String, String> LanguageList = new Dictionary<String, String>();

            //sql = $@"select * from C_LANGUAGE ORDER BY SORT";
            //dt = DB.ExecSelect(sql).Tables[0];
            //foreach (DataRow dr in dt.Rows)
            //{
            //    LanguageList.Add(dr["LANGUAGE_NAME"].ToString(), dr["LANGUAGE_VALUE"].ToString());

            //}

            Dictionary<string,string> languages = DB.ORM.Queryable<C_LANGUAGE>().Select<KeyValuePair<string, string>>("language_name,language_value").ToList().ToDictionary(q => q.Key, q => q.Value);
            return languages;
            //return LanguageList;
        }


    }
    public class Row_C_LANGUAGE : DataObjectBase
    {
        public Row_C_LANGUAGE(DataObjectInfo info) : base(info)
        {

        }
        public C_LANGUAGE GetDataObject()
        {
            C_LANGUAGE DataObject = new C_LANGUAGE();
            DataObject.ID = this.ID;
            DataObject.LANGUAGE_NAME = this.LANGUAGE_NAME;
            DataObject.LANGUAGE_VALUE = this.LANGUAGE_VALUE;
            DataObject.SORT = this.SORT;
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
        public string LANGUAGE_NAME
        {
            get
            {
                return (string)this["LANGUAGE_NAME"];
            }
            set
            {
                this["LANGUAGE_NAME"] = value;
            }
        }
        public string LANGUAGE_VALUE
        {
            get
            {
                return (string)this["LANGUAGE_VALUE"];
            }
            set
            {
                this["LANGUAGE_VALUE"] = value;
            }
        }
        public double? SORT
        {
            get
            {
                return (double?)this["SORT"];
            }
            set
            {
                this["SORT"] = value;
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
    public class C_LANGUAGE
    {
        public string ID{get;set;}
        public string LANGUAGE_NAME{get;set;}
        public string LANGUAGE_VALUE{get;set;}
        public double? SORT{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}