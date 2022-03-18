using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_NETGEAR_PTM_CTL : DataObjectTable
    {
        public T_R_NETGEAR_PTM_CTL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_NETGEAR_PTM_CTL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_NETGEAR_PTM_CTL);
            TableName = "R_NETGEAR_PTM_CTL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_NETGEAR_PTM_CTL : DataObjectBase
    {
        public Row_R_NETGEAR_PTM_CTL(DataObjectInfo info) : base(info)
        {

        }
        public R_NETGEAR_PTM_CTL GetDataObject()
        {
            R_NETGEAR_PTM_CTL DataObject = new R_NETGEAR_PTM_CTL();
            DataObject.ID = this.ID;
            DataObject.SHIPORDERID = this.SHIPORDERID;
            DataObject.DN = this.DN;
            DataObject.TONO = this.TONO;
            DataObject.PO = this.PO;
            DataObject.SHIPTOPART = this.SHIPTOPART;
            DataObject.ORGCODE = this.ORGCODE;
            DataObject.SHIPQTY = this.SHIPQTY;
            DataObject.PTMFILE = this.PTMFILE;
            DataObject.CQA = this.CQA;
            DataObject.CONVERTED = this.CONVERTED;
            DataObject.SENT = this.SENT;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
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
        public string SHIPORDERID
        {
            get
            {
                return (string)this["SHIPORDERID"];
            }
            set
            {
                this["SHIPORDERID"] = value;
            }
        }
        public string DN
        {
            get
            {
                return (string)this["DN"];
            }
            set
            {
                this["DN"] = value;
            }
        }
        public string TONO
        {
            get
            {
                return (string)this["TONO"];
            }
            set
            {
                this["TONO"] = value;
            }
        }
        public string PO
        {
            get
            {
                return (string)this["PO"];
            }
            set
            {
                this["PO"] = value;
            }
        }
        public string SHIPTOPART
        {
            get
            {
                return (string)this["SHIPTOPART"];
            }
            set
            {
                this["SHIPTOPART"] = value;
            }
        }
        public string ORGCODE
        {
            get
            {
                return (string)this["ORGCODE"];
            }
            set
            {
                this["ORGCODE"] = value;
            }
        }
        public double? SHIPQTY
        {
            get
            {
                return (double?)this["SHIPQTY"];
            }
            set
            {
                this["SHIPQTY"] = value;
            }
        }
        public string PTMFILE
        {
            get
            {
                return (string)this["PTMFILE"];
            }
            set
            {
                this["PTMFILE"] = value;
            }
        }
        public string CQA
        {
            get
            {
                return (string)this["CQA"];
            }
            set
            {
                this["CQA"] = value;
            }
        }
        public string CONVERTED
        {
            get
            {
                return (string)this["CONVERTED"];
            }
            set
            {
                this["CONVERTED"] = value;
            }
        }
        public string SENT
        {
            get
            {
                return (string)this["SENT"];
            }
            set
            {
                this["SENT"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
    }
    public class R_NETGEAR_PTM_CTL
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SHIPORDERID { get; set; }
        public string DN { get; set; }
        public string TONO { get; set; }
        public string PO { get; set; }
        public string SHIPTOPART { get; set; }
        public string ORGCODE { get; set; }
        public double? SHIPQTY { get; set; }
        public string PTMFILE { get; set; }
        public string CQA { get; set; }
        public string CONVERTED { get; set; }
        public string SENT { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }

    public enum ENUM_R_NETGEAR_PTM_CTL
    {
        /// <summary>
        /// EnumValue=0
        /// </summary>
        [EnumValue("0")]
        WAIT_CQA,
        /// <summary>
        /// EnumValue=1
        /// </summary>
        [EnumValue("1")]
        CQA_END,
        /// <summary>
        /// EnumValue=0
        /// </summary>
        [EnumValue("0")]
        WAIT_CONVER,
        /// <summary>
        /// EnumValue=1
        /// </summary>
        [EnumValue("1")]
        CONVER_END,
        /// <summary>
        /// EnumValue=0
        /// </summary>
        [EnumValue("0")]
        WAIT_SEND,
        /// <summary>
        /// EnumValue=1
        /// </summary>
        [EnumValue("1")]
        SEND_END
    }
}