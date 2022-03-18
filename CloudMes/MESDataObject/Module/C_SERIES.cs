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
    public class T_C_SERIES : DataObjectTable
    {
        public T_C_SERIES(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SERIES(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SERIES);
            TableName = "C_SERIES".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        
        public C_SERIES GetDetailById(OleExec sfcdb, string seriesid)
        {
            string sql = null;
            DataTable dt = null;
            Row_C_SERIES r_c_series = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                if (seriesid.Length > 0)
                {
                    sql = $@"select * from {TableName} where id='{seriesid.Replace("'", "''")}' ";
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count == 0) return null;

                    r_c_series = (Row_C_SERIES)this.NewRow();
                    r_c_series.loadData(dt.Rows[0]);
                    return r_c_series == null ? null : r_c_series.GetDataObject();
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public bool isExist(OleExec sfcdb, string seriesName, string custid)
        {
            string sql = null;
            DataTable dt = null;
            //Row_C_SERIES r_c_series = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select * from {TableName} where customer_id='{custid.Replace("'", "''")}' and series_name='{seriesName.Replace("'","''")}' ";
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

                
            }
            else
            {
                throw new Exception();
            }
        }

        public DataTable GetQueryAll(string field, string value, OleExec sfcdb)
        {
            OleDbParameter[] paras = null;
            StringBuilder builder = new StringBuilder("select sis.id,sis.customer_id,cust.bu,cust.customer_name,sis.series_name,sis.description ");
            builder.Append("from c_series sis left join c_customer cust on sis.customer_id=cust.id where 1=1 ");
            switch (field)
            {
                case "BU":
                    builder.Append("and cust.bu=:Value ");
                    paras = new OleDbParameter[] { new OleDbParameter(":Value", value) };
                    break;
                case "CustomerName":
                    builder.Append("and cust.customer_name=:Value ");
                    paras = new OleDbParameter[] {  new OleDbParameter(":Value", value) };
                    break;
                case "SeriesName":
                    builder.Append("and sis.series_name=:Value ");
                    paras = new OleDbParameter[] { new OleDbParameter(":Value", value) };
                    break;
                case "ID":
                    builder.Append("and sis.id=:Value ");
                    paras = new OleDbParameter[] { new OleDbParameter(":Value", value) };
                    break;
                default:
                    //builder.Append("and cust.bu=:BU ");
                    //paras = new OleDbParameter[] { new OleDbParameter(":BU", this.BU) };
                    break;
            }
            return sfcdb.ExecuteDataTable(builder.ToString(), CommandType.Text, paras);
        }
    }
    public class Row_C_SERIES : DataObjectBase
    {
        public Row_C_SERIES(DataObjectInfo info) : base(info)
        {

        }
        public C_SERIES GetDataObject()
        {
            C_SERIES DataObject = new C_SERIES();
            DataObject.ID = this.ID;
            //DataObject.SERIES_ID = this.SERIES_ID;
            DataObject.CUSTOMER_ID = this.CUSTOMER_ID;
            DataObject.SERIES_NAME = this.SERIES_NAME;
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

        //public string SERIES_ID
        //{
        //    get
        //    {
        //        return (string)this["SERIES_ID"];
        //    }
        //    set
        //    {
        //        this["SERIES_ID"] = value;
        //    }
        //}
        
        public string CUSTOMER_ID
        {
            get
            {
                return (string)this["CUSTOMER_ID"];
            }
            set
            {
                this["CUSTOMER_ID"] = value;
            }
        }
        public string SERIES_NAME
        {
            get
            {
                return (string)this["SERIES_NAME"];
            }
            set
            {
                this["SERIES_NAME"] = value;
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
    }
    public class C_SERIES
    {
        public string ID{get;set;}
        //public string SERIES_ID{get;set;}
        public string CUSTOMER_ID{get;set;}
        public string SERIES_NAME{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}