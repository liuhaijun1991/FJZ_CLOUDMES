using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_BU_LOCATION : DataObjectTable
    {
        public T_C_BU_LOCATION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
        }
        public T_C_BU_LOCATION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_BU_LOCATION);
            TableName = "C_BU_EX".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_BU_LOCATION ConstructWarehouse(DataRow dr)
        {
            C_BU_LOCATION warehouse_config_t = new C_BU_LOCATION();
            Row_C_BU_LOCATION row = (Row_C_BU_LOCATION)NewRow();
            row.loadData(dr);
            warehouse_config_t = row.GetDataObject();
            return warehouse_config_t;
        }
        public List<C_BU_LOCATION> GetAllWarehouse(OleExec DB)
        {
            List<C_BU_LOCATION> WarehouseList = new List<C_BU_LOCATION>();
            string sql = "";
            DataTable dt = null;
            try
            {

                sql = $@"SELECT * FROM  C_BU_EX ";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        WarehouseList.Add(ConstructWarehouse(dr));
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return WarehouseList;
        }

    }
    public class C_BU_LOCATION
    {
        public string ID { get; set; }
        public double? SEQ_NO { get; set; }
   
        public string NAME { get; set; }
        public string VALUE { get; set; }

    }

    public class Row_C_BU_LOCATION : DataObjectBase
    {
        public Row_C_BU_LOCATION(DataObjectInfo info) : base(info)
        {

        }
        public C_BU_LOCATION GetDataObject()
        {
            C_BU_LOCATION DataObject = new C_BU_LOCATION();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
           

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

        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        } 


        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }


        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }

    }
}
