using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_COUNT : DataObjectTable
    {
        public T_R_REPAIR_COUNT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_COUNT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_COUNT);
            TableName = "R_REPAIR_COUNT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public R_REPAIR_COUNT GetSNFailStationCodeBYCount(string SN, string Station, string Code, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_COUNT>().Where(t => t.SN == SN && t.FAIL_CODE == Code && t.FAIL_STATION == Station).ToList().FirstOrDefault();
        }
        public int UpdateFailSNCountINFO(R_REPAIR_COUNT RepairSNINFO, string SN, string Station, string Code, OleExec DB)
        {
            return DB.ORM.Updateable<R_REPAIR_COUNT>(RepairSNINFO).Where(t => t.SN == SN && t.FAIL_CODE == Code && t.FAIL_STATION == Station).ExecuteCommand();
        }
        public int INFailSNCountINFO(R_REPAIR_COUNT RepairSNINFO, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_COUNT>(RepairSNINFO).ExecuteCommand();
        }
        public void NewEFox_ImportSysEvent_SPCheckIN(string SN, string BU, string EMP, ref Dictionary<string, string> DicRef, OleExec DB)
        {

        }
    }
    public class Row_R_REPAIR_COUNT : DataObjectBase
    {
        public Row_R_REPAIR_COUNT(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_COUNT GetDataObject()
        {
            R_REPAIR_COUNT DataObject = new R_REPAIR_COUNT();
            DataObject.ID = this.ID;
            DataObject.SN_ID = this.SN_ID;
            DataObject.SN = this.SN;
            DataObject.FAIL_STATION = this.FAIL_STATION;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.REPAIR_COUNT = this.REPAIR_COUNT;
            DataObject.REPAIR_CONTROL_NUM = this.REPAIR_CONTROL_NUM;
            DataObject.REPAIR_CONTROL_EMP = this.REPAIR_CONTROL_EMP;
            DataObject.REPAIR_CONTROL_TIME = this.REPAIR_CONTROL_TIME;
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
        public string SN_ID
        {
            get
            {
                return (string)this["SN_ID"];
            }
            set
            {
                this["SN_ID"] = value;
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
        public string FAIL_STATION
        {
            get
            {
                return (string)this["FAIL_STATION"];
            }
            set
            {
                this["FAIL_STATION"] = value;
            }
        }
        public string FAIL_CODE
        {
            get
            {
                return (string)this["FAIL_CODE"];
            }
            set
            {
                this["FAIL_CODE"] = value;
            }
        }
        public double? REPAIR_COUNT
        {
            get
            {
                return (double?)this["REPAIR_COUNT"];
            }
            set
            {
                this["REPAIR_COUNT"] = value;
            }
        }
        public double? REPAIR_CONTROL_NUM
        {
            get
            {
                return (double?)this["REPAIR_CONTROL_NUM"];
            }
            set
            {
                this["REPAIR_CONTROL_NUM"] = value;
            }
        }
        public string REPAIR_CONTROL_EMP
        {
            get
            {
                return (string)this["REPAIR_CONTROL_EMP"];
            }
            set
            {
                this["REPAIR_CONTROL_EMP"] = value;
            }
        }
        public DateTime? REPAIR_CONTROL_TIME
        {
            get
            {
                return (DateTime?)this["REPAIR_CONTROL_TIME"];
            }
            set
            {
                this["REPAIR_CONTROL_TIME"] = value;
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
    public class R_REPAIR_COUNT
    {
        public string ID { get; set; }
        public string SN_ID { get; set; }
        public string SN { get; set; }
        public string FAIL_STATION { get; set; }
        public string FAIL_CODE { get; set; }
        public double? REPAIR_COUNT { get; set; }
        public double? REPAIR_CONTROL_NUM { get; set; }
        public string REPAIR_CONTROL_EMP { get; set; }
        public DateTime? REPAIR_CONTROL_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}