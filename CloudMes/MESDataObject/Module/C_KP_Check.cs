using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_Check : DataObjectTable
    {
        public T_C_KP_Check(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_Check(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_Check);
            TableName = "C_KP_Check".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_KP_Check> GetCKpCheckByType(string TypeName,OleExec DB)
        {
            string sql = TypeName.Equals("") ? $@"SELECT * FROM c_kp_check " : $@"SELECT * FROM c_kp_check where typename='{TypeName}'";
            DataSet ds = DB.ExecSelect(sql);
            List<C_KP_Check> List = new List<C_KP_Check>();
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_C_KP_Check row = (Row_C_KP_Check)this.NewRow();
                row.loadData(VARIABLE);
                List.Add(row.GetDataObject());
            }
            return List;
        }

        public bool IsExists(string SkuTypeName, string Dll, string Class, string Function, OleExec DB)
        {
            string sql = $@" select * from c_kp_check where typename='{SkuTypeName}' and dll ='{Dll}' and class='{Class}' and function='{Function}' ";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            if (dsCSkuMpn.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
    }
    public class Row_C_KP_Check : DataObjectBase
    {
        public Row_C_KP_Check(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_Check GetDataObject()
        {
            C_KP_Check DataObject = new C_KP_Check();
            DataObject.ID = this.ID;
            DataObject.TYPENAME = this.TYPENAME;
            DataObject.DLL = this.DLL;
            DataObject.CLASS = this.CLASS;
            DataObject.FUNCTION = this.FUNCTION;
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
        public string TYPENAME
        {
            get
            {
                return (string)this["TYPENAME"];
            }
            set
            {
                this["TYPENAME"] = value;
            }
        }
        public string DLL
        {
            get
            {
                return (string)this["DLL"];
            }
            set
            {
                this["DLL"] = value;
            }
        }
        public string CLASS
        {
            get
            {
                return (string)this["CLASS"];
            }
            set
            {
                this["CLASS"] = value;
            }
        }
        public string FUNCTION
        {
            get
            {
                return (string)this["FUNCTION"];
            }
            set
            {
                this["FUNCTION"] = value;
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
    public class C_KP_Check
    {
        public string ID{get;set;}
        public string TYPENAME{get;set;}
        public string DLL{get;set;}
        public string CLASS{get;set;}
        public string FUNCTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}