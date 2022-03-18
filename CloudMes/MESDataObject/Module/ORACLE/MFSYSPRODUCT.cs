using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_MFSYSPRODUCT : DataObjectTable
    {
        public T_MFSYSPRODUCT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_MFSYSPRODUCT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_MFSYSPRODUCT);
            TableName = "MFSYSPRODUCT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public bool ChkFNNPID(string FNNSN,string FNNSKU, OleExec SFCDB)
        {
            List<MFSYSPRODUCT> mfsysproduct = new List<MFSYSPRODUCT>();
            mfsysproduct = SFCDB.ORM.Queryable<MFSYSPRODUCT>().Where(t => t.SYSSERIALNO == FNNSN && t.SKUNO == FNNSKU).ToList();
            if (mfsysproduct.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
    public class Row_MFSYSPRODUCT : DataObjectBase
    {
        public Row_MFSYSPRODUCT(DataObjectInfo info) : base(info)
        {

        }
        public MFSYSPRODUCT GetDataObject()
        {
            MFSYSPRODUCT DataObject = new MFSYSPRODUCT();
            DataObject.FIELD1 = this.FIELD1;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.SEQNO = this.SEQNO;
            DataObject.FACTORYID = this.FACTORYID;
            DataObject.ROUTEID = this.ROUTEID;
            DataObject.CUSTOMERID = this.CUSTOMERID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.EEECODE = this.EEECODE;
            DataObject.CUSTSSN = this.CUSTSSN;
            DataObject.FIRMWARE = this.FIRMWARE;
            DataObject.SOFTWARE = this.SOFTWARE;
            DataObject.SERVICETAG = this.SERVICETAG;
            DataObject.ENETID = this.ENETID;
            DataObject.PRIORITYCODE = this.PRIORITYCODE;
            DataObject.PRODUCTFAMILY = this.PRODUCTFAMILY;
            DataObject.PRODUCTLEVEL = this.PRODUCTLEVEL;
            DataObject.PRODUCTCOLOR = this.PRODUCTCOLOR;
            DataObject.PRODUCTLANGULAGE = this.PRODUCTLANGULAGE;
            DataObject.SHIPCOUNTRY = this.SHIPCOUNTRY;
            DataObject.PRODUCTDESC = this.PRODUCTDESC;
            DataObject.ORDERNO = this.ORDERNO;
            DataObject.COMPCODE = this.COMPCODE;
            DataObject.SHIPPED = this.SHIPPED;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.LOCATION = this.LOCATION;
            DataObject.WHID = this.WHID;
            DataObject.AREAID = this.AREAID;
            DataObject.WORKORDERTYPE = this.WORKORDERTYPE;
            DataObject.PACKAGENO = this.PACKAGENO;
            DataObject.SYSTEMSTAGE = this.SYSTEMSTAGE;
            DataObject.UNITCOST = this.UNITCOST;
            DataObject.LINESEQNO = this.LINESEQNO;
            DataObject.RESEATPRE = this.RESEATPRE;
            DataObject.RESEAT = this.RESEAT;
            DataObject.RESEATTAG = this.RESEATTAG;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.COO = this.COO;
            return DataObject;
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
        public string CUSTOMERID
        {
            get
            {
                return (string)this["CUSTOMERID"];
            }
            set
            {
                this["CUSTOMERID"] = value;
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
        public string CUSTSSN
        {
            get
            {
                return (string)this["CUSTSSN"];
            }
            set
            {
                this["CUSTSSN"] = value;
            }
        }
        public string FIRMWARE
        {
            get
            {
                return (string)this["FIRMWARE"];
            }
            set
            {
                this["FIRMWARE"] = value;
            }
        }
        public string SOFTWARE
        {
            get
            {
                return (string)this["SOFTWARE"];
            }
            set
            {
                this["SOFTWARE"] = value;
            }
        }
        public string SERVICETAG
        {
            get
            {
                return (string)this["SERVICETAG"];
            }
            set
            {
                this["SERVICETAG"] = value;
            }
        }
        public string ENETID
        {
            get
            {
                return (string)this["ENETID"];
            }
            set
            {
                this["ENETID"] = value;
            }
        }
        public string PRIORITYCODE
        {
            get
            {
                return (string)this["PRIORITYCODE"];
            }
            set
            {
                this["PRIORITYCODE"] = value;
            }
        }
        public string PRODUCTFAMILY
        {
            get
            {
                return (string)this["PRODUCTFAMILY"];
            }
            set
            {
                this["PRODUCTFAMILY"] = value;
            }
        }
        public string PRODUCTLEVEL
        {
            get
            {
                return (string)this["PRODUCTLEVEL"];
            }
            set
            {
                this["PRODUCTLEVEL"] = value;
            }
        }
        public string PRODUCTCOLOR
        {
            get
            {
                return (string)this["PRODUCTCOLOR"];
            }
            set
            {
                this["PRODUCTCOLOR"] = value;
            }
        }
        public string PRODUCTLANGULAGE
        {
            get
            {
                return (string)this["PRODUCTLANGULAGE"];
            }
            set
            {
                this["PRODUCTLANGULAGE"] = value;
            }
        }
        public string SHIPCOUNTRY
        {
            get
            {
                return (string)this["SHIPCOUNTRY"];
            }
            set
            {
                this["SHIPCOUNTRY"] = value;
            }
        }
        public string PRODUCTDESC
        {
            get
            {
                return (string)this["PRODUCTDESC"];
            }
            set
            {
                this["PRODUCTDESC"] = value;
            }
        }
        public string ORDERNO
        {
            get
            {
                return (string)this["ORDERNO"];
            }
            set
            {
                this["ORDERNO"] = value;
            }
        }
        public string COMPCODE
        {
            get
            {
                return (string)this["COMPCODE"];
            }
            set
            {
                this["COMPCODE"] = value;
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
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string WHID
        {
            get
            {
                return (string)this["WHID"];
            }
            set
            {
                this["WHID"] = value;
            }
        }
        public string AREAID
        {
            get
            {
                return (string)this["AREAID"];
            }
            set
            {
                this["AREAID"] = value;
            }
        }
        public string WORKORDERTYPE
        {
            get
            {
                return (string)this["WORKORDERTYPE"];
            }
            set
            {
                this["WORKORDERTYPE"] = value;
            }
        }
        public double? PACKAGENO
        {
            get
            {
                return (double?)this["PACKAGENO"];
            }
            set
            {
                this["PACKAGENO"] = value;
            }
        }
        public string SYSTEMSTAGE
        {
            get
            {
                return (string)this["SYSTEMSTAGE"];
            }
            set
            {
                this["SYSTEMSTAGE"] = value;
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
        public double? LINESEQNO
        {
            get
            {
                return (double?)this["LINESEQNO"];
            }
            set
            {
                this["LINESEQNO"] = value;
            }
        }
        public string RESEATPRE
        {
            get
            {
                return (string)this["RESEATPRE"];
            }
            set
            {
                this["RESEATPRE"] = value;
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
        public double? RESEATTAG
        {
            get
            {
                return (double?)this["RESEATTAG"];
            }
            set
            {
                this["RESEATTAG"] = value;
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
    }
    public class MFSYSPRODUCT
    {
        public string FIELD1 { get; set; }
        public string SYSSERIALNO { get; set; }
        public double? SEQNO { get; set; }
        public string FACTORYID { get; set; }
        public string ROUTEID { get; set; }
        public string CUSTOMERID { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string CUSTPARTNO { get; set; }
        public string EEECODE { get; set; }
        public string CUSTSSN { get; set; }
        public string FIRMWARE { get; set; }
        public string SOFTWARE { get; set; }
        public string SERVICETAG { get; set; }
        public string ENETID { get; set; }
        public string PRIORITYCODE { get; set; }
        public string PRODUCTFAMILY { get; set; }
        public string PRODUCTLEVEL { get; set; }
        public string PRODUCTCOLOR { get; set; }
        public string PRODUCTLANGULAGE { get; set; }
        public string SHIPCOUNTRY { get; set; }
        public string PRODUCTDESC { get; set; }
        public string ORDERNO { get; set; }
        public string COMPCODE { get; set; }
        public string SHIPPED { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string LOCATION { get; set; }
        public string WHID { get; set; }
        public string AREAID { get; set; }
        public string WORKORDERTYPE { get; set; }
        public double? PACKAGENO { get; set; }
        public string SYSTEMSTAGE { get; set; }
        public double? UNITCOST { get; set; }
        public double? LINESEQNO { get; set; }
        public string RESEATPRE { get; set; }
        public string RESEAT { get; set; }
        public double? RESEATTAG { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string COO { get; set; }
    }
}
