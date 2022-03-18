using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_B2B_ACK : DataObjectTable
    {
        public T_O_B2B_ACK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_B2B_ACK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_B2B_ACK);
            TableName = "O_B2B_ACK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_B2B_ACK : DataObjectBase
    {
        public Row_O_B2B_ACK(DataObjectInfo info) : base(info)
        {

        }
        public O_B2B_ACK GetDataObject()
        {
            O_B2B_ACK DataObject = new O_B2B_ACK();
            DataObject.EXCEPTIONTYPE = this.EXCEPTIONTYPE;
            DataObject.F_MSG = this.F_MSG;
            DataObject.PARTNERID = this.PARTNERID;
            DataObject.F_LASTEDIT = this.F_LASTEDIT;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            //DataObject.F_SFC_ID = this.F_SFC_ID;
            DataObject.F_MSG_TYPE = this.F_MSG_TYPE;
            DataObject.F_DOC_NO = this.F_DOC_NO;
            return DataObject;
        }
        public string EXCEPTIONTYPE
        {
            get
            {
                return (string)this["EXCEPTIONTYPE"];
            }
            set
            {
                this["EXCEPTIONTYPE"] = value;
            }
        }
        public string F_MSG
        {
            get
            {
                return (string)this["F_MSG"];
            }
            set
            {
                this["F_MSG"] = value;
            }
        }
        public string PARTNERID
        {
            get
            {
                return (string)this["PARTNERID"];
            }
            set
            {
                this["PARTNERID"] = value;
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
        public string F_SFC_ID
        {
            get
            {
                return (string)this["F_SFC_ID"];
            }
            set
            {
                this["F_SFC_ID"] = value;
            }
        }
        public string F_MSG_TYPE
        {
            get
            {
                return (string)this["F_MSG_TYPE"];
            }
            set
            {
                this["F_MSG_TYPE"] = value;
            }
        }
        public string F_DOC_NO
        {
            get
            {
                return (string)this["F_DOC_NO"];
            }
            set
            {
                this["F_DOC_NO"] = value;
            }
        }
    }
    public class O_B2B_ACK
    {
        public string EXCEPTIONTYPE { get; set; }
        public string F_MSG { get; set; }
        public string PARTNERID { get; set; }
        public DateTime? F_LASTEDIT { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string ID { get; set; }
        public string F_ID { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string TRANID { get; set; }
        public string F_MSG_TYPE { get; set; }
        public string F_DOC_NO { get; set; }
    }

    [SqlSugar.SugarTable("jnp.TB_ACK")]
    public class B2B_ACK
    {
        public string EXCEPTIONTYPE { get; set; }
        public string F_MSG { get; set; }
        public string PARTNERID { get; set; }
        public DateTime? F_LASTEDIT { get; set; }
        public string F_ID { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string TRANID { get; set; }
        public string F_MSG_TYPE { get; set; }
        public string F_DOC_NO { get; set; }
    }
}