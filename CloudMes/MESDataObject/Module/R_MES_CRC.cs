using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MES_CRC : DataObjectTable
    {
        public T_R_MES_CRC(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MES_CRC(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MES_CRC);
            TableName = "R_MES_CRC".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MES_CRC : DataObjectBase
    {
        public Row_R_MES_CRC(DataObjectInfo info) : base(info)
        {

        }
        public R_MES_CRC GetDataObject()
        {
            R_MES_CRC DataObject = new R_MES_CRC();
            DataObject.ID = this.ID;
            DataObject.F_SITE = this.F_SITE;
            DataObject.F_BU = this.F_BU;
            DataObject.F_FLOOR = this.F_FLOOR;
            DataObject.F_SUBJECT = this.F_SUBJECT;
            DataObject.F_PRIORITY = this.F_PRIORITY;
            DataObject.F_ERRORMESSAGE = this.F_ERRORMESSAGE;
            DataObject.F_DATASOURCE = this.F_DATASOURCE;
            DataObject.F_REPORTER = this.F_REPORTER;
            DataObject.F_CONTACTNUMBER = this.F_CONTACTNUMBER;
            DataObject.F_EMAIL = this.F_EMAIL;
            DataObject.F_SENDER = this.F_SENDER;
            DataObject.F_OWNER = this.F_OWNER;
            DataObject.F_ESCALATION1 = this.F_ESCALATION1;
            DataObject.F_ESCALATION2 = this.F_ESCALATION2;
            DataObject.F_ESCALATION3 = this.F_ESCALATION3;
            DataObject.F_CASEGROUP = this.F_CASEGROUP;
            DataObject.FLAG = this.FLAG;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.SENDTIME = this.SENDTIME;
            DataObject.R_STATUS = this.R_STATUS;
            DataObject.R_CODE = this.R_CODE;
            DataObject.R_MESSAGE = this.R_MESSAGE;
            DataObject.R_DATA = this.R_DATA;
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
        public string F_SITE
        {
            get
            {
                return (string)this["F_SITE"];
            }
            set
            {
                this["F_SITE"] = value;
            }
        }
        public string F_BU
        {
            get
            {
                return (string)this["F_BU"];
            }
            set
            {
                this["F_BU"] = value;
            }
        }
        public string F_FLOOR
        {
            get
            {
                return (string)this["F_FLOOR"];
            }
            set
            {
                this["F_FLOOR"] = value;
            }
        }
        public string F_SUBJECT
        {
            get
            {
                return (string)this["F_SUBJECT"];
            }
            set
            {
                this["F_SUBJECT"] = value;
            }
        }
        public string F_PRIORITY
        {
            get
            {
                return (string)this["F_PRIORITY"];
            }
            set
            {
                this["F_PRIORITY"] = value;
            }
        }
        public string F_ERRORMESSAGE
        {
            get
            {
                return (string)this["F_ERRORMESSAGE"];
            }
            set
            {
                this["F_ERRORMESSAGE"] = value;
            }
        }
        public string F_DATASOURCE
        {
            get
            {
                return (string)this["F_DATASOURCE"];
            }
            set
            {
                this["F_DATASOURCE"] = value;
            }
        }
        public string F_REPORTER
        {
            get
            {
                return (string)this["F_REPORTER"];
            }
            set
            {
                this["F_REPORTER"] = value;
            }
        }
        public string F_CONTACTNUMBER
        {
            get
            {
                return (string)this["F_CONTACTNUMBER"];
            }
            set
            {
                this["F_CONTACTNUMBER"] = value;
            }
        }
        public string F_EMAIL
        {
            get
            {
                return (string)this["F_EMAIL"];
            }
            set
            {
                this["F_EMAIL"] = value;
            }
        }
        public string F_SENDER
        {
            get
            {
                return (string)this["F_SENDER"];
            }
            set
            {
                this["F_SENDER"] = value;
            }
        }
        public string F_OWNER
        {
            get
            {
                return (string)this["F_OWNER"];
            }
            set
            {
                this["F_OWNER"] = value;
            }
        }
        public string F_ESCALATION1
        {
            get
            {
                return (string)this["F_ESCALATION1"];
            }
            set
            {
                this["F_ESCALATION1"] = value;
            }
        }
        public string F_ESCALATION2
        {
            get
            {
                return (string)this["F_ESCALATION2"];
            }
            set
            {
                this["F_ESCALATION2"] = value;
            }
        }
        public string F_ESCALATION3
        {
            get
            {
                return (string)this["F_ESCALATION3"];
            }
            set
            {
                this["F_ESCALATION3"] = value;
            }
        }
        public string F_CASEGROUP
        {
            get
            {
                return (string)this["F_CASEGROUP"];
            }
            set
            {
                this["F_CASEGROUP"] = value;
            }
        }
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public DateTime? SENDTIME
        {
            get
            {
                return (DateTime?)this["SENDTIME"];
            }
            set
            {
                this["SENDTIME"] = value;
            }
        }
        public string R_STATUS
        {
            get
            {
                return (string)this["R_STATUS"];
            }
            set
            {
                this["R_STATUS"] = value;
            }
        }
        public string R_CODE
        {
            get
            {
                return (string)this["R_CODE"];
            }
            set
            {
                this["R_CODE"] = value;
            }
        }
        public string R_MESSAGE
        {
            get
            {
                return (string)this["R_MESSAGE"];
            }
            set
            {
                this["R_MESSAGE"] = value;
            }
        }
        public string R_DATA
        {
            get
            {
                return (string)this["R_DATA"];
            }
            set
            {
                this["R_DATA"] = value;
            }
        }
    }
    public class R_MES_CRC
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string F_SITE { get; set; }
        public string F_BU { get; set; }
        public string F_FLOOR { get; set; }
        public string F_SUBJECT { get; set; }
        public string F_PRIORITY { get; set; }
        public string F_ERRORMESSAGE { get; set; }
        public string F_DATASOURCE { get; set; }
        public string F_REPORTER { get; set; }
        public string F_CONTACTNUMBER { get; set; }
        public string F_EMAIL { get; set; }
        public string F_SENDER { get; set; }
        public string F_OWNER { get; set; }
        public string F_ESCALATION1 { get; set; }
        public string F_ESCALATION2 { get; set; }
        public string F_ESCALATION3 { get; set; }
        public string F_CASEGROUP { get; set; }
        public string FLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? SENDTIME { get; set; }
        public string R_STATUS { get; set; }
        public string R_CODE { get; set; }
        public string R_MESSAGE { get; set; }
        public string R_DATA { get; set; }
    }
}