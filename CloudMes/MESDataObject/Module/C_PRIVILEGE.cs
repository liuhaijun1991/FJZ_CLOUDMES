using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESDataObject.Module
{
    public class T_C_PRIVILEGE : DataObjectTable
    {
        public T_C_PRIVILEGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PRIVILEGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PRIVILEGE);
            TableName = "C_PRIVILEGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public DataTable CheckPrivilegeID(string PRIVILEGE_ID, string PRIVILEGE_NAME, OleExec DB, DB_TYPE_ENUM DBType)
        {
            return DB.ORM.Queryable<C_PRIVILEGE>().Where(p => p.ID == PRIVILEGE_ID || p.PRIVILEGE_NAME == PRIVILEGE_NAME).ToDataTable();
        }

        public DataTable SelectPrivilegeID(OleExec DB, DB_TYPE_ENUM DBType)
        {
            return DB.ORM.Queryable<C_PRIVILEGE>().ToDataTable();
        }
        public C_PRIVILEGE getC_PrivilegebyID(string id, OleExec DB)
        {
            List<C_PRIVILEGE> Ps = DB.ORM.Queryable<C_PRIVILEGE>().Where(p => p.ID == id).ToList();
            if (Ps.Count > 0)
            {
                return Ps.First();
            }
            else
            {
                return null;
            }
        }

        public C_PRIVILEGE getC_PrivilegebyMenuID(string id, OleExec DB)
        {
            List<C_PRIVILEGE> Ps = DB.ORM.Queryable<C_PRIVILEGE>().Where(p => p.MENU_ID == id).ToList();
            if (Ps.Count > 0)
            {
                return Ps.First();
            }
            else
            {
                return null;
            }
        }

        public C_PRIVILEGE GetPrivilegeByName(string PrivilegeName, OleExec DB)
        {
            return DB.ORM.Queryable<C_PRIVILEGE>().Where(t => t.PRIVILEGE_NAME == PrivilegeName).ToList().FirstOrDefault();
        }

        public int DeleteById(string PrivilgeId, OleExec DB)
        {
            return DB.ORM.Deleteable<C_PRIVILEGE>().Where(p => p.ID == PrivilgeId).ExecuteCommand();
        }

        public int DeleteByMenuId(string MenuId, OleExec DB)
        {
            return DB.ORM.Deleteable<C_PRIVILEGE>().Where(p => p.MENU_ID == MenuId).ExecuteCommand();
        }

        public int Update(C_PRIVILEGE privilege, OleExec DB)
        {
            return DB.ORM.Updateable<C_PRIVILEGE>(privilege).Where(t => t.ID == privilege.ID).ExecuteCommand();
        }

        public List<PrivilegeEditModel> GetUserRolePrivilege(string LoginUserEmp, string EditEmp, string EmpLevel, MESDBHelper.OleExec SFCDB)
        {
            List<string> UserPrivileges = SFCDB.ORM.Queryable<C_USER_PRIVILEGE, C_USER>((up, u) => up.USER_ID == u.ID)
                        .Where((up, u) => u.EMP_NO == EditEmp)
                        .Select((up, u) => up.PRIVILEGE_ID)
                        .ToList();

            List<string> RolePrivileges = SFCDB.ORM.Queryable<C_USER, C_USER_ROLE, C_ROLE_PRIVILEGE>((u, ur, rp) => u.ID == ur.USER_ID && ur.ROLE_ID == rp.ROLE_ID)
                    .Where((u, ur, rp) => u.EMP_NO == EditEmp)
                    .Select((u, ur, rp) => rp.PRIVILEGE_ID).ToList();

            if (EmpLevel == "9")
            {
                return Enumerable.Union(
                        SFCDB.ORM.Queryable<C_PRIVILEGE>()
                        .Where(p => !UserPrivileges.Contains(p.ID)&& p.SYSTEM_NAME != null)
                            .Select(p => new PrivilegeEditModel { ID = p.ID, SYSTEM_NAME = p.SYSTEM_NAME, PRIVILEGE_NAME = p.PRIVILEGE_NAME, PRIVILEGE_DESC = p.PRIVILEGE_DESC })
                        .ToList()
                        ,
                        SFCDB.ORM.Queryable<C_PRIVILEGE>()
                        .Where(p => !RolePrivileges.Contains(p.ID) && p.SYSTEM_NAME != null)
                            .Select(p => new PrivilegeEditModel { ID = p.ID, SYSTEM_NAME = p.SYSTEM_NAME, PRIVILEGE_NAME = p.PRIVILEGE_NAME, PRIVILEGE_DESC = p.PRIVILEGE_DESC })
                        .ToList()
                        , new Comparer()).ToList();

            }
            else
            {
                return Enumerable.Union(
                    SFCDB.ORM.Queryable<C_USER, C_PRIVILEGE, C_USER_PRIVILEGE>((u, p, up) => u.ID == up.USER_ID && p.ID == up.PRIVILEGE_ID)
                    .Where((u, p, up) => u.EMP_NO == LoginUserEmp && p.SYSTEM_NAME != null)
                    .Select((u, p, up) => new PrivilegeEditModel
                    {
                        ID = p.ID,
                        SYSTEM_NAME = p.SYSTEM_NAME,
                        PRIVILEGE_NAME = p.PRIVILEGE_NAME,
                        PRIVILEGE_DESC = p.PRIVILEGE_DESC
                    })
                    .ToList()
                    ,
                    SFCDB.ORM.Queryable<C_USER, C_PRIVILEGE, C_ROLE, C_USER_ROLE, C_ROLE_PRIVILEGE>((u, p, r, ur, rp) => u.ID == ur.USER_ID && ur.ROLE_ID == rp.ROLE_ID&&ur.ROLE_ID==r.ID && rp.PRIVILEGE_ID == p.ID)
                    .Where((u, p, r, ur, rp) => p.SYSTEM_NAME != null && u.EMP_NO == LoginUserEmp && (r.ROLE_TYPE.Contains(u.DPT_NAME) || r.ROLE_TYPE == "ALL" || r.ROLE_TYPE == u.DPT_NAME))
                    .Select((u, p, ur, rp) => new PrivilegeEditModel
                    {
                        ID = p.ID,
                        SYSTEM_NAME = p.SYSTEM_NAME,
                        PRIVILEGE_NAME = p.PRIVILEGE_NAME,
                        PRIVILEGE_DESC = p.PRIVILEGE_DESC
                    })
                    .ToList()
                    , new Comparer()).ToList();
            }
        }

        public List<PrivilegeEditModel> GetUserEditPrivilege(string LoginUserEmp, string EditEmp, string EmpLevel, MESDBHelper.OleExec SFCDB)
        {
            //string sql = string.Empty;
            ///////9 emp_leve
            //sql = $@"   SELECT A.ID,
            //                   A.SYSTEM_NAME,
            //                   A.PRIVILEGE_NAME,
            //                   A.PRIVILEGE_DESC
            //              FROM C_PRIVILEGE A, C_USER B, C_USER_PRIVILEGE C
            //             WHERE C.USER_ID = B.ID
            //               AND C.PRIVILEGE_ID = A.ID  AND B.EMP_NO = '{EditEmp}' ";


            return SFCDB.ORM.Queryable<C_PRIVILEGE, C_USER, C_USER_PRIVILEGE>((p, u, up) => up.USER_ID == u.ID && up.PRIVILEGE_ID == p.ID)
                .Where((p, u, up) => u.EMP_NO == EditEmp)
                .Select((p, u, up) => new PrivilegeEditModel
                {
                    ID = p.ID,
                    SYSTEM_NAME = p.SYSTEM_NAME,
                    PRIVILEGE_NAME = p.PRIVILEGE_NAME,
                    PRIVILEGE_DESC = p.PRIVILEGE_DESC
                })
                .ToList();
            //DataSet res = SFCDB.ExecSelect(sql);
            //List<PrivilegeEditModel> Privilegelist = new List<PrivilegeEditModel>();
            //if (res.Tables[0].Rows.Count > 0)
            //{
            //    foreach (DataRow item in res.Tables[0].Rows)
            //    {
            //        Privilegelist.Add(new PrivilegeEditModel
            //        {
            //            ID = item["ID"].ToString(),
            //            SYSTEM_NAME = item["SYSTEM_NAME"].ToString(),
            //            PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
            //            PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()
            //        });
            //    }

            //}
            //return Privilegelist;
        }
    }

    public class Comparer : IEqualityComparer<PrivilegeEditModel>
    {
        public bool Equals(PrivilegeEditModel x, PrivilegeEditModel y)
        {
            return GetHashCode(x).Equals(GetHashCode(y));
        }

        public int GetHashCode(PrivilegeEditModel obj)
        {
            int hashId = obj.ID.GetHashCode();
            int hashValue = obj.PRIVILEGE_NAME.GetHashCode();
            return hashId ^ hashValue;
        }
    }

    public class C_PRIVILEGE
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string MENU_ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string BASECONFIG_FLAG { get; set; }
    }

    public class Row_C_PRIVILEGE : DataObjectBase
    {
        public Row_C_PRIVILEGE(DataObjectInfo info) : base(info)
        {

        }
        public C_PRIVILEGE GetDataObject()
        {
            C_PRIVILEGE DataObject = new C_PRIVILEGE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.MENU_ID = this.MENU_ID;
            DataObject.PRIVILEGE_NAME = this.PRIVILEGE_NAME;
            DataObject.PRIVILEGE_DESC = this.PRIVILEGE_DESC;
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
        public string MENU_ID
        {
            get
            {
                return (string)this["MENU_ID"];
            }
            set
            {
                this["MENU_ID"] = value;
            }
        }
        public string PRIVILEGE_NAME
        {
            get
            {
                return (string)this["PRIVILEGE_NAME"];
            }
            set
            {
                this["PRIVILEGE_NAME"] = value;
            }
        }
        public string PRIVILEGE_DESC
        {
            get
            {
                return (string)this["PRIVILEGE_DESC"];
            }
            set
            {
                this["PRIVILEGE_DESC"] = value;
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

        public string BASECONFIG_FLAG
        {
            get
            {
                return (string)this["BASECONFIG_FLAG"];
            }
            set
            {
                this["BASECONFIG_FLAG"] = value;
            }
        }
        public class C_PRIVILEGE
        {
            public string ID { get; set; }
            public string SYSTEM_NAME { get; set; }
            public string MENU_ID { get; set; }
            public string PRIVILEGE_NAME { get; set; }
            public string PRIVILEGE_DESC { get; set; }
            public DateTime EDIT_TIME { get; set; }
            public string EDIT_EMP { get; set; }
        }

    }

    public class PrivilegeEditModel
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }
    }
}