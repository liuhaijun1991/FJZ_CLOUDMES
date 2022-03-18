using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module
{
    public class T_R_SAP_TEMP : DataObjectTable
    {
        public T_R_SAP_TEMP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_TEMP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_TEMP);
            TableName = "R_SAP_TEMP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 獲取用於StockInBackFlush拋帳的數據
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_SAP_TEMP> GetStockInBackFlushList(OleExec sfcdb)
        {
            List<R_SAP_TEMP> backFlushList = new List<R_SAP_TEMP>();
            Row_R_SAP_TEMP r_sap_temp = null;
            string sql = "select * from sfcruntime.r_sap_temp t where t.sap_flag = 0";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    r_sap_temp =(Row_R_SAP_TEMP)this.NewRow();
                    r_sap_temp.loadData(dr);
                    backFlushList.Add(r_sap_temp.GetDataObject());
                }
            }
            return backFlushList;
        }
    }
    public class Row_R_SAP_TEMP : DataObjectBase
    {
        public Row_R_SAP_TEMP(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_TEMP GetDataObject()
        {
            R_SAP_TEMP DataObject = new R_SAP_TEMP();
            DataObject.ID = this.ID;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.FAIL_COUNT = this.FAIL_COUNT;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.TO_STORAGE = this.TO_STORAGE;
            DataObject.FROM_STORAGE = this.FROM_STORAGE;
            DataObject.QTY = this.QTY;
            DataObject.TYPE = this.TYPE;
            DataObject.SKUNO = this.SKUNO;
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
        public double? FAIL_COUNT
        {
            get
            {
                return (double?)this["FAIL_COUNT"];
            }
            set
            {
                this["FAIL_COUNT"] = value;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
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
    }
    public class R_SAP_TEMP
    {
        public string ID{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public double? FAIL_COUNT{get;set;}
        public string SAP_FLAG{get;set;}
        public string TO_STORAGE{get;set;}
        public string FROM_STORAGE{get;set;}
        public double? QTY{get;set;}
        public string TYPE{get;set;}
        public string SKUNO{get;set;}
    }
}
