using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_COST : DataObjectTable
    {
        public T_R_QUACK_COST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_COST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_COST);
            TableName = "R_QUACK_COST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_QUACK_COST> GetSNCheckin(string QSN, string Checkin, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_COST>().Where(x => x.QSN == QSN && x.CHECKIN_FLAG == Checkin).ToList();
        }
    }
    public class Row_R_QUACK_COST : DataObjectBase
    {
        public Row_R_QUACK_COST(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_COST GetDataObject()
        {
            R_QUACK_COST DataObject = new R_QUACK_COST();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.QSN = this.QSN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CHECKIN_FLAG = this.CHECKIN_FLAG;
            DataObject.CHECKIN_TIME = this.CHECKIN_TIME;
            DataObject.CHECKOUT_FLAG = this.CHECKOUT_FLAG;
            DataObject.CHECKOUT_TIME = this.CHECKOUT_TIME;
            DataObject.COST_CODE = this.COST_CODE;
            DataObject.STATUS = this.STATUS;
            DataObject.RMA_FLAG = this.RMA_FLAG;
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
        public string QSN
        {
            get
            {
                return (string)this["QSN"];
            }
            set
            {
                this["QSN"] = value;
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
        public string CHECKIN_FLAG
        {
            get
            {
                return (string)this["CHECKIN_FLAG"];
            }
            set
            {
                this["CHECKIN_FLAG"] = value;
            }
        }
        public DateTime? CHECKIN_TIME
        {
            get
            {
                return (DateTime?)this["CHECKIN_TIME"];
            }
            set
            {
                this["CHECKIN_TIME"] = value;
            }
        }
        public string CHECKOUT_FLAG
        {
            get
            {
                return (string)this["CHECKOUT_FLAG"];
            }
            set
            {
                this["CHECKOUT_FLAG"] = value;
            }
        }
        public DateTime? CHECKOUT_TIME
        {
            get
            {
                return (DateTime?)this["CHECKOUT_TIME"];
            }
            set
            {
                this["CHECKOUT_TIME"] = value;
            }
        }
        public string COST_CODE
        {
            get
            {
                return (string)this["COST_CODE"];
            }
            set
            {
                this["COST_CODE"] = value;
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
        public string RMA_FLAG
        {
            get
            {
                return (string)this["RMA_FLAG"];
            }
            set
            {
                this["RMA_FLAG"] = value;
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
    public class R_QUACK_COST
    {
        public string ID;
        public string SN;
        public string QSN;
        public string SKUNO;
        public string CHECKIN_FLAG;
        public DateTime? CHECKIN_TIME;
        public string CHECKOUT_FLAG;
        public DateTime? CHECKOUT_TIME;
        public string COST_CODE;
        public string STATUS;
        public string RMA_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}