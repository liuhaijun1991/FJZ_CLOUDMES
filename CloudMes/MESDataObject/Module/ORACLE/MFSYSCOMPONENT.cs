using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_MFSYSCOMPONENT : DataObjectTable
    {
        public T_MFSYSCOMPONENT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_MFSYSCOMPONENT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_MFSYSCOMPONENT);
            TableName = "MFSYSCOMPONENT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_MFSYSCOMPONENT : DataObjectBase
    {
        public Row_MFSYSCOMPONENT(DataObjectInfo info) : base(info)
        {

        }
        public MFSYSCOMPONENT GetDataObject()
        {
            MFSYSCOMPONENT DataObject = new MFSYSCOMPONENT();
            DataObject.ID = this.ID;
            DataObject.SHIPORDERID = this.SHIPORDERID;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.VERSION = this.VERSION;
            DataObject.SEQNO = this.SEQNO;
            DataObject.QTY = this.QTY;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.REPLACENO = this.REPLACENO;
            DataObject.REPLACETOPARTNO = this.REPLACETOPARTNO;
            DataObject.KEYPART = this.KEYPART;
            DataObject.INSTALLED = this.INSTALLED;
            DataObject.INSTALLEDQTY = this.INSTALLEDQTY;
            DataObject.EEECODE = this.EEECODE;
            DataObject.CSERIALNO1 = this.CSERIALNO1;
            DataObject.CSERIALNO2 = this.CSERIALNO2;
            DataObject.CSERIALNO3 = this.CSERIALNO3;
            DataObject.CSERIALNO4 = this.CSERIALNO4;
            DataObject.CATEGORYNAME = this.CATEGORYNAME;
            DataObject.PRODCATEGORYNAME = this.PRODCATEGORYNAME;
            DataObject.PRODTYPE = this.PRODTYPE;
            DataObject.ORIGINALQTY = this.ORIGINALQTY;
            DataObject.UNITCOST = this.UNITCOST;
            DataObject.REPLACEGROUP = this.REPLACEGROUP;
            DataObject.NOREPLACEPART = this.NOREPLACEPART;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
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
        public string SHIPORDERID
        {
            get
            {
                return (string)this["SHIPORDERID"];
            }
            set
            {
                this["SHIPORDERID"] = value;
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
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string CUSTPARTNO
        {
            get
            {
                return (string)this["CUSTPARTNO"];
            }
            set
            {
                this["CUSTPARTNO"] = value;
            }
        }
        public double? REPLACENO
        {
            get
            {
                return (double?)this["REPLACENO"];
            }
            set
            {
                this["REPLACENO"] = value;
            }
        }
        public string REPLACETOPARTNO
        {
            get
            {
                return (string)this["REPLACETOPARTNO"];
            }
            set
            {
                this["REPLACETOPARTNO"] = value;
            }
        }
        public string KEYPART
        {
            get
            {
                return (string)this["KEYPART"];
            }
            set
            {
                this["KEYPART"] = value;
            }
        }
        public string INSTALLED
        {
            get
            {
                return (string)this["INSTALLED"];
            }
            set
            {
                this["INSTALLED"] = value;
            }
        }
        public double? INSTALLEDQTY
        {
            get
            {
                return (double?)this["INSTALLEDQTY"];
            }
            set
            {
                this["INSTALLEDQTY"] = value;
            }
        }
        public string EEECODE
        {
            get
            {
                return (string)this["EEECODE"];
            }
            set
            {
                this["EEECODE"] = value;
            }
        }
        public string CSERIALNO1
        {
            get
            {
                return (string)this["CSERIALNO1"];
            }
            set
            {
                this["CSERIALNO1"] = value;
            }
        }
        public string CSERIALNO2
        {
            get
            {
                return (string)this["CSERIALNO2"];
            }
            set
            {
                this["CSERIALNO2"] = value;
            }
        }
        public string CSERIALNO3
        {
            get
            {
                return (string)this["CSERIALNO3"];
            }
            set
            {
                this["CSERIALNO3"] = value;
            }
        }
        public string CSERIALNO4
        {
            get
            {
                return (string)this["CSERIALNO4"];
            }
            set
            {
                this["CSERIALNO4"] = value;
            }
        }
        public string CATEGORYNAME
        {
            get
            {
                return (string)this["CATEGORYNAME"];
            }
            set
            {
                this["CATEGORYNAME"] = value;
            }
        }
        public string PRODCATEGORYNAME
        {
            get
            {
                return (string)this["PRODCATEGORYNAME"];
            }
            set
            {
                this["PRODCATEGORYNAME"] = value;
            }
        }
        public string PRODTYPE
        {
            get
            {
                return (string)this["PRODTYPE"];
            }
            set
            {
                this["PRODTYPE"] = value;
            }
        }
        public string ORIGINALQTY
        {
            get
            {
                return (string)this["ORIGINALQTY"];
            }
            set
            {
                this["ORIGINALQTY"] = value;
            }
        }
        public double? UNITCOST
        {
            get
            {
                return (double?)this["UNITCOST"];
            }
            set
            {
                this["UNITCOST"] = value;
            }
        }
        public string REPLACEGROUP
        {
            get
            {
                return (string)this["REPLACEGROUP"];
            }
            set
            {
                this["REPLACEGROUP"] = value;
            }
        }
        public string NOREPLACEPART
        {
            get
            {
                return (string)this["NOREPLACEPART"];
            }
            set
            {
                this["NOREPLACEPART"] = value;
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
    }
    public class MFSYSCOMPONENT
    {
        public string ID { get; set; }
        public string SHIPORDERID { get; set; }
        public string SYSSERIALNO { get; set; }
        public string PARTNO { get; set; }
        public string VERSION { get; set; }
        public double? SEQNO { get; set; }
        public double? QTY { get; set; }
        public string CUSTPARTNO { get; set; }
        public double? REPLACENO { get; set; }
        public string REPLACETOPARTNO { get; set; }
        public string KEYPART { get; set; }
        public string INSTALLED { get; set; }
        public double? INSTALLEDQTY { get; set; }
        public string EEECODE { get; set; }
        public string CSERIALNO1 { get; set; }
        public string CSERIALNO2 { get; set; }
        public string CSERIALNO3 { get; set; }
        public string CSERIALNO4 { get; set; }
        public string CATEGORYNAME { get; set; }
        public string PRODCATEGORYNAME { get; set; }
        public string PRODTYPE { get; set; }
        public string ORIGINALQTY { get; set; }
        public double? UNITCOST { get; set; }
        public string REPLACEGROUP { get; set; }
        public string NOREPLACEPART { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
    }
}