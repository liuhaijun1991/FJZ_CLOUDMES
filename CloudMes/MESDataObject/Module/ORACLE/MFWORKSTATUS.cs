using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
namespace MESDataObject.Module
{
    public class T_MFWORKSTATUS : DataObjectTable
    {
        public T_MFWORKSTATUS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_MFWORKSTATUS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_MFWORKSTATUS);
            TableName = "MFWORKSTATUS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public bool ChkFNNUnitSN(string FNNSN, OleExec SFCDB)
        {
            List<MFWORKSTATUS> mfworkstatus = new List<MFWORKSTATUS>();
            mfworkstatus = SFCDB.ORM.Queryable<MFWORKSTATUS>().Where(t => t.SYSSERIALNO == FNNSN && t.COMPLETED == "True").ToList();
            if (mfworkstatus.Count > 0)
            {
                return true;
            }
            return false;
        }

    }
    public class Row_MFWORKSTATUS : DataObjectBase
    {
        public Row_MFWORKSTATUS(DataObjectInfo info) : base(info)
        {

        }
        public MFWORKSTATUS GetDataObject()
        {
            MFWORKSTATUS DataObject = new MFWORKSTATUS();
            DataObject.QUITDATE = this.QUITDATE;
            DataObject.LASTEVENT = this.LASTEVENT;
            DataObject.PRODUCTSTATUS = this.PRODUCTSTATUS;
            DataObject.RESEATCOUNT = this.RESEATCOUNT;
            DataObject.RESEAT = this.RESEAT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.REFLOW = this.REFLOW;
            DataObject.REFLOWTIME = this.REFLOWTIME;
            DataObject.ORT_IN_TIME = this.ORT_IN_TIME;
            DataObject.ORT_OUT_TIME = this.ORT_OUT_TIME;
            DataObject.ORT_FAIL_TIME = this.ORT_FAIL_TIME;
            DataObject.ORT_COUNT = this.ORT_COUNT;
            DataObject.ORT_FLAG = this.ORT_FLAG;
            DataObject.ORT_OUTFLAG = this.ORT_OUTFLAG;
            DataObject.STOCKINTIME = this.STOCKINTIME;
            DataObject.STOCKOUTTIME = this.STOCKOUTTIME;
            DataObject.STOCKSTATUS = this.STOCKSTATUS;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.ASSIGNDATE = this.ASSIGNDATE;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.FACTORYID = this.FACTORYID;
            DataObject.PRODUCTIONLINE = this.PRODUCTIONLINE;
            DataObject.SHIFT = this.SHIFT;
            DataObject.BUILDNO = this.BUILDNO;
            DataObject.ROUTEID = this.ROUTEID;
            DataObject.STARTED = this.STARTED;
            DataObject.STARTDATE = this.STARTDATE;
            DataObject.PACKED = this.PACKED;
            DataObject.PACKDATE = this.PACKDATE;
            DataObject.COMPLETED = this.COMPLETED;
            DataObject.COMPLETEDATE = this.COMPLETEDATE;
            DataObject.SHIPPED = this.SHIPPED;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.REPAIRHELD = this.REPAIRHELD;
            DataObject.REPAIRDATE = this.REPAIRDATE;
            DataObject.CURRENTPDLINE = this.CURRENTPDLINE;
            DataObject.CURRENTSHIFT = this.CURRENTSHIFT;
            DataObject.CURRENTEVENT = this.CURRENTEVENT;
            DataObject.NEXTEVENT = this.NEXTEVENT;
            DataObject.STUFFINGQTY = this.STUFFINGQTY;
            DataObject.FIELD1 = this.FIELD1;
            DataObject.QUITED = this.QUITED;
            return DataObject;
        }
        public DateTime? QUITDATE
        {
            get
            {
                return (DateTime?)this["QUITDATE"];
            }
            set
            {
                this["QUITDATE"] = value;
            }
        }
        public string LASTEVENT
        {
            get
            {
                return (string)this["LASTEVENT"];
            }
            set
            {
                this["LASTEVENT"] = value;
            }
        }
        public string PRODUCTSTATUS
        {
            get
            {
                return (string)this["PRODUCTSTATUS"];
            }
            set
            {
                this["PRODUCTSTATUS"] = value;
            }
        }
        public double? RESEATCOUNT
        {
            get
            {
                return (double?)this["RESEATCOUNT"];
            }
            set
            {
                this["RESEATCOUNT"] = value;
            }
        }
        public string RESEAT
        {
            get
            {
                return (string)this["RESEAT"];
            }
            set
            {
                this["RESEAT"] = value;
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
        public string REFLOW
        {
            get
            {
                return (string)this["REFLOW"];
            }
            set
            {
                this["REFLOW"] = value;
            }
        }
        public DateTime? REFLOWTIME
        {
            get
            {
                return (DateTime?)this["REFLOWTIME"];
            }
            set
            {
                this["REFLOWTIME"] = value;
            }
        }
        public DateTime? ORT_IN_TIME
        {
            get
            {
                return (DateTime?)this["ORT_IN_TIME"];
            }
            set
            {
                this["ORT_IN_TIME"] = value;
            }
        }
        public DateTime? ORT_OUT_TIME
        {
            get
            {
                return (DateTime?)this["ORT_OUT_TIME"];
            }
            set
            {
                this["ORT_OUT_TIME"] = value;
            }
        }
        public DateTime? ORT_FAIL_TIME
        {
            get
            {
                return (DateTime?)this["ORT_FAIL_TIME"];
            }
            set
            {
                this["ORT_FAIL_TIME"] = value;
            }
        }
        public double? ORT_COUNT
        {
            get
            {
                return (double?)this["ORT_COUNT"];
            }
            set
            {
                this["ORT_COUNT"] = value;
            }
        }
        public string ORT_FLAG
        {
            get
            {
                return (string)this["ORT_FLAG"];
            }
            set
            {
                this["ORT_FLAG"] = value;
            }
        }
        public string ORT_OUTFLAG
        {
            get
            {
                return (string)this["ORT_OUTFLAG"];
            }
            set
            {
                this["ORT_OUTFLAG"] = value;
            }
        }
        public DateTime? STOCKINTIME
        {
            get
            {
                return (DateTime?)this["STOCKINTIME"];
            }
            set
            {
                this["STOCKINTIME"] = value;
            }
        }
        public DateTime? STOCKOUTTIME
        {
            get
            {
                return (DateTime?)this["STOCKOUTTIME"];
            }
            set
            {
                this["STOCKOUTTIME"] = value;
            }
        }
        public string STOCKSTATUS
        {
            get
            {
                return (string)this["STOCKSTATUS"];
            }
            set
            {
                this["STOCKSTATUS"] = value;
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
        public DateTime? ASSIGNDATE
        {
            get
            {
                return (DateTime?)this["ASSIGNDATE"];
            }
            set
            {
                this["ASSIGNDATE"] = value;
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
        public string FACTORYID
        {
            get
            {
                return (string)this["FACTORYID"];
            }
            set
            {
                this["FACTORYID"] = value;
            }
        }
        public string PRODUCTIONLINE
        {
            get
            {
                return (string)this["PRODUCTIONLINE"];
            }
            set
            {
                this["PRODUCTIONLINE"] = value;
            }
        }
        public string SHIFT
        {
            get
            {
                return (string)this["SHIFT"];
            }
            set
            {
                this["SHIFT"] = value;
            }
        }
        public string BUILDNO
        {
            get
            {
                return (string)this["BUILDNO"];
            }
            set
            {
                this["BUILDNO"] = value;
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
        public string STARTED
        {
            get
            {
                return (string)this["STARTED"];
            }
            set
            {
                this["STARTED"] = value;
            }
        }
        public DateTime? STARTDATE
        {
            get
            {
                return (DateTime?)this["STARTDATE"];
            }
            set
            {
                this["STARTDATE"] = value;
            }
        }
        public string PACKED
        {
            get
            {
                return (string)this["PACKED"];
            }
            set
            {
                this["PACKED"] = value;
            }
        }
        public DateTime? PACKDATE
        {
            get
            {
                return (DateTime?)this["PACKDATE"];
            }
            set
            {
                this["PACKDATE"] = value;
            }
        }
        public string COMPLETED
        {
            get
            {
                return (string)this["COMPLETED"];
            }
            set
            {
                this["COMPLETED"] = value;
            }
        }
        public DateTime? COMPLETEDATE
        {
            get
            {
                return (DateTime?)this["COMPLETEDATE"];
            }
            set
            {
                this["COMPLETEDATE"] = value;
            }
        }
        public string SHIPPED
        {
            get
            {
                return (string)this["SHIPPED"];
            }
            set
            {
                this["SHIPPED"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string REPAIRHELD
        {
            get
            {
                return (string)this["REPAIRHELD"];
            }
            set
            {
                this["REPAIRHELD"] = value;
            }
        }
        public DateTime? REPAIRDATE
        {
            get
            {
                return (DateTime?)this["REPAIRDATE"];
            }
            set
            {
                this["REPAIRDATE"] = value;
            }
        }
        public string CURRENTPDLINE
        {
            get
            {
                return (string)this["CURRENTPDLINE"];
            }
            set
            {
                this["CURRENTPDLINE"] = value;
            }
        }
        public string CURRENTSHIFT
        {
            get
            {
                return (string)this["CURRENTSHIFT"];
            }
            set
            {
                this["CURRENTSHIFT"] = value;
            }
        }
        public string CURRENTEVENT
        {
            get
            {
                return (string)this["CURRENTEVENT"];
            }
            set
            {
                this["CURRENTEVENT"] = value;
            }
        }
        public string NEXTEVENT
        {
            get
            {
                return (string)this["NEXTEVENT"];
            }
            set
            {
                this["NEXTEVENT"] = value;
            }
        }
        public string STUFFINGQTY
        {
            get
            {
                return (string)this["STUFFINGQTY"];
            }
            set
            {
                this["STUFFINGQTY"] = value;
            }
        }
        public string FIELD1
        {
            get
            {
                return (string)this["FIELD1"];
            }
            set
            {
                this["FIELD1"] = value;
            }
        }
        public string QUITED
        {
            get
            {
                return (string)this["QUITED"];
            }
            set
            {
                this["QUITED"] = value;
            }
        }
    }
    public class MFWORKSTATUS
    {
        public DateTime? QUITDATE { get; set; }
        public string LASTEVENT { get; set; }
        public string PRODUCTSTATUS { get; set; }
        public double? RESEATCOUNT { get; set; }
        public string RESEAT { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string REFLOW { get; set; }
        public DateTime? REFLOWTIME { get; set; }
        public DateTime? ORT_IN_TIME { get; set; }
        public DateTime? ORT_OUT_TIME { get; set; }
        public DateTime? ORT_FAIL_TIME { get; set; }
        public double? ORT_COUNT { get; set; }
        public string ORT_FLAG { get; set; }
        public string ORT_OUTFLAG { get; set; }
        public DateTime? STOCKINTIME { get; set; }
        public DateTime? STOCKOUTTIME { get; set; }
        public string STOCKSTATUS { get; set; }
        public string SYSSERIALNO { get; set; }
        public DateTime? ASSIGNDATE { get; set; }
        public string WORKORDERNO{ get; set; }
        public string FACTORYID { get; set; }
        public string PRODUCTIONLINE { get; set; }
        public string SHIFT { get; set; }
        public string BUILDNO { get; set; }
        public string ROUTEID { get; set; }
        public string STARTED { get; set; }
        public DateTime? STARTDATE { get; set; }
        public string PACKED { get; set; }
        public DateTime? PACKDATE { get; set; }
        public string COMPLETED { get; set; }
        public DateTime? COMPLETEDATE { get; set; }
        public string SHIPPED { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string REPAIRHELD { get; set; }
        public DateTime? REPAIRDATE { get; set; }
        public string CURRENTPDLINE { get; set; }
        public string CURRENTSHIFT { get; set; }
        public string CURRENTEVENT { get; set; }
        public string NEXTEVENT { get; set; }
        public string STUFFINGQTY { get; set; }
        public string FIELD1 { get; set; }
        public string QUITED { get; set; }
    }
}
