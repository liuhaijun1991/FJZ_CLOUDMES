using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_C_SKU_7B5_CONFIG : DataObjectTable
    {
        public T_C_SKU_7B5_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_7B5_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_7B5_CONFIG);
            TableName = "C_SKU_7B5_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SKU_7B5_CONFIG> GetListByType(OleExec sfcdb, string type)
        {
            return sfcdb.ORM.Queryable<C_SKU_7B5_CONFIG>().Where(r => r.TYPE == type).OrderBy(r => r.LASTEDITDT, SqlSugar.OrderByType.Desc).ToList();
        }
        public C_SKU_7B5_CONFIG GetListByTypeAndSkuno(OleExec sfcdb, string type,string skuno)
        {
            return sfcdb.ORM.Queryable<C_SKU_7B5_CONFIG>().Where(r => r.TYPE == type && r.SKUNO == skuno).ToList().FirstOrDefault();
        }
        public int SaveNewModel(OleExec sfcdb, C_SKU_7B5_CONFIG configModel)
        {
            return sfcdb.ORM.Insertable<C_SKU_7B5_CONFIG>(configModel).ExecuteCommand();
        }
    }
    public class Row_C_SKU_7B5_CONFIG : DataObjectBase
    {
        public Row_C_SKU_7B5_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_7B5_CONFIG GetDataObject()
        {
            C_SKU_7B5_CONFIG DataObject = new C_SKU_7B5_CONFIG();
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA1 = this.DATA1;
            DataObject.VIR_FLAG = this.VIR_FLAG;
            DataObject.UPD = this.UPD;
            DataObject.SKUNO = this.SKUNO;
            DataObject.TYPE = this.TYPE;
            return DataObject;
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
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
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
        public string VIR_FLAG
        {
            get
            {
                return (string)this["VIR_FLAG"];
            }
            set
            {
                this["VIR_FLAG"] = value;
            }
        }
        public double? UPD
        {
            get
            {
                return (double?)this["UPD"];
            }
            set
            {
                this["UPD"] = value;
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
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
    }
    public class C_SKU_7B5_CONFIG
    {
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
        public string DATA3 { get; set; }
        public string DATA2 { get; set; }
        public string DATA1 { get; set; }
        public string VIR_FLAG { get; set; }
        public double? UPD { get; set; }
        public string SKUNO { get; set; }
        public string TYPE { get; set; }
    }
}