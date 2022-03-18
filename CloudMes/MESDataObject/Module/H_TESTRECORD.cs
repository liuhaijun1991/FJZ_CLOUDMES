using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_TESTRECORD : DataObjectTable
    {
        public T_H_TESTRECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_TESTRECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_TESTRECORD);
            TableName = "H_TESTRECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Station"></param>
        /// <param name="Status"></param>
        /// <param name="TESTDATE"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<H_TESTRECORD> GetListBYSNStationStatus(string SN, List<string> Station, string Status, DateTime? TESTDATE, OleExec DB)
        {
            return DB.ORM.Queryable<H_TESTRECORD>().Where(t => t.SN == SN && Station.Contains(t.STATION_NAME) && t.STATUS == Status && t.TEST_TIME > TESTDATE).ToList();
        }
    }
    public class Row_H_TESTRECORD : DataObjectBase
    {
        public Row_H_TESTRECORD(DataObjectInfo info) : base(info)
        {

        }
        public H_TESTRECORD GetDataObject()
        {
            H_TESTRECORD DataObject = new H_TESTRECORD();
            DataObject.ID = this.ID;
            DataObject.SN_ID = this.SN_ID;
            DataObject.SN = this.SN;
            DataObject.TEST_TIME = this.TEST_TIME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.MES_STATION = this.MES_STATION;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.STATUS = this.STATUS;
            DataObject.REPAIR_STATUS = this.REPAIR_STATUS;
            DataObject.TEST_INFO = this.TEST_INFO;
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
        public string SN_ID
        {
            get
            {
                return (string)this["SN_ID"];
            }
            set
            {
                this["SN_ID"] = value;
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
        public DateTime? TEST_TIME
        {
            get
            {
                return (DateTime?)this["TEST_TIME"];
            }
            set
            {
                this["TEST_TIME"] = value;
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
        public string MES_STATION
        {
            get
            {
                return (string)this["MES_STATION"];
            }
            set
            {
                this["MES_STATION"] = value;
            }
        }
        public string CUSTPARTNO
        {
            get
            {
                return (string)this["CUSTPARTNO"];
            }
            set
            {
                this["CUSTPARTNO"] = value;
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
        public string TEST_INFO
        {
            get
            {
                return (string)this["TEST_INFO"];
            }
            set
            {
                this["TEST_INFO"] = value;
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
    public class H_TESTRECORD
    {
        public string ID { get; set; }
        public string SN_ID { get; set; }
        public string SN { get; set; }
        public DateTime? TEST_TIME { get; set; }
        public string STATION_NAME { get; set; }
        public string MES_STATION { get; set; }
        public string CUSTPARTNO { get; set; }
        public string STATUS { get; set; }
        public string REPAIR_STATUS { get; set; }
        public string TEST_INFO { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}