using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_NPF_CODE_MAP : DataObjectTable
    {
        public T_C_NPF_CODE_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_NPF_CODE_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_NPF_CODE_MAP);
            TableName = "C_NPF_CODE_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_NPF_CODE_MAP> GetListBYINFO(string INFO, OleExec DB)
        {
            return DB.ORM.Queryable<C_NPF_CODE_MAP>().Where(t => t.NPF_INFO== INFO).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public List<C_NPF_CODE_MAP> GetByErrorCode(string ErrorCode, OleExec DB)
        {
            return DB.ORM.Queryable<C_NPF_CODE_MAP>().Where(t =>t.ERROR_CODE==ErrorCode).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public List<C_NPF_CODE_MAP> GetAllByErrorCode(OleExec DB)
        {
            return DB.ORM.Queryable<C_NPF_CODE_MAP>().OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public List<C_NPF_CODE_MAP> GetByCodeInfo(string eRROR_CODE,string NPF_INFO, OleExec DB)
        {
            return DB.ORM.Queryable<C_NPF_CODE_MAP>().Where(t => t.NPF_INFO == NPF_INFO && t.ERROR_CODE==eRROR_CODE).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public int ADDErrorCode(C_NPF_CODE_MAP newErrorCode, OleExec DB)
        {
            Row_C_NPF_CODE_MAP NewErrorCode = (Row_C_NPF_CODE_MAP)NewRow();
            NewErrorCode.ID = newErrorCode.ID;
            NewErrorCode.ERROR_CODE = newErrorCode.ERROR_CODE;
            NewErrorCode.NPF_INFO = newErrorCode.NPF_INFO;            
            NewErrorCode.EDIT_TIME = newErrorCode.EDIT_TIME;
            NewErrorCode.EDIT_EMP = newErrorCode.EDIT_EMP;
            int result = DB.ExecuteNonQuery(NewErrorCode.GetInsertString(DBType), CommandType.Text);
            return result;
        }

        public int UpdateById(C_NPF_CODE_MAP newErrorCode, OleExec DB)
        {
            Row_C_NPF_CODE_MAP NewErrorCode = (Row_C_NPF_CODE_MAP)NewRow();
            NewErrorCode.ID = newErrorCode.ID;
            NewErrorCode.ERROR_CODE = newErrorCode.ERROR_CODE;
            NewErrorCode.NPF_INFO = newErrorCode.NPF_INFO;
            NewErrorCode.EDIT_TIME = newErrorCode.EDIT_TIME;
            NewErrorCode.EDIT_EMP = newErrorCode.EDIT_EMP;
            int result = DB.ExecuteNonQuery(NewErrorCode.GetUpdateString(DBType, newErrorCode.ID), CommandType.Text);
            return result;
        }

        public string CheckErrorCode(string eRROR_CODE, OleExec DB)
        {         
            T_C_CONTROL C_CONTROL = new T_C_CONTROL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> Control = new List<string>();
            Control= C_CONTROL.GetControlListByName("NPF_ERROR_CODE", DB);
            //string[] ErrorCode = new string[];
            foreach (string item in Control)
            {
                string[] ErrorCode = item.Split(',');
                if (ErrorCode.Contains(eRROR_CODE))
                {
                    return eRROR_CODE;
                }
            }            
            return null;
        }

        public int DeleteById(string Id, OleExec DB)
        {
            string strSql = $@"DELETE C_NPF_CODE_MAP WHERE ID=:ID";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
    }
    public class Row_C_NPF_CODE_MAP : DataObjectBase
    {
        public Row_C_NPF_CODE_MAP(DataObjectInfo info) : base(info)
        {

        }
        public C_NPF_CODE_MAP GetDataObject()
        {
            C_NPF_CODE_MAP DataObject = new C_NPF_CODE_MAP();
            DataObject.ID = this.ID;
            DataObject.ERROR_CODE = this.ERROR_CODE;
            DataObject.NPF_INFO = this.NPF_INFO;
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
        public string NPF_INFO
        {
            get
            {
                return (string)this["NPF_INFO"];
            }
            set
            {
                this["NPF_INFO"] = value;
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
    public class C_NPF_CODE_MAP
    {
        public string ID { get; set; }
        public string ERROR_CODE { get; set; }
        public string NPF_INFO { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}