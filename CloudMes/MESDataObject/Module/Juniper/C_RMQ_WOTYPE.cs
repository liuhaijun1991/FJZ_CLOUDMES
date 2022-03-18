using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_C_RMQ_WOTYPE : DataObjectTable
    {
        public T_C_RMQ_WOTYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_RMQ_WOTYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_RMQ_WOTYPE);
            TableName = "C_RMQ_WOTYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_RMQ_WOTYPE : DataObjectBase
    {
        public Row_C_RMQ_WOTYPE(DataObjectInfo info) : base(info)
        {

        }
        public C_RMQ_WOTYPE GetDataObject()
        {
            C_RMQ_WOTYPE DataObject = new C_RMQ_WOTYPE();
            DataObject.ID = this.ID;
            DataObject.PLANT = this.PLANT;
            DataObject.WOTYPE = this.WOTYPE;
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
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string WOTYPE
        {
            get
            {
                return (string)this["WOTYPE"];
            }
            set
            {
                this["WOTYPE"] = value;
            }
        }
    }
    public class C_RMQ_WOTYPE
    {
        public string ID { get; set; }
        public string PLANT { get; set; }
        public string WOTYPE { get; set; }
    }
}