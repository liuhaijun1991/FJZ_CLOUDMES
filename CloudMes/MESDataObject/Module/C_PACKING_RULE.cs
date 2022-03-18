using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_PACKING_RULE : DataObjectTable
    {
        public T_C_PACKING_RULE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PACKING_RULE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PACKING_RULE);
            TableName = "C_PACKING_RULE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_PACKING_RULE> GetPackingBySku(string SkuNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_PACKING_RULE> Packing = new List<C_PACKING_RULE>();

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM SFCBASE.C_PACKING_RULE WHERE 1=1 tempsql";
                if (SkuNo.Length > 0)
                {
                    sql = sql.Replace("tempsql", "and SKU='" + SkuNo + "'");
                }
                else
                {
                    sql = sql.Replace("tempsql", "");
                }
                dt = DB.ExecSelect(sql).Tables[0];
                Row_C_PACKING_RULE row = (Row_C_PACKING_RULE)NewRow();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row.loadData(dt.Rows[i]);
                    Packing.Add(row.GetDataObject());
                }
            }
            return Packing;
        }

        public Row_C_PACKING_RULE GetPackingInfoBySku(string SkuNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            C_PACKING_RULE Packing = new C_PACKING_RULE();


            sql = $@"SELECT * FROM SFCBASE.C_PACKING_RULE WHERE SKU='{SkuNo}'";
            dt = DB.ExecSelect(sql).Tables[0];
            Row_C_PACKING_RULE row = (Row_C_PACKING_RULE)NewRow();
            row.loadData(dt.Rows[0]);
            //Packing.ID = row.ID;
            //Packing.SKU = row.SKU;
            //Packing.REGEX = row.REGEX;
            //Packing.EDIT_EMP = row.EDIT_EMP;
            //Packing.EDIT_TIME = row.EDIT_TIME;

            return row;
        }

        public C_PACKING_RULE GetPackingRuleBySkuNo(string SkuNo, string scanType, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            C_PACKING_RULE Packing = null;
            sql = $@"SELECT * FROM SFCBASE.C_PACKING_RULE WHERE SKU='{SkuNo}' AND SCANTYPE='{scanType}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Packing = new C_PACKING_RULE();
                Row_C_PACKING_RULE row = (Row_C_PACKING_RULE)NewRow();
                row.loadData(dt.Rows[0]);
                Packing.ID = row.ID;
                Packing.SKU = row.SKU;
                Packing.REGEX = row.REGEX;
                Packing.EDIT_EMP = row.EDIT_EMP;
                Packing.EDIT_TIME = row.EDIT_TIME;
                Packing.SCANTYPE = row.SCANTYPE;
            }
            return Packing;
        }

        public List<C_SKU> GetAllPackingSku(OleExec DB, string SkuNo)
        {
            return DB.ORM.Queryable<C_SKU>().WhereIF(!SkuNo.Equals(""), t => t.SKUNO == SkuNo).GroupBy(x => x.SKUNO).ToList();
        }
        public bool IsExistsFTX(OleExec DB, string SkuNo, string ScanType, string REGEX)
        {
            string sql = $@" select * from SFCBASE.C_PACKING_RULE where SKU='{SkuNo}'  and scantype='{ScanType}' ";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            if (dsCSkuMpn.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public string GenarateID(OleExec DB)
        {
            //string strSql = "exec sfc.get_id('KKI','C_sku',OUT_RES); ";
            //string ret = "";
            OleDbParameter[] para = new OleDbParameter[]
            {
                new OleDbParameter(":IN_BU",OleDbType.VarChar,300),
                new OleDbParameter(":IN_TYPE",OleDbType.VarChar,300),
                new OleDbParameter(":OUT_RES",OleDbType.VarChar,500)
            };
            para[0].Value = "VNDCN";
            para[1].Value = "C_PACKING_RULE";
            para[2].Direction = ParameterDirection.Output;
            DB.ExecProcedureNoReturn("SFC.GET_ID", para);
            string newID = para[2].Value.ToString();
            if (newID.StartsWith("ERR"))
            {
                throw new Exception("ERROR'" + TableName + "' ID 異常! " + newID);
            }
            return newID;
        }

        public bool InsertData(OleExec DB, string id, string SkuNo, string ScanType, string REGEX, string userNo)
        {
            string sql = $@" INSERT
	                            INTO
		                            SFCBASE.C_PACKING_RULE(ID,
		                            SKU,
		                            SCANTYPE,
		                            REGEX,
                                    EDIT_TIME,
		                            EDIT_EMP)
	                            VALUES('{SkuNo}','{SkuNo}','{ScanType}','{REGEX}',sysdate,'{userNo}')";
            string flag = "";
            flag = DB.ExecSQL(sql);
            if ("1".Equals(flag)) return true;
            else return false;
        }

        public bool UpdateDate(OleExec DB, string SkuNo, string REGEX, DateTime editTime, string userNo)
        {
            string sql = $@" UPDATE SFCBASE.C_PACKING_RULE SET REGEX='{REGEX}',EDIT_TIME=sysdate,EDIT_EMP='{userNo}' WHERE SKU='{SkuNo}'";
            string flag = "";
            flag = DB.ExecSQL(sql);
            if ("1".Equals(flag)) return true;
            else return false;
        }

        public bool DeleteSku(OleExec DB, string SkuNo)
        {
            string sql = $@" DELETE FROM  SFCBASE.C_PACKING_RULE WHERE SKU='{SkuNo}'";
            string flag = "";
            flag = DB.ExecSQL(sql);
            if ("1".Equals(flag)) return true;
            else return false;
        }

        public bool IsExists(OleExec DB, string SkuNo, string ScanType)
        {
            string sql = $@" select * from SFCBASE.C_PACKING_RULE where SKU='{SkuNo}' and scantype='{ScanType}' ";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            if (dsCSkuMpn.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
    }
    public class Row_C_PACKING_RULE : DataObjectBase
    {
        public Row_C_PACKING_RULE(DataObjectInfo info) : base(info)
        {

        }
        public C_PACKING_RULE GetDataObject()
        {
            C_PACKING_RULE DataObject = new C_PACKING_RULE();
            DataObject.ID = this.ID;
            DataObject.SKU = this.SKU;
            DataObject.SCANTYPE = this.SCANTYPE;
            DataObject.REGEX = this.REGEX;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string SKU
        {
            get
            {
                return (string)this["SKU"];
            }
            set
            {
                this["SKU"] = value;
            }
        }
        public string SCANTYPE
        {
            get
            {
                return (string)this["SCANTYPE"];
            }
            set
            {
                this["SCANTYPE"] = value;
            }
        }
        public string REGEX
        {
            get
            {
                return (string)this["REGEX"];
            }
            set
            {
                this["REGEX"] = value;
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
    }
    public class C_PACKING_RULE
    {
        public string ID { get; set; }
        public string SKU { get; set; }
        public string SCANTYPE { get; set; }
        public string REGEX { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}