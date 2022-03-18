using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SHIPPING_ROUTE : DataObjectTable
    {
        public T_C_SHIPPING_ROUTE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SHIPPING_ROUTE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SHIPPING_ROUTE);
            TableName = "C_SHIPPING_ROUTE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_SHIPPING_ROUTE : DataObjectBase
    {
        public Row_C_SHIPPING_ROUTE(DataObjectInfo info) : base(info)
        {

        }
        public C_SHIPPING_ROUTE GetDataObject()
        {
            C_SHIPPING_ROUTE DataObject = new C_SHIPPING_ROUTE();
            DataObject.ID = this.ID;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
    public class C_SHIPPING_ROUTE
    {
        public string ID{ get; set; }
        public string DESCRIPTION{ get; set; }
        public string ROUTENAME{ get; set; }
    }
}