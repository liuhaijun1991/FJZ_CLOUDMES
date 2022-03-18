using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_C_ERROR_CODE : DataObjectTable
    {
        public T_C_ERROR_CODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ERROR_CODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ERROR_CODE);
            TableName = "C_ERROR_CODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_ERROR_CODE GetByErrorCode(string ErrorCode, OleExec DB)
        {
            //string strSql = $@"select * from c_error_code where error_code=:ErrorCode";
            string strSql = $@"select * from c_error_code where error_code='{ErrorCode}'";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":ErrorCode", ErrorCode) };
            //DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, null);
            
            if (res.Rows.Count > 0)
            {
                Row_C_ERROR_CODE ret = (Row_C_ERROR_CODE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public int AddNewErrorCode(C_ERROR_CODE NewErrorCode, OleExec DB)
        {
            Row_C_ERROR_CODE NewErrorCodeRow = (Row_C_ERROR_CODE)NewRow();
            NewErrorCodeRow.ID = NewErrorCode.ID;
            NewErrorCodeRow.ERROR_CODE = NewErrorCode.ERROR_CODE;
            NewErrorCodeRow.ENGLISH_DESCRIPTION = NewErrorCode.ENGLISH_DESC;
            NewErrorCodeRow.CHINESE_DESCRIPTION = NewErrorCode.CHINESE_DESC;
            NewErrorCodeRow.EDIT_EMP = NewErrorCode.EDIT_EMP;
            NewErrorCodeRow.EDIT_TIME = NewErrorCode.EDIT_TIME;
            NewErrorCodeRow.ERROR_CATEGORY = NewErrorCode.ERROR_CATEGORY;
            NewErrorCodeRow.DISC_DEFFECT = NewErrorCode.DISC_DEFFECT;
            int result = DB.ExecuteNonQuery(NewErrorCodeRow.GetInsertString(DBType), CommandType.Text);
            return result;
        }
        public int UpdateById(C_ERROR_CODE NewErrorCode, OleExec DB)
        {
            Row_C_ERROR_CODE NewErrorCodeRow = (Row_C_ERROR_CODE)NewRow();
            NewErrorCodeRow.ID = NewErrorCode.ID;
            NewErrorCodeRow.ERROR_CODE = NewErrorCode.ERROR_CODE;
            NewErrorCodeRow.ENGLISH_DESCRIPTION = NewErrorCode.ENGLISH_DESC;
            NewErrorCodeRow.CHINESE_DESCRIPTION = NewErrorCode.CHINESE_DESC;
            NewErrorCodeRow.EDIT_EMP = NewErrorCode.EDIT_EMP;
            NewErrorCodeRow.EDIT_TIME = NewErrorCode.EDIT_TIME;
            NewErrorCodeRow.ERROR_CATEGORY = NewErrorCode.ERROR_CATEGORY;
            NewErrorCodeRow.DISC_DEFFECT = NewErrorCode.DISC_DEFFECT;

            int result = DB.ExecuteNonQuery(NewErrorCodeRow.GetUpdateString(DBType, NewErrorCode.ID), CommandType.Text);
            return result;
        }
        public C_ERROR_CODE GetByid(string id, OleExec DB)
        {
            string strSql = $@"select * from c_error_code where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_ERROR_CODE ret = (Row_C_ERROR_CODE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public List<C_ERROR_CODE> GetByFuzzySearch(string ParametValue, OleExec DB)
        {
            string strSql = $@"select * from c_error_code where upper(error_code) like'%{ParametValue}%' or upper(english_description) like'%{ParametValue}%' or upper(chinese_description) like'%{ParametValue}%'";
            List<C_ERROR_CODE> result = new List<C_ERROR_CODE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ERROR_CODE ret = (Row_C_ERROR_CODE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }              
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<C_ERROR_CODE> GetAllErrorCode(OleExec DB)
        {
            string strSql = $@"select * from c_error_code ";
            List<C_ERROR_CODE> result = new List<C_ERROR_CODE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ERROR_CODE ret = (Row_C_ERROR_CODE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public int DeleteById(string Id, OleExec DB)
        {
            string strSql = $@"delete c_error_code where id=:Id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }

        public bool CheckErrorCodeByErrorCode(OleExec DB, string ErrorCode)
        {
            bool checkflag;
            string strSql = $@" select count(*) from c_error_code where ERROR_CODE ='{ErrorCode}' ";
            DataTable dt = DB.ExecSelect(strSql).Tables[0];
            int count = Convert.ToInt16(dt.Rows[0][0].ToString());
            if (count > 0)
                checkflag = true;
            else
                checkflag = false;
            return checkflag;
        }

        public C_ERROR_CODE GetCodeByCategory(OleExec SFCDB, string errorCode, List<string> listCategory)
        {
            return SFCDB.ORM.Queryable<C_ERROR_CODE>().Where(r => r.ERROR_CODE == errorCode && listCategory.Contains(r.ERROR_CATEGORY)).ToList().FirstOrDefault();
        }
    }
    public class Row_C_ERROR_CODE : DataObjectBase
    {
        public Row_C_ERROR_CODE(DataObjectInfo info) : base(info)
        {

        }
        public C_ERROR_CODE GetDataObject()
        {
            C_ERROR_CODE DataObject = new C_ERROR_CODE();
            DataObject.ID = this.ID;
            DataObject.ERROR_CODE = this.ERROR_CODE;
            DataObject.ENGLISH_DESC = this.ENGLISH_DESCRIPTION;
            DataObject.CHINESE_DESC = this.CHINESE_DESCRIPTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.ERROR_CATEGORY = this.ERROR_CATEGORY;
            DataObject.DISC_DEFFECT = this.DISC_DEFFECT;
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
        public string ERROR_CODE
        {
            get
            {
                return (string)this["ERROR_CODE"];
            }
            set
            {
                this["ERROR_CODE"] = value;
            }
        }
        public string ENGLISH_DESCRIPTION
        {
            get
            {
                return (string)this["ENGLISH_DESCRIPTION"];
            }
            set
            {
                this["ENGLISH_DESCRIPTION"] = value;
            }
        }
        public string CHINESE_DESCRIPTION
        {
            get
            {
                return (string)this["CHINESE_DESCRIPTION"];
            }
            set
            {
                this["CHINESE_DESCRIPTION"] = value;
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
        public string ERROR_CATEGORY
        {
            get
            {
                return (string)this["ERROR_CATEGORY"];
            }
            set
            {
                this["ERROR_CATEGORY"] = value;
            }
        }
        public string DISC_DEFFECT
        {
            get
            {
                return (string)this["DISC_DEFFECT"];
            }
            set
            {
                this["DISC_DEFFECT"] = value;
            }
        }
    }
    public class C_ERROR_CODE
    {
        public string ID{get;set;}
        public string ERROR_CODE{get;set; }
        [SugarColumn(ColumnName = "ENGLISH_DESCRIPTION")]
        public string ENGLISH_DESC{get;set; }
        [SugarColumn(ColumnName = "CHINESE_DESCRIPTION")]
        public string CHINESE_DESC{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string ERROR_CATEGORY { get; set; }
        public string DISC_DEFFECT { get; set; }

    }
}