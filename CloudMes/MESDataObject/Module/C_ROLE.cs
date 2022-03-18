using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESDataObject.Module
{
    public class T_C_ROLE : DataObjectTable
    {
        public T_C_ROLE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ROLE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROLE);
            TableName = "C_ROLE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public DataTable getC_Rolebyrolename(string Role_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql;
            if (Role_Name == "")
            {
                strSql = $@"select * from c_role  ";
            }
            else
            {
                strSql = $@"select * from c_role where Role_Name like '%{Role_Name}%' ";
            }
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;
        }

        public Row_C_ROLE SELECTC_Rolebyrolename(string Role_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql;
            if (Role_Name == "")
            {
                strSql = $@"select * from c_role  ";

            }
            else
            {
                strSql = $@"select * from c_role where Role_Name  like '%{Role_Name}%' ";
            }

            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_ROLE ret = (Row_C_ROLE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<C_ROLE> Getrolelist(string ROLE_NAME, string EmpLevel, string DptName, OleExec DB)
        {
            string sql = string.Empty;
            string Strsql = string.Empty;
            DataTable dt = new DataTable();
            List<C_ROLE> rolelist = new List<C_ROLE>();

            if (ROLE_NAME.Length == 0)
            {
                if (EmpLevel == "9")//9级用户可以拿到整个表的角色
                {
                    rolelist = DB.ORM.Queryable<C_ROLE>().OrderBy(t => t.ROLE_TYPE).OrderBy(t => t.ROLE_NAME).ToList();
                }

                if (EmpLevel == "1")//1级用户只可以拿到自己部门相关的角色
                {
                    rolelist = DB.ORM.Queryable<C_ROLE>().Where((t) => t.ROLE_TYPE.Contains(DptName)).OrderBy(t => t.ROLE_TYPE).OrderBy(t => t.ROLE_NAME).ToList();
                }
            }
            else
            {
                rolelist = DB.ORM.Queryable<C_ROLE>().Where((t) => t.ROLE_NAME.Contains(ROLE_NAME)).OrderBy(t => t.ROLE_TYPE).OrderBy(t => t.ROLE_NAME).ToList();
            }
            return rolelist;
        }

        /// <summary>
        /// /   两个等级用户都只能管理自己拥有的角色，不拥有的角色是不能管理的
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EmpLevel"></param>
        /// <param name="DptName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_ROLE> GetUserRolelist(string UserID, bool LoginTrue, string EmpLevel, string DptName, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_ROLE> rolelist = new List<C_ROLE>();
            if (EmpLevel == "9")//9级用户可以拿到整个表该管理员拥有的角色
            {
                if (LoginTrue)
                {
                    rolelist = DB.ORM.Queryable<C_ROLE>().OrderBy(t => t.ROLE_TYPE).OrderBy(t => t.ROLE_NAME).ToList();
                }
                else
                {
                    rolelist = DB.ORM.Queryable<C_ROLE, C_USER_ROLE>((A, B) => new object[] { SqlSugar.JoinType.Left, A.ID == B.ROLE_ID })
                    .Where((A, B) => B.USER_ID == UserID)
                    .OrderBy(A => A.ROLE_TYPE)
                    .OrderBy(A => A.ROLE_NAME)
                    .Select((A, B) => A)
                    .ToList();
                }
            }

            if (EmpLevel == "1")//1级用户只可以拿到自己拥有的角色         
            {
                rolelist = DB.ORM.Queryable<C_ROLE, C_USER_ROLE>((A, B) => new object[] { SqlSugar.JoinType.Left, A.ID == B.ROLE_ID })
                .Where((A, B) => B.USER_ID == UserID && A.ROLE_TYPE == DptName)
                .OrderBy(A => A.ROLE_TYPE)
                .OrderBy(A => A.ROLE_NAME)
                .Select((A, B) => A)
                .ToList();
            }
            return rolelist;
        }

        /// <summary>
        /// 獲取當前用戶可管理的角色
        /// </summary>
        /// <param name="CurrentUser"></param>
        /// <param name="DeptName"></param>
        /// <param name="BuName"></param>
        /// <param name="Factory"></param>
        /// <param name="EmpLevel"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<c_role_byempl> GetManagableRole(string CurrentUser, string DeptName, string BuName, string Factory, string EmpLevel, OleExec DB)
        {

            List<c_role_byempl> ManagableRoles = new List<c_role_byempl>();
            ManagableRoles = DB.ORM.Queryable<C_ROLE>()
                    .Where(t => SqlSugar.SqlFunc.Subqueryable<C_USER>().Where(u => u.BU_NAME == BuName && u.FACTORY == Factory).Any())
                    .Select(r => new c_role_byempl()
                    {
                        ID = r.ID,
                        ROLE_DESC = r.ROLE_DESC,
                        ROLE_NAME = r.ROLE_NAME,
                        BU_NAME = BuName,
                        DPT_NAME = DeptName,
                        FACTORY = Factory
                    }).ToList();
            if (EmpLevel == "9")
            {
                return ManagableRoles;
            }
            else if (EmpLevel == "1")
            {
                return ManagableRoles.Where(r => r.DPT_NAME == DeptName).ToList();
            }
            else if (EmpLevel == "0")
            {
                return DB.ORM.Queryable<C_ROLE, C_USER_ROLE, C_USER>((r, ur, u) => r.ID == ur.ROLE_ID && ur.USER_ID == u.ID)
                    .Where((r, ur, u) => u.EMP_NO == CurrentUser && ur.OPERATE_FLAG == "1")
                    .Select((r, ur, u) => new c_role_byempl()
                    {
                        ID = r.ID,
                        ROLE_DESC = r.ROLE_DESC,
                        ROLE_NAME = r.ROLE_NAME,
                        BU_NAME = BuName,
                        DPT_NAME = DeptName,
                        FACTORY = Factory
                    }).ToList();

            }
            return ManagableRoles;

        }


        public List<c_role_byempl> ManageRoleByUser(List<get_c_roleid> ROLE_ID, string DPT_NAME, string BU_NAME, string FACTORY, string EMP_LEVEL, OleExec DB)
        {
            DataTable dt = new DataTable();
            List<c_role_byempl> rolelist = new List<c_role_byempl>();
            List<string> ExcludeRoles = ROLE_ID.Select(t => t.ROLE_ID).ToList();
            if (EMP_LEVEL == "9")
            {
                //9级账户能管理相同厂别和BU下的所有角色
                rolelist = DB.ORM.Queryable<C_ROLE>()
                    .Where(t => !ExcludeRoles.Contains(t.ID))
                    .OrderBy(t => t.ROLE_TYPE)
                    .OrderBy(t => t.ROLE_NAME)
                    .Select(t => new c_role_byempl { ID = t.ID, ROLE_NAME = t.ROLE_NAME, BU_NAME = BU_NAME, DPT_NAME = DPT_NAME, FACTORY = FACTORY, ROLE_DESC = t.ROLE_DESC })
                    .ToList();
            }
            else
            {
                rolelist = DB.ORM.Queryable<C_ROLE>()
                    .Where(R => !ExcludeRoles.Contains(R.ID) && (R.ROLE_TYPE == DPT_NAME || R.ROLE_TYPE.Contains(DPT_NAME) || R.ROLE_TYPE == "ALL"))
                    .OrderBy(R => R.ROLE_TYPE)
                    .OrderBy(R => R.ROLE_NAME)
                    .Select(R => new c_role_byempl { ID = R.ID, ROLE_NAME = R.ROLE_NAME, BU_NAME = BU_NAME, DPT_NAME = DPT_NAME, FACTORY = FACTORY, ROLE_DESC = R.ROLE_DESC })
                    .ToList();
            }
            return rolelist;
        }

        public bool CheckRole(string ROLE_ID, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@" SELECT *FROM C_USER_ROLE WHERE ROLE_ID ='{ROLE_ID}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public bool CheckRoleData(string RoleName, string Role_type, string EMP_LEVEL, string DPT_NAME, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            //     if ((EMP_LEVEL == "1" && DPT_NAME == Role_type) || (EMP_LEVEL == "9"))
            if ((EMP_LEVEL == "1") || (EMP_LEVEL == "9"))
            {
                if (RoleName.Length != 0)
                {
                    sql = $@"SELECT * FROM c_role where role_name='{RoleName}'";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        res = true;
                    }
                }
            }
            return res;

        }
    }
    public class c_role1
    {
        public c_role1()
        {

        }
        public string ID { get; set; }
        public string ROLE_TYPE { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string EDIT_EMP { get; set; }


    }

    public class c_role_byempl
    {
        public string ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string FACTORY { get; set; }
        public string BU_NAME { get; set; }
        public string DPT_NAME { get; set; }
    }
    public class Row_C_ROLE : DataObjectBase
    {
        public Row_C_ROLE(DataObjectInfo info) : base(info)
        {

        }
        public C_ROLE GetDataObject()
        {
            C_ROLE DataObject = new C_ROLE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.ROLE_NAME = this.ROLE_NAME;
            DataObject.ROLE_DESC = this.ROLE_DESC;
            DataObject.ROLE_TYPE = this.ROLE_DESC;
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
        public string ROLE_NAME
        {
            get
            {
                return (string)this["ROLE_NAME"];
            }
            set
            {
                this["ROLE_NAME"] = value;
            }
        }
        public string ROLE_DESC
        {
            get
            {
                return (string)this["ROLE_DESC"];
            }
            set
            {
                this["ROLE_DESC"] = value;
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

        public string ROLE_TYPE
        {
            get
            {
                return (string)this["ROLE_TYPE"];
            }
            set
            {
                this["ROLE_TYPE"] = value;
            }
        }
    }

    public class C_ROLE
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string ROLE_TYPE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string ROLE_DESC { get; set; }
        public string ROLE_NAME { get; set; }
        public string SYSTEM_NAME { get; set; }
    }
}