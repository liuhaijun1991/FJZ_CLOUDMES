using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Public;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MESStation.MESUserManager
{
    class UserLogin : MesAPIBase
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

        protected APIInfo FCheckPermission = new APIInfo()
        {
            FunctionName = "CheckPermission",
            Description = "Check User Permission",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "USER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PWD", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PERMISSION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限

        };


        public UserLogin()
        {
            this.Apis.Add(FLogin.FunctionName, FLogin);
        }


        public void Login(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string EMP_NO = Data["EMP_NO"].ToString();
            string PWD = Data["Password"].ToString();
            string BU_NAME = Data["BU_NAME"].ToString();
            DataSet res = new DataSet();
            Language = Data["Language"].ToString();
            MESReturnMessage.Language = Language;
            LoginReturn lr = new LoginReturn();
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetLoginUser = new MESDataObject.Module.T_c_user(SFCDB, this.DBTYPE);
            MESDataObject.Module.Row_c_user rcu = (Row_c_user)GetLoginUser.NewRow();
            rcu = GetLoginUser.getC_Userbyempno(EMP_NO, SFCDB, this.DBTYPE);
            if (rcu == null)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000010";
                _DBPools["SFCDB"].Return(SFCDB);
                return;
            }
            c_user_info user_info = new c_user_info();
            user_info = GetLoginUser.GetLoginUser(EMP_NO, SFCDB);

            MESPubLab.MESStation.LogicObject.User lu = new MESPubLab.MESStation.LogicObject.User();
            if (PWD == rcu.EMP_PASSWORD)
            {
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
                //lr = new LoginReturn() { Token = TokenBas64, User_ID = user.EMP_NO};
                lr = new LoginReturn() { Token = TokenBas64, User_ID = LoginUser.EMP_NO, UserInfo = user_info };
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000010";
            }
            StationReturn.Data = lr;
            _DBPools["SFCDB"].Return(SFCDB);

        }

        public void CheckPermission(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            string USER = Data["USER"].ToString();
            string PWD = Data["PWD"].ToString();
            string PERMISSION = Data["PERMISSION"].ToString();
            var users = SFCDB.ORM.Queryable<C_USER>().Where(t => t.EMP_NAME == USER).ToList();
            if (users.Count == 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180620163103", new string[] { USER });
                _DBPools["SFCDB"].Return(SFCDB);
                return;
            }

            if (users[0].EMP_PASSWORD != PWD)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180813154717", new string[] { USER });
                _DBPools["SFCDB"].Return(SFCDB);
                return;
            }
            var prs = SFCDB.ORM.Queryable<C_USER, C_USER_PRIVILEGE, C_PRIVILEGE>((U, UP, P) => new object[] {
                SqlSugar.JoinType.Left, U.ID == UP.USER_ID,
                SqlSugar.JoinType.Left, P.ID == UP.PRIVILEGE_ID
            })
                .Where((U, UP, P) => U.EMP_NO == USER && P.PRIVILEGE_NAME == PERMISSION)
                .ToList();
            if (prs.Count == 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200318154141", new string[] { PERMISSION });
                _DBPools["SFCDB"].Return(SFCDB);
                return;
            }
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Data = USER;
            _DBPools["SFCDB"].Return(SFCDB);
        }

    }
}
