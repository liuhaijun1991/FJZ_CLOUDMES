using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CRITICAL_BONEPILE : DataObjectTable
    {
        public T_R_CRITICAL_BONEPILE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CRITICAL_BONEPILE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CRITICAL_BONEPILE);
            TableName = "R_CRITICAL_BONEPILE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Save(OleExec DB, R_CRITICAL_BONEPILE normal_bonepile)
        {
            return DB.ORM.Insertable<R_CRITICAL_BONEPILE>(normal_bonepile).ExecuteCommand();
        }

        public int Update(OleExec DB, R_CRITICAL_BONEPILE normal_bonepile)
        {
            return DB.ORM.Updateable<R_CRITICAL_BONEPILE>(normal_bonepile).Where(r => r.ID == normal_bonepile.ID).ExecuteCommand();
        }

        public R_CRITICAL_BONEPILE GetOpenRecord(OleExec DB, string sn)
        {
            return DB.ORM.Queryable<R_CRITICAL_BONEPILE>().Where(r => r.SN == sn && r.CLOSED_FLAG == "0").ToList().FirstOrDefault();
        }
        public R_CRITICAL_BONEPILE GetOpenRecord(OleExec DB, string sn,string failStation)
        {
            return DB.ORM.Queryable<R_CRITICAL_BONEPILE>().Where(r => r.SN == sn && r.CLOSED_FLAG == "0" && r.FAIL_STATION == failStation).ToList().FirstOrDefault();
        }
        public List<R_CRITICAL_BONEPILE> GetRecordBySn(OleExec DB, string sn)
        {
            return DB.ORM.Queryable<R_CRITICAL_BONEPILE>().Where(r => r.SN == sn).ToList();
        }

    }
    public class Row_R_CRITICAL_BONEPILE : DataObjectBase
    {
        public Row_R_CRITICAL_BONEPILE(DataObjectInfo info) : base(info)
        {

        }
        public R_CRITICAL_BONEPILE GetDataObject()
        {
            R_CRITICAL_BONEPILE DataObject = new R_CRITICAL_BONEPILE();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.BRC_START_DATE = this.BRC_START_DATE;
            DataObject.FLH_START_DATE = this.FLH_START_DATE;
            DataObject.FIRST_LOAD_DATE = this.FIRST_LOAD_DATE;
            DataObject.CTO_SN_OLD = this.CTO_SN_OLD;
            DataObject.VANILLA_SN_OLD = this.VANILLA_SN_OLD;
            DataObject.PCBA_SN_OLD = this.PCBA_SN_OLD;
            DataObject.CTO_SN_NEW = this.CTO_SN_NEW;
            DataObject.VANILLA_SN_NEW = this.VANILLA_SN_NEW;
            DataObject.PCBA_SN_NEW = this.PCBA_SN_NEW;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PRODUCT_NAME = this.PRODUCT_NAME;
            DataObject.SUB_SERIES = this.SUB_SERIES;
            DataObject.PRODUCT_SERIES = this.PRODUCT_SERIES;
            DataObject.FAIL_STATION = this.FAIL_STATION;
            DataObject.FAILURE_SYMPTOM = this.FAILURE_SYMPTOM;
            DataObject.BONEPILE_DESCRIPTION = this.BONEPILE_DESCRIPTION;
            DataObject.BONEPILE_STATION = this.BONEPILE_STATION;
            DataObject.BONEPILE_CATEGORY = this.BONEPILE_CATEGORY;
            DataObject.OWNER = this.OWNER;
            DataObject.BRC_REMARK = this.BRC_REMARK;
            DataObject.FLH_REMARK = this.FLH_REMARK;
            DataObject.RMA_FLAG = this.RMA_FLAG;
            DataObject.CRITICAL_BONEPILE_FLAG = this.CRITICAL_BONEPILE_FLAG;
            DataObject.HARDCORE_BOARD = this.HARDCORE_BOARD;
            DataObject.PRICE = this.PRICE;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.CURRENT_STATUS = this.CURRENT_STATUS;
            DataObject.SCRAPPED_FLAG = this.SCRAPPED_FLAG;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.CLOSED_REASON = this.CLOSED_REASON;
            DataObject.CLOSED_BY = this.CLOSED_BY;
            DataObject.CLOSED_DATE = this.CLOSED_DATE;
            DataObject.UPLOAD_BY = this.UPLOAD_BY;
            DataObject.UPLOAD_DATE = this.UPLOAD_DATE;
            DataObject.UPLOADLH_BY = this.UPLOADLH_BY;
            DataObject.UPLOADLH_DATE = this.UPLOADLH_DATE;
            DataObject.LASTEDIT_BY = this.LASTEDIT_BY;
            DataObject.LASTEDIT_DATE = this.LASTEDIT_DATE;
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
        public DateTime? BRC_START_DATE
        {
            get
            {
                return (DateTime?)this["BRC_START_DATE"];
            }
            set
            {
                this["BRC_START_DATE"] = value;
            }
        }
        public DateTime? FLH_START_DATE
        {
            get
            {
                return (DateTime?)this["FLH_START_DATE"];
            }
            set
            {
                this["FLH_START_DATE"] = value;
            }
        }
        public DateTime? FIRST_LOAD_DATE
        {
            get
            {
                return (DateTime?)this["FIRST_LOAD_DATE"];
            }
            set
            {
                this["FIRST_LOAD_DATE"] = value;
            }
        }
        public string CTO_SN_OLD
        {
            get
            {
                return (string)this["CTO_SN_OLD"];
            }
            set
            {
                this["CTO_SN_OLD"] = value;
            }
        }
        public string VANILLA_SN_OLD
        {
            get
            {
                return (string)this["VANILLA_SN_OLD"];
            }
            set
            {
                this["VANILLA_SN_OLD"] = value;
            }
        }
        public string PCBA_SN_OLD
        {
            get
            {
                return (string)this["PCBA_SN_OLD"];
            }
            set
            {
                this["PCBA_SN_OLD"] = value;
            }
        }
        public string CTO_SN_NEW
        {
            get
            {
                return (string)this["CTO_SN_NEW"];
            }
            set
            {
                this["CTO_SN_NEW"] = value;
            }
        }
        public string VANILLA_SN_NEW
        {
            get
            {
                return (string)this["VANILLA_SN_NEW"];
            }
            set
            {
                this["VANILLA_SN_NEW"] = value;
            }
        }
        public string PCBA_SN_NEW
        {
            get
            {
                return (string)this["PCBA_SN_NEW"];
            }
            set
            {
                this["PCBA_SN_NEW"] = value;
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
        public string PRODUCT_NAME
        {
            get
            {
                return (string)this["PRODUCT_NAME"];
            }
            set
            {
                this["PRODUCT_NAME"] = value;
            }
        }
        public string SUB_SERIES
        {
            get
            {
                return (string)this["SUB_SERIES"];
            }
            set
            {
                this["SUB_SERIES"] = value;
            }
        }
        public string PRODUCT_SERIES
        {
            get
            {
                return (string)this["PRODUCT_SERIES"];
            }
            set
            {
                this["PRODUCT_SERIES"] = value;
            }
        }
        public string FAIL_STATION
        {
            get
            {
                return (string)this["FAIL_STATION"];
            }
            set
            {
                this["FAIL_STATION"] = value;
            }
        }
        public string FAILURE_SYMPTOM
        {
            get
            {
                return (string)this["FAILURE_SYMPTOM"];
            }
            set
            {
                this["FAILURE_SYMPTOM"] = value;
            }
        }
        public string BONEPILE_DESCRIPTION
        {
            get
            {
                return (string)this["BONEPILE_DESCRIPTION"];
            }
            set
            {
                this["BONEPILE_DESCRIPTION"] = value;
            }
        }
        public string BONEPILE_STATION
        {
            get
            {
                return (string)this["BONEPILE_STATION"];
            }
            set
            {
                this["BONEPILE_STATION"] = value;
            }
        }
        public string BONEPILE_CATEGORY
        {
            get
            {
                return (string)this["BONEPILE_CATEGORY"];
            }
            set
            {
                this["BONEPILE_CATEGORY"] = value;
            }
        }
        public string OWNER
        {
            get
            {
                return (string)this["OWNER"];
            }
            set
            {
                this["OWNER"] = value;
            }
        }
        public string BRC_REMARK
        {
            get
            {
                return (string)this["BRC_REMARK"];
            }
            set
            {
                this["BRC_REMARK"] = value;
            }
        }
        public string FLH_REMARK
        {
            get
            {
                return (string)this["FLH_REMARK"];
            }
            set
            {
                this["FLH_REMARK"] = value;
            }
        }
        public string RMA_FLAG
        {
            get
            {
                return (string)this["RMA_FLAG"];
            }
            set
            {
                this["RMA_FLAG"] = value;
            }
        }
        public string CRITICAL_BONEPILE_FLAG
        {
            get
            {
                return (string)this["CRITICAL_BONEPILE_FLAG"];
            }
            set
            {
                this["CRITICAL_BONEPILE_FLAG"] = value;
            }
        }
        public string HARDCORE_BOARD
        {
            get
            {
                return (string)this["HARDCORE_BOARD"];
            }
            set
            {
                this["HARDCORE_BOARD"] = value;
            }
        }
        public double? PRICE
        {
            get
            {
                return (double?)this["PRICE"];
            }
            set
            {
                this["PRICE"] = value;
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
        public string CURRENT_STATUS
        {
            get
            {
                return (string)this["CURRENT_STATUS"];
            }
            set
            {
                this["CURRENT_STATUS"] = value;
            }
        }
        public string SCRAPPED_FLAG
        {
            get
            {
                return (string)this["SCRAPPED_FLAG"];
            }
            set
            {
                this["SCRAPPED_FLAG"] = value;
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
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public string CLOSED_REASON
        {
            get
            {
                return (string)this["CLOSED_REASON"];
            }
            set
            {
                this["CLOSED_REASON"] = value;
            }
        }
        public string CLOSED_BY
        {
            get
            {
                return (string)this["CLOSED_BY"];
            }
            set
            {
                this["CLOSED_BY"] = value;
            }
        }
        public DateTime? CLOSED_DATE
        {
            get
            {
                return (DateTime?)this["CLOSED_DATE"];
            }
            set
            {
                this["CLOSED_DATE"] = value;
            }
        }
        public string UPLOAD_BY
        {
            get
            {
                return (string)this["UPLOAD_BY"];
            }
            set
            {
                this["UPLOAD_BY"] = value;
            }
        }
        public DateTime? UPLOAD_DATE
        {
            get
            {
                return (DateTime?)this["UPLOAD_DATE"];
            }
            set
            {
                this["UPLOAD_DATE"] = value;
            }
        }
        public string UPLOADLH_BY
        {
            get
            {
                return (string)this["UPLOADLH_BY"];
            }
            set
            {
                this["UPLOADLH_BY"] = value;
            }
        }
        public DateTime? UPLOADLH_DATE
        {
            get
            {
                return (DateTime?)this["UPLOADLH_DATE"];
            }
            set
            {
                this["UPLOADLH_DATE"] = value;
            }
        }
        public string LASTEDIT_BY
        {
            get
            {
                return (string)this["LASTEDIT_BY"];
            }
            set
            {
                this["LASTEDIT_BY"] = value;
            }
        }
        public DateTime? LASTEDIT_DATE
        {
            get
            {
                return (DateTime?)this["LASTEDIT_DATE"];
            }
            set
            {
                this["LASTEDIT_DATE"] = value;
            }
        }
    }
    public class R_CRITICAL_BONEPILE
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public DateTime? BRC_START_DATE { get; set; }
        public DateTime? FLH_START_DATE { get; set; }
        public DateTime? FIRST_LOAD_DATE { get; set; }
        public string CTO_SN_OLD { get; set; }
        public string VANILLA_SN_OLD { get; set; }
        public string PCBA_SN_OLD { get; set; }
        public string CTO_SN_NEW { get; set; }
        public string VANILLA_SN_NEW { get; set; }
        public string PCBA_SN_NEW { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SUB_SERIES { get; set; }
        public string PRODUCT_SERIES { get; set; }
        public string FAIL_STATION { get; set; }
        public string FAILURE_SYMPTOM { get; set; }
        public string BONEPILE_DESCRIPTION { get; set; }
        public string BONEPILE_STATION { get; set; }
        public string BONEPILE_CATEGORY { get; set; }
        public string OWNER { get; set; }
        public string BRC_REMARK { get; set; }
        public string FLH_REMARK { get; set; }
        public string RMA_FLAG { get; set; }
        public string CRITICAL_BONEPILE_FLAG { get; set; }
        public string HARDCORE_BOARD { get; set; }
        public double? PRICE { get; set; }
        public string CURRENT_STATION { get; set; }
        public string CURRENT_STATUS { get; set; }
        public string SCRAPPED_FLAG { get; set; }
        public string SHIPPED_FLAG { get; set; }
        public string CLOSED_FLAG { get; set; }
        public string CLOSED_REASON { get; set; }
        public string CLOSED_BY { get; set; }
        public DateTime? CLOSED_DATE { get; set; }
        public string UPLOAD_BY { get; set; }
        public DateTime? UPLOAD_DATE { get; set; }
        public string UPLOADLH_BY { get; set; }
        public DateTime? UPLOADLH_DATE { get; set; }
        public string LASTEDIT_BY { get; set; }
        public DateTime? LASTEDIT_DATE { get; set; }
    }
}