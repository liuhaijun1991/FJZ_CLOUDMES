using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_CODE_MAPPING : DataObjectTable
    {
        public T_C_CODE_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CODE_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CODE_MAPPING);
            TableName = "C_CODE_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_CODE_MAPPING> GetDataByName(string name, OleExec DB)
        {
            //List<Row_C_CODE_MAPPING> RET = new List<Row_C_CODE_MAPPING>();
            //string strSql = $@"select * from c_code_mapping where codetype='{name}' order by SEQ";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    Row_C_CODE_MAPPING newRow = (Row_C_CODE_MAPPING)NewRow();
            //    newRow.loadData(res.Tables[0].Rows[i]);
            //    RET.Add(newRow);
            //}

            //return RET;
            return DB.ORM.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == name).OrderBy(t => t.SEQ).ToList();
            

        }


    }
    public class Row_C_CODE_MAPPING : DataObjectBase
    {
        public Row_C_CODE_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public C_CODE_MAPPING GetDataObject()
        {
            C_CODE_MAPPING DataObject = new C_CODE_MAPPING();
            DataObject.SEQ = this.SEQ;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CODEVALUE = this.CODEVALUE;
            DataObject.VALUE = this.VALUE;
            DataObject.CODETYPE = this.CODETYPE;
            DataObject.ID = this.ID;
            return DataObject;
        }

        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
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
        public string CODEVALUE
        {
            get
            {
                return (string)this["CODEVALUE"];
            }
            set
            {
                this["CODEVALUE"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public string CODETYPE
        {
            get
            {
                return (string)this["CODETYPE"];
            }
            set
            {
                this["CODETYPE"] = value;
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
    public class C_CODE_MAPPING
    {
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string CODEVALUE{get;set;}
        public string VALUE{get;set;}
        public string CODETYPE{get;set;}
        public string ID{get;set;}
        public double? SEQ{get;set;}
    }
}