using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PROCCESS_ALERT : DataObjectTable
    {
        public T_C_PROCCESS_ALERT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PROCCESS_ALERT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PROCCESS_ALERT);
            TableName = "C_PROCCESS_ALERT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_PROCCESS_ALERT : DataObjectBase
    {
        public Row_C_PROCCESS_ALERT(DataObjectInfo info) : base(info)
        {

        }
        public C_PROCCESS_ALERT GetDataObject()
        {
            C_PROCCESS_ALERT DataObject = new C_PROCCESS_ALERT();
            DataObject.ID = this.ID;
            DataObject.PROCCESS_NAME = this.PROCCESS_NAME;
            DataObject.LV1_SMS = this.LV1_SMS;
            DataObject.LV1_MAIL = this.LV1_MAIL;
            DataObject.LV2_SMS = this.LV2_SMS;
            DataObject.LV2_MAIL = this.LV2_MAIL;
            DataObject.LV3_SMS = this.LV3_SMS;
            DataObject.LV3_MAIL = this.LV3_MAIL;
            DataObject.MONITOR_NAME = this.MONITOR_NAME;
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
        public string LV1_SMS
        {
            get
            {
                return (string)this["LV1_SMS"];
            }
            set
            {
                this["LV1_SMS"] = value;
            }
        }
        public string LV1_MAIL
        {
            get
            {
                return (string)this["LV1_MAIL"];
            }
            set
            {
                this["LV1_MAIL"] = value;
            }
        }
        public string LV2_SMS
        {
            get
            {
                return (string)this["LV2_SMS"];
            }
            set
            {
                this["LV2_SMS"] = value;
            }
        }
        public string LV2_MAIL
        {
            get
            {
                return (string)this["LV2_MAIL"];
            }
            set
            {
                this["LV2_MAIL"] = value;
            }
        }
        public string LV3_SMS
        {
            get
            {
                return (string)this["LV3_SMS"];
            }
            set
            {
                this["LV3_SMS"] = value;
            }
        }
        public string LV3_MAIL
        {
            get
            {
                return (string)this["LV3_MAIL"];
            }
            set
            {
                this["LV3_MAIL"] = value;
            }
        }
        public string MONITOR_NAME
        {
            get
            {
                return (string)this["MONITOR_NAME"];
            }
            set
            {
                this["MONITOR_NAME"] = value;
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
    public class C_PROCCESS_ALERT
    {
        public string ID { get; set; }
        public string PROCCESS_NAME { get; set; }
        public string LV1_SMS { get; set; }
        public string LV1_MAIL { get; set; }
        public string LV2_SMS { get; set; }
        public string LV2_MAIL { get; set; }
        public string LV3_SMS { get; set; }
        public string LV3_MAIL { get; set; }
        public string MONITOR_NAME { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
    }
}