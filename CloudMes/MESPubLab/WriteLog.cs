using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;

namespace MESPubLab
{
    public class WriteLog
    {
        public static void WriteIntoMESLog(OleExec SFCDB, string bu, string programName, string className, string functionName, string logMessage, string logSql, string editEmp)
        {
            //OleExec SFCDB = new OleExec(db, false);
            T_R_MES_LOG mesLog = new T_R_MES_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
            string id = mesLog.GetNewID(bu, SFCDB);
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
            rowMESLog.ID = id;
            rowMESLog.PROGRAM_NAME = programName;
            rowMESLog.CLASS_NAME = className;
            rowMESLog.FUNCTION_NAME = functionName;
            rowMESLog.LOG_MESSAGE = logMessage;
            rowMESLog.LOG_SQL = logSql;
            rowMESLog.EDIT_EMP = editEmp;
            rowMESLog.EDIT_TIME = System.DateTime.Now;
            SFCDB.ThrowSqlExeception = true;
            SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
        }

        public static void WriteIntoMESLog(OleExec SFCDB, string bu, string programName, string className, string functionName, string logMessage, string logSql, string editEmp, string data1, string data2)
        {
            //OleExec SFCDB = new OleExec(db, false);
            T_R_MES_LOG mesLog = new T_R_MES_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
            string id = mesLog.GetNewID(bu, SFCDB);
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
            rowMESLog.ID = id;
            rowMESLog.PROGRAM_NAME = programName;
            rowMESLog.CLASS_NAME = className;
            rowMESLog.FUNCTION_NAME = functionName;
            rowMESLog.LOG_MESSAGE = logMessage;
            rowMESLog.LOG_SQL = logSql;
            rowMESLog.EDIT_EMP = editEmp;
            rowMESLog.EDIT_TIME = System.DateTime.Now;
            rowMESLog.DATA1 = data1;
            rowMESLog.DATA2 = data2;
            SFCDB.ThrowSqlExeception = true;
            SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
        }

        /// <summary>
        /// 使用參數SFCDB的連接字符串,不使用SFCDB本身,本次操作單獨提交.
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="bu"></param>
        /// <param name="programName"></param>
        /// <param name="className"></param>
        /// <param name="functionName"></param>
        /// <param name="logMessage"></param>
        /// <param name="logSql"></param>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="data3"></param>
        /// <param name="editEmp"></param>
        /// <param name="mailflag"></param>

        public static void WriteIntoMESLog(string Dbstr, string bu, string programName, string className, string functionName, string logMessage, string logSql, string data1, string data2, string data3, string editEmp, string mailflag)
        {
            using (var db = OleExec.GetSqlSugarClient(Dbstr, true))
            {
                db.Insertable(new R_MES_LOG
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(db, bu),
                    PROGRAM_NAME = programName,
                    CLASS_NAME = className,
                    FUNCTION_NAME = functionName,
                    LOG_MESSAGE = logMessage,
                    LOG_SQL = logSql,
                    DATA1 = data1,
                    DATA2 = data2,
                    DATA3 = data3,
                    EDIT_EMP = editEmp,
                    EDIT_TIME = System.DateTime.Now,
                    MAILFLAG = mailflag
                }).ExecuteCommand();
            }
        }

        public static void WriteIntoMESLog(OleExec SFCDB, string bu, string programName, string className, string functionName, string logMessage, string logSql, string data1, string data2, string data3, string editEmp, string mailflag)
        {
            SFCDB.ORM.Insertable(new R_MES_LOG
            {
                ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, bu),
                PROGRAM_NAME = programName,
                CLASS_NAME = className,
                FUNCTION_NAME = functionName,
                LOG_MESSAGE = logMessage,
                LOG_SQL = logSql,
                DATA1 = data1,
                DATA2 = data2,
                DATA3 = data3,
                EDIT_EMP = editEmp,
                EDIT_TIME = System.DateTime.Now,
                MAILFLAG = mailflag
            }).ExecuteCommand();
        }

        public static void WriteIntoMESLog(OleExec SFCDB, string bu, string programName, string className, string functionName, string logMessage, string logSql, string data1, string data2, string data3, string data4, string editEmp, string mailflag)
        {
            SFCDB.ORM.Insertable(new R_MES_LOG
            {
                ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, bu),
                PROGRAM_NAME = programName,
                CLASS_NAME = className,
                FUNCTION_NAME = functionName,
                LOG_MESSAGE = logMessage,
                LOG_SQL = logSql,
                DATA1 = data1,
                DATA2 = data2,
                DATA3 = data3,
                DATA4 = data4,
                EDIT_EMP = editEmp,
                EDIT_TIME = System.DateTime.Now,
                MAILFLAG = mailflag
            }).ExecuteCommand();
        }

        public static void WriteIntoMESLog(SqlSugar.SqlSugarClient _db, string bu, string programName, string className, string functionName, string logMessage, string logSql, string data1, string data2, string data3, string data4, string editEmp, string mailflag)
        {
            _db.Insertable(new R_MES_LOG
            {
                ID = MesDbBase.GetNewID<R_MES_LOG>(_db, bu),
                PROGRAM_NAME = programName,
                CLASS_NAME = className,
                FUNCTION_NAME = functionName,
                LOG_MESSAGE = logMessage,
                LOG_SQL = logSql,
                DATA1 = data1,
                DATA2 = data2,
                DATA3 = data3,
                DATA4 = data4,
                EDIT_EMP = editEmp,
                EDIT_TIME = System.DateTime.Now,
                MAILFLAG = mailflag
            }).ExecuteCommand();
        }


        /// <summary>
        /// 取當前執行的FunctionFullName--add by Eden
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentMethodFullName()
        {
            try
            {
                StackTrace st = new StackTrace();
                int maxFrames = st.GetFrames().Length;
                StackFrame sf;
                string methodName, className;
                Type classType;
                sf = st.GetFrame(2);
                classType = sf.GetMethod().DeclaringType;
                className = classType.ToString();
                methodName = sf.GetMethod().Name;
                return className + "." + methodName;
            }
            catch (Exception)
            {
                return "获取方法名失败";
            }
        }
    }
}
