using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_c_user_role : DataObjectTable
    {
        public T_c_user_role(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_c_user_role(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_c_user_role);
            TableName = "c_user_role".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckUserRole(string UserId, string RoleId, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@" SELECT *FROM C_USER_ROLE WHERE USER_ID='{UserId}' AND ROLE_ID ='{RoleId}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public string GetUserID(string EmpNo, OleExec DB)
        {
            string res = null;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@" SELECT * FROM  C_USER  WHERE EMP_NO='{EmpNo}'  ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count != 0)
            {
                res = dt.Rows[0]["ID"].ToString();
            }

            return res;
        }

        public List<get_c_roleid> GetRoleID(string EMP_NO, OleExec DB)
        {            
            string sql = string.Empty;
            String USERID = GetUserID(EMP_NO,DB);
            List <get_c_roleid> GetRoleIDList = new List<get_c_roleid>();
            DataTable dt = new DataTable();

            sql = $@" SELECT * FROM  C_USER_ROLE  WHERE USER_ID='{USERID}'  ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    GetRoleIDList.Add(new get_c_roleid
                    {
                        USER_ID=item["USER_ID"].ToString(),
                        ROLE_ID=item["ROLE_ID"].ToString()

                    });
                }
            }

            return GetRoleIDList;
        }


        public List<c_load_userrole> QueryUserRoleInfo(string Emp_No,String Dpt_Name,string Bu_Name,string Factory,String EmpLevel, OleExec DB)
        {
            //string sql = string.Empty;
            //string strsql = string.Empty;
            DataTable dt = new DataTable();
            List<c_load_userrole> UserRoleInfoList = new List<c_load_userrole>();

            dt = DB.ORM.Queryable<C_USER, C_USER_ROLE, C_ROLE>((u, ur, r) => new object[] { JoinType.Left, u.ID == ur.USER_ID, JoinType.Left, r.ID == ur.ROLE_ID })
                    .WhereIF(Emp_No.Length != 0, u => u.EMP_NO == Emp_No)
                    .WhereIF(Emp_No.Length == 0, u => u.BU_NAME == Bu_Name && u.FACTORY == Factory)
                    .WhereIF(EmpLevel != "9", u => u.DPT_NAME == Dpt_Name)
                    .Select("factory,bu_name,emp_no,emp_name,dpt_name,to_char(wm_concat(role_name))  role_name")
                    .GroupBy("factory,bu_name,emp_no,emp_name,dpt_name").ToDataTable();
            //if (Emp_No.Length != 0)
            //{

            //    dt = DB.ORM.Queryable<C_USER, C_USER_ROLE, C_ROLE>((u, ur, r) => new object[] { JoinType.Left, u.ID == ur.USER_ID, r.ID == ur.ROLE_ID })
            //        .Where(u => u.EMP_NO == Emp_No)
            //        .Select("factory,bu_name,emp_no,emp_name,dpt_name,to_char(wm_concat(role_name)) as role_name")
            //        .ToDataTable();
            //    //sql = $@" SELECT A.FACTORY,
            //    //                 A.BU_NAME,
            //    //                 A.EMP_NO,
            //    //                 A.EMP_NAME,
            //    //                 A.DPT_NAME,
            //    //                 TO_CHAR (WM_CONCAT (C.ROLE_NAME)) AS ROLE_NAME
            //    //            FROM C_USER A, C_USER_ROLE B, C_ROLE C
            //    //           WHERE C.ID = B.ROLE_ID AND A.ID = B.USER_ID AND A.EMP_NO='{Emp_No}'
            //    //        GROUP BY A.FACTORY,
            //    //                 A.BU_NAME,
            //    //                 A.EMP_NO,
            //    //                 A.EMP_NAME,
            //    //                 A.DPT_NAME
            //    //        UNION
            //    //        SELECT A.FACTORY,
            //    //               A.BU_NAME,
            //    //               A.EMP_NO,
            //    //               A.EMP_NAME,
            //    //               A.DPT_NAME,
            //    //               NULL AS ROLE_NAME
            //    //          FROM C_USER A
            //    //         WHERE NOT EXISTS
            //    //                  (SELECT 1
            //    //                     FROM C_USER_ROLE B, C_ROLE C
            //    //                    WHERE B.ROLE_ID = C.ID AND B.USER_ID = A.ID  ) AND A.EMP_NO='{Emp_No}'";
            //}
            //else
            //{
            //    //if (EmpLevel!="9")
            //    //{
            //    //    strsql = $@" AND A.DPT_NAME='{Dpt_Name}'";
            //    //}

            //    dt = DB.ORM.Queryable<C_USER, C_USER_ROLE, C_ROLE>((u, ur, r) => new object[] { JoinType.Left, u.ID == ur.USER_ID,JoinType.Inner, r.ID == ur.ROLE_ID })
            //        .WhereIF(Emp_No.Length!=0,u=>u.EMP_NO==Emp_No)
            //        .WhereIF(Emp_No.Length==0,u=> u.BU_NAME == Bu_Name && u.FACTORY == Factory)
            //        .WhereIF(EmpLevel!="9",u=>u.DPT_NAME==Dpt_Name)
            //        .Select("factory,bu_name,emp_no,emp_name,dpt_name,to_char(wm_concat(role_name)) as role_name")
            //        .GroupBy(u => new object[] { u.FACTORY, u.BU_NAME, u.EMP_NO, u.EMP_NAME, u.DPT_NAME }).ToDataTable();

            //    //sql = $@" SELECT A.FACTORY,
            //    //                 A.BU_NAME,
            //    //                 A.EMP_NO,
            //    //                 A.EMP_NAME,
            //    //                 A.DPT_NAME,
            //    //                 TO_CHAR (WM_CONCAT (C.ROLE_NAME)) AS ROLE_NAME
            //    //            FROM C_USER A, C_USER_ROLE B, C_ROLE C
            //    //           WHERE C.ID = B.ROLE_ID AND A.ID = B.USER_ID {strsql} AND A.BU_NAME='{Bu_Name}' AND A.FACTORY='{Factory}'
            //    //        GROUP BY A.FACTORY,
            //    //                 A.BU_NAME,
            //    //                 A.EMP_NO,
            //    //                 A.EMP_NAME,
            //    //                 A.DPT_NAME
            //    //        UNION
            //    //        SELECT A.FACTORY,
            //    //               A.BU_NAME,
            //    //               A.EMP_NO,
            //    //               A.EMP_NAME,
            //    //               A.DPT_NAME,
            //    //               NULL AS ROLE_NAME
            //    //          FROM C_USER A
            //    //         WHERE NOT EXISTS
            //    //                  (SELECT 1
            //    //                     FROM C_USER_ROLE B, C_ROLE C
            //    //                    WHERE B.ROLE_ID = C.ID AND B.USER_ID = A.ID)  {strsql}  AND A.BU_NAME='{Bu_Name}' AND A.FACTORY='{Factory}' ";
            //}

            //dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                UserRoleInfoList.Add(new c_load_userrole
                {
                    FACTORY=item["FACTORY"].ToString(),
                    BU_NAME=item["BU_NAME"].ToString(),
                    EMP_NO=item["EMP_NO"].ToString(),
                    EMP_NAME=item["EMP_NAME"].ToString(),
                    DPT_NAME=item["DPT_NAME"].ToString(),
                    ROLE_NAME=item["ROLE_NAME"].ToString()
                });
            }
            return UserRoleInfoList;
        }

        /// <summary>
        /// 給用戶添加角色，并且根據前端選擇是否可以編輯角色將 C_USER 中的 EMP_LEVEL 值改變（改變 C_USER 中的 EMP_LEVEL 我覺得沒必要）
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <param name="RoleId"></param>
        /// <param name="LevelFlag"></param>
        /// <param name="BuName"></param>
        /// <param name="UserName"></param>
        /// <param name="SystemName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int Add(string EmpNo, string RoleId, string LevelFlag, string BuName, string UserName, string SystemName, OleExec DB)
        {
            List<C_USER> users = DB.ORM.Queryable<C_USER>().Where(u => u.EMP_NO == EmpNo).ToList();
            int i = 0;
            if (users.Count > 0)
            {
                //插入到 C_USER_ROLE 中
                C_USER u = users.First();
                bool exist = DB.ORM.Queryable<C_USER_ROLE>().Where(ur => ur.USER_ID == u.ID && ur.ROLE_ID == RoleId).Any();
                if (!exist)
                {
                    i+=DB.ORM.Insertable<C_USER_ROLE>(new C_USER_ROLE()
                    {
                        ID = GetNewID(BuName, DB),
                        USER_ID = u.ID,
                        ROLE_ID = RoleId,
                        SYSTEM_NAME = SystemName,
                        EDIT_TIME = GetDBDateTime(DB),
                        EDIT_EMP = UserName,
                        OPERATE_FLAG = LevelFlag
                    }).ExecuteCommand();

                    //更新 C_USER 表中的 EMP_LEVEL
                    u.EMP_LEVEL = LevelFlag;
                    u.EDIT_EMP = UserName;
                    u.EDIT_TIME = GetDBDateTime(DB);
                    i+=DB.ORM.Updateable<C_USER>(u).Where(t => t.ID == u.ID).ExecuteCommand();
                }

            }
            return i;
        }

        /// <summary>
        /// 刪除指定的用戶所屬的角色
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <param name="RoleId"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int Delete(string EmpNo, string RoleId, OleExec DB)
        {
            int i = 0;
            List<C_USER> us = DB.ORM.Queryable<C_USER>().Where(u=>u.EMP_NO==EmpNo).ToList();
            if (us.Count>0)
            {
                string UserId = us.First().ID;

                i=DB.ORM.Deleteable<C_USER_ROLE>().Where(ur => ur.USER_ID == UserId && ur.ROLE_ID == RoleId).ExecuteCommand();
            }
            return i;
        }
        public DataObjectBase GetObjByUserIDRoleID(string USERID, string ROLE_ID, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from c_user_role where USER_ID = '{USERID}' AND ROLE_ID='{ROLE_ID}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (RowType == null)
            {
                DataObjectBase ret = NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                Assembly assembly = Assembly.Load("MESDataObject");
                object API_CLASS = assembly.CreateInstance(RowType.FullName, true, BindingFlags.CreateInstance, null, new object[] { DataInfo }, null, null);
                MethodInfo Function = RowType.GetMethod("loadData");
                Function.Invoke(API_CLASS, new object[] { res.Tables[0].Rows[0] });
                return (DataObjectBase)API_CLASS;
            }
        }
    }
    public class Row_c_user_role : DataObjectBase
    {
        public Row_c_user_role(DataObjectInfo info) : base(info)
        {

        }
        public c_user_role GetDataObject()
        {
            c_user_role DataObject = new c_user_role();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.USER_ID = this.USER_ID;
            DataObject.ROLE_ID = this.ROLE_ID;
            DataObject.OPERATE_FLAG = this.OPERATE_FLAG;
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
        public string USER_ID
        {
            get
            {
                return (string)this["USER_ID"];
            }
            set
            {
                this["USER_ID"] = value;
            }
        }
        public string ROLE_ID
        {
            get
            {
                return (string)this["ROLE_ID"];
            }
            set
            {
                this["ROLE_ID"] = value;
            }
        }
        public string OPERATE_FLAG
        {
            get
            {
                return (string)this["OPERATE_FLAG"];
            }
            set
            {
                this["OPERATE_FLAG"] = value;
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
    public class c_user_role
    {
        public string ID{get;set;}
        public string SYSTEM_NAME{get;set;}
        public string USER_ID{get;set;}
        public string ROLE_ID{get;set;}
        public string OPERATE_FLAG{get;set;}
        public DateTime EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }

    public class C_USER_ROLE
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string USER_ID { get; set; }
        public string ROLE_ID { get; set; }
        public string OPERATE_FLAG { get; set; }
        public DateTime EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }

    public class c_load_userrole
    {
        public string FACTORY{get;set;}
        public string BU_NAME{get;set;}
        public string EMP_NO{get;set;}
        public string EMP_NAME{get;set;}
        public string DPT_NAME{get;set;}
        public string ROLE_NAME{get;set;}
    }

    public class get_c_roleid
    {
        public string USER_ID{get;set;}
        public string ROLE_ID{get;set;}

    }
}