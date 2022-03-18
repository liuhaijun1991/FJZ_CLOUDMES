using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_RMA : DataObjectTable
    {
        public T_R_QUACK_RMA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_RMA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_RMA);
            TableName = "R_QUACK_RMA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_QUACK_RMA> GetSNAndBySN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_RMA>().Where(t => (t.SN == SN || t.SN == "F" + SN.Substring(SN.Length - 10, 10))).ToList();
        }
    }
    public class Row_R_QUACK_RMA : DataObjectBase
    {
        public Row_R_QUACK_RMA(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_RMA GetDataObject()
        {
            R_QUACK_RMA DataObject = new R_QUACK_RMA();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.DF_SITE = this.DF_SITE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.BU = this.BU;
            DataObject.DF_INPUT_DATE = this.DF_INPUT_DATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string DF_SITE
        {
            get
            {
                return (string)this["DF_SITE"];
            }
            set
            {
                this["DF_SITE"] = value;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string DF_INPUT_DATE
        {
            get
            {
                return (string)this["DF_INPUT_DATE"];
            }
            set
            {
                this["DF_INPUT_DATE"] = value;
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
    public class R_QUACK_RMA
    {
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string DF_SITE { get; set; }
        public string SKUNO { get; set; }
        public string BU { get; set; }
        public string DF_INPUT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}