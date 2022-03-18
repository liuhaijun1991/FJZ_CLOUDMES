using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_I138_RECORD : DataObjectTable
    {
        public T_O_I138_RECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_I138_RECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_I138_RECORD);
            TableName = "O_I138_RECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_I138_RECORD : DataObjectBase
    {
        public Row_O_I138_RECORD(DataObjectInfo info) : base(info)
        {

        }
        public O_I138_RECORD GetDataObject()
        {
            O_I138_RECORD DataObject = new O_I138_RECORD();
            DataObject.ID = this.ID;
            DataObject.UPOID = this.UPOID;
            DataObject.I138ID = this.I138ID;
            DataObject.I137ID = this.I137ID;
            DataObject.I138CREATETIME = this.I138CREATETIME;
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
        public string UPOID
        {
            get
            {
                return (string)this["UPOID"];
            }
            set
            {
                this["UPOID"] = value;
            }
        }
        public string I138ID
        {
            get
            {
                return (string)this["I138ID"];
            }
            set
            {
                this["I138ID"] = value;
            }
        }
        public string I137ID
        {
            get
            {
                return (string)this["I137ID"];
            }
            set
            {
                this["I137ID"] = value;
            }
        }
        public DateTime? I138CREATETIME
        {
            get
            {
                return (DateTime?)this["I138CREATETIME"];
            }
            set
            {
                this["I138CREATETIME"] = value;
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
    public class O_I138_RECORD
    {
        public string ID { get; set; }
        public string UPOID { get; set; }
        public string I138ID { get; set; }
        public string I137ID { get; set; }
        public DateTime? I138CREATETIME { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}