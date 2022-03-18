using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_SFCATE_CONFIG_WIP : DataObjectTable
    {
        public T_R_SFCATE_CONFIG_WIP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SFCATE_CONFIG_WIP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SFCATE_CONFIG_WIP);
            TableName = "R_SFCATE_CONFIG_WIP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SFCATE_CONFIG_WIP : DataObjectBase
    {
        public Row_R_SFCATE_CONFIG_WIP(DataObjectInfo info) : base(info)
        {

        }
        public R_SFCATE_CONFIG_WIP GetDataObject()
        {
            R_SFCATE_CONFIG_WIP DataObject = new R_SFCATE_CONFIG_WIP();
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.DESC_REASION = this.DESC_REASION;
            DataObject.LOCK_STATUS = this.LOCK_STATUS;
            DataObject.LOCK_TIME = this.LOCK_TIME;
            DataObject.LOCK_BY = this.LOCK_BY;
            DataObject.UNLOCK_TIME = this.UNLOCK_TIME;
            DataObject.UNLOCK_BY = this.UNLOCK_BY;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.ID = this.ID;
            DataObject.ATENAME = this.ATENAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LOCATION = this.LOCATION;
            DataObject.STATION = this.STATION;
            DataObject.STATUS = this.STATUS;
            return DataObject;
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string DESC_REASION
        {
            get
            {
                return (string)this["DESC_REASION"];
            }
            set
            {
                this["DESC_REASION"] = value;
            }
        }
        public double? LOCK_STATUS
        {
            get
            {
                return (double?)this["LOCK_STATUS"];
            }
            set
            {
                this["LOCK_STATUS"] = value;
            }
        }
        public DateTime? LOCK_TIME
        {
            get
            {
                return (DateTime?)this["LOCK_TIME"];
            }
            set
            {
                this["LOCK_TIME"] = value;
            }
        }
        public string LOCK_BY
        {
            get
            {
                return (string)this["LOCK_BY"];
            }
            set
            {
                this["LOCK_BY"] = value;
            }
        }
        public DateTime? UNLOCK_TIME
        {
            get
            {
                return (DateTime?)this["UNLOCK_TIME"];
            }
            set
            {
                this["UNLOCK_TIME"] = value;
            }
        }
        public string UNLOCK_BY
        {
            get
            {
                return (string)this["UNLOCK_BY"];
            }
            set
            {
                this["UNLOCK_BY"] = value;
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
        public string ATENAME
        {
            get
            {
                return (string)this["ATENAME"];
            }
            set
            {
                this["ATENAME"] = value;
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
    }
    public class R_SFCATE_CONFIG_WIP
    {
        public string DESCRIPTION{ get; set; }
        public string DESC_REASION{ get; set; }
        public double? LOCK_STATUS{ get; set; }
        public DateTime? LOCK_TIME{ get; set; }
        public string LOCK_BY{ get; set; }
        public DateTime? UNLOCK_TIME{ get; set; }
        public string UNLOCK_BY{ get; set; }
        public string DATA1{ get; set; }
        public string DATA2{ get; set; }
        public string EDIT_EMP{ get; set; }
        public DateTime? EDIT_TIME{ get; set; }
        public string ID{ get; set; }
        public string ATENAME{ get; set; }
        public string SKUNO{ get; set; }
        public string LOCATION{ get; set; }
        public string STATION{ get; set; }
        public string STATUS{ get; set; }
    }
}