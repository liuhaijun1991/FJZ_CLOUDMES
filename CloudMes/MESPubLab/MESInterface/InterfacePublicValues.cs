using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MESPubLab.MESInterface
{

    public class InterfacePublicValues
    {
        public static bool TimerStarted = false;
        /// <summary>
        /// 是否月結
        /// </summary>
        /// <param name="DB">OleExec</param>
        /// <param name="dbType">DB_TYPE_ENUM</param>
        /// <returns></returns>
        public static bool IsMonthly(OleExec DB, DB_TYPE_ENUM dbType)
        {
            bool isMonthly = false;
            string[] times;
            string sql = string.Empty;
            T_C_CONTROL controlObject = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
            C_CONTROL control = controlObject.GetControlByName("BACKFLUSH", DB);
            if (control != null && dbType == DB_TYPE_ENUM.Oracle)
            {
                times = control.CONTROL_VALUE.Split(new char[] { '~' });

                sql = $@"select 1 from dual where sysdate between to_date('{times[0]}' ,'yyyy-mm-dd hh24:mi:ss') and to_date('{times[1]}' ,'yyyy-mm-dd hh24:mi:ss')";
                DataSet temp = DB.RunSelect(sql);
                if (temp.Tables[0].Rows.Count > 0)
                {
                    isMonthly = true;
                }
            }
            return isMonthly;
        }

        public static string GetPostDate(OleExec sfcdb)
        {
            #region 增加判断逻辑，否则提前设置关帐和抛帐日期会导致关账前取到开账后的抛帐日期，抛帐失败
            var controlValue = sfcdb.ORM.Queryable<C_CONTROL>()
                .Where(t => t.CONTROL_NAME == "BACKFLUSH")
                .Select(t => t.CONTROL_VALUE).First();
            var times = controlValue.Split('~');
            var beginTime = DateTime.Parse(times[0]);
            var endTime = DateTime.Parse(times[1]);
            var dbtime = GetDBDateTime(sfcdb, DB_TYPE_ENUM.Oracle);
            #endregion

            string strPostDATE = "";
            string sqlString = $@"select to_char((to_date(a.control_value, 'mm/dd/yyyy') - sysdate)) as C,
                                  a.control_value as DAT,
                                  to_char(sysdate, 'mm/dd/yyyy') as NOW
                                  from c_control a where upper(CONTROL_NAME)='BACKFLUSHPOSTEDATE'";

            DataSet postDate = sfcdb.ExecuteDataSet(sqlString, CommandType.Text);
            //增加判断逻辑 && dbtime > beginTime 只有大于关账时间才能取到开帐后的抛帐日期
            if (float.Parse(postDate.Tables[0].Rows[0]["C"].ToString()) > 0 && dbtime > beginTime)
            {
                strPostDATE = postDate.Tables[0].Rows[0]["DAT"].ToString();
            }
            else
            {
                strPostDATE = postDate.Tables[0].Rows[0]["NOW"].ToString();
            }
            return strPostDATE;

        }

        public static string GetPostResumeDate(OleExec sfcdb)
        {
            string strPostDATE = "";
            string sqlString = $@"select 'PostDate' as Type, to_char((to_date(a.control_value, 'mm/dd/yyyy') - sysdate)) as C,
                                a.control_value as DAT,
                                to_char(sysdate, 'mm/dd/yyyy') as NOW
                                from c_control a where upper(CONTROL_NAME)='BACKFLUSHPOSTEDATE'
                                UNION
                                select 'ResumeDate' as Type, to_char((to_date(a.control_value, 'mm/dd/yyyy') - sysdate)) as C,
                                a.control_value as DAT,
                                to_char(sysdate, 'mm/dd/yyyy') as NOW
                                from c_control a where upper(CONTROL_NAME)='BACKFLUSHRESUMEDATE'
                                order by Type ";

            DataSet postDate = sfcdb.ExecuteDataSet(sqlString, CommandType.Text);

            if (float.Parse(postDate.Tables[0].Rows[1]["C"].ToString()) > 0)
            {
                strPostDATE = postDate.Tables[0].Rows[1]["NOW"].ToString();
            }
            else if (float.Parse(postDate.Tables[0].Rows[0]["C"].ToString()) > 0)
            {
                strPostDATE = postDate.Tables[0].Rows[0]["DAT"].ToString();
            }
            else
            {
                strPostDATE = postDate.Tables[0].Rows[0]["NOW"].ToString();
            }
            return strPostDATE;

        }

        /// <summary>
        /// Get DB system datetime
        /// </summary>
        /// <param name="DB">OleExec</param>
        /// <param name="dbType">DB_TYPE_ENUM</param>
        /// <returns></returns>
        public static DateTime GetDBDateTime(OleExec DB, DB_TYPE_ENUM dbType)
        {
            string strSql = "select sysdate from dual";
            if (dbType == DB_TYPE_ENUM.Oracle)
            {
                strSql = "select sysdate from dual";
            }
            else if (dbType == DB_TYPE_ENUM.SqlServer)
            {
                strSql = "select get_date() ";
            }
            else
            {
                throw new Exception(dbType.ToString() + " not Work");
            }
            DateTime DBTime = (DateTime)DB.ExecSelectOneValue(strSql);
            return DBTime;
        }

        public static DateTime StringToDate(string ts)
        {
            //string ts = ordermaincurrent.LASTCHANGETIME;
            DateTime time;
            if (ts.Contains("上午"))
            {
                ts = ts.Replace("上午", "");
                time = DateTime.Parse(ts);

            }
            else if (ts.Contains("下午"))
            {
                ts = ts.Replace("下午", "");
                time = DateTime.Parse(ts);
                time = time.AddHours(12);
            }
            else if (ts.Contains("PM"))
            {
                ts = ts.Replace("PM", "");
                time = DateTime.Parse(ts);
                time = time.AddHours(12);
            }
            else if (ts.Contains("AM"))
            {
                ts = ts.Replace("AM", "");
                time = DateTime.Parse(ts);

            }
            else
            {
                time = DateTime.Parse(ts);
            }
            return time;

        }

        //打開定時器
        public static void InterfaceTimerStart(string ProgramName)
        {
            System.Timers.Timer Interface_Timer = new System.Timers.Timer();
            if (!TimerStarted)
            {
                TimerStarted = true;
                Interface_Timer.Enabled = true;
                Interface_Timer.Interval = 1000;
                Interface_Timer.Start();
                Interface_Timer.Elapsed += new ElapsedEventHandler(InterfaceControlTimer);
            }
        }

        public static void InterfaceSchedule(string Command, string ItemName)
        {
            switch (Command.ToUpper())
            {
                case "START":

                    break;
                case "STOP":

                    break;
                case "EXECURE":

                    break;
            }
        }

        private static void InterfaceControlTimer(object Source, ElapsedEventArgs Args)
        {
            InterfaceSchedule("EXECURE", "ALL");
        }
    }
}
