using MESDBHelper;
using System;

namespace MESDataObject.Module.Juniper
{
    public class T_R_I054 : DataObjectTable
    {
        public T_R_I054(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I054(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I054);
            TableName = "R_I054".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I054 : DataObjectBase
    {
        public Row_R_I054(DataObjectInfo info) : base(info)
        {

        }
        public R_I054 GetDataObject()
        {
            R_I054 DataObject = new R_I054();
            DataObject.ID = this.ID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CREATIONDATETIME = this.CREATIONDATETIME;
            DataObject.CREATEDBY = this.CREATEDBY;
            DataObject.LOGICALSYSTEM = this.LOGICALSYSTEM;
            DataObject.SENDER = this.SENDER;
            DataObject.SALESORDERNUMBER = this.SALESORDERNUMBER;
            DataObject.SALESORDERLINENUMBER = this.SALESORDERLINENUMBER;
            DataObject.PARENTMODEL = this.PARENTMODEL;
            DataObject.PNTYPE = this.PNTYPE;
            DataObject.PARENTSN = this.PARENTSN;
            DataObject.CHILDMATERIAL = this.CHILDMATERIAL;
            DataObject.SN = this.SN;
            DataObject.REVISION = this.REVISION;
            DataObject.QTY = this.QTY;
            DataObject.COO = this.COO;
            DataObject.CLEICODE = this.CLEICODE;
            DataObject.MACADDRESS = this.MACADDRESS;
            DataObject.BUILTSITE = this.BUILTSITE;
            DataObject.BUILDDATE = this.BUILDDATE;
            DataObject.SOFTWAREVERSION = this.SOFTWAREVERSION;
            DataObject.SUBASSEMBLYNUMBER = this.SUBASSEMBLYNUMBER;
            DataObject.SUBASSEMBLYREVISION = this.SUBASSEMBLYREVISION;
            DataObject.IBMSERIALNUMBER = this.IBMSERIALNUMBER;
            DataObject.FUTURE1 = this.FUTURE1;
            DataObject.FUTURE2 = this.FUTURE2;
            DataObject.FUTURE3 = this.FUTURE3;
            DataObject.FUTURE4 = this.FUTURE4;
            DataObject.FUTURE5 = this.FUTURE5;
            DataObject.TRANID = this.TRANID;
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
        public string F_PLANT
        {
            get
            {
                return (string)this["F_PLANT"];
            }
            set
            {
                this["F_PLANT"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
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
        public DateTime? CREATIONDATETIME
        {
            get
            {
                return (DateTime?)this["CREATIONDATETIME"];
            }
            set
            {
                this["CREATIONDATETIME"] = value;
            }
        }
        public string CREATEDBY
        {
            get
            {
                return (string)this["CREATEDBY"];
            }
            set
            {
                this["CREATEDBY"] = value;
            }
        }
        public string LOGICALSYSTEM
        {
            get
            {
                return (string)this["LOGICALSYSTEM"];
            }
            set
            {
                this["LOGICALSYSTEM"] = value;
            }
        }
        public string SENDER
        {
            get
            {
                return (string)this["SENDER"];
            }
            set
            {
                this["SENDER"] = value;
            }
        }
        public string SALESORDERNUMBER
        {
            get
            {
                return (string)this["SALESORDERNUMBER"];
            }
            set
            {
                this["SALESORDERNUMBER"] = value;
            }
        }
        public string SALESORDERLINENUMBER
        {
            get
            {
                return (string)this["SALESORDERLINENUMBER"];
            }
            set
            {
                this["SALESORDERLINENUMBER"] = value;
            }
        }
        public string PARENTMODEL
        {
            get
            {
                return (string)this["PARENTMODEL"];
            }
            set
            {
                this["PARENTMODEL"] = value;
            }
        }
        public string PNTYPE
        {
            get
            {
                return (string)this["PNTYPE"];
            }
            set
            {
                this["PNTYPE"] = value;
            }
        }
        public string PARENTSN
        {
            get
            {
                return (string)this["PARENTSN"];
            }
            set
            {
                this["PARENTSN"] = value;
            }
        }
        public string CHILDMATERIAL
        {
            get
            {
                return (string)this["CHILDMATERIAL"];
            }
            set
            {
                this["CHILDMATERIAL"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
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
        public string QTY
        {
            get
            {
                return (string)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string COO
        {
            get
            {
                return (string)this["COO"];
            }
            set
            {
                this["COO"] = value;
            }
        }
        public string CLEICODE
        {
            get
            {
                return (string)this["CLEICODE"];
            }
            set
            {
                this["CLEICODE"] = value;
            }
        }
        public string MACADDRESS
        {
            get
            {
                return (string)this["MACADDRESS"];
            }
            set
            {
                this["MACADDRESS"] = value;
            }
        }
        public string BUILTSITE
        {
            get
            {
                return (string)this["BUILTSITE"];
            }
            set
            {
                this["BUILTSITE"] = value;
            }
        }
        public DateTime? BUILDDATE
        {
            get
            {
                return (DateTime?)this["BUILDDATE"];
            }
            set
            {
                this["BUILDDATE"] = value;
            }
        }
        public string SOFTWAREVERSION
        {
            get
            {
                return (string)this["SOFTWAREVERSION"];
            }
            set
            {
                this["SOFTWAREVERSION"] = value;
            }
        }
        public string SUBASSEMBLYNUMBER
        {
            get
            {
                return (string)this["SUBASSEMBLYNUMBER"];
            }
            set
            {
                this["SUBASSEMBLYNUMBER"] = value;
            }
        }
        public string SUBASSEMBLYREVISION
        {
            get
            {
                return (string)this["SUBASSEMBLYREVISION"];
            }
            set
            {
                this["SUBASSEMBLYREVISION"] = value;
            }
        }
        public string IBMSERIALNUMBER
        {
            get
            {
                return (string)this["IBMSERIALNUMBER"];
            }
            set
            {
                this["IBMSERIALNUMBER"] = value;
            }
        }
        public string FUTURE1
        {
            get
            {
                return (string)this["FUTURE1"];
            }
            set
            {
                this["FUTURE1"] = value;
            }
        }
        public string FUTURE2
        {
            get
            {
                return (string)this["FUTURE2"];
            }
            set
            {
                this["FUTURE2"] = value;
            }
        }
        public string FUTURE3
        {
            get
            {
                return (string)this["FUTURE3"];
            }
            set
            {
                this["FUTURE3"] = value;
            }
        }
        public string FUTURE4
        {
            get
            {
                return (string)this["FUTURE4"];
            }
            set
            {
                this["FUTURE4"] = value;
            }
        }
        public string FUTURE5
        {
            get
            {
                return (string)this["FUTURE5"];
            }
            set
            {
                this["FUTURE5"] = value;
            }
        }
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
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
    public class R_I054
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATETIME { get; set; }
        public string CREATEDBY { get; set; }
        public string LOGICALSYSTEM { get; set; }
        public string SENDER { get; set; }
        public string SALESORDERNUMBER { get; set; }
        public string SALESORDERLINENUMBER { get; set; }
        public string PARENTMODEL { get; set; }
        public string PNTYPE { get; set; }
        public string PARENTSN { get; set; }
        public string CHILDMATERIAL { get; set; }
        public string SN { get; set; }
        public string REVISION { get; set; }
        public string QTY { get; set; }
        public string COO { get; set; }
        public string CLEICODE { get; set; }
        public string MACADDRESS { get; set; }
        public string BUILTSITE { get; set; }
        public DateTime? BUILDDATE { get; set; }
        public string SOFTWAREVERSION { get; set; }
        public string SUBASSEMBLYNUMBER { get; set; }
        public string SUBASSEMBLYREVISION { get; set; }
        public string IBMSERIALNUMBER { get; set; }
        public string FUTURE1 { get; set; }
        public string FUTURE2 { get; set; }
        public string FUTURE3 { get; set; }
        public string FUTURE4 { get; set; }
        public string FUTURE5 { get; set; }
        public string TRANID { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}