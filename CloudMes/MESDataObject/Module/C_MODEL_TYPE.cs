using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_MODEL_TYPE : DataObjectTable
    {
        public T_C_MODEL_TYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MODEL_TYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MODEL_TYPE);
            TableName = "C_MODEL_TYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 查詢該用戶已有的ModelType權限
        /// </summary>
        public List<ModelTypePrivilege> GetPrivilegeForUser(string LoginUserEmp, string EditEmp, string EmpLevel, MESDBHelper.OleExec SFCDB)
        {
            string sql = string.Empty;
            sql = $@"Select b.id,a.type_no, a.department,a.model_name,a.program_name,a.description,b.EDIT_EMP,b.EDIT_TIME 
                     From sfcbase.c_model_type a,sfcbase.c_model_user b,c_user c 
                     Where a.id =b.type_id and b.user_id=c.id and emp_no='{EditEmp}'";
            DataSet res = SFCDB.ExecSelect(sql);
            List<ModelTypePrivilege> list = new List<ModelTypePrivilege>();
            if (res.Tables[0].Rows.Count > 0)
            {
                foreach(DataRow dr in res.Tables[0].Rows)
                {
                    if (!(dr["EDIT_TIME"] is System.DBNull))
                    {
                        list.Add(new ModelTypePrivilege
                        {
                            ID = dr["ID"].ToString(),
                            TYPE_NO = dr["TYPE_NO"].ToString(),
                            DEPARTMENT = dr["DEPARTMENT"].ToString(),
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            PROGRAM_NAME = dr["PROGRAM_NAME"].ToString(),
                            DESCRIPTION = dr["DESCRIPTION"].ToString(),
                            EDIT_EMP = dr["EDIT_EMP"].ToString(),
                            EDIT_TIME = (DateTime)dr["EDIT_TIME"]
                        });
                    }
                    else
                    {
                        list.Add(new ModelTypePrivilege
                        {
                            ID = dr["ID"].ToString(),
                            TYPE_NO = dr["TYPE_NO"].ToString(),
                            DEPARTMENT = dr["DEPARTMENT"].ToString(),
                            MODEL_NAME = dr["MODEL_NAME"].ToString(),
                            PROGRAM_NAME = dr["PROGRAM_NAME"].ToString(),
                            DESCRIPTION = dr["DESCRIPTION"].ToString(),
                            EDIT_EMP = dr["EDIT_EMP"].ToString(),
                            EDIT_TIME = null
                        });
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 從登陸用戶有的權限列表中,只取受分配員工沒有權限的列表
        /// </summary>
        public List<ModelTypePrivilege> GetModelTypeForUser(string LoginUserEmp, string EditEmp, string EmpLevel, MESDBHelper.OleExec SFCDB)
        {
            string sql = string.Empty;
            if (EmpLevel == "9")
            {
                sql = $@"select id,type_no,department,model_name,program_name,description 
                         from sfcbase.c_model_type a 
                         where a.id not in (select type_id from sfcbase.c_model_user b,c_user c 
                                            where b.user_id=c.id and emp_no = '{EditEmp}')  ";
            }
            else
            {
                sql = $@"select a.id,a.type_no,a.department,a.model_name,a.program_name,a.description 
                       from sfcbase.c_model_type a,sfcbase.c_model_user d,c_user e 
                       where d.type_id=a.id and d.USER_ID=e.ID and e.emp_no='{LoginUserEmp}' and 
                             a.id not in (select type_id from sfcbase.c_model_user b,c_user c 
                                          where b.user_id=c.id and emp_no like  '{EditEmp}')  ";
            }
            DataSet res = SFCDB.ExecSelect(sql);
            List<ModelTypePrivilege> list = new List<ModelTypePrivilege>();
            if (res.Tables[0].Rows.Count > 0) { 
                foreach(DataRow dr in res.Tables[0].Rows)
                {
                    list.Add(new ModelTypePrivilege
                    {
                        ID = dr["ID"].ToString(),
                        TYPE_NO = dr["TYPE_NO"].ToString(),
                        DEPARTMENT = dr["DEPARTMENT"].ToString(),
                        MODEL_NAME = dr["MODEL_NAME"].ToString(),
                        PROGRAM_NAME = dr["PROGRAM_NAME"].ToString(),
                        DESCRIPTION=dr["DESCRIPTION"].ToString()
                    });
                }
            }
            return list;
        }
    }
    public class Row_C_MODEL_TYPE : DataObjectBase
    {
        public Row_C_MODEL_TYPE(DataObjectInfo info) : base(info)
        {

        }
        public C_MODEL_TYPE GetDataObject()
        {
            C_MODEL_TYPE DataObject = new C_MODEL_TYPE();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.PROGRAM_NAME = this.PROGRAM_NAME;
            DataObject.MODEL_NAME = this.MODEL_NAME;
            DataObject.DEPARTMENT = this.DEPARTMENT;
            DataObject.TYPE_NO = this.TYPE_NO;
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
        public string PROGRAM_NAME
        {
            get
            {
                return (string)this["PROGRAM_NAME"];
            }
            set
            {
                this["PROGRAM_NAME"] = value;
            }
        }
        public string MODEL_NAME
        {
            get
            {
                return (string)this["MODEL_NAME"];
            }
            set
            {
                this["MODEL_NAME"] = value;
            }
        }
        public string DEPARTMENT
        {
            get
            {
                return (string)this["DEPARTMENT"];
            }
            set
            {
                this["DEPARTMENT"] = value;
            }
        }
        public string TYPE_NO
        {
            get
            {
                return (string)this["TYPE_NO"];
            }
            set
            {
                this["TYPE_NO"] = value;
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
    public class C_MODEL_TYPE
    {
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string DESCRIPTION{get;set;}
        public string PROGRAM_NAME{get;set;}
        public string MODEL_NAME{get;set;}
        public string DEPARTMENT{get;set;}
        public string TYPE_NO{get;set;}
        public string ID{get;set;}
    }

    public class ModelTypePrivilege
    {
        public string ID{get;set;}
        public string TYPE_NO{get;set;}
        public string DEPARTMENT{get;set;}
        public string MODEL_NAME{get;set;}
        public string PROGRAM_NAME{get;set;}
        public string DESCRIPTION{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}