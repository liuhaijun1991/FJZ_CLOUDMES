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
    public class T_R_DISC_REPAIR : DataObjectTable
    {
        public T_R_DISC_REPAIR(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DISC_REPAIR(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DISC_REPAIR);
            TableName = "R_DISC_REPAIR".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_DISC_REPAIR : DataObjectBase
    {
        public Row_R_DISC_REPAIR(DataObjectInfo info) : base(info)
        {

        }
        public R_DISC_REPAIR GetDataObject()
        {
            R_DISC_REPAIR DataObject = new R_DISC_REPAIR();
            DataObject.SENT_TIME = this.SENT_TIME;
            DataObject.ID = this.ID;
            DataObject.SUPPLIER = this.SUPPLIER;
            DataObject.SUPPLIER_SITE = this.SUPPLIER_SITE;
            DataObject.PART_NUMBER = this.PART_NUMBER;
            DataObject.CM_ODM_PARTNUMBER = this.CM_ODM_PARTNUMBER;
            DataObject.SERIAL_NUMBER = this.SERIAL_NUMBER;
            DataObject.PART_NUMBER_REVISION = this.PART_NUMBER_REVISION;
            DataObject.UNIQUE_TEST_ID = this.UNIQUE_TEST_ID;
            DataObject.TEST_START_TIMESTAMP = this.TEST_START_TIMESTAMP;
            DataObject.TEST_STEP = this.TEST_STEP;
            DataObject.TEST_CYCLE_TEST_LOOP = this.TEST_CYCLE_TEST_LOOP;
            DataObject.DEFECT_CLASSIFICATION = this.DEFECT_CLASSIFICATION;
            DataObject.LOCATION = this.LOCATION;
            DataObject.COMPONENT_PART_NUMBER = this.COMPONENT_PART_NUMBER;
            DataObject.CM_ODM_COMPONENT_ID = this.CM_ODM_COMPONENT_ID;
            DataObject.DEFECT_TYPE = this.DEFECT_TYPE;
            DataObject.DEFECT_DESCRIPTION = this.DEFECT_DESCRIPTION;
            DataObject.REPAIR_STATION = this.REPAIR_STATION;
            DataObject.REPAIR_STATION_NAME = this.REPAIR_STATION_NAME;
            DataObject.REPAIR_STATUS = this.REPAIR_STATUS;
            DataObject.REPAIR_COMMENTS = this.REPAIR_COMMENTS;
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
        public string DEFECT_CLASSIFICATION
        {
            get
            {
                return (string)this["DEFECT_CLASSIFICATION"];
            }
            set
            {
                this["DEFECT_CLASSIFICATION"] = value;
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
        public string COMPONENT_PART_NUMBER
        {
            get
            {
                return (string)this["COMPONENT_PART_NUMBER"];
            }
            set
            {
                this["COMPONENT_PART_NUMBER"] = value;
            }
        }
        public string CM_ODM_COMPONENT_ID
        {
            get
            {
                return (string)this["CM_ODM_COMPONENT_ID"];
            }
            set
            {
                this["CM_ODM_COMPONENT_ID"] = value;
            }
        }
        public string DEFECT_TYPE
        {
            get
            {
                return (string)this["DEFECT_TYPE"];
            }
            set
            {
                this["DEFECT_TYPE"] = value;
            }
        }
        public string DEFECT_DESCRIPTION
        {
            get
            {
                return (string)this["DEFECT_DESCRIPTION"];
            }
            set
            {
                this["DEFECT_DESCRIPTION"] = value;
            }
        }
        public string REPAIR_STATION
        {
            get
            {
                return (string)this["REPAIR_STATION"];
            }
            set
            {
                this["REPAIR_STATION"] = value;
            }
        }
        public string REPAIR_STATION_NAME
        {
            get
            {
                return (string)this["REPAIR_STATION_NAME"];
            }
            set
            {
                this["REPAIR_STATION_NAME"] = value;
            }
        }
        public string REPAIR_STATUS
        {
            get
            {
                return (string)this["REPAIR_STATUS"];
            }
            set
            {
                this["REPAIR_STATUS"] = value;
            }
        }
        public string REPAIR_COMMENTS
        {
            get
            {
                return (string)this["REPAIR_COMMENTS"];
            }
            set
            {
                this["REPAIR_COMMENTS"] = value;
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
    public class R_DISC_REPAIR
    {     
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SUPPLIER { get; set; }
        public string SUPPLIER_SITE { get; set; }
        public string PART_NUMBER { get; set; }
        public string CM_ODM_PARTNUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string PART_NUMBER_REVISION { get; set; }
        public string UNIQUE_TEST_ID { get; set; }
        public string TEST_START_TIMESTAMP { get; set; }
        public string TEST_STEP { get; set; }
        public string TEST_CYCLE_TEST_LOOP { get; set; }
        public string DEFECT_CLASSIFICATION { get; set; }
        public string LOCATION { get; set; }
        public string COMPONENT_PART_NUMBER { get; set; }
        public string CM_ODM_COMPONENT_ID { get; set; }
        public string DEFECT_TYPE { get; set; }
        public string DEFECT_DESCRIPTION { get; set; }
        public string REPAIR_STATION { get; set; }
        public string REPAIR_STATION_NAME { get; set; }
        public string REPAIR_STATUS { get; set; }
        public string REPAIR_COMMENTS { get; set; }
        public DateTime? LOAD_DATE { get; set; }
        public string FILE_NAME { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public double? SENT_FLAG { get; set; }
        public DateTime? SENT_TIME { get; set; }
        public string HEAD_ID { get; set; }
        public double? VALID_FLAG { get; set; }
        public string MFG_TEST_LOG { get; set; }//disc repair data need this cloumn 2022-01-24
    }
}