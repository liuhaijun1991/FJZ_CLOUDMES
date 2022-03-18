using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_ict_detail : DataObjectTable
    {
        public T_ict_detail(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_ict_detail(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_ict_detail);
            TableName = "ict_detail".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_ict_detail : DataObjectBase
    {
        public Row_ict_detail(DataObjectInfo info) : base(info)
        {

        }
        public ict_detail GetDataObject()
        {
            ict_detail DataObject = new ict_detail();
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.ICTDATETIME = this.ICTDATETIME;
            DataObject.ICTHEADER = this.ICTHEADER;
            DataObject.ICTDETAIL = this.ICTDETAIL;
            DataObject.ICTDATE = this.ICTDATE;
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
        public string ICTHEADER
        {
            get
            {
                return (string)this["ICTHEADER"];
            }
            set
            {
                this["ICTHEADER"] = value;
            }
        }
        public string ICTDETAIL
        {
            get
            {
                return (string)this["ICTDETAIL"];
            }
            set
            {
                this["ICTDETAIL"] = value;
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
    public class ict_detail
    {
        public string SYSSERIALNO { get; set; }
        public string ICTDATETIME { get; set; }
        public string ICTHEADER { get; set; }
        public string ICTDETAIL { get; set; }
        public string ICTDATE { get; set; }
        public string STATION_TYPE { get; set; }
    }
}