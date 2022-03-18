using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;

namespace MESDataObject.Module
{
    public class T_C_ROLE_PRIVILEGE : DataObjectTable
    {
        public T_C_ROLE_PRIVILEGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ROLE_PRIVILEGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROLE_PRIVILEGE);
            TableName = "C_ROLE_PRIVILEGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<c_role_privilegeinfobyemp> QueryRolePrivilegeByEmpNo(String LEVEL_FLAG, String BU, string FACTORY, string DPT_NAME, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfobyemp> RolePrivilegeList = new List<c_role_privilegeinfobyemp>();
            if (LEVEL_FLAG == "9")
            {
                sql = $@" select b.id,b.role_name,a.privilege_id,C.PRIVILEGE_NAME,c.privilege_desc from c_role_privilege a,c_role b ,c_privilege c 
                          where b.id=a.role_id and a.privilege_id=c.id and  EXISTS( SELECT 1 FROM C_USER C WHERE B.ROLE_TYPE=C.DPT_NAME AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}')";
            }
            else
            {
                sql = $@" select b.id,b.role_name,a.privilege_id,C.PRIVILEGE_NAME,c.privilege_desc from c_role_privilege a,c_role b ,c_privilege c 
                          where b.id=a.role_id and a.privilege_id=c.id and  EXISTS( SELECT 1 FROM C_USER C WHERE B.ROLE_TYPE=C.DPT_NAME AND C.DPT_NAME='{DPT_NAME}' AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}')";
            }

            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                RolePrivilegeList.Add(new c_role_privilegeinfobyemp
                {
                    ID = item["ID"].ToString().Trim(),
                    ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                    PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                    PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                    PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                });
            }
            return RolePrivilegeList;
        }

        public List<c_role_privilegeinfo> QueryRolePrivilegeByUserID(String LEVEL_FLAG, String BU, string FACTORY, string DPT_NAME, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfo> RolePrivilegeList = new List<c_role_privilegeinfo>();
            if (LEVEL_FLAG == "9")
            {
                sql = $@" SELECT B.* FROM C_USER_ROLE A,C_ROLE B WHERE B.ID=A.ROLE_ID  AND EXISTS( SELECT 1 FROM C_USER C WHERE A.USER_ID=C.ID AND  B.ROLE_TYPE=C.DPT_NAME AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}' )";
            }
            else
            {
                sql = $@" SELECT B.* FROM C_USER_ROLE A,C_ROLE B WHERE B.ID=A.ROLE_ID  AND EXISTS( SELECT 1 FROM C_USER C WHERE A.USER_ID=C.ID AND  B.ROLE_TYPE=C.DPT_NAME AND C.DPT_NAME='{DPT_NAME}' AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}' )";
            }

            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                RolePrivilegeList.Add(new c_role_privilegeinfo
                {
                    ID = item["ID"].ToString().Trim(),
                    ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                    SON_ROLE = QueryRolePrivilegeByRoleID(item["ID"].ToString().Trim(), DB)
                });
            }
            return RolePrivilegeList;
        }

        public List<SON_ROLE> QueryRolePrivilegeByRoleID(string ROLE_ID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<SON_ROLE> SonRolePrivilegeList = new List<SON_ROLE>();
            sql = $@" SELECT * FROM C_PRIVILEGE B WHERE   EXISTS (SELECT 1 FROM C_ROLE_PRIVILEGE A WHERE B.ID=A.PRIVILEGE_ID AND A.ROLE_ID='{ROLE_ID}')";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                SonRolePrivilegeList.Add(new SON_ROLE
                {
                    ID = item["ID"].ToString().Trim(),
                    PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString().Trim(),
                    PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString().Trim()

                });
            }
            return SonRolePrivilegeList;
        }

        public bool CheckPrivilegeData(string RoleId, String RolePrivilegeId, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            //string RolePrivileges = "";
            DataTable dt = new DataTable();
            sql = $@" SELECT *FROM C_ROLE_PRIVILEGE WHERE ROLE_ID='{RoleId}'AND PRIVILEGE_ID ='{RolePrivilegeId}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }

            return res;

        }

        public List<c_role_privilegeinfobyemp> QueryRolePrivilege(string ROLE_ID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfobyemp> RolePrivilegeList = new List<c_role_privilegeinfobyemp>();

            if (ROLE_ID.Length != 0)
            {
                sql = $@" select c.id,c.role_name,a.privilege_id,b.privilege_name,b.privilege_desc from c_role_privilege a,c_privilege b,c_role c where c.id=a.role_id and a.privilege_id=b.id and a.role_id='{ROLE_ID}'";
            }
            if (ROLE_ID.Length == 0)
            {
                sql = $@" select c.id,c.role_name,a.privilege_id,b.privilege_name,b.privilege_desc from c_role_privilege a,c_privilege b,c_role c where c.id=a.role_id and a.privilege_id=b.id";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                RolePrivilegeList.Add(new c_role_privilegeinfobyemp
                {
                    ID = item["ID"].ToString().Trim(),
                    ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                    PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                    PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                    PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                });
            }
            return RolePrivilegeList;
        }


        public List<c_role_privilegeinfobyemp> CheckTwoRolePrivilegeID(List<c_role_byempl> AllRoleID, string EDITROLE_ID,string EmpLevel, OleExec DB)
        {
            string allroleid = "";
            string PrivilegeID = "";
            string strsql = "";
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfobyemp> CheckRolePrivilegeIDList = new List<c_role_privilegeinfobyemp>();
            if (AllRoleID.Count != 0)
            {
                //当登录用户等级不为9时,只能管理自己拥有的角色
                AllRoleID = AllRoleID.Where(Q => Q.ID != EDITROLE_ID).ToList();
                foreach (c_role_byempl item in AllRoleID)
                {
                    allroleid += "'" + item.ID + "',";
                }

                allroleid = allroleid.TrimEnd(',');
                strsql = " a.role_id in (" + allroleid + ")";
                if (EmpLevel!="0")
                {
                    //当登录用户等级不为9时,只能管理自己拥有的角色
                    sql = $@" select c.id,c.role_name,a.privilege_id,d.privilege_name,d.privilege_desc from c_role_privilege a,c_role c,c_privilege d where a.role_id=c.id and a.privilege_id=d.id  and  {strsql}  
                      and not exists(select 1 from c_role_privilege b where b.privilege_id=a.privilege_id and b.role_id='{EDITROLE_ID}')";
                    
                    dt = DB.ExecSelect(sql).Tables[0];
                    foreach (DataRow item in dt.Rows)
                    {
                        CheckRolePrivilegeIDList.Add(new c_role_privilegeinfobyemp
                        {
                            ID = item["ID"].ToString().Trim(),
                            ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                            PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                            PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                            PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                        });
                    }
                }
                if (EmpLevel=="9")
                {
                    strsql = "";
                    if (CheckRolePrivilegeIDList!=null)
                    {
                        foreach (c_role_privilegeinfobyemp item in CheckRolePrivilegeIDList)
                        {
                            PrivilegeID += "'" + item.PRIVILEGE_ID + "',";
                        }
                        PrivilegeID = PrivilegeID.TrimEnd(',');
                        if (PrivilegeID.Trim().Length>0)
                        {
                            strsql = " and  a.id not in (" + PrivilegeID + ")";
                        }
                    }
                    sql = $@" select   'NO' ID ,
                                 'NO' role_name  ,
                                 a.id privilege_id,
                                 a.privilege_name,
                                 a.privilege_desc from c_privilege a    where
                                     NOT EXISTS
                                   (SELECT 1
                                     FROM c_role_privilege b
                                    WHERE b.privilege_id = a.id AND b.role_id = '{EDITROLE_ID}' ) {strsql}";

                    dt = DB.ExecSelect(sql).Tables[0];
                    foreach (DataRow item in dt.Rows)
                    {
                        CheckRolePrivilegeIDList.Add(new c_role_privilegeinfobyemp
                        {
                            ID = item["ID"].ToString().Trim(),
                            ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                            PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                            PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                            PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                        });
                    }

                    //添加一個有所有權限的分組以便用於添加權限的查詢 add by fgg 2018.11.22
                    string allSql = "select 'ALL' ID,'ALL' role_name,a.id privilege_id,a.privilege_name,a.privilege_desc from c_privilege a";
                    DataTable allDt = DB.ExecSelect(allSql).Tables[0];
                    List<c_role_privilegeinfobyemp> allList = new List<c_role_privilegeinfobyemp>();
                    foreach (DataRow row in allDt.Rows)
                    {
                        allList.Add(new c_role_privilegeinfobyemp
                        {
                            ID = row["ID"].ToString().Trim(),
                            ROLE_NAME = row["ROLE_NAME"].ToString().Trim(),
                            PRIVILEGE_ID = row["PRIVILEGE_ID"].ToString(),
                            PRIVILEGE_NAME = row["PRIVILEGE_NAME"].ToString(),
                            PRIVILEGE_DESC = row["PRIVILEGE_DESC"].ToString()

                        });
                    }
                    CheckRolePrivilegeIDList.AddRange(allList);
                }
        
            }

            return CheckRolePrivilegeIDList;
        }

        public Row_C_ROLE_PRIVILEGE GetC_Role_Privilege_ID(string ROLE_ID,string PRIVILEGE_ID,OleExec DB)
        {
            string sql = string.Empty;
            sql = $@" SELECT * FROM  C_ROLE_PRIVILEGE  WHERE ROLE_ID='{ROLE_ID}' AND PRIVILEGE_ID='{PRIVILEGE_ID}'  ";
            DataSet res = DB.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_ROLE_PRIVILEGE ret = (Row_C_ROLE_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }

             
        }


        public bool CheckRolePrivilege(string ROLE_ID, OleExec DB)
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

        public DataObjectBase GetObjByRoleID(string RoleID, OleExec DB)
        {
            return GetObjByRoleID(RoleID, DB, DBType);
        }

        public DataObjectBase GetObjByRoleID(string RoleID, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from {TableName} where USER_ID = '{RoleID}'";
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

        /// <summary>
        /// 根據傳入的 RoleId，PrivilegeId 遞歸刪除該角色的權限
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="PrivilegeId"></param>
        /// <param name="Counter"></param>
        /// <param name="DB"></param>
        public void Delete(string RoleId,string PrivilegeId,ref Int32 Counter,OleExec DB)
        {
            //根據傳入的 RoleId 和 PrivilegeId 查詢得到 C_ROLE_PRIVILEGE 對象
            var exist = DB.ORM.Queryable<C_ROLE_PRIVILEGE>().Where(t => t.ROLE_ID == RoleId && t.PRIVILEGE_ID == PrivilegeId).Any();
            if (exist) //如果 C_ROLE_PRIVILEGE 對象不爲空
            {
                ////查詢到該 PrivilegeId 下面對應的子頁面的集合
                List<C_PRIVILEGE> ChildPrivileges = DB.ORM.Queryable<C_PRIVILEGE, C_MENU, C_PRIVILEGE>((p1, c1, p2) => p1.MENU_ID == c1.PARENT_CODE && c1.ID == p2.MENU_ID)
                    .Where((p1, c1, p2) => p1.ID == PrivilegeId).Select((p1, c1, p2) => p2).ToList();
                //如果子權限的集合數目大於0
                if (ChildPrivileges.Count > 0)
                {
                    //遍歷每個子權限，繼續調用 Delete 進行遞歸刪除
                    foreach (C_PRIVILEGE rp in ChildPrivileges)
                    {
                        Delete(RoleId, rp.ID,ref Counter, DB);
                    }
                }
                //刪除當前權限
                Counter += DB.ORM.Deleteable<C_ROLE_PRIVILEGE>().Where(t=>t.ROLE_ID==RoleId && t.PRIVILEGE_ID==PrivilegeId).ExecuteCommand();
            }
        }

        /// <summary>
        /// 根據傳入的 RoleId，PrivilegeId 遞歸添加角色權限
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="PrivilegeId"></param>
        /// <param name="Counter"></param>
        /// <param name="Bu"></param>
        /// <param name="Emp"></param>
        /// <param name="DB"></param>
        public void Add(string RoleId, string PrivilegeId, ref Int32 Counter,string Bu,string Emp, OleExec DB)
        {
            bool exist = DB.ORM.Queryable<C_ROLE_PRIVILEGE>().Where(t => t.ROLE_ID == RoleId && t.PRIVILEGE_ID == PrivilegeId).Any();
            if (!exist)
            {
                //List<C_PRIVILEGE> ParentPrivileges=DB.ORM.Queryable<C_PRIVILEGE,C_MENU,C_PRIVILEGE>((privilege,menu,pp)=>privilege.ID==)
                var m = DB.ORM.Queryable<C_PRIVILEGE, C_MENU>((privilege, menu) => privilege.MENU_ID == menu.ID).Where((privilege, menu) => privilege.ID == PrivilegeId)
                    .Select((privilege, menu) => menu).ToList().FirstOrDefault();
                if (m != null && m.PARENT_CODE != "0")
                {
                    var p = DB.ORM.Queryable<C_PRIVILEGE>().Where(t => t.MENU_ID == m.PARENT_CODE).ToList().First();
                    if (p != null)
                    {
                        Add(RoleId, p.ID, ref Counter, Bu, Emp, DB);
                    }
                }

                C_ROLE_PRIVILEGE RolePrivilege = new C_ROLE_PRIVILEGE {  ID=GetNewID(Bu, DB), SYSTEM_NAME="MES", ROLE_ID=RoleId,PRIVILEGE_ID=PrivilegeId, EDIT_TIME=GetDBDateTime(DB),EDIT_EMP=Emp };
                Counter += DB.ORM.Insertable(RolePrivilege).ExecuteCommand();
            }
        }

        public bool CheckRoleHasPrivilege(string RoleId, string PrivilegeName, OleExec DB)
        {
            return DB.ORM.Queryable<C_ROLE_PRIVILEGE, C_PRIVILEGE>((crp, cp) => crp.PRIVILEGE_ID == cp.ID).Where((crp, cp) => crp.ID == RoleId && cp.PRIVILEGE_NAME == PrivilegeName).Select((crp,cp)=>crp).Any();
        }

        public bool CheckUserHasPrivilege(string EmpNo, string PrivilegeName, OleExec DB)
        {
            List<C_USER_ROLE> Roles = DB.ORM.Queryable<C_USER, C_USER_ROLE>((cu, cur) => cu.ID == cur.USER_ID).Where(cu => cu.EMP_NO == EmpNo).Select((cu,cur)=>cur).ToList();
            bool hasPrivilege = false;
            Roles.ForEach(role =>
            {
                if (CheckRoleHasPrivilege(role.ID, PrivilegeName, DB))
                {
                    hasPrivilege = true;
                    return;
                }
            });
            return hasPrivilege;
        }
    }


    public class c_role_privilegeinfobyemp
    {
        public string ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string PRIVILEGE_ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }

    }

    public class c_role_privilegeinfo
    {
        public string ID { get; set; }
        public string ROLE_NAME { get; set; }
        public List<SON_ROLE> SON_ROLE { get; set; }


    }

    public class SON_ROLE
    {
        public string ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }

    }

    public class C_ROLE_PRIVILEGE
    {
        public string ID { get; set; }

        public string SYSTEM_NAME { get; set; }
        public string ROLE_ID { get; set; }
        public string PRIVILEGE_ID { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        //public string PRIVILEGE_NAME { get; set; }
        //public string PRIVILEGE_DESC { get; set; }
    }
    public class Row_C_ROLE_PRIVILEGE : DataObjectBase
    {
        public Row_C_ROLE_PRIVILEGE(DataObjectInfo info) : base(info)
        {

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
        public string PRIVILEGE_ID
        {
            get
            {
                return (string)this["PRIVILEGE_ID"];
            }
            set
            {
                this["PRIVILEGE_ID"] = value;
            }
        }
    }
}