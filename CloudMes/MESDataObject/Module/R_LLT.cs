using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_LLT : DataObjectTable
    {
        public T_R_LLT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LLT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LLT);
            TableName = "R_LLT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_LLT : DataObjectBase
    {
        public Row_R_LLT(DataObjectInfo info) : base(info)
        {

        }
        public R_LLT GetDataObject()
        {
            R_LLT DataObject = new R_LLT();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.STATUS = this.STATUS;
            DataObject.STATION = this.STATION;
            DataObject.INTIME = this.INTIME;
            DataObject.OUTTIME = this.OUTTIME;
            DataObject.CANCELTIME = this.CANCELTIME;
            DataObject.CANCELBY = this.CANCELBY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.SUPPLEMENT = this.SUPPLEMENT;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public DateTime? INTIME
        {
            get
            {
                return (DateTime?)this["INTIME"];
            }
            set
            {
                this["INTIME"] = value;
            }
        }
        public DateTime? OUTTIME
        {
            get
            {
                return (DateTime?)this["OUTTIME"];
            }
            set
            {
                this["OUTTIME"] = value;
            }
        }
        public DateTime? CANCELTIME
        {
            get
            {
                return (DateTime?)this["CANCELTIME"];
            }
            set
            {
                this["CANCELTIME"] = value;
            }
        }
        public string CANCELBY
        {
            get
            {
                return (string)this["CANCELBY"];
            }
            set
            {
                this["CANCELBY"] = value;
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
        public string SUPPLEMENT
        {
            get
            {
                return (string)this["SUPPLEMENT"];
            }
            set
            {
                this["SUPPLEMENT"] = value;
            }
        }
    }
    public class R_LLT
    {
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string STATUS { get; set; }
        public string STATION { get; set; }
        public DateTime? INTIME { get; set; }
        public DateTime? OUTTIME { get; set; }
        public DateTime? CANCELTIME { get; set; }
        public string CANCELBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string SUPPLEMENT { get; set; }
    }
}