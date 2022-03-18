using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_PO_LINE : DataObjectTable
    {
        public T_PO_LINE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_PO_LINE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_PO_LINE);
            TableName = "PO_LINE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_PO_LINE : DataObjectBase
    {
        public Row_PO_LINE(DataObjectInfo info) : base(info)
        {

        }
        public PO_LINE GetDataObject()
        {
            PO_LINE DataObject = new PO_LINE();
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CUSTOMERPRODID = this.CUSTOMERPRODID;
            DataObject.PO = this.PO;
            DataObject.LINENO = this.LINENO;
            DataObject.SALESORDERLINEITEM = this.SALESORDERLINEITEM;
            DataObject.PARTNO = this.PARTNO;
            DataObject.UOM = this.UOM;
            DataObject.QTY = this.QTY;
            return DataObject;
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
        public string CUSTOMERPRODID
        {
            get
            {
                return (string)this["CUSTOMERPRODID"];
            }
            set
            {
                this["CUSTOMERPRODID"] = value;
            }
        }
        public string PO
        {
            get
            {
                return (string)this["PO"];
            }
            set
            {
                this["PO"] = value;
            }
        }
        public string LINENO
        {
            get
            {
                return (string)this["LINENO"];
            }
            set
            {
                this["LINENO"] = value;
            }
        }
        public string SALESORDERLINEITEM
        {
            get
            {
                return (string)this["SALESORDERLINEITEM"];
            }
            set
            {
                this["SALESORDERLINEITEM"] = value;
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
    }
    public class PO_LINE
    {
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string CUSTOMERPRODID { get; set; }
        public string PO { get; set; }
        public string LINENO { get; set; }
        public string SALESORDERLINEITEM { get; set; }
        public string PARTNO { get; set; }
        public string UOM { get; set; }
        public string QTY { get; set; }
    }
}