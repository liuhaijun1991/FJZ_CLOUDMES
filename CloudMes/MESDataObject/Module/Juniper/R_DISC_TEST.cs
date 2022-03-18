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
    public class T_R_DISC_TEST : DataObjectTable
    {
        public T_R_DISC_TEST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DISC_TEST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DISC_TEST);
            TableName = "R_DISC_TEST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_DISC_TEST : DataObjectBase
    {
        public Row_R_DISC_TEST(DataObjectInfo info) : base(info)
        {

        }
        public R_DISC_TEST GetDataObject()
        {
            R_DISC_TEST DataObject = new R_DISC_TEST();
            DataObject.ID = this.ID;
            DataObject.SUPPLIER = this.SUPPLIER;
            DataObject.SUPPLIER_SITE = this.SUPPLIER_SITE;
            DataObject.PART_NUMBER = this.PART_NUMBER;
            DataObject.CM_ODM_PARTNUMBER = this.CM_ODM_PARTNUMBER;
            DataObject.SERIAL_NUMBER = this.SERIAL_NUMBER;
            DataObject.PHASE = this.PHASE;
            DataObject.PART_NUMBER_REVISION = this.PART_NUMBER_REVISION;
            DataObject.UNIQUE_TEST_ID = this.UNIQUE_TEST_ID;
            DataObject.TEST_START_TIMESTAMP = this.TEST_START_TIMESTAMP;
            DataObject.TEST_STEP = this.TEST_STEP;
            DataObject.TEST_CYCLE_TEST_LOOP = this.TEST_CYCLE_TEST_LOOP;
            DataObject.CAPTURE_TIME = this.CAPTURE_TIME;
            DataObject.TEST_RESULT = this.TEST_RESULT;
            DataObject.FAILCODE = this.FAILCODE;
            DataObject.TEST_STATION_NUMBER = this.TEST_STATION_NUMBER;
            DataObject.TEST_STATION_NAME = this.TEST_STATION_NAME;
            DataObject.LOAD_DATE = this.LOAD_DATE;
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.SENT_FLAG = this.SENT_FLAG;
            DataObject.SENT_TIME = this.SENT_TIME;
            DataObject.HEAD_ID = this.HEAD_ID;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.MFG_TEST_LOG = this.MFG_TEST_LOG;
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
        public string CM_ODM_PARTNUMBER
        {
            get
            {
                return (string)this["CM_ODM_PARTNUMBER"];
            }
            set
            {
                this["CM_ODM_PARTNUMBER"] = value;
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
        public string PHASE
        {
            get
            {
                return (string)this["PHASE"];
            }
            set
            {
                this["PHASE"] = value;
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
        public string UNIQUE_TEST_ID
        {
            get
            {
                return (string)this["UNIQUE_TEST_ID"];
            }
            set
            {
                this["UNIQUE_TEST_ID"] = value;
            }
        }
        public string TEST_START_TIMESTAMP
        {
            get
            {
                return (string)this["TEST_START_TIMESTAMP"];
            }
            set
            {
                this["TEST_START_TIMESTAMP"] = value;
            }
        }
        public string TEST_STEP
        {
            get
            {
                return (string)this["TEST_STEP"];
            }
            set
            {
                this["TEST_STEP"] = value;
            }
        }
        public string TEST_CYCLE_TEST_LOOP
        {
            get
            {
                return (string)this["TEST_CYCLE_TEST_LOOP"];
            }
            set
            {
                this["TEST_CYCLE_TEST_LOOP"] = value;
            }
        }
        public DateTime? CAPTURE_TIME
        {
            get
            {
                return (DateTime?)this["CAPTURE_TIME"];
            }
            set
            {
                this["CAPTURE_TIME"] = value;
            }
        }
        public string TEST_RESULT
        {
            get
            {
                return (string)this["TEST_RESULT"];
            }
            set
            {
                this["TEST_RESULT"] = value;
            }
        }
        public string FAILCODE
        {
            get
            {
                return (string)this["FAILCODE"];
            }
            set
            {
                this["FAILCODE"] = value;
            }
        }
        public string TEST_STATION_NUMBER
        {
            get
            {
                return (string)this["TEST_STATION_NUMBER"];
            }
            set
            {
                this["TEST_STATION_NUMBER"] = value;
            }
        }
        public string TEST_STATION_NAME
        {
            get
            {
                return (string)this["TEST_STATION_NAME"];
            }
            set
            {
                this["TEST_STATION_NAME"] = value;
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
        public string HEAD_ID
        {
            get
            {
                return (string)this["HEAD_ID"];
            }
            set
            {
                this["HEAD_ID"] = value;
            }
        }
        public double? VALID_FLAG
        {
            get
            {
                return (double?)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string MFG_TEST_LOG
        {
            get
            {
                return (string)this["MFG_TEST_LOG"];
            }
            set
            {
                this["MFG_TEST_LOG"] = value;
            }
        }
    }
    public class R_DISC_TEST
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SUPPLIER { get; set; }
        public string SUPPLIER_SITE { get; set; }
        public string PART_NUMBER { get; set; }
        public string CM_ODM_PARTNUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string PHASE { get; set; }
        public string PART_NUMBER_REVISION { get; set; }
        public string UNIQUE_TEST_ID { get; set; }
        public string TEST_START_TIMESTAMP { get; set; }
        public string TEST_STEP { get; set; }
        public string TEST_CYCLE_TEST_LOOP { get; set; }
        public DateTime? CAPTURE_TIME { get; set; }
        public string TEST_RESULT { get; set; }
        public string FAILCODE { get; set; }
        public string TEST_STATION_NUMBER { get; set; }
        public string TEST_STATION_NAME { get; set; }
        public DateTime? LOAD_DATE { get; set; }
        public string FILE_NAME { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public double? SENT_FLAG { get; set; }
        public DateTime? SENT_TIME { get; set; }
        public string HEAD_ID { get; set; }
        public double? VALID_FLAG { get; set; }
        public string MFG_TEST_LOG { get; set; }//disc test data need this cloumn 2022-01-13
    }
}