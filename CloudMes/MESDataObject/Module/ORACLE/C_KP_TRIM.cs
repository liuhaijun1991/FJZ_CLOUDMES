using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_TRIM : DataObjectTable
    {
        public T_C_KP_TRIM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_TRIM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_TRIM);
            TableName = "C_KP_TRIM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_KP_TRIM> GetKPTrimBySkuWo(string WO, string Skuno, OleExec SFCDB)
        {
            string strSql = $@"select distinct * from C_KP_TRIM where partno in 
(select partno from C_sku_mpn where skuno='{Skuno}')";
            List<C_KP_TRIM> ret = new List<C_KP_TRIM>();
            DataSet res = SFCDB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_KP_TRIM R = new Row_C_KP_TRIM(this.DataInfo);
                R.loadData(res.Tables[0].Rows[i]);
                ret.Add(R.GetDataObject());
            }
            return ret;

        }
    }
    public class Row_C_KP_TRIM : DataObjectBase
    {
        public Row_C_KP_TRIM(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_TRIM GetDataObject()
        {
            C_KP_TRIM DataObject = new C_KP_TRIM();
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.MPN = this.MPN;
            DataObject.SCANTYPE = this.SCANTYPE;
            DataObject.PREFIX = this.PREFIX;
            DataObject.PREFIXLEN = this.PREFIXLEN;
            DataObject.LSTART = this.LSTART;
            DataObject.LLEN = this.LLEN;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
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
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
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
        public string PREFIX
        {
            get
            {
                return (string)this["PREFIX"];
            }
            set
            {
                this["PREFIX"] = value;
            }
        }
        public string PREFIXLEN
        {
            get
            {
                return (string)this["PREFIXLEN"];
            }
            set
            {
                this["PREFIXLEN"] = value;
            }
        }
        public string LSTART
        {
            get
            {
                return (string)this["LSTART"];
            }
            set
            {
                this["LSTART"] = value;
            }
        }
        public string LLEN
        {
            get
            {
                return (string)this["LLEN"];
            }
            set
            {
                this["LLEN"] = value;
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
    public class C_KP_TRIM
    {
        public string SKUNO;
        public string PARTNO;
        public string MPN;
        public string SCANTYPE;
        public string PREFIX;
        public string PREFIXLEN;
        public string LSTART;
        public string LLEN;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}