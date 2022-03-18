using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BGA_DETAIL : DataObjectTable
    {
        public T_R_BGA_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BGA_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BGA_DETAIL);
            TableName = "R_BGA_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_BGA_DETAIL : DataObjectBase
    {
        public Row_R_BGA_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_BGA_DETAIL GetDataObject()
        {
            R_BGA_DETAIL DataObject = new R_BGA_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.BGA_LOCATION = this.BGA_LOCATION;
            DataObject.STATION = this.STATION;
            DataObject.ACTION = this.ACTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.LOCATION = this.LOCATION;
            DataObject.COMPONENT_ID = this.COMPONENT_ID;
            DataObject.XRAYRESULT = this.XRAYRESULT;
            DataObject.DEBUGRESULT = this.DEBUGRESULT;
            DataObject.TESTRESULT = this.TESTRESULT;
            DataObject.REMARK = this.REMARK;
            DataObject.REPAIR_ID = this.REPAIR_ID;
            DataObject.REWORK_NUM = this.REWORK_NUM;
            DataObject.BAKE_FLAG = this.BAKE_FLAG;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.BAKE_START = this.BAKE_START;
            DataObject.BAKE_END = this.BAKE_END;
            DataObject.BAKE_ID = this.BAKE_ID;
            DataObject.BGA_TYPE = this.BGA_TYPE;
            DataObject.HOURS = this.HOURS;
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
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string BGA_LOCATION
        {
            get
            {
                return (string)this["BGA_LOCATION"];
            }
            set
            {
                this["BGA_LOCATION"] = value;
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
        public string ACTION
        {
            get
            {
                return (string)this["ACTION"];
            }
            set
            {
                this["ACTION"] = value;
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
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string COMPONENT_ID
        {
            get
            {
                return (string)this["COMPONENT_ID"];
            }
            set
            {
                this["COMPONENT_ID"] = value;
            }
        }
        public string XRAYRESULT
        {
            get
            {
                return (string)this["XRAYRESULT"];
            }
            set
            {
                this["XRAYRESULT"] = value;
            }
        }
        public string DEBUGRESULT
        {
            get
            {
                return (string)this["DEBUGRESULT"];
            }
            set
            {
                this["DEBUGRESULT"] = value;
            }
        }
        public string TESTRESULT
        {
            get
            {
                return (string)this["TESTRESULT"];
            }
            set
            {
                this["TESTRESULT"] = value;
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
        public string REPAIR_ID
        {
            get
            {
                return (string)this["REPAIR_ID"];
            }
            set
            {
                this["REPAIR_ID"] = value;
            }
        }
        public string REWORK_NUM
        {
            get
            {
                return (string)this["REWORK_NUM"];
            }
            set
            {
                this["REWORK_NUM"] = value;
            }
        }
        public string BAKE_FLAG
        {
            get
            {
                return (string)this["BAKE_FLAG"];
            }
            set
            {
                this["BAKE_FLAG"] = value;
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
        public DateTime? BAKE_START
        {
            get
            {
                return (DateTime?)this["BAKE_START"];
            }
            set
            {
                this["BAKE_START"] = value;
            }
        }
        public DateTime? BAKE_END
        {
            get
            {
                return (DateTime?)this["BAKE_END"];
            }
            set
            {
                this["BAKE_END"] = value;
            }
        }
        public string BAKE_ID
        {
            get
            {
                return (string)this["BAKE_ID"];
            }
            set
            {
                this["BAKE_ID"] = value;
            }
        }
        public string BGA_TYPE
        {
            get
            {
                return (string)this["BGA_TYPE"];
            }
            set
            {
                this["BGA_TYPE"] = value;
            }
        }
        public string HOURS
        {
            get
            {
                return (string)this["HOURS"];
            }
            set
            {
                this["HOURS"] = value;
            }
        }
    }
    public class R_BGA_DETAIL
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string PARTNO { get; set; }
        public string BGA_LOCATION { get; set; }
        public string STATION { get; set; }
        public string ACTION { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string LOCATION { get; set; }
        public string COMPONENT_ID { get; set; }
        public string XRAYRESULT { get; set; }
        public string DEBUGRESULT { get; set; }
        public string TESTRESULT { get; set; }
        public string REMARK { get; set; }
        public string REPAIR_ID { get; set; }
        public string REWORK_NUM { get; set; }
        public string BAKE_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public DateTime? BAKE_START { get; set; }
        public DateTime? BAKE_END { get; set; }
        public string BAKE_ID { get; set; }
        public string BGA_TYPE { get; set; }
        public string HOURS { get; set; }
    }
}