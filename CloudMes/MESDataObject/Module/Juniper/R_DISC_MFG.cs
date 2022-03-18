using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module.Juniper
{
    public class T_R_DISC_MFG : DataObjectTable
    {
        public T_R_DISC_MFG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DISC_MFG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DISC_MFG);
            TableName = "R_DISC_MFG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_DISC_MFG : DataObjectBase
    {
        public Row_R_DISC_MFG(DataObjectInfo info) : base(info)
        {

        }
        public R_DISC_MFG GetDataObject()
        {
            R_DISC_MFG DataObject = new R_DISC_MFG();
            DataObject.ID = this.ID;
            DataObject.SUPPLIER = this.SUPPLIER;
            DataObject.SUPPLIER_SITE = this.SUPPLIER_SITE;
            DataObject.PART_NUMBER = this.PART_NUMBER;
            DataObject.SERIAL_NUMBER = this.SERIAL_NUMBER;
            DataObject.PART_NUMBER_REVISION = this.PART_NUMBER_REVISION;
            DataObject.SHOP_FLOOR_ORDER_NUMBER = this.SHOP_FLOOR_ORDER_NUMBER;
            DataObject.ROUTING_STEP_NUMBER = this.ROUTING_STEP_NUMBER;
            DataObject.WORK_STATION = this.WORK_STATION;
            DataObject.WORK_STATION_DESCRIPTION = this.WORK_STATION_DESCRIPTION;
            DataObject.START_DATE_TIME = this.START_DATE_TIME;
            DataObject.END_DATE_TIME = this.END_DATE_TIME;
            DataObject.LOAD_DATE = this.LOAD_DATE;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.SENT_FLAG = this.SENT_FLAG;
            DataObject.SENT_TIME = this.SENT_TIME;
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
        public string SUPPLIER
        {
            get
            {
                return (string)this["SUPPLIER"];
            }
            set
            {
                this["SUPPLIER"] = value;
            }
        }
        public string SUPPLIER_SITE
        {
            get
            {
                return (string)this["SUPPLIER_SITE"];
            }
            set
            {
                this["SUPPLIER_SITE"] = value;
            }
        }
        public string PART_NUMBER
        {
            get
            {
                return (string)this["PART_NUMBER"];
            }
            set
            {
                this["PART_NUMBER"] = value;
            }
        }
        public string SERIAL_NUMBER
        {
            get
            {
                return (string)this["SERIAL_NUMBER"];
            }
            set
            {
                this["SERIAL_NUMBER"] = value;
            }
        }
        public string PART_NUMBER_REVISION
        {
            get
            {
                return (string)this["PART_NUMBER_REVISION"];
            }
            set
            {
                this["PART_NUMBER_REVISION"] = value;
            }
        }
        public string SHOP_FLOOR_ORDER_NUMBER
        {
            get
            {
                return (string)this["SHOP_FLOOR_ORDER_NUMBER"];
            }
            set
            {
                this["SHOP_FLOOR_ORDER_NUMBER"] = value;
            }
        }
        public string ROUTING_STEP_NUMBER
        {
            get
            {
                return (string)this["ROUTING_STEP_NUMBER"];
            }
            set
            {
                this["ROUTING_STEP_NUMBER"] = value;
            }
        }
        public string WORK_STATION
        {
            get
            {
                return (string)this["WORK_STATION"];
            }
            set
            {
                this["WORK_STATION"] = value;
            }
        }
        public string WORK_STATION_DESCRIPTION
        {
            get
            {
                return (string)this["WORK_STATION_DESCRIPTION"];
            }
            set
            {
                this["WORK_STATION_DESCRIPTION"] = value;
            }
        }
        public DateTime? START_DATE_TIME
        {
            get
            {
                return (DateTime?)this["START_DATE_TIME"];
            }
            set
            {
                this["START_DATE_TIME"] = value;
            }
        }
        public DateTime? END_DATE_TIME
        {
            get
            {
                return (DateTime?)this["END_DATE_TIME"];
            }
            set
            {
                this["END_DATE_TIME"] = value;
            }
        }
        public DateTime? LOAD_DATE
        {
            get
            {
                return (DateTime?)this["LOAD_DATE"];
            }
            set
            {
                this["LOAD_DATE"] = value;
            }
        }
        public string FILE_NAME
        {
            get
            {
                return (string)this["FILE_NAME"];
            }
            set
            {
                this["FILE_NAME"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public double? SENT_FLAG
        {
            get
            {
                return (double?)this["SENT_FLAG"];
            }
            set
            {
                this["SENT_FLAG"] = value;
            }
        }
        public DateTime? SENT_TIME
        {
            get
            {
                return (DateTime?)this["SENT_TIME"];
            }
            set
            {
                this["SENT_TIME"] = value;
            }
        }
    }
    public class R_DISC_MFG
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SUPPLIER { get; set; }
        public string SUPPLIER_SITE { get; set; }
        public string PART_NUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string PART_NUMBER_REVISION { get; set; }
        public string SHOP_FLOOR_ORDER_NUMBER { get; set; }
        public string ROUTING_STEP_NUMBER { get; set; }
        public string WORK_STATION { get; set; }
        public string WORK_STATION_DESCRIPTION { get; set; }
        public DateTime? START_DATE_TIME { get; set; }
        public DateTime? END_DATE_TIME { get; set; }
        public DateTime? LOAD_DATE { get; set; }
        public string FILE_NAME { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public double? SENT_FLAG { get; set; }
        public DateTime? SENT_TIME { get; set; }
        public string HEAD_ID { get; set; }
        public double? VALID_FLAG { get; set; }
    }
}