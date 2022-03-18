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
    public class T_C_UPH_SAMPLING : DataObjectTable
    {
        public T_C_UPH_SAMPLING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_UPH_SAMPLING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_UPH_SAMPLING);
            TableName = "C_UPH_SAMPLING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_UPH_SAMPLING : DataObjectBase
    {
        public Row_C_UPH_SAMPLING(DataObjectInfo info) : base(info)
        {

        }
        public C_UPH_SAMPLING GetDataObject()
        {
            C_UPH_SAMPLING DataObject = new C_UPH_SAMPLING();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION = this.STATION;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.SAMPLING_QTY = this.SAMPLING_QTY;
            DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
            DataObject.UPH = this.UPH;
            DataObject.REMARK = this.REMARK;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CREATE_EMP = this.CREATE_EMP;
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
        public double? LOT_QTY
        {
            get
            {
                return (double?)this["LOT_QTY"];
            }
            set
            {
                this["LOT_QTY"] = value;
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
        public string UPH
        {
            get
            {
                return (string)this["UPH"];
            }
            set
            {
                this["UPH"] = value;
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
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
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
    }
    public class C_UPH_SAMPLING
    {
        public string ID;
        public string SKUNO;
        public string STATION;
        public double? LOT_QTY;
        public double? SAMPLING_QTY;
        public string SAMPLING_TYPE;
        public string UPH;
        public string REMARK;
        public string DATA1;
        public string DATA2;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
        public DateTime? CREATE_TIME;
        public string CREATE_EMP;
    }
}