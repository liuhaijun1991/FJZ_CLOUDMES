using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PO_TRACKING : DataObjectTable
    {
        public T_R_PO_TRACKING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PO_TRACKING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PO_TRACKING);
            TableName = "R_PO_TRACKING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PO_TRACKING : DataObjectBase
    {
        public Row_R_PO_TRACKING(DataObjectInfo info) : base(info)
        {

        }
        public R_PO_TRACKING GetDataObject()
        {
            R_PO_TRACKING DataObject = new R_PO_TRACKING();
            DataObject.ID = this.ID;
            DataObject.POID = this.POID;
            DataObject.PONO = this.PONO;
            DataObject.POLINE = this.POLINE;
            DataObject.STATUSID = this.STATUSID;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITEMP = this.EDITEMP;
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
        public string POID
        {
            get
            {
                return (string)this["POID"];
            }
            set
            {
                this["POID"] = value;
            }
        }
        public string PONO
        {
            get
            {
                return (string)this["PONO"];
            }
            set
            {
                this["PONO"] = value;
            }
        }
        public string POLINE
        {
            get
            {
                return (string)this["POLINE"];
            }
            set
            {
                this["POLINE"] = value;
            }
        }
        public string STATUSID
        {
            get
            {
                return (string)this["STATUSID"];
            }
            set
            {
                this["STATUSID"] = value;
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
        public string EDITEMP
        {
            get
            {
                return (string)this["EDITEMP"];
            }
            set
            {
                this["EDITEMP"] = value;
            }
        }
    }
    public class R_PO_TRACKING
    {
        public string ID { get; set; }
        public string POID { get; set; }
        public string PONO { get; set; }
        public string POLINE { get; set; }
        public string STATUSID { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITEMP { get; set; }
    }
}