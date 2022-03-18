using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_TYPE_SAMPLING : DataObjectTable
    {
        public T_C_TYPE_SAMPLING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_TYPE_SAMPLING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_TYPE_SAMPLING);
            TableName = "C_TYPE_SAMPLING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_TYPE_SAMPLING : DataObjectBase
    {
        public Row_C_TYPE_SAMPLING(DataObjectInfo info) : base(info)
        {

        }
        public C_TYPE_SAMPLING GetDataObject()
        {
            C_TYPE_SAMPLING DataObject = new C_TYPE_SAMPLING();
            DataObject.ID = this.ID;
            DataObject.TYPE = this.TYPE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.MINQTY = this.MINQTY;
            DataObject.MAXQTY = this.MAXQTY;
            DataObject.SAMPLINGQTY = this.SAMPLINGQTY;
            DataObject.REMARK = this.REMARK;
            DataObject.CREATEDATETIME = this.CREATEDATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public double? MINQTY
        {
            get
            {
                return (double?)this["MINQTY"];
            }
            set
            {
                this["MINQTY"] = value;
            }
        }
        public double? MAXQTY
        {
            get
            {
                return (double?)this["MAXQTY"];
            }
            set
            {
                this["MAXQTY"] = value;
            }
        }
        public double? SAMPLINGQTY
        {
            get
            {
                return (double?)this["SAMPLINGQTY"];
            }
            set
            {
                this["SAMPLINGQTY"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public DateTime? CREATEDATETIME
        {
            get
            {
                return (DateTime?)this["CREATEDATETIME"];
            }
            set
            {
                this["CREATEDATETIME"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
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
    }
    public class C_TYPE_SAMPLING
    {
        public string ID;
        public string TYPE;
        public string SKUNO;
        public double? MINQTY;
        public double? MAXQTY;
        public double? SAMPLINGQTY;
        public string REMARK;
        public DateTime? CREATEDATETIME;
        public string CREATEBY;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}