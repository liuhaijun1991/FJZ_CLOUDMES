using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SHIP_CUSTOMER : DataObjectTable
    {
        public T_C_SHIP_CUSTOMER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SHIP_CUSTOMER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SHIP_CUSTOMER);
            TableName = "C_SHIP_CUSTOMER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_SHIP_CUSTOMER : DataObjectBase
    {
        public Row_C_SHIP_CUSTOMER(DataObjectInfo info) : base(info)
        {

        }
        public C_SHIP_CUSTOMER GetDataObject()
        {
            C_SHIP_CUSTOMER DataObject = new C_SHIP_CUSTOMER();
            DataObject.ID = this.ID;
            DataObject.ROUTENAME = this.ROUTENAME;
            DataObject.BILLTOCODE = this.BILLTOCODE;
            DataObject.CURRENCYCODE = this.CURRENCYCODE;
            DataObject.BILLTOCOUNTRY = this.BILLTOCOUNTRY;
            DataObject.BILLTOADDR1 = this.BILLTOADDR1;
            DataObject.BILLTOLOCATION = this.BILLTOLOCATION;
            DataObject.CUSTOMERNAME = this.CUSTOMERNAME;
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
        public string ROUTENAME
        {
            get
            {
                return (string)this["ROUTENAME"];
            }
            set
            {
                this["ROUTENAME"] = value;
            }
        }
        public string BILLTOCODE
        {
            get
            {
                return (string)this["BILLTOCODE"];
            }
            set
            {
                this["BILLTOCODE"] = value;
            }
        }
        public string CURRENCYCODE
        {
            get
            {
                return (string)this["CURRENCYCODE"];
            }
            set
            {
                this["CURRENCYCODE"] = value;
            }
        }
        public string BILLTOCOUNTRY
        {
            get
            {
                return (string)this["BILLTOCOUNTRY"];
            }
            set
            {
                this["BILLTOCOUNTRY"] = value;
            }
        }
        public string BILLTOADDR1
        {
            get
            {
                return (string)this["BILLTOADDR1"];
            }
            set
            {
                this["BILLTOADDR1"] = value;
            }
        }
        public string BILLTOLOCATION
        {
            get
            {
                return (string)this["BILLTOLOCATION"];
            }
            set
            {
                this["BILLTOLOCATION"] = value;
            }
        }
        public string CUSTOMERNAME
        {
            get
            {
                return (string)this["CUSTOMERNAME"];
            }
            set
            {
                this["CUSTOMERNAME"] = value;
            }
        }
    }
    public class C_SHIP_CUSTOMER
    {
        public string ID{ get; set; }
        public string ROUTENAME{ get; set; }
        public string BILLTOCODE{ get; set; }
        public string CURRENCYCODE{ get; set; }
        public string BILLTOCOUNTRY{ get; set; }
        public string BILLTOADDR1{ get; set; }
        public string BILLTOLOCATION{ get; set; }
        public string CUSTOMERNAME{ get; set; }
    }
}