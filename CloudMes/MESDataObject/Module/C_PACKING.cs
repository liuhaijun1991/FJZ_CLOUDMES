using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PACKING : DataObjectTable
    {
        public T_C_PACKING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PACKING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PACKING);
            TableName = "C_PACKING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List< C_PACKING> GetPackingBySku(string SkuNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_PACKING> Packing = new List<C_PACKING>();

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM C_PACKING WHERE SKUNO='{SkuNo}'";
                dt = DB.ExecSelect(sql).Tables[0];
                Row_C_PACKING row = (Row_C_PACKING)NewRow();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row.loadData(dt.Rows[i]);
                    Packing.Add(row.GetDataObject());
                }
            }
            return Packing;
        }

        public C_PACKING GetPackingBySkuAndType(string SkuNo,string PackType, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            C_PACKING Packing = new C_PACKING();

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM C_PACKING WHERE SKUNO='{SkuNo}' AND PACK_TYPE='{PackType}' AND ROWNUM=1";
                dt = DB.ExecSelect(sql).Tables[0];
                Row_C_PACKING row = (Row_C_PACKING)NewRow();
                if (dt.Rows.Count > 0)
                {
                    row.loadData(dt.Rows[0]);
                    Packing = row.GetDataObject();
                }
            }
            return Packing;
        }
    }
    public class Row_C_PACKING : DataObjectBase
    {
        public Row_C_PACKING(DataObjectInfo info) : base(info)
        {

        }
        public C_PACKING GetDataObject()
        {
            C_PACKING DataObject = new C_PACKING();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PACK_TYPE = this.PACK_TYPE;
            DataObject.TRANSPORT_TYPE = this.TRANSPORT_TYPE;
            DataObject.INSIDE_PACK_TYPE = this.INSIDE_PACK_TYPE;
            DataObject.MAX_QTY = this.MAX_QTY;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.SN_RULE = this.SN_RULE;
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
        public string PACK_TYPE
        {
            get
            {
                return (string)this["PACK_TYPE"];
            }
            set
            {
                this["PACK_TYPE"] = value;
            }
        }
        public string TRANSPORT_TYPE
        {
            get
            {
                return (string)this["TRANSPORT_TYPE"];
            }
            set
            {
                this["TRANSPORT_TYPE"] = value;
            }
        }
        public string INSIDE_PACK_TYPE
        {
            get
            {
                return (string)this["INSIDE_PACK_TYPE"];
            }
            set
            {
                this["INSIDE_PACK_TYPE"] = value;
            }
        }
        public double? MAX_QTY
        {
            get
            {
                return (double?)this["MAX_QTY"];
            }
            set
            {
                this["MAX_QTY"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
        public string SN_RULE
        {
            get
            {
                return (string)this["SN_RULE"];
            }
            set
            {
                this["SN_RULE"] = value;
            }
        }
    }
    public class C_PACKING
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public string PACK_TYPE{get;set;}
        public string TRANSPORT_TYPE{get;set;}
        public string INSIDE_PACK_TYPE{get;set;}
        public double? MAX_QTY{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string SN_RULE{get;set;}
    }
}