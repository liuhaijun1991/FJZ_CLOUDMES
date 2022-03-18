using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PO_STATUS_MAPPING : DataObjectTable
    {
        public T_C_PO_STATUS_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PO_STATUS_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PO_STATUS_MAPPING);
            TableName = "C_PO_STATUS_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_PO_STATUS_MAPPING : DataObjectBase
    {
        public Row_C_PO_STATUS_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public C_PO_STATUS_MAPPING GetDataObject()
        {
            C_PO_STATUS_MAPPING DataObject = new C_PO_STATUS_MAPPING();
            DataObject.ID = this.ID;
            DataObject.NAME = this.NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
    }
    public class C_PO_STATUS_MAPPING
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
    }
}