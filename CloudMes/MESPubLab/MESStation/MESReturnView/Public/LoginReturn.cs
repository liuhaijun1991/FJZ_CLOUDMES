using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;

namespace MESPubLab.MESStation.MESReturnView.Public
{
    public class LoginReturn
    {            
        public string Token;
        public string User_ID;
        public c_user_info UserInfo;
        public string nextInput;
    }

    //public class c_user_info
    //{
    //    public c_user_info()
    //    {

    //    }

    //    public string ID { get; set; }
    //    public string FACTORY { get; set; }
    //    public string BU_NAME { get; set; }
    //    public string EMP_NO { get; set; }
    //    public string EMP_NAME { get; set; }
    //    public string EMP_LEVEL { get; set; }
    //    public string DPT_NAME { get; set; }
    //}
    public class Privileges
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string MENU_NAME { get; set; }
        public string PAGE_PATH { get; set; }
        public string PARENT_CODE { get; set; }
        public string SORT { get; set; }
        public string STYLE_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string LANGUAGE_ID { get; set; }
        public string MENU_DESC { get; set; }
        public List<Privileges> MENU_ITEM { get; set; }
    }

    public class Privilegesid
    {
        public string PRIVILEGE_ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }
    }

    public class c_role
    {
        public c_role()
        {

        }
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string EDIT_EMP { get; set; }


    }

    public class C_UserRole_Info
    {
        public C_UserRole_Info()
        {

        }

        public string ID { get; set; }
        public string FACTORY { get; set; }
        public string BU_NAME { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_PASSWORD { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_LEVEL { get; set; }
        public string DPT_NAME { get; set; }
        public string POSITION_NAME { get; set; }
        public string MAIL_ADDRESS { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string LOCATION { get; set; }
        public string LOCK_FLAG { get; set; }
        public string AGENT_EMP_NO { get; set; }
        public string EMP_DESC { get; set; }
        public string EDIT_EMP { get; set; }
        public string EMP_EN_NAME { get; set; }
    }
}
