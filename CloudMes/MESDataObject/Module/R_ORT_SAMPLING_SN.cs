using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORT_SAMPLING_SN : DataObjectTable
    {
        public T_R_ORT_SAMPLING_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORT_SAMPLING_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORT_SAMPLING_SN);
            TableName = "R_ORT_SAMPLING_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORT_SAMPLING_SN : DataObjectBase
    {
        public Row_R_ORT_SAMPLING_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_ORT_SAMPLING_SN GetDataObject()
        {
            R_ORT_SAMPLING_SN DataObject = new R_ORT_SAMPLING_SN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.STATION = this.STATION;
            DataObject.FLAG = this.FLAG;
            DataObject.SAMPLING_TIME = this.SAMPLING_TIME;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
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
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
            }
        }
        public DateTime? SAMPLING_TIME
        {
            get
            {
                return (DateTime?)this["SAMPLING_TIME"];
            }
            set
            {
                this["SAMPLING_TIME"] = value;
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
    public class R_ORT_SAMPLING_SN
    {
        public string ID;
        public string SN;
        public string WO;
        public string STATION;
        public string FLAG;
        public DateTime? SAMPLING_TIME;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}