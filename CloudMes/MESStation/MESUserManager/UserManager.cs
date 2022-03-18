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
//using WebServer.SocketService;

namespace MESStation.MESUserManager
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
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Password", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Language", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "BU_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FCheckToken = new APIInfo()
        {
            FunctionName = "CheckToken",
            Description = "檢查 Token是否有效",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Token", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
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
             //   new APIInputInfo() {InputName = "EMP_LEVEL", InputType = "string", DefaultValue = "" },///創建用戶默認全部為普通用戶 0表示普通用戶，1表示可編輯本部門角色權限的用戶，9表示後台管理，能操作權限相關的任何動作
                new APIInputInfo() {InputName = "DPT_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "POSITION_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAIL_ADDRESS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PHONE_NUMBER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AGENT_EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_DESC", InputType = "string", DefaultValue = "" },
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

        protected APIInfo FUpdatePassword = new APIInfo()
        {
            FunctionName = "UpdatePassword",
            Description = "修改用戶密碼",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "OLDPWD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NEWPWD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CONNEWPWD", InputType = "string", DefaultValue = "" }
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

        protected APIInfo FDeleteInformation = new APIInfo()
        {
            FunctionName = "DeleteInformation",
            Description = "刪除用戶個人信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUpdateInformation = new APIInfo()
        {
            FunctionName = "UpdateInformation",
            Description = "更新用戶個人信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FACTORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_PASSWORD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DPT_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "POSITION_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAIL_ADDRESS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PHONE_NUMBER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AGENT_EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_EN_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CHANGE_PASSWORD_TIME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetMaxPage = new APIInfo()
        {
            FunctionName = "GetMaxPage",
            Description = "根据传进来的数据对C_USER数据进行分页",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PageRow", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Emp_No", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FLoginSMO = new APIInfo()
        {
            FunctionName = "LoginForSMO",
            Description = "SMO用戶登錄，成功返回Token",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "BU_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限

        };


        protected APIInfo FUpdateUserInformation = new APIInfo()
        {
            FunctionName = "UpdateUserInformation",
            Description = "更新用戶個人信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FACTORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_PASSWORD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DPT_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "POSITION_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAIL_ADDRESS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PHONE_NUMBER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AGENT_EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EMP_EN_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CHANGE_PASSWORD_TIME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        public UserManager()
        {            
            this.Apis.Add(FLogin.FunctionName, FLogin);
            this.Apis.Add(FGetPassword.FunctionName, FGetPassword);
            this.Apis.Add(FSelectInformation.FunctionName, FSelectInformation);
            this.Apis.Add(FLoadInformation.FunctionName, FLoadInformation);
            this.Apis.Add(FDeleteInformation.FunctionName, FDeleteInformation);
            this.Apis.Add(FUpdateInformation.FunctionName, FUpdateInformation);
            this.Apis.Add(FCheckToken.FunctionName, FCheckToken);
            this.Apis.Add(FGetMaxPage.FunctionName, FGetMaxPage);
            this.Apis.Add(FCreateNewUser.FunctionName, FCreateNewUser);
            this.Apis.Add(FUpdatePassword.FunctionName, FUpdatePassword);
            this.Apis.Add(FUpdateUserInformation.FunctionName, FUpdateUserInformation);
            this.Apis.Add(FLoginSMO.FunctionName, FLoginSMO);
        }
        /// <summary>
        /// 登錄API
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Login(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string DebugCode = "000";
            OleExec SFCDB = null;
            try
            {
                string EMP_NO = Data["EMP_NO"].ToString();
                string PWD = Data["Password"].ToString();
                string BU_NAME = Data["BU_NAME"].ToString();
                DebugCode = "010";
                DataSet res = new DataSet();
                Language = Data["Language"].ToString();
                MESReturnMessage.Language = Language;
                LoginReturn lr = new LoginReturn();
                SFCDB = _DBPools["SFCDB"].Borrow();
                DebugCode = "020";
                T_c_user GetLoginUser = new T_c_user(SFCDB, this.DBTYPE);
                Row_c_user rcu = (Row_c_user)GetLoginUser.NewRow();
                rcu = GetLoginUser.getC_Userbyempno(EMP_NO, SFCDB, this.DBTYPE);
                T_C_CONTROL t_c_control = new T_C_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
                if (rcu == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000010";
                    _DBPools["SFCDB"].Return(SFCDB);
                    return;
                }
                DebugCode = "030";
                c_user_info user_info = new c_user_info();
                user_info = GetLoginUser.GetLoginUser(EMP_NO, SFCDB);
                DebugCode = "040";
                MESPubLab.MESStation.LogicObject.User lu = new MESPubLab.MESStation.LogicObject.User();
                DebugCode = "050";
                if (PWD == rcu.EMP_PASSWORD)
                {
                    lu.ID = user_info.ID;
                    lu.FACTORY = user_info.FACTORY;
                    //lu.BU = user_info.BU_NAME;
                    lu.BU = Data["BU_NAME"].ToString();
                    lu.EMP_NO = user_info.EMP_NO;
                    lu.EMP_LEVEL = user_info.EMP_LEVEL;
                    lu.DPT_NAME = user_info.DPT_NAME;
                    DebugCode = "060";
                    string token1 = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string token2 = rand.Next(100, 999).ToString();
                    char[] TokenChars = (token1 + token2).ToArray();
                    byte[] TokenBytes = Encoding.Default.GetBytes(TokenChars);
                    string TokenBas64 = Convert.ToBase64String(TokenBytes);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000009";
                    LoginUser = lu;
                    lr = new LoginReturn() { Token = TokenBas64, User_ID = LoginUser.EMP_NO, UserInfo = user_info };
                    var bu = SFCDB.ORM.Queryable<C_BU>().Where(t => t.BU == BU_NAME).First();
                    DebugCode = "070";
                    if (bu == null)
                    {
                        bu = SFCDB.ORM.Queryable<C_BU>().First();
                        if (bu != null)
                        {
                            LoginUser.BU = bu.BU;
                            user_info.BU_NAME = bu.BU;
                        }
                    }
                    DebugCode = "080";

                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000010";
                }
                if (t_c_control.existControlValue("ISCHECKOVERTIME", "ISCHECKOVERTIME", "Y", SFCDB)) {
                    if (BU_NAME == "VERTIV" || BU_NAME == "HWD")
                    {
                        var times = rcu.EDIT_TIME.AddDays(90);
                        if (times < GetDBDateTime())
                        {
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MSGCODE20211216172028";
                        }
                    }
                }
                
                StationReturn.Data = lr;
                DebugCode = "090";
                _DBPools["SFCDB"].Return(SFCDB);
                DebugCode = "100";
            }
            catch (Exception eee)
            {
                throw new Exception(DebugCode + ":" + eee.Message, eee);
            }
            finally
            {
                if (SFCDB != null)
                {
                    _DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }

        /// <summary>
        /// CHECK TOKEN
        /// </summary>
        public void CheckToken(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            try
            {
                c_user_info CheckToken_info = new c_user_info();
                if (this.LoginUser != null)
                {
                    CheckToken_info = new c_user_info
                    {
                        ID = this.LoginUser.ID,
                        FACTORY = this.LoginUser.FACTORY,
                        BU_NAME = this.LoginUser.BU,
                        EMP_NO = this.LoginUser.EMP_NO,
                        EMP_NAME = this.LoginUser.EMP_NAME,
                        EMP_LEVEL = this.LoginUser.EMP_LEVEL,
                        DPT_NAME = this.LoginUser.DPT_NAME
                    };
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000009";
                    StationReturn.Data = CheckToken_info;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000011";
                }
            }
            catch (Exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000011";
            }

        }
        /// <summary>
        /// 創建新用戶
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CreateNewUser(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
           
            OleExec sfcdb = null;
            T_c_user USER;
            try
            {
               if( LoginUser.EMP_LEVEL != "0")
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "You don't have privileges to Create a New User!";
                    return;
                }

                sfcdb = this.DBPools["SFCDB"].Borrow();
                USER = new T_c_user(sfcdb, DBTYPE);
                Row_c_user UserRow = (Row_c_user)USER.NewRow();
                UserRow.ID = USER.GetNewID(BU, sfcdb);
                UserRow.FACTORY = Data["FACTORY"].ToString();
                UserRow.BU_NAME = Data["BU_NAME"].ToString();
                UserRow.EMP_NO = Data["EMP_NO"].ToString();
                UserRow.EMP_PASSWORD = Data["EMP_PASSWORD"].ToString();
                UserRow.EMP_NAME = Data["EMP_NAME"].ToString();
                UserRow.EMP_LEVEL = "0";//Data["EMP_LEVEL"].ToString();///創建用戶默認全部為普通用戶 0表示普通用戶，1表示可編輯本部門角色權限的用戶，9表示後台管理，能操作權限相關的任何動作
                UserRow.DPT_NAME = Data["DPT_NAME"].ToString();
                UserRow.POSITION_NAME = Data["POSITION_NAME"].ToString();
                UserRow.MAIL_ADDRESS = Data["MAIL_ADDRESS"].ToString().ToLower();
                UserRow.PHONE_NUMBER = Data["PHONE_NUMBER"].ToString();
                UserRow.LOCATION = Data["LOCATION"].ToString();
                UserRow.LOCK_FLAG = "N";
                UserRow.AGENT_EMP_NO = Data["AGENT_EMP_NO"].ToString();
                UserRow.CHANGE_PASSWORD_TIME = GetDBDateTime();
                UserRow.EMP_DESC = Data["EMP_DESC"].ToString();
                UserRow.EDIT_TIME = GetDBDateTime();
                UserRow.EDIT_EMP = LoginUser.EMP_NO;
                UserRow.EMP_EN_NAME = Data["EMP_EN_NAME"].ToString();

                if (Data["FACTORY"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "";
                    return;
                }

                if (Data["BU_NAME"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "廠別為空";
                    return;
                }

                if (Data["EMP_NO"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "工號為空";
                    return;
                }
                if (Data["DPT_NAME"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "所屬部門";
                    return;
                }

                if (Data["EMP_NAME"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "員工姓名為空";
                    return;
                }

                ///CHECK 用戶密碼
                string STRPWD = USER.CheckPWD(Data["EMP_PASSWORD"].ToString(), sfcdb);
                if (STRPWD != "Pass")
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = STRPWD;
                    return;
                }

                /// CHECK EMAIL地址
                string STRMAIL = USER.CheckMail(Data["MAIL_ADDRESS"].ToString().ToLower(), sfcdb);
                if (STRMAIL != "Pass")
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = STRMAIL;
                    return;
                }

                ///CHECK 電話號碼
                if (!USER.CheckPhone(Data["PHONE_NUMBER"].ToString(), sfcdb))
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "电话号码填寫格式不正確";
                    return;
                }

                DataTable userstr = new DataTable();
                userstr = USER.SelectC_Userbyempno(UserRow.EMP_NO, sfcdb, this.DBTYPE);
                if (userstr.Rows.Count != 0)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000012";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    string strRet = sfcdb.ExecSQL(UserRow.GetInsertString(this.DBTYPE));

                    if (strRet == "1")
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000013";
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000014";
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 取回用戶密碼
        /// </summary>
        public void GetPassword(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_c_user GetPassword;
            string EMP_NO = Data["EMP_NO"].ToString();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                GetPassword = new T_c_user(sfcdb, DBTYPE);
                Row_c_user row = (Row_c_user)GetPassword.NewRow();
                row = GetPassword.getC_Userbyempno(EMP_NO, sfcdb, this.DBTYPE);

                if (row != null)
                {
                    ///缺發送EMAIL 或者短信功能
                    dic.Add("EMP_PASSWORD", row.EMP_PASSWORD.ToString());
                    StationReturn.MessageCode = "MES00000015";
                    StationReturn.Data = dic;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        /// <summary>
        ///修改用戶密碼
        /// </summary>
        public void UpdatePassword(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_c_user UserInformation;
            string OldPwd = Data["OLDPWD"].ToString();
            string NewPwd = Data["NEWPWD"].ToString();
            string ConNewPwd = Data["CONNEWPWD"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                UserInformation = new T_c_user(sfcdb, DBTYPE);
                Row_c_user row = (Row_c_user)UserInformation.GetObjByID(LoginUser.ID, sfcdb);

                if (OldPwd == row.EMP_PASSWORD)
                {
                    if (NewPwd == ConNewPwd)
                    {
                        row.ID = LoginUser.ID;
                        row.EMP_PASSWORD = ConNewPwd;
                        row.EDIT_EMP = LoginUser.EMP_NO;
                        row.EDIT_TIME = GetDBDateTime();

                        UpdateSql = row.GetUpdateString(DBTYPE);
                        sfcdb.ExecSQL(UpdateSql);
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        //StationReturn.Message = "Pwd Change Success!";
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144709");
                        StationReturn.Data = "";
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        //StationReturn.Message = "前後兩次輸入密碼不一致";
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145106");
                        StationReturn.Data = NewPwd + " <> " + ConNewPwd;
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }

                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.Message = "舊密碼不正確";
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145131");
                    StationReturn.Data = OldPwd;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

    /// <summary>
    /// 查詢用戶信息
    /// </summary>
    public void SelectInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetInformation = new MESDataObject.Module.T_c_user(SFCDB, this.DBTYPE);
            MESDataObject.Module.Row_c_user InformationRow = (MESDataObject.Module.Row_c_user)GetInformation.NewRow();
            try
            {
                string EMP_NO = Data["EMP_NO"].ToString();
                List<c_user_model> LoadUsetInfolsit = new List<c_user_model>();
                LoadUsetInfolsit = GetInformation.SelectUserInfomation(EMP_NO, SFCDB);                                 
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = LoadUsetInfolsit;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                throw e;
            }

        }

        /// <summary>
        /// 刪除用戶信息
        /// </summary>
        public void DeleteInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string strid = "";
            T_c_user DeleteInformation;
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                DeleteInformation = new T_c_user(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    strid = ID[i].ToString();
                    Row_c_user row = (Row_c_user)DeleteInformation.GetObjByID(strid, sfcdb);
                    DeleteSql += row.GetDeleteString(DBTYPE) + ";\n";
                }
                DeleteSql = "begin\n" + DeleteSql + "end;";
                sfcdb.ExecSQL(DeleteSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
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
        /// 更新用戶信息
        /// </summary>
        public void UpdateInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_c_user GetInformation;
            string ID = Data["ID"].ToString();
            string FACTORY = Data["FACTORY"].ToString();
            string BU_NAME = Data["BU_NAME"].ToString();
            string EMP_NO = Data["EMP_NO"].ToString();
            string EMP_PASSWORD = Data["EMP_PASSWORD"].ToString();
            string EMP_NAME = Data["EMP_NAME"].ToString();
            string DPT_NAME = Data["DPT_NAME"].ToString();
            string POSITION_NAME = Data["POSITION_NAME"].ToString();
            string MAIL_ADDRESS = Data["MAIL_ADDRESS"].ToString();
            string PHONE_NUMBER = Data["PHONE_NUMBER"].ToString();
            string LOCATION = Data["LOCATION"].ToString();
            string AGENT_EMP_NO = Data["AGENT_EMP_NO"].ToString();
            string EMP_DESC = Data["EMP_DESC"].ToString();
            string EMP_EN_NAME = Data["EMP_EN_NAME"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                GetInformation = new T_c_user(sfcdb, DBTYPE);
                Row_c_user row = (Row_c_user)GetInformation.GetObjByID(Data["ID"].ToString(), sfcdb);
                row.ID = ID;
                row.FACTORY = FACTORY;
                row.BU_NAME = BU_NAME;
                row.EMP_NO = EMP_NO;
                row.EMP_PASSWORD = EMP_PASSWORD;
                row.EMP_NAME = EMP_NAME;
                row.DPT_NAME = DPT_NAME;
                row.POSITION_NAME = POSITION_NAME;
                row.MAIL_ADDRESS = MAIL_ADDRESS;
                row.PHONE_NUMBER = PHONE_NUMBER;
                row.LOCATION = LOCATION;
                row.AGENT_EMP_NO = AGENT_EMP_NO;
                row.EMP_DESC = EMP_DESC;
                row.EMP_EN_NAME = EMP_EN_NAME;
                row.EDIT_TIME = GetDBDateTime();
                row.CHANGE_PASSWORD_TIME = GetDBDateTime();

                ///CHECK 用戶密碼
                string STRPWD = GetInformation.CheckPWD(EMP_PASSWORD, sfcdb);
                if (STRPWD != "Pass")
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = STRPWD;
                    return;
                }

                /// CHECK EMAIL地址
                string STRMAIL = GetInformation.CheckMail(MAIL_ADDRESS.ToLower(), sfcdb);
                if (STRMAIL != "Pass")
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = STRMAIL;
                    return;
                }

                ///CHECK 電話號碼
                if (!GetInformation.CheckPhone(PHONE_NUMBER, sfcdb))
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.Message = "电话号码填寫格式不正確";
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145156");
                    return;
                }

                UpdateSql = row.GetUpdateString(DBTYPE);
                sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// 加載所有用戶列表
        /// </summary>
        public void LoadInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_c_user LoadUsetInfo;
            List<c_user_model> LoadUsetInfolsit = new List<c_user_model>();
            string r = Data["PageRow"].ToString() == "" ? "10" : Data["PageRow"].ToString();
            string i = Data["PageCount"].ToString() == "" ? "1" : Data["PageCount"].ToString();
            int PageRow = Convert.ToInt32(r == "0" ? "10" : r);
            int PageCount = Convert.ToInt32(i == "0" ? "1" : i);
            string emp_no = Data["emp_no"].ToString();//需要查询人的工号
            int MaxPage;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                LoadUsetInfo = new T_c_user(sfcdb, DBTYPE);
                MaxPage = LoadUsetInfo.CheckMaxPage(PageRow,emp_no, sfcdb);
                if (MaxPage >= PageCount)
                {
                    LoadUsetInfolsit = LoadUsetInfo.SelectUserInfo(PageRow, PageCount, emp_no,this.LoginUser.EMP_LEVEL,this.LoginUser.DPT_NAME,this.LoginUser.EMP_NO,this.LoginUser.FACTORY,this.LoginUser.BU, sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000017";
                    StationReturn.Data = LoadUsetInfolsit;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {

                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000018";
                    StationReturn.MessagePara.Add(PageCount);
                    StationReturn.MessagePara.Add(MaxPage);
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }


        /// <summary>
        /// 根据页面传进来的每页数据行数加載出最大页数
        /// </summary>
        public void GetMaxPage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_c_user LoadUsetInfo;
            List<c_user_model> LoadUsetInfolsit = new List<c_user_model>();
            string r = Data["PageRow"].ToString() == "" ? "10" : Data["PageRow"].ToString();
            string emp_no = Data["Emp_No"].ToString();
            int PageRow = Convert.ToInt32(r == "0" ? "10" : r);
            int MaxPage;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                LoadUsetInfo = new T_c_user(sfcdb, DBTYPE);
                MaxPage = LoadUsetInfo.CheckMaxPage(PageRow, emp_no, sfcdb);

                StationReturn.Status = StationReturnStatusValue.Pass;
                //StationReturn.Message = "获取页数成功";
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145225");
                StationReturn.Data = MaxPage;
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
            
        }

        public void LoginForSMO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string EMP_NO = Data["EMP_NO"].ToString();
            string BU_NAME = Data["BU_NAME"].ToString();
            //string Station_Name = Data["Station"].ToString();
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            MESDataObject.Module.T_c_user GetLoginUser = new MESDataObject.Module.T_c_user(SFCDB, this.DBTYPE);
            MESDataObject.Module.Row_c_user rcu = (Row_c_user)GetLoginUser.NewRow();
            if (GetLoginUser.getC_Userbyempno(EMP_NO, SFCDB, this.DBTYPE) == null)
            {
                GetLoginUser = new T_c_user(SFCDB, DBTYPE);
                Row_c_user UserRow = (Row_c_user)GetLoginUser.NewRow();
                UserRow.ID = GetLoginUser.GetNewID(BU, SFCDB);
                UserRow.FACTORY ="TEST";
                UserRow.BU_NAME = "TEST";
                UserRow.EMP_NO = EMP_NO;
                UserRow.EMP_PASSWORD = EMP_NO;
                UserRow.EMP_NAME = "TEST";
                UserRow.EMP_LEVEL = "0";//Data["EMP_LEVEL"].ToString();///創建用戶默認全部為普通用戶 0表示普通用戶，1表示可編輯本部門角色權限的用戶，9表示後台管理，能操作權限相關的任何動作
                UserRow.DPT_NAME = "TEST";
                UserRow.POSITION_NAME = "TEST";
                UserRow.MAIL_ADDRESS = "TEST";
                UserRow.PHONE_NUMBER = "TEST";
                UserRow.LOCATION = "TEST";
                UserRow.LOCK_FLAG = "N";
                UserRow.AGENT_EMP_NO = "TEST";
                UserRow.CHANGE_PASSWORD_TIME = GetDBDateTime();
                UserRow.EMP_DESC = "TEST";
                UserRow.EDIT_TIME = GetDBDateTime();
                UserRow.EDIT_EMP = "TEST";
                UserRow.EMP_EN_NAME = "TEST";

                SFCDB.ExecSQL(UserRow.GetInsertString(this.DBTYPE));
            }
            c_user_info user_info = new c_user_info();
            user_info = GetLoginUser.GetLoginUser(EMP_NO, SFCDB);

            MESPubLab.MESStation.LogicObject.User lu = new MESPubLab.MESStation.LogicObject.User();
            lu.ID = user_info.ID;
            lu.FACTORY = user_info.FACTORY;
            lu.BU = user_info.BU_NAME;
            lu.EMP_NO = user_info.EMP_NO;
            lu.EMP_LEVEL = user_info.EMP_LEVEL;
            lu.DPT_NAME = user_info.DPT_NAME;
            string token1 = DateTime.Now.ToString("yyyyMMddHHmmss");
            string token2 = rand.Next(100, 999).ToString();
            char[] TokenChars = (token1 + token2).ToArray();
            byte[] TokenBytes = Encoding.Default.GetBytes(TokenChars);
            string TokenBas64 = Convert.ToBase64String(TokenBytes);
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000009";
            LoginUser = lu;
            LoginReturn lr = new LoginReturn() { Token = TokenBas64, User_ID = LoginUser.EMP_NO };
            StationReturn.Data = lr;
            _DBPools["SFCDB"].Return(SFCDB);
        }
        /// <summary>
        /// 更新EMP信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateUserInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_c_user UserInformation;
            string UpdateSql = "";
            string ID = Data["ID"].ToString();
            string FACTORY = Data["FACTORY"].ToString();
            string BU_NAME = Data["BU_NAME"].ToString();
            string EMP_NO = Data["EMP_NO"].ToString();
            string EMP_PASSWORD = Data["EMP_PASSWORD"].ToString();
            string EMP_NAME = Data["EMP_NAME"].ToString();
            string DPT_NAME = Data["DPT_NAME"].ToString();
            string POSITION_NAME = Data["POSITION_NAME"].ToString();
            string MAIL_ADDRESS = Data["MAIL_ADDRESS"].ToString();
            string PHONE_NUMBER = Data["PHONE_NUMBER"].ToString();
            string LOCATION = Data["LOCATION"].ToString();
            string AGENT_EMP_NO = Data["AGENT_EMP_NO"].ToString();
            string EMP_DESC = Data["EMP_DESC"].ToString();
            string EMP_EN_NAME = Data["EMP_EN_NAME"].ToString();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                UserInformation = new T_c_user(sfcdb, DBTYPE);
                Row_c_user row = (Row_c_user)UserInformation.GetObjByID(Data["ID"].ToString(), sfcdb);
                row.ID = ID;
                row.FACTORY = FACTORY;
                row.BU_NAME = BU_NAME;
                row.EMP_NO = EMP_NO;
                row.EMP_PASSWORD = EMP_PASSWORD;
                row.EMP_NAME = EMP_NAME;
                row.DPT_NAME = DPT_NAME;
                row.POSITION_NAME = POSITION_NAME;
                row.MAIL_ADDRESS = MAIL_ADDRESS;
                row.PHONE_NUMBER = PHONE_NUMBER;
                row.LOCATION = LOCATION;
                row.AGENT_EMP_NO = AGENT_EMP_NO;
                row.EMP_DESC = EMP_DESC;
                row.EMP_EN_NAME = EMP_EN_NAME;
                row.EDIT_TIME = GetDBDateTime();
                row.CHANGE_PASSWORD_TIME = GetDBDateTime();

                ///CHECK 用戶密碼
                //string STRPWD = UserInformation.CheckPWD(EMP_PASSWORD, sfcdb);
                //if (STRPWD != "Pass")
                //{
                //    this.DBPools["SFCDB"].Return(sfcdb);
                //    StationReturn.Status = StationReturnStatusValue.Fail;
                //    StationReturn.Message = STRPWD;
                //    return;
                //}

                /// CHECK EMAIL地址
                string STRMAIL = UserInformation.CheckMail(MAIL_ADDRESS.ToLower(), sfcdb);
                if (STRMAIL != "Pass")
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.Message = STRMAIL;
                    //StationReturn.Message = "Email填寫格式不正確";
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145253");
                    return;
                }

                ///CHECK 電話號碼
                if (!UserInformation.CheckPhone(PHONE_NUMBER, sfcdb))
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.Message = "电话号码填寫格式不正確";
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145317");
                    return;
                }

                UpdateSql = row.GetUpdateString(DBTYPE);
                sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

    }
}
