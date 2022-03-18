using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_PLAN_TEMP : DataObjectTable
    {
        public T_R_7B5_PLAN_TEMP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_PLAN_TEMP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_PLAN_TEMP);
            TableName = "R_7B5_PLAN_TEMP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_7B5_PLAN_TEMP> GetPlanTempList(OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_7B5_PLAN_TEMP>().ToList();
        }

        public void DeleteAllRecord(OleExec sfcdb)
        {
            sfcdb.ORM.Deleteable<R_7B5_PLAN_TEMP>().ExecuteCommand();
        }
    }
    public class Row_R_7B5_PLAN_TEMP : DataObjectBase
    {
        public Row_R_7B5_PLAN_TEMP(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_PLAN_TEMP GetDataObject()
        {
            R_7B5_PLAN_TEMP DataObject = new R_7B5_PLAN_TEMP();
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.DAY14 = this.DAY14;
            DataObject.DAY13 = this.DAY13;
            DataObject.DAY12 = this.DAY12;
            DataObject.DAY11 = this.DAY11;
            DataObject.DAY10 = this.DAY10;
            DataObject.DAY9 = this.DAY9;
            DataObject.DAY8 = this.DAY8;
            DataObject.DAY7 = this.DAY7;
            DataObject.DAY6 = this.DAY6;
            DataObject.DAY5 = this.DAY5;
            DataObject.DAY4 = this.DAY4;
            DataObject.DAY3 = this.DAY3;
            DataObject.DAY2 = this.DAY2;
            DataObject.DAY1 = this.DAY1;
            DataObject.ITEM = this.ITEM;
            DataObject.MODEL = this.MODEL;
            return DataObject;
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public double? DAY14
        {
            get
            {
                return (double?)this["DAY14"];
            }
            set
            {
                this["DAY14"] = value;
            }
        }
        public double? DAY13
        {
            get
            {
                return (double?)this["DAY13"];
            }
            set
            {
                this["DAY13"] = value;
            }
        }
        public double? DAY12
        {
            get
            {
                return (double?)this["DAY12"];
            }
            set
            {
                this["DAY12"] = value;
            }
        }
        public double? DAY11
        {
            get
            {
                return (double?)this["DAY11"];
            }
            set
            {
                this["DAY11"] = value;
            }
        }
        public double? DAY10
        {
            get
            {
                return (double?)this["DAY10"];
            }
            set
            {
                this["DAY10"] = value;
            }
        }
        public double? DAY9
        {
            get
            {
                return (double?)this["DAY9"];
            }
            set
            {
                this["DAY9"] = value;
            }
        }
        public double? DAY8
        {
            get
            {
                return (double?)this["DAY8"];
            }
            set
            {
                this["DAY8"] = value;
            }
        }
        public double? DAY7
        {
            get
            {
                return (double?)this["DAY7"];
            }
            set
            {
                this["DAY7"] = value;
            }
        }
        public double? DAY6
        {
            get
            {
                return (double?)this["DAY6"];
            }
            set
            {
                this["DAY6"] = value;
            }
        }
        public double? DAY5
        {
            get
            {
                return (double?)this["DAY5"];
            }
            set
            {
                this["DAY5"] = value;
            }
        }
        public double? DAY4
        {
            get
            {
                return (double?)this["DAY4"];
            }
            set
            {
                this["DAY4"] = value;
            }
        }
        public double? DAY3
        {
            get
            {
                return (double?)this["DAY3"];
            }
            set
            {
                this["DAY3"] = value;
            }
        }
        public double? DAY2
        {
            get
            {
                return (double?)this["DAY2"];
            }
            set
            {
                this["DAY2"] = value;
            }
        }
        public double? DAY1
        {
            get
            {
                return (double?)this["DAY1"];
            }
            set
            {
                this["DAY1"] = value;
            }
        }
        public string ITEM
        {
            get
            {
                return (string)this["ITEM"];
            }
            set
            {
                this["ITEM"] = value;
            }
        }
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
            }
        }
    }
    public class R_7B5_PLAN_TEMP
    {
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public double? DAY14 { get; set; }
        public double? DAY13 { get; set; }
        public double? DAY12 { get; set; }
        public double? DAY11 { get; set; }
        public double? DAY10 { get; set; }
        public double? DAY9 { get; set; }
        public double? DAY8 { get; set; }
        public double? DAY7 { get; set; }
        public double? DAY6 { get; set; }
        public double? DAY5 { get; set; }
        public double? DAY4 { get; set; }
        public double? DAY3 { get; set; }
        public double? DAY2 { get; set; }
        public double? DAY1 { get; set; }
        public string ITEM { get; set; }
        public string MODEL { get; set; }

    }
}