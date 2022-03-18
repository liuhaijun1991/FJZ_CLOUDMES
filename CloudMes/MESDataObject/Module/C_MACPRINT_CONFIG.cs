using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    //add by hgb 2019.06.24
    public class T_C_MACPRINT_CONFIG : DataObjectTable
    {
        public T_C_MACPRINT_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MACPRINT_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MACPRINT_CONFIG);
            TableName = "C_MACPRINT_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
          
        public bool CheckExists(string SKUNO, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM C_MACPRINT_CONFIG WHERE SKUNO = '{SKUNO}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// add by hgb 2019.08.26
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="LABEL_TYPE"></param>
        /// <param name="EVENTPOINT"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckExists(string SKUNO, string LABEL_TYPE, string EVENTPOINT, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            string StrsqlLABEL_TYPE = string.Empty;
            string StrsqlEVENTPOINT = string.Empty;
            if (LABEL_TYPE.Length != 0)
            {
                StrsqlLABEL_TYPE = $@" AND LABEL_TYPE='{LABEL_TYPE}'";
            }
            if (EVENTPOINT.Length != 0)
            {
                StrsqlEVENTPOINT = $@" AND EVENTPOINT='{EVENTPOINT}'";
            }
            StrSql = $@"SELECT * FROM C_MACPRINT_CONFIG WHERE SKUNO = '{SKUNO}' {StrsqlLABEL_TYPE} {StrsqlEVENTPOINT}   ";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool CheckExistsByskuAndLabeltype(string SKUNO, string TYPE, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM C_MACPRINT_CONFIG WHERE SKUNO = '{SKUNO}' and LABEL_TYPE = '{TYPE}' ";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public C_MACPRINT_CONFIG LoadDataBysku(string SKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_MACPRINT_CONFIG>().Where(t => t.SKUNO == SKUNO).ToList().FirstOrDefault();
        }


        public C_MACPRINT_CONFIG LoadDataByskuAndLabeltype(string SKUNO, string TYPE, OleExec DB)
        {
            return DB.ORM.Queryable<C_MACPRINT_CONFIG>().Where(t => t.SKUNO == SKUNO && t.LABEL_TYPE== TYPE).ToList().FirstOrDefault();
        }
    }
    public class Row_C_MACPRINT_CONFIG : DataObjectBase
    {
        public Row_C_MACPRINT_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public C_MACPRINT_CONFIG GetDataObject()
        {
            C_MACPRINT_CONFIG DataObject = new C_MACPRINT_CONFIG();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LABEL_SAMPLE = this.LABEL_SAMPLE;
            DataObject.EVENTPOINT = this.EVENTPOINT;
            DataObject.LABEL_TYPE = this.LABEL_TYPE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
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
        public string LABEL_SAMPLE
        {
            get
            {
                return (string)this["LABEL_SAMPLE"];
            }
            set
            {
                this["LABEL_SAMPLE"] = value;
            }
        }
        public string EVENTPOINT
        {
            get
            {
                return (string)this["EVENTPOINT"];
            }
            set
            {
                this["EVENTPOINT"] = value;
            }
        }
        public string LABEL_TYPE
        {
            get
            {
                return (string)this["LABEL_TYPE"];
            }
            set
            {
                this["LABEL_TYPE"] = value;
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
    }
    public class C_MACPRINT_CONFIG
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public string LABEL_SAMPLE{get;set;}
        public string EVENTPOINT{get;set;}
        public string LABEL_TYPE{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string DATA1{get;set;}
        public string DATA2{get;set;}
        public string DATA3{get;set;}
    }
}