using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
using System.Data;

namespace MESStation.MESUserManager
{
    public class UserRole : MesAPIBase
    {
        static Random rand = new Random();

        protected APIInfo FCreateRole = new APIInfo()
        {
            FunctionName = "CreateRole",
            Description = "創建角色",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectRole = new APIInfo()
        {
            FunctionName = "SelectRole",
            Description = "查詢角色ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLE_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectUserRoleInfo = new APIInfo()
        {
            FunctionName = "SelectUserRoleInfo",
            Description = "BY工號查詢用戶所擁有角色",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUpdateRole = new APIInfo()
        {
            FunctionName = "UpdateRole",
            Description = "更新角色ID（c_role表）信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_DESC", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteRole = new APIInfo()
        {
            FunctionName = "DeleteRole",
            Description = "刪除角色ID（delete c_role 信息）",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddRolePrivilege = new APIInfo()
        {
            FunctionName = "AddRolePrivilege",
            Description = "添加某一角色能擁有的權限（Insert c_role_privilege）",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectRolePrivilege = new APIInfo()
        {
            FunctionName = "SelectRolePrivilege",
            Description = "根據傳入條件查詢某一角色能擁有的權限", 

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLE_ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteRolePrivilege = new APIInfo()
        {
            FunctionName = "DeleteRolePrivilege",
            Description = "剔除某一角色所所擁有的權限  ",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddUserRolePrivilege = new APIInfo()
        {
            FunctionName = "AddUserRolePrivilege",
            Description = "給用戶賦予角色權限，并可更新該用戶等級",

            Parameters = new List<APIInputInfo>()
                {
                  new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                  new APIInputInfo() {InputName = "LEVEL_FLAG", InputType = "string", DefaultValue = "" },
                  new APIInputInfo() {InputName = "ROLE_ID", InputType = "string", DefaultValue = "" }
                },
            Permissions = new List<MESPermission>()
        };


        protected APIInfo FDeleteUserRolePrivilege = new APIInfo()
        {
            FunctionName = "DeleteUserRolePrivilege",
            Description = "BY USER_ID刪除用戶某一角色",

            Parameters = new List<APIInputInfo>()
                {
                  new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                  new APIInputInfo() {InputName = "ROLE_ID", InputType = "string", DefaultValue = "" }
                },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectRolePrivilegeByUserID = new APIInfo()
        {
            FunctionName = "SelectRolePrivilegeByUserID",
            Description = "根據登錄系統的用戶查詢該用戶下能管理的角色及所擁有的權限,EMP_LEVEL=9 會帶出所有角色, EMP_LEVEL=1,只能帶出本部門角色 ,EMP_LEVEL=0 無權限管理角色",

            Parameters = new List<APIInputInfo>()
            {
                //   new APIInputInfo() {InputName = "USER_ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectRoleByEmp_level = new APIInfo()
        {
            FunctionName = "SelectRoleByEmp_level",
            Description = "根據用戶等級帶出該USER所能管理的角色,EMP_LEVEL=9 代表超級管理員,1代表可以管理本部門所有角色,0 代表普通用戶",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };


        protected APIInfo FLoadUserRoleInfo = new APIInfo()
        {
            FunctionName = "LoadUserRoleInfo",
            Description = "根據條件查詢用戶擁有的角色（EMP_NO為空時查詢本廠別,BU所有人員角色，不為空查詢該工號角色）",

            Parameters = new List<APIInputInfo>()
                {
                  new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" }
                },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FLoadUserRoleListByRole = new APIInfo()
        {
            FunctionName = "LoadUserRoleListByRole",
            Description = "根据角色查询下面的用户列表",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLENAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        


        protected APIInfo FSelectRolePrivilegeByEmpNo = new APIInfo()
        {
            FunctionName = "SelectRolePrivilegeByEmpNo",
            Description = "通過登錄用戶工號查詢該用戶所能管理的角色以及角色中打包的權限",

            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FCheckTwoRolePrivilegeID = new APIInfo()
        {
            FunctionName = "CheckTwoRolePrivilegeID",
            Description = "根據第一個角色ID比對第二個角色篩選出角色2沒有的權限ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EDITROLE_ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };


        public UserRole()
        {
            this.Apis.Add(FCreateRole.FunctionName, FCreateRole);      /// 創建角色ID Inser into C_ROLE
            this.Apis.Add(FSelectRole.FunctionName, FSelectRole);      /// 查詢角色ID(單純查詢c_role表BY字段ROLE_NAME)
            this.Apis.Add(FUpdateRole.FunctionName, FUpdateRole);      /// 更新角色ID信息,更新C_ROLE表相關內容
            this.Apis.Add(FDeleteRole.FunctionName, FDeleteRole);      /// 刪除角色ID信息刪除C_ROLE表,C_ROLE_PRIVILEGE ,C_USER_ROLE
            this.Apis.Add(FAddRolePrivilege.FunctionName, FAddRolePrivilege);   ///添加角色所擁有的權限ID INSERT C_ROLE_PRIVILEGE
            this.Apis.Add(FSelectRolePrivilege.FunctionName, FSelectRolePrivilege);      /// 查詢角色權限C_ROLE_PRIVILEGE
            this.Apis.Add(FSelectRolePrivilegeByUserID.FunctionName, FSelectRolePrivilegeByUserID);    /// 根據USER_ID查詢該用戶下能管理的角色所擁有的權限    EMP_LEVEL=9 會帶出所有角色, EMP_LEVEL=1 只能帶出本部門角色  EMP_LEVEL=0 無權限管理角色
            this.Apis.Add(FDeleteRolePrivilege.FunctionName, FDeleteRolePrivilege);        /// 刪除角色權限 
            this.Apis.Add(FAddUserRolePrivilege.FunctionName, FAddUserRolePrivilege);        ///給用戶添加角色 INSERT C_USER_ROLE,并更新C_USER LEVEL_FLAG欄位的值 0表示普通用戶，1表示普通管理員，9 IT所有
            this.Apis.Add(FDeleteUserRolePrivilege.FunctionName, FDeleteUserRolePrivilege);
            this.Apis.Add(FSelectUserRoleInfo.FunctionName, FSelectUserRoleInfo);   /// 查詢該USE擁有的角色 C_USER_ROLE
            this.Apis.Add(FSelectRoleByEmp_level.FunctionName, FSelectRoleByEmp_level);   /// 根據用戶等級帶出該USER所能管理的角色   EMP_LEVEL=9 代表超級管理員  1代表可以管理本部門所有角色 0 代表普通用戶
            this.Apis.Add(FLoadUserRoleInfo.FunctionName, FLoadUserRoleInfo);
            this.Apis.Add(FSelectRolePrivilegeByEmpNo.FunctionName, FSelectRolePrivilegeByEmpNo);
            this.Apis.Add(FCheckTwoRolePrivilegeID.FunctionName, FCheckTwoRolePrivilegeID);
        }

        /// <summary>
        /// 創建角色ID Inser into C_ROLE
        /// </summary>
        public void CreateRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            string InsertSql = "";
            T_C_ROLE Roler;

            string EMP_LEVEL = this.LoginUser.EMP_LEVEL;
            string DPT_NAME = this.LoginUser.DPT_NAME;

            string ROLE_NAME = Data["ROLE_NAME"].ToString();
            string ROLE_TYPE = Data["ROLE_TYPE"].ToString();
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                Roler = new T_C_ROLE(SFCDB, DBTYPE);

                if (Roler.CheckRoleData(ROLE_NAME, ROLE_TYPE, EMP_LEVEL, DPT_NAME, SFCDB))
                {
                    Row_C_ROLE row = (Row_C_ROLE)Roler.NewRow();
                    row.ID = Roler.GetNewID(BU, SFCDB);
                    row.ROLE_NAME = Data["ROLE_NAME"].ToString();
                    row.ROLE_DESC = Data["ROLE_DESC"].ToString();
                    if (this.LoginUser.EMP_LEVEL == "9")
                    {
                        row.ROLE_TYPE = Data["ROLE_TYPE"].ToString();

                        row.EDIT_EMP = LoginUser.EMP_NO;
                        row.SYSTEM_NAME = SystemName;
                        row.EDIT_TIME = GetDBDateTime();
                        InsertSql = row.GetInsertString(DBTYPE);
                        SFCDB.ExecSQL(InsertSql);
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000002");
                        StationReturn.Data = "";
                        this.DBPools["SFCDB"].Return(SFCDB);
                    }
                    else
                    {
                        row.ROLE_TYPE = this.LoginUser.DPT_NAME;

                        if (Data["ROLE_TYPE"].ToString()!=this.LoginUser.DPT_NAME)
                        {
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            //StationReturn.Message = "该用户等级不能创建本部门之外的权限";
                            StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145330"); 
                            this.DBPools["SFCDB"].Return(SFCDB);
                        }
                        else
                        {
                            row.EDIT_EMP = LoginUser.EMP_NO;
                            row.SYSTEM_NAME = SystemName;
                            row.EDIT_TIME = GetDBDateTime();
                            InsertSql = row.GetInsertString(DBTYPE);
                            SFCDB.ExecSQL(InsertSql);
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000002");
                            StationReturn.Data = "";
                            this.DBPools["SFCDB"].Return(SFCDB);
                        }
                        
                    }

                  

                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.Message = "角色已存在";
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145428");
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                throw e;
            }

        }

        /// <summary>
        /// 查詢角色ID(單純查詢c_role表BY字段ROLE_NAME)
        /// </summary>
        public void SelectRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_ROLE> rolelist = new List<C_ROLE>();
            T_C_ROLE Roler;
            string RoleName = Data["ROLE_NAME"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Roler = new T_C_ROLE(sfcdb, DBTYPE);
                rolelist = Roler.Getrolelist(RoleName,this.LoginUser.EMP_LEVEL,this.LoginUser.DPT_NAME, sfcdb);
                if (rolelist != null)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000026");
                    StationReturn.Data = rolelist;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000034");
                    StationReturn.Data = rolelist;
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 根據用戶等級帶出該USER所能管理的角色   EMP_LEVEL=9 代表超級管理員  1代表可以管理本部門所有角色 0 代表普通用戶
        /// </summary>
        public void SelectRoleByEmp_level(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<c_role_byempl> RoleInfoLevel;
            T_C_ROLE RoleInfo;
            T_c_user_role GetRoleID;

            string EMP_LEVEL = this.LoginUser.EMP_LEVEL;
            string FACTORY = this.LoginUser.FACTORY;
            string BU_NAME = this.LoginUser.BU;
            string DPT_NAME = this.LoginUser.DPT_NAME;
            string EDIT_EMP = Data["EDIT_EMP"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RoleInfo = new T_C_ROLE(sfcdb, DBTYPE);
                GetRoleID = new T_c_user_role(sfcdb, DBTYPE);
                List<get_c_roleid> ROLE_ID = GetRoleID.GetRoleID(EDIT_EMP, sfcdb);
                RoleInfoLevel = RoleInfo.ManageRoleByUser(ROLE_ID, DPT_NAME, BU_NAME, FACTORY, EMP_LEVEL, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000026");
                StationReturn.Data = RoleInfoLevel;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 查詢該USE擁有的角色 C_USER_ROLE
        /// </summary>
        public void SelectUserRoleInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_ROLE> RoleInfoLevel;
            T_C_ROLE RoleInfo;
            T_c_user_role GetUserID;

            string EMP_NO = Data["EMP_NO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RoleInfo = new T_C_ROLE(sfcdb, DBTYPE);
                GetUserID = new T_c_user_role(sfcdb, DBTYPE);
                string USERID = GetUserID.GetUserID(EMP_NO, sfcdb);
                string LoginUserID = GetUserID.GetUserID(this.LoginUser.EMP_NO, sfcdb);
                bool LoginEmp=false;
                if (USERID==LoginUserID)
                {
                    LoginEmp = true;
                }
                RoleInfoLevel = RoleInfo.GetUserRolelist(USERID, LoginEmp,this.LoginUser.EMP_LEVEL,this.LoginUser.DPT_NAME, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000026");
                StationReturn.Data = RoleInfoLevel;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }



        /// <summary>
        /// 更新角色ID信息,更新C_ROLE表相關內容
        /// </summary>
        public void UpdateRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_C_ROLE GetRoleInformation;
            string ID = Data["ID"].ToString();
            string ROLE_NAME = Data["ROLE_NAME"].ToString();
            string ROLE_DESC = Data["ROLE_DESC"].ToString();
            string ROLE_TYPE = Data["ROLE_TYPE"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                GetRoleInformation = new T_C_ROLE(sfcdb, DBTYPE);
                Row_C_ROLE row = (Row_C_ROLE)GetRoleInformation.GetObjByID(Data["ID"].ToString(), sfcdb);
                row.ID = ID;
                row.ROLE_NAME = ROLE_NAME;
                row.ROLE_DESC = ROLE_DESC;
                row.ROLE_TYPE = ROLE_TYPE;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.SYSTEM_NAME = SystemName;
                row.EDIT_TIME = GetDBDateTime();

                UpdateSql = row.GetUpdateString(DBTYPE);
                string s = sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000035", new[] { s });
                StationReturn.Data = "";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 刪除角色ID信息刪除C_ROLE表,C_ROLE_PRIVILEGE ,C_USER_ROLE
        /// </summary>
        public void DeleteRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string strid = "";
            T_C_ROLE GetRoleInformation;
            T_C_ROLE_PRIVILEGE GetRolePrivilegeInfo;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                GetRoleInformation = new T_C_ROLE(sfcdb, DBTYPE);
                GetRolePrivilegeInfo = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
                foreach (string item in Data["ID"])
                {
                    strid = item.Trim('\'').Trim('\"');
                    if (GetRoleInformation.CheckRole(strid, sfcdb))
                    {                       
                        Row_C_ROLE row = (Row_C_ROLE)GetRoleInformation.GetObjByID(strid, sfcdb);
                        DeleteSql += row.GetDeleteString(DBTYPE) + ";\n";
                        if (!GetRolePrivilegeInfo.CheckRolePrivilege(strid, sfcdb)) //檢查C_ROLE_PRIVILEGE是否有添加角色權限，如果存在就刪除
                        {
                            Row_C_PRIVILEGE PrivilegeRow = (Row_C_PRIVILEGE)GetRolePrivilegeInfo.GetObjByRoleID(strid, sfcdb);
                            DeleteSql += PrivilegeRow.GetDeleteString(DBTYPE) + ";\n";
                        } 
                    }
                    else
                    {
                        sfcdb.RollbackTrain();
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145454");
                        //StationReturn.Message = "該角色已被賦予用戶使用,不能刪除";
                        StationReturn.Data = "";
                        this.DBPools["SFCDB"].Return(sfcdb);
                        return;
                    }
                }            

                DeleteSql = "BEGIN\n" + DeleteSql + "END;";

                sfcdb.ExecSQL(DeleteSql);
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000004");
                sfcdb.CommitTrain();
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        /// <summary>
        ///添加角色所擁有的權限ID INSERT C_ROLE_PRIVILEGE
        /// </summary>
        public void AddRolePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            //string InsertSql = "";
            //string  P_code = "";
            T_C_ROLE_PRIVILEGE Roler;
            string ROLE_ID = Data["ROLE_ID"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                Roler = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
                //T_C_PRIVILEGE  CheckExistMenuID = new T_C_PRIVILEGE(sfcdb, DBTYPE);
                //T_C_MENU tcm = new T_C_MENU(sfcdb, DBTYPE);
                //Row_C_MENU rcm = (Row_C_MENU)tcm.NewRow();
                //T_C_ROLE_PRIVILEGE tcup = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
                //Row_C_ROLE_PRIVILEGE rcup = (Row_C_ROLE_PRIVILEGE)tcup.NewRow();
                Int32 Counter = 0;

                foreach (string item in Data["PRIVILEGE_ID"])
                {
                    string PRIVILEGE_ID = item.Trim('\'').Trim('\"');
                    Roler.Add(ROLE_ID, PRIVILEGE_ID, ref Counter, LoginUser.BU, LoginUser.EMP_NO, sfcdb);
                    //    if (Roler.CheckPrivilegeData(ROLE_ID, PRIVILEGE_ID, sfcdb))
                    //    {                       
                    //        Row_C_ROLE_PRIVILEGE row = (Row_C_ROLE_PRIVILEGE)Roler.NewRow();
                    //        row.ID = Roler.GetNewID(BU, sfcdb);
                    //        row.ROLE_ID = ROLE_ID;
                    //        row.PRIVILEGE_ID = PRIVILEGE_ID;
                    //        row.EDIT_EMP = LoginUser.EMP_NO;
                    //        row.SYSTEM_NAME = SystemName;
                    //        row.EDIT_TIME = GetDBDateTime();
                    //        InsertSql += row.GetInsertString(DBTYPE) + ";\n";

                    //        Row_C_PRIVILEGE ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.getC_PrivilegebyID(PRIVILEGE_ID, sfcdb);
                    //        if (ChRow.MENU_ID!=null|| ChRow.MENU_ID!="N/A")
                    //        {
                    //            P_code += ChRow.MENU_ID + ",";
                    //            rcm = tcm.getC_MenubyID(ChRow.MENU_ID, sfcdb);

                    //            if (rcm!=null && rcm.PARENT_CODE != "0")
                    //            {
                    //                if (P_code.IndexOf(rcm.PARENT_CODE) < 0 && tcup.GetC_Role_Privilege_ID(ROLE_ID, ChRow.MENU_ID, sfcdb) == null && Roler.CheckPrivilegeData(ROLE_ID, rcm.PARENT_CODE, sfcdb))
                    //                {
                    //                    rcup.ID = tcup.GetNewID(BU, sfcdb);
                    //                    rcup.SYSTEM_NAME = SystemName;
                    //                    rcup.ROLE_ID = ROLE_ID;
                    //                    rcup.PRIVILEGE_ID = rcm.PARENT_CODE;
                    //                    rcup.EDIT_EMP = LoginUser.EMP_NO;
                    //                    rcup.EDIT_TIME = DateTime.Now;
                    //                    InsertSql += rcup.GetInsertString(this.DBTYPE) + ";\n";
                    //                    P_code += rcm.PARENT_CODE + ",";
                    //                }
                    //                do
                    //                {
                    //                    rcm = tcm.getC_MenubyID(rcm.PARENT_CODE, sfcdb);
                    //                    if (rcm!=null && rcm.PARENT_CODE != "0")
                    //                    {
                    //                        if (P_code.IndexOf(rcm.PARENT_CODE) < 0 && tcup.GetC_Role_Privilege_ID(ROLE_ID, ChRow.MENU_ID, sfcdb) == null && Roler.CheckPrivilegeData(ROLE_ID, rcm.PARENT_CODE, sfcdb))
                    //                        {
                    //                            rcup.ID = tcup.GetNewID(BU, sfcdb);
                    //                            rcup.SYSTEM_NAME = SystemName;
                    //                            rcup.ROLE_ID = ROLE_ID;
                    //                            rcup.PRIVILEGE_ID = rcm.PARENT_CODE;
                    //                            rcup.EDIT_EMP = LoginUser.EMP_NO;
                    //                            rcup.EDIT_TIME = DateTime.Now;
                    //                            InsertSql += rcup.GetInsertString(this.DBTYPE) + ";\n";
                    //                            P_code += rcm.PARENT_CODE + ",";
                    //                        }
                    //                    }
                    //                } while (rcm.PARENT_CODE != "0");
                    //            }
                    //        }

                    //    }
                    //    else
                    //    {
                    //        StationReturn.Status = StationReturnStatusValue.Fail;
                    //        StationReturn.Message = "該角色已包含該權限ID";
                    //        StationReturn.Data = "";
                    //        this.DBPools["SFCDB"].Return(sfcdb);
                    //        return;
                    //    }
                    //}

                    //InsertSql = "begin\n" + InsertSql + "end;";

                    //sfcdb.ExecSQL(InsertSql);
                    //sfcdb.CommitTrain();
                    
                }

                if (Counter > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000002");
                }
                sfcdb.CommitTrain();
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 根據登錄系統的用戶查詢該用戶下能管理的角色所擁有的權限    EMP_LEVEL=9 會帶出所有角色, EMP_LEVEL=1 只能帶出本部門角色  EMP_LEVEL=0 無權限管理角色
        /// </summary>
        public void SelectRolePrivilegeByUserID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_ROLE_PRIVILEGE RolePrivilege;
            List<c_role_privilegeinfo> RolePrivilegeList;
            //   string USER_ID = Data["USER_ID"].ToString();
            String LEVEL_FLAG = LoginUser.EMP_LEVEL;
            if (LEVEL_FLAG == "0")
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000028");
                return;
            }
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RolePrivilege = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
                RolePrivilegeList = RolePrivilege.QueryRolePrivilegeByUserID(LEVEL_FLAG, this.LoginUser.BU, this.LoginUser.FACTORY, this.LoginUser.DPT_NAME, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000026");
                StationReturn.Data = RolePrivilegeList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        /// <summary>
        /// 查詢角色權限C_ROLE_PRIVILEGE
        /// </summary>
        public void SelectRolePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_ROLE_PRIVILEGE RolePrivilege;
            List<c_role_privilegeinfobyemp> RolePrivilegeList;
            string Role_Id = Data["ROLE_ID"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RolePrivilege = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
                RolePrivilegeList = RolePrivilege.QueryRolePrivilege(Role_Id, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000026");
                StationReturn.Data = RolePrivilegeList;
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 刪除角色權限
        /// </summary>
        public void DeleteRolePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec  sfcdb = this.DBPools["SFCDB"].Borrow();
            //string DeleteSql = "", P_code="";
            //string strid = "";
            //string MenuID = "";

            //T_C_PRIVILEGE CheckExistMenuID = new T_C_PRIVILEGE(sfcdb, DBTYPE);
            //Row_C_PRIVILEGE ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.NewRow();
            //T_C_MENU tcm = new T_C_MENU(sfcdb, DBTYPE);
            //DataTable rcm = new DataTable();
            //DataTable TTcm = new DataTable();
            //List<c_role_privilegeinfobyemp> RolePrivilegeList = new List<c_role_privilegeinfobyemp>();
            //List<string> DeletePrivilege = new List<string>();
            //List<MENUS> GetMenuList = new List<MENUS>();
            T_C_ROLE_PRIVILEGE tcup = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
            //Row_C_ROLE_PRIVILEGE rcup = (Row_C_ROLE_PRIVILEGE)tcup.NewRow();
            //T_C_MENU Tmenu = new T_C_MENU(sfcdb, DBTYPE);
            string ROLE_ID = Data["ROLE_ID"].ToString();
            Int32 Counter = 0;
            try
            {

                //sfcdb.BeginTrain();
                //foreach (string  item in Data["PRIVILEGE_ID"])
                //{
                //    DeletePrivilege.Add(item.Trim('\'').Trim('\"'));
                //}


                foreach (string item in Data["PRIVILEGE_ID"])
                {

                    string PRIVILEGE_ID = item.Trim('\'').Trim('\"');
                    tcup.Delete(ROLE_ID, PRIVILEGE_ID, ref Counter, sfcdb);

                    //RolePrivilegeList = tcup.QueryRolePrivilege(ROLE_ID, sfcdb);

                    //ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.getC_PrivilegebyID(PRIVILEGE_ID, sfcdb);
                    //if (ChRow.MENU_ID != null && ChRow.MENU_ID != "N/A")
                    //{
                    //    if (!Tmenu.Check_PARENT(ChRow.MENU_ID,sfcdb))  //CHECK刪除的權限是否存在下級權限
                    //    {
                    //        GetMenuList = Tmenu.GetMenuNextID("PARENT_CODE", ChRow.MENU_ID, sfcdb);
                    //        if (GetMenuList!=null)
                    //        {
                    //            for (int i = 0; i < GetMenuList.Count-1; i++)
                    //            {
                    //                ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.getC_PrivilegebyMenuID(GetMenuList[i].ID.ToString(), sfcdb);
                    //                c_role_privilegeinfobyemp h = RolePrivilegeList.Find(s=>s.PRIVILEGE_ID== ChRow.ID);
                    //                if (h != null)
                    //                {
                    //                    string u = DeletePrivilege.Find(t => t == ChRow.ID);
                    //                    if (u == null || u == "")
                    //                    {
                    //                        StationReturn.Status = StationReturnStatusValue.Fail;
                    //                        StationReturn.Message = "要刪除的權限存在下級權限，無法越級刪除";
                    //                        StationReturn.Data = ChRow.PRIVILEGE_NAME;
                    //                        this.DBPools["SFCDB"].Return(sfcdb);
                    //                        return;
                    //                    }
                    //                }

                    //            }
                    //        }

                    //    }

                    //        rcup = tcup.GetC_Role_Privilege_ID(ROLE_ID, PRIVILEGE_ID, sfcdb);
                    //        strid = rcup.ID.ToString();
                    //        rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetObjByID(strid, sfcdb);
                    //        DeleteSql += rcup.GetDeleteString(DBTYPE) + ";\n";

                    //}
                    //else
                    //{
                    //    rcup = tcup.GetC_Role_Privilege_ID(ROLE_ID, PRIVILEGE_ID, sfcdb);
                    //    strid = rcup.ID.ToString();
                    //    rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetObjByID(strid, sfcdb);
                    //    DeleteSql += rcup.GetDeleteString(DBTYPE) + ";\n";
                    //}


                    // by SDL  CHECK要刪除的權限要是存在下級菜單則不允許刪除
                    //    rcup = tcup.GetC_Role_Privilege_ID(ROLE_ID, PRIVILEGE_ID, sfcdb); //根據 Role Id 和 Privilege ID 獲取到 C_ROLE_PRIVILEGE 對象
                    //    strid = rcup.ID.ToString(); //獲取 C_ROLE_PRIVILEGE 對象的 ID 屬性
                    //    rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetObjByID(strid, sfcdb); //根據 ID 屬性獲取到 ROW_C_ROLE_PRIVILEGE 對象
                    //    DeleteSql += rcup.GetDeleteString(DBTYPE) + ";\n"; // 構建刪除語句

                    //    ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.getC_PrivilegebyID(PRIVILEGE_ID, sfcdb); // 獲取傳入的 PRIVILEGE_ID 到 ROW_C_PRIVILEGE 對象
                    //    if (ChRow.MENU_ID != null || ChRow.MENU_ID != "N/A") //如果 ROW_C_PRIVILEGE 對象的 MENU_ID 不爲空
                    //    {
                    //        rcm = tcm.getC_MenubyPARENT_CODE(ChRow.MENU_ID, sfcdb); //查詢該菜單下面的子菜單

                    //        if (rcm.Rows.Count != 0) //如果該菜單下面存在子菜單
                    //        {

                    //            for (int i = 0; i < rcm.Rows.Count - 1; i++)
                    //            {

                    //                MenuID = rcm.Rows[i]["ID"].ToString(); //獲取子菜單的 ID 

                    //                ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.getC_PrivilegebyMenuID(MenuID, sfcdb); //查詢子菜單的 ID 對應的權限 C_PRIVILEGE 對象
                    //                rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetC_Role_Privilege_ID(ROLE_ID, ChRow.ID, sfcdb); // 根據 Role_ID 和 Privilge_ID
                    //                if (P_code.IndexOf(MenuID) < 0 && rcup != null)
                    //                { //如果該角色存在該子菜單對應的權限，那麽將級聯刪除該權限
                    //                    rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetObjByID(rcup.ID, sfcdb);
                    //                    DeleteSql += rcup.GetDeleteString(DBTYPE) + ";\n";
                    //                    P_code += MenuID + ",";
                    //                }
                    //                TTcm = tcm.getC_MenubyPARENT_CODE(MenuID, sfcdb); //查詢子菜單下面是否存在有子菜單

                    //                if (TTcm.Rows.Count != 0) //如果存在子子菜單
                    //                {
                    //                    for (int j = 0; j < TTcm.Rows.Count - 1; j++)
                    //                    {
                    //                        MenuID = TTcm.Rows[j]["ID"].ToString();
                    //                        ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.getC_PrivilegebyMenuID(MenuID, sfcdb);//根據子子菜單的Menu_ID獲取對應的 C_PRIVILEGE 對象
                    //                        if (ChRow != null)
                    //                        {
                    //                            rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetC_Role_Privilege_ID(ROLE_ID, ChRow.ID, sfcdb);//查看該角色是否存在該權限
                    //                            if (P_code.IndexOf(MenuID) < 0 && rcup != null)
                    //                            {//如果存在，級聯刪除該權限
                    //                                rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetObjByID(rcup.ID, sfcdb);
                    //                                DeleteSql += rcup.GetDeleteString(DBTYPE) + ";\n";
                    //                                P_code += MenuID + ",";
                    //                            }
                    //                        }

                    //                        //在下一層，如果該角色依舊存在子子子權限，那麽也級聯刪除
                    //                        DataTable HHcm = tcm.getC_MenubyPARENT_CODE(MenuID, sfcdb);

                    //                        if (HHcm.Rows.Count != 0)
                    //                        {
                    //                            for (int z = 0; z < HHcm.Rows.Count - 1; z++)
                    //                            {
                    //                                MenuID = HHcm.Rows[z]["ID"].ToString();
                    //                                ChRow = (Row_C_PRIVILEGE)CheckExistMenuID.getC_PrivilegebyMenuID(MenuID, sfcdb);
                    //                                if (ChRow != null)
                    //                                {
                    //                                    rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetC_Role_Privilege_ID(ROLE_ID, ChRow.ID, sfcdb);
                    //                                    if (P_code.IndexOf(MenuID) < 0 && rcup != null)
                    //                                    {//
                    //                                        rcup = (Row_C_ROLE_PRIVILEGE)tcup.GetObjByID(rcup.ID, sfcdb);
                    //                                        DeleteSql += rcup.GetDeleteString(DBTYPE) + ";\n";
                    //                                        P_code += MenuID + ",";
                    //                                    }
                    //                                }

                    //                            }
                    //                        }

                    //                    }
                    //                }
                    //            }
                    //        }

                    //    }

                    //}

                    //DeleteSql = "begin\n" + DeleteSql + "end;";
                    //sfcdb.ExecSQL(DeleteSql);
                    //sfcdb.CommitTrain();
                    if (Counter > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000004");
                        sfcdb.CommitTrain();
                        
                    }
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }


        /// <summary>
        ///給用戶添加角色 INSERT C_USER_ROLE,并更新C_USER LEVEL_FLAG欄位的值 0表示普通用戶，1表示普通管理員，9 IT所有
        /// </summary>
        public void AddUserRolePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            //string InsertSql = "", UpdateSql = "";
            T_c_user_role UserRoler;
            //T_c_user EmpLevel;
            string EMP_NO = Data["EMP_NO"].ToString();
            string LEVEL_FLAG = Data["LEVEL_FLAG"].ToString();
            int i = 0;
            //    string[] ROLE_ID = Data["ROLE_ID"].ToString().Trim('[').Trim(']').Split(',');
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                UserRoler = new T_c_user_role(sfcdb, DBTYPE);
               // EmpLevel = new T_c_user(sfcdb, DBTYPE);
               // string USER_ID = UserRoler.GetUserID(EMP_NO, sfcdb);

                foreach (string item in Data["ROLE_ID"])
                {
                    string strroleid = item.Trim('\'').Trim('\"');
                    i+=UserRoler.Add(EMP_NO, strroleid, LEVEL_FLAG, LoginUser.BU, LoginUser.EMP_NO, SystemName, sfcdb);
                    //if (UserRoler.CheckUserRole(USER_ID, strroleid, sfcdb))
                    //{
                    //    Row_c_user_role row = (Row_c_user_role)UserRoler.NewRow();
                    //    row.ID = UserRoler.GetNewID(BU, sfcdb);
                    //    row.USER_ID = USER_ID;
                    //    row.ROLE_ID = strroleid;
                    //    row.EDIT_EMP = LoginUser.EMP_NO;
                    //    row.SYSTEM_NAME = SystemName;
                    //    row.EDIT_TIME = GetDBDateTime();
                    //    InsertSql += row.GetInsertString(DBTYPE) + ";\n";
                    //}
                    //else
                    //{
                    //    sfcdb.RollbackTrain();
                    //    StationReturn.Status = StationReturnStatusValue.Fail;
                    //    StationReturn.Message = "該用戶已擁有該角色";
                    //    StationReturn.Data = "";
                    //    this.DBPools["SFCDB"].Return(sfcdb);
                    //    return;
                    //}

                }

                //InsertSql = "begin\n" + InsertSql + "end;";
                //sfcdb.ExecSQL(InsertSql);

                //if (EmpLevel.CheckEmpLevel(USER_ID, LEVEL_FLAG, sfcdb))
                //{
                //    Row_c_user RowLevel = (Row_c_user)EmpLevel.GetObjByID(USER_ID, sfcdb);
                //    RowLevel.ID = USER_ID;
                //    RowLevel.EMP_LEVEL = LEVEL_FLAG;
                //    RowLevel.EDIT_EMP = LoginUser.EMP_NO;
                //    RowLevel.EDIT_TIME = GetDBDateTime();

                //    UpdateSql = RowLevel.GetUpdateString(DBTYPE);
                //    sfcdb.ExecSQL(UpdateSql);
                //}

                if (i > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000002");
                    sfcdb.CommitTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        ///刪除用戶添加角色 DELETE C_USER_ROLE
        /// </summary>
        public void DeleteUserRolePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            //string DeleteSql = "";
            T_c_user_role UserRole;
            string EMP_NO = Data["EMP_NO"].ToString();
            int i = 0;
            //   string[] ROLE_ID = Data["ROLE_ID"].ToString().Split(',');

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                UserRole = new T_c_user_role(sfcdb, DBTYPE);
                //string USER_ID = GetUserRole.GetUserID(EMP_NO, sfcdb);
                sfcdb.BeginTrain();
                foreach (string item in Data["ROLE_ID"])
                {
                    string strroleid = item.Trim('\'').Trim('\"');
                    i+=UserRole.Delete(EMP_NO, strroleid, sfcdb);
                    //Row_c_user_role row = (Row_c_user_role)GetUserRole.GetObjByUserIDRoleID(USER_ID, strroleid, sfcdb, this.DBTYPE);
                    //DeleteSql += row.GetDeleteString(DBTYPE) + ";\n";
                }
                //DeleteSql = "begin\n" + DeleteSql + "end;";
                //sfcdb.ExecSQL(DeleteSql);
                //sfcdb.CommitTrain();
                if (i > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000004");
                    sfcdb.CommitTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }


        public void LoadUserRoleInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_c_user_role UserRoleInfo;
            List<c_load_userrole> UserRoleInfoList;
            string EMP_NO = Data["EMP_NO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                UserRoleInfo = new T_c_user_role(sfcdb, DBTYPE);
                UserRoleInfoList = UserRoleInfo.QueryUserRoleInfo(EMP_NO, this.LoginUser.DPT_NAME, this.LoginUser.BU, this.LoginUser.FACTORY,this.LoginUser.EMP_LEVEL, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000026");
                StationReturn.Data = UserRoleInfoList;
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void LoadUserRoleListByRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ROLENAME = Data["ROLENAME"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var UserRoleInfoList = sfcdb.ORM
                    .Queryable<C_USER, c_user_role, C_ROLE>((cu, cur, cr) =>
                        cr.ID == cur.ROLE_ID && cur.USER_ID == cu.ID && cr.ROLE_NAME.Contains(ROLENAME))
                    .Select((cu, cur, cr) =>
                        new {cu.FACTORY, cu.BU_NAME, cu.EMP_NO, cu.EMP_NAME, cu.DPT_NAME, cr.ROLE_NAME}).ToList()
                    .Distinct().ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000026");
                StationReturn.Data = UserRoleInfoList;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        /// <summary>
        /// 根據EMP_NO查詢該用戶下能管理的角色所擁有的權限    EMP_LEVEL=9 會帶出所有角色, EMP_LEVEL=1 只能帶出本部門角色  EMP_LEVEL=0 無權限管理角色
        /// </summary>
        public void SelectRolePrivilegeByEmpNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_ROLE_PRIVILEGE RolePrivilegeByEmp;
            List<c_role_privilegeinfobyemp> RolePrivilegeByEmpList;
            String LEVEL_FLAG = LoginUser.EMP_LEVEL;
            if (LEVEL_FLAG == "0")
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000028";
                this.DBPools["SFCDB"].Return(sfcdb);
                //  StationReturn.Message = "該用戶無權限管理角色";
                return;
            }
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RolePrivilegeByEmp = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
                RolePrivilegeByEmpList = RolePrivilegeByEmp.QueryRolePrivilegeByEmpNo(LEVEL_FLAG, this.LoginUser.BU, this.LoginUser.FACTORY, this.LoginUser.DPT_NAME, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                // StationReturn.Message = "獲取成功";
                StationReturn.Data = RolePrivilegeByEmpList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 根據傳入角色 除掉登錄人能管理的角色中存在和該角色相同的權限ID
        /// </summary>
        public void CheckTwoRolePrivilegeID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_ROLE_PRIVILEGE CheckRolePrivilege;
            T_C_ROLE GetMangeRole;
            List<c_role_privilegeinfobyemp> CheckRolePrivilegeList;
            List<c_role_byempl> GetMangeRoleList;
            string EDITROLE_ID = Data["EDITROLE_ID"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                CheckRolePrivilege = new T_C_ROLE_PRIVILEGE(sfcdb, DBTYPE);
                GetMangeRole = new T_C_ROLE(sfcdb, DBTYPE);
                GetMangeRoleList = GetMangeRole.ManageRoleByUser(new List<get_c_roleid>(), this.LoginUser.DPT_NAME, this.LoginUser.BU, this.LoginUser.FACTORY, this.LoginUser.EMP_LEVEL, sfcdb);
                if (GetMangeRoleList.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.Message = "無角色能管理";
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145519");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                CheckRolePrivilegeList = CheckRolePrivilege.CheckTwoRolePrivilegeID(GetMangeRoleList, EDITROLE_ID,this.LoginUser.EMP_LEVEL, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                // StationReturn.Message = "獲取成功";
                StationReturn.Data = CheckRolePrivilegeList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }


    }
}
