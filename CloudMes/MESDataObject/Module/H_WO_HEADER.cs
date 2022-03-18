using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_WO_HEADER : DataObjectTable
    {
        public T_H_WO_HEADER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_WO_HEADER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_WO_HEADER);
            TableName = "H_WO_HEADER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_H_WO_HEADER : DataObjectBase
    {
        public Row_H_WO_HEADER(DataObjectInfo info) : base(info)
        {

        }
        public H_WO_HEADER GetDataObject()
        {
            H_WO_HEADER DataObject = new H_WO_HEADER();
            DataObject.GLTRP = this.GLTRP;
            DataObject.AETXT = this.AETXT;
            DataObject.SAENR = this.SAENR;
            DataObject.CHARG = this.CHARG;
            DataObject.BISMT = this.BISMT;
            DataObject.WEMNG = this.WEMNG;
            DataObject.MVGR3 = this.MVGR3;
            DataObject.FTRMI = this.FTRMI;
            DataObject.ROHS_VALUE = this.ROHS_VALUE;
            DataObject.ABLAD = this.ABLAD;
            DataObject.LGORT = this.LGORT;
            DataObject.GLUPS = this.GLUPS;
            DataObject.GLTRS = this.GLTRS;
            DataObject.ERFZEIT = this.ERFZEIT;
            DataObject.GSUPS = this.GSUPS;
            DataObject.ERDAT = this.ERDAT;
            DataObject.MAKTX = this.MAKTX;
            DataObject.MATKL = this.MATKL;
            DataObject.AENAM = this.AENAM;
            DataObject.AEDAT = this.AEDAT;
            DataObject.KDMAT = this.KDMAT;
            DataObject.GAMNG = this.GAMNG;
            DataObject.GSTRS = this.GSTRS;
            DataObject.KDAUF = this.KDAUF;
            DataObject.REVLV = this.REVLV;
            DataObject.MATNR = this.MATNR;
            DataObject.AUART = this.AUART;
            DataObject.WERKS = this.WERKS;
            DataObject.AUFNR = this.AUFNR;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string GLTRP
        {
            get
            {
                return (string)this["GLTRP"];
            }
            set
            {
                this["GLTRP"] = value;
            }
        }
        public string AETXT
        {
            get
            {
                return (string)this["AETXT"];
            }
            set
            {
                this["AETXT"] = value;
            }
        }
        public string SAENR
        {
            get
            {
                return (string)this["SAENR"];
            }
            set
            {
                this["SAENR"] = value;
            }
        }
        public string CHARG
        {
            get
            {
                return (string)this["CHARG"];
            }
            set
            {
                this["CHARG"] = value;
            }
        }
        public string BISMT
        {
            get
            {
                return (string)this["BISMT"];
            }
            set
            {
                this["BISMT"] = value;
            }
        }
        public string WEMNG
        {
            get
            {
                return (string)this["WEMNG"];
            }
            set
            {
                this["WEMNG"] = value;
            }
        }
        public string MVGR3
        {
            get
            {
                return (string)this["MVGR3"];
            }
            set
            {
                this["MVGR3"] = value;
            }
        }
        public string FTRMI
        {
            get
            {
                return (string)this["FTRMI"];
            }
            set
            {
                this["FTRMI"] = value;
            }
        }
        public string ROHS_VALUE
        {
            get
            {
                return (string)this["ROHS_VALUE"];
            }
            set
            {
                this["ROHS_VALUE"] = value;
            }
        }
        public string ABLAD
        {
            get
            {
                return (string)this["ABLAD"];
            }
            set
            {
                this["ABLAD"] = value;
            }
        }
        public string LGORT
        {
            get
            {
                return (string)this["LGORT"];
            }
            set
            {
                this["LGORT"] = value;
            }
        }
        public string GLUPS
        {
            get
            {
                return (string)this["GLUPS"];
            }
            set
            {
                this["GLUPS"] = value;
            }
        }
        public string GLTRS
        {
            get
            {
                return (string)this["GLTRS"];
            }
            set
            {
                this["GLTRS"] = value;
            }
        }
        public string ERFZEIT
        {
            get
            {
                return (string)this["ERFZEIT"];
            }
            set
            {
                this["ERFZEIT"] = value;
            }
        }
        public string GSUPS
        {
            get
            {
                return (string)this["GSUPS"];
            }
            set
            {
                this["GSUPS"] = value;
            }
        }
        public string ERDAT
        {
            get
            {
                return (string)this["ERDAT"];
            }
            set
            {
                this["ERDAT"] = value;
            }
        }
        public string MAKTX
        {
            get
            {
                return (string)this["MAKTX"];
            }
            set
            {
                this["MAKTX"] = value;
            }
        }
        public string MATKL
        {
            get
            {
                return (string)this["MATKL"];
            }
            set
            {
                this["MATKL"] = value;
            }
        }
        public string AENAM
        {
            get
            {
                return (string)this["AENAM"];
            }
            set
            {
                this["AENAM"] = value;
            }
        }
        public string AEDAT
        {
            get
            {
                return (string)this["AEDAT"];
            }
            set
            {
                this["AEDAT"] = value;
            }
        }
        public string KDMAT
        {
            get
            {
                return (string)this["KDMAT"];
            }
            set
            {
                this["KDMAT"] = value;
            }
        }
        public string GAMNG
        {
            get
            {
                return (string)this["GAMNG"];
            }
            set
            {
                this["GAMNG"] = value;
            }
        }
        public string GSTRS
        {
            get
            {
                return (string)this["GSTRS"];
            }
            set
            {
                this["GSTRS"] = value;
            }
        }
        public string KDAUF
        {
            get
            {
                return (string)this["KDAUF"];
            }
            set
            {
                this["KDAUF"] = value;
            }
        }
        public string REVLV
        {
            get
            {
                return (string)this["REVLV"];
            }
            set
            {
                this["REVLV"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string AUART
        {
            get
            {
                return (string)this["AUART"];
            }
            set
            {
                this["AUART"] = value;
            }
        }
        public string WERKS
        {
            get
            {
                return (string)this["WERKS"];
            }
            set
            {
                this["WERKS"] = value;
            }
        }
        public string AUFNR
        {
            get
            {
                return (string)this["AUFNR"];
            }
            set
            {
                this["AUFNR"] = value;
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
    }
    public class H_WO_HEADER
    {
        public string GLTRP{get;set;}
        public string AETXT{get;set;}
        public string SAENR{get;set;}
        public string CHARG{get;set;}
        public string BISMT{get;set;}
        public string WEMNG{get;set;}
        public string MVGR3{get;set;}
        public string FTRMI{get;set;}
        public string ROHS_VALUE{get;set;}
        public string ABLAD{get;set;}
        public string LGORT{get;set;}
        public string GLUPS{get;set;}
        public string GLTRS{get;set;}
        public string ERFZEIT{get;set;}
        public string GSUPS{get;set;}
        public string ERDAT{get;set;}
        public string MAKTX{get;set;}
        public string MATKL{get;set;}
        public string AENAM{get;set;}
        public string AEDAT{get;set;}
        public string KDMAT{get;set;}
        public string GAMNG{get;set;}
        public string GSTRS{get;set;}
        public string KDAUF{get;set;}
        public string REVLV{get;set;}
        public string MATNR{get;set;}
        public string AUART{get;set;}
        public string WERKS{get;set;}
        public string AUFNR{get;set;}
        public string ID{get;set;}
    }
}