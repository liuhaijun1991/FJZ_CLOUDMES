using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_PO_STATUS_MAP_J : DataObjectTable
    {
        public T_O_PO_STATUS_MAP_J(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_PO_STATUS_MAP_J(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_PO_STATUS_MAP_J);
            TableName = "O_PO_STATUS_MAP_J".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_PO_STATUS_MAP_J : DataObjectBase
    {
        public Row_O_PO_STATUS_MAP_J(DataObjectInfo info) : base(info)
        {

        }
        public O_PO_STATUS_MAP_J GetDataObject()
        {
            O_PO_STATUS_MAP_J DataObject = new O_PO_STATUS_MAP_J();
            DataObject.ID = this.ID;
            DataObject.NAME = this.NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.LEV = this.LEV;
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
        public string LEV
        {
            get
            {
                return (string)this["LEV"];
            }
            set
            {
                this["LEV"] = value;
            }
        }
    }
    public class O_PO_STATUS_MAP_J
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string LEV { get; set; }
        public string CUST { get; set; }
    }
}