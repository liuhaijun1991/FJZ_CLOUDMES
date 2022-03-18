using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_FT_LOGDATA : DataObjectTable
    {
        public T_R_FT_LOGDATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FT_LOGDATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FT_LOGDATA);
            TableName = "R_FT_LOGDATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_FT_LOGDATA : DataObjectBase
    {
        public Row_R_FT_LOGDATA(DataObjectInfo info) : base(info)
        {

        }
        public R_FT_LOGDATA GetDataObject()
        {
            R_FT_LOGDATA DataObject = new R_FT_LOGDATA();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.START_TIME = this.START_TIME;
            DataObject.STOP_TIME = this.STOP_TIME;
            DataObject.RESULT = this.RESULT;
            DataObject.FAIL_ITEM = this.FAIL_ITEM;
            DataObject.FAIL_SYMPTOM = this.FAIL_SYMPTOM;
            DataObject.TPS_NAME = this.TPS_NAME;
            DataObject.ATE_NAME = this.ATE_NAME;
            DataObject.UUT_NAME = this.UUT_NAME;
            DataObject.OPR_ID = this.OPR_ID;
            DataObject.DATASOURCE = this.DATASOURCE;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
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
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public DateTime? STOP_TIME
        {
            get
            {
                return (DateTime?)this["STOP_TIME"];
            }
            set
            {
                this["STOP_TIME"] = value;
            }
        }
        public string RESULT
        {
            get
            {
                return (string)this["RESULT"];
            }
            set
            {
                this["RESULT"] = value;
            }
        }
        public string FAIL_ITEM
        {
            get
            {
                return (string)this["FAIL_ITEM"];
            }
            set
            {
                this["FAIL_ITEM"] = value;
            }
        }
        public string FAIL_SYMPTOM
        {
            get
            {
                return (string)this["FAIL_SYMPTOM"];
            }
            set
            {
                this["FAIL_SYMPTOM"] = value;
            }
        }
        public string TPS_NAME
        {
            get
            {
                return (string)this["TPS_NAME"];
            }
            set
            {
                this["TPS_NAME"] = value;
            }
        }
        public string ATE_NAME
        {
            get
            {
                return (string)this["ATE_NAME"];
            }
            set
            {
                this["ATE_NAME"] = value;
            }
        }
        public string UUT_NAME
        {
            get
            {
                return (string)this["UUT_NAME"];
            }
            set
            {
                this["UUT_NAME"] = value;
            }
        }
        public string OPR_ID
        {
            get
            {
                return (string)this["OPR_ID"];
            }
            set
            {
                this["OPR_ID"] = value;
            }
        }
        public string DATASOURCE
        {
            get
            {
                return (string)this["DATASOURCE"];
            }
            set
            {
                this["DATASOURCE"] = value;
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
    }
    public class R_FT_LOGDATA
    {
        public string ID{get;set;}
        public string SN{get;set;}
        public string WORKORDERNO{get;set;}
        public string SKUNO{get;set;}
        public DateTime? START_TIME{get;set;}
        public DateTime? STOP_TIME{get;set;}
        public string RESULT{get;set;}
        public string FAIL_ITEM{get;set;}
        public string FAIL_SYMPTOM{get;set;}
        public string TPS_NAME{get;set;}
        public string ATE_NAME{get;set;}
        public string UUT_NAME{get;set;}
        public string OPR_ID{get;set;}
        public string DATASOURCE{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}