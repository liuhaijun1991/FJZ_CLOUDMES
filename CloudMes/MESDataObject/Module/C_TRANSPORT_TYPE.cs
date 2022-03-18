using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_TRANSPORT_TYPE : DataObjectTable
    {
        public T_C_TRANSPORT_TYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_TRANSPORT_TYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_TRANSPORT_TYPE);
            TableName = "C_TRANSPORT_TYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_TRANSPORT_TYPE> GetAllList(OleExec DB)
        {
            List<C_TRANSPORT_TYPE> ret = new List<C_TRANSPORT_TYPE>();
            string strSql = "select * from C_TRANSPORT_TYPE";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_TRANSPORT_TYPE r = (Row_C_TRANSPORT_TYPE)NewRow();
                r.loadData(res.Tables[0].Rows[i]);
                ret.Add(r.GetDataObject());
            }

            return ret;
        }
    }
    public class Row_C_TRANSPORT_TYPE : DataObjectBase
    {
        public Row_C_TRANSPORT_TYPE(DataObjectInfo info) : base(info)
        {

        }
        public C_TRANSPORT_TYPE GetDataObject()
        {
            C_TRANSPORT_TYPE DataObject = new C_TRANSPORT_TYPE();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.TRANSPORT_TYPE = this.TRANSPORT_TYPE;
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
    public class C_TRANSPORT_TYPE
    {
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string TRANSPORT_TYPE{get;set;}
        public string ID{get;set;}
    }
}