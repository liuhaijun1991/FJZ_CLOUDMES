using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_I138_REASON : DataObjectTable
    {
        public T_O_I138_REASON(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_I138_REASON(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_I138_REASON);
            TableName = "O_I138_REASON".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_I138_REASON : DataObjectBase
    {
        public Row_O_I138_REASON(DataObjectInfo info) : base(info)
        {

        }
        public O_I138_REASON GetDataObject()
        {
            O_I138_REASON DataObject = new O_I138_REASON();
            DataObject.ID = this.ID;
            DataObject.REASONCODE = this.REASONCODE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CODETYPE = this.CODETYPE;
            DataObject.DETAIL = this.DETAIL;
            DataObject.DRIVER = this.DRIVER;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string CODETYPE
        {
            get
            {
                return (string)this["CODETYPE"];
            }
            set
            {
                this["CODETYPE"] = value;
            }
        }
        public string DETAIL
        {
            get
            {
                return (string)this["DETAIL"];
            }
            set
            {
                this["DETAIL"] = value;
            }
        }
        public string DRIVER
        {
            get
            {
                return (string)this["DRIVER"];
            }
            set
            {
                this["DRIVER"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
    }
    public class O_I138_REASON
    {
        public string ID { get; set; }
        public string REASONCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string CODETYPE { get; set; }
        public string DETAIL { get; set; }
        public string DRIVER { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}