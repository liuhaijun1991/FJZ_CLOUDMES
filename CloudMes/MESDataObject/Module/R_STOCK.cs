using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_STOCK : DataObjectTable
    {
        public T_R_STOCK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_STOCK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_STOCK);
            TableName = "R_STOCK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool IsStockIn(string sn,string wo,OleExec sfcdb)
        {
            string sql = $@" select * from R_STOCK where sn='{sn}' and workorderno='{wo}' ";            
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<R_STOCK> GetStockListByWo(string wo,OleExec sfcdb)
        {
            string sql = $@" select * from R_STOCK where workorderno='{wo}' ";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            Row_R_STOCK rowStock;
            List<R_STOCK> stockList = new List<R_STOCK>();
            if (dt.Rows.Count > 0)
            {
                foreach(DataRow row in dt.Rows)
                {
                    rowStock = (Row_R_STOCK)this.NewRow();
                    rowStock.loadData(row);
                    stockList.Add(rowStock.GetDataObject());
                }                
            }
            return stockList;
        }

        public string UpdateSapFlagByGTID(string gtId,string sapFlag,OleExec sfcdb)
        {
            string sql = $@"  update r_stock set sap_flag='{sapFlag}',backflush_time=sysdate where gt_id='{gtId}' ";
            return sfcdb.ExecSQL(sql);
        }
    }
    public class Row_R_STOCK : DataObjectBase
    {
        public Row_R_STOCK(DataObjectInfo info) : base(info)
        {

        }
        public R_STOCK GetDataObject()
        {
            R_STOCK DataObject = new R_STOCK();
            DataObject.STOCK_TYPE = this.STOCK_TYPE;
            DataObject.GT_ID = this.GT_ID;
            DataObject.BACKFLUSH_TIME = this.BACKFLUSH_TIME;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.CONFIRMED_FLAG = this.CONFIRMED_FLAG;
            DataObject.TO_STORAGE = this.TO_STORAGE;
            DataObject.FROM_STORAGE = this.FROM_STORAGE;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SN = this.SN;
            DataObject.ID = this.ID;
            return DataObject;
        }

        public string STOCK_TYPE
        {
            get
            {
                return (string)this["STOCK_TYPE"];
            }
            set
            {
                this["STOCK_TYPE"] = value;
            }
        }
        public string GT_ID
        {
            get
            {
                return (string)this["GT_ID"];
            }
            set
            {
                this["GT_ID"] = value;
            }
        }
        public DateTime? BACKFLUSH_TIME
        {
            get
            {
                return (DateTime?)this["BACKFLUSH_TIME"];
            }
            set
            {
                this["BACKFLUSH_TIME"] = value;
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
        public string SAP_FLAG
        {
            get
            {
                return (string)this["SAP_FLAG"];
            }
            set
            {
                this["SAP_FLAG"] = value;
            }
        }
        public string CONFIRMED_FLAG
        {
            get
            {
                return (string)this["CONFIRMED_FLAG"];
            }
            set
            {
                this["CONFIRMED_FLAG"] = value;
            }
        }
        public string TO_STORAGE
        {
            get
            {
                return (string)this["TO_STORAGE"];
            }
            set
            {
                this["TO_STORAGE"] = value;
            }
        }
        public string FROM_STORAGE
        {
            get
            {
                return (string)this["FROM_STORAGE"];
            }
            set
            {
                this["FROM_STORAGE"] = value;
            }
        }
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
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
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
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
    public class R_STOCK
    {
        public string STOCK_TYPE { get; set; }
        public string GT_ID { get; set; }
        public DateTime? BACKFLUSH_TIME { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string SAP_FLAG { get; set; }
        public string CONFIRMED_FLAG { get; set; }
        public string TO_STORAGE { get; set; }
        public string FROM_STORAGE { get; set; }
        public string NEXT_STATION { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string SN { get; set; }
        public string ID { get; set; }
    }
}