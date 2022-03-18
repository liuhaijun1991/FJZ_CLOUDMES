using System;
using System.Data;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PLAN_LOCK : DataObjectTable
    {
        public T_R_PLAN_LOCK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PLAN_LOCK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PLAN_LOCK);
            TableName = "R_PLAN_LOCK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }


        public bool IsPlanLock(string Value, string SN,string Type,string remark, OleExec DB)
        {
            
            try
            {
                string strSql = $@"select * From r_plan_lock where CONTROL_TYPE='{Type}' and CONTROL_VALUE='{Value}' and LEFT(DESCRIPT, 1) = '{remark}'";
                DataTable dt = DB.ExecSelect(strSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    strSql = $@"INSERT INTO r_sn_lock
                                  (id, sn, TYPE, lock_station, lock_reason, lock_emp)
                                  SELECT 'HWT' || sfc.seq_r_sn_lock.nextval,
                                         '{SN}',
                                         'SN' TYPE,
                                         lock_station,
                                         remark AS lock_reason,
                                         edit_emp
                                    FROM r_plan_lock
                                   WHERE control_type = '{Type}'
                                     AND control_value = '{Value}' 
                                     AND LEFT(descript, 1) = '{remark}'";
                    DB.ThrowSqlExeception = true;
                    DB.ExecSQL(strSql);
                    return true;
                }
                return false;
            }
            catch (Exception e) { throw e; }
            finally
            {
                DB.ThrowSqlExeception = false;
            }
        }
    }
    public class Row_R_PLAN_LOCK : DataObjectBase
    {
        public Row_R_PLAN_LOCK(DataObjectInfo info) : base(info)
        {

        }
        public R_PLAN_LOCK GetDataObject()
        {
            R_PLAN_LOCK DataObject = new R_PLAN_LOCK();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.CONTROL_VALUE = this.CONTROL_VALUE;
            DataObject.LOCK_STATION = this.LOCK_STATION;
            DataObject.DESCRIPT = this.DESCRIPT;
            DataObject.REMARK = this.REMARK;
            DataObject.CREATED_DATE = this.CREATED_DATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }
        public string CONTROL_TYPE
        {
            get
            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string CONTROL_VALUE
        {
            get
            {
                return (string)this["CONTROL_VALUE"];
            }
            set
            {
                this["CONTROL_VALUE"] = value;
            }
        }
        public string LOCK_STATION
        {
            get
            {
                return (string)this["LOCK_STATION"];
            }
            set
            {
                this["LOCK_STATION"] = value;
            }
        }
        public string DESCRIPT
        {
            get
            {
                return (string)this["DESCRIPT"];
            }
            set
            {
                this["DESCRIPT"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public DateTime? CREATED_DATE
        {
            get
            {
                return (DateTime?)this["CREATED_DATE"];
            }
            set
            {
                this["CREATED_DATE"] = value;
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
    }
    public class R_PLAN_LOCK
    {
        public string ID;
        public string BU;
        public string SKUNO;
        public string LOT_NO;
        public string CONTROL_TYPE;
        public string CONTROL_VALUE;
        public string LOCK_STATION;
        public string DESCRIPT;
        public string REMARK;
        public DateTime? CREATED_DATE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}