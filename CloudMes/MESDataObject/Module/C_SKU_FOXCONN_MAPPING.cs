using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module
{
    /// <summary>
    /// 廠內機種和鴻海料號對應關係
    /// </summary>
    public class T_C_SKU_FOXCONN_MAPPING : DataObjectTable
    {
        public T_C_SKU_FOXCONN_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_FOXCONN_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_FOXCONN_MAPPING);
            TableName = "C_SKU_FOXCONN_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_SKU_FOXCONN_MAPPING GetMappingBySkuAndVer(string Skuno, string SkuVersion, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_FOXCONN_MAPPING>().Where(t => t.SKUNO == Skuno).WhereIF(SkuVersion!=null && SkuVersion.Length>0,t=>t.SKU_VER==SkuVersion).ToList().FirstOrDefault();
        }

        public List<C_SKU_FOXCONN_MAPPING> GetAllFoxconnSkuMappings(OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_FOXCONN_MAPPING>().OrderBy(t => t.SKUNO).OrderBy(t => t.SKU_VER).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public List<C_SKU_FOXCONN_MAPPING> GetFoxconnSkuMappingByCondition(string SearchCondition, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_FOXCONN_MAPPING>()
                .Where(t => t.SKUNO.Contains(SearchCondition) 
                || t.SKU_VER.Contains(SearchCondition) 
                || t.SKU_FOXCONN.Contains(SearchCondition)).ToList();
        }

        public int DeleteFoxconnSkuMappingByIds(string Ids, OleExec DB)
        {
            return DB.ORM.Deleteable<C_SKU_FOXCONN_MAPPING>().Where(t => Ids.Contains(t.ID)).ExecuteCommand();
        }

        public int AddFoxconnSkuMapping(string InnerSku, string InnerSkuVer, string FoxconnSku, string FoxconnSkuVer, string Bu, string EmpNo, OleExec DB)
        {
            C_SKU_FOXCONN_MAPPING mapping = new C_SKU_FOXCONN_MAPPING();
            mapping.ID = GetNewID(Bu, DB);
            mapping.SKUNO = InnerSku;
            mapping.SKU_VER = InnerSkuVer;
            mapping.SKU_FOXCONN = FoxconnSku;
            mapping.SKU_VER_FOXCONN = FoxconnSkuVer;
            mapping.EDIT_TIME = GetDBDateTime(DB);
            mapping.EDIT_EMP = EmpNo;
            return DB.ORM.Insertable<C_SKU_FOXCONN_MAPPING>(mapping).ExecuteCommand();
        }

        public int UpdateFoxconnSkuMapping(string InnerSku, string InnerSkuVer, string FoxconnSku, string FoxconnSkuVer, string EmpNo, OleExec DB)
        {
            return DB.ORM.Updateable<C_SKU_FOXCONN_MAPPING>()
                    .UpdateColumns(t => new C_SKU_FOXCONN_MAPPING() { SKU_FOXCONN = FoxconnSku, SKU_VER_FOXCONN = FoxconnSkuVer })
                    .Where(t => t.SKUNO == InnerSku && t.SKU_VER == InnerSkuVer).ExecuteCommand();
        }

        public int UpdateFoxconnSkuMappingById(string Id, string FoxconnSku, string FoxconnSkuVer, string EmpNo, OleExec DB)
        {
            return DB.ORM.Updateable<C_SKU_FOXCONN_MAPPING>()
                .UpdateColumns(t => new C_SKU_FOXCONN_MAPPING() { SKU_FOXCONN = FoxconnSku, SKU_VER_FOXCONN = FoxconnSkuVer, EDIT_EMP = EmpNo, EDIT_TIME = DateTime.Now })
                .Where(t => t.ID == Id).ExecuteCommand();
        }

    }
    public class Row_C_SKU_FOXCONN_MAPPING : DataObjectBase
    {
        public Row_C_SKU_FOXCONN_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_FOXCONN_MAPPING GetDataObject()
        {
            C_SKU_FOXCONN_MAPPING DataObject = new C_SKU_FOXCONN_MAPPING();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_VER = this.SKU_VER;
            DataObject.SKU_FOXCONN = this.SKU_FOXCONN;
            DataObject.SKU_VER_FOXCONN = this.SKU_VER_FOXCONN;
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
        public string SKU_VER
        {
            get
            {
                return (string)this["SKU_VER"];
            }
            set
            {
                this["SKU_VER"] = value;
            }
        }
        public string SKU_FOXCONN
        {
            get
            {
                return (string)this["SKU_FOXCONN"];
            }
            set
            {
                this["SKU_FOXCONN"] = value;
            }
        }
        public string SKU_VER_FOXCONN
        {
            get
            {
                return (string)this["SKU_VER_FOXCONN"];
            }
            set
            {
                this["SKU_VER_FOXCONN"] = value;
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
    public class C_SKU_FOXCONN_MAPPING
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string SKU_VER { get; set; }
        public string SKU_FOXCONN { get; set; }
        public string SKU_VER_FOXCONN { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
