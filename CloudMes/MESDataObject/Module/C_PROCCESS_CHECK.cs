using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PROCCESS_CHECK : DataObjectTable
    {
        public T_C_PROCCESS_CHECK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PROCCESS_CHECK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PROCCESS_CHECK);
            TableName = "C_PROCCESS_CHECK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_PROCCESS_CHECK : DataObjectBase
    {
        public Row_C_PROCCESS_CHECK(DataObjectInfo info) : base(info)
        {

        }
        public C_PROCCESS_CHECK GetDataObject()
        {
            C_PROCCESS_CHECK DataObject = new C_PROCCESS_CHECK();
            DataObject.ID = this.ID;
            DataObject.PROCCESS_NAME = this.PROCCESS_NAME;
            DataObject.CHECK_TYPE = this.CHECK_TYPE;
            DataObject.CONFIG = this.CONFIG;
            DataObject.ALERT_STATE = this.ALERT_STATE;
            DataObject.RUN_TIME_DATA = this.RUN_TIME_DATA;
            DataObject.EDIT_DATE = this.EDIT_DATE;
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
        public string PROCCESS_NAME
        {
            get
            {
                return (string)this["PROCCESS_NAME"];
            }
            set
            {
                this["PROCCESS_NAME"] = value;
            }
        }
        public string CHECK_TYPE
        {
            get
            {
                return (string)this["CHECK_TYPE"];
            }
            set
            {
                this["CHECK_TYPE"] = value;
            }
        }
        public string CONFIG
        {
            get
            {
                return (string)this["CONFIG"];
            }
            set
            {
                this["CONFIG"] = value;
            }
        }
        public string ALERT_STATE
        {
            get
            {
                return (string)this["ALERT_STATE"];
            }
            set
            {
                this["ALERT_STATE"] = value;
            }
        }
        public string RUN_TIME_DATA
        {
            get
            {
                return (string)this["RUN_TIME_DATA"];
            }
            set
            {
                this["RUN_TIME_DATA"] = value;
            }
        }
        public DateTime? EDIT_DATE
        {
            get
            {
                return (DateTime?)this["EDIT_DATE"];
            }
            set
            {
                this["EDIT_DATE"] = value;
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
    public class C_PROCCESS_CHECK
    {
        public string ID { get; set; }
        public string PROCCESS_NAME { get; set; }
        public string CHECK_TYPE { get; set; }
        public string CONFIG { get; set; }
        public string ALERT_STATE { get; set; }
        public string RUN_TIME_DATA { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
    }
}