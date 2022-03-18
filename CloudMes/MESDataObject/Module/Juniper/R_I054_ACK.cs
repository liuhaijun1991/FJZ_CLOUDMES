using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_I054_ACK : DataObjectTable
    {
        public T_R_I054_ACK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I054_ACK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I054_ACK);
            TableName = "R_I054_ACK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I054_ACK : DataObjectBase
    {
        public Row_R_I054_ACK(DataObjectInfo info) : base(info)
        {

        }
        public R_I054_ACK GetDataObject()
        {
            R_I054_ACK DataObject = new R_I054_ACK();
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.SALESORDERNUMBER = this.SALESORDERNUMBER;
            DataObject.SALESORDERLINENUMBER = this.SALESORDERLINENUMBER;
            DataObject.SERIALNUMBER = this.SERIALNUMBER;
            DataObject.MODELNUMBER = this.MODELNUMBER;
            DataObject.RESPONSECODE = this.RESPONSECODE;
            DataObject.RESPONSEMESSAGE = this.RESPONSEMESSAGE;
            DataObject.TRANID = this.TRANID;
            DataObject.F_LASTEDIT = this.F_LASTEDIT;
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
        public string F_ID
        {
            get
            {
                return (string)this["F_ID"];
            }
            set
            {
                this["F_ID"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public string MESSAGEID
        {
            get
            {
                return (string)this["MESSAGEID"];
            }
            set
            {
                this["MESSAGEID"] = value;
            }
        }
        public string SALESORDERNUMBER
        {
            get
            {
                return (string)this["SALESORDERNUMBER"];
            }
            set
            {
                this["SALESORDERNUMBER"] = value;
            }
        }
        public string SALESORDERLINENUMBER
        {
            get
            {
                return (string)this["SALESORDERLINENUMBER"];
            }
            set
            {
                this["SALESORDERLINENUMBER"] = value;
            }
        }
        public string SERIALNUMBER
        {
            get
            {
                return (string)this["SERIALNUMBER"];
            }
            set
            {
                this["SERIALNUMBER"] = value;
            }
        }
        public string MODELNUMBER
        {
            get
            {
                return (string)this["MODELNUMBER"];
            }
            set
            {
                this["MODELNUMBER"] = value;
            }
        }
        public string RESPONSECODE
        {
            get
            {
                return (string)this["RESPONSECODE"];
            }
            set
            {
                this["RESPONSECODE"] = value;
            }
        }
        public string RESPONSEMESSAGE
        {
            get
            {
                return (string)this["RESPONSEMESSAGE"];
            }
            set
            {
                this["RESPONSEMESSAGE"] = value;
            }
        }
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
            }
        }
        public DateTime? F_LASTEDIT
        {
            get
            {
                return (DateTime?)this["F_LASTEDIT"];
            }
            set
            {
                this["F_LASTEDIT"] = value;
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
    public class R_I054_ACK
    {
        public string ID { get; set; }
        public string F_ID { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string SALESORDERNUMBER { get; set; }
        public string SALESORDERLINENUMBER { get; set; }
        public string SERIALNUMBER { get; set; }
        public string MODELNUMBER { get; set; }
        public string RESPONSECODE { get; set; }
        public string RESPONSEMESSAGE { get; set; }
        public string TRANID { get; set; }
        public DateTime? F_LASTEDIT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }

    [SqlSugar.SugarTable("jnp.TB_I054R")]
    public class B2B_I054_ACK
    {
        public string F_ID { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string SALESORDERNUMBER { get; set; }
        public string SALESORDERLINENUMBER { get; set; }
        public string SERIALNUMBER { get; set; }
        public string MODELNUMBER { get; set; }
        public string RESPONSECODE { get; set; }
        public string RESPONSEMESSAGE { get; set; }
        public string TRANID { get; set; }
        public DateTime? F_LASTEDIT { get; set; }
    }
}