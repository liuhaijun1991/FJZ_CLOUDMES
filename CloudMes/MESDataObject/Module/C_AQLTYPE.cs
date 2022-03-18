using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_AQLTYPE : DataObjectTable
    {
        public T_C_AQLTYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_AQLTYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_AQLTYPE);
            TableName = "C_AQLTYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_AQLTYPE> GetAllAql(OleExec DB)
        {
            return DB.ORM.Queryable<C_AQLTYPE>().ToList();
        }

        public List<C_AQLTYPE> GetAqlBySkuno(string aqltype, OleExec DB)
        {
            return DB.ORM.Queryable<C_AQLTYPE>()
                .Where(t => t.AQL_TYPE == aqltype)
                .OrderBy(t => t.AQL_TYPE)
                .OrderBy(t => t.GL_LEVEL)
                .OrderBy(t => t.LOT_QTY, SqlSugar.OrderByType.Asc).ToList();

        }

        public List<string> GetAqlLevelByType(string aqltype, OleExec DB)
        {
            return DB.ORM.Queryable<C_AQLTYPE>().Where(t => t.AQL_TYPE == aqltype).Select(t => t.GL_LEVEL).ToList();
        }

        public List<C_AQLTYPE> GetAqlTypeBySkunoAndLevel(string skuno,string AqlLevel, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_AQL, C_AQLTYPE>((sa, a) => sa.AQLTYPE == a.AQL_TYPE)
                .Where((sa, a) => sa.SKUNO == skuno && a.GL_LEVEL == AqlLevel)
                .Select((sa, a) => a)
                .ToList();
        }

        public List<C_AQLTYPE> GetAqlTypeBySkuno(string skuno, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_AQL, C_AQLTYPE>((sa, a) => sa.AQLTYPE == a.AQL_TYPE)
                .Where((sa, a) => sa.SKUNO == skuno)
                .Select((sa, a) => a)
                .ToList();
        }

        public List<C_AQLTYPE> GetAqlTypeBySkunoId(string skuId, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU, C_AQLTYPE, C_SKU_AQL>((s, a, sa) => s.SKUNO == sa.SKUNO && a.AQL_TYPE == sa.AQLTYPE)
                .Where((s, a, sa) => s.ID == skuId).Select((s, a, sa) => a).ToList();
        }

        public List<string> GetAql( OleExec DB)
        {
            return DB.ORM.Queryable<C_AQLTYPE>().GroupBy(t => t.AQL_TYPE).Select(t => t.AQL_TYPE).ToList();
        }

        public C_AQLTYPE GetByAqltype(string _Aqltype, OleExec DB)
        {
            List<C_AQLTYPE> As = DB.ORM.Queryable<C_AQLTYPE>().Where(t => t.AQL_TYPE == _Aqltype.Replace("'", "''")).ToList();
            if (As.Count > 0)
            {
                return As.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 獲取SAMPLEQTY
        /// </summary>
        /// <param name="AQLType"></param>
        /// <param name="LotQty"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetSampleQty(string AQLType, int LotQty, OleExec DB)
        {
            //string StrSql = "";
            //int SampleQty = 0;
            //if (DBType == DB_TYPE_ENUM.Oracle)
            //{
            //    StrSql = $@"select case when {LotQty} < sample_qty then {LotQty} else sample_qty end as SAMPLEQTY from 
            //         (select * from C_AQLTYPE where LOT_QTY >= {LotQty} order by LOT_QTY) where rownum = 1";
            //    SampleQty = Convert.ToInt16(DB.ExecSelectOneValue(StrSql)?.ToString());

            //    return SampleQty;
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}
            //List<double?> SampleQtys = DB.ORM.Queryable<C_AQLTYPE>().Where(t => t.AQL_TYPE == AQLType && t.LOT_QTY >= LotQty).OrderBy(t => t.LOT_QTY).Select(t => SqlSugar.SqlFunc.IIF(LotQty < t.SAMPLE_QTY, LotQty, t.SAMPLE_QTY)).ToList();
            //modify fgg 2018.08.30 改了語句也不測試，int 類型直接與 double比較報錯
            List<double?> SampleQtys=DB.ORM.Queryable<C_AQLTYPE>().Where(t => t.AQL_TYPE == AQLType && t.LOT_QTY >= LotQty).OrderBy(t => t.LOT_QTY).Select(t => SqlSugar.SqlFunc.IIF(Convert.ToDouble(LotQty) < t.SAMPLE_QTY, LotQty, t.SAMPLE_QTY)).ToList();
            if (SampleQtys.Count > 0)
            {
                return Convert.ToInt16(SampleQtys.First());
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_C_AQLTYPE : DataObjectBase
    {
        public Row_C_AQLTYPE(DataObjectInfo info) : base(info)
        {

        }
        public C_AQLTYPE GetDataObject()
        {
            C_AQLTYPE DataObject = new C_AQLTYPE();
            DataObject.ID = this.ID;
            DataObject.AQL_TYPE = this.AQL_TYPE;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.GL_LEVEL = this.GL_LEVEL;
            DataObject.SAMPLE_QTY = this.SAMPLE_QTY;
            DataObject.ACCEPT_QTY = this.ACCEPT_QTY;
            DataObject.REJECT_QTY = this.REJECT_QTY;
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
        public string AQL_TYPE
        {
            get

            {
                return (string)this["AQL_TYPE"];
            }
            set
            {
                this["AQL_TYPE"] = value;
            }
        }
        public double? LOT_QTY
        {
            get

            {
                return (double?)this["LOT_QTY"];
            }
            set
            {
                this["LOT_QTY"] = value;
            }
        }
        public string GL_LEVEL
        {
            get

            {
                return (string)this["GL_LEVEL"];
            }
            set
            {
                this["GL_LEVEL"] = value;
            }
        }
        public double? SAMPLE_QTY
        {
            get

            {
                return (double?)this["SAMPLE_QTY"];
            }
            set
            {
                this["SAMPLE_QTY"] = value;
            }
        }
        public double? ACCEPT_QTY
        {
            get

            {
                return (double?)this["ACCEPT_QTY"];
            }
            set
            {
                this["ACCEPT_QTY"] = value;
            }
        }
        public double? REJECT_QTY
        {
            get

            {
                return (double?)this["REJECT_QTY"];
            }
            set
            {
                this["REJECT_QTY"] = value;
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
    public class C_AQLTYPE
    {
        public string ID{get;set;}
        public string AQL_TYPE{get;set;}
        public double? LOT_QTY{get;set;}
        public string GL_LEVEL{get;set;}
        public double? SAMPLE_QTY{get;set;}
        public double? ACCEPT_QTY{get;set;}
        public double? REJECT_QTY{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}