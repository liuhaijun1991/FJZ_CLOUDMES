using System;
using System.Collections.Generic;
using System.Data;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_RANGE_RULE : DataObjectTable
    {
        public T_R_RANGE_RULE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RANGE_RULE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RANGE_RULE);
            TableName = "R_RANGE_RULE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public DataTable GetAllData(OleExec DB)
        {
            return DB.ORM.Queryable<R_RANGE_RULE>().OrderBy(sd => sd.RULEID, SqlSugar.OrderByType.Asc).OrderBy(sd => sd.MIN, SqlSugar.OrderByType.Asc).ToDataTable();
        }
        public bool CheckExisted(OleExec DB, string minRange, string maxRange)
        {
            List<R_RANGE_RULE> ruleList = DB.ORM.Queryable<R_RANGE_RULE>().Where(t => t.VALID == "Y").OrderBy(t => t.ID).ToList();
            if (ruleList.Count == 0)
            {
                return false;
            }
            else
            {
                bool existed = false;
                for (int i = 0; i < ruleList.Count; i++)
                {
                    int min = Convert.ToInt32(ruleList[i].MIN, 16);
                    int max = Convert.ToInt32(ruleList[i].MAX, 16);

                    //如果輸入的最小區間大於存在的最小區間且小於存在的最大區間
                    //如果輸入的最大區間大於存在的最小區間且小於存在的最大區間
                    //如果輸入的最小區間等於存在的最小區間
                    //如果輸入的最大區間等於存在的最大區間
                    if ((Convert.ToInt32(minRange, 16) > min && Convert.ToInt32(minRange, 16) < max) || 
                        (Convert.ToInt32(maxRange, 16) > min && Convert.ToInt32(maxRange, 16) < max) || 
                        Convert.ToInt32(minRange, 16) == min || Convert.ToInt32(maxRange, 16) == max)
                    {
                        existed = true;
                    }
                    else
                    {
                        existed = false;
                    }
                }
                return existed;
            }
        }
    }
    public class Row_R_RANGE_RULE : DataObjectBase
    {
        public Row_R_RANGE_RULE(DataObjectInfo info) : base(info)
        {

        }
        public R_RANGE_RULE GetDataObject()
        {
            R_RANGE_RULE DataObject = new R_RANGE_RULE();
            DataObject.QTY = this.QTY;
            DataObject.CURVAL = this.CURVAL;
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.COMPLITED = this.COMPLITED;
            DataObject.VALID = this.VALID;
            DataObject.MAX = this.MAX;
            DataObject.MIN = this.MIN;
            DataObject.RULEID = this.RULEID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string CURVAL
        {
            get
            {
                return (string)this["CURVAL"];
            }
            set
            {
                this["CURVAL"] = value;
            }
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
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public string COMPLITED
        {
            get
            {
                return (string)this["COMPLITED"];
            }
            set
            {
                this["COMPLITED"] = value;
            }
        }
        public string VALID
        {
            get
            {
                return (string)this["VALID"];
            }
            set
            {
                this["VALID"] = value;
            }
        }
        public string MAX
        {
            get
            {
                return (string)this["MAX"];
            }
            set
            {
                this["MAX"] = value;
            }
        }
        public string MIN
        {
            get
            {
                return (string)this["MIN"];
            }
            set
            {
                this["MIN"] = value;
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
    public class R_RANGE_RULE
    {
        public double? QTY { get; set; }
        public string CURVAL { get; set; }
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string COMPLITED { get; set; }
        public string VALID { get; set; }
        public string MAX { get; set; }
        public string MIN { get; set; }
        public string RULEID { get; set; }
        public string ID { get; set; }
    }
}
