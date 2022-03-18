using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_C_SKU_LINK_7B5 : DataObjectTable
    {
        public T_C_SKU_LINK_7B5(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_LINK_7B5(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_LINK_7B5);
            TableName = "C_SKU_LINK_7B5".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_SKU_LINK_7B5 GetLinkObject(OleExec sfcdb, string skuno, string subskuno)
        {
            return sfcdb.ORM.Queryable<C_SKU_LINK_7B5>().Where(r => r.SKUNO == skuno && r.SUBSKUNO == subskuno).ToList().FirstOrDefault();
        }

        public List<C_SKU_LINK_7B5> GetLinkList(OleExec sfcdb, string skuno)
        {
            return sfcdb.ORM.Queryable<C_SKU_LINK_7B5>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(skuno), r => r.SKUNO == skuno).OrderBy(r=>r.LASTEDIT,SqlSugar.OrderByType.Desc).ToList();
        }

        public List<C_SKU_LINK_7B5> GetPCBALinkList(OleExec sfcdb, string skuno)
        {
            return sfcdb.ORM.Queryable<C_SKU_LINK_7B5, C_SKU>((csl, cs) => csl.SKUNO == cs.SKUNO).Where((csl, cs) => csl.SKUNO == skuno && cs.SKU_TYPE == "PCBA")
                .Select((csl, cs) => csl).ToList();
        }

        public DataTable GetSubSku(OleExec sfcdb, string skuno)
        {
            string sql = $@" SELECT DISTINCT skuno, subskuno  FROM c_sku_link_7b5 WHERE skuno = '{skuno}'";
            return sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
        }

        public List<string> GetSubSkuListBySeq(OleExec sfcdb, string skuno)
        {
            return sfcdb.ORM.Queryable<C_SKU_LINK_7B5>().Where(r => r.SKUNO == skuno && r.SEQNO == 8)
                .Select(r => r.SUBSKUNO).ToList().Distinct().ToList();
        }

        public bool IsExistBySkuAndSeq(OleExec sfcdb, string skuno, double seq)
        {
            return sfcdb.ORM.Queryable<C_SKU_LINK_7B5>().Any(r => r.SKUNO == skuno && r.SEQNO == seq);
        }

        public int SaveObject(OleExec sfcdb, C_SKU_LINK_7B5 objectLink)
        {
            return sfcdb.ORM.Insertable<C_SKU_LINK_7B5>(objectLink).ExecuteCommand();
        }
        public int UpdateObject(OleExec sfcdb, C_SKU_LINK_7B5 objectLink)
        {
            return sfcdb.ORM.Updateable<C_SKU_LINK_7B5>().UpdateColumns(r => new C_SKU_LINK_7B5
            {
                SKUNO = objectLink.SKUNO,
                SUBSKUNO = objectLink.SUBSKUNO,
                SEQNO = objectLink.SEQNO,
                LASTEDIT = objectLink.LASTEDIT,
                LASTEDITBY = objectLink.LASTEDITBY
            }).Where(r => r.ID == objectLink.ID).ExecuteCommand();
        }
    }
    public class Row_C_SKU_LINK_7B5 : DataObjectBase
    {
        public Row_C_SKU_LINK_7B5(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_LINK_7B5 GetDataObject()
        {
            C_SKU_LINK_7B5 DataObject = new C_SKU_LINK_7B5();
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDIT = this.LASTEDIT;
            DataObject.SEQNO = this.SEQNO;
            DataObject.SUBSKUNO = this.SUBSKUNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.ID = this.ID;
            return DataObject;
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
        public DateTime? LASTEDIT
        {
            get
            {
                return (DateTime?)this["LASTEDIT"];
            }
            set
            {
                this["LASTEDIT"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string SUBSKUNO
        {
            get
            {
                return (string)this["SUBSKUNO"];
            }
            set
            {
                this["SUBSKUNO"] = value;
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
    public class C_SKU_LINK_7B5
    {
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDIT { get; set; }
        public double? SEQNO { get; set; }
        public string SUBSKUNO { get; set; }
        public string SKUNO { get; set; }
        public string ID { get; set; }
    }
}