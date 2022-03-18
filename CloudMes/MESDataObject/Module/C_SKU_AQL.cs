using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_AQL : DataObjectTable
    {
        public T_C_SKU_AQL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_AQL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_AQL);
            TableName = "C_SKU_AQL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_SKU_AQL GetSkuAql(OleExec DB,string Skuno)
        {
            C_SKU_AQL res = new C_SKU_AQL();
            string strSql = $@" select * from C_SKU_AQL where skuno='{Skuno}' ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                Row_C_SKU_AQL r = (Row_C_SKU_AQL)this.NewRow();
                r.loadData(item);
                res=r.GetDataObject();
            }
            return res;
        }

        public List<C_SKU_AQL> GetSkuAqlBySkuId(OleExec DB, string SkuId)
        {
            List<C_SKU_AQL> res = new List<C_SKU_AQL>();
            string strSql = $@" select A.* from C_SKU_AQL A,C_SKU B where B.SKUNO=A.SKUNO AND B.ID='{SkuId}' ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                Row_C_SKU_AQL r = (Row_C_SKU_AQL)this.NewRow();
                r.loadData(item);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        public bool DeleteBySkuno(OleExec DB, string Skuno)
        {
            string strSql = $@"  delete from C_SKU_AQL WHERE SKUNO='{Skuno}' ";
            DB.ThrowSqlExeception = true;
            try
            {
                DB.ExecSQL(strSql);
            }
            catch(Exception e) { throw e; }
            finally { DB.ThrowSqlExeception = false; }
            return true;
        }

        public List<C_SKU_AQL> GetAQLListBySkuAndType(OleExec DB,string skuno,string type)
        {
            return DB.ORM.Queryable<C_SKU_AQL>().Where(r => r.SKUNO == skuno && r.AQLTYPE == type).ToList();
        }
    }
    public class Row_C_SKU_AQL : DataObjectBase
    {
        public Row_C_SKU_AQL(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_AQL GetDataObject()
        {
            C_SKU_AQL DataObject = new C_SKU_AQL();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.DEFAULLEVEL = this.DEFAULLEVEL;
            DataObject.SKUNO = this.SKUNO;
            DataObject.AQLTYPE = this.AQLTYPE;
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
        public string DEFAULLEVEL
        {
            get
            {
                return (string)this["DEFAULLEVEL"];
            }
            set
            {
                this["DEFAULLEVEL"] = value;
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
        public string AQLTYPE
        {
            get
            {
                return (string)this["AQLTYPE"];
            }
            set
            {
                this["AQLTYPE"] = value;
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
    public class C_SKU_AQL
    {
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string DEFAULLEVEL{get;set;}
        public string SKUNO{get;set;}
        public string AQLTYPE{get;set;}
        public string ID{get;set;}
    }
}