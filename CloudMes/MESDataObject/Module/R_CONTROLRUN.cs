using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CONTROLRUN : DataObjectTable
    {
        public T_R_CONTROLRUN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CONTROLRUN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CONTROLRUN);
            TableName = "R_CONTROLRUN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_CONTROLRUN> GetMainMenuList(OleExec DB)
        {
            

            return DB.ORM.Queryable<R_CONTROLRUN>()
                .Where(a => a.VALID_FLAG == "1")
                .OrderBy(a => a.EDITTIME, SqlSugar.OrderByType.Desc)
                .ToList();
        }
        public void DelControlRun(string ID,OleExec DB)
        {
             DB.ORM.Updateable<R_CONTROLRUN>()
                .Where(a => a.VALID_FLAG == "1" && a.ID== ID)
                .UpdateColumns(a=>new R_CONTROLRUN {
                    VALID_FLAG="0"
                })
                .ExecuteCommand();
        }

 
    }
    public class Row_R_CONTROLRUN : DataObjectBase
    {
        public Row_R_CONTROLRUN(DataObjectInfo info) : base(info)
        {

        }
        public R_CONTROLRUN GetDataObject()
        {
            R_CONTROLRUN DataObject = new R_CONTROLRUN();
            DataObject.ID = this.ID;
            DataObject.TYPE = this.TYPE;
            DataObject.CUS_DEV = this.CUS_DEV;
            DataObject.REVISION = this.REVISION;
            DataObject.STARTTIME = this.STARTTIME;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.STATUS = this.STATUS;
            DataObject.REASON = this.REASON;
            DataObject.COMMENTS = this.COMMENTS;
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
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public string CUS_DEV
        {
            get
            {
                return (string)this["CUS_DEV"];
            }
            set
            {
                this["CUS_DEV"] = value;
            }
        }
        public string REVISION
        {
            get
            {
                return (string)this["REVISION"];
            }
            set
            {
                this["REVISION"] = value;
            }
        }
        public DateTime? STARTTIME
        {
            get
            {
                return (DateTime?)this["STARTTIME"];
            }
            set
            {
                this["STARTTIME"] = value;
            }
        }
        public DateTime? ENDTIME
        {
            get
            {
                return (DateTime?)this["ENDTIME"];
            }
            set
            {
                this["ENDTIME"] = value;
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
        public string REASON
        {
            get
            {
                return (string)this["REASON"];
            }
            set
            {
                this["REASON"] = value;
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
    public class R_CONTROLRUN
    {
        public string ID { get; set; }
        public string TYPE { get; set; }
        public string CUS_DEV { get; set; }
        public string REVISION { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string STATUS { get; set; }
        public string REASON { get; set; }
        public string COMMENTS { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
    public class R_CONTROLRUN_MENU_LIST
    {
        public string ID { get; set; }
        public string TYPE { get; set; }
        public int QTY { get; set; }
        public string CUS_DEV { get; set; }
        public string REVISION { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string STATUS { get; set; }
        public string REASON { get; set; }
        public string COMMENTS { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}