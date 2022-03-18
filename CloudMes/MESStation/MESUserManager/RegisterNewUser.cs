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

namespace MESStation.MESUserManager
{
    class RegisterNewUserPage : MesAPIBase
    {
        static Random rand = new Random();

        protected APIInfo FRegisterNewUser = new APIInfo()
        {

            FunctionName = "RegisterNewUser",
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

        public RegisterNewUserPage()
        {
            _MastLogin = false;
            this.Apis.Add(FRegisterNewUser.FunctionName, FRegisterNewUser);
        }

        /// <summary>
        /// 創建新用戶
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void RegisterNewUser(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec sfcdb = null;
            T_c_user USER;
            try
            {
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
                UserRow.EDIT_EMP = Data["EMP_NO"].ToString();
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
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000006", new[] { "BU_NAME" });
                    return;
                }

                if (Data["EMP_NO"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000006", new[] { "EMP_NO" });
                    return;
                }
                if (Data["DPT_NAME"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000006", new[] { "DPT_NAME" });
                    return;
                }

                if (Data["EMP_NAME"].ToString() == null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000006", new[] { "EMP_NAME" });
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
                //if (!USER.CheckPhone(Data["PHONE_NUMBER"].ToString(), sfcdb))
                //{
                //    this.DBPools["SFCDB"].Return(sfcdb);
                //    StationReturn.Status = StationReturnStatusValue.Fail;
                //    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529165155", new[] { "phone number" }); ;
                //    return;
                //}

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
    }
}
