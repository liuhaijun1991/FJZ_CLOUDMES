using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_C_STOCK_CONFIG : DataObjectTable
    {
        public T_C_STOCK_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_STOCK_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_STOCK_CONFIG);
            TableName = "C_STOCK_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_STOCK_CONFIG> GetAllStock(OleExec DB)
        {
            string strSql = $@"select * from c_stock_config order by section, location";
            List<C_STOCK_CONFIG> result = new List<C_STOCK_CONFIG>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STOCK_CONFIG ret = (Row_C_STOCK_CONFIG)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<C_STOCK_CONFIG> GetStockByValue(string value, OleExec DB)
        {
            string strSql = $@"select * from c_stock_config where upper(location) like '%{value}%' or upper(section)='{value}' order by section, location";
            List<C_STOCK_CONFIG> result = new List<C_STOCK_CONFIG>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STOCK_CONFIG ret = (Row_C_STOCK_CONFIG)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<C_STOCK_CONFIG> GetStockByValue(string section, string location, OleExec DB)
        {
            string strSql = $@"select * from c_stock_config where upper(location)='{location}' and upper(section)='{section}'";
            List<C_STOCK_CONFIG> result = new List<C_STOCK_CONFIG>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_STOCK_CONFIG ret = (Row_C_STOCK_CONFIG)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public C_STOCK_CONFIG GetStockByid(string id, OleExec DB)
        {
            string strSql = $@"select * from c_stock_config where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_STOCK_CONFIG ret = (Row_C_STOCK_CONFIG)NewRow();
                ret.loadData(res.Rows[0]);

                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public int DeleteStockById(string Id, OleExec DB)
        {
            string strSql = $@"delete c_stock_config where id=:Id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int AddNewStockConfig(C_STOCK_CONFIG stock, OleExec DB)
        {
            Row_C_STOCK_CONFIG row_Stock = (Row_C_STOCK_CONFIG)NewRow();
            row_Stock.ID = stock.ID;
            row_Stock.SECTION = stock.SECTION;
            row_Stock.LOCATION = stock.LOCATION;
            row_Stock.EDIT_EMP = stock.EDIT_EMP;
            row_Stock.EDIT_TIME = stock.EDIT_TIME;
            int result = DB.ExecuteNonQuery(row_Stock.GetInsertString(DBType), CommandType.Text);
            return result;
        }
    }
    public class Row_C_STOCK_CONFIG : DataObjectBase
    {
        public Row_C_STOCK_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public C_STOCK_CONFIG GetDataObject()
        {
            C_STOCK_CONFIG DataObject = new C_STOCK_CONFIG();
            DataObject.ID = this.ID;
            DataObject.SECTION = this.SECTION;
            DataObject.LOCATION = this.LOCATION;
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
        public string SECTION
        {
            get
            {
                return (string)this["SECTION"];
            }
            set
            {
                this["SECTION"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
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
    public class C_STOCK_CONFIG
    {
        public string ID { get; set; }
        public string SECTION { get; set; }
        public string LOCATION { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}
