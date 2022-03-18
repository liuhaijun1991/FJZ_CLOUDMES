using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP_PODETAIL : DataObjectTable
    {
        public T_R_SAP_PODETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_PODETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_PODETAIL);
            TableName = "R_SAP_PODETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_PODETAIL : DataObjectBase
    {
        public Row_R_SAP_PODETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_PODETAIL GetDataObject()
        {
            R_SAP_PODETAIL DataObject = new R_SAP_PODETAIL();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.PLANT = this.PLANT;
            DataObject.PN = this.PN;
            DataObject.ORDERQTY = this.ORDERQTY;
            DataObject.PNREV = this.PNREV;
            DataObject.PID = this.PID;
            DataObject.PIDREV = this.PIDREV;
            DataObject.SPARTDESC = this.SPARTDESC;
            DataObject.PPARTDESC = this.PPARTDESC;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string ORDERQTY
        {
            get
            {
                return (string)this["ORDERQTY"];
            }
            set
            {
                this["ORDERQTY"] = value;
            }
        }
        public string PNREV
        {
            get
            {
                return (string)this["PNREV"];
            }
            set
            {
                this["PNREV"] = value;
            }
        }
        public string PID
        {
            get
            {
                return (string)this["PID"];
            }
            set
            {
                this["PID"] = value;
            }
        }
        public string PIDREV
        {
            get
            {
                return (string)this["PIDREV"];
            }
            set
            {
                this["PIDREV"] = value;
            }
        }
        public string SPARTDESC
        {
            get
            {
                return (string)this["SPARTDESC"];
            }
            set
            {
                this["SPARTDESC"] = value;
            }
        }
        public string PPARTDESC
        {
            get
            {
                return (string)this["PPARTDESC"];
            }
            set
            {
                this["PPARTDESC"] = value;
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
    public class R_SAP_PODETAIL
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string PLANT { get; set; }
        public string PN { get; set; }
        public string ORDERQTY { get; set; }
        public string PNREV { get; set; }
        public string PID { get; set; }
        public string PIDREV { get; set; }
        public string SPARTDESC { get; set; }
        public string PPARTDESC { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}