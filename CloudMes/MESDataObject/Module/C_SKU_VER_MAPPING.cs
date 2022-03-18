using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_VER_MAPPING : DataObjectTable
    {
        public T_C_SKU_VER_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_VER_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_VER_MAPPING);
            TableName = "C_SKU_VER_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SKU_VER_MAPPING> GetMappingListBySku(string sku, OleExec sfcdb)
        {
            string sql = "";
            if (string.IsNullOrEmpty(sku))
            {
                sql = $@" select * from C_SKU_VER_MAPPING  ";
            }
            else
            {
                sql = $@" select * from C_SKU_VER_MAPPING where fox_skuno='{sku}' ";
            }
            DataTable dataTable = sfcdb.ExecSelect(sql).Tables[0];
            List<C_SKU_VER_MAPPING> skuMappingList = new List<C_SKU_VER_MAPPING>();
            Row_C_SKU_VER_MAPPING rowMapping = null;
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    rowMapping = (Row_C_SKU_VER_MAPPING)this.NewRow();
                    rowMapping.loadData(row);
                    skuMappingList.Add(rowMapping.GetDataObject());
                }
            }
            return skuMappingList;
        }

        public C_SKU_VER_MAPPING GetMappingBySkuAndVersion(string sku, string version,OleExec sfcdb)
        {            
            string sql = $@" select * from C_SKU_VER_MAPPING where fox_skuno='{sku}' and fox_version1='{version}' ";
            DataTable dataTable = sfcdb.ExecSelect(sql).Tables[0];
            C_SKU_VER_MAPPING skuMapping = null;
            Row_C_SKU_VER_MAPPING rowMapping = null;
            if (dataTable.Rows.Count > 0)
            {
                rowMapping = (Row_C_SKU_VER_MAPPING)this.NewRow();
                rowMapping.loadData(dataTable.Rows[0]);
                skuMapping = rowMapping.GetDataObject();
            }

            return skuMapping;
        }

        public bool MappingIsExistBySku(string sku,string version, OleExec sfcdb)
        {
            string sql = $@" select * from C_SKU_VER_MAPPING where fox_skuno='{sku}' and fox_version1='{version}'  ";
            DataTable dataTable = sfcdb.ExecSelect(sql).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MappingIsExistBySkuEditMode(string sku, string version,string id, OleExec sfcdb)
        {
            string sql = $@" select * from C_SKU_VER_MAPPING where fox_skuno='{sku}' and fox_version1='{version}' and ID!='{id}'  ";
            DataTable dataTable = sfcdb.ExecSelect(sql).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Row_C_SKU_VER_MAPPING : DataObjectBase
    {
        public Row_C_SKU_VER_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_VER_MAPPING GetDataObject()
        {
            C_SKU_VER_MAPPING DataObject = new C_SKU_VER_MAPPING();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.CUSTOMER_VERSION = this.CUSTOMER_VERSION;
            DataObject.FOX_VERSION2 = this.FOX_VERSION2;
            DataObject.FOX_VERSION1 = this.FOX_VERSION1;
            DataObject.CUSTOMER_SKUNO = this.CUSTOMER_SKUNO;
            DataObject.FOX_SKUNO = this.FOX_SKUNO;
            DataObject.ID = this.ID;
            return DataObject;
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
        public string CUSTOMER_VERSION
        {
            get
            {
                return (string)this["CUSTOMER_VERSION"];
            }
            set
            {
                this["CUSTOMER_VERSION"] = value;
            }
        }
        public string FOX_VERSION2
        {
            get
            {
                return (string)this["FOX_VERSION2"];
            }
            set
            {
                this["FOX_VERSION2"] = value;
            }
        }
        public string FOX_VERSION1
        {
            get
            {
                return (string)this["FOX_VERSION1"];
            }
            set
            {
                this["FOX_VERSION1"] = value;
            }
        }
        public string CUSTOMER_SKUNO
        {
            get
            {
                return (string)this["CUSTOMER_SKUNO"];
            }
            set
            {
                this["CUSTOMER_SKUNO"] = value;
            }
        }
        public string FOX_SKUNO
        {
            get
            {
                return (string)this["FOX_SKUNO"];
            }
            set
            {
                this["FOX_SKUNO"] = value;
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
    public class C_SKU_VER_MAPPING
    {
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string CUSTOMER_VERSION { get; set; }
        public string FOX_VERSION2 { get; set; }
        public string FOX_VERSION1 { get; set; }
        public string CUSTOMER_SKUNO { get; set; }
        public string FOX_SKUNO { get; set; }
        public string ID { get; set; }
    }
}