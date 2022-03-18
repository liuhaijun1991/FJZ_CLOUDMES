using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_RANGE_DETAIL : DataObjectTable
    {
        public T_R_RANGE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RANGE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RANGE_DETAIL);
            TableName = "R_RANGE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_RANGE_DETAIL> GetAllDataByRuleId(OleExec DB, string ruleId)
        {
            return DB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.RULEID == ruleId).OrderBy(t => t.VALUE).ToList();
        }
    }
    public class Row_R_RANGE_DETAIL : DataObjectBase
    {
        public Row_R_RANGE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_RANGE_DETAIL GetDataObject()
        {
            R_RANGE_DETAIL DataObject = new R_RANGE_DETAIL();
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EXT5 = this.EXT5;
            DataObject.EXT4 = this.EXT4;
            DataObject.EXT3 = this.EXT3;
            DataObject.EXT2 = this.EXT2;
            DataObject.EXT1 = this.EXT1;
            DataObject.VALUE = this.VALUE;
            DataObject.RULEID = this.RULEID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EXT5
        {
            get
            {
                return (string)this["EXT5"];
            }
            set
            {
                this["EXT5"] = value;
            }
        }
        public string EXT4
        {
            get
            {
                return (string)this["EXT4"];
            }
            set
            {
                this["EXT4"] = value;
            }
        }
        public string EXT3
        {
            get
            {
                return (string)this["EXT3"];
            }
            set
            {
                this["EXT3"] = value;
            }
        }
        public string EXT2
        {
            get
            {
                return (string)this["EXT2"];
            }
            set
            {
                this["EXT2"] = value;
            }
        }
        public string EXT1
        {
            get
            {
                return (string)this["EXT1"];
            }
            set
            {
                this["EXT1"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public string RULEID
        {
            get
            {
                return (string)this["RULEID"];
            }
            set
            {
                this["RULEID"] = value;
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
    }
    public class R_RANGE_DETAIL
    {
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EXT5 { get; set; }
        public string EXT4 { get; set; }
        public string EXT3 { get; set; }
        public string EXT2 { get; set; }
        public string EXT1 { get; set; }
        public string VALUE { get; set; }
        public string RULEID { get; set; }
        public string ID { get; set; }
    }
}
