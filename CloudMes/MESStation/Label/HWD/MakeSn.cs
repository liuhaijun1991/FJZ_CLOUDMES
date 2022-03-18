using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.HWD
{
    public class MakeSn
    {
        private static char[] Tags = new char[] {'0','1','2', '3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H',
            'J','K','L','N','M','P','Q',
            'R','S','T','U','W','X','Y','Z'};
        private static char[] NUM = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        //private static string PX = "DWWDN1";

        private static string intTostring(int num, char[] tags, int SLength)
        {
            string RET = "";
            int C = tags.Length;

            do
            {
                int K = num % C;
                num = num / C;
                RET = tags[K] + RET;

            } while (num > 0);

            if (RET.Length < SLength)
            {
                C = SLength - RET.Length;
                while (C > 0)
                {
                    RET = "0" + RET;
                    C--;
                }
            }

            return RET;
        }


        public static string GetSN(string PX, OleExec sfcdb)
        {
            string strSql = "select to_char( sysdate,'yyyy-mm-dd') YD from dual";
            //string strSql = "select to_char( sysdate+1,'yyyy-mm-dd') YD from dual";
            string strRet = sfcdb.RunSelect(strSql).Tables[0].Rows[0][0].ToString();
            DateTime DATE = DateTime.Parse(strRet);
            string DateCode = DATE.Year.ToString().Substring(2) + intTostring(DATE.Month, Tags, 1) + intTostring(DATE.Day, Tags, 1);
            sfcdb.BeginTrain();
            C_CONTROL controlObjec = sfcdb.ORM.Queryable<C_CONTROL>().Where(c => c.CONTROL_NAME == "HWDGeneralSN").ToList().FirstOrDefault();
            if (controlObjec.CONTROL_TYPE != DateCode)
            {
                sfcdb.ORM.Updateable<C_CONTROL>().UpdateColumns(c => new C_CONTROL { CONTROL_TYPE = DateCode, CONTROL_VALUE = "0" }).Where(c => c.ID == controlObjec.ID).ExecuteCommand();
                controlObjec = sfcdb.ORM.Queryable<C_CONTROL>().Where(c => c.CONTROL_NAME == "HWDGeneralSN").ToList().FirstOrDefault();
            }           
            string SN = PX + DateCode + intTostring(Int32.Parse(controlObjec.CONTROL_VALUE), NUM, 6);
            string controlValue = (Int32.Parse(controlObjec.CONTROL_VALUE) + 1).ToString();
            sfcdb.ORM.Updateable<C_CONTROL>().UpdateColumns(c => new C_CONTROL() { CONTROL_VALUE = controlValue }).Where(c => c.ID == controlObjec.ID).ExecuteCommand();
            //T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            //string id= t_r_mes_log.GetNewID("HWD", sfcdb);
            //strSql = $@"insert into r_mes_log(id,program_name,class_name,function_name,log_message,edit_emp,edit_time)
            //                            values('{id}','PrintLab','MESStation.Label.HWD.MakeSn','GetSN','{SN}','SYSTEM',sysdate)";
            //sfcdb.ExecSQL(strSql);
            sfcdb.CommitTrain();
            return SN;
        }

        public static string GetCurrSN(string PX, OleExec sfcdb)
        {
            string strSql = "select to_char( sysdate,'yyyy-mm-dd') YD from dual";
            string strRet = sfcdb.RunSelect(strSql).Tables[0].Rows[0][0].ToString();
            DateTime DATE = DateTime.Parse(strRet);
            string DateCode = DATE.Year.ToString().Substring(2) + intTostring(DATE.Month, Tags, 1) + intTostring(DATE.Day, Tags, 1);

            C_CONTROL controlObjec = sfcdb.ORM.Queryable<C_CONTROL>().Where(c => c.CONTROL_NAME == "HWDGeneralSN").ToList().FirstOrDefault();
            if (controlObjec.CONTROL_TYPE != DateCode)
            {
                sfcdb.ORM.Updateable<C_CONTROL>().UpdateColumns(c => new C_CONTROL() { CONTROL_TYPE = DateCode, CONTROL_LEVEL = "0" }).Where(c => c.ID == controlObjec.ID).ExecuteCommand();
            }

            string SN = PX + DateCode + intTostring(Int32.Parse(controlObjec.CONTROL_VALUE), NUM, 6);
            return SN;
        }
    }
}
