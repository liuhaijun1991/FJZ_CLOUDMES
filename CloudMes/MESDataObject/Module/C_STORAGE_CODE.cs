using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_STORAGE_CODE : DataObjectTable
    {
        public T_C_STORAGE_CODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_STORAGE_CODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_STORAGE_CODE);
            TableName = "C_STORAGE_CODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_STORAGE_CODE> GetAll(OleExec DB)
        {
            string strSql = $@"select * from c_storage_code ";        
            List<C_STORAGE_CODE> result = new List<C_STORAGE_CODE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STORAGE_CODE ret = (Row_C_STORAGE_CODE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
            }
            return result;
        }
        public C_STORAGE_CODE GetById(string id,OleExec DB)
        {
            string strSql = $@"select * from c_storage_code where id=:id ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text,paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_STORAGE_CODE ret = (Row_C_STORAGE_CODE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public List<C_STORAGE_CODE> GetByStorageCode(string StorageCode, OleExec DB)
        {
            string strSql = $@"select * from c_storage_code where storage_code=:StorageCode ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":StorageCode", StorageCode) };
            List<C_STORAGE_CODE> result = new List<C_STORAGE_CODE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text,paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STORAGE_CODE ret = (Row_C_STORAGE_CODE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
            }
            return result;
        }
        public List<C_STORAGE_CODE> GetByPlant(string strPlant, OleExec DB)
        {
            string strSql = $@"select * from c_storage_code where plant=:strPlant ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":strPlant", strPlant) };
            List<C_STORAGE_CODE> result = new List<C_STORAGE_CODE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STORAGE_CODE ret = (Row_C_STORAGE_CODE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
            }
            return result;
        }
        public C_STORAGE_CODE GetByPlantAndStorageCode(string strPlant,string strStorageCode, OleExec DB)
        {
            string strSql = $@"select * from c_storage_code where plant=:strPlant and storage_code=:StorageCode ";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":strPlant", strPlant),
                new OleDbParameter(":StorageCode", strStorageCode)
            };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text,paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_STORAGE_CODE ret = (Row_C_STORAGE_CODE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public int Add(C_STORAGE_CODE NewStorageCode, OleExec DB)
        {
            Row_C_STORAGE_CODE NewStorageRow = (Row_C_STORAGE_CODE)NewRow();
            NewStorageRow.ID = NewStorageCode.ID;
            NewStorageRow.PLANT = NewStorageCode.PLANT;
            NewStorageRow.STORAGE_CODE = NewStorageCode.STORAGE_CODE;
            NewStorageRow.DESCRIPTION = NewStorageCode.DESCRIPTION;
            NewStorageRow.EDIT_EMP = NewStorageCode.EDIT_EMP;
            NewStorageRow.EDIT_TIME = NewStorageCode.EDIT_TIME; 
            int result = DB.ExecuteNonQuery(NewStorageRow.GetInsertString(DBType), CommandType.Text);
            return result;
        }
        public int DeleteById(string id, OleExec DB)
        {
            string strSql = $@"delete c_storage_code  where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[]{new OleDbParameter(":id", id)};
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int UpdateById(C_STORAGE_CODE NewStorageCode, OleExec DB)
        {
            Row_C_STORAGE_CODE NewStorageRow = (Row_C_STORAGE_CODE)NewRow();
            NewStorageRow.ID = NewStorageCode.ID;
            NewStorageRow.PLANT = NewStorageCode.PLANT;
            NewStorageRow.STORAGE_CODE = NewStorageCode.STORAGE_CODE;
            NewStorageRow.DESCRIPTION = NewStorageCode.DESCRIPTION;
            NewStorageRow.EDIT_EMP = NewStorageCode.EDIT_EMP;
            NewStorageRow.EDIT_TIME = NewStorageCode.EDIT_TIME;
            int result = DB.ExecuteNonQuery(NewStorageRow.GetUpdateString(DBType, NewStorageCode.ID), CommandType.Text);
            return result;
        }
    }
    public class Row_C_STORAGE_CODE : DataObjectBase
    {
        public Row_C_STORAGE_CODE(DataObjectInfo info) : base(info)
        {

        }
        public C_STORAGE_CODE GetDataObject()
        {
            C_STORAGE_CODE DataObject = new C_STORAGE_CODE();
            DataObject.ID = this.ID;
            DataObject.PLANT = this.PLANT;
            DataObject.STORAGE_CODE = this.STORAGE_CODE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string STORAGE_CODE
        {
            get
            {
                return (string)this["STORAGE_CODE"];
            }
            set
            {
                this["STORAGE_CODE"] = value;
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
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string STORAGE_NAME
        {
            get
            {
                return (string)this["STORAGE_NAME"];
            }
            set
            {
                this["STORAGE_NAME"] = value;
            }
        }
    }
    public class C_STORAGE_CODE
    {
        public string ID{get;set;}
        public string PLANT{get;set;}
        public string STORAGE_CODE{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string CATEGORY { get; set; }
        public string STORAGE_NAME { get; set; }
    }
}