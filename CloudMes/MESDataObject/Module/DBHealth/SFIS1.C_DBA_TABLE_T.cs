using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using MESDataObject.Common;
using MESDataObject;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.DBHealth
{
    public class T_C_DBA_TABLE_T : DataObjectTable
    {
        public T_C_DBA_TABLE_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_DBA_TABLE_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_DBA_TABLE_T);
            TableName = "SFIS1.C_DBA_TABLE_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public T_C_DBA_TABLE_T()
        {
        }

        public DataTable GetDataTable(OleExec DB)
        {
            string StrSql = "";
            StrSql = $@"select * from SFIS1.C_DBA_TABLE_T   ";
            DataTable dt = DB.ExecSelect(StrSql).Tables[0];
            return dt;
        }
        public string Insert(string APTYPE, string TABLENAME, string TABLEATTRIBUTE, string TABLEKPI, string EMPNO, OleExec DB)
        {
            if (CheckDupTable(TABLENAME, DB) == true)
            {
                return "FAIL";
            }
            else
            {
                string sql = $@"insert into SFIS1.C_DBA_TABLE_T values('{APTYPE}','{TABLENAME}','{TABLEATTRIBUTE}','{TABLEKPI}',SYSDATE,'{EMPNO}')";
                return DB.ExecSQL(sql);
            }
               
        }
        public bool CheckDupTable(string TABLE_NAME, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM SFIS1.C_DBA_TABLE_T WHERE TABLE_NAME='{TABLE_NAME}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }
        public string Update(string APTYPE, string TABLENAME, string TABLEATTRIBUTE, string TABLEKPI,string EMP, OleExec DB)
        {
            string sql = $@"UPDATE SFIS1.C_DBA_TABLE_T SET TABLE_ATTRIBUTE='{TABLEATTRIBUTE}',TABLE_KPI='{TABLEKPI}',EMP_NO='{EMP}' WHERE AP_TYPE='DBHEALTH' AND TABLE_NAME='{TABLENAME}' ";
            return DB.ExecSQL(sql);
            //return DB.ORM.Insertable<R_SN_EX>(R_SN).ExecuteCommand();
        }
        public string Delete(string TABLENAME, OleExec DB)
        {
            string sql = $@"DELETE SFIS1.C_DBA_TABLE_T WHERE AP_TYPE='DBHEALTH' AND TABLE_NAME='{TABLENAME}' ";
            return DB.ExecSQL(sql);
            //return DB.ORM.Insertable<R_SN_EX>(R_SN).ExecuteCommand();
        }
    }
    
    public class Row_C_DBA_TABLE_T : DataObjectBase
    {
        public Row_C_DBA_TABLE_T(DataObjectInfo info) : base(info)
        {

        }
        public C_DBA_TABLE_T GetDataObject()
        {
            C_DBA_TABLE_T DataObject = new C_DBA_TABLE_T();
            DataObject.AP_TYPE = this.AP_TYPE;
            DataObject.TABLE_NAME = this.TABLE_NAME;
            DataObject.TABLE_ATTRIBUTE = this.TABLE_ATTRIBUTE;
            DataObject.TABLE_KPI = this.TABLE_KPI;
            DataObject.IN_STATION_TIME = this.IN_STATION_TIME;
            DataObject.EMP_NO = this.EMP_NO;
            return DataObject;
        }
        public string AP_TYPE
        {
            get
            {
                return (string)this["AP_TYPE"];
            }
            set
            {
                this["AP_TYPE"] = value;
            }
        }
        public string TABLE_NAME
        {
            get
            {
                return (string)this["TABLE_NAME"];
            }
            set
            {
                this["TABLE_NAME"] = value;
            }
        }
        public string TABLE_ATTRIBUTE
        {
            get
            {
                return (string)this["TABLE_ATTRIBUTE"];
            }
            set
            {
                this["TABLE_ATTRIBUTE"] = value;
            }
        }
        public string TABLE_KPI
        {
            get
            {
                return (string)this["TABLE_KPI"];
            }
            set
            {
                this["TABLE_KPI"] = value;
            }
        }
        public DateTime? IN_STATION_TIME
        {
            get
            {
                return (DateTime?)this["IN_STATION_TIME"];
            }
            set
            {
                this["IN_STATION_TIME"] = value;
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
    }
    public class C_DBA_TABLE_T
    {
        public string AP_TYPE { get; set; }
        public string TABLE_NAME { get; set; }
        public string TABLE_ATTRIBUTE { get; set; }
        public string TABLE_KPI { get; set; }
        public DateTime? IN_STATION_TIME { get; set; }
        public string EMP_NO { get; set; }
    }
}