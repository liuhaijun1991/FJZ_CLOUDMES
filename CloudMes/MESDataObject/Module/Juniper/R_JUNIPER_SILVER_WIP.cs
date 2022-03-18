using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JUNIPER_SILVER_WIP : DataObjectTable
    {
        public T_R_JUNIPER_SILVER_WIP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JUNIPER_SILVER_WIP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JUNIPER_SILVER_WIP);
            TableName = "R_JUNIPER_SILVER_WIP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JUNIPER_SILVER_WIP : DataObjectBase
    {
        public Row_R_JUNIPER_SILVER_WIP(DataObjectInfo info) : base(info)
        {

        }
        public R_JUNIPER_SILVER_WIP GetDataObject()
        {
            R_JUNIPER_SILVER_WIP DataObject = new R_JUNIPER_SILVER_WIP();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.START_TIME = this.START_TIME;
            DataObject.IN_WIP_USER = this.IN_WIP_USER;
            DataObject.END_TIME = this.END_TIME;
            DataObject.OUT_WIP_USER = this.OUT_WIP_USER;
            DataObject.SKUNO = this.SKUNO;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.STATE_FLAG = this.STATE_FLAG;
            DataObject.TEST_HOURS = this.TEST_HOURS;
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
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public string IN_WIP_USER
        {
            get
            {
                return (string)this["IN_WIP_USER"];
            }
            set
            {
                this["IN_WIP_USER"] = value;
            }
        }
        public DateTime? END_TIME
        {
            get
            {
                return (DateTime?)this["END_TIME"];
            }
            set
            {
                this["END_TIME"] = value;
            }
        }
        public string OUT_WIP_USER
        {
            get
            {
                return (string)this["OUT_WIP_USER"];
            }
            set
            {
                this["OUT_WIP_USER"] = value;
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
        public string STATE_FLAG
        {
            get
            {
                return (string)this["STATE_FLAG"];
            }
            set
            {
                this["STATE_FLAG"] = value;
            }
        }
        public double? TEST_HOURS
        {
            get
            {
                return (double?)this["TEST_HOURS"];
            }
            set
            {
                this["TEST_HOURS"] = value;
            }
        }
    }
    public class R_JUNIPER_SILVER_WIP
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public DateTime? START_TIME { get; set; }
        public string IN_WIP_USER { get; set; }
        public DateTime? END_TIME { get; set; }
        public string OUT_WIP_USER { get; set; }
        public string SKUNO { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string STATE_FLAG { get; set; }
        public double? TEST_HOURS { get; set; }
    }
}
