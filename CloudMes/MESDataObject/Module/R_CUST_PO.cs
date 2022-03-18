using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_CUST_PO : DataObjectTable
    {
        public T_R_CUST_PO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CUST_PO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CUST_PO);
            TableName = "R_CUST_PO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_CUST_PO : DataObjectBase
    {
        public Row_R_CUST_PO(DataObjectInfo info) : base(info)
        {

        }
        public R_CUST_PO GetDataObject()
        {
            R_CUST_PO DataObject = new R_CUST_PO();
            DataObject.ID = this.ID;
            DataObject.CUST_PO_NO = this.CUST_PO_NO;
            DataObject.C_CUST_ID = this.C_CUST_ID;
            DataObject.BILL_TO_CODE = this.BILL_TO_CODE;
            DataObject.STATUS = this.STATUS;
            DataObject.PO_FILE_TYPE = this.PO_FILE_TYPE;
            DataObject.PO_FILE_DESC = this.PO_FILE_DESC;
            DataObject.PO_FILE_ID = this.PO_FILE_ID;
            DataObject.EDIT_DATE = this.EDIT_DATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string CUST_PO_NO
        {
            get
            {
                return (string)this["CUST_PO_NO"];
            }
            set
            {
                this["CUST_PO_NO"] = value;
            }
        }
        public string C_CUST_ID
        {
            get
            {
                return (string)this["C_CUST_ID"];
            }
            set
            {
                this["C_CUST_ID"] = value;
            }
        }
        public string BILL_TO_CODE
        {
            get
            {
                return (string)this["BILL_TO_CODE"];
            }
            set
            {
                this["BILL_TO_CODE"] = value;
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
        public string PO_FILE_TYPE
        {
            get
            {
                return (string)this["PO_FILE_TYPE"];
            }
            set
            {
                this["PO_FILE_TYPE"] = value;
            }
        }
        public string PO_FILE_DESC
        {
            get
            {
                return (string)this["PO_FILE_DESC"];
            }
            set
            {
                this["PO_FILE_DESC"] = value;
            }
        }
        public string PO_FILE_ID
        {
            get
            {
                return (string)this["PO_FILE_ID"];
            }
            set
            {
                this["PO_FILE_ID"] = value;
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
    }
    public class R_CUST_PO
    {
        public string ID { get; set; }
        public string CUST_PO_NO { get; set; }
        public string C_CUST_ID { get; set; }
        public string BILL_TO_CODE { get; set; }
        public string STATUS { get; set; }
        public string PO_FILE_TYPE { get; set; }
        public string PO_FILE_DESC { get; set; }
        public string PO_FILE_ID { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
    }
}