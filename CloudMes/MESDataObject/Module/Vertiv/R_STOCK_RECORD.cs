using System;
using System.Collections.Generic;
using System.Data;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_STOCK_RECORD : DataObjectTable
    {
        public T_R_STOCK_RECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_STOCK_RECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_STOCK_RECORD);
            TableName = "R_STOCK_RECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int AddNewStockRecord(string Pallet, string Location, string SkuNo, string Wo, string StationName, string Bu, string EmpNo, OleExec DB)
        {
            Row_R_STOCK_RECORD row_Record = (Row_R_STOCK_RECORD)NewRow();
            row_Record.ID = GetNewID(Bu, DB);
            row_Record.LOCATION = Location;
            row_Record.VALUE = Pallet;
            row_Record.SKUNO = SkuNo;
            row_Record.WORKORDERNO = Wo;
            row_Record.STATION = StationName;
            row_Record.STATUS = "1";
            row_Record.EDIT_TIME = GetDBDateTime(DB);
            row_Record.EDIT_EMP = EmpNo;
            int result = DB.ExecuteNonQuery(row_Record.GetInsertString(DBType), CommandType.Text);
            return result;
        }
    }
    public class Row_R_STOCK_RECORD : DataObjectBase
    {
        public Row_R_STOCK_RECORD(DataObjectInfo info) : base(info)
        {

        }
        public R_STOCK_RECORD GetDataObject()
        {
            R_STOCK_RECORD DataObject = new R_STOCK_RECORD();
            DataObject.ID = this.ID;
            DataObject.LOCATION = this.LOCATION;
            DataObject.VALUE = this.VALUE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.STATION = this.STATION;
            DataObject.CHECKIN = this.CHECKIN;
            DataObject.CHECKINBY = this.CHECKINBY;
            DataObject.CHECKOUT = this.CHECKOUT;
            DataObject.CHECKOUTBY = this.CHECKOUTBY;
            DataObject.STATUS = this.STATUS;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
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
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
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
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public DateTime? CHECKIN
        {
            get
            {
                return (DateTime?)this["CHECKIN"];
            }
            set
            {
                this["CHECKIN"] = value;
            }
        }
        public string CHECKINBY
        {
            get
            {
                return (string)this["CHECKINBY"];
            }
            set
            {
                this["CHECKINBY"] = value;
            }
        }
        public DateTime? CHECKOUT
        {
            get
            {
                return (DateTime?)this["CHECKOUT"];
            }
            set
            {
                this["CHECKOUT"] = value;
            }
        }
        public string CHECKOUTBY
        {
            get
            {
                return (string)this["CHECKOUTBY"];
            }
            set
            {
                this["CHECKOUTBY"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
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
    public class R_STOCK_RECORD
    {
        public string ID { get; set; }
        public string LOCATION { get; set; }
        public string VALUE { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string STATION { get; set; }
        public DateTime? CHECKIN { get; set; }
        public string CHECKINBY { get; set; }
        public DateTime? CHECKOUT { get; set; }
        public string CHECKOUTBY { get; set; }
        public string STATUS { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}
