using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_DEPARTMENT : DataObjectTable
    {
        public T_C_DEPARTMENT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_DEPARTMENT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_DEPARTMENT);
            TableName = "C_DEPARTMENT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<String> GetDepartment(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<String> DepartmentList = new List<String>();

            sql = $@"SELECT * FROM  C_DEPARTMENT ORDER BY SEQ_NO";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DepartmentList.Add(dr["DEPARTMENT_NAME"].ToString());

            }
            return DepartmentList;
        }

        public bool CheckDataExist(string DepartmentName, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where department_name ='{DepartmentName}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public List<C_DEPARTMENT> QueryDepartment(string ID, string DepartmentName, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_DEPARTMENT> LanguageList = new List<C_DEPARTMENT>();
            sql = $@"SELECT * FROM {TableName} where 1=1 ";
            if (ID != "")
            {
                sql = sql + $@" AND ID = '{ID}'";
            }

            if (DepartmentName != "")
            {
                sql = sql + $@" AND department_name = '{DepartmentName}'";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateDepartmentClass(dr));
            }
            return LanguageList;
        }
        public C_DEPARTMENT CreateDepartmentClass(DataRow dr)
        {
            Row_C_DEPARTMENT row = (Row_C_DEPARTMENT)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }
    }
    public class Row_C_DEPARTMENT : DataObjectBase
    {
        public Row_C_DEPARTMENT(DataObjectInfo info) : base(info)
        {

        }
        public C_DEPARTMENT GetDataObject()
        {
            C_DEPARTMENT DataObject = new C_DEPARTMENT();
            DataObject.ID = this.ID;
            DataObject.DEPARTMENT_NAME = this.DEPARTMENT_NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.SEQ_NO = this.SEQ_NO;
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
        public string DEPARTMENT_NAME
        {
            get
            {
                return (string)this["DEPARTMENT_NAME"];
            }
            set
            {
                this["DEPARTMENT_NAME"] = value;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
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
    public class C_DEPARTMENT
    {
        public string ID{get;set;}
        public string DEPARTMENT_NAME{get;set;}
        public string DESCRIPTION{get;set;}
        public double? SEQ_NO{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}