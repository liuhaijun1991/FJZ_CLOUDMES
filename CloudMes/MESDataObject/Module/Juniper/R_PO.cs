using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PO : DataObjectTable
    {
        public T_R_PO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PO);
            TableName = "R_PO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PO : DataObjectBase
    {
        public Row_R_PO(DataObjectInfo info) : base(info)
        {

        }
        public R_PO GetDataObject()
        {
            R_PO DataObject = new R_PO();
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.PODOCTYPE = this.PODOCTYPE;
            DataObject.COMPLETEDELIVERY = this.COMPLETEDELIVERY;
            DataObject.SHIPCOUNTRYCODE = this.SHIPCOUNTRYCODE;
            DataObject.SCHEDULINGSTATUS = this.SCHEDULINGSTATUS;
            DataObject.PACKOUTLABELTYPE = this.PACKOUTLABELTYPE;
            DataObject.COUNTRYSPECIFICLABEL = this.COUNTRYSPECIFICLABEL;
            DataObject.SO = this.SO;
            DataObject.SOLINE = this.SOLINE;
            DataObject.ID = this.ID;
            DataObject.PONO = this.PONO;
            DataObject.POLINE = this.POLINE;
            DataObject.POTYPE = this.POTYPE;
            DataObject.STATUSID = this.STATUSID;
            DataObject.POQTY = this.POQTY;
            DataObject.PREWOFLAG = this.PREWOFLAG;
            DataObject.UNITPRICE = this.UNITPRICE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.FINISHED = this.FINISHED;
            DataObject.CUSTREQSHIPDATE = this.CUSTREQSHIPDATE;
            DataObject.DELIVERYDATETIME = this.DELIVERYDATETIME;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITEMP = this.EDITEMP;
            return DataObject;
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string MESSAGEID
        {
            get
            {
                return (string)this["MESSAGEID"];
            }
            set
            {
                this["MESSAGEID"] = value;
            }
        }
        public string PODOCTYPE
        {
            get
            {
                return (string)this["PODOCTYPE"];
            }
            set
            {
                this["PODOCTYPE"] = value;
            }
        }
        public string COMPLETEDELIVERY
        {
            get
            {
                return (string)this["COMPLETEDELIVERY"];
            }
            set
            {
                this["COMPLETEDELIVERY"] = value;
            }
        }
        public string SHIPCOUNTRYCODE
        {
            get
            {
                return (string)this["SHIPCOUNTRYCODE"];
            }
            set
            {
                this["SHIPCOUNTRYCODE"] = value;
            }
        }
        public string SCHEDULINGSTATUS
        {
            get
            {
                return (string)this["SCHEDULINGSTATUS"];
            }
            set
            {
                this["SCHEDULINGSTATUS"] = value;
            }
        }
        public string PACKOUTLABELTYPE
        {
            get
            {
                return (string)this["PACKOUTLABELTYPE"];
            }
            set
            {
                this["PACKOUTLABELTYPE"] = value;
            }
        }
        public string COUNTRYSPECIFICLABEL
        {
            get
            {
                return (string)this["COUNTRYSPECIFICLABEL"];
            }
            set
            {
                this["COUNTRYSPECIFICLABEL"] = value;
            }
        }
        public string SO
        {
            get
            {
                return (string)this["SO"];
            }
            set
            {
                this["SO"] = value;
            }
        }
        public string SOLINE
        {
            get
            {
                return (string)this["SOLINE"];
            }
            set
            {
                this["SOLINE"] = value;
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
        public string POTYPE
        {
            get
            {
                return (string)this["POTYPE"];
            }
            set
            {
                this["POTYPE"] = value;
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
        public double? POQTY
        {
            get
            {
                return (double?)this["POQTY"];
            }
            set
            {
                this["POQTY"] = value;
            }
        }
        public string PREWOFLAG
        {
            get
            {
                return (string)this["PREWOFLAG"];
            }
            set
            {
                this["PREWOFLAG"] = value;
            }
        }
        public string UNITPRICE
        {
            get
            {
                return (string)this["UNITPRICE"];
            }
            set
            {
                this["UNITPRICE"] = value;
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
        public string FINISHED
        {
            get
            {
                return (string)this["FINISHED"];
            }
            set
            {
                this["FINISHED"] = value;
            }
        }
        public DateTime? CUSTREQSHIPDATE
        {
            get
            {
                return (DateTime?)this["CUSTREQSHIPDATE"];
            }
            set
            {
                this["CUSTREQSHIPDATE"] = value;
            }
        }
        public DateTime? DELIVERYDATETIME
        {
            get
            {
                return (DateTime?)this["DELIVERYDATETIME"];
            }
            set
            {
                this["DELIVERYDATETIME"] = value;
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
    public class R_PO
    {
        public string DESCRIPTION { get; set; }
        public string MESSAGEID { get; set; }
        public string PODOCTYPE { get; set; }
        public string COMPLETEDELIVERY { get; set; }
        public string SHIPCOUNTRYCODE { get; set; }
        public string SCHEDULINGSTATUS { get; set; }
        public string PACKOUTLABELTYPE { get; set; }
        public string COUNTRYSPECIFICLABEL { get; set; }
        public string SO { get; set; }
        public string SOLINE { get; set; }
        public string ID { get; set; }
        public string PONO { get; set; }
        public string POLINE { get; set; }
        public string POTYPE { get; set; }
        public string STATUSID { get; set; }
        public double? POQTY { get; set; }
        public string PREWOFLAG { get; set; }
        public string UNITPRICE { get; set; }
        public string SKUNO { get; set; }
        public string FINISHED { get; set; }
        public DateTime? CUSTREQSHIPDATE { get; set; }
        public DateTime? DELIVERYDATETIME { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITEMP { get; set; }
    }
}