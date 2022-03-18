using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MDS_YIELD : DataObjectTable
    {
        public T_R_MDS_YIELD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MDS_YIELD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MDS_YIELD);
            TableName = "R_MDS_YIELD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MDS_YIELD : DataObjectBase
    {
        public Row_R_MDS_YIELD(DataObjectInfo info) : base(info)
        {

        }
        public R_MDS_YIELD GetDataObject()
        {
            R_MDS_YIELD DataObject = new R_MDS_YIELD();
            DataObject.ID = this.ID;
            DataObject.DATAPOINT = this.DATAPOINT;
            DataObject.RECORD_CREATION_DATE = this.RECORD_CREATION_DATE;
            DataObject.CM_CODE = this.CM_CODE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.EVENTPOINT = this.EVENTPOINT;
            DataObject.YIELD_DATE = this.YIELD_DATE;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.RECORDTYPE = this.RECORDTYPE;
            DataObject.WORKORDERTYPE = this.WORKORDERTYPE;
            DataObject.SCANBY = this.SCANBY;
            DataObject.FAILCODE = this.FAILCODE;
            DataObject.COMMENTS = this.COMMENTS;
            DataObject.FAILLOCATION = this.FAILLOCATION;
            DataObject.FAILPARTNO = this.FAILPARTNO;
            DataObject.FAILSERIALNO = this.FAILSERIALNO;
            DataObject.REPLACSERIALNO = this.REPLACSERIALNO;
            DataObject.CORRENTTYPE = this.CORRENTTYPE;
            DataObject.ROUTEID = this.ROUTEID;
            DataObject.ROUTEREV = this.ROUTEREV;
            DataObject.ATTRIBUTE1 = this.ATTRIBUTE1;
            DataObject.ATTRIBUTE2 = this.ATTRIBUTE2;
            DataObject.ATTRIBUTE3 = this.ATTRIBUTE3;
            DataObject.ATTRIBUTE4 = this.ATTRIBUTE4;
            DataObject.FAULT_CONFIRMED_FLAG = this.FAULT_CONFIRMED_FLAG;
            DataObject.FIRMWARE_VERSION = this.FIRMWARE_VERSION;
            DataObject.UPTIME = this.UPTIME;
            DataObject.FRU_SERIAL_NUMBER = this.FRU_SERIAL_NUMBER;
            DataObject.HOLD_FLAG = this.HOLD_FLAG;
            DataObject.FA_REQUIRED_FLAG = this.FA_REQUIRED_FLAG;
            DataObject.CUST_TAG = this.CUST_TAG;
            DataObject.CHG_PART_NUM = this.CHG_PART_NUM;
            DataObject.REF1 = this.REF1;
            DataObject.REF2 = this.REF2;
            DataObject.REF3 = this.REF3;
            DataObject.REF4 = this.REF4;
            DataObject.REF5 = this.REF5;
            DataObject.HEADID = this.HEADID;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
        }
        public string HEADID
        {
            get
            {
                return (string)this["HEADID"];
            }
            set
            {
                this["HEADID"] = value;
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
        public string DATAPOINT
        {
            get
            {
                return (string)this["DATAPOINT"];
            }
            set
            {
                this["DATAPOINT"] = value;
            }
        }
        public string RECORD_CREATION_DATE
        {
            get
            {
                return (string)this["RECORD_CREATION_DATE"];
            }
            set
            {
                this["RECORD_CREATION_DATE"] = value;
            }
        }
        public string CM_CODE
        {
            get
            {
                return (string)this["CM_CODE"];
            }
            set
            {
                this["CM_CODE"] = value;
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
        public string EVENTPOINT
        {
            get
            {
                return (string)this["EVENTPOINT"];
            }
            set
            {
                this["EVENTPOINT"] = value;
            }
        }
        public string YIELD_DATE
        {
            get
            {
                return (string)this["YIELD_DATE"];
            }
            set
            {
                this["YIELD_DATE"] = value;
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
        public string RECORDTYPE
        {
            get
            {
                return (string)this["RECORDTYPE"];
            }
            set
            {
                this["RECORDTYPE"] = value;
            }
        }
        public string WORKORDERTYPE
        {
            get
            {
                return (string)this["WORKORDERTYPE"];
            }
            set
            {
                this["WORKORDERTYPE"] = value;
            }
        }
        public string SCANBY
        {
            get
            {
                return (string)this["SCANBY"];
            }
            set
            {
                this["SCANBY"] = value;
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
        public string COMMENTS
        {
            get
            {
                return (string)this["COMMENTS"];
            }
            set
            {
                this["COMMENTS"] = value;
            }
        }
        public string FAILLOCATION
        {
            get
            {
                return (string)this["FAILLOCATION"];
            }
            set
            {
                this["FAILLOCATION"] = value;
            }
        }
        public string FAILPARTNO
        {
            get
            {
                return (string)this["FAILPARTNO"];
            }
            set
            {
                this["FAILPARTNO"] = value;
            }
        }
        public string FAILSERIALNO
        {
            get
            {
                return (string)this["FAILSERIALNO"];
            }
            set
            {
                this["FAILSERIALNO"] = value;
            }
        }
        public string REPLACSERIALNO
        {
            get
            {
                return (string)this["REPLACSERIALNO"];
            }
            set
            {
                this["REPLACSERIALNO"] = value;
            }
        }
        public string CORRENTTYPE
        {
            get
            {
                return (string)this["CORRENTTYPE"];
            }
            set
            {
                this["CORRENTTYPE"] = value;
            }
        }
        public string ROUTEID
        {
            get
            {
                return (string)this["ROUTEID"];
            }
            set
            {
                this["ROUTEID"] = value;
            }
        }
        public string ROUTEREV
        {
            get
            {
                return (string)this["ROUTEREV"];
            }
            set
            {
                this["ROUTEREV"] = value;
            }
        }
        public string ATTRIBUTE1
        {
            get
            {
                return (string)this["ATTRIBUTE1"];
            }
            set
            {
                this["ATTRIBUTE1"] = value;
            }
        }
        public string ATTRIBUTE2
        {
            get
            {
                return (string)this["ATTRIBUTE2"];
            }
            set
            {
                this["ATTRIBUTE2"] = value;
            }
        }
        public string ATTRIBUTE3
        {
            get
            {
                return (string)this["ATTRIBUTE3"];
            }
            set
            {
                this["ATTRIBUTE3"] = value;
            }
        }
        public string ATTRIBUTE4
        {
            get
            {
                return (string)this["ATTRIBUTE4"];
            }
            set
            {
                this["ATTRIBUTE4"] = value;
            }
        }
        public string FAULT_CONFIRMED_FLAG
        {
            get
            {
                return (string)this["FAULT_CONFIRMED_FLAG"];
            }
            set
            {
                this["FAULT_CONFIRMED_FLAG"] = value;
            }
        }
        public string FIRMWARE_VERSION
        {
            get
            {
                return (string)this["FIRMWARE_VERSION"];
            }
            set
            {
                this["FIRMWARE_VERSION"] = value;
            }
        }
        public string UPTIME
        {
            get
            {
                return (string)this["UPTIME"];
            }
            set
            {
                this["UPTIME"] = value;
            }
        }
        public string FRU_SERIAL_NUMBER
        {
            get
            {
                return (string)this["FRU_SERIAL_NUMBER"];
            }
            set
            {
                this["FRU_SERIAL_NUMBER"] = value;
            }
        }
        public string HOLD_FLAG
        {
            get
            {
                return (string)this["HOLD_FLAG"];
            }
            set
            {
                this["HOLD_FLAG"] = value;
            }
        }
        public string FA_REQUIRED_FLAG
        {
            get
            {
                return (string)this["FA_REQUIRED_FLAG"];
            }
            set
            {
                this["FA_REQUIRED_FLAG"] = value;
            }
        }
        public string CUST_TAG
        {
            get
            {
                return (string)this["CUST_TAG"];
            }
            set
            {
                this["CUST_TAG"] = value;
            }
        }
        public string CHG_PART_NUM
        {
            get
            {
                return (string)this["CHG_PART_NUM"];
            }
            set
            {
                this["CHG_PART_NUM"] = value;
            }
        }
        public string REF1
        {
            get
            {
                return (string)this["REF1"];
            }
            set
            {
                this["REF1"] = value;
            }
        }
        public string REF2
        {
            get
            {
                return (string)this["REF2"];
            }
            set
            {
                this["REF2"] = value;
            }
        }
        public string REF3
        {
            get
            {
                return (string)this["REF3"];
            }
            set
            {
                this["REF3"] = value;
            }
        }
        public string REF4
        {
            get
            {
                return (string)this["REF4"];
            }
            set
            {
                this["REF4"] = value;
            }
        }
        public string REF5
        {
            get
            {
                return (string)this["REF5"];
            }
            set
            {
                this["REF5"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
    }
    public class R_MDS_YIELD
    {
        public string ID { get; set; }
        public string DATAPOINT { get; set; }
        public string RECORD_CREATION_DATE { get; set; }
        public string CM_CODE { get; set; }
        public string SKUNO { get; set; }
        public string SYSSERIALNO { get; set; }
        public string EVENTPOINT { get; set; }
        public string YIELD_DATE { get; set; }
        public string WORKORDERNO { get; set; }
        public string RECORDTYPE { get; set; }
        public string WORKORDERTYPE { get; set; }
        public string SCANBY { get; set; }
        public string FAILCODE { get; set; }
        public string COMMENTS { get; set; }
        public string FAILLOCATION { get; set; }
        public string FAILPARTNO { get; set; }
        public string FAILSERIALNO { get; set; }
        public string REPLACSERIALNO { get; set; }
        public string CORRENTTYPE { get; set; }
        public string ROUTEID { get; set; }
        public string ROUTEREV { get; set; }
        public string ATTRIBUTE1 { get; set; }
        public string ATTRIBUTE2 { get; set; }
        public string ATTRIBUTE3 { get; set; }
        public string ATTRIBUTE4 { get; set; }
        public string FAULT_CONFIRMED_FLAG { get; set; }
        public string FIRMWARE_VERSION { get; set; }
        public string UPTIME { get; set; }
        public string FRU_SERIAL_NUMBER { get; set; }
        public string HOLD_FLAG { get; set; }
        public string FA_REQUIRED_FLAG { get; set; }
        public string CUST_TAG { get; set; }
        public string CHG_PART_NUM { get; set; }
        public string REF1 { get; set; }
        public string REF2 { get; set; }
        public string REF3 { get; set; }
        public string REF4 { get; set; }
        public string REF5 { get; set; }
        public string HEADID { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}