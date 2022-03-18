using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_WORK_CLASS : DataObjectTable
    {
        public T_C_WORK_CLASS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_WORK_CLASS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_WORK_CLASS);
            TableName = "C_WORK_CLASS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// Get Work Class
        /// </summary>        
        /// <param name="DB">DB</param>
        /// <param name="dateTime">dateTime</param>
        /// <returns></returns>
        public string GetWorkClass(OleExec DB,string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
            {
                throw new Exception("Please in put a DataTime");
            }
            DateTime sysdate;
            DateTime start_time;
            DateTime end_time;
            DateTime tempTime1 = Convert.ToDateTime("00:00:00");
            DateTime tempTime24 = Convert.ToDateTime("23:59:59");
            string workClass = "";
            string sql = $@"select * from c_work_class ";
            DataSet dsWorkClass = DB.ExecSelect(sql);
            try
            {
                sysdate = Convert.ToDateTime(Convert.ToDateTime(dateTime).ToLongTimeString());
                foreach (DataRow row in dsWorkClass.Tables[0].Rows)
                {
                    start_time = Convert.ToDateTime(row["START_TIME"].ToString());
                    end_time = Convert.ToDateTime(row["End_TIME"].ToString());
                    if (DateTime.Compare(start_time, end_time) > 0)
                    {                        
                        if ((DateTime.Compare(sysdate, tempTime1) >= 0) && (DateTime.Compare(sysdate, end_time) <= 0))
                        {
                            workClass = row["NAME"].ToString();
                            break;
                        }
                        else if ((DateTime.Compare(sysdate, tempTime24) >= 0) && (DateTime.Compare(sysdate, start_time) <= 0))
                        {
                            workClass = row["NAME"].ToString();
                            break;
                        }
                    }
                    else
                    {
                        if ((DateTime.Compare(sysdate, start_time) >= 0) && (DateTime.Compare(sysdate, end_time) <= 0))
                        {
                            workClass = row["NAME"].ToString();
                            break;
                        }
                    }                    
                }
                if (string.IsNullOrEmpty(workClass))
                {
                    throw new Exception("Get Work Class Fail");
                }
                return workClass;               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Work Class
        /// </summary>
        /// <param name="oleDB">OleExec</param>
        /// <param name="DBtype">DB_TYPE_ENUM</param>
        /// <returns></returns>
        public string GetWorkClass(OleExec oleDB, DB_TYPE_ENUM DBtype)
        {
            string strSql = "select sysdate from dual";
            DateTime sysdate;
            DateTime start_time;
            DateTime end_time;
            DateTime tempTime1 = Convert.ToDateTime("00:00:00");
            DateTime tempTime24 = Convert.ToDateTime("23:59:59");
            string workClass = "";
            string sql = $@"select * from c_work_class ";
            if (DBtype == DB_TYPE_ENUM.Oracle)
            {
                strSql = "select sysdate from dual";
            }
            else if (DBtype == DB_TYPE_ENUM.SqlServer)
            {
                strSql = "select get_date() ";
            }
            else
            {
                throw new Exception(DBtype.ToString() + " not Work");
            } 
            try
            {
                sysdate = Convert.ToDateTime(oleDB.ExecSelectOneValue(strSql));
                DataSet dsWorkClass = oleDB.ExecSelect(sql);
                foreach (DataRow row in dsWorkClass.Tables[0].Rows)
                {
                    start_time = Convert.ToDateTime(row["START_TIME"].ToString());
                    end_time = Convert.ToDateTime(row["End_TIME"].ToString());
                    if (DateTime.Compare(start_time, end_time) > 0)
                    {
                        if ((DateTime.Compare(sysdate, tempTime1) >= 0) && (DateTime.Compare(sysdate, end_time) <= 0))
                        {
                            workClass = row["NAME"].ToString();
                            break;
                        }
                        else if ((DateTime.Compare(sysdate, tempTime24) >= 0) && (DateTime.Compare(sysdate, start_time) <= 0))
                        {
                            workClass = row["NAME"].ToString();
                            break;
                        }
                    }
                    else
                    {
                        if ((DateTime.Compare(sysdate, start_time) >= 0) && (DateTime.Compare(sysdate, end_time) <= 0))
                        {
                            workClass = row["NAME"].ToString();
                            break;
                        }
                    }
                }
                if(string.IsNullOrEmpty(workClass))
                {
                    throw new Exception("Get Work Class Fail");
                }
                return workClass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        /// <summary>
        /// Get work class list
        /// </summary>
        /// <param name="oleDB"></param>
        /// <param name="workClassName"></param>
        /// <returns></returns>
        public List<C_WORK_CLASS> GetWorkClassList(OleExec oleDB,string workClassName)
        {
            string sql = "";   
            List<C_WORK_CLASS> workClassList = new List<C_WORK_CLASS>();            
            if (string.IsNullOrEmpty(workClassName))
            {
                sql = $@"select * from c_work_class ";
            }
            else
            {
                sql = $@"select * from c_work_class  where name like '%{workClassName}%'";
            }
            DataSet dsWorkClass = oleDB.ExecSelect(sql);
            Row_C_WORK_CLASS workClassRow;
            foreach (DataRow row in dsWorkClass.Tables[0].Rows)
            {
                workClassRow = (Row_C_WORK_CLASS)this.NewRow();
                workClassRow.loadData(row);
                workClassList.Add(workClassRow.GetDataObject());                
            }
            return workClassList;
        }

        /// <summary>
        /// Get Work Class Infos
        /// </summary>
        /// <param name="parameters">Dictionary<column,value></param>
        /// <param name="DB">DB</param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetWorkClassInfos(string className, OleExec DB)
        {
            string sql = "";
            DataSet dsExpand;
            List<Dictionary<string, string>> detailList = new List<Dictionary<string, string>>();
            Dictionary<string, string> detial;
            if (string.IsNullOrEmpty(className))
            {
                sql = $@"select * from c_work_class ";
            }
            else
            {
                sql = $@"select * from c_work_class  where name='{className}'";
            }
            DataSet dsWorkClass = DB.ExecSelect(sql);
            foreach (DataRow row in dsWorkClass.Tables[0].Rows)
            {
                detial = new Dictionary<string, string>();
                //detial.Add("ID", row["ID"].ToString());
                detial.Add("NAME", row["NAME"].ToString());
                detial.Add("START_TIME", row["START_TIME"].ToString());
                detial.Add("END_TIME", row["END_TIME"].ToString());
                sql = $@"select* from c_work_class_ex where id = '{row["ID"].ToString()}' order by seq_no";
                dsExpand = DB.ExecSelect(sql);
                foreach (DataRow rowExpand in dsExpand.Tables[0].Rows)
                {
                    detial.Add(rowExpand["NAME"].ToString(), rowExpand["VALUE"].ToString());
                }
                detailList.Add(detial);
            }
            return detailList;
        }

        /// <summary>
        /// Check work class is exist
        /// </summary>
        /// <param name="classID"></param>
        /// <param name="oleDB"></param>
        /// <returns></returns>
        public bool WorkClassIsExistByID(string classID, OleExec oleDB)
        {
            string sql = $@"select * from c_work_class  where id='{classID}'";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check work class is exist
        /// </summary>
        /// <param name="className"></param>
        /// <param name="oleDB"></param>
        /// <returns></returns>
        public bool WorkClassIsExistByName(string className, OleExec oleDB)
        {
            string sql = $@"select * from c_work_class  where name='{className}'";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get a work class id
        /// </summary>
        /// <param name="className"></param>
        /// <param name="oleDB"></param>
        /// <returns></returns>
        public string GetWorkClassName(string classID, OleExec oleDB)
        {
            try
            {
                string sql = $@"select * from c_work_class  where id='{classID}'";
                DataSet dsWorkClass = oleDB.ExecSelect(sql);
                return dsWorkClass.Tables[0].Rows[0]["name"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool TimeIsExist(string time,OleExec oleDB)
        {
            DateTime dataTime = Convert.ToDateTime(time);
            DateTime startTime;
            DateTime endTime;
            DateTime tempTime1 = Convert.ToDateTime("00:00:00");
            DateTime tempTime24 = Convert.ToDateTime("23:59:59");
            bool isExist = false;
            string sql = $@"select * from c_work_class ";
            DataSet dsWorkClass = oleDB.ExecSelect(sql);
            try
            {
                foreach (DataRow row in dsWorkClass.Tables[0].Rows)
                {
                    startTime =Convert.ToDateTime(row["START_TIME"].ToString());
                    endTime = Convert.ToDateTime(row["END_TIME"].ToString());
                    if (DateTime.Compare(dataTime, startTime) == 0 || DateTime.Compare(dataTime, endTime) == 0)
                    {
                        isExist = true;
                        break;
                    }
                    else
                    {
                        if (DateTime.Compare(startTime, endTime) > 0)
                        {
                            if ((DateTime.Compare(dataTime, tempTime1) >= 0) && (DateTime.Compare(dataTime, endTime) <= 0))
                            {
                                isExist = true;
                                break;
                            }
                            else if ((DateTime.Compare(dataTime, tempTime24) >= 0) && (DateTime.Compare(dataTime, startTime) <= 0))
                            {
                                isExist = true;
                                break;
                            }
                        }
                        else
                        {
                            if (DateTime.Compare(dataTime, startTime) >= 0 && DateTime.Compare(dataTime, endTime) <= 0)
                            {
                                isExist = true;
                                break;
                            }
                        }
                    }
                }
                return isExist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class Row_C_WORK_CLASS : DataObjectBase
    {
        public Row_C_WORK_CLASS(DataObjectInfo info) : base(info)
        {

        }
        public C_WORK_CLASS GetDataObject()
        {
            C_WORK_CLASS DataObject = new C_WORK_CLASS();
            DataObject.ID = this.ID;
            DataObject.NAME = this.NAME;
            DataObject.START_TIME = this.START_TIME;
            DataObject.END_TIME = this.END_TIME;
            return DataObject;
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
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string START_TIME
        {
            get
            {
                return (string)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public string END_TIME
        {
            get
            {
                return (string)this["END_TIME"];
            }
            set
            {
                this["END_TIME"] = value;
            }
        }
    }
    public class C_WORK_CLASS
    {
        public string ID{get;set;}
        public string NAME{get;set;}
        public string START_TIME{get;set;}
        public string END_TIME{get;set;}
    }
}