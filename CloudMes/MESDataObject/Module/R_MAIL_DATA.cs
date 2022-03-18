using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MAIL_DATA : DataObjectTable
    {
        public T_R_MAIL_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MAIL_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MAIL_DATA);
            TableName = "R_MAIL_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MAIL_DATA : DataObjectBase
    {
        public Row_R_MAIL_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_MAIL_DATA GetDataObject()
        {
            R_MAIL_DATA DataObject = new R_MAIL_DATA();
            DataObject.ID = this.ID;
            DataObject.FUNCTIONNAME = this.FUNCTIONNAME;
            DataObject.TITLE = this.TITLE;
            DataObject.CONTENTS = this.CONTENTS;
            DataObject.MAILTO = this.MAILTO;
            DataObject.ATTRFILEID = this.ATTRFILEID;
            DataObject.SENDTIME = this.SENDTIME;
            DataObject.MFLAG = this.MFLAG;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string FUNCTIONNAME
        {
            get
            {
                return (string)this["FUNCTIONNAME"];
            }
            set
            {
                this["FUNCTIONNAME"] = value;
            }
        }
        public string TITLE
        {
            get
            {
                return (string)this["TITLE"];
            }
            set
            {
                this["TITLE"] = value;
            }
        }
        public string CONTENTS
        {
            get
            {
                return (string)this["CONTENTS"];
            }
            set
            {
                this["CONTENTS"] = value;
            }
        }
        public string MAILTO
        {
            get
            {
                return (string)this["MAILTO"];
            }
            set
            {
                this["MAILTO"] = value;
            }
        }
        public string ATTRFILEID
        {
            get
            {
                return (string)this["ATTRFILEID"];
            }
            set
            {
                this["ATTRFILEID"] = value;
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
        public string MFLAG
        {
            get
            {
                return (string)this["MFLAG"];
            }
            set
            {
                this["MFLAG"] = value;
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
    }
    public class R_MAIL_DATA
    {
        public string ID { get; set; }
        public string FUNCTIONNAME { get; set; }
        public string TITLE { get; set; }
        public string CONTENTS { get; set; }
        public string MAILTO { get; set; }
        public string ATTRFILEID { get; set; }
        public DateTime? SENDTIME { get; set; }
        public string MFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}