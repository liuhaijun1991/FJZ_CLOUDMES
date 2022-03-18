using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SHIPPING_ROUTE_DETAIL : DataObjectTable
    {
        public T_C_SHIPPING_ROUTE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SHIPPING_ROUTE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SHIPPING_ROUTE_DETAIL);
            TableName = "C_SHIPPING_ROUTE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_SHIPPING_ROUTE_DETAIL : DataObjectBase
    {
        public Row_C_SHIPPING_ROUTE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_SHIPPING_ROUTE_DETAIL GetDataObject()
        {
            C_SHIPPING_ROUTE_DETAIL DataObject = new C_SHIPPING_ROUTE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.RFC_NAME = this.RFC_NAME;
            DataObject.TO_PLANT = this.TO_PLANT;
            DataObject.FROM_PLANT = this.FROM_PLANT;
            DataObject.ACTIONTYPE = this.ACTIONTYPE;
            DataObject.TO_STOCK = this.TO_STOCK;
            DataObject.FROM_STOCK = this.FROM_STOCK;
            DataObject.ACTIONNAME = this.ACTIONNAME;
            DataObject.SEQ = this.SEQ;
            DataObject.ROUTENAME = this.ROUTENAME;
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
        public string RFC_NAME
        {
            get
            {
                return (string)this["RFC_NAME"];
            }
            set
            {
                this["RFC_NAME"] = value;
            }
        }
        public string TO_PLANT
        {
            get
            {
                return (string)this["TO_PLANT"];
            }
            set
            {
                this["TO_PLANT"] = value;
            }
        }
        public string FROM_PLANT
        {
            get
            {
                return (string)this["FROM_PLANT"];
            }
            set
            {
                this["FROM_PLANT"] = value;
            }
        }
        public string ACTIONTYPE
        {
            get
            {
                return (string)this["ACTIONTYPE"];
            }
            set
            {
                this["ACTIONTYPE"] = value;
            }
        }
        public string TO_STOCK
        {
            get
            {
                return (string)this["TO_STOCK"];
            }
            set
            {
                this["TO_STOCK"] = value;
            }
        }
        public string FROM_STOCK
        {
            get
            {
                return (string)this["FROM_STOCK"];
            }
            set
            {
                this["FROM_STOCK"] = value;
            }
        }
        public string ACTIONNAME
        {
            get
            {
                return (string)this["ACTIONNAME"];
            }
            set
            {
                this["ACTIONNAME"] = value;
            }
        }
        public string SEQ
        {
            get
            {
                return (string)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
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
    }
    public class C_SHIPPING_ROUTE_DETAIL
    {
        public string ID{ get; set; }
        public string RFC_NAME{ get; set; }
        public string TO_PLANT{ get; set; }
        public string FROM_PLANT{ get; set; }
        public string ACTIONTYPE{ get; set; }
        public string TO_STOCK{ get; set; }
        public string FROM_STOCK{ get; set; }
        public string ACTIONNAME{ get; set; }
        public string SEQ{ get; set; }
        public string ROUTENAME{ get; set; }
    }
}