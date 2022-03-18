using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_PLAN_UPLOAD : DataObjectTable
    {
        public T_R_7B5_PLAN_UPLOAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_PLAN_UPLOAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_PLAN_UPLOAD);
            TableName = "R_7B5_PLAN_UPLOAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_7B5_PLAN_UPLOAD> GetUploadPlanListPCBA(OleExec sfcdb)
        {
            //return sfcdb.ORM.Queryable<R_7B5_PLAN_UPLOAD>().Where(r => SqlSugar.SqlFunc.Contains(r.PCBA, ",") && !SqlSugar.SqlFunc.StartsWith(r.PCBA, ",")).ToList();
            string sql = $@"SELECT * FROM r_7b5_plan_upload WHERE INSTR(pcba, ',') > 0";
            DataTable dt = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
            List<R_7B5_PLAN_UPLOAD> list = new List<R_7B5_PLAN_UPLOAD>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    Row_R_7B5_PLAN_UPLOAD row = (Row_R_7B5_PLAN_UPLOAD)NewRow();
                    row.loadData(r);
                    R_7B5_PLAN_UPLOAD rp = new R_7B5_PLAN_UPLOAD();
                    rp = row.GetDataObject();
                    list.Add(rp);
                }               
            }
            return list;
        }

        public int SaveUploadPlan(OleExec sfcdb, R_7B5_PLAN_UPLOAD uploadObj)
        {
            return sfcdb.ORM.Insertable<R_7B5_PLAN_UPLOAD>(uploadObj).ExecuteCommand();
        }

        public int UpdatePCBA(OleExec sfcdb,string new_pcba, string old_pcba, string item)
        {
            return sfcdb.ORM.Updateable<R_7B5_PLAN_UPLOAD>().UpdateColumns(r => new R_7B5_PLAN_UPLOAD { PCBA = new_pcba })
                .Where(r => r.PCBA == old_pcba && r.ITEM == item).ExecuteCommand();
        }
    }
    public class Row_R_7B5_PLAN_UPLOAD : DataObjectBase
    {
        public Row_R_7B5_PLAN_UPLOAD(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_PLAN_UPLOAD GetDataObject()
        {
            R_7B5_PLAN_UPLOAD DataObject = new R_7B5_PLAN_UPLOAD();
            DataObject.UPLOADBY = this.UPLOADBY;
            DataObject.UPLOADDT = this.UPLOADDT;
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
            DataObject.PCBA = this.PCBA;
            DataObject.ITEM = this.ITEM;
            DataObject.MODEL = this.MODEL;
            return DataObject;
        }
        public string UPLOADBY
        {
            get
            {
                return (string)this["UPLOADBY"];
            }
            set
            {
                this["UPLOADBY"] = value;
            }
        }
        public DateTime? UPLOADDT
        {
            get
            {
                return (DateTime?)this["UPLOADDT"];
            }
            set
            {
                this["UPLOADDT"] = value;
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
        public string PCBA
        {
            get
            {
                return (string)this["PCBA"];
            }
            set
            {
                this["PCBA"] = value;
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
    public class R_7B5_PLAN_UPLOAD
    {
        public string UPLOADBY { get; set; }
        public DateTime? UPLOADDT { get; set; }
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
        public string PCBA { get; set; }
        public string ITEM { get; set; }
        public string MODEL { get; set; }
    }
}