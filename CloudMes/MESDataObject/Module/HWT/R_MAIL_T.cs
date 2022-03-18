using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_MAIL_T : DataObjectTable
    {
        public T_R_MAIL_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MAIL_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MAIL_T);
            TableName = "R_MAIL_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MAIL_T : DataObjectBase
    {
        public Row_R_MAIL_T(DataObjectInfo info) : base(info)
        {

        }
        public R_MAIL_T GetDataObject()
        {
            R_MAIL_T DataObject = new R_MAIL_T();
            DataObject.MAIL_ID = this.MAIL_ID;
            DataObject.MAIL_TO = this.MAIL_TO;
            DataObject.MAIL_FROM = this.MAIL_FROM;
            DataObject.MAIL_CC = this.MAIL_CC;
            DataObject.MAIL_BCC = this.MAIL_BCC;
            DataObject.MAIL_ATT = this.MAIL_ATT;
            DataObject.MAIL_SUBJECT = this.MAIL_SUBJECT;
            DataObject.MAIL_SEQUENCE = this.MAIL_SEQUENCE;
            DataObject.MAIL_CONTENT = this.MAIL_CONTENT;
            DataObject.MAIL_DATE = this.MAIL_DATE;
            DataObject.MAIL_FLAG = this.MAIL_FLAG;
            DataObject.MAIL_PROGRAM = this.MAIL_PROGRAM;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string MAIL_ID
        {
            get
            {
                return (string)this["MAIL_ID"];
            }
            set
            {
                this["MAIL_ID"] = value;
            }
        }
        public string MAIL_TO
        {
            get
            {
                return (string)this["MAIL_TO"];
            }
            set
            {
                this["MAIL_TO"] = value;
            }
        }
        public string MAIL_FROM
        {
            get
            {
                return (string)this["MAIL_FROM"];
            }
            set
            {
                this["MAIL_FROM"] = value;
            }
        }
        public string MAIL_CC
        {
            get
            {
                return (string)this["MAIL_CC"];
            }
            set
            {
                this["MAIL_CC"] = value;
            }
        }
        public string MAIL_BCC
        {
            get
            {
                return (string)this["MAIL_BCC"];
            }
            set
            {
                this["MAIL_BCC"] = value;
            }
        }
        public string MAIL_ATT
        {
            get
            {
                return (string)this["MAIL_ATT"];
            }
            set
            {
                this["MAIL_ATT"] = value;
            }
        }
        public string MAIL_SUBJECT
        {
            get
            {
                return (string)this["MAIL_SUBJECT"];
            }
            set
            {
                this["MAIL_SUBJECT"] = value;
            }
        }
        public string MAIL_SEQUENCE
        {
            get
            {
                return (string)this["MAIL_SEQUENCE"];
            }
            set
            {
                this["MAIL_SEQUENCE"] = value;
            }
        }
        public string MAIL_CONTENT
        {
            get
            {
                return (string)this["MAIL_CONTENT"];
            }
            set
            {
                this["MAIL_CONTENT"] = value;
            }
        }
        public DateTime? MAIL_DATE
        {
            get
            {
                return (DateTime?)this["MAIL_DATE"];
            }
            set
            {
                this["MAIL_DATE"] = value;
            }
        }
        public string MAIL_FLAG
        {
            get
            {
                return (string)this["MAIL_FLAG"];
            }
            set
            {
                this["MAIL_FLAG"] = value;
            }
        }
        public string MAIL_PROGRAM
        {
            get
            {
                return (string)this["MAIL_PROGRAM"];
            }
            set
            {
                this["MAIL_PROGRAM"] = value;
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
    public class R_MAIL_T
    {
        public string MAIL_ID{ get; set; }
        public string MAIL_TO{ get; set; }
        public string MAIL_FROM{ get; set; }
        public string MAIL_CC{ get; set; }
        public string MAIL_BCC{ get; set; }
        public string MAIL_ATT{ get; set; }
        public string MAIL_SUBJECT{ get; set; }
        public string MAIL_SEQUENCE{ get; set; }
        public string MAIL_CONTENT{ get; set; }
        public DateTime? MAIL_DATE{ get; set; }
        public string MAIL_FLAG{ get; set; }
        public string MAIL_PROGRAM{ get; set; }
        public string EDIT_EMP{ get; set; }
        public DateTime? EDIT_TIME{ get; set; }
    }
}
