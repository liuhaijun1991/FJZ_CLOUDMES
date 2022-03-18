using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ORT_SAMPLING_SKU : DataObjectTable
    {
        public T_C_ORT_SAMPLING_SKU(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ORT_SAMPLING_SKU(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ORT_SAMPLING_SKU);
            TableName = "C_ORT_SAMPLING_SKU".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_ORT_SAMPLING_SKU : DataObjectBase
    {
        public Row_C_ORT_SAMPLING_SKU(DataObjectInfo info) : base(info)
        {

        }
        public C_ORT_SAMPLING_SKU GetDataObject()
        {
            C_ORT_SAMPLING_SKU DataObject = new C_ORT_SAMPLING_SKU();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.MINQTY = this.MINQTY;
            DataObject.MAXQTY = this.MAXQTY;
            DataObject.SAMPLING_QTY = this.SAMPLING_QTY;
            DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
            DataObject.STATION = this.STATION;
            DataObject.LASTEDIT_EMP = this.LASTEDIT_EMP;
            DataObject.LASTEDIT_TIME = this.LASTEDIT_TIME;
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
        public double? SAMPLING_QTY
        {
            get
            {
                return (double?)this["SAMPLING_QTY"];
            }
            set
            {
                this["SAMPLING_QTY"] = value;
            }
        }
        public string SAMPLING_TYPE
        {
            get
            {
                return (string)this["SAMPLING_TYPE"];
            }
            set
            {
                this["SAMPLING_TYPE"] = value;
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
        public string LASTEDIT_EMP
        {
            get
            {
                return (string)this["LASTEDIT_EMP"];
            }
            set
            {
                this["LASTEDIT_EMP"] = value;
            }
        }
        public DateTime? LASTEDIT_TIME
        {
            get
            {
                return (DateTime?)this["LASTEDIT_TIME"];
            }
            set
            {
                this["LASTEDIT_TIME"] = value;
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
    public class C_ORT_SAMPLING_SKU
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public double? MINQTY{get;set;}
        public double? MAXQTY{get;set;}
        public double? SAMPLING_QTY{get;set;}
        public string SAMPLING_TYPE{get;set;}
        public string STATION{get;set;}
        public string LASTEDIT_EMP{get;set;}
        public DateTime? LASTEDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}