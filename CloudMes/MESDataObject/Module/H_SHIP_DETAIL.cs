using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_SHIP_DETAIL : DataObjectTable
    {
        public T_H_SHIP_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_SHIP_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_SHIP_DETAIL);
            TableName = "H_SHIP_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool HasSN(string SN, OleExec DB)
        {
            bool res = false;
            string sql = $@"select * from h_ship_detail where sn='{SN}'";
            DataTable Dt = DB.ExecSelect(sql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }
        public bool GetSN(string SN, OleExec DB)
        {
            bool res = false;
            string sql = $@"select * from h_ship_detail where sn='{SN}'";
            int Num = DB.ExecSqlNoReturn(sql, null);
            if (Num > 0)
            {
                res = true;
            }
            return res;
        }
    }
    public class Row_H_SHIP_DETAIL : DataObjectBase
    {
        public Row_H_SHIP_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public H_SHIP_DETAIL GetDataObject()
        {
            H_SHIP_DETAIL DataObject = new H_SHIP_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.DN_NO = this.DN_NO;
            DataObject.DN_LINE = this.DN_LINE;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
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
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string DN_LINE
        {
            get
            {
                return (string)this["DN_LINE"];
            }
            set
            {
                this["DN_LINE"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
    }
    public class H_SHIP_DETAIL
    {
        public string ID;
        public string SN;
        public string SKUNO;
        public string DN_NO;
        public string DN_LINE;
        public DateTime? SHIPDATE;
        public string CREATEBY;
    }
}