using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_SKU_CONFIG : DataObjectTable
    {
        public T_O_SKU_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_SKU_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_SKU_CONFIG);
            TableName = "O_SKU_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_SKU_CONFIG : DataObjectBase
    {
        public Row_O_SKU_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public O_SKU_CONFIG GetDataObject()
        {
            O_SKU_CONFIG DataObject = new O_SKU_CONFIG();
            DataObject.ID = this.ID;
            DataObject.USERITEMTYPE = this.USERITEMTYPE;
            DataObject.OFFERINGTYPE = this.OFFERINGTYPE;
            DataObject.BOMEXPLOSION = this.BOMEXPLOSION;
            DataObject.PRODUCTTYPE = this.PRODUCTTYPE;
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
        public string USERITEMTYPE
        {
            get
            {
                return (string)this["USERITEMTYPE"];
            }
            set
            {
                this["USERITEMTYPE"] = value;
            }
        }
        public string OFFERINGTYPE
        {
            get
            {
                return (string)this["OFFERINGTYPE"];
            }
            set
            {
                this["OFFERINGTYPE"] = value;
            }
        }
        public string BOMEXPLOSION
        {
            get
            {
                return (string)this["BOMEXPLOSION"];
            }
            set
            {
                this["BOMEXPLOSION"] = value;
            }
        }
        public string PRODUCTTYPE
        {
            get
            {
                return (string)this["PRODUCTTYPE"];
            }
            set
            {
                this["PRODUCTTYPE"] = value;
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
    public class O_SKU_CONFIG
    {
        public string ID { get; set; }
        public string USERITEMTYPE { get; set; }
        public string OFFERINGTYPE { get; set; }
        public string BOMEXPLOSION { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string SWOTYPE { get; set; }
        public string NSWOTYPE { get; set; }
        public string I054 { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}