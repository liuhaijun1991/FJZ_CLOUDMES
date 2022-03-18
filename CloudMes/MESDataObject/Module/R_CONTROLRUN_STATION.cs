using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CONTROLRUN_STATION : DataObjectTable
    {
        public T_R_CONTROLRUN_STATION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CONTROLRUN_STATION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CONTROLRUN_STATION);
            TableName = "R_CONTROLRUN_STATION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_CONTROLRUN_STATION> GetListByControlID (string CONTROLID,OleExec DB)
        {
            return DB.ORM.Queryable<R_CONTROLRUN_STATION>()
                .Where(it => it.CONTROLID == CONTROLID && it.VALID_FLAG == "1")
                .OrderBy(it => it.SEQNO, SqlSugar.OrderByType.Asc)
                .ToList();
         }

        public List<string> GetStationNameByControlID(string CONTROLID, OleExec DB)
        {
            return DB.ORM.Queryable<R_CONTROLRUN_STATION>()
                .Where(it => it.CONTROLID == CONTROLID && it.VALID_FLAG == "1")
                .OrderBy(it => it.SEQNO, SqlSugar.OrderByType.Asc)
                .Select(it=>it.STATION_NAME)
                .ToList();
        }


        public void SetInvalidByControlID(string CONTROLID,string EDITBY,DateTime EDITTIME, OleExec DB)
        {
            DB.ORM.Updateable<R_CONTROLRUN_STATION>().UpdateColumns
           (it => new R_CONTROLRUN_STATION
           {
               VALID_FLAG = "0",
               EDITBY = EDITBY,
               EDITTIME = EDITTIME
           })
           .Where(it => it.CONTROLID == CONTROLID && it.VALID_FLAG == "1")
           .ExecuteCommand();
               
        }
    }
    public class Row_R_CONTROLRUN_STATION : DataObjectBase
    {
        public Row_R_CONTROLRUN_STATION(DataObjectInfo info) : base(info)
        {

        }
        public R_CONTROLRUN_STATION GetDataObject()
        {
            R_CONTROLRUN_STATION DataObject = new R_CONTROLRUN_STATION();
            DataObject.ID = this.ID;
            DataObject.CONTROLID = this.CONTROLID;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
            DataObject.SEQNO = this.SEQNO;
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
        public string CONTROLID
        {
            get
            {
                return (string)this["CONTROLID"];
            }
            set
            {
                this["CONTROLID"] = value;
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
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
    }
    public class R_CONTROLRUN_STATION
    {
        public string ID { get; set; }
        public string CONTROLID { get; set; }
        public string STATION_NAME { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
        public double? SEQNO { get; set; }
    }
}