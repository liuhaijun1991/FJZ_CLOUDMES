using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_PO_CONFIRM_HEAD : DataObjectTable
    {
        public T_PO_CONFIRM_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_PO_CONFIRM_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_PO_CONFIRM_HEAD);
            TableName = "PO_CONFIRM_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_PO_CONFIRM_HEAD : DataObjectBase
    {
        public Row_PO_CONFIRM_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public PO_CONFIRM_HEAD GetDataObject()
        {
            PO_CONFIRM_HEAD DataObject = new PO_CONFIRM_HEAD();
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CREATIONDATETIME = this.CREATIONDATETIME;
            DataObject.PO = this.PO;
            DataObject.VENDORID = this.VENDORID;
            DataObject.POITEM = this.POITEM;
            DataObject.PN = this.PN;
            DataObject.PLANT = this.PLANT;
            DataObject.DELIVERYDATE = this.DELIVERYDATE;
            DataObject.SO = this.SO;
            DataObject.SOITEM = this.SOITEM;
            DataObject.UNITCODE = this.UNITCODE;
            DataObject.QTY = this.QTY;
            DataObject.REASONCODE = this.REASONCODE;
            DataObject.LOAD_CDB_FLAG = this.LOAD_CDB_FLAG;
            DataObject.LOAD_CDB_TIME = this.LOAD_CDB_TIME;
            DataObject.CREATEDT = this.CREATEDT;
            DataObject.STD_OUT_FILE_NAME = this.STD_OUT_FILE_NAME;
            return DataObject;
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
        public string CREATIONDATETIME
        {
            get
            {
                return (string)this["CREATIONDATETIME"];
            }
            set
            {
                this["CREATIONDATETIME"] = value;
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
        public string VENDORID
        {
            get
            {
                return (string)this["VENDORID"];
            }
            set
            {
                this["VENDORID"] = value;
            }
        }
        public string POITEM
        {
            get
            {
                return (string)this["POITEM"];
            }
            set
            {
                this["POITEM"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string DELIVERYDATE
        {
            get
            {
                return (string)this["DELIVERYDATE"];
            }
            set
            {
                this["DELIVERYDATE"] = value;
            }
        }
        public string SO
        {
            get
            {
                return (string)this["SO"];
            }
            set
            {
                this["SO"] = value;
            }
        }
        public string SOITEM
        {
            get
            {
                return (string)this["SOITEM"];
            }
            set
            {
                this["SOITEM"] = value;
            }
        }
        public string UNITCODE
        {
            get
            {
                return (string)this["UNITCODE"];
            }
            set
            {
                this["UNITCODE"] = value;
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
        public string REASONCODE
        {
            get
            {
                return (string)this["REASONCODE"];
            }
            set
            {
                this["REASONCODE"] = value;
            }
        }
        public string LOAD_CDB_FLAG
        {
            get
            {
                return (string)this["LOAD_CDB_FLAG"];
            }
            set
            {
                this["LOAD_CDB_FLAG"] = value;
            }
        }
        public string LOAD_CDB_TIME
        {
            get
            {
                return (string)this["LOAD_CDB_TIME"];
            }
            set
            {
                this["LOAD_CDB_TIME"] = value;
            }
        }
        public string CREATEDT
        {
            get
            {
                return (string)this["CREATEDT"];
            }
            set
            {
                this["CREATEDT"] = value;
            }
        }
        public string STD_OUT_FILE_NAME
        {
            get
            {
                return (string)this["STD_OUT_FILE_NAME"];
            }
            set
            {
                this["STD_OUT_FILE_NAME"] = value;
            }
        }
    }
    public class PO_CONFIRM_HEAD
    {
        public string MESSAGEID { get; set; }
        public string CREATIONDATETIME { get; set; }
        public string PO { get; set; }
        public string VENDORID { get; set; }
        public string POITEM { get; set; }
        public string PN { get; set; }
        public string PLANT { get; set; }
        public string DELIVERYDATE { get; set; }
        public string SO { get; set; }
        public string SOITEM { get; set; }
        public string UNITCODE { get; set; }
        public string QTY { get; set; }
        public string REASONCODE { get; set; }
        public string LOAD_CDB_FLAG { get; set; }
        public string LOAD_CDB_TIME { get; set; }
        public string CREATEDT { get; set; }
        public string STD_OUT_FILE_NAME { get; set; }
    }
}