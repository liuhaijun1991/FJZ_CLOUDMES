using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_USER_PRIVILEGE : DataObjectTable
    {
        public T_C_USER_PRIVILEGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_USER_PRIVILEGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_USER_PRIVILEGE);
            TableName = "C_USER_PRIVILEGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_C_USER_PRIVILEGE getC_PrivilegebyID(string id, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_USER_PRIVILEGE where PRIVILEGE_ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_USER_PRIVILEGE ret = (Row_C_USER_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public Row_C_USER_PRIVILEGE getC_PrivilegebyIDemp(string id,string emp, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_USER_PRIVILEGE a,c_user b where a.PRIVILEGE_ID='{id}' and EMP_NO='{emp}' and A.USER_ID=B.ID ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_USER_PRIVILEGE ret = (Row_C_USER_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 使用遞歸給用戶添加權限以及上層權限
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <param name="user"></param>
        /// <param name="PrivilegeId"></param>
        /// <param name="Bu"></param>
        /// <param name="SystemName"></param>
        /// <param name="EditEmp"></param>
        /// <param name="Counter"></param>
        /// <param name="DB"></param>
        public void Add(string EmpNo,C_USER user, string PrivilegeId, string Bu,string SystemName, string EditEmp, ref Int32 Counter, OleExec DB)
        {
            if (user == null)
            {
                List<C_USER> Users = DB.ORM.Queryable<C_USER>().Where(u => u.EMP_NO == EmpNo).ToList();
                if (Users.Count > 0)
                {
                    user = Users.First();
                }
                else
                {
                    return;
                }
            }

            bool exist = DB.ORM.Queryable<C_USER_PRIVILEGE>().Where(up => up.USER_ID == user.ID && up.PRIVILEGE_ID == PrivilegeId).Any();
            if (!exist)
            {
                List<C_PRIVILEGE> ParentPrivileges = DB.ORM.Queryable<C_PRIVILEGE, C_MENU, C_PRIVILEGE>((p1, c1, p2) => p1.MENU_ID == c1.ID && c1.PARENT_CODE == p2.MENU_ID)
                    .Where((p1, c1, p2) => p1.ID == PrivilegeId).Select((p1, c1, p2) => p2).ToList();
                foreach (C_PRIVILEGE p in ParentPrivileges)
                {
                    Add(EmpNo, user, p.ID, Bu, SystemName, EditEmp, ref Counter, DB);
                }
                C_USER_PRIVILEGE up = new C_USER_PRIVILEGE()
                {
                    ID = GetNewID(Bu, DB),
                    USER_ID = user.ID,
                    PRIVILEGE_ID = PrivilegeId,
                    SYSTEM_NAME = SystemName,
                    EDIT_TIME = GetDBDateTime(DB),
                    EDIT_EMP = EditEmp
                };
                Counter+=DB.ORM.Insertable<C_USER_PRIVILEGE>(up).ExecuteCommand();
                //Counter +=DB.ORM.Insertable(up).ExecuteCommand();
                
            }
            
            
        }

        /// <summary>
        /// 使用遞歸刪除用戶的權限以及下層權限
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <param name="user"></param>
        /// <param name="PrivilegeId"></param>
        /// <param name="Counter"></param>
        /// <param name="DB"></param>
        public void Delete(string EmpNo, C_USER user, string PrivilegeId, ref Int32 Counter, OleExec DB)
        {
            if (user == null)
            {
                List<C_USER> Users = DB.ORM.Queryable<C_USER>().Where(u => u.EMP_NO == EmpNo).ToList();
                if (Users.Count > 0)
                {
                    user = Users.First();
                }
                else
                {
                    return;
                }
            }

            bool exist = DB.ORM.Queryable<C_USER_PRIVILEGE>().Where(up => up.USER_ID == user.ID && up.PRIVILEGE_ID == PrivilegeId).Any();
            if (exist)
            {
                List<C_PRIVILEGE> ChildPrivileges = DB.ORM.Queryable<C_PRIVILEGE, C_MENU, C_PRIVILEGE>((p1, c1, p2) => p1.MENU_ID == c1.PARENT_CODE && c1.ID == p2.MENU_ID)
                    .Where((p1, c1, p2) => p1.ID == PrivilegeId).Select((p1, c1, p2) => p2).ToList();
                foreach (C_PRIVILEGE p in ChildPrivileges)
                {
                    Delete(EmpNo, user, p.ID, ref Counter, DB);
                }
                Counter += DB.ORM.Deleteable<C_USER_PRIVILEGE>().Where(up => up.USER_ID == user.ID && up.PRIVILEGE_ID == PrivilegeId).ExecuteCommand();
            }
        }

        public bool CheckpPivilegeByName(OleExec sfcdb, string privilegeName, string emp_no)
        {
            bool bPivilege= sfcdb.ORM.Queryable<C_USER_PRIVILEGE, C_PRIVILEGE, C_USER>((cup, cp, cu) => cup.USER_ID == cu.ID && cup.PRIVILEGE_ID == cp.ID)
                .Where((cup, cp, cu) => cu.EMP_NO == emp_no && cp.PRIVILEGE_NAME == privilegeName).Select((cup, cp, cu)=>cup).Any();
            if (bPivilege)
            {
                return bPivilege;
            }
            else
            {                
                return sfcdb.ORM.Queryable<C_ROLE_PRIVILEGE, C_PRIVILEGE, C_USER, C_USER_ROLE>((crp, cp, cu, cur) => crp.ROLE_ID == cur.ROLE_ID && crp.PRIVILEGE_ID == cp.ID
                && cur.USER_ID == cu.ID).Where((crp, cp, cu, cur) => cu.EMP_NO == emp_no && cp.PRIVILEGE_NAME == privilegeName).Select((crp, cp, cu, cur) => cp).Any();
            }
        }
    }

    /// <summary>
    /// 用戶與權限對照表
    /// 注意：使用 SqlSugar 時，實體類的屬性一定要有 get/set 方法，否則無法進行正常插入、更新等。
    /// </summary>
    public class C_USER_PRIVILEGE
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string USER_ID { get; set; }
        public string PRIVILEGE_ID { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }

    public class Row_C_USER_PRIVILEGE : DataObjectBase
    {
        public Row_C_USER_PRIVILEGE(DataObjectInfo info) : base(info)
        {

        }
        public C_USER_PRIVILEGE GetDataObject()
        {
            C_USER_PRIVILEGE DataObject = new C_USER_PRIVILEGE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.USER_ID = this.USER_ID;
            DataObject.PRIVILEGE_ID = this.PRIVILEGE_ID;
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
        public class C_USER_PRIVILEGE
        {
            public string ID;
            public string SYSTEM_NAME;
            public string USER_ID;
            public string PRIVILEGE_ID;
            public DateTime EDIT_TIME;
            public string EDIT_EMP;
        }
    }
}