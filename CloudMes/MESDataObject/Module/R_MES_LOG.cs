using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_MES_LOG : DataObjectTable
    {
        public T_R_MES_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MES_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MES_LOG);
            TableName = "R_MES_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable GetMESLog(string programName, string className, string functionName, string startTime, string endTime, OleExec db, DB_TYPE_ENUM dbType)
        {
            string sql = $@"select * from r_mes_log where 1=1";
            if (!string.IsNullOrEmpty(programName))
            {
                sql = sql + $@" and program_name='{programName}'";
            }
            if (!string.IsNullOrEmpty(className))
            {
                sql = sql + $@" and class_name='{className}'";
            }
            if (!string.IsNullOrEmpty(functionName))
            {
                sql = sql + $@" and function_name='{functionName}'";
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                sql = sql + $@" and edit_time between to_date('{startTime}','yyyy/mm/dd hh24:mi:ss') and to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')";
            }
            sql = sql + " order by edit_time desc ";

            return db.ExecSelect(sql).Tables[0];
        }
        public DataTable GetMESLog(string programName, string className, string functionName, string startTime, string endTime, string EDITEMP, OleExec db, DB_TYPE_ENUM dbType)
        {
            string sql = $@"select * from r_mes_log where 1=1";
            if (!string.IsNullOrEmpty(programName))
            {
                sql = sql + $@" and program_name='{programName}'";
            }
            if (!string.IsNullOrEmpty(className))
            {
                sql = sql + $@" and class_name='{className}'";
            }
            if (!string.IsNullOrEmpty(functionName))
            {
                sql = sql + $@" and function_name='{functionName}'";
            }
            if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                sql = sql + $@" and edit_time >=to_date('{startTime}','yyyy/mm/dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                sql = sql + $@" and edit_time between to_date('{startTime}','yyyy/mm/dd hh24:mi:ss') and to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(EDITEMP))
            {
                sql = sql + $@" and EDIT_EMP='{EDITEMP}'";
            }
            sql = sql + " order by edit_time desc ";

            return db.ExecSelect(sql).Tables[0];
        }
        public int InsertkpsnLog(R_MES_LOG MESLOG, OleExec DB)
        {
            return DB.ORM.Insertable<R_MES_LOG>(MESLOG).ExecuteCommand();
        }

        public int InsertMESLog(R_MES_LOG MESLOG, OleExec DB)
        {
            return DB.ORM.Insertable<R_MES_LOG>(MESLOG).ExecuteCommand();
        }
        /// <summary>
        /// 用於那些沒有最新欄位的數據庫
        /// </summary>
        /// <param name="MESLOG"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InsertMESLogOld(R_MES_LOG MESLOG, OleExec DB)
        {
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)NewRow();
            rowMESLog.ID = MESLOG.ID;
            rowMESLog.PROGRAM_NAME = MESLOG.PROGRAM_NAME;
            rowMESLog.CLASS_NAME = MESLOG.CLASS_NAME;
            rowMESLog.FUNCTION_NAME = MESLOG.FUNCTION_NAME;
            rowMESLog.LOG_MESSAGE = MESLOG.LOG_MESSAGE;
            rowMESLog.LOG_SQL = MESLOG.LOG_SQL;
            rowMESLog.DATA1 = MESLOG.DATA1;
            rowMESLog.DATA2 = MESLOG.DATA2;
            rowMESLog.DATA3 = MESLOG.DATA3;
            rowMESLog.EDIT_EMP = MESLOG.EDIT_EMP;
            rowMESLog.EDIT_TIME = MESLOG.EDIT_TIME;
            string r = DB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
            int o;
            if (int.TryParse(r, out o))
            {
                return o;
            }
            else
            {
                return 0;
            }

        }

        public int InsertMESLog(OleExec DB, string BU, string program_name, string class_name, string function_name, string log_message, string data1, string emp_no)
        {
            string id = this.GetNewID(BU, DB);
            string sql = $@"insert into r_mes_log(id,program_name,class_name,function_name,log_message,data1,edit_emp,edit_time)
                                        values('{id}','{program_name}','{class_name}','{function_name}','{log_message}','{data1}','{emp_no}',sysdate)";
            string result = DB.ExecSQL(sql);
            int re;
            bool bResult = int.TryParse(result, out re);
            if (!bResult || (bResult && re == 0))
            {
                throw new Exception($@"insert into log error!{result}");
            }
            return re;
        }
        public int InsertMESLog(OleExec DB, string BU, string program_name, string class_name, string function_name, string log_message, string log_sql, string data1, string emp_no)
        {
            string id = this.GetNewID(BU, DB);
            string sql = $@"insert into r_mes_log(id,program_name,class_name,function_name,log_message,log_sql,data1,edit_emp,edit_time)
                                        values('{id}','{program_name}','{class_name}','{function_name}','{log_message}','{log_sql}','{data1}','{emp_no}',sysdate)";
            string result = DB.ExecSQL(sql);
            int re;
            bool bResult = int.TryParse(result, out re);
            if (!bResult || (bResult && re == 0))
            {
                throw new Exception($@"insert into log error!{result}");
            }
            return re;
        }
    }
    public class Row_R_MES_LOG : DataObjectBase
    {
        public Row_R_MES_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_MES_LOG GetDataObject()
        {
            R_MES_LOG DataObject = new R_MES_LOG();
            DataObject.FUNCTION_NAME = this.FUNCTION_NAME;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.PROGRAM_NAME = this.PROGRAM_NAME;
            DataObject.ID = this.ID;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA1 = this.DATA1;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.LOG_SQL = this.LOG_SQL;
            DataObject.LOG_MESSAGE = this.LOG_MESSAGE;
            DataObject.MAILFLAG = this.MAILFLAG;
            return DataObject;
        }
        public string FUNCTION_NAME
        {
            get
            {
                return (string)this["FUNCTION_NAME"];
            }
            set
            {
                this["FUNCTION_NAME"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string PROGRAM_NAME
        {
            get
            {
                return (string)this["PROGRAM_NAME"];
            }
            set
            {
                this["PROGRAM_NAME"] = value;
            }
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
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
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
        public string LOG_SQL
        {
            get
            {
                return (string)this["LOG_SQL"];
            }
            set
            {
                this["LOG_SQL"] = value;
            }
        }
        public string LOG_MESSAGE
        {
            get
            {
                return (string)this["LOG_MESSAGE"];
            }
            set
            {
                this["LOG_MESSAGE"] = value;
            }
        }
        public string MAILFLAG
        {
            get
            {
                return (string)this["MAILFLAG"];
            }
            set
            {
                this["MAILFLAG"] = value;
            }
        }
    }
    public class R_MES_LOG
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string PROGRAM_NAME { get; set; }
        public string CLASS_NAME { get; set; }
        public string FUNCTION_NAME { get; set; }
        public string LOG_MESSAGE { get; set; }
        public string LOG_SQL { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string MAILFLAG { get; set; }
        public string DATA4 { get; set; }
    }

    public enum ENUM_R_MES_LOG
    {
        /// <summary>
        /// "Y"
        /// </summary>
        [EnumValue("Y")]
        MAILFLAG_TRUE,
        /// <summary>
        /// "N"
        /// </summary>
        [EnumValue("N")]
        MAILFLAG_FALSE
    }
}