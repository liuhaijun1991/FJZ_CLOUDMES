
using MESDataObject;
using MESDBHelper;
using System;
using System.Data;

namespace MESStation
{
    public class Common
    {
        public static string GetWorkClass(OleExec DB)
        {
            string TimeFormat = "HH24:MI:SS";
            DataTable dt = new DataTable();
            string sql = string.Empty;

            sql = $@"SELECT * FROM C_WORK_CLASS WHERE TO_DATE(TO_CHAR(SYSDATE,'{TimeFormat}'),'{TimeFormat}')
                            BETWEEN TO_DATE(START_TIME,'{TimeFormat}') AND TO_DATE(END_TIME,'{TimeFormat}')";

            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["NAME"] != null)
                {
                    return dt.Rows[0]["NAME"].ToString();
                }
                else
                {
                    //throw new Exception("班別的名字不能為空");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154537"));
                }
            }
            else
            {
                //如果上面的沒有結果，表示某一條數據的 END_TIME 是第二天的時間，那麼那一條的 START_TIME 肯定是所有數據中最大的
                sql = "SELECT * FROM C_WORK_CLASS ORDER BY START_TIME DESC";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["NAME"].ToString();
                }
                else
                {
                    //throw new Exception("沒有配置班別");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154821"));
                }
            }


        }
    }
}
