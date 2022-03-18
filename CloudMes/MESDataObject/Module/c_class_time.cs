using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;

namespace MESDataObject.Module
{
    // <copyright file="C_CLASS_TIME.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2017-11-24 </date>
    /// <summary>
    /// 映射數據庫中的C_CLASS_TIME表
    /// </summary>
    public class T_C_CLASS_TIME : DataObjectTable
    {
        public T_C_CLASS_TIME(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CLASS_TIME(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CLASS_TIME);
            TableName = "C_CLASS_TIME".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
 
        /// <summary>
        /// 獲取時間點當期數據庫時間的對應的班別
        /// </summary>
        /// <param name="workSection">workSection</param>
        /// <param name="DB">DB</param>
        /// <returns></returns>
        public string GetShift(string workSection, OleExec DB)
        {
            string sql = $@"select * from c_class_time  where work_section='{workSection}' 
                         and start_time < to_char (sysdate, 'HH24:MI:SS')  
                         and end_time >= to_char (sysdate, 'HH24:MI:SS')";
            DataSet dsClassTime = DB.ExecSelect(sql);
            
            if (dsClassTime.Tables[0].Rows.Count > 0)
            {
                return dsClassTime.Tables[0].Rows[0]["day_distinct"].ToString();
            }
            else
            {
               throw new Exception("This "+ workSection+" Can't Config");
            }
        }
        /// <summary>
        /// Get Total Rows
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetTotalRows(OleExec DB)
        {
            string sql = $@"select count(*) from c_class_time ";
            DataSet dsClassTime = DB.ExecSelect(sql);
            return Convert.ToInt32(dsClassTime.Tables[0].Rows[0][0].ToString());           
        }

        public DataTable GetShiftInfo(Dictionary<string, string> parameters, OleExec DB)
        {
            string sql = $@"select * from c_class_time  where 1=1 ";            
            string tempSql = "";
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> paras in parameters)
                {
                    if (paras.Value != "")
                    {
                        tempSql = tempSql + $@" and {paras.Key} = '{paras.Value}' ";
                    }
                }
            }
            sql = sql + tempSql;
            return DB.ExecSelect(sql).Tables[0];            
        }
    }
    public class Row_C_CLASS_TIME : DataObjectBase
    {
        public Row_C_CLASS_TIME(DataObjectInfo info) : base(info)
        {

        }
        public C_CLASS_TIME GetDataObject()
        {
            C_CLASS_TIME DataObject = new C_CLASS_TIME();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.WORK_SECTION = this.WORK_SECTION;
            DataObject.START_TIME = this.START_TIME;
            DataObject.END_TIME = this.END_TIME;
            DataObject.WORK_CLASS = this.WORK_CLASS;
            DataObject.DAY_DISTINCT = this.DAY_DISTINCT;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string WORK_SECTION
        {
            get
            {
                return (string)this["WORK_SECTION"];
            }
            set
            {
                this["WORK_SECTION"] = value;
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
        public string WORK_CLASS
        {
            get
            {
                return (string)this["WORK_CLASS"];
            }
            set
            {
                this["WORK_CLASS"] = value;
            }
        }
        public string DAY_DISTINCT
        {
            get
            {
                return (string)this["DAY_DISTINCT"];
            }
            set
            {
                this["DAY_DISTINCT"] = value;
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
        public class C_CLASS_TIME
        {
            public string ID;
            public double? SEQ_NO;
            public string WORK_SECTION;
            public string START_TIME;
            public string END_TIME;
            public string WORK_CLASS;
            public string DAY_DISTINCT;
            public DateTime EDIT_TIME;
            public string EDIT_EMP;
        }
    }

    public enum WORK_CLASS
    {
        Shift1,
        Shift2
    }
}