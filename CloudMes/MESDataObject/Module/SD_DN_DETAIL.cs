using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SD_DN_DETAIL : DataObjectTable
    {
        public T_SD_DN_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SD_DN_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SD_DN_DETAIL);
            TableName = "SD_DN_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_SD_DN_DETAIL : DataObjectBase
    {
        public Row_SD_DN_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public SD_DN_DETAIL GetDataObject()
        {
            SD_DN_DETAIL DataObject = new SD_DN_DETAIL();
            DataObject.NETPR = this.NETPR;
            DataObject.ORT01 = this.ORT01;
            DataObject.LAND1 = this.LAND1;
            DataObject.KUNNR = this.KUNNR;
            DataObject.SDABW = this.SDABW;
            DataObject.KDMAT = this.KDMAT;
            DataObject.LGORT = this.LGORT;
            DataObject.MATKL = this.MATKL;
            DataObject.MTART = this.MTART;
            DataObject.GEWEI = this.GEWEI;
            DataObject.ERDAT = this.ERDAT;
            DataObject.VGPOS = this.VGPOS;
            DataObject.VGBEL = this.VGBEL;
            DataObject.LFIMG = this.LFIMG;
            DataObject.KBETR = this.KBETR;
            DataObject.VOLUM = this.VOLUM;
            DataObject.BRGEW = this.BRGEW;
            DataObject.NTGEW = this.NTGEW;
            DataObject.ARKTX = this.ARKTX;
            DataObject.MATNR = this.MATNR;
            DataObject.BSTKD = this.BSTKD;
            DataObject.POSNR = this.POSNR;
            DataObject.VBELN = this.VBELN;
            return DataObject;
        }
        public string NETPR
        {
            get
            {
                return (string)this["NETPR"];
            }
            set
            {
                this["NETPR"] = value;
            }
        }
        public string ORT01
        {
            get
            {
                return (string)this["ORT01"];
            }
            set
            {
                this["ORT01"] = value;
            }
        }
        public string LAND1
        {
            get
            {
                return (string)this["LAND1"];
            }
            set
            {
                this["LAND1"] = value;
            }
        }
        public string KUNNR
        {
            get
            {
                return (string)this["KUNNR"];
            }
            set
            {
                this["KUNNR"] = value;
            }
        }
        public string SDABW
        {
            get
            {
                return (string)this["SDABW"];
            }
            set
            {
                this["SDABW"] = value;
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
        public string MTART
        {
            get
            {
                return (string)this["MTART"];
            }
            set
            {
                this["MTART"] = value;
            }
        }
        public string GEWEI
        {
            get
            {
                return (string)this["GEWEI"];
            }
            set
            {
                this["GEWEI"] = value;
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
        public string VGPOS
        {
            get
            {
                return (string)this["VGPOS"];
            }
            set
            {
                this["VGPOS"] = value;
            }
        }
        public string VGBEL
        {
            get
            {
                return (string)this["VGBEL"];
            }
            set
            {
                this["VGBEL"] = value;
            }
        }
        public string LFIMG
        {
            get
            {
                return (string)this["LFIMG"];
            }
            set
            {
                this["LFIMG"] = value;
            }
        }
        public string KBETR
        {
            get
            {
                return (string)this["KBETR"];
            }
            set
            {
                this["KBETR"] = value;
            }
        }
        public string VOLUM
        {
            get
            {
                return (string)this["VOLUM"];
            }
            set
            {
                this["VOLUM"] = value;
            }
        }
        public string BRGEW
        {
            get
            {
                return (string)this["BRGEW"];
            }
            set
            {
                this["BRGEW"] = value;
            }
        }
        public string NTGEW
        {
            get
            {
                return (string)this["NTGEW"];
            }
            set
            {
                this["NTGEW"] = value;
            }
        }
        public string ARKTX
        {
            get
            {
                return (string)this["ARKTX"];
            }
            set
            {
                this["ARKTX"] = value;
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
        public string BSTKD
        {
            get
            {
                return (string)this["BSTKD"];
            }
            set
            {
                this["BSTKD"] = value;
            }
        }
        public string POSNR
        {
            get
            {
                return (string)this["POSNR"];
            }
            set
            {
                this["POSNR"] = value;
            }
        }
        public string VBELN
        {
            get
            {
                return (string)this["VBELN"];
            }
            set
            {
                this["VBELN"] = value;
            }
        }
    }
    public class SD_DN_DETAIL
    {
        public string NETPR{ get; set; }
        public string ORT01{ get; set; }
        public string LAND1{ get; set; }
        public string KUNNR{ get; set; }
        public string SDABW{ get; set; }
        public string KDMAT{ get; set; }
        public string LGORT{ get; set; }
        public string MATKL{ get; set; }
        public string MTART{ get; set; }
        public string GEWEI{ get; set; }
        public string ERDAT{ get; set; }
        public string VGPOS{ get; set; }
        public string VGBEL{ get; set; }
        public string LFIMG{ get; set; }
        public string KBETR{ get; set; }
        public string VOLUM{ get; set; }
        public string BRGEW{ get; set; }
        public string NTGEW{ get; set; }
        public string ARKTX{ get; set; }
        public string MATNR{ get; set; }
        public string BSTKD{ get; set; }
        public string POSNR{ get; set; }
        public string VBELN{ get; set; }
    }
}