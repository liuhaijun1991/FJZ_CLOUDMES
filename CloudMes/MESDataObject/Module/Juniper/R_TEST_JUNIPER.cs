using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_TEST_JUNIPER : DataObjectTable
    {
        public T_R_TEST_JUNIPER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_JUNIPER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_JUNIPER);
            TableName = "R_TEST_JUNIPER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_TEST_JUNIPER> GetTestJuniperBySN(OleExec sfcdb, string sn)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_TEST_JUNIPER row_main = null;
            List<R_TEST_JUNIPER> mains = new List<R_TEST_JUNIPER>();
            string sql = $@"select * from {TableName} where SYSSERIALNO = '{sn.Replace("'", "''")}' ";

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_R_TEST_JUNIPER)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public DataTable GetRowTestJuniper(OleExec sfcdb, string sn)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            string sql = $@"select * from {TableName} where SYSSERIALNO = '{sn.Replace("'", "''")}' and upper(STATUS) = 'FAIL' and rownum = 1 order by testdate desc ";

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return dt;
        }
    }
    public class Row_R_TEST_JUNIPER : DataObjectBase
    {
        public Row_R_TEST_JUNIPER(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_JUNIPER GetDataObject()
        {
            R_TEST_JUNIPER DataObject = new R_TEST_JUNIPER();
            DataObject.ID = this.ID;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.TESTDATE = this.TESTDATE;
            DataObject.EVENTNAME = this.EVENTNAME;
            DataObject.STATUS = this.STATUS;
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
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.TESTERNO = this.TESTERNO;
            DataObject.TATIME = this.TATIME;
            DataObject.R_TEST_RECORD_ID = this.R_TEST_RECORD_ID;
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
        public string SYSSERIALNO
        {
            get
            {
                return (string)this["SYSSERIALNO"];
            }
            set
            {
                this["SYSSERIALNO"] = value;
            }
        }
        public DateTime? TESTDATE
        {
            get
            {
                return (DateTime?)this["TESTDATE"];
            }
            set
            {
                this["TESTDATE"] = value;
            }
        }
        public string EVENTNAME
        {
            get
            {
                return (string)this["EVENTNAME"];
            }
            set
            {
                this["EVENTNAME"] = value;
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
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
        public string TESTERNO
        {
            get
            {
                return (string)this["TESTERNO"];
            }
            set
            {
                this["TESTERNO"] = value;
            }
        }
        public DateTime? TATIME
        {
            get
            {
                return (DateTime?)this["TATIME"];
            }
            set
            {
                this["TATIME"] = value;
            }
        }
        public string R_TEST_RECORD_ID
        {
            get
            {
                return (string)this["R_TEST_RECORD_ID"];
            }
            set
            {
                this["R_TEST_RECORD_ID"] = value;
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
    public class R_TEST_JUNIPER
    {
        public string ID { get; set; }
        public string SYSSERIALNO { get; set; }
        public DateTime? TESTDATE { get; set; }
        public string EVENTNAME { get; set; }
        public string STATUS { get; set; }
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
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
        public string TESTERNO { get; set; }
        public DateTime? TATIME { get; set; }
        public string R_TEST_RECORD_ID { get; set; }
        public string MFG_TEST_LOG { get; set; }
    }
}