using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_STOCK_GT : DataObjectTable
    {
        public T_R_STOCK_GT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_STOCK_GT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_STOCK_GT);
            TableName = "R_STOCK_GT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool WOIsExistAndNotGT(string wo, OleExec sfcdb)
        {
            string sql = $@"select * from R_STOCK_GT where workorderno='{wo}' and sap_flag='0' and backflush_time is null ";
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

        /// <summary>
        /// 獲取未拋賬的工單
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="confirmed_flag"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public R_STOCK_GT GetNotGTbjByWO(string wo, string confirmed_flag, OleExec sfcdb)
        {
            string sql = $@"select * from R_STOCK_GT where workorderno='{wo}' and sap_flag='0' and confirmed_flag='{confirmed_flag}' and backflush_time is null ";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];            
            if (dt.Rows.Count > 0)
            {
                Row_R_STOCK_GT rowObj = (Row_R_STOCK_GT)this.NewRow();
                rowObj.loadData(dt.Rows[0]);
                return rowObj.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  按stock_type獲取未拋賬的工單
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="confirmed_flag"></param>
        /// <param name="stock_type"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public R_STOCK_GT GetNotGTbjByWOAndStockType(string wo, string confirmed_flag, string stock_type, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_STOCK_GT>().Where(r => r.WORKORDERNO == wo && r.SAP_FLAG == "0" && r.CONFIRMED_FLAG == confirmed_flag && r.STOCK_TYPE == stock_type)
                .ToList().FirstOrDefault();            
        }

        /// <summary>
        /// 獲取未拋賬的工單list
        /// </summary>
        /// <param name="confirmed_flag"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_STOCK_GT> GetNotGTListByConfirmedFlag(string confirmed_flag,OleExec sfcdb)
        {
            string sql = $@"select * from R_STOCK_GT where 1=1 and sap_flag='0' and confirmed_flag='{confirmed_flag}' ";
            List<R_STOCK_GT> GTList = new List<R_STOCK_GT>();
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {               
                foreach (DataRow row in dt.Rows)
                {
                    Row_R_STOCK_GT rowObj = (Row_R_STOCK_GT)this.NewRow();
                    rowObj.loadData(row);
                    GTList.Add(rowObj.GetDataObject());
                }
            }            
            return GTList;
        }

        public List<R_STOCK_GT> GetNotGTListByFlag(OleExec sfcdb,string sap_flag, string confirmed_flag)
        {
            return sfcdb.ORM.Queryable<R_STOCK_GT>().Where(r => r.SAP_FLAG == sap_flag && r.CONFIRMED_FLAG == confirmed_flag).ToList();
        }


        public R_STOCK_GT GetNotGTBySku(string sku, string confirmed_flag, string stock_type, OleExec sfcdb)
        {
            //return sfcdb.ORM.Queryable<R_STOCK_GT>().Where(gt => gt.SKUNO == sku && gt.CONFIRMED_FLAG == confirmed_flag
            //&& gt.STOCK_TYPE == stock_type && gt.SAP_FLAG == "0").ToList().FirstOrDefault();

            string sql = $@"select * from R_STOCK_GT where skuno='{sku}' and sap_flag='0' and confirmed_flag='{confirmed_flag}' and stock_type='{stock_type}' and backflush_time is null ";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Row_R_STOCK_GT rowObj = (Row_R_STOCK_GT)this.NewRow();
                rowObj.loadData(dt.Rows[0]);
                return rowObj.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public List<R_STOCK_GT> GetNotGTList(string confirmed_flag, string stock_type, OleExec sfcdb)
        {
            //return sfcdb.ORM.Queryable<R_STOCK_GT>().Where(gt => gt.SAP_FLAG == "0" && gt.CONFIRMED_FLAG == confirmed_flag && gt.STOCK_TYPE == stock_type).ToList();

            string sql = $@"select * from R_STOCK_GT where 1=1 and sap_flag='0' and confirmed_flag='{confirmed_flag}' and stock_type='{stock_type}'";
            List<R_STOCK_GT> GTList = new List<R_STOCK_GT>();
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Row_R_STOCK_GT rowObj = (Row_R_STOCK_GT)this.NewRow();
                    rowObj.loadData(row);
                    GTList.Add(rowObj.GetDataObject());
                }
            }
            return GTList;

        }

        public int Insert(OleExec sfcdb, R_STOCK_GT gtObj)
        {
            return sfcdb.ORM.Insertable<R_STOCK_GT>(gtObj).ExecuteCommand();
        }
        public int Update(OleExec sfcdb, R_STOCK_GT gtObj)
        {
            return sfcdb.ORM.Updateable<R_STOCK_GT>(gtObj).Where(r => r.ID == gtObj.ID).ExecuteCommand();
        }       
    }
    public class Row_R_STOCK_GT : DataObjectBase
    {
        public Row_R_STOCK_GT(DataObjectInfo info) : base(info)
        {

        }
        public R_STOCK_GT GetDataObject()
        {
            R_STOCK_GT DataObject = new R_STOCK_GT();
            DataObject.STOCK_TYPE = this.STOCK_TYPE;
            DataObject.SAP_STATION_CODE = this.SAP_STATION_CODE;
            DataObject.BACKFLUSH_TIME = this.BACKFLUSH_TIME;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.SAP_MESSAGE = this.SAP_MESSAGE;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.CONFIRMED_FLAG = this.CONFIRMED_FLAG;
            DataObject.TO_STORAGE = this.TO_STORAGE;
            DataObject.FROM_STORAGE = this.FROM_STORAGE;
            DataObject.TOTAL_QTY = this.TOTAL_QTY;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
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
        public string SAP_STATION_CODE
        {
            get
            {
                return (string)this["SAP_STATION_CODE"];
            }
            set
            {
                this["SAP_STATION_CODE"] = value;
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
        public string SAP_MESSAGE
        {
            get
            {
                return (string)this["SAP_MESSAGE"];
            }
            set
            {
                this["SAP_MESSAGE"] = value;
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
        public double? TOTAL_QTY
        {
            get
            {
                return (double?)this["TOTAL_QTY"];
            }
            set
            {
                this["TOTAL_QTY"] = value;
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
    public class R_STOCK_GT
    {
        public string STOCK_TYPE { get; set; }
        public string SAP_STATION_CODE { get; set; }
        public DateTime? BACKFLUSH_TIME { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string SAP_MESSAGE { get; set; }
        public string SAP_FLAG { get; set; }
        public string CONFIRMED_FLAG { get; set; }
        public string TO_STORAGE { get; set; }
        public string FROM_STORAGE { get; set; }
        public double? TOTAL_QTY { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string ID { get; set; }
    }
}