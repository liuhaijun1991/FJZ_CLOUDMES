using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SKU_JNP_P : DataObjectTable
    {
        public T_R_SKU_JNP_P(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SKU_JNP_P(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SKU_JNP_P);
            TableName = "R_SKU_JNP_P".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SKU_JNP_P : DataObjectBase
    {
        public Row_R_SKU_JNP_P(DataObjectInfo info) : base(info)
        {

        }
        public R_SKU_JNP_P GetDataObject()
        {
            R_SKU_JNP_P DataObject = new R_SKU_JNP_P();
            DataObject.ID = this.ID;
            DataObject.JUNIPER = this.JUNIPER;
            DataObject.FOXCONN = this.FOXCONN;
            DataObject.PRICE = this.PRICE;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string PRICE
        {
            get
            {
                return (string)this["PRICE"];
            }
            set
            {
                this["PRICE"] = value;
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
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
    }
    public class R_SKU_JNP_P
    {
        public string ID { get; set; }
        public string JUNIPER { get; set; }
        public string FOXCONN { get; set; }
        public string PRICE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}