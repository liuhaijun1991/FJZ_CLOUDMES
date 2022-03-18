using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_Rule : DataObjectTable
    {
        public T_C_KP_Rule(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_Rule(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_Rule);
            TableName = "C_KP_Rule".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_KP_Rule> GetDataBySkuWo(string WO,string Skuno,OleExec SFCDB )
        {
            string strSql = $@"select distinct * from c_kp_rule where partno in 
(select partno from C_sku_mpn where skuno='{Skuno}')
or partno in
(select repalcepartno from R_WO_KP_Repalce where wo='{WO}')";
            List<C_KP_Rule> ret = new List<C_KP_Rule>();
            DataSet res = SFCDB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_KP_Rule R = new Row_C_KP_Rule(this.DataInfo);
                R.loadData(res.Tables[0].Rows[i]);
                ret.Add(R.GetDataObject());
            }
            return ret;

        }

        public bool IsExists(OleExec DB, string Partno, string Mpn, string ScanType)
        {
            string sql = $@" select * from c_kp_rule where partno='{Partno}' and mpn='{Mpn}' and scantype='{ScanType}' ";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            if (dsCSkuMpn.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool IsExistsFTX(OleExec DB, string Partno, string Mpn, string ScanType, string REGEX)
        {
            string sql = $@" select * from c_kp_rule where partno='{Partno}' and mpn='{Mpn}' and scantype='{ScanType}' and REGEX = '{REGEX}' ";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            if (dsCSkuMpn.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public List<C_KP_Rule> GetCKpRule(OleExec DB, string Partno)
        {
            string sql = Partno.Equals("") ? $@"SELECT * FROM c_kp_rule " : $@"SELECT * FROM c_kp_rule where PARTNO='{Partno}'";
            DataSet DS = DB.ExecSelect(sql);
            List<C_KP_Rule> CL = new List<C_KP_Rule>();
            foreach (DataRow VARIABLE in DS.Tables[0].Rows)
            {
                Row_C_KP_Rule row = (Row_C_KP_Rule)this.NewRow();
                row.loadData(VARIABLE);
                CL.Add(row.GetDataObject());
            }
            return CL;
        }

        public C_KP_Rule GetKPRule(OleExec sfcdb, string partno, string mpn, string scanType)
        {
            C_KP_Rule kpRule = new C_KP_Rule();
            try
            {                
                string sql = $@" select * from c_kp_rule where partno='{partno}' and mpn='{mpn}' and scantype='{scanType}' ";
                DataSet ds = sfcdb.ExecSelect(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Row_C_KP_Rule row = (Row_C_KP_Rule)this.NewRow();
                    row.loadData(ds.Tables[0].Rows[0]);
                    kpRule = row.GetDataObject();
                }
                else
                {
                    kpRule = null;
                }
            }
            catch
            {
                kpRule = null;
            }
            return kpRule;
        }


        public List<C_KP_Rule> GetMPNfromPNScanType(OleExec DB, string Partno)
        {
            //patty 20190121 added
            return DB.ORM.Queryable<C_KP_Rule>().Where(t => t.PARTNO == Partno && t.SCANTYPE == "MPN").ToList();
        }
    }
    public class Row_C_KP_Rule : DataObjectBase
    {
        public Row_C_KP_Rule(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_Rule GetDataObject()
        {
            C_KP_Rule DataObject = new C_KP_Rule();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.REGEX = this.REGEX;
            DataObject.SCANTYPE = this.SCANTYPE;
            DataObject.MPN = this.MPN;
            DataObject.PARTNO = this.PARTNO;
            DataObject.ID = this.ID;
            return DataObject;
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
    public class C_KP_Rule
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string REGEX{get;set;}
        public string SCANTYPE{get;set;}
        public string MPN{get;set;}
        public string PARTNO{get;set;}
    }
}