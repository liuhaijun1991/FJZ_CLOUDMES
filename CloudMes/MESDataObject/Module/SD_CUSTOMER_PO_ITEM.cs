using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SD_CUSTOMER_PO_ITEM : DataObjectTable
    {
        public T_SD_CUSTOMER_PO_ITEM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SD_CUSTOMER_PO_ITEM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SD_CUSTOMER_PO_ITEM);
            TableName = "SD_CUSTOMER_PO_ITEM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_SD_CUSTOMER_PO_ITEM : DataObjectBase
    {
        public Row_SD_CUSTOMER_PO_ITEM(DataObjectInfo info) : base(info)
        {

        }
        public SD_CUSTOMER_PO_ITEM GetDataObject()
        {
            SD_CUSTOMER_PO_ITEM DataObject = new SD_CUSTOMER_PO_ITEM();
            DataObject.POSEX = this.POSEX;
            DataObject.NETPR = this.NETPR;
            DataObject.KWMENG = this.KWMENG;
            DataObject.ARKTX = this.ARKTX;
            DataObject.MATNR = this.MATNR;
            DataObject.POSNR = this.POSNR;
            DataObject.VBELN = this.VBELN;
            return DataObject;
        }
        public string POSEX
        {
            get
            {
                return (string)this["POSEX"];
            }
            set
            {
                this["POSEX"] = value;
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
    public class SD_CUSTOMER_PO_ITEM
    {
        public string POSEX{ get; set; }
        public string NETPR{ get; set; }
        public string KWMENG{ get; set; }
        public string ARKTX{ get; set; }
        public string MATNR{ get; set; }
        public string POSNR{ get; set; }
        public string VBELN{ get; set; }
    }
}