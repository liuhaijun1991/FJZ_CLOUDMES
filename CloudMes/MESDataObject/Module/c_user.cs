using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace MESDataObject.Module
{
    public class T_c_user : DataObjectTable
    {
        public T_c_user(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_c_user(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_c_user);
            TableName = "c_user".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_c_user getC_Userbyempno(string empno, OleExec DB, DB_TYPE_ENUM DBType)
        {

            string strSql = $@"select * from c_user where emp_no in ('{empno}') ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_c_user ret = (Row_c_user)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public Row_c_user getC_Userbyempnoandpass(string empno, String bu, string password, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = "";
            if (bu == "FJZ")
            {
                strSql = $@"select * from c_user where emp_no in ('{empno}') and EMP_PASSWORD in ('{password}') and DPT_NAME in('QE','PQE') and position_name='Quality Engineer' ";
            }
            else
            {
                strSql = $@"select * from c_user where emp_no in ('{empno}') and EMP_PASSWORD in ('{password}') and DPT_NAME in('QE','PQE') ";
            }
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_c_user ret = (Row_c_user)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public DataTable SelectC_Userbyempno(string empno, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from c_user where emp_no='{empno}' ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;

        }

        public DataTable SelectC_User(int rowcount, int pageindex, string emp_no, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = (emp_no == "" ? $@"select * from c_user order by  edit_time  " : $@"select *from c_user  where (factory,bu_name,dpt_name ) in (select factory, bu_name, dpt_name from c_user    where emp_no = '{emp_no}')");
            DataTable res = DB.ExecSelect_b(rowcount, pageindex, strSql);
            return res;

        }

        public c_user_info GetLoginUser(string EMP_NO, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            c_user_info user_info = new c_user_info();
            sql = $@" SELECT *FROM C_USER WHERE EMP_NO='{EMP_NO}'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                user_info = new c_user_info
                {
                    ID = item["ID"].ToString(),
                    FACTORY = item["FACTORY"].ToString(),
                    BU_NAME = item["BU_NAME"].ToString(),
                    EMP_NO = item["EMP_NO"].ToString(),
                    EMP_NAME = item["EMP_NAME"].ToString(),
                    EMP_LEVEL = item["EMP_LEVEL"].ToString(),
                    DPT_NAME = item["DPT_NAME"].ToString()


                };
            }
            return user_info;
        }

        public List<String> GetDptName(string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<String> DptNameList = new List<String>();

            sql = $@" SELECT * FROM  C_USER WHERE EMP_NO='{EmpNo}'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DptNameList.Add(dr["DPT_NAME"].ToString());

            }
            return DptNameList;
        }

        public List<c_user_model> SelectUserInfo(int rowcount, int pageindex, string SearchEmp, string EmpLevel, string DptName, string LoginEmp, string Factory, string Bu, OleExec DB)
        {
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            List<c_user_model> UsetInfolsit = new List<c_user_model>();
            if (EmpLevel == "9") //9级用户取到本厂别所有部门的用户信息
            {
                strSql = (SearchEmp == "" ? $@" select * from c_user where factory='{Factory}' order by  edit_time  " : $@" select * from c_user    where factory='{Factory}' and  emp_no like '%{SearchEmp}%' order by  edit_time ");
            }
            else if (EmpLevel == "1")  //1级用户取到本厂别本部门 的用户信息
            {
                strSql = (SearchEmp == "" ? $@" select * from c_user where factory='{Factory}' and bu_name='{Bu}' and  dpt_name='{DptName}' order by  edit_time  " : $@" select * from c_user    where emp_no like '%{SearchEmp}%' and  factory='{Factory}' and bu_name='{Bu}' and  dpt_name='{DptName}' order by  edit_time ");
            }
            else if (EmpLevel == "0") //0级用户取到本人 的用户信息
            {
                strSql = $@" select * from c_user where emp_no like '%{LoginEmp}%' order by  edit_time  ";
            }
            else
            {
                return UsetInfolsit;
            }
            dt = DB.ExecSelect_b(rowcount, pageindex, strSql);
            foreach (DataRow item in dt.Rows)
            {
                UsetInfolsit.Add(new c_user_model
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
            return UsetInfolsit;

        }

        public List<c_user_model> SelectUserInfomation(string emp_no, OleExec DB)
        {
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            List<c_user_model> UsetInfolsit = new List<c_user_model>();
            strSql = $@" SELECT *FROM C_USER WHERE EMP_NO= '{emp_no}'";
            dt = DB.ExecSelect(strSql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                strSql = $@" SELECT *FROM C_USER WHERE EMP_NO LIKE '%{emp_no}%'";
                dt = DB.ExecSelect(strSql).Tables[0];
            }

            foreach (DataRow item in dt.Rows)
            {
                UsetInfolsit.Add(new c_user_model
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
            return UsetInfolsit;

        }

        public bool CheckIfLevel9(string emp_no,OleExec DB) {

            return DB.ORM.Queryable<C_USER>().Where(it => it.EMP_NO==emp_no && it.EMP_LEVEL == "9").Any();
        }

        public int CheckMaxPage(int PageRow, string Emp_No, OleExec DB)
        {
            int MaxPage = 1;
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            strSql = Emp_No == "" ? $@" select *from c_user order by edit_time" : $@" select * from c_user where emp_no like '%{Emp_No}%' order by edit_time";
            dt = DB.ExecSelect(strSql).Tables[0];

            MaxPage = (PageRow == 1 ? (dt.Rows.Count / PageRow) : (dt.Rows.Count / PageRow + 1));

            return MaxPage;
        }

        /// <summary>
        /// 检查电话号码是否输入是否正确
        /// </summary>
        /// <param name="PhoneNum"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckPhone(string PhoneNum, OleExec DB)
        {

            bool CheckRes = false;
            string regexstr = string.Empty;
            //  regexstr="^[0-9]+$";
            regexstr = @"^5\d{2,3}-\d{5,6}|4\d{1,2}\+\d{6}|\d{11}$";

            CheckRes = Regex.IsMatch(PhoneNum, regexstr);
            //if (PhoneNum.Length < 11)
            //{
            //    for (int i = 0; i < PhoneNum.Length; i++)
            //    {
            //        Byte TmpPhoneNum = Convert.ToByte(PhoneNum[i]);
            //        if (TmpPhoneNum > 48 && TmpPhoneNum < 57)
            //        {
            //            CheckRes = true;
            //        }
            //    }
            //}
            return CheckRes;
        }

        public string CheckPWD(string PWD, OleExec DB)
        {
            string respwd = "Pass";
            string regexstr = string.Empty;
            bool PWDBOOL;
            regexstr = @"^(?![0-9]+$)(?![a-zA-Z]+$)(?!([^(0-9a-zA-Z)]|[\(\)])+$)([^(0-9a-zA-Z)]|[\(\)]|[a-zA-Z]|[0-9]){6,}$";//由数字、26个英文字母或者特殊字符组成的字符串 

            PWDBOOL = Regex.IsMatch(PWD, regexstr);
            if (!PWDBOOL)
            {
                respwd = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529165155", new[] { "Password" });
            }

            return respwd;
        }

        public string CheckMail(string MailAddres, OleExec DB)
        {
            string respwd = "Pass";
            string regexstr = string.Empty;
            bool PWDBOOL = false;
            //  regexstr = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+$";　//驗證email地址 
            regexstr = @"^[A-Za-z0-9d]+([-_.][A-Za-z0-9d]+)*@foxconn.com|[A-Za-z0-9d]+([-_.][A-Za-z0-9d]+)*@mail.foxconn.com$";
            PWDBOOL = Regex.IsMatch(MailAddres, regexstr);
            if (!PWDBOOL)
            {
                respwd = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529165155", new[] { "Email" });
            }

            return respwd;
        }


    }
    

    //    public bool CheckEmpLevel(string UserID,string EmpLevel_flag, OleExec DB)
    //    {
    //        bool respwd = false;
    //        string strsql = string.Empty;
    //        DataTable dt = new DataTable();
    //        strsql = $@" select *from c_user where ID='{UserID}'  ";
    //        dt = DB.ExecSelect(strsql).Tables[0];
    //        if (dt.Rows.Count!=0)
    //        {
    //         string   Level = dt.Rows[0]["EMP_LEVEL"].ToString();

    //            if (Level != "9"&& Level!= EmpLevel_flag)
    //            {
    //                respwd = true;
    //            }
    //        }

    //        return respwd;
    //    }

    //}

    public class c_user_info
    {
        public string ID { get; set; }
        public string FACTORY { get; set; }
        public string BU_NAME { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_LEVEL { get; set; }
        public string DPT_NAME { get; set; }
    }

    public class c_user_model
    {
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

    public class Row_c_user : DataObjectBase
    {
        public Row_c_user(DataObjectInfo info) : base(info)
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
        public string FACTORY
        {
            get
            {
                return (string)this["FACTORY"];
            }
            set
            {
                this["FACTORY"] = value;
            }
        }
        public string BU_NAME
        {
            get
            {
                return (string)this["BU_NAME"];
            }
            set
            {
                this["BU_NAME"] = value;
            }
        }
        public string EMP_NO
        {
            get
            {
                return (string)this["EMP_NO"];
            }
            set
            {
                this["EMP_NO"] = value;
            }
        }
        public string EMP_PASSWORD
        {
            get
            {
                return (string)this["EMP_PASSWORD"];
            }
            set
            {
                this["EMP_PASSWORD"] = value;
            }
        }
        public string EMP_NAME
        {
            get
            {
                return (string)this["EMP_NAME"];
            }
            set
            {
                this["EMP_NAME"] = value;
            }
        }
        public string EMP_LEVEL
        {
            get
            {
                return (string)this["EMP_LEVEL"];
            }
            set
            {
                this["EMP_LEVEL"] = value;
            }
        }
        public string DPT_NAME
        {
            get
            {
                return (string)this["DPT_NAME"];
            }
            set
            {
                this["DPT_NAME"] = value;
            }
        }
        public string POSITION_NAME
        {
            get
            {
                return (string)this["POSITION_NAME"];
            }
            set
            {
                this["POSITION_NAME"] = value;
            }
        }
        public string MAIL_ADDRESS
        {
            get
            {
                return (string)this["MAIL_ADDRESS"];
            }
            set
            {
                this["MAIL_ADDRESS"] = value;
            }
        }
        public string PHONE_NUMBER
        {
            get
            {
                return (string)this["PHONE_NUMBER"];
            }
            set
            {
                this["PHONE_NUMBER"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string LOCK_FLAG
        {
            get
            {
                return (string)this["LOCK_FLAG"];
            }
            set
            {
                this["LOCK_FLAG"] = value;
            }
        }
        public string AGENT_EMP_NO
        {
            get
            {
                return (string)this["AGENT_EMP_NO"];
            }
            set
            {
                this["AGENT_EMP_NO"] = value;
            }
        }
        public DateTime CHANGE_PASSWORD_TIME
        {
            get
            {
                return (DateTime)this["CHANGE_PASSWORD_TIME"];
            }
            set
            {
                this["CHANGE_PASSWORD_TIME"] = value;
            }
        }
        public string EMP_DESC
        {
            get
            {
                return (string)this["EMP_DESC"];
            }
            set
            {
                this["EMP_DESC"] = value;
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
        public string EMP_EN_NAME
        {
            get
            {
                return (string)this["EMP_EN_NAME"];
            }
            set
            {
                this["EMP_EN_NAME"] = value;
            }
        }
    }

    public class C_USER
    {
        public string EMP_EN_NAME{ get; set; }
        public string EDIT_EMP{ get; set; }
        public DateTime? EDIT_TIME{ get; set; }
        public string EMP_DESC{ get; set; }
        public DateTime? CHANGE_PASSWORD_TIME{ get; set; }
        public string AGENT_EMP_NO{ get; set; }
        public string LOCK_FLAG{ get; set; }
        public string LOCATION{ get; set; }
        public string PHONE_NUMBER{ get; set; }
        public string MAIL_ADDRESS{ get; set; }
        public string POSITION_NAME{ get; set; }
        public string DPT_NAME{ get; set; }
        public string EMP_LEVEL{ get; set; }
        public string EMP_NAME{ get; set; }
        public string EMP_PASSWORD{ get; set; }
        public string EMP_NO{ get; set; }
        public string BU_NAME{ get; set; }
        public string FACTORY{ get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{ get; set; }
    }
}