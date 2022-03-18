using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_C_OBA_SAMPLING : DataObjectTable
    {
        public T_C_OBA_SAMPLING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_OBA_SAMPLING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_OBA_SAMPLING);
            TableName = "C_OBA_SAMPLING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_OBA_SAMPLING GetSamplingObj(OleExec sfcdb, string model, string skuno, double qty)
        {
            return sfcdb.ORM.Queryable<C_OBA_SAMPLING>().Where(r => r.SKUNO == skuno && r.MODELNO == model && r.MINQTY <= qty && r.MAXQTY >= qty)
                .OrderBy(r => r.CREATEDATETIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }
    }
    public class Row_C_OBA_SAMPLING : DataObjectBase
    {
        public Row_C_OBA_SAMPLING(DataObjectInfo info) : base(info)
        {

        }
        public C_OBA_SAMPLING GetDataObject()
        {
            C_OBA_SAMPLING DataObject = new C_OBA_SAMPLING();
            DataObject.ID = this.ID;
            DataObject.MODELNO = this.MODELNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.MINQTY = this.MINQTY;
            DataObject.MAXQTY = this.MAXQTY;
            DataObject.ISALL = this.ISALL;
            DataObject.SAMPLINGQTY = this.SAMPLINGQTY;
            DataObject.REMARK = this.REMARK;
            DataObject.CREATEDATETIME = this.CREATEDATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string MODELNO
        {
            get
            {
                return (string)this["MODELNO"];
            }
            set
            {
                this["MODELNO"] = value;
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
        public double? MINQTY
        {
            get
            {
                return (double?)this["MINQTY"];
            }
            set
            {
                this["MINQTY"] = value;
            }
        }
        public double? MAXQTY
        {
            get
            {
                return (double?)this["MAXQTY"];
            }
            set
            {
                this["MAXQTY"] = value;
            }
        }
        public double? ISALL
        {
            get
            {
                return (double?)this["ISALL"];
            }
            set
            {
                this["ISALL"] = value;
            }
        }
        public double? SAMPLINGQTY
        {
            get
            {
                return (double?)this["SAMPLINGQTY"];
            }
            set
            {
                this["SAMPLINGQTY"] = value;
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
        public DateTime? CREATEDATETIME
        {
            get
            {
                return (DateTime?)this["CREATEDATETIME"];
            }
            set
            {
                this["CREATEDATETIME"] = value;
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
    }
    public class C_OBA_SAMPLING
    {
        public string ID { get; set; }
        public string MODELNO { get; set; }
        public string SKUNO { get; set; }
        public double? MINQTY { get; set; }
        public double? MAXQTY { get; set; }
        public double? ISALL { get; set; }
        public double? SAMPLINGQTY { get; set; }
        public string REMARK { get; set; }
        public DateTime? CREATEDATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}