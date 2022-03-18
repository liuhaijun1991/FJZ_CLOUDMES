using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_WAREHOUSE_CONFIG_T : DataObjectTable
    {
        public T_C_WAREHOUSE_CONFIG_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_WAREHOUSE_CONFIG_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_WAREHOUSE_CONFIG_T);
            TableName = "C_WAREHOUSE_CONFIG_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_WAREHOUSE_CONFIG_T ConstructWarehouse(DataRow dr)
        {
            C_WAREHOUSE_CONFIG_T warehouse_config_t = new C_WAREHOUSE_CONFIG_T();
            Row_C_WAREHOUSE_CONFIG_T row = (Row_C_WAREHOUSE_CONFIG_T)NewRow();
            row.loadData(dr);
            warehouse_config_t = row.GetDataObject();
            return warehouse_config_t;
        }
        public DataTable GetAllWarehouseConfig(OleExec DB)
        {
            List<C_WAREHOUSE_CONFIG_T> WarehouseConfigList = new List<C_WAREHOUSE_CONFIG_T>();
            string sql = "";
            DataTable dt = null;
            try
            {

                sql = $@"SELECT B.NAME,A.* FROM  SFCBASE.C_WAREHOUSE_CONFIG_T A,C_BU_EX B where A.FACTORY_ID=B.ID and COL_SIZE=1  ORDER BY A.FACTORY_ID";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        WarehouseConfigList.Add(ConstructWarehouse(dr));
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return dt;
        }
        public string Insert( string WH_NAME,String EMP_NO, String IDFAC, string id,OleExec DB)
        {
            string sql;
            DataTable table;
            try
            {
                sql = $@"SELECT * FROM SFCBASE.C_WAREHOUSE_CONFIG_T WHERE UPPER(WH_NAME)='{WH_NAME}' and COL_SIZE=1";
                table = DB.ExecSelect(sql).Tables[0];
                if (table.Rows.Count > 0)
                {
                    return "FAIL";
                }
                
                sql = $@"Insert into SFCBASE.C_WAREHOUSE_CONFIG_T
                       (FACTORY_ID, WH_ID, WH_NAME, COL_SIZE, 
                        EMP, DATE_CREATE, DATE_RESET)
                     Values
                       ('{IDFAC}', '{id}', '{WH_NAME}', 1, 
                        '{EMP_NO}', sysdate,null)";
                int a= DB.ExecSqlNoReturn(sql,null);
                return "Pass";
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }
        public string Edit(string WH_NAME, string EMP_NO , OleExec DB)
        {
            string sql;
            try
            {
                sql = $@"update sfcbase.C_WAREHOUSE_CONFIG_T set  ROW_SIZE=0, COL_SIZE =0 ,DATE_RESET=sysdate where WH_NAME='{WH_NAME}'";
                int a = DB.ExecSqlNoReturn(sql, null);
                return "Pass";
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public string EditConfigWh(string WH_NAME,  string EMP_NO, OleExec DB)
        {
            string sql,SQLCHECK;
            DataTable table = new DataTable();
            try
            {
                sql = $@"SELECT
	                    SN.*
                    FROM
	                    SFCBASE.C_WAREHOUSE_PALLET_POSITION_T CPP,
	                    sfcbase.C_WAREHOUSE_CONFIG_T CC,
	                    R_SN SN,
	                    R_SN_PACKING SNP,
	                    R_PACKING CT,
	                    R_PACKING PL
                    WHERE
	                    cc.WH_NAME='{WH_NAME}'
	                    AND CPP.OUT_FLAG = 0
	                    AND CPP.WH_ID = CC.WH_ID
	                    AND CPP.PALLET_NO = PL.PACK_NO
	                    AND PL.ID = CT.PARENT_PACK_ID
	                    AND CT.ID = SNP.PACK_ID
	                    AND SNP.SN_ID = SN.ID 
                        AND SN.SHIPPED_FLAG <> 1";
                table = DB.ExecSelect(sql).Tables[0];
                if (table.Rows.Count > 0)
                {
                    return "FAIL";
                }
                sql = $@"update sfcbase.C_WAREHOUSE_CONFIG_T set COL_SIZE=0 where WH_NAME='{WH_NAME}'    ";
                int a = DB.ExecSqlNoReturn(sql, null);
                return "Pass";
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
   
    public class C_WAREHOUSE_CONFIG_T
    {
        public string FACTORY_ID { get; set; }
        public string WH_ID { get; set; }
        
        public string WH_NAME { get; set; }
        public double? ROW_SIZE { get; set; }
        public double? COL_SIZE { get; set; }
        public string EMP { get; set; }
        public DateTime? DATE_CREATE { get; set; }
        public DateTime? DATE_RESET { get; set; }
    }

    public class Row_C_WAREHOUSE_CONFIG_T : DataObjectBase
    {
        public Row_C_WAREHOUSE_CONFIG_T(DataObjectInfo info) : base(info)
        {

        }
        public C_WAREHOUSE_CONFIG_T GetDataObject()
        {
            C_WAREHOUSE_CONFIG_T DataObject = new C_WAREHOUSE_CONFIG_T();
            DataObject.FACTORY_ID = this.FACTORY_ID;
            DataObject.WH_ID = this.WH_ID;
            DataObject.WH_NAME = this.WH_NAME;
            DataObject.ROW_SIZE = this.ROW_SIZE;
            DataObject.COL_SIZE = this.COL_SIZE;
            DataObject.EMP = this.EMP;
            DataObject.DATE_CREATE = this.DATE_CREATE;
            DataObject.DATE_RESET = this.DATE_RESET;
            return DataObject;
        }
        public string FACTORY_ID
        {
            get
            {
                return (string)this["FACTORY_ID"];
            }
            set
            {
                this["FACTORY_ID"] = value;
            }
        }
        public string WH_ID
        {
            get
            {
                return (string)this["WH_ID"];
            }
            set
            {
                this["WH_ID"] = value;
            }
        }
        public string WH_NAME
        {
            get
            {
                return (string)this["WH_NAME"];
            }
            set
            {
                this["WH_NAME"] = value;
            }
        }
        public double? ROW_SIZE
        {
            get
            {
                return (double?)this["ROW_SIZE"];
            }
            set
            {
                this["ROW_SIZE"] = value;
            }
        }
        public double? COL_SIZE
        {
            get
            {
                return (double?)this["COL_SIZE"];
            }
            set
            {
                this["COL_SIZE"] = value;
            }
        }
        public string EMP
        {
            get
            {
                return (string)this["EMP"];
            }
            set
            {
                this["EMP"] = value;
            }
        }
        public DateTime? DATE_CREATE
        {
            get
            {
                return (DateTime?)this["DATE_CREATE"];
            }
            set
            {
                this["DATE_CREATE"] = value;
            }
        }

        public DateTime? DATE_RESET
        {
            get
            {
                return (DateTime?)this["DATE_RESET"];
            }
            set
            {
                this["DATE_RESET"] = value;
            }
        }

    }
}
