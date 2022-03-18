using MESDBHelper;
using System;

namespace MESDataObject.Module
{
    public class T_C_MMPRODMASTER : DataObjectTable
    {
        public T_C_MMPRODMASTER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MMPRODMASTER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MMPRODMASTER);
            TableName = "C_MMPRODMASTER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_MMPRODMASTER : DataObjectBase
    {
        public Row_C_MMPRODMASTER(DataObjectInfo info) : base(info)
        {

        }
        public C_MMPRODMASTER GetDataObject()
        {
            C_MMPRODMASTER DataObject = new C_MMPRODMASTER();
            DataObject.LOT_SIZE = this.LOT_SIZE;
            DataObject.MIN_LOT_SIZE = this.MIN_LOT_SIZE;
            DataObject.PLANTID = this.PLANTID;
            DataObject.PARTNO = this.PARTNO;
            DataObject.CATEGORYNAME = this.CATEGORYNAME;
            DataObject.PARTNAME = this.PARTNAME;
            DataObject.VERSION = this.VERSION;
            DataObject.ALTERNATENAME = this.ALTERNATENAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.PRODUCTTYPE = this.PRODUCTTYPE;
            DataObject.MATERIALTYPE = this.MATERIALTYPE;
            DataObject.SOURCETYPE = this.SOURCETYPE;
            DataObject.VENDORCODE = this.VENDORCODE;
            DataObject.UPCCODE = this.UPCCODE;
            DataObject.EEECODE = this.EEECODE;
            DataObject.TAXEXEMPT = this.TAXEXEMPT;
            DataObject.TAXCODE = this.TAXCODE;
            DataObject.MAINWHID = this.MAINWHID;
            DataObject.UOM = this.UOM;
            DataObject.WEIGHT = this.WEIGHT;
            DataObject.LENGETH = this.LENGETH;
            DataObject.WIDTH = this.WIDTH;
            DataObject.HEIGHT = this.HEIGHT;
            DataObject.BOMITEM = this.BOMITEM;
            DataObject.VIRTUAL = this.VIRTUAL;
            DataObject.JITITEM = this.JITITEM;
            DataObject.OOD = this.OOD;
            DataObject.OODDATE = this.OODDATE;
            DataObject.ISMATERIAL = this.ISMATERIAL;
            DataObject.CONSUMPTIONITEM = this.CONSUMPTIONITEM;
            DataObject.PACKMATERIAL = this.PACKMATERIAL;
            DataObject.CURRENCYTYPE = this.CURRENCYTYPE;
            DataObject.STDCOST = this.STDCOST;
            DataObject.STDPRICE = this.STDPRICE;
            DataObject.UNITCOST = this.UNITCOST;
            DataObject.UNITPRICE = this.UNITPRICE;
            DataObject.GRAPHICLINK = this.GRAPHICLINK;
            DataObject.FIELD1 = this.FIELD1;
            DataObject.FIELD2 = this.FIELD2;
            DataObject.FIELD3 = this.FIELD3;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATEDATE = this.CREATEDATE;
            DataObject.CUSTVERSION = this.CUSTVERSION;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.ABCCODE = this.ABCCODE;
            DataObject.KEYPART = this.KEYPART;
            DataObject.RFA = this.RFA;
            DataObject.OTHERCATEGORYNAME = this.OTHERCATEGORYNAME;
            DataObject.PRODUCTIONGROUP = this.PRODUCTIONGROUP;
            DataObject.GROSSWEIGHT = this.GROSSWEIGHT;
            DataObject.WEIGHTUNIT = this.WEIGHTUNIT;
            DataObject.ASSEMBLYCODE = this.ASSEMBLYCODE;
            DataObject.UPCCODE2 = this.UPCCODE2;
            DataObject.MRPGROUP = this.MRPGROUP;
            DataObject.UOMOFDIM = this.UOMOFDIM;
            DataObject.SERIALPROFILE = this.SERIALPROFILE;
            DataObject.COMMODITYCODE = this.COMMODITYCODE;
            DataObject.PROCUREMENTTYPE = this.PROCUREMENTTYPE;
            DataObject.KITSLOC = this.KITSLOC;
            DataObject.OTHERDATE = this.OTHERDATE;
            DataObject.OTHERTIME = this.OTHERTIME;
            DataObject.BACKFLUSHFLAG = this.BACKFLUSHFLAG;
            DataObject.BUYERCODE = this.BUYERCODE;
            DataObject.POST_TO_INSP = this.POST_TO_INSP;
            DataObject.STORAGE_BIN = this.STORAGE_BIN;
            DataObject.MEMO = this.MEMO;
            DataObject.IND_STD_DESC = this.IND_STD_DESC;
            DataObject.EXT_MAT_GRP = this.EXT_MAT_GRP;
            DataObject.SPEC_PROC = this.SPEC_PROC;
            DataObject.DEL_FG_VAL_TYPE = this.DEL_FG_VAL_TYPE;
            DataObject.BASE_QTY = this.BASE_QTY;
            DataObject.PROD_ALLOCATION = this.PROD_ALLOCATION;
            DataObject.MATERIAL_GROUP_3 = this.MATERIAL_GROUP_3;
            DataObject.PROFIT_CENTER = this.PROFIT_CENTER;
            DataObject.GR_PROC_TIME = this.GR_PROC_TIME;
            DataObject.PLND_DLV_TIME = this.PLND_DLV_TIME;
            DataObject.MRP_TYPE = this.MRP_TYPE;
            DataObject.ROP = this.ROP;
            return DataObject;
        }
        public string LOT_SIZE
        {
            get
            {
                return (string)this["LOT_SIZE"];
            }
            set
            {
                this["LOT_SIZE"] = value;
            }
        }
        public string MIN_LOT_SIZE
        {
            get
            {
                return (string)this["MIN_LOT_SIZE"];
            }
            set
            {
                this["MIN_LOT_SIZE"] = value;
            }
        }
        public string PLANTID
        {
            get
            {
                return (string)this["PLANTID"];
            }
            set
            {
                this["PLANTID"] = value;
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
        public string PARTNAME
        {
            get
            {
                return (string)this["PARTNAME"];
            }
            set
            {
                this["PARTNAME"] = value;
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
        public string ALTERNATENAME
        {
            get
            {
                return (string)this["ALTERNATENAME"];
            }
            set
            {
                this["ALTERNATENAME"] = value;
            }
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
        public string PRODUCTTYPE
        {
            get
            {
                return (string)this["PRODUCTTYPE"];
            }
            set
            {
                this["PRODUCTTYPE"] = value;
            }
        }
        public string MATERIALTYPE
        {
            get
            {
                return (string)this["MATERIALTYPE"];
            }
            set
            {
                this["MATERIALTYPE"] = value;
            }
        }
        public string SOURCETYPE
        {
            get
            {
                return (string)this["SOURCETYPE"];
            }
            set
            {
                this["SOURCETYPE"] = value;
            }
        }
        public string VENDORCODE
        {
            get
            {
                return (string)this["VENDORCODE"];
            }
            set
            {
                this["VENDORCODE"] = value;
            }
        }
        public string UPCCODE
        {
            get
            {
                return (string)this["UPCCODE"];
            }
            set
            {
                this["UPCCODE"] = value;
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
        public string TAXEXEMPT
        {
            get
            {
                return (string)this["TAXEXEMPT"];
            }
            set
            {
                this["TAXEXEMPT"] = value;
            }
        }
        public string TAXCODE
        {
            get
            {
                return (string)this["TAXCODE"];
            }
            set
            {
                this["TAXCODE"] = value;
            }
        }
        public string MAINWHID
        {
            get
            {
                return (string)this["MAINWHID"];
            }
            set
            {
                this["MAINWHID"] = value;
            }
        }
        public string UOM
        {
            get
            {
                return (string)this["UOM"];
            }
            set
            {
                this["UOM"] = value;
            }
        }
        public double? WEIGHT
        {
            get
            {
                return (double?)this["WEIGHT"];
            }
            set
            {
                this["WEIGHT"] = value;
            }
        }
        public double? LENGETH
        {
            get
            {
                return (double?)this["LENGETH"];
            }
            set
            {
                this["LENGETH"] = value;
            }
        }
        public double? WIDTH
        {
            get
            {
                return (double?)this["WIDTH"];
            }
            set
            {
                this["WIDTH"] = value;
            }
        }
        public double? HEIGHT
        {
            get
            {
                return (double?)this["HEIGHT"];
            }
            set
            {
                this["HEIGHT"] = value;
            }
        }
        public string BOMITEM
        {
            get
            {
                return (string)this["BOMITEM"];
            }
            set
            {
                this["BOMITEM"] = value;
            }
        }
        public string VIRTUAL
        {
            get
            {
                return (string)this["VIRTUAL"];
            }
            set
            {
                this["VIRTUAL"] = value;
            }
        }
        public string JITITEM
        {
            get
            {
                return (string)this["JITITEM"];
            }
            set
            {
                this["JITITEM"] = value;
            }
        }
        public string OOD
        {
            get
            {
                return (string)this["OOD"];
            }
            set
            {
                this["OOD"] = value;
            }
        }
        public DateTime? OODDATE
        {
            get
            {
                return (DateTime?)this["OODDATE"];
            }
            set
            {
                this["OODDATE"] = value;
            }
        }
        public string ISMATERIAL
        {
            get
            {
                return (string)this["ISMATERIAL"];
            }
            set
            {
                this["ISMATERIAL"] = value;
            }
        }
        public string CONSUMPTIONITEM
        {
            get
            {
                return (string)this["CONSUMPTIONITEM"];
            }
            set
            {
                this["CONSUMPTIONITEM"] = value;
            }
        }
        public string PACKMATERIAL
        {
            get
            {
                return (string)this["PACKMATERIAL"];
            }
            set
            {
                this["PACKMATERIAL"] = value;
            }
        }
        public string CURRENCYTYPE
        {
            get
            {
                return (string)this["CURRENCYTYPE"];
            }
            set
            {
                this["CURRENCYTYPE"] = value;
            }
        }
        public double? STDCOST
        {
            get
            {
                return (double?)this["STDCOST"];
            }
            set
            {
                this["STDCOST"] = value;
            }
        }
        public double? STDPRICE
        {
            get
            {
                return (double?)this["STDPRICE"];
            }
            set
            {
                this["STDPRICE"] = value;
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
        public double? UNITPRICE
        {
            get
            {
                return (double?)this["UNITPRICE"];
            }
            set
            {
                this["UNITPRICE"] = value;
            }
        }
        public string GRAPHICLINK
        {
            get
            {
                return (string)this["GRAPHICLINK"];
            }
            set
            {
                this["GRAPHICLINK"] = value;
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
        public string FIELD2
        {
            get
            {
                return (string)this["FIELD2"];
            }
            set
            {
                this["FIELD2"] = value;
            }
        }
        public string FIELD3
        {
            get
            {
                return (string)this["FIELD3"];
            }
            set
            {
                this["FIELD3"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? CREATEDATE
        {
            get
            {
                return (DateTime?)this["CREATEDATE"];
            }
            set
            {
                this["CREATEDATE"] = value;
            }
        }
        public string CUSTVERSION
        {
            get
            {
                return (string)this["CUSTVERSION"];
            }
            set
            {
                this["CUSTVERSION"] = value;
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
        public string ABCCODE
        {
            get
            {
                return (string)this["ABCCODE"];
            }
            set
            {
                this["ABCCODE"] = value;
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
        public string RFA
        {
            get
            {
                return (string)this["RFA"];
            }
            set
            {
                this["RFA"] = value;
            }
        }
        public string OTHERCATEGORYNAME
        {
            get
            {
                return (string)this["OTHERCATEGORYNAME"];
            }
            set
            {
                this["OTHERCATEGORYNAME"] = value;
            }
        }
        public string PRODUCTIONGROUP
        {
            get
            {
                return (string)this["PRODUCTIONGROUP"];
            }
            set
            {
                this["PRODUCTIONGROUP"] = value;
            }
        }
        public double? GROSSWEIGHT
        {
            get
            {
                return (double?)this["GROSSWEIGHT"];
            }
            set
            {
                this["GROSSWEIGHT"] = value;
            }
        }
        public string WEIGHTUNIT
        {
            get
            {
                return (string)this["WEIGHTUNIT"];
            }
            set
            {
                this["WEIGHTUNIT"] = value;
            }
        }
        public string ASSEMBLYCODE
        {
            get
            {
                return (string)this["ASSEMBLYCODE"];
            }
            set
            {
                this["ASSEMBLYCODE"] = value;
            }
        }
        public string UPCCODE2
        {
            get
            {
                return (string)this["UPCCODE2"];
            }
            set
            {
                this["UPCCODE2"] = value;
            }
        }
        public string MRPGROUP
        {
            get
            {
                return (string)this["MRPGROUP"];
            }
            set
            {
                this["MRPGROUP"] = value;
            }
        }
        public string UOMOFDIM
        {
            get
            {
                return (string)this["UOMOFDIM"];
            }
            set
            {
                this["UOMOFDIM"] = value;
            }
        }
        public string SERIALPROFILE
        {
            get
            {
                return (string)this["SERIALPROFILE"];
            }
            set
            {
                this["SERIALPROFILE"] = value;
            }
        }
        public string COMMODITYCODE
        {
            get
            {
                return (string)this["COMMODITYCODE"];
            }
            set
            {
                this["COMMODITYCODE"] = value;
            }
        }
        public string PROCUREMENTTYPE
        {
            get
            {
                return (string)this["PROCUREMENTTYPE"];
            }
            set
            {
                this["PROCUREMENTTYPE"] = value;
            }
        }
        public string KITSLOC
        {
            get
            {
                return (string)this["KITSLOC"];
            }
            set
            {
                this["KITSLOC"] = value;
            }
        }
        public string OTHERDATE
        {
            get
            {
                return (string)this["OTHERDATE"];
            }
            set
            {
                this["OTHERDATE"] = value;
            }
        }
        public string OTHERTIME
        {
            get
            {
                return (string)this["OTHERTIME"];
            }
            set
            {
                this["OTHERTIME"] = value;
            }
        }
        public string BACKFLUSHFLAG
        {
            get
            {
                return (string)this["BACKFLUSHFLAG"];
            }
            set
            {
                this["BACKFLUSHFLAG"] = value;
            }
        }
        public string BUYERCODE
        {
            get
            {
                return (string)this["BUYERCODE"];
            }
            set
            {
                this["BUYERCODE"] = value;
            }
        }
        public string POST_TO_INSP
        {
            get
            {
                return (string)this["POST_TO_INSP"];
            }
            set
            {
                this["POST_TO_INSP"] = value;
            }
        }
        public string STORAGE_BIN
        {
            get
            {
                return (string)this["STORAGE_BIN"];
            }
            set
            {
                this["STORAGE_BIN"] = value;
            }
        }
        public string MEMO
        {
            get
            {
                return (string)this["MEMO"];
            }
            set
            {
                this["MEMO"] = value;
            }
        }
        public string IND_STD_DESC
        {
            get
            {
                return (string)this["IND_STD_DESC"];
            }
            set
            {
                this["IND_STD_DESC"] = value;
            }
        }
        public string EXT_MAT_GRP
        {
            get
            {
                return (string)this["EXT_MAT_GRP"];
            }
            set
            {
                this["EXT_MAT_GRP"] = value;
            }
        }
        public string SPEC_PROC
        {
            get
            {
                return (string)this["SPEC_PROC"];
            }
            set
            {
                this["SPEC_PROC"] = value;
            }
        }
        public string DEL_FG_VAL_TYPE
        {
            get
            {
                return (string)this["DEL_FG_VAL_TYPE"];
            }
            set
            {
                this["DEL_FG_VAL_TYPE"] = value;
            }
        }
        public string BASE_QTY
        {
            get
            {
                return (string)this["BASE_QTY"];
            }
            set
            {
                this["BASE_QTY"] = value;
            }
        }
        public string PROD_ALLOCATION
        {
            get
            {
                return (string)this["PROD_ALLOCATION"];
            }
            set
            {
                this["PROD_ALLOCATION"] = value;
            }
        }
        public string MATERIAL_GROUP_3
        {
            get
            {
                return (string)this["MATERIAL_GROUP_3"];
            }
            set
            {
                this["MATERIAL_GROUP_3"] = value;
            }
        }
        public string PROFIT_CENTER
        {
            get
            {
                return (string)this["PROFIT_CENTER"];
            }
            set
            {
                this["PROFIT_CENTER"] = value;
            }
        }
        public string GR_PROC_TIME
        {
            get
            {
                return (string)this["GR_PROC_TIME"];
            }
            set
            {
                this["GR_PROC_TIME"] = value;
            }
        }
        public string PLND_DLV_TIME
        {
            get
            {
                return (string)this["PLND_DLV_TIME"];
            }
            set
            {
                this["PLND_DLV_TIME"] = value;
            }
        }
        public string MRP_TYPE
        {
            get
            {
                return (string)this["MRP_TYPE"];
            }
            set
            {
                this["MRP_TYPE"] = value;
            }
        }
        public string ROP
        {
            get
            {
                return (string)this["ROP"];
            }
            set
            {
                this["ROP"] = value;
            }
        }
    }
    public class C_MMPRODMASTER
    {
        public string PARTNO { get; set; }
        public string LOT_SIZE { get; set; }
        public string MIN_LOT_SIZE { get; set; }
        public string PLANTID { get; set; }
        public string CATEGORYNAME { get; set; }
        public string PARTNAME { get; set; }
        public string VERSION { get; set; }
        public string ALTERNATENAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string MATERIALTYPE { get; set; }
        public string SOURCETYPE { get; set; }
        public string VENDORCODE { get; set; }
        public string UPCCODE { get; set; }
        public string EEECODE { get; set; }
        public string TAXEXEMPT { get; set; }
        public string TAXCODE { get; set; }
        public string MAINWHID { get; set; }
        public string UOM { get; set; }
        public double? WEIGHT { get; set; }
        public double? LENGETH { get; set; }
        public double? WIDTH { get; set; }
        public double? HEIGHT { get; set; }
        public string BOMITEM { get; set; }
        public string VIRTUAL { get; set; }
        public string JITITEM { get; set; }
        public string OOD { get; set; }
        public DateTime? OODDATE { get; set; }
        public string ISMATERIAL { get; set; }
        public string CONSUMPTIONITEM { get; set; }
        public string PACKMATERIAL { get; set; }
        public string CURRENCYTYPE { get; set; }
        public double? STDCOST { get; set; }
        public double? STDPRICE { get; set; }
        public double? UNITCOST { get; set; }
        public double? UNITPRICE { get; set; }
        public string GRAPHICLINK { get; set; }
        public string FIELD1 { get; set; }
        public string FIELD2 { get; set; }
        public string FIELD3 { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string CUSTVERSION { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string ABCCODE { get; set; }
        public string KEYPART { get; set; }
        public string RFA { get; set; }
        public string OTHERCATEGORYNAME { get; set; }
        public string PRODUCTIONGROUP { get; set; }
        public double? GROSSWEIGHT { get; set; }
        public string WEIGHTUNIT { get; set; }
        public string ASSEMBLYCODE { get; set; }
        public string UPCCODE2 { get; set; }
        public string MRPGROUP { get; set; }
        public string UOMOFDIM { get; set; }
        public string SERIALPROFILE { get; set; }
        public string COMMODITYCODE { get; set; }
        public string PROCUREMENTTYPE { get; set; }
        public string KITSLOC { get; set; }
        public string OTHERDATE { get; set; }
        public string OTHERTIME { get; set; }
        public string BACKFLUSHFLAG { get; set; }
        public string BUYERCODE { get; set; }
        public string POST_TO_INSP { get; set; }
        public string STORAGE_BIN { get; set; }
        public string MEMO { get; set; }
        public string IND_STD_DESC { get; set; }
        public string EXT_MAT_GRP { get; set; }
        public string SPEC_PROC { get; set; }
        public string DEL_FG_VAL_TYPE { get; set; }
        public string BASE_QTY { get; set; }
        public string PROD_ALLOCATION { get; set; }
        public string MATERIAL_GROUP_3 { get; set; }
        public string PROFIT_CENTER { get; set; }
        public string GR_PROC_TIME { get; set; }
        public string PLND_DLV_TIME { get; set; }
        public string MRP_TYPE { get; set; }
        public string ROP { get; set; }
    }
}