using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SN_PASS : DataObjectTable
    {
        public T_R_SN_PASS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_PASS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_PASS);
            TableName = "R_SN_PASS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SN_PASS : DataObjectBase
    {
        public Row_R_SN_PASS(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_PASS GetDataObject()
        {
            R_SN_PASS DataObject = new R_SN_PASS();
            DataObject.ID = this.ID;
            DataObject.LOTNO = this.LOTNO;
            DataObject.SN = this.SN;
            DataObject.TYPE = this.TYPE;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PASS_STATION = this.PASS_STATION;
            DataObject.STATUS = this.STATUS;
            DataObject.REASON = this.REASON;
            DataObject.CANCEL_REASON = this.CANCEL_REASON;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CANCEL_EMP = this.CANCEL_EMP;
            DataObject.CANCEL_TIME = this.CANCEL_TIME;
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
        public string LOTNO
        {
            get
            {
                return (string)this["LOTNO"];
            }
            set
            {
                this["LOTNO"] = value;
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
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
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
        public string PASS_STATION
        {
            get
            {
                return (string)this["PASS_STATION"];
            }
            set
            {
                this["PASS_STATION"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string REASON
        {
            get
            {
                return (string)this["REASON"];
            }
            set
            {
                this["REASON"] = value;
            }
        }
        public string CANCEL_REASON
        {
            get
            {
                return (string)this["CANCEL_REASON"];
            }
            set
            {
                this["CANCEL_REASON"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public string CANCEL_EMP
        {
            get
            {
                return (string)this["CANCEL_EMP"];
            }
            set
            {
                this["CANCEL_EMP"] = value;
            }
        }
        public DateTime? CANCEL_TIME
        {
            get
            {
                return (DateTime?)this["CANCEL_TIME"];
            }
            set
            {
                this["CANCEL_TIME"] = value;
            }
        }
    }
    public class R_SN_PASS
    {
        public string ID { get; set; }
        public string LOTNO { get; set; }
        public string SN { get; set; }
        public string TYPE { get; set; }
        public string WORKORDERNO { get; set; }
        public string PASS_STATION { get; set; }
        public string STATUS { get; set; }
        public string REASON { get; set; }
        public string CANCEL_REASON { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string CANCEL_EMP { get; set; }
        public DateTime? CANCEL_TIME { get; set; }
    }
}