using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_DN_CUST_PO : DataObjectTable
    {
        public T_R_DN_CUST_PO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DN_CUST_PO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DN_CUST_PO);
            TableName = "R_DN_CUST_PO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_DN_CUST_PO : DataObjectBase
    {
        public Row_R_DN_CUST_PO(DataObjectInfo info) : base(info)
        {

        }
        public R_DN_CUST_PO GetDataObject()
        {
            R_DN_CUST_PO DataObject = new R_DN_CUST_PO();
            DataObject.ID = this.ID;
            DataObject.CUST_PO_NO = this.CUST_PO_NO;
            DataObject.CUST_PO_LINE_NO = this.CUST_PO_LINE_NO;
            DataObject.CUST_SKUNO = this.CUST_SKUNO;
            DataObject.PO_QTY = this.PO_QTY;
            DataObject.DN_NO = this.DN_NO;
            DataObject.DN_LINE_NO = this.DN_LINE_NO;
            DataObject.DN_SKUNO = this.DN_SKUNO;
            DataObject.DN_QTY = this.DN_QTY;
            DataObject.EXT_KEY1 = this.EXT_KEY1;
            DataObject.EXT_VALUE1 = this.EXT_VALUE1;
            DataObject.EXT_KEY2 = this.EXT_KEY2;
            DataObject.EXT_VALUE2 = this.EXT_VALUE2;
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
        public string CUST_PO_LINE_NO
        {
            get
            {
                return (string)this["CUST_PO_LINE_NO"];
            }
            set
            {
                this["CUST_PO_LINE_NO"] = value;
            }
        }
        public string CUST_SKUNO
        {
            get
            {
                return (string)this["CUST_SKUNO"];
            }
            set
            {
                this["CUST_SKUNO"] = value;
            }
        }
        public double? PO_QTY
        {
            get
            {
                return (double?)this["PO_QTY"];
            }
            set
            {
                this["PO_QTY"] = value;
            }
        }
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string DN_LINE_NO
        {
            get
            {
                return (string)this["DN_LINE_NO"];
            }
            set
            {
                this["DN_LINE_NO"] = value;
            }
        }
        public string DN_SKUNO
        {
            get
            {
                return (string)this["DN_SKUNO"];
            }
            set
            {
                this["DN_SKUNO"] = value;
            }
        }
        public double? DN_QTY
        {
            get
            {
                return (double?)this["DN_QTY"];
            }
            set
            {
                this["DN_QTY"] = value;
            }
        }
        public string EXT_KEY1
        {
            get
            {
                return (string)this["EXT_KEY1"];
            }
            set
            {
                this["EXT_KEY1"] = value;
            }
        }
        public string EXT_VALUE1
        {
            get
            {
                return (string)this["EXT_VALUE1"];
            }
            set
            {
                this["EXT_VALUE1"] = value;
            }
        }
        public string EXT_KEY2
        {
            get
            {
                return (string)this["EXT_KEY2"];
            }
            set
            {
                this["EXT_KEY2"] = value;
            }
        }
        public string EXT_VALUE2
        {
            get
            {
                return (string)this["EXT_VALUE2"];
            }
            set
            {
                this["EXT_VALUE2"] = value;
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
    public class R_DN_CUST_PO
    {
        public string ID { get; set; }
        public string CUST_PO_NO { get; set; }
        public string CUST_PO_LINE_NO { get; set; }
        public string CUST_SKUNO { get; set; }
        public double? PO_QTY { get; set; }
        public string DN_NO { get; set; }
        public string DN_LINE_NO { get; set; }
        public string DN_SKUNO { get; set; }
        public double? DN_QTY { get; set; }
        public string EXT_KEY1 { get; set; }
        public string EXT_VALUE1 { get; set; }
        public string EXT_KEY2 { get; set; }
        public string EXT_VALUE2 { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
    }
}