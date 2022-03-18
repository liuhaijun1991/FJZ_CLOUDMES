using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SD_TO_HEAD : DataObjectTable
    {
        public T_SD_TO_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SD_TO_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SD_TO_HEAD);
            TableName = "SD_TO_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_SD_TO_HEAD : DataObjectBase
    {
        public Row_SD_TO_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public SD_TO_HEAD GetDataObject()
        {
            SD_TO_HEAD DataObject = new SD_TO_HEAD();
            DataObject.TPBEZ = this.TPBEZ;
            DataObject.SHIPTO = this.SHIPTO;
            DataObject.SHTYP = this.SHTYP;
            DataObject.WERKS = this.WERKS;
            DataObject.KUNAG = this.KUNAG;
            DataObject.EXTI1 = this.EXTI1;
            DataObject.EXTI2 = this.EXTI2;
            DataObject.SIGNI = this.SIGNI;
            DataObject.AEZET = this.AEZET;
            DataObject.ERZET = this.ERZET;
            DataObject.AEDAT = this.AEDAT;
            DataObject.ERDAT = this.ERDAT;
            DataObject.UPABF = this.UPABF;
            DataObject.DPABF = this.DPABF;
            DataObject.UPREG = this.UPREG;
            DataObject.DPREG = this.DPREG;
            DataObject.TKNUM = this.TKNUM;
            return DataObject;
        }
        public string TPBEZ
        {
            get
            {
                return (string)this["TPBEZ"];
            }
            set
            {
                this["TPBEZ"] = value;
            }
        }
        public string ERZET
        {
            get
            {
                return (string)this["ERZET"];
            }
            set
            {
                this["ERZET"] = value;
            }
        }
        public string SHIPTO
        {
            get
            {
                return (string)this["SHIPTO"];
            }
            set
            {
                this["SHIPTO"] = value;
            }
        }
        public string SHTYP
        {
            get
            {
                return (string)this["SHTYP"];
            }
            set
            {
                this["SHTYP"] = value;
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
        public string KUNAG
        {
            get
            {
                return (string)this["KUNAG"];
            }
            set
            {
                this["KUNAG"] = value;
            }
        }
        public string EXTI1
        {
            get
            {
                return (string)this["EXTI1"];
            }
            set
            {
                this["EXTI1"] = value;
            }
        }
        public string EXTI2
        {
            get
            {
                return (string)this["EXTI2"];
            }
            set
            {
                this["EXTI2"] = value;
            }
        }
        public string SIGNI
        {
            get
            {
                return (string)this["SIGNI"];
            }
            set
            {
                this["SIGNI"] = value;
            }
        }
        public string AEZET
        {
            get
            {
                return (string)this["AEZET"];
            }
            set
            {
                this["AEZET"] = value;
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
        public string UPABF
        {
            get
            {
                return (string)this["UPABF"];
            }
            set
            {
                this["UPABF"] = value;
            }
        }
        public string DPABF
        {
            get
            {
                return (string)this["DPABF"];
            }
            set
            {
                this["DPABF"] = value;
            }
        }
        public string UPREG
        {
            get
            {
                return (string)this["UPREG"];
            }
            set
            {
                this["UPREG"] = value;
            }
        }
        public string DPREG
        {
            get
            {
                return (string)this["DPREG"];
            }
            set
            {
                this["DPREG"] = value;
            }
        }
        public string TKNUM
        {
            get
            {
                return (string)this["TKNUM"];
            }
            set
            {
                this["TKNUM"] = value;
            }
        }
    }
    public class SD_TO_HEAD
    {
        public string TPBEZ{ get; set; }
        public string SHIPTO{ get; set; }
        public string SHTYP{ get; set; }
        public string WERKS{ get; set; }
        public string KUNAG{ get; set; }
        public string EXTI1{ get; set; }
        public string EXTI2{ get; set; }
        public string SIGNI{ get; set; }
        public string AEZET{ get; set; }
        public string AEDAT{ get; set; }
        public string ERDAT{ get; set; }
        public string ERZET { get; set; }
        public string UPABF{ get; set; }
        public string DPABF{ get; set; }
        public string UPREG{ get; set; }
        public string DPREG{ get; set; }
        public string TKNUM{ get; set; }
    }
}