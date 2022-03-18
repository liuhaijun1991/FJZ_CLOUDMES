using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_REPLACE : DataObjectTable
    {
        public T_R_REPAIR_REPLACE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_REPLACE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_REPLACE);
            TableName = "R_REPAIR_REPLACE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_REPAIR_REPLACE> GetBYRepairNo(string RepairNO, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_REPLACE>().Where(t => t.REPLACE_NO == RepairNO).ToList();
        }
        public int InsertRepairReplace(R_REPAIR_REPLACE ReapriINFO, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_REPLACE>(ReapriINFO).ExecuteCommand();
        }
    }
    public class Row_R_REPAIR_REPLACE : DataObjectBase
    {
        public Row_R_REPAIR_REPLACE(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_REPLACE GetDataObject()
        {
            R_REPAIR_REPLACE DataObject = new R_REPAIR_REPLACE();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.SN_ID = this.SN_ID;
            DataObject.SN = this.SN;
            DataObject.FAIL_STATION = this.FAIL_STATION;
            DataObject.LOCATION = this.LOCATION;
            DataObject.TRACK_NO = this.TRACK_NO;
            DataObject.REPLACE_NO = this.REPLACE_NO;
            DataObject.FAIL_EMP = this.FAIL_EMP;
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
        public string TRACK_NO
        {
            get
            {
                return (string)this["TRACK_NO"];
            }
            set
            {
                this["TRACK_NO"] = value;
            }
        }
        public string REPLACE_NO
        {
            get
            {
                return (string)this["REPLACE_NO"];
            }
            set
            {
                this["REPLACE_NO"] = value;
            }
        }
        public string FAIL_EMP
        {
            get
            {
                return (string)this["FAIL_EMP"];
            }
            set
            {
                this["FAIL_EMP"] = value;
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
    public class R_REPAIR_REPLACE
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string SN_ID { get; set; }
        public string SN { get; set; }
        public string FAIL_STATION { get; set; }
        public string LOCATION { get; set; }
        public string TRACK_NO { get; set; }
        public string REPLACE_NO { get; set; }
        public string FAIL_EMP { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}