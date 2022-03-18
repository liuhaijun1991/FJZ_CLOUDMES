using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_ICT_HEADER : DataObjectTable
    {
        public T_ICT_HEADER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_ICT_HEADER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_ICT_HEADER);
            TableName = "ICT_HEADER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_ICT_HEADER : DataObjectBase
    {
        public Row_ICT_HEADER(DataObjectInfo info) : base(info)
        {

        }
        public ICT_HEADER GetDataObject()
        {
            ICT_HEADER DataObject = new ICT_HEADER();
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.ICTDATETIME = this.ICTDATETIME;
            DataObject.MODEL = this.MODEL;
            DataObject.ICT_NO = this.ICT_NO;
            DataObject.ICTDATE = this.ICTDATE;
            DataObject.ICTTIME = this.ICTTIME;
            DataObject.RESULT = this.RESULT;
            DataObject.RETEST = this.RETEST;
            DataObject.WORKSCHEDULE = this.WORKSCHEDULE;
            DataObject.OP = this.OP;
            DataObject.DETAILRESULT = this.DETAILRESULT;
            DataObject.STATION_TYPE = this.STATION_TYPE;
            return DataObject;
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
        public string ICTDATETIME
        {
            get
            {
                return (string)this["ICTDATETIME"];
            }
            set
            {
                this["ICTDATETIME"] = value;
            }
        }
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
            }
        }
        public string ICT_NO
        {
            get
            {
                return (string)this["ICT_NO"];
            }
            set
            {
                this["ICT_NO"] = value;
            }
        }
        public string ICTDATE
        {
            get
            {
                return (string)this["ICTDATE"];
            }
            set
            {
                this["ICTDATE"] = value;
            }
        }
        public string ICTTIME
        {
            get
            {
                return (string)this["ICTTIME"];
            }
            set
            {
                this["ICTTIME"] = value;
            }
        }
        public string RESULT
        {
            get
            {
                return (string)this["RESULT"];
            }
            set
            {
                this["RESULT"] = value;
            }
        }
        public string RETEST
        {
            get
            {
                return (string)this["RETEST"];
            }
            set
            {
                this["RETEST"] = value;
            }
        }
        public string WORKSCHEDULE
        {
            get
            {
                return (string)this["WORKSCHEDULE"];
            }
            set
            {
                this["WORKSCHEDULE"] = value;
            }
        }
        public string OP
        {
            get
            {
                return (string)this["OP"];
            }
            set
            {
                this["OP"] = value;
            }
        }
        public string DETAILRESULT
        {
            get
            {
                return (string)this["DETAILRESULT"];
            }
            set
            {
                this["DETAILRESULT"] = value;
            }
        }
        public string STATION_TYPE
        {
            get
            {
                return (string)this["STATION_TYPE"];
            }
            set
            {
                this["STATION_TYPE"] = value;
            }
        }
    }
    public class ICT_HEADER
    {
        public string SYSSERIALNO { get; set; }
        public string ICTDATETIME { get; set; }
        public string MODEL { get; set; }
        public string ICT_NO { get; set; }
        public string ICTDATE { get; set; }
        public string ICTTIME { get; set; }
        public string RESULT { get; set; }
        public string RETEST { get; set; }
        public string WORKSCHEDULE { get; set; }
        public string OP { get; set; }
        public string DETAILRESULT { get; set; }
        public string STATION_TYPE { get; set; }
    }
}