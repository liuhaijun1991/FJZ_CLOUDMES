using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_CONTROL_SKU : DataObjectTable
    {
        public T_C_CONTROL_SKU(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CONTROL_SKU(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CONTROL_SKU);
            TableName = "C_CONTROL_SKU".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// Add By Simon 2019/2/22
        /// In the old SFC system,it updated by elain 20121026 only when the subskuno is NC/C then check ,else ,no check rohs 
        /// </summary>
        /// <param name="subSkuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool SubSkunoIsNCOrC(string subSkuno,OleExec DB)
        {
            string sql = $@"select 1 from C_CONTROL_SKU where skuno='{subSkuno}' and control_flag in('0','1')";
            object res = DB.ExecSelectOneValue(sql);
            return res != null;
        }
        /// <summary>
        /// Add By Simon 2019/2/22
        /// get "no impact" skus,
        /// for no impact skus no need to check LH_NSDI_SILoadingRohsDataCheck
        /// </summary>
        /// <param name="mainSkuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_CONTROL_SKU> GetNoImpactSkusBySkuno(string mainSkuno, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL_SKU>().Where(t => t.SKUNO == mainSkuno && t.SKUNO == "2").ToList();
        }

        public List<C_CONTROL_SKU> Get500SkusBySkuno(string Skuno, OleExec DB)
        {
            return DB.ORM.Queryable<C_CONTROL_SKU>().Where(t => t.SKUNO == Skuno).ToList();
        }
    }
    public class Row_C_CONTROL_SKU : DataObjectBase
    {
        public Row_C_CONTROL_SKU(DataObjectInfo info) : base(info)
        {

        }
        public C_CONTROL_SKU GetDataObject()
        {
            C_CONTROL_SKU DataObject = new C_CONTROL_SKU();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CONTROL_FLAG = this.CONTROL_FLAG;
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
        public string CONTROL_FLAG
        {
            get
            {
                return (string)this["CONTROL_FLAG"];
            }
            set
            {
                this["CONTROL_FLAG"] = value;
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
    public class C_CONTROL_SKU
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string CONTROL_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}