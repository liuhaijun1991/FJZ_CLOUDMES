using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SYNC_LOCK : DataObjectTable
    {
        public T_R_SYNC_LOCK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SYNC_LOCK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SYNC_LOCK);
            TableName = "R_SYNC_LOCK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 檢查運行項目是否被鎖
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="IP"></param>
        /// <param name="Program_Name"></param>
        /// <param name="ItemName"></param>
        /// <param name="Emp_No"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public bool Check_SYNC_Lock(string BU, string IP, string Program_Name, string ItemName, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string Message = "";
            //string StrSql = $@"SELECT * From R_SYNC_LOCK where Lock_Name='{ItemName}' ";
            //DataTable dt = DB.ExecSelect(StrSql).Tables[0];
            R_SYNC_LOCK SYNC_Lock= new R_SYNC_LOCK();
            SYNC_Lock=GetSYNCLockID(ItemName, DB, DBType);

            if (SYNC_Lock==null)
            {
                return true;
            }
            else
            {
                Message = ItemName + "正在被其他人使用！";
                //WriteLog(BU, Program_Name, ItemName, Message, StrSql, Emp_No, DB, DBType);
                return false;
            }
        }
        /// <summary>
        /// add by fgg 2018.03.14 Check sync lock,if locked return lock ip
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IsLock(string lockName, OleExec DB, DB_TYPE_ENUM DBType,out string ip)
        {
            R_SYNC_LOCK lockObject = new R_SYNC_LOCK();
            lockObject = GetSYNCLockObject(lockName, DB, DBType);

            if (lockObject == null)
            {
                ip = "";
                return false;
            }
            else
            {
                ip = lockObject.LOCK_IP;
                return true;
            }
        }

        /// <summary>
        /// 增加鎖
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="IP"></param>
        /// <param name="Program_Name"></param>
        /// <param name="ItemName"></param>
        /// <param name="Emp_No"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public bool SYNC_Lock(string BU, string IP, string Program_Name, string ItemName, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            R_SYNC_LOCK Lock = new Module.R_SYNC_LOCK();

            Lock.ID = GetNewID(BU, DB);
            Lock.LOCK_NAME = ItemName;
            Lock.LOCK_KEY = "1";
            Lock.LOCK_TIME_LONG = 5;
            Lock.EDIT_EMP = Emp_No;
            Lock.LOCK_TIME = System.DateTime.Now;
            Lock.LOCK_IP = IP;

            int result = DB.ORM.Insertable<R_SYNC_LOCK>(Lock).ExecuteCommand();
            try
            {
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 程式完，釋放鎖
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="IP"></param>
        /// <param name="Program_Name"></param>
        /// <param name="ItemName"></param>
        /// <param name="Emp_No"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
         public bool SYNC_UnLock(string BU, string IP, string Program_Name, string ItemName, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //string ID = GetNewID(BU, DB);

            //T_R_SYNC_LOCK R_SYNC_LOCK = new T_R_SYNC_LOCK(DB, DBType);
            //Row_R_SYNC_LOCK Rows = (Row_R_SYNC_LOCK)R_SYNC_LOCK.NewRow();

            //R_SYNC_LOCK SYNC_Lock = new R_SYNC_LOCK();
            //SYNC_Lock = GetSYNCLockID(ItemName, DB, DBType);
            return DB.ORM.Deleteable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME.Equals(ItemName) && t.LOCK_IP.Equals(IP)).ExecuteCommand()>0;

            //if (SYNC_Lock != null)
            //{
            //    //Rows.ID = SYNC_Lock.ID;
            //    string result = DB.ExecSQL(Rows.GetDeleteString(DBType, SYNC_Lock.ID));
            //    try
            //    {
            //        int rows = int.Parse(result);
            //        if (rows > 0)
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    catch
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    return true;
            //}            
        }

        public R_SYNC_LOCK GetSYNCLockID(string ItemName, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //string StrSql = $@"SELECT * From R_SYNC_LOCK where Lock_Name='{ItemName}' ";
            //DataTable dt = DB.ExecSelect(StrSql).Tables[0];

            //R_SYNC_LOCK Row_SYNC = new R_SYNC_LOCK();

            //if (dt.Rows.Count > 0)
            //{
            //    Row_SYNC.ID = dt.Rows[0]["ID"].ToString();
            //}

            //return Row_SYNC;
            return DB.ORM.Queryable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME.Equals(ItemName)).ToList().FirstOrDefault();
        }
        /// <summary>
        /// add by fgg 2018.03.14 Get Lock Object
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="db"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public R_SYNC_LOCK GetSYNCLockObject(string lockName, OleExec db, DB_TYPE_ENUM dbType)
        {
            //string StrSql = $@"SELECT * From R_SYNC_LOCK where Lock_Name='{lockName}' ";
            //DataTable dt = db.ExecSelect(StrSql).Tables[0];
            //Row_R_SYNC_LOCK Row_SYNC = (Row_R_SYNC_LOCK)this.NewRow();
            //if (dt.Rows.Count > 0)
            //{
            //    Row_SYNC.loadData(dt.Rows[0]);
            //    return Row_SYNC.GetDataObject();
            //}
            //else
            //{
            //    return null;
            //}
            return db.ORM.Queryable<R_SYNC_LOCK>().Where(t => t.LOCK_NAME.Equals(lockName)).ToList().FirstOrDefault();
           
        }
    }
    public class Row_R_SYNC_LOCK : DataObjectBase
    {
        public Row_R_SYNC_LOCK(DataObjectInfo info) : base(info)
        {

        }
        public R_SYNC_LOCK GetDataObject()
        {
            R_SYNC_LOCK DataObject = new R_SYNC_LOCK();
            DataObject.ID = this.ID;
            DataObject.LOCK_NAME = this.LOCK_NAME;
            DataObject.LOCK_KEY = this.LOCK_KEY;
            DataObject.LOCK_TIME_LONG = this.LOCK_TIME_LONG;
            DataObject.LOCK_TIME = this.LOCK_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.LOCK_IP = this.LOCK_IP;
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
        public string LOCK_NAME
        {
            get
            {
                return (string)this["LOCK_NAME"];
            }
            set
            {
                this["LOCK_NAME"] = value;
            }
        }
        public string LOCK_KEY
        {
            get
            {
                return (string)this["LOCK_KEY"];
            }
            set
            {
                this["LOCK_KEY"] = value;
            }
        }
        public double? LOCK_TIME_LONG
        {
            get
            {
                return (double?)this["LOCK_TIME_LONG"];
            }
            set
            {
                this["LOCK_TIME_LONG"] = value;
            }
        }
        public DateTime LOCK_TIME
        {
            get
            {
                return (DateTime)this["LOCK_TIME"];
            }
            set
            {
                this["LOCK_TIME"] = value;
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
        public string LOCK_IP
        {
            get
            {
                return (string)this["LOCK_IP"];
            }
            set
            {
                this["LOCK_IP"] = value;
            }
        }
    }
    public class R_SYNC_LOCK
    {
        public string ID{get;set;}
        public string LOCK_NAME{get;set;}
        public string LOCK_KEY{get;set;}
        public double? LOCK_TIME_LONG{get;set;}
        public DateTime LOCK_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string LOCK_IP{get;set;}
    }
}