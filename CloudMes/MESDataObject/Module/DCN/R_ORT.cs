using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORT : DataObjectTable
    {
        public T_R_ORT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORT);
            TableName = "R_ORT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORT : DataObjectBase
    {
        public Row_R_ORT(DataObjectInfo info) : base(info)
        {

        }
        public R_ORT GetDataObject()
        {
            R_ORT DataObject = new R_ORT();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SNID = this.SNID;
            DataObject.ORTEVENT = this.ORTEVENT;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.REASONCODE = this.REASONCODE;
            DataObject.COUNTER = this.COUNTER;
            DataObject.SENDFLAG = this.SENDFLAG;
            DataObject.MDSTIME = this.MDSTIME;
            DataObject.WORKTIME = this.WORKTIME;
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
        public string ORTEVENT
        {
            get
            {
                return (string)this["ORTEVENT"];
            }
            set
            {
                this["ORTEVENT"] = value;
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
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string REASONCODE
        {
            get
            {
                return (string)this["REASONCODE"];
            }
            set
            {
                this["REASONCODE"] = value;
            }
        }
        public double? COUNTER
        {
            get
            {
                return (double?)this["COUNTER"];
            }
            set
            {
                this["COUNTER"] = value;
            }
        }
        public string SENDFLAG
        {
            get
            {
                return (string)this["SENDFLAG"];
            }
            set
            {
                this["SENDFLAG"] = value;
            }
        }
        public DateTime? MDSTIME
        {
            get
            {
                return (DateTime?)this["MDSTIME"];
            }
            set
            {
                this["MDSTIME"] = value;
            }
        }
        public DateTime? WORKTIME
        {
            get
            {
                return (DateTime?)this["WORKTIME"];
            }
            set
            {
                this["WORKTIME"] = value;
            }
        }
    }
    public class R_ORT
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SNID { get; set; }
        public string ORTEVENT { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string REASONCODE { get; set; }
        public double? COUNTER { get; set; }
        public string SENDFLAG { get; set; }
        public DateTime? MDSTIME { get; set; }
        public DateTime? WORKTIME { get; set; }
    }
}
