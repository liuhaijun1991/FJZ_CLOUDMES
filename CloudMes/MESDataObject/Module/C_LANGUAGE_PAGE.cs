using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_LANGUAGE_PAGE : DataObjectTable
    {
        public T_C_LANGUAGE_PAGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_LANGUAGE_PAGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_LANGUAGE_PAGE);
            TableName = "C_LANGUAGE_PAGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 獲取網頁語言
        /// </summary>
        /// <param name="PageName">頁面名</param>
        /// <param name="LanguageValue">語言列名</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Dictionary<String,String> GetPageLanguage(string PageName, string LanguageValue, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Dictionary<String, String> LanguageList =new  Dictionary<String, String>();

            sql = $@"SELECT label_name,{LanguageValue} LANGUAGE    FROM C_LANGUAGE_PAGE
                  WHERE SYSTEM_NAME = 'MES' AND PAGE_NAME = '{PageName}'
                  UNION
                SELECT label_name,{LanguageValue} LANGUAGE  FROM C_LANGUAGE_PAGE
                  WHERE SYSTEM_NAME = 'MES' AND PAGE_NAME = 'COMMON'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if (!LanguageList.ContainsKey(dr["label_name"].ToString()))
                {
                    LanguageList.Add(dr["label_name"].ToString(), dr["LANGUAGE"].ToString());
                }
            }

            return LanguageList;
        }
        /// <summary>
        /// 查詢整頁面標簽語言或單個標簽語言
        /// </summary>
        /// <param name="PageNameOrLabelName"></param>
        public List<C_LANGUAGE_PAGE> QueryPageLanguage(string PageName,string LabelName,string Chinese_TW, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_LANGUAGE_PAGE> LanguageList = new List<C_LANGUAGE_PAGE>();
            sql = $@"SELECT * FROM c_language_page where 1=1 ";
            if (PageName!="")
            {
               sql=sql + $@" AND page_name = '{PageName}'";
            }
            if (LabelName != "" )
            {
                sql = sql + $@" AND LABEL_NAME = '{LabelName}'";
            }
            if (Chinese_TW != "" )
            {
                sql = sql + $@" AND CHINESE_TW = '{Chinese_TW}'";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }
        public C_LANGUAGE_PAGE CreateLanguageClass(DataRow dr)
        {
            Row_C_LANGUAGE_PAGE row = (Row_C_LANGUAGE_PAGE)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }
        /// <summary>
        /// 檢查數據是否存在
        /// </summary>
        /// <param name="PageName"></param>
        /// <param name="LabelName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckDataExist(string PageName, string LabelName, OleExec DB)
        {
            bool res= false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM c_language_page where page_name='{PageName}' AND LABEL_NAME='{LabelName}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        //public void DeleteLanguage(Array[] ID,OleExec DB)
        //{
        //    string sql = string.Empty;
        //    DataTable dt = new DataTable();
        //    sql = $@"SELECT * FROM c_language_page where ";
        //    dt = DB.ExecSelect(sql).Tables[0];

        //}

        //public String ArrayToSQLString(Array[] ID)
        //{
        //    String StrID = "";
        //    if (ID.Length == 1)
        //    {
        //        StrID = "ID=";
        //        StrID = "'"+ID[0].ToString()+"'";
        //    }
        //    else
        //    {
        //        StrID = "ID IN (";
        //        for (int i = 0; i < ID.Length; i++)
        //        {
        //            StrID = StrID + "'" + ID[i].ToString() + "',";
        //        }
        //        StrID = StrID.Substring(1,StrID.Length-1)+")";
        //    }
        //    return StrID;
        //}



    }
    public class Row_C_LANGUAGE_PAGE : DataObjectBase
    {
        public Row_C_LANGUAGE_PAGE(DataObjectInfo info) : base(info)
        {

        }
        public C_LANGUAGE_PAGE GetDataObject()
        {
            C_LANGUAGE_PAGE DataObject = new C_LANGUAGE_PAGE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.PAGE_NAME = this.PAGE_NAME;
            DataObject.LABEL_NAME = this.LABEL_NAME;
            DataObject.CHINESE = this.CHINESE;
            DataObject.CHINESE_TW = this.CHINESE_TW;
            DataObject.ENGLISH = this.ENGLISH;
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
        public string LABEL_NAME
        {
            get
            {
                return (string)this["LABEL_NAME"];
            }
            set
            {
                this["LABEL_NAME"] = value;
            }
        }
        public string CHINESE
        {
            get
            {
                return (string)this["CHINESE"];
            }
            set
            {
                this["CHINESE"] = value;
            }
        }
        public string CHINESE_TW
        {
            get
            {
                return (string)this["CHINESE_TW"];
            }
            set
            {
                this["CHINESE_TW"] = value;
            }
        }
        public string ENGLISH
        {
            get
            {
                return (string)this["ENGLISH"];
            }
            set
            {
                this["ENGLISH"] = value;
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
    public class C_LANGUAGE_PAGE
    {
        public string ID{get;set;}
        public string SYSTEM_NAME{get;set;}
        public string PAGE_NAME{get;set;}
        public string LABEL_NAME{get;set;}
        public string CHINESE{get;set;}
        public string CHINESE_TW{get;set;}
        public string ENGLISH{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}