using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_C_ACTION_CODE : DataObjectTable
    {
        public T_C_ACTION_CODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ACTION_CODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ACTION_CODE);
            TableName = "C_ACTION_CODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_ACTION_CODE GetByActionCode(string ActionCode, OleExec DB)
        {
            List<C_ACTION_CODE> ACs = DB.ORM.Queryable<C_ACTION_CODE>().Where(t => t.ACTION_CODE == ActionCode).ToList();
            if (ACs.Count > 0)
            {
                return ACs.First();
            }
            else
            {
                return null;
            }
        }
        public int AddNewActionCode(C_ACTION_CODE NewActionCode, OleExec DB)
        {
            return DB.ORM.Insertable<C_ACTION_CODE>(NewActionCode).ExecuteCommand();
        }
        public int UpdateById(C_ACTION_CODE NewActionCode, OleExec DB)
        {
            return DB.ORM.Updateable<C_ACTION_CODE>(NewActionCode).Where(t => t.ID == NewActionCode.ID).ExecuteCommand();
        }
        public C_ACTION_CODE GetByid(string id, OleExec DB)
        {
            List<C_ACTION_CODE> ACs = DB.ORM.Queryable<C_ACTION_CODE>().Where(t => t.ID == id).ToList();
            if (ACs.Count >0)
            {
                return ACs.First();
            }
            else
            {
                return null;
            }
        }
        public List<C_ACTION_CODE> GetByFuzzySearch(string ParametValue, OleExec DB)
        {
            return DB.ORM.Queryable<C_ACTION_CODE>()
                .Where(t => t.ACTION_CODE.ToUpper().Contains(ParametValue) || t.ENGLISH_DESC.ToUpper().Contains(ParametValue) || t.CHINESE_DESC.ToUpper().Contains(ParametValue))
                .ToList();
        }
        public List<C_ACTION_CODE> GetAllActionCode(OleExec DB)
        {
            return DB.ORM.Queryable<C_ACTION_CODE>().ToList();
        }
        public int DeleteById(string Id, OleExec DB)
        {
            return DB.ORM.Deleteable<C_ACTION_CODE>().Where(t => t.ID == Id).ExecuteCommand();
        }
    }
    public class Row_C_ACTION_CODE : DataObjectBase
    {
        public Row_C_ACTION_CODE(DataObjectInfo info) : base(info)
        {

        }
        public C_ACTION_CODE GetDataObject()
        {
            C_ACTION_CODE DataObject = new C_ACTION_CODE();
            DataObject.ID = this.ID;
            DataObject.ACTION_CODE = this.ACTION_CODE;
            DataObject.ENGLISH_DESC = this.ENGLISH_DESCRIPTION;
            DataObject.CHINESE_DESC = this.CHINESE_DESCRIPTION;
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
        public string ACTION_CODE
        {
            get
            {
                return (string)this["ACTION_CODE"];
            }
            set
            {
                this["ACTION_CODE"] = value;
            }
        }
        public string ENGLISH_DESCRIPTION
        {
            get
            {
                return (string)this["ENGLISH_DESCRIPTION"];
            }
            set
            {
                this["ENGLISH_DESCRIPTION"] = value;
            }
        }
        public string CHINESE_DESCRIPTION
        {
            get
            {
                return (string)this["CHINESE_DESCRIPTION"];
            }
            set
            {
                this["CHINESE_DESCRIPTION"] = value;
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
    public class C_ACTION_CODE
    {
        public string ID{get;set;}
        public string ACTION_CODE{get;set;}
        [SugarColumn(ColumnName = "ENGLISH_DESCRIPTION")]
        public string ENGLISH_DESC {get;set; }
        [SugarColumn(ColumnName = "CHINESE_DESCRIPTION")]
        public string CHINESE_DESC {get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}