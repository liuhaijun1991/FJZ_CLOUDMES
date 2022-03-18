using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_SN_STATION_DETAIL : DataObjectTable
    {
        public T_R_QUACK_SN_STATION_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_SN_STATION_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_SN_STATION_DETAIL);
            TableName = "R_QUACK_SN_STATION_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int InsertRowToRQuackSnStationDetail(R_QUACK_SN_STATION_DETAIL RQuackSnStationDetail , OleExec db)
        {
            return db.ORM.Insertable(RQuackSnStationDetail).ExecuteCommand();
        }

        public int UpdateRowsByStationAndQsn(R_QUACK_SN_STATION_DETAIL RQuackSnStationDetail, string StationName, string Qsn, OleExec db)
        {
            //Modify by LLF 2019-03-21
            //return db.ORM.Updateable(RQuackSnStationDetail).Where(t => t.STATION_NAME == StationName && t.QSN == Qsn).ExecuteCommand();
            return db.ORM.Updateable(RQuackSnStationDetail).Where(t => t.ID == RQuackSnStationDetail.ID).ExecuteCommand();
        }
		
        public List<R_QUACK_SN_STATION_DETAIL> GetRowsByStationAndQsn( string StationName, string Qsn, OleExec db)
        {
            return db.ORM.Queryable<R_QUACK_SN_STATION_DETAIL>().Where(t => t.STATION_NAME == StationName && t.QSN == Qsn).ToList();
        }
		
    }
    public class Row_R_QUACK_SN_STATION_DETAIL : DataObjectBase
    {
        public Row_R_QUACK_SN_STATION_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_SN_STATION_DETAIL GetDataObject()
        {
            R_QUACK_SN_STATION_DETAIL DataObject = new R_QUACK_SN_STATION_DETAIL();
            DataObject.ID = this.ID;
            DataObject.QSN = this.QSN;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SCAN_DATE = this.SCAN_DATE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PLANT = this.PLANT;
            DataObject.LINE = this.LINE;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.PRODUCT_STATUS = this.PRODUCT_STATUS;
            DataObject.PASS_FLAG = this.PASS_FLAG;
            DataObject.FAIL_FLAG = this.FAIL_FLAG;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.RFROM_EMP = this.RFROM_EMP;
            DataObject.RFROM_TIME = this.RFROM_TIME;
            DataObject.RTO_EMP = this.RTO_EMP;
            DataObject.RTO_TIME = this.RTO_TIME;
            DataObject.DFROM_EMP = this.DFROM_EMP;
            DataObject.DFROM_TIME = this.DFROM_TIME;
            DataObject.DTO_EMP = this.DTO_EMP;
            DataObject.DTO_TIME = this.DTO_TIME;
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
        public string QSN
        {
            get
            {
                return (string)this["QSN"];
            }
            set
            {
                this["QSN"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public DateTime? SCAN_DATE
        {
            get
            {
                return (DateTime?)this["SCAN_DATE"];
            }
            set
            {
                this["SCAN_DATE"] = value;
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
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
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
        public string PASS_FLAG
        {
            get
            {
                return (string)this["PASS_FLAG"];
            }
            set
            {
                this["PASS_FLAG"] = value;
            }
        }
        public string FAIL_FLAG
        {
            get
            {
                return (string)this["FAIL_FLAG"];
            }
            set
            {
                this["FAIL_FLAG"] = value;
            }
        }
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }
        public string RFROM_EMP
        {
            get
            {
                return (string)this["RFROM_EMP"];
            }
            set
            {
                this["RFROM_EMP"] = value;
            }
        }
        public DateTime? RFROM_TIME
        {
            get
            {
                return (DateTime?)this["RFROM_TIME"];
            }
            set
            {
                this["RFROM_TIME"] = value;
            }
        }
        public string RTO_EMP
        {
            get
            {
                return (string)this["RTO_EMP"];
            }
            set
            {
                this["RTO_EMP"] = value;
            }
        }
        public DateTime? RTO_TIME
        {
            get
            {
                return (DateTime?)this["RTO_TIME"];
            }
            set
            {
                this["RTO_TIME"] = value;
            }
        }
        public string DFROM_EMP
        {
            get
            {
                return (string)this["DFROM_EMP"];
            }
            set
            {
                this["DFROM_EMP"] = value;
            }
        }
        public DateTime? DFROM_TIME
        {
            get
            {
                return (DateTime?)this["DFROM_TIME"];
            }
            set
            {
                this["DFROM_TIME"] = value;
            }
        }
        public string DTO_EMP
        {
            get
            {
                return (string)this["DTO_EMP"];
            }
            set
            {
                this["DTO_EMP"] = value;
            }
        }
        public DateTime? DTO_TIME
        {
            get
            {
                return (DateTime?)this["DTO_TIME"];
            }
            set
            {
                this["DTO_TIME"] = value;
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
    public class R_QUACK_SN_STATION_DETAIL
    {
        public string ID { get; set; }
        public string QSN { get; set; }
        public string STATION_NAME { get; set; }
        public DateTime? SCAN_DATE { get; set; }
        public string SKUNO { get; set; }
        public string PLANT { get; set; }
        public string LINE { get; set; }
        public string CLASS_NAME { get; set; }
        public string PRODUCT_STATUS { get; set; }
        public string PASS_FLAG { get; set; }
        public string FAIL_FLAG { get; set; }
        public string LOT_NO { get; set; }
        public string RFROM_EMP { get; set; }
        public DateTime? RFROM_TIME { get; set; }
        public string RTO_EMP { get; set; }
        public DateTime? RTO_TIME { get; set; }
        public string DFROM_EMP { get; set; }
        public DateTime? DFROM_TIME { get; set; }
        public string DTO_EMP { get; set; }
        public DateTime? DTO_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}