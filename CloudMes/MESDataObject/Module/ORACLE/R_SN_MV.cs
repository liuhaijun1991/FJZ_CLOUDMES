using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SN_MV : DataObjectTable
    {
        public T_R_SN_MV(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_MV(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_MV);
            TableName = "R_SN_MV".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SN_MV : DataObjectBase
    {
        public Row_R_SN_MV(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_MV GetDataObject()
        {
            R_SN_MV DataObject = new R_SN_MV();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PLANT = this.PLANT;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.STARTED_FLAG = this.STARTED_FLAG;
            DataObject.START_TIME = this.START_TIME;
            DataObject.PACKED_FLAG = this.PACKED_FLAG;
            DataObject.PACKDATE = this.PACKDATE;
            DataObject.COMPLETED_FLAG = this.COMPLETED_FLAG;
            DataObject.COMPLETED_TIME = this.COMPLETED_TIME;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.REPAIR_FAILED_FLAG = this.REPAIR_FAILED_FLAG;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.KP_LIST_ID = this.KP_LIST_ID;
            DataObject.PO_NO = this.PO_NO;
            DataObject.CUST_ORDER_NO = this.CUST_ORDER_NO;
            DataObject.CUST_PN = this.CUST_PN;
            DataObject.BOXSN = this.BOXSN;
            DataObject.SCRAPED_FLAG = this.SCRAPED_FLAG;
            DataObject.SCRAPED_TIME = this.SCRAPED_TIME;
            DataObject.PRODUCT_STATUS = this.PRODUCT_STATUS;
            DataObject.REWORK_COUNT = this.REWORK_COUNT;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.STOCK_STATUS = this.STOCK_STATUS;
            DataObject.STOCK_IN_TIME = this.STOCK_IN_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string ROUTE_ID
        {
            get
            {
                return (string)this["ROUTE_ID"];
            }
            set
            {
                this["ROUTE_ID"] = value;
            }
        }
        public string STARTED_FLAG
        {
            get
            {
                return (string)this["STARTED_FLAG"];
            }
            set
            {
                this["STARTED_FLAG"] = value;
            }
        }
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public string PACKED_FLAG
        {
            get
            {
                return (string)this["PACKED_FLAG"];
            }
            set
            {
                this["PACKED_FLAG"] = value;
            }
        }
        public DateTime? PACKDATE
        {
            get
            {
                return (DateTime?)this["PACKDATE"];
            }
            set
            {
                this["PACKDATE"] = value;
            }
        }
        public string COMPLETED_FLAG
        {
            get
            {
                return (string)this["COMPLETED_FLAG"];
            }
            set
            {
                this["COMPLETED_FLAG"] = value;
            }
        }
        public DateTime? COMPLETED_TIME
        {
            get
            {
                return (DateTime?)this["COMPLETED_TIME"];
            }
            set
            {
                this["COMPLETED_TIME"] = value;
            }
        }
        public string SHIPPED_FLAG
        {
            get
            {
                return (string)this["SHIPPED_FLAG"];
            }
            set
            {
                this["SHIPPED_FLAG"] = value;
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
        public string REPAIR_FAILED_FLAG
        {
            get
            {
                return (string)this["REPAIR_FAILED_FLAG"];
            }
            set
            {
                this["REPAIR_FAILED_FLAG"] = value;
            }
        }
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
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
        public string KP_LIST_ID
        {
            get
            {
                return (string)this["KP_LIST_ID"];
            }
            set
            {
                this["KP_LIST_ID"] = value;
            }
        }
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string CUST_ORDER_NO
        {
            get
            {
                return (string)this["CUST_ORDER_NO"];
            }
            set
            {
                this["CUST_ORDER_NO"] = value;
            }
        }
        public string CUST_PN
        {
            get
            {
                return (string)this["CUST_PN"];
            }
            set
            {
                this["CUST_PN"] = value;
            }
        }
        public string BOXSN
        {
            get
            {
                return (string)this["BOXSN"];
            }
            set
            {
                this["BOXSN"] = value;
            }
        }
        public string SCRAPED_FLAG
        {
            get
            {
                return (string)this["SCRAPED_FLAG"];
            }
            set
            {
                this["SCRAPED_FLAG"] = value;
            }
        }
        public DateTime? SCRAPED_TIME
        {
            get
            {
                return (DateTime?)this["SCRAPED_TIME"];
            }
            set
            {
                this["SCRAPED_TIME"] = value;
            }
        }
        public string PRODUCT_STATUS
        {
            get
            {
                return (string)this["PRODUCT_STATUS"];
            }
            set
            {
                this["PRODUCT_STATUS"] = value;
            }
        }
        public double? REWORK_COUNT
        {
            get
            {
                return (double?)this["REWORK_COUNT"];
            }
            set
            {
                this["REWORK_COUNT"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string STOCK_STATUS
        {
            get
            {
                return (string)this["STOCK_STATUS"];
            }
            set
            {
                this["STOCK_STATUS"] = value;
            }
        }
        public DateTime? STOCK_IN_TIME
        {
            get
            {
                return (DateTime?)this["STOCK_IN_TIME"];
            }
            set
            {
                this["STOCK_IN_TIME"] = value;
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
    }
    public class R_SN_MV
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string PLANT { get; set; }
        public string ROUTE_ID { get; set; }
        public string STARTED_FLAG { get; set; }
        public DateTime? START_TIME { get; set; }
        public string PACKED_FLAG { get; set; }
        public DateTime? PACKDATE { get; set; }
        public string COMPLETED_FLAG { get; set; }
        public DateTime? COMPLETED_TIME { get; set; }
        public string SHIPPED_FLAG { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string REPAIR_FAILED_FLAG { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string KP_LIST_ID { get; set; }
        public string PO_NO { get; set; }
        public string CUST_ORDER_NO { get; set; }
        public string CUST_PN { get; set; }
        public string BOXSN { get; set; }
        public string SCRAPED_FLAG { get; set; }
        public DateTime? SCRAPED_TIME { get; set; }
        public string PRODUCT_STATUS { get; set; }
        public double? REWORK_COUNT { get; set; }
        public string VALID_FLAG { get; set; }
        public string STOCK_STATUS { get; set; }
        public DateTime? STOCK_IN_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}