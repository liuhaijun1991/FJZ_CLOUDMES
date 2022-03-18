using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_Replace : DataObjectTable
    {
        public T_C_KP_Replace(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_Replace(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_Replace);
            TableName = "C_KP_Replace".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_KP_Replace> GetReplaceKpBySku(OleExec DB, string Sku)
        {
            string sql = Sku.Equals("") ? $@"SELECT * FROM C_KP_REPLACE  " : $@"SELECT * FROM C_KP_REPLACE WHERE SKUNO='{Sku}'";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            List<C_KP_Replace> cKpReplaceList = new List<C_KP_Replace>();
            foreach (DataRow VARIABLE in dsCSkuMpn.Tables[0].Rows)
            {
                Row_C_KP_Replace rowCKpReplace = (Row_C_KP_Replace)this.NewRow();
                rowCKpReplace.loadData(VARIABLE);
                cKpReplaceList.Add(rowCKpReplace.GetDataObject());
            }
            return cKpReplaceList;
        }

        public bool IsExists(OleExec DB, string Sku, string PartNo, string ReplacePartno)
        {
            string sql = $@"SELECT * FROM C_KP_REPLACE WHERE SKUNO='{Sku}' AND PARTNO='{PartNo}' AND REPLACEPARTNO='{ReplacePartno}' ";
            DataSet ds = DB.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
    }
    public class Row_C_KP_Replace : DataObjectBase
    {
        public Row_C_KP_Replace(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_Replace GetDataObject()
        {
            C_KP_Replace DataObject = new C_KP_Replace();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.REPLACEPARTNO = this.REPLACEPARTNO;
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
        public string REPLACEPARTNO
        {
            get
            {
                return (string)this["REPLACEPARTNO"];
            }
            set
            {
                this["REPLACEPARTNO"] = value;
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
    public class C_KP_Replace
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public string PARTNO{get;set;}
        public string REPLACEPARTNO{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}