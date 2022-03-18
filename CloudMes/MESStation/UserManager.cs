using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject;
using System.Data;
using MESDataObject.Module;
using MESPubLab.MESStation.MESReturnView.Public;
using MESPubLab.MESStation.LogicObject;

namespace MESStation
{
    public class UserManager : MesAPIBase
    {
        static Random rand = new Random();
        ///List<APIInfo> TCodes = new List<APIInfo>();
        protected APIInfo FLogin = new APIInfo()
        {
            FunctionName = "Login",
            Description = "用戶登錄，成功返回Token",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "User_Name", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Password", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Language", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FCreateNewUser = new APIInfo()
        {
            FunctionName = "CreateNewUser",
            Description = "創建新用戶",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FACTORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_PASSWORD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_LEVEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DPT_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "POSITION_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAIL_ADDRESS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PHONE_NUMBER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AGENT_EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_EN_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { new MESPermission(0) }//需要 創建用戶 權限
        };

        protected APIInfo FGetPassword = new APIInfo()
        {
            FunctionName = "GetPassword",
            Description = "取回用戶密碼，并發送至用戶郵箱",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectInformation = new APIInfo()
        {
            FunctionName = "SelectInformation",
            Description = "查詢用戶個人信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteInformation = new APIInfo()
        {
            FunctionName = "DeleteInformation",
            Description = "刪除用戶個人信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUpdateInformation = new APIInfo()
        {
            FunctionName = "UpdateInformation",
            Description = "更新用戶個人信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_PASSWORD", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };



        protected APIInfo FCreateRole = new APIInfo()
        {
            FunctionName = "CreateRole",
            Description = "創建角色",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SYSTEM_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_DESC", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" }
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

        protected APIInfo FUpdateRole = new APIInfo()
        {
            FunctionName = "UpdateRole",
            Description = "更新角色ID信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_DESC", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteRole = new APIInfo()
        {
            FunctionName = "DeleteRole",
            Description = "刪除角色ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ROLE_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddRolePrivilege = new APIInfo()
        {
            FunctionName = "AddRolePrivilege",
            Description = "添加某一角色能擁有的權限",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SYSTEM_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROLE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FCreatePrivilegeID = new APIInfo()
        {
            FunctionName = "CreatePrivilegeID",
            Description = "創建站位權限對用的ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SYSTEM_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectPrivilegeID = new APIInfo()
        {
            FunctionName = "SelectPrivilegeID",
            Description = "查詢站位權限對用的ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SYSTEM_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FLoadInformation = new APIInfo()
        {
            FunctionName = "LoadInformation",
            Description = "查詢站位權限對用的ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PageRow", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PageCount", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "emp_no", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectUserRolePrivilege = new APIInfo()
        {
            FunctionName = "SelectUserRolePrivilege",
            Description = "查詢用戶角色權限",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmp", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetMenu = new APIInfo()
        {
            FunctionName = "GetMenu",
            Description = "查詢用戶菜單",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };


        protected APIInfo FCreatMenuId = new APIInfo()
        {
            FunctionName = "CreatMenuId",
            Description = "創建系統菜單配置ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "MENU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LANGUAGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_PATH", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARENT_CODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SORT", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STYLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LANGUAGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MENU_DESC", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };



        public UserManager()
        {
            this.Apis.Add(FCreateNewUser.FunctionName, FCreateNewUser);
            this.Apis.Add(FLogin.FunctionName, FLogin);
            this.Apis.Add(FGetPassword.FunctionName, FGetPassword);
            this.Apis.Add(FSelectInformation.FunctionName, FSelectInformation);
            this.Apis.Add(FCreateRole.FunctionName, FCreateRole);
            this.Apis.Add(FSelectRole.FunctionName, FSelectRole);
            this.Apis.Add(FUpdateRole.FunctionName, FUpdateRole);
            this.Apis.Add(FDeleteRole.FunctionName, FDeleteRole);
            this.Apis.Add(FAddRolePrivilege.FunctionName, FAddRolePrivilege);
            this.Apis.Add(FCreatePrivilegeID.FunctionName, FCreatePrivilegeID);
            this.Apis.Add(FSelectPrivilegeID.FunctionName, FSelectPrivilegeID);
            this.Apis.Add(FLoadInformation.FunctionName, FLoadInformation);
            this.Apis.Add(FDeleteInformation.FunctionName, FDeleteInformation);
            this.Apis.Add(FUpdateInformation.FunctionName, FUpdateInformation);
            this.Apis.Add(FSelectUserRolePrivilege.FunctionName, FSelectUserRolePrivilege);
            this.Apis.Add(FGetMenu.FunctionName, FGetMenu);
        }

        /// <summary>
        /// 登錄API
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Login(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string User_Name = Data["User_Name"].ToString();/// EMP_NAME  IS EMP_NO
            string PWD = Data["Password"].ToString();
            DataSet res = new DataSet();
            Language = Data["Language"].ToString();
            MESReturnMessage.Language = Language;
            LoginReturn lr = new LoginReturn();
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetLoginInformation = new MESDataObject.Module.T_c_user(SFCDB, this.DBTYPE);
            //   string strSql = $@" select * from C_USER where EMP_NO='{User_Name}' and EMP_PASSWORD='{PWD}' ";
            //MESDataObject.Module.Row_c_user user = GetPassword.getC_Userbyempno("TEST",SFCDB,this.DBTYPE);

            DataTable dt = GetLoginInformation.SelectC_Userbyempno(User_Name, SFCDB, this.DBTYPE);
            List<c_user_model> lsit = new List<c_user_model>();
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow item in dt.Rows)
                {
                    lsit.Add(new c_user_model
                    {
                        ID = item["ID"].ToString(),
                        FACTORY = item["FACTORY"].ToString(),
                        BU_NAME = item["BU_NAME"].ToString(),
                        EMP_NO = item["EMP_NO"].ToString(),
                        EMP_NAME = item["EMP_NAME"].ToString(),
                        EMP_PASSWORD = item["EMP_PASSWORD"].ToString(),
                        EMP_LEVEL = item["EMP_LEVEL"].ToString(),
                        DPT_NAME = item["DPT_NAME"].ToString(),
                        POSITION_NAME = item["POSITION_NAME"].ToString(),
                        MAIL_ADDRESS = item["MAIL_ADDRESS"].ToString(),
                        PHONE_NUMBER = item["PHONE_NUMBER"].ToString(),
                        LOCATION = item["LOCATION"].ToString(),
                        LOCK_FLAG = item["LOCK_FLAG"].ToString(),
                        AGENT_EMP_NO = item["AGENT_EMP_NO"].ToString(),
                        EMP_DESC = item["EMP_DESC"].ToString(),
                        EDIT_EMP = item["EDIT_EMP"].ToString(),
                        EMP_EN_NAME = item["EMP_EN_NAME"].ToString()
                    }
                    );
                }

            }

            User lu = new User();
            //if (user.EMP_PASSWORD == user.EMP_PASSWORD)
            if (true)
            {
                string token1 = DateTime.Now.ToString("yyyyMMddHHmmss");
                string token2 = rand.Next(100, 999).ToString();
                char[] TokenChars = (token1 + token2).ToArray();
                byte[] TokenBytes = Encoding.Default.GetBytes(TokenChars);
                string TokenBas64 = Convert.ToBase64String(TokenBytes);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000009");//"登錄成功";
                LoginUser = lu;
                //lr = new LoginReturn() { Token = TokenBas64, User_ID = user.EMP_NO};
                lr = new LoginReturn() { Token = TokenBas64, User_ID = LoginUser.EMP_NO };
            }
            else
            {
                //StationReturn.Status = StationReturnStatusValue.Fail;
                //StationReturn.Message = "登錄失敗！密碼錯誤或用戶名不存在！！";
            }
            StationReturn.Data = lr;
            _DBPools["SFCDB"].Return(SFCDB);

        }
        /// <summary>
        /// 創建新用戶
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CreateNewUser(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user USER = new MESDataObject.Module.T_c_user(SFCDB, DB_TYPE_ENUM.Oracle);

            MESDataObject.Module.Row_c_user UserRow = (MESDataObject.Module.Row_c_user)USER.NewRow();

            UserRow.ID = USER.GetNewID(BU, SFCDB);
            UserRow.FACTORY = Data["FACTORY"].ToString();
            UserRow.BU_NAME = Data["BU_NAME"].ToString();
            UserRow.EMP_NO = Data["EMP_NO"].ToString();
            UserRow.EMP_PASSWORD = Data["EMP_PASSWORD"].ToString();
            UserRow.EMP_NAME = Data["EMP_NAME"].ToString();
            UserRow.EMP_LEVEL = Data["EMP_LEVEL"].ToString();
            UserRow.DPT_NAME = Data["DPT_NAME"].ToString();
            UserRow.POSITION_NAME = Data["POSITION_NAME"].ToString();
            UserRow.MAIL_ADDRESS = Data["MAIL_ADDRESS"].ToString();
            UserRow.PHONE_NUMBER = Data["PHONE_NUMBER"].ToString();
            UserRow.LOCATION = Data["LOCATION"].ToString();
            UserRow.LOCK_FLAG = "N";
            UserRow.AGENT_EMP_NO = Data["AGENT_EMP_NO"].ToString();
            UserRow.CHANGE_PASSWORD_TIME = DateTime.Now;
            UserRow.EMP_DESC = Data["EMP_DESC"].ToString();
            UserRow.EDIT_TIME = DateTime.Now;
            UserRow.EDIT_EMP = Data["EDIT_EMP"].ToString();
            UserRow.EMP_EN_NAME = Data["EMP_EN_NAME"].ToString();

            DataTable userstr = new DataTable();
            userstr = USER.SelectC_Userbyempno(UserRow.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);
            if (userstr.Rows.Count != 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000012");// "註冊失敗,已存在該用戶";
            }
            else
            {
                string strRet = SFCDB.ExecSQL(UserRow.GetInsertString(DB_TYPE_ENUM.Oracle));

                if (strRet == "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000013");// "註冊成功！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000014"); //"註冊失敗！";
                }
            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 取回用戶密碼
        /// </summary>
        public void GetPassword(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetPassword = new MESDataObject.Module.T_c_user(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_c_user Resetrow = (MESDataObject.Module.Row_c_user)GetPassword.NewRow();

            string EMP_NO = Data["EMP_NO"].ToString();
            Dictionary<string, string> dic = new Dictionary<string, string>();

            Resetrow = GetPassword.getC_Userbyempno(EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);

            if (Resetrow != null)
            {
                ///缺發送EMAIL 或者短信功能
                dic.Add("EMP_PASSWORD", Resetrow.EMP_PASSWORD.ToString());
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000015");// "取回成功,密碼已發送至郵箱！";
                StationReturn.Data = dic;
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155113");// "無此用戶！";
            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 查詢用戶信息
        /// </summary>
        public void SelectInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetInformation = new MESDataObject.Module.T_c_user(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_c_user InformationRow = (MESDataObject.Module.Row_c_user)GetInformation.NewRow();

            string EMP_NO = Data["EMP_NO"].ToString();
            DataTable dt = GetInformation.SelectC_Userbyempno(EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dic.Add(dt.Columns[i].ColumnName, dt.Rows[0][i].ToString());
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000016");// "成功取到用戶信息！";
                StationReturn.Data = dic;
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155113"); //"無此用戶！";
            }

            this.DBPools["SFCDB"].Return(SFCDB);

        }

        /// <summary>
        /// 刪除用戶信息
        /// </summary>
        public void DeleteInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetInformation = new MESDataObject.Module.T_c_user(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_c_user InformationRow = (MESDataObject.Module.Row_c_user)GetInformation.NewRow();

            string EMP_NO = "";
            List<string> list = new List<string>();
            for (int i = 0; i < Data["EMP_NO"].Count(s => s != null); i++)
            {
                list.Add(Data["EMP_NO"][i].ToString());
                EMP_NO += "'" + Data["EMP_NO"][i].ToString() + "',";
            }
            EMP_NO = EMP_NO.TrimEnd(',');
            InformationRow = (MESDataObject.Module.Row_c_user)GetInformation.getC_Userbyempno(EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);


            if (InformationRow != null)
            {
                string strRet = SFCDB.ExecSQL(InformationRow.GetDeleteString(DB_TYPE_ENUM.Oracle));

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功//"成功刪除用戶信息！";
                StationReturn.Data = "";
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155113"); //"無此用戶！";
            }

            this.DBPools["SFCDB"].Return(SFCDB);

        }

        /// <summary>
        /// 更新用戶信息
        /// </summary>
        public void UpdateInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetInformation = new MESDataObject.Module.T_c_user(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_c_user InformationRow = (MESDataObject.Module.Row_c_user)GetInformation.NewRow();

            string EMP_NO = Data["EMP_NO"].ToString();
            string EMP_PASSWORD = Data["EMP_PASSWORD"].ToString();

            InformationRow = (MESDataObject.Module.Row_c_user)GetInformation.getC_Userbyempno(EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);


            if (InformationRow != null)
            {
                InformationRow.EMP_PASSWORD = Data["EMP_PASSWORD"].ToString();
                string strRet = SFCDB.ExecSQL(InformationRow.GetUpdateString(DB_TYPE_ENUM.Oracle));
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功"成功更新用戶信息！";
                StationReturn.Data = "";
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155113"); //"無此用戶！";
            }
            this.DBPools["SFCDB"].Return(SFCDB);

        }

        /// <summary>
        /// 加載所有用戶列表
        /// </summary>
        public void LoadInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetInformation = new MESDataObject.Module.T_c_user(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_c_user InformationRow = (MESDataObject.Module.Row_c_user)GetInformation.NewRow();
            string r = Data["PageRow"].ToString() == "" ? "10" : Data["PageRow"].ToString();
            string i = Data["PageCount"].ToString() == "" ? "1" : Data["PageCount"].ToString();
            int PageRow = Convert.ToInt32(r == "0" ? "10" : r);
            int PageCount = Convert.ToInt32(i == "0" ? "1" : i);
            string emp_no = Data["emp_no"].ToString();

            string strSql = $@"select * from c_user order by  edit_time  ";

            System.Data.DataSet res = SFCDB.ExecSelect(strSql);

            int MaxPage = res.Tables[0].Rows.Count / PageRow + 1;
            if (MaxPage >= PageCount)
            {
                DataTable dt = GetInformation.SelectC_User(PageRow, PageCount, emp_no, SFCDB, DB_TYPE_ENUM.Oracle);

                if (dt.Rows.Count > 0)
                {
                    List<c_user_model> lsit = new List<c_user_model>();
                    foreach (DataRow item in dt.Rows)
                    {
                        lsit.Add(new c_user_model
                        {
                            ID = item["ID"].ToString(),
                            FACTORY = item["FACTORY"].ToString(),
                            BU_NAME = item["BU_NAME"].ToString(),
                            EMP_NO = item["EMP_NO"].ToString(),
                            EMP_NAME = item["EMP_NAME"].ToString(),
                            EMP_PASSWORD = item["EMP_PASSWORD"].ToString(),
                            EMP_LEVEL = item["EMP_LEVEL"].ToString(),
                            DPT_NAME = item["DPT_NAME"].ToString(),
                            POSITION_NAME = item["POSITION_NAME"].ToString(),
                            MAIL_ADDRESS = item["MAIL_ADDRESS"].ToString(),
                            PHONE_NUMBER = item["PHONE_NUMBER"].ToString(),
                            LOCATION = item["LOCATION"].ToString(),
                            LOCK_FLAG = item["LOCK_FLAG"].ToString(),
                            AGENT_EMP_NO = item["AGENT_EMP_NO"].ToString(),
                            EMP_DESC = item["EMP_DESC"].ToString(),
                            EDIT_EMP = item["EDIT_EMP"].ToString(),
                            EMP_EN_NAME = item["EMP_EN_NAME"].ToString()
                        });
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000016");// "成功取到用戶信息！";
                    StationReturn.Data = lsit;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155113"); //"無此用戶！";
                }

            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155451", new string[] { PageCount.ToString(), MaxPage.ToString() });// "所需分頁數(" + PageCount + ")大於最大分頁(" + MaxPage + ")";
            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 創建角色ID
        /// </summary>
        public void CreateRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_ROLE Roler = new MESDataObject.Module.T_C_ROLE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_ROLE RoleRow = (MESDataObject.Module.Row_C_ROLE)Roler.NewRow();

            string Role_Name = Data["ROLE_NAME"].ToString();

            DataTable StrRes = new DataTable();
            StrRes = Roler.getC_Rolebyrolename(Role_Name, SFCDB, DB_TYPE_ENUM.Oracle);

            if (StrRes.Rows.Count != 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155521");// "已存在該角色";
            }
            else
            {
                RoleRow.ID = Roler.GetNewID(BU, SFCDB);
                RoleRow.SYSTEM_NAME = Data["SYSTEM_NAME"].ToString(); ;
                RoleRow.ROLE_NAME = Role_Name;
                RoleRow.ROLE_DESC = Data["ROLE_DESC"].ToString();
                RoleRow.EDIT_TIME = DateTime.Now;
                RoleRow.EDIT_EMP = Data["EDIT_EMP"].ToString();

                string strRet = SFCDB.ExecSQL(RoleRow.GetInsertString(DB_TYPE_ENUM.Oracle));

                if (strRet == "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155549");// "創建角色成功！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155613");// "創建角色失敗！";
                }

            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 查詢角色ID
        /// </summary>
        public void SelectRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_ROLE Roler = new MESDataObject.Module.T_C_ROLE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_ROLE RoleRow = (MESDataObject.Module.Row_C_ROLE)Roler.NewRow();

            string Role_Name = Data["ROLE_NAME"].ToString();

            DataTable StrRes = new DataTable();
            StrRes = Roler.getC_Rolebyrolename(Role_Name, SFCDB, DB_TYPE_ENUM.Oracle);
            List<c_role> rolelist = new List<c_role>();

            if (StrRes.Rows.Count > 0)
            {
                foreach (DataRow item in StrRes.Rows)
                {
                    rolelist.Add(new c_role
                    {
                        ID = item["ID"].ToString(),
                        SYSTEM_NAME = item["SYSTEM_NAME"].ToString(),
                        ROLE_NAME = item["ROLE_NAME"].ToString(),
                        ROLE_DESC = item["ROLE_DESC"].ToString(),
                        EDIT_EMP = item["EDIT_EMP"].ToString()
                    });
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155643");// "角色信息";
                StationReturn.Data = rolelist;
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155658"); //"無任何角色信息！";
            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 更新角色ID信息
        /// </summary>
        public void UpdateRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_ROLE GetRoleInformation = new MESDataObject.Module.T_C_ROLE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_ROLE RoleRow = (MESDataObject.Module.Row_C_ROLE)GetRoleInformation.NewRow();

            string ROLE_NAME = Data["ROLE_NAME"].ToString();
            string ROLE_DESC = Data["ROLE_DESC"].ToString();

            RoleRow = (MESDataObject.Module.Row_C_ROLE)GetRoleInformation.SELECTC_Rolebyrolename(ROLE_NAME, SFCDB, DB_TYPE_ENUM.Oracle);


            if (RoleRow != null)
            {
                RoleRow.ROLE_DESC = Data["ROLE_DESC"].ToString();
                RoleRow.EDIT_EMP = Data["EDIT_EMP"].ToString();
                RoleRow.EDIT_TIME = DateTime.Now;
                string strRet = SFCDB.ExecSQL(RoleRow.GetUpdateString(DB_TYPE_ENUM.Oracle));
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //编辑成功"成功更新角色信息！";
                StationReturn.Data = "";
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155744"); //"無此角色信息！";
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 刪除角色ID信息
        /// </summary>
        public void DeleteRole(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_ROLE GetRoleInformation = new MESDataObject.Module.T_C_ROLE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_ROLE RoleRow = (MESDataObject.Module.Row_C_ROLE)GetRoleInformation.NewRow();

            string EMP_NO = Data["EMP_NO"].ToString();

            RoleRow = (MESDataObject.Module.Row_C_ROLE)GetRoleInformation.SELECTC_Rolebyrolename(EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);


            if (RoleRow != null)
            {
                string strRet = SFCDB.ExecSQL(RoleRow.GetDeleteString(DB_TYPE_ENUM.Oracle));

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112057");  //删除成功;
                StationReturn.Data = "";
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155113"); //"無此用戶！";
            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }
        /// <summary>
        ///添加角色所擁有的權限ID
        /// </summary>
        public void AddRolePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_ROLE_PRIVILEGE Roler = new MESDataObject.Module.T_C_ROLE_PRIVILEGE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_ROLE_PRIVILEGE RoleRow = (MESDataObject.Module.Row_C_ROLE_PRIVILEGE)Roler.NewRow();

            string Role_Name = Data["ROLE_NAME"].ToString();

            DataTable StrRes = new DataTable();

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 創建權限對應的ID
        /// </summary>
        public void CreatePrivilegeID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_PRIVILEGE RolerPrivilege = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_PRIVILEGE RolerPrivilegeRow = (MESDataObject.Module.Row_C_PRIVILEGE)RolerPrivilege.NewRow();

            string PRIVILEGE_ID = Data["PRIVILEGE_ID"].ToString();
            string PRIVILEGE_NAME = Data["PRIVILEGE_NAME"].ToString();

            DataTable StrRes = new DataTable();
            StrRes = RolerPrivilege.CheckPrivilegeID(PRIVILEGE_ID, PRIVILEGE_NAME, SFCDB, this.DBTYPE);
            if (StrRes.Rows.Count != 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155758");// "創建權限ID失敗！";
            }
            else
            {
                RolerPrivilegeRow.ID = RolerPrivilege.GetNewID(BU, SFCDB);
                RolerPrivilegeRow.MENU_ID = Data["MENU_ID"].ToString();
                RolerPrivilegeRow.PRIVILEGE_NAME = Data["PRIVILEGE_NAME"].ToString();
                RolerPrivilegeRow.PRIVILEGE_DESC = Data["PRIVILEGE_DESC"].ToString();
                RolerPrivilegeRow.EDIT_TIME = DateTime.Now;
                RolerPrivilegeRow.EDIT_EMP = Data["EDIT_EMP"].ToString();

                string STRRES = SFCDB.ExecSQL(RolerPrivilegeRow.GetInsertString(this.DBTYPE));

                if (STRRES == "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155931");//"創建權限ID成功！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155946");// "權限ID已存在！";
                }

            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 查詢權限對應的ID
        /// </summary>
        public void SelectPrivilegeID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            LoginReturn lr = new LoginReturn();
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_PRIVILEGE RolerPrivilege = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_PRIVILEGE RolerPrivilegeRow = (MESDataObject.Module.Row_C_PRIVILEGE)RolerPrivilege.NewRow();

            DataTable TablePrivilege = new DataTable();
            TablePrivilege = RolerPrivilege.SelectPrivilegeID(SFCDB, DB_TYPE_ENUM.Oracle);

            List<Privilegesid> Privilegesid = new List<Privilegesid>();
            if (TablePrivilege.Rows.Count > 0)
            {
                foreach (DataRow item in TablePrivilege.Rows)
                {
                    List<string> menu = new List<string>();

                    Privilegesid.Add(new Privilegesid
                    {
                        PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                        PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                        PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()
                    });
                }
            }
            else
                Privilegesid.Add(null);

            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Message = "Get Data Success!";
            StationReturn.Data = Privilegesid;

            this.DBPools["SFCDB"].Return(SFCDB);
        }


        /// <summary>
        /// 查詢用戶角色權限
        /// </summary>
        public void SelectUserRolePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user User = new MESDataObject.Module.T_c_user(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_c_user UserRow = (MESDataObject.Module.Row_c_user)User.NewRow();

            string LoginUserEmp = Data["LoginUserEmp"].ToString();
            string EditEmp = Data["LoginUserEmp"].ToString();


            DataTable StrRes = new DataTable();
            //  StrRes = UserRow.getC_Rolebyrolename(Role_Name, SFCDB, DB_TYPE_ENUM.Oracle);
            List<c_role> rolelist = new List<c_role>();

            if (StrRes.Rows.Count > 0)
            {
                foreach (DataRow item in StrRes.Rows)
                {
                    rolelist.Add(new c_role
                    {
                        ID = item["ID"].ToString(),
                        SYSTEM_NAME = item["SYSTEM_NAME"].ToString(),
                        ROLE_NAME = item["ROLE_NAME"].ToString(),
                        ROLE_DESC = item["ROLE_DESC"].ToString(),
                        EDIT_EMP = item["EDIT_EMP"].ToString()
                    });
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155643");// "角色信息";
                StationReturn.Data = rolelist;
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155658"); //"無任何角色信息！";
            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        //public void GetMenu(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    string User_Name = Data["EMP_NO"].ToString();
        //    MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
        //    try
        //    {
        //        LogicObject.Privilege lu = new LogicObject.Privilege();
        //        List<Privileges> pi= lu.GetMenu(User_Name, SFCDB);
        //        StationReturn.Status = StationReturnStatusValue.Pass;
        //        StationReturn.Message = "获取权限列表成功！！！";
        //        StationReturn.Data = pi;
        //    }
        //    catch(Exception ex)
        //    {
        //        StationReturn.Status = StationReturnStatusValue.Fail;
        //        StationReturn.Message = "获取权限列表失败！！！该用户可能没有任何权限！！！";
        //        StationReturn.Data = ex.Message;
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(SFCDB);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Logout(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            StationReturn.Status = StationReturnStatusValue.Pass;
        }
    }
}
