using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SKU_PLANT : DataObjectTable
    {
        public T_R_SKU_PLANT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SKU_PLANT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SKU_PLANT);
            TableName = "R_SKU_PLANT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SKU_PLANT : DataObjectBase
    {
        public Row_R_SKU_PLANT(DataObjectInfo info) : base(info)
        {

        }
        public R_SKU_PLANT GetDataObject()
        {
            R_SKU_PLANT DataObject = new R_SKU_PLANT();
            DataObject.ID = this.ID;
            DataObject.JUNIPER = this.JUNIPER;
            DataObject.FOXCONN = this.FOXCONN;
            DataObject.PLANTCODE = this.PLANTCODE;
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
        public string JUNIPER
        {
            get
            {
                return (string)this["JUNIPER"];
            }
            set
            {
                this["JUNIPER"] = value;
            }
        }
        public string FOXCONN
        {
            get
            {
                return (string)this["FOXCONN"];
            }
            set
            {
                this["FOXCONN"] = value;
            }
        }
        public string PLANTCODE
        {
            get
            {
                return (string)this["PLANTCODE"];
            }
            set
            {
                this["PLANTCODE"] = value;
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
    public class R_SKU_PLANT
    {
        public string ID { get; set; }
        public string JUNIPER { get; set; }
        public string FOXCONN { get; set; }
        public string PLANTCODE { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}