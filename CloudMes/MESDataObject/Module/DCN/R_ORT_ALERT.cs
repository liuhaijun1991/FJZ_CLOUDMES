using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORT_ALERT : DataObjectTable
    {
        public T_R_ORT_ALERT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORT_ALERT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORT_ALERT);
            TableName = "R_ORT_ALERT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORT_ALERT : DataObjectBase
    {
        public Row_R_ORT_ALERT(DataObjectInfo info) : base(info)
        {

        }
        public R_ORT_ALERT GetDataObject()
        {
            R_ORT_ALERT DataObject = new R_ORT_ALERT();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SN = this.SN;
            DataObject.SNID = this.SNID;
            DataObject.SKUNO_ORT = this.SKUNO_ORT;
            DataObject.SN_ORT = this.SN_ORT;
            DataObject.SCANREASON = this.SCANREASON;
            DataObject.ALERT_FLAG = this.ALERT_FLAG;
            DataObject.CONTROLBY = this.CONTROLBY;
            DataObject.CONTROLDT = this.CONTROLDT;
            DataObject.SCANBY = this.SCANBY;
            DataObject.SCANDT = this.SCANDT;
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
        public string SNID
        {
            get
            {
                return (string)this["SNID"];
            }
            set
            {
                this["SNID"] = value;
            }
        }
        public string SKUNO_ORT
        {
            get
            {
                return (string)this["SKUNO_ORT"];
            }
            set
            {
                this["SKUNO_ORT"] = value;
            }
        }
        public string SN_ORT
        {
            get
            {
                return (string)this["SN_ORT"];
            }
            set
            {
                this["SN_ORT"] = value;
            }
        }
        public string SCANREASON
        {
            get
            {
                return (string)this["SCANREASON"];
            }
            set
            {
                this["SCANREASON"] = value;
            }
        }
        public double? ALERT_FLAG
        {
            get
            {
                return (double?)this["ALERT_FLAG"];
            }
            set
            {
                this["ALERT_FLAG"] = value;
            }
        }
        public string CONTROLBY
        {
            get
            {
                return (string)this["CONTROLBY"];
            }
            set
            {
                this["CONTROLBY"] = value;
            }
        }
        public DateTime? CONTROLDT
        {
            get
            {
                return (DateTime?)this["CONTROLDT"];
            }
            set
            {
                this["CONTROLDT"] = value;
            }
        }
        public string SCANBY
        {
            get
            {
                return (string)this["SCANBY"];
            }
            set
            {
                this["SCANBY"] = value;
            }
        }
        public DateTime? SCANDT
        {
            get
            {
                return (DateTime?)this["SCANDT"];
            }
            set
            {
                this["SCANDT"] = value;
            }
        }
    }
    public class R_ORT_ALERT
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string SN { get; set; }
        public string SNID { get; set; }
        public string SKUNO_ORT { get; set; }
        public string SN_ORT { get; set; }
        public string SCANREASON { get; set; }
        public double? ALERT_FLAG { get; set; }
        public string CONTROLBY { get; set; }
        public DateTime? CONTROLDT { get; set; }
        public string SCANBY { get; set; }
        public DateTime? SCANDT { get; set; }
    }
}
