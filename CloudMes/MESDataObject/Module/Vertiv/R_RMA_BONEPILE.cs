using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_RMA_BONEPILE : DataObjectTable
    {
        public T_R_RMA_BONEPILE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RMA_BONEPILE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RMA_BONEPILE);
            TableName = "R_RMA_BONEPILE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_RMA_BONEPILE> GetRMA_BONEPILEs(OleExec sfcdb, string lotNo, string skuno, string sn)
        {
            var sql = sfcdb.ORM.Queryable<R_RMA_BONEPILE>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lotNo), t => t.LOT_NO.Contains(lotNo))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(skuno), t => t.SKUNO.Contains(skuno))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(sn), t => t.SN.Contains(sn)).OrderBy(t => t.LOT_NO).OrderBy(t => t.SN).ToSql();

            return sfcdb.ORM.Queryable<R_RMA_BONEPILE>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lotNo), t => t.LOT_NO.Contains(lotNo))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(skuno), t => t.SKUNO.Contains(skuno))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(sn), t => t.SN.Contains(sn)).OrderBy(t => t.LOT_NO).OrderBy(t => t.SN).ToList();
        }

        public void Update(List<R_SN> sNs, string empNo, OleExec sfcdb)
        {
            var sql = string.Empty;
            var dt = new DataTable();
            foreach (R_SN snobj in sNs)
            {
                sql = $@"select * from r_rma_bonepile where closed_flag = '0' and sn = '{snobj.SN}'";
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var dateTime = GetDBDateTime(sfcdb);
                    int i = sfcdb.ORM.Updateable<R_RMA_BONEPILE>()
                        .UpdateColumns(r => new R_RMA_BONEPILE { CLOSED_FLAG = "1", CLOSED_DATE = dateTime })
                        .Where(r => r.SN == snobj.SN && r.CLOSED_FLAG == "0").ExecuteCommand();
                }
            }
        }

        public bool IsInRmaBonepile(OleExec sfcdb, string sn)
        {
            return sfcdb.ORM.Queryable<R_RMA_BONEPILE>().Any(r => r.SN == sn);
        }

        public bool RmaBonepileIsOpen(OleExec sfcdb, string sn)
        {
            return sfcdb.ORM.Queryable<R_RMA_BONEPILE>().Any(r => r.SN == sn && r.CLOSED_FLAG == "0");
        }

        public R_RMA_BONEPILE GetOpenRecord(OleExec DB, string sn)
        {
            return DB.ORM.Queryable<R_RMA_BONEPILE>().Where(r => r.SN == sn && r.CLOSED_FLAG == "0").ToList().FirstOrDefault();
        }
    }
    public class Row_R_RMA_BONEPILE : DataObjectBase
    {
        public Row_R_RMA_BONEPILE(DataObjectInfo info) : base(info)
        {

        }
        public R_RMA_BONEPILE GetDataObject()
        {
            R_RMA_BONEPILE DataObject = new R_RMA_BONEPILE();
            DataObject.UPLOAD_TIME = this.UPLOAD_TIME;
            DataObject.UPLOAD_EMP = this.UPLOAD_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.CLOSED_DATE = this.CLOSED_DATE;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.COSMETIC_TIMES = this.COSMETIC_TIMES;
            DataObject.FUNCTION_TIMES = this.FUNCTION_TIMES;
            DataObject.RMA_TIMES = this.RMA_TIMES;
            DataObject.VALUABLE = this.VALUABLE;
            DataObject.REMARK = this.REMARK;
            DataObject.OWNER = this.OWNER;
            DataObject.FAILURE_TYPES = this.FAILURE_TYPES;
            DataObject.FAILURE_SYMPTOM = this.FAILURE_SYMPTOM;
            DataObject.FAIL_STATION = this.FAIL_STATION;
            DataObject.LASTPACK_DATE = this.LASTPACK_DATE;
            DataObject.RECEIVED_DATE = this.RECEIVED_DATE;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public DateTime? UPLOAD_TIME
        {
            get
            {
                return (DateTime?)this["UPLOAD_TIME"];
            }
            set
            {
                this["UPLOAD_TIME"] = value;
            }
        }
        public string UPLOAD_EMP
        {
            get
            {
                return (string)this["UPLOAD_EMP"];
            }
            set
            {
                this["UPLOAD_EMP"] = value;
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
        public DateTime? CLOSED_DATE
        {
            get
            {
                return (DateTime?)this["CLOSED_DATE"];
            }
            set
            {
                this["CLOSED_DATE"] = value;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public double? COSMETIC_TIMES
        {
            get
            {
                return (double?)this["COSMETIC_TIMES"];
            }
            set
            {
                this["COSMETIC_TIMES"] = value;
            }
        }
        public double? FUNCTION_TIMES
        {
            get
            {
                return (double?)this["FUNCTION_TIMES"];
            }
            set
            {
                this["FUNCTION_TIMES"] = value;
            }
        }
        public double? RMA_TIMES
        {
            get
            {
                return (double?)this["RMA_TIMES"];
            }
            set
            {
                this["RMA_TIMES"] = value;
            }
        }
        public string VALUABLE
        {
            get
            {
                return (string)this["VALUABLE"];
            }
            set
            {
                this["VALUABLE"] = value;
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
        public string OWNER
        {
            get
            {
                return (string)this["OWNER"];
            }
            set
            {
                this["OWNER"] = value;
            }
        }
        public string FAILURE_TYPES
        {
            get
            {
                return (string)this["FAILURE_TYPES"];
            }
            set
            {
                this["FAILURE_TYPES"] = value;
            }
        }
        public string FAILURE_SYMPTOM
        {
            get
            {
                return (string)this["FAILURE_SYMPTOM"];
            }
            set
            {
                this["FAILURE_SYMPTOM"] = value;
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
        public DateTime? LASTPACK_DATE
        {
            get
            {
                return (DateTime?)this["LASTPACK_DATE"];
            }
            set
            {
                this["LASTPACK_DATE"] = value;
            }
        }
        public DateTime? RECEIVED_DATE
        {
            get
            {
                return (DateTime?)this["RECEIVED_DATE"];
            }
            set
            {
                this["RECEIVED_DATE"] = value;
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
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
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
    public class R_RMA_BONEPILE
    {
        public DateTime? UPLOAD_TIME { get; set; }
        public string UPLOAD_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? CLOSED_DATE { get; set; }
        public string CLOSED_FLAG { get; set; }
        public double? COSMETIC_TIMES { get; set; }
        public double? FUNCTION_TIMES { get; set; }
        public double? RMA_TIMES { get; set; }
        public string VALUABLE { get; set; }
        public string REMARK { get; set; }
        public string OWNER { get; set; }
        public string FAILURE_TYPES { get; set; }
        public string FAILURE_SYMPTOM { get; set; }
        public string FAIL_STATION { get; set; }
        public DateTime? LASTPACK_DATE { get; set; }
        public DateTime? RECEIVED_DATE { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string LOT_NO { get; set; }
        public string ID { get; set; }
    }
}