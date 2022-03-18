using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{
    public class T_R_OBASAMPLING_BYTIME : DataObjectTable
    {
        public T_R_OBASAMPLING_BYTIME(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_OBASAMPLING_BYTIME(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_OBASAMPLING_BYTIME);
            TableName = "R_OBASAMPLING_BYTIME".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_OBASAMPLING_BYTIME> GetSamplingList(OleExec sfcdb, string skuno, string version, string station,string effect)
        {
            return sfcdb.ORM.Queryable<R_OBASAMPLING_BYTIME>()
                .WhereIF(SqlSugar.SqlFunc.IsNullOrEmpty(skuno), r => r.SKUNO == skuno)
                .WhereIF(SqlSugar.SqlFunc.IsNullOrEmpty(version), r => r.VERSION == version)
                .WhereIF(SqlSugar.SqlFunc.IsNullOrEmpty(station), r => r.SAMPLING_EVENTPOINT == station)
                .WhereIF(SqlSugar.SqlFunc.IsNullOrEmpty(effect), r => r.EFFECT_FLAG == effect).ToList();
        }
    }
    public class Row_R_OBASAMPLING_BYTIME : DataObjectBase
    {
        public Row_R_OBASAMPLING_BYTIME(DataObjectInfo info) : base(info)
        {

        }
        public R_OBASAMPLING_BYTIME GetDataObject()
        {
            R_OBASAMPLING_BYTIME DataObject = new R_OBASAMPLING_BYTIME();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.SAMPLING_TYPE = this.SAMPLING_TYPE;
            DataObject.SAMPLING_EVENTPOINT = this.SAMPLING_EVENTPOINT;
            DataObject.EFFECT_FLAG = this.EFFECT_FLAG;
            DataObject.REASON_DESC = this.REASON_DESC;
            DataObject.SAMPLING_TIME = this.SAMPLING_TIME;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITTIME = this.LASTEDITTIME;
            DataObject.WORK_FLAG = this.WORK_FLAG;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
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
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string SAMPLING_TYPE
        {
            get
            {
                return (string)this["SAMPLING_TYPE"];
            }
            set
            {
                this["SAMPLING_TYPE"] = value;
            }
        }
        public string SAMPLING_EVENTPOINT
        {
            get
            {
                return (string)this["SAMPLING_EVENTPOINT"];
            }
            set
            {
                this["SAMPLING_EVENTPOINT"] = value;
            }
        }
        public string EFFECT_FLAG
        {
            get
            {
                return (string)this["EFFECT_FLAG"];
            }
            set
            {
                this["EFFECT_FLAG"] = value;
            }
        }
        public string REASON_DESC
        {
            get
            {
                return (string)this["REASON_DESC"];
            }
            set
            {
                this["REASON_DESC"] = value;
            }
        }
        public DateTime? SAMPLING_TIME
        {
            get
            {
                return (DateTime?)this["SAMPLING_TIME"];
            }
            set
            {
                this["SAMPLING_TIME"] = value;
            }
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
        public DateTime? LASTEDITTIME
        {
            get
            {
                return (DateTime?)this["LASTEDITTIME"];
            }
            set
            {
                this["LASTEDITTIME"] = value;
            }
        }
        public string WORK_FLAG
        {
            get
            {
                return (string)this["WORK_FLAG"];
            }
            set
            {
                this["WORK_FLAG"] = value;
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
    }
    public class R_OBASAMPLING_BYTIME
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string SAMPLING_TYPE { get; set; }
        public string SAMPLING_EVENTPOINT { get; set; }
        public string EFFECT_FLAG { get; set; }
        public string REASON_DESC { get; set; }
        public DateTime? SAMPLING_TIME { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITTIME { get; set; }
        public string WORK_FLAG { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
    }
}