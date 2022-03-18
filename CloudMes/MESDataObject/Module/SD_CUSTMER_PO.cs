using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SD_CUSTMER_PO : DataObjectTable
    {
        public T_SD_CUSTMER_PO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SD_CUSTMER_PO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SD_CUSTMER_PO);
            TableName = "SD_CUSTMER_PO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_SD_CUSTMER_PO : DataObjectBase
    {
        public Row_SD_CUSTMER_PO(DataObjectInfo info) : base(info)
        {

        }
        public SD_CUSTMER_PO GetDataObject()
        {
            SD_CUSTMER_PO DataObject = new SD_CUSTMER_PO();
            DataObject.BSTDK = this.BSTDK;
            DataObject.AUART = this.AUART;
            DataObject.BSTNK = this.BSTNK;
            DataObject.NAME1 = this.NAME1;
            DataObject.KUNNR = this.KUNNR;
            DataObject.ERDAT = this.ERDAT;
            DataObject.VBELN = this.VBELN;
            return DataObject;
        }
        public string BSTDK
        {
            get
            {
                return (string)this["BSTDK"];
            }
            set
            {
                this["BSTDK"] = value;
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
        public string NAME1
        {
            get
            {
                return (string)this["NAME1"];
            }
            set
            {
                this["NAME1"] = value;
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
    public class SD_CUSTMER_PO
    {
        public string BSTDK { get; set; }
        public string AUART { get; set; }
        public string BSTNK { get; set; }
        public string NAME1 { get; set; }
        public string KUNNR { get; set; }
        public string ERDAT { get; set; }
        public string VBELN { get; set; }
    }
}