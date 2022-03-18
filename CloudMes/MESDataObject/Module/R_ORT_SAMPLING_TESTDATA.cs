using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORT_SAMPLING_TESTDATA : DataObjectTable
    {
        public T_R_ORT_SAMPLING_TESTDATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORT_SAMPLING_TESTDATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORT_SAMPLING_TESTDATA);
            TableName = "R_ORT_SAMPLING_TESTDATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORT_SAMPLING_TESTDATA : DataObjectBase
    {
        public Row_R_ORT_SAMPLING_TESTDATA(DataObjectInfo info) : base(info)
        {

        }
        public R_ORT_SAMPLING_TESTDATA GetDataObject()
        {
            R_ORT_SAMPLING_TESTDATA DataObject = new R_ORT_SAMPLING_TESTDATA();
            DataObject.MACHINE = this.MACHINE;
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.STATION = this.STATION;
            DataObject.RESULT = this.RESULT;
            DataObject.TESTLOG = this.TESTLOG;
            DataObject.TESTDATE = this.TESTDATE;
            DataObject.TBS_NAME = this.TBS_NAME;
            DataObject.REPAIRED = this.REPAIRED;
            DataObject.ATE_NAME = this.ATE_NAME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string MACHINE
        {
            get
            {
                return (string)this["MACHINE"];
            }
            set
            {
                this["MACHINE"] = value;
            }
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
        public string RESULT
        {
            get
            {
                return (string)this["RESULT"];
            }
            set
            {
                this["RESULT"] = value;
            }
        }
        public string TESTLOG
        {
            get
            {
                return (string)this["TESTLOG"];
            }
            set
            {
                this["TESTLOG"] = value;
            }
        }
        public DateTime? TESTDATE
        {
            get
            {
                return (DateTime?)this["TESTDATE"];
            }
            set
            {
                this["TESTDATE"] = value;
            }
        }
        public string TBS_NAME
        {
            get
            {
                return (string)this["TBS_NAME"];
            }
            set
            {
                this["TBS_NAME"] = value;
            }
        }
        public string REPAIRED
        {
            get
            {
                return (string)this["REPAIRED"];
            }
            set
            {
                this["REPAIRED"] = value;
            }
        }
        public string ATE_NAME
        {
            get
            {
                return (string)this["ATE_NAME"];
            }
            set
            {
                this["ATE_NAME"] = value;
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
    public class R_ORT_SAMPLING_TESTDATA
    {
        public string MACHINE;
        public string ID;
        public string SN;
        public string STATION;
        public string RESULT;
        public string TESTLOG;
        public DateTime? TESTDATE;
        public string TBS_NAME;
        public string REPAIRED;
        public string ATE_NAME;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}