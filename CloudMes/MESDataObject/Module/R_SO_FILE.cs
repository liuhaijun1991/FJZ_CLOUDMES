using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SO_FILE : DataObjectTable
    {
        public T_R_SO_FILE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SO_FILE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SO_FILE);
            TableName = "R_SO_FILE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SO_FILE : DataObjectBase
    {
        public Row_R_SO_FILE(DataObjectInfo info) : base(info)
        {

        }
        public R_SO_FILE GetDataObject()
        {
            R_SO_FILE DataObject = new R_SO_FILE();
            DataObject.ID = this.ID;
            DataObject.VBELN = this.VBELN;
            DataObject.POSNR = this.POSNR;
            DataObject.MATNR = this.MATNR;
            DataObject.BSTNK = this.BSTNK;
            DataObject.KWMENG = this.KWMENG;
            DataObject.CMPRE = this.CMPRE;
            DataObject.KUNNR = this.KUNNR;
            DataObject.KUNNV = this.KUNNV;
            DataObject.ARKTX = this.ARKTX;
            DataObject.NAME = this.NAME;
            DataObject.LAND1 = this.LAND1;
            DataObject.NETPR = this.NETPR;
            DataObject.STATUS = this.STATUS;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_DATE = this.EDIT_DATE;
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
        public string BSTNK
        {
            get
            {
                return (string)this["BSTNK"];
            }
            set
            {
                this["BSTNK"] = value;
            }
        }
        public string KWMENG
        {
            get
            {
                return (string)this["KWMENG"];
            }
            set
            {
                this["KWMENG"] = value;
            }
        }
        public string CMPRE
        {
            get
            {
                return (string)this["CMPRE"];
            }
            set
            {
                this["CMPRE"] = value;
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
        public string KUNNV
        {
            get
            {
                return (string)this["KUNNV"];
            }
            set
            {
                this["KUNNV"] = value;
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
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
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
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_DATE
        {
            get
            {
                return (DateTime?)this["EDIT_DATE"];
            }
            set
            {
                this["EDIT_DATE"] = value;
            }
        }
    }
    public class R_SO_FILE
    {
        public string ID { get; set; }
        public string VBELN { get; set; }
        public string POSNR { get; set; }
        public string MATNR { get; set; }
        public string BSTNK { get; set; }
        public string KWMENG { get; set; }
        public string CMPRE { get; set; }
        public string KUNNR { get; set; }
        public string KUNNV { get; set; }
        public string ARKTX { get; set; }
        public string NAME { get; set; }
        public string LAND1 { get; set; }
        public string NETPR { get; set; }
        public string STATUS { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_DATE { get; set; }
    }
    public class R_SO_FILE_Status
    {
        public static  string WaitToConvert = "WaitToConvert";
        public static string WaitToShip = "WaitToShip";
        public static string ShipFinish = "ShipFinish";
        public static string WaitToDN = "WaitToDN";
    }
}