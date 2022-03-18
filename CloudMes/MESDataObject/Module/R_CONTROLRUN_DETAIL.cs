using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CONTROLRUN_DETAIL : DataObjectTable
    {
        public T_R_CONTROLRUN_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CONTROLRUN_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CONTROLRUN_DETAIL);
            TableName = "R_CONTROLRUN_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckControlDataExist(string CONTROLID, string DATA, OleExec DB)=> 
            DB.ORM.Queryable<R_CONTROLRUN_DETAIL>()
            .Where(it => 
            it.CONTROLID == CONTROLID 
            && it.DATA == DATA 
            && it.VALID_FLAG == "1")
            .Any();
        
        public int GetSNQTYbyControlID(string CONTROLID, OleExec DB)
        {
            return DB.ORM.Queryable<R_CONTROLRUN_DETAIL>()
            .Where(it => it.CONTROLID == CONTROLID && it.VALID_FLAG == "1")
            .Count();
        }

        public List<string> GetSNListByControlID(string CONTROLID, OleExec DB)
        {
            return DB.ORM.Queryable<R_CONTROLRUN_DETAIL>()
            .Where(it => it.CONTROLID == CONTROLID && it.VALID_FLAG == "1")
            .Select(it=>it.DATA).ToList();
        }

        public int GetBuildQty(string CONTROLID,string StationName,OleExec DB)
        {
            return DB.ORM.Queryable<R_CONTROLRUN_DETAIL, R_SN_STATION_DETAIL>((a, b) => a.DATA == b.SN)
                .Where((a, b) => 
                a.VALID_FLAG == "1" 
                && b.VALID_FLAG=="1" 
                && a.CONTROLID== CONTROLID
                && b.STATION_NAME== StationName)
                .GroupBy((a,b)=>a.DATA)
                .Select((a,b)=>a.DATA)
                .Count();
        }

        public int GetFailQty(string CONTROLID, string StationName, OleExec DB)
        {
            return DB.ORM.Queryable<R_CONTROLRUN_DETAIL, R_SN_STATION_DETAIL>((a, b) => a.DATA == b.SN)
                .Where((a, b) =>
                a.VALID_FLAG == "1"
                && b.VALID_FLAG == "1"
                && a.CONTROLID == CONTROLID
                && b.STATION_NAME == StationName
                && b.REPAIR_FAILED_FLAG=="1")
                .GroupBy((a, b) => a.DATA)
                .Select((a, b) => a.DATA)
                .Count();
        }
    }
    public class Row_R_CONTROLRUN_DETAIL : DataObjectBase
    {
        public Row_R_CONTROLRUN_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_CONTROLRUN_DETAIL GetDataObject()
        {
            R_CONTROLRUN_DETAIL DataObject = new R_CONTROLRUN_DETAIL();
            DataObject.ID = this.ID;
            DataObject.CONTROLID = this.CONTROLID;
            DataObject.DATA = this.DATA;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
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
        public string DATA
        {
            get
            {
                return (string)this["DATA"];
            }
            set
            {
                this["DATA"] = value;
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
    }
    public class R_CONTROLRUN_DETAIL
    {
        public string ID { get; set; }
        public string CONTROLID { get; set; }
        public string DATA { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}