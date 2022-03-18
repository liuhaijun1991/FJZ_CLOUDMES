using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SKU_JNP_G : DataObjectTable
    {
        public T_R_SKU_JNP_G(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SKU_JNP_G(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SKU_JNP_G);
            TableName = "R_SKU_JNP_G".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SKU_JNP_G : DataObjectBase
    {
        public Row_R_SKU_JNP_G(DataObjectInfo info) : base(info)
        {

        }
        public R_SKU_JNP_G GetDataObject()
        {
            R_SKU_JNP_G DataObject = new R_SKU_JNP_G();
            DataObject.ID = this.ID;
            DataObject.JUNIPER = this.JUNIPER;
            DataObject.FOXCONN = this.FOXCONN;
            DataObject.G_CODE = this.G_CODE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string G_CODE
        {
            get
            {
                return (string)this["G_CODE"];
            }
            set
            {
                this["G_CODE"] = value;
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
    public class R_SKU_JNP_G
    {
        public string ID { get; set; }
        public string JUNIPER { get; set; }
        public string FOXCONN { get; set; }
        public string G_CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}