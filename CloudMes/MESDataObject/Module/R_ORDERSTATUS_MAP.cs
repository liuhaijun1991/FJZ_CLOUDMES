using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORDERSTATUS_MAP : DataObjectTable
    {
        public T_R_ORDERSTATUS_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORDERSTATUS_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORDERSTATUS_MAP);
            TableName = "R_ORDERSTATUS_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORDERSTATUS_MAP : DataObjectBase
    {
        public Row_R_ORDERSTATUS_MAP(DataObjectInfo info) : base(info)
        {

        }
        public R_ORDERSTATUS_MAP GetDataObject()
        {
            R_ORDERSTATUS_MAP DataObject = new R_ORDERSTATUS_MAP();
            DataObject.ID = this.ID;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
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
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
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
    public class R_ORDERSTATUS_MAP
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public string DESCRIPTION { get; set; }
    }
}